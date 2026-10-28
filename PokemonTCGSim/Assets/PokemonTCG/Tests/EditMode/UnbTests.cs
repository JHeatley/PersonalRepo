using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v73: Unbroken Bonds. The engine features it brought in (Battery, Ruler of the Night, Swelling Spite, Acme of Heroism-GX,
    /// Force Canceler, Dust Island) and a few cards that are easy to get wrong.
    /// </summary>
    public class UnbTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Sneasel = "UNM-131";
        const string RecycleEnergy = "UNM-212";
        const string PheromosaBuzzwoleGX = "UNB-1";
        const string Caterpie = "UNB-2";
        const string Metapod = "UNB-3";
        const string DedenneGX = "UNB-57";
        const string Charjabug = "UNB-58";
        const string Vikavolt = "UNB-59";
        const string MukGX = "UNB-61";
        const string Gastly = "UNB-67";
        const string Haunter = "UNB-69";
        const string Mew = "UNB-76";
        const string MarshadowMachampGX = "UNB-82";
        const string Diglett = "UNB-85";
        const string Gligar = "UNB-98";
        const string Stakataka = "UNB-106";
        const string HonchkrowGX = "UNB-109";
        const string LucarioMelmetalGX = "UNB-120";
        const string AlolanDugtrio = "UNB-122";
        const string Meowth = "UNB-147";
        const string Persian = "UNB-148";
        const string CelesteelaGX = "UNB-163";
        const string DustIsland = "UNB-168";
        const string MetalCoreBarrier = "UNB-180";
        const string Welder = "UNB-189";
        const string TripleAcceleration = "UNB-190";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ================================================================== Engine features

        [Test]
        public void Battery_Attaches_Charjabug_To_Vikavolt_As_2_Lightning_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Battery"), new ScriptedAgent());
            var vikavolt = e.ScenarioActive(0, C(Vikavolt));
            e.ScenarioActive(1, C(Sneasel));
            var charjabug = e.ScenarioAddToHand(0, C(Charjabug));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.Contains(vikavolt.Energy, charjabug);
            var units = e.EnergyUnits(vikavolt);
            Assert.AreEqual(2, units.Count);
            Assert.IsTrue(units.All(u => (u & EnergyMask.Of(EnergyType.Lightning)) != 0));
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn, "it isn't the turn's Energy attachment");
        }

        [Test]
        public void Ruler_Of_The_Night_Stops_Special_Energy_From_The_Hand_While_Honchkrow_GX_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Vikavolt));
            e.ScenarioAddToHand(0, C(RecycleEnergy));
            e.ScenarioAddToHand(0, C(Charjabug));
            var honchkrow = e.ScenarioActive(1, C(HonchkrowGX));
            e.ScenarioSetTurn(0, 3);
            var actions = e.GetMainActions(0);
            Assert.IsFalse(actions.Any(o => o.Label.Contains("Attach Recycle Energy")));
            Assert.IsFalse(actions.Any(o => o.Label.Contains("Battery")), "Battery attaches it as a Special Energy card (your ruling)");
            e.ScenarioActive(1, C(Sneasel));
            e.State.Players[1].Bench.Add(honchkrow);
            actions = e.GetMainActions(0);
            Assert.IsTrue(actions.Any(o => o.Label.Contains("Attach Recycle Energy")), "only while it's Active");
            Assert.IsTrue(actions.Any(o => o.Label.Contains("Battery")));
        }

        [Test]
        public void Swelling_Spite_Benches_2_Haunter_Once_Gastly_Has_Left_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Static Shock"), new ScriptedAgent().Then("Haunter|Haunter"));
            var dedenne = e.ScenarioActive(0, C(DedenneGX));
            e.ScenarioAttach(dedenne, C(Lightning));
            e.ScenarioAttach(dedenne, C(Lightning));
            e.ScenarioActive(1, C(Gastly));
            for (int i = 0; i < 3; i++) e.ScenarioBench(1, C(Sneasel));
            e.ScenarioAddToDeck(1, C(Haunter), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(2, opp.InPlay().Count(m => m.Name == "Haunter"));
            Assert.AreEqual(1, opp.Discard.Count(c => c.Def.Name == "Gastly"));
        }

        [Test]
        public void Acme_Of_Heroism_GX_Leaves_It_At_10_HP_During_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Acme of Heroism-GX"), new ScriptedAgent());
            var mm = e.ScenarioActive(0, C(MarshadowMachampGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(mm, C(Fighting));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioBench(1, C(Sneasel));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var attacker = e.State.Players[1].Active;
            e.DealAttackDamage(attacker, mm, 500);
            Assert.AreEqual(10, e.RemainingHp(mm));
        }

        [Test]
        public void Force_Canceler_Stops_The_Opponents_GX_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tingly Return-GX"), new ScriptedAgent());
            var dedenne = e.ScenarioActive(0, C(DedenneGX));
            e.ScenarioAttach(dedenne, C(Lightning));
            e.ScenarioAttach(dedenne, C(Lightning));
            e.ScenarioBench(0, C(Sneasel));
            var celesteela = e.ScenarioActive(1, C(CelesteelaGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].GxUsed);
            Assert.AreEqual(0, celesteela.DamageCounters);
            Assert.IsFalse(celesteela.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Dust_Island_Poisons_The_New_Active_After_A_Trainer_Switch()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Switch"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Sneasel));
            active.Conditions = SpecialCondition.Poisoned;
            var benched = e.ScenarioBench(0, C(Gligar));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioSetStadium(1, C(DustIsland));
            e.ScenarioAddToHand(0, C("MEG-130"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(benched, e.State.Players[0].Active);
            Assert.IsTrue(benched.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(active.Has(SpecialCondition.Poisoned), "the Benched one recovers as usual");
        }

        [Test]
        public void Giovannis_Exile_Discarding_Diglett_Discards_The_Top_Card_Of_The_Opponents_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Giovanni's Exile", "Diglett"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioBench(0, C(Diglett));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C("HIF-57"));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[1].Deck.Count;
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(deck - 1, e.State.Players[1].Deck.Count);
        }

        // ================================================================== Pokémon

        [Test]
        public void Metal_Core_Barrier_And_Hair_Wall_Both_Reduce_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dedenne = e.ScenarioActive(0, C(DedenneGX));
            var lucario = e.ScenarioActive(1, C(LucarioMelmetalGX));
            e.ScenarioAttachTool(lucario, C(MetalCoreBarrier));
            e.ScenarioBench(1, C(AlolanDugtrio));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(dedenne, lucario, 100);
            Assert.AreEqual(2, lucario.DamageCounters, "100 - 70 - 10");
        }

        [Test]
        public void Wall_Of_Stone_Makes_Stakatakas_Max_HP_200_Late_In_The_Game()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var stakataka = e.ScenarioActive(0, C(Stakataka));
            e.ScenarioActive(1, C(Sneasel));
            Assert.AreEqual(120, e.MaxHp(stakataka));
            e.State.Players[1].Prizes.RemoveRange(0, 3);
            Assert.AreEqual(200, e.MaxHp(stakataka));
        }

        [Test]
        public void Free_Flight_And_Stealthy_Body()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gligar = e.ScenarioActive(0, C(Gligar));
            e.ScenarioActive(1, C(Sneasel));
            Assert.AreEqual(0, e.RetreatCostOf(gligar), "no Energy attached");
            e.ScenarioAttach(gligar, C(Psychic));
            Assert.AreEqual(2, e.RetreatCostOf(gligar));
            var vikavolt = e.ScenarioBench(0, C(Vikavolt));
            Assert.IsNotNull(e.WeaknessOf(vikavolt));
            e.ScenarioSetStadium(1, C(DustIsland));
            Assert.IsNull(e.WeaknessOf(vikavolt), "any Stadium in play");
        }

        [Test]
        public void Bench_Barrier_Stops_Damage_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jet Punch"), new ScriptedAgent());
            var pb = e.ScenarioActive(0, C(PheromosaBuzzwoleGX));
            e.ScenarioAttach(pb, C(Grass));
            var target = e.ScenarioActive(1, C(Sneasel));
            var mew = e.ScenarioBench(1, C(Mew));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.Greater(target.DamageCounters, 0);
            Assert.AreEqual(0, mew.DamageCounters);
        }

        [Test]
        public void Dedechange_Discards_The_Hand_And_Draws_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Dedenne-GX", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C(DedenneGX));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(6, me.Hand.Count);
            Assert.AreEqual(3, me.Discard.Count(c => c.Def.Name == "Fire Energy"));
        }

        [Test]
        public void Gathering_Of_Cats_Frees_The_Attacks_Of_Pokemon_With_Caturday()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Meowth));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Tail Whap")));
            e.ScenarioBench(0, C(Persian));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Tail Whap")));
        }

        [Test]
        public void Severe_Poison_Puts_8_Damage_Counters_In_Pokemon_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Severe Poison"), new ScriptedAgent());
            var muk = e.ScenarioActive(0, C(MukGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(muk, C(Psychic));
            var target = e.ScenarioActive(1, C(LucarioMelmetalGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(8, target.PoisonCounterBase);
        }

        // ================================================================== Trainers and Energy

        [Test]
        public void Triple_Acceleration_Energy_Only_On_Evolution_Pokemon_And_Only_For_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Triple Acceleration Energy to Metapod"), new ScriptedAgent());
            var metapod = e.ScenarioActive(0, C(Caterpie));
            e.ScenarioEvolve(metapod, C(Metapod));
            e.ScenarioBench(0, C(Caterpie));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C(TripleAcceleration));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Triple Acceleration Energy to Caterpie")));
            Step(e);
            Assert.AreEqual(3, e.EnergyUnits(metapod).Count);
            e.EndTurn();
            Assert.AreEqual(0, metapod.Energy.Count, "discarded at the end of the turn");
        }

        [Test]
        public void Welder_Attaches_2_Fire_Energy_And_Draws_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Welder", "Fire Energy|Fire Energy"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C(Welder));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, mon.Energy.Count);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }
    }
}
