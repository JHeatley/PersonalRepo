using System.Collections.Generic;

namespace PokemonTCG.Core
{
    /// <summary>One physical card in a game. Immutable, so cloned game states share instances.</summary>
    public sealed class CardInstance
    {
        public readonly int Id;
        public readonly CardDef Def;
        public readonly int Owner;

        public CardInstance(int id, CardDef def, int owner)
        {
            Id = id;
            Def = def;
            Owner = owner;
        }

        public override string ToString()
        {
            return Def.Name + "#" + Id;
        }
    }

    /// <summary>A Pokémon in play: its evolution stack plus everything attached to it.</summary>
    public sealed class PokemonInPlay
    {
        /// <summary>Stable id for this Pokémon while it stays in play (survives evolving, switching and retreating).</summary>
        public readonly int Uid;
        public readonly int Owner;
        /// <summary>Evolution stack: [0] is the Basic, the last entry is the card you see.</summary>
        public readonly List<CardInstance> Stack = new List<CardInstance>();
        public readonly List<CardInstance> Energy = new List<CardInstance>();
        /// <summary>Attached Pokémon Tools, in the order they were attached. A Pokémon holds 1 unless an effect allows more (Rotom ex's Multi Adapter: 2).</summary>
        public readonly List<CardInstance> Tools = new List<CardInstance>();
        /// <summary>The first attached Tool, or null when it has none. Use Tools (or HasTool / HasToolNamed) when a Pokémon might hold more than one.</summary>
        public CardInstance Tool { get { return Tools.Count > 0 ? Tools[0] : null; } }
        /// <summary>True when this particular Tool card is attached to this Pokémon.</summary>
        public bool HasTool(CardInstance card) { return card != null && Tools.Contains(card); }
        /// <summary>True when a Tool with this name is attached to this Pokémon.</summary>
        public bool HasToolNamed(string name)
        {
            foreach (var t in Tools) if (t.Def.Name == name) return true;
            return false;
        }
        /// <summary>
        /// v126: other cards attached to it that aren't Energy or Tools (Brock's Ninetales's Shapeshift: Evolution cards). They go
        /// wherever its attached cards go when it leaves play (GameEngine.RemoveFromPlay puts them with its Tools).
        /// </summary>
        public readonly List<CardInstance> Attached = new List<CardInstance>();
        public int DamageCounters;
        public SpecialCondition Conditions;
        public int ParalyzedOnTurn;
        /// <summary>Damage counters this Pokémon puts on itself when its Confusion flip is tails (3; Lanturn's Disorienting Flash: 8).</summary>
        public int ConfusionCounters = 3;
        /// <summary>Damage counters Poison puts on it in Pokémon Checkup before other effects (Poison Fang: 2).</summary>
        public int PoisonCounterBase = 1;
        /// <summary>Coins flipped for Asleep in Pokémon Checkup; it wakes up only if all are heads (Altaria's Soothing Lullaby: 2).</summary>
        public int SleepCoins = 1;
        /// <summary>v136: this Asleep Knocks the Pokémon Out if every one of its coins is tails (Darkrai LV.X's Endless Darkness).</summary>
        public bool SleepKnocksOut;
        public int TurnEnteredPlay;
        public int TurnEvolved = -1;
        /// <summary>The last attack this Pokémon used and the turn it used it ("if this Pokémon used X during your last turn"). Set after the attack resolves.</summary>
        public string LastAttackName;
        public int LastAttackTurn = -100;
        /// <summary>Turn this Pokémon last moved from the Bench to the Active Spot.</summary>
        public int MovedToActiveOnTurn = -100;
        /// <summary>
        /// When this Pokémon's Ability locks could last have turned on (GameState.LockSeq order, 0 = during setup): it came into
        /// play, evolved or devolved (PlaySeq), moved between the Active Spot and the Bench (SpotSeq), or got its first Tool
        /// (ToolSeq). Two locks that would each switch the other off: the one on first wins (GameEngine.EarlierLockKeeps).
        /// </summary>
        public int PlaySeq, SpotSeq, ToolSeq;
        /// <summary>Turn this Pokémon last retreated from the Active Spot (Skuntank V's Pursuit Blast, v50).</summary>
        public int RetreatedOnTurn = -100;
        /// <summary>Turn damage was last healed from this Pokémon (Maractus's Lively Needles).</summary>
        public int HealedOnTurn = -100;
        /// <summary>Turn a Pokémon Tool card was last attached to it from its owner's hand (Mienshao's Whip Expert).</summary>
        public int ToolFromHandOnTurn = -100;
        /// <summary>Turn this Pokémon was last damaged by an attack, and how much attack damage it took that turn (Mega Heracross ex's Juggernaut Horn).</summary>
        public int AttackDamageTurn = -100;
        public int AttackDamageThatTurn;
        /// <summary>Turn this Pokémon was last damaged by an attack while it was the Active Pokémon (Sudowoodo's Reply Strongly, v81).</summary>
        public int ActiveAttackDamageTurn = -100;
        /// <summary>Turn on which this Pokémon can't evolve (e.g. after Strange Timepiece).</summary>
        public int CantEvolveOnTurn = -100;
        /// <summary>Set by effects that Knock Out a Pokémon directly ("is Knocked Out").</summary>
        public bool ForcedKnockOut;
        /// <summary>Damaged by an attack during the current action (used for "Knocked Out by damage from an attack").</summary>
        public bool HitByAttack;
        /// <summary>
        /// It was devolved after that attack damage (Golurk V's Rewind Beam): the Pokémon left in play isn't the one that took the
        /// damage, so Vengeful Punch doesn't trigger (Compendium, 2023-11-09).
        /// </summary>
        public bool DevolvedAfterHit;
        /// <summary>
        /// "Its remaining HP becomes 10" (Resolute Heart, Durable Body...): the turn it happened, and the damage counters and
        /// max HP right after. If its max HP then drops that turn with no new damage, it keeps 10 HP (Compendium, 2024-11-07).
        /// </summary>
        public int SavedTo10HpTurn = -100;
        public int SavedTo10HpCounters;
        public int SavedTo10HpMaxHp;
        /// <summary>That attack damage landed while this Pokémon was in the Active Spot. "If this Pokémon is in the Active Spot and is
        /// Knocked Out by damage from an attack" still triggers if the attack then moved it to the Bench (Compendium: Destiny Burst).</summary>
        public bool HitWhileActive;
        /// <summary>Player whose attack last damaged this Pokémon during the current action, or -1.</summary>
        public int HitByPlayer = -1;
        /// <summary>The attacking Pokémon's card for that damage (Briar checks it was a Tera Pokémon).</summary>
        public CardDef HitByAttackerDef;
        /// <summary>Uid of the Pokémon whose attack did that damage ("the Attacking Pokémon"), or -1. It may be on the Bench.</summary>
        public int HitByAttackerUid = -1;
        /// <summary>Ability name -> turn number it was last used on. Handles "Once during your turn".</summary>
        public readonly Dictionary<string, int> AbilityUsedOnTurn = new Dictionary<string, int>();
        /// <summary>Lost Crisis: if this turn's attack damage Knocks this Pokémon Out, it goes to the Lost Zone with everything attached.</summary>
        public int LostZoneKnockOutTurn = -100;
        /// <summary>
        /// Markers and counters other than damage counters (v48): "Lightning Rod" (value 1) from Electabuzz, "Coloring" (value =
        /// the EnergyType it makes the Pokémon) from Smeargle, "Imprison" (value 1) from Gardevoir ex δ (v56). They stay through
        /// Benching and switching and are removed when the Pokémon evolves (or devolves) or leaves play (your rulings).
        /// </summary>
        public readonly Dictionary<string, int> Markers = new Dictionary<string, int>();

