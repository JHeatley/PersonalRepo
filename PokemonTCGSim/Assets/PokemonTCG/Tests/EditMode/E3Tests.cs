using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v147: Skyridge (E3): the card data and prints, the new engine pieces (Ancient Wind, Dark Gaze, Mirror Coat, Primal Aura and
    /// Primal Stare, Buried Fossil, Synchronicity, Mirage Stadium, Retro Cave), the Technical Machines, Tools, Stadiums and special
    /// Energy, and your rulings (Lonely Fang, Empathic Healing, Streaming Mantle).
    /// </summary>
    public class E3Tests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Fighting = "MEE-14";
        const string Grass = "MEE-9";
        const string Darkness = "MEE-15";
        const string Psychic = "MEE-13";

        const string Aerodactyl = "E3-1";
        const string Dewgong = "E3-7";
        const string Flareon = "E3-8";
        const string HoundoomE3 = "E3-12";
        const string Kabutops = "E3-14";
        const string Magcargo = "E3-18";
        const string Omastar = "E3-23";
        const string Steelix = "E3-31";
        const string Umbreon = "E3-32";
        const string Wigglytuff = "E3-34";
        const string Xatu = "E3-35";
        const string Kabuto = "E3-37";
        const string Wobbuffet = "E3-45";
        const string BuriedFossil = "E3-47";
        const string Ditto = "E3-51";
        const string Natu = "E3-80";
        const string Seel = "E3-95";
        const string Shuckle = "E3-96";
        const string Slugma = "E3-98";
        const string Zubat = "E3-118";
        const string HyperPotion = "E3-127";
        const string MiracleSphereAlpha = "E3-129";
        const string MirageStadium = "E3-132";
        const string MysteryPlateGamma = "E3-135";
        const string StarPiece = "E3-139";
        const string UndergroundLake = "E3-141";
        const string BounceEnergy = "E3-142";
        const string RetroEnergy = "E3-144";
        const string Celebi = "E3-145";
        const string Chansey = "E2-69";
        const string Horsea = "E2-84";
        const string Seadra = "E2-58";
        const string DarknessCube = "E2-119";

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
        public void Skyridge_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "E3").ToList();
            Assert.AreEqual(145, set.Count, "182 cards minus the 32 holo prints and 5 Trainers already in the pool");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual(182, db.All.Count(d => d.SetCode == "E3"));
            Assert.AreEqual("E3-2", db.MainPrinting(db.Get("E3-H1")).Key);
            Assert.AreEqual("E3-35", db.MainPrinting(db.Get("E3-H32")).Key);
            Assert.AreEqual("CES-124", db.MainPrinting(db.Get("E3-121")).Key);
            Assert.AreEqual("HIF-SV83", db.MainPrinting(db.Get("E3-125")).Key);
            Assert.AreEqual("CES-131", db.MainPrinting(db.Get("E3-126")).Key);
            Assert.AreEqual("CES-138", db.MainPrinting(db.Get("E3-128")).Key);
            Assert.AreEqual("CES-150", db.MainPrinting(db.Get("E3-140")).Key);
            Assert.AreEqual("E3-127", db.MainPrinting(db.Get("E3-127")).Key, "Skyridge's own Hyper Potion");
            Assert.IsTrue(C("E3-48").IsBaby && C("E3-67").IsBaby);
            Assert.AreEqual("Nidoran♀", C("E3-83").EvolvesFrom);
            Assert.AreEqual("Ho-Oh", C("E3-149").Name);
            Assert.IsTrue(C(MiracleSphereAlpha).IsTool && C(MysteryPlateGamma).IsTool);
        }

        // ------------------------------------------------------------------ Poké-Powers and Poké-Bodies

        [Test]
        public void Ancient_Wind_Ignores_Every_Poke_Body_This_Turn()
        {
            foreach (bool wind in new[] { false, true })
            {
                var agent = wind ? new ScriptedAgent().Then("Use Ancient Wind", "Attack: Rising Lunge") : new ScriptedAgent().Then("Attack: Rising Lunge");
                var e = Scenario(agent, new ScriptedAgent());
                var aero = e.ScenarioActive(0, C(Aerodactyl));
                Attach(e, aero, Fighting, 2);
                var shuckle = e.ScenarioActive(1, C(Shuckle));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(false));
                if (wind) Step(e);
                Step(e);
                Assert.AreEqual(wind ? 2 : 0, shuckle.DamageCounters, wind ? "Vase Body ignored" : "Vase Body: 20 less");
            }
        }

        [Test]
        public void Dark_Gaze_Stops_Benched_Poke_Powers_While_Umbreon_Is_Active()
        {
            foreach (bool active in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                if (active)
                {
                    e.ScenarioActive(0, C(Umbreon));
                    e.ScenarioBench(0, C(Wigglytuff));
                }
                else
                {
                    e.ScenarioActive(0, C(Chansey));
                    e.ScenarioBench(0, C(Wigglytuff));
                    e.ScenarioBench(0, C(Umbreon));
                }
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!active, Offered(e, 0, "Use Good Neighbor"));
            }
        }

        [Test]
        public void Mirror_Coat_Gives_The_Attacker_The_Same_Poison()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Poison Spray"), new ScriptedAgent());
            var zubat = e.ScenarioActive(0, C(Zubat));
            Attach(e, zubat, Grass, 2);
            var wobbuffet = e.ScenarioActive(1, C(Wobbuffet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(wobbuffet.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(zubat.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Primal_Aura_And_Primal_Stare_Stop_Evolving_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kabutops = e.ScenarioActive(0, C(BuriedFossil));
            e.ScenarioEvolve(kabutops, C(Kabuto));
            e.ScenarioEvolve(kabutops, C(Kabutops));
            var myBench = e.ScenarioBench(0, C(Seel));
            var theirActive = e.ScenarioActive(1, C(Seel));
            var theirBench = e.ScenarioBench(1, C(Seel));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.EvolutionFromHandBlocked(myBench));
            Assert.IsTrue(e.EvolutionFromHandBlocked(theirBench));
            Assert.IsFalse(e.EvolutionFromHandBlocked(theirActive), "Primal Aura: Benched Pokémon only");

            var f = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var omastar = f.ScenarioActive(0, C(BuriedFossil));
            f.ScenarioEvolve(omastar, C("E3-41"));
            f.ScenarioEvolve(omastar, C(Omastar));
            var bench = f.ScenarioBench(1, C(Seel));
            var active = f.ScenarioActive(1, C(Seel));
            f.ScenarioSetTurn(0, 3);
            Assert.IsTrue(f.EvolutionFromHandBlocked(active));
            Assert.IsFalse(f.EvolutionFromHandBlocked(bench));
            Assert.IsFalse(f.EvolutionFromHandBlocked(omastar), "only the opponent's Active Pokémon");
        }

        [Test]
        public void Buried_Fossil_Evolves_Into_Kabuto_And_Has_No_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var fossil = e.ScenarioActive(0, C(BuriedFossil));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.CanEvolveInto(fossil, C(Kabuto), true));
            Assert.IsFalse(e.CanEvolveInto(fossil, C(Seadra), true));
            Assert.IsFalse(e.ApplyCondition(fossil, SpecialCondition.Poisoned));
        }

        [Test]
        public void Synchronicity_Lets_Xatu_Take_Any_Technical_Machine()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var xatu = e.ScenarioActive(0, C(Natu));
            e.ScenarioEvolve(xatu, C(Xatu));
            var natu = e.ScenarioBench(0, C(Natu));
            var cube = e.ScenarioAddToHand(0, C(DarknessCube));
            var sphere = e.ScenarioAddToHand(0, C(MiracleSphereAlpha));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.CanAttachToolTo(cube, xatu));
            Assert.IsTrue(e.CanAttachToolTo(sphere, xatu));
            Assert.IsFalse(e.CanAttachToolTo(cube, natu));
        }

        [Test]
        public void Rare_Metal_And_Prismatic_Body_Change_What_Basic_Energy_Provides()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var steelix = e.ScenarioActive(0, C(Steelix));
            Attach(e, steelix, Fire, 1);
            var ditto = e.ScenarioBench(0, C(Ditto));
            Attach(e, ditto, Fire, 1);
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(1, e.EnergyCountOfType(steelix, EnergyType.Metal));
            Assert.AreEqual(0, e.EnergyCountOfType(steelix, EnergyType.Fire));
            Assert.IsTrue(e.CanPayCost(ditto, new List<EnergyType> { EnergyType.Water }));
            Assert.IsFalse(e.CanPayCost(ditto, new List<EnergyType> { EnergyType.Water, EnergyType.Psychic }), "1 Energy at a time");
        }

        [Test]
        public void Self_Healing_Cures_Flareon_When_Fire_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Fire Energy to Flareon"), new ScriptedAgent());
            var flareon = e.ScenarioActive(0, C(Flareon));
            e.ApplyCondition(flareon, SpecialCondition.Poisoned);
            e.ApplyCondition(flareon, SpecialCondition.Confused);
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, flareon.Conditions);
        }

        // ------------------------------------------------------------------ Attacks and your rulings

        [Test]
        public void Lonely_Fang_Never_Goes_Below_30()
        {
            foreach (bool more in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Lonely Fang"), new ScriptedAgent());
                var houndoom = e.ScenarioActive(0, C(HoundoomE3));
                Attach(e, houndoom, Darkness, 3);
                for (int i = 0; i < (more ? 3 : 1); i++) e.ScenarioBench(0, C(Seel));
                var chansey = e.ScenarioActive(1, C(Chansey));
                for (int i = 0; i < (more ? 1 : 3); i++) e.ScenarioBench(1, C(Seel));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(more ? 3 : 7, chansey.DamageCounters, more ? "your ruling: just 30" : "30 + 2 x 20");
            }
        }

        [Test]
        public void Empathic_Healing_Heals_Both_Players_Pokemon_Of_Celebis_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Empathic Healing"), new ScriptedAgent());
            var celebi = e.ScenarioActive(0, C(Celebi));
            Attach(e, celebi, Grass, 1);
            Attach(e, celebi, Water, 1);
            celebi.DamageCounters = 2;
            var mine = e.ScenarioBench(0, C(Chansey));
            mine.DamageCounters = 2;
            var seel = e.ScenarioBench(0, C(Seel));
            seel.DamageCounters = 2;
            var theirs = e.ScenarioActive(1, C(Chansey));
            theirs.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, celebi.DamageCounters);
            Assert.AreEqual(0, mine.DamageCounters);
            Assert.AreEqual(1, theirs.DamageCounters, "your ruling: the opponent's Colorless Pokémon too");
            Assert.AreEqual(2, seel.DamageCounters, "a Water Pokémon isn't Celebi's type");
        }

        [Test]
        public void Streaming_Mantle_Shuffles_In_As_Many_Basic_Energy_As_There_Are()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve Slugma", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Slugma));
            e.ScenarioAddToHand(0, C(Magcargo));
            e.ScenarioActive(1, C(Chansey));
            var p1 = e.State.Players[1];
            p1.Deck.Clear();
            e.ScenarioAddToDeck(1, C(Chansey), 5);
            e.ScenarioAddToDiscard(1, C(Fire), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(20, e.State.Players[0].Deck.Count, "3 discarded, 3 basic Energy back");
            Assert.AreEqual(4, p1.Deck.Count, "your ruling: the opponent has only 2 basic Energy, so 2 go back");
            Assert.AreEqual(0, p1.Discard.Count(c => c.Def.IsBasicEnergy));
        }

        [Test]
        public void Freeze_Lock_Stops_Energy_From_The_Hand_To_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Freeze Lock"), new ScriptedAgent());
            var dewgong = e.ScenarioActive(0, C(Seel));
            e.ScenarioEvolve(dewgong, C(Dewgong));
            Attach(e, dewgong, Water, 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Seel));
            e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Assert.IsFalse(Offered(e, 1, "Attach Water Energy to Chansey"));
            Assert.IsTrue(Offered(e, 1, "Attach Water Energy to Seel"));
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Mirage_Stadium_Tails_Stops_The_Retreat_Without_Discarding()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat Chansey"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Water, 2);
            e.ScenarioBench(0, C(Seel));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(0, C(MirageStadium));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreSame(chansey, e.State.Players[0].Active, "the owner of the Stadium flips too");
            Assert.AreEqual(2, chansey.Energy.Count);
        }

        [Test]
        public void Retro_Cave_Puts_The_Top_Card_On_The_Bottom_Of_The_Deck_At_2_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Retro Cave"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Water, 1);
            e.ScenarioAttachTool(chansey, C(MysteryPlateGamma));
            var seadra = EvolvedSeadra(e, 1, true);
            e.State.Players[1].Prizes.RemoveRange(0, 4);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Horsea", seadra.Name);
            Assert.AreEqual("Seadra", e.State.Players[1].Deck.Last().Def.Name);
        }

        [Test]
        public void Underground_Lake_Benches_A_Kabuto_As_A_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Underground Lake"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToDiscard(0, C(Kabuto), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(UndergroundLake));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(1, bench.Count);
            Assert.AreEqual("Kabuto", bench[0].Name);
            Assert.IsTrue(bench[0].IsBasic);
        }

        [Test]
        public void Star_Piece_Evolves_A_Damaged_Benched_Pokemon_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var horsea = e.ScenarioBench(0, C(Horsea));
            horsea.DamageCounters = 2;
            e.ScenarioAttachTool(horsea, C(StarPiece));
            e.ScenarioAddToDeck(0, C(Seadra), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual("Seadra", horsea.Name);
            Assert.AreEqual(0, horsea.Tools.Count);
        }

        [Test]
        public void Skyridge_Hyper_Potion_Removes_5_Counters_For_2_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hyper Potion", "Water Energy|Water Energy"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Water, 2);
            chansey.DamageCounters = 6;
            e.ScenarioAddToHand(0, C(HyperPotion));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, chansey.DamageCounters);
            Assert.AreEqual(0, chansey.Energy.Count);
        }

        [Test]
        public void Bounce_Energy_Needs_Basic_Energy_And_Returns_One_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Bounce Energy to Chansey"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Water, 1);
            e.ScenarioBench(0, C(Seel));
            e.ScenarioAddToHand(0, C(BounceEnergy));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Bounce Energy to Seel"));
            Step(e);
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.AreEqual(2, e.EnergyCount(chansey));
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Water Energy"));
        }

        [Test]
        public void Retro_Energy_Heals_2_Counters_And_Devolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Retro Energy to Seadra", "Yes"), new ScriptedAgent());
            var seadra = EvolvedSeadra(e, 0, true);
            seadra.DamageCounters = 3;
            e.ScenarioAddToHand(0, C(RetroEnergy));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Horsea", seadra.Name);
            Assert.AreEqual(1, seadra.DamageCounters);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Seadra"));
        }
    }
}
