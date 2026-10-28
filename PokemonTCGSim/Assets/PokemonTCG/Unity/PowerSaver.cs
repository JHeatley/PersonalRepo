using PokemonTCG.AI;
using UnityEngine;
using UnityEngine.Rendering;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// v120: the battery saver, for phones, tablets and laptops. Auto turns it on while the device runs on its battery (not
    /// plugged in); On and Off force it. While it's on:
    ///   - the background AI work (training, the Regional Sim's AI tables, deck trials) plays half the time and rests the
    ///     other half, with at most 2 AI tables at once, so the processor runs cooler and slower;
    ///   - the screen is drawn 30 times a second instead of 60;
    ///   - with AI work running, no game on the board and no touch for a minute on a phone or tablet, the screen goes black (a
    ///     black OLED screen uses almost no power) with one dim line of progress; a tap brings it back. "Dark screen" (Train
    ///     and Regional Sim screens) does it at once, saver on or not.
    /// On a phone or tablet (unless the saver is Off), AI work also pauses while the app is in the background or the screen is
    /// off, and the AI is saved when that happens. The screen is kept on only while AI work runs or a game is on the board;
    /// otherwise the phone's own screen timeout applies.
    /// </summary>
    public static class PowerSaver
    {
        public const string PrefMode = "PokemonTCG.BatterySaver";
        public const int Auto = 0, On = 1, Off = 2;
        public static readonly string[] ModeNames = { "Auto", "On", "Off" };
        /// <summary>Seconds without a touch before the screen goes black (AI work running, saver on, phone or tablet).</summary>
        public const float DarkAfterSeconds = 60f;
        public const double SaverBusyShare = 0.5;
        public const int SaverMaxThreads = 2;

        private static int mode = -1;
        private static float lastInput;
        private static bool dark;
        private static int appliedFrameRate = int.MinValue, appliedInterval = -1, appliedSleep = int.MinValue;
        private static GUIStyle darkStyle;

        /// <summary>The saved choice: Auto, On or Off.</summary>
        public static int Mode
        {
            get
            {
                if (mode < 0) mode = Mathf.Clamp(PlayerPrefs.GetInt(PrefMode, Auto), Auto, Off);
                return mode;
            }
            set
            {
                mode = Mathf.Clamp(value, Auto, Off);
                PlayerPrefs.SetInt(PrefMode, mode);
                PlayerPrefs.Save();
            }
        }

        /// <summary>Running on the battery (not plugged in).</summary>
        public static bool OnBattery
        {
            get { return SystemInfo.batteryStatus == BatteryStatus.Discharging; }
        }

        /// <summary>Whether the saver is on right now.</summary>
        public static bool Active
        {
            get { return Mode == On || (Mode == Auto && OnBattery); }
        }

        /// <summary>The screen is black (AI work keeps going).</summary>
        public static bool Dark
        {
            get { return dark; }
        }

        /// <summary>Makes the screen black now (the Dark screen button).</summary>
        public static void GoDark()
        {
            dark = true;
        }

        /// <summary>The menu's line about the saver.</summary>
        public static string StatusLine()
        {
            string battery = SystemInfo.batteryLevel >= 0f ? Mathf.RoundToInt(SystemInfo.batteryLevel * 100f) + "%" : "";
            string plugged = SystemInfo.batteryStatus == BatteryStatus.Charging || SystemInfo.batteryStatus == BatteryStatus.Full
                             || SystemInfo.batteryStatus == BatteryStatus.NotCharging ? "plugged in" : OnBattery ? "on battery" : "";
            string state = Active ? "On: AI work runs at half speed, 30 frames a second" : "Off: full speed";
            string power = (battery + (battery.Length > 0 && plugged.Length > 0 ? ", " : "") + plugged).Trim();
            return state + (power.Length > 0 ? " (" + power + ")" : "");
        }

        /// <summary>
        /// First thing in OnGUI: notes touches, clicks and keys. A tap on the black screen only wakes it (returns true: the
        /// event is used up).
        /// </summary>
        public static bool NoteInput(Event ev)
        {
            if (ev == null) return false;
            if (ev.type != EventType.MouseDown && ev.type != EventType.KeyDown && ev.type != EventType.ScrollWheel && ev.type != EventType.MouseDrag)
                return false;
            lastInput = Time.unscaledTime;
            if (!dark) return false;
            dark = false;
            ev.Use();
            return true;
        }

        /// <summary>
        /// Every frame (Update). aiWorking: training, AI tables or a deck trial are running. boardBusy: a game (or a video)
        /// is on the board, so the screen stays on even with no touches.
        /// </summary>
        public static void Tick(bool aiWorking, bool boardBusy)
        {
            bool active = Active;
            WorkThrottle.BusyShare = active ? SaverBusyShare : 1.0;
            WorkThrottle.MaxThreads = active ? SaverMaxThreads : 0;

            if (!aiWorking) dark = false;
            else if (active && Application.isMobilePlatform && !boardBusy && !dark && Time.unscaledTime - lastInput > DarkAfterSeconds) dark = true;
            if (lastInput <= 0f) lastInput = Time.unscaledTime;

            int frameRate = dark ? 10 : active ? 30 : Application.isMobilePlatform ? 60 : -1;
            if (frameRate != appliedFrameRate)
            {
                Application.targetFrameRate = frameRate;
                appliedFrameRate = frameRate;
            }
            int interval = dark ? 5 : 1;
            if (interval != appliedInterval)
            {
                OnDemandRendering.renderFrameInterval = interval;
                appliedInterval = interval;
            }
            if (Application.isMobilePlatform)
            {
                int sleep = aiWorking || boardBusy ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
                if (sleep != appliedSleep)
                {
                    Screen.sleepTimeout = sleep;
                    appliedSleep = sleep;
                }
            }
        }

        /// <summary>The app went to the background (or came back) on a phone or tablet: AI work waits meanwhile, unless the saver is Off.</summary>
        public static void AppPaused(bool paused)
        {
            WorkThrottle.Paused = paused && Application.isMobilePlatform && Mode != Off;
            if (!paused) lastInput = Time.unscaledTime;
        }

        /// <summary>Draws the black screen with one dim line (moved now and then so no pixel stays lit for long).</summary>
        public static void DrawDark(string line)
        {
            var prev = GUI.color;
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(0f, 0f, UiScale.Width, UiScale.Height), CardVisuals.Solid(Color.white));
            GUI.color = prev;
            if (darkStyle == null)
            {
                darkStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, wordWrap = true };
                darkStyle.normal.textColor = new Color(0.32f, 0.32f, 0.32f, 1f);
            }
            var style = darkStyle;
            int slot = (int)(Time.unscaledTime / 30f) % 5;
            float y = UiScale.Height * (0.2f + 0.15f * slot);
            GUI.Label(new Rect(20f, y, UiScale.Width - 40f, 60f), (string.IsNullOrEmpty(line) ? "The AI is working" : line) + "\nBattery saver: tap to wake", style);
        }
    }
}
