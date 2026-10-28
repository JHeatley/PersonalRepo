using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Nintendo Black Star Promos (NP 1-40, 2003-2006; v137). Every card came out before Black &amp; White: Unlimited only.
    /// Poké-Powers can't be used while the Pokémon has a Special Condition; Poké-Bodies always work. Prints of other cards
    /// (Treecko 7, Torchic 8, Mudkip 10, Tropical Wind 26, Tropical Tidal Wave 27 and 36, Celebi 29, Suicune 30) use those cards'
    /// scripts. Your rulings: Rayquaza ex's Frenzy is "40 more damage" like Kyogre ex's and Groudon ex's (a word missing on the
    /// card); the "X ex" that Frenzy and Synchronized Lift name are the Pokémon-ex of that era only, not the Pokémon ex of Scarlet
    /// &amp; Violet on, and the plain names are exact names (Groudon-EX, Primal Groudon-EX and Groudon V don't count).
    /// Pokémon whose attacks only do their printed damage: Treecko 3.
    /// </summary>
    public static class CardsNP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("NP-1", new MajorQuakeMon("Major Flood", EnergyType.Water, true));
            r.RegisterByKey("NP-2", new MajorQuakeMon("Major Earthquake", EnergyType.Fighting, false));
            r.RegisterByKey("NP-3", new PlainMon());
            r.RegisterByKey("NP-4", new ComboMon(new ConditionAttackMon("Poison Breath", SpecialCondition.Poisoned), new CardsCRZ1.PiercingMon("Swift")));
            r.RegisterByKey("NP-5", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("NP-6", new CardsGEN1.FlipTailsDiscardTypeMon("Fireworks", EnergyType.Fire, 1));
            r.RegisterByKey("NP-9", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("NP-11", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("NP-12", new ComboMon(new DodgeMon("Agility", false), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("NP-13", new CardsSUM4.DiscardDefendingToolsMon("Plunder"));
            r.RegisterByKey("NP-14", new CardsPAR2.FlipConditionMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("NP-15", new ParalyzeFlipMon("Freeze Dry"));
            r.RegisterByKey("NP-16", new CardsBCR1.FlipHealSelfMon("Sporadic Sponging", 10));
            r.RegisterByKey("NP-17", new CardsSUM1.DiscardTypeFromSelfMon("Ember", EnergyType.Fire, 1));
            r.RegisterByKey("NP-18", new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool"));
            r.RegisterByKey("NP-19", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("NP-20", new RainDishMon(new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("NP-21", new ComboMon(new ThirdEyeMon(), new CardsASR1.TimesMon("Psywave", DefendingEnergy)));
            r.RegisterByKey("NP-22", new MagneticCallMon());
            r.RegisterByKey("NP-23", new CardsOBF1.AttachFromDiscardToSelfMon("Metal Load", EnergyType.Metal));
            r.RegisterByKey("NP-24", new ChimechoNP());
            r.RegisterByKey("NP-25", new ComboMon(new NoRetreatMon("Sand Pit"), new DragonbreathMon()));
            r.RegisterByKey("NP-28", new ChampionshipArena());
            r.RegisterByKey("NP-31", new SynchronizedLiftMon(new ComboMon(new GiftMon("Flame Gift", EnergyType.Fire),
                new CardsTEU2.DiscardAllTypeMon("Scorching Wing", EnergyType.Fire)), "Articuno ex", "Zapdos ex"));
            r.RegisterByKey("NP-32", new SynchronizedLiftMon(new ComboMon(new GiftMon("Ice Gift", EnergyType.Water),
                new CardsPAR2.FlipConditionMon("Freezing Wing", SpecialCondition.Asleep)), "Zapdos ex", "Moltres ex"));
            r.RegisterByKey("NP-33", new SynchronizedLiftMon(new ComboMon(new GiftMon("Thunder Gift", EnergyType.Lightning),
                new CardsASR1.BenchRecoilMon("Lightning Wing", 10)), "Articuno ex", "Moltres ex"));
            r.RegisterByKey("NP-34", new BurningAuraMon(new ComboMon(new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep),
                new Cards30C.SelfCounterBonusMon("Rage", 10))));
            r.RegisterByKey("NP-35", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new CardsPFL.FlipUntilTailsTimesMon("Iron Tail")));
            r.RegisterByKey("NP-37", new FrenzyMon(new ComboMon(new AquaSupplyMon(), new CardsMEW1.SpreadBenchMon("Tsunami", 10)),
                "Groudon", "Groudon ex", "Rayquaza", "Rayquaza ex"));
            r.RegisterByKey("NP-38", new FrenzyMon(new ComboMon(new MixUpMon(), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)),
                "Kyogre", "Kyogre ex", "Rayquaza", "Rayquaza ex"));
            r.RegisterByKey("NP-39", new FrenzyMon(new ComboMon(new ParalyzeFlipMon("Dragon Bind"), new TwisterMon()),
                "Kyogre", "Kyogre ex", "Groudon", "Groudon ex"));
            r.RegisterByKey("NP-40", new ComboMon(new CardsASR1.TimesMon("Psywave", DefendingEnergy), new DevolutionBeamMon()));
        }

        /// <summary>Poké-Powers: "This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        private static bool NoCondition(PokemonInPlay mon)
        {
            return mon != null && mon.Conditions == SpecialCondition.None;
        }

        /// <summary>"The amount of Energy attached to the Defending Pokémon" (Psywave).</summary>
        private static int DefendingEnergy(EffectContext ctx)
        {
            return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending);
        }

        /// <summary>
        /// A Pokémon a card of this era names (Frenzy, Synchronized Lift). "Groudon ex" is the old Pokémon-ex only, not the
        /// Pokémon ex of Scarlet &amp; Violet on (your ruling); a plain name is that exact name (a LV.X counts as its base name).
        /// </summary>
        internal static bool IsNamed(PokemonInPlay mon, string name)
        {
            if (mon == null) return false;
            if (name.EndsWith(" ex", StringComparison.Ordinal)) return mon.Def.IsOldEx && mon.Name == name;
            return CardsDPP.Named(mon, name);
        }

        // ================================================================== Pokémon

        /// <summary>
        /// Major Flood / Major Earthquake: discard a [T] Energy card from this Pokémon; 10 damage to each of your Benched Pokémon
        /// (and the opponent's too, for Major Flood). No Weakness or Resistance for Benched Pokémon.
        /// </summary>
        private sealed class MajorQuakeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly bool bothBenches;

            public MajorQuakeMon(string attack, EnergyType type, bool bothBenches)
            {
                this.attack = attack;
                this.type = type;
                this.bothBenches = bothBenches;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, attack + ": discard a " + type + " Energy card from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, type));
                foreach (var benched in ctx.MyBench()) ctx.DamageTarget(benched, 10);
                if (bothBenches)
                    foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>Rain Dish (Poké-Body): at any time between turns, remove 1 damage counter from this Pokémon.</summary>
        private sealed class RainDishMon : Mon
        {
            private readonly Mon attacks;

            public RainDishMon(Mon attacks) { this.attacks = attacks; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.InPlay(me) || me.DamageCounters == 0) return;
                Announce(src, "Rain Dish");
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>The Third Eye: discard an Energy card attached to this Pokémon and draw 3 cards.</summary>
        private sealed class ThirdEyeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "The Third Eye") return false;
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 1, "The Third Eye: discard an Energy card from " + me.Name);
                ctx.Draw(3);
                return true;
            }
        }

        /// <summary>
        /// Magnetic Call (Poké-Power): once during your turn, flip a coin; heads, put a Metal Basic Pokémon from your deck onto your
        /// Bench and shuffle. Metal Charge: 1 damage counter on this Pokémon.
        /// </summary>
        private sealed class MagneticCallMon : Mon
        {
            private const string MagneticCall = "Magnetic Call";
            private static readonly Mon MetalCharge = new CardsFST2.SelfCountersMon("Metal Charge", 1);

            protected override string Activated { get { return MagneticCall; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.SearchToBench(d => d.HasType(EnergyType.Metal), 1, MagneticCall + ": choose a Metal Basic Pokémon to put onto your Bench");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MetalCharge.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Resonate: your opponent switches the Defending Pokémon with 1 of their Benched Pokémon (they choose). Healing Sound:
        /// remove 3 damage counters from your Active Pokémon; the Defending Pokémon is now Asleep.
        /// </summary>
        private sealed class ChimechoNP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Resonate")
                {
                    ctx.OpponentSwitchesActive();
                    return true;
                }
                if (atk.Name != "Healing Sound") return false;
                var defending = ctx.Defending;
                var active = ctx.Me.Active;
                if (active != null && active.DamageCounters > 0) ctx.Heal(active, 30);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Dragonbreath: flip a coin; heads, the damage and the Defending Pokémon is now Paralyzed; tails, nothing.</summary>
        private sealed class DragonbreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragonbreath") return false;
                if (!ctx.Flip()) return true;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Synchronized Lift (Poké-Body): as long as you have the other two birds (the Pokémon-ex of this era) in play, this
        /// Pokémon's Retreat Cost is 0.
        /// </summary>
        private sealed class SynchronizedLiftMon : Mon
        {
            private readonly Mon attacks;
            private readonly string[] partners;

            public SynchronizedLiftMon(Mon attacks, params string[] partners)
            {
                this.attacks = attacks;
                this.partners = partners;
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon != me) return cost;
                var mine = src.Me.InPlay().ToList();
                return partners.All(name => mine.Any(m => IsNamed(m, name))) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Flame Gift / Ice Gift / Thunder Gift: after the damage, you may move a [T] Energy card from this Pokémon to 1 of your other Pokémon.</summary>
        private sealed class GiftMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public GiftMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var typed = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, type)).ToList();
                var others = ctx.MyPokemon(m => m != me);
                if (typed.Count == 0 || others.Count == 0) return true;
                if (!ctx.AskYesNo(attack + ": move a " + type + " Energy card from " + me.Name + " to another of your Pokémon?")) return true;
                var card = ctx.ChooseCards(typed, 1, 1, attack + ": move which Energy card?")[0];
                var to = ctx.ChoosePokemon(others, attack + ": move " + card.Def.Name + " to which of your Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>
        /// Burning Aura (Poké-Body): as long as this Pokémon is your Active Pokémon, put 1 damage counter on each Active Pokémon
        /// (yours and your opponent's) between turns.
        /// </summary>
        private sealed class BurningAuraMon : Mon
        {
            private readonly Mon attacks;

            public BurningAuraMon(Mon attacks) { this.attacks = attacks; }

            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? -1 : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                Announce(src, "Burning Aura");
                var actives = new List<PokemonInPlay> { src.Me.Active, src.Opp.Active };
                foreach (var active in actives)
                    if (active != null && src.InPlay(active)) src.Engine.PlaceDamageCounters(active, 1, "Burning Aura");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Frenzy (Poké-Body): as long as your opponent has one of the named Pokémon in play, this Pokémon's attacks do 40 more
        /// damage to the Defending Pokémon (before Weakness and Resistance). Rayquaza ex's too (your ruling).
        /// </summary>
        private sealed class FrenzyMon : Mon
        {
            private readonly Mon attacks;
            private readonly string[] rivals;

            public FrenzyMon(Mon attacks, params string[] rivals)
            {
                this.attacks = attacks;
                this.rivals = rivals;
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Opp.InPlay().Any(m => rivals.Any(name => IsNamed(m, name))) ? damage + 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Aqua Supply: after the damage, you may attach a basic Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class AquaSupplyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Supply") return false;
                ctx.DamageDefending(atk.Damage);
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0) ctx.AttachCardsAnyWay(basics, 1, (c, m) => true, "Aqua Supply: attach a basic Energy card from your hand");
                return true;
            }
        }

        /// <summary>Mix-Up: flip a coin; heads, your opponent discards the top card of their deck.</summary>
        private sealed class MixUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mix-Up") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>Twister: flip 2 coins; both tails, nothing. Otherwise the damage, then discard an Energy from the Defending Pokémon for each heads.</summary>
        internal sealed class TwisterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twister") return false;
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return true;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, heads, "Twister: discard " + heads + (heads == 1 ? " Energy" : " Energy cards") + " from " + defending.Name);
                return true;
            }
        }

        /// <summary>
        /// Devolution Beam: flip a coin; heads, choose 1 of either player's Evolved Pokémon and put its highest Stage Evolution card
        /// into its owner's hand.
        /// </summary>
        private sealed class DevolutionBeamMon : Mon
        {
            private static bool Evolved(PokemonInPlay mon)
            {
                return mon != null && mon.Stack.Count > 1 && !mon.Def.IsFossil && !mon.Def.IsMultiCard;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devolution Beam") return false;
                if (!ctx.Flip()) return true;
                var evolved = ctx.MyPokemon(Evolved);
                evolved.AddRange(ctx.OpponentPokemon(Evolved));
                if (evolved.Count == 0) return true;
                var target = ctx.ChoosePokemon(evolved, "Devolution Beam: devolve which Evolved Pokémon (yours or your opponent's)?");
                if (target != null) ctx.Engine.Devolve(target, 1);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>
        /// Championship Arena (Stadium): at the end of each player's turn, if that player has 8 or more cards in their hand, they
        /// discard cards of their choice until they have 7.
        /// </summary>
        private sealed class ChampionshipArena : CardScript
        {
            public override void OnEndOfEachTurn(EffectContext src)
            {
                int player = src.State.CurrentPlayer;
                var hand = src.State.Players[player].Hand;
                if (hand.Count < 8) return;
                int n = hand.Count - 7;
                var picks = src.Engine.ChooseCards(player, hand.ToList(), n, n,
                    "Championship Arena: discard " + n + (n == 1 ? " card" : " cards") + " from your hand (down to 7)", src.Source);
                foreach (var card in picks) src.Engine.DiscardFromHand(player, card);
            }
        }
    }
}
