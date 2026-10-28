using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v36: Paldean Fates (PAF), all 245 cards: new cards, alternate prints of pool cards, and a selection of effects.</summary>
    public class PafTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string MistEnergy = "TEF-161";

        const string Forretress = "PAF-2";
        const string Toedscruel = "PAF-5";
        const string Exeggcute = "PAF-23";
        const string Ralts = "PAF-27";
        const string Kirlia = "PAF-28";
        const string Gardevoir = "PAF-29";
        const string Swoobat = "PAF-33";
        const string MimeJr = "PAF-31";
        const string Noivern = "PAF-69";
        const string Artazon = "PAF-76";
        const string MoonlitHill = "PAF-81";
        const string Weavile = "PAF-183";
        const string PaldeanStudent = "PAF-85";
        const string CrisisPunch = "PAF-90";
        const string Oricorio = "PAF-113";
        const string Entei = "PAF-112";
        const string Dondozo = "PAF-126";
        const string Pawmi = "PAF-142";
        const string Pawmo = "PAF-143";
        const string Klefki = "PAF-159";
        const string Gengar = "PAF-57";
        const string Kingambit = "PAF-187";
        const string Ditto = "PAF-201";
        const string Glimmora = "PAF-218";
        const string Pineco = "SVP-61";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        // ================================================================== Data

        [Test]
        public void Every_Paldean_Fates_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 245; n++)
                Assert.IsNotNull(db.Get("PAF-" + n), "PAF " + n + " is missing");
        }

        [Test]
        public void Reprints_Are_Alternate_Prints_Of_The_Pool_Card()
        {
            Assert.AreEqual("SVP-61", db.MainPrinting(C("PAF-1")).Key, "Pineco is the SVP Pineco");
            Assert.AreEqual("SVP-61", db.MainPrinting(C("PAF-99")).Key, "and so is the shiny one");
            Assert.AreEqual("MEG-125", db.MainPrinting(C("PAF-89")).Key, "Rare Candy");
            Assert.AreEqual("SVP-124", db.MainPrinting(C("PAF-237")).Key, "Iono");
            Assert.AreEqual(Gardevoir, db.MainPrinting(C("PAF-217")).Key, "the Special Illustration Rare Gardevoir ex");
            Assert.AreEqual(Gardevoir, db.MainPrinting(C("SVI-86")).Key, "and its Scarlet & Violet print");
            Assert.AreEqual("G", C(Gardevoir).RegulationMark, "Paldean Fates is G: Expanded only");
        }

        // ================================================================== Abilities

        [Test]
        public void Psychic_Embrace_Attaches_From_The_Discard_With_2_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Psychic Embrace", "Ralts"), new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C(Gardevoir));
            var ralts = e.ScenarioBench(0, C(Ralts));
            e.ScenarioAddToDiscard(0, C(Psychic), 2);
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(1, ralts.Energy.Count);
            Assert.AreEqual(2, ralts.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Psychic Energy"));
            Assert.AreEqual(0, gardevoir.DamageCounters);
        }

        [Test]
        public void Psychic_Embrace_Cannot_Be_Used_On_A_Pokemon_It_Would_Knock_Out()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C(Gardevoir));
            var ralts = e.ScenarioBench(0, C(Ralts));
            e.ScenarioAddToDiscard(0, C(Psychic), 2);
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);
            gardevoir.DamageCounters = 29; // 20 HP left
            ralts.DamageCounters = 5;      // 20 HP left

            Assert.IsFalse(Offered(e, 0, "Psychic Embrace"));
            ralts.DamageCounters = 4;
            Assert.IsTrue(Offered(e, 0, "Psychic Embrace"));
        }

        [Test]
        public void Exploding_Energy_Attaches_Grass_Energy_Then_Knocks_Out_Forretress_Ex()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Exploding Energy"), new ScriptedAgent());
            var pineco = e.ScenarioActive(0, C(Pineco));
            var forretress = e.ScenarioBench(0, C(Forretress));
            e.ScenarioAddToDeck(0, C(Grass), 5);
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.IsFalse(e.State.Players[0].Bench.Contains(forretress), "Forretress ex is Knocked Out");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Forretress ex"));
            Assert.AreEqual(1, pineco.Energy.Count(c => c.Def.Name == "Grass Energy"), "the Energy it found went onto Pineco");
        }

        [Test]
        public void Dust_Field_Limits_The_Opponents_Bench_To_3()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Glimmora));
            e.ScenarioActive(1, C(Dondozo));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Pawmi));
            e.ScenarioSetTurn(0, 3);

            Assert.AreEqual(3, e.MaxBench(1));
            Assert.AreEqual(5, e.MaxBench(0), "only the opponent's Bench");
            e.EnforceBenchLimits(0);
            Assert.AreEqual(3, e.State.Players[1].Bench.Count);
        }

        [Test]
        public void Mischievous_Lock_Turns_Off_Basic_Pokemon_Abilities_While_Klefki_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var klefki = e.ScenarioActive(0, C(Klefki));
            var oricorio = e.ScenarioBench(0, C(Oricorio));
            var gengar = e.ScenarioBench(0, C(Gengar));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Assert.IsTrue(e.AbilityBlocked(oricorio, "Ardent Dancing"), "a Basic Pokémon's Ability");
            Assert.IsFalse(e.AbilityBlocked(gengar, "Night Gate"), "a Stage 2 keeps its Ability");
            Assert.IsFalse(e.AbilityBlocked(klefki, "Mischievous Lock"));

            e.SwitchActive(0, gengar);
            Assert.IsFalse(e.AbilityBlocked(oricorio, "Ardent Dancing"), "only while Klefki is Active");
        }

        [Test]
        public void Protective_Mycelium_Stops_Attack_Effects_On_Pokemon_With_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hypnosis"), new ScriptedAgent());
            var exeggcute = e.ScenarioActive(0, C(Exeggcute));
            e.ScenarioAttach(exeggcute, C(Psychic));
            var toedscruel = e.ScenarioActive(1, C(Toedscruel));
            e.ScenarioAttach(toedscruel, C(Grass));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.IsFalse(toedscruel.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Pressure_Takes_20_Off_The_Opponents_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Head Bolt"), new ScriptedAgent());
            var pawmo = e.ScenarioActive(0, C(Pawmo));
            e.ScenarioAttach(pawmo, C(Lightning));
            e.ScenarioAttach(pawmo, C(Lightning));
            e.ScenarioAttach(pawmo, C(Psychic));
            var entei = e.ScenarioActive(1, C(Entei));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(5, entei.DamageCounters, "Head Bolt 70 - 20");
        }

        [Test]
        public void Leadership_Gives_Basic_Pokemon_30_More_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Zap Kick"), new ScriptedAgent());
            var pawmi = e.ScenarioActive(0, C(Pawmi));
            e.ScenarioAttach(pawmi, C(Lightning));
            e.ScenarioAttach(pawmi, C(Psychic));
            e.ScenarioBench(0, C(Kingambit));
            var pineco = e.ScenarioActive(1, C(Pineco));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(5, pineco.DamageCounters, "Zap Kick 20 + 30");
        }

        [Test]
        public void Transformative_Start_Puts_A_Basic_From_The_Deck_In_Dittos_Place()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Transformative Start", "Pawmi"), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(Ditto));
            e.ScenarioAttach(ditto, C(Psychic));
            e.ScenarioAddToDeck(0, C(Pawmi), 1);
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 1);

            Step(e);
            var active = e.State.Players[0].Active;
            Assert.AreEqual("Pawmi", active.Name);
            Assert.AreEqual(0, active.Energy.Count, "Ditto's Energy was discarded with it");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Ditto"));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Psychic Energy"));
        }

        // ================================================================== Attacks

        [Test]
        public void Attraction_Wave_Confusion_Puts_6_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Attraction Wave"), new ScriptedAgent());
            var swoobat = e.ScenarioActive(0, C(Swoobat));
            e.ScenarioAttach(swoobat, C(Psychic));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.IsTrue(dondozo.Has(SpecialCondition.Confused));
            Assert.AreEqual(6, dondozo.ConfusionCounters);
        }

        [Test]
        public void Mimed_Games_Uses_The_Attack_The_Opponent_Chooses()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mimed Games"), new ScriptedAgent().Then("Heavy Splash"));
            e.ScenarioActive(0, C(MimeJr));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(12, dondozo.DamageCounters, "Heavy Splash: 120");
        }

        [Test]
        public void Mimed_Games_Does_Nothing_When_The_Opponent_Only_Has_Mimed_Games()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mimed Games"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(MimeJr));
            var theirs = e.ScenarioActive(1, C(MimeJr));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(0, mine.DamageCounters);
            Assert.AreEqual(0, theirs.DamageCounters, "they can't choose their own Mimed Games (Compendium)");
        }

        [Test]
        public void Assaulting_Hunt_Works_When_Switched_In_But_Not_When_Promoted()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Pawmi));
            var weavile = e.ScenarioBench(0, C(Weavile));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            var benched = e.ScenarioBench(1, C(Pawmi));
            e.ScenarioSetTurn(0, 3);

            e.State.Players[0].Active = null;
            e.State.Players[0].Bench.Remove(weavile);
            e.State.Players[0].Bench.Insert(0, weavile);
            e.PromoteActive(0);
            Assert.AreSame(weavile, e.State.Players[0].Active);
            Assert.AreSame(dondozo, e.State.Players[1].Active, "not after a promotion (Compendium)");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Pawmi));
            var weavile2 = e2.ScenarioBench(0, C(Weavile));
            e2.ScenarioActive(1, C(Dondozo));
            var benched2 = e2.ScenarioBench(1, C(Pawmi));
            e2.ScenarioSetTurn(0, 3);
            e2.SwitchActive(0, weavile2);
            Assert.AreSame(benched2, e2.State.Players[1].Active, "switching it in during your turn pulls up a Benched Basic");
            Assert.IsNotNull(benched);
        }

        [Test]
        public void Moonlit_Hill_Needs_Something_To_Heal()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pawmi = e.ScenarioActive(0, C(Pawmi));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetStadium(1, C(MoonlitHill));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);

            Assert.IsFalse(Offered(e, 0, "Use Moonlit Hill"), "discarding the Energy is a cost (Compendium)");
            pawmi.DamageCounters = 4;
            Assert.IsTrue(Offered(e, 0, "Use Moonlit Hill"));
        }

        [Test]
        public void Dominating_Echo_Stops_Special_Energy_And_Stadiums_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dominating Echo"), new ScriptedAgent());
            var noivern = e.ScenarioActive(0, C(Noivern));
            e.ScenarioAttach(noivern, C(Psychic));
            e.ScenarioAttach(noivern, C(Darkness));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioAddToHand(1, C(MistEnergy));
            e.ScenarioAddToHand(1, C(Artazon));
            e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Attach Mist Energy"));
            Assert.IsFalse(Offered(e, 1, "Play Artazon"));
            Assert.IsTrue(Offered(e, 1, "Attach Water Energy"), "Basic Energy is fine");
        }

        // ================================================================== Trainers

        [Test]
        public void Technical_Machine_Crisis_Punch_Needs_Exactly_1_Prize_Left()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dondozo = e.ScenarioActive(0, C(Dondozo));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(dondozo, C(Psychic));
            e.ScenarioAttachTool(dondozo, C(CrisisPunch));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Assert.IsFalse(Offered(e, 0, "Attack: Crisis Punch"));
            e.State.Players[1].Prizes.RemoveRange(0, 5);
            Assert.IsTrue(Offered(e, 0, "Attack: Crisis Punch"));
        }

        [Test]
        public void Paldean_Student_Searches_1_More_For_Each_Other_Paldean_Student_In_The_Discard()
        {
            var agent = new ScriptedAgent().Then("Play Paldean Student", "Ralts|Kirlia");
            var e = Scenario(agent, new ScriptedAgent());
            e.ScenarioActive(0, C(Pawmi));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioAddToHand(0, C(PaldeanStudent));
            e.ScenarioAddToDiscard(0, C(PaldeanStudent), 2);
            e.ScenarioAddToDeck(0, C(Ralts), 1);
            e.ScenarioAddToDeck(0, C(Kirlia), 1);
            e.ScenarioAddToDeck(0, C(Gardevoir), 1);
            e.ScenarioAddToDeck(0, C(Pawmi), 1);
            e.ScenarioAddToDeck(0, C(Exeggcute), 1);
            e.ScenarioSetTurn(0, 3);

            Step(e);
            var search = agent.Seen.Last(r => r.Kind != DecisionKind.MainAction);
            Assert.AreEqual(3, search.MaxChoices, "1 + 2 Paldean Students in the discard pile");
            Assert.IsFalse(search.Options.Any(o => o.Label.Contains("Gardevoir ex")), "Pokémon ex have a Rule Box");
            var hand = e.State.Players[0].Hand.Select(c => c.Def.Name).ToList();
            CollectionAssert.Contains(hand, "Ralts");
            CollectionAssert.Contains(hand, "Kirlia");
        }
    }
}
