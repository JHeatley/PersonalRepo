using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Celestial Storm (CES, v83), part 4: Trainers and Rainbow Energy (CES 123-151). Copycat, Energy Switch, Fisherman, Hiker, Rare
    /// Candy, Shrine of Punishment and Switch were already in the pool. Pokémon Tools are not Item cards (your ruling, v90).
    /// </summary>
    public static class CardsCES4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Acro Bike", new AcroBike());
            r.RegisterByName("Apricorn Maker", new ApricornMaker());
            r.RegisterByName("Beast Ball", new BeastBall());
            r.RegisterByName("Bill's Maintenance", new BillsMaintenance());
            r.RegisterByName("Energy Recycle System", new EnergyRecycleSystem());
            r.RegisterByName("Friend Ball", new FriendBall());
            r.RegisterByName("Hau", new DrawSupporter(3));
            r.RegisterByName("Hustle Belt", new HustleBelt());
            r.RegisterByName("Last Chance Potion", new LastChancePotion());
            r.RegisterByName("Life Herb", new LifeHerb());
            r.RegisterByName("Lisia", new Lisia());
            r.RegisterByName("Lure Ball", new LureBall());
            r.RegisterByName("The Masked Royal", new MaskedRoyal());
            r.RegisterByName("PokéNav", new PokeNav());
            r.RegisterByName("Rainbow Brush", new RainbowBrush());
            r.RegisterByName("Sky Pillar", new SkyPillar());
            r.RegisterByName("Steven's Resolve", new StevensResolve());
            r.RegisterByName("Super Scoop Up", new SuperScoopUp());
            r.RegisterByName("Tate & Liza", new TateAndLiza());
            r.RegisterByName("TV Reporter", new TvReporter());
            r.RegisterByName("Underground Expedition", new UndergroundExpedition());
            r.RegisterByName("Rainbow Energy", new RainbowEnergy());
        }

        // ================================================================== Items

        /// <summary>Acro Bike: look at the top 2 cards of your deck, put 1 into your hand and discard the other.</summary>
        private sealed class AcroBike : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return;
                var keep = ctx.ChooseCards(top, 1, 1, "Acro Bike: put which card into your hand? (The other is discarded.)");
                if (keep.Count == 0) keep = new List<CardInstance> { top[0] };
                ctx.DeckToHand(keep);
                if (top.Count > 1) ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
            }
        }

        /// <summary>
        /// Beast Ball: look at your face-down Prize cards; you may put an Ultra Beast there into your hand, and Beast Ball takes its place
        /// as a face-down Prize card (otherwise it's discarded). Then shuffle the face-down Prize cards. Swapping isn't taking a Prize
        /// card (Compendium, 2018-08-16).
        /// </summary>
        private sealed class BeastBall : CardScript
        {
            private static List<int> FaceDown(PlayerState me)
            {
                var list = new List<int>();
                for (int i = 0; i < me.Prizes.Count; i++)
                    if (!me.FaceUpPrizeIds.Contains(me.Prizes[i].Id)) list.Add(i);
                return list;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return FaceDown(ctx.Me).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                var prizes = FaceDown(me).Select(i => me.Prizes[i]).ToList();
                ctx.Engine.NoteLookedAt(prizes, "Your face-down Prize cards", ctx.Player);
                var beasts = prizes.Where(c => c.Def.IsUltraBeast).ToList();
                if (beasts.Count == 0) ctx.ChooseCards(beasts, 0, 1, "Beast Ball: there's no Ultra Beast in your face-down Prize cards");
                else
                {
                    var pick = ctx.ChooseCards(beasts, 0, 1, "Beast Ball: take an Ultra Beast into your hand (Beast Ball takes its place)?");
                    if (pick.Count > 0)
                    {
                        var taken = pick[0];
                        var slots = me.PrizeSlotList();
                        int index = me.Prizes.IndexOf(taken);
                        int slot = slots[index];
                        me.Prizes[index] = ctx.Card;
                        me.PrizeSlotByCard.Remove(taken.Id);
                        me.PrizeSlotByCard[ctx.Card.Id] = slot;
                        me.Hand.Add(taken);
                        ctx.CardPlacedElsewhere = true;
                        ctx.Engine.Reveal(ctx.Player, pick, "took from " + ctx.Engine.Their(ctx.Player) + " Prize cards (Beast Ball took its place)");
                    }
                }
                ShuffleFaceDownPrizes(ctx);
            }

            private static void ShuffleFaceDownPrizes(EffectContext ctx)
            {
                var me = ctx.Me;
                var slots = me.PrizeSlotList();
                var idx = FaceDown(me);
                var cards = idx.Select(i => me.Prizes[i]).ToList();
                ctx.State.Rng.Shuffle(cards);
                for (int k = 0; k < idx.Count; k++)
                {
                    me.Prizes[idx[k]] = cards[k];
                    me.PrizeSlotByCard[cards[k].Id] = slots[idx[k]];
                }
                ctx.Engine.ForgetLookedAt();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + ctx.Engine.Their(ctx.Player) + " face-down Prize cards.");
            }
        }

        /// <summary>Energy Recycle System: choose 1 — put a basic Energy card from your discard pile into your hand, or shuffle 3 into your deck (as many as you have).</summary>
        private sealed class EnergyRecycleSystem : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return;
                int choice = ctx.ChooseOption("Energy Recycle System: choose 1",
                    "Put a basic Energy card from your discard pile into your hand", "Shuffle 3 basic Energy cards from your discard pile into your deck");
                if (choice == 0)
                {
                    var pick = ctx.ChooseCards(energy, 1, 1, "Energy Recycle System: put which basic Energy card into your hand?");
                    ctx.DiscardToHand(pick);
                    return;
                }
                int n = Math.Min(3, energy.Count);
                var picks = energy.Count <= n ? energy : ctx.ChooseCards(energy, n, n, "Energy Recycle System: shuffle which 3 basic Energy cards into your deck?");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Friend Ball: search your deck for a Pokémon that shares a type with 1 of the opponent's Pokémon in play.</summary>
        private sealed class FriendBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Opp.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var types = new HashSet<EnergyType>();
                foreach (var mon in ctx.OpponentPokemon())
                    foreach (var t in ctx.Engine.TypesOf(mon)) types.Add(t);
                ctx.SearchToHand(d => d.IsPokemon && (types.Contains(d.PokemonType) || (d.SecondType.HasValue && types.Contains(d.SecondType.Value))), 1,
                    "Friend Ball: choose a Pokémon of the same type as 1 of your opponent's Pokémon");
            }
        }

        /// <summary>Last Chance Potion: heal 120 damage from 1 of your Pokémon that has 30 HP or less remaining.</summary>
        private sealed class LastChancePotion : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.RemainingHp(m) <= 30);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "Last Chance Potion: heal 120 damage from which Pokémon?");
                if (mon != null) ctx.Heal(mon, 120);
            }
        }

        /// <summary>Life Herb: flip a coin; on heads, heal 60 damage from 1 of your Pokémon and remove all Special Conditions from it.</summary>
        private sealed class LifeHerb : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 || m.Conditions != SpecialCondition.None);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "Life Herb: heal 60 damage and remove all Special Conditions from which Pokémon?");
                if (mon == null) return;
                if (mon.DamageCounters > 0) ctx.Heal(mon, 60);
                if (mon.Conditions != SpecialCondition.None) ctx.Engine.RemoveAllConditions(mon);
            }
        }

        /// <summary>Lure Ball: flip 3 coins; for each heads, put an Evolution Pokémon from your discard pile into your hand.</summary>
        private sealed class LureBall : CardScript
        {
            private static List<CardInstance> Evolutions(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsEvolution).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Evolutions(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(3);
                var evolutions = Evolutions(ctx);
                int n = Math.Min(heads, evolutions.Count);
                if (n <= 0) return;
                var picks = evolutions.Count <= n ? evolutions : ctx.ChooseCards(evolutions, n, n, "Lure Ball: put which " + n + " Evolution Pokémon into your hand?");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>PokéNav: look at the top 3 cards of your deck; you may put a Pokémon or Energy card there into your hand; put the rest back in any order.</summary>
        private sealed class PokeNav : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return;
                var options = top.Where(c => c.Def.IsPokemon || c.Def.IsEnergy).ToList();
                int left = top.Count;
                if (options.Count == 0)
                    ctx.Engine.ShowCards(ctx.Player, top, "PokéNav", "There's no Pokémon or Energy card in the top " + top.Count + " cards of your deck", ctx.Source);
                else
                {
                    var pick = ctx.ChooseCards(options, 0, 1, "PokéNav: put a Pokémon or Energy card into your hand?");
                    if (pick.Count > 0)
                    {
                        ctx.Engine.Reveal(ctx.Player, pick, "found with PokéNav");
                        ctx.DeckToHand(pick);
                        left--;
                    }
                }
                CardsOBF2.ReorderOwnTop(ctx, left, "PokéNav");
            }
        }

        /// <summary>
        /// Rainbow Brush: choose an Energy card attached to 1 of your Pokémon, search your deck for a basic Energy card and switch them,
        /// then shuffle the first card into your deck.
        /// </summary>
        private sealed class RainbowBrush : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var holders = ctx.MyPokemon(m => m.Energy.Count > 0);
                if (holders.Count == 0) return;
                var mon = ctx.ChoosePokemon(holders, "Rainbow Brush: switch an Energy on which Pokémon?");
                if (mon == null) return;
                var old = ctx.ChooseCards(mon.Energy.ToList(), 1, 1, "Rainbow Brush: switch which Energy card on " + mon.Name + "?");
                if (old.Count == 0) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Rainbow Brush: choose a basic Energy card to attach to " + mon.Name);
                if (found.Count == 0)
                {
                    ctx.ShuffleMyDeck();
                    return;
                }
                ctx.Engine.AttachEnergyCard(found[0], mon);
                ctx.Engine.ShuffleEnergyIntoDeck(mon, old);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Super Scoop Up: flip a coin; on heads, put 1 of your Pokémon and all cards attached to it into your hand.</summary>
        private sealed class SuperScoopUp : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var mon = ctx.ChoosePokemon(ctx.MyPokemon(), "Super Scoop Up: put which of your Pokémon and all attached cards into your hand?");
                if (mon != null) ctx.Engine.ReturnPokemonToHand(mon);
            }
        }

        // ================================================================== Tool

        /// <summary>Hustle Belt: with 30 HP or less remaining and any damage counters, the holder's attacks do 60 more damage to the opponent's Active Pokémon.</summary>
        private sealed class HustleBelt : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0) return damage;
                if (holder.DamageCounters == 0 || src.RemainingHp(holder) > 30) return damage;
                return damage + 60;
            }
        }

        // ================================================================== Supporters

        /// <summary>"Draw N cards."</summary>
        private sealed class DrawSupporter : CardScript
        {
            private readonly int cards;

            public DrawSupporter(int cards) { this.cards = cards; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(cards); }
        }

        /// <summary>Apricorn Maker: search your deck for up to 2 Item cards with "Ball" in their names (Compendium, 2018-08-02).</summary>
        private sealed class ApricornMaker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsItem && d.Name.Contains("Ball"), 2, "Apricorn Maker: choose up to 2 Item cards with \"Ball\" in their names");
            }
        }

        /// <summary>Bill's Maintenance: shuffle a card from your hand into your deck; if you do, draw 3 cards.</summary>
        private sealed class BillsMaintenance : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "Bill's Maintenance: shuffle which card from your hand into your deck? (Then draw 3.)");
                if (pick.Count == 0 || !ctx.Me.Hand.Remove(pick[0])) return;
                ctx.Me.Deck.Add(pick[0]);
                ctx.ShuffleMyDeck();
                ctx.Draw(3);
            }
        }

        /// <summary>Lisia: search your deck for up to 2 Prism Star cards.</summary>
        private sealed class Lisia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPrismStar, 2, "Lisia: choose up to 2 Prism Star cards");
            }
        }

        /// <summary>The Masked Royal: attach a basic Energy card from your hand to 1 of your Stage 2 Grass, Fire or Water Pokémon.</summary>
        private sealed class MaskedRoyal : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Def.Stage == Stage.Stage2
                    && (ctx.Engine.IsType(m, EnergyType.Grass) || ctx.Engine.IsType(m, EnergyType.Fire) || ctx.Engine.IsType(m, EnergyType.Water)));
            }

            private static List<CardInstance> Energy(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsBasicEnergy).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Energy(ctx).Count > 0 && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                var energy = Energy(ctx);
                if (targets.Count == 0 || energy.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "The Masked Royal: attach a basic Energy card to which Stage 2 Pokémon?");
                if (target == null) return;
                ctx.AttachCardsAnyWay(energy, 1, (card, mon) => mon == target, "The Masked Royal: attach a basic Energy card to " + target.Name, false);
            }
        }

        /// <summary>Steven's Resolve: search your deck for up to 3 cards; your turn ends, unless your Active Metagross's Extend works (Compendium, 2018-08-02).</summary>
        private sealed class StevensResolve : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => true, 3, "Steven's Resolve: choose up to 3 cards (your turn then ends)", false);
                var active = ctx.Me.Active;
                if (active != null && ctx.Engine.HasAbilityNamed(active, "Extend"))
                {
                    ctx.Log(active.Name + "'s Extend: the turn doesn't end.");
                    return;
                }
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Tate &amp; Liza: choose 1 — shuffle your hand into your deck and draw 5 cards, or switch your Active Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class TateAndLiza : CardScript
        {
            private static bool CanDraw(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Me.Hand.Any(c => c != ctx.Card); }

            private static bool CanSwitch(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            public override bool CanPlayTrainer(EffectContext ctx) { return CanDraw(ctx) || CanSwitch(ctx); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                const string Draw = "Shuffle your hand into your deck and draw 5 cards";
                const string Swap = "Switch your Active Pokémon with 1 of your Benched Pokémon";
                var labels = new List<string>();
                if (CanDraw(ctx)) labels.Add(Draw);
                if (CanSwitch(ctx)) labels.Add(Swap);
                if (labels.Count == 0) return;
                string choice = labels[ctx.ChooseOneOption("Tate & Liza: choose 1", labels.ToArray())];
                if (choice == Swap)
                {
                    ctx.SwitchMyActive("Tate & Liza: switch your Active Pokémon with which Benched Pokémon?");
                    return;
                }
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
            }
        }

        /// <summary>TV Reporter: draw 3 cards, then discard a card from your hand.</summary>
        private sealed class TvReporter : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "TV Reporter: discard a card from your hand");
                if (pick.Count > 0) ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
            }
        }

        /// <summary>Underground Expedition: look at the bottom 4 cards of your deck, put 2 into your hand, and the rest back on the bottom in any order.</summary>
        private sealed class UndergroundExpedition : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                int look = Math.Min(4, deck.Count);
                if (look == 0) return;
                var bottom = deck.Skip(deck.Count - look).ToList();
                ctx.Engine.NoteLookedAt(bottom, "Bottom " + look + (look == 1 ? " card" : " cards") + " of your deck", ctx.Player);
                int n = Math.Min(2, bottom.Count);
                var picks = bottom.Count <= n ? bottom : ctx.ChooseCards(bottom, n, n, "Underground Expedition: put which 2 cards into your hand?");
                ctx.DeckToHand(picks);
                var left = bottom.Where(x => !picks.Contains(x)).ToList();
                if (left.Count < 2) return;
                foreach (var card0 in left) deck.Remove(card0);
                var ordered = new List<CardInstance>();
                while (left.Count > 1)
                {
                    var pick = ctx.ChooseCards(left, 1, 1, "Underground Expedition: choose the card to put back " + (ordered.Count == 0 ? "on the very bottom" : "next, above the last one"));
                    var card = pick.Count > 0 ? pick[0] : left[0];
                    ordered.Add(card);
                    left.Remove(card);
                }
                ordered.Add(left[0]);
                ordered.Reverse();
                deck.AddRange(ordered);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + ordered.Count + " cards back on the bottom of their deck in any order.");
            }
        }

        // ================================================================== Stadium

        /// <summary>
        /// Sky Pillar: prevent all effects of the opponent's attacks, including damage, done to Benched Pokémon (both players'). A
        /// player's own attacks still reach their own Bench (Compendium, 2018-08-09: Zapdos's Raging Thunder).
        /// </summary>
        private sealed class SkyPillar : CardScript
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && target != null && !src.State.IsActive(target);
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                var a = info.Attacker;
                if (target == null || a == null || a.Owner == target.Owner || src.State.IsActive(target)) return damage;
                return 0;
            }
        }

        // ================================================================== Energy

        /// <summary>Rainbow Energy: provides every type of Energy, 1 at a time; when attached from the hand, put 1 damage counter on that Pokémon.</summary>
        private sealed class RainbowEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon != null) ctx.Engine.PlaceDamageCounters(ctx.Pokemon, 1, "Rainbow Energy");
            }
        }
    }
}
