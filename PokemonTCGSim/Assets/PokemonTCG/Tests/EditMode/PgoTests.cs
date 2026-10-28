using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v52: Pokémon GO (PGO): the Symbols (your ruling: they stack), Burn Brightly, Vitality Spring, Reassuring Dam, Egg
    /// Incubator going to the bottom of the deck, and the other cards with new effects.
    /// </summary>
    public class PgoTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";

        const string Tauros = "MEW-128";
        const string Pikachu = "MEW-25";
        const string MawileV = "SIT-70";
        const string CorviknightVmax = "SIT-TG19";
        const string Litwick = "LOR-24";
        const string ArcPhone = "LOR-152";

        const string Charmander = "PGO-8";
        const string Charizard = "PGO-10";
        const string Moltres = "PGO-12";
        const string Blastoise = "PGO-17";
        const string RadiantBlastoise = "PGO-18";
        const string Slowbro = "PGO-20";
        const string Wimpod = "PGO-25";
        const string Xatu = "PGO-33";
        const string ConkeldurrV = "PGO-40";
        const string AlolanRaticate = "PGO-42";
        const string MelmetalVmax = "PGO-48";
        const string DragoniteVstar = "PGO-50";
        const string Snorlax = "PGO-55";
        const string SlakingV = "PGO-58";
        const string Bibarel = "PGO-60";
        const string EggIncubator = "PGO-66";
        const string LureModule = "PGO-67";
        const string PokeStop = "PGO-68";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        // ================================================================== Data

        [Test]
        public void Every_Pokemon_GO_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 88; n++) Assert.IsNotNull(db.Get("PGO-" + n), "PGO " + n + " is missing");
            Assert.AreEqual("CRZ-20", db.MainPrinting(C("PGO-11")).Key, "Radiant Charizard was already in the pool");
            Assert.AreEqual("CRZ-GG44", db.MainPrinting(C("PGO-31")).Key, "Mewtwo VSTAR too");
            Assert.AreEqual("PGO-5", db.MainPrinting(C("PGO-71")).Key, "the full art is a second print of Alolan Exeggutor V");
            Assert.IsTrue(C(Charizard).Abilities[0].Text.Contains("2 Fire Energy"));
            Assert.IsTrue(C(DragoniteVstar).Attacks[1].VstarPower, "Draconic Star");
            Assert.IsFalse(Formats.IsStandardCard(C(Charizard)), "regulation mark F: Expanded");
        }

        // ================================================================== Symbols (your ruling)

        [Test]
        public void Flare_Symbols_Stack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tail on Fire"), new ScriptedAgent());
            var charmander = e.ScenarioActive(0, C(Charmander));
            Attach(e, charmander, Fire, 1);
            e.ScenarioBench(0, C(Moltres));
            e.ScenarioBench(0, C(Moltres));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters, "10 + 10 for each Moltres (your ruling: they stack)");
        }

        [Test]
        public void Flare_Symbol_Skips_Moltres()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fire Wing"), new ScriptedAgent());
            var moltres = e.ScenarioActive(0, C(Moltres));
            Attach(e, moltres, Fire, 3);
            e.ScenarioBench(0, C(Moltres));
            var mawile = e.ScenarioActive(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(11, mawile.DamageCounters, "\"except any Moltres\"");
        }

        // ================================================================== Energy

        [Test]
        public void Burn_Brightly_Makes_Basic_Fire_Energy_Provide_2_And_Doesnt_Stack()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var moltres = e.ScenarioActive(0, C(Moltres));
            Attach(e, moltres, Fire, 1);
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(1, e.EnergyCount(moltres));
            e.ScenarioBench(0, C(Charizard));
            Assert.AreEqual(2, e.EnergyCount(moltres));
            e.ScenarioBench(0, C(Charizard));
            Assert.AreEqual(2, e.EnergyCount(moltres), "only 1 Burn Brightly applies");
        }

        [Test]
        public void Vitality_Spring_Attaches_Energy_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Vitality Spring", "Psychic|Psychic|Psychic"), new ScriptedAgent());
            var blastoise = e.ScenarioActive(0, C(Blastoise));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "your turn ends");
            Assert.AreEqual(3, blastoise.Energy.Count);
            Assert.AreEqual(17, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void G_Max_Juggernaut_Counts_Extra_Metal_Energy_Up_To_120()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: G-Max Juggernaut"), new ScriptedAgent());
            var melmetal = e.ScenarioActive(0, C(MelmetalVmax));
            Attach(e, melmetal, Metal, 6);
            var corviknight = e.ScenarioActive(1, C(CorviknightVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(28, corviknight.DamageCounters, "160 + 120 at most");
        }

        [Test]
        public void Draconic_Star_Attaches_Water_And_Lightning_Energy_From_The_Top_12()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Draconic Star"), new ScriptedAgent());
            var dragonite = e.ScenarioActive(0, C(DragoniteVstar));
            Attach(e, dragonite, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            var deck = e.State.Players[0].Deck;
            deck.Insert(0, e.NewCard(0, C(Water)));
            deck.Insert(3, e.NewCard(0, C(Lightning)));
            deck.Insert(8, e.NewCard(0, C(Water)));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, dragonite.Energy.Count);
            Assert.AreEqual(20, deck.Count);
            Assert.IsTrue(e.State.Players[0].VstarUsed);
        }

        // ================================================================== Abilities

        [Test]
        public void Pump_Shot_Discards_A_Water_Energy_For_2_Counters_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Pump Shot"), new ScriptedAgent());
            e.ScenarioActive(0, C(RadiantBlastoise));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            var pikachu = e.ScenarioBench(1, C(Pikachu));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(2, pikachu.DamageCounters);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.AreEqual("Water Energy", e.State.Players[0].Discard.Single().Def.Name);
        }

        [Test]
        public void Kinda_Lazy_Stops_Slaking_V_With_2_4_Or_6_Prize_Cards_Left()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slaking = e.ScenarioActive(0, C(SlakingV));
            Attach(e, slaking, Psychic, 4);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Heavy Impact"), "6 Prize cards left");
            e.State.Players[0].Prizes.RemoveAt(0);
            Assert.IsTrue(Offered(e, 0, "Attack: Heavy Impact"), "5 Prize cards left");
        }

        [Test]
        public void Block_Stops_The_Opponents_Active_Pokemon_Retreating()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            var spare = e.ScenarioBench(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Pikachu));
            Assert.IsTrue(e.RetreatPrevented(tauros));
            Assert.IsFalse(e.RetreatPrevented(snorlax), "only the opponent's");
            e.SwitchActive(0, spare);
            Assert.IsFalse(e.RetreatPrevented(tauros), "only while Snorlax is Active");
        }

        [Test]
        public void Reassuring_Dam_Protects_The_Deck_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kindling Panic"), new ScriptedAgent());
            var litwick = e.ScenarioActive(0, C(Litwick));
            Attach(e, litwick, Fire, 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Bibarel));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(20, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Reassuring_Dam_Only_Works_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kindling Panic"), new ScriptedAgent());
            var litwick = e.ScenarioActive(0, C(Litwick));
            Attach(e, litwick, Fire, 1);
            e.ScenarioActive(1, C(Bibarel));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Punk_Out_Removes_Wimpods_Retreat_Cost_Against_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wimpod = e.ScenarioActive(0, C(Wimpod));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(3, e.RetreatCostOf(wimpod));
            e.ScenarioBench(1, C(MawileV));
            Assert.AreEqual(0, e.RetreatCostOf(wimpod));
        }

        // ================================================================== Attacks

        [Test]
        public void Twilight_Inspiration_Takes_2_Prize_Cards_When_The_Opponent_Has_1_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Twilight Inspiration"), new ScriptedAgent());
            var slowbro = e.ScenarioActive(0, C(Slowbro));
            Attach(e, slowbro, Psychic, 2);
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[1].Prizes.RemoveRange(0, 4);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Twilight Inspiration"), "2 Prize cards left");
            e.State.Players[1].Prizes.RemoveAt(0);
            Assert.IsTrue(Offered(e, 0, "Attack: Twilight Inspiration"));
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Counter_Adds_The_Damage_Taken_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Counter"), new ScriptedAgent());
            var conkeldurr = e.ScenarioActive(0, C(ConkeldurrV));
            Attach(e, conkeldurr, Fighting, 1);
            conkeldurr.DamageCounters = 10;
            conkeldurr.AttackDamageTurn = 2;
            conkeldurr.AttackDamageThatTurn = 100;
            var mawile = e.ScenarioActive(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, mawile.DamageCounters, "20 + 100");
        }

        [Test]
        public void Pinpoint_Wave_Hits_A_Pokemon_V_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pinpoint Wave"), new ScriptedAgent());
            var xatu = e.ScenarioActive(0, C(Xatu));
            Attach(e, xatu, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            var mawile = e.ScenarioBench(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, mawile.DamageCounters);
            Assert.AreEqual(0, tauros.DamageCounters, "only a Pokémon V");
        }

        [Test]
        public void Super_Fang_Leaves_10_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Super Fang"), new ScriptedAgent());
            var raticate = e.ScenarioActive(0, C(AlolanRaticate));
            Attach(e, raticate, Psychic, 3);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, e.RemainingHp(tauros));
        }

        // ================================================================== Trainers

        [Test]
        public void Egg_Incubator_Benches_A_Basic_Or_Goes_To_The_Bottom_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Egg Incubator"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDeck(0, C(Pikachu), 1);
            e.ScenarioAddToHand(0, C(EggIncubator));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            if (p.Bench.Count == 1)
            {
                Assert.AreEqual("Pikachu", p.Bench[0].Name, "heads");
                Assert.AreEqual("Egg Incubator", p.Discard.Single().Def.Name);
            }
            else
            {
                Assert.AreEqual("Egg Incubator", p.Deck[p.Deck.Count - 1].Def.Name, "tails: the bottom of the deck");
                Assert.AreEqual(0, p.Discard.Count);
            }
        }

        [Test]
        public void Lure_Module_Gives_Both_Players_The_Pokemon_On_Top()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lure Module"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].Deck.Insert(1, e.NewCard(0, C(Pikachu)));
            e.State.Players[1].Deck.Insert(2, e.NewCard(1, C(Tauros)));
            e.ScenarioAddToHand(0, C(LureModule));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual("Pikachu", e.State.Players[0].Hand.Single().Def.Name);
            Assert.AreEqual("Tauros", e.State.Players[1].Hand.Single().Def.Name);
            Assert.AreEqual(20, e.State.Players[0].Deck.Count);
            Assert.AreEqual(20, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void PokeStop_Keeps_The_Items_It_Discards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use PokéStop"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            var deck = e.State.Players[0].Deck;
            deck.Insert(0, e.NewCard(0, C(ArcPhone)));
            deck.Insert(0, e.NewCard(0, C(EggIncubator)));
            e.ScenarioSetStadium(0, C(PokeStop));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            CollectionAssert.AreEquivalent(new[] { "Egg Incubator", "Arc Phone" }, p.Hand.Select(c => c.Def.Name).ToArray());
            Assert.AreEqual("Psychic Energy", p.Discard.Single().Def.Name);
            Assert.IsFalse(Offered(e, 0, "Use PokéStop"), "once during each player's turn");
        }
    }
}
