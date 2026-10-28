using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v152: Delta Reign (DLR), the cards shown so far: Legendary Stadiums (two halves played together as 1 Stadium), Legendary
    /// Pyroduct and Legendary Summit, and the four Mega Evolution Pokémon ex, with your rulings. v152.1: Incarnate Solidarity, Bug
    /// Out, Voltaic Hammer, Buddy Boost, Beat Scales, Rising Heart and Adventure Lantern. v152.4: Rewarm, Brutal Lands, Zinnia's Trust,
    /// Energizing Present. v152.5: Magical Bullet, Eager Fang, Ocean Gain, Thunder-Clad. v152.6: Energy Crush, Aarune, Grisham,
    /// Legendary Abyss.
    /// </summary>
    public class DlrTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Rainbow = "CES-151";

        const string MegaGolurk = "DLR-47";
        const string Golett = "BLK-42";
        const string MegaRayquaza = "DLR-84";
        const string PyroductTop = "DLR-94";
        const string PyroductBottom = "DLR-95";
        const string SummitTop = "DLR-96";
        const string MegaDragonite = "MEP-91";
        const string Electrike = "RS-52";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Delta_Reign_Cards_And_Legendary_Stadium_Halves()
        {
            Assert.AreEqual(38, db.Cards.Count(d => d.SetCode == "DLR"));
            // v153.2: Enamorus 110 is the Delta Reign promo's card, Aarune 139 a print of Aarune 89.
            Assert.AreEqual("MEP-113", db.MainPrinting(C("DLR-110")).Key);
            Assert.AreEqual("DLR-89", db.MainPrinting(C("DLR-139")).Key);
            Assert.AreEqual("Drowzee", C("DLR-44").EvolvesFrom);
            // v152.6: illustration rares are prints of the regular cards.
            Assert.AreEqual("MEP-111", db.MainPrinting(C("DLR-104")).Key);
            Assert.AreEqual("DLR-12", db.MainPrinting(C("DLR-105")).Key);
            Assert.AreEqual("DLR-18", db.MainPrinting(C("DLR-106")).Key);
            Assert.AreEqual("DLR-36", db.MainPrinting(C("DLR-109")).Key);
            Assert.AreEqual("DLR-78", db.MainPrinting(C("DLR-114")).Key);
            Assert.AreEqual("DLR-33", db.MainPrinting(C("DLR-136")).Key);
            Assert.AreEqual("MEP-111", db.MainPrinting(C("DLR-3")).Key, "v152.5: Masquerain 3 is the same card as the promo");
            Assert.AreEqual("MEP-114", db.MainPrinting(C("DLR-75")).Key, "v152.4: Kommo-o 75 is the same card as the Delta Reign promo");
            foreach (var key in new[] { PyroductTop, PyroductBottom, SummitTop, "DLR-97" })
            {
                Assert.IsTrue(C(key).IsStadium && C(key).IsStadiumHalf, key);
                Assert.IsTrue(C(key).IsStadiumCard, "v154.2 ruling: a half is a Stadium card for searches");
                Assert.IsFalse(C(key).IsStadiumAlone, "but it can't be put into play alone");
            }
            Assert.AreEqual(C(PyroductTop).Name, C(PyroductBottom).Name, "same name: 4 copies in all, like LEGEND");
            var rayquaza = C(MegaRayquaza);
            Assert.IsTrue(rayquaza.IsBasicPokemon && rayquaza.IsMegaEx);
            Assert.AreEqual(3, rayquaza.PrizeValue);
            Assert.AreEqual(EnergyType.Colorless, rayquaza.PokemonType);
        }

        [Test]
        public void A_Legendary_Stadium_Needs_Both_Halves_And_Leaves_With_Both()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Legendary Pyroduct"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            var top = e.ScenarioAddToHand(0, C(PyroductTop));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Legendary Pyroduct"), "one half can't be played");
            var bottom = e.ScenarioAddToHand(0, C(PyroductBottom));
            Assert.AreEqual(1, e.GetMainActions(0).Count(o => o.Label.Contains("Play Legendary Pyroduct")));
            e.ScenarioAddToHand(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Chansey));
            Assert.IsNull(Invariants.Check(e, 3), "3 cards each");
            Step(e);
            Assert.AreSame(top, e.State.Stadium);
            Assert.AreSame(bottom, e.State.StadiumPartner);
            Assert.IsNull(Invariants.Check(e, 3), "v154.1: the other half in play is still counted (it was 59 of 60)");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            e.DiscardStadium();
            Assert.IsNull(e.State.Stadium);
            Assert.IsNull(e.State.StadiumPartner);
            Assert.IsTrue(e.State.Players[0].Discard.Contains(top) && e.State.Players[0].Discard.Contains(bottom));
        }

        [Test]
        public void Restricted_Activation_Needs_10_Cards_Unless_Pyroduct_Turns_It_Off()
        {
            foreach (int variant in new[] { 0, 1, 2 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var golurk = e.ScenarioActive(0, C(Golett));
                e.ScenarioEvolve(golurk, C(MegaGolurk));
                Attach(e, golurk, Psychic, 2);
                e.ScenarioActive(1, C(MegaDragonite));
                int hand = variant == 1 ? 10 : 3;
                for (int i = 0; i < hand; i++) e.ScenarioAddToHand(0, C(Fire));
                if (variant == 2) e.ScenarioSetStadium(1, C(PyroductTop));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(variant != 0, Offered(e, 0, "Attack: Goliath's Punch"), "variant " + variant);
            }
        }

        [Test]
        public void Legendary_Summit_Takes_1_Fewer_Prize_For_A_Colorless_Pokemon()
        {
            foreach (bool summit in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Headbutt"), new ScriptedAgent());
                var electrike = e.ScenarioActive(0, C(Electrike));
                Attach(e, electrike, Lightning, 1);
                var rayquaza = e.ScenarioActive(1, C(MegaRayquaza));
                rayquaza.DamageCounters = 27;
                e.ScenarioBench(1, C(Chansey));
                if (summit) e.ScenarioSetStadium(1, C(SummitTop));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(summit ? 4 : 3, e.State.Players[0].Prizes.Count, summit ? "3 - 1 = 2 taken" : "3 taken");
            }
        }

        [Test]
        public void Storm_Emerald_Counts_Fire_And_Lightning_On_All_Your_Pokemon_Rainbow_Once()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Storm Emerald"), new ScriptedAgent());
            var rayquaza = e.ScenarioActive(0, C(MegaRayquaza));
            Attach(e, rayquaza, Fire, 1);
            Attach(e, rayquaza, Lightning, 1);
            Attach(e, rayquaza, Rainbow, 1);
            Attach(e, rayquaza, Psychic, 1);
            var bench = e.ScenarioBench(0, C(Chansey));
            Attach(e, bench, Fire, 1);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(20, dragonite.DamageCounters, "Fire + Lightning + Rainbow (once) + the Benched Fire = 4 x 50");
        }

        [Test]
        public void Rulers_Roar_Attaches_A_Basic_Energy_From_The_Top_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Mega Rayquaza ex", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(MegaRayquaza));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            var rayquaza = e.State.Players[0].Bench.Single();
            Assert.AreEqual(1, rayquaza.Energy.Count);
            Assert.AreEqual(deck - 1, e.State.Players[0].Deck.Count, "the other 3 go to the bottom");
        }

        // ------------------------------------------------------------------ v152.1

        [Test]
        public void Incarnate_Solidarity_Ignores_Colorless_With_All_Four_In_Play()
        {
            foreach (bool all in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C("DLR-85"));
                e.ScenarioBench(0, C("DLR-35"));
                e.ScenarioBench(0, C("DLR-54"));
                if (all) e.ScenarioBench(0, C("MEP-113"));
                e.ScenarioActive(1, C(MegaDragonite));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(all, Offered(e, 0, "Attack: Corkscrew Dive"), all ? "[C][C][C] ignored" : "Enamorus missing");
            }
        }

        [Test]
        public void Bug_Out_Counts_Bug_Out_Pokemon_In_The_Bottom_7_And_Discards_The_Rest()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bug Out"), new ScriptedAgent());
            var spinarak = e.ScenarioActive(0, C("DLR-58"));
            Attach(e, spinarak, Psychic, 3);
            e.ScenarioAddToDeck(0, C("DLR-58"), 1);
            e.ScenarioAddToDeck(0, C("MEP-111"), 1);
            e.ScenarioAddToDeck(0, C(Fire), 1);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, dragonite.DamageCounters, "Spinarak and Masquerain: 2 x 50");
            Assert.AreEqual(18, e.State.Players[0].Deck.Count, "23 cards, the 5 that aren't Pokémon discarded");
            Assert.AreEqual(5, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Voltaic_Hammer_Does_60_For_Each_Basic_Energy_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Voltaic Hammer", "Lightning Energy|Lightning Energy"), new ScriptedAgent());
            var electivire = e.ScenarioActive(0, C("DLR-31"));
            Attach(e, electivire, Lightning, 4);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, dragonite.DamageCounters);
            Assert.AreEqual(2, electivire.Energy.Count);
        }

        [Test]
        public void Buddy_Boost_Attaches_A_Fire_And_A_Lightning_To_Electivire_And_Magmortar()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Buddy Boost", "Fire Energy", "Electivire", "Lightning Energy", "Magmortar"), new ScriptedAgent());
            var magmortar = e.ScenarioActive(0, C("MEP-112"));
            var electivire = e.ScenarioBench(0, C("DLR-31"));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioAddToHand(0, C(Lightning));
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(Fire, electivire.Energy.Single().Def.Key);
            Assert.AreEqual(Lightning, magmortar.Energy.Single().Def.Key);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Beat_Scales_Attaches_Basic_Energy_From_The_Top_6_To_Dragon_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Beat Scales"), new ScriptedAgent());
            var kommo = e.ScenarioActive(0, C("MEP-114"));
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, kommo.Energy.Count);
            Assert.AreEqual(14, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Rising_Heart_Does_100_More_To_A_Pokemon_ex()
        {
            foreach (bool ex in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Rising Heart"), new ScriptedAgent());
                var enamorus = e.ScenarioActive(0, C("MEP-113"));
                Attach(e, enamorus, Psychic, 4);
                var target = e.ScenarioActive(1, C(ex ? MegaDragonite : "RS-14"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(ex ? 20 : 10, target.DamageCounters);
            }
        }

        [Test]
        public void Adventure_Lantern_Finds_A_Basic_Fire_And_A_Basic_Lightning_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Adventure Lantern"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToDeck(0, C(Fire), 1);
            e.ScenarioAddToDeck(0, C(Lightning), 1);
            e.ScenarioAddToHand(0, C("DLR-90"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand.Select(x => x.Def.Key).OrderBy(k => k).ToList();
            CollectionAssert.AreEqual(new[] { Fire, Lightning }, hand);
        }

        // ------------------------------------------------------------------ v152.4

        [Test]
        public void Rewarm_Counts_Basic_Fire_In_The_Discard_And_Shuffles_Them_Back()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rewarm"), new ScriptedAgent());
            var rotom = e.ScenarioActive(0, C("DLR-23"));
            Attach(e, rotom, Fire, 2);
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            e.ScenarioAddToDiscard(0, C(Lightning), 1);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(9, dragonite.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count, "only the Lightning Energy is left");
            Assert.AreEqual(deck + 3, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Brutal_Lands_Does_170_More_With_A_Legendary_Stadium()
        {
            foreach (bool legendary in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Brutal Lands"), new ScriptedAgent());
                var groudon = e.ScenarioActive(0, C("DLR-53"));
                Attach(e, groudon, "MEE-14", 3);
                var dragonite = e.ScenarioActive(1, C(MegaDragonite));
                if (legendary) e.ScenarioSetStadium(1, C(SummitTop));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(legendary ? 27 : 10, dragonite.DamageCounters);
            }
        }

        [Test]
        public void Zinnias_Trust_Switches_And_Moves_An_Energy_To_The_New_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Zinnia's Trust"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Fire, 2);
            var eevee = e.ScenarioBench(0, C("DLR-78"));
            e.ScenarioAddToHand(0, C("DLR-103"));
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(eevee, e.State.Players[0].Active);
            Assert.AreEqual(1, eevee.Energy.Count);
            Assert.AreEqual(1, chansey.Energy.Count);
        }

        [Test]
        public void Energizing_Present_Takes_An_Energy_From_The_Top_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energizing Present"), new ScriptedAgent());
            e.ScenarioActive(0, C("DLR-79"));
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(x => x.Def.IsEnergy));
            Assert.AreEqual(deck - 1, e.State.Players[0].Deck.Count);
        }

        // ------------------------------------------------------------------ v152.5

        [Test]
        public void Magical_Bullet_Also_Hits_A_Damaged_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Magical Bullet"), new ScriptedAgent());
            var meowscarada = e.ScenarioActive(0, C("DLR-14"));
            Attach(e, meowscarada, "MEE-9", 2);
            var active = e.ScenarioActive(1, C(MegaDragonite));
            var undamaged = e.ScenarioBench(1, C("DLR-78"));
            var damaged = e.ScenarioBench(1, C(MegaDragonite));
            damaged.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, active.DamageCounters);
            Assert.AreEqual(13, damaged.DamageCounters);
            Assert.AreEqual(0, undamaged.DamageCounters, "only a Benched Pokémon with damage counters");
        }

        [Test]
        public void Eager_Fang_Does_90_More_With_4_Or_Fewer_Opponent_Prizes()
        {
            foreach (bool late in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Eager Fang"), new ScriptedAgent());
                var arcanine = e.ScenarioActive(0, C("DLR-19"));
                Attach(e, arcanine, Fire, 3);
                var dragonite = e.ScenarioActive(1, C(MegaDragonite));
                if (late) e.State.Players[1].Prizes.RemoveRange(0, 2);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(late ? 18 : 9, dragonite.DamageCounters);
            }
        }

        [Test]
        public void Ocean_Gain_Heals_50_While_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ocean Gain"), new ScriptedAgent());
            var wishiwashi = e.ScenarioActive(0, C("DLR-29"));
            wishiwashi.DamageCounters = 6;
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, wishiwashi.DamageCounters);
        }

        [Test]
        public void Thunder_Clad_Can_Be_Used_On_The_First_Turn_Going_First()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var raikou = e.ScenarioActive(0, C("DLR-33"));
            Attach(e, raikou, Lightning, 3);
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 1);
            Assert.IsTrue(Offered(e, 0, "Attack: Thunder-Clad"));
            Assert.IsFalse(Offered(e, 0, "Attack: Power Rush"), "only Thunder-Clad on the first turn");
        }

        // ------------------------------------------------------------------ v152.6

        [Test]
        public void Energy_Crush_Counts_Energy_On_All_The_Opponents_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Energy Crush"), new ScriptedAgent());
            var pincurchin = e.ScenarioActive(0, C("DLR-36"));
            Attach(e, pincurchin, Lightning, 2);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            Attach(e, dragonite, Fire, 2);
            var bench = e.ScenarioBench(1, C(Chansey));
            Attach(e, bench, Fire, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, dragonite.DamageCounters, "3 Energy x 20");
        }

        [Test]
        public void Grisham_Heals_Everyone_And_Legendary_Abyss_Doubles_It_Alpha_Recovery_Too()
        {
            foreach (bool abyss in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Grisham"), new ScriptedAgent());
                var mine = e.ScenarioActive(0, C(MegaDragonite));
                mine.DamageCounters = 15;
                var bidoof = e.ScenarioBench(0, C("PRC-117"));
                bidoof.DamageCounters = 6;
                var theirs = e.ScenarioActive(1, C(MegaDragonite));
                theirs.DamageCounters = 15;
                e.ScenarioAddToHand(0, C("DLR-93"));
                if (abyss) e.ScenarioSetStadium(1, C("DLR-98"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(abyss ? 5 : 10, mine.DamageCounters);
                Assert.AreEqual(abyss ? 5 : 10, theirs.DamageCounters, "both players' Pokémon");
                Assert.AreEqual(0, bidoof.DamageCounters, "α Recovery: 100 (200 under Legendary Abyss) heals all 60");
            }
        }

        [Test]
        public void Legendary_Abyss_Quadruples_Alpha_Recovery_Healing()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var bidoof = e.ScenarioActive(0, C("PRC-117"));
            bidoof.DamageCounters = 6;
            e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetStadium(1, C("DLR-98"));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(40, e.Heal(bidoof, 10), "10 x 2 (α Recovery) x 2 (Legendary Abyss)");
            Assert.AreEqual(2, bidoof.DamageCounters);
        }

        [Test]
        public void Aarune_Finds_Supporters_And_Legendary_Stadium_Halves_Each_Counting_As_1()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Aarune", "Grisham|Legendary Pyroduct|Legendary Pyroduct"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToDeck(0, C(PyroductTop), 1);
            e.ScenarioAddToDeck(0, C(PyroductBottom), 1);
            e.ScenarioAddToDeck(0, C("DLR-93"), 2);
            e.ScenarioAddToHand(0, C("DLR-89"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(1, hand.Count(x => x.Def.Name == "Grisham"));
            Assert.AreEqual(2, hand.Count(x => x.Def.IsStadiumHalf), "v154.2: both halves, using 2 of the 3 picks");
        }

        // ------------------------------------------------------------------ v153.2

        [Test]
        public void Colorful_Whip_Counts_Each_Different_Type_Revealed_Dual_Types_Both()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Colorful Whip"), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C("DLR-83"));
            Attach(e, kecleon, Fire, 2);
            e.ScenarioAddToHand(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Electrike));
            e.ScenarioAddToHand(0, C("STS-26"));
            e.ScenarioAddToHand(0, C("STS-42"));
            e.ScenarioAddToHand(0, C(Fire));
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, dragonite.DamageCounters, "Colorless, Lightning, Fire + Water, Grass: 5 types x 30 (a bot reveals them all)");
        }

        [Test]
        public void Mind_Ruler_Counts_The_Opponents_Hand_And_Hypnoblast_Puts_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mind Ruler", "Attack: Hypnoblast"), new ScriptedAgent());
            var hypno = e.ScenarioActive(0, C("DLR-44"));
            Attach(e, hypno, Psychic, 2);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, dragonite.DamageCounters, "3 cards x 20");
            dragonite.DamageCounters = 0;
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.IsTrue(dragonite.Has(SpecialCondition.Asleep));
            Assert.AreEqual(5, dragonite.DamageCounters);
        }
    }
}
