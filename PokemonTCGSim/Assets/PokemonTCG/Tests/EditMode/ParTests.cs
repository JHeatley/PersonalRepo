using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v37: Paradox Rift (PAR), all 266 cards: new cards, alternate prints of pool cards, Compendium rulings and a selection of effects.</summary>
    public class ParTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";

        const string Simisage = "PAR-5";
        const string Toedscruel = "PAR-17";
        const string WoChien = "PAR-18";
        const string Magby = "PAR-19";
        const string Simisear = "PAR-21";
        const string Remoraid = "PAR-33";
        const string Octillery = "PAR-34";
        const string Simipour = "PAR-42";
        const string Vanilluxe = "PAR-45";
        const string Golisopod = "PAR-49";
        const string GolisopodEx = "PAR-50";
        const string Dondozo = "PAR-55";
        const string Plusle = "PAR-60";
        const string Minun = "PAR-61";
        const string Flittle = "PAR-79";
        const string Espathra = "PAR-81";
        const string Mienshao = "PAR-97";
        const string Minior = "PAR-99";
        const string Crobat = "PAR-112";
        const string Absol = "PAR-113";
        const string Jirachi = "PAR-126";
        const string Loudred = "PAR-149";
        const string Exploud = "PAR-150";
        const string Tandemaus = "PAR-153";
        const string MausholdEx = "PAR-155";
        const string IronJugulis = "PAR-158";
        const string CursedDuster = "PAR-161";
        const string DefianceVest = "PAR-162";
        const string LuxuriousCape = "PAR-166";
        const string Norman = "PAR-168";
        const string Roark = "PAR-173";
        const string SnorlaxDoll = "PAR-175";
        const string ReversalEnergy = "PAR-266";
        const string Froslass = "PAR-3";
        const string FutureBooster = "TEF-149";
        const string Diancie = "SCR-86";
        const string TingLuEx = "PAF-244";
        const string NestBall = "PAF-84";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static AttackDef AttackNamed(CardDef d, string name)
        {
            return d.Attacks.First(a => a.Name == name);
        }

        /// <summary>
        /// Sets up with Snorlax Doll as the Active Pokémon whenever it's offered; otherwise takes the first option. Says No to
        /// the first MulligansWanted "keep a hand with only Snorlax Doll?" questions, then Yes.
        /// </summary>
        private sealed class DollSetupAgent : IPlayerAgent
        {
            public int MulligansWanted;
            public int DollQuestions;

            public List<int> Decide(DecisionRequest r)
            {
                if (r.Kind == DecisionKind.YesNo && r.Prompt != null && r.Prompt.StartsWith("Snorlax Doll:"))
                {
                    DollQuestions++;
                    return new List<int> { DollQuestions <= MulligansWanted ? 1 : 0 };
                }
                if (r.Kind == DecisionKind.SetupActive)
                {
                    int doll = r.Options.FindIndex(o => o.Label == "Snorlax Doll");
                    return new List<int> { doll < 0 ? 0 : doll };
                }
                int n = System.Math.Min(r.MaxChoices, System.Math.Max(r.MinChoices, 1));
                return Enumerable.Range(0, n).ToList();
            }
        }

        // ================================================================== Data

        [Test]
        public void Every_Paradox_Rift_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 266; n++)
                Assert.IsNotNull(db.Get("PAR-" + n), "PAR " + n + " is missing");
        }

        [Test]
        public void Reprints_Are_Alternate_Prints_Of_The_Pool_Card()
        {
            Assert.AreEqual("PRE-31", db.MainPrinting(C("PAR-70")).Key, "Iron Hands ex");
            Assert.AreEqual("SVP-68", db.MainPrinting(C("PAR-89")).Key, "Iron Valiant ex");
            Assert.AreEqual("PAF-25", db.MainPrinting(C("PAR-71")).Key, "Natu");
            Assert.AreEqual("TEF-140", db.MainPrinting(C("PAR-159")).Key, "Ancient Booster Energy Capsule");
            Assert.AreEqual("PAR-139", db.MainPrinting(C("PRE-164")).Key, "the Prismatic Evolutions Gholdengo ex is a print of the new card");
            Assert.AreEqual("PAR-263", db.MainPrinting(C("SVI-167")).Key, "Beach Court");
            Assert.AreEqual("G", C(Simisage).RegulationMark, "Paradox Rift is G: Expanded only");
            Assert.IsTrue(C(IronJugulis).IsFuture, "paradox tags are read");
        }

        // ================================================================== Abilities

        [Test]
        public void Monkey_Trio_Ignores_Colorless_With_All_Three_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var simisage = e.ScenarioActive(0, C(Simisage));
            e.ScenarioAttach(simisage, C(Grass));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Assert.IsFalse(Offered(e, 0, "Attack: Arm Thrust Needle"), "GCC with 1 Energy");
            e.ScenarioBench(0, C(Simisear));
            Assert.IsFalse(Offered(e, 0, "Attack: Arm Thrust Needle"), "all three are needed");
            e.ScenarioBench(0, C(Simipour));
            Assert.IsTrue(Offered(e, 0, "Attack: Arm Thrust Needle"));
            Assert.AreEqual(1, e.AttackCost(simisage, AttackNamed(C(Simisage), "Arm Thrust Needle")).Count);
        }

        [Test]
        public void Frigid_Room_Stops_The_Opponents_Pokemon_With_40_HP_Or_Less_Attacking()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Remoraid));
            e.ScenarioBench(0, C(Vanilluxe));
            var theirs = e.ScenarioActive(1, C(Remoraid));
            e.ScenarioSetTurn(1, 4);

            theirs.DamageCounters = 2;
            Assert.IsFalse(e.AttacksPrevented(theirs), "50 HP left");
            theirs.DamageCounters = 3;
            Assert.IsTrue(e.AttacksPrevented(theirs), "40 HP left");
            mine.DamageCounters = 3;
            Assert.IsFalse(e.AttacksPrevented(mine), "only the opponent's Pokémon");
        }

        [Test]
        public void Slime_Mold_Colony_Keeps_The_Opponents_Trainers_From_Taking_Cards_From_The_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Roark"), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            e.ScenarioActive(1, C(Toedscruel));
            e.ScenarioAddToHand(0, C(Roark));
            e.ScenarioAddToDiscard(0, C(Water), 1);
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Roark still draws 2");
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Water Energy"), "but the Energy stays in the discard pile");

            var e2 = Scenario(new ScriptedAgent().Then("Play Roark"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Remoraid));
            e2.ScenarioActive(1, C(Remoraid));
            e2.ScenarioAddToHand(0, C(Roark));
            e2.ScenarioAddToDiscard(0, C(Water), 1);
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(3, e2.State.Players[0].Hand.Count, "without Toedscruel the Energy comes back");
        }

        [Test]
        public void Stance_Stops_Scorching_Heater_And_Stays_When_Espathra_Moves_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes", "Attack: Glittering Eyes"), new ScriptedAgent().Then("Attack: Scorching Heater"));
            var flittle = e.ScenarioActive(0, C(Flittle));
            e.ScenarioAttach(flittle, C(Psychic));
            e.ScenarioAttach(flittle, C(Psychic));
            var benched = e.ScenarioBench(0, C(Remoraid));
            e.ScenarioAddToHand(0, C(Espathra));
            e.ScenarioActive(1, C(Magby));
            e.ScenarioBench(1, C(Remoraid));

            e.ScenarioSetTurn(1, 2);
            Step(e);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Espathra", flittle.Name);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Magby"), "Glittering Eyes Knocked Out Magby");
            Assert.AreEqual(0, flittle.DamageCounters, "Stance prevents Scorching Heater's damage counters (Compendium)");

            e.SwitchActive(0, benched);
            Assert.IsTrue(e.State.Effects.Any(x => x is CardsPAR1.StanceEffect && x.TargetUid == flittle.Uid), "an Ability's effect stays on the Bench");
        }

        [Test]
        public void Hard_Bashing_Gets_Through_Stance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hard Bashing"), new ScriptedAgent());
            var aegislash = e.ScenarioActive(0, C("SVP-60"));
            e.ScenarioAttach(aegislash, C("MEE-16"));
            e.ScenarioAttach(aegislash, C(Psychic));
            var espathra = e.ScenarioActive(1, C(Espathra));
            e.ScenarioBench(1, C(Remoraid));
            e.AddEffect(new CardsPAR1.StanceEffect("Stance", 1, espathra.Uid, 4));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Espathra"), "its damage ignores effects on the Defending Pokémon (Compendium)");
        }

        [Test]
        public void Buddy_Pulse_Does_Not_Stack_And_Works_Once_Per_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Psychic Energy"));
            e.ScenarioActive(0, C(Minun));
            e.ScenarioBench(0, C(Minun));
            e.ScenarioBench(0, C(Plusle));
            var target = e.ScenarioActive(1, C(Remoraid));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(1, 4);

            Step(e);
            Assert.AreEqual(2, target.DamageCounters, "2 Minun, but it doesn't stack");

            var extra = e.ScenarioAddToHand(1, C(Psychic));
            e.State.Players[1].Hand.Remove(extra);
            e.AttachEnergyCard(extra, target, true);
            Assert.AreEqual(2, target.DamageCounters, "once per turn (Compendium)");
        }

        [Test]
        public void Far_Flying_Meteor_Is_Not_Once_Per_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var remoraid = e.ScenarioActive(0, C(Remoraid));
            var minior = e.ScenarioBench(0, C(Minior));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioSetTurn(0, 3);

            var first = e.ScenarioAddToHand(0, C(Fighting));
            e.State.Players[0].Hand.Remove(first);
            e.AttachEnergyCard(first, minior, true);
            Assert.AreSame(minior, e.State.Players[0].Active);

            e.SwitchActive(0, remoraid);
            var second = e.ScenarioAddToHand(0, C(Fighting));
            e.State.Players[0].Hand.Remove(second);
            e.AttachEnergyCard(second, minior, true);
            Assert.AreSame(minior, e.State.Players[0].Active, "the errata removed \"once during your turn\"");
        }

        [Test]
        public void Evanescent_Only_Works_In_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            e.ScenarioActive(1, C(Remoraid));
            var froslass = e.ScenarioBench(1, C(Froslass));
            e.ScenarioSetTurn(0, 3);

            Assert.AreEqual(2, e.PrizesFor(froslass), "no coin flip for a Benched Froslass ex");
        }

        [Test]
        public void Solidarity_Counts_Each_Tandemaus_Maushold_And_Maushold_Ex()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Waterfall"), new ScriptedAgent());
            var golisopod = e.ScenarioActive(0, C(Golisopod));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(golisopod, C(Water));
            e.ScenarioActive(1, C(MausholdEx));
            e.ScenarioBench(1, C(Tandemaus));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(6, golisopod.DamageCounters, "3 for Maushold ex and 3 for Tandemaus");
        }

        [Test]
        public void Stellar_Veil_Stops_Damage_Counters_From_Basic_Attackers_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Land Scoop", "Remoraid"), new ScriptedAgent());
            var tingLu = e.ScenarioActive(0, C(TingLuEx));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tingLu, C(Fighting));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            e.ScenarioBench(1, C(Jirachi));
            var remoraid = e.ScenarioBench(1, C(Remoraid));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(15, dondozo.DamageCounters, "the damage isn't stopped");
            Assert.AreEqual(0, remoraid.DamageCounters, "Land Scoop's counters from a Basic Pokémon are");
        }

        // ================================================================== Attacks

        [Test]
        public void Scorching_Heater_Puts_6_Damage_Counters_On_The_Attacker_Even_If_Magby_Is_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Waterfall"), new ScriptedAgent().Then("Attack: Scorching Heater"));
            var golisopod = e.ScenarioActive(0, C(Golisopod));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(golisopod, C(Water));
            e.ScenarioActive(1, C(Magby));
            e.ScenarioBench(1, C(Remoraid));

            e.ScenarioSetTurn(1, 2);
            Step(e);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, golisopod.DamageCounters);
        }

        [Test]
        public void Binding_Greed_Makes_The_Defending_Pokemons_Attacks_Cost_2_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Binding Greed"), new ScriptedAgent());
            var woChien = e.ScenarioActive(0, C(WoChien));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(woChien, C(Grass));
            e.ScenarioAttach(woChien, C(Psychic));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(14, dondozo.DamageCounters);
            Assert.AreEqual(7, e.AttackCost(dondozo, AttackNamed(C(Dondozo), "Hydro Splash")).Count, "WCCCC plus 2");
        }

        [Test]
        public void Drawareness_Discards_Down_To_4_Then_Draws_Up_To_5()
        {
            var agent = new ScriptedAgent().Then("Attack: Drawareness", "Yes");
            var e = Scenario(agent, new ScriptedAgent());
            var absol = e.ScenarioActive(0, C(Absol));
            e.ScenarioAttach(absol, C(Darkness));
            e.ScenarioActive(1, C(Remoraid));
            for (int i = 0; i < 6; i++) e.ScenarioAddToHand(0, C(Water));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            var discard = agent.Seen.Last(r => r.Kind == DecisionKind.ChooseCards);
            Assert.AreEqual(2, discard.MinChoices, "a discard goes down to 4 or fewer (Compendium)");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Water Energy"));
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Whip_Expert_Does_More_After_Attaching_A_Tool_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Defiance Vest", "Attack: Whip Expert"), new ScriptedAgent());
            var mienshao = e.ScenarioActive(0, C(Mienshao));
            e.ScenarioAttach(mienshao, C(Psychic));
            e.ScenarioAttach(mienshao, C(Psychic));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            e.ScenarioAddToHand(0, C(DefianceVest));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Step(e);
            Assert.AreEqual(12, dondozo.DamageCounters, "50 + 70");
        }

        [Test]
        public void Echoing_Madness_Stops_The_Chosen_Kind_Of_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Echoing Madness", "Supporter cards"), new ScriptedAgent());
            var crobat = e.ScenarioActive(0, C(Crobat));
            e.ScenarioAttach(crobat, C(Darkness));
            e.ScenarioAttach(crobat, C(Darkness));
            e.ScenarioActive(1, C(Dondozo));
            e.ScenarioAddToHand(1, C(Norman));
            e.ScenarioAddToHand(1, C(NestBall));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Play Norman"));
            Assert.IsTrue(Offered(e, 1, "Play Nest Ball"), "Items are fine");
        }

        [Test]
        public void Sudden_Shout_Needs_Exploud_To_Have_Evolved_From_Loudred_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Attack: Sudden Shout"), new ScriptedAgent());
            var loudred = e.ScenarioActive(0, C(Loudred));
            e.ScenarioAttach(loudred, C(Psychic));
            e.ScenarioAddToHand(0, C(Exploud));
            var dondozo = e.ScenarioActive(1, C(Dondozo));
            e.ScenarioBench(1, C(Remoraid));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Dondozo"));
            Assert.AreNotSame(dondozo, e.State.Players[1].Active);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Sudden Shout"), new ScriptedAgent());
            var exploud = e2.ScenarioActive(0, C(Exploud));
            e2.ScenarioAttach(exploud, C(Psychic));
            var dondozo2 = e2.ScenarioActive(1, C(Dondozo));
            e2.ScenarioBench(1, C(Remoraid));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreSame(dondozo2, e2.State.Players[1].Active, "an Exploud that didn't just evolve does nothing");
        }

        [Test]
        public void Baryon_Beam_Costs_3_Colorless_With_A_Future_Booster_Energy_Capsule()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var jugulis = e.ScenarioActive(0, C(IronJugulis));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioSetTurn(0, 3);

            var beam = AttackNamed(C(IronJugulis), "Baryon Beam");
            Assert.AreEqual(5, e.AttackCost(jugulis, beam).Count);
            e.ScenarioAttachTool(jugulis, C(FutureBooster));
            Assert.AreEqual(3, e.AttackCost(jugulis, beam).Count);
        }

        // ================================================================== Trainers and Energy

        [Test]
        public void Snorlax_Doll_Only_Goes_Into_Play_When_Setting_Up()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioAddToHand(0, C(SnorlaxDoll));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Card != null && o.Card.Name == "Snorlax Doll"), "not from the hand during a turn");
            Assert.IsTrue(C(SnorlaxDoll).IsDoll && C(SnorlaxDoll).IsFossil);
            Assert.AreEqual(120, C(SnorlaxDoll).Hp);

            var deck0 = new List<CardDef> { C(Remoraid) };
            for (int i = 0; i < 59; i++) deck0.Add(C(SnorlaxDoll));
            var deck1 = new List<CardDef>();
            for (int i = 0; i < 60; i++) deck1.Add(C(Remoraid));
            var setup = new GameEngine(StandardCards.CreateRegistry(), new DollSetupAgent(), new ScriptedAgent());
            setup.Setup(deck0, deck1, 7);
            Assert.AreEqual("Snorlax Doll", setup.State.Players[0].Active.Name, "it can be the Active Pokémon at setup");
        }

        [Test]
        public void A_Hand_With_Only_Snorlax_Doll_Can_Be_Kept_Or_Mulliganed()
        {
            var deck0 = new List<CardDef>();
            for (int i = 0; i < 60; i++) deck0.Add(C(SnorlaxDoll));
            var deck1 = new List<CardDef>();
            for (int i = 0; i < 60; i++) deck1.Add(C(Remoraid));

            var keeper = new DollSetupAgent();
            var kept = new GameEngine(StandardCards.CreateRegistry(), keeper, new ScriptedAgent());
            kept.Setup(deck0, deck1, 11);
            Assert.AreEqual(1, keeper.DollQuestions, "the player is asked (your ruling)");
            Assert.AreEqual(0, kept.State.Players[0].Mulligans, "keeping it isn't a mulligan");
            Assert.AreEqual("Snorlax Doll", kept.State.Players[0].Active.Name);

            var mulliganOnce = new DollSetupAgent { MulligansWanted = 1 };
            var redrawn = new GameEngine(StandardCards.CreateRegistry(), mulliganOnce, new ScriptedAgent());
            redrawn.Setup(deck0, deck1, 11);
            Assert.AreEqual(1, redrawn.State.Players[0].Mulligans, "or it can be mulliganed");
            Assert.AreEqual("Snorlax Doll", redrawn.State.Players[0].Active.Name);
        }

        [Test]
        public void Snorlax_Doll_Gives_No_Prize_Cards_Even_With_Luxurious_Cape()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            e.ScenarioActive(1, C(Remoraid));
            var doll = e.ScenarioBench(1, C(SnorlaxDoll));
            e.ScenarioAttachTool(doll, C(LuxuriousCape));
            e.ScenarioSetTurn(1, 4);

            Assert.AreEqual(220, e.MaxHp(doll));
            doll.HitByAttack = true;
            doll.HitByPlayer = 0;
            Assert.AreEqual(0, e.PrizesFor(doll), "Compendium: no Prize cards at all");
            Assert.IsTrue(Offered(e, 1, "Discard Snorlax Doll"));
        }

        [Test]
        public void Luxurious_Cape_Gives_100_HP_And_1_More_Prize_Without_A_Rule_Box()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            var remoraid = e.ScenarioActive(1, C(Remoraid));
            var golisopodEx = e.ScenarioBench(1, C(GolisopodEx));
            e.ScenarioAttachTool(remoraid, C(LuxuriousCape));
            e.ScenarioAttachTool(golisopodEx, C(LuxuriousCape));
            e.ScenarioSetTurn(0, 3);

            Assert.AreEqual(170, e.MaxHp(remoraid));
            Assert.AreEqual(270, e.MaxHp(golisopodEx), "a Pokémon ex has a Rule Box");
            remoraid.HitByAttack = true;
            remoraid.HitByPlayer = 0;
            golisopodEx.HitByAttack = true;
            golisopodEx.HitByPlayer = 0;
            Assert.AreEqual(2, e.PrizesFor(remoraid));
            Assert.AreEqual(2, e.PrizesFor(golisopodEx));
        }

        [Test]
        public void Cursed_Duster_Discards_A_Random_Card_From_The_Attackers_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Aqua Blade"), new ScriptedAgent());
            var golisopodEx = e.ScenarioActive(0, C(GolisopodEx));
            e.ScenarioAttach(golisopodEx, C(Psychic));
            e.ScenarioAttach(golisopodEx, C(Psychic));
            e.ScenarioAddToHand(0, C(Norman));
            var remoraid = e.ScenarioActive(1, C(Remoraid));
            e.ScenarioAttachTool(remoraid, C(CursedDuster));
            e.ScenarioBench(1, C(Remoraid));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Norman"), "the only card in the hand was discarded before the Prize was taken");
        }

        [Test]
        public void Reversal_Energy_Provides_3_Of_Any_Type_When_Behind_But_Counts_As_1_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Diffuse Reflection"), new ScriptedAgent());
            var diancie = e.ScenarioActive(0, C(Diancie));
            e.ScenarioAttach(diancie, C(Psychic));
            var octillery = e.ScenarioActive(1, C(Octillery));
            e.ScenarioAttach(octillery, C(ReversalEnergy));
            e.ScenarioSetTurn(0, 3);

            Assert.AreEqual(1, e.EnergyCount(octillery), "1 Colorless while its owner isn't behind");
            e.State.Players[0].Prizes.RemoveAt(0);
            Assert.AreEqual(3, e.EnergyCount(octillery));
            Assert.IsTrue(e.HasEnergyOfType(octillery, EnergyType.Water));

            Step(e);
            Assert.AreEqual(4, octillery.DamageCounters, "Diffuse Reflection counts Special Energy cards: 40, not 120 (Compendium)");
        }
    }
}
