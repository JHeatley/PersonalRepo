using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>Temporal Forces cards (v8): data, the Standard / Expanded split, new engine hooks, and fuzz games with every TEF card.</summary>
    public class TefTests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private static bool Has(GameEngine e, int player, string label)
        {
            return e.GetMainActions(player).Any(o => o.Label == label);
        }

        private static bool HasStarting(GameEngine e, int player, string label)
        {
            return e.GetMainActions(player).Any(o => o.Label.StartsWith(label));
        }

        // ------------------------------------------------------------------ Data and formats

        [Test]
        public void Every_Temporal_Forces_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 162; n++) Assert.IsNotNull(db.Get("TEF", n.ToString()), "TEF " + n);
            // Secret rares are alternate printings of main-set cards.
            for (int n = 163; n <= 218; n++)
            {
                var d = db.Get("TEF", n.ToString());
                Assert.IsNotNull(d, "TEF " + n);
                Assert.IsTrue(d.IsAlternatePrint, "TEF " + n + " should be an alternate printing");
            }
        }

        [Test]
        public void Tags_And_Ace_Specs()
        {
            Assert.IsTrue(C("TEF-147").IsAncient, "Explorer's Guidance is an Ancient Supporter");
            Assert.IsTrue(C("TEF-145").IsFuture, "Ciphermaniac's Codebreaking is a Future Supporter");
            Assert.IsTrue(C("TEF-141").IsAncient && C("TEF-141").IsAceSpec, "Awakening Drum");
            Assert.IsTrue(C("TEF-158").IsFuture && C("TEF-158").IsAceSpec, "Reboot Pod");
            Assert.IsTrue(C("TEF-153").IsAceSpec, "Master Ball");
            Assert.IsFalse(C("TEF-143").IsAceSpec, "Boxed Order");
            Assert.IsTrue(C("TEF-96").IsAncient && C("TEF-118").IsFuture);
        }

        [Test]
        public void G_Cards_Are_Expanded_And_H_Cards_Are_Standard()
        {
            foreach (var key in new[] { "TEF-22", "TEF-140", "TEF-149" })
                Assert.IsFalse(Formats.IsStandardCard(C(key)), key + " has a G mark");
            Assert.IsTrue(Formats.IsStandardCard(C("TEF-104")));
            Assert.IsTrue(Formats.IsStandardCard(C(Psychic)), "Basic Energy");
            Assert.IsTrue(Formats.IsStandardCard(C("SVI-186")), "Pokégear 3.0 is legal through a reprint");

            var deck = new List<CardDef>();
            for (int i = 0; i < 4; i++) deck.Add(C("TEF-121"));
            deck.Add(C("TEF-149"));
            while (deck.Count < 60) deck.Add(C(Lightning));
            Assert.IsNotEmpty(Decklist.Validate(deck, Formats.LegalMarks(DeckFormat.Standard)));
            CollectionAssert.IsEmpty(Decklist.Validate(deck, Formats.LegalMarks(DeckFormat.Expanded)));
        }

        [Test]
        public void Deck_Lists_Remember_Their_Format()
        {
            var entries = new List<DeckEntry> { new DeckEntry { Card = C("TEF-121"), Count = 4 } };
            var text = Decklist.Format("Booster Test", entries, DeckFormat.Expanded);
            StringAssert.Contains("# Format: Expanded", text);
            Assert.AreEqual(DeckFormat.Expanded, Decklist.ReadFormat(text));
            Assert.AreEqual("Booster Test", Decklist.ReadName(text));
            Assert.AreEqual(DeckFormat.Standard, Decklist.ReadFormat(Decklist.Format("Plain", entries)));
        }

        // ------------------------------------------------------------------ New engine hooks

        [Test]
        public void Gnawing_Curse_Puts_2_Counters_On_Energy_Attached_From_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Mr. Mime"), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C("TEF-63"));
            var gengar = e.ScenarioActive(1, C("TEF-104"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mime.Energy.Count);
            Assert.AreEqual(2, mime.DamageCounters);
            Assert.AreEqual(0, gengar.DamageCounters);
        }

        [Test]
        public void Lava_Zone_Burns_The_New_Active_After_A_Retreat()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var ponyta = e.ScenarioActive(0, C("TEF-26"));
            var mime = e.ScenarioBench(0, C("TEF-63"));
            e.ScenarioAttach(ponyta, C(Fire));
            e.ScenarioActive(1, C("TEF-29"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(mime, e.State.Players[0].Active);
            Assert.IsTrue(mime.Has(SpecialCondition.Burned));
            Assert.IsFalse(ponyta.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Memory_Dive_Gives_Evolved_Pokemon_Their_Earlier_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var croc = e.ScenarioActive(0, C("TEF-39"));
            e.ScenarioEvolve(croc, C("TEF-40"));
            e.ScenarioAttach(croc, C(Water));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Has(e, 0, "Attack: Big Bite (Totodile)"));
            e.ScenarioBench(0, C("TEF-84"));
            Assert.IsTrue(Has(e, 0, "Attack: Big Bite (Totodile)"), "Relicanth's Memory Dive");
        }

        [Test]
        public void Dual_Core_Makes_Iron_Treads_Fighting_Type_For_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wheel Pass"), new ScriptedAgent());
            var treads = e.ScenarioActive(0, C("TEF-118"));
            e.ScenarioAttach(treads, C(Metal));
            e.ScenarioAttach(treads, C(Metal));
            e.ScenarioAttachTool(treads, C("TEF-149"));
            var wugtrio = e.ScenarioActive(1, C("TEF-60"));
            e.ScenarioSetTurn(0, 3);
            CollectionAssert.AreEquivalent(new[] { EnergyType.Metal, EnergyType.Fighting }, e.TypesOf(treads));
            Step(e);
            // (60 + 20 from Future Booster Energy Capsule) x2 for Wugtrio ex's Fighting Weakness.
            Assert.AreEqual(16, wugtrio.DamageCounters);
        }

        [Test]
        public void Boxed_Order_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boxed Order", "Buddy-Buddy Poffin|Buddy-Buddy Poffin"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("TEF-143"));
            e.ScenarioAddToDeck(0, C("TEF-144"), 2);
            e.ScenarioSetTurn(0, 3);
            bool over = e.MainPhaseStep();
            Assert.IsTrue(over, "Boxed Order: your turn ends");
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.Name == "Buddy-Buddy Poffin"));
        }

        [Test]
        public void Heavy_Baton_Moves_Energy_When_A_Retreat_4_Active_Is_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Power Stomp"), new ScriptedAgent());
            var boulder = e.ScenarioActive(0, C("TEF-99"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(boulder, C(Fighting));
            var grotle = e.ScenarioActive(1, C("TEF-11"));
            e.ScenarioAttach(grotle, C(Grass));
            e.ScenarioAttach(grotle, C(Grass));
            e.ScenarioAttachTool(grotle, C("TEF-151"));
            var bench = e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(bench, e.State.Players[1].Active, "Grotle was Knocked Out");
            Assert.AreEqual(2, bench.Energy.Count, "both Grass Energy moved by Heavy Baton");
        }

        [Test]
        public void Expelling_Tornado_Keeps_3_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Expelling Tornado"), new ScriptedAgent());
            var shiftry = e.ScenarioActive(0, C("TEF-5"));
            e.ScenarioAttach(shiftry, C(Grass));
            e.ScenarioActive(1, C(Dreepy));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Dreepy));
            int deck = e.State.Players[1].Deck.Count;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Bench.Count);
            Assert.AreEqual(deck + 2, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Plentiful_Pollen_Gives_2_More_Prizes_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Plentiful Pollen", "Attack: Plentiful Pollen"), new ScriptedAgent());
            var ribombee = e.ScenarioActive(0, C("TEF-76"));
            e.ScenarioAttach(ribombee, C(Psychic));
            e.ScenarioActive(1, C("TEF-70"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Prizes.Count, "1 Prize for Solosis + 2 from Plentiful Pollen");
        }

        [Test]
        public void Hustle_Play_Lowers_The_Cost_For_Each_Opposing_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var inci = e.ScenarioActive(0, C("TEF-34"));
            e.ScenarioAttach(inci, C(Fire));
            e.ScenarioAttach(inci, C(Fire));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Has(e, 0, "Attack: Blaze Blast"));
            for (int i = 0; i < 3; i++) e.ScenarioBench(1, C(Dreepy));
            Assert.IsTrue(Has(e, 0, "Attack: Blaze Blast"));
        }

        [Test]
        public void Blaze_Blitz_Is_Locked_Until_Gouging_Fire_Leaves_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blaze Blitz"), new ScriptedAgent());
            var gouging = e.ScenarioActive(0, C("TEF-38"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(gouging, C(Fire));
            var bench = e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C("TEF-104"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Has(e, 0, "Attack: Blaze Blitz"));
            Assert.IsTrue(Has(e, 0, "Attack: Heat Blast"));
            e.SwitchActive(0, bench);
            e.SwitchActive(0, gouging);
            Assert.IsTrue(Has(e, 0, "Attack: Blaze Blitz"), "back in the Active Spot after leaving it");
        }

        [Test]
        public void Meteor_Mash_Does_60_More_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Meteor Mash", "Attack: Meteor Mash"), new ScriptedAgent());
            var meta = e.ScenarioActive(0, C("TEF-115"));
            e.ScenarioAttach(meta, C(Metal));
            var gengar = e.ScenarioActive(1, C("TEF-104"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, gengar.DamageCounters);
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(18, gengar.DamageCounters);
        }

        [Test]
        public void Emergency_Evolution_Evolves_Pidove_Into_Unfezant_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Emergency Evolution"), new ScriptedAgent());
            var pidove = e.ScenarioActive(0, C("TEF-133"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C("TEF-135"), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(HasStarting(e, 0, "Use Emergency Evolution"), "50 HP left");
            pidove.DamageCounters = 2;
            Step(e);
            Assert.AreEqual("Unfezant", pidove.Name);
        }

        [Test]
        public void Emergency_Evolution_Ignores_Normal_Evolution_Timing()
        {
            // Your ruling: first turn, and a Pidove put into play this turn, are both fine.
            var e = Scenario(new ScriptedAgent().Then("Use Emergency Evolution"), new ScriptedAgent());
            var pidove = e.ScenarioActive(0, C("TEF-133"));
            pidove.TurnEnteredPlay = 1;
            pidove.DamageCounters = 2;
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C("TEF-135"), 1);
            e.ScenarioSetTurn(0, 1);
            Assert.IsTrue(e.State.IsPlayersFirstTurn);
            Step(e);
            Assert.AreEqual("Unfezant", pidove.Name);
        }

        [Test]
        public void Invite_And_Strike_Needs_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C("TEF-112"));
            e.ScenarioAttach(mawile, C(Metal));
            e.ScenarioAttach(mawile, C(Metal));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Has(e, 0, "Attack: Invite and Strike"), "your ruling: not usable with an empty Bench");
            e.ScenarioBench(1, C("TEF-70"));
            Assert.IsTrue(Has(e, 0, "Attack: Invite and Strike"));
        }

        [Test]
        public void Summoning_Gate_Can_Bench_Evolution_Pokemon()
        {
            var agent = new ScriptedAgent().Then("Attack: Summoning Gate", "Duosion|Solosis");
            var e = Scenario(agent, new ScriptedAgent());
            var reuniclus = e.ScenarioActive(0, C("TEF-72"));
            e.ScenarioAttach(reuniclus, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C("TEF-71"))); // Duosion on top of the deck
            e.State.Players[0].Deck.Insert(1, e.NewCard(0, C("TEF-70"))); // then Solosis
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var choice = agent.Seen.FirstOrDefault(r => r.Kind == DecisionKind.ChooseCards);
            Assert.IsNotNull(choice);
            Assert.IsTrue(choice.Options.Any(o => o.Card.Name == "Duosion"), "your ruling: Evolution Pokémon can be put onto the Bench too");
            Assert.IsTrue(choice.Options.Any(o => o.Card.Name == "Solosis"));
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(2, bench.Count);
            var duosion = bench.First(m => m.Name == "Duosion");
            Assert.AreEqual(1, duosion.Stack.Count, "put into play on its own, not evolved");
            e.CardsPerPlayer = 0;
            e.AssertInvariants();
        }

        [Test]
        public void Hand_Trimmer_Leaves_Both_Players_With_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hand Trimmer"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("TEF-150"));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(0, C(Psychic));
            for (int i = 0; i < 8; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(5, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Majestic_Sword_Needs_A_Future_Supporter_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ciphermaniac's Codebreaking", "Psychic Energy|Psychic Energy", "Psychic Energy", "Attack: Majestic Sword"), new ScriptedAgent());
            var valiant = e.ScenarioActive(0, C("TEF-80"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(valiant, C(Psychic));
            var gengar = e.ScenarioActive(1, C("TEF-104"));
            e.ScenarioAddToHand(0, C("TEF-145"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(20, gengar.DamageCounters, "100 + 100");
        }

        [Test]
        public void Ancient_Booster_Energy_Capsule_Adds_60_HP_And_Stops_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tusk = e.ScenarioActive(0, C("TEF-96"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(140, e.MaxHp(tusk));
            e.ApplyCondition(tusk, SpecialCondition.Poisoned);
            e.ScenarioAttachTool(tusk, C("TEF-140"));
            Assert.AreEqual(200, e.MaxHp(tusk));
            e.StateChecks();
            Assert.IsFalse(tusk.Has(SpecialCondition.Poisoned), "recovers");
            Assert.IsFalse(e.ApplyCondition(tusk, SpecialCondition.Confused));
        }

        [Test]
        public void Look_Alike_Show_Uses_A_Supporter_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Look-Alike Show", "Use Boss's Orders"), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C("TEF-63"));
            e.ScenarioAttach(mime, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            var benched = e.ScenarioBench(1, C("TEF-70"));
            e.ScenarioAddToHand(1, C("MEG-114"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(benched, e.State.Players[1].Active);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count, "the Supporter stays in the opponent's hand");
        }

        // ------------------------------------------------------------------ Every TEF card in real games

        /// <summary>Four 60-card decks that between them hold one of every Temporal Forces card (filled with Basic Energy).</summary>
        private List<List<CardDef>> TefDecks()
        {
            var tef = db.Cards.Where(d => d.SetCode == "TEF").OrderBy(d => int.Parse(d.Number)).ToList();
            var energy = new[] { Grass, Fire, Water, Lightning, Psychic, Fighting, Darkness, Metal };
            var decks = new List<List<CardDef>>();
            int per = (tef.Count + 3) / 4;
            for (int k = 0; k < 4; k++)
            {
                var deck = tef.Skip(k * per).Take(per).ToList();
                for (int i = 0; deck.Count < 60; i++) deck.Add(C(energy[(i + k) % energy.Length]));
                decks.Add(deck);
            }
            return decks;
        }

        [Test]
        public void Every_Tef_Card_Survives_Random_Play()
        {
            var decks = TefDecks();
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
                for (int j = 0; j < decks.Count; j++)
                {
                    if (i == j) continue;
                    var report = MatchRunner.Run(scripts, decks[i], decks[j], 4, 7000 + i * 100 + j * 10, (seat, s) => new RandomAgent(s), true);
                    foreach (var err in report.Errors) errors.Add("TEF deck " + i + " vs " + j + ": " + err);
                }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }

        [Test]
        public void Every_Tef_Card_Plays_Full_Games_With_Heuristic_Bots()
        {
            var decks = TefDecks();
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
            {
                var report = MatchRunner.Run(scripts, decks[i], decks[(i + 1) % decks.Count], 3, 8000 + i * 10, (seat, s) => new HeuristicAgent(s), true);
                foreach (var err in report.Errors) errors.Add("TEF deck " + i + ": " + err);
            }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }
    }
}
