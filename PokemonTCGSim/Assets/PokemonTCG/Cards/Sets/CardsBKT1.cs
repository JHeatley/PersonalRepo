using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// BREAKthrough (BKT, v97), part 1: the Grass, Fire, Water and Lightning Pokémon (4-57). Paras, Parasect and Pinsir (1-3) are the
    /// same cards as Generations' and keep those scripts.
    /// </summary>
    public static class CardsBKT1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BKT-4", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("BKT-5", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("BKT-6", new ComboMon(new FlingMon("Fire Fling", EnergyType.Fire), HandFling()));
            r.RegisterByKey("BKT-7", new CardsTWM1.HealSelfMon("Nosh", 20));
            r.RegisterByKey("BKT-9", new SearchAttackMon("Tree Climb", d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 1));
            r.RegisterByKey("BKT-10", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("BKT-11", new ComboMon(new CardsPFL.DamagedBonusMon("Spike Lariat", 60), new CardsASC.SelfGuardMon("Adamantine Press", 20)));
            r.RegisterByKey("BKT-12", new ToughHammerMon("Tough Hammer"));
            r.RegisterByKey("BKT-13", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("BKT-14", new DodgeMon("Protect", false));
            r.RegisterByKey("BKT-15", new VivillonBKT());
            r.RegisterByKey("BKT-16", new FlipBonusMon("Rising Lunge", 20));
            r.RegisterByKey("BKT-17", new ComboMon(new Cards30CC.BenchHitMon("Overrun", 30), new CardsTWM1.HealSelfMon("Horn Leech", 30)));
            r.RegisterByKey("BKT-18", new CardsSUM1.DiscardTypeFromSelfMon("Ember", EnergyType.Fire, 1));
            r.RegisterByKey("BKT-19", new MiniEruptionMon());
            r.RegisterByKey("BKT-20", new ComboMon(new MassiveEruptionMon(), new FlareDestroyMon()));
            r.RegisterByKey("BKT-21", new ComboMon(new CardsSSP2.StompOffMon("Melting Horn", 2), new GrandFlameMon()));
            r.RegisterByKey("BKT-22", new InfernoFangMon());
            r.RegisterByKey("BKT-23", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("BKT-24", new ComboMon(new FlingMon("Water Fling", EnergyType.Water), HandFling()));
            r.RegisterByKey("BKT-25", new FlipBonusMon("Firebreathing", 30));
            r.RegisterByKey("BKT-26", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("BKT-27", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("BKT-28", new ComboMon(new HealedBonusMon("Soaking Horn", 80), new RecoilMon("Reckless Charge", 10)));
            r.RegisterByKey("BKT-29", new CardsPFL.FlipUntilTailsTimesMon("Continuous Spin"));
            r.RegisterByKey("BKT-30", new ComboMon(new DeepSeaSwirlMon("Deep Sea Swirl", 7), new BalanceBindMon()));
            r.RegisterByKey("BKT-31", new SwitchSelfMon("Wild River", false));
            r.RegisterByKey("BKT-32", new CardsRCL1.DiscardStadiumAttackMon("Ion Pool"));
            r.RegisterByKey("BKT-33", new OctilleryBKT());
            r.RegisterByKey("BKT-34", new ComboMon(new ParalyzeFlipMon("Ice Breath"), new CardsFST2.ConditionBonusMon("Instant Freeze", 100, c => c.Me.Hand.Count == c.Opp.Hand.Count)));
            r.RegisterByKey("BKT-35", new CardsFST2.ConditionBonusMon("Cryo Mouth", 150, c => c.Pokemon.DamageCounters >= 10));
            r.RegisterByKey("BKT-37", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("BKT-38", new EmpoleonBKT());
            r.RegisterByKey("BKT-39", new ComboMon(new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep), new CoinsMon("Double Smash", 2)));
            r.RegisterByKey("BKT-40", new IceAgeMon());
            r.RegisterByKey("BKT-41", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("BKT-42", new ComboMon(new FlingMon("Green Fling", EnergyType.Grass), HandFling()));
            r.RegisterByKey("BKT-43", new CardsASC.SelfGuardMon("Stiffen", 20));
            r.RegisterByKey("BKT-44", new ComboMon(new FlipBonusMon("Ice Edge", 30), new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("BKT-45", new ComboMon(new FrigidBreathMon(), new DeepFreezeMon()));
            r.RegisterByKey("BKT-48", new DodgeMon("Agility", false));
            r.RegisterByKey("BKT-49", new ComboMon(new ThunderclapShotMon(), new FlipBonusMon("Electrosmash", 20)));
            r.RegisterByKey("BKT-50", new CardsTWM1.SelfDiscardMon("Grand Bolt", 99));
            r.RegisterByKey("BKT-51", new MagnemiteBKT());
            r.RegisterByKey("BKT-52", new ParalyzeFlipMon("Shock Generator"));
            r.RegisterByKey("BKT-53", new ComboMon(new ParalyzeFlipMon("Static Shock"), new CardsTWM1.SelfDiscardMon("Electro Ball", 1)));
            r.RegisterByKey("BKT-54", new MagnezoneBKT());
            r.RegisterByKey("BKT-55", new RaikouBKT());
            r.RegisterByKey("BKT-56", new ComboMon(new CardsFST2.ConditionBonusMon("Revenge", 80, c => c.WasKnockedOutLastOpponentTurn(d => true, true)),
                new CardsTWM1.SelfDiscardMon("Thunder Blast", 1)));
            r.RegisterByKey("BKT-57", new ParalyzeFlipMon("Thunder Shock"));
        }

        // ================================================================== Shared helpers

        internal static Mon HandFling() { return new CardsASR1.TimesMon("Hand Fling", c => c.Me.Hand.Count); }

        /// <summary>"Put 3 [T] Energy cards from your discard pile into your hand" (Fire Fling, Water Fling, Green Fling): basic Energy of that type.</summary>
        internal sealed class FlingMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public FlingMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, type)).ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(3, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, attack + ": put 3 " + type + " Energy cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>"If this Pokémon was healed during this turn, this attack does N more damage" (Soaking Horn).</summary>
        internal sealed class HealedBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public HealedBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.HealedOnTurn == ctx.Turn ? bonus : 0));
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Tough Hammer: 30 damage to this Pokémon and 30 to 1 of the opponent's Benched Pokémon. Used for Tough Hammer, Vigorous Dash (the attack name is passed in).</summary>
        internal sealed class ToughHammerMon : Mon
        {
            private readonly string attack;

            public ToughHammerMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(30);
                ctx.DamageOneBenched(30);
                return true;
            }
        }

        /// <summary>Energy Color: once during your turn, flip a coin; heads, attach a basic Energy card from your deck to 1 of your Pokémon. Sleep Powder: Asleep.</summary>
        private sealed class VivillonBKT : Mon
        {
            private static readonly Mon Powder = new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep);

            protected override string Activated { get { return "Energy Color"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Energy Color: choose a basic Energy card to attach to 1 of your Pokémon");
                foreach (var card in found)
                {
                    var mine = ctx.MyPokemon();
                    var target = ctx.ChoosePokemon(mine, "Energy Color: attach " + card.Def.Name + " to which of your Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Powder.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Mini Eruption: discard the top card of your deck; if it's an Energy card, 30 more damage.</summary>
        private sealed class MiniEruptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mini Eruption") return false;
                var top = ctx.Me.Deck.Count > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Player, 1) : new List<CardInstance>();
                ctx.DamageDefending(atk.Damage + (top.Any(c => c.Def.IsEnergy) ? 30 : 0));
                return true;
            }
        }

        /// <summary>Massive Eruption: discard the top 5 cards of your deck; 80 damage for each Energy card discarded.</summary>
        private sealed class MassiveEruptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Massive Eruption") return false;
                var top = ctx.Me.Deck.Count > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Player, 5) : new List<CardInstance>();
                ctx.DamageDefending(atk.Damage * top.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        /// <summary>Flare Destroy: discard an Energy from this Pokémon, then one from the opponent's Active Pokémon.</summary>
        internal sealed class FlareDestroyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Destroy") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Flare Destroy: discard an Energy from " + me.Name);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Flare Destroy: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Grand Flame: attach a Fire Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        internal sealed class GrandFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Flame") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                var bench = ctx.MyBench();
                if (pile.Count == 0 || bench.Count == 0) return true;
                var card = pile[0];
                var target = ctx.ChoosePokemon(bench, "Grand Flame: attach a Fire Energy card to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Inferno Fang: you may discard all Fire Energy from this Pokémon for 80 more damage.</summary>
        private sealed class InfernoFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inferno Fang") return false;
                var me = ctx.Pokemon;
                var fire = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fire)).ToList();
                bool more = fire.Count > 0 && ctx.AskYesNo("Inferno Fang: discard all Fire Energy from " + me.Name + " for 80 more damage?");
                if (more)
                    foreach (var e in fire) ctx.Engine.DiscardEnergy(me, e);
                ctx.DamageDefending(atk.Damage + (more ? 80 : 0));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Deep Sea Swirl: shuffle your hand into your deck, then draw 7 cards. Used for Deep Sea Swirl, Psychic Exchange, Windfall, Overhaul (the attack name and the numbers are passed in).</summary>
        internal sealed class DeepSeaSwirlMon : Mon
        {
            private readonly string attack;
            private readonly int cards;

            public DeepSeaSwirlMon(string attack, int cards)
            {
                this.attack = attack;
                this.cards = cards;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(cards);
                return true;
            }
        }

        /// <summary>Balance Bind: if both players have the same number of Benched Pokémon, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class BalanceBindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Balance Bind") return false;
                var defending = ctx.Defending;
                bool same = ctx.Me.Bench.Count == ctx.Opp.Bench.Count;
                ctx.DamageDefending(atk.Damage);
                if (same && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Abyssal Hand: once during your turn, draw until you have 5 cards in your hand. Hug: the Defending Pokémon can't retreat.</summary>
        private sealed class OctilleryBKT : Mon
        {
            private static readonly Mon Hug = new NoRetreatMon("Hug");

            protected override string Activated { get { return "Abyssal Hand"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 5 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(5); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hug.HandleAttack(ctx, atk); }
        }

        /// <summary>Dignified Fighter: your Basic Pokémon's attacks do 20 more to the opponent's Active Pokémon (before W/R); each Empoleon adds its own.</summary>
        private sealed class EmpoleonBKT : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive || damage <= 0 || !a.Def.IsBasicPokemon) return damage;
                return damage + 20;
            }
        }

        /// <summary>Ice Age: if the opponent's Active Pokémon is a Dragon Pokémon, it's now Paralyzed.</summary>
        private sealed class IceAgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Age") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.Engine.IsType(defending, EnergyType.Dragon)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>"Until the end of your next turn, each player can't play any Supporter or Stadium cards from their hand."</summary>
        internal sealed class NoSupporterOrStadiumEffect : ContinuousEffect
        {
            public NoSupporterOrStadiumEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return card != null && (card.Def.IsSupporter || card.Def.IsStadium);
            }
        }

        /// <summary>Frigid Breath: until the end of your next turn, neither player can play Supporter or Stadium cards from their hand.</summary>
        private sealed class FrigidBreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frigid Breath") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new NoSupporterOrStadiumEffect("No player can play Supporter or Stadium cards until the end of " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Frigid Breath)",
                    ctx.Player, ctx.Turn + 2));
                return true;
            }
        }

        /// <summary>Deep Freeze: flip a coin; heads, Paralyzed; tails, Confused.</summary>
        private sealed class DeepFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deep Freeze") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Paralyzed : SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Thunderclap Shot: 50 damage to each of the opponent's Pokémon-EX (W/R only for the Active).</summary>
        private sealed class ThunderclapShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderclap Shot") return false;
                foreach (var mon in ctx.OpponentPokemon(m => m.Def.IsPokemonEX))
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 50);
                return true;
            }
        }

        /// <summary>Sparkling Induction: while Active, its Retreat Cost is [C] less for each Magnemite on your Bench.</summary>
        private sealed class MagnemiteBKT : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !src.IsActive(mon)) return cost;
                return Math.Max(0, cost - src.Me.Bench.Count(m => m.Name == "Magnemite"));
            }
        }

        /// <summary>Magnetic Circuit: as often as you like during your turn, attach a Lightning Energy card from your hand to 1 of your Pokémon. Thunder Blast: discard a Lightning Energy.</summary>
        private sealed class MagnezoneBKT : Mon
        {
            private const string Circuit = "Magnetic Circuit";
            private static readonly Mon Blast = new CardsSUM1.DiscardTypeFromSelfMon("Thunder Blast", EnergyType.Lightning, 1);

            private static bool IsLightning(CardDef d) { return CardsSSP1.IsBasicOf(d, EnergyType.Lightning); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Circuit || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(x => IsLightning(x.Def)) && ctx.MyPokemon().Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.AttachFromHand(ctx, IsLightning, () => ctx.MyPokemon(), Circuit, "a Lightning Energy card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Shining Body: with any Lightning Energy attached, it takes 20 less damage from attacks (after W/R). Thunder Lance: 20 more per Lightning Energy.</summary>
        private sealed class RaikouBKT : Mon
        {
            private static readonly Mon Lance = new CardsMEG.EnergyBonusMon("Thunder Lance", EnergyType.Lightning, 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || damage <= 0) return damage;
                return src.Engine.HasEnergyOfType(me, EnergyType.Lightning) ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lance.HandleAttack(ctx, atk); }
        }
    }
}
