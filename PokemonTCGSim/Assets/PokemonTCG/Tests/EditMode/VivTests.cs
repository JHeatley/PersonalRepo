using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v62: Vivid Voltage (VIV 1-203). Your ruling: while Shiftry's Shifty Substitution is on, the opponent can play any
    /// Supporter (its effect is only "Draw 3 cards").
    /// </summary>
    public class VivTests : ScenarioTestBase
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
        const string BlisseyV = "CRE-119";
        const string ZacianV = "CRZ-95";
        const string BossesOrders = "MEG-114";
        const string DittoV = "SHF-50";
        const string DittoVmax = "SHF-51";
        const string Nessa = "CRZ-136";
        const string Dewgong = "LOR-34";
        const string Kecleon = "CRE-122";
        const string Vaporeon = "BRS-TG2";
        const string Jolteon = "BRS-TG4";
        const string Eevee = "ASR-119";
        const string EmpoleonV = "BST-40";

        const string Beedrill = "VIV-3";
        const string Exeggutor = "VIV-5";
        const string Celebi = "VIV-9";
        const string Shiftry = "VIV-12";
        const string Nincada = "VIV-13";
        const string Ninjask = "VIV-14";
        const string Genesect = "VIV-16";
        const string Skiddo = "VIV-17";
        const string Gogoat = "VIV-18";
        const string ZarudeV = "VIV-22";
        const string TalonflameV = "VIV-29";
        const string Wailmer = "VIV-31";
        const string Drednaw = "VIV-39";
        const string Cramorant = "VIV-40";
        const string Arrokuda = "VIV-41";
        const string Zapdos = "VIV-48";
        const string Clefable = "VIV-64";
        const string Shedinja = "VIV-66";
        const string Dusclops = "VIV-70";
        const string Swoobat = "VIV-74";
        const string Diancie = "VIV-79";
        const string Terrakion = "VIV-92";
        const string Mudsdale = "VIV-97";
        const string CoalossalV = "VIV-98";
        const string CoalossalVmax = "VIV-99";
        const string Grapploct = "VIV-101";
        const string Zamazenta = "VIV-102";
        const string Forretress = "VIV-114";
        const string Metagross = "VIV-118";
        const string Jirachi = "VIV-119";
        const string AegislashV = "VIV-126";
        const string Rayquaza = "VIV-138";
        const string SirfetchdV = "VIV-174";
        const string Beauty = "VIV-148";
        const string CirchesterBath = "VIV-150";
        const string HerosMedal = "VIV-152";
        const string LeagueStaff = "VIV-153";
        const string MemoryCapsule = "VIV-155";
        const string MoomooCheese = "VIV-156";
        const string TelescopicSight = "VIV-160";
        const string WyndonStadium = "VIV-161";
        const string AromaticGrass = "VIV-162";
        const string CoatingMetal = "VIV-163";
        const string StoneFighting = "VIV-164";
        const string WashWater = "VIV-165";
        const string CapeOfToughness = "VIV-200";

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

        private PokemonInPlay CoalossalVmaxIn(GameEngine e, int player)
        {
            var mon = e.ScenarioActive(player, C(CoalossalV));
            e.ScenarioEvolve(mon, C(CoalossalVmax));
            return mon;
        }

        // ================================================================== Data

        [Test]
        public void Every_Vivid_Voltage_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 203; n++) Assert.IsNotNull(db.Get("VIV-" + n), "VIV " + n + " is missing");
            Assert.AreEqual("LOR-TG12", db.MainPrinting(C("VIV-20")).Key, "Orbeetle V was already in the pool");
            Assert.AreEqual("30C-CC27", db.MainPrinting(C("VIV-50")).Key, "Raikou came with the 30th Celebration Classic Collection");
            Assert.AreEqual("SHF-67", db.MainPrinting(C("VIV-149")).Key, "Cara Liss came with Shining Fates");
            Assert.AreEqual("VIV-148", db.MainPrinting(C("VIV-181")).Key, "the full-art Beauty");
            Assert.AreEqual("VIV-160", db.MainPrinting(C("VIV-203")).Key, "the gold Telescopic Sight");
            Assert.AreEqual("VIV-200", db.MainPrinting(C(CapeOfToughness)).Key, "Cape of Toughness is only a gold card");
            Assert.AreEqual("VIV-172", db.MainPrinting(C("VIV-172")).Key, "Alakazam V is only a full art");
            Assert.IsFalse(Formats.IsStandardCard(C(Shiftry)), "regulation mark D: Expanded");
            Assert.AreEqual(EnergyType.Grass, C(AromaticGrass).EnergyProvides);
            Assert.IsTrue(C(AromaticGrass).IsSpecialEnergy);
        }

        // ================================================================== Your ruling

        [Test]
        public void Shifty_Substitution_Lets_Any_Supporter_Be_Played_To_Draw_3()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Play Boss's Orders"));
            var shiftry = e.ScenarioActive(0, C(Shiftry));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(BossesOrders));
            e.ScenarioSetTurn(1, 3);
            Assert.IsTrue(Offered(e, 1, "Play Boss's Orders"), "your ruling: playable with no Benched Pokémon to switch in");
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Hand.Count, "it only draws 3 cards");
            Assert.AreEqual(shiftry, e.State.Players[0].Active);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioAddToHand(1, C(BossesOrders));
            e2.ScenarioSetTurn(1, 3);
            Assert.IsFalse(Offered(e2, 1, "Play Boss's Orders"), "without Shiftry it needs a Benched Pokémon");
        }

        [Test]
        public void Vise_Wave_Still_Paralyzes_After_Nessa_Drew_3_Under_Shifty_Substitution()
        {
            // Compendium (2020-11-12): Nessa keeps its name, so Vise Wave's "if you played Nessa" still counts.
            var e = Scenario(new ScriptedAgent().Then("Play Nessa", "Attack: Vise Wave"), new ScriptedAgent());
            var drednaw = e.ScenarioActive(0, C(Drednaw));
            Attach(e, drednaw, Water, 3);
            e.ScenarioAddToHand(0, C(Nessa));
            var shiftry = e.ScenarioActive(1, C(Shiftry));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "Nessa only drew 3 cards");
            Step(e);
            Assert.AreEqual(8, shiftry.DamageCounters);
            Assert.IsTrue(shiftry.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Swim_Freely_Stops_Fan_Tornados_Switch()
        {
            // Compendium (2022-09-08): the switch is an effect of the attack on the Active Pokémon.
            var e = Scenario(new ScriptedAgent().Then("Attack: Swim Freely"), new ScriptedAgent().Then("Attack: Fan Tornado", "Yes"));
            var dewgong = e.ScenarioActive(0, C(Dewgong));
            Attach(e, dewgong, Water, 1);
            e.ScenarioBench(0, C(Tauros));
            var shiftry = e.ScenarioActive(1, C(Shiftry));
            Attach(e, shiftry, Grass, 2);
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(1, shiftry.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(dewgong, e.State.Players[0].Active, "heads on Swim Freely: no switch");
            Assert.AreEqual(0, dewgong.DamageCounters);
        }

        [Test]
        public void Memory_Capsule_Turns_On_Thunderous_Awakening_And_Kecleon_Loses_Chromashift()
        {
            // Compendium (2022-06-02): Kecleon with Water Energy loses Chromashift to Thunderous Awakening and is Colorless.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var jolteon = e.ScenarioActive(0, C(Jolteon));
            var vaporeon = e.ScenarioActive(1, C(Vaporeon));
            var kecleon = e.ScenarioBench(1, C(Kecleon));
            Attach(e, kecleon, Water, 1);
            Assert.IsFalse(e.AbilityBlocked(vaporeon, "Torrential Awakening"), "no Memory Capsule yet");
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Water));
            e.ScenarioAttachTool(jolteon, C(MemoryCapsule));
            Assert.IsTrue(e.AbilityBlocked(vaporeon, "Torrential Awakening"), "Water Pokémon have no Abilities");
            Assert.IsTrue(e.AbilityBlocked(kecleon, "Chromashift"));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Water));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Colorless));
        }

        [Test]
        public void Thunderous_Awakening_Turns_Off_Emperors_Eyes_So_Resonant_Evolution_Works()
        {
            // Compendium (2022-10-06): Eevee's Resonant Evolution is usable once Emperor's Eyes is off.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C(Eevee));
            var jolteon = e.ScenarioBench(0, C(Jolteon));
            var empoleon = e.ScenarioActive(1, C(EmpoleonV));
            Assert.IsTrue(e.AbilityBlocked(eevee, "Resonant Evolution"), "Emperor's Eyes");
            e.ScenarioAttachTool(jolteon, C(MemoryCapsule));
            Assert.IsTrue(e.AbilityBlocked(empoleon, "Emperor's Eyes"), "Empoleon V is a Water Pokémon");
            Assert.IsFalse(e.AbilityBlocked(eevee, "Resonant Evolution"));
        }

        // ================================================================== Pokémon

        [Test]
        public void Shedinja_Cant_Be_The_Decks_Only_Basic_Pokemon()
        {
            // Compendium (2020-11-12): Shell Survival means a deck needs another Basic Pokémon.
            var onlyShedinja = Enumerable.Repeat(C(Shedinja), 4).Concat(Enumerable.Repeat(C(Psychic), 56)).ToList();
            Assert.IsTrue(Decklist.Validate(onlyShedinja, DeckFormat.Unlimited).Any(p => p.Contains("Shell Survival")));
            var withTauros = Enumerable.Repeat(C(Shedinja), 4).Concat(Enumerable.Repeat(C(Tauros), 4)).Concat(Enumerable.Repeat(C(Psychic), 52)).ToList();
            CollectionAssert.IsEmpty(Decklist.Validate(withTauros, DeckFormat.Unlimited));
        }

        [Test]
        public void Head_Crack_Ends_When_The_Pokemon_Goes_To_The_Bench()
        {
            // Compendium (2020-11-12): the effect is on the Pokémon that was Active; it ends when that Pokémon goes to the Bench.
            var e = Scenario(new ScriptedAgent().Then("Attack: Head Crack", "Tauros: Rage"), new ScriptedAgent());
            var exeggutor = e.ScenarioActive(0, C(Exeggutor));
            Attach(e, exeggutor, Grass, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            var other = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Attack: Rage"), "Head Crack picked Rage");
            e.SwitchActive(1, other);
            e.SwitchActive(1, tauros);
            Assert.IsTrue(Offered(e, 1, "Attack: Rage"), "the effect ended when Tauros went to the Bench");
        }

        [Test]
        public void Shedinja_Only_Comes_Into_Play_With_Cast_Off_Shell()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Nincada));
            e.ScenarioAddToHand(0, C(Ninjask));
            e.ScenarioAddToHand(0, C(Shedinja));
            e.ScenarioAddToDeck(0, C(Shedinja), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Bench Shedinja"), "not from the hand");
            Assert.IsTrue(e.PutIntoPlayBlocked(0, C(Shedinja)), "nor by other effects");
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count(b => b.Name == "Shedinja"), "Cast-Off Shell put it onto the Bench");
        }

        [Test]
        public void Life_Squeeze_Leaves_The_Defending_Pokemon_At_10_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Life Squeeze"), new ScriptedAgent());
            var shedinja = e.ScenarioActive(0, C(Shedinja));
            Attach(e, shedinja, Psychic, 3);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, e.RemainingHp(blissey));
        }

        [Test]
        public void Elusive_Master_Benches_Beedrill_From_A_One_Card_Hand_And_Draws_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Elusive Master"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Beedrill));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count(b => b.Name == "Beedrill"));
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(Beedrill));
            e2.ScenarioAddToHand(0, C(Grass));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Use Elusive Master"), "only as the last card in the hand");
        }

        [Test]
        public void Amazing_Bloom_Evolves_Each_Benched_Pokemon_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Amazing Bloom"), new ScriptedAgent());
            var celebi = e.ScenarioActive(0, C(Celebi));
            Attach(e, celebi, Lightning, 1);
            Attach(e, celebi, Psychic, 1);
            var nincada = e.ScenarioBench(0, C(Nincada));
            var skiddo = e.ScenarioBench(0, C(Skiddo));
            e.ScenarioAddToDeck(0, C(Ninjask), 1);
            e.ScenarioAddToDeck(0, C(Gogoat), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Ninjask", nincada.Name);
            Assert.AreEqual("Gogoat", skiddo.Name);
        }

        [Test]
        public void Jungle_Rising_Heals_The_Benched_Pokemon_That_Got_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jungle Rising"), new ScriptedAgent());
            var zarude = e.ScenarioActive(0, C(ZarudeV));
            Attach(e, zarude, Grass, 2);
            var tauros = e.ScenarioBench(0, C(Tauros));
            tauros.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, tauros.Energy.Count);
            Assert.AreEqual(0, tauros.DamageCounters);
        }

        [Test]
        public void Fast_Flight_Can_Be_Used_On_The_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var talonflame = e.ScenarioActive(0, C(TalonflameV));
            Attach(e, talonflame, Fire, 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 1);
            Assert.IsTrue(Offered(e, 0, "Attack: Fast Flight"));
        }

        [Test]
        public void Thunder_Snipe_Hits_A_Pokemon_V_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Thunder Snipe"), new ScriptedAgent());
            var zapdos = e.ScenarioActive(0, C(Zapdos));
            Attach(e, zapdos, Lightning, 3);
            e.ScenarioActive(1, C(Tauros));
            var blissey = e.ScenarioBench(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, blissey.DamageCounters);
            Assert.AreEqual(0, zapdos.Energy.Count);
        }

        [Test]
        public void Continuous_Gulp_Missile_Discards_Arrokuda_For_60_Each()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Continuous Gulp Missile", "Arrokuda|Arrokuda"), new ScriptedAgent());
            var cramorant = e.ScenarioActive(0, C(Cramorant));
            Attach(e, cramorant, Water, 2);
            e.ScenarioBench(0, C(Arrokuda));
            e.ScenarioBench(0, C(Arrokuda));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters, "2 x 60");
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Lunar_Blessing_Heals_20_And_Cures_A_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Lunar Blessing"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 1);
            tauros.DamageCounters = 3;
            tauros.Conditions |= SpecialCondition.Poisoned;
            e.ScenarioBench(0, C(Clefable));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.DamageCounters);
            Assert.IsFalse(tauros.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Sparkle_Veil_Takes_30_Off_The_Opponents_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var diancie = e.ScenarioActive(0, C(Diancie));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(0, diancie.DamageCounters);
        }

        [Test]
        public void Unaware_Stops_Attack_Effects_On_Swoobat()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Confuse Ray"));
            var swoobat = e.ScenarioActive(0, C(Swoobat));
            var dusclops = e.ScenarioActive(1, C(Dusclops));
            Attach(e, dusclops, Psychic, 1);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.IsFalse(swoobat.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Heavy_Slam_Does_30_Less_For_Each_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Slam"), new ScriptedAgent());
            var mudsdale = e.ScenarioActive(0, C(Mudsdale));
            Attach(e, mudsdale, Fighting, 3);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters, "(180 - 4 x 30) x2 Weakness");
        }

        [Test]
        public void Eruption_Shot_Attaches_A_Discarded_Energy_For_90_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Eruption Shot"), new ScriptedAgent());
            var coalossal = CoalossalVmaxIn(e, 0);
            Attach(e, coalossal, Fighting, 1);
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Fighting)));
            var zacian = e.ScenarioActive(1, C(ZacianV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, zacian.DamageCounters, "40 + 90");
            Assert.AreEqual(2, coalossal.Energy.Count);
        }

        [Test]
        public void Tentacle_Buster_Adds_120_After_Full_Nelson()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Full Nelson", "Attack: Tentacle Buster"), new ScriptedAgent());
            var grapploct = e.ScenarioActive(0, C(Grapploct));
            Attach(e, grapploct, Fighting, 2);
            var zacian = e.ScenarioActive(1, C(ZacianV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, zacian.DamageCounters);
            NextTurn(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(20, zacian.DamageCounters, "30, then 50 + 120");
        }

        [Test]
        public void Amazing_Shield_Stops_Damage_From_Pokemon_VMAX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Amazing Shield"), new ScriptedAgent().Then("Attack: Eruption Shot"));
            var zamazenta = e.ScenarioActive(0, C(Zamazenta));
            Attach(e, zamazenta, Lightning, 1);
            Attach(e, zamazenta, Fighting, 1);
            Attach(e, zamazenta, Metal, 1);
            var coalossal = CoalossalVmaxIn(e, 1);
            Attach(e, coalossal, Fighting, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, coalossal.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(0, zamazenta.DamageCounters);
        }

        [Test]
        public void Resolute_Spear_Moves_Fighting_Energy_To_Sirfetchd_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes", "Fighting Energy|Fighting Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Fighting, 1);
            var terrakion = e.ScenarioBench(0, C(Terrakion));
            Attach(e, terrakion, StoneFighting, 1);
            var sirfetchd = e.ScenarioBench(0, C(SirfetchdV));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.SwitchActive(0, sirfetchd);
            Assert.AreEqual(2, sirfetchd.Energy.Count, "the Compendium: Stone Fighting Energy provides Fighting while attached, so it moves too");
            Assert.AreEqual(0, tauros.Energy.Count + terrakion.Energy.Count);
        }

        [Test]
        public void Levitation_Field_Removes_Your_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Metagross));
            var theirs = e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(0, e.RetreatCostOf(mine));
            Assert.AreEqual(2, e.RetreatCostOf(theirs), "only your Pokémon");
        }

        [Test]
        public void Dreamy_Revelation_Takes_1_Of_The_Top_2_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dreamy Revelation", "Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Jirachi));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Tauros)));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Tauros"));
            Assert.AreEqual("Psychic Energy", e.State.Players[0].Deck[0].Def.Name, "the other card stays on top");
        }

        [Test]
        public void Amazing_Burst_Counts_Types_Of_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Amazing Burst"), new ScriptedAgent());
            var rayquaza = e.ScenarioActive(0, C(Rayquaza));
            Attach(e, rayquaza, Grass, 1);
            Attach(e, rayquaza, Lightning, 1);
            Attach(e, rayquaza, Fighting, 2);
            var ditto = e.ScenarioActive(1, C(DittoV));
            e.ScenarioEvolve(ditto, C(DittoVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(24, ditto.DamageCounters, "3 types x 80");
            Assert.AreEqual(0, rayquaza.Energy.Count);
        }

        [Test]
        public void Double_KO_Knocks_Out_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Double KO"), new ScriptedAgent());
            var forretress = e.ScenarioActive(0, C(Forretress));
            Attach(e, forretress, Metal, 4);
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Forretress"));
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Blissey V"));
        }

        // ================================================================== Trainers

        [Test]
        public void Heros_Medal_Takes_100_HP_And_1_Prize_From_A_Pokemon_VMAX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var coalossal = CoalossalVmaxIn(e, 0);
            e.ScenarioAttachTool(coalossal, C(HerosMedal));
            Assert.AreEqual(230, e.MaxHp(coalossal));
            coalossal.DamageCounters = 22;
            e.ScenarioBench(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Prizes.Count, "3 - 1 Prize cards");
        }

        [Test]
        public void Heros_Medal_Cant_Go_On_A_Pokemon_VMAX_With_100_HP_Or_Less_Left()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var coalossal = CoalossalVmaxIn(e, 0);
            coalossal.DamageCounters = 23;
            e.ScenarioAddToHand(0, C(HerosMedal));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Hero's Medal"), "100 HP left");
            coalossal.DamageCounters = 22;
            Assert.IsTrue(Offered(e, 0, "Play Hero's Medal"), "110 HP left");
        }

        [Test]
        public void Memory_Capsule_Gives_The_Attacks_Of_Previous_Evolutions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var coalossal = CoalossalVmaxIn(e, 0);
            Attach(e, coalossal, Fighting, 3);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Searing Flame"));
            e.ScenarioAttachTool(coalossal, C(MemoryCapsule));
            Assert.IsTrue(Offered(e, 0, "Attack: Searing Flame"), "Coalossal V's attack");
        }

        [Test]
        public void Circhester_Bath_Takes_20_Off_Damage_To_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var mine = e.ScenarioActive(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetStadium(0, C(CirchesterBath));
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(1, mine.DamageCounters, "30 - 20");
        }

        [Test]
        public void Wyndon_Stadium_Heals_100_When_A_Pokemon_VMAX_Evolves_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var coalossal = e.ScenarioActive(0, C(CoalossalV));
            coalossal.DamageCounters = 10;
            e.ScenarioAddToHand(0, C(CoalossalVmax));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(WyndonStadium));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Coalossal VMAX", coalossal.Name);
            Assert.AreEqual(0, coalossal.DamageCounters);
        }

        [Test]
        public void League_Staff_Draws_4_With_Wyndon_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Play League Staff"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(LeagueStaff));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(0, C(WyndonStadium));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Beauty_Can_Be_Played_On_The_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Beauty));
            e.ScenarioAddToHand(0, C(LeagueStaff));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 1);
            Assert.IsTrue(Offered(e, 0, "Play Beauty"));
            Assert.IsFalse(Offered(e, 0, "Play League Staff"), "other Supporters still wait");
        }

        [Test]
        public void Telescopic_Sight_Adds_30_To_Benched_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Linear Attack", "Blissey V"), new ScriptedAgent());
            var genesect = e.ScenarioActive(0, C(Genesect));
            Attach(e, genesect, Grass, 2);
            e.ScenarioAttachTool(genesect, C(TelescopicSight));
            e.ScenarioActive(1, C(Tauros));
            var blissey = e.ScenarioBench(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, blissey.DamageCounters, "50 + 30");
        }

        [Test]
        public void Cape_Of_Toughness_Gives_A_Basic_Pokemon_50_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(CapeOfToughness));
            var coalossal = CoalossalVmaxIn(e, 1);
            e.ScenarioAttachTool(coalossal, C(CapeOfToughness));
            Assert.AreEqual(170, e.MaxHp(tauros));
            Assert.AreEqual(330, e.MaxHp(coalossal), "not a Basic Pokémon");
        }

        [Test]
        public void Moomoo_Cheese_Heals_2_Pokemon_With_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Moomoo Cheese"), new ScriptedAgent());
            var a = e.ScenarioActive(0, C(Tauros));
            Attach(e, a, Psychic, 1);
            a.DamageCounters = 5;
            var b = e.ScenarioBench(0, C(Tauros));
            Attach(e, b, Psychic, 1);
            b.DamageCounters = 2;
            var c = e.ScenarioBench(0, C(Tauros));
            c.DamageCounters = 4;
            e.ScenarioAddToHand(0, C(MoomooCheese));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, a.DamageCounters);
            Assert.AreEqual(0, b.DamageCounters);
            Assert.AreEqual(4, c.DamageCounters, "no Energy attached");
        }

        // ================================================================== Special Energy

        [Test]
        public void Coating_Metal_Energy_Removes_A_Metal_Pokemons_Weakness()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var aegislash = e.ScenarioActive(0, C(AegislashV));
            Assert.IsNotNull(e.WeaknessOf(aegislash));
            e.ScenarioAttach(aegislash, C(CoatingMetal));
            Assert.IsNull(e.WeaknessOf(aegislash));
        }

        [Test]
        public void Stone_Fighting_Energy_Takes_20_Off()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var terrakion = e.ScenarioActive(0, C(Terrakion));
            e.ScenarioAttach(terrakion, C(StoneFighting));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(1, terrakion.DamageCounters, "30 - 20");
        }

        [Test]
        public void Wash_Water_Energy_Stops_Attack_Effects()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Confuse Ray"));
            var wailmer = e.ScenarioActive(0, C(Wailmer));
            e.ScenarioAttach(wailmer, C(WashWater));
            var dusclops = e.ScenarioActive(1, C(Dusclops));
            Attach(e, dusclops, Psychic, 1);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.IsFalse(wailmer.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Aromatic_Grass_Energy_Keeps_A_Grass_Pokemon_Free_Of_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zarude = e.ScenarioActive(0, C(ZarudeV));
            e.ScenarioAttach(zarude, C(AromaticGrass));
            e.ScenarioActive(1, C(Tauros));
            e.ApplyCondition(zarude, SpecialCondition.Poisoned);
            Assert.IsFalse(zarude.Has(SpecialCondition.Poisoned));
        }
    }
}
