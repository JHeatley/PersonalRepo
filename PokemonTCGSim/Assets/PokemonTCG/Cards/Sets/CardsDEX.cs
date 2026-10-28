using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dark Explorers (DEX 1-111, 2012; Expanded). Cards reprinted in Legendary Treasures (Torchic, Plusle, Minun, Darkrai-EX, Zorua,
    /// Zoroark, Eevee), Plasma Freeze (Empoleon), Dragons Exalted (Krookodile), Fates Collide (Old Amber Aerodactyl, Random Receiver),
    /// Plasma Blast (Professor Juniper), White Flare (Cheren), Lost Origin (Dark Patch), Twilight Masquerade (Enhanced Hammer),
    /// 30th Celebration (N), Mega Evolution (Rare Candy, Ultra Ball) and Perfect Order (Pokémon Catcher) are prints of those, and
    /// 103-108 are the full arts. Your rulings (v128): Ancient Scream stacks; Ancient Power stops Rare Candy too; Slurp Shakedown
    /// and Lure Poison do nothing with no Benched Pokémon to switch in.
    /// Pokémon whose attacks only do their printed damage need no script: Bulbasaur, Dwebble, Larvesta (20), Joltik, Drilbur,
    /// Krokorok, Timburr, Pawniard and Eevee (84).
    /// </summary>
    public static class CardsDEX
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DEX-2", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned)));
            r.RegisterByKey("DEX-3", new VenusaurFloralScent());
            r.RegisterByKey("DEX-4", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("DEX-5", new ComboMon(new LurePoisonMon(), new SpitSquallMon()));
            r.RegisterByKey("DEX-6", new ComboMon(new FlipBonusMon("Quick Attack", 30), new EnergyAssistOneMon()));
            r.RegisterByKey("DEX-8", new HeavyBulletMon());
            r.RegisterByKey("DEX-9", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("DEX-10", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("DEX-11", new DeckAndCoverMon());
            r.RegisterByKey("DEX-12", new ComboMon(new CardsMEG.SandAttackMon("Sand-Attack"), new CardsFLF1.OptionalTypeDiscardBonusMon("Fire Slash", EnergyType.Fire, 30)));
            r.RegisterByKey("DEX-13", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new CardsBKT1.GrandFlameMon()));
            r.RegisterByKey("DEX-14", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("DEX-16", new ComboMon(new CoinsMon("Double Kick", 2), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("DEX-17", new ComboMon(new BlazeKickMon(), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("DEX-18", new FlipChargeMon("Flame Cloak", d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), "Fire Energy card"));
            r.RegisterByKey("DEX-19", new ComboMon(new CardsFST2.ConditionBonusMon("Hot Lick", 50, c => c.Defending != null && c.Defending.Name == "Durant"), new FlipBonusMon("Firebreathing", 20)));
            r.RegisterByKey("DEX-21", new ConditionAttackMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("DEX-22", new VolcaronaScorchingScales());
            r.RegisterByKey("DEX-23", new ComboMon(new BigYawnMon(), new CardsBWP.BothHeadsOrNothingMon("Shot in the Dark")));
            r.RegisterByKey("DEX-24", new SlowbroAirhead());
            r.RegisterByKey("DEX-25", new ComboMon(new Cards30CC.BenchHitMon("Muddy Water", 20), new CardsTWM1.HealSelfMon("Spiral Drain", 20)));
            r.RegisterByKey("DEX-26", new ComboMon(new SwitchSelfMon("Smash Turn", true), new CardsCES1.TwoTargetsMon("Dual Splash", 50)));
            r.RegisterByKey("DEX-27", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("DEX-28", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("DEX-30", new ComboMon(new FlipBonusMon("Quick Attack", 30), new ReflectEnergyMon()));
            r.RegisterByKey("DEX-31", new CardsTWM1.HealSelfMon("Spiral Drain", 20));
            r.RegisterByKey("DEX-32", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("DEX-33", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("DEX-34", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("DEX-35", new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep));
            r.RegisterByKey("DEX-36", new ComboMon(new CardsPLF1.HealAllMineMon("Healing Pirouette", 30), new CardsMEG.FlipUntilTailsBonusMon("Incessant Peck", 20)));
            r.RegisterByKey("DEX-37", new ComboMon(new CardsFLF1.OptionalTypeDiscardBonusMon("Electrigun", EnergyType.Lightning, 40), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("DEX-38", new ComboMon(new ParalyzeFlipMon("Thunder Fang"), new VoltBoltMon("Volt Bolt", 100)));
            r.RegisterByKey("DEX-43", new ComboMon(new CoinsMon("Quick Turn", 2), new CardsFST2.ConditionBonusMon("Electrisilk", 40, c => c.Defending != null && c.Engine.RetreatCostOf(c.Defending) == 0)));
            r.RegisterByKey("DEX-44", new FlipChargeMon("Charge Beam", d => d.IsEnergy, "Energy card"));
            r.RegisterByKey("DEX-45", new Cards30CC.BenchHitMon("Spark", 10));
            r.RegisterByKey("DEX-46", new CardsTEU2.DiscardAllTypeMon("Shock Bolt", EnergyType.Lightning));
            r.RegisterByKey("DEX-47", new ComboMon(new CardsDAA1.DrainMon("Suction Heal"), new SlurpShakedownMon()));
            r.RegisterByKey("DEX-48", new EspeonSolarRevelation());
            r.RegisterByKey("DEX-49", new ComboMon(new ParalyzeFlipMon("Psy Bolt"), new CardsFST2.ConditionBonusMon("Hand Press", 30, c => c.Me.Hand.Count > c.Opp.Hand.Count)));
            r.RegisterByKey("DEX-50", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("DEX-51", new CardsPLF2.AstonishMon());
            r.RegisterByKey("DEX-52", new ComboMon(new ChuckMon(), new NoRetreatMon("Lock Up")));
            r.RegisterByKey("DEX-53", new AerodactylAncientScream());
            r.RegisterByKey("DEX-54", new ComboMon(new CardsMEW1.SpreadBenchMon("Tromp", 10), new CardsFST2.ConditionBonusMon("Giant Claw", 40, c => c.Defending != null && c.Defending.DamageCounters >= 2)));
            r.RegisterByKey("DEX-56", new ComboMon(new Cards30CC.BenchHitMon("Tunnel Strike", 30), new DigUppercutMon()));
            r.RegisterByKey("DEX-57", new ComboMon(new CardsASC.ToolBonusMon("Reinforced Drill", 30), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("DEX-59", new CoinsMon("Steel Swing", 2));
            r.RegisterByKey("DEX-60", new ComboMon(new CardsDAA1.DrainMon("Shadow Drain"), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("DEX-61", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new ShadowShutdownMon()));
            r.RegisterByKey("DEX-62", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new JunkHuntTwoMon()));
            r.RegisterByKey("DEX-64", new SandDiveMon());
            r.RegisterByKey("DEX-67", new FlipBonusMon("Rising Lunge", 20));
            r.RegisterByKey("DEX-68", new CardsASC.FlipDiscardDefendingEnergyMon("Crushing Blow"));
            r.RegisterByKey("DEX-70", new ComboMon(new ParalyzeFlipMon("Paralyzing Gaze"), new NoRetreatMon("Shadow Bind")));
            r.RegisterByKey("DEX-72", new FuryCutterMon(60, 30, 10));
            r.RegisterByKey("DEX-73", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("DEX-74", new ComboMon(new CardsBLK.PeckOffMon("Joust"), new CavalryLanceMon()));
            r.RegisterByKey("DEX-75", new SwitchSelfMon("Smash Turn", false));
            r.RegisterByKey("DEX-76", new FlipChargeMon("Charge Beam", d => d.IsEnergy, "Energy card"));
            r.RegisterByKey("DEX-77", new ComboMon(new CardsMEG.EnergyBonusMon("Metal Blast", EnergyType.Metal, 20), new CardsBCR1.DrawUntilAttackMon("Lock Gear", 6)));
            r.RegisterByKey("DEX-79", new FlipBonusMon("Aerial Ace", 20));
            r.RegisterByKey("DEX-80", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new RecoilMon("Double-Edge", 30)));
            r.RegisterByKey("DEX-81", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("DEX-82", new BlisseySoftboiled());
            r.RegisterByKey("DEX-85", new CardsPAR2.FlipConditionMon("Tone-Deaf", SpecialCondition.Confused));
            r.RegisterByKey("DEX-86", new RecoilMon("Take Down", 10));
            r.RegisterByKey("DEX-87", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("DEX-88", new ComboMon(new CardsFST2.ConditionBonusMon("Special Fang", 40, c => c.Pokemon.Energy.Any(e => e.Def.IsSpecialEnergy)), new CardsMEG.WeakenDefendingMon("Ferocious Bellow", 30)));
            r.RegisterByKey("DEX-89", new StunningUppercutMon());
            r.RegisterByKey("DEX-90", new ComboMon(new CardsROS2.AnyStadiumBonusMon("Blow Through", 30), new CardsFLF1.FlipTailsSelfDiscardMon("Power Blast")));
            r.RegisterByKey("DEX-92", new DarkClaw());
            r.RegisterByKey("DEX-95", new HooligansJimAndCas());
            r.RegisterByKey("DEX-101", new TwistMountain());
            r.RegisterByKey("DEX-109", new GardevoirPsychicMirage());
            r.RegisterByKey("DEX-110", new ArcheopsAncientPower());
        }

        // ================================================================== Abilities

        /// <summary>Floral Scent: once during your turn, search your deck for a Pokémon and put it into your hand. Poison Powder: Poisoned.</summary>
        private sealed class VenusaurFloralScent : Mon
        {
            private const string Scent = "Floral Scent";
            private static readonly Mon Powder = new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned);

            protected override string Activated { get { return Scent; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon, 1, Scent + ": choose a Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Powder.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Scorching Scales: the opponent's Burned Pokémon get 4 damage counters instead of 2 between turns (more Volcarona add
        /// nothing). Burning Wind: you may discard an Energy from this Pokémon; if you do, the Defending Pokémon is now Burned.
        /// </summary>
        private sealed class VolcaronaScorchingScales : Mon
        {
            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return mon != null && mon.Owner != src.Player ? Math.Max(counters, 4) : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Wind") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.AskYesNo("Burning Wind: discard an Energy from " + me.Name + " to Burn the Defending Pokémon?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Burning Wind: discard which Energy?") > 0 && TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Airhead: with 2, 4 or 6 Prize cards left, this Pokémon can't attack. Lazy Headbutt: this Pokémon is now Asleep.</summary>
        private sealed class SlowbroAirhead : Mon
        {
            private static readonly Mon Headbutt = new CardsRCL2.SelfConditionMon("Lazy Headbutt", SpecialCondition.Asleep);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Pokemon == null || ctx.Engine.AbilityBlocked(ctx.Pokemon, "Airhead")) return true;
                int left = ctx.Me.Prizes.Count;
                return left != 2 && left != 4 && left != 6;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Headbutt.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Solar Revelation: prevent all effects of the opponent's attacks, except damage, done to each of your Pokémon that has
        /// any Energy attached. Psy Report: the opponent reveals their hand.
        /// </summary>
        private sealed class EspeonSolarRevelation : Mon
        {
            private static readonly Mon Report = new Cards30C.PeerAtMon("Psy Report");

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target == null || target.Owner != src.Player || kind != EffectSourceKind.Attack || target.Energy.Count == 0) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Report.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Ancient Scream: your Pokémon's attacks do 10 more damage to the opponent's Active Pokémon (before Weakness and Resistance).
        /// Your ruling (v128): each of these Aerodactyl adds 10.
        /// </summary>
        private sealed class AerodactylAncientScream : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker == null || info.Attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 10;
            }
        }

        /// <summary>Softboiled: once during your turn, flip a coin; if heads, heal 30 damage from your Active Pokémon. Double-Edge: 60 to itself.</summary>
        private sealed class BlisseySoftboiled : Mon
        {
            private const string Softboiled = "Softboiled";
            private static readonly Mon Edge = new RecoilMon("Double-Edge", 60);

            protected override string Activated { get { return Softboiled; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (ctx.Flip() && active != null) ctx.Heal(active, 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Edge.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Psychic Mirage: each basic Psychic Energy attached to your Psychic Pokémon provides 2 Psychic Energy. More than 1
        /// Psychic Mirage adds nothing (the Energy already gives 2). Mind Shock: not affected by Weakness or Resistance.
        /// </summary>
        private sealed class GardevoirPsychicMirage : Mon
        {
            private static readonly Mon Shock = new CardsFST1.NoWeaknessResistanceMon("Mind Shock");

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || holder == null || holder.Owner != src.Player || units.Count != 1) return;
                if (!CardsSSP1.IsBasicOf(energy.Def, EnergyType.Psychic) || !src.Engine.IsType(holder, EnergyType.Psychic)) return;
                units.Add(units[0]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shock.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Ancient Power: no player can play a Pokémon from their hand to evolve their Pokémon (your ruling, v128: Rare Candy
        /// included; evolving from the deck still works). Rock Slide: 10 to 2 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class ArcheopsAncientPower : Mon
        {
            private static readonly Mon Slide = new CardsFST1.TwoBenchedMon("Rock Slide", 10);

            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target) { return src.Pokemon != null && target != null; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slide.HandleAttack(ctx, atk); }
        }

        // ================================================================== Attacks

        /// <summary>
        /// Lure Poison: switch 1 of the opponent's Benched Pokémon with the Defending Pokémon; the new Defending Pokémon is now
        /// Poisoned. Your ruling (v128): with nothing switched in, nothing happens.
        /// </summary>
        private sealed class LurePoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lure Poison") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count == 0) return true;
                var pulled = ctx.GustOpponent("Lure Poison: switch which of your opponent's Benched Pokémon with the Defending Pokémon?");
                if (pulled != null && TefPokemon.StillDefending(ctx, pulled)) ctx.ApplyCondition(pulled, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Spit Squall: the opponent puts the Defending Pokémon and all cards attached to it into their hand.</summary>
        private sealed class SpitSquallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spit Squall") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Spit Squall on " + defending.Name + " was prevented.");
                    return true;
                }
                ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>Energy Assist: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        internal sealed class EnergyAssistOneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, 1, (card, mon) => !ctx.IsActive(mon), "Energy Assist: attach a basic Energy card to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        /// <summary>Heavy Bullet: flip a coin; if heads, 20 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class HeavyBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Bullet") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Flip()) ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Deck and Cover: the Defending Pokémon is now Paralyzed and Poisoned; shuffle this Pokémon and its cards into your deck.</summary>
        private sealed class DeckAndCoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deck and Cover") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Blaze Kick: flip a coin; heads, 30 more damage; tails, the Defending Pokémon is now Burned.</summary>
        private sealed class BlazeKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze Kick") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, attach [a card] from your discard pile to this Pokémon" (Flame Cloak, Charge Beam).</summary>
        private sealed class FlipChargeMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;

            public FlipChargeMon(string attack, Func<CardDef, bool> filter, string what)
            {
                this.attack = attack;
                this.filter = filter;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !ctx.Me.Discard.Any(c => filter(c.Def)) || !ctx.Flip()) return true;
                var pile = ctx.Me.Discard.Where(c => filter(c.Def)).ToList();
                if (pile.Count == 0) return true;
                var card = pile.Select(c => c.Def.Name).Distinct().Count() == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, attack + ": attach which " + what + " to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Big Yawn: this Pokémon and the Defending Pokémon are now Asleep.</summary>
        internal sealed class BigYawnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Yawn") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Asleep);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Reflect Energy: move an Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class ReflectEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflect Energy") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0 || me.Energy.Count == 0) return true;
                var energy = me.Energy.ToList();
                var card = energy.Select(e => e.Def.Name).Distinct().Count() == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Reflect Energy: move which Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Reflect Energy: move " + card.Def.Name + " to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Volt Bolt: discard all Lightning Energy from this Pokémon; 100 damage to 1 of the opponent's Pokémon. Used for Volt Bolt, Lightning Crash (the attack name and the numbers are passed in).</summary>
        internal sealed class VoltBoltMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public VoltBoltMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.InPlay(ctx.Pokemon)) CardsTEU2.DiscardAllOfType(ctx, ctx.Pokemon, EnergyType.Lightning);
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>
        /// Slurp Shakedown: switch 1 of the opponent's Benched Pokémon with the Defending Pokémon; 60 damage to the new Defending
        /// Pokémon. Your ruling (v128): with nothing switched in, the attack does nothing.
        /// </summary>
        private sealed class SlurpShakedownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slurp Shakedown") return false;
                if (ctx.Opp.Bench.Count == 0) return Nothing(ctx);
                var pulled = ctx.GustOpponent("Slurp Shakedown: switch which of your opponent's Benched Pokémon with the Defending Pokémon?");
                if (pulled == null || !TefPokemon.StillDefending(ctx, pulled)) return Nothing(ctx);
                ctx.DamageDefending(60);
                return true;
            }
        }

        /// <summary>Chuck: discard any number of Pokémon Tool cards from your hand; 40 damage for each card discarded.</summary>
        private sealed class ChuckMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chuck") return false;
                var tools = ctx.Me.Hand.Where(c => c.Def.IsTool).ToList();
                var picks = tools.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(tools, 0, tools.Count, "Chuck: discard any number of Pokémon Tool cards from your hand (" + atk.Damage + " damage for each)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (picks.Count > 0) ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        /// <summary>Dig Uppercut: put a card from your discard pile into your hand.</summary>
        internal sealed class DigUppercutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Uppercut") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "Dig Uppercut: put a card from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>Shadow Shutdown: flip 2 coins; if both are heads, discard all Energy attached to the Defending Pokémon.</summary>
        private sealed class ShadowShutdownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Shutdown") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) < 2 || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Shadow Shutdown on " + defending.Name + " was prevented.");
                    return true;
                }
                ctx.Engine.DiscardAllEnergy(defending);
                return true;
            }
        }

        /// <summary>Junk Hunt: put 2 Item cards from your discard pile into your hand.</summary>
        private sealed class JunkHuntTwoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Junk Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                int n = Math.Min(2, items.Count);
                if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(items, n, n, "Junk Hunt: put which " + n + " Item card" + (n == 1 ? "" : "s") + " into your hand?"));
                return true;
            }
        }

        /// <summary>Sand Dive: flip a coin; if heads, prevent all damage done to this Pokémon by attacks during the opponent's next turn.</summary>
        private sealed class SandDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Dive") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage from attacks during the opponent's next turn (Sand Dive)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Fury Cutter: flip 3 coins; 1 heads, 10 more damage; 2 heads, 30 more; 3 heads, 60 more. The numbers are passed in.</summary>
        internal sealed class FuryCutterMon : Mon
        {
            private readonly int amount;
            private readonly int amount2;
            private readonly int amount3;

            public FuryCutterMon(int amount, int amount2, int amount3)
            {
                this.amount = amount;
                this.amount2 = amount2;
                this.amount3 = amount3;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(3);
                int bonus = heads == 3 ? amount : heads == 2 ? amount2 : heads == 1 ? amount3 : 0;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Cavalry Lance: during the opponent's next turn, this Pokémon has no Weakness.</summary>
        private sealed class CavalryLanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cavalry Lance") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new CardsPLF2.NoWeaknessEffect(me.Name + " has no Weakness during your opponent's next turn (Cavalry Lance)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Stunning Uppercut: flip 2 coins; both heads, the Defending Pokémon is now Paralyzed; both tails, the attack does nothing.</summary>
        private sealed class StunningUppercutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stunning Uppercut") return false;
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (heads == 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Dark Claw: the Darkness Pokémon it's attached to does 20 more damage to the opponent's Active Pokémon with its attacks (before Weakness and Resistance).</summary>
        private sealed class DarkClaw : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !src.Engine.IsType(holder, EnergyType.Darkness)) return damage;
                return damage + 20;
            }
        }

        /// <summary>Hooligans Jim &amp; Cas: flip a coin; if heads, 3 random cards from the opponent's hand are revealed and shuffled into their deck.</summary>
        private sealed class HooligansJimAndCas : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                if (ctx.Opp.Hand.Count == 0)
                {
                    ctx.Log("Hooligans Jim & Cas: your opponent has no cards in their hand.");
                    return;
                }
                CardsTWM1.ShuffleRandomFromOpponentHand(ctx, 3);
            }
        }

        /// <summary>Twist Mountain: once during each player's turn, that player may flip a coin; if heads, they put a Restored Pokémon from their hand onto their Bench.</summary>
        private sealed class TwistMountain : StadiumAction
        {
            private static List<CardInstance> Restored(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Stage == Stage.Restored && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            protected override bool Ready(EffectContext ctx)
            {
                return ctx.BenchSpace > 0 && Restored(ctx).Count > 0;
            }

            protected override bool Use(EffectContext ctx)
            {
                if (!ctx.Flip()) return false;
                var options = Restored(ctx);
                if (options.Count == 0 || ctx.BenchSpace <= 0) return false;
                var card = ctx.ChooseCards(options, 1, 1, "Twist Mountain: put which Restored Pokémon onto your Bench?")[0];
                if (ctx.Me.Hand.Remove(card)) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                return false;
            }
        }
    }
}
