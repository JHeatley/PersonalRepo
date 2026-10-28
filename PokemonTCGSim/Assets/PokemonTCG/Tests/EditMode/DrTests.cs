using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v159: Dragon (DR): the card data and prints, 2 Resistances, Toxic Gas (Poké-Powers and Poké-Bodies off), Conductivity,
    /// Wonder Guard, Buffer Piece and Mr. Briney's Compassion.
    /// </summary>
    public class DrTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Fighting = "MEE-14";

        const string Altaria = "DR-2";
        const string Shedinja = "DR-11";
        const string Graveler = "DR-30";
        const string Geodude = "DR-55";
        const string Buffer = "DR-83";
        const string Briney = "DR-87";
        const string Ampharos = "DR-89";
        const string Muk = "DR-96";
        const string Lunatone = "SS-8";
        const string Solrock = "SS-13";
        const string Kecleon = "SS-18";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Dragon_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "DR").ToList();
            Assert.AreEqual(97, set.Count, "100 cards minus 3 prints");
            Assert.AreEqual(100, db.All.Count(d => d.SetCode == "DR"));
            Assert.AreEqual("N3-60", db.MainPrinting(db.Get("DR-82")).Key);
            Assert.AreEqual("CES-128", db.MainPrinting(db.Get("DR-84")).Key);
            Assert.AreEqual("CES-149", db.MainPrinting(db.Get("DR-88")).Key);
            Assert.AreEqual(EnergyType.Grass, C(Altaria).Resistance);
            Assert.AreEqual(EnergyType.Fighting, C(Altaria).SecondResistance);
            Assert.IsTrue(C(Muk).IsOldEx);
            Assert.IsTrue(C(Buffer).IsTool);
        }

        [Test]
        public void The_Second_Resistance_Takes_30_Off()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rollout"), new ScriptedAgent());
            var graveler = e.ScenarioActive(0, C(Graveler));
            Attach(e, graveler, Fighting, 3);
            var altaria = e.ScenarioActive(1, C(Altaria));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, altaria.DamageCounters, "40 - 30 for the Fighting Resistance");
        }

        [Test]
        public void Toxic_Gas_Turns_Off_Poke_Powers_And_Poke_Bodies_While_Muk_ex_Is_Active()
        {
            foreach (bool active in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Lunatone));
                var kecleon = e.ScenarioBench(0, C(Kecleon));
                Attach(e, kecleon, Fire, 1);
                e.ScenarioBench(0, C(Solrock));
                if (active) e.ScenarioActive(1, C(Muk));
                else
                {
                    e.ScenarioActive(1, C(Chansey));
                    e.ScenarioBench(1, C(Muk));
                }
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!active, Offered(e, 0, "Use Lunar Eclipse"), "a Poké-Power");
                Assert.AreEqual(!active, e.IsType(kecleon, EnergyType.Fire), "a Poké-Body (Energy Variation)");
            }
        }

        [Test]
        public void Conductivity_Puts_1_Counter_On_A_Pokemon_The_Opponent_Attaches_Energy_To()
        {
            foreach (int count in new[] { 1, 2 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attach Fire Energy to Chansey"), new ScriptedAgent());
                var chansey = e.ScenarioActive(0, C(Chansey));
                e.ScenarioAddToHand(0, C(Fire));
                e.ScenarioActive(1, C(Ampharos));
                if (count == 2) e.ScenarioBench(1, C(Ampharos));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(1, chansey.Energy.Count);
                Assert.AreEqual(1, chansey.DamageCounters, "only 1 even with 2 Ampharos ex");
            }
        }

        [Test]
        public void Wonder_Guard_Stops_Evolved_Pokemon_But_Not_Basic_Ones()
        {
            foreach (bool evolved in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then(evolved ? "Attack: Rollout" : "Attack: Light Punch"), new ScriptedAgent());
                var mine = e.ScenarioActive(0, C(Geodude));
                if (evolved) e.ScenarioEvolve(mine, C(Graveler));
                Attach(e, mine, Fighting, 3);
                var shedinja = e.ScenarioActive(1, C(Shedinja));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(evolved ? 0 : 1, shedinja.DamageCounters);
            }
        }

        [Test]
        public void Buffer_Piece_Takes_20_Off_And_Goes_At_The_End_Of_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rollout"), new ScriptedAgent());
            var graveler = e.ScenarioActive(0, C(Graveler));
            Attach(e, graveler, Fighting, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAttachTool(chansey, C(Buffer));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, chansey.DamageCounters, "40 x 2 for Chansey's Fighting Weakness, then 20 less");
            e.EndTurn();
            Assert.AreEqual(0, chansey.Tools.Count);
        }

        [Test]
        public void Mr_Brineys_Compassion_Returns_A_Pokemon_But_Not_A_Pokemon_ex()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mr. Briney's Compassion"), new ScriptedAgent());
            e.ScenarioActive(0, C(Muk));
            var geodude = e.ScenarioBench(0, C(Geodude));
            Attach(e, geodude, Fighting, 1);
            e.ScenarioAddToHand(0, C(Briney));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual("Muk ex", e.State.Players[0].Active.Name);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Geodude and its Energy");
        }
    }
}
