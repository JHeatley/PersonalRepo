using System;
using System.Collections.Generic;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// v176: the game's language (English or 日本語), chosen on the menu and remembered. Every piece of text the screens draw goes
    /// through T: in Japanese the screens are translated (Resources/Lang/ui_ja.txt), and Pokémon card names, attack names and
    /// Ability names are the official Japanese ones (Resources/Lang/names_ja.txt). Card text, Trainer names, the prompts and the
    /// battle log are still in English. Japanese text is drawn with Noto Sans JP (Resources/Fonts), so it shows on every device.
    /// </summary>
    public static class Loc
    {
        public enum Language { English, Japanese }

        public static readonly string[] Choices = { "English", "日本語" };
        private const string PrefLanguage = "PokemonTCG.Language";

        private static bool loaded;
        private static Language current;
        private static Translator japanese;
        private static Font japaneseFont;
        private static bool fontTried;

        public static Language Current
        {
            get
            {
                Load();
                return current;
            }
        }

        public static bool IsJapanese { get { return Current == Language.Japanese; } }

        public static void Set(Language language)
        {
            Load();
            if (language == current) return;
            current = language;
            PlayerPrefs.SetInt(PrefLanguage, (int)language);
        }

        private static void Load()
        {
            if (loaded) return;
            loaded = true;
            current = (Language)Mathf.Clamp(PlayerPrefs.GetInt(PrefLanguage, 0), 0, Choices.Length - 1);
        }

        /// <summary>The Japanese translator, built the first time it's needed (the card names come from the loaded card pool).</summary>
        private static Translator Japanese
        {
            get
            {
                if (japanese != null) return japanese;
                var t = new Translator();
                try
                {
                    var ui = Resources.Load<TextAsset>("Lang/ui_ja");
                    if (ui != null) t.LoadTemplates(ui.text);
                    var names = Resources.Load<TextAsset>("Lang/names_ja");
                    if (names != null) LoadNames(t, names.text);
                    ApplyPool(t);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                japanese = t;
                return t;
            }
        }

        /// <summary>names_ja.txt (see Translator.LoadNameTable).</summary>
        public static void LoadNames(Translator t, string text) { t.LoadNameTable(text); }

        /// <summary>Tells the translator the Pokémon card names to look for in text (from the card pool, once it's loaded).</summary>
        public static void UseCardPool(CardDatabase db)
        {
            pool = db;
            if (japanese != null) ApplyPool(japanese);
        }

        private static CardDatabase pool;

        private static void ApplyPool(Translator t)
        {
            if (pool == null) return;
            var names = new List<string>();
            foreach (var d in pool.Cards) if (d != null && d.IsPokemon) names.Add(d.Name);
            t.SetKnownCardNames(names);
        }

        // ------------------------------------------------------------------ Text

        public static string T(string text)
        {
            if (text == null || !IsJapanese) return text;
            return Japanese.Translate(text);
        }

        public static GUIContent T(GUIContent content)
        {
            if (content == null || !IsJapanese) return content;
            return new GUIContent(T(content.text), content.image, T(content.tooltip));
        }

        public static Texture T(Texture texture) { return texture; }

        public static string[] T(string[] texts)
        {
            if (texts == null || !IsJapanese) return texts;
            var list = new string[texts.Length];
            for (int i = 0; i < texts.Length; i++) list[i] = T(texts[i]);
            return list;
        }

        public static GUIContent[] T(GUIContent[] contents)
        {
            if (contents == null || !IsJapanese) return contents;
            var list = new GUIContent[contents.Length];
            for (int i = 0; i < contents.Length; i++) list[i] = T(contents[i]);
            return list;
        }

        public static string CardName(string english) { return IsJapanese ? Japanese.CardName(english) : english; }
        public static string AttackName(string english) { return IsJapanese ? Japanese.AttackName(english) : english; }
        public static string AbilityName(string english) { return IsJapanese ? Japanese.AbilityName(english) : english; }

        // ------------------------------------------------------------------ Font

        /// <summary>Sets the font for this frame's drawing: Noto Sans JP in Japanese, Unity's own in English. Call at the start of OnGUI.</summary>
        public static void ApplyFont()
        {
            if (!IsJapanese)
            {
                if (GUI.skin.font == japaneseFont && japaneseFont != null) GUI.skin.font = null;
                return;
            }
            if (!fontTried)
            {
                fontTried = true;
                japaneseFont = Resources.Load<Font>("Fonts/NotoSansJP-Regular");
            }
            if (japaneseFont != null && GUI.skin.font != japaneseFont) GUI.skin.font = japaneseFont;
        }
    }
}
