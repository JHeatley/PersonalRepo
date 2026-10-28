using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Team Rocket Returns Uncommon and Common Pokémon (TRR 29-82; v164). Unlimited only.</summary>
    public static class CardsTRRB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TRR-29", new ComboMon(
                new CardsFST2.ConditionBonusMon("Swallow Up", 30, c => c.Defending != null && c.RemainingHp(c.Defending) < c.RemainingHp(c.Pokemon)),
                new CardsSS.ExtraPoisonMon()));
            r.RegisterByKey("TRR-30", new ComboMon(new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned), new BreakingImpactMon()));
            r.RegisterByKey("TRR-31", new DarkDragonairLight());
            r.RegisterByKey("TRR-32", new ComboMon(new CardsRS.TextDamageMon("Double Tackle", 20), new CardsASC.FlipDiscardDefendingEnergyMon("Crushing Blow")));
            r.RegisterByKey("TRR-33", new ThunderSlashMon());
            r.RegisterByKey("TRR-34", new NightAmbushMon());
            r.RegisterByKey("TRR-35", new ComboMon(new DarknessShieldMon(), new OwnChoiceColdCrushMon()));
            r.RegisterByKey("TRR-36", new DarkGyaradosScale());
            r.RegisterByKey("TRR-37", new DarkHoundoomFireBreath());
            r.RegisterByKey("TRR-38", new ComboMon(new CardsASR1.PlusMon("Energy Press", 10, c => c.Me.Active == null ? 0 : c.Me.Active.Energy.Count(e => e.Def.IsBasicEnergy)),
                new CardsSSB.AnyOpponentTextMon("Linear Attack", 40)));
            r.RegisterByKey("TRR-39", new ComboMon(new MagneticLinesMon(), new ConditionAttackMon("Poison Pulse", SpecialCondition.Poisoned)));
            r.RegisterByKey("TRR-40", new ComboMon(new ExplosiveEvolutionMon(), new CardsRS.TextDamageMon("Double Tackle", 20)));
            r.RegisterByKey("TRR-41", new ComboMon(new ParalyzeFlipMon("Dark Streak"), new CardsFLF1.NoResistanceMon("Rock Tumble")));
            r.RegisterByKey("TRR-42", new DarkWeezingMethaneLeak());
            r.RegisterByKey("TRR-43", new HeracrossCrust());
            r.RegisterByKey("TRR-44", new DumpAndDrawMon());
            r.RegisterByKey("TRR-45", new MantineRipples());
            r.RegisterByKey("TRR-46", new ComboMon(new SnatchAndRunMon(), new MiraculousComebackMon()));
            r.RegisterByKey("TRR-47", new ComboMon(new DarkAidMon(), new CardsTEU1.AmnesiaMon("Amnesia")));
            r.RegisterByKey("TRR-48", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new AquaTrickMon()));
            r.RegisterByKey("TRR-49", new CardsTRR.BufferMon(new CardsSVI1.MiraclePowderMon("Miracle Powder")));
            r.RegisterByKey("TRR-50", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep),
                new CardsTRR.CopyAttackMon("Mini-Metronome", true, c => c.Defending == null ? new List<PokemonInPlay>() : new List<PokemonInPlay> { c.Defending }, "the Defending Pokémon")));
            r.RegisterByKey("TRR-51", new ComboMon(new CardsRGB.LookForFriendsMon(), new CoinsMon("Bonemerang", 2)));
            r.RegisterByKey("TRR-52", new PlainMon());
            r.RegisterByKey("TRR-53", new ComboMon(new ConditionAttackMon("Dragon Song", SpecialCondition.Asleep), new FlipBonusMon("Tail Strike", 10)));
            r.RegisterByKey("TRR-54", new CardsP1.InsomniaMon(new CardsPAR2.FlipConditionMon("Soothing Wave", SpecialCondition.Asleep)));
            r.RegisterByKey("TRR-55", new ParalyzeFlipMon("Tail Wrap"));
            r.RegisterByKey("TRR-56", new ComboMon(new CardsBS1.LureMon("Taunt"), new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("TRR-57", new CardsTRR.BufferMon(new CardsSVI1.MiraclePowderMon("Miracle Powder")));
            r.RegisterByKey("TRR-58", new ComboMon(new WaterPlantMon(), new CardsMA.SwiftMon("Swift")));
            r.RegisterByKey("TRR-59", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("TRR-60", new CardsMAB.DarkLiftMon(new FlipBonusMon("Firebreathing", 10)));
            r.RegisterByKey("TRR-61", new KoffingKnockoutGas());
            r.RegisterByKey("TRR-62", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("TRR-63", new CardsTWM1.HealSelfMon("Dig Drain", 10));
            r.RegisterByKey("TRR-64", new GrassFamilyOneMon());
            r.RegisterByKey("TRR-65", new BenchSearchAttackMon("Call for Friends", 2));
            r.RegisterByKey("TRR-66", new CardsHL.NoWeaknessResistanceMon("Sonicboom"));
            r.RegisterByKey("TRR-67", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 1), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("TRR-68", new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("TRR-69", new ComboMon(new TailsNothingMon("Horn Rush"), new ShieldAttackMon("Granite Head", 10)));
            r.RegisterByKey("TRR-70", new HeadacheMon());
            r.RegisterByKey("TRR-71", new RattataScramble());
            r.RegisterByKey("TRR-72", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new CardsMAB.SnarlMon()));
            r.RegisterByKey("TRR-73", new FlipEnergyDrawMon());
            r.RegisterByKey("TRR-74", new CardsMA.SwiftMon("Swift"));
            r.RegisterByKey("TRR-75", new ComboMon(new FriendSearchMon(), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("TRR-76", new SlowpokeDense());
            r.RegisterByKey("TRR-77", new ComboMon(new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned), new RecoilMon("Heat Tackle", 10)));
            r.RegisterByKey("TRR-78", new ComboMon(new DrawAttackMon("Collect", 1), new FlipBonusMon("Rising Lunge", 10)));
            r.RegisterByKey("TRR-79", new FullRestMon());
            r.RegisterByKey("TRR-80", new ComboMon(new CardsRGB.PsychoWavesMon(), new ParalyzeFlipMon("Thunder Wave")));
            r.RegisterByKey("TRR-81", new CardsTRR.SaturationMon(10, null));
            r.RegisterByKey("TRR-82", new ComboMon(new ParalyzeFlipMon("Dark Streak"), new FlipBonusMon("Ambush", 10)));
        }

        /// <summary>Breaking Impact: 20 damage for each Colorless in the chosen opponent's Pokémon's Retreat Cost (after effects on it).</summary>
        private sealed class BreakingImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Breaking Impact") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Breaking Impact: choose 1 of your opponent's Pokémon");
                if (target != null) ctx.DamageTarget(target, 20 * ctx.Engine.RetreatCostOf(target));
                return true;
            }
        }

        /// <summary>Evolutionary Light (Poké-Power): once during your turn, if Dark Dragonair is Active, an Evolution card from your deck into your hand; not with a Special Condition.</summary>
        private sealed class DarkDragonairLight : Mon
        {
            protected override string Activated { get { return "Evolutionary Light"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(d => d.IsEvolution, 1, "Evolutionary Light: choose an Evolution card"); }
        }

        /// <summary>Thunder Slash: a Basic Defending Pokémon is now Paralyzed; Dark Flaaffy can't use Thunder Slash during your next turn.</summary>
        private sealed class ThunderSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Slash") return false;
                var defending = ctx.Defending;
                bool basic = defending != null && defending.Def.IsBasicPokemon;
                ctx.DamageDefending(atk.Damage);
                if (basic && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Night Ambush: 30 to 1 of the opponent's Pokémon; Dark Golbat can't attack during your next turn.</summary>
        private sealed class NightAmbushMon : Mon
        {
            private static readonly Mon Hit = new CardsSSB.AnyOpponentTextMon("Night Ambush", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!Hit.HandleAttack(ctx, atk)) return false;
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Darkness Shield: heads, during the opponent's next turn, no damage or effects of attacks on your Active Pokémon.</summary>
        private sealed class DarknessShieldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Shield") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsTRR.ActiveProtectEffect(ctx.Engine.Possessive(ctx.Player) + " Active Pokémon is protected from attacks during the opponent's next turn (Darkness Shield)", ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Cold Crush (Dark Golduck): you may discard an Energy card from it; if you do, discard an Energy from the Defending Pokémon (you choose).</summary>
        private sealed class OwnChoiceColdCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cold Crush") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Cold Crush: discard an Energy card from " + me.Name + " to discard an Energy from " + defending.Name + "?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Cold Crush: discard which Energy card?") == 0 || !ctx.CanAffect(defending)) return true;
                ctx.DiscardEnergyFrom(defending, 1, "Cold Crush: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Dark Scale (Poké-Body): if Dark Gyarados is Active and Knocked Out by an opponent's attack, 3 damage counters on the Attacking Pokémon. Sharp Fang. Dark Streak.</summary>
        private sealed class DarkGyaradosScale : Mon
        {
            private static readonly Mon Streak = new ParalyzeFlipMon("Dark Streak");

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Pokemon == null) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                Announce(src, "Dark Scale");
                src.Engine.PlaceDamageCounters(attacker, 3, "Dark Scale");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Streak.HandleAttack(ctx, atk); }
        }

        /// <summary>Fire Breath (Poké-Power): once during your turn, if Dark Houndoom is Active, flip; heads, the Defending Pokémon is Burned. Fire Payback: 20 more with fewer Benched Pokémon than the opponent.</summary>
        private sealed class DarkHoundoomFireBreath : Mon
        {
            private static readonly Mon Payback = new CardsFST2.ConditionBonusMon("Fire Payback", 20, c => c.Me.Bench.Count < c.Opp.Bench.Count);

            protected override string Activated { get { return "Fire Breath"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var theirs = ctx.Opp.Active;
                if (ctx.Flip() && ctx.InPlay(theirs)) ctx.ApplyCondition(theirs, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Payback.HandleAttack(ctx, atk); }
        }

        /// <summary>Magnetic Lines: if the opponent has 2 or more Pokémon in play, move a basic Energy card from the Defending Pokémon to another of their Pokémon.</summary>
        private sealed class MagneticLinesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Lines") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var cards = defending.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (cards.Count == 0) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Magnetic Lines: move which basic Energy card from " + defending.Name + "?")[0];
                var bench = ctx.OpponentBench();
                var to = ctx.ChoosePokemon(bench, "Magnetic Lines: move " + card.Def.Name + " to which of your opponent's Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>Explosive Evolution: heads, 10 to each of the opponent's Pokémon (no W/R); then evolve Dark Pupitar from your deck.</summary>
        private sealed class ExplosiveEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explosive Evolution") return false;
                var me = ctx.Pokemon;
                if (ctx.Flip())
                    foreach (var mon in ctx.OpponentPokemon()) ctx.DamageWith(mon, 10, false, false, false);
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                string name = me.Name;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name, 1, "Explosive Evolution: choose a card that evolves from " + name, false);
                if (found.Count > 0) ctx.Engine.EvolvePokemon(me, found[0]);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Methane Leak (Poké-Body): while Dark Weezing is your Active Pokémon, 1 more damage counter on each Poisoned Pokémon between turns. Smog. Darkness Charge: 2 damage counters on itself.</summary>
        private sealed class DarkWeezingMethaneLeak : Mon
        {
            private static readonly Mon Smog = new ConditionAttackMon("Smog", SpecialCondition.Poisoned);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                for (int side = 0; side < 2; side++)
                    foreach (var mon in src.State.Players[side].InPlay().ToList())
                        if (mon.Has(SpecialCondition.Poisoned) && src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Methane Leak");
            }

            public override int CheckupAffects(EffectContext src) { return -1; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Charge") return Smog.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, 2);
                return true;
            }
        }

        /// <summary>Crust (Poké-Body): attack damage from the opponent's Basic Pokémon is 20 less (after W/R). Get Even: 10 more per Prize card you have over the opponent.</summary>
        private sealed class HeracrossCrust : Mon
        {
            private static readonly Mon Even = new CardsASR1.PlusMon("Get Even", 10, c => Math.Max(0, c.Me.Prizes.Count - c.Opp.Prizes.Count));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.IsBasic ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Even.HandleAttack(ctx, atk); }
        }

        /// <summary>Dump and Draw: discard up to 2 Energy cards from your hand, draw 2 cards for each.</summary>
        private sealed class DumpAndDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dump and Draw") return false;
                var cards = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (cards.Count == 0) return true;
                int n = 0;
                foreach (var card in ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Dump and Draw: discard up to 2 Energy cards (draw 2 for each)"))
                    if (ctx.Me.Hand.Contains(card))
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                if (n > 0) ctx.Draw(2 * n);
                return true;
            }
        }

        /// <summary>Ripples (Poké-Power): once during your turn, 1 damage counter off 1 of your other Pokémon; not with a Special Condition. Aqua Slash.</summary>
        private sealed class MantineRipples : Mon
        {
            private static readonly Mon Slash = new LockAfterAttackMon("Aqua Slash", false);

            private static List<PokemonInPlay> Hurt(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.DamageCounters > 0); }

            protected override string Activated { get { return "Ripples"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && Hurt(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = Hurt(ctx);
                var target = ctx.ChoosePokemon(hurt, "Ripples: remove 1 damage counter from which Pokémon?");
                if (target != null) ctx.Heal(target, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slash.HandleAttack(ctx, atk); }
        }

        /// <summary>Snatch and Run: a Pokémon Tool or Rocket's Secret Machine card from your deck into your hand; if you do, you may switch Rocket's Meowth with a Benched Pokémon.</summary>
        private sealed class SnatchAndRunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snatch and Run") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchToHand(d => d.IsTool || CardsTRR.IsSecretMachine(d), 1, "Snatch and Run: choose a Pokémon Tool or Rocket's Secret Machine card");
                if (found.Count > 0 && ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Snatch and Run: switch Rocket's Meowth with 1 of your Benched Pokémon?"))
                    ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Miraculous Comeback: a coin for each Pokémon in play (both players'); 10 per heads to the Defending Pokémon, 10 per tails to Rocket's Meowth.</summary>
        private sealed class MiraculousComebackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Comeback") return false;
                int coins = ctx.MyPokemon().Count + ctx.OpponentPokemon().Count;
                int heads = ctx.FlipCoins(coins);
                ctx.DamageDefending(atk.Damage * heads);
                if (coins - heads > 0 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10 * (coins - heads));
                return true;
            }
        }

        /// <summary>Dark Aid: 1 Pokémon Tool / Rocket's Secret Machine from your discard pile into your hand, or 3 of them into your deck.</summary>
        private sealed class DarkAidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Aid") return false;
                CardsTRR.RetrieveOneOrThree(ctx, d => d.IsTool || CardsTRR.IsSecretMachine(d), "Pokémon Tool or Rocket's Secret Machine card", "Dark Aid");
                return true;
            }
        }

        /// <summary>Aqua Trick: move 1 Energy card from the Defending Pokémon to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class AquaTrickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Trick") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var cards = defending.Energy.ToList();
                var card = ctx.ChooseCards(cards, 1, 1, "Aqua Trick: move which Energy card from " + defending.Name + "?")[0];
                var bench = ctx.OpponentBench();
                var to = ctx.ChoosePokemon(bench, "Aqua Trick: move " + card.Def.Name + " to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>Water Plant: up to 2 Water Energy cards from your discard pile into your hand.</summary>
        private sealed class WaterPlantMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Plant") return false;
                var cards = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                if (cards.Count == 0) return true;
                var picks = ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Water Plant: put up to 2 Water Energy cards into your hand");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Me.Hand.Add(card);
                if (picks.Count > 0) ctx.Engine.Reveal(ctx.Player, picks, "put these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " hand (Water Plant)");
                return true;
            }
        }

        /// <summary>Knockout Gas (Poké-Body): if Koffing is Active and Knocked Out by an opponent's attack, the Attacking Pokémon is Confused and Poisoned. Gnaw.</summary>
        private sealed class KoffingKnockoutGas : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Pokemon == null) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                Announce(src, "Knockout Gas");
                src.ApplyCondition(attacker, SpecialCondition.Confused);
                src.ApplyCondition(attacker, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Call for Family: a Grass Basic Pokémon from your deck onto your Bench.</summary>
        private sealed class GrassFamilyOneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.HasType(EnergyType.Grass), 1, "Call for Family: put a Grass Basic Pokémon onto your Bench");
                return true;
            }
        }

        /// <summary>Headache: heads, the opponent can't play Trainer cards from their hand until the end of their next turn. Gentle Slap.</summary>
        private sealed class HeadacheMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Headache") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsCES3.TrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (Headache)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Scramble (Poké-Body): while the opponent's Active Pokémon is a Pokémon-ex, Rattata's Retreat Cost is 0. Fury Swipes.</summary>
        private sealed class RattataScramble : Mon
        {
            private static readonly Mon Swipes = new CoinsMon("Fury Swipes", 3);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Opp.Active != null && src.Opp.Active.Def.IsOldEx ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swipes.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Draw: heads, a basic Energy card from your deck onto Remoraid.</summary>
        private sealed class FlipEnergyDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Draw") return false;
                var me = ctx.Pokemon;
                if (!ctx.Flip() || !ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                foreach (var card in ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Energy Draw: choose a basic Energy card to attach to " + me.Name))
                    if (!ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Friend Search: look at the top 5 cards, a Basic Pokémon or Evolution card there into your hand, shuffle.</summary>
        private sealed class FriendSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Friend Search") return false;
                var top = ctx.LookAtTop(5);
                if (top.Count == 0) return true;
                ctx.Engine.NoteLookedAt(top, "The top cards of your deck", ctx.Player);
                var options = top.Where(c => c.Def.IsPokemon).ToList();
                if (options.Count > 0)
                {
                    var picks = ctx.ChooseCards(options, 0, 1, "Friend Search: put a Basic Pokémon or Evolution card into your hand");
                    foreach (var card in picks)
                        if (ctx.Me.Deck.Remove(card))
                        {
                            ctx.Me.Hand.Add(card);
                            ctx.Engine.Reveal(ctx.Player, picks, "put this card into " + ctx.Engine.Their(ctx.Player) + " hand (Friend Search)");
                        }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Dense (Poké-Body): attack damage from the opponent's Evolved Pokémon is 10 less (after W/R). Bite.</summary>
        private sealed class SlowpokeDense : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return CardsE2.IsEvolved(a) ? damage - 10 : damage;
            }
        }

        /// <summary>Rest: remove all Special Conditions and damage counters from Swinub; then it is Asleep. Tackle.</summary>
        private sealed class FullRestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                ctx.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) ctx.Heal(me, 10 * me.DamageCounters);
                if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }
    }
}
