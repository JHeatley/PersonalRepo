using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PokemonTCG
{
    /// <summary>Small helpers for building uGUI from code.</summary>
    public static class Ui
    {
        static Font font;

        public static Font Font
        {
            get
            {
                if (font == null)
                {
                    try { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); }
                    catch (Exception) { font = null; }
                    if (font == null)
                        font = Font.CreateDynamicFontFromOSFont("Arial", 16);
                }
                return font;
            }
        }

        public static readonly Color Background = new Color(0.08f, 0.10f, 0.14f);
        public static readonly Color Panel = new Color(0.13f, 0.16f, 0.22f, 0.95f);
        public static readonly Color PanelLight = new Color(0.20f, 0.24f, 0.32f, 1f);
        public static readonly Color Accent = new Color(1f, 0.80f, 0.20f);
        public static readonly Color ButtonColor = new Color(0.22f, 0.36f, 0.62f);
        public static readonly Color TextColor = new Color(0.93f, 0.94f, 0.96f);
        public static readonly Color Muted = new Color(0.62f, 0.66f, 0.74f);

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        /// <summary>Anchor a rect to a fraction of its parent (x0,y0 bottom-left to x1,y1 top-right).</summary>
        public static RectTransform Place(RectTransform rt, float x0, float y0, float x1, float y1)
        {
            rt.anchorMin = new Vector2(x0, y0);
            rt.anchorMax = new Vector2(x1, y1);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static RectTransform Fill(RectTransform rt, float pad = 0f)
        {
            Place(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
            return rt;
        }

        public static Image Panelled(string name, Transform parent, Color color)
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Text Label(Transform parent, string text, int size, TextAnchor align, Color color,
                                 FontStyle style = FontStyle.Normal)
        {
            var rt = Rect("Text", parent);
            var t = rt.gameObject.AddComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = size;
            t.alignment = align;
            t.color = color;
            t.fontStyle = style;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            t.raycastTarget = false;
            return t;
        }

        public static Button Button(Transform parent, string label, UnityAction onClick, int fontSize = 22)
        {
            var img = Panelled("Button", parent, ButtonColor);
            var b = img.gameObject.AddComponent<Button>();
            var colors = b.colors;
            colors.highlightedColor = new Color(1.25f, 1.25f, 1.25f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
            b.colors = colors;
            b.onClick.AddListener(onClick);
            var t = Label(img.transform, label, fontSize, TextAnchor.MiddleCenter, Color.white);
            Fill(t.rectTransform, 6);
            return b;
        }

        public static void SetButtonText(Button b, string text)
        {
            b.GetComponentInChildren<Text>().text = text;
        }

        public static LayoutElement Size(Component c, float w, float h)
        {
            var le = c.gameObject.GetComponent<LayoutElement>();
            if (le == null)
                le = c.gameObject.AddComponent<LayoutElement>();
            if (w >= 0) le.preferredWidth = w;
            if (h >= 0) le.preferredHeight = h;
            return le;
        }

        public static HorizontalLayoutGroup Row(RectTransform rt, float spacing, TextAnchor align)
        {
            var h = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = spacing;
            h.childAlignment = align;
            h.childControlWidth = true;
            h.childControlHeight = true;
            h.childForceExpandWidth = false;
            h.childForceExpandHeight = false;
            return h;
        }

        public static VerticalLayoutGroup Column(RectTransform rt, float spacing, TextAnchor align)
        {
            var v = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            v.spacing = spacing;
            v.childAlignment = align;
            v.childControlWidth = true;
            v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            return v;
        }

        /// <summary>A scrolling area; add children to the returned content rect.</summary>
        public static RectTransform Scroll(RectTransform area, bool vertical, Color background)
        {
            var bg = area.gameObject.AddComponent<Image>();
            bg.color = background;
            var scroll = area.gameObject.AddComponent<ScrollRect>();
            var viewport = Fill(Rect("Viewport", area), 4);
            viewport.gameObject.AddComponent<RectMask2D>();
            var content = Rect("Content", viewport);
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            if (vertical)
            {
                content.anchorMin = new Vector2(0, 1);
                content.anchorMax = new Vector2(1, 1);
                content.pivot = new Vector2(0.5f, 1);
                content.offsetMin = Vector2.zero;
                content.offsetMax = Vector2.zero;
                Column(content, 6, TextAnchor.UpperCenter);
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else
            {
                content.anchorMin = new Vector2(0, 0);
                content.anchorMax = new Vector2(0, 1);
                content.pivot = new Vector2(0, 0.5f);
                content.offsetMin = Vector2.zero;
                content.offsetMax = Vector2.zero;
                Row(content, 8, TextAnchor.MiddleLeft);
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = !vertical;
            scroll.vertical = vertical;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30;
            return content;
        }

        public static void Clear(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
                UnityEngine.Object.Destroy(t.GetChild(i).gameObject);
        }

        public static Color EnergyColor(string symbol)
        {
            switch (symbol)
            {
                case "G": return new Color(0.35f, 0.75f, 0.30f);
                case "R": return new Color(0.92f, 0.33f, 0.20f);
                case "W": return new Color(0.25f, 0.55f, 0.95f);
                case "L": return new Color(0.98f, 0.85f, 0.20f);
                case "P": return new Color(0.70f, 0.35f, 0.80f);
                case "F": return new Color(0.78f, 0.45f, 0.22f);
                case "D": return new Color(0.25f, 0.30f, 0.38f);
                case "M": return new Color(0.62f, 0.66f, 0.72f);
                case "N": return new Color(0.75f, 0.62f, 0.20f);
                default: return new Color(0.88f, 0.88f, 0.88f);
            }
        }

        public static Color KindColor(CardData c)
        {
            if (c == null) return PanelLight;
            switch (c.kind)
            {
                case "Pokemon": return new Color(0.30f, 0.36f, 0.48f);
                case "Supporter": return new Color(0.70f, 0.42f, 0.20f);
                case "Item": return new Color(0.22f, 0.45f, 0.70f);
                case "Stadium": return new Color(0.25f, 0.55f, 0.35f);
                case "Pokémon Tool": return new Color(0.50f, 0.32f, 0.62f);
                default: return new Color(0.45f, 0.45f, 0.50f);
            }
        }
    }
}