        public const string LightningRodMarker = "Lightning Rod";
        public const string ColoringMarker = "Coloring";
        /// <summary>Jungle Venomoth's Shift (v114): its type, until Shift is used again or it leaves play (your ruling).</summary>
        public const string ShiftTypeMarker = "Shift type";
        /// <summary>Gardevoir ex δ's Imprison (v56): the Pokémon can't use Poké-Powers or Poké-Bodies while it has one.</summary>
        public const string ImprisonMarker = "Imprison";
        /// <summary>
        /// Neo Genesis Quilava's Char (v138): after every turn the Pokémon's owner flips a coin; tails, 2 damage counters on it.
        /// "Char counters stay on the Pokémon as long as it's in play": evolving or devolving doesn't remove it.
        /// </summary>
        public const string CharMarker = "Char";

        /// <summary>Evolving or devolving removes the markers, except a Char counter (v138).</summary>
        public void ClearMarkersOnEvolve()
        {
            bool charred = Markers.ContainsKey(CharMarker);
            Markers.Clear();
            if (charred) Markers[CharMarker] = 1;
        }
        /// <summary>
        /// Base Set Porygon (v113): Conversion 1 sets the Pokémon's Weakness, Conversion 2 Porygon's Resistance (value = the
        /// EnergyType). Your ruling: until the Pokémon evolves or leaves play (they stay through Benching).
        /// </summary>
        public const string ConversionWeaknessMarker = "Conversion Weakness";
        public const string ConversionResistanceMarker = "Conversion Resistance";
        /// <summary>
        /// Base Set Pidgeotto's Mirror Move (v113): the turn an opponent's attack last gave this Pokémon Special Conditions, and
        /// which ones (value = SpecialCondition flags).
        /// </summary>
        public const string AttackConditionsTurnMarker = "Attack conditions turn";
        public const string AttackConditionsMarker = "Attack conditions";

