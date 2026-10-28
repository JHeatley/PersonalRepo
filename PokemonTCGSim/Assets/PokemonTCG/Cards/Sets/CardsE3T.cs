using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Skyridge Trainers and Energy (E3 119-120, 122-124, 127, 129-139, 141-144; v147). Apricorn Maker, Fisherman, Friend Ball, Lure
    /// Ball and Underground Expedition are prints of cards already in the pool (Limitless files them with those cards; same effect).
    /// Hyper Potion 127 is Skyridge's own card (the newer Hyper Potion does something else). The Miracle Spheres and Mystery Plates
    /// are Technical Machines: attached like Pokémon Tools but not Tools, discarded at the end of your turn.
    /// </summary>
    public static class CardsE3T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E3-119", new AncientRuins());
            r.RegisterByKey("E3-120", new RelicHunter());
            r.RegisterByKey("E3-122", new CrystalShard());
            r.RegisterByKey("E3-123", new DesertShaman());
            r.RegisterByKey("E3-124", new FastBall());
            r.RegisterByKey("E3-127", new HyperPotionE3());
            r.RegisterByKey("E3-129", new AttachedTM(MiracleSphere(EnergyType.Fire, EnergyType.Lightning, EnergyType.Fighting), FireForce));
            r.RegisterByKey("E3-130", new AttachedTM(MiracleSphere(EnergyType.Fire, EnergyType.Water, EnergyType.Psychic), FreezingForce));
            r.RegisterByKey("E3-131", new AttachedTM(MiracleSphere(EnergyType.Grass, EnergyType.Water, EnergyType.Lightning), ThunderForce));
            r.RegisterByKey("E3-132", new MirageStadium());
            r.RegisterByKey("E3-133", new AttachedTM(null, DesertBurn));
            r.RegisterByKey("E3-134", new AttachedTM(null, StoneCrush));
            r.RegisterByKey("E3-135", new AttachedTM(null, RetroCave));
            r.RegisterByKey("E3-136", new AttachedTM(null, HealingOasis));
            r.RegisterByKey("E3-137", new MysteryZone());
            r.RegisterByKey("E3-138", new Oracle());
            r.RegisterByKey("E3-139", new StarPiece());
            r.RegisterByKey("E3-141", new UndergroundLake());
            r.RegisterByKey("E3-142", new BounceEnergy());
            r.RegisterByKey("E3-143", new CycloneEnergy());
            r.RegisterByKey("E3-144", new RetroEnergy());
        }

        // ================================================================== Technical Machines

        /// <summary>
        /// An old Technical Machine: attached to 1 of your Pokémon (that the TM allows, or any), which may use its attack; discarded
        /// at the end of your turn. It isn't a Pokémon Tool (like Expedition's Multi Technical Machine 01).
        /// </summary>
        internal sealed class AttachedTM : CardScript
        {
            private readonly Func<EffectContext, PokemonInPlay, bool> canAttach;
            private readonly Action<EffectContext, AttackDef> resolve;

            public AttachedTM(Func<EffectContext, PokemonInPlay, bool> canAttach, Action<EffectContext, AttackDef> resolve)
            {
                this.canAttach = canAttach;
                this.resolve = resolve;
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && (canAttach == null || canAttach(ctx, target)); }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override bool DiscardedAtEndOfOwnersTurn { get { return true; } }

            public override bool IsAttachedNonTool { get { return true; } }

            public override void ResolveAttack(EffectContext ctx, int attackIndex) { resolve(ctx, ctx.Attack(attackIndex)); }
        }

        /// <summary>Miracle Sphere: only on an Evolved Pokémon of 1 of its 3 types.</summary>
        private static Func<EffectContext, PokemonInPlay, bool> MiracleSphere(EnergyType a, EnergyType b, EnergyType c)
        {
            return (ctx, mon) => CardsE2.IsEvolved(mon) && (ctx.Engine.IsType(mon, a) || ctx.Engine.IsType(mon, b) || ctx.Engine.IsType(mon, c));
        }

        private static bool Has(PokemonInPlay mon, EnergyType a, EnergyType b) { return CardsE3.HasBasicOf(mon, a) && CardsE3.HasBasicOf(mon, b); }

        /// <summary>Fire Force: Fire and Lightning basic Energy, Confused; Fire and Fighting, 10 more and 3 damage counters off the attacker.</summary>
        private static void FireForce(EffectContext ctx, AttackDef atk)
        {
            var me = ctx.Pokemon;
            var defending = ctx.Defending;
            bool confuse = Has(me, EnergyType.Fire, EnergyType.Lightning);
            bool power = Has(me, EnergyType.Fire, EnergyType.Fighting);
            ctx.DamageDefending(atk.Damage + (power ? 10 : 0));
            if (confuse && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
            if (power && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 30);
        }

        /// <summary>Freezing Force: Fire and Water basic Energy, 10 more and Burned; Water and Psychic, an Energy card from the Defending Pokémon into the opponent's deck.</summary>
        private static void FreezingForce(EffectContext ctx, AttackDef atk)
        {
            var me = ctx.Pokemon;
            var defending = ctx.Defending;
            bool burn = Has(me, EnergyType.Fire, EnergyType.Water);
            bool shuffle = Has(me, EnergyType.Water, EnergyType.Psychic);
            ctx.DamageDefending(atk.Damage + (burn ? 10 : 0));
            if (!TefPokemon.StillDefending(ctx, defending)) return;
            if (burn) ctx.ApplyCondition(defending, SpecialCondition.Burned);
            if (shuffle && defending.Energy.Count > 0 && ctx.CanAffect(defending))
            {
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Freezing Force: choose an Energy card attached to " + defending.Name)[0];
                ShuffleEnergyIntoOwnersDeck(ctx, defending, new[] { card });
            }
        }

        /// <summary>Thunder Force: Grass and Lightning basic Energy, Asleep and Poisoned; Water and Lightning, 10 more and a damage counter on each of the opponent's Benched Pokémon.</summary>
        private static void ThunderForce(EffectContext ctx, AttackDef atk)
        {
            var me = ctx.Pokemon;
            var defending = ctx.Defending;
            bool sleep = Has(me, EnergyType.Grass, EnergyType.Lightning);
            bool bench = Has(me, EnergyType.Water, EnergyType.Lightning);
            ctx.DamageDefending(atk.Damage + (bench ? 10 : 0));
            if (sleep && TefPokemon.StillDefending(ctx, defending))
            {
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }
            if (bench)
                foreach (var mon in ctx.OpponentBench())
                    if (ctx.InPlay(mon) && ctx.CanAffect(mon)) ctx.PlaceCounters(mon, 1);
        }

        private static void ShuffleEnergyIntoOwnersDeck(EffectContext ctx, PokemonInPlay mon, IEnumerable<CardInstance> cards)
        {
            var list = cards.Where(c => mon.Energy.Contains(c)).ToList();
            if (list.Count == 0) return;
            ctx.Engine.ShuffleEnergyIntoDeck(mon, list);
        }

        /// <summary>Desert Burn: opponent with 5 or more Prize cards, a Trainer card from your deck into your hand; with only 1, the Defending Pokémon is Burned, Paralyzed and Poisoned.</summary>
        private static void DesertBurn(EffectContext ctx, AttackDef atk)
        {
            int prizes = ctx.Opp.Prizes.Count;
            if (prizes >= 5 && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsTrainer, 1, "Desert Burn: choose a Trainer card");
            else if (prizes == 1)
            {
                var defending = ctx.Defending;
                if (defending == null) return;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Stone Crush: opponent with 5 or more Prize cards, draw 3; with only 1, 2 Energy cards from the Defending Pokémon (1 if only 1) into their deck.</summary>
        private static void StoneCrush(EffectContext ctx, AttackDef atk)
        {
            int prizes = ctx.Opp.Prizes.Count;
            if (prizes >= 5) ctx.Draw(3);
            else if (prizes == 1)
            {
                var defending = ctx.Defending;
                if (defending == null || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return;
                int n = Math.Min(2, defending.Energy.Count);
                var picks = defending.Energy.Count == n ? defending.Energy.ToList() : ctx.ChooseCards(defending.Energy.ToList(), n, n, "Stone Crush: choose " + n + " Energy cards attached to " + defending.Name);
                ShuffleEnergyIntoOwnersDeck(ctx, defending, picks);
            }
        }

        /// <summary>Retro Cave: opponent with 5 or more Prize cards, shuffle your hand into your deck and draw 6; with exactly 2, the top card of 1 of their Evolved Pokémon goes on the bottom of their deck.</summary>
        private static void RetroCave(EffectContext ctx, AttackDef atk)
        {
            int prizes = ctx.Opp.Prizes.Count;
            if (prizes >= 5)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(6);
            }
            else if (prizes == 2)
            {
                var evolved = ctx.OpponentPokemon(m => CardsE2.IsEvolved(m) && ctx.CanAffect(m));
                if (evolved.Count == 0) return;
                var target = ctx.ChoosePokemon(evolved, "Retro Cave: choose 1 of your opponent's Evolved Pokémon");
                if (target != null) ctx.Engine.DevolveToBottomOfDeck(target, 1);
            }
        }

        /// <summary>Healing Oasis: opponent with 5 or more Prize cards, up to 3 basic Energy cards from your deck into your hand; with exactly 2, all damage counters off 1 of your Pokémon.</summary>
        private static void HealingOasis(EffectContext ctx, AttackDef atk)
        {
            int prizes = ctx.Opp.Prizes.Count;
            if (prizes >= 5 && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsBasicEnergy, 3, "Healing Oasis: choose up to 3 basic Energy cards");
            else if (prizes == 2)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Healing Oasis: remove all damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
            }
        }

        // ================================================================== Supporters

        /// <summary>Relic Hunter: up to 2 Supporter and/or Stadium cards from your deck into your hand.</summary>
        private sealed class RelicHunter : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter || d.IsStadiumCard, 2, "Relic Hunter: choose up to 2 Supporter and/or Stadium cards");
            }
        }

        /// <summary>Desert Shaman: each player shuffles their hand into their deck and draws up to 4 cards (you first).</summary>
        private sealed class DesertShaman : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    ctx.Engine.ShuffleHandIntoDeck(player);
                    int max = Math.Min(4, ctx.State.Players[player].Deck.Count);
                    int n = max == 0 ? 0 : ctx.Engine.AskNumber(player, 0, max, "Desert Shaman: draw how many cards (up to 4)?", ctx.Source);
                    if (n > 0) ctx.Engine.DrawCards(player, n);
                }
            }
        }

        /// <summary>Oracle: choose 2 cards from your deck, shuffle the rest and put the 2 on top in any order.</summary>
        private sealed class Oracle : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                int n = Math.Min(2, deck.Count);
                var picks = deck.Count == n ? deck.ToList() : ctx.ChooseCards(deck.ToList(), n, n, "Oracle: choose 2 cards to put on top of your deck");
                foreach (var c in picks) deck.Remove(c);
                ctx.ShuffleMyDeck();
                deck.InsertRange(0, picks);
                if (picks.Count > 1) CardsE2.ReorderTop(ctx, ctx.Player, picks.Count, "Oracle");
            }
        }

        // ================================================================== Items

        /// <summary>Fast Ball: reveal cards from the top of your deck until an Evolution card; put it into your hand and shuffle the rest back.</summary>
        private sealed class FastBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                int idx = deck.FindIndex(c => c.Def.IsEvolution);
                var revealed = deck.Take(idx < 0 ? deck.Count : idx + 1).ToList();
                if (revealed.Count > 0) ctx.Engine.ShowCards(ctx.Opponent, revealed, "Fast Ball", "Fast Ball: your opponent revealed these cards", ctx.Source);
                if (idx >= 0) ctx.DeckToHand(new[] { deck[idx] });
                else ctx.Log("Fast Ball: no Evolution card was revealed.");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Hyper Potion (Skyridge): discard 1 or 2 basic Energy cards from 1 of your Pokémon; 1, up to 3 damage counters off it; 2, up to 5.</summary>
        private sealed class HyperPotionE3 : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && m.Energy.Any(e => e.Def.IsBasicEnergy)); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Hyper Potion: choose 1 of your Pokémon");
                if (target == null) return;
                var basics = target.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var picks = ctx.ChooseCards(basics, 1, Math.Min(2, basics.Count), "Hyper Potion: discard 1 basic Energy card (3 counters off) or 2 (5 counters off)");
                int n = 0;
                foreach (var card in picks) if (ctx.Engine.DiscardEnergy(target, card)) n++;
                if (n > 0 && target.DamageCounters > 0) ctx.Heal(target, n == 2 ? 50 : 30);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Crystal Shard: its Pokémon is Colorless; discarded at the end of a turn in which it attacked.</summary>
        private sealed class CrystalShard : CardScript
        {
            private const string Marker = "Crystal Shard attacked";

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                types.Clear();
                types.Add(EnergyType.Colorless);
            }

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                if (src.Pokemon != null && attacker == src.Pokemon) src.Pokemon.AbilityUsedOnTurn[Marker] = src.Turn;
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                int turn;
                if (holder == null || !holder.AbilityUsedOnTurn.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                holder.AbilityUsedOnTurn.Remove(Marker);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>
        /// Star Piece: between turns, if its Pokémon is Benched with 2 or more damage counters, evolve it with an Evolution card from
        /// your deck (shuffle afterward), then discard Star Piece.
        /// </summary>
        private sealed class StarPiece : CardScript
        {
            public override void OnBetweenTurns(EffectContext src, bool startOfTurn)
            {
                var holder = src.Pokemon;
                if (startOfTurn || holder == null || !holder.Tools.Contains(src.Card)) return;
                if (!src.Me.Bench.Contains(holder) || holder.DamageCounters < 2) return;
                src.Log("Star Piece: " + holder.Name + " is Benched with " + holder.DamageCounters + " damage counters.");
                if (src.Me.Deck.Count > 0) CardsEVS1.EvolveFromDeck(src, holder, "Star Piece");
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        // ================================================================== Stadiums

        /// <summary>Ancient Ruins: once during each player's turn, if they haven't played a Supporter, they may show their hand; with no Supporter there, they draw a card.</summary>
        private sealed class AncientRuins : CardScript
        {
            private const string Limit = "Ancient Ruins";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && !ctx.Me.SupporterPlayedThisTurn && ctx.Me.Deck.Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count > 0) ctx.Engine.ShowCards(ctx.Opponent, hand, "Ancient Ruins", "Ancient Ruins: your opponent's hand", ctx.Source);
                if (hand.Any(c => c.Def.IsSupporter))
                {
                    ctx.Log("Ancient Ruins: there's a Supporter card in the hand, so no card is drawn.");
                    return false;
                }
                ctx.Draw(1);
                return false;
            }
        }

        /// <summary>Mirage Stadium: whenever a player tries to retreat during their turn, they flip; tails, that Pokémon can't retreat this turn (nothing is discarded).</summary>
        private sealed class MirageStadium : CardScript
        {
            public override bool ForcesRetreatFlip(EffectContext src, int player) { return true; }
        }

        /// <summary>
        /// Mystery Zone: once during each player's turn, with an Evolution card in their hand, they may put a basic Energy card from their
        /// deck into their hand, then shuffle an Evolution card from their hand into their deck.
        /// </summary>
        private sealed class MysteryZone : CardScript
        {
            private const string Limit = "Mystery Zone";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.Hand.Any(c => c.Def.IsEvolution); }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsBasicEnergy, 1, "Mystery Zone: choose a basic Energy card");
                var evolutions = ctx.Me.Hand.Where(c => c.Def.IsEvolution).ToList();
                if (evolutions.Count == 0) return false;
                var card = ctx.ChooseCards(evolutions, 1, 1, "Mystery Zone: put which Evolution card from your hand into your deck?")[0];
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { card }, "Mystery Zone", "Your opponent shuffles this card into their deck.", ctx.Source);
                if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return false;
            }
        }

        /// <summary>Underground Lake: once during each player's turn, they may put an Omanyte or Kabuto from their discard pile onto their Bench (it's considered a Basic Pokémon).</summary>
        private sealed class UndergroundLake : CardScript
        {
            private const string Limit = "Underground Lake";

            private static List<CardInstance> Fossils(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon && (c.Def.Name == "Omanyte" || c.Def.Name == "Kabuto") && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.BenchSpace > 0 && Fossils(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var cards = Fossils(ctx);
                if (cards.Count == 0 || ctx.BenchSpace <= 0) return false;
                var card = ctx.ChooseCards(cards, 1, 1, "Underground Lake: put which Omanyte or Kabuto onto your Bench?")[0];
                if (!ctx.Me.Discard.Remove(card)) return false;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                if (mon != null) mon.Markers[PokemonInPlay.ConsideredBasicMarker] = 1;
                return false;
            }
        }

        // ================================================================== Special Energy

        /// <summary>Bounce Energy: [C][C]; only on a Pokémon with basic Energy attached; attached from the hand, a basic Energy card on that Pokémon goes back to your hand.</summary>
        private sealed class BounceEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return new List<int> { EnergyMask.Colorless, EnergyMask.Colorless }; }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Energy.Any(e => e.Def.IsBasicEnergy); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                var basics = mon == null ? new List<CardInstance>() : mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return;
                var card = ctx.ChooseCards(basics, 1, 1, "Bounce Energy: return which basic Energy card to your hand?")[0];
                ctx.Engine.ReturnEnergyToHand(mon, card);
            }
        }

        /// <summary>Cyclone Energy: [C]; attached from the hand to your Active Pokémon, the opponent switches their Active Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class CycloneEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon != null && ctx.PokemonIsActive && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
            }
        }

        /// <summary>Retro Energy: [C]; attached from the hand to an Evolved Pokémon, you may remove up to 2 damage counters from it and discard its top card (devolving it).</summary>
        private sealed class RetroEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (!CardsE2.IsEvolved(mon)) return;
                if (!ctx.AskYesNo("Retro Energy: remove up to 2 damage counters from " + mon.Name + " and discard its top card (devolving it)?")) return;
                if (mon.DamageCounters > 0) ctx.Heal(mon, 20);
                ctx.Engine.DevolveIntoDiscard(mon, 1);
            }
        }
    }
}
