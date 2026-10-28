using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Pokémon Rumble (RM 1-16, 2009; v148): a 16-card promo set, every card new to the pool and checked against Limitless. Unlimited
    /// only. Only 5 attacks have effects; the rest do their printed damage.
    /// </summary>
    public static class CardsRM
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RM-1", new CardsDAA1.DrainMon("Giga Drain"));
            r.RegisterByKey("RM-2", new PlainMon());
            r.RegisterByKey("RM-3", new PlainMon());
            r.RegisterByKey("RM-4", new HeatBoilerMon());
            r.RegisterByKey("RM-5", new CardsE3B.HealSelfMon("Spiral Drain", 10));
            r.RegisterByKey("RM-6", new RecoilMon("Thrash Tackle", 20));
            r.RegisterByKey("RM-7", new RecoilMon("Volt Tackle", 10));
            r.RegisterByKey("RM-8", new PlainMon());
            r.RegisterByKey("RM-9", new PlainMon());
            r.RegisterByKey("RM-10", new PlainMon());
            r.RegisterByKey("RM-11", new PlainMon());
            r.RegisterByKey("RM-12", new PlainMon());
            r.RegisterByKey("RM-13", new CardsE2B.RageMon());
            r.RegisterByKey("RM-14", new PlainMon());
            r.RegisterByKey("RM-15", new PlainMon());
            r.RegisterByKey("RM-16", new PlainMon());
        }

        /// <summary>Heat Boiler: 40 more against a Water Defending Pokémon.</summary>
        private sealed class HeatBoilerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heat Boiler") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && ctx.Engine.IsType(defending, EnergyType.Water) ? 40 : 0));
                return true;
            }
        }
    }
}
