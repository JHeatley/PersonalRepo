using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Shining Legends (SLG, v87), Pokémon (SLG 1-57). No regulation mark: Expanded. The Shining Pokémon have no special rule;
    /// they're ordinary Basic Pokémon. Mewtwo-GX (Hidden Fates Shiny Vault) and Zorua were already in the pool and keep their
    /// scripts. "A [T] Energy card" outside play is basic Energy only (your ruling, v65).
    /// </summary>
    public static class CardsSLG1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("SLG-2", new CardsCES1.ConditionsMon("Toxic Whip", SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("SLG-3", new VenusaurSLG());
            r.RegisterByKey("SLG-4", new CardsTWM1.HealSelfMon("Absorb", 20));
            r.RegisterByKey("SLG-5", new DeepSleepMon("Hibernation Spore"));
            r.RegisterByKey("SLG-6", new ComboMon(new FlickPoisonMon(), new CardsDRI2.CrunchMon("Crunch")));
            r.RegisterByKey("SLG-7", new ComboMon(new VictiniTEF(), new CardsFST2.ConditionBonusMon("Rally Back", 90, c => c.WasKnockedOutByAttackLastOpponentTurn())));
            r.RegisterByKey("SLG-8", new VirizionSLG());
            r.RegisterByKey("SLG-9", new GenesectSLG());
            // Fire
            r.RegisterByKey("SLG-10", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new CardsMEG.SnipeBenchMon("Brave Burn-GX", 150)));
            r.RegisterByKey("SLG-11", new ComboMon(new CardsASC.NextTurnBoostMon("High-Pressure Heat", 50), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("SLG-13", new VolcaronaSLG());
            r.RegisterByKey("SLG-14", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new LockAfterAttackMon("Scorching Breath", false)));
            r.RegisterByKey("SLG-16", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("SLG-17", new ComboMon(new CardsFST2.ConditionBonusMon("Profane Punch", 80, c => c.Me.Bench.Any(m => m.DamageCounters > 0)),
                new CardsASC.SelfDiscardTypeMon("Flare Blitz", EnergyType.Fire, 99)));
            // Water
            r.RegisterByKey("SLG-19", new CroconawSLG());
            r.RegisterByKey("SLG-20", new CardsDRI2.CrunchMon("Crunch"));
            r.RegisterByKey("SLG-21", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Shocking Sting", 50, c => c.Opp.Active != null && c.Opp.Active.Conditions != SpecialCondition.None)));
            r.RegisterByKey("SLG-23", new RecoilMon("Aqua Bomb", 30));
            r.RegisterByKey("SLG-24", new CardsTWM1.HealSelfMon("Spiral Drain", 30));
            r.RegisterByKey("SLG-25", new ManaphySLG());
            r.RegisterByKey("SLG-26", new ComboMon(new WaterBailOutMon(), new CardsASR1.PlusMon("Resolute Blade", 20, c => c.Opp.Bench.Count)));
            r.RegisterByKey("SLG-27", new ComboMon(new CardsCES1.TwoTargetsMon("Dual Pump", 50), new CoinsMon("Quad Smash", 4)));
            // Lightning
            r.RegisterByKey("SLG-28", new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"));
            r.RegisterByKey("SLG-29", new ComboMon(new CardsASR1.PlusMon("Powerful Spark", 20, c => c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Lightning))),
                new RecoilMon("Thunder", 30), new ConditionAttackMon("Voltail-GX", SpecialCondition.Paralyzed)));
            r.RegisterByKey("SLG-31", new CardsCRZ1.PiercingMon("Swift"));
            r.RegisterByKey("SLG-32", new DiscardEnergyToBenchMon("Booming Thunder", EnergyType.Lightning));
            r.RegisterByKey("SLG-33", new CardsFST2.ConditionBonusMon("Tag Team Boost", 50, c => c.Me.Bench.Any(m => m.Def.Name == "Minun")));
            r.RegisterByKey("SLG-34", new ComboMon(new DrawAttackMon("Collect", 1), new Cards30CC.BenchHitMon("Spark", 20)));
            r.RegisterByKey("SLG-35", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new CardsTWM1.SelfDiscardMon("Storm Blade", 2)));
            // Psychic
            r.RegisterByKey("SLG-36", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("SLG-37", new ArbokSLG());
            r.RegisterByKey("SLG-38", new ComboMon(new CardsDAA1.RandomHandToDeckMon("Whiny Voice", 1), new ConditionAttackMon("Feverish Kiss", SpecialCondition.Confused)));
            r.RegisterByKey("SLG-40", new LegendaryGuidanceMon());
            r.RegisterByKey("SLG-41", new Cards30CC.BenchHitMon("Break Through", 30));
            r.RegisterByKey("SLG-42", new StellarReignMon());
            r.RegisterByKey("SLG-44", new CardsWHT.FlipMoreMon("Triple Smash", 3, 60));
            r.RegisterByKey("SLG-45", new MarshadowSLG());
            // Fighting
            r.RegisterByKey("SLG-46", new ParalyzeFlipMon("Thunder Shock"));
            // Darkness
            r.RegisterByKey("SLG-47", new SpiritombSLG());
            r.RegisterByKey("SLG-48", new CoinsMon("Tail Rap", 2));
            r.RegisterByKey("SLG-49", new CardsPOR.MemoryLockMon("Torment"));
            r.RegisterByKey("SLG-50", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("SLG-51", new CardsFST2.ConditionBonusMon("Dangerous Head", 50, c => c.Opp.Active != null && c.Opp.Active.IsBasic));
            r.RegisterByKey("SLG-53", new ZoroarkGX());
            r.RegisterByKey("SLG-54", new ComboMon(new SwitchSelfMon("Strafe", true), new DiscardEnergyToBenchMon("Oblivion Wing", EnergyType.Darkness)));
            r.RegisterByKey("SLG-55", new HoopaSLG());
            // Dragon and Colorless
            r.RegisterByKey("SLG-56", new ComboMon(new CardsCRZ1.DragonPulseMon(), new CardsTWM1.SelfDiscardMon("Sky Judgment", 3)));
            r.RegisterByKey("SLG-57", new ArceusSLG());
        }

        // ================================================================== Shared

        /// <summary>Damage from an attack (None: damage dealt straight by the engine, which is attack damage too), not from an Ability.</summary>
        private static bool AttackDamage(EffectContext src)
        {
            var kind = src.Engine.CurrentSourceKind;
            return kind == EffectSourceKind.Attack || kind == EffectSourceKind.None;
        }

        /// <summary>
        /// "Your opponent's Active Pokémon is now Asleep. Your opponent flips 2 coins instead of 1 between turns. If either of them
        /// is tails, that Pokémon is still Asleep" (Hibernation Spore; Ultra Prism's Abyssal Sleep works the same way).
        /// </summary>
        internal sealed class DeepSleepMon : Mon
        {
            private readonly string attack;

            public DeepSleepMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Asleep)) defending.SleepCoins = 2;
                return true;
            }
        }

        /// <summary>"Attach a [T] Energy card from your discard pile to 1 of your Benched Pokémon" (Booming Thunder, Oblivion Wing).</summary>
        internal sealed class DiscardEnergyToBenchMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public DiscardEnergyToBenchMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                if (energy.Count > 0 && ctx.Me.Bench.Count > 0)
                    ctx.AttachCardsAnyWay(energy, 1, (card, mon) => ctx.Me.Bench.Contains(mon), attack + ": attach a " + type + " Energy card from your discard pile to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>
        /// Jungle Totem: each basic Grass Energy card attached to your Pokémon provides 2 units of what it provides (Grass Grass).
        /// More than 1 Jungle Totem doesn't add more. A basic Grass Energy whose type was changed still counts, and gives 2 of the
        /// new type (Compendium: Blazing Energy / Energy Burn); it's still 1 basic Energy card (Compendium: Dragon Break, 2018-08-09).
        /// </summary>
        private sealed class VenusaurSLG : Mon
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || holder == null || holder.Owner != src.Player || units.Count != 1) return;
                if (!CardsSSP1.IsBasicOf(energy.Def, EnergyType.Grass)) return;
                units.Add(units[0]);
            }
        }

        /// <summary>Flick Poison: switch 1 of the opponent's Benched Pokémon with their Active Pokémon; the new Active Pokémon is now Poisoned.</summary>
        private sealed class FlickPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flick Poison") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var target = ctx.GustOpponent("Flick Poison: switch which of your opponent's Benched Pokémon into the Active Spot?");
                if (target != null && ctx.InPlay(target) && ctx.IsActive(target)) ctx.ApplyCondition(target, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Wrapped in Wind: 30, then you may attach a basic Energy card from your hand to this Pokémon. Pike: 30 to 1 Benched Pokémon too.</summary>
        private sealed class VirizionSLG : Mon
        {
            private static readonly Mon Pike = new Cards30CC.BenchHitMon("Pike", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wrapped in Wind") return Pike.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || !ctx.Engine.CanAttachFromHand(me)) return true;
                var basic = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basic.Count == 0) return true;
                var picks = ctx.ChooseCards(basic, 0, 1, "Wrapped in Wind: you may attach a basic Energy card from your hand to " + me.Name);
                if (picks.Count == 0 || !ctx.Me.Hand.Remove(picks[0])) return true;
                ctx.Engine.AttachEnergyCard(picks[0], me, true);
                return true;
            }
        }

        /// <summary>
        /// Energy Reload: once during your turn, move a Grass Energy from 1 of your other Pokémon to this Pokémon. Gaia Blaster: 20
        /// more for each Grass Energy attached to this Pokémon.
        /// </summary>
        private sealed class GenesectSLG : Mon
        {
            private static readonly Mon Blaster = new CardsMEG.EnergyBonusMon("Gaia Blaster", EnergyType.Grass, 20);

            protected override string Activated { get { return "Energy Reload"; } }

            private static Dictionary<CardInstance, PokemonInPlay> GrassElsewhere(EffectContext ctx)
            {
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != ctx.Pokemon))
                    foreach (var en in mon.Energy)
                        if (CardsDRI1.ProvidesType(ctx, mon, en, EnergyType.Grass)) holders[en] = mon;
                return holders;
            }

            protected override bool AbilityReady(EffectContext ctx) { return GrassElsewhere(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var holders = GrassElsewhere(ctx);
                if (holders.Count == 0) return;
                var pick = ctx.ChooseCards(holders.Keys.ToList(), 1, 1, "Energy Reload: move which Grass Energy to " + ctx.Pokemon.Name + "?")[0];
                ctx.Engine.MoveEnergy(holders[pick], ctx.Pokemon, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blaster.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Heat Cyclone: once during your turn, you may have the opponent switch their Active Pokémon with 1 of their Benched Pokémon (they choose).</summary>
        private sealed class VolcaronaSLG : Mon
        {
            protected override string Activated { get { return "Heat Cyclone"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
        }

        // ================================================================== Water

        /// <summary>
        /// Plunge: once during your turn, if this Pokémon is on your Bench, move all Energy from your Active Pokémon to it, then switch
        /// it with your Active Pokémon. With no Energy on the Active Pokémon nothing moves, but it still switches in (your ruling, v87).
        /// </summary>
        private sealed class CroconawSLG : Mon
        {
            protected override string Activated { get { return "Plunge"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var active = ctx.Me.Active;
                if (active == null || !ctx.Me.Bench.Contains(me)) return;
                foreach (var en in active.Energy.ToList()) ctx.Engine.MoveEnergy(active, me, en);
                ctx.Engine.SwitchActive(ctx.Player, me);
            }
        }

        /// <summary>Blessings of the Deep: once during your turn, heal 20 damage from 1 of your Pokémon that has any Water Energy attached.</summary>
        private sealed class ManaphySLG : Mon
        {
            protected override string Activated { get { return "Blessings of the Deep"; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && m.Energy.Any(en => CardsDRI1.ProvidesType(ctx, m, en, EnergyType.Water)));
            }

            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Targets(ctx), "Blessings of the Deep: heal 20 damage from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }
        }

        /// <summary>Bail Out: put a Water Pokémon from your discard pile into your hand (if there is one, you must take 1: Compendium, Virizion's Bail Out).</summary>
        private sealed class WaterBailOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bail Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var water = ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Water).ToList();
                if (water.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(water, 1, 1, "Bail Out: put a Water Pokémon from your discard pile into your hand"));
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Intimidating Pattern: while this is your Active Pokémon, the opponent's Active Pokémon's attacks do 30 less damage (before Weakness and Resistance). Venomous Fang: Poisoned.</summary>
        private sealed class ArbokSLG : Mon
        {
            private static readonly Mon Fang = new ConditionAttackMon("Venomous Fang", SpecialCondition.Poisoned);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (!AttackDamage(src)) return damage;
                return src.State.IsActive(info.Attacker) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fang.HandleAttack(ctx, atk); }
        }

        /// <summary>Legendary Guidance: search your deck for up to 2 Energy cards and attach them to your Pokémon in any way you like.</summary>
        private sealed class LegendaryGuidanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Legendary Guidance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsBRS1.SearchAttachAnyWay(ctx, d => d.IsEnergy, 2, m => true, "Legendary Guidance", "Energy cards");
                return true;
            }
        }

        /// <summary>
        /// Stellar Reign: 10, then if the opponent's Active Pokémon is an evolved Pokémon, put all Evolution cards on it into the
        /// opponent's hand. A Stage 1 put into play by Pokémon Research Lab stays (Compendium, 2024-02-15); Focus Sash doesn't
        /// save the devolved Pokémon (Compendium, 2018-05-31).
        /// </summary>
        private sealed class StellarReignMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stellar Reign") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Stack.Count < 2 || ctx.Engine.IsKnockedOut(defending)) return true;
                ctx.Engine.Devolve(defending, 99);
                return true;
            }
        }

        /// <summary>
        /// Let Loose: when you play this Pokémon from your hand onto your Bench during your turn, you may have each player shuffle
        /// their hand into their deck and draw 4 cards. Shadow Punch: not affected by Resistance.
        /// </summary>
        private sealed class MarshadowSLG : Mon
        {
            private static readonly Mon Punch = new CardsFLF1.NoResistanceMon("Shadow Punch");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Let Loose", true, "Use Let Loose? Each player shuffles their hand into their deck and draws 4 cards.")) return;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Player, 4);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Cursed Whirlpool: while this is your Active Pokémon, the opponent's Active Pokémon can't retreat. Cursed Drop: 3 damage counters in any way you like.</summary>
        private sealed class SpiritombSLG : Mon
        {
            private static readonly Mon Drop = new CardsLOR1.CountersAnyWayMon("Cursed Drop", 3);

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                var self = src.Pokemon;
                return self != null && mon != null && mon.Owner != src.Player && src.State.IsActive(self) && src.State.IsActive(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drop.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Trade: once during your turn, discard a card from your hand; if you do, draw 2 (the old wording: usable with an empty
        /// deck, Compendium 2019-03-21). Riotous Beating: 20 for each of your Pokémon in play. Trickster-GX: use 1 of the opponent's
        /// Pokémon's attacks as this attack; it can copy a GX attack, and that isn't a second GX attack (Compendium, 2017-12-07). The
        /// Energy pays for Trickster-GX, so a copied attack's "extra Energy" counts beyond [D][D] (Compendium, 2020-08-13).
        /// </summary>
        private sealed class ZoroarkGX : Mon
        {
            private static readonly Mon Beating = new CardsASR1.TimesMon("Riotous Beating", c => c.MyPokemon().Count);

            protected override string Activated { get { return "Trade"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Trade: discard a card from your hand (then draw 2 cards)")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trickster-GX") return Beating.HandleAttack(ctx, atk);
                var withAttacks = ctx.OpponentPokemon(m => m.Def.Attacks.Any(a => a.Name != "Trickster-GX"));
                var source = ctx.ChoosePokemon(withAttacks, "Trickster-GX: use an attack of which of your opponent's Pokémon?");
                if (source == null) return Nothing(ctx);
                // Trickster-GX is this player's GX attack; a copied GX attack is part of it, not a second one.
                ctx.Me.GxUsed = false;
                CardDef def;
                int index;
                bool chosen = ChooseCopiedAttack(ctx, source, "Trickster-GX: choose 1 of " + source.Name + "'s attacks to use", out def, out index);
                if (!chosen || def.Attacks[index].Name == "Trickster-GX")
                {
                    ctx.Me.GxUsed = true;
                    return Nothing(ctx);
                }
                ctx.Engine.CopiedCostBasis = atk;
                try { UseCopiedAttack(ctx, def, index); }
                finally { ctx.Engine.CopiedCostBasis = null; }
                ctx.Me.GxUsed = true;
                return true;
            }
        }

        /// <summary>Scoundrel Guard: prevent all effects of attacks, including damage, done to this Pokémon by the opponent's Pokémon-GX or Pokémon-EX.</summary>
        private sealed class HoopaSLG : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsGXorEX;
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && info.Attacker.Def.IsGXorEX
                    && AttackDamage(src);
            }
        }

        // ================================================================== Colorless

        /// <summary>Fabled Defense: while this is your Active Pokémon, prevent all damage done to your Benched Pokémon by the opponent's attacks. Ultimate Arrow: 30 to each of the opponent's Pokémon.</summary>
        private sealed class ArceusSLG : Mon
        {
            private static readonly Mon Arrow = new CardsCEC3.SpreadAmountMon("Ultimate Arrow", 30);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && info.Target != null && info.Target.Owner == src.Player && !info.TargetIsActive && info.AgainstOpponent
                    && AttackDamage(src);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Arrow.HandleAttack(ctx, atk); }
        }
    }
}
