using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v138: Neo Genesis (N1): the card data (Baby Pokémon, the Special Darkness and Metal Energy beside the basic Energy of the
    /// same name, the prints already in the pool), the new rules (no more Trainer cards this turn, Mind Games, Jaw Clamp, Char
    /// counters, Ecogym, the Berries, Pollen Shield) and the Pokémon Powers and attacks, with your rulings.
    /// </summary>
    public class N1Tests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string BasicDarkness = "MEE-15";

        const string Lugia = "N1-9";
        const string HerbalMeganium = "N1-10";
        const string WildMeganium = "N1-11";
        const string Pichu = "N1-12";
        const string Skarmory = "N1-13";
        const string Slowking = "N1-14";
        const string Togetic = "N1-16";
        const string DownpourFeraligatr = "N1-5";
        const string MetalEnergy = "N1-19";
        const string Elekid = "N1-22";
        const string Magby = "N1-23";
        const string Murkrow = "N1-24";
        const string Ariados = "N1-27";
        const string ShieldBayleef = "N1-28";
        const string ScentBayleef = "N1-29";
        const string ClampCroconaw = "N1-31";
        const string Granbull = "N1-37";
        const string CharQuilava = "N1-47";
        const string Chikorita = "N1-53";
        const string Horsea = "N1-62";
        const string Ledyba = "N1-63";
        const string Natu = "N1-67";
        const string NeoPikachu = "N1-70";
        const string Totodile = "N1-81";
        const string Ecogym = "N1-84";
        const string FocusBand = "N1-86";
        const string BillsTeleporter = "N1-91";
        const string CardFlipGame = "N1-92";
        const string GoldBerry = "N1-93";
        const string MiracleBerry = "N1-94";
        const string ProfessorElm = "N1-96";
        const string SproutTower = "N1-97";
        const string Berry = "N1-99";
        const string DoubleGust = "N1-100";
        const string DarknessEnergy = "N1-104";
        const string WhirlpoolMudkip = "NP-18";
        const string Jacq = "SVI-175";
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
        public void Neo_Genesis_Is_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "N1").ToList();
            Assert.AreEqual(100, set.Count, "111 cards less Cleffa, Sneasel, Super Scoop Up, Super Rod, Recycle Energy and the 6 basic Energy");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.IsFalse(set.Any(d => d.Number == "20" || d.Number == "25" || d.Number == "98" || d.Number == "103" || d.Number == "105"));
            Assert.IsTrue(C(Pichu).IsBaby);
            Assert.IsTrue(C(Pichu).BabyEvolvesInto(C(NeoPikachu)), "Pichu evolves into Pikachu");
            Assert.AreEqual(AbilityDef.PokemonPower, C(Slowking).Abilities[0].Kind);
            Assert.AreEqual(10, C(Ledyba).Attacks[1].Damage, "Comet Punch is 10x (Limitless)");
            Assert.AreEqual(20, C(Natu).Attacks[1].Damage, "Telekinesis does 20");
            var dark = C(DarknessEnergy);
            Assert.IsTrue(dark.IsEnergy && !dark.IsBasicEnergy);
            Assert.AreEqual(EnergyType.Darkness, dark.EnergyProvides);
            Assert.IsTrue(db.GetByName("Darkness Energy").IsBasicEnergy, "a bare name in a deck list is the basic Energy");
            Assert.IsTrue(C(FocusBand).IsTool);
            Assert.IsTrue(C(SproutTower).IsStadium);
        }

        [Test]
        public void Special_Darkness_Energy_Has_Its_Own_Copy_Limit_Apart_From_The_Basic()
        {
            var deck = new List<CardDef>();
            for (int i = 0; i < 4; i++) deck.Add(C(DarknessEnergy));
            for (int i = 0; i < 12; i++) deck.Add(C(BasicDarkness));
            while (deck.Count < 60) deck.Add(C(Psychic));
            Assert.IsFalse(Decklist.Validate(deck, null).Any(p => p.Contains("Darkness")), "4 Special and 12 basic");
            deck[59] = C(DarknessEnergy);
            Assert.IsTrue(Decklist.Validate(deck, null).Any(p => p.Contains("Darkness")), "a 5th Special Darkness Energy");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Professor_Elm_Draws_7_And_No_More_Trainers_Can_Be_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Professor Elm"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(0, C(ProfessorElm));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
            e.ScenarioAddToHand(0, C(BillsTeleporter));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.PlayTrainer), "no more Trainer cards this turn");
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(Offered(e, 0, "Play Bill's Teleporter"), "next turn it's over");
        }

        [Test]
        public void Card_Flip_Game_Turns_A_Prize_Face_Up_And_Draws_2_On_A_Right_Guess()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Card-Flip Game", "Face-down Prize card 1", "Energy card"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(0, C(CardFlipGame));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].FaceUpPrizeIds.Count, "it stays face up");
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "the Prize cards are Fire Energy: right guess");
        }

        [Test]
        public void Ecogym_Returns_Basic_Energy_The_Opponents_Attack_Discards()
        {
            foreach (bool gym in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Whirlpool"), new ScriptedAgent());
                var mudkip = e.ScenarioActive(0, C(WhirlpoolMudkip));
                Attach(e, mudkip, Water, 2);
                var blissey = e.ScenarioActive(1, C(Blissey));
                Attach(e, blissey, Psychic, 1);
                if (gym) e.ScenarioSetStadium(1, C(Ecogym));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(true));
                Step(e);
                Assert.AreEqual(0, blissey.Energy.Count);
                Assert.AreEqual(gym ? 1 : 0, e.State.Players[1].Hand.Count(c => c.Def.IsEnergy), gym ? "back to the hand" : "discarded");
            }
        }

        [Test]
        public void Berry_Heals_At_The_Start_Of_A_Turn_And_Gold_Berry_Needs_4_Counters()
        {
            foreach (var tool in new[] { Berry, GoldBerry })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var lee = e.ScenarioActive(0, C(Hitmonlee));
                lee.DamageCounters = 3;
                e.ScenarioAttachTool(lee, C(tool));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(1, 4);
                e.StartTurn();
                Assert.AreEqual(tool == Berry ? 1 : 3, lee.DamageCounters, tool);
                Assert.AreEqual(tool == Berry ? 0 : 1, lee.Tools.Count);
            }
        }

        [Test]
        public void Miracle_Berry_Cures_At_The_Start_Of_A_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAttachTool(lee, C(MiracleBerry));
            e.ScenarioActive(1, C(Blissey));
            e.ApplyCondition(lee, SpecialCondition.Asleep);
            e.ApplyCondition(lee, SpecialCondition.Poisoned);
            e.ScenarioSetTurn(1, 4);
            e.StartTurn();
            Assert.AreEqual(SpecialCondition.None, lee.Conditions);
            Assert.AreEqual(0, lee.Tools.Count);
        }

        [Test]
        public void Focus_Band_Keeps_The_Pokemon_At_10_HP_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var target = e.ScenarioActive(1, C(Hitmonlee));
            target.DamageCounters = 7;
            e.ScenarioAttachTool(target, C(FocusBand));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreSame(target, e.State.Players[1].Active);
            Assert.AreEqual(10, e.RemainingHp(target));
            Assert.AreEqual(0, target.Tools.Count, "then discard Focus Band");
        }

        [Test]
        public void Sprout_Tower_Takes_30_Off_Colorless_Attacks()
        {
            foreach (bool tower in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
                var granbull = e.ScenarioActive(0, C(Granbull));
                Attach(e, granbull, Psychic, 2);
                var blissey = e.ScenarioActive(1, C(Blissey));
                if (tower) e.ScenarioSetStadium(1, C(SproutTower));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(tower ? 0 : 2, blissey.DamageCounters);
            }
        }

        [Test]
        public void Double_Gust_Cant_Switch_Out_A_Jaw_Clamped_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jaw Clamp", "Chansey"), new ScriptedAgent().Then("Play Double Gust", "Totodile"));
            var croconaw = e.ScenarioActive(0, C(ClampCroconaw));
            Attach(e, croconaw, Water, 3);
            var totodile = e.ScenarioBench(0, C(Totodile));
            var blissey = e.ScenarioActive(1, C(Blissey));
            Attach(e, blissey, Psychic, 3);
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Retreat"), "Jaw Clamp: can't retreat");
            e.ScenarioAddToHand(1, C(DoubleGust));
            Step(e);
            Assert.AreSame(blissey, e.State.Players[1].Active, "its Active Pokémon can't be changed");
            Assert.AreSame(totodile, e.State.Players[0].Active, "the other half of Double Gust still happens");
        }

        // ------------------------------------------------------------------ Special Energy

        [Test]
        public void Darkness_Energy_Adds_10_Only_For_A_Darkness_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Feint Attack"), new ScriptedAgent());
            var murkrow = e.ScenarioActive(0, C(Murkrow));
            e.ScenarioAttach(murkrow, C(DarknessEnergy));
            e.ScenarioAttach(murkrow, C(BasicDarkness));
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, blissey.DamageCounters, "20 + 10");

            var other = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = other.ScenarioActive(0, C(Hitmonlee));
            other.ScenarioAttach(lee, C(Fighting));
            other.ScenarioAttach(lee, C(DarknessEnergy));
            var chansey = other.ScenarioActive(1, C(Blissey));
            other.ScenarioSetTurn(0, 3);
            Step(other);
            Assert.AreEqual(4, chansey.DamageCounters, "not a Darkness Pokémon: no bonus (20 x 2 for Blissey's Fighting Weakness)");
            Assert.AreEqual(0, lee.DamageCounters);
        }

        [Test]
        public void Metal_Energy_Takes_10_Off_Attacks_On_A_Metal_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var skarmory = e.ScenarioActive(1, C(Skarmory));
            e.ScenarioAttach(skarmory, C(MetalEnergy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, skarmory.DamageCounters, "20 - 10");
        }

        // ------------------------------------------------------------------ Pokémon Powers

        [Test]
        public void Mind_Games_Stops_A_Trainer_Only_From_The_Active_Spot()
        {
            var me = new ScriptedAgent().Then("Yes");
            var e = Scenario(me, new ScriptedAgent().Then("Play Bill's Teleporter"));
            e.ScenarioActive(0, C(Slowking));
            e.ScenarioActive(1, C(Blissey));
            var card = e.ScenarioAddToHand(1, C(BillsTeleporter));
            e.ScenarioSetTurn(1, 4);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreSame(card, e.State.Players[1].Deck[0], "on top of the opponent's deck");
            Assert.AreEqual(0, e.State.Players[1].Hand.Count, "it did nothing");

            var benchMe = new ScriptedAgent();
            var bench = Scenario(benchMe, new ScriptedAgent().Then("Play Bill's Teleporter"));
            bench.ScenarioActive(0, C(Hitmonlee));
            bench.ScenarioBench(0, C(Slowking));
            bench.ScenarioActive(1, C(Blissey));
            bench.ScenarioAddToHand(1, C(BillsTeleporter));
            bench.ScenarioSetTurn(1, 4);
            Step(bench);
            Assert.IsFalse(benchMe.Seen.Any(r => r.Kind == DecisionKind.YesNo), "only from the Active Spot (your ruling)");
        }

        [Test]
        public void A_Supporter_Stopped_By_Mind_Games_Isnt_The_Turns_Supporter()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes"), new ScriptedAgent().Then("Play Jacq"));
            e.ScenarioActive(0, C(Slowking));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(Jacq));
            e.ScenarioSetTurn(1, 4);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsFalse(e.State.Players[1].SupporterPlayedThisTurn, "your ruling");
            Assert.AreEqual("Jacq", e.State.Players[1].Deck[0].Def.Name);
        }

        [Test]
        public void Sputter_Stops_Pokemon_Powers_Until_The_End_Of_Your_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sputter"), new ScriptedAgent());
            var magby = e.ScenarioActive(0, C(Magby));
            Attach(e, magby, Fire, 1);
            e.ScenarioActive(1, C(DownpourFeraligatr));
            e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Use Downpour"), "stopped during the opponent's turn");
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(Offered(e, 1, "Use Downpour"), "over after Magby's next turn");
        }

        [Test]
        public void Zzzap_Hits_Only_Pokemon_With_A_Pokemon_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Zzzap"), new ScriptedAgent());
            var pichu = e.ScenarioActive(0, C(Pichu));
            Attach(e, pichu, Lightning, 1);
            var mine = e.ScenarioBench(0, C(DownpourFeraligatr));
            var slowking = e.ScenarioActive(1, C(Slowking));
            var lee = e.ScenarioBench(1, C(Hitmonlee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, slowking.DamageCounters);
            Assert.AreEqual(2, mine.DamageCounters, "yours too");
            Assert.AreEqual(0, lee.DamageCounters);
        }

        [Test]
        public void Playful_Punch_Does_20_On_Heads_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Playful Punch"), new ScriptedAgent());
            e.ScenarioActive(0, C(Elekid));
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            bool over = e.MainPhaseStep();
            Assert.AreEqual(2, lee.DamageCounters);
            Assert.IsTrue(over, "either way, the turn ends");
        }

        [Test]
        public void Wild_Growth_Doubles_Grass_Energy_On_Grass_Pokemon()
        {
            foreach (bool meganium in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var bayleef = e.ScenarioActive(0, C(ScentBayleef));
                Attach(e, bayleef, Grass, 2);
                if (meganium) e.ScenarioBench(0, C(WildMeganium));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(meganium, Offered(e, 0, "Attack: Double Razor Leaf"), "GGG with 2 Grass Energy cards");
            }
        }

        [Test]
        public void Herbal_Scent_Heals_Every_Grass_Pokemon_In_Play_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes"), new ScriptedAgent());
            var bayleef = e.ScenarioActive(0, C(ShieldBayleef));
            bayleef.DamageCounters = 3;
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            lee.DamageCounters = 2;
            var chikorita = e.ScenarioActive(1, C(Chikorita));
            chikorita.DamageCounters = 1;
            e.ScenarioAddToHand(0, C(HerbalMeganium));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Meganium", bayleef.Name);
            Assert.AreEqual(0, bayleef.DamageCounters);
            Assert.AreEqual(0, chikorita.DamageCounters, "the opponent's Grass Pokémon too");
            Assert.AreEqual(2, lee.DamageCounters, "not a Grass Pokémon");
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Char_Counter_Burns_After_Every_Turn_And_Stays_Through_Evolution()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Char"), new ScriptedAgent());
            var quilava = e.ScenarioActive(0, C(CharQuilava));
            Attach(e, quilava, Fire, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, false));
            Step(e);
            Assert.IsTrue(chansey.Markers.ContainsKey(PokemonInPlay.CharMarker));
            e.EndTurn();
            Assert.AreEqual(3 + 2, chansey.DamageCounters, "tails after the turn: 2 damage counters");
            e.ScenarioEvolve(chansey, C(Blissey));
            Assert.IsTrue(chansey.Markers.ContainsKey(PokemonInPlay.CharMarker), "it stays while the Pokémon is in play");
        }

        [Test]
        public void Mean_Look_Lasts_While_Murkrow_Stays_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mean Look"), new ScriptedAgent());
            var murkrow = e.ScenarioActive(0, C(Murkrow));
            Attach(e, murkrow, BasicDarkness, 1);
            var bench = e.ScenarioBench(0, C(Hitmonlee));
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 2);
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Retreat"));
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Retreat"), "no time limit");
            e.SwitchActive(0, bench);
            Assert.IsTrue(Offered(e, 1, "Retreat"), "Benching Murkrow ends it");
        }

        [Test]
        public void Spider_Web_Has_No_Time_Limit()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spider Web"), new ScriptedAgent());
            var ariados = e.ScenarioActive(0, C(Ariados));
            Attach(e, ariados, Grass, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 2);
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            NextTurn(e);
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Retreat"));
        }

        [Test]
        public void Pollen_Shield_Keeps_4_Special_Conditions_Off_But_Not_Burned()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pollen Shield"), new ScriptedAgent());
            var bayleef = e.ScenarioActive(0, C(ShieldBayleef));
            Attach(e, bayleef, Grass, 3);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(e.ApplyCondition(bayleef, SpecialCondition.Poisoned));
            Assert.IsFalse(e.ApplyCondition(bayleef, SpecialCondition.Asleep));
            Assert.IsTrue(e.ApplyCondition(bayleef, SpecialCondition.Burned));
        }

        [Test]
        public void Elemental_Blast_Needs_And_Discards_A_Fire_Water_And_Lightning_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Elemental Blast"), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(Lugia));
            Attach(e, lugia, Fire, 1);
            Attach(e, lugia, Water, 1);
            Attach(e, lugia, Lightning, 1);
            Attach(e, lugia, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, blissey.DamageCounters);
            Assert.AreEqual(1, lugia.Energy.Count, "the Psychic Energy stays");
        }

        [Test]
        public void Fin_Slap_Adds_10_After_Being_Damaged_Last_Turn()
        {
            foreach (bool hit in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Fin Slap"), new ScriptedAgent());
                var horsea = e.ScenarioActive(0, C(Horsea));
                Attach(e, horsea, Water, 2);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                if (hit) horsea.AttackDamageTurn = 2;
                Step(e);
                Assert.AreEqual(hit ? 3 : 2, blissey.DamageCounters);
            }
        }

        [Test]
        public void Super_Metronome_Copies_An_Attack_Of_The_Opponents_Pokemon_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Super Metronome", "Kick"), new ScriptedAgent());
            var togetic = e.ScenarioActive(0, C(Togetic));
            Attach(e, togetic, Psychic, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, lee.DamageCounters, "Kick's 20, no Energy needed");
        }
    }
}
