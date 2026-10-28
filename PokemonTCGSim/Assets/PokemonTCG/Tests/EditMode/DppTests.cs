using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v136: DP Black Star Promos (DPP DP01-DP56): the prints, the Arceus deck rule (your ruling: any number, every format), the
    /// new engine rules (Endless Darkness, Time Skip, Heat Metal, Heat Wave, Enigma Berry, Omniscient) and the other Poké-Powers,
    /// Poké-Bodies and attacks with effects.
    /// </summary>
    public class DppTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        const string TropicalWind = "DPP-5";
        const string DialgaLvX = "DPP-17";
        const string DarkraiLvX = "DPP-19";
        const string DarkraiBerry = "DPP-24";
        const string Dialga = "DPP-26";
        const string MewtwoLvX = "DPP-28";
        const string RegigigasLvX = "DPP-30";
        const string HeatranLvX = "DPP-31";
        const string PorygonZ = "DPP-35";
        const string GiratinaLvX = "DPP-38";
        const string ProbopassG = "DPP-43";
        const string Arceus = "DPP-50";
        const string Cresselia = "DPP-51";
        const string Darkrai = "DPP-52";
        const string ArceusOmniscient = "DPP-53";
        const string UltimateZone = "DPP-55";
        const string ArceusMeteor = "DPP-56";
        const string EmpoleonLvX = "DPP-11";
        const string Hoothoot = "HSP-5";
        const string Entei = "HSP-20";
        const string Hitmonlee = "HSP-25";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";

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

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Promos_Are_In_The_Pool_With_Their_Prints_Linked()
        {
            Assert.AreEqual(51, db.Cards.Count(d => d.SetCode == "DPP"));
            Assert.IsTrue(db.Cards.Where(d => d.SetCode == "DPP").All(d => d.UnlimitedOnly));
            Assert.AreEqual(TropicalWind, C("DPP-25").BaseKey);
            Assert.AreEqual(TropicalWind, C("DPP-48").BaseKey);
            Assert.AreEqual(DialgaLvX, C("DPP-37").BaseKey);
            Assert.AreEqual("30C-CC12", C("DPP-18").BaseKey, "Palkia LV.X");
            Assert.AreEqual("CEL-CC18", C("DPP-46").BaseKey, "Garchomp C LV.X");
            Assert.IsTrue(C(DarkraiLvX).IsLevelUp);
            Assert.IsTrue(C("DPP-41").IsPokemonSP, "Toxicroak G");
            Assert.AreEqual(0, C(EmpoleonLvX).Attacks[0].Damage, "Hydro Impact: one hit to the chosen Pokémon");
        }

        [Test]
        public void Arceus_May_Be_Played_In_Any_Number_In_Every_Format()
        {
            var deck = new List<CardDef>();
            for (int i = 0; i < 8; i++) deck.Add(C(Arceus));
            for (int i = 0; i < 6; i++) deck.Add(C(ArceusMeteor));
            while (deck.Count < 60) deck.Add(C(Psychic));
            CollectionAssert.IsEmpty(Decklist.Validate(deck, null), "8 Arceus and 6 Arceus LV.X");
            Assert.IsFalse(Decklist.Validate(deck, null, 60, 1).Any(p => p.Contains("Arceus")), "a singleton format too (your ruling)");
            deck[59] = C(Hitmonlee);
            deck[58] = C(Hitmonlee);
            Assert.IsTrue(Decklist.Validate(deck, null, 60, 1).Any(p => p.Contains("Hitmonlee")), "other cards keep the limit");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Tropical_Wind_Heals_Or_Puts_Both_Actives_To_Sleep()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Tropical Wind"), new ScriptedAgent());
                var mine = e.ScenarioActive(0, C(Chansey));
                var theirs = e.ScenarioActive(1, C(Chansey));
                mine.DamageCounters = 3;
                theirs.DamageCounters = 1;
                e.ScenarioAddToHand(0, C(TropicalWind));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 1 : 3, mine.DamageCounters);
                Assert.AreEqual(heads ? 0 : 1, theirs.DamageCounters);
                Assert.AreEqual(!heads, mine.Has(SpecialCondition.Asleep) && theirs.Has(SpecialCondition.Asleep));
            }
        }

        [Test]
        public void Ultimate_Zone_Moves_Energy_From_The_Bench_To_The_Active_Arceus()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ultimate Zone"), new ScriptedAgent());
            var arceus = e.ScenarioActive(0, C(Arceus));
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(UltimateZone));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, arceus.Energy.Count);
            Assert.AreEqual(0, lee.Energy.Count);
        }

        // ------------------------------------------------------------------ New engine rules

        [Test]
        public void Endless_Darkness_Knocks_Out_On_Two_Tails_And_Dark_Shadow_Adds_10_Per_Darkness_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Endless Darkness"), new ScriptedAgent());
            var darkrai = e.ScenarioActive(0, C(Darkrai));
            e.ScenarioEvolve(darkrai, C(DarkraiLvX));
            Attach(e, darkrai, Darkness, 3);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, blissey.DamageCounters, "40 + 3 x 10 (Dark Shadow)");
            Assert.IsTrue(blissey.Has(SpecialCondition.Asleep));
            e.State.Rng = new Rng(SeedFor(false, false));
            NextTurn(e);
            Assert.IsNull(e.State.FindPokemon(blissey.Uid), "both coins tails: Knocked Out");
        }

        [Test]
        public void Time_Skip_Ends_The_Opponents_Next_Turn_After_They_Draw_On_Two_Tails()
        {
            var foe = new ScriptedAgent();
            var e = Scenario(new ScriptedAgent().Then("Use Time Skip"), foe);
            var dialga = e.ScenarioActive(0, C(Dialga));
            e.ScenarioEvolve(dialga, C(DialgaLvX));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false, false));
            Step(e);
            NextTurn(e);
            int hand = e.State.Players[1].Hand.Count;
            int asked = foe.Seen.Count;
            Assert.IsTrue(e.MainPhaseStep(), "the turn is over");
            Assert.AreEqual(asked, foe.Seen.Count, "no action was offered");
            Assert.AreEqual(1, hand, "they drew first");
        }

        [Test]
        public void Time_Skip_Ends_Your_Own_Turn_On_Two_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Time Skip"), new ScriptedAgent());
            var dialga = e.ScenarioActive(0, C(Dialga));
            e.ScenarioEvolve(dialga, C(DialgaLvX));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true));
            Assert.IsTrue(e.MainPhaseStep());
        }

        [Test]
        public void Heat_Metal_Keeps_The_Opponents_Pokemon_Burned()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(HeatranLvX));
            var foe = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(foe, SpecialCondition.Burned);
            Assert.IsTrue(e.BurnRecoveryBlocked(foe));
            var mine = e.State.Players[0].Active;
            Assert.IsFalse(e.BurnRecoveryBlocked(mine), "only the opponent's Pokémon");
        }

        [Test]
        public void Heat_Wave_Attaches_Back_2_Energy_Your_Fire_Active_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flare Blitz", "Yes", "Fire Energy|Fire Energy"), new ScriptedAgent());
            var entei = e.ScenarioActive(0, C(Entei));
            Attach(e, entei, Fire, 3);
            e.ScenarioBench(0, C(HeatranLvX));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, entei.Energy.Count, "Flare Blitz discarded them");
            e.EndTurn();
            Assert.AreEqual(2, entei.Energy.Count, "Heat Wave");
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Enigma_Berry_Removes_4_Damage_Counters_After_A_Fighting_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var darkrai = e.ScenarioActive(1, C(DarkraiBerry));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, darkrai.DamageCounters, "20 + 20 Weakness");
            e.EndTurn();
            Assert.AreEqual(0, darkrai.DamageCounters);
        }

        [Test]
        public void Omniscient_Uses_The_Attacks_Of_Another_Arceus_LvX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Arceus));
            e.ScenarioEvolve(active, C(ArceusOmniscient));
            var benched = e.ScenarioBench(0, C(Arceus));
            e.ScenarioEvolve(benched, C(ArceusMeteor));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            var names = e.AttacksOf(active).Select(a => a.Source.Attacks[a.Index].Name).ToList();
            CollectionAssert.Contains(names, "Meteor Blast", "the other Arceus LV.X's attack (same name as this card)");
            CollectionAssert.Contains(names, "Judgment", "the Arceus under it");
            Assert.AreEqual(1, names.Count(n => n == "Judgment"), "the same attack isn't offered twice");
        }

        // ------------------------------------------------------------------ Poké-Bodies

        [Test]
        public void Invisible_Tentacles_Costs_A_Card_To_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioActive(1, C(GiratinaLvX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Kick"), "no card to discard: no attack");
            e.ScenarioAddToHand(0, C(Psychic));
            Assert.IsTrue(Offered(e, 0, "Attack: Kick"));
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Psybarrier_Stops_Attacks_From_Unevolved_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kick"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var mewtwo = e.ScenarioActive(1, C(MewtwoLvX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, mewtwo.DamageCounters);
        }

        [Test]
        public void Lunar_Aura_And_Darkness_Aura_Need_Each_Other()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hypnoblast"), new ScriptedAgent());
            var darkrai = e.ScenarioActive(0, C(Darkrai));
            Attach(e, darkrai, Darkness, 3);
            var cresselia = e.ScenarioBench(0, C(Cresselia));
            cresselia.DamageCounters = 2;
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(8, blissey.DamageCounters, "60 + 20 with Cresselia in play");
            NextTurn(e);
            Assert.AreEqual(1, cresselia.DamageCounters, "1 removed between turns with Darkrai in play");
        }

        // ------------------------------------------------------------------ Poké-Powers and attacks

        [Test]
        public void Supreme_Command_Sets_Aside_2_Cards_Until_The_End_Of_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Supreme Command", "2"), new ScriptedAgent());
            e.ScenarioActive(0, C(EmpoleonLvX));
            e.ScenarioActive(1, C(Chansey));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.AreEqual(2, e.State.Players[1].SetAsideHand.Count);
            NextTurn(e);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count, "1 left + the card drawn");
            e.EndTurn();
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
            Assert.AreEqual(0, e.State.Players[1].SetAsideHand.Count);
        }

        [Test]
        public void Sacrifice_Knocks_Out_Your_Pokemon_Then_Recharges_And_Heals()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Sacrifice", "Hitmonlee"), new ScriptedAgent());
            var regigigas = e.ScenarioActive(0, C(RegigigasLvX));
            regigigas.DamageCounters = 9;
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(lee.Uid));
            Assert.AreEqual(2, regigigas.Energy.Count, "Hitmonlee's 2 Fighting Energy");
            Assert.AreEqual(1, regigigas.DamageCounters);
        }

        [Test]
        public void Learning_Levels_Up_A_Benched_Pokemon_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Learning"), new ScriptedAgent());
            e.ScenarioActive(0, C(PorygonZ));
            var dialga = e.ScenarioBench(0, C(Dialga));
            e.ScenarioAddToDeck(0, C(DialgaLvX), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(DialgaLvX, dialga.Def.Key, "a Benched Pokémon, played this game with no timing limits (your ruling)");
        }

        [Test]
        public void Nose_Unit_Hits_Only_Pokemon_With_Poke_Bodies()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Nose Unit"), new ScriptedAgent());
            var probopass = e.ScenarioActive(0, C(ProbopassG));
            Attach(e, probopass, Metal, 2);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            var hoothoot = e.ScenarioBench(1, C(Hoothoot));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, lee.DamageCounters);
            Assert.AreEqual(2, hoothoot.DamageCounters, "Insomnia is a Poké-Body");
        }

        [Test]
        public void Flash_Cannon_Shuffles_The_Top_Evolution_Card_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flash Cannon", "Yes"), new ScriptedAgent());
            var dialga = e.ScenarioActive(0, C(Dialga));
            Attach(e, dialga, Metal, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioEvolve(chansey, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[1].Deck.Count;
            Step(e);
            Assert.AreEqual("Chansey", chansey.Name);
            Assert.AreEqual(deck + 1, e.State.Players[1].Deck.Count);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "the Energy went back to the hand");
        }
    }
}
