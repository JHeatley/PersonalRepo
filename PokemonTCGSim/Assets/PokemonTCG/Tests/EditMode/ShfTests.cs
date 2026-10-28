using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v61: Shining Fates (SHF 1-73, SV1-SV122). Your rulings: Ditto V's V Transformation sends Ditto V to the discard pile
    /// and can't choose another Ditto V; Arctozolt's Biting Whirlpool works from the Bench and each Arctozolt adds 2 damage
    /// counters.
    /// </summary>
    public class ShfTests : ScenarioTestBase
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
        const string ZamazentaV = "CRZ-98";
        const string Tepig = "BST-23";
        const string Pignite = "BST-24";
        const string PokeBall = "POR-80";
        const string GreatBall = "PAL-183";

        const string Celebi = "SHF-3";
        const string DhelmiseV = "SHF-9";
        const string Decidueye = "SHF-8";
        const string Yanma = "SHF-1";
        const string Luxio = "SHF-32";
        const string Luxray = "SHF-33";
        const string Yveltal = "SHF-46";
        const string Nickit = "SHF-47";
        const string GalarianWeezing = "SHF-42";
        const string Cufant = "SHF-49";
        const string DittoV = "SHF-50";
        const string DittoVmax = "SHF-51";
        const string AlcremieV = "SHF-64";
        const string AlcremieVmax = "SHF-73";
        const string BallGuy = "SHF-57";
        const string RustedShield = "SHF-61";
        const string RustedSword = "SHF-62";
        const string TeamYellTowel = "SHF-63";
        const string BirdKeeper = "SHF-66";
        const string Rose = "SHF-71";
        const string RillaboomV = "SHF-SV105";
        const string RillaboomVmax = "SHF-SV106";
        const string Cinderace = "SHF-SV17";
        const string MrRime = "SHF-SV21";
        const string Sobble = "SHF-SV25";
        const string Drizzile = "SHF-SV26";
        const string Eiscue = "SHF-SV35";
        const string Dracovish = "SHF-SV36";
        const string Arctozolt = "SHF-SV46";
        const string GalarianRapidash = "SHF-SV48";
        const string GalarianCursola = "SHF-SV50";
        const string Dedenne = "SHF-SV51";
        const string Polteageist = "SHF-SV53";
        const string Dragapult = "SHF-SV62";
        const string Sirfetchd = "SHF-SV64";
        const string Coalossal = "SHF-SV69";
        const string Grapploct = "SHF-SV73";
        const string Falinks = "SHF-SV74";
        const string Grimmsnarl = "SHF-SV85";
        const string GalarianMeowth = "SHF-SV86";
        const string Perrserker = "SHF-SV87";
        const string GalarianStunfisk = "SHF-SV88";
        const string Swanna = "SHF-SV96";
        const string FalinksV = "SHF-SV115";
        const string GrimmsnarlV = "SHF-SV116";
        const string GrimmsnarlVmax = "SHF-SV117";

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
        public void Every_Shining_Fates_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 73; n++) Assert.IsNotNull(db.Get("SHF-" + n), "SHF " + n + " is missing");
            for (int n = 1; n <= 122; n++) Assert.IsNotNull(db.Get("SHF-SV" + n), "SHF SV" + n + " is missing");
            Assert.AreEqual("ASR-TG4", db.MainPrinting(C("SHF-30")).Key, "Frosmoth was already in the pool");
            Assert.AreEqual("LOR-18", db.MainPrinting(C("SHF-SV7")).Key, "the Shiny Vault Blipbug is Lost Origin's");
            Assert.AreEqual("SIT-158", db.MainPrinting(C("SHF-59")).Key, "Gym Trainer came with Silver Tempest");
            Assert.AreEqual("SIT-158", db.MainPrinting(C("SHF-68")).Key, "and its full art");
            Assert.AreEqual("SHF-6", db.MainPrinting(C("SHF-SV1")).Key, "the Shiny Vault Rowlet is the same card");
            Assert.AreEqual("SHF-35", db.MainPrinting(C("SHF-36")).Key, "both Morpeko are the same card");
            Assert.AreEqual("SHF-57", db.MainPrinting(C("SHF-65")).Key, "the full-art Ball Guy");
            Assert.AreEqual("SHF-50", db.MainPrinting(C("SHF-SV118")).Key, "the Shiny Vault Ditto V");
            Assert.AreNotEqual("SHF-35", db.MainPrinting(C("SHF-SV44")).Key, "the Shiny Vault Morpeko is a different card");
            Assert.IsFalse(Formats.IsStandardCard(C(DittoV)), "regulation mark D: Expanded");
            Assert.AreEqual("Rare Fossil", C(Dracovish).EvolvesFrom);
        }

        // ================================================================== Your rulings

        [Test]
        public void V_Transformation_Swaps_In_A_Basic_Pokemon_V_And_Discards_Ditto_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Use V Transformation"), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(DittoV));
            Attach(e, ditto, Psychic, 2);
            ditto.DamageCounters = 3;
            e.ScenarioAddToDiscard(0, C(BlisseyV), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Blissey V", ditto.Name, "the same Pokémon in play, now Blissey V");
            Assert.AreEqual(3, ditto.DamageCounters);
            Assert.AreEqual(2, ditto.Energy.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Ditto V"), "your ruling: Ditto V goes to the discard pile");
            Assert.IsFalse(e.State.Players[0].Discard.Any(c => c.Def.Name == "Blissey V"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(DittoV));
            e2.ScenarioAddToDiscard(0, C(DittoV), 1);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Use V Transformation"), "your ruling: not another Ditto V");
        }

        [Test]
        public void Biting_Whirlpool_Works_From_The_Bench_And_Each_Arctozolt_Adds_2_Counters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Psychic Energy"));
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Arctozolt));
            e.ScenarioBench(0, C(Arctozolt));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(1, theirs.Energy.Count);
            Assert.AreEqual(4, theirs.DamageCounters, "your ruling: 2 from each Arctozolt");
        }

        // ================================================================== Pokémon

        [Test]
        public void Max_Transform_Uses_The_Defending_Pokemons_Attack_As_Ditto_VMAX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Max Transform", "Rage"), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(DittoV));
            e.ScenarioEvolve(ditto, C(DittoVmax));
            Attach(e, ditto, Psychic, 3);
            ditto.DamageCounters = 2;
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, tauros.DamageCounters, "Rage: 30 + 10 for each damage counter on Ditto VMAX");
        }

        [Test]
        public void Shuffle_Dance_Swaps_A_Prize_Card_With_The_Top_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shuffle Dance"), new ScriptedAgent());
            e.ScenarioActive(0, C(MrRime));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[1];
            Assert.AreEqual(6, p.Prizes.Count);
            Assert.AreEqual(1, p.Prizes.Count(c => c.Def.Name == "Psychic Energy"), "the old top card is a Prize card");
            Assert.AreEqual("Fire Energy", p.Deck[0].Def.Name, "and a Prize card is on top of the deck");
        }

        [Test]
        public void Neutralizing_Gas_Turns_Off_The_Opponents_Abilities_While_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(GalarianWeezing));
            var mine = e.ScenarioBench(0, C(BlisseyV));
            var tauros = e.ScenarioBench(0, C(Tauros));
            var theirs = e.ScenarioActive(1, C(BlisseyV));
            Assert.IsTrue(e.AbilityBlocked(theirs, "Natural Cure"));
            Assert.IsFalse(e.AbilityBlocked(mine, "Natural Cure"), "only the opponent's");
            e.SwitchActive(0, tauros);
            Assert.IsFalse(e.AbilityBlocked(theirs, "Natural Cure"), "only while Galarian Weezing is Active");
        }

        [Test]
        public void Severe_Poison_Puts_4_Counters_During_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Severe Poison"), new ScriptedAgent());
            var weezing = e.ScenarioActive(0, C(GalarianWeezing));
            Attach(e, weezing, Darkness, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(4, e.PoisonCounters(tauros));
        }

        [Test]
        public void Octolock_Raises_Costs_And_Stops_Retreat_While_Grapploct_Stays_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Octolock", "Attack: Octolock"), new ScriptedAgent());
            var grapploct = e.ScenarioActive(0, C(Grapploct));
            Attach(e, grapploct, Fighting, 2);
            var benched = e.ScenarioBench(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var rage = tauros.Def.Attacks[1];
            Assert.AreEqual(4, e.AttackCost(tauros, rage).Count, "[C][C] more");
            Assert.IsTrue(e.RetreatPrevented(tauros));
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(4, e.AttackCost(tauros, rage).Count, "it can't be applied more than once");
            e.SwitchActive(0, benched);
            Assert.AreEqual(2, e.AttackCost(tauros, rage).Count, "Grapploct left the Active Spot");
            Assert.IsFalse(e.RetreatPrevented(tauros));
        }

        [Test]
        public void Meteor_Assault_Waits_Until_Sirfetchd_Leaves_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Meteor Assault"), new ScriptedAgent());
            var sirfetchd = e.ScenarioActive(0, C(Sirfetchd));
            Attach(e, sirfetchd, Fighting, 3);
            var tauros = e.ScenarioBench(0, C(Tauros));
            var zacian = e.ScenarioActive(1, C(ZacianV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, zacian.DamageCounters);
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 0, "Attack: Meteor Assault"));
            Assert.IsTrue(Offered(e, 0, "Attack: Pierce"));
            e.SwitchActive(0, tauros);
            e.SwitchActive(0, sirfetchd);
            Assert.IsTrue(Offered(e, 0, "Attack: Meteor Assault"));
        }

        [Test]
        public void Steely_Spirit_Adds_20_For_Each_Galarian_Perrserker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Strength"), new ScriptedAgent());
            var cufant = e.ScenarioActive(0, C(Cufant));
            Attach(e, cufant, Metal, 3);
            e.ScenarioBench(0, C(Perrserker));
            e.ScenarioBench(0, C(Perrserker));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, tauros.DamageCounters, "60 + 20 + 20");
        }

        [Test]
        public void Iron_Defense_Formation_Protects_Falinks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var falinks = e.ScenarioActive(0, C(Falinks));
            e.ScenarioBench(0, C(FalinksV));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(1, falinks.DamageCounters, "30 - 20");
        }

        [Test]
        public void Dark_Oath_Makes_The_Opponents_Active_Attacks_Cost_1_More()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var grimmsnarl = e.ScenarioActive(0, C(Grimmsnarl));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(3, e.AttackCost(tauros, tauros.Def.Attacks[1]).Count);
            Assert.AreEqual(3, e.AttackCost(grimmsnarl, grimmsnarl.Def.Attacks[0]).Count, "not our own");
        }

        [Test]
        public void Primal_Law_Stops_The_Opponent_Evolving_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var dracovish = e.ScenarioBench(0, C(Dracovish));
            e.ScenarioActive(1, C(Tepig));
            e.ScenarioAddToHand(1, C(Pignite));
            e.ScenarioSetTurn(1, 3);
            Assert.IsTrue(Offered(e, 1, "Evolve"), "Dracovish on the Bench");
            e.SwitchActive(0, dracovish);
            Assert.IsFalse(Offered(e, 1, "Evolve"), "Dracovish Active");
        }

        [Test]
        public void Ice_Face_Takes_60_Less_Only_At_Full_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var eiscue = e.ScenarioActive(0, C(Eiscue));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(0, eiscue.DamageCounters);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var e2scue = e2.ScenarioActive(0, C(Eiscue));
            e2scue.DamageCounters = 1;
            var t2 = e2.ScenarioActive(1, C(Tauros));
            Attach(e2, t2, Psychic, 2);
            e2.ScenarioSetTurn(1, 3);
            Step(e2);
            Assert.AreEqual(4, e2scue.DamageCounters);
        }

        [Test]
        public void Deep_Forest_Camo_Stops_Damage_From_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Blissful Blast"));
            var decidueye = e.ScenarioActive(0, C(Decidueye));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            Attach(e, blissey, Psychic, 1);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(0, decidueye.DamageCounters);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var d2 = e2.ScenarioActive(0, C(Decidueye));
            var tauros = e2.ScenarioActive(1, C(Tauros));
            Attach(e2, tauros, Psychic, 2);
            e2.ScenarioSetTurn(1, 3);
            Step(e2);
            Assert.AreEqual(3, d2.DamageCounters, "other Pokémon still do damage");
        }

        [Test]
        public void Perish_Body_Knocks_Out_The_Attacker_On_Heads()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var cursola = e.ScenarioActive(0, C(GalarianCursola));
            cursola.DamageCounters = 9;
            e.ScenarioBench(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(1, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Galarian Cursola"));
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Tauros"), "the Attacking Pokémon is Knocked Out too");
            Assert.AreNotEqual(tauros, e.State.Players[1].Active);
        }

        [Test]
        public void Snap_Trap_Discards_An_Energy_From_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rage"));
            var stunfisk = e.ScenarioActive(0, C(GalarianStunfisk));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(3, stunfisk.DamageCounters);
            Assert.AreEqual(1, tauros.Energy.Count);
        }

        [Test]
        public void Libero_Attaches_Fire_Energy_When_Cinderace_Moves_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat Tauros"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            var cinderace = e.ScenarioBench(0, C(Cinderace));
            e.ScenarioAddToDiscard(0, C(Fire), 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(cinderace, e.State.Players[0].Active);
            Assert.AreEqual(2, cinderace.Energy.Count(c => c.Def.Name == "Fire Energy"));
        }

        [Test]
        public void Top_Entry_Puts_Luxio_Onto_The_Bench_When_Drawn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Luxio)));
            e.ScenarioSetTurn(1, 2);
            NextTurn(e);
            Assert.IsTrue(e.State.Players[0].Bench.Any(b => b.Name == "Luxio"));
            Assert.IsFalse(e.State.Players[0].Hand.Any(c => c.Def.Name == "Luxio"));
        }

        [Test]
        public void Raid_Adds_100_Right_After_Evolving_From_Luxio()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Attack: Raid"), new ScriptedAgent());
            var luxio = e.ScenarioActive(0, C(Luxio));
            Attach(e, luxio, Lightning, 1);
            e.ScenarioAddToHand(0, C(Luxray));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(16, blissey.DamageCounters, "60 + 100");
        }

        [Test]
        public void Sky_Circus_Ignores_Energy_Costs_After_Bird_Keeper()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bird Keeper"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var swanna = e.ScenarioBench(0, C(Swanna));
            e.ScenarioAddToHand(0, C(BirdKeeper));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(swanna, e.State.Players[0].Active);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "Bird Keeper draws 3");
            Assert.IsTrue(Offered(e, 0, "Attack: Feather Slice"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Swanna));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Attack: Feather Slice"));
        }

        [Test]
        public void Tar_Generator_Attaches_A_Fire_And_A_Fighting_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Tar Generator"), new ScriptedAgent());
            var coalossal = e.ScenarioActive(0, C(Coalossal));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioAddToDiscard(0, C(Fighting), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, coalossal.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Tea_Break_Discards_A_Mad_Party_Pokemon_That_Mad_Party_Then_Counts()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Tea Break", "Attack: Mad Party"), new ScriptedAgent());
            var polteageist = e.ScenarioActive(0, C(Polteageist));
            Attach(e, polteageist, Psychic, 2);
            e.ScenarioAddToHand(0, C(Dedenne));
            e.ScenarioAddToHand(0, C(Grass));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "discarded Dedenne, drew 2");
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters, "20 for the 1 Mad Party Pokémon in the discard pile");
        }

        [Test]
        public void Anchor_Anger_Adds_90_After_A_Grass_Pokemon_Was_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Anchor Anger"), new ScriptedAgent());
            var dhelmise = e.ScenarioActive(0, C(DhelmiseV));
            Attach(e, dhelmise, Grass, 1);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.State.Players[0].KnockOuts.Add(new KnockOutRecord(2, C(Yanma), true));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters, "30 + 90");
        }

        [Test]
        public void Phantom_Force_Puts_3_Counters_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Phantom Force"), new ScriptedAgent());
            var dragapult = e.ScenarioActive(0, C(Dragapult));
            Attach(e, dragapult, Psychic, 2);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            var benched = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters);
            Assert.AreEqual(3, benched.DamageCounters);
        }

        [Test]
        public void Instigate_Puts_The_Opponents_Hand_On_The_Bottom_And_They_Draw_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Instigate"), new ScriptedAgent());
            var nickit = e.ScenarioActive(0, C(Nickit));
            Attach(e, nickit, Darkness, 1);
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[1];
            Assert.AreEqual(3, p.Hand.Count);
            Assert.IsTrue(p.Deck.Skip(p.Deck.Count - 5).All(c => c.Def.Name == "Grass Energy"), "the old hand is on the bottom");
        }

        [Test]
        public void Amazing_Destruction_Knocks_Out_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Amazing Destruction"), new ScriptedAgent());
            var yveltal = e.ScenarioActive(0, C(Yveltal));
            Attach(e, yveltal, Fire, 1);
            Attach(e, yveltal, Psychic, 1);
            Attach(e, yveltal, Darkness, 1);
            Attach(e, yveltal, Grass, 2);
            e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Blissey V"));
        }

        [Test]
        public void Evolution_Roar_Discards_2_To_Find_Galarian_Perrserker()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Evolution Roar"), new ScriptedAgent());
            e.ScenarioActive(0, C(GalarianMeowth));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToDeck(0, C(Perrserker), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(2, p.Discard.Count(c => c.Def.Name == "Grass Energy"));
            Assert.IsTrue(p.Hand.Any(c => c.Def.Name == "Galarian Perrserker"));
        }

        [Test]
        public void Woodland_Stroll_Only_Works_From_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Woodland Stroll"), new ScriptedAgent());
            e.ScenarioActive(0, C(Celebi));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.IsEnergy));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioBench(0, C(Celebi));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Use Woodland Stroll"));
        }

        [Test]
        public void Pastel_Veil_Keeps_Your_Pokemon_Free_Of_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(GalarianRapidash));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ApplyCondition(mine, SpecialCondition.Poisoned);
            e.ApplyCondition(theirs, SpecialCondition.Poisoned);
            Assert.IsFalse(mine.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(theirs.Has(SpecialCondition.Poisoned), "only your Pokémon");
        }

        [Test]
        public void G_Max_Drill_Adds_Up_To_100_For_Extra_Darkness_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: G-Max Drill"), new ScriptedAgent());
            var grimmsnarl = e.ScenarioActive(0, C(GrimmsnarlV));
            e.ScenarioEvolve(grimmsnarl, C(GrimmsnarlVmax));
            Attach(e, grimmsnarl, Darkness, 6);
            var ditto = e.ScenarioActive(1, C(DittoV));
            e.ScenarioEvolve(ditto, C(DittoVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(27, ditto.DamageCounters, "170 + 100 (3 extra Energy, capped)");
        }

        [Test]
        public void Adornment_Attaches_A_Psychic_Energy_To_Each_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Adornment", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var alcremie = e.ScenarioActive(0, C(AlcremieV));
            e.ScenarioEvolve(alcremie, C(AlcremieVmax));
            Attach(e, alcremie, Psychic, 1);
            var a = e.ScenarioBench(0, C(Tauros));
            var b = e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, a.Energy.Count);
            Assert.AreEqual(1, b.Energy.Count);
        }

        [Test]
        public void G_Max_Whisk_Does_60_For_Each_Energy_Discarded_From_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: G-Max Whisk", "Psychic Energy|Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var alcremie = e.ScenarioActive(0, C(AlcremieV));
            e.ScenarioEvolve(alcremie, C(AlcremieVmax));
            Attach(e, alcremie, Psychic, 2);
            var benched = e.ScenarioBench(0, C(Tauros));
            Attach(e, benched, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, blissey.DamageCounters, "3 x 60");
            Assert.AreEqual(0, alcremie.Energy.Count);
            Assert.AreEqual(0, benched.Energy.Count);
        }

        // ================================================================== Trainers

        [Test]
        public void Ball_Guy_Takes_Ball_Items_With_Different_Names()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ball Guy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(BallGuy));
            e.ScenarioAddToDeck(0, C(PokeBall), 2);
            e.ScenarioAddToDeck(0, C(GreatBall), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(2, hand.Count);
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Poké Ball"));
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Great Ball"));
        }

        [Test]
        public void Rose_Attaches_2_Basic_Energy_To_A_Pokemon_VMAX_And_Discards_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rose"), new ScriptedAgent());
            var rillaboom = e.ScenarioActive(0, C(RillaboomV));
            e.ScenarioEvolve(rillaboom, C(RillaboomVmax));
            e.ScenarioAddToDiscard(0, C(Grass), 2);
            e.ScenarioAddToHand(0, C(Rose));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, rillaboom.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Rusted_Shield_Gives_Zamazenta_V_70_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zamazenta = e.ScenarioActive(0, C(ZamazentaV));
            e.ScenarioAttachTool(zamazenta, C(RustedShield));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(RustedShield));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(290, e.MaxHp(zamazenta));
            Assert.AreEqual(120, e.MaxHp(tauros), "only Zamazenta V");
        }

        [Test]
        public void Rusted_Sword_Adds_30_To_Zacian_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Behemoth Blade"), new ScriptedAgent());
            var zacian = e.ScenarioActive(0, C(ZacianV));
            Attach(e, zacian, Metal, 3);
            e.ScenarioAttachTool(zacian, C(RustedSword));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, blissey.DamageCounters, "100 + 30");
        }

        [Test]
        public void Team_Yell_Towel_Heals_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Yell Towel"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            mine.DamageCounters = 6;
            e.ScenarioAddToHand(0, C(TeamYellTowel));
            var theirs = e.ScenarioActive(1, C(Tauros));
            theirs.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mine.DamageCounters);
            Assert.AreEqual(0, theirs.DamageCounters);
        }
    }
}
