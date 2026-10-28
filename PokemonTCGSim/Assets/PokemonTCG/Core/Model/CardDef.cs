using System.Collections.Generic;

namespace PokemonTCG.Core
{
    public sealed class AttackDef
    {
        public string Name;
        public EnergyType[] Cost = new EnergyType[0];
        public int Damage;
        /// <summary>"", "+", "x" or "-" as printed next to the damage number.</summary>
        public string DamageSuffix = "";
        public string Text = "";
        /// <summary>A VSTAR Power attack (v49): a player can use only 1 VSTAR Power (attack or Ability) in a game.</summary>
        public bool VstarPower;

        public string CostString
        {
            get
            {
                var chars = new char[Cost.Length];
                for (int i = 0; i < Cost.Length; i++) chars[i] = EnergyTypes.ToSymbol(Cost[i]);
                return new string(chars);
            }
        }
    }

    public sealed class AbilityDef
    {
        public const string AbilityKind = "Ability";
        public const string PokemonPower = "Pokémon Power";
        public const string PokePower = "Poké-Power";
        public const string PokeBody = "Poké-Body";

        public string Name;
        public string Text = "";
        /// <summary>
        /// "Ability", or an older kind: "Pokémon Power" (Base Set to Neo), "Poké-Power" or "Poké-Body" (EX to HGSS era).
        /// The older kinds aren't Abilities: effects about Abilities (blocking, preventing, "has an Ability") ignore them.
        /// </summary>
        public string Kind = AbilityKind;
        /// <summary>A VSTAR Power Ability (v49): a player can use only 1 VSTAR Power (attack or Ability) in a game.</summary>
        public bool VstarPower;

        public bool IsAbility { get { return string.IsNullOrEmpty(Kind) || Kind == AbilityKind; } }
        /// <summary>Poké-Powers and Poké-Bodies, which a LV.X Pokémon keeps from the Pokémon under it.</summary>
        public bool IsPokePowerOrBody { get { return Kind == PokePower || Kind == PokeBody; } }
    }

    /// <summary>
    /// Immutable printed card. Shared between every copy of the card and every cloned game state.
    /// Game behaviour beyond plain stats lives in a CardScript looked up by Key or Name.
    /// </summary>
    public sealed class CardDef
    {
        /// <summary>Unique printing key, e.g. "TWM-130".</summary>
        public string Key;
        /// <summary>
        /// Key of the printing whose game data this card uses. Equal to Key for the main printing; alternate
        /// printings of the same card (full arts, reprints) share it, so they play identically and use the same script.
        /// </summary>
        public string CanonicalKey;
        /// <summary>Set name of this printing, e.g. "Prismatic Evolutions" (for art pickers).</summary>
        public string PrintSetName = "";
        public string Name;
        public string SetCode;
        public string Number;
        public string RegulationMark = "";
        public CardKind Kind;
        public string Text = "";

        // Pokémon
        public Stage Stage = Stage.None;
        public string EvolvesFrom = "";
        public int Hp;
        public EnergyType PokemonType = EnergyType.Colorless;
        public EnergyType? Weakness;
        public EnergyType? Resistance;
        /// <summary>EX-era cards with 2 Weaknesses or 2 Resistances (v158): the second one (same x2 / same amount).</summary>
        public EnergyType? SecondWeakness;
        public EnergyType? SecondResistance;
        public int ResistanceAmount = 30;
        /// <summary>Weakness "+N" (Uxie: +20) instead of x2; 0 means x2.</summary>
        public int WeaknessAmount;
        public int RetreatCost;
        public bool IsEx;
        public bool IsMegaEx;
        public bool IsTera;
        public bool IsAncient;
        /// <summary>
        /// XY-era Ancient Trait the engine applies (v98): "θ Stop" (the opponent's Abilities don't affect it), "θ Double" (up to 2
        /// Pokémon Tools), "θ Max" (heal all damage when a Pokémon becomes it). Not an Ability: nothing that blocks Abilities
        /// touches it, and it only works in play. Empty for every other card.
        /// </summary>
        public string AncientTrait = "";
        public bool IsFuture;
        /// <summary>
        /// Sword &amp; Shield battle style (v49): "Single Strike", "Rapid Strike" or "Fusion Strike" (Pokémon and some Trainers
        /// and Energy), empty for everything else. A card can have more than one (Fusion Strike's Deoxys is all three, v55):
        /// they're listed with ", " between them, so check with the Is...Strike properties.
        /// </summary>
        public string Strike = "";
        public bool IsSingleStrike { get { return HasStrike("Single Strike"); } }
        public bool IsRapidStrike { get { return HasStrike("Rapid Strike"); } }
        public bool IsFusionStrike { get { return HasStrike("Fusion Strike"); } }

