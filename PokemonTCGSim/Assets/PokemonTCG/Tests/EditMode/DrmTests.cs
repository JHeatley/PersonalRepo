using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v82: Dragon Majesty (DRM 1-78).</summary>
    public class DrmTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Metal = "MEE-16";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string WishiwashiGX = "CEC-63";
        const string Charizard = "DRM-3";
        const string Combusken = "DRM-5";
        const string Victini = "DRM-7";
        const string Horsea = "DRM-16";
        const string KingdraGX = "DRM-18";
        const string Gyarados = "DRM-20";
        const string Feraligatr = "DRM-24";
        const string Phione = "DRM-30";
        const string Wishiwashi = "DRM-31";
        const string Hydreigon = "DRM-33";
        const string Dratini = "DRM-35";
        const string Dragonair = "DRM-36";
        const string DragoniteGX = "DRM-37";
        const string Flygon = "DRM-39";
        const string Bagon = "DRM-42";
        const string SalamenceGX = "DRM-44";
        const string WhiteKyuremGX = "DRM-48";
        const string Kangaskhan = "DRM-55";
        const string Swablu = "DRM-57";
        const string FieryFlint = "DRM-60";
        const string Lance = "DRM-61";
        const string Wela = "DRM-63";
        const string Zinnia = "DRM-64";
        const string UltraNecrozma = "DRM-78";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "DRM" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(56, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Dragon Majesty", C(Charizard).PrintSetName);
            Assert.AreEqual("HIF-SV6", C("DRM-1").BaseKey, "Charmander was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("HIF-52", C("DRM-58").BaseKey, "so was Blaine's Last Stand");
            Assert.AreEqual("HIF-SV37", C("DRM-40a").BaseKey);
            Assert.AreEqual("HIF-SV51", C("DRM-65").BaseKey, "the full art Reshiram-GX");
            Assert.AreEqual(DragoniteGX, C("DRM-67").BaseKey);
            Assert.AreEqual("Ultra Necrozma-GX", C(UltraNecrozma).Name);
            Assert.IsTrue(C(Victini).IsPrismStar);
            Assert.IsTrue(C(Lance).IsPrismStar);
            Assert.AreEqual(0, C("DRM-15").Attacks[0].Damage, "Splatter's 20 goes to the Pokémon you choose");
            Assert.AreEqual(20, C("DRM-21").Attacks[0].Damage, "Aqua Bullet's printed 20 hits the Active Pokémon too (your ruling, v83)");
        }

        [Test]
        public void Resolute_Flame_Adds_30_For_Each_Opposing_Pokemon_GX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var charizard = e.ScenarioActive(0, C(Charizard));
            var kingdra = e.ScenarioActive(1, C(KingdraGX));
            e.ScenarioBench(1, C(SalamenceGX));
            e.ScenarioBench(1, C(WhiteKyuremGX));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(charizard, kingdra, 130);
            Assert.AreEqual(22, kingdra.DamageCounters, "130 + 3 x 30 (the Active Pokémon-GX counts too)");
        }

        [Test]
        public void Infinity_Counts_Basic_Energy_In_The_Discard_Then_Shuffles_It_In()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Infinity"), new ScriptedAgent());
            var victini = e.ScenarioActive(0, C(Victini));
            e.ScenarioAttach(victini, C(Fire));
            e.ScenarioAttach(victini, C(Fire));
            e.ScenarioAddToDiscard(0, C(Water), 3);
            e.ScenarioAddToDiscard(0, C(FieryFlint), 1);
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(6, theirs.DamageCounters, "20 x 3");
            Assert.AreEqual(1, e.State.Players[0].Discard.Count, "only the Item stays");
            Assert.AreEqual(deck + 3, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Downpour_Feeds_Riptide()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Downpour", "Use Downpour", "Attack: Riptide"), new ScriptedAgent());
            var feraligatr = e.ScenarioActive(0, C(Feraligatr));
            e.ScenarioAttach(feraligatr, C(Water));
            e.ScenarioAttach(feraligatr, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count);
            Step(e);
            Assert.AreEqual(5, theirs.DamageCounters, "10 + 2 x 20");
            Assert.AreEqual(0, e.State.Players[0].Discard.Count, "the Water Energy cards were shuffled back");
        }

        [Test]
        public void Murmurs_Of_The_Sea_Keeps_Water_Pokemon_From_Being_Confused()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var horsea = e.ScenarioActive(0, C(Horsea));
            horsea.Conditions = SpecialCondition.Confused | SpecialCondition.Poisoned;
            e.ScenarioBench(0, C(Phione));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            e.ResolveKnockouts();
            Assert.IsFalse(horsea.Has(SpecialCondition.Confused), "it recovers");
            Assert.IsTrue(horsea.Has(SpecialCondition.Poisoned), "other conditions stay");
            Assert.IsFalse(e.ApplyCondition(horsea, SpecialCondition.Confused));
        }

        [Test]
        public void Wela_Volcano_Park_Keeps_Both_Players_Pokemon_Burned()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Snubbull));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.BurnRecoveryBlocked(mine));
            e.ScenarioSetStadium(0, C(Wela));
            Assert.IsTrue(e.BurnRecoveryBlocked(mine), "its owner's Pokémon too");
            Assert.IsTrue(e.BurnRecoveryBlocked(theirs));
        }

        [Test]
        public void Meet_Up_Gives_Wishiwashi_GX_HP_And_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gx = e.ScenarioActive(0, C(WishiwashiGX));
            e.ScenarioBench(0, C(Wishiwashi));
            e.ScenarioBench(0, C(Wishiwashi));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(C(WishiwashiGX).Hp + 40, e.MaxHp(gx), "each Wishiwashi adds 20 HP");
            e.DealAttackDamage(gx, theirs, 30);
            Assert.AreEqual(7, theirs.DamageCounters, "30 + 2 x 20");
        }

        [Test]
        public void Dragon_Guard_Stops_Attack_Effects_On_Dragon_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Sing"));
            var dragonair = e.ScenarioActive(0, C(Dragonair));
            e.ScenarioBench(0, C(Flygon));
            var swablu = e.ScenarioActive(1, C(Swablu));
            e.ScenarioAttach(swablu, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsFalse(dragonair.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Dragon_Lift_Frees_The_Retreat_Of_Pokemon_That_Arent_GX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dragonair = e.ScenarioActive(0, C(Dragonair));
            var kyurem = e.ScenarioBench(0, C(WhiteKyuremGX));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(dragonair));
            e.ScenarioBench(0, C(SalamenceGX));
            Assert.AreEqual(0, e.RetreatCostOf(dragonair));
            Assert.AreEqual(C(WhiteKyuremGX).RetreatCost, e.RetreatCostOf(kyurem));
        }

        [Test]
        public void Weed_Out_Keeps_3_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Weed Out"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hydreigon));
            for (int i = 0; i < 5; i++) e.ScenarioBench(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Bench.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Snubbull"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Hydreigon));
            for (int i = 0; i < 3; i++) e2.ScenarioBench(0, C(Snubbull));
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.Contains("Weed Out")), "with 3 it would do nothing");
        }

        [Test]
        public void Dragonporter_GX_Benches_3_Dragon_Pokemon_From_The_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragonporter-GX"), new ScriptedAgent());
            var dragonite = e.ScenarioActive(0, C(DragoniteGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(dragonite, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Dratini), 1);
            e.ScenarioAddToDiscard(0, C(Dragonair), 1);
            e.ScenarioAddToDiscard(0, C(SalamenceGX), 1);
            e.ScenarioAddToDiscard(0, C(Bagon), 1);
            e.ScenarioAddToDiscard(0, C(Snubbull), 1);
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(3, me.Bench.Count);
            Assert.IsTrue(me.Bench.All(m => m.Def.PokemonType == EnergyType.Dragon));
            Assert.IsTrue(me.GxUsed);
        }

        [Test]
        public void Lance_And_Zinnia_Need_A_Knock_Out_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lance", "Dragonite-GX|Dratini"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAddToHand(0, C(Lance));
            e.ScenarioAddToDeck(0, C(DragoniteGX), 1);
            e.ScenarioAddToDeck(0, C(Dratini), 1);
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Lance")));
            e.State.Players[0].LastKnockedOutTurn = 2;
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Bench.Count, "Evolution cards can be put onto the Bench too");
            Assert.AreEqual(1, me.LostZone.Count(c => c.Def.Name == "Lance ◇"), "a Prism Star card goes to the Lost Zone");

            var e2 = Scenario(new ScriptedAgent().Then("Play Zinnia"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Snubbull));
            var dratini = e2.ScenarioBench(0, C(Dratini));
            e2.ScenarioAddToHand(0, C(Zinnia));
            e2.ScenarioAddToHand(0, C(Water));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            e2.State.Players[0].LastKnockedOutTurn = 2;
            Step(e2);
            Assert.AreEqual(2, dratini.Energy.Count);
        }

        [Test]
        public void Fiery_Flint_Costs_2_Other_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Fiery Flint", "Fire Energy|Fire Energy|Fire Energy|Fire Energy") /* the 2 other cards are the only choice: not asked */, new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAddToHand(0, C(FieryFlint));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToDeck(0, C(Fire), 5);
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Fiery Flint")), "only 1 other card");
            e.ScenarioAddToHand(0, C(Psychic));
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(4, me.Hand.Count(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(2, me.Discard.Count(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Sky_Scorching_Light_Needs_6_Or_Fewer_Prize_Cards_Left_In_All()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sky-Scorching Light-GX"), new ScriptedAgent());
            var necrozma = e.ScenarioActive(0, C(UltraNecrozma));
            e.ScenarioAttach(necrozma, C(Psychic));
            e.ScenarioAttach(necrozma, C(Metal));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Sky-Scorching Light-GX")));
            e.State.Players[0].Prizes.RemoveRange(0, 3);
            e.State.Players[1].Prizes.RemoveRange(0, 3);
            Step(e);
            Assert.AreEqual(6, theirs.DamageCounters);
            Assert.AreEqual(6, benched.DamageCounters);
        }

        [Test]
        public void Commotion_Damages_Your_Own_Bench_And_Natural_Cure_Clears_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Headbutt"));
            e.ScenarioActive(0, C(Gyarados));
            var a = e.ScenarioBench(0, C(Snubbull));
            var b = e.ScenarioBench(0, C(Mewtwo));
            var kangaskhan = e.ScenarioActive(1, C(Kangaskhan));
            e.ScenarioAttach(kangaskhan, C(Psychic));
            e.ScenarioAttach(kangaskhan, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, a.DamageCounters);
            Assert.AreEqual(2, b.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Fire Energy to Combusken"), new ScriptedAgent());
            var combusken = e2.ScenarioActive(0, C(Combusken));
            combusken.Conditions = SpecialCondition.Burned | SpecialCondition.Paralyzed;
            e2.ScenarioAddToHand(0, C(Fire));
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(SpecialCondition.None, combusken.Conditions);
        }
    }
}
