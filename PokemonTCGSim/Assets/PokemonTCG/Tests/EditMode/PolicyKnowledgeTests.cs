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

        // ------------------------------------------------------------------ What a Masters player keeps track of

        [Test]
        public void Gust_And_Switch_Cards_Are_Recognised()
        {
            Assert.IsTrue(PolicyCards.Gusts(C("MEG-114")), "Boss's Orders");
            Assert.IsTrue(PolicyCards.Gusts(C("PAR-160")), "Counter Catcher");
            Assert.IsFalse(PolicyCards.Gusts(C("MEG-130")), "Switch moves your own Pokémon");
            Assert.IsTrue(PolicyCards.SwitchesOwn(C("MEG-130")), "Switch");
            Assert.IsTrue(PolicyCards.SwitchesOwn(C("BST-125")), "Escape Rope");
            Assert.IsFalse(PolicyCards.SwitchesOwn(C("MEG-114")), "Boss's Orders moves theirs");
            Assert.IsFalse(PolicyCards.Gusts(C("SVP-44")) || PolicyCards.SwitchesOwn(C("SVP-44")), "a Pokémon is neither");
        }

        [Test]
        public void Attack_Previews_Give_The_Real_Damage_Knock_Out_And_Prizes()
        {
            // Heat Tackle prints 30; Vitality Band adds 10, which only playing it out shows.
            var probe = new PreviewProbe();
            var e = ProbeScenario(probe);
            var charmander = e.ScenarioActive(0, C("SVP-44"));
            e.ScenarioAttach(charmander, C("MEE-10"));
            e.ScenarioAttachTool(charmander, C("SVI-197"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioBench(1, C("ASC-16"));
            var x = probe.Run(e);
            var attack = probe.Results.Single();
            Assert.AreEqual(40, attack.Value.Damage, "30 + 10 from Vitality Band");
            Assert.IsFalse(attack.Value.KnocksOut);
            Assert.AreEqual(0, attack.Value.Prizes);
            int f = PolicyInput.OptionFeatures;
            int i = Option(probe.Request, probe.Choices, o => o.Action == ActionType.Attack);
            Assert.AreEqual(40 / 170f / 2, x.OptFeat[i * f + 15], 1e-6, "the network sees 40 of Meowth ex's 170 HP");
            Assert.AreEqual(0, x.OptFeat[i * f + 14]);

            // Into Budew (30 HP, weak to Fire): a Knock Out and 1 Prize card.
            probe = new PreviewProbe();
            e = ProbeScenario(probe);
            charmander = e.ScenarioActive(0, C("SVP-44"));
            e.ScenarioAttach(charmander, C("MEE-10"));
            e.ScenarioActive(1, C("ASC-16"));
            e.ScenarioBench(1, C("POR-62"));
            x = probe.Run(e);
            attack = probe.Results.Single();
            Assert.IsTrue(attack.Value.KnocksOut);
            Assert.AreEqual(30, attack.Value.Damage, "the HP it had left");
            Assert.AreEqual(1, attack.Value.Prizes);
            i = Option(probe.Request, probe.Choices, o => o.Action == ActionType.Attack);
            Assert.AreEqual(1, x.OptFeat[i * f + 14]);
            Assert.AreEqual(1 / 3f, x.OptFeat[i * f + 21], 1e-6, "takes 1 Prize card");
        }

        [Test]
        public void The_Network_Sees_The_Prize_Race_And_The_Best_Gust_Target()
        {
            // Charmander (30 for R, 60 into Budew's Fire Weakness) against Meowth ex (170 HP, 2 Prizes) with Budew on the Bench.
            var probe = new PreviewProbe();
            var e = ProbeScenario(probe);
            var charmander = e.ScenarioActive(0, C("SVP-44"));
            e.ScenarioAttach(charmander, C("MEE-10"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioBench(1, C("ASC-16"));
            var x = probe.Run(e);

            int n = PolicyInput.SlotFeatures, theirs = PolicyInput.SideSlots;
            Assert.AreEqual(1, x.SlotFeat[(theirs + 1) * n + 47], "Budew is the Benched Pokémon to bring up and Knock Out");
            Assert.AreEqual(0, x.SlotFeat[theirs * n + 47], "not their Active");

            // Yours: Budew first (1 Prize in 1 turn), then Meowth ex (2 Prizes, 3 turns at 30), then 3 more Pokémon: 5 Knock
            // Outs in 7 turns, 1 Prize card in the next 2. Theirs: Charmander takes 3 turns (Itchy Pollen's 10, then Tuck Tail's
            // 60 once Meowth ex has the Energy), then 5 more Pokémon: 6 Knock Outs in 8 turns.
            int o = 2 + PolicyInput.SideSlots + 16;
            var tf = x.TurnFeats;
            Assert.AreEqual(5 / 6f, tf[o], 1e-6, "your Knock Outs needed");
            Assert.AreEqual(1, tf[o + 1], 1e-6, "theirs");
            Assert.AreEqual(0, tf[o + 2], "no multi-Prize Pokémon of yours in play");
            Assert.AreEqual(1 / 3f, tf[o + 3], 1e-6, "Meowth ex");
            Assert.AreEqual(7 / 8f, tf[o + 4], 1e-6, "your turns to win");
            Assert.AreEqual(1, tf[o + 5], 1e-6, "theirs");
            Assert.AreEqual(5 / 8f, tf[o + 6], 1e-6, "you're a turn ahead");
            Assert.AreEqual(1 / 6f, tf[o + 17], 1e-6, "1 Prize card in your next 2 turns");
            Assert.AreEqual(0, tf[o + 18], "none in theirs");
        }

        [Test]
        public void Known_Prizes_Outs_And_The_Opponents_Gust_And_Switch_Cards()
        {
            var probe = new PreviewProbe { Preview = false };
            var e = ProbeScenario(probe);
            var charmander = e.ScenarioActive(0, C("SVP-44"));
            e.ScenarioAttach(charmander, C("MEE-10"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioAddToHand(0, C("MEG-114"));
            e.ScenarioAddToHand(0, C("MEG-130"));
            e.ScenarioAddToDiscard(1, C("MEG-114"), 1);
            e.ScenarioAddToDiscard(1, C("BST-125"), 1);
            // The deck list: what's in play and in hand, 2 Counter Catcher, 2 Professor's Research and 9 Fire Energy: 1 attached,
            // 6 in the Prizes (all Fire Energy, as Scenario's are) and 2 left in the deck.
            probe.Memory.DeckList = new List<CardDef> { C("SVP-44"), C("MEE-10"), C("MEG-114"), C("MEG-130"), C("PAR-160"), C("PAR-160") };
            var research = db.All.First(d => d.Name == "Professor's Research");
            probe.Memory.DeckList.Add(research);
            probe.Memory.DeckList.Add(research);
            for (int k = 0; k < 8; k++) probe.Memory.DeckList.Add(C("MEE-10"));
            var catcher = e.ScenarioAddToHand(1, C("PAR-160"));
            probe.Memory.OppKnownHand[catcher.Id] = catcher.Def;
            // Searching the deck shows which cards are in the Prizes.
            probe.Memory.OnEvent(e, new GameEvent { Type = GameEventType.Search, Player = 0 });
            Assert.AreEqual(6, probe.Memory.KnownPrizes.Count);
            var x = probe.Run(e);

            int o = 2 + PolicyInput.SideSlots + 23;
            var tf = x.TurnFeats;
            Assert.AreEqual(1, tf[o], 1e-6, "all 6 Prize cards known");
            Assert.AreEqual(2 / 10f, tf[o + 1], 1e-6, "2 Supporters left in the deck");
            Assert.AreEqual(2 / 20f, tf[o + 2], 1e-6, "2 in 20 to draw one next");
            Assert.AreEqual(2 / 15f, tf[o + 3], 1e-6, "2 Fire Energy left in the deck (the other 6 are Prizes)");
            Assert.AreEqual(1 / 2f, tf[o + 4], 1e-6, "Boss's Orders in hand");
            Assert.AreEqual(2 / 4f, tf[o + 5], 1e-6, "2 Counter Catcher in the deck");
            Assert.AreEqual(1 / 2f, tf[o + 6], 1e-6, "Switch in hand");
            Assert.AreEqual(1 / 4f, tf[o + 7], 1e-6, "they've used a Boss's Orders");
            Assert.AreEqual(1 / 4f, tf[o + 8], 1e-6, "and an Escape Rope");
            Assert.AreEqual(1, tf[o + 9], "a Counter Catcher is known to be in their hand");
        }

        [Test]
        public void The_Network_Sees_Boss_Stall_Targets_And_Switch_Cards()
        {
            // Their Bench: Charmeleon (Retreat 2, R R attack, no Energy) would be stuck if brought up; Budew retreats for free.
            var probe = new PreviewProbe { Preview = false };
            var e = ProbeScenario(probe);
            var charmander = e.ScenarioActive(0, C("SVP-44"));
            e.ScenarioAttach(charmander, C("MEE-10"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioBench(1, C("ASC-16"));
            e.ScenarioBench(1, C("OBF-27"));
            var known = e.ScenarioAddToHand(1, C("MEG-130"));
            probe.Memory.OppKnownHand[known.Id] = known.Def;
            probe.Memory.DeckList = new List<CardDef> { C("SVP-44"), C("MEE-10"), C("MEG-130"), C("BST-125") };
            var x = probe.Run(e);

            int n = PolicyInput.SlotFeatures, theirs = PolicyInput.SideSlots;
            Assert.AreEqual(0, x.SlotFeat[(theirs + 1) * n + 48], "Budew isn't stuck (free retreat, free attack)");
            Assert.AreEqual(0.5f, x.SlotFeat[(theirs + 2) * n + 48], 1e-6, "Charmeleon is: it can't attack next turn and retreats for 2");
            Assert.AreEqual(1, x.SlotFeat[(theirs + 2) * n + 49], "the Boss-stall target");
            Assert.AreEqual(0, x.SlotFeat[(theirs + 1) * n + 49]);
            Assert.AreEqual(0, x.SlotFeat[theirs * n + 48], "their Active can't be brought up");

            int o = 2 + PolicyInput.SideSlots;
            Assert.AreEqual(1, x.TurnFeats[o + 35], "a Switch is known to be in their hand");
            Assert.AreEqual(2 / 4f, x.TurnFeats[o + 36], 1e-6, "Switch and Escape Rope left in your deck");
        }

        [Test]
        public void Draw_And_Mill_Cards_Are_Recognised()
        {
            var research = PolicyCards.CardDraws(db.All.First(d => d.Name == "Professor's Research"));
            Assert.AreEqual(7, research.Count, "Professor's Research draws 7");
            Assert.IsFalse(research.Refills);
            var iono = PolicyCards.CardDraws(db.All.First(d => d.Name == "Iono"));
            Assert.IsTrue(iono.PerPrize && iono.Refills, "Iono: shuffle the hand in, draw for each remaining Prize card");
            var until = PolicyCards.CardDraws(db.All.First(d => d.IsTrainer && (d.Text ?? "").StartsWith("Draw cards until you have 6 cards", System.StringComparison.Ordinal)));
            Assert.AreEqual(6, until.Until);
            Assert.AreEqual(2, PolicyCards.AttackMills(C("BKT-21").Attacks[0]), "Houndoom-EX discards the top 2 cards of their deck");
            Assert.IsFalse(PolicyCards.CardDraws(C("MEG-114")).Any, "Boss's Orders draws nothing");
        }

        [Test]
        public void The_Network_Sees_How_Deep_A_Play_Draws_And_Who_Is_Running_Out_Of_Cards()
        {
            // 20 cards in each deck: Professor's Research draws 7 of yours, Houndoom-EX's attack mills 2 of theirs.
            var probe = new PreviewProbe { Preview = false };
            var e = ProbeScenario(probe);
            var houndoom = e.ScenarioActive(0, C("BKT-21"));
            e.ScenarioAttach(houndoom, C("MEE-10"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioBench(1, C("ASC-16"));
            e.ScenarioAddToHand(0, db.All.First(d => d.Name == "Professor's Research"));
            var x = probe.Run(e);
            int f = PolicyInput.OptionFeatures;
            int play = Option(probe.Request, probe.Choices, o => o.Action == ActionType.PlayTrainer && o.Card != null && o.Card.Name == "Professor's Research");
            Assert.AreEqual(7 / 20f, x.OptFeat[play * f + 22], 1e-6, "draws 7 of the 20 cards left");
            int attack = Option(probe.Request, probe.Choices, o => o.Action == ActionType.Attack);
            Assert.AreEqual(2 / 20f, x.OptFeat[attack * f + 24], 1e-6, "mills 2 of their 20");
            int o0 = 2 + PolicyInput.SideSlots;
            Assert.AreEqual(1, x.TurnFeats[o0 + 37], 1e-6, "10 or more draws left");
            Assert.Greater(x.TurnFeats[o0 + 39], 0.25f, "cards to spare: drawing deep is safe");

            // With 3 cards left and 6 Prize cards to take, it would run out first.
            probe = new PreviewProbe { Preview = false };
            e = ProbeScenario(probe, 3);
            houndoom = e.ScenarioActive(0, C("BKT-21"));
            e.ScenarioAttach(houndoom, C("MEE-10"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioBench(1, C("ASC-16"));
            x = probe.Run(e);
            Assert.AreEqual(0.3f, x.TurnFeats[o0 + 37], 1e-6, "3 draws left");
            Assert.Less(x.TurnFeats[o0 + 39], 0.25f, "fewer cards than the turns it needs to win");
            Assert.Greater(x.TurnFeats[o0 + 40], 0.25f, "the opponent has cards to spare");
        }

        /// <summary>Earlier networks' sizes: per Pokémon, per option, turn numbers.</summary>
        [TestCase(47, 21, 16, TestName = "A network from before the Masters inputs loads and plays exactly as before")]
        [TestCase(48, 22, 35, TestName = "A network from before the Boss-stall and deck-out inputs loads and plays exactly as before")]
        public void An_Earlier_Network_Loads_And_Plays_Exactly_As_Before(int slots, int options, int turnExtra)
        {
            var net = new PolicyNet();
            var probe = new PreviewProbe { Net = net };
            var e = ProbeScenario(probe);
            var charmander = e.ScenarioActive(0, C("SVP-44"));
            e.ScenarioAttach(charmander, C("MEE-10"));
            e.ScenarioActive(1, C("POR-62"));
            e.ScenarioBench(1, C("ASC-16"));
            e.ScenarioBench(1, C("OBF-27"));
            var x = probe.Run(e);

            int turns = 2 + PolicyInput.SideSlots + turnExtra;
            var old = Narrow(net.ToBytes(), new Dictionary<string, int>
            {
                { "slot.0.weight", PolicyInput.SlotFeatures - slots }, { "option.0.weight", PolicyInput.OptionFeatures - options },
                { "inp.0.weight", PolicyInput.TurnFeatures - turns }
            });
            var loaded = new PolicyNet();
            Assert.IsTrue(loaded.FromBytes(old), "it loads");

            // It plays as the network it came from does with the newer numbers blank.
            for (int s = 0; s < PolicyInput.NumSlots; s++)
                for (int i = slots; i < PolicyInput.SlotFeatures; i++) x.SlotFeat[s * PolicyInput.SlotFeatures + i] = 0;
            for (int k = 0; k < x.Count; k++)
                for (int i = options; i < PolicyInput.OptionFeatures; i++) x.OptFeat[k * PolicyInput.OptionFeatures + i] = 0;
            for (int i = turns; i < PolicyInput.TurnFeatures; i++) x.TurnFeats[i] = 0;
            var want = net.Forward(x);
            var got = loaded.Forward(x);
            Assert.AreEqual(want.Value, got.Value, 1e-5);
            for (int i = 0; i < want.Logits.Length; i++) Assert.AreEqual(want.Logits[i], got.Logits[i], 1e-5);
        }

        /// <summary>The bytes of a saved network with the last columns of these weights removed (as an older network saved them).</summary>
        private static byte[] Narrow(byte[] bytes, Dictionary<string, int> cut)
        {
            var rows = new Dictionary<string, int> { { "slot.0.weight", PolicyNet.SlotDim }, { "option.0.weight", PolicyNet.OptionDim }, { "inp.0.weight", PolicyNet.Width } };
            using (var input = new System.IO.MemoryStream(bytes))
            using (var r = new System.IO.BinaryReader(input))
            using (var output = new System.IO.MemoryStream())
            using (var w = new System.IO.BinaryWriter(output))
            {
                w.Write(r.ReadBytes(8));
                w.Write(r.ReadInt32());
                w.Write(r.ReadInt64());
                w.Write(r.ReadInt64());
                w.Write(r.ReadDouble());
                w.Write(r.ReadString());
                int count = r.ReadInt32();
                w.Write(count);
                for (int k = 0; k < count; k++)
                {
                    string name = r.ReadString();
                    int len = r.ReadInt32();
                    var a = new float[len];
                    for (int i = 0; i < len; i++) a[i] = r.ReadSingle();
                    int less;
                    if (cut.TryGetValue(name, out less))
                    {
                        int n = rows[name], cols = len / n, keep = cols - less;
                        var b = new float[n * keep];
                        for (int row = 0; row < n; row++) System.Array.Copy(a, row * cols, b, row * keep, keep);
                        a = b;
                    }
                    w.Write(name);
                    w.Write(a.Length);
                    foreach (float v in a) w.Write(v);
                }
                w.Write(r.ReadBytes((int)(input.Length - input.Position)));
                w.Flush();
                return output.ToArray();
            }
        }

        /// <summary>Scenario's empty board (6 Fire Energy Prizes and 20 Psychic Energy in each deck, or myDeck in yours), with the probe as seat 0, on turn 3.</summary>
        private GameEngine ProbeScenario(PreviewProbe probe, int myDeck = 20)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), probe, new ScriptedAgent());
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C("MEE-13"), myDeck);
            e.ScenarioAddToDeck(1, C("MEE-13"), 20);
            probe.Engine = e;
            return e;
        }

        /// <summary>At its first main action, previews the attacks on offer (PolicySearch.PreviewAttacks) and encodes the decision with them; then ends the turn.</summary>
        private sealed class PreviewProbe : IPlayerAgent
        {
            public GameEngine Engine;
            public bool Preview = true;
            public readonly PolicyMemory Memory = new PolicyMemory(0) { DeckList = new List<CardDef>() };
            public DecisionRequest Request;
            public List<PolicyEncoder.Choice> Choices;
            public Dictionary<int, PolicySearch.AttackResult> Results;
            public PolicyInput Input;
            public PolicyNet Net = new PolicyNet();

            public PolicyInput Run(GameEngine e)
            {
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.NotNull(Input, "no main action came");
                return Input;
            }

            public List<int> Decide(DecisionRequest r)
            {
                if (r.Kind == DecisionKind.MainAction && Request == null)
                {
                    Request = r;
                    Memory.See(r.View);
                    Results = Preview ? new PolicySearch(Net, new SearchSettings()).PreviewAttacks(Engine, r, Memory) : null;
                    Choices = PolicyEncoder.Choices(r, new List<int>());
                    Input = PolicyEncoder.Encode(r, Memory, Net, Choices, Results);
                }
                for (int i = 0; i < r.Options.Count; i++)
                    if (r.Options[i].Action == ActionType.EndTurn) return new List<int> { i };
                return r.MinChoices > 0 ? new List<int> { 0 } : new List<int>();
            }
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