        private bool HasStrike(string style)
        {
            return !string.IsNullOrEmpty(Strike) && Strike.Contains(style);
        }

        /// <summary>Adds a battle style (a card can have several).</summary>
        public void AddStrike(string style)
        {
            if (HasStrike(style)) return;
            Strike = string.IsNullOrEmpty(Strike) ? style : Strike + ", " + style;
        }
        public List<AttackDef> Attacks = new List<AttackDef>();
        public List<AbilityDef> Abilities = new List<AbilityDef>();

        // Trainer
        public TrainerKind TrainerKind = TrainerKind.None;
        public bool IsAceSpec;
        /// <summary>
        /// Antique Fossils: an Item played onto the Bench as a Basic Colorless Pokémon with Hp HP (Stage, Hp, PokemonType
        /// and Abilities are filled in). It isn't a Basic Pokémon card (not for setup, mulligans or Pokémon searches),
        /// can't retreat, can't be affected by Special Conditions, gives no Prize cards and may be discarded during your turn.
        /// </summary>
        public bool IsFossil;
        /// <summary>
        /// Snorlax Doll: follows the Antique Fossil rules above (IsFossil is set too), except that it's put into play only
        /// when setting up to play, face down in the Active Spot or on the Bench, never from the hand during a turn.
        /// </summary>
        public bool IsDoll;
        /// <summary>
        /// Unidentified Fossil (Silver Tempest, v50): follows the Antique Fossil rules (IsFossil is set too) except that, as its
        /// text doesn't say otherwise, Special Conditions affect it and the opponent takes Prize cards for it (your ruling).
        /// </summary>
        public bool FossilPlain;
        /// <summary>
        /// A fossil whose Knock Out gives the opponent a Prize card: Unidentified Fossil (FossilPlain) and Rare Fossil (Darkness
        /// Ablaze, v64), which otherwise follows the Antique Fossil rules (Special Conditions don't affect it).
        /// </summary>
        public bool FossilGivesPrize;
        /// <summary>
        /// Lillie's Poké Doll (Cosmic Eclipse, v68): played as a 30-HP Basic Pokémon like Unidentified Fossil (Special Conditions
        /// affect it), but the opponent takes no Prize cards for it, and instead of being discarded from play it may be put on the
        /// bottom of the deck (its attached cards discarded) during your turn, while it's your Active Pokémon.
        /// </summary>
        public bool IsPokeDoll;
        /// <summary>
        /// v126: the stand-in for a card put face down onto the Bench by Lt. Surge's Secret Plan (GameEngine.FaceDownDef). It's a
        /// Basic Pokémon with no attacks; the real card waits in GameState.FaceDownCards until it's flipped over.
        /// </summary>
        public bool IsFaceDown;

        // Energy
        public bool IsBasicEnergy;
        public EnergyType EnergyProvides = EnergyType.Colorless;

        /// <summary>Older printing that stays Standard-legal because the card was reprinted with a legal mark.</summary>
        public bool LegalByReprint;

