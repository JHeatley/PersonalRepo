using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v90: Sun &amp; Moon (SUM 1-163), and Pokémon Tools no longer counting as Item cards (your ruling).</summary>
    public class SumTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string DoubleColorless = "SLG-69";
        const string FairyEnergy = "SUM-Y";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Turtwig = "UPR-7";
        const string Celesteela = "UPR-144";
        const string ChoiceBand = "BUS-162";
        const string PokeBall = "POR-80";
        const string Mawile = "CES-91";
        const string Pinsir = "SUM-6";
        const string Masquerain = "SUM-8";
        const string LurantisGX = "SUM-15";
        const string Shiinotic = "SUM-17";
        const string Steenee = "SUM-19";
        const string Tsareena = "SUM-20";
        const string Arcanine = "SUM-22";
        const string Golduck = "SUM-29";
        const string Cloyster = "SUM-34";
        const string LaprasGX = "SUM-35";
        const string Wingull = "SUM-37";
        const string Pelipper = "SUM-38";
        const string Wishiwashi = "SUM-44";
        const string Pyukumuku = "SUM-47";
        const string Bruxish = "SUM-48";
        const string Chinchou = "SUM-49";
        const string Vikavolt = "SUM-52";
        const string Crobat = "SUM-56";
        const string AlolanMuk = "SUM-58";
        const string Toxapex = "SUM-63";
        const string Gigalith = "SUM-71";
        const string Palossand = "SUM-75";
        const string AlolanRaticate = "SUM-77";
        const string Krookodile = "SUM-85";
        const string AlolanDugtrio = "SUM-87";
        const string Skarmory = "SUM-88";
        const string Cutiefly = "SUM-92";
        const string Ribombee = "SUM-93";
        const string Dratini = "SUM-94";
        const string Dragonair = "SUM-95";
        const string Dragonite = "SUM-96";
        const string Lillipup = "SUM-103";
        const string Herdier = "SUM-104";
        const string Bewear = "SUM-112";
        const string Oranguru = "SUM-113";
        const string BigMalasada = "SUM-114";
        const string Ilima = "SUM-121";
        const string PoisonBarb = "SUM-124";
        const string ProfessorKukui = "SUM-128";
        const string RotomDex = "SUM-131";
        const string TeamSkullGrunt = "SUM-133";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "SUM" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(109, own.Count, "108 new cards and the Fairy Energy already in the pool");
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Sun & Moon", C(Pinsir).PrintSetName);
            Assert.AreEqual("HIF-SV47", C("SUM-12").BaseKey, "Decidueye-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("GRI-147", C("SUM-27").BaseKey, "Incineroar-GX came with Guardians Rising");
            Assert.AreEqual("SUM-15", C("SUM-138").BaseKey, "full art Lurantis-GX");
            Assert.AreEqual("SUM-15", C("SUM-150").BaseKey, "rainbow rare Lurantis-GX");
            Assert.AreEqual("MEE-13", C("SUM-162").BaseKey, "gold Psychic Energy");
            Assert.AreEqual("HIF-SV41", C("SUM-101a").BaseKey, "alternate Eevee");
            Assert.AreEqual(0, C(AlolanDugtrio).Attacks[0].Damage, "Dig Under's 50 is only in the text");
            Assert.AreEqual(0, C("SUM-57").Attacks[0].Cost.Length, "Super Poison Breath costs nothing");
            Assert.AreEqual("Megaton Slap Push", C("SUM-68").Attacks[1].Name);
        }

        // ------------------------------------------------------------------ Tools aren't Items (your ruling, v90)

        [Test]
        public void Mining_Finds_Item_Cards_But_Never_A_Pokemon_Tool()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mining"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(Mawile));
            e.ScenarioAttach(mawile, C(Psychic));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C(ChoiceBand), 1);
            e.ScenarioAddToDeck(0, C(PokeBall), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(1, hand.Count);
            Assert.AreEqual("Poké Ball", hand[0].Def.Name);
            Assert.IsTrue(e.State.Players[0].Deck.Any(c => c.Def.Name == "Choice Band"));
        }

        [Test]
        public void Treasure_Hunt_Takes_An_Item_Not_A_Tool()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Lillipup));
            e.ScenarioAddToHand(0, C(Herdier));
            e.ScenarioAddToDiscard(0, C(ChoiceBand), 1);
            e.ScenarioAddToDiscard(0, C(PokeBall), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Poké Ball", e.State.Players[0].Hand.Single().Def.Name);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Choice Band"));
        }

        // ------------------------------------------------------------------ Grass and Fire

        [Test]
        public void Roof_Fling_Puts_The_Defending_Pokemon_Into_The_Hand_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Roof Fling"), new ScriptedAgent());
            var pinsir = e.ScenarioActive(0, C(Pinsir));
            e.ScenarioAttach(pinsir, C(Psychic));
            e.ScenarioAttach(pinsir, C(Psychic));
            var target = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAttach(target, C(Psychic));
            e.ScenarioBench(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            bool inHand = e.State.Players[1].Hand.Any(c => c.Def.Name == "Snubbull");
            bool inPlay = e.State.FindPokemon(target.Uid) != null;
            Assert.AreNotEqual(inHand, inPlay, "heads: into the hand with its Energy; tails: nothing happens");
            if (inHand) Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.IsEnergy));
        }

        [Test]
        public void Struggle_Bug_Moves_An_Energy_To_The_Opponents_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Struggle Bug"), new ScriptedAgent());
            var masquerain = e.ScenarioActive(0, C(Masquerain));
            e.ScenarioAttach(masquerain, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAttach(target, C(Psychic));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
            Assert.AreEqual(0, target.Energy.Count);
            Assert.AreEqual(1, benched.Energy.Count);
        }

        [Test]
        public void Flower_Supply_Attaches_2_Basic_Energy_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flower Supply"), new ScriptedAgent());
            var lurantis = e.ScenarioActive(0, C(LurantisGX));
            e.ScenarioAttach(lurantis, C(Grass));
            var benched = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, lurantis.Energy.Count + benched.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.IsEnergy));
        }

        [Test]
        public void Illuminate_Finds_A_Grass_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Illuminate"), new ScriptedAgent());
            e.ScenarioActive(0, C(Shiinotic));
            e.ScenarioAddToDeck(0, C(Snubbull), 1);
            e.ScenarioAddToDeck(0, C(Turtwig), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Turtwig", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Queenly_Majesty_Discards_A_Card_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Steenee));
            e.ScenarioAddToHand(0, C(Tsareena));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[1].Hand.Count);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Snubbull"));
        }

        [Test]
        public void Firestorm_Discards_3_Fire_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Firestorm"), new ScriptedAgent());
            var arcanine = e.ScenarioActive(0, C(Arcanine));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(arcanine, C(Fire));
            e.ScenarioAttach(arcanine, C(Psychic));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, arcanine.Energy.Count);
            Assert.AreEqual(1, arcanine.Energy.Count(c => c.Def.Name == "Fire Energy"));
        }

        // ------------------------------------------------------------------ Water and Lightning

        [Test]
        public void Double_Jet_Does_60_For_Each_Water_Energy_Discarded_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Double Jet", "Water Energy|Water Energy"), new ScriptedAgent());
            var golduck = e.ScenarioActive(0, C(Golduck));
            e.ScenarioAttach(golduck, C(Water));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Water));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Headlock_Does_30_More_On_Heads_And_Paralyzes_On_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Headlock"), new ScriptedAgent());
            var cloyster = e.ScenarioActive(0, C(Cloyster));
            e.ScenarioAttach(cloyster, C(Water));
            e.ScenarioAttach(cloyster, C(Water));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            bool paralyzed = target.Has(SpecialCondition.Paralyzed);
            Assert.AreEqual(paralyzed ? 3 : 6, target.DamageCounters);
        }

        [Test]
        public void Roost_Heals_And_Stops_Retreating_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Roost"), new ScriptedAgent());
            var wingull = e.ScenarioActive(0, C(Wingull));
            e.ScenarioAttach(wingull, C(Psychic));
            e.ScenarioAttach(wingull, C(Psychic));
            wingull.DamageCounters = 3;
            e.ScenarioBench(0, C(Turtwig));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, wingull.DamageCounters);
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Retreat));
        }

        [Test]
        public void Courier_Puts_A_Benched_Pokemon_And_Its_Cards_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Courier"), new ScriptedAgent());
            var pelipper = e.ScenarioActive(0, C(Pelipper));
            e.ScenarioAttach(pelipper, C(Psychic));
            e.ScenarioAttach(pelipper, C(Psychic));
            var benched = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAttach(benched, C(Grass));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(benched.Uid));
            var hand = e.State.Players[0].Hand;
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Turtwig"));
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Grass Energy"));
        }

        [Test]
        public void Cowardice_Returns_An_Active_Wishiwashi_And_Promotes_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Cowardice"), new ScriptedAgent());
            var fish = e.ScenarioActive(0, C(Wishiwashi));
            e.ScenarioAttach(fish, C(Water));
            var benched = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Wishiwashi", e.State.Players[0].Hand.Single().Def.Name, "only the Pokémon; its Energy is discarded");
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Water Energy"));
            Assert.AreSame(benched, e.State.Players[0].Active);
        }

        [Test]
        public void Cowardice_Not_On_The_Turn_Wishiwashi_Came_Into_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Mewtwo));
            var fish = e.ScenarioBench(0, C(Wishiwashi));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            fish.TurnEnteredPlay = 3;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Cowardice")));
            fish.TurnEnteredPlay = 1;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Cowardice")));
        }

        [Test]
        public void Innards_Out_Puts_6_Counters_On_The_Attacker_When_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blizzard Burn"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(lapras, C(Water));
            e.ScenarioActive(1, C(Pyukumuku));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, lapras.DamageCounters);
        }

        [Test]
        public void Psychic_Fangs_Ignores_Wall_Of_Sand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psychic Fangs"), new ScriptedAgent());
            var bruxish = e.ScenarioActive(0, C(Bruxish));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(bruxish, C(Water));
            var target = e.ScenarioActive(1, C(Palossand));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, target.DamageCounters);
            e.DealAttackDamage(bruxish, target, 30);
            Assert.AreEqual(10, target.DamageCounters, "an ordinary 30 is 20 less");
        }

        [Test]
        public void Searching_Light_Shows_A_Prize_Card()
        {
            var me = new ScriptedAgent().Then("Attack: Searching Light");
            var e = Scenario(me, new ScriptedAgent());
            var chinchou = e.ScenarioActive(0, C(Chinchou));
            e.ScenarioAttach(chinchou, C(Psychic));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(me.Seen.Any(r => (r.Prompt ?? "").Contains("Searching Light")));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Strong_Charge_Attaches_A_Grass_And_A_Lightning_Energy_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Strong Charge"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Mewtwo));
            var vikavolt = e.ScenarioBench(0, C(Vikavolt));
            e.ScenarioAddToDeck(0, C(Grass), 1);
            e.ScenarioAddToDeck(0, C("MEE-12"), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var attached = active.Energy.Concat(vikavolt.Energy).Select(c => c.Def.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "Grass Energy", "Lightning Energy" }, attached);
        }

        // ------------------------------------------------------------------ Psychic and Fighting

        [Test]
        public void Surprise_Strike_Does_60_More_After_Coming_In_This_Turn()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Surprise Strike"), new ScriptedAgent());
                var crobat = e.ScenarioActive(0, C(Crobat));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(crobat, C(Psychic));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1) crobat.MovedToActiveOnTurn = 3;
                Step(e);
                Assert.AreEqual(pass == 0 ? 6 : 12, target.DamageCounters);
            }
        }

        [Test]
        public void Power_Of_Alchemy_Turns_Off_Basic_Pokemon_Abilities()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Mewtwo));
                e.ScenarioBench(0, C(Oranguru));
                var opp = e.ScenarioActive(1, C(Mewtwo));
                if (pass == 1) e.ScenarioBench(1, C(AlolanMuk));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(pass == 0, e.GetMainActions(0).Any(o => o.Label.Contains("Instruct")), pass == 0 ? "no Alolan Muk" : "Alolan Muk in play");
            }
        }

        [Test]
        public void Toxic_Spikes_Poisons_The_New_Active_Pokemon_After_A_Retreat()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Retreat"));
            e.ScenarioActive(0, C(Toxapex));
            var retreating = e.ScenarioActive(1, C(Turtwig));
            e.ScenarioAttach(retreating, C(Psychic));
            e.ScenarioAttach(retreating, C(Psychic));
            var incoming = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreSame(incoming, e.State.Players[1].Active);
            Assert.IsTrue(incoming.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(retreating.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Rock_Artillery_Does_50_For_Each_Fighting_Energy_Discarded_From_Any_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Artillery", "Fighting Energy|Fighting Energy|Fighting Energy"), new ScriptedAgent());
            var gigalith = e.ScenarioActive(0, C(Gigalith));
            e.ScenarioAttach(gigalith, C(Fighting));
            e.ScenarioAttach(gigalith, C(Fighting));
            var benched = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAttach(benched, C(Fighting));
            var target = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, target.DamageCounters);
            Assert.AreEqual(0, gigalith.Energy.Count + benched.Energy.Count);
        }

        [Test]
        public void Absorb_Vitality_Heals_The_Damage_Done()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Absorb Vitality"), new ScriptedAgent());
            var palossand = e.ScenarioActive(0, C(Palossand));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(palossand, C(Fighting));
            palossand.DamageCounters = 6;
            var target = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, target.DamageCounters);
            Assert.AreEqual(1, palossand.DamageCounters);
        }

        // ------------------------------------------------------------------ Darkness, Metal, Fairy and Dragon

        [Test]
        public void Evil_Orders_Finds_As_Many_Cards_As_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Evil Orders", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var raticate = e.ScenarioActive(0, C(AlolanRaticate));
            e.ScenarioAttach(raticate, C(Darkness));
            e.ScenarioAttach(raticate, C(Darkness));
            e.ScenarioBench(0, C(Turtwig));
            e.ScenarioBench(0, C(Snubbull));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Obsidian_Fang_Discards_Tools_Before_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Obsidian Fang"), new ScriptedAgent());
            var krookodile = e.ScenarioActive(0, C(Krookodile));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(krookodile, C(Darkness));
            var target = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAttachTool(target, C(PoisonBarb));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, target.Tools.Count);
            Assert.AreEqual(12, target.DamageCounters);
            Assert.IsFalse(krookodile.Has(SpecialCondition.Poisoned), "Poison Barb was gone before the damage");
        }

        [Test]
        public void Tangling_Hair_Adds_1_To_The_Opponents_Active_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Turtwig));
            var theirs = e.ScenarioActive(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            int before = e.RetreatCostOf(theirs);
            e.ScenarioBench(0, C(AlolanDugtrio));
            Assert.AreEqual(before + 1, e.RetreatCostOf(theirs));
            Assert.AreEqual(before, e.RetreatCostOf(mine));
        }

        [Test]
        public void Dig_Under_Ignores_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dig Under", "Snubbull"), new ScriptedAgent());
            var dugtrio = e.ScenarioActive(0, C(AlolanDugtrio));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(dugtrio, C(Psychic));
            var target = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, target.DamageCounters, "Snubbull is weak to Metal, but it's 50");
        }

        [Test]
        public void Metallic_Sound_Discards_Special_Energy_From_Every_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Metallic Sound"), new ScriptedAgent());
            var skarmory = e.ScenarioActive(0, C(Skarmory));
            e.ScenarioAttach(skarmory, C(Psychic));
            e.ScenarioAttach(skarmory, C(Psychic));
            var mine = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAttach(mine, C(DoubleColorless));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAttach(theirs, C(DoubleColorless));
            e.ScenarioAttach(theirs, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, skarmory.Energy.Count);
            Assert.AreEqual(0, mine.Energy.Count);
            Assert.AreEqual("Psychic Energy", theirs.Energy.Single().Def.Name);
        }

        [Test]
        public void Curative_Pollen_Heals_20()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Curative Pollen"), new ScriptedAgent());
            var hurt = e.ScenarioActive(0, C(Mewtwo));
            hurt.DamageCounters = 3;
            e.ScenarioBench(0, C(Ribombee));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, hurt.DamageCounters);
        }

        [Test]
        public void Fly_Around_May_Dodge_Damage_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fly Around"), new ScriptedAgent());
            var cutiefly = e.ScenarioActive(0, C(Cutiefly));
            e.ScenarioAttach(cutiefly, C(FairyEnergy));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            e.DealAttackDamage(theirs, cutiefly, 20);
            Assert.That(cutiefly.DamageCounters, Is.EqualTo(0).Or.EqualTo(2));
            Assert.IsTrue(e.State.Effects.Any(x => x.Name.Contains("Fly Around")));
        }

        [Test]
        public void Signs_Of_Evolution_Finds_1_Of_Each_Name()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Signs of Evolution", "Dragonair|Dragonair|Dratini"), new ScriptedAgent());
            var dratini = e.ScenarioActive(0, C(Dratini));
            e.ScenarioAttach(dratini, C(Grass));
            e.ScenarioAddToDeck(0, C(Dratini), 1);
            e.ScenarioAddToDeck(0, C(Dragonair), 2);
            e.ScenarioAddToDeck(0, C(Dragonite), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand.Select(c => c.Def.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "Dragonair", "Dratini" }, hand);
            Assert.AreEqual(1, e.State.Players[0].Deck.Count(c => c.Def.Name == "Dragonair"));
        }

        [Test]
        public void Dragons_Wish_Allows_Any_Number_Of_Energy_Attachments_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragon's Wish"), new ScriptedAgent());
            var dragonair = e.ScenarioActive(0, C(Dragonair));
            e.ScenarioAttach(dragonair, C(Psychic));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].EnergyAttachedThisTurn = true;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.AttachEnergy), "only 1 attachment a turn normally");
            Step(e);
            NextTurn(e);
            NextTurn(e);
            e.State.Players[0].EnergyAttachedThisTurn = true;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Action == ActionType.AttachEnergy));
        }

        // ------------------------------------------------------------------ Colorless

        [Test]
        public void Superpower_May_Do_40_More_With_Recoil()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Superpower", "Yes"), new ScriptedAgent());
            var bewear = e.ScenarioActive(0, C(Bewear));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(bewear, C(Psychic));
            var target = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
            Assert.AreEqual(2, bewear.DamageCounters);
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Big_Malasada_Heals_20_And_Removes_One_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Big Malasada", "Poisoned"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Mewtwo));
            active.DamageCounters = 3;
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(BigMalasada));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(active, SpecialCondition.Confused);
            e.ApplyCondition(active, SpecialCondition.Poisoned);
            Step(e);
            Assert.AreEqual(1, active.DamageCounters);
            Assert.IsFalse(active.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(active.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Ilima_Refreshes_Both_Hands_To_6_Or_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ilima"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(Ilima));
            e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.That(e.State.Players[0].Hand.Count, Is.EqualTo(3).Or.EqualTo(6));
            Assert.That(e.State.Players[1].Hand.Count, Is.EqualTo(3).Or.EqualTo(6));
            Assert.AreEqual(1, e.State.Players[1].Deck.Count(c => c.Def.Name == "Grass Energy") + e.State.Players[1].Hand.Count(c => c.Def.Name == "Grass Energy"));
        }

        [Test]
        public void Poison_Barb_Poisons_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blizzard Burn"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(lapras, C(Water));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioAttachTool(target, C(PoisonBarb));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(lapras.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Professor_Kukui_Draws_2_And_Adds_20_Damage_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Professor Kukui", "Attack: Bear Hug"), new ScriptedAgent());
            var bewear = e.ScenarioActive(0, C(Bewear));
            e.ScenarioAttach(bewear, C(Psychic));
            e.ScenarioAttach(bewear, C(Psychic));
            var target = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAddToHand(0, C(ProfessorKukui));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Step(e);
            Assert.AreEqual(6, target.DamageCounters, "40 + 20");
        }

        [Test]
        public void Rotom_Dex_Deals_The_Same_Number_Of_New_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rotom Dex"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioActive(1, C(Mewtwo));
            e.State.Players[0].Prizes.RemoveRange(0, 2);
            e.ScenarioAddToHand(0, C(RotomDex));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count);
            Assert.AreEqual(20, e.State.Players[0].Deck.Count);
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count(c => c.Def.Name == "Fire Energy") + e.State.Players[0].Prizes.Count(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Team_Skull_Grunt_Discards_2_Energy_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Skull Grunt", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(TeamSkullGrunt));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[1].Hand;
            Assert.AreEqual(1, hand.Count(c => c.Def.IsEnergy));
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Snubbull"));
        }
    }
}
