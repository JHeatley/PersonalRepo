using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Guardians Rising (GRI, v89), part 2: Lightning and Psychic (GRI 40-61). Tapu Koko-GX and Tapu Lele-GX were already in the pool
    /// and keep their scripts; this Mimikyu is a Psychic print of Team Up's Fairy one and shares its script.
    /// </summary>
    public static class CardsGRI2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("GRI-40", new RockPolishMon());
            r.RegisterByKey("GRI-41", new ComboMon(new FlipBonusOrRecoilMon("Thunder Punch", 20, 20), new Cards30CC.BenchHitMon("Electrobullet", 20)));
            r.RegisterByKey("GRI-42", new ComboMon(new RockWreckerMon(), new CardsVIV2.HeavySlamMon()));
            r.RegisterByKey("GRI-43", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("GRI-44", new ComboMon(new FlipBonusMon("Quick Attack", 20), new CardsASR1.BenchRecoilMon("Raging Thunder", 30)));
            r.RegisterByKey("GRI-45", new ComboMon(new AttachDiscardEnergyToSelfMon("Charge Beam"), new CardsTWM1.SelfDiscardMon("Super Zap Cannon", 2),
                new CardsMEW1.SpreadBenchMon("Gigatron-GX", 60)));
            r.RegisterByKey("GRI-46", new CardsRCL1.SetupBoostMon("Feather Dance", "Pom-Pom Punch", 80));
            // Psychic
            r.RegisterByKey("GRI-48", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("GRI-49", new ComboMon(new CardsPOR.MemoryLockMon("Amnesia"),
                new CardsFST2.ConditionBonusMon("Facade", 80, c => c.Pokemon.Has(SpecialCondition.Burned) || c.Pokemon.Has(SpecialCondition.Poisoned))));
            r.RegisterByKey("GRI-50", new CardsSSP2.StompOffMon("Stomp Off", 1));
            r.RegisterByKey("GRI-51", new ComboMon(new CardsASR1.TimesMon("Trashalanche", ItemsInOpponentsDiscard), new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray")));
            r.RegisterByKey("GRI-52", new CardsLOR1.CountersAnyWayMon("Blown Kiss", 1));
            r.RegisterByKey("GRI-53", new ConditionAttackMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("GRI-54", new ComboMon(new TractorbeamMon(30),
                new CardsFST2.ConditionBonusMon("Link Blast", 80, c => c.Defending != null && c.EnergyCount(c.Pokemon) == c.EnergyCount(c.Defending))));
            r.RegisterByKey("GRI-55", new VitalDanceOricorio());
            r.RegisterByKey("GRI-56", new ComboMon(new SupernaturalDanceMon(), new RevelationDanceMon()));
            r.RegisterByKey("GRI-57", new ComboMon(new CoinsMon("Spike Cannon", 4), new CardsCIN2.SeverePoisonMon("Super Intense Poison", 10), new CardsCRZ2.CrystalStarMon("Total Shelter-GX")));
            CardsUPR1.Same(r, "GRI-58", "TEU-112"); // Filch and Copycat; only this attack's own cost is needed (Compendium, 2019-01-31)
            r.RegisterByKey("GRI-59", new DhelmiseGRI());
            r.RegisterByKey("GRI-61", new ComboMon(new CardsASR1.TimesMon("Shatter Shot", c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Psychic)),
                new CardsGRI1.MoveAllEnergyToBenchMon("Wings of the Moone")));
        }

        /// <summary>Item cards in the opponent's discard pile; Pokémon Tools are not Item cards (your ruling, v90).</summary>
        internal static int ItemsInOpponentsDiscard(EffectContext ctx) { return ctx.Opp.Discard.Count(c => c.Def.IsItem); }

        // ================================================================== Lightning

        /// <summary>Rock Polish: during your next turn, this Pokémon has no Retreat Cost.</summary>
        private sealed class RockPolishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Polish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var fx = new FreeRetreatEffect(me.Name + " has no Retreat Cost during your next turn (Rock Polish)", ctx.Player, me.Uid, ctx.Turn + 2);
                fx.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>The Pokémon has no Retreat Cost while the effect lasts.</summary>
        public sealed class FreeRetreatEffect : ContinuousEffect
        {
            public FreeRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.Uid == TargetUid ? 0 : cost;
            }
        }

        /// <summary>"Flip a coin; heads, N more damage; tails, this Pokémon does M damage to itself" (Thunder Punch).</summary>
        internal sealed class FlipBonusOrRecoilMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly int recoil;

            public FlipBonusOrRecoilMon(string attack, int bonus, int recoil)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Flip())
                {
                    ctx.DamageDefending(atk.Damage + bonus);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(recoil);
                return true;
            }
        }

        /// <summary>Electromagnetic Rock Wrecker: flip a coin for each Lightning Energy attached to this Pokémon; 80 damage for each heads.</summary>
        private sealed class RockWreckerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electromagnetic Rock Wrecker") return false;
                int coins = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Lightning);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>"Attach an Energy card from your discard pile to this Pokémon" (Charge Beam).</summary>
        internal sealed class AttachDiscardEnergyToSelfMon : Mon
        {
            private readonly string attack;

            public AttachDiscardEnergyToSelfMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0 || !ctx.InPlay(me)) return true;
                var card = pile.Count == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, attack + ": attach which Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Tractorbeam (Gothitelle 30, Hydreigon 40): switch 1 of the opponent's Benched Pokémon with their Active Pokémon, then the
        /// damage to the new Active Pokémon (like Hunt). With no Benched Pokémon there's no switch and no damage (Compendium,
        /// 2017-05-04).
        /// </summary>
        internal sealed class TractorbeamMon : Mon
        {
            private readonly int amount;

            public TractorbeamMon(int amount) { this.amount = amount; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tractorbeam") return false;
                if (ctx.Opp.Bench.Count == 0) return Nothing(ctx);
                if (ctx.GustOpponent("Tractorbeam: choose 1 of your opponent's Benched Pokémon to switch in") != null) ctx.DamageDefending(amount);
                return true;
            }
        }

        /// <summary>Vital Dance: when you play this Pokémon from your hand onto your Bench, you may search your deck for up to 2 basic Energy cards.</summary>
        private sealed class VitalDanceOricorio : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Vital Dance", ctx.Me.Deck.Count > 0, "Use Vital Dance? Search your deck for up to 2 basic Energy cards.")) return;
                ctx.SearchToHand(d => d.IsBasicEnergy, 2, "Vital Dance: search your deck for up to 2 basic Energy cards");
            }
        }

        /// <summary>Supernatural Dance: for each Pokémon in the opponent's discard pile, put 1 damage counter on their Pokémon in any way you like.</summary>
        private sealed class SupernaturalDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supernatural Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Opp.Discard.Count(c => c.Def.IsPokemon);
                if (n > 0) ctx.PlaceCountersAnyWay(n, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Revelation Dance: does nothing if there's no Stadium card in play.</summary>
        private sealed class RevelationDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Revelation Dance") return false;
                if (ctx.State.Stadium == null) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Steelworker: your Metal Pokémon's attacks do 10 more damage to the opponent's Active Pokémon (before Weakness and Resistance); several stack. Anchor Shot: no retreat.</summary>
        private sealed class DhelmiseGRI : Mon
        {
            private static readonly Mon Anchor = new NoRetreatMon("Anchor Shot");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(attacker, EnergyType.Metal) ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Anchor.HandleAttack(ctx, atk); }
        }
    }
}