        // Older mechanics (30th Celebration's Classic Collection reprints)
        /// <summary>
        /// A rule-box mechanic from before Scarlet & Violet: "Pokémon-ex", "EX", "M EX", "GX", "TAG TEAM" (a GX), "V", "VMAX",
        /// "VSTAR", "Radiant", "LEGEND", "LV.X" or "BREAK" (empty for everything else). Sets the Prize count and the Rule Box.
        /// Pokémon VMAX and VSTAR are stored as Stage 1 cards that evolve from their Pokémon V.
        /// </summary>
        public string Mechanic = "";
        /// <summary>A second type: Delta Species Metagross is Lightning and Metal, Darkrai & Cresselia LEGEND Darkness and Psychic.</summary>
        public EnergyType? SecondType;
        /// <summary>The card first came out before Black & White, so it's only legal in the Unlimited format.</summary>
        public bool UnlimitedOnly;
        /// <summary>
        /// v136: "You may have as many of this card in your deck as you like" (Arceus, Arceus LV.X of the DP era). It's left out of
        /// the copy limit in every format, singleton custom formats too (your ruling).
        /// </summary>
        public bool AnyNumberInDeck;
        /// <summary>
        /// v139: "You may have up to 4 Basic Pokémon cards in your deck with Unown in their names" (Neo Discovery's Unown). With
        /// any card like that in the deck, all Basic Pokémon with Unown in their names count together toward 4 (your ruling).
        /// </summary>
        public bool UnownRule;
        /// <summary>
        /// v142: "You can't have more than 1 [this card] in your deck" (Neo Destiny's Miracle Energy and the Shining Pokémon of
        /// Neo Revelation and Neo Destiny). Set from the card data or from that line of card text.
        /// </summary>
        public bool OnePerDeck;
        /// <summary>A Basic Pokémon with Unown in its name (Unown [A], Wizards promo Unown J...), for the Unown rule.</summary>
        public bool IsUnownBasic { get { return IsBasicPokemon && Name != null && Name.Contains("Unown"); } }
        /// <summary>
        /// The card first came out before Black &amp; White (an old Trainer for GameRules.OldTrainersArePlain), even when a Black &amp;
        /// White-era reprint makes it legal in Expanded (v132: PlusPower, by your ruling). Set along with UnlimitedOnly.
        /// </summary>
        public bool PreBlackWhite;
        /// <summary>LEGEND cards come in two halves ("top" and "bottom") that are played together as one Pokémon; so do Legendary Stadiums (v152).</summary>
        public string LegendHalf = "";
        /// <summary>
        /// Neo-era Baby Pokémon with the Baby Rule printed on it (Wizards promos Pichu, Cleffa, Igglybuff; later cards of those
        /// Pokémon don't have it). While it's its owner's Active Pokémon, the opponent flips a coin when they announce an attack
        /// and their turn ends on tails; it can evolve into its Basic Pokémon from the hand (your rulings, v48).
        /// </summary>
        public bool IsBaby;

        private static readonly Dictionary<string, string[]> BabyEvolutions = new Dictionary<string, string[]>
        {
            { "Pichu", new[] { "Pikachu" } },
            { "Cleffa", new[] { "Clefairy" } },
            { "Igglybuff", new[] { "Jigglypuff" } },
            { "Smoochum", new[] { "Jynx" } },
            { "Elekid", new[] { "Electabuzz" } },
            { "Magby", new[] { "Magmar" } },
            { "Tyrogue", new[] { "Hitmonlee", "Hitmonchan", "Hitmontop" } }
        };

        /// <summary>True when this Baby Pokémon can evolve into that Basic Pokémon card: any card with the right name (Pichu: any Pikachu).</summary>
        public bool BabyEvolvesInto(CardDef basic)
        {
            if (!IsBaby || basic == null || !basic.IsBasicPokemon || basic.IsBaby) return false;
            string[] names;
            if (!BabyEvolutions.TryGetValue(Name, out names)) return false;
            foreach (var n in names) if (n == basic.Name) return true;
            return false;
        }

