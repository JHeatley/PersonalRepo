using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v84: Forbidden Light (FLI 1-146).</summary>
    public class FliTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Pupitar = "CES-75";
        const string Celesteela = "HIF-SV32";
        const string ZygardeGX = "HIF-SV65";
        const string Hau = "CES-132";
        const string Delphox = "FLI-17";
        const string Palkia = "FLI-20";
        const string Clawitzer = "FLI-26";
        const string Magnezone = "FLI-36";
        const string Rotom = "FLI-40";
        const string Mesprit = "FLI-42";
        const string Azelf = "FLI-43";
        const string Aegislash = "FLI-49";
        const string Infernape = "FLI-59";
        const string Tyrantrum = "FLI-69";
        const string Zygarde = "FLI-71";
        const string Diancie = "FLI-74";
        const string Yveltal = "FLI-79";
        const string Dialga = "FLI-82";
        const string Sylveon = "FLI-87";
        const string Xerneas = "FLI-90";
        const string Goomy = "FLI-91";
        const string Goodra = "FLI-94";
        const string Arceus = "FLI-96";
        const string Bonnie = "FLI-103";
        const string LysandrePrism = "FLI-110";
        const string MetalFryingPan = "FLI-112";
        const string BeastEnergy = "FLI-117";
        const string UnitFDY = "FLI-118";
        const string SkyPillar = "CES-144";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "FLI" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(101, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Forbidden Light", C(Palkia).PrintSetName);
            Assert.AreEqual("HIF-SV56", C("FLI-24").BaseKey, "Greninja-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("DRM-78", C("FLI-95").BaseKey, "Ultra Necrozma-GX is Dragon Majesty 78's card");
            Assert.AreEqual("HIF-SV64", C("FLI-122").BaseKey, "Lucario-GX has no regular print in Forbidden Light");
            Assert.AreEqual("DRI-164", C("FLI-143").BaseKey, "Energy Recycler was already in the pool");
            Assert.AreEqual("FLI-20", C("FLI-119").BaseKey);
            Assert.AreEqual("FLI-2", C("FLI-2a").BaseKey);
            Assert.AreEqual("Lysandre ◇", C(LysandrePrism).Name, "not the ordinary Lysandre");
            Assert.IsTrue(C(LysandrePrism).IsPrismStar);
            Assert.IsTrue(C(BeastEnergy).IsPrismStar);
            Assert.AreEqual(0, C(Clawitzer).Attacks[1].Damage, "Sharpshooting's 40 is only in the text");
            Assert.AreEqual(0, C("FLI-62").Attacks[0].Damage, "so is Quick Dive's 50");
            Assert.AreEqual(20, C(Xerneas).Attacks[0].Damage, "Overrun's printed 20 hits the Active Pokémon too (your ruling, v83)");
        }

        [Test]
        public void Roto_Motor_Sets_The_Cost_To_Nothing_So_Sticky_Membrane_Cant_Add_To_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var rotom = e.ScenarioActive(0, C(Rotom));
            e.ScenarioActive(1, C(Goomy));
            e.ScenarioSetTurn(0, 3);
            var slice = rotom.Def.Attacks[0];
            Assert.AreEqual(4, e.AttackCost(rotom, slice).Count, "PPP + C from Sticky Membrane");
            e.ScenarioAddToDiscard(0, C(MetalFryingPan), 9);
            Assert.AreEqual(0, e.AttackCost(rotom, slice).Count, "ignored, and Sticky Membrane doesn't add to it (Compendium)");
        }

        [Test]
        public void Ticking_Knock_Out_Knocks_Out_The_Defending_Pokemon_When_Damaged_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ticking Knock Out"), new ScriptedAgent());
            var aegislash = e.ScenarioActive(0, C(Aegislash));
            e.ScenarioAttach(aegislash, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            Assert.AreEqual(target, e.State.Players[1].Active);
            e.DealAttackDamage(aegislash, target, 10);
            e.ResolveKnockouts();
            Assert.IsNull(e.State.FindPokemon(target.Uid), "Knocked Out by the 10 damage");
        }

        [Test]
        public void Bonnie_Lets_Zygarde_GX_Use_Its_GX_Attack_Again()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bonnie"), new ScriptedAgent());
            var zygarde = e.ScenarioActive(0, C(ZygardeGX));
            e.ScenarioAttach(zygarde, C(Fighting));
            e.ScenarioAttach(zygarde, C(Fighting));
            e.ScenarioAttach(zygarde, C(Psychic));
            e.ScenarioAttach(zygarde, C(Psychic));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetStadium(1, C(SkyPillar));
            e.ScenarioAddToHand(0, C(Bonnie));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].GxUsed = true;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Verdict-GX")), "the GX attack was used");
            Step(e);
            Assert.IsNull(e.State.Stadium, "Bonnie discards the Stadium");
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Verdict-GX")));
        }

        [Test]
        public void Silent_Waves_Takes_Away_Resistance_With_Azelf_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pupitar = e.ScenarioActive(0, C(Pupitar));
            e.ScenarioBench(0, C(Mesprit));
            var target = e.ScenarioActive(1, C(Aegislash));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(pupitar, target, 50);
            Assert.AreEqual(3, target.DamageCounters, "Resistance -20 without Azelf");
            e.ScenarioBench(0, C(Azelf));
            e.DealAttackDamage(pupitar, target, 50);
            Assert.AreEqual(8, target.DamageCounters, "no Resistance with Azelf in play");
        }

        [Test]
        public void Earthen_Aura_Ignores_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Peace Maker"), new ScriptedAgent());
            var zygarde = e.ScenarioActive(0, C(Zygarde));
            e.ScenarioAttach(zygarde, C(Fighting));
            var target = e.ScenarioActive(1, C(Magnezone));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters, "30, not doubled by Fighting Weakness");
        }

        [Test]
        public void Princess_s_Cheers_Adds_20_To_Fighting_Attacks_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pupitar = e.ScenarioActive(0, C(Pupitar));
            var diancie = e.ScenarioBench(0, C(Diancie));
            var target = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(pupitar, target, 20);
            Assert.AreEqual(4, target.DamageCounters, "20 + 20");
            e.SwitchActive(0, diancie);
            target.DamageCounters = 0;
            e.DealAttackDamage(pupitar, target, 20);
            Assert.AreEqual(2, target.DamageCounters, "only while Diancie ◇ is on the Bench");
        }

        [Test]
        public void Tyrannical_Heart_Works_While_You_Dont_Have_More_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tyrantrum = e.ScenarioActive(0, C(Tyrantrum));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tyrantrum, theirs, 30);
            Assert.AreEqual(9, theirs.DamageCounters, "30 + 60");
            e.DealAttackDamage(theirs, tyrantrum, 50);
            Assert.AreEqual(2, tyrantrum.DamageCounters, "50 - 30");
            e.ScenarioBench(0, C(Snubbull));
            e.DealAttackDamage(theirs, tyrantrum, 50);
            Assert.AreEqual(7, tyrantrum.DamageCounters, "with more Pokémon in play, no reduction");
        }

        [Test]
        public void Doom_Count_GX_Knocks_Out_A_Pokemon_With_Exactly_4_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Doom Count-GX"), new ScriptedAgent());
            var yveltal = e.ScenarioActive(0, C(Yveltal));
            e.ScenarioAttach(yveltal, C(Darkness));
            var target = e.ScenarioActive(1, C(Mewtwo));
            target.DamageCounters = 4;
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid));
            Assert.IsTrue(e.State.Players[0].GxUsed);
        }

        [Test]
        public void Timeless_GX_Gives_Another_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Timeless-GX"), new ScriptedAgent());
            var dialga = e.ScenarioActive(0, C(Dialga));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(dialga, C(Metal));
            e.ScenarioAttach(dialga, C(Psychic));
            e.ScenarioAttach(dialga, C(Psychic));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.ExtraTurnFor);
            Assert.IsTrue(e.State.SkipCheckup);
        }

        [Test]
        public void Sanctuary_GX_Moves_All_Your_Damage_Counters_To_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sanctuary-GX"), new ScriptedAgent());
            var xerneas = e.ScenarioActive(0, C(Xerneas));
            xerneas.DamageCounters = 3;
            var bench = e.ScenarioBench(0, C(Snubbull));
            bench.DamageCounters = 2;
            for (int i = 0; i < 3; i++) e.ScenarioAttach(xerneas, C(Psychic));
            e.ScenarioAttach(xerneas, C("SUM-Y"));
            e.ScenarioAttach(xerneas, C("SUM-Y"));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, xerneas.DamageCounters);
            Assert.AreEqual(0, bench.DamageCounters);
            Assert.AreEqual(5, target.DamageCounters);
        }

        [Test]
        public void Wink_Wink_Discards_An_Opponents_Supporter_And_Uses_Its_Effect()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wink Wink", "Discard Hau"), new ScriptedAgent());
            var sylveon = e.ScenarioActive(0, C(Sylveon));
            e.ScenarioAttach(sylveon, C(Psychic));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(1, C(Hau));
            e.ScenarioSetTurn(0, 3);
            int hand = e.State.Players[0].Hand.Count;
            Step(e);
            Assert.AreEqual(hand + 3, e.State.Players[0].Hand.Count, "Hau: draw 3");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Hau"), "discarded from the opponent's hand");
        }

        [Test]
        public void Hydration_Heals_And_Feeds_Soaking_Horn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy", "Attack: Soaking Horn"), new ScriptedAgent());
            var goodra = e.ScenarioActive(0, C(Goodra));
            goodra.DamageCounters = 5;
            e.ScenarioAttach(goodra, C("SUM-Y"));
            e.ScenarioAttach(goodra, C(Psychic));
            e.ScenarioAddToHand(0, C(Water));
            var target = e.ScenarioActive(1, C(Dialga));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, goodra.DamageCounters, "healed 20");
            Step(e);
            Assert.AreEqual(16, target.DamageCounters, "80 + 80: Goodra was healed this turn");
        }

        [Test]
        public void First_Law_Stops_Attack_Effects_But_Not_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var arceus = e.ScenarioActive(0, C(Arceus));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e.EffectsPrevented(arceus, EffectSourceKind.Attack, 1, false));
            Assert.IsFalse(e.EffectsPrevented(arceus, EffectSourceKind.Ability, 1, false), "only attacks");
            e.DealAttackDamage(theirs, arceus, 30);
            Assert.AreEqual(3, arceus.DamageCounters, "damage still happens");
        }

        [Test]
        public void Lysandre_Prism_Star_Puts_A_Card_In_The_Lost_Zone_For_Each_Fire_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lysandre"), new ScriptedAgent());
            e.ScenarioActive(0, C(Delphox));
            e.ScenarioBench(0, C(Infernape));
            e.ScenarioBench(0, C(Delphox));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToDiscard(1, C(Psychic), 4);
            e.ScenarioAddToHand(0, C(LysandrePrism));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[1].LostZone.Count, "2 Fire Pokémon (Infernape is Fighting)");
            Assert.AreEqual(2, e.State.Players[1].Discard.Count);
            Assert.IsTrue(e.State.Players[0].LostZone.Any(c => c.Def.Name == "Lysandre ◇"), "Prism Star: Lysandre ◇ goes to the Lost Zone");
        }

        [Test]
        public void Beast_Energy_Prism_Star_Gives_Ultra_Beasts_Any_Type_And_30_More()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var beast = e.ScenarioActive(0, C(Celesteela));
            e.ScenarioAttach(beast, C(BeastEnergy));
            var other = e.ScenarioBench(0, C(Snubbull));
            e.ScenarioAttach(other, C(BeastEnergy));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(EnergyMask.Provides(e.EnergyUnitsOf(beast, beast.Energy[0])[0], EnergyType.Fire));
            Assert.IsFalse(EnergyMask.Provides(e.EnergyUnitsOf(other, other.Energy[0])[0], EnergyType.Fire), "only Colorless on other Pokémon");
            e.DealAttackDamage(beast, theirs, 50);
            Assert.AreEqual(8, theirs.DamageCounters, "50 + 30");
        }

        [Test]
        public void Unit_Energy_FDY_Provides_Fighting_Darkness_And_Fairy_When_Attached()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttach(mon, C(UnitFDY));
            e.ScenarioActive(1, C(Mewtwo));
            var units = e.EnergyUnitsOf(mon, mon.Energy[0]);
            Assert.AreEqual(1, units.Count, "1 Energy at a time");
            Assert.IsTrue(EnergyMask.Provides(units[0], EnergyType.Fighting) && EnergyMask.Provides(units[0], EnergyType.Darkness) && EnergyMask.Provides(units[0], EnergyType.Fairy));
            Assert.IsFalse(EnergyMask.Provides(units[0], EnergyType.Fire));
        }

        [Test]
        public void Metal_Frying_Pan_Takes_30_Less_And_Removes_Weakness_On_Metal_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dialga = e.ScenarioActive(0, C(Dialga));
            e.ScenarioAttachTool(dialga, C(MetalFryingPan));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(1, 4);
            Assert.IsNull(e.WeaknessOf(dialga));
            e.DealAttackDamage(theirs, dialga, 50);
            Assert.AreEqual(2, dialga.DamageCounters, "50 - 30");
        }

        [Test]
        public void Flaming_Fighter_Makes_Burn_6_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Infernape));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            theirs.Conditions = SpecialCondition.Burned;
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(6, theirs.DamageCounters);
        }

        [Test]
        public void Standing_By_Makes_Next_Turns_Sharpshooting_Do_120()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Standing By", "Attack: Sharpshooting"), new ScriptedAgent());
            var clawitzer = e.ScenarioActive(0, C(Clawitzer));
            e.ScenarioAttach(clawitzer, C(Water));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, target.DamageCounters);
            NextTurn(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters, "120");
        }
    }
}
