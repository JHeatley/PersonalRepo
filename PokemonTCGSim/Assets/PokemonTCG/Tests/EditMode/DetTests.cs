using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v79: Detective Pikachu (DET 1-18).</summary>
    public class DetTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Water = "MEE-11";
        const string Fairy = "SUM-Y";
        const string Ludicolo = "DET-2";
        const string Arcanine = "DET-6";
        const string Greninja = "DET-9";
        const string MrMime = "DET-11";
        const string Mewtwo = "DET-12";
        const string Jigglypuff = "DET-14";
        const string Snubbull = "DET-15";
        const string Lickitung = "DET-16";
        const string Ditto = "DET-17";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var cards = db.Cards.Where(d => d.SetCode == "DET").ToList();
            Assert.AreEqual(18, cards.Count);
            Assert.IsTrue(cards.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            var pikachu = C("DET-10");
            Assert.AreEqual("Detective Pikachu", pikachu.Name);
            Assert.AreEqual(90, pikachu.Hp);
            Assert.AreEqual(EnergyType.Metal, pikachu.Resistance);
            Assert.AreEqual(20, pikachu.ResistanceAmount);
            Assert.AreEqual("Vigoroth", C("DET-18").EvolvesFrom);
            Assert.AreEqual("x", C(Lickitung).Attacks[0].DamageSuffix);
            Assert.AreEqual("Detective Pikachu", C("DET-1").PrintSetName);
        }

        [Test]
        public void Security_Guard_Takes_30_Off_All_Your_Pokemon_While_Arcanine_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Mewtwo));
            var arcanine = e.ScenarioActive(1, C(Arcanine));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, arcanine, 100);
            Assert.AreEqual(120 - 70, e.RemainingHp(arcanine));
            e.DealAttackDamage(attacker, benched, 50, false);
            Assert.AreEqual(70 - 20, e.RemainingHp(benched), "the Bench is covered too");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker2 = e2.ScenarioActive(0, C(Mewtwo));
            var active = e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioBench(1, C(Arcanine));
            e2.ScenarioSetTurn(0, 3);
            e2.DealAttackDamage(attacker2, active, 50);
            Assert.AreEqual(70 - 50, e2.RemainingHp(active), "only while Arcanine is Active");
        }

        [Test]
        public void Evasion_Jutsu_Prevents_The_Damage_On_Heads()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Mewtwo));
            var greninja = e.ScenarioActive(1, C(Greninja));
            e.ScenarioSetTurn(0, 3);
            int prevented = 0, taken = 0;
            for (int i = 0; i < 40; i++)
            {
                greninja.DamageCounters = 0;
                e.DealAttackDamage(attacker, greninja, 30);
                if (greninja.DamageCounters == 0) prevented++;
                else taken++;
            }
            Assert.Greater(prevented, 0);
            Assert.Greater(taken, 0);
        }

        [Test]
        public void Furious_Shurikens_Does_50_To_2_Of_The_Opponents_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Furious Shurikens"), new ScriptedAgent());
            var greninja = e.ScenarioActive(0, C(Greninja));
            e.ScenarioAttach(greninja, C(Water));
            e.ScenarioAttach(greninja, C(Water));
            var active = e.ScenarioActive(1, C(Mewtwo));
            var bench = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, active.DamageCounters);
            Assert.AreEqual(5, bench.DamageCounters);
        }

        [Test]
        public void Pantomime_Switches_A_Prize_Card_With_The_Top_Card_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Mr. Mime", "Yes", "Face-down Prize card 1"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(MrMime));
            e.ScenarioSetTurn(0, 3);
            var me = e.State.Players[0];
            Assert.IsTrue(me.Prizes.All(c => c.Def.Name == "Fire Energy"));
            Step(e);
            Assert.AreEqual(1, me.Bench.Count(m => m.Name == "Mr. Mime"));
            Assert.AreEqual(6, me.Prizes.Count);
            Assert.AreEqual(1, me.Prizes.Count(c => c.Def.Name == "Psychic Energy"), "the deck's top card became a Prize card");
            Assert.AreEqual("Fire Energy", me.Deck[0].Def.Name, "and the Prize card went on top of the deck");
        }

        [Test]
        public void Copy_Anything_Uses_An_Attack_Of_Any_Opposing_Pokemon_If_Ditto_Has_Its_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Copy Anything", "Mewtwo: Psyjack", "Play Rough"), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(Ditto));
            e.ScenarioAttach(ditto, C(Psychic));
            e.ScenarioAttach(ditto, C(Psychic));
            var snubbull = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAttach(snubbull, C(Psychic));
            e.ScenarioAttach(snubbull, C(Psychic));
            e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snubbull.DamageCounters, "Psyjack from the Bench, used by Ditto");
            NextTurn(e);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label == "Attack: Play Rough"), "and its effect too");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Copy Anything", "Mewtwo: Psyjack"), new ScriptedAgent());
            var ditto2 = e2.ScenarioActive(0, C(Ditto));
            e2.ScenarioAttach(ditto2, C(Psychic));
            var snubbull2 = e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioBench(1, C(Mewtwo));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, snubbull2.DamageCounters, "1 Energy isn't enough for Psyjack: nothing happens");
        }

        [Test]
        public void Psyjack_Stops_The_Chosen_Attack_During_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psyjack", "Play Rough"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioAttach(mewtwo, C(Psychic));
            e.ScenarioAttach(mewtwo, C(Psychic));
            var snubbull = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAttach(snubbull, C(Psychic));
            e.ScenarioAttach(snubbull, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snubbull.DamageCounters);
            NextTurn(e);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label == "Attack: Play Rough"));
        }

        [Test]
        public void Table_Service_And_Healing_Melody_Heal()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Table Service"), new ScriptedAgent());
            var ludicolo = e.ScenarioActive(0, C(Ludicolo));
            var hurt = e.ScenarioBench(0, C(Snubbull));
            hurt.DamageCounters = 5;
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, hurt.DamageCounters, "30 healed");
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Table Service")), "once a turn");
            Assert.AreEqual(0, ludicolo.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Healing Melody"), new ScriptedAgent());
            var jigglypuff = e2.ScenarioActive(0, C(Jigglypuff));
            e2.ScenarioAttach(jigglypuff, C(Fairy));
            jigglypuff.DamageCounters = 2;
            var benched = e2.ScenarioBench(0, C(Snubbull));
            benched.DamageCounters = 3;
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, jigglypuff.DamageCounters);
            Assert.AreEqual(2, benched.DamageCounters);
        }
    }
}
