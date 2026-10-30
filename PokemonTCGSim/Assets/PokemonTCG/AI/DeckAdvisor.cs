using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One suggested change: take out Count copies of Cut and put in Count copies of Add (either may be null).</summary>
    public sealed class DeckChange
    {
        public CardDef Add;
        public CardDef Cut;
        public int Count = 1;
        /// <summary>Why the card goes in.</summary>
        public string Why = "";
        /// <summary>Why the other card is the one to go.</summary>
        public string CutWhy = "";

        public string Label
        {
            get
            {
                var sb = new StringBuilder();
                if (Add != null) sb.Append("+").Append(Count).Append(' ').Append(Add.Name);
                if (Cut != null)
                {
                    if (sb.Length > 0) sb.Append("   ");
                    sb.Append("−").Append(Count).Append(' ').Append(Cut.Name);
                }
                return sb.ToString();
            }
        }
    }

    /// <summary>What the deck advisor came up with: a few lines about the deck, the changes, and the deck with them made.</summary>
    public sealed class DeckAdvice
    {
        public string Title = "";
        public readonly List<string> Notes = new List<string>();
        public readonly List<DeckChange> Changes = new List<DeckChange>();
        /// <summary>The deck with every change made (or the deck that was built).</summary>
        public List<CardDef> Result = new List<CardDef>();
        /// <summary>A name for a deck that was built.</summary>
        public string DeckName = "";
        public DeckFormat Format;
        /// <summary>A whole deck was built (Load it), rather than changes to the deck in the editor.</summary>
        public bool IsWholeDeck;
        /// <summary>Set when nothing could be suggested or built, with the reason.</summary>
        public string Error;
    }

    /// <summary>A deck the advisor learns from: a built-in list or one of your saved decks.</summary>
    public sealed class AdvisorDeck
    {
        public string Name = "";
        public List<CardDef> Cards = new List<CardDef>();

        public AdvisorDeck()
        {
        }

        public AdvisorDeck(string name, List<CardDef> cards)
        {
            Name = name ?? "";
            Cards = cards ?? new List<CardDef>();
        }
    }

    /// <summary>
    /// The deck advisor (v75). It reads three kinds of data:
    ///  - the cards' own text (what each card does: draw, Pokémon search, gust, switch, recovery, Energy costs, evolution lines),
    ///  - the decks in your library (built-in lists and your saved decks): what similar decks play and how many copies, and which
    ///    cards most decks in the format play,
    ///  - the learning AI's brain: what it has learned about playing each card (LearningBrain.CardOpinions) and how decks did.
    ///
    /// It does three things:
    ///  - SuggestChanges: swaps for the deck you're editing (fix what's missing, cut what does least), each with its reasons;
    ///  - BuildAround: a whole deck around a card (the consensus of the library decks that play it, or a template built from
    ///    its evolution line, the format's most played Trainers and the Energy its attacks need);
    ///  - Counter: swaps that help against one opponent's deck (a Pokémon that hits its main attacker for Weakness, answers to
    ///    what it relies on: Abilities, Tools, Special Energy, Items, Stadiums).
    ///
    /// The changes are suggestions with reasons, not rules: DeckTrial plays games before and after to check them.
    /// </summary>
    public sealed class DeckAdvisor
    {
        /// <summary>A swap is suggested when what goes in is worth at least this much more than what goes out.</summary>
        public const double Margin = 1.5;
        private const double BaseCut = 3.0;
        /// <summary>Two decks this alike (cards in common over cards in either, by copies) count as similar.</summary>
        public const double SimilarAt = 0.4;

        private readonly DeckFormat format;
        private readonly LearningBrain brain;
        private readonly List<LibraryDeck> library = new List<LibraryDeck>();
        /// <summary>Card id (see Id) -> a printing of it that this format allows.</summary>
        private readonly Dictionary<string, CardDef> pool = new Dictionary<string, CardDef>(StringComparer.Ordinal);
        private readonly Dictionary<string, List<CardDef>> poolByName = new Dictionary<string, List<CardDef>>(StringComparer.Ordinal);
        /// <summary>Every card in the database by name, any format (to look up evolution lines).</summary>
        private readonly Dictionary<string, List<CardDef>> allByName = new Dictionary<string, List<CardDef>>(StringComparer.Ordinal);
        /// <summary>Card id -> how many library decks play it, and how many copies in all.</summary>
        private readonly Dictionary<string, int> libraryDecks = new Dictionary<string, int>(StringComparer.Ordinal);
        private readonly Dictionary<string, int> libraryCopies = new Dictionary<string, int>(StringComparer.Ordinal);
        private Dictionary<string, CardOpinion> opinions = new Dictionary<string, CardOpinion>(StringComparer.Ordinal);

        private sealed class LibraryDeck
        {
            public string Name;
            public Profile Profile;
        }

        private sealed class Similar
        {
            /// <summary>How much the deck counts: its likeness, times how well the AI does with it (Strength).</summary>
            public double Weight;
            public double Likeness;
            /// <summary>The Neural AI's win rate with the deck in training, or -1 when it hasn't played it enough.</summary>
            public double AiWinRate = -1;
            public Profile Profile;
            public string Name;
        }

        private sealed class Need
        {
            public CardDef Card;
            public double Priority;
            public string Why;
        }

        private sealed class CutOption
        {
            public string Id;
            public CardDef Card;
            public double Value;
            public readonly List<string> Why = new List<string>();
        }

        private sealed class Family
        {
            public readonly HashSet<string> Names = new HashSet<string>(StringComparer.Ordinal);
            public int Copies;
            public double Score;
        }

        public DeckAdvisor(CardDatabase db, DeckFormat format, IEnumerable<AdvisorDeck> decks, LearningBrain brain)
        {
            this.format = format;
            this.brain = brain;
            foreach (var d in db.Cards)
            {
                List<CardDef> all;
                if (!allByName.TryGetValue(d.Name, out all))
                {
                    all = new List<CardDef>();
                    allByName[d.Name] = all;
                }
                all.Add(d);
                if (!Formats.Allows(format, d)) continue;
                string id = Id(d);
                if (pool.ContainsKey(id)) continue;
                pool[id] = d;
                List<CardDef> named;
                if (!poolByName.TryGetValue(d.Name, out named))
                {
                    named = new List<CardDef>();
                    poolByName[d.Name] = named;
                }
                named.Add(d);
            }
            if (decks != null)
            {
                foreach (var deck in decks)
                {
                    if (deck == null || deck.Cards == null || deck.Cards.Count == 0) continue;
                    if (deck.Cards.Any(x => x == null || !Formats.Allows(format, x))) continue;
                    var p = new Profile(deck.Cards);
                    library.Add(new LibraryDeck { Name = deck.Name, Profile = p });
                    foreach (var kv in p.Copies)
                    {
                        libraryDecks[kv.Key] = Get(libraryDecks, kv.Key) + 1;
                        libraryCopies[kv.Key] = Get(libraryCopies, kv.Key) + kv.Value;
                    }
                }
            }
        }

        /// <summary>Library decks the advice can learn from (the ones legal in this format).</summary>
        public int LibrarySize { get { return library.Count; } }

        // ------------------------------------------------------------------ Card helpers

        /// <summary>What counts as "the same card": a Pokémon by its printing (two Pokémon with one name can differ), anything else by name.</summary>
        public static string Id(CardDef d)
        {
            return d.IsPokemon ? "P:" + d.BaseKey : "N:" + d.Name;
        }

        /// <summary>Rough damage of an attack: printed damage, more for "+" and "×", a guess for damage counters and copied attacks.</summary>
        public static int AttackValue(AttackDef a)
        {
            if (a == null) return 0;
            int dmg = a.Damage;
            string suffix = a.DamageSuffix ?? "";
            string text = a.Text ?? "";
            if ((suffix == "×" || suffix == "x") && dmg > 0) return dmg * 3;
            if (suffix == "+" && dmg > 0) return dmg * 3 / 2;
            if (dmg == 0 && text.IndexOf("damage counter", StringComparison.OrdinalIgnoreCase) >= 0)
                return text.IndexOf("for each", StringComparison.OrdinalIgnoreCase) >= 0 ? 180 : 100;
            if (CopiesAttack.IsMatch(text)) return Math.Max(dmg, 150);
            return dmg;
        }

        public static int BestDamage(CardDef d)
        {
            int best = 0;
            if (d == null) return 0;
            foreach (var a in d.Attacks) best = Math.Max(best, AttackValue(a));
            return best;
        }

        /// <summary>The attack with the most damage (the one a deck plays the Pokémon for), or null.</summary>
        public static AttackDef MainAttack(CardDef d)
        {
            AttackDef best = null;
            int value = -1;
            if (d == null) return null;
            foreach (var a in d.Attacks)
            {
                int v = AttackValue(a);
                if (v > value)
                {
                    value = v;
                    best = a;
                }
            }
            return best;
        }

        /// <summary>Chance of no Basic Pokémon in a 7-card opening hand.</summary>
        public static double MulliganChance(int basics, int deckSize)
        {
            if (basics <= 0) return 1.0;
            if (deckSize - basics < 7) return 0.0;
            double p = 1.0;
            for (int i = 0; i < 7; i++) p *= (double)(deckSize - basics - i) / (deckSize - i);
            return p;
        }

        private static int StageRank(CardDef d)
        {
            return d.Stage == Stage.Stage2 ? 2 : d.Stage == Stage.Stage1 ? 1 : 0;
        }

        private static int Get(Dictionary<string, int> map, string key)
        {
            int n;
            return map.TryGetValue(key, out n) ? n : 0;
        }

        private static int CountWhere(IEnumerable<CardDef> cards, Func<CardDef, bool> test)
        {
            int n = 0;
            foreach (var d in cards) if (test(d)) n++;
            return n;
        }

        private static string Pct(double x)
        {
            return Math.Round(x * 100).ToString(CultureInfo.InvariantCulture) + "%";
        }

        private static string Num(double x)
        {
            return x.ToString("0.#", CultureInfo.InvariantCulture);
        }

        private static readonly Regex CopiesAttack = new Regex(@"\bas this attack\b", RegexOptions.IgnoreCase);
        private static readonly Regex TypeWord = new Regex(@"(?<!non-)\b(Grass|Fire|Water|Lightning|Psychic|Fighting|Darkness|Metal|Dragon|Fairy)\b");
        private static readonly Regex TypeSymbol = new Regex(@"\{([GRWLPFDMNY])\}");
        private static readonly Regex AncientWord = new Regex(@"\bAncient\b");
        private static readonly Regex FutureWord = new Regex(@"\bFuture\b");
        private static readonly Regex WallText = new Regex(@"(prevent all damage|no damage|isn't affected)[^.]*(Pokémon ex|Rule Box|Pokémon V\b)", RegexOptions.IgnoreCase);
        private static readonly Regex NoWeakness = new Regex(@"has no Weakness", RegexOptions.IgnoreCase);

        // ------------------------------------------------------------------ What a deck is

        /// <summary>A deck taken apart: copies, roles, evolution families, main attacker, Energy needs.</summary>
        private sealed class Profile
        {
            public readonly List<CardDef> Cards;
            public readonly Dictionary<string, int> Copies = new Dictionary<string, int>(StringComparer.Ordinal);
            public readonly Dictionary<string, CardDef> Printing = new Dictionary<string, CardDef>(StringComparer.Ordinal);
            public readonly Dictionary<string, int> Names = new Dictionary<string, int>(StringComparer.Ordinal);
            /// <summary>Copies of each name that count toward the copy limit (not basic Energy: v138's Special Darkness Energy).</summary>
            public readonly Dictionary<string, int> LimitedNames = new Dictionary<string, int>(StringComparer.Ordinal);
            public readonly Dictionary<CardRole, double> Roles = new Dictionary<CardRole, double>();
            public int Basics, Pokemon, Trainers, Energy, Supporters, Items, Tools, Stadiums, AceSpecs, Radiants;
            public readonly List<Family> Families = new List<Family>();
            public readonly HashSet<string> KeyNames = new HashSet<string>(StringComparer.Ordinal);
            public CardDef Main;
            public double MainScore;
            /// <summary>Energy symbols in the attack costs of the key Pokémon, and of the main attacker's line.</summary>
            public readonly Dictionary<EnergyType, double> Demand = new Dictionary<EnergyType, double>();
            public readonly Dictionary<EnergyType, double> MainDemand = new Dictionary<EnergyType, double>();
            /// <summary>Types something in the deck uses: any attack cost, or a type named in any text.</summary>
            public readonly HashSet<EnergyType> Uses = new HashSet<EnergyType>();
            public readonly Dictionary<EnergyType, int> BasicEnergy = new Dictionary<EnergyType, int>();
            public int SpecialEnergy;
            /// <summary>Pokémon types and Basic Energy types in the deck.</summary>
            public readonly HashSet<EnergyType> Types = new HashSet<EnergyType>();
            public int MainCost;
            public bool MainTyped;
            public bool DifferentTypes;
            /// <summary>Something in the deck uses other Pokémon's attacks (Slowking's Seek Inspiration): evolutions may be there to be copied.</summary>
            public bool CopiesAttacks;
            /// <summary>Draw cards plus half the Supporters that find Trainers.</summary>
            public double DrawLike;
            public int AbilityPokemon;
            /// <summary>Usual Energy count of similar decks, when there are some (set by Needs).</summary>
            public int EnergyLow = -1, EnergyHigh = -1;

            public int Count(string id)
            {
                int n;
                return Copies.TryGetValue(id, out n) ? n : 0;
            }

            public int NameCount(string name)
            {
                int n;
                return name != null && Names.TryGetValue(name, out n) ? n : 0;
            }

            public int LimitedCount(string name)
            {
                int n;
                return name != null && LimitedNames.TryGetValue(name, out n) ? n : 0;
            }

            public double Role(CardRole r)
            {
                double n;
                return Roles.TryGetValue(r, out n) ? n : 0;
            }

            /// <summary>Copies of a card the way the 4-copy rule counts them (by name), or of that exact Pokémon.</summary>
            public int Have(CardDef d)
            {
                return d.IsPokemon ? Count(Id(d)) : NameCount(d.Name);
            }

            public double Search { get { return Role(CardRole.PokemonSearch) + Role(CardRole.Tutor); } }

            public Profile(IList<CardDef> cards)
            {
                Cards = new List<CardDef>();
                foreach (var d in cards) if (d != null) Cards.Add(d);
                foreach (var d in Cards)
                {
                    string id = Id(d);
                    Copies[id] = Count(id) + 1;
                    if (!Printing.ContainsKey(id)) Printing[id] = d;
                    Names[d.Name] = NameCount(d.Name) + 1;
                    if (!(d.IsEnergy && d.IsBasicEnergy)) LimitedNames[d.Name] = LimitedCount(d.Name) + 1;
                    var roles = CardRoles.Of(d);
                    foreach (var r in CardRoles.All) if ((roles & r) != 0) Roles[r] = Role(r) + 1;
                    if (d.IsBasicPokemon) Basics++;
                    if (d.IsPokemon)
                    {
                        Pokemon++;
                        Types.Add(d.PokemonType);
                        if (d.HasAbility) AbilityPokemon++;
                        if (d.IsRadiant) Radiants++;
                    }
                    if (d.IsTrainer) Trainers++;
                    if (d.IsSupporter) Supporters++;
                    if (d.IsItem) Items++;
                    if (d.IsTool) Tools++;
                    if (d.IsStadium) Stadiums++;
                    if (d.IsAceSpec) AceSpecs++;
                    if (d.IsEnergy)
                    {
                        Energy++;
                        if (d.IsBasicEnergy)
                        {
                            int have;
                            BasicEnergy.TryGetValue(d.EnergyProvides, out have);
                            BasicEnergy[d.EnergyProvides] = have + 1;
                            Types.Add(d.EnergyProvides);
                        }
                        else SpecialEnergy++;
                    }
                    if (d.IsSupporter && (roles & CardRole.TrainerSearch) != 0 && (roles & CardRole.Draw) == 0) DrawLike += 0.5;
                }
                DrawLike += Role(CardRole.Draw);
                FindFamilies();
                FindEnergyNeeds();
            }

            private static string Find(Dictionary<string, string> parent, string x)
            {
                string p;
                while (parent.TryGetValue(x, out p) && p != x) x = p;
                return x;
            }

            private void FindFamilies()
            {
                var parent = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var d in Cards)
                {
                    if (!d.IsPokemon) continue;
                    if (!parent.ContainsKey(d.Name)) parent[d.Name] = d.Name;
                    if (string.IsNullOrEmpty(d.EvolvesFrom)) continue;
                    if (!parent.ContainsKey(d.EvolvesFrom)) parent[d.EvolvesFrom] = d.EvolvesFrom;
                    string a = Find(parent, d.Name), b = Find(parent, d.EvolvesFrom);
                    if (a != b) parent[a] = b;
                }
                var groups = new Dictionary<string, Family>(StringComparer.Ordinal);
                foreach (var name in parent.Keys.ToList())
                {
                    string root = Find(parent, name);
                    Family f;
                    if (!groups.TryGetValue(root, out f))
                    {
                        f = new Family();
                        groups[root] = f;
                    }
                    f.Names.Add(name);
                }
                foreach (var f in groups.Values)
                {
                    foreach (var name in f.Names) f.Copies += NameCount(name);
                    if (f.Copies > 0) Families.Add(f);
                }

                // The main attacker: most copies times damage (a Pokémon with a Rule Box a little more).
                var score = new Dictionary<string, double>(StringComparer.Ordinal);
                string top = null;
                foreach (var kv in Copies)
                {
                    var d = Printing[kv.Key];
                    if (!d.IsPokemon) continue;
                    int dmg = BestDamage(d);
                    if (dmg <= 0) continue;
                    double s = NameCount(d.Name) * dmg * (d.HasRuleBox ? 1.2 : 1.0);
                    score[kv.Key] = s;
                    if (top == null || s > MainScore || (s == MainScore && StageRank(d) > StageRank(Printing[top])))
                    {
                        top = kv.Key;
                        MainScore = s;
                    }
                }
                if (top != null)
                {
                    Main = Printing[top];
                    foreach (var f in Families)
                    {
                        foreach (var kv in score)
                            if (f.Names.Contains(Printing[kv.Key].Name)) f.Score = Math.Max(f.Score, kv.Value);
                    }
                    string mainName = Main.Name;
                    Families.Sort((x, y) =>
                    {
                        int byMain = (y.Names.Contains(mainName) ? 1 : 0).CompareTo(x.Names.Contains(mainName) ? 1 : 0);
                        return byMain != 0 ? byMain : y.Score.CompareTo(x.Score);
                    });
                    foreach (var name in Families[0].Names) KeyNames.Add(name);
                    for (int i = 1; i < Families.Count; i++)
                        if (Families[i].Score >= 0.6 * MainScore) foreach (var name in Families[i].Names) KeyNames.Add(name);
                    var attack = MainAttack(Main);
                    if (attack != null)
                    {
                        MainCost = attack.Cost.Length;
                        foreach (var t in attack.Cost) if (t != EnergyType.Colorless) MainTyped = true;
                    }
                }
                else
                {
                    Families.Sort((x, y) => y.Copies.CompareTo(x.Copies));
                    if (Families.Count > 0) foreach (var name in Families[0].Names) KeyNames.Add(name);
                }
            }

            private void FindEnergyNeeds()
            {
                var mainNames = Main != null && Families.Count > 0 ? Families[0].Names : null;
                foreach (var d in Cards)
                {
                    if (!d.IsPokemon) continue;
                    int withCost = 0;
                    foreach (var a in d.Attacks) if (a.Cost != null && a.Cost.Length > 0) withCost++;
                    if (withCost == 0) continue;
                    bool key = KeyNames.Contains(d.Name);
                    bool main = mainNames != null && mainNames.Contains(d.Name);
                    foreach (var a in d.Attacks)
                    {
                        if (a.Cost == null) continue;
                        foreach (var t in a.Cost)
                        {
                            if (t == EnergyType.Colorless) continue;
                            Uses.Add(t);
                            if (key) AddTo(Demand, t, 1.0 / withCost);
                            if (main) AddTo(MainDemand, t, 1.0 / withCost);
                        }
                    }
                }
                foreach (var d in Printing.Values)
                {
                    ReadTypes(d.Text);
                    foreach (var ab in d.Abilities) ReadTypes(ab.Text);
                    foreach (var a in d.Attacks)
                    {
                        ReadTypes(a.Text);
                        if (CopiesAttack.IsMatch(a.Text ?? "")) CopiesAttacks = true;
                    }
                    if ((d.Text ?? "").IndexOf("different types", StringComparison.OrdinalIgnoreCase) >= 0) DifferentTypes = true;
                }
            }

            private void ReadTypes(string text)
            {
                if (string.IsNullOrEmpty(text)) return;
                foreach (Match m in TypeWord.Matches(text))
                {
                    EnergyType t;
                    if (Enum.TryParse(m.Groups[1].Value, out t)) Uses.Add(t);
                }
                foreach (Match m in TypeSymbol.Matches(text)) Uses.Add(EnergyTypes.FromSymbol(m.Groups[1].Value[0]));
            }

            private static void AddTo(Dictionary<EnergyType, double> map, EnergyType t, double amount)
            {
                double have;
                map.TryGetValue(t, out have);
                map[t] = have + amount;
            }

            /// <summary>The type the deck's main attacks need most, or null.</summary>
            public EnergyType? MainType()
            {
                EnergyType? best = null;
                double most = 0;
                foreach (var kv in Demand)
                {
                    if (kv.Value > most)
                    {
                        most = kv.Value;
                        best = kv.Key;
                    }
                }
                return best;
            }

            public List<string> FamilyNamesInDeck(Family f)
            {
                var list = new List<string>();
                foreach (var name in f.Names) if (NameCount(name) > 0) list.Add(name);
                return list;
            }
        }

        // ------------------------------------------------------------------ Library

        private List<Similar> SimilarDecks(Profile prof)
        {
            var list = new List<Similar>();
            foreach (var lib in library)
            {
                double sim = Likeness(prof, lib.Profile);
                if (sim < SimilarAt || sim >= 0.999) continue;
                double winRate;
                double strength = Strength(lib.Name, out winRate);
                list.Add(new Similar { Weight = sim * strength, Likeness = sim, AiWinRate = winRate, Profile = lib.Profile, Name = lib.Name });
            }
            list.Sort((x, y) => y.Weight.CompareTo(x.Weight));
            return list;
        }

        /// <summary>Games the AI needs with a library deck before how it does with it changes how much the deck counts.</summary>
        public const int StrengthGames = 200;

        /// <summary>
        /// How much a library deck counts as an example, from how the AI does with it in training: 0.5 (it never wins) to 1.5 (it
        /// always wins), 1 until it has played the deck enough. As the AI masters the archetypes, the lists that win count more.
        /// </summary>
        private double Strength(string deckName, out double winRate)
        {
            winRate = -1;
            if (brain == null) return 1.0;
            var skill = brain.SkillOf(deckName);
            if (skill.Games < StrengthGames) return 1.0;
            winRate = skill.WinRate;
            return 0.5 + winRate;
        }

        /// <summary>Cards in common over cards in either, by copies (1 = the same list).</summary>
        private static double Likeness(Profile a, Profile b)
        {
            double common = 0, either = 0;
            foreach (var kv in a.Copies)
            {
                int other = b.Count(kv.Key);
                common += Math.Min(kv.Value, other);
                either += Math.Max(kv.Value, other);
            }
            foreach (var kv in b.Copies)
                if (!a.Copies.ContainsKey(kv.Key)) either += kv.Value;
            return either <= 0 ? 0 : common / either;
        }

        /// <summary>How much of the similar decks (by likeness) play the card, smoothed so one deck isn't "all of them"; avg = their usual copies.</summary>
        private static double ShareIn(List<Similar> similar, string id, out double avg)
        {
            double total = 0, with = 0, copies = 0;
            int n = 0;
            foreach (var s in similar)
            {
                total += s.Weight;
                int c = s.Profile.Count(id);
                if (c <= 0) continue;
                with += s.Weight;
                copies += c;
                n++;
            }
            avg = n == 0 ? 0 : copies / n;
            return similar.Count == 0 ? 0 : with / (total + 0.5);
        }

        private static int PlayedIn(List<Similar> similar, string id)
        {
            int n = 0;
            foreach (var s in similar) if (s.Profile.Count(id) > 0) n++;
            return n;
        }

        private double LibraryShare(string id)
        {
            return library.Count == 0 ? 0 : (double)Get(libraryDecks, id) / library.Count;
        }

        private CardDef PrintingFor(string id, List<Similar> similar)
        {
            foreach (var s in similar)
            {
                CardDef d;
                if (s.Profile.Printing.TryGetValue(id, out d)) return d;
            }
            CardDef p;
            return pool.TryGetValue(id, out p) ? p : null;
        }

        private double Opinion(CardDef d, out CardOpinion op)
        {
            op = null;
            if (d == null || opinions == null || !opinions.TryGetValue(d.Name, out op) || op.Updates <= 0) return 0;
            double v = Math.Max(-1.0, Math.Min(1.0, op.Value));
            return v * op.Updates / (op.Updates + 100.0);
        }

        // ------------------------------------------------------------------ Fitting cards

        private bool Fits(CardDef d, Profile prof)
        {
            if (d == null) return false;
            string t = d.Text ?? "";
            if (d.IsTrainer)
            {
                if (t.Contains("70 HP or less") && CountWhere(prof.Cards, x => x.IsBasicPokemon && x.Hp <= 70) < 3) return false;
                if (t.Contains("without a Rule Box") && CountWhere(prof.Cards, x => x.IsPokemon && !x.HasRuleBox) < 4) return false;
                if (t.Contains("Stage 2") && CountWhere(prof.Cards, x => x.IsPokemon && x.Stage == Stage.Stage2) == 0) return false;
                if (t.Contains("Team Rocket") && CountWhere(prof.Cards, x => x.Name.StartsWith("Team Rocket's", StringComparison.Ordinal)) < 4) return false;
                if (t.Contains("different types") && prof.BasicEnergy.Count < 2) return false;
                foreach (Match m in TypeWord.Matches(t))
                {
                    EnergyType et;
                    if (Enum.TryParse(m.Groups[1].Value, out et) && !prof.Types.Contains(et)) return false;
                }
                if (AncientWord.IsMatch(t) && CountWhere(prof.Cards, x => x.IsAncient) < 3) return false;
                if (FutureWord.IsMatch(t) && CountWhere(prof.Cards, x => x.IsFuture) < 3) return false;
            }
            if (d.IsPokemon && CardRoles.Of(d) == CardRole.None)
            {
                var a = MainAttack(d);
                if (a == null) return false;
                foreach (var et in a.Cost)
                    if (et != EnergyType.Colorless && !prof.BasicEnergy.ContainsKey(et) && prof.SpecialEnergy == 0) return false;
            }
            if (d.IsEnergy && d.IsBasicEnergy) return prof.Uses.Contains(d.EnergyProvides);
            return true;
        }

        private static readonly string[] Professors = { "Professor's Research", "Professor Sycamore", "Professor Juniper" };
        private static readonly string[] Gusts = { "Lysandre", "Boss's Orders" };

        /// <summary>One more copy would still be legal (4 copies, 1 ACE SPEC, 1 Radiant, the format).</summary>
        private bool CanAdd(CardDef d, Profile prof)
        {
            if (d == null || !Formats.Allows(format, d)) return false;
            if (!(d.IsEnergy && d.IsBasicEnergy) && !d.AnyNumberInDeck && prof.LimitedCount(d.Name) >= 4) return false;
            if (d.IsAceSpec && prof.AceSpecs >= 1) return false;
            if (d.IsPokemon && d.IsRadiant && prof.Radiants >= 1) return false;
            if (d.IsPrismStar && prof.NameCount(d.Name) >= 1) return false; // Prism Star: 1 of each name (v80)
            if (d.OnePerDeck && prof.NameCount(d.Name) >= 1) return false; // "You can't have more than 1" (v142)
            if (Decklist.UnownRuleBlocks(prof.Cards, d)) return false; // Neo Discovery's Unown: 4 Unown in all (v139)
            // Professor's Research, Professor Sycamore and Professor Juniper can't share a deck (v109).
            if (Professors.Contains(d.Name) && Professors.Any(n => n != d.Name && prof.NameCount(n) > 0)) return false;
            // Lysandre and Boss's Orders can't share a deck (v134).
            if (Gusts.Contains(d.Name) && Gusts.Any(n => n != d.Name && prof.NameCount(n) > 0)) return false;
            return true;
        }

        /// <summary>A Stage 2 with no Stage 1 is fine when the deck plays Rare Candy and the Basic it starts from.</summary>
        private bool CandyLine(Profile prof, CardDef d)
        {
            if (d.Stage != Stage.Stage2 || prof.Role(CardRole.RareCandy) <= 0) return false;
            List<CardDef> stage1;
            if (!allByName.TryGetValue(d.EvolvesFrom ?? "", out stage1)) return false;
            foreach (var s1 in stage1)
                if (!string.IsNullOrEmpty(s1.EvolvesFrom) && prof.NameCount(s1.EvolvesFrom) > 0) return true;
            return false;
        }

        private CardDef BestNamed(string name)
        {
            List<CardDef> list;
            if (name == null || !poolByName.TryGetValue(name, out list) || list.Count == 0) return null;
            CardDef best = null;
            double bestScore = double.MinValue;
            foreach (var d in list)
            {
                double s = Get(libraryDecks, Id(d)) * 1000.0 + RoleCount(CardRoles.Of(d)) * 100.0 + d.Hp + BestDamage(d) / 10.0;
                if (s > bestScore)
                {
                    bestScore = s;
                    best = d;
                }
            }
            return best;
        }

        private CardDef BasicEnergyOf(EnergyType t)
        {
            CardDef best = null;
            int bestUse = -1;
            foreach (var d in pool.Values)
            {
                if (!d.IsEnergy || !d.IsBasicEnergy || d.EnergyProvides != t) continue;
                int use = Get(libraryDecks, Id(d));
                if (use > bestUse)
                {
                    bestUse = use;
                    best = d;
                }
            }
            return best;
        }

        private static int RoleCount(CardRole r)
        {
            int n = 0;
            foreach (var x in CardRoles.All) if ((r & x) != 0) n++;
            return n;
        }

        /// <summary>The best card for a role that this deck can still add: played by similar decks and the library, liked by the AI.</summary>
        private CardDef BestForRole(CardRole role, Profile prof, List<Similar> similar)
        {
            CardDef best = null;
            double bestScore = double.MinValue;
            foreach (var kv in pool)
            {
                var d = kv.Value;
                var roles = CardRoles.Of(d);
                bool match = (roles & role) != 0 || (role == CardRole.PokemonSearch && (roles & CardRole.Tutor) != 0);
                if (!match || d.IsPokemon || d.IsAceSpec) continue;
                if (!CanAdd(d, prof) || !Fits(d, prof)) continue;
                double avg;
                double q = 3.0 * LibraryShare(kv.Key) + 4.0 * ShareIn(similar, kv.Key, out avg);
                if (prof.Have(d) > 0) q += 0.5;
                if (d.IsItem && role != CardRole.Draw) q += 0.3;
                if ((d.Text ?? "").Contains("Flip a coin")) q -= 0.3;
                CardOpinion op;
                q += 0.5 * Opinion(d, out op);
                if (q > bestScore || (q == bestScore && best != null && string.CompareOrdinal(d.Name, best.Name) < 0))
                {
                    bestScore = q;
                    best = d;
                }
            }
            return best;
        }

        // ------------------------------------------------------------------ Needs: what the deck is missing

        private static readonly CardRole[] CheckedRoles = { CardRole.Draw, CardRole.PokemonSearch, CardRole.Gust, CardRole.Recovery, CardRole.Switch };

        private int DefaultMinimum(CardRole role, Profile prof)
        {
            switch (role)
            {
                case CardRole.Draw: return 3;
                case CardRole.PokemonSearch: return prof.Cards.Any(x => x.IsPokemon && x.Stage != Stage.Basic) ? 4 : 3;
                case CardRole.Gust: return 1;
                case CardRole.Recovery: return 1;
                case CardRole.Switch: return prof.Main != null && prof.Main.RetreatCost >= 3 ? 1 : 0;
                default: return 0;
            }
        }

        private static double RoleAmount(Profile prof, CardRole role)
        {
            if (role == CardRole.Draw) return prof.DrawLike;
            if (role == CardRole.PokemonSearch) return prof.Search;
            return prof.Role(role);
        }

        private List<Need> Needs(Profile prof, List<Similar> similar)
        {
            var needs = new List<Need>();

            // Evolutions with nothing to evolve from (unless several are there on purpose, e.g. for Slowking to copy).
            var orphans = new List<CardDef>();
            foreach (var kv in prof.Copies)
            {
                var d = prof.Printing[kv.Key];
                if (d.IsPokemon && !string.IsNullOrEmpty(d.EvolvesFrom) && prof.NameCount(d.EvolvesFrom) == 0 && !CandyLine(prof, d)) orphans.Add(d);
            }
            if (orphans.Count > 0 && orphans.Count <= 2 && !prof.CopiesAttacks)
            {
                foreach (var d in orphans)
                    AddNeed(needs, prof, BestNamed(d.EvolvesFrom), 9,
                        d.Name + " evolves from " + d.EvolvesFrom + ", and the deck has no " + d.EvolvesFrom + ".");
            }

            // The main attacker's Energy type.
            if (prof.SpecialEnergy == 0 && prof.Main != null)
            {
                foreach (var kv in prof.MainDemand)
                {
                    int have;
                    prof.BasicEnergy.TryGetValue(kv.Key, out have);
                    if (kv.Value >= 0.5 && have == 0)
                        AddNeed(needs, prof, BasicEnergyOf(kv.Key), 9,
                            prof.Main.Name + "'s attacks need " + kv.Key + " Energy and the deck has none.");
                }
            }

            // Evolution lines: enough Basics for the Stage 1s, enough Stage 1s and Rare Candy for the Stage 2s.
            foreach (var name in prof.Names.Keys.ToList())
            {
                CardDef d = null;
                foreach (var c in prof.Cards) if (c.Name == name && c.IsPokemon) { d = c; break; }
                if (d == null || string.IsNullOrEmpty(d.EvolvesFrom)) continue;
                int n = prof.NameCount(name);
                int pre = prof.NameCount(d.EvolvesFrom);
                if (pre == 0) continue;
                if (d.Stage == Stage.Stage1 && pre < n)
                    AddNeed(needs, prof, BestNamed(d.EvolvesFrom), 6, n + " " + name + " but only " + pre + " " + d.EvolvesFrom + " to evolve from.");
                int candy = (int)prof.Role(CardRole.RareCandy);
                if (d.Stage == Stage.Stage2 && n > pre + candy)
                    AddNeed(needs, prof, BestNamed("Rare Candy"), 6,
                        n + " " + name + " but only " + pre + " " + d.EvolvesFrom + " and " + candy + " Rare Candy to evolve with.");
            }

            // Mulligans.
            if (prof.Basics < 7)
            {
                CardDef basic = null;
                foreach (var f in prof.Families)
                {
                    foreach (var name in f.Names)
                    {
                        if (basic != null || prof.NameCount(name) >= 4) continue;
                        var c = BestNamed(name);
                        if (c != null && c.IsBasicPokemon) basic = c;
                    }
                }
                AddNeed(needs, prof, basic, 5 + (7 - prof.Basics) * 0.5,
                    "Only " + prof.Basics + " Basic Pokémon: you'd mulligan " + Pct(MulliganChance(prof.Basics, 60)) + " of games.");
            }

            // Roles: draw, Pokémon search, gust, recovery, switching.
            foreach (var role in CheckedRoles)
            {
                int target = DefaultMinimum(role, prof);
                bool fromSimilar = similar.Count >= 2;
                if (fromSimilar)
                {
                    double total = 0, sum = 0;
                    foreach (var s in similar)
                    {
                        total += s.Weight;
                        sum += s.Weight * RoleAmount(s.Profile, role);
                    }
                    double avg = sum / total;
                    target = Math.Max(0, Math.Min((int)Math.Floor(avg - 1.5), (int)Math.Ceiling(avg * 0.6)));
                }
                int have = (int)Math.Floor(RoleAmount(prof, role) + 1e-9);
                if (have >= target) continue;
                var card = BestForRole(role, prof, similar);
                AddNeed(needs, prof, card, (fromSimilar ? 4 : 3.5) + 1.5 * (target - have),
                    have + (have == 1 ? " card that " : " cards that ") + CardRoles.Describe(role) + "; "
                    + (fromSimilar ? "similar decks play about " : "most decks play at least ") + target + ".");
            }

            // Energy count.
            if (prof.Main != null && prof.Demand.Count > 0)
            {
                int low;
                if (similar.Count >= 2)
                {
                    double total = 0, sum = 0;
                    foreach (var s in similar)
                    {
                        total += s.Weight;
                        sum += s.Weight * s.Profile.Energy;
                    }
                    double avg = sum / total;
                    prof.EnergyLow = (int)Math.Floor(avg - 1.5);
                    prof.EnergyHigh = (int)Math.Ceiling(avg + 1.5);
                    low = prof.EnergyLow;
                }
                else low = Math.Max(3, 2 * prof.MainCost);
                bool sure = similar.Count >= 2 || (prof.MainTyped && prof.NameCount(prof.Main.Name) >= 2);
                var type = prof.MainType();
                if (prof.Energy < low && sure && type.HasValue)
                    AddNeed(needs, prof, BasicEnergyOf(type.Value), 5 + 0.5 * (low - prof.Energy),
                        prof.Energy + " Energy is few for " + prof.Main.Name + "'s main attack (it costs " + prof.MainCost + ")"
                        + (similar.Count >= 2 ? "; similar decks play " + prof.EnergyLow + "–" + prof.EnergyHigh + "." : "."));
            }

            // ACE SPEC.
            if (prof.AceSpecs == 0 && library.Count > 0)
            {
                CardDef ace = null;
                int aceDecks = 0;
                foreach (var kv in libraryDecks)
                {
                    CardDef d;
                    if (!pool.TryGetValue(kv.Key, out d) || !d.IsAceSpec || !Fits(d, prof)) continue;
                    if (kv.Value > aceDecks || (kv.Value == aceDecks && ace != null && string.CompareOrdinal(d.Name, ace.Name) < 0))
                    {
                        aceDecks = kv.Value;
                        ace = d;
                    }
                }
                int withAce = library.Count(x => x.Profile.AceSpecs > 0);
                if (ace != null)
                    AddNeed(needs, prof, ace, 2 + 2.0 * withAce / library.Count,
                        "No ACE SPEC: " + withAce + " of " + library.Count + " decks in your library play one (" + ace.Name + " is the most played).");
            }

            // What similar decks agree on.
            if (similar.Count >= 2)
            {
                var ids = new HashSet<string>(StringComparer.Ordinal);
                foreach (var s in similar) foreach (var id in s.Profile.Copies.Keys) ids.Add(id);
                foreach (var id in ids)
                {
                    double avg;
                    double share = ShareIn(similar, id, out avg);
                    if (share < 0.7) continue;
                    var c = PrintingFor(id, similar);
                    if (c == null || !Fits(c, prof)) continue;
                    int have = prof.Have(c);
                    int usual = (int)Math.Round(avg);
                    if (have == 0)
                        AddNeed(needs, prof, c, 3 + 4 * (share - 0.7) / 0.3,
                            "In " + PlayedIn(similar, id) + " of " + similar.Count + " similar decks (" + SimilarNames(similar) + "), usually " + usual + ".");
                    else if (have < avg - 1.25 && (have < 4 || (c.IsEnergy && c.IsBasicEnergy)))
                        AddNeed(needs, prof, c, 2.5 + 2 * share, "Similar decks play " + Num(avg) + "; you have " + have + ".");
                }
            }
            needs.Sort((x, y) => y.Priority.CompareTo(x.Priority));
            return needs;
        }

        private static string SimilarNames(List<Similar> similar)
        {
            var names = similar.Take(3).Select(s => s.Name).ToList();
            return string.Join(", ", names.ToArray()) + (similar.Count > 3 ? "…" : "");
        }

        private void AddNeed(List<Need> needs, Profile prof, CardDef card, double priority, string why)
        {
            if (card == null || !CanAdd(card, prof)) return;
            needs.Add(new Need { Card = card, Priority = priority, Why = why });
        }

        /// <summary>The deck is short but nothing is missing: more of what similar decks play, then the format's staples, then Energy.</summary>
        private Need FillNeed(Profile prof, List<Similar> similar)
        {
            if (similar.Count > 0)
            {
                var ids = new HashSet<string>(StringComparer.Ordinal);
                foreach (var s in similar) foreach (var id in s.Profile.Copies.Keys) ids.Add(id);
                CardDef best = null;
                double bestScore = double.MinValue, bestAvg = 0;
                foreach (var id in ids.OrderBy(x => x, StringComparer.Ordinal))
                {
                    double avg;
                    double share = ShareIn(similar, id, out avg);
                    var c = PrintingFor(id, similar);
                    if (c == null || !CanAdd(c, prof) || !Fits(c, prof)) continue;
                    if (c.IsPokemon && !string.IsNullOrEmpty(c.EvolvesFrom) && prof.NameCount(c.EvolvesFrom) == 0) continue;
                    int have = prof.Have(c);
                    if (have >= avg - 0.25) continue;
                    double score = share * (avg - have) + (share >= 0.5 ? 10 : 0);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = c;
                        bestAvg = avg;
                    }
                }
                if (best != null)
                    return new Need { Card = best, Priority = 1, Why = "Filling the deck: similar decks play " + Num(bestAvg) + " " + best.Name + "; you have " + prof.Have(best) + "." };
            }
            CardDef staple = null;
            int staplesDecks = 0;
            foreach (var kv in libraryDecks.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                CardDef c;
                if (!pool.TryGetValue(kv.Key, out c) || !c.IsTrainer || c.IsAceSpec || !CanAdd(c, prof) || !Fits(c, prof)) continue;
                if (kv.Value > staplesDecks)
                {
                    staplesDecks = kv.Value;
                    staple = c;
                }
            }
            if (staple != null)
                return new Need { Card = staple, Priority = 1, Why = "Filling the deck: " + staple.Name + " is played in " + staplesDecks + " of " + library.Count + " decks in your library." };
            var type = prof.MainType();
            if (type.HasValue)
            {
                var e = BasicEnergyOf(type.Value);
                if (e != null) return new Need { Card = e, Priority = 1, Why = "Filling the deck with Energy for its attacks." };
            }
            return null;
        }

        // ------------------------------------------------------------------ Cuts: what does least

        private List<CutOption> CutOptions(Profile prof, List<Similar> similar)
        {
            var list = new List<CutOption>();
            foreach (var kv in prof.Copies)
            {
                var d = prof.Printing[kv.Key];
                int k = d.IsPokemon ? kv.Value : prof.NameCount(d.Name);
                var cut = new CutOption { Id = kv.Key, Card = d, Value = BaseCut };
                bool basicEnergy = d.IsEnergy && d.IsBasicEnergy;
                if (similar.Count >= 2)
                {
                    double avg;
                    double share = ShareIn(similar, kv.Key, out avg);
                    if (share == 0)
                    {
                        cut.Value -= 0.5;
                        cut.Why.Add("no similar deck plays it");
                    }
                    else if (k > avg + 0.5)
                    {
                        cut.Value += 2 * share - (basicEnergy ? Math.Min(3.0, k - avg) : 1.0);
                        cut.Why.Add("similar decks play " + Num(avg));
                    }
                    else cut.Value += 4 * share;
                }
                cut.Value += 2.0 * LibraryShare(kv.Key);
                if (!basicEnergy) cut.Value -= 0.2 * (k - 1);

                var roles = CardRoles.Of(d);
                foreach (var role in CheckedRoles)
                {
                    if ((roles & role) == 0 && !(role == CardRole.PokemonSearch && (roles & CardRole.Tutor) != 0)) continue;
                    if (RoleAmount(prof, role) - 1 < DefaultMinimum(role, prof) + 0.5) cut.Value += 3;
                }
                if (d.IsPokemon)
                {
                    if (prof.KeyNames.Contains(d.Name)) cut.Value += 4;
                    int evolutions = CountWhere(prof.Cards, x => x.IsPokemon && x.EvolvesFrom == d.Name);
                    if (evolutions > 0) cut.Value += prof.NameCount(d.Name) <= evolutions ? 5 : 2;
                    if (!string.IsNullOrEmpty(d.EvolvesFrom) && prof.NameCount(d.EvolvesFrom) == 0 && !CandyLine(prof, d) && !prof.CopiesAttacks)
                    {
                        cut.Value -= 3;
                        cut.Why.Add("nothing to evolve it from");
                    }
                }
                if ((roles & CardRole.RareCandy) != 0 && CandyNeeded(prof)) cut.Value += 5;
                if (basicEnergy)
                {
                    if (!prof.Uses.Contains(d.EnergyProvides))
                    {
                        cut.Value -= prof.DifferentTypes ? 3 : 6;
                        cut.Why.Add("nothing in the deck uses " + d.EnergyProvides + " Energy");
                    }
                    else if (prof.EnergyHigh >= 0 && prof.Energy > prof.EnergyHigh)
                    {
                        cut.Value -= 1.5;
                        cut.Why.Add(prof.Energy + " Energy (similar decks play " + prof.EnergyLow + "–" + prof.EnergyHigh + ")");
                    }
                    else if (prof.EnergyLow >= 0 && prof.Energy <= prof.EnergyLow) cut.Value += 3;
                }
                if ((d.Text ?? "").Contains("Flip a coin"))
                {
                    cut.Value -= 0.3;
                    cut.Why.Add("a coin flip");
                }
                CardOpinion op;
                double opinion = Opinion(d, out op);
                cut.Value += 1.5 * opinion;
                if (opinion <= -0.2) cut.Why.Add("the AI's games rate playing it poorly (" + op.Value.ToString("+0.00;−0.00", CultureInfo.InvariantCulture) + " over " + op.Updates + " decisions)");

                // Illegal copies must go whatever they're worth.
                if (!Formats.Allows(format, d))
                {
                    cut.Value = -100;
                    cut.Why.Clear();
                    cut.Why.Add("not legal in " + Formats.Label(format));
                }
                else if (!basicEnergy && !d.AnyNumberInDeck && prof.LimitedCount(d.Name) > 4)
                {
                    cut.Value = -100;
                    cut.Why.Clear();
                    cut.Why.Add("more than 4 copies");
                }
                else if (d.IsAceSpec && prof.AceSpecs > 1)
                {
                    cut.Value = -100;
                    cut.Why.Clear();
                    cut.Why.Add("more than 1 ACE SPEC");
                }
                else if (d.IsPokemon && d.IsRadiant && prof.Radiants > 1)
                {
                    cut.Value = -100;
                    cut.Why.Clear();
                    cut.Why.Add("more than 1 Radiant Pokémon");
                }
                else if (d.IsPrismStar && prof.NameCount(d.Name) > 1)
                {
                    cut.Value = -100;
                    cut.Why.Clear();
                    cut.Why.Add("more than 1 " + d.Name + " (Prism Star)");
                }
                list.Add(cut);
            }
            list.Sort((x, y) =>
            {
                int byValue = x.Value.CompareTo(y.Value);
                return byValue != 0 ? byValue : string.CompareOrdinal(x.Id, y.Id);
            });
            return list;
        }

        /// <summary>Taking a Rare Candy out would leave a Stage 2 with more copies than Stage 1s and Rare Candy to evolve it with.</summary>
        private static bool CandyNeeded(Profile prof)
        {
            int candy = (int)prof.Role(CardRole.RareCandy);
            foreach (var p in prof.Printing.Values)
            {
                if (!p.IsPokemon || p.Stage != Stage.Stage2 || string.IsNullOrEmpty(p.EvolvesFrom)) continue;
                if (prof.NameCount(p.Name) > prof.NameCount(p.EvolvesFrom) + candy - 1) return true;
            }
            return false;
        }

        // ------------------------------------------------------------------ The swap loop

        /// <summary>
        /// Makes the best swaps one copy at a time: fill a short deck, trim a long one, cut illegal cards, then swap while what
        /// goes in is worth Margin more than what goes out. Cards just added aren't cut and cards just cut aren't added back.
        /// </summary>
        private List<CardDef> Improve(List<CardDef> start, int maxChanges, List<DeckChange> changes, Func<Profile, List<Similar>, List<Need>> extraNeeds)
        {
            var cards = new List<CardDef>(start);
            var added = new HashSet<string>(StringComparer.Ordinal);
            var removed = new HashSet<string>(StringComparer.Ordinal);
            for (int step = 0; step < 150; step++)
            {
                var prof = new Profile(cards);
                var similar = SimilarDecks(prof);
                var needs = Needs(prof, similar);
                if (extraNeeds != null) needs.AddRange(extraNeeds(prof, similar));
                needs.RemoveAll(x => removed.Contains(Id(x.Card)));
                needs.Sort((x, y) => y.Priority.CompareTo(x.Priority));
                var cuts = CutOptions(prof, similar);
                cuts.RemoveAll(x => added.Contains(x.Id));

                Need need = null;
                CutOption cut = null;
                if (cards.Count < 60)
                {
                    need = needs.Count > 0 ? needs[0] : FillNeed(prof, similar);
                    if (need == null) break;
                }
                else if (cards.Count > 60)
                {
                    if (cuts.Count == 0) break;
                    cut = cuts[0];
                }
                else
                {
                    double bestGain = double.MinValue;
                    foreach (var n in needs)
                    {
                        string nid = Id(n.Card);
                        foreach (var c in cuts)
                        {
                            if (c.Id == nid) continue;
                            double gain = n.Priority - c.Value;
                            if (gain > bestGain)
                            {
                                bestGain = gain;
                                need = n;
                                cut = c;
                            }
                            break;
                        }
                    }
                    bool forced = cuts.Count > 0 && cuts[0].Value <= -50;
                    if (need == null || bestGain < Margin)
                    {
                        if (!forced) break;
                        cut = cuts[0];
                        need = FillNeed(new Profile(Without(cards, cut.Card)), similar);
                    }
                }
                if (cut != null)
                {
                    cards.Remove(cut.Card);
                    removed.Add(cut.Id);
                }
                if (need != null)
                {
                    cards.Add(need.Card);
                    added.Add(Id(need.Card));
                }
                Record(changes, need, cut);
                if (need != null && cut != null && SwapCount(changes) > maxChanges)
                {
                    // One swap too many: undo it. (Filling a short deck or trimming a long one isn't limited.)
                    var last = changes[changes.Count - 1];
                    if (last.Add != null) cards.Remove(last.Add);
                    if (last.Cut != null) cards.Add(last.Cut);
                    changes.RemoveAt(changes.Count - 1);
                    break;
                }
            }
            return cards;
        }

        private static int SwapCount(List<DeckChange> changes)
        {
            int n = 0;
            foreach (var c in changes) if (c.Add != null && c.Cut != null) n++;
            return n;
        }

        private static List<CardDef> Without(List<CardDef> cards, CardDef card)
        {
            var list = new List<CardDef>(cards);
            list.Remove(card);
            return list;
        }

        private static void Record(List<DeckChange> changes, Need need, CutOption cut)
        {
            var add = need == null ? null : need.Card;
            var gone = cut == null ? null : cut.Card;
            foreach (var c in changes)
            {
                if (SameCard(c.Add, add) && SameCard(c.Cut, gone))
                {
                    c.Count++;
                    return;
                }
            }
            changes.Add(new DeckChange
            {
                Add = add,
                Cut = gone,
                Why = need == null ? "" : need.Why,
                CutWhy = cut == null ? "" : CutReason(cut)
            });
        }

        private static string CutReason(CutOption cut)
        {
            if (cut.Why.Count == 0) return "does the least for this deck of the cards in it";
            return string.Join(", ", cut.Why.ToArray());
        }

        private static bool SameCard(CardDef a, CardDef b)
        {
            if (a == null || b == null) return a == null && b == null;
            return Id(a) == Id(b);
        }

        // ------------------------------------------------------------------ 1. Changes to your deck

        /// <summary>Swaps for a deck (any size: a short deck is filled, a long one trimmed), with notes about it.</summary>
        public DeckAdvice SuggestChanges(string deckName, IList<CardDef> deck, int maxChanges)
        {
            var advice = new DeckAdvice { Format = format, DeckName = deckName ?? "", Title = "Suggestions for " + (string.IsNullOrEmpty(deckName) ? "this deck" : deckName) };
            var cards = deck == null ? new List<CardDef>() : deck.Where(x => x != null).ToList();
            if (cards.Count == 0)
            {
                advice.Error = "The deck is empty. Add some cards (or use \"Build a deck around a card\").";
                return advice;
            }
            opinions = brain == null ? new Dictionary<string, CardOpinion>(StringComparer.Ordinal) : brain.CardOpinions(deckName, null);
            var prof = new Profile(cards);
            Describe(prof, SimilarDecks(prof), advice.Notes, deckName);
            advice.Result = Improve(cards, Math.Max(1, maxChanges), advice.Changes, null);
            if (advice.Changes.Count == 0) advice.Notes.Add("Nothing stands out: every card in the deck is pulling its weight by these measures.");
            AddProblems(advice);
            return advice;
        }

        private void Describe(Profile prof, List<Similar> similar, List<string> notes, string deckName)
        {
            if (prof.Main != null)
            {
                var family = prof.Families.Count > 0 ? prof.Families[0] : null;
                string line = family == null ? "" : string.Join(", ", prof.FamilyNamesInDeck(family).OrderBy(n => StageOf(prof, n)).Select(n => prof.NameCount(n) + " " + n).ToArray());
                notes.Add("Main attacker: " + prof.Main.Name + (line.Length > 0 ? " (" + line + ")" : "") + ".");
            }
            notes.Add("Basic Pokémon: " + prof.Basics + " (mulligan chance " + Pct(MulliganChance(prof.Basics, Math.Max(60, prof.Cards.Count))) + ").");
            var roles = new List<string>();
            foreach (var role in CheckedRoles)
                roles.Add(CardRoles.ShortName(role) + " " + Num(RoleAmount(prof, role)));
            notes.Add(string.Join(" · ", roles.ToArray()) + ".");
            var energy = new List<string>();
            foreach (var kv in prof.BasicEnergy.OrderByDescending(x => x.Value)) energy.Add(kv.Value + " " + kv.Key);
            if (prof.SpecialEnergy > 0) energy.Add(prof.SpecialEnergy + " Special");
            notes.Add("Energy: " + prof.Energy + (energy.Count > 0 ? " (" + string.Join(", ", energy.ToArray()) + ")" : "")
                      + (prof.Main != null && prof.MainCost > 0 ? "; the main attack costs " + prof.MainCost + "." : "."));
            if (similar.Count > 0)
                notes.Add("Similar decks in your library: " + string.Join(", ", similar.Take(4).Select(s => s.Name + " (" + Pct(s.Likeness) + " alike"
                                                                                                          + (s.AiWinRate >= 0 ? ", the AI wins " + Pct(s.AiWinRate) + " with it" : "") + ")").ToArray())
                          + (similar.Any(s => s.AiWinRate >= 0) ? "; the ones the AI wins more with count more." : "."));
            else
                notes.Add(library.Count > 0
                    ? "No similar deck in your library, so suggestions come from the cards' text and what the " + library.Count + " library decks play most."
                    : "No library decks for this format, so suggestions come from the cards' text.");
            if (brain != null)
            {
                var record = brain.RecordOf(deckName);
                if (record.Games > 0)
                    notes.Add("The Neural AI has played this deck " + record.Games.ToString("N0", CultureInfo.InvariantCulture) + " times: "
                              + record.Wins.ToString("N0", CultureInfo.InvariantCulture) + " wins, " + record.Losses.ToString("N0", CultureInfo.InvariantCulture) + " losses.");
                var skill = brain.SkillOf(deckName);
                if (skill.Decisions > 0)
                    notes.Add("Neural AI training with this deck: " + skill.Games.ToString("N0", CultureInfo.InvariantCulture) + " games, won " + Pct(skill.WinRate)
                              + "; " + skill.Level + " (uncertainty " + skill.Uncertainty.ToString("0.00", CultureInfo.InvariantCulture)
                              + ": about 0.6 is a deck it plays confidently, 0.9 one it's still working out).");
                var lifts = brain.CardResultsOf(deckName ?? "").Where(r => r.HasLift(LearningBrain.LiftGames) && prof.Names.ContainsKey(r.Card)).ToList();
                if (lifts.Count > 0)
                {
                    var helps = lifts.Where(r => r.Lift >= 0.03).OrderByDescending(r => r.Lift).Take(3).ToList();
                    var hurts = lifts.Where(r => r.Lift <= -0.03).OrderBy(r => r.Lift).Take(3).ToList();
                    Func<CardResult, string> say = r => r.Card + " (" + (r.Lift >= 0 ? "+" : "−") + Math.Abs(r.Lift * 100).ToString("0", CultureInfo.InvariantCulture) + ")";
                    if (helps.Count > 0 || hurts.Count > 0)
                        notes.Add("In its training games this deck wins more often when it draws " + (helps.Count > 0 ? string.Join(", ", helps.Select(say).ToArray()) : "nothing in particular")
                                  + (hurts.Count > 0 ? ", and less often when it draws " + string.Join(", ", hurts.Select(say).ToArray()) : "")
                                  + " (points of win rate, drawn vs not drawn).");
                }
                var rated = new List<KeyValuePair<string, CardOpinion>>();
                foreach (var name in prof.Names.Keys)
                {
                    CardOpinion op;
                    if (opinions.TryGetValue(name, out op) && op.Updates >= 30) rated.Add(new KeyValuePair<string, CardOpinion>(name, op));
                }
                if (rated.Count > 0)
                {
                    var liked = rated.Where(x => x.Value.Value > 0.05).OrderByDescending(x => x.Value.Value).Take(3).Select(x => x.Key).ToArray();
                    var disliked = rated.Where(x => x.Value.Value < -0.05).OrderBy(x => x.Value.Value).Take(3).Select(x => x.Key).ToArray();
                    if (liked.Length > 0 || disliked.Length > 0)
                        notes.Add("From the AI's games: playing " + (liked.Length > 0 ? string.Join(", ", liked) + " tends to pay off" : "")
                                  + (liked.Length > 0 && disliked.Length > 0 ? "; " : "")
                                  + (disliked.Length > 0 ? string.Join(", ", disliked) + " less so" : "") + ".");
                }
            }
        }

        private static int StageOf(Profile prof, string name)
        {
            foreach (var d in prof.Cards) if (d.Name == name) return StageRank(d);
            return 0;
        }

        private void AddProblems(DeckAdvice advice)
        {
            var problems = Decklist.Validate(advice.Result, format);
            foreach (var p in problems) advice.Notes.Add("Still to fix: " + p);
        }

        // ------------------------------------------------------------------ 2. A whole deck around a card

        /// <summary>A whole 60-card deck around one card.</summary>
        public DeckAdvice BuildAround(CardDef card)
        {
            var advice = new DeckAdvice { Format = format, IsWholeDeck = true };
            if (card == null)
            {
                advice.Error = "Pick a card to build around.";
                return advice;
            }
            card = Formats.Allows(format, card) ? card : (BestNamed(card.Name) ?? card);
            if (!Formats.Allows(format, card))
            {
                advice.Error = card.Name + " isn't legal in " + Formats.Label(format) + ".";
                return advice;
            }
            opinions = brain == null ? new Dictionary<string, CardOpinion>(StringComparer.Ordinal) : brain.CardOpinions(null, null);
            advice.DeckName = card.Name + " (AI)";
            advice.Title = "A deck built around " + card.Name;

            List<string> sources;
            var cards = Consensus(card, out sources);
            if (cards != null)
            {
                advice.Notes.Add(sources.Count == 1
                    ? "Starts from " + sources[0] + ", the deck in your library that plays the most " + card.Name + "."
                    : "The common core of " + sources.Count + " decks in your library that play " + card.Name + ": " + string.Join(", ", sources.ToArray()) + ".");
            }
            else
            {
                var target = card;
                if (!card.IsPokemon)
                {
                    target = PartnerFor(card);
                    if (target == null)
                    {
                        advice.Error = "No deck in your library plays " + card.Name + ", and no Pokémon in the card pool mentions it. Pick a Pokémon to build around.";
                        return advice;
                    }
                    advice.Notes.Add(card.Name + " isn't in any deck in your library; " + target.Name + " is the Pokémon that works with it best.");
                }
                cards = Template(target, advice.Notes);
                if (!card.IsPokemon) cards = WithCopies(cards, card, card.IsEnergy && !card.IsBasicEnergy ? 4 : 3);
                advice.Notes.Add("Built from the card data: the evolution line, the Trainers most played in your library for each job, and the Energy its attacks need.");
            }

            var fixes = new List<DeckChange>();
            advice.Result = Improve(cards, 4, fixes, null);
            foreach (var f in fixes) advice.Changes.Add(f);
            if (fixes.Count > 0) advice.Notes.Add("The changes below were already made to the list.");
            var prof = new Profile(advice.Result);
            Describe(prof, SimilarDecks(prof), advice.Notes, null);
            AddProblems(advice);
            return advice;
        }

        /// <summary>The cards most of the similar decks that play the card agree on, or null if no library deck plays it.</summary>
        private List<CardDef> Consensus(CardDef card, out List<string> sources)
        {
            sources = new List<string>();
            string id = Id(card);
            var hits = new List<LibraryDeck>();
            foreach (var lib in library)
            {
                int have = card.IsPokemon ? lib.Profile.Count(id) : lib.Profile.NameCount(card.Name);
                if (have > 0) hits.Add(lib);
            }
            if (hits.Count == 0) return null;
            LibraryDeck seed = null;
            double seedScore = double.MinValue;
            foreach (var h in hits)
            {
                int have = card.IsPokemon ? h.Profile.Count(id) : h.Profile.NameCount(card.Name);
                double together = 0;
                foreach (var o in hits) together += Likeness(h.Profile, o.Profile);
                // The list the AI wins most with is the better starting point (once it has played it enough).
                double winRate;
                double score = have * 100 + together + 60 * (Strength(h.Name, out winRate) - 1);
                if (score > seedScore)
                {
                    seedScore = score;
                    seed = h;
                }
            }
            var group = new List<LibraryDeck>();
            foreach (var h in hits) if (h == seed || Likeness(h.Profile, seed.Profile) >= SimilarAt) group.Add(h);
            foreach (var g in group) sources.Add(g.Name);

            var decks = new Dictionary<string, int>(StringComparer.Ordinal);
            var copies = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            var printing = new Dictionary<string, CardDef>(StringComparer.Ordinal);
            foreach (var g in group)
            {
                foreach (var kv in g.Profile.Copies)
                {
                    decks[kv.Key] = Get(decks, kv.Key) + 1;
                    List<int> list;
                    if (!copies.TryGetValue(kv.Key, out list))
                    {
                        list = new List<int>();
                        copies[kv.Key] = list;
                    }
                    list.Add(kv.Value);
                    if (!printing.ContainsKey(kv.Key)) printing[kv.Key] = g.Profile.Printing[kv.Key];
                }
            }
            double n = group.Count;
            var deck = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var kv in decks)
                if (kv.Value / n >= 0.5) deck[kv.Key] = (int)Math.Round(copies[kv.Key].Average(), MidpointRounding.AwayFromZero);
            var order = decks.Keys.OrderByDescending(key => decks[key]).ThenBy(key => key, StringComparer.Ordinal).ToList();
            int total = deck.Values.Sum();
            while (total > 60)
            {
                string victim = null;
                foreach (var k in deck.Keys.OrderBy(key => decks[key]).ThenByDescending(key => deck[key]).ThenBy(key => key, StringComparer.Ordinal))
                {
                    if (k == id) continue;
                    victim = k;
                    break;
                }
                if (victim == null) break;
                deck[victim]--;
                total--;
                if (deck[victim] == 0) deck.Remove(victim);
            }
            foreach (var k in order)
            {
                int most = copies[k].Max();
                while (total < 60 && Get(deck, k) < most)
                {
                    deck[k] = Get(deck, k) + 1;
                    total++;
                }
            }
            var cards = new List<CardDef>();
            foreach (var k in order)
            {
                int c = Get(deck, k);
                for (int i = 0; i < c; i++) cards.Add(printing[k]);
            }
            return cards;
        }

        /// <summary>For a Trainer or Energy with no deck: the Pokémon that names it, or for an Energy the most played attacker of its type.</summary>
        private CardDef PartnerFor(CardDef card)
        {
            CardDef best = null;
            double bestScore = double.MinValue;
            foreach (var d in pool.Values)
            {
                if (!d.IsPokemon) continue;
                bool names = false;
                foreach (var ab in d.Abilities) if ((ab.Text ?? "").Contains(card.Name)) names = true;
                foreach (var a in d.Attacks) if ((a.Text ?? "").Contains(card.Name)) names = true;
                bool typed = card.IsEnergy && card.EnergyProvides != EnergyType.Colorless && d.HasType(card.EnergyProvides) && BestDamage(d) > 0;
                if (!names && !typed) continue;
                double score = (names ? 1000 : 0) + Get(libraryDecks, Id(d)) * 50 + BestDamage(d) * (d.HasRuleBox ? 1.2 : 1.0) / 10.0 + StageRank(d);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = d;
                }
            }
            return best;
        }

        private List<CardDef> WithCopies(List<CardDef> cards, CardDef card, int copies)
        {
            var list = new List<CardDef>(cards);
            int have = CountWhere(list, x => x.Name == card.Name);
            for (int i = have; i < copies; i++) list.Add(card);
            while (list.Count > 60)
            {
                var prof = new Profile(list);
                CardDef victim = null;
                double lowest = double.MaxValue;
                foreach (var d in list)
                {
                    if (!d.IsTrainer || d.Name == card.Name || d.IsAceSpec) continue;
                    double v = LibraryShare(Id(d)) * 10 - prof.NameCount(d.Name);
                    if (v < lowest)
                    {
                        lowest = v;
                        victim = d;
                    }
                }
                if (victim == null) break;
                list.Remove(victim);
            }
            return list;
        }

        /// <summary>The strongest evolution the format has, from a Basic or Stage 1 (Dreepy becomes Dragapult ex).</summary>
        private CardDef Climb(CardDef card)
        {
            var current = card;
            for (int i = 0; i < 2; i++)
            {
                CardDef best = null;
                double bestScore = double.MinValue;
                foreach (var d in pool.Values)
                {
                    if (!d.IsPokemon || d.EvolvesFrom != current.Name) continue;
                    double score = BestDamage(d) * (d.HasRuleBox ? 1.2 : 1.0) + d.Hp / 100.0 + Get(libraryDecks, Id(d)) * 10;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = d;
                    }
                }
                if (best == null) break;
                current = best;
            }
            return current;
        }

        private List<CardDef> Template(CardDef card, List<string> notes)
        {
            var target = card.Stage == Stage.Stage2 ? card : Climb(card);
            if (target != card) notes.Add(card.Name + " evolves into " + target.Name + ", so the deck is built to attack with that.");
            var line = new List<CardDef> { target };
            var current = target;
            for (int i = 0; i < 2 && !string.IsNullOrEmpty(current.EvolvesFrom); i++)
            {
                var pre = BestNamed(current.EvolvesFrom);
                if (pre == null) break;
                line.Add(pre);
                current = pre;
            }
            int stage = StageRank(target);
            int[] counts = stage == 2 ? new[] { 3, 2, 4 } : stage == 1 ? new[] { 4, 4 } : new[] { 4 };
            var cards = new List<CardDef>();
            var lineText = new List<string>();
            for (int i = line.Count - 1; i >= 0; i--)
            {
                int n = i < counts.Length ? counts[i] : 2;
                for (int c = 0; c < n; c++) cards.Add(line[i]);
                lineText.Add(n + " " + line[i].Name);
            }
            notes.Add("Evolution line: " + string.Join(", ", lineText.ToArray()) + ".");
            if (stage == 2)
            {
                var candy = BestNamed("Rare Candy");
                if (candy != null) for (int c = 0; c < 4; c++) cards.Add(candy);
            }

            // Energy for the attacks: the main attacker's costs count fully, the rest of the line a little.
            var attack = MainAttack(target);
            int cost = attack == null ? 1 : attack.Cost.Length;
            int energy = cost <= 1 ? 7 : cost == 2 ? 8 : cost == 3 ? 10 : 11;
            var demand = new Dictionary<EnergyType, double>();
            foreach (var d in line)
            {
                foreach (var a in d.Attacks)
                {
                    foreach (var t in a.Cost)
                    {
                        if (t == EnergyType.Colorless) continue;
                        double have;
                        demand.TryGetValue(t, out have);
                        demand[t] = have + (d == target ? 1.0 : 0.3);
                    }
                }
            }
            if (demand.Count == 0 && target.PokemonType != EnergyType.Colorless) demand[target.PokemonType] = 1;
            double totalDemand = demand.Values.Sum();
            foreach (var kv in demand.OrderByDescending(x => x.Value))
            {
                var e = BasicEnergyOf(kv.Key);
                if (e == null) continue;
                int n = Math.Max(2, (int)Math.Round(energy * kv.Value / totalDemand, MidpointRounding.AwayFromZero));
                for (int c = 0; c < n; c++) cards.Add(e);
            }

            // Trainers for each job, most played first.
            Pick(cards, CardRole.Draw, 7);
            Pick(cards, CardRole.PokemonSearch, stage == 2 ? 8 : 7);
            Pick(cards, CardRole.Gust, 3);
            Pick(cards, CardRole.Recovery, 2);
            Pick(cards, CardRole.Switch, target.RetreatCost >= 2 ? 2 : 1);

            var prof = new Profile(cards);
            CardDef ace = null;
            int aceDecks = -1;
            foreach (var d in pool.Values)
            {
                if (!d.IsAceSpec || !Fits(d, prof)) continue;
                int used = Get(libraryDecks, Id(d));
                if (used > aceDecks || (used == aceDecks && ace != null && string.CompareOrdinal(d.Name, ace.Name) < 0))
                {
                    aceDecks = used;
                    ace = d;
                }
            }
            if (ace != null) cards.Add(ace);

            // Support Pokémon most decks in the library play (one copy each, up to three).
            int support = 0;
            foreach (var kv in libraryDecks.OrderByDescending(x => x.Value).ThenBy(x => x.Key, StringComparer.Ordinal))
            {
                if (support >= 3 || library.Count == 0) break;
                CardDef d;
                if (!pool.TryGetValue(kv.Key, out d) || !d.IsBasicPokemon || CardRoles.Of(d) == CardRole.None) continue;
                if ((double)kv.Value / library.Count < 0.2) break;
                if ((double)Get(libraryCopies, kv.Key) / kv.Value > 2.0) continue;
                if (cards.Any(x => x.Name == d.Name)) continue;
                cards.Add(d);
                support++;
            }
            // At least 8 Basic Pokémon, so the deck rarely mulligans: more of the most played support Pokémon.
            foreach (var kv in libraryDecks.OrderByDescending(x => x.Value).ThenBy(x => x.Key, StringComparer.Ordinal))
            {
                if (support >= 5 || CountWhere(cards, x => x.IsBasicPokemon) >= 8) break;
                CardDef d;
                if (!pool.TryGetValue(kv.Key, out d) || !d.IsBasicPokemon || CardRoles.Of(d) == CardRole.None) continue;
                if (cards.Any(x => x.Name == d.Name)) continue;
                cards.Add(d);
                support++;
            }

            // Fill with what the library plays most, then more copies of draw and search.
            foreach (var kv in libraryDecks.OrderByDescending(x => x.Value).ThenBy(x => x.Key, StringComparer.Ordinal))
            {
                if (cards.Count >= 60) break;
                CardDef d;
                if (!pool.TryGetValue(kv.Key, out d) || !d.IsTrainer || d.IsAceSpec) continue;
                if (cards.Any(x => x.Name == d.Name) || !Fits(d, new Profile(cards))) continue;
                int n = Math.Min(60 - cards.Count, Math.Max(1, (int)Math.Round((double)Get(libraryCopies, kv.Key) / kv.Value, MidpointRounding.AwayFromZero)));
                for (int c = 0; c < n; c++) cards.Add(d);
            }
            while (cards.Count < 60)
            {
                CardDef more = null;
                double bestShare = -1;
                foreach (var d in cards.Distinct())
                {
                    if (!d.IsTrainer || d.IsAceSpec || CountWhere(cards, x => x.Name == d.Name) >= 4) continue;
                    var r = CardRoles.Of(d);
                    if ((r & (CardRole.Draw | CardRole.PokemonSearch)) == 0) continue;
                    double share = LibraryShare(Id(d));
                    if (share > bestShare)
                    {
                        bestShare = share;
                        more = d;
                    }
                }
                if (more == null) more = cards.FirstOrDefault(x => x.IsEnergy && x.IsBasicEnergy);
                if (more == null) more = cards.FirstOrDefault(x => x.IsBasicPokemon);
                if (more == null) break;
                cards.Add(more);
            }
            while (cards.Count > 60)
            {
                CardDef victim = null;
                double lowest = double.MaxValue;
                foreach (var d in cards)
                {
                    if (!d.IsTrainer || d.IsAceSpec) continue;
                    double v = LibraryShare(Id(d)) * 10 - CountWhere(cards, x => x.Name == d.Name);
                    if (v < lowest)
                    {
                        lowest = v;
                        victim = d;
                    }
                }
                if (victim == null) break;
                cards.Remove(victim);
            }
            return cards;
        }

        /// <summary>Adds the most played cards for a role until the deck has n cards that do it.</summary>
        private void Pick(List<CardDef> cards, CardRole role, int n)
        {
            var prof = new Profile(cards);
            int got = (int)Math.Floor(RoleAmount(prof, role) + 1e-9);
            var candidates = new List<KeyValuePair<double, CardDef>>();
            foreach (var kv in pool)
            {
                var d = kv.Value;
                var roles = CardRoles.Of(d);
                bool match = (roles & role) != 0 || (role == CardRole.PokemonSearch && (roles & CardRole.Tutor) != 0);
                if (!match || d.IsPokemon || d.IsAceSpec) continue;
                if (prof.NameCount(d.Name) > 0 || !Fits(d, prof)) continue;
                CardOpinion op;
                double score = LibraryShare(kv.Key) + 0.1 * Opinion(d, out op) - ((d.Text ?? "").Contains("Flip a coin") ? 0.05 : 0);
                candidates.Add(new KeyValuePair<double, CardDef>(score, d));
            }
            candidates.Sort((x, y) =>
            {
                int byScore = y.Key.CompareTo(x.Key);
                return byScore != 0 ? byScore : string.CompareOrdinal(x.Value.Name, y.Value.Name);
            });
            foreach (var c in candidates)
            {
                if (got >= n) break;
                int k = Math.Min(4, n - got);
                for (int i = 0; i < k; i++) cards.Add(c.Value);
                got += k;
            }
        }

        // ------------------------------------------------------------------ 3. Counter a matchup

        /// <summary>Swaps that help this deck against one opponent's deck.</summary>
        public DeckAdvice Counter(string deckName, IList<CardDef> deck, string opponentName, IList<CardDef> opponent, int maxChanges)
        {
            var advice = new DeckAdvice { Format = format, DeckName = deckName ?? "", Title = "Against " + (string.IsNullOrEmpty(opponentName) ? "that deck" : opponentName) };
            var cards = deck == null ? new List<CardDef>() : deck.Where(x => x != null).ToList();
            var theirs = opponent == null ? new List<CardDef>() : opponent.Where(x => x != null).ToList();
            if (cards.Count == 0 || theirs.Count == 0)
            {
                advice.Error = cards.Count == 0 ? "The deck is empty." : "Pick the deck you want to beat.";
                return advice;
            }
            opinions = brain == null ? new Dictionary<string, CardOpinion>(StringComparer.Ordinal) : brain.CardOpinions(deckName, opponentName);
            var them = new Profile(theirs);
            var me = new Profile(cards);
            DescribeOpponent(me, them, opponentName, advice.Notes);
            if (brain != null)
            {
                var m = brain.MatchupOf(deckName, opponentName);
                if (m != null)
                    advice.Notes.Add("The learning AI has played " + (string.IsNullOrEmpty(deckName) ? "this deck" : deckName) + " against " + opponentName + " "
                                     + m.Games + " times: " + m.Wins + " wins, " + m.Losses + " losses.");
            }
            advice.Result = Improve(cards, Math.Max(1, maxChanges), advice.Changes, (p, s) => CounterNeeds(p, them));
            if (advice.Changes.Count == 0) advice.Notes.Add("No change stands out for this matchup by these measures.");
            AddProblems(advice);
            return advice;
        }

        private void DescribeOpponent(Profile me, Profile them, string name, List<string> notes)
        {
            if (them.Main != null)
            {
                var m = them.Main;
                notes.Add("Their main attacker: " + m.Name + " (" + them.NameCount(m.Name) + " copies, " + m.Hp + " HP, " + m.PokemonType + " type"
                          + (m.Weakness.HasValue ? ", weak to " + m.Weakness.Value : "") + ").");
                if (me.Main != null && me.Main.Weakness.HasValue && them.Main.HasType(me.Main.Weakness.Value))
                    notes.Add("Watch out: your " + me.Main.Name + " is weak to their " + them.Main.Name + ".");
            }
            notes.Add("Their deck: " + them.Pokemon + " Pokémon (" + them.AbilityPokemon + " with Abilities), " + them.Trainers + " Trainers ("
                      + them.Items + " Items, " + them.Tools + " Tools, " + them.Stadiums + " Stadiums), " + them.Energy + " Energy ("
                      + them.SpecialEnergy + " Special).");
        }

        private List<Need> CounterNeeds(Profile me, Profile them)
        {
            var needs = new List<Need>();

            // An attacker that hits their main attacker for Weakness.
            if (them.Main != null && them.Main.Weakness.HasValue && them.Main.Weakness.Value != EnergyType.Colorless)
            {
                var weak = them.Main.Weakness.Value;
                int hp = Math.Max(1, them.Main.Hp);
                bool covered = false;
                foreach (var d in me.Printing.Values)
                    if (d.IsPokemon && d.HasType(weak) && Hit(d, them.Main) >= hp * 0.6) covered = true;
                if (!covered)
                {
                    CardDef best = null;
                    double bestScore = double.MinValue;
                    foreach (var d in pool.Values)
                    {
                        if (!d.IsBasicPokemon || !d.HasType(weak) || !CanAdd(d, me)) continue;
                        var a = MainAttack(d);
                        if (a == null || a.Damage <= 0 || !Payable(a, me)) continue;
                        int hit = Hit(d, them.Main);
                        double score = (hit >= hp ? 2.0 : Math.Min(1.0, (double)hit / hp)) + LibraryShare(Id(d)) - 0.1 * a.Cost.Length - (d.HasRuleBox ? 0.2 : 0);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            best = d;
                        }
                    }
                    if (best != null)
                    {
                        var a = MainAttack(best);
                        int hit = Hit(best, them.Main);
                        AddNeed(needs, me, best, hit >= hp ? 7.5 : 5.5,
                            "Their " + them.Main.Name + " is weak to " + weak + ": " + best.Name + "'s " + a.Name + " does " + hit + " to it (" + hp + " HP"
                            + (hit >= hp ? ", a one-hit Knock Out" : "") + ").");
                    }
                }
            }

            // What their deck leans on.
            if (them.AbilityPokemon >= 6)
            {
                var lockCard = BestCounter(CardRole.AbilityLock, me, them, true);
                if (lockCard != null)
                    AddNeed(needs, me, lockCard, 5.5, "Their deck leans on Abilities (" + them.AbilityPokemon + " Pokémon with one); " + lockCard.Name + " turns them off.");
            }
            if (them.Tools >= 3)
            {
                var c = BestCounter(CardRole.ToolRemoval, me, them, false);
                if (c != null) AddNeed(needs, me, c, 4.5, "They play " + them.Tools + " Pokémon Tools; " + c.Name + " discards them.");
            }
            if (them.SpecialEnergy >= 4)
            {
                var c = BestCounter(CardRole.EnergyRemoval, me, them, false, "Special Energy");
                if (c != null) AddNeed(needs, me, c, 5, "They play " + them.SpecialEnergy + " Special Energy; " + c.Name + " discards it.");
            }
            else if (them.MainCost >= 3 && them.Role(CardRole.EnergyAccel) < 3)
            {
                var c = BestCounter(CardRole.EnergyRemoval, me, them, false);
                if (c != null) AddNeed(needs, me, c, 4, "Their " + them.Main.Name + " needs " + them.MainCost + " Energy to attack and they have little to speed it up; " + c.Name + " sets them back.");
            }
            if (them.Items >= 18)
            {
                var c = ItemLocker(me);
                if (c != null) AddNeed(needs, me, c, 5, "They play " + them.Items + " Items; " + c.Name + "'s attack stops them playing Items for a turn.");
            }
            if (them.Stadiums >= 2 && me.Stadiums == 0)
            {
                CardDef stadium = null;
                double bestShare = -1;
                foreach (var kv in pool)
                {
                    var d = kv.Value;
                    if (!d.IsStadium || !CanAdd(d, me) || !Fits(d, me) || them.NameCount(d.Name) > 0) continue;
                    double share = LibraryShare(kv.Key);
                    if (share > bestShare || (share == bestShare && stadium != null && string.CompareOrdinal(d.Name, stadium.Name) < 0))
                    {
                        bestShare = share;
                        stadium = d;
                    }
                }
                if (stadium != null) AddNeed(needs, me, stadium, 4, "They play " + them.Stadiums + " Stadiums; a Stadium of your own (" + stadium.Name + ") replaces theirs.");
            }
            if (them.Main != null && them.Main.HasRuleBox && them.NameCount(them.Main.Name) >= 2)
            {
                CardDef wall = null;
                double bestScore = double.MinValue;
                foreach (var d in pool.Values)
                {
                    if (!d.IsBasicPokemon || !CanAdd(d, me) || me.Have(d) > 0) continue;
                    bool walls = false;
                    foreach (var ab in d.Abilities) if (WallText.IsMatch(ab.Text ?? "")) walls = true;
                    if (!walls) continue;
                    var a = MainAttack(d);
                    double score = LibraryShare(Id(d)) + (a != null && Payable(a, me) ? 1 : 0);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        wall = d;
                    }
                }
                if (wall != null) AddNeed(needs, me, wall, 4.5, "Their attackers have Rule Boxes; " + wall.Name + "'s Ability protects it from them.");
            }
            if (me.Main != null && me.Main.Weakness.HasValue && them.Main != null && them.Main.HasType(me.Main.Weakness.Value))
            {
                CardDef tool = null;
                double bestShare = -1;
                foreach (var kv in pool)
                {
                    var d = kv.Value;
                    if (!d.IsTool || !NoWeakness.IsMatch(d.Text ?? "") || !CanAdd(d, me) || !Fits(d, me)) continue;
                    double share = LibraryShare(kv.Key);
                    if (share > bestShare)
                    {
                        bestShare = share;
                        tool = d;
                    }
                }
                if (tool != null) AddNeed(needs, me, tool, 5, "Your " + me.Main.Name + " is weak to their " + them.Main.Name + "; " + tool.Name + " takes the Weakness away.");
            }
            return needs;
        }

        /// <summary>Damage a Pokémon's main attack does to a target, with Weakness and Resistance.</summary>
        private static int Hit(CardDef attacker, CardDef target)
        {
            var a = MainAttack(attacker);
            if (a == null) return 0;
            int dmg = AttackValue(a);
            if (target.Weakness.HasValue && attacker.HasType(target.Weakness.Value))
                dmg = target.WeaknessAmount > 0 ? dmg + target.WeaknessAmount : dmg * 2;
            if (target.Resistance.HasValue && attacker.HasType(target.Resistance.Value)) dmg = Math.Max(0, dmg - target.ResistanceAmount);
            return dmg;
        }

        /// <summary>The deck's Energy can pay for the attack (its typed costs are in the deck, or it has Special Energy).</summary>
        private static bool Payable(AttackDef a, Profile prof)
        {
            foreach (var t in a.Cost)
                if (t != EnergyType.Colorless && !prof.BasicEnergy.ContainsKey(t) && prof.SpecialEnergy == 0) return false;
            return true;
        }

        private CardDef BestCounter(CardRole role, Profile me, Profile them, bool weighAbilities, string mention = null)
        {
            CardDef best = null;
            double bestScore = double.MinValue;
            foreach (var kv in pool)
            {
                var d = kv.Value;
                if (d.IsPokemon || (CardRoles.Of(d) & role) == 0 || !CanAdd(d, me) || !Fits(d, me)) continue;
                string text = d.Text ?? "";
                double score = LibraryShare(kv.Key);
                if (mention != null && text.Contains(mention)) score += 1;
                if (weighAbilities)
                {
                    int theirsHit = 0, mineHit = 0;
                    foreach (var p in them.Cards) if (p.IsPokemon && p.HasAbility && Affected(text, p)) theirsHit++;
                    foreach (var p in me.Cards) if (p.IsPokemon && p.HasAbility && Affected(text, p)) mineHit++;
                    if (theirsHit < 3 || theirsHit <= mineHit) continue;
                    score += (theirsHit - mineHit) / 10.0;
                }
                if (score > bestScore)
                {
                    bestScore = score;
                    best = d;
                }
            }
            return best;
        }

        /// <summary>Whether an Ability-locking text reaches this Pokémon ("Pokémon with a Rule Box", "Colorless Pokémon", or everyone).</summary>
        private static bool Affected(string text, CardDef p)
        {
            if (text.Contains("Rule Box")) return p.HasRuleBox;
            foreach (Match m in TypeWord.Matches(text))
            {
                EnergyType t;
                if (Enum.TryParse(m.Groups[1].Value, out t)) return p.HasType(t);
            }
            if (text.Contains("Colorless Pokémon")) return p.HasType(EnergyType.Colorless);
            if (text.Contains("Basic Pokémon")) return p.IsBasicPokemon;
            if (text.Contains("Evolution Pokémon")) return p.IsEvolution;
            return true;
        }

        private CardDef ItemLocker(Profile me)
        {
            CardDef best = null;
            double bestScore = double.MinValue;
            foreach (var kv in pool)
            {
                var d = kv.Value;
                if (!d.IsBasicPokemon || (CardRoles.OfAttacks(d) & CardRole.ItemLock) == 0 || !CanAdd(d, me)) continue;
                bool payable = false;
                int cheapest = 99;
                foreach (var a in d.Attacks)
                {
                    if ((CardRoles.FromText(a.Text) & CardRole.ItemLock) == 0) continue;
                    if (Payable(a, me))
                    {
                        payable = true;
                        cheapest = Math.Min(cheapest, a.Cost.Length);
                    }
                }
                if (!payable) continue;
                double score = LibraryShare(kv.Key) * 2 - cheapest * 0.3 - (d.HasRuleBox ? 0.5 : 0);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = d;
                }
            }
            return best;
        }
    }
}
