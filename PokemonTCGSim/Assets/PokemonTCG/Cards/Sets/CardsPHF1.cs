using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Phantom Forces (PHF, v102), part 1: the Grass, Fire, Water, Lightning and Psychic Pokémon (1-45). Krabby and Golbat are
    /// Generations' cards and Wobbuffet the Radiant Collection one.
    /// </summary>
    public static class CardsPHF1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PHF-1", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("PHF-2", new ComboMon(new DizzyingWindMon(), new CardsROS1.ConditionsAttackMon("Noxious Scales", SpecialCondition.Confused, SpecialCondition.Poisoned)));
            r.RegisterByKey("PHF-3", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("PHF-4", new ComboMon(new CardsBKT1.DeepSeaSwirlMon("Windfall", 6), new CardsFST2.ConditionBonusMon("Surprise Strike", 50, CardsUNM1.CameInThisTurn)));
            r.RegisterByKey("PHF-5", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("PHF-6", new DodgeMon("Protect", false));
            r.RegisterByKey("PHF-7", new ComboMon(new CoordinateMon(), new HealTypeEachMon("Leaf Storm", EnergyType.Grass, 20)));
            r.RegisterByKey("PHF-10", new ComboMon(new CardsWHT.FlipMoreMon("Acrobatics", 2, 30), new JetShootMon()));
            r.RegisterByKey("PHF-11", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("PHF-12", new PyroarCommand());
            r.RegisterByKey("PHF-14", new CardsASC.SelfGuardMon("Guard Claw", 20));
            r.RegisterByKey("PHF-15", new CoinsMon("Fury Strikes", 3));
            r.RegisterByKey("PHF-16", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("PHF-17", new ComboMon(new HyperWhirlpoolMon(), new CardsPFL.DamagedBonusMon("Second Strike", 80)));
            r.RegisterByKey("PHF-19", new ComboMon(new SearchAttackMon("Neon Sign", d => d.IsPokemon, 2), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("PHF-20", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("PHF-21", new ComboMon(new MeddlingMon(), new CardsMEG.DefendingEnergyBonusMon("Ensnaring Spray", 10)));
            r.RegisterByKey("PHF-22", new SuperDeepDiveMon());
            r.RegisterByKey("PHF-23", new ComboMon(new Cards30CC.BenchHitMon("Overrun", 20), new CardsFST2.ConditionBonusMon("Assault Laser", 60, c => c.Defending != null && c.Defending.Tools.Count > 0)));
            r.RegisterByKey("PHF-24", new TurboBoltMon());
            r.RegisterByKey("PHF-25", new ComboMon(new TrickStickerMon(), new FlipBonusMon("Pachi", 30)));
            r.RegisterByKey("PHF-26", new NightMarchMon());
            r.RegisterByKey("PHF-27", new ComboMon(new CardsDAA1.GustAttackMon("Cobweb Trip", true), new NoRetreatMon("Electroweb")));
            r.RegisterByKey("PHF-28", new CardsWHT.FlipMoreMon("Tail Rap", 2, 20));
            r.RegisterByKey("PHF-29", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("PHF-30", new ParabolicSparkMon());
            r.RegisterByKey("PHF-31", new CardsMEG.SnipeMon("Skill Dive", 10));
            r.RegisterByKey("PHF-33", new CrobatBite());
            r.RegisterByKey("PHF-34", new ComboMon(new NightAttackMon(), new DarkCorridorMon()));
            r.RegisterByKey("PHF-35", new CardsEVO3.MetronomeMon("Phantom Gate"));
            r.RegisterByKey("PHF-37", new CardsPAR2.FlipConditionMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("PHF-38", new ComboMon(new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Swallow Up", 50, c => c.Defending != null && c.RemainingHp(c.Defending) < c.RemainingHp(c.Pokemon))));
            r.RegisterByKey("PHF-39", new ComboMon(new Cards30C.PeerAtMon("See Through"), new CoinsMon("Double Headbutt", 2)));
            r.RegisterByKey("PHF-40", new ComboMon(new DreamOfMemoriesMon(), new DreamDanceMon()));
            r.RegisterByKey("PHF-41", new FlipBonusMon("Trip Over", 10));
            r.RegisterByKey("PHF-42", new ComboMon(new CardsLOR1.CountersAnyWayMon("Cursed Drop", 3), new NightMarchMon()));
            r.RegisterByKey("PHF-43", new ChandelureSpell());
            r.RegisterByKey("PHF-44", new NightMarchMon());
            r.RegisterByKey("PHF-45", new GourgeistGigantic());
        }

        // ================================================================== Shared helpers

        /// <summary>"Heal N damage from each of your [type] Pokémon" after the damage (Leaf Storm, Diamond Storm).</summary>
        internal sealed class HealTypeEachMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int amount;

            public HealTypeEachMon(string attack, EnergyType type, int amount)
            {
                this.attack = attack;
                this.type = type;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => ctx.Engine.IsType(m, type)))
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, amount);
                return true;
            }
        }

        /// <summary>Night March: 20 damage for each Pokémon in your discard pile that has the Night March attack.</summary>
        internal sealed class NightMarchMon : Mon
        {
            internal static int Marchers(EffectContext ctx)
            {
                return ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.Attacks.Any(a => a.Name == "Night March"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night March") return false;
                int n = Marchers(ctx);
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log("Night March did no damage.");
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>During the opponent's next turn, each Trainer card they play from their hand needs heads to work (Dizzying Wind).</summary>
        internal sealed class TrainerFlipEffect : ContinuousEffect
        {
            public TrainerFlipEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool ForcesTrainerFlip(GameEngine engine, int player) { return player != Owner; }
        }

        private sealed class DizzyingWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dizzying Wind") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var flip = new TrainerFlipEffect(ctx.Engine.PlayerName(ctx.Opponent) + " flips a coin for each Trainer card played during their next turn (Dizzying Wind)", ctx.Player, ctx.Turn + 1);
                flip.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(flip);
                return true;
            }
        }

        /// <summary>Coordinate: for up to 2 of your Benched Pokémon without a Tool, search your deck for a Pokémon Tool and attach it.</summary>
        private sealed class CoordinateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coordinate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.Me.Bench.Where(m => m.Tools.Count == 0 && ctx.Engine.CanAttachToolTo(null, m)).ToList();
                if (bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var picks = ctx.ChoosePokemonMany(bench, 0, Math.Min(2, bench.Count), "Coordinate: choose up to 2 of your Benched Pokémon without a Pokémon Tool");
                foreach (var mon in picks)
                {
                    var found = ctx.SearchMyDeck(d => d.IsTool, 1, "Coordinate: choose a Pokémon Tool to attach to " + mon.Name);
                    if (found.Count == 0) continue;
                    if (ctx.Engine.CanAttachToolTo(found[0], mon)) ctx.Engine.AttachTool(found[0], mon);
                    else ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>During the opponent's next turn, attack damage to this Pokémon is increased by 40 after Weakness and Resistance (Jet Shoot).</summary>
        internal sealed class TakesMoreEffect : ContinuousEffect
        {
            private readonly int amount;

            public TakesMoreEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || damage <= 0) return damage;
                return damage + amount;
            }
        }

        private sealed class JetShootMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jet Shoot") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new TakesMoreEffect(me.Name + " takes 40 more damage from attacks during the opponent's next turn (Jet Shoot)", ctx.Player, me.Uid, ctx.Turn + 1, 40));
                return true;
            }
        }

        /// <summary>Flare Command: once during your turn, discard a Fire Energy from this Pokémon to switch 1 of the opponent's Benched Pokémon with their Active.</summary>
        private sealed class PyroarCommand : Mon
        {
            private static readonly Mon Onrush = new RecoilMon("Inferno Onrush", 30);

            protected override string Activated { get { return "Flare Command"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.Opp.Bench.Count > 0 && me.Energy.Any(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fire));
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.DiscardEnergyFrom(me, 1, "Flare Command: discard a Fire Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fire)) > 0)
                    ctx.GustOpponent("Flare Command: switch which of your opponent's Benched Pokémon into the Active Spot?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Onrush.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Hyper Whirlpool: flip a coin until you get tails; discard an Energy from the opponent's Active Pokémon for each heads.</summary>
        private sealed class HyperWhirlpoolMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Whirlpool") return false;
                var defending = ctx.Defending;
                int heads = 0;
                while (ctx.Flip()) heads++;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    ctx.DiscardEnergyFrom(defending, 1, "Hyper Whirlpool: discard an Energy from " + defending.Name);
                }
                return true;
            }
        }

        /// <summary>Meddling: attach 3 Energy cards from the opponent's discard pile to their Pokémon in any way you like.</summary>
        private sealed class MeddlingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Meddling") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 3; i++)
                {
                    var pile = ctx.Opp.Discard.Where(c => c.Def.IsEnergy).ToList();
                    var targets = ctx.OpponentPokemon(m => ctx.CanAffect(m));
                    if (pile.Count == 0 || targets.Count == 0) break;
                    var card = ctx.ChooseCards(pile, 1, 1, "Meddling: attach which Energy card from your opponent's discard pile (" + (i + 1) + " of 3)?")[0];
                    var target = ctx.ChoosePokemon(targets, "Meddling: attach " + card.Def.Name + " to which of your opponent's Pokémon?");
                    if (target == null || !ctx.Opp.Discard.Remove(card)) break;
                    if (!ctx.Engine.AttachEnergyCard(card, target)) ctx.Opp.Discard.Add(card);
                }
                return true;
            }
        }

        /// <summary>Super Deep Dive (Alomomola): heal 30 from this Pokémon, then switch it with 1 of your Benched Pokémon.</summary>
        private sealed class SuperDeepDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Deep Dive") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.Heal(ctx.Pokemon, 30);
                if (ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Turbo Bolt: attach 2 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class TurboBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                var bench = ctx.MyBench();
                if (pile.Count == 0 || bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Turbo Bolt: attach 2 basic Energy cards to which Benched Pokémon?");
                if (target != null) ctx.AttachCardsAnyWay(pile, 2, (card, mon) => mon == target, "Turbo Bolt: attach a basic Energy card to " + target.Name, false);
                return true;
            }
        }

        /// <summary>Until the end of your next turn, the Defending Pokémon's Weakness is Lightning (Trick Sticker).</summary>
        internal sealed class WeaknessBecomesEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public WeaknessBecomesEffect(string name, int owner, int targetUid, int expiresAfterTurn, EnergyType type) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.type = type;
            }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon != null && mon.Uid == TargetUid ? type : weakness;
            }
        }

        private sealed class TrickStickerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick Sticker") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new WeaknessBecomesEffect(defending.Name + "'s Weakness is Lightning until the end of " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Trick Sticker)",
                        ctx.Player, defending.Uid, ctx.Turn + 2, EnergyType.Lightning));
                return true;
            }
        }

        /// <summary>Parabolic Spark: discard any number of Lightning Energy from your Pokémon; 30 damage for each.</summary>
        private sealed class ParabolicSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parabolic Spark") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var en in mon.Energy)
                        if (CardsDRI1.ProvidesType(ctx, mon, en, EnergyType.Lightning)) holders[en] = mon;
                int n = 0;
                if (holders.Count > 0)
                {
                    var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Parabolic Spark: discard any number of Lightning Energy from your Pokémon (30 damage each)");
                    foreach (var card in picks)
                        if (ctx.Engine.DiscardEnergy(holders[card], card)) n++;
                }
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log("Parabolic Spark did no damage.");
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Surprise Bite: when it evolves from the hand, you may put 3 damage counters on 1 of the opponent's Pokémon. Skill Dive: 30 to any.</summary>
        private sealed class CrobatBite : Mon
        {
            private static readonly Mon Dive = new CardsMEG.SnipeMon("Skill Dive", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Surprise Bite", ctx.OpponentPokemon().Count > 0, "Surprise Bite: put 3 damage counters on 1 of your opponent's Pokémon?")) return;
                var targets = ctx.OpponentPokemon();
                var target = ctx.ChoosePokemon(targets, "Surprise Bite: put 3 damage counters on which Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dive.HandleAttack(ctx, atk); }
        }

        /// <summary>Night Attack: put 3 damage counters on 1 of the opponent's Pokémon.</summary>
        private sealed class NightAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Attack") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon();
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Night Attack: put 3 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
                return true;
            }
        }

        /// <summary>Dark Corridor: the opponent's Active Pokémon is now Poisoned; switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class DarkCorridorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Corridor") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Dream of Memories: shuffle 3 cards from your discard pile into your deck.</summary>
        private sealed class DreamOfMemoriesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream of Memories") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                var picks = pile.Count <= 3 ? pile : ctx.ChooseCards(pile, 3, 3, "Dream of Memories: shuffle which 3 cards into your deck?");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Dream Dance: both Active Pokémon are now Asleep.</summary>
        internal sealed class DreamDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream Dance") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Fainting Spell: Knocked Out by damage from the opponent's attack, flip a coin; heads, the Attacking Pokémon is Knocked Out. Cursed Drop: 6 counters.</summary>
        private sealed class ChandelureSpell : Mon
        {
            private static readonly Mon Drop = new CardsLOR1.CountersAnyWayMon("Cursed Drop", 6);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (!byOpponentsAttack || attacker == null || attacker.Owner == src.Player || src.Engine.IsKnockedOut(attacker)) return;
                Announce(src, "Fainting Spell");
                if (src.Flip()) src.Engine.KnockOut(attacker, "Fainting Spell");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drop.HandleAttack(ctx, atk); }
        }

        /// <summary>Gourgantic: with any Grass Energy attached, its maximum HP is 200. Horror Note: 10 for each card in your hand.</summary>
        private sealed class GourgeistGigantic : Mon
        {
            private static readonly Mon Note = new CardsASR1.TimesMon("Horror Note", c => c.Me.Hand.Count);

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon != src.Pokemon) return hp;
                return mon.Energy.Any(en => CardsDRI1.ProvidesType(src, mon, en, EnergyType.Grass)) ? 200 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Note.HandleAttack(ctx, atk); }
        }
    }
}
