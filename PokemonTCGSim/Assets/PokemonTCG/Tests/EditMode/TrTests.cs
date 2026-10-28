using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v121: Team Rocket (TR 1-83): Goop Gas Attack, Stare, Hay Fever, Frenzy (your ruling), Mirror Shell (your ruling), Sinkhole,
    /// Sticky Goo, Summon Minions, Challenge!, Digger, Imposter Oak's Revenge, Full Heal Energy and Potion Energy.
    /// </summary>
    public class TrTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Fighting = "MEE-14";

        const string DarkArbok = "TR-2";
        const string DarkDragonite = "TR-5";
        const string DarkDugtrio = "TR-6";
        const string DarkVileplume = "TR-13";
        const string DarkDragonair = "TR-33";
        const string DarkGloom = "TR-36";
        const string DarkKadabra = "TR-39";
        const string DarkMuk = "TR-41";
        const string DarkPrimeape = "TR-43";
        const string DarkRapidash = "TR-44";
        const string DarkWartortle = "TR-46";
        const string Abra = "TR-49";
        const string Diglett = "TR-52";
        const string Dratini = "TR-53";
        const string Ekans = "TR-56";
        const string Grimer = "TR-57";
        const string Mankey = "TR-61";
        const string Oddish = "TR-63";
        const string Ponyta = "TR-64";
        const string Rattata = "TR-66";
        const string Squirtle = "TR-68";
        const string Challenge = "TR-74";
        const string Digger = "TR-75";
        const string ImposterOaksRevenge = "TR-76";
        const string GoopGasAttack = "TR-78";
        const string Sleep = "TR-79";
        const string FullHealEnergy = "TR-81";
        const string PotionEnergy = "TR-82";
        const string DragonairBS = "BS-18";
        const string DratiniBS = "BS-26";

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

        private PokemonInPlay Evolved(GameEngine e, int player, bool active, params string[] line)
        {
            var mon = active ? e.ScenarioActive(player, C(line[0])) : e.ScenarioBench(player, C(line[0]));
            for (int i = 1; i < line.Length; i++) e.ScenarioEvolve(mon, C(line[i]));
            return mon;
        }

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(64, db.Cards.Count(d => d.SetCode == "TR"));
            Assert.AreEqual("TR-1", C("TR-18").BaseKey, "the non-holo Dark Alakazam");
            Assert.AreEqual("TR-16", C("TR-72").BaseKey, "Rocket's Sneak Attack");
            Assert.AreEqual("CEL-CC5", C("TR-8").BaseKey, "Dark Gyarados");
            Assert.AreEqual("CEL-CC5", C("TR-25").BaseKey, "the non-holo Dark Gyarados");
            Assert.AreEqual("CEL-CC6", C("TR-15").BaseKey, "Here Comes Team Rocket!");
            Assert.AreEqual("TR-17", C("TR-80").BaseKey, "Rainbow Energy is its own card (never errata'd)");
            Assert.IsTrue(C("TR-17").UnlimitedOnly);
            Assert.IsTrue(C(FullHealEnergy).UnlimitedOnly);
            Assert.AreEqual("Dark Kadabra", C("TR-1").EvolvesFrom);
            Assert.AreEqual("Abra", C(DarkKadabra).EvolvesFrom);
            Assert.AreEqual(AbilityDef.PokemonPower, C(DarkVileplume).Abilities[0].Kind);
            Assert.IsFalse(C(FullHealEnergy).IsBasicEnergy);
        }

        [Test]
        public void Goop_Gas_Attack_Stops_Pokemon_Powers_Until_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Goop Gas Attack"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(GoopGasAttack));
            var kadabra = Evolved(e, 1, true, Abra, DarkKadabra);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.AbilityBlocked(kadabra, "Matter Exchange"));
            Step(e);
            Assert.IsTrue(e.AbilityBlocked(kadabra, "Matter Exchange"));
            Assert.IsTrue(e.State.Effects.Any(x => x.ExpiresAfterTurn == 4), "until the end of the opponent's next turn");
        }

        [Test]
        public void Stare_Damages_Any_Pokemon_And_Stops_Its_Pokemon_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stare", "Dark Kadabra"), new ScriptedAgent());
            var arbok = Evolved(e, 0, true, Ekans, DarkArbok);
            Attach(e, arbok, Grass, 2);
            e.ScenarioActive(1, C(Rattata));
            var kadabra = Evolved(e, 1, false, Abra, DarkKadabra);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, kadabra.DamageCounters, "10 to a Benched Pokémon");
            Assert.IsTrue(e.AbilityBlocked(kadabra, "Matter Exchange"));
        }

        [Test]
        public void Hay_Fever_Stops_Trainers_Unless_Dark_Vileplume_Is_Asleep()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Sleep));
            var vileplume = Evolved(e, 1, true, Oddish, DarkGloom, DarkVileplume);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Sleep!"));
            vileplume.Conditions = SpecialCondition.Asleep;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play Sleep!"), "the power stops while Dark Vileplume is Asleep");
        }

        [Test]
        public void Frenzy_Adds_30_While_Confused()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Frenzied Attack"), new ScriptedAgent());
            var primeape = Evolved(e, 0, true, Mankey, DarkPrimeape);
            Attach(e, primeape, Fighting, 2);
            e.ScenarioBench(0, C(Rattata));
            var dragonair = Evolved(e, 1, true, DratiniBS, DragonairBS);
            primeape.Conditions = SpecialCondition.Confused;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(7, dragonair.DamageCounters, "40 + 30 (Confused)");
        }

        [Test]
        public void Frenzy_Puts_3_More_Counters_On_A_Failed_Confusion_Flip()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Frenzied Attack"), new ScriptedAgent());
            var primeape = Evolved(e, 0, true, Mankey, DarkPrimeape);
            Attach(e, primeape, Fighting, 2);
            e.ScenarioBench(0, C(Rattata));
            var dragonair = Evolved(e, 1, true, DratiniBS, DragonairBS);
            primeape.Conditions = SpecialCondition.Confused;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(0, dragonair.DamageCounters);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Key == DarkPrimeape), "6 counters Knock Out the 60-HP Dark Primeape (your ruling)");
        }

        [Test]
        public void Mirror_Shell_Sends_The_Damage_Back_With_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mirror Shell"), new ScriptedAgent().Then("Attack: Rear Kick"));
            var wartortle = Evolved(e, 0, true, Squirtle, DarkWartortle);
            Attach(e, wartortle, Water, 2);
            var rapidash = Evolved(e, 1, true, Ponyta, DarkRapidash);
            Attach(e, rapidash, Fire, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(2, wartortle.DamageCounters);
            Assert.AreEqual(4, rapidash.DamageCounters, "20 back, doubled by Dark Rapidash's Water Weakness (your ruling)");
        }

        [Test]
        public void Sinkhole_Hurts_A_Retreating_Pokemon_On_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            e.ScenarioBench(0, C(Abra));
            e.ScenarioActive(1, C(Mankey));
            Evolved(e, 1, false, Diglett, DarkDugtrio);
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(rattata));
            Assert.AreEqual(2, rattata.DamageCounters);
        }

        [Test]
        public void Sticky_Goo_Raises_The_Retreat_Cost_While_Dark_Muk_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            var muk = Evolved(e, 1, true, Grimer, DarkMuk);
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(rattata));
            muk.Conditions = SpecialCondition.Paralyzed;
            Assert.AreEqual(0, e.RetreatCostOf(rattata));
        }

        [Test]
        public void Summon_Minions_Benches_2_Basic_Pokemon_When_Played_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes", "Dratini|Dratini"), new ScriptedAgent());
            Evolved(e, 0, true, Dratini, DarkDragonair);
            e.ScenarioAddToHand(0, C(DarkDragonite));
            e.ScenarioAddToDeck(0, C(Dratini), 3);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Dark Dragonite", e.State.Players[0].Active.Name);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Challenge_Declined_Draws_2_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Challenge!"), new ScriptedAgent().Then("No"));
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Challenge));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Challenge_Accepted_Benches_Basic_Pokemon_For_Both_Players()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Challenge!", "Dratini"), new ScriptedAgent().Then("Yes", "Abra"));
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Challenge));
            e.ScenarioAddToDeck(0, C(Dratini), 2);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioAddToDeck(1, C(Abra), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
            Assert.AreEqual(1, e.State.Players[1].Bench.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "no cards drawn");
        }

        [Test]
        public void Digger_Tails_Hurts_Your_Own_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Digger"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Rattata));
            var theirs = e.ScenarioActive(1, C(Rattata));
            e.ScenarioAddToHand(0, C(Digger));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, false));
            Step(e);
            Assert.AreEqual(0, mine.DamageCounters);
            Assert.AreEqual(1, theirs.DamageCounters, "heads, then the opponent's tails");
        }

        [Test]
        public void Imposter_Oaks_Revenge_Makes_The_Opponent_Draw_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Imposter Oak's Revenge"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(ImposterOaksRevenge));
            e.ScenarioAddToHand(0, C(Abra));
            e.ScenarioActive(1, C(Rattata));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Abra));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Key == Abra), "the discarded card");
        }

        [Test]
        public void Fix_Destiny_Bond_Knocks_Out_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Destiny Bond"), new ScriptedAgent().Then("Attack: Ember"));
            var gastly = e.ScenarioActive(0, C("BS-50"));
            e.ScenarioAttach(gastly, C("MEE-13"));
            e.ScenarioAttach(gastly, C("MEE-13"));
            e.ScenarioBench(0, C(Rattata));
            var ponyta = e.ScenarioActive(1, C(Ponyta));
            Attach(e, ponyta, Fire, 2);
            e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Key == "BS-50"), "Gastly was Knocked Out by Ember");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Key == Ponyta), "and Destiny Bond Knocked Out Ponyta");
        }

        [Test]
        public void Team_Rocket_Rainbow_Energy_Does_10_Damage_Not_A_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Rainbow Energy"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C("TR-17"));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            var log = new DamageLog();
            e.AddObserver(log);
            Step(e);
            Assert.AreEqual(1, rattata.DamageCounters);
            Assert.IsTrue(log.Damage.Contains(10), "dealt as 10 damage");
            Assert.IsTrue(e.IsType(rattata, EnergyType.Colorless));
            Assert.AreEqual(1, e.EnergyCountOfType(rattata, EnergyType.Water), "counts as every type");

            var e2 = Scenario(new ScriptedAgent().Then("Attach Rainbow Energy"), new ScriptedAgent());
            var rattata2 = e2.ScenarioActive(0, C(Rattata));
            e2.ScenarioAddToHand(0, C("CES-151"));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            var log2 = new DamageLog();
            e2.AddObserver(log2);
            Step(e2);
            Assert.AreEqual(1, rattata2.DamageCounters);
            Assert.IsFalse(log2.Damage.Contains(10), "the later Rainbow Energy places a damage counter instead");
        }

        private sealed class DamageLog : IGameObserver
        {
            public readonly System.Collections.Generic.List<int> Damage = new System.Collections.Generic.List<int>();
            public void OnEvent(GameEngine engine, GameEvent ev) { if (ev.Type == GameEventType.Damage) Damage.Add(ev.Amount); }
        }

        [Test]
        public void Full_Heal_And_Potion_Energy_Work_When_Attached_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Full Heal Energy"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            rattata.Conditions = SpecialCondition.Asleep | SpecialCondition.Poisoned;
            e.ScenarioAddToHand(0, C(FullHealEnergy));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(rattata.Has(SpecialCondition.Asleep));
            Assert.IsFalse(rattata.Has(SpecialCondition.Poisoned));

            var e2 = Scenario(new ScriptedAgent().Then("Attach Potion Energy"), new ScriptedAgent());
            var rattata2 = e2.ScenarioActive(0, C(Rattata));
            rattata2.DamageCounters = 2;
            e2.ScenarioAddToHand(0, C(PotionEnergy));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, rattata2.DamageCounters);
        }
    }
}