        public PokemonInPlay(int uid, int owner, CardInstance basic, int turn)
        {
            Uid = uid;
            Owner = owner;
            Stack.Add(basic);
            TurnEnteredPlay = turn;
        }

        public CardInstance Top { get { return Stack[Stack.Count - 1]; } }
        public CardDef Def { get { return Top.Def; } }
        /// <summary>A Basic Pokémon in play, Antique Fossils included ("play this card as a Basic Pokémon").</summary>
        public bool IsBasic { get { return Def.IsBasicPokemon || Def.IsFossil || Markers.ContainsKey(ConsideredBasicMarker); } }
        /// <summary>v147: an Omanyte or Kabuto put onto the Bench by Underground Lake is considered a Basic Pokémon (until it evolves).</summary>
        public const string ConsideredBasicMarker = "Considered Basic";
        /// <summary>
        /// Its name: the top card's, or (Brock's Ninetales's Shapeshift, v126 your ruling) the newest Evolution card attached to it,
        /// since it's treated as that Pokémon.
        /// </summary>
        public string Name
        {
            get
            {
                for (int i = Attached.Count - 1; i >= 0; i--)
                    if (Attached[i].Def.IsEvolution) return Attached[i].Def.Name;
                return Top.Def.Name;
            }
        }
        /// <summary>Printed HP only. Use GameEngine.MaxHp / RemainingHp for HP after Tools, Stadiums and Abilities.</summary>
        public int PrintedHp { get { return Top.Def.Hp; } }

        public bool Has(SpecialCondition c)
        {
            return (Conditions & c) != 0;
        }

        public int CountEnergy(EnergyType type)
        {
            int n = 0;
            foreach (var e in Energy) if (e.Def.EnergyProvides == type) n++;
            return n;
        }

        /// <summary>Every card that belongs to this Pokémon (stack, energy, tool).</summary>
        public List<CardInstance> AllCards()
        {
            var list = new List<CardInstance>(Stack);
            list.AddRange(Energy);
            list.AddRange(Tools);
            list.AddRange(Attached);
            return list;
        }

