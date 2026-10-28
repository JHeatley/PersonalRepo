using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// v154: Japanese cards not yet in English (your choice: their own set codes and Japanese numbers, labelled "Japan, not yet in
    /// English", Standard now; they become prints when the English cards show up). Storm Emeralda (M6, the Japanese Delta Reign)
    /// and the ex Starter Set Zorua &amp; Zoroark ex (MEZ). Card, attack and Ability names are translations until the English
    /// cards are out (Pokémon names are the English ones).
    /// </summary>
    public static class CardsM6
    {
        public static void Register(CardScriptRegistry r)
        {
            // MEZ (6 and 20 Zorua are prints of DLR-112).
            r.RegisterByKey("MEZ-3", new CardsMEG.ChargeSelfMon("Shadow Cloak", EnergyType.Darkness, 2));
            r.RegisterByKey("MEZ-7", new ComboMon(new CardsASR1.TimesMon("Riot Beat", c => c.MyPokemon().Count),
                                                  new LockAfterAttackMon("Slash Down", true)));

            // M6 (85 Inkay and 96 Talonflame ex are prints; 104 is a print of Grow Grass Energy POR-86, 105 of Nitro Fire
            // Energy CRI-86).
            r.RegisterByKey("M6-29", new PlainMon());
            r.RegisterByKey("M6-31", new ChaosCrawlerMon());
            r.RegisterByKey("M6-32", new PlainMon());
            r.RegisterByKey("M6-47", new KnockOffMon("Knock Off"));
            r.RegisterByKey("M6-60", new TailsNothingMon("Ambush"));
            r.RegisterByKey("M6-61", new NoRetreatMon("Clutch"));
            r.RegisterByKey("M6-62", new TalonflameExM6());
            r.RegisterByName("Delicious Rice Ball", new DeliciousRiceBall());
            r.RegisterByName("Mega Rayquaza Cap", new MegaRayquazaCap());
        }

        /// <summary>"Discard a random card from your opponent's hand."</summary>
        private sealed class KnockOffMon : Mon
        {
            private readonly string attack;

            public KnockOffMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.DiscardRandomFromOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>
        /// Chaos Crawler (Giratina M6 31): during the opponent's next turn this Pokémon takes no damage from attacks (effects still
        /// apply); it can't be used if any of your Pokémon used Chaos Crawler during your last turn.
        /// </summary>
        private sealed class ChaosCrawlerMon : Mon
        {
            private const string Crawler = "Chaos Crawler";

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != Crawler) return true;
                int turn;
                return !(ctx.Me.NamedLimitUsedOnTurn.TryGetValue(Crawler, out turn) && turn == ctx.MyLastTurn);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Crawler) return false;
                ctx.MarkNamedLimit(Crawler);
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (" + Crawler + ")",
                                                          ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>
        /// Talonflame ex (M6 62). Excite Dive (from your hand): once during your turn, if you have a Colorless Mega Evolution
        /// Pokémon ex in play, put this card onto your Bench. Talon Hunt: you may search your deck for up to 2 cards.
        /// </summary>
        private sealed class TalonflameExM6 : Mon
        {
            private static string Limit(EffectContext ctx) { return "Excite Dive#" + ctx.Card.Id; }

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && !ctx.NamedLimitUsedThisTurn(Limit(ctx))
                       && ctx.BenchSpace > 0 && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def)
                       && ctx.Me.InPlay().Any(m => m.Def.IsMegaEx && m.Def.HasType(EnergyType.Colorless));
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!CanUseFromHand(ctx)) return;
                ctx.MarkNamedLimit(Limit(ctx));
                ctx.Me.Hand.Remove(ctx.Card);
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Talon Hunt") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Talon Hunt: search your deck for up to 2 cards and put them into your hand?"))
                    ctx.SearchToHand(d => true, 2, "Talon Hunt: choose up to 2 cards to put into your hand", false);
                return true;
            }
        }

        /// <summary>
        /// Delicious Rice Ball (Item): heal 30 from your Active Pokémon, and 30 more for each other Delicious Rice Ball in your
        /// discard pile. Like other healing cards, it needs a damaged Active Pokémon.
        /// </summary>
        private sealed class DeliciousRiceBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && active.DamageCounters > 0 && !ctx.Engine.HealingBlocked(active);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                int others = ctx.Me.Discard.Count(c => c != ctx.Card && c.Def.Name == "Delicious Rice Ball");
                ctx.Heal(active, 30 + 30 * others);
            }
        }

        /// <summary>
        /// Mega Rayquaza Cap (Tool): the Pokémon it's attached to can use Delta Gift: for each of your Pokémon with a Mega Rayquaza
        /// Cap attached, a Basic Energy card from your deck is attached to it. It stays attached (it isn't a Technical Machine).
        /// </summary>
        private sealed class MegaRayquazaCap : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var capped = ctx.MyPokemon(m => m.Tools.Any(t => t.Def.Name == "Mega Rayquaza Cap"));
                if (capped.Count == 0 || ctx.Me.Deck.Count == 0) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, capped.Count,
                                             "Delta Gift: choose a Basic Energy card for each of your Pokémon with a Mega Rayquaza Cap (up to " + capped.Count + ")");
                var waiting = new List<PokemonInPlay>(capped);
                foreach (var energy in found)
                {
                    waiting.RemoveAll(m => !ctx.InPlay(m));
                    if (waiting.Count == 0)
                    {
                        ctx.Me.Deck.Add(energy);
                        continue;
                    }
                    var target = waiting.Count == 1 ? waiting[0]
                        : ctx.ChoosePokemon(waiting, "Delta Gift: attach " + energy.Def.Name + " to which of your Pokémon with a Mega Rayquaza Cap?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(energy, target))
                    {
                        if (!ctx.Me.Deck.Contains(energy)) ctx.Me.Deck.Add(energy);
                        continue;
                    }
                    waiting.Remove(target);
                }
                ctx.ShuffleMyDeck();
            }
        }
    }
}
