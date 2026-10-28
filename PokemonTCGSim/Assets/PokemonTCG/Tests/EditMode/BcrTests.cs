using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v112: Boundaries Crossed (BCR 1-153): Allergy Panic, Shell Shield, Stickiness, Time Recall, Dream Waltz (attacking while
    /// Asleep), Sturdy, Sentinel, Busybody, Transform, Sand Slammer, Crystal Wall, Aspertia City Gym, Hugh, Gold Potion and
    /// Devolution Punch.
    /// </summary>
    public class BcrTests : ScenarioTestBase
    {
        const string Fighting = "MEE-14";
        const string Psychic = "MEE-13";

        const string Oddish = "BCR-1";
        const string Gloom = "BCR-2";
        const string Vileplume = "BCR-3";
        const string CelebiEX = "BCR-9";
        const string Squirtle = "BCR-29";
        const string Frillish = "BCR-44";
        const string Jellicent = "BCR-45";
        const string Munna = "BCR-68";
        const string Musharna = "BCR-69";
        const string Dwebble = "BCR-84";
        const string Crustle = "BCR-85";
        const string Mienfoo = "BCR-87";
        const string Trapinch = "BCR-83";
        const string Vibrava = "BCR-98";
        const string Flygon = "BCR-99";
        const string Rattata = "BCR-104";
        const string Raticate = "BCR-105";
        const string Ditto = "BCR-108";
        const string Snorlax = "BCR-109";
        const string Patrat = "BCR-118";
        const string Lillipup = "BCR-120";
        const string Herdier = "BCR-121";
        const string Stoutland = "BCR-122";
        const string Audino = "BCR-126";
        const string Aspertia = "BCR-127";
        const string Hugh = "BCR-130";
        const string ComputerSearch = "BCR-137";
        const string CrystalWall = "BCR-139";
        const string GoldPotion = "BCR-140";
        const string Golurk = "BCR-150";
        const string BlackKyuremEX = "LTR-100";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(122, db.Cards.Count(d => d.SetCode == "BCR"));
            Assert.AreEqual(CelebiEX, C("BCR-141").BaseKey, "full-art Celebi-EX");
            Assert.AreEqual("LTR-19", C("BCR-20").BaseKey, "Charizard");
            Assert.AreEqual(BlackKyuremEX, C("BCR-101").BaseKey);
            Assert.IsTrue(C(ComputerSearch).IsAceSpec && C(CrystalWall).IsAceSpec && C(GoldPotion).IsAceSpec);
            Assert.IsFalse(C(Patrat).IsTeamPlasma);
        }

        [Test]
        public void Allergy_Panic_Makes_Weakness_Times_4()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mienfoo = e.ScenarioActive(0, C(Mienfoo));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mienfoo, snorlax, 20);
            Assert.AreEqual(4, snorlax.DamageCounters, "×2");
            var vileplume = e.ScenarioBench(1, C(Oddish));
            e.ScenarioEvolve(vileplume, C(Gloom));
            e.ScenarioEvolve(vileplume, C(Vileplume));
            e.DealAttackDamage(mienfoo, snorlax, 10);
            Assert.AreEqual(8, snorlax.DamageCounters, "×4 with Vileplume in play (either side's)");
        }

        [Test]
        public void Shell_Shield_Protects_Squirtle_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mienfoo = e.ScenarioActive(0, C(Mienfoo));
            e.ScenarioActive(1, C(Patrat));
            var squirtle = e.ScenarioBench(1, C(Squirtle));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mienfoo, squirtle, 20, false);
            Assert.AreEqual(0, squirtle.DamageCounters);
        }

        [Test]
        public void Stickiness_Raises_The_Opponents_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var jellicent = e.ScenarioActive(0, C(Frillish));
            e.ScenarioEvolve(jellicent, C(Jellicent));
            var raticate = e.ScenarioActive(1, C(Rattata));
            e.ScenarioEvolve(raticate, C(Raticate));
            var patrat = e.ScenarioBench(1, C(Patrat));
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(1, e.RetreatCostOf(raticate), "a printed 0 goes up (2012-11-08)");
            Assert.AreEqual(2, e.RetreatCostOf(patrat));
            Assert.AreEqual(3, e.RetreatCostOf(jellicent), "not its owner's");
        }

        [Test]
        public void Time_Recall_Gives_Previous_Evolutions_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var herdier = e.ScenarioActive(0, C(Lillipup));
            e.ScenarioEvolve(herdier, C(Herdier));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(herdier, C(Fighting));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.UsableAttacks(herdier).Any(a => a.Attack.Name == "Roar"));
            e.ScenarioBench(0, C(CelebiEX));
            Assert.IsTrue(e.UsableAttacks(herdier).Any(a => a.Attack.Name == "Roar"), "Lillipup's attack");
        }

        [Test]
        public void Dream_Waltz_Can_Be_Used_While_Asleep()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var musharna = e.ScenarioActive(0, C(Munna));
            e.ScenarioEvolve(musharna, C(Musharna));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(musharna, C(Psychic));
            e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(musharna, SpecialCondition.Asleep);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Contains("Attack: Dream Waltz"));
            Assert.IsFalse(labels.Contains("Attack: Telekinesis"));
            e.ApplyCondition(musharna, SpecialCondition.Paralyzed);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack:")), "not while Paralyzed");
        }

        [Test]
        public void Sturdy_Survives_A_Knock_Out_From_Full_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mienfoo = e.ScenarioActive(0, C(Mienfoo));
            var crustle = e.ScenarioActive(1, C(Dwebble));
            e.ScenarioEvolve(crustle, C(Crustle));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mienfoo, crustle, 200, false);
            Assert.AreEqual(9, crustle.DamageCounters, "10 HP left");
        }

        [Test]
        public void Sentinel_Stops_The_Opponents_Supporters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Stoutland));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioAddToHand(1, C(Hugh));
            e.ScenarioAddToDeck(1, C(Fighting), 10);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label == "Play Hugh"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Patrat));
            e2.ScenarioBench(0, C(Stoutland));
            e2.ScenarioActive(1, C(Patrat));
            e2.ScenarioAddToHand(1, C(Hugh));
            e2.ScenarioAddToDeck(1, C(Fighting), 10);
            e2.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e2.GetMainActions(1).Any(o => o.Label == "Play Hugh"), "Stoutland on the Bench");
        }

        [Test]
        public void Busybody_Heals_And_Cures_The_Active_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Busybody"), new ScriptedAgent());
            var patrat = e.ScenarioActive(0, C(Patrat));
            patrat.DamageCounters = 3;
            e.ApplyCondition(patrat, SpecialCondition.Poisoned);
            e.ScenarioAddToHand(0, C(Audino));
            e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, patrat.DamageCounters);
            Assert.IsFalse(patrat.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Audino"));
        }

        [Test]
        public void Transform_Puts_A_Basic_On_Ditto_Keeping_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Transform"), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(Ditto));
            ditto.DamageCounters = 2;
            e.ScenarioAttach(ditto, C(Fighting));
            e.ScenarioAddToHand(0, C(Snorlax));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Snorlax", ditto.Name);
            Assert.AreEqual(2, ditto.DamageCounters);
            Assert.AreEqual(1, ditto.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Sand_Slammer_Hits_Each_Opposing_Pokemon_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var flygon = e.ScenarioActive(0, C(Trapinch));
            e.ScenarioEvolve(flygon, C(Vibrava));
            e.ScenarioEvolve(flygon, C(Flygon));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            var patrat = e.ScenarioBench(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, snorlax.DamageCounters);
            Assert.AreEqual(1, patrat.DamageCounters);
            Assert.AreEqual(0, flygon.DamageCounters);
        }

        [Test]
        public void Crystal_Wall_And_Aspertia_City_Gym_Change_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C(BlackKyuremEX));
            var patrat = e.ScenarioBench(0, C(Patrat));
            e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(180, e.MaxHp(kyurem));
            e.ScenarioAttachTool(kyurem, C(CrystalWall));
            Assert.AreEqual(300, e.MaxHp(kyurem));
            Assert.AreEqual(60, e.MaxHp(patrat));
            e.ScenarioSetStadium(1, C(Aspertia));
            Assert.AreEqual(80, e.MaxHp(patrat), "a Colorless Pokémon");
            Assert.AreEqual(300, e.MaxHp(kyurem), "a Dragon Pokémon");
        }

        [Test]
        public void Hugh_Sets_Both_Hands_To_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hugh"), new ScriptedAgent());
            e.ScenarioActive(0, C(Patrat));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioAddToHand(0, C(Hugh));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioAddToDeck(0, C(Fighting), 10);
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Fighting));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(5, e.State.Players[1].Hand.Count);
            Assert.AreEqual(2, e.State.Players[1].Discard.Count);
        }

        [Test]
        public void Gold_Potion_Heals_90_From_The_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Gold Potion"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            snorlax.DamageCounters = 10;
            e.ScenarioAddToHand(0, C(GoldPotion));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, snorlax.DamageCounters);
        }

        [Test]
        public void Devolution_Punch_Sends_The_Evolution_Card_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Devolution Punch"), new ScriptedAgent());
            var golurk = e.ScenarioActive(0, C(Golurk));
            e.ScenarioAttach(golurk, C(Psychic));
            e.ScenarioAttach(golurk, C(Fighting));
            e.ScenarioAttach(golurk, C(Fighting));
            var stoutland = e.ScenarioActive(1, C(Lillipup));
            e.ScenarioEvolve(stoutland, C(Herdier));
            e.ScenarioEvolve(stoutland, C(Stoutland));
            e.ScenarioBench(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Herdier", stoutland.Name);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Stoutland"));
            Assert.AreEqual(6, stoutland.DamageCounters, "the damage first (2012-08-23)");
        }
    }
}
