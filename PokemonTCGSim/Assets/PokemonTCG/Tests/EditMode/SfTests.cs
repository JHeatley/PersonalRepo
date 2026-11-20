using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Stormfront (SF): the card data and prints, and the new effects (Energy Link, No Guard, Energy Burn, Link Lightning,
    /// Ectoplasm, the Poké "+" Items, Luxury Ball, Battle Rush, Take Out, Land Shake).
    /// </summary>
    public class SfTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
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
        public void Stormfront_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(95, db.Cards.Count(d => d.SetCode == "SF"));
            Assert.AreEqual("DPP-31", db.MainPrinting(C("SF-97")).Key, "Infernape LV.X");
            Assert.AreEqual("SF-103", db.MainPrinting(C("SF-103")).Key, "this Charizard is its own card");
            Assert.AreEqual(1, C("SF-83").Abilities.Count, "Energy Link's effect is usable");
        }

        [Test]
        public void Every_Stormfront_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "SF" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Energy_Link_Moves_Energy_Between_Linked_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energy Link"), new ScriptedAgent());
            var a = e.ScenarioActive(0, C(Chansey));
            var b = e.ScenarioBench(0, C(Chansey));
            e.ScenarioAttachTool(a, C("SF-83"));
            e.ScenarioAttachTool(b, C("SF-83"));
            Attach(e, a, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, a.Energy.Count);
            Assert.AreEqual(1, b.Energy.Count);
        }

        [Test]
        public void No_Guard_Hits_60_Harder_Both_Ways()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Strong-Willed"), new ScriptedAgent());
            var machamp = e.ScenarioActive(0, C("SF-20"));
            e.ScenarioEvolve(machamp, C("SF-98"));
            Attach(e, machamp, Fighting, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, chansey.DamageCounters, "Strong-Willed (20 + 60) x 2 for Chansey's Weakness");
        }

        [Test]
        public void Energy_Burn_Makes_All_Energy_Fire()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var charizard = e.ScenarioActive(0, C("SF-103"));
            Attach(e, charizard, Psychic, 4);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(4, e.EnergyCountOfType(charizard, EnergyType.Fire));
            Assert.IsTrue(Offered(e, 0, "Fire Spin"));
        }

        [Test]
        public void Link_Lightning_Offers_A_Second_Attack_After_Voltage_Shoot()
        {
            var agent = new ScriptedAgent().Then("Attack: Voltage Shoot", "Mew ex", "Don't attack again");
            var e = Scenario(agent, new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C("SF-8"));
            e.ScenarioEvolve(raichu, C("SF-99"));
            Attach(e, raichu, Lightning, 3);
            e.ScenarioAddToHand(0, C(Lightning));
            e.ScenarioAddToHand(0, C(Lightning));
            e.ScenarioActive(1, C("HP-100"));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            raichu.TurnEvolved = e.State.TurnNumber;
            Step(e);
            Assert.IsTrue(agent.Seen.Any(r => r.Prompt.Contains("attack a second time")));
        }

        [Test]
        public void Ectoplasm_Turns_Dusknoir_Lv_X_Into_A_Stadium()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var dusknoir = e.ScenarioActive(0, C("SF-17"));
            e.ScenarioEvolve(dusknoir, C("SF-96"));
            dusknoir.DamageCounters = 13;
            e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsNotNull(e.State.Stadium);
            Assert.AreEqual("Dusknoir LV.X", e.State.Stadium.Def.Name);
            Assert.AreEqual(0, e.State.StadiumOwner);
        }

        [Test]
        public void Two_Poke_Drawer_Plus_Search_For_2_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Poké Drawer +", "Yes", "Psychic|Psychic"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("SF-89"));
            e.ScenarioAddToHand(0, C("SF-89"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Poké Drawer +"));
        }

        [Test]
        public void Luxury_Ball_Cant_Be_Played_With_One_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("SF-86"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Luxury Ball"));
            e.ScenarioAddToDiscard(0, C("SF-86"), 1);
            Assert.IsFalse(Offered(e, 0, "Play Luxury Ball"));
        }

        [Test]
        public void Battle_Rush_Drops_Colorless_Costs_Against_Big_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var salamence = e.ScenarioActive(0, C("SF-24"));
            Attach(e, salamence, Fire, 1);
            Attach(e, salamence, Water, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            var twister = salamence.Def.Attacks.First(a => a.Name == "Steam Twister");
            bool big = e.MaxHp(chansey) >= 120;
            Assert.AreEqual(big, e.CanPayAttack(salamence, twister));
            var gyarados = e.ScenarioBench(1, C("MT-89"));
            e.ScenarioEvolve(gyarados, C("MT-26"));
            Assert.IsTrue(e.CanPayAttack(salamence, twister), "Gyarados has 120 HP");
        }

        [Test]
        public void Take_Out_Knocks_Out_A_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Take Out"), new ScriptedAgent());
            var machamp = e.ScenarioActive(0, C("SF-20"));
            Attach(e, machamp, Fighting, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[1].InPlay().Contains(chansey));
        }

        [Test]
        public void Land_Shake_Hurts_Basic_Pokemon_Benched_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Land Shake"), new ScriptedAgent().Then("Bench Chansey"));
            var torterra = e.ScenarioActive(0, C("SF-11"));
            Attach(e, torterra, "MEE-9", 4);
            e.ScenarioActive(1, C(MewEx));
            e.ScenarioAddToHand(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Step(e);
            var benched = e.State.Players[1].Bench.FirstOrDefault(m => m.Name == "Chansey");
            Assert.IsNotNull(benched);
            Assert.AreEqual(2, benched.DamageCounters);
        }
    }
}
