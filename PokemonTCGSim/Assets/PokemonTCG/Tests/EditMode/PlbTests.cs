using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v109: Plasma Blast (PLB 1-105): the Team Plasma cards (Plasma Energy, Red Signal, Strike of the Champion, Silver Bangle and
    /// Silver Mirror, Drifting Balloon), the ACE SPEC Genesect-EX Tools, Restored Pokémon and their Fossils, Verdant Wind, Cursed
    /// Glare, Mental Shroud, Toolbox, Badge of Discipline and Iris.
    /// </summary>
    public class PlbTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";

        const string Lileep = "PLB-3";
        const string Tropius = "PLB-5";
        const string Accelgor = "PLB-8";
        const string GenesectEX = "PLB-11";
        const string Froslass = "PLB-23";
        const string Snorunt = "PLB-21";
        const string KyuremEX = "PLB-30";
        const string Drifloon = "PLB-34";
        const string Drifblim = "PLB-35";
        const string Uxie = "PLB-36";
        const string Mesprit = "PLB-37";
        const string Azelf = "PLB-38";
        const string Sigilyph = "PLB-41";
        const string Machop = "PLB-47";
        const string Machoke = "PLB-48";
        const string Machamp = "PLB-49";
        const string Axew = "PLB-67";
        const string Fraxure = "PLB-68";
        const string Haxorus = "PLB-69";
        const string Iris = "PLB-81";
        const string RootFossil = "PLB-87";
        const string SilverBangle = "PLB-88";
        const string SilverMirror = "PLB-89";
        const string PlasmaEnergy = "PLB-91";
        const string GBooster = "PLB-92";
        const string Throh = "PLB-51";
        const string VirizionEX = "PLB-9";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "PLB").ToList();
            Assert.AreEqual(89, own.Count);
            Assert.AreEqual("30C-CC20", C(GenesectEX).BaseKey, "Genesect-EX is the Classic Collection card");
            Assert.IsTrue(C(GenesectEX).IsTeamPlasma);
            Assert.AreEqual("LTR-15", C("PLB-103").BaseKey, "Virizion");
            Assert.AreEqual("PLB-9", C("PLB-96").BaseKey);
            Assert.AreEqual("TEF-153", C("PLB-94").BaseKey, "Master Ball");
            Assert.IsTrue(C(PlasmaEnergy).IsTeamPlasma && C(PlasmaEnergy).IsSpecialEnergy);
            Assert.IsTrue(C(GBooster).IsAceSpec && C(GBooster).IsTeamPlasma && C(GBooster).IsTool);
            Assert.AreEqual(Stage.Restored, C(Lileep).Stage);
            Assert.IsFalse(C(Machop).IsTeamPlasma);
        }

        [Test]
        public void Red_Signal_Switches_In_A_Benched_Pokemon_When_Plasma_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Plasma Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(GenesectEX));
            e.ScenarioAddToHand(0, C(PlasmaEnergy));
            e.ScenarioActive(1, C(Machop));
            e.ScenarioBench(1, C(Throh));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Throh", e.State.Players[1].Active.Name);
        }

        [Test]
        public void Verdant_Wind_Protects_Pokemon_With_Grass_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var virizion = e.ScenarioActive(0, C(VirizionEX));
            e.ScenarioActive(1, C(Machop));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e.ApplyCondition(virizion, SpecialCondition.Poisoned), "no Grass Energy yet");
            e.ScenarioAttach(virizion, C(Grass));
            Assert.IsFalse(e.ApplyCondition(virizion, SpecialCondition.Confused), "with Grass Energy attached");
        }

        [Test]
        public void Cursed_Glare_Stops_The_Opponents_Special_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var froslass = e.ScenarioActive(0, C(Snorunt));
            e.ScenarioEvolve(froslass, C(Froslass));
            e.ScenarioActive(1, C(Machop));
            var plasma = e.ScenarioAddToHand(1, C(PlasmaEnergy));
            var basic = e.ScenarioAddToHand(1, C(Fighting));
            var mine = e.ScenarioAddToHand(0, C(PlasmaEnergy));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e.EnergyCardBlocked(1, plasma));
            Assert.IsFalse(e.EnergyCardBlocked(1, basic));
            Assert.IsFalse(e.EnergyCardBlocked(0, mine), "not its owner's");
        }

        [Test]
        public void Drifting_Balloon_Costs_Less_For_Each_Opposing_Team_Plasma_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var drifblim = e.ScenarioActive(0, C(Drifloon));
            e.ScenarioEvolve(drifblim, C(Drifblim));
            e.ScenarioActive(1, C(Accelgor));
            e.ScenarioBench(1, C(KyuremEX));
            e.ScenarioSetTurn(0, 3);
            var derail = drifblim.Def.Attacks.First(a => a.Name == "Derail");
            Assert.AreEqual(2, e.AttackCost(drifblim, derail).Count, "Accelgor is Team Plasma, Kyurem-EX isn't");
            e.ScenarioBench(1, C(GenesectEX));
            Assert.AreEqual(1, e.AttackCost(drifblim, derail).Count);
        }

        [Test]
        public void Mental_Shroud_Needs_Uxie_And_Azelf()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mesprit = e.ScenarioActive(0, C(Mesprit));
            e.ScenarioBench(0, C(Uxie));
            var foe = e.ScenarioActive(1, C(Mesprit));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(foe, mesprit, 20);
            Assert.AreEqual(4, mesprit.DamageCounters, "no Azelf: Psychic Weakness");
            e.ScenarioBench(0, C(Azelf));
            e.DealAttackDamage(foe, mesprit, 10);
            Assert.AreEqual(5, mesprit.DamageCounters, "Uxie and Azelf: no Weakness");
        }

        [Test]
        public void Toolbox_Holds_4_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sigilyph = e.ScenarioActive(0, C(Sigilyph));
            var machop = e.ScenarioBench(0, C(Machop));
            e.ScenarioActive(1, C(Machop));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(4, e.MaxTools(sigilyph));
            Assert.AreEqual(1, e.MaxTools(machop));
        }

        [Test]
        public void Badge_Of_Discipline_Ignores_Resistance_For_Fighting_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var machop = e.ScenarioActive(0, C(Machop));
            var tropius = e.ScenarioActive(1, C(Tropius));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(machop, tropius, 30);
            Assert.AreEqual(1, tropius.DamageCounters, "Fighting Resistance -20");
            var machamp = e.ScenarioBench(0, C(Machop));
            e.ScenarioEvolve(machamp, C(Machoke));
            e.ScenarioEvolve(machamp, C(Machamp));
            e.DealAttackDamage(machop, tropius, 30);
            Assert.AreEqual(4, tropius.DamageCounters, "Badge of Discipline");
        }

        [Test]
        public void Strike_Of_The_Champion_Knocks_Out_A_Team_Plasma_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Strike of the Champion"), new ScriptedAgent());
            var haxorus = e.ScenarioActive(0, C(Axew));
            e.ScenarioEvolve(haxorus, C(Fraxure));
            e.ScenarioEvolve(haxorus, C(Haxorus));
            e.ScenarioAttach(haxorus, C(Fighting));
            e.ScenarioAttach(haxorus, C(Metal));
            var accelgor = e.ScenarioActive(1, C(Accelgor));
            e.ScenarioBench(1, C(Machop));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(accelgor.Uid), "Knocked Out");
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Silver_Bangle_And_Silver_Mirror()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var machop = e.ScenarioActive(0, C(Machop));
            e.ScenarioAttachTool(machop, C(SilverBangle));
            var kyurem = e.ScenarioActive(1, C(KyuremEX));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(machop, kyurem, 30);
            Assert.AreEqual(6, kyurem.DamageCounters, "30 more against a Pokémon-EX");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mirrored = e2.ScenarioActive(0, C(Machop));
            e2.ScenarioAttachTool(mirrored, C(SilverMirror));
            var accelgor = e2.ScenarioActive(1, C(Accelgor));
            var throh = e2.ScenarioBench(1, C(Throh));
            e2.ScenarioSetTurn(1, 4);
            e2.DealAttackDamage(accelgor, mirrored, 30);
            Assert.AreEqual(0, mirrored.DamageCounters, "Accelgor is Team Plasma");
            e2.DealAttackDamage(throh, mirrored, 30);
            Assert.AreEqual(3, mirrored.DamageCounters);
        }

        [Test]
        public void G_Booster_Gives_Genesect_EX_Its_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: G Booster"), new ScriptedAgent());
            var genesect = e.ScenarioActive(0, C(GenesectEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(genesect, C(Grass));
            e.ScenarioAttachTool(genesect, C(GBooster));
            var foe = e.ScenarioActive(1, C(KyuremEX));
            e.ScenarioBench(1, C(Machop));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(foe.Uid), "200 damage");
            Assert.AreEqual(1, genesect.Energy.Count, "2 Energy discarded");
        }

        [Test]
        public void Iris_Adds_10_Per_Prize_The_Opponent_Took()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Iris"), new ScriptedAgent());
            var machop = e.ScenarioActive(0, C(Machop));
            var throh = e.ScenarioActive(1, C(Throh));
            e.ScenarioAddToHand(0, C(Iris));
            e.State.Players[1].Prizes.RemoveAt(0);
            e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.DealAttackDamage(machop, throh, 30);
            Assert.AreEqual(5, throh.DamageCounters, "30 + 2 x 10");
        }

        [Test]
        public void Professor_Juniper_Sycamore_And_Research_Cant_Share_A_Deck()
        {
            var juniper = db.Cards.First(d => d.Name == "Professor Juniper");
            var sycamore = db.Cards.First(d => d.Name == "Professor Sycamore");
            var research = db.Cards.First(d => d.Name == "Professor's Research");
            var deck = new System.Collections.Generic.List<CardDef> { juniper, juniper };
            for (int i = 0; i < 4; i++) deck.Add(C(Machop));
            while (deck.Count < 60) deck.Add(C(Fighting));
            Assert.IsFalse(Decklist.Validate(deck, (System.Collections.Generic.ICollection<string>)null).Any(p => p.Contains("same deck")));
            deck[58] = sycamore;
            Assert.IsTrue(Decklist.Validate(deck, (System.Collections.Generic.ICollection<string>)null).Any(p => p.Contains("same deck")));
            deck[58] = research;
            Assert.IsTrue(Decklist.Validate(deck, (System.Collections.Generic.ICollection<string>)null).Any(p => p.Contains("same deck")));
        }

        [Test]
        public void Root_Fossil_Lileep_And_Prehistoric_Call()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Root Fossil Lileep"), new ScriptedAgent());
            e.ScenarioActive(0, C(Machop));
            e.ScenarioActive(1, C(Machop));
            e.ScenarioAddToDeck(0, C(Lileep), 1);
            e.ScenarioAddToHand(0, C(RootFossil));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Bench.Any(m => m.Name == "Lileep"), "from the bottom 7 cards");

            var e2 = Scenario(new ScriptedAgent().Then("Prehistoric Call"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Machop));
            e2.ScenarioActive(1, C(Machop));
            e2.ScenarioAddToDiscard(0, C(Lileep), 1);
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsFalse(e2.State.Players[0].Discard.Any(c => c.Def.Name == "Lileep"));
            Assert.AreEqual("Lileep", e2.State.Players[0].Deck.Last().Def.Name, "the bottom of the deck");
        }
    }
}
