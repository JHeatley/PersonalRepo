using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: Rising Rivals (RR): the card data and prints, and the new effects (Water Rescue, Unlucky Wind, Sand Reset, SP Energy,
    /// Upper Energy, Colorful Body, Submerge, Territoriality, Charon's Choice).
    /// </summary>
    public class RrTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void Rising_Rivals_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(112, db.Cards.Count(d => d.SetCode == "RR"));
            Assert.AreEqual("CEL-CC17", db.MainPrinting(C("RR-109")).Key);
            Assert.AreEqual("N1-104", db.MainPrinting(C("RR-99")).Key, "Darkness Energy");
        }

        [Test]
        public void Every_Rising_Rivals_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "RR" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Water_Rescue_Returns_A_Knocked_Out_Water_Pokemon_With_Its_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes"), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var magikarp = e.ScenarioActive(0, C("MT-89"));
            Attach(e, magikarp, Water, 1);
            magikarp.DamageCounters = e.MaxHp(magikarp) / 10 - 1;
            var floatzel = e.ScenarioBench(0, C("RR-4"));
            e.ScenarioEvolve(floatzel, C("RR-104"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Magikarp"));
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Water Energy"), "the attached Energy too");
        }

        [Test]
        public void Unlucky_Wind_Makes_The_Opponents_Flips_Tails()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("RR-13"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(1, 4);
            for (int i = 0; i < 10; i++) Assert.IsFalse(e.FlipCoin(1, "test"));
        }

        [Test]
        public void Sand_Reset_Shuffles_Energy_Into_The_Decks()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Sand Reset"), new ScriptedAgent());
            var hippowdon = e.ScenarioActive(0, C("RR-25"));
            e.ScenarioEvolve(hippowdon, C("RR-107"));
            Attach(e, hippowdon, "MEE-14", 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            Attach(e, chansey, Psychic, 3);
            e.ScenarioSetTurn(0, 3);
            int mine = e.State.Players[0].Deck.Count, theirs = e.State.Players[1].Deck.Count;
            Step(e);
            Assert.AreEqual(0, hippowdon.Energy.Count);
            Assert.AreEqual(0, chansey.Energy.Count);
            Assert.AreEqual(mine + 2, e.State.Players[0].Deck.Count);
            Assert.AreEqual(theirs + 3, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Sp_Energy_Provides_Every_Type_Only_For_Pokemon_SP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var weavile = e.ScenarioActive(0, C("PL-17"));
            Attach(e, weavile, "RR-101", 3);
            var chansey = e.ScenarioBench(0, C("PL-29"));
            Attach(e, chansey, "RR-101", 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.CanPayAttack(weavile, weavile.Def.Attacks.First(a => a.Name == "Team Attack")), "Darkness from SP Energy");
            Assert.IsFalse(e.CanPayAttack(chansey, chansey.Def.Attacks.First(a => a.Name == "Water Slide")), "only Colorless on Golduck");
        }

        [Test]
        public void Upper_Energy_Provides_Two_When_Behind()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, "RR-102", 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.EnergyUnits(chansey).Count);
            e.State.Players[1].Prizes.RemoveAt(0);
            Assert.AreEqual(2, e.EnergyUnits(chansey).Count);
        }

        [Test]
        public void Colorful_Body_Makes_Kecleon_Every_Type()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C("RR-67"));
            e.ScenarioActive(1, C(Chansey));
            var types = e.TypesOf(kecleon);
            Assert.IsTrue(types.Contains(EnergyType.Fire) && types.Contains(EnergyType.Water) && types.Contains(EnergyType.Metal));
        }

        [Test]
        public void Submerge_Protects_Quagsire_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot"));
            e.ScenarioActive(0, C(Chansey));
            var quagsire = e.ScenarioBench(0, C("RR-76"));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, Water, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, quagsire.DamageCounters);
        }

        [Test]
        public void Territoriality_Hurts_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("RR-29"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, mew.DamageCounters);
        }

        [Test]
        public void Charons_Choice_Switches_A_Rotom_And_Comes_Back()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Charon's Choice"), new ScriptedAgent());
            var rotom = e.ScenarioActive(0, C("RR-RT1"));
            Attach(e, rotom, Psychic, 1);
            rotom.DamageCounters = 2;
            e.ScenarioAddToDeck(0, C("RR-RT3"), 1);
            e.ScenarioAddToHand(0, C("RR-RT6"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Heat Rotom", rotom.Def.Name);
            Assert.AreEqual(2, rotom.DamageCounters);
            Assert.AreEqual(1, rotom.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Charon's Choice"));
            Assert.AreEqual(1, e.State.Players[0].Deck.Count(c => c.Def.Name == "Fan Rotom"));
        }
    }
}
