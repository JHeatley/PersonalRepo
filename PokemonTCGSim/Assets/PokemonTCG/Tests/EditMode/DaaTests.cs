using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v64: Darkness Ablaze (DAA 1-201). Your rulings: discarding the last Prize cards to Billowing Smoke still wins; Pokémon
    /// Breeder's Nurturing only has its printed limits (a Pokémon that evolved earlier this turn can evolve again). Rare Fossil
    /// is in the pool now, so the Shining Fates fossil Pokémon can be played.
    /// </summary>
    public class DaaTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Tauros = "MEW-128";
        const string Potion = "POR-83";
        const string BirdKeeper = "SHF-66";
        const string Dracovish = "SHF-SV36";
        const string WailordV = "CPA-13";
        const string CaraLiss = "SHF-67";

        const string ButterfreeV = "DAA-1";
        const string Carnivine = "DAA-5";
        const string Steenee = "DAA-15";
        const string Tsareena = "DAA-16";
        const string Golisopod = "DAA-18";
        const string HoundoomV = "DAA-21";
        const string Blaziken = "DAA-24";
        const string FireDarmanitan = "DAA-28";
        const string Volcarona = "DAA-30";
        const string Talonflame = "DAA-32";
        const string Milotic = "DAA-39";
        const string Relicanth = "DAA-40";
        const string Vanilluxe = "DAA-47";
        const string Wishiwashi = "DAA-50";
        const string Toxapex = "DAA-52";
        const string Manectric = "DAA-59";
        const string VikavoltV = "DAA-60";
        const string MewV = "DAA-69";
        const string Lunatone = "DAA-72";
        const string Gothitelle = "DAA-75";
        const string Shiinotic = "DAA-80";
        const string Mimikyu = "DAA-81";
        const string Larvitar = "DAA-86";
        const string Pupitar = "DAA-87";
        const string Tyranitar = "DAA-88";
        const string Flygon = "DAA-91";
        const string Solrock = "DAA-92";
        const string RhyperiorV = "DAA-95";
        const string Passimian = "DAA-97";
        const string SlowbroV = "DAA-99";
        const string Muk = "DAA-101";
        const string Darkrai = "DAA-105";
        const string Hydreigon = "DAA-110";
        const string ScizorV = "DAA-118";
        const string Lairon = "DAA-122";
        const string Aggron = "DAA-123";
        const string Escavalier = "DAA-124";
        const string Klink = "DAA-125";
        const string Klang = "DAA-126";
        const string Klinklang = "DAA-127";
        const string StunfiskV = "DAA-128";
        const string DaaTauros = "DAA-134";
        const string Dunsparce = "DAA-137";
        const string SalamenceVmax = "DAA-144";
        const string SalamenceV = "DAA-143";
        const string Starly = "DAA-145";
        const string Fletchling = "DAA-151";
        const string Skwovet = "DAA-152";
        const string Greedent = "DAA-153";
        const string Corvisquire = "DAA-155";
        const string Corviknight = "DAA-156";
        const string BigParasol = "DAA-157";
        const string BillowingSmoke = "DAA-158";
        const string FamiliarBell = "DAA-161";
        const string GlimwoodTangle = "DAA-162";
        const string OldPC = "DAA-164";
        const string Nurturing = "DAA-166";
        const string RareFossil = "DAA-167";
        const string RoseTower = "DAA-169";
        const string Spikemuth = "DAA-170";
        const string StruggleGloves = "DAA-171";
        const string TurboPatch = "DAA-172";
        const string YellHorn = "DAA-173";
        const string HeatFire = "DAA-174";
        const string HidingDarkness = "DAA-175";
        const string PowerfulColorless = "DAA-176";
        const string CaptureEnergy = "DAA-201";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
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

        /// <summary>A seed whose first coin flips come up as given (true = heads).</summary>
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

        // ================================================================== Data

        [Test]
        public void Every_Darkness_Ablaze_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 201; n++) Assert.IsNotNull(db.Get("DAA-" + n), "DAA " + n + " is missing");
            Assert.AreEqual("CPA-79", db.MainPrinting(C("DAA-19")).Key, "Charizard V came with Champion's Path");
            Assert.AreEqual("SHF-SV107", db.MainPrinting(C("DAA-20")).Key, "Charizard VMAX came with Shining Fates");
            Assert.AreEqual("CPA-39", db.MainPrinting(C("DAA-106")).Key, "Purrloin came with Champion's Path");
            Assert.AreEqual("SHF-71", db.MainPrinting(C("DAA-196")).Key, "the rainbow Rose is Shining Fates's Rose");
            Assert.AreEqual("VIV-200", db.MainPrinting(C("DAA-160")).Key, "Cape of Toughness came with Vivid Voltage");
            Assert.AreEqual("DAA-1", db.MainPrinting(C("DAA-177")).Key, "the full-art Butterfree V");
            Assert.AreEqual("DAA-201", db.MainPrinting(C(CaptureEnergy)).Key, "Capture Energy is only a gold card here");
            Assert.AreEqual("LOR-TG22", db.MainPrinting(C("DAA-117")).Key, "Eternatus VMAX was already in the pool");
            var fossil = C(RareFossil);
            Assert.IsTrue(fossil.IsItem && fossil.IsFossil && fossil.FossilGivesPrize && !fossil.FossilPlain);
            Assert.AreEqual(70, fossil.Hp);
            Assert.IsFalse(Formats.IsStandardCard(C(Hydreigon)), "regulation mark D: Expanded");
        }

        // ================================================================== Rare Fossil

        [Test]
        public void Rare_Fossil_Goes_Onto_The_Bench_And_Dracovish_Evolves_From_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Rare Fossil"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(RareFossil));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var fossil = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Rare Fossil", fossil.Name);
            Assert.AreEqual(70, e.MaxHp(fossil));
            Assert.AreEqual(1, e.PrizesFor(fossil), "unlike an Antique Fossil, its Knock Out gives a Prize card");

            var e2 = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            var f2 = e2.ScenarioBench(0, C(RareFossil));
            e2.ScenarioAddToHand(0, C(Dracovish));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual("Dracovish", f2.Name);
        }

        [Test]
        public void Rare_Fossil_Cant_Be_Affected_By_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var fossil = e.ScenarioActive(0, C(RareFossil));
            e.ScenarioActive(1, C(Tauros));
            Assert.IsFalse(e.ApplyCondition(fossil, SpecialCondition.Paralyzed));
            Assert.AreEqual(SpecialCondition.None, fossil.Conditions);
        }

        [Test]
        public void Fossil_Search_And_Cara_Liss_Bench_Rare_Fossils()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fossil Search", "Rare Fossil|Rare Fossil"), new ScriptedAgent());
            var relicanth = e.ScenarioActive(0, C(Relicanth));
            Attach(e, relicanth, Water, 1);
            e.ScenarioAddToDeck(0, C(RareFossil), 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count(b => b.Name == "Rare Fossil"));

            var e2 = Scenario(new ScriptedAgent().Then("Play Cara Liss", "Rare Fossil"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(CaraLiss));
            e2.ScenarioAddToDeck(0, C(RareFossil), 1);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, e2.State.Players[0].Bench.Count(b => b.Name == "Rare Fossil"));
        }

        // ================================================================== Pokémon

        [Test]
        public void Dizzying_Poison_Confuses_And_Poisons()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dizzying Poison"), new ScriptedAgent());
            var butterfree = e.ScenarioActive(0, C(ButterfreeV));
            Attach(e, butterfree, Grass, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Confused) && tauros.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Giga_Drain_Heals_The_Damage_It_Did()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Giga Drain"), new ScriptedAgent());
            var carnivine = e.ScenarioActive(0, C(Carnivine));
            Attach(e, carnivine, Grass, 2);
            carnivine.DamageCounters = 5;
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, tauros.DamageCounters);
            Assert.AreEqual(1, carnivine.DamageCounters);
        }

        [Test]
        public void Power_Whip_Does_20_For_Each_Energy_To_Any_Opposing_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Power Whip", "Tauros"), new ScriptedAgent());
            var tsareena = e.ScenarioActive(0, C(Tsareena));
            Attach(e, tsareena, Grass, 3);
            e.ScenarioActive(1, C(WailordV));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, tauros.DamageCounters);
        }

        [Test]
        public void Hard_Times_Slash_Counts_The_Opponents_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hard Times Slash"), new ScriptedAgent());
            var golisopod = e.ScenarioActive(0, C(Golisopod));
            Attach(e, golisopod, Grass, 2);
            var butterfree = e.ScenarioActive(1, C(ButterfreeV));
            e.ScenarioBench(1, C(RhyperiorV));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, butterfree.DamageCounters, "30 + 2 x 50");
        }

        [Test]
        public void Vengeful_Flame_Needs_A_Hurt_Benched_Fire_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Vengeful Flame"), new ScriptedAgent());
            var houndoom = e.ScenarioActive(0, C(HoundoomV));
            Attach(e, houndoom, Fire, 3);
            var volcarona = e.ScenarioBench(0, C(Volcarona));
            volcarona.DamageCounters = 1;
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(20, wailord.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Vengeful Flame"), new ScriptedAgent());
            var h2 = e2.ScenarioActive(0, C(HoundoomV));
            Attach(e2, h2, Fire, 3);
            var tauros = e2.ScenarioBench(0, C(Tauros));
            tauros.DamageCounters = 1;
            var w2 = e2.ScenarioActive(1, C(WailordV));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(10, w2.DamageCounters, "Tauros isn't a Fire Pokémon");
        }

        [Test]
        public void Blaziken_Is_Fire_And_Fighting_And_Turbo_Drive_Powers_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Turbo Drive", "Fighting Energy"), new ScriptedAgent());
            var blaziken = e.ScenarioActive(0, C(Blaziken));
            Attach(e, blaziken, Fire, 3);
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Fighting), 1);
            e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.IsType(blaziken, EnergyType.Fire) && e.IsType(blaziken, EnergyType.Fighting));
            Step(e);
            Assert.AreEqual("Fighting Energy", tauros.Energy.Single().Def.Name);
        }

        [Test]
        public void Frozen_Heat_Discards_Water_Energy_For_60_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Frozen Heat", "Yes"), new ScriptedAgent());
            var darmanitan = e.ScenarioActive(0, C(FireDarmanitan));
            Attach(e, darmanitan, Water, 2);
            Attach(e, darmanitan, Fire, 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(17, wailord.DamageCounters, "110 + 60");
            Assert.AreEqual("Fire Energy", darmanitan.Energy.Single().Def.Name);
        }

        [Test]
        public void Cauterize_Does_Nothing_To_An_Undamaged_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Cauterize"), new ScriptedAgent());
            var volcarona = e.ScenarioActive(0, C(Volcarona));
            Attach(e, volcarona, Fire, 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, wailord.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Cauterize"), new ScriptedAgent());
            var v2 = e2.ScenarioActive(0, C(Volcarona));
            Attach(e2, v2, Fire, 1);
            var w2 = e2.ScenarioActive(1, C(WailordV));
            w2.DamageCounters = 1;
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(9, w2.DamageCounters);
        }

        [Test]
        public void Scorching_Feathers_Burns_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioActive(1, C(Talonflame));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Bright_Heal_Heals_20_From_Each_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Bright Heal"), new ScriptedAgent());
            var milotic = e.ScenarioActive(0, C(Milotic));
            milotic.DamageCounters = 3;
            var tauros = e.ScenarioBench(0, C(Tauros));
            tauros.DamageCounters = 1;
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(1, milotic.DamageCounters);
            Assert.AreEqual(0, tauros.DamageCounters);
        }

        [Test]
        public void Bitter_Cold_Paralyzes_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Bitter Cold"), new ScriptedAgent());
            e.ScenarioActive(0, C(Vanilluxe));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsTrue(tauros.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Deep_Sea_Swirl_Draws_8_And_Recover_Heals_All()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Deep Sea Swirl"), new ScriptedAgent());
            var wishiwashi = e.ScenarioActive(0, C(Wishiwashi));
            Attach(e, wishiwashi, Water, 1);
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, e.State.Players[0].Hand.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Recover"), new ScriptedAgent());
            var toxapex = e2.ScenarioActive(0, C(Toxapex));
            Attach(e2, toxapex, Water, 2);
            toxapex.DamageCounters = 9;
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, toxapex.DamageCounters);
            Assert.AreEqual(1, toxapex.Energy.Count);
        }

        [Test]
        public void Flash_Impact_Hits_One_Of_Your_Own_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flash Impact"), new ScriptedAgent());
            var manectric = e.ScenarioActive(0, C(Manectric));
            Attach(e, manectric, Lightning, 3);
            var mine = e.ScenarioBench(0, C(Tauros));
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(30, wailord.DamageCounters, "150 x 2 (Weakness)");
            Assert.AreEqual(3, mine.DamageCounters);
        }

        [Test]
        public void Paralyzing_Bolt_Stops_Item_Cards_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Paralyzing Bolt"), new ScriptedAgent());
            var vikavolt = e.ScenarioActive(0, C(VikavoltV));
            Attach(e, vikavolt, Lightning, 2);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Potion));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(5, tauros.DamageCounters);
            Assert.IsFalse(Offered(e, 1, "Play Potion"));
        }

        [Test]
        public void X_Ball_Counts_Energy_On_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: X Ball"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(MewV));
            Attach(e, mew, Psychic, 2);
            var wailord = e.ScenarioActive(1, C(WailordV));
            Attach(e, wailord, Water, 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, wailord.DamageCounters, "5 Energy x 30");
        }

        [Test]
        public void Bend_Shuffles_2_Random_Cards_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bend"), new ScriptedAgent());
            var gothitelle = e.ScenarioActive(0, C(Gothitelle));
            Attach(e, gothitelle, Psychic, 2);
            e.ScenarioActive(1, C(WailordV));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.AreEqual(22, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Fear_The_Forest_Does_60_More_With_Glimwood_Tangle()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fear the Forest"), new ScriptedAgent());
            var shiinotic = e.ScenarioActive(0, C(Shiinotic));
            Attach(e, shiinotic, Psychic, 2);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetStadium(1, C(GlimwoodTangle));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters);
        }

        [Test]
        public void Heal_Jamming_Stops_Healing_On_The_Opponents_Bench_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Mimikyu));
            var active = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Tauros));
            active.DamageCounters = 3;
            benched.DamageCounters = 3;
            Assert.AreEqual(0, e.Heal(benched, 30));
            Assert.AreEqual(30, e.Heal(active, 30));
        }

        [Test]
        public void Labyrinth_Of_Sand_Stops_Retreat_And_Desert_Geyser_Discards_Their_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Desert Geyser"), new ScriptedAgent());
            var flygon = e.ScenarioActive(0, C(Flygon));
            Attach(e, flygon, Fighting, 3);
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetStadium(1, C(RoseTower));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.Stadium, "the opponent's Stadium was discarded");
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Retreat"), "Labyrinth of Sand");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Desert Geyser"), new ScriptedAgent());
            var f2 = e2.ScenarioActive(0, C(Flygon));
            Attach(e2, f2, Fighting, 3);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetStadium(0, C(RoseTower));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsNotNull(e2.State.Stadium, "only the opponent's Stadium");
        }

        [Test]
        public void Fusion_Strike_Energy_Lets_Genesect_V_Retreat_Past_Labyrinth_Of_Sand()
        {
            // Compendium (2022-03-31): Fusion Strike Energy prevents the Ability's effect; the Retreat that starts unblocked
            // finishes even when the Fusion Strike Energy is discarded to pay for it.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var flygon = e.ScenarioActive(0, C(Flygon));
            Attach(e, flygon, Fighting, 1);
            var genesect = e.ScenarioActive(1, C("FST-185"));
            Attach(e, genesect, "FST-244", 1);
            Attach(e, genesect, Metal, 1);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(Offered(e, 1, "Retreat"), "Fusion Strike Energy");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var f2 = e2.ScenarioActive(0, C(Flygon));
            Attach(e2, f2, Fighting, 1);
            var g2 = e2.ScenarioActive(1, C("FST-185"));
            Attach(e2, g2, Metal, 2);
            e2.ScenarioBench(1, C(Tauros));
            e2.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e2, 1, "Retreat"), "no Fusion Strike Energy: Labyrinth of Sand");
        }

        [Test]
        public void Resistance_Shade_Needs_Lunatone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Throw"), new ScriptedAgent());
            var solrock = e.ScenarioActive(0, C(Solrock));
            Attach(e, solrock, Fighting, 2);
            e.ScenarioBench(0, C(Lunatone));
            var talonflame = e.ScenarioActive(1, C(Talonflame));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, talonflame.DamageCounters, "no Resistance");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Rock Throw"), new ScriptedAgent());
            var s2 = e2.ScenarioActive(0, C(Solrock));
            Attach(e2, s2, Fighting, 2);
            var t2 = e2.ScenarioActive(1, C(Talonflame));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, t2.DamageCounters, "Resistance -30");
        }

        [Test]
        public void Drill_Run_Damage_Is_Reduced_By_Darkness_Guard_Before_The_Discard()
        {
            // Compendium (2020-08-13): the reduction applies before Drill Run discards the Darkness Energy.
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Run"), new ScriptedAgent());
            var rhyperior = e.ScenarioActive(0, C(RhyperiorV));
            Attach(e, rhyperior, Fighting, 3);
            var darkrai = e.ScenarioActive(1, C(Darkrai));
            Attach(e, darkrai, Darkness, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, darkrai.DamageCounters, "80 - 20");
            Assert.AreEqual(0, darkrai.Energy.Count);
        }

        [Test]
        public void Rock_Hurl_Ignores_Resistance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Hurl"), new ScriptedAgent());
            var passimian = e.ScenarioActive(0, C(Passimian));
            Attach(e, passimian, Fighting, 3);
            var talonflame = e.ScenarioActive(1, C(Talonflame));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, talonflame.DamageCounters);
        }

        [Test]
        public void Rapid_Fire_Poison_And_Triple_Poison()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Rapid-Fire Poison"), new ScriptedAgent());
            e.ScenarioActive(0, C(SlowbroV));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Triple Poison"), new ScriptedAgent());
            var muk = e2.ScenarioActive(0, C(Muk));
            Attach(e2, muk, Darkness, 1);
            var t2 = e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(3, e2.PoisonCounters(t2));
            NextTurn(e2);
            Assert.AreEqual(3, t2.DamageCounters, "Pokémon Checkup");
        }

        [Test]
        public void Dark_Squall_Attaches_Darkness_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dark Squall", "Tauros", "Use Dark Squall", "Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hydreigon));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Darkness));
            e.ScenarioAddToHand(0, C(Darkness));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(2, tauros.Energy.Count);
        }

        [Test]
        public void Hack_Off_Discards_A_Tool_And_A_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hack Off"), new ScriptedAgent());
            var scizor = e.ScenarioActive(0, C(ScizorV));
            Attach(e, scizor, Metal, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            // Not Big Parasol: in the Active Spot it prevents all effects of the opponent's attacks, Hack Off's discards included.
            e.ScenarioAttachTool(tauros, C(StruggleGloves));
            Attach(e, tauros, PowerfulColorless, 1);
            Attach(e, tauros, Psychic, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, tauros.Tools.Count);
            Assert.AreEqual("Psychic Energy", tauros.Energy.Single().Def.Name);
        }

        [Test]
        public void Big_Parasol_Keeps_Hack_Off_From_Discarding_While_Active()
        {
            // Your ruling: in the Active Spot, Big Parasol prevents all effects of the opponent's attacks, Hack Off's discards included.
            var e = Scenario(new ScriptedAgent().Then("Attack: Hack Off"), new ScriptedAgent());
            var scizor = e.ScenarioActive(0, C(ScizorV));
            Attach(e, scizor, Metal, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(BigParasol));
            Attach(e, tauros, PowerfulColorless, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Tools.Count, "Big Parasol stays");
            Assert.AreEqual(1, tauros.Energy.Count, "the Special Energy stays");
        }

        [Test]
        public void Sturdy_Leaves_Aggron_At_10_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Rock Artillery"), new ScriptedAgent());
            var rhyperior = e.ScenarioActive(0, C(RhyperiorV));
            Attach(e, rhyperior, Fighting, 4);
            var aggron = e.ScenarioActive(1, C(Aggron));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(aggron, e.State.Players[1].Active);
            Assert.AreEqual(10, e.RemainingHp(aggron));
        }

        [Test]
        public void Fury_Cutter_Does_70_More_With_2_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fury Cutter"), new ScriptedAgent());
            var escavalier = e.ScenarioActive(0, C(Escavalier));
            Attach(e, escavalier, Metal, 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(8, wailord.DamageCounters, "10 + 70");
        }

        [Test]
        public void Clockwork_Needs_Klink_And_Klang_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Clockwork"), new ScriptedAgent());
            var klinklang = e.ScenarioActive(0, C(Klinklang));
            Attach(e, klinklang, Metal, 3);
            e.ScenarioBench(0, C(Klink));
            e.ScenarioBench(0, C(Klang));
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(20, wailord.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Clockwork"), new ScriptedAgent());
            var k2 = e2.ScenarioActive(0, C(Klinklang));
            Attach(e2, k2, Metal, 3);
            e2.ScenarioBench(0, C(Klink));
            var w2 = e2.ScenarioActive(1, C(WailordV));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, w2.DamageCounters);
        }

        [Test]
        public void Metal_Skin_And_Trapping_Bite()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trapping Bite"), new ScriptedAgent().Then("Attack: Drill Run"));
            var stunfisk = e.ScenarioActive(0, C(StunfiskV));
            Attach(e, stunfisk, Metal, 2);
            var rhyperior = e.ScenarioActive(1, C(RhyperiorV));
            Attach(e, rhyperior, Fighting, 3);
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(240, e.MaxHp(stunfisk), "200 + 2 x 20");
            Step(e);
            int bitten = rhyperior.DamageCounters; // Trapping Bite's own 60 damage
            NextTurn(e);
            Step(e);
            Assert.AreEqual(12, rhyperior.DamageCounters - bitten, "12 counters on the Attacking Pokémon");
        }

        [Test]
        public void Final_Dig_Discards_The_Top_2_Cards_Of_The_Attackers_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Horn Attack"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(DaaTauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioActive(1, C(Dunsparce));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, e.State.Players[0].Deck.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Sonic_Double_Hits_2_Of_The_Opponents_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sonic Double", "Wailord V|Fletchling"), new ScriptedAgent());
            var salamence = e.ScenarioActive(0, C(SalamenceV));
            e.ScenarioEvolve(salamence, C(SalamenceVmax));
            Attach(e, salamence, Psychic, 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            var fletchling = e.ScenarioBench(1, C(Fletchling));
            var other = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, wailord.DamageCounters);
            Assert.AreEqual(4, fletchling.DamageCounters);
            Assert.AreEqual(0, other.DamageCounters);
        }

        [Test]
        public void Smack_And_Run_Puts_Greedent_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Smack and Run"), new ScriptedAgent());
            var skwovet = e.ScenarioActive(0, C(Skwovet));
            e.ScenarioEvolve(skwovet, C(Greedent));
            Attach(e, skwovet, Psychic, 2);
            e.ScenarioBench(0, C(Tauros));
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, wailord.DamageCounters);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count, "Skwovet, Greedent and 2 Energy");
        }

        [Test]
        public void Flying_Taxi_Picks_Up_One_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes", "Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Corvisquire));
            var tauros = e.ScenarioBench(0, C(Tauros));
            Attach(e, tauros, Psychic, 1);
            e.ScenarioAddToHand(0, C(Corviknight));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Corviknight", e.State.Players[0].Active.Name);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Tauros and its Energy");
        }

        [Test]
        public void Starly_Attacks_For_Free_After_Bird_Keeper()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bird Keeper"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Starly));
            e.ScenarioAddToHand(0, C(BirdKeeper));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Keen Eye"));
            Step(e);
            Assert.AreEqual("Starly", e.State.Players[0].Active.Name);
            Assert.IsTrue(Offered(e, 0, "Attack: Keen Eye"));
        }

        // ================================================================== Trainers

        [Test]
        public void Big_Parasol_Stops_Attack_Effects_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dizzying Poison"), new ScriptedAgent());
            var butterfree = e.ScenarioActive(0, C(ButterfreeV));
            Attach(e, butterfree, Grass, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(BigParasol));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, tauros.Conditions);
        }

        [Test]
        public void Billowing_Smoke_Discards_The_Prize_Cards_And_They_Dont_Count_As_Taken()
        {
            // Compendium (2020): Prize cards discarded this way don't count as taken (Revenge Blast).
            var e = Scenario(new ScriptedAgent().Then("Attack: Horn Attack"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(DaaTauros));
            Attach(e, tauros, Psychic, 2);
            var fletchling = e.ScenarioActive(1, C(Fletchling));
            e.ScenarioAttachTool(fletchling, C(BillowingSmoke));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "no Prize card in the hand");
            Assert.AreEqual(0, e.PrizesTaken(0));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Fire Energy"));
        }

        [Test]
        public void Billowing_Smoke_On_The_Last_Prize_Card_Still_Wins()
        {
            // Your ruling: an empty Prize area wins, even though the discarded Prize card isn't "taken".
            var e = Scenario(new ScriptedAgent().Then("Attack: Horn Attack"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(DaaTauros));
            Attach(e, tauros, Psychic, 2);
            var fletchling = e.ScenarioActive(1, C(Fletchling));
            e.ScenarioAttachTool(fletchling, C(BillowingSmoke));
            e.ScenarioBench(1, C(Tauros));
            e.State.Players[0].Prizes.RemoveRange(0, 5);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.IsOver);
            Assert.AreEqual(0, e.State.Winner, e.State.EndReason);
        }

        [Test]
        public void Familiar_Bell_And_Old_PC()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Familiar Bell"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(FamiliarBell));
            e.ScenarioAddToDiscard(0, C(Fletchling), 1);
            e.ScenarioAddToDeck(0, C(Fletchling), 1);
            e.ScenarioAddToDeck(0, C(Skwovet), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Fletchling", e.State.Players[0].Hand.Single().Def.Name);

            var e2 = Scenario(new ScriptedAgent().Then("Play Old PC", "Fletchling"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(OldPC));
            e2.ScenarioAddToDiscard(0, C(Fletchling), 1);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            e2.State.Rng = new Rng(SeedFor(true, true));
            Step(e2);
            Assert.AreEqual("Fletchling", e2.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Glimwood_Tangle_Offers_A_Reflip_Of_Attack_Coins()
        {
            var me = new ScriptedAgent().Then("Attack: Knock Away", "Yes");
            var e = Scenario(me, new ScriptedAgent());
            var lairon = e.ScenarioActive(0, C(Lairon));
            Attach(e, lairon, Metal, 2);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetStadium(1, C(GlimwoodTangle));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false, true));
            Step(e);
            Assert.IsTrue(me.Seen.Any(r => r.Prompt != null && r.Prompt.Contains("Glimwood Tangle")));
            Assert.AreEqual(6, wailord.DamageCounters, "tails, then heads on the reflip: 30 + 30");
        }

        [Test]
        public void Breeders_Nurturing_Evolves_2_Pokemon_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Breeder's Nurturing", "Skwovet|Larvitar"), new ScriptedAgent());
            var skwovet = e.ScenarioActive(0, C(Skwovet));
            var larvitar = e.ScenarioBench(0, C(Larvitar));
            e.ScenarioAddToHand(0, C(Nurturing));
            e.ScenarioAddToDeck(0, C(Greedent), 1);
            e.ScenarioAddToDeck(0, C(Pupitar), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Greedent", skwovet.Name);
            Assert.AreEqual("Pupitar", larvitar.Name);
        }

        [Test]
        public void Breeders_Nurturing_Limits()
        {
            // Not during your first turn (even going second), nor on a Pokémon put into play this turn.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Skwovet));
            e.ScenarioAddToHand(1, C(Nurturing));
            e.ScenarioAddToDeck(1, C(Greedent), 1);
            e.ScenarioSetTurn(1, 2);
            Assert.IsFalse(Offered(e, 1, "Play Pokémon Breeder's Nurturing"), "first turn");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var fresh = e2.ScenarioActive(0, C(Skwovet));
            fresh.TurnEnteredPlay = 3;
            e2.ScenarioAddToHand(0, C(Nurturing));
            e2.ScenarioAddToDeck(0, C(Greedent), 1);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Play Pokémon Breeder's Nurturing"), "put into play this turn");
        }

        [Test]
        public void Breeders_Nurturing_Evolves_A_Pokemon_That_Evolved_This_Turn_Even_Under_Primal_Law()
        {
            // Your ruling: only the printed limits. Compendium (2020-08-13): evolving from the deck isn't stopped by Primal Law.
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Breeder's Nurturing"), new ScriptedAgent());
            var larvitar = e.ScenarioActive(0, C(Larvitar));
            e.ScenarioEvolve(larvitar, C(Pupitar));
            larvitar.TurnEvolved = 3;
            e.ScenarioAddToHand(0, C(Nurturing));
            e.ScenarioAddToDeck(0, C(Tyranitar), 1);
            var fossil = e.ScenarioActive(1, C(RareFossil));
            e.ScenarioEvolve(fossil, C(Dracovish));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Tyranitar", larvitar.Name);
        }

        [Test]
        public void Rose_Tower_Draws_Up_To_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Rose Tower"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(RoseTower));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.IsFalse(Offered(e, 0, "Use Rose Tower"), "once during each player's turn");
        }

        [Test]
        public void Spikemuth_Hurts_A_Pokemon_Its_Owner_Moves_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var fletchling = e.ScenarioActive(0, C(Fletchling));
            Attach(e, fletchling, Psychic, 1);
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(Spikemuth));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(fletchling));
            Assert.AreEqual(2, fletchling.DamageCounters);
        }

        [Test]
        public void Spikemuth_Doesnt_Hurt_A_Pokemon_The_Opponent_Moved()
        {
            // Compendium (2020-08-13): Steenee's Captivate moves it during the other player's turn, so no counters.
            var e = Scenario(new ScriptedAgent().Then("Attack: Captivate"), new ScriptedAgent());
            var steenee = e.ScenarioActive(0, C(Steenee));
            Attach(e, steenee, Psychic, 1);
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Fletchling));
            e.ScenarioSetStadium(0, C(Spikemuth));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Bench.Contains(theirs));
            Assert.AreEqual(0, theirs.DamageCounters);
        }

        [Test]
        public void Struggle_Gloves_Add_30_Against_The_Type_The_Holder_Is_Weak_To()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioAttachTool(tauros, C(StruggleGloves));
            var rhyperior = e.ScenarioActive(1, C(RhyperiorV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, rhyperior.DamageCounters, "Tauros is weak to Fighting: 30 + 30");
        }

        [Test]
        public void Turbo_Patch_And_Yell_Horn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Turbo Patch"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(TurboPatch));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Play Yell Horn"), new ScriptedAgent());
            var mine = e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(YellHorn));
            var theirs = e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(mine.Has(SpecialCondition.Confused) && theirs.Has(SpecialCondition.Confused));
        }

        // ================================================================== Special Energy

        [Test]
        public void Heat_Fire_And_Hiding_Darkness_Energy_Only_Help_Their_Type()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var houndoom = e.ScenarioActive(0, C(HoundoomV));
            Attach(e, houndoom, HeatFire, 1);
            var tauros = e.ScenarioBench(0, C(Tauros));
            Attach(e, tauros, HeatFire, 1);
            var darkrai = e.ScenarioBench(0, C(Darkrai));
            Attach(e, darkrai, HidingDarkness, 1);
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(230, e.MaxHp(houndoom));
            Assert.AreEqual(120, e.MaxHp(tauros));
            Assert.AreEqual(0, e.RetreatCostOf(darkrai));
            Assert.IsTrue(e.IsType(houndoom, EnergyType.Fire));
        }

        [Test]
        public void Powerful_Colorless_Energy_Adds_20_For_A_Colorless_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Horn Attack"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(DaaTauros));
            Attach(e, tauros, PowerfulColorless, 1);
            Attach(e, tauros, Psychic, 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, wailord.DamageCounters, "60 + 20");
        }

        [Test]
        public void Capture_Energy_Benches_A_Basic_Pokemon_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Capture Energy to Tauros"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(CaptureEnergy));
            e.ScenarioAddToDeck(0, C(Fletchling), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual("Fletchling", e.State.Players[0].Bench.Single().Name);
        }
    }
}
