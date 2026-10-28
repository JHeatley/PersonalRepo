using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v158: Sandstorm (SS): the card data and prints, 2 Weaknesses, Multi Energy, Baby Evolution, Wally's Training, Primal Veil,
    /// Primal Lock, Team Assembly, the Eclipse Poké-Powers, Energy Variation and Supernatural.
    /// </summary>
    public class SsTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";

        const string Armaldo = "SS-1";
        const string Lunatone = "SS-8";
        const string Sableye = "SS-10";
        const string Solrock = "SS-13";
        const string Kecleon = "SS-18";
        const string Pichu = "SS-20";
        const string Kabuto = "SS-39";
        const string Growlithe = "SS-65";
        const string Omanyte = "SS-70";
        const string Pikachu = "SS-72";
        const string Wallys = "SS-89";
        const string ClawFossil = "SS-90";
        const string MultiEnergy = "SS-93";
        const string AerodactylEx = "SS-94";
        const string AggronEx = "SS-95";
        const string RaichuEx = "SS-98";
        const string OranBerry = "RS-85";
        const string Birch = "RS-89";
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
        public void Sandstorm_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "SS").ToList();
            Assert.AreEqual(97, set.Count, "100 cards minus 3 prints");
            Assert.AreEqual(100, db.All.Count(d => d.SetCode == "SS"));
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual("CES-110", db.MainPrinting(db.Get("SS-60")).Key);
            Assert.AreEqual("MEG-125", db.MainPrinting(db.Get("SS-88")).Key);
            Assert.AreEqual("FO-62", db.MainPrinting(db.Get("SS-91")).Key);
            var aggron = C(AggronEx);
            Assert.IsTrue(aggron.IsOldEx);
            Assert.AreEqual(EnergyType.Fighting, aggron.Weakness);
            Assert.AreEqual(EnergyType.Fire, aggron.SecondWeakness);
            Assert.IsTrue(C(ClawFossil).IsFossil);
            Assert.AreEqual(40, C(ClawFossil).Hp);
            Assert.IsFalse(C(MultiEnergy).IsBasicEnergy);
            Assert.IsTrue(C(Wallys).IsSupporter);
        }

        [Test]
        public void Either_Of_Two_Weaknesses_Doubles_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flare"), new ScriptedAgent());
            var growlithe = e.ScenarioActive(0, C(Growlithe));
            Attach(e, growlithe, Fire, 2);
            var aggron = e.ScenarioActive(1, C(AggronEx));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(EnergyType.Fire, e.SecondWeaknessOf(aggron));
            Step(e);
            Assert.AreEqual(4, aggron.DamageCounters, "20 x 2 for the second Weakness (Fire)");
        }

        [Test]
        public void Multi_Energy_Is_Any_Type_Until_Another_Special_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Chansey));
            Attach(e, mon, MultiEnergy, 1);
            var fire = new List<EnergyType> { EnergyType.Fire };
            Assert.IsTrue(EnergyMask.CanPay(e.EnergyUnits(mon), fire));
            Attach(e, mon, MultiEnergy, 1);
            Assert.IsFalse(EnergyMask.CanPay(e.EnergyUnits(mon), fire), "your ruling: 2 Multi Energy both give Colorless");
            Assert.AreEqual(2, e.EnergyCount(mon));
        }

        [Test]
        public void Baby_Evolution_Puts_Pikachu_On_Pichu_And_Heals_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Baby Evolution"), new ScriptedAgent());
            var pichu = e.ScenarioActive(0, C(Pichu));
            pichu.DamageCounters = 2;
            e.ScenarioAddToHand(0, C(Pikachu));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 1);
            Step(e);
            Assert.AreEqual("Pikachu", pichu.Name, "any turn, even the first");
            Assert.AreEqual(0, pichu.DamageCounters);
        }

        [Test]
        public void Wallys_Training_Evolves_With_No_Timing_Limits_And_Primal_Veil_Stops_Supporters()
        {
            foreach (bool veil in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Wally's Training"), new ScriptedAgent());
                var pikachu = e.ScenarioActive(0, C(Pikachu));
                e.ScenarioAddToHand(0, C(Wallys));
                e.ScenarioAddToDeck(0, C(RaichuEx), 1);
                if (veil) e.ScenarioActive(1, C(Armaldo));
                else
                {
                    e.ScenarioActive(1, C(Chansey));
                    e.ScenarioBench(1, C(Armaldo));
                }
                e.ScenarioSetTurn(0, 2);
                pikachu.TurnEnteredPlay = 2;
                Assert.AreEqual(!veil, Offered(e, 0, "Play Wally's Training"), veil ? "Primal Veil: Armaldo is Active" : "Armaldo on the Bench");
                if (veil) continue;
                Step(e);
                Assert.AreEqual("Raichu ex", pikachu.Name, "your ruling: first turn and put into play this turn are fine");
            }
        }

        [Test]
        public void Team_Assembly_Benches_Omanyte_And_Kabuto_As_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Team Assembly", "Kabuto|Pikachu"), new ScriptedAgent());
            var omanyte = e.ScenarioActive(0, C(Omanyte));
            Attach(e, omanyte, Water, 1);
            e.ScenarioAddToDeck(0, C(Kabuto), 1);
            e.ScenarioAddToDeck(0, C(Pikachu), 1);
            e.ScenarioAddToDeck(0, C(RaichuEx), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var bench = e.State.Players[0].Bench.Select(m => m.Name).ToList();
            Assert.Contains("Kabuto", bench);
            Assert.Contains("Pikachu", bench);
            Assert.IsFalse(bench.Contains("Raichu ex"), "an Evolution card other than Omanyte or Kabuto can't come");
        }

        [Test]
        public void Lunar_Eclipse_Makes_Lunatone_Darkness_Only_With_Solrock_In_Play()
        {
            foreach (bool solrock in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Lunar Eclipse"), new ScriptedAgent());
                var lunatone = e.ScenarioActive(0, C(Lunatone));
                e.ScenarioActive(1, C(Chansey));
                if (solrock) e.ScenarioBench(1, C(Solrock));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(solrock, Offered(e, 0, "Use Lunar Eclipse"), "either player's Solrock");
                if (!solrock) continue;
                Step(e);
                Assert.IsTrue(e.IsType(lunatone, EnergyType.Darkness));
                Assert.IsFalse(e.IsType(lunatone, EnergyType.Psychic));
            }
        }

        [Test]
        public void Energy_Variation_Gives_Kecleon_The_Types_Of_Its_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C(Kecleon));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Colorless));
            Attach(e, kecleon, Fire, 1);
            Attach(e, kecleon, Water, 1);
            Attach(e, kecleon, MultiEnergy, 1);
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Fire));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Water));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Colorless));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Grass), "Multi Energy isn't a basic Energy card");
        }

        [Test]
        public void Primal_Lock_Discards_The_Opponents_Tools_And_Stops_New_Ones()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Supersonic"), new ScriptedAgent());
            var aero = e.ScenarioActive(0, C(AerodactylEx));
            Attach(e, aero, Fire, 1);
            var theirs = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAttachTool(theirs, C(OranBerry));
            e.ScenarioAddToHand(1, C(OranBerry));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, theirs.Tools.Count);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Play Oran Berry"));
        }

        [Test]
        public void Supernatural_Uses_A_Supporter_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Supernatural", "Use Professor Birch"), new ScriptedAgent());
            var sableye = e.ScenarioActive(0, C(Sableye));
            Attach(e, sableye, Fire, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Birch));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count, "Professor Birch: draw until you have 6");
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Name == "Professor Birch"), "it stays in their hand");
        }
    }
}
