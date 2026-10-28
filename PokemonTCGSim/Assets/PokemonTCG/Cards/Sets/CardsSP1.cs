using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// SWSH Black Star Promos (SP, SWSH001-SWSH307), the new cards numbered up to SWSH137. The rest, the V-UNIONs and Champions
    /// Festival are in CardsSP2. Most of the 307 promos are printings of cards already in the pool (linked in printings.json).
    /// Regulation marks D to F: Expanded. The throwback promos printed "not legal for official tournaments" (Dragapult 132,
    /// Zacian LV.X, Mimikyu δ, Light Toxtricity, Hydreigon C, Greninja ★) are Unlimited only and keep their old rules (your
    /// rulings, v67); Lance's Charizard V and Dark Sylveon V are Expanded.
    /// Rule used here (your ruling, v65): "a [T] Energy card" outside play is basic Energy only (CardsEVS1.IsEnergyOf).
    /// </summary>
    public static class CardsSP1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SP-2", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("SP-3", new ParalyzeFlipMon("Bind"));
            r.RegisterByKey("SP-4", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("SP-5", new DrawAttackMon("G-Max Gold Rush", 3));
            r.RegisterByKey("SP-10", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("SP-11", new FlipNoDamageMon("Defense Curl"));
            r.RegisterByKey("SP-12", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("SP-13", new CardsPOR.SweetScentMon("Heal Pulse", 30));
            r.RegisterByKey("SP-17", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning), new ConditionAttackMon("Venom Slap", SpecialCondition.Poisoned)));
            r.RegisterByKey("SP-20", new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"));
            r.RegisterByKey("SP-21", new PolteageistVSP());
            r.RegisterByKey("SP-28", new RecoilMon("Steel Beam", 40));
            r.RegisterByKey("SP-29", new ComboMon(new NoRetreatMon("Jaw Lock"), new CardsTWM1.SelfDiscardMon("Power Blast", 1)));
            r.RegisterByKey("SP-33", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"), new CardsTWM1.SelfDiscardMon("Metal Blade", 3)));
            r.RegisterByKey("SP-34", new CardsFST1.TakeLessMon(20, null));
            r.RegisterByKey("SP-39", new CardsDAA1.FlipCantAttackMon("Tail Whip"));
            r.RegisterByKey("SP-41", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new SelfThenDefendingDiscardMon("Kindle")));
            r.RegisterByKey("SP-55", new ComboMon(new CardsDAA1.GustAttackMon("Confounding Pulse", true), new CardsRCL1.ConditionBonusMon("Mental Crush", SpecialCondition.Confused, 90)));
            r.RegisterByKey("SP-63", new CardsASR1.TimesMon("Circle Circuit", MyBenchCount));
            r.RegisterByKey("SP-65", new ComboMon(new DrawAttackMon("Collect", 3), new CardsASR1.PlusMon("Brave Buddies", 80, SupporterPlayed)));
            r.RegisterByKey("SP-70", new TailsNothingMon("Full On"));
            r.RegisterByKey("SP-71", new DrawAttackMon("Me First", 1));
            r.RegisterByKey("SP-72", new VaporeonSP());
            r.RegisterByKey("SP-73", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("SP-74", new SearchAttackMon("Happy Delivery", IsItemCard, 1));
            r.RegisterByKey("SP-75", new ComboMon(new SearchAttackMon("Happy Delivery", IsItemCard, 2), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("SP-87", new CardsMEG.SandAttackMon("G-Max Cuddle"));
            r.RegisterByKey("SP-94", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning), new RecoilMon("Thunder", 30)));
            r.RegisterByKey("SP-95", new DrawAttackMon("First Step", 1));
            r.RegisterByKey("SP-100", new ComboMon(new CardsTWM1.HealSelfMon("Leaf Drain", 30), new RecoilMon("Double-Edge", 30)));
            r.RegisterByKey("SP-101", new LockAfterAttackMon("Torrential Cannon", true));
            r.RegisterByKey("SP-102", new ComboMon(new CardsASR1.TimesMon("Forest Storm", GrassEnergyInPlay), new CardsTWM1.HealSelfMon("G-Max Bloom", 30)));
            r.RegisterByKey("SP-103", new ComboMon(new GrandFallsMon(), new CardsFST1.TwoBenchedMon("G-Max Bombard", 30)));
            r.RegisterByKey("SP-106", new CardsFLF1.NoResistanceMon("Brawny Knuckle"));
            r.RegisterByKey("SP-107", new CardsFST1.TwoBenchedMon("Sonic Legs", 20));
            r.RegisterByKey("SP-119", new RecoilMon("Single Strike Tackle", 30));
            r.RegisterByKey("SP-126", new CardsPLF1.HealAllMineMon("Everyone Laze Around", 10));
            r.RegisterByKey("SP-127", new BePreparedMon());
            r.RegisterByKey("SP-128", new IceBonusMon());
            r.RegisterByKey("SP-129", new ComboMon(new BlindsideSPMon("Blindside", 60), new ConditionAttackMon("Moon Mirage", SpecialCondition.Confused)));
            r.RegisterByKey("SP-130", new CardsMEW1.SpreadBenchMon("Blizzard", 10));
            r.RegisterByKey("SP-131", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Cloak in Shadows", EnergyType.Psychic), new NoRetreatMon("Hollow Binding")));
            r.RegisterByKey("SP-132", new ComboMon(new SwitchSelfMon("Mach Turn", true), new CardsASC.RandomDiscardMon("Diving Swipe")));
            r.RegisterByKey("SP-133", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("SP-134", new ComboMon(new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused), new CardsEVS1.AstonishMon("Tricky Ribbon")));
            r.RegisterByKey("SP-135", new ZacianLvXSP());
            r.RegisterByKey("SP-136", new ComboMon(new DrawAttackMon("Filch", 2), new ReturnDefendingEnergyMon("Wet Claw")));
            r.RegisterByKey("SP-137", new ComboMon(new SlowBalladMon(), new LockAfterAttackMon("Beatdown Smash", true)));
        }

        private static bool IsItemCard(CardDef d) { return d.IsItem; }
        private static int MyBenchCount(EffectContext ctx) { return ctx.Me.Bench.Count; }
        private static int SupporterPlayed(EffectContext ctx) { return ctx.Me.SupporterPlayedThisTurn ? 1 : 0; }
        private static int GrassEnergyInPlay(EffectContext ctx) { return ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Grass)); }

        // ================================================================== Shared (also used by CardsSP2)

        /// <summary>"Flip a coin. If heads, prevent all damage done to this Pokémon by attacks during your opponent's next turn" (damage only).</summary>
        internal sealed class FlipNoDamageMon : Mon
        {
            private readonly string attack;

            public FlipNoDamageMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ProtectFromDamage(ctx, attack);
                return true;
            }

            internal static void ProtectFromDamage(EffectContext ctx, string attack)
            {
                ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (" + attack + ")",
                    ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
            }
        }

        /// <summary>"Discard an Energy from this Pokémon. If you do, discard an Energy from your opponent's Active Pokémon" (Kindle).</summary>
        internal sealed class SelfThenDefendingDiscardMon : Mon
        {
            private readonly string attack;

            public SelfThenDefendingDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || ctx.DiscardEnergyFrom(ctx.Pokemon, 1, attack + ": discard an Energy from " + ctx.Pokemon.Name) == 0) return true;
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, attack + ": discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>"This attack does N damage to 1 of your opponent's Pokémon that has any damage counters on it" (Blindside).</summary>
        internal sealed class BlindsideSPMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BlindsideSPMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0)
                {
                    ctx.Log(attack + " did nothing: none of the opponent's Pokémon has damage counters on it.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, attack + ": " + amount + " damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, amount);
                return true;
            }
        }

        /// <summary>"Put an Energy attached to your opponent's Active Pokémon into their hand" after the damage (Wet Claw).</summary>
        internal sealed class ReturnDefendingEnergyMon : Mon
        {
            private readonly string attack;

            public ReturnDefendingEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var pick = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, attack + ": put which Energy attached to " + defending.Name + " into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, pick);
                return true;
            }
        }

        /// <summary>"Attach up to N [T] Energy cards from your discard pile to this Pokémon" (Union Gain): basic Energy only.</summary>
        internal sealed class AttachUpToFromDiscardToSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;

            public AttachUpToFromDiscardToSelfMon(string attack, EnergyType type, int count)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var energy = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, type)).ToList();
                if (energy.Count == 0) return true;
                var picks = ctx.ChooseCards(energy, Math.Min(1, energy.Count), Math.Min(count, energy.Count), attack + ": attach up to " + count + " " + type + " Energy cards to " + me.Name);
                foreach (var e in picks)
                    if (ctx.Me.Discard.Remove(e)) ctx.Engine.AttachEnergyCard(e, me);
                return true;
            }
        }

        /// <summary>"Flip a coin until you get tails. For each heads, draw a card" (Flaring Dash, Shake and Gather).</summary>
        internal sealed class FlipUntilTailsDrawMon : Mon
        {
            private readonly string attack;

            public FlipUntilTailsDrawMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Draw(heads);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>
        /// Teapot of Surprises: if this Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), a random card
        /// from the opponent's hand is revealed and put on the bottom of their deck. Mind Bend: Confused.
        /// </summary>
        private sealed class PolteageistVSP : Mon
        {
            private static readonly Mon MindBend = new ConditionAttackMon("Mind Bend", SpecialCondition.Confused);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player) return;
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Log("Teapot of Surprises: " + ctx.Engine.PlayerName(ctx.Opponent) + " revealed " + card.Def.Name + " and put it on the bottom of their deck.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MindBend.HandleAttack(ctx, atk); }
        }

        /// <summary>Watery Shroud: on the Bench, prevent all damage done to this Pokémon by attacks (yours too). Hydro Pump: +20 per Water Energy.</summary>
        private sealed class VaporeonSP : Mon
        {
            private static readonly Mon HydroPump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && !src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HydroPump.HandleAttack(ctx, atk); }
        }

        /// <summary>Grand Falls: search your deck for up to 3 basic Water Energy cards and attach them to your Benched Pokémon in any way you like.</summary>
        private sealed class GrandFallsMon : Mon
        {
            private static bool WaterCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Water); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Falls") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(WaterCard, 3, "Grand Falls: search your deck for up to 3 Water Energy cards");
                var bench = ctx.Me.Bench;
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (card, mon) => bench.Contains(mon), "Grand Falls: attach a Water Energy card to 1 of your Benched Pokémon", false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Be Prepared: attach a basic Energy card from your hand to this Pokémon.</summary>
        internal sealed class BePreparedMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Be Prepared") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "Be Prepared: attach which basic Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, me, true);
                return true;
            }
        }

        /// <summary>Ice Bonus: discard a basic Water Energy card from your hand; if you do, draw 3 cards.</summary>
        private sealed class IceBonusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Bonus") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var water = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                ctx.Engine.DiscardFromHand(ctx.Player, water[0]);
                if (ctx.Me.Discard.Contains(water[0])) ctx.Draw(3);
                return true;
            }
        }

        /// <summary>
        /// Zacian LV.X (Unlimited only). Bladed Armament (Poké-Body): damage from its attacks isn't affected by any effects on the
        /// opponent's Active Pokémon, the attacks of the Zacian under it included. Brave Blade: it can't attack next turn.
        /// </summary>
        private sealed class ZacianLvXSP : Mon
        {
            private static readonly Mon BraveBlade = new LockAfterAttackMon("Brave Blade", false);

            public override bool AttacksIgnoreDefenderEffects(EffectContext src, DamageInfo info)
            {
                return src.Pokemon != null && info.Attacker == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BraveBlade.HandleAttack(ctx, atk); }
        }

        /// <summary>Slow Ballad: heal 30 damage from both Active Pokémon.</summary>
        private sealed class SlowBalladMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slow Ballad") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0) ctx.Heal(ctx.Me.Active, 30);
                var defending = ctx.Defending;
                if (defending != null && defending.DamageCounters > 0 && ctx.CanAffect(defending)) ctx.Heal(defending, 30);
                return true;
            }
        }
    }
}
