using NUnit.Framework;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>v47: card image files named with just the card number take the set of the folder they're in.</summary>
    public class ImageFolderTests : ScenarioTestBase
    {
        [Test]
        public void A_Number_Only_File_Takes_The_Set_Of_Its_Folder()
        {
            Assert.IsNotNull(db, "loading the cards also teaches the image folders the set codes and names");
            Assert.AreEqual("BS-4", CardVisuals.KeyFor("4", new[] { "BS" }));
            Assert.AreEqual("BS-4", CardVisuals.KeyFor("004 Charizard", new[] { "Base Set" }));
            Assert.AreEqual("BS-4", CardVisuals.KeyFor("4_hires", new[] { "base-set" }));
            Assert.AreEqual("BS-4", CardVisuals.KeyFor("4", new[] { "Base Set (BS)" }));
            Assert.AreEqual("BS-4", CardVisuals.KeyFor("4", new[] { "BS - Base Set" }));
            Assert.AreEqual("BS2-4", CardVisuals.KeyFor("4", new[] { "Base Set 2" }));
            Assert.AreEqual("EVO-12", CardVisuals.KeyFor("12", new[] { "XY Evolutions" }));
            Assert.AreEqual("PGO-10", CardVisuals.KeyFor("10", new[] { "Pokemon GO" }));
            Assert.AreEqual("RS-90", CardVisuals.KeyFor("90", new[] { "Ruby and Sapphire" }));
            Assert.AreEqual("BS-4", CardVisuals.KeyFor("4", new[] { "hires", "Base Set" }), "a folder inside the set's folder is fine");
            // v48: the Wizards Black Star Promos, under the names Limitless and others give them.
            Assert.AreEqual("WP-24", CardVisuals.KeyFor("24", new[] { "WP" }));
            Assert.AreEqual("WP-24", CardVisuals.KeyFor("24", new[] { "WotC Promos" }));
            Assert.AreEqual("WP-24", CardVisuals.KeyFor("24", new[] { "Wizards Black Star Promos" }));
            // v137: the Nintendo Black Star Promos, under Limitless's name and the full one.
            Assert.AreEqual("NP-12", CardVisuals.KeyFor("12", new[] { "Nintendo Promos" }));
            Assert.AreEqual("NP-12", CardVisuals.KeyFor("12", new[] { "Nintendo Black Star Promos" }));
            Assert.AreEqual("NP-12", CardVisuals.KeyFor("12", new[] { "NP" }));
            // v138: Neo Genesis.
            Assert.AreEqual("N1-12", CardVisuals.KeyFor("12", new[] { "Neo Genesis" }));
            // v139: Neo Discovery.
            Assert.AreEqual("N2-12", CardVisuals.KeyFor("12", new[] { "Neo Discovery" }));
            // v140: Southern Islands.
            Assert.AreEqual("SI-12", CardVisuals.KeyFor("12", new[] { "Southern Islands" }));
            // v141: Neo Revelation.
            Assert.AreEqual("N3-12", CardVisuals.KeyFor("12", new[] { "Neo Revelation" }));
            // v142: Neo Destiny.
            Assert.AreEqual("N4-12", CardVisuals.KeyFor("12", new[] { "Neo Destiny" }));
            // v143: Legendary Collection (reprints: the key is the LC print).
            Assert.AreEqual("LC-12", CardVisuals.KeyFor("12", new[] { "Legendary Collection" }));
            // v144: Expedition Base Set.
            Assert.AreEqual("E1-12", CardVisuals.KeyFor("12", new[] { "Expedition Base Set" }));
            // v145: Best of Game.
            Assert.AreEqual("BG-4", CardVisuals.KeyFor("4", new[] { "Best of Game" }));
            // v146: Aquapolis, holo prints numbered H1-H32 included.
            Assert.AreEqual("E2-12", CardVisuals.KeyFor("12", new[] { "Aquapolis" }));
            Assert.AreEqual("E2-H7", CardVisuals.KeyFor("H07", new[] { "Aquapolis" }));
            // v147: Skyridge.
            Assert.AreEqual("E3-12", CardVisuals.KeyFor("12", new[] { "Skyridge" }));
            Assert.AreEqual("E3-H30", CardVisuals.KeyFor("H30", new[] { "Skyridge" }));
            // v148: Pokémon Rumble.
            Assert.AreEqual("RM-12", CardVisuals.KeyFor("12", new[] { "Pokémon Rumble" }));
            // v149: Ruby & Sapphire.
            Assert.AreEqual("RS-12", CardVisuals.KeyFor("12", new[] { "Ruby & Sapphire" }));
            // v150: the China prints (Simplified Chinese SARs of Hilda and N's Plan).
            Assert.AreEqual("CSV10.6-173", CardVisuals.KeyFor("173", new[] { "CSV10.6" }));
            Assert.AreEqual("CSV10.5-173", CardVisuals.KeyFor("173", new[] { "CSV10.5" }));
            Assert.AreEqual("CBB7-307", CardVisuals.KeyFor("0307", new[] { "CBB7" }));
            // v151: the Japanese M-P promos.
            Assert.AreEqual("MP-169", CardVisuals.KeyFor("169", new[] { "MP" }));
            // v152.3: the stamped prints go in a "League Stamps" folder.
            Assert.AreEqual("TOT-51", CardVisuals.KeyFor("51", new[] { "League Stamps" }));
            // v153: PokéBeach's file names (set, language, number), saved as AVIF.
            Assert.AreEqual("MEP-111", CardVisuals.KeyFor("MEP_EN_111", new string[0]));
            Assert.AreEqual("DLR-84", CardVisuals.KeyFor("DLR_EN_084", new[] { "Delta Reign" }));
            Assert.IsTrue(ImageConverter.IsConvertible(".avif") && ImageConverter.IsConvertible(".WEBP"));
            Assert.IsFalse(ImageConverter.IsConvertible(".png"));
            // v154: the Japanese sets by code or by name.
            Assert.AreEqual("M6-62", CardVisuals.KeyFor("62", new[] { "M6" }));
            Assert.AreEqual("M6-62", CardVisuals.KeyFor("062", new[] { "Storm Emeralda" }));
            Assert.AreEqual("MEZ-7", CardVisuals.KeyFor("7", new[] { "MEZ" }));
            // v153.1: an AVIF or WebP picture renamed to .jpg is told by its first bytes.
            Assert.AreEqual(".avif", ImageConverter.RealFormat(new byte[] { 0, 0, 0, 28, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'a', (byte)'v', (byte)'i', (byte)'f' }));
            Assert.AreEqual(".webp", ImageConverter.RealFormat(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 1, 2, 3, 4, (byte)'W', (byte)'E', (byte)'B', (byte)'P' }));
            Assert.IsNull(ImageConverter.RealFormat(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 16, (byte)'J', (byte)'F', (byte)'I', (byte)'F', 0, 1 }), "a real JPG");
            Assert.IsNull(ImageConverter.RealFormat(new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G' }), "too short to tell");
            // v49: Crown Zenith's Galarian Gallery numbers, with or without a leading zero.
            Assert.AreEqual("CRZ-GG1", CardVisuals.KeyFor("GG01", new[] { "Crown Zenith" }));
            Assert.AreEqual("CRZ-GG13", CardVisuals.KeyFor("GG13 Diancie", new[] { "CRZ" }));
            Assert.AreEqual("CRZ-GG5", CardVisuals.KeyFromFileName("CRZ_GG05"));
            // v50: Silver Tempest's Trainer Gallery numbers.
            Assert.AreEqual("SIT-TG5", CardVisuals.KeyFor("TG05", new[] { "Silver Tempest" }));
            Assert.AreEqual("SIT-TG22", CardVisuals.KeyFromFileName("SIT_TG22"));
            // v51: Lost Origin's Trainer Gallery.
            Assert.AreEqual("LOR-TG3", CardVisuals.KeyFor("TG03", new[] { "Lost Origin" }));
            Assert.AreEqual("LOR-TG22", CardVisuals.KeyFromFileName("LOR_TG22"));
            // v53: Astral Radiance's Trainer Gallery.
            Assert.AreEqual("ASR-TG5", CardVisuals.KeyFor("TG05", new[] { "Astral Radiance" }));
            Assert.AreEqual("ASR-TG22", CardVisuals.KeyFromFileName("ASR_TG22"));
            // v54: Brilliant Stars' Trainer Gallery.
            Assert.AreEqual("BRS-TG5", CardVisuals.KeyFor("TG05", new[] { "Brilliant Stars" }));
            Assert.AreEqual("BRS-TG22", CardVisuals.KeyFromFileName("BRS_TG22"));
            // v55: Fusion Strike's secret rares.
            Assert.AreEqual("FST-244", CardVisuals.KeyFor("244", new[] { "Fusion Strike" }));
            Assert.AreEqual("FST-270", CardVisuals.KeyFromFileName("FST_270"));
            // v56: Celebrations' Classic Collection.
            Assert.AreEqual("CEL-CC5", CardVisuals.KeyFor("CC05", new[] { "Celebrations" }));
            Assert.AreEqual("CEL-CC5", CardVisuals.KeyFor("CC05", new[] { "Celebrations Classic Collection" }));
            Assert.AreEqual("CEL-CC24", CardVisuals.KeyFromFileName("CEL_CC24"));
        }

        [Test]
        public void A_File_Named_With_A_Set_Code_Keeps_Its_Own_Set()
        {
            Assert.AreEqual("MEE-13", CardVisuals.KeyFor("MEE_013", new[] { "Base Set" }));
            Assert.AreEqual("TWM-130", CardVisuals.KeyFor("TWM 130 Dragapult ex", new string[0]));
            Assert.AreEqual("TWM-200", CardVisuals.KeyFor("TWM_200_R_EN_LG", new[] { "Twilight Masquerade" }));
            Assert.IsNull(CardVisuals.KeyFor("4", new string[0]), "just a number, outside any set's folder");
            Assert.IsNull(CardVisuals.KeyFor("4", new[] { "My Pictures" }));
        }

        /// <summary>v69: images saved from pkmncards.com under the site's own names, in any folder.</summary>
        [Test]
        public void A_File_Saved_From_Pkmncards_Keeps_The_Sites_Name()
        {
            Assert.IsNotNull(db, "the older names need the pool's set names");
            // The two the person saved: a thumbnail's size on the end, in whatever folder.
            Assert.AreEqual("SP-289", CardVisuals.KeyFor("en_US-Promo_SWSH-SWSH289_morpeko_v_union-312x435", new[] { "My Pictures" }));
            Assert.AreEqual("SP-299", CardVisuals.KeyFor("swshbsp_en_299_std-312x436", new string[0]));
            Assert.AreEqual("SP-299", CardVisuals.KeyFor("swshbsp_en_299_std-312x436", new[] { "Base Set" }), "the name beats the folder");
            // TPCi's style: Scarlet & Violet on, the newer promos, the Energy.
            Assert.AreEqual("TWM-1", CardVisuals.KeyFromPkmncardsName("sv6_en_001_std"));
            Assert.AreEqual("SVI-1", CardVisuals.KeyFromPkmncardsName("sv1_en_001-1"));
            Assert.AreEqual("SVI-256", CardVisuals.KeyFromPkmncardsName("sv1_en_256"));
            Assert.AreEqual("MEW-205", CardVisuals.KeyFromPkmncardsName("sv3-5_en_205_std"));
            Assert.AreEqual("BLK-1", CardVisuals.KeyFromPkmncardsName("zsv10-5_en_001_std-scaled"));
            Assert.AreEqual("WHT-173", CardVisuals.KeyFromPkmncardsName("rsv10-5_en_173_std"));
            Assert.AreEqual("MEG-188", CardVisuals.KeyFromPkmncardsName("me1_en_188_std"));
            Assert.AreEqual("30C-1", CardVisuals.KeyFromPkmncardsName("me5-5_en_001_std"));
            Assert.AreEqual("30C-CC27", CardVisuals.KeyFromPkmncardsName("me5-5c_en_027_std"), "the Classic Collection");
            Assert.AreEqual("MEP-91", CardVisuals.KeyFromPkmncardsName("mebsp_en_091_std"));
            Assert.AreEqual("MEE-16", CardVisuals.KeyFromPkmncardsName("mee_en_016_std"));
            Assert.AreEqual("SVE-1", CardVisuals.KeyFromPkmncardsName("sve_en_001"));
            Assert.AreEqual("SVP-176", CardVisuals.KeyFromPkmncardsName("svbsp_en_176_std"));
            // The "en_US" style: Black & White to Sword & Shield, and the earlier promos.
            Assert.AreEqual("BRS-1", CardVisuals.KeyFromPkmncardsName("en_US-SWSH9-001-exeggcute-1"));
            Assert.AreEqual("BRS-TG26", CardVisuals.KeyFromPkmncardsName("en_US-SWSH9-TG026-gloria"));
            Assert.AreEqual("CRZ-GG65", CardVisuals.KeyFromPkmncardsName("en_US-CZ-GG065-raihan"));
            Assert.AreEqual("SHF-SV117", CardVisuals.KeyFromPkmncardsName("en_US-SF-SV117-grimmsnarl_vmax"));
            Assert.AreEqual("CPA-80", CardVisuals.KeyFromPkmncardsName("en_US-CP-080-suspicious_food_tin"));
            Assert.AreEqual("PGO-88", CardVisuals.KeyFromPkmncardsName("en_US-PGO-088-lure_module"));
            Assert.AreEqual("CEC-271", CardVisuals.KeyFromPkmncardsName("en_US-SM12-271-draw_energy"));
            Assert.AreEqual("CEC-143A", CardVisuals.KeyFromPkmncardsName("en_US-SM12-143a-togepi_cleffa_igglybuff_gx-ya"));
            Assert.AreEqual("HIF-1", CardVisuals.KeyFromPkmncardsName("en_US-HF-001-caterpie"));
            Assert.AreEqual("HIF-SV44", CardVisuals.KeyFromPkmncardsName("en_US-SM1-SV44-oranguru-yaa-044ya"), "the Shiny Vault cards are filed under the set the art came from");
            Assert.AreEqual("HIF-SV94", CardVisuals.KeyFromPkmncardsName("en_US-SM2-SV94-tapu_lele_gx-yaa-094ya"));
            Assert.AreEqual("SUM-1", CardVisuals.KeyFromPkmncardsName("en_US-SM1-001-caterpie"));
            Assert.AreEqual("GEN-28A", CardVisuals.KeyFromPkmncardsName("en_US-TwentiethAnn-28a-jolteon_ex-yaa"));
            Assert.AreEqual("EVO-113", CardVisuals.KeyFromPkmncardsName("en_US-XY12-113-here_comes_team_rocket"));
            Assert.AreEqual("LTR-RC25", CardVisuals.KeyFromPkmncardsName("en_US-BW11-RC25-meloetta_ex"));
            Assert.AreEqual("SP-1", CardVisuals.KeyFromPkmncardsName("en_US-Promo_SWSH-SWSH001-grookey"));
            Assert.AreEqual("SVP-1", CardVisuals.KeyFromPkmncardsName("en_US-Promo_SV-SV001-sprigatito"));
            Assert.AreEqual("SVP-4", CardVisuals.KeyFromPkmncardsName("en_US-Promo_SV-004-mimikyu_ex"));
            // v93: the Sun & Moon promos (the person's Ash's Pikachu files), and the XY and Black & White promos.
            Assert.AreEqual("SMP-108", CardVisuals.KeyFor("en_US-Promo_SM-SM108-ashs_pikachu-312x435", new[] { "SM Black Star Promos" }));
            Assert.AreEqual("SMP-113", CardVisuals.KeyFor("en_US-Promo_SM-SM113-ashs_pikachu-312x435", new string[0]));
            Assert.AreEqual("SMP-1", CardVisuals.KeyFromPkmncardsName("en_US-Promo_SM-SM01-rowlet"));
            Assert.AreEqual("XYP-1", CardVisuals.KeyFromPkmncardsName("en_US-Promo_XY-XY01-chespin"));
            Assert.AreEqual("BWP-1", CardVisuals.KeyFromPkmncardsName("en_US-Promo_BW-BW01-snivy"));
            Assert.AreEqual("HSP-1", CardVisuals.KeyFromPkmncardsName("en_US-Promo_HGSS-HGSS01-ho_oh"), "v135: the HGSS promos");
            Assert.AreEqual("DPP-9", CardVisuals.KeyFromPkmncardsName("en_US-Promo_DP-DP09-torterra_lv_x"), "v136: the DP promos");
            Assert.AreEqual("SMP-108", CardVisuals.KeyFor("SM108", new[] { "SMP" }), "a promo number with its prefix, in the set's folder");
            // Celebrations' Classic Collection is numbered by the original cards, four of them 015: the name decides.
            Assert.AreEqual("CEL-1", CardVisuals.KeyFromPkmncardsName("en_US-Ann25th-001-ho_oh"));
            Assert.AreEqual("CEL-CC18", CardVisuals.KeyFromPkmncardsName("en_US-Ann25thR-145-garchomp_c"));
            Assert.AreEqual("CEL-CC6", CardVisuals.KeyFromPkmncardsName("en_US-Ann25thR-015-here_comes_team_rocket"));
            Assert.AreEqual("CEL-CC3", CardVisuals.KeyFromPkmncardsName("en_US-Ann25thR-015-venusaur-1"));
            Assert.AreEqual("CEL-CC8", CardVisuals.KeyFromPkmncardsName("en_US-Ann25thR-024-_s_pikachu"));
            // The Sun & Moon Energy, which the pool letters by type.
            Assert.AreEqual("SUM-G", CardVisuals.KeyFromPkmncardsName("en_US-SM_Energy-001-grass_energy-1"));
            Assert.AreEqual("TEU-M", CardVisuals.KeyFromPkmncardsName("en_US-SM_Energy-017-metal_energy-1"));
            // Fairy Energy: the pool's only one is Sun & Moon's (SUM Y; there's no SUM 172 Fairy Energy), and every Fairy Energy image can be its image.
            Assert.AreEqual("SUM-Y", CardVisuals.KeyFromPkmncardsName("en_US-SM_Energy-009-fairy_energy-1"));
            Assert.AreEqual("SUM-Y", CardVisuals.KeyFromPkmncardsName("en_US-SM_Energy-018-fairy_energy-1"));
            Assert.AreEqual("SUM-Y", CardVisuals.KeyFor("SUM_Y_R_EN", new string[0]));
            Assert.AreEqual("SUM-Y", CardVisuals.FallbackKeyFromPkmncardsName("en_US-XY_Energy-009-fairy_energy"));
            Assert.AreEqual("MEE-9", CardVisuals.FallbackKeyFromPkmncardsName("en_US-SWSH_Energy-001-grass_energy-312x436"));
            Assert.AreEqual("MEE-16", CardVisuals.FallbackKeyFromPkmncardsName("en_US-XY_Energy-008-metal_energy"));
            Assert.IsNull(CardVisuals.FallbackKeyFromPkmncardsName("en_US-SM12-001-venusaur_snivy_gx"), "only for other printings' basic Energy");
            // The older style, with the set's name written out.
            Assert.AreEqual("BS-4", CardVisuals.KeyFromPkmncardsName("charizard-base-set-bs-4"));
            Assert.AreEqual("BS-4", CardVisuals.KeyFromPkmncardsName("charizard-base-set-bs-4-1-312x436"));
            Assert.AreEqual("BS2-1", CardVisuals.KeyFromPkmncardsName("alakazam-base-set-2-b2-1"));
            Assert.AreEqual("DP-129", CardVisuals.KeyFromPkmncardsName("darkness-energy-diamond-pearl-dp-129"));
            Assert.AreEqual("TR-1", CardVisuals.KeyFromPkmncardsName("dark-alakazam-team-rocket-tr-1"));
            Assert.AreEqual("TRR-1", CardVisuals.KeyFromPkmncardsName("azumarill-team-rocket-returns-rr-1"), "the set's name decides, not the code");
            Assert.AreEqual("E1-1", CardVisuals.KeyFromPkmncardsName("alakazam-expedition-ex-1"));
            Assert.AreEqual("WP-7", CardVisuals.KeyFromPkmncardsName("jigglypuff-wizards-black-star-promos-7"));
            Assert.AreEqual("NP-1", CardVisuals.KeyFromPkmncardsName("kyogre-ex-nintendo-black-star-promos-001"));
            Assert.AreEqual("P5-1", CardVisuals.KeyFromPkmncardsName("ho-oh-pop-series-5-1"));
            // Not the site's English names.
            Assert.IsNull(CardVisuals.KeyFromPkmncardsName("pt_BR-SM9-152a-comunicacao_pokemon-yaa"), "a Portuguese scan");
            Assert.IsNull(CardVisuals.KeyFromPkmncardsName("TWM_200_R_EN_LG"));
            Assert.IsNull(CardVisuals.KeyFromPkmncardsName("004 Charizard"));
            Assert.IsNull(CardVisuals.KeyFromPkmncardsName("TWM-130"));
            Assert.AreEqual("TWM-130", CardVisuals.KeyFor("TWM-130", new string[0]), "still read the old way");
        }

        [Test]
        public void Unown_Files_Named_With_A_Letter_Or_Sign_Find_Their_Card()
        {
            Assert.IsNotNull(db, "the letters come from the pool's Unown");
            // v168.1: pkmncards.com's names, with and without the thumbnail size; ? is "question-mark", ! is dropped.
            Assert.AreEqual("UF-X", CardVisuals.KeyFor("unown-unseen-forces-uf-x-312x429", new string[0]));
            Assert.AreEqual("UF-U", CardVisuals.KeyFor("unown-unseen-forces-uf-u", new string[0]));
            Assert.AreEqual("UF-?", CardVisuals.KeyFor("unown-unseen-forces-uf-question-mark", new string[0]));
            Assert.AreEqual("UF-!", CardVisuals.KeyFor("unown-unseen-forces-uf-", new string[0]));
            Assert.AreEqual("UF-!", CardVisuals.KeyFor("unown-unseen-forces-uf--312x429", new string[0]));
            // Just the letter, in a folder named after the set.
            Assert.AreEqual("UF-V", CardVisuals.KeyFor("V", new[] { "UF" }));
            Assert.AreEqual("UF-W", CardVisuals.KeyFor("w", new[] { "Unseen Forces" }));
            Assert.AreEqual("UF-V", CardVisuals.KeyFor("Unown V", new[] { "Unseen Forces" }));
            Assert.AreEqual("UF-!", CardVisuals.KeyFor("!", new[] { "UF" }));
            Assert.AreEqual("UF-?", CardVisuals.KeyFor("question mark", new[] { "UF" }), "Windows doesn't allow ? in a file name");
            Assert.AreEqual("UF-?", CardVisuals.KeyFor("UF_question", new string[0]));
            Assert.AreEqual("UF-117", CardVisuals.KeyFor("117", new[] { "UF" }), "numbered cards as before");
            Assert.IsNull(CardVisuals.KeyFor("V", new[] { "Base Set" }), "only sets with a card numbered that way");
        }
    }
}
