using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v114: Jungle (JU 1-64): Invisible Wall, Thick Skinned, Shift, Retreat Aid, Heal, Peek, Minimize, Pounce, Chain Lightning,
    /// Boyfriends, Hurricane, Swords Dance, Do the Wave, Mega Drain, Foul Odor, Call for Family, Ram and Spark.
    /// </summary>
    public class JuTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Lightning = "MEE-12";
        const string Fighting = "MEE-14";
        const string Psychic = "MEE-13";

        const string Clefairy = "BS-5";
        const string Nidoking = "BS-11";
        const string Pidgeotto = "BS-22";
        const string Nidorino = "BS-37";
        const string Caterpie = "BS-45";
        const string Doduo = "BS-48";
        const string Metapod = "BS-54";
        const string NidoranM = "BS-55";
        const string Pidgey = "BS-57";
        const string Rattata = "BS-61";
        const string Voltorb = "BS-67";

        const string Clefable = "JU-1";
        const string Electrode = "JU-2";
        const string MrMime = "JU-6";
        const string Nidoqueen = "JU-7";
        const string Pidgeot = "JU-8";
        const string Pinsir = "JU-9";
        const string Scyther = "JU-10";
        const string Snorlax = "JU-11";
        const string Venomoth = "JU-13";
        const string Vileplume = "JU-15";
        const string Wigglytuff = "JU-16";
        const string Butterfree = "JU-33";
        const string Dodrio = "JU-34";
        const string Gloom = "JU-37";
        const string Nidorina = "JU-40";
        const string Persian = "JU-42";
        const string Rhydon = "JU-45";
        const string Bellsprout = "JU-49";
        const string Jigglypuff = "JU-54";
        const string Mankey = "JU-55";
        const string Meowth = "JU-56";
        const string NidoranF = "JU-57";
        const string Oddish = "JU-58";
        const string Pikachu = "JU-60";
        const string Rhyhorn = "JU-61";
        const string Venonat = "JU-63";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(47, db.Cards.Count(d => d.SetCode == "JU"));
            Assert.AreEqual(Clefable, C("JU-17").BaseKey, "the non-holo Clefable");
            Assert.AreEqual(Wigglytuff, C("JU-32").BaseKey);
            Assert.AreEqual("POR-80", C("JU-64").BaseKey, "Poké Ball");
            Assert.AreEqual(EnergyType.Fighting, C(Venomoth).Resistance);
            Assert.AreEqual(30, C(Venomoth).ResistanceAmount);
        }

        [Test]
        public void Invisible_Wall_Stops_30_Or_More_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C(MrMime));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(rattata, mime, 30);
            Assert.AreEqual(0, mime.DamageCounters);
            e.DealAttackDamage(rattata, mime, 20);
            Assert.AreEqual(2, mime.DamageCounters);
        }

        [Test]
        public void Thick_Skinned_Stops_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(1, 4);
            e.ApplyCondition(snorlax, SpecialCondition.Asleep);
            e.ApplyCondition(snorlax, SpecialCondition.Poisoned);
            e.ApplyCondition(snorlax, SpecialCondition.Paralyzed);
            Assert.AreEqual(SpecialCondition.None, snorlax.Conditions);
        }

        [Test]
        public void Shift_Changes_Venomoths_Type_Until_It_Shifts_Again()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shift"), new ScriptedAgent());
            var venomoth = e.ScenarioActive(0, C(Venonat));
            e.ScenarioEvolve(venomoth, C(Venomoth));
            e.ScenarioActive(1, C(Pikachu));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.IsType(venomoth, EnergyType.Lightning));
            Assert.IsFalse(e.IsType(venomoth, EnergyType.Grass));
            e.ScenarioSetTurn(0, 5);
            Assert.IsTrue(e.IsType(venomoth, EnergyType.Lightning), "your ruling: it stays");
        }

        [Test]
        public void Retreat_Aid_Lowers_The_Active_Retreat_Cost_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            var dodrio = e.ScenarioBench(0, C(Doduo));
            e.ScenarioEvolve(dodrio, C(Dodrio));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(3, e.RetreatCostOf(snorlax));
            dodrio.Conditions = SpecialCondition.Asleep;
            Assert.AreEqual(4, e.RetreatCostOf(snorlax), "a Pokémon Power stops while Asleep");
        }

        [Test]
        public void Heal_And_Peek_Are_Offered_Only_When_Awake()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var vileplume = e.ScenarioActive(0, C(Oddish));
            e.ScenarioEvolve(vileplume, C(Gloom));
            e.ScenarioEvolve(vileplume, C(Vileplume));
            vileplume.DamageCounters = 2;
            e.ScenarioBench(0, C(Mankey));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Any(l => l.StartsWith("Use Heal")));
            Assert.IsTrue(labels.Any(l => l.StartsWith("Use Peek")));
            e.ApplyCondition(vileplume, SpecialCondition.Confused);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Heal")));
        }

        [Test]
        public void Minimize_Takes_20_Less_During_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Minimize"), new ScriptedAgent());
            var clefable = e.ScenarioActive(0, C(Clefairy));
            e.ScenarioEvolve(clefable, C(Clefable));
            e.ScenarioAttach(clefable, C(Psychic));
            e.ScenarioAttach(clefable, C(Psychic));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(rattata, clefable, 30);
            Assert.AreEqual(1, clefable.DamageCounters);
        }

        [Test]
        public void Pounce_Reduces_Damage_From_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pounce"), new ScriptedAgent());
            var persian = e.ScenarioActive(0, C(Meowth));
            e.ScenarioEvolve(persian, C(Persian));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(persian, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snorlax.DamageCounters);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(snorlax, persian, 30);
            Assert.AreEqual(2, persian.DamageCounters);
        }

        [Test]
        public void Chain_Lightning_Hits_Benched_Pokemon_Of_The_Defending_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chain Lightning"), new ScriptedAgent());
            var electrode = e.ScenarioActive(0, C(Voltorb));
            e.ScenarioEvolve(electrode, C(Electrode));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(electrode, C(Lightning));
            var bellsprout = e.ScenarioBench(0, C(Bellsprout));
            var pikachu = e.ScenarioBench(0, C(Pikachu));
            var pinsir = e.ScenarioActive(1, C(Pinsir));
            var oddish = e.ScenarioBench(1, C(Oddish));
            var rattata = e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, pinsir.DamageCounters);
            Assert.AreEqual(1, oddish.DamageCounters);
            Assert.AreEqual(0, rattata.DamageCounters);
            Assert.AreEqual(1, bellsprout.DamageCounters, "your own Bench too");
            Assert.AreEqual(0, pikachu.DamageCounters);
        }

        [Test]
        public void Boyfriends_Counts_Your_Nidoking()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Boyfriends"), new ScriptedAgent());
            var nidoqueen = e.ScenarioActive(0, C(NidoranF));
            e.ScenarioEvolve(nidoqueen, C(Nidorina));
            e.ScenarioEvolve(nidoqueen, C(Nidoqueen));
            e.ScenarioAttach(nidoqueen, C(Grass));
            e.ScenarioAttach(nidoqueen, C(Grass));
            var nidoking = e.ScenarioBench(0, C(NidoranM));
            e.ScenarioEvolve(nidoking, C(Nidorino));
            e.ScenarioEvolve(nidoking, C(Nidoking));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, snorlax.DamageCounters);
        }

        [Test]
        public void Hurricane_Returns_The_Defending_Pokemon_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hurricane"), new ScriptedAgent());
            var pidgeot = e.ScenarioActive(0, C(Pidgey));
            e.ScenarioEvolve(pidgeot, C(Pidgeotto));
            e.ScenarioEvolve(pidgeot, C(Pidgeot));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(pidgeot, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioAttach(snorlax, C(Fighting));
            e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[1].Hand;
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Snorlax"));
            Assert.IsTrue(hand.Any(c => c.Def.Name == "Fighting Energy"));
            Assert.AreNotEqual(snorlax, e.State.Players[1].Active);
        }

        [Test]
        public void Swords_Dance_Makes_Next_Turns_Slash_60()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Swords Dance", "Attack: Slash"), new ScriptedAgent());
            var scyther = e.ScenarioActive(0, C(Scyther));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(scyther, C(Grass));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(0, 5);
            Step(e);
            Assert.AreEqual(6, snorlax.DamageCounters);
        }

        [Test]
        public void Do_The_Wave_Counts_Your_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Do the Wave"), new ScriptedAgent());
            var wigglytuff = e.ScenarioActive(0, C(Jigglypuff));
            e.ScenarioEvolve(wigglytuff, C(Wigglytuff));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(wigglytuff, C(Psychic));
            e.ScenarioBench(0, C(Rattata));
            e.ScenarioBench(0, C(Meowth));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snorlax.DamageCounters);
        }

        [Test]
        public void Mega_Drain_Heals_Half_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mega Drain"), new ScriptedAgent());
            var butterfree = e.ScenarioActive(0, C(Caterpie));
            e.ScenarioEvolve(butterfree, C(Metapod));
            e.ScenarioEvolve(butterfree, C(Butterfree));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(butterfree, C(Grass));
            butterfree.DamageCounters = 4;
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, snorlax.DamageCounters);
            Assert.AreEqual(2, butterfree.DamageCounters);
        }

        [Test]
        public void Foul_Odor_Confuses_Both_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Foul Odor"), new ScriptedAgent());
            var gloom = e.ScenarioActive(0, C(Oddish));
            e.ScenarioEvolve(gloom, C(Gloom));
            e.ScenarioAttach(gloom, C(Grass));
            e.ScenarioAttach(gloom, C(Grass));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, rattata.DamageCounters);
            Assert.IsTrue(rattata.Has(SpecialCondition.Confused));
            Assert.IsTrue(gloom.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Call_For_Family_Benches_A_Bellsprout_And_Needs_Bench_Space()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Call for Family"), new ScriptedAgent());
            var bellsprout = e.ScenarioActive(0, C(Bellsprout));
            e.ScenarioAttach(bellsprout, C(Grass));
            e.ScenarioAddToDeck(0, C(Bellsprout), 1);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
            Assert.AreEqual("Bellsprout", e.State.Players[0].Bench[0].Name);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var other = e2.ScenarioActive(0, C(Bellsprout));
            e2.ScenarioAttach(other, C(Grass));
            for (int i = 0; i < 5; i++) e2.ScenarioBench(0, C(Rattata));
            e2.ScenarioActive(1, C(Rattata));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.UsableAttacks(other).Any(a => a.Attack.Name == "Call for Family"), "a full Bench");
        }

        [Test]
        public void Ram_Hurts_Rhydon_And_Switches_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ram"), new ScriptedAgent());
            var rhydon = e.ScenarioActive(0, C(Rhyhorn));
            e.ScenarioEvolve(rhydon, C(Rhydon));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(rhydon, C(Fighting));
            var pinsir = e.ScenarioActive(1, C(Pinsir));
            var rattata = e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, pinsir.DamageCounters);
            Assert.AreEqual(2, rhydon.DamageCounters);
            Assert.AreEqual(rattata, e.State.Players[1].Active);
        }

        [Test]
        public void Spark_Also_Hits_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spark"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            e.ScenarioAttach(pikachu, C(Lightning));
            e.ScenarioAttach(pikachu, C(Lightning));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            var rattata = e.ScenarioBench(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, snorlax.DamageCounters);
            Assert.AreEqual(1, rattata.DamageCounters);
        }

        [Test]
        public void Fix_Superb_Scissors_Ignores_Resistance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Superb Scissors"), new ScriptedAgent());
            var crustle = e.ScenarioActive(0, C("DRI-12"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(crustle, C(Grass));
            var gholdengo = e.ScenarioActive(1, C("PAR-139"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, gholdengo.DamageCounters, "your ruling (v114): Gholdengo ex's Grass Resistance doesn't apply");
        }
    }
}
