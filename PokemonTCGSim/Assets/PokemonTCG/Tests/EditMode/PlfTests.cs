using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v110: Plasma Freeze (PLF 1-122): Team Plasma Badge, Dark Shade, Freeze Zone, Frozen City, Ghetsis, Life Dew and Rock Guard
    /// (ACE SPEC), Bright Down, Color Change, Jet Blast, Diving Draw, Attack Command and Six Feet Under.
    /// </summary>
    public class PlfTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Fighting = "MEE-14";

        const string HeatranEX = "PLF-13";
        const string Glaceon = "PLF-23";
        const string MrMime = "PLF-47";
        const string DeoxysEX = "PLF-53";
        const string Yamask = "PLF-54";
        const string Cofagrigus = "PLF-56";
        const string Mankey = "PLF-59";
        const string Umbreon = "PLF-64";
        const string LatiasEX = "PLF-85";
        const string LatiosEX = "PLF-86";
        const string Eevee = "PLF-89";
        const string Kecleon = "PLF-94";
        const string TornadusEX = "PLF-98";
        const string FrozenCity = "PLF-100";
        const string Ghetsis = "PLF-101";
        const string Badge = "PLF-104";
        const string PlasmaBall = "PLF-105";
        const string PlasmaEnergy = "PLB-91";
        const string LifeDew = "PLF-107";
        const string RockGuard = "PLF-108";
        const string Empoleon = "PLF-117";
        const string Throh = "PLB-51";
        const string Onix = "PLF-61";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "PLF").ToList();
            Assert.AreEqual(105, own.Count);
            Assert.AreEqual(HeatranEX, C("PLF-109").BaseKey, "full-art Heatran-EX");
            Assert.AreEqual(PlasmaEnergy, C("PLF-106").BaseKey, "Plasma Energy");
            Assert.IsTrue(C(HeatranEX).IsTeamPlasma);
            Assert.IsTrue(C(FrozenCity).IsTeamPlasma && C(FrozenCity).IsStadium);
            Assert.IsTrue(C(LifeDew).IsAceSpec && C(LifeDew).IsTool);
            Assert.IsTrue(C(RockGuard).IsAceSpec);
            Assert.IsFalse(C(Mankey).IsTeamPlasma);
        }

        [Test]
        public void Dark_Shade_And_Team_Plasma_Badge()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var umbreon = e.ScenarioActive(0, C(Eevee));
            e.ScenarioEvolve(umbreon, C(Umbreon));
            var deoxys = e.ScenarioBench(0, C(DeoxysEX));
            var mankey = e.ScenarioBench(0, C(Mankey));
            var foe = e.ScenarioActive(1, C(DeoxysEX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(120, e.MaxHp(umbreon), "Umbreon is a Team Plasma Pokémon itself");
            Assert.AreEqual(190, e.MaxHp(deoxys));
            Assert.AreEqual(50, e.MaxHp(mankey));
            Assert.AreEqual(170, e.MaxHp(foe), "only your own Pokémon");
            Assert.IsFalse(e.IsTeamPlasma(mankey));
            e.ScenarioAttachTool(mankey, C(Badge));
            Assert.IsTrue(e.IsTeamPlasma(mankey), "Team Plasma Badge");
            Assert.AreEqual(70, e.MaxHp(mankey));
        }

        [Test]
        public void Freeze_Zone_Lowers_Team_Plasma_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var glaceon = e.ScenarioActive(0, C(Eevee));
            e.ScenarioEvolve(glaceon, C(Glaceon));
            var deoxys = e.ScenarioBench(0, C(DeoxysEX));
            var mankey = e.ScenarioBench(0, C(Mankey));
            e.ScenarioActive(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(deoxys));
            Assert.AreEqual(0, e.RetreatCostOf(glaceon));
            Assert.AreEqual(1, e.RetreatCostOf(mankey), "not Team Plasma");
        }

        [Test]
        public void Frozen_City_Hurts_Pokemon_That_Arent_Team_Plasma()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Fighting Energy"), new ScriptedAgent());
            var mankey = e.ScenarioActive(0, C(Mankey));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioActive(1, C(Mankey));
            e.ScenarioSetStadium(1, C(FrozenCity));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mankey.Energy.Count);
            Assert.AreEqual(2, mankey.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Fighting Energy"), new ScriptedAgent());
            var deoxys = e2.ScenarioActive(0, C(DeoxysEX));
            e2.ScenarioAddToHand(0, C(Fighting));
            e2.ScenarioActive(1, C(Mankey));
            e2.ScenarioSetStadium(0, C(FrozenCity));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, deoxys.Energy.Count);
            Assert.AreEqual(0, deoxys.DamageCounters, "Team Plasma");
        }

        [Test]
        public void Frozen_City_Comes_Before_Energy_Evolution()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy", "Yes"), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C("HIF-SV41"));
            e.ScenarioActive(1, C(Mankey));
            e.ScenarioAddToDeck(0, C("PLF-20"), 1);
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioSetStadium(1, C(FrozenCity));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Vaporeon", eevee.Name);
            Assert.AreEqual(2, eevee.DamageCounters, "Compendium 2014-08-28: the counters go on Eevee first (Vaporeon is Team Plasma)");
        }

        [Test]
        public void Ghetsis_Shuffles_Away_Items_And_Draws_For_Each()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ghetsis"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mankey));
            e.ScenarioActive(1, C(Mankey));
            e.ScenarioAddToHand(0, C(Ghetsis));
            e.ScenarioAddToDeck(0, C(Fighting), 5);
            e.ScenarioAddToHand(1, C(PlasmaBall));
            e.ScenarioAddToHand(1, C(PlasmaBall));
            e.ScenarioAddToHand(1, C(Mankey));
            int oppDeck = e.State.Players[1].Deck.Count;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.AreEqual(oppDeck + 2, e.State.Players[1].Deck.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "a card for each Item");
        }

        [Test]
        public void Life_Dew_Gives_Up_1_Fewer_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var throh = e.ScenarioActive(0, C(Onix));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(throh, C(Fighting));
            var latios = e.ScenarioActive(1, C(LatiosEX));
            e.ScenarioAttachTool(latios, C(LifeDew));
            e.ScenarioBench(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(throh, latios, 160, false);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(latios.Uid));
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "1 Prize card for a Pokémon-EX");
        }

        [Test]
        public void Rock_Guard_Puts_6_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var throh = e.ScenarioActive(0, C(Onix));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(throh, C(Fighting));
            var mankey = e.ScenarioActive(1, C(Mankey));
            e.ScenarioAttachTool(mankey, C(RockGuard));
            e.ScenarioBench(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, mankey.DamageCounters);
            Assert.AreEqual(6, throh.DamageCounters);
        }

        [Test]
        public void Bright_Down_Stops_Attacks_From_Pokemon_With_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var latias = e.ScenarioActive(0, C(LatiasEX));
            var mime = e.ScenarioActive(1, C(MrMime));
            var mankey = e.ScenarioBench(1, C(Mankey));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(mime, latias, 30);
            Assert.AreEqual(0, latias.DamageCounters, "Mr. Mime has an Ability");
            e.DealAttackDamage(mankey, latias, 30);
            Assert.AreEqual(3, latias.DamageCounters);
        }

        [Test]
        public void Color_Change_Copies_The_Opponents_Active_Type()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C(Kecleon));
            var benched = e.ScenarioBench(0, C(Kecleon));
            e.ScenarioActive(1, C(DeoxysEX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Psychic));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Colorless));
            Assert.IsTrue(e.IsType(benched, EnergyType.Colorless), "only as the Active Pokémon");
        }

        [Test]
        public void Jet_Blast_Adds_30_Per_Plasma_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jet Blast"), new ScriptedAgent());
            var tornadus = e.ScenarioActive(0, C(TornadusEX));
            e.ScenarioAttach(tornadus, C(PlasmaEnergy));
            e.ScenarioAttach(tornadus, C(PlasmaEnergy));
            e.ScenarioAttach(tornadus, C(Fighting));
            e.ScenarioAttach(tornadus, C(Fighting));
            var heatran = e.ScenarioActive(1, C(HeatranEX));
            e.ScenarioBench(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, heatran.DamageCounters, "60 + 2 x 30");
        }

        [Test]
        public void Diving_Draw_Discards_Then_Draws_2_And_Attack_Command_Counts_Everything()
        {
            var e = Scenario(new ScriptedAgent().Then("Diving Draw"), new ScriptedAgent());
            var empoleon = e.ScenarioActive(0, C(Empoleon));
            e.ScenarioAddToHand(0, C(Mankey));
            e.ScenarioAddToDeck(0, C(Fighting), 5);
            e.ScenarioActive(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Mankey"));
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.IsNotNull(empoleon);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Attack Command"), new ScriptedAgent());
            var emp = e2.ScenarioActive(0, C(Empoleon));
            e2.ScenarioAttach(emp, C(Water));
            e2.ScenarioBench(0, C(Mankey));
            var throh = e2.ScenarioActive(1, C(Throh));
            e2.ScenarioBench(1, C(Mankey));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(4, throh.DamageCounters, "4 Pokémon in play");
        }

        [Test]
        public void Six_Feet_Under_Knocks_Out_Cofagrigus_For_3_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Six Feet Under"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mankey));
            var cofagrigus = e.ScenarioBench(0, C(Yamask));
            e.ScenarioEvolve(cofagrigus, C(Cofagrigus));
            var throh = e.ScenarioActive(1, C(Throh));
            var mankey = e.ScenarioBench(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(cofagrigus.Uid));
            Assert.AreEqual(3, throh.DamageCounters + mankey.DamageCounters);
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "the opponent takes a Prize card");
        }
    }
}
