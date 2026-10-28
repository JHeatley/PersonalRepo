using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v98: Ancient Origins (AOR 1-100): the Ancient Traits θ Stop, θ Double and θ Max, Irritating Pollen, the Eeveelution Effects,
    /// Double Type, Renegade Pulse, Hex Maniac, Forest of Giant Plants, Faded Town, Dangerous and Flash Energy and the rest.
    /// </summary>
    public class AorTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string Garbodor = "BKP-57";
        const string FightingFuryBelt = "BKP-99";
        const string FairyGarden = "FCO-100";
        const string FloatStone = "BKT-137";
        const string TownMap = "BKT-150";
        const string RayquazaEX = "30C-CC19";

        const string Oddish = "AOR-1";
        const string Gloom = "AOR-2";
        const string Vileplume = "AOR-3";
        const string Ariados = "AOR-6";
        const string SceptileEX = "AOR-7";
        const string MSceptileEX = "AOR-8";
        const string Vespiquen = "AOR-10";
        const string VespiquenDouble = "AOR-11";
        const string Flareon = "AOR-13";
        const string Volcarona = "AOR-17";
        const string KyuremEX = "AOR-25";
        const string Jolteon = "AOR-26";
        const string AmpharosEX = "AOR-27";
        const string Unown = "AOR-30";
        const string Golett = "AOR-34";
        const string Golurk = "AOR-35";
        const string HoopaEX = "AOR-36";
        const string TyranitarEX = "AOR-42";
        const string MTyranitarEX = "AOR-43";
        const string Gardevoir = "AOR-54";
        const string GiratinaEX = "AOR-57";
        const string Eevee = "AOR-63";
        const string AceTrainer = "AOR-69";
        const string FadedTown = "AOR-73";
        const string ForestOfGiantPlants = "AOR-74";
        const string HexManiac = "AOR-75";
        const string Lysandre = "AOR-78";
        const string PaintRoller = "AOR-79";
        const string DangerousEnergy = "AOR-82";
        const string FlashEnergy = "AOR-83";
        const string Steven = "AOR-95";
        const string MRayquazaEX = "AOR-98";

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "AOR" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(81, own.Count);
            Assert.IsTrue(own.Where(d => d.Name != "Hex Maniac" && d.Name != "Forest of Giant Plants").All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(HexManiac)), "on the Expanded ban list");
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(ForestOfGiantPlants)), "on the Expanded ban list");
            Assert.AreEqual("AOR-7", C("AOR-84").BaseKey, "full art Sceptile-EX");
            Assert.AreEqual("AOR-75", C("AOR-75a").BaseKey);
            Assert.AreEqual("GEN-22", C("AOR-19").BaseKey, "Magikarp is the Generations card");
            Assert.AreEqual("BST-129", C("AOR-76").BaseKey, "Level Ball keeps its script");
            Assert.AreEqual("θ Stop", C(MSceptileEX).AncientTrait);
            Assert.AreEqual("θ Double", C(VespiquenDouble).AncientTrait);
            Assert.AreEqual("θ Max", C(MRayquazaEX).AncientTrait);
            Assert.IsTrue(C("AOR-96").IsXYMegaEvolution, "Primal Pokémon are Mega Evolution Pokémon");
        }

        // ------------------------------------------------------------------ Ancient Traits

        [Test]
        public void Theta_Stop_Keeps_The_Opponents_Abilities_Off_This_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Poisonous Nest"), new ScriptedAgent());
            e.ScenarioActive(0, C(Ariados));
            var golurk = e.ScenarioActive(1, C(Golurk));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(golurk.Has(SpecialCondition.Poisoned), "θ Stop");

            var e2 = Scenario(new ScriptedAgent().Then("Use Poisonous Nest"), new ScriptedAgent());
            var ariados = e2.ScenarioActive(0, C(Ariados));
            var lapras = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(lapras.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(ariados.Has(SpecialCondition.Poisoned), "Grass Pokémon aren't Poisoned");
        }

        [Test]
        public void Theta_Stop_Keeps_The_Pokemons_Own_Ability_On_Against_The_Opponents_Blockers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var golurk = e.ScenarioActive(0, C(Golett));
            e.ScenarioEvolve(golurk, C(Golurk));
            var hoopa = e.ScenarioBench(0, C(HoopaEX));
            var garbodor = e.ScenarioActive(1, C(Garbodor));
            e.ScenarioAttachTool(garbodor, C(FightingFuryBelt));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.AbilityBlocked(golurk, "Double Type"), "θ Stop (Compendium: Bide Barricade, 2015-09-03)");
            Assert.IsTrue(e.TypesOf(golurk).Contains(EnergyType.Fighting));
            Assert.IsTrue(e.AbilityBlocked(hoopa, "Scoundrel Ring"));
        }

        [Test]
        public void Theta_Double_Holds_2_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var two = e.ScenarioActive(0, C(VespiquenDouble));
            var one = e.ScenarioBench(0, C(Vespiquen));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.MaxTools(two));
            Assert.AreEqual(1, e.MaxTools(one));
        }

        [Test]
        public void Theta_Max_Heals_All_Damage_When_A_Pokemon_Becomes_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var rayquaza = e.ScenarioActive(0, C(RayquazaEX));
            rayquaza.DamageCounters = 9;
            var card = e.ScenarioAddToHand(0, C(MRayquazaEX));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].Hand.Remove(card);
            e.EvolvePokemon(rayquaza, card, true);
            Assert.AreEqual(0, rayquaza.DamageCounters);
            Assert.AreEqual("M Rayquaza-EX", rayquaza.Name);
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Irritating_Pollen_Stops_Item_Cards_But_Not_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var item = e.ScenarioAddToHand(0, C(TownMap));
            var tool = e.ScenarioAddToHand(0, C(FloatStone));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioBench(1, C(Vileplume));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, item), "each player can't play Item cards");
            Assert.IsTrue(e.CanPlayTrainer(0, tool), "a Pokémon Tool isn't an Item card");
        }

        [Test]
        public void Flare_Effect_And_Double_Type_Add_Types()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var flareon = e.ScenarioActive(0, C(Eevee));
            e.ScenarioEvolve(flareon, C(Flareon));
            var golurk = e.ScenarioBench(0, C(Golett));
            e.ScenarioEvolve(golurk, C(Golurk));
            var basic = e.ScenarioBench(0, C(Bounsweet));
            var theirs = e.ScenarioActive(1, C(Golett));
            e.ScenarioEvolve(theirs, C(Golurk));
            e.ScenarioSetTurn(0, 3);
            var types = e.TypesOf(golurk);
            Assert.IsTrue(types.Contains(EnergyType.Psychic) && types.Contains(EnergyType.Fighting) && types.Contains(EnergyType.Fire), "they add up (2015-08-13)");
            Assert.IsFalse(e.IsType(basic, EnergyType.Fire), "Stage 1 Pokémon only");
            Assert.IsFalse(e.IsType(theirs, EnergyType.Fire), "only your own");
        }

        [Test]
        public void Renegade_Pulse_Stops_Mega_Evolution_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var giratina = e.ScenarioActive(0, C(GiratinaEX));
            var mega = e.ScenarioActive(1, C(SceptileEX));
            e.ScenarioEvolve(mega, C(MSceptileEX));
            var plain = e.ScenarioBench(1, C(SceptileEX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(mega, giratina, 100);
            Assert.AreEqual(0, giratina.DamageCounters);
            e.DealAttackDamage(plain, giratina, 30);
            Assert.AreEqual(3, giratina.DamageCounters);
        }

        [Test]
        public void Scoundrel_Ring_Finds_3_Pokemon_EX_When_Hoopa_Is_Benched()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Hoopa-EX", "Yes", "Sceptile-EX|Kyurem-EX|Ampharos-EX"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(HoopaEX));
            e.ScenarioAddToDeck(0, C(SceptileEX), 1);
            e.ScenarioAddToDeck(0, C(KyuremEX), 1);
            e.ScenarioAddToDeck(0, C(AmpharosEX), 1);
            e.ScenarioAddToDeck(0, C(HoopaEX), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(3, hand.Count(c => c.Def.IsPokemonEX));
            Assert.IsFalse(hand.Any(c => c.Def.Name == "Hoopa-EX"), "not Hoopa-EX");
        }

        [Test]
        public void Bright_Heal_Heals_20_From_Each_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Bright Heal"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            lapras.DamageCounters = 5;
            var gardevoir = e.ScenarioBench(0, C(Gardevoir));
            gardevoir.DamageCounters = 1;
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, lapras.DamageCounters);
            Assert.AreEqual(0, gardevoir.DamageCounters);
        }

        [Test]
        public void Farewell_Letter_Draws_And_Discards_Unown_Without_A_Knock_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Farewell Letter"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var unown = e.ScenarioBench(0, C(Unown));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
            Assert.IsFalse(e.State.Players[0].Bench.Contains(unown));
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Unown"));
            Assert.AreEqual(6, e.State.Players[1].Prizes.Count, "not a Knock Out");
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Bee_Revenge_Counts_The_Pokemon_In_Your_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bee Revenge"), new ScriptedAgent());
            var vespiquen = e.ScenarioActive(0, C(Vespiquen));
            e.ScenarioAttach(vespiquen, C(Psychic));
            e.ScenarioAttach(vespiquen, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Bounsweet), 5);
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, snorlax.DamageCounters, "20 + 10 x 5");
        }

        [Test]
        public void Destroyer_King_Adds_60_For_Each_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Destroyer King"), new ScriptedAgent());
            var tyranitar = e.ScenarioActive(0, C(TyranitarEX));
            e.ScenarioEvolve(tyranitar, C(MTyranitarEX));
            e.ScenarioAttach(tyranitar, C(Darkness));
            e.ScenarioAttach(tyranitar, C(Darkness));
            e.ScenarioAttach(tyranitar, C(Psychic));
            e.ScenarioAttach(tyranitar, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            snorlax.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(18, snorlax.DamageCounters, "1 + (110 + 60) / 10");
        }

        [Test]
        public void Glaciate_Hits_Each_Of_The_Opponents_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Glaciate"), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C(KyuremEX));
            e.ScenarioAttach(kyurem, C(Water));
            e.ScenarioAttach(kyurem, C(Water));
            e.ScenarioAttach(kyurem, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            var benched = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snorlax.DamageCounters);
            Assert.AreEqual(3, benched.DamageCounters);
        }

        // ------------------------------------------------------------------ Trainers and Energy

        [Test]
        public void Hex_Maniac_Takes_Every_Ability_Away_Theta_Stop_Included()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hex Maniac"), new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C(Gardevoir));
            e.ScenarioAddToHand(0, C(HexManiac));
            var golurk = e.ScenarioActive(1, C(Golett));
            e.ScenarioEvolve(golurk, C(Golurk));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.AbilityBlocked(gardevoir, "Bright Heal"), "yours too");
            Assert.IsTrue(e.AbilityBlocked(golurk, "Double Type"), "a Trainer's effect: θ Stop doesn't stop it");
            Assert.IsTrue(e.OutOfPlayAbilitiesBlocked(1, C(HoopaEX)));
        }

        [Test]
        public void Forest_Of_Giant_Plants_Lets_Grass_Pokemon_Evolve_Right_Away()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var oddish = e.ScenarioBench(0, C(Oddish));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            oddish.TurnEnteredPlay = e.State.TurnNumber;
            Assert.IsFalse(e.CanEvolveInto(oddish, C(Gloom), true));
            e.ScenarioSetStadium(1, C(ForestOfGiantPlants));
            Assert.IsTrue(e.CanEvolveInto(oddish, C(Gloom), true));
        }

        [Test]
        public void Faded_Town_Hits_Each_Mega_Evolution_Pokemon_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mega = e.ScenarioActive(0, C(SceptileEX));
            e.ScenarioEvolve(mega, C(MSceptileEX));
            var plain = e.ScenarioBench(0, C(SceptileEX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetStadium(1, C(FadedTown));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(2, mega.DamageCounters);
            Assert.AreEqual(0, plain.DamageCounters);
        }

        [Test]
        public void Ace_Trainer_Needs_More_Prize_Cards_Left_And_Gives_6_And_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ace Trainer"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(AceTrainer));
            e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Water));
            e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
            Assert.AreEqual(3, e.State.Players[1].Hand.Count);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            var card = e2.ScenarioAddToHand(0, C(AceTrainer));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.CanPlayTrainer(0, card), "not with the same number of Prize cards");
        }

        [Test]
        public void Lysandre_Paint_Roller_And_Steven()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lysandre"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Lysandre));
            e.ScenarioActive(1, C(SnorlaxGX));
            var bench = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(bench, e.State.Players[1].Active);

            var e2 = Scenario(new ScriptedAgent().Then("Play Paint Roller"), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            e2.ScenarioAddToHand(0, C(PaintRoller));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetStadium(1, C(FairyGarden));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsNull(e2.State.Stadium);
            Assert.AreEqual(1, e2.State.Players[0].Hand.Count);

            var e3 = Scenario(new ScriptedAgent().Then("Play Steven", "Lysandre", "Psychic Energy"), new ScriptedAgent());
            e3.ScenarioActive(0, C(LaprasGX));
            e3.ScenarioAddToHand(0, C(Steven));
            e3.ScenarioAddToDeck(0, C(Lysandre), 1);
            e3.ScenarioActive(1, C(SnorlaxGX));
            e3.ScenarioSetTurn(0, 3);
            Step(e3);
            var hand = e3.State.Players[0].Hand;
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Lysandre") && hand.Any(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Dangerous_Energy_Hits_Back_At_Pokemon_EX_And_Flash_Energy_Removes_Weakness()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tyranitar = e.ScenarioActive(0, C(TyranitarEX));
            e.ScenarioAttach(tyranitar, C(DangerousEnergy));
            var jolteon = e.ScenarioBench(0, C(Eevee));
            e.ScenarioEvolve(jolteon, C(Jolteon));
            e.ScenarioAttach(jolteon, C(FlashEnergy));
            var sceptile = e.ScenarioActive(1, C(SceptileEX));
            var lapras = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(sceptile, tyranitar, 30);
            Assert.AreEqual(2, sceptile.DamageCounters, "a Pokémon-EX's attack");
            e.DealAttackDamage(lapras, tyranitar, 30);
            Assert.AreEqual(0, lapras.DamageCounters, "Lapras-GX isn't a Pokémon-EX");
            Assert.IsNull(e.WeaknessOf(jolteon));
            Assert.AreEqual(1, e.EnergyCountOfType(tyranitar, EnergyType.Darkness));
        }
    }
}
