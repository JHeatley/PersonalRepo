using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sun &amp; Moon (SUM, v90), part 1: Grass and Fire (SUM 1-27). Rowlet, Dartrix and Decidueye-GX (Hidden Fates Shiny Vault) and
    /// Incineroar-GX (Guardians Rising) were already in the pool and keep their scripts; this Morelull plays like Ultra Prism's.
    /// </summary>
    public static class CardsSUM1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("SUM-1", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("SUM-2", new CardsSP1.FlipNoDamageMon("Iron Defense"));
            r.RegisterByKey("SUM-3", new ComboMon(new ParalyzeFlipMon("Psy Bolt"), new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("SUM-5", new ComboMon(new FuryCutterTableMon("Fury Cutter", 20, 60, 120), new CardsTWM1.HealSelfMon("Mushroom Drain", 30)));
            r.RegisterByKey("SUM-6", new RoofFlingMon());
            r.RegisterByKey("SUM-7", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("SUM-8", new StruggleBugMon());
            r.RegisterByKey("SUM-11", new ComboMon(new FlipBonusMon("Leaf Blade", 30), new RecoilMon("Brave Bird", 20)));
            r.RegisterByKey("SUM-14", new CardsVIV1.SynthesisMon());
            r.RegisterByKey("SUM-15", new ComboMon(new FlowerSupplyMon(), new CardsTWM1.HealSelfMon("Solar Blade", 30),
                new CardsASR1.TimesMon("Chloroscythe-GX", c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Grass))));
            CardsUPR1.Same(r, "SUM-16", "UPR-92"); // Flickering Spores (Asleep), Ram
            r.RegisterByKey("SUM-17", new ShiinoticSUM());
            r.RegisterByKey("SUM-18", new CardsPOR.SweetScentMon("Sweet Scent", 30));
            r.RegisterByKey("SUM-19", new ComboMon(new CardsPOR.SweetScentMon("Sweet Scent", 30), new FlipBonusMon("Stomp", 20)));
            r.RegisterByKey("SUM-20", new TsareenaSUM());
            // Fire
            r.RegisterByKey("SUM-21", new RecoilMon("Take Down", 20));
            r.RegisterByKey("SUM-22", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new DiscardTypeFromSelfMon("Firestorm", EnergyType.Fire, 3)));
            r.RegisterByKey("SUM-23", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("SUM-25", new ComboMon(new CoinsMon("Fury Swipes", 3), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("SUM-26", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new CoinsMon("Darkest Lariat", 2)));
        }

        // ================================================================== Shared helpers

        /// <summary>"Flip 3 coins. If 1 of them is heads, N1 more damage; if 2 are, N2 more; if all are, N3 more" (Fury Cutter).</summary>
        internal sealed class FuryCutterTableMon : Mon
        {
            private readonly string attack;
            private readonly int one;
            private readonly int two;
            private readonly int three;

            public FuryCutterTableMon(string attack, int one, int two, int three)
            {
                this.attack = attack;
                this.one = one;
                this.two = two;
                this.three = three;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int heads = ctx.FlipCoins(3);
                int bonus = heads >= 3 ? three : heads == 2 ? two : heads == 1 ? one : 0;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>"Discard N [T] Energy from this Pokémon" (Firestorm, Dragon Wave uses its own).</summary>
        internal sealed class DiscardTypeFromSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;

            public DiscardTypeFromSelfMon(string attack, EnergyType type, int count)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, count, attack + ": discard " + count + " " + type + " Energy from " + me.Name, en => CardsHIF1.Provides(ctx, me, en, type));
                return true;
            }
        }

        /// <summary>Puts cards found in the deck onto your Pokémon in any way you like (each card to a Pokémon you choose).</summary>
        internal static void AttachFoundAnyWay(EffectContext ctx, List<CardInstance> found, string source)
        {
            foreach (var card in found)
            {
                var targets = ctx.MyPokemon();
                if (targets.Count == 0)
                {
                    ctx.Me.Deck.Add(card);
                    continue;
                }
                var target = ctx.ChoosePokemon(targets, source + ": attach " + card.Def.Name + " to which of your Pokémon?");
                ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        // ================================================================== Grass

        /// <summary>Roof Fling: flip a coin; heads, put the opponent's Active Pokémon and all cards attached to it into their hand.</summary>
        private sealed class RoofFlingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roof Fling") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>Struggle Bug: move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon (you choose both).</summary>
        private sealed class StruggleBugMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Struggle Bug") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Energy.Count == 0) return true;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var energy = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Struggle Bug: move which Energy from " + defending.Name + "?")[0];
                var dest = ctx.ChoosePokemon(bench, "Struggle Bug: move " + energy.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (dest != null) ctx.Engine.MoveEnergy(defending, dest, energy);
                return true;
            }
        }

        /// <summary>Flower Supply: attach 2 basic Energy cards from your discard pile to your Pokémon in any way you like.</summary>
        private sealed class FlowerSupplyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flower Supply") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, 2, (card, mon) => true, "Flower Supply: attach basic Energy cards to your Pokémon", false);
                return true;
            }
        }

        /// <summary>Illuminate: once during your turn, search your deck for a Grass Pokémon and put it into your hand. Flickering Spores: Asleep.</summary>
        private sealed class ShiinoticSUM : Mon
        {
            private static readonly Mon Spores = new ConditionAttackMon("Flickering Spores", SpecialCondition.Asleep);

            protected override string Activated { get { return "Illuminate"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Grass), 1, "Illuminate: choose a Grass Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spores.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Queenly Majesty: when you play this Pokémon from your hand to evolve, you may have your opponent reveal their hand; then you
        /// discard a card from it. Trop Kick: heal 20 damage and remove all Special Conditions from this Pokémon.
        /// </summary>
        private sealed class TsareenaSUM : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Queenly Majesty", ctx.Opp.Hand.Count > 0, "Use Queenly Majesty? Your opponent reveals their hand and you discard a card from it.")) return;
                CardsBUS4.RevealHand(ctx, "Queenly Majesty");
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "Queenly Majesty: discard which card from your opponent's hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trop Kick") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Heal(me, 20);
                ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }
    }
}
