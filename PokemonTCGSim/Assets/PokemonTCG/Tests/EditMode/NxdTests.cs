using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v129: Next Destinies (NXD 1-103): the prints, Flame Burst (your ruling: 30 to the Defending Pokémon too), Heal Block, Dark
    /// Aura, Skyarrow Bridge, Pokémon Center, Blazing Mane, Giant Fan, Sporprise, Vengeful Wish, Dark Penalty, Cursed Shadow,
    /// Slippery Soles, Brain Control, Forewarn and Stadium Wave.
    /// </summary>
    public class NxdTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        const string Amoonguss = "NXD-9";
        const string Foongus = "NXD-8";
        const string Arcanine = "NXD-12";
        const string Chandelure = "NXD-20";
        const string Simipour = "NXD-29";
        const string Vanilluxe = "NXD-33";
        const string Jellicent = "NXD-35";
        const string Musharna = "NXD-59";
        const string Beheeyem = "NXD-62";
        const string Weavile = "NXD-70";
        const string Nuzleaf = "NXD-71";
        const string Shiftry = "NXD-72";
        const string Bronzong = "NXD-76";
        const string PokemonCenter = "NXD-90";
        const string SkyarrowBridge = "NXD-91";
        const string CursedChandelure = "NXD-101";
        const string Hydreigon = "NXD-103";
        const string Pawniard = "DEX-78";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string Eevee = "DEX-84";
        const string DarkClaw = "DEX-92";
        const string Hooligans = "DEX-95";
        const string TwistMountain = "DEX-101";

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

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(75, db.Cards.Count(d => d.SetCode == "NXD"));
            Assert.AreEqual("LTR-RC21", C("NXD-5").BaseKey, "Shaymin-EX");
            Assert.AreEqual("CEL-CC20", C("NXD-21").BaseKey, "Reshiram");
            Assert.AreEqual("DEX-109", C("NXD-57").BaseKey, "Gardevoir");
            Assert.AreEqual("SLG-69", C("NXD-92").BaseKey, "Double Colorless Energy");
            Assert.AreEqual("NXD-82", C("NXD-99").BaseKey, "the full art Regigigas-EX");
            Assert.AreEqual("LTR-27", C("NXD-100").BaseKey, "Emboar");
            Assert.IsTrue(C("NXD-82").IsPokemonEX);
            Assert.IsFalse(C("NXD-82").UnlimitedOnly, "Expanded");
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Flame_Burst_Hits_The_Defending_Pokemon_And_2_Benched()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flame Burst"), new ScriptedAgent());
            var chandelure = e.ScenarioActive(0, C(Chandelure));
            Attach(e, chandelure, Fire, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            var a = e.ScenarioBench(1, C(Eevee));
            var b = e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, chansey.DamageCounters, "the printed 30 (your ruling)");
            Assert.AreEqual(3, a.DamageCounters);
            Assert.AreEqual(3, b.DamageCounters);
        }

        [Test]
        public void Vengeful_Wish_Returns_The_Damage_Taken_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Vengeful Wish"), new ScriptedAgent());
            var jellicent = e.ScenarioActive(0, C(Jellicent));
            Attach(e, jellicent, Water, 1);
            jellicent.DamageCounters = 5;
            jellicent.AttackDamageTurn = 2;
            jellicent.AttackDamageThatTurn = 50;
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, chansey.DamageCounters);
        }

        [Test]
        public void Dark_Penalty_Needs_A_Tool_On_The_Defending_Pokemon()
        {
            foreach (bool tool in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Dark Penalty"), new ScriptedAgent());
                var weavile = e.ScenarioActive(0, C(Weavile));
                Attach(e, weavile, Darkness, 1);
                var chansey = e.ScenarioActive(1, C(Chansey));
                if (tool) e.ScenarioAttachTool(chansey, C(DarkClaw));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(tool ? 9 : 0, chansey.DamageCounters);
            }
        }

        [Test]
        public void Brain_Control_Puts_A_Card_From_The_Hand_On_The_Bottom_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Brain Control", "Hooligans Jim & Cas"), new ScriptedAgent());
            var beheeyem = e.ScenarioActive(0, C(Beheeyem));
            Attach(e, beheeyem, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Hooligans));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(1, opp.Hand.Count);
            Assert.AreEqual(Hooligans, opp.Deck.Last().Def.Key, "on the bottom");
        }

        [Test]
        public void Stadium_Wave_Does_30_More_And_Sleep_With_A_Stadium_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stadium Wave"), new ScriptedAgent());
            var simipour = e.ScenarioActive(0, C(Simipour));
            Attach(e, simipour, Water, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(TwistMountain));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, chansey.DamageCounters);
            Assert.IsTrue(chansey.Has(SpecialCondition.Asleep));
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Heal_Block_Stops_All_Healing()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.DamageCounters = 5;
            var theirs = e.ScenarioActive(1, C(Chansey));
            theirs.DamageCounters = 5;
            var ctx = new EffectContext(e, 0, null, null);
            ctx.Heal(chansey, 30);
            Assert.AreEqual(2, chansey.DamageCounters, "no Bronzong yet");
            e.ScenarioBench(1, C(Bronzong));
            ctx.Heal(chansey, 20);
            ctx.Heal(theirs, 20);
            Assert.AreEqual(2, chansey.DamageCounters, "yours");
            Assert.AreEqual(5, theirs.DamageCounters, "and the opponent's");
        }

        [Test]
        public void Dark_Aura_Makes_Every_Energy_Darkness()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hydreigon = e.ScenarioActive(0, C(Hydreigon));
            Attach(e, hydreigon, Psychic, 4);
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(4, e.EnergyCountOfType(hydreigon, EnergyType.Darkness));
            Assert.AreEqual(0, e.EnergyCountOfType(hydreigon, EnergyType.Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Berserker Blade")), "DDDD paid with Psychic Energy");
        }

        [Test]
        public void Blazing_Mane_Burns_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Scratch"), new ScriptedAgent());
            var pawniard = e.ScenarioActive(0, C(Pawniard));
            Attach(e, pawniard, Metal, 2);
            e.ScenarioActive(1, C(Arcanine));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(pawniard.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Giant_Fan_Shuffles_An_Opponents_Pokemon_Into_Their_Deck_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Shiftry", "Yes", "Eevee"), new ScriptedAgent());
            e.ScenarioActive(0, C(Nuzleaf));
            e.ScenarioAddToHand(0, C(Shiftry));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(0, e.State.Players[1].Bench.Count);
            Assert.IsTrue(e.State.Players[1].Deck.Any(c => c.Def.Key == Eevee));
        }

        [Test]
        public void Sporprise_Confuses_And_Poisons_The_Opponents_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Amoonguss", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Foongus));
            e.ScenarioAddToHand(0, C(Amoonguss));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(chansey.Has(SpecialCondition.Confused));
            Assert.IsTrue(chansey.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Cursed_Shadow_Works_Only_From_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C(CursedChandelure));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Cursed Shadow")), "on the Bench");

            var m = Scenario(new ScriptedAgent(), new ScriptedAgent());
            m.ScenarioActive(0, C(CursedChandelure));
            m.ScenarioActive(1, C(Chansey));
            m.ScenarioSetTurn(0, 3);
            Assert.IsTrue(m.GetMainActions(0).Any(o => o.Label.Contains("Cursed Shadow")), "Active");
        }

        [Test]
        public void Slippery_Soles_Switches_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Slippery Soles", "Eevee"), new ScriptedAgent());
            e.ScenarioActive(0, C(Vanilluxe));
            e.ScenarioBench(0, C(Eevee));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Eevee", e.State.Players[0].Active.Name);
            Assert.AreEqual("Blissey", e.State.Players[1].Active.Name);
        }

        [Test]
        public void Forewarn_Takes_1_Of_The_Top_2_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Forewarn", "Dark Claw"), new ScriptedAgent());
            e.ScenarioActive(0, C(Musharna));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(DarkClaw)));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Hooligans)));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == DarkClaw));
            Assert.AreEqual(Hooligans, e.State.Players[0].Deck[0].Def.Key, "the other stays on top");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Skyarrow_Bridge_Lowers_The_Retreat_Cost_Of_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            var blissey = e.ScenarioBench(0, C(Blissey));
            var theirs = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(SkyarrowBridge));
            Assert.AreEqual(2, e.RetreatCostOf(chansey));
            Assert.AreEqual(2, e.RetreatCostOf(theirs), "both players'");
            Assert.AreEqual(3, e.RetreatCostOf(blissey), "not a Basic Pokémon");
        }

        [Test]
        public void Pokemon_Center_Heals_20_From_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Pokémon Center"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var eevee = e.ScenarioBench(0, C(Eevee));
            eevee.DamageCounters = 3;
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(PokemonCenter));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, eevee.DamageCounters);
        }
    }
}
