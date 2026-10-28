using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v165: Deoxys (DX): the card data and prints, Space Center, Form Change, Meteor Falls, Empty Shell, Lazy Aura, Swords Dance,
    /// Commanding Aura and Scramble Energy.
    /// </summary>
    public class DxTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";

        const string Gyarados = "DX-8";
        const string Ninjask = "DX-13";
        const string Shedinja = "DX-14";
        const string Slaking = "DX-15";
        const string DeoxysLink = "DX-16";
        const string DeoxysBarrier = "DX-18";
        const string Corphish = "DX-57";
        const string Magikarp = "DX-64";
        const string MeteorFalls = "DX-89";
        const string SpaceCenter = "DX-91";
        const string ScrambleEnergy = "DX-95";
        const string HariyamaEx = "DX-100";
        const string SharpedoEx = "DX-104";
        const string Kangaskhan = "RG-6";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Deoxys_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(102, db.Cards.Count(d => d.SetCode == "DX"), "108 cards minus 6 prints");
            Assert.AreEqual(108, db.All.Count(d => d.SetCode == "DX"));
            Assert.AreEqual("N3-60", db.MainPrinting(db.Get("DX-84")).Key, "your ruling: Balloon Berry is a print");
            Assert.AreEqual("E3-122", db.MainPrinting(db.Get("DX-85")).Key);
            Assert.AreEqual("DX-86", db.MainPrinting(db.Get("DX-86")).Key, "your ruling: Energy Charge is its own card");
            Assert.AreEqual("RS-83", db.MainPrinting(db.Get("DX-87")).Key);
            Assert.AreEqual("G2-116", db.MainPrinting(db.Get("DX-88")).Key);
            Assert.AreEqual("E1-150", db.MainPrinting(db.Get("DX-92")).Key);
            Assert.AreEqual("E2-145", db.MainPrinting(db.Get("DX-93")).Key);
        }

        [Test]
        public void Space_Center_Ignores_Basic_Pokemon_s_Poke_Bodies()
        {
            foreach (bool stadium in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Headbutt"), new ScriptedAgent());
                var kangaskhan = e.ScenarioActive(0, C(Kangaskhan));
                Attach(e, kangaskhan, Fire, 2);
                var corphish = e.ScenarioActive(1, C(Corphish));
                if (stadium) e.ScenarioSetStadium(1, C(SpaceCenter));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(stadium ? 2 : 1, corphish.DamageCounters, "Exoskeleton: 10 less unless ignored");
            }
        }

        [Test]
        public void Form_Change_Swaps_Deoxys_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Form Change"), new ScriptedAgent());
            var deoxys = e.ScenarioActive(0, C(DeoxysLink));
            deoxys.DamageCounters = 2;
            e.ScenarioBench(0, C(DeoxysLink));
            e.ScenarioAddToDeck(0, C(DeoxysBarrier), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("DX-18", deoxys.Top.Def.Key);
            Assert.AreEqual(2, deoxys.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Deck.Count(c => c.Def.Key == "DX-16"));
            Assert.IsFalse(Offered(e, 0, "Use Form Change"), "only 1 Form Change each turn");
        }

        [Test]
        public void Meteor_Falls_Lends_The_Attacks_Under_An_Evolved_Pokemon()
        {
            foreach (bool stadium in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var gyarados = e.ScenarioActive(0, C(Magikarp));
                e.ScenarioEvolve(gyarados, C(Gyarados));
                Attach(e, gyarados, Water, 1);
                e.ScenarioActive(1, C(Chansey));
                if (stadium) e.ScenarioSetStadium(0, C(MeteorFalls));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(stadium, Offered(e, 0, "Attack: Leap Out"));
            }
        }

        [Test]
        public void Empty_Shell_Gives_No_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var shedinja = e.ScenarioActive(0, C(Shedinja));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.PrizesFor(shedinja));
        }

        [Test]
        public void Lazy_Aura_Takes_30_Off_Pokemon_ex_Attacks()
        {
            foreach (bool slaking in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Drag Off"), new ScriptedAgent());
                var sharpedo = e.ScenarioActive(0, C(SharpedoEx));
                Attach(e, sharpedo, Psychic, 2);
                var target = e.ScenarioActive(1, C(slaking ? Slaking : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(slaking ? 0 : 2, target.DamageCounters);
            }
        }

        [Test]
        public void Swords_Dance_Makes_Slash_Do_80()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Swords Dance", "Attack: Slash"), new ScriptedAgent());
            var ninjask = e.ScenarioActive(0, C(Ninjask));
            Attach(e, ninjask, Fire, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(0, 5);
            Step(e);
            Assert.AreEqual(8, chansey.DamageCounters);
        }

        [Test]
        public void Commanding_Aura_Stops_The_Opponent_s_Stadiums()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(HariyamaEx));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(SpaceCenter));
            e.ScenarioAddToHand(0, C(SpaceCenter));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Play Space Center"));
            e.ScenarioSetTurn(0, 5);
            Assert.IsTrue(Offered(e, 0, "Play Space Center"));
        }

        [Test]
        public void Scramble_Energy_Goes_Only_On_Evolved_Pokemon_That_Aren_t_ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gyarados = e.ScenarioActive(0, C(Magikarp));
            e.ScenarioEvolve(gyarados, C(Gyarados));
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ScrambleEnergy));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attach Scramble Energy to Gyarados"));
            Assert.IsFalse(Offered(e, 0, "Attach Scramble Energy to Chansey"));
        }
    }
}
