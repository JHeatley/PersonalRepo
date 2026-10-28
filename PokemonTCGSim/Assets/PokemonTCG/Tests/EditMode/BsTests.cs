using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v113: Base Set (BS 1-102): Damage Swap, Strikes Back, Buzzap, Toxic, Conversion 1 / 2, Super Fang, Karate Chop, Leek Slap,
    /// Dream Eater, Mirror Move, Selfdestruct and the old Trainers (PlusPower, Defender, Professor Oak, Energy Removal, Revive,
    /// Devolution Spray, Scoop Up, Lass, Item Finder, Super Potion, Clefairy Doll).
    /// </summary>
    public class BsTests : ScenarioTestBase
    {
        const string Fighting = "MEE-14";
        const string Psychic = "MEE-13";
        const string Grass = "MEE-9";
        const string Lightning = "MEE-12";

        const string Alakazam = "BS-1";
        const string Chansey = "BS-3";
        const string Machamp = "BS-8";
        const string Mewtwo = "BS-10";
        const string Nidoking = "BS-11";
        const string Electrode = "BS-21";
        const string Pidgeotto = "BS-22";
        const string Dratini = "BS-26";
        const string Farfetchd = "BS-27";
        const string Haunter = "BS-29";
        const string Kadabra = "BS-32";
        const string Machoke = "BS-34";
        const string Nidorino = "BS-37";
        const string Porygon = "BS-39";
        const string Raticate = "BS-40";
        const string Abra = "BS-43";
        const string Gastly = "BS-50";
        const string Machop = "BS-52";
        const string Magnemite = "BS-53";
        const string NidoranM = "BS-55";
        const string Pidgey = "BS-57";
        const string Pikachu = "BS-58";
        const string Rattata = "BS-61";
        const string Voltorb = "BS-67";
        const string ClefairyDoll = "BS-70";
        const string ComputerSearch = "BS-71";
        const string DevolutionSpray = "BS-72";
        const string ItemFinder = "BS-74";
        const string Lass = "BS-75";
        const string ScoopUp = "BS-78";
        const string Defender = "BS-80";
        const string PlusPower = "BS-84";
        const string ProfessorOak = "BS-88";
        const string Revive = "BS-89";
        const string SuperPotion = "BS-90";
        const string Bill = "BS-91";
        const string EnergyRemoval = "BS-92";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(83, db.Cards.Count(d => d.SetCode == "BS"));
            Assert.AreEqual("CEL-CC1", C("BS-2").BaseKey, "Blastoise");
            Assert.AreEqual("MEE-11", C("BS-102").BaseKey, "Water Energy");
            Assert.IsFalse(C(ComputerSearch).IsAceSpec, "your ruling (v113): a normal Item");
            Assert.IsTrue(C(ClefairyDoll).IsFossil);
            Assert.AreEqual(10, C(ClefairyDoll).Hp);
            Assert.AreEqual(EnergyType.Psychic, C(Chansey).Resistance);
            Assert.AreEqual(30, C(Chansey).ResistanceAmount);
        }

        [Test]
        public void Damage_Swap_Moves_A_Counter_And_Stops_While_Asleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Damage Swap"), new ScriptedAgent());
            var alakazam = e.ScenarioActive(0, C(Abra));
            e.ScenarioEvolve(alakazam, C(Kadabra));
            e.ScenarioEvolve(alakazam, C(Alakazam));
            alakazam.DamageCounters = 2;
            var abra = e.ScenarioBench(0, C(Abra));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, alakazam.DamageCounters);
            Assert.AreEqual(1, abra.DamageCounters);
            e.ApplyCondition(alakazam, SpecialCondition.Asleep);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Damage Swap")), "a Pokémon Power stops while Asleep");
        }

        [Test]
        public void Strikes_Back_Hurts_The_Attacker_Unless_Machamp_Was_Paralyzed()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var machamp = e.ScenarioActive(0, C(Machop));
            e.ScenarioEvolve(machamp, C(Machoke));
            e.ScenarioEvolve(machamp, C(Machamp));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(rattata, machamp, 20);
            Assert.AreEqual(1, rattata.DamageCounters);
            e.ApplyCondition(machamp, SpecialCondition.Paralyzed);
            e.ScenarioSetTurn(1, 6);
            e.DealAttackDamage(rattata, machamp, 20);
            Assert.AreEqual(1, rattata.DamageCounters, "already Paralyzed");
        }

        [Test]
        public void Buzzap_Turns_Electrode_Into_2_Energy_Of_The_Chosen_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Buzzap", "Water"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            var electrode = e.ScenarioBench(0, C(Voltorb));
            e.ScenarioEvolve(electrode, C(Electrode));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(1, chansey.Energy.Count, "Electrode is attached as 1 card");
            Assert.AreEqual(2, e.EnergyCountOfType(chansey, EnergyType.Water));
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "the opponent took a Prize card");
        }

        [Test]
        public void Toxic_Poisons_For_2_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Toxic"), new ScriptedAgent());
            var nidoking = e.ScenarioActive(0, C(NidoranM));
            e.ScenarioEvolve(nidoking, C(Nidorino));
            e.ScenarioEvolve(nidoking, C(Nidoking));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(nidoking, C(Grass));
            var mewtwo = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, mewtwo.DamageCounters);
            Assert.IsTrue(mewtwo.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(2, mewtwo.PoisonCounterBase);
        }

        [Test]
        public void Conversion_2_Changes_Porygons_Resistance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Conversion 2", "Water"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(Porygon));
            e.ScenarioAttach(porygon, C(Psychic));
            e.ScenarioAttach(porygon, C(Psychic));
            e.ScenarioActive(1, C(Machop));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(EnergyType.Water, e.ResistanceOf(porygon));
        }

        [Test]
        public void Conversion_1_Changes_The_Defending_Pokemons_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Conversion 1", "Yes", "Grass"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(Porygon));
            e.ScenarioAttach(porygon, C(Psychic));
            var machop = e.ScenarioActive(1, C(Machop));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(EnergyType.Grass, e.WeaknessOf(machop));
        }

        [Test]
        public void Super_Fang_Does_Half_The_Remaining_HP_Rounded_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Super Fang"), new ScriptedAgent());
            var raticate = e.ScenarioActive(0, C(Rattata));
            e.ScenarioEvolve(raticate, C(Raticate));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(raticate, C(Psychic));
            var machop = e.ScenarioActive(1, C(Machop));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, machop.DamageCounters, "half of 50 is 25, rounded up to 30");
        }

        [Test]
        public void Karate_Chop_Does_Less_For_Each_Counter_On_Machoke()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Karate Chop"), new ScriptedAgent());
            var machoke = e.ScenarioActive(0, C(Machop));
            e.ScenarioEvolve(machoke, C(Machoke));
            e.ScenarioAttach(machoke, C(Fighting));
            e.ScenarioAttach(machoke, C(Fighting));
            e.ScenarioAttach(machoke, C(Psychic));
            machoke.DamageCounters = 2;
            var mewtwo = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, mewtwo.DamageCounters);
        }

        [Test]
        public void Leek_Slap_Works_Once_While_Farfetchd_Stays_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Leek Slap"), new ScriptedAgent());
            var farfetchd = e.ScenarioActive(0, C(Farfetchd));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(farfetchd, C(Psychic));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.UsableAttacks(farfetchd).Any(a => a.Attack.Name == "Leek Slap"));
            Step(e);
            e.ScenarioSetTurn(0, 5);
            Assert.IsFalse(e.UsableAttacks(farfetchd).Any(a => a.Attack.Name == "Leek Slap"));
            Assert.IsTrue(e.UsableAttacks(farfetchd).Any(a => a.Attack.Name == "Pot Smash"));
        }

        [Test]
        public void Dream_Eater_Needs_An_Asleep_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var haunter = e.ScenarioActive(0, C(Gastly));
            e.ScenarioEvolve(haunter, C(Haunter));
            e.ScenarioAttach(haunter, C(Psychic));
            e.ScenarioAttach(haunter, C(Psychic));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.UsableAttacks(haunter).Any(a => a.Attack.Name == "Dream Eater"));
            e.ApplyCondition(chansey, SpecialCondition.Asleep);
            Assert.IsTrue(e.UsableAttacks(haunter).Any(a => a.Attack.Name == "Dream Eater"));
        }

        [Test]
        public void Mirror_Move_Returns_Last_Turns_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mirror Move"), new ScriptedAgent());
            var pidgeotto = e.ScenarioActive(0, C(Pidgey));
            e.ScenarioEvolve(pidgeotto, C(Pidgeotto));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(pidgeotto, C(Psychic));
            var machop = e.ScenarioActive(1, C(Machop));
            e.ScenarioSetTurn(1, 3);
            e.DealAttackDamage(machop, pidgeotto, 20, false);
            e.ScenarioSetTurn(0, 4);
            Step(e);
            Assert.AreEqual(2, machop.DamageCounters);
        }

        [Test]
        public void Selfdestruct_Hits_Both_Benches_And_Magnemite()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Selfdestruct"), new ScriptedAgent());
            var magnemite = e.ScenarioActive(0, C(Magnemite));
            e.ScenarioAttach(magnemite, C(Lightning));
            e.ScenarioAttach(magnemite, C(Psychic));
            var pikachu = e.ScenarioBench(0, C(Pikachu));
            var chansey = e.ScenarioActive(1, C(Chansey));
            var rattata = e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, chansey.DamageCounters);
            Assert.AreEqual(1, rattata.DamageCounters);
            Assert.AreEqual(1, pikachu.DamageCounters);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Magnemite"), "40 damage to itself");
        }

        [Test]
        public void PlusPower_Adds_10_To_This_Turns_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Play PlusPower", "Attack: Pound"), new ScriptedAgent());
            var dratini = e.ScenarioActive(0, C(Dratini));
            e.ScenarioAttach(dratini, C(Psychic));
            e.ScenarioAddToHand(0, C(PlusPower));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
        }

        [Test]
        public void Defender_Reduces_Attack_Damage_By_20()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Defender"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Defender));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(rattata, chansey, 30);
            Assert.AreEqual(1, chansey.DamageCounters);
        }

        [Test]
        public void Professor_Oak_Discards_The_Hand_And_Draws_7()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Professor Oak"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ProfessorOak));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioAddToHand(0, C(Bill));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
            Assert.AreEqual(3, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Energy_Removal_Discards_An_Opposing_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Energy Removal"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(EnergyRemoval));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioAttach(rattata, C(Fighting));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, rattata.Energy.Count);
        }

        [Test]
        public void Revive_Benches_A_Basic_With_Half_Its_HP_In_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Revive"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            e.ScenarioAddToHand(0, C(Revive));
            e.ScenarioAddToDiscard(0, C(Chansey), 1);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
            Assert.AreEqual(6, e.State.Players[0].Bench[0].DamageCounters);
        }

        [Test]
        public void Devolution_Spray_Discards_The_Evolution_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Devolution Spray"), new ScriptedAgent());
            var kadabra = e.ScenarioActive(0, C(Abra));
            e.ScenarioEvolve(kadabra, C(Kadabra));
            e.ApplyCondition(kadabra, SpecialCondition.Confused);
            e.ScenarioAddToHand(0, C(DevolutionSpray));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Abra", kadabra.Name);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Kadabra"));
            Assert.IsFalse(e.State.Players[0].Hand.Any(c => c.Def.Name == "Kadabra"));
            Assert.IsFalse(kadabra.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Scoop_Up_Returns_Only_The_Basic_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Scoop Up", "Kadabra"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var kadabra = e.ScenarioBench(0, C(Abra));
            e.ScenarioEvolve(kadabra, C(Kadabra));
            e.ScenarioAttach(kadabra, C(Psychic));
            e.ScenarioAddToHand(0, C(ScoopUp));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(0, p.Bench.Count);
            Assert.AreEqual(1, p.Hand.Count);
            Assert.AreEqual("Abra", p.Hand[0].Def.Name);
            Assert.IsTrue(p.Discard.Any(c => c.Def.Name == "Kadabra"));
            Assert.IsTrue(p.Discard.Any(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Lass_Shuffles_Both_Players_Trainers_Into_Their_Decks()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lass"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Lass));
            e.ScenarioAddToHand(0, C(Bill));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioAddToHand(1, C(PlusPower));
            e.ScenarioAddToHand(1, C(Fighting));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.AreEqual(21, e.State.Players[0].Deck.Count);
            Assert.AreEqual(21, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Item_Finder_Takes_Back_A_Trainer()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Item Finder"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ItemFinder));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioAddToHand(0, C(Fighting));
            e.ScenarioAddToDiscard(0, C(Bill), 1);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(1, hand.Count);
            Assert.AreEqual("Bill", hand[0].Def.Name);
        }

        [Test]
        public void Super_Potion_Discards_An_Energy_To_Heal_40()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Super Potion"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.DamageCounters = 6;
            e.ScenarioAttach(chansey, C(Fighting));
            e.ScenarioAddToHand(0, C(SuperPotion));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
            Assert.AreEqual(0, chansey.Energy.Count);
        }

        [Test]
        public void Clefairy_Doll_Is_Benched_As_A_10_HP_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Clefairy Doll"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ClefairyDoll));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var doll = e.State.Players[0].Bench.Single();
            Assert.AreEqual(10, e.MaxHp(doll));
            e.ApplyCondition(doll, SpecialCondition.Poisoned);
            Assert.IsFalse(doll.Has(SpecialCondition.Poisoned));
        }
    }
}
