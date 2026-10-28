using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v100: Double Crisis (DCR 1-34): Team Magma and Team Aqua, Power Saver, the Secret Bases, Sludge Festival, Rock Stomp, the
    /// Tools with attacks, the Grunts and the Double Aqua / Magma Energy.
    /// </summary>
    public class DcrTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string HydreigonEX = "ROS-62";

        const string Numel = "DCR-1";
        const string Spheal = "DCR-3";
        const string Sealeo = "DCR-4";
        const string KyogreEX = "DCR-6";
        const string Grimer = "DCR-7";
        const string Muk = "DCR-8";
        const string Aron = "DCR-12";
        const string Aggron = "DCR-14";
        const string AquaPoochyena = "DCR-16";
        const string MagmaPoochyena = "DCR-17";
        const string Carvanha = "DCR-20";
        const string AquaDiffuser = "DCR-23";
        const string TeamAquaGrunt = "DCR-26";
        const string AquaSecretBase = "DCR-28";
        const string MagmaSecretBase = "DCR-32";
        const string DoubleAquaEnergy = "DCR-33";
        const string DoubleMagmaEnergy = "DCR-34";

        [Test]
        public void The_Set_Is_In_The_Pool()
        {
            var own = db.Cards.Where(d => d.SetCode == "DCR" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(34, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Aqua Diffuser", C(AquaDiffuser).Attacks[0].Name, "the Tool carries its attack");
        }

        [Test]
        public void Power_Saver_Needs_5_Team_Aqua_Pokemon_In_Play_Both_Sides()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kyogre = e.ScenarioActive(0, C(KyogreEX));
            e.ScenarioBench(0, C(Spheal));
            e.ScenarioBench(0, C(Grimer));
            e.ScenarioActive(1, C(Carvanha));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.AttacksPrevented(kyogre), "4 Team Aqua Pokémon");
            e.ScenarioBench(1, C(AquaPoochyena));
            Assert.IsFalse(e.AttacksPrevented(kyogre), "5, the opponent's count (2015-04-23)");
        }

        [Test]
        public void Sludge_Festival_And_Team_Aquas_Secret_Base_Raise_Retreat_Costs_Except_Team_Aqua()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var muk = e.ScenarioActive(0, C(Grimer));
            e.ScenarioEvolve(muk, C(Muk));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            int printed = C(SnorlaxGX).RetreatCost;
            Assert.AreEqual(printed + 1, e.RetreatCostOf(snorlax));
            Assert.AreEqual(C(LaprasGX).RetreatCost + 1, e.RetreatCostOf(lapras), "your own too");
            Assert.AreEqual(C(Muk).RetreatCost, e.RetreatCostOf(muk), "not Team Aqua Pokémon");
            e.ScenarioSetStadium(1, C(AquaSecretBase));
            Assert.AreEqual(printed + 2, e.RetreatCostOf(snorlax));
            Assert.AreEqual(C(Muk).RetreatCost, e.RetreatCostOf(muk));
        }

        [Test]
        public void Team_Magmas_Secret_Base_Hurts_Basic_Pokemon_Benched_From_The_Hand_Except_Team_Magma()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Lapras-GX", "Bench Team Magma's Aron"), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Aron));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetStadium(1, C(MagmaSecretBase));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(2, bench.First(m => m.Name == "Lapras-GX").DamageCounters);
            Assert.AreEqual(0, bench.First(m => m.Name == "Team Magma's Aron").DamageCounters);
        }

        [Test]
        public void Rock_Stomp_Counts_Double_Magma_Energy_As_2()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Stomp", "Fighting Energy|Double Magma Energy"), new ScriptedAgent());
            var aggron = e.ScenarioActive(0, C(Aron));
            e.ScenarioEvolve(aggron, C(Aggron));
            e.ScenarioAttach(aggron, C(Fighting));
            e.ScenarioAttach(aggron, C(DoubleMagmaEnergy));
            var hydreigon = e.ScenarioActive(1, C(HydreigonEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, hydreigon.DamageCounters, "40 x 3 (your ruling, v100)");
            Assert.AreEqual(0, aggron.Energy.Count);
        }

        [Test]
        public void Double_Aqua_Energy_Gives_2_Water_To_Team_Aqua_And_Goes_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var spheal = e.ScenarioActive(0, C(Spheal));
            e.ScenarioAttach(spheal, C(DoubleAquaEnergy));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.EnergyCountOfType(spheal, EnergyType.Water));
            var script = StandardCards.CreateRegistry().For(C(DoubleAquaEnergy));
            Assert.IsFalse(script.CanAttachTo(new EffectContext(e, 0, spheal.Energy[0], spheal), lapras), "Team Aqua Pokémon only");
            e.EndTurn();
            Assert.AreEqual(0, spheal.Energy.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Double Aqua Energy"));
        }

        [Test]
        public void Aqua_Diffuser_Gives_Its_Attack_To_A_Team_Aqua_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Aqua Diffuser"), new ScriptedAgent());
            var poochyena = e.ScenarioActive(0, C(AquaPoochyena));
            e.ScenarioAttachTool(poochyena, C(AquaDiffuser));
            e.ScenarioAttach(poochyena, C(Water));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(snorlax.Has(SpecialCondition.Confused));
            Assert.IsTrue(snorlax.Has(SpecialCondition.Poisoned));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var magma = e2.ScenarioActive(0, C(MagmaPoochyena));
            e2.ScenarioAttachTool(magma, C(AquaDiffuser));
            e2.ScenarioAttach(magma, C(Water));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.UsableAttacks(magma).Any(a => a.Source.Name == "Aqua Diffuser"), "Team Aqua Pokémon only");
        }

        [Test]
        public void Hail_Storm_Does_60_More_To_Team_Magma()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hail Storm"), new ScriptedAgent());
            var sealeo = e.ScenarioActive(0, C(Spheal));
            e.ScenarioEvolve(sealeo, C(Sealeo));
            e.ScenarioAttach(sealeo, C(Water));
            e.ScenarioAttach(sealeo, C(Psychic));
            e.ScenarioAttach(sealeo, C(Psychic));
            var numel = e.ScenarioActive(1, C(Numel));
            e.ScenarioBench(1, C(Numel));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(numel.Uid), "120 x2 (Water Weakness) Knocks Out the 70 HP Numel");
        }

        [Test]
        public void Team_Aqua_Grunt_Draws_4_After_Discarding_A_Team_Aqua_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Aqua Grunt", "Team Aqua's Spheal"), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(TeamAquaGrunt));
            e.ScenarioAddToHand(0, C(Spheal));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
        }
    }
}
