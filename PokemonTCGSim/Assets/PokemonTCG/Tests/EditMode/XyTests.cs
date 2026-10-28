using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v105: XY (XY 1-146): Forest's Curse, Water Shuriken, Mystical Fire, Upside-Down Evolution (any turn, your ruling), Spirit
    /// Scream, Sweet Veil, Fur Coat, Cassius (even your only Pokémon, your ruling), Muscle Band, Hard Charm, Shadow Circle and the rest.
    /// </summary>
    public class XyTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Fairy = "SUM-Y";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string FfiLucarioEX = "FFI-54";
        const string RollerSkates = "PHF-103";

        const string Fennekin = "XY-24";
        const string Braixen = "XY-25";
        const string Delphox = "XY-26";
        const string Froakie = "XY-39";
        const string Frogadier = "XY-40";
        const string Greninja = "XY-41";
        const string Phantump = "XY-54";
        const string Trevenant = "XY-55";
        const string Pumpkaboo = "XY-56";
        const string Gourgeist = "XY-57";
        const string Inkay = "XY-74";
        const string Malamar = "XY-76";
        const string Swirlix = "XY-94";
        const string Slurpuff = "XY-95";
        const string Furfrou = "XY-114";
        const string Cassius = "XY-115";
        const string HardCharm = "XY-119";
        const string MuscleBand = "XY-121";
        const string ShadowCircle = "XY-126";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "XY" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(104, own.Count);
            Assert.AreEqual("EVO-1", C("XY-1").BaseKey, "Venusaur-EX is the Evolutions card");
            Assert.AreEqual("EVO-1", C("XY-141").BaseKey, "and so is its full art");
            Assert.AreEqual("GEN-71", C("XY-124").BaseKey, "Red Card");
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C("XY-124")), "Red Card is on the Expanded ban list");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Trevenant)));
        }

        [Test]
        public void Forest_s_Curse_Stops_Items_While_Trevenant_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var skates = e.ScenarioAddToHand(0, C(RollerSkates));
            var trevenant = e.ScenarioActive(1, C(Phantump));
            e.ScenarioEvolve(trevenant, C(Trevenant));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, skates));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            var other = e2.ScenarioAddToHand(0, C(RollerSkates));
            e2.ScenarioActive(1, C(SnorlaxGX));
            var benched = e2.ScenarioBench(1, C(Phantump));
            e2.ScenarioEvolve(benched, C(Trevenant));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e2.CanPlayTrainer(0, other), "only from the Active Spot");
        }

        [Test]
        public void Water_Shuriken_Puts_3_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Water Shuriken"), new ScriptedAgent());
            var greninja = e.ScenarioActive(0, C(Froakie));
            e.ScenarioEvolve(greninja, C(Frogadier));
            e.ScenarioEvolve(greninja, C(Greninja));
            e.ScenarioAddToHand(0, C(Water));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snorlax.DamageCounters);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Mystical_Fire_Draws_Up_To_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Mystical Fire"), new ScriptedAgent());
            var delphox = e.ScenarioActive(0, C(Fennekin));
            e.ScenarioEvolve(delphox, C(Braixen));
            e.ScenarioEvolve(delphox, C(Delphox));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Upside_Down_Evolution_Works_The_Turn_Inkay_Came_Into_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Upside-Down Evolution"), new ScriptedAgent());
            e.ScenarioSetTurn(0, 1);
            var inkay = e.ScenarioActive(0, C(Inkay));
            e.ScenarioAddToDeck(0, C(Malamar), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ApplyCondition(inkay, SpecialCondition.Confused);
            Step(e);
            Assert.AreEqual("Malamar", inkay.Name);
            Assert.IsFalse(inkay.Has(SpecialCondition.Confused), "evolving cures it");
        }

        [Test]
        public void Spirit_Scream_Leaves_Both_Active_Pokemon_With_10_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spirit Scream"), new ScriptedAgent());
            var gourgeist = e.ScenarioActive(0, C(Pumpkaboo));
            e.ScenarioEvolve(gourgeist, C(Gourgeist));
            e.ScenarioAttach(gourgeist, C(Psychic));
            e.ScenarioAttach(gourgeist, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, snorlax.DamageCounters);
            Assert.AreEqual(9, gourgeist.DamageCounters);
        }

        [Test]
        public void Sweet_Veil_Shields_Pokemon_With_Fairy_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAttach(lapras, C(Fairy));
            var slurpuff = e.ScenarioBench(0, C(Swirlix));
            e.ScenarioEvolve(slurpuff, C(Slurpuff));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.ApplyCondition(lapras, SpecialCondition.Poisoned));
            Assert.IsTrue(e.ApplyCondition(snorlax, SpecialCondition.Poisoned));
        }

        [Test]
        public void Fur_Coat_And_Hard_Charm_Take_20_Off_And_Muscle_Band_Adds_20()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var furfrou = e.ScenarioActive(1, C(Furfrou));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, furfrou, 60);
            Assert.AreEqual(4, furfrou.DamageCounters);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e2.ScenarioActive(0, C(SnorlaxGX));
            var lapras = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            e2.ScenarioAttachTool(lapras, C(HardCharm));
            e2.DealAttackDamage(attacker, lapras, 60);
            Assert.AreEqual(4, lapras.DamageCounters);
            e2.ScenarioAttachTool(attacker, C(MuscleBand));
            e2.DealAttackDamage(attacker, lapras, 60);
            Assert.AreEqual(10, lapras.DamageCounters, "60 + 20 - 20");
        }

        [Test]
        public void Shadow_Circle_Removes_Weakness_With_Darkness_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lucario = e.ScenarioActive(0, C(FfiLucarioEX));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAttach(snorlax, C(Darkness));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(lucario, snorlax, 50);
            Assert.AreEqual(10, snorlax.DamageCounters, "Fighting Weakness");
            e.ScenarioSetStadium(0, C(ShadowCircle));
            e.DealAttackDamage(lucario, snorlax, 50);
            Assert.AreEqual(15, snorlax.DamageCounters);
        }

        [Test]
        public void Cassius_Can_Shuffle_In_Your_Only_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cassius"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Cassius));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Deck.Any(c => c.Def.Name == "Lapras-GX"));
            Assert.AreEqual(1, e.State.Winner, "no Pokémon left in play");
        }
    }
}
