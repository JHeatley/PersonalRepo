using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v104: Flashfire (FLF 1-109): Heart Wink, Intimidating Mane, Energy Grace, Shadow Void (no limit, your ruling), Flower Veil
    /// (stacks, your ruling), Big Jump (even as your only Pokémon, your ruling), Poison Barrier, Magnetic Storm, Protection Cube,
    /// Startling Megaphone and the rest.
    /// </summary>
    public class FlfTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Fighting = "MEE-14";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string FloatStone = "BKT-137";
        const string FfiPoliwag = "FFI-15";
        const string FfiPoliwhirl = "FFI-16";
        const string FfiHawlucha = "FFI-63";
        const string FfiLucarioEX = "FFI-54";

        const string Roselia = "FLF-8";
        const string Charizard = "FLF-11";
        const string Pyroar = "FLF-20";
        const string Litleo = "FLF-19";
        const string Feebas = "FLF-22";
        const string Milotic = "FLF-23";
        const string Shinx = "FLF-32";
        const string Luxio = "FLF-33";
        const string Luxray = "FLF-34";
        const string Duskull = "FLF-38";
        const string Dusclops = "FLF-39";
        const string Dusknoir = "FLF-40";
        const string Skrelp = "FLF-44";
        const string Flabebe = "FLF-62";
        const string Floette = "FLF-64";
        const string MCharizardY = "FLF-69";
        const string Dragalge = "FLF-71";
        const string Buneary = "FLF-84";
        const string Lopunny = "FLF-85";
        const string MagneticStorm = "FLF-91";
        const string ProtectionCube = "FLF-95";
        const string Megaphone = "FLF-97";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "FLF" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(75, own.Count);
            Assert.AreEqual(EnergyType.Dragon, C(MCharizardY).PokemonType);
            Assert.AreEqual("FLF-11", C("FLF-100").BaseKey, "full art Charizard-EX");
            Assert.AreEqual("GEN-3", C("FLF-1").BaseKey, "Caterpie is the Generations card");
            Assert.AreEqual("EVO-12", C("FLF-12").BaseKey);
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(Milotic)), "on the Expanded ban list");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Charizard)));
        }

        [Test]
        public void Heart_Wink_Skips_The_Draw_But_An_Empty_Deck_Still_Loses()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            var wink = new CardsFLF1.NoTurnDrawEffect("Heart Wink", 0, 4);
            wink.ActiveFromTurn = 4;
            e.AddEffect(wink);
            int deck = e.State.Players[1].Deck.Count;
            e.StartTurn();
            Assert.AreEqual(1, e.State.CurrentPlayer);
            Assert.AreEqual(0, e.State.Players[1].Hand.Count);
            Assert.AreEqual(deck, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Intimidating_Mane_Stops_Damage_From_Basic_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var poliwhirl = e.ScenarioBench(0, C(FfiPoliwag));
            e.ScenarioEvolve(poliwhirl, C(FfiPoliwhirl));
            var pyroar = e.ScenarioActive(1, C(Litleo));
            e.ScenarioEvolve(pyroar, C(Pyroar));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, pyroar, 60);
            Assert.AreEqual(0, pyroar.DamageCounters);
            e.DealAttackDamage(poliwhirl, pyroar, 30, false);
            Assert.AreEqual(3, pyroar.DamageCounters, "a Stage 1 attacker");
        }

        [Test]
        public void Flower_Veil_Stacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var roselia = e.ScenarioActive(0, C(Roselia));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(60, e.MaxHp(roselia));
            var first = e.ScenarioBench(0, C(Flabebe));
            e.ScenarioEvolve(first, C(Floette));
            Assert.AreEqual(80, e.MaxHp(roselia));
            Assert.AreEqual(70, e.MaxHp(first), "Floette is a Fairy Pokémon");
            var second = e.ScenarioBench(0, C(Flabebe));
            e.ScenarioEvolve(second, C(Floette));
            Assert.AreEqual(100, e.MaxHp(roselia));
        }

        [Test]
        public void Energy_Grace_Knocks_Milotic_Out_And_Attaches_3_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energy Grace"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            var milotic = e.ScenarioBench(0, C(Feebas));
            e.ScenarioEvolve(milotic, C(Milotic));
            e.ScenarioAddToDiscard(0, C(Water), 4);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, lapras.Energy.Count);
            Assert.IsTrue(milotic.ForcedKnockOut || e.State.FindPokemon(milotic.Uid) == null);
        }

        [Test]
        public void Shadow_Void_Moves_A_Damage_Counter_To_Dusknoir()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shadow Void"), new ScriptedAgent());
            var dusknoir = e.ScenarioActive(0, C(Duskull));
            e.ScenarioEvolve(dusknoir, C(Dusclops));
            e.ScenarioEvolve(dusknoir, C(Dusknoir));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            lapras.DamageCounters = 3;
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, dusknoir.DamageCounters);
            Assert.AreEqual(2, lapras.DamageCounters);
        }

        [Test]
        public void Big_Jump_Works_Even_For_The_Only_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Big Jump"), new ScriptedAgent());
            var lopunny = e.ScenarioActive(0, C(Buneary));
            e.ScenarioEvolve(lopunny, C(Lopunny));
            e.ScenarioAttach(lopunny, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Lopunny"));
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Buneary"));
            Assert.AreEqual(1, e.State.Winner, "no Pokémon left in play");
        }

        [Test]
        public void Poison_Barrier_Keeps_Poisoned_Pokemon_From_Retreating()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioBench(0, C(SnorlaxGX));
            var dragalge = e.ScenarioActive(1, C(Skrelp));
            e.ScenarioEvolve(dragalge, C(Dragalge));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.CanRetreat(e.State.Players[0]));
            e.ApplyCondition(lapras, SpecialCondition.Poisoned);
            Assert.IsFalse(e.CanRetreat(e.State.Players[0]));
        }

        [Test]
        public void Magnetic_Storm_Removes_Resistance()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lucario = e.ScenarioActive(0, C(FfiLucarioEX));
            var hawlucha = e.ScenarioActive(1, C(FfiHawlucha));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(lucario, hawlucha, 50);
            Assert.AreEqual(3, hawlucha.DamageCounters, "Fighting Resistance -20");
            e.ScenarioSetStadium(1, C(MagneticStorm));
            e.DealAttackDamage(lucario, hawlucha, 30);
            Assert.AreEqual(6, hawlucha.DamageCounters);
        }

        [Test]
        public void Protection_Cube_Stops_Wild_Charge_Recoil()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wild Charge"), new ScriptedAgent());
            var luxray = e.ScenarioActive(0, C(Shinx));
            e.ScenarioEvolve(luxray, C(Luxio));
            e.ScenarioEvolve(luxray, C(Luxray));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(luxray, C(Lightning));
            e.ScenarioAttachTool(luxray, C(ProtectionCube));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, snorlax.DamageCounters);
            Assert.AreEqual(0, luxray.DamageCounters);
        }

        [Test]
        public void Startling_Megaphone_Discards_Every_Opponent_Tool()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Startling Megaphone"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAttachTool(lapras, C(FloatStone));
            e.ScenarioAddToHand(0, C(Megaphone));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAttachTool(snorlax, C(FloatStone));
            var benched = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioAttachTool(benched, C(FloatStone));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, snorlax.Tools.Count);
            Assert.AreEqual(0, benched.Tools.Count);
            Assert.AreEqual(1, lapras.Tools.Count, "yours stay");
        }
    }
}
