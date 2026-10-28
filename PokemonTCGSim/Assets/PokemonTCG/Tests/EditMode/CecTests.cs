using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v68: Cosmic Eclipse (CEC 1-271), plus the Sun &amp; Moon basic Fairy Energy (your call). Rulings used: Lana's Fishing Rod
    /// works with only a Pokémon or only a Pokémon Tool card in the discard pile; Light of the Protector-GX protects every one of
    /// your Pokémon during the opponent's next turn (your rulings, v68); TAG TEAM Supporters resolve in printed order, with the
    /// "when you play this card" discard made as they're played; Will isn't used on a Special Condition's coin; Chaotic Swell's
    /// discarded Stadium still used up the Stadium play; Island Challenge Amulet's reduction is only for Knock Outs by damage
    /// from the opponent's attacks; each Pokémon Roxie discards uses its own Blow-Away Bomb (Compendium, 2019-10-31).
    /// </summary>
    public class CecTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Tauros = "MEW-128";
        const string FairyEnergy = "SUM-Y";
        const string GlimwoodTangle = "DAA-162";

        const string VenusaurSnivy = "CEC-1";
        const string Buzzwole = "CEC-21";
        const string Victini = "CEC-30";
        const string BlastoisePiplup = "CEC-38";
        const string Wishiwashi = "CEC-62";
        const string SolgaleoLunala = "CEC-75";
        const string Koffing = "CEC-76";
        const string Weezing = "CEC-77";
        const string Duskull = "CEC-83";
        const string Sawsbuck = "CEC-16";
        const string MimikyuShadowBox = "CEC-97";
        const string NaganadelGuzzlord = "CEC-158";
        const string Drampa = "CEC-159";
        const string Bewear = "CEC-182";
        const string TypeNull = "CEC-183";
        const string Silvally = "CEC-184";
        const string Beastite = "CEC-185";
        const string Bellelba = "CEC-186";
        const string ChaoticSwell = "CEC-187";
        const string Clay = "CEC-188";
        const string CynthiaCaitlin = "CEC-189";
        const string DragoniumZ = "CEC-190";
        const string Erika = "CEC-191";
        const string GreatCatcher = "CEC-192";
        const string Amulet = "CEC-194";
        const string FishingRod = "CEC-195";
        const string FullForce = "CEC-196";
        const string PokeDoll = "CEC-197";
        const string MallowLana = "CEC-198";
        const string MistyLorelei = "CEC-199";
        const string NsResolve = "CEC-200";
        const string OaksSetup = "CEC-201";
        const string RedBlue = "CEC-202";
        const string RollerSkater = "CEC-203";
        const string Rosa = "CEC-204";
        const string Roxie = "CEC-205";
        const string TagCall = "CEC-206";
        const string Will = "CEC-208";
        const string DrawEnergy = "CEC-209";
        const string GiantHearth = "CEC-263";
        const string Dojo = "CEC-268";
        const string PowerPlant = "CEC-269";

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

        private void Hand(GameEngine e, int player, params string[] keys)
        {
            foreach (var key in keys) e.ScenarioAddToHand(player, C(key));
        }

        /// <summary>Puts a card on top of a player's deck.</summary>
        private void OnTop(GameEngine e, int player, string key)
        {
            e.State.Players[player].Deck.Insert(0, e.NewCard(player, C(key)));
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

        private static int CountNamed(List<CardInstance> pile, string name)
        {
            return pile.Count(x => x.Def.Name == name);
        }

        // ================================================================== Data and formats

        [Test]
        public void Every_Cosmic_Eclipse_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 271; n++) Assert.IsNotNull(db.Get("CEC-" + n), "CEC " + n + " is missing");
            Assert.AreEqual("SIT-165", db.MainPrinting(C("CEC-207")).Key, "Unidentified Fossil is already in the pool");
            Assert.AreEqual(VenusaurSnivy, db.MainPrinting(C("CEC-210")).Key, "full art");
            Assert.AreEqual(Amulet, db.MainPrinting(C("CEC-265")).Key, "gold");
            Assert.AreEqual("Unidentified Fossil", C("CEC-10").EvolvesFrom, "Lileep");
            Assert.IsFalse(Formats.IsStandardCard(C(Drampa)), "no regulation mark");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Drampa)));

            var fairy = C(FairyEnergy);
            Assert.IsTrue(fairy.IsBasicEnergy, "your call: the Sun & Moon basic Fairy Energy");
            Assert.AreEqual(EnergyType.Fairy, fairy.EnergyProvides);
        }

        [Test]
        public void Duskull_And_Island_Challenge_Amulet_Are_Banned_In_Expanded()
        {
            foreach (var key in new[] { Duskull, Amulet, "CEC-265" })
            {
                Assert.IsTrue(Formats.IsExpandedBanned(C(key)), key);
                Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(key)), key);
            }
            Assert.IsFalse(Formats.IsExpandedBanned(C("CEC-84")), "Dusclops");
        }

        [Test]
        public void Tag_Team_And_Ultra_Beast_Cards_Are_Marked()
        {
            var tag = C(BlastoisePiplup);
            Assert.IsTrue(tag.IsTagTeam && tag.IsGX && tag.IsGXorEX);
            Assert.AreEqual(3, tag.PrizeValue, "a TAG TEAM gives 3 Prize cards");
            Assert.IsTrue(C(NaganadelGuzzlord).IsTagTeam && C(NaganadelGuzzlord).IsUltraBeast);
            foreach (var key in new[] { Buzzwole, "CEC-104", "CEC-136", "CEC-164" }) Assert.IsTrue(C(key).IsUltraBeast, key);
            Assert.IsFalse(C(Drampa).IsUltraBeast);
            Assert.IsTrue(C(CynthiaCaitlin).IsTagTeam && C(CynthiaCaitlin).IsSupporter, "TAG TEAM Supporter");
            Assert.IsFalse(C(Erika).IsTagTeam);
            Assert.AreEqual(2, C(Silvally).PrizeValue);

            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mon = e.ScenarioActive(0, tag);
            Assert.AreEqual(3, e.PrizesFor(mon));
        }

        // ================================================================== Lillie's Poké Doll

        [Test]
        public void Poke_Doll_Is_A_30_HP_Item_Pokemon_With_No_Prize_That_Goes_To_The_Deck_Bottom_From_The_Active_Spot()
        {
            var me = new ScriptedAgent().Then("Bench Lillie's Poké Doll");
            var e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            var card = e.ScenarioAddToHand(0, C(PokeDoll));
            Assert.IsTrue(card.Def.IsItem);
            Assert.IsFalse(card.Def.IsBasicPokemon, "not a Basic Pokémon card");
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            var doll = p.Bench.Single();
            Assert.AreEqual(30, e.MaxHp(doll));
            Assert.AreEqual(0, e.PrizesFor(doll), "no Prize cards for it");
            Assert.IsFalse(Offered(e, 0, "on the bottom of your deck"), "only from the Active Spot");

            e.SwitchActive(0, doll);
            Assert.IsFalse(Offered(e, 0, "Retreat"), "it can't retreat");
            Assert.IsTrue(Offered(e, 0, "Put Lillie's Poké Doll on the bottom of your deck"));
            me.Then("Put Lillie's Poké Doll on the bottom of your deck");
            Step(e);
            Assert.AreEqual("Lillie's Poké Doll", p.Deck.Last().Def.Name);
            Assert.IsNotNull(p.Active);
            Assert.AreEqual("Tauros", p.Active.Name, "the Benched Pokémon comes up");
        }

        // ================================================================== Trainers

        [TestCase(true)]
        [TestCase(false)]
        public void Will_Chooses_The_First_Coin_Of_The_Next_Flip(bool heads)
        {
            PokemonInPlay bewear, target;
            var e = Board(new ScriptedAgent().Then("Play Will", "Attack: Lariat", heads ? "Heads" : "Tails"), new ScriptedAgent(), Bewear, Wishiwashi, out bewear, out target);
            Attach(e, bewear, Psychic, 3);
            Hand(e, 0, Will);
            Step(e);
            Assert.IsTrue(e.State.Players[0].WillCoinChoice);
            Step(e);
            Assert.AreEqual(heads ? 12 : 0, target.DamageCounters);
            Assert.IsFalse(e.State.Players[0].WillCoinChoice, "used up");
        }

        [Test]
        public void Will_Also_Calls_The_First_Coin_When_Glimwood_Tangle_Flips_Again()
        {
            PokemonInPlay bewear, target;
            var e = Board(new ScriptedAgent().Then("Play Will", "Attack: Lariat", "Tails", "Yes", "Heads"), new ScriptedAgent(), Bewear, Wishiwashi, out bewear, out target);
            Attach(e, bewear, Psychic, 3);
            e.ScenarioSetStadium(1, C(GlimwoodTangle));
            Hand(e, 0, Will);
            Step(e);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters, "Compendium (2020-09-24): Will applies to the new flip's first coin too");
        }

        [Test]
        public void Chaotic_Swell_Discards_Itself_And_The_New_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Martial Arts Dojo"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(ChaoticSwell));
            Hand(e, 0, Dojo);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.AreEqual(1, CountNamed(e.State.Players[1].Discard, "Chaotic Swell"));
            Assert.AreEqual(1, CountNamed(e.State.Players[0].Discard, "Martial Arts Dojo"));
            Assert.IsTrue(e.State.Players[0].StadiumPlayedThisTurn, "Compendium: it still used your Stadium play for the turn");
        }

        [Test]
        public void Misty_And_Lorelei_Lets_Water_Pokemon_Use_A_Second_Gx_Attack_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Misty & Lorelei"), new ScriptedAgent());
            var duo = e.ScenarioActive(0, C(BlastoisePiplup));
            Attach(e, duo, Water, 3);
            e.ScenarioActive(1, C(Wishiwashi));
            e.ScenarioAddToDeck(0, C(Water), 3);
            Hand(e, 0, MistyLorelei, Psychic, Psychic, Psychic, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].GxUsed = true;
            Assert.IsFalse(Offered(e, 0, "Attack: Bubble Launcher-GX"), "the GX attack was used");
            Step(e);
            Assert.AreEqual(5, CountNamed(e.State.Players[0].Discard, "Psychic Energy"), "5 other cards discarded");
            Assert.IsTrue(e.State.Players[0].Hand.Any(x => x.Def.Name == "Water Energy"), "the search still happens");
            Assert.IsTrue(Offered(e, 0, "Attack: Bubble Launcher-GX"));
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 0, "Attack: Bubble Launcher-GX"), "only during that turn");
        }

        [Test]
        public void Lanas_Fishing_Rod_Works_With_Only_A_Tool_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lana's Fishing Rod"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, FishingRod);
            Discard(e, 0, Psychic);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Lana's Fishing Rod"), "nothing to shuffle back");
            Discard(e, 0, Beastite);
            Assert.IsTrue(Offered(e, 0, "Play Lana's Fishing Rod"), "your ruling: a Tool card alone is enough");
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(0, CountNamed(p.Discard, "Beastite"));
            Assert.AreEqual(1, CountNamed(p.Deck, "Beastite"));
        }

        [Test]
        public void Cynthia_And_Caitlin_Cant_Take_Back_The_Card_Discarded_For_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cynthia & Caitlin"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, CynthiaCaitlin, Roxie);
            Discard(e, 0, Will, CynthiaCaitlin);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(1, CountNamed(p.Hand, "Will"), "the only Supporter it can take");
            Assert.AreEqual(1, CountNamed(p.Discard, "Roxie"), "discarded for it");
            Assert.AreEqual(4, p.Hand.Count, "Will and 3 cards drawn");
        }

        [Test]
        public void Bellelba_And_Brycen_Man_Cuts_Each_Bench_To_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bellelba & Brycen-Man"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 5; i++)
            {
                e.ScenarioBench(0, C(Tauros));
                e.ScenarioBench(1, C(Koffing));
            }
            Hand(e, 0, Bellelba, Psychic, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Bench.Count);
            Assert.AreEqual(3, e.State.Players[1].Bench.Count);
            Assert.AreEqual(17, e.State.Players[0].Deck.Count, "top 3 discarded");
            Assert.AreEqual(17, e.State.Players[1].Deck.Count, "top 3 discarded");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "3 other cards discarded as it was played");
        }

        [Test]
        public void Red_And_Blue_Evolves_Into_A_Pokemon_GX_And_Attaches_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Red & Blue"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var typeNull = e.ScenarioBench(0, C(TypeNull));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(Silvally), 1);
            Hand(e, 0, RedBlue, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Silvally-GX", typeNull.Name);
            Assert.AreEqual(1, typeNull.Energy.Count, "basic Energy from the deck after discarding 2");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Red_And_Blue_Cant_Be_Played_During_Your_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(TypeNull));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, RedBlue);
            e.ScenarioSetTurn(0, 2);
            Assert.IsFalse(Offered(e, 0, "Play Red & Blue"));
        }

        [Test]
        public void Roxie_Draws_3_For_Each_Pokemon_And_Each_Blow_Away_Bomb_Stacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Roxie", "Koffing|Weezing"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var active = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Tauros));
            Hand(e, 0, Roxie, Koffing, Weezing, Silvally);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(7, p.Hand.Count, "Silvally-GX stays, 6 cards drawn");
            Assert.AreEqual(2, active.DamageCounters, "1 counter from each Blow-Away Bomb");
            Assert.AreEqual(2, benched.DamageCounters);
        }

        [Test]
        public void Professor_Oaks_Setup_Benches_Basics_Of_Different_Types()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Professor Oak's Setup"), new ScriptedAgent());
            e.ScenarioActive(0, C(Wishiwashi));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(Tauros), 2);
            e.ScenarioAddToDeck(0, C(Victini), 1);
            e.ScenarioAddToDeck(0, C(Koffing), 1);
            Hand(e, 0, OaksSetup);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var bench = e.State.Players[0].Bench;
            CollectionAssert.AreEquivalent(new[] { "Tauros", "Victini", "Koffing" }, bench.Select(m => m.Name).ToArray());
            Assert.AreEqual(1, CountNamed(e.State.Players[0].Deck, "Tauros"), "a second Colorless Pokémon isn't a different type");
        }

        [Test]
        public void Lillies_Full_Force_Shuffles_The_Hand_Down_To_2_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lillie's Full Force"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, FullForce, Fire, Fire);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
            e.EndTurn();
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual(20, e.State.Players[0].Deck.Count, "4 drawn, 4 shuffled back");
        }

        [Test]
        public void Light_Of_The_Protector_Protects_Every_Pokemon_During_The_Opponents_Next_Turn()
        {
            var me = new ScriptedAgent().Then("Play Lillie's Full Force", "Attack: Light of the Protector-GX");
            var them = new ScriptedAgent();
            PokemonInPlay duo, target;
            var e = Board(me, them, SolgaleoLunala, Wishiwashi, out duo, out target);
            Attach(e, duo, Psychic, 3);
            e.ScenarioBench(0, C(Tauros));
            var next = e.ScenarioBench(1, C(Wishiwashi));
            Attach(e, next, Water, 3);
            Hand(e, 0, FullForce);
            Step(e);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "200 damage Knocked Out Wishiwashi");
            NextTurn(e);
            Assert.AreEqual(next, e.State.Players[1].Active);
            them.Then("Attack: Hydro Splash");
            Step(e);
            Assert.AreEqual(0, duo.DamageCounters, "your ruling: all of your Pokémon are protected during that turn");
        }

        [Test]
        public void Great_Catcher_Only_Brings_Up_A_Pokemon_GX_Or_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Great Catcher"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Wishiwashi));
            Hand(e, 0, GreatCatcher, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Great Catcher"), "no Benched Pokémon-GX or Pokémon-EX");
            e.ScenarioBench(1, C(Silvally));
            Assert.IsTrue(Offered(e, 0, "Play Great Catcher"));
            Step(e);
            Assert.AreEqual("Silvally-GX", e.State.Players[1].Active.Name);
            Assert.AreEqual(2, CountNamed(e.State.Players[0].Discard, "Psychic Energy"), "2 other cards discarded");
        }

        [Test]
        public void Mallow_And_Lana_Heals_The_Pokemon_Moved_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mallow & Lana"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 10;
            e.ScenarioBench(0, C(Victini));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, MallowLana, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Victini", e.State.Players[0].Active.Name);
            Assert.AreEqual(0, tauros.DamageCounters, "120 healed after the switch");
        }

        [Test]
        public void Clay_Keeps_The_Items_Among_The_Top_7_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Clay"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            OnTop(e, 0, GreatCatcher);
            OnTop(e, 0, TagCall);
            Hand(e, 0, Clay);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            CollectionAssert.AreEquivalent(new[] { "Tag Call", "Great Catcher" }, p.Hand.Select(x => x.Def.Name).ToArray());
            Assert.AreEqual(5, CountNamed(p.Discard, "Psychic Energy"));
        }

        [Test]
        public void Erika_Lets_Each_Player_Draw_Up_To_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Erika"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, Erika);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.AreEqual(3, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Rosa_Needs_A_Knock_Out_During_The_Opponents_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rosa"), new ScriptedAgent());
            e.ScenarioActive(0, C(Wishiwashi));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(Tauros), 1);
            e.ScenarioAddToDeck(0, C(GreatCatcher), 1);
            Hand(e, 0, Rosa);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Rosa"));
            e.State.Players[0].LastKnockedOutTurn = 2;
            Step(e);
            CollectionAssert.AreEquivalent(new[] { "Tauros", "Great Catcher", "Psychic Energy" }, e.State.Players[0].Hand.Select(x => x.Def.Name).ToArray());
        }

        [Test]
        public void Roller_Skater_Draws_4_After_Discarding_An_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Roller Skater"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, RollerSkater, Fire);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Tag_Call_Finds_Tag_Team_Pokemon_And_Supporters()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Tag Call", "Blastoise & Piplup-GX|Cynthia & Caitlin"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(BlastoisePiplup), 1);
            e.ScenarioAddToDeck(0, C(CynthiaCaitlin), 1);
            e.ScenarioAddToDeck(0, C(Silvally), 1);
            Hand(e, 0, TagCall);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEquivalent(new[] { "Blastoise & Piplup-GX", "Cynthia & Caitlin" }, e.State.Players[0].Hand.Select(x => x.Def.Name).ToArray());
        }

        [Test]
        public void Ns_Resolve_Attaches_The_Discarded_Basic_Energy_To_A_Benched_Dragon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play N's Resolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            OnTop(e, 0, TagCall);
            OnTop(e, 0, Fire);
            Hand(e, 0, NsResolve);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play N's Resolve"), "needs a Benched Dragon Pokémon");
            var drampa = e.ScenarioBench(0, C(Drampa));
            Step(e);
            Assert.AreEqual(5, drampa.Energy.Count, "the Fire Energy and 4 Psychic Energy");
            Assert.AreEqual(1, CountNamed(e.State.Players[0].Discard, "Tag Call"));
        }

        [Test]
        public void Giant_Hearth_Trades_A_Card_For_2_Fire_Energy_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Giant Hearth", "Fire Energy|Fire Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(GiantHearth));
            e.ScenarioAddToDeck(0, C(Fire), 3);
            Hand(e, 0, Psychic);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(2, CountNamed(p.Hand, "Fire Energy"));
            Assert.AreEqual(1, CountNamed(p.Discard, "Psychic Energy"));
            Assert.IsFalse(Offered(e, 0, "Use Giant Hearth"), "once during each player's turn");
        }

        // ================================================================== Tools, Energy and Stadiums

        [Test]
        public void Island_Challenge_Amulet_Costs_100_HP_And_Saves_A_Prize()
        {
            PokemonInPlay attacker, silvally;
            var e = Board(new ScriptedAgent().Then("Attack: Hydro Splash"), new ScriptedAgent(), Wishiwashi, Silvally, out attacker, out silvally);
            Attach(e, attacker, Water, 3);
            e.ScenarioAttachTool(silvally, C(Amulet));
            e.ScenarioBench(1, C(Tauros));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(Amulet));
            Assert.AreEqual(110, e.MaxHp(silvally));
            Assert.AreEqual(120, e.MaxHp(tauros), "only a Pokémon-GX or Pokémon-EX loses HP");
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "1 Prize card instead of 2");
        }

        [Test]
        public void Beastite_Adds_10_For_Each_Prize_Taken_To_An_Ultra_Beast()
        {
            PokemonInPlay buzzwole, target;
            var e = Board(new ScriptedAgent().Then("Attack: Touchdown"), new ScriptedAgent(), Buzzwole, Wishiwashi, out buzzwole, out target);
            Attach(e, buzzwole, Grass, 2);
            e.ScenarioAttachTool(buzzwole, C(Beastite));
            e.State.Players[0].Prizes.RemoveRange(0, 2);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters, "60 + Beast Boost 40 + Beastite 20");

            PokemonInPlay tauros, other;
            var e2 = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Wishiwashi, out tauros, out other);
            Attach(e2, tauros, Psychic, 2);
            e2.ScenarioAttachTool(tauros, C(Beastite));
            e2.State.Players[0].Prizes.RemoveRange(0, 2);
            Step(e2);
            Assert.AreEqual(3, other.DamageCounters, "not an Ultra Beast");
        }

        [Test]
        public void Dragonium_Z_Gives_Destructive_Drake_GX_To_A_Pokemon_With_Dragon_Claw()
        {
            PokemonInPlay drampa, target;
            var e = Board(new ScriptedAgent().Then("Attack: Destructive Drake-GX"), new ScriptedAgent(), Drampa, Wishiwashi, out drampa, out target);
            Attach(e, drampa, Fire, 1);
            Attach(e, drampa, Water, 1);
            Attach(e, drampa, DrawEnergy, 1);
            Assert.IsFalse(Offered(e, 0, "Destructive Drake-GX"));
            e.ScenarioAttachTool(drampa, C(DragoniumZ));
            Assert.IsTrue(Offered(e, 0, "Attack: Destructive Drake-GX"));
            Step(e);
            Assert.AreEqual(16, target.DamageCounters, "80 for each of the 2 basic Energy");
            Assert.AreEqual(1, drampa.Energy.Count, "Draw Energy stays");
            Assert.IsTrue(e.State.Players[0].GxUsed);

            PokemonInPlay tauros, other;
            var e2 = Board(new ScriptedAgent(), new ScriptedAgent(), Tauros, Wishiwashi, out tauros, out other);
            Attach(e2, tauros, Psychic, 3);
            e2.ScenarioAttachTool(tauros, C(DragoniumZ));
            Assert.IsFalse(Offered(e2, 0, "Destructive Drake-GX"), "no Dragon Claw attack");
        }

        [Test]
        public void Draw_Energy_Draws_A_Card_When_Attached_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Draw Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, DrawEnergy);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "drew a card");
        }

        [TestCase(Fighting, 6, 4)]
        [TestCase(Fighting, 5, 7)]
        [TestCase(Psychic, 6, 3)]
        public void Martial_Arts_Dojo_Boosts_Pokemon_With_Basic_Fighting_Energy(string energy, int theirPrizes, int counters)
        {
            PokemonInPlay tauros, target;
            var e = Board(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent(), Tauros, Wishiwashi, out tauros, out target);
            Attach(e, tauros, energy, 1);
            Attach(e, tauros, Psychic, 1);
            e.ScenarioSetStadium(1, C(Dojo));
            while (e.State.Players[1].Prizes.Count > theirPrizes) e.State.Players[1].Prizes.RemoveAt(0);
            Step(e);
            Assert.AreEqual(counters, target.DamageCounters);
        }

        [Test]
        public void Martial_Arts_Dojo_Doesnt_Boost_Ultra_Beasts()
        {
            PokemonInPlay buzzwole, target;
            var e = Board(new ScriptedAgent().Then("Attack: Touchdown"), new ScriptedAgent(), Buzzwole, Wishiwashi, out buzzwole, out target);
            Attach(e, buzzwole, Grass, 1);
            Attach(e, buzzwole, Fighting, 1);
            e.ScenarioSetStadium(1, C(Dojo));
            Step(e);
            Assert.AreEqual(6, target.DamageCounters);
        }

        [Test]
        public void Power_Plant_Turns_Off_Abilities_Of_Pokemon_GX_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Silvally));
            e.ScenarioBench(0, C(Sawsbuck));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Use Disk Reload"));
            e.ScenarioSetStadium(1, C(PowerPlant));
            Assert.IsFalse(Offered(e, 0, "Use Disk Reload"));
            Assert.IsTrue(Offered(e, 0, "Use Seasonal Blessings"), "not a Pokémon-GX");
        }

        [Test]
        public void Shadow_Box_Turns_Off_Abilities_Of_Damaged_Pokemon_GX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var silvally = e.ScenarioActive(0, C(Silvally));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(MimikyuShadowBox));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Use Disk Reload"), "no damage counters");
            silvally.DamageCounters = 1;
            Assert.IsFalse(Offered(e, 0, "Use Disk Reload"));
        }

        [Test]
        public void Shining_Vine_Switches_In_A_Benched_Pokemon_When_Grass_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Grass Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(VenusaurSnivy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Wishiwashi));
            Hand(e, 0, Grass);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Wishiwashi", e.State.Players[1].Active.Name);
        }
    }
}
