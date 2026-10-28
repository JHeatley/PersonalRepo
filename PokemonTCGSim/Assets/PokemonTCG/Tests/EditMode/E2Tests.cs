using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v146: Aquapolis (E2): the card data and prints (H1-H32 holos, cards already in the pool), the new engine pieces (Dark Impact,
    /// Fluff, Anti-Lightning, Undersea Ruins), the Cube 01 Technical Machines, the Berries and Time Shard, the special Energy and
    /// your rulings (Crystal Energy, Apricorn Forest, Healing Berry, Pokémon Park).
    /// </summary>
    public class E2Tests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";

        const string Ariados = "E2-3";
        const string Elekid = "E2-9";
        const string HoundoomFire = "E2-14";
        const string HoundoomDark = "E2-15";
        const string Hypno = "E2-16";
        const string Jumpluff = "E2-17";
        const string Jynx = "E2-18";
        const string Zapdos = "E2-44";
        const string Gloom = "E2-49";
        const string Seadra = "E2-58";
        const string Chansey = "E2-69";
        const string Horsea = "E2-84";
        const string Vulpix = "E2-116";
        const string ApricornForest = "E2-118";
        const string DarknessCube = "E2-119";
        const string HealingBerry = "E2-125";
        const string Juggler = "E2-126";
        const string MemoryBerry = "E2-128";
        const string FanClub = "E2-130";
        const string PokemonPark = "E2-131";
        const string TimeShard = "E2-135";
        const string UnderseaRuins = "E2-138";
        const string PowerPlant = "E2-139";
        const string WeaknessGuard = "E2-141";
        const string BoostEnergy = "E2-145";
        const string CrystalEnergy = "E2-146";
        const string Kingdra = "E2-148";
        const string Magby = "E1-17";

        private static int SeedFor(params bool[] flips)
        {
            for (int seed = 1; seed < 100000; seed++)
            {
                var rng = new Rng(seed);
                bool match = true;
                foreach (var heads in flips)
                    if (rng.CoinFlip() != heads) { match = false; break; }
                if (match) return seed;
            }
            throw new InvalidOperationException("no seed for those flips");
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private PokemonInPlay EvolvedSeadra(GameEngine e, int player, bool active)
        {
            var mon = active ? e.ScenarioActive(player, C(Horsea)) : e.ScenarioBench(player, C(Horsea));
            e.ScenarioEvolve(mon, C(Seadra));
            return mon;
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void Aquapolis_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "E2").ToList();
            Assert.AreEqual(143, set.Count, "182 cards minus the 32 holo prints and 7 cards already in the pool");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual(182, db.All.Count(d => d.SetCode == "E2"));
            Assert.AreEqual("E2-1", db.MainPrinting(db.Get("E2-H1")).Key);
            Assert.AreEqual("E2-44", db.MainPrinting(db.Get("E2-H32")).Key);
            Assert.AreEqual("WP-53", db.MainPrinting(db.Get("E2-37")).Key);
            Assert.AreEqual("WP-53", db.MainPrinting(db.Get("E2-H25")).Key);
            Assert.AreEqual("MEG-115", db.MainPrinting(db.Get("E2-120")).Key);
            Assert.AreEqual("N1-104", db.MainPrinting(db.Get("E2-142")).Key);
            Assert.AreEqual("N1-19", db.MainPrinting(db.Get("E2-143")).Key);
            Assert.AreEqual("CES-151", db.MainPrinting(db.Get("E2-144")).Key);
            Assert.AreEqual("CIN-123", db.MainPrinting(db.Get("E2-147")).Key);
            Assert.AreEqual("30C-CC7", db.MainPrinting(db.Get("E2-149")).Key);
            Assert.AreEqual("E2-130", db.MainPrinting(db.Get("E2-130")).Key, "Aquapolis's own Pokémon Fan Club");
            Assert.AreEqual("E2-139", db.MainPrinting(db.Get("E2-139")).Key, "Aquapolis's own Power Plant");
            Assert.IsTrue(C(FanClub).IsSupporter && C(PowerPlant).IsStadium);
            Assert.IsTrue(C(Elekid).IsBaby && C("E2-61").IsBaby && C("E2-63").IsBaby);
            Assert.IsTrue(C(DarknessCube).IsTool && C(WeaknessGuard).IsTool && C(HealingBerry).IsTool);
            Assert.AreEqual("Nidoran♂", C("E2-55").EvolvesFrom);
        }

        // ------------------------------------------------------------------ Poké-Powers and Poké-Bodies

        [Test]
        public void Dark_Impact_Stops_Poke_Powers_Until_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dark Impact"), new ScriptedAgent());
            var houndoom = e.ScenarioActive(0, C(HoundoomDark));
            Attach(e, houndoom, Darkness, 3);
            var hypno = e.ScenarioActive(1, C(Hypno));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, hypno.DamageCounters);
            e.EndTurn();
            e.StartTurn();
            Assert.IsFalse(Offered(e, 1, "Use Sleep Pendulum"));
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Assert.IsTrue(Offered(e, 1, "Use Sleep Pendulum"), "the effect is over");
        }

        [Test]
        public void Fluff_Flips_Once_It_Has_A_Damage_Counter()
        {
            foreach (int counters in new[] { 0, 1 })
                foreach (bool heads in new[] { true, false })
                {
                    var e = Scenario(new ScriptedAgent().Then("Attack: Meditate"), new ScriptedAgent());
                    var jynx = e.ScenarioActive(0, C(Jynx));
                    Attach(e, jynx, Psychic, 2);
                    var jumpluff = e.ScenarioActive(1, C(Jumpluff));
                    jumpluff.DamageCounters = counters;
                    e.ScenarioSetTurn(0, 3);
                    e.State.Rng = new Rng(SeedFor(heads));
                    Step(e);
                    int expected = counters == 0 ? 1 : heads ? 1 : 3;
                    Assert.AreEqual(expected, jumpluff.DamageCounters, counters + " counters, " + (heads ? "heads" : "tails"));
                }
        }

        [Test]
        public void Anti_Lightning_Stops_Lightning_Energy_From_The_Hand_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zapdos = e.ScenarioActive(0, C(Zapdos));
            var lightning = e.ScenarioAddToHand(0, C(Lightning));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Lightning Energy to Zapdos"));
            Assert.IsTrue(Offered(e, 0, "Attach Fire Energy to Zapdos"));
            Assert.IsTrue(Offered(e, 0, "Attach Lightning Energy to Chansey"));
            Assert.IsTrue(e.EnergyFromHandBlockedTo(lightning, zapdos));
        }

        [Test]
        public void Gluey_Slime_Adds_Only_One_Colorless_However_Many_Ariados()
        {
            foreach (int ariados in new[] { 0, 1, 2 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var chansey = e.ScenarioActive(0, C(Chansey));
                if (ariados >= 1) e.ScenarioBench(0, C(Ariados));
                if (ariados >= 2) e.ScenarioBench(1, C(Ariados));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(ariados == 0 ? 2 : 3, e.RetreatCostOf(chansey), ariados + " Ariados");
            }
        }

        [Test]
        public void Enervating_Pollen_Makes_Resistance_Take_Off_Only_10()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var jumpluff = e.ScenarioActive(0, C(Jumpluff));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(30, e.ResistanceAmountOf(jumpluff));
            e.ScenarioBench(1, C(Gloom));
            Assert.AreEqual(10, e.ResistanceAmountOf(jumpluff));
        }

        [Test]
        public void Crystal_Type_Makes_Kingdra_The_Attached_Type_For_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Lightning Energy to Kingdra"), new ScriptedAgent());
            var kingdra = EvolvedSeadra(e, 0, true);
            e.ScenarioEvolve(kingdra, C(Kingdra));
            e.ScenarioAddToHand(0, C(Lightning));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.IsType(kingdra, EnergyType.Colorless));
            Step(e);
            Assert.IsTrue(e.IsType(kingdra, EnergyType.Lightning));
            e.EndTurn();
            e.StartTurn();
            Assert.IsTrue(e.IsType(kingdra, EnergyType.Colorless), "until the end of the turn");
        }

        // ------------------------------------------------------------------ Special Energy

        [Test]
        public void Crystal_Energy_Provides_One_Energy_Of_Any_Attached_Basic_Type()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, CrystalEnergy, 1);
            e.ScenarioActive(1, C(Chansey));
            Assert.IsTrue(e.CanPayCost(chansey, new List<EnergyType> { EnergyType.Colorless }));
            Assert.IsFalse(e.CanPayCost(chansey, new List<EnergyType> { EnergyType.Fire }), "no basic Energy: [C]");
            Attach(e, chansey, Fire, 1);
            Attach(e, chansey, Water, 1);
            Assert.IsTrue(e.CanPayCost(chansey, new List<EnergyType> { EnergyType.Fire, EnergyType.Fire, EnergyType.Water }));
            Assert.IsTrue(e.CanPayCost(chansey, new List<EnergyType> { EnergyType.Fire, EnergyType.Water, EnergyType.Water }));
            Assert.IsFalse(e.CanPayCost(chansey, new List<EnergyType> { EnergyType.Fire, EnergyType.Fire, EnergyType.Water, EnergyType.Water }), "your ruling: only 1 Energy");
            Assert.IsFalse(e.CanPayCost(chansey, new List<EnergyType> { EnergyType.Fire, EnergyType.Water, EnergyType.Psychic }));
        }

        [Test]
        public void Boost_Energy_Goes_On_Evolved_Pokemon_Stops_Retreat_And_Leaves_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Boost Energy to Seadra"), new ScriptedAgent());
            var seadra = EvolvedSeadra(e, 0, true);
            e.ScenarioBench(0, C(Horsea));
            e.ScenarioAddToHand(0, C(BoostEnergy));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Boost Energy to Horsea"));
            Step(e);
            Assert.AreEqual(3, e.EnergyCount(seadra));
            Assert.IsFalse(Offered(e, 0, "Retreat"));
            e.EndTurn();
            Assert.AreEqual(0, seadra.Energy.Count);
        }

        // ------------------------------------------------------------------ Technical Machines and Tools

        [Test]
        public void Darkness_Cube_Goes_On_Darkness_Pokemon_And_Plunder_Discards_Trainers_First()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Plunder"), new ScriptedAgent());
            var dark = e.ScenarioActive(0, C(HoundoomDark));
            var fire = e.ScenarioBench(0, C(HoundoomFire));
            Attach(e, dark, Darkness, 1);
            var cube = e.ScenarioAddToHand(0, C(DarknessCube));
            Assert.IsTrue(e.CanAttachToolTo(cube, dark));
            Assert.IsFalse(e.CanAttachToolTo(cube, fire));
            e.ScenarioAttachTool(dark, C(DarknessCube));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAttachTool(chansey, C(HealingBerry));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.Tools.Count);
            Assert.AreEqual(1, chansey.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(0, dark.Tools.Count, "discarded at the end of your turn");
        }

        [Test]
        public void Weakness_Guard_Lasts_Until_The_End_Of_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Weakness Guard"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(WeaknessGuard));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(chansey));
            Step(e);
            Assert.IsNull(e.WeaknessOf(chansey));
            e.EndTurn();
            Assert.AreEqual(1, chansey.Tools.Count);
            e.StartTurn();
            e.EndTurn();
            Assert.AreEqual(0, chansey.Tools.Count);
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(chansey));
        }

        [Test]
        public void Healing_Berry_Heals_3_Counters_At_20_HP_Or_Less()
        {
            foreach (int counters in new[] { 7, 8 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var chansey = e.ScenarioActive(0, C(Chansey));
                chansey.DamageCounters = counters;
                e.ScenarioAttachTool(chansey, C(HealingBerry));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.EndTurn();
                Assert.AreEqual(counters == 8 ? 5 : 7, chansey.DamageCounters);
                Assert.AreEqual(counters == 8 ? 0 : 1, chansey.Tools.Count);
            }
        }

        [Test]
        public void Memory_Berry_Gives_The_Pre_Evolution_Attacks_And_Goes_After_An_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bubble"), new ScriptedAgent());
            var seadra = EvolvedSeadra(e, 0, true);
            Attach(e, seadra, Water, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Bubble"));
            e.ScenarioAttachTool(seadra, C(MemoryBerry));
            Assert.IsTrue(Offered(e, 0, "Attack: Bubble"));
            Step(e);
            e.EndTurn();
            Assert.AreEqual(0, seadra.Tools.Count);
        }

        [Test]
        public void Time_Shard_Returns_2_Basic_Energy_When_Knocked_Out_By_The_Opponents_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Water Energy|Water Energy"), new ScriptedAgent().Then("Attack: Meditate"));
            var horsea = e.ScenarioActive(0, C(Horsea));
            Attach(e, horsea, Water, 3);
            horsea.DamageCounters = 3;
            e.ScenarioAttachTool(horsea, C(TimeShard));
            e.ScenarioBench(0, C(Chansey));
            var jynx = e.ScenarioActive(1, C(Jynx));
            Attach(e, jynx, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.Name == "Water Energy"));
        }

        // ------------------------------------------------------------------ Supporters

        [Test]
        public void Juggler_Draws_5_After_Discarding_2_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Juggler", "Water Energy|Water Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Juggler));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Water Energy"));
        }

        [Test]
        public void Aquapolis_Pokemon_Fan_Club_Benches_Baby_And_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Fan Club", "Elekid|Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(FanClub));
            e.ScenarioAddToDeck(0, C(Elekid), 1);
            e.ScenarioAddToDeck(0, C(Chansey), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
            Assert.IsTrue(e.State.Players[0].Bench.Any(m => m.Name == "Elekid"));
        }

        // ------------------------------------------------------------------ Stadiums

        [Test]
        public void Undersea_Ruins_Discards_The_Top_Evolution_Card_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Undersea Ruins"), new ScriptedAgent());
            var seadra = EvolvedSeadra(e, 0, true);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(UnderseaRuins));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Horsea", seadra.Name);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Seadra"));
            Assert.IsFalse(Offered(e, 0, "Use Undersea Ruins"), "once during each player's turn");
        }

        [Test]
        public void Apricorn_Forest_Benches_A_Basic_Of_The_Shown_Type_But_No_Baby()
        {
            foreach (bool baby in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Apricorn Forest"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioAddToHand(0, C(Fire));
                e.ScenarioAddToDeck(0, C(baby ? Magby : Vulpix), 1);
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetStadium(0, C(ApricornForest));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(true));
                Step(e);
                Assert.AreEqual(baby ? 0 : 1, e.State.Players[0].Bench.Count, baby ? "your ruling: no Baby Pokémon" : "Vulpix");
            }
        }

        [Test]
        public void Pokemon_Park_Heals_Only_A_Damaged_Benched_Pokemon_And_Then_Is_Used()
        {
            foreach (bool damaged in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attach Water Energy to Horsea"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                var horsea = e.ScenarioBench(0, C(Horsea));
                if (damaged) horsea.DamageCounters = 2;
                e.ScenarioAddToHand(0, C(Water));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetStadium(1, C(PokemonPark));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(damaged ? 1 : 0, horsea.DamageCounters);
                Assert.AreEqual(damaged, e.State.Players[0].NamedLimitUsedOnTurn.ContainsKey("Pokémon Park"), "your ruling: an undamaged Pokémon doesn't use it up");
            }
        }

        [Test]
        public void Aquapolis_Power_Plant_Swaps_A_Basic_Energy_In_The_Hand_For_One_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Power Plant", "Water Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioAddToDiscard(0, C(Water), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(PowerPlant));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.IsTrue(p.Hand.Any(c => c.Def.Name == "Water Energy"));
            Assert.IsTrue(p.Discard.Any(c => c.Def.Name == "Fire Energy"));
        }
    }
}
