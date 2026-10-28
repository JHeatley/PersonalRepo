using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v128: Dark Explorers (DEX 1-111): the prints, Ancient Scream (your ruling: it stacks), Dark Claw, Psychic Mirage, Ancient
    /// Power (your ruling: Rare Candy too), Airhead, Scorching Scales, Solar Revelation, Slurp Shakedown and Lure Poison (your
    /// rulings: nothing with an empty Bench), Spit Squall, Deck and Cover, Volt Bolt, Softboiled, Electrisilk, Twist Mountain,
    /// Hooligans Jim &amp; Cas; and cards looked at with nothing to take are shown to a person (Trainers' Mail, Max Elixir).
    /// </summary>
    public class DexTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";

        const string Bulbasaur = "DEX-1";
        const string Ivysaur = "DEX-2";
        const string Venusaur = "DEX-3";
        const string Carnivine = "DEX-5";
        const string Accelgor = "DEX-11";
        const string Volcarona = "DEX-22";
        const string Slowbro = "DEX-24";
        const string Jolteon = "DEX-37";
        const string Raikou = "DEX-38";
        const string Galvantula = "DEX-43";
        const string Eelektross = "DEX-47";
        const string Espeon = "DEX-48";
        const string Woobat = "DEX-50";
        const string Aerodactyl = "DEX-53";
        const string Sableye = "DEX-62";
        const string Zorua = "DEX-70";
        const string Pawniard = "DEX-78";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string Eevee = "DEX-84";
        const string Haxorus = "DEX-89";
        const string DarkClaw = "DEX-92";
        const string Hooligans = "DEX-95";
        const string TwistMountain = "DEX-101";
        const string Gardevoir = "DEX-109";
        const string Archeops = "DEX-110";
        const string RareCandy = "MEG-125";
        const string TrainersMail = "AOR-100";
        const string MaxElixir = "BKP-102";

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

        /// <summary>Plays like the person on screen (IInteractiveAgent), answering like a ScriptedAgent.</summary>
        private sealed class PersonAgent : IInteractiveAgent
        {
            public readonly ScriptedAgent Inner = new ScriptedAgent();
            public List<DecisionRequest> Seen { get { return Inner.Seen; } }
            public List<int> Decide(DecisionRequest r) { return Inner.Decide(r); }
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(86, db.Cards.Count(d => d.SetCode == "DEX"));
            Assert.AreEqual("LTR-RC5", C("DEX-15").BaseKey, "Torchic");
            Assert.AreEqual("PLF-117", C("DEX-29").BaseKey, "Empoleon");
            Assert.AreEqual("LTR-88", C("DEX-63").BaseKey, "Darkrai-EX");
            Assert.AreEqual("LTR-88", C("DEX-107").BaseKey, "the full art Darkrai-EX");
            Assert.AreEqual("DRX-127", C("DEX-66").BaseKey, "Krookodile");
            Assert.AreEqual("FCO-106", C("DEX-97").BaseKey, "Old Amber Aerodactyl");
            Assert.AreEqual("DEX-13", C("DEX-103").BaseKey, "the full art Entei-EX");
            Assert.AreEqual("POR-82", C("DEX-111").BaseKey, "Pokémon Catcher");
            Assert.AreEqual(Stage.Restored, C(Aerodactyl).Stage);
            Assert.IsFalse(C(Aerodactyl).IsBasicPokemon);
            Assert.IsFalse(C(Sableye).Weakness.HasValue);
            Assert.AreEqual(EnergyType.Colorless, C(Haxorus).PokemonType);
            Assert.IsTrue(C("DEX-90").IsPokemonEX);
            Assert.IsFalse(C("DEX-90").UnlimitedOnly, "Expanded");
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Ancient_Scream_Adds_10_For_Each_Aerodactyl()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Scratch"), new ScriptedAgent());
            var pawniard = e.ScenarioActive(0, C(Pawniard));
            Attach(e, pawniard, Psychic, 2);
            e.ScenarioBench(0, C(Aerodactyl));
            e.ScenarioBench(0, C(Aerodactyl));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, chansey.DamageCounters, "20, plus 10 for each Aerodactyl (your ruling)");
        }

        [Test]
        public void Dark_Claw_Only_Helps_Darkness_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shadow Bind"), new ScriptedAgent());
            var zorua = e.ScenarioActive(0, C(Zorua));
            Attach(e, zorua, Darkness, 2);
            e.ScenarioAttachTool(zorua, C(DarkClaw));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, chansey.DamageCounters, "20 + 20");

            var m = Scenario(new ScriptedAgent().Then("Attack: Scratch"), new ScriptedAgent());
            var pawniard = m.ScenarioActive(0, C(Pawniard));
            Attach(m, pawniard, Psychic, 2);
            m.ScenarioAttachTool(pawniard, C(DarkClaw));
            var target = m.ScenarioActive(1, C(Chansey));
            m.ScenarioSetTurn(0, 3);
            Step(m);
            Assert.AreEqual(2, target.DamageCounters, "a Metal Pokémon gets nothing from it");
        }

        [Test]
        public void Psychic_Mirage_Doubles_Basic_Psychic_Energy_On_Psychic_Pokemon_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var woobat = e.ScenarioActive(0, C(Woobat));
            Attach(e, woobat, Psychic, 2);
            var pawniard = e.ScenarioBench(0, C(Pawniard));
            Attach(e, pawniard, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(2, e.EnergyCount(woobat), "no Gardevoir yet");
            e.ScenarioBench(0, C(Gardevoir));
            Assert.AreEqual(4, e.EnergyCount(woobat));
            Assert.AreEqual(1, e.EnergyCount(pawniard), "not a Psychic Pokémon");
            e.ScenarioBench(0, C(Gardevoir));
            Assert.AreEqual(4, e.EnergyCount(woobat), "only 1 Psychic Mirage applies");
        }

        [Test]
        public void Ancient_Power_Stops_Evolving_From_The_Hand_And_Rare_Candy()
        {
            foreach (bool archeops in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Bulbasaur));
                e.ScenarioAddToHand(0, C(Ivysaur));
                e.ScenarioAddToHand(0, C(Venusaur));
                e.ScenarioAddToHand(0, C(RareCandy));
                e.ScenarioActive(1, C(archeops ? Archeops : Chansey));
                e.ScenarioSetTurn(0, 3);
                var actions = e.GetMainActions(0);
                bool evolve = actions.Any(o => o.Action == ActionType.Evolve);
                bool candy = actions.Any(o => o.Label.Contains("Rare Candy"));
                Assert.AreEqual(!archeops, evolve, archeops ? "Ancient Power stops evolving from the hand" : "Bulbasaur can evolve");
                Assert.AreEqual(!archeops, candy, archeops ? "and Rare Candy (your ruling)" : "Rare Candy can be played");
            }
        }

        [Test]
        public void Airhead_Stops_Attacks_With_2_4_Or_6_Prize_Cards_Left()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slowbro = e.ScenarioActive(0, C(Slowbro));
            Attach(e, slowbro, Water, 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Func<bool> canAttack = () => e.GetMainActions(0).Any(o => o.Label.Contains("Lazy Headbutt"));
            Assert.IsFalse(canAttack(), "6 Prize cards left");
            e.State.Players[0].Prizes.RemoveAt(0);
            Assert.IsTrue(canAttack(), "5 left");
            e.State.Players[0].Prizes.RemoveAt(0);
            Assert.IsFalse(canAttack(), "4 left");
        }

        [Test]
        public void Scorching_Scales_Puts_4_Counters_On_The_Opponents_Burned_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Pawniard));
            e.ScenarioBench(0, C(Volcarona));
            var theirs = e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(4, e.BurnCounters(theirs));
            Assert.AreEqual(2, e.BurnCounters(mine), "only the opponent's");
        }

        [Test]
        public void Solar_Revelation_Prevents_Effects_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sleep Powder"), new ScriptedAgent());
            var ivysaur = e.ScenarioActive(0, C(Ivysaur));
            Attach(e, ivysaur, Grass, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            e.ScenarioBench(1, C(Espeon));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters, "the damage isn't prevented");
            Assert.IsFalse(chansey.Has(SpecialCondition.Asleep), "it has Energy attached: no Sleep");
        }

        [Test]
        public void Softboiled_Heals_30_From_The_Active_Pokemon_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Softboiled"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.DamageCounters = 5;
            e.ScenarioBench(0, C(Blissey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Slurp_Shakedown_Hits_The_New_Defending_Pokemon_Or_Does_Nothing()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Slurp Shakedown"), new ScriptedAgent());
            var eelektross = e.ScenarioActive(0, C(Eelektross));
            Attach(e, eelektross, Lightning, 4);
            var alone = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, alone.DamageCounters, "no Benched Pokémon: the attack does nothing (your ruling)");

            var m = Scenario(new ScriptedAgent().Then("Attack: Slurp Shakedown", "Chansey"), new ScriptedAgent());
            var attacker = m.ScenarioActive(0, C(Eelektross));
            Attach(m, attacker, Lightning, 4);
            var eevee = m.ScenarioActive(1, C(Eevee));
            m.ScenarioBench(1, C(Chansey));
            m.ScenarioBench(1, C("DEX-81"));
            m.ScenarioSetTurn(0, 3);
            Step(m);
            var active = m.State.Players[1].Active;
            Assert.AreEqual("Chansey", active.Name);
            Assert.AreEqual(6, active.DamageCounters);
            Assert.AreEqual(0, eevee.DamageCounters);
        }

        [Test]
        public void Lure_Poison_Poisons_The_Pokemon_Switched_In()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lure Poison", "Chansey"), new ScriptedAgent());
            var carnivine = e.ScenarioActive(0, C(Carnivine));
            Attach(e, carnivine, Grass, 1);
            var eevee = e.ScenarioActive(1, C(Eevee));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioBench(1, C("DEX-81"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var active = e.State.Players[1].Active;
            Assert.AreEqual("Chansey", active.Name);
            Assert.IsTrue(active.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(eevee.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Spit_Squall_Puts_The_Defending_Pokemon_And_Its_Cards_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spit Squall"), new ScriptedAgent());
            var carnivine = e.ScenarioActive(0, C(Carnivine));
            Attach(e, carnivine, Grass, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[1].Hand;
            Assert.IsTrue(hand.Any(c => c.Def.Key == Chansey));
            Assert.IsTrue(hand.Any(c => c.Def.Key == Psychic), "with its Energy");
            Assert.AreEqual("Eevee", e.State.Players[1].Active.Name);
        }

        [Test]
        public void Deck_And_Cover_Paralyzes_And_Poisons_Then_Accelgor_Goes_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Deck and Cover"), new ScriptedAgent());
            var accelgor = e.ScenarioActive(0, C(Accelgor));
            Attach(e, accelgor, Psychic, 2);
            e.ScenarioBench(0, C(Eevee));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(chansey.Has(SpecialCondition.Paralyzed));
            Assert.IsTrue(chansey.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(e.State.Players[0].InPlay().Contains(accelgor));
            Assert.IsTrue(e.State.Players[0].Deck.Any(c => c.Def.Key == Accelgor));
        }

        [Test]
        public void Volt_Bolt_Discards_The_Lightning_Energy_And_Hits_Any_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Volt Bolt", "Blissey"), new ScriptedAgent());
            var raikou = e.ScenarioActive(0, C(Raikou));
            Attach(e, raikou, Lightning, 3);
            e.ScenarioActive(1, C(Eevee));
            var blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, blissey.DamageCounters);
            Assert.AreEqual(0, raikou.Energy.Count);
        }

        [Test]
        public void Electrisilk_Does_40_More_To_A_Pokemon_With_No_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Electrisilk"), new ScriptedAgent());
            var galvantula = e.ScenarioActive(0, C(Galvantula));
            Attach(e, galvantula, Lightning, 2);
            var jolteon = e.ScenarioActive(1, C(Jolteon));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, jolteon.DamageCounters, "30 + 40");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Twist_Mountain_Benches_A_Restored_Pokemon_From_The_Hand_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Twist Mountain"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Aerodactyl));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(0, C(TwistMountain));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Bench.Any(m => m.Def.Key == Aerodactyl));
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Hooligans_Jim_And_Cas_Shuffles_3_Random_Cards_Back_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hooligans Jim & Cas"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Hooligans));
            e.ScenarioActive(1, C(Chansey));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count);
            Assert.AreEqual(23, e.State.Players[1].Deck.Count);
        }

        // ------------------------------------------------------------------ Looking at cards (your request)

        [Test]
        public void Trainers_Mail_Shows_A_Person_The_4_Cards_Even_With_No_Trainer_There()
        {
            var person = new PersonAgent();
            person.Inner.Then("Play Trainers' Mail");
            var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(TrainersMail));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var shown = person.Seen.Where(r => r.Kind == DecisionKind.ChooseCards).ToList();
            Assert.AreEqual(1, shown.Count, "shown once");
            Assert.AreEqual(0, shown[0].MaxChoices, "nothing to take, just Done");
            Assert.AreEqual(4, shown[0].SeenCards.Count, "the 4 cards it looked at");
            Assert.AreEqual(20, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Trainers_Mail_With_One_Trainer_Lets_A_Person_Choose_It_Or_Nothing()
        {
            var person = new PersonAgent();
            person.Inner.Then("Play Trainers' Mail", "Hooligans Jim & Cas");
            var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Hooligans)));
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(TrainersMail));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var asks = person.Seen.Where(r => r.Kind == DecisionKind.ChooseCards).ToList();
            Assert.AreEqual(1, asks.Count, "one choice, not an extra look");
            Assert.AreEqual(1, asks[0].Options.Count, "the one Trainer can be taken");
            Assert.AreEqual(0, asks[0].MinChoices, "or none");
            Assert.AreEqual(4, asks[0].SeenCards.Count, "all 4 cards are there to look at");
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Hooligans));
        }

        [Test]
        public void Max_Elixir_Shows_A_Person_The_6_Cards_With_No_Basic_Energy_There()
        {
            var person = new PersonAgent();
            person.Inner.Then("Play Max Elixir");
            var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C(Hooligans), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C(Eevee));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(MaxElixir));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var shown = person.Seen.Where(r => r.Kind == DecisionKind.ChooseCards).ToList();
            Assert.AreEqual(1, shown.Count);
            Assert.AreEqual(0, shown[0].MaxChoices);
            Assert.AreEqual(6, shown[0].SeenCards.Count);

            var bot = new ScriptedAgent().Then("Play Max Elixir");
            var b = Scenario(bot, new ScriptedAgent());
            b.State.Players[0].Deck.Clear();
            b.ScenarioAddToDeck(0, C(Hooligans), 20);
            b.ScenarioActive(0, C(Chansey));
            b.ScenarioBench(0, C(Eevee));
            b.ScenarioActive(1, C(Chansey));
            b.ScenarioAddToHand(0, C(MaxElixir));
            b.ScenarioSetTurn(0, 3);
            Step(b);
            Assert.IsFalse(bot.Seen.Any(r => r.Kind == DecisionKind.ChooseCards), "a bot isn't asked");
        }
    }
}
