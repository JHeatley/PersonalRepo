using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v60: Battle Styles (BST 1-183). Your ruling: Aegislash's Stance Change can't be used again that turn by the Aegislash
    /// it brings in.
    /// </summary>
    public class BstTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string Tauros = "MEW-128";
        const string PokeBall = "POR-80";
        const string GreatBall = "PAL-183";
        const string BlisseyV = "CRE-119";
        const string Kecleon = "CRE-122";
        const string SlowkingV = "CRE-99";
        const string Cinderace = "CRE-28";
        const string Chestplate = "CRE-141";
        const string UrnOfVitality = "CRE-229";

        const string Cherrim = "BST-8";
        const string KricketuneV = "BST-6";
        const string FlappleV = "BST-18";
        const string Pignite = "BST-24";
        const string Tepig = "BST-23";
        const string Emboar = "BST-25";
        const string Centiskorch = "BST-30";
        const string MrRime = "BST-35";
        const string EmpoleonV = "BST-40";
        const string TapuKokoV = "BST-50";
        const string TapuKokoVmax = "BST-51";
        const string Conkeldurr = "BST-75";
        const string Mienfoo = "BST-76";
        const string Sandaconda = "BST-82";
        const string Stonjourner = "BST-84";
        const string Pawniard = "BST-103";
        const string AegislashBlade = "BST-107";
        const string AegislashShield = "BST-108";
        const string Lickilicky = "BST-114";
        const string Bruno = "BST-121";
        const string Cheryl = "BST-123";
        const string EscapeRope = "BST-125";
        const string Phoebe = "BST-130";
        const string ToolJammer = "BST-136";
        const string TowerOfWaters = "BST-138";
        const string RapidStrikeEnergy = "BST-140";
        const string SingleStrikeEnergy = "BST-141";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
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

        // ================================================================== Data

        [Test]
        public void Every_Battle_Styles_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 183; n++) Assert.IsNotNull(db.Get("BST-" + n), "BST " + n + " is missing");
            Assert.AreEqual("SIT-TG12", db.MainPrinting(C("BST-6")).Key, "Kricketune V was already in the pool");
            Assert.AreEqual("BRS-TG18", db.MainPrinting(C("BST-150")).Key, "and Single Strike Urshifu V");
            Assert.AreEqual("CRE-226", db.MainPrinting(C("BST-127")).Key, "Fan of Waves came with Chilling Reign");
            Assert.AreEqual("CRE-229", db.MainPrinting(C("BST-139")).Key, "and Urn of Vitality");
            Assert.AreEqual("BST-129", db.MainPrinting(C("BST-181")).Key, "the gold Level Ball");
            Assert.AreEqual("BST-141", db.MainPrinting(C("BST-183")).Key, "the gold Single Strike Energy");
            Assert.IsFalse(Formats.IsStandardCard(C(Emboar)), "regulation mark E: Expanded");
            Assert.IsTrue(C(Tepig).IsSingleStrike);
            Assert.IsTrue(C(Mienfoo).IsRapidStrike);
            Assert.IsTrue(C("BST-128").IsRapidStrike, "Korrina's Focus is a Rapid Strike Supporter");
            Assert.IsTrue(C(SingleStrikeEnergy).IsSingleStrike && C(SingleStrikeEnergy).IsSpecialEnergy);
            Assert.AreEqual(3, C("BST-131").Attacks[0].Cost.Length, "Matchless Maelstrom costs [F][C][C]");
        }

        // ================================================================== Your ruling

        [Test]
        public void Stance_Change_Swaps_In_The_Other_Aegislash_Once_Per_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Stance Change"), new ScriptedAgent());
            var aegislash = e.ScenarioActive(0, C(AegislashBlade));
            Attach(e, aegislash, Metal, 2);
            aegislash.DamageCounters = 3;
            e.ScenarioAddToHand(0, C(AegislashShield));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Gigaton Bash", aegislash.Def.Attacks[0].Name, "the other Aegislash is in play");
            Assert.AreEqual(3, aegislash.DamageCounters);
            Assert.AreEqual(2, aegislash.Energy.Count);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Attacks.Any(a => a.Name == "Full Metal Blade")), "the first one went to the hand");
            Assert.IsFalse(Offered(e, 0, "Use Stance Change"), "your ruling: not again this turn");
        }

        // ================================================================== Pokémon

        [Test]
        public void Spring_Bloom_Attaches_Grass_Energy_From_The_Hand_To_A_Pokemon_Without_A_Rule_Box()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Spring Bloom", "Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Cherrim));
            e.ScenarioBench(0, C(KricketuneV));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.IsTrue(Offered(e, 0, "Use Spring Bloom"), "as often as you like");
        }

        [Test]
        public void Fighting_Fury_Stance_Adds_30_To_Single_Strike_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Land's Pulse"), new ScriptedAgent());
            var stonjourner = e.ScenarioActive(0, C(Stonjourner));
            Attach(e, stonjourner, Fighting, 1);
            Attach(e, stonjourner, Psychic, 1);
            e.ScenarioBench(0, C(Emboar));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters, "(30 + 30) x2 Weakness");
        }

        [Test]
        public void Single_Strike_Energy_Pays_Fighting_And_Adds_20()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Land's Pulse"), new ScriptedAgent());
            var stonjourner = e.ScenarioActive(0, C(Stonjourner));
            e.ScenarioAttach(stonjourner, C(SingleStrikeEnergy));
            Attach(e, stonjourner, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, blissey.DamageCounters, "(30 + 20) x2 Weakness");
        }

        [Test]
        public void Rapid_Strike_Energy_Gives_A_Rapid_Strike_Pokemon_2_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mienfoo = e.ScenarioActive(0, C(Mienfoo));
            e.ScenarioAttach(mienfoo, C(RapidStrikeEnergy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Double Stab"), "[F][C] paid by one card");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(RapidStrikeEnergy));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Attach Rapid Strike Energy"));
        }

        [Test]
        public void Emperors_Eyes_Turns_Off_The_Opponents_Basic_Pokemon_Abilities_While_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(EmpoleonV));
            var kecleon = e.ScenarioActive(1, C(Kecleon));
            Attach(e, kecleon, Fire, 1);
            var blissey = e.ScenarioBench(1, C(BlisseyV));
            Assert.IsTrue(e.AbilityBlocked(kecleon, "Chromashift"));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Fire));
            Assert.IsFalse(e.AbilityBlocked(blissey, "Natural Cure"), "Pokémon with a Rule Box keep theirs");
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.SwitchActive(0, tauros);
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Fire), "only while Empoleon V is Active");
        }

        [Test]
        public void Overheater_Keeps_The_Opponents_Burned_Pokemon_Burned()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Centiskorch));
            var theirs = e.ScenarioActive(1, C(Tauros));
            theirs.Conditions |= SpecialCondition.Burned;
            mine.Conditions |= SpecialCondition.Burned;
            Assert.IsTrue(e.BurnRecoveryBlocked(theirs));
            Assert.IsFalse(e.BurnRecoveryBlocked(mine), "only the opponent's");
        }

        [Test]
        public void Ball_Juggling_Adds_40_For_Each_Ball_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ball Juggling", "Poké Ball|Poké Ball|Great Ball"), new ScriptedAgent());
            var rime = e.ScenarioActive(0, C(MrRime));
            Attach(e, rime, Psychic, 2);
            e.ScenarioAddToHand(0, C(PokeBall));
            e.ScenarioAddToHand(0, C(PokeBall));
            e.ScenarioAddToHand(0, C(GreatBall));
            e.ScenarioAddToHand(0, C(Grass));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, blissey.DamageCounters, "10 + 3 x 40");
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Big_Sand_Cannon_Counts_Fighting_Energy_In_The_Top_6()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Big Sand Cannon"), new ScriptedAgent());
            var sandaconda = e.ScenarioActive(0, C(Sandaconda));
            Attach(e, sandaconda, Fighting, 1);
            var deck = e.State.Players[0].Deck;
            deck.Insert(0, e.NewCard(0, C(Fighting)));
            deck.Insert(0, e.NewCard(0, C(Fighting)));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(24, blissey.DamageCounters, "2 x 60, x2 Weakness");
            Assert.AreEqual(6, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Selickt_Lets_The_Opponent_Choose_What_To_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Selickt"), new ScriptedAgent().Then("top 3 cards"));
            var lickilicky = e.ScenarioActive(0, C(Lickilicky));
            Attach(e, lickilicky, Psychic, 3);
            e.ScenarioActive(1, C(Tauros));
            int deck = e.State.Players[1].Deck.Count;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(deck - 3, e.State.Players[1].Deck.Count);
            Assert.AreEqual(3, e.State.Players[1].Discard.Count);
        }

        [Test]
        public void Sour_Spit_Makes_The_Defending_Pokemons_Attacks_Cost_2_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sour Spit"), new ScriptedAgent());
            var flapple = e.ScenarioActive(0, C(FlappleV));
            Attach(e, flapple, Grass, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(3, e.AttackCost(tauros, tauros.Def.Attacks[0]).Count);
        }

        [Test]
        public void Swords_Dance_Powers_Up_Next_Turns_Slash()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Swords Dance", "Attack: Slash"), new ScriptedAgent());
            var pawniard = e.ScenarioActive(0, C(Pawniard));
            Attach(e, pawniard, Metal, 1);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(8, blissey.DamageCounters, "10 + 70");
        }

        [Test]
        public void Gigaton_Bash_Stops_Damage_From_Pokemon_VMAX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gigaton Bash"), new ScriptedAgent().Then("Attack: Max Shock"));
            var aegislash = e.ScenarioActive(0, C(AegislashShield));
            Attach(e, aegislash, Metal, 2);
            var koko = e.ScenarioActive(1, C(TapuKokoV));
            e.ScenarioEvolve(koko, C(TapuKokoVmax));
            Attach(e, koko, Lightning, 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, koko.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(0, aegislash.DamageCounters);
        }

        [Test]
        public void Hammer_Pressure_Stops_An_Evolution_Pokemon_Attacking()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hammer Pressure"), new ScriptedAgent());
            var conkeldurr = e.ScenarioActive(0, C(Conkeldurr));
            Attach(e, conkeldurr, Fighting, 1);
            Attach(e, conkeldurr, Psychic, 2);
            var pignite = e.ScenarioActive(1, C(Pignite));
            Attach(e, pignite, Fire, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Attack: Ram"));
        }

        // ================================================================== Trainers

        [Test]
        public void Tool_Jammer_Turns_Off_The_Opponents_Active_Tool()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fireball Shot"), new ScriptedAgent());
            var cinderace = e.ScenarioActive(0, C(Cinderace));
            Attach(e, cinderace, Fire, 2);
            e.ScenarioAttachTool(cinderace, C(ToolJammer));
            var slowking = e.ScenarioActive(1, C(SlowkingV));
            e.ScenarioAttachTool(slowking, C(Chestplate));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, slowking.DamageCounters, "Galarian Chestplate does nothing");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Fireball Shot"), new ScriptedAgent());
            var c2 = e2.ScenarioActive(0, C(Cinderace));
            Attach(e2, c2, Fire, 2);
            var s2 = e2.ScenarioActive(1, C(SlowkingV));
            e2.ScenarioAttachTool(s2, C(Chestplate));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(12, s2.DamageCounters, "without Tool Jammer: 30 less");
        }

        [Test]
        public void Phoebe_Lets_Pokemon_VMAX_Ignore_Effects_On_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Phoebe", "Attack: Max Shock"), new ScriptedAgent());
            var koko = e.ScenarioActive(0, C(TapuKokoV));
            e.ScenarioEvolve(koko, C(TapuKokoVmax));
            Attach(e, koko, Lightning, 3);
            e.ScenarioAddToHand(0, C(Phoebe));
            var slowking = e.ScenarioActive(1, C(SlowkingV));
            e.ScenarioAttachTool(slowking, C(Chestplate));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(18, slowking.DamageCounters, "Galarian Chestplate ignored");
        }

        [Test]
        public void Tower_Of_Waters_Lowers_Rapid_Strike_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mienfoo = e.ScenarioActive(0, C(Mienfoo));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(0, C(TowerOfWaters));
            Assert.AreEqual(0, e.RetreatCostOf(mienfoo));
            Assert.AreEqual(2, e.RetreatCostOf(tauros));
        }

        [Test]
        public void Bruno_Draws_7_After_A_Knock_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bruno"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Bruno));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].LastKnockedOutTurn = 2;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Cheryl_Heals_Evolution_Pokemon_And_Discards_Their_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cheryl"), new ScriptedAgent());
            var pignite = e.ScenarioActive(0, C(Pignite));
            pignite.DamageCounters = 4;
            Attach(e, pignite, Fire, 2);
            var tepig = e.ScenarioBench(0, C(Tepig));
            tepig.DamageCounters = 2;
            e.ScenarioAddToHand(0, C(Cheryl));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, pignite.DamageCounters);
            Assert.AreEqual(0, pignite.Energy.Count);
            Assert.AreEqual(2, tepig.DamageCounters, "Basic Pokémon aren't healed");
        }

        [Test]
        public void Escape_Rope_Switches_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Escape Rope"), new ScriptedAgent());
            var a = e.ScenarioActive(0, C(Tauros));
            var b = e.ScenarioBench(0, C(Tepig));
            e.ScenarioAddToHand(0, C(EscapeRope));
            var c = e.ScenarioActive(1, C(Tauros));
            var d = e.ScenarioBench(1, C(Tepig));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(b, e.State.Players[0].Active);
            Assert.AreEqual(d, e.State.Players[1].Active);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(a) && e.State.Players[1].Bench.Contains(c));
        }

        [Test]
        public void Urn_Of_Vitality_Takes_Single_Strike_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(UrnOfVitality));
            e.ScenarioAddToDiscard(0, C(SingleStrikeEnergy), 2);
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Urn of Vitality"));
        }
    }
}
