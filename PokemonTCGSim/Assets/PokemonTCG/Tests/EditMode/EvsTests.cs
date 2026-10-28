using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v58: Evolving Skies (EVS 1-237). Your rulings: Dream Ball puts any Pokémon onto the Bench and works whenever you take it
    /// as a face-down Prize card (either player's turn); Bubble Pod takes any Water Pokémon; Zoroark's Phantom Transformation
    /// replacement counts as being in play since Zoroark was; Solar Evolution ignores the usual evolution timing.
    /// </summary>
    public class EvsTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string Tauros = "MEW-128";
        const string Judge = "POR-76";

        const string Pinsir = "EVS-1";
        const string Hoppip = "EVS-2";
        const string Skiploom = "EVS-3";
        const string Jumpluff = "EVS-4";
        const string Pyroar = "EVS-23";
        const string GyaradosV = "EVS-28";
        const string GyaradosVmax = "EVS-29";
        const string VaporeonVmax = "EVS-30";
        const string Lombre = "EVS-33";
        const string Ludicolo = "EVS-34";
        const string Feebas = "EVS-37";
        const string Milotic = "EVS-38";
        const string GlaceonV = "EVS-40";
        const string GlaceonVmax = "EVS-41";
        const string Wishiwashi = "EVS-46";
        const string Drowzee = "EVS-61";
        const string Hitmonchan = "EVS-81";
        const string MedichamV = "EVS-83";
        const string LycanrocV = "EVS-91";
        const string LycanrocVmax = "EVS-92";
        const string GarbodorV = "EVS-100";
        const string GarbodorVmax = "EVS-101";
        const string Zorua = "EVS-102";
        const string Zoroark = "EVS-103";
        const string Herdier = "EVS-134";
        const string Stoutland = "EVS-135";
        const string BoostShake = "EVS-142";
        const string Copycat = "EVS-143";
        const string DreamBall = "EVS-146";
        const string LuckyIcePop = "EVS-150";
        const string SingleScroll = "EVS-158";
        const string SpiritMask = "EVS-160";
        const string TreasureEnergy = "EVS-165";
        const string VaporeonV = "EVS-172";
        const string Inteleon = "EVS-227";
        const string Turffield = "EVS-234";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
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

        /// <summary>Puts a card on top of the player's Prize cards so it's the next one taken.</summary>
        private void PrizeOnTop(GameEngine e, int player, string key)
        {
            var p = e.State.Players[player];
            var card = e.NewCard(player, C(key));
            p.Prizes.Insert(0, card);
        }

        // ================================================================== Data

        [Test]
        public void Every_Evolving_Skies_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 237; n++) Assert.IsNotNull(db.Get("EVS-" + n), "EVS " + n + " is missing");
            Assert.AreEqual("CRZ-101", db.MainPrinting(C("EVS-111")).Key, "Rayquaza VMAX was already in the pool");
            Assert.AreEqual("BRS-TG23", db.MainPrinting(C("EVS-215")).Key, "and Umbreon VMAX");
            Assert.AreEqual("SIT-TG24", db.MainPrinting(C("EVS-149")).Key, "and Gordie");
            Assert.AreEqual("EVS-7", db.MainPrinting(C("EVS-167")).Key, "a full art of Leafeon V");
            Assert.AreEqual("EVS-142", db.MainPrinting(C("EVS-229")).Key, "the gold Boost Shake");
            Assert.IsFalse(Formats.IsStandardCard(C(Pinsir)), "regulation mark E: Expanded");
            Assert.IsTrue(C(Pinsir).IsSingleStrike);
            Assert.IsTrue(C(Inteleon).IsRapidStrike);
            Assert.IsTrue(C(Turffield).IsStadium);
            Assert.IsTrue(C(SingleScroll).IsTool);
            Assert.AreEqual(1, C(SingleScroll).Attacks.Count);
            Assert.AreEqual(0, C("EVS-64").Attacks[0].Damage, "Zen Shot has no printed damage");
            Assert.IsTrue(C(TreasureEnergy).IsSpecialEnergy);
        }

        // ================================================================== Your rulings

        [Test]
        public void Dream_Ball_From_The_Prize_Cards_Puts_Any_Pokemon_Onto_The_Bench_Even_On_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes", "Jumpluff"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(Jumpluff), 1);
            PrizeOnTop(e, 0, DreamBall);
            e.ScenarioSetTurn(1, 4);
            e.TakePrizeCards(0, 1);
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(1, bench.Count, "your ruling: any turn");
            Assert.AreEqual("Jumpluff", bench[0].Name, "your ruling: an Evolution Pokémon goes onto the Bench as it is");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Dream Ball"));
            Assert.IsFalse(e.State.Players[0].Hand.Any(c => c.Def.Name == "Dream Ball"));
        }

        [Test]
        public void Dream_Ball_Cant_Be_Played_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(DreamBall));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Dream Ball"));
        }

        [Test]
        public void Treasure_Energy_From_The_Prize_Cards_Is_Attached_Only_During_Your_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            PrizeOnTop(e, 0, TreasureEnergy);
            e.ScenarioSetTurn(0, 3);
            e.TakePrizeCards(0, 1);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn, "not the attachment for the turn");

            PrizeOnTop(e, 0, TreasureEnergy);
            e.ScenarioSetTurn(1, 4);
            e.TakePrizeCards(0, 1);
            Assert.AreEqual(1, tauros.Energy.Count, "not during the opponent's turn");
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Treasure Energy"));
        }

        [Test]
        public void Bubble_Pod_Benches_An_Evolution_Water_Pokemon_With_Up_To_3_Water_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bubble Pod"), new ScriptedAgent());
            var vaporeon = e.ScenarioActive(0, C(VaporeonV));
            e.ScenarioEvolve(vaporeon, C(VaporeonVmax));
            Attach(e, vaporeon, Water, 1);
            e.ScenarioAddToDiscard(0, C(Milotic), 1);
            e.ScenarioAddToDiscard(0, C(Water), 4);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var milotic = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Milotic", milotic.Name, "your ruling: any Water Pokémon");
            Assert.AreEqual(3, milotic.Energy.Count);
        }

        [Test]
        public void Phantom_Transformation_Puts_The_Stage_1_In_Zoroarks_Place_As_If_It_Had_Been_There()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Phantom Transformation"), new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C(Zorua));
            e.ScenarioEvolve(zoroark, C(Zoroark));
            Attach(e, zoroark, Psychic, 2);
            e.ScenarioAddToDiscard(0, C(Herdier), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 5);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual("Herdier", p.Active.Name);
            Assert.AreEqual(0, p.Active.Energy.Count);
            Assert.IsTrue(p.Discard.Any(c => c.Def.Name == "Zoroark") && p.Discard.Any(c => c.Def.Name == "Zorua"));
            Assert.AreEqual(2, p.Discard.Count(c => c.Def.Name == "Psychic Energy"));
            Assert.AreEqual(zoroark.TurnEnteredPlay, p.Active.TurnEnteredPlay);
            Assert.IsTrue(e.CanEvolveInto(p.Active, C(Stoutland), true), "your ruling: it can evolve this turn, as Zoroark could have");
        }

        [Test]
        public void Solar_Evolution_Evolves_Skiploom_The_Turn_It_Evolved()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Attach Grass Energy", "Yes", "Jumpluff"), new ScriptedAgent());
            var hoppip = e.ScenarioActive(0, C(Hoppip));
            e.ScenarioAddToHand(0, C(Skiploom));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToDeck(0, C(Jumpluff), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Skiploom", hoppip.Name);
            Step(e);
            Assert.AreEqual("Jumpluff", hoppip.Name, "your ruling: Solar Evolution ignores the usual timing");
            Assert.AreEqual(1, hoppip.Energy.Count);
        }

        // ================================================================== Pokémon

        [Test]
        public void Fluffy_Barrage_Attacks_Twice()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spinning Attack", "Attack again"), new ScriptedAgent());
            var jumpluff = e.ScenarioActive(0, C(Jumpluff));
            Attach(e, jumpluff, Grass, 1);
            var gyarados = e.ScenarioActive(1, C(GyaradosV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, gyarados.DamageCounters, "60 twice");
        }

        [Test]
        public void Vise_Coach_Lets_Single_Strike_Attacks_Ignore_Resistance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Clean Hit"), new ScriptedAgent());
            var hitmonchan = e.ScenarioActive(0, C(Hitmonchan));
            Attach(e, hitmonchan, Fighting, 1);
            e.ScenarioBench(0, C(Pinsir));
            var drowzee = e.ScenarioActive(1, C(Drowzee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, drowzee.DamageCounters, "20, Fighting Resistance ignored");
        }

        [Test]
        public void Without_Vise_Coach_Resistance_Applies()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Clean Hit"), new ScriptedAgent());
            var hitmonchan = e.ScenarioActive(0, C(Hitmonchan));
            Attach(e, hitmonchan, Fighting, 1);
            var drowzee = e.ScenarioActive(1, C(Drowzee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, drowzee.DamageCounters, "20 - 30");
        }

        [Test]
        public void Enthusiastic_Dance_Adds_100_To_Basic_Pokemon_Attacks_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes", "Attack: Clean Hit"), new ScriptedAgent());
            var hitmonchan = e.ScenarioActive(0, C(Hitmonchan));
            Attach(e, hitmonchan, Fighting, 1);
            e.ScenarioBench(0, C(Lombre));
            e.ScenarioAddToHand(0, C(Ludicolo));
            var gyarados = e.ScenarioActive(1, C(GyaradosV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(12, gyarados.DamageCounters, "20 + 100");
        }

        [Test]
        public void Dew_Guard_Keeps_The_Hand_Against_The_Opponents_Supporter()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Judge"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Judge));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Milotic));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count, "the Judge player still shuffles and draws 4");
            Assert.AreEqual(3, e.State.Players[1].Hand.Count, "Milotic's player keeps their hand");
            Assert.IsTrue(e.State.Players[1].Hand.All(c => c.Def.Name == "Water Energy"));
        }

        [Test]
        public void Without_Dew_Guard_Judge_Works_On_Both_Players()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Judge"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Judge));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Crystal_Veil_Stops_Damage_From_Pokemon_VMAX_But_Not_Glaceon_VMAX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hyper Beam"), new ScriptedAgent());
            var gyarados = e.ScenarioActive(0, C(GyaradosV));
            e.ScenarioEvolve(gyarados, C(GyaradosVmax));
            Attach(e, gyarados, Water, 3);
            var glaceon = e.ScenarioActive(1, C(GlaceonV));
            e.ScenarioEvolve(glaceon, C(GlaceonVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, glaceon.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Max Icicle"), new ScriptedAgent());
            var attacker = e2.ScenarioActive(0, C(GlaceonV));
            e2.ScenarioEvolve(attacker, C(GlaceonVmax));
            Attach(e2, attacker, Water, 3);
            var defender = e2.ScenarioActive(1, C(GlaceonV));
            e2.ScenarioEvolve(defender, C(GlaceonVmax));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(15, defender.DamageCounters, "except any Glaceon VMAX");
        }

        [Test]
        public void Group_Power_Gives_150_HP_With_3_Water_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wishiwashi = e.ScenarioActive(0, C(Wishiwashi));
            e.ScenarioActive(1, C(Tauros));
            Attach(e, wishiwashi, Water, 2);
            Assert.AreEqual(30, e.MaxHp(wishiwashi));
            Attach(e, wishiwashi, Water, 1);
            Assert.AreEqual(180, e.MaxHp(wishiwashi));
        }

        [Test]
        public void Rubbish_Collecting_Holds_2_Pokemon_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var garbodor = e.ScenarioActive(0, C(GarbodorV));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(1, e.MaxTools(garbodor));
            e.ScenarioEvolve(garbodor, C(GarbodorVmax));
            Assert.AreEqual(2, e.MaxTools(garbodor));
        }

        [Test]
        public void Yoga_Loop_Takes_Another_Turn_After_A_Knock_Out_And_Then_Waits_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Yoga Loop", "Feebas"), new ScriptedAgent());
            var medicham = e.ScenarioActive(0, C(MedichamV));
            Attach(e, medicham, Fighting, 3); // Smash Uppercut costs [F][C][C]
            e.ScenarioActive(1, C(Tauros));
            var feebas = e.ScenarioBench(1, C(Feebas));
            feebas.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[1].Bench.Contains(feebas), "Knocked Out");
            Assert.AreEqual(0, e.State.ExtraTurnFor, "another turn after this one");
            Assert.IsTrue(e.State.SkipCheckup);
            e.State.ExtraTurnFor = -1;
            e.State.SkipCheckup = false;
            e.ScenarioSetTurn(0, 5);
            Assert.IsFalse(Offered(e, 0, "Attack: Yoga Loop"), "used during your last turn");
            Assert.IsTrue(Offered(e, 0, "Attack: Smash Uppercut"));
            e.ScenarioSetTurn(0, 7);
            Assert.IsTrue(Offered(e, 0, "Attack: Yoga Loop"));
        }

        [Test]
        public void Hunting_Claw_Knocks_Out_A_Pokemon_With_60_HP_Or_Less_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hunting Claw", "Feebas"), new ScriptedAgent());
            var lycanroc = e.ScenarioActive(0, C(LycanrocV));
            e.ScenarioEvolve(lycanroc, C(LycanrocVmax));
            Attach(e, lycanroc, Fighting, 1);
            e.ScenarioActive(1, C(Tauros));
            var feebas = e.ScenarioBench(1, C(Feebas));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[1].Bench.Contains(feebas));
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Intimidating_Fang_Takes_30_Off_The_Opponents_Active_Pokemons_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Combustion"), new ScriptedAgent());
            var pyroar = e.ScenarioActive(0, C(Pyroar));
            Attach(e, pyroar, Water, 1);
            var stoutland = e.ScenarioActive(1, C(Stoutland));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, stoutland.DamageCounters, "30 - 30");
        }

        // ================================================================== Trainers

        [Test]
        public void Boost_Shake_Evolves_A_Pokemon_Played_This_Turn_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Hoppip", "Play Boost Shake", "Skiploom"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Hoppip));
            e.ScenarioAddToHand(0, C(BoostShake));
            e.ScenarioAddToDeck(0, C(Skiploom), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.MainPhaseStep(), "your turn ends");
            Assert.AreEqual("Skiploom", e.State.Players[0].Bench.Single().Name);
        }

        [Test]
        public void Lucky_Ice_Pop_Goes_Back_To_The_Hand_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lucky Ice Pop"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 3;
            e.ScenarioAddToHand(0, C(LuckyIcePop));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(1, tauros.DamageCounters);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Lucky Ice Pop"));
            Assert.IsFalse(e.State.Players[0].Discard.Any(c => c.Def.Name == "Lucky Ice Pop"));
        }

        [Test]
        public void Copycat_Draws_As_Many_Cards_As_The_Opponent_Has()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Copycat"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Copycat));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Single_Strike_Scroll_Gives_Its_Attack_Only_To_Single_Strike_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hitmonchan = e.ScenarioActive(0, C(Hitmonchan));
            e.ScenarioAttachTool(hitmonchan, C(SingleScroll));
            Attach(e, hitmonchan, Fighting, 1);
            Attach(e, hitmonchan, Metal, 2);
            Attach(e, hitmonchan, Water, 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Superstrong Slash"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAttachTool(tauros, C(SingleScroll));
            Attach(e2, tauros, Fighting, 1);
            Attach(e2, tauros, Metal, 2);
            Attach(e2, tauros, Water, 2);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Superstrong Slash"), "Tauros isn't a Single Strike Pokémon");
        }

        [Test]
        public void Spirit_Mask_Makes_The_Attacker_Discard_A_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Combustion"), new ScriptedAgent());
            var pyroar = e.ScenarioActive(0, C(Pyroar));
            Attach(e, pyroar, Water, 1);
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(SpiritMask));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "the attacker's player discarded a card");
        }
    }
}
