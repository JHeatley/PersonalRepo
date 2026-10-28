using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v83: Celestial Storm (CES 1-183).</summary>
    public class CesTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Dreepy = "TWM-128";
        const string Celesteela = "HIF-SV32";
        const string Magcargo = "CES-24";
        const string Swampert = "CES-35";
        const string Ludicolo = "CES-38";
        const string Wailord = "CES-40";
        const string Regice = "CES-45";
        const string Chinchou = "CES-49";
        const string Lanturn = "CES-50";
        const string Manectric = "CES-52";
        const string MrMimeGX = "CES-56";
        const string Lunatone = "CES-61";
        const string Solrock = "CES-62";
        const string Pupitar = "CES-75";
        const string Minior = "CES-83";
        const string Metagross = "CES-95";
        const string Jirachi = "CES-97";
        const string Solgaleo = "CES-99";
        const string RayquazaGX = "CES-109";
        const string Slaking = "CES-115";
        const string Whismur = "CES-116";
        const string BeastBall = "CES-125";
        const string HustleBelt = "CES-134";
        const string SkyPillar = "CES-144";
        const string StevensResolve = "CES-145";
        const string RainbowEnergy = "CES-151";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        /// <summary>Default bot answers, except that it asks to go first or second when it wins the opening coin flip.</summary>
        private sealed class OrderAgent : IPlayerAgent
        {
            private readonly bool second;

            public OrderAgent(bool second) { this.second = second; }

            public List<int> Decide(DecisionRequest r)
            {
                if (r.Kind == DecisionKind.GoFirstOrSecond)
                    return new List<int> { r.Options.FindIndex(o => o.Label == (second ? "Go second" : "Go first")) };
                if (r.Kind == DecisionKind.MainAction)
                    return new List<int> { r.Options.FindIndex(o => o.Action == ActionType.EndTurn) };
                int n = Math.Min(r.MaxChoices, Math.Max(r.MinChoices, 1));
                return Enumerable.Range(0, n).ToList();
            }
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "CES" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(134, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Celestial Storm", C(Manectric).PrintSetName);
            Assert.AreEqual("HIF-SV54", C("CES-31").BaseKey, "Articuno-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("MEG-125", C("CES-142").BaseKey, "so was Rare Candy");
            Assert.AreEqual("CES-14", C("CES-152").BaseKey, "the full art Shiftry-GX");
            Assert.AreEqual("CES-10", C("CES-10a").BaseKey);
            Assert.AreEqual("CES-86", C("CES-86").BaseKey, "Sneasel isn't the 30C Classic Collection one (Beat Up 30x, not 20x)");
            Assert.AreEqual("Jirachi ◇", C(Jirachi).Name);
            Assert.IsTrue(C(Jirachi).IsPrismStar);
            Assert.IsNull(C(MrMimeGX).Weakness, "Mr. Mime-GX has no Weakness");
            Assert.AreEqual(0, C("CES-46").Attacks[0].Damage, "Dual Splash's 30 goes to the 2 Pokémon you choose");
            Assert.AreEqual(20, C("CES-34").Attacks[0].Damage, "Muddy Water's printed 20 hits the Active Pokémon too (your ruling, v83)");
            Assert.AreEqual(10, C(Chinchou).Attacks[1].Damage, "so does Spark's printed 10");
            Assert.AreEqual(0, C("CES-20").Attacks[0].Damage, "Feint Attack's 50 is only in the text (v84 fix)");
            Assert.AreEqual(0, C("CES-69").Attacks[1].Damage, "so is Spear Dive's");
        }

        [Test]
        public void Electric_Start_Lets_A_Player_Going_Second_Keep_And_Start_Manectric()
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), new OrderAgent(true), new OrderAgent(false));
            var manectric = Enumerable.Repeat(C(Manectric), 60).ToList();
            var dreepy = Enumerable.Repeat(C(Dreepy), 60).ToList();
            e.Setup(manectric, dreepy, 7);
            Assert.AreEqual(1, e.State.FirstPlayer);
            Assert.AreEqual(0, e.State.Players[0].Mulligans, "a hand whose only Pokémon is Manectric is kept (your ruling, v83)");
            Assert.AreEqual("Manectric", e.State.Players[0].Active.Name, "face down in the Active Spot");
            Assert.IsTrue(e.State.Players[0].Bench.All(m => m.Name == "Manectric"));
            Assert.IsTrue(e.SetupPlaceable(0, C(Manectric)));
            Assert.IsFalse(e.SetupPlaceable(1, C(Manectric)), "not for the player going first");
        }

        [Test]
        public void Wish_Upon_A_Star_Benches_Jirachi_And_Takes_1_More_Prize()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            var p = e.State.Players[0];
            p.Prizes.Insert(0, e.NewCard(0, C(Jirachi)));
            e.ScenarioSetTurn(0, 3);
            e.TakePrizeCards(0, 1);
            Assert.IsTrue(p.Bench.Any(m => m.Name == "Jirachi ◇"), "benched instead of going to the hand");
            Assert.AreEqual(5, p.Prizes.Count, "1 more Prize card");
            Assert.AreEqual(1, p.Hand.Count);
        }

        [Test]
        public void Perish_Dream_Knocks_Out_The_Defending_Pokemon_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Perish Dream"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C(Jirachi));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(jirachi, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, target.DamageCounters);
            Assert.IsTrue(jirachi.Has(SpecialCondition.Asleep), "Jirachi falls asleep");
            NextTurn(e);
            Assert.IsNotNull(e.State.FindPokemon(target.Uid), "still in play during the opponent's turn");
            e.EndTurn();
            Assert.IsNull(e.State.FindPokemon(target.Uid), "Knocked Out at the end of the opponent's turn");
        }

        [Test]
        public void Magic_Evens_Prevents_Only_Even_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C(MrMimeGX));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(theirs, mime, 60);
            Assert.AreEqual(0, mime.DamageCounters, "60 is prevented");
            e.DealAttackDamage(theirs, mime, 30);
            Assert.AreEqual(3, mime.DamageCounters, "30 isn't");
            e.DealAttackDamage(theirs, mime, 280);
            Assert.AreEqual(31, mime.DamageCounters, "above 260 isn't either");
        }

        [Test]
        public void Lazy_Turns_Off_The_Opponents_Abilities_While_Slaking_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slaking = e.ScenarioActive(0, C(Slaking));
            var ludicolo = e.ScenarioBench(0, C(Ludicolo));
            var swampert = e.ScenarioActive(1, C(Swampert));
            var theirSlaking = e.ScenarioBench(1, C(Slaking));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.AbilityBlocked(swampert, "Power Draw"));
            Assert.IsFalse(e.AbilityBlocked(theirSlaking, "Lazy"), "except Lazy");
            Assert.IsFalse(e.AbilityBlocked(ludicolo, "Swing Dance"), "your own Abilities work");
            e.SwitchActive(0, ludicolo);
            Assert.IsFalse(e.AbilityBlocked(swampert, "Power Draw"), "only while Slaking is Active");
            Assert.IsTrue(e.State.Players[0].Bench.Contains(slaking));
        }

        [Test]
        public void Sol_Shade_And_Sunbeam_Work_With_Both_Lunatone_And_Solrock()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lunatone = e.ScenarioActive(0, C(Lunatone));
            var magcargo = e.ScenarioActive(1, C(Magcargo));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.AbilityBlocked(magcargo, "Smooth Over"), "no Solrock yet");
            Assert.AreEqual(80, e.MaxHp(lunatone));
            e.ScenarioBench(0, C(Solrock));
            Assert.IsTrue(e.AbilityBlocked(magcargo, "Smooth Over"), "Fire Pokémon have no Abilities");
            Assert.AreEqual(130, e.MaxHp(lunatone), "Sunbeam");
        }

        [Test]
        public void Sky_Pillar_Protects_Benched_Pokemon_From_The_Opponents_Attacks_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dust Devil"), new ScriptedAgent());
            var pupitar = e.ScenarioActive(0, C(Pupitar));
            e.ScenarioAttach(pupitar, C(Fighting));
            e.ScenarioAttach(pupitar, C(Fighting));
            var myBench = e.ScenarioBench(0, C(Snubbull));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            var theirBench = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetStadium(1, C(SkyPillar));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, theirs.DamageCounters, "the Active Pokémon isn't protected");
            Assert.AreEqual(0, theirBench.DamageCounters, "Sky Pillar");
            Assert.AreEqual(2, myBench.DamageCounters, "your own attack still reaches your Bench (Compendium)");
        }

        [Test]
        public void Hustle_Belt_Adds_60_At_30_HP_Or_Less()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttachTool(mine, C(HustleBelt));
            var theirs = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(mine, theirs, 10);
            Assert.AreEqual(1, theirs.DamageCounters, "no bonus at full HP");
            mine.DamageCounters = (C(Snubbull).Hp - 30) / 10;
            e.DealAttackDamage(mine, theirs, 10);
            Assert.AreEqual(8, theirs.DamageCounters, "10 + 60");
        }

        [Test]
        public void Stevens_Resolve_Ends_The_Turn_Unless_Metagross_Extends_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Steven's Resolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(StevensResolve));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "your turn ends");
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "a card was found");

            var withMetagross = Scenario(new ScriptedAgent().Then("Play Steven's Resolve"), new ScriptedAgent());
            withMetagross.ScenarioActive(0, C(Metagross));
            withMetagross.ScenarioActive(1, C(Snubbull));
            withMetagross.ScenarioAddToHand(0, C(StevensResolve));
            withMetagross.ScenarioSetTurn(0, 3);
            Assert.IsFalse(withMetagross.MainPhaseStep(), "Extend: the turn goes on");
        }

        [Test]
        public void Beast_Ball_Swaps_An_Ultra_Beast_Out_Of_The_Prize_Cards()
        {
            Assert.IsTrue(C(Celesteela).IsUltraBeast);
            var e = Scenario(new ScriptedAgent().Then("Play Beast Ball", "Celesteela"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            var p = e.State.Players[0];
            p.Prizes.Insert(0, e.NewCard(0, C(Celesteela)));
            e.ScenarioAddToHand(0, C(BeastBall));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(p.Hand.Any(c => c.Def.Name == "Celesteela"));
            Assert.IsTrue(p.Prizes.Any(c => c.Def.Name == "Beast Ball"), "Beast Ball took its place");
            Assert.AreEqual(7, p.Prizes.Count);
            Assert.IsFalse(p.Discard.Any(c => c.Def.Name == "Beast Ball"));
        }

        [Test]
        public void Energy_Grounding_Moves_A_Basic_Energy_From_A_Knocked_Out_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var victim = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioAttach(victim, C(Lightning));
            var lanturn = e.ScenarioBench(0, C(Lanturn));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(theirs, victim, 200);
            e.ResolveKnockouts();
            Assert.IsNull(e.State.FindPokemon(victim.Uid));
            Assert.AreEqual(1, lanturn.Energy.Count, "moved to Lanturn");
        }

        [Test]
        public void Dragon_Break_Counts_Basic_Grass_And_Lightning_Energy_On_All_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragon Break"), new ScriptedAgent());
            var rayquaza = e.ScenarioActive(0, C(RayquazaGX));
            e.ScenarioAttach(rayquaza, C(Grass));
            e.ScenarioAttach(rayquaza, C(Lightning));
            e.ScenarioAttach(rayquaza, C(Psychic));
            var bench = e.ScenarioBench(0, C(Snubbull));
            e.ScenarioAttach(bench, C(Lightning));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, theirs.DamageCounters, "30 x 3 (the Psychic Energy doesn't count)");
        }

        [Test]
        public void Rainbow_Energy_Provides_Every_Type_And_Puts_A_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Rainbow Energy"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(RainbowEnergy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mine.Energy.Count);
            Assert.AreEqual(1, mine.DamageCounters);
            var units = e.EnergyUnitsOf(mine, mine.Energy[0]);
            Assert.AreEqual(1, units.Count);
            Assert.IsTrue(EnergyMask.Provides(units[0], EnergyType.Fire) && EnergyMask.Provides(units[0], EnergyType.Metal));
        }

        [Test]
        public void Bawl_Is_Only_For_The_Second_Players_First_Turn_And_Locks_Trainers()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bawl"), new ScriptedAgent());
            var whismur = e.ScenarioActive(0, C(Whismur));
            e.ScenarioAttach(whismur, C(Psychic));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(0, 4);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Attack: Bawl")), "not after your first turn");
            e.ScenarioSetTurn(0, 2);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Attack: Bawl")), "going second, first turn");
            Step(e);
            var trainer = e.NewCard(1, C(BeastBall));
            var stadium = e.NewCard(1, C(SkyPillar));
            Assert.IsTrue(e.TrainerBlocked(1, trainer));
            Assert.IsTrue(e.TrainerBlocked(1, stadium), "every kind of Trainer card");
        }

        [Test]
        public void Falling_Star_Plays_Minior_As_The_New_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Falling Star"), new ScriptedAgent());
            var old = e.ScenarioActive(0, C(Snubbull));
            old.Conditions = SpecialCondition.Paralyzed;
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(Minior));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Minior", e.State.Players[0].Active.Name);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(old), "the old Active Pokémon moved to the Bench");
            Assert.AreEqual(SpecialCondition.None, old.Conditions);
        }

        [Test]
        public void Spark_Hits_The_Active_Pokemon_And_2_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spark"), new ScriptedAgent());
            var chinchou = e.ScenarioActive(0, C(Chinchou));
            e.ScenarioAttach(chinchou, C(Lightning));
            e.ScenarioAttach(chinchou, C(Psychic));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            var b1 = e.ScenarioBench(1, C(Snubbull));
            var b2 = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, theirs.DamageCounters, "the printed 10 (your ruling, v83)");
            Assert.AreEqual(1, b1.DamageCounters);
            Assert.AreEqual(1, b2.DamageCounters);
        }

        [Test]
        public void Dwindling_Wave_Does_40_Less_For_Each_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dwindling Wave"), new ScriptedAgent());
            var wailord = e.ScenarioActive(0, C(Wailord));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(wailord, C(Water));
            wailord.DamageCounters = 2;
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, theirs.DamageCounters, "200 - 2 x 40");
        }

        [Test]
        public void Icy_Barrier_Stops_The_Opponent_Playing_Stadiums()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Regice));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioSetTurn(1, 4);
            Assert.IsTrue(e.TrainerBlocked(1, e.NewCard(1, C(SkyPillar))));
            Assert.IsFalse(e.TrainerBlocked(1, e.NewCard(1, C(BeastBall))), "other Trainer cards are fine");
            Assert.IsFalse(e.TrainerBlocked(0, e.NewCard(0, C(SkyPillar))), "Regice's owner can play Stadiums");
        }

        [Test]
        public void Full_Metal_Body_Removes_Weakness_With_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var solgaleo = e.ScenarioActive(0, C(Solgaleo));
            e.ScenarioActive(1, C(Snubbull));
            Assert.AreEqual(EnergyType.Fire, e.WeaknessOf(solgaleo));
            e.ScenarioAttach(solgaleo, C(Metal));
            Assert.IsNull(e.WeaknessOf(solgaleo));
        }
    }
}
