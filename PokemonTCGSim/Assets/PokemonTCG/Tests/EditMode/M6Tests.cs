using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v154: the Japanese Storm Emeralda (M6) and ex Starter Set Zorua &amp; Zoroark ex (MEZ) cards: their own set codes with the
    /// Japanese numbers (your choice), the prints, and the new effects.
    /// </summary>
    public class M6Tests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Chansey = "E2-69";
        const string MegaDragonite = "MEP-91";
        const string MegaRayquaza = "DLR-84";
        const string Giratina = "M6-31";
        const string Talonflame = "M6-62";
        const string RiceBall = "M6-63";
        const string Cap = "M6-66";
        const string Zoroark = "MEZ-7";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Storm_Emeralda_And_The_Zoroark_Starter_Are_Japanese_Sets_With_Prints()
        {
            Assert.AreEqual(9, db.Cards.Count(d => d.SetCode == "M6"));
            Assert.AreEqual(2, db.Cards.Count(d => d.SetCode == "MEZ"));
            Assert.IsTrue(C(Giratina).PrintSetName.Contains("Japan"));
            Assert.IsTrue(C(Zoroark).PrintSetName.Contains("Japan"));
            Assert.IsTrue(C(Talonflame).IsEx && C(Zoroark).IsEx);
            Assert.AreEqual("Fletchinder", C(Talonflame).EvolvesFrom);
            Assert.IsTrue(C(Cap).IsTool && C(RiceBall).IsItem);
            Assert.AreEqual("M6-47", db.MainPrinting(C("M6-85")).Key);
            Assert.AreEqual("M6-62", db.MainPrinting(C("M6-96")).Key);
            Assert.AreEqual("POR-86", db.MainPrinting(C("M6-104")).Key, "Grow Grass Energy");
            Assert.AreEqual("CRI-86", db.MainPrinting(C("M6-105")).Key, "Nitro Fire Energy");
            Assert.AreEqual("DLR-112", db.MainPrinting(C("MEZ-6")).Key, "same Zorua as Delta Reign's");
            Assert.AreEqual("DLR-112", db.MainPrinting(C("MEZ-20")).Key);
        }

        [Test]
        public void Chaos_Crawler_Prevents_Damage_And_Cant_Be_Used_Two_Turns_In_A_Row()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chaos Crawler"), new ScriptedAgent());
            var giratina = e.ScenarioActive(0, C(Giratina));
            Attach(e, giratina, Psychic, 3);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Chaos Crawler"));
            Step(e);
            Assert.AreEqual(12, dragonite.DamageCounters);
            Assert.IsTrue(e.State.Effects.Any(x => x is PreventDamageEffect && x.TargetUid == giratina.Uid));
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Assert.IsFalse(Offered(e, 0, "Attack: Chaos Crawler"), "used during your last turn");
        }

        [Test]
        public void Excite_Dive_Benches_Talonflame_ex_With_A_Colorless_Mega_Evolution_ex()
        {
            foreach (bool mega in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Excite Dive"), new ScriptedAgent());
                e.ScenarioActive(0, C(mega ? MegaRayquaza : Chansey));
                e.ScenarioAddToHand(0, C(Talonflame));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(mega, Offered(e, 0, "Use Excite Dive"));
                if (!mega) continue;
                Step(e);
                Assert.AreEqual(1, e.State.Players[0].Bench.Count(m => m.Name == "Talonflame ex"));
            }
        }

        [Test]
        public void Delicious_Rice_Ball_Heals_30_More_For_Each_Other_In_The_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Delicious Rice Ball"), new ScriptedAgent());
            var dragonite = e.ScenarioActive(0, C(MegaDragonite));
            dragonite.DamageCounters = 10;
            e.ScenarioAddToDiscard(0, C(RiceBall), 2);
            e.ScenarioAddToHand(0, C(RiceBall));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, dragonite.DamageCounters, "30 + 2 x 30");
        }

        [Test]
        public void Delta_Gift_Attaches_A_Basic_Energy_To_Each_Pokemon_With_A_Cap()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Delta Gift"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            Attach(e, active, Fire, 1);
            e.ScenarioAttachTool(active, C(Cap));
            var capped = e.ScenarioBench(0, C(Chansey));
            e.ScenarioAttachTool(capped, C(Cap));
            var plain = e.ScenarioBench(0, C(Chansey));
            e.ScenarioAddToDeck(0, C(Lightning), 3);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, active.Energy.Count);
            Assert.AreEqual(1, capped.Energy.Count);
            Assert.AreEqual(0, plain.Energy.Count);
            Assert.AreEqual(1, active.Tools.Count, "the Cap stays attached");
        }

        [Test]
        public void Riot_Beat_Counts_Your_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Riot Beat"), new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C(Zoroark));
            Attach(e, zoroark, Darkness, 1);
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioBench(0, C(Chansey));
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, dragonite.DamageCounters, "3 Pokémon x 20");
        }

        [Test]
        public void Mega_Rayquaza_Cap_Shows_Its_Attack_In_The_Card_Text()
        {
            string text = PokemonTCG.Presentation.CardText.Describe(C(Cap));
            StringAssert.Contains("Delta Gift", text, "v168.1: a Tool's own attack is listed after its text");
            StringAssert.Contains("Basic Energy card", text);
            string seal = PokemonTCG.Presentation.CardText.Describe(C("SIT-154"));
            Assert.AreEqual(1, seal.Split(new[] { "Star Gravity" }, System.StringSplitOptions.None).Length - 1, "not twice when the text already has it");
        }
    }
}
