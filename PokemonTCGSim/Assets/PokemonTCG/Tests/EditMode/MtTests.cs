using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Mysterious Treasures (MT): the card data and prints, and the new effects (Power Cancel, Lake Boundary, Dark Genes,
    /// Dragon DNA, Airy Wall, the lake trio, Sticky, EQUIP, Team Galactic's Wager, Quick Ball, Wake-up Punch, Shocking Tail).
    /// </summary>
    public class MtTests : ScenarioTestBase
    {
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
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
        public void Mysterious_Treasures_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(114, db.Cards.Count(d => d.SetCode == "MT"));
            for (int n = 1; n <= 124; n++) Assert.IsNotNull(db.Get("MT", n.ToString()), "MT " + n);
            Assert.AreEqual("DPP-16", db.MainPrinting(C("MT-94")).Key, "Pikachu");
            Assert.AreEqual("DPP-12", db.MainPrinting(C("MT-122")).Key, "Lucario LV.X");
            Assert.AreEqual("MT-114", db.MainPrinting(C("MT-114")).Key, "this Quick Ball is its own card");
            Assert.IsTrue(C("MT-116").FossilGivesPrize, "Armor Fossil gives a Prize card");
        }

        [Test]
        public void Every_Mysterious_Treasures_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "MT" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Power_Cancel_Stops_A_Poke_Power_And_It_Still_Counts_As_Used()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Miracle Oracle"), new ScriptedAgent());
            var bronzong = e.ScenarioActive(0, C("MT-74"));
            e.ScenarioEvolve(bronzong, C("MT-6"));
            e.ScenarioActive(1, C("MT-2"));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "Miracle Oracle drew nothing");
            Assert.AreEqual(0, e.State.Players[1].Hand.Count, "Alakazam's owner discarded 2 cards");
            Assert.IsFalse(Offered(e, 0, "Use Miracle Oracle"), "it counts as used");
        }

        [Test]
        public void Lake_Boundary_Makes_Weakness_Times_Two()
        {
            foreach (bool stadium in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Electromagnetic Jam"), new ScriptedAgent());
                var electrike = e.ScenarioActive(0, C("MT-81"));
                Attach(e, electrike, Lightning, 1);
                var gyarados = e.ScenarioActive(1, C("MT-89"));
                e.ScenarioEvolve(gyarados, C("MT-26"));
                if (stadium) e.ScenarioSetStadium(0, C("MT-112"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(stadium ? 2 : 4, gyarados.DamageCounters, stadium ? "10 x 2" : "10 + 30");
            }
        }

        [Test]
        public void Dark_Genes_Lets_Murkrow_Use_Honchkrows_Attack_For_Free()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("MT-90"));
            var honchkrow = e.ScenarioBench(0, C("MT-90"));
            e.ScenarioEvolve(honchkrow, C("MT-10"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Dark Wing Flaps"), "Honchkrow doesn't have its Energy yet");
            Attach(e, honchkrow, Darkness, 3);
            Assert.IsTrue(Offered(e, 0, "Dark Wing Flaps"), "the Active Murkrow can use it with no Energy");
        }

        [Test]
        public void Dragon_Dna_Uses_Magikarps_Attacks_With_30_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flail"), new ScriptedAgent());
            var gyarados = e.ScenarioActive(0, C("MT-89"));
            e.ScenarioEvolve(gyarados, C("MT-26"));
            Attach(e, gyarados, Psychic, 1);
            gyarados.DamageCounters = 2;
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, chansey.DamageCounters, "Flail 20 + 30");
        }

        [Test]
        public void Airy_Wall_Stops_Damage_From_Pokemon_With_2_Energy_Or_Less()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var mime = e.ScenarioActive(0, C("MT-30"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, mime.DamageCounters);
        }

        [Test]
        public void Upper_Material_Frees_Every_Lake_Trio_Pokemon_And_Downer_Material_Taxes_Basics()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mesprit = e.ScenarioActive(0, C("MT-14"));
            e.ScenarioBench(0, C("MT-18"));
            var opposingUxie = e.ScenarioBench(1, C("MT-18"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(1, e.RetreatCostOf(mesprit), "no Azelf yet");
            Assert.IsTrue(e.CanPayAttack(mew, mew.Def.Attacks.First(a => a.Name == "Super Psy Bolt")));
            e.ScenarioBench(0, C("MT-4"));
            Assert.AreEqual(0, e.RetreatCostOf(mesprit));
            Assert.AreEqual(0, e.RetreatCostOf(opposingUxie), "the opponent's Uxie too");
            Assert.IsFalse(e.CanPayAttack(mew, mew.Def.Attacks.First(a => a.Name == "Super Psy Bolt")), "Downer Material: 1 Colorless more");
        }

        [Test]
        public void Sticky_Raises_Every_Retreat_Cost_But_Ariados()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ariados = e.ScenarioActive(0, C("MT-103"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            int before = e.RetreatCostOf(chansey);
            e.ScenarioEvolve(ariados, C("MT-20"));
            Assert.AreEqual(1, e.RetreatCostOf(ariados));
            Assert.AreEqual(before + 1, e.RetreatCostOf(chansey));
        }

        [Test]
        public void Equip_Attaches_Unown_E_As_A_Tool_For_10_More_Hp()
        {
            var e = Scenario(new ScriptedAgent().Then("Use EQUIP"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            var unown = e.ScenarioBench(0, C("MT-65"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            int hp = e.MaxHp(chansey);
            Step(e);
            Assert.IsFalse(e.State.Players[0].Bench.Contains(unown));
            Assert.IsTrue(chansey.HasToolNamed("Unown [E]"));
            Assert.AreEqual(hp + 10, e.MaxHp(chansey));
        }

        [Test]
        public void Team_Galactics_Wager_Winner_Draws_6_Loser_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Galactic's Wager", "Paper"), new ScriptedAgent().Then("Rock"));
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("MT-115"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count, "paper beats rock");
            Assert.AreEqual(3, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Quick_Ball_Takes_The_First_Pokemon_Revealed()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Quick Ball"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(0, C("MT-99"), 1);
            e.ScenarioAddToHand(0, C("MT-114"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Slakoth"));
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Wake_Up_Punch_Hits_Hard_After_Evolving_From_An_Asleep_Slakoth()
        {
            foreach (bool asleep in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Wake-up Punch"), new ScriptedAgent());
                var slakoth = e.ScenarioActive(0, C("MT-99"));
                Attach(e, slakoth, Psychic, 1);
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.ScenarioEvolve(slakoth, C("MT-68"));
                slakoth.TurnEvolved = e.State.TurnNumber; // as if it evolved from hand this turn (evolving wakes it up)
                if (asleep) slakoth.ConditionsBeforeEvolve = SpecialCondition.Asleep;
                Step(e);
                Assert.AreEqual(asleep ? 6 : 1, chansey.DamageCounters);
            }
        }

        [Test]
        public void Shocking_Tail_Hurts_Pokemon_That_Get_Energy_From_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach"));
            var electivire = e.ScenarioActive(0, C("DP-3"));
            e.ScenarioEvolve(electivire, C("MT-121"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.AreEqual(2, chansey.DamageCounters);
        }

        [Test]
        public void Fossil_Excavator_Takes_A_Fossil_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Fossil Excavator", "Discard"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDiscard(0, C("MT-116"), 1);
            e.ScenarioAddToHand(0, C("MT-111"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Armor Fossil"));
        }
    }
}
