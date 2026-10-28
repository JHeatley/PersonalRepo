using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v135: HGSS Black Star Promos (HSP HGSS01-HGSS25): the card data (Ho-Oh's 100 HP, Pikachu's +10 Weakness, Unlimited only),
    /// the Poké-Powers (Night Scope, Rain Dance, Leaf Trans, Afterburner) and Poké-Bodies (Tenacious Bind, Luster Float, Sweet
    /// Sleeping Face, Fermenting Liquid, Shortcut, which stacks by your ruling), Tropical Tidal Wave (Tools and Stadium only, your
    /// ruling) and the attacks with effects.
    /// </summary>
    public class HspTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string HoOh = "HSP-1";
        const string Pikachu = "HSP-3";
        const string Wobbuffet = "HSP-4";
        const string Noctowl = "HSP-6";
        const string Feraligatr = "HSP-7";
        const string Meganium = "HSP-8";
        const string Typhlosion = "HSP-9";
        const string Latias = "HSP-10";
        const string Latios = "HSP-11";
        const string Cleffa = "HSP-12";
        const string Smoochum = "HSP-13";
        const string Lapras = "HSP-14";
        const string Shuckle = "HSP-15";
        const string Plusle = "HSP-16";
        const string Minun = "HSP-17";
        const string TropicalTidalWave = "HSP-18";
        const string Suicune = "HSP-21";
        const string Porygon = "HSP-22";
        const string Porygon2 = "HSP-23";
        const string Hitmonlee = "HSP-25";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string FightingFuryBelt = "BKP-99";
        const string FadedTown = "AOR-73";

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

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
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
        public void The_Promos_Are_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "HSP").ToList();
            Assert.AreEqual(25, set.Count);
            Assert.IsTrue(set.All(d => d.UnlimitedOnly), "every card came out before Black & White");
            Assert.AreEqual(100, C(HoOh).Hp, "Ho-Oh has 100 HP (Limitless)");
            Assert.AreEqual(10, C(Pikachu).WeaknessAmount, "Pikachu's Weakness is +10");
            Assert.AreEqual(20, C(Pikachu).ResistanceAmount);
            Assert.AreEqual(AbilityDef.PokePower, C(Feraligatr).Abilities[0].Kind);
            Assert.AreEqual(AbilityDef.PokeBody, C(Wobbuffet).Abilities[0].Kind);
            Assert.IsTrue(C(TropicalTidalWave).IsTrainer);
        }

        [Test]
        public void Pikachus_Weakness_Adds_10()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var pikachu = e.ScenarioActive(1, C(Pikachu));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, pikachu.DamageCounters, "20 + 10");
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Sacred_Fire_Does_80_On_Heads_Without_Weakness()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Sacred Fire"), new ScriptedAgent());
                var hooh = e.ScenarioActive(0, C(HoOh));
                Attach(e, hooh, Fire, 4);
                var meganium = e.ScenarioActive(1, C(Meganium));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 8 : 0, meganium.DamageCounters, heads ? "80, not doubled by Fire Weakness" : "tails: nothing");
            }
        }

        [Test]
        public void Hydro_Crunch_Adds_10_For_Each_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hydro Crunch"), new ScriptedAgent());
            var feraligatr = e.ScenarioActive(0, C(Feraligatr));
            Attach(e, feraligatr, Water, 4);
            var blissey = e.ScenarioActive(1, C(Blissey));
            blissey.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters, "3 + (60 + 30)");
        }

        [Test]
        public void Infinite_Wind_Heals_The_Bench_Only_With_Latios_There()
        {
            foreach (bool latios in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Infinite Wind"), new ScriptedAgent());
                var latias = e.ScenarioActive(0, C(Latias));
                Attach(e, latias, Psychic, 3);
                var lee = e.ScenarioBench(0, C(Hitmonlee));
                lee.DamageCounters = 3;
                if (latios) e.ScenarioBench(0, C(Latios));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(latios ? 1 : 3, lee.DamageCounters);
            }
        }

        [Test]
        public void Eeeeeeek_Draws_6_And_Puts_Cleffa_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Eeeeeeek"), new ScriptedAgent());
            var cleffa = e.ScenarioActive(0, C(Cleffa));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
            Assert.AreEqual(17, e.State.Players[0].Deck.Count);
            Assert.IsTrue(cleffa.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Energy_Antics_Moves_The_Opponents_Energy_And_Puts_Smoochum_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Energy Antics"), new ScriptedAgent());
            var smoochum = e.ScenarioActive(0, C(Smoochum));
            var chansey = e.ScenarioActive(1, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            var blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.Energy.Count);
            Assert.AreEqual(1, blissey.Energy.Count);
            Assert.IsTrue(smoochum.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Tag_Team_Boost_Adds_20_With_Plusle_On_The_Bench()
        {
            foreach (bool plusle in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Tag Team Boost"), new ScriptedAgent());
                var minun = e.ScenarioActive(0, C(Minun));
                Attach(e, minun, Lightning, 1);
                if (plusle) e.ScenarioBench(0, C(Plusle));
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(plusle ? 3 : 1, blissey.DamageCounters);
            }
        }

        [Test]
        public void Sheer_Cold_Stops_The_Defending_Pokemon_Attacking_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Sheer Cold"), new ScriptedAgent());
                var suicune = e.ScenarioActive(0, C(Suicune));
                Attach(e, suicune, Water, 3);
                var lee = e.ScenarioActive(1, C(Hitmonlee));
                Attach(e, lee, Fighting, 1);
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                NextTurn(e);
                Assert.AreEqual(!heads, e.GetMainActions(1).Any(o => o.Action == ActionType.Attack));
            }
        }

        [Test]
        public void Version_Update_Finds_Porygon2()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Version Update"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(Porygon));
            Attach(e, porygon, Psychic, 2);
            e.ScenarioAddToDeck(0, C(Porygon2), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Porygon2));
        }

        // ------------------------------------------------------------------ Poké-Powers

        [Test]
        public void Night_Scope_Works_Once_A_Turn_And_Not_With_A_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Night Scope"), new ScriptedAgent());
            var noctowl = e.ScenarioActive(0, C(Noctowl));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Night Scope"));
            Step(e);
            Assert.IsFalse(Offered(e, 0, "Night Scope"), "once during your turn");
            var asleep = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sleepy = asleep.ScenarioActive(0, C(Noctowl));
            asleep.ScenarioActive(1, C(Chansey));
            asleep.ScenarioAddToHand(1, C(Psychic));
            asleep.ScenarioSetTurn(0, 3);
            asleep.ApplyCondition(sleepy, SpecialCondition.Poisoned);
            Assert.IsFalse(Offered(asleep, 0, "Night Scope"), "a Poké-Power can't be used with any Special Condition");
        }

        [Test]
        public void Rain_Dance_Attaches_Water_Energy_From_The_Hand_To_Water_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Rain Dance", "Lapras", "Use Rain Dance", "Feraligatr"), new ScriptedAgent());
            var feraligatr = e.ScenarioActive(0, C(Feraligatr));
            var lapras = e.ScenarioBench(0, C(Lapras));
            e.ScenarioBench(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(1, lapras.Energy.Count);
            Assert.AreEqual(1, feraligatr.Energy.Count);
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn, "it doesn't use the turn's attachment");
            e.ScenarioAddToHand(0, C(Water));
            e.ApplyCondition(feraligatr, SpecialCondition.Poisoned);
            Assert.IsFalse(Offered(e, 0, "Use Rain Dance"), "not with a Special Condition");
        }

        [Test]
        public void Leaf_Trans_Moves_Grass_Energy_Between_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Leaf Trans"), new ScriptedAgent());
            var meganium = e.ScenarioActive(0, C(Meganium));
            Attach(e, meganium, Grass, 1);
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, meganium.Energy.Count);
            Assert.AreEqual(1, lee.Energy.Count);
            Attach(e, meganium, Grass, 1);
            e.ApplyCondition(meganium, SpecialCondition.Confused);
            Assert.IsFalse(Offered(e, 0, "Use Leaf Trans"), "not with a Special Condition");
        }

        [Test]
        public void Afterburner_Attaches_Fire_Energy_From_The_Discard_Pile_With_A_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Afterburner", "Hitmonlee"), new ScriptedAgent());
            e.ScenarioActive(0, C(Typhlosion));
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            e.ScenarioAddToDiscard(0, C(Fire), 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, lee.Energy.Count);
            Assert.AreEqual(1, lee.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
            Assert.IsFalse(Offered(e, 0, "Afterburner"), "once during your turn");
        }

        // ------------------------------------------------------------------ Poké-Bodies

        [Test]
        public void Tenacious_Bind_Adds_2_To_The_Opponents_Active_Retreat_Cost_From_The_Active_Spot()
        {
            foreach (bool active in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                if (active) e.ScenarioActive(0, C(Wobbuffet));
                else
                {
                    e.ScenarioActive(0, C(Chansey));
                    e.ScenarioBench(0, C(Wobbuffet));
                }
                var lee = e.ScenarioActive(1, C(Hitmonlee));
                e.ScenarioSetTurn(1, 4);
                Assert.AreEqual(active ? 4 : 2, e.RetreatCostOf(lee));
            }
        }

        [Test]
        public void Luster_Float_Gives_Latios_No_Retreat_Cost_With_Latias_In_Play()
        {
            foreach (bool latias in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var latios = e.ScenarioActive(0, C(Latios));
                if (latias) e.ScenarioBench(0, C(Latias));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(latias ? 0 : 2, e.RetreatCostOf(latios));
            }
        }

        [Test]
        public void Sweet_Sleeping_Face_Prevents_Attack_Damage_While_Asleep()
        {
            foreach (bool asleep in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
                var lee = e.ScenarioActive(0, C(Hitmonlee));
                Attach(e, lee, Fighting, 1);
                var cleffa = e.ScenarioActive(1, C(Cleffa));
                if (asleep) e.ApplyCondition(cleffa, SpecialCondition.Asleep);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(asleep ? 0 : 2, cleffa.DamageCounters);
            }
        }

        [Test]
        public void Fermenting_Liquid_Draws_When_Energy_Is_Attached_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Fighting Energy to Shuckle"), new ScriptedAgent());
            var shuckle = e.ScenarioActive(0, C(Shuckle));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, shuckle.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "drew a card");
        }

        [Test]
        public void Shortcut_Stacks_For_Each_Porygon2()
        {
            var one = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var first = one.ScenarioActive(0, C(Porygon2));
            one.ScenarioActive(1, C(Chansey));
            one.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, one.RetreatCostOf(first), "2 - 1");

            var two = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = two.ScenarioActive(0, C(Porygon2));
            two.ScenarioBench(0, C(Porygon2));
            var porygon = two.ScenarioBench(0, C(Porygon));
            var lee = two.ScenarioBench(0, C(Hitmonlee));
            two.ScenarioActive(1, C(Chansey));
            two.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, two.RetreatCostOf(active), "2 - 1 - 1 (your ruling: it stacks)");
            Assert.AreEqual(0, two.RetreatCostOf(porygon));
            Assert.AreEqual(2, two.RetreatCostOf(lee), "only the Porygon line");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Tropical_Tidal_Wave_Discards_The_Losers_Tools_And_Stadium()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Tropical Tidal Wave"), new ScriptedAgent());
                var mine = e.ScenarioActive(0, C(Chansey));
                e.ScenarioAttachTool(mine, C(FightingFuryBelt));
                var theirs = e.ScenarioActive(1, C(Chansey));
                e.ScenarioAttachTool(theirs, C(FightingFuryBelt));
                e.ScenarioBench(1, C(Hitmonlee));
                e.ScenarioSetStadium(1, C(FadedTown));
                e.ScenarioAddToHand(0, C(TropicalTidalWave));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 1 : 0, mine.Tools.Count, "yours go on tails");
                Assert.AreEqual(heads ? 0 : 1, theirs.Tools.Count, "theirs go on heads");
                Assert.AreEqual(heads, e.State.Stadium == null, "the opponent's Stadium goes on heads");
                Assert.AreEqual(1, e.State.Players[1].Bench.Count, "Pokémon in play are never discarded");
            }
        }
    }
}
