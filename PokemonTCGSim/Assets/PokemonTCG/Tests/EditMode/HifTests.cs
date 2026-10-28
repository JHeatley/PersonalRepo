using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v69: Hidden Fates (HIF 1-69 and the Shiny Vault SV1-SV94). Your rulings (v69): Misty's Water Command and Brock's Training
    /// name exact Pokémon; Sky Legends-GX checks its extra Energy before it shuffles itself away; Brock's Grit shuffles in all of
    /// them with fewer than 6; Extra Energy Bomb attaches what there is. Compendium: Lt. Surge's Strategy must be the first
    /// Supporter of the turn; Stinger-GX and Symbiont-GX don't change the count of Prize cards taken; Sabrina's Suggestion uses
    /// only the effect, the Supporter stays in the opponent's hand.
    /// </summary>
    public class HifTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Tauros = "MEW-128";
        const string Will = "CEC-208";

        const string Staryu = "HIF-13";
        const string GyaradosGX = "HIF-16";
        const string Lapras = "HIF-17";
        const string Vaporeon = "HIF-18";
        const string Jolteon = "HIF-23";
        const string Koffing = "HIF-28";
        const string Weezing = "HIF-29";
        const string MewtwoGX = "HIF-31";
        const string MZA = "HIF-44";
        const string Snorlax = "HIF-50";
        const string BrocksGrit = "HIF-53";
        const string ErikasHospitality = "HIF-56";
        const string JessieJames = "HIF-58";
        const string KogasTrap = "HIF-59";
        const string LtSurge = "HIF-60";
        const string WaterCommand = "HIF-63";
        const string Sabrina = "HIF-65";
        const string Seviper = "HIF-SV15";
        const string Poipole = "HIF-SV19";
        const string Sudowoodo = "HIF-SV20";
        const string Eevee = "HIF-SV41";
        const string Oranguru = "HIF-SV44";
        const string DecidueyeGX = "HIF-SV47";
        const string GlaceonGX = "HIF-SV55";
        const string ElectrodeGX = "HIF-SV57";
        const string XurkitreeGX = "HIF-SV58";
        const string NihilegoGX = "HIF-SV62";
        const string NaganadelGX = "HIF-SV63";
        const string DarkraiGX = "HIF-SV70";
        const string Fisherman = "HIF-SV83";
        const string Guzma = "HIF-SV84";
        const string Hiker = "HIF-SV85";
        const string BrookletHill = "HIF-SV88";
        const string MtCoronet = "HIF-SV89";
        const string Shrine = "HIF-SV90";

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
        public void Every_Hidden_Fates_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 69; n++) Assert.IsNotNull(db.Get("HIF-" + n), "HIF " + n + " is missing");
            for (int n = 1; n <= 94; n++) Assert.IsNotNull(db.Get("HIF-SV" + n), "HIF SV" + n + " is missing");
            Assert.AreEqual(MZA, db.MainPrinting(C("HIF-66")).Key, "full art");
            Assert.AreEqual(MZA, db.MainPrinting(C("HIF-69")).Key, "rainbow");
            Assert.AreEqual("HIF-57", db.MainPrinting(C("HIF-67")).Key, "Giovanni's Exile full art");
            Assert.AreEqual(JessieJames, db.MainPrinting(C("HIF-68")).Key, "Jessie & James full art");
            Assert.AreEqual("Pokémon Center Lady", C("HIF-64").Name);
            Assert.AreEqual("Buzzwole-GX", C("HIF-SV68").Name);
            Assert.AreEqual("Lady", C("HIF-SV86").Name);
            Assert.AreEqual("Tapu Lele-GX", C("HIF-SV94").Name);
            Assert.IsFalse(Formats.IsStandardCard(C(MewtwoGX)), "no regulation mark");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(MewtwoGX)));
            Assert.IsTrue(GameEngine.IsGxAttack(C("HIF-9").Attacks[1]), "Flare Blitz-GX has only the reminder text");
        }

        [Test]
        public void Jessie_And_James_And_Lt_Surges_Strategy_Are_Banned_In_Expanded()
        {
            foreach (var key in new[] { JessieJames, "HIF-68", LtSurge })
            {
                Assert.IsTrue(Formats.IsExpandedBanned(C(key)), key);
                Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(key)), key);
            }
            Assert.IsFalse(Formats.IsExpandedBanned(C("HIF-57")), "Giovanni's Exile");
        }

        [Test]
        public void Tag_Team_And_Ultra_Beast_Cards_Are_Marked()
        {
            var mza = C(MZA);
            Assert.IsTrue(mza.IsTagTeam && mza.IsGX && mza.IsGXorEX);
            Assert.AreEqual(3, mza.PrizeValue, "a TAG TEAM gives 3 Prize cards");
            foreach (var key in new[] { Poipole, NihilegoGX, NaganadelGX, "HIF-SV71", "HIF-SV68" }) Assert.IsTrue(C(key).IsUltraBeast, key);
            Assert.IsFalse(C(GlaceonGX).IsUltraBeast);
        }

        // ================================================================== Sky Legends-GX

        [TestCase(true)]
        [TestCase(false)]
        public void Sky_Legends_GX_Checks_The_Extra_Energy_Before_Shuffling_Itself_Away(bool extra)
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sky Legends-GX"), new ScriptedAgent());
            var mza = e.ScenarioActive(0, C(MZA));
            Attach(e, mza, Fire, 1);
            Attach(e, mza, Water, 1);
            Attach(e, mza, Lightning, 1);
            if (extra) Attach(e, mza, Psychic, 1);
            e.ScenarioBench(0, C(Tauros));
            var targets = new List<PokemonInPlay> { e.ScenarioActive(1, C(Snorlax)), e.ScenarioBench(1, C(Snorlax)), e.ScenarioBench(1, C(Snorlax)) };
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(1, CountNamed(p.Deck, "Moltres & Zapdos & Articuno-GX"), "shuffled into the deck");
            Assert.AreEqual("Tauros", p.Active.Name);
            foreach (var target in targets)
                Assert.AreEqual(extra ? 11 : 0, target.DamageCounters, "your ruling: the extra Energy counts as it was before the shuffle");
        }

        // ================================================================== Supporters

        [Test]
        public void Lt_Surges_Strategy_Allows_3_Supporters_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lt. Surge's Strategy", "Play Koga's Trap", "Play Fisherman"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Snorlax));
            Hand(e, 0, LtSurge, KogasTrap, Fisherman, Guzma);
            Discard(e, 0, Fire);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Lt. Surge's Strategy"), "only with more Prize cards left than the opponent");
            e.State.Players[1].Prizes.RemoveAt(0);
            Assert.IsTrue(Offered(e, 0, "Play Lt. Surge's Strategy"));
            Step(e);
            Step(e);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(3, p.SupportersPlayedCount);
            Assert.IsTrue(theirs.Has(SpecialCondition.Poisoned) && theirs.Has(SpecialCondition.Confused), "Koga's Trap");
            Assert.AreEqual(1, CountNamed(p.Hand, "Fire Energy"), "Fisherman");
            Assert.IsFalse(Offered(e, 0, "Play Guzma"), "3 Supporters, this one included");
            NextTurn(e);
            NextTurn(e);
            Assert.AreEqual(1, e.State.Players[0].SupporterLimit, "only during that turn");
        }

        [Test]
        public void Lt_Surges_Strategy_Must_Be_The_First_Supporter_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Koga's Trap"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, KogasTrap, LtSurge);
            e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(Offered(e, 0, "Play Lt. Surge's Strategy"), "Compendium: not after another Supporter");
        }

        [Test]
        public void Jessie_And_James_Discards_Opponent_First_And_Weezing_Uses_Surrender_Now()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Jessie & James", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, JessieJames, Weezing, Koffing);
            Hand(e, 1, Psychic, Psychic, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            var opp = e.State.Players[1];
            Assert.AreEqual(0, p.Hand.Count);
            Assert.AreEqual(1, CountNamed(p.Discard, "Weezing"));
            Assert.AreEqual(1, opp.Hand.Count, "2 discarded, then 1 more for Surrender Now");
            Assert.AreEqual(3, opp.Discard.Count);
        }

        [Test]
        public void Sabrinas_Suggestion_Uses_The_Effect_Of_A_Supporter_In_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Sabrina's Suggestion", "Use Koga's Trap's effect"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            var theirs = e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, Sabrina);
            Hand(e, 1, KogasTrap, Psychic);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(theirs.Has(SpecialCondition.Poisoned) && theirs.Has(SpecialCondition.Confused));
            Assert.IsFalse(mine.Has(SpecialCondition.Poisoned), "the effect is ours");
            Assert.AreEqual(1, CountNamed(e.State.Players[1].Hand, "Koga's Trap"), "only its effect was used");
        }

        [Test]
        public void Brocks_Grit_Shuffles_In_All_Of_Them_With_Fewer_Than_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Brock's Grit"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, BrocksGrit);
            Discard(e, 0, Tauros, Tauros, Fire, Will);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(0, CountNamed(p.Discard, "Tauros"));
            Assert.AreEqual(0, CountNamed(p.Discard, "Fire Energy"));
            Assert.AreEqual(1, CountNamed(p.Discard, "Will"), "a Supporter isn't a Pokémon or basic Energy card");
            Assert.AreEqual(23, p.Deck.Count, "your ruling: all 3");
        }

        [Test]
        public void Mistys_Water_Command_Names_Exact_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Misty's Water Command"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Water, 2);
            var gyarados = e.ScenarioBench(0, C(GyaradosGX));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, WaterCommand);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Misty's Water Command"), "your ruling: Gyarados-GX isn't Gyarados");
            var lapras = e.ScenarioBench(0, C(Lapras));
            Assert.IsTrue(Offered(e, 0, "Play Misty's Water Command"));
            Step(e);
            Assert.AreEqual(1, lapras.Energy.Count, "the bots move one");
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(0, gyarados.Energy.Count);
        }

        [Test]
        public void Erikas_Hospitality_Needs_4_Or_Fewer_Other_Cards_And_Draws_For_Each_Opposing_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Erika's Hospitality"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            Hand(e, 0, ErikasHospitality, Psychic, Psychic, Psychic, Psychic, Psychic);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Erika's Hospitality"), "5 other cards");
            var p = e.State.Players[0];
            p.Hand.Remove(p.Hand.First(x => x.Def.Name == "Psychic Energy"));
            Assert.IsTrue(Offered(e, 0, "Play Erika's Hospitality"));
            Step(e);
            Assert.AreEqual(7, p.Hand.Count, "4 cards and 3 drawn");
        }

        [Test]
        public void Guzma_Switches_Both_Active_Pokemon_And_Needs_An_Opposing_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Guzma"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var myBenched = e.ScenarioBench(0, C(Snorlax));
            e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, Guzma);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Guzma"), "Compendium: not with no opposing Bench");
            var theirBenched = e.ScenarioBench(1, C(Snorlax));
            Assert.IsTrue(Offered(e, 0, "Play Guzma"));
            Step(e);
            Assert.AreEqual(theirBenched, e.State.Players[1].Active);
            Assert.AreEqual(myBenched, e.State.Players[0].Active);
        }

        [Test]
        public void Hiker_Puts_The_Chosen_Card_On_Top_Of_The_Opponents_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hiker", "Your opponent's deck", "Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[1].Deck.Insert(3, e.NewCard(1, C(Tauros)));
            Hand(e, 0, Hiker);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var deck = e.State.Players[1].Deck;
            Assert.AreEqual("Tauros", deck[0].Def.Name);
            Assert.AreEqual(21, deck.Count);
        }

        // ================================================================== Abilities

        [Test]
        public void Roadblock_Cuts_The_Opponents_Bench_To_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Sudowoodo"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Tauros));
            Hand(e, 0, Sudowoodo);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Bench.Count);
            Assert.AreEqual(1, CountNamed(e.State.Players[1].Discard, "Tauros"));
            Assert.AreEqual(1, e.State.Players[0].Bench.Count, "your own Bench isn't limited");
            e.ScenarioSetTurn(1, 4);
            Hand(e, 1, Tauros);
            Assert.IsFalse(Offered(e, 1, "Bench Tauros"), "no 5th Benched Pokémon");
        }

        [Test]
        public void Freezing_Gaze_Stops_Opposing_Pokemon_GX_Abilities_In_Play_And_In_The_Discard_Pile()
        {
            var them = new ScriptedAgent();
            var e = Scenario(new ScriptedAgent(), them);
            e.ScenarioActive(0, C(GlaceonGX));
            var benched = e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(DecidueyeGX));
            e.ScenarioBench(1, C(Oranguru));
            Discard(e, 1, DarkraiGX, Darkness);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Use Feather Arrow"), "a Pokémon-GX in play");
            Assert.IsFalse(Offered(e, 1, "Restoration"), "a Pokémon-GX in the discard pile");
            Assert.IsTrue(Offered(e, 1, "Use Instruct"), "Oranguru isn't a Pokémon-GX");

            e.SwitchActive(0, benched);
            Assert.IsTrue(Offered(e, 1, "Use Feather Arrow"), "only while Glaceon-GX is Active");
            Assert.IsTrue(Offered(e, 1, "Restoration"));
            them.Then("Restoration");
            Step(e);
            var darkrai = e.State.Players[1].Bench.Single(m => m.Name == "Darkrai-GX");
            Assert.AreEqual(1, darkrai.Energy.Count, "a Darkness Energy from the discard pile");
        }

        [Test]
        public void Electromagnetic_Wall_Hurts_A_Pokemon_The_Opponent_Attaches_Energy_To()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Psychic Energy"));
            e.ScenarioActive(0, C(Jolteon));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Hand(e, 1, Psychic);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(2, tauros.DamageCounters);
        }

        [Test]
        public void More_Poison_Adds_A_Damage_Counter_To_Poison()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Koga's Trap"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Seviper));
            var theirs = e.ScenarioActive(1, C(Tauros));
            Hand(e, 0, KogasTrap);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            Assert.AreEqual(2, theirs.DamageCounters);
        }

        [Test]
        public void Energy_Evolution_Evolves_Eevee_From_The_Deck_Even_On_The_First_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy", "Yes"), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C(Eevee));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(Vaporeon), 1);
            Hand(e, 0, Water);
            e.ScenarioSetTurn(0, 1);
            Step(e);
            Assert.AreEqual("Vaporeon", eevee.Name, "your ruling (v58): like Solar Evolution, it ignores the usual timing");
            Assert.AreEqual(1, eevee.Energy.Count);
            Assert.AreEqual(20, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Extra_Energy_Bomb_Attaches_What_There_Is_And_Knocks_Out_Electrode_GX()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Extra Energy Bomb"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(ElectrodeGX));
            e.ScenarioActive(1, C(Tauros));
            Discard(e, 0, Lightning, Lightning, Lightning);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(3, tauros.Energy.Count, "your ruling: what there is, with fewer than 5");
            Assert.AreEqual(0, p.Bench.Count);
            Assert.AreEqual(1, CountNamed(p.Discard, "Electrode-GX"));
            Assert.AreEqual(4, e.State.Players[1].Prizes.Count, "2 Prize cards for the opponent");
        }

        [Test]
        public void Shrine_Of_Punishment_Hurts_Every_Pokemon_GX_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(MewtwoGX));
            var tauros = e.ScenarioBench(0, C(Tauros));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            var xurkitree = e.ScenarioBench(1, C(XurkitreeGX));
            e.ScenarioSetStadium(1, C(Shrine));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, mewtwo.DamageCounters);
            Assert.AreEqual(1, xurkitree.DamageCounters, "on the Bench too");
            Assert.AreEqual(0, tauros.DamageCounters);
            Assert.AreEqual(0, snorlax.DamageCounters);
        }

        [Test]
        public void Brooklet_Hill_Benches_A_Basic_Water_Or_Fighting_Pokemon_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Brooklet Hill"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(Snorlax), 1);
            e.ScenarioAddToDeck(0, C(Staryu), 1);
            e.ScenarioSetStadium(1, C(BrookletHill));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Staryu", e.State.Players[0].Bench.Single().Name);
            Assert.IsFalse(Offered(e, 0, "Use Brooklet Hill"), "once during each player's turn");
        }

        [Test]
        public void Mt_Coronet_Returns_2_Metal_Energy_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Mt. Coronet"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Discard(e, 0, Metal, Metal, Metal);
            e.ScenarioSetStadium(0, C(MtCoronet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(2, CountNamed(p.Hand, "Metal Energy"));
            Assert.AreEqual(1, CountNamed(p.Discard, "Metal Energy"));
        }

        // ================================================================== Attacks

        [Test]
        public void Knockout_Reviver_Gives_No_Prize_Cards_During_The_Opponents_Next_Turn()
        {
            var me = new ScriptedAgent().Then("Attack: Knockout Reviver");
            var them = new ScriptedAgent();
            PokemonInPlay poipole, mewtwo;
            var e = Board(me, them, Poipole, MewtwoGX, out poipole, out mewtwo);
            Attach(e, poipole, Psychic, 2);
            e.ScenarioBench(0, C(Tauros));
            Attach(e, mewtwo, Psychic, 3);
            Step(e);
            NextTurn(e);
            them.Then("Attack: Super Psy Bolt");
            Step(e);
            Assert.AreEqual(6, e.State.Players[1].Prizes.Count, "no Prize cards for Poipole");
            Assert.AreEqual("Tauros", e.State.Players[0].Active.Name);
        }

        [Test]
        public void Stinger_GX_Leaves_3_Prize_Cards_Each_And_Keeps_The_Count_Taken()
        {
            PokemonInPlay naganadel, snorlax;
            var e = Board(new ScriptedAgent().Then("Attack: Stinger-GX"), new ScriptedAgent(), NaganadelGX, Snorlax, out naganadel, out snorlax);
            Attach(e, naganadel, Psychic, 3);
            var p = e.State.Players[0];
            var opp = e.State.Players[1];
            p.Prizes.RemoveRange(0, 2);
            Assert.AreEqual(2, e.PrizesTaken(0));
            Step(e);
            Assert.AreEqual(3, p.Prizes.Count);
            Assert.AreEqual(3, opp.Prizes.Count);
            Assert.AreEqual(2, e.PrizesTaken(0), "Compendium: Prize cards taken stay taken");
            Assert.AreEqual(0, e.PrizesTaken(1));
            Assert.AreEqual(21, p.Deck.Count);
            Assert.AreEqual(23, opp.Deck.Count);
        }

        [Test]
        public void Symbiont_GX_Adds_2_Prize_Cards_That_Arent_Taken()
        {
            PokemonInPlay nihilego, snorlax;
            var e = Board(new ScriptedAgent().Then("Attack: Symbiont-GX"), new ScriptedAgent(), NihilegoGX, Snorlax, out nihilego, out snorlax);
            Attach(e, nihilego, Psychic, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(8, opp.Prizes.Count);
            Assert.AreEqual(0, e.PrizesTaken(1));
            Assert.AreEqual(18, opp.Deck.Count);
        }
    }
}