        public PokemonInPlay Clone()
        {
            var c = new PokemonInPlay(Uid, Owner, Stack[0], TurnEnteredPlay);
            for (int i = 1; i < Stack.Count; i++) c.Stack.Add(Stack[i]);
            c.Energy.AddRange(Energy);
            c.Tools.AddRange(Tools);
            c.Attached.AddRange(Attached);
            c.DamageCounters = DamageCounters;
            c.Conditions = Conditions;
            c.ParalyzedOnTurn = ParalyzedOnTurn;
            c.ConfusionCounters = ConfusionCounters;
            c.PoisonCounterBase = PoisonCounterBase;
            c.SleepCoins = SleepCoins;
            c.SleepKnocksOut = SleepKnocksOut;
            c.TurnEvolved = TurnEvolved;
            c.LastAttackName = LastAttackName;
            c.LastAttackTurn = LastAttackTurn;
            c.MovedToActiveOnTurn = MovedToActiveOnTurn;
            c.PlaySeq = PlaySeq;
            c.SpotSeq = SpotSeq;
            c.ToolSeq = ToolSeq;
            c.RetreatedOnTurn = RetreatedOnTurn;
            c.HealedOnTurn = HealedOnTurn;
            c.ToolFromHandOnTurn = ToolFromHandOnTurn;
            c.AttackDamageTurn = AttackDamageTurn;
            c.AttackDamageThatTurn = AttackDamageThatTurn;
            c.ActiveAttackDamageTurn = ActiveAttackDamageTurn;
            c.CantEvolveOnTurn = CantEvolveOnTurn;
            c.ForcedKnockOut = ForcedKnockOut;
            c.HitByAttack = HitByAttack;
            c.DevolvedAfterHit = DevolvedAfterHit;
            c.SavedTo10HpTurn = SavedTo10HpTurn;
            c.SavedTo10HpCounters = SavedTo10HpCounters;
            c.SavedTo10HpMaxHp = SavedTo10HpMaxHp;
            c.HitWhileActive = HitWhileActive;
            c.HitByPlayer = HitByPlayer;
            c.HitByAttackerDef = HitByAttackerDef;
            c.HitByAttackerUid = HitByAttackerUid;
            foreach (var kv in AbilityUsedOnTurn) c.AbilityUsedOnTurn[kv.Key] = kv.Value;
            c.LostZoneKnockOutTurn = LostZoneKnockOutTurn;
            foreach (var kv in Markers) c.Markers[kv.Key] = kv.Value;
            return c;
        }
    }

    /// <summary>One Knock Out of a player's Pokémon. Immutable, shared by cloned states.</summary>
    public sealed class KnockOutRecord
    {
        public readonly int Turn;
        /// <summary>The Pokémon's top card when it was Knocked Out.</summary>
        public readonly CardDef Def;
        /// <summary>Knocked Out by damage from an attack (not by damage counters, Poison or "is Knocked Out" effects).</summary>
        public readonly bool ByAttack;

        public KnockOutRecord(int turn, CardDef def, bool byAttack)
        {
            Turn = turn;
            Def = def;
            ByAttack = byAttack;
        }
    }

    /// <summary>
    /// A player's discard pile (v80). A Prism Star (◇) card that would be put in it goes to its owner's Lost Zone instead (Prism
    /// Star rule: "If this card would go to the discard pile, put it in the Lost Zone instead"). The engine's sweep
    /// (GameEngine.SweepPrismStars) catches any card put here some other way.
    /// </summary>
    public sealed class DiscardPile : List<CardInstance>
    {
        private readonly PlayerState owner;

        public DiscardPile(PlayerState owner)
        {
            this.owner = owner;
        }

        public DiscardPile(PlayerState owner, IEnumerable<CardInstance> cards) : base(cards)
        {
            this.owner = owner;
        }

        private bool GoesToLostZone(CardInstance card)
        {
            return owner != null && card != null && card.Def != null && card.Def.IsPrismStar;
        }

        public new void Add(CardInstance card)
        {
            if (GoesToLostZone(card)) owner.LostZone.Add(card);
            else base.Add(card);
        }

        public new void AddRange(IEnumerable<CardInstance> cards)
        {
            if (cards == null) return;
            foreach (var card in new List<CardInstance>(cards)) Add(card);
        }

        public new void Insert(int index, CardInstance card)
        {
            if (GoesToLostZone(card)) owner.LostZone.Add(card);
            else base.Insert(index, card);
        }
    }

    public sealed class PlayerState
    {
        public readonly int Index;
        /// <summary>Index 0 is the top of the deck.</summary>
        public List<CardInstance> Deck = new List<CardInstance>();
        public List<CardInstance> Hand = new List<CardInstance>();
        /// <summary>The discard pile: a Prism Star (◇) card put here goes to the Lost Zone instead (v80).</summary>
        public DiscardPile Discard;
        public List<CardInstance> Prizes = new List<CardInstance>();
        public PokemonInPlay Active;
        public List<PokemonInPlay> Bench = new List<PokemonInPlay>();

