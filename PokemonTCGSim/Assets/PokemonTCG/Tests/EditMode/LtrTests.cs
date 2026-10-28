using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v108: Legendary Treasures (LTR 1-115, RC1-RC25): Cedric Juniper and the Pokédex heights (your rulings), Sealing Scream (your
    /// ruling: ACE SPEC Energy too), Overflow, Leaf Tailor, Royal Heal, Dark Cloak, Magic Room, Safeguard and the prints linked.
    /// </summary>
    public class LtrTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";

        const string Sewaddle = "LTR-9";
        const string Swadloon = "LTR-11";
        const string Leavanny = "LTR-12";
        const string Snivy = "LTR-6";
        const string Servine = "LTR-7";
        const string Serperior = "LTR-8";
        const string Charizard = "LTR-19";
        const string VictiniEX = "LTR-24";
        const string Tepig = "LTR-25";
        const string Magikarp = "LTR-30";
        const string Tympole = "LTR-40";
        const string Kyurem = "LTR-43";
        const string Plusle = "LTR-47";
        const string Minun = "LTR-48";
        const string Sigilyph = "LTR-66";
        const string Gothita = "LTR-69";
        const string Gothorita = "LTR-71";
        const string Gothitelle = "LTR-72";
        const string Spiritomb = "LTR-87";
        const string Zorua = "LTR-89";
        const string WhiteKyuremEX = "LTR-101";
        const string LugiaEX = "LTR-102";
        const string Bianca = "LTR-109";
        const string Cedric = "LTR-110";
        const string Ralts = "LTR-RC8";
        const string Kirlia = "LTR-RC9";
        const string Gardevoir = "LTR-RC10";
        const string SerperiorRC = "LTR-RC3";
        const string MasterBall = "TEF-153";
        const string LegacyEnergy = "TWM-167";
        const string CrushingHammer = "POR-71";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "LTR" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(127, own.Count);
            Assert.AreEqual("CEL-CC20", C("LTR-28").BaseKey, "Reshiram is Celebrations' Classic Collection card");
            Assert.AreEqual("CEL-CC20", C("LTR-114").BaseKey);
            Assert.AreEqual("CEL-CC20", C("LTR-RC22").BaseKey);
            Assert.AreEqual("LTR-105", C("LTR-RC19").BaseKey, "Cinccino");
            Assert.AreEqual("LTR-RC11", C("LTR-RC25").BaseKey, "Meloetta-EX");
            Assert.AreEqual("LTR-49", C("LTR-RC23").BaseKey, "Emolga");
            Assert.AreEqual("XYP-126", C("LTR-RC24").BaseKey, "Mew-EX");
            Assert.IsTrue(C(LugiaEX).IsTeamPlasma, "Lugia-EX is a Team Plasma card");
            Assert.IsFalse(C(Kyurem).IsTeamPlasma);
        }

        [Test]
        public void Pokedex_Heights_Know_The_Species_And_The_Printed_Height()
        {
            Assert.AreEqual("Alolan Vulpix", PokedexHeights.SpeciesOf("Alolan Vulpix"));
            Assert.AreEqual("Mewtwo", PokedexHeights.SpeciesOf("Team Rocket's Mewtwo ex"));
            Assert.AreEqual("Mew", PokedexHeights.SpeciesOf("Mew"));
            Assert.AreEqual("Porygon-Z", PokedexHeights.SpeciesOf("Porygon-Z"));
            Assert.AreEqual(17, PokedexHeights.HeightOf(C(Charizard)));
            Assert.IsTrue(PokedexHeights.HasPrintedHeight(C(Charizard)));
            Assert.IsFalse(PokedexHeights.HasPrintedHeight(C(LugiaEX)), "Pokémon-EX have no Pokédex data");
            Assert.AreEqual("1.7 m (5'07\")", PokedexHeights.Label(17));
            Assert.AreEqual("0.4 m (1'04\")", PokedexHeights.Label(4));
        }

        [Test]
        public void Cedric_Juniper_Needs_A_Pokemon_With_A_Height_And_Someone_Draws_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cedric Juniper"), new ScriptedAgent());
            e.ScenarioActive(0, C(Magikarp));
            e.ScenarioActive(1, C(Magikarp));
            var cedric = e.ScenarioAddToHand(0, C(Cedric));
            var ex = e.ScenarioAddToHand(0, C(VictiniEX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, cedric), "only a Pokémon-EX in the hand");
            e.ScenarioAddToHand(0, C(Charizard));
            Assert.IsTrue(e.CanPlayTrainer(0, cedric));
            int mine = e.State.Players[0].Hand.Count, theirs = e.State.Players[1].Hand.Count;
            Step(e);
            int drewMine = e.State.Players[0].Hand.Count - (mine - 1), drewTheirs = e.State.Players[1].Hand.Count - theirs;
            Assert.IsTrue((drewMine == 3 && drewTheirs == 0) || (drewMine == 0 && drewTheirs == 3), "mine " + drewMine + ", theirs " + drewTheirs);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Charizard"), "the Pokémon goes back to the hand");
            Assert.IsTrue(e.State.Players[0].Hand.Contains(ex));
        }

        [Test]
        public void Bianca_Draws_Until_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bianca"), new ScriptedAgent());
            e.ScenarioActive(0, C(Magikarp));
            e.ScenarioActive(1, C(Magikarp));
            e.ScenarioAddToHand(0, C(Bianca));
            e.ScenarioAddToHand(0, C(Tepig));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Sealing_Scream_Stops_ACE_SPEC_Trainers_And_Energy_For_Both_Players()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Magikarp));
            e.ScenarioBench(1, C(Spiritomb));
            e.ScenarioActive(1, C(Magikarp));
            var ball = e.ScenarioAddToHand(0, C(MasterBall));
            var legacy = e.ScenarioAddToHand(0, C(LegacyEnergy));
            var hammer = e.ScenarioAddToHand(0, C(CrushingHammer));
            var theirBall = e.ScenarioAddToHand(1, C(MasterBall));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.TrainerBlocked(0, ball));
            Assert.IsTrue(e.TrainerBlocked(1, theirBall), "its owner too");
            Assert.IsTrue(e.EnergyCardBlocked(0, legacy), "your ruling: ACE SPEC Energy too");
            Assert.IsFalse(e.TrainerBlocked(0, hammer));
        }

        [Test]
        public void Overflow_Takes_1_More_Prize_And_Plasma_Gale_Needs_Plasma_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Plasma Gale"), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(LugiaEX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(lugia, C(Psychic));
            var karp = e.ScenarioActive(1, C(Magikarp));
            e.ScenarioBench(1, C(Magikarp));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, karp.DamageCounters, "no Plasma Energy: the attack does nothing");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lugia2 = e2.ScenarioActive(0, C(LugiaEX));
            var karp2 = e2.ScenarioActive(1, C(Magikarp));
            e2.ScenarioBench(1, C(Magikarp));
            e2.ScenarioSetTurn(0, 3);
            e2.DealAttackDamage(lugia2, karp2, 120);
            e2.ResolveKnockouts();
            Assert.AreEqual(4, e2.State.Players[0].Prizes.Count, "1 + 1 more for Overflow");
        }

        [Test]
        public void Leaf_Tailor_Removes_Weakness_From_Pokemon_With_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tepig = e.ScenarioActive(0, C(Tepig));
            var leavanny = e.ScenarioActive(1, C(Sewaddle));
            e.ScenarioEvolve(leavanny, C(Swadloon));
            e.ScenarioEvolve(leavanny, C(Leavanny));
            var bare = e.ScenarioBench(1, C(Sewaddle));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tepig, leavanny, 30);
            Assert.AreEqual(6, leavanny.DamageCounters, "no Energy: Fire Weakness");
            e.ScenarioAttach(leavanny, C(Psychic));
            e.DealAttackDamage(tepig, leavanny, 30);
            Assert.AreEqual(9, leavanny.DamageCounters, "with Energy: no Weakness");
            Assert.AreEqual(0, bare.DamageCounters);
        }

        [Test]
        public void Royal_Heal_Heals_10_From_Each_Of_Your_Pokemon_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snivy = e.ScenarioActive(0, C(Snivy));
            var serperior = e.ScenarioBench(0, C(Snivy));
            e.ScenarioEvolve(serperior, C(Servine));
            e.ScenarioEvolve(serperior, C(Serperior));
            var foe = e.ScenarioActive(1, C(Magikarp));
            snivy.DamageCounters = 3;
            serperior.DamageCounters = 2;
            foe.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(2, snivy.DamageCounters);
            Assert.AreEqual(1, serperior.DamageCounters);
            Assert.AreEqual(1, foe.DamageCounters, "only your own Pokémon");
        }

        [Test]
        public void Dark_Cloak_And_Royal_Garden_Remove_The_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zorua = e.ScenarioActive(0, C(Zorua));
            e.ScenarioBench(0, C("LTR-88"));
            var serperior = e.ScenarioBench(0, C(Snivy));
            e.ScenarioEvolve(serperior, C("LTR-RC2"));
            e.ScenarioEvolve(serperior, C(SerperiorRC));
            e.ScenarioActive(1, C(Magikarp));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.RetreatCostOf(zorua));
            e.ScenarioAttach(zorua, C(Darkness));
            Assert.AreEqual(0, e.RetreatCostOf(zorua), "Dark Cloak");
            Assert.AreEqual(2, e.RetreatCostOf(serperior));
            e.ScenarioAttach(serperior, C(Grass));
            Assert.AreEqual(0, e.RetreatCostOf(serperior), "Royal Garden");
        }

        [Test]
        public void Magic_Room_Stops_The_Opponents_Items_While_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gothitelle = e.ScenarioActive(0, C(Gothita));
            e.ScenarioEvolve(gothitelle, C(Gothorita));
            e.ScenarioEvolve(gothitelle, C(Gothitelle));
            e.ScenarioActive(1, C(Magikarp));
            var theirs = e.ScenarioAddToHand(1, C(CrushingHammer));
            var mine = e.ScenarioAddToHand(0, C(CrushingHammer));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e.TrainerBlocked(1, theirs));
            Assert.IsFalse(e.TrainerBlocked(0, mine), "not its owner's Items");
        }

        [Test]
        public void Safeguard_Stops_Pokemon_EX_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sigilyph = e.ScenarioActive(0, C(Sigilyph));
            var victini = e.ScenarioActive(1, C(VictiniEX));
            var tepig = e.ScenarioBench(1, C(Tepig));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(victini, sigilyph, 50);
            Assert.AreEqual(0, sigilyph.DamageCounters);
            e.DealAttackDamage(tepig, sigilyph, 30);
            Assert.AreEqual(3, sigilyph.DamageCounters, "not a Pokémon-EX");
        }

        [Test]
        public void Turbo_Energize_Energy_For_Safeguard_Sigilyph_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Turbo Energize", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var victini = e.ScenarioActive(0, C(VictiniEX));
            e.ScenarioAttach(victini, C(Fire));
            var sigilyph = e.ScenarioBench(0, C(Sigilyph));
            e.ScenarioActive(1, C(Magikarp));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, sigilyph.Energy.Count, "Safeguard: even your own Pokémon-EX's attack");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.IsBasicEnergy), "the 2 Energy are discarded (2013-06-20)");
        }

        [Test]
        public void Glaciate_Hits_Each_Opposing_Pokemon_For_30()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Glaciate"), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C(Kyurem));
            e.ScenarioAttach(kyurem, C(Water));
            e.ScenarioAttach(kyurem, C(Water));
            e.ScenarioAttach(kyurem, C(Psychic));
            var active = e.ScenarioActive(1, C(Tympole));
            var bench = e.ScenarioBench(1, C(Tympole));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, active.DamageCounters);
            Assert.AreEqual(3, bench.DamageCounters);
        }

        [Test]
        public void Tag_Draw_Draws_8_With_Minun_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tag Draw"), new ScriptedAgent());
            var plusle = e.ScenarioActive(0, C(Plusle));
            e.ScenarioAttach(plusle, C(Psychic));
            e.ScenarioBench(0, C(Minun));
            e.ScenarioAddToHand(0, C(Tepig));
            e.ScenarioActive(1, C(Magikarp));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Ice_Burn_Discards_2_Fire_And_Burns()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ice Burn"), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C(WhiteKyuremEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(kyurem, C(Fire));
            e.ScenarioAttach(kyurem, C(Water));
            var foe = e.ScenarioActive(1, C("LTR-29"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, kyurem.Energy.Count(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(15, foe.DamageCounters);
            Assert.IsTrue(foe.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Eternal_Radiance_Moves_All_Damage_Counters_To_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Eternal Radiance"), new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C(Ralts));
            e.ScenarioEvolve(gardevoir, C(Kirlia));
            e.ScenarioEvolve(gardevoir, C(Gardevoir));
            e.ScenarioAttach(gardevoir, C(Psychic));
            e.ScenarioAttach(gardevoir, C(Psychic));
            gardevoir.DamageCounters = 5;
            var foe = e.ScenarioActive(1, C("LTR-26"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, gardevoir.DamageCounters);
            Assert.AreEqual(5, foe.DamageCounters);
        }
    }
}
