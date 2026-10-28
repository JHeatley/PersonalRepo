using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Guardians Rising (GRI, v89), part 4: Fairy, Dragon and Colorless (GRI 88-115), and Incineroar-GX (147) and Primarina-GX (149),
    /// printed here only as rainbow rares. Sylveon-GX and Drampa-GX (Hidden Fates Shiny Vault) were already in the pool.
    /// </summary>
    public static class CardsGRI4
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fairy
            r.RegisterByKey("GRI-88", new CardsPOR.FollowMeMon("Follow Me"));
            r.RegisterByKey("GRI-89", new ComboMon(new ConditionAttackMon("Lullaby", SpecialCondition.Asleep), new CardsASC.NextTurnBoostMon("Meteor Mash", 60)));
            r.RegisterByKey("GRI-90", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("GRI-91", new WagesOfFluffMon());
            r.RegisterByKey("GRI-93", new ComfeyGRI());
            // Dragon
            r.RegisterByKey("GRI-94", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("GRI-95", new CardsUNM1.NamedBenchSearchMon("Division", "Sliggoo", 2));
            r.RegisterByKey("GRI-96", new ComboMon(new CardsDAA1.PowerWhipMon(), new CardsASC.PushDownMon("Beat Slider")));
            r.RegisterByKey("GRI-97", new ComboMon(new AmassMon(), new CardsFST1.MoveEnergyToBenchMon("Power Cyclone")));
            r.RegisterByKey("GRI-98", new CardsDRI2.MudCoatMon(10));
            r.RegisterByKey("GRI-100", new ComboMon(new ShieldAttackMon("Adamantine Press", 30), new CardsPAF2.IgnoreDefenderEffectsMon("Shred")));
            // Colorless
            r.RegisterByKey("GRI-101", new BindWoundMon());
            r.RegisterByKey("GRI-102", new BlisseyGRI());
            r.RegisterByKey("GRI-103", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("GRI-104", new ComboMon(new DodgeMon("Agility", false),
                new CardsFST2.ConditionBonusMon("Swallow Dive", 80, c => c.Pokemon.LastAttackName == "Agility" && c.Pokemon.LastAttackTurn == c.MyLastTurn)));
            r.RegisterByKey("GRI-105", new ComboMon(new SearchAttackMon("Weather Teller", d => d.IsStadiumCard, 2), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("GRI-106", new TurboStormMon());
            r.RegisterByKey("GRI-107", new CardsSHF2.GlanceMon());
            r.RegisterByKey("GRI-108", new ComboMon(new ScrutinizeMon(), new CoinsMon("Slam", 2)));
            r.RegisterByKey("GRI-109", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("GRI-110", new TailsNothingMon("Razor Wind"));
            r.RegisterByKey("GRI-111", new ComboMon(new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1), new LoopTheLoopMon()));
            r.RegisterByKey("GRI-113", new BewearGRI());
            r.RegisterByKey("GRI-114", new KomalaGRI());
            // Rainbow rares with no regular print here
            r.RegisterByKey("GRI-147", new ComboMon(new CardsASR1.PlusMon("Hustling Strike", 20, c => c.Me.Bench.Count(m => c.Engine.IsType(m, EnergyType.Fire))),
                new CardsWHT.FlipMoreMon("Tiger Swing", 2, 50), new ConditionAttackMon("Burning Slam-GX", SpecialCondition.Burned)));
            r.RegisterByKey("GRI-149", new ComboMon(new CardsASR1.PlusMon("Bubble Beat", 20, c => c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Water))),
                new CardsDRI2.CrunchMon("Roaring Seas"), new HealAllMineMon("Grand Echo-GX")));
        }

        // ================================================================== Fairy

        /// <summary>The Wages of Fluff: if the Defending Pokémon is Knocked Out during your next turn, take 2 more Prize cards.</summary>
        private sealed class WagesOfFluffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "The Wages of Fluff") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var fx = new WagesOfFluffEffect(defending.Name + ": 2 more Prize cards if it's Knocked Out during " + ctx.Engine.Their(ctx.Player) + " next turn (The Wages of Fluff)",
                    ctx.Player, defending.Uid, ctx.Turn + 2);
                fx.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>The owner takes 2 more Prize cards for this Pokémon while the effect lasts. It ends if the Pokémon leaves the Active Spot.</summary>
        public sealed class WagesOfFluffEffect : ContinuousEffect
        {
            public WagesOfFluffEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return knockedOut != null && knockedOut.Uid == TargetUid && taker == Owner ? prizes + 2 : prizes;
            }
        }

        /// <summary>Flower Shield: your Pokémon with any Fairy Energy attached can't be affected by Special Conditions (and recover from them). Sweet Kiss: the opponent draws a card.</summary>
        private sealed class ComfeyGRI : Mon
        {
            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                if (src.Pokemon == null || target == null || target.Owner != src.Player) return false;
                return target.Energy.Any(en => CardsDRI1.ProvidesType(src, target, en, EnergyType.Fairy));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Kiss") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DrawCards(ctx.Opponent, 1);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Amass: search your deck for a basic Energy card and attach it to this Pokémon.</summary>
        private sealed class AmassMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amass") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(me)) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Amass: search your deck for a basic Energy card");
                foreach (var en in found)
                    if (!ctx.Engine.AttachEnergyCard(en, me)) ctx.Me.Deck.Add(en);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Bind Wound: flip a coin; heads, heal 30 damage from 1 of your Pokémon.</summary>
        internal sealed class BindWoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bind Wound") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Bind Wound: heal 30 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 30);
                return true;
            }
        }

        /// <summary>Fresh Egg: when you play this Pokémon from your hand to evolve, you may heal 80 damage from your Active Pokémon. Double-Edge: 80 to itself.</summary>
        private sealed class BlisseyGRI : Mon
        {
            private static readonly Mon Edge = new RecoilMon("Double-Edge", 80);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (!TriggerOnce(ctx, "Fresh Egg", active != null && active.DamageCounters > 0, "Use Fresh Egg? Heal 80 damage from your Active Pokémon.")) return;
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 80);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Edge.HandleAttack(ctx, atk); }
        }

        /// <summary>Turbo Storm: attach 2 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class TurboStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Storm") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var dest = ctx.ChoosePokemon(ctx.MyBench(), "Turbo Storm: attach basic Energy cards from your discard pile to which Benched Pokémon?");
                if (dest == null) return true;
                ctx.AttachCardsAnyWay(energy, Math.Min(2, energy.Count), (card, mon) => mon == dest, "Turbo Storm: attach a basic Energy card to " + dest.Name, false);
                return true;
            }
        }

        /// <summary>Scrutinize: look at the top 2 cards of the opponent's deck; discard 1 and put the other back.</summary>
        private sealed class ScrutinizeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scrutinize") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.Opp.Deck.Take(Math.Min(2, ctx.Opp.Deck.Count)).ToList();
                if (top.Count == 0) return true;
                ctx.Engine.NoteLookedAt(top, "Top cards of your opponent's deck", ctx.Player);
                var pick = top.Count == 1 ? top[0] : ctx.ChooseCards(top, 1, 1, "Scrutinize: discard which of the top 2 cards of your opponent's deck?")[0];
                if (!ctx.Opp.Deck.Remove(pick)) return true;
                ctx.Opp.Discard.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + pick.Def.Name + " from the top of " + ctx.Engine.Their(ctx.Opponent) + " deck (Scrutinize).");
                return true;
            }
        }

        /// <summary>Loop-the-Loop: put all Energy attached to this Pokémon into your hand.</summary>
        private sealed class LoopTheLoopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Loop-the-Loop") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    foreach (var en in me.Energy.ToList()) ctx.Engine.ReturnEnergyToHand(me, en);
                return true;
            }
        }

        /// <summary>Rake It In: when you play this Pokémon from your hand to evolve, you may draw 3 cards. Dangerous Blow: 60 more against a Basic Pokémon.</summary>
        private sealed class BewearGRI : Mon
        {
            private static readonly Mon Blow = new CardsCRZ1.BasicTargetBonusMon("Dangerous Blow", 60);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Rake It In", ctx.Me.Deck.Count > 0, "Use Rake It In? Draw 3 cards.")) return;
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blow.HandleAttack(ctx, atk); }
        }

        /// <summary>Comatose: while it's your Active Pokémon, whenever you attach an Energy from your hand to it, it is now Asleep. Hypno Roll: only while Asleep.</summary>
        private sealed class KomalaGRI : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !src.PokemonIsActive) return;
                Announce(src, "Comatose");
                src.ApplyCondition(me, SpecialCondition.Asleep);
            }

            /// <summary>Hypno Roll is its only attack and is meant to be used while Asleep (not while Paralyzed).</summary>
            public override bool AllowsAttackWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Paralyzed);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypno Roll") return false;
                if (!ctx.Pokemon.Has(SpecialCondition.Asleep)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>"Heal all damage from all of your Pokémon" (Grand Echo-GX).</summary>
        internal sealed class HealAllMineMon : Mon
        {
            private readonly string attack;

            public HealAllMineMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }
    }
}
