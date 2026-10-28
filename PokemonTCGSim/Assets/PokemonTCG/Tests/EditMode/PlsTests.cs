using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v111: Plasma Storm (PLS 1-138): Dual Brains, Plasma Steel, Eviolite, Virbank City Gym with Ultra-Toxic Fang and Hypnotoxic
    /// Laser, Plasma Frigate, Block, Bright Veil (with Escape Rope), Victory Piece, Dual Armor, Silent Claw, Damakinesis, Colress
    /// Machine and Moon Guidance (your rulings for the last two attacks/Abilities).
    /// </summary>
    public class PlsTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string PlasmaEnergy = "PLB-91";

        const string Crobat = "PLS-55";
        const string Magnezone = "PLS-46";
        const string Zapdos = "PLS-48";
        const string Beheeyem = "PLS-70";
        const string Riolu = "PLS-75";
        const string LucarioDual = "PLS-78";
        const string Lucario = "PLS-77";
        const string Liepard = "PLS-84";
        const string Klinklang = "PLS-90";
        const string Durant = "PLS-91";
        const string Cobalion = "PLS-93";
        const string BlackKyurem = "PLS-95";
        const string Clefable = "PLS-98";
        const string Snorlax = "PLS-101";
        const string Togekiss = "PLS-104";
        const string Patrat = "PLS-110";
        const string Colress = "PLS-118";
        const string ColressMachine = "PLS-119";
        const string Eviolite = "PLS-122";
        const string Laser = "PLS-123";
        const string Frigate = "PLS-124";
        const string Virbank = "PLS-126";
        const string DowsingMachine = "PLS-128";
        const string VictoryPiece = "PLS-130";
        const string VictiniEX = "LTR-24";
        const string LugiaEX = "LTR-102";
        const string EscapeRope = "BST-125";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(125, db.Cards.Count(d => d.SetCode == "PLS"));
            Assert.AreEqual(VictiniEX, C("PLS-18").BaseKey);
            Assert.AreEqual(VictiniEX, C("PLS-131").BaseKey, "full-art Victini-EX");
            Assert.AreEqual(LugiaEX, C("PLS-108").BaseKey);
            Assert.AreEqual(PlasmaEnergy, C("PLS-127").BaseKey);
            Assert.AreEqual(Colress, C("PLS-135").BaseKey);
            Assert.IsTrue(C(Colress).IsTeamPlasma && C(Laser).IsTeamPlasma);
            Assert.IsTrue(C(DowsingMachine).IsAceSpec && C(VictoryPiece).IsAceSpec);
            Assert.IsFalse(C(Patrat).IsTeamPlasma);
        }

        [Test]
        public void Dual_Brains_Allows_2_Supporters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Magnezone));
            e.ScenarioBench(0, C(Magnezone));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioAddToHand(0, C(Colress));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.SupporterLimitFor(0), "2 in all, however many Magnezone");
            Assert.AreEqual(1, e.SupporterLimitFor(1));
            var me = e.State.Players[0];
            me.SupporterPlayedThisTurn = true;
            me.SupportersPlayedCount = 1;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play Colress"));
            me.SupportersPlayedCount = 2;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Colress"));
        }

        [Test]
        public void Plasma_Steel_Stops_Pokemon_EX_Damage_To_Metal_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var cobalion = e.ScenarioActive(0, C(Cobalion));
            e.ScenarioBench(0, C(Klinklang));
            var patrat = e.ScenarioBench(0, C(Patrat));
            var kyurem = e.ScenarioActive(1, C(BlackKyurem));
            var snorlax = e.ScenarioBench(1, C(Snorlax));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(kyurem, cobalion, 60);
            Assert.AreEqual(0, cobalion.DamageCounters, "a Pokémon-EX against a Metal Pokémon");
            e.DealAttackDamage(snorlax, cobalion, 60);
            Assert.AreEqual(6, cobalion.DamageCounters, "not a Pokémon-EX");
            e.DealAttackDamage(kyurem, patrat, 30, false);
            Assert.AreEqual(3, patrat.DamageCounters, "not a Metal Pokémon");
        }

        [Test]
        public void Eviolite_Protects_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var patrat = e.ScenarioActive(0, C(Patrat));
            e.ScenarioAttachTool(patrat, C(Eviolite));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(snorlax, patrat, 40);
            Assert.AreEqual(2, patrat.DamageCounters);
        }

        [Test]
        public void Virbank_City_Gym_Adds_2_And_Hypnotoxic_Laser_Starts_A_New_Poison()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hypnotoxic Laser"), new ScriptedAgent());
            e.ScenarioActive(0, C(Patrat));
            var target = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetStadium(0, C(Virbank));
            e.ScenarioAddToHand(0, C(Laser));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(target, SpecialCondition.Poisoned);
            target.PoisonCounterBase = 4;
            Assert.AreEqual(6, e.PoisonCounters(target), "Ultra-Toxic Fang's 4 + 2 (2013-02-28)");
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(3, e.PoisonCounters(target), "a new single-counter Poison (2013-02-21) + 2");
        }

        [Test]
        public void Plasma_Frigate_Removes_Weakness_With_Plasma_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zapdos = e.ScenarioActive(0, C(Zapdos));
            var lugia = e.ScenarioActive(1, C(LugiaEX));
            e.ScenarioAttach(lugia, C(PlasmaEnergy));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(zapdos, lugia, 30);
            Assert.AreEqual(6, lugia.DamageCounters, "Lightning Weakness");
            e.ScenarioSetStadium(1, C(Frigate));
            e.DealAttackDamage(zapdos, lugia, 30);
            Assert.AreEqual(9, lugia.DamageCounters, "no Weakness on Plasma Frigate");
        }

        [Test]
        public void Block_Stops_The_Opponents_Active_From_Retreating()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            var patrat = e.ScenarioActive(1, C(Patrat));
            e.ScenarioAttach(patrat, C(Fighting));
            e.ScenarioBench(1, C(Patrat));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.CanRetreat(e.State.Players[1]));
            e.State.Players[0].Active = null;
            e.State.Players[0].Bench.Add(snorlax);
            e.ScenarioActive(0, C(Patrat));
            Assert.IsTrue(e.CanRetreat(e.State.Players[1]), "Snorlax on the Bench");
        }

        [Test]
        public void Bright_Veil_Stops_The_Opponents_Items()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Play Hypnotoxic Laser"));
            var togekiss = e.ScenarioActive(0, C(Togekiss));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioAddToHand(1, C(Laser));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsFalse(togekiss.Has(SpecialCondition.Poisoned));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Play Escape Rope"));
            var veiled = e2.ScenarioActive(0, C(Togekiss));
            e2.ScenarioBench(0, C(Patrat));
            var theirs = e2.ScenarioActive(1, C(Patrat));
            e2.ScenarioBench(1, C(Snorlax));
            e2.ScenarioAddToHand(1, C(EscapeRope));
            e2.ScenarioSetTurn(1, 4);
            Step(e2);
            Assert.AreSame(veiled, e2.State.Players[0].Active, "Bright Veil stops its owner's part of the switch (2013-02-07)");
            Assert.AreNotSame(theirs, e2.State.Players[1].Active);
        }

        [Test]
        public void Victory_Piece_Frees_Victini_EX_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var victini = e.ScenarioActive(0, C(VictiniEX));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            var burn = victini.Def.Attacks.First(a => a.Name == "Intensifying Burn");
            Assert.AreEqual(3, e.AttackCost(victini, burn).Count);
            e.ScenarioAttachTool(victini, C(VictoryPiece));
            Assert.AreEqual(0, e.AttackCost(victini, burn).Count);
        }

        [Test]
        public void Dual_Armor_Makes_Lucario_Metal()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lucario = e.ScenarioActive(0, C(Riolu));
            e.ScenarioEvolve(lucario, C(LucarioDual));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.IsType(lucario, EnergyType.Metal));
            e.ScenarioAttach(lucario, C(Metal));
            Assert.IsTrue(e.IsType(lucario, EnergyType.Metal));
            Assert.IsTrue(e.IsType(lucario, EnergyType.Fighting));
        }

        [Test]
        public void Silent_Claw_Uses_The_Discarded_Supporters_Effect()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Silent Claw"), new ScriptedAgent());
            var liepard = e.ScenarioActive(0, C(Liepard));
            e.ScenarioAttach(liepard, C(Fighting));
            e.ScenarioAddToHand(0, C(Patrat));
            e.ScenarioAddToHand(0, C(Patrat));
            e.ScenarioAddToDeck(0, C(Fighting), 10);
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioBench(1, C(Patrat));
            e.ScenarioBench(1, C(Patrat));
            e.ScenarioAddToHand(1, C(Colress));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Colress"));
            Assert.AreEqual(0, e.State.Players[1].Hand.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Colress: a card for each of the 2 Benched Pokémon");
        }

        [Test]
        public void Damakinesis_Moves_6_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Damakinesis"), new ScriptedAgent());
            var beheeyem = e.ScenarioActive(0, C(Beheeyem));
            e.ScenarioAttach(beheeyem, C(Psychic));
            e.ScenarioAttach(beheeyem, C(Fighting));
            e.ScenarioAttach(beheeyem, C(Fighting));
            beheeyem.DamageCounters = 4;
            var patrat = e.ScenarioBench(0, C(Patrat));
            patrat.DamageCounters = 3;
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioBench(1, C(Patrat));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, snorlax.DamageCounters);
            Assert.AreEqual(1, beheeyem.DamageCounters + patrat.DamageCounters);
        }

        [Test]
        public void Colress_Machine_Attaches_Plasma_Energy_To_A_Team_Plasma_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Colress Machine"), new ScriptedAgent());
            var durant = e.ScenarioActive(0, C(Durant));
            e.ScenarioActive(1, C(Patrat));
            e.ScenarioAddToDeck(0, C(PlasmaEnergy), 1);
            e.ScenarioAddToDeck(0, C(Fighting), 3);
            e.ScenarioAddToHand(0, C(ColressMachine));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, durant.Energy.Count);
            Assert.AreEqual("Plasma Energy", durant.Energy[0].Def.Name);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Patrat));
            e2.ScenarioActive(1, C(Patrat));
            e2.ScenarioAddToDeck(0, C(PlasmaEnergy), 1);
            e2.ScenarioAddToHand(0, C(ColressMachine));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.StartsWith("Play Colress Machine")), "no Team Plasma Pokémon");
        }

        [Test]
        public void Moon_Guidance_Evolves_From_The_Deck_On_Heads_Even_On_The_First_Turn()
        {
            int evolved = 0, missed = 0;
            for (int i = 0; i < 30; i++)
            {
                var e = Scenario(new ScriptedAgent().Then("Moon Guidance"), new ScriptedAgent());
                e.ScenarioActive(0, C(Clefable));
                var riolu = e.ScenarioBench(0, C(Riolu));
                e.ScenarioActive(1, C(Patrat));
                e.ScenarioAddToDeck(0, C(Lucario), 1);
                e.ScenarioAddToDeck(0, C(Fighting), 3);
                e.ScenarioSetTurn(0, 1);
                Step(e);
                if (riolu.Name == "Lucario") evolved++;
                else missed++;
            }
            Assert.Greater(evolved, 0, "your ruling (v111): no timing limits");
            Assert.Greater(missed, 0, "tails");
        }
    }
}
