using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>v175: the AI builds its own deck to play: at random or around a card, in a format, legal, and playable.</summary>
    public class RandomDeckTests : ScenarioTestBase
    {
        private List<AdvisorDeck> library;

        /// <summary>The built-in decks, handed to the builder the way the menu does.</summary>
        private List<AdvisorDeck> Library()
        {
            if (library != null) return library;
            library = new List<AdvisorDeck>();
            foreach (var record in DeckLibrary.LoadAll())
            {
                var problems = new List<string>();
                var cards = DeckLibrary.Build(db, record, problems);
                if (cards.Count == 60 && problems.Count == 0) library.Add(new AdvisorDeck(record.Name, cards));
            }
            return library;
        }

        private static void AssertLegal(BuiltDeck deck, DeckFormat format)
        {
            Assert.IsTrue(deck.Ok, deck.Error);
            Assert.AreEqual(60, deck.Cards.Count);
            var problems = Decklist.Validate(deck.Cards, format);
            Assert.AreEqual(0, problems.Count, string.Join("\n", problems.ToArray()));
        }

        [TestCase(DeckFormat.Standard)]
        [TestCase(DeckFormat.Expanded)]
        [TestCase(DeckFormat.Unlimited)]
        public void A_Random_Deck_Is_Legal_In_Its_Format_And_Plays_Its_Card(DeckFormat format)
        {
            var deck = RandomDeckBuilder.Build(db, format, Library(), null, null, new System.Random(7));
            AssertLegal(deck, format);
            Assert.IsNotNull(deck.Around);
            Assert.IsTrue(deck.Cards.Any(c => c.Name == deck.Around.Name), deck.Around.Name + " is in the deck");
            Assert.IsTrue(Formats.Allows(format, deck.Around));
        }

        [Test]
        public void The_Deck_Text_Loads_Back_As_The_Same_Deck()
        {
            var deck = RandomDeckBuilder.Build(db, DeckFormat.Expanded, Library(), null, null, new System.Random(3));
            AssertLegal(deck, DeckFormat.Expanded);
            Assert.AreEqual(DeckFormat.Expanded, Decklist.ReadFormat(deck.Text));
            Assert.AreEqual(deck.Name, Decklist.ReadName(deck.Text));
            var errors = new List<string>();
            var loaded = Decklist.Expand(Decklist.Parse(deck.Text, db, errors));
            CollectionAssert.IsEmpty(errors);
            CollectionAssert.AreEquivalent(deck.Cards.Select(c => c.Key).ToList(), loaded.Select(c => c.Key).ToList());
        }

        [Test]
        public void The_Same_Seed_Builds_The_Same_Deck()
        {
            var a = RandomDeckBuilder.Build(db, DeckFormat.Standard, Library(), null, null, new System.Random(11));
            var b = RandomDeckBuilder.Build(db, DeckFormat.Standard, Library(), null, null, new System.Random(11));
            Assert.AreEqual(a.Text, b.Text);
        }

        [Test]
        public void A_Deck_Built_Around_A_Named_Card_Plays_It()
        {
            var absol = C("ASK-30");
            var deck = RandomDeckBuilder.Build(db, DeckFormat.Standard, Library(), null, absol, new System.Random(1));
            AssertLegal(deck, DeckFormat.Standard);
            Assert.GreaterOrEqual(deck.Cards.Count(c => c.Name == absol.Name), 2);
        }

        [Test]
        public void A_Card_Not_Legal_In_The_Format_Gives_A_Reason()
        {
            var deck = RandomDeckBuilder.Build(db, DeckFormat.Standard, Library(), null, C("BS-4"), new System.Random(1));
            Assert.IsFalse(deck.Ok);
            StringAssert.Contains("isn't legal", deck.Error);
        }

        [Test]
        public void Candidates_And_Search_Stay_In_The_Format()
        {
            var standard = RandomDeckBuilder.Candidates(db, DeckFormat.Standard);
            Assert.Greater(standard.Count, 20);
            Assert.IsTrue(standard.All(d => d.IsPokemon && Formats.Allows(DeckFormat.Standard, d)));
            Assert.Greater(RandomDeckBuilder.Candidates(db, DeckFormat.Unlimited).Count, standard.Count);
            var found = RandomDeckBuilder.Search(db, DeckFormat.Unlimited, "charizard", 10);
            Assert.IsNotEmpty(found);
            Assert.IsTrue(found.All(d => d.Name.ToLowerInvariant().Contains("charizard")));
            Assert.IsEmpty(RandomDeckBuilder.Search(db, DeckFormat.Standard, "", 10));
        }

        // Whole games are slow in the Unity Editor, so only a few.
        [Test, Timeout(600000)]
        public void Random_Decks_Play_Whole_Games_Without_Errors()
        {
            var rng = new System.Random(5);
            for (int i = 0; i < 3; i++)
            {
                var a = RandomDeckBuilder.Build(db, DeckFormat.Standard, Library(), null, null, rng);
                var b = RandomDeckBuilder.Build(db, DeckFormat.Standard, Library(), null, null, rng);
                AssertLegal(a, DeckFormat.Standard);
                AssertLegal(b, DeckFormat.Standard);
                var report = MatchRunner.Run(StandardCards.CreateRegistry(), a.Cards, b.Cards, 4, 600 + i, (seat, s) => new HeuristicAgent(s), true);
                TestContext.WriteLine(a.Name + " vs " + b.Name + ": " + report.Summary());
                Assert.AreEqual(0, report.Errors.Count, report.Summary());
            }
        }
    }
}
