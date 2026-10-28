using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// UI size (v118, for phones and tablets). The whole IMGUI interface is drawn at Factor times its size through GUI.matrix, so
    /// a phone's small, dense screen gets readable text and buttons big enough to tap. Every layout works in GUI units: use
    /// Width and Height instead of Screen.width and Screen.height. Mouse and touch positions inside OnGUI already come in GUI
    /// units; positions given to or taken from a camera are real pixels (ScreenPixels / FromScreenPixels convert).
    ///
    /// Auto picks 1 on a computer and, on a phone or tablet, the size that makes the screen about 720 GUI units tall (never
    /// smaller than 1). The menu's UI size setting (PlayerPrefs) can pick 100% to 250% instead; v118.2 caps it at what the
    /// screen fits (MinWidth x MinHeight), and Ctrl+0 or Pokemon TCG > Reset UI Size in the Editor puts it back to Auto.
    /// </summary>
    public static class UiScale
    {
        public const string PrefUiScale = "PokemonTCG.UiScale";
        /// <summary>Choices for the menu (percent); 0 is Auto.</summary>
        public static readonly int[] Choices = { 0, 100, 125, 150, 175, 200, 250 };

        private static int setting = -1;
        private static float factor = 1f;
        private static int lastW, lastH, lastSetting = -2;

        /// <summary>The saved choice: 0 for Auto, or a percent.</summary>
        public static int Setting
        {
            get
            {
                if (setting < 0) setting = PlayerPrefs.GetInt(PrefUiScale, 0);
                return setting;
            }
            set
            {
                setting = Mathf.Max(0, value);
                PlayerPrefs.SetInt(PrefUiScale, setting);
                PlayerPrefs.Save();
            }
        }

        /// <summary>How many real pixels one GUI unit is.</summary>
        public static float Factor
        {
            get
            {
                if (Screen.width != lastW || Screen.height != lastH || Setting != lastSetting)
                {
                    lastW = Screen.width;
                    lastH = Screen.height;
                    lastSetting = Setting;
                    factor = Compute();
                }
                return factor;
            }
        }

        /// <summary>
        /// v118.2: the smallest screen, in GUI units, every screen is laid out for. A chosen size never makes the screen smaller
        /// than this (200% in a small Game view acts as the biggest size that fits), so the menu's UI size row can't end up off
        /// the screen.
        /// </summary>
        public const float MinWidth = 1000f, MinHeight = 620f;

        /// <summary>The biggest factor that keeps the screen at least MinWidth x MinHeight GUI units (never below 1).</summary>
        public static float Fits
        {
            get { return Mathf.Max(1f, Mathf.Min(Screen.height / MinHeight, Screen.width / MinWidth)); }
        }

        /// <summary>Whether the chosen size is bigger than the screen fits, so a smaller one is used.</summary>
        public static bool Capped
        {
            get { return Setting > 0 && Setting / 100f > Factor + 0.001f; }
        }

        /// <summary>The size in use, in percent.</summary>
        public static int EffectivePercent
        {
            get { return Mathf.RoundToInt(Factor * 100f); }
        }

        private static float Compute()
        {
            int s = Setting;
            if (s > 0) return Mathf.Min(Mathf.Clamp(s / 100f, 0.5f, 4f), Fits);
            if (!Application.isMobilePlatform) return 1f;
            // A phone or tablet: about 720 GUI units tall, and at least 1100 wide when the screen allows it.
            float byHeight = Screen.height / 720f;
            float byWidth = Screen.width / 1100f;
            return Mathf.Max(1f, Mathf.Min(byHeight, byWidth));
        }

        /// <summary>The screen's width in GUI units.</summary>
        public static float Width { get { return Screen.width / Factor; } }

        /// <summary>The screen's height in GUI units.</summary>
        public static float Height { get { return Screen.height / Factor; } }

        /// <summary>Call first thing in OnGUI: everything drawn after it is scaled.</summary>
        public static void Apply()
        {
            float f = Factor;
            GUI.matrix = Mathf.Approximately(f, 1f) ? Matrix4x4.identity : Matrix4x4.Scale(new Vector3(f, f, 1f));
        }

        /// <summary>A GUI position (y down, GUI units) as a camera screen position (y up, real pixels).</summary>
        public static Vector3 ScreenPixels(Vector2 gui)
        {
            float f = Factor;
            return new Vector3(gui.x * f, Screen.height - gui.y * f, 0f);
        }

        /// <summary>A camera screen position (y up, real pixels) as a GUI position (y down, GUI units).</summary>
        public static Vector2 FromScreenPixels(Vector3 screen)
        {
            float f = Factor;
            return new Vector2(screen.x / f, (Screen.height - screen.y) / f);
        }

        /// <summary>
        /// v118.2: keyboard shortcuts that work on every screen: Ctrl+0 puts the UI size back to Auto, Ctrl+minus and Ctrl+plus step
        /// it down and up (Cmd on a Mac). Call at the start of OnGUI, before Apply. Returns true when it changed the size.
        /// </summary>
        public static bool HandleShortcuts(Event ev)
        {
            if (ev == null || ev.type != EventType.KeyDown || !(ev.control || ev.command)) return false;
            int at = System.Array.IndexOf(Choices, Setting);
            if (at < 0) at = 0;
            int pick;
            switch (ev.keyCode)
            {
                case KeyCode.Alpha0:
                case KeyCode.Keypad0:
                    pick = 0;
                    break;
                case KeyCode.Minus:
                case KeyCode.KeypadMinus:
                    pick = Mathf.Max(0, at - 1);
                    break;
                case KeyCode.Equals:
                case KeyCode.Plus:
                case KeyCode.KeypadPlus:
                    pick = Mathf.Min(Choices.Length - 1, at + 1);
                    break;
                default:
                    return false;
            }
            ev.Use();
            if (Choices[pick] == Setting) return false;
            Setting = Choices[pick];
            return true;
        }

        /// <summary>Label for a setting in the menu.</summary>
        public static string Label(int choice)
        {
            return choice <= 0 ? "Auto" : choice + "%";
        }
    }
}
