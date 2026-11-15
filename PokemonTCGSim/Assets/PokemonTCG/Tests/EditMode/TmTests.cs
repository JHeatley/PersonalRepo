using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: HS—Triumphant (TM): the card data and prints, and the new effects (Dittobolic, Heal Block, Rescue Energy, Junk Arm,
    /// Black Belt, Lost Link, Insight, Time Control, Indigo Plateau, Eye of Disaster).
    /// </summary>
    public class TmTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Triumphant_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(100, db.Cards.Count(d => d.SetCode == "TM"));
            Assert.AreEqual("30C-CC15", db.MainPrinting(C("TM-94")).Key, "Gengar");
            Assert.AreEqual("30C-CC16", db.MainPrinting(C("TM-99")).Key, "Darkrai & Cresselia LEGEND");
        }

        [Test]
        public void Every_Triumphant_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "TM" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Dittobolic_Limits_The_Opponents_Bench_To_4()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            for (int i = 0; i < 4; i++) e.ScenarioBench(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Chansey));
            e.ScenarioActive(1, C("TM-17"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Bench Chansey"));
        }

        [Test]
        public void Heal_Block_Stops_Healing_With_Lunatone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shed Skin"), new ScriptedAgent());
            var seviper = e.ScenarioActive(0, C("PL-61"));
            Attach(e, seviper, Psychic, 1);
            seviper.DamageCounters = 4;
            e.ScenarioActive(1, C("TM-9"));
            e.ScenarioBench(1, C("TM-25"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, seviper.DamageCounters);
        }

        [Test]
        public void Rescue_Energy_Returns_A_Knocked_Out_Pokemon_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.DamageCounters = e.MaxHp(chansey) / 10 - 1;
            Attach(e, chansey, "TM-90", 1);
            e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Chansey"));
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Rescue Energy"));
        }

        [Test]
        public void Junk_Arm_Gets_Back_A_Trainer_But_Not_Junk_Arm()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("TM-87"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToDiscard(0, C("TM-87"), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Junk Arm"), "only another Junk Arm in the discard pile");
            e.ScenarioAddToDiscard(0, C("PL-115"), 1);
            Assert.IsTrue(Offered(e, 0, "Play Junk Arm"));
        }

        [Test]
        public void Black_Belt_Adds_40_When_Behind()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Black Belt", "Attack: Super Psy Bolt"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioAddToHand(0, C("TM-85"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(7, chansey.DamageCounters, "30 + 40");
        }

        [Test]
        public void Lost_Link_Uses_Attacks_From_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C("TM-97"));
            Attach(e, mew, Psychic, 2);
            e.ScenarioActive(1, C(Chansey));
            e.State.Players[1].LostZone.Add(e.ScenarioAddToHand(1, C(MewEx)));
            e.State.Players[1].Hand.Clear();
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Super Psy Bolt"));
        }

        [Test]
        public void Insight_Makes_Attacks_Free_With_Equal_Hands()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var yanmega = e.ScenarioActive(0, C("TM-98"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            var sonic = yanmega.Def.Attacks.First(a => a.Name == "Sonicboom");
            Assert.IsTrue(e.CanPayAttack(yanmega, sonic));
            e.ScenarioAddToHand(0, C(Psychic));
            Assert.IsFalse(e.CanPayAttack(yanmega, sonic));
        }

        [Test]
        public void Time_Control_Adds_Prize_Cards_To_The_Opponent()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Time Control"), new ScriptedAgent());
            var legend = e.ScenarioActive(0, C("TM-101"));
            Attach(e, legend, "MEE-16", 2);
            Attach(e, legend, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, e.State.Players[1].Prizes.Count);
            Assert.AreEqual(1, legend.Energy.Count, "the Metal Energy was discarded");
        }

        [Test]
        public void Indigo_Plateau_Gives_Legend_Pokemon_30_Hp()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var legend = e.ScenarioActive(0, C("TM-101"));
            e.ScenarioActive(1, C(Chansey));
            int hp = e.MaxHp(legend);
            e.ScenarioSetStadium(0, C("TM-86"));
            Assert.AreEqual(hp + 30, e.MaxHp(legend));
        }

        [Test]
        public void Eye_Of_Disaster_Hurts_Basic_Pokemon_The_Opponent_Benches()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Bench Chansey"));
            e.ScenarioActive(0, C("TM-91"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Chansey));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, e.State.Players[1].Bench[0].DamageCounters);
        }
    }
}
