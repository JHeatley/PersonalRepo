using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v88: Burning Shadows (BUS 1-169).</summary>
    public class BusTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Turtwig = "UPR-7";
        const string Celesteela = "UPR-144";
        const string Oddish = "BUS-4";
        const string Gloom = "BUS-5";
        const string Vileplume = "BUS-6";
        const string Araquanid = "BUS-15";
        const string Charmander = "BUS-18";
        const string Salazzle = "BUS-25";
        const string Kingdra = "BUS-31";
        const string Gyarados = "BUS-33";
        const string Azumarill = "BUS-35";
        const string Eelektrik = "BUS-45";
        const string Slowking = "BUS-48";
        const string Dusclops = "BUS-52";
        const string Dusknoir = "BUS-53";
        const string Palossand = "BUS-62";
        const string Necrozma = "BUS-63";
        const string Lunatone = "BUS-68";
        const string Solrock = "BUS-69";
        const string Riolu = "BUS-70";
        const string Lucario = "BUS-71";
        const string MarshadowGX = "BUS-80";
        const string AlolanMuk = "BUS-84";
        const string Weavile = "BUS-86";
        const string Meowth = "BUS-101";
        const string Porygon2 = "BUS-104";
        const string PorygonZ = "BUS-105";
        const string Dumbbells = "BUS-113";
        const string Kiawe = "BUS-116";
        const string MountLanakila = "BUS-118";
        const string PoTown = "BUS-121";
        const string WeaknessPolicy = "BUS-126";
        const string Wicke = "BUS-127";
        const string ChoiceBand = "BUS-162";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "BUS" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(112, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Burning Shadows", C(Vileplume).PrintSetName);
            Assert.AreEqual("HIF-SV48", C("BUS-129").BaseKey, "Golisopod-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("HIF-SV91", C("BUS-149").BaseKey, "so was Tapu Bulu-GX");
            Assert.AreEqual("BUS-136", C("BUS-155").BaseKey, "rainbow Lycanroc-GX");
            Assert.AreEqual("SUM-Y", C("BUS-169").BaseKey, "the gold Fairy Energy is the Sun & Moon Fairy Energy");
            Assert.AreEqual("BUS-18", C("BUS-18a").BaseKey);
            Assert.AreEqual(EnergyType.Water, C("BUS-28").PokemonType, "this Alolan Ninetales is a Water Pokémon");
            Assert.AreEqual(0, C(Kingdra).Attacks[0].Damage, "Brine's 90 is only in the text");
            Assert.AreEqual(0, C("BUS-27").Attacks[0].Cost.Length, "Powder Snow costs nothing");
        }

        [Test]
        public void Disgusting_Pollen_Stops_Only_Basic_Pokemon_From_Attacking()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snubbull = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttach(snubbull, C(Psychic));
            e.ScenarioAttach(snubbull, C(Psychic));
            e.ScenarioActive(1, C(Vileplume));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack")));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gloom = e2.ScenarioActive(0, C(Gloom));
            e2.ScenarioAttach(gloom, C(Grass));
            e2.ScenarioActive(1, C(Vileplume));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e2.GetMainActions(0).Any(o => o.Label.Contains("Stinky Scent")), "Gloom is a Stage 1 Pokémon");
        }

        [Test]
        public void Bubble_Net_Stops_Energy_From_The_Hand_On_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bubble Net"), new ScriptedAgent());
            var araquanid = e.ScenarioActive(0, C(Araquanid));
            e.ScenarioAttach(araquanid, C(Grass));
            e.ScenarioAttach(araquanid, C(Grass));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            var labels = e.GetMainActions(1).Select(o => o.Label).ToList();
            Assert.IsFalse(labels.Any(l => l.StartsWith("Attach") && l.Contains("to Mewtwo")));
            Assert.IsTrue(labels.Any(l => l.StartsWith("Attach") && l.Contains("to Snubbull")));
        }

        [Test]
        public void Diabolical_Claws_Counts_Prize_Cards_Taken()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Diabolical Claws"), new ScriptedAgent());
            var salazzle = e.ScenarioActive(0, C(Salazzle));
            e.ScenarioAttach(salazzle, C(Fire));
            e.ScenarioAttach(salazzle, C(Fire));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].Prizes.RemoveAt(0);
            e.State.Players[0].Prizes.RemoveAt(0);
            Step(e);
            Assert.AreEqual(10, target.DamageCounters, "2 Prize cards taken: 100");
        }

        [Test]
        public void Brine_Only_Hits_A_Damaged_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Brine"), new ScriptedAgent());
            var kingdra = e.ScenarioActive(0, C(Kingdra));
            e.ScenarioAttach(kingdra, C(Water));
            var active = e.ScenarioActive(1, C(Mewtwo));
            var hurt = e.ScenarioBench(1, C(Celesteela));
            hurt.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, active.DamageCounters);
            Assert.AreEqual(10, hurt.DamageCounters);
        }

        [Test]
        public void Splash_Burn_Hits_Your_Own_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Splash Burn"), new ScriptedAgent());
            var gyarados = e.ScenarioActive(0, C(Gyarados));
            e.ScenarioAttach(gyarados, C(Water));
            e.ScenarioAttach(gyarados, C(Water));
            e.ScenarioAttach(gyarados, C(Psychic));
            e.ScenarioAttach(gyarados, C(Psychic));
            var mine = e.ScenarioBench(0, C(Turtwig));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, target.DamageCounters);
            Assert.AreEqual(3, mine.DamageCounters);
        }

        [Test]
        public void Thick_Fat_Takes_30_Off_Fire_And_Water_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var charmander = e.ScenarioActive(0, C(Charmander));
            var snubbull = e.ScenarioBench(0, C(Snubbull));
            var azumarill = e.ScenarioActive(1, C(Azumarill));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(charmander, azumarill, 50);
            Assert.AreEqual(2, azumarill.DamageCounters);
            e.DealAttackDamage(snubbull, azumarill, 50);
            Assert.AreEqual(7, azumarill.DamageCounters, "Snubbull isn't a Fire or Water Pokémon");
        }

        [Test]
        public void Small_Appetite_Counts_The_Dumbbells_In_The_Maximum_HP()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Small Appetite"), new ScriptedAgent());
                var eelektrik = e.ScenarioActive(0, C(Eelektrik));
                e.ScenarioAttach(eelektrik, C(Lightning));
                e.ScenarioAttach(eelektrik, C(Psychic));
                var gloom = e.ScenarioActive(1, C(Gloom));
                if (pass == 1) e.ScenarioAttachTool(gloom, C(Dumbbells));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 5 : 0, gloom.DamageCounters, "80 HP, or 120 with Bodybuilding Dumbbells (Compendium)");
            }
        }

        [Test]
        public void Unarmed_Costs_Nothing_With_An_Empty_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Slowking));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Unarmed")));
            e.ScenarioAddToHand(0, C(Grass));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Unarmed")));
        }

        [Test]
        public void Night_Roam_Puts_A_Counter_On_Every_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Night Roam"), new ScriptedAgent());
            var dusclops = e.ScenarioActive(0, C(Dusclops));
            e.ScenarioAttach(dusclops, C(Psychic));
            var mine = e.ScenarioBench(0, C(Turtwig));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, dusclops.DamageCounters);
            Assert.AreEqual(1, mine.DamageCounters);
            Assert.AreEqual(1, theirs.DamageCounters);
            Assert.AreEqual(1, benched.DamageCounters);
        }

        [Test]
        public void Dark_Invitation_Benches_A_Basic_From_The_Opponents_Hand_With_3_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dark Invitation"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dusknoir));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(1, opp.Bench.Count);
            Assert.AreEqual("Snubbull", opp.Bench[0].Name);
            Assert.AreEqual(3, opp.Bench[0].DamageCounters);
            Assert.AreEqual(1, opp.Hand.Count);
        }

        [Test]
        public void Dust_Storm_Discards_Tools_And_Energy_But_Not_The_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dust Storm"), new ScriptedAgent());
            var palossand = e.ScenarioActive(0, C(Palossand));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(palossand, C(Psychic));
            e.ScenarioAttachTool(palossand, C(WeaknessPolicy));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioAttach(target, C(Psychic));
            e.ScenarioAttachTool(target, C(ChoiceBand));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(palossand, e.State.Players[0].Active, "your ruling: the Pokémon stay");
            Assert.AreEqual(target, e.State.Players[1].Active);
            Assert.AreEqual(0, palossand.Energy.Count + palossand.Tools.Count);
            Assert.AreEqual(0, target.Energy.Count + target.Tools.Count);
            Assert.AreEqual(10, target.DamageCounters);
        }

        [Test]
        public void Lights_End_Stops_Colorless_Attackers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var meowth = e.ScenarioActive(0, C(Meowth));
            var snubbull = e.ScenarioBench(0, C(Snubbull));
            var necrozma = e.ScenarioActive(1, C(Necrozma));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(meowth, necrozma, 50);
            Assert.AreEqual(0, necrozma.DamageCounters);
            e.DealAttackDamage(snubbull, necrozma, 30);
            Assert.AreEqual(3, necrozma.DamageCounters);
        }

        [Test]
        public void Prismatic_Burst_Discards_Psychic_Energy_For_60_Each()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Prismatic Burst"), new ScriptedAgent());
            var necrozma = e.ScenarioActive(0, C(Necrozma));
            e.ScenarioAttach(necrozma, C(Psychic));
            e.ScenarioAttach(necrozma, C(Psychic));
            e.ScenarioAttach(necrozma, C(Grass));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, target.DamageCounters, "10 + 2 x 60");
            Assert.AreEqual("Grass Energy", necrozma.Energy.Single().Def.Name);
        }

        [Test]
        public void Rule_Of_Evil_Hits_Every_Pokemon_With_An_Ability()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rule of Evil"), new ScriptedAgent());
            var weavile = e.ScenarioActive(0, C(Weavile));
            e.ScenarioAttach(weavile, C(Psychic));
            var lunatone = e.ScenarioBench(0, C(Lunatone));
            var mewtwo = e.ScenarioActive(1, C(Mewtwo));
            var vileplume = e.ScenarioBench(1, C(Vileplume));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, lunatone.DamageCounters, "your own Pokémon too");
            Assert.AreEqual(6, vileplume.DamageCounters);
            Assert.AreEqual(0, mewtwo.DamageCounters);
            Assert.AreEqual(0, weavile.DamageCounters);
        }

        [Test]
        public void Stance_Protects_Lucario_Even_After_It_Moves_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var riolu = e.ScenarioActive(0, C(Riolu));
            var other = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAddToHand(0, C(Lucario));
            var attacker = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Lucario", riolu.Name);
            e.SwitchActive(0, other);
            NextTurn(e);
            e.DealAttackDamage(attacker, riolu, 30);
            Assert.AreEqual(0, riolu.DamageCounters, "Compendium: Stance stays on the Benched Lucario");
        }

        [Test]
        public void Heal_Block_Needs_Solrock_And_Stops_All_Healing()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Lunatone));
            var hurt = e.ScenarioBench(0, C(Turtwig));
            hurt.DamageCounters = 3;
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            theirs.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            e.Heal(hurt, 10);
            Assert.AreEqual(2, hurt.DamageCounters, "no Solrock yet");
            e.ScenarioBench(0, C(Solrock));
            e.Heal(hurt, 10);
            e.Heal(theirs, 10);
            Assert.AreEqual(2, hurt.DamageCounters);
            Assert.AreEqual(3, theirs.DamageCounters, "the opponent's Pokémon too");
        }

        [Test]
        public void Shadow_Hunt_Uses_Attacks_Of_Basic_Pokemon_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var marshadow = e.ScenarioActive(0, C(MarshadowGX));
            e.ScenarioAttach(marshadow, C(Psychic));
            e.ScenarioAttach(marshadow, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Snubbull), 1);
            e.ScenarioAddToDiscard(0, C(Gloom), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Any(l => l.Contains("Play Rough")));
            Assert.IsFalse(labels.Any(l => l.Contains("Stinky Scent")), "Gloom isn't a Basic Pokémon");
        }

        [Test]
        public void Tri_Hazard_GX_Brings_Up_A_Benched_Pokemon_With_3_Conditions()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tri Hazard-GX"), new ScriptedAgent());
            e.ScenarioActive(0, C(AlolanMuk));
            e.ScenarioActive(1, C(Snubbull));
            var benched = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(benched, e.State.Players[1].Active);
            Assert.IsTrue(benched.Has(SpecialCondition.Burned));
            Assert.IsTrue(benched.Has(SpecialCondition.Paralyzed));
            Assert.IsTrue(benched.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(e.State.Players[0].GxUsed);
        }

        [Test]
        public void Initialize_Devolves_The_Opponents_Evolved_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var porygon2 = e.ScenarioActive(0, C(Porygon2));
            e.ScenarioAddToHand(0, C(PorygonZ));
            var target = e.ScenarioActive(1, C(Oddish));
            e.ScenarioEvolve(target, C(Gloom));
            var basic = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Porygon-Z", porygon2.Name);
            Assert.AreEqual("Oddish", target.Name);
            Assert.AreEqual("Snubbull", basic.Name);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Gloom"));
        }

        [Test]
        public void Kiawe_Attaches_4_Fire_Energy_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Kiawe", "Fire Energy|Fire Energy|Fire Energy|Fire Energy"), new ScriptedAgent());
            var charmander = e.ScenarioActive(0, C(Charmander));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(Kiawe));
            e.ScenarioAddToDeck(0, C(Fire), 5);
            e.ScenarioSetTurn(0, 3);
            bool over = e.MainPhaseStep();
            Assert.IsTrue(over, "Kiawe: your turn ends");
            Assert.AreEqual(4, charmander.Energy.Count);
        }

        [Test]
        public void Mount_Lanakila_Adds_1_To_Basic_Pokemon_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var basic = e.ScenarioActive(0, C(Snubbull));
            var stage1 = e.ScenarioBench(0, C(Gloom));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            int b = e.RetreatCostOf(basic), s = e.RetreatCostOf(stage1), t = e.RetreatCostOf(theirs);
            e.ScenarioSetStadium(1, C(MountLanakila));
            Assert.AreEqual(b + 1, e.RetreatCostOf(basic));
            Assert.AreEqual(t + 1, e.RetreatCostOf(theirs), "both players' Basic Pokémon");
            Assert.AreEqual(s, e.RetreatCostOf(stage1));
        }

        [Test]
        public void Po_Town_Puts_3_Counters_On_A_Pokemon_Evolved_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var oddish = e.ScenarioActive(0, C(Oddish));
            e.ScenarioAddToHand(0, C(Gloom));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetStadium(1, C(PoTown));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Gloom", oddish.Name);
            Assert.AreEqual(3, oddish.DamageCounters);
        }

        [Test]
        public void Weakness_Policy_Removes_Weakness()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var attacker = e.ScenarioActive(0, C(Mewtwo));
                var target = e.ScenarioActive(1, C(Mewtwo));
                if (pass == 1) e.ScenarioAttachTool(target, C(WeaknessPolicy));
                e.ScenarioSetTurn(0, 3);
                e.DealAttackDamage(attacker, target, 30);
                Assert.AreEqual(pass == 0 ? 6 : 3, target.DamageCounters);
            }
        }

        [Test]
        public void Choice_Band_Adds_30_Only_Against_Pokemon_GX_And_EX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snubbull = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttachTool(snubbull, C(ChoiceBand));
            var gx = e.ScenarioActive(1, C(Celesteela));
            var other = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snubbull, gx, 30);
            Assert.AreEqual(6, gx.DamageCounters);
            e.SwitchActive(1, other);
            e.DealAttackDamage(snubbull, other, 30);
            Assert.AreEqual(3, other.DamageCounters);
        }

        [Test]
        public void Wicke_Gives_Each_Player_A_New_Hand_Of_The_Same_Size()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Wicke"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(Wicke));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Grass));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.AreEqual(5, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Bodybuilding_Dumbbells_Only_Help_Stage_1_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gloom = e.ScenarioActive(0, C(Gloom));
            e.ScenarioAttachTool(gloom, C(Dumbbells));
            var snubbull = e.ScenarioBench(0, C(Snubbull));
            e.ScenarioAttachTool(snubbull, C(Dumbbells));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(120, e.MaxHp(gloom));
            Assert.AreEqual(70, e.MaxHp(snubbull));
        }
    }
}
