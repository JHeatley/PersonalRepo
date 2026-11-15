using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: Platinum (PL): the card data and prints, and the new effects (Damage Bind, Electric Barrier, Lost Cyclone,
    /// Thankfulness, Time Crystal, Galactic HQ, Miasma Valley, Power Spray, Broken-space Blow, Cold Feet).
    /// </summary>
    public class PlTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
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
        public void Platinum_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(119, db.Cards.Count(d => d.SetCode == "PL"));
            Assert.AreEqual("DPP-38", db.MainPrinting(C("PL-124")).Key, "Giratina LV.X");
            Assert.AreEqual("CES-151", db.MainPrinting(C("PL-121")).Key, "Rainbow Energy");
            Assert.IsTrue(C("PL-17").IsPokemonSP, "Weavile G is a Pokémon SP");
        }

        [Test]
        public void Every_Platinum_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "PL" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Damage_Bind_Stops_Poke_Powers_Of_Damaged_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var banette = e.ScenarioActive(0, C("PL-19"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C("PL-1"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Temper Tantrum"));
            banette.DamageCounters = 1;
            Assert.IsFalse(Offered(e, 0, "Temper Tantrum"));
        }

        [Test]
        public void Electric_Barrier_Protects_The_Other_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot"));
            e.ScenarioActive(0, C("PL-11"));
            var bench = e.ScenarioBench(0, C(Chansey));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, Water, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, bench.DamageCounters);
        }

        [Test]
        public void Lost_Cyclone_Leaves_Each_Big_Bench_With_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Lost Cyclone"), new ScriptedAgent());
            var palkia = e.ScenarioActive(0, C("PL-12"));
            e.ScenarioEvolve(palkia, C("PL-125"));
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Bench.Count);
            Assert.AreEqual(2, e.State.Players[1].LostZone.Count(c => c.Def.Name == "Chansey"));
            Assert.AreEqual(1, e.State.Players[0].Bench.Count, "a Bench of 1 is left alone");
        }

        [Test]
        public void Thankfulness_Gives_Other_Grass_Pokemon_40_Hp()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var shaymin = e.ScenarioActive(0, C("PL-38"));
            var turtwig = e.ScenarioBench(0, C("PL-101"));
            int before = e.MaxHp(turtwig), own = e.MaxHp(shaymin);
            e.ScenarioEvolve(shaymin, C("PL-126"));
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(before + 40, e.MaxHp(turtwig));
            Assert.AreEqual(e.MaxHp(shaymin), C("PL-126").Hp, "not Shaymin itself");
        }

        [Test]
        public void Time_Crystal_Stops_Poke_Bodies_Of_Pokemon_That_Arent_SP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lotad = e.ScenarioActive(0, C("PL-SH4"));
            Attach(e, lotad, Water, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(lotad), "Swift Swim");
            var dialga = e.ScenarioBench(1, C("PL-7"));
            e.ScenarioEvolve(dialga, C("PL-122"));
            Assert.AreEqual(C("PL-SH4").RetreatCost, e.RetreatCostOf(lotad), "Time Crystal stops Swift Swim");
        }

        [Test]
        public void Galactic_Hq_Puts_2_Counters_On_An_Evolved_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Grotle"), new ScriptedAgent());
            var turtwig = e.ScenarioActive(0, C("PL-101"));
            e.ScenarioAddToHand(0, C("PL-49"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C("PL-106"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Grotle", turtwig.Name);
            Assert.AreEqual(2, turtwig.DamageCounters);
        }

        [Test]
        public void Miasma_Valley_Hurts_Basic_Pokemon_Benched_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C("PL-111"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench[0].DamageCounters);
        }

        [Test]
        public void Power_Spray_Stops_A_Poke_Power_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Temper Tantrum"), new ScriptedAgent().Then("Yes"));
            var banette = e.ScenarioActive(0, C("PL-19"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C("PL-17"));
            e.ScenarioBench(1, C("PL-40"));
            e.ScenarioBench(1, C("PL-41"));
            e.ScenarioAddToHand(1, C("PL-117"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, banette.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "nothing was discarded");
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name.Contains("Power Spray")));
        }

        [Test]
        public void Broken_Space_Blow_Sends_A_Knocked_Out_Pokemon_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Broken-space Blow"), new ScriptedAgent());
            var giratina = e.ScenarioActive(0, C("PL-27"));
            Attach(e, giratina, Psychic, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            chansey.DamageCounters = e.MaxHp(chansey) / 10 - 1;
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].LostZone.Count(c => c.Def.Name == "Chansey"));
        }

        [Test]
        public void Cold_Feet_Lets_A_Confused_Mightyena_Attack_For_Free()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mightyena = e.ScenarioActive(0, C("PL-54"));
            e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            var collude = mightyena.Def.Attacks.First(a => a.Name == "Collude");
            Assert.IsFalse(e.CanPayAttack(mightyena, collude));
            mightyena.Conditions = SpecialCondition.Confused;
            Assert.IsTrue(e.CanPayAttack(mightyena, collude));
        }
    }
}
