using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v151: the Japanese M-P promos: Mega Heatran ex (MP 169), its High Density Armor and Gigantic Press; Lebanne (MP 171, v151.1); the M-P prints of Hariyama, Gastly, Energy Recycler and Gwynn (v151.2).</summary>
    public class MpTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Metal = "MEE-16";

        const string MegaHeatran = "MP-169";
        const string Wailord = "RS-14";
        const string MegaDragonite = "MEP-91";
        const string Lebanne = "MP-171";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void Mega_Heatran_ex_Is_A_Standard_Legal_Japanese_Promo()
        {
            var heatran = C(MegaHeatran);
            Assert.AreEqual(5, db.Cards.Count(d => d.SetCode == "MP"));
            Assert.IsTrue(heatran.IsMegaEx && heatran.IsBasicPokemon);
            Assert.AreEqual(3, heatran.PrizeValue);
            Assert.AreEqual(300, heatran.Hp);
            Assert.AreEqual(4, heatran.RetreatCost);
            Assert.AreEqual("M-P Promos (Japan, not yet in English)", heatran.PrintSetName);
            Assert.IsTrue(Formats.Allows(DeckFormat.Standard, heatran));
        }

        [Test]
        public void High_Density_Armor_Takes_60_Less_Only_At_Full_HP()
        {
            foreach (bool damaged in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Surf"), new ScriptedAgent());
                var wailord = e.ScenarioActive(0, C(Wailord));
                Attach(e, wailord, Water, 5);
                var heatran = e.ScenarioActive(1, C(MegaHeatran));
                if (damaged) heatran.DamageCounters = 1;
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(damaged ? 8 : 1, heatran.DamageCounters, damaged ? "70 in full" : "70 - 60");
            }
        }

        [Test]
        public void Gigantic_Press_Does_140_More_During_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gigantic Press", "Attack: Gigantic Press"), new ScriptedAgent());
            var heatran = e.ScenarioActive(0, C(MegaHeatran));
            Attach(e, heatran, Metal, 3);
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(14, dragonite.DamageCounters);
            dragonite.DamageCounters = 0;
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.AreEqual(28, dragonite.DamageCounters, "140 + 140");
        }

        [Test]
        public void Lebanne_Heals_150_On_Heads_And_30_On_Tails()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Lebanne"), new ScriptedAgent());
                var heatran = e.ScenarioActive(0, C(MegaHeatran));
                heatran.DamageCounters = 20;
                e.ScenarioAddToHand(0, C(Lebanne));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 5 : 17, heatran.DamageCounters);
            }
            Assert.IsTrue(C(Lebanne).IsSupporter);
        }

        [Test]
        public void Lebanne_Needs_A_Damaged_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(MegaHeatran));
            e.ScenarioBench(0, C(Chansey)).DamageCounters = 3;
            e.ScenarioAddToHand(0, C(Lebanne));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Lebanne")), "only the Active Pokémon is healed");
        }

        [Test]
        public void Four_M_P_Promos_Are_Prints_Of_Cards_In_The_Pool()
        {
            Assert.AreEqual("MEG-73", db.MainPrinting(C("MP-167")).Key);
            Assert.AreEqual("POR-48", db.MainPrinting(C("MP-168")).Key);
            Assert.AreEqual("DRI-164", db.MainPrinting(C("MP-170")).Key);
            Assert.AreEqual("PBL-78", db.MainPrinting(C("MP-172")).Key);
            Assert.AreEqual("Gwynn", C("MP-172").Name);
            Assert.AreEqual("M-P Promos (Japan, not yet in English)", C("MP-170").PrintSetName);
            Assert.AreEqual(56, db.All.Count(d => d.SetCode == "MP"), "5 new cards and 51 prints (v156.13)");
        }

        // ------------------------------------------------------------------ v156

        [Test]
        public void Gym_Promos_And_Stamped_Prints()
        {
            Assert.AreEqual("DLR-78", db.MainPrinting(C("MP-134")).Key, "Eevee");
            Assert.AreEqual("BLK-30", db.MainPrinting(C("MP-140")).Key, "Tynamo");
            Assert.AreEqual("CRI-70", db.MainPrinting(C("MP-141")).Key, "Patrat");
            Assert.AreEqual("CRI-82", db.MainPrinting(C("MP-142")).Key, "Special Red Card");
            Assert.AreEqual("MEG-114", db.MainPrinting(C("MP-144")).Key, "Boss's Orders");
            // v156.3: more Pokémon Card Gym promos, all prints.
            Assert.AreEqual("POR-81", db.MainPrinting(C("MP-85")).Key, "Poké Pad");
            Assert.AreEqual("DRI-10", db.MainPrinting(C("MP-95")).Key, "Shaymin (Flower Curtain)");
            Assert.AreEqual("PBL-8", db.MainPrinting(C("MP-96")).Key, "Mega Delphox ex");
            Assert.AreEqual("POR-18", db.MainPrinting(C("MP-97")).Key, "Seel");
            Assert.AreEqual("POR-35", db.MainPrinting(C("MP-98")).Key, "Spritzee");
            Assert.AreEqual("DLR-93", db.MainPrinting(C("MP-99")).Key, "Grisham (Japanese グリ)");
            Assert.AreEqual("PFL-87", db.MainPrinting(C("MP-100")).Key, "Dawn");
            // v156.5: M-P 70-76 and the Pokémon Day stamps.
            Assert.AreEqual("MEP-86", db.MainPrinting(C("MP-70")).Key, "Slowpoke");
            Assert.AreEqual("PBL-31", db.MainPrinting(C("MP-71")).Key, "Mega Slowbro ex");
            Assert.AreEqual("TEF-84", db.MainPrinting(C("MP-72")).Key, "Relicanth");
            Assert.AreEqual("MEP-87", db.MainPrinting(C("MP-73")).Key, "Binacle");
            Assert.AreEqual("WHT-80", db.MainPrinting(C("MP-74")).Key, "Brave Bangle");
            Assert.AreEqual("PBL-79", db.MainPrinting(C("MP-75")).Key, "Jett");
            Assert.AreEqual("MEG-132", db.MainPrinting(C("MP-76")).Key, "Wally's Compassion");
            Assert.AreEqual("PRE-40", db.MainPrinting(C("PKD25-40")).Key, "Sylveon, Pokémon Day 2025");
            Assert.AreEqual("MEG-1", db.MainPrinting(C("PKD26-1")).Key, "Bulbasaur, Pokémon Day 2026");
            Assert.AreEqual("SSP-143", db.MainPrinting(C("PKD25-74")).Key, "Eevee, Pokémon Day 2025 (v156.20)");
            // v156.13: M-P 1-7 Gym promos.
            Assert.AreEqual("MEG-8", db.MainPrinting(C("MP-1")).Key, "Chikorita");
            Assert.AreEqual("POR-22", db.MainPrinting(C("MP-2")).Key, "Lapras ex");
            Assert.AreEqual("TWM-95", db.MainPrinting(C("MP-3")).Key, "Munkidori");
            Assert.AreEqual("MEG-72", db.MainPrinting(C("MP-4")).Key, "Makuhita");
            Assert.AreEqual("POR-74", db.MainPrinting(C("MP-5")).Key, "Hole-Digging Shovel");
            Assert.AreEqual("SSP-169", db.MainPrinting(C("MP-6")).Key, "Counter Gain");
            Assert.AreEqual("MEG-119", db.MainPrinting(C("MP-7")).Key, "Lillie's Determination");
            // v156.27: Japanese SV-P Gym promos 184-190, and the SVP 190 Pikachu with and without the Worlds 2024 stamp.
            var gym = new[] { "DRI-25", "SVP-58", "DRI-105", "DRI-106", "MEG-114", "DRI-181", "PAL-190" };
            for (int i = 0; i < gym.Length; i++)
                Assert.AreEqual(gym[i], db.MainPrinting(C("SVPJ-" + (184 + i))).Key, "SVPJ " + (184 + i));
            Assert.AreEqual("SVP-101", db.MainPrinting(C("SVP-190")).Key);
            Assert.AreEqual("SVP-101", db.MainPrinting(C("WC24-190")).Key);
            // v156.19: Japanese SV-P Gym promos 262-268.
            Assert.AreEqual("ASC-39", db.MainPrinting(C("SVPJ-262")).Key, "Psyduck");
            Assert.AreEqual("MEP-8", db.MainPrinting(C("SVPJ-263")).Key, "Golduck");
            Assert.AreEqual("TEF-78", db.MainPrinting(C("SVPJ-264")).Key, "Flutter Mane");
            Assert.AreEqual("ASC-162", db.MainPrinting(C("SVPJ-265")).Key, "Team Rocket's Kangaskhan ex");
            Assert.AreEqual("ASC-196", db.MainPrinting(C("SVPJ-266")).Key, "Night Stretcher");
            Assert.AreEqual("ASC-206", db.MainPrinting(C("SVPJ-267")).Key, "Team Rocket's Hypnotizer");
            Assert.AreEqual("TWM-154", db.MainPrinting(C("SVPJ-268")).Key, "Kieran");
            // v156.9: M-P 45-51 Gym promos.
            Assert.AreEqual("PFL-12", db.MainPrinting(C("MP-45")).Key, "Charmeleon");
            Assert.AreEqual("CRI-55", db.MainPrinting(C("MP-46")).Key, "Krookodile ex");
            Assert.AreEqual("PFL-67", db.MainPrinting(C("MP-47")).Key, "Toxel");
            Assert.AreEqual("TEF-129", db.MainPrinting(C("MP-48")).Key, "Dudunsparce");
            Assert.AreEqual("CRI-74", db.MainPrinting(C("MP-49")).Key, "Adversity Policy");
            Assert.AreEqual("ASC-181", db.MainPrinting(C("MP-50")).Key, "Air Balloon");
            Assert.AreEqual("WHT-84", db.MainPrinting(C("MP-51")).Key, "Hilda");
            // v156.6: the Extra Battle Day stamped reprints keep their cards' own legality (none of them is Standard).
            // v156.25: the SV-P Extra Battle Day reprints 220-231, also not Standard.
            var svpj = new[] { "LOT-94", "LTR-68", "LOR-TG20", "AOR-100", "BCR-137", "PHF-92", "ROS-110", "BKT-137", "XY-121", "STS-104", "PRC-140", "ROS-97" };
            for (int i = 0; i < svpj.Length; i++)
            {
                var print = C("SVPJ-" + (220 + i));
                Assert.AreEqual(svpj[i], db.MainPrinting(print).Key, "SVPJ " + (220 + i));
                Assert.IsFalse(Formats.Allows(DeckFormat.Standard, print), "SVPJ " + (220 + i) + " keeps its original legality");
            }
            var extraBattleDay = new[] { "MP-53", "MP-54", "MP-55", "MP-56", "MP-57", "MP-58", "MP-59", "MP-60", "MP-61", "MP-62", "MP-63", "MP-64", "MP-65", "SVPJ-290", "SVPJ-219", "SVPJ-130" };
            var mains = new[] { "PAF-235", "UNB-57", "LTR-87", "PRC-121", "STS-105", "AOR-100", "PHF-92", "ROS-110", "GRI-125", "BKT-137", "BUS-120", "BKT-145", "ROS-97", "CRZ-GG66", "CRZ-GG63", "CRZ-140" };
            for (int i = 0; i < extraBattleDay.Length; i++)
            {
                var print = C(extraBattleDay[i]);
                Assert.AreEqual(mains[i], db.MainPrinting(print).Key, extraBattleDay[i]);
                Assert.IsFalse(Formats.Allows(DeckFormat.Standard, print), extraBattleDay[i] + " keeps its original legality");
            }
            Assert.AreEqual("SVP-101", db.MainPrinting(C("MEP-93")).Key, "the WINNER-stamped Pikachu");
            Assert.AreEqual("ASC-153", db.MainPrinting(C("PXP-153")).Key, "the PXP-stamped Rayquaza");
            // v156.1: the McDonald's Pikachu and the "What's Your Favorite? 30" stamped Pitch Black cards.
            Assert.AreEqual("TEF-51", db.MainPrinting(C("MP-20")).Key, "Random Spark Pikachu");
            Assert.AreEqual("PBL-12", db.MainPrinting(C("WYF-12")).Key, "Armarouge");
            Assert.AreEqual("PBL-20", db.MainPrinting(C("WYF-20")).Key, "Primarina");
            Assert.AreEqual("MEP-88", db.MainPrinting(C("WYF-56")).Key, "Zarude (PBL 56 is itself a print of MEP 88)");
            Assert.IsTrue(Formats.Allows(DeckFormat.Standard, C("SVP-101")), "Scrappy Spark Pikachu is Standard through its J reprint");
            var tatsugiri = C("MP-139");
            Assert.IsTrue(tatsugiri.IsMegaEx && tatsugiri.IsBasicPokemon);
            Assert.AreEqual(3, tatsugiri.PrizeValue);
            Assert.IsTrue(C("MP-143").IsSupporter);
        }

        [Test]
        public void Wimp_Out_Removes_The_Retreat_Cost_Against_A_Pokemon_ex()
        {
            foreach (bool ex in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var wimpod = e.ScenarioActive(0, C("MP-138"));
                e.ScenarioActive(1, C(ex ? MegaDragonite : Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(ex ? 0 : 3, e.RetreatCostOf(wimpod));
            }
        }

        [Test]
        public void Lida_Needs_A_Mega_Evolution_ex_Knocked_Out_Last_Turn()
        {
            foreach (bool mega in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Lida"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                var heatran = e.ScenarioBench(0, C(MegaHeatran));
                e.State.Players[0].KnockOuts.Add(new KnockOutRecord(2, C(mega ? MegaDragonite : Chansey), true));
                e.ScenarioAddToDeck(0, C(Metal), 3);
                e.ScenarioAddToHand(0, C("MP-143"));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(mega, e.GetMainActions(0).Any(o => o.Label.Contains("Play Lida")));
                if (!mega) continue;
                Step(e);
                Assert.AreEqual(2, heatran.Energy.Count);
            }
        }

        /// <summary>v156.16: Pikachu with Grey Felt Hat (SVP 85; G mark, so Expanded) benches a Pikachu; the Pikachu-stamped Archeops.</summary>
        [Test]
        public void Pika_Portrait_Benches_A_Pikachu()
        {
            var felt = C("SVP-85");
            Assert.IsFalse(Formats.Allows(DeckFormat.Standard, felt));
            Assert.IsTrue(Formats.Allows(DeckFormat.Expanded, felt));
            Assert.AreEqual("WHT-51", db.MainPrinting(C("WHTPIKA-51")).Key, "Archeops");
            var e = Scenario(new ScriptedAgent().Then("Attack: Pika-Portrait"), new ScriptedAgent());
            var hat = e.ScenarioActive(0, felt);
            e.ScenarioAttach(hat, C("MEE-12"));
            e.ScenarioAddToDeck(0, C("SVP-101"), 1);
            e.ScenarioAddToDeck(0, C(Chansey), 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count(m => m.Name == "Pikachu"));
        }

        private static int SeedFor(bool heads)
        {
            for (int seed = 1; seed < 100000; seed++)
                if (new Rng(seed).CoinFlip() == heads) return seed;
            throw new System.InvalidOperationException("no seed");
        }
    }
}
