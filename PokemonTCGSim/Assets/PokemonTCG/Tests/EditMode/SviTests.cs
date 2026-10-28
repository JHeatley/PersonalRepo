using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v41: Scarlet &amp; Violet (SVI), with your Poison Sacs and Forced Sleep rulings.</summary>
    public class SviTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";

        const string Scatterbug = "SVI-8";
        const string Spewpa = "SVI-9";
        const string Vivillon = "SVI-10";
        const string SpidopsEx = "SVI-19";
        const string Cetitan = "SVI-60";
        const string MagnezoneEx = "SVI-65";
        const string Toxtricity = "SVI-72";
        const string Drowzee = "SVI-82";
        const string BanetteEx = "SVI-88";
        const string Florges = "SVI-93";
        const string Greavard = "SVI-105";
        const string Krokorok = "SVI-116";
        const string Sandaconda = "SVI-120";
        const string Stonjourner = "SVI-121";
        const string Klawf = "SVI-122";
        const string GreatTuskEx = "SVI-123";
        const string Muk = "SVI-127";
        const string ToxicroakEx = "SVI-131";
        const string IronTreadsEx = "SVI-143";
        const string Blissey = "SVI-145";
        const string Indeedee = "SVI-153";
        const string DefianceBand = "SVI-169";
        const string ExpShare = "SVI-174";
        const string Jacq = "SVI-175";
        const string Katy = "SVI-177";
        const string Mesagoza = "SVI-178";
        const string Miriam = "SVI-179";
        const string PalPad = "SVI-182";
        const string PicnicBasket = "SVI-184";
        const string RockChestplate = "SVI-192";
        const string RockyHelmet = "SVI-193";
        const string VitalityBand = "SVI-197";
        const string Youngster = "SVI-198";

        const string Tauros = "MEW-128";
        const string Pidgey = "OBF-162";
        const string Pidgeotto = "OBF-163";
        const string DragoniteEx = "OBF-159";
        const string PokeBall = "POR-80";
        const string Potion = "POR-83";
        const string RareCandy = "MEG-125";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ================================================================== Data

        [Test]
        public void Every_Scarlet_And_Violet_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 258; n++)
                Assert.IsNotNull(db.Get("SVI-" + n), "SVI " + n + " is missing");
        }

        [Test]
        public void Reprints_Are_Alternate_Prints_Of_The_Pool_Card()
        {
            Assert.AreEqual("SVP-24", db.MainPrinting(C("SVI-31")).Key, "Growlithe");
            Assert.AreEqual("SVP-5", db.MainPrinting(C("SVI-54")).Key, "Quaquaval");
            Assert.AreEqual("SVP-14", db.MainPrinting(C("SVI-124")).Key, "Koraidon");
            Assert.AreEqual("POR-72", db.MainPrinting(C("SVI-172")).Key, "Energy Search");
            Assert.AreEqual(SpidopsEx, db.MainPrinting(C("SVI-243")).Key, "Spidops ex's gold print");
            Assert.IsTrue(C("SVI-32").IsTera && C("SVI-45").IsTera, "Arcanine ex and Gyarados ex are Tera");
        }

        // ================================================================== Your rulings

        [Test]
        public void Poison_Sacs_Keeps_The_Opponents_Poison_With_Toxic_Rippers_6_Counters_When_It_Evolves()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("into Pidgeotto"));
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Muk));
            var pidgey = e.ScenarioActive(1, C(Pidgey));
            e.ScenarioAddToHand(1, C(Pidgeotto));
            e.ScenarioSetTurn(1, 4);
            e.ApplyCondition(pidgey, SpecialCondition.Poisoned);
            pidgey.PoisonCounterBase = 6;
            Step(e);
            Assert.AreEqual("Pidgeotto", pidgey.Name);
            Assert.IsTrue(pidgey.Has(SpecialCondition.Poisoned), "Poison Sacs");
            Assert.AreEqual(6, pidgey.PoisonCounterBase, "your ruling: still 6 counters");

            var plain = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("into Pidgeotto"));
            plain.ScenarioActive(0, C(Tauros));
            var other = plain.ScenarioActive(1, C(Pidgey));
            plain.ScenarioAddToHand(1, C(Pidgeotto));
            plain.ScenarioSetTurn(1, 4);
            plain.ApplyCondition(other, SpecialCondition.Poisoned);
            Step(plain);
            Assert.IsFalse(other.Has(SpecialCondition.Poisoned), "without Muk, evolving cures it");
        }

        [Test]
        public void Poison_Sacs_Does_Not_Cover_Your_Own_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("into Pidgeotto"), new ScriptedAgent());
            var pidgey = e.ScenarioActive(0, C(Pidgey));
            e.ScenarioBench(0, C(Muk));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Pidgeotto));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(pidgey, SpecialCondition.Poisoned);
            Step(e);
            Assert.AreEqual("Pidgeotto", pidgey.Name);
            Assert.IsFalse(pidgey.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Forced_Sleep_Does_Nothing_When_The_Opponent_Has_No_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Forced Sleep"), new ScriptedAgent());
            var drowzee = e.ScenarioActive(0, C(Drowzee));
            e.ScenarioAttach(drowzee, C(Psychic));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(tauros, e.State.Players[1].Active);
            Assert.IsFalse(tauros.Has(SpecialCondition.Asleep), "your ruling: nothing happens");

            var b = Scenario(new ScriptedAgent().Then("Attack: Forced Sleep"), new ScriptedAgent());
            var drowzee2 = b.ScenarioActive(0, C(Drowzee));
            b.ScenarioAttach(drowzee2, C(Psychic));
            var oldActive = b.ScenarioActive(1, C(Tauros));
            var pidgey = b.ScenarioBench(1, C(Pidgey));
            b.ScenarioSetTurn(0, 3);
            Step(b);
            Assert.AreSame(pidgey, b.State.Players[1].Active);
            Assert.IsTrue(pidgey.Has(SpecialCondition.Asleep), "the new Active Pokémon is now Asleep");
            Assert.IsFalse(oldActive.Has(SpecialCondition.Asleep));
        }

        // ================================================================== Abilities

        [Test]
        public void Adaptive_Evolution_Works_On_The_First_Turn_And_The_Turn_It_Is_Played()
        {
            var first = Scenario(new ScriptedAgent(), new ScriptedAgent());
            first.ScenarioActive(0, C(Scatterbug));
            first.ScenarioBench(0, C(Pidgey));
            first.ScenarioActive(1, C(Tauros));
            first.ScenarioAddToHand(0, C(Spewpa));
            first.ScenarioAddToHand(0, C(Pidgeotto));
            first.ScenarioSetTurn(0, 1);
            Assert.IsTrue(Offered(first, 0, "into Spewpa"), "your first turn");
            Assert.IsFalse(Offered(first, 0, "into Pidgeotto"), "Pidgey has no Adaptive Evolution");

            var e = Scenario(new ScriptedAgent().Then("Bench Scatterbug", "into Spewpa", "into Vivillon"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Scatterbug));
            e.ScenarioAddToHand(0, C(Spewpa));
            e.ScenarioAddToHand(0, C(Vivillon));
            e.ScenarioAddToHand(0, C(RareCandy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(Offered(e, 0, "Play Rare Candy"), "Compendium: Rare Candy still can't be used on it this turn");
            Step(e);
            Step(e);
            Assert.AreEqual("Vivillon", e.State.Players[0].Bench[0].Name, "Spewpa has Adaptive Evolution too");
        }

        [Test]
        public void Trap_Territory_Raises_The_Opponents_Active_Retreat_Cost_And_Wire_Hang_Counts_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wire Hang"), new ScriptedAgent());
            var spidops = e.ScenarioActive(0, C(SpidopsEx));
            e.ScenarioAttach(spidops, C(Grass));
            e.ScenarioAttach(spidops, C(Grass));
            var mine = e.ScenarioBench(0, C(Tauros));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            var theirBench = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(3, e.RetreatCostOf(dragonite));
            Assert.AreEqual(2, e.RetreatCostOf(theirBench), "only the Active Pokémon");
            Assert.AreEqual(2, e.RetreatCostOf(mine), "only the opponent's");
            Step(e);
            Assert.AreEqual(18, dragonite.DamageCounters, "90 + 30 x 3");
        }

        [Test]
        public void Blooming_Garden_Removes_Weakness_From_Your_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Florges));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNull(e.WeaknessOf(mine));
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(theirs));
        }

        [Test]
        public void Exoskeleton_Takes_20_Less()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var stonjourner = e.ScenarioActive(1, C(Stonjourner));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, stonjourner, 50);
            Assert.AreEqual(3, stonjourner.DamageCounters);
        }

        [Test]
        public void Busybody_Nurse_Cures_Your_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Busybody Nurse"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Blissey));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Busybody Nurse"), "nothing to cure");
            e.ApplyCondition(tauros, SpecialCondition.Poisoned);
            e.ApplyCondition(tauros, SpecialCondition.Confused);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, tauros.Conditions);
            Assert.IsFalse(Offered(e, 0, "Use Busybody Nurse"), "once during your turn");
        }

        // ================================================================== Attacks

        [Test]
        public void Toxic_Ripper_Poison_Puts_6_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Toxic Ripper"), new ScriptedAgent());
            var toxicroak = e.ScenarioActive(0, C(ToxicroakEx));
            e.ScenarioAttach(toxicroak, C(Darkness));
            e.ScenarioAttach(toxicroak, C(Psychic));
            e.ScenarioAttach(toxicroak, C(Psychic));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, dragonite.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(18, dragonite.DamageCounters, "6 Poison counters in Pokémon Checkup");
        }

        [Test]
        public void Yank_Away_Shuffles_2_Random_Cards_From_The_Opponents_Hand_Into_Their_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Yank Away"), new ScriptedAgent());
            var toxtricity = e.ScenarioActive(0, C(Toxtricity));
            e.ScenarioAttach(toxtricity, C(Psychic));
            e.ScenarioAttach(toxtricity, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.AreEqual(22, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Underworld_Stroll_Puts_A_Supporter_On_The_Bottom_Of_The_Opponents_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Underworld Stroll"), new ScriptedAgent());
            var greavard = e.ScenarioActive(0, C(Greavard));
            e.ScenarioAttach(greavard, C(Psychic));
            e.ScenarioAttach(greavard, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Katy));
            e.ScenarioAddToHand(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(1, opp.Hand.Count);
            Assert.AreEqual("Pidgey", opp.Hand[0].Def.Name);
            Assert.AreEqual("Katy", opp.Deck[opp.Deck.Count - 1].Def.Name);
        }

        [Test]
        public void Poltergeist_Does_60_For_Each_Trainer_In_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Poltergeist"), new ScriptedAgent());
            var banette = e.ScenarioActive(0, C(BanetteEx));
            e.ScenarioAttach(banette, C(Psychic));
            e.ScenarioAttach(banette, C(Psychic));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioAddToHand(1, C(Katy));
            e.ScenarioAddToHand(1, C(Potion));
            e.ScenarioAddToHand(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, dragonite.DamageCounters);
        }

        [Test]
        public void Everlasting_Darkness_Locks_Items_But_Not_Pokemon_Tools()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Everlasting Darkness"), new ScriptedAgent());
            var banette = e.ScenarioActive(0, C(BanetteEx));
            e.ScenarioAttach(banette, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(PokeBall));
            e.ScenarioAddToHand(1, C(VitalityBand));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Play Poké Ball"));
            Assert.IsTrue(Offered(e, 1, "Vitality Band"), "Compendium: a Pokémon Tool isn't an Item");
        }

        [Test]
        public void Triple_Laser_Hits_Every_Opponent_Pokemon_When_They_Have_Fewer_Than_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Triple Laser"), new ScriptedAgent());
            var treads = e.ScenarioActive(0, C(IronTreadsEx));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(treads, C(Psychic));
            var tauros = e.ScenarioActive(1, C(Tauros));
            var pidgey = e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters);
            Assert.AreEqual(3, pidgey.DamageCounters);
        }

        [Test]
        public void Sandstorm_Spray_Discards_2_Energy_And_Shuffles_The_Opponents_Active_Into_Their_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sandstorm Spray"), new ScriptedAgent());
            var sandaconda = e.ScenarioActive(0, C(Sandaconda));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(sandaconda, C(Fighting));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, sandaconda.Energy.Count);
            Assert.AreEqual("Pidgey", e.State.Players[1].Active.Name);
            Assert.AreEqual(22, e.State.Players[1].Deck.Count, "Tauros and its Energy");
        }

        [Test]
        public void Expert_Nurturer_Evolves_A_Pokemon_Put_Into_Play_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Pidgey", "Attack: Expert Nurturer"), new ScriptedAgent());
            var indeedee = e.ScenarioActive(0, C(Indeedee));
            e.ScenarioAttach(indeedee, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Pidgey));
            e.ScenarioAddToDeck(0, C(Pidgeotto), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual("Pidgeotto", e.State.Players[0].Bench[0].Name, "Compendium (2023-03-30)");
        }

        [Test]
        public void Sweeping_Tackle_Does_20_Less_For_Each_Damage_Counter_On_Cetitan()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sweeping Tackle"), new ScriptedAgent());
            var cetitan = e.ScenarioActive(0, C(Cetitan));
            e.ScenarioAttach(cetitan, C(Water));
            e.ScenarioAttach(cetitan, C(Psychic));
            e.ScenarioAttach(cetitan, C(Psychic));
            cetitan.DamageCounters = 3;
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(14, dragonite.DamageCounters, "200 - 60");
        }

        [Test]
        public void Energy_Crush_Counts_Energy_On_All_The_Opponents_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Energy Crush"), new ScriptedAgent());
            var magnezone = e.ScenarioActive(0, C(MagnezoneEx));
            e.ScenarioAttach(magnezone, C(Lightning));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioAttach(dragonite, C(Psychic));
            e.ScenarioAttach(dragonite, C(Psychic));
            var pidgey = e.ScenarioBench(1, C(Pidgey));
            e.ScenarioAttach(pidgey, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, dragonite.DamageCounters);
        }

        [Test]
        public void Payback_Does_90_More_When_The_Opponent_Has_1_Prize_Card_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Payback"), new ScriptedAgent());
            var krokorok = e.ScenarioActive(0, C(Krokorok));
            e.ScenarioAttach(krokorok, C(Fighting));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            while (e.State.Players[1].Prizes.Count > 1) e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, dragonite.DamageCounters);
        }

        // ================================================================== Pokémon Tools

        [Test]
        public void Exp_Share_Moves_A_Basic_Energy_From_The_Knocked_Out_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gigantic Tusks"), new ScriptedAgent());
            var tusk = e.ScenarioActive(0, C(GreatTuskEx));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tusk, C(Fighting));
            var pidgey = e.ScenarioActive(1, C(Pidgey));
            e.ScenarioAttach(pidgey, C(Psychic));
            e.ScenarioAttach(pidgey, C(Psychic));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(ExpShare));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(tauros, e.State.Players[1].Active, "Pidgey was Knocked Out");
            Assert.AreEqual(1, tauros.Energy.Count, "1 basic Energy card moved");
        }

        [Test]
        public void Rocky_Helmet_Puts_2_Damage_Counters_On_The_Attacking_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Tauros));
            var holder = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Tauros));
            e.ScenarioAttachTool(holder, C(RockyHelmet));
            e.ScenarioAttachTool(benched, C(RockyHelmet));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, holder, 30);
            Assert.AreEqual(2, attacker.DamageCounters);
            e.DealAttackDamage(attacker, benched, 30);
            Assert.AreEqual(2, attacker.DamageCounters, "only while Active");
        }

        [Test]
        public void Defiance_Band_Adds_30_Only_With_More_Prize_Cards_Left_Than_The_Opponent()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(DefianceBand));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, dragonite, 30);
            Assert.AreEqual(3, dragonite.DamageCounters, "6 Prize cards each");
            e.State.Players[1].Prizes.RemoveAt(0);
            e.DealAttackDamage(tauros, dragonite, 30);
            Assert.AreEqual(9, dragonite.DamageCounters);
        }

        [Test]
        public void Vitality_Band_Adds_10_And_Rock_Chestplate_Covers_Fighting_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(VitalityBand));
            var klawf = e.ScenarioActive(1, C(Klawf));
            e.ScenarioAttachTool(klawf, C(RockChestplate));
            var benchTauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioAttachTool(benchTauros, C(RockChestplate));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, klawf, 50);
            Assert.AreEqual(3, klawf.DamageCounters, "50 + 10 - 30");
            e.SwitchActive(1, benchTauros);
            e.DealAttackDamage(tauros, benchTauros, 50);
            Assert.AreEqual(6, benchTauros.DamageCounters, "not a Fighting Pokémon");
        }

        [Test]
        public void Resolute_Heart_Keeps_10_HP_When_Calamity_Storm_Discards_Lively_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Calamity Storm", "Yes"), new ScriptedAgent());
            var moon = e.ScenarioActive(0, C("SVP-67"));
            e.ScenarioAttach(moon, C(Darkness));
            e.ScenarioAttach(moon, C(Darkness));
            e.ScenarioAttach(moon, C(Psychic));
            e.ScenarioAttachTool(moon, C(VitalityBand));
            var pikachu = e.ScenarioActive(1, C("SSP-57"));
            e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetStadium(1, C("SSP-180"));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(230, e.MaxHp(pikachu));
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.AreSame(pikachu, e.State.Players[1].Active, "Compendium (2024-11-07): 230 damage, Resolute Heart");
            Assert.AreEqual(19, pikachu.DamageCounters, "10 HP left after the Stadium goes");

            var plain = Scenario(new ScriptedAgent().Then("Attack: Calamity Storm", "Yes"), new ScriptedAgent());
            var moon2 = plain.ScenarioActive(0, C("SVP-67"));
            plain.ScenarioAttach(moon2, C(Darkness));
            plain.ScenarioAttach(moon2, C(Darkness));
            plain.ScenarioAttach(moon2, C(Psychic));
            var pikachu2 = plain.ScenarioActive(1, C("SSP-57"));
            plain.ScenarioBench(1, C(Pidgey));
            plain.ScenarioSetStadium(1, C("SSP-180"));
            plain.ScenarioSetTurn(0, 3);
            Step(plain);
            Assert.IsFalse(plain.State.Players[1].InPlay().Contains(pikachu2), "220 damage: not saved, then Knocked Out by the HP drop");
        }

        // ================================================================== Trainers

        [Test]
        public void Katy_Draws_8_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Katy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Katy));
            e.ScenarioAddToHand(0, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            bool over = e.MainPhaseStep();
            Assert.IsTrue(over, "your turn ends");
            Assert.AreEqual(8, e.State.Players[0].Hand.Count);
            Assert.AreEqual(13, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Youngster_Shuffles_The_Hand_In_And_Draws_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Youngster"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Youngster));
            e.ScenarioAddToHand(0, C(Pidgey));
            e.ScenarioAddToHand(0, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(17, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Miriam_Shuffles_Pokemon_From_The_Discard_And_Draws_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Miriam", "Pidgey|Pidgey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Miriam));
            e.ScenarioAddToDiscard(0, C(Pidgey), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(0, p.Discard.Count(c => c.Def.Name == "Pidgey"));
            Assert.AreEqual(3, p.Hand.Count);
            Assert.AreEqual(19, p.Deck.Count);
        }

        [Test]
        public void Pal_Pad_Needs_A_Supporter_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pal Pad"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(PalPad));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Pal Pad"));
            e.ScenarioAddToDiscard(0, C(Katy), 1);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(1, p.Deck.Count(c => c.Def.Name == "Katy"));
            Assert.AreEqual(0, p.Discard.Count(c => c.Def.Name == "Katy"));
        }

        [Test]
        public void Picnic_Basket_Heals_Every_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Picnic Basket"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            var myBench = e.ScenarioBench(0, C(Pidgey));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(PicnicBasket));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Picnic Basket"), "nothing to heal");
            mine.DamageCounters = 5;
            myBench.DamageCounters = 1;
            theirs.DamageCounters = 2;
            Step(e);
            Assert.AreEqual(2, mine.DamageCounters);
            Assert.AreEqual(0, myBench.DamageCounters);
            Assert.AreEqual(0, theirs.DamageCounters, "the opponent's too");
        }

        [Test]
        public void Jacq_Finds_Up_To_2_Evolution_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Jacq", "Pidgeotto|Spewpa"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Jacq));
            e.ScenarioAddToDeck(0, C(Pidgeotto), 1);
            e.ScenarioAddToDeck(0, C(Spewpa), 1);
            e.ScenarioAddToDeck(0, C(Pidgey), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(2, hand.Count);
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Pidgeotto") && hand.Any(c => c.Def.Name == "Spewpa"));
        }

        [Test]
        public void Mesagoza_Is_Used_Once_During_Each_Players_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Mesagoza"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(Mesagoza));
            e.ScenarioAddToDeck(0, C(Pidgey), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Use Mesagoza"));
            Step(e);
            Assert.IsFalse(Offered(e, 0, "Use Mesagoza"), "once during your turn");
            NextTurn(e);
            Assert.IsTrue(Offered(e, 1, "Use Mesagoza"), "the other player gets their own use");
        }
    }
}
