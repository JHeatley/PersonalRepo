using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v67: SWSH Black Star Promos (SP, SWSH001-SWSH307). Rulings used: a Pokémon V-UNION is combined from 4 different pieces in
    /// the discard pile, each V-UNION once per game, and a deck plays 1 of each piece (your rulings, v67); a single piece is never put
    /// into play (Compendium 2021-09-30); the throwback promos marked "not legal for official tournaments" are Unlimited only
    /// with their old rules, while Lance's Charizard V and Dark Sylveon V are Expanded (your rulings, v67); the Expanded ban
    /// list (your list, v67); Champions Festival needs exactly 6 Pokémon in play (your ruling, v67); Pulling Currents does
    /// nothing with a full Bench (Compendium 2023-06-01).
    /// </summary>
    public class SpTests : ScenarioTestBase
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
        const string HeatFire = "DAA-174";
        const string SpeedLightning = "RCL-173";
        const string BossOrders = "MEG-114";
        const string ScoopUpNet = "RCL-165";
        const string MedichamV = "EVS-83";

        const string Polteageist = "SP-21";
        const string Toxtricity = "SP-17";
        const string Zacian = "SP-33";
        const string Vaporeon = "SP-72";
        const string VenusaurVmax = "SP-102";
        const string Slowpoke = "SP-126";
        const string Eevee = "SP-127";
        const string Eiscue = "SP-128";
        const string Umbreon = "SP-129";
        const string Dragapult = "SP-132";
        const string LancesCharizard = "SP-133";
        const string DarkSylveon = "SP-134";
        const string ZacianLvX = "SP-135";
        const string Mimikyu = "SP-136";
        const string LightToxtricity = "SP-137";
        const string HydreigonC = "SP-138";
        const string GreninjaStar = "SP-144";
        const string Pikachu = "SP-153";
        const string Espeon = "SP-174";
        const string Flapple = "SP-189";
        const string Leafeon = "SP-191";
        const string Glaceon = "SP-192";
        const string Obstagoon = "SP-193";
        const string Oricorio = "SP-210";
        const string LucarioVstar = "SP-214";
        const string RadiantEevee = "SP-230";
        const string Weavile = "SP-246";
        const string RotomV = "SP-257";
        const string Basculin = "SP-273";
        const string Manaphy = "SP-275";
        const string PikachuVmax = "SP-286";
        const string ChampionsFestival = "SP-296";
        const string Greninja = "SP-305";

        static readonly string[] PikachuUnion = { "SP-139", "SP-140", "SP-141", "SP-142" };
        static readonly string[] GreninjaUnion = { "SP-155", "SP-156", "SP-157", "SP-158" };
        static readonly string[] MewtwoUnion = { "SP-159", "SP-160", "SP-161", "SP-162" };
        static readonly string[] ZacianUnion = { "SP-163", "SP-164", "SP-165", "SP-166" };
        static readonly string[] MorpekoUnion = { "SP-215", "SP-216", "SP-217", "SP-218" };

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

        private void Discard(GameEngine e, int player, params string[] keys)
        {
            foreach (var key in keys) e.ScenarioAddToDiscard(player, C(key), 1);
        }

        /// <summary>A Pokémon V-UNION in play: piece 1 on top of the other 3, as the engine combines them.</summary>
        private PokemonInPlay UnionActive(GameEngine e, int player, string[] pieces)
        {
            var mon = e.ScenarioActive(player, C(pieces[0]));
            for (int k = 1; k < pieces.Length; k++) mon.Stack.Insert(0, e.NewCard(player, C(pieces[k])));
            return mon;
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

        private List<CardDef> DeckWith(params string[] keys)
        {
            var deck = keys.Select(k => C(k)).ToList();
            for (int i = 0; i < 4; i++) deck.Add(C(Tauros));
            while (deck.Count < 60) deck.Add(C(Psychic));
            return deck;
        }

        // ================================================================== Data and formats

        [Test]
        public void Every_SWSH_Promo_Is_In_The_Pool()
        {
            for (int n = 1; n <= 307; n++) Assert.IsNotNull(db.Get("SP-" + n), "SWSH" + n.ToString("000") + " is missing");
            Assert.AreEqual("ASR-TG2", db.MainPrinting(C("SP-22")).Key, "the Flapple promo");
            Assert.AreEqual("SP-143", db.MainPrinting(C("SP-145")).Key, "the second Pikachu V print");
            Assert.AreEqual("SP-127", db.MainPrinting(C("SP-175")).Key, "the Eevee reprint");
            Assert.AreEqual("SP-215", db.MainPrinting(C("SP-287")).Key, "the Morpeko V-UNION reprint");
            Assert.AreEqual("30C-CC26", db.MainPrinting(C("SP-292")).Key);
            Assert.AreEqual(EnergyType.Lightning, C(GreninjaUnion[0]).Weakness, "Limitless leaves out the Weakness");
            Assert.IsTrue(C("SP-20").Attacks.Any(a => a.Name == "Electro Ball"), "Limitless calls it Attack 2");
            Assert.IsFalse(Formats.IsStandardCard(C(Oricorio)), "regulation mark F: Expanded");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Oricorio)));
        }

        [Test]
        public void Throwback_Promos_Are_Unlimited_Only_With_Their_Old_Rules()
        {
            foreach (var key in new[] { Dragapult, ZacianLvX, Mimikyu, LightToxtricity, HydreigonC, GreninjaStar })
            {
                Assert.IsTrue(Formats.IsUnlimitedOnly(C(key)), key + " isn't legal for official tournaments");
                Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(key)), key);
            }
            Assert.IsTrue(Formats.Allows(DeckFormat.Expanded, C(LancesCharizard)), "your ruling: Lance's Charizard V is Expanded");
            Assert.IsTrue(Formats.Allows(DeckFormat.Expanded, C(DarkSylveon)), "your ruling: Dark Sylveon V is Expanded");
            Assert.IsTrue(C(HydreigonC).IsPokemonSP, "Pokémon SP");
            Assert.IsTrue(C(GreninjaStar).IsPokemonStar, "Pokémon ★");
            Assert.IsTrue(C(ZacianLvX).IsLevelUp);
            Assert.AreEqual("Zacian", C(ZacianLvX).EvolvesFrom);

            var expanded = Decklist.Validate(DeckWith(Dragapult), DeckFormat.Expanded);
            Assert.IsTrue(expanded.Any(m => m.Contains("Unlimited only")), string.Join("; ", expanded.ToArray()));
            CollectionAssert.IsEmpty(Decklist.Validate(DeckWith(Dragapult), DeckFormat.Unlimited));
            var stars = Decklist.Validate(DeckWith(GreninjaStar, GreninjaStar), DeckFormat.Unlimited);
            Assert.IsTrue(stars.Any(m => m.Contains("Pokémon ★")), string.Join("; ", stars.ToArray()));
        }

        [Test]
        public void Expanded_Ban_List_Cards_Are_Unlimited_Only()
        {
            foreach (var key in new[] { ScoopUpNet, MedichamV, "ASR-TG2" })
            {
                Assert.IsTrue(Formats.IsExpandedBanned(C(key)), key);
                Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(key)), key);
                Assert.IsFalse(Formats.Allows(DeckFormat.Expanded, C(key)), key);
                Assert.IsTrue(Formats.Allows(DeckFormat.Unlimited, C(key)), key);
            }
            Assert.IsTrue(Formats.IsExpandedBanned(C("SP-22")), "every printing of a banned card is banned");
            Assert.IsFalse(Formats.IsExpandedBanned(C(Tauros)));

            var problems = Decklist.Validate(DeckWith(ScoopUpNet), DeckFormat.Expanded);
            Assert.IsTrue(problems.Any(m => m.Contains("banned in Expanded")), string.Join("; ", problems.ToArray()));
            CollectionAssert.IsEmpty(Decklist.Validate(DeckWith(ScoopUpNet), DeckFormat.Unlimited));
            Assert.AreEqual(DeckFormat.Unlimited, Formats.Needed(DeckWith(MedichamV)));
        }

        // ================================================================== V-UNION

        [Test]
        public void A_V_Union_Piece_Is_A_3_Prize_Pokemon_V_That_Is_Never_Played_On_Its_Own()
        {
            var piece = C(PikachuUnion[0]);
            Assert.IsTrue(piece.IsVUnion && piece.IsMultiCard && piece.IsPokemonV);
            Assert.IsFalse(piece.IsBasicPokemon);
            Assert.AreEqual(3, piece.PrizeValue);

            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, piece);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Bench Pikachu V-UNION"));
            Assert.IsTrue(e.PutIntoPlayBlocked(0, piece), "Compendium: a single piece can't be put into play by an effect");
        }

        [Test]
        public void Deck_Building_Counts_The_Pieces_As_One_Name()
        {
            CollectionAssert.IsEmpty(Decklist.Validate(DeckWith(PikachuUnion), DeckFormat.Expanded), "1 of each piece");
            var five = Decklist.Validate(DeckWith(PikachuUnion.Concat(new[] { PikachuUnion[0] }).ToArray()), DeckFormat.Expanded);
            Assert.IsTrue(five.Any(m => m.Contains("More than 4 copies of Pikachu V-UNION")), string.Join("; ", five.ToArray()));
        }

        [Test]
        public void Combining_Needs_4_Different_Pieces_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Combine Pikachu V-UNION"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Discard(e, 0, PikachuUnion[0], PikachuUnion[1], PikachuUnion[2], PikachuUnion[2]);
            Assert.IsFalse(Offered(e, 0, "Combine Pikachu V-UNION"), "piece 4 is missing");

            Discard(e, 0, PikachuUnion[3]);
            Assert.IsTrue(Offered(e, 0, "Combine Pikachu V-UNION"));
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(1, p.Bench.Count);
            var union = p.Bench[0];
            Assert.AreEqual("Pikachu V-UNION", union.Name);
            Assert.AreEqual(4, union.Stack.Count, "all 4 pieces make up the Pokémon");
            Assert.AreEqual(4, union.Stack.Select(c => c.Def.Key).Distinct().Count());
            Assert.AreEqual(1, p.Discard.Count, "the extra copy of piece 3 stays in the discard pile");
            CollectionAssert.AreEqual(new[] { "Pikachu V-UNION" }, p.VUnionsCombined);
        }

        [Test]
        public void Each_V_Union_Is_Combined_Once_Per_Game()
        {
            // Your ruling (v67): the limit is for each V-UNION, so Mewtwo and Zacian can both be combined in one game.
            var me = new ScriptedAgent().Then("Combine Mewtwo V-UNION");
            var e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Discard(e, 0, MewtwoUnion);
            Discard(e, 0, ZacianUnion);
            Step(e);
            Assert.AreEqual("Mewtwo V-UNION", e.State.Players[0].Bench[0].Name);
            Assert.IsTrue(Offered(e, 0, "Combine Zacian V-UNION"), "a different V-UNION");
            me.Then("Combine Zacian V-UNION");
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
            Assert.AreEqual("Zacian V-UNION", e.State.Players[0].Bench[1].Name);

            Discard(e, 0, MewtwoUnion);
            Assert.IsFalse(Offered(e, 0, "Combine Mewtwo V-UNION"), "Mewtwo V-UNION only once per game");
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 0, "Combine Mewtwo V-UNION"), "not on a later turn either");
        }

        [Test]
        public void Combining_Needs_Room_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Discard(e, 0, GreninjaUnion);
            Assert.IsFalse(Offered(e, 0, "Combine Greninja V-UNION"));
        }

        [Test]
        public void A_Reprinted_Piece_Is_The_Same_Piece()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Discard(e, 0, "SP-287", MorpekoUnion[1], MorpekoUnion[2], MorpekoUnion[3]);
            Assert.IsTrue(Offered(e, 0, "Combine Morpeko V-UNION"), "SWSH287 is piece 1");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Discard(e2, 0, MorpekoUnion[0], "SP-287", MorpekoUnion[1], MorpekoUnion[2]);
            Assert.IsFalse(Offered(e2, 0, "Combine Morpeko V-UNION"), "two copies of piece 1, no piece 4");
        }

        [Test]
        public void A_Knocked_Out_V_Union_Gives_3_Prizes_And_All_Pieces_Go_To_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Claw Slash"), new ScriptedAgent());
            var weavile = e.ScenarioActive(0, C(Weavile));
            Attach(e, weavile, Darkness, 3);
            var mewtwo = UnionActive(e, 1, MewtwoUnion);
            mewtwo.DamageCounters = 20;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Prizes.Count, "3 Prize cards");
            Assert.AreEqual(4, e.State.Players[1].Discard.Count(c => c.Def.Name == "Mewtwo V-UNION"));
        }

        // ================================================================== V-UNION attacks and Abilities

        [Test]
        public void Union_Gain_Attaches_Up_To_2_Basic_Energy_Of_Its_Type()
        {
            PokemonInPlay pikachu, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Union Gain", "Lightning Energy|Lightning Energy"), new ScriptedAgent(), PikachuUnion[0], WailordV, out pikachu, out wailord);
            Attach(e, pikachu, Psychic, 1);
            Discard(e, 0, Lightning, SpeedLightning, Lightning, Lightning);
            Step(e);
            Assert.AreEqual(3, pikachu.Energy.Count);
            Assert.AreEqual(2, pikachu.Energy.Count(c => c.Def.Name == "Lightning Energy"));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Speed Lightning Energy"), "basic Energy only");
        }

        [Test]
        public void Twister_Shuriken_Hits_Only_The_Bench()
        {
            PokemonInPlay greninja, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Twister Shuriken"), new ScriptedAgent(), GreninjaUnion[0], Tauros, out greninja, out tauros);
            Attach(e, greninja, Water, 3);
            var b1 = e.ScenarioBench(1, C(Tauros));
            var b2 = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters);
            Assert.AreEqual(10, b1.DamageCounters);
            Assert.AreEqual(10, b2.DamageCounters);
        }

        [Test]
        public void Antidote_Jutsu_Stops_Poison()
        {
            PokemonInPlay toxtricity, greninja;
            var e = Board(new ScriptedAgent().Then("Attack: Venom Slap"), new ScriptedAgent(), Toxtricity, GreninjaUnion[0], out toxtricity, out greninja);
            Attach(e, toxtricity, Lightning, 3);
            Step(e);
            Assert.AreEqual(24, greninja.DamageCounters, "120 x 2 (Weakness)");
            Assert.IsFalse(greninja.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Feel_The_Way_Needs_Cards_In_The_Opponents_Hand_And_Is_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Feel the Way"), new ScriptedAgent());
            e.ScenarioActive(0, C(GreninjaUnion[0]));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Feel the Way"), "the opponent's hand is empty");
            e.ScenarioAddToHand(1, C(Fire));
            Assert.IsTrue(Offered(e, 0, "Use Feel the Way"));
            Step(e);
            Assert.IsFalse(Offered(e, 0, "Use Feel the Way"), "once during your turn");
        }

        [Test]
        public void Psysplosion_Puts_16_Damage_Counters_And_Super_Regeneration_Heals_200()
        {
            PokemonInPlay mewtwo, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Psysplosion"), new ScriptedAgent(), MewtwoUnion[0], WailordV, out mewtwo, out wailord);
            Attach(e, mewtwo, Psychic, 3);
            Step(e);
            Assert.AreEqual(16, wailord.DamageCounters);

            PokemonInPlay m2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Super Regeneration"), new ScriptedAgent(), MewtwoUnion[0], WailordV, out m2, out w2);
            Attach(e2, m2, Psychic, 3);
            m2.DamageCounters = 25;
            Step(e2);
            Assert.AreEqual(5, m2.DamageCounters);
        }

        [Test]
        public void Photon_Barrier_Stops_The_Effects_Of_Attacks_But_Not_The_Damage()
        {
            PokemonInPlay umbreon, mewtwo;
            var e = Board(new ScriptedAgent().Then("Attack: Moon Mirage"), new ScriptedAgent(), Umbreon, MewtwoUnion[0], out umbreon, out mewtwo);
            Attach(e, umbreon, Darkness, 3);
            Step(e);
            Assert.AreEqual(16, mewtwo.DamageCounters, "80 x 2 (Weakness)");
            Assert.IsFalse(mewtwo.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Master_Blade_Discards_3_Energy()
        {
            PokemonInPlay zacian, venusaur;
            var e = Board(new ScriptedAgent().Then("Attack: Master Blade"), new ScriptedAgent(), ZacianUnion[0], VenusaurVmax, out zacian, out venusaur);
            Attach(e, zacian, Metal, 4);
            e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(1, zacian.Energy.Count);
            Assert.AreEqual(3, e.State.Players[0].Prizes.Count, "340 Knocks Out the Pokémon VMAX");
        }

        [Test]
        public void Burst_Wheel_Does_100_For_Each_Energy_Discarded_And_All_You_Can_Eat_Draws_To_10()
        {
            PokemonInPlay morpeko, venusaur;
            var e = Board(new ScriptedAgent().Then("Attack: Burst Wheel"), new ScriptedAgent(), MorpekoUnion[0], VenusaurVmax, out morpeko, out venusaur);
            Attach(e, morpeko, Lightning, 2);
            Attach(e, morpeko, Psychic, 1);
            Step(e);
            Assert.AreEqual(30, venusaur.DamageCounters, "3 Energy x 100");
            Assert.AreEqual(0, morpeko.Energy.Count);
            Assert.AreEqual(3, e.State.Players[0].Discard.Count);

            PokemonInPlay m2, v2;
            var e2 = Board(new ScriptedAgent().Then("Attack: All You Can Eat"), new ScriptedAgent(), MorpekoUnion[0], VenusaurVmax, out m2, out v2);
            Attach(e2, m2, Lightning, 2);
            for (int i = 0; i < 3; i++) e2.ScenarioAddToHand(0, C(Fire));
            Step(e2);
            Assert.AreEqual(10, e2.State.Players[0].Hand.Count);
        }

        // ================================================================== Pokémon

        [Test]
        public void Teapot_Of_Surprises_Puts_A_Random_Card_From_The_Attackers_Hand_On_The_Bottom_Of_The_Deck()
        {
            PokemonInPlay umbreon, polteageist;
            var e = Board(new ScriptedAgent().Then("Attack: Moon Mirage"), new ScriptedAgent(), Umbreon, Polteageist, out umbreon, out polteageist);
            Attach(e, umbreon, Darkness, 3);
            e.ScenarioAddToHand(0, C(Tauros));
            Step(e);
            Assert.AreEqual(16, polteageist.DamageCounters);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.AreEqual("Tauros", e.State.Players[0].Deck.Last().Def.Name, "on the bottom of the deck");
        }

        [Test]
        public void Watery_Shroud_Protects_Vaporeon_On_The_Bench()
        {
            PokemonInPlay greninja, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Coordinated Shuriken"), new ScriptedAgent(), Greninja, WailordV, out greninja, out wailord);
            Attach(e, greninja, Water, 3);
            var vaporeon = e.ScenarioBench(1, C(Vaporeon));
            var tauros = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters);
            Assert.AreEqual(0, vaporeon.DamageCounters);
            Assert.AreEqual(1, tauros.DamageCounters);
        }

        [Test]
        public void Bladed_Armament_Ignores_Effects_On_The_Defending_Pokemon_For_Its_Own_And_Inherited_Attacks()
        {
            PokemonInPlay plain, w0;
            var e0 = Board(new ScriptedAgent().Then("Attack: Crunch"), new ScriptedAgent(), Zacian, WailordV, out plain, out w0);
            Attach(e0, plain, Metal, 3);
            e0.AddEffect(new DamageTakenEffect("Test shield", 1, w0.Uid, e0.State.TurnNumber + 1, -30));
            e0.State.Rng = new Rng(SeedFor(false));
            Step(e0);
            Assert.AreEqual(3, w0.DamageCounters, "Zacian alone: 60 - 30");

            PokemonInPlay zacian, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Crunch"), new ScriptedAgent(), Zacian, WailordV, out zacian, out wailord);
            e.ScenarioEvolve(zacian, C(ZacianLvX));
            Attach(e, zacian, Metal, 3);
            e.AddEffect(new DamageTakenEffect("Test shield", 1, wailord.Uid, e.State.TurnNumber + 1, -30));
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(6, wailord.DamageCounters, "Crunch from the previous Level ignores the shield");

            PokemonInPlay z2, w2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Brave Blade"), new ScriptedAgent(), Zacian, WailordV, out z2, out w2);
            e2.ScenarioEvolve(z2, C(ZacianLvX));
            Attach(e2, z2, Metal, 3);
            e2.AddEffect(new DamageTakenEffect("Test shield", 1, w2.Uid, e2.State.TurnNumber + 1, -30));
            Step(e2);
            Assert.AreEqual(24, w2.DamageCounters);
        }

        [Test]
        public void Shadow_Knife_Puts_A_Damage_Counter_When_Greninja_Star_Is_Benched_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Greninja"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(GreninjaStar));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
            Assert.AreEqual(1, tauros.DamageCounters);
        }

        [Test]
        public void Everyone_Laze_Around_Heals_10_From_Each_Of_Your_Pokemon()
        {
            PokemonInPlay slowpoke, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Everyone Laze Around"), new ScriptedAgent(), Slowpoke, Tauros, out slowpoke, out tauros);
            Attach(e, slowpoke, Psychic, 1);
            slowpoke.DamageCounters = 2;
            var benched = e.ScenarioBench(0, C(Tauros));
            benched.DamageCounters = 3;
            Step(e);
            Assert.AreEqual(1, slowpoke.DamageCounters);
            Assert.AreEqual(2, benched.DamageCounters);
        }

        [Test]
        public void Be_Prepared_Attaches_A_Basic_Energy_From_The_Hand()
        {
            PokemonInPlay eevee, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Be Prepared"), new ScriptedAgent(), Eevee, Tauros, out eevee, out tauros);
            Attach(e, eevee, Psychic, 1);
            e.ScenarioAddToHand(0, C(HeatFire));
            e.ScenarioAddToHand(0, C(Fire));
            Step(e);
            Assert.AreEqual(2, eevee.Energy.Count);
            Assert.AreEqual("Heat Fire Energy", e.State.Players[0].Hand.Single().Def.Name, "basic Energy only");
        }

        [Test]
        public void Ice_Bonus_Draws_3_Only_If_A_Water_Energy_Is_Discarded()
        {
            PokemonInPlay eiscue, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Ice Bonus"), new ScriptedAgent(), Eiscue, Tauros, out eiscue, out tauros);
            Attach(e, eiscue, Water, 1);
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Fire));
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
            Assert.AreEqual("Water Energy", e.State.Players[0].Discard.Single().Def.Name);

            PokemonInPlay e2Eiscue, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Ice Bonus"), new ScriptedAgent(), Eiscue, Tauros, out e2Eiscue, out t2);
            Attach(e2, e2Eiscue, Water, 1);
            e2.ScenarioAddToHand(0, C(Fire));
            Step(e2);
            Assert.AreEqual(1, e2.State.Players[0].Hand.Count);
        }

        [Test]
        public void Blindside_Only_Hits_A_Damaged_Pokemon()
        {
            PokemonInPlay umbreon, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Blindside"), new ScriptedAgent(), Umbreon, Tauros, out umbreon, out tauros);
            Attach(e, umbreon, Darkness, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            benched.DamageCounters = 1;
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters);
            Assert.AreEqual(7, benched.DamageCounters);

            PokemonInPlay u2, t2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Blindside"), new ScriptedAgent(), Umbreon, Tauros, out u2, out t2);
            Attach(e2, u2, Darkness, 1);
            Step(e2);
            Assert.AreEqual(0, t2.DamageCounters);
        }

        [Test]
        public void Wet_Claw_Returns_An_Energy_To_The_Opponents_Hand()
        {
            PokemonInPlay mimikyu, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Wet Claw"), new ScriptedAgent(), Mimikyu, Tauros, out mimikyu, out tauros);
            Attach(e, mimikyu, Water, 2);
            Attach(e, tauros, Fire, 2);
            Step(e);
            Assert.AreEqual(4, tauros.DamageCounters);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Slow_Ballad_Heals_Both_Active_Pokemon()
        {
            PokemonInPlay toxtricity, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Slow Ballad"), new ScriptedAgent(), LightToxtricity, Tauros, out toxtricity, out tauros);
            Attach(e, toxtricity, Lightning, 1);
            toxtricity.DamageCounters = 4;
            tauros.DamageCounters = 5;
            Step(e);
            Assert.AreEqual(1, toxtricity.DamageCounters);
            Assert.AreEqual(2, tauros.DamageCounters);
        }

        [Test]
        public void Psychic_Adds_40_For_Each_Energy_On_The_Defending_Pokemon()
        {
            PokemonInPlay espeon, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Psychic"), new ScriptedAgent(), Espeon, WailordV, out espeon, out wailord);
            Attach(e, espeon, Psychic, 2);
            Attach(e, wailord, Water, 2);
            Step(e);
            Assert.AreEqual(9, wailord.DamageCounters, "10 + 2 x 40");
        }

        [Test]
        public void Flight_Up_Attaches_Up_To_3_Basic_Energy_To_A_Benched_Pokemon()
        {
            PokemonInPlay flapple, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Flight Up", "Fire Energy|Fire Energy|Water Energy"), new ScriptedAgent(), Flapple, Tauros, out flapple, out tauros);
            Attach(e, flapple, Fire, 1);
            var benched = e.ScenarioBench(0, C(Tauros));
            Discard(e, 0, Fire, HeatFire, Fire, Fire, Water);
            Step(e);
            Assert.AreEqual(3, benched.Energy.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count, "a Fire Energy and Heat Fire Energy (not basic) are left");
        }

        [Test]
        public void Grass_Knot_Adds_30_For_Each_Colorless_In_The_Retreat_Cost()
        {
            PokemonInPlay leafeon, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Grass Knot"), new ScriptedAgent(), Leafeon, WailordV, out leafeon, out wailord);
            Attach(e, leafeon, Grass, 2);
            Step(e);
            Assert.AreEqual(17, wailord.DamageCounters, "50 + 4 x 30");
        }

        [Test]
        public void Hail_Does_20_To_Each_Opposing_Pokemon()
        {
            PokemonInPlay glaceon, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Hail"), new ScriptedAgent(), Glaceon, Tauros, out glaceon, out tauros);
            Attach(e, glaceon, Water, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters);
            Assert.AreEqual(2, benched.DamageCounters);
        }

        [Test]
        public void Rampaging_Kick_Discards_2_Darkness_Energy()
        {
            PokemonInPlay obstagoon, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Rampaging Kick"), new ScriptedAgent(), Obstagoon, WailordV, out obstagoon, out wailord);
            Attach(e, obstagoon, Darkness, 3);
            Attach(e, obstagoon, Psychic, 1);
            Step(e);
            Assert.AreEqual(18, wailord.DamageCounters);
            Assert.AreEqual(2, obstagoon.Energy.Count);
            Assert.IsTrue(obstagoon.Energy.Any(c => c.Def.Name == "Psychic Energy"), "only Darkness Energy is discarded");
        }

        [Test]
        public void Mixed_Call_Finds_A_Pokemon_And_A_Supporter()
        {
            PokemonInPlay oricorio, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Mixed Call"), new ScriptedAgent(), Oricorio, Tauros, out oricorio, out tauros);
            Attach(e, oricorio, Psychic, 1);
            e.ScenarioAddToDeck(0, C(Tauros), 1);
            e.ScenarioAddToDeck(0, C(BossOrders), 1);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(2, hand.Count);
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Tauros"));
            Assert.IsTrue(hand.Any(c => c.Def.IsSupporter));
        }

        [Test]
        public void Fighting_Knuckle_Adds_120_Against_A_Pokemon_V_And_Aura_Star_Counts_All_Opposing_Energy()
        {
            PokemonInPlay lucario, venusaur;
            var e = Board(new ScriptedAgent().Then("Attack: Fighting Knuckle"), new ScriptedAgent(), LucarioVstar, VenusaurVmax, out lucario, out venusaur);
            Attach(e, lucario, Fighting, 3);
            Step(e);
            Assert.AreEqual(24, venusaur.DamageCounters, "120 + 120");

            PokemonInPlay l2, v2;
            var e2 = Board(new ScriptedAgent().Then("Attack: Aura Star"), new ScriptedAgent(), LucarioVstar, VenusaurVmax, out l2, out v2);
            Attach(e2, l2, Fighting, 2);
            Attach(e2, v2, Grass, 2);
            var benched = e2.ScenarioBench(1, C(Tauros));
            Attach(e2, benched, Fire, 1);
            Step(e2);
            Assert.AreEqual(21, v2.DamageCounters, "3 Energy x 70");
            Assert.IsTrue(e2.State.Players[0].VstarUsed);
        }

        [Test]
        public void Twinkle_Gathering_Searches_For_As_Many_Cards_As_Types_In_Play()
        {
            var me = new ScriptedAgent().Then("Attack: Twinkle Gathering");
            PokemonInPlay eevee, tauros;
            var e = Board(me, new ScriptedAgent(), RadiantEevee, Tauros, out eevee, out tauros);
            Attach(e, eevee, Psychic, 1);
            e.ScenarioBench(0, C(Pikachu));
            e.ScenarioBench(0, C(Pikachu));
            e.ScenarioBench(0, C(Vaporeon));
            Step(e);
            var search = me.Seen.FirstOrDefault(r => r.Prompt != null && r.Prompt.StartsWith("Twinkle Gathering"));
            Assert.IsNotNull(search);
            Assert.AreEqual(3, search.MaxChoices, "Colorless, Lightning and Water");
        }

        [Test]
        public void Pursuit_Claw_Does_20_For_Each_Damage_Counter_On_A_Benched_Pokemon()
        {
            PokemonInPlay weavile, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Pursuit Claw"), new ScriptedAgent(), Weavile, Tauros, out weavile, out tauros);
            Attach(e, weavile, Darkness, 1);
            var benched = e.ScenarioBench(1, C(Tauros));
            benched.DamageCounters = 3;
            Step(e);
            Assert.AreEqual(9, benched.DamageCounters, "3 + 3 x 20 / 10");
            Assert.AreEqual(0, tauros.DamageCounters);
        }

        [Test]
        public void Lost_Hack_Puts_A_Special_Energy_In_The_Lost_Zone()
        {
            PokemonInPlay rotom, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Lost Hack"), new ScriptedAgent(), RotomV, Tauros, out rotom, out tauros);
            Attach(e, rotom, Lightning, 1);
            Attach(e, tauros, Fire, 1);
            Attach(e, tauros, SpeedLightning, 1);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual("Speed Lightning Energy", e.State.Players[1].LostZone.Single().Def.Name);
        }

        [Test]
        public void Submerge_Silently_Is_Only_For_The_Second_Players_First_Turn_And_Stops_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Submerge Silently"), new ScriptedAgent().Then("Attack: Moon Mirage"));
            var basculin = e.ScenarioActive(0, C(Basculin));
            Attach(e, basculin, Psychic, 1);
            var umbreon = e.ScenarioActive(1, C(Umbreon));
            Attach(e, umbreon, Darkness, 3);
            e.ScenarioSetTurn(0, 2);
            Assert.IsTrue(Offered(e, 0, "Attack: Submerge Silently"), "going second, first turn");
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(0, basculin.DamageCounters);
            Assert.AreEqual(basculin, e.State.Players[0].Active);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var b2 = e2.ScenarioActive(0, C(Basculin));
            Attach(e2, b2, Psychic, 1);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 4);
            Assert.IsFalse(Offered(e2, 0, "Attack: Submerge Silently"), "not a later turn");
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Attack: Submerge Silently"), "not when going first");
        }

        [Test]
        public void Pulling_Currents_Benches_Basic_Pokemon_From_The_Opponents_Hand()
        {
            PokemonInPlay manaphy, tauros;
            var e = Board(new ScriptedAgent().Then("Attack: Pulling Currents", "Tauros|Tauros"), new ScriptedAgent(), Manaphy, Tauros, out manaphy, out tauros);
            Attach(e, manaphy, Water, 1);
            e.ScenarioAddToHand(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioAddToHand(1, C(PikachuUnion[0]));
            Step(e);
            Assert.AreEqual(2, e.State.Players[1].Bench.Count);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count, "the Energy and the V-UNION piece stay");
        }

        [Test]
        public void Pulling_Currents_Does_Nothing_With_A_Full_Bench()
        {
            var me = new ScriptedAgent().Then("Attack: Pulling Currents");
            PokemonInPlay manaphy, tauros;
            var e = Board(me, new ScriptedAgent(), Manaphy, Tauros, out manaphy, out tauros);
            Attach(e, manaphy, Water, 1);
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Tauros));
            Step(e);
            Assert.AreEqual(5, e.State.Players[1].Bench.Count);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.IsFalse(me.Seen.Any(r => r.Prompt != null && r.Prompt.StartsWith("Pulling Currents")), "no choice to make");
        }

        [Test]
        public void Tail_Charge_Attaches_Up_To_3_Lightning_Energy_To_Any_Of_Your_Pokemon()
        {
            PokemonInPlay pikachu, wailord;
            var e = Board(new ScriptedAgent().Then("Attack: Tail Charge", "Bench 1", "Lightning Energy|Lightning Energy|Lightning Energy"), new ScriptedAgent(), PikachuVmax, WailordV, out pikachu, out wailord);
            Attach(e, pikachu, Lightning, 1);
            var benched = e.ScenarioBench(0, C(Tauros));
            Discard(e, 0, Lightning, Lightning, Lightning, Lightning);
            Step(e);
            Assert.AreEqual(6, wailord.DamageCounters, "30 x 2 (Weakness)");
            Assert.AreEqual(3, benched.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
        }

        // ================================================================== Trainers

        [Test]
        public void Champions_Festival_Needs_Exactly_6_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Champions Festival"), new ScriptedAgent());
            var mine = new List<PokemonInPlay> { e.ScenarioActive(0, C(Tauros)) };
            for (int i = 0; i < 5; i++) mine.Add(e.ScenarioBench(0, C(Tauros)));
            foreach (var mon in mine) mon.DamageCounters = 2;
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(ChampionsFestival));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(mine.All(m => m.DamageCounters == 1), "10 healed from each");
            Assert.IsFalse(Offered(e, 0, "Use Champions Festival"), "once during each player's turn");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e2.ScenarioActive(0, C(Tauros));
            active.DamageCounters = 2;
            for (int i = 0; i < 4; i++) e2.ScenarioBench(0, C(Tauros));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetStadium(0, C(ChampionsFestival));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Use Champions Festival"), "5 Pokémon in play");
        }
    }
}
