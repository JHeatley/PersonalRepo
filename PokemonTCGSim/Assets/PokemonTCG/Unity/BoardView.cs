using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Lays out a PlayerView on a 2.5D table. Everything is derived from the snapshot each time,
    /// so the board can never drift out of sync with the rules engine.
    /// The bottom seat's cards use local coordinates; the top seat is the same layout rotated 180°.
    /// </summary>
    public sealed class BoardView : MonoBehaviour
    {
        public int BottomSeat;

        private readonly Dictionary<int, CardView> cards = new Dictionary<int, CardView>();
        private readonly HashSet<int> handIds = new HashSet<int>();
        /// <summary>Attached Energy cards, tucked under their Pokémon (the game screen draws Energy symbols instead).</summary>
        private readonly HashSet<int> tuckedIds = new HashSet<int>();
        /// <summary>Top card of each discard pile -> the seat it belongs to (so a click can open that pile).</summary>
        private readonly Dictionary<int, int> discardTops = new Dictionary<int, int>();
        /// <summary>Top card of each Lost Zone -> the seat it belongs to (a click opens that player's Lost Zone, like a discard pile).</summary>
        private readonly Dictionary<int, int> lostZoneTops = new Dictionary<int, int>();
        /// <summary>Each seat's face-down Prize slots in the last snapshot, to see which Prize cards were taken (null = not known yet).</summary>
        private readonly HashSet<int>[] lastFaceDownPrizes = { null, null };
        private Transform cardRoot;

        // Layout for the bottom seat (x right, z towards the far side), measured from the middle of the field.
        // Side() mirrors it for the top seat and moves the whole field FieldBack away from the viewer; the viewer's own hand
        // stays put, so there's a gap between the hand and the Bench and the Bench's HP / damage badges aren't covered.
        private const float FieldBack = 1.5f;
        /// <summary>Half the playmat's length before the field was moved back (its near edge stays under the viewer's hand).</summary>
        private const float MatHalf = 6.4f;
        private static readonly Vector3 ActivePos = new Vector3(0f, 0.02f, -1.45f);
        // A little in from the edge, so the decision panel on the right doesn't cover the deck and discard pile.
        private static readonly Vector3 DeckPos = new Vector3(7.0f, 0.02f, -3.75f);
        private static readonly Vector3 DiscardPos = new Vector3(7.0f, 0.02f, -1.45f);
        /// <summary>
        /// The Lost Zone (v49): its own pile above the Prize cards, a little smaller than a card. Turned sideways (v69), so it's
        /// clear of both the Prize cards below it and the opponent's discard pile above it.
        /// </summary>
        private static readonly Vector3 LostZonePos = new Vector3(-7.175f, 0.02f, -0.45f);
        private const float LostZoneScale = 0.85f;
        /// <summary>The card that's thrown into the Lost Zone with a spinning animation (Frenzied Escape, v92).</summary>
        private const string ThrownToLostZone = "Imakuni?'s Doduo";
        /// <summary>
        /// The VSTAR and GX markers (v49), small, just left of the discard pile: face up until the player uses their VSTAR
        /// Power or GX attack, then turned over to show the back.
        /// </summary>
        private static readonly Vector3 VstarMarkerPos = new Vector3(5.89f, 0.03f, -1.2f);
        private static readonly Vector3 GxMarkerPos = new Vector3(5.89f, 0.03f, -1.7f);
        private const float MarkerWidth = 0.62f;
        private const float MarkerHeight = 0.4f;
        /// <summary>[seat, 0 = VSTAR / 1 = GX]: the marker, and whether it's turned over.</summary>
        private readonly Transform[,] markers = new Transform[2, 2];
        private readonly bool[,] markerUsed = new bool[2, 2];
        private bool snapMarkers = true;
        /// <summary>The viewer's hand. Not part of the field, so it isn't moved back.</summary>
        private static readonly Vector3 HandCenter = new Vector3(0f, 1.6f, -6.4f);
        /// <summary>The opponent's face-down hand, in field coordinates for their seat (mirrored and moved back like the field).</summary>
        private const float OppHandZ = -6.6f;
        private static readonly Vector3 StadiumPos = new Vector3(-3.6f, 0.02f, 0f);
        private const float BenchZ = -3.75f;
        private const float BenchSpacing = 2.2f;

        public IEnumerable<CardView> Cards { get { return cards.Values; } }

        /// <summary>Where the board camera sits (GameBootstrap puts it here, looking at the field).</summary>
        public static readonly Vector3 ViewPoint = new Vector3(0f, 13.5f, -10.8f);
        /// <summary>
        /// The playmat and the slot outlines lie well below the cards (v46), each moved down along the line of sight and
        /// scaled so it looks exactly as it did at table height. That was meant to fix the playmat covering the opponent's
        /// Pokémon on a friend's laptop, but the real cause was the shader (v54.1: see Resources/Shaders/CardUnlit.shader).
        /// </summary>
        private const float MatY = -0.6f;
        private const float SlotY = -0.4f;
        /// <summary>Height gap between cards stacked on each other (Evolution stages, tucked Energy, Tools, Prize cards).</summary>
        private const float Layer = 0.012f;

        /// <summary>A flat piece meant to lie at (x, atY, z), moved down to y along the line of sight from ViewPoint; scale is how much bigger it must be to look the same.</summary>
        private static Vector3 Sink(Vector3 atTable, float y, out float scale)
        {
            scale = (ViewPoint.y - y) / (ViewPoint.y - atTable.y);
            return ViewPoint + (atTable - ViewPoint) * scale;
        }

        public void Build()
        {
            // The near edge stays under the viewer's hand; the far edge moves back with the field.
            float matScale;
            var matPos = Sink(new Vector3(0f, 0f, FieldBack / 2f), MatY, out matScale);
            var mat = MakeFlatQuad(transform, "Playmat", matPos, new Vector2(19.5f, 2f * MatHalf + FieldBack) * matScale, new Color(0.09f, 0.22f, 0.2f), -2);
            mat.transform.SetAsFirstSibling();
            var slotColor = new Color(0.13f, 0.3f, 0.27f);
            foreach (bool bottom in new[] { true, false })
            {
                MakeSlot(Side(bottom, ActivePos), slotColor);
                for (int i = 0; i < PlayerState.DefaultMaxBench; i++) MakeSlot(Side(bottom, BenchPos(i, PlayerState.DefaultMaxBench)), slotColor);
                MakeSlot(Side(bottom, DeckPos), slotColor);
                MakeSlot(Side(bottom, DiscardPos), slotColor);
                MakeSlot(Side(bottom, LostZonePos), new Color(0.16f, 0.2f, 0.3f), LostZoneScale, true);
                for (int i = 0; i < 6; i++) MakeSlot(Side(bottom, PrizePos(i)) - new Vector3(0f, 0.01f, 0f), new Color(0.11f, 0.26f, 0.24f));
            }
            MakeSlot(Field(StadiumPos), slotColor);
            cardRoot = new GameObject("Cards").transform;
            cardRoot.SetParent(transform, false);
            BuildMarkers();
        }

        private void BuildMarkers()
        {
            var back = Resources.Load<Texture2D>("Markers/marker_back");
            var fronts = new[] { Resources.Load<Texture2D>("Markers/vstar_front"), Resources.Load<Texture2D>("Markers/gx_front") };
            for (int seat = 0; seat < 2; seat++)
            {
                for (int k = 0; k < 2; k++)
                {
                    var root = new GameObject(k == 0 ? "VSTAR marker" : "GX marker").transform;
                    root.SetParent(transform, false);
                    var size = new Vector2(MarkerWidth, MarkerHeight);
                    var front = MakeFlatQuad(root, "Front", new Vector3(0f, 0.003f, 0f), size, Color.white);
                    front.GetComponent<MeshRenderer>().sharedMaterial = CardVisuals.MaterialFor(fronts[k] != null ? (Texture)fronts[k] : CardVisuals.Solid(k == 0 ? new Color(0.85f, 0.75f, 0.4f) : new Color(0.6f, 0.65f, 0.7f)));
                    var backQuad = MakeFlatQuad(root, "Back", new Vector3(0f, -0.003f, 0f), size, Color.white);
                    // Facing down, and the right way up once the marker is turned over (a half turn about its long axis).
                    backQuad.transform.localRotation = Quaternion.Euler(0f, 0f, 180f) * Quaternion.Euler(90f, 0f, 0f);
                    backQuad.GetComponent<MeshRenderer>().sharedMaterial = CardVisuals.MaterialFor(back != null ? (Texture)back : CardVisuals.Solid(new Color(0.08f, 0.08f, 0.08f)));
                    markers[seat, k] = root;
                }
            }
        }

        /// <summary>Places the markers for the seats and turns over the ones that have been used (animated in Update).</summary>
        private void LayoutMarkers(PlayerView v)
        {
            for (int seat = 0; seat < 2; seat++)
            {
                bool bottom = seat == BottomSeat;
                var side = v.Sides[seat];
                for (int k = 0; k < 2; k++)
                {
                    var root = markers[seat, k];
                    if (root == null) continue;
                    root.localPosition = Side(bottom, k == 0 ? VstarMarkerPos : GxMarkerPos);
                    markerUsed[seat, k] = side != null && (k == 0 ? side.VstarUsed : side.GxUsed);
                    if (snapMarkers) root.localRotation = MarkerRotation(seat, k);
                }
            }
            snapMarkers = false;
        }

        private Quaternion MarkerRotation(int seat, int k)
        {
            return Quaternion.Euler(0f, seat == BottomSeat ? 0f : 180f, markerUsed[seat, k] ? 180f : 0f);
        }

        private void Update()
        {
            for (int seat = 0; seat < 2; seat++)
                for (int k = 0; k < 2; k++)
                {
                    var root = markers[seat, k];
                    if (root == null) continue;
                    var target = MarkerRotation(seat, k);
                    if (root.localRotation != target) root.localRotation = Quaternion.RotateTowards(root.localRotation, target, 540f * Time.deltaTime);
                }
        }

        /// <summary>Where a seat's VSTAR (or GX) marker is on screen (GUI coordinates).</summary>
        public Rect MarkerScreenRect(Camera cam, int seat, bool vstar)
        {
            return ScreenRectOf(cam, Side(seat == BottomSeat, vstar ? VstarMarkerPos : GxMarkerPos), MarkerWidth, MarkerHeight);
        }

        private void MakeSlot(Vector3 pos, Color color, float cardScale = 1f, bool sideways = false)
        {
            float scale;
            var sunk = Sink(new Vector3(pos.x, 0.02f, pos.z), SlotY, out scale);
            float w = CardView.Width * cardScale + 0.15f, h = CardView.Height * cardScale + 0.15f;
            MakeFlatQuad(transform, "Slot", sunk, (sideways ? new Vector2(h, w) : new Vector2(w, h)) * scale, color, -1);
        }

        /// <summary>
        /// A flat coloured quad. sortingOrder puts the playmat (-2) and slots (-1) first in the drawing order (v54.1): with the game's
        /// own shader depth already keeps them under the cards, but if a build ever has to fall back to a shader that doesn't
        /// write depth, they still can't be drawn over the cards.
        /// </summary>
        private static GameObject MakeFlatQuad(Transform parent, string name, Vector3 pos, Vector2 size, Color color, int sortingOrder = 0)
        {
            var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            q.name = name;
            var col = q.GetComponent<Collider>();
            if (col != null) Destroy(col);
            q.transform.SetParent(parent, false);
            q.transform.localPosition = pos;
            q.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            q.transform.localScale = new Vector3(size.x, size.y, 1f);
            var mr = q.GetComponent<MeshRenderer>();
            mr.sharedMaterial = CardVisuals.MaterialFor(CardVisuals.Solid(color));
            mr.sortingOrder = sortingOrder;
            return q;
        }

        /// <summary>Bench slot i of n. Up to 5 use the fixed slots; a bigger Bench (Area Zero Underdepths, up to 8) is squeezed in.</summary>
        private static Vector3 BenchPos(int i, int n)
        {
            if (n <= PlayerState.DefaultMaxBench) return new Vector3(-4.4f + BenchSpacing * i, 0.02f, BenchZ);
            float spacing = 10f / (n - 1);
            return new Vector3(-5f + spacing * i, 0.02f, BenchZ);
        }

        /// <summary>The board id of the face-down Prize card in that seat's slot (0-5).</summary>
        public static int PrizeCardId(int seat, int slot)
        {
            return -(300000 + seat * 10 + slot);
        }

        /// <summary>The table slot of a seat's i-th Prize card (its place in the list when the view has no slots).</summary>
        private static int PrizeSlotOf(SideView s, int i)
        {
            return i < s.PrizePositions.Count ? s.PrizePositions[i] : i;
        }

        /// <summary>Where a seat's deck is on screen (GUI coordinates), whether or not it has cards left.</summary>
        public Rect DeckScreenRect(Camera cam, int seat)
        {
            return SlotScreenRect(cam, Side(seat == BottomSeat, DeckPos), 1f);
        }

        /// <summary>Where a seat's Lost Zone is on screen (GUI coordinates), whether or not it has cards in it.</summary>
        public Rect LostZoneScreenRect(Camera cam, int seat)
        {
            // Sideways (v69): as wide as a card is tall.
            return ScreenRectOf(cam, Side(seat == BottomSeat, LostZonePos), CardView.Height * LostZoneScale, CardView.Width * LostZoneScale);
        }

        private Rect SlotScreenRect(Camera cam, Vector3 center, float cardScale)
        {
            return ScreenRectOf(cam, center, CardView.Width * cardScale, CardView.Height * cardScale);
        }

        private Rect ScreenRectOf(Camera cam, Vector3 center, float width, float height)
        {
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            for (int k = 0; k < 4; k++)
            {
                var local = center + new Vector3((k % 2 == 0 ? -0.5f : 0.5f) * width, 0f, (k < 2 ? -0.5f : 0.5f) * height);
                // v118: real pixels to GUI units (UiScale).
                var gp = UiScale.FromScreenPixels(cam.WorldToScreenPoint(transform.TransformPoint(local)));
                minX = Mathf.Min(minX, gp.x);
                maxX = Mathf.Max(maxX, gp.x);
                minY = Mathf.Min(minY, gp.y);
                maxY = Mathf.Max(maxY, gp.y);
            }
            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        /// <summary>Prize slot i: 3 rows of 2, below the Lost Zone (v49: moved down from -1.0 with rows 0.7 apart instead of 0.8).</summary>
        private static Vector3 PrizePos(int i)
        {
            return new Vector3(-7.95f + (i % 2) * 1.55f, 0.02f + i * Layer, -2.35f - (i / 2) * 0.7f);
        }

        /// <summary>A field position for that seat: mirrored for the top seat, then moved back (see FieldBack).</summary>
        private static Vector3 Side(bool bottom, Vector3 local)
        {
            return Field(bottom ? local : new Vector3(-local.x, local.y, -local.z));
        }

        /// <summary>A position on the field (already for the right seat) moved back from the viewer.</summary>
        private static Vector3 Field(Vector3 p)
        {
            return new Vector3(p.x, p.y, p.z + FieldBack);
        }

        /// <summary>Where cards played from a hand fly in from: the viewer's hand, or the opponent's face-down hand.</summary>
        private static Vector3 HandSpawn(bool bottom)
        {
            return bottom ? HandCenter : Side(false, new Vector3(0f, 0.3f, OppHandZ));
        }

        private static Quaternion Rot(bool bottom, bool faceUp, float tilt = 0f)
        {
            return Quaternion.Euler(tilt, bottom ? 0f : 180f, faceUp ? 0f : 180f);
        }

        /// <summary>Face up and turned a quarter turn, its top towards the middle of the table (the Lost Zone, v69).</summary>
        private static Quaternion SidewaysRot(bool bottom)
        {
            return Quaternion.Euler(0f, bottom ? 90f : 270f, 0f);
        }

        public void Clear()
        {
            foreach (var cv in cards.Values) if (cv != null) Destroy(cv.gameObject);
            cards.Clear();
            handIds.Clear();
            lastFaceDownPrizes[0] = null;
            lastFaceDownPrizes[1] = null;
            snapMarkers = true;
        }

        public CardView Get(int id)
        {
            CardView cv;
            return cards.TryGetValue(id, out cv) ? cv : null;
        }

        public bool IsInViewerHand(int id)
        {
            return handIds.Contains(id);
        }

        /// <summary>An attached Energy card hidden under its Pokémon.</summary>
        public bool IsTucked(int id)
        {
            return tuckedIds.Contains(id);
        }

        /// <summary>The seat whose discard pile this card tops, or -1.</summary>
        public int DiscardSeatOf(int id)
        {
            int seat;
            return discardTops.TryGetValue(id, out seat) ? seat : -1;
        }

        /// <summary>The seat whose Lost Zone this card tops, or -1.</summary>
        public int LostZoneSeatOf(int id)
        {
            int seat;
            return lostZoneTops.TryGetValue(id, out seat) ? seat : -1;
        }

        /// <summary>Top edge (GUI y) of the viewer's hand on screen; releasing a dragged card below it puts it back.</summary>
        public float HandTopY(Camera cam)
        {
            float top = float.MaxValue;
            foreach (var id in handIds)
            {
                var cv = Get(id);
                if (cv == null) continue;
                top = Mathf.Min(top, cv.ScreenRect(cam).y);
            }
            return top == float.MaxValue ? UiScale.Height * 0.8f : top;
        }

        // ------------------------------------------------------------------ Snapshot -> layout

        public void Apply(PlayerView v)
        {
            if (v == null) return;
            var used = new HashSet<int>();
            handIds.Clear();
            tuckedIds.Clear();
            discardTops.Clear();
            lostZoneTops.Clear();
            for (int seat = 0; seat < 2; seat++) LayoutSide(v.Sides[seat], seat == BottomSeat, used);
            LayoutMarkers(v);
            if (v.Stadium != null)
            {
                // A two-way Stadium (Parallel City, Reverse Valley, Chaos Tower) is turned towards the player it faces (v97).
                int faces = v.StadiumFacing >= 0 ? v.StadiumFacing : v.StadiumOwner;
                Place(v.Stadium.Id, v.Stadium.Def, Field(StadiumPos), Rot(faces == BottomSeat || faces < 0, true), Vector3.one, used,
                    HandSpawn(v.StadiumOwner == BottomSeat));
            }

            foreach (var id in cards.Keys.ToList())
            {
                if (used.Contains(id)) continue;
                if (cards[id] != null) Destroy(cards[id].gameObject);
                cards.Remove(id);
            }
        }

        private void LayoutSide(SideView s, bool bottom, HashSet<int> used)
        {
            int seat = s.Player;
            Vector3 handSpawn = HandSpawn(bottom);
            Vector3 deckSpawn = Side(bottom, DeckPos);

            // Face-down Prize cards taken since the last snapshot: the new cards in the hand fly in from those Prize slots, not
            // the deck. (A face-up Prize card keeps its id, so it simply moves.)
            var faceDownNow = new HashSet<int>();
            for (int i = 0; i < s.PrizeCount; i++)
                if (i >= s.PrizeSlots.Count || s.PrizeSlots[i] == null) faceDownNow.Add(PrizeSlotOf(s, i));
            var prizeSpawns = new Queue<Vector3>();
            if (seat >= 0 && seat < 2)
            {
                var before = lastFaceDownPrizes[seat];
                if (before != null)
                    foreach (int slot in before.OrderBy(q => q))
                        if (!faceDownNow.Contains(slot)) prizeSpawns.Enqueue(Side(bottom, PrizePos(slot)));
                lastFaceDownPrizes[seat] = faceDownNow;
            }

            // Deck: one face-down card standing in for the whole pile.
            if (s.DeckCount > 0)
                Place(-(200000 + seat), null, Side(bottom, DeckPos + new Vector3(0f, 0.01f + s.DeckCount * 0.002f, 0f)), Rot(bottom, false), Vector3.one, used, deckSpawn);

            // Prizes: each card stays in its own slot as others are taken.
            for (int i = 0; i < s.PrizeCount; i++)
            {
                int slot = PrizeSlotOf(s, i);
                var faceUp = i < s.PrizeSlots.Count ? s.PrizeSlots[i] : null;
                if (faceUp != null) Place(faceUp.Id, faceUp.Def, Side(bottom, PrizePos(slot)), Rot(bottom, true), Vector3.one, used, deckSpawn);
                else Place(PrizeCardId(seat, slot), null, Side(bottom, PrizePos(slot)), Rot(bottom, false), Vector3.one, used, deckSpawn);
            }

            // Discard: top card only.
            if (s.Discard.Count > 0)
            {
                var top = s.Discard[s.Discard.Count - 1];
                Place(top.Id, top.Def, Side(bottom, DiscardPos + new Vector3(0f, 0.01f, 0f)), Rot(bottom, true), Vector3.one, used, handSpawn);
                discardTops[top.Id] = seat;
            }

            // Lost Zone: top card only, face up (anyone can look through it, like a discard pile).
            if (s.LostZone.Count > 0)
            {
                var top = s.LostZone[s.LostZone.Count - 1];
                // Imakuni?'s Doduo spins wildly all the way into the Lost Zone and stops when it gets there (v92, your request):
                // only a card already on the table is thrown (not one drawn fresh when a board is first laid out).
                bool wasOnTable = Get(top.Id) != null;
                Place(top.Id, top.Def, Side(bottom, LostZonePos + new Vector3(0f, 0.01f, 0f)), SidewaysRot(bottom), Vector3.one * LostZoneScale, used, handSpawn);
                if (wasOnTable && top.Def != null && top.Def.Name == ThrownToLostZone)
                {
                    var thrownView = Get(top.Id);
                    if (thrownView != null && !thrownView.InFlight && (thrownView.transform.localPosition - thrownView.TargetPosition).sqrMagnitude > 0.25f)
                        thrownView.Throw();
                }
                lostZoneTops[top.Id] = seat;
            }

            // Hand
            if (bottom && s.Hand.Count > 0)
            {
                int n = s.Hand.Count;
                float spacing = Mathf.Min(1.3f, 10.5f / n);
                for (int i = 0; i < n; i++)
                {
                    var c = s.Hand[i];
                    float x = (i - (n - 1) / 2f) * spacing;
                    var pos = HandCenter + new Vector3(x, i * 0.004f, 0f);
                    Place(c.Id, c.Def, pos, Rot(true, true, -40f), Vector3.one, used, SpawnFor(c.Id, prizeSpawns, deckSpawn));
                    handIds.Add(c.Id);
                }
            }
            else if (!bottom && s.Hand.Count > 0 && s.HandFaceUp)
            {
                // Omanyte's Clairvoyance (v116): the opponent plays with their hand face up. v134.6: only then; a watched game's
                // view lists both hands, but the player at the top keeps theirs face down like in any other game.
                int n = s.Hand.Count;
                float spacing = Mathf.Min(0.9f, 8f / Mathf.Max(1, n));
                for (int i = 0; i < n; i++)
                {
                    var c = s.Hand[i];
                    float x = (i - (n - 1) / 2f) * spacing;
                    var local = new Vector3(x, 0.02f + i * Layer * 0.5f, OppHandZ);
                    Place(c.Id, c.Def, Side(bottom, local), Rot(bottom, true), new Vector3(0.8f, 0.8f, 0.8f), used, SpawnFor(c.Id, prizeSpawns, deckSpawn));
                }
            }
            else if (!bottom || s.Hand.Count == 0)
            {
                int n = bottom ? 0 : s.HandCount;
                float spacing = Mathf.Min(0.9f, 8f / Mathf.Max(1, n));
                for (int i = 0; i < n; i++)
                {
                    float x = (i - (n - 1) / 2f) * spacing;
                    var local = new Vector3(x, 0.02f + i * Layer * 0.5f, OppHandZ);
                    int id = -(100000 + seat * 1000 + i);
                    Place(id, null, Side(bottom, local), Rot(bottom, false), new Vector3(0.8f, 0.8f, 0.8f), used, SpawnFor(id, prizeSpawns, deckSpawn));
                }
            }

            // Pokémon in play
            if (s.Active != null) LayoutPokemon(s.Active, bottom, ActivePos, used, handSpawn);
            for (int i = 0; i < s.Bench.Count; i++) LayoutPokemon(s.Bench[i], bottom, BenchPos(i, s.Bench.Count), used, handSpawn);
        }

        /// <summary>Where a card that is new to the board appears from: a Prize slot just emptied, else the deck.</summary>
        private Vector3 SpawnFor(int id, Queue<Vector3> prizeSpawns, Vector3 deckSpawn)
        {
            CardView existing;
            bool isNew = !cards.TryGetValue(id, out existing) || existing == null;
            return isNew && prizeSpawns.Count > 0 ? prizeSpawns.Dequeue() : deckSpawn;
        }

        private void LayoutPokemon(PokemonView p, bool bottom, Vector3 slot, HashSet<int> used, Vector3 spawn)
        {
            // Lower evolution stages peek out above the top card.
            int below = p.Stack.Count - 1;
            for (int k = 0; k < below; k++)
            {
                var c = p.Stack[k];
                var pos = slot + new Vector3(0f, -Layer * (below - k), 0.18f * (below - k));
                Place(c.Id, c.Def, Side(bottom, pos), Rot(bottom, true), Vector3.one, used, spawn);
            }
            Place(p.Top.Id, p.Top.Def, Side(bottom, slot), Rot(bottom, true), Vector3.one, used, spawn);

            // Attached Energy: the cards are tucked under the Pokémon (so attaching still shows the card flying in), and the
            // game screen draws a row of Energy symbols along its bottom-left edge.
            for (int j = 0; j < p.Energy.Count; j++)
            {
                var e = p.Energy[j];
                var pos = slot + new Vector3(0f, -Layer * (below + 1) - j * 0.001f, 0f);
                Place(e.Id, e.Def, Side(bottom, pos), Rot(bottom, true), new Vector3(0.5f, 0.5f, 0.5f), used, spawn);
                tuckedIds.Add(e.Id);
            }
            // Pokémon Tools on top of the top-left corner, like a sticker; a second one (Rotom ex's Multi Adapter) sits below it.
            for (int j = 0; j < p.Tools.Count; j++)
            {
                var t = p.Tools[j];
                Place(t.Id, t.Def, Side(bottom, slot + new Vector3(-0.42f, Layer + j * 0.002f, 0.42f - j * 0.62f)), Rot(bottom, true), new Vector3(0.42f, 0.42f, 0.42f), used, spawn);
            }
            // v126: Evolution cards attached by Brock's Ninetales's Shapeshift, on the top-right corner.
            for (int j = 0; j < p.Attached.Count; j++)
            {
                var a = p.Attached[j];
                Place(a.Id, a.Def, Side(bottom, slot + new Vector3(0.42f, Layer + j * 0.002f, 0.42f - j * 0.62f)), Rot(bottom, true), new Vector3(0.42f, 0.42f, 0.42f), used, spawn);
            }
        }

        private void Place(int id, CardDef def, Vector3 pos, Quaternion rot, Vector3 scale, HashSet<int> used, Vector3 spawnFrom)
        {
            CardView cv;
            if (!cards.TryGetValue(id, out cv) || cv == null)
            {
                cv = CardView.Create(cardRoot, id, def);
                cv.TargetPosition = pos;
                cv.TargetRotation = rot;
                cv.TargetScale = scale;
                cv.transform.localPosition = spawnFrom;
                cv.transform.localRotation = rot;
                cv.transform.localScale = scale;
                cards[id] = cv;
            }
            else if (def != null && cv.Def != def)
            {
                cv.SetDef(def);
            }
            cv.TargetPosition = pos;
            cv.TargetRotation = rot;
            cv.TargetScale = scale;
            used.Add(id);
        }

        // ------------------------------------------------------------------ Picking

        public CardView Pick(Camera cam, Vector2 guiPosition)
        {
            if (cam == null) return null;
            var screen = UiScale.ScreenPixels(guiPosition); // v118: GUI units to real pixels
            var ray = cam.ScreenPointToRay(screen);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 200f)) return null;
            return hit.collider.GetComponentInParent<CardView>();
        }

        /// <summary>Lifts the hovered hand card; the card being dragged out of the hand shrinks back (its picture follows the mouse).</summary>
        public void SetHovered(CardView hovered, int draggedId = int.MinValue)
        {
            foreach (var cv in cards.Values)
            {
                if (cv == null) continue;
                cv.Liftable = handIds.Contains(cv.Id);
                cv.Hovered = cv == hovered;
                cv.Dragged = cv.Id == draggedId;
            }
        }
    }
}
