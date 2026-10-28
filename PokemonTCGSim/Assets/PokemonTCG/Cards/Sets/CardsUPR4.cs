using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ultra Prism (UPR, v85), part 4: Colorless (UPR 102-117) and the secret rares with no regular Ultra Prism print: Pheromosa-GX
    /// (140), Celesteela-GX (144) and Lunala-GX (172). Eevee (104) and Silvally-GX were already in the pool (Hidden Fates) and keep
    /// their scripts.
    /// </summary>
    public static class CardsUPR4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UPR-102", new ComboMon(new DrawAttackMon("Lap Up", 3), new CoinsMon("Slam", 2)));
            r.RegisterByKey("UPR-103", new DangerousLickMon());
            r.RegisterByKey("UPR-105", new CardsASR1.TimesMon("Palette of Friends", BenchTypes));
            r.RegisterByKey("UPR-106", new CardsUPR3.SmokescreenMon("Weak Kneed", 1));
            r.RegisterByKey("UPR-107", new ComboMon(new CoinsMon("Stompy Stomp", 2), new HappyTurnMon()));
            r.RegisterByKey("UPR-108", new GentleBiteMon());
            r.RegisterByKey("UPR-109", new ComboMon(new OwnThePlaceMon(), new TossAsideMon()));
            r.RegisterByKey("UPR-110", new CardsUPR1.RotoMotorMon(new CardsCEC3.SpreadAmountMon("Spinning Fan", 20)));
            r.RegisterByKey("UPR-111", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("UPR-112", new ComboMon(new Cards30C.PeerAtMon("Scout"), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("UPR-113", new IdentifyMon());
            r.RegisterByKey("UPR-114", new ComboMon(new ResourceManagementMon(), new ConditionAttackMon("Profound Knowledge", SpecialCondition.Confused)));
            r.RegisterByKey("UPR-115", new CardsPFL.DamagedBonusMon("Merciless Strike", 30));
            r.RegisterByKey("UPR-117", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new CardsBRS1.MillSelfMon("Dragon Pulse", 2)));
            // Secret rares with no regular print in this set
            r.RegisterByKey("UPR-140", new PheromosaGX());
            r.RegisterByKey("UPR-144", new CelesteelaGX());
            r.RegisterByKey("UPR-172", new LunalaGX());
        }

        /// <summary>The different types of Pokémon on your Bench (Palette of Friends).</summary>
        internal static int BenchTypes(EffectContext ctx)
        {
            var types = new HashSet<EnergyType>();
            foreach (var mon in ctx.MyBench())
                foreach (var t in ctx.Engine.TypesOf(mon)) types.Add(t);
            return types.Count;
        }

        // ================================================================== Colorless

        /// <summary>Dangerous Lick: flip until tails, 50 more damage for each heads; if the first flip is tails, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class DangerousLickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dangerous Lick") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipUntilTails();
                ctx.DamageDefending(atk.Damage + 50 * heads);
                if (heads == 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Happy Turn: you may shuffle this Pokémon and all cards attached to it into your deck.</summary>
        private sealed class HappyTurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Happy Turn") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.AskYesNo("Happy Turn: shuffle " + me.Name + " and all cards attached to it into your deck?"))
                    ctx.Engine.ShufflePokemonIntoDeck(me);
                return true;
            }
        }

        /// <summary>Gentle Bite: during the opponent's next turn, the Defending Pokémon's attacks do 40 less damage (before Weakness and Resistance).</summary>
        private sealed class GentleBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gentle Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 40 less damage during its next turn (Gentle Bite)", ctx.Player, defending.Uid, ctx.Turn + 1, 40));
                return true;
            }
        }

        /// <summary>Own the Place: if the opponent has a Stadium card in play, discard it; if you do, prevent all effects of attacks, including damage, done to this Pokémon during their next turn.</summary>
        private sealed class OwnThePlaceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Own the Place") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium == null || ctx.State.StadiumOwner != ctx.Opponent) return true;
                ctx.Engine.DiscardStadium();
                if (ctx.State.Stadium == null) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Toss Aside: discard random cards from the opponent's hand until they have 3 cards in their hand.</summary>
        private sealed class TossAsideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toss Aside") return false;
                ctx.DamageDefending(atk.Damage);
                int extra = ctx.Opp.Hand.Count - 3;
                if (extra > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, extra);
                return true;
            }
        }

        /// <summary>Identify: the opponent reveals their hand; 80 more damage if there's a Pokémon there.</summary>
        private sealed class IdentifyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Identify") return false;
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed an empty hand.");
                else ctx.Engine.Reveal(ctx.Opponent, hand, "revealed " + ctx.Engine.Their(ctx.Opponent) + " hand (Identify)");
                ctx.DamageDefending(atk.Damage + (hand.Any(c => c.Def.IsPokemon) ? 80 : 0));
                return true;
            }
        }

        /// <summary>Resource Management: put 3 cards from your discard pile on the bottom of your deck in any order.</summary>
        private sealed class ResourceManagementMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Resource Management") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(3, pile.Count);
                var picks = pile.Count <= n ? pile : ctx.ChooseCards(pile, n, n, "Resource Management: put which 3 cards from your discard pile on the bottom of your deck?");
                var order = new List<CardInstance>();
                var left = picks.ToList();
                while (left.Count > 1)
                {
                    var next = ctx.ChooseCards(left, 1, 1, "Resource Management: which card goes on the bottom first (" + (order.Count + 1) + " of " + picks.Count + ")?")[0];
                    order.Add(next);
                    left.Remove(next);
                }
                order.AddRange(left);
                var moved = ctx.DiscardToDeck(order);
                if (moved.Count > 0) ctx.Log(moved.Count + (moved.Count == 1 ? " card" : " cards") + " went from the discard pile to the bottom of the deck (Resource Management).");
                return true;
            }
        }

        // ================================================================== Secret rares

        /// <summary>Fast Raid: usable on your first turn if you go first. Cruel Spike: Confused. Beauty-GX: 50 damage for each Prize card the opponent has taken.</summary>
        private sealed class PheromosaGX : Mon
        {
            private static readonly Mon Spike = new ConditionAttackMon("Cruel Spike", SpecialCondition.Confused);
            private static readonly Mon Beauty = new CardsASR1.TimesMon("Beauty-GX", c => c.OpponentPrizesTaken);

            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Fast Raid"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spike.HandleAttack(ctx, atk) || Beauty.HandleAttack(ctx, atk); }
        }

        /// <summary>Rocket Fall: 30 more for each [C] in the opponent's Active Pokémon's Retreat Cost. Blaster-GX: turn all of your Prize cards face up for the rest of the game.</summary>
        private sealed class CelesteelaGX : Mon
        {
            private static readonly Mon Fall = new CardsOBF1.RetreatCostDamageMon("Rocket Fall", 30, false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaster-GX") return Fall.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var p = ctx.Me;
                foreach (var prize in p.Prizes)
                    if (!p.FaceUpPrizeIds.Contains(prize.Id)) p.FaceUpPrizeIds.Add(prize.Id);
                if (p.Prizes.Count > 0) ctx.Engine.Reveal(ctx.Player, p.Prizes.ToList(), "turned " + ctx.Engine.Their(ctx.Player) + " Prize cards face up for the rest of the game (Blaster-GX)");
                return true;
            }
        }

        /// <summary>
        /// Psychic Transfer: as often as you like during your turn, move a Psychic Energy from 1 of your Pokémon to another of your
        /// Pokémon. Moongeist Beam: the Defending Pokémon can't be healed during the opponent's next turn. Lunar Fall-GX: Knock Out
        /// 1 of the opponent's Basic Pokémon that isn't a Pokémon-GX.
        /// </summary>
        private sealed class LunalaGX : Mon
        {
            private const string Transfer = "Psychic Transfer";

            private static List<CardInstance> PsychicOn(EffectContext ctx, PokemonInPlay holder)
            {
                return holder.Energy.Where(en => CardsDRI1.ProvidesType(ctx, holder, en, EnergyType.Psychic)).ToList();
            }

            private static bool CanMove(EffectContext ctx)
            {
                return ctx.MyPokemon().Count > 1 && ctx.MyPokemon(m => PsychicOn(ctx, m).Count > 0).Count > 0;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var abilities = ctx.Pokemon.Def.Abilities;
                if (abilityIndex < 0 || abilityIndex >= abilities.Count || abilities[abilityIndex].Name != Transfer) return false;
                return ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var origin = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => PsychicOn(ctx, m).Count > 0),
                    Transfer + ": move a Psychic Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (origin == null) return false;
                var energy = PsychicOn(ctx, origin);
                if (energy.Count == 0) return false;
                var card = energy.Select(en => en.Def.Name).Distinct().Count() == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, Transfer + ": move which Energy?")[0];
                var dest = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != origin), Transfer + ": move it from " + origin.Name + " to which Pokémon?", offerDone);
                if (dest == null) return false;
                return ctx.Engine.MoveEnergy(origin, dest, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Moongeist Beam")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.AddEffect(new NoHealingEffect(defending.Name + " can't be healed during its next turn (Moongeist Beam)", ctx.Player, defending.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name != "Lunar Fall-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Def.IsBasicPokemon && !m.Def.IsGX && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Lunar Fall-GX: Knock Out which of your opponent's Basic Pokémon (not a Pokémon-GX)?");
                if (target != null) ctx.Engine.KnockOut(target, "Lunar Fall-GX");
                return true;
            }
        }

        /// <summary>"The Defending Pokémon can't be healed during your opponent's next turn" (Moongeist Beam). Ends if it moves to the Bench.</summary>
        internal sealed class NoHealingEffect : ContinuousEffect
        {
            public NoHealingEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksHealing(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }
    }
}
