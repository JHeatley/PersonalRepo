using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Up (TEU, v80), part 4: Trainers (TEU 134-158). Bill's Analysis, Brock's Grit, Erika's Hospitality and Sabrina's
    /// Suggestion are the Hidden Fates cards, Unidentified Fossil is Silver Tempest's and Viridian Forest is Unified Minds'.
    /// The Prism Star Stadiums (Black Market ◇, Wondrous Labyrinth ◇) aren't affected by Item and Supporter cards. Nanu replaces
    /// only a Basic Pokémon in play (your ruling, v80).
    /// </summary>
    public static class CardsTEU4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Black Market ◇", new BlackMarket());
            r.RegisterByName("Buff Padding", new BuffPadding());
            r.RegisterByName("Dana", new StageDrawSupporter(Stage.Stage2, "Dana"));
            r.RegisterByName("Dangerous Drill", new DangerousDrill());
            r.RegisterByName("Electrocharger", new Electrocharger());
            r.RegisterByName("Evelyn", new StageDrawSupporter(Stage.Stage1, "Evelyn"));
            r.RegisterByName("Fairy Charm UB", new FairyCharmUB());
            r.RegisterByName("Grass Memory", new MemoryTool(EnergyType.Grass));
            r.RegisterByName("Ingo & Emmet", new IngoAndEmmet());
            r.RegisterByName("Jasmine", new Jasmine());
            r.RegisterByName("Judge Whistle", new JudgeWhistle());
            r.RegisterByName("Lavender Town", new LavenderTown());
            r.RegisterByName("Metal Goggles", new MetalGoggles());
            r.RegisterByName("Morgan", new Morgan());
            r.RegisterByName("Nanu", new Nanu());
            r.RegisterByName("Nita", new Nita());
            r.RegisterByName("Pokémon Communication", new PokemonCommunication());
            r.RegisterByName("Return Label", new ReturnLabel());
            r.RegisterByName("Water Memory", new MemoryTool(EnergyType.Water));
            r.RegisterByName("Wondrous Labyrinth ◇", new WondrousLabyrinth());
        }

        // ================================================================== Stadiums

        /// <summary>
        /// Black Market ◇: when a Darkness Pokémon with any Darkness Energy attached is Knocked Out by damage from an opponent's
        /// attack, that opponent takes 1 fewer Prize card.
        /// </summary>
        private sealed class BlackMarket : CardScript
        {
            public override bool IgnoresItemsAndSupporters { get { return true; } }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut == null || prizes <= 0 || taker == knockedOut.Owner) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                if (!src.Engine.IsType(knockedOut, EnergyType.Darkness) || !src.Engine.HasEnergyOfType(knockedOut, EnergyType.Darkness)) return prizes;
                return prizes - 1;
            }
        }

        /// <summary>Wondrous Labyrinth ◇: the attacks of non-Fairy Pokémon (both players') cost [C] more.</summary>
        private sealed class WondrousLabyrinth : CardScript
        {
            public override bool IgnoresItemsAndSupporters { get { return true; } }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != null && !src.Engine.IsType(mon, EnergyType.Fairy)) cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>Lavender Town: once during each player's turn, that player may have their opponent reveal their hand.</summary>
        private sealed class LavenderTown : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.Reveal(ctx.Opponent, hand, "revealed their hand (Lavender Town)");
                ctx.Engine.ShowCards(ctx.Player, hand, "Lavender Town", ctx.Engine.PlayerName(ctx.Opponent) + "'s hand", ctx.Card.Def);
                return false;
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Buff Padding: if the Pokémon this is attached to has a Retreat Cost of exactly 4, it gets +50 HP.</summary>
        private sealed class BuffPadding : CardScript
        {
            [ThreadStatic] private static bool busy;

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (busy || src.Pokemon == null || mon != src.Pokemon) return hp;
                busy = true;
                try { return src.Engine.RetreatCostOf(mon) == 4 ? hp + 50 : hp; }
                finally { busy = false; }
            }
        }

        /// <summary>Fairy Charm UB: prevent all damage done to the Fairy Pokémon this is attached to by attacks from the opponent's Ultra Beast Pokémon-GX and Pokémon-EX.</summary>
        private sealed class FairyCharmUB : CardScript
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !info.AgainstOpponent) return false;
                var attacker = info.Attacker.Def;
                return src.Engine.IsType(me, EnergyType.Fairy) && attacker.IsUltraBeast && attacker.IsGXorEX;
            }
        }

        /// <summary>Grass Memory / Water Memory: the Silvally-GX this is attached to is a Pokémon of that type.</summary>
        private sealed class MemoryTool : CardScript
        {
            private readonly EnergyType type;

            public MemoryTool(EnergyType type) { this.type = type; }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Name != "Silvally-GX") return;
                types.Clear();
                types.Add(type);
            }
        }

        /// <summary>
        /// Metal Goggles: the Metal Pokémon this is attached to takes 30 less damage from the opponent's attacks (after Weakness and
        /// Resistance), and the opponent's attacks and Abilities can't put damage counters on it.
        /// </summary>
        private sealed class MetalGoggles : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !info.AgainstOpponent || !src.Engine.IsType(me, EnergyType.Metal)) return damage;
                return damage - 30;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return damageCounters && src.Pokemon != null && target == src.Pokemon && src.Engine.IsType(target, EnergyType.Metal)
                       && (kind == EffectSourceKind.Attack || kind == EffectSourceKind.Ability);
            }
        }

        // ================================================================== Items

        /// <summary>
        /// Dangerous Drill: only if you discard a Darkness Pokémon from your hand. Discard a Pokémon Tool or Special Energy card from
        /// 1 of your opponent's Pokémon, or discard a Stadium card in play.
        /// </summary>
        private sealed class DangerousDrill : CardScript
        {
            private static List<CardInstance> DarknessInHand(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsPokemon && (c.Def.PokemonType == EnergyType.Darkness || c.Def.SecondType == EnergyType.Darkness)).ToList();
            }

            private static bool HasTarget(EffectContext ctx)
            {
                if (ctx.State.Stadium != null && !ctx.Engine.Scripts.For(ctx.State.Stadium.Def).IgnoresItemsAndSupporters) return true;
                return ctx.Opp.InPlay().Any(m => m.Tools.Count > 0 || m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return DarknessInHand(ctx).Count > 0 && HasTarget(ctx); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cost = DarknessInHand(ctx);
                if (cost.Count == 0) return;
                var pay = cost.Count == 1 ? cost[0] : ctx.ChooseCards(cost, 1, 1, "Dangerous Drill: discard a Darkness Pokémon from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pay);

                var labels = new List<string>();
                var holders = new List<PokemonInPlay>();
                var cards = new List<CardInstance>();
                foreach (var mon in ctx.Opp.InPlay())
                {
                    if (!ctx.CanAffect(mon)) continue;
                    foreach (var t in mon.Tools)
                    {
                        labels.Add(t.Def.Name + " (on " + ctx.Engine.OwnedName(mon) + ")");
                        holders.Add(mon);
                        cards.Add(t);
                    }
                    foreach (var e in mon.Energy.Where(x => x.Def.IsAttachedSpecialEnergy))
                    {
                        labels.Add(e.Def.Name + " (on " + ctx.Engine.OwnedName(mon) + ")");
                        holders.Add(mon);
                        cards.Add(e);
                    }
                }
                var stadium = ctx.State.Stadium;
                if (stadium != null && ctx.Engine.StadiumIgnoresCurrentTrainer()) stadium = null;
                if (stadium != null) labels.Add(stadium.Def.Name + " (Stadium)");
                if (labels.Count == 0) return;
                int pick = ctx.ChooseOption("Dangerous Drill: discard which card?", labels.ToArray());
                if (pick < 0) return;
                if (pick < cards.Count)
                {
                    var holder = holders[pick];
                    var card = cards[pick];
                    if (holder.Tools.Contains(card)) ctx.Engine.DiscardTool(holder, card);
                    else ctx.Engine.DiscardEnergy(holder, card);
                    return;
                }
                if (stadium != null && ctx.State.Stadium == stadium) ctx.Engine.DiscardStadium();
            }
        }

        /// <summary>Electrocharger: flip 2 coins; for each heads, shuffle an Electropower card from your discard pile into your deck.</summary>
        private sealed class Electrocharger : CardScript
        {
            private static List<CardInstance> InDiscard(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.Name == CardsTEU2.Electropower).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return InDiscard(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                var cards = InDiscard(ctx);
                int n = Math.Min(heads, cards.Count);
                if (n <= 0) return;
                ctx.DiscardToDeck(cards.Take(n).ToList());
                ctx.ShuffleMyDeck();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + n + " Electropower " + (n == 1 ? "card" : "cards") + " into their deck.");
            }
        }

        /// <summary>Judge Whistle: draw a card, or put a Judge card from your discard pile into your hand.</summary>
        private sealed class JudgeWhistle : CardScript
        {
            private static List<CardInstance> Judges(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.Name == "Judge").ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || Judges(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var judges = Judges(ctx);
                bool canDraw = ctx.Me.Deck.Count > 0;
                int pick;
                if (judges.Count == 0) pick = 0;
                else if (!canDraw) pick = 1;
                else pick = ctx.ChooseOption("Judge Whistle: choose 1", "Draw a card", "Put a Judge card from your discard pile into your hand");
                if (pick == 0) ctx.Draw(1);
                else ctx.DiscardToHand(new[] { judges[0] });
            }
        }

        /// <summary>Pokémon Communication: put a Pokémon from your hand into your deck; then search your deck for a Pokémon and put it into your hand.</summary>
        private sealed class PokemonCommunication : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pokemon = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return;
                var card = pokemon.Count == 1 ? pokemon[0] : ctx.ChooseCards(pokemon, 1, 1, "Pokémon Communication: reveal a Pokémon from your hand and put it into your deck")[0];
                ctx.Engine.Reveal(ctx.Player, new[] { card }, "put this card into their deck (Pokémon Communication)");
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Add(card);
                ctx.SearchToHand(d => d.IsPokemon, 1, "Pokémon Communication: search your deck for a Pokémon");
            }
        }

        /// <summary>Return Label: put a card from your opponent's discard pile on the bottom of their deck.</summary>
        private sealed class ReturnLabel : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Discard.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var opp = ctx.Opp;
                var card = ctx.ChooseCards(opp.Discard.ToList(), 1, 1, "Return Label: put a card from your opponent's discard pile on the bottom of their deck")[0];
                opp.Discard.Remove(card);
                opp.Deck.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + card.Def.Name + " from " + ctx.Engine.PlayerName(ctx.Opponent) + "'s discard pile on the bottom of their deck.");
            }
        }

        // ================================================================== Supporters

        /// <summary>
        /// Dana (Stage 2: search your deck for up to 2 cards) and Evelyn (Stage 1: draw 4 cards): only if your opponent's Active
        /// Pokémon is that Stage.
        /// </summary>
        private sealed class StageDrawSupporter : CardScript
        {
            private readonly Stage stage;
            private readonly string name;

            public StageDrawSupporter(Stage stage, string name)
            {
                this.stage = stage;
                this.name = name;
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var active = ctx.Opp.Active;
                return active != null && active.Def.IsPokemon && active.Def.Stage == stage && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (stage == Stage.Stage1) ctx.Draw(4);
                else ctx.SearchToHand(d => true, 2, name + ": search your deck for up to 2 cards", false, 1);
            }
        }

        /// <summary>
        /// Ingo &amp; Emmet: look at the top card of your deck, then discard your hand and draw 5 cards, either from the top or from
        /// the bottom of your deck.
        /// </summary>
        private sealed class IngoAndEmmet : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(1);
                if (top.Count > 0) ctx.Engine.ShowCards(ctx.Player, top, "Ingo & Emmet", "The top card of your deck", ctx.Source);
                int pick = ctx.ChooseOption("Ingo & Emmet: choose 1", "Discard your hand and draw 5 cards", "Discard your hand and draw 5 cards from the bottom of your deck");
                ctx.Engine.DiscardHand(ctx.Player);
                if (pick == 0)
                {
                    ctx.Draw(5);
                    return;
                }
                var me = ctx.Me;
                int n = 0;
                for (; n < 5 && me.Deck.Count > 0; n++)
                {
                    var card = me.Deck[me.Deck.Count - 1];
                    me.Deck.RemoveAt(me.Deck.Count - 1);
                    me.Hand.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew " + n + (n == 1 ? " card" : " cards") + " from the bottom of their deck.");
            }
        }

        /// <summary>Jasmine: search your deck for a Metal Pokémon, or up to 5 if you went second and it's your first turn.</summary>
        private sealed class Jasmine : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var st = ctx.State;
                int n = st.IsPlayersFirstTurn && st.FirstPlayer != ctx.Player ? 5 : 1;
                ctx.SearchToHand(d => d.IsPokemon && (d.PokemonType == EnergyType.Metal || d.SecondType == EnergyType.Metal), n,
                    "Jasmine: search your deck for " + (n == 1 ? "a Metal Pokémon" : "up to 5 Metal Pokémon"));
            }
        }

        /// <summary>
        /// Morgan: only if you discard Dana, Evelyn and Nita from your hand. Look at the top 12 cards of your deck and attach any
        /// number of Energy cards there to your Pokémon in any way you like; shuffle the other cards back.
        /// </summary>
        private sealed class Morgan : CardScript
        {
            private static readonly string[] Needed = { "Dana", "Evelyn", "Nita" };

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return Needed.All(n => ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.Name == n)) && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var name in Needed)
                {
                    var card = ctx.Me.Hand.FirstOrDefault(c => c.Def.Name == name);
                    if (card != null) ctx.Engine.DiscardFromHand(ctx.Player, card);
                }
                var top = ctx.LookAtTop(12);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) ctx.Engine.ShowCards(ctx.Player, top, "Morgan", "The top " + top.Count + " cards of your deck (no Energy cards)", ctx.Source);
                else ctx.AttachCardsAnyWay(energy, energy.Count, (c, m) => true, "Morgan: attach Energy to your Pokémon");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Nanu: choose a Basic Darkness Pokémon in your discard pile and switch it with 1 of your Basic Pokémon in play (your
        /// ruling). Attached cards, damage counters, Special Conditions, turns in play and other effects stay on the new Pokémon;
        /// the old card goes to the discard pile.
        /// </summary>
        private sealed class Nanu : CardScript
        {
            private static List<CardInstance> Candidates(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && (c.Def.PokemonType == EnergyType.Darkness || c.Def.SecondType == EnergyType.Darkness)).ToList();
            }

            private static List<PokemonInPlay> Replaceable(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Stack.Count == 1 && m.Def.IsBasicPokemon);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Candidates(ctx).Count > 0 && Replaceable(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Candidates(ctx);
                var targets = Replaceable(ctx);
                if (cards.Count == 0 || targets.Count == 0) return;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Nanu: choose a Basic Darkness Pokémon in your discard pile")[0];
                var mon = ctx.ChoosePokemon(targets, "Nanu: switch " + card.Def.Name + " with which of your Basic Pokémon?");
                var old = mon.Stack[0];
                ctx.Me.Discard.Remove(card);
                mon.Stack[0] = card;
                ctx.Me.Discard.Add(old);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched " + old.Def.Name + " with " + card.Def.Name + " from their discard pile (Nanu).");
            }
        }

        /// <summary>Nita: only if your opponent's Active Pokémon is a Basic Pokémon. Put an Energy from it on top of their deck.</summary>
        private sealed class Nita : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var active = ctx.Opp.Active;
                return active != null && active.IsBasic && active.Energy.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Opp.Active;
                if (active == null || active.Energy.Count == 0 || !ctx.CanAffect(active)) return;
                var energy = active.Energy.Count == 1 ? active.Energy[0] : ctx.ChooseCards(active.Energy.ToList(), 1, 1, "Nita: put an Energy from " + active.Name + " on top of your opponent's deck")[0];
                if (ctx.Engine.EnergyRemovalProtected(active, energy))
                {
                    ctx.Log(energy.Def.Name + " can't be moved by Trainer cards.");
                    return;
                }
                active.Energy.Remove(energy);
                ctx.Opp.Deck.Insert(0, energy);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + energy.Def.Name + " from " + active.Name + " on top of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck (Nita).");
            }
        }
    }
}
