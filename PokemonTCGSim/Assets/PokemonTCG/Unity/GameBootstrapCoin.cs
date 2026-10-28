using System.Collections.Generic;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Coin flips on screen: calling the opening flip (heads or tails), and an animated coin for every flip the engine makes
    /// (the opening flip, attacks, Abilities, Trainers, Sleep and Burn checks...). The game waits while the coin is in the air;
    /// a click skips to the result.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        // Seconds in the air and seconds showing the result, by pace (Slow, Normal, Fast).
        private static readonly float[] CoinSpin = { 1.4f, 1.0f, 0.65f };
        private static readonly float[] CoinHold = { 1.3f, 0.9f, 0.55f };
        /// <summary>Half turns in the air (odd, so the coin starts on the other face and lands on the result).</summary>
        private const int CoinHalfTurns = 9;

        private float coinStart, coinLands, coinUntil = -1f, lastCoinEnd = -10f;
        private bool coinHeads;
        private string coinCaption, coinSpinCaption;
        private static Texture2D coinHeadsTex, coinTailsTex;
        private GUIStyle coinLetter, coinResult;

        private bool CoinCallVisible
        {
            get { return shownRequest != null && human != null && shownRequest.Kind == DecisionKind.CallCoinFlip; }
        }

        private bool CoinVisible { get { return coinUntil > 0f && Time.time < coinUntil; } }

        /// <summary>Starts the coin animation for a CoinFlip event; returns how long the game should wait for it.</summary>
        private float StartCoin(GameEvent ev)
        {
            float spin = CoinSpin[pace], hold = CoinHold[pace];
            // Several flips in a row ("flip 3 coins", "flip until you get tails") go a little faster after the first.
            if (Time.time - lastCoinEnd < 0.4f)
            {
                spin *= 0.65f;
                hold *= 0.65f;
            }
            string text = ev.Text ?? "";
            if (text.Contains("won the coin flip")) hold += 0.6f; // the opening flip: give it a moment
            coinHeads = ev.Amount == 1;
            coinCaption = Colorize(text);
            coinSpinCaption = Colorize(SpinCaption(text));
            coinStart = Time.time;
            coinLands = coinStart + spin;
            coinUntil = coinLands + hold;
            lastCoinEnd = coinUntil;
            return spin + hold;
        }

        /// <summary>The caption while the coin is in the air: the event text without the result.</summary>
        private static string SpinCaption(string text)
        {
            if (text.EndsWith(": heads.") || text.EndsWith(": tails.")) return text.Substring(0, text.Length - 8);
            int it = text.IndexOf(" It's ");
            if (it > 0) return text.Substring(0, it);
            return "Flipping a coin...";
        }

        private void DrawCoin(UiStyles ui)
        {
            if (!CoinVisible) return;
            EnsureCoinArt();
            float bandH = Mathf.Min(UiScale.Height * 0.6f, 460f);
            var band = new Rect(0f, (UiScale.Height - bandH) / 2f, UiScale.Width, bandH);
            CardDraw.Fill(band, new Color(0f, 0f, 0f, 0.62f));

            float now = Time.time;
            bool landed = now >= coinLands;
            GUI.Label(new Rect(band.x + 30f, band.y + 8f, band.width - 60f, 56f), landed ? coinCaption : coinSpinCaption, CardDraw.BigCaption());

            float size = Mathf.Min(170f, bandH * 0.42f);
            var center = new Vector2(UiScale.Width / 2f, band.y + bandH * 0.5f);
            float scaleX = 1f;
            bool face = coinHeads;
            if (!landed)
            {
                float p = Mathf.Clamp01((now - coinStart) / Mathf.Max(0.01f, coinLands - coinStart));
                float eased = 1f - (1f - p) * (1f - p);
                float angle = eased * CoinHalfTurns * Mathf.PI;
                scaleX = Mathf.Max(0.04f, Mathf.Abs(Mathf.Cos(angle)));
                int crossings = Mathf.FloorToInt(angle / Mathf.PI + 0.5f);
                bool startFace = !coinHeads; // an odd number of half turns lands on the other face
                face = crossings % 2 == 0 ? startFace : !startFace;
                float arc = Mathf.Sin(p * Mathf.PI);
                center.y -= arc * bandH * 0.18f; // tossed up and back down
                size *= 1f + 0.22f * arc;         // nearer the camera at the top
            }

            var rect = new Rect(center.x - size / 2f, center.y - size / 2f, size, size);
            var prev = GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(scaleX, 1f), center);
            GUI.DrawTexture(rect, face ? coinHeadsTex : coinTailsTex);
            coinLetter.fontSize = Mathf.RoundToInt(size * 0.48f);
            coinLetter.normal.textColor = face ? new Color(0.45f, 0.3f, 0.05f) : new Color(0.28f, 0.3f, 0.36f);
            GUI.Label(rect, face ? "H" : "T", coinLetter);
            GUI.matrix = prev;

            if (landed)
            {
                coinResult.normal.textColor = coinHeads ? new Color(1f, 0.85f, 0.35f) : new Color(0.85f, 0.88f, 0.95f);
                GUI.Label(new Rect(band.x, rect.yMax + 8f, band.width, 44f), coinHeads ? "HEADS" : "TAILS", coinResult);
            }
            GUI.Label(new Rect(band.x, band.yMax - 30f, band.width, 24f), "Click to continue", WhiteSmall());

            var e = Event.current;
            if (e.type == EventType.MouseDown && mulliganSeat < 0)
            {
                // First click lands the coin; a click on the result moves on.
                guiActions.Add(() =>
                {
                    if (Time.time < coinLands)
                    {
                        float shift = coinLands - Time.time;
                        coinStart -= shift;
                        coinLands = Time.time;
                        coinUntil = Mathf.Max(Time.time + 0.5f, coinUntil - shift);
                        nextEventTime = coinUntil;
                    }
                    else
                    {
                        coinUntil = -1f;
                        nextEventTime = Time.time;
                    }
                    lastCoinEnd = Time.time;
                    pressing = false;
                    dragging = false;
                });
                e.Use();
            }
        }

        /// <summary>Calling the opening flip.</summary>
        private void DrawCoinCall(UiStyles ui)
        {
            var req = shownRequest;
            if (req == null) return;
            EnsureCoinArt();
            CardDraw.Dim(0.6f);
            float w = Mathf.Min(560f, UiScale.Width - 40f), h = 330f;
            var r = new Rect((UiScale.Width - w) / 2f, (UiScale.Height - h) / 2f, w, h);
            CardDraw.Fill(r, new Color(0.09f, 0.11f, 0.15f, 0.97f));
            GUI.Label(new Rect(r.x + 20f, r.y + 14f, r.width - 40f, 40f), "Call the coin flip", CardDraw.BigCaption());
            GUI.Label(new Rect(r.x + 20f, r.y + 54f, r.width - 40f, 26f), "The winner chooses to go first or second.", WhiteSmall());

            float bw = Mathf.Min(200f, (r.width - 60f) / 2f), bh = 200f;
            float top = r.y + 96f;
            var calls = new[] { "Heads", "Tails" };
            for (int k = 0; k < 2; k++)
            {
                int idx = req.Options.FindIndex(o => o.Label == calls[k]);
                if (idx < 0) continue;
                var b = new Rect(r.center.x + (k == 0 ? -bw - 10f : 10f), top, bw, bh);
                bool clicked = GUI.Button(b, GUIContent.none);
                float coin = bh - 60f;
                var coinRect = new Rect(b.center.x - coin / 2f, b.y + 12f, coin, coin);
                GUI.DrawTexture(coinRect, k == 0 ? coinHeadsTex : coinTailsTex);
                coinLetter.fontSize = Mathf.RoundToInt(coin * 0.48f);
                coinLetter.normal.textColor = k == 0 ? new Color(0.45f, 0.3f, 0.05f) : new Color(0.28f, 0.3f, 0.36f);
                GUI.Label(coinRect, k == 0 ? "H" : "T", coinLetter);
                GUI.Label(new Rect(b.x, coinRect.yMax + 8f, b.width, 30f), calls[k], CardDraw.Banner());
                if (clicked)
                {
                    int pick = idx;
                    guiActions.Add(() => Submit(new List<int> { pick }));
                }
            }
        }

        private void EnsureCoinArt()
        {
            if (coinHeadsTex == null)
                coinHeadsTex = MakeCoin(new Color(0.96f, 0.77f, 0.22f), new Color(0.72f, 0.52f, 0.1f), new Color(1f, 0.95f, 0.65f));
            if (coinTailsTex == null)
                coinTailsTex = MakeCoin(new Color(0.78f, 0.8f, 0.85f), new Color(0.52f, 0.55f, 0.6f), new Color(0.97f, 0.98f, 1f));
            if (coinLetter == null)
                coinLetter = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            if (coinResult == null)
                coinResult = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 32 };
        }

        /// <summary>A round coin with a rim, an inner ring and a little shine (drawn once).</summary>
        private static Texture2D MakeCoin(Color face, Color rim, Color shine)
        {
            const int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            var px = new Color[size * size];
            float c = (size - 1) / 2f;
            float radius = size / 2f - 2f;
            var ring = Color.Lerp(rim, Color.black, 0.25f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - c, dy = y - c;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    Color col;
                    if (d > radius * 0.88f) col = rim;
                    else if (d > radius * 0.8f && d < radius * 0.84f) col = ring;
                    else
                    {
                        float t = Mathf.Clamp01(0.5f + (dy - dx) / (2f * radius)); // lighter towards the top left
                        col = Color.Lerp(face, shine, t * 0.65f);
                    }
                    col.a = Mathf.Clamp01(radius - d + 0.5f); // smooth edge
                    px[y * size + x] = col;
                }
            }
            tex.SetPixels(px);
            tex.Apply();
            return tex;
        }
    }
}
