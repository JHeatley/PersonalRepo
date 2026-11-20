using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v171: Crystal Guardians (CG): the card data and prints, and the new effects (Holon Circle, Dark Eyes, Time Travel, Crystal
    /// Beach, Cessation Crystal, Mysterious Shard, Windstorm, Castaway, Intimidating Armor, Extra Liquid, Focus Energy, Even Game,
    /// Burn Away, Constrain, Target Attack, Chlorophyll, Delta Sign).
    /// </summary>
    public class CgTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";
        const string PikachuDelta = "HP-79";
        const string DoubleRainbow = "EM-87";
        const string DoubleColorless = "SLG-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Crystal_Guardians_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(88, db.Cards.Count(d => d.SetCode == "CG"));
            for (int n = 1; n <= 100; n++) Assert.IsNotNull(db.Get("CG", n.ToString()), "CG " + n);
            Assert.AreEqual("CES-126", db.MainPrinting(C("CG-71")).Key, "Bill's Maintenance");
            Assert.AreEqual("EM-87", db.MainPrinting(C("CG-88")).Key, "Double Rainbow Energy");
            Assert.AreEqual("E3-122", db.MainPrinting(C("CG-76")).Key, "Crystal Shard");
            Assert.IsTrue(C("CG-99").IsPokemonStar && C("CG-89").IsOldEx);
        }

        [Test]
        public void Every_Crystal_Guardians_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "CG" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Holon_Circle_Ends_An_Attack_At_Once_And_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rollout"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            var target = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C("CG-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, target.DamageCounters);
            Assert.IsNull(e.State.Stadium);
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Holon Circle"));
        }

        [Test]
        public void Dark_Eyes_Puts_2_Counters_On_A_Pokemon_That_Used_A_Poke_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Excavate", "Leave it"), new ScriptedAgent());
            var sableye = e.ScenarioActive(0, C("CG-10"));
            e.ScenarioActive(1, C("CG-97"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, sableye.DamageCounters);
        }

        [Test]
        public void Cessation_Crystal_On_The_Active_Pokemon_Stops_Poke_Powers()
        {
            foreach (bool crystal in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C("CG-10"));
                var holder = e.ScenarioActive(1, C(Chansey));
                if (crystal) e.ScenarioAttachTool(holder, C("CG-74"));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!crystal, Offered(e, 0, "Use Excavate"));
            }
        }

        [Test]
        public void Crystal_Beach_Makes_Double_Rainbow_Energy_Provide_1_Colorless()
        {
            foreach (bool beach in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var mon = e.ScenarioActive(0, C(Chansey));
                Attach(e, mon, DoubleRainbow, 1);
                e.ScenarioActive(1, C(Chansey));
                if (beach) e.ScenarioSetStadium(0, C("CG-75"));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(beach ? 1 : 2, e.EnergyCount(mon));
            }
        }

        [Test]
        public void Mysterious_Shard_Stops_A_Pokemon_ex_And_Goes_At_The_End_Of_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var holder = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAttachTool(holder, C("CG-81"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, holder.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(0, holder.Tools.Count, "discarded at the end of the opponent's turn");
        }

        [Test]
        public void Windstorm_Discards_Tools_And_A_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Windstorm", "Holon Circle|Mysterious Shard"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var holder = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAttachTool(holder, C("CG-81"));
            e.ScenarioSetStadium(1, C("CG-79"));
            e.ScenarioAddToHand(0, C("CG-85"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.AreEqual(0, holder.Tools.Count);
        }

        [Test]
        public void Castaway_Finds_A_Supporter_A_Tool_And_A_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Castaway"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(0, C("CG-72"), 1);
            e.ScenarioAddToDeck(0, C("CG-81"), 1);
            e.ScenarioAddToHand(0, C("CG-72"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(3, hand.Count);
            Assert.IsTrue(hand.Any(c => c.Def.IsSupporter) && hand.Any(c => c.Def.IsTool) && hand.Any(c => c.Def.IsBasicEnergy));
        }

        [Test]
        public void Intimidating_Armor_Stops_The_Opponents_Basic_Pokemon_Attacking()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            var aggron = e.ScenarioActive(1, C("CG-89"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Rollout"));
        }

        [Test]
        public void Extra_Liquid_Makes_Pokemon_ex_Attacks_Cost_1_More()
        {
            foreach (bool sceptile in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var mew = e.ScenarioActive(0, C(MewEx));
                Attach(e, mew, Psychic, 2);
                if (sceptile) e.ScenarioActive(1, C("CG-96"));
                else e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!sceptile, Offered(e, 0, "Attack: Super Psy Bolt"));
            }
        }

        [Test]
        public void Focus_Energy_Makes_High_Jump_Kick_70_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Focus Energy", "Attack: High Jump Kick"), new ScriptedAgent());
            var combusken = e.ScenarioActive(0, C("CG-16"));
            Attach(e, combusken, Fighting, 2);
            var target = e.ScenarioActive(1, C("CG-89"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, target.DamageCounters);
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.AreEqual(7, target.DamageCounters, "base damage 70");
        }

        [Test]
        public void Even_Game_Benches_As_Many_Basic_Pokemon_As_The_Opponent_Has_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Even Game", "Chansey|Chansey|Chansey"), new ScriptedAgent());
            var luvdisc = e.ScenarioActive(0, C("CG-7"));
            Attach(e, luvdisc, Water, 1);
            e.ScenarioAddToDeck(0, C(Chansey), 5);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Burn_Away_Hurts_Blaziken_ex_For_The_Extra_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Burn Away"), new ScriptedAgent());
            var blaziken = e.ScenarioActive(0, C("CG-90"));
            Attach(e, blaziken, Fire, 2);
            Attach(e, blaziken, Fighting, 2);
            var target = e.ScenarioActive(1, C(Chansey));
            target.DamageCounters = 7; // 30 HP left
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, blaziken.DamageCounters, "100 - 30");
        }

        [Test]
        public void Constrain_Makes_Each_Player_Discard_Down_To_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Constrain"), new ScriptedAgent());
            e.ScenarioActive(0, C("CG-91"));
            e.ScenarioActive(1, C(Chansey));
            for (int i = 0; i < 8; i++) e.ScenarioAddToHand(0, C(Psychic));
            for (int i = 0; i < 9; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
            Assert.AreEqual(6, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Target_Attack_Does_50_To_A_Damaged_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Target Attack", "Chansey"), new ScriptedAgent());
            var shiftry = e.ScenarioActive(0, C("CG-97"));
            Attach(e, shiftry, Darkness, 2);
            e.ScenarioActive(1, C(MewEx));
            var benched = e.ScenarioBench(1, C(Chansey));
            benched.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, benched.DamageCounters, "1 + 50");
        }

        [Test]
        public void Chlorophyll_Turns_Colorless_Only_Energy_On_Grass_Pokemon_Into_Grass()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var venusaur = e.ScenarioActive(0, C("CG-28"));
            Attach(e, venusaur, DoubleColorless, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.EnergyCountOfType(venusaur, EnergyType.Grass));
        }

        [Test]
        public void Delta_Sign_Finds_A_Delta_Pokemon_Once_Per_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Delta Sign"), new ScriptedAgent());
            e.ScenarioActive(0, C("CG-18"));
            e.ScenarioBench(0, C("CG-18"));
            e.ScenarioAddToDeck(0, C(PikachuDelta), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Pikachu", e.State.Players[0].Hand.Single().Def.Name);
            Assert.IsFalse(Offered(e, 0, "Use Delta Sign"), "only 1 Delta Sign each turn");
        }

        [Test]
        public void Time_Travel_Puts_Celebi_Star_On_The_Bottom_Of_The_Deck_On_Heads()
        {
            int escaped = 0, knockedOut = 0;
            for (int seed = 1; seed <= 12; seed++)
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
                e.BeginScenario(seed);
                e.ScenarioAddPrizes(0, C(Fire), 6);
                e.ScenarioAddPrizes(1, C(Fire), 6);
                e.ScenarioAddToDeck(0, C(Psychic), 20);
                e.ScenarioAddToDeck(1, C(Psychic), 20);
                var celebi = e.ScenarioActive(0, C("CG-100"));
                Attach(e, celebi, Grass, 1);
                e.ScenarioBench(0, C(Chansey));
                var mew = e.ScenarioActive(1, C(MewEx));
                Attach(e, mew, Psychic, 2);
                celebi.DamageCounters = 4;
                e.ScenarioSetTurn(1, 4);
                Step(e);
                var me = e.State.Players[0];
                if (me.Deck.Last().Def.Name == "Celebi ★")
                {
                    escaped++;
                    Assert.AreEqual(6, e.State.Players[1].Prizes.Count, "not Knocked Out: no Prize card");
                    Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Grass Energy"));
                }
                else
                {
                    knockedOut++;
                    Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Celebi ★"));
                }
            }
            Assert.Greater(escaped, 0);
            Assert.Greater(knockedOut, 0);
        }
    }
}
