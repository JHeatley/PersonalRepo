using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v56: Celebrations (CEL 1-25 and the Classic Collection CC1-CC25): Δ Evolution, Pokémon ★, Imprison markers (your
    /// ruling: they come off only when the Pokémon evolves, devolves or leaves play), Here Comes Team Rocket! (your ruling: a
    /// Supporter, Expanded) and the other cards with new effects.
    /// </summary>
    public class CelTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Tauros = "MEW-128";
        const string Baltoy = "CRZ-70";
        const string RayquazaEx = "30C-CC19";
        const string SkatersPark = "FST-242";

        const string Palkia = "CEL-4";
        const string FlyingPikachuV = "CEL-6";
        const string FlyingPikachuVmax = "CEL-7";
        const string SurfingPikachuVmax = "CEL-9";
        const string Groudon = "CEL-17";
        const string ZamazentaV = "CEL-18";
        const string Solgaleo = "CEL-21";
        const string Lugia = "CEL-22";
        const string Blastoise = "CEL-CC1";
        const string ImposterOak = "CEL-CC4";
        const string DarkGyarados = "CEL-CC5";
        const string TeamRocket = "CEL-CC6";
        const string ShiningMagikarp = "CEL-CC10";
        const string MagmaGroudon = "CEL-CC11";
        const string RocketsAdmin = "CEL-CC12";
        const string MewEx = "CEL-CC13";
        const string GardevoirEx = "CEL-CC14";
        const string UmbreonStar = "CEL-CC15";
        const string Claydol = "CEL-CC16";
        const string Donphan = "CEL-CC19";
        const string MRayquazaEx = "CEL-CC24";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        /// <summary>A seed whose first coin flips come up as given (true = heads).</summary>
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

        private List<CardDef> DeckWith(params string[] keys)
        {
            var deck = keys.Select(k => C(k)).ToList();
            for (int i = 0; i < 4; i++) deck.Add(C(Tauros));
            while (deck.Count < 60) deck.Add(C(Psychic));
            return deck;
        }

        // ================================================================== Data

        [Test]
        public void Every_Celebrations_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 25; n++) Assert.IsNotNull(db.Get("CEL-" + n), "CEL " + n + " is missing");
            for (int n = 1; n <= 25; n++) Assert.IsNotNull(db.Get("CEL-CC" + n), "CEL CC" + n + " is missing");
            Assert.AreEqual("CRZ-GG10", db.MainPrinting(C("CEL-11")).Key, "Mew was already in the pool");
            Assert.AreEqual("CRZ-GG10", db.MainPrinting(C("CEL-25")).Key);
            Assert.AreEqual("CRZ-GG48", db.MainPrinting(C("CEL-16")).Key, "and Zacian V");
            Assert.AreEqual("PRE-122", db.MainPrinting(C("CEL-23")).Key, "and Professor's Research");
            Assert.AreEqual("30C-CC2", db.MainPrinting(C("CEL-CC2")).Key, "and the Base Set Charizard");
            Assert.AreEqual("WP-24", db.MainPrinting(C("CEL-CC8")).Key, "and _____'s Pikachu");
            Assert.IsFalse(Formats.IsStandardCard(C(Palkia)), "regulation mark E: Expanded");
            Assert.IsTrue(C(Blastoise).UnlimitedOnly, "Base Set 2: Unlimited only");
            Assert.IsTrue(C(RocketsAdmin).UnlimitedOnly);
            Assert.IsFalse(C(TeamRocket).UnlimitedOnly, "your ruling: Expanded, like its Evolutions reprint");
            Assert.IsTrue(C(TeamRocket).IsSupporter, "your ruling: a Supporter");
            Assert.IsTrue(C(ImposterOak).IsItem, "old Trainers are Items");
            Assert.IsFalse(C(MRayquazaEx).UnlimitedOnly, "Roaring Skies: Expanded");
            Assert.IsTrue(C(MRayquazaEx).IsOldMega);
            Assert.AreEqual(EnergyType.Fairy, C("CEL-CC23").PokemonType);
            Assert.AreEqual(EnergyType.Darkness, C(MagmaGroudon).SecondType);
            Assert.IsTrue(C(MewEx).IsOldEx && C(GardevoirEx).IsOldEx);
            Assert.IsFalse(C(GardevoirEx).IsEx, "an old Pokémon-ex isn't today's Pokémon ex");
            Assert.AreEqual(EnergyType.Fire, C(GardevoirEx).PokemonType, "Delta Species");
            Assert.IsTrue(C("CEL-CC9").IsBaby);
            Assert.IsTrue(C("CEL-CC17").IsLevelUp && C("CEL-CC18").IsLevelUp);
            Assert.IsTrue(C("CEL-CC17").IsPokemonSP && C("CEL-CC18").IsPokemonSP && C("30C-CC14").IsPokemonSP, "Luxray GL, Garchomp C, Crobat G");
            Assert.IsFalse(C(Tauros).IsPokemonSP);
            Assert.AreEqual(20, C(Claydol).WeaknessAmount, "Grass +20");
            Assert.IsTrue(C(UmbreonStar).IsPokemonStar);
        }

        // ================================================================== Deck rules

        [Test]
        public void Only_1_Pokemon_Star_And_1_Shining_Magikarp_In_A_Deck()
        {
            CollectionAssert.IsEmpty(Decklist.Validate(DeckWith(UmbreonStar, ShiningMagikarp), DeckFormat.Unlimited));
            var stars = Decklist.Validate(DeckWith(UmbreonStar, UmbreonStar), DeckFormat.Unlimited);
            Assert.IsTrue(stars.Any(p => p.Contains("Pokémon ★")), string.Join("; ", stars.ToArray()));
            var shining = Decklist.Validate(DeckWith(ShiningMagikarp, ShiningMagikarp), DeckFormat.Unlimited);
            Assert.IsTrue(shining.Any(p => p.Contains("Shining Magikarp")), string.Join("; ", shining.ToArray()));
            var expanded = Decklist.Validate(DeckWith(UmbreonStar), DeckFormat.Expanded);
            Assert.IsTrue(expanded.Any(p => p.Contains("Unlimited")), "POP Series 5: Unlimited only");
            CollectionAssert.IsEmpty(Decklist.Validate(DeckWith(TeamRocket, MRayquazaEx), DeckFormat.Expanded));
        }

        // ================================================================== Δ Evolution

        [Test]
        public void Delta_Evolution_Evolves_On_The_First_Turn_And_The_Turn_The_Pokemon_Was_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Rayquaza-EX"), new ScriptedAgent());
            e.ScenarioActive(0, C(RayquazaEx));
            e.ScenarioBench(0, C(Baltoy));
            e.ScenarioAddToHand(0, C(MRayquazaEx));
            e.ScenarioAddToHand(0, C(Claydol));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 1);
            var evolve = e.GetMainActions(0).Where(o => o.Action == ActionType.Evolve).Select(o => o.Label).ToList();
            Assert.AreEqual(1, evolve.Count, "Claydol can't evolve Baltoy on the first turn: " + string.Join(", ", evolve.ToArray()));
            StringAssert.Contains("M Rayquaza-EX", evolve[0], "your first turn");

            e.ScenarioSetTurn(0, 3);
            e.ScenarioAddToHand(0, C(RayquazaEx));
            e.ScenarioAddToHand(0, C(MRayquazaEx));
            Step(e);
            var bench = e.State.Players[0].Bench.Single(m => m.Name == "Rayquaza-EX");
            Assert.AreEqual(3, bench.TurnEnteredPlay);
            Assert.IsTrue(e.CanEvolveInto(bench, C(MRayquazaEx), true), "the turn Rayquaza-EX was played");
            Assert.IsFalse(e.CanEvolveInto(bench, C(MRayquazaEx), false), "only when played from the hand");
        }

        [Test]
        public void Becoming_M_Rayquaza_EX_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var rayquaza = e.ScenarioActive(0, C(RayquazaEx));
            e.ScenarioAddToHand(0, C(MRayquazaEx));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "the Mega Evolution rule ends the turn");
            Assert.AreEqual("M Rayquaza-EX", rayquaza.Name);
        }

        // ================================================================== Imprison (your ruling)

        [Test]
        public void Imprison_Stops_Poke_Powers_Until_The_Pokemon_Evolves_Devolves_Or_Leaves_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Imprison", "Claydol"), new ScriptedAgent());
            e.ScenarioActive(0, C(GardevoirEx));
            e.ScenarioActive(1, C(Tauros));
            var claydol = e.ScenarioBench(1, C(Claydol));
            var solgaleo = e.ScenarioBench(1, C(Solgaleo));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.AbilityBlocked(claydol, "Cosmic Power"));
            Step(e);
            Assert.IsTrue(claydol.Markers.ContainsKey(PokemonInPlay.ImprisonMarker));
            Assert.IsTrue(e.AbilityBlocked(claydol, "Cosmic Power"), "a Poké-Power");
            solgaleo.Markers[PokemonInPlay.ImprisonMarker] = 1;
            Assert.IsFalse(e.AbilityBlocked(solgaleo, "Rush In"), "Abilities aren't Poké-Powers or Poké-Bodies");

            e.ScenarioActive(0, C(Tauros));
            Assert.IsTrue(e.AbilityBlocked(claydol, "Cosmic Power"), "your ruling: the marker stays after Gardevoir ex leaves play");
            e.ScenarioSetTurn(1, 4);
            e.ScenarioAddToHand(1, C(Psychic));
            Assert.IsFalse(Offered(e, 1, "Use Cosmic Power"));

            var baltoy = e.ScenarioBench(1, C(Baltoy));
            baltoy.Markers[PokemonInPlay.ImprisonMarker] = 1;
            e.EvolvePokemon(baltoy, e.NewCard(1, C(Claydol)));
            Assert.IsFalse(baltoy.Markers.ContainsKey(PokemonInPlay.ImprisonMarker), "your ruling: evolving takes the marker off");
            Assert.IsFalse(e.AbilityBlocked(baltoy, "Cosmic Power"));
        }

        [Test]
        public void Imprison_Needs_Gardevoir_Ex_Active_Without_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gardevoir = e.ScenarioBench(0, C(GardevoirEx));
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Imprison"), "only from the Active Spot");
            e.ScenarioActive(0, C(GardevoirEx));
            Assert.IsTrue(Offered(e, 0, "Use Imprison"));
            e.ApplyCondition(e.State.Players[0].Active, SpecialCondition.Confused);
            Assert.IsFalse(Offered(e, 0, "Use Imprison"), "not with a Special Condition");
            Assert.IsNotNull(gardevoir);
        }

        // ================================================================== Trainers

        [Test]
        public void Here_Comes_Team_Rocket_Turns_Every_Prize_Card_Face_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Here Comes Team Rocket!"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(TeamRocket));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            for (int p = 0; p < 2; p++)
            {
                var player = e.State.Players[p];
                Assert.AreEqual(6, player.FaceUpPrizeIds.Count);
                Assert.IsTrue(player.Prizes.All(c => player.FaceUpPrizeIds.Contains(c.Id)));
            }
            Assert.IsTrue(e.State.Players[0].SupporterPlayedThisTurn, "your ruling: a Supporter");
        }

        [Test]
        public void Rockets_Admin_Draws_Up_To_Each_Players_Prize_Count()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rocket's Admin."), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(RocketsAdmin));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 8; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.State.Players[1].Prizes.RemoveRange(0, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count, "6 Prize cards left");
            Assert.AreEqual(4, e.State.Players[1].Hand.Count, "4 Prize cards left");
        }

        [Test]
        public void Imposter_Professor_Oak_Gives_The_Opponent_A_New_Hand_Of_7()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Imposter Professor Oak"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(ImposterOak));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[1].Hand.Count);
            Assert.AreEqual(14, e.State.Players[1].Deck.Count);
        }

        // ================================================================== Pokémon

        [Test]
        public void Rain_Dance_Attaches_Water_Energy_To_Water_Pokemon_Without_Using_The_Attachment()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Rain Dance", "Use Rain Dance"), new ScriptedAgent());
            var blastoise = e.ScenarioActive(0, C(Blastoise));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, blastoise.Energy.Count);
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn);
            e.ScenarioAddToHand(0, C(Water));
            e.ApplyCondition(blastoise, SpecialCondition.Asleep);
            Assert.IsFalse(Offered(e, 0, "Use Rain Dance"), "not while Asleep");
        }

        [Test]
        public void Hydro_Pump_Counts_Up_To_2_Extra_Water_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hydro Pump"), new ScriptedAgent());
            var blastoise = e.ScenarioActive(0, C(Blastoise));
            Attach(e, blastoise, Water, 6);
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, foe.DamageCounters, "40 + 20");
        }

        [Test]
        public void Final_Beam_Hits_The_Pokemon_That_Knocked_Out_Dark_Gyarados()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Massive Rend"), new ScriptedAgent());
            var groudon = e.ScenarioActive(0, C(Groudon));
            Attach(e, groudon, Fighting, 4);
            var gyarados = e.ScenarioActive(1, C(DarkGyarados));
            Attach(e, gyarados, Water, 2);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(4, groudon.DamageCounters, "heads: 20 for each of the 2 Water Energy");
        }

        [Test]
        public void Versatile_Uses_The_Attacks_Of_Every_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(MewEx));
            Attach(e, mew, Psychic, 4);
            e.ScenarioActive(1, C(Lugia));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Deep Crush"), "the opponent's Lugia's attack");
            mew.Markers[PokemonInPlay.ImprisonMarker] = 1;
            Assert.IsFalse(Offered(e, 0, "Attack: Deep Crush"), "an Imprison marker stops the Poké-Body");
            Assert.IsTrue(Offered(e, 0, "Attack: Power Move"));
        }

        [Test]
        public void Power_Saver_Needs_4_Team_Magma_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var groudon = e.ScenarioActive(0, C(MagmaGroudon));
            Attach(e, groudon, Fighting, 3);
            e.ScenarioBench(0, C(MagmaGroudon));
            e.ScenarioBench(0, C(MagmaGroudon));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Pulverize"), "3 Team Magma Pokémon");
            e.ScenarioBench(1, C(MagmaGroudon));
            Assert.IsTrue(Offered(e, 0, "Attack: Pulverize"), "4, the opponent's included");
        }

        [Test]
        public void Exoskeleton_Takes_20_Less_After_Weakness_And_Resistance()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var donphan = e.ScenarioActive(1, C(Donphan));
            Assert.AreEqual(40, e.DealAttackDamage(tauros, donphan, 60));
        }

        [Test]
        public void Growl_Of_The_Shield_Protects_Fighting_Pokemon_From_Pokemon_VMAX_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var vmax = e.ScenarioActive(0, C(FlyingPikachuVmax));
            var v = e.ScenarioBench(0, C(FlyingPikachuV));
            var groudon = e.ScenarioActive(1, C(Groudon));
            e.ScenarioBench(1, C(ZamazentaV));
            var tauros = e.ScenarioBench(1, C(Tauros));
            Assert.AreEqual(80, e.DealAttackDamage(vmax, groudon, 100));
            e.ScenarioBench(1, C(ZamazentaV));
            Assert.AreEqual(80, e.DealAttackDamage(vmax, groudon, 100), "you can't apply more than 1 Growl of the Shield");
            Assert.AreEqual(100, e.DealAttackDamage(v, groudon, 100), "only Pokémon VMAX");
            Assert.AreEqual(30, e.DealAttackDamage(vmax, tauros, 30), "only Fighting Pokémon");
        }

        [Test]
        public void Absolute_Space_Stops_The_Opponents_Stadiums_While_Palkia_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Palkia));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(SkatersPark));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Play Skaters' Park"));
            e.ScenarioActive(0, C(Tauros));
            Assert.IsTrue(Offered(e, 1, "Play Skaters' Park"));
        }

        [Test]
        public void Overdrive_Smash_Does_80_More_During_Your_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Overdrive Smash", "Attack: Overdrive Smash"), new ScriptedAgent());
            var palkia = e.ScenarioActive(0, C(Palkia));
            Attach(e, palkia, Water, 3);
            var foe = e.ScenarioActive(1, C(SurfingPikachuVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, foe.DamageCounters);
            e.ScenarioSetTurn(0, 5);
            Step(e);
            Assert.AreEqual(24, foe.DamageCounters, "80 + 160");
        }
    }
}
