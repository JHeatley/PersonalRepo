using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v144: Expedition Base Set (E1): the card data and prints, e-card Poké-Powers (stopped by any Special Condition), the extra
    /// Energy attachments of Harvest Bounty, Burning Energy, Psymimic, the Trainers with their Compendium errata, and your rulings.
    /// </summary>
    public class E1Tests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Grass = "MEE-9";

        const string Alakazam = "E1-1";
        const string Ampharos = "E1-2";
        const string Blastoise = "E1-4";
        const string Butterfree = "E1-5";
        const string Charizard = "E1-6";
        const string Dragonite = "E1-9";
        const string Gengar = "E1-13";
        const string Golem = "E1-14";
        const string Magby = "E1-17";
        const string MewtwoE1 = "E1-20";
        const string Poliwrath = "E1-24";
        const string Tyranitar = "E1-29";
        const string Venusaur = "E1-30";
        const string Magmar = "E1-86";
        const string Metapod = "E1-87";
        const string Bulbasaur = "E1-94";
        const string Charmander = "E1-97";
        const string DualBall = "E1-139";
        const string MarysImpulse = "E1-142";
        const string MultiTM = "E1-144";
        const string PokemonReversal = "E1-146";
        const string ElmsMethod = "E1-148";
        const string StrengthCharm = "E1-150";
        const string Hitmonlee = "HSP-25";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";

        private static int SeedFor(params bool[] flips)
        {
            for (int seed = 1; seed < 100000; seed++)
            {
                var rng = new Rng(seed);
                bool match = true;
                foreach (var heads in flips)
                    if (rng.CoinFlip() != heads) { match = false; break; }
                if (match) return seed;
            }
            throw new InvalidOperationException("no seed for those flips");
        }

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
        public void Expedition_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "E1").ToList();
            Assert.AreEqual(115, set.Count, "1-32, the 6 other rares in 33-70, 71-136 and 11 new Trainers");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual(165, db.All.Count(d => d.SetCode == "E1"));
            Assert.AreEqual("E1-1", db.MainPrinting(db.Get("E1-33")).Key);
            Assert.AreEqual("E1-6", db.MainPrinting(db.Get("E1-40")).Key);
            Assert.AreEqual("E1-39", db.MainPrinting(db.Get("E1-39")).Key, "the other Charizard is its own card");
            Assert.AreEqual("BS-94", db.MainPrinting(db.Get("E1-156")).Key, "Potion: Base Set's text");
            Assert.AreEqual("N1-104", db.MainPrinting(db.Get("E1-158")).Key);
            Assert.AreEqual("CPA-51", db.MainPrinting(db.Get("E1-154")).Key);
            Assert.IsTrue(C(Magby).IsBaby && C(Magby).BabyEvolvesInto(C(Magmar)));
            Assert.IsTrue(C(MarysImpulse).IsSupporter && C(ElmsMethod).IsSupporter);
            Assert.IsTrue(C(StrengthCharm).IsTool && C(MultiTM).IsTool);
        }

        // ------------------------------------------------------------------ Poké-Powers and Poké-Bodies

        [Test]
        public void Poke_Powers_Stop_Under_Any_Special_Condition()
        {
            foreach (bool burned in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var lee = e.ScenarioActive(0, C(Hitmonlee));
                e.ApplyCondition(lee, SpecialCondition.Confused);
                var butterfree = e.ScenarioBench(0, C(Butterfree));
                if (burned) butterfree.Conditions |= SpecialCondition.Burned;
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!burned, Offered(e, 0, "Use Miraculous Powder"));
            }
        }

        [Test]
        public void Burning_Energy_Makes_Basic_Energy_Fire_For_The_Turn_Even_Energy_Attached_Later()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Burning Energy", "Attach Water Energy to Charmander"), new ScriptedAgent());
            var charmander = e.ScenarioActive(0, C(Charmander));
            Attach(e, charmander, Water, 1);
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioBench(0, C(Charizard));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            e.MainPhaseStep();
            Assert.AreEqual(2, e.EnergyCountOfType(charmander, EnergyType.Fire));
            Assert.IsTrue(Offered(e, 0, "Attack: Flare"), "your ruling: the Water Energy attached after the power is Fire too");
        }

        [Test]
        public void Harvest_Bounty_Gives_Each_Venusaur_One_More_Attachment()
        {
            foreach (int venusaurs in new[] { 1, 2 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attach Grass Energy to Bulbasaur", "Attach Grass Energy", "Attach Grass Energy"), new ScriptedAgent());
                var bulbasaur = e.ScenarioActive(0, C(Bulbasaur));
                for (int i = 0; i < venusaurs; i++) e.ScenarioBench(0, C(Venusaur));
                for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Grass));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.MainPhaseStep();
                Assert.AreEqual(venusaurs + 1, bulbasaur.Energy.Count);
            }
        }

        [Test]
        public void Psymimic_Uses_The_Opponents_Attacks_Unless_Alakazam_Has_A_Special_Condition()
        {
            foreach (bool poisoned in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var alakazam = e.ScenarioActive(0, C(Alakazam));
                Attach(e, alakazam, Psychic, 3);
                if (poisoned) e.ApplyCondition(alakazam, SpecialCondition.Poisoned);
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!poisoned, Offered(e, 0, "Attack: Double-Edge"));
            }
        }

        [Test]
        public void Tailwind_Takes_Away_The_Active_Pokemons_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Tailwind"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioBench(0, C(Dragonite));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(lee));
            e.MainPhaseStep();
            Assert.AreEqual(0, e.RetreatCostOf(lee));
        }

        [Test]
        public void Energy_Connect_Moves_Basic_Energy_From_The_Bench_As_Often_As_You_Like()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energy Connect", "Fighting Energy", "Use Energy Connect"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            var ampharos = e.ScenarioBench(0, C(Ampharos));
            Attach(e, ampharos, Fighting, 2);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            e.MainPhaseStep();
            Assert.AreEqual(2, lee.Energy.Count);
            Assert.IsFalse(Offered(e, 0, "Use Energy Connect"));
        }

        [Test]
        public void Jet_Stream_Discards_From_Both_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Jet Stream"), new ScriptedAgent());
            var blastoise = e.ScenarioActive(0, C(Blastoise));
            Attach(e, blastoise, Water, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            Attach(e, blissey, Psychic, 2);
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            e.MainPhaseStep();
            Assert.AreEqual(1, blastoise.Energy.Count);
            Assert.AreEqual(1, blissey.Energy.Count);
        }

        [Test]
        public void Chaos_Move_Needs_3_Or_Fewer_Opponent_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Chaos Move"), new ScriptedAgent());
            var gengar = e.ScenarioActive(0, C(Gengar));
            gengar.DamageCounters = 2;
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Chaos Move"));
            e.State.Players[1].Prizes.RemoveRange(0, 3);
            e.MainPhaseStep();
            Assert.AreEqual(1, gengar.DamageCounters);
            Assert.AreEqual(1, blissey.DamageCounters);
        }

        [Test]
        public void Plunge_Takes_The_Active_Pokemons_Energy_And_Switches_In_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Plunge"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 2);
            var poliwrath = e.ScenarioBench(0, C(Poliwrath));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            e.MainPhaseStep();
            Assert.AreSame(poliwrath, e.State.Players[0].Active);
            Assert.AreEqual(2, poliwrath.Energy.Count);
            Assert.AreEqual(0, lee.Energy.Count);
        }

        [Test]
        public void Dark_Aura_Makes_All_Of_Tyranitars_Energy_Darkness()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tyranitar = e.ScenarioActive(0, C(Tyranitar));
            Attach(e, tyranitar, Fire, 4);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(4, e.EnergyCountOfType(tyranitar, EnergyType.Darkness));
            Assert.IsTrue(Offered(e, 0, "Attack: Stamp"));
        }

        [Test]
        public void Rock_Body_And_Exoskeleton_Reduce_Attack_Damage()
        {
            foreach (string key in new[] { Golem, Metapod })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Kick"));
                var mon = e.ScenarioActive(0, C(key));
                var lee = e.ScenarioActive(1, C(Hitmonlee));
                Attach(e, lee, Fighting, 1);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(key == Golem ? 1 : 0, mon.DamageCounters);
            }
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Strength_Charm_Adds_10_Before_Weakness_And_Is_Discarded_After_Attacking()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioAttachTool(lee, C(StrengthCharm));
            var mewtwo = e.ScenarioActive(1, C(MewtwoE1));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, mewtwo.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(0, lee.Tools.Count);
        }

        [Test]
        public void Multi_Technical_Machine_01_Gives_Paralyzing_Gaze_For_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Paralyzing Gaze"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioAttachTool(lee, C(MultiTM));
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(blissey.Has(SpecialCondition.Paralyzed));
            e.EndTurn();
            Assert.AreEqual(0, lee.Tools.Count);
        }

        [Test]
        public void A_Technical_Machine_Doesnt_Take_The_Tools_Place()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAttachTool(lee, C(StrengthCharm));
            var tm = e.ScenarioAddToHand(0, C(MultiTM));
            var charm = e.ScenarioAddToHand(0, C(StrengthCharm));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.CanAttachToolTo(tm, lee), "Compendium: a Tool can share a Pokémon with a non-Tool Trainer");
            Assert.IsFalse(e.CanAttachToolTo(charm, lee));
            Assert.AreEqual(1, e.PokemonToolCount(lee));
        }

        [Test]
        public void Pokemon_Reversal_Switches_The_Chosen_Pokemon_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Pokémon Reversal"), new ScriptedAgent());
                e.ScenarioActive(0, C(Hitmonlee));
                e.ScenarioAddToHand(0, C(PokemonReversal));
                var blissey = e.ScenarioActive(1, C(Blissey));
                var chansey = e.ScenarioBench(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreSame(heads ? chansey : blissey, e.State.Players[1].Active);
            }
        }

        [Test]
        public void Professor_Elms_Training_Method_Needs_A_Deck()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(ElmsMethod));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Professor Elm's Training Method"));
            e.State.Players[0].Deck.Clear();
            Assert.IsFalse(Offered(e, 0, "Play Professor Elm's Training Method"));
        }

        [Test]
        public void Marys_Impulse_Draws_2_For_Each_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mary's Impulse"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(MarysImpulse));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Dual_Ball_Skips_Baby_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Dual Ball"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(DualBall));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C(Magby), 2);
            e.ScenarioAddToDeck(0, C(Bulbasaur), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true));
            Step(e);
            Assert.AreEqual("Bulbasaur", e.State.Players[0].Hand.Single().Def.Name);
        }
    }
}
