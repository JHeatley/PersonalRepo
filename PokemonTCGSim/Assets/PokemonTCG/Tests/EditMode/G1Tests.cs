using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v122: Gym Heroes (G1 1-132): the Gym Stadiums (Pewter, Cerulean, Vermilion, Narrow, The Rocket's Training Gym, No Removal
    /// Gym), the owners' Pokémon Powers (Bench Guard, Shell Armor, Strange Barrier, Photosynthesis, Restless Sleep, Rebirth),
    /// Crosscounter and Fire Wall, and the Trainers (Recall, Tickling Machine, Charity, Lt. Surge, Misty's Duel, Blaine's Quiz #1).
    /// </summary>
    public class G1Tests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Fighting = "MEE-14";

        const string RocketsMoltres = "G1-12";
        const string RhydonBrock = "G1-2";
        const string RhyhornBrock = "G1-22";
        const string Hitmonchan = "G1-11";
        const string Scyther = "G1-13";
        const string Cloyster = "G1-29";
        const string Snorlax = "G1-33";
        const string Graveler = "G1-40";
        const string Lickitung = "G1-41";
        const string Dratini = "G1-42";
        const string Oddish = "G1-47";
        const string Seel = "G1-88";
        const string Shellder = "G1-89";
        const string Spearow = "G1-52";
        const string GeodudeTackle = "G1-38";
        const string Mankey = "G1-67";
        const string Onix = "G1-69";
        const string Tangela = "G1-79";
        const string Rattata = "G1-82";
        const string BlainesQuiz = "G1-97";
        const string Charity = "G1-99";
        const string LtSurge = "G1-101";
        const string NoRemovalGym = "G1-103";
        const string RocketsTrainingGym = "G1-104";
        const string CeruleanCityGym = "G1-108";
        const string PewterCityGym = "G1-115";
        const string Recall = "G1-116";
        const string TicklingMachine = "G1-119";
        const string VermilionCityGym = "G1-120";
        const string MistysDuel = "G1-123";
        const string NarrowGym = "G1-124";
        const string EnergyRemoval = "BS-92";

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

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(121, db.Cards.Count(d => d.SetCode == "G1"));
            Assert.AreEqual("G1-98", C("G1-15").BaseKey, "the holo Brock");
            Assert.AreEqual("G1-100", C("G1-16").BaseKey, "Erika (Item, not the later Supporter)");
            Assert.AreEqual("G1-101", C("G1-17").BaseKey, "Lt. Surge");
            Assert.AreEqual(C("G1-102").BaseKey, C("G1-18").BaseKey, "Misty");
            Assert.AreEqual("MEE-14", C("G1-127").BaseKey, "basic Fighting Energy");
            Assert.AreEqual("Brock's Rhyhorn", C(RhydonBrock).EvolvesFrom);
            Assert.IsTrue(C(RhydonBrock).UnlimitedOnly);
            Assert.AreEqual(AbilityDef.PokemonPower, C(Cloyster).Abilities[0].Kind);
        }

        [Test]
        public void Pewter_City_Gym_Ignores_Resistance_For_Brocks_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Horn Toss"), new ScriptedAgent());
            var rhyhorn = e.ScenarioActive(0, C(RhyhornBrock));
            Attach(e, rhyhorn, Fighting, 2);
            var scyther = e.ScenarioActive(1, C(Scyther));
            e.ScenarioSetStadium(0, C(PewterCityGym));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, scyther.DamageCounters, "20, Scyther's Fighting Resistance doesn't apply");
        }

        [Test]
        public void Retreat_Gyms_Change_The_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            var seel = e.ScenarioActive(1, C(Seel));
            e.ScenarioSetStadium(0, C(RocketsTrainingGym));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(rattata));
            Assert.AreEqual(2, e.RetreatCostOf(seel));
            e.ScenarioSetStadium(0, C(CeruleanCityGym));
            Assert.AreEqual(1, e.RetreatCostOf(rattata));
            Assert.AreEqual(0, e.RetreatCostOf(seel), "Misty's Seel pays [C] less");
        }

        [Test]
        public void Narrow_Gym_Sends_The_Fifth_Benched_Pokemon_Back_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Narrow Gym"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(NarrowGym));
            e.ScenarioActive(1, C(Rattata));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Bench.Count);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Key == Mankey));
            Assert.AreEqual(0, e.BenchSpace(1));
            Assert.AreEqual(4, e.BenchSpace(0));
        }

        [Test]
        public void Vermilion_City_Gym_Heads_Does_10_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck", "Yes"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetStadium(0, C(VermilionCityGym));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(3, lickitung.DamageCounters);
            Assert.AreEqual(0, spearow.DamageCounters);
        }

        [Test]
        public void Vermilion_City_Gym_Tails_Hurts_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck", "Yes"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetStadium(0, C(VermilionCityGym));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(2, lickitung.DamageCounters);
            Assert.AreEqual(1, spearow.DamageCounters, "10 damage to itself");
        }

        [Test]
        public void Shell_Armor_And_Strange_Barrier_Reduce_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var cloyster = e.ScenarioActive(1, C(Shellder));
            e.ScenarioEvolve(cloyster, C(Cloyster));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, cloyster.DamageCounters, "20 - 10");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Rock Throw"), new ScriptedAgent());
            var onix = e2.ScenarioActive(0, C(Onix));
            Attach(e2, onix, Fighting, 3);
            var dratini = e2.ScenarioActive(1, C(Dratini));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, dratini.DamageCounters, "30 from a Basic Pokémon becomes 10");
        }

        [Test]
        public void Photosynthesis_Makes_Every_Energy_Grass()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var oddish = e.ScenarioActive(0, C(Oddish));
            Attach(e, oddish, Water, 2);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Attack: Poisonpowder"), "2 Water Energy pay [G][G]");
        }

        [Test]
        public void Restless_Sleep_Puts_2_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            snorlax.Conditions = SpecialCondition.Asleep;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, snorlax.DamageCounters);
            Assert.AreEqual(2, spearow.DamageCounters);
        }

        [Test]
        public void Crosscounter_Heads_Doubles_The_Damage_Back()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Crosscounter"), new ScriptedAgent().Then("Attack: Rock Throw"));
            var hitmonchan = e.ScenarioActive(0, C(Hitmonchan));
            Attach(e, hitmonchan, Fighting, 1);
            var onix = e.ScenarioActive(1, C(Onix));
            Attach(e, onix, Fighting, 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(3, hitmonchan.DamageCounters);
            Assert.AreEqual(6, onix.DamageCounters, "double the 30");
        }

        [Test]
        public void Fire_Wall_Hits_Back_For_10()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fire Wall"), new ScriptedAgent().Then("Attack: Tongue Slap"));
            var moltres = e.ScenarioActive(0, C(RocketsMoltres));
            Attach(e, moltres, Fire, 3);
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            Attach(e, lickitung, Fire, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, lickitung.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(2, moltres.DamageCounters);
            Assert.AreEqual(5, lickitung.DamageCounters);
        }

        [Test]
        public void Rebirth_Returns_Rockets_Moltres_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var moltres = e.ScenarioActive(1, C(RocketsMoltres));
            moltres.DamageCounters = 5;
            e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Key == RocketsMoltres), "back in the hand");
            Assert.IsFalse(e.State.Players[1].Discard.Any(c => c.Def.Key == RocketsMoltres));
        }

        [Test]
        public void Bench_Guard_Takes_10_Of_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stretch Vine", "Lt. Surge's Rattata"), new ScriptedAgent().Then("Yes"));
            var tangela = e.ScenarioActive(0, C(Tangela));
            Attach(e, tangela, Grass, 3);
            e.ScenarioActive(1, C(Lickitung));
            var rhydon = e.ScenarioBench(1, C(RhyhornBrock));
            e.ScenarioEvolve(rhydon, C(RhydonBrock));
            var rattata = e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, rattata.DamageCounters);
            Assert.AreEqual(1, rhydon.DamageCounters);
        }

        [Test]
        public void Recall_Uses_The_Attack_Of_The_Card_Under_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Recall", "Attack: Tackle"), new ScriptedAgent());
            var graveler = e.ScenarioActive(0, C(GeodudeTackle));
            e.ScenarioEvolve(graveler, C(Graveler));
            Attach(e, graveler, Fighting, 1);
            e.ScenarioAddToHand(0, C(Recall));
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack: Tackle")));
            Step(e);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack: Tackle")));
            Step(e);
            Assert.AreEqual(2, lickitung.DamageCounters, "Tackle's 10, doubled by Lickitung's Fighting Weakness");
        }

        [Test]
        public void Tickling_Machine_Sets_The_Hand_Aside_Until_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Tickling Machine"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(TicklingMachine));
            e.ScenarioActive(1, C(Rattata));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(0, opp.Hand.Count);
            Assert.AreEqual(3, opp.SetAsideHand.Count);
            NextTurn(e);
            Assert.AreEqual(0, opp.Hand.Count(c => c.Def.Key == Mankey), "still set aside during their turn");
            e.EndTurn();
            Assert.AreEqual(0, opp.SetAsideHand.Count);
            Assert.AreEqual(3, opp.Hand.Count(c => c.Def.Key == Mankey), "back at the end of their turn");
        }

        [Test]
        public void Charity_Goes_Back_To_The_Hand_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Charity"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Charity));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[0].Hand.Any(c => c.Def.Key == Charity));
            e.EndTurn();
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Charity));
        }

        [Test]
        public void Lt_Surge_Puts_A_Basic_Pokemon_In_As_The_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lt. Surge"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(LtSurge));
            e.ScenarioAddToHand(0, C(Lickitung));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(Lickitung, e.State.Players[0].Active.Def.Key);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(rattata));
        }

        [Test]
        public void Mistys_Duel_Gives_The_Winner_5_New_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Misty's Duel"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(MistysDuel));
            e.ScenarioAddToHand(0, C(Mankey));
            e.ScenarioAddToHand(0, C(Mankey));
            e.ScenarioActive(1, C(Rattata));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Mankey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            int mine = e.State.Players[0].Hand.Count, theirs = e.State.Players[1].Hand.Count;
            Assert.IsTrue((mine == 5 && theirs == 3) || (mine == 2 && theirs == 5), "one winner: " + mine + " / " + theirs);
        }

        [Test]
        public void Blaines_Quiz_Draws_2_For_Someone_And_Keeps_The_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Blaine's Quiz #1"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(BlainesQuiz));
            e.ScenarioAddToHand(0, C("BS-61"));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            int mine = e.State.Players[0].Hand.Count, theirs = e.State.Players[1].Hand.Count;
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == "BS-61"), "the card goes back to the hand");
            Assert.IsTrue((mine == 3 && theirs == 0) || (mine == 1 && theirs == 2), mine + " / " + theirs);
        }

        [Test]
        public void No_Removal_Gym_Needs_2_Cards_Discarded_For_Energy_Removal()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(EnergyRemoval));
            e.ScenarioAddToHand(0, C(Mankey));
            var theirs = e.ScenarioActive(1, C(Rattata));
            Attach(e, theirs, Water, 1);
            e.ScenarioSetStadium(1, C(NoRemovalGym));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Energy Removal"), "only 1 other card");

            var e2 = Scenario(new ScriptedAgent().Then("Play Energy Removal"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Rattata));
            e2.ScenarioAddToHand(0, C(EnergyRemoval));
            e2.ScenarioAddToHand(0, C(Mankey));
            e2.ScenarioAddToHand(0, C(Mankey));
            var theirs2 = e2.ScenarioActive(1, C(Rattata));
            Attach(e2, theirs2, Water, 1);
            e2.ScenarioSetStadium(1, C(NoRemovalGym));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, theirs2.Energy.Count);
            Assert.AreEqual(0, e2.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e2.State.Players[0].Discard.Count(c => c.Def.Key == Mankey));
        }

        [Test]
        public void Fix_Buzzap_Attaches_Electrode_As_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Buzzap"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            var electrode = e.ScenarioBench(0, C("BS-67"));
            e.ScenarioEvolve(electrode, C("BS-21"));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[0].Bench.Contains(electrode));
            Assert.IsTrue(rattata.Energy.Any(c => c.Def.Key == "BS-21"), "Electrode is attached as Energy");
        }
    }
}
