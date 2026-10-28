using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v81: Lost Thunder (LOT 1-236).</summary>
    public class LotTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Switch = "MEG-130";
        const string Spinarak = "LOT-9";
        const string Ariados = "LOT-10";
        const string Hoppip = "LOT-11";
        const string Skiploom = "LOT-13";
        const string Jumpluff = "LOT-14";
        const string Celebi = "LOT-19";
        const string Nincada = "LOT-29";
        const string Ninjask = "LOT-30";
        const string Cyndaquil = "LOT-39";
        const string Quilava = "LOT-41";
        const string Typhlosion = "LOT-42";
        const string Popplio = "LOT-64";
        const string Brionne = "LOT-66";
        const string Primarina = "LOT-67";
        const string Electabuzz = "LOT-71";
        const string Zeraora = "LOT-86";
        const string UnownHand = "LOT-91";
        const string Wobbuffet = "LOT-93";
        const string Shedinja = "LOT-95";
        const string SigilyphGX = "LOT-98";
        const string Sudowoodo = "LOT-110";
        const string Larvitar = "LOT-115";
        const string Pupitar = "LOT-116";
        const string TyranitarGX = "LOT-121";
        const string Steelix = "LOT-125";
        const string Ditto = "LOT-154";
        const string ChoiceHelmet = "LOT-169";
        const string CustomCatcher = "LOT-171";
        const string Electropower = "LOT-172";
        const string Faba = "LOT-173";
        const string HeatFactory = "LOT-178";
        const string ThunderMountain = "LOT-191";
        const string MemoryEnergy = "LOT-194";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private PokemonInPlay Line(GameEngine e, int player, bool active, params string[] stages)
        {
            var mon = active ? e.ScenarioActive(player, C(stages[0])) : e.ScenarioBench(player, C(stages[0]));
            for (int i = 1; i < stages.Length; i++) e.ScenarioEvolve(mon, C(stages[i]));
            return mon;
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "LOT" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(190, own.Count);
            // Cards banned in Expanded are Unlimited only, and basic Energy is Standard as always.
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == (Formats.IsExpandedBanned(d) ? CardCategory.Unlimited
                : d.IsBasicEnergy ? CardCategory.Standard : CardCategory.Expanded)), "no regulation mark: Expanded");
            Assert.AreEqual("Lost Thunder", C("LOT-1").PrintSetName);
            Assert.AreEqual("HIF-12", C("LOT-54").BaseKey, "Slowpoke is the card already in the pool");
            Assert.AreEqual("POR-76", C("LOT-209").BaseKey, "so is Judge");
            Assert.AreEqual("LOT-17", C("LOT-195").BaseKey, "the full art Shuckle-GX");
            Assert.IsTrue(C(Ditto).IsPrismStar);
            Assert.IsTrue(C(HeatFactory).IsPrismStar);
            Assert.AreEqual(0, C("LOT-26").Attacks[0].Damage, "Skill Dive's 50 goes to the Pokémon you choose");
            Assert.AreEqual(1, C("LOT-34").RetreatCost);
        }

        [Test]
        public void Floral_Path_To_The_Sky_Puts_Jumpluff_In_Skiplooms_Place()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Floral Path to the Sky"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioBench(0, C(Snubbull));
            var skiploom = Line(e, 0, false, Hoppip, Skiploom);
            e.ScenarioAttach(skiploom, C(Grass));
            skiploom.DamageCounters = 2;
            e.ScenarioBench(0, C(Snubbull));
            e.ScenarioAddToDeck(0, C(Jumpluff), 1);
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(3, me.Bench.Count);
            var jumpluff = me.Bench[1];
            Assert.AreEqual("Jumpluff", jumpluff.Name, "in the same Bench spot");
            Assert.AreEqual(0, jumpluff.DamageCounters);
            Assert.AreEqual(0, jumpluff.Energy.Count);
            Assert.AreEqual(3, me.LostZone.Count, "Hoppip, Skiploom and the Grass Energy");
            Assert.AreEqual(2, CardsLostMarch(me));

            // No Jumpluff in the deck: nothing happens (your ruling).
            var e2 = Scenario(new ScriptedAgent().Then("Use Floral Path to the Sky"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Snubbull));
            var other = Line(e2, 0, false, Hoppip, Skiploom);
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual("Skiploom", other.Name);
            Assert.IsTrue(e2.State.Players[0].Bench.Contains(other));
            Assert.AreEqual(0, e2.State.Players[0].LostZone.Count);
        }

        private static int CardsLostMarch(PlayerState p) { return p.LostZone.Count(c => c.Def.IsPokemon && !c.Def.IsPrismStar); }

        [Test]
        public void Lost_March_Counts_The_Pokemon_In_The_Lost_Zone_But_Not_Prism_Stars()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lost March"), new ScriptedAgent());
            var jumpluff = Line(e, 0, true, Hoppip, Skiploom, Jumpluff);
            e.ScenarioAttach(jumpluff, C(Grass));
            var me = e.State.Players[0];
            me.LostZone.Add(e.NewCard(0, C(Snubbull)));
            me.LostZone.Add(e.NewCard(0, C(Mewtwo)));
            me.LostZone.Add(e.NewCard(0, C(Celebi)));
            me.LostZone.Add(e.NewCard(0, C(Grass)));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, theirs.DamageCounters, "20 x 2");
        }

        [Test]
        public void Spider_Scram_Paralyzes_And_Poisons_Then_Spinarak_Goes_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spider Scram"), new ScriptedAgent());
            var spinarak = e.ScenarioActive(0, C(Spinarak));
            e.ScenarioAttach(spinarak, C(Grass));
            var next = e.ScenarioBench(0, C(Snubbull));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.IsTrue(theirs.Has(SpecialCondition.Paralyzed));
            Assert.IsTrue(theirs.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(2, me.LostZone.Count, "Spinarak and its Energy");
            Assert.AreSame(next, me.Active, "the Benched Pokémon became Active");
        }

        [Test]
        public void Trapping_Thread_Stops_Switch_On_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trapping Thread"), new ScriptedAgent().Then("Play Switch"));
            var ariados = Line(e, 0, true, Spinarak, Ariados);
            e.ScenarioAttach(ariados, C(Psychic));
            e.ScenarioAttach(ariados, C(Psychic));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioAddToHand(1, C(Switch));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreSame(theirs, e.State.Players[1].Active, "Switch did nothing to the Defending Pokémon");
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Switch"), "but it was played");
        }

        [Test]
        public void Blazing_Energy_Makes_Every_Energy_Fire_For_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Blazing Energy"), new ScriptedAgent());
            var typhlosion = Line(e, 0, true, Cyndaquil, Quilava, Typhlosion);
            for (int i = 0; i < 4; i++) e.ScenarioAttach(typhlosion, C(Water));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.EnergyCountOfType(typhlosion, EnergyType.Fire));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Attack: Lost Flame")));
            Step(e);
            Assert.AreEqual(4, e.EnergyCountOfType(typhlosion, EnergyType.Fire));
            Assert.AreEqual(0, e.EnergyCountOfType(typhlosion, EnergyType.Water));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Attack: Lost Flame")));
            NextTurn(e);
            Assert.AreEqual(4, e.EnergyCountOfType(typhlosion, EnergyType.Water), "only until the end of the turn");
        }

        [Test]
        public void Harmonics_Attaches_A_Second_Energy_To_The_Same_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy to Snubbull"), new ScriptedAgent());
            var snubbull = e.ScenarioActive(0, C(Snubbull));
            var primarina = Line(e, 0, false, Popplio, Brionne, Primarina);
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, snubbull.Energy.Count);
            Assert.AreEqual(0, primarina.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.AttachEnergy), "the attachment for the turn is used");
        }

        [Test]
        public void Vessel_Of_Life_Makes_Shedinja_A_Tool_That_Gives_Up_1_Prize_Card_Less()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Vessel of Life", "Mewtwo"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            var shedinja = e.ScenarioBench(0, C(Shedinja));
            e.ScenarioAttach(shedinja, C(Psychic));
            e.ScenarioBench(0, C(Snubbull));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(1, mewtwo.Tools.Count);
            Assert.AreEqual("Shedinja", mewtwo.Tools[0].Def.Name);
            Assert.AreEqual(1, me.Bench.Count);
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Psychic Energy"));

            e.DealAttackDamage(theirs, mewtwo, 300);
            e.ResolveKnockouts();
            Assert.AreEqual(6 - (C(Mewtwo).PrizeValue - 1), e.State.Players[1].Prizes.Count, "1 Prize card fewer");
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Shedinja"));
        }

        [Test]
        public void Unown_Hand_Wins_With_35_Cards_In_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use HAND"), new ScriptedAgent());
            e.ScenarioActive(0, C(UnownHand));
            e.ScenarioActive(1, C(Snubbull));
            for (int i = 0; i < 34; i++) e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Use HAND")), "34 isn't enough");
            e.ScenarioAddToHand(0, C(Psychic));
            Step(e);
            Assert.IsTrue(e.State.IsOver);
            Assert.AreEqual(0, e.State.Winner);
        }

        [Test]
        public void Almighty_Evolution_Puts_Any_Stage_1_On_Ditto_Unless_Wobbuffet_Is_Benched()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Almighty Evolution"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            var ditto = e.ScenarioBench(0, C(Ditto));
            e.ScenarioAddToHand(0, C(Ninjask));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 1);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Almighty Evolution")), "not on your first turn");
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Ninjask", ditto.Name);
            Assert.AreEqual(2, ditto.Stack.Count);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Snubbull));
            e2.ScenarioBench(0, C(Ditto));
            e2.ScenarioAddToHand(0, C(Ninjask));
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioBench(1, C(Wobbuffet));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.Contains("Almighty Evolution")), "Shady Tail: Prism Star Pokémon have no Abilities");

            var e3 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var celebi = e3.ScenarioActive(0, C(Celebi));
            e3.ScenarioAttach(celebi, C(Grass));
            e3.ScenarioActive(1, C(Snubbull));
            e3.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e3.GetMainActions(0).Any(o => o.Label.Contains("Attack: Leech Seed")));
            e3.ScenarioBench(0, C(Wobbuffet));
            Assert.IsFalse(e3.GetMainActions(0).Any(o => o.Label.Contains("Attack:")), "and can't attack");
        }

        [Test]
        public void Lost_Out_Sends_The_Knocked_Out_Pokemon_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dusty Ruckus"), new ScriptedAgent());
            var tyranitar = Line(e, 0, true, Larvitar, Pupitar, TyranitarGX);
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tyranitar, C(Darkness));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAttach(theirs, C(Psychic));
            var benched = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(2, opp.LostZone.Count, "Snubbull and its Energy");
            Assert.AreEqual(0, opp.Discard.Count);
            Assert.AreEqual(3, benched.DamageCounters, "30 to each Benched Basic Pokémon");
        }

        [Test]
        public void Mirror_Counter_Puts_The_Damage_Back_On_A_Pokemon_GX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dusty Ruckus"), new ScriptedAgent());
            var tyranitar = Line(e, 0, true, Larvitar, Pupitar, TyranitarGX);
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tyranitar, C(Darkness));
            var sigilyph = e.ScenarioActive(1, C(SigilyphGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, sigilyph.DamageCounters);
            Assert.AreEqual(13, tyranitar.DamageCounters, "130 back on the attacker");
        }

        [Test]
        public void Custom_Catcher_Draws_To_3_Or_Gusts_When_2_Are_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Custom Catcher"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAddToHand(0, C(CustomCatcher));
            e.ScenarioAddToHand(0, C(CustomCatcher));
            e.ScenarioActive(1, C(Snubbull));
            var mewtwo = e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreSame(mewtwo, e.State.Players[1].Active);
            Assert.AreEqual(2, me.Discard.Count(c => c.Def.Name == "Custom Catcher"));
            Assert.AreEqual(0, me.Hand.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Play Custom Catcher"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Snubbull));
            e2.ScenarioAddToHand(0, C(CustomCatcher));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioActive(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(3, e2.State.Players[0].Hand.Count);
        }

        [Test]
        public void Memory_Energy_Gives_The_Attacks_Of_Previous_Evolutions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ninjask = Line(e, 0, true, Nincada, Ninjask);
            e.ScenarioAttach(ninjask, C(Psychic));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Fury Swipes")));
            ninjask.Energy.Clear();
            e.ScenarioAttach(ninjask, C(MemoryEnergy));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Fury Swipes")), string.Join(" | ", e.GetMainActions(0).Select(o => o.Label).ToArray()));
        }

        [Test]
        public void Faba_Cannot_Take_A_Prism_Star_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Faba"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAddToHand(0, C(Faba));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAttachTool(theirs, C(ChoiceHelmet));
            e.ScenarioSetStadium(1, C(HeatFactory));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, theirs.Tools.Count);
            Assert.AreEqual(1, e.State.Players[1].LostZone.Count(c => c.Def.Name == "Choice Helmet"));
            Assert.AreEqual("Heat Factory ◇", e.State.Stadium.Def.Name);
        }

        [Test]
        public void Thunder_Mountain_And_Thunderclap_Zone_Help_Lightning_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var electabuzz = e.ScenarioActive(0, C(Electabuzz));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            var shock = electabuzz.Def.Attacks[0];
            Assert.AreEqual(2, e.AttackCost(electabuzz, shock).Count);
            e.ScenarioSetStadium(1, C(ThunderMountain));
            var cost = e.AttackCost(electabuzz, shock);
            Assert.AreEqual(1, cost.Count);
            Assert.IsFalse(cost.Contains(EnergyType.Lightning));

            Assert.AreEqual(electabuzz.Def.RetreatCost, e.RetreatCostOf(electabuzz));
            e.ScenarioBench(0, C(Zeraora));
            Assert.AreEqual(electabuzz.Def.RetreatCost, e.RetreatCostOf(electabuzz), "no Lightning Energy attached yet");
            e.ScenarioAttach(electabuzz, C(Lightning));
            Assert.AreEqual(0, e.RetreatCostOf(electabuzz));
        }

        [Test]
        public void Electropower_And_Reply_Strongly_Add_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Electropower", "Attack: Thunder Shock"), new ScriptedAgent());
            var electabuzz = e.ScenarioActive(0, C(Electabuzz));
            e.ScenarioAttach(electabuzz, C(Lightning));
            e.ScenarioAttach(electabuzz, C(Lightning));
            e.ScenarioAddToHand(0, C(Electropower));
            var steelix = e.ScenarioActive(1, C(Steelix));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(6, steelix.DamageCounters, "30 + 30");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Reply Strongly"), new ScriptedAgent());
            var sudowoodo = e2.ScenarioActive(0, C(Sudowoodo));
            e2.ScenarioAttach(sudowoodo, C(Fighting));
            var target = e2.ScenarioActive(1, C(Steelix));
            e2.ScenarioSetTurn(1, 2);
            e2.DealAttackDamage(target, sudowoodo, 20);
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(10, target.DamageCounters, "20 + 80: it was hit while Active last turn");
        }
    }
}
