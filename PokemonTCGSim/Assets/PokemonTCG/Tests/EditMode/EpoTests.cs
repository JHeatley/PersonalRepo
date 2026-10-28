using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v131: Emerging Powers (EPO 1-98): the prints (your ruling: Recycle is its own Expanded card), the energy "Power" attacks,
    /// Encore, Nurturing, Final Gambit, Phat Sound, Telekinesis and Feint Attack (your ruling: only the chosen Pokémon is hit),
    /// Smack Down, Shear, Black Eyes, Power Whip, Quick Tail Smash, Rushing Water, Giga Drain and Recycle.
    /// </summary>
    public class EpoTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";

        const string Simisage = "EPO-2";
        const string Sewaddle = "EPO-4";
        const string Swadloon = "EPO-5";
        const string Leavanny = "EPO-7";
        const string Whimsicott = "EPO-12";
        const string Virizion = "EPO-17";
        const string Simipour = "EPO-23";
        const string Basculin = "EPO-24";
        const string Swanna = "EPO-27";
        const string Swoobat = "EPO-37";
        const string Sigilyph = "EPO-41";
        const string Boldore = "EPO-51";
        const string Gigalith = "EPO-53";
        const string Krookodile = "EPO-62";
        const string Liepard = "EPO-65";
        const string Ferrothorn = "EPO-73";
        const string Watchog = "EPO-79";
        const string Recycle = "EPO-96";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string Eevee = "DEX-84";
        const string Hooligans = "DEX-95";

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

        /// <summary>The attacker (player 0) against Chansey, with Blissey on the opponent's Bench.</summary>
        private GameEngine Attacking(string attacker, string[] labels, out PokemonInPlay mon, out PokemonInPlay chansey, out PokemonInPlay blissey)
        {
            var e = Scenario(new ScriptedAgent().Then(labels), new ScriptedAgent());
            mon = e.ScenarioActive(0, C(attacker));
            chansey = e.ScenarioActive(1, C(Chansey));
            blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            return e;
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(85, db.Cards.Count(d => d.SetCode == "EPO"));
            Assert.AreEqual("LTR-50", C("EPO-35").BaseKey, "Thundurus");
            Assert.AreEqual("LTR-72", C("EPO-47").BaseKey, "Gothitelle");
            Assert.AreEqual("LTR-109", C("EPO-90").BaseKey, "Bianca");
            Assert.AreEqual("GRI-128", C("EPO-94").BaseKey, "Max Potion");
            Assert.AreEqual("LTR-108", C("EPO-98").BaseKey, "the full art Tornadus");
            Assert.AreEqual(Recycle, C(Recycle).BaseKey, "its own card, not Fossil's (your ruling)");
            Assert.IsFalse(C(Recycle).UnlimitedOnly, "Expanded");
            Assert.AreEqual(0, C(Sigilyph).Attacks[1].Damage, "Telekinesis has no printed damage for the Defending Pokémon (your ruling)");
        }

        // ------------------------------------------------------------------ Grass, Fire and Water

        [Test]
        public void Fires_Power_Burns_Only_With_Fire_Energy_Attached()
        {
            foreach (bool fire in new[] { true, false })
            {
                PokemonInPlay simisage, chansey, blissey;
                var e = Attacking(Simisage, new[] { "Attack: Fire's Power" }, out simisage, out chansey, out blissey);
                Attach(e, simisage, Psychic, 1);
                Attach(e, simisage, fire ? Fire : Psychic, 1);
                Step(e);
                Assert.AreEqual(3, chansey.DamageCounters);
                Assert.AreEqual(fire, chansey.Has(SpecialCondition.Burned));
            }
        }

        [Test]
        public void Encore_Leaves_The_Defending_Pokemon_Only_The_Chosen_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Encore", "Seed Bomb"), new ScriptedAgent());
            var whimsicott = e.ScenarioActive(0, C(Whimsicott));
            Attach(e, whimsicott, Psychic, 1);
            var simisage = e.ScenarioActive(1, C(Simisage));
            Attach(e, simisage, Grass, 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var labels = e.GetMainActions(1).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Any(l => l.Contains("Seed Bomb")), string.Join(" | ", labels.ToArray()));
            Assert.IsFalse(labels.Any(l => l.Contains("Fire's Power")), string.Join(" | ", labels.ToArray()));
        }

        [Test]
        public void Nurturing_Evolves_1_Of_Your_Pokemon_From_The_Deck()
        {
            PokemonInPlay leavanny, chansey, blissey;
            var e = Attacking(Leavanny, new[] { "Attack: Nurturing" }, out leavanny, out chansey, out blissey);
            Attach(e, leavanny, Psychic, 1);
            var sewaddle = e.ScenarioBench(0, C(Sewaddle));
            e.ScenarioAddToDeck(0, C(Swadloon), 1);
            Step(e);
            Assert.AreEqual("Swadloon", sewaddle.Name);
        }

        [Test]
        public void Giga_Drain_Heals_The_Damage_It_Did()
        {
            PokemonInPlay virizion, chansey, blissey;
            var e = Attacking(Virizion, new[] { "Attack: Giga Drain" }, out virizion, out chansey, out blissey);
            Attach(e, virizion, Grass, 2);
            virizion.DamageCounters = 5;
            Step(e);
            Assert.AreEqual(3, chansey.DamageCounters);
            Assert.AreEqual(2, virizion.DamageCounters);
        }

        [Test]
        public void Rushing_Water_Moves_An_Energy_To_The_Opponents_Bench()
        {
            PokemonInPlay simipour, chansey, blissey;
            var e = Attacking(Simipour, new[] { "Attack: Rushing Water" }, out simipour, out chansey, out blissey);
            Attach(e, simipour, Water, 3);
            Attach(e, chansey, Psychic, 1);
            Step(e);
            Assert.AreEqual(0, chansey.Energy.Count);
            Assert.AreEqual(1, blissey.Energy.Count);
        }

        [Test]
        public void Final_Gambit_Hurts_Itself_Only_On_Two_Tails()
        {
            foreach (bool twoTails in new[] { true, false })
            {
                PokemonInPlay basculin, chansey, blissey;
                var e = Attacking(Basculin, new[] { "Attack: Final Gambit" }, out basculin, out chansey, out blissey);
                Attach(e, basculin, Water, 3);
                e.ScenarioBench(0, C(Eevee));
                e.State.Rng = new Rng(twoTails ? SeedFor(false, false) : SeedFor(true));
                Step(e);
                Assert.AreEqual(8, chansey.DamageCounters);
                Assert.AreEqual(!twoTails, e.State.Players[0].InPlay().Contains(basculin), twoTails ? "80 to itself Knocks it Out" : "no damage to itself");
            }
        }

        // ------------------------------------------------------------------ Psychic

        [Test]
        public void Phat_Sound_Does_10_Per_Heads_To_Each_Opposing_Pokemon()
        {
            PokemonInPlay swoobat, chansey, blissey;
            var e = Attacking(Swoobat, new[] { "Attack: Phat Sound" }, out swoobat, out chansey, out blissey);
            Attach(e, swoobat, Psychic, 2);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
            Assert.AreEqual(2, blissey.DamageCounters);
        }

        [Test]
        public void Telekinesis_Hits_Only_The_Chosen_Pokemon()
        {
            PokemonInPlay sigilyph, chansey, blissey;
            var e = Attacking(Sigilyph, new[] { "Attack: Telekinesis", "Blissey" }, out sigilyph, out chansey, out blissey);
            Attach(e, sigilyph, Psychic, 3);
            Step(e);
            Assert.AreEqual(5, blissey.DamageCounters);
            Assert.AreEqual(0, chansey.DamageCounters, "your ruling: no hit to the Defending Pokémon");
        }

        // ------------------------------------------------------------------ Fighting and Darkness

        [Test]
        public void Smack_Down_Does_60_More_Against_Fighting_Resistance()
        {
            foreach (bool resists in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Smack Down"), new ScriptedAgent());
                var boldore = e.ScenarioActive(0, C(Boldore));
                Attach(e, boldore, Fighting, 1);
                Attach(e, boldore, Psychic, 1);
                var defender = e.ScenarioActive(1, C(resists ? Swanna : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(resists ? 6 : 4, defender.DamageCounters, resists ? "20 + 60 - 20 Resistance" : "20 x2 Weakness");
            }
        }

        [Test]
        public void Shear_Attaches_The_Fighting_Energy_It_Discards()
        {
            PokemonInPlay gigalith, chansey, blissey;
            var e = Attacking(Gigalith, new[] { "Attack: Shear" }, out gigalith, out chansey, out blissey);
            Attach(e, gigalith, Fighting, 1);
            var deck = e.State.Players[0].Deck;
            deck.Insert(0, e.NewCard(0, C(Fighting)));
            deck.Insert(2, e.NewCard(0, C(Fighting)));
            Step(e);
            Assert.AreEqual(3, gigalith.Energy.Count);
            Assert.AreEqual(3, e.State.Players[0].Discard.Count, "the other 3 cards");
        }

        [Test]
        public void Black_Eyes_Discards_An_Energy_From_The_Opponents_Active_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Black Eyes"), new ScriptedAgent());
                e.ScenarioActive(0, C(Krookodile));
                var chansey = e.ScenarioActive(1, C(Chansey));
                Attach(e, chansey, Psychic, 1);
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 0 : 1, chansey.Energy.Count);
            }
        }

        [Test]
        public void Black_Eyes_Needs_Krookodile_In_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Eevee));
            e.ScenarioBench(0, C(Krookodile));
            var chansey = e.ScenarioActive(1, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Black Eyes")));
        }

        [Test]
        public void Feint_Attack_Hits_Only_The_Chosen_Pokemon()
        {
            PokemonInPlay liepard, chansey, blissey;
            var e = Attacking(Liepard, new[] { "Attack: Feint Attack", "Blissey" }, out liepard, out chansey, out blissey);
            Attach(e, liepard, Darkness, 1);
            Step(e);
            Assert.AreEqual(3, blissey.DamageCounters);
            Assert.AreEqual(0, chansey.DamageCounters);
        }

        // ------------------------------------------------------------------ Metal and Colorless

        [Test]
        public void Power_Whip_Does_10_For_Each_Energy_To_Any_Pokemon()
        {
            PokemonInPlay ferrothorn, chansey, blissey;
            var e = Attacking(Ferrothorn, new[] { "Attack: Power Whip", "Blissey" }, out ferrothorn, out chansey, out blissey);
            Attach(e, ferrothorn, Psychic, 3);
            Step(e);
            Assert.AreEqual(3, blissey.DamageCounters);
            Assert.AreEqual(0, chansey.DamageCounters);
        }

        [Test]
        public void Quick_Tail_Smash_Is_An_Optional_Gamble()
        {
            foreach (string answer in new[] { "Yes", "No" })
            {
                PokemonInPlay watchog, chansey, blissey;
                var e = Attacking(Watchog, new[] { "Attack: Quick Tail Smash", answer }, out watchog, out chansey, out blissey);
                Attach(e, watchog, Psychic, 2);
                e.State.Rng = new Rng(SeedFor(true));
                Step(e);
                Assert.AreEqual(answer == "Yes" ? 8 : 2, chansey.DamageCounters);
            }
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Recycle_Puts_A_Card_On_Top_Of_The_Deck_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Recycle"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Recycle));
            e.ScenarioAddToDiscard(0, C(Hooligans), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(Hooligans, e.State.Players[0].Deck[0].Def.Key);
        }
    }
}
