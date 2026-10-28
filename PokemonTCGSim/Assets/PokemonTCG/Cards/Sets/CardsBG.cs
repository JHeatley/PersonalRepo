using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Best of Game promos (BG 1-9, 2002; v145). Unlimited only. Electabuzz, Hitmonchan, Professor Elm, Rocket's Mewtwo and Rocket's
    /// Hitmonchan are prints of pool cards (Base Set, Neo Genesis, Gym Challenge, Gym Heroes); Rocket's Scizor, Rocket's Sneasel,
    /// Dark Ivysaur and Dark Venusaur are new. Your ruling: Vine Pull is a Poké-Body, so Toxic Gas and Scare don't stop it.
    /// </summary>
    public static class CardsBG
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BG-4", new RocketsScizorBG());
            r.RegisterByKey("BG-5", new ComboMon(new EntrapMon(), new CoinsMon("Continuous Scratch", 4)));
            r.RegisterByKey("BG-6", new DarkIvysaurBG());
            r.RegisterByKey("BG-7", new HorridPollenMon());
        }

        /// <summary>Focus: during your next turn, Agility's base damage is doubled. Agility: heads, attacks can't touch it during the opponent's next turn.</summary>
        private sealed class RocketsScizorBG : Mon
        {
            private const string Marker = "Focus turn";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Agility") return false;
                int turn;
                bool focused = me != null && me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn;
                bool heads = ctx.Flip();
                ctx.DamageDefending(focused ? atk.Damage * 2 : atk.Damage);
                if (heads) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Entrap: the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class EntrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entrap") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>
        /// Vine Pull (Poké-Body, your ruling): once during your turn, when this Pokémon retreats, switch 1 of the opponent's Benched
        /// Pokémon (you choose) with their Active Pokémon. Fury Strikes: the opponent puts 3 markers on their Pokémon as they like;
        /// 10 damage to each Pokémon for each marker on it (no Weakness or Resistance).
        /// </summary>
        private sealed class DarkIvysaurBG : Mon
        {
            private const string Power = "Vine Pull";

            public override void OnRetreated(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !ctx.IsMyTurn || ctx.AbilityUsedThisTurn(Power) || ctx.GustTargets().Count == 0) return;
                ctx.MarkAbilityUsed(Power);
                Announce(ctx, Power);
                ctx.GustOpponent(Power + ": choose 1 of your opponent's Benched Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Strikes") return false;
                var theirs = ctx.OpponentPokemon();
                if (theirs.Count == 0) return true;
                var markers = new Dictionary<PokemonInPlay, int>();
                for (int i = 0; i < 3; i++)
                {
                    var pick = ctx.Engine.ChoosePokemon(ctx.Opponent, theirs, "Fury Strikes: put marker " + (i + 1) + " of 3 on which of your Pokémon?", ctx.Source);
                    if (pick == null) pick = theirs[0];
                    int n;
                    markers.TryGetValue(pick, out n);
                    markers[pick] = n + 1;
                }
                foreach (var kv in markers.ToList())
                {
                    ctx.Log("Fury Strikes: " + kv.Value + (kv.Value == 1 ? " marker" : " markers") + " on " + kv.Key.Name + ".");
                    if (ctx.InPlay(kv.Key)) ctx.DamageWith(kv.Key, 10 * kv.Value, false, false, false);
                }
                return true;
            }
        }

        /// <summary>Horrid Pollen: 2 coins; 1 heads, Asleep and Poisoned; 2 heads, Confused and Poisoned; 2 tails, Paralyzed and Poisoned.</summary>
        private sealed class HorridPollenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Horrid Pollen") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, heads == 1 ? SpecialCondition.Asleep : heads == 2 ? SpecialCondition.Confused : SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }
    }
}
