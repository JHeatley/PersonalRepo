using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v141: Neo Revelation (N3): the card data, the new engine hooks (Energy attached by any means for timed effects, a Pokémon
    /// leaving the Active Spot at any time, Balloon Berry paying the Retreat Cost, Allergic Pollen's discard shield, Trainer cards
    /// stopped whoever plays them) and the Pokémon Powers, attacks and Trainers, with your rulings.
    /// </summary>
    public class N3Tests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";

        const string Celebi = "N3-3";
        const string Crobat = "N3-4";
        const string Delibird = "N3-5";
        const string HowlEntei = "N3-6";
        const string Houndoom = "N3-8";
        const string Magneton = "N3-10";
        const string Misdreavus = "N3-11";
        const string Porygon2 = "N3-12";
        const string BurstRaikou = "N3-13";
        const string CrystalSuicune = "N3-14";
        const string Aerodactyl = "N3-15";
        const string LegendaryEntei = "N3-17";
        const string Kingdra = "N3-19";
        const string Raichu = "N3-21";
        const string Sudowoodo = "N3-26";
        const string Lanturn = "N3-32";
        const string Magcargo = "N3-33";
        const string Parasect = "N3-35";
        const string UnownB = "N3-39";
        const string UnownY = "N3-40";
        const string Shuckle = "N3-51";
        const string Smoochum = "N3-54";
        const string JynxN3 = "N3-31";
        const string BalloonBerry = "N3-60";
        const string HealingField = "N3-61";
        const string BreederFields = "N3-62";
        const string RocketsHideout = "N3-63";
        const string OldRod = "N3-64";
        const string ShiningGyarados = "N3-65";
        const string Magnemite = "N2-7";
        const string Poliwag = "N2-62";
        const string Poliwhirl = "N2-44";
        const string EnergyArk = "N2-75";
        const string Potion = "BS-94";
        const string GustOfWind = "BS-93";
        const string DarkGyarados = "CEL-CC5";
        const string Darkrai = "BUS-87";
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
        public void Neo_Revelation_Is_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "N3").ToList();
            Assert.AreEqual(65, set.Count, "66 cards less Shining Magikarp (Celebrations' Classic Collection card)");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.IsFalse(set.Any(d => d.Number == "66"));
            Assert.IsTrue(C(Smoochum).IsBaby);
            Assert.IsTrue(C(Smoochum).BabyEvolvesInto(C(JynxN3)));
            Assert.IsTrue(C(UnownB).UnownRule && C(UnownY).UnownRule);
            Assert.AreEqual("Ho-Oh", C("N3-7").Name, "named like the other Ho-Oh in the pool");
            Assert.IsTrue(C(BalloonBerry).IsTool);
            Assert.IsTrue(C(HealingField).IsStadium && C(RocketsHideout).IsStadium);
            var deck = Enumerable.Repeat(C(ShiningGyarados), 2).ToList();
            while (deck.Count < 60) deck.Add(C(Psychic));
            Assert.IsTrue(Decklist.Validate(deck, null).Any(p => p.Contains("Shining Gyarados")), "1 Shining Gyarados per deck");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Rockets_Hideout_Gives_Only_Dark_Pokemon_20_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dark = e.ScenarioActive(0, C(DarkGyarados));
            var darkrai = e.ScenarioActive(1, C(Darkrai));
            e.ScenarioSetStadium(0, C(RocketsHideout));
            Assert.AreEqual(90, e.MaxHp(dark));
            Assert.AreEqual(120, e.MaxHp(darkrai), "your ruling: Darkrai isn't a Dark Pokémon");
        }

        [Test]
        public void Balloon_Berry_Pays_The_Retreat_Cost_And_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAttachTool(lee, C(BalloonBerry));
            var chansey = e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(lee), "Compendium: its Retreat Cost is 0 while Balloon Berry is attached");
            Assert.IsTrue(Offered(e, 0, "Retreat"), "no Energy needed");
            Step(e);
            Assert.AreSame(chansey, e.State.Players[0].Active);
            Assert.AreEqual(0, lee.Tools.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Balloon Berry"));
        }

        [Test]
        public void Healing_Field_Heals_The_Active_Pokemon_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Healing Field"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            lee.DamageCounters = 3;
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetStadium(1, C(HealingField));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(1, lee.DamageCounters);
            Assert.IsFalse(Offered(e, 0, "Use Healing Field"), "once a turn");
        }

        [Test]
        public void Pokemon_Breeder_Fields_Finds_A_Card_That_Evolves_From_The_Chosen_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Breeder Fields"), new ScriptedAgent());
            e.ScenarioActive(0, C(Poliwag));
            e.ScenarioAddToHand(0, C(BreederFields));
            e.ScenarioAddToDeck(0, C(Poliwhirl), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Poliwhirl", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Old_Rod_Takes_A_Trainer_On_Two_Tails_And_A_Pokemon_On_Two_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Old Rod"), new ScriptedAgent());
                e.ScenarioActive(0, C(Hitmonlee));
                e.ScenarioAddToHand(0, C(OldRod));
                e.ScenarioAddToDiscard(0, C(Chansey), 1);
                e.ScenarioAddToDiscard(0, C(EnergyArk), 1);
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads, heads));
                Step(e);
                Assert.AreEqual(heads ? "Chansey" : "Energy Ark", e.State.Players[0].Hand.Single().Def.Name);
            }
        }

        // ------------------------------------------------------------------ Legendary Body

        [Test]
        public void Legendary_Body_Stops_Your_Own_Potion_And_The_Opponents_Gust_Of_Wind()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Potion"), new ScriptedAgent().Then("Play Gust of Wind"));
            var entei = e.ScenarioActive(0, C(LegendaryEntei));
            entei.DamageCounters = 2;
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Potion));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(GustOfWind));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, entei.DamageCounters, "your ruling: your own Trainers too");
            NextTurn(e);
            Step(e);
            Assert.AreSame(entei, e.State.Players[0].Active, "the opponent's Gust of Wind can't switch it out");
        }

        [Test]
        public void Legendary_Body_Discards_Tools_On_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var entei = e.ScenarioActive(0, C(LegendaryEntei));
            e.ScenarioAttachTool(entei, C(BalloonBerry));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.StateChecks();
            Assert.AreEqual(0, entei.Tools.Count);
        }

        // ------------------------------------------------------------------ Pokémon Powers

        [Test]
        public void Time_Travel_Shuffles_Celebi_Into_The_Deck_On_Heads_Unless_It_Was_Already_Asleep()
        {
            foreach (bool asleep in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
                var lee = e.ScenarioActive(0, C(Hitmonlee));
                Attach(e, lee, Fighting, 1);
                var celebi = e.ScenarioActive(1, C(Celebi));
                celebi.DamageCounters = 3;
                e.ScenarioBench(1, C(Chansey));
                if (asleep) e.ApplyCondition(celebi, SpecialCondition.Asleep);
                e.ScenarioSetTurn(0, 3);
                int deck = e.State.Players[1].Deck.Count;
                e.State.Rng = new Rng(SeedFor(true));
                Step(e);
                Assert.IsNull(e.State.FindPokemon(celebi.Uid));
                Assert.AreEqual(asleep ? deck : deck + 1, e.State.Players[1].Deck.Count, asleep ? "Knocked Out" : "shuffled into the deck");
                Assert.AreEqual(asleep ? 5 : 6, e.State.Players[0].Prizes.Count, asleep ? "a Prize card" : "no Prize card");
            }
        }

        [Test]
        public void Triggered_Poison_Poisons_When_The_Opponent_Attaches_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Triggered Poison"), new ScriptedAgent().Then("Attach Psychic Energy"));
            var crobat = e.ScenarioActive(0, C(Crobat));
            Attach(e, crobat, Grass, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(blissey.Has(SpecialCondition.Poisoned));
            NextTurn(e);
            Step(e);
            Assert.IsTrue(blissey.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Magma_Pool_Moves_A_Fire_Energy_To_The_New_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var magcargo = e.ScenarioActive(0, C(Magcargo));
            Attach(e, magcargo, Fire, 4);
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(lee, e.State.Players[0].Active);
            Assert.AreEqual(1, lee.Energy.Count, "1 Fire Energy card follows");
            Assert.AreEqual(0, magcargo.Energy.Count, "3 paid the Retreat Cost");
        }

        [Test]
        public void Allergic_Pollen_Keeps_Cards_In_The_Discard_Pile_Out_Of_Attacks()
        {
            foreach (bool parasect in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Dark Flame"), new ScriptedAgent());
                var houndoom = e.ScenarioActive(0, C(Houndoom));
                Attach(e, houndoom, Fire, 1);
                e.ScenarioAddToDiscard(0, C(Darkness), 1);
                e.ScenarioActive(1, C(Blissey));
                if (parasect) e.ScenarioBench(1, C(Parasect));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(parasect ? 0 : 1, houndoom.Energy.Count(c => c.Def.Name == "Darkness Energy"));
                Assert.AreEqual(parasect ? 1 : 0, e.State.Players[0].Discard.Count(c => c.Def.Name == "Darkness Energy"));
            }
        }

        [Test]
        public void Mimic_Gives_Sudowoodo_The_Defending_Pokemons_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sudowoodo = e.ScenarioActive(0, C(Sudowoodo));
            Attach(e, sudowoodo, Fighting, 1);
            e.ScenarioActive(1, C(Hitmonlee));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Kick"));
            Assert.IsFalse(Offered(e, 0, "Attack: High Jump Kick"), "it still pays the cost");
        }

        [Test]
        public void Prehistoric_Memory_Lets_Evolved_Pokemon_Use_Their_Lower_Stages_Attacks()
        {
            foreach (bool aerodactyl in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var poliwhirl = e.ScenarioActive(0, C(Poliwag));
                e.ScenarioEvolve(poliwhirl, C(Poliwhirl));
                Attach(e, poliwhirl, Water, 1);
                if (aerodactyl) e.ScenarioBench(1, C(Aerodactyl));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(aerodactyl, Offered(e, 0, "Attack: Rollout"), "Poliwag's attack, even with the opponent's Aerodactyl");
            }
        }

        [Test]
        public void Genetic_Memory_Uses_A_Lower_Stage_Attack_For_Free()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Genetic Memory", "Rollout"), new ScriptedAgent());
            var kingdra = e.ScenarioActive(0, C(Poliwag));
            e.ScenarioEvolve(kingdra, C(Poliwhirl));
            e.ScenarioEvolve(kingdra, C(Kingdra));
            Attach(e, kingdra, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters, "Poliwag's Rollout");
        }

        [Test]
        public void Energy_Converter_Makes_A_Basic_Energy_Another_Type_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energy Converter", "Psychic Energy", "Lightning"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(Raichu));
            Attach(e, raichu, Psychic, 2);
            e.ScenarioBench(0, C(Porygon2));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Thundershock"));
            Step(e);
            Assert.IsTrue(Offered(e, 0, "Attack: Thundershock"), "a Psychic Energy is Lightning now");
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 0, "Attack: Thundershock"), "only that turn");
        }

        [Test]
        public void Submerge_Makes_Lanturn_A_Water_Pokemon_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Submerge"), new ScriptedAgent());
            var lanturn = e.ScenarioActive(0, C(Lanturn));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEqual(new[] { EnergyType.Water }, e.TypesOf(lanturn));
            e.ApplyCondition(lanturn, SpecialCondition.Confused);
            e.StateChecks();
            CollectionAssert.AreEqual(new[] { EnergyType.Lightning }, e.TypesOf(lanturn), "back to Lightning");
        }

        [Test]
        public void Lightning_Burst_Makes_The_Opponent_Switch_When_You_Attach_Lightning_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Lightning Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(BurstRaikou));
            e.ScenarioAddToHand(0, C(Lightning));
            e.ScenarioActive(1, C(Blissey));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(chansey, e.State.Players[1].Active);
        }

        [Test]
        public void Crystal_Body_Stops_Attack_Effects_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Night Eyes"), new ScriptedAgent());
            var misdreavus = e.ScenarioActive(0, C(Misdreavus));
            Attach(e, misdreavus, Psychic, 1);
            var suicune = e.ScenarioActive(1, C(CrystalSuicune));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(suicune.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Perish_Song_Knocks_Out_A_Pokemon_Still_Asleep_From_Night_Eyes()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Night Eyes", "Attack: Perish Song"), new ScriptedAgent());
            var misdreavus = e.ScenarioActive(0, C(Misdreavus));
            Attach(e, misdreavus, Psychic, 3);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.State.Rng = new Rng(SeedFor(false, false));
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(blissey.Has(SpecialCondition.Asleep));
            Step(e);
            Assert.IsNull(e.State.FindPokemon(blissey.Uid));
        }

        [Test]
        public void Hard_Shell_Turns_Small_Damage_Into_10()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var shuckle = e.ScenarioActive(1, C(Shuckle));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, shuckle.DamageCounters);
        }

        [Test]
        public void Present_Removes_Every_Counter_On_Three_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Present"), new ScriptedAgent());
            var delibird = e.ScenarioActive(0, C(Delibird));
            Attach(e, delibird, Psychic, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            blissey.DamageCounters = 7;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false, false, false));
            Step(e);
            Assert.AreEqual(0, blissey.DamageCounters);
        }

        [Test]
        public void Electromagnetic_Power_Moves_Energy_Between_Magnet_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Electromagnetic Power"), new ScriptedAgent());
            var magneton = e.ScenarioActive(0, C(Magneton));
            var magnemite = e.ScenarioBench(0, C(Magnemite));
            Attach(e, magnemite, Lightning, 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, magneton.Energy.Count);
            Assert.AreEqual(0, magnemite.Energy.Count);
        }

        [Test]
        public void Bear_Moves_A_Damage_Counter_From_Another_Unown()
        {
            var e = Scenario(new ScriptedAgent().Then("Use [Bear]"), new ScriptedAgent());
            var bear = e.ScenarioActive(0, C(UnownB));
            var yield = e.ScenarioBench(0, C(UnownY));
            yield.DamageCounters = 2;
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, bear.DamageCounters);
            Assert.AreEqual(1, yield.DamageCounters);
        }

        [Test]
        public void Howl_Attaches_The_Fire_Energy_From_The_Top_5_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Entei", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(HowlEntei));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C(Fire), 2);
            e.ScenarioAddToDeck(0, C(Psychic), 5);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            bool over = e.MainPhaseStep();
            var entei = e.State.Players[0].Bench.Single();
            Assert.AreEqual(2, entei.Energy.Count, "both Fire Energy cards to the only Fire Pokémon");
            Assert.AreEqual(3, e.State.Players[0].Discard.Count);
            Assert.IsTrue(over, "the turn ends");
        }
    }
}
