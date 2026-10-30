using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// What the Neural AI's network is told about the cards and the board: each attack's cost and what it does with Energy,
    /// Items and HP, the Energy each Pokémon still needs (and its evolutions), Knock Outs both ways, retreat and pivots, the
    /// Bench and Abilities that work as a Pokémon is played. And that networks from before these inputs still load and play
    /// exactly as they did.
    /// </summary>
    public class PolicyKnowledgeTests : ScenarioTestBase
    {
        private const int Fire = 1, Water = 2, Grass = 0, Colorless = 9;

        [Test]
        public void Card_Stats_Know_Each_Attacks_Cost_And_What_It_Does()
        {
            var chienPao = PolicyCards.Static(C("SVP-30"));
            int a = PolicyCards.AttackStart;
            Assert.AreEqual(0.5f, chienPao[a + Water], 1e-6, "Hail Blade costs 2 Water");
            Assert.AreEqual(60 / 300f, chienPao[a + 10], 1e-6);
            Assert.AreEqual(1, chienPao[a + 13], "60 for each Energy discarded");
            Assert.AreEqual(1, chienPao[a + 14], "discards Energy");
            Assert.AreEqual(1, chienPao[a + 20], "from any of your Pokémon");
            Assert.AreEqual(1, chienPao[a + 21], "names Water Energy");
            Assert.IsTrue(PolicyCards.UsesTeamEnergy(C("SVP-30")));

            var budew = PolicyCards.Static(C("ASC-16"));
            for (int t = 0; t < 10; t++) Assert.AreEqual(0, budew[a + t], "Itchy Pollen costs nothing");
            Assert.AreEqual(1, budew[a + 23], "Itchy Pollen locks Items");
            Assert.AreEqual(0, budew[37], "no Retreat Cost");

            var growing = PolicyCards.Static(C("POR-86"));
            int x = PolicyCards.ExtraStart;
            Assert.AreEqual(1, growing[x], "special Energy");
            Assert.AreEqual(1, growing[x + 1], "+20 HP");
            Assert.AreEqual(1, growing[x + 2], "only on one type of Pokémon");
            Assert.AreEqual(1, growing[x + 3 + Grass], "a Grass Pokémon");
            Assert.AreEqual(Grass, PolicyCards.BonusType(C("POR-86")));

            var meowth = PolicyCards.Static(C("POR-62"));
            int p = PolicyCards.PlayStart;
            Assert.AreEqual(1, meowth[p], "Last-Ditch Catch works as it's played from the hand");
            Assert.AreEqual(1, meowth[p + 1], "and finds a Supporter");
            Assert.AreEqual(2 / 3f, meowth[p + 7], 1e-6, "gives up 2 Prize cards");
            Assert.AreEqual(0.75f, meowth[a + Colorless], 1e-6, "its attack costs 3 Colorless");
        }

        [Test]
        public void Energy_That_Helps_No_Attack_Is_Wasted_But_Not_Energy_For_An_Evolution_Or_Hail_Blade()
        {
            var fire = C("MEE-10");
            Assert.AreEqual(EnergyType.Fire, fire.EnergyProvides);
            var none = new List<int>();
            var oneFire = new List<int> { EnergyMask.Of(EnergyType.Fire) };

            // Budew: a free attack and no Retreat Cost, so any basic Energy on it does nothing.
            Assert.IsTrue(PolicyPlan.Wasted(none, fire, new List<CardDef> { C("ASC-16") }, 0, true, false), "Energy on Budew");
            Assert.IsFalse(PolicyPlan.Wasted(none, fire, new List<CardDef> { C("ASC-16") }, 0, true, true), "unless the deck has Hail Blade");

            // Charmander (R: 30) on the Bench: its first Fire Energy helps, a second one doesn't...
            var charmander = C("SVP-44");
            Assert.IsFalse(PolicyPlan.Wasted(none, fire, new List<CardDef> { charmander }, 1, false, false));
            Assert.IsTrue(PolicyPlan.Wasted(oneFire, fire, new List<CardDef> { charmander }, 1, false, false), "a second Fire on a Benched Charmander");
            // ...unless it's the Active Pokémon and needs it to retreat, or Charizard ex (RR) is still to come.
            Assert.IsFalse(PolicyPlan.Wasted(new List<int>(), fire, new List<CardDef> { charmander }, 2, true, false));
            var line = PolicyPlan.Line(charmander, new[] { C("OBF-27"), C("SVP-56"), C("ASC-16") });
            CollectionAssert.AreEqual(new[] { "Charmander", "Charmeleon", "Charizard ex" }, line.Select(c => c.Name).ToArray());
            Assert.IsFalse(PolicyPlan.Wasted(oneFire, fire, line, 1, false, false), "the second Fire is for Charizard ex");
            Assert.IsFalse(PolicyPlan.Wasted(none, C("POR-86"), new List<CardDef> { C("ASC-16") }, 0, true, false),
                "special Energy isn't judged (its effect may be the point)");
        }

        [Test]
        public void The_Network_Sees_Energy_Needed_Knock_Outs_Retreat_Items_And_The_Bench()
        {
            var me = new ScriptedAgent();
            var e = Scenario(me, new ScriptedAgent());
            var charmander = e.ScenarioActive(0, C("SVP-44"));
            var budew = e.ScenarioBench(0, C("ASC-16"));
            e.ScenarioActive(1, C("ASC-16"));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioAddToHand(0, C("POR-62"));
            e.ScenarioAddToDeck(0, C("OBF-27"), 1);
            e.ScenarioAddToDeck(0, C("SVP-56"), 1);
            e.ScenarioSetTurn(0, 3);
            e.State.Players[1].ItemLockTurn = 4; // locked on their next turn (as after Itchy Pollen)
            Step(e);
            var r = me.Seen.First(d => d.Kind == DecisionKind.MainAction);

            var mem = new PolicyMemory(0) { DeckList = new List<CardDef> { C("SVP-44"), C("ASC-16"), C("MEE-10"), C("POR-62"), C("OBF-27"), C("SVP-56") } };
            var choices = PolicyEncoder.Choices(r, new List<int>());
            var x = PolicyEncoder.Encode(r, mem, new PolicyNet(), choices);

            // Slots: Charmander is your Active (slot 0), Budew your first Benched Pokémon (slot 1).
            int n = PolicyInput.SlotFeatures;
            Assert.AreEqual(0, x.SlotFeat[0 * n + 26], "Charmander can't attack yet");
            Assert.AreEqual(0.25f, x.SlotFeat[0 * n + 27], 1e-6, "it needs 1 Energy");
            Assert.AreEqual(0.25f, x.SlotFeat[0 * n + 31], 1e-6, "an evolution (Charmeleon) is still in the deck");
            Assert.AreEqual(0, x.SlotFeat[0 * n + 37], "it can't Knock Out Budew now...");
            Assert.AreEqual(1, x.SlotFeat[0 * n + 38], "...but can with one more Energy (30 damage, 30 HP)");
            Assert.AreEqual(1, x.SlotFeat[1 * n + 26], "Budew's attack is free");
            Assert.AreEqual(1, x.SlotFeat[1 * n + 41], "Budew retreats for free");

            // This turn: the opponent is Item-locked next turn, Budew is a free pivot, 4 Bench spaces, Meowth ex in hand.
            Assert.AreEqual(1, x.TurnFeats[2 + PolicyInput.SideSlots + 1], "their Items are locked next turn");
            Assert.AreEqual(1 / 3f, x.TurnFeats[2 + PolicyInput.SideSlots + 7], 1e-6, "one free-retreat pivot on your Bench");
            Assert.AreEqual(4 / 8f, x.TurnFeats[2 + PolicyInput.SideSlots + 10], 1e-6, "4 free Bench spaces");
            Assert.AreEqual(0.5f, x.TurnFeats[2 + PolicyInput.SideSlots + 11], 1e-6, "Meowth ex in hand");

            // Options: Fire on Charmander helps, makes its attack usable and Knocks Out Budew; Fire on Budew is wasted.
            int f = PolicyInput.OptionFeatures;
            int onCharmander = Option(r, choices, o => o.Action == ActionType.AttachEnergy && o.PokemonUid == charmander.Uid);
            int onBudew = Option(r, choices, o => o.Action == ActionType.AttachEnergy && o.PokemonUid == budew.Uid);
            Assert.AreEqual(1, x.OptFeat[onCharmander * f + 6]);
            Assert.AreEqual(1, x.OptFeat[onCharmander * f + 7]);
            Assert.AreEqual(0, x.OptFeat[onCharmander * f + 8]);
            Assert.AreEqual(1, x.OptFeat[onCharmander * f + 10], "Fire Energy on a Fire Pokémon");
            Assert.AreEqual(1, x.OptFeat[onCharmander * f + 11], "that attack Knocks Out their Active");
            Assert.AreEqual(1, x.OptFeat[onBudew * f + 8], "Energy on Budew is wasted");
            Assert.IsTrue(PolicyEncoder.WastedAttachment(r, mem, r.Options[choices[onBudew].Option]));
            Assert.IsFalse(PolicyEncoder.WastedAttachment(r, mem, r.Options[choices[onCharmander].Option]));

            // Benching Meowth ex: its Ability works as it's played, and it leaves 3 Bench spaces.
            int bench = Option(r, choices, o => o.Action == ActionType.BenchBasic && o.Card != null && o.Card.Name == "Meowth ex");
            Assert.AreEqual(1, x.OptFeat[bench * f + 16]);
            Assert.AreEqual(3 / 8f, x.OptFeat[bench * f + 17], 1e-6);
        }

        [Test]
        public void Protection_Abilities_Attacks_That_Ignore_Them_And_Abilities_Turned_Off()
        {
            int g = PolicyCards.GuardStart, a = PolicyCards.AttackStart;
            var crustle = PolicyCards.Static(C("DRI-12"));
            Assert.AreEqual(1, crustle[g], "Mysterious Rock Inn prevents damage");
            Assert.AreEqual(1, crustle[g + 1], "from Pokémon ex");
            Assert.AreEqual(1, crustle[a + 24], "Superb Scissors ignores effects on the defender");
            var ogerpon = PolicyCards.Static(C("TWM-112"));
            Assert.AreEqual(1, ogerpon[g + 2], "Cornerstone Stance: from Pokémon with an Ability");
            Assert.AreEqual(1, ogerpon[a + 24], "Demolish ignores effects on the defender");
            Assert.AreEqual(1, ogerpon[a + 25], "and Weakness and Resistance");
            Assert.AreEqual(1, PolicyCards.Static(C("DLR-94"))[g + 5], "Legendary Pyroduct turns Abilities off");
            Assert.AreEqual(1, PolicyCards.Static(C("TEF-161"))[g + 7], "Mist Energy stops the effects of attacks");
            Assert.IsTrue(PolicyCards.UsesTeamEnergy(C("DLR-84")), "Storm Emerald counts Energy on all your Pokémon");
            Assert.IsFalse(PolicyCards.UsesTeamEnergy(C("SVP-56")), "Infernal Reign only attaches Energy");

            // Mega Rayquaza ex can't hurt Crustle...
            var me = new ScriptedAgent();
            var e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C("DLR-84"));
            e.ScenarioActive(1, C("DRI-12"));
            var x = EncodeFirstMainAction(e, me);
            int n = PolicyInput.SlotFeatures, theirs = PolicyInput.SideSlots;
            Assert.AreEqual(1, x.SlotFeat[0 * n + 45], "Storm Emerald is stopped by Mysterious Rock Inn");
            Assert.AreEqual(1, x.SlotFeat[theirs * n + 43], "Crustle's Ability works");

            // ...until Legendary Pyroduct takes Crustle's Ability away.
            me = new ScriptedAgent();
            e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C("DLR-84"));
            e.ScenarioActive(1, C("DRI-12"));
            e.ScenarioSetStadium(0, C("DLR-94"));
            x = EncodeFirstMainAction(e, me);
            Assert.AreEqual(0, x.SlotFeat[theirs * n + 43], "no Abilities for Evolution Pokémon under Legendary Pyroduct");
            Assert.AreEqual(0, x.SlotFeat[0 * n + 45], "so Mega Rayquaza ex can damage it");

            // Cornerstone Mask Ogerpon ex and Crustle protect themselves from each other, but both attacks ignore that.
            me = new ScriptedAgent();
            e = Scenario(me, new ScriptedAgent());
            var mon = e.ScenarioActive(0, C("TWM-112"));
            e.ScenarioActive(1, C("DRI-12"));
            e.ScenarioAttach(mon, C("TEF-161"));
            x = EncodeFirstMainAction(e, me);
            Assert.AreEqual(0, x.SlotFeat[0 * n + 45], "Demolish gets through Mysterious Rock Inn");
            Assert.AreEqual(0, x.SlotFeat[theirs * n + 45], "Superb Scissors gets through Cornerstone Stance");
            Assert.AreEqual(1, x.SlotFeat[0 * n + 46], "Mist Energy stops the effects of attacks on Ogerpon");
            Assert.IsTrue(PolicyPlan.Blocks(C("POR-87"), C("TWM-112")), "Rocky Fighting Energy works on a Fighting Pokémon (Ogerpon)...");
            Assert.IsFalse(PolicyPlan.Blocks(C("POR-87"), C("DLR-84")), "...and not on a Dragon one (Mega Rayquaza ex)");
        }

        private PolicyInput EncodeFirstMainAction(GameEngine e, ScriptedAgent me)
        {
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var r = me.Seen.First(d => d.Kind == DecisionKind.MainAction);
            var mem = new PolicyMemory(0) { DeckList = new List<CardDef>() };
            return PolicyEncoder.Encode(r, mem, new PolicyNet(), PolicyEncoder.Choices(r, new List<int>()));
        }

        private static int Option(DecisionRequest r, List<PolicyEncoder.Choice> choices, System.Func<DecisionOption, bool> match)
        {
            for (int i = 0; i < choices.Count; i++)
                if (!choices[i].Done && match(r.Options[choices[i].Option])) return i;
            Assert.Fail("no such option: " + string.Join(" | ", r.Options.Select(o => o.Label)));
            return -1;
        }

        [Test]
        public void A_Network_From_Before_The_New_Inputs_Loads_And_Plays_Exactly_As_Before()
        {
            // Read the starting network without leaving it set for other tests (a brain made with it set starts from it).
            var saved = LearningBrain.PolicyStart;
            UnityCardData.LoadPolicyStart();
            var start = LearningBrain.PolicyStart;
            LearningBrain.PolicyStart = saved;
            Assert.NotNull(start, "Resources/AI/policy_start.bytes");
            var net = new PolicyNet();
            Assert.IsTrue(net.FromBytes(start), "the Python bot's network (made before the new inputs) loads");

            PolicyInput found = null;
            var agent = new PolicyAgent(net, 1) { Record = true };
            var engine = new GameEngine(StandardCards.CreateRegistry(), new Recorder(agent, xs => { if (found == null && xs.Count >= 3) found = xs; }), new HeuristicAgent(2));
            engine.Run(UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck), UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck), 7);
            Assert.NotNull(found);

            // Its weights for the new inputs are zero, so blanking them changes nothing.
            var before = net.Forward(found);
            for (int s = 0; s < PolicyInput.NumSlots; s++)
                for (int i = 26; i < PolicyInput.SlotFeatures; i++) found.SlotFeat[s * PolicyInput.SlotFeatures + i] = 0;
            for (int o = 0; o < found.Count; o++)
                for (int i = 6; i < PolicyInput.OptionFeatures; i++) found.OptFeat[o * PolicyInput.OptionFeatures + i] = 0;
            for (int i = 2 + PolicyInput.SideSlots; i < PolicyInput.TurnFeatures; i++) found.TurnFeats[i] = 0;
            var after = net.Forward(found);
            Assert.AreEqual(before.Value, after.Value, 1e-5);
            for (int i = 0; i < before.Logits.Length; i++) Assert.AreEqual(before.Logits[i], after.Logits[i], 1e-5);

            // Saved with the new sizes, it loads again.
            var copy = new PolicyNet();
            Assert.IsTrue(copy.FromBytes(net.ToBytes()));
            Assert.AreEqual(before.Value, copy.Forward(found).Value, 1e-5);
        }

        private sealed class Recorder : IPlayerAgent, IGameObserver
        {
            private readonly PolicyAgent inner;
            private readonly System.Action<PolicyInput> seen;

            public Recorder(PolicyAgent inner, System.Action<PolicyInput> seen)
            {
                this.inner = inner;
                this.seen = seen;
            }

            public List<int> Decide(DecisionRequest r)
            {
                var picks = inner.Decide(r);
                foreach (var s in inner.Trajectory) seen(s.Input);
                inner.Trajectory.Clear();
                return picks;
            }

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                inner.OnEvent(engine, ev);
            }
        }
    }
}
