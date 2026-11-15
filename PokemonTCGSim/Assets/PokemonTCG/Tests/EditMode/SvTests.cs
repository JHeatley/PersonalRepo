using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: Supreme Victors (SV): the card data and prints, and the new effects (Doom News, Gravitation, Green Aroma, Dragon
    /// Intimidation, Silver Feather, Focus Wall, Marvel Shell, Intimidating Pattern, Champion's Room).
    /// </summary>
    public class SvTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private GameEngine MewAttacks(out PokemonInPlay mew)
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            return e;
        }

        [Test]
        public void Supreme_Victors_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(145, db.Cards.Count(d => d.SetCode == "SV"));
            Assert.AreEqual("CEL-CC18", db.MainPrinting(C("SV-145")).Key);
            Assert.AreEqual("DPP-47", db.MainPrinting(C("SV-146")).Key);
        }

        [Test]
        public void Every_Supreme_Victors_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "SV" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Doom_News_Knocks_Out_The_Defending_Pokemon_A_Turn_Later()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Doom News"), new ScriptedAgent());
            var absol = e.ScenarioActive(0, C("SV-1"));
            Attach(e, absol, "MEE-15", 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, absol.Energy.Count);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "the Energy went back to the hand");
            e.EndTurn();
            e.StartTurn();
            Assert.IsTrue(e.State.Players[1].InPlay().Contains(chansey));
            e.EndTurn();
            Assert.IsFalse(e.State.Players[1].InPlay().Contains(chansey));
        }

        [Test]
        public void Gravitation_Takes_20_Hp_From_Every_Pokemon_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(1, C(Chansey));
            int hp = e.MaxHp(chansey);
            e.ScenarioActive(0, C("SV-7"));
            e.ScenarioBench(0, C("SV-7"));
            Assert.AreEqual(hp - 20, e.MaxHp(chansey));
        }

        [Test]
        public void Green_Aroma_Keeps_Grass_Pokemon_Free_Of_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var turtwig = e.ScenarioActive(0, C("PL-101"));
            e.ScenarioBench(0, C("SV-13"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.ApplyCondition(turtwig, SpecialCondition.Poisoned));
            Assert.IsFalse(turtwig.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Dragon_Intimidation_Returns_An_Energy_Card_From_The_Attacker()
        {
            PokemonInPlay mew;
            var e = MewAttacks(out mew);
            e.ScenarioActive(0, C("SV-5"));
            e.ScenarioBench(0, C(Chansey));
            Step(e);
            Assert.AreEqual(1, mew.Energy.Count);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.IsEnergy));
        }

        [Test]
        public void Silver_Feather_Hurts_Basic_Pokemon_Benched_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Silver Feather"), new ScriptedAgent().Then("Bench Chansey"));
            var skarmory = e.ScenarioActive(0, C("SV-83"));
            Attach(e, skarmory, "MEE-16", 1);
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

        [Test]
        public void Focus_Wall_Survives_A_Big_Hit_At_10_Hp()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Double-edge"));
            var mime = e.ScenarioActive(0, C("SV-37"));
            e.ScenarioBench(0, C(Chansey));
            var golem = e.ScenarioActive(1, C("RR-23"));
            Attach(e, golem, "MEE-14", 4);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsTrue(e.State.Players[0].InPlay().Contains(mime));
            Assert.AreEqual(10, e.MaxHp(mime) - mime.DamageCounters * 10);
        }

        [Test]
        public void Marvel_Shell_Stops_Pokemon_With_Poke_Powers_Or_Bodies()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Darkness Sleep"));
            var shedinja = e.ScenarioActive(0, C("SV-44"));
            e.ScenarioBench(0, C(Chansey));
            var darkrai = e.ScenarioActive(1, C("RR-3"));
            Attach(e, darkrai, "MEE-15", 3);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, shedinja.DamageCounters);
        }

        [Test]
        public void Intimidating_Pattern_Weakens_Attacks_On_Your_Pokemon()
        {
            PokemonInPlay mew;
            var e = MewAttacks(out mew);
            var masquerain = e.ScenarioActive(0, C("SV-68"));
            e.ScenarioBench(0, C(Chansey));
            Step(e);
            int full = mew.Def.Attacks.First(a => a.Name == "Super Psy Bolt").Damage;
            Assert.AreEqual((full - 20) / 10, masquerain.DamageCounters);
        }

        [Test]
        public void Champions_Room_Lowers_The_Retreat_Cost_Of_Pokemon_SP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var weavile = e.ScenarioActive(0, C("PL-17"));
            e.ScenarioActive(1, C(Chansey));
            int before = e.RetreatCostOf(weavile);
            e.ScenarioSetStadium(0, C("SV-135"));
            Assert.AreEqual(System.Math.Max(0, before - 1), e.RetreatCostOf(weavile));
        }
    }
}
