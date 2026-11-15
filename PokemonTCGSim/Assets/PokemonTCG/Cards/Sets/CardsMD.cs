using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Majestic Dawn (MD, 2008; v172): the cards not already in the pool (the others are prints). Unlimited only. Turtwig 77 and
    /// 78 have no card text. The fossils give a Prize card when Knocked Out. Call Energy's effect is listed as an ability in the
    /// pool so the engine can offer it (GameEngine: Special Energy with an ability).
    /// </summary>
    public static partial class CardsMD
    {
        private static readonly string[] FossilNames = { "Helix Fossil", "Dome Fossil", "Old Amber" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MD-1", new BenchEntryPowerMon("Freezing Screech", FreezingScreech, c => c.Opp.Active != null, new BlizzardMon()));
            r.RegisterByKey("MD-2", new ComboMon(new CardsSW.FutureSightMon(), new HealEachMineMon("Healing Light", 10, false)));
            r.RegisterByKey("MD-3", new BenchEntryPowerMon("Darkness Shade", c => { if (c.Opp.Active != null && c.CanAffect(c.Opp.Active)) c.ApplyCondition(c.Opp.Active, SpecialCondition.Asleep); }, c => c.Opp.Active != null,
                new ComboMon(new CardsMA.DelayedConditionMon("Dark Slumber", SpecialCondition.Asleep), new DarkResolveMon())));
            r.RegisterByKey("MD-4", new ComboMon(new CardsBCR1.DrawUntilAttackMon("Time Shift", 6), new FlipBonusMon("Diamond Blast", 20)));
            r.RegisterByKey("MD-5", new ComboMon(new DodgeMon("Snow Cloak", false), new SpeedSlideMon()));
            r.RegisterByKey("MD-6", new KabutopsPrimalShell());
            r.RegisterByKey("MD-7", new ComboMon(new NoRetreatMon("Bind Down"), new CardsASC.SelfGuardMon("Leaf Guard", 20)));
            r.RegisterByKey("MD-8", new ManaphyAquaSkin());
            r.RegisterByKey("MD-9", new ComboMon(new EnergyAbsorptionMon(), new MewtwoRecoverMon()));
            r.RegisterByKey("MD-10", new BenchEntryPowerMon("Flame Charge", FlameCharge, c => c.Me.Discard.Any(e => e.Def.IsEnergy && e.Def.EnergyProvides == EnergyType.Fire),
                new CardsDP.DiscardAllTypeMon("Scorching Wing", EnergyType.Fire)));
            r.RegisterByKey("MD-11", new ComboMon(new CardsDRI1.WhirlwindMon("Zone Shift"), new PearlBlastMon()));
            r.RegisterByKey("MD-12", new ComboMon(new EvolutionWishMon(), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("MD-13", new ComboMon(new DualTransMon(), new ReflectEnergyMon()));
            r.RegisterByKey("MD-14", new BenchEntryPowerMon("Sheet Lightning", SheetLightning, c => c.Opp.HasPokemonInPlay, new RagingThunderMon()));
            r.RegisterByKey("MD-15", new AerodactylPrimalClaw());
            r.RegisterByKey("MD-16", new BronzongCursedAlloy());
            r.RegisterByKey("MD-17", new ComboMon(new CardsCES1.TwoTargetsMon("Dual Splash", 30), new SurfTogetherMon()));
            r.RegisterByKey("MD-18", new EspeonSunlightVeil());
            r.RegisterByKey("MD-19", new ComboMon(new CardsPAR2.FlipConditionMon("Fire Fang", SpecialCondition.Burned), new KindleMon()));
            r.RegisterByKey("MD-20", new ComboMon(new Cards30CC.BenchHitMon("Ice Shot", 10), new CardsPAR2.FlipConditionMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("MD-21", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Vacuum Sand", EnergyType.Fighting), new SandImpactMon()));
            r.RegisterByKey("MD-22", new ComboMon(new Cards30CC.BenchHitMon("Mach Punch", 10), new MegaBravoMon()));
            r.RegisterByKey("MD-23", new ComboMon(new ParalyzeFlipMon("Thunder Fang"), new LightningStrikeMon()));
            r.RegisterByKey("MD-24", new ComboMon(new CardsTWM1.HealSelfMon("Sprial Drain", 10), new FlipBonusMon("Leaf Blade", 20)));
            r.RegisterByKey("MD-25", new PartnerSparkMon());
            r.RegisterByKey("MD-26", new CardsDS.EvolutionaryMon("Primal Swirl", PrimalSwirl, c => !c.NamedLimitUsedThisTurn("Primal Swirl") && c.OpponentBench().Any(CardsE2.IsEvolved), new CardsDRI1.WhirlwindMon("Send Back")));
            r.RegisterByKey("MD-27", new ComboMon(new CardsMEG.WeakenDefendingMon("Charm", 20), new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool")));
            r.RegisterByKey("MD-28", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new PartnerBoostMon()));
            r.RegisterByKey("MD-29", new ComboMon(new CardsFST2.ConditionBonusMon("Special Blow", 50, c => c.Defending != null && c.Defending.Energy.Any(e => e.Def.IsSpecialEnergy)), new FlipBonusMon("X-Scissor", 40)));
            r.RegisterByKey("MD-30", new ComboMon(new OwnBenchQuakeMon("Earthquake", 10), new LockAfterAttackMon("Frenzy Plant", true)));
            r.RegisterByKey("MD-31", new ParalyzePoisonMon());
            r.RegisterByKey("MD-32", new UmbreonMoonlightVeil());
            r.RegisterByKey("MD-33", new UnownPut());
            r.RegisterByKey("MD-34", new ComboMon(new HealEachMineMon("Cleanse Away", 20, true), new HyperWhirlpoolMon()));
            r.RegisterByKey("MD-35", new ComboMon(new AstonishMon("Astonish"), new CardsSSB.AnyOpponentTextMon("Hang High", 40)));
            r.RegisterByKey("MD-36", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("MD-37", new ComboMon(new PlantingMon(), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("MD-38", new CoinsMon("Comet Punch", 4));
            r.RegisterByKey("MD-39", new ComboMon(new DrawAttackMon("Lap Up", 1), new CoinsMon("Slam", 2)));
            r.RegisterByKey("MD-40", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new VoltCrushMon()));
            r.RegisterByKey("MD-41", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new FlipBonusMon("Mid-air Strike", 30)));
            r.RegisterByKey("MD-42", new MothimDisturbanceScales());
            r.RegisterByKey("MD-43", new ComboMon(new TransTailMon(), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("MD-44", new ComboMon(new CardsSSB.AnyOpponentTextMon("Ice Blade", 30), new CardsFST1.TwoBenchedMon("Wash Over", 10)));
            r.RegisterByKey("MD-45", new ComboMon(new DodgeMon("Agility", false), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("MD-46", new CardsDEX.FuryCutterMon(40, 20, 10));
            r.RegisterByKey("MD-47", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("MD-48", new ComboMon(new CardsASR1.TimesMon("Flail", CardsCG.SelfCounters), new RecoilMon("Wood Hammer", 30)));
            r.RegisterByKey("MD-49", new UnownQuick());
            r.RegisterByKey("MD-50", new ComboMon(new DrawAttackMon("Collect", 1), new HandTrickMon()));
            r.RegisterByKey("MD-51", new TailsNothingMon("Last Resort"));
            r.RegisterByKey("MD-52", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("MD-53", new ComboMon(new SearchAttackMon("Drawup Power", d => d.IsEnergy, 1), new HealEachMineMon("Extend Ears", 10, true)));
            r.RegisterByKey("MD-54", new DodgeMon("Iron Defense", false));
            r.RegisterByKey("MD-55", new ComboMon(new CardsUNB3.MimicMon(), new NoRetreatMon("Chatter")));
            r.RegisterByKey("MD-56", new TailsNothingMon("Lunge"));
            r.RegisterByKey("MD-57", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("MD-58", new CardsSS.BabyEvolutionMon("Chimecho", new CardsDR.FlipEachOpponentMon("Uproar", 10)));
            r.RegisterByKey("MD-59", new ComboMon(new HoneyScentMon(), new CardsE2.AnyOpponentMon("Flitter", 20)));
            r.RegisterByKey("MD-60", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("MD-61", new ComboMon(new DeliveryMon(), new SwitchSelfMon("Reaction", false)));
            r.RegisterByKey("MD-62", new ComboMon(new CardsROS1.BenchFamilyMon("Call for Family", "Eevee", 5), new TailsNothingMon("Lunge")));
            r.RegisterByKey("MD-63", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("MD-64", new CardsSSB.AnyOpponentTextMon("Random Spark", 10));
            r.RegisterByKey("MD-65", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new CardsASC.FlipDiscardDefendingEnergyMon("Rip Claw")));
            r.RegisterByKey("MD-66", new ComboMon(new FlipBonusMon("Grainy Sand", 10), new RecoilMon("Double-edge", 10)));
            r.RegisterByKey("MD-67", new FossilFinderMon("Ancient Guidance", false));
            r.RegisterByKey("MD-68", new CardsSS.BabyEvolutionMon("Snorlax", new ParalyzeFlipMon("Lick")));
            r.RegisterByKey("MD-69", new FossilFinderMon("Call Up", true, new CardsFST1.TwoBenchedMon("Wash Over", 10)));
            r.RegisterByKey("MD-70", new ComboMon(new FlipBonusMon("Quick Attack", 10), new RecoilMon("Volt Tackle", 10)));
            r.RegisterByKey("MD-71", new FlipBonusMon("Water Splash", 10));
            r.RegisterByKey("MD-72", new CardsE2.AnyOpponentMon("Splatter", 10));
            r.RegisterByKey("MD-73", new Cards30CC.BenchHitMon("Muddy Water", 10));
            r.RegisterByKey("MD-74", new TailsNothingMon("Claw"));
            r.RegisterByKey("MD-75", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("MD-76", new CardsPAR2.FlipConditionMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("MD-77", new PlainMon());
            r.RegisterByKey("MD-78", new PlainMon());

            r.RegisterByKey("MD-79", new DawnStadium());
            r.RegisterByKey("MD-83", new MomsKindness());
            r.RegisterByKey("MD-84", new CardsEMB.BenchShieldMon(false, null));
            r.RegisterByKey("MD-89", new FossilReactionMon("Rock Reaction", EnergyType.Fighting));
            r.RegisterByKey("MD-91", new FossilReactionMon("Aqua Reaction", EnergyType.Water));
            r.RegisterByKey("MD-92", new CallEnergy());
            r.RegisterByKey("MD-94", new HealthEnergy());
            r.RegisterByKey("MD-96", new RecoverEnergy());
            r.RegisterByKey("MD-97", new GarchompLvXDragonPulse());
            r.RegisterByKey("MD-98", new GlaceonLvXChillyBreath());
            r.RegisterByKey("MD-99", new LeafeonLvXEnergyForcing());
            r.RegisterByKey("MD-100", new PorygonZLvXModeCrash());
        }

        /// <summary>"Once during your turn, when you put this Pokémon from your hand onto your Bench, you may..." (Freezing Screech, Darkness Shade, Flame Charge, Sheet Lightning).</summary>
        private sealed class BenchEntryPowerMon : Mon
        {
            private readonly string power;
            private readonly Action<EffectContext> effect;
            private readonly Func<EffectContext, bool> ready;
            private readonly Mon attacks;

            public BenchEntryPowerMon(string power, Action<EffectContext> effect, Func<EffectContext, bool> ready, Mon attacks)
            {
                this.power = power;
                this.effect = effect;
                this.ready = ready;
                this.attacks = attacks;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, power, ready(ctx), power + ": use it?")) return;
                effect(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        private static void FreezingScreech(EffectContext ctx)
        {
            var target = ctx.Opp.Active;
            if (ctx.Flip() && target != null && ctx.CanAffect(target)) ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
        }

        private static void FlameCharge(EffectContext ctx)
        {
            if (!ctx.Flip()) return;
            var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Fire).ToList();
            if (pile.Count == 0) return;
            foreach (var card in ctx.ChooseCards(pile, 1, Math.Min(3, pile.Count), "Flame Charge: attach up to 3 Fire Energy cards"))
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, ctx.Pokemon) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
        }

        private static void SheetLightning(EffectContext ctx)
        {
            if (!ctx.Flip()) return;
            foreach (var m in ctx.OpponentPokemon()) if (ctx.CanAffect(m)) ctx.PlaceCounters(m, 1);
        }

        /// <summary>Blizzard: heads, 10 to each of the opponent's Benched Pokémon; tails, 10 to each of yours.</summary>
        private sealed class BlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blizzard") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.Flip() ? ctx.OpponentBench() : ctx.MyBench();
                foreach (var m in bench.ToList()) ctx.DamageTarget(m, 10);
                return true;
            }
        }

        /// <summary>"Remove N damage from each of your (Benched) Pokémon" (Healing Light, Cleanse Away, Extend Ears).</summary>
        private sealed class HealEachMineMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly bool benchOnly;

            public HealEachMineMon(string attack, int amount, bool benchOnly)
            {
                this.attack = attack;
                this.amount = amount;
                this.benchOnly = benchOnly;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in (benchOnly ? ctx.MyBench() : ctx.MyPokemon()).Where(p => p.DamageCounters > 0).ToList()) ctx.Heal(m, amount);
                return true;
            }
        }

        /// <summary>Dark Resolve: if the Defending Pokémon is Asleep, remove 4 damage counters from this Pokémon.</summary>
        private sealed class DarkResolveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Resolve") return false;
                bool asleep = ctx.Defending != null && (ctx.Defending.Conditions & SpecialCondition.Asleep) != 0;
                ctx.DamageDefending(atk.Damage);
                if (asleep && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 40);
                return true;
            }
        }

        /// <summary>Speed Slide: the damage ignores Resistance and effects on the Defending Pokémon (Weakness still applies).</summary>
        private sealed class SpeedSlideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Speed Slide") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
                return true;
            }
        }

        /// <summary>Primal Shell (Poké-Body): while Kabutops is Active, the opponent can't play Trainer cards (Fossils included). Chop Up.</summary>
        private sealed class KabutopsPrimalShell : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && player != src.Player && src.IsActive(src.Pokemon) && card != null && DpTrainer(card.Def);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chop Up") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.OpponentBench().Where(b => b.DamageCounters > 0).ToList()) ctx.DamageTarget(m, 10);
                return true;
            }
        }

        /// <summary>Aqua Skin (Poké-Body): when you attach a Water Energy card from your hand to Manaphy, remove 2 damage counters from it. Chase Up, Fountain.</summary>
        private sealed class ManaphyAquaSkin : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsGE.PresentMon("Chase Up"), new FountainMon());

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || energy.Def.EnergyProvides != EnergyType.Water || target.DamageCounters == 0) return;
                Announce(src, "Aqua Skin");
                src.Heal(target, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Fountain: attach up to 2 basic Water Energy cards from your hand to your Benched Pokémon in any way (they count as from the hand: MD FAQ).</summary>
        private sealed class FountainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fountain") return false;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 2 && ctx.Me.Bench.Count > 0; i++)
                {
                    var water = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Water).ToList();
                    if (water.Count == 0) break;
                    var picks = ctx.ChooseCards(water, 0, 1, "Fountain: attach a basic Water Energy card from your hand to a Benched Pokémon? (none to stop)");
                    if (picks.Count == 0) break;
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Fountain: attach it to which Benched Pokémon?");
                    if (target == null) break;
                    ctx.Me.Hand.Remove(picks[0]);
                    ctx.Engine.AttachEnergyCard(picks[0], target, true);
                }
                return true;
            }
        }

        /// <summary>Energy Absorption: up to 2 Energy cards from your discard pile attached to this Pokémon.</summary>
        private sealed class EnergyAbsorptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Absorption") return false;
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0) return true;
                foreach (var card in ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Energy Absorption: attach which Energy cards?"))
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Recover: discard a Psychic Energy from this Pokémon and remove 6 damage counters from it.</summary>
        private sealed class MewtwoRecoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recover") return false;
                var me = ctx.Pokemon;
                if (ctx.DiscardEnergyFrom(me, 1, "Recover: discard a Psychic Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Psychic)) > 0 && me.DamageCounters > 0) ctx.Heal(me, 60);
                return true;
            }
        }

        /// <summary>Pearl Blast: you may return an Energy card from this Pokémon to your hand; if you do, an Energy card on the Defending Pokémon goes back to the opponent's hand.</summary>
        private sealed class PearlBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pearl Blast") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !ctx.AskYesNo("Pearl Blast: return an Energy card from " + me.Name + " to your hand?")) return true;
                var mine = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Pearl Blast: return which Energy card?")[0];
                if (!ctx.Engine.ReturnEnergyToHand(me, mine)) return true;
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var theirs = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Pearl Blast: return which Energy card to your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, theirs);
                return true;
            }
        }

        /// <summary>Evolution Wish: search your deck for a card that evolves from 1 of your Pokémon and evolve it (the next stage only: May 2008).</summary>
        private sealed class EvolutionWishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evolution Wish") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var names = ctx.MyPokemon().Select(m => m.Name).ToList();
                var found = ctx.SearchMyDeck(d => d.IsPokemon && !d.IsLevelUp && d.IsEvolution && names.Contains(d.EvolvesFrom), 1, "Evolution Wish: choose a card that evolves from 1 of your Pokémon");
                if (found.Count > 0)
                {
                    var card = found[0];
                    var targets = ctx.MyPokemon(m => m.Name == card.Def.EvolvesFrom);
                    var target = targets.Count == 0 ? null : ctx.ChoosePokemon(targets, "Evolution Wish: evolve which Pokémon?");
                    if (target != null && ctx.Me.Deck.Remove(card)) ctx.Engine.EvolvePokemon(target, card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Dual Trans: up to 2 basic Energy cards from your discard pile attached to 1 of your Pokémon.</summary>
        private sealed class DualTransMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Trans") return false;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return true;
                var picks = ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Dual Trans: attach which basic Energy cards?");
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Dual Trans: attach them to which Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Reflect Energy: move an Energy card from this Pokémon to 1 of your Benched Pokémon (no Bench: just the damage).</summary>
        private sealed class ReflectEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflect Energy") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Reflect Energy: move which Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Reflect Energy: move it to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Raging Thunder: also 40 to 1 of your Pokémon (no W/R).</summary>
        private sealed class RagingThunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Thunder") return false;
                ctx.DamageDefending(atk.Damage);
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Raging Thunder: do 40 damage to which of your Pokémon?");
                if (target != null) ctx.DamageWith(target, 40, false, false, false);
                return true;
            }
        }

        /// <summary>Primal Claw (Poké-Body): after an opponent's Pokémon uses a Poké-Power, 2 damage counters on it. Supersonic.</summary>
        private sealed class AerodactylPrimalClaw : Mon
        {
            private static readonly Mon Sonic = new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused);

            public override void OnPokePowerUsed(EffectContext src, PokemonInPlay user, string power)
            {
                if (src.Pokemon == null || user == null || user.Owner == src.Player || !src.InPlay(user)) return;
                Announce(src, "Primal Claw");
                src.Engine.PlaceDamageCounters(user, 2, "Primal Claw");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sonic.HandleAttack(ctx, atk); }
        }

        /// <summary>Cursed Alloy (Poké-Body): while Bronzong is Active, 1 damage counter between turns on each of the opponent's Pokémon that has a Poké-Power. Pain Amplifier, Coating.</summary>
        private sealed class BronzongCursedAlloy : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new PainAmplifierMon(), new CardsASC.SelfGuardMon("Coating", 20));

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? src.Opponent : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var targets = src.OpponentPokemon(m => m.Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower) || (m.Stack.Count > 1 && m.Def.IsLevelUp && m.Stack[m.Stack.Count - 2].Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower)));
                if (targets.Count == 0) return;
                Announce(src, "Cursed Alloy");
                foreach (var m in targets) src.Engine.PlaceDamageCounters(m, 1, "Cursed Alloy");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Pain Amplifier: 1 damage counter on each of the opponent's Pokémon that already has damage counters.</summary>
        private sealed class PainAmplifierMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pain Amplifier") return false;
                foreach (var m in ctx.OpponentPokemon(p => p.DamageCounters > 0)) if (ctx.CanAffect(m)) ctx.PlaceCounters(m, 1);
                return true;
            }
        }

        /// <summary>Delivery: heads, any 1 card from your discard pile into your hand.</summary>
        private sealed class DeliveryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delivery") return false;
                if (!ctx.Flip() || ctx.Me.Discard.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Delivery: put which card into your hand?"));
                return true;
            }
        }

        /// <summary>Surf Together: 10 more for each of your Benched Pokémon; tails, 10 to each of them.</summary>
        private sealed class SurfTogetherMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surf Together") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.Me.Bench.Count);
                if (!ctx.Flip()) foreach (var m in ctx.MyBench().ToList()) ctx.DamageTarget(m, 10);
                return true;
            }
        }

        private static bool FromEevee(PokemonInPlay m)
        {
            return m != null && m.Stack.Any(c => c.Def.IsPokemon && c.Def.EvolvesFrom == "Eevee" && !c.Def.IsLevelUp);
        }

        /// <summary>Sunlight Veil (Poké-Body): your Pokémon that evolve from Eevee get +20 HP (only 1 Sunlight Veil counts). Morning Sun.</summary>
        private sealed class EspeonSunlightVeil : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !FromEevee(mon)) return hp;
                var first = src.MyPokemon(m => m.Name == "Espeon" && src.Engine.HasAbilityNamed(m, "Sunlight Veil")).OrderBy(m => m.Uid).FirstOrDefault();
                return first == src.Pokemon ? hp + 20 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Morning Sun") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0 || !ctx.AskYesNo("Morning Sun: move an Energy card to 1 of your Benched Pokémon?")) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Morning Sun: move which Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Morning Sun: move it to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Moonlight Veil (Poké-Body): your Pokémon that evolve from Eevee have no Weakness and no Retreat Cost. Confuse Ray.</summary>
        private sealed class UmbreonMoonlightVeil : Mon
        {
            private static readonly Mon Ray = new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon != null && mon.Owner == src.Player && FromEevee(mon); }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return src.Pokemon != null && mon != null && mon.Owner == src.Player && FromEevee(mon) ? 0 : cost; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        /// <summary>Kindle: discard an Energy card from this Pokémon, then one from the Defending Pokémon.</summary>
        private sealed class KindleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kindle") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Kindle: discard an Energy card from " + me.Name);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending)) ctx.DiscardEnergyFrom(defending, 1, "Kindle: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Sand Impact: a coin for each Fighting Energy on this Pokémon; 20 more per heads.</summary>
        private sealed class SandImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Impact") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.FlipCoins(ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fighting)));
                return true;
            }
        }

        /// <summary>Mega Bravo: discard all Fire Energy from this Pokémon; 40 times the Fire Energy discarded.</summary>
        private sealed class MegaBravoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mega Bravo") return false;
                var me = ctx.Pokemon;
                int fire = ctx.Engine.EnergyCountOfType(me, EnergyType.Fire);
                foreach (var card in me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage * fire);
                return true;
            }
        }

        /// <summary>Lightning Strike: you may discard 2 Energy for base 90.</summary>
        private sealed class LightningStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Strike") return false;
                var me = ctx.Pokemon;
                bool more = me.Energy.Count >= 2 && ctx.AskYesNo("Lightning Strike: discard 2 Energy to do 90 damage?");
                if (more) ctx.DiscardEnergyFrom(me, 2, "Lightning Strike: discard 2 Energy");
                ctx.DamageDefending(more ? 90 : atk.Damage);
                return true;
            }
        }

        /// <summary>(+) Spark: with Plusle on your Bench, also 20 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class PartnerSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!atk.Name.Contains("Spark")) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Me.Bench.Any(m => m.Name == "Plusle") || ctx.OpponentBench().Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Spark: do 20 damage to which Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 20);
                return true;
            }
        }

        /// <summary>(−) Boost: 20 more with Minun on your Bench.</summary>
        private sealed class PartnerBoostMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!atk.Name.Contains("Boost")) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Bench.Any(m => m.Name == "Minun") ? 20 : 0));
                return true;
            }
        }

        /// <summary>Primal Swirl: devolve each of the opponent's Benched Evolved Pokémon by its highest Stage card (a LV.X goes too: MD FAQ); 1 Primal Swirl a turn.</summary>
        private static void PrimalSwirl(EffectContext ctx)
        {
            ctx.MarkNamedLimit("Primal Swirl");
            foreach (var m in ctx.OpponentBench().Where(CardsE2.IsEvolved).ToList())
            {
                if (!ctx.CanAffect(m)) continue;
                int n = m.Def.IsLevelUp && m.Stack.Count >= 3 ? 2 : 1;
                ctx.Engine.Devolve(m, n);
            }
        }

        /// <summary>"Does N damage to each of your Benched Pokémon" (Earthquake).</summary>
        private sealed class OwnBenchQuakeMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public OwnBenchQuakeMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench().ToList()) ctx.DamageTarget(m, amount);
                return true;
            }
        }

        /// <summary>Paralyze Poison: Poisoned; heads, also Paralyzed.</summary>
        private sealed class ParalyzePoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paralyze Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.Flip() && ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>PUT (Poké-Power): once during your turn, if Unown P is on your Bench, 1 damage counter on 1 of your non-Unown Pokémon. Hidden Power: each player discards the top card; 20 more per Unown discarded.</summary>
        private sealed class UnownPut : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => !m.Name.StartsWith("Unown", StringComparison.Ordinal)); }

            protected override string Activated { get { return "PUT"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon) && Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Targets(ctx), "PUT: put 1 damage counter on which of your Pokémon?");
                if (target != null) ctx.Engine.PlaceDamageCounters(target, 1, "PUT");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                int unown = 0;
                foreach (var p in new[] { ctx.Player, ctx.Opponent })
                    unown += ctx.Engine.DiscardTopOfDeck(p, 1).Count(c => c.Def.IsPokemon && c.Def.Name.StartsWith("Unown", StringComparison.Ordinal));
                ctx.DamageDefending(atk.Damage + 20 * unown);
                return true;
            }
        }

        /// <summary>Hyper Whirlpool: flip until tails; discard an Energy card from the Defending Pokémon for each heads.</summary>
        private sealed class HyperWhirlpoolMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Whirlpool") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    ctx.DiscardEnergyFrom(defending, 1, "Hyper Whirlpool: discard an Energy card from " + defending.Name);
                }
                return true;
            }
        }

        /// <summary>Planting: attach a Grass Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class PlantingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Planting") return false;
                ctx.DamageDefending(atk.Damage);
                var grass = ctx.Me.Hand.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Grass).ToList();
                if (grass.Count == 0) return true;
                var card = ctx.ChooseCards(grass, 1, 1, "Planting: attach which Grass Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Planting: attach it to which Pokémon?");
                if (target == null) return true;
                ctx.Me.Hand.Remove(card);
                if (!ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Hand.Contains(card) && !ctx.Me.Discard.Contains(card)) ctx.Me.Hand.Add(card);
                return true;
            }
        }

        /// <summary>Volt Crush: heads, discard an Energy from this Pokémon and 30 more.</summary>
        private sealed class VoltCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Volt Crush") return false;
                bool heads = ctx.Flip();
                if (heads && ctx.Pokemon.Energy.Count > 0) ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Volt Crush: discard an Energy");
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                return true;
            }
        }

        /// <summary>Disturbance Scales (Poké-Body): your Pokémon's attack damage to the Defending Pokémon ignores Resistance. Get Help: 30 times the Wormadam types on your Bench. Quick Touch.</summary>
        private sealed class MothimDisturbanceScales : Mon
        {
            private static readonly Mon Touch = new CardsSW.BatonPassMon("Quick Touch");

            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                return src.Pokemon != null && info.Attacker != null && info.Attacker.Owner == src.Player && info.ToOpponentsActive;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Get Help")
                {
                    int types = ctx.MyBench().Where(m => m.Name.StartsWith("Wormadam", StringComparison.Ordinal)).SelectMany(m => ctx.Engine.TypesOf(m)).Distinct().Count();
                    ctx.DamageDefending(atk.Damage * types);
                    return true;
                }
                return Touch.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Trans Tail: a Lightning Energy card from your discard pile into your hand.</summary>
        private sealed class TransTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trans Tail") return false;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList();
                if (pile.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "Trans Tail: put which Lightning Energy card into your hand?"));
                return true;
            }
        }

        /// <summary>QUICK (Poké-Power): once during your turn, if Unown Q is on your Bench, discard its attached cards and attach it to 1 of your Pokémon as a Pokémon Tool; that Pokémon's Retreat Cost is 1 less.</summary>
        private sealed class UnownQuick : Mon
        {
            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && ctx.Engine.CanAttachToolTo(ctx.Card, m)); }

            protected override string Activated { get { return "QUICK"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Pokemon.Stack.Count == 1 && Holders(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var target = ctx.ChoosePokemon(Holders(ctx), "QUICK: attach Unown Q to which of your Pokémon as a Pokémon Tool?");
                if (target == null) return;
                var card = me.Stack[0];
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.Discard.Remove(card);
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachTool(card, target);
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var holder = src.Pokemon;
                return holder != null && mon == holder && holder.HasTool(src.Card) ? Math.Max(0, cost - 1) : cost;
            }
        }

        /// <summary>Hand Trick: this Pokémon and its cards go back to your hand (no Benched Pokémon: nothing).</summary>
        private sealed class HandTrickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hand Trick") return false;
                if (ctx.Me.Bench.Count == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Honey Scent: remove 2 damage counters from 1 of your Pokémon.</summary>
        private sealed class HoneyScentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Honey Scent") return false;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var target = hurt.Count == 0 ? null : ctx.ChoosePokemon(hurt, "Honey Scent: remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Ancient Guidance / Call Up (Poké-Power): once during your turn, heads puts Helix Fossil, Dome Fossil or Old Amber from your deck (or discard pile) onto your Bench. Not with a Special Condition.</summary>
        private sealed class FossilFinderMon : Mon
        {
            private readonly string power;
            private readonly bool fromDiscard;
            private readonly Mon attacks;

            public FossilFinderMon(string power, bool fromDiscard, Mon attacks = null)
            {
                this.power = power;
                this.fromDiscard = fromDiscard;
                this.attacks = attacks;
            }

            private List<CardInstance> Pile(EffectContext ctx) { return (fromDiscard ? ctx.Me.Discard : ctx.Me.Deck).Where(c => FossilNames.Contains(c.Def.Name)).ToList(); }

            protected override string Activated { get { return power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.BenchSpace > 0 && (fromDiscard ? Pile(ctx).Count > 0 : ctx.Me.Deck.Count > 0); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                if (fromDiscard)
                {
                    var pile = Pile(ctx);
                    if (pile.Count == 0) return;
                    var card = ctx.ChooseCards(pile, 1, 1, power + ": put which Fossil onto your Bench?")[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    return;
                }
                var found = ctx.SearchMyDeck(d => FossilNames.Contains(d.Name), 1, power + ": choose Helix Fossil, Dome Fossil or Old Amber");
                if (found.Count > 0 && ctx.Me.Deck.Remove(found[0])) ctx.Engine.PutOntoBench(ctx.Player, found[0], false);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ------------------------------------------------------------------ Trainers and Energy

        /// <summary>Dawn Stadium (Stadium): whenever a player attaches an Energy card from their hand to a Grass or Water Pokémon, remove 1 damage counter and all Special Conditions from it.</summary>
        private sealed class DawnStadium : CardScript
        {
            private static void Soothe(EffectContext src, PokemonInPlay target)
            {
                if (target == null || !(src.Engine.IsType(target, EnergyType.Grass) || src.Engine.IsType(target, EnergyType.Water))) return;
                if (target.DamageCounters > 0) src.Heal(target, 10);
                src.Engine.RemoveAllConditions(target);
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Soothe(src, target); }

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Soothe(src, target); }
        }

        /// <summary>Mom's Kindness (Supporter): draw 2 cards.</summary>
        private sealed class MomsKindness : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(2); }
        }

        /// <summary>Rock / Aqua Reaction (Poké-Body): when you attach a [type] Energy card from your hand to this Fossil, evolve it from your deck.</summary>
        private sealed class FossilReactionMon : Mon
        {
            private readonly string body;
            private readonly EnergyType type;

            public FossilReactionMon(string body, EnergyType type)
            {
                this.body = body;
                this.type = type;
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || energy.Def.EnergyProvides != type || src.Me.Deck.Count == 0) return;
                Announce(src, body);
                CardsEVS1.EvolveFromDeck(src, target, body);
            }
        }

        /// <summary>Call Energy: once during your turn, if its Pokémon is Active, up to 2 Basic Pokémon from your deck onto your Bench; if you do, your turn ends.</summary>
        private sealed class CallEnergy : Mon
        {
            protected override string Activated { get { return "Call Energy"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsMyTurn && ctx.Pokemon != null && ctx.IsActive(ctx.Pokemon) && ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchToBench(d => !d.IsLevelUp, 2, "Call Energy: choose up to 2 Basic Pokémon");
                ctx.ShuffleMyDeck();
                if (found.Count == 0) return;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + "'s turn ends (Call Energy).");
                ctx.Engine.RequestEndTurn();
            }

            public override List<int> ProvidedEnergy(EffectContext ctx) { return new List<int> { EnergyMask.Colorless }; }
        }

        /// <summary>Health Energy: when attached from your hand, remove 1 damage counter from that Pokémon.</summary>
        private sealed class HealthEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon != null && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 10);
            }
        }

        /// <summary>Recover Energy: when attached from your hand, remove all Special Conditions from that Pokémon.</summary>
        private sealed class RecoverEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon != null) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
            }
        }

        // ------------------------------------------------------------------ LV.X

        /// <summary>
        /// Dragon Pulse (Poké-Power): when you level up your Active Garchomp to this from your hand, flip 3 coins; for each heads, 1
        /// damage counter on each of the opponent's Benched Pokémon. Restore: a non-LV.X Pokémon from your discard pile onto your Bench
        /// as a Basic Pokémon; then you may attach up to 3 basic Energy cards from your discard pile to it.
        /// </summary>
        private sealed class GarchompLvXDragonPulse : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!ctx.PokemonIsActive || !TriggerOnce(ctx, "Dragon Pulse", ctx.OpponentBench().Count > 0, "Dragon Pulse: flip 3 coins?")) return;
                int heads = ctx.FlipCoins(3);
                for (int i = 0; i < heads; i++)
                    foreach (var m in ctx.OpponentBench()) if (ctx.CanAffect(m)) ctx.PlaceCounters(m, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Restore") return false;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon && !c.Def.IsLevelUp).ToList();
                if (pile.Count == 0 || ctx.BenchSpace == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Restore: put which Pokémon onto your Bench as a Basic Pokémon?")[0];
                if (!ctx.Me.Discard.Remove(card)) return true;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                var energy = ctx.BasicEnergyInDiscard();
                if (mon == null || energy.Count == 0 || !ctx.AskYesNo("Restore: attach up to 3 basic Energy cards from your discard pile to " + mon.Name + "?")) return true;
                foreach (var e in ctx.ChooseCards(energy, 1, Math.Min(3, energy.Count), "Restore: attach which basic Energy cards?"))
                    if (ctx.Me.Discard.Remove(e) && !ctx.Engine.AttachEnergyCard(e, mon) && !ctx.Me.Discard.Contains(e)) ctx.Me.Discard.Add(e);
                return true;
            }
        }

        /// <summary>Chilly Breath (Poké-Body): while Glaceon is Active, the opponent's Pokémon can't use Poké-Powers. Avalanche: heads, 20 to each of the opponent's Benched Pokémon.</summary>
        private sealed class GlaceonLvXChillyBreath : Mon
        {
            private static readonly Mon Avalanche = new FlipGateMon("Avalanche", new CardsSS.AlsoEachBenchMon("Avalanche", 20));

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && src.IsActive(src.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Avalanche.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Forcing (Poké-Power): once during your turn, attach an Energy card from your hand to 1 of your Pokémon. Not with a Special Condition. Verdant Dance: 10 more per Energy on all your Pokémon.</summary>
        private sealed class LeafeonLvXEnergyForcing : Mon
        {
            protected override string Activated { get { return "Energy Forcing"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Any(c => c.Def.IsEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList(), 1, 1, "Energy Forcing: attach which Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Energy Forcing: attach it to which Pokémon?");
                if (target == null) return;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Verdant Dance") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)));
                return true;
            }
        }

        /// <summary>
        /// Mode Crash (Poké-Power): when you level up your Active Porygon-Z to this from your hand, you may discard all of the
        /// opponent's Special Energy cards in play. Decode (Poké-Power): once during your turn, search your deck for up to 2 cards,
        /// shuffle, and put them on top in any order. Not with a Special Condition.
        /// </summary>
        private sealed class PorygonZLvXModeCrash : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool any = ctx.OpponentPokemon().Any(m => m.Energy.Any(e => e.Def.IsSpecialEnergy));
                if (!ctx.PokemonIsActive || !TriggerOnce(ctx, "Mode Crash", any, "Mode Crash: discard all of your opponent's Special Energy cards in play?")) return;
                foreach (var m in ctx.OpponentPokemon())
                {
                    if (!ctx.CanAffect(m)) continue;
                    foreach (var e in m.Energy.Where(x => x.Def.IsSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(m, e);
                }
            }

            protected override string Activated { get { return "Decode"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, Math.Min(2, ctx.Me.Deck.Count), "Decode: choose up to 2 cards to put on top of your deck");
                foreach (var c in picks) ctx.Me.Deck.Remove(c);
                ctx.ShuffleMyDeck();
                CardsCRZ1.PutOnTopInOrder(ctx, picks, "Decode");
            }
        }
    }
}
