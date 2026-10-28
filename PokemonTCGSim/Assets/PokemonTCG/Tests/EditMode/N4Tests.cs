using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v142: Neo Destiny (N4): the card data and the 1-per-deck rule, the new engine hooks (a Pokémon going from a hand onto the
    /// Bench, a retreat attempt seen by Pokémon Powers in play, a Pokémon Power stopping other Pokémon Powers, Resistance ignored by
    /// a timed effect, Hidden Power bonuses) and the Pokémon Powers, attacks, Trainers and Miracle Energy, with your rulings.
    /// </summary>
    public class N4Tests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";

        const string DarkAmpharos = "N4-1";
        const string DarkFeraligatr = "N4-5";
        const string DarkGengar = "N4-6";
        const string DarkHoundoom = "N4-7";
        const string DarkPorygon2 = "N4-8";
        const string LightArcanine = "N4-12";
        const string LightDragonite = "N4-14";
        const string MiracleEnergy = "N4-16";
        const string DarkMagcargo = "N4-18";
        const string DarkUrsaring = "N4-21";
        const string LightPiloswine = "N4-26";
        const string UnownG = "N4-27";
        const string UnownW = "N4-29";
        const string UnownX = "N4-30";
        const string DarkCroconaw = "N4-32";
        const string DarkForretress = "N4-35";
        const string DarkHaunter = "N4-36";
        const string LightJolteon = "N4-48";
        const string UnownC = "N4-57";
        const string UnownP = "N4-58";
        const string UnownQ = "N4-59";
        const string UnownZ = "N4-60";
        const string UnownT = "N4-88";
        const string UnownV = "N4-89";
        const string BrokenGroundGym = "N4-92";
        const string ExpAll = "N4-93";
        const string ImpostorOak = "N4-94";
        const string ThoughtWaveMachine = "N4-96";
        const string CounterattackClaws = "N4-97";
        const string Magnifier = "N4-101";
        const string PersonalityTest = "N4-102";
        const string MailFromBill = "N4-105";
        const string ShiningMewtwo = "N4-109";
        const string ShiningRaichu = "N4-111";
        const string UnownA = "N2-14";
        const string UnownN = "N2-50";
        const string Igglybuff = "N2-40";
        const string Tyrogue = "N2-66";
        const string Potion = "BS-94";
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

        private List<CardDef> DeckOf(params string[] keys)
        {
            var deck = keys.Select(C).ToList();
            while (deck.Count < 60) deck.Add(C(Psychic));
            return deck;
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void Neo_Destiny_Is_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "N4").ToList();
            Assert.AreEqual(111, set.Count, "113 cards less Lucky Stadium (Wizards promo) and Shining Celebi (30th Celebration)");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.IsFalse(set.Any(d => d.Number == "100" || d.Number == "106"));
            Assert.IsTrue(C(UnownG).UnownRule && C(UnownV).UnownRule && C(UnownX).UnownRule);
            Assert.IsTrue(C(MiracleEnergy).IsSpecialEnergy && C(MiracleEnergy).OnePerDeck);
            Assert.IsTrue(C(ShiningMewtwo).OnePerDeck && C(ShiningRaichu).OnePerDeck);
            Assert.IsFalse(C(LightArcanine).OnePerDeck);
            Assert.IsTrue(C(BrokenGroundGym).IsStadium && C(ExpAll).IsTool && C(CounterattackClaws).IsTool);
        }

        [Test]
        public void Miracle_Energy_And_Shining_Pokemon_Are_1_Per_Deck()
        {
            Assert.IsTrue(Decklist.Validate(DeckOf(MiracleEnergy, MiracleEnergy, Hitmonlee), null).Any(p => p.Contains("Miracle Energy")));
            Assert.IsTrue(Decklist.Validate(DeckOf(ShiningMewtwo, ShiningMewtwo), null).Any(p => p.Contains("Shining Mewtwo")));
            var one = Decklist.Validate(DeckOf(MiracleEnergy, ShiningMewtwo), DeckFormat.Unlimited);
            Assert.IsFalse(one.Any(p => p.Contains("Miracle Energy") || p.Contains("Shining Mewtwo")), string.Join("; ", one.ToArray()));
            var two = Decklist.Validate(DeckOf(ShiningMewtwo, ShiningMewtwo), DeckFormat.Unlimited);
            Assert.AreEqual(1, two.Count(p => p.Contains("Shining Mewtwo")), "reported once");
        }

        // ------------------------------------------------------------------ Miracle Energy and Trainers

        [Test]
        public void Miracle_Energy_Goes_On_Light_Pokemon_Gives_2_Energy_And_Is_Discarded_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Miracle Energy"), new ScriptedAgent());
            var arcanine = e.ScenarioActive(0, C(LightArcanine));
            e.ScenarioBench(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(MiracleEnergy));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Miracle Energy to Hitmonlee"), "only Shining or Light Pokémon");
            Step(e);
            Assert.AreEqual(1, arcanine.Energy.Count);
            Assert.AreEqual(2, e.EnergyCount(arcanine));
            e.EndTurn();
            Assert.AreEqual(0, arcanine.Energy.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Miracle Energy"));
        }

        [Test]
        public void Miraculous_Wind_Makes_Special_Energy_Colorless()
        {
            foreach (bool asleep in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var dragonite = e.ScenarioActive(0, C(LightDragonite));
                var arcanine = e.ScenarioActive(1, C(LightArcanine));
                e.ScenarioAttach(arcanine, C(MiracleEnergy));
                if (asleep) e.ApplyCondition(dragonite, SpecialCondition.Asleep);
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(asleep ? 2 : 1, e.EnergyCount(arcanine));
            }
        }

        [Test]
        public void Broken_Ground_Gym_Adds_1_To_Basic_Pokemon_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            var arcanine = e.ScenarioActive(1, C(LightArcanine));
            e.ScenarioSetStadium(0, C(BrokenGroundGym));
            Assert.AreEqual(3, e.RetreatCostOf(lee));
            Assert.AreEqual(2, e.RetreatCostOf(arcanine), "a Stage 1 isn't affected");
        }

        [Test]
        public void Magnifier_Lets_The_Attack_Ignore_Resistance()
        {
            foreach (bool magnifier in new[] { false, true })
            {
                var agent = new ScriptedAgent();
                if (magnifier) agent.Then("Play Magnifier");
                agent.Then("Attack: Kick");
                var e = Scenario(agent, new ScriptedAgent());
                var lee = e.ScenarioActive(0, C(Hitmonlee));
                Attach(e, lee, Fighting, 1);
                if (magnifier) e.ScenarioAddToHand(0, C(Magnifier));
                var dragonite = e.ScenarioActive(1, C(LightDragonite));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                if (magnifier) Step(e);
                Assert.AreEqual(magnifier ? 2 : 0, dragonite.DamageCounters, "Fighting Resistance 30");
            }
        }

        [Test]
        public void Counterattack_Claws_Puts_2_Counters_On_The_Attacker_And_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Kick"));
            var mine = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAttachTool(mine, C(CounterattackClaws));
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(1, 4);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(4, mine.DamageCounters, "Kick against Fighting Weakness");
            Assert.AreEqual(2, lee.DamageCounters);
            Assert.AreEqual(0, mine.Tools.Count);
        }

        [Test]
        public void Exp_All_Takes_A_Basic_Energy_From_The_Knocked_Out_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Kick"));
            var mine = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, mine, Fighting, 1);
            mine.DamageCounters = 7;
            var chansey = e.ScenarioBench(0, C(Chansey));
            e.ScenarioAttachTool(chansey, C(ExpAll));
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(mine.Uid));
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.AreEqual(0, chansey.Tools.Count, "EXP.ALL is discarded");
        }

        [Test]
        public void Mail_From_Bill_Draws_Up_To_4_Only_With_A_Small_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mail from Bill"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(MailFromBill));
            e.ScenarioAddToHand(0, C(Potion));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Mail from Bill"), "5 cards counting itself");
            e.State.Players[0].Hand.RemoveAll(c => c.Def.Name == "Fire Energy");
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Thought_Wave_Machine_Returns_Energy_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Thought Wave Machine"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(ThoughtWaveMachine));
            var blissey = e.ScenarioActive(1, C(Blissey));
            Attach(e, blissey, Psychic, 3);
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            bool over = e.MainPhaseStep();
            Assert.AreEqual(1, blissey.Energy.Count);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count);
            Assert.IsTrue(over, "the turn ends");
        }

        [Test]
        public void Pokemon_Personality_Test_Draws_For_Whoever_Was_Right()
        {
            foreach (bool right in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Pokémon Personality Test"), new ScriptedAgent().Then(right ? "Dark" : "Neither"));
                e.ScenarioActive(0, C(Hitmonlee));
                e.ScenarioAddToHand(0, C(PersonalityTest));
                e.ScenarioAddToHand(0, C(DarkCroconaw));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(right ? 1 : 4, e.State.Players[0].Hand.Count, "Dark Croconaw goes back to the hand");
                Assert.AreEqual(right ? 3 : 0, e.State.Players[1].Hand.Count);
            }
        }

        [Test]
        public void Impostor_Professor_Oaks_Invention_Replaces_The_Opponents_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Impostor", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(ImpostorOak));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[1];
            Assert.AreEqual(6, p.Prizes.Count);
            Assert.AreEqual(20, p.Deck.Count);
            Assert.AreEqual(6, p.Deck.Count(c => c.Def.Name == "Fire Energy") + p.Prizes.Count(c => c.Def.Name == "Fire Energy"), "the old Prize cards went into the deck");
        }

        // ------------------------------------------------------------------ Pokémon Powers

        [Test]
        public void Conductivity_Damages_What_The_Opponent_Attaches_Energy_To()
        {
            foreach (int ampharos in new[] { 1, 2 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Psychic Energy"));
                e.ScenarioActive(0, C(Hitmonlee));
                for (int i = 0; i < ampharos; i++) e.ScenarioBench(0, C(DarkAmpharos));
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioAddToHand(1, C(Psychic));
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(ampharos == 1 ? 1 : 0, blissey.DamageCounters, "it stops with more than 1 Dark Ampharos");
            }
        }

        [Test]
        public void Scare_Stops_The_Opponents_Baby_Pokemon()
        {
            foreach (bool feraligatr in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                if (feraligatr) e.ScenarioActive(0, C(DarkFeraligatr));
                else e.ScenarioActive(0, C(Hitmonlee));
                var tyrogue = e.ScenarioActive(1, C(Tyrogue));
                Attach(e, tyrogue, Psychic, 1);
                var igglybuff = e.ScenarioBench(1, C(Igglybuff));
                e.ScenarioSetTurn(1, 4);
                Assert.AreEqual(!feraligatr, Offered(e, 1, "Attack: Smash Punch"));
                Assert.AreEqual(feraligatr, e.AbilityBlocked(igglybuff, "Gaze"));
            }
        }

        [Test]
        public void Deep_Sleep_Takes_2_Heads_To_Wake_Up()
        {
            foreach (bool gengar in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Hitmonlee));
                if (gengar) e.ScenarioBench(0, C(DarkGengar));
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ApplyCondition(blissey, SpecialCondition.Asleep);
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(true, false));
                e.EndTurn();
                Assert.AreEqual(gengar, blissey.Has(SpecialCondition.Asleep));
            }
        }

        [Test]
        public void Hot_Plate_Hits_A_Pokemon_Benched_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Bench Hitmonlee"));
            e.ScenarioActive(0, C(DarkMagcargo));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(Hitmonlee));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].Bench.Single().DamageCounters);
        }

        [Test]
        public void Provoke_Benches_Both_Players_Basic_Pokemon_And_Hot_Plate_Sees_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Provoke"), new ScriptedAgent());
            var ursaring = e.ScenarioActive(0, C(DarkUrsaring));
            Attach(e, ursaring, Psychic, 1);
            e.ScenarioAddToHand(0, C(Chansey));
            e.ScenarioActive(1, C(DarkMagcargo));
            e.ScenarioAddToHand(1, C(Hitmonlee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var theirs = e.State.Players[1].Bench.Single();
            var mine = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Hitmonlee", theirs.Name);
            Assert.AreEqual("Chansey", mine.Name);
            Assert.AreEqual(1, theirs.DamageCounters, "your ruling: put there by an effect counts");
            Assert.AreEqual(1, mine.DamageCounters);
        }

        [Test]
        public void Chase_Hits_A_Retreating_Pokemon_On_Heads()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Retreat"));
            e.ScenarioActive(0, C(UnownC));
            var blissey = e.ScenarioActive(1, C(Blissey));
            Attach(e, blissey, Psychic, 3);
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(1, 4);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters);
            Assert.AreSame(chansey, e.State.Players[1].Active, "the retreat still happens");
        }

        [Test]
        public void Zoom_Takes_Away_Unown_Retreat_Costs_From_The_Bench()
        {
            foreach (bool benched in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var active = benched ? e.ScenarioActive(0, C(UnownC)) : e.ScenarioActive(0, C(UnownZ));
                var other = benched ? e.ScenarioBench(0, C(UnownZ)) : e.ScenarioBench(0, C(UnownC));
                e.ScenarioSetStadium(1, C(BrokenGroundGym));
                Assert.AreEqual(benched ? 0 : 2, e.RetreatCostOf(active), "no Retreat Cost wins over Broken Ground Gym");
                Assert.IsNotNull(other);
            }
        }

        [Test]
        public void Xxxxx_Adds_10_For_Each_Heads_To_Hidden_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hidden Power"), new ScriptedAgent());
            var g = e.ScenarioActive(0, C(UnownG));
            Attach(e, g, Psychic, 1);
            e.ScenarioBench(0, C(UnownX));
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(3, blissey.DamageCounters);
        }

        [Test]
        public void Perform_Adds_The_Damage_Taken_Last_Turn_To_Hidden_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hidden Power"), new ScriptedAgent().Then("Attack: Kick"));
            var p = e.ScenarioActive(0, C(UnownP));
            Attach(e, p, Psychic, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, p.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(6, lee.DamageCounters, "(10 + 20) x2 for Psychic Weakness");
        }

        [Test]
        public void Quicken_Protects_Unown_From_The_Next_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Use [Quicken]"), new ScriptedAgent().Then("Attack: Kick"));
            var q = e.ScenarioActive(0, C(UnownQ));
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsFalse(Offered(e, 0, "Use [Quicken]"), "once a turn");
            NextTurn(e);
            Step(e);
            Assert.AreEqual(0, q.DamageCounters);
        }

        [Test]
        public void Want_Takes_A_Trainer_From_The_Discard_Pile_With_W_A_N_T_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use [Want]"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioBench(0, C(UnownW));
            e.ScenarioBench(0, C(UnownA));
            e.ScenarioBench(0, C(UnownN));
            e.ScenarioAddToDiscard(0, C(Potion), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use [Want]"), "no Unown [T] yet");
            e.ScenarioBench(0, C(UnownT));
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Potion", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Vanish_Returns_Another_Unown_And_Discards_Its_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Unown [V]", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            var g = e.ScenarioBench(0, C(UnownG));
            Attach(e, g, Psychic, 1);
            e.ScenarioAddToHand(0, C(UnownV));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsNull(e.State.FindPokemon(g.Uid));
            Assert.AreEqual("Unown [G]", e.State.Players[0].Hand.Single().Def.Name);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Fluffy_Wool_Puts_The_Attacker_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Kick"));
            var piloswine = e.ScenarioActive(0, C(LightPiloswine));
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(1, 4);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, piloswine.DamageCounters);
            Assert.IsTrue(lee.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Spatial_Distortion_Puts_A_Stadium_From_The_Discard_Pile_Into_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Spatial Distortion"), new ScriptedAgent());
            e.ScenarioActive(0, C(DarkPorygon2));
            e.ScenarioAddToDiscard(0, C(BrokenGroundGym), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Broken Ground Gym", e.State.Stadium.Def.Name);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Dark_Fire_Does_60_When_It_Discards_A_Darkness_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dark Fire"), new ScriptedAgent());
            var houndoom = e.ScenarioActive(0, C(DarkHoundoom));
            Attach(e, houndoom, Fire, 1);
            Attach(e, houndoom, Darkness, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, blissey.DamageCounters);
            Assert.AreEqual(1, houndoom.Energy.Count);
        }

        [Test]
        public void Eerie_Howl_Benches_A_Basic_From_The_Opponents_Hand_And_Switches_It_In()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Eerie Howl"), new ScriptedAgent());
            var houndoom = e.ScenarioActive(0, C(DarkHoundoom));
            Attach(e, houndoom, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(Hitmonlee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Hitmonlee", e.State.Players[1].Active.Name);
            Assert.AreSame(blissey, e.State.Players[1].Bench.Single());
        }

        [Test]
        public void Reflect_Shield_Prevents_The_Damage_And_Hits_Back_With_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Reflect Shield"), new ScriptedAgent().Then("Attack: Kick"));
            var mewtwo = e.ScenarioActive(0, C(ShiningMewtwo));
            Attach(e, mewtwo, Psychic, 1);
            Attach(e, mewtwo, Lightning, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(0, mewtwo.DamageCounters);
            Assert.AreEqual(4, lee.DamageCounters, "your ruling: 20 doubled by Psychic Weakness");
        }

        [Test]
        public void Pulse_Guard_Prevents_30_Or_More_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pulse Guard"), new ScriptedAgent().Then("Attack: High Jump Kick"));
            var jolteon = e.ScenarioActive(0, C(LightJolteon));
            Attach(e, jolteon, Lightning, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(0, jolteon.DamageCounters);
        }

        [Test]
        public void Armor_Up_Can_Save_Dark_Forretress_From_Its_Own_Explosion()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Armor Up", "Attack: Explosion"), new ScriptedAgent());
            var forretress = e.ScenarioActive(0, C(DarkForretress));
            Attach(e, forretress, Grass, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(6, blissey.DamageCounters);
            Assert.IsNotNull(e.State.FindPokemon(forretress.Uid));
            Assert.AreEqual(5, forretress.DamageCounters, "10 HP left");
        }

        [Test]
        public void Call_Back_Needs_Room_On_The_Opponents_Bench()
        {
            foreach (bool full in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Call Back"), new ScriptedAgent());
                var haunter = e.ScenarioActive(0, C(DarkHaunter));
                Attach(e, haunter, Psychic, 1);
                e.ScenarioActive(1, C(Blissey));
                if (full) for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Chansey));
                e.ScenarioAddToDiscard(1, C(Hitmonlee), 1);
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!full, Offered(e, 0, "Attack: Call Back"));
                if (full) continue;
                Step(e);
                var lee = e.State.Players[1].Bench.Single();
                Assert.AreEqual("Hitmonlee", lee.Name);
                Assert.AreEqual(1, lee.DamageCounters);
            }
        }
    }
}
