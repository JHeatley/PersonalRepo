using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;

/// <summary>
/// Trains and measures the sim's Neural AI from the command line, without Unity (much faster: .NET uses the CPU's vector
/// instructions, Unity's runtime doesn't). See README.md.
/// </summary>
public static class Program
{
    /// <summary>
    /// The PokemonTCGSim folder (its Assets hold the card data), or a game build's folder (its TrainerData holds them): --root,
    /// or found upward from this program's folder.
    /// </summary>
    static string Root = FindRoot();

    static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (var d = dir; d != null; d = d.Parent)
            if (Directory.Exists(Path.Combine(d.FullName, "Assets", "PokemonTCG", "Resources", "CardData"))
                || Directory.Exists(Path.Combine(d.FullName, "TrainerData", "CardData"))) return d.FullName;
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    }

    /// <summary>
    /// The card data, decklists and starting network: the project's Resources, or in a game build (which packs Resources
    /// into Unity's own files) the TrainerData folder the build copies beside the game (Editor/TrainerBuildFiles.cs).
    /// </summary>
    static string Resources => Directory.Exists(Path.Combine(Root, "TrainerData", "CardData"))
        ? Path.Combine(Root, "TrainerData")
        : Path.Combine(Root, "Assets", "PokemonTCG", "Resources");
    static string DefaultOut => Path.Combine(Root, "Headless", "out", "policy.neural");

    public static int Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        if (args.Length == 0 || args[0] == "help" || args[0] == "--help")
        {
            Console.WriteLine(File.ReadAllText(Path.Combine(Root, "Headless", "README.md")));
            return 0;
        }
        var opts = Options(args.Skip(1).ToArray());
        if (opts.TryGetValue("root", out var root)) Root = Path.GetFullPath(root);
        if (opts.TryGetValue("skip-cpu", out var skipCpu)) Console.WriteLine(CpuThreads.Apply(CpuThreads.Parse(skipCpu)));
        try
        {
            switch (args[0])
            {
                case "train": return Train(opts);
                case "eval": return Eval(opts);
                case "background": return Background(opts);
                case "version":
                    // The app checks this before a background run (an older trainer may not read the AI's file as it is now).
                    Console.WriteLine("sim-train " + TrainerVersion.Number);
                    return 0;
                default:
                    Console.WriteLine("Unknown command " + args[0] + " (train, eval, background or version; see README.md)");
                    return 1;
            }
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }

    static Dictionary<string, string> Options(string[] a)
    {
        var d = new Dictionary<string, string>();
        for (int i = 0; i < a.Length; i++)
        {
            if (!a[i].StartsWith("--")) throw new ArgumentException("Expected an option like --games, got " + a[i]);
            string key = a[i].Substring(2);
            d[key] = i + 1 < a.Length && !a[i + 1].StartsWith("--") ? a[++i] : "true";
        }
        return d;
    }

    static string Get(Dictionary<string, string> o, string key, string fallback)
    {
        return o.TryGetValue(key, out var v) ? v : fallback;
    }

    static int Int(Dictionary<string, string> o, string key, int fallback)
    {
        return int.Parse(Get(o, key, fallback.ToString()));
    }

    static double Dbl(Dictionary<string, string> o, string key, double fallback)
    {
        return double.Parse(Get(o, key, fallback.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
    }

    // ------------------------------------------------------------------ Loading

    static readonly JsonSerializerOptions Json = new JsonSerializerOptions { IncludeFields = true };

    static CardDatabase LoadCards()
    {
        var db = new CardDatabase();
        foreach (var f in Directory.GetFiles(Path.Combine(Resources, "CardData"), "*.json").OrderBy(Path.GetFileNameWithoutExtension, StringComparer.Ordinal))
            db.AddAll(JsonSerializer.Deserialize<CardDataFile>(File.ReadAllText(f), Json));
        var printings = Path.Combine(Resources, "CardPrintings", "printings.json");
        if (File.Exists(printings)) db.AddPrintings(JsonSerializer.Deserialize<PrintingsFile>(File.ReadAllText(printings), Json));
        CustomFormats.Database = db;
        return db;
    }

    /// <summary>Every built-in decklist (Resources/Decklists), or the ones named in --decks, plus any files in --deck-folder.</summary>
    static List<KeyValuePair<string, List<CardDef>>> LoadDecks(CardDatabase db, Dictionary<string, string> o)
    {
        var files = Directory.GetFiles(Path.Combine(Resources, "Decklists"), "*.txt").ToList();
        if (o.TryGetValue("deck-folder", out var folder)) files.AddRange(Directory.GetFiles(folder, "*.txt"));
        var only = o.TryGetValue("decks", out var names) ? new HashSet<string>(names.Split(','), StringComparer.OrdinalIgnoreCase) : null;
        var list = new List<KeyValuePair<string, List<CardDef>>>();
        foreach (var f in files.OrderBy(x => x))
        {
            string name = Path.GetFileNameWithoutExtension(f);
            if (only != null && !only.Contains(name)) continue;
            var errors = new List<string>();
            var cards = Decklist.Expand(Decklist.Parse(File.ReadAllText(f), db, errors));
            foreach (var e in errors) Console.WriteLine("  " + name + ": " + e);
            if (cards.Count > 0) list.Add(new KeyValuePair<string, List<CardDef>>(name, cards));
        }
        if (list.Count == 0) throw new ArgumentException("No decks found.");
        return list;
    }

    /// <summary>--net: a .neural file (the sim's) or a Python export; default: the AI's starting network.</summary>
    static PolicyNet LoadNet(Dictionary<string, string> o, string fallback)
    {
        string path = Get(o, "net", fallback);
        if (!File.Exists(path)) path = Path.Combine(Resources, "AI", "policy_start.bytes");
        var net = new PolicyNet();
        if (!net.FromBytes(File.ReadAllBytes(path))) throw new ArgumentException(path + " isn't a network file.");
        Console.WriteLine("network: " + path + " (" + (string.IsNullOrEmpty(net.Origin) ? "fresh" : net.Origin) + ", " + net.Steps + " steps here, "
                          + net.GamesTrained.ToString("N0") + " games, " + net.Blocks + " blocks)");
        return net;
    }

    static void Save(PolicyNet net, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
        File.WriteAllBytes(path + ".tmp", net.ToBytes());
        if (File.Exists(path)) File.Delete(path);
        File.Move(path + ".tmp", path);
    }

    // ------------------------------------------------------------------ Commands

    static int Train(Dictionary<string, string> o)
    {
        string outPath = Get(o, "out", DefaultOut);
        var db = LoadCards();
        var decks = LoadDecks(db, o);
        var net = LoadNet(o, outPath);
        int blocks = Int(o, "blocks", net.Blocks);
        if (blocks > net.Blocks)
        {
            // Deeper, keeping what it knows (it plays the same until training puts the new blocks to use).
            Console.WriteLine("making the network bigger: " + net.Blocks + " -> " + blocks + " blocks");
            net.Grow(blocks);
        }
        else if (blocks < net.Blocks) Console.WriteLine("--blocks " + blocks + ": the network already has " + net.Blocks + " blocks (it can't be made smaller)");
        var trainer = new PolicyTrainer(net, StandardCards.CreateRegistry(), decks, Int(o, "seed", Environment.TickCount & 0xFFFFF))
        {
            GamesPerRound = Int(o, "games-per-round", 128),
            Threads = Int(o, "threads", Math.Max(1, Environment.ProcessorCount - 1)),
            SequencePenalty = Dbl(o, "sequence-penalty", 0.02),
            PrizeReward = Dbl(o, "prize-reward", 0.05),
            FocusDeck = o.TryGetValue("focus-deck", out var focus) ? focus : null,
            Minibatch = Int(o, "minibatch", 1024),
            SearchShare = Dbl(o, "search-share", 0),
            SearchDecisions = Dbl(o, "search-decisions", 0.3),
            GameThreads = Int(o, "game-threads", 0)
        };
        net.LearningRate = Dbl(o, "lr", 2e-4);
        if (o.TryGetValue("gpu", out var gpu))
        {
            if (!GpuLearner.TryParse(gpu == "true" ? GpuLearner.DefaultAddress : gpu, out var gpuHost, out var gpuPort))
                throw new ArgumentException("--gpu takes host:port, like " + GpuLearner.DefaultAddress);
            trainer.Gpu = new GpuLearner(gpuHost, gpuPort);
            trainer.GpuMoves = o.ContainsKey("gpu-moves");
            Console.WriteLine($"learning on the GPU learner at {gpuHost}:{gpuPort} (start ../GpuLearner/learner.py first)"
                              + (trainer.GpuMoves ? "; choosing moves there too, many games at once" : ""));
        }
        if (o.ContainsKey("games-per-second")) trainer.Pace = new GamePace(Dbl(o, "games-per-second", 0));
        int games = Int(o, "games", 10000);
        int reportEvery = Int(o, "report-every", 5000);
        int evalGames = Int(o, "eval-games", 200);
        Console.WriteLine($"training on {decks.Count} decks for {games:N0} games, {trainer.Threads} threads; saving to {outPath}");
        bool stop = false;
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            stop = true;
            Console.WriteLine("Stopping after this round (Ctrl+C again to quit at once)...");
        };
        trainer.ShouldStop = () => stop;
        int done = 0, nextReport = reportEvery;
        var clock = Stopwatch.StartNew();
        while (done < games && !stop)
        {
            trainer.GameBudget = games - done;
            var r = trainer.Round();
            done += r.Games;
            Console.WriteLine($"games {done,8:N0} | {r.Decisions / Math.Max(1, r.Games),4} decisions/game | seq mistakes/game {r.Mistakes / (double)Math.Max(1, r.Games),4:F1}"
                              + $" | vs pool/classic {(r.VsPool > 0 ? (r.PoolWins / (double)r.VsPool).ToString("F2") : " -  ")} | pi {r.PolicyLoss:+0.000;-0.000} v {r.ValueLoss:F3}"
                              + $" H {r.Entropy:F2} clip {r.ClipFraction:F2}" + (r.Searched > 0 ? $" | look-ahead {r.Searched} decisions, match {r.DistillLoss:F2}" : "") + (r.Errors > 0 || (r.LastError ?? "").StartsWith("GPU") ? $" | {r.Errors} errors: {r.LastError}" : "")
                              + (r.GpuDecisions > 0 ? $" | {r.GamesAtOnce} games at once, {r.GpuDecisions:N0} moves on the GPU" : "")
                              + (r.LearnedOn.Length > 0 ? $" | learned on {r.LearnedOn} in {r.LearnSeconds:F1}s" : $" | learned in {r.LearnSeconds:F1}s")
                              + $" | {done / clock.Elapsed.TotalSeconds:F1} games/s");
            Save(net, outPath);
            if (done >= nextReport || done >= games || stop)
            {
                while (nextReport <= done) nextReport += reportEvery;
                if (evalGames > 0)
                {
                    double wr = trainer.Evaluate(seed => new HeuristicAgent(seed), evalGames, 900000, out int w, out int d, out int l);
                    Console.WriteLine($"   REPORT {done:N0} games: vs classic bot {wr:P1} ({w}-{d}-{l})");
                }
            }
        }
        Console.WriteLine($"saved {outPath} ({net.GamesTrained:N0} games of training in all). Copy it next to your AI's file as <AI file>.neural to use it in the sim.");
        return 0;
    }

    // ------------------------------------------------------------------ Training for the app, in the background

    /// <summary>
    /// The app's "Train in the background": a training session on the AI's own brain file, as the Train screen would run it,
    /// but here (faster, and the app stays light). The app writes a job folder: job.txt (key=value lines: brain, games, measure,
    /// style, deck.N names with decks/N.txt lists, pool, a, b, randomA, randomB, threads, rest, gpu, rate, prizes, seed,
    /// lookahead, lookaheaddecisions...) and the decklists. This keeps progress.txt up to date, stops when a file named "stop" appears, and saves the brain (the
    /// network, the matchups, how well it knows each deck, the card results and the session's record) for the app to reload.
    /// </summary>
    static int Background(Dictionary<string, string> o)
    {
        string jobDir = Get(o, "job", null) ?? throw new ArgumentException("background needs --job <folder>");
        var job = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadAllLines(Path.Combine(jobDir, "job.txt")))
        {
            int eq = line.IndexOf('=');
            if (eq > 0) job[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
        }
        string J(string key, string fallback = "") => job.TryGetValue(key, out var v) ? v : fallback;
        int JI(string key, int fallback) => int.TryParse(J(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;
        double JD(string key, double fallback) => double.TryParse(J(key), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;

        string progressPath = Path.Combine(jobDir, "progress.txt");
        string stopPath = Path.Combine(jobDir, "stop");
        void Progress(TrainingStatus st, string note)
        {
            var lines = new List<string>
            {
                "pid=" + Environment.ProcessId, "phase=" + Clean(st?.Phase ?? note), "done=" + (st?.Done ?? 0), "games=" + (st?.Games ?? 0),
                "winsA=" + (st?.WinsA ?? 0), "winsB=" + (st?.WinsB ?? 0), "draws=" + (st?.Draws ?? 0), "errors=" + (st?.Errors ?? 0),
                "evalDone=" + (st?.EvalDone ?? 0), "evalTotal=" + (st?.EvalTotal ?? 0),
                "scoreBefore=" + (st?.ScoreBefore ?? -1).ToString("R", CultureInfo.InvariantCulture),
                "scoreAfter=" + (st?.ScoreAfter ?? -1).ToString("R", CultureInfo.InvariantCulture),
                "seconds=" + (st?.Seconds ?? 0).ToString("0.0", CultureInfo.InvariantCulture),
                "gamesPerSecond=" + (st?.GamesPerSecond ?? 0).ToString("0.00", CultureInfo.InvariantCulture),
                "running=" + (st != null && st.Running ? 1 : 0), "finished=" + (st != null && st.Finished ? 1 : 0),
                "cancelled=" + (st != null && st.Cancelled ? 1 : 0), "lastError=" + Clean(st?.LastError ?? ""), "note=" + Clean(note ?? "")
            };
            try
            {
                File.WriteAllLines(progressPath + ".tmp", lines);
                File.Copy(progressPath + ".tmp", progressPath, true);
            }
            catch (IOException)
            {
                // The app was reading it: the next update gets through.
            }
        }
        static string Clean(string s) => (s ?? "").Replace('\r', ' ').Replace('\n', ' ');

        try
        {
            if (File.Exists(stopPath)) File.Delete(stopPath);
            Progress(null, "Loading the cards and the AI");
            var db = LoadCards();
            var policyStart = Path.Combine(Resources, "AI", "policy_start.bytes");
            if (File.Exists(policyStart)) LearningBrain.PolicyStart = File.ReadAllBytes(policyStart);
            var decks = new List<KeyValuePair<string, List<CardDef>>>();
            var byIndex = new Dictionary<int, KeyValuePair<string, List<CardDef>>>();
            foreach (var kv in job.Where(k => k.Key.StartsWith("deck.", StringComparison.OrdinalIgnoreCase)).OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                if (!int.TryParse(kv.Key.Substring(5), out int index)) continue;
                var errors = new List<string>();
                var cards = Decklist.Expand(Decklist.Parse(File.ReadAllText(Path.Combine(jobDir, "decks", index + ".txt")), db, errors));
                if (cards.Count == 0) continue;
                byIndex[index] = new KeyValuePair<string, List<CardDef>>(kv.Value, cards);
            }
            foreach (var part in J("pool").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(part, out int i) && byIndex.TryGetValue(i, out var d)) decks.Add(d);
            var championDecks = new List<KeyValuePair<string, List<CardDef>>>();
            foreach (var part in J("champion").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (int.TryParse(part, out int i) && byIndex.TryGetValue(i, out var d)) championDecks.Add(d);
            bool randomA = J("randomA") == "1", randomB = J("randomB") == "1";
            byIndex.TryGetValue(JI("a", -1), out var deckA);
            byIndex.TryGetValue(JI("b", -1), out var deckB);
            if ((randomA || randomB) && decks.Count == 0) throw new ArgumentException("no legal decks to pick random ones from");
            if (!randomA && deckA.Value == null || !randomB && deckB.Value == null) throw new ArgumentException("a deck couldn't be read");
            if (deckA.Value == null) deckA = new KeyValuePair<string, List<CardDef>>("Random deck", decks[0].Value);
            if (deckB.Value == null) deckB = new KeyValuePair<string, List<CardDef>>("Random deck", decks[0].Value);

            var brain = LearningBrain.Load(J("brain"));
            // v171: never train (and save) over a network file this trainer can't read: it would replace it with the starting network.
            if (brain.PolicyLoadError != null) throw new InvalidDataException(brain.PolicyLoadError);
            var style = J("style", "neural").ToLowerInvariant();
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, deckA.Key, deckA.Value, deckB.Key, deckB.Value,
                JI("games", 1000), JI("measure", 0), JI("seed", Environment.TickCount & 0x0FFFFFFF) | 1)
            {
                Style = style == "learning" ? AiStyle.Learning : style == "hybrid" ? AiStyle.Hybrid : AiStyle.Neural,
                DeckPool = randomA || randomB ? decks : null,
                RandomA = randomA,
                RandomB = randomB,
                NeuralThreads = JI("threads", 0),
                NeuralRest = JD("rest", 0),
                GpuAddress = J("gpu", null),
                GpuMoves = J("gpumoves") == "1",
                GamesPerSecond = JD("rate", 0),
                PrizeCount = JI("prizes", 6),
                ChampionDecks = championDecks.Count > 0 ? championDecks : null,
                LearnFromLookAhead = JD("lookahead", 0),
                LookAheadDecisions = JD("lookaheaddecisions", 0.3),
                UseLeague = J("league", "1") != "0",
                Exploiters = J("exploiters", "1") != "0",
                ExploiterEvery = JI("exploiterevery", TrainingSession.DefaultExploiterEvery),
                ExploiterGames = JI("exploitergames", TrainingSession.DefaultExploiterGames)
            };
            Console.WriteLine($"background training: {session.MatchLabel}, {session.Games:N0} games as the {session.StyleLabel}; brain {brain.FilePath}");
            session.Start();
            DateTime? stopAsked = null;
            while (session.Running || !session.Finished)
            {
                Thread.Sleep(1000);
                if (File.Exists(stopPath))
                {
                    session.Cancel();
                    if (stopAsked == null) stopAsked = DateTime.UtcNow;
                }
                Progress(session.Status(), "");
                // v170: a stop that still hasn't finished after 3 minutes (something it waits on never answers) ends the trainer anyway,
                // so the app isn't left "Stopping" for good. What it learned since the brain was last saved is lost.
                if (stopAsked != null && DateTime.UtcNow - stopAsked.Value > TimeSpan.FromMinutes(3))
                {
                    var stuck = session.Status();
                    stuck.Running = false;
                    stuck.Finished = true;
                    stuck.Cancelled = true;
                    Progress(stuck, "Stopped without saving: the trainer didn't stop within 3 minutes");
                    Console.WriteLine("stop: the session didn't finish within 3 minutes; leaving without saving");
                    Environment.Exit(2);
                }
            }
            session.Join(60000);
            var final = session.Status();
            Progress(final, final.Cancelled ? "Stopped: what it learned is saved" : "Done: what it learned is saved");
            Console.WriteLine($"done: {final.Done:N0} games in {final.Seconds / 60:0.0} minutes" + (string.IsNullOrEmpty(final.LastError) ? "" : "; " + final.LastError));
            return 0;
        }
        catch (Exception e)
        {
            Progress(new TrainingStatus { Finished = true, LastError = e.GetType().Name + ": " + e.Message, Phase = "Couldn't train" }, "");
            Console.WriteLine(e);
            return 1;
        }
    }

    static int Eval(Dictionary<string, string> o)
    {
        var db = LoadCards();
        var decks = LoadDecks(db, o);
        var net = LoadNet(o, DefaultOut);
        int games = Int(o, "games", 400);
        var trainer = new PolicyTrainer(net, StandardCards.CreateRegistry(), decks, 1)
        {
            Threads = Int(o, "threads", Math.Max(1, Environment.ProcessorCount - 1)),
            FocusDeck = o.TryGetValue("focus-deck", out var focus) ? focus : null
        };
        PolicyNet other = null;
        if (o.TryGetValue("vs", out var vs) && vs != "classic")
        {
            if (vs == "self") other = net; // the same network on both sides (to compare look-aheads)
            else
            {
                other = new PolicyNet();
                if (!other.FromBytes(File.ReadAllBytes(vs))) throw new ArgumentException(vs + " isn't a network file.");
            }
        }
        // Look-ahead settings for one side: --search off|quick|deep [--search-seconds S] [--search-playouts N] [--plan on|off]
        // [--belief B] [--explore C] [--plan-depth D]; the opponent's are the same names after "vs-" (v172, to measure one
        // look-ahead against another).
        SearchSettings Settings(string prefix)
        {
            string level = Get(o, prefix + "search", "off");
            SearchSettings st = level == "quick" ? SearchSettings.Quick(1) : level == "deep" ? SearchSettings.Deep(1) : null;
            if (st == null && level != "off") throw new ArgumentException("--" + prefix + "search takes off, quick or deep");
            if (st == null) return null;
            st.MaxSeconds = Dbl(o, prefix + "search-seconds", st.MaxSeconds);
            st.Playouts = Int(o, prefix + "search-playouts", st.Playouts);
            string plan = Get(o, prefix + "plan", st.PlanTurn ? "on" : "off");
            if (plan != "on" && plan != "off") throw new ArgumentException("--" + prefix + "plan takes on or off");
            st.PlanTurn = plan == "on";
            st.DeckBelief = Dbl(o, prefix + "belief", st.DeckBelief);
            st.Explore = Dbl(o, prefix + "explore", st.Explore);
            st.PlanDepth = Int(o, prefix + "plan-depth", st.PlanDepth);
            return st;
        }
        string Describe(string prefix, SearchSettings st)
        {
            return st == null ? "" : " (look-ahead " + Get(o, prefix + "search", "off") + (st.PlanTurn ? ", planning the turn" : "") + ")";
        }
        var search = Settings("");
        var vsSearch = Settings("vs-");
        if (vsSearch != null && other == null) throw new ArgumentException("--vs-search needs --vs self or a network file");
        PolicySearch opponentSearch = null;
        if (vsSearch != null)
        {
            opponentSearch = new PolicySearch(other, vsSearch) { PrizeReward = trainer.PrizeReward };
            foreach (var deck in decks) opponentSearch.KnownDecks.Add(deck.Value);
        }
        IPlayerAgent Opponent(int seed)
        {
            return other == null ? new HeuristicAgent(seed) : new PolicyAgent(other, seed) { Greedy = true, Search = opponentSearch };
        }
        var clock = System.Diagnostics.Stopwatch.StartNew();
        double wr = trainer.Evaluate(Opponent, games, Int(o, "seed", 123), search, out int w, out int d, out int l);
        Console.WriteLine($"{Describe("", search).Trim()} vs {Get(o, "vs", "classic bot")}{Describe("vs-", vsSearch)}: {wr:P1} ({w}-{d}-{l}) over {w + d + l} games"
                          + $" in {clock.Elapsed.TotalSeconds:0}s");
        var s = trainer.LastSearch;
        if (s != null)
            Console.WriteLine($"look-ahead: {s.Searches:N0} decisions looked at, {s.Changed:N0} moves changed from the network's own, "
                              + $"{s.Playouts:N0} play-outs ({s.Failed:N0} guesses failed)");
        if (opponentSearch != null)
            Console.WriteLine($"opponent's look-ahead: {opponentSearch.Searches:N0} decisions looked at, {opponentSearch.Changed:N0} moves changed, "
                              + $"{opponentSearch.Playouts:N0} play-outs ({opponentSearch.Failed:N0} guesses failed)");
        return 0;
    }
}
