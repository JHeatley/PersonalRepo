using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v149: Ruby &amp; Sapphire (RS): the card data and prints, the Poké-Powers and Poké-Bodies (Lazy, Intimidating Fang, Withering
    /// Dust, Rough Skin, Natural Cure, Natural Remedy), Stockpile, Mega Throw, Lizard Poison and the new Trainers.
    /// </summary>
    public class RsTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Hariyama = "RS-8";
        const string MightyenaFang = "RS-10";
        const string Sceptile = "RS-11";
        const string Slaking = "RS-12";
        const string SwampertWaterCall = "RS-13";
        const string Pelipper = "RS-19";
        const string Sharpedo = "RS-22";
        const string SwampertRemedy = "RS-23";
        const string Beautifly = "RS-2";
        const string Kirlia = "RS-34";
        const string Marshtomp = "RS-41";
        const string Mightyena = "RS-42";
        const string Makuhita = "RS-56";
        const string LadyOuting = "RS-83";
        const string OranBerry = "RS-85";
        const string LaprasEx = "RS-99";
        const string Chansey = "E2-69";
        const string Kingdra = "E2-19";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void Ruby_And_Sapphire_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "RS").ToList();
            Assert.AreEqual(83, set.Count, "109 cards minus 26 prints of cards already in the pool");
            Assert.AreEqual(109, db.All.Count(d => d.SetCode == "RS"));
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual("30C-CC8", db.MainPrinting(db.Get("RS-5")).Key);
            Assert.AreEqual("NP-9", db.MainPrinting(db.Get("RS-27")).Key);
            Assert.AreEqual("NP-3", db.MainPrinting(db.Get("RS-76")).Key);
            Assert.AreEqual("E1-140", db.MainPrinting(db.Get("RS-80")).Key);
            Assert.AreEqual("SSH-168", db.MainPrinting(db.Get("RS-84")).Key);
            Assert.AreEqual("CES-140", db.MainPrinting(db.Get("RS-88")).Key);
            Assert.AreEqual("N1-104", db.MainPrinting(db.Get("RS-93")).Key);
            Assert.AreEqual("CES-151", db.MainPrinting(db.Get("RS-95")).Key);
            Assert.AreEqual("MEE-9", db.MainPrinting(db.Get("RS-104")).Key);
            Assert.IsTrue(C("RS-96").IsOldEx && C("RS-103").IsOldEx);
            Assert.IsFalse(C(Mightyena).IsOldEx);
            Assert.IsTrue(C(OranBerry).IsTool);
            Assert.IsTrue(C(LadyOuting).IsSupporter);
        }

        // ------------------------------------------------------------------ Poké-Powers and Poké-Bodies

        [Test]
        public void Lazy_Stops_The_Opponents_Poke_Powers_While_Slaking_Is_Active()
        {
            foreach (bool active in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                if (active) e.ScenarioActive(0, C(Slaking));
                else
                {
                    e.ScenarioActive(0, C(Chansey));
                    e.ScenarioBench(0, C(Slaking));
                }
                e.ScenarioActive(1, C(SwampertWaterCall));
                e.ScenarioAddToHand(1, C(Water));
                e.ScenarioSetTurn(1, 4);
                Assert.AreEqual(!active, Offered(e, 1, "Use Water Call"));
            }
        }

        [Test]
        public void Intimidating_Fang_Takes_10_Off_Before_Weakness()
        {
            foreach (bool fang in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Super Slap Push"), new ScriptedAgent());
                var hariyama = e.ScenarioActive(0, C(Hariyama));
                Attach(e, hariyama, Fighting, 1);
                var target = e.ScenarioActive(1, C(fang ? MightyenaFang : Mightyena));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(fang ? 2 : 4, target.DamageCounters, fang ? "(20 - 10) x 2" : "20 x 2");
            }
        }

        [Test]
        public void Withering_Dust_Turns_Resistance_Off_From_The_Bench()
        {
            foreach (bool dust in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Super Psy"), new ScriptedAgent());
                var kirlia = e.ScenarioActive(0, C(Kirlia));
                Attach(e, kirlia, Psychic, 3);
                if (dust) e.ScenarioBench(0, C(Beautifly));
                var mightyena = e.ScenarioActive(1, C(Mightyena));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(dust ? 5 : 2, mightyena.DamageCounters, "Resistance -30 to Psychic");
            }
        }

        [Test]
        public void Rough_Skin_Puts_2_Damage_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Slap Push"), new ScriptedAgent());
            var makuhita = e.ScenarioActive(0, C(Makuhita));
            Attach(e, makuhita, Psychic, 1);
            var sharpedo = e.ScenarioActive(1, C(Sharpedo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, sharpedo.DamageCounters);
            Assert.AreEqual(2, makuhita.DamageCounters);
        }

        [Test]
        public void Natural_Cure_Cures_Marshtomp_When_Water_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy to Marshtomp"), new ScriptedAgent());
            var marshtomp = e.ScenarioActive(0, C(Marshtomp));
            e.ApplyCondition(marshtomp, SpecialCondition.Poisoned);
            e.ApplyCondition(marshtomp, SpecialCondition.Asleep);
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, marshtomp.Conditions);
            Assert.AreEqual(1, marshtomp.Energy.Count);
        }

        [Test]
        public void Natural_Remedy_Removes_A_Damage_Counter_And_Water_Call_Triggers_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Water Call"), new ScriptedAgent());
            var remedy = e.ScenarioActive(0, C(SwampertRemedy));
            remedy.DamageCounters = 3;
            e.ScenarioBench(0, C(SwampertWaterCall));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, remedy.DamageCounters, "Water Call attached from the hand");
            Assert.AreEqual(1, remedy.Energy.Count);
            var hand = e.State.Players[0].Hand;
            var second = hand.First(c => c.Def.IsBasicEnergy);
            hand.Remove(second);
            e.AttachEnergyCard(second, remedy, true);
            Assert.AreEqual(2, remedy.DamageCounters, "only once a turn");
            Assert.AreEqual(2, remedy.Energy.Count);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Stockpile_Makes_Spit_Up_Do_70_During_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stockpile", "Attack: Spit Up"), new ScriptedAgent());
            var pelipper = e.ScenarioActive(0, C(Pelipper));
            Attach(e, pelipper, Water, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.DamageCounters);
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.AreEqual(7, chansey.DamageCounters);
        }

        [Test]
        public void Mega_Throw_Does_40_More_To_A_Pokemon_ex()
        {
            foreach (bool ex in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Mega Throw"), new ScriptedAgent());
                var hariyama = e.ScenarioActive(0, C(Hariyama));
                Attach(e, hariyama, Fighting, 3);
                var target = e.ScenarioActive(1, C(ex ? LaprasEx : Kingdra));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(ex ? 8 : 4, target.DamageCounters);
            }
        }

        [Test]
        public void Lizard_Poison_With_4_Energy_Is_Asleep_Burned_And_Poisoned()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lizard Poison"), new ScriptedAgent());
            var sceptile = e.ScenarioActive(0, C(Sceptile));
            Attach(e, sceptile, Grass, 4);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(chansey.Has(SpecialCondition.Asleep));
            Assert.IsTrue(chansey.Has(SpecialCondition.Burned));
            Assert.IsTrue(chansey.Has(SpecialCondition.Poisoned));
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Oran_Berry_Removes_2_Counters_From_2_Or_More()
        {
            foreach (int counters in new[] { 1, 2 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var chansey = e.ScenarioActive(0, C(Chansey));
                chansey.DamageCounters = counters;
                e.ScenarioAttachTool(chansey, C(OranBerry));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.EndTurn();
                Assert.AreEqual(counters == 2 ? 0 : 1, chansey.DamageCounters);
                Assert.AreEqual(counters == 2 ? 0 : 1, chansey.Tools.Count);
            }
        }

        [Test]
        public void Lady_Outing_Takes_Basic_Energy_Of_Different_Types()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lady Outing"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToDeck(0, C(Fire), 2);
            e.ScenarioAddToDeck(0, C(Water), 2);
            e.ScenarioAddToHand(0, C(LadyOuting));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(3, hand.Count);
            Assert.AreEqual(3, hand.Select(c => c.Def.EnergyProvides).Distinct().Count());
        }
    }
}
