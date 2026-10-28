using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Japanese M-P promos (set code MP, Japan's M-P; v151, Lebanne v151.1): cards announced in Japan that aren't out in English yet. Regulation
    /// mark J, so they're Standard-legal here (your choice).
    /// </summary>
    public static class CardsMP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MP-169", new MegaHeatranExMP());
            r.RegisterByKey("MP-171", new Lebanne());

            // v156 (134 Eevee, 140 Tynamo, 141 Patrat, 142 Special Red Card and 144 Boss's Orders are prints).
            r.RegisterByKey("MP-138", new WimpodMP());
            r.RegisterByKey("MP-139", new CoinsMon("Triple Crown Head", 3));
            r.RegisterByKey("MP-143", new Lida());
        }

        /// <summary>Wimp Out (Wimpod 138/M-P): while the opponent has any Pokémon ex in play, this Pokémon has no Retreat Cost.</summary>
        private sealed class WimpodMP : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Opp.InPlay().Any(m => m.Def.IsEx) ? 0 : cost;
            }
        }

        /// <summary>
        /// Lida (Supporter, 143/M-P; Japanese デウロ): only if any of your Mega Evolution Pokémon ex were Knocked Out during your
        /// opponent's last turn. Up to 2 Basic Energy cards from your deck, attached to 1 of your Mega Evolution Pokémon ex.
        /// </summary>
        private sealed class Lida : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.WasKnockedOutLastOpponentTurn(d => d.IsMegaEx, false);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var megas = ctx.MyPokemon(m => m.Def.IsMegaEx);
                if (megas.Count > 0 && ctx.Me.Deck.Count > 0)
                {
                    var target = ctx.ChoosePokemon(megas, "Lida: attach up to 2 Basic Energy cards to which of your Mega Evolution Pokémon ex?");
                    if (target != null)
                    {
                        var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Lida: choose up to 2 Basic Energy cards to attach to " + target.Name);
                        foreach (var energy in found)
                            if (!ctx.Engine.AttachEnergyCard(energy, target) && !ctx.Me.Deck.Contains(energy)) ctx.Me.Deck.Add(energy);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// High Density Armor: with full HP, this Pokémon takes 60 less damage from the opponent's Pokémon's attacks (after Weakness
        /// and Resistance). Gigantic Press: during your next turn, Gigantic Press does 140 more damage.
        /// </summary>
        private sealed class MegaHeatranExMP : Mon
        {
            private static readonly Mon Press = new CardsASC.NextTurnBoostMon("Gigantic Press", 140);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent || src.Pokemon.DamageCounters > 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 60;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Press.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Lebanne (Supporter, 171/M-P): flip a coin; heads, heal 150 damage from your Active Pokémon, tails 30. Like Potion, it needs
        /// a damaged Active Pokémon that can be healed.
        /// </summary>
        private sealed class Lebanne : CardScript
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
                int amount = ctx.Flip() ? 150 : 30;
                if (active.DamageCounters > 0) ctx.Heal(active, amount);
            }
        }
    }
}
