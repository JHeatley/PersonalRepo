using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Destiny Trainers and Miracle Energy (N4 16, 92-105; v142). Lucky Stadium 100 is the Wizards promo (WP 41). Your ruling:
    /// Miracle Energy follows its text, discarded at the end of each of its owner's turns. Pokémon Personality Test's "Dark in its
    /// name" means Dark Pokémon (names starting with "Dark "). Magnifier works like the old PlusPower here: the chosen Pokémon's
    /// attacks ignore Resistance for the rest of the turn and the card goes to the discard pile at once.
    /// </summary>
    public static class CardsN4T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N4-16", new MiracleEnergy());
            r.RegisterByKey("N4-92", new BrokenGroundGym());
            r.RegisterByKey("N4-93", new ExpAll());
            r.RegisterByKey("N4-94", new ImpostorOaksInvention());
            r.RegisterByKey("N4-95", new RadioTower());
            r.RegisterByKey("N4-96", new ThoughtWaveMachine());
            r.RegisterByKey("N4-97", new CounterattackClaws());
            r.RegisterByKey("N4-98", new EnergyAmplifier());
            r.RegisterByKey("N4-99", new EnergyStadium());
            r.RegisterByKey("N4-101", new Magnifier());
            r.RegisterByKey("N4-102", new PokemonPersonalityTest());
            r.RegisterByKey("N4-103", new TeamRocketsEvilDeeds());
            r.RegisterByKey("N4-104", new HealPowder());
            r.RegisterByKey("N4-105", new MailFromBill());
        }

        /// <summary>
        /// Miracle Energy (1 per deck): attach it only to your Shining or Light Pokémon; it counts as every type but provides 2 Energy
        /// at a time; discarded at the end of each of your turns (your ruling: every time).
        /// </summary>
        private sealed class MiracleEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 2); }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target)
            {
                return target != null && (target.Name.StartsWith("Shining ", StringComparison.Ordinal) || target.Name.StartsWith("Light ", StringComparison.Ordinal));
            }

            public override void OnEndOfTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || src.Card == null || !holder.Energy.Contains(src.Card)) return;
                src.Log("Miracle Energy is discarded at the end of the turn.");
                src.Engine.DiscardEnergy(holder, src.Card);
            }
        }

        /// <summary>Broken Ground Gym (Stadium): each player pays [C] more to retreat a Baby Pokémon or Basic Pokémon.</summary>
        private sealed class BrokenGroundGym : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.Def.IsPokemon && (mon.Def.IsBasicPokemon || mon.Def.IsBaby) ? cost + 1 : cost;
            }
        }

        /// <summary>
        /// EXP.ALL (Pokémon Tool): during the opponent's turn, if your Active Pokémon is Knocked Out by their attack, you may move 1 of
        /// its basic Energy cards to the Pokémon with EXP.ALL; if you do, discard EXP.ALL.
        /// </summary>
        private sealed class ExpAll : CardScript
        {
            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                var holder = src.Pokemon;
                if (!byOpponentsAttack || holder == null || mon == null || mon == holder || src.IsMyTurn || !src.IsActive(mon) || !src.InPlay(holder)) return;
                var basics = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0 || !src.AskYesNo("EXP.ALL: move a basic Energy card from " + mon.Name + " to " + holder.Name + "?")) return;
                var pick = basics.Count == 1 ? basics : src.ChooseCards(basics, 1, 1, "EXP.ALL: move which basic Energy card to " + holder.Name + "?");
                if (pick.Count == 0 || !src.Engine.MoveEnergy(mon, holder, pick[0])) return;
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>
        /// Impostor Professor Oak's Invention: look at your opponent's Prize cards; you may have them shuffle the Prize cards into their
        /// deck and set aside that many cards from the top as new Prize cards.
        /// </summary>
        private sealed class ImpostorOaksInvention : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var prizes = ctx.Opp.Prizes;
                if (prizes.Count == 0) return;
                ctx.Engine.ShowCards(ctx.Player, prizes.ToList(), "Your opponent's Prize cards", "Impostor Professor Oak's Invention: your opponent's Prize cards", ctx.Source);
                if (!ctx.AskYesNo("Impostor Professor Oak's Invention: have your opponent shuffle their Prize cards into their deck and set out new ones?")) return;
                int count = prizes.Count;
                ctx.Opp.Deck.AddRange(prizes);
                prizes.Clear();
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                int take = Math.Min(count, ctx.Opp.Deck.Count);
                var fresh = ctx.Opp.Deck.Take(take).ToList();
                foreach (var card in fresh) ctx.Opp.Deck.Remove(card);
                prizes.AddRange(fresh);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled their Prize cards into their deck and set out " + take + " new ones.");
            }
        }

        /// <summary>Radio Tower (Stadium): once during each player's turn, they may look at the top 2 cards of their deck (they stay in order).</summary>
        private sealed class RadioTower : CardScript
        {
            private const string Limit = "Radio Tower";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.Deck.Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var top = ctx.Me.Deck.Take(2).ToList();
                ctx.Engine.ShowCards(ctx.Player, top, "Top of your deck", "Radio Tower: the top " + top.Count + " cards of your deck", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top " + top.Count + " cards of their deck (Radio Tower).");
                return false;
            }
        }

        /// <summary>Thought Wave Machine: flip until tails; for each heads, return an Energy card from the opponent's Active Pokémon to their hand. Your turn ends.</summary>
        private sealed class ThoughtWaveMachine : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipUntilTails();
                var active = ctx.Opp.Active;
                if (heads > 0 && active != null && active.Energy.Count > 0)
                {
                    var energy = active.Energy.ToList();
                    var picks = heads >= energy.Count ? energy : ctx.ChooseCards(energy, heads, heads, "Thought Wave Machine: return " + heads + " Energy card" + (heads == 1 ? "" : "s") + " to your opponent's hand");
                    foreach (var card in picks) ctx.Engine.ReturnEnergyToHand(active, card);
                }
                ctx.Log(ctx.Engine.Possessive(ctx.Player) + " turn ends (Thought Wave Machine).");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>
        /// Counterattack Claws (Pokémon Tool): during the opponent's turn, if its holder is your Active Pokémon and their attack
        /// damages it (even if it's Knocked Out), flip; heads, 2 damage counters on the attacking Pokémon. Then discard this card.
        /// </summary>
        private sealed class CounterattackClaws : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (holder == null || attacker == null || !holderWasActive || attacker.Owner == ctx.Player || ctx.IsMyTurn || damage <= 0) return;
                if (!holder.Tools.Contains(ctx.Card)) return;
                if (ctx.Flip() && ctx.InPlay(attacker)) ctx.Engine.PlaceDamageCounters(attacker, 2, "Counterattack Claws");
                if (holder.Tools.Contains(ctx.Card)) ctx.Engine.DiscardTool(holder, ctx.Card);
            }
        }

        /// <summary>Energy Amplifier: shuffle an Energy card from your hand into your deck (shown); heads, search for up to 3 basic Energy cards.</summary>
        private sealed class EnergyAmplifier : CardScript
        {
            private static List<CardInstance> Energy(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsEnergy).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Energy(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = Energy(ctx);
                if (energy.Count == 0) return;
                var pick = energy.Count == 1 ? energy : ctx.ChooseCards(energy, 1, 1, "Energy Amplifier: shuffle which Energy card into your deck?");
                if (pick.Count == 0 || !ctx.Me.Hand.Remove(pick[0])) return;
                ctx.Engine.ShowCards(ctx.Opponent, pick, "Energy Amplifier", "Energy Amplifier: " + ctx.Engine.PlayerName(ctx.Player) + " shuffles this card into their deck", ctx.Source);
                ctx.Me.Deck.Add(pick[0]);
                ctx.ShuffleMyDeck();
                if (!ctx.Flip()) return;
                ctx.SearchToHand(d => d.IsBasicEnergy, 3, "Energy Amplifier: choose up to 3 basic Energy cards");
            }
        }

        /// <summary>Energy Stadium (Stadium): once during each player's turn, they may flip; heads, a basic Energy card from their discard pile into their hand.</summary>
        private sealed class EnergyStadium : CardScript
        {
            private const string Limit = "Energy Stadium";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.BasicEnergyInDiscard().Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return false;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return false;
                var pick = energy.Count == 1 ? energy : ctx.ChooseCards(energy, 1, 1, "Energy Stadium: put which basic Energy card into your hand?");
                ctx.DiscardToHand(pick);
                return false;
            }
        }

        /// <summary>Magnifier: this turn, attacks by the chosen Pokémon of yours don't apply Resistance (see the class notes).</summary>
        private sealed class Magnifier : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = ctx.MyPokemon();
                if (mons.Count == 0) return;
                var target = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, "Magnifier: attach it to which of your Pokémon?");
                if (target == null) return;
                ctx.AddEffect(new MagnifierEffect(target.Name + "'s attacks don't apply Resistance this turn (Magnifier)", ctx.Player, target.Uid, ctx.Turn));
            }
        }

        private sealed class MagnifierEffect : ContinuousEffect
        {
            public MagnifierEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool IgnoresResistance(GameEngine engine, DamageInfo info)
            {
                return info.Attacker != null && info.Attacker.Uid == TargetUid && engine.CurrentSourceKind == EffectSourceKind.Attack;
            }
        }

        /// <summary>
        /// Pokémon Personality Test: put an Evolution card from your hand face down; your opponent guesses Light, Dark (your ruling: a
        /// Dark Pokémon) or neither. Right, they draw 3 cards; wrong, you draw 3. The card goes back to your hand.
        /// </summary>
        private sealed class PokemonPersonalityTest : CardScript
        {
            private static readonly string[] Guesses = { "A Pokémon with Light in its name", "A Pokémon with Dark in its name", "Neither" };

            private static List<CardInstance> Evolutions(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsEvolution).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Evolutions(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Evolutions(ctx);
                if (cards.Count == 0) return;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Pokémon Personality Test: put which Evolution card face down?")[0];
                string name = card.Def.Name;
                int truth = name.StartsWith("Light ", StringComparison.Ordinal) ? 0 : name.StartsWith("Dark ", StringComparison.Ordinal) ? 1 : 2;
                var options = new List<DecisionOption>();
                for (int i = 0; i < Guesses.Length; i++) options.Add(new DecisionOption { Label = Guesses[i], Index = i });
                int guess = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Pokémon Personality Test: what is the face-down Evolution card?", options, ctx.Source);
                if (guess < 0 || guess >= Guesses.Length) guess = 2;
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { card }, "Pokémon Personality Test", "Pokémon Personality Test: the card was " + name, ctx.Source);
                if (guess == truth)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed right (Pokémon Personality Test).");
                    ctx.Engine.DrawCards(ctx.Opponent, 3);
                }
                else
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed wrong (Pokémon Personality Test).");
                    ctx.Draw(3);
                }
            }
        }

        /// <summary>Team Rocket's Evil Deeds: look at your opponent's hand; they shuffle a card you choose into their deck, then may draw up to 2 cards.</summary>
        private sealed class TeamRocketsEvilDeeds : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count > 0)
                {
                    ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Team Rocket's Evil Deeds: your opponent's hand", ctx.Source);
                    var pick = ctx.ChooseCards(hand, 1, 1, "Team Rocket's Evil Deeds: your opponent shuffles which card into their deck?");
                    if (pick.Count > 0 && ctx.Opp.Hand.Remove(pick[0]))
                    {
                        ctx.Opp.Deck.Add(pick[0]);
                        ctx.Engine.ShuffleDeck(ctx.Opponent);
                        ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + pick[0].Def.Name + " into their deck (Team Rocket's Evil Deeds).");
                    }
                }
                int max = Math.Min(2, ctx.Opp.Deck.Count);
                if (max <= 0) return;
                int draw = ctx.Engine.AskNumber(ctx.Opponent, 0, max, "Team Rocket's Evil Deeds: draw up to " + max + " cards", ctx.Source);
                if (draw > 0) ctx.Engine.DrawCards(ctx.Opponent, Math.Min(max, draw));
            }
        }

        /// <summary>Heal Powder: heads, your Active Pokémon is no longer Asleep, Confused, Paralyzed or Poisoned and 2 damage counters come off it.</summary>
        private sealed class HealPowder : CardScript
        {
            private static readonly SpecialCondition[] Healed = { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null || !ctx.Flip()) return;
                foreach (var c in Healed) if (active.Has(c)) ctx.Engine.RemoveCondition(active, c);
                if (active.DamageCounters > 0) ctx.Heal(active, 20);
            }
        }

        /// <summary>Mail from Bill: only with 4 or fewer cards in your hand counting this one; draw until you have 4.</summary>
        private sealed class MailFromBill : CardScript
        {
            private static int Others(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Others(ctx) + 1 < 5 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int need = 4 - Others(ctx);
                if (need > 0) ctx.Draw(need);
            }
        }
    }
}