        public bool IsLevelUp { get { return Mechanic == "LV.X"; } }
        public bool IsBreak { get { return Mechanic == "BREAK"; } }
        public bool IsLegend { get { return Mechanic == "LEGEND"; } }
        /// <summary>
        /// A V-UNION piece (v67). Every piece carries the whole Pokémon's attacks and Abilities; 4 different pieces are combined
        /// from the discard pile into 1 Pokémon (GameEngine.CombineVUnion). A piece is never put into play on its own.
        /// </summary>
        public bool IsVUnion { get { return Mechanic == "V-UNION"; } }
        /// <summary>One Pokémon made of several cards that aren't an evolution (LEGEND halves, V-UNION pieces): nothing devolves it.</summary>
        public bool IsMultiCard { get { return IsLegend || IsVUnion; } }
        /// <summary>Mega Evolution Pokémon-EX of the XY era: evolving into one ends your turn.</summary>
        public bool IsOldMega { get { return Mechanic == "M EX"; } }
        /// <summary>
        /// What an XY-era card means by "Mega Evolution Pokémon" (Spirit Link, the Mega Evolution rule, and later Mega Turbo or
        /// Faded Town): only the XY-era M Pokémon-EX, never the Mega Evolution Pokémon ex from Mega Evolution on (IsMegaEx).
        /// Your rules update, v92.
        /// </summary>
        public bool IsXYMegaEvolution { get { return IsOldMega; } }
        /// <summary>Old Pokémon-ex (EX Ruby & Sapphire to EX Power Keepers), not the same thing as today's Pokémon ex.</summary>
        public bool IsOldEx { get { return Mechanic == "Pokémon-ex"; } }
        /// <summary>Pokémon V: V, VMAX, VSTAR and V-UNION (Compendium, Adventurer's Discovery, 2021-11-11).</summary>
        public bool IsPokemonV { get { return Mechanic == "V" || Mechanic == "VMAX" || Mechanic == "VSTAR" || Mechanic == "V-UNION"; } }
        public bool IsVmax { get { return Mechanic == "VMAX"; } }
        public bool IsVstar { get { return Mechanic == "VSTAR"; } }
        /// <summary>Radiant Pokémon (v49): a Rule Box, 1 Prize card, and only 1 Radiant Pokémon in a deck.</summary>
        public bool IsRadiant { get { return Mechanic == "Radiant"; } }
        /// <summary>Pokémon-GX, TAG TEAM included (one GX attack per game).</summary>
        public bool IsGX { get { return Mechanic == "GX" || Mechanic == "TAG TEAM"; } }
        /// <summary>Pokémon-EX of the Black & White and XY eras (Mega Evolution Pokémon-EX included), not today's Pokémon ex.</summary>
        public bool IsPokemonEX { get { return Mechanic == "EX" || Mechanic == "M EX"; } }
        /// <summary>"Pokémon-GX or Pokémon-EX" (Sun & Moon era wording).</summary>
        public bool IsGXorEX { get { return IsGX || IsPokemonEX; } }
        /// <summary>Ultra Beasts (Sun & Moon era label: Buzzwole, Guzzlord, Naganadel & Guzzlord-GX...; v68).</summary>
        public bool IsUltraBeast;
        /// <summary>A TAG TEAM Supporter (Cosmic Eclipse, v68). TAG TEAM Pokémon have the "TAG TEAM" mechanic instead.</summary>
        public bool HasTagTeamTag;
        /// <summary>Team Plasma card (v108, "Team Plasma" tag): Lugia-EX, the Plasma Storm era's Team Plasma Pokémon, Trainers and Energy.</summary>
        public bool IsTeamPlasma;
        /// <summary>A TAG TEAM card: a TAG TEAM Pokémon-GX or a TAG TEAM Supporter (Tag Call, Mawile's Dual Calling).</summary>
        public bool IsTagTeam { get { return Mechanic == "TAG TEAM" || HasTagTeamTag; } }

        public bool IsPokemon { get { return Kind == CardKind.Pokemon; } }
        public bool IsTrainer { get { return Kind == CardKind.Trainer; } }
        public bool IsEnergy { get { return Kind == CardKind.Energy; } }
        public bool IsBasicPokemon { get { return Kind == CardKind.Pokemon && Stage == Stage.Basic; } }
        public bool IsSupporter { get { return Kind == CardKind.Trainer && TrainerKind == TrainerKind.Supporter; } }
        /// <summary>
        /// An Item card. Under a format's rules (v123) a Pokémon Tool can be an Item too (ToolIsItem), and an old Trainer card can be
        /// a plain Trainer that isn't an Item (PlainTrainer).
        /// </summary>
        public bool IsItem { get { return Kind == CardKind.Trainer && !PlainTrainer && (TrainerKind == TrainerKind.Item || (TrainerKind == TrainerKind.Tool && ToolIsItem)); } }
        /// <summary>A format whose Pokémon Tools are Item cards (GameRules.ToolsAreItems, v123): this copy of a Tool is an Item too.</summary>
        public bool ToolIsItem;
        /// <summary>
        /// A format whose old Trainer cards are plain Trainers (GameRules.OldTrainersArePlain, v123): this copy isn't an Item or a
        /// Supporter (it's played like an Item, with no limit); an old Gym is still a Stadium. Its label is "Trainer".
        /// </summary>
        public bool PlainTrainer;
        /// <summary>A Trainer card first printed before Black &amp; White (Base Set to Gym Heroes, the Wizards promos and their Classic Collection reprints).</summary>
        public bool IsOldTrainer { get { return Kind == CardKind.Trainer && (UnlimitedOnly || PreBlackWhite); } }

