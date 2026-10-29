using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v169: Delta Species (DS): the card data and prints, δ Pokémon, Holon's Pokémon attached as Energy, Holon Energy, Holon Research
    /// Tower, the Holon Supporters' discard cost, and the Compendium rulings on them.
    /// </summary>
    public class DsTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Grass = "MEE-9";
        const string BasicMetal = "MEE-16";

        const string Beedrill = "DS-1";
        const string Rayquaza = "DS-13";
        const string Umbreon = "DS-17";
        const string HolonsElectrode = "DS-21";
        const string HolonsVoltorb = "DS-71";
        const string HolonMentor = "DS-93";
        const string HolonResearchTower = "DS-94";
        const string HolonScientist = "DS-97";
        const string HolonEnergyGL = "DS-105";
        const string HolonEnergyWP = "DS-106";
        const string Chansey = "E2-69";
        const string CacturneEx = "EM-91";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Delta_Species_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(105, db.Cards.Count(d => d.SetCode == "DS"), "114 cards minus 9 prints");
            Assert.AreEqual(114, db.All.Count(d => d.SetCode == "DS"));
            Assert.AreEqual("MA-72", db.MainPrinting(db.Get("DS-89")).Key, "Dual Ball: Team Magma vs Team Aqua's (Baby Pokémon allowed)");
            Assert.AreEqual("EM-86", db.MainPrinting(db.Get("DS-103")).Key, "the special Darkness Energy");
            Assert.AreEqual("N1-19", db.MainPrinting(db.Get("DS-107")).Key, "the special Metal Energy");
            Assert.AreEqual("30C-CC11", db.MainPrinting(db.Get("DS-11")).Key);
            var beedrill = C(Beedrill);
            Assert.AreEqual("Beedrill", beedrill.Name, "the δ isn't part of the name");
            Assert.IsTrue(beedrill.IsDeltaSpecies);
            Assert.AreEqual(EnergyType.Grass, beedrill.PokemonType);
            Assert.AreEqual(EnergyType.Metal, beedrill.SecondType);
            Assert.IsTrue(C(HolonsVoltorb).AttachesAsEnergy);
            Assert.IsFalse(C(Chansey).IsDeltaSpecies);
        }

        [Test]
        public void Holons_Voltorb_Is_The_Energy_Attachment_For_The_Turn_And_Provides_Colorless()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Holon's Voltorb as Energy"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(HolonsVoltorb));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Bench Holon's Voltorb"), "it's still a Basic Pokémon too");
            Step(e);
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.IsTrue(chansey.Energy[0].Def.IsAttachedSpecialEnergy, "a Special Energy card while attached");
            CollectionAssert.AreEqual(new[] { EnergyMask.Colorless }, e.EnergyUnitsOf(chansey, chansey.Energy[0]));
            Assert.IsFalse(Offered(e, 0, "Attach Fire Energy"), "the attachment for the turn is used");
        }

        [Test]
        public void Holons_Electrode_Needs_An_Energy_Returns_One_And_Provides_2_Of_Every_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Holon's Electrode as Energy"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            var bare = e.ScenarioBench(0, C(Chansey));
            Attach(e, chansey, Fire, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(HolonsElectrode));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Attach Holon's Electrode") && o.PokemonUid == bare.Uid), "only onto a Pokémon with an Energy card");
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.AreEqual("Holon's Electrode", chansey.Energy[0].Def.Name);
            Assert.AreEqual(Fire, me.Hand.Single().Def.Key, "the Fire Energy went back to the hand");
            CollectionAssert.AreEqual(new[] { EnergyMask.AnyType, EnergyMask.AnyType }, e.EnergyUnitsOf(chansey, chansey.Energy[0]));
        }

        [Test]
        public void Cursed_Glare_Stops_A_Holons_Pokemon_Being_Attached_To_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(CacturneEx));
            e.ScenarioAddToHand(0, C(HolonsVoltorb));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Holon's Voltorb"), "Compendium 2006-10-05: it's a Special Energy card when attached");
        }

        [Test]
        public void Lightning_Storm_Puts_7_Counters_On_Rayquaza_Unless_A_Holon_Energy_Is_Attached()
        {
            foreach (string extra in new[] { Lightning, HolonEnergyGL, HolonsVoltorb })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Lightning Storm"), new ScriptedAgent());
                var rayquaza = e.ScenarioActive(0, C(Rayquaza));
                Attach(e, rayquaza, Lightning, 1);
                Attach(e, rayquaza, BasicMetal, 2);
                e.ScenarioAttach(rayquaza, C(extra));
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(extra == HolonEnergyGL ? 0 : 7, rayquaza.DamageCounters,
                    extra == HolonsVoltorb ? "a Holon's Pokémon isn't a Holon Energy (Compendium 2005-11-03)" : extra);
                Assert.AreEqual(7, chansey.DamageCounters, "the damage is still done");
            }
        }

        [Test]
        public void Holon_Research_Tower_Makes_Basic_Energy_On_Delta_Pokemon_Provide_Metal_Too()
        {
            foreach (bool tower in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var rayquaza = e.ScenarioActive(0, C(Rayquaza));
                Attach(e, rayquaza, Lightning, 4);
                e.ScenarioActive(1, C(Chansey));
                if (tower) e.ScenarioSetStadium(0, C(HolonResearchTower));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(tower, Offered(e, 0, "Lightning Storm"), "Lightning Storm costs [L][M][C][C]");
            }
        }

        [Test]
        public void Holon_Energy_WP_With_Water_Stops_Effects_Of_Attacks_But_Not_Damage()
        {
            foreach (bool water in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Psybeam"), new ScriptedAgent());
                var cacturne = e.ScenarioActive(0, C(CacturneEx));
                Attach(e, cacturne, Grass, 1);
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioAttach(chansey, C(HolonEnergyWP));
                if (water) Attach(e, chansey, Water, 1);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(!water, chansey.Has(SpecialCondition.Confused));
                Assert.AreEqual(2, chansey.DamageCounters, "damage isn't an effect it stops");
            }
        }

        [Test]
        public void Holon_Energy_GL_With_Lightning_Takes_10_Off_Damage_From_Pokemon_ex_And_With_Grass_Stops_Conditions()
        {
            foreach (string second in new[] { Fire, Lightning, Grass })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Psybeam"), new ScriptedAgent());
                var cacturne = e.ScenarioActive(0, C(CacturneEx));
                Attach(e, cacturne, Grass, 1);
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioAttach(chansey, C(HolonEnergyGL));
                Attach(e, chansey, second, 1);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(second == Lightning ? 1 : 2, chansey.DamageCounters, second);
                Assert.AreEqual(second != Grass, chansey.Has(SpecialCondition.Confused), second);
            }
        }

        [Test]
        public void Holon_Supporters_Need_Another_Card_To_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Holon Mentor", "Chansey|Chansey|Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(0, C(Chansey), 3);
            e.ScenarioAddToHand(0, C(HolonMentor));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Holon Mentor"), "nothing else in the hand to discard");
            e.ScenarioAddToHand(0, C(Fire));
            Assert.IsTrue(Offered(e, 0, "Play Holon Mentor"));
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(Fire, me.Discard.First(c => c.Def.IsEnergy).Def.Key, "the Fire Energy paid the cost");
            Assert.AreEqual(3, me.Hand.Count(c => c.Def.Name == "Chansey"), "3 Basic Pokémon with 100 HP or less");
        }

        [Test]
        public void Holon_Scientist_Can_t_Be_Played_Unless_It_Draws_After_The_Discard()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(0, C(Chansey), 5);
            e.ScenarioAddToHand(0, C(HolonScientist));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Holon Scientist"), "0 cards after the discard, 1 in the opponent's hand");
            e.ScenarioAddToHand(0, C(Fire));
            Assert.IsFalse(Offered(e, 0, "Play Holon Scientist"), "1 card after the discard: nothing to draw (Compendium 2007-01-04)");
        }

        [Test]
        public void Delta_Moon_Puts_A_Counter_When_The_Opponent_Attaches_Special_Energy_Once_A_Turn()
        {
            foreach (string energy in new[] { Fire, HolonEnergyGL, HolonsVoltorb })
            {
                var e = Scenario(new ScriptedAgent().Then("Attach"), new ScriptedAgent());
                var chansey = e.ScenarioActive(0, C(Chansey));
                e.ScenarioActive(1, C(Umbreon));
                e.ScenarioBench(1, C(Umbreon));
                e.ScenarioAddToHand(0, C(energy));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(energy == Fire ? 0 : 1, chansey.DamageCounters, energy + ": 2 Umbreon don't add up");
            }
        }
    }
}
