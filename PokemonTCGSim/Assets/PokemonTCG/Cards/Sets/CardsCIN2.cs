using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crimson Invasion (CIN, v86), part 2: Lightning and Psychic (CIN 30-48). Nihilego-GX was already in the pool (Hidden Fates
    /// Shiny Vault) and keeps its script.
    /// </summary>
    public static class CardsCIN2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("CIN-30", new ComboMon(new DrawAttackMon("Pika Draw", 1), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("CIN-31", new AlolanRaichuCIN());
            r.RegisterByKey("CIN-32", new CardsSP1.FlipNoDamageMon("Defense Curl"));
            r.RegisterByKey("CIN-33", new RecoilMon("Self-Destruct", 100));
            r.RegisterByKey("CIN-34", new ComboMon(new RecoilMon("Super Electromagnetic Tackle", 50), new HeavyRockMon()));
            r.RegisterByKey("CIN-35", new ComboMon(new EnergyCatchMon(), new CardsTEU2.VoltSwitchMon("Volt Switch", EnergyType.Lightning)));
            // Psychic
            r.RegisterByKey("CIN-36", new CardsRCL1.CountersOnOneMon("Ominous Eyes", 1));
            r.RegisterByKey("CIN-37", new CardsBKT2.PainAmplifierMon("Pain Amplifier", 2));
            r.RegisterByKey("CIN-38", new GengarCIN());
            r.RegisterByKey("CIN-39", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("CIN-40", new ComboMon(new ChaosWheelMon(), new CardsASR1.TimesMon("Dark Arts", c => c.Opp.Hand.Count)));
            r.RegisterByKey("CIN-42", new GrumpigCIN());
            r.RegisterByKey("CIN-43", new BellOfSilenceMon());
            r.RegisterByKey("CIN-44", new CardsEVS1.AstonishMon("Astonish"));
            r.RegisterByKey("CIN-45", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new PumpkinBombMon()));
            r.RegisterByKey("CIN-46", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("CIN-47", new ComboMon(new SearchAttackMon("Nasty Plot", d => true, 2), new SeverePoisonMon("Severe Poison", 4)));
            r.RegisterByKey("CIN-48", new FixerOfTheForestMon());
        }

        // ================================================================== Lightning

        /// <summary>Surge Surfer: no Retreat Cost while any Stadium card is in play. Psychic: 20 more for each Energy on the opponent's Active Pokémon.</summary>
        private sealed class AlolanRaichuCIN : Mon
        {
            private static readonly Mon Psy = new CardsMEG.DefendingEnergyBonusMon("Psychic", 20);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.State.Stadium != null ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psy.HandleAttack(ctx, atk); }
        }

        /// <summary>Heavy Rock-GX: the opponent can't play any cards from their hand during their next turn.</summary>
        private sealed class HeavyRockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Rock-GX") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsTEU2.HandLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play any cards from their hand during their next turn (Heavy Rock-GX)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Energy Catch: put 3 basic Energy cards from your discard pile into your hand (as many as there are, up to 3).</summary>
        private sealed class EnergyCatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Catch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                int n = Math.Min(3, energy.Count);
                var picks = energy.Count <= n ? energy : ctx.ChooseCards(energy, n, n, "Energy Catch: put which 3 basic Energy cards into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Gnawing Curse: whenever the opponent attaches an Energy card from their hand to 1 of their Pokémon, put 2 damage counters on it. Fade to Black: Confused.</summary>
        private sealed class GengarCIN : Mon
        {
            private static readonly Mon Fade = new ConditionAttackMon("Fade to Black", SpecialCondition.Confused);

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target == null || target.Owner == src.Player) return;
                Announce(src, "Gnawing Curse");
                src.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fade.HandleAttack(ctx, atk); }
        }

        /// <summary>Chaos Wheel: the opponent can't play Pokémon Tool, Special Energy or Stadium cards from their hand during their next turn.</summary>
        internal sealed class ChaosWheelMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chaos Wheel") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new ChaosWheelEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Pokémon Tool, Special Energy or Stadium cards during their next turn (Chaos Wheel)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class ChaosWheelEffect : ContinuousEffect
        {
            private readonly int affected;

            public ChaosWheelEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card != null && (card.Def.IsTool || card.Def.IsStadium);
            }

            public override bool BlocksEnergyCard(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card != null && card.Def.IsSpecialEnergy;
            }
        }

        /// <summary>Own Tempo: this Pokémon can't be Confused. Psych Up: during your next turn, Psych Up does 60 more damage (before Weakness and Resistance).</summary>
        private sealed class GrumpigCIN : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && condition == SpecialCondition.Confused;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psych Up") return false;
                var me = ctx.Pokemon;
                int uid = me.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == "Psych Up").Sum(b => b.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                if (!ctx.InPlay(me)) return true;
                var boost = new AttackBoostEffect(me.Name + "'s Psych Up does 60 more damage during its next turn (Psych Up)", ctx.Player, uid, ctx.Turn + 2, "Psych Up", 60);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        /// <summary>Bell of Silence: the opponent can't play any Pokémon that has an Ability from their hand during their next turn.</summary>
        private sealed class BellOfSilenceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bell of Silence") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new BellOfSilenceEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Pokémon with an Ability from their hand during their next turn (Bell of Silence)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class BellOfSilenceEffect : ContinuousEffect
        {
            private readonly int affected;

            public BellOfSilenceEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksPlayingPokemon(GameEngine engine, int player, CardDef card)
            {
                return player == affected && card != null && card.Abilities.Any(a => a.IsAbility);
            }
        }

        /// <summary>Pumpkin Bomb: before doing damage, you may discard any number of Pokémon Tools from your Pokémon; 40 more damage for each.</summary>
        private sealed class PumpkinBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pumpkin Bomb") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var tool in mon.Tools) holders[tool] = mon;
                int n = 0;
                if (holders.Count > 0)
                    foreach (var tool in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Pumpkin Bomb: discard any number of Pokémon Tools from your Pokémon (40 more damage each)"))
                        if (ctx.Engine.DiscardTool(holders[tool], tool)) n++;
                ctx.DamageDefending(atk.Damage + 40 * n);
                return true;
            }
        }

        /// <summary>Severe Poison: Poisoned, with 4 damage counters instead of 1 between turns. Used for Severe Poison, Super Intense Poison (the attack name and the numbers are passed in).</summary>
        internal sealed class SeverePoisonMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SeverePoisonMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) CardsLOT1.PoisonWith(ctx, defending, amount);
                return true;
            }
        }

        /// <summary>Fixer of the Forest: put 3 Pokémon Tool cards from your discard pile into your hand (as many as there are, up to 3).</summary>
        private sealed class FixerOfTheForestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fixer of the Forest") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var tools = ctx.Me.Discard.Where(c => c.Def.IsTool).ToList();
                if (tools.Count == 0) return true;
                int n = Math.Min(3, tools.Count);
                var picks = tools.Count <= n ? tools : ctx.ChooseCards(tools, n, n, "Fixer of the Forest: put which 3 Pokémon Tool cards into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }
    }
}
