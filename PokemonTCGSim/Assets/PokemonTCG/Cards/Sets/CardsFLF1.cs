using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Flashfire (FLF, v104), part 1: the Grass, Fire, Water, Lightning and Psychic Pokémon (1-44). Caterpie, Metapod,
    /// Butterfree, Ponyta, Rapidash and Meowstic are Generations' cards, Spheal is Primal Clash's and Charizard-EX (12) and
    /// M Charizard-EX (13) are Evolutions' cards.
    /// </summary>
    public static class CardsFLF1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FLF-5", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("FLF-7", new ShiftryDraw());
            r.RegisterByKey("FLF-8", new CardsPAR2.FlipConditionMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("FLF-9", new ComboMon(new WhiplashMon(), new CardsTWM1.HealSelfMon("Mega Drain", 20)));
            r.RegisterByKey("FLF-10", new MaractusShake());
            r.RegisterByKey("FLF-11", new ComboMon(new StokeMon(), new CardsTWM1.SelfDiscardMon("Fire Blast", 1)));
            r.RegisterByKey("FLF-16", new FlipTailsSelfDiscardMon("Flamethrower"));
            r.RegisterByKey("FLF-17", new FlipBonusMon("Firebreathing", 20));
            r.RegisterByKey("FLF-20", new PyroarMane());
            r.RegisterByKey("FLF-21", new CardsASC.CounterattackMon("Counterattack Quills", 2, new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("FLF-22", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("FLF-23", new MiloticGrace());
            r.RegisterByKey("FLF-25", new CardsPAL1.RestMon());
            r.RegisterByKey("FLF-26", new ComboMon(new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep), new CardsUPR1.LessPerSelfCounterMon("Big Tusk", 10)));
            r.RegisterByKey("FLF-27", new ComboMon(new HeartWinkMon(), new DrawAttackMon("Spike Draw", 1)));
            r.RegisterByKey("FLF-28", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("FLF-29", new ComboMon(new RescueMon(), new CardsASC.FlipDiscardDefendingEnergyMon("Screw Tail")));
            r.RegisterByKey("FLF-30", new CardsSSP2.StompOffMon("Stomp Off", 1));
            r.RegisterByKey("FLF-31", new ComboMon(new ShieldAttackMon("Frost Barrier", 20), new CardsPAF1.DiscardStadiumMon("Shatter")));
            r.RegisterByKey("FLF-32", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("FLF-33", new CardsASR1.TimesMon("Electricounter", c => c.Engine.PrizesTaken(c.Opponent)));
            r.RegisterByKey("FLF-34", new ComboMon(new CardsASR1.FangSnipeMon(), new RecoilMon("Wild Charge", 30)));
            r.RegisterByKey("FLF-35", new DualBulletMon());
            r.RegisterByKey("FLF-36", new CardsASC.FlipDiscardDefendingEnergyMon("Destructive Beam"));
            r.RegisterByKey("FLF-37", new ComboMon(new SearchAttackMon("Parabolic Charge", d => d.IsEnergy, 2), new FlipBonusMon("Quick Attack", 30)));
            r.RegisterByKey("FLF-38", new ComboMon(new RevivalMon(), new SneakyPlacementMon()));
            r.RegisterByKey("FLF-39", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new CardsLOR1.CountersAnyWayMon("Cursed Drop", 4)));
            r.RegisterByKey("FLF-40", new DusknoirVoid());
            r.RegisterByKey("FLF-41", new ComboMon(new CardsDAA2.TriplePoisonMon("Triple Poison", 3), new NoResistanceMon("Smash Uppercut")));
            r.RegisterByKey("FLF-42", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("FLF-44", new CardsPAR2.FlipConditionMon("Spit Poison", SpecialCondition.Poisoned));
        }

        // ================================================================== Shared helpers

        /// <summary>"Flip a coin. If tails, discard an Energy attached to this Pokémon." (Flamethrower).</summary>
        internal sealed class FlipTailsSelfDiscardMon : Mon
        {
            private readonly string attack;

            public FlipTailsSelfDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>"You may discard a [type] Energy attached to this Pokémon. If you do, this attack does N more damage." (Scorching Fang).</summary>
        internal sealed class OptionalTypeDiscardBonusMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int bonus;

            public OptionalTypeDiscardBonusMon(string attack, EnergyType type, int bonus)
            {
                this.attack = attack;
                this.type = type;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                bool boost = false;
                if (me.Energy.Any(en => CardsDRI1.ProvidesType(ctx, me, en, type))
                    && ctx.AskYesNo(attack + ": discard a " + type + " Energy from " + me.Name + " for " + bonus + " more damage?"))
                    boost = ctx.DiscardEnergyFrom(me, 1, attack + ": discard a " + type + " Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, type)) > 0;
                ctx.DamageDefending(atk.Damage + (boost ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Your opponent can't draw a card at the beginning of their next turn" (Heart Wink).</summary>
        public sealed class NoTurnDrawEffect : ContinuousEffect
        {
            public NoTurnDrawEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool SkipsTurnDraw(GameEngine engine, int player) { return player != Owner; }
        }

        // ================================================================== Grass

        /// <summary>Leaf Draw: once during your turn, discard a Grass Energy card from your hand to draw 3 cards. Deranged Dance: 20 x every Benched Pokémon.</summary>
        private sealed class ShiftryDraw : Mon
        {
            private static readonly Mon Dance = new CardsASR1.TimesMon("Deranged Dance", c => c.Me.Bench.Count + c.Opp.Bench.Count);

            protected override string Activated { get { return "Leaf Draw"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass));
            }

            protected override void Ability(EffectContext ctx)
            {
                var grass = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass)).ToList();
                if (grass.Count == 0) return;
                var card = ctx.ChooseCards(grass, 1, 1, "Leaf Draw: discard which Grass Energy card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dance.HandleAttack(ctx, atk); }
        }

        /// <summary>Whiplash: flip until tails; for each heads, discard an Energy attached to the opponent's Active Pokémon.</summary>
        private sealed class WhiplashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whiplash") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Energy.Count == 0) break;
                    ctx.DiscardEnergyFrom(defending, 1, "Whiplash: discard an Energy from " + defending.Name + " (" + (i + 1) + " of " + heads + ")");
                }
                return true;
            }
        }

        /// <summary>Exciting Shake: during your next turn, Prickly Needles flips 6 coins instead of 2. Prickly Needles: 20 for each heads.</summary>
        private sealed class MaractusShake : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Exciting Shake")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var shake = new AttackBoostEffect(me.Name + "'s Prickly Needles flips 6 coins during its next turn (Exciting Shake)", ctx.Player, me.Uid, ctx.Turn + 2, "Prickly Needles", 4);
                    shake.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(shake);
                    return true;
                }
                if (atk.Name == "Prickly Needles")
                {
                    int uid = me.Uid;
                    int coins = 2 + ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == "Prickly Needles").Sum(b => b.Bonus);
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(coins));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>Stoke: flip a coin; heads, search your deck for up to 3 basic Energy cards and attach them to this Pokémon.</summary>
        private sealed class StokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stoke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0 || !ctx.InPlay(me)) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Stoke: choose up to 3 basic Energy cards to attach to " + me.Name);
                foreach (var en in found) ctx.Engine.AttachEnergyCard(en, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Intimidating Mane: no damage from the opponent's Basic Pokémon's attacks. Scorching Fang: may discard a Fire Energy for 30 more.</summary>
        private sealed class PyroarMane : Mon
        {
            private static readonly Mon Fang = new OptionalTypeDiscardBonusMon("Scorching Fang", EnergyType.Fire, 30);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                return me != null && info.Target == me && attacker != null && attacker.Owner != me.Owner && attacker.Def.IsBasicPokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fang.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>
        /// Energy Grace: once during your turn, Knock Out this Pokémon; then attach 3 basic Energy cards from your discard pile to 1 of
        /// your other Pokémon that isn't a Pokémon-EX (the opponent takes the Prize card as usual).
        /// </summary>
        private sealed class MiloticGrace : Mon
        {
            protected override string Activated { get { return "Energy Grace"; } }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Energy Grace"; }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && !m.Def.IsPokemonEX);
            }

            protected override bool AbilityReady(EffectContext ctx) { return true; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!ctx.Engine.KnockOut(me, "Energy Grace")) return;
                var targets = Targets(ctx);
                var pile = ctx.BasicEnergyInDiscard();
                if (targets.Count == 0 || pile.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Energy Grace: attach 3 basic Energy cards from your discard pile to which Pokémon?");
                if (target == null) return;
                int n = Math.Min(3, pile.Count);
                var picks = ctx.ChooseCards(pile, n, n, "Energy Grace: choose 3 basic Energy cards to attach to " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>Heart Wink: flip a coin; heads, the opponent can't draw a card at the beginning of their next turn.</summary>
        private sealed class HeartWinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heart Wink") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var wink = new NoTurnDrawEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't draw a card at the beginning of their next turn (Heart Wink)", ctx.Player, ctx.Turn + 1);
                wink.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(wink);
                return true;
            }
        }

        /// <summary>Rescue: shuffle 3 Pokémon from your discard pile into your deck.</summary>
        private sealed class RescueMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rescue") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(3, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Rescue: shuffle 3 Pokémon from your discard pile into your deck");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Dual Bullet: 50 damage to 2 of the opponent's Pokémon (no Weakness or Resistance for Benched Pokémon).</summary>
        private sealed class DualBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Bullet") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                int n = Math.Min(2, all.Count);
                var picks = ctx.ChoosePokemonMany(all, n, n, "Dual Bullet: choose 2 of your opponent's Pokémon (50 damage each)");
                foreach (var target in picks) ctx.DamageTarget(target, 50);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Revival: put a Basic Pokémon from the opponent's discard pile onto their Bench.</summary>
        private sealed class RevivalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Revival") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return true;
                var basics = ctx.Opp.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (basics.Count == 0) return true;
                var pick = ctx.ChooseCards(basics, 1, 1, "Revival: put which Basic Pokémon from your opponent's discard pile onto their Bench?");
                if (pick.Count == 0 || !ctx.Opp.Discard.Remove(pick[0])) return true;
                ctx.Engine.PutOntoBench(ctx.Opponent, pick[0], false);
                return true;
            }
        }

        /// <summary>Sneaky Placement: put 1 damage counter on the opponent's Active Pokémon.</summary>
        private sealed class SneakyPlacementMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sneaky Placement") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 1);
                return true;
            }
        }

        /// <summary>
        /// Shadow Void: as often as you like during your turn, move 1 damage counter from 1 of your other Pokémon to this Pokémon
        /// (with no limit: it can Knock itself Out, your ruling). Pain Pellets: as many counters on 1 of the opponent's Pokémon as
        /// this Pokémon has.
        /// </summary>
        private sealed class DusknoirVoid : Mon
        {
            private const string Void = "Shadow Void";

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.DamageCounters > 0);
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Void
                    && ctx.IsMyTurn && ctx.InPlay(ctx.Pokemon) && Givers(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => ctx.InPlay(me) && !me.ForcedKnockOut && ctx.RemainingHp(me) > 0 && Givers(ctx).Count > 0, offerDone =>
                {
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), Void + ": move 1 damage counter from which Pokémon to " + me.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    return ctx.Engine.MoveDamageCounters(giver, me, 1, Void) > 0;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pain Pellets") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Pokemon.DamageCounters;
                var all = ctx.OpponentPokemon();
                if (n <= 0 || all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Pain Pellets: put " + n + " damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, n);
                return true;
            }
        }

        /// <summary>Smash Uppercut: this attack's damage isn't affected by Resistance. Used for Smash Uppercut, Hook Shot, Mach Claw (the attack name is passed in).</summary>
        internal sealed class NoResistanceMon : Mon
        {
            private readonly string attack;

            public NoResistanceMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                return true;
            }
        }
    }
}
