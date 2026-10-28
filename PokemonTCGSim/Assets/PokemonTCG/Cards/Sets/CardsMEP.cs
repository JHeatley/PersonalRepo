using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Mega Evolution Black Star Promos (MEP): the cards that weren't already in the pool. Promos whose attacks only do
    /// their printed damage need no script: Sneasel, Turtwig, Piplup, Rowlet, Chikorita, Cyndaquil, Totodile, Snivy,
    /// Sobble, Treecko, Torchic, Mudkip, Fennekin (MEP-59), Froakie, Sprigatito, Quaxly.
    /// </summary>
    public static class CardsMEP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MEP-5", new DrifloonMEP());
            r.RegisterByKey("MEP-6", new DrifblimMEP());
            r.RegisterByKey("MEP-8", new GolduckMEP());
            r.RegisterByKey("MEP-18", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("MEP-19", new WhimsicottMEP());
            r.RegisterByKey("MEP-21", new WeavileMEP());
            r.RegisterByKey("MEP-30", new ExplosionYMon("Explosion Y", 3, 280));
            r.RegisterByKey("MEP-34", new CardsMEG.EnergyBonusMon("Giant Bouquet", EnergyType.Grass, 50));
            r.RegisterByKey("MEP-35", new RecoilMon("Crimson Blast", 60));
            r.RegisterByKey("MEP-36", new CardsPFL.DamagedBonusMon("Mortal Crunch", 200));
            r.RegisterByKey("MEP-37", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("MEP-38", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("MEP-39", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("MEP-41", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("MEP-44", new ConditionAttackMon("Fire Fang", SpecialCondition.Burned));
            r.RegisterByKey("MEP-45", new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("MEP-50", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("MEP-51", new FlipBonusMon("Razor Shell", 30));
            r.RegisterByKey("MEP-53", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("MEP-58", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("MEP-62", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("MEP-64", new SerperiorMEP());
            r.RegisterByKey("MEP-65", new BarbaracleMEP());
            r.RegisterByKey("MEP-66", new TyrantrumMEP());
            r.RegisterByKey("MEP-67", new DoubladeMEP());
            r.RegisterByKey("MEP-70", new CardsBLK.FlailMon("Get Angry"));
            r.RegisterByKey("MEP-71", new MegaZygardeExMEP());
            r.RegisterByKey("MEP-72", new MegaClefableExMEP());
            r.RegisterByKey("MEP-74", new DelphoxMEP());
            r.RegisterByKey("MEP-75", new AmpharosMEP());
            r.RegisterByKey("MEP-76", new CrobatMEP());
            r.RegisterByKey("MEP-77", new GoodraMEP());
            r.RegisterByKey("MEP-80", new CallForFamilyMon());
            r.RegisterByKey("MEP-81", new MegaGreninjaExMEP());
            r.RegisterByKey("MEP-82", new MiraidonMEP());
            r.RegisterByKey("MEP-83", new CardsLOR1.VeryVulnerableMon("All Out", 160));
            r.RegisterByKey("MEP-85", new BastiodonMEP());
            r.RegisterByKey("MEP-86", new SlowpokeMEP());
            r.RegisterByKey("MEP-87", new DrawAttackMon("Double Draw", 2));
            r.RegisterByKey("MEP-88", new ZarudeMEP());
            r.RegisterByKey("MEP-89", new MegaZeraoraExMEP());
            r.RegisterByKey("MEP-90", new MegaDarkraiExMEP());
            r.RegisterByKey("MEP-91", new MegaDragoniteExMEP());
            r.RegisterByKey("MEP-94", new AlolanExeggutorMEP());
            r.RegisterByKey("MEP-96", new MoltresMEP());
            r.RegisterByKey("MEP-99", new GreninjaExMEP());
            r.RegisterByKey("MEP-100", new SylveonExMEP());
            r.RegisterByKey("MEP-101", new NidorinaMEP());

            r.RegisterByName("Celebratory Fanfare", new CelebratoryFanfare());
        }

        // ================================================================== Psychic / Water / Darkness (older Pokémon)

        /// <summary>Pull: flip; heads, switch in 1 of the opponent's Benched Pokémon.</summary>
        private sealed class DrifloonMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull") return false;
                if (ctx.Flip()) ctx.GustOpponent("Pull: switch in which of your opponent's Benched Pokémon?");
                return true;
            }
        }

        /// <summary>Creepy Wind: Confused. Balloon Return: put this Pokémon and all attached cards into your hand.</summary>
        private sealed class DrifblimMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Creepy Wind")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name == "Balloon Return")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Damp: Abilities that need the Pokémon using them to Knock Out itself don't exist. Hydro Pump: +20 per Water Energy.</summary>
        private sealed class GolduckMEP : Mon
        {
            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Engine.Scripts.For(mon.Def).AbilityKnocksOutSelf(abilityName);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Pump") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water));
                return true;
            }
        }

        /// <summary>Healing Fluff: heal all damage from 1 of your Benched Pokémon. U-turn: switch with a Benched Pokémon.</summary>
        private sealed class WhimsicottMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Healing Fluff")
                {
                    var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                    if (hurt.Count == 0) return true;
                    var target = ctx.ChoosePokemon(hurt, "Healing Fluff: heal all damage from which Benched Pokémon?");
                    if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                    return true;
                }
                if (atk.Name == "U-turn")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Retaliatory Claw: +170 with 50 HP or less left.</summary>
        private sealed class WeavileMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Retaliatory Claw") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Engine.RemainingHp(ctx.Pokemon) <= 50 ? 170 : 0));
                return true;
            }
        }

        // ================================================================== Mega Evolution starters and friends

        /// <summary>Explosion Y: discard 3 Energy from it, then 280 to 1 of the opponent's Pokémon. Used for Explosion Y, Sniping Feathers, Feather Shot (the attack name and the numbers are passed in).</summary>
        internal sealed class ExplosionYMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly int amount2;

            public ExplosionYMon(string attack, int amount, int amount2)
            {
                this.attack = attack;
                this.amount = amount;
                this.amount2 = amount2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, amount, atk.Name);
                ctx.DamageAnyOpponent(amount2);
                return true;
            }
        }

        /// <summary>Regal Command: 20 per Pokémon you have in play. Solar Coiling: +150 with Rosa's Encouragement in your discard pile.</summary>
        private sealed class SerperiorMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Regal Command")
                {
                    ctx.DamageDefending(atk.Damage * ctx.MyPokemon().Count);
                    return true;
                }
                if (atk.Name == "Solar Coiling")
                {
                    bool rosa = ctx.Me.Discard.Any(c => c.Def.Name == "Rosa's Encouragement");
                    ctx.DamageDefending(atk.Damage + (rosa ? 150 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Stone Arms: once during your turn, attach a Basic Fighting Energy from your hand to 1 of your Fighting Pokémon.</summary>
        private sealed class BarbaracleMEP : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fighting) && ctx.Engine.CanAttachFromHand(m));
            }

            protected override string Activated { get { return "Stone Arms"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting)) && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx) { CardsPFL.AttachTypeFromHand(ctx, EnergyType.Fighting, Targets(ctx), "Stone Arms"); }
        }

        /// <summary>Tyrannically Gutsy: +150 HP with any Special Energy attached. Wreak Havoc: flip until tails, discard the top card of the opponent's deck for each heads.</summary>
        private sealed class TyrantrumMEP : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon == src.Pokemon && mon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) ? hp + 150 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wreak Havoc") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        /// <summary>Weaponized Swords: reveal any number of Honedge, Doublade and Aegislash from your hand, 60 each.</summary>
        private sealed class DoubladeMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Weaponized Swords") return false;
                var swords = ctx.Me.Hand.Where(c => c.Def.Name == "Honedge" || c.Def.Name == "Doublade" || c.Def.Name == "Aegislash").ToList();
                var picks = swords.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(swords, 0, swords.Count, "Weaponized Swords: reveal Honedge, Doublade and Aegislash from your hand (" + atk.Damage + " damage each)");
                if (picks.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " revealed " + string.Join(", ", picks.Select(c => c.Def.Name)) + ".");
                ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        /// <summary>Gaia Wave: takes 30 less next turn. Nullifying Zero: flip for each of the opponent's Pokémon; heads, 150 to it.</summary>
        private sealed class MegaZygardeExMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gaia Wave")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(30);
                    return true;
                }
                if (atk.Name == "Nullifying Zero")
                {
                    var targets = ctx.OpponentPokemon();
                    var hit = new List<PokemonInPlay>();
                    foreach (var m in targets) if (ctx.Flip()) hit.Add(m);
                    foreach (var m in hit) ctx.DamageTarget(m, 150);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Luminous Wing: prevent all effects of the opponent's Pokémon's Abilities done to it. Shooting Moons: discard up to 4 Energy cards from your hand, +40 each.</summary>
        private sealed class MegaClefableExMEP : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Ability;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shooting Moons") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(energy, 0, Math.Min(4, energy.Count), "Shooting Moons: discard up to 4 Energy cards from your hand (40 damage each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.DamageDefending(atk.Damage + 40 * picks.Count);
                return true;
            }
        }

        /// <summary>Flaring Magic: once during your turn, discard a Basic Fire Energy from your hand to draw until you have 7. Energized Storm: 30 per Energy on all Pokémon.</summary>
        private sealed class DelphoxMEP : Mon
        {
            protected override string Activated { get { return "Flaring Magic"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)) && ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count - 1 < 7;
            }

            protected override void Ability(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                var pick = fire.Select(c => c.Def.Name).Distinct().Count() == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Flaring Magic: discard a Basic Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.DrawUntil(7);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energized Storm") return false;
                int energy = ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)) + ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
                ctx.DamageDefending(atk.Damage * energy);
                return true;
            }
        }

        /// <summary>Synchro Pulse: with as many cards in hand as the opponent, its attacks do 80 more to the opponent's Active Pokémon. Flashing Bolt: can't use it next turn.</summary>
        private sealed class AmpharosMEP : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive) return damage;
                return src.Me.Hand.Count == src.Opp.Hand.Count ? damage + 80 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flashing Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn("Flashing Bolt");
                return true;
            }
        }

        /// <summary>Nighttime Maneuvers: once during your turn, while Active, put any card from your deck on top of it. Poison Sound Wave: Confused and Poisoned.</summary>
        private sealed class CrobatMEP : Mon
        {
            protected override string Activated { get { return "Nighttime Maneuvers"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => true, 1, "Nighttime Maneuvers: choose a card to put on top of your deck", false, 1);
                ctx.ShuffleMyDeck();
                foreach (var c in found) ctx.Me.Deck.Insert(0, c);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Sound Wave") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Slimy Sliding: when the opponent's Active Pokémon retreats they flip; tails, nothing happens. Dragon Pulse: discard your top card.</summary>
        private sealed class GoodraMEP : Mon
        {
            public override bool ForcesRetreatFlip(EffectContext src, int player) { return player != src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                return true;
            }
        }

        /// <summary>Mortal Shuriken: once during your turn, while Active, discard a Basic Water Energy from your hand to put 6 damage counters on 1 of the opponent's Pokémon. Ninja Spinner: may return a Water Energy to your hand for 80 more.</summary>
        private sealed class MegaGreninjaExMEP : Mon
        {
            protected override string Activated { get { return "Mortal Shuriken"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Opp.HasPokemonInPlay && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water));
            }

            protected override void Ability(EffectContext ctx)
            {
                var water = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                var pick = water.Select(c => c.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Mortal Shuriken: discard a Basic Water Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Mortal Shuriken: put 6 damage counters on which Pokémon?");
                if (target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, 6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ninja Spinner") return false;
                var me = ctx.Pokemon;
                var water = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)).ToList();
                bool bonus = water.Count > 0 && ctx.AskYesNo("Ninja Spinner: put a Water Energy from " + me.Name + " into your hand for 80 more damage?");
                if (bonus)
                {
                    var e = water.Count == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Ninja Spinner: put which Water Energy into your hand?")[0];
                    ctx.Engine.ReturnEnergyToHand(me, e);
                }
                ctx.DamageDefending(atk.Damage + (bonus ? 80 : 0));
                return true;
            }
        }

        /// <summary>Photon Cord: when it's Knocked Out in the Active Spot by an opponent's attack, move up to 2 Basic Lightning Energy from it to 1 of your Benched Pokémon. Thunder: 30 to itself.</summary>
        private sealed class MiraidonMEP : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || !wasActive || me == null || src.Me.Bench.Count == 0) return;
                var lightning = me.Energy.Where(e => CardsSSP1.IsBasicOf(e.Def, EnergyType.Lightning)).ToList();
                if (lightning.Count == 0) return;
                var picks = src.ChooseCards(lightning, 0, Math.Min(2, lightning.Count), "Photon Cord: move up to 2 Basic Lightning Energy to a Benched Pokémon");
                if (picks.Count == 0) return;
                var target = src.ChoosePokemon(src.MyBench(), "Photon Cord: move the Energy to which Benched Pokémon?");
                if (target == null) return;
                Announce(src, "Photon Cord");
                foreach (var e in picks) src.Engine.MoveEnergy(me, target, e);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Ancient Bulwark: while it's on your Bench, prevent damage to your Pokémon from attacks of the opponent's Pokémon with 2 or less Energy attached.</summary>
        private sealed class BastiodonMEP : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || !src.Me.Bench.Contains(src.Pokemon)) return damage;
                if (info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                return src.EnergyCount(info.Attacker) <= 2 ? 0 : damage;
            }
        }

        /// <summary>Dopey Face: this Pokémon can't be Confused.</summary>
        private sealed class SlowpokeMEP : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target == src.Pokemon && condition == SpecialCondition.Confused;
            }
        }

        /// <summary>Overhead Throw: also 30 to 1 of your own Benched Pokémon. Shadowy Whip: +70 if your Benched Pokémon have Shadowy Darkness Energy attached.</summary>
        private sealed class ZarudeMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Overhead Throw")
                {
                    ctx.DamageDefending(atk.Damage);
                    var bench = ctx.MyBench();
                    if (bench.Count == 0) return true;
                    var target = ctx.ChoosePokemon(bench, "Overhead Throw: do 30 damage to which of your Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 30);
                    return true;
                }
                if (atk.Name == "Shadowy Whip")
                {
                    bool shadowy = ctx.Me.Bench.Any(m => m.Energy.Any(e => e.Def.Name.StartsWith("Shadowy")));
                    ctx.DamageDefending(atk.Damage + (shadowy ? 70 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Thunderous Fist: 60 per Lightning Energy on it. Zepto Turn: switch with a Benched Pokémon.</summary>
        private sealed class MegaZeraoraExMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Thunderous Fist")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Lightning));
                    return true;
                }
                if (atk.Name == "Zepto Turn")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Dusk Raid: +110 if your Benched Pokémon have damage counters. Abyss Eye: Knocks Out the opponent's Active Pokémon if it has a Special Condition.</summary>
        private sealed class MegaDarkraiExMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Dusk Raid")
                {
                    bool hurt = ctx.Me.Bench.Any(m => m.DamageCounters > 0);
                    ctx.DamageDefending(atk.Damage + (hurt ? 110 : 0));
                    return true;
                }
                if (atk.Name == "Abyss Eye")
                {
                    var d = ctx.Defending;
                    if (d != null && d.Conditions != SpecialCondition.None) ctx.Engine.KnockOut(d, atk.Name);
                    else Nothing(ctx);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sky Transport: once during your turn, switch your Active Pokémon with 1 of your Benched Pokémon. Ryuno Glide: discard 2 Energy.</summary>
        private sealed class MegaDragoniteExMEP : Mon
        {
            protected override string Activated { get { return "Sky Transport"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Sky Transport: switch your Active Pokémon with which Benched Pokémon?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Player, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ryuno Glide") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Scale Up: +250 HP with 6 or more Grass Energy attached. Mega Drain: heal 50 from it.</summary>
        private sealed class AlolanExeggutorMEP : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon == src.Pokemon && src.Engine.EnergyCountOfType(mon, EnergyType.Grass) >= 6 ? hp + 250 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mega Drain") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 50);
                return true;
            }
        }

        /// <summary>Fiery Flapping: once during your turn, with Articuno and Zapdos in play, attach a Basic Fire Energy from your hand to it. Fire Spin: discard 2 Energy.</summary>
        private sealed class MoltresMEP : Mon
        {
            protected override string Activated { get { return "Fiery Flapping"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                bool birds = ctx.MyPokemon(m => m.Name == "Articuno").Count > 0 && ctx.MyPokemon(m => m.Name == "Zapdos").Count > 0;
                return birds && ctx.Engine.CanAttachFromHand(ctx.Pokemon) && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
            }

            protected override void Ability(EffectContext ctx)
            {
                CardsPFL.AttachTypeFromHand(ctx, EnergyType.Fire, new List<PokemonInPlay> { ctx.Pokemon }, "Fiery Flapping");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Spin") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Stealthy Slash: 30 for each damage counter on 1 of the opponent's Pokémon (you choose which).</summary>
        private sealed class GreninjaExMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stealthy Slash") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Stealthy Slash: 30 damage for each damage counter on which Pokémon?");
                if (target != null) ctx.DamageTarget(target, 30 * target.DamageCounters);
                return true;
            }
        }

        /// <summary>Colorful Harmony: 50 for each type of Basic Energy attached to all your Pokémon.</summary>
        private sealed class SylveonExMEP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Harmony") return false;
                int types = ctx.MyPokemon().SelectMany(m => m.Energy).Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count();
                ctx.DamageDefending(atk.Damage * types);
                return true;
            }
        }

        /// <summary>Share Happiness: once during your turn, heal 30 from 1 of your Pokémon.</summary>
        private sealed class NidorinaMEP : Mon
        {
            protected override string Activated { get { return "Share Happiness"; } }
            protected override bool AbilityReady(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }
            protected override void Ability(EffectContext ctx) { CardsMEG.HealOneOfMine(ctx, 30, "Share Happiness"); }
        }

        // ================================================================== Stadium

        /// <summary>Once during each player's turn: heal 10 from each of your Pokémon; if anything was healed, your turn ends.</summary>
        private sealed class CelebratoryFanfare : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }

            protected override bool Use(EffectContext ctx)
            {
                int healed = 0;
                foreach (var m in ctx.MyPokemon())
                    if (m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)) healed += ctx.Heal(m, 10);
                if (healed > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + "'s turn ends (Celebratory Fanfare).");
                return healed > 0;
            }
        }
    }
}
