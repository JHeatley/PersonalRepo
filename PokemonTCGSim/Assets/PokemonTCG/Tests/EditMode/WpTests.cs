using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v48: Wizards Black Star Promos (WP), with your Baby, Chain Reaction, marker, Pokémon Tower and Aurora Veil rulings.</summary>
    public class WpTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";

        const string GrowlPikachu = "WP-1";
        const string LightScreenElectabuzz = "WP-2";
        const string DevolutionMew = "WP-8";
        const string Eevee = "WP-11";
        const string TelekinesisMewtwo = "WP-12";
        const string Venusaur = "WP-13";
        const string CoolPorygon = "WP-15";
        const string ComputerError = "WP-16";
        const string SabrinasAbra = "WP-19";
        const string Psyduck = "WP-20";
        const string BirthdayPikachu = "WP-24";
        const string Togepi = "WP-30";
        const string Cleffa = "WP-31";
        const string Smeargle = "WP-32";
        const string Scizor = "WP-33";
        const string Entei = "WP-34";
        const string Pichu = "WP-35";
        const string Igglybuff = "WP-36";
        const string PokemonCenter = "WP-40";
        const string PokemonTower = "WP-42";
        const string RodElectabuzz = "WP-46";
        const string ShieldMew = "WP-47";
        const string VeilArticuno = "WP-48";
        const string Snorlax = "WP-49";
        const string Suicune = "WP-53";
        const string Dragonite = "WP-5";

        const string Tauros = "MEW-128";
        const string Pidgey = "OBF-162";
        const string Pidgeotto = "OBF-163";
        const string ModernPikachu = "MEW-25";
        const string ModernCleffa = "SVP-37";
        const string Clefairy = "MEW-35";
        const string Vaporeon = "PRE-22";
        const string Flareon = "PRE-13";
        const string Potion = "POR-83";
        const string EnergyRetrieval = "SVI-171";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private sealed class FlipLog : IGameObserver
        {
            public readonly List<string> Flips = new List<string>();

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                if (ev.Type == GameEventType.CoinFlip) Flips.Add(ev.Text);
            }
        }

        /// <summary>Like Scenario, with the coin flips set by the seed.</summary>
        private GameEngine Seeded(int seed, ScriptedAgent p1, ScriptedAgent p2)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), p1, p2);
            e.BeginScenario(seed);
            e.ScenarioAddPrizes(0, C(Fire), 6);
            e.ScenarioAddPrizes(1, C(Fire), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            return e;
        }

        private PokemonInPlay EvolvedPidgeotto(GameEngine e, int player)
        {
            var bird = e.ScenarioActive(player, C(Pidgey));
            e.ScenarioEvolve(bird, C(Pidgeotto));
            e.ScenarioAttach(bird, C(Psychic));
            e.ScenarioAttach(bird, C(Psychic));
            return bird;
        }

        // ================================================================== Data

        [Test]
        public void Every_Wizards_Promo_Is_In_The_Pool_And_Unlimited_Only()
        {
            for (int n = 1; n <= 53; n++)
            {
                var d = db.Get("WP-" + n);
                Assert.IsNotNull(d, "WP " + n + " is missing");
                // Flying Pikachu (WP 25) and Surfing Pikachu (WP 28) were reprinted later, so they're Expanded legal (your ruling).
                if (n == 25 || n == 28) Assert.IsFalse(d.UnlimitedOnly, d.Name + " was reprinted: Expanded");
                else Assert.IsTrue(d.UnlimitedOnly, d.Name + " came out before Black & White");
            }
            Assert.AreEqual(DevolutionMew, db.MainPrinting(C("WP-9")).Key, "the second Mew print");
            Assert.AreEqual("WP-3", db.MainPrinting(C("WP-14")).Key, "the second Mewtwo print");
            Assert.AreEqual(PokemonCenter, db.MainPrinting(C("BS-85")).Key, "Base Set's Pokémon Center has the same text");
            Assert.IsTrue(C(Pichu).IsBaby && C(Cleffa).IsBaby && C(Igglybuff).IsBaby);
            Assert.IsFalse(C(ModernCleffa).IsBaby, "later Cleffa have no Baby Rule");
            Assert.AreEqual(AbilityDef.PokeBody, C(Suicune).Abilities[0].Kind);
            Assert.AreEqual(AbilityDef.PokemonPower, C(Entei).Abilities[0].Kind);
        }

        // ================================================================== Baby Pokémon (your rulings)

        [Test]
        public void Baby_Rule_Flip_Comes_First_And_Tails_Ends_The_Turn()
        {
            var outcomes = new HashSet<bool>();
            for (int seed = 0; seed < 24; seed++)
            {
                var e = Seeded(seed, new ScriptedAgent().Then("Attack: Gust"), new ScriptedAgent());
                var pidgey = e.ScenarioActive(0, C(Pidgey));
                e.ScenarioAttach(pidgey, C(Psychic));
                var pichu = e.ScenarioActive(1, C(Pichu));
                e.ScenarioBench(1, C(Tauros));
                e.ScenarioSetTurn(0, 3);
                var log = new FlipLog();
                e.AddObserver(log);
                Assert.IsTrue(e.MainPhaseStep(), "attacking ends the turn either way");
                Assert.IsTrue(log.Flips.Count > 0 && log.Flips[0].Contains("Baby Rule"), "the Baby Rule flip comes before anything else");
                bool attacked = pichu.DamageCounters > 0;
                Assert.AreEqual(log.Flips[0].Contains("heads"), attacked, "heads: the attack happens; tails: it doesn't");
                outcomes.Add(attacked);
            }
            CollectionAssert.AreEquivalent(new[] { true, false }, outcomes.ToArray());
        }

        [Test]
        public void A_Baby_Evolves_Into_Any_Card_Of_Its_Basics_Name_But_Later_Babies_Do_Not()
        {
            var e = Scenario(new ScriptedAgent().Then("into Pikachu"), new ScriptedAgent());
            var pichu = e.ScenarioActive(0, C(Pichu));
            e.ScenarioBench(0, C(ModernCleffa));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(ModernPikachu));
            e.ScenarioAddToHand(0, C(Clefairy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Evolve Pichu"), "any Pikachu, a modern one included (your ruling)");
            Assert.IsFalse(Offered(e, 0, "Evolve Cleffa"), "a Cleffa without the Baby Rule doesn't evolve");
            Assert.IsTrue(Offered(e, 0, "Bench Pikachu"), "Pikachu can still be played as a Basic");
            Step(e);
            Assert.AreEqual("Pikachu", pichu.Name);
            Assert.AreEqual(2, pichu.Stack.Count);
        }

        [Test]
        public void Lets_Play_Puts_A_Baby_Pokemon_Onto_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Let's Play!"), new ScriptedAgent());
            var pichu = e.ScenarioActive(0, C(Pichu));
            e.ScenarioAttach(pichu, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(ModernCleffa), 1);
            e.ScenarioAddToDeck(0, C(Cleffa), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
            Assert.IsTrue(e.State.Players[0].Bench[0].Def.IsBaby, "only Baby Pokémon cards can be chosen");
        }

        // ================================================================== Your other rulings

        [Test]
        public void Birthday_Surprise_Flips_Only_On_Your_Birthday()
        {
            Assert.IsTrue(GameEngine.IsBirthday(1009, new DateTime(2026, 10, 9)));
            Assert.IsFalse(GameEngine.IsBirthday(1009, new DateTime(2026, 10, 10)));
            Assert.IsFalse(GameEngine.IsBirthday(0, new DateTime(2026, 10, 9)), "not set");
            var onBirthday = new HashSet<int>();
            for (int seed = 0; seed < 20; seed++)
            {
                foreach (bool birthday in new[] { false, true })
                {
                    var e = Seeded(seed, new ScriptedAgent().Then("Attack: Birthday Surprise"), new ScriptedAgent());
                    var pikachu = e.ScenarioActive(0, C(BirthdayPikachu));
                    e.ScenarioAttach(pikachu, C(Lightning));
                    e.ScenarioAttach(pikachu, C(Lightning));
                    var tauros = e.ScenarioActive(1, C(Tauros));
                    e.State.Players[0].BirthdayToday = birthday;
                    e.ScenarioSetTurn(0, 3);
                    var log = new FlipLog();
                    e.AddObserver(log);
                    Step(e);
                    if (birthday)
                    {
                        onBirthday.Add(tauros.DamageCounters);
                    }
                    else
                    {
                        Assert.AreEqual(3, tauros.DamageCounters, "not your birthday: 30");
                        Assert.AreEqual(0, log.Flips.Count, "and no coin flip");
                    }
                }
            }
            CollectionAssert.AreEquivalent(new[] { 3, 8 }, onBirthday.ToArray(), "your birthday: 30, or 80 on heads");
        }

        [Test]
        public void Lightning_Bolt_Hits_Every_Marked_Pokemon_Yours_Too_With_Weakness_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lightning Bolt"), new ScriptedAgent());
            var electabuzz = e.ScenarioActive(0, C(RodElectabuzz));
            e.ScenarioAttach(electabuzz, C(Lightning));
            e.ScenarioAttach(electabuzz, C(Lightning));
            var mine = e.ScenarioBench(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            var marked = e.ScenarioBench(1, C(Pidgey));
            var unmarked = e.ScenarioBench(1, C(Pidgey));
            tauros.Markers[PokemonInPlay.LightningRodMarker] = 1;
            marked.Markers[PokemonInPlay.LightningRodMarker] = 1;
            mine.Markers[PokemonInPlay.LightningRodMarker] = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters, "a marked Defending Pokémon takes 10 + 20 (your ruling)");
            Assert.AreEqual(4, marked.DamageCounters, "20 with Weakness applied on the Bench (Pidgey is weak to Lightning)");
            Assert.AreEqual(0, unmarked.DamageCounters);
            Assert.AreEqual(2, mine.DamageCounters, "your own marked Pokémon is hit too (your ruling)");
        }

        [Test]
        public void Lightning_Rod_Markers_Stay_Through_Switching_And_Go_When_The_Pokemon_Evolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lightning Rod", "Pidgey"), new ScriptedAgent());
            var electabuzz = e.ScenarioActive(0, C(RodElectabuzz));
            e.ScenarioAttach(electabuzz, C(Lightning));
            e.ScenarioActive(1, C(Tauros));
            var pidgey = e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(pidgey.Markers.ContainsKey(PokemonInPlay.LightningRodMarker));
            Assert.IsTrue(PlayerView.Create(e, 0).FindPokemon(pidgey.Uid).Notes.Contains("Lightning Rod marker"), "shown on the board");
            e.SwitchActive(1, pidgey);
            Assert.IsTrue(pidgey.Markers.ContainsKey(PokemonInPlay.LightningRodMarker), "switching keeps it");
            var card = e.ScenarioAddToHand(1, C(Pidgeotto));
            e.State.Players[1].Hand.Remove(card);
            e.EvolvePokemon(pidgey, card);
            Assert.AreEqual(0, pidgey.Markers.Count, "evolving removes it (your ruling)");
        }

        [Test]
        public void Paint_Changes_The_Type_Until_The_Pokemon_Evolves_Or_Devolves()
        {
            var outcomes = new HashSet<bool>();
            for (int seed = 0; seed < 20; seed++)
            {
                var e = Seeded(seed, new ScriptedAgent().Then("Attack: Paint", "Water"), new ScriptedAgent());
                var smeargle = e.ScenarioActive(0, C(Smeargle));
                e.ScenarioAttach(smeargle, C(Psychic));
                var tauros = e.ScenarioActive(1, C(Tauros));
                e.ScenarioSetTurn(0, 3);
                var log = new FlipLog();
                e.AddObserver(log);
                Step(e);
                bool painted = tauros.Markers.ContainsKey(PokemonInPlay.ColoringMarker);
                Assert.AreEqual(log.Flips[0].Contains("heads"), painted);
                if (painted) CollectionAssert.AreEqual(new[] { EnergyType.Water }, e.TypesOf(tauros).ToArray());
                outcomes.Add(painted);
            }
            CollectionAssert.AreEquivalent(new[] { true, false }, outcomes.ToArray());

            var d = Scenario(new ScriptedAgent().Then("Attack: Devolution Beam"), new ScriptedAgent());
            var mew = d.ScenarioActive(0, C(DevolutionMew));
            d.ScenarioAttach(mew, C(Psychic));
            d.ScenarioAttach(mew, C(Psychic));
            var bird = d.ScenarioActive(1, C(Pidgey));
            d.ScenarioEvolve(bird, C(Pidgeotto));
            bird.Markers[PokemonInPlay.ColoringMarker] = (int)EnergyType.Fire;
            d.ApplyCondition(bird, SpecialCondition.Confused);
            d.ScenarioSetTurn(0, 3);
            Step(d);
            Assert.AreEqual("Pidgey", bird.Name, "Devolution Beam");
            Assert.IsTrue(d.State.Players[1].Hand.Any(c => c.Def.Name == "Pidgeotto"), "the Evolution card goes to its owner's hand");
            Assert.IsFalse(bird.Has(SpecialCondition.Confused));
            Assert.AreEqual(0, bird.Markers.Count, "devolving removes markers too (your ruling)");
        }

        [Test]
        public void Pokemon_Tower_Stops_Attacks_Trainers_And_Pokemon_Powers_But_Not_Abilities_Or_Poke_Bodies()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Energy Retrieval"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(PokemonTower));
            e.ScenarioAddToHand(0, C(EnergyRetrieval));
            e.ScenarioAddToDiscard(0, C(Psychic), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count(c => c.Def.IsEnergy), "Energy Retrieval's cards stay in the discard pile");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.IsBasicEnergy));

            var dragonite = e.ScenarioBench(0, C(Dragonite));
            var suicune = e.ScenarioBench(0, C(Suicune));
            e.PushSource(EffectSourceKind.Attack, 0, null);
            Assert.IsTrue(e.DiscardToHandBlocked(0), "attacks");
            e.PopSource();
            e.PushSource(EffectSourceKind.PokemonPower, 0, dragonite);
            Assert.IsTrue(e.DiscardToHandBlocked(0), "Pokémon Powers");
            e.PopSource();
            e.PushSource(EffectSourceKind.PokemonPower, 0, suicune);
            Assert.IsFalse(e.DiscardToHandBlocked(0), "Poké-Bodies aren't Pokémon Powers (your ruling)");
            e.PopSource();
            e.PushSource(EffectSourceKind.Ability, 0, null);
            Assert.IsFalse(e.DiscardToHandBlocked(0), "nor are Abilities (your ruling)");
            e.PopSource();
        }

        [Test]
        public void Chain_Reaction_Evolves_Eevee_When_Any_Pokemon_Evolves_And_Sets_Off_The_Next_Eevee()
        {
            var me = new ScriptedAgent().Then("Yes", "Vaporeon", "Yes", "Flareon");
            var e = Scenario(me, new ScriptedAgent().Then("into Pidgeotto"));
            e.ScenarioActive(0, C(Tauros));
            var first = e.ScenarioBench(0, C(Eevee));
            var second = e.ScenarioBench(0, C(Eevee));
            e.ScenarioAddToDeck(0, C(Vaporeon), 1);
            e.ScenarioAddToDeck(0, C(Flareon), 1);
            e.ScenarioActive(1, C(Pidgey));
            e.ScenarioAddToHand(1, C(Pidgeotto));
            e.ScenarioSetTurn(1, 4);
            first.TurnEnteredPlay = 4; // just played: no timing limits (your ruling)
            Step(e);
            Assert.AreEqual("Vaporeon", first.Name, "the opponent's Pokémon evolving sets it off");
            Assert.AreEqual("Flareon", second.Name, "and that Eevee evolving sets off the other one");
        }

        // ================================================================== Pokémon Powers and Poké-Bodies

        [Test]
        public void Bolt_Flips_To_Shuffle_Entei_Away_Unless_It_Was_Knocked_Out_Or_Paralyzed()
        {
            var outcomes = new HashSet<bool>();
            for (int seed = 0; seed < 24; seed++)
            {
                var e = Seeded(seed, new ScriptedAgent().Then("Attack: Gust"), new ScriptedAgent());
                var pidgey = e.ScenarioActive(0, C(Pidgey));
                e.ScenarioAttach(pidgey, C(Psychic));
                var entei = e.ScenarioActive(1, C(Entei));
                e.ScenarioBench(1, C(Tauros));
                e.ScenarioSetTurn(0, 3);
                var log = new FlipLog();
                e.AddObserver(log);
                Step(e);
                bool gone = e.State.FindPokemon(entei.Uid) == null;
                Assert.AreEqual(1, log.Flips.Count);
                Assert.AreEqual(log.Flips[0].Contains("heads"), gone);
                if (gone)
                {
                    Assert.IsTrue(e.State.Players[1].Deck.Any(c => c.Def.Name == "Entei"), "shuffled into the deck");
                    Assert.AreEqual("Tauros", e.State.Players[1].Active.Name, "a new Active Pokémon comes in");
                }
                outcomes.Add(gone);
            }
            CollectionAssert.AreEquivalent(new[] { true, false }, outcomes.ToArray());

            var p = Scenario(new ScriptedAgent().Then("Attack: Gust"), new ScriptedAgent());
            var attacker = p.ScenarioActive(0, C(Pidgey));
            p.ScenarioAttach(attacker, C(Psychic));
            var paralyzed = p.ScenarioActive(1, C(Entei));
            p.ApplyCondition(paralyzed, SpecialCondition.Paralyzed);
            p.ScenarioSetTurn(0, 3);
            var flips = new FlipLog();
            p.AddObserver(flips);
            Step(p);
            Assert.AreEqual(0, flips.Flips.Count, "already Paralyzed when damaged: no Bolt");
            Assert.AreEqual(2, paralyzed.DamageCounters);
        }

        [Test]
        public void Neutral_Shield_Stops_Evolved_Pokemon_Unless_Mew_Is_Asleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wing Attack"), new ScriptedAgent());
            EvolvedPidgeotto(e, 0);
            var mew = e.ScenarioActive(1, C(ShieldMew));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, mew.DamageCounters, "an Evolved Pokémon's attack does nothing to it");

            var b = Scenario(new ScriptedAgent().Then("Attack: Gust"), new ScriptedAgent());
            var pidgey = b.ScenarioActive(0, C(Pidgey));
            b.ScenarioAttach(pidgey, C(Psychic));
            var mew2 = b.ScenarioActive(1, C(ShieldMew));
            b.ScenarioSetTurn(0, 3);
            Step(b);
            Assert.AreEqual(2, mew2.DamageCounters, "a Basic Pokémon's attack gets through");

            var s = Scenario(new ScriptedAgent().Then("Attack: Wing Attack"), new ScriptedAgent());
            EvolvedPidgeotto(s, 0);
            var sleepy = s.ScenarioActive(1, C(ShieldMew));
            s.ScenarioBench(1, C(Tauros));
            s.ApplyCondition(sleepy, SpecialCondition.Asleep);
            s.ScenarioSetTurn(0, 3);
            Step(s);
            Assert.IsNull(s.State.FindPokemon(sleepy.Uid), "Asleep: no Neutral Shield, so 40 Knocks it Out");
        }

        [Test]
        public void Aurora_Veil_Protects_The_Bench_While_Articuno_Is_Active_Without_A_Special_Condition()
        {
            foreach (bool paralyzed in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Telekinesis", "Tauros"), new ScriptedAgent());
                var mewtwo = e.ScenarioActive(0, C(TelekinesisMewtwo));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(mewtwo, C(Psychic));
                var articuno = e.ScenarioActive(1, C(VeilArticuno));
                var benched = e.ScenarioBench(1, C(Tauros));
                if (paralyzed) e.ApplyCondition(articuno, SpecialCondition.Paralyzed);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(paralyzed ? 3 : 0, benched.DamageCounters, paralyzed ? "a Special Condition turns it off" : "the Bench is protected");
            }
        }

        [Test]
        public void Guard_Stops_The_Defending_Pokemon_Retreating_Unless_Snorlax_Has_A_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            var tauros = e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Retreat"));
            e.ApplyCondition(snorlax, SpecialCondition.Asleep);
            Assert.IsTrue(Offered(e, 1, "Retreat"));
        }

        [Test]
        public void Solar_Power_Cures_Both_Active_Pokemon_But_Not_Burns()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Solar Power"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Venusaur));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ApplyCondition(mine, SpecialCondition.Asleep);
            e.ApplyCondition(mine, SpecialCondition.Poisoned);
            e.ApplyCondition(theirs, SpecialCondition.Confused);
            e.ApplyCondition(theirs, SpecialCondition.Burned);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, mine.Conditions);
            Assert.AreEqual(SpecialCondition.Burned, theirs.Conditions);
        }

        [Test]
        public void Pure_Body_Discards_An_Energy_When_Water_Is_Attached_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("to Suicune"), new ScriptedAgent());
            var suicune = e.ScenarioActive(0, C(Suicune));
            e.ScenarioAttach(suicune, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, suicune.Energy.Count);
        }

        // ================================================================== Attacks

        [Test]
        public void Growl_Snivel_And_Light_Screen_Cut_The_Damage_From_The_Next_Attack()
        {
            var cases = new[] { GrowlPikachu + "|Growl|3", Togepi + "|Snivel|2", LightScreenElectabuzz + "|Light Screen|2" };
            foreach (var row in cases)
            {
                var parts = row.Split('|');
                var e = Scenario(new ScriptedAgent().Then("Attack: " + parts[1]), new ScriptedAgent().Then("Attack: Wing Attack"));
                var defender = e.ScenarioActive(0, C(parts[0]));
                e.ScenarioAttach(defender, C(Lightning));
                EvolvedPidgeotto(e, 1);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                NextTurn(e);
                Step(e);
                Assert.AreEqual(int.Parse(parts[2]), defender.DamageCounters, parts[1] + " against a 40-damage attack");
            }
        }

        [Test]
        public void Texture_Magic_Changes_Its_Resistance_And_The_Defending_Pokemons_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Texture Magic", "Yes", "Fire", "Yes", "Lightning"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(CoolPorygon));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(porygon, C(Psychic));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(EnergyType.Fire, e.ResistanceOf(porygon));
            Assert.AreEqual(EnergyType.Lightning, e.WeaknessOf(tauros));
        }

        [Test]
        public void Headache_Stops_Trainer_Cards_For_One_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Headache"), new ScriptedAgent());
            var psyduck = e.ScenarioActive(0, C(Psyduck));
            e.ScenarioAttach(psyduck, C(Psychic));
            var tauros = e.ScenarioActive(1, C(Tauros));
            tauros.DamageCounters = 2;
            e.ScenarioAddToHand(1, C(Potion));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Play Potion"));
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(Offered(e, 1, "Play Potion"), "only during their next turn");
        }

        [Test]
        public void Synchronize_Needs_The_Same_Number_Of_Energy_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var abra = e.ScenarioActive(0, C(SabrinasAbra));
            e.ScenarioAttach(abra, C(Psychic));
            e.ScenarioAttach(abra, C(Psychic));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Synchronize"));
            e.ScenarioAttach(tauros, C(Psychic));
            Assert.IsTrue(Offered(e, 0, "Attack: Synchronize"));
        }

        [Test]
        public void Leer_Stops_The_Defending_Pokemon_Attacking_On_Heads()
        {
            var outcomes = new HashSet<bool>();
            for (int seed = 0; seed < 20; seed++)
            {
                var e = Seeded(seed, new ScriptedAgent().Then("Attack: Leer"), new ScriptedAgent());
                var scizor = e.ScenarioActive(0, C(Scizor));
                e.ScenarioAttach(scizor, C(Psychic));
                var tauros = e.ScenarioActive(1, C(Tauros));
                e.ScenarioAttach(tauros, C(Psychic));
                e.ScenarioAttach(tauros, C(Psychic));
                e.ScenarioSetTurn(0, 3);
                var log = new FlipLog();
                e.AddObserver(log);
                Step(e);
                bool heads = log.Flips[0].Contains("heads");
                NextTurn(e);
                Assert.AreEqual(!heads, Offered(e, 1, "Attack:"));
                outcomes.Add(heads);
            }
            CollectionAssert.AreEquivalent(new[] { true, false }, outcomes.ToArray());
        }

        // ================================================================== Trainers

        [Test]
        public void Computer_Error_Lets_Both_Players_Draw_Up_To_5_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Computer Error", "5"), new ScriptedAgent().Then("3"));
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(ComputerError));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "your turn ends");
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(3, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Pokemon_Center_Heals_Your_Damaged_Pokemon_And_Discards_Their_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Center"), new ScriptedAgent());
            var hurt = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttach(hurt, C(Psychic));
            e.ScenarioAttach(hurt, C(Psychic));
            var healthy = e.ScenarioBench(0, C(Pidgey));
            e.ScenarioAttach(healthy, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(PokemonCenter));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Pokémon Center"), "nothing to heal");
            hurt.DamageCounters = 5;
            Step(e);
            Assert.AreEqual(0, hurt.DamageCounters);
            Assert.AreEqual(0, hurt.Energy.Count);
            Assert.AreEqual(1, healthy.Energy.Count, "an undamaged Pokémon keeps its Energy");
        }
    }
}
