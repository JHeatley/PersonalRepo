using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// Rulings from the Pokémon TCG Rulings Compendium (compendium.pokegym.net) that the engine was checked against
    /// (RULES_NOTES 159 onward).
    /// </summary>
    public class CompendiumTests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Psychic = "MEE-13";
        const string Castle = "JTG-152";     // N's Castle: N's Pokémon have no Retreat Cost
        const string Ariados = "TWM-5";      // Big Net: the opponent's Active Evolution Pokémon's Retreat Cost is 1 more
        const string NsZoroark = "JTG-98";   // N's Zoroark ex (Stage 1)
        const string Clefable = "TWM-79";    // Metronome: use 1 of the opponent's Active Pokémon's attacks
        const string Palkia = "30C-20";      // Wormhole
        const string Latias = "SVP-139";     // Mist Float: no Retreat Cost with Psychic Energy attached
        const string GravityGemstone = "SCR-137"; // both Active Pokémon's Retreat Cost is 1 more
        const string PalkiaLvX = "30C-CC12"; // Hydro Reflect
        const string Lightning = "MEE-12";
        const string Ampharos = "SVP-16";    // Ampharos ex: Thunderstrike Tail (140, may discard 2 Energy for 100 more)
        const string GengarEx = "30C-90";    // 280 HP, weak to Fighting
        const string Water = "MEE-11";
        const string Primarina = "PBL-20";   // Aqua Return: 120, then shuffle this Pokémon and everything attached into the deck
        const string Wailord = "PBL-16";     // Wailord ex, 380 HP
        const string HandheldFan = "TWM-150";// damaged in the Active Spot: move an Energy from the Attacking Pokémon to its Bench
        const string Pancham = "ASC-115";    // Reckless Charge: 20, and 10 damage to itself
        const string IronValiant = "SVP-68"; // Tachyon Bits: moved from your Bench to the Active Spot -> 2 damage counters
        const string Darkness = "MEE-15";
        const string Umbreon = "PRE-59";     // Feint Attack: 50 to any opponent's Pokémon, not affected by effects on it
        const string Miraidon = "ASC-73";    // Miraidon ex, a Tera Pokémon
        const string Buzzwole = "30C-CC24";  // Buzzwole-GX: Absorption-GX
        const string Cryogonal = "BLK-27";   // Drag Off: switch in a Benched Pokémon, 20 damage to the new Active Pokémon
        const string Kyurem = "SFA-47";      // Trifrost costs only [C] if the opponent has a "Colress" card in their discard pile
        const string ColresssTenacity = "SFA-57";
        const string RootFossil = "SCR-130"; // Primal Root: while Active, the opponent's Basic Pokémon's attacks cost [C] more
        const string Fighting = "MEE-14";
        const string Cranidos = "PBL-44";    // Push Down: 70, then the opponent switches their Active Pokémon
        const string Maractus = "JTG-8";     // Exploding Needles: KO'd in the Active Spot by an attack -> 6 counters on the attacker
        const string Cofagrigus = "WHT-40";  // Extended Damagriiigus: move every counter from 1 of your Benched Pokémon to 1 of the opponent's
        const string Poltchageist = "TWM-20";// Storehouse Hideaway: on the Bench, prevent damage and effects of the opponent's attacks
        const string MewEx = "SVP-53";       // Genome Hacking: use 1 of the opponent's Active Pokémon's attacks
        const string Feraligatr = "TEF-41";  // Giant Wave: 160; this Pokémon can't use Giant Wave during your next turn
        const string GengarCC = "30C-CC15";  // Catastrophe (Poké-Body): while Active, the opponent's KO'd Pokémon go to the Lost Zone
        const string MegatonBlower = "SSP-182";
        const string AreaZero = "SCR-131";   // Area Zero Underdepths: up to 8 Benched Pokémon with a Tera Pokémon in play
        const string SkullFossil = "PBL-73"; // Antique Skull Fossil (Cranidos evolves from it)
        const string RampardosEx = "PBL-45"; // Stage 2, evolves from Cranidos
        const string RareCandy = "MEG-125";

        // ================================================================== Meta-Rulings

        [Test]
        public void A_Retreat_Cost_Set_To_Zero_Cannot_Be_Increased()
        {
            // Latias's Mist Float (no Retreat Cost with Psychic Energy attached) is applied before the opponent's Gravity
            // Gemstone (+1 to both Active Pokémon), so adding them up in order would give 1.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var latias = e.ScenarioActive(0, C(Latias));
            var dreepy = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAttachTool(dreepy, C(GravityGemstone));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(C(Latias).RetreatCost + 1, e.RetreatCostOf(latias), "no Psychic Energy yet: Gravity Gemstone adds 1");

            e.ScenarioAttach(latias, C(Psychic));
            Assert.AreEqual(0, e.RetreatCostOf(latias), "no Retreat Cost can't be increased (Compendium meta-ruling)");
        }

        [Test]
        public void A_Stadium_That_Sets_Retreat_Cost_To_Zero_Beats_An_Increase()
        {
            // N's Castle sets N's Pokémon's Retreat Cost to zero; Big Net's +1 doesn't get past it.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Ariados));
            var zoroark = e.ScenarioActive(1, C(NsZoroark));
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(C(NsZoroark).RetreatCost + 1, e.RetreatCostOf(zoroark), "Big Net adds 1");
            e.State.Stadium = e.NewCard(1, C(Castle));
            Assert.AreEqual(0, e.RetreatCostOf(zoroark));
        }

        [Test]
        public void Copying_A_Lv_X_Can_Copy_The_Attacks_Of_The_Pokemon_Under_It()
        {
            var me = new ScriptedAgent().Then("Attack: Metronome", "Palkia: Wormhole");
            var e = Scenario(me, new ScriptedAgent());
            var clefable = e.ScenarioActive(0, C(Clefable));
            e.ScenarioAttach(clefable, C(Psychic));
            e.ScenarioAttach(clefable, C(Psychic));
            var palkia = e.ScenarioActive(1, C(Palkia));
            e.ScenarioEvolve(palkia, C(PalkiaLvX));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            var choice = me.Seen.FirstOrDefault(r => (r.Prompt ?? "").StartsWith("Metronome"));
            Assert.IsNotNull(choice);
            Assert.IsTrue(choice.Options.Any(o => o.Label.StartsWith("Palkia LV.X: Hydro Reflect")), "its own printed attack");
            Assert.IsTrue(choice.Options.Any(o => o.Label.StartsWith("Palkia: Wormhole")), "a LV.X is an extension of the Pokémon under it");
            Assert.AreEqual(10, palkia.DamageCounters, "Wormhole's 100 damage");
        }

        // ================================================================== Attacks

        [Test]
        public void Thunderstrike_Tail_Does_100_More_Even_With_Fewer_Than_2_Energy()
        {
            // Compendium (2024-03-07): you choose to do the extra damage, then discard 2 Energy if you can, fewer if you can't.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent()); // answers "Yes"
            var ampharos = e.ScenarioActive(0, C(Ampharos));
            e.ScenarioAttach(ampharos, C(Lightning));
            var gengar = e.ScenarioActive(1, C(GengarEx));
            e.ScenarioSetTurn(0, 3);
            var def = C(Ampharos);
            int index = -1;
            for (int i = 0; i < def.Attacks.Count; i++) if (def.Attacks[i].Name == "Thunderstrike Tail") index = i;
            Assert.GreaterOrEqual(index, 0);
            e.Scripts.For(def).ResolveAttack(new EffectContext(e, 0, ampharos.Top, ampharos), index);
            Assert.AreEqual(0, ampharos.Energy.Count, "its only Energy is discarded");
            Assert.AreEqual(24, gengar.DamageCounters, "140 + 100");
        }

        [Test]
        public void An_Attacks_Own_Effects_Happen_Before_The_Defending_Pokemons_Triggers()
        {
            // Compendium (2024-12-19, Lumineon V's Aqua Return vs Handheld Fan): the attacker is shuffled into the deck
            // first, so Handheld Fan has nothing to move.
            var e = Scenario(new ScriptedAgent().Then("Attack: Aqua Return"), new ScriptedAgent());
            var primarina = e.ScenarioActive(0, C(Primarina));
            e.ScenarioAttach(primarina, C(Water));
            e.ScenarioAttach(primarina, C(Water));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            var wailord = e.ScenarioActive(1, C(Wailord));
            e.ScenarioAttachTool(wailord, C(HandheldFan));
            int deckBefore = e.State.Players[0].Deck.Count;
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.AreEqual(12, wailord.DamageCounters);
            Assert.IsNull(e.State.FindPokemon(primarina.Uid), "Primarina went back into the deck");
            Assert.AreEqual(0, dreepy.Energy.Count, "Handheld Fan found no Attacking Pokémon to move Energy from");
            Assert.AreEqual(deckBefore + 3, e.State.Players[0].Deck.Count, "Primarina and both Energy were shuffled in");
        }

        [Test]
        public void When_Both_Players_Win_At_Once_Effects_Finish_And_More_Win_Conditions_Wins()
        {
            // Compendium (2024-09-26, Reckless Charge vs Tachyon Bits): both Active Pokémon are Knocked Out and both players
            // take their last Prize card. The attacker promotes Iron Valiant ex, whose Tachyon Bits Knocks Out the
            // opponent's last Pokémon: 2 win conditions to 1, so the attacker wins instead of a tie.
            var e = Scenario(new ScriptedAgent().Then("Attack: Reckless Charge"), new ScriptedAgent());
            e.State.Players[0].Prizes.RemoveRange(0, 5);
            e.State.Players[1].Prizes.RemoveRange(0, 5);
            var pancham = e.ScenarioActive(0, C(Pancham));
            e.ScenarioAttach(pancham, C(Psychic));
            pancham.DamageCounters = 6;               // its 10 damage to itself Knocks it Out
            e.ScenarioBench(0, C(IronValiant));
            var defending = e.ScenarioActive(1, C(Dreepy));
            defending.DamageCounters = 5;             // Reckless Charge Knocks it Out
            var last = e.ScenarioBench(1, C(Dreepy));
            last.DamageCounters = 6;                  // 10 HP left: Tachyon Bits Knocks it Out
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.IsTrue(e.State.IsOver);
            Assert.AreEqual(0, e.State.Winner, e.State.EndReason);
        }

        [Test]
        public void Feint_Attack_Gets_Through_A_Benched_Tera_Pokemons_Protection()
        {
            // Compendium (2024-01-04): the Tera rule is an effect on that Pokémon, and Feint Attack ignores those.
            var e = Scenario(new ScriptedAgent().Then("Attack: Feint Attack", "Miraidon ex"), new ScriptedAgent());
            var umbreon = e.ScenarioActive(0, C(Umbreon));
            e.ScenarioAttach(umbreon, C(Darkness));
            e.ScenarioActive(1, C(Dreepy));
            var miraidon = e.ScenarioBench(1, C(Miraidon));
            Assert.IsTrue(miraidon.Def.IsTera);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, miraidon.DamageCounters);
        }

        [Test]
        public void Copying_A_Gx_Attack_Uses_Your_Gx_Attack()
        {
            // Compendium (Genome Hacking and VSTAR attacks, 2023-12-14), applied to GX attacks: you can't pick one once
            // you've used yours, and using one this way uses it up.
            foreach (bool usedAlready in new[] { false, true })
            {
                var me = new ScriptedAgent().Then(usedAlready ? new[] { "Attack: Metronome", "Buzzwole-GX: Jet Punch" } : new[] { "Attack: Metronome", "Buzzwole-GX: Absorption-GX" });
                var e = Scenario(me, new ScriptedAgent());
                var clefable = e.ScenarioActive(0, C(Clefable));
                e.ScenarioAttach(clefable, C(Psychic));
                e.ScenarioAttach(clefable, C(Psychic));
                e.ScenarioActive(1, C(Buzzwole));
                e.State.Players[0].GxUsed = usedAlready;
                e.ScenarioSetTurn(0, 3);
                Step(e);

                var choice = me.Seen.First(r => (r.Prompt ?? "").StartsWith("Metronome"));
                Assert.AreEqual(!usedAlready, choice.Options.Any(o => o.Label.Contains("Absorption-GX")), "offered only while your GX attack is unused");
                Assert.IsTrue(e.State.Players[0].GxUsed);
            }
        }

        [Test]
        public void A_Search_For_Any_Card_Must_Take_At_Least_One()
        {
            // Compendium (Beak Catch, Shinobi Blade): a card can't fail to be found when any card will do, so at least 1 is
            // taken; a search with a condition ("a Basic Energy card") may still find nothing.
            var me = new ScriptedAgent();
            var e = Scenario(me, new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            var ctx = new EffectContext(e, 0, null, null);

            ctx.SearchToHand(d => true, 2, "Search your deck for up to 2 cards", false, 1);
            Assert.AreEqual(1, me.Seen.Last(r => r.Kind == DecisionKind.ChooseCards).MinChoices);

            ctx.SearchToHand(d => d.IsBasicEnergy, 1, "Search your deck for a Basic Energy card", false);
            Assert.AreEqual(0, me.Seen.Last(r => r.Kind == DecisionKind.ChooseCards).MinChoices);
        }

        [Test]
        public void Drag_Off_Does_No_Damage_Without_A_New_Active_Pokemon()
        {
            // Compendium (Nidoking's Drag Off, Probopass's Magnetic Pull): no switch means no "new Active Pokémon" to damage.
            var e = Scenario(new ScriptedAgent().Then("Attack: Drag Off"), new ScriptedAgent());
            var cryogonal = e.ScenarioActive(0, C(Cryogonal));
            e.ScenarioAttach(cryogonal, C(Water));
            var defending = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, defending.DamageCounters);
        }

        [Test]
        public void An_Attack_Cost_Set_By_An_Effect_Cannot_Be_Increased()
        {
            // Compendium (Plasma Bane vs Pokémon League Headquarters, 2024-09-12; Insight vs Headwind, 2011-09-29).
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C(Kyurem));
            e.ScenarioActive(1, C(RootFossil));
            e.ScenarioSetTurn(0, 3);
            AttackDef trifrost = null;
            foreach (var a in kyurem.Def.Attacks) if (a.Name == "Trifrost") trifrost = a;
            Assert.IsNotNull(trifrost);
            Assert.AreEqual(trifrost.Cost.Length + 1, e.AttackCost(kyurem, trifrost).Count, "Primal Root adds [C] to the printed cost");

            e.State.Players[1].Discard.Add(e.NewCard(1, C(ColresssTenacity)));
            Assert.AreEqual(1, e.AttackCost(kyurem, trifrost).Count, "a cost set to [C] isn't increased");
        }

        // ================================================================== Abilities

        [Test]
        public void Knocked_Out_In_The_Active_Spot_Counts_When_The_Attack_Then_Moved_It_To_The_Bench()
        {
            // Compendium (Voltorb's Destiny Burst vs Blastoise-EX's Rapid Spin, 2014, reaffirmed 2018): it was damaged in the
            // Active Spot and Knocked Out by that attack, even though the KO is checked after the attack moved it to the Bench.
            var e = Scenario(new ScriptedAgent().Then("Attack: Push Down"), new ScriptedAgent());
            var cranidos = e.ScenarioActive(0, C(Cranidos));
            e.ScenarioAttach(cranidos, C(Fighting));
            e.ScenarioAttach(cranidos, C(Fighting));
            var maractus = e.ScenarioActive(1, C(Maractus));
            maractus.DamageCounters = 5; // 110 HP: Push Down's 70 Knocks it Out
            var dreepy = e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.IsNull(e.State.FindPokemon(maractus.Uid), "Maractus was Knocked Out");
            Assert.AreSame(dreepy, e.State.Players[1].Active, "the opponent switched before the Knock Out was checked");
            Assert.AreEqual(6, cranidos.DamageCounters, "Exploding Needles still puts 6 damage counters on the Attacking Pokémon");
        }

        [Test]
        public void Damage_Counters_Moved_Onto_A_Protected_Pokemon_Are_Removed()
        {
            // Compendium (Shady Move vs Daunting Pose, 2017; Painful Spoons vs Fusion Strike Energy, 2023).
            var e = Scenario(new ScriptedAgent().Then("Attack: Extended Damagriiigus", "Poltchageist"), new ScriptedAgent());
            var cofagrigus = e.ScenarioActive(0, C(Cofagrigus));
            e.ScenarioAttach(cofagrigus, C(Psychic));
            e.ScenarioAttach(cofagrigus, C(Psychic));
            var hurt = e.ScenarioBench(0, C(Dreepy));
            hurt.DamageCounters = 4;
            e.ScenarioActive(1, C(Dreepy));
            var poltchageist = e.ScenarioBench(1, C(Poltchageist));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.AreEqual(0, poltchageist.DamageCounters, "Storehouse Hideaway prevents the counters being placed on it");
            Assert.AreEqual(0, hurt.DamageCounters, "but they still leave the Pokémon they were moved from");
        }

        [Test]
        public void A_Copied_Attack_That_Cannot_Be_Used_Next_Turn_Cannot_Be_Copied_Next_Turn()
        {
            // Compendium (Mew-EX's Versatile copying X Blast, 2015-02-05): the copied attack is used by its name.
            var e = Scenario(new ScriptedAgent().Then("Attack: Genome Hacking", "Attack: Genome Hacking"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(MewEx));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mew, C(Psychic));
            var feraligatr = e.ScenarioActive(1, C(Feraligatr));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, feraligatr.DamageCounters, "Genome Hacking used Giant Wave");

            feraligatr.DamageCounters = 0;
            e.ScenarioSetTurn(0, 5);
            Step(e);
            Assert.AreEqual(0, feraligatr.DamageCounters, "during its next turn Mew ex can't use Giant Wave, copied or not");
        }

        [Test]
        public void Catastrophe_Sends_An_Attacker_Knocked_Out_With_Gengar_To_The_Lost_Zone()
        {
            // Compendium (Gengar's Catastrophe, 2010-11-04): both are Knocked Out at the same time, and Catastrophe is still
            // on when Knock Outs are checked. Gengar's owner is seat 0, so Gengar is removed from play first.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Reckless Charge"));
            var gengar = e.ScenarioActive(0, C(GengarCC));
            gengar.DamageCounters = 12;               // 130 HP: Reckless Charge's 20 Knocks it Out
            e.ScenarioBench(0, C(Dreepy));
            var pancham = e.ScenarioActive(1, C(Pancham));
            e.ScenarioAttach(pancham, C(Psychic));
            pancham.DamageCounters = 6;               // its 10 damage to itself Knocks it Out
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(1, 4);
            Step(e);

            Assert.IsNull(e.State.FindPokemon(gengar.Uid));
            Assert.IsNull(e.State.FindPokemon(pancham.Uid));
            Assert.IsTrue(e.State.Players[1].LostZone.Any(c => c.Def.Name == "Pancham"), "Pancham went to the Lost Zone");
            Assert.IsFalse(e.State.Players[1].Discard.Any(c => c.Def.Name == "Pancham"));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Gengar"), "Gengar's own Knock Out isn't affected");
        }

        // ================================================================== Trainers

        [Test]
        public void Megaton_Blower_Cannot_Be_Played_For_No_Effect()
        {
            // Compendium (Surging Sparks FAQ, 2024-11-07): not with nothing to discard; a Stadium alone is enough.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C(MegatonBlower));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.PlayTrainer && o.Card.Name == "Megaton Blower"));

            e.ScenarioSetStadium(0, C(Castle));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Action == ActionType.PlayTrainer && o.Card.Name == "Megaton Blower"));
        }

        [Test]
        public void Area_Zero_Bench_Is_Cut_To_Five_Before_The_New_Active_Is_Chosen()
        {
            // Compendium (Stellar Crown FAQ, 2024-09-12): the last Tera Pokémon is Knocked Out, the Bench goes down to 5, and
            // only then is a new Active Pokémon chosen from it (so 4 are left on the Bench, not 5).
            var e = Scenario(new ScriptedAgent().Then("Attack: Reckless Charge"), new ScriptedAgent());
            var pancham = e.ScenarioActive(0, C(Pancham));
            e.ScenarioAttach(pancham, C(Psychic));
            e.ScenarioSetStadium(0, C(AreaZero));
            var miraidon = e.ScenarioActive(1, C(Miraidon));
            miraidon.DamageCounters = 21;             // 220 HP: Reckless Charge Knocks it Out
            for (int i = 0; i < 6; i++) e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);

            Assert.IsNull(e.State.FindPokemon(miraidon.Uid));
            Assert.IsNotNull(e.State.Players[1].Active);
            Assert.AreEqual(4, e.State.Players[1].Bench.Count, "6 Benched: discard down to 5, then promote 1");
        }

        [Test]
        public void Rare_Candy_Works_On_An_Antique_Fossil_In_Play()
        {
            // Compendium (Rare Candy and Fossils, 2003-10-16): a Fossil in play is a Basic Pokémon, so Rare Candy can skip
            // its Stage 1. Antique Skull Fossil -> (Cranidos) -> Rampardos ex.
            var e = Scenario(new ScriptedAgent().Then("Play Rare Candy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var fossil = e.ScenarioBench(0, C(SkullFossil));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C(RareCandy));
            e.ScenarioAddToHand(0, C(RampardosEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Rampardos ex", fossil.Name);
        }
    }
}
