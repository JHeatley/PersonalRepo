using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Game rules a format can change (custom formats, v123). The defaults are the normal rules: the player going first can't attack
    /// or play a Supporter on their first turn, the opening coin flip comes before the hands are drawn, Pokémon Tools are their own
    /// kind of Trainer card, and old Trainer cards keep the kinds the pool gives them (Item, Stadium, Supporter).
    /// </summary>
    public sealed class GameRules
    {
        /// <summary>The player going first may attack during their first turn.</summary>
        public bool FirstTurnAttack;
        /// <summary>The player going first may play a Supporter during their first turn.</summary>
        public bool FirstTurnSupporter;
        /// <summary>Both players draw their opening hands (and take mulligans) before the coin flip for who goes first.</summary>
        public bool DrawBeforeCoinFlip;
        /// <summary>Pokémon Tools are Item cards too: "Item card" effects (Item lock, Item searches) include them.</summary>
        public bool ToolsAreItems;
        /// <summary>
        /// Old Trainer cards (first printed before Black &amp; White) are plain Trainer cards (your rulings, v123): they aren't Items
        /// (Item lock and "Item card" effects skip them) and have no Supporter limit (Here Comes Team Rocket!); old Gyms still work
        /// as Stadiums, labeled Trainer.
        /// </summary>
        public bool OldTrainersArePlain;

        public static readonly GameRules Default = new GameRules();

        public bool IsDefault
        {
            get { return !FirstTurnAttack && !FirstTurnSupporter && !DrawBeforeCoinFlip && !ToolsAreItems && !OldTrainersArePlain; }
        }

        /// <summary>True when the cards themselves change (Tools as Items, old Trainers as plain Trainers).</summary>
        public bool ChangesCards { get { return ToolsAreItems || OldTrainersArePlain; } }

        public GameRules Clone() { return (GameRules)MemberwiseClone(); }

        private static readonly string[] Names = { "first-turn-attack", "first-turn-supporter", "draw-before-flip", "tools-are-items", "plain-old-trainers" };

        private bool[] Flags() { return new[] { FirstTurnAttack, FirstTurnSupporter, DrawBeforeCoinFlip, ToolsAreItems, OldTrainersArePlain }; }

        /// <summary>"first-turn-attack, tools-are-items" (the rules that differ from the defaults), or "" for the normal rules.</summary>
        public override string ToString()
        {
            var flags = Flags();
            var sb = new StringBuilder();
            for (int i = 0; i < flags.Length; i++)
            {
                if (!flags[i]) continue;
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(Names[i]);
            }
            return sb.ToString();
        }

        /// <summary>Reads ToString's text (unknown words are ignored).</summary>
        public static GameRules Parse(string text)
        {
            var r = new GameRules();
            foreach (var raw in (text ?? "").Split(','))
            {
                switch (raw.Trim().ToLowerInvariant())
                {
                    case "first-turn-attack": r.FirstTurnAttack = true; break;
                    case "first-turn-supporter": r.FirstTurnSupporter = true; break;
                    case "draw-before-flip": r.DrawBeforeCoinFlip = true; break;
                    case "tools-are-items": r.ToolsAreItems = true; break;
                    case "plain-old-trainers": r.OldTrainersArePlain = true; break;
                }
            }
            return r;
        }

        // ------------------------------------------------------------------ Cards under these rules

        private static readonly Dictionary<string, CardDef> adapted = new Dictionary<string, CardDef>(StringComparer.Ordinal);

        /// <summary>
        /// The card as these rules see it: a copy marked as an Item (a Tool, with Tools as Items) or as a plain Trainer (an old
        /// Trainer card), or the card itself when nothing changes. Copies are shared, so the same printing always gives the same copy.
        /// </summary>
        public CardDef Adapt(CardDef d)
        {
            if (d == null || !d.IsTrainer) return d;
            bool tool = ToolsAreItems && d.TrainerKind == TrainerKind.Tool;
            bool plain = OldTrainersArePlain && d.IsOldTrainer;
            if (!tool && !plain) return d;
            string key = (tool ? "T" : "-") + (plain ? "P" : "-") + "|" + d.Key;
            lock (adapted)
            {
                CardDef copy;
                if (!adapted.TryGetValue(key, out copy))
                {
                    copy = d.CopyForRules(tool, plain);
                    adapted[key] = copy;
                }
                return copy;
            }
        }

        public List<CardDef> Adapt(IList<CardDef> deck)
        {
            var list = new List<CardDef>(deck.Count);
            foreach (var d in deck) list.Add(Adapt(d));
            return list;
        }
    }
}
