using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v150: the Simplified Chinese special art rares coming to China (Hilda 173/086 SAR in CSV10.6, N's Plan 173/086 SAR in CSV10.5; v150.1),
    /// added as prints of the cards in the pool and labelled as China prints in their set name; v150.2: Misty's Psyduck 0307/07 in
    /// CBB7.
    /// </summary>
    public class ChinaPrintsTests : ScenarioTestBase
    {
        [Test]
        public void Hilda_And_Ns_Plan_Have_Their_China_Prints()
        {
            var hilda = C("CSV10.6-173");
            Assert.AreEqual("Hilda", hilda.Name);
            Assert.AreEqual("WHT-84", db.MainPrinting(hilda).Key);
            Assert.AreEqual("Simplified Chinese CSV10.6 (coming to China)", hilda.PrintSetName);

            var plan = C("CSV10.5-173");
            Assert.AreEqual("N's Plan", plan.Name);
            Assert.AreEqual("BLK-83", db.MainPrinting(plan).Key);
            Assert.AreEqual("Simplified Chinese CSV10.5 (coming to China)", plan.PrintSetName);

            Assert.IsTrue(db.PrintingsOf(C("WHT-84")).Contains(hilda));
            Assert.IsTrue(db.PrintingsOf(C("BLK-83")).Contains(plan));

            // The set code has a dot: deck lists ("1 Hilda CSV10.6 173") still find the print.
            var errors = new System.Collections.Generic.List<string>();
            var deck = Decklist.Parse("1 Hilda CSV10.6 173\n1 N's Plan CSV10.5 173", db, errors);
            CollectionAssert.IsEmpty(errors);
            Assert.AreEqual("CSV10.6-173", deck[0].Card.Key);
            Assert.AreEqual("CSV10.5-173", deck[1].Card.Key);
        }

        [Test]
        public void Mistys_Psyduck_Has_Its_China_Print()
        {
            var psyduck = C("CBB7-307");
            Assert.AreEqual("Misty's Psyduck", psyduck.Name);
            Assert.AreEqual("DRI-45", db.MainPrinting(psyduck).Key);
            Assert.AreEqual("Simplified Chinese CBB7 (coming to China)", psyduck.PrintSetName);
            Assert.AreSame(psyduck, db.Get("CBB7", "0307"), "printed 0307/07; the leading zero doesn't matter");
        }

        /// <summary>v152.2: the Halloween-stamped Trick or Trade 2026 prints of Chaos Rising's Zubat, Golbat and Crobat.</summary>
        [Test]
        public void Trick_Or_Trade_Stamped_Prints()
        {
            Assert.AreEqual("CRI-49", db.MainPrinting(C("TOT-49")).Key);
            Assert.AreEqual("CRI-50", db.MainPrinting(C("TOT-50")).Key);
            Assert.AreEqual("MEP-76", db.MainPrinting(C("TOT-51")).Key, "Chaos Rising's Crobat is itself a print of the Mega Promo");
            Assert.AreEqual("Trick or Trade 2026 (Halloween stamp)", C("TOT-51").PrintSetName);
            Assert.AreEqual("Crobat", C("TOT-51").Name);
            // v156.11: the 2025 pumpkin-stamped Spiritomb has its own code (your choice).
            Assert.AreEqual("MEG-87", db.MainPrinting(C("TOT25-87")).Key);
            Assert.AreEqual("Trick or Trade 2025 (Halloween stamp)", C("TOT25-87").PrintSetName);
            // v156.12: the ACE TRAINER stamped Celebratory Fanfare (set code + ACE, your choice).
            Assert.AreEqual("MEP-28", db.MainPrinting(C("MEPACE-28")).Key);
            Assert.AreEqual("Celebratory Fanfare", C("MEPACE-28").Name);
            // v156.13: Premier Ball League stamps (PREPBL, Judge PREPBLJ), the Asia Promo stamp (MEGASIA), the Worlds 2025 Pikachu
            // (SVP 225) and the CBB3C China prints.
            Assert.AreEqual("ASC-16", db.MainPrinting(C("PREPBL-4")).Key, "Budew");
            Assert.AreEqual("ASC-16", db.MainPrinting(C("PREPBLJ-4")).Key, "Budew (Judge)");
            Assert.AreEqual("MEG-64", db.MainPrinting(C("MEGASIA-64")).Key, "Xerneas");
            Assert.AreEqual("SVP-101", db.MainPrinting(C("SVP-225")).Key, "Scrappy Spark Pikachu");
            Assert.AreEqual("PAF-57", db.MainPrinting(C("CBB3C-307")).Key, "Gengar");
            Assert.AreEqual("MEW-52", db.MainPrinting(C("CBB3C-207")).Key, "Meowth");
            Assert.AreEqual("MEW-104", db.MainPrinting(C("CBB3C-407")).Key, "Cubone");
            // v156.14: Team Rocket's Mewtwo ex with the Play! Pokémon and Premier Ball League stamps.
            Assert.AreEqual("DRI-81", db.MainPrinting(C("DRIPPS-81")).Key);
            Assert.AreEqual("DRI-81", db.MainPrinting(C("DRIPBL-81")).Key);
        }

        /// <summary>v156.2: Brock's Scouting's China SAR, and the Play! Pokémon and World Championships 2026 stamped prints.</summary>
        [Test]
        public void Brocks_Scouting_China_Print_And_Play_And_Worlds_Stamps()
        {
            var brock = C("CSV10C-276");
            Assert.AreEqual("JTG-146", db.MainPrinting(brock).Key);
            Assert.AreEqual("Simplified Chinese CSV10C (coming to China)", brock.PrintSetName);
            // v156.7: a Play! Pokémon stamped print's set code is its own set's code with PPS after it (your choice).
            Assert.AreEqual("MEP-91", db.MainPrinting(C("ASCPPS-152")).Key, "Mega Dragonite ex");
            Assert.AreEqual("ASC-155", db.MainPrinting(C("ASCPPS-155")).Key, "N's Zekrom");
            Assert.AreEqual("POR-21", db.MainPrinting(C("PORPPS-21")).Key, "Mega Starmie ex");
            Assert.AreEqual("POR-62", db.MainPrinting(C("PORPPS-62")).Key, "Meowth ex");
            Assert.AreEqual("Perfect Order (Play! Pokémon stamp)", C("PORPPS-62").PrintSetName);
            Assert.AreEqual("MEG-86", db.MainPrinting(C("MEGPPS-86")).Key, "Mega Absol ex");
            Assert.AreEqual("MEG-104", db.MainPrinting(C("MEGPPS-104")).Key, "Mega Kangaskhan ex");
            Assert.AreEqual("MEG-119", db.MainPrinting(C("MEGPPS-119")).Key, "Lillie's Determination");
            Assert.AreEqual("MEG-74", db.MainPrinting(C("MEGPPS-74")).Key, "Lunatone");
            Assert.AreEqual("DRI-34", db.MainPrinting(C("DRIPPS-34")).Key, "Ethan's Typhlosion");
            Assert.IsNull(db.Get("PPS-62"), "the old PPS code is gone");
            // v156.26: Pokémon Horizons promos (SVP 191, 192) and Horizons-stamped prints (set code + HOR, your choice).
            Assert.AreEqual("SVI-13", db.MainPrinting(C("SVP-191")).Key, "Sprigatito");
            Assert.AreEqual("SVP-79", db.MainPrinting(C("SVP-192")).Key, "Fuecoco");
            Assert.AreEqual("SVP-76", db.MainPrinting(C("PALHOR-12")).Key, "Sprigatito");
            Assert.AreEqual("SSP-29", db.MainPrinting(C("SSPHOR-29")).Key, "Fuecoco");
            Assert.AreEqual("SSP-50", db.MainPrinting(C("SSPHOR-50")).Key, "Quaxly");
            Assert.AreEqual("SSP-161", db.MainPrinting(C("SSPHOR-161")).Key, "Terapagos");
            Assert.AreEqual("SCR-31", db.MainPrinting(C("SCR711-31")).Key, "7-Eleven Lapras (v156.28, your code SCR711)");
            // v156.29: Play! Pokémon prize pack stamps use the set code + P (your choice).
            Assert.AreEqual("PAR-178", db.MainPrinting(C("PARP-178")).Key, "Technical Machine: Evolution");
            Assert.AreEqual("PAL-188", db.MainPrinting(C("PALP-188")).Key, "Super Rod");
            Assert.AreEqual("TEF-144", db.MainPrinting(C("TEFP-144")).Key, "Buddy-Buddy Poffin");
            // v156.21: more Play! Pokémon stamps.
            Assert.AreEqual("SCR-131", db.MainPrinting(C("SCRPPS-131")).Key, "Area Zero Underdepths");
            Assert.AreEqual("TWM-159", db.MainPrinting(C("TWMPPS-159")).Key, "Ogre's Mask");
            Assert.AreEqual("TWM-165", db.MainPrinting(C("TWMPPS-165")).Key, "Unfair Stamp");
            Assert.AreEqual("SCR-128", db.MainPrinting(C("SCRPPS-128")).Key, "Terapagos ex");
            Assert.AreEqual("ASC-142", db.MainPrinting(C("SFAPPS-38")).Key, "Fezandipiti ex");
            Assert.AreEqual("SVP-101", db.MainPrinting(C("WC26-93")).Key, "Worlds Pikachu");
            Assert.AreEqual("SVP-45", db.MainPrinting(C("WC26-92")).Key, "Worlds Paradise Resort");
            Assert.AreEqual("Mega Promos (WINNER stamp)", C("MEP-93").PrintSetName, "your choice: MEP 93 stays the WINNER one");
        }

        /// <summary>v156.17: Captain Pikachu (China-only, its own card, Standard by its H mark) and the Gem Pack anime prints.</summary>
        [Test]
        public void Captain_Pikachu_And_Gem_Pack_Prints()
        {
            var captain = C("CBB5C-107");
            Assert.AreEqual("Captain Pikachu", captain.Name);
            Assert.IsTrue(Formats.Allows(DeckFormat.Standard, captain));
            Assert.AreEqual("CBB5C-107", db.MainPrinting(C("CBB1C-709")).Key);
            Assert.AreEqual("JTG-17", db.MainPrinting(C("CBB5C-2207")).Key, "Floragato");
            Assert.AreEqual("SSP-30", db.MainPrinting(C("CBB5C-2307")).Key, "Crocalor");
            Assert.AreEqual("SSP-51", db.MainPrinting(C("CBB5C-2407")).Key, "Quaxwell");
            Assert.AreEqual("SVI-13", db.MainPrinting(C("CBB1C-109")).Key, "Sprigatito");
            Assert.AreEqual("PAL-35", db.MainPrinting(C("CBB1C-309")).Key, "Fuecoco");
            Assert.AreEqual("PAL-49", db.MainPrinting(C("CBB1C-509")).Key, "Quaxly");
            // v156.24: more Gem Pack Vol. 1 prints.
            Assert.AreEqual("MEE-9", db.MainPrinting(C("CBB1C-1801")).Key, "Basic Grass Energy");
            Assert.AreEqual("MEE-10", db.MainPrinting(C("CBB1C-1802")).Key, "Basic Fire Energy");
            Assert.AreEqual("MEE-11", db.MainPrinting(C("CBB1C-1803")).Key, "Basic Water Energy");
            Assert.AreEqual("PAL-183", db.MainPrinting(C("CBB1C-1702")).Key, "Great Ball");
            Assert.AreEqual("MEG-131", db.MainPrinting(C("CBB1C-1703")).Key, "Ultra Ball");
            // v156.18: Gem Pack Vol. 2 (CBB2C) prints.
            Assert.AreEqual("SP-211", db.MainPrinting(C("CBB2C-915")).Key, "Sylveon");
            Assert.AreEqual("ASR-13", db.MainPrinting(C("CBB2C-715")).Key, "Leafeon");
            Assert.AreEqual("SP-129", db.MainPrinting(C("CBB2C-615")).Key, "Umbreon");
            Assert.AreEqual("ASR-119", db.MainPrinting(C("CBB2C-115")).Key, "Eevee");
            Assert.AreEqual("MEE-13", db.MainPrinting(C("CBB2C-1102")).Key, "Basic Psychic Energy");
            Assert.AreEqual("BST-129", db.MainPrinting(C("CBB2C-1002")).Key, "Level Ball");
            Assert.AreEqual("FST-237", db.MainPrinting(C("CBB2C-1003")).Key, "Quick Ball");
            // v156.22: Chinese SV-P promos use SVPC (your choice; the English SVP 3 is a different card).
            Assert.AreEqual("SVP-53", db.MainPrinting(C("SVPC-3")).Key, "Mew ex");
            Assert.AreEqual("Quaxly", C("SVP-3").Name);
            // v156.23: the four Pikachu art rares of the Chinese 151 (code 151C).
            foreach (var n in new[] { "170", "171", "172", "173" })
                Assert.AreEqual("MEW-25", db.MainPrinting(C("151C-" + n)).Key, "Pikachu " + n);

            var e = Scenario(new ScriptedAgent().Then("Attack: Volt Tackle"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, captain);
            for (int i = 0; i < 3; i++) e.ScenarioAttach(pikachu, C("MEE-12"));
            var target = e.ScenarioActive(1, C("MEP-91"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, target.DamageCounters);
            Assert.AreEqual(3, pikachu.DamageCounters, "30 to itself");
        }

        /// <summary>v156.4: eight more Simplified Chinese special art and gold prints.</summary>
        [Test]
        public void More_China_Prints()
        {
            Assert.AreEqual("SSP-179", db.MainPrinting(C("CSV9C-257")).Key, "Lisia's Appeal");
            Assert.AreEqual("SVI-32", db.MainPrinting(C("CSV3C-155")).Key, "Arcanine ex");
            Assert.AreEqual("PAR-155", db.MainPrinting(C("CSV4C-158")).Key, "Maushold ex");
            Assert.AreEqual("PAR-172", db.MainPrinting(C("CSV4C-159")).Key, "Rika");
            Assert.AreEqual("PAR-139", db.MainPrinting(C("CSV4C-162")).Key, "Gholdengo ex");
            Assert.AreEqual("PAR-165", db.MainPrinting(C("CSV5C-157")).Key, "Larry");
            Assert.AreEqual("SVI-169", db.MainPrinting(C("CSV5C-164")).Key, "Defiance Band");
            Assert.AreEqual("TWM-145", db.MainPrinting(C("CSV8C-255")).Key, "Carmine");
            Assert.AreEqual("Simplified Chinese CSV8C (coming to China)", C("CSV8C-255").PrintSetName);
            // v156.6: Ponyta's China print (0107/07 in CBB4C) is Destined Rivals' Double Headbutt Ponyta, Standard by its own I mark.
            Assert.AreEqual("DRI-29", db.MainPrinting(C("CBB4C-107")).Key);
            Assert.IsTrue(Formats.Allows(DeckFormat.Standard, C("CBB4C-107")));
            // v156.8: Ciphermaniac's Codebreaking's China SAR.
            Assert.AreEqual("TEF-145", db.MainPrinting(C("CSV7C-245")).Key);
            // v156.10: Iron Hands ex and Pal Pad China prints; the Phantasmal Flames prerelease-stamped Genesect (code PFLSTP, your choice).
            Assert.AreEqual("PRE-31", db.MainPrinting(C("CSV6C-152")).Key, "Iron Hands ex");
            Assert.AreEqual("SSP-86", db.MainPrinting(C("CSV9.5C-234")).Key, "Sylveon ex (v156.15; the code has a dot)");
            Assert.AreEqual("SVI-182", db.MainPrinting(C("CSV6C-163")).Key, "Pal Pad");
            Assert.AreEqual("PFL-8", db.MainPrinting(C("PFLSTP-8")).Key, "Genesect");
            Assert.AreEqual("Phantasmal Flames (prerelease stamp)", C("PFLSTP-8").PrintSetName);
        }
    }
}