        // Per-turn flags (reset at the start of this player's turn).
        public bool SupporterPlayedThisTurn;
        public bool EnergyAttachedThisTurn;
        public bool RetreatedThisTurn;
        public bool StadiumPlayedThisTurn;
        /// <summary>Will (CEC, v68): this turn, the next coin flip for an attack, Ability or Trainer effect is called by the player.</summary>
        public bool WillCoinChoice;
        /// <summary>Misty &amp; Lorelei (CEC, v68): this turn, this player's Water Pokémon can use GX attacks even after the GX attack was used.</summary>
        public bool WaterGxThisTurn;
        /// <summary>Bonnie (FLI, v84): this turn, this player's Zygarde-GX can use its GX attack even after the GX attack was used.</summary>
        public bool ZygardeGxThisTurn;

        /// <summary>Turn number on which this player can't play Items (e.g. Budew's Itchy Pollen). -1 = none.</summary>
        public int ItemLockTurn = -1;
        /// <summary>
        /// This player's latest turn (the current one while it's their turn) and the one before it. Extra turns (Star Chronos,
        /// Yoga Loop) skip a turn number, so "your last turn" and "your opponent's last turn" are looked up here instead of
        /// counting back from the current turn number. NoTurn before a player has had one.
        /// </summary>
        public int LastTurnNumber = NoTurn, PreviousTurnNumber = NoTurn;
        public const int NoTurn = -1000;
        /// <summary>Turn number during which one of this player's Pokémon was last Knocked Out (not counting Pokémon Checkup).</summary>
        public int LastKnockedOutTurn = -100;
        /// <summary>Same, but only Knock Outs by damage from an attack.</summary>
        public int LastKnockedOutByAttackTurn = -100;
        /// <summary>Same, but only this player's Team Rocket's Pokémon.</summary>
        public int LastTeamRocketKnockedOutTurn = -100;
        /// <summary>Turn on which this player can't evolve Pokémon from their hand (e.g. Evolution Jammer).</summary>
        public int EvolveLockTurn = -1;
        /// <summary>Name of the Supporter played this turn, or null.</summary>
        public string SupporterNameThisTurn;
        /// <summary>The Supporter card played from the hand this turn, or null ("if you played an Ancient Supporter card").</summary>
        public CardDef SupporterThisTurn;
        /// <summary>
        /// Lt. Surge's Strategy (v69): how many Supporter cards this player may play this turn (normally 1), how many they have
        /// played, and their names in order (SupporterThisTurn is the last one).
        /// </summary>
        public int SupporterLimit = 1;
        public int SupportersPlayedCount;
        public List<string> SupportersThisTurn = new List<string>();
        /// <summary>
        /// Prize cards added or taken away without being taken (Symbiont-GX, Lightning-GX, Stinger-GX; v69). Prize cards already
        /// taken still count as taken (Compendium: Stinger-GX and Assembly-GX, 2018), so GameEngine.PrizesTaken adds this.
        /// </summary>
        public int PrizeAdjust;
        /// <summary>Legacy Energy's Prize reduction can only happen once per game.</summary>
        public bool LegacyEnergyUsed;
        /// <summary>Turn on which this player last took Prize cards, and how many they took that turn (Settle the Score).</summary>
        public int PrizesTakenTurn = -100;
        public int PrizesTakenThatTurn;
        /// <summary>Prize cards this player had to discard instead of taking (Billowing Smoke, v64): they don't count as taken.</summary>
        public int PrizesDiscarded;
        /// <summary>Effect name -> turn used, for "you can't use more than 1 X each turn" style limits.</summary>
        public Dictionary<string, int> NamedLimitUsedOnTurn = new Dictionary<string, int>();
        public int Mulligans;
        /// <summary>Each hand this player mulliganed, in order (revealed to both players).</summary>
        public List<List<CardInstance>> MulliganHands = new List<List<CardInstance>>();
        /// <summary>This player's Pokémon Knocked Out so far (not counting Pokémon Checkup, which is between turns).</summary>
        public List<KnockOutRecord> KnockOuts = new List<KnockOutRecord>();
        /// <summary>Prize cards turned face up (Crescent Purge); they stay face up while they're Prize cards.</summary>
        public List<int> FaceUpPrizeIds = new List<int>();
        /// <summary>
        /// Where each Prize card lies on the table (card id -> slot 0-5), so the cards stay put as others are taken. Filled
        /// in by PrizeSlotList(); a card new to the Prize cards takes the first free slot.
        /// </summary>
        public Dictionary<int, int> PrizeSlotByCard = new Dictionary<int, int>();
        /// <summary>Cards put in the Lost Zone (older cards: Gengar, Darkrai & Cresselia LEGEND). They're out of the game.</summary>
        public List<CardInstance> LostZone = new List<CardInstance>();
        /// <summary>v122: a hand set aside face down (Tickling Machine); it comes back at the end of the player's turn.</summary>
        public List<CardInstance> SetAsideHand = new List<CardInstance>();
        /// <summary>
        /// v136: the basic Energy cards this player discarded from their Active Pokémon with that Pokémon's own attack, the turn
        /// they did and that Pokémon's Uid (Heatran LV.X's Heat Wave attaches up to 2 of them back at the end of the turn).
        /// </summary>
        public List<CardInstance> OwnAttackDiscardedEnergy = new List<CardInstance>();
        public int OwnAttackDiscardedTurn = -100;
        public int OwnAttackDiscardedFromUid = -1;
        /// <summary>v138: "You can't play any more Trainer cards this turn" (Professor Elm, PokéGear, Time Capsule): that turn's number.</summary>
        public int TrainersLockedTurn = -100;
        /// <summary>A GX attack has been used this game (only 1 per game).</summary>
        public bool GxUsed;
        /// <summary>A VSTAR Power has been used this game (only 1 per game).</summary>
        public bool VstarUsed;
        /// <summary>The Pokémon V-UNION (by name) this player has combined this game: each one only once per game (your ruling, v67).</summary>
        public List<string> VUnionsCombined = new List<string>();
        /// <summary>
        /// This player's last attack (v50): the turn, the attack's name and the attacking Pokémon (its card and Uid), kept even
        /// after that Pokémon leaves play ("if 1 of your Dedenne used Dede-Short during your last turn", Zeraora V's Cross Fist).
        /// </summary>
        public int LastAttackTurn = -100;
        public string LastAttackName;
        public CardDef LastAttackerDef;
        public int LastAttackerUid = -1;
        /// <summary>The card whose attack was used last (the attacker, or the card a copied attack belongs to) and its index (Watch and Learn, v96).</summary>
        public CardDef LastAttackSourceDef;
        public int LastAttackIndex = -1;
        /// <summary>
        /// Today is this player's birthday (_____'s Pikachu's Birthday Surprise). Set when the game starts from the birthday
        /// the person entered on the main menu (GameEngine.Birthdays); a bot never has a birthday (your ruling, v48).
        /// </summary>
        public bool BirthdayToday;

