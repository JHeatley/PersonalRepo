using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Rebel Clash (RCL, 2020), Pokémon 1-93: Grass, Fire, Water, Lightning and Psychic. Fighting through Colorless, the
    /// Trainers and the Special Energy are in CardsRCL2. Regulation mark D: Expanded only.
    /// Already in the pool (most reprinted in Shining Fates): Rillaboom V / VMAX, Eldegoss V, Applin (Withdraw), Flapple,
    /// Appletun, Cinderace V / VMAX, Galarian Darumaka, Galarian Darmanitan, Cramorant, Arrokuda, Barraskewda, Eiscue, Shinx,
    /// Luxio, Luxray, Boltund V, Toxtricity V / VMAX, Morpeko, Galarian Corsola, Galarian Cursola, Hatenna, Hattrem, Hatterene,
    /// Milcery, Alcremie, Indeedee, Dreepy, Drakloak and Dragapult; the secret rare Frosmoth too.
    /// Pokémon whose attacks only do their printed damage need no script (Grubbin, Snover, Electabuzz).
    /// Rule used here (your ruling, v65): "a [T] Energy card" outside play is basic Energy only (CardsEVS1.IsEnergyOf).
    /// </summary>
    public static class CardsRCL1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("RCL-1", new CardsSVI1.AdaptiveEvolutionMon());
            r.RegisterByKey("RCL-2", new CardsSVI1.AdaptiveEvolutionMon());
            r.RegisterByKey("RCL-3", new CardsCES1.ConditionsMon("Panic Poison", SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("RCL-4", new SetupBoostMon("Swords Dance", "Blinding Scythe", 70));
            r.RegisterByKey("RCL-5", new ComboMon(new BerryPickingMon(), new ParalyzeFlipMon("Bind")));
            r.RegisterByKey("RCL-6", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("RCL-7", new CardsTWM1.HealSelfMon("Mini Drain", 10));
            r.RegisterByKey("RCL-8", new TopEntryMon(new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("RCL-9", new ComboMon(new CardsASR1.TimesMon("Spirited Rushdown", MyPrizesTaken), new CardsTWM1.HealSelfMon("Mega Drain", 30)));
            r.RegisterByKey("RCL-10", new DodgeMon("Agility", false));
            r.RegisterByKey("RCL-11", new ComboMon(new NoEnergyFromHandAttackMon("Threatening Pattern"), new SwitchSelfMon("U-turn", false)));
            r.RegisterByKey("RCL-13", new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep));
            r.RegisterByKey("RCL-14", new DiscardBasicToBenchMon("Dark Guidance"));
            r.RegisterByKey("RCL-15", new NoRetreatMon("Shadow Cage"));
            r.RegisterByKey("RCL-20", new CardsMEG.FlipUntilTailsBonusMon("Continuous Tumble", 30));
            // Fire
            r.RegisterByKey("RCL-24", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("RCL-25", new ComboMon(new ConditionBonusMon("Hex", SpecialCondition.None, 90), new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep)));
            r.RegisterByKey("RCL-26", new ComboMon(new CopyDefendingAttackMon("Nine-Tailed Shapeshifter"), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("RCL-27", new FlipDiscardToHandMon("Odor Sleuth"));
            r.RegisterByKey("RCL-28", new ArcanineRCL());
            r.RegisterByKey("RCL-29", new FlipBonusMon("Heat Breath", 30));
            r.RegisterByKey("RCL-30", new ComboMon(new ConditionAttackMon("Burst Punch", SpecialCondition.Burned), new GroundBurnMon()));
            r.RegisterByKey("RCL-31", new ConditionAttackMon("Scorch", SpecialCondition.Burned));
            r.RegisterByKey("RCL-32", new TopEntryMon(new AttachFromDiscardMon("Reignite", EnergyType.Fire, false)));
            r.RegisterByKey("RCL-33", new ChandelureRCL());
            r.RegisterByKey("RCL-34", new ComboMon(new ParalyzeFlipMon("Lick"), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            // Water
            r.RegisterByKey("RCL-37", new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep), new CoinsMon("Triple Spin", 3)));
            r.RegisterByKey("RCL-38", new MrRimeRCL());
            r.RegisterByKey("RCL-39", new SwitchSelfMon("Leap Out", false));
            r.RegisterByKey("RCL-40", new DiscardStadiumAttackMon("Big Storm"));
            r.RegisterByKey("RCL-41", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("RCL-42", new CardsFST1.MoveEnergyToBenchMon("Power Cyclone"));
            r.RegisterByKey("RCL-43", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Aqua Impact", 50, false), new ConditionAttackMon("Hypno Splash", SpecialCondition.Asleep)));
            r.RegisterByKey("RCL-44", new ConditionAttackMon("Spiral Attack", SpecialCondition.Confused));
            r.RegisterByKey("RCL-45", new ConditionAttackMon("Twirling Sign", SpecialCondition.Confused));
            r.RegisterByKey("RCL-46", new ComboMon(new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused), new ConditionBonusMon("Resonance", SpecialCondition.Confused, 120)));
            r.RegisterByKey("RCL-49", new ComboMon(new CardsMEG.SnipeMon("Snipe Shot", 40), new CardsASR1.RevealHandAttackMon("Aqua Report")));
            r.RegisterByKey("RCL-50", new ComboMon(new MayReturnDefendingEnergyMon("Hydro Snipe"), new Cards30CC.BenchHitMon("Max Bullet", 60)));
            r.RegisterByKey("RCL-55", new EiscueVRCL());
            // Lightning
            r.RegisterByKey("RCL-56", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("RCL-57", new SearchAttackMon("Orb Collector", IsEnergyCard, 3));
            r.RegisterByKey("RCL-59", new ComboMon(new ParalyzeFlipMon("Thunder Shock"), new SpecialEnergyBonusMon("Electrified Bolt", 90)));
            r.RegisterByKey("RCL-63", new RecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("RCL-64", new ComboMon(new FlipDiscardOpponentEnergyMon("Eerie Impulse"), new RecoilMon("Thunder", 30)));
            r.RegisterByKey("RCL-65", new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 2));
            r.RegisterByKey("RCL-66", new ComboMon(new CardsASR1.PlusMon("Powerful Storm", 20, AllMyEnergy), new RecoilMon("Thunder Jolt Beam", 30)));
            r.RegisterByKey("RCL-68", new ParalyzeFlipMon("Tight Jaw"));
            r.RegisterByKey("RCL-69", new PoisonShoutMon());
            r.RegisterByKey("RCL-72", new PincurchinVRCL());
            // Psychic
            r.RegisterByKey("RCL-74", new ConditionAttackMon("Shining Fingers", SpecialCondition.Asleep));
            r.RegisterByKey("RCL-75", new ClefableRCL());
            r.RegisterByKey("RCL-76", new DrawAttackMon("Me First", 1));
            r.RegisterByKey("RCL-77", new ComboMon(new EnergyWarpMon(), new CardsMEG.DefendingEnergyBonusMon("Psychic", 30)));
            r.RegisterByKey("RCL-80", new SigilyphRCL());
            r.RegisterByKey("RCL-81", new CountersOnOneMon("Sneaky Placement", 1));
            r.RegisterByKey("RCL-82", new ComboMon(new SandSinkMon(), new CardsTWM1.HealSelfMon("Super Absorption", 30)));
            r.RegisterByKey("RCL-92", new JetAssaultMon());
            r.RegisterByKey("RCL-93", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new BenchCountersMon("Max Phantom", 5)));
        }

        private static int MyPrizesTaken(EffectContext ctx) { return ctx.MyPrizesTaken; }
        private static bool IsEnergyCard(CardDef d) { return d.IsEnergy; }
        private static int AllMyEnergy(EffectContext ctx) { return ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)); }

        // ================================================================== Shared (also used by CardsRCL2)

        /// <summary>Top Entry (Lombre, Lampent): offered right after the card is drawn for the turn; the attacks are another script's.</summary>
        internal sealed class TopEntryMon : Mon
        {
            private readonly Mon attacks;

            public TopEntryMon(Mon attacks) { this.attacks = attacks; }

            public override void OnDrawnForTurn(EffectContext ctx) { CardsSHF1.TopEntry(ctx); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"During your next turn, this Pokémon's [other attack] does N more damage" (Swords Dance) and that other attack.</summary>
        internal sealed class SetupBoostMon : Mon
        {
            private readonly string setup;
            private readonly string boosted;
            private readonly int bonus;

            public SetupBoostMon(string setup, string boosted, int bonus)
            {
                this.setup = setup;
                this.boosted = boosted;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == setup)
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s " + boosted + " does " + bonus + " more damage during its next turn (" + setup + ")", ctx.Player, me.Uid, ctx.Turn + 2, boosted, bonus);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name != boosted) return false;
                int uid = me.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == boosted).Sum(b => b.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                return true;
            }
        }

        /// <summary>"Attach a [T] Energy card from your discard pile to this Pokémon / 1 of your Pokémon" (basic Energy only).</summary>
        internal sealed class AttachFromDiscardMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly bool selfOnly;

            public AttachFromDiscardMon(string attack, EnergyType type, bool selfOnly)
            {
                this.attack = attack;
                this.type = type;
                this.selfOnly = selfOnly;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (selfOnly && !ctx.InPlay(me)) return true;
                var kind = type;
                var pile = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, kind)).ToList();
                if (pile.Count == 0) return true;
                if (selfOnly) ctx.AttachCardsAnyWay(pile, 1, (card, mon) => mon == me, attack, false);
                else ctx.AttachCardsAnyWay(pile, 1, (card, mon) => true, attack, false);
                return true;
            }
        }

        /// <summary>"N more damage if the opponent's Active Pokémon is [condition]" (Resonance); None means any Special Condition (Hex).</summary>
        internal sealed class ConditionBonusMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;
            private readonly int bonus;

            public ConditionBonusMon(string attack, SpecialCondition condition, int bonus)
            {
                this.attack = attack;
                this.condition = condition;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool on = defending != null && (condition == SpecialCondition.None ? defending.Conditions != SpecialCondition.None : defending.Has(condition));
                ctx.DamageDefending(atk.Damage + (on ? bonus : 0));
                return true;
            }
        }

        /// <summary>"N more damage if this Pokémon evolved from [Pokémon] during this turn" (Raid).</summary>
        internal sealed class EvolvedFromBonusMon : Mon
        {
            private readonly string attack;
            private readonly string baseName;
            private readonly int bonus;

            public EvolvedFromBonusMon(string attack, string baseName, int bonus)
            {
                this.attack = attack;
                this.baseName = baseName;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                bool fresh = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == baseName;
                ctx.DamageDefending(atk.Damage + (fresh ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Put N damage counters on 1 of your opponent's Pokémon" (Sneaky Placement).</summary>
        internal sealed class CountersOnOneMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public CountersOnOneMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, attack + ": put " + counters + (counters == 1 ? " damage counter" : " damage counters") + " on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, counters);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Berry Picking: shuffle up to 5 basic Energy cards from your discard pile into your deck.</summary>
        private sealed class BerryPickingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Berry Picking") return false;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return true;
                var picks = ctx.ChooseCards(pile, 0, Math.Min(5, pile.Count), "Berry Picking: shuffle up to 5 basic Energy cards from your discard pile into your deck");
                if (picks.Count == 0) return true;
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, Energy can't be attached from your opponent's hand to the Defending Pokémon."</summary>
        private sealed class NoEnergyFromHandAttackMon : Mon
        {
            private readonly string attack;

            public NoEnergyFromHandAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsCRI.NoEnergyFromHandEffect(defending.Name + ": no Energy can be attached to it from the hand during its next turn (" + attack + ")",
                    ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Dark Guidance: put a Basic Pokémon from your discard pile onto your Bench.</summary>
        internal sealed class DiscardBasicToBenchMon : Mon
        {
            private readonly string attack;

            public DiscardBasicToBenchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace <= 0) return true;
                var basics = ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                if (basics.Count == 0) return true;
                var picks = ctx.ChooseCards(basics, 1, 1, attack + ": put which Basic Pokémon from your discard pile onto your Bench?");
                foreach (var card in picks)
                    if (ctx.BenchSpace > 0 && ctx.Me.Discard.Remove(card)) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Nine-Tailed Shapeshifter: choose 1 of the opponent's Active Pokémon's attacks and use it as this attack (only this attack's cost is needed).</summary>
        private sealed class CopyDefendingAttackMon : Mon
        {
            private readonly string attack;

            public CopyDefendingAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                CardDef copied;
                int index;
                if (ChooseCopiedAttack(ctx, defending, attack + ": choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index)
                    && copied.Attacks[index].Name != attack)
                    UseCopiedAttack(ctx, copied, index);
                return true;
            }
        }

        /// <summary>Odor Sleuth: flip a coin; heads, put a card from your discard pile into your hand.</summary>
        internal sealed class FlipDiscardToHandMon : Mon
        {
            private readonly string attack;

            public FlipDiscardToHandMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Discard.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, attack + ": put which card from your discard pile into your hand?");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        /// <summary>Warming Up: with a Burning Scarf attached, this Pokémon gets +100 HP.</summary>
        private sealed class ArcanineRCL : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.HasToolNamed("Burning Scarf") ? hp + 100 : hp;
            }
        }

        /// <summary>Ground Burn: each player discards the top card of their deck; 100 more damage for each Energy card discarded.</summary>
        private sealed class GroundBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ground Burn") return false;
                var mine = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                var theirs = ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                int energy = mine.Count(c => c.Def.IsEnergy) + theirs.Count(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + 100 * energy);
                return true;
            }
        }

        /// <summary>Protective Glow: your Pokémon that have Energy attached have no Weakness. Mirage Flare: Confused.</summary>
        private sealed class ChandelureRCL : Mon
        {
            private static readonly Mon MirageFlare = new ConditionAttackMon("Mirage Flare", SpecialCondition.Confused);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && mon.Energy.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MirageFlare.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Screen Cleaner: prevent all effects of the opponent's attacks, except damage, done to your Pokémon that have Energy attached.</summary>
        private sealed class MrRimeRCL : Mon
        {
            private static readonly Mon TripleSpin = new CoinsMon("Triple Spin", 3);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && target.Energy.Count > 0 && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TripleSpin.HandleAttack(ctx, atk); }
        }

        /// <summary>Big Storm: discard any Stadium in play.</summary>
        internal sealed class DiscardStadiumAttackMon : Mon
        {
            private readonly string attack;

            public DiscardStadiumAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.DiscardStadiumInPlay(ctx);
                return true;
            }
        }

        /// <summary>Hydro Snipe: you may put an Energy attached to the opponent's Active Pokémon into their hand.</summary>
        internal sealed class MayReturnDefendingEnergyMon : Mon
        {
            private readonly string attack;

            public MayReturnDefendingEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var picks = ctx.ChooseCards(defending.Energy.ToList(), 0, 1, attack + ": you may put an Energy attached to " + defending.Name + " into your opponent's hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(defending, e);
                return true;
            }
        }

        /// <summary>Cold Absorption: whenever you attach a basic Water Energy card from your hand to this Pokémon during your turn, heal 30 damage from it.</summary>
        private sealed class EiscueVRCL : Mon
        {
            private static readonly Mon Blizzard = new CardsMEW1.SpreadBenchMon("Blizzard", 10);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !src.IsMyTurn || energy == null || !CardsEVS1.IsEnergyOf(energy.Def, EnergyType.Water)) return;
                if (!src.InPlay(me) || me.DamageCounters == 0) return;
                Announce(src, "Cold Absorption");
                src.Heal(me, 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blizzard.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Electrified Bolt: N more damage if this Pokémon has any Special Energy attached.</summary>
        private sealed class SpecialEnergyBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public SpecialEnergyBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool special = ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
                ctx.DamageDefending(atk.Damage + (special ? bonus : 0));
                return true;
            }
        }

        /// <summary>Eerie Impulse: flip a coin; heads, discard an Energy from 1 of your opponent's Pokémon.</summary>
        internal sealed class FlipDiscardOpponentEnergyMon : Mon
        {
            private readonly string attack;

            public FlipDiscardOpponentEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, attack + ": discard an Energy from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, attack + ": discard an Energy from " + target.Name);
                return true;
            }
        }

        /// <summary>Poison Shout: 20 damage to each of the opponent's Pokémon (Weakness and Resistance for the Active only); the Active is Poisoned.</summary>
        private sealed class PoisonShoutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Shout") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(20);
                foreach (var b in ctx.OpponentBench()) ctx.DamageTarget(b, 20);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Counterattack Kerzap: if this Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), flip 3 coins; 3 damage counters on the Attacking Pokémon for each heads.</summary>
        private sealed class PincurchinVRCL : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Counterattack Kerzap");
                int heads = ctx.FlipCoins(3);
                if (heads > 0 && ctx.InPlay(attacker)) ctx.PlaceCounters(attacker, 3 * heads);
            }
        }

        // ================================================================== Psychic

        /// <summary>Prankish: when you play this Pokémon from your hand to evolve, you may put an Energy attached to the opponent's Active Pokémon on top of their deck.</summary>
        private sealed class ClefableRCL : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                bool ready = defending != null && defending.Energy.Count > 0;
                if (!TriggerOnce(ctx, "Prankish", ready, "Prankish: put an Energy attached to " + (defending == null ? "" : defending.Name) + " on top of your opponent's deck?")) return;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Prankish on " + defending.Name + " was prevented.");
                    return;
                }
                var pick = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Prankish: put which Energy on top of your opponent's deck?")[0];
                defending.Energy.Remove(pick);
                ctx.Opp.Deck.Insert(0, pick);
                ctx.Engine.Emit(GameEventType.Info, ctx.Opponent, pick.Def.Name + " was put on top of " + ctx.Engine.Their(ctx.Opponent) + " deck (Prankish).", pick.Id, defending.Uid);
            }
        }

        /// <summary>Energy Warp: move an Energy from 1 of the opponent's Benched Pokémon to their Active Pokémon.</summary>
        private sealed class EnergyWarpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Warp") return false;
                var active = ctx.Opp.Active;
                if (active == null || !ctx.CanAffect(active)) return true;
                var donors = ctx.OpponentBench().Where(m => m.Energy.Count > 0 && ctx.CanAffect(m)).ToList();
                if (donors.Count == 0) return true;
                var donor = ctx.ChoosePokemon(donors, "Energy Warp: move an Energy from which of your opponent's Benched Pokémon?");
                if (donor == null) return true;
                var card = donor.Energy.Count == 1 ? donor.Energy[0]
                    : ctx.ChooseCards(donor.Energy.ToList(), 1, 1, "Energy Warp: move which Energy to " + active.Name + "?")[0];
                ctx.Engine.MoveEnergy(donor, active, card);
                return true;
            }
        }

        /// <summary>Counterattack: if this Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), 3 damage counters on the Attacking Pokémon. Psychic Assault: +10 per damage counter.</summary>
        private sealed class SigilyphRCL : Mon
        {
            private static readonly Mon PsychicAssault = new CardsMEW1.CounterBonusMon("Psychic Assault", 10);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Counterattack");
                ctx.PlaceCounters(attacker, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PsychicAssault.HandleAttack(ctx, atk); }
        }

        /// <summary>Sand Sink: discard the top card of the opponent's deck, 2 more if this Pokémon has a Cursed Shovel attached.</summary>
        private sealed class SandSinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Sink") return false;
                int n = 1 + (ctx.Pokemon.HasToolNamed("Cursed Shovel") ? 2 : 0);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }

        /// <summary>Jet Assault: 80 more if this Pokémon moved from your Bench to the Active Spot this turn.</summary>
        private sealed class JetAssaultMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jet Assault") return false;
                bool moved = ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn;
                ctx.DamageDefending(atk.Damage + (moved ? 80 : 0));
                return true;
            }
        }

        /// <summary>Max Phantom: put N damage counters on your opponent's Benched Pokémon in any way you like.</summary>
        private sealed class BenchCountersMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public BenchCountersMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentBench());
                return true;
            }
        }
    }
}