        /// <summary>"Item", "Supporter", "Stadium", "Pokémon Tool" or, for a plain old Trainer, "Trainer".</summary>
        public string TrainerLabel
        {
            get
            {
                if (Kind != CardKind.Trainer) return "";
                if (PlainTrainer) return "Trainer";
                if (TrainerKind == TrainerKind.Tool) return ToolIsItem ? "Item (Pokémon Tool)" : "Pokémon Tool";
                return TrainerKind.ToString();
            }
        }

        /// <summary>A copy of this card under a format's rules (GameRules.Adapt): a Tool that is an Item, or a plain old Trainer.</summary>
        public CardDef CopyForRules(bool toolIsItem, bool plainTrainer)
        {
            var d = (CardDef)MemberwiseClone();
            d.ToolIsItem = toolIsItem && TrainerKind == TrainerKind.Tool;
            if (plainTrainer && IsOldTrainer)
            {
                d.PlainTrainer = true;
                // Here Comes Team Rocket! (a Supporter by your v56 ruling): played like any other old Trainer, with no limit.
                if (TrainerKind == TrainerKind.Supporter) d.TrainerKind = TrainerKind.Item;
            }
            return d;
        }
        public bool IsStadium { get { return Kind == CardKind.Trainer && TrainerKind == TrainerKind.Stadium; } }
        /// <summary>
        /// v152: one half of a Legendary Stadium (Delta Reign's Legendary Pyroduct, Legendary Summit). Both halves (LegendHalf "top"
        /// and "bottom", same name) are played from the hand together as 1 Stadium. v154.2: each half IS a Stadium card (your new
        /// ruling, replacing v152's): searches and recovery for a Stadium card (Aarune, Lusamine...) can take a half, each half 1 card.
        /// Only putting a Stadium into play from elsewhere (IsStadiumAlone) leaves halves out, since a half can't be in play alone.
        /// </summary>
        public bool IsStadiumHalf { get { return IsStadium && !string.IsNullOrEmpty(LegendHalf); } }
        /// <summary>A Stadium card for effects that search for, recover, count or discard "Stadium cards" (Legendary Stadium halves included, v154.2).</summary>
        public bool IsStadiumCard { get { return IsStadium; } }
        /// <summary>v154.2: a Stadium that can be put into play on its own (not a Legendary Stadium half).</summary>
        public bool IsStadiumAlone { get { return IsStadium && !IsStadiumHalf; } }
        public bool IsTool { get { return Kind == CardKind.Trainer && TrainerKind == TrainerKind.Tool; } }
        public bool IsSpecialEnergy { get { return Kind == CardKind.Energy && !IsBasicEnergy; } }
        /// <summary>
        /// For a card attached to a Pokémon as Energy: it's a Special Energy card. Special Energy cards, and a Pokémon card attached
        /// as Energy, which is a Special Energy card while it's attached (Electrode's Buzzap Thunder, v92: Enhanced Hammer and the
        /// other "Special Energy attached" effects see it, your ruling). Use IsSpecialEnergy for cards anywhere else.
        /// </summary>
        public bool IsAttachedSpecialEnergy { get { return IsSpecialEnergy || IsPokemon; } }
        public bool IsEvolution { get { return Kind == CardKind.Pokemon && (Stage == Stage.Stage1 || Stage == Stage.Stage2); } }
        public bool IsTeamRockets { get { return Name.StartsWith("Team Rocket's "); } }
        /// <summary>
        /// A Prism Star (◇) card (Team Up, v80): only 1 card with that name in a deck, and if it would go to the discard pile it
        /// goes to the Lost Zone instead. Its name ends in " ◇" ("Tapu Koko ◇", "Black Market ◇").
        /// </summary>
        public bool IsPrismStar { get { return Name != null && Name.EndsWith(" \u25C7", System.StringComparison.Ordinal); } }
        /// <summary>Pokémon ★ (Umbreon ★, v56): only 1 Pokémon ★ in a deck.</summary>
        public bool IsPokemonStar { get { return Kind == CardKind.Pokemon && Name != null && Name.EndsWith(" ★"); } }
        /// <summary>Pokémon SP (Diamond & Pearl Platinum era): Pokémon with G, GL, C, FB, 4 or E4 in their name (Crobat G, Garchomp C...).</summary>
        public bool IsPokemonSP
        {
            get
            {
                if (Kind != CardKind.Pokemon || Name == null) return false;
                var words = Name.Split(' ');
                for (int i = 1; i < words.Length; i++)
                {
                    var w = words[i];
                    if (w == "G" || w == "GL" || w == "C" || w == "FB" || w == "4" || w == "E4") return true;
                }
                return false;
            }
        }

