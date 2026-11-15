using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v154: the Japanese Storm Emeralda (M6) and ex Starter Set Zorua &amp; Zoroark ex (MEZ) cards: their own set codes with the
    /// Japanese numbers (your choice), the prints, and the new effects. v157: Delta Reign is out in English, so they're prints of
    /// the Delta Reign cards, with the English names.
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
        const string Giratina = "DLR-45";
        const string Talonflame = "DLR-88";
        const string RiceBall = "DLR-101";
        const string Cap = "DLR-100";
        const string Zoroark = "DLR-67";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Storm_Emeralda_And_The_Zoroark_Starter_Cards_Are_Prints_Of_Delta_Reign()
        {
            Assert.AreEqual(0, db.Cards.Count(d => d.SetCode == "M6" || d.SetCode == "MEZ"), "every one is out in English now");
            var jp = new[]
            {
                new[] { "M6-29", "DLR-43" }, new[] { "M6-31", "DLR-45" }, new[] { "M6-32", "DLR-46" }, new[] { "M6-47", "DLR-68" },
                new[] { "M6-85", "DLR-68" }, new[] { "M6-60", "DLR-86" }, new[] { "M6-61", "DLR-87" }, new[] { "M6-62", "DLR-88" },
                new[] { "M6-96", "DLR-88" }, new[] { "M6-63", "DLR-101" }, new[] { "M6-66", "DLR-100" }, new[] { "MEZ-3", "DLR-63" },
                new[] { "MEZ-7", "DLR-67" }, new[] { "M6-104", "POR-86" }, new[] { "M6-105", "CRI-86" }, new[] { "MEZ-6", "DLR-112" },
                new[] { "MEZ-20", "DLR-112" },
            };
            foreach (var pair in jp)
            {
                Assert.AreEqual(pair[1], db.MainPrinting(C(pair[0])).Key, pair[0]);
                StringAssert.Contains("Japan", C(pair[0]).PrintSetName, pair[0]);
                StringAssert.DoesNotContain("not yet in English", C(pair[0]).PrintSetName, pair[0]);
            }
            Assert.AreEqual("Tasty Rice Ball", C("M6-63").Name, "the English name");
            Assert.IsTrue(C(Talonflame).IsEx && C(Zoroark).IsEx);
            Assert.AreEqual("Fletchinder", C(Talonflame).EvolvesFrom);
            Assert.IsTrue(C(Cap).IsTool && C(RiceBall).IsItem);
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
        public void Excited_Dive_Benches_Talonflame_ex_With_A_Colorless_Mega_Evolution_ex()
        {
            foreach (bool mega in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Excited Dive"), new ScriptedAgent());
                e.ScenarioActive(0, C(mega ? MegaRayquaza : Chansey));
                e.ScenarioAddToHand(0, C(Talonflame));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(mega, Offered(e, 0, "Use Excited Dive"));
                if (!mega) continue;
                Step(e);
                Assert.AreEqual(1, e.State.Players[0].Bench.Count(m => m.Name == "Talonflame ex"));
            }
        }

        [Test]
        public void Tasty_Rice_Ball_Heals_30_More_For_Each_Other_In_The_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Tasty Rice Ball"), new ScriptedAgent());
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
            var e = Scenario(new ScriptedAgent().Then("Attack: Delta Gift", "Lightning Energy|Lightning Energy"), new ScriptedAgent());
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
        public void Riotous_Beating_Counts_Your_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Riotous Beating"), new ScriptedAgent());
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
