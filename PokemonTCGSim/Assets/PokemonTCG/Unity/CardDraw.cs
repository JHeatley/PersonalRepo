using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Drawing card pictures in IMGUI screens (card picker, discard viewer, pop-ups). Images load a few per frame so
    /// opening a 60-card view doesn't stall; cards without an image (yet) get a coloured placeholder with the name.
    /// Call only from OnGUI.
    /// </summary>
    public static class CardDraw
    {
        private const int ImagesPerFrame = 4;
        private static int budgetFrame = -1;
        private static int budget;
        private static GUIStyle nameStyle, smallStyle, panelStyle, panelHot, panelText, bigCaption, banner, iconStyle;

        /// <summary>The card's picture in r (keeps the card's shape). def null = a face-down card.</summary>
        public static void Card(Rect r, CardDef def, float alpha = 1f)
        {
            if (Time.frameCount != budgetFrame)
            {
                budgetFrame = Time.frameCount;
                budget = ImagesPerFrame;
            }
            var prev = GUI.color;
            GUI.color = new Color(prev.r, prev.g, prev.b, prev.a * alpha);
            if (def == null) GUI.DrawTexture(r, CardVisuals.Back(), ScaleMode.ScaleToFit);
            else
            {
                var tex = CardVisuals.TryImage(def, ref budget);
                if (tex != null) GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
                else Placeholder(r, def);
            }
            GUI.color = prev;
        }

        /// <summary>A coloured card-shaped box with the card's name, set and kind.</summary>
        public static void Placeholder(Rect r, CardDef def)
        {
            var color = CardVisuals.TypeColor(def);
            GUI.DrawTexture(r, CardVisuals.Solid(color));
            float inset = Mathf.Max(3f, r.width * 0.04f);
            var inner = new Rect(r.x + inset, r.y + inset, r.width - inset * 2f, r.height - inset * 2f);
            GUI.DrawTexture(inner, CardVisuals.Solid(Color.Lerp(color, Color.white, 0.55f)));
            var ns = NameStyle();
            ns.fontSize = Mathf.Clamp(Mathf.RoundToInt(r.width / 8f), 10, 26);
            GUI.Label(new Rect(inner.x + 2f, inner.y + 4f, inner.width - 4f, inner.height * 0.6f), def.Name, ns);
            var ss = SmallStyle();
            ss.fontSize = Mathf.Clamp(Mathf.RoundToInt(r.width / 11f), 9, 18);
            GUI.Label(new Rect(inner.x + 2f, inner.yMax - inner.height * 0.32f, inner.width - 4f, inner.height * 0.3f),
                def.SetCode + " " + def.Number + "\n" + CardText.Short(def), ss);
        }

        /// <summary>
        /// An attached Energy card as a round symbol: its type's colour and letter ([G], [R]...). Special Energy gets a gold
        /// ring. Draws nothing for null.
        /// </summary>
        public static void EnergyIcon(Rect r, CardDef energy)
        {
            if (energy == null) return;
            var prev = GUI.color;
            var col = CardVisuals.TypeColor(energy);
            GUI.color = new Color(0f, 0f, 0f, 0.75f * prev.a);
            GUI.DrawTexture(new Rect(r.x - 1.5f, r.y - 1.5f, r.width + 3f, r.height + 3f), CardVisuals.Disc());
            var inner = r;
            if (energy.IsSpecialEnergy)
            {
                GUI.color = new Color(1f, 0.82f, 0.25f, prev.a);
                GUI.DrawTexture(r, CardVisuals.Disc());
                float inset = r.width * 0.16f;
                inner = new Rect(r.x + inset, r.y + inset, r.width - 2f * inset, r.height - 2f * inset);
            }
            GUI.color = new Color(col.r, col.g, col.b, prev.a);
            GUI.DrawTexture(inner, CardVisuals.Disc());
            GUI.color = prev;
            if (iconStyle == null)
                iconStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, clipping = TextClipping.Overflow, padding = new RectOffset(0, 0, 0, 0) };
            iconStyle.fontSize = Mathf.Max(8, Mathf.RoundToInt(inner.height * 0.6f));
            iconStyle.normal.textColor = col.grayscale > 0.6f ? Color.black : Color.white;
            GUI.Label(inner, CardVisuals.TypeLetter(energy.EnergyProvides), iconStyle);
        }

        public static void Frame(Rect r, Texture2D tex, float t)
        {
            GUI.DrawTexture(new Rect(r.x - t, r.y - t, r.width + 2 * t, t), tex);
            GUI.DrawTexture(new Rect(r.x - t, r.yMax, r.width + 2 * t, t), tex);
            GUI.DrawTexture(new Rect(r.x - t, r.y, t, r.height), tex);
            GUI.DrawTexture(new Rect(r.xMax, r.y, t, r.height), tex);
        }

        /// <summary>Darkens the whole screen behind a pop-up.</summary>
        public static void Dim(float alpha)
        {
            var prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, alpha);
            GUI.DrawTexture(new Rect(0f, 0f, UiScale.Width, UiScale.Height), CardVisuals.Solid(Color.white));
            GUI.color = prev;
        }

        /// <summary>A filled rectangle.</summary>
        public static void Fill(Rect r, Color color)
        {
            GUI.DrawTexture(r, CardVisuals.Solid(color));
        }

        /// <summary>Size of a card that fits in a box, keeping the 63 x 88 card shape.</summary>
        public static Vector2 FitSize(float maxW, float maxH)
        {
            float h = Mathf.Min(maxH, maxW * 88f / 63f);
            return new Vector2(h * 63f / 88f, h);
        }

        // ------------------------------------------------------------------ Styles

        private static GUIStyle NameStyle()
        {
            if (nameStyle == null)
            {
                nameStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperCenter, wordWrap = true };
                nameStyle.normal.textColor = Color.black;
            }
            return nameStyle;
        }

        private static GUIStyle SmallStyle()
        {
            if (smallStyle == null)
            {
                smallStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerCenter, wordWrap = true };
                smallStyle.normal.textColor = Color.black;
            }
            return smallStyle;
        }

        /// <summary>Box for an Ability or attack in the Pokémon pop-up (not usable right now).</summary>
        public static GUIStyle Panel()
        {
            if (panelStyle == null)
            {
                panelStyle = new GUIStyle(GUI.skin.box) { richText = true, wordWrap = true, alignment = TextAnchor.UpperLeft, fontSize = 14, padding = new RectOffset(10, 10, 8, 8) };
                panelStyle.normal.textColor = new Color(0.85f, 0.85f, 0.88f);
            }
            return panelStyle;
        }

        /// <summary>Same box, highlighted and clickable (the Ability or attack can be used now).</summary>
        public static GUIStyle PanelHot()
        {
            if (panelHot == null)
            {
                panelHot = new GUIStyle(GUI.skin.button) { richText = true, wordWrap = true, alignment = TextAnchor.UpperLeft, fontSize = 14, padding = new RectOffset(10, 10, 8, 8) };
                var bg = CardVisuals.Solid(new Color(0.16f, 0.45f, 0.22f, 1f));
                var bgHover = CardVisuals.Solid(new Color(0.22f, 0.58f, 0.3f, 1f));
                panelHot.normal.background = bg;
                panelHot.hover.background = bgHover;
                panelHot.active.background = bgHover;
                panelHot.normal.textColor = Color.white;
                panelHot.hover.textColor = Color.white;
                panelHot.active.textColor = Color.white;
            }
            return panelHot;
        }

        /// <summary>Rich-text label that wraps (pop-up text).</summary>
        public static GUIStyle PanelText()
        {
            if (panelText == null)
            {
                panelText = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 14 };
                panelText.normal.textColor = new Color(0.92f, 0.92f, 0.95f);
            }
            return panelText;
        }

        /// <summary>Large white centred caption.</summary>
        public static GUIStyle BigCaption()
        {
            if (bigCaption == null)
            {
                bigCaption = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                bigCaption.normal.textColor = Color.white;
            }
            return bigCaption;
        }

        /// <summary>White centred text on a coloured banner.</summary>
        public static GUIStyle Banner()
        {
            if (banner == null)
            {
                banner = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                banner.normal.textColor = Color.white;
            }
            return banner;
        }
    }
}
