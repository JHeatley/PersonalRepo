using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Pokémon GO (PGO 1-88, 2022): the cards that weren't already in the pool (Radiant Charizard, the Pikachu with Wild
    /// Charge, Mewtwo VSTAR, Lunatone, Solrock and Ditto came in with Crown Zenith's reprints; Rare Candy and Professor's
    /// Research are in the pool too). Regulation mark F: Expanded only. The full arts and secret rares (71-88) are second
    /// prints. Pokémon whose attacks only do their printed damage need no script: Bulbasaur and Squirtle. Your ruling (v52):
    /// the Symbol Abilities of Moltres, Articuno and Zapdos stack.
    /// </summary>
    public static class CardsPGO
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PGO-2", new SearchAttackMon("Summoning Aroma", IsPokemonCard, 2));
            r.RegisterByKey("PGO-3", new VenusaurPGO());
            r.RegisterByKey("PGO-4", new RadiantVenusaur());
            r.RegisterByKey("PGO-5", new ComboMon(new GrowingTallMon(), new HeadSwingMon()));
            r.RegisterByKey("PGO-6", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("PGO-7", new ComboMon(new PoisonStringUpMon("Poison String-Up"), new CardsTWM1.HealSelfMon("Absorb", 50)));
            r.RegisterByKey("PGO-8", new CardsMEG.ChargeSelfMon("Tail on Fire", EnergyType.Fire, 1));
            r.RegisterByKey("PGO-9", new CardsASC.SelfDiscardTypeMon("Flamethrower", EnergyType.Fire, 1));
            r.RegisterByKey("PGO-10", new CharizardPGO());
            r.RegisterByKey("PGO-12", new SymbolMon(EnergyType.Fire, "Moltres"));
            r.RegisterByKey("PGO-13", new FlipBonusMon("Stomp", 50));
            r.RegisterByKey("PGO-14", new SplitBombMon("Split Bomb"));
            r.RegisterByKey("PGO-16", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10));
            r.RegisterByKey("PGO-17", new BlastoisePGO());
            r.RegisterByKey("PGO-18", new RadiantBlastoise());
            r.RegisterByKey("PGO-19", new ComboMon(new CardsTWM1.HealSelfMon("Hold Still", 30), new IdealFishingDayMon()));
            r.RegisterByKey("PGO-20", new SlowbroPGO());
            r.RegisterByKey("PGO-21", new ComboMon(new SearchAttackMon("Lively Grouping", IsMagikarp, 60), new RagingFinMon()));
            r.RegisterByKey("PGO-22", new ComboMon(new WreakHavocMon(), new CardsOBF1.MountainHurlMon("Wild Splash", 5)));
            r.RegisterByKey("PGO-23", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("PGO-24", new SymbolMon(EnergyType.Water, "Articuno"));
            r.RegisterByKey("PGO-25", new WimpodPGO());
            r.RegisterByKey("PGO-26", new CardsBKP2.GaleThrustMon("First Impression", 90));
            r.RegisterByKey("PGO-27", new BuddyBoltMon("Buddy Bolt", 30));
            r.RegisterByKey("PGO-29", new SymbolMon(EnergyType.Lightning, "Zapdos"));
            r.RegisterByKey("PGO-30", new CardsLOR1.ShadowCycloneMon("Transfer Break"));
            r.RegisterByKey("PGO-32", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("PGO-33", new ComboMon(new PinpointWaveMon(), new ConditionAttackMon("Mind Bend", SpecialCondition.Confused)));
            r.RegisterByKey("PGO-35", new ComboMon(new SearchAttackMon("Souvenir", AnyCard, 2), new WonderFlashMon()));
            r.RegisterByKey("PGO-36", new ComboMon(new NoRetreatMon("Rock Tomb"), new RagingSwingMon()));
            r.RegisterByKey("PGO-37", new FlipBonusMon("Rock Smash", 10));
            r.RegisterByKey("PGO-38", new CrashingBulletMon());
            r.RegisterByKey("PGO-40", new ComboMon(new CounterMon(), new CardsPRC1.DynamicPunchMon(90)));
            r.RegisterByKey("PGO-41", new TailsNothingMon("Hyper Fang"));
            r.RegisterByKey("PGO-42", new ComboMon(new ChaseUpMon(), new SuperFangMon()));
            r.RegisterByKey("PGO-43", new ComboMon(new RagingCrashMon(), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20)));
            r.RegisterByKey("PGO-44", new ComboMon(new ParalyzeFlipMon("Body Slam"), new LockAfterAttackMon("Iron Buster", false)));
            r.RegisterByKey("PGO-45", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("PGO-46", new CardsWHT.FlipMoreMon("Swinging Smash", 2, 90));
            r.RegisterByKey("PGO-47", new ArmChargeMon());
            r.RegisterByKey("PGO-48", new JuggernautMon());
            r.RegisterByKey("PGO-49", new CardsDRI2.CrunchMon("Hyper Beam"));
            r.RegisterByKey("PGO-50", new ComboMon(new LockAfterAttackMon("Giga Impact", false), new DraconicStarMon()));
            r.RegisterByKey("PGO-51", new CardsOBF1.HealBenchedMon("Delicious Egg", 30));
            r.RegisterByKey("PGO-52", new CardsOBF1.HealBenchedMon("Enriching Egg", 9999));
            r.RegisterByKey("PGO-54", new WhinyVoiceMon());
            r.RegisterByKey("PGO-55", new SnorlaxPGO());
            r.RegisterByKey("PGO-56", new DodgeMon("Bustle", false));
            r.RegisterByKey("PGO-57", new AmbipomPGO());
            r.RegisterByKey("PGO-58", new SlakingV());
            r.RegisterByKey("PGO-59", new RecoilMon("Take Down", 10));
            r.RegisterByKey("PGO-60", new BibarelPGO());
            r.RegisterByKey("PGO-61", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("PGO-62", new FlipBonusMon("Aerial Ace", 20));
            r.RegisterByKey("PGO-63", new CoinsMon("Hurricane Wing", 4));

            r.RegisterByName("Blanche", new TeamLeader(EnergyType.Water));
            r.RegisterByName("Candela", new TeamLeader(EnergyType.Fire));
            r.RegisterByName("Spark", new TeamLeader(EnergyType.Lightning));
            r.RegisterByName("Egg Incubator", new EggIncubator());
            r.RegisterByName("Lure Module", new LureModule());
            r.RegisterByName("PokéStop", new PokeStop());
        }

        private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static bool AnyCard(CardDef d) { return true; }
        private static bool IsMagikarp(CardDef d) { return d.IsPokemon && d.Name == "Magikarp"; }

        // ================================================================== Grass

        /// <summary>Loopy Lasso: once during your turn, flip; heads, switch in 1 of the opponent's Benched Pokémon, which becomes Asleep and Poisoned.</summary>
        private sealed class VenusaurPGO : Mon
        {
            protected override string Activated { get { return "Loopy Lasso"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var target = ctx.GustOpponent("Loopy Lasso: choose 1 of your opponent's Benched Pokémon to switch in (it becomes Asleep and Poisoned)");
                if (target == null) return;
                ctx.ApplyCondition(target, SpecialCondition.Asleep);
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Sunny Bloom: once at the end of your turn (after attacking), you may draw until you have 4 cards. Pollen Hazard: Burned, Confused and Poisoned.</summary>
        private sealed class RadiantVenusaur : Mon
        {
            private static readonly Mon hazard = new CardsCES1.ConditionsMon("Pollen Hazard", SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned);

            public override void OnEndOfTurn(EffectContext src)
            {
                // Compendium (2022-08-04): after attacking, Knock Outs and Prize cards, before Pokémon Checkup (like Precious Gift).
                if (src.Pokemon == null || !src.InPlay(src.Pokemon) || src.Me.Hand.Count >= 4 || src.Me.Deck.Count == 0) return;
                if (src.AbilityUsedThisTurn("Sunny Bloom") || !src.AskYesNo("Sunny Bloom: draw cards until you have 4 cards in your hand?")) return;
                src.MarkAbilityUsed("Sunny Bloom");
                Announce(src, "Sunny Bloom");
                src.DrawUntil(4);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return hazard.HandleAttack(ctx, atk); }
        }

        /// <summary>Growing Tall: flip; heads, search for up to 5 Grass Energy cards and attach them to your Pokémon in any way you like.</summary>
        private sealed class GrowingTallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Growing Tall") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 5, "Growing Tall: choose up to 5 Grass Energy cards to attach to your Pokémon");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, mon) => true, "Growing Tall");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Head Swing: 30 damage to 1 of the opponent's Pokémon for each Grass Energy attached to this Pokémon.</summary>
        private sealed class HeadSwingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Head Swing") return false;
                int n = CardsCRZ1.EnergyOfType(ctx, ctx.Pokemon, EnergyType.Grass).Count;
                if (n > 0) ctx.DamageAnyOpponent(30 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Head Swing did no damage.");
                return true;
            }
        }

        /// <summary>Poison String-Up: 20; flip, heads: the opponent's Active Pokémon is Paralyzed and Poisoned. Used for Poison String-Up, Dangerous Spores (the attack name is passed in).</summary>
        internal sealed class PoisonStringUpMon : Mon
        {
            private readonly string attack;

            public PoisonStringUpMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Burn Brightly: each basic Fire Energy on your Pokémon provides 2 Fire Energy (doesn't stack). Flare Blitz: discard all Fire Energy from it.</summary>
        private sealed class CharizardPGO : Mon
        {
            private static readonly Mon blitz = new CardsASC.SelfDiscardTypeMon("Flare Blitz", EnergyType.Fire, 99);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || holder.Owner != src.Player || !CardsSSP1.IsBasicOf(energy.Def, EnergyType.Fire)) return;
                units.Clear();
                units.Add(EnergyMask.Of(EnergyType.Fire));
                units.Add(EnergyMask.Of(EnergyType.Fire));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return blitz.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Flare / Ice / Lightning Symbol: your Basic Pokémon of that type, except this Pokémon's name, do 10 more damage to the
        /// opponent's Active Pokémon (before Weakness and Resistance). Several copies stack (your ruling, v52).
        /// </summary>
        private sealed class SymbolMon : Mon
        {
            private readonly EnergyType type;
            private readonly string self;

            public SymbolMon(EnergyType type, string self)
            {
                this.type = type;
                this.self = self;
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive) return damage;
                if (!attacker.Def.IsBasicPokemon || attacker.Def.Name == self || !src.Engine.IsType(attacker, type)) return damage;
                return damage + 10;
            }
        }

        /// <summary>Split Bomb: 50 damage to 2 of the opponent's Pokémon. Used for Split Bomb, Sonic Double, Dual Splash (the attack name is passed in).</summary>
        internal sealed class SplitBombMon : Mon
        {
            private readonly string attack;

            public SplitBombMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                CardsDRI1.DamageTwoOpponents(ctx, 50, atk.Name);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Vitality Spring: once during your turn, search for up to 6 Energy cards and attach them to your Pokémon in any way you like; your turn ends. Hydro Pump: 30 more per Water Energy.</summary>
        private sealed class BlastoisePGO : Mon
        {
            private static readonly Mon pump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 30);

            protected override string Activated { get { return "Vitality Spring"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsEnergy, 6, "Vitality Spring: choose up to 6 Energy cards to attach to your Pokémon");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, mon) => true, "Vitality Spring");
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Pump Shot: once during your turn, discard a Water Energy card from your hand to put 2 damage counters on 1 of the opponent's Benched Pokémon. Torrential Cannon: not next turn.</summary>
        private sealed class RadiantBlastoise : Mon
        {
            private static readonly Mon cannon = new LockAfterAttackMon("Torrential Cannon", true);

            private static List<CardInstance> Water(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
            }

            protected override string Activated { get { return "Pump Shot"; } }
            protected override bool AbilityReady(EffectContext ctx) { return Water(ctx).Count > 0 && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var water = Water(ctx);
                if (water.Count == 0) return;
                var card = water.Select(c => c.Def.Name).Distinct().Count() == 1 ? water[0]
                    : ctx.ChooseCards(water, 1, 1, "Pump Shot: discard a Water Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return;
                var target = ctx.ChoosePokemon(bench, "Pump Shot: put 2 damage counters on which of your opponent's Benched Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return cannon.HandleAttack(ctx, atk); }
        }

        /// <summary>Ideal Fishing Day: put an Item card from your discard pile into your hand.</summary>
        private sealed class IdealFishingDayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ideal Fishing Day") return false;
                ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(items, 1, 1, "Ideal Fishing Day: put an Item card from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>Tumbling Tackle: both Active Pokémon become Asleep. Twilight Inspiration: only when the opponent has exactly 1 Prize card left; take 2 Prize cards.</summary>
        private sealed class SlowbroPGO : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Twilight Inspiration" || ctx.Opp.Prizes.Count == 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tumbling Tackle")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name == "Twilight Inspiration")
                {
                    ctx.Engine.TakePrizeCards(ctx.Player, 2);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Raging Fin: 30 more for each Magikarp and Gyarados in your discard pile.</summary>
        private sealed class RagingFinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Fin") return false;
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon && (c.Def.Name == "Magikarp" || c.Def.Name == "Gyarados"));
                ctx.DamageDefending(atk.Damage + 30 * n);
                return true;
            }
        }

        /// <summary>Wreak Havoc: flip until tails; discard the top 2 cards of the opponent's deck for each heads.</summary>
        private sealed class WreakHavocMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wreak Havoc") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2 * heads);
                return true;
            }
        }

        /// <summary>Punk Out: no Retreat Cost while the opponent has any Pokémon V in play.</summary>
        private sealed class WimpodPGO : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return src.Opp.InPlay().Any(m => m.Def.IsPokemonV) ? 0 : cost;
            }
        }

        // ================================================================== Lightning

        /// <summary>Buddy Bolt: 30 more if you played a Supporter card from your hand this turn. Used for Buddy Bolt, Driving Buddy (the attack name and the numbers are passed in).</summary>
        internal sealed class BuddyBoltMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BuddyBoltMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.SupporterPlayedThisTurn ? amount : 0));
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Pinpoint Wave: 90 damage to 1 of the opponent's Pokémon V, not affected by Weakness or Resistance.</summary>
        private sealed class PinpointWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pinpoint Wave") return false;
                var targets = ctx.OpponentPokemon(m => m.Def.IsPokemonV);
                if (targets.Count == 0)
                {
                    ctx.Log(ctx.Pokemon.Name + "'s Pinpoint Wave had no Pokémon V to hit.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Pinpoint Wave: do 90 damage to which of your opponent's Pokémon V?");
                ctx.DamageWith(target, 90, false, false, false);
                return true;
            }
        }

        /// <summary>Wonder Flash: 90 more against a Dragon Pokémon.</summary>
        private sealed class WonderFlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wonder Flash") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && ctx.Engine.IsType(defending, EnergyType.Dragon) ? 90 : 0));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Raging Swing: 50 for each damage counter on this Pokémon.</summary>
        private sealed class RagingSwingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Swing") return false;
                int n = ctx.Pokemon.DamageCounters;
                if (n > 0) ctx.DamageDefending(50 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Raging Swing did no damage.");
                return true;
            }
        }

        /// <summary>Crashing Bullet: 20, and 20 to each Benched Pokémon (both players').</summary>
        private sealed class CrashingBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crashing Bullet") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).ToList()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Counter: as much more damage as this Pokémon took from attacks during the opponent's last turn.</summary>
        private sealed class CounterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Counter") return false;
                var me = ctx.Pokemon;
                int took = me.AttackDamageTurn == ctx.OpponentsLastTurn ? me.AttackDamageThatTurn : 0;
                ctx.DamageDefending(atk.Damage + took);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Chase Up: search your deck for a card.</summary>
        private sealed class ChaseUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chase Up") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Chase Up: put any card from your deck into your hand", false, 1);
                return true;
            }
        }

        /// <summary>Super Fang: damage counters on the opponent's Active Pokémon until it has 10 HP left.</summary>
        internal sealed class SuperFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Fang") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int n = (ctx.RemainingHp(defending) - 10) / 10;
                if (n > 0) ctx.PlaceCounters(defending, n);
                return true;
            }
        }

        /// <summary>Raging Crash: 10 for each damage counter on all of your Benched Pokémon.</summary>
        private sealed class RagingCrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Crash") return false;
                int n = ctx.Me.Bench.Sum(m => m.DamageCounters);
                if (n > 0) ctx.DamageDefending(10 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Raging Crash did no damage.");
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Arm Charge: 50; you may attach a Metal Energy card from your hand to this Pokémon.</summary>
        private sealed class ArmChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Arm Charge") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) CardsCRZ1.AttachEnergyFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Metal), 1, m => m == me, "Arm Charge");
                return true;
            }
        }

        /// <summary>G-Max Juggernaut: 60 more for each Metal Energy beyond the attack's cost, up to 120 more.</summary>
        private sealed class JuggernautMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Juggernaut") return false;
                int metal = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal);
                int needed = atk.Cost.Count(t => t == EnergyType.Metal);
                int extra = Math.Min(2, Math.Max(0, metal - needed));
                ctx.DamageDefending(atk.Damage + 60 * extra);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Draconic Star (VSTAR Power): look at the top 12 cards and attach any Water or Lightning Energy cards there to your Pokémon in any way you like; shuffle the rest back.</summary>
        private sealed class DraconicStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draconic Star") return false;
                var top = ctx.LookAtTop(12);
                var energy = top.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water) || CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, energy.Count, (card, mon) => true, "Draconic Star: attach a Water or Lightning Energy card to your Pokémon");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Whiny Voice: a random card from the opponent's hand is revealed and shuffled into their deck.</summary>
        private sealed class WhinyVoiceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whiny Voice") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { card }, "revealed a random card from " + ctx.Engine.Their(ctx.Opponent) + " hand");
                hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Block: while this Pokémon is Active, the opponent's Active Pokémon can't retreat. Collapse: 150, then it's Asleep.</summary>
        private sealed class SnorlaxPGO : Mon
        {
            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.PokemonIsActive && mon.Owner != src.Player && src.Opp.Active == mon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Collapse") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Primate Dexterity: whenever attack damage would be done to it, flip; heads prevents it. Full Tilt Fling: a coin per Energy, 60 per heads.</summary>
        private sealed class AmbipomPGO : Mon
        {
            private static readonly Mon fling = new CardsASC.FlipPerEnergyMon("Full Tilt Fling");

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && src.Engine.FlipCoin(src.Player, "Primate Dexterity (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return fling.HandleAttack(ctx, atk); }
        }

        /// <summary>Kinda Lazy: it can't attack while you have exactly 2, 4 or 6 Prize cards left.</summary>
        private sealed class SlakingV : Mon
        {
            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return false;
                int left = src.Me.Prizes.Count;
                return left == 2 || left == 4 || left == 6;
            }
        }

        /// <summary>
        /// Reassuring Dam: while on the Bench, cards in your deck can't be discarded by the opponent's attacks, Abilities, Item
        /// cards or Supporter cards (not their Pokémon Tools: Compendium, Cursed Shovel, 2024-03-21).
        /// </summary>
        private sealed class BibarelPGO : Mon
        {
            public override bool ProtectsDeckFromDiscard(EffectContext src, int player)
            {
                if (src.Pokemon == null || player != src.Player || src.PokemonIsActive) return false;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Trainer) return true;
                var trainer = src.Engine.ResolvingTrainer;
                return trainer != null && (trainer.IsItem || trainer.IsSupporter);
            }
        }

        // ================================================================== Trainers

        /// <summary>Blanche, Candela, Spark: draw 2 cards; if you drew any, flip; heads, attach a basic Energy of the team's type from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class TeamLeader : CardScript
        {
            private readonly EnergyType type;

            public TeamLeader(EnergyType type) { this.type = type; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Draw(2) == 0 || !ctx.Flip()) return;
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                var bench = ctx.MyBench();
                if (energy.Count == 0 || bench.Count == 0) return;
                var card = energy.Select(c => c.Def.Name).Distinct().Count() == 1 ? energy[0]
                    : ctx.ChooseCards(energy, 1, 1, ctx.SourceName + ": choose a " + type + " Energy card from your discard pile")[0];
                var target = ctx.ChoosePokemon(bench, ctx.SourceName + ": attach " + card.Def.Name + " to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>Egg Incubator: flip; heads, put a Basic Pokémon from your deck onto your Bench; tails, this card goes to the bottom of your deck.</summary>
        private sealed class EggIncubator : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip())
                {
                    ctx.ReturnToDeckBottom = true;
                    return;
                }
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => true, 1, "Egg Incubator: choose a Basic Pokémon to put onto your Bench");
            }
        }

        /// <summary>Lure Module: each player reveals the top 3 cards of their deck, puts the Pokémon there into their hand and shuffles the rest back.</summary>
        private sealed class LureModule : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    var top = p.Deck.Take(Math.Min(3, p.Deck.Count)).ToList();
                    if (top.Count == 0) continue;
                    ctx.Engine.Reveal(player, top, "revealed the top of " + ctx.Engine.Their(player) + " deck");
                    foreach (var card in top.Where(c => c.Def.IsPokemon).ToList())
                    {
                        p.Deck.Remove(card);
                        p.Hand.Add(card);
                    }
                    ctx.Engine.ShuffleDeck(player);
                }
            }
        }

        /// <summary>PokéStop: once during each player's turn, they may discard the top 3 cards of their deck and put the Item cards discarded into their hand.</summary>
        private sealed class PokeStop : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var gone = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                var items = gone.Where(c => c.Def.IsItem && ctx.Me.Discard.Contains(c)).ToList();
                // You see the 3 cards before the Item cards among them go into your hand (v69).
                ctx.Engine.ShowCards(ctx.Player, gone, "Discarded by PokéStop",
                    "PokéStop discarded these cards" + (items.Count == 0 ? " (no Item cards among them)."
                        : items.Count == 1 ? ". The Item card goes into your hand." : ". The " + items.Count + " Item cards go into your hand."), ctx.Source);
                if (items.Count > 0) ctx.DiscardToHand(items);
                return false;
            }
        }
    }
}
