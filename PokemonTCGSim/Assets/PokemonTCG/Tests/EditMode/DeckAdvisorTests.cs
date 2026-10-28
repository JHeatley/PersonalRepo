using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;
using UnityEngine;

namespace PokemonTCG.Tests
{
    /// <summary>v75: the deck advisor (card roles from text, suggestions, whole decks, counters, the before/after test).</summary>
    public class DeckAdvisorTests : ScenarioTestBase
    {
        private const string Water = "MEE-11";

        /// <summary>Every legal built-in deck list, the way the deck builder hands the library to the advisor.</summary>
        private List<AdvisorDeck> Library()
        {
            var list = new List<AdvisorDeck>();
            foreach (var asset in Resources.LoadAll<TextAsset>("Decklists").OrderBy(a => a.name))
            {
                var errors = new List<string>();
                var cards = Decklist.Expand(Decklist.Parse(asset.text, db, errors));
                if (cards.Count == 60 && Decklist.Validate(cards, Decklist.ReadFormat(asset.text)).Count == 0)
                    list.Add(new AdvisorDeck(asset.name, cards));
            }
            return list;
        }

        private List<CardDef> Dragapult() { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); }

        private static List<CardDef> Swap(List<CardDef> deck, string name, int copies, CardDef replacement)
        {
            var list = new List<CardDef>(deck);
            for (int i = 0; i < copies; i++)
            {
                var found = list.First(c => c.Name == name);
                list.Remove(found);
                list.Add(replacement);
            }
            return list;
        }

        private DeckAdvisor Advisor(LearningBrain brain = null)
        {
            return new DeckAdvisor(db, DeckFormat.Standard, Library(), brain);
        }

        private static void AssertLegal(List<CardDef> deck)
        {
            Assert.AreEqual(60, deck.Count);
            var problems = Decklist.Validate(deck, DeckFormat.Standard);
            Assert.AreEqual(0, problems.Count, string.Join("\n", problems.ToArray()));
        }

        // ------------------------------------------------------------------ Roles from card text

        [TestCase("MEG-114", CardRole.Gust)]
        [TestCase("PAR-160", CardRole.Gust)]
        [TestCase("MEG-131", CardRole.PokemonSearch)]
        [TestCase("PAF-84", CardRole.PokemonSearch)]
        [TestCase("TEF-144", CardRole.PokemonSearch)]
        [TestCase("MEG-119", CardRole.Draw)]
        [TestCase("ASC-196", CardRole.Recovery)]
        [TestCase("MEG-130", CardRole.Switch)]
        [TestCase("ASC-181", CardRole.Switch)]
        [TestCase("MEG-125", CardRole.RareCandy)]
        [TestCase("SVI-186", CardRole.TrainerSearch)]
        [TestCase("POR-71", CardRole.EnergyRemoval)]
        [TestCase("ASC-212", CardRole.ToolRemoval)]
        [TestCase("ASR-213", CardRole.AbilityLock)]
        [TestCase("PRE-106", CardRole.EnergySearch)]
        [TestCase("PAF-221", CardRole.Tutor)]
        [TestCase("ASC-142", CardRole.Draw)]
        public void Roles_Are_Read_From_The_Card_Text(string key, CardRole role)
        {
            Assert.IsTrue(CardRoles.Has(C(key), role), C(key).Name + ": " + CardRoles.Of(C(key)));
        }

        [Test]
        public void Roles_Tell_Draw_For_You_From_Draw_For_Them()
        {
            var iono = CardRoles.Of(C("SVP-124"));
            Assert.IsTrue((iono & CardRole.Draw) != 0 && (iono & CardRole.Disruption) != 0, "Iono: both players draw, and it disrupts");
            var redCard = CardRoles.Of(C("CRI-82"));
            Assert.IsTrue((redCard & CardRole.Draw) == 0, "Special Red Card only makes your opponent draw");
            Assert.IsTrue((redCard & CardRole.Disruption) != 0);
            Assert.IsTrue(CardRoles.Has(C("TWM-165"), CardRole.Draw), "Unfair Stamp: you draw 5, they draw 2");
            Assert.IsTrue((CardRoles.OfAttacks(C("ASC-16")) & CardRole.ItemLock) != 0, "Budew's Itchy Pollen");
            Assert.AreEqual(CardRole.None, CardRoles.Of(C("ASC-16")), "Budew has no Ability");
            Assert.IsFalse(CardRoles.Has(C("TWM-163"), CardRole.ToolRemoval), "Secret Box finds a Tool, it doesn't discard one");
        }

        [Test]
        public void Mulligan_Chance_Is_The_Chance_Of_No_Basic_In_Seven()
        {
            Assert.AreEqual(1.0, DeckAdvisor.MulliganChance(0, 60), 1e-9);
            Assert.AreEqual(0.25863, DeckAdvisor.MulliganChance(10, 60), 1e-4);
            Assert.AreEqual(0.19065, DeckAdvisor.MulliganChance(12, 60), 1e-4);
        }

