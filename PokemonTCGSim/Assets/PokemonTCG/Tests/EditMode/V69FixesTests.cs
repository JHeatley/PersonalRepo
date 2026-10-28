using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v69: fixes from your bug list (deck import, setup, card effects).</summary>
    public class V69FixesTests : ScenarioTestBase
    {
        const string Tauros = "MEW-128";
        const string Dreepy = "TWM-128";
        const string Fire = "MEE-10";
        const string Psychic = "MEE-13";
        const string SuperiorEnergyRetrieval = "PAL-189";
        const string HisuianHeavyBall = "ASR-146";
        const string PokeStop = "PGO-68";

        /// <summary>A seat a person plays on screen (the engine shows it everything), answered by a function.</summary>
        private sealed class Person : IInteractiveAgent
        {
            public readonly List<DecisionRequest> Seen = new List<DecisionRequest>();
            private readonly ScriptedAgent script = new ScriptedAgent();
            private readonly Func<DecisionRequest, List<int>> answer;

            public Person(Func<DecisionRequest, List<int>> answer) { this.answer = answer; }
            public Person Then(params string[] labels) { script.Then(labels); return this; }

            public List<int> Decide(DecisionRequest r)
            {
                Seen.Add(r);
                var own = answer == null ? null : answer(r);
                return own ?? script.Decide(r);
            }
        }

        private GameEngine PersonScenario(Person person)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C(Fire), 6);
            e.ScenarioAddPrizes(1, C(Fire), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            return e;
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }
        // ================================================================== Deck import

        [Test]
        public void Deck_Lists_With_The_Set_And_Number_In_Brackets_Import()
        {
            var errors = new List<string>();
            var text = "2 Charmander (PAF-7)\n1 Charmander (PFL-11)\n2 Charmeleon (PFL-12)\n1 Mega Charizard X ex (PFL-13)\n"
                       + "1 Mega Charizard X ex (MEP-29)\n2 Tepig (WHT-11)\n1 Pignite (ASC-30)\n2 Emboar (WHT-13)\n1 Budew (PRE-4)\n"
                       + "1 Budew [ASC 16]\n1 Psyduck (MEP-7)\n3 Poke Pad\n1 Budew PRE-4\n";
            var entries = Decklist.Parse(text, db, errors);
            CollectionAssert.IsEmpty(errors, string.Join("; ", errors.ToArray()));
            Assert.AreEqual(13, entries.Count);
            Assert.AreEqual("PAF-7", entries[0].Card.Key);
            Assert.AreEqual("Charmander", entries[1].Card.Name);
            Assert.AreEqual("Mega Charizard X ex", entries[4].Card.Name, "a promo printing of it");
            Assert.AreEqual("Budew", entries[8].Card.Name);
            Assert.AreEqual("Poké Pad", entries[11].Card.Name, "typed without the accent");
            Assert.AreEqual(3, entries[11].Count);
        }

        [Test]
        public void Card_Names_Match_Without_Accents_Or_Apostrophes()
        {
            Assert.AreEqual("Poké Pad", db.GetByLooseName("poke pad").Name);
            Assert.AreEqual("Boss's Orders", db.GetByLooseName("Boss’s Orders").Name, "a curly apostrophe");
            Assert.AreEqual("Boss's Orders", db.GetByLooseName("Bosss Orders").Name);
            Assert.IsNull(db.GetByLooseName("Not A Card"));
            var errors = new List<string>();
            var entries = Decklist.Parse("4 Pokegear 3.0\n2 Bosses Orders\n", db, errors);
            Assert.AreEqual("Pokégear 3.0", entries[0].Card.Name);
            Assert.AreEqual(1, errors.Count, "a different word still isn't a card");
        }

        // ================================================================== Setup

        [Test]
        public void Basic_Pokemon_Can_Go_Onto_The_Bench_After_The_Mulligan_Draws()
        {
            var basics = Enumerable.Repeat(C(Dreepy), 60).ToList();
            // One Basic in 60 cards: the opponent almost always mulligans.
            var oneBasic = new List<CardDef> { C(Dreepy) };
            oneBasic.AddRange(Enumerable.Repeat(C(Psychic), 59));
            for (int seed = 1; seed <= 200; seed++)
            {
                bool drew = false;
                Person person = null;
                person = new Person(r =>
                {
                    if (r.Kind == DecisionKind.MulliganDraws) drew = true;
                    // Done straight away at the first Bench step, bench everything at the second.
                    if (r.Kind == DecisionKind.SetupBench) return new List<int> { drew ? 0 : r.Options.Count - 1 };
                    return null;
                });
                var e = new GameEngine(StandardCards.CreateRegistry(), person, new ScriptedAgent());
                e.Setup(basics, oneBasic, seed);
                if (e.State.Players[1].Mulligans == 0) continue;
                var kinds = person.Seen.Select(r => r.Kind).ToList();
                int drawAt = kinds.IndexOf(DecisionKind.MulliganDraws);
                Assert.GreaterOrEqual(drawAt, 0);
                Assert.Greater(kinds.LastIndexOf(DecisionKind.SetupBench), drawAt, "asked again after the extra cards are drawn");
                Assert.Greater(e.State.Players[0].Bench.Count, 0, "benched after the draws");
                Assert.AreEqual(0, e.State.TurnNumber, "before the game starts");
                return;
            }
            Assert.Fail("No seed produced a mulligan.");
        }

        // ================================================================== Card effects

        [Test]
        public void Superior_Energy_Retrieval_Needs_Basic_Energy_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(SuperiorEnergyRetrieval));
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Superior Energy Retrieval"), "no basic Energy in the discard pile");
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            Assert.IsTrue(Offered(e, 0, "Play Superior Energy Retrieval"));
        }

        [Test]
        public void Hisuian_Heavy_Ball_Shows_The_Prize_Cards_Even_With_No_Basic_Pokemon_There()
        {
            var person = new Person(null).Then("Play Hisuian Heavy Ball");
            var e = PersonScenario(person);
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(HisuianHeavyBall));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            var shown = person.Seen.Where(r => r.Prompt != null && r.Prompt.StartsWith("Hisuian Heavy Ball")).ToList();
            Assert.AreEqual(1, shown.Count, "the Prize cards are shown");
            Assert.AreEqual(6, shown[0].SeenCards.Count);
            Assert.AreEqual(0, shown[0].MaxChoices, "nothing to take, just Done");
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Hisuian Heavy Ball"));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void PokeStop_Shows_The_Cards_It_Discards()
        {
            var person = new Person(null).Then("Use PokéStop");
            var e = PersonScenario(person);
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].Deck.Insert(1, e.NewCard(0, C(SuperiorEnergyRetrieval)));
            e.ScenarioSetStadium(1, C(PokeStop));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            var shown = person.Seen.Single(r => r.SeenTitle == "Discarded by PokéStop");
            Assert.AreEqual(3, shown.SeenCards.Count);
            StringAssert.Contains("The Item card goes into your hand", shown.Prompt);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Superior Energy Retrieval"));
        }

        // ================================================================== Card data

        [Test]
        public void The_30th_Celebration_Arceus_VSTAR_Is_An_Alternate_Art_Of_Brilliant_Stars_Arceus_VSTAR()
        {
            var cc = C("30C-CC29");
            Assert.IsTrue(cc.IsAlternatePrint);
            Assert.AreEqual("CRZ-GG70", db.MainPrinting(cc).Key, "the same card as Brilliant Stars 123 and Crown Zenith GG70");
            Assert.IsTrue(cc.Abilities[0].VstarPower, "Starbirth is a VSTAR Power");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(cc));
            Assert.IsTrue(db.Cards.Count(d => d.Name == "Arceus VSTAR") == 1, "one card in the deck builder's pool");
        }
    }
}
