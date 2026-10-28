using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>Mechanics added for the Recife Regional card pool, plus whole games with every built-in deck.</summary>
    public class RecifeTests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Fire = "MEE-10";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";
        const string Lightning = "MEE-12";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string Kangaskhan = "MEG-104";

        private sealed class EventLog : IGameObserver
        {
            public readonly List<GameEvent> Events = new List<GameEvent>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Events.Add(ev); }
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private static bool Has(GameEngine e, int player, string label)
        {
            return e.GetMainActions(player).Any(o => o.Label == label);
        }

        // ------------------------------------------------------------------ Decks

        [Test]
        public void Every_Builtin_Deck_Is_A_Legal_60()
        {
            var decks = DeckLibrary.LoadAll().Where(d => d.BuiltIn).ToList();
            Assert.GreaterOrEqual(decks.Count, 28);
            foreach (var d in decks)
            {
                var problems = new List<string>();
                var cards = DeckLibrary.Build(db, d, problems);
                CollectionAssert.IsEmpty(problems, d.Name + ": " + string.Join(" ", problems.ToArray()));
                Assert.AreEqual(60, cards.Count, d.Name);
            }
        }

        private List<KeyValuePair<string, List<CardDef>>> BuiltInDecks()
        {
            var list = new List<KeyValuePair<string, List<CardDef>>>();
            foreach (var d in DeckLibrary.LoadAll().Where(x => x.BuiltIn))
                list.Add(new KeyValuePair<string, List<CardDef>>(d.Name, DeckLibrary.Build(db, d, new List<string>())));
            return list;
        }

        [Test]
        public void Every_Builtin_Deck_Plays_Full_Games_With_Heuristic_Bots()
        {
            var decks = BuiltInDecks();
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
            {
                var other = decks[(i + 1) % decks.Count];
                var report = MatchRunner.Run(scripts, decks[i].Value, other.Value, 2, 500 + i * 10, (seat, s) => new HeuristicAgent(s), true);
                foreach (var err in report.Errors) errors.Add(decks[i].Key + " vs " + other.Key + ": " + err);
            }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }

        // Slow in the Unity Editor: a longer limit than the Test Runner's default.
        [Test, Timeout(600000)]
        public void Every_Builtin_Deck_Survives_Random_Play()
        {
            var decks = BuiltInDecks();
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
            {
                var other = decks[(i + 7) % decks.Count];
                var report = MatchRunner.Run(scripts, decks[i].Value, other.Value, 3, 900 + i * 10, (seat, s) => new RandomAgent(s), true);
                foreach (var err in report.Errors) errors.Add(decks[i].Key + " vs " + other.Key + ": " + err);
            }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }

        // ------------------------------------------------------------------ Evolution

        [Test]
        public void Rare_Candy_Evolves_A_Basic_To_Stage_2_And_Triggers_Psychic_Draw()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rare Candy", "Yes"), new ScriptedAgent());
            var abra = e.ScenarioActive(0, C("MEG-54"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEG-125"));
            e.ScenarioAddToHand(0, C("MEG-56"));

            e.ScenarioSetTurn(0, 2);
            Assert.IsFalse(Has(e, 0, "Play Rare Candy"), "Rare Candy during the first turn");

            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Alakazam", abra.Name);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "Psychic Draw draws 3");
        }

        [Test]
        public void Salvatore_Works_On_The_Second_Players_First_Turn_But_Not_Going_First()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Salvatore"), new ScriptedAgent());
            var chikorita = e.ScenarioActive(0, C("MEG-8"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C("MEG-9"), 1);
            e.ScenarioAddToHand(0, C("TEF-160"));

            e.ScenarioSetTurn(0, 1);
            Assert.IsFalse(Has(e, 0, "Play Salvatore"), "no Supporters on the first player's first turn");

            e.ScenarioSetTurn(0, 2);
            chikorita.TurnEnteredPlay = 2;
            Step(e);
            Assert.AreEqual("Bayleef", chikorita.Name);
        }

        // ------------------------------------------------------------------ Energy

        [Test]
        public void Prism_And_Neo_Upper_Energy_Depend_On_The_Holders_Stage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var abra = e.ScenarioActive(0, C("MEG-54"));
            e.ScenarioAttach(abra, C("ASC-216"));
            Assert.IsTrue(e.CanPayCost(abra, new[] { EnergyType.Grass }), "Prism Energy on a Basic provides any type");

            var kadabra = e.ScenarioBench(0, C("MEG-54"));
            e.ScenarioEvolve(kadabra, C("MEG-55"));
            e.ScenarioAttach(kadabra, C("ASC-216"));
            Assert.IsFalse(e.CanPayCost(kadabra, new[] { EnergyType.Psychic }), "Prism Energy on a Stage 1 is only Colorless");
            Assert.IsTrue(e.CanPayCost(kadabra, new[] { EnergyType.Colorless }));

            var alakazam = e.ScenarioBench(0, C("MEG-54"));
            e.ScenarioEvolve(alakazam, C("MEG-55"));
            e.ScenarioEvolve(alakazam, C("MEG-56"));
            e.ScenarioAttach(alakazam, C("TEF-162"));
            Assert.AreEqual(2, e.EnergyCount(alakazam), "Neo Upper Energy on a Stage 2 provides 2");
            Assert.IsTrue(e.CanPayCost(alakazam, new[] { EnergyType.Psychic, EnergyType.Fire }));
        }

        [Test]
        public void Team_Rockets_Energy_Only_Goes_On_Team_Rockets_Pokemon_And_Provides_Two()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var murkrow = e.ScenarioActive(0, C("DRI-127"));
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("DRI-182"));
            e.ScenarioSetTurn(0, 3);

            var actions = e.GetMainActions(0);
            Assert.IsTrue(actions.Any(o => o.Label == "Attach Team Rocket's Energy to Team Rocket's Murkrow (Active)"));
            Assert.IsFalse(actions.Any(o => o.Label.StartsWith("Attach Team Rocket's Energy to Dreepy")));

            e.ScenarioAttach(murkrow, C("DRI-182"));
            Assert.AreEqual(2, e.EnergyCount(murkrow));
            Assert.IsTrue(Has(e, 0, "Attack: Torment"), "Torment costs Darkness + Colorless");
        }

        [Test]
        public void Meganium_Makes_Basic_Grass_Energy_Provide_Two()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var meganium = e.ScenarioBench(0, C("MEG-8"));
            e.ScenarioEvolve(meganium, C("MEG-9"));
            e.ScenarioEvolve(meganium, C("MEG-10"));
            var ogerpon = e.ScenarioActive(0, C("TWM-25"));
            e.ScenarioAttach(ogerpon, C(Grass));
            e.ScenarioAttach(ogerpon, C(Grass));
            Assert.AreEqual(4, e.EnergyCount(ogerpon));
            Assert.IsTrue(e.CanPayAttack(ogerpon, ogerpon.Def.Attacks[0]), "Myriad Leaf Shower (GGG) with 2 Grass Energy");
        }

        [Test]
        public void Telepathic_Psychic_Energy_Benches_Psychic_Basics_When_Attached_From_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Telepathic Psychic Energy to Abra (Active)", "Abra|Abra"), new ScriptedAgent());
            e.ScenarioActive(0, C("MEG-54"));
            e.ScenarioAddToDeck(0, C("MEG-54"), 2);
            e.ScenarioAddToHand(0, C("POR-88"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Boomerang_Energy_Comes_Back_After_Its_Holders_Attack_Discards_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Smashing Headbutt", "Boomerang Energy|Psychic Energy"), new ScriptedAgent());
            var bouffalant = e.ScenarioActive(0, C("SSP-151"));
            e.ScenarioAttach(bouffalant, C("TWM-166"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(bouffalant, C(Psychic));
            var target = e.ScenarioActive(1, C(Kangaskhan));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, target.DamageCounters);
            Assert.AreEqual(3, bouffalant.Energy.Count);
            Assert.IsTrue(bouffalant.Energy.Any(c => c.Def.Name == "Boomerang Energy"));
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Ignition_Energy_Provides_Three_On_An_Evolution_And_Is_Discarded_At_End_Of_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kadabra = e.ScenarioActive(0, C("MEG-54"));
            e.ScenarioEvolve(kadabra, C("MEG-55"));
            e.ScenarioAttach(kadabra, C("WHT-86"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(3, e.EnergyCount(kadabra));
            e.EndTurn();
            Assert.AreEqual(0, kadabra.Energy.Count);
        }

        [Test]
        public void Hops_Choice_Band_Lowers_The_Attack_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zacian = e.ScenarioActive(0, C("JTG-111"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(zacian, C(Metal));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Has(e, 0, "Attack: Brave Slash"), "Brave Slash needs 4 Energy without the band");
            e.ScenarioAttachTool(zacian, C("JTG-148"));
            Assert.IsTrue(Has(e, 0, "Attack: Brave Slash"));
        }

        // ------------------------------------------------------------------ Prevention and blocks

        [Test]
        public void Mist_Energy_Prevents_Poison_And_Cant_Retreat_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Poison Chain"), new ScriptedAgent());
            var pecharunt = e.ScenarioActive(0, C("SVP-149"));
            e.ScenarioAttach(pecharunt, C(Darkness));
            e.ScenarioAttach(pecharunt, C(Darkness));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAttach(target, C("TEF-161"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, target.DamageCounters);
            Assert.IsFalse(target.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(e.RetreatPrevented(target));
        }

        [Test]
        public void Hide_N_Sneak_Stops_Cursed_Blast_Counters_But_The_User_Is_Still_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Cursed Blast", "Poltchageist"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C("PRE-36"));
            var poltchageist = e.ScenarioActive(1, C("PBL-5"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, poltchageist.DamageCounters);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count, "Dusclops is Knocked Out");
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count);
        }

        [Test]
        public void Damp_Blocks_Cursed_Blast_Unless_Flutter_Mane_Silences_The_Active_Psyduck()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C("PRE-36"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C("ASC-39"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Cursed Blast")), "Damp from the Bench");

            var f = Scenario(new ScriptedAgent(), new ScriptedAgent());
            f.ScenarioActive(0, C("TEF-78"));
            f.ScenarioBench(0, C("PRE-36"));
            f.ScenarioActive(1, C("ASC-39"));
            f.ScenarioSetTurn(0, 3);
            Assert.IsTrue(f.GetMainActions(0).Any(o => o.Label.StartsWith("Use Cursed Blast")), "Flutter Mane turns off the Active Psyduck's Damp");
        }

        [Test]
        public void Flutter_Mane_Turns_Off_The_Opposing_Active_Pokemons_Ability()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var latias = e.ScenarioActive(1, C("SSP-76"));
            Assert.AreEqual(0, e.RetreatCostOf(latias), "Skyliner: Basic Pokémon have no Retreat Cost");

            var f = Scenario(new ScriptedAgent(), new ScriptedAgent());
            f.ScenarioActive(0, C("TEF-78"));
            var latias2 = f.ScenarioActive(1, C("SSP-76"));
            Assert.AreEqual(2, f.RetreatCostOf(latias2));
        }

        [Test]
        public void Battle_Cage_Stops_Phantom_Dive_Counters_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Phantom Dive"), new ScriptedAgent());
            var dragapult = e.ScenarioActive(0, C("TWM-130"));
            e.ScenarioAttach(dragapult, C(Fire));
            e.ScenarioAttach(dragapult, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            var benched = e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetStadium(1, C("PFL-85"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, benched.DamageCounters);
            Assert.AreSame(benched, e.State.Players[1].Active);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Jellicent_Ex_Stops_Items_And_Tools_But_Not_Supporters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C(Dreepy));
            var jellicent = e.ScenarioActive(1, C("WHT-44"));
            e.ScenarioEvolve(jellicent, C("WHT-45"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEG-130"));
            e.ScenarioAddToHand(0, C("ASC-181"));
            e.ScenarioAddToHand(0, C("MEG-114"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Has(e, 0, "Play Switch"));
            Assert.IsFalse(Has(e, 0, "Play Air Balloon"));
            Assert.IsTrue(Has(e, 0, "Play Boss's Orders"));
        }

        [Test]
        public void Itchy_Pollen_Stops_Items_But_Not_Pokemon_Tools()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Itchy Pollen"), new ScriptedAgent());
            e.ScenarioActive(0, C("ASC-16"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioAddToHand(1, C("MEG-130"));
            e.ScenarioAddToHand(1, C("ASC-181"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Has(e, 1, "Play Switch"));
            Assert.IsTrue(Has(e, 1, "Play Air Balloon"));
        }

        // ------------------------------------------------------------------ Damage and Prizes

        [Test]
        public void Legacy_Energy_Means_One_Fewer_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jet Headbutt"), new ScriptedAgent());
            var dragapult = e.ScenarioActive(0, C("TWM-130"));
            e.ScenarioAttach(dragapult, C(Psychic));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAttach(target, C("TWM-167"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
            Assert.IsTrue(e.State.Players[1].LegacyEnergyUsed);
        }

        [Test]
        public void Spiky_Energy_Hits_Back_With_Two_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jet Headbutt"), new ScriptedAgent());
            var dragapult = e.ScenarioActive(0, C("TWM-130"));
            e.ScenarioAttach(dragapult, C(Psychic));
            var target = e.ScenarioActive(1, C(Kangaskhan));
            e.ScenarioAttach(target, C("JTG-159"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, target.DamageCounters);
            Assert.AreEqual(2, dragapult.DamageCounters);
        }

        [Test]
        public void Pouncing_Trap_Adds_100_Damage_Only_During_The_Attackers_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouncing Trap", "Attack: Pouncing Trap"), new ScriptedAgent());
            var stunfisk = e.ScenarioActive(0, C("ASC-62"));
            e.ScenarioAttach(stunfisk, C(Lightning));
            var target = e.ScenarioActive(1, C(Kangaskhan));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
            NextTurn(e); // opponent's turn 4
            NextTurn(e); // our turn 5
            Step(e);
            Assert.AreEqual(16, target.DamageCounters, "30 + 100 from the trap");
        }

        [Test]
        public void Festival_Lead_Attacks_Twice_With_Festival_Grounds()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rapid Draw", "Attack again: Rapid Draw"), new ScriptedAgent());
            var seaking = e.ScenarioActive(0, C("PRE-21"));
            e.ScenarioAttach(seaking, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetStadium(0, C("TWM-149"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count, "Rapid Draw twice (2 + 2), and the Prize card for Dreepy");
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "60 + 60 Knocks Out Dreepy");
        }

        [Test]
        public void Mew_Ex_Can_Use_Its_Benched_Pokemons_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C("30C-66"));
            e.ScenarioAttach(mew, C(Psychic));
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Has(e, 0, "Attack: Petty Grudge (Dreepy)"));
            Assert.IsFalse(Has(e, 0, "Attack: Bite (Dreepy)"), "Bite needs Fire Energy");
        }

        [Test]
        public void Gravity_Mountain_Lowers_Stage_2_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var meganium = e.ScenarioActive(0, C("MEG-8"));
            e.ScenarioEvolve(meganium, C("MEG-9"));
            e.ScenarioEvolve(meganium, C("MEG-10"));
            Assert.AreEqual(160, e.MaxHp(meganium));
            e.ScenarioSetStadium(1, C("SSP-177"));
            Assert.AreEqual(130, e.MaxHp(meganium));
        }

        [Test]
        public void Simultaneous_Knock_Outs_Next_Player_Takes_Prizes_And_Promotes_First()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Destined Fight"), new ScriptedAgent());
            var log = new EventLog();
            e.AddObserver(log);
            var annihilape = e.ScenarioActive(0, C("SSP-100"));
            e.ScenarioAttach(annihilape, C(Fighting));
            e.ScenarioAttach(annihilape, C(Psychic));
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var prizes = log.Events.Where(ev => ev.Type == GameEventType.TakePrize).ToList();
            var promotes = log.Events.Where(ev => ev.Type == GameEventType.Promote).ToList();
            Assert.AreEqual(2, prizes.Count);
            Assert.AreEqual(1, prizes[0].Player, "the player whose turn is next takes Prizes first");
            Assert.AreEqual(2, promotes.Count);
            Assert.AreEqual(1, promotes[0].Player, "the player whose turn is next promotes first");
        }

        // ------------------------------------------------------------------ Stadiums and Bench size

        [Test]
        public void Area_Zero_Allows_8_On_The_Bench_Until_The_Last_Tera_Pokemon_Leaves()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ogerpon = e.ScenarioActive(0, C("TWM-25"));
            for (int i = 0; i < 5; i++) e.ScenarioBench(0, C("MEG-54"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetStadium(0, C("SCR-131"));
            e.ScenarioAddToHand(0, C("MEG-54"));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(8, e.MaxBench(0));
            Assert.AreEqual(5, e.MaxBench(1), "no Tera Pokémon, no extra Bench");
            Assert.IsTrue(Has(e, 0, "Bench Abra"));

            e.ScenarioBench(0, C("MEG-54"));
            e.ScenarioBench(0, C("MEG-54"));
            ogerpon.DamageCounters = 21;
            e.ResolveKnockouts();
            Assert.AreEqual(4, e.State.Players[0].Bench.Count, "discard 7 down to 5, then promote 1 (Compendium, 2024-09-12)");
            Assert.AreEqual(4, e.State.Players[1].Prizes.Count);
        }

        [Test]
        public void Lumiose_City_Benches_A_Basic_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Lumiose City"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioAddToDeck(0, C(Dreepy), 1);
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetStadium(1, C("POR-77"));
            e.ScenarioSetTurn(0, 3);
            bool turnOver = e.MainPhaseStep();
            Assert.IsTrue(turnOver);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
        }
    }
}