        // ------------------------------------------------------------------ The brain's opinions

        [Test]
        public void The_Brain_Says_What_It_Thinks_Of_Each_Card()
        {
            Assert.AreEqual("Ultra Ball", LearningBrain.CardOfFeature("M:Trainer:Ultra Ball"));
            Assert.AreEqual("Dragapult ex", LearningBrain.CardOfFeature("M:Attack:Dragapult ex/Phantom Dive"));
            Assert.AreEqual("Fezandipiti ex", LearningBrain.CardOfFeature("M:Ability:Fezandipiti ex/Flip the Script"));
            Assert.IsNull(LearningBrain.CardOfFeature("M:Trainer:Ultra Ball|early"), "situational features aren't about the card itself");
            Assert.IsNull(LearningBrain.CardOfFeature("M:End|atk1"));
            Assert.IsNull(LearningBrain.CardOfFeature("SA:Dreepy"));

            var brain = new LearningBrain();
            brain.LoadText("w\t-2.5\t300\tM:Trainer:Crushing Hammer\nw\t1\t100\tM:Attack:Dragapult ex/Phantom Dive\nw\t3\t100\tD:Other|M:Trainer:Crushing Hammer\n");
            var mine = brain.CardOpinions("Mine", null);
            Assert.AreEqual(-2.5, mine["Crushing Hammer"].Value, 1e-9, "another deck's lessons don't count");
            Assert.AreEqual(300, mine["Crushing Hammer"].Updates);
            Assert.AreEqual(1.0, mine["Dragapult ex"].Value, 1e-9);
            var other = brain.CardOpinions("Other", null);
            Assert.AreEqual((-2.5 * 200 + 3.0 * 100 * 1.5) / (200 + 150), other["Crushing Hammer"].Value, 1e-9, "this deck's own lessons count a little more");
        }

        // ------------------------------------------------------------------ Changes to a deck

        [Test]
        public void A_Deck_Without_Gust_Gets_Boss_Back_For_Energy_It_Cannot_Use()
        {
            var deck = Swap(Dragapult(), "Boss's Orders", 3, C(Water));
            var advice = Advisor().SuggestChanges("Test", deck, 6);
            Assert.IsNull(advice.Error);
            Assert.IsTrue(advice.Changes.Any(c => c.Add != null && c.Add.Name == "Boss's Orders" && c.Cut != null && c.Cut.Name == "Water Energy"),
                string.Join("\n", advice.Changes.Select(c => c.Label + " | " + c.Why + " | " + c.CutWhy).ToArray()));
            Assert.IsTrue(advice.Changes.Count <= 6);
            AssertLegal(advice.Result);
            Assert.Less(advice.Result.Count(c => c.Name == "Water Energy"), 3);
        }

        [Test]
        public void An_Evolution_With_Nothing_To_Evolve_From_Gets_Its_Basic()
        {
            var deck = Swap(Dragapult(), "Dreepy", 4, C(Water));
            var advice = Advisor().SuggestChanges("Test", deck, 6);
            var change = advice.Changes.FirstOrDefault(c => c.Add != null && c.Add.Name == "Dreepy");
            Assert.IsNotNull(change, string.Join("\n", advice.Changes.Select(c => c.Label + " | " + c.Why).ToArray()));
            StringAssert.Contains("evolves from Dreepy", change.Why);
            Assert.GreaterOrEqual(advice.Result.Count(c => c.Name == "Dreepy"), 3);
            AssertLegal(advice.Result);
        }

        [Test]
        public void A_Short_Deck_Is_Filled_And_A_Long_One_Trimmed()
        {
            var advisor = Advisor();
            var shortDeck = Dragapult().Take(45).ToList();
            var filled = advisor.SuggestChanges("Short", shortDeck, 6);
            AssertLegal(filled.Result);
            Assert.IsTrue(filled.Changes.Any(c => c.Add != null && c.Cut == null), "cards are added to a short deck");

            var longDeck = Dragapult();
            longDeck.Add(C(Water));
            longDeck.Add(C(Water));
            var trimmed = advisor.SuggestChanges("Long", longDeck, 6);
            AssertLegal(trimmed.Result);
            Assert.AreEqual(0, trimmed.Result.Count(c => c.Name == "Water Energy"), "the Energy nothing uses goes first");
        }

        [Test]
        public void A_Card_That_Is_Not_Legal_Is_Always_Cut()
        {
            var axe = C("UNB-165");
            Assert.IsFalse(Formats.Allows(DeckFormat.Standard, axe));
            var deck = Swap(Dragapult(), "Crushing Hammer", 1, axe);
            var advice = Advisor().SuggestChanges("Test", deck, 6);
            Assert.IsFalse(advice.Result.Contains(axe));
            var change = advice.Changes.First(c => c.Cut != null && c.Cut.Key == axe.Key);
            StringAssert.Contains("not legal", change.CutWhy);
            AssertLegal(advice.Result);
        }

