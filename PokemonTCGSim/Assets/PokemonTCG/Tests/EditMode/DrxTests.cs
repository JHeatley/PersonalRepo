using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v125: Dragons Exalted (DRX 1-128): the prints, Damage Swap (your ruling: it may Knock Out), Bouffer, Empty Shell, Rescue Scarf,
    /// Giant Cape, Electromagnetic Wall, Unobservant, Ho-Oh-EX's Rebirth, Triple Laser, Blend Energy, Smogscreen and Consume.
    /// </summary>
    public class DrxTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string Darkness = "MEE-15";

        const string Vulpix = "DRX-18";
        const string HoOh = "DRX-22";
        const string Ampharos = "DRX-40";
        const string Shedinja = "DRX-48";
        const string Skuntank = "DRX-77";
        const string Registeel = "DRX-81";
        const string Hydreigon = "DRX-98";
        const string Slaking = "DRX-103";
        const string Bouffalant = "DRX-110";
        const string GiantCape = "DRX-114";
        const string RescueScarf = "DRX-115";
        const string BlendGRPD = "DRX-117";
        const string BlendWLFM = "DRX-118";
        const string Reuniclus = "DRX-126";
        const string Rattata = "G1-82";
        const string Spearow = "G1-52";
        const string Lickitung = "G1-41";
        const string Onix = "G1-69";
        const string KyuremLtr = "LTR-43";

        private static int SeedFor(params bool[] flips)
        {
            for (int seed = 1; seed < 100000; seed++)
            {
                var rng = new Rng(seed);
                bool match = true;
                foreach (var heads in flips)
                    if (rng.CoinFlip() != heads) { match = false; break; }
                if (match) return seed;
            }
            throw new InvalidOperationException("no seed for those flips");
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(103, db.Cards.Count(d => d.SetCode == "DRX"));
            Assert.AreEqual("LTR-49", C("DRX-45").BaseKey, "Emolga");
            Assert.AreEqual("XYP-126", C("DRX-46").BaseKey, "Mew-EX");
            Assert.AreEqual("30C-CC19", C("DRX-85").BaseKey, "Rayquaza-EX");
            Assert.AreEqual("LTR-99", C("DRX-97").BaseKey, "Hydreigon");
            Assert.AreEqual("EVO-76", C("DRX-113").BaseKey, "Devolution Spray");
            Assert.AreEqual("ASC-212", C("DRX-116").BaseKey, "Tool Scrapper");
            Assert.AreEqual(HoOh, C("DRX-119").BaseKey, "the full art Ho-Oh-EX");
            Assert.AreEqual("LTR-93", C("DRX-128").BaseKey, "Rayquaza");
            Assert.IsTrue(C(HoOh).IsPokemonEX);
            Assert.IsFalse(C(HoOh).UnlimitedOnly, "Expanded");
            Assert.IsFalse(C(Shedinja).Weakness.HasValue);
        }

        [Test]
        public void Damage_Swap_Can_Knock_Out_Your_Own_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Damage Swap", "Reuniclus", "Lt. Surge's Rattata"), new ScriptedAgent());
            var reuniclus = e.ScenarioActive(0, C(Reuniclus));
            reuniclus.DamageCounters = 3;
            var rattata = e.ScenarioBench(0, C(Rattata));
            rattata.DamageCounters = 3;
            e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, reuniclus.DamageCounters);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Key == Rattata), "moved onto it until it was Knocked Out (your ruling)");
        }

        [Test]
        public void Bouffer_Takes_20_Less_From_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Throw"), new ScriptedAgent());
            var onix = e.ScenarioActive(0, C(Onix));
            Attach(e, onix, Fighting, 3);
            var bouffalant = e.ScenarioActive(1, C(Bouffalant));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, bouffalant.DamageCounters, "30, doubled by Weakness, minus 20");
        }

        [Test]
        public void Empty_Shell_Gives_No_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var shedinja = e.ScenarioActive(1, C(Shedinja));
            shedinja.DamageCounters = 5;
            e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            int prizes = e.State.Players[0].Prizes.Count;
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Key == Shedinja), "Knocked Out");
            Assert.AreEqual(prizes, e.State.Players[0].Prizes.Count, "no Prize card taken");
        }

        [Test]
        public void Rescue_Scarf_Returns_The_Knocked_Out_Pokemon_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var rattata = e.ScenarioActive(1, C(Rattata));
            rattata.DamageCounters = 3;
            e.ScenarioAttachTool(rattata, C(RescueScarf));
            e.ScenarioBench(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Key == Rattata), "back in the hand");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Key == RescueScarf), "the Tool is discarded");
        }

        [Test]
        public void Giant_Cape_Adds_20_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(40, e.MaxHp(rattata));
            e.ScenarioAttachTool(rattata, C(GiantCape));
            Assert.AreEqual(60, e.MaxHp(rattata));
        }

        [Test]
        public void Electromagnetic_Wall_Hurts_Pokemon_That_Get_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Ampharos));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, rattata.Energy.Count);
            Assert.AreEqual(3, rattata.DamageCounters);
        }

        [Test]
        public void Unobservant_Stops_Slaking_Against_A_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slaking = e.ScenarioActive(0, C(Slaking));
            Attach(e, slaking, Water, 4);
            e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Attack: Crushing Blow"), "Lickitung is a Basic Pokémon");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slaking2 = e2.ScenarioActive(0, C(Slaking));
            Attach(e2, slaking2, Water, 4);
            e2.ScenarioActive(1, C(Reuniclus));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e2.GetMainActions(0).Any(o => o.Label == "Attack: Crushing Blow"), "an evolved Pokémon");
        }

        [Test]
        public void Ho_Oh_EX_Comes_Back_From_The_Discard_Pile_With_3_Types_Of_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ho-Oh-EX's Rebirth from your discard pile"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToDiscard(0, C(HoOh), 1);
            e.ScenarioAddToDiscard(0, C(Fire), 2);
            e.ScenarioAddToDiscard(0, C(Water), 1);
            e.ScenarioAddToDiscard(0, C(Lightning), 1);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            var hoOh = e.State.Players[0].Bench.FirstOrDefault(m => m.Def.Key == HoOh);
            Assert.IsNotNull(hoOh, "on the Bench");
            Assert.AreEqual(3, hoOh.Energy.Count);
            Assert.AreEqual(3, hoOh.Energy.Select(c => c.Def.EnergyProvides).Distinct().Count(), "3 different types");
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Rebirth")));
        }

        [Test]
        public void Triple_Laser_Hits_3_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Triple Laser"), new ScriptedAgent());
            var registeel = e.ScenarioActive(0, C(Registeel));
            Attach(e, registeel, Metal, 3);
            var active = e.ScenarioActive(1, C(Lickitung));
            var b1 = e.ScenarioBench(1, C(Rattata));
            var b2 = e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, active.DamageCounters);
            Assert.AreEqual(3, b1.DamageCounters);
            Assert.AreEqual(3, b2.DamageCounters);
        }

        [Test]
        public void Blend_Energy_Provides_One_Of_Its_Four_Types()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var vulpix = e.ScenarioActive(0, C(Vulpix));
            e.ScenarioAttach(vulpix, C(BlendGRPD));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Attack: Singe"), "Fire from Blend Energy GRPD");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var vulpix2 = e2.ScenarioActive(0, C(Vulpix));
            e2.ScenarioAttach(vulpix2, C(BlendWLFM));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label == "Attack: Singe"), "no Fire in Blend Energy WLFM");
        }

        [Test]
        public void Smogscreen_Poisons_And_Consume_Heals()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Smogscreen"), new ScriptedAgent());
            var skuntank = e.ScenarioActive(0, C(Skuntank));
            Attach(e, skuntank, Darkness, 1);
            var kyurem = e.ScenarioActive(1, C(KyuremLtr));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(kyurem.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(e.State.Effects.Any(x => x is CardsMEG.SandAttackEffect && x.TargetUid == kyurem.Uid));

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Consume"), new ScriptedAgent());
            var hydreigon = e2.ScenarioActive(0, C(Hydreigon));
            Attach(e2, hydreigon, Darkness, 3);
            hydreigon.DamageCounters = 6;
            e2.ScenarioActive(1, C(KyuremLtr));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(2, hydreigon.DamageCounters, "healed the 40 it did");
        }
    }
}
