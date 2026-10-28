using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v126: Gym Challenge (G2 1-132): Lt. Surge's Secret Plan's face-down cards, Brock's Ninetales's Shapeshift, Chaos Gym,
    /// Sabrina's Psychic Control, Blaine, Brock's Protection, Giovanni, Koga, Koga's Ninja Trick, the other Gyms, and the owners'
    /// Pokémon Powers (Fortitude, Rebellion, Relaxing Scent, Scram, Psylink).
    /// </summary>
    public class G2Tests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Spearow = "G1-52";
        const string Lickitung = "G1-41";
        const string Rattata = "G1-82";
        const string Mankey = "G1-67";
        const string Bill = "BS-91";
        const string EnergyRemoval = "BS-92";

        const string Charizard = "G2-2";
        const string Ninetales = "G2-3";
        const string Machamp = "G2-6";
        const string Gyarados = "G2-13";
        const string Alakazam = "G2-16";
        const string Charmeleon = "G2-31";
        const string Primeape = "G2-35";
        const string BrocksVulpix = "G2-37";
        const string Ivysaur = "G2-41";
        const string Machoke = "G2-42";
        const string Kadabra = "G2-58";
        const string Charmander = "G2-60";
        const string Machop = "G2-72";
        const string Ekans = "G2-77";
        const string Grimer = "G2-78";
        const string Zubat = "G2-83";
        const string MistysMagikarp = "G2-88";
        const string Seel = "G2-91";
        const string Abra = "G2-94";
        const string Blaine = "G2-100";
        const string BrocksProtection = "G2-101";
        const string ChaosGym = "G2-102";
        const string Giovanni = "G2-104";
        const string Koga = "G2-106";
        const string SecretPlan = "G2-107";
        const string MistysWish = "G2-108";
        const string ResistanceGym = "G2-109";
        const string CinnabarCityGym = "G2-113";
        const string FuchsiaCityGym = "G2-114";
        const string NinjaTrick = "G2-115";
        const string MinefieldGym = "G2-119";
        const string PsychicControl = "G2-121";
        const string ViridianCityGym = "G2-123";
        const string TransparentWalls = "G2-125";
        const string WarpPoint = "G2-126";

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

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(119, db.Cards.Count(d => d.SetCode == "G2"));
            Assert.AreEqual("G2-100", C("G2-17").BaseKey, "the holo Blaine");
            Assert.AreEqual("G2-104", C("G2-18").BaseKey, "Giovanni");
            Assert.AreEqual("G2-106", C("G2-19").BaseKey, "Koga");
            Assert.AreEqual("G2-110", C("G2-20").BaseKey, "Sabrina");
            Assert.AreEqual("CEL-CC7", C("G2-15").BaseKey, "Rocket's Zapdos");
            Assert.AreEqual("30C-CC4", C("G2-69").BaseKey, "Erika's Jigglypuff");
            Assert.AreEqual("WP-19", C("G2-93").BaseKey, "Sabrina's Abra");
            Assert.AreEqual("MEE-14", C("G2-127").BaseKey, "basic Fighting Energy");
            Assert.AreEqual("G2-116", C("G2-116").BaseKey, "Master Ball: not the Temporal Forces card");
            Assert.AreEqual("Brock's Vulpix", C(Ninetales).EvolvesFrom);
            Assert.AreEqual(AbilityDef.PokemonPower, C(Ninetales).Abilities[0].Kind);
            Assert.AreEqual(EnergyType.Water, C("G2-30").PokemonType, "Sabrina's Golduck is a Water Pokémon");
            Assert.IsTrue(C(SecretPlan).UnlimitedOnly);
            Assert.AreEqual(TrainerKind.Stadium, C(ChaosGym).TrainerKind);
        }

        // ------------------------------------------------------------------ Lt. Surge's Secret Plan

        [Test]
        public void Secret_Plan_Puts_A_Card_Face_Down_And_Flipping_A_Trainer_Discards_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lt. Surge's Secret Plan", "Bill", "Flip over"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(SecretPlan));
            e.ScenarioAddToHand(0, C(Bill));
            e.ScenarioAddToHand(0, C(Charmander));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(1, me.Bench.Count);
            var faceDown = me.Bench[0];
            Assert.IsTrue(GameEngine.IsFaceDown(faceDown));
            Assert.AreEqual("Bill", e.FaceDownCardOf(faceDown).Def.Name);
            Assert.IsTrue(faceDown.Def.IsBasicPokemon, "a Basic Pokémon while it's face down");
            var theirView = PlayerView.Create(e, 1);
            Assert.IsTrue(theirView.Opponent.Bench[0].Top.Def.IsFaceDown, "the opponent doesn't see it");
            var myView = PlayerView.Create(e, 0);
            Assert.IsTrue(myView.Me.Bench[0].Notes.Contains("Face down: Bill"), "its owner knows what it is");
            Assert.IsNull(Invariants.Check(e, 0));

            Step(e);
            Assert.AreEqual(0, me.Bench.Count, "not a Basic Pokémon: discarded");
            Assert.IsTrue(me.Discard.Any(c => c.Def.Key == Bill));
            Assert.AreEqual(0, e.State.FaceDownCards.Count);
            Assert.IsNull(Invariants.Check(e, 0));
        }

        [Test]
        public void A_Face_Down_Basic_Pokemon_Is_Flipped_When_It_Takes_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lt. Surge's Secret Plan", "Blaine's Charmander"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(SecretPlan));
            e.ScenarioAddToHand(0, C(Bill));
            e.ScenarioAddToHand(0, C(Charmander));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var mon = e.State.Players[0].Bench[0];
            Assert.IsTrue(GameEngine.IsFaceDown(mon));
            e.PlaceDamageCounters(mon, 1, "test");
            Assert.IsFalse(GameEngine.IsFaceDown(mon));
            Assert.AreEqual("Blaine's Charmander", mon.Name);
            Assert.AreEqual(1, mon.DamageCounters);
            Assert.IsNull(Invariants.Check(e, 0));
        }

        [Test]
        public void An_Attack_On_A_Face_Down_Trainer_Discards_It_And_Hits_The_New_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            e.ScenarioActive(1, C(Rattata));
            var bill = e.ScenarioAddToHand(1, C(Bill));
            e.State.Players[1].Hand.Remove(bill);
            var faceDown = e.PutFaceDownOntoBench(1, bill);
            var lickitung = e.ScenarioBench(1, C(Lickitung));
            e.SwitchActive(1, faceDown);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.IsTrue(opp.Discard.Any(c => c.Def.Key == Bill), "flipped over when attacked: not a Basic Pokémon");
            Assert.AreNotEqual(faceDown, opp.Active);
            Assert.AreEqual(2, opp.Active.DamageCounters, "the Pokémon promoted in its place takes the attack");
            Assert.IsTrue(opp.Active == lickitung || opp.Active.Def.Key == Rattata);
        }

        // ------------------------------------------------------------------ Shapeshift

        [Test]
        public void Shapeshift_Makes_Ninetales_The_Attached_Pokemon_Until_It_Is_Asleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shapeshift", "Attach Blaine's Charizard"), new ScriptedAgent());
            var ninetales = e.ScenarioActive(0, C(BrocksVulpix));
            e.ScenarioEvolve(ninetales, C(Ninetales));
            Attach(e, ninetales, Fire, 2);
            e.ScenarioAddToHand(0, C(Charizard));
            e.ScenarioAddToHand(0, C(Charmeleon));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(70, e.MaxHp(ninetales));
            Step(e);
            Assert.AreEqual(1, ninetales.Attached.Count);
            Assert.AreEqual("Blaine's Charizard", ninetales.Name, "treated as that Pokémon (your ruling)");
            Assert.AreEqual(100, e.MaxHp(ninetales), "Blaine's Charizard's HP");
            Assert.AreEqual(EnergyType.Fighting, e.ResistanceOf(ninetales));
            Assert.AreEqual(3, e.RetreatCostOf(ninetales));
            var actions = e.GetMainActions(0);
            Assert.IsTrue(actions.Any(o => o.Label.StartsWith("Attack: Roaring Flames")));
            Assert.IsFalse(actions.Any(o => o.Label.StartsWith("Attack: Will-o'-the-wisp")), "its own attack is gone");
            Assert.IsNull(Invariants.Check(e, 0));

            ninetales.Conditions = SpecialCondition.Asleep;
            e.StateChecks();
            Assert.AreEqual(0, ninetales.Attached.Count, "Asleep: the Evolution card is discarded");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Key == Charizard));
            Assert.AreEqual(70, e.MaxHp(ninetales));
        }

        [Test]
        public void Shapeshift_Gives_Ninetales_The_Pokemon_Powers_Of_The_Attached_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shapeshift", "Attach Lt. Surge's Magneton", "Use Energy Charge"), new ScriptedAgent());
            var ninetales = e.ScenarioActive(0, C(BrocksVulpix));
            e.ScenarioEvolve(ninetales, C(Ninetales));
            var pikachu = e.ScenarioBench(0, C("G1-81"));
            Attach(e, pikachu, Lightning, 1);
            e.ScenarioAddToHand(0, C("G1-8"));
            e.ScenarioAddToHand(0, C(Charmeleon));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Lt. Surge's Magneton", ninetales.Name);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Energy Charge")), "your ruling: it can use that Pokémon's Power");
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Shapeshift")), "and it still has Shapeshift (to discard the card)");
            Step(e);
            Assert.AreEqual(1, ninetales.Energy.Count, "Energy Charge moved the Lightning Energy to it");
            Assert.AreEqual(0, pikachu.Energy.Count);
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Chaos_Gym_Tails_Lets_The_Opponent_Use_The_Trainer()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Bill"), new ScriptedAgent().Then("Yes"));
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Bill));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetStadium(1, C(ChaosGym));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "it didn't work for the player");
            Assert.AreEqual(2, e.State.Players[1].Hand.Count, "the opponent drew 2 with it");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Key == Bill), "to its owner's discard pile");
        }

        [Test]
        public void Sabrinas_Psychic_Control_Uses_A_Trainer_From_The_Opponents_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Sabrina's Psychic Control"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(PsychicControl));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioAddToDiscard(1, C(Bill), 1);
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Bill's 2 cards");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Key == Bill), "it stays in the opponent's discard pile");
        }

        [Test]
        public void Blaine_Lets_A_Second_Fire_Energy_Follow_Later_In_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Blaine", "Attach Fire Energy to Blaine's Charmander", "Attach Fire Energy to Blaine's Charmander"), new ScriptedAgent());
            var charmander = e.ScenarioActive(0, C(Charmander));
            e.ScenarioAddToHand(0, C(Blaine));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, charmander.Energy.Count);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.AttachEnergy));
        }

        [Test]
        public void Brocks_Protection_Stops_Energy_Removal()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Brock's Protection"), new ScriptedAgent().Then("Play Energy Removal"));
            var vulpix = e.ScenarioActive(0, C(BrocksVulpix));
            Attach(e, vulpix, Fire, 1);
            e.ScenarioAddToHand(0, C(BrocksProtection));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioAddToHand(1, C(EnergyRemoval));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, vulpix.Energy.Count);
        }

        [Test]
        public void Giovanni_Lets_A_Giovannis_Pokemon_Evolve_The_Turn_It_Was_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Giovanni's Machop", "Play Giovanni"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Machop));
            e.ScenarioAddToHand(0, C(Machoke));
            e.ScenarioAddToHand(0, C(Giovanni));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve), "just played");
            Step(e);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve && o.Label.Contains("Giovanni's Machoke")));
        }

        [Test]
        public void Koga_Poisons_The_Defending_Pokemon_A_Kogas_Pokemon_Damages()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Koga", "Attack: Fast-Acting Poison"), new ScriptedAgent());
            var ekans = e.ScenarioActive(0, C(Ekans));
            Attach(e, ekans, Grass, 1);
            e.ScenarioAddToHand(0, C(Koga));
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false, false));
            Step(e);
            Step(e);
            Assert.AreEqual(1, lickitung.DamageCounters);
            Assert.IsTrue(lickitung.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(lickitung.Has(SpecialCondition.Confused), "Fast-Acting Poison itself missed");
        }

        [Test]
        public void Kogas_Ninja_Trick_Switches_Before_The_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Koga's Ninja Trick", "Yes"), new ScriptedAgent().Then("Attack: Drill Peck"));
            var grimer = e.ScenarioActive(0, C(Grimer));
            var zubat = e.ScenarioBench(0, C(Zubat));
            e.ScenarioAddToHand(0, C(NinjaTrick));
            var spearow = e.ScenarioActive(1, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, grimer.DamageCounters);
            Assert.AreEqual(zubat, e.State.Players[0].Active);
            Assert.AreEqual(2, zubat.DamageCounters, "the Pokémon switched in takes the attack");
            Assert.IsFalse(e.State.Effects.Any(x => x.Name.StartsWith("Koga's Ninja Trick")), "discarded once it moved to the Bench");
        }

        [Test]
        public void Misty_s_Wish_Refused_Draws_A_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Misty's Wish"), new ScriptedAgent().Then("No"));
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(MistysWish));
            e.ScenarioAddToHand(0, C(Bill));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Bill and the card drawn");
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Transparent_Walls_And_Warp_Point()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Transparent Walls"), new ScriptedAgent().Then("Attack: Fire Ring"));
            var lickitung = e.ScenarioActive(0, C(Lickitung));
            var rattata = e.ScenarioBench(0, C(Rattata));
            e.ScenarioAddToHand(0, C(TransparentWalls));
            var vulpix = e.ScenarioActive(1, C(BrocksVulpix));
            Attach(e, vulpix, Fire, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, lickitung.DamageCounters);
            Assert.AreEqual(0, rattata.DamageCounters, "no damage to the Bench");

            var e2 = Scenario(new ScriptedAgent().Then("Play Warp Point"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Rattata));
            var mine = e2.ScenarioBench(0, C(Lickitung));
            e2.ScenarioAddToHand(0, C(WarpPoint));
            e2.ScenarioActive(1, C(Rattata));
            var theirs = e2.ScenarioBench(1, C(Lickitung));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(mine, e2.State.Players[0].Active);
            Assert.AreEqual(theirs, e2.State.Players[1].Active);
        }

        // ------------------------------------------------------------------ Gyms

        [Test]
        public void Resistance_Gym_And_Cinnabar_City_Gym()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Frostbite"), new ScriptedAgent());
            var seel = e.ScenarioActive(0, C(Seel));
            Attach(e, seel, Water, 1);
            var charmander = e.ScenarioActive(1, C(Charmander));
            e.ScenarioSetStadium(0, C(CinnabarCityGym));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, charmander.DamageCounters, "no Weakness: 10, not 20");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var charizard = e2.ScenarioActive(0, C(Charizard));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            Assert.AreEqual(30, e2.ResistanceAmountOf(charizard));
            e2.ScenarioSetStadium(0, C(ResistanceGym));
            Assert.AreEqual(10, e2.ResistanceAmountOf(charizard));
        }

        [Test]
        public void Rockets_Minefield_Gym_Tails_Puts_2_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Blaine's Charmander"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Charmander));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetStadium(1, C(MinefieldGym));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench[0].DamageCounters);
        }

        [Test]
        public void Viridian_City_Gym_Heals_A_Giovannis_Pokemon_When_It_Evolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var machop = e.ScenarioActive(0, C(Machop));
            machop.DamageCounters = 3;
            e.ScenarioAddToHand(0, C(Machoke));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetStadium(1, C(ViridianCityGym));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Giovanni's Machoke", machop.Name);
            Assert.AreEqual(1, machop.DamageCounters);
        }

        [Test]
        public void Fuchsia_City_Gym_Heads_Shuffles_A_Kogas_Pokemon_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Fuchsia City Gym", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioBench(0, C(Zubat));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetStadium(1, C(FuchsiaCityGym));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(deck + 1, e.State.Players[0].Deck.Count);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Use Fuchsia City Gym"), "once a turn");
        }

        // ------------------------------------------------------------------ Pokémon Powers

        [Test]
        public void Fortitude_Heads_Leaves_Machamp_With_10_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var machamp = e.ScenarioActive(1, C(Machop));
            e.ScenarioEvolve(machamp, C(Machoke));
            e.ScenarioEvolve(machamp, C(Machamp));
            machamp.DamageCounters = 9;
            e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(machamp, e.State.Players[1].Active);
            Assert.AreEqual(10, e.RemainingHp(machamp));
        }

        [Test]
        public void Rebellion_Two_Tails_Shuffles_Gyarados_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tidal Wave"), new ScriptedAgent());
            var gyarados = e.ScenarioActive(0, C(MistysMagikarp));
            e.ScenarioEvolve(gyarados, C(Gyarados));
            Attach(e, gyarados, Water, 4);
            e.ScenarioBench(0, C(Rattata));
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false, false));
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(0, lickitung.DamageCounters, "the attack did nothing");
            Assert.AreEqual(deck + 6, e.State.Players[0].Deck.Count, "Gyarados, Magikarp and 4 Energy");
            Assert.AreEqual(Rattata, e.State.Players[0].Active.Def.Key);
        }

        [Test]
        public void Relaxing_Scent_Halves_Attack_Damage_Rounded_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var ivysaur = e.ScenarioActive(1, C("G2-39"));
            e.ScenarioEvolve(ivysaur, C(Ivysaur));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, ivysaur.DamageCounters, "20 halved");
        }

        [Test]
        public void Scram_Shuffles_Primeape_Away_At_Exactly_10_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drill Peck"), new ScriptedAgent());
            var spearow = e.ScenarioActive(0, C(Spearow));
            Attach(e, spearow, Lightning, 1);
            var primeape = e.ScenarioActive(1, C(Mankey));
            e.ScenarioEvolve(primeape, C(Primeape));
            primeape.DamageCounters = 4;
            e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(primeape.Uid), "60 damage on 70 HP: Scram");
            Assert.AreEqual(Rattata, e.State.Players[1].Active.Def.Key);
        }

        [Test]
        public void Psylink_Gives_Alakazam_The_Attacks_Of_Your_Psychic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var alakazam = e.ScenarioActive(0, C(Abra));
            e.ScenarioEvolve(alakazam, C(Kadabra));
            e.ScenarioEvolve(alakazam, C(Alakazam));
            Attach(e, alakazam, Psychic, 1);
            e.ScenarioBench(0, C(Abra));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Attack: Quick Attack (Sabrina's Abra)"));
            alakazam.Conditions = SpecialCondition.Confused;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Attack: Quick Attack (Sabrina's Abra)"), "not while Confused");
        }
    }
}
