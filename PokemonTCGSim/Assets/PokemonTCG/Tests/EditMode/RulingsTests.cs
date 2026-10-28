using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>Rulings confirmed by the project owner (see RULES_NOTES.md).</summary>
    public class RulingsTests : ScenarioTestBase
    {
        /// <summary>
        /// Ruling: mulligan bonus draws happen after Active, Bench and Prizes are set,
        /// and before the first player's draw for turn 1.
        /// </summary>
        [Test]
        public void Mulligan_Draws_Happen_After_Pokemon_And_Prizes_Are_Set()
        {
            var basic = C("TWM-128");
            var energy = C("MEE-13");
            var deck0 = Enumerable.Repeat(basic, 60).ToList();
            // One Basic in 60 cards: Player 2 will almost always mulligan at least once.
            var deck1 = new List<CardDef> { basic };
            deck1.AddRange(Enumerable.Repeat(energy, 59));

            for (int seed = 1; seed <= 200; seed++)
            {
                var p1 = new ScriptedAgent();
                var engine = new GameEngine(StandardCards.CreateRegistry(), p1, new ScriptedAgent());
                engine.Setup(deck0, deck1, seed);
                if (engine.State.Players[1].Mulligans == 0) continue;

                var kinds = p1.Seen.Select(r => r.Kind).ToList();
                int drawAt = kinds.IndexOf(DecisionKind.MulliganDraws);
                Assert.GreaterOrEqual(drawAt, 0, "Player 1 should be offered bonus draws");
                Assert.Less(kinds.IndexOf(DecisionKind.SetupBench), drawAt, "Bench is placed before the bonus draws");
                var view = p1.Seen[drawAt].View;
                Assert.IsNotNull(view.Me.Active, "Active is placed before the bonus draws");
                Assert.AreEqual(6, view.Me.PrizeCount, "Prizes are set before the bonus draws");
                Assert.AreEqual(0, engine.State.TurnNumber, "no turn has started yet");
                return;
            }
            Assert.Fail("No seed produced a mulligan; the test deck should make one almost certain.");
        }

        /// <summary>
        /// Ruling (v73.1): when both players mulligan, the same number cancels out; only the player who mulliganed fewer times may
        /// draw, 1 card for each mulligan of the opponent's beyond their own (5 against 6: 1 card for the player with 5).
        /// </summary>
        [Test]
        public void Mulligan_Bonus_Draws_Cancel_Out_When_Both_Players_Mulligan()
        {
            var basic = C("TWM-128");
            var energy = C("MEE-13");
            var deck = new List<CardDef> { basic };
            deck.AddRange(Enumerable.Repeat(energy, 59));
            int checkedGames = 0;
            for (int seed = 1; seed <= 400 && checkedGames < 8; seed++)
            {
                var p0 = new ScriptedAgent();
                var p1 = new ScriptedAgent();
                var engine = new GameEngine(StandardCards.CreateRegistry(), p0, p1);
                engine.Setup(deck, deck.ToList(), seed);
                int m0 = engine.State.Players[0].Mulligans;
                int m1 = engine.State.Players[1].Mulligans;
                if (m0 == 0 || m1 == 0) continue;
                checkedGames++;
                var offer0 = p0.Seen.FirstOrDefault(r => r.Kind == DecisionKind.MulliganDraws);
                var offer1 = p1.Seen.FirstOrDefault(r => r.Kind == DecisionKind.MulliganDraws);
                Assert.AreEqual(System.Math.Max(0, m1 - m0), engine.MulliganDrawsFor(0));
                Assert.AreEqual(System.Math.Max(0, m0 - m1), engine.MulliganDrawsFor(1));
                if (m0 == m1)
                {
                    Assert.IsNull(offer0, "equal mulligans: no extra cards");
                    Assert.IsNull(offer1, "equal mulligans: no extra cards");
                }
                else if (m0 < m1)
                {
                    Assert.IsNull(offer1, "the player who mulliganed more draws nothing");
                    Assert.IsNotNull(offer0);
                    Assert.AreEqual(m1 - m0, offer0.Options.Max(o => o.Index), "only the difference");
                }
                else
                {
                    Assert.IsNull(offer0, "the player who mulliganed more draws nothing");
                    Assert.IsNotNull(offer1);
                    Assert.AreEqual(m0 - m1, offer1.Options.Max(o => o.Index), "only the difference");
                }
            }
            Assert.Greater(checkedGames, 0, "no seed made both players mulligan");
        }

        /// <summary>
        /// Ruling: a Knock Out during Pokémon Checkup happens between turns, so it doesn't count as
        /// "during your opponent's last turn" (Unfair Stamp, Flip the Script). A Knock Out from an attack does.
        /// </summary>
        [Test]
        public void Checkup_Knock_Outs_Do_Not_Enable_Unfair_Stamp_Or_Flip_The_Script()
        {
            // Knocked Out by Poison during Pokémon Checkup after the opponent's turn.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var poisoned = e.ScenarioActive(0, C("TWM-128"));
            poisoned.DamageCounters = 6;
            poisoned.Conditions = SpecialCondition.Poisoned;
            e.ScenarioBench(0, C("ASC-142"));
            e.ScenarioAddToHand(0, C("TWM-165"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(1, 4);
            e.EndTurn();
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "the Poison Knock Out still gives a Prize");
            e.StartTurn();
            var actions = e.GetMainActions(0);
            Assert.IsFalse(actions.Any(o => o.Label.StartsWith("Use Flip the Script")), "Checkup KO shouldn't enable Flip the Script");
            Assert.IsFalse(actions.Any(o => o.Label == "Play Unfair Stamp"), "Checkup KO shouldn't enable Unfair Stamp");

            // Knocked Out by an attack during the opponent's turn.
            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Petty Grudge"));
            var hurt = e2.ScenarioActive(0, C("TWM-128"));
            hurt.DamageCounters = 6;
            e2.ScenarioBench(0, C("ASC-142"));
            e2.ScenarioAddToHand(0, C("TWM-165"));
            var attacker = e2.ScenarioActive(1, C("TWM-128"));
            e2.ScenarioAttach(attacker, C("MEE-13"));
            e2.ScenarioSetTurn(1, 4);
            Step(e2);
            e2.EndTurn();
            e2.StartTurn();
            var actions2 = e2.GetMainActions(0);
            Assert.IsTrue(actions2.Any(o => o.Label.StartsWith("Use Flip the Script")), "attack KO should enable Flip the Script");
            Assert.IsTrue(actions2.Any(o => o.Label == "Play Unfair Stamp"), "attack KO should enable Unfair Stamp");
        }

        /// <summary>Ruling: Night Joker can't pick a Benched N's Zoroark ex's Night Joker (it would do nothing).</summary>
        [Test]
        public void Night_Joker_Cannot_Copy_Night_Joker()
        {
            var p1 = new ScriptedAgent().Then("Attack: Night Joker");
            var e = Scenario(p1, new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C("JTG-98"));
            e.ScenarioAttach(zoroark, C("MEE-15"));
            e.ScenarioAttach(zoroark, C("MEE-15"));
            e.ScenarioBench(0, C("JTG-98")); // a Benched N's Zoroark ex only has Night Joker
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Attack: Night Joker"), "nothing to copy yet");

            e.ScenarioBench(0, C("ASC-155")); // N's Zekrom: Shred and Rampaging Thunder
            Step(e);
            var choice = p1.Seen.First(r => r.Kind == DecisionKind.ChooseOption);
            Assert.AreEqual(2, choice.Options.Count, string.Join(" | ", choice.Options.Select(o => o.Label).ToArray()));
            Assert.IsFalse(choice.Options.Any(o => o.Label.StartsWith("N's Zoroark ex")));
        }

        /// <summary>Ruling: Crushing Hammer can't be played if the opponent has no Energy attached anywhere.</summary>
        [Test]
        public void Crushing_Hammer_Needs_Energy_On_The_Opponents_Board()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            var benched = e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C("POR-71"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Crushing Hammer"), "no Energy to remove");
            e.ScenarioAttach(benched, C("MEE-13"));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play Crushing Hammer"), "Energy on a Benched Pokémon counts");
        }
    }
}
