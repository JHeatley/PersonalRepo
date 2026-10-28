using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v139: Neo Discovery (N2): the card data (the non-holo prints, the Unown deck rule, Baby Pokémon), the new engine hooks (any
    /// Energy attachment, a retreat attempt, a Benched Pokémon becoming Active during the opponent's turn) and the Pokémon Powers,
    /// attacks and Trainers, with your rulings.
    /// </summary>
    public class N2Tests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        const string HoloEspeon = "N2-1";
        const string Forretress = "N2-2";
        const string Magnemite = "N2-7";
        const string Politoed = "N2-8";
        const string Scizor = "N2-10";
        const string Smeargle = "N2-11";
        const string Tyranitar = "N2-12";
        const string FeintUmbreon = "N2-13";
        const string UnownA = "N2-14";
        const string Wobbuffet = "N2-16";
        const string Yanma = "N2-17";
        const string Butterfree = "N2-19";
        const string PursuitUmbreon = "N2-32";
        const string Eevee = "N2-38";
        const string Igglybuff = "N2-40";
        const string Kakuna = "N2-41";
        const string Metapod = "N2-42";
        const string Poliwhirl = "N2-44";
        const string Pupitar = "N2-45";
        const string UnownD = "N2-47";
        const string UnownF = "N2-48";
        const string UnownN = "N2-50";
        const string UnownU = "N2-51";
        const string Kabuto = "N2-56";
        const string Omanyte = "N2-60";
        const string Pineco = "N2-61";
        const string Poliwag = "N2-62";
        const string Tyrogue = "N2-66";
        const string UnownE = "N2-67";
        const string UnownI = "N2-68";
        const string UnownO = "N2-69";
        const string Wooper = "N2-71";
        const string FossilEgg = "N2-72";
        const string HyperDevolutionSpray = "N2-73";
        const string RuinWall = "N2-74";
        const string EnergyArk = "N2-75";
        const string UnownJ = "WP-38";
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

        private List<CardDef> Deck(params string[] keys)
        {
            var deck = keys.Select(C).ToList();
            while (deck.Count < 60) deck.Add(C(Psychic));
            return deck;
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void Neo_Discovery_Is_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "N2").ToList();
            Assert.AreEqual(60, set.Count, "75 cards less the non-holo prints 21-31 and 33-36");
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.IsFalse(set.Any(d => { int n = int.Parse(d.Number); return n >= 21 && n <= 36 && n != 32; }));
            Assert.AreEqual(70, C(PursuitUmbreon).Hp, "Umbreon 32 is its own card");
            Assert.IsTrue(C(Igglybuff).IsBaby);
            Assert.IsTrue(C(Tyrogue).IsBaby);
            Assert.IsTrue(C(Tyrogue).BabyEvolvesInto(C(Hitmonlee)));
            Assert.IsTrue(C(UnownA).UnownRule && C(UnownA).IsUnownBasic);
            Assert.IsFalse(C(UnownJ).UnownRule, "Wizards promo Unown J doesn't have the rule");
            Assert.IsTrue(C(UnownJ).IsUnownBasic, "but it's a Basic Pokémon with Unown in its name");
            Assert.AreEqual(AbilityDef.PokemonPower, C(Forretress).Abilities[0].Kind);
            Assert.AreEqual(20, C(Scizor).Attacks[1].Damage);
            Assert.AreEqual("x", C(Scizor).Attacks[1].DamageSuffix, "Double Claw is 20x");
            Assert.AreEqual(30, C(FeintUmbreon).Attacks[1].Damage, "Feint Attack does 30");
            Assert.IsTrue(C(FossilEgg).IsTrainer && C(EnergyArk).IsTrainer);
        }

        [Test]
        public void A_Deck_With_A_Neo_Discovery_Unown_Has_4_Unown_In_All()
        {
            var four = Deck(UnownA, UnownA, UnownD, UnownJ);
            Assert.IsFalse(Decklist.Validate(four, null).Any(p => p.Contains("Unown")), "4 Unown of any letters");
            var five = Deck(UnownA, UnownA, UnownD, UnownJ, UnownF);
            Assert.IsTrue(Decklist.Validate(five, null).Any(p => p.Contains("Unown")), "a 5th Unown (your ruling: Unown J counts too)");
            var wizards = Deck(UnownJ, UnownJ, UnownJ, UnownJ);
            Assert.IsFalse(Decklist.Validate(wizards, null).Any(p => p.Contains("Unown")));
            Assert.IsFalse(Decklist.UnownRuleBlocks(wizards.Take(4), C(UnownJ)), "no card with the rule: only the 4-copy rule");
            Assert.IsTrue(Decklist.UnownRuleBlocks(wizards.Take(4), C(UnownA)), "adding the rule's card");
            Assert.IsTrue(Decklist.UnownRuleBlocks(four.Take(4), C(UnownJ)));
            Assert.IsFalse(Decklist.UnownRuleBlocks(four.Take(3), C(UnownJ)));
        }

        // ------------------------------------------------------------------ Pokémon Powers

        [Test]
        public void Spikes_Hits_The_Opponents_New_Active_Pokemon_During_Their_Turn()
        {
            foreach (int turnPlayer in new[] { 1, 0 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Forretress));
                var lee = e.ScenarioActive(1, C(Hitmonlee));
                var chansey = e.ScenarioBench(1, C(Chansey));
                e.ScenarioSetTurn(turnPlayer, 4);
                e.SwitchActive(1, chansey);
                Assert.AreSame(chansey, e.State.Players[1].Active);
                Assert.AreEqual(turnPlayer == 1 ? 1 : 0, chansey.DamageCounters, "only during the opponent's turn");
                Assert.AreEqual(0, lee.DamageCounters);
            }
        }

        [Test]
        public void Spikes_Stops_While_Forretress_Is_Asleep()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var forretress = e.ScenarioActive(0, C(Forretress));
            e.ApplyCondition(forretress, SpecialCondition.Asleep);
            e.ScenarioActive(1, C(Hitmonlee));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(1, 4);
            e.SwitchActive(1, chansey);
            Assert.AreEqual(0, chansey.DamageCounters);
        }

        [Test]
        public void Gaze_Stops_A_Benched_Pokemon_Power_This_Turn()
        {
            foreach (bool gaze in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Gaze"), new ScriptedAgent());
                e.ScenarioActive(0, C(Igglybuff));
                var lee = e.ScenarioBench(0, C(Hitmonlee));
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioBench(1, C(Forretress));
                e.ScenarioSetTurn(0, 3);
                Assert.IsTrue(Offered(e, 0, "Use Gaze"));
                if (gaze) Step(e);
                e.SwitchActive(0, lee);
                Assert.AreEqual(gaze ? 0 : 1, lee.DamageCounters, gaze ? "Spikes is stopped" : "Spikes hits Hitmonlee");
            }
        }

        [Test]
        public void Frog_Song_Adds_40_After_Weakness_With_More_Than_3_Poliwag_Line_Pokemon_In_Play()
        {
            foreach (bool fourth in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Doubleslap"), new ScriptedAgent());
                var politoed = e.ScenarioActive(0, C(Politoed));
                Attach(e, politoed, Water, 4);
                e.ScenarioBench(0, C(Poliwag));
                e.ScenarioBench(0, C(Poliwhirl));
                var blissey = e.ScenarioActive(1, C(Blissey));
                if (fourth) e.ScenarioBench(1, C(Poliwag));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(true, false));
                Step(e);
                Assert.AreEqual(fourth ? 8 : 4, blissey.DamageCounters, "40 for 1 heads, +40 with the opponent's Poliwag making 4");
            }
        }

        [Test]
        public void Unown_D_Takes_30_Off_A_Darkness_Pokemons_Damage_Unless_There_Are_2()
        {
            foreach (int count in new[] { 1, 2 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Bite"), new ScriptedAgent());
                var umbreon = e.ScenarioActive(0, C(FeintUmbreon));
                Attach(e, umbreon, Psychic, 2);
                var blissey = e.ScenarioActive(1, C(Blissey));
                for (int i = 0; i < count; i++) e.ScenarioBench(1, C(UnownD));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(count == 1 ? 0 : 2, blissey.DamageCounters);
            }
        }

        [Test]
        public void Anger_Adds_10_For_Each_Counter_On_Unown_A_To_Any_Unowns_Hidden_Power()
        {
            foreach (var attacker in new[] { UnownN, UnownJ })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Hidden Power"), new ScriptedAgent());
                var unown = e.ScenarioActive(0, C(attacker));
                Attach(e, unown, Psychic, 1);
                var angry = e.ScenarioBench(0, C(UnownA));
                angry.DamageCounters = 3;
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(4, blissey.DamageCounters, attacker + ": 10 + 30");
            }
        }

        [Test]
        public void Find_Needs_Unown_F_I_N_And_D_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use [Find]", "Energy Ark"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioBench(0, C(UnownF));
            e.ScenarioBench(0, C(UnownI));
            e.ScenarioBench(0, C(UnownN));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToDeck(0, C(EnergyArk), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "[Find]"), "no Unown D");
            e.ScenarioBench(0, C(UnownD));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Energy Ark"));
        }

        [Test]
        public void Undo_Returns_The_Active_Pokemon_And_Its_Cards_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use [Undo]"), new ScriptedAgent());
            var lee = e.ScenarioActive(0, C(Hitmonlee));
            Attach(e, lee, Fighting, 2);
            e.ScenarioBench(0, C(UnownU));
            e.ScenarioBench(0, C(UnownN));
            e.ScenarioBench(0, C(UnownD));
            e.ScenarioBench(0, C(UnownO));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "Hitmonlee and 2 Energy");
            Assert.AreNotSame(lee, e.State.Players[0].Active);
            Assert.IsNotNull(e.State.Players[0].Active, "a Benched Pokémon takes its place");
        }

        [Test]
        public void Join_Counts_Neo_Discovery_Unown_Letters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioBench(0, C(UnownJ));
            e.ScenarioBench(0, C(UnownO));
            e.ScenarioBench(0, C(UnownI));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Join"));
            e.ScenarioBench(0, C(UnownN));
            Assert.IsTrue(Offered(e, 0, "Use Join"), "Unown [O], [I] and [N] are Unown O, I and N");
        }

        [Test]
        public void Engage_Lets_Each_Player_Shuffle_Their_Hand_And_Draw_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Unown [E]", "No"), new ScriptedAgent().Then("Yes"));
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(UnownE));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count, "the opponent took it");
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "you didn't");
        }

        [Test]
        public void Increase_Benches_An_Unown_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Unown [I]", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(UnownI));
            e.ScenarioAddToDeck(0, C(UnownO), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEquivalent(new[] { "Unown [I]", "Unown [O]" }, e.State.Players[0].Bench.Select(b => b.Name).ToArray());
        }

        [Test]
        public void Energy_Evolution_Evolves_Eevee_Into_The_Energys_Type_On_Heads_Even_On_The_First_Turn()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy", "Espeon"), new ScriptedAgent());
                var eevee = e.ScenarioActive(0, C(Eevee));
                e.ScenarioAddToHand(0, C(Psychic));
                e.ScenarioAddToDeck(0, C(FeintUmbreon), 1);
                e.ScenarioAddToDeck(0, C(HoloEspeon), 1);
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 1);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? "Espeon" : "Eevee", eevee.Name, "Psychic Energy: the Psychic Espeon, not Umbreon");
                Assert.AreEqual(1, eevee.Energy.Count);
            }
        }

        [Test]
        public void Energy_Evolution_Also_Reacts_To_An_Energy_Attached_By_An_Effect()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C(Eevee));
            e.ScenarioAddToDeck(0, C(HoloEspeon), 1);
            e.ScenarioAddToDeck(0, C(FeintUmbreon), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            var dark = e.ScenarioAddToHand(0, C(Darkness));
            e.State.Players[0].Hand.Remove(dark);
            e.State.Rng = new Rng(SeedFor(true));
            e.AttachEnergyCard(dark, eevee);
            Assert.AreEqual("Umbreon", eevee.Name);
        }

        [Test]
        public void Revive_Fossil_Benches_A_Fossil_Pokemon_From_The_Deck_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Revive Fossil"), new ScriptedAgent());
            e.ScenarioActive(0, C(Omanyte));
            e.ScenarioAddToDeck(0, C(Kabuto), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Kabuto", e.State.Players[0].Bench.Single().Name);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Pursuit_Damages_The_Defending_Pokemon_When_It_Tries_To_Retreat()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pursuit"), new ScriptedAgent().Then("Retreat"));
            var umbreon = e.ScenarioActive(0, C(PursuitUmbreon));
            Attach(e, umbreon, Darkness, 1);
            Attach(e, umbreon, Psychic, 2);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 2);
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, lee.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(4, lee.DamageCounters, "10 more, no Weakness or Resistance");
            Assert.AreSame(chansey, e.State.Players[1].Active, "and it still retreats");
        }

        [Test]
        public void Counter_Sends_The_Damage_Back_With_Weakness_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Counter"), new ScriptedAgent().Then("Attack: Kick"));
            var wobbuffet = e.ScenarioActive(0, C(Wobbuffet));
            Attach(e, wobbuffet, Psychic, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, wobbuffet.DamageCounters);
            Assert.AreEqual(4, lee.DamageCounters, "20 back, doubled by Hitmonlee's Psychic Weakness (your ruling)");
        }

        [Test]
        public void Lock_On_Makes_Electric_Bolts_Tails_Count_As_Heads()
        {
            foreach (bool lockOn in new[] { true, false })
            {
                var e = Scenario(lockOn ? new ScriptedAgent().Then("Attack: Lock-on", "Attack: Electric Bolt") : new ScriptedAgent().Then("Attack: Electric Bolt"), new ScriptedAgent());
                var magnemite = e.ScenarioActive(0, C(Magnemite));
                Attach(e, magnemite, Lightning, 1);
                Attach(e, magnemite, Psychic, 1);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                if (lockOn) Step(e);
                NextTurn(e);
                NextTurn(e);
                e.State.Rng = new Rng(SeedFor(false, false));
                Step(e);
                Assert.AreEqual(lockOn ? 5 : 0, blissey.DamageCounters);
                Assert.AreEqual(lockOn, blissey.Has(SpecialCondition.Paralyzed));
            }
        }

        [Test]
        public void Sketch_Copies_The_Defending_Pokemons_Last_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sketch"), new ScriptedAgent().Then("Attack: Kick"));
            var smeargle = e.ScenarioActive(0, C(Smeargle));
            Attach(e, smeargle, Psychic, 2);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(1, 2);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(2, lee.DamageCounters, "Kick's 20 (Smeargle is Colorless: no Weakness)");
        }

        [Test]
        public void Trample_Flips_For_Every_Benched_Pokemon_Yours_Too()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trample"), new ScriptedAgent());
            var tyranitar = e.ScenarioActive(0, C(Tyranitar));
            Attach(e, tyranitar, Darkness, 4);
            var mine = e.ScenarioBench(0, C(Hitmonlee));
            var blissey = e.ScenarioActive(1, C(Blissey));
            var theirs = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, false));
            Step(e);
            Assert.AreEqual(5, blissey.DamageCounters);
            Assert.AreEqual(3, mine.DamageCounters);
            Assert.AreEqual(0, theirs.DamageCounters);
        }

        [Test]
        public void False_Swipe_Does_Half_The_Remaining_HP_Rounded_Down()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: False Swipe"), new ScriptedAgent());
            var scizor = e.ScenarioActive(0, C(Scizor));
            Attach(e, scizor, Metal, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            chansey.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, chansey.DamageCounters, "70 HP left: 30 more");
        }

        [Test]
        public void Shockwave_Does_Nothing_On_Tails_And_Hits_Everything_Then_Switches_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Shockwave"), new ScriptedAgent());
                var yanma = e.ScenarioActive(0, C(Yanma));
                Attach(e, yanma, Grass, 1);
                var blissey = e.ScenarioActive(1, C(Blissey));
                var chansey = e.ScenarioBench(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 1 : 0, blissey.DamageCounters);
                Assert.AreEqual(heads ? 1 : 0, chansey.DamageCounters);
                Assert.AreSame(heads ? chansey : blissey, e.State.Players[1].Active, "the switch only on heads (your ruling)");
            }
        }

        [Test]
        public void Secrete_Poison_Poisons_The_Attacker_And_Hits_Their_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Secrete Poison"), new ScriptedAgent().Then("Attack: Kick"));
            var kakuna = e.ScenarioActive(0, C(Kakuna));
            Attach(e, kakuna, Grass, 2);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(2, kakuna.DamageCounters);
            Assert.IsTrue(lee.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(1, chansey.DamageCounters);
        }

        [Test]
        public void Slime_Stops_The_Damage_When_The_Attacker_Flips_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Slime"), new ScriptedAgent().Then("Attack: Kick"));
            var wooper = e.ScenarioActive(0, C(Wooper));
            Attach(e, wooper, Water, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            Attach(e, lee, Fighting, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(0, wooper.DamageCounters);
        }

        [Test]
        public void Hatch_Heals_And_Evolves_Metapod_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hatch"), new ScriptedAgent());
            var metapod = e.ScenarioActive(0, C(Metapod));
            metapod.DamageCounters = 4;
            Attach(e, metapod, Grass, 2);
            e.ScenarioAddToDeck(0, C(Butterfree), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Butterfree", metapod.Name);
            Assert.AreEqual(0, metapod.DamageCounters);
        }

        [Test]
        public void Burst_Does_Nothing_When_The_Second_Coin_Is_Tails()
        {
            foreach (bool second in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Burst"), new ScriptedAgent());
                var pineco = e.ScenarioActive(0, C(Pineco));
                Attach(e, pineco, Grass, 1);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(false, second));
                Step(e);
                Assert.AreEqual(second ? 4 : 0, blissey.DamageCounters);
                Assert.AreEqual(0, pineco.DamageCounters, "first coin tails: no damage to itself");
            }
        }

        [Test]
        public void Dust_Devil_Hits_Every_Pokemon_That_Isnt_Fighting()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dust Devil"), new ScriptedAgent());
            var pupitar = e.ScenarioActive(0, C(Pupitar));
            Attach(e, pupitar, Fighting, 2);
            var mine = e.ScenarioBench(0, C(Chansey));
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters, "no Weakness");
            Assert.AreEqual(1, mine.DamageCounters);
            Assert.AreEqual(0, lee.DamageCounters);
            Assert.AreEqual(0, pupitar.DamageCounters);
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Fossil_Egg_Benches_A_Fossil_Pokemon_From_The_Hand_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Fossil Egg", "Your hand"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(FossilEgg));
            e.ScenarioAddToHand(0, C(Kabuto));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Kabuto", e.State.Players[0].Bench.Single().Name);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Hyper_Devolution_Spray_Takes_The_Top_Stage_And_Stops_Evolving_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hyper Devolution Spray"), new ScriptedAgent());
            var poliwag = e.ScenarioActive(0, C(Poliwag));
            e.ScenarioEvolve(poliwag, C(Poliwhirl));
            e.ScenarioAddToHand(0, C(HyperDevolutionSpray));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Poliwag", poliwag.Name);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Poliwhirl"));
            Assert.IsFalse(Offered(e, 0, "Evolve"), "it can't evolve the turn it's devolved");
        }

        [Test]
        public void Ruin_Wall_And_Energy_Ark_Search_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ruin Wall", "Unown", "Play Energy Ark", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioAddToHand(0, C(RuinWall));
            e.ScenarioAddToHand(0, C(EnergyArk));
            e.ScenarioAddToDeck(0, C(UnownA), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Unown [A]", e.State.Players[0].Bench.Single().Name);
            e.State.Rng = new Rng(SeedFor(true, true));
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.IsBasicEnergy));
        }
    }
}