        /// <summary>Normal Bench size. Effects can raise it (Area Zero Underdepths: 8).</summary>
        public const int DefaultMaxBench = 5;
        /// <summary>The largest Bench any effect allows; the board layout has this many slots.</summary>
        public const int MaxBenchLimit = 8;

        public PlayerState(int index)
        {
            Index = index;
            Discard = new DiscardPile(this);
        }

        public IEnumerable<PokemonInPlay> InPlay()
        {
            if (Active != null) yield return Active;
            foreach (var b in Bench) yield return b;
        }

        public bool HasPokemonInPlay { get { return Active != null || Bench.Count > 0; } }

        public void ResetTurnFlags()
        {
            SupporterPlayedThisTurn = false;
            SupporterNameThisTurn = null;
            SupporterThisTurn = null;
            EnergyAttachedThisTurn = false;
            RetreatedThisTurn = false;
            StadiumPlayedThisTurn = false;
            WillCoinChoice = false;
            WaterGxThisTurn = false;
            ZygardeGxThisTurn = false;
            SupporterLimit = 1;
            SupportersPlayedCount = 0;
            SupportersThisTurn.Clear();
        }

        /// <summary>"If you played [Supporter] from your hand during this turn" (any of them, with Lt. Surge's Strategy).</summary>
        public bool PlayedSupporterThisTurn(string name)
        {
            return SupportersThisTurn.Contains(name) || SupporterNameThisTurn == name;
        }

