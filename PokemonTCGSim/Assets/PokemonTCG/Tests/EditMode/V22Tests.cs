using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v22-v23: attacks from the Bench (Alakazam ex's Dimensional Hand), "the Attacking Pokémon" being the Pokémon that
    /// actually attacked (your Fainting Spell ruling), Quaking Fist, and King's Order / Grimsley's Move taking Evolution Pokémon.
    /// </summary>
    public class V22Tests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Psychic = "MEE-13";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string N = "30C-CC18";
        const string Castle = "JTG-152";    // N's Castle (Stadium)
        const string Alakazam = "SVP-50";   // Alakazam ex: Mind Jack (CC), Dimensional Hand (PP, 120, usable from the Bench)
        const string GengarEx = "30C-90";   // Gengar ex: Fainting Spell
        const string Wailord = "PBL-16";    // Wailord ex, 380 HP
        const string BenchAttack = "Attack: Dimensional Hand (Alakazam ex on the Bench)";

        private PokemonInPlay BenchedAlakazam(GameEngine e)
        {
            var alakazam = e.ScenarioBench(0, C(Alakazam));
            e.ScenarioAttach(alakazam, C(Psychic));
            e.ScenarioAttach(alakazam, C(Psychic));
            return alakazam;
        }

        [Test]
        public void Dimensional_Hand_Can_Be_Used_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then(BenchAttack), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Dreepy));
            var alakazam = BenchedAlakazam(e);
            var wailord = e.ScenarioActive(1, C(Wailord));
            e.ScenarioSetTurn(0, 3);

            var options = e.GetMainActions(0).Where(o => o.Action == ActionType.Attack && o.PokemonUid == alakazam.Uid).ToList();
            Assert.AreEqual(1, options.Count, "only the attack that says it can be used from the Bench");
            Assert.AreEqual("Dimensional Hand", options[0].Card.Attacks[options[0].Index].Name);

            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters, "120 damage to the opponent's Active Pokémon");
            Assert.IsTrue(e.State.Players[0].Bench.Contains(alakazam), "it attacks from the Bench and stays there");
            Assert.AreSame(active, e.State.Players[0].Active);
        }

        [Test]
        public void Fainting_Spell_Knocks_Out_The_Pokemon_That_Attacked_Even_On_The_Bench()
        {
            int heads = 0, tails = 0;
            for (int seed = 1; seed <= 40 && (heads == 0 || tails == 0); seed++)
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent().Then(BenchAttack), new ScriptedAgent());
                e.BeginScenario(seed);
                e.ScenarioAddPrizes(0, C(Fire), 6);
                e.ScenarioAddPrizes(1, C(Fire), 6);
                e.ScenarioAddToDeck(0, C(Psychic), 10);
                e.ScenarioAddToDeck(1, C(Psychic), 10);
                var active = e.ScenarioActive(0, C(Dreepy));
                var alakazam = BenchedAlakazam(e);
                var gengar = e.ScenarioActive(1, C(GengarEx));
                gengar.DamageCounters = 17; // 280 HP: Dimensional Hand's 120 Knocks it Out
                e.ScenarioBench(1, C(Dreepy));
                e.ScenarioSetTurn(0, 3);
                Step(e);

                Assert.IsNull(e.State.FindPokemon(gengar.Uid), "Gengar ex was Knocked Out by the attack");
                Assert.AreSame(active, e.State.Players[0].Active, "the Active Pokémon didn't attack, so Fainting Spell never hits it");
                if (e.State.FindPokemon(alakazam.Uid) == null) heads++;
                else tails++;
            }
            Assert.Greater(heads, 0, "heads: the Attacking Pokémon is Knocked Out even though it's on the Bench (your ruling)");
            Assert.Greater(tails, 0, "tails: nothing happens");
        }

        [Test]
        public void Dimensional_Hand_Works_While_The_Active_Pokemon_Is_Paralyzed()
        {
            var e = Scenario(new ScriptedAgent().Then(BenchAttack), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Dreepy));
            BenchedAlakazam(e);
            var wailord = e.ScenarioActive(1, C(Wailord));
            e.ScenarioSetTurn(0, 3);
            active.Conditions = SpecialCondition.Paralyzed;
            Step(e);
            Assert.AreEqual(12, wailord.DamageCounters, "Special Conditions only affect the Active Pokémon (your ruling)");
        }

        [Test]
        public void A_Supporter_Discarded_By_Quaking_Fist_Does_Not_Use_Up_Your_Supporter()
        {
            int tails = 0;
            for (int seed = 1; seed <= 40 && tails == 0; seed++)
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent().Then("Play N"), new ScriptedAgent());
                e.BeginScenario(seed);
                e.ScenarioAddPrizes(0, C(Fire), 6);
                e.ScenarioAddPrizes(1, C(Fire), 6);
                e.ScenarioAddToDeck(0, C(Psychic), 10);
                e.ScenarioAddToDeck(1, C(Psychic), 10);
                e.ScenarioActive(0, C(Dreepy));
                e.ScenarioActive(1, C(Dreepy));
                e.ScenarioSetTurn(0, 3);
                e.ScenarioAddToHand(0, C(N));
                e.ScenarioAddToHand(0, C(N));
                e.State.Effects.Add(new TrainerFlipEffect("Quaking Fist", 1, 0, e.State.TurnNumber));
                Step(e);
                if (e.State.Players[0].Hand.Count != 1) continue; // heads: N was played (hand shuffled away, 6 drawn)

                tails++;
                Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "N"), "tails: that N was discarded instead");
                Assert.IsFalse(e.State.Players[0].SupporterPlayedThisTurn, "it doesn't count as your Supporter for the turn (your ruling)");
                Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play N"), "so you can still play the other N");
            }
            Assert.Greater(tails, 0, "a Quaking Fist coin came up tails at least once");
        }

        [Test]
        public void A_Stadium_Discarded_By_Quaking_Fist_Does_Not_Use_Up_Your_Stadium()
        {
            int tails = 0;
            for (int seed = 1; seed <= 40 && tails == 0; seed++)
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent().Then("Play N's Castle"), new ScriptedAgent());
                e.BeginScenario(seed);
                e.ScenarioAddPrizes(0, C(Fire), 6);
                e.ScenarioAddPrizes(1, C(Fire), 6);
                e.ScenarioAddToDeck(0, C(Psychic), 10);
                e.ScenarioAddToDeck(1, C(Psychic), 10);
                e.ScenarioActive(0, C(Dreepy));
                e.ScenarioActive(1, C(Dreepy));
                e.ScenarioSetTurn(0, 3);
                e.ScenarioAddToHand(0, C(Castle));
                e.ScenarioAddToHand(0, C(Castle));
                e.State.Effects.Add(new TrainerFlipEffect("Quaking Fist", 1, 0, e.State.TurnNumber));
                Step(e);
                if (e.State.Stadium != null) continue; // heads: N's Castle was played

                tails++;
                Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "N's Castle"), "tails: it was discarded instead");
                Assert.IsFalse(e.State.Players[0].StadiumPlayedThisTurn, "it doesn't count as your Stadium for the turn (your ruling)");
                Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play N's Castle"), "so you can still play the other one");
            }
            Assert.Greater(tails, 0, "a Quaking Fist coin came up tails at least once");
        }

        [Test]
        public void Kings_Order_Can_Bench_Evolution_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: King's Order"), new ScriptedAgent());
            var kingdra = e.ScenarioActive(0, C("SFA-12"));
            e.ScenarioAttach(kingdra, C(Water));
            e.ScenarioActive(1, C(Dreepy));
            e.State.Players[0].Discard.Add(e.NewCard(0, C("DRI-58"))); // Floatzel, a Stage 1 Water Pokémon
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var floatzel = e.State.Players[0].Bench.FirstOrDefault(m => m.Name == "Floatzel");
            Assert.IsNotNull(floatzel, "your ruling: Evolution Pokémon too, not just Basics");
            Assert.AreEqual(1, floatzel.Stack.Count, "put into play on its own, not evolved");
        }

        [Test]
        public void Grimsleys_Move_Can_Bench_Evolution_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Grimsley's Move"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioAddToHand(0, C("PFL-90"));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C("JTG-99"))); // Pangoro, a Stage 1 Darkness Pokémon
            Step(e);
            var pangoro = e.State.Players[0].Bench.FirstOrDefault(m => m.Name == "Pangoro");
            Assert.IsNotNull(pangoro, "your ruling: Evolution Pokémon too, not just Basics");
            Assert.AreEqual(1, pangoro.Stack.Count, "put into play on its own, not evolved");
        }
    }
}
