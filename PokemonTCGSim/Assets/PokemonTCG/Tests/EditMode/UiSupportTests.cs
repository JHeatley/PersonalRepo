using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>Engine data the game screen relies on: the "All" tab of card choices, revealed cards and mulligan hands.</summary>
    public class UiSupportTests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Psychic = "MEE-13";
        const string Fire = "MEE-10";
        const string Grass = "MEE-9";
        const string MegaVenusaur = "MEG-3"; // Solar Transfer: as many times as you like, move a Basic Grass Energy

        private sealed class EventLog : IGameObserver
        {
            public readonly List<GameEvent> Events = new List<GameEvent>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Events.Add(ev); }
        }

        /// <summary>Plays like the person on screen (IInteractiveAgent), answering like a ScriptedAgent.</summary>
        private sealed class PersonAgent : IInteractiveAgent
        {
            public readonly ScriptedAgent Inner = new ScriptedAgent();
            public List<DecisionRequest> Seen { get { return Inner.Seen; } }
            public List<int> Decide(DecisionRequest r) { return Inner.Decide(r); }
        }

        /// <summary>Answers with a function (to act like the UI, which can pick which copy of a card to play).</summary>
        private sealed class FuncAgent : IPlayerAgent
        {
            private readonly System.Func<DecisionRequest, List<int>> decide;
            public FuncAgent(System.Func<DecisionRequest, List<int>> decide) { this.decide = decide; }
            public List<int> Decide(DecisionRequest r) { return decide(r); }
        }

        private GameEngine ScenarioWith(IPlayerAgent p1, IPlayerAgent p2)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), p1, p2);
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            return e;
        }

        [Test]
        public void A_Search_That_Finds_Nothing_Still_Shows_The_Deck_To_A_Person()
        {
            var person = new PersonAgent();
            var e = ScenarioWith(person, new ScriptedAgent());
            var found = e.SearchDeck(0, d => d.IsBasicPokemon, 1, "Choose a Basic Pokémon", null, true);
            Assert.AreEqual(0, found.Count);
            var req = person.Seen.Last();
            Assert.AreEqual(DecisionKind.ChooseCards, req.Kind);
            Assert.AreEqual(0, req.Options.Count, "nothing can be taken");
            Assert.AreEqual(0, req.MaxChoices);
            Assert.AreEqual(20, req.SeenCards.Count, "but the whole deck is there to look at");
            Assert.AreEqual("Your deck", req.SeenTitle);
        }

        [Test]
        public void Looking_At_Cards_With_Nothing_To_Take_Still_Shows_Them_To_A_Person()
        {
            var person = new PersonAgent();
            person.Inner.Then("Play Bug Catching Set");
            var e = ScenarioWith(person, new ScriptedAgent());
            e.ScenarioAddToHand(0, C("TWM-143"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var req = person.Seen.Last();
            Assert.AreEqual(DecisionKind.ChooseCards, req.Kind);
            Assert.AreEqual(0, req.Options.Count, "no Grass Pokémon or Grass Energy in the top 7");
            Assert.AreEqual(7, req.SeenCards.Count, "but the 7 cards are shown, so it's clear Bug Catching Set worked");
            Assert.AreEqual(20, e.State.Players[0].Deck.Count, "and they're shuffled back");

            var bot = new ScriptedAgent().Then("Play Bug Catching Set");
            var b = ScenarioWith(bot, new ScriptedAgent());
            b.ScenarioAddToHand(0, C("TWM-143"));
            b.ScenarioSetTurn(0, 3);
            Step(b);
            Assert.IsFalse(bot.Seen.Any(r => r.Kind == DecisionKind.ChooseCards), "a bot isn't asked");
        }

        [Test]
        public void A_Bot_Is_Not_Asked_When_A_Search_Finds_Nothing()
        {
            var bot = new ScriptedAgent();
            var e = ScenarioWith(bot, new ScriptedAgent());
            Assert.AreEqual(0, e.SearchDeck(0, d => d.IsBasicPokemon, 1, "Choose a Basic Pokémon", null, true).Count);
            Assert.AreEqual(0, bot.Seen.Count);
        }

        [Test]
        public void A_Person_Picks_Cards_From_The_Discard_Pile_Even_With_One_Possible_Answer()
        {
            var person = new PersonAgent();
            var e = ScenarioWith(person, new ScriptedAgent());
            e.ScenarioAddToDiscard(0, C("MEE-9"), 1);
            var picks = new EffectContext(e, 0, null, null).ChooseCards(e.State.Players[0].Discard.ToList(), 1, 1, "Put an Energy into your hand");
            Assert.AreEqual(1, picks.Count);
            Assert.AreEqual(1, person.Seen.Count(r => r.Kind == DecisionKind.ChooseCards), "the person was asked");
            Assert.AreEqual("Your discard pile", person.Seen.Last().SeenTitle);

            var bot = new ScriptedAgent();
            var e2 = ScenarioWith(bot, new ScriptedAgent());
            e2.ScenarioAddToDiscard(0, C("MEE-9"), 1);
            new EffectContext(e2, 0, null, null).ChooseCards(e2.State.Players[0].Discard.ToList(), 1, 1, "Put an Energy into your hand");
            Assert.AreEqual(0, bot.Seen.Count, "a bot gets the only answer at once");
        }

        [Test]
        public void Identical_Cards_In_The_Hand_Share_An_Option_That_Can_Play_Either_Copy()
        {
            CardInstance first = null, second = null;
            var agent = new FuncAgent(r =>
            {
                if (r.Kind != DecisionKind.MainAction) return Enumerable.Range(0, System.Math.Max(r.MinChoices, 0)).ToList();
                int i = r.Options.FindIndex(o => o.Action == ActionType.Evolve);
                Assert.IsTrue(r.Options[i].UsesCard(second.Id), "the option also stands for the second copy");
                r.Options[i].UseCopy(second.Id); // what the UI does when you drag the second copy
                return new List<int> { i };
            });
            var e = ScenarioWith(agent, new ScriptedAgent());
            var tepig = e.ScenarioBench(0, C("WHT-11"));
            first = e.ScenarioAddToHand(0, C("WHT-12"));
            second = e.ScenarioAddToHand(0, C("WHT-12"));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.GetMainActions(0).Count(o => o.Action == ActionType.Evolve), "listed once");
            e.MainPhaseStep();
            Assert.AreEqual("Pignite", tepig.Name);
            Assert.AreSame(second, tepig.Top, "the copy that was dragged");
            CollectionAssert.Contains(e.State.Players[0].Hand, first);
        }

        [Test]
        public void A_Person_Benches_Basic_Pokemon_One_At_A_Time_At_Setup()
        {
            var person = new PersonAgent();
            var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
            var deck = Enumerable.Repeat(C(Dreepy), 60).ToList();
            e.Setup(deck, deck, 7);
            var benchAsks = person.Seen.Where(r => r.Kind == DecisionKind.SetupBench).ToList();
            Assert.AreEqual(5, benchAsks.Count, "one question per Pokémon benched (each answered by dragging a card)");
            Assert.IsTrue(benchAsks.All(r => r.IsSingleChoice && r.Options.Last().Label.StartsWith("Done")));
            Assert.AreEqual(5, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void A_Person_Calls_The_Opening_Coin_Flip()
        {
            foreach (var call in new[] { "Heads", "Tails" })
            {
                var person = new PersonAgent();
                person.Inner.Then(call);
                var log = new EventLog();
                var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
                e.AddObserver(log);
                var deck = Enumerable.Repeat(C(Dreepy), 60).ToList();
                e.Setup(deck, deck, 7);

                Assert.AreEqual(DecisionKind.CallCoinFlip, person.Seen[0].Kind, "the first question of the game");
                var flip = log.Events.First(ev => ev.Type == GameEventType.CoinFlip);
                bool heads = flip.Amount == 1;
                Assert.AreEqual(heads == (call == "Heads") ? 0 : 1, flip.Player, "the call decides who won");
                StringAssert.Contains("called " + call.ToLowerInvariant(), flip.Text);
            }
        }

        [Test]
        public void Bots_Are_Not_Asked_To_Call_The_Coin_Flip()
        {
            var a = new ScriptedAgent();
            var b = new ScriptedAgent();
            var log = new EventLog();
            var e = new GameEngine(StandardCards.CreateRegistry(), a, b);
            e.AddObserver(log);
            var deck = Enumerable.Repeat(C(Dreepy), 60).ToList();
            e.Setup(deck, deck, 7);

            Assert.IsFalse(a.Seen.Concat(b.Seen).Any(r => r.Kind == DecisionKind.CallCoinFlip));
            var flip = log.Events.First(ev => ev.Type == GameEventType.CoinFlip);
            Assert.AreEqual(flip.Amount == 1 ? 0 : 1, flip.Player, "heads: seat 0 wins, as before");
        }

        [Test]
        public void A_Person_Places_Their_Active_Themselves_Even_With_One_Basic()
        {
            // Only one kind of Basic (Dreepy) in the deck, so there's a single choice: bots take it automatically, but the
            // person on screen is still asked, so they can drag it out of their hand like every other play.
            var person = new PersonAgent();
            var bot = new ScriptedAgent();
            var e = new GameEngine(StandardCards.CreateRegistry(), person, bot);
            var deck = new List<CardDef>();
            for (int i = 0; i < 4; i++) deck.Add(C(Dreepy));
            while (deck.Count < 60) deck.Add(C(Psychic));
            e.Setup(deck, deck, 7);

            var asks = person.Seen.Where(r => r.Kind == DecisionKind.SetupActive).ToList();
            Assert.AreEqual(1, asks.Count, "asked even though there's only one choice");
            Assert.AreEqual(1, asks[0].Options.Count);
            Assert.IsFalse(bot.Seen.Any(r => r.Kind == DecisionKind.SetupActive), "a bot isn't asked a question with one answer");
            Assert.IsNotNull(e.State.Players[0].Active);
        }

        [Test]
        public void A_Search_Shows_The_Whole_Deck_In_The_All_Tab()
        {
            var me = new ScriptedAgent();
            var e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C(Dreepy), 2); // 20 Psychic Energy + 2 Dreepy
            var found = e.SearchDeck(0, d => d.IsBasicPokemon, 1, "Choose a Basic Pokémon", null, true);
            Assert.AreEqual(1, found.Count);
            var req = me.Seen.Last();
            Assert.AreEqual(2, req.Options.Count, "Valid: the 2 Dreepy");
            Assert.IsNotNull(req.SeenCards);
            Assert.AreEqual(22, req.SeenCards.Count, "All: every card in the deck");
        }

        [Test]
        public void Looking_At_The_Top_Cards_Shows_Them_All()
        {
            var me = new ScriptedAgent();
            var e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Dreepy)));
            e.ScenarioSetTurn(0, 3);
            var ctx = new EffectContext(e, 0, null, null);
            var top = ctx.LookAtTop(7);
            ctx.ChooseCards(top.Where(c => c.Def.IsPokemon).ToList(), 0, 1, "Take a Pokémon");
            var req = me.Seen.Last();
            Assert.AreEqual(1, req.Options.Count);
            Assert.AreEqual(7, req.SeenCards.Count);
            StringAssert.Contains("Top 7", req.SeenTitle);

            // After a shuffle the looked-at cards are forgotten: a later choice from the hand shows the hand instead.
            ctx.ShuffleMyDeck();
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(0, C(Dreepy));
            // (A choice of 0-1, so it isn't answered automatically.)
            ctx.ChooseCards(e.State.Players[0].Hand.Where(c => c.Def.IsBasicEnergy).ToList(), 0, 1, "Discard an Energy");
            req = me.Seen.Last();
            Assert.AreEqual("Your hand", req.SeenTitle);
            Assert.AreEqual(2, req.SeenCards.Count);
        }

        [Test]
        public void Played_Cards_Are_Named_On_Public_Events_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Dreepy"), new ScriptedAgent());
            var log = new EventLog();
            e.AddObserver(log);
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var bench = log.Events.First(ev => ev.Type == GameEventType.PutOnBench);
            Assert.IsNotNull(bench.Card);
            Assert.AreEqual("Dreepy", bench.Card.Name);
            e.DrawCards(0, 1);
            Assert.IsNull(log.Events.Last(ev => ev.Type == GameEventType.Draw).Card, "drawn cards stay hidden");
        }

        // ------------------------------------------------------------------ "As often as you like" Abilities

        [Test]
        public void A_Person_Attaches_As_Many_As_They_Like_With_One_Use_Then_Presses_Done()
        {
            var person = new PersonAgent();
            person.Inner.Then("Use Inferno Fandango", "Emboar", "Dreepy", "Done");
            var e = ScenarioWith(person, new ScriptedAgent());
            var emboar = e.ScenarioActive(0, C("WHT-11"));
            e.ScenarioEvolve(emboar, C("WHT-12"));
            e.ScenarioEvolve(emboar, C("WHT-13"));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.AreEqual(1, emboar.Energy.Count);
            Assert.AreEqual(1, dreepy.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.IsBasicEnergy), "Done left the third one in the hand");
            Assert.AreEqual(1, person.Seen.Count(r => r.Kind == DecisionKind.MainAction), "one use of the Ability");
            Assert.IsTrue(person.Seen.Where(r => r.Kind == DecisionKind.ChoosePokemon).All(r => r.Options.Any(o => o.Label == RepeatableAbility.DoneLabel)));
        }

        [Test]
        public void A_Person_Moves_As_Many_As_They_Like_With_One_Use_Then_Presses_Done()
        {
            var person = new PersonAgent();
            person.Inner.Then("Use Solar Transfer", "Dreepy", "Venusaur", "Dreepy", "Venusaur", "Done");
            var e = ScenarioWith(person, new ScriptedAgent());
            var venusaur = e.ScenarioActive(0, C(MegaVenusaur));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            e.ScenarioAttach(dreepy, C(Grass));
            e.ScenarioAttach(dreepy, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.AreEqual(2, venusaur.Energy.Count);
            Assert.AreEqual(0, dreepy.Energy.Count);
            Assert.AreEqual(1, person.Seen.Count(r => r.Kind == DecisionKind.MainAction), "one use of the Ability");
        }

        [Test]
        public void A_Bot_Still_Moves_One_Energy_Per_Use()
        {
            var bot = new ScriptedAgent().Then("Use Solar Transfer", "Dreepy", "Venusaur");
            var e = ScenarioWith(bot, new ScriptedAgent());
            var venusaur = e.ScenarioActive(0, C(MegaVenusaur));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            e.ScenarioAttach(dreepy, C(Grass));
            e.ScenarioAttach(dreepy, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.AreEqual(1, venusaur.Energy.Count);
            Assert.AreEqual(1, dreepy.Energy.Count);
            Assert.IsFalse(bot.Seen.Any(r => r.Options.Any(o => o.Label == RepeatableAbility.DoneLabel)), "bots never get the Done button");
        }

        [Test]
        public void Mulligan_Hands_Are_Recorded_And_Revealed()
        {
            // A deck whose first Basic is far down: shuffling decides how many mulligans happen, so only check consistency.
            var deck = new List<CardDef>();
            for (int i = 0; i < 4; i++) deck.Add(C(Dreepy));
            while (deck.Count < 60) deck.Add(C(Psychic));
            var e = new GameEngine(PokemonTCG.Cards.StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent());
            var log = new EventLog();
            e.AddObserver(log);
            e.Setup(deck, deck, 7);
            for (int p = 0; p < 2; p++)
            {
                var state = e.State.Players[p];
                Assert.AreEqual(state.Mulligans, state.MulliganHands.Count);
                foreach (var hand in state.MulliganHands)
                {
                    Assert.AreEqual(7, hand.Count);
                    Assert.IsFalse(hand.Any(c => c.Def.IsBasicPokemon));
                }
                int reveals = log.Events.Count(ev => ev.Type == GameEventType.MulliganReveal && ev.Player == p);
                Assert.AreEqual(state.Mulligans > 0 ? 1 : 0, reveals);
                var view = PlayerView.Create(e, 1 - p);
                Assert.AreEqual(state.Mulligans, view.Sides[p].MulliganHands.Count, "both players see the mulligan hands");
            }
        }

        // ------------------------------------------------------------------ Prize cards (v42)

        /// <summary>Great Tusk ex (Gigantic Tusks, 250) against a Great Tusk ex: a 2-Prize Knock Out. benchedPidgey: the opponent can promote.</summary>
        private GameEngine PrizeScenario(IPlayerAgent attacker, int prizes, bool benchedPidgey)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), attacker, new ScriptedAgent());
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C(Fire), prizes);
            e.ScenarioAddPrizes(1, C(Fire), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            var tusk = e.ScenarioActive(0, C("SVI-123"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tusk, C("MEE-14"));
            e.ScenarioActive(1, C("SVI-123"));
            if (benchedPidgey) e.ScenarioBench(1, C("OBF-162"));
            e.ScenarioSetTurn(0, 3);
            return e;
        }

        [Test]
        public void A_Person_Chooses_Which_Prize_Cards_To_Take_And_The_Rest_Stay_In_Place()
        {
            var person = new PersonAgent();
            person.Inner.Then("Attack: Gigantic Tusks", "Take Prize card 4", "Take Prize card 2");
            var e = PrizeScenario(person, 6, true);
            var before = e.State.Players[0].Prizes.ToList();
            Step(e);

            var asks = person.Seen.Where(r => r.Kind == DecisionKind.ChoosePrize).ToList();
            Assert.AreEqual(2, asks.Count, "one question per Prize card");
            Assert.AreEqual(6, asks[0].Options.Count);
            Assert.AreEqual(5, asks[1].Options.Count);
            StringAssert.Contains("(1 of 2)", asks[0].Prompt);
            Assert.IsTrue(asks.All(r => r.Options.All(o => o.Card == null && o.CardId < 0)), "face-down Prize cards stay hidden");
            var p = e.State.Players[0];
            Assert.AreEqual(4, p.Prizes.Count);
            Assert.IsTrue(p.Hand.Contains(before[3]) && p.Hand.Contains(before[1]), "slots 4 and 2");
            var view = PlayerView.Create(e, 0);
            CollectionAssert.AreEqual(new[] { 0, 2, 4, 5 }, view.Sides[0].PrizePositions.ToArray(), "the other Prize cards keep their places");
        }

        [Test]
        public void The_Last_Prize_Cards_Are_Taken_Without_Asking()
        {
            var person = new PersonAgent();
            person.Inner.Then("Attack: Gigantic Tusks");
            var e = PrizeScenario(person, 2, true);
            Step(e);
            Assert.IsFalse(person.Seen.Any(r => r.Kind == DecisionKind.ChoosePrize), "the game is over");
            Assert.AreEqual(0, e.State.Players[0].Prizes.Count);
            Assert.IsTrue(e.State.IsOver);

            var benchOut = new PersonAgent();
            benchOut.Inner.Then("Attack: Gigantic Tusks");
            var b = PrizeScenario(benchOut, 6, false);
            Step(b);
            Assert.IsFalse(benchOut.Seen.Any(r => r.Kind == DecisionKind.ChoosePrize), "no Pokémon left: the game is over too");
            Assert.AreEqual(4, b.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Bots_Take_The_First_Prize_Cards_Without_Being_Asked()
        {
            var bot = new ScriptedAgent().Then("Attack: Gigantic Tusks");
            var e = PrizeScenario(bot, 6, true);
            var before = e.State.Players[0].Prizes.ToList();
            Step(e);
            Assert.IsFalse(bot.Seen.Any(r => r.Kind == DecisionKind.ChoosePrize));
            var p = e.State.Players[0];
            Assert.IsTrue(p.Hand.Contains(before[0]) && p.Hand.Contains(before[1]));
        }
    }
}
