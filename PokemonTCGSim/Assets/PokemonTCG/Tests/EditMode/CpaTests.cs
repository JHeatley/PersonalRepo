using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v63: Champion's Path (CPA 1-80), with the Compendium rulings for its cards: Gnawing Aura against Jungle Rising and
    /// Solar Evolution, Marnie against Dew Guard, Sonia and Hyper Potion.
    /// </summary>
    public class CpaTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Tauros = "MEW-128";
        const string BossesOrders = "MEG-114";
        const string Potion = "POR-83";
        const string Piers = "ASR-TG28";
        const string Milotic = "EVS-38";
        const string Skiploom = "EVS-3";
        const string Jumpluff = "EVS-4";
        const string ZarudeV = "VIV-22";

        const string VenusaurV = "CPA-1";
        const string EldegossV = "CPA-5";
        const string IncineroarV = "CPA-8";
        const string WailordV = "CPA-13";
        const string DrednawV = "CPA-14";
        const string GardevoirV = "CPA-16";
        const string Hatterene = "CPA-20";
        const string CursolaV = "CPA-21";
        const string Machop = "CPA-24";
        const string Machoke = "CPA-25";
        const string Machamp = "CPA-26";
        const string LucarioV = "CPA-27";
        const string GrapploctV = "CPA-32";
        const string Obstagoon = "CPA-37";
        const string Liepard = "CPA-40";
        const string Scrafty = "CPA-42";
        const string Trubbish = "CPA-43";
        const string Inkay = "CPA-44";
        const string DuraludonV = "CPA-47";
        const string Altaria = "CPA-49";
        const string CharizardV = "CPA-79";
        const string FullHeal = "CPA-51";
        const string HyperPotion = "CPA-54";
        const string Marnie = "CPA-56";
        const string RotomBike = "CPA-63";
        const string RotomPhone = "CPA-64";
        const string Sonia = "CPA-65";
        const string FoodTin = "CPA-66";
        const string TeamYellGrunt = "CPA-67";

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

        private sealed class EventLog : IGameObserver
        {
            public readonly List<GameEvent> Events = new List<GameEvent>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Events.Add(ev); }
        }

        // ================================================================== Data

        [Test]
        public void Every_Champions_Path_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 80; n++) Assert.IsNotNull(db.Get("CPA-" + n), "CPA " + n + " is missing");
            Assert.AreEqual("SHF-SV18", db.MainPrinting(C("CPA-9")).Key, "Sizzlipede came with Shining Fates");
            Assert.AreEqual("SHF-64", db.MainPrinting(C("CPA-22")).Key, "Alcremie V came with Shining Fates");
            Assert.AreEqual("SHF-SV107", db.MainPrinting(C("CPA-74")).Key, "Charizard VMAX came with Shining Fates");
            Assert.AreEqual("CRZ-124", db.MainPrinting(C("CPA-50")).Key, "Bede was already in the pool");
            Assert.AreEqual("CPA-14", db.MainPrinting(C("CPA-69")).Key, "the full-art Drednaw V");
            Assert.AreEqual("CPA-66", db.MainPrinting(C("CPA-80")).Key, "the gold Suspicious Food Tin");
            Assert.AreEqual("CPA-79", db.MainPrinting(C(CharizardV)).Key, "Charizard V is only a secret rare here");
            Assert.AreEqual("Charizard V", C("CPA-74").EvolvesFrom);
            Assert.IsFalse(Formats.IsStandardCard(C(Marnie)), "regulation mark D: Expanded");
        }

        // ================================================================== Pokémon

        [Test]
        public void Pollen_Bomb_Makes_The_Defending_Pokemon_Asleep_And_Poisoned()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pollen Bomb"), new ScriptedAgent());
            var venusaur = e.ScenarioActive(0, C(VenusaurV));
            Attach(e, venusaur, Grass, 3);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, tauros.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Asleep) && tauros.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Happy_Match_Takes_A_Supporter_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Eldegoss V", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(EldegossV));
            e.ScenarioAddToDiscard(0, C(BossesOrders), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Boss's Orders"));
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Float_Up_Shuffles_Eldegoss_V_And_Its_Cards_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Float Up", "Yes"), new ScriptedAgent());
            var eldegoss = e.ScenarioActive(0, C(EldegossV));
            Attach(e, eldegoss, Grass, 2);
            e.ScenarioBench(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, tauros.DamageCounters);
            Assert.IsFalse(e.State.Players[0].InPlay().Any(m => m.Name == "Eldegoss V"));
            Assert.AreEqual(23, e.State.Players[0].Deck.Count, "20 + Eldegoss V + 2 Energy");
        }

        [Test]
        public void Grand_Flame_Attaches_2_Fire_Energy_From_The_Discard_Pile_To_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Grand Flame", "Fire Energy|Fire Energy"), new ScriptedAgent());
            var incineroar = e.ScenarioActive(0, C(IncineroarV));
            Attach(e, incineroar, Fire, 3);
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, tauros.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Draw_Up_Attaches_Up_To_3_Water_Energy_To_Wailord_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Draw Up", "Water Energy|Water Energy|Water Energy"), new ScriptedAgent());
            var wailord = e.ScenarioActive(0, C(WailordV));
            Attach(e, wailord, Water, 1);
            e.ScenarioAddToDiscard(0, C(Water), 4);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, wailord.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Solid_Shell_Takes_30_Off_And_Aura_Sphere_Hits_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Aura Sphere"), new ScriptedAgent());
            var lucario = e.ScenarioActive(0, C(LucarioV));
            Attach(e, lucario, Fighting, 1);
            var drednaw = e.ScenarioActive(1, C(DrednawV));
            var benched = e.ScenarioBench(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, drednaw.DamageCounters, "40 - 30");
            Assert.AreEqual(2, benched.DamageCounters, "20 to the Bench");
        }

        [Test]
        public void Swelling_Pulse_Does_80_More_After_Gardevoir_V_Was_Healed()
        {
            var me = new ScriptedAgent().Then("Play Potion");
            var e = Scenario(me, new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C(GardevoirV));
            Attach(e, gardevoir, Psychic, 3);
            gardevoir.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(Potion));
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, gardevoir.DamageCounters);
            me.Then("Attack: Swelling Pulse");
            Step(e);
            Assert.AreEqual(20, wailord.DamageCounters, "120 + 80");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Swelling Pulse"), new ScriptedAgent());
            var g2 = e2.ScenarioActive(0, C(GardevoirV));
            Attach(e2, g2, Psychic, 3);
            var w2 = e2.ScenarioActive(1, C(WailordV));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(12, w2.DamageCounters, "not healed this turn");
        }

        [Test]
        public void Hazard_Sensor_Confuses_The_Attacking_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            var hatterene = e.ScenarioActive(1, C(Hatterene));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, hatterene.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Gnawing_Aura_Puts_3_Counters_On_A_Pokemon_Getting_Energy_From_The_Hand_While_Cursola_Is_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Tauros"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(CursolaV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, tauros.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Tauros"), new ScriptedAgent());
            var t2 = e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioBench(1, C(CursolaV));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, t2.DamageCounters, "only while Galarian Cursola V is Active");
        }

        [Test]
        public void Jungle_Rising_Heals_After_Gnawing_Aura_Puts_Its_Counters()
        {
            // Compendium (2022-05-05): the player using Jungle Rising chooses the order; the engine takes the counters first.
            var log = new EventLog();
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Jungle Rising", "Grass Energy", "Grass Energy"));
            e.AddObserver(log);
            e.ScenarioActive(0, C(CursolaV));
            var zarude = e.ScenarioActive(1, C(ZarudeV));
            Attach(e, zarude, Grass, 2);
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual(2, tauros.Energy.Count);
            Assert.AreEqual(2, log.Events.Count(ev => ev.Type == GameEventType.UseAbility && ev.Text != null && ev.Text.Contains("Gnawing Aura")), "both attachments were from the hand");
            Assert.AreEqual(0, tauros.DamageCounters, "the healing came after the counters");
        }

        [Test]
        public void Solar_Evolution_Goes_First_So_Gnawing_Aura_Misses_The_Evolved_Pokemon()
        {
            // Compendium (2022-04-28): the owner chooses the order; if Solar Evolution goes first, Jumpluff isn't the Pokémon
            // the Energy was attached to, so Gnawing Aura does nothing. Skiploom's player sits second, so the order is the engine's.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Grass Energy to Skiploom", "Yes", "Jumpluff"));
            e.ScenarioActive(0, C(CursolaV));
            var skiploom = e.ScenarioActive(1, C(Skiploom));
            e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioAddToDeck(1, C(Jumpluff), 1);
            e.ScenarioSetTurn(1, 3);
            Step(e);
            Assert.AreEqual("Jumpluff", skiploom.Name);
            Assert.AreEqual(0, skiploom.DamageCounters);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Grass Energy to Skiploom", "No"));
            e2.ScenarioActive(0, C(CursolaV));
            var s2 = e2.ScenarioActive(1, C(Skiploom));
            e2.ScenarioAddToHand(1, C(Grass));
            e2.ScenarioAddToDeck(1, C(Jumpluff), 1);
            e2.ScenarioSetTurn(1, 3);
            Step(e2);
            Assert.AreEqual("Skiploom", s2.Name);
            Assert.AreEqual(3, s2.DamageCounters, "without evolving, Gnawing Aura applies");
        }

        [Test]
        public void Hollow_Missile_Puts_3_Counters_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hollow Missile"), new ScriptedAgent());
            var cursola = e.ScenarioActive(0, C(CursolaV));
            Attach(e, cursola, Psychic, 2);
            var active = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, active.DamageCounters);
            Assert.AreEqual(3, benched.DamageCounters);
        }

        [Test]
        public void Macho_Revenge_Counts_Fighting_Pokemon_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Macho Revenge"), new ScriptedAgent());
            var machamp = e.ScenarioActive(0, C(Machamp));
            Attach(e, machamp, Fighting, 2);
            e.ScenarioAddToDiscard(0, C(Machop), 2);
            e.ScenarioAddToDiscard(0, C(LucarioV), 1);
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            e.ScenarioAddToDiscard(0, C(Fighting), 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, wailord.DamageCounters, "3 Fighting Pokémon x 20");
        }

        [Test]
        public void Tie_Up_Stops_Only_A_Basic_Defending_Pokemon_Attacking()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tie Up"), new ScriptedAgent());
            var grapploct = e.ScenarioActive(0, C(GrapploctV));
            Attach(e, grapploct, Fighting, 1);
            var wailord = e.ScenarioActive(1, C(WailordV));
            Attach(e, wailord, Water, 4);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsFalse(Offered(e, 1, "Attack: Ocean Waves"), "Wailord V is a Basic Pokémon");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Tie Up"), new ScriptedAgent());
            var g2 = e2.ScenarioActive(0, C(GrapploctV));
            Attach(e2, g2, Fighting, 1);
            var machoke = e2.ScenarioActive(1, C(Machoke));
            Attach(e2, machoke, Fighting, 1);
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            NextTurn(e2);
            Assert.IsTrue(Offered(e2, 1, "Attack: Low Kick"), "Machoke is a Stage 1");
        }

        [Test]
        public void Wicked_Ruler_Has_The_Opponent_Discard_Down_To_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Wicked Ruler"), new ScriptedAgent());
            e.ScenarioActive(0, C(Obstagoon));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
            Assert.AreEqual(3, e.State.Players[1].Discard.Count);
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Psychic));
            Assert.IsFalse(Offered(e, 0, "Use Wicked Ruler"), "once during your turn");
        }

        [Test]
        public void Limber_Keeps_Liepard_From_Being_Paralyzed()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var liepard = e.ScenarioActive(0, C(Liepard));
            e.ScenarioActive(1, C(Tauros));
            e.ApplyCondition(liepard, SpecialCondition.Paralyzed);
            Assert.IsFalse(liepard.Has(SpecialCondition.Paralyzed));
            e.ApplyCondition(liepard, SpecialCondition.Confused);
            Assert.IsTrue(liepard.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Bad_Brawl_Does_90_More_After_Piers()
        {
            var me = new ScriptedAgent().Then("Play Piers");
            var e = Scenario(me, new ScriptedAgent());
            var scrafty = e.ScenarioActive(0, C(Scrafty));
            Attach(e, scrafty, Darkness, 3);
            e.ScenarioAddToHand(0, C(Piers));
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            me.Then("Attack: Bad Brawl");
            Step(e);
            Assert.AreEqual(18, wailord.DamageCounters, "90 + 90");
        }

        [Test]
        public void Venoshock_And_Attack_From_Behind_Do_50_More_Against_Poisoned_Or_Confused()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Venoshock"), new ScriptedAgent());
            var trubbish = e.ScenarioActive(0, C(Trubbish));
            Attach(e, trubbish, Darkness, 2);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ApplyCondition(wailord, SpecialCondition.Poisoned);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, wailord.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Attack from Behind"), new ScriptedAgent());
            var inkay = e2.ScenarioActive(0, C(Inkay));
            Attach(e2, inkay, Darkness, 2);
            var w2 = e2.ScenarioActive(1, C(WailordV));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(2, w2.DamageCounters, "not Confused: 20");
        }

        [Test]
        public void Gatling_Slug_Counts_Metal_Energy_And_Hard_Coat_Takes_30_Off()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gatling Slug"), new ScriptedAgent());
            var duraludon = e.ScenarioActive(0, C(DuraludonV));
            Attach(e, duraludon, Metal, 3);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, wailord.DamageCounters, "10 + 3 x 40");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var tauros = e2.ScenarioActive(0, C(Tauros));
            Attach(e2, tauros, Psychic, 2);
            var d2 = e2.ScenarioActive(1, C(DuraludonV));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, d2.DamageCounters, "30 - 30");
        }

        [Test]
        public void Miraculous_Charm_Stops_Damage_From_Pokemon_V_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Magical Shot"), new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C(GardevoirV));
            Attach(e, gardevoir, Psychic, 1);
            var altaria = e.ScenarioActive(1, C(Altaria));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, altaria.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var tauros = e2.ScenarioActive(0, C(Tauros));
            Attach(e2, tauros, Psychic, 2);
            var a2 = e2.ScenarioActive(1, C(Altaria));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(3, a2.DamageCounters);
        }

        [Test]
        public void Fire_Spin_Discards_2_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fire Spin"), new ScriptedAgent());
            var charizard = e.ScenarioActive(0, C(CharizardV));
            Attach(e, charizard, Fire, 4);
            var wailord = e.ScenarioActive(1, C(WailordV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(22, wailord.DamageCounters);
            Assert.AreEqual(2, charizard.Energy.Count);
        }

        // ================================================================== Trainers

        [Test]
        public void Full_Heal_Cures_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Full Heal"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(FullHeal));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Full Heal"), "nothing to cure");
            e.ApplyCondition(tauros, SpecialCondition.Confused);
            e.ApplyCondition(tauros, SpecialCondition.Poisoned);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, tauros.Conditions);
        }

        [Test]
        public void Hyper_Potion_Heals_120_And_Discards_2_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hyper Potion"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            tauros.DamageCounters = 11;
            var benched = e.ScenarioBench(0, C(Tauros));
            Attach(e, benched, Psychic, 1);
            benched.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(HyperPotion));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual(5, benched.DamageCounters, "only 1 Energy");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var t2 = e2.ScenarioActive(0, C(Tauros));
            Attach(e2, t2, Psychic, 2);
            e2.ScenarioAddToHand(0, C(HyperPotion));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Play Hyper Potion"), "Compendium: it needs damage to heal");
        }

        [Test]
        public void Marnie_Puts_Both_Hands_On_The_Bottom_Then_Draws_5_And_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Marnie"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Marnie));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 6; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
            Assert.AreEqual("Fire Energy", e.State.Players[0].Deck.Last().Def.Name, "the hand went to the bottom");
            Assert.AreEqual("Water Energy", e.State.Players[1].Deck.Last().Def.Name);
            Assert.IsTrue(e.State.Players[0].Hand.All(c => c.Def.Name == "Psychic Energy"), "drawn from the top");
        }

        [Test]
        public void Dew_Guard_Keeps_The_Opponents_Hand_Out_Of_Marnie()
        {
            // Compendium (2021): Milotic's Dew Guard stops Marnie's effect on its player.
            var e = Scenario(new ScriptedAgent().Then("Play Marnie"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Marnie));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Milotic));
            for (int i = 0; i < 6; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(6, e.State.Players[1].Hand.Count);
            Assert.IsTrue(e.State.Players[1].Hand.All(c => c.Def.Name == "Water Energy"));
        }

        [Test]
        public void Dew_Guard_Stops_Averys_Bench_Discard_But_Not_Team_Yell_Grunt()
        {
            // Compendium (2021): Dew Guard stops Avery's Bench discard, not a Supporter removing Energy from your Pokémon.
            var e = Scenario(new ScriptedAgent().Then("Play Avery"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C("CRE-130"));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Milotic));
            for (int i = 0; i < 4; i++) e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.AreEqual(5, e.State.Players[1].Bench.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Play Team Yell Grunt"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(TeamYellGrunt));
            var tauros = e2.ScenarioActive(1, C(Tauros));
            Attach(e2, tauros, Water, 1);
            e2.ScenarioBench(1, C(Milotic));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual(1, e2.State.Players[1].Hand.Count);
        }

        [Test]
        public void Rotom_Bike_Draws_Up_To_6_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rotom Bike"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(RotomBike));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "the turn ends");
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Rotom_Phone_Puts_The_Chosen_Card_On_Top()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rotom Phone", "Charizard V"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(RotomPhone));
            e.ScenarioAddToDeck(0, C(CharizardV), 1);
            var deck = e.State.Players[0].Deck;
            var charizard = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            deck.Insert(3, charizard);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(charizard, e.State.Players[0].Deck[0]);
            Assert.AreEqual(21, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Sonia_Finds_2_Basic_Energy_Or_2_Basic_Pokemon_Not_A_Mix()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Sonia", "basic Energy", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Sonia));
            e.ScenarioAddToDeck(0, C(Tauros), 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.IsTrue(e.State.Players[0].Hand.All(c => c.Def.Name == "Psychic Energy"));

            var e2 = Scenario(new ScriptedAgent().Then("Play Sonia", "Basic Pokémon", "Tauros|Tauros"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(Sonia));
            e2.ScenarioAddToDeck(0, C(Tauros), 2);
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(2, e2.State.Players[0].Hand.Count(c => c.Def.Name == "Tauros"));
        }

        [Test]
        public void Suspicious_Food_Tin_Heals_80_And_Discards_A_Psychic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Suspicious Food Tin"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 1);
            Attach(e, tauros, Fire, 1);
            tauros.DamageCounters = 10;
            e.ScenarioAddToHand(0, C(FoodTin));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters);
            Assert.AreEqual("Fire Energy", tauros.Energy.Single().Def.Name);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var t2 = e2.ScenarioActive(0, C(Tauros));
            Attach(e2, t2, Fire, 1);
            t2.DamageCounters = 10;
            e2.ScenarioAddToHand(0, C(FoodTin));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Play Suspicious Food Tin"), "needs a Psychic Energy attached");
        }

        [Test]
        public void Team_Yell_Grunt_Puts_An_Opponents_Energy_Into_Their_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Yell Grunt"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(TeamYellGrunt));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Water, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual("Water Energy", e.State.Players[1].Hand.Single().Def.Name);
        }
    }
}
