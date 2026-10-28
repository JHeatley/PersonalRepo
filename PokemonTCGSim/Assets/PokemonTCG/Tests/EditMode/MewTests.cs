using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v38: Pokémon 151 (MEW), your rulings from this round, and the fixes found while checking for effects that didn't work.</summary>
    public class MewTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Butterfree = "MEW-12";
        const string Spearow = "MEW-21";
        const string Fearow = "MEW-22";
        const string Rattata = "MEW-19";
        const string Raticate = "MEW-20";
        const string ArbokEx = "MEW-24";
        const string Nidoqueen = "MEW-31";
        const string Nidoking = "MEW-34";
        const string Psyduck = "MEW-54";
        const string Victreebel = "MEW-71";
        const string Muk = "MEW-89";
        const string Cubone = "MEW-104";
        const string Marowak = "MEW-105";
        const string Hitmonlee = "MEW-106";
        const string Lickitung = "MEW-108";
        const string Rhydon = "MEW-112";
        const string Chansey = "MEW-113";
        const string MrMime = "MEW-122";
        const string Tauros = "MEW-128";
        const string Magikarp = "MEW-129";
        const string Gyarados = "MEW-130";
        const string Porygon = "MEW-137";
        const string Omastar = "MEW-139";
        const string Kabutops = "MEW-141";
        const string Dratini = "MEW-147";
        const string Dragonite = "MEW-149";
        const string BigAirBalloon = "MEW-155";
        const string ErikasInvitation = "MEW-160";
        const string GiovannisCharisma = "MEW-161";
        const string ProtectiveGoggles = "MEW-164";
        const string RigidBand = "MEW-165";
        const string NidoranF30C = "30C-87";
        const string NidorinaMEP = "MEP-101";
        const string Toedscruel = "PAR-17";
        const string IronMoth = "PAR-28";
        const string RabscaPAF = "PAF-163";
        const string MetapodJTG = "JTG-2";
        const string NightStretcher = "ASC-196";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ================================================================== Data

        [Test]
        public void Every_Pokemon_151_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 207; n++)
                Assert.IsNotNull(db.Get("MEW-" + n), "MEW " + n + " is missing");
        }

        [Test]
        public void Reprints_Are_Alternate_Prints_Of_The_Pool_Card()
        {
            Assert.AreEqual("SVP-161", db.MainPrinting(C("MEW-6")).Key, "Charizard ex");
            Assert.AreEqual("SVP-55", db.MainPrinting(C("MEW-115")).Key, "Kangaskhan ex");
            Assert.AreEqual("SVP-49", db.MainPrinting(C("MEW-145")).Key, "Zapdos ex");
            Assert.AreEqual("SVP-52", db.MainPrinting(C("MEW-150")).Key, "Mewtwo");
            Assert.AreEqual("MEW-114", db.MainPrinting(C("MEW-178")).Key, "Tangela's illustration rare");
        }

        [Test]
        public void Nidorina_From_Mega_Evolution_Promos_Evolves_From_Nidoran_F()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var nidoran = e.ScenarioActive(0, C(NidoranF30C));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.CanEvolveInto(nidoran, C(NidorinaMEP)), "the data said \"Nidoran ♀\" with a space");
            Assert.IsTrue(e.CanEvolveInto(nidoran, C("MEW-30")), "151's Nidorina");
        }

        // ================================================================== Abilities

        [Test]
        public void Evolutionary_Advantage_Lets_Spearow_Evolve_On_Your_First_Turn_Going_Second()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Spearow));
            e.ScenarioBench(0, C(Rattata));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Fearow));
            e.ScenarioAddToHand(0, C(Raticate));
            e.ScenarioSetTurn(0, 2);
            Assert.IsTrue(Offered(e, 0, "into Fearow"));
            Assert.IsFalse(Offered(e, 0, "into Raticate"), "no Ability, normal first-turn rule");

            var first = Scenario(new ScriptedAgent(), new ScriptedAgent());
            first.ScenarioActive(0, C(Spearow));
            first.ScenarioActive(1, C(Tauros));
            first.ScenarioAddToHand(0, C(Fearow));
            first.ScenarioSetTurn(0, 1);
            Assert.IsFalse(Offered(first, 0, "into Fearow"), "going first");
        }

        [Test]
        public void Enthusiastic_King_Ignores_Every_Energy_With_Nidoqueen_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var nidoking = e.ScenarioActive(0, C(Nidoking));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            var impact = C(Nidoking).Attacks[0];
            Assert.AreEqual(4, e.AttackCost(nidoking, impact).Count);

            e.ScenarioBench(0, C(Nidoqueen));
            Assert.AreEqual(0, e.AttackCost(nidoking, impact).Count);
            e.AddEffect(new CardsPAR1.AttackCostUpEffect("test", 1, nidoking.Uid, 4, 1));
            Assert.AreEqual(0, e.AttackCost(nidoking, impact).Count, "a cost set to nothing can't be raised");
        }

        [Test]
        public void Cheering_Bone_Adds_30_To_Marowak_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var marowak = e.ScenarioActive(0, C(Marowak));
            e.ScenarioBench(0, C(Cubone));
            var target = e.ScenarioActive(1, C(Dragonite));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(marowak, target, 30);
            Assert.AreEqual(6, target.DamageCounters);
        }

        [Test]
        public void Ancient_Way_Makes_Weakness_Times_4_For_All_Your_Attacks_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hitmonlee = e.ScenarioActive(0, C(Hitmonlee));
            e.ScenarioBench(0, C(Kabutops));
            var lickitung = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(hitmonlee, lickitung, 30);
            Assert.AreEqual(12, lickitung.DamageCounters, "30 x4 (your ruling)");

            var plain = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lee = plain.ScenarioActive(0, C(Hitmonlee));
            var tongue = plain.ScenarioActive(1, C(Lickitung));
            plain.ScenarioSetTurn(0, 3);
            plain.DealAttackDamage(lee, tongue, 30);
            Assert.AreEqual(6, tongue.DamageCounters);
        }

        [Test]
        public void Mimic_Barrier_Prevents_Damage_With_Matching_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C(MrMime));
            e.ScenarioAttach(mime, C(Psychic));
            var attacker = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioAttach(attacker, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(attacker, mime, 50);
            Assert.AreEqual(0, mime.DamageCounters);
            e.ScenarioAttach(attacker, C(Psychic));
            e.DealAttackDamage(attacker, mime, 50);
            Assert.AreEqual(5, mime.DamageCounters);
        }

        [Test]
        public void Primordial_Tentacles_Stops_The_Opponents_Active_From_Retreating()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var omastar = e.ScenarioActive(0, C(Omastar));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Dratini));
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.CanRetreat(e.State.Players[1]));

            var bench = Scenario(new ScriptedAgent(), new ScriptedAgent());
            bench.ScenarioActive(0, C(Tauros));
            bench.ScenarioBench(0, C(Omastar));
            var t2 = bench.ScenarioActive(1, C(Tauros));
            bench.ScenarioBench(1, C(Dratini));
            bench.ScenarioAttach(t2, C(Psychic));
            bench.ScenarioAttach(t2, C(Psychic));
            bench.ScenarioSetTurn(1, 4);
            Assert.IsTrue(bench.CanRetreat(bench.State.Players[1]), "only while Omastar is Active");
            Assert.IsNotNull(omastar);
        }

        [Test]
        public void Jet_Cruise_Gives_No_Retreat_Cost_Even_With_Sticky_Jail()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var muk = e.ScenarioActive(0, C(Muk));
            e.ScenarioBench(0, C(Dragonite));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(muk));
            e.AddEffect(new CardsMEW1.RetreatUpEffect("test", 1, muk.Uid, 4));
            Assert.AreEqual(0, e.RetreatCostOf(muk), "no Retreat Cost can't be raised");
        }

        [Test]
        public void Lucky_Bonus_Benches_Chansey_Taken_As_A_Prize_During_Your_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            var p = e.State.Players[0];
            p.Prizes.Insert(0, e.NewCard(0, C(Chansey)));
            e.ScenarioSetTurn(0, 3);
            e.TakePrizeCards(0, 1);
            Assert.IsTrue(p.Bench.Any(m => m.Name == "Chansey"), "benched instead of going to the hand");
            Assert.AreEqual(6, p.Prizes.Count + p.Hand.Count, "heads takes 1 more Prize card");
            Assert.LessOrEqual(p.Hand.Count, 1);

            var theirTurn = Scenario(new ScriptedAgent(), new ScriptedAgent());
            theirTurn.ScenarioActive(0, C(Tauros));
            theirTurn.ScenarioActive(1, C(Tauros));
            var q = theirTurn.State.Players[0];
            q.Prizes.Insert(0, theirTurn.NewCard(0, C(Chansey)));
            theirTurn.ScenarioSetTurn(1, 4);
            theirTurn.TakePrizeCards(0, 1);
            Assert.AreEqual(0, q.Bench.Count, "only during your turn");
            Assert.IsTrue(q.Hand.Any(c => c.Def.Name == "Chansey"));
        }

        [Test]
        public void Untamed_One_Discards_The_Top_5_Cards_When_Evolving_From_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("into Gyarados"), new ScriptedAgent());
            e.ScenarioActive(0, C(Magikarp));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Gyarados));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Gyarados", e.State.Players[0].Active.Name);
            Assert.AreEqual(15, e.State.Players[0].Deck.Count);
            Assert.AreEqual(5, e.State.Players[0].Discard.Count);
        }

        // ================================================================== Attacks

        [Test]
        public void Overthink_Makes_Every_Opponent_Flip_Tails_During_Their_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Overthink"), new ScriptedAgent());
            var psyduck = e.ScenarioActive(0, C(Psyduck));
            e.ScenarioAttach(psyduck, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(1, e.State.CurrentPlayer);
            for (int i = 0; i < 20; i++) Assert.IsFalse(e.FlipCoin(1, "test"), "flip " + i);
            NextTurn(e);
            NextTurn(e);
            Assert.AreEqual(1, e.State.CurrentPlayer);
            int heads = 0;
            for (int i = 0; i < 20; i++) if (e.FlipCoin(1, "test")) heads++;
            Assert.Greater(heads, 0, "the effect is over");
        }

        [Test]
        public void Conversion_Gives_A_Pokemon_Without_Weakness_That_Weakness_But_Not_Through_Goggles()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Conversion", "Fire"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(Porygon));
            e.ScenarioAttach(porygon, C(Psychic));
            var dratini = e.ScenarioActive(1, C(Dratini));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(EnergyType.Fire, e.WeaknessOf(dratini), "your ruling: it gains that Weakness");

            var goggles = Scenario(new ScriptedAgent().Then("Attack: Conversion", "Fire"), new ScriptedAgent());
            var p2 = goggles.ScenarioActive(0, C(Porygon));
            goggles.ScenarioAttach(p2, C(Psychic));
            var d2 = goggles.ScenarioActive(1, C(Dratini));
            goggles.ScenarioAttachTool(d2, C(ProtectiveGoggles));
            goggles.ScenarioSetTurn(0, 3);
            Step(goggles);
            Assert.IsNull(goggles.WeaknessOf(d2), "your ruling: Protective Goggles win");
        }

        [Test]
        public void Slow_Acting_Acid_Puts_12_Counters_Unless_The_Pokemon_Leaves_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Slow-Acting Acid"), new ScriptedAgent());
            var victreebel = e.ScenarioActive(0, C(Victreebel));
            e.ScenarioAttach(victreebel, C(Grass));
            e.ScenarioAttach(victreebel, C(Psychic));
            var arbok = e.ScenarioActive(1, C(ArbokEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, arbok.DamageCounters);
            NextTurn(e);
            e.EndTurn();
            Assert.AreEqual(24, arbok.DamageCounters);

            var moved = Scenario(new ScriptedAgent().Then("Attack: Slow-Acting Acid"), new ScriptedAgent());
            var v2 = moved.ScenarioActive(0, C(Victreebel));
            moved.ScenarioAttach(v2, C(Grass));
            moved.ScenarioAttach(v2, C(Psychic));
            var a2 = moved.ScenarioActive(1, C(ArbokEx));
            var benched = moved.ScenarioBench(1, C(Tauros));
            moved.ScenarioSetTurn(0, 3);
            Step(moved);
            NextTurn(moved);
            moved.SwitchActive(1, benched);
            moved.EndTurn();
            Assert.AreEqual(12, a2.DamageCounters, "your ruling: it's no longer the Defending Pokémon");
        }

        [Test]
        public void Charismatic_Drill_Does_140_More_After_Giovannis_Charisma()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Giovanni's Charisma", "Fighting Energy", "Attack: Charismatic Drill"), new ScriptedAgent());
            var rhydon = e.ScenarioActive(0, C(Rhydon));
            e.ScenarioAttach(rhydon, C(Fighting));
            e.ScenarioAttach(rhydon, C(Fighting));
            e.ScenarioAttach(rhydon, C(Fighting));
            e.ScenarioAddToHand(0, C(GiovannisCharisma));
            e.ScenarioAddToHand(0, C(Fighting));
            var gyarados = e.ScenarioActive(1, C(Gyarados));
            e.ScenarioAttach(gyarados, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, gyarados.Energy.Count);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Psychic Energy"));
            Assert.AreEqual(4, rhydon.Energy.Count);
            Step(e);
            Assert.AreEqual(18, gyarados.DamageCounters);
        }

        [Test]
        public void Erikas_Invitation_Brings_A_Basic_From_The_Opponents_Hand_Into_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Erika's Invitation"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Lickitung));
            e.ScenarioAddToHand(1, C(Dratini));
            e.ScenarioAddToHand(1, C(Fearow));
            e.ScenarioAddToHand(0, C(ErikasInvitation));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Dratini", e.State.Players[1].Active.Name);
            Assert.AreEqual("Lickitung", e.State.Players[1].Bench[0].Name);

            var full = Scenario(new ScriptedAgent(), new ScriptedAgent());
            full.ScenarioActive(0, C(Tauros));
            full.ScenarioActive(1, C(Lickitung));
            for (int i = 0; i < 5; i++) full.ScenarioBench(1, C(Dratini));
            full.ScenarioAddToHand(1, C(Dratini));
            full.ScenarioAddToHand(0, C(ErikasInvitation));
            full.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(full, 0, "Play Erika's Invitation"), "Compendium: not into a full Bench");
        }

        // ================================================================== Tools

        [Test]
        public void Big_Air_Balloon_And_Rigid_Band_Check_The_Stage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var queen = e.ScenarioActive(0, C(Nidoqueen));
            var rhydon = e.ScenarioBench(0, C(Rhydon));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAttachTool(queen, C(BigAirBalloon));
            e.ScenarioAttachTool(rhydon, C(RigidBand));
            e.ScenarioAttachTool(tauros, C(RigidBand));
            var attacker = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(0, e.RetreatCostOf(queen), "Stage 2");
            e.DealAttackDamage(attacker, rhydon, 100);
            e.DealAttackDamage(attacker, tauros, 100);
            Assert.AreEqual(7, rhydon.DamageCounters, "Stage 1");
            Assert.AreEqual(10, tauros.DamageCounters, "Basic: no reduction");
        }

        // ================================================================== Your rulings on earlier sets

        [Test]
        public void Thermal_Reactor_Works_When_Switched_In_But_Not_When_Promoted()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var moth = e.ScenarioBench(0, C(IronMoth));
            var holder = e.ScenarioBench(0, C(Lickitung));
            e.ScenarioAttach(holder, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.SwitchActive(0, moth);
            Assert.AreEqual(1, moth.Energy.Count, "switched in during your turn");

            var promo = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var m2 = promo.ScenarioBench(0, C(IronMoth));
            var h2 = promo.ScenarioBench(0, C(Lickitung));
            promo.ScenarioAttach(h2, C(Fire));
            promo.ScenarioActive(1, C(Tauros));
            promo.ScenarioSetTurn(0, 3);
            promo.PromoteActive(0);
            Assert.AreEqual(m2, promo.State.Players[0].Active);
            Assert.AreEqual(0, m2.Energy.Count, "your ruling: not when promoted after a Knock Out");
        }

        [Test]
        public void Revival_Blessing_Can_Bench_An_Evolution_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Revival Blessing"), new ScriptedAgent());
            var rabsca = e.ScenarioActive(0, C(RabscaPAF));
            e.ScenarioAttach(rabsca, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Butterfree), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Bench.Any(m => m.Name == "Butterfree"), "your ruling: any Pokémon");
        }

        [Test]
        public void Night_Stretcher_Is_Stopped_By_Slime_Mold_Colony()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Night Stretcher"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Dratini), 1);
            e.ScenarioAddToHand(0, C(NightStretcher));
            e.ScenarioActive(1, C(Toedscruel));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[0].Hand.Any(c => c.Def.Name == "Dratini"));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Dratini"));
        }

        [Test]
        public void Harden_Checks_The_Final_Damage_After_Other_Reductions()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Harden"), new ScriptedAgent());
            var metapod = e.ScenarioActive(0, C(MetapodJTG));
            e.ScenarioAttach(metapod, C(Grass));
            var attacker = e.ScenarioActive(1, C(Lickitung));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.AddEffect(new DamageTakenEffect("test: 30 less", 0, metapod.Uid, 4, -30));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(attacker, metapod, 80);
            Assert.AreEqual(0, metapod.DamageCounters, "80 - 30 = 50, which Harden prevents");
        }
    }
}
