using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>v76: reading TCG Live battle logs, playing back games saved by this app, learning from both, and Tie-Breaker games.</summary>
    public class LogStudyTests : ScenarioTestBase
    {
        private static readonly string[] AllLogs = { SampleLogs.Crustle, SampleLogs.Zeraora, SampleLogs.Metang, SampleLogs.Gengar };

        private List<CardDef> Dragapult() { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); }
        private List<CardDef> Zoroark() { return UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck); }

        // ------------------------------------------------------------------ Reading TCG Live logs

        [Test]
        public void A_Battle_Log_Is_Split_Into_Players_Turns_And_Lines()
        {
            var log = TcgLiveLog.Parse(SampleLogs.Crustle);
            CollectionAssert.AreEqual(new[] { "Ivanmp1702", "TFR89" }, log.Players, "the first player is player 0");
            Assert.AreEqual("Ivanmp1702", log.FirstPlayer);
            Assert.AreEqual("TFR89", log.Winner);
            StringAssert.Contains("conceded", log.Ending);
            Assert.AreEqual(6, log.TurnCount);
            var opening = log.Turns[0].Lines.First(l => l.Text.StartsWith("TFR89 drew 7 cards", StringComparison.Ordinal));
            Assert.AreEqual(7, opening.Subs[0].Cards.Count, "the opening hand is listed under \"- 7 drawn cards.\"");
            Assert.AreEqual("Lillie's Determination", opening.Subs[0].Cards[0]);
            var lillie = log.Turns[2].Lines.First(l => l.Text == "TFR89 played Lillie's Determination.");
            Assert.AreEqual(2, lillie.Subs.Count);
            Assert.AreEqual(8, lillie.Subs[1].Cards.Count);
        }

        [Test]
        public void Sudden_Death_Starts_A_New_Game_In_The_Log()
        {
            var log = TcgLiveLog.Parse(SampleLogs.Gengar);
            Assert.AreEqual(19, log.TurnCount);
            Assert.AreEqual(1, log.Turns.Count(t => t.SuddenDeath));
            var first = log.Turns.First(t => t.Game == 1 && t.Number > 0);
            Assert.AreEqual(1, first.Number, "turns count from 1 again");
            Assert.AreEqual("trev386", first.Player);
            Assert.AreEqual("trev386", log.FirstPlayer);
            Assert.AreEqual("TFR89", log.Winner);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Every_Line_Of_The_Sample_Logs_Is_Understood(int which)
        {
            var report = LogStudy.Study(AllLogs[which], "sample " + which, db, StandardCards.CreateRegistry(), null);
            Assert.IsNull(report.Error, report.Error);
            Assert.AreEqual(0, report.UnreadLines, string.Join("\n", report.Unread.ToArray()));
            Assert.AreEqual("TFR89", report.Winner);
            Assert.Greater(report.Plays, 0);
            Assert.Greater(report.Decisions[Array.IndexOf(report.Players, "TFR89")], 0, report.Summary());
        }

        [Test]
        public void The_Log_Owners_Plays_Line_Up_With_What_The_Game_Allowed()
        {
            int plays = 0, matched = 0;
            foreach (var text in AllLogs)
            {
                var report = LogStudy.Study(text, "sample", db, StandardCards.CreateRegistry(), null);
                plays += report.Plays;
                matched += report.Matched;
            }
            Assert.GreaterOrEqual(matched * 2, plays, matched + " of " + plays + " plays lined up");
            var crustle = LogStudy.Study(SampleLogs.Crustle, "Crustle", db, StandardCards.CreateRegistry(), null);
            Assert.GreaterOrEqual(crustle.Decisions[1], 10, crustle.Summary());
        }

        [Test]
        public void Studying_A_Battle_Log_Teaches_The_AI()
        {
            var brain = new LearningBrain();
            var report = LogStudy.Study(SampleLogs.Metang, "Metang", db, StandardCards.CreateRegistry(), brain);
            Assert.IsNull(report.Error, report.Error);
            Assert.Greater(report.TotalStudied, 0, report.Summary());
            Assert.Greater(brain.Net.Updates, 0, "the network studied the plays");
            Assert.AreEqual(1, brain.LogsStudied);
            Assert.AreEqual(report.TotalStudied, brain.LogDecisionsStudied);
            StringAssert.Contains("concession", report.Note, "TCG Live concedes for a player whose timer runs out");
            Assert.AreEqual(0.75, LogStudy.Weight(0, 1, true), 1e-9);
            Assert.AreEqual(0.5, LogStudy.Weight(0, 1, false), 1e-9);
            Assert.AreEqual(1.0, LogStudy.Weight(1, 1, true), 1e-9);
        }

        [Test]
        public void Text_That_Is_Not_A_Log_Is_Turned_Down()
        {
            var report = LogStudy.Study("4 Dreepy TWM 128\n4 Drakloak TWM 129\n", "a deck", db, StandardCards.CreateRegistry(), null);
            Assert.IsNotNull(report.Error);
        }

        [Test]
        public void The_Logs_Studied_Are_Saved_With_The_Brain()
        {
            var brain = new LearningBrain();
            brain.NoteLogStudied(17);
            brain.NoteLogStudied(3);
            var back = new LearningBrain();
            back.LoadText(brain.Serialize());
            Assert.AreEqual(2, back.LogsStudied);
            Assert.AreEqual(20, back.LogDecisionsStudied);
            back.Reset();
            Assert.AreEqual(0, back.LogsStudied);
        }

        // ------------------------------------------------------------------ Games saved by this app

        private GameRecord PlayRecordedGame(out GameResult result)
        {
            var d0 = Dragapult();
            var d1 = Zoroark();
            var rec = new GameRecord { Seed = 4321, Names = new[] { "A", "B" }, DeckNames = new[] { "Dragapult", "Zoroark" } };
            rec.Decks[0].AddRange(d0);
            rec.Decks[1].AddRange(d1);
            var engine = new GameEngine(StandardCards.CreateRegistry(), GameRecord.Wrap(new HeuristicAgent(5), rec, 0), GameRecord.Wrap(new HeuristicAgent(6), rec, 1));
            engine.CardsPerPlayer = 60;
            result = engine.Run(d0, d1, rec.Seed);
            rec.Today = engine.Today;
            rec.Winner = result.Winner;
            rec.Turns = result.Turns;
            rec.Reason = result.Reason;
            return rec;
        }

        [Test]
        public void A_Game_From_This_App_Is_Saved_And_Played_Back_Exactly()
        {
            GameResult result;
            var rec = PlayRecordedGame(out result);
            Assert.Greater(rec.AnswerCount(0), 0);
            string text = rec.Serialize();
            Assert.IsTrue(GameRecord.IsRecord(text));
            var problems = new List<string>();
            var back = GameRecord.Parse(text, db, problems);
            Assert.AreEqual(0, problems.Count, string.Join("\n", problems.ToArray()));
            for (int s = 0; s < 2; s++)
            {
                Assert.AreEqual(rec.Answers[s].Count, back.Answers[s].Count);
                CollectionAssert.AreEqual(rec.Decks[s].Select(d => d.Key).ToList(), back.Decks[s].Select(d => d.Key).ToList(), "the deck order is kept");
            }
            Assert.AreEqual(rec.Winner, back.Winner);

            var brain = new LearningBrain();
            var report = LogStudy.Study(text, "game", db, StandardCards.CreateRegistry(), brain);
            Assert.IsNull(report.Error, report.Error);
            Assert.IsTrue(report.Complete, report.Note);
            Assert.AreEqual("", report.Note, "it played out the same way");
            Assert.AreEqual(result.Turns, report.Turns);
            for (int s = 0; s < 2; s++) Assert.AreEqual(rec.Answers[s].Count, report.Decisions[s], "every decision came back, seat " + s);
            Assert.Greater(report.TotalStudied, 0);
            Assert.AreEqual(1, brain.LogsStudied);
        }

        [Test]
        public void A_Record_That_No_Longer_Fits_Is_Studied_Up_To_There()
        {
            GameResult result;
            var rec = PlayRecordedGame(out result);
            var problems = new List<string>();
            var back = GameRecord.Parse(rec.Serialize(), db, problems);
            back.Answers[1][back.Answers[1].Count / 2] = new List<int> { 999 };
            var report = LogStudy.StudyRecord(back, StandardCards.CreateRegistry(), null);
            Assert.IsNull(report.Error);
            Assert.IsFalse(report.Complete);
            StringAssert.Contains("doesn't fit", report.Note);
            Assert.Greater(report.Decisions[0], 0, "the decisions before it still count");
        }

        // ------------------------------------------------------------------ Tie-Breaker games

        [TestCase(false)]
        [TestCase(true)]
        public void A_Tie_Breaker_Game_Ends_At_The_First_Prize_Lead(bool tieBreaker)
        {
            var p1 = new ScriptedAgent().Then("Attack: Phantom Dive");
            for (int i = 0; i < 6; i++) p1.Then("Opponent's Dreepy (Bench 1)");
            var e = Scenario(p1, new ScriptedAgent());
            e.TieBreaker = tieBreaker;
            var drag = e.ScenarioActive(0, C("TWM-130"));
            e.ScenarioAttach(drag, C("MEE-10"));
            e.ScenarioAttach(drag, C("MEE-13"));
            e.ScenarioActive(1, C("TWM-95"));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "Munkidori is Knocked Out for 1 Prize card");
            if (tieBreaker)
            {
                Assert.IsTrue(e.State.IsOver, "5 Prize cards left to 6 is a lead");
                Assert.AreEqual(0, e.State.Winner);
                StringAssert.Contains("Tie-Breaker", e.State.EndReason);
            }
            else Assert.IsFalse(e.State.IsOver, "an ordinary game goes on");
        }

        [Test]
        public void Prize_Cards_Taken_At_The_Same_Time_Are_Compared_After_Both_Are_Taken()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.TieBreaker = true;
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            // Both players take a Prize card (a Cursed Blast that Knocks Out a 1-Prize Pokémon): level, no lead.
            e.State.Players[0].Prizes.RemoveAt(0);
            e.State.Players[1].Prizes.RemoveAt(0);
            try { e.CheckWinConditions(); }
            catch (GameOverException) { }
            Assert.IsFalse(e.State.IsOver, "1 to 1 is no lead");
            // Cursed Blast on a Pokémon ex: 2 Prize cards to 1 is a lead (your ruling).
            e.State.Players[1].Prizes.RemoveAt(0);
            try { e.CheckWinConditions(); }
            catch (GameOverException) { }
            Assert.IsTrue(e.State.IsOver);
            Assert.AreEqual(1, e.State.Winner);
        }

        [Test]
        public void A_Drawn_Top_Cut_Game_Is_Followed_By_A_Tie_Breaker_Game()
        {
            var cut = new RegionalMatch { A = 0, B = 1, SeedA = 1, SeedB = 2 };
            Assert.IsFalse(RegionalSim.TieBreakerNext(cut), "game 1 is an ordinary game");
            cut.Games.Add(-1);
            Assert.IsTrue(RegionalSim.TieBreakerNext(cut), "after a drawn game");
            cut.Games.Add(0);
            Assert.IsFalse(RegionalSim.TieBreakerNext(cut), "after a decided game");
            var swiss = new RegionalMatch { A = 0, B = 1 };
            swiss.Games.Add(-1);
            Assert.IsFalse(RegionalSim.TieBreakerNext(swiss), "Swiss games can be drawn");
        }

        [Test]
        public void A_Tie_Breaker_Game_Is_Saved_In_Its_Record()
        {
            var rec = new GameRecord { TieBreaker = true };
            var back = GameRecord.Parse(rec.Serialize(), db, new List<string>());
            Assert.IsTrue(back.TieBreaker);
            Assert.IsFalse(GameRecord.Parse(new GameRecord().Serialize(), db, new List<string>()).TieBreaker);
        }
    }
}
