using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v170: Holon Phantoms (HP): the card data and prints, δ Rainbow Energy, Holon Lake's attack, Delta Reactor, Dual Aura, Delta
    /// Reserve, Delta Block, Holon Adventurer and Mr. Stone's Project.
    /// </summary>
    public class HpTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";

        const string GyaradosDelta = "HP-8";
        const string LatiasDelta = "HP-11";
        const string LatiosDelta = "HP-12";
        const string PidgeotDelta = "HP-14";
        const string GolduckDelta = "HP-43";
        const string Pikachu = "HP-78";
        const string PikachuDelta = "HP-79";
        const string HolonAdventurer = "HP-85";
        const string HolonLake = "HP-87";
        const string MrStonesProject = "HP-88";
        const string DeltaRainbow = "HP-98";
        const string HolonEnergyGL = "DS-105";
        const string Gorebyss = "LM-17";
        const string Clamperl = "LM-51";
        const string Huntail = "LM-18";
        const string Kecleon = "LM-37";
        const string React = "LM-82";
        const string FullFlame = "LM-74";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Holon_Phantoms_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(96, db.Cards.Count(d => d.SetCode == "HP"), "111 cards minus 15 prints");
            Assert.AreEqual(111, db.All.Count(d => d.SetCode == "HP"));
            Assert.AreEqual("SS-90", db.MainPrinting(db.Get("HP-91")).Key);
            Assert.AreEqual("LM-79", db.MainPrinting(db.Get("HP-92")).Key, "the 50-HP Mysterious Fossil");
            Assert.AreEqual("SS-92", db.MainPrinting(db.Get("HP-93")).Key);
            Assert.AreEqual("EM-86", db.MainPrinting(db.Get("HP-94")).Key, "the special Darkness Energy");
            Assert.AreEqual("N1-19", db.MainPrinting(db.Get("HP-95")).Key, "the special Metal Energy");
            Assert.AreEqual("SS-93", db.MainPrinting(db.Get("HP-96")).Key);
            Assert.AreEqual("HP-88", db.MainPrinting(db.Get(MrStonesProject)).Key, "basic Energy only, unlike Emerald's");
            Assert.AreEqual("Delta Call", C(HolonLake).Attacks.Single().Name);
            Assert.AreEqual("δ Rainbow Energy", C(DeltaRainbow).Name);
            Assert.IsTrue(C(PikachuDelta).IsDeltaSpecies);
        }

        [Test]
        public void Delta_Rainbow_Energy_Provides_Every_Type_Only_On_Delta_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var delta = e.ScenarioActive(0, C(PikachuDelta));
            var plain = e.ScenarioBench(0, C(Pikachu));
            e.ScenarioAttach(delta, C(DeltaRainbow));
            e.ScenarioAttach(plain, C(DeltaRainbow));
            e.ScenarioActive(1, C(Kecleon));
            CollectionAssert.AreEqual(new[] { EnergyMask.AnyType }, e.EnergyUnitsOf(delta, delta.Energy[0]));
            CollectionAssert.AreEqual(new[] { EnergyMask.Colorless }, e.EnergyUnitsOf(plain, plain.Energy[0]));
        }

        [Test]
        public void Holon_Lake_Gives_Delta_Pokemon_Delta_Call()
        {
            foreach (string active in new[] { PikachuDelta, Pikachu })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var mon = e.ScenarioActive(0, C(active));
                e.ScenarioAttach(mon, C(Psychic));
                e.ScenarioActive(1, C(Kecleon));
                e.ScenarioSetStadium(0, C(HolonLake));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(active == PikachuDelta, Offered(e, 0, "Delta Call"), active);
            }
        }

        [Test]
        public void Delta_Reactor_Adds_10_To_Delta_Pokemon_With_A_Holon_Stadium()
        {
            foreach (bool lake in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Tail Whap"), new ScriptedAgent());
                var pikachu = e.ScenarioActive(0, C(PikachuDelta));
                e.ScenarioAttach(pikachu, C(Psychic));
                e.ScenarioBench(0, C(GyaradosDelta));
                var target = e.ScenarioActive(1, C(Kecleon));
                if (lake) e.ScenarioSetStadium(0, C(HolonLake));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(lake ? 2 : 1, target.DamageCounters);
            }
        }

        [Test]
        public void Dual_Aura_Stops_Evolved_Pokemons_Poke_Bodies_With_Its_Partner_In_Play()
        {
            foreach (bool partner in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(LatiasDelta));
                if (partner) e.ScenarioBench(0, C(LatiosDelta));
                var gorebyss = e.ScenarioActive(1, C(Clamperl));
                e.ScenarioEvolve(gorebyss, C(Gorebyss)); // an Evolved Pokémon
                var huntail = e.ScenarioBench(1, C(Huntail));
                e.ScenarioAttach(huntail, C(React));
                Assert.AreEqual(partner, e.AbilityBlocked(gorebyss, "Reactive Booster"));
                CollectionAssert.AreEqual(partner ? new[] { EnergyMask.Colorless } : new[] { EnergyMask.AnyType, EnergyMask.AnyType }, e.EnergyUnitsOf(huntail, huntail.Energy[0]));
            }
        }

        [Test]
        public void Delta_Reserve_Stops_Poke_Powers_Of_Pokemon_Without_Delta()
        {
            foreach (bool holon in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Huntail));
                e.ScenarioAddToDeck(0, C(React), 1);
                var pidgeot = e.ScenarioActive(1, C(PidgeotDelta));
                if (holon) e.ScenarioAttach(pidgeot, C(HolonEnergyGL));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!holon, Offered(e, 0, "Use Reactive Generator"));
            }
        }

        [Test]
        public void Delta_Block_Stops_The_Opponents_Stadiums_While_A_Holon_Stadium_Is_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Kecleon));
            e.ScenarioActive(1, C(GolduckDelta));
            e.ScenarioAddToHand(0, C(FullFlame));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Full Flame"), "no Holon Stadium yet");
            e.ScenarioSetStadium(1, C(HolonLake));
            Assert.IsFalse(Offered(e, 0, "Play Full Flame"));
        }

        [Test]
        public void Holon_Adventurer_Draws_4_After_Discarding_A_Delta_Pokemon()
        {
            foreach (string discard in new[] { PikachuDelta, Pikachu })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Holon Adventurer"), new ScriptedAgent());
                e.ScenarioActive(0, C(Kecleon));
                e.ScenarioActive(1, C(Kecleon));
                e.ScenarioAddToHand(0, C(HolonAdventurer));
                e.ScenarioAddToHand(0, C(discard));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                var me = e.State.Players[0];
                Assert.AreEqual(discard == PikachuDelta ? 4 : 3, me.Hand.Count);
                Assert.IsTrue(me.Discard.Any(c => c.Def.Key == discard));
            }
        }

        [Test]
        public void Mr_Stones_Project_Takes_Only_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mr. Stone's Project", "Your discard pile", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Kecleon));
            e.ScenarioActive(1, C(Kecleon));
            e.ScenarioAddToHand(0, C(MrStonesProject));
            e.ScenarioAddToDiscard(0, C(Pikachu), 1);
            e.ScenarioAddToDiscard(0, C(Psychic), 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Hand.Count(c => c.Def.Key == Psychic));
            Assert.IsFalse(me.Hand.Any(c => c.Def.Key == Pikachu));
        }
    }
}