        /// <summary>The table slot of each Prize card, in the order of Prizes. Keeps known slots and gives new cards the first free one.</summary>
        public List<int> PrizeSlotList()
        {
            var present = new HashSet<int>();
            foreach (var c in Prizes) present.Add(c.Id);
            foreach (var id in new List<int>(PrizeSlotByCard.Keys))
                if (!present.Contains(id)) PrizeSlotByCard.Remove(id);
            var taken = new HashSet<int>();
            var slots = new List<int>(Prizes.Count);
            foreach (var c in Prizes)
            {
                int slot;
                if (PrizeSlotByCard.TryGetValue(c.Id, out slot) && !taken.Contains(slot))
                {
                    taken.Add(slot);
                    slots.Add(slot);
                }
                else slots.Add(-1);
            }
            for (int k = 0; k < Prizes.Count; k++)
            {
                if (slots[k] >= 0) continue;
                int free = 0;
                while (taken.Contains(free)) free++;
                taken.Add(free);
                slots[k] = free;
                PrizeSlotByCard[Prizes[k].Id] = free;
            }
            return slots;
        }

        public PlayerState Clone()
        {
            var c = new PlayerState(Index);
            c.Deck = new List<CardInstance>(Deck);
            c.Hand = new List<CardInstance>(Hand);
            c.Discard = new DiscardPile(c, Discard);
            c.Prizes = new List<CardInstance>(Prizes);
            c.Active = Active == null ? null : Active.Clone();
            foreach (var b in Bench) c.Bench.Add(b.Clone());
            c.SupporterPlayedThisTurn = SupporterPlayedThisTurn;
            c.EnergyAttachedThisTurn = EnergyAttachedThisTurn;
            c.RetreatedThisTurn = RetreatedThisTurn;
            c.StadiumPlayedThisTurn = StadiumPlayedThisTurn;
            c.WillCoinChoice = WillCoinChoice;
            c.WaterGxThisTurn = WaterGxThisTurn;
            c.ZygardeGxThisTurn = ZygardeGxThisTurn;
            c.ItemLockTurn = ItemLockTurn;
            c.LastTurnNumber = LastTurnNumber;
            c.PreviousTurnNumber = PreviousTurnNumber;
            c.LastKnockedOutTurn = LastKnockedOutTurn;
            c.LastKnockedOutByAttackTurn = LastKnockedOutByAttackTurn;
            c.LastTeamRocketKnockedOutTurn = LastTeamRocketKnockedOutTurn;
            c.EvolveLockTurn = EvolveLockTurn;
            c.SupporterNameThisTurn = SupporterNameThisTurn;
            c.SupporterThisTurn = SupporterThisTurn;
            c.SupporterLimit = SupporterLimit;
            c.SupportersPlayedCount = SupportersPlayedCount;
            c.SupportersThisTurn = new List<string>(SupportersThisTurn);
            c.PrizeAdjust = PrizeAdjust;
            c.LegacyEnergyUsed = LegacyEnergyUsed;
            c.PrizesTakenTurn = PrizesTakenTurn;
            c.PrizesTakenThatTurn = PrizesTakenThatTurn;
            c.PrizesDiscarded = PrizesDiscarded;
            c.NamedLimitUsedOnTurn = new Dictionary<string, int>(NamedLimitUsedOnTurn);
            c.Mulligans = Mulligans;
            c.MulliganHands = new List<List<CardInstance>>(MulliganHands);
            c.KnockOuts = new List<KnockOutRecord>(KnockOuts);
            c.FaceUpPrizeIds = new List<int>(FaceUpPrizeIds);
            c.PrizeSlotByCard = new Dictionary<int, int>(PrizeSlotByCard);
            c.LostZone = new List<CardInstance>(LostZone);
            c.SetAsideHand = new List<CardInstance>(SetAsideHand);
            c.OwnAttackDiscardedEnergy = new List<CardInstance>(OwnAttackDiscardedEnergy);
            c.OwnAttackDiscardedTurn = OwnAttackDiscardedTurn;
            c.OwnAttackDiscardedFromUid = OwnAttackDiscardedFromUid;
            c.TrainersLockedTurn = TrainersLockedTurn;
            c.GxUsed = GxUsed;
            c.VstarUsed = VstarUsed;
            c.VUnionsCombined = new List<string>(VUnionsCombined);
            c.LastAttackTurn = LastAttackTurn;
            c.LastAttackName = LastAttackName;
            c.LastAttackerDef = LastAttackerDef;
            c.LastAttackerUid = LastAttackerUid;
            c.LastAttackSourceDef = LastAttackSourceDef;
            c.LastAttackIndex = LastAttackIndex;
            c.BirthdayToday = BirthdayToday;
            return c;
        }
    }

