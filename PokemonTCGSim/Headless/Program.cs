using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;

/// <summary>
/// Trains and measures the sim's Neural AI from the command line, without Unity (much faster: .NET uses the CPU's vector
/// instructions, Unity's runtime doesn't). See README.md.
/// </summary>
public static class Program
{
    static readonly string Root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string Resources => Path.Combine(Root, "Assets", "PokemonTCG", "Resources");
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
        try
        {
            switch (args[0])
            {
                case "train": return Train(opts);
                case "eval": return Eval(opts);
                default:
                    Console.WriteLine("Unknown command " + args[0] + " (train or eval; see README.md)");
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
                          + net.GamesTrained.ToString("N0") + " games)");
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
        var trainer = new PolicyTrainer(net, StandardCards.CreateRegistry(), decks, Int(o, "seed", Environment.TickCount & 0xFFFFF))
        {
            GamesPerRound = Int(o, "games-per-round", 128),
            Threads = Int(o, "threads", Math.Max(1, Environment.ProcessorCount - 1)),
            SequencePenalty = Dbl(o, "sequence-penalty", 0.02),
            PrizeReward = Dbl(o, "prize-reward", 0.05),
            FocusDeck = o.TryGetValue("focus-deck", out var focus) ? focus : null,
            Minibatch = Int(o, "minibatch", 1024)
        };
        net.LearningRate = Dbl(o, "lr", 2e-4);
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
                              + $" H {r.Entropy:F2} clip {r.ClipFraction:F2}" + (r.Errors > 0 ? $" | {r.Errors} errors: {r.LastError}" : "")
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
            other = new PolicyNet();
            if (!other.FromBytes(File.ReadAllBytes(vs))) throw new ArgumentException(vs + " isn't a network file.");
        }
        IPlayerAgent Opponent(int seed)
        {
            return other == null ? new HeuristicAgent(seed) : new PolicyAgent(other, seed) { Greedy = true };
        }
        double wr = trainer.Evaluate(Opponent, games, Int(o, "seed", 123), out int w, out int d, out int l);
        Console.WriteLine($"vs {Get(o, "vs", "classic bot")}: {wr:P1} ({w}-{d}-{l}) over {w + d + l} games");
        return 0;
    }
}
