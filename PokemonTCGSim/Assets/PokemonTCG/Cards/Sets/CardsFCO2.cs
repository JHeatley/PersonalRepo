using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Fates Collide (FCO, v94), part 2: the Fighting, Darkness and Metal Pokémon (36-64).</summary>
    public static class CardsFCO2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FCO-37", new MarowakFCO());
            r.RegisterByKey("FCO-39", new ComboMon(new CardsDAA1.DrainMon("Cling"), new FlipBonusMon("X-Scissor", 60)));
            r.RegisterByKey("FCO-40", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("FCO-41", new CardsSSP2.StompOffMon("Mountain Munch", 1));
            r.RegisterByKey("FCO-42", new CardsGRI2.FlipBonusOrRecoilMon("Thrash", 20, 20));
            r.RegisterByKey("FCO-43", new RegirockEX());
            r.RegisterByKey("FCO-44", new CardsFST2.ConditionBonusMon("Twin Bursts", 60, c => c.Me.Bench.Any(m => m.Def.Name == "Mothim")));
            r.RegisterByKey("FCO-45", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("FCO-46", new TailsNothingMon("Smash Punch"));
            r.RegisterByKey("FCO-48", new ComboMon(new DrawAttackMon("Backflip", 1), new ParalyzeFlipMon("Wicked Jab")));
            r.RegisterByKey("FCO-49", new CarbinkEnergyKeeper());
            r.RegisterByKey("FCO-50", new CarbinkSafeguard());
            r.RegisterByKey("FCO-51", new DiamondGiftMon());
            r.RegisterByKey("FCO-52", new ComboMon(new CardsDAA1.GustAttackMon("Lookout", false), new AuraBreakMon()));
            r.RegisterByKey("FCO-53", new ComboMon(new NoRetreatMon("Rumble"), new GeostrikeMon()));
            r.RegisterByKey("FCO-54", new ComboMon(new CardsFST2.ConditionBonusMon("Land's Pulse", 20, StadiumInPlay), new CardsTWM1.HealSelfMon("Cell Storm", 30)));
            r.RegisterByKey("FCO-55", new ComboMon(new VeilOfDarknessMon(), new EndgameMon()));
            r.RegisterByKey("FCO-56", new TyranitarFCO());
            r.RegisterByKey("FCO-57", new ParalyzeFlipMon("Leer"));
            r.RegisterByKey("FCO-58", new BoneDropMon());
            r.RegisterByKey("FCO-59", new ComboMon(new CardsASR1.TimesMon("Strike Back", c => c.Pokemon.DamageCounters), new FlipUntilTailsMoreMon("Iron Head", 20)));
            r.RegisterByKey("FCO-60", new FlipProtectMon("Iron Defense"));
            r.RegisterByKey("FCO-61", new BronzongFCO());
            r.RegisterByKey("FCO-62", new MetalRainMon());
            r.RegisterByKey("FCO-63", new ComboMon(new NoWeaknessResistanceMon("Vacuum Wave"), new CardsUNB2.FightAloneMon(60)));
            r.RegisterByKey("FCO-64", new GenesectEX());
        }

        internal static bool StadiumInPlay(EffectContext ctx) { return ctx.State.Stadium != null; }

        // ================================================================== Shared helpers

        /// <summary>"Flip a coin until you get tails. This attack does N more damage for each heads" (Iron Head).</summary>
        internal sealed class FlipUntilTailsMoreMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public FlipUntilTailsMoreMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int heads = ctx.FlipUntilTails();
                ctx.DamageDefending(atk.Damage + per * heads);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, prevent all effects of attacks, including damage, done to this Pokémon during your opponent's next turn."</summary>
        internal sealed class FlipProtectMon : Mon
        {
            private readonly string attack;

            public FlipProtectMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (heads) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Metal Energy cards attached to the Pokémon (anything that provides Metal Energy there).</summary>
        internal static List<CardInstance> MetalEnergyOn(EffectContext ctx, PokemonInPlay mon)
        {
            return mon.Energy.Where(e => CardsHIF1.Provides(ctx, mon, e, EnergyType.Metal)).ToList();
        }

        // ================================================================== Fighting

        /// <summary>
        /// Bodyguard: prevent all effects of the opponent's attacks done to you or your hand; existing ones are removed (your ruling,
        /// v94: Item, Supporter, Stadium and Pokémon locks, Trainer flips, the Item and evolution locks, and anything that
        /// discards, shuffles, moves or reveals cards from your hand; not your deck, Prize cards or Pokémon). The engine does the
        /// work (GameEngine.Bodyguard.cs). Bonemerang: 2 coins, 60 for each heads.
        /// </summary>
        private sealed class MarowakFCO : Mon
        {
            private static readonly Mon Bonemerang = new CoinsMon("Bonemerang", 2);

            public override bool ShieldsPlayerFromAttackEffects(EffectContext src) { return src.Pokemon != null; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bonemerang.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Regi Power: your Fighting Pokémon's attacks (not Regirock-EX's) do 10 more damage to the opponent's Active Pokémon (before
        /// Weakness and Resistance); each Regirock-EX adds its own. Bedrock Press: 20 less damage next turn.
        /// </summary>
        private sealed class RegirockEX : Mon
        {
            private static readonly Mon Press = new CardsASC.SelfGuardMon("Bedrock Press", 20);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                if (attacker.Def.Name == "Regirock-EX" || !src.Engine.IsType(attacker, EnergyType.Fighting)) return damage;
                return damage + 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Press.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Energy Keeper: basic Energy attached to your Basic Pokémon can't be discarded by the opponent's attacks, Abilities or
        /// Trainer cards. Stone Edge: flip, 30 more.
        /// </summary>
        private sealed class CarbinkEnergyKeeper : Mon
        {
            private static readonly Mon StoneEdge = new FlipBonusMon("Stone Edge", 30);

            public override bool ProtectsEnergyFromOpponentsDiscard(EffectContext src, PokemonInPlay holder, CardInstance energy)
            {
                return src.Pokemon != null && holder != null && holder.Owner == src.Player && holder.Def.IsBasicPokemon && energy != null && energy.Def.IsBasicEnergy;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return StoneEdge.HandleAttack(ctx, atk); }
        }

        /// <summary>Safeguard: prevent all effects of attacks, damage included, done to this Pokémon by the opponent's Pokémon-EX (the XY-era ones).</summary>
        private sealed class CarbinkSafeguard : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !a.Def.IsPokemonEX) return damage;
                return 0;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsPokemonEX;
            }
        }

        /// <summary>Diamond Gift: attach 2 Energy cards from your discard pile to 1 of your Fighting Pokémon (1 if that's all there is).</summary>
        private sealed class DiamondGiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diamond Gift") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                var targets = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fighting));
                if (energy.Count == 0 || targets.Count == 0) return true;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Diamond Gift: attach Energy to which of your Fighting Pokémon?");
                if (target == null) return true;
                int n = Math.Min(2, energy.Count);
                var picks = n == energy.Count ? energy : ctx.ChooseCards(energy, n, n, "Diamond Gift: choose 2 Energy cards from your discard pile");
                foreach (var card in picks)
                    if (ctx.InPlay(target) && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Aura Break: if the Defending Pokémon is a Darkness or Fairy Pokémon, it can't attack during the opponent's next turn.</summary>
        private sealed class AuraBreakMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aura Break") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                if (!ctx.Engine.IsType(defending, EnergyType.Darkness) && !ctx.Engine.IsType(defending, EnergyType.Fairy)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Aura Break)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Geostrike: also 10 damage to each of your Benched Pokémon (no Weakness or Resistance for Benched Pokémon).</summary>
        private sealed class GeostrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Geostrike") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Veil of Darkness: discard any number of cards from your hand, then draw that many.</summary>
        private sealed class VeilOfDarknessMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Veil of Darkness") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = ctx.ChooseCards(hand, 0, hand.Count, "Veil of Darkness: discard any number of cards from your hand, then draw that many");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (picks.Count > 0) ctx.Draw(picks.Count);
                return true;
            }
        }

        /// <summary>Endgame: if the opponent's Mega Evolution Pokémon (an XY-era M Pokémon-EX) is Knocked Out by this attack's damage, take 2 more Prize cards.</summary>
        private sealed class EndgameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Endgame") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && ctx.InPlay(defending) && defending.Def.IsXYMegaEvolution && ctx.Engine.IsKnockedOut(defending))
                    ctx.AddEffect(new KnockOutPrizeBonusEffect("Endgame: take 2 more Prize cards for " + defending.Name, ctx.Player, defending.Uid, ctx.Turn, 2));
                return true;
            }
        }

        /// <summary>
        /// Raging Roar: when played from your hand to evolve, you may attach a Darkness Energy card from your discard pile to it for
        /// each Prize card your opponent has taken. Dark Mountain: discard the top 2 cards of your deck; 50 more for each Supporter.
        /// </summary>
        private sealed class TyranitarFCO : Mon
        {
            private const string RagingRoar = "Raging Roar";

            private static List<CardInstance> Darkness(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Darkness);
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                int taken = ctx.OpponentPrizesTaken;
                var pile = Darkness(ctx);
                if (!TriggerOnce(ctx, RagingRoar, taken > 0 && pile.Count > 0, "Raging Roar: attach up to " + taken + " Darkness Energy from your discard pile to " + ctx.Pokemon.Name + "?")) return;
                int n = Math.Min(taken, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Raging Roar: choose " + n + " Darkness Energy cards");
                foreach (var card in picks)
                    if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Mountain") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 2);
                int supporters = milled.Count(c => c.Def.IsSupporter);
                ctx.DamageDefending(atk.Damage + 50 * supporters);
                return true;
            }
        }

        /// <summary>
        /// Bone Drop: 60 damage to 1 of the opponent's Pokémon that has an Ability (a working one: your v85 ruling for Weavile's Evil
        /// Admonition). No Weakness or Resistance for Benched Pokémon.
        /// </summary>
        private sealed class BoneDropMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bone Drop") return false;
                var targets = ctx.OpponentPokemon(m => ctx.Engine.HasAbility(m));
                if (targets.Count == 0) return true;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Bone Drop: do 60 damage to which of your opponent's Pokémon with an Ability?");
                if (target != null) ctx.DamageTarget(target, 60);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Metal Fortress: prevent all effects of the opponent's attacks, damage included, done to your Benched Pokémon. Guard Press: 20 less next turn.</summary>
        private sealed class BronzongFCO : Mon
        {
            private static readonly Mon GuardPress = new CardsASC.SelfGuardMon("Guard Press", 20);

            private static bool Guarded(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && src.Me.Bench.Contains(target);
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (a == null || a.Owner == src.Player || !Guarded(src, info.Target)) return damage;
                return 0;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && Guarded(src, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return GuardPress.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Metal Rain: discard any number of Metal Energy from this Pokémon; for each one, 30 damage to 1 of the opponent's Pokémon
        /// (the same one may be chosen again), not affected by Weakness or Resistance.
        /// </summary>
        private sealed class MetalRainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Rain") return false;
                var me = ctx.Pokemon;
                var metal = MetalEnergyOn(ctx, me);
                if (metal.Count == 0) return true;
                var picks = ctx.ChooseCards(metal, 0, metal.Count, "Metal Rain: discard any number of Metal Energy (30 damage for each)");
                int discarded = 0;
                foreach (var card in picks)
                    if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                for (int i = 0; i < discarded; i++)
                {
                    var targets = ctx.OpponentPokemon();
                    if (targets.Count == 0) break;
                    var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Metal Rain: do 30 damage to which of your opponent's Pokémon? (" + (i + 1) + " of " + discarded + ")");
                    if (target != null) ctx.DamageWith(target, 30, false, false, false);
                }
                return true;
            }
        }

        /// <summary>Vacuum Wave: this attack's damage isn't affected by Weakness or Resistance. Used for Vacuum Wave, Sonicboom, Jet Needle (the attack name is passed in).</summary>
        internal sealed class NoWeaknessResistanceMon : Mon
        {
            private readonly string attack;

            public NoWeaknessResistanceMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Drive Change: once during your turn (before your attack), put a Pokémon Tool attached to this Pokémon into your hand.
        /// Rapid Blaster: discard any number of Metal Energy from this Pokémon; 20 more for each.
        /// </summary>
        private sealed class GenesectEX : Mon
        {
            protected override string Activated { get { return "Drive Change"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon.Tools.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me.Tools.Count == 0) return;
                var tool = me.Tools.Count == 1 ? me.Tools[0] : ctx.ChooseCards(me.Tools.ToList(), 1, 1, "Drive Change: put which Pokémon Tool into your hand?")[0];
                if (!me.Tools.Remove(tool)) return;
                ctx.Me.Hand.Add(tool);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + tool.Def.Name + " from " + me.Name + " into " + ctx.Engine.Their(ctx.Player) + " hand (Drive Change).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Blaster") return false;
                var me = ctx.Pokemon;
                var metal = MetalEnergyOn(ctx, me);
                int discarded = 0;
                if (metal.Count > 0)
                {
                    var picks = ctx.ChooseCards(metal, 0, metal.Count, "Rapid Blaster: discard any number of Metal Energy (20 more damage for each)");
                    foreach (var card in picks)
                        if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                }
                ctx.DamageDefending(atk.Damage + 20 * discarded);
                return true;
            }
        }
    }
}
