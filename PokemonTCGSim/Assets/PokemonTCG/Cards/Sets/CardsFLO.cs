using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// v155: Flowing Fists (FLO; name announced v157.1), the English set from Japan's Aura Seeker (revealed at Worlds 2026): the cards shown so
    /// far. Regulation mark J. (Japan's MEGA x MEGA Parade, M7A, has only prints of cards already in the pool so far.)
    /// </summary>
    public static class CardsFLO
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FLO-32", new SigilyphExFLO());
            r.RegisterByKey("FLO-45", new PlainMon());
            r.RegisterByKey("FLO-58", new MegaLucarioZEx());
        }

        /// <summary>
        /// Mega Protection: prevent all damage done to this Pokémon by attacks from the opponent's Mega Evolution Pokémon ex (on
        /// the Bench too). Power Cyclone: move an Energy from this Pokémon to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class SigilyphExFLO : Mon
        {
            private static readonly Mon Cyclone = new CardsFST1.MoveEnergyToBenchMon("Power Cyclone");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return info.Attacker != null && info.Attacker.Def.IsMegaEx ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cyclone.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Aura Seeker: once during your turn, while this Pokémon is Active, a Supporter card from your deck into your hand.
        /// Dancing Fist: 120 more if the opponent's Active Pokémon is a Pokémon ex.
        /// </summary>
        private sealed class MegaLucarioZEx : Mon
        {
            private const string Power = "Aura Seeker";
            private static readonly Mon Fist = new CardsFST2.ConditionBonusMon("Dancing Fist", 120, c => c.Defending != null && c.Defending.Def.IsEx);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter, 1, Power + ": choose a Supporter card to put into your hand");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fist.HandleAttack(ctx, atk); }
        }
    }
}
