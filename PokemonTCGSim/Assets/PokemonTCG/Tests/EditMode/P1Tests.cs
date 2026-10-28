using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v163: POP Series 1 (P1): the card data, Armaldo ex's second Weakness, Insomnia and Focus Energy.</summary>
    public class P1Tests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Fire = "MEE-10";

        const string Murkrow = "P1-8";
        const string Surskit = "P1-14";
        const string Swellow = "P1-15";
        const string ArmaldoEx = "P1-16";
        const string TyranitarEx = "P1-17";
        const string Chansey = "E2-69";

        [Test]
        public void Pop_Series_1_Is_In_The_Pool()
        {
            Assert.AreEqual(17, db.Cards.Count(d => d.SetCode == "P1"));
            Assert.AreEqual(EnergyType.Water, C(ArmaldoEx).SecondWeakness);
            Assert.AreEqual(EnergyType.Fighting, C(TyranitarEx).SecondWeakness);
            Assert.IsTrue(C(TyranitarEx).IsOldEx);
        }

        [Test]
        public void Armaldo_ex_Is_Weak_To_Water_Too()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bubble"), new ScriptedAgent());
            var surskit = e.ScenarioActive(0, C(Surskit));
            e.ScenarioAttach(surskit, C(Water));
            var armaldo = e.ScenarioActive(1, C(ArmaldoEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, armaldo.DamageCounters, "10 x 2");
        }

        [Test]
        public void Insomnia_Keeps_Murkrow_Awake()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var murkrow = e.ScenarioActive(0, C(Murkrow));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(murkrow, SpecialCondition.Asleep);
            Assert.IsFalse(murkrow.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Focus_Energy_Makes_Agility_Do_70_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Focus Energy", "Attack: Agility"), new ScriptedAgent());
            var swellow = e.ScenarioActive(0, C(Swellow));
            e.ScenarioAttach(swellow, C(Fire));
            e.ScenarioAttach(swellow, C(Fire));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.DamageCounters);
            e.ScenarioSetTurn(0, 5);
            Step(e);
            Assert.AreEqual(7, chansey.DamageCounters);
        }
    }
}
