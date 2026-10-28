using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v124: Dragon Vault (DRV 1-21): the prints, Scornful Storm (your ruling), Grit, Sky Blade and Sky Heal, Axe Slugger, Kyurem's
    /// Blizzard, Dragon Pulse, and First Ticket (your ruling: only with the hands drawn before the coin flip).
    /// </summary>
    public class DrvTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Fire = "MEE-10";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";

        const string Dratini = "DRV-1";
        const string Dragonair = "DRV-3";
        const string Salamence = "DRV-8";
        const string Bagon = "DRV-6";
        const string Shelgon = "DRV-7";
        const string Latias = "DRV-9";
        const string Latios = "DRV-10";
        const string Axew = "DRV-13";
        const string FraxureGrit = "DRV-14";
        const string Fraxure = "DRV-15";
        const string Haxorus = "DRV-16";
        const string FirstTicket = "DRV-19";
        const string Kyurem = "DRV-21";
        const string KyuremLtr = "LTR-43";
        const string Rattata = "G1-82";
        const string Lickitung = "G1-41";

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

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(16, db.Cards.Count(d => d.SetCode == "DRV"));
            Assert.AreEqual("PLB-62", C(Bagon).BaseKey);
            Assert.AreEqual("PLB-63", C(Shelgon).BaseKey);
            Assert.AreEqual("LTR-93", C("DRV-11").BaseKey, "Rayquaza");
            Assert.AreEqual("SVI-174", C("DRV-18").BaseKey, "Exp. Share");
            Assert.AreEqual("PAL-188", C("DRV-20").BaseKey, "Super Rod");
            Assert.AreEqual(EnergyType.Dragon, C(Dratini).PokemonType);
            Assert.AreEqual(EnergyType.Dragon, C(Dratini).Weakness);
            Assert.IsFalse(C(Salamence).UnlimitedOnly, "Expanded");
            Assert.AreEqual("Shelgon", C(Salamence).EvolvesFrom);
        }

        [Test]
        public void Scornful_Storm_Makes_The_Opponent_Discard_Down_To_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Scornful Storm"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            var salamence = e.ScenarioBench(0, C(Bagon));
            e.ScenarioEvolve(salamence, C(Shelgon));
            e.ScenarioEvolve(salamence, C(Salamence));
            e.ScenarioActive(1, C(Rattata));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count, "works from the Bench (your ruling)");
            Assert.AreEqual(3, e.State.Players[1].Discard.Count);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Scornful Storm")), "once a turn, and they have 4 now");
        }

        [Test]
        public void Grit_Adds_40_While_Fraxure_Has_A_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragon Claw"), new ScriptedAgent());
            var fraxure = e.ScenarioActive(0, C(Axew));
            e.ScenarioEvolve(fraxure, C(FraxureGrit));
            Attach(e, fraxure, Fighting, 2);
            Attach(e, fraxure, Metal, 1);
            fraxure.Conditions = SpecialCondition.Poisoned;
            var kyurem = e.ScenarioActive(1, C(KyuremLtr));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, kyurem.DamageCounters, "60 + 40");
        }

        [Test]
        public void Latios_And_Latias_Help_Each_Other_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sky Blade"), new ScriptedAgent());
            var latios = e.ScenarioActive(0, C(Latios));
            Attach(e, latios, Water, 2);
            e.ScenarioBench(0, C(Latias));
            var kyurem = e.ScenarioActive(1, C(KyuremLtr));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, kyurem.DamageCounters, "20 + 20 with Latias on the Bench");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Sky Heal"), new ScriptedAgent());
            var latias = e2.ScenarioActive(0, C(Latias));
            Attach(e2, latias, Fire, 1);
            Attach(e2, latias, Psychic, 1);
            latias.DamageCounters = 3;
            e2.ScenarioBench(0, C(Latios));
            e2.ScenarioActive(1, C(KyuremLtr));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, latias.DamageCounters, "heal 20 with Latios on the Bench");
        }

        [Test]
        public void Axe_Slugger_Does_60_More_To_A_Colorless_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Axe Slugger"), new ScriptedAgent());
            var haxorus = e.ScenarioActive(0, C(Axew));
            e.ScenarioEvolve(haxorus, C(Fraxure));
            e.ScenarioEvolve(haxorus, C(Haxorus));
            Attach(e, haxorus, Fighting, 2);
            var kyurem = e.ScenarioActive(1, C(KyuremLtr));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, kyurem.DamageCounters, "a Water Pokémon: 60");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Axe Slugger"), new ScriptedAgent());
            var haxorus2 = e2.ScenarioActive(0, C(Axew));
            e2.ScenarioEvolve(haxorus2, C(Fraxure));
            e2.ScenarioEvolve(haxorus2, C(Haxorus));
            Attach(e2, haxorus2, Fighting, 2);
            e2.ScenarioActive(1, C(Lickitung));
            e2.ScenarioBench(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(e2.State.Players[1].Discard.Any(c => c.Def.Key == Lickitung), "120 Knocks Out the 80-HP Colorless Lickitung");
        }

        [Test]
        public void Kyurem_Blizzard_Hits_Every_Benched_Pokemon_And_Dragon_Pulse_Discards()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blizzard"), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C(Kyurem));
            Attach(e, kyurem, Water, 1);
            Attach(e, kyurem, Psychic, 3);
            var target = e.ScenarioActive(1, C(KyuremLtr));
            var b1 = e.ScenarioBench(1, C(Rattata));
            var b2 = e.ScenarioBench(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, target.DamageCounters);
            Assert.AreEqual(1, b1.DamageCounters);
            Assert.AreEqual(1, b2.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Dragon Pulse"), new ScriptedAgent());
            var dragonair = e2.ScenarioActive(0, C(Dratini));
            e2.ScenarioEvolve(dragonair, C(Dragonair));
            Attach(e2, dragonair, "MEE-9", 1);
            Attach(e2, dragonair, "MEE-12", 1);
            Attach(e2, dragonair, Water, 1);
            e2.ScenarioActive(1, C(KyuremLtr));
            e2.ScenarioSetTurn(0, 3);
            int deck = e2.State.Players[0].Deck.Count;
            Step(e2);
            Assert.AreEqual(deck - 1, e2.State.Players[0].Deck.Count, "Dragonair's Dragon Pulse discards 1 card");
        }

        private List<CardDef> TicketDeck(int tickets)
        {
            var deck = new List<CardDef>();
            for (int i = 0; i < 20; i++) deck.Add(C(Rattata));
            for (int i = 0; i < tickets; i++) deck.Add(C(FirstTicket));
            while (deck.Count < 60) deck.Add(C(Water));
            return deck;
        }

        private sealed class Events : IGameObserver
        {
            public readonly List<GameEventType> Types = new List<GameEventType>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Types.Add(ev.Type); }
        }

        [Test]
        public void First_Ticket_Lets_A_Player_Go_First_When_Hands_Come_Before_The_Coin_Flip()
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent());
            e.Rules = new GameRules { DrawBeforeCoinFlip = true };
            var log = new Events();
            e.AddObserver(log);
            e.Setup(TicketDeck(0), TicketDeck(35), 11);
            Assert.AreEqual(1, e.State.FirstPlayer, "player 2 played First Ticket");
            Assert.IsFalse(log.Types.Contains(GameEventType.CoinFlip), "no coin flip");
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Key == FirstTicket));
        }

        [Test]
        public void First_Ticket_Does_Nothing_With_The_Normal_Setup_Or_When_Both_Play_It()
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent());
            var log = new Events();
            e.AddObserver(log);
            e.Setup(TicketDeck(0), TicketDeck(35), 11);
            Assert.IsTrue(log.Types.Contains(GameEventType.CoinFlip), "the normal setup flips first (your ruling)");
            Assert.AreEqual(0, e.State.Players[1].Discard.Count);

            var e2 = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent());
            e2.Rules = new GameRules { DrawBeforeCoinFlip = true };
            var log2 = new Events();
            e2.AddObserver(log2);
            e2.Setup(TicketDeck(35), TicketDeck(35), 11);
            Assert.IsTrue(log2.Types.Contains(GameEventType.CoinFlip), "both played it: the coin flip decides");

            var e3 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e3.ScenarioActive(0, C(Rattata));
            e3.ScenarioAddToHand(0, C(FirstTicket));
            e3.ScenarioActive(1, C(Rattata));
            e3.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e3.GetMainActions(0).Any(o => o.Label == "Play First Ticket"), "never during a game");
        }
    }
}
