using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v107: XY Black Star Promos (XYP XY01-XY211): Leap Through Time (your ruling), Contrary, Type Switch, Diamond Force, Ozone Wall,
    /// Benevolent Boss, Levitate, Karen, Trevor, the Spirit Links and the reprinted promos linked as prints.
    /// </summary>
    public class XypTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Fairy = "SUM-Y";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Inkay = "KSS-18";
        const string Pancham = "BKP-71";
        const string Koffing = "UNB-73";

        const string Diancie = "XYP-43";
        const string MDiancie = "XYP-44";
        const string Pangoro = "XYP-50";
        const string Malamar = "XYP-58";
        const string Rayquaza = "XYP-64";
        const string Celebi = "XYP-93";
        const string Arceus = "XYP-116";
        const string Weezing = "XYP-163";
        const string Karen = "XYP-177";
        const string Trevor = "XYP-33";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "XYP" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(129, own.Count);
            Assert.AreEqual("AOR-36", C("XYP-71").BaseKey, "Hoopa-EX");
            Assert.AreEqual("FCO-21", C("XYP-128").BaseKey, "White Kyurem");
            Assert.AreEqual("XYP-111", C("XYP-187").BaseKey, "the Mythical collection reprint of Celebi");
            Assert.AreEqual("XYP-203", C("XYP-210").BaseKey, "Gym Badge");
            Assert.AreEqual("XYP-67", C("XYP-67a").BaseKey);
        }

        [Test]
        public void Leap_Through_Time_Either_Escapes_Or_Is_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var celebi = e.ScenarioActive(1, C(Celebi));
            e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[1].Deck.Count;
            e.DealAttackDamage(snorlax, celebi, 100);
            e.ResolveKnockouts();
            bool escaped = e.State.Players[1].Deck.Count == deck + 1;
            if (escaped)
            {
                Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "no Prize card");
                Assert.IsFalse(e.State.Players[1].Discard.Any(c => c.Def.Name == "Celebi"));
                Assert.AreEqual(0, e.State.Players[1].KnockOuts.Count, "not Knocked Out");
            }
            else
            {
                Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
                Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Celebi"));
            }
            Assert.IsNull(e.State.FindPokemon(celebi.Uid));
        }

        [Test]
        public void Contrary_Makes_The_Opponents_Flips_Tails_On_Their_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            var malamar = e.ScenarioActive(1, C(Inkay));
            e.ScenarioEvolve(malamar, C(Malamar));
            e.ScenarioSetTurn(0, 3);
            for (int i = 0; i < 8; i++) Assert.IsFalse(e.FlipCoin(0, "test"));
        }

        [Test]
        public void Type_Switch_Changes_Arceus_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Type Switch", "Water"), new ScriptedAgent());
            var arceus = e.ScenarioActive(0, C(Arceus));
            e.ScenarioAttach(arceus, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.IsType(arceus, EnergyType.Colorless));
            Step(e);
            Assert.IsTrue(e.IsType(arceus, EnergyType.Water));
            Assert.IsFalse(e.IsType(arceus, EnergyType.Colorless));
        }

        [Test]
        public void Diamond_Force_Stops_Pokemon_EX_Damage_To_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Diamond Force"), new ScriptedAgent());
            var diancie = e.ScenarioActive(0, C(Diancie));
            e.ScenarioEvolve(diancie, C(MDiancie));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(diancie, C(Fairy));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            var ex = e.ScenarioActive(1, C("FFI-54"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(ex, diancie, 100);
            e.DealAttackDamage(ex, lapras, 30);
            Assert.AreEqual(0, diancie.DamageCounters);
            Assert.AreEqual(0, lapras.DamageCounters);
        }

        [Test]
        public void Ozone_Wall_Protects_Rayquaza_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioActive(1, C(LaprasGX));
            var rayquaza = e.ScenarioBench(1, C(Rayquaza));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, rayquaza, 30);
            Assert.AreEqual(0, rayquaza.DamageCounters);
        }

        [Test]
        public void Benevolent_Boss_And_Levitate()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pangoro = e.ScenarioActive(0, C(Pancham));
            e.ScenarioEvolve(pangoro, C(Pangoro));
            e.ScenarioBench(0, C(LaprasGX));
            var weezing = e.ScenarioBench(0, C(Koffing));
            e.ScenarioEvolve(weezing, C(Weezing));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(140, e.MaxHp(pangoro), "100 + 2 Benched x 20");
            Assert.AreEqual(3, e.RetreatCostOf(weezing));
            e.ScenarioAttach(weezing, C(Psychic));
            Assert.AreEqual(0, e.RetreatCostOf(weezing));
        }

        [Test]
        public void Karen_And_Trevor()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Karen"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Karen));
            e.ScenarioAddToDiscard(0, C(Inkay), 2);
            e.ScenarioAddToDiscard(1, C(Pancham), 1);
            e.ScenarioAddToDiscard(1, C(Water), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[0].Discard.Any(c => c.Def.IsPokemon));
            Assert.IsFalse(e.State.Players[1].Discard.Any(c => c.Def.IsPokemon));
            Assert.AreEqual(1, e.State.Players[1].Discard.Count, "the Energy stays");

            var e2 = Scenario(new ScriptedAgent().Then("Play Trevor"), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            e2.ScenarioAddToHand(0, C(Trevor));
            e2.ScenarioAddToDeck(0, C(Inkay), 1);
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(e2.State.Players[0].Hand.Any(c => c.Def.Name == "Inkay"));
        }
    }
}
