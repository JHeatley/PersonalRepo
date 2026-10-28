using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v65: Rebel Clash (RCL 1-209). Your rulings: Dan's Rock-Paper-Scissors has both players choose (bots at random) and
    /// replays ties; "a [T] Energy card" outside play is basic Energy only (typed Special Energy has its type only while
    /// attached), for these cards and the older ones that took typed Special Energy from the deck, discard pile or hand.
    /// </summary>
    public class RclTests : ScenarioTestBase
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
        const string WailordV = "CPA-13";
        const string CinderaceV = "SHF-18";
        const string GalarianWeezing = "SHF-42";
        const string Coalossal = "SHF-SV69";
        const string Bibarel = "PGO-60";
        const string HeatFire = "DAA-174";
        const string WashWater = "VIV-165";
        const string StoneFighting = "VIV-164";
        const string BossOrders = "MEG-114";

        const string Caterpie = "RCL-1";
        const string Metapod = "RCL-2";
        const string Butterfree = "RCL-3";
        const string Scyther = "RCL-4";
        const string Shuckle = "RCL-5";
        const string Lombre = "RCL-8";
        const string Ludicolo = "RCL-9";
        const string Masquerain = "RCL-11";
        const string Abomasnow = "RCL-13";
        const string Phantump = "RCL-14";
        const string Ninetales = "RCL-25";
        const string NinetalesV = "RCL-26";
        const string Growlithe = "RCL-27";
        const string Arcanine = "RCL-28";
        const string Magmortar = "RCL-30";
        const string Lampent = "RCL-32";
        const string Chandelure = "RCL-33";
        const string MrRime = "RCL-38";
        const string Magikarp = "RCL-39";
        const string Gyarados = "RCL-40";
        const string Wingull = "RCL-41";
        const string MiloticV = "RCL-43";
        const string Seismitoad = "RCL-46";
        const string InteleonV = "RCL-49";
        const string InteleonVmax = "RCL-50";
        const string EiscueV = "RCL-55";
        const string Electrode = "RCL-57";
        const string Electivire = "RCL-59";
        const string Heliolisk = "RCL-64";
        const string Vikavolt = "RCL-66";
        const string Toxel = "RCL-68";
        const string Toxtricity = "RCL-69";
        const string PincurchinV = "RCL-72";
        const string Clefairy = "RCL-74";
        const string Clefable = "RCL-75";
        const string Xatu = "RCL-77";
        const string Sigilyph = "RCL-80";
        const string Sandygast = "RCL-81";
        const string Palossand = "RCL-82";
        const string DragapultV = "RCL-92";
        const string DragapultVmax = "RCL-93";
        const string Whiscash = "RCL-100";
        const string Barbaracle = "RCL-104";
        const string SandacondaV = "RCL-108";
        const string Skuntank = "RCL-115";
        const string Spiritomb = "RCL-116";
        const string Garbodor = "RCL-118";
        const string Mandibuzz = "RCL-120";
        const string MalamarVmax = "RCL-122";
        const string Scizor = "RCL-128";
        const string Bronzong = "RCL-130";
        const string Probopass = "RCL-131";
        const string Durant = "RCL-132";
        const string Aegislash = "RCL-135";
        const string CopperajahVmax = "RCL-137";
        const string Zacian = "RCL-139";
        const string Snorlax = "RCL-141";
        const string Chatot = "RCL-142";
        const string Pidove = "RCL-143";
        const string Unfezant = "RCL-145";
        const string Diggersby = "RCL-147";
        const string Hawlucha = "RCL-148";
        const string Bewear = "RCL-150";
        const string BurningScarf = "RCL-155";
        const string CapaciousBucket = "RCL-156";
        const string CursedShovel = "RCL-157";
        const string Dan = "RCL-158";
        const string GalarMine = "RCL-160";
        const string Nugget = "RCL-162";
        const string Oleana = "RCL-163";
        const string ScoopUpNet = "RCL-165";
        const string HorrorPsychic = "RCL-172";
        const string SpeedLightning = "RCL-173";
        const string TwinEnergy = "RCL-174";
        const string BigCharm = "RCL-206";

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

        /// <summary>Board with each player's Active Pokémon, on player 0's third turn.</summary>
        private GameEngine Board(ScriptedAgent me, ScriptedAgent them, string mine, string theirs, out PokemonInPlay attacker, out PokemonInPlay defender)
        {
            var e = Scenario(me, them);
            attacker = e.ScenarioActive(0, C(mine));
            defender = e.ScenarioActive(1, C(theirs));
            e.ScenarioSetTurn(0, 3);
            return e;
        }

        // ================================================================== Data

        [Test]
        public void Every_Rebel_Clash_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 209; n++) Assert.IsNotNull(db.Get("RCL-" + n), "RCL " + n + " is missing");
            Assert.AreEqual("SHF-SV105", db.MainPrinting(C("RCL-17")).Key, "Rillaboom V came with Shining Fates");
            Assert.AreEqual("SHF-SV105", db.MainPrinting(C("RCL-175")).Key, "and its full art");
            Assert.AreEqual("CPA-5", db.MainPrinting(C("RCL-19")).Key, "Eldegoss V came with Champion's Path");
            Assert.AreEqual("ASR-TG2", db.MainPrinting(C("RCL-22")).Key, "Flapple was already in the pool");
            Assert.AreEqual("MEG-114", db.MainPrinting(C("RCL-154")).Key, "Boss's Orders was already in the pool");
            Assert.AreEqual("DAA-201", db.MainPrinting(C("RCL-171")).Key, "Capture Energy came with Darkness Ablaze");
            Assert.AreEqual("RCL-163", db.MainPrinting(C("RCL-202")).Key, "the gold Oleana");
            Assert.AreEqual("RCL-174", db.MainPrinting(C("RCL-209")).Key, "the gold Twin Energy");
            Assert.AreEqual("SHF-SV87", db.MainPrinting(C("RCL-205")).Key, "the Steely Spirit Galarian Perrserker");
            Assert.AreEqual("RCL-206", db.MainPrinting(C(BigCharm)).Key, "Big Charm is only a gold card here");
            Assert.AreEqual(3, C("RCL-140").Attacks[1].Cost.Length, "Zamazenta's Power Rush costs [M][M][C]");
            Assert.AreEqual(EnergyType.Psychic, C(HorrorPsychic).EnergyProvides);
            Assert.IsFalse(Formats.IsStandardCard(C(DragapultVmax)), "regulation mark D: Expanded");
        }

        // ================================================================== Grass

        [Test]
        public void Adaptive_Evolution_Lets_Caterpie_Become_Butterfree_The_Turn_It_Was_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var caterpie = e.ScenarioBench(0, C(Caterpie));
            caterpie.TurnEnteredPlay = 3;
            e.ScenarioAddToHand(0, C(Metapod));
            e.ScenarioAddToHand(0, C(Butterfree));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Metapod", caterpie.Name);
            Step(e);
            Assert.AreEqual("Butterfree", caterpie.Name);
        }

        [Test]
        public void Panic_Poison_Burns_Confuses_And_Poisons()
        {
            PokemonInPlay butterfree, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Panic Poison"), new ScriptedAgent(), Butterfree, Tauros, out butterfree, out tauros);
            Attach(e, butterfree, Grass, 1);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Burned) && tauros.Has(SpecialCondition.Confused) && tauros.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Swords_Dance_Adds_70_To_Blinding_Scythe_Next_Turn()
        {
            var me = new ScriptedAgent().Then("Attack: Swords Dance");
            PokemonInPlay scyther, wailord;
            var e = Board(me, new ScriptedAgent(), Scyther, WailordV, out scyther, out wailord);
            Attach(e, scyther, Grass, 1);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            me.Then("Attack: Blinding Scythe");
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters, "20 + 70");
        }

        [Test]
        public void Berry_Picking_Shuffles_Up_To_5_Basic_Energy_Into_The_Deck()
        {
            PokemonInPlay shuckle, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Berry Picking", "Fire Energy|Fire Energy|Fire Energy|Fire Energy|Fire Energy"), new ScriptedAgent(),
                Shuckle, Tauros, out shuckle, out tauros);
            Attach(e, shuckle, Psychic, 1);
            e.ScenarioAddToDiscard(0, C(Fire), 6);
            Step(e);
            Assert.AreEqual(25, e.State.Players[0].Deck.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Fire Energy"));
        }

        [Test]
        public void Spirited_Rushdown_Does_60_For_Each_Prize_Card_Taken()
        {
            PokemonInPlay ludicolo, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Spirited Rushdown"), new ScriptedAgent(), Ludicolo, WailordV, out ludicolo, out wailord);
            Attach(e, ludicolo, Psychic, 2);
            e.State.Players[0].Prizes.RemoveRange(0, 2);
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters);
        }

        [Test]
        public void Threatening_Pattern_Stops_Energy_From_The_Hand_To_The_Defending_Pokemon()
        {
            PokemonInPlay masquerain, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Threatening Pattern"), new ScriptedAgent(), Masquerain, Tauros, out masquerain, out tauros);
            Attach(e, masquerain, Psychic, 1);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Psychic));
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters);
            NextTurn(e);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label.StartsWith("Attach") && o.Label.Contains("(Active)")));
            Assert.IsTrue(e.GetMainActions(1).Any(o => o.Label.StartsWith("Attach") && o.Label.Contains("(Bench 1)")));
        }

        [Test]
        public void Dark_Guidance_Benches_A_Basic_From_The_Discard_Pile()
        {
            PokemonInPlay phantump, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Dark Guidance"), new ScriptedAgent(), Phantump, Tauros, out phantump, out tauros);
            Attach(e, phantump, Psychic, 1);
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            Step(e);
            Assert.AreEqual("Tauros", e.State.Players[0].Bench.Single().Name);
            Assert.IsFalse(e.State.Players[0].Discard.Any(c => c.Def.Name == "Tauros"));
        }

        // ================================================================== Fire

        [Test]
        public void Hex_Adds_90_Against_A_Special_Condition()
        {
            PokemonInPlay ninetales, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Hex"), new ScriptedAgent(), Ninetales, WailordV, out ninetales, out wailord);
            Attach(e, ninetales, Fire, 1);
            wailord.Conditions = SpecialCondition.Poisoned;
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters, "30 + 90");
        }

        [Test]
        public void Nine_Tailed_Shapeshifter_Uses_The_Defending_Pokemons_Attack_With_Its_Own_Cost()
        {
            // Compendium (2020-04-30): only Nine-Tailed Shapeshifter's own cost is needed.
            PokemonInPlay ninetales, toad;
            var e = Board(new ScriptedAgent().Then("Attack: Nine-Tailed Shapeshifter", "Resonance"), new ScriptedAgent(), NinetalesV, Seismitoad, out ninetales, out toad);
            Attach(e, ninetales, Fire, 1);
            Attach(e, ninetales, Psychic, 2);
            Step(e);
            Assert.AreEqual(12, toad.DamageCounters, "Resonance's 120");
        }

        [Test]
        public void Odor_Sleuth_Takes_A_Card_From_The_Discard_Pile_On_Heads()
        {
            PokemonInPlay growlithe, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Odor Sleuth"), new ScriptedAgent(), Growlithe, Tauros, out growlithe, out tauros);
            Attach(e, growlithe, Psychic, 1);
            e.ScenarioAddToDiscard(0, C(BossOrders), 1);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Boss's Orders"));
        }

        [Test]
        public void Burning_Scarf_Burns_The_Attacker_And_Warms_Up_Arcanine()
        {
            PokemonInPlay tauros, arcanine;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Arcanine, out tauros, out arcanine);
            Attach(e, tauros, Psychic, 2);
            Assert.AreEqual(130, e.MaxHp(arcanine));
            e.ScenarioAttachTool(arcanine, C(BurningScarf));
            Assert.AreEqual(230, e.MaxHp(arcanine), "Warming Up: +100 HP");
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Ground_Burn_Adds_100_For_Each_Energy_Card_Milled()
        {
            PokemonInPlay magmortar, dragapult;
            var e = Board(new ScriptedAgent().Then("Attack: Ground Burn"), new ScriptedAgent(), Magmortar, DragapultVmax, out magmortar, out dragapult);
            Attach(e, magmortar, Fire, 2);
            Attach(e, magmortar, Psychic, 1);
            Step(e);
            Assert.AreEqual(28, dragapult.DamageCounters, "80 + 2 x 100 (both decks are Psychic Energy)");

            PokemonInPlay m2, d2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Ground Burn"), new ScriptedAgent(), Magmortar, DragapultVmax, out m2, out d2);
            Attach(e2, m2, Fire, 2);
            Attach(e2, m2, Psychic, 1);
            e2.State.Players[0].Deck.Insert(0, e2.NewCard(0, C(Tauros)));
            e2.State.Players[1].Deck.Insert(0, e2.NewCard(1, C(Tauros)));
            Step(e2);
            Assert.AreEqual(8, d2.DamageCounters);
        }

        [Test]
        public void Top_Entry_Benches_Lampent_When_Drawn_And_Reignite_Takes_Basic_Fire_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Lampent)));
            e.ScenarioSetTurn(1, 2);
            NextTurn(e);
            Assert.IsTrue(e.State.Players[0].Bench.Any(b => b.Name == "Lampent"));

            PokemonInPlay lampent, tauros;
            var e2 = Board(new ScriptedAgent().Then("Attack: Reignite"), new ScriptedAgent(), Lampent, Tauros, out lampent, out tauros);
            Attach(e2, lampent, Fire, 1);
            e2.ScenarioAddToDiscard(0, C(HeatFire), 1);
            e2.ScenarioAddToDiscard(0, C(Fire), 1);
            Step(e2);
            Assert.AreEqual(2, lampent.Energy.Count(c => c.Def.Name == "Fire Energy"));
            Assert.IsTrue(e2.State.Players[0].Discard.Any(c => c.Def.Name == "Heat Fire Energy"), "your ruling: a typed Special Energy isn't a Fire Energy card outside play");
        }

        [Test]
        public void Top_Entry_Works_Under_Neutralizing_Gas()
        {
            // Compendium (2021-01-21): Top Entry is used before the Pokémon is in play.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(GalarianWeezing));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Lombre)));
            e.ScenarioSetTurn(1, 2);
            NextTurn(e);
            Assert.IsTrue(e.State.Players[0].Bench.Any(b => b.Name == "Lombre"));
        }

        [Test]
        public void Protective_Glow_Removes_Weakness_From_Pokemon_With_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chandelure = e.ScenarioActive(0, C(Chandelure));
            Attach(e, chandelure, Fire, 1);
            var bare = e.ScenarioBench(0, C(Tauros));
            var powered = e.ScenarioBench(0, C(Tauros));
            Attach(e, powered, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            Assert.IsNull(e.WeaknessOf(chandelure));
            Assert.IsNull(e.WeaknessOf(powered));
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(bare));
        }

        // ================================================================== Water

        [Test]
        public void Screen_Cleaner_Stops_Attack_Effects_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Soothing Scent"));
            var rime = e.ScenarioActive(0, C(MrRime));
            Attach(e, rime, Water, 1);
            var abomasnow = e.ScenarioActive(1, C(Abomasnow));
            Attach(e, abomasnow, Grass, 3);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(8, rime.DamageCounters);
            Assert.IsFalse(rime.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Big_Storm_Discards_The_Stadium()
        {
            PokemonInPlay gyarados, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Big Storm"), new ScriptedAgent(), Gyarados, WailordV, out gyarados, out wailord);
            Attach(e, gyarados, Water, 2);
            Attach(e, gyarados, Psychic, 3);
            e.ScenarioSetStadium(1, C(GalarMine));
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.AreEqual(20, wailord.DamageCounters);
        }

        [Test]
        public void Aqua_Impact_Counts_The_Current_Retreat_Cost()
        {
            PokemonInPlay milotic, copperajah;
            var e = Board(new ScriptedAgent().Then("Attack: Aqua Impact"), new ScriptedAgent(), MiloticV, CopperajahVmax, out milotic, out copperajah);
            Attach(e, milotic, Water, 1);
            Attach(e, milotic, Psychic, 2);
            Step(e);
            Assert.AreEqual(21, copperajah.DamageCounters, "10 + 4 x 50");

            PokemonInPlay m2, c2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Aqua Impact"), new ScriptedAgent(), MiloticV, CopperajahVmax, out m2, out c2);
            Attach(e2, m2, Water, 1);
            Attach(e2, m2, Psychic, 2);
            e2.ScenarioSetStadium(1, C(GalarMine));
            Step(e2);
            Assert.AreEqual(31, c2.DamageCounters, "Galar Mine: 10 + 6 x 50");
        }

        [Test]
        public void Resonance_Adds_120_Against_A_Confused_Pokemon()
        {
            PokemonInPlay toad, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Resonance"), new ScriptedAgent(), Seismitoad, WailordV, out toad, out wailord);
            Attach(e, toad, Water, 1);
            Attach(e, toad, Psychic, 3);
            wailord.Conditions = SpecialCondition.Confused;
            Step(e);
            Assert.AreEqual(24, wailord.DamageCounters);
        }

        [Test]
        public void Snipe_Shot_And_Max_Bullet_Hit_The_Bench()
        {
            PokemonInPlay inteleon, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Snipe Shot", "Tauros"), new ScriptedAgent(), InteleonV, WailordV, out inteleon, out wailord);
            Attach(e, inteleon, Water, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(4, benched.DamageCounters);
            Assert.AreEqual(0, wailord.DamageCounters);

            PokemonInPlay vmax, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Max Bullet"), new ScriptedAgent(), InteleonVmax, WailordV, out vmax, out w2);
            Attach(e2, vmax, Water, 2);
            Attach(e2, vmax, Psychic, 1);
            var b2 = e2.ScenarioBench(1, C(Tauros));
            Step(e2);
            Assert.AreEqual(16, w2.DamageCounters);
            Assert.AreEqual(6, b2.DamageCounters);
        }

        [Test]
        public void Hydro_Snipe_May_Return_An_Energy_To_The_Opponents_Hand()
        {
            PokemonInPlay vmax, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Hydro Snipe"), new ScriptedAgent(), InteleonVmax, WailordV, out vmax, out wailord);
            Attach(e, vmax, Water, 1);
            Attach(e, wailord, Psychic, 2);
            Step(e);
            Assert.AreEqual(1, wailord.Energy.Count);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Cold_Absorption_Heals_30_For_A_Basic_Water_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy"), new ScriptedAgent());
            var eiscue = e.ScenarioActive(0, C(EiscueV));
            eiscue.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, eiscue.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Wash Water Energy"), new ScriptedAgent());
            var e2iscue = e2.ScenarioActive(0, C(EiscueV));
            e2iscue.DamageCounters = 5;
            e2.ScenarioAddToHand(0, C(WashWater));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(5, e2iscue.DamageCounters, "your ruling: Wash Water Energy isn't a Water Energy card in the hand");
        }

        // ================================================================== Lightning

        [Test]
        public void Orb_Collector_Finds_Up_To_3_Energy()
        {
            PokemonInPlay electrode, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Orb Collector", "Psychic Energy|Psychic Energy|Psychic Energy"), new ScriptedAgent(), Electrode, Tauros, out electrode, out tauros);
            Attach(e, electrode, Psychic, 1);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Electrified_Bolt_Adds_90_With_A_Special_Energy_And_Twin_Energy_Gives_2()
        {
            PokemonInPlay electivire, copperajah;
            var e = Board(new ScriptedAgent().Then("Attack: Electrified Bolt"), new ScriptedAgent(), Electivire, CopperajahVmax, out electivire, out copperajah);
            Attach(e, electivire, Lightning, 2);
            Attach(e, electivire, TwinEnergy, 1);
            Assert.AreEqual(4, e.EnergyCount(electivire), "Twin Energy provides [C][C] to a Pokémon that isn't a Pokémon V");
            Step(e);
            Assert.AreEqual(18, copperajah.DamageCounters, "90 + 90");

            PokemonInPlay e2vire, c2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Electrified Bolt"), new ScriptedAgent(), Electivire, CopperajahVmax, out e2vire, out c2);
            Attach(e2, e2vire, Lightning, 3);
            Step(e2);
            Assert.AreEqual(9, c2.DamageCounters);
        }

        [Test]
        public void Twin_Energy_Gives_Only_1_To_A_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var inteleon = e.ScenarioActive(0, C(InteleonV));
            Attach(e, inteleon, TwinEnergy, 1);
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(1, e.EnergyCount(inteleon));
        }

        [Test]
        public void Eerie_Impulse_Discards_An_Energy_From_Any_Opposing_Pokemon_On_Heads()
        {
            PokemonInPlay heliolisk, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Eerie Impulse"), new ScriptedAgent(), Heliolisk, Tauros, out heliolisk, out tauros);
            Attach(e, heliolisk, Lightning, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            Attach(e, benched, Psychic, 1);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(0, benched.Energy.Count);
        }

        [Test]
        public void Powerful_Storm_Counts_All_Your_Energy()
        {
            PokemonInPlay vikavolt, copperajah;
            var e = Board(new ScriptedAgent().Then("Attack: Powerful Storm"), new ScriptedAgent(), Vikavolt, CopperajahVmax, out vikavolt, out copperajah);
            Attach(e, vikavolt, Lightning, 1);
            Attach(e, vikavolt, Psychic, 2);
            var benched = e.ScenarioBench(0, C(Tauros));
            Attach(e, benched, Psychic, 2);
            Step(e);
            Assert.AreEqual(16, copperajah.DamageCounters, "60 + 5 x 20");
        }

        [Test]
        public void Poison_Shout_Hits_Every_Opposing_Pokemon_And_Poisons_The_Active()
        {
            PokemonInPlay toxtricity, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Poison Shout"), new ScriptedAgent(), Toxtricity, Tauros, out toxtricity, out tauros);
            Attach(e, toxtricity, Lightning, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters);
            Assert.AreEqual(2, benched.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Counterattack_Kerzap_Puts_3_Counters_For_Each_Heads()
        {
            PokemonInPlay tauros, pincurchin;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, PincurchinV, out tauros, out pincurchin);
            Attach(e, tauros, Psychic, 2);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(6, tauros.DamageCounters);
        }

        [Test]
        public void Speed_Lightning_Energy_Draws_2_On_A_Lightning_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Speed Lightning Energy"), new ScriptedAgent());
            var toxel = e.ScenarioActive(0, C(Toxel));
            e.ScenarioAddToHand(0, C(SpeedLightning));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, toxel.Energy.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Speed Lightning Energy"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(SpeedLightning));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, e2.State.Players[0].Hand.Count, "Tauros isn't a Lightning Pokémon");
        }

        // ================================================================== Psychic

        [Test]
        public void Prankish_Puts_An_Energy_On_Top_Of_The_Opponents_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var clefairy = e.ScenarioActive(0, C(Clefairy));
            e.ScenarioAddToHand(0, C(Clefable));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Fire, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Clefable", clefairy.Name);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual("Fire Energy", e.State.Players[1].Deck[0].Def.Name);
        }

        [Test]
        public void Energy_Warp_Moves_Energy_To_The_Active_And_Psychic_Counts_It()
        {
            PokemonInPlay xatu, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Energy Warp"), new ScriptedAgent(), Xatu, Tauros, out xatu, out tauros);
            Attach(e, xatu, Psychic, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            Attach(e, benched, Fire, 1);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(0, benched.Energy.Count);

            PokemonInPlay x2, wailord;
            var e2 = Board(new ScriptedAgent().Then("Attack: Psychic"), new ScriptedAgent(), Xatu, WailordV, out x2, out wailord);
            Attach(e2, x2, Psychic, 1);
            Attach(e2, wailord, Water, 2);
            Step(e2);
            Assert.AreEqual(7, wailord.DamageCounters, "10 + 2 x 30");
        }

        [Test]
        public void Sigilyphs_Counterattack_Puts_3_Counters_On_The_Attacker()
        {
            PokemonInPlay tauros, sigilyph;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Sigilyph, out tauros, out sigilyph);
            Attach(e, tauros, Psychic, 2);
            Step(e);
            Assert.AreEqual(3, sigilyph.DamageCounters);
            Assert.AreEqual(3, tauros.DamageCounters);
        }

        [Test]
        public void Horror_Psychic_Energy_Hurts_The_Attacker_Of_A_Psychic_Pokemon()
        {
            PokemonInPlay tauros, sandygast;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Sandygast, out tauros, out sandygast);
            Attach(e, tauros, Psychic, 2);
            Attach(e, sandygast, HorrorPsychic, 1);
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters);

            PokemonInPlay t2, theirs;
            var e2 = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Tauros, out t2, out theirs);
            Attach(e2, t2, Psychic, 2);
            Attach(e2, theirs, HorrorPsychic, 1);
            Step(e2);
            Assert.AreEqual(0, t2.DamageCounters, "Tauros isn't a Psychic Pokémon");
        }

        [Test]
        public void Sand_Sink_Discards_3_With_Cursed_Shovel()
        {
            PokemonInPlay palossand, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Sand Sink"), new ScriptedAgent(), Palossand, Tauros, out palossand, out tauros);
            Attach(e, palossand, Psychic, 2);
            Step(e);
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);

            PokemonInPlay p2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Sand Sink"), new ScriptedAgent(), Palossand, Tauros, out p2, out t2);
            Attach(e2, p2, Psychic, 2);
            e2.ScenarioAttachTool(p2, C(CursedShovel));
            Step(e2);
            Assert.AreEqual(17, e2.State.Players[1].Deck.Count);
        }

        [Test]
        public void Jet_Assault_Adds_80_After_Moving_Up_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat", "Attack: Jet Assault"), new ScriptedAgent());
            var magikarp = e.ScenarioActive(0, C(Magikarp));
            Attach(e, magikarp, Psychic, 1);
            var dragapult = e.ScenarioBench(0, C(DragapultV));
            Attach(e, dragapult, Psychic, 2);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(dragapult, e.State.Players[0].Active);
            Step(e);
            Assert.AreEqual(14, wailord.DamageCounters, "60 + 80");
        }

        [Test]
        public void Max_Phantom_Spreads_5_Counters_On_The_Bench()
        {
            PokemonInPlay dragapult, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Max Phantom"), new ScriptedAgent(), DragapultVmax, WailordV, out dragapult, out wailord);
            Attach(e, dragapult, Psychic, 2);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(13, wailord.DamageCounters);
            Assert.AreEqual(5, e.State.Players[1].Bench.Sum(b => b.DamageCounters));
        }

        // ================================================================== Fighting

        [Test]
        public void Submerge_Keeps_A_Benched_Whiscash_Safe_From_Earthquake()
        {
            PokemonInPlay whiscash, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Earthquake"), new ScriptedAgent(), Whiscash, WailordV, out whiscash, out wailord);
            Attach(e, whiscash, Fighting, 2);
            var benchedWhiscash = e.ScenarioBench(0, C(Whiscash));
            var benchedTauros = e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(14, wailord.DamageCounters);
            Assert.AreEqual(0, benchedWhiscash.DamageCounters, "Submerge");
            Assert.AreEqual(2, benchedTauros.DamageCounters);
        }

        [Test]
        public void Hand_Press_Adds_80_With_The_Bigger_Hand()
        {
            PokemonInPlay barbaracle, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Hand Press"), new ScriptedAgent(), Barbaracle, WailordV, out barbaracle, out wailord);
            Attach(e, barbaracle, Fighting, 1);
            Attach(e, barbaracle, Psychic, 2);
            e.ScenarioAddToHand(0, C(Tauros));
            Step(e);
            Assert.AreEqual(16, wailord.DamageCounters);
        }

        [Test]
        public void Sand_Eater_Attaches_A_Basic_Fighting_Energy_From_The_Discard_Pile()
        {
            PokemonInPlay sandaconda, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Sand Eater"), new ScriptedAgent(), SandacondaV, Tauros, out sandaconda, out tauros);
            Attach(e, sandaconda, Fighting, 1);
            e.ScenarioAddToDiscard(0, C(StoneFighting), 1);
            e.ScenarioAddToDiscard(0, C(Fighting), 1);
            Step(e);
            Assert.AreEqual(2, sandaconda.Energy.Count(c => c.Def.Name == "Fighting Energy"));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Stone Fighting Energy"));
        }

        // ================================================================== Darkness

        [Test]
        public void Poison_Ring_Poisons_And_Stops_Retreat()
        {
            PokemonInPlay skuntank, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Poison Ring"), new ScriptedAgent(), Skuntank, Tauros, out skuntank, out tauros);
            Attach(e, skuntank, Darkness, 1);
            Attach(e, skuntank, Psychic, 2);
            Attach(e, tauros, Psychic, 2);
            e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Retreat"));
        }

        [Test]
        public void Splitting_Spite_And_Dripping_Grudge()
        {
            PokemonInPlay spiritomb, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Splitting Spite"), new ScriptedAgent(), Spiritomb, WailordV, out spiritomb, out wailord);
            Attach(e, spiritomb, Darkness, 1);
            Step(e);
            Assert.AreEqual(19, e.State.Players[0].Deck.Count);
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);

            PokemonInPlay s2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Dripping Grudge"), new ScriptedAgent(), Spiritomb, WailordV, out s2, out w2);
            Attach(e2, s2, Darkness, 2);
            e2.ScenarioAddToDiscard(0, C(Tauros), 3);
            Step(e2);
            Assert.AreEqual(3, w2.DamageCounters);
        }

        [Test]
        public void Poisonous_Puddle_Needs_A_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Poisonous Puddle"), new ScriptedAgent());
            e.ScenarioActive(0, C(Garbodor));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Poisonous Puddle"));
            e.ScenarioSetStadium(1, C(GalarMine));
            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Blindside_Hits_A_Damaged_Pokemon_For_100()
        {
            PokemonInPlay mandibuzz, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Blindside"), new ScriptedAgent(), Mandibuzz, Tauros, out mandibuzz, out tauros);
            Attach(e, mandibuzz, Darkness, 2);
            var benched = e.ScenarioBench(1, C(Tauros));
            benched.DamageCounters = 1;
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters);
            Assert.AreEqual(11, benched.DamageCounters);
        }

        [Test]
        public void Max_Jammer_Puts_A_Card_From_The_Opponents_Hand_On_The_Bottom_Of_Their_Deck()
        {
            PokemonInPlay malamar, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Max Jammer"), new ScriptedAgent(), MalamarVmax, WailordV, out malamar, out wailord);
            Attach(e, malamar, Darkness, 2);
            Attach(e, malamar, Psychic, 1);
            e.ScenarioAddToHand(1, C(BossOrders));
            Step(e);
            Assert.AreEqual(18, wailord.DamageCounters);
            Assert.AreEqual(0, e.State.Players[1].Hand.Count);
            Assert.AreEqual("Boss's Orders", e.State.Players[1].Deck.Last().Def.Name);
        }

        // ================================================================== Metal

        [Test]
        public void Raid_Adds_90_Right_After_Evolving_From_Scyther()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Attack: Raid"), new ScriptedAgent());
            var scyther = e.ScenarioActive(0, C(Scyther));
            Attach(e, scyther, Metal, 1);
            e.ScenarioAddToHand(0, C(Scizor));
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters, "30 + 90");
        }

        [Test]
        public void Gyro_Ball_Switches_Both_Only_If_Bronzong_Can_Switch()
        {
            // Compendium (2020-04-30): it must switch if it can; with no Benched Pokémon the opponent doesn't switch either.
            PokemonInPlay bronzong, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Gyro Ball"), new ScriptedAgent(), Bronzong, WailordV, out bronzong, out wailord);
            Attach(e, bronzong, Metal, 1);
            Attach(e, bronzong, Psychic, 2);
            var mine = e.ScenarioBench(0, C(Tauros));
            var theirs = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(mine, e.State.Players[0].Active);
            Assert.AreEqual(theirs, e.State.Players[1].Active);

            PokemonInPlay b2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Gyro Ball"), new ScriptedAgent(), Bronzong, WailordV, out b2, out w2);
            Attach(e2, b2, Metal, 1);
            Attach(e2, b2, Psychic, 2);
            e2.ScenarioBench(1, C(Tauros));
            Step(e2);
            Assert.AreEqual(b2, e2.State.Players[0].Active);
            Assert.AreEqual(w2, e2.State.Players[1].Active);
        }

        [Test]
        public void Gravitational_Drop_Adds_30_For_Each_Retreat_Cost()
        {
            PokemonInPlay probopass, copperajah;
            var e = Board(new ScriptedAgent().Then("Attack: Gravitational Drop"), new ScriptedAgent(), Probopass, CopperajahVmax, out probopass, out copperajah);
            Attach(e, probopass, Metal, 1);
            Step(e);
            Assert.AreEqual(13, copperajah.DamageCounters, "10 + 4 x 30");
        }

        [Test]
        public void Gathering_Food_Finds_A_Trainer_For_Each_Energy()
        {
            PokemonInPlay durant, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Gathering Food", "Boss's Orders|Boss's Orders|Boss's Orders"), new ScriptedAgent(), Durant, Tauros, out durant, out tauros);
            Attach(e, durant, TwinEnergy, 1);
            Attach(e, durant, Metal, 1);
            e.ScenarioAddToDeck(0, C(BossOrders), 4);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count(c => c.Def.Name == "Boss's Orders"), "Twin Energy counts as 2 Energy");
        }

        [Test]
        public void Only_One_Big_Shield_Applies()
        {
            PokemonInPlay toad, aegislash;
            var e = Board(new ScriptedAgent().Then("Attack: Resonance"), new ScriptedAgent(), Seismitoad, Aegislash, out toad, out aegislash);
            Attach(e, toad, Water, 1);
            Attach(e, toad, Psychic, 3);
            e.ScenarioBench(1, C(Aegislash));
            Step(e);
            Assert.AreEqual(9, aegislash.DamageCounters, "120 - 30");
        }

        [Test]
        public void Dangerous_Nose_Adds_100_Against_A_Basic_Pokemon()
        {
            PokemonInPlay copperajah, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Dangerous Nose"), new ScriptedAgent(), CopperajahVmax, WailordV, out copperajah, out wailord);
            Attach(e, copperajah, Metal, 2);
            Attach(e, copperajah, Psychic, 1);
            Step(e);
            Assert.AreEqual(20, wailord.DamageCounters);

            PokemonInPlay c2, gyarados;
            var e2 = Board(new ScriptedAgent().Then("Attack: Dangerous Nose"), new ScriptedAgent(), CopperajahVmax, Gyarados, out c2, out gyarados);
            Attach(e2, c2, Metal, 2);
            Attach(e2, c2, Psychic, 1);
            Step(e2);
            Assert.AreEqual(10, gyarados.DamageCounters);
        }

        [Test]
        public void Energy_Stream_Attaches_Metal_And_Smashing_Edge_Discards_2_On_Tails()
        {
            PokemonInPlay zacian, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Energy Stream"), new ScriptedAgent(), Zacian, WailordV, out zacian, out wailord);
            Attach(e, zacian, Metal, 1);
            Attach(e, zacian, Psychic, 1);
            e.ScenarioAddToDiscard(0, C(Metal), 1);
            Step(e);
            Assert.AreEqual(3, zacian.Energy.Count);

            PokemonInPlay z2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Smashing Edge"), new ScriptedAgent(), Zacian, WailordV, out z2, out w2);
            Attach(e2, z2, Metal, 2);
            Attach(e2, z2, Psychic, 1);
            e2.State.Rng = new Rng(SeedFor(false));
            Step(e2);
            Assert.AreEqual(12, w2.DamageCounters);
            Assert.AreEqual(1, z2.Energy.Count);
        }

        // ================================================================== Colorless

        [Test]
        public void Collapse_Puts_Snorlax_To_Sleep()
        {
            PokemonInPlay snorlax, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Collapse"), new ScriptedAgent(), Snorlax, WailordV, out snorlax, out wailord);
            Attach(e, snorlax, Psychic, 3);
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters);
            Assert.IsTrue(snorlax.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Lucky_Match_Takes_A_Supporter_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Chatot"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Chatot));
            e.ScenarioAddToDiscard(0, C(BossOrders), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Boss's Orders"));
        }

        [Test]
        public void Chirp_Finds_Pokemon_With_Fighting_Resistance()
        {
            PokemonInPlay pidove, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Chirp"), new ScriptedAgent(), Pidove, Tauros, out pidove, out tauros);
            Attach(e, pidove, Psychic, 1);
            e.ScenarioAddToDeck(0, C(Wingull), 1);
            e.ScenarioAddToDeck(0, C(Tauros), 1);
            Step(e);
            Assert.AreEqual("Wingull", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Daunt_Weakens_The_Defending_Pokemons_Next_Attack()
        {
            var them = new ScriptedAgent();
            PokemonInPlay unfezant, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Daunt"), them, Unfezant, Tauros, out unfezant, out tauros);
            Attach(e, unfezant, Psychic, 2);
            Attach(e, tauros, Psychic, 2);
            Step(e);
            Assert.AreEqual(5, tauros.DamageCounters);
            NextTurn(e);
            them.Then("Attack: Rage");
            Step(e);
            Assert.AreEqual(3, unfezant.DamageCounters, "Rage: 30 + 50 for its counters, then 50 less");
        }

        [Test]
        public void Mining_Rush_Discards_One_Card_At_A_Time_And_Can_Stop()
        {
            // Compendium (2020-05-07): discard one card at a time and stop whenever you like.
            PokemonInPlay diggersby, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Mining Rush", "Yes", "Yes", "No"), new ScriptedAgent(), Diggersby, WailordV, out diggersby, out wailord);
            Attach(e, diggersby, Psychic, 3);
            Step(e);
            Assert.AreEqual(18, e.State.Players[0].Deck.Count);
            Assert.AreEqual(6, wailord.DamageCounters);
        }

        [Test]
        public void Windfall_Shuffles_The_Hand_Away_And_Draws_5()
        {
            PokemonInPlay hawlucha, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Windfall"), new ScriptedAgent(), Hawlucha, Tauros, out hawlucha, out tauros);
            Attach(e, hawlucha, Psychic, 1);
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Tauros));
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(17, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Big_Throw_Discards_The_Defending_Pokemon_Without_A_Prize()
        {
            PokemonInPlay bewear, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Big Throw"), new ScriptedAgent(), Bewear, Tauros, out bewear, out tauros);
            Attach(e, bewear, Psychic, 4);
            Attach(e, tauros, Fire, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, e.State.Players[1].Discard.Count, "Tauros and its Energy");
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "not a Knock Out");
            Assert.AreEqual(benched, e.State.Players[1].Active);
        }

        // ================================================================== Trainers

        [Test]
        public void Big_Charm_And_Galar_Mine()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(BigCharm));
            var benched = e.ScenarioBench(0, C(Tauros));
            var theirs = e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(150, e.MaxHp(tauros));
            e.ScenarioSetStadium(1, C(GalarMine));
            Assert.AreEqual(4, e.RetreatCostOf(tauros));
            Assert.AreEqual(4, e.RetreatCostOf(theirs));
            Assert.AreEqual(2, e.RetreatCostOf(benched), "only the Active Pokémon");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var cinderace = e2.ScenarioActive(0, C(CinderaceV));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetStadium(1, C(GalarMine));
            Assert.AreEqual(0, e2.RetreatCostOf(cinderace), "Compendium (2020-04-30): Field Runner's no Retreat Cost wins");
        }

        [Test]
        public void Capacious_Bucket_Finds_Basic_Water_Energy_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Capacious Bucket"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(CapaciousBucket));
            e.ScenarioAddToDeck(0, C(WashWater), 2);
            e.ScenarioAddToDeck(0, C(Water), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Water Energy", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Cursed_Shovel_Mills_2_Even_Past_Reassuring_Dam()
        {
            // Compendium (2024-03-21): Bibarel's Reassuring Dam doesn't stop a Pokémon Tool.
            PokemonInPlay tauros, theirs;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Tauros, out tauros, out theirs);
            Attach(e, tauros, Psychic, 2);
            e.ScenarioBench(0, C(Bibarel));
            theirs.DamageCounters = 11;
            e.ScenarioAttachTool(theirs, C(CursedShovel));
            e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(18, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Dan_Draws_2_More_For_Winning_Rock_Paper_Scissors()
        {
            // Your ruling: both players choose; a tie is played again.
            var e = Scenario(new ScriptedAgent().Then("Play Dan", "Rock", "Paper"), new ScriptedAgent().Then("Rock", "Rock"));
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Dan));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Play Dan", "Scissors"), new ScriptedAgent().Then("Rock"));
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(Dan));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(2, e2.State.Players[0].Hand.Count);
        }

        [Test]
        public void Nugget_Is_Played_Only_Right_After_Being_Drawn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Nugget)));
            e.ScenarioSetTurn(1, 2);
            NextTurn(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Nugget"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioActive(1, C(Tauros));
            e2.State.Players[0].Deck.Insert(0, e2.NewCard(0, C(Nugget)));
            e2.State.Players[0].ItemLockTurn = 3;
            e2.ScenarioSetTurn(1, 2);
            NextTurn(e2);
            Assert.AreEqual("Nugget", e2.State.Players[0].Hand.Single().Def.Name, "Item lock stops it");
            e2.State.Players[0].ItemLockTurn = 0;
            Assert.IsFalse(Offered(e2, 0, "Play Nugget"), "it can't be played from the hand later");
        }

        [Test]
        public void Oleana_Costs_2_Cards_And_Bottoms_A_Trainer()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Oleana", "Tauros|Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Oleana));
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(BossOrders));
            e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Fire Energy", e.State.Players[0].Hand.Single().Def.Name);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Tauros"));
            Assert.AreEqual("Fire Energy", e.State.Players[1].Hand.Single().Def.Name);
            Assert.AreEqual("Boss's Orders", e.State.Players[1].Deck.Last().Def.Name);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(Oleana));
            e2.ScenarioAddToHand(0, C(Fire));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Play Oleana"), "needs 2 other cards");
        }

        [Test]
        public void Scoop_Up_Net_Returns_Every_Pokemon_Card_And_Discards_The_Rest()
        {
            // Compendium (2020-04-30): every Pokémon card in the stack goes to the hand.
            var e = Scenario(new ScriptedAgent().Then("Play Scoop Up Net", "Metapod"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var mon = e.ScenarioBench(0, C(Caterpie));
            e.ScenarioEvolve(mon, C(Metapod));
            Attach(e, mon, Grass, 1);
            e.ScenarioAttachTool(mon, C(BigCharm));
            e.ScenarioAddToHand(0, C(ScoopUpNet));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand.Select(c => c.Def.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "Caterpie", "Metapod" }, hand);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Grass Energy") && e.State.Players[0].Discard.Any(c => c.Def.Name == "Big Charm"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(InteleonV));
            e2.ScenarioAddToHand(0, C(ScoopUpNet));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Play Scoop Up Net"), "not a Pokémon V");
        }

        [Test]
        public void Tar_Generator_Takes_Only_Basic_Energy_Now()
        {
            // Your ruling (v65): typed Special Energy has its type only while attached, so older cards changed too.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Coalossal));
            e.ScenarioAddToDiscard(0, C(HeatFire), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Tar Generator"));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            Assert.IsTrue(Offered(e, 0, "Use Tar Generator"));
        }
    }
}
