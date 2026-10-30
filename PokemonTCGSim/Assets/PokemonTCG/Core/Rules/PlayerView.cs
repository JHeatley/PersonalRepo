using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    public sealed class CardRef
    {
        public int Id;
        public CardDef Def;

        public CardRef(CardInstance c)
        {
            Id = c.Id;
            Def = c.Def;
        }
    }

    public sealed class PokemonView
    {
        public int Uid;
        public int Owner;
        public CardRef Top;
        public List<CardRef> Stack = new List<CardRef>();
        public List<CardRef> Energy = new List<CardRef>();
        /// <summary>The first attached Tool (null when none); Tools lists them all (Rotom ex's Multi Adapter allows 2).</summary>
        public CardRef Tool;
        public List<CardRef> Tools = new List<CardRef>();
        /// <summary>v126: other attached cards (the Evolution cards Brock's Ninetales's Shapeshift attaches).</summary>
        public List<CardRef> Attached = new List<CardRef>();
        public int DamageCounters;
        /// <summary>HP after Tools, Stadiums and Abilities (printed HP when built without an engine).</summary>
        public int MaxHp;
        public SpecialCondition Conditions;
        /// <summary>Retreat Cost right now.</summary>
        public int RetreatCost;
        /// <summary>Energy units provided right now (see EnergyMask), one entry per Energy.</summary>
        public List<int> EnergyUnits = new List<int>();
        public EnergyType? Weakness;
        /// <summary>EX-era second Weakness (v158), null for most cards.</summary>
        public EnergyType? Weakness2;
        /// <summary>False when every Ability this Pokémon has is blocked.</summary>
        public bool AbilitiesActive = true;
        /// <summary>Timed effects on this Pokémon, e.g. "Can't retreat (Clutch)".</summary>
        public List<string> Notes = new List<string>();

        public int RemainingHp { get { return MaxHp - DamageCounters * 10; } }

        public PokemonView(PokemonInPlay p, GameEngine engine)
        {
            Uid = p.Uid;
            Owner = p.Owner;
            Top = new CardRef(p.Top);
            foreach (var c in p.Stack) Stack.Add(new CardRef(c));
            foreach (var e in p.Energy) Energy.Add(new CardRef(e));
            Tool = p.Tool == null ? null : new CardRef(p.Tool);
            foreach (var t in p.Tools) Tools.Add(new CardRef(t));
            foreach (var a in p.Attached) Attached.Add(new CardRef(a));
            if (p.Attached.Count > 0) Notes.Add("Shapeshift: treated as " + p.Attached[p.Attached.Count - 1].Def.Name);
            DamageCounters = p.DamageCounters;
            Conditions = p.Conditions;
            // Markers on the Pokémon itself (v48): shown with the other notes.
            int marker;
            if (p.Markers.ContainsKey(PokemonInPlay.LightningRodMarker)) Notes.Add("Lightning Rod marker");
            if (p.Markers.ContainsKey(PokemonInPlay.ImprisonMarker)) Notes.Add("Imprison marker: no Poké-Powers or Poké-Bodies");
            if (p.Markers.ContainsKey(PokemonInPlay.CharMarker)) Notes.Add("Char counter: its owner flips after every turn; tails, 2 damage counters");
            if (p.Markers.TryGetValue(PokemonInPlay.ColoringMarker, out marker)) Notes.Add("Coloring counter: " + (EnergyType)marker + " type");
            if (p.Markers.TryGetValue(PokemonInPlay.ShiftTypeMarker, out marker)) Notes.Add("Shift: " + (EnergyType)marker + " type");
            if (p.Markers.TryGetValue(PokemonInPlay.ConversionWeaknessMarker, out marker)) Notes.Add("Weakness changed to " + (EnergyType)marker + " (Conversion 1)");
            if (p.Markers.TryGetValue(PokemonInPlay.ConversionResistanceMarker, out marker)) Notes.Add("Resistance changed to " + (EnergyType)marker + " (Conversion 2)");
            if (engine != null)
            {
                MaxHp = engine.MaxHp(p);
                RetreatCost = engine.RetreatCostOf(p);
                EnergyUnits = engine.EnergyUnits(p);
                Weakness = engine.WeaknessOf(p);
                Weakness2 = engine.SecondWeaknessOf(p);
                AbilitiesActive = p.Def.Abilities.Count == 0 || engine.HasWorkingAbilityOrPower(p);
            }
            else
            {
                MaxHp = p.PrintedHp;
                RetreatCost = GameEngine.PrintedStatsOf(p).RetreatCost;
                foreach (var e in p.Energy) EnergyUnits.Add(EnergyMask.Of(e.Def.EnergyProvides));
                Weakness = GameEngine.PrintedStatsOf(p).Weakness;
                Weakness2 = GameEngine.PrintedStatsOf(p).SecondWeakness;
            }
        }

        /// <summary>True when the attached Energy can pay this cost.</summary>
        public bool CanPay(IList<EnergyType> cost)
        {
            return EnergyMask.CanPay(EnergyUnits, cost);
        }
    }

    public sealed class SideView
    {
        public int Player;
        /// <summary>Card list for the viewer's own hand; empty for the opponent (use HandCount).</summary>
        public List<CardRef> Hand = new List<CardRef>();
        public int HandCount;
        /// <summary>
        /// v134.6: this hand is face up for everyone (Omanyte's Clairvoyance). A view for a spectator (viewer -1) lists both hands
        /// whether or not they're face up; the board shows a spectator only the bottom player's hand and the face-up ones.
        /// </summary>
        public bool HandFaceUp;
        public int DeckCount;
        public int PrizeCount;
        /// <summary>One entry per Prize card: the card if it's face up (Crescent Purge), otherwise null.</summary>
        public List<CardRef> PrizeSlots = new List<CardRef>();
        /// <summary>Where each Prize card lies on the table (slot 0-5), in the same order as PrizeSlots. Slots don't move as cards are taken.</summary>
        public List<int> PrizePositions = new List<int>();
        public List<CardRef> Discard = new List<CardRef>();
        /// <summary>Cards in the Lost Zone (public).</summary>
        public List<CardRef> LostZone = new List<CardRef>();
        /// <summary>This player has used their VSTAR Power / GX attack this game (the markers are turned over).</summary>
        public bool VstarUsed;
        public bool GxUsed;
        /// <summary>The turn this player can't play Items in (an Item lock such as Budew's Itchy Pollen), or -1.</summary>
        public int ItemLockTurn = -1;
        /// <summary>How many Benched Pokémon this player may have now (5, more or fewer with Stadiums such as Area Zero Underdepths).</summary>
        public int BenchLimit = PlayerState.DefaultMaxBench;
        public PokemonView Active;
        public List<PokemonView> Bench = new List<PokemonView>();
        /// <summary>Hands this player mulliganed (public: both players see them).</summary>
        public List<List<CardRef>> MulliganHands = new List<List<CardRef>>();
    }

    /// <summary>
    /// Everything one player is allowed to know. The UI and every AI only ever see this,
    /// never the GameState, so nothing can peek at hidden cards.
    /// </summary>
    public sealed class PlayerView
    {
        /// <summary>Seat this view belongs to, or -1 for an all-seeing spectator/debug view.</summary>
        public int Viewer;
        public int TurnNumber;
        public int CurrentPlayer;
        public int FirstPlayer;
        public bool InSetup;
        public bool IsOver;
        public int Winner;
        public string EndReason;
        public CardRef Stadium;
        public int StadiumOwner;
        /// <summary>The player a two-way Stadium faces (Parallel City, Reverse Valley, Chaos Tower), or -1 (v97).</summary>
        public int StadiumFacing = -1;
        public SideView[] Sides = new SideView[2];
        /// <summary>Prize cards each player started with (for "Prize cards taken" counts).</summary>
        public int PrizeCountAtStart = 6;

        public SideView Me { get { return Sides[Viewer < 0 ? 0 : Viewer]; } }
        public SideView Opponent { get { return Sides[Viewer < 0 ? 1 : 1 - Viewer]; } }

        public PokemonView FindPokemon(int uid)
        {
            foreach (var s in Sides)
            {
                if (s.Active != null && s.Active.Uid == uid) return s.Active;
                foreach (var b in s.Bench) if (b.Uid == uid) return b;
            }
            return null;
        }

        /// <summary>View with values computed by the engine (HP after Tools, Energy from Special Energy, Retreat Cost...).</summary>
        public static PlayerView Create(GameEngine engine, int viewer)
        {
            return Create(engine.State, viewer, engine);
        }

        public static PlayerView Create(GameState s, int viewer)
        {
            return Create(s, viewer, null);
        }

        private static PlayerView Create(GameState s, int viewer, GameEngine engine)
        {
            var v = new PlayerView();
            v.Viewer = viewer;
            v.TurnNumber = s.TurnNumber;
            v.CurrentPlayer = s.CurrentPlayer;
            v.FirstPlayer = s.FirstPlayer;
            v.InSetup = s.InSetup;
            v.IsOver = s.IsOver;
            v.Winner = s.Winner;
            v.EndReason = s.EndReason;
            v.Stadium = s.Stadium == null ? null : new CardRef(s.Stadium);
            v.StadiumOwner = s.StadiumOwner;
            if (s.Stadium != null)
            {
                var facing = s.Effects.OfType<StadiumFacingEffect>().FirstOrDefault(f => f.StadiumId == s.Stadium.Id);
                if (facing != null) v.StadiumFacing = facing.FacedPlayer;
            }
            v.PrizeCountAtStart = engine == null ? 6 : engine.PrizeCount;
            for (int i = 0; i < 2; i++)
            {
                var p = s.Players[i];
                var side = new SideView();
                side.Player = i;
                bool mine = viewer < 0 || viewer == i;
                side.HandCount = p.Hand.Count;
                // Omanyte's Clairvoyance (v116): the opponent's hand is face up.
                side.HandFaceUp = engine != null && engine.HandFaceUp(i);
                if (mine || side.HandFaceUp) foreach (var c in p.Hand) side.Hand.Add(new CardRef(c));
                side.DeckCount = p.Deck.Count;
                side.PrizeCount = p.Prizes.Count;
                foreach (var c in p.Prizes) side.PrizeSlots.Add(p.FaceUpPrizeIds.Contains(c.Id) ? new CardRef(c) : null);
                side.PrizePositions.AddRange(p.PrizeSlotList());
                foreach (var c in p.Discard) side.Discard.Add(new CardRef(c));
                foreach (var c in p.LostZone) side.LostZone.Add(new CardRef(c));
                side.VstarUsed = p.VstarUsed;
                side.GxUsed = p.GxUsed;
                side.ItemLockTurn = p.ItemLockTurn;
                if (engine != null) side.BenchLimit = engine.MaxBench(i);
                foreach (var hand in p.MulliganHands) side.MulliganHands.Add(hand.Select(c => new CardRef(c)).ToList());
                // During setup both players place Pokémon face down, so the opponent's board is hidden.
                bool boardVisible = mine || !s.InSetup;
                if (boardVisible)
                {
                    side.Active = p.Active == null ? null : new PokemonView(p.Active, engine);
                    foreach (var b in p.Bench) side.Bench.Add(new PokemonView(b, engine));
                    // Lt. Surge's Secret Plan (v126): the owner knows what their face-down card is.
                    if (mine)
                        foreach (var mon in p.InPlay())
                        {
                            CardInstance real;
                            if (!GameEngine.IsFaceDown(mon) || !s.FaceDownCards.TryGetValue(mon.Stack[0].Id, out real)) continue;
                            foreach (var view in side.Bench.Concat(new[] { side.Active }))
                                if (view != null && view.Uid == mon.Uid) view.Notes.Add("Face down: " + real.Def.Name);
                        }
                }
                v.Sides[i] = side;
            }
            foreach (var e in s.Effects)
            {
                if (e.TargetUid < 0) continue;
                var pv = v.FindPokemon(e.TargetUid);
                if (pv != null) pv.Notes.Add(e.Name);
            }
            return v;
        }
    }
}
