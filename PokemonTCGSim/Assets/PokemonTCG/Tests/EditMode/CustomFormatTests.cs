using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v123: custom formats: the format file, deck rules (deck size, singleton, legal sets, bans, Rule Box Pokémon, ACE SPECs),
    /// and the game rules (first-turn attacks and Supporters, hands before the coin flip, Tools as Items, old Trainers as plain
    /// Trainers: your rulings).
    /// </summary>
    public class CustomFormatTests : ScenarioTestBase
    {
        const string Rattata = "G1-82";
        const string Spearow = "G1-52";
        const string Lightning = "MEE-12";
        const string Youngster = "SVI-198";
        const string ChoiceBelt = "PAL-176";
        const string BlainesLastResort = "G1-105";
        const string HereComesTeamRocket = "CEL-CC6";
        const string PewterCityGym = "G1-115";

        [TearDown]
        public void ClearFormats()
        {
            CustomFormats.Set(null);
        }

        private List<CardDef> SmallDeck(int size, params string[] extra)
        {
            var deck = new List<CardDef> { C(Rattata) };
            foreach (var k in extra) deck.Add(C(k));
            while (deck.Count < size) deck.Add(C(Lightning));
            return deck;
        }

        [Test]
        public void The_Format_File_Round_Trips()
        {
            var f = new CustomFormat { Name = "Gym Challenge", DeckSize = 40, PrizeCount = 4, AllSets = false, Singleton = true, AllowRuleBox = false, AllowAceSpec = false };
            f.Sets.Add("BS");
            f.Sets.Add("G1");
            f.Banned.Add("BS-92");
            f.Rules.FirstTurnAttack = true;
            f.Rules.ToolsAreItems = true;
            f.Rules.OldTrainersArePlain = true;
            var back = CustomFormat.Parse(f.ToText());
            Assert.IsNotNull(back);
            Assert.AreEqual("Gym Challenge", back.Name);
            Assert.AreEqual(40, back.DeckSize);
            Assert.AreEqual(4, back.PrizeCount);
            Assert.IsFalse(back.AllSets);
            CollectionAssert.AreEquivalent(new[] { "BS", "G1" }, back.Sets.ToArray());
            CollectionAssert.AreEquivalent(new[] { "BS-92" }, back.Banned.ToArray());
            Assert.IsTrue(back.Singleton);
            Assert.IsFalse(back.AllowRuleBox);
            Assert.IsFalse(back.AllowAceSpec);
            Assert.IsTrue(back.Rules.FirstTurnAttack);
            Assert.IsFalse(back.Rules.FirstTurnSupporter);
            Assert.IsFalse(back.Rules.DrawBeforeCoinFlip);
            Assert.IsTrue(back.Rules.ToolsAreItems);
            Assert.IsTrue(back.Rules.OldTrainersArePlain);
            Assert.AreEqual("first-turn-attack, tools-are-items, plain-old-trainers", back.Rules.ToString());
            Assert.IsTrue(GameRules.Parse(back.Rules.ToString()).OldTrainersArePlain);
        }

        [Test]
        public void Names_Can_Not_Be_Built_In_Or_Used_Twice()
        {
            var a = new CustomFormat { Name = "Mine" };
            Assert.IsNotNull(CustomFormat.NameProblem("Standard", null, null));
            Assert.IsNotNull(CustomFormat.NameProblem("  ", null, null));
            Assert.IsNotNull(CustomFormat.NameProblem("mine", new[] { a }, null));
            Assert.IsNull(CustomFormat.NameProblem("mine", new[] { a }, a), "renaming itself is fine");
            Assert.IsNull(CustomFormat.NameProblem("Other", new[] { a }, null));
        }

        [Test]
        public void Deck_Size_And_Singleton()
        {
            var f = new CustomFormat { Name = "Tiny", DeckSize = 20, PrizeCount = 3, Singleton = true };
            Assert.AreEqual(0, f.Validate(db, SmallDeck(20)).Count, string.Join(" ", f.Validate(db, SmallDeck(20)).ToArray()));
            Assert.IsTrue(f.Validate(db, SmallDeck(21)).Any(p => p.Contains("needs exactly 20")));
            Assert.IsTrue(f.Validate(db, SmallDeck(20, Rattata)).Any(p => p.Contains("singleton")), "2 copies");
            f.Singleton = false;
            Assert.AreEqual(0, f.Validate(db, SmallDeck(20, Rattata)).Count);
        }

        [Test]
        public void Sets_Bans_Rule_Box_And_Ace_Spec()
        {
            var f = new CustomFormat { Name = "Limited", DeckSize = 20, PrizeCount = 3 };
            Assert.AreEqual(0, f.Validate(db, SmallDeck(20)).Count);

            f.AllSets = false;
            f.Sets.Add("SVI");
            f.Invalidate();
            Assert.IsFalse(f.Allows(db, C(Rattata)), "a Gym Heroes card");
            Assert.IsTrue(f.Allows(db, C(Lightning)), "basic Energy is always legal");
            f.Sets.Add("G1");
            f.Invalidate();
            Assert.IsTrue(f.Allows(db, C(Rattata)));

            f.Banned.Add(Rattata);
            Assert.IsFalse(f.Allows(db, C(Rattata)));
            Assert.IsTrue(f.Validate(db, SmallDeck(20)).Any(p => p.Contains("banned")));

            var ruleBox = db.Cards.First(d => d.IsPokemon && d.HasRuleBox);
            var aceSpec = db.Cards.First(d => d.IsAceSpec);
            var open = new CustomFormat { Name = "Open" };
            Assert.IsTrue(open.Allows(db, ruleBox));
            Assert.IsTrue(open.Allows(db, aceSpec));
            open.AllowRuleBox = false;
            open.AllowAceSpec = false;
            Assert.IsFalse(open.Allows(db, ruleBox));
            Assert.IsFalse(open.Allows(db, aceSpec));
        }

        [Test]
        public void Decks_Name_Their_Custom_Format()
        {
            var f = new CustomFormat { Name = "Gym Challenge", DeckSize = 20, PrizeCount = 3 };
            CustomFormats.Set(new[] { f });
            CustomFormats.Database = db;
            var text = Decklist.Format("Test", Decklist.Group(SmallDeck(20)), "Gym Challenge");
            Assert.AreEqual("Gym Challenge", Decklist.ReadFormatName(text));
            var format = GameFormat.Named(Decklist.ReadFormatName(text));
            Assert.IsNotNull(format);
            Assert.IsTrue(format.IsCustom);
            Assert.AreEqual(3, format.PrizeCount);
            Assert.AreEqual(0, format.Validate(Decklist.Expand(Decklist.Parse(text, db, new List<string>()))).Count);
            Assert.AreSame(GameFormat.Expanded, GameFormat.Named("expanded"));
            Assert.AreEqual("Standard", Decklist.ReadFormatName(Decklist.Format("Plain", Decklist.Group(SmallDeck(20)), "Standard")));
            Assert.IsNull(GameFormat.Named("Deleted Format"));
        }

        [Test]
        public void The_First_Player_Can_Attack_On_Turn_1_When_The_Format_Allows_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            e.ScenarioAttach(spearow, C(Lightning));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 1);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Attack: Drill Peck"));
            e.Rules = new GameRules { FirstTurnAttack = true };
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Attack: Drill Peck"));
        }

        [Test]
        public void The_First_Player_Can_Play_A_Supporter_On_Turn_1_When_The_Format_Allows_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Youngster));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 1);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Youngster"));
            e.Rules = new GameRules { FirstTurnSupporter = true };
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play Youngster"));
        }

        [Test]
        public void Tools_Are_Items_Only_When_The_Format_Says_So()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(ChoiceBelt));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].ItemLockTurn = 3;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play Choice Belt"), "Item lock doesn't stop Tools normally");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.Rules = new GameRules { ToolsAreItems = true };
            e2.ScenarioActive(0, C(Rattata));
            var belt = e2.ScenarioAddToHand(0, C(ChoiceBelt));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsTrue(belt.Def.IsItem && belt.Def.IsTool);
            Assert.IsTrue(e2.GetMainActions(0).Any(o => o.Label == "Play Choice Belt"));
            e2.State.Players[0].ItemLockTurn = 3;
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label == "Play Choice Belt"), "now Item lock stops it");
            Assert.IsFalse(C(ChoiceBelt).IsItem, "the card pool's card doesn't change");
        }

        [Test]
        public void Old_Trainers_Are_Plain_Trainers_Only_When_The_Format_Says_So()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(BlainesLastResort));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].ItemLockTurn = 3;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Blaine's Last Resort"), "an Item normally");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.Rules = new GameRules { OldTrainersArePlain = true };
            e2.ScenarioActive(0, C(Rattata));
            var card = e2.ScenarioAddToHand(0, C(BlainesLastResort));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            e2.State.Players[0].ItemLockTurn = 3;
            Assert.IsFalse(card.Def.IsItem);
            Assert.AreEqual("Trainer", card.Def.TrainerLabel);
            Assert.IsTrue(e2.GetMainActions(0).Any(o => o.Label == "Play Blaine's Last Resort"), "not an Item, so Item lock doesn't stop it (your ruling)");

            var rules = new GameRules { OldTrainersArePlain = true };
            var hctr = rules.Adapt(C(HereComesTeamRocket));
            Assert.IsFalse(hctr.IsSupporter, "no Supporter limit (your ruling)");
            Assert.IsFalse(hctr.IsItem);
            var gym = rules.Adapt(C(PewterCityGym));
            Assert.IsTrue(gym.IsStadium, "old Gyms still work as Stadiums (your ruling)");
            Assert.AreEqual("Trainer", gym.TrainerLabel);
            Assert.AreSame(C(Youngster), rules.Adapt(C(Youngster)), "newer Trainers don't change");
            Assert.AreSame(gym, rules.Adapt(C(PewterCityGym)), "one shared copy per card");
        }

        [Test]
        public void Hands_Can_Be_Drawn_Before_The_Coin_Flip()
        {
            foreach (bool first in new[] { false, true })
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent());
                e.Rules = new GameRules { DrawBeforeCoinFlip = first };
                var log = new Order();
                e.AddObserver(log);
                var deck = new List<CardDef>();
                for (int i = 0; i < 20; i++) deck.Add(C(Rattata));
                while (deck.Count < 60) deck.Add(C(Lightning));
                e.Setup(deck, deck, 7);
                int flip = log.Types.IndexOf(GameEventType.CoinFlip);
                int draw = log.Types.IndexOf(GameEventType.Draw);
                Assert.GreaterOrEqual(flip, 0);
                Assert.GreaterOrEqual(draw, 0);
                Assert.AreEqual(first, draw < flip, first ? "hands first" : "coin flip first");
            }
        }

        private sealed class Order : IGameObserver
        {
            public readonly List<GameEventType> Types = new List<GameEventType>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Types.Add(ev.Type); }
        }
    }
}
