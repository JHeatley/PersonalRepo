using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>v176: the Japanese language option: screen templates, official Pokémon/attack/Ability names, and the shipped data files.</summary>
    public class TranslatorTests : ScenarioTestBase
    {
        private static Translator Small()
        {
            var t = new Translator();
            t.LoadTemplates("Start game\t対戦開始\nSaved as \"{0}\" in your decks.\t「{0}」としてマイデッキに保存しました。\n" +
                            "Turn {0} - {1} turn\tターン {0} - {1}の番\nYou\tあなた\n{0} of {1} cards\t{1}枚中{0}枚\n");
            t.LoadNames(new Dictionary<string, string> { { "Charizard", "リザードン" }, { "Vileplume", "ラフレシア" }, { "Pikachu", "ピカチュウ" } },
                        new Dictionary<string, string> { { "Dark Charizard", "わるいリザードン" } },
                        new Dictionary<string, string> { { "Fire Spin", "ほのおのうず" }, { "Tackle", "たいあたり" } },
                        new Dictionary<string, string> { { "Teal Dance", "みどりのまい" } },
                        new[] { new KeyValuePair<string, string>("Erika's", "エリカの"), new KeyValuePair<string, string>("Mega", "メガ") });
            t.SetKnownCardNames(new[] { "Charizard ex", "Erika's Vileplume ex", "Mega Charizard X ex", "Pikachu", "Dark Charizard" });
            return t;
        }

        [Test]
        public void Screen_Text_Uses_Exact_Strings_And_Templates()
        {
            var t = Small();
            Assert.AreEqual("対戦開始", t.Translate("Start game"));
            Assert.AreEqual("「Dragapult」としてマイデッキに保存しました。", t.Translate("Saved as \"Dragapult\" in your decks."));
            Assert.AreEqual("ターン 3 - あなたの番", t.Translate("Turn 3 - You turn"), "a placeholder is translated too");
            Assert.AreEqual("60枚中12枚", t.Translate("12 of 60 cards"), "the translation can reorder the placeholders");
            Assert.AreEqual(" 対戦開始", t.Translate(" Start game"), "spaces around the text are kept");
            Assert.AreEqual("Something else", t.Translate("Something else"), "text without a translation stays English");
        }

        [Test]
        public void Pokemon_Names_Are_Swapped_In_English_Text()
        {
            var t = Small();
            Assert.AreEqual("リザードンex", t.CardName("Charizard ex"));
            Assert.AreEqual("エリカのラフレシアex", t.CardName("Erika's Vileplume ex"));
            Assert.AreEqual("メガリザードンXex", t.CardName("Mega Charizard X ex"));
            Assert.AreEqual("わるいリザードン", t.CardName("Dark Charizard"), "an official whole name wins");
            Assert.AreEqual("Choose リザードンex to attack.", t.Translate("Choose Charizard ex to attack."));
            Assert.AreEqual("Your opponent's ピカチュウ is Knocked Out.", t.Translate("Your opponent's Pikachu is Knocked Out."));
        }

        [Test]
        public void Attack_And_Ability_Names_Are_Swapped_Where_Text_Names_Them()
        {
            var t = Small();
            Assert.AreEqual("ほのおのうず", t.AttackName("Fire Spin"));
            Assert.AreEqual("みどりのまい", t.AbilityName("Teal Dance"));
            Assert.AreEqual("Attack: ほのおのうず (100)", t.Translate("Attack: Fire Spin (100)"));
            Assert.AreEqual("リザードン used ほのおのうず.", t.Translate("Charizard used Fire Spin."));
            Assert.AreEqual("Use みどりのまい", t.Translate("Use Teal Dance"));
            Assert.AreEqual("たいあたり", t.Translate("Tackle"), "the whole text");
            Assert.AreEqual("Tackle the problem first", t.Translate("Tackle the problem first"), "a word in a sentence isn't an attack");
        }

        /// <summary>A shipped file in Resources/Lang, found from this test file's own place in the project.</summary>
        private static string Res(string file, [System.Runtime.CompilerServices.CallerFilePath] string here = "")
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(here), "..", "..", "Resources", "Lang", file));
        }

        [Test]
        public void The_Shipped_Japanese_Files_Load_And_Translate()
        {
            var t = new Translator();
            t.LoadTemplates(Res("ui_ja.txt"));
            t.LoadNameTable(Res("names_ja.txt"));
            t.SetKnownCardNames(db.Cards.Where(d => d.IsPokemon).Select(d => d.Name));
            Assert.Greater(t.TemplateCount, 1000);
            Assert.AreEqual("対戦開始", t.Translate("Start game"));
            Assert.AreEqual("リザードンex", t.CardName("Charizard ex"));
            Assert.AreEqual("ファントムダイブ", t.AttackName("Phantom Dive"));
            Assert.AreEqual("10まんボルト", t.AttackName("Thunderbolt"));
            Assert.AreEqual("ウツボット", t.CardName("Victreebel"));
            Assert.AreEqual("ボスの指令", t.TrainerName("Boss's Orders"));
            Assert.AreEqual("ふしぎなアメ", t.TrainerName("Rare Candy"));
            Assert.AreEqual("ラムのみ", t.TrainerName("Lum Berry"), "the games' item name checks the match");
            Assert.AreEqual("基本炎エネルギー", t.TrainerName("Fire Energy"));
            Assert.AreEqual("Play ハイパーボール", t.Translate("Play Ultra Ball"));
            // Effect texts: the official text of the matching Japanese printing.
            Assert.Greater(t.CardTextCount, 3000);
            Assert.AreEqual("相手のポケモン2匹に、それぞれ残りHPが「60」になるように、ダメカンをのせる。",
                            t.CardText("Place damage counters on 2 of your opponent's Pokémon until their remaining HP is 60 each."), "Mega Glimmora ex's Toxic Gem");
            Assert.AreEqual("相手のバトルポケモンがこんらんでないなら、このワザは失敗。", t.CardText("If your opponent's Active Pokémon isn't Confused, this attack does nothing."));
            Assert.AreEqual("相手のバトルポケモンにダメカンがのっているなら、30ダメージ追加。",
                            t.CardText("If your opponent's Active Pokémon already has damage counters on it, it does 30 more damage."), "a mismatched pair is caught");
            Assert.AreEqual("A text nobody has matched yet.", t.CardText("A text nobody has matched yet."));
        }

        [Test]
        public void Trainer_And_Energy_Names_Are_Swapped_Only_Where_Text_Names_A_Card()
        {
            var t = new Translator();
            t.LoadNameTable("trainer\tSwitch\tポケモンいれかえ\ntrainer\tFire Energy\t基本炎エネルギー\ntrainer\tBoss's Orders\tボスの指令\n");
            t.SetKnownCardNames(new string[0]);
            Assert.AreEqual("ポケモンいれかえ", t.Translate("Switch"));
            Assert.AreEqual("Play ポケモンいれかえ", t.Translate("Play Switch"));
            Assert.AreEqual("Switch your Active Pokémon with 1 of your Benched Pokémon.", t.Translate("Switch your Active Pokémon with 1 of your Benched Pokémon."));
            Assert.AreEqual("Attach a Fire Energy to 1 of your Pokémon.", t.Translate("Attach a Fire Energy to 1 of your Pokémon."));
            Assert.AreEqual("You played ボスの指令.", t.Translate("You played Boss's Orders."), "a name of more than one word is swapped anywhere");
        }

        [Test]
        public void Every_Shipped_Template_Keeps_Its_Placeholders()
        {
            foreach (var raw in Res("ui_ja.txt").Split('\n'))
            {
                if (raw.Length == 0 || raw[0] == '#') continue;
                var f = raw.Split('\t');
                Assert.AreEqual(2, f.Length, raw);
                var a = System.Text.RegularExpressions.Regex.Matches(f[0], @"\{\d+\}").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value).OrderBy(x => x).ToList();
                var b = System.Text.RegularExpressions.Regex.Matches(f[1], @"\{\d+\}").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value).OrderBy(x => x).ToList();
                CollectionAssert.AreEqual(a, b, raw);
            }
        }
    }
}