    public sealed class GameState
    {
        public readonly PlayerState[] Players = new PlayerState[2];
        public int CurrentPlayer;
        public int FirstPlayer;
        /// <summary>1 on the first player's first turn, 2 on the second player's first turn, and so on.</summary>
        public int TurnNumber;
        public CardInstance Stadium;
        /// <summary>v152: the other half of a Legendary Stadium in play (Stadium holds the top half); null for a normal Stadium.</summary>
        public CardInstance StadiumPartner;
        public int StadiumOwner = -1;
        public bool InSetup;
        public bool InCheckup;
        /// <summary>
        /// Star Chronos (v49): this player takes another turn after the current one, or -1. The opponent's turn in between is
        /// skipped as a turn number (see GameEngine.StartTurn), so each player's turns stay odd or even.
        /// </summary>
        public int ExtraTurnFor = -1;
        /// <summary>Star Chronos: "(Skip Pokémon Checkup.)" at the end of the current turn.</summary>
        public bool SkipCheckup;
        public bool IsOver;
        /// <summary>Winning player index, or -1 for a draw / unfinished game.</summary>
        public int Winner = -1;
        public string EndReason = "";
        public int NextUid = 1;
        /// <summary>Order of the events that can turn an Ability lock on (PokemonInPlay.PlaySeq, SpotSeq, ToolSeq).</summary>
        public int LockSeq;
        public Rng Rng;
        /// <summary>Timed effects in play (see ContinuousEffect). Effects are immutable and shared by clones.</summary>
        public List<ContinuousEffect> Effects = new List<ContinuousEffect>();
        /// <summary>
        /// v126, Lt. Surge's Secret Plan: the real card behind each face-down card on a Bench, by card id. In play the card is a
        /// stand-in with the same id (GameEngine.FaceDownDef), so card counts stay right; it's swapped back when it's flipped over
        /// or leaves play.
        /// </summary>
        public Dictionary<int, CardInstance> FaceDownCards = new Dictionary<int, CardInstance>();

        public GameState(int seed)
        {
            Players[0] = new PlayerState(0);
            Players[1] = new PlayerState(1);
            Rng = new Rng(seed);
        }

        private GameState()
        {
        }

        public PlayerState Current { get { return Players[CurrentPlayer]; } }
        public PlayerState Opponent(int player) { return Players[1 - player]; }

        /// <summary>True during each player's own first turn (no evolving; first player also can't attack or use a Supporter).</summary>
        public bool IsPlayersFirstTurn { get { return TurnNumber <= 2; } }

        public PokemonInPlay FindPokemon(int uid)
        {
            foreach (var p in Players)
                foreach (var mon in p.InPlay())
                    if (mon.Uid == uid) return mon;
            return null;
        }

        public bool IsActive(PokemonInPlay mon)
        {
            return Players[mon.Owner].Active == mon;
        }

        public GameState Clone()
        {
            var c = new GameState();
            c.Players[0] = Players[0].Clone();
            c.Players[1] = Players[1].Clone();
            c.CurrentPlayer = CurrentPlayer;
            c.FirstPlayer = FirstPlayer;
            c.TurnNumber = TurnNumber;
            c.Stadium = Stadium;
            c.StadiumPartner = StadiumPartner;
            c.StadiumOwner = StadiumOwner;
            c.InSetup = InSetup;
            c.InCheckup = InCheckup;
            c.ExtraTurnFor = ExtraTurnFor;
            c.SkipCheckup = SkipCheckup;
            c.IsOver = IsOver;
            c.Winner = Winner;
            c.EndReason = EndReason;
            c.NextUid = NextUid;
            c.LockSeq = LockSeq;
            c.Rng = Rng.Clone();
            c.Effects = new List<ContinuousEffect>(Effects);
            c.FaceDownCards = new Dictionary<int, CardInstance>(FaceDownCards);
            return c;
        }
    }
}