        [Test]
        public void The_Brains_Opinion_Shapes_What_Is_Cut()
        {
            var brain = new LearningBrain();
            brain.LoadText("w\t-2.5\t300\tM:Trainer:Crushing Hammer\n");
            var advice = Advisor(brain).SuggestChanges("Test", Dragapult(), 6);
            var change = advice.Changes.FirstOrDefault(c => c.Cut != null && c.Cut.Name == "Crushing Hammer");
            Assert.IsNotNull(change, string.Join("\n", advice.Changes.Select(c => c.Label + " | " + c.CutWhy).ToArray()));
            StringAssert.Contains("AI's games", change.CutWhy);
            Assert.IsTrue(advice.Notes.Any(n => n.Contains("Crushing Hammer less so")), string.Join("\n", advice.Notes.ToArray()));
        }

        [Test]
        public void Applying_Every_Change_Gives_The_Result()
        {
            var deck = Swap(Dragapult(), "Boss's Orders", 3, C(Water));
            var advice = Advisor().SuggestChanges("Test", deck, 6);
            var applied = new List<CardDef>(deck);
            foreach (var c in advice.Changes)
            {
                for (int i = 0; i < c.Count; i++)
                {
                    if (c.Cut != null) applied.Remove(applied.First(x => DeckAdvisor.Id(x) == DeckAdvisor.Id(c.Cut)));
                    if (c.Add != null) applied.Add(c.Add);
                }
            }
            CollectionAssert.AreEquivalent(advice.Result.Select(x => DeckAdvisor.Id(x)).ToList(), applied.Select(x => DeckAdvisor.Id(x)).ToList());
        }

        // ------------------------------------------------------------------ Whole decks

        [Test]
        public void A_Deck_Around_A_Card_The_Library_Plays_Starts_From_Those_Decks()
        {
            var advice = Advisor().BuildAround(C("TWM-130"));
            Assert.IsNull(advice.Error);
            Assert.IsTrue(advice.IsWholeDeck);
            AssertLegal(advice.Result);
            foreach (var name in new[] { "Dragapult ex", "Drakloak", "Dreepy" })
                Assert.IsTrue(advice.Result.Any(c => c.Name == name), name);
            Assert.IsTrue(advice.Notes.Any(n => n.Contains("decks in your library") || n.Contains("Starts from")), string.Join("\n", advice.Notes.ToArray()));
        }

        [Test]
        public void A_Deck_Around_A_New_Card_Is_Built_From_The_Card_Data()
        {
            var venusaur = C("MEG-3");
            Assert.AreEqual("Mega Venusaur ex", venusaur.Name);
            var advice = Advisor().BuildAround(venusaur);
            Assert.IsNull(advice.Error, advice.Error);
            AssertLegal(advice.Result);
            foreach (var name in new[] { "Mega Venusaur ex", "Ivysaur", "Bulbasaur", "Rare Candy", "Grass Energy" })
                Assert.IsTrue(advice.Result.Any(c => c.Name == name), name);
            Assert.GreaterOrEqual(advice.Result.Count(c => c.IsBasicPokemon), 7);
        }

        [Test]
        public void Building_From_A_Basic_Climbs_To_Its_Evolution()
        {
            var advice = Advisor().BuildAround(C("MEG-1"));
            Assert.IsNull(advice.Error, advice.Error);
            AssertLegal(advice.Result);
            Assert.IsTrue(advice.Result.Any(c => c.Stage == Stage.Stage2 || c.Stage == Stage.Stage1), "a Basic with evolutions is built with its line");
        }

        // ------------------------------------------------------------------ Counters

        [Test]
        public void Counter_Advice_Reads_The_Opponents_Deck()
        {
            var zoroark = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            var advice = Advisor().Counter("Dragapult", Dragapult(), "N's Zoroark", zoroark, 5);
            Assert.IsNull(advice.Error);
            Assert.IsTrue(advice.Notes.Any(n => n.StartsWith("Their main attacker: N's Zoroark ex")), string.Join("\n", advice.Notes.ToArray()));
            Assert.IsTrue(advice.Notes.Any(n => n.StartsWith("Their deck:")));
            Assert.LessOrEqual(advice.Changes.Count, 5);
            AssertLegal(advice.Result);
        }

        // ------------------------------------------------------------------ Testing a change

        [Test]
        public void The_Before_And_After_Test_Plays_The_Same_Games()
        {
            var deck = Dragapult();
            var zoroark = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            var trial = new DeckTrial(StandardCards.CreateRegistry(), deck, deck, new List<AdvisorDeck> { new AdvisorDeck("Zoroark", zoroark) }, 2, 75);
            trial.RunBlocking();
            var s = trial.Status();
            Assert.IsTrue(s.Finished);
            Assert.AreEqual(0, s.Errors, s.LastError);
            Assert.AreEqual(4, s.Done);
            Assert.AreEqual(2, s.GamesBefore);
            Assert.AreEqual(s.WinsBefore, s.WinsAfter, "the same deck in the same games gets the same results");
            Assert.AreEqual(1, s.Lines.Count);
        }
    }
}
