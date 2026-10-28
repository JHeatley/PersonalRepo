using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v66: Sword &amp; Shield (SSH 1-216). Rulings used: Aurora Energy's discard is a cost of attaching it from the hand, however
    /// it's attached from there (Compendium 2020-02-06), and Goodra's Slimy Room flips before that discard (your ruling, v66);
    /// Toxicroak's More Poison stacks (2020-02-06); Lucky Egg is played as written (2024-12-12); Cinderace (36) has a Retreat
    /// Cost of 2 (2020-12-23); Carry Off works as Bye-Bye Flight (your ruling, v38); "a [T] Energy card" outside play is basic
    /// Energy only (your ruling, v65).
    /// </summary>
    public class SshTests : ScenarioTestBase
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
        const string OtherDurant = "RCL-132";
        const string HeatFire = "DAA-174";
        const string WashWater = "VIV-165";
        const string StoneFighting = "VIV-164";
        const string SpeedLightning = "RCL-173";
        const string BossOrders = "MEG-114";
        const string CheerUpEevee = "OBF-166";
        const string Goodra = "FST-197";

        const string CelebiV = "SSH-1";
        const string Roserade = "SSH-4";
        const string Whimsicott = "SSH-6";
        const string Maractus = "SSH-7";
        const string Durant = "SSH-8";
        const string Rillaboom = "SSH-15";
        const string TorkoalV = "SSH-24";
        const string VictiniV = "SSH-25";
        const string FlameCloakCinderace = "SSH-36";
        const string ShowerKrabby = "SSH-42";
        const string SliceKrabby = "SSH-43";
        const string Kingler = "SSH-44";
        const string Seaking = "SSH-47";
        const string Qwilfish = "SSH-51";
        const string Mantine = "SSH-52";
        const string KeldeoV = "SSH-53";
        const string Pikachu = "SSH-65";
        const string Lanturn = "SSH-69";
        const string Morpeko = "SSH-78";
        const string Gastly = "SSH-83";
        const string Gengar = "SSH-85";
        const string WobbuffetV = "SSH-86";
        const string Musharna = "SSH-88";
        const string Sinistea = "SSH-89";
        const string Polteageist = "SSH-90";
        const string Hitmonlee = "SSH-94";
        const string Hitmonchan = "SSH-95";
        const string Rhydon = "SSH-98";
        const string Rhyperior = "SSH-99";
        const string Sudowoodo = "SSH-100";
        const string Claydol = "SSH-103";
        const string RegirockV = "SSH-104";
        const string Sandaconda = "SSH-110";
        const string StonjournerVmax = "SSH-116";
        const string SableyeV = "SSH-120";
        const string Toxicroak = "SSH-124";
        const string GalarianMeowth = "SSH-127";
        const string Ferrothorn = "SSH-131";
        const string Pawniard = "SSH-133";
        const string Bisharp = "SSH-134";
        const string Copperajah = "SSH-137";
        const string SnorlaxV = "SSH-141";
        const string SnorlaxVmax = "SSH-142";
        const string Noctowl = "SSH-144";
        const string Drampa = "SSH-149";
        const string EvolutionIncense = "SSH-163";
        const string LuckyEgg = "SSH-167";
        const string LumBerry = "SSH-168";
        const string MetalSaucer = "SSH-170";
        const string OrdinaryRod = "SSH-171";
        const string SitrusBerry = "SSH-182";
        const string AuroraEnergy = "SSH-186";

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
        public void Every_Sword_And_Shield_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 216; n++) Assert.IsNotNull(db.Get("SSH-" + n), "SSH " + n + " is missing");
            Assert.AreEqual("SHF-9", db.MainPrinting(C("SSH-9")).Key, "Dhelmise V came with Shining Fates");
            Assert.AreEqual("SHF-9", db.MainPrinting(C("SSH-187")).Key, "and its full art");
            Assert.AreEqual("RCL-206", db.MainPrinting(C("SSH-158")).Key, "Big Charm came with Rebel Clash");
            Assert.AreEqual("CPA-54", db.MainPrinting(C("SSH-166")).Key, "Hyper Potion came with Champion's Path");
            Assert.AreEqual("SSH-24", db.MainPrinting(C("SSH-188")).Key, "the full-art Torkoal V");
            Assert.AreEqual("SSH-170", db.MainPrinting(C("SSH-214")).Key, "the gold Metal Saucer");
            Assert.AreEqual(2, C(FlameCloakCinderace).RetreatCost, "Retreat Cost erratum (2020-12-23)");
            Assert.IsTrue(C(AuroraEnergy).IsSpecialEnergy);
            Assert.IsFalse(Formats.IsStandardCard(C(CelebiV)), "regulation mark D: Expanded");
        }

        // ================================================================== Grass

        [Test]
        public void Line_Force_Adds_20_For_Each_Benched_Pokemon()
        {
            PokemonInPlay celebi, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Line Force"), new ScriptedAgent(), CelebiV, WailordV, out celebi, out wailord);
            Attach(e, celebi, Grass, 2);
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters, "50 + 2 x 20");
        }

        [Test]
        public void Paralyze_Poison_Poisons_And_Paralyzes_On_Heads()
        {
            PokemonInPlay roserade, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Paralyze Poison"), new ScriptedAgent(), Roserade, Tauros, out roserade, out tauros);
            Attach(e, roserade, Grass, 1);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned) && tauros.Has(SpecialCondition.Paralyzed));

            PokemonInPlay r2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Paralyze Poison"), new ScriptedAgent(), Roserade, Tauros, out r2, out t2);
            Attach(e2, r2, Grass, 1);
            e2.State.Rng = new Rng(SeedFor(false));
            Step(e2);
            Assert.IsTrue(t2.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(t2.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Cotton_Ride_Shuffles_The_Defending_Pokemon_Into_The_Deck_On_Heads()
        {
            PokemonInPlay whimsicott, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Cotton Ride"), new ScriptedAgent(), Whimsicott, Tauros, out whimsicott, out tauros);
            Attach(e, whimsicott, Grass, 1);
            Attach(e, tauros, Fire, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(22, e.State.Players[1].Deck.Count, "Tauros and its Energy");
            Assert.AreEqual(benched, e.State.Players[1].Active);
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "not a Knock Out");

            PokemonInPlay w2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Cotton Ride"), new ScriptedAgent(), Whimsicott, Tauros, out w2, out t2);
            Attach(e2, w2, Grass, 1);
            e2.ScenarioBench(1, C(Tauros));
            e2.State.Rng = new Rng(SeedFor(false));
            Step(e2);
            Assert.AreEqual(t2, e2.State.Players[1].Active);
        }

        [Test]
        public void Powerful_Needles_Flips_A_Coin_For_Each_Energy()
        {
            PokemonInPlay maractus, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Powerful Needles"), new ScriptedAgent(), Maractus, WailordV, out maractus, out wailord);
            Attach(e, maractus, Grass, 3);
            e.State.Rng = new Rng(SeedFor(true, false, true));
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters, "2 heads x 60");
        }

        [Test]
        public void Bite_Together_Needs_A_Durant_On_The_Bench()
        {
            PokemonInPlay durant, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Bite Together"), new ScriptedAgent(), Durant, WailordV, out durant, out wailord);
            Attach(e, durant, Grass, 2);
            e.ScenarioBench(0, C(OtherDurant));
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters, "any card named Durant: 30 + 60");

            PokemonInPlay d2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Bite Together"), new ScriptedAgent(), Durant, WailordV, out d2, out w2);
            Attach(e2, d2, Grass, 2);
            e2.ScenarioBench(0, C(Tauros));
            Step(e2);
            Assert.AreEqual(3, w2.DamageCounters);
        }

        [Test]
        public void Drum_Roll_Also_Hits_Each_Benched_Pokemon_For_10()
        {
            PokemonInPlay rillaboom, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Drum Roll"), new ScriptedAgent(), Rillaboom, WailordV, out rillaboom, out wailord);
            Attach(e, rillaboom, Grass, 3);
            var b1 = e.ScenarioBench(1, C(Tauros));
            var b2 = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters);
            Assert.AreEqual(1, b1.DamageCounters);
            Assert.AreEqual(1, b2.DamageCounters);
        }

        // ================================================================== Fire

        [Test]
        public void Combustion_Pillar_Adds_90_Only_For_A_Basic_Fire_Energy()
        {
            PokemonInPlay torkoal, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Combustion Pillar"), new ScriptedAgent(), TorkoalV, WailordV, out torkoal, out wailord);
            Attach(e, torkoal, Fire, 3);
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Fire)));
            Step(e);
            Assert.AreEqual(18, wailord.DamageCounters, "90 + 90");
            Assert.AreEqual("Fire Energy", e.State.Players[0].Discard.Single().Def.Name);

            // Your ruling (v65): a Special Energy has its type only while attached.
            PokemonInPlay t2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Combustion Pillar"), new ScriptedAgent(), TorkoalV, WailordV, out t2, out w2);
            Attach(e2, t2, Fire, 3);
            e2.State.Players[0].Deck.Insert(0, e2.NewCard(0, C(HeatFire)));
            Step(e2);
            Assert.AreEqual(9, w2.DamageCounters);
        }

        [Test]
        public void Steam_Crush_Discards_2_Energy_From_The_Defending_Pokemon()
        {
            PokemonInPlay torkoal, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Steam Crush"), new ScriptedAgent(), TorkoalV, WailordV, out torkoal, out wailord);
            Attach(e, torkoal, Fire, 4);
            Attach(e, wailord, Water, 3);
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters);
            Assert.AreEqual(1, wailord.Energy.Count);
        }

        [Test]
        public void Energy_Burst_Counts_Both_Active_Pokemons_Energy_And_Spreading_Flames_Takes_Basic_Fire_Only()
        {
            PokemonInPlay victini, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Energy Burst"), new ScriptedAgent(), VictiniV, WailordV, out victini, out wailord);
            Attach(e, victini, Fire, 2);
            Attach(e, victini, Psychic, 1);
            Attach(e, wailord, Water, 2);
            Step(e);
            Assert.AreEqual(15, wailord.DamageCounters, "5 Energy x 30");

            PokemonInPlay v2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Spreading Flames"), new ScriptedAgent(), VictiniV, WailordV, out v2, out w2);
            Attach(e2, v2, Psychic, 1);
            e2.ScenarioAddToDiscard(0, C(HeatFire), 1);
            Step(e2);
            Assert.AreEqual(1, v2.Energy.Count, "Heat Fire Energy isn't a Fire Energy card in the discard pile");
            Assert.AreEqual(1, e2.State.Players[0].Discard.Count);
        }

        [Test]
        public void Flame_Cloak_Takes_Basic_Fire_And_Bright_Flame_Discards_2_On_Tails()
        {
            PokemonInPlay cinderace, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Flame Cloak"), new ScriptedAgent(), FlameCloakCinderace, WailordV, out cinderace, out wailord);
            Attach(e, cinderace, Fire, 1);
            e.ScenarioAddToDiscard(0, C(HeatFire), 1);
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            Step(e);
            Assert.AreEqual(4, wailord.DamageCounters);
            Assert.AreEqual(2, cinderace.Energy.Count);
            Assert.AreEqual("Heat Fire Energy", e.State.Players[0].Discard.Single().Def.Name);

            PokemonInPlay c2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Bright Flame"), new ScriptedAgent(), FlameCloakCinderace, WailordV, out c2, out w2);
            Attach(e2, c2, Fire, 3);
            e2.State.Rng = new Rng(SeedFor(false));
            Step(e2);
            Assert.AreEqual(16, w2.DamageCounters);
            Assert.AreEqual(1, c2.Energy.Count);

            PokemonInPlay c3, w3;
            var e3 = Board(new ScriptedAgent().Then("Attack: Bright Flame"), new ScriptedAgent(), FlameCloakCinderace, WailordV, out c3, out w3);
            Attach(e3, c3, Fire, 3);
            e3.State.Rng = new Rng(SeedFor(true));
            Step(e3);
            Assert.AreEqual(3, c3.Energy.Count);
        }

        // ================================================================== Water

        [Test]
        public void Aqua_Shower_Does_10_To_Each_Opposing_Pokemon()
        {
            PokemonInPlay krabby, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Aqua Shower"), new ScriptedAgent(), ShowerKrabby, Tauros, out krabby, out tauros);
            Attach(e, krabby, Water, 1);
            var b1 = e.ScenarioBench(1, C(Tauros));
            var b2 = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(1, tauros.DamageCounters);
            Assert.AreEqual(1, b1.DamageCounters);
            Assert.AreEqual(1, b2.DamageCounters);
        }

        [Test]
        public void Super_Slice_Needs_Two_Heads()
        {
            PokemonInPlay krabby, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Super Slice"), new ScriptedAgent(), SliceKrabby, WailordV, out krabby, out wailord);
            Attach(e, krabby, Water, 2);
            e.State.Rng = new Rng(SeedFor(true, false));
            Step(e);
            Assert.AreEqual(0, wailord.DamageCounters);

            PokemonInPlay k2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Super Slice"), new ScriptedAgent(), SliceKrabby, WailordV, out k2, out w2);
            Attach(e2, k2, Water, 2);
            e2.State.Rng = new Rng(SeedFor(true, true));
            Step(e2);
            Assert.AreEqual(12, w2.DamageCounters);
        }

        [Test]
        public void Claw_Rend_Adds_60_Against_A_Damaged_Pokemon_And_Heavy_Pincers_Mills_1()
        {
            PokemonInPlay kingler, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Claw Rend"), new ScriptedAgent(), Kingler, WailordV, out kingler, out wailord);
            Attach(e, kingler, Water, 3);
            wailord.DamageCounters = 1;
            Step(e);
            Assert.AreEqual(16, wailord.DamageCounters, "1 + 90 + 60");

            PokemonInPlay k2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Heavy Pincers"), new ScriptedAgent(), Kingler, WailordV, out k2, out w2);
            Attach(e2, k2, Water, 1);
            Step(e2);
            Assert.AreEqual(4, w2.DamageCounters);
            Assert.AreEqual(19, e2.State.Players[1].Deck.Count);
        }

        [Test]
        public void Ripping_Horn_Discards_An_Energy_For_Each_Heads()
        {
            PokemonInPlay seaking, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Ripping Horn"), new ScriptedAgent(), Seaking, WailordV, out seaking, out wailord);
            Attach(e, seaking, Water, 1);
            Attach(e, wailord, Water, 3);
            e.State.Rng = new Rng(SeedFor(true, false, true));
            Step(e);
            Assert.AreEqual(1, wailord.Energy.Count);
            Assert.AreEqual(0, wailord.DamageCounters);
        }

        [Test]
        public void Poison_Point_Poisons_The_Attacker_And_Venoshock_Adds_60()
        {
            PokemonInPlay tauros, qwilfish;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Qwilfish, out tauros, out qwilfish);
            Attach(e, tauros, Psychic, 2);
            Step(e);
            Assert.AreEqual(3, qwilfish.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));

            PokemonInPlay q2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Venoshock"), new ScriptedAgent(), Qwilfish, Tauros, out q2, out t2);
            Attach(e2, q2, Water, 1);
            t2.Conditions = SpecialCondition.Poisoned;
            Step(e2);
            Assert.AreEqual(7, t2.DamageCounters, "10 + 60");
        }

        [Test]
        public void Water_Reserve_Finds_Basic_Water_Energy_Only()
        {
            PokemonInPlay mantine, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Water Reserve", "Water Energy|Water Energy"), new ScriptedAgent(), Mantine, Tauros, out mantine, out tauros);
            Attach(e, mantine, Water, 1);
            e.ScenarioAddToDeck(0, C(WashWater), 2);
            e.ScenarioAddToDeck(0, C(Water), 2);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(2, hand.Count);
            Assert.IsTrue(hand.All(c => c.Def.Name == "Water Energy" && c.Def.IsBasicEnergy));
        }

        [Test]
        public void Secret_Sword_Adds_30_For_Each_Water_Energy()
        {
            PokemonInPlay keldeo, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Secret Sword"), new ScriptedAgent(), KeldeoV, WailordV, out keldeo, out wailord);
            Attach(e, keldeo, Water, 2);
            Attach(e, keldeo, Psychic, 1);
            Step(e);
            Assert.AreEqual(11, wailord.DamageCounters, "50 + 2 x 30");
        }

        // ================================================================== Lightning

        [Test]
        public void Charge_Attaches_A_Basic_Lightning_Energy_From_The_Deck()
        {
            PokemonInPlay pikachu, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Charge"), new ScriptedAgent(), Pikachu, Tauros, out pikachu, out tauros);
            Attach(e, pikachu, Psychic, 1);
            e.ScenarioAddToDeck(0, C(Lightning), 1);
            Step(e);
            Assert.AreEqual(2, pikachu.Energy.Count);

            PokemonInPlay p2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Charge"), new ScriptedAgent(), Pikachu, Tauros, out p2, out t2);
            Attach(e2, p2, Psychic, 1);
            e2.ScenarioAddToDeck(0, C(SpeedLightning), 1);
            Step(e2);
            Assert.AreEqual(1, p2.Energy.Count, "Speed Lightning Energy isn't a Lightning Energy card in the deck");
        }

        [Test]
        public void Strobe_Shock_Paralyzes_Only_When_The_Hand_Has_An_Energy_Card()
        {
            PokemonInPlay lanturn, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Strobe Shock"), new ScriptedAgent(), Lanturn, Tauros, out lanturn, out tauros);
            Attach(e, lanturn, Lightning, 3);
            e.ScenarioAddToHand(1, C(BossOrders));
            e.ScenarioAddToHand(1, C(HeatFire));
            Step(e);
            Assert.AreEqual(9, tauros.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Paralyzed), "any Energy card, Special ones too");

            PokemonInPlay l2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Strobe Shock"), new ScriptedAgent(), Lanturn, Tauros, out l2, out t2);
            Attach(e2, l2, Lightning, 3);
            e2.ScenarioAddToHand(1, C(BossOrders));
            Step(e2);
            Assert.IsFalse(t2.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Attack_The_Wound_Adds_50_Against_A_Damaged_Pokemon()
        {
            PokemonInPlay morpeko, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Attack the Wound"), new ScriptedAgent(), Morpeko, Tauros, out morpeko, out tauros);
            Attach(e, morpeko, Lightning, 1);
            tauros.DamageCounters = 1;
            Step(e);
            Assert.AreEqual(7, tauros.DamageCounters, "1 + 10 + 50");

            PokemonInPlay m2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Attack the Wound"), new ScriptedAgent(), Morpeko, Tauros, out m2, out t2);
            Attach(e2, m2, Lightning, 1);
            Step(e2);
            Assert.AreEqual(1, t2.DamageCounters);
        }

        // ================================================================== Psychic

        [Test]
        public void Fade_Out_Puts_Gastly_And_Its_Energy_Into_The_Hand()
        {
            PokemonInPlay gastly, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Fade Out"), new ScriptedAgent(), Gastly, Tauros, out gastly, out tauros);
            Attach(e, gastly, Psychic, 1);
            var benched = e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters);
            CollectionAssert.AreEquivalent(new[] { "Gastly", "Psychic Energy" }, e.State.Players[0].Hand.Select(c => c.Def.Name).ToArray());
            Assert.AreEqual(benched, e.State.Players[0].Active);
        }

        [Test]
        public void Life_Shaker_Moves_Damage_Counters_Between_Psychic_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Life Shaker"), new ScriptedAgent());
            var gengar = e.ScenarioActive(0, C(Gengar));
            gengar.DamageCounters = 2;
            var gastly = e.ScenarioBench(0, C(Gastly));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, gengar.DamageCounters);
            Assert.AreEqual(1, gastly.DamageCounters);
            Assert.AreEqual(0, tauros.DamageCounters);
            Assert.IsTrue(Offered(e, 0, "Use Life Shaker"), "as often as you like");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Gengar));
            var hurt = e2.ScenarioBench(0, C(Tauros));
            hurt.DamageCounters = 3;
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Use Life Shaker"), "Tauros isn't a Psychic Pokémon");
        }

        [Test]
        public void Life_Shaker_Can_Knock_Out_Your_Own_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Life Shaker", "Gengar"), new ScriptedAgent());
            var gengar = e.ScenarioActive(0, C(Gengar));
            gengar.DamageCounters = 1;
            var gastly = e.ScenarioBench(0, C(Gastly));
            gastly.DamageCounters = 5;
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count, "Gastly (60 HP) was Knocked Out");
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "your opponent takes a Prize card");
        }

        [Test]
        public void Gritty_Comeback_Switches_The_Damage_Counters()
        {
            PokemonInPlay wobbuffet, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Gritty Comeback"), new ScriptedAgent(), WobbuffetV, Tauros, out wobbuffet, out tauros);
            Attach(e, wobbuffet, Psychic, 2);
            wobbuffet.DamageCounters = 10;
            tauros.DamageCounters = 3;
            Step(e);
            Assert.AreEqual(3, wobbuffet.DamageCounters);
            Assert.AreEqual(10, tauros.DamageCounters);

            PokemonInPlay w2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Gritty Comeback"), new ScriptedAgent(), WobbuffetV, Tauros, out w2, out t2);
            Attach(e2, w2, Psychic, 2);
            w2.DamageCounters = 13;
            var benched = e2.ScenarioBench(1, C(Tauros));
            Step(e2);
            Assert.AreEqual(0, w2.DamageCounters);
            Assert.AreEqual(benched, e2.State.Players[1].Active, "Tauros (120 HP) was Knocked Out");
            Assert.AreEqual(5, e2.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Sleepy_Pulse_Wakes_Only_On_Two_Heads()
        {
            PokemonInPlay musharna, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Sleepy Pulse"), new ScriptedAgent(), Musharna, Tauros, out musharna, out tauros);
            Attach(e, musharna, Psychic, 1);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Asleep));
            e.State.Rng = new Rng(SeedFor(true, false));
            e.EndTurn();
            Assert.IsTrue(tauros.Has(SpecialCondition.Asleep), "one tails keeps it Asleep");

            PokemonInPlay m2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Sleepy Pulse"), new ScriptedAgent(), Musharna, Tauros, out m2, out t2);
            Attach(e2, m2, Psychic, 1);
            Step(e2);
            e2.State.Rng = new Rng(SeedFor(true, true));
            e2.EndTurn();
            Assert.IsFalse(t2.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Super_Hypnoblast_Adds_120_Against_An_Asleep_Pokemon()
        {
            PokemonInPlay musharna, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Super Hypnoblast"), new ScriptedAgent(), Musharna, WailordV, out musharna, out wailord);
            Attach(e, musharna, Psychic, 2);
            wailord.Conditions = SpecialCondition.Asleep;
            Step(e);
            Assert.AreEqual(13, wailord.DamageCounters);
        }

        [Test]
        public void Teatime_Has_Each_Player_Draw_2()
        {
            PokemonInPlay sinistea, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Teatime"), new ScriptedAgent(), Sinistea, Tauros, out sinistea, out tauros);
            Attach(e, sinistea, Psychic, 1);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Poltergeist_Does_50_For_Each_Trainer_In_The_Opponents_Hand()
        {
            PokemonInPlay polteageist, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Poltergeist"), new ScriptedAgent(), Polteageist, Tauros, out polteageist, out tauros);
            Attach(e, polteageist, Psychic, 2);
            e.ScenarioAddToHand(1, C(BossOrders));
            e.ScenarioAddToHand(1, C(LumBerry));
            e.ScenarioAddToHand(1, C(Fire));
            Step(e);
            Assert.AreEqual(10, tauros.DamageCounters);
        }

        // ================================================================== Fighting

        [Test]
        public void Coordinated_Strike_And_Coordinated_Beatdown_Need_The_Partner_On_The_Bench()
        {
            PokemonInPlay lee, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Coordinated Strike"), new ScriptedAgent(), Hitmonlee, WailordV, out lee, out wailord);
            Attach(e, lee, Fighting, 3);
            e.ScenarioBench(0, C(Hitmonchan));
            Step(e);
            Assert.AreEqual(16, wailord.DamageCounters, "80 + 80");

            PokemonInPlay chan, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Coordinated Beatdown"), new ScriptedAgent(), Hitmonchan, WailordV, out chan, out w2);
            Attach(e2, chan, Fighting, 1);
            e2.ScenarioBench(0, C(Hitmonlee));
            Step(e2);
            Assert.AreEqual(4, w2.DamageCounters, "20 + 20");

            PokemonInPlay c3, w3;
            var e3 = Board(new ScriptedAgent().Then("Attack: Coordinated Beatdown"), new ScriptedAgent(), Hitmonchan, WailordV, out c3, out w3);
            Attach(e3, c3, Fighting, 1);
            Step(e3);
            Assert.AreEqual(2, w3.DamageCounters);
        }

        [Test]
        public void Rock_Slide_Also_Hits_2_Benched_Pokemon_For_10()
        {
            PokemonInPlay rhydon, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Rock Slide"), new ScriptedAgent(), Rhydon, WailordV, out rhydon, out wailord);
            Attach(e, rhydon, Fighting, 4);
            var b1 = e.ScenarioBench(1, C(Tauros));
            var b2 = e.ScenarioBench(1, C(Tauros));
            var b3 = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(10, wailord.DamageCounters);
            Assert.AreEqual(2, b1.DamageCounters + b2.DamageCounters + b3.DamageCounters);
        }

        [Test]
        public void Bedrock_Shake_Hits_Every_Damaged_Benched_Pokemon_On_Both_Sides()
        {
            PokemonInPlay rhyperior, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Bedrock Shake"), new ScriptedAgent(), Rhyperior, WailordV, out rhyperior, out wailord);
            Attach(e, rhyperior, Fighting, 4);
            var myHurt = e.ScenarioBench(0, C(Tauros));
            myHurt.DamageCounters = 2;
            var myFresh = e.ScenarioBench(0, C(Tauros));
            var theirHurt = e.ScenarioBench(1, C(Tauros));
            theirHurt.DamageCounters = 1;
            var theirFresh = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters);
            Assert.AreEqual(8, myHurt.DamageCounters, "your own damaged Benched Pokémon too");
            Assert.AreEqual(0, myFresh.DamageCounters);
            Assert.AreEqual(7, theirHurt.DamageCounters, "no Weakness for the Bench");
            Assert.AreEqual(0, theirFresh.DamageCounters);
        }

        [Test]
        public void Flail_And_Raging_Hammer_Count_Their_Own_Damage_Counters()
        {
            PokemonInPlay sudowoodo, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Flail"), new ScriptedAgent(), Sudowoodo, Tauros, out sudowoodo, out tauros);
            Attach(e, sudowoodo, Fighting, 1);
            sudowoodo.DamageCounters = 4;
            Step(e);
            Assert.AreEqual(8, tauros.DamageCounters, "40, doubled by Weakness");

            PokemonInPlay regirock, wailord;
            var e2 = Board(new ScriptedAgent().Then("Attack: Raging Hammer"), new ScriptedAgent(), RegirockV, WailordV, out regirock, out wailord);
            Attach(e2, regirock, Fighting, 2);
            regirock.DamageCounters = 5;
            Step(e2);
            Assert.AreEqual(8, wailord.DamageCounters, "30 + 5 x 10");
        }

        [Test]
        public void Explosion_Knocks_Out_Claydol_Too()
        {
            PokemonInPlay claydol, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Explosion"), new ScriptedAgent(), Claydol, WailordV, out claydol, out wailord);
            Attach(e, claydol, Fighting, 2);
            var benched = e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(20, wailord.DamageCounters);
            Assert.AreEqual(benched, e.State.Players[0].Active);
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count);
        }

        [Test]
        public void Sand_Sac_Takes_30_Less_And_Power_Press_Needs_An_Extra_Fighting_Energy()
        {
            PokemonInPlay tauros, sandaconda;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Sandaconda, out tauros, out sandaconda);
            Attach(e, tauros, Psychic, 2);
            Step(e);
            Assert.AreEqual(0, sandaconda.DamageCounters);

            PokemonInPlay s2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Power Press"), new ScriptedAgent(), Sandaconda, WailordV, out s2, out w2);
            Attach(e2, s2, Fighting, 3);
            Step(e2);
            Assert.AreEqual(13, w2.DamageCounters, "60 + 70");

            PokemonInPlay s3, w3;
            var e3 = Board(new ScriptedAgent().Then("Attack: Power Press"), new ScriptedAgent(), Sandaconda, WailordV, out s3, out w3);
            Attach(e3, s3, Fighting, 2);
            Attach(e3, s3, Psychic, 1);
            Step(e3);
            Assert.AreEqual(6, w3.DamageCounters, "the extra Energy must be Fighting");
        }

        [Test]
        public void Stone_Gift_Attaches_A_Basic_Fighting_Energy_From_The_Hand_And_Heals_120()
        {
            PokemonInPlay stonjourner, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Stone Gift", "Bench 1"), new ScriptedAgent(), StonjournerVmax, Tauros, out stonjourner, out tauros);
            Attach(e, stonjourner, Fighting, 1);
            var hurt = e.ScenarioBench(0, C(Tauros));
            hurt.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(StoneFighting));
            e.ScenarioAddToHand(0, C(Fighting));
            Step(e);
            Assert.AreEqual(1, hurt.Energy.Count);
            Assert.AreEqual(0, hurt.DamageCounters);
            Assert.AreEqual("Stone Fighting Energy", e.State.Players[0].Hand.Single().Def.Name, "basic Energy only");
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn, "it isn't the attachment for the turn");
        }

        // ================================================================== Darkness

        [Test]
        public void Lode_Search_Takes_A_Trainer_And_Crazy_Claws_Adds_60_Per_Counter()
        {
            PokemonInPlay sableye, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Lode Search"), new ScriptedAgent(), SableyeV, Tauros, out sableye, out tauros);
            Attach(e, sableye, Darkness, 1);
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioAddToDiscard(0, C(BossOrders), 1);
            Step(e);
            Assert.AreEqual("Boss's Orders", e.State.Players[0].Hand.Single().Def.Name);

            PokemonInPlay s2, wailord;
            var e2 = Board(new ScriptedAgent().Then("Attack: Crazy Claws"), new ScriptedAgent(), SableyeV, WailordV, out s2, out wailord);
            Attach(e2, s2, Darkness, 2);
            wailord.DamageCounters = 2;
            Step(e2);
            Assert.AreEqual(15, wailord.DamageCounters, "2 + 10 + 2 x 60");
        }

        [Test]
        public void More_Poison_Adds_2_Counters_And_Stacks()
        {
            // Compendium (2020-02-06): two Toxicroak add 4.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Toxicroak));
            e.ScenarioBench(0, C(Toxicroak));
            var tauros = e.ScenarioActive(1, C(Tauros));
            tauros.Conditions = SpecialCondition.Poisoned;
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(5, tauros.DamageCounters, "1 + 2 + 2");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioBench(0, C(Toxicroak));
            e2.ScenarioActive(0, C(Tauros));
            var t2 = e2.ScenarioActive(1, C(Tauros));
            t2.Conditions = SpecialCondition.Poisoned;
            e2.ScenarioSetTurn(0, 3);
            e2.EndTurn();
            Assert.AreEqual(3, t2.DamageCounters, "from the Bench too");
        }

        // ================================================================== Metal

        [Test]
        public void Hone_Claws_Adds_60_To_Slash_Next_Turn()
        {
            var me = new ScriptedAgent().Then("Attack: Hone Claws");
            PokemonInPlay meowth, wailord;
            var e = Board(me, new ScriptedAgent(), GalarianMeowth, WailordV, out meowth, out wailord);
            Attach(e, meowth, Metal, 2);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            me.Then("Attack: Slash");
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters, "30 + 60");
        }

        [Test]
        public void Energy_Slap_May_Move_All_Energy_To_A_Benched_Pokemon()
        {
            PokemonInPlay ferrothorn, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Energy Slap", "Yes"), new ScriptedAgent(), Ferrothorn, WailordV, out ferrothorn, out wailord);
            Attach(e, ferrothorn, Metal, 3);
            var benched = e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(10, wailord.DamageCounters);
            Assert.AreEqual(0, ferrothorn.Energy.Count);
            Assert.AreEqual(3, benched.Energy.Count);

            PokemonInPlay f2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Energy Slap", "No"), new ScriptedAgent(), Ferrothorn, WailordV, out f2, out w2);
            Attach(e2, f2, Metal, 3);
            e2.ScenarioBench(0, C(Tauros));
            Step(e2);
            Assert.AreEqual(3, f2.Energy.Count);
        }

        [Test]
        public void Charge_Order_Adds_30_For_Each_Benched_Pawniard()
        {
            PokemonInPlay bisharp, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Charge Order"), new ScriptedAgent(), Bisharp, WailordV, out bisharp, out wailord);
            Attach(e, bisharp, Metal, 1);
            e.ScenarioBench(0, C(Pawniard));
            e.ScenarioBench(0, C(Pawniard));
            e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters, "30 + 2 x 30");
        }

        [Test]
        public void Muscular_Nose_Does_Nothing_With_8_Or_More_Counters()
        {
            PokemonInPlay copperajah, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Muscular Nose"), new ScriptedAgent(), Copperajah, WailordV, out copperajah, out wailord);
            Attach(e, copperajah, Metal, 3);
            copperajah.DamageCounters = 7;
            Step(e);
            Assert.AreEqual(22, wailord.DamageCounters);

            PokemonInPlay c2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Muscular Nose"), new ScriptedAgent(), Copperajah, WailordV, out c2, out w2);
            Attach(e2, c2, Metal, 3);
            c2.DamageCounters = 8;
            Step(e2);
            Assert.AreEqual(0, w2.DamageCounters);
        }

        // ================================================================== Colorless

        [Test]
        public void Swallow_Heals_The_Damage_Done_And_Falling_Down_Puts_Snorlax_To_Sleep()
        {
            PokemonInPlay snorlax, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Swallow"), new ScriptedAgent(), SnorlaxV, Tauros, out snorlax, out tauros);
            Attach(e, snorlax, Psychic, 3);
            snorlax.DamageCounters = 8;
            Step(e);
            Assert.AreEqual(6, tauros.DamageCounters);
            Assert.AreEqual(2, snorlax.DamageCounters);

            PokemonInPlay s2, wailord;
            var e2 = Board(new ScriptedAgent().Then("Attack: Falling Down"), new ScriptedAgent(), SnorlaxV, WailordV, out s2, out wailord);
            Attach(e2, s2, Psychic, 4);
            Step(e2);
            Assert.AreEqual(17, wailord.DamageCounters);
            Assert.IsTrue(s2.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void G_Max_Fall_Adds_30_For_Each_Benched_Pokemon()
        {
            PokemonInPlay snorlax, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: G-Max Fall"), new ScriptedAgent(), SnorlaxVmax, WailordV, out snorlax, out wailord);
            Attach(e, snorlax, Psychic, 3);
            for (int i = 0; i < 3; i++) e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(15, wailord.DamageCounters, "60 + 3 x 30");
        }

        [Test]
        public void Carry_Off_Shuffles_A_Benched_Pokemon_And_Noctowl_Into_The_Decks()
        {
            PokemonInPlay noctowl, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Carry Off"), new ScriptedAgent(), Noctowl, Tauros, out noctowl, out tauros);
            Attach(e, noctowl, Psychic, 3);
            var theirBench = e.ScenarioBench(1, C(Tauros));
            Attach(e, theirBench, Fire, 1);
            var myBench = e.ScenarioBench(0, C(Tauros));
            Step(e);
            Assert.AreEqual(0, e.State.Players[1].Bench.Count);
            Assert.AreEqual(22, e.State.Players[1].Deck.Count);
            Assert.AreEqual(24, e.State.Players[0].Deck.Count, "Noctowl and its 3 Energy");
            Assert.AreEqual(myBench, e.State.Players[0].Active);

            // As Butterfree's Bye-Bye Flight (your ruling, v38): with no Benched Pokémon to choose, the attack does nothing.
            PokemonInPlay n2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Carry Off"), new ScriptedAgent(), Noctowl, Tauros, out n2, out t2);
            Attach(e2, n2, Psychic, 3);
            Step(e2);
            Assert.IsFalse(e2.State.IsOver);
            Assert.AreEqual(n2, e2.State.Players[0].Active);
        }

        [Test]
        public void Dragon_Pulse_Discards_The_Top_2_Cards_Of_Your_Deck()
        {
            PokemonInPlay drampa, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Dragon Pulse"), new ScriptedAgent(), Drampa, WailordV, out drampa, out wailord);
            Attach(e, drampa, Psychic, 2);
            Step(e);
            Assert.AreEqual(8, wailord.DamageCounters);
            Assert.AreEqual(18, e.State.Players[0].Deck.Count);
        }

        // ================================================================== Trainers

        [Test]
        public void Evolution_Incense_Finds_An_Evolution_Pokemon_Including_A_Vmax()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Evolution Incense"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(EvolutionIncense));
            e.ScenarioAddToDeck(0, C(Gastly), 1);
            e.ScenarioAddToDeck(0, C(SnorlaxVmax), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Snorlax VMAX", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Lucky_Egg_Draws_Up_To_7_Only_After_A_Knock_Out_By_Attack_Damage()
        {
            PokemonInPlay tauros, theirs;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Tauros, out tauros, out theirs);
            Attach(e, tauros, Psychic, 2);
            theirs.DamageCounters = 10;
            e.ScenarioAttachTool(theirs, C(LuckyEgg));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Fire));
            Step(e);
            Assert.AreEqual(7, e.State.Players[1].Hand.Count);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Lucky Egg"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            var poisoned = e2.ScenarioActive(1, C(Tauros));
            poisoned.DamageCounters = 11;
            poisoned.Conditions = SpecialCondition.Poisoned;
            e2.ScenarioAttachTool(poisoned, C(LuckyEgg));
            e2.ScenarioBench(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            e2.EndTurn();
            Assert.AreEqual(0, e2.State.Players[1].Hand.Count, "Knocked Out by Poison: no draw");
        }

        [Test]
        public void Lum_Berry_Cures_Before_Pokemon_Checkup_And_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.Conditions = SpecialCondition.Poisoned | SpecialCondition.Confused;
            e.ScenarioAttachTool(tauros, C(LumBerry));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(SpecialCondition.None, tauros.Conditions);
            Assert.AreEqual(0, tauros.DamageCounters, "no Poison damage");
            Assert.AreEqual(0, tauros.Tools.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Lum Berry"));
        }

        [Test]
        public void Sitrus_Berry_Heals_30_At_3_Or_More_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 4;
            e.ScenarioAttachTool(tauros, C(SitrusBerry));
            var theirs = e.ScenarioActive(1, C(Tauros));
            theirs.DamageCounters = 2;
            e.ScenarioAttachTool(theirs, C(SitrusBerry));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, tauros.DamageCounters);
            Assert.AreEqual(0, tauros.Tools.Count);
            Assert.AreEqual(2, theirs.DamageCounters);
            Assert.AreEqual(1, theirs.Tools.Count, "only 2 damage counters");
        }

        [Test]
        public void Metal_Saucer_Attaches_A_Basic_Metal_Energy_To_A_Benched_Metal_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Metal Saucer"), new ScriptedAgent());
            e.ScenarioActive(0, C(GalarianMeowth));
            var meowth = e.ScenarioBench(0, C(GalarianMeowth));
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(MetalSaucer));
            e.ScenarioAddToDiscard(0, C(Metal), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, meowth.Energy.Count);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(GalarianMeowth));
            e2.ScenarioBench(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(MetalSaucer));
            e2.ScenarioAddToDiscard(0, C(Metal), 1);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Play Metal Saucer"), "a Benched Metal Pokémon only");
        }

        [Test]
        public void Ordinary_Rod_Shuffles_Up_To_2_Pokemon_And_2_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ordinary Rod", "Both", "Tauros|Gastly", "Fire Energy|Fire Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(OrdinaryRod));
            e.ScenarioAddToDiscard(0, C(Gastly), 1);
            e.ScenarioAddToDiscard(0, C(Tauros), 2);
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            e.ScenarioAddToDiscard(0, C(HeatFire), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var discard = e.State.Players[0].Discard;
            Assert.AreEqual(24, e.State.Players[0].Deck.Count);
            Assert.AreEqual(1, discard.Count(c => c.Def.Name == "Tauros"));
            Assert.AreEqual(0, discard.Count(c => c.Def.Name == "Gastly"));
            Assert.AreEqual(1, discard.Count(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(1, discard.Count(c => c.Def.Name == "Heat Fire Energy"), "basic Energy only");
        }

        // ================================================================== Aurora Energy

        [Test]
        public void Aurora_Energy_Needs_Another_Card_In_The_Hand_To_Discard()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(AuroraEnergy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Aurora Energy"));
            e.ScenarioAddToHand(0, C(BossOrders));
            Assert.IsTrue(Offered(e, 0, "Attach Aurora Energy"));
        }

        [Test]
        public void Aurora_Energy_Discards_The_Chosen_Card_And_Provides_Every_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Aurora Energy", "Boss's Orders"), new ScriptedAgent());
            var cinderace = e.ScenarioActive(0, C(FlameCloakCinderace));
            e.ScenarioAddToHand(0, C(AuroraEnergy));
            e.ScenarioAddToHand(0, C(BossOrders));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Aurora Energy", cinderace.Energy.Single().Def.Name);
            Assert.AreEqual("Boss's Orders", e.State.Players[0].Discard.Single().Def.Name);
            Assert.AreEqual("Water Energy", e.State.Players[0].Hand.Single().Def.Name);
            Assert.AreEqual(1, e.EnergyCount(cinderace), "1 Energy at a time");
            Assert.IsTrue(Offered(e, 0, "Attack: Flame Cloak"), "it provides Fire");
            Assert.IsFalse(Offered(e, 0, "Attack: Bright Flame"));
        }

        [Test]
        public void Slimy_Room_Flips_Before_Aurora_Energys_Discard()
        {
            // Your ruling (v66): Slimy Room flips when the player tries to attach; on tails Aurora Energy is discarded instead and
            // no other card is; on heads the other card is discarded and Aurora Energy is attached.
            var e = Scenario(new ScriptedAgent().Then("Attach Aurora Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(AuroraEnergy));
            e.ScenarioAddToHand(0, C(BossOrders));
            e.ScenarioActive(1, C(Goodra));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual("Aurora Energy", e.State.Players[0].Discard.Single().Def.Name);
            Assert.AreEqual("Boss's Orders", e.State.Players[0].Hand.Single().Def.Name);
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Aurora Energy"), new ScriptedAgent());
            var t2 = e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(AuroraEnergy));
            e2.ScenarioAddToHand(0, C(BossOrders));
            e2.ScenarioActive(1, C(Goodra));
            e2.ScenarioSetTurn(0, 3);
            e2.State.Rng = new Rng(SeedFor(true));
            Step(e2);
            Assert.AreEqual("Aurora Energy", t2.Energy.Single().Def.Name);
            Assert.AreEqual("Boss's Orders", e2.State.Players[0].Discard.Single().Def.Name);
        }

        [Test]
        public void Aurora_Energy_Attached_From_The_Hand_By_An_Attack_Still_Costs_A_Card()
        {
            // Compendium (2020-02-06): the discard is needed whenever it's attached from the hand.
            PokemonInPlay eevee, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Cheer Up"), new ScriptedAgent(), CheerUpEevee, Tauros, out eevee, out tauros);
            Attach(e, eevee, Psychic, 1);
            e.ScenarioAddToHand(0, C(AuroraEnergy));
            e.ScenarioAddToHand(0, C(BossOrders));
            Step(e);
            Assert.AreEqual(2, eevee.Energy.Count);
            Assert.AreEqual("Boss's Orders", e.State.Players[0].Discard.Single().Def.Name);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);

            PokemonInPlay e2vee, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Cheer Up"), new ScriptedAgent(), CheerUpEevee, Tauros, out e2vee, out t2);
            Attach(e2, e2vee, Psychic, 1);
            e2.ScenarioAddToHand(0, C(AuroraEnergy));
            Step(e2);
            Assert.AreEqual(1, e2vee.Energy.Count);
            Assert.AreEqual("Aurora Energy", e2.State.Players[0].Hand.Single().Def.Name, "nothing else to discard: it stays in the hand");
        }
    }
}
