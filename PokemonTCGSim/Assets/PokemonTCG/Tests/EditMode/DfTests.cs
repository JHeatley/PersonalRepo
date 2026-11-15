using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v171: Dragon Frontiers (DF): the card data and prints, and the new effects (Holon Veil, Battle Aura, Sharing, Volunteer,
    /// Delta Copy, Holon Legacy, Island Hermit, Dragon Roar, Shock-wave, Rage Aura, Type Shift, Mimicry, Rainbow Wave, Tropical
    /// Heal, Extra Smoke, Dozing and Bedhead).
    /// </summary>
    public class DfTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
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
        public void Dragon_Frontiers_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(80, db.Cards.Count(d => d.SetCode == "DF"));
            for (int n = 1; n <= 101; n++) Assert.IsNotNull(db.Get("DF", n.ToString()), "DF " + n);
            Assert.AreEqual("CEL-CC14", db.MainPrinting(C("DF-93")).Key, "Gardevoir ex δ");
            Assert.AreEqual("DS-105", db.MainPrinting(C("DF-85")).Key, "Holon Energy GL");
            Assert.AreEqual("E2-145", db.MainPrinting(C("DF-87")).Key, "Boost Energy");
            Assert.AreNotEqual("TWM-49", db.MainPrinting(C("DF-49")).Key, "Feebas δ is a Fire Pokémon, its own card");
            Assert.IsTrue(C("DF-1").IsDeltaSpecies && C("DF-101").IsPokemonStar && C("DF-99").IsOldEx);
        }

        [Test]
        public void Every_Dragon_Frontiers_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "DF" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Holon_Veil_Makes_Your_Pokemon_Delta()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Delta Circle"), new ScriptedAgent());
            var ampharos = e.ScenarioActive(0, C("DF-1"));
            Attach(e, ampharos, Psychic, 3);
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioBench(0, C(Chansey));
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, target.DamageCounters, "20 + 3 δ Pokémon x 10 (the 2 Chansey count)");
        }

        [Test]
        public void Battle_Aura_Adds_10_To_Your_Delta_Pokemons_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Peck"), new ScriptedAgent());
            var nidoran = e.ScenarioActive(0, C("DF-57"));
            Attach(e, nidoran, Darkness, 1);
            e.ScenarioBench(0, C("DF-2"));
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, target.DamageCounters, "Peck: 10 + 10");
        }

        [Test]
        public void Sharing_Uses_A_Supporter_From_The_Opponents_Hand_And_Leaves_It_There()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Sharing", "Use Island Hermit"), new ScriptedAgent());
            e.ScenarioActive(0, C("DF-5"));
            e.ScenarioActive(1, C(Chansey));
            var hermit = e.ScenarioAddToHand(1, C("DF-76"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "drew 2");
            Assert.IsTrue(e.State.Players[1].Hand.Contains(hermit));
            Assert.IsFalse(e.State.Players[0].SupporterPlayedThisTurn, "doesn't use up your Supporter");
        }

        [Test]
        public void Volunteer_Heals_And_Swaps_Ninetales_For_Another_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Volunteer"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C("CG-30")); // any Pokémon to hold the Vulpix stack
            mon.Stack.Clear();
            e.ScenarioEvolve(mon, C("DF-70"));
            e.ScenarioEvolve(mon, C("DF-8"));
            mon.DamageCounters = 5;
            e.ScenarioAddToDeck(0, C("DF-8"), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mon.DamageCounters, "4 removed");
            Assert.AreEqual("Ninetales", mon.Name);
            Assert.AreEqual(2, mon.Stack.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Ninetales"));
        }

        [Test]
        public void Delta_Copy_Uses_An_Opponents_Delta_Attack_Without_Its_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Delta Copy", "Sky-high Claws"), new ScriptedAgent());
            var togetic = e.ScenarioActive(0, C("DF-11"));
            Attach(e, togetic, Water, 2);
            var rayquaza = e.ScenarioActive(1, C("DF-97"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, rayquaza.DamageCounters, "Sky-high Claws (LLCC) with only 2 Water Energy");
        }

        [Test]
        public void Holon_Legacy_Removes_Weakness_And_Poke_Powers_Of_Delta_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("DF-7"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Use Invitation"));
            e.ScenarioSetStadium(1, C("DF-74"));
            Assert.IsFalse(Offered(e, 0, "Use Invitation"));
        }

        [Test]
        public void Island_Hermit_Turns_Prize_Cards_Face_Up_And_Draws_2()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Island Hermit", "2 Prize"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("DF-76"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].FaceUpPrizeIds.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Dragon_Roar_Spills_Extra_Counters_Onto_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragon Roar"), new ScriptedAgent());
            var dragonite = e.ScenarioActive(0, C("DF-91"));
            Attach(e, dragonite, Grass, 4);
            var target = e.ScenarioActive(1, C(Chansey));
            target.DamageCounters = 7; // 3 counters Knock it Out
            var benched = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, benched.DamageCounters, "8 - 3");
        }

        [Test]
        public void Shock_Wave_Knocks_Out_A_Marked_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Electromark", "Chansey", "Attack: Shock-wave"), new ScriptedAgent());
            var tyranitar = e.ScenarioActive(0, C("DF-99"));
            Attach(e, tyranitar, Lightning, 3);
            e.ScenarioActive(1, C(MewEx));
            var benched = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(benched.Markers.ContainsKey(CardsDF.ShockWaveMarker));
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.IsFalse(e.State.Players[1].Bench.Contains(benched), "Knocked Out");
        }

        [Test]
        public void Rage_Aura_Drops_The_Colorless_Cost_When_Behind_On_Prizes()
        {
            foreach (bool behind in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var rayquaza = e.ScenarioActive(0, C("DF-97"));
                Attach(e, rayquaza, Lightning, 2);
                e.ScenarioActive(1, C(Chansey));
                if (behind) e.State.Players[1].Prizes.RemoveAt(0);
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(behind, Offered(e, 0, "Attack: Sky-high Claws"), "LLCC with 2 Lightning");
            }
        }

        [Test]
        public void Type_Shift_Makes_Salamence_ex_Fire_Until_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Type Shift"), new ScriptedAgent());
            var salamence = e.ScenarioActive(0, C("DF-98"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.IsType(salamence, EnergyType.Water));
            Step(e);
            Assert.IsTrue(e.IsType(salamence, EnergyType.Fire));
            Assert.IsFalse(e.IsType(salamence, EnergyType.Water));
        }

        [Test]
        public void Mimicry_Does_Nothing_Without_The_Energy_For_The_Copied_Attack()
        {
            foreach (bool energy in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Mimicry"), new ScriptedAgent());
                var mew = e.ScenarioActive(0, C("DF-101"));
                Attach(e, mew, Water, 1);
                if (energy) Attach(e, mew, Psychic, 2);
                var chansey = e.ScenarioActive(1, C(Chansey));
                var target = e.State.Players[1].Active;
                e.ScenarioSetTurn(0, 3);
                Step(e);
                // Chansey's attacks: Rollout (C, 10) is chosen first.
                Assert.AreEqual(1, target.DamageCounters, "Rollout costs C, which Mew ★ always has");
            }
        }

        [Test]
        public void Rainbow_Wave_Hits_Each_Opponent_Pokemon_Of_The_Chosen_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rainbow Wave"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C("DF-101"));
            Attach(e, mew, Water, 1);
            var active = e.ScenarioActive(1, C("CG-14"));
            var waterBench = e.ScenarioBench(1, C("CG-64"));
            var other = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, active.DamageCounters);
            Assert.AreEqual(1, waterBench.DamageCounters, "20 - 10 (Solid Shell)");
            Assert.AreEqual(0, other.DamageCounters);
        }

        [Test]
        public void Tropical_Heal_Clears_Conditions_And_Markers()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Tropius"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            active.Conditions = SpecialCondition.Poisoned;
            active.Markers[CardsDF.ShockWaveMarker] = 1;
            e.ScenarioAddToHand(0, C("DF-23"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(active.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(active.Markers.ContainsKey(CardsDF.ShockWaveMarker));
        }

        [Test]
        public void Extra_Smoke_Cuts_Damage_To_Your_Stage_2_Pokemon_ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var salamence = e.ScenarioActive(0, C("DF-98"));
            e.ScenarioBench(0, C("DF-94"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, salamence.DamageCounters, "30 - 10");
        }

        [Test]
        public void Dozing_Heals_And_Puts_Snorlax_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dozing"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C("DF-10"));
            snorlax.DamageCounters = 3;
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, snorlax.DamageCounters);
            Assert.IsTrue(snorlax.Has(SpecialCondition.Asleep));
        }
    }
}
