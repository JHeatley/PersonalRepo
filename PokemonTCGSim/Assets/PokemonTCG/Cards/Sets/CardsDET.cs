using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Detective Pikachu (DET 1-18, v79). No regulation mark: Expanded. Arcanine's Security Guard works like Flygon-GX's Dusty
    /// Defense (30 less damage to all of your Pokémon from the opponent's attacks while it's Active), Greninja's Evasion Jutsu like
    /// Drifting Dodge, Mr. Mime's Pantomime swaps a face-down Prize card with the top card of the deck when it's played to the
    /// Bench, and Ditto's Copy Anything uses an attack of any of the opponent's Pokémon if Ditto has the Energy for it.
    /// </summary>
    public static class CardsDET
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DET-1", new SearchAttackMon("Find a Friend", d => d.IsPokemon && d.PokemonType == EnergyType.Grass, 1));
            r.RegisterByKey("DET-2", new LudicoloDET());
            r.RegisterByKey("DET-3", new ConditionAttackMon("Sleep Spore", SpecialCondition.Asleep));
            r.RegisterByKey("DET-4", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("DET-5", new RecoilMon("Wild Tackle", 50));
            r.RegisterByKey("DET-6", new ArcanineDET());
            r.RegisterByKey("DET-7", new CardsRCL2.SelfConditionMon("Panic", SpecialCondition.Confused));
            r.RegisterByKey("DET-8", new CardsTWM1.HealSelfMon("Hold Still", 10));
            r.RegisterByKey("DET-9", new GreninjaDET());
            r.RegisterByKey("DET-10", new ComboMon(new Cards30C.PeerAtMon("Scout"), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("DET-11", new MrMimeDET());
            r.RegisterByKey("DET-12", new ComboMon(new CardsCEC2.DisableMon("Psyjack"), new CardsASC.SelfDiscardTypeMon("Break Burn", EnergyType.Psychic, 2)));
            r.RegisterByKey("DET-13", new ComboMon(new DirectingTrafficMon(), new FlipBonusMon("Cross Chop", 60)));
            r.RegisterByKey("DET-14", new HealingMelodyMon("Healing Melody", 10));
            r.RegisterByKey("DET-15", new FlipBonusMon("Play Rough", 30));
            r.RegisterByKey("DET-16", new CardsPFL.FlipUntilTailsTimesMon("Continuous Lick"));
            r.RegisterByKey("DET-17", new CopyAnythingMon());
            r.RegisterByKey("DET-18", new CardsASC.PushDownMon("Pitch"));
        }

        /// <summary>Table Service: once during your turn, heal 30 damage from 1 of your Pokémon. Punch and Run: then switch with a Benched Pokémon.</summary>
        private sealed class LudicoloDET : Mon
        {
            private static readonly Mon PunchAndRun = new SwitchSelfMon("Punch and Run", false);

            protected override string Activated { get { return "Table Service"; } }
            protected override bool AbilityReady(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }
            protected override void Ability(EffectContext ctx) { CardsMEG.HealOneOfMine(ctx, 30, "Table Service"); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PunchAndRun.HandleAttack(ctx, atk); }
        }

        /// <summary>Security Guard: while this Pokémon is your Active Pokémon, all of your Pokémon take 30 less damage from the opponent's attacks.</summary>
        private sealed class ArcanineDET : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                return damage - 30;
            }
        }

        /// <summary>Evasion Jutsu: when an attack would damage this Pokémon, flip a coin; heads prevents it. Furious Shurikens: 50 to 2 of the opponent's Pokémon.</summary>
        private sealed class GreninjaDET : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && src.Engine.FlipCoin(src.Player, "Evasion Jutsu (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Furious Shurikens") return false;
                CardsDRI1.DamageTwoOpponents(ctx, 50, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Pantomime: when you play this Pokémon from your hand onto your Bench during your turn, you may switch 1 of your face-down
        /// Prize cards with the top card of your deck.
        /// </summary>
        private sealed class MrMimeDET : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Me;
                var faceDown = me.Prizes.Where(c => !me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (!TriggerOnce(ctx, "Pantomime", faceDown.Count > 0 && me.Deck.Count > 0,
                        "Pantomime: switch 1 of your face-down Prize cards with the top card of your deck?")) return;
                var labels = new string[faceDown.Count];
                for (int i = 0; i < faceDown.Count; i++) labels[i] = "Face-down Prize card " + (i + 1);
                var prize = faceDown[ctx.ChooseOneOption("Pantomime: which Prize card?", labels)];
                int at = me.Prizes.IndexOf(prize);
                if (at < 0 || me.Deck.Count == 0) return;
                var top = me.Deck[0];
                me.Prizes[at] = top;
                me.Deck[0] = prize;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched 1 of their Prize cards with the top card of their deck (Pantomime).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Juggling") return false;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(20 * heads);
                return true;
            }
        }

        /// <summary>Directing Traffic: look at the top 5 cards of your deck and put them back in any order.</summary>
        private sealed class DirectingTrafficMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Directing Traffic") return false;
                if (ctx.Me.Deck.Count > 0) CardsOBF2.ReorderOwnTop(ctx, 5, "Directing Traffic");
                return true;
            }
        }

        /// <summary>Healing Melody: heal 10 damage from each of your Pokémon. Used for Healing Melody, Aromatherapy (the attack name and the numbers are passed in).</summary>
        internal sealed class HealingMelodyMon : Mon
        {
            private readonly string attack;
            private readonly int heal;

            public HealingMelodyMon(string attack, int heal)
            {
                this.attack = attack;
                this.heal = heal;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, heal);
                return true;
            }
        }

        /// <summary>
        /// Copy Anything: choose 1 attack of any of the opponent's Pokémon (Active or Benched) and use it as this attack; if this
        /// Pokémon doesn't have the Energy that attack needs, nothing happens.
        /// </summary>
        private sealed class CopyAnythingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Copy Anything") return false;
                var opts = new List<DecisionOption>();
                foreach (var mon in ctx.OpponentPokemon())
                {
                    var d = mon.Def;
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (a.Name == atk.Name) continue;
                        if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                        if (a.VstarPower && ctx.Me.VstarUsed) continue;
                        if (ctx.Engine.AttackNamedPrevented(ctx.Pokemon, a.Name)) continue;
                        opts.Add(new DecisionOption
                        {
                            Label = d.Name + ": " + a.Name + " [" + a.CostString + "]" + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""),
                            Card = d,
                            Index = i
                        });
                    }
                }
                if (opts.Count == 0) return Nothing(ctx);
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Copy Anything: choose 1 of your opponent's Pokémon's attacks to use", opts, ctx.Source);
                var source = opts[pick].Card;
                var copied = source.Attacks[opts[pick].Index];
                if (!ctx.Engine.CanPayAttack(ctx.Pokemon, copied))
                {
                    ctx.Log(ctx.Pokemon.Name + " doesn't have the Energy for " + copied.Name + ", so Copy Anything did nothing.");
                    return true;
                }
                UseCopiedAttack(ctx, source, opts[pick].Index);
                return true;
            }
        }
    }
}
