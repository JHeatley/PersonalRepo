using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v102: Phantom Forces (PHF 1-122): Night March, Pokémon Tool F (Head Ringer, Jamming Net: on the opponent's Pokémon-EX, your
    /// ruling), Robo Substitute, Dimension Valley, Mystery Energy, Steel Shelter, Mighty Shield, High Density Armor, Slip Trip and the rest.
    /// </summary>
    public class PhfTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string DoubleColorless = "SLG-69";
        const string FloatStone = "BKT-137";

        const string Joltik = "PHF-26";
        const string Lampent = "PHF-42";
        const string Pumpkaboo = "PHF-44";
        const string Gourgeist = "PHF-45";
        const string Roggenrola = "PHF-48";
        const string Boldore = "PHF-49";
        const string Gigalith = "PHF-50";
        const string AegislashEX = "PHF-65";
        const string Goomy = "PHF-75";
        const string Sliggoo = "PHF-76";
        const string Goodra = "PHF-77";
        const string DimensionValley = "PHF-93";
        const string HeadRinger = "PHF-97";
        const string JammingNet = "PHF-98";
        const string TrumpCard = "PHF-99";
        const string RoboSubstitute = "PHF-102";
        const string SteelShelter = "PHF-105";
        const string Xerosic = "PHF-110";
        const string MysteryEnergy = "PHF-112";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "PHF" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(101, own.Count);
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(TrumpCard)), "on the Expanded ban list");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Joltik)));
            Assert.AreEqual("PHF-65", C("PHF-65a").BaseKey);
            Assert.AreEqual("ROS-110", C("PHF-109").BaseKey, "VS Seeker");
            var robo = C(RoboSubstitute);
            Assert.IsTrue(robo.IsFossil && robo.FossilPlain && !robo.FossilGivesPrize);
            Assert.AreEqual(30, robo.Hp);
        }

        [Test]
        public void Night_March_Counts_The_Night_Marchers_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Night March"), new ScriptedAgent());
            var joltik = e.ScenarioActive(0, C(Joltik));
            e.ScenarioAttach(joltik, C(Psychic));
            e.ScenarioAttach(joltik, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Pumpkaboo), 2);
            e.ScenarioAddToDiscard(0, C(Lampent), 1);
            e.ScenarioAddToDiscard(0, C(Gourgeist), 1);
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, snorlax.DamageCounters, "3 Night Marchers: 60");
        }

        [Test]
        public void Head_Ringer_Goes_On_The_Opponents_Pokemon_EX_And_Raises_Its_Costs()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Head Ringer"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var ringer = e.ScenarioAddToHand(0, C(HeadRinger));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, ringer), "no Pokémon-EX on the other side");
            var aegislash = e.ScenarioBench(1, C(AegislashEX));
            Assert.IsTrue(e.CanPlayTrainer(0, ringer));
            Step(e);
            Assert.AreEqual(1, aegislash.Tools.Count);
            Assert.AreEqual(0, snorlax.Tools.Count);
            Assert.AreEqual(0, e.State.Players[0].Active.Tools.Count, "not on your own Pokémon");
            e.ScenarioAttach(aegislash, C(Psychic));
            e.ScenarioAttach(aegislash, C(Psychic));
            e.ScenarioAttach(aegislash, C(Psychic));
            Assert.IsFalse(e.UsableAttacks(aegislash).Any(a => a.Attack.Name == "Slash Blast"), "[C][C][C] + [C]");
            e.ScenarioAttach(aegislash, C(Psychic));
            Assert.IsTrue(e.UsableAttacks(aegislash).Any(a => a.Attack.Name == "Slash Blast"));
        }

        [Test]
        public void Jamming_Net_Takes_20_Off_Its_Pokemons_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            var aegislash = e.ScenarioActive(1, C(AegislashEX));
            e.ScenarioAttachTool(aegislash, C(JammingNet));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(aegislash, lapras, 60, false);
            Assert.AreEqual(4, lapras.DamageCounters);
        }

        [Test]
        public void Dimension_Valley_And_Mystery_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pumpkaboo = e.ScenarioActive(0, C(Pumpkaboo));
            e.ScenarioAttach(pumpkaboo, C(Psychic));
            e.ScenarioAttach(pumpkaboo, C(MysteryEnergy));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(pumpkaboo), "2 less");
            Assert.IsFalse(e.UsableAttacks(pumpkaboo).Any(a => a.Attack.Name == "Night March"));
            e.ScenarioSetStadium(1, C(DimensionValley));
            Assert.IsTrue(e.UsableAttacks(pumpkaboo).Any(a => a.Attack.Name == "Night March"), "[C][C][C] - [C]");
        }

        [Test]
        public void Mighty_Shield_And_Steel_Shelter()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var aegislash = e.ScenarioActive(1, C(AegislashEX));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, aegislash, 50);
            Assert.AreEqual(5, aegislash.DamageCounters);
            e.ScenarioAttach(snorlax, C(DoubleColorless));
            e.DealAttackDamage(snorlax, aegislash, 50);
            Assert.AreEqual(5, aegislash.DamageCounters, "Special Energy on the attacker");
            e.ScenarioSetStadium(0, C(SteelShelter));
            Assert.IsFalse(e.ApplyCondition(aegislash, SpecialCondition.Poisoned));
            Assert.IsTrue(e.ApplyCondition(snorlax, SpecialCondition.Poisoned));
        }

        [Test]
        public void High_Density_Armor_Only_At_Full_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var gigalith = e.ScenarioActive(1, C(Roggenrola));
            e.ScenarioEvolve(gigalith, C(Boldore));
            e.ScenarioEvolve(gigalith, C(Gigalith));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, gigalith, 60);
            Assert.AreEqual(1, gigalith.DamageCounters, "60 - 50");
            e.DealAttackDamage(snorlax, gigalith, 60);
            Assert.AreEqual(7, gigalith.DamageCounters);
        }

        [Test]
        public void Gourgantic_Makes_It_200_HP_With_Grass_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gourgeist = e.ScenarioActive(0, C(Pumpkaboo));
            e.ScenarioEvolve(gourgeist, C(Gourgeist));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(100, e.MaxHp(gourgeist));
            e.ScenarioAttach(gourgeist, C(Grass));
            Assert.AreEqual(200, e.MaxHp(gourgeist));
        }

        [Test]
        public void Slip_Trip_Stops_Tools_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var stone = e.ScenarioAddToHand(0, C(FloatStone));
            var goodra = e.ScenarioActive(1, C(Goomy));
            e.ScenarioEvolve(goodra, C(Sliggoo));
            e.ScenarioEvolve(goodra, C(Goodra));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, stone));
        }

        [Test]
        public void Robo_Substitute_Is_Played_Onto_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Robo Substitute"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(RoboSubstitute));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var robo = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Robo Substitute", robo.Name);
            Assert.AreEqual(30, e.MaxHp(robo));
            Assert.AreEqual(0, e.PrizesFor(robo), "no Prize card for it");
            Assert.IsTrue(e.ApplyCondition(robo, SpecialCondition.Poisoned), "Special Conditions affect it");
        }

        [Test]
        public void Xerosic_Discards_A_Tool_Or_Special_Energy_From_Either_Side()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Xerosic"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Xerosic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAttach(snorlax, C(DoubleColorless));
            e.ScenarioAttach(snorlax, C(Fighting));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, snorlax.Energy.Count);
            Assert.IsTrue(snorlax.Energy[0].Def.IsBasicEnergy, "the Special Energy went");
        }
    }
}