        /// <summary>The main printing's key: use this to treat every printing of a card as the same card.</summary>
        public string BaseKey { get { return CanonicalKey ?? Key; } }
        /// <summary>True for an alternate printing (another art or a reprint) of a card in the pool.</summary>
        public bool IsAlternatePrint { get { return CanonicalKey != null && CanonicalKey != Key; } }

        /// <summary>An alternate printing of this card: same game data, its own set, number and art.</summary>
        public CardDef CopyAsPrinting(string key, string setCode, string number, string setName)
        {
            var d = (CardDef)MemberwiseClone();
            d.Key = key;
            d.SetCode = setCode;
            d.Number = number;
            d.CanonicalKey = BaseKey;
            d.PrintSetName = setName ?? "";
            return d;
        }

        /// <summary>Trainer's Pokémon groups such as "N's", "Hop's", "Cynthia's", "Steven's".</summary>
        public bool IsOwnersPokemon(string owner)
        {
            return Kind == CardKind.Pokemon && Name.StartsWith(owner + "'s ");
        }

        /// <summary>Pokémon ex (including Mega Evolution ex) and the older rule-box kinds (EX, GX, V, LEGEND, LV.X, BREAK...).</summary>
        public bool HasRuleBox { get { return IsEx || IsMegaEx || !string.IsNullOrEmpty(Mechanic); } }

        /// <summary>Prize cards the opponent takes when this Pokémon is Knocked Out.</summary>
        public int PrizeValue
        {
            get
            {
                if (IsMegaEx) return 3;
                switch (Mechanic)
                {
                    case "VMAX":
                    case "V-UNION":
                    case "TAG TEAM":
                        return 3;
                    case "Pokémon-ex":
                    case "EX":
                    case "M EX":
                    case "GX":
                    case "V":
                    case "VSTAR":
                    case "LEGEND":
                        return 2;
                }
                if (IsEx) return 2;
                return 1;
            }
        }

        /// <summary>
        /// True when the card has a real Ability (for card effects like Potent Glare or Salvatore). Pokémon Powers,
        /// Poké-Powers and Poké-Bodies don't count.
        /// </summary>
        public bool HasAbility
        {
            get
            {
                foreach (var a in Abilities) if (a.IsAbility) return true;
                return false;
            }
        }

        /// <summary>The source kind for this card's Abilities, Powers and Bodies: Ability, or PokemonPower when it only has older kinds.</summary>
        public EffectSourceKind AbilitySourceKind
        {
            get
            {
                if (Abilities.Count == 0) return EffectSourceKind.Ability;
                foreach (var a in Abilities) if (a.IsAbility) return EffectSourceKind.Ability;
                return EffectSourceKind.PokemonPower;
            }
        }

        /// <summary>This card's type, or its second type.</summary>
        public bool HasType(EnergyType t)
        {
            return PokemonType == t || (SecondType.HasValue && SecondType.Value == t);
        }

        public override string ToString()
        {
            return Name + " (" + Key + ")";
        }
    }
}
