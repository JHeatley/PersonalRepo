using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>Secret Wonders: Uncommons, Commons and Trainers (Rares and LV.X are in CardsSW.cs).</summary>
    public static partial class CardsSW
    {
        private static bool UnownOnBench(EffectContext ctx, params string[] letters)
        {
            return letters.All(l => ctx.Me.Bench.Any(m => m.Name == "Unown [" + l + "]" || m.Name == "Unown " + l));
        }

        private static void RegisterUncommonsAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("SW-21", new ComboMon(new BalefulWindMon(), new RaidMon()));
            r.RegisterByKey("SW-22", new CardsDS.EvolutionaryMon("Flame Dash", FlameDash, c => c.Me.Bench.Contains(c.Pokemon) && c.Me.Active != null, new RecoilMon("Inferno Onrush", 40)));
            r.RegisterByKey("SW-23", new ComboMon(new GhostHeadMon(), new SpitefulPainMon()));
            r.RegisterByKey("SW-24", new ComboMon(new DigTrapMon(), new PitTrapMon()));
            r.RegisterByKey("SW-25", new ElectivireMotorDrive());
            r.RegisterByKey("SW-26", new ElectrodeEnergyShift());
            r.RegisterByKey("SW-27", new ComboMon(new KeenEyeMon(), new BatonPassMon("Baton Pass")));
            r.RegisterByKey("SW-28", new ComboMon(new EncoreMon(), new BreakBeamMon()));
            r.RegisterByKey("SW-29", new ComboMon(new CardsCES1.TwoTargetsMon("Double Throw", 30), new MegatonRockMon()));
            r.RegisterByKey("SW-30", new ComboMon(new IcyKissMon(), new LovelyKissMon()));
            r.RegisterByKey("SW-31", new MagmortarFlameBody());
            r.RegisterByKey("SW-32", new MinunMinusCharge());
            r.RegisterByKey("SW-33", new ComboMon(new SilverWindMon(), new CardsASC.SelfDamagedBonusMon("Raging Scales", 40)));
            r.RegisterByKey("SW-34", new ComboMon(new PoisonRubMon(), new PrideAttackMon()));
            r.RegisterByKey("SW-35", new ComboMon(new MirrorMoveMon(), new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("SW-36", new PlusleplusCharge());
            r.RegisterByKey("SW-37", new CardsASC.CounterattackMon("Rough Skin", 2, new StrikeWoundMon()));
            r.RegisterByKey("SW-38", new SunfloraGrassWhistle());
            r.RegisterByKey("SW-39", new UnownSet());
            r.RegisterByKey("SW-40", new WeavileDarkEngage());
            r.RegisterByKey("SW-41", new WormadamPlantCloak());
            r.RegisterByKey("SW-42", new WormadamSandyCloak());
            r.RegisterByKey("SW-43", new WormadamTrashCloak());
            r.RegisterByKey("SW-44", new XatuPsychicShift());
            r.RegisterByKey("SW-45", new ComboMon(new DrainPunchMon(), new HomingUppercutMon()));
            r.RegisterByKey("SW-46", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new FlareTailMon()));
            r.RegisterByKey("SW-47", new ComboMon(new CardsSP1.FlipNoDamageMon("Withdraw"), new SpineMissileMon()));
            r.RegisterByKey("SW-48", new ComboMon(new CardsFST2.RapidSpinMon("Rapid Spin"), new CardsFST2.ConditionBonusMon("Bash In", 60, c => c.Defending != null && c.Defending.Tools.Count > 0)));
            r.RegisterByKey("SW-49", new LeekSlapMon());
            r.RegisterByKey("SW-50", new ComboMon(new SearchAttachMon("Attract Current", d => CardsRS.BasicOf(d, EnergyType.Lightning)), new CardsXYP1.FlipTailsRecoilMon("Electromagnetic Kick", 10)));
            r.RegisterByKey("SW-51", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("SW-52", new KecleonCamouflage());
            r.RegisterByKey("SW-53", new ComboMon(new PsychicResearchMon(), new CardsE2.AnyOpponentMon("Telekinesis", 40)));
            r.RegisterByKey("SW-54", new ComboMon(new FlipSupporterLockMon("Torment"), new CardsTWM1.HealSelfMon("Absorb", 20)));
            r.RegisterByKey("SW-55", new ComboMon(new HealingMilkMon(), new CardsE2.FlipUntilTailsMon("Continuous Tumble")));
            r.RegisterByKey("SW-56", new MukToxicSludge());
            r.RegisterByKey("SW-57", new ComboMon(new SpiritedDrillMon(), new ConditionAttackMon("Poison Horn", SpecialCondition.Poisoned)));
            r.RegisterByKey("SW-58", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("SW-59", new ComboMon(new NoRetreatMon("Grip and Squeeze"), new PowerGuillotineMon()));
            r.RegisterByKey("SW-60", new QuagsireAquaHealing());
            r.RegisterByKey("SW-61", new ComboMon(new FlipBonusMon("Gnaw Off", 60), new CardsFST2.ConditionBonusMon("Sneaky Attack", 20, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Darkness) > 0)));
            r.RegisterByKey("SW-62", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("SW-63", new ComboMon(new RummageMon(), new CardsFST2.ConditionBonusMon("Shadow Sneak", 20, c => c.Me.Prizes.Count != c.Opp.Prizes.Count)));
            r.RegisterByKey("SW-64", new ProtectChargeMon());
            r.RegisterByKey("SW-65", new SkiploomCottonBalloon());
            r.RegisterByKey("SW-66", new ComboMon(new SearchAttackMon("Color Pick", d => d.IsBasicEnergy, 3),
                new CardsTRR.CopyAttackMon("Trace", true, c => c.OpponentBench(), "your opponent's Benched Pokémon")));
            r.RegisterByKey("SW-67", new CardsUF.BabyEvolutionAnyMon(new[] { "Jynx" }, new PsykissMon()));
            r.RegisterByKey("SW-68", new UnownKind());
            r.RegisterByKey("SW-69", new UnownNod());
            r.RegisterByKey("SW-70", new UnownOne());
            r.RegisterByKey("SW-71", new UnownXRay());
            r.RegisterByKey("SW-72", new UnownZero());
            r.RegisterByKey("SW-73", new VenomothDangerousScales());
            r.RegisterByKey("SW-74", new ComboMon(new CardsLTR1.SpreadMon("Sand Wind", 10), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("SW-75", new LockAfterAttackMon("Giant Wave", true));
            r.RegisterByKey("SW-76", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("SW-77", new ComboMon(new ConditionAttackMon("Shake Vine", SpecialCondition.Asleep), new CoinsMon("Bullet Seed", 4)));
            r.RegisterByKey("SW-78", new BurmyWearCloak(EnergyType.Grass, "Plant Cloak Tackle"));
            r.RegisterByKey("SW-79", new BurmyWearCloak(EnergyType.Fighting, "Sandy Cloak Tackle"));
            r.RegisterByKey("SW-80", new BurmyWearCloak(EnergyType.Metal, "Trash Cloak Tackle"));
            r.RegisterByKey("SW-81", new CardsRS.FlipHandDiscardMon("Bite Away"));
            r.RegisterByKey("SW-82", new Cards30CC.BenchHitMon("Lava Burn", 10));
            r.RegisterByKey("SW-83", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("SW-84", new RallyMon());
            r.RegisterByKey("SW-85", new DownDrawMon());
            r.RegisterByKey("SW-86", new FlipGateMon("Astonish", new AstonishMon("Astonish")));
            r.RegisterByKey("SW-87", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CardsFST1.FlipRecoilMon("Thunder", 30)));
            r.RegisterByKey("SW-88", new CardsMEG.SandAttackMon("Gunky"));
            r.RegisterByKey("SW-89", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("SW-90", new HoppipCottonweed());
            r.RegisterByKey("SW-91", new ComboMon(new ParalyzeFlipMon("Lick"), new FlipBonusMon("Stomp", 30)));
            r.RegisterByKey("SW-92", new CardsFST2.ConditionBonusMon("Water Sport", 20, c => c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending)));
            r.RegisterByKey("SW-93", new ConditionAttackMon("Smog", SpecialCondition.Poisoned));
            r.RegisterByKey("SW-94", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("SW-95", new CardsCES1.FeintAttackMon("Feint Attack", 20));
            r.RegisterByKey("SW-96", new ComboMon(new BotheringWaveMon("Bothering Wave"), new FutureSightMon()));
            r.RegisterByKey("SW-97", new ComboMon(new DrawAttackMon("Collect", 1), new PoisonSkinMon()));
            r.RegisterByKey("SW-98", new ComboMon(new CardsASR1.TimesMon("Flail", CardsCG.SelfCounters), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("SW-99", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("SW-100", new ComboMon(new DelusionMon(), new MigraineMon()));
            r.RegisterByKey("SW-101", new QwilfishBalloonSting());
            r.RegisterByKey("SW-102", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("SW-103", new CardsLOR1.EntanglingStringMon("Tail Whip"));
            r.RegisterByKey("SW-104", new GropeMon());
            r.RegisterByKey("SW-105", new CardsE3B.HealSelfMon("Shell Rest", 20));
            r.RegisterByKey("SW-106", new CardsSS.BenchNamedMon("Shell Hunting", new[] { "Shellos West Sea", "Shellos East Sea" }, false, 2));
            r.RegisterByKey("SW-107", new RecoverMon());
            r.RegisterByKey("SW-108", new CardsPAR2.FlipConditionMon("Toxic Spore", SpecialCondition.Poisoned));
            r.RegisterByKey("SW-109", new ShucklePotShell());
            r.RegisterByKey("SW-110", new CardsUF.CountersAnyOpponentMon("Gossip", 1));
            r.RegisterByKey("SW-111", new ComboMon(new DishOutMon(), new SynchroPunchMon()));
            r.RegisterByKey("SW-112", new ComboMon(new ParalyzeFlipMon("Bubble"), new AquaTailMon()));
            r.RegisterByKey("SW-113", new ComboMon(new SearchAttackMon("Lead", d => d.IsSupporter, 1), new CardsCG.IfDefendingConditionMon("Frighten Horn", SpecialCondition.Confused, m => !CardsE2.IsEvolved(m))));
            r.RegisterByKey("SW-114", new SunSoakMon());
            r.RegisterByKey("SW-115", new ComboMon(new CardsDAA1.GustAttackMon("Inviting Trap", false), new NoRetreatMon("Sand Tomb")));
            r.RegisterByKey("SW-116", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new CardsPAR2.FlipConditionMon("Signal Beam", SpecialCondition.Confused)));
            r.RegisterByKey("SW-117", new RecoilMon("Big Explosion", 50));
            r.RegisterByKey("SW-118", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new CardsE2.AnyOpponentMon("Splatter", 10)));

            r.RegisterByKey("SW-122", new ProfessorOaksVisit());
            r.RegisterByKey("SW-125", new RoseannesResearch());
            r.RegisterByKey("SW-126", new TeamGalacticsMars());
        }

        // ------------------------------------------------------------------ Uncommons

        /// <summary>Baleful Wind: discard a random card from the opponent's hand; a Trainer, Supporter or Stadium means 1 more.</summary>
        private sealed class BalefulWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Baleful Wind") return false;
                ctx.DamageDefending(atk.Damage);
                var card = CardsHP.RandomOpponentCard(ctx);
                if (card == null) return true;
                bool trainer = card.Def.IsTrainer;
                ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                if (!trainer) return true;
                var more = CardsHP.RandomOpponentCard(ctx);
                if (more != null) ctx.Engine.DiscardFromHand(ctx.Opponent, more);
                return true;
            }
        }

        /// <summary>Raid: base 40 if you played this Pokémon from your hand this turn (not during setup: Secret Wonders FAQ).</summary>
        private sealed class RaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raid") return false;
                ctx.DamageDefending(ctx.Pokemon.TurnEnteredPlay == ctx.Turn ? 40 : atk.Damage);
                return true;
            }
        }

        /// <summary>Flame Dash: when Arcanine evolves a Benched Pokémon, switch it with your Active Pokémon, then move any Energy cards from that Pokémon to it.</summary>
        private static void FlameDash(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            var old = ctx.Me.Active;
            if (old == null || !ctx.Me.Bench.Contains(me) || !ctx.Engine.SwitchActive(ctx.Player, me)) return;
            for (int i = 0; i < 20 && old.Energy.Count > 0 && ctx.InPlay(old); i++)
            {
                var picks = ctx.ChooseCards(old.Energy.ToList(), 0, 1, "Flame Dash: move which Energy card from " + old.Name + " to " + me.Name + "? (none to stop)");
                if (picks.Count == 0) break;
                ctx.Engine.MoveEnergy(old, me, picks[0]);
            }
        }

        /// <summary>Ghost Head: put any number of damage counters on Banette (it can't Knock itself Out: errata) and as many on the Defending Pokémon.</summary>
        private sealed class GhostHeadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ghost Head") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                int max = Math.Max(0, ctx.RemainingHp(me) / 10 - 1);
                if (max == 0 || defending == null) return Nothing(ctx);
                int n = ctx.AskNumber(0, max, "Ghost Head: put how many damage counters on " + me.Name + "?");
                if (n == 0) return true;
                ctx.Engine.PlaceDamageCounters(me, n, "Ghost Head");
                if (ctx.CanAffect(defending)) ctx.PlaceCounters(defending, n);
                return true;
            }
        }

        /// <summary>Spiteful Pain: with a Banette in your discard pile, 40 more, and that Banette is shuffled into your deck.</summary>
        private sealed class SpitefulPainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiteful Pain") return false;
                var banette = ctx.Me.Discard.FirstOrDefault(c => c.Def.IsPokemon && c.Def.Name == "Banette");
                ctx.DamageDefending(atk.Damage + (banette != null ? 40 : 0));
                if (banette != null && ctx.Me.Discard.Contains(banette))
                {
                    ctx.DiscardToDeck(new[] { banette });
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>
        /// Dig Trap: tails, nothing; heads, prevent all effects of attacks (damage included) on Dugtrio next turn, and if it's still
        /// your Active Pokémon at the end of that turn, 6 damage counters on 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class DigTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Trap") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                ctx.ProtectSelfNextTurn();
                ctx.AddEffect(new DigTrapEffect(me.Name + " springs its trap at the end of the opponent's next turn (Dig Trap)", ctx.Player, me.Uid, ctx.Turn + 1, ctx.Card.Def));
                return true;
            }
        }

        private sealed class DigTrapEffect : ContinuousEffect
        {
            private readonly int turn;
            private readonly CardDef source;

            public DigTrapEffect(string name, int owner, int targetUid, int expiresAfterTurn, CardDef source) : base(name, owner, targetUid, expiresAfterTurn, false)
            {
                turn = expiresAfterTurn;
                this.source = source;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != turn) return;
                var me = engine.State.Players[Owner].Active;
                var bench = engine.State.Players[1 - Owner].Bench.ToList();
                if (me == null || me.Uid != TargetUid || bench.Count == 0) return;
                var target = engine.ChoosePokemon(Owner, bench, "Dig Trap: put 6 damage counters on which Benched Pokémon?", source);
                if (target != null) engine.PlaceDamageCounters(target, 6, "Dig Trap");
            }
        }

        /// <summary>Pit Trap: your opponent flips a coin; tails, 50 more.</summary>
        private sealed class PitTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pit Trap") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Engine.FlipCoin(ctx.Opponent, "Pit Trap") ? 0 : 50));
                return true;
            }
        }

        /// <summary>Motor Drive (Poké-Power): once during your turn, attach a Lightning Energy card from your discard pile to Electivire. Not with a Special Condition. Discharge: discard all Lightning Energy cards; 50 per heads, a coin for each.</summary>
        private sealed class ElectivireMotorDrive : Mon
        {
            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList(); }

            protected override string Activated { get { return "Motor Drive"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && Pile(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(Pile(ctx), 1, 1, "Motor Drive: attach which Lightning Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, ctx.Pokemon) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Discharge") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList();
                foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(lightning.Count));
                return true;
            }
        }

        /// <summary>
        /// Energy Shift (Poké-Power): if Electrode is Knocked Out by damage from an attack (errata: any attack), it isn't discarded:
        /// attach it as an Energy card to 1 of your Pokémon. Attached, it's a Special Energy card that provides every type but only 1
        /// Energy at a time. It still counts as Knocked Out. Ion Blast: you may do 60 more and 100 to itself.
        /// </summary>
        private sealed class ElectrodeEnergyShift : Mon
        {
            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                var card = src.Card;
                if (me == null || card == null || !(byOpponentsAttack || me.HitByAttack) || !src.Me.Discard.Contains(card)) return;
                var targets = src.MyPokemon();
                if (targets.Count == 0 || !src.AskYesNo("Energy Shift: attach Electrode to 1 of your Pokémon as an Energy card?")) return;
                var target = src.ChoosePokemon(targets, "Energy Shift: attach Electrode to which Pokémon?");
                if (target == null) return;
                Announce(src, "Energy Shift");
                if (src.Me.Discard.Remove(card) && !src.Engine.AttachEnergyCard(card, target) && !src.Me.Discard.Contains(card)) src.Me.Discard.Add(card);
            }

            public override bool ReactsToOwnKnockOutWithoutAbility { get { return true; } }

            public override List<int> ProvidedEnergy(EffectContext ctx) { return new List<int> { EnergyMask.AnyType }; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ion Blast") return false;
                bool more = ctx.AskYesNo("Ion Blast: do 60 more damage (and 100 damage to Electrode)?");
                ctx.DamageDefending(atk.Damage + (more ? 60 : 0));
                if (more) ctx.DamageSelf(100);
                return true;
            }
        }

        /// <summary>Keen Eye: search your deck for up to 2 cards (at least 1: Dec 2008) for your hand.</summary>
        private sealed class KeenEyeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Keen Eye") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var picks = ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, Math.Min(2, ctx.Me.Deck.Count), "Keen Eye: put which cards into your hand?");
                ctx.DeckToHand(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Baton Pass: you may switch this Pokémon with a Benched Pokémon and move any of its Energy cards to the new Active Pokémon.</summary>
        internal sealed class BatonPassMon : Mon
        {
            private readonly string attack;

            public BatonPassMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0 || !ctx.AskYesNo(attack + ": switch " + me.Name + " with 1 of your Benched Pokémon?")) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), attack + ": switch in which Pokémon?");
                if (target == null || !ctx.Engine.SwitchActive(ctx.Player, target)) return true;
                for (int i = 0; i < 20 && me.Energy.Count > 0; i++)
                {
                    var picks = ctx.ChooseCards(me.Energy.ToList(), 0, 1, attack + ": move which Energy card to " + target.Name + "? (none to stop)");
                    if (picks.Count == 0) break;
                    ctx.Engine.MoveEnergy(me, target, picks[0]);
                }
                return true;
            }
        }

        /// <summary>Encore: choose 1 of the Defending Pokémon's attacks; it can use only that attack next turn.</summary>
        private sealed class EncoreMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Encore") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Def.Attacks.Count == 0) return true;
                int index = ChooseAttackOf(ctx, defending.Def, "Encore: the Defending Pokémon can use only which attack?");
                if (index < 0) return true;
                string keep = defending.Def.Attacks[index].Name;
                ctx.AddEffect(new EncoreEffect(defending.Name + " can use only " + keep + " during its next turn (Encore)", ctx.Player, defending.Uid, ctx.Turn + 1, keep));
                return true;
            }
        }

        private sealed class EncoreEffect : ContinuousEffect
        {
            private readonly string keep;

            public EncoreEffect(string name, int owner, int targetUid, int expiresAfterTurn, string keep) : base(name, owner, targetUid, expiresAfterTurn, true) { this.keep = keep; }

            public override bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string attackName) { return mon != null && mon.Uid == TargetUid && attackName != keep; }
        }

        /// <summary>Break Beam: you may do 20 more; if you do, this Pokémon is now Confused.</summary>
        private sealed class BreakBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Break Beam") return false;
                var me = ctx.Pokemon;
                bool more = ctx.AskYesNo("Break Beam: do 20 more damage (and " + me.Name + " is Confused)?");
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                if (more && ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Megaton Rock: you may do 40 instead of 80; if you do, attack damage to Golem is 40 less next turn (after W/R).</summary>
        private sealed class MegatonRockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Megaton Rock") return false;
                bool soft = ctx.AskYesNo("Megaton Rock: do 40 damage instead of " + atk.Damage + " (and take 40 less damage next turn)?");
                ctx.DamageDefending(soft ? 40 : atk.Damage);
                if (soft && ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(40);
                return true;
            }
        }

        /// <summary>Icy Kiss: if this Pokémon was damaged by an attack during the opponent's last turn, the Defending Pokémon is Paralyzed.</summary>
        private sealed class IcyKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Icy Kiss") return false;
                var defending = ctx.Defending;
                bool hurt = ctx.Pokemon.AttackDamageTurn == ctx.OpponentsLastTurn;
                ctx.DamageDefending(atk.Damage);
                if (hurt && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Lovely Kiss: move 2 damage counters (4 with Smoochum under it) from this Pokémon to the Defending Pokémon.</summary>
        private sealed class LovelyKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lovely Kiss") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                int n = Math.Min(EvolvedFrom(ctx, "Smoochum") ? 4 : 2, me.DamageCounters);
                if (n > 0 && defending != null && ctx.CanAffect(defending)) ctx.Engine.MoveDamageCounters(me, defending, n, "Lovely Kiss");
                return true;
            }
        }

        /// <summary>Flame Body (Poké-Body): when you attach a Fire Energy card from your hand to Magmortar, remove 2 damage counters from it. Flame Blast, Fireball Bazooka.</summary>
        private sealed class MagmortarFlameBody : Mon
        {
            private static readonly Mon Attacks = new ComboMon(
                new CardsASR1.TimesMon("Flame Blast", c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fire)),
                new CardsFST1.TwoBenchedMon("Fireball Bazooka", 20));

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || energy.Def.EnergyProvides != EnergyType.Fire || target.DamageCounters == 0) return;
                Announce(src, "Flame Body");
                src.Heal(target, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Minus Charge (Poké-Power): once during your turn, if any Pokémon was Knocked Out during the opponent's last turn, draw 2 cards (1 Minus Charge a turn). Not with a Special Condition. Tag Play −.</summary>
        private sealed class MinunMinusCharge : Mon
        {
            protected override string Activated { get { return "Minus Charge"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0 && (ctx.Me.LastKnockedOutTurn == ctx.OpponentsLastTurn || ctx.Opp.LastKnockedOutTurn == ctx.OpponentsLastTurn);
            }

            protected override void Ability(EffectContext ctx) { ctx.Draw(2); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tag Play —" && atk.Name != "Tag Play -" && atk.Name != "Tag Play −") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !ctx.Me.Bench.Any(m => m.Name == "Plusle") || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Tag Play: move an Energy card from " + me.Name + " to 1 of your Benched Pokémon?")) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Tag Play: move which Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Tag Play: move it to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Plus Charge (Poké-Power): once during your turn, if any of your Pokémon was Knocked Out during the opponent's last turn, up to 2 basic Energy cards from your discard pile into your hand (1 Plus Charge a turn). Not with a Special Condition. Tag Play +.</summary>
        private sealed class PlusleplusCharge : Mon
        {
            protected override string Activated { get { return "Plus Charge"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.WasKnockedOutLastOpponentTurn() && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy) && !ctx.Engine.DiscardToHandBlocked(ctx.Player);
            }

            protected override void Ability(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Plus Charge: put which basic Energy cards into your hand?"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!atk.Name.StartsWith("Tag Play", StringComparison.Ordinal)) return false;
                bool partner = ctx.Me.Bench.Any(m => m.Name == "Minun");
                if (partner && ctx.OpponentBench().Count > 0 && ctx.AskYesNo("Tag Play: do 20 damage to 1 of your opponent's Benched Pokémon instead?"))
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Tag Play: do 20 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 20);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Silver Wind: during your next turn, attacks that damage the Defending Pokémon (after W/R) do 40 more to it.</summary>
        private sealed class SilverWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silver Wind") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new SilverWindEffect(defending.Name + " takes 40 more damage from attacks during the next turn (Silver Wind)", ctx.Player, defending.Uid, ctx.Turn + 2, 40));
                return true;
            }
        }

        /// <summary>"During your next turn, if an attack does damage to the Defending Pokémon (after W/R), it does N more" (Silver Wind, Jaw Bite).</summary>
        internal sealed class SilverWindEffect : ContinuousEffect
        {
            private readonly int turn;
            private readonly int amount;

            public SilverWindEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                turn = expiresAfterTurn;
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || damage <= 0 || info.Attacker == null || info.Attacker.Owner != Owner) return damage;
                if (engine.State.TurnNumber != turn || engine.CurrentSourceKind != EffectSourceKind.Attack || !info.TargetIsActive) return damage;
                return damage + amount;
            }
        }

        /// <summary>Poison Rub: before the damage, you may switch in 1 of the opponent's Benched Pokémon; the Defending Pokémon is now Poisoned.</summary>
        private sealed class PoisonRubMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Rub") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Poison Rub: switch in 1 of your opponent's Benched Pokémon first?"))
                    ctx.GustOpponent("Poison Rub: switch in which Benched Pokémon?");
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Pride Attack: a coin for each Nidoqueen on your Bench; 30 more per heads.</summary>
        private sealed class PrideAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pride Attack") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.FlipCoins(ctx.Me.Bench.Count(m => m.Name == "Nidoqueen")));
                return true;
            }
        }

        /// <summary>Mirror Move: if this Pokémon was damaged by an attack during the opponent's last turn, the same amount of damage to the Defending Pokémon.</summary>
        private sealed class MirrorMoveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Move") return false;
                var me = ctx.Pokemon;
                if (me.AttackDamageTurn != ctx.OpponentsLastTurn || me.AttackDamageThatTurn <= 0) return Nothing(ctx);
                ctx.DamageDefending(me.AttackDamageThatTurn);
                return true;
            }
        }

        /// <summary>Strike Wound: 20 more if the Defending Pokémon has 2 or more damage counters; the damage ignores W/R and effects on it.</summary>
        private sealed class StrikeWoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strike Wound") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                ctx.DamageWith(defending, atk.Damage + (defending.DamageCounters >= 2 ? 20 : 0), false, false, true);
                return true;
            }
        }

        /// <summary>Grass Whistle (Poké-Power): once during your turn, remove 1 damage counter from each of your Grass Pokémon (1 Grass Whistle a turn). Not with a Special Condition. Petal Dance.</summary>
        private sealed class SunfloraGrassWhistle : Mon
        {
            protected override string Activated { get { return "Grass Whistle"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Grass)).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var m in ctx.MyPokemon(p => p.DamageCounters > 0 && ctx.Engine.IsType(p, EnergyType.Grass))) ctx.Heal(m, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Petal Dance") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Set (Poké-Power): once during your turn, with Unown S, E and T on your Bench, heads puts an Energy card from your discard pile on top of your deck. Hidden Power: heads, Confused.</summary>
        private sealed class UnownSet : Mon
        {
            private static readonly Mon Hidden = new CardsPAR2.FlipConditionMon("Hidden Power", SpecialCondition.Confused);

            protected override string Activated { get { return "Set"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownOnBench(ctx, "S", "E", "T") && ctx.Me.Discard.Any(c => c.Def.IsEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var card = ctx.ChooseCards(ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList(), 1, 1, "Set: put which Energy card on top of your deck?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Insert(0, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Engage (Poké-Power): once during your turn, your Active Pokémon is Darkness until the end of your turn (ends if it leaves the Active Spot). Shadow Charge, Chip Off.</summary>
        private sealed class WeavileDarkEngage : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsMT.SearchAttachAnyWayMon("Shadow Charge", EnergyType.Darkness, 2), new ChipOffMon());

            protected override string Activated { get { return "Dark Engage"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                ctx.AddEffect(new DarkTypeEffect(active.Name + " is Darkness this turn (Dark Engage)", ctx.Player, active.Uid, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        private sealed class DarkTypeEffect : ContinuousEffect
        {
            public DarkTypeEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void ModifyTypes(GameEngine engine, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon.Uid != TargetUid) return;
                types.Clear();
                types.Add(EnergyType.Darkness);
            }
        }

        /// <summary>Chip Off: if the opponent has 6 or more cards in hand, discard random cards until they have 5.</summary>
        private sealed class ChipOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chip Off") return false;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 60 && ctx.Opp.Hand.Count >= 6; i++)
                {
                    var card = CardsHP.RandomOpponentCard(ctx);
                    if (card == null) break;
                    ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                }
                return true;
            }
        }

        /// <summary>Plant Cloak (Poké-Body): with 2 or more Grass Energy attached, +40 HP. Leaf Hurricane: Confused if the Defending Pokémon has fewer HP left than this one (checked before the damage: Jul 2008).</summary>
        private sealed class WormadamPlantCloak : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon != null && mon == src.Pokemon && src.Engine.EnergyCountOfType(mon, EnergyType.Grass) >= 2 ? hp + 40 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Hurricane") return false;
                var defending = ctx.Defending;
                bool weaker = defending != null && ctx.RemainingHp(defending) < ctx.RemainingHp(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage);
                if (weaker && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Sandy Cloak (Poké-Body): prevent all effects of the opponent's attacks, except damage, done to it (errata). Push Over.</summary>
        private sealed class WormadamSandyCloak : Mon
        {
            private static readonly Mon Push = new CardsASR1.PlusMon("Push Over", 10, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fighting));

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Push.HandleAttack(ctx, atk); }
        }

        /// <summary>Trash Cloak (Poké-Body): with a Pokémon Tool attached, attack damage to it is 20 less (after W/R). Iron Tackle.</summary>
        private sealed class WormadamTrashCloak : Mon
        {
            private static readonly Mon Tackle = new RecoilMon("Iron Tackle", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || me.Tools.Count == 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Shift (Poké-Power): once during your turn, if Xatu is on your Bench, move 1 Special Condition from your Active Pokémon to the Defending Pokémon (the plain condition: Jan 2008). Energy Singe.</summary>
        private sealed class XatuPsychicShift : Mon
        {
            private static readonly SpecialCondition[] All = { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override string Activated { get { return "Psychic Shift"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Active != null && ctx.Me.Active.Conditions != SpecialCondition.None && ctx.Opp.Active != null;
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var has = All.Where(c => (active.Conditions & c) != 0).ToList();
                if (has.Count == 0) return;
                int pick = has.Count == 1 ? 0 : ctx.ChooseOneOption("Psychic Shift: move which Special Condition?", has.Select(c => c.ToString()).ToArray());
                var condition = has[Math.Max(0, Math.Min(has.Count - 1, pick))];
                active.Conditions &= ~condition;
                if (condition == SpecialCondition.Poisoned) active.PoisonCounterBase = 1;
                ctx.Log(active.Name + " is no longer " + condition + " (Psychic Shift).");
                var target = ctx.Opp.Active;
                if (target != null && ctx.CanAffect(target)) ctx.ApplyCondition(target, condition);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Singe") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Energy Singe: choose 1 of your opponent's Pokémon");
                if (target != null) ctx.DamageTarget(target, 20 + 10 * ctx.EnergyCount(target));
                return true;
            }
        }

        /// <summary>Drain Punch: remove as many damage counters from this Pokémon as the amount of Energy on the Defending Pokémon.</summary>
        private sealed class DrainPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drain Punch") return false;
                var me = ctx.Pokemon;
                int energy = ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending);
                ctx.DamageDefending(atk.Damage);
                if (energy > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10 * energy);
                return true;
            }
        }

        /// <summary>Homing Uppercut: 60 more if the Defending Pokémon's Retreat Cost is 0.</summary>
        private sealed class HomingUppercutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Homing Uppercut") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && ctx.Engine.RetreatCostOf(ctx.Defending) == 0 ? 60 : 0));
                return true;
            }
        }

        /// <summary>Flare Tail: heads, discard a Fire Energy from this Pokémon and 20 more.</summary>
        private sealed class FlareTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Tail") return false;
                var me = ctx.Pokemon;
                bool heads = ctx.Flip();
                if (heads) ctx.DiscardEnergyFrom(me, 1, "Flare Tail: discard a Fire Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                return true;
            }
        }

        /// <summary>Spine Missile: flip 4 coins first; for each heads, 20 to an opponent's Pokémon you choose (the same one can be chosen again; the hits on one Pokémon add up first: Dec 2007).</summary>
        private sealed class SpineMissileMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spine Missile") return false;
                int heads = ctx.FlipCoins(4);
                var totals = new Dictionary<PokemonInPlay, int>();
                for (int i = 0; i < heads; i++)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Spine Missile: do 20 damage to which Pokémon? (" + (i + 1) + " of " + heads + ")");
                    if (target == null) break;
                    int t;
                    totals[target] = (totals.TryGetValue(target, out t) ? t : 0) + 20;
                }
                foreach (var kv in totals) if (ctx.InPlay(kv.Key)) ctx.DamageTarget(kv.Key, kv.Value);
                return true;
            }
        }

        /// <summary>Swords Dance: during your next turn Leek Slap's base damage is 60. Leek Slap: can't be used during your next turn.</summary>
        private sealed class LeekSlapMon : Mon
        {
            private const string Marker = "Swords Dance";
            private static readonly Mon Lock = new LockAfterAttackMon("Leek Slap", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == Marker)
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Leek Slap") return false;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 60 : atk.Damage);
                if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn("Leek Slap");
                return true;
            }
        }

        /// <summary>Camouflage (Poké-Body): if a basic Energy card attached to Kecleon matches the Attacking Pokémon's type, its attack damage to Kecleon is 40 less (after W/R). Blind Scratch.</summary>
        private sealed class KecleonCamouflage : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                var types = src.Engine.TypesOf(a);
                return me.Energy.Any(e => e.Def.IsBasicEnergy && types.Contains(e.Def.EnergyProvides)) ? damage - 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blind Scratch") return false;
                if (ctx.Flip())
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Blind Scratch: do 20 damage to which of your Pokémon?");
                if (target != null) ctx.DamageWith(target, 20, target == ctx.Me.Active, target == ctx.Me.Active, false);
                return true;
            }
        }

        /// <summary>Psychic Research: use the effect of a Supporter card in your discard pile (it stays there).</summary>
        private sealed class PsychicResearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Research") return false;
                if (!UseSupporterFrom(ctx, ctx.Me.Discard, "Psychic Research")) return Nothing(ctx);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, your opponent can't play Supporter cards from their hand during their next turn" (Torment).</summary>
        internal sealed class FlipSupporterLockMon : Mon
        {
            private readonly string attack;

            public FlipSupporterLockMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsTWM2.SupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards during their next turn (" + attack + ")", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Healing Milk: flip 2 coins; for each heads, remove 3 damage counters from 1 of your Pokémon.</summary>
        private sealed class HealingMilkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Milk") return false;
                int heads = ctx.FlipCoins(2);
                for (int i = 0; i < heads; i++)
                {
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count == 0) break;
                    var target = ctx.ChoosePokemon(hurt, "Healing Milk: remove 3 damage counters from which Pokémon?");
                    if (target != null) ctx.Heal(target, 30);
                }
                return true;
            }
        }

        /// <summary>Toxic Sludge (Poké-Body): at the end of each player's turn, the opponent's Active Pokémon with Grass Energy attached is Poisoned (no effect if it already is). Panic Liquid.</summary>
        private sealed class MukToxicSludge : Mon
        {
            private static readonly Mon Liquid = new ConditionNoRetreatMon("Panic Liquid", SpecialCondition.Confused, false);

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var target = src.Opp.Active;
                if (src.Pokemon == null || target == null || (target.Conditions & SpecialCondition.Poisoned) != 0 || src.Engine.EnergyCountOfType(target, EnergyType.Grass) == 0) return;
                Announce(src, "Toxic Sludge");
                src.ApplyCondition(target, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Liquid.HandleAttack(ctx, atk); }
        }

        /// <summary>Spirited Drill: during your next turn, this Pokémon's attacks do 20 more to the Defending Pokémon (before W/R).</summary>
        private sealed class SpiritedDrillMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirited Drill") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && defending != null)
                    ctx.AddEffect(new DrillEffect(ctx.Pokemon.Name + "'s attacks do 20 more to " + defending.Name + " next turn (Spirited Drill)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, defending.Uid, 20));
                return true;
            }
        }

        /// <summary>"During your next turn, each of this Pokémon's attacks does N more to the Defending Pokémon (before W/R)" (Spirited Drill, Bulk Up). It stays on the attacker.</summary>
        internal sealed class DrillEffect : ContinuousEffect
        {
            private readonly int turn;
            private readonly int defendingUid;
            private readonly int amount;

            public DrillEffect(string name, int owner, int targetUid, int expiresAfterTurn, int defendingUid, int amount) : base(name, owner, targetUid, expiresAfterTurn, false)
            {
                turn = expiresAfterTurn;
                this.defendingUid = defendingUid;
                this.amount = amount;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target == null || target.Uid != defendingUid || damage <= 0) return damage;
                if (engine.State.TurnNumber != turn || engine.CurrentSourceKind != EffectSourceKind.Attack || engine.State.Players[target.Owner].Active != target) return damage;
                return damage + amount;
            }
        }

        /// <summary>Power Guillotine: flip 2 coins; if either is tails, base 10.</summary>
        private sealed class PowerGuillotineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Guillotine") return false;
                ctx.DamageDefending(ctx.FlipCoins(2) == 2 ? atk.Damage : 10);
                return true;
            }
        }

        /// <summary>Aqua Healing (Poké-Power): once during your turn, if Quagsire is Active and the Defending Pokémon has Water Energy, remove 3 damage counters from Quagsire. Muddy Water.</summary>
        private sealed class QuagsireAquaHealing : Mon
        {
            private static readonly Mon Water = new Cards30CC.BenchHitMon("Muddy Water", 20);

            protected override string Activated { get { return "Aqua Healing"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Pokemon.DamageCounters > 0 && ctx.Opp.Active != null && ctx.Engine.EnergyCountOfType(ctx.Opp.Active, EnergyType.Water) > 0;
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Pokemon, 30); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Water.HandleAttack(ctx, atk); }
        }

        /// <summary>Rummage: look at the top 5 cards; any Trainer cards there go into your hand; shuffle.</summary>
        private sealed class RummageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rummage") return false;
                var top = ctx.LookAtTop(5);
                var trainers = top.Where(c => DpTrainer(c.Def)).ToList();
                if (trainers.Count > 0) ctx.DeckToHand(ctx.ChooseCards(trainers, 0, trainers.Count, "Rummage: put which Trainer cards into your hand?"));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Focus Energy: during your next turn Protect Charge's base damage is 80. Protect Charge: attack damage to this Pokémon is 30 less next turn (after W/R).</summary>
        private sealed class ProtectChargeMon : Mon
        {
            private const string Marker = "Focus Energy";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == Marker)
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Protect Charge") return false;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 80 : atk.Damage);
                if (ctx.InPlay(me)) ctx.SelfTakesLessNextTurn(30);
                return true;
            }
        }

        /// <summary>Cotton Balloon (Poké-Body): with Grass Energy attached, attack damage from the opponent's Evolved Pokémon is 20 less (after W/R). U-turn.</summary>
        private sealed class SkiploomCottonBalloon : Mon
        {
            private static readonly Mon Turn = new SwitchSelfMon("U-turn", false);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || a == null || a.Owner == src.Player || !CardsE2.IsEvolved(a) || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.EnergyCountOfType(me, EnergyType.Grass) > 0 ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Turn.HandleAttack(ctx, atk); }
        }

        /// <summary>Psykiss: heads, a Special Energy card on 1 of the opponent's Pokémon is shuffled into their deck.</summary>
        private sealed class PsykissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psykiss") return false;
                ctx.DamageDefending(atk.Damage);
                var holders = ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsSpecialEnergy) && ctx.CanAffect(m));
                if (!ctx.Flip() || holders.Count == 0) return true;
                var holder = ctx.ChoosePokemon(holders, "Psykiss: choose a Special Energy card on which Pokémon?");
                if (holder == null) return true;
                var card = ctx.ChooseCards(holder.Energy.Where(e => e.Def.IsSpecialEnergy).ToList(), 1, 1, "Psykiss: shuffle which Special Energy card into your opponent's deck?")[0];
                holder.Energy.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Kind (Poké-Power): once during your turn, remove 2 damage counters from the Defending Pokémon.</summary>
        private sealed class UnownKind : Mon
        {
            protected override string Activated { get { return "Kind"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Active.DamageCounters > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Opp.Active, 20); }
        }

        /// <summary>Nod (Poké-Power): once during your turn, with Unown N, O and D on your Bench, ask the opponent to take a Prize card; if they do, you take one too; if not, draw a card. Hidden Power: heads, Burned.</summary>
        private sealed class UnownNod : Mon
        {
            private static readonly Mon Hidden = new CardsPAR2.FlipConditionMon("Hidden Power", SpecialCondition.Burned);

            protected override string Activated { get { return "Nod"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownOnBench(ctx, "N", "O", "D"); }

            protected override void Ability(EffectContext ctx)
            {
                bool take = ctx.Opp.Prizes.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Nod: take a Prize card? (If you do, your opponent takes one too.)", ctx.Source);
                if (take)
                {
                    ctx.Engine.TakePrizeCards(ctx.Opponent, 1);
                    if (!ctx.State.IsOver && ctx.Me.Prizes.Count > 0) ctx.Engine.TakePrizeCards(ctx.Player, 1);
                }
                else ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>ONE (Poké-Power): once during your turn, with Unown O, N and E on your Bench and 1 card in your hand, draw until you have 7. Hidden Power: heads, Asleep.</summary>
        private sealed class UnownOne : Mon
        {
            private static readonly Mon Hidden = new CardsPAR2.FlipConditionMon("Hidden Power", SpecialCondition.Asleep);

            protected override string Activated { get { return "ONE"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownOnBench(ctx, "O", "N", "E") && ctx.Me.Hand.Count == 1 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(7); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>X-RAY (Poké-Power): once during your turn, if Unown X is on your Bench, look at the top card of the opponent's deck. Hidden Power: heads, Paralyzed.</summary>
        private sealed class UnownXRay : Mon
        {
            private static readonly Mon Hidden = new ParalyzeFlipMon("Hidden Power");

            protected override string Activated { get { return "X-RAY"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Deck.Take(1).ToList(), "X-RAY", "X-RAY: the top card of your opponent's deck", ctx.Source);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>ZERO (Poké-Power): once during your turn, if Unown Z is on your Bench and your deck is empty, discard its attached cards and put it on top of your deck. Hidden Power: move damage counters from your Unown to the Defending Pokémon.</summary>
        private sealed class UnownZero : Mon
        {
            protected override string Activated { get { return "ZERO"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Deck.Count == 0 && ctx.Pokemon.Stack.Count == 1; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var card = me.Stack[0];
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.Discard.Remove(card);
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Insert(0, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                foreach (var unown in ctx.MyPokemon(m => m.Name.StartsWith("Unown", StringComparison.Ordinal) && m.DamageCounters > 0))
                {
                    int n = ctx.AskNumber(0, unown.DamageCounters, "Hidden Power: move how many damage counters from " + unown.Name + "?");
                    if (n > 0 && ctx.InPlay(defending)) ctx.Engine.MoveDamageCounters(unown, defending, n, "Hidden Power");
                }
                return true;
            }
        }

        /// <summary>Dangerous Scales (Poké-Body): while Active, when damaged by the opponent's attack (even if Knocked Out), the Attacking Pokémon is Asleep and Poisoned. Disturbance Dive.</summary>
        private sealed class VenomothDangerousScales : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                Announce(ctx, "Dangerous Scales");
                ctx.ApplyCondition(attacker, SpecialCondition.Asleep);
                if (ctx.InPlay(attacker)) ctx.ApplyCondition(attacker, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disturbance Dive") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new NoAttackEffectsEffect(ctx.Pokemon.Name + " is protected from attack effects other than damage next turn (Disturbance Dive)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class NoAttackEffectsEffect : ContinuousEffect
        {
            public NoAttackEffectsEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind) { return target != null && target.Uid == TargetUid && kind == EffectSourceKind.Attack; }
        }

        /// <summary>Wear Cloak (Poké-Power): once during your turn, if Burmy is Active, attach a basic [type] Energy card from your discard pile to it. [Cloak] Tackle: 10 more with [type] Energy attached.</summary>
        private sealed class BurmyWearCloak : Mon
        {
            private readonly EnergyType type;
            private readonly string tackle;

            public BurmyWearCloak(EnergyType type, string tackle)
            {
                this.type = type;
                this.tackle = tackle;
            }

            private List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == type).ToList(); }

            protected override string Activated { get { return "Wear Cloak"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && Pile(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = Pile(ctx)[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, ctx.Pokemon) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != tackle) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Engine.EnergyCountOfType(ctx.Pokemon, type) > 0 ? 10 : 0));
                return true;
            }
        }

        private static List<EnergyType> TypesOf(CardDef d)
        {
            var t = new List<EnergyType> { d.PokemonType };
            if (d.SecondType.HasValue && d.SecondType.Value != d.PokemonType) t.Add(d.SecondType.Value);
            return t;
        }

        /// <summary>Rally: up to 3 Basic Pokémon of different types from your deck onto your Bench.</summary>
        private sealed class RallyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rally") return false;
                var taken = new List<EnergyType>();
                for (int i = 0; i < 3 && ctx.BenchSpace > 0; i++)
                {
                    var got = ctx.SearchToBench(d => d.IsBasicPokemon && !d.IsLevelUp && !TypesOf(d).Any(taken.Contains), 1, "Rally: choose a Basic Pokémon of a type you haven't chosen (" + (i + 1) + " of 3)");
                    if (got.Count == 0) break;
                    taken.AddRange(TypesOf(got[0].Def));
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Down Draw: draw the bottom card of your deck.</summary>
        private sealed class DownDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Down Draw") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.DeckToHand(new[] { ctx.Me.Deck[ctx.Me.Deck.Count - 1] });
                return true;
            }
        }

        /// <summary>Cottonweed (Poké-Body): with Grass Energy attached, Hoppip's Retreat Cost is 0. Hover Heal: remove all Special Conditions from it.</summary>
        private sealed class HoppipCottonweed : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && src.Engine.EnergyCountOfType(mon, EnergyType.Grass) > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hover Heal") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Bothering Wave: tails, nothing; heads, discard an Energy card from the Defending Pokémon.</summary>
        internal sealed class BotheringWaveMon : Mon
        {
            private readonly string attack;

            public BotheringWaveMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, attack + ": discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Future Sight: look at the top 5 cards of either player's deck and put them back in any order.</summary>
        internal sealed class FutureSightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Future Sight") return false;
                bool mine = ctx.Opp.Deck.Count == 0 || (ctx.Me.Deck.Count > 0 && ctx.ChooseOption("Future Sight: look at whose deck?", "Your deck", "Your opponent's deck") == 0);
                var deck = mine ? ctx.Me.Deck : ctx.Opp.Deck;
                if (deck.Count == 0) return true;
                var top = deck.Take(5).ToList();
                foreach (var card in top) deck.Remove(card);
                if (mine) CardsCRZ1.PutOnTopInOrder(ctx, top, "Future Sight");
                else
                {
                    var order = new List<CardInstance>();
                    var left = top.ToList();
                    while (left.Count > 1)
                    {
                        var pick = ctx.ChooseCards(left, 1, 1, "Future Sight: which card goes next from the top of your opponent's deck?")[0];
                        order.Add(pick);
                        left.Remove(pick);
                    }
                    order.AddRange(left);
                    deck.InsertRange(0, order);
                }
                return true;
            }
        }

        /// <summary>Poison Skin: flip 2 coins; if either is heads, the Defending Pokémon is Poisoned.</summary>
        private sealed class PoisonSkinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Skin") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Delusion: if this Pokémon is Confused, draw 2 cards.</summary>
        private sealed class DelusionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delusion") return false;
                if ((ctx.Pokemon.Conditions & SpecialCondition.Confused) != 0) ctx.Draw(2);
                return true;
            }
        }

        /// <summary>Migraine: heads, the Defending Pokémon is Confused; tails, this Pokémon is.</summary>
        private sealed class MigraineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Migraine") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) { if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused); }
                else if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Balloon Sting (Poké-Power): once during the opponent's turn, if Qwilfish is Active and damaged by an attack (even if Knocked Out), heads Poisons the Attacking Pokémon with 2 counters between turns. Nerve Shot.</summary>
        private sealed class QwilfishBalloonSting : Mon
        {
            private static readonly Mon Shot = new ParalyzeFlipMon("Nerve Shot");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0 || ctx.AbilityUsedThisTurn("Balloon Sting")) return;
                if (!ctx.AskYesNo("Balloon Sting: flip a coin to Poison " + attacker.Name + "?")) return;
                ctx.MarkAbilityUsed("Balloon Sting");
                Announce(ctx, "Balloon Sting");
                if (ctx.Flip() && ctx.ApplyCondition(attacker, SpecialCondition.Poisoned)) attacker.PoisonCounterBase = Math.Max(attacker.PoisonCounterBase, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shot.HandleAttack(ctx, atk); }
        }

        /// <summary>Grope: look at the top 2 cards; 1 into your hand, the other on the bottom of your deck.</summary>
        private sealed class GropeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grope") return false;
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return true;
                var keep = ctx.ChooseCards(top, 1, 1, "Grope: put which card into your hand?")[0];
                ctx.DeckToHand(new[] { keep });
                foreach (var card in top.Where(c => c != keep))
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Deck.Add(card);
                return true;
            }
        }

        /// <summary>Recover: discard a Water Energy from this Pokémon and remove all its damage counters.</summary>
        private sealed class RecoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recover") return false;
                var me = ctx.Pokemon;
                if (ctx.DiscardEnergyFrom(me, 1, "Recover: discard a Water Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)) > 0 && me.DamageCounters > 0)
                    ctx.Heal(me, me.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Pot Shell (Poké-Body): prevent all effects of attacks, damage included, from the opponent's Pokémon with a Special Energy
        /// card attached. Ferment Poison: Poisoned, and while it stays Poisoned from this attack, it can't use Poké-Bodies.
        /// </summary>
        private sealed class ShucklePotShell : Mon
        {
            private static bool Special(PokemonInPlay a) { return a != null && a.Energy.Any(e => e.Def.IsSpecialEnergy); }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != src.Player && Special(info.Attacker) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && !src.IsMyTurn && Special(src.Opp.Active);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ferment Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned))
                    ctx.AddEffect(new FermentEffect(defending.Name + " can't use Poké-Bodies while Poisoned (Ferment Poison)", ctx.Player, defending.Uid, ctx.Turn + 1000));
                return true;
            }
        }

        private sealed class FermentEffect : ContinuousEffect
        {
            public FermentEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool StopsPokeBody(GameEngine engine, PokemonInPlay mon, string bodyName)
            {
                return mon != null && mon.Uid == TargetUid && (mon.Conditions & SpecialCondition.Poisoned) != 0;
            }
        }

        /// <summary>Dish Out: draw the top and the bottom card of your deck.</summary>
        private sealed class DishOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dish Out") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.Draw(1);
                if (ctx.Me.Deck.Count > 0) ctx.DeckToHand(new[] { ctx.Me.Deck[ctx.Me.Deck.Count - 1] });
                return true;
            }
        }

        /// <summary>Synchro Punch: 30 more if a basic Energy card on this Pokémon is the same type as any Energy on the Defending Pokémon.</summary>
        private sealed class SynchroPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchro Punch") return false;
                var defending = ctx.Defending;
                var mine = ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().ToList();
                bool match = defending != null && mine.Any(t => ctx.Engine.EnergyCountOfType(defending, t) > 0);
                ctx.DamageDefending(atk.Damage + (match ? 30 : 0));
                return true;
            }
        }

        /// <summary>Aqua Tail: a coin for each Water Energy on this Pokémon; 10 more per heads.</summary>
        private sealed class AquaTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Tail") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.FlipCoins(ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water)));
                return true;
            }
        }

        /// <summary>Sun Soak: heads, remove all damage counters from this Pokémon and evolve it from your deck (shuffle).</summary>
        private sealed class SunSoakMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sun Soak") return false;
                var me = ctx.Pokemon;
                if (!ctx.Flip()) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == me.Name && !d.IsLevelUp, 1, "Sun Soak: choose a card that evolves from " + me.Name);
                if (found.Count > 0 && ctx.InPlay(me)) ctx.Engine.EvolvePokemon(me, found[0]);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ------------------------------------------------------------------ Trainers

        /// <summary>Professor Oak's Visit (Supporter): draw 3 cards, then put a card from your hand on the bottom of your deck. Not with an empty deck.</summary>
        private sealed class ProfessorOaksVisit : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count == 0) return;
                var card = ctx.ChooseCards(hand, 1, 1, "Professor Oak's Visit: put which card on the bottom of your deck?")[0];
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Add(card);
            }
        }

        /// <summary>Roseanne's Research (Supporter): up to 2 Basic Pokémon and basic Energy cards in any combination from your deck into your hand (not LV.X: Sep 2008).</summary>
        private sealed class RoseannesResearch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => (d.IsBasicPokemon && !d.IsLevelUp) || d.IsBasicEnergy, 2, "Roseanne's Research: choose up to 2 Basic Pokémon and basic Energy cards");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Team Galactic's Mars (Supporter): draw 2 cards, then a random card from the opponent's hand goes on the bottom of their deck. Not with an empty deck.</summary>
        private sealed class TeamGalacticsMars : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                var card = CardsHP.RandomOpponentCard(ctx);
                if (card == null) return;
                ctx.Opp.Hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Log("A card from " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand was put on the bottom of their deck (Team Galactic's Mars).");
            }
        }
    }
}
