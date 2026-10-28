using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v137: Nintendo Black Star Promos (NP 1-40): the card data (Unlimited only, the old Pokémon-ex, Delta Species Pikachu, the
    /// prints that aren't new cards), Frenzy and Synchronized Lift with your naming ruling (the Pokémon-ex of that era only),
    /// Rain Dish, Burning Aura, Magnetic Call, Championship Arena and the attacks with effects.
    /// </summary>
    public class NpTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string MajorKyogre = "NP-1";
        const string MajorGroudon = "NP-2";
        const string Meowth = "NP-13";
        const string Ludicolo = "NP-20";
        const string Jirachi = "NP-21";
        const string Beldum = "NP-22";
        const string Chimecho = "NP-24";
        const string Flygon = "NP-25";
        const string ChampionshipArena = "NP-28";
        const string Moltres = "NP-31";
        const string Articuno = "NP-32";
        const string Zapdos = "NP-33";
        const string Typhlosion = "NP-34";
        const string DeltaPikachu = "NP-35";
        const string KyogreEx = "NP-37";
        const string GroudonEx = "NP-38";
        const string RayquazaEx = "NP-39";
        const string Mew = "NP-40";
        const string SvZapdosEx = "SVP-49";
        const string GroudonEX = "DEX-54";
        const string Kyogre = "CEL-3";
        const string Hitmonlee = "HSP-25";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string FightingFuryBelt = "BKP-99";

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
        public void The_Promos_Are_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "NP").ToList();
            Assert.AreEqual(32, set.Count, "40 cards less the 8 prints of other cards");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly), "every card came out before Black & White");
            Assert.IsFalse(set.Any(d => d.Number == "7" || d.Number == "26" || d.Number == "29"), "prints of Treecko 3, DPP Tropical Wind, WP Celebi");
            Assert.IsTrue(C(MajorKyogre).IsOldEx);
            Assert.AreEqual(120, C(MajorKyogre).Hp);
            Assert.AreEqual("Pikachu", C(DeltaPikachu).Name, "the δ isn't part of the name");
            Assert.AreEqual(EnergyType.Metal, C(DeltaPikachu).PokemonType);
            Assert.AreEqual(AbilityDef.PokePower, C(Beldum).Abilities[0].Kind);
            Assert.AreEqual(AbilityDef.PokeBody, C(KyogreEx).Abilities[0].Kind);
            Assert.IsTrue(C(ChampionshipArena).IsStadium);
        }

        [Test]
        public void An_Old_Pokemon_ex_Gives_2_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kyogre = e.ScenarioActive(0, C(MajorKyogre));
            e.ScenarioActive(1, C(Blissey));
            Assert.AreEqual(2, e.PrizesFor(kyogre));
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Major_Flood_Discards_A_Water_Energy_And_Hits_Both_Benches()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Major Flood"), new ScriptedAgent());
            var kyogre = e.ScenarioActive(0, C(MajorKyogre));
            Attach(e, kyogre, Water, 4);
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            var blissey = e.ScenarioActive(1, C(Blissey));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, blissey.DamageCounters);
            Assert.AreEqual(1, lee.DamageCounters, "your Bench too");
            Assert.AreEqual(1, chansey.DamageCounters);
            Assert.AreEqual(3, kyogre.Energy.Count);
        }

        [Test]
        public void Major_Earthquake_Hits_Only_Your_Own_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Major Earthquake"), new ScriptedAgent());
            var groudon = e.ScenarioActive(0, C(MajorGroudon));
            Attach(e, groudon, Fighting, 4);
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            var target = e.ScenarioActive(1, C(MajorGroudon));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, target.DamageCounters);
            Assert.AreEqual(1, lee.DamageCounters);
            Assert.AreEqual(0, chansey.DamageCounters, "not the opponent's Bench");
            Assert.AreEqual(3, groudon.Energy.Count);
        }

        [Test]
        public void Plunder_Discards_The_Defending_Pokemons_Tools_First()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Plunder"), new ScriptedAgent());
            var meowth = e.ScenarioActive(0, C(Meowth));
            Attach(e, meowth, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioAttachTool(blissey, C(FightingFuryBelt));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, blissey.Tools.Count);
            Assert.AreEqual(1, blissey.DamageCounters);
        }

        [Test]
        public void The_Third_Eye_Discards_An_Energy_And_Draws_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: The Third Eye"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C(Jirachi));
            Attach(e, jirachi, Psychic, 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, jirachi.Energy.Count);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Psywave_Does_10_For_Each_Energy_On_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psywave"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C(Jirachi));
            Attach(e, jirachi, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            Attach(e, blissey, Psychic, 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, blissey.DamageCounters);
        }

        [Test]
        public void Resonate_Makes_The_Opponent_Switch()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Resonate"), new ScriptedAgent());
            var chimecho = e.ScenarioActive(0, C(Chimecho));
            Attach(e, chimecho, Psychic, 1);
            e.ScenarioActive(1, C(Blissey));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(chansey, e.State.Players[1].Active);
        }

        [Test]
        public void Healing_Sound_Heals_30_And_Puts_The_Defending_Pokemon_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Healing Sound"), new ScriptedAgent());
            var chimecho = e.ScenarioActive(0, C(Chimecho));
            Attach(e, chimecho, Psychic, 2);
            chimecho.DamageCounters = 5;
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, chimecho.DamageCounters);
            Assert.IsTrue(blissey.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Dragonbreath_Does_Nothing_On_Tails()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Dragonbreath"), new ScriptedAgent());
                var flygon = e.ScenarioActive(0, C(Flygon));
                Attach(e, flygon, Grass, 1);
                Attach(e, flygon, Lightning, 1);
                Attach(e, flygon, Psychic, 2);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 8 : 0, blissey.DamageCounters);
                Assert.AreEqual(heads, blissey.Has(SpecialCondition.Paralyzed));
            }
        }

        [Test]
        public void Twister_Discards_An_Energy_For_Each_Heads_And_Does_Nothing_On_2_Tails()
        {
            foreach (bool oneHeads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Twister"), new ScriptedAgent());
                var rayquaza = e.ScenarioActive(0, C(RayquazaEx));
                Attach(e, rayquaza, Fire, 1);
                Attach(e, rayquaza, Lightning, 1);
                Attach(e, rayquaza, Psychic, 1);
                var blissey = e.ScenarioActive(1, C(Blissey));
                Attach(e, blissey, Psychic, 2);
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(oneHeads, false));
                Step(e);
                Assert.AreEqual(oneHeads ? 5 : 0, blissey.DamageCounters);
                Assert.AreEqual(oneHeads ? 1 : 2, blissey.Energy.Count);
            }
        }

        [Test]
        public void Mix_Up_Discards_The_Top_Card_Of_The_Opponents_Deck_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mix-Up"), new ScriptedAgent());
            var groudon = e.ScenarioActive(0, C(GroudonEx));
            Attach(e, groudon, Fighting, 1);
            Attach(e, groudon, Psychic, 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);
            Assert.AreEqual(1, e.State.Players[1].Discard.Count);
        }

        [Test]
        public void Devolution_Beam_Devolves_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Devolution Beam"), new ScriptedAgent());
                var mew = e.ScenarioActive(0, C(Mew));
                Attach(e, mew, Psychic, 1);
                var mon = e.ScenarioActive(1, C(Chansey));
                e.ScenarioEvolve(mon, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? "Chansey" : "Blissey", e.State.Players[1].Active.Name);
                Assert.AreEqual(heads ? 1 : 0, e.State.Players[1].Hand.Count(c => c.Def.Name == "Blissey"));
            }
        }

        [Test]
        public void Flame_Gift_May_Move_A_Fire_Energy_To_Another_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flame Gift", "Yes", "Hitmonlee"), new ScriptedAgent());
            var moltres = e.ScenarioActive(0, C(Moltres));
            Attach(e, moltres, Fire, 1);
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters);
            Assert.AreEqual(0, moltres.Energy.Count);
            Assert.AreEqual(1, lee.Energy.Count);
        }

        // ------------------------------------------------------------------ Poké-Bodies

        [Test]
        public void Frenzy_Adds_40_Only_Against_The_Old_Pokemon_ex_And_The_Exact_Names()
        {
            foreach (string rival in new[] { GroudonEx, GroudonEX, null })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Aqua Supply"), new ScriptedAgent());
                var kyogre = e.ScenarioActive(0, C(KyogreEx));
                Attach(e, kyogre, Water, 2);
                var blissey = e.ScenarioActive(1, C(Blissey));
                if (rival != null) e.ScenarioBench(1, C(rival));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(rival == GroudonEx ? 7 : 3, blissey.DamageCounters, rival ?? "no rival");
            }
        }

        [Test]
        public void Rayquaza_exs_Frenzy_Is_40_More_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragon Bind"), new ScriptedAgent());
            var rayquaza = e.ScenarioActive(0, C(RayquazaEx));
            Attach(e, rayquaza, Psychic, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioBench(1, C(Kyogre));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, blissey.DamageCounters, "20 + 40 (your ruling)");
        }

        [Test]
        public void Synchronized_Lift_Needs_The_Other_Two_Old_Birds()
        {
            foreach (bool oldZapdos in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var moltres = e.ScenarioActive(0, C(Moltres));
                e.ScenarioBench(0, C(Articuno));
                e.ScenarioBench(0, C(oldZapdos ? Zapdos : SvZapdosEx));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(oldZapdos ? 0 : 2, e.RetreatCostOf(moltres), oldZapdos ? "Articuno ex and Zapdos ex" : "Scarlet & Violet's Zapdos ex doesn't count");
            }
        }

        [Test]
        public void Burning_Aura_Puts_A_Counter_On_Each_Active_From_The_Active_Spot()
        {
            foreach (bool active in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                PokemonInPlay mine;
                if (active) mine = e.ScenarioActive(0, C(Typhlosion));
                else
                {
                    mine = e.ScenarioActive(0, C(Hitmonlee));
                    e.ScenarioBench(0, C(Typhlosion));
                }
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.EndTurn();
                Assert.AreEqual(active ? 1 : 0, mine.DamageCounters);
                Assert.AreEqual(active ? 1 : 0, blissey.DamageCounters);
            }
        }

        [Test]
        public void Rain_Dish_Removes_A_Damage_Counter_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ludicolo = e.ScenarioActive(0, C(Ludicolo));
            ludicolo.DamageCounters = 3;
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(2, ludicolo.DamageCounters);
        }

        // ------------------------------------------------------------------ Poké-Power

        [Test]
        public void Magnetic_Call_Benches_A_Metal_Basic_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Magnetic Call", "Beldum"), new ScriptedAgent());
                e.ScenarioActive(0, C(Beldum));
                e.ScenarioAddToDeck(0, C(Beldum), 1);
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 1 : 0, e.State.Players[0].Bench.Count);
            }
            var asleep = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var beldum = asleep.ScenarioActive(0, C(Beldum));
            asleep.ScenarioAddToDeck(0, C(Beldum), 1);
            asleep.ScenarioActive(1, C(Blissey));
            asleep.ScenarioSetTurn(0, 3);
            asleep.ApplyCondition(beldum, SpecialCondition.Asleep);
            Assert.IsFalse(Offered(asleep, 0, "Magnetic Call"), "not with a Special Condition");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Championship_Arena_Makes_Each_Player_Discard_Down_To_7_At_The_End_Of_Their_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetStadium(1, C(ChampionshipArena));
            for (int i = 0; i < 9; i++) e.ScenarioAddToHand(0, C(Psychic));
            for (int i = 0; i < 9; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(7, e.State.Players[0].Hand.Count, "the player whose turn ended");
            Assert.AreEqual(9, e.State.Players[1].Hand.Count, "not the other player yet");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count);
        }
    }
}
