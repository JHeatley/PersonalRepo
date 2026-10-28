using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Temporal Forces (TEF) Pokémon added in v8. Registered by printing key. Pokémon whose attacks only do their
    /// printed damage (Pineco, Bramblin, Carvanha, Finizen, Electabuzz, Charjabug, Meditite, Medicham, Rockruff,
    /// Mudbray, Rolycoly, Meltan, Melmetal) need no script.
    /// </summary>
    public static class TefPokemon
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TEF-1", new LockAfterAttackMon("Slashing Strike", true));
            r.RegisterByKey("TEF-3", new ShieldAttackMon("Rigidify", 30));
            r.RegisterByKey("TEF-4", new CoinsMon("Comet Slap", 3));
            r.RegisterByKey("TEF-5", new ShiftryTEF());
            r.RegisterByKey("TEF-6", new CoinsMon("Double Headbutt", 2));
            r.RegisterByKey("TEF-7", new LockAfterAttackMon("Knuckle Impact", false));
            r.RegisterByKey("TEF-8", new PoisonPointMon(null));
            r.RegisterByKey("TEF-9", new PoisonPointMon("Bind Down"));
            r.RegisterByKey("TEF-10", new SolidShellMon());
            r.RegisterByKey("TEF-11", new ShieldAttackMon("Ramming Shell", 20));
            r.RegisterByKey("TEF-12", new TorterraEx());
            r.RegisterByKey("TEF-13", new ShayminTEF());
            r.RegisterByKey("TEF-14", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("TEF-15", new WhimsicottTEF());
            r.RegisterByKey("TEF-16", new DeerlingTEF());
            r.RegisterByKey("TEF-17", new SawsbuckTEF());
            r.RegisterByKey("TEF-18", new GrubbinTEF());
            r.RegisterByKey("TEF-19", new DhelmiseTEF());
            r.RegisterByKey("TEF-21", new BrambleghastTEF());
            r.RegisterByKey("TEF-22", new ScovillainEx());
            r.RegisterByKey("TEF-26", new SearchAttackMon("Charge Energy", d => d.IsBasicEnergy, 1));
            r.RegisterByKey("TEF-27", new RecoilMon("Inferno Onrush", 30));
            r.RegisterByKey("TEF-28", new SlugmaTEF());
            r.RegisterByKey("TEF-29", new MagcargoTEF());
            r.RegisterByKey("TEF-30", new VictiniTEF());
            r.RegisterByKey("TEF-31", new HeatmorTEF());
            r.RegisterByKey("TEF-32", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("TEF-33", new LockAfterAttackMon("Flare Strike", true));
            r.RegisterByKey("TEF-34", new IncineroarEx());
            r.RegisterByKey("TEF-35", new TurtonatorTEF());
            r.RegisterByKey("TEF-36", new RecoilMon("Heat Dive", 10));
            r.RegisterByKey("TEF-37", new CentiskorchTEF());
            r.RegisterByKey("TEF-38", new MeteorAssaultMon("Blaze Blitz"));
            r.RegisterByKey("TEF-39", new NoRetreatMon("Big Bite"));
            r.RegisterByKey("TEF-40", new SwitchSelfMon("Reverse Thrust", false));
            r.RegisterByKey("TEF-41", new FeraligatrTEF());
            r.RegisterByKey("TEF-43", new SharpedoTEF());
            r.RegisterByKey("TEF-44", new DrawAttackMon("Rapid Draw", 2));
            r.RegisterByKey("TEF-45", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("TEF-46", new FrosmothTEF());
            r.RegisterByKey("TEF-47", new WiglettTEF());
            r.RegisterByKey("TEF-49", new PalafinTEF());
            r.RegisterByKey("TEF-50", new WalkingWakeEx());
            r.RegisterByKey("TEF-51", new CardsMEG.SnipeMon("Random Spark", 10));
            r.RegisterByKey("TEF-52", new RaichuTEF());
            r.RegisterByKey("TEF-54", new ElectivireTEF());
            r.RegisterByKey("TEF-56", new VikavoltTEF());
            r.RegisterByKey("TEF-57", new ZeraoraTEF());
            r.RegisterByKey("TEF-58", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("TEF-59", new BoltundTEF());
            r.RegisterByKey("TEF-60", new WugtrioEx());
            r.RegisterByKey("TEF-61", new IronHandsTEF());
            r.RegisterByKey("TEF-62", new IronThornsTEF());
            r.RegisterByKey("TEF-63", new MrMimeTEF());
            r.RegisterByKey("TEF-64", new CardsPFL.FlipUntilTailsTimesMon("Ball Roll"));
            r.RegisterByKey("TEF-65", new AzumarillTEF());
            r.RegisterByKey("TEF-66", new PlusMon("Psychic Assault"));
            r.RegisterByKey("TEF-67", new DrawAttackMon("Allure", 2));
            r.RegisterByKey("TEF-70", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("TEF-71", new CoinsMon("Double Trick", 2));
            r.RegisterByKey("TEF-72", new ReuniclusTEF());
            r.RegisterByKey("TEF-73", new DodgeMon("Hide", false));
            r.RegisterByKey("TEF-74", new BeheeyemTEF());
            r.RegisterByKey("TEF-75", new CardsTWM1.HealSelfMon("Mini Drain", 10));
            r.RegisterByKey("TEF-76", new RibombeeTEF());
            r.RegisterByKey("TEF-77", new ScreamTailTEF());
            r.RegisterByKey("TEF-79", new IronValiantTEF79());
            r.RegisterByKey("TEF-80", new IronValiantTEF80());
            r.RegisterByKey("TEF-84", new RelicanthTEF());
            r.RegisterByKey("TEF-86", new RecoilMon("Wild Tackle", 50));
            r.RegisterByKey("TEF-87", new IronDefenseMon(null));
            r.RegisterByKey("TEF-88", new IronDefenseMon("Fighting Fist"));
            r.RegisterByKey("TEF-90", new LycanrocTEF());
            r.RegisterByKey("TEF-92", new MudsdaleTEF());
            r.RegisterByKey("TEF-94", new FlipBonusMon("Knock Away", 40));
            r.RegisterByKey("TEF-95", new CoalossalTEF());
            r.RegisterByKey("TEF-96", new GreatTuskTEF96());
            r.RegisterByKey("TEF-97", new GreatTuskTEF97());
            r.RegisterByKey("TEF-98", new SandyShocksTEF());
            r.RegisterByKey("TEF-99", new IronBoulderEx());
            r.RegisterByKey("TEF-100", new EkansTEF());
            r.RegisterByKey("TEF-101", new ArbokTEF());
            r.RegisterByKey("TEF-102", new GastlyTEF());
            r.RegisterByKey("TEF-103", new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("TEF-104", new GengarEx());
            r.RegisterByKey("TEF-105", new FlipBonusMon("Gnaw Off", 20));
            r.RegisterByKey("TEF-106", new CardsASC.PushDownMon("Kick Away"));
            r.RegisterByKey("TEF-107", new SableyeTEF());
            r.RegisterByKey("TEF-108", new FarigirafEx());
            r.RegisterByKey("TEF-109", new RoaringMoonTEF());
            r.RegisterByKey("TEF-110", new ForretressTEF());
            r.RegisterByKey("TEF-111", new ScizorEx());
            r.RegisterByKey("TEF-112", new MawileTEF());
            r.RegisterByKey("TEF-115", new MetagrossTEF());
            r.RegisterByKey("TEF-118", new IronTreadsTEF());
            r.RegisterByKey("TEF-119", new KoraidonTEF());
            r.RegisterByKey("TEF-120", new KoraidonExTEF());
            r.RegisterByKey("TEF-121", new MiraidonTEF());
            r.RegisterByKey("TEF-122", new MiraidonExTEF());
            r.RegisterByKey("TEF-124", new LickitungTEF());
            r.RegisterByKey("TEF-125", new LickilickyTEF());
            r.RegisterByKey("TEF-126", new CardsASR1.RevealHandAttackMon("Silent Wing"));
            r.RegisterByKey("TEF-127", new WiseHeadbuttMon("Talon Hunt"));
            r.RegisterByKey("TEF-130", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("TEF-131", new DelcattyTEF());
            r.RegisterByKey("TEF-132", new BenchSearchAttackMon("A Cappella", 3));
            r.RegisterByKey("TEF-133", new PidoveTEF());
            r.RegisterByKey("TEF-134", new LockAfterAttackMon("Jet Wing", false));
            r.RegisterByKey("TEF-135", new UnfezantTEF());
            r.RegisterByKey("TEF-136", new MinccinoTEF());
            r.RegisterByKey("TEF-137", new CinccinoTEF());
            r.RegisterByKey("TEF-138", new DrampaTEF());
            r.RegisterByKey("TEF-139", new IronJugulisTEF());
        }

        /// <summary>"Pokémon ex or Pokémon V".</summary>
        public static bool IsExOrV(CardDef d)
        {
            if (d == null) return false;
            return d.IsEx || d.IsMegaEx || d.Name.EndsWith(" V") || d.Name.EndsWith(" VSTAR") || d.Name.EndsWith(" VMAX");
        }

        /// <summary>The opponent reveals their hand (logged for both players).</summary>
        public static void RevealOpponentHand(EffectContext ctx)
        {
            var names = ctx.Opp.Hand.Select(c => c.Def.Name).ToArray();
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed their hand: " + (names.Length == 0 ? "no cards" : string.Join(", ", names)) + ".");
        }

        /// <summary>Discards a random card from the opponent's hand. Returns false if their hand was empty.</summary>
        public static bool DiscardRandomFromOpponentHand(EffectContext ctx)
        {
            var hand = ctx.Opp.Hand;
            if (hand.Count == 0) return false;
            var card = hand[ctx.State.Rng.Next(hand.Count)];
            ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            return true;
        }

        /// <summary>
        /// Milotic's Dew Guard (v58): the opponent is protected from the effects of this Supporter done to them or their hand.
        /// Logs it and returns true; the Supporter skips that part.
        /// </summary>
        public static bool OpponentShielded(EffectContext ctx)
        {
            if (!ctx.Engine.SupporterEffectsPreventedOn(ctx.Opponent)) return false;
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " is protected from the effects of " + ctx.SourceName + " on them and their hand.");
            return true;
        }

        /// <summary>The Pokémon is still the opponent's Active Pokémon and in play (it wasn't Knocked Out or moved).</summary>
        public static bool StillDefending(EffectContext ctx, PokemonInPlay mon)
        {
            return mon != null && ctx.InPlay(mon) && ctx.Opp.Active == mon;
        }
    }

    // ================================================================== Shared simple effects

    /// <summary>"During your opponent's next turn, this Pokémon takes N less damage from attacks."</summary>
    public sealed class ShieldAttackMon : Mon
    {
        private readonly string attack;
        private readonly int amount;

        public ShieldAttackMon(string attack, int amount)
        {
            this.attack = attack;
            this.amount = amount;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            ctx.SelfTakesLessNextTurn(amount);
            return true;
        }
    }

    /// <summary>"The opponent's Active Pokémon is now [condition]" after the damage.</summary>
    public sealed class ConditionAttackMon : Mon
    {
        private readonly string attack;
        private readonly SpecialCondition condition;

        public ConditionAttackMon(string attack, SpecialCondition condition)
        {
            this.attack = attack;
            this.condition = condition;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
            return true;
        }
    }

    /// <summary>"Flip a coin. If heads, the opponent's Active Pokémon is now Paralyzed."</summary>
    public sealed class ParalyzeFlipMon : Mon
    {
        private readonly string attack;

        public ParalyzeFlipMon(string attack)
        {
            this.attack = attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
            return true;
        }
    }

    /// <summary>"Flip a coin. If tails, this attack does nothing."</summary>
    public sealed class TailsNothingMon : Mon
    {
        private readonly string attack;

        public TailsNothingMon(string attack)
        {
            this.attack = attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            if (!ctx.Flip()) return Nothing(ctx);
            ctx.DamageDefending(atk.Damage);
            return true;
        }
    }

    /// <summary>"Flip a coin. If heads, this attack does N more damage."</summary>
    public sealed class FlipBonusMon : Mon
    {
        private readonly string attack;
        private readonly int bonus;

        public FlipBonusMon(string attack, int bonus)
        {
            this.attack = attack;
            this.bonus = bonus;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage + (ctx.Flip() ? bonus : 0));
            return true;
        }
    }

    /// <summary>"Put up to N Basic Pokémon from your deck onto your Bench" (Call for Family, A Cappella).</summary>
    public sealed class BenchSearchAttackMon : Mon
    {
        private readonly string attack;
        private readonly int count;

        public BenchSearchAttackMon(string attack, int count)
        {
            this.attack = attack;
            this.count = count;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                ctx.SearchToBench(d => true, count, atk.Name + ": put up to " + count + " Basic Pokémon onto your Bench");
            return true;
        }
    }

    /// <summary>Poison Point: if this Pokémon is Active and damaged by an opponent's attack, the Attacking Pokémon is Poisoned.</summary>
    public sealed class PoisonPointMon : Mon
    {
        private readonly string noRetreatAttack;

        public PoisonPointMon(string noRetreatAttack)
        {
            this.noRetreatAttack = noRetreatAttack;
        }

        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (!holderWasActive || attacker == null || !ctx.InPlay(attacker) || attacker.Owner == ctx.Player) return;
            ctx.ApplyCondition(attacker, SpecialCondition.Poisoned);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (noRetreatAttack == null || atk.Name != noRetreatAttack) return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            ctx.DefendingCantRetreat(defending);
            return true;
        }
    }

    /// <summary>Iron Defense: flip; if heads, prevent all damage done to this Pokémon by attacks next turn. Optional "+120 against ex/V" attack.</summary>
    public sealed class IronDefenseMon : Mon
    {
        private readonly string exBonusAttack;

        public IronDefenseMon(string exBonusAttack)
        {
            this.exBonusAttack = exBonusAttack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Iron Defense")
            {
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (Iron Defense)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
            if (exBonusAttack != null && atk.Name == exBonusAttack)
            {
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && TefPokemon.IsExOrV(ctx.Defending.Def) ? 120 : 0));
                return true;
            }
            return false;
        }
    }

    // ================================================================== Grass

    public sealed class ShiftryTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Expelling Tornado")
            {
                var bench = ctx.OpponentBench();
                if (bench.Count <= 3) return true;
                var keep = ctx.ChoosePokemonMany(bench, 3, 3, "Expelling Tornado: choose 3 of your opponent's Benched Pokémon to keep (the rest are shuffled into their deck)");
                foreach (var mon in bench)
                {
                    if (keep.Contains(mon) || !ctx.InPlay(mon)) continue;
                    if (!ctx.CanAffect(mon))
                    {
                        ctx.Log("The effect on " + mon.Name + " was prevented.");
                        continue;
                    }
                    ctx.Engine.ShufflePokemonIntoDeck(mon);
                }
                return true;
            }
            if (atk.Name == "Energy Loop")
            {
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || ctx.Pokemon.Energy.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Pokemon.Energy.ToList(), 1, 1, "Energy Loop: put an Energy attached to this Pokémon into your hand")[0];
                ctx.Engine.ReturnEnergyToHand(ctx.Pokemon, pick);
                return true;
            }
            return false;
        }
    }

    /// <summary>Solid Shell (Turtwig), Exoskeleton (Kakuna CRI): this Pokémon takes 20 less damage from attacks.</summary>
    public sealed class SolidShellMon : Mon
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            return info.Target == src.Pokemon && info.Attacker != null ? damage - 20 : damage;
        }
    }

    public sealed class TorterraEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Forest March")
            {
                int grass = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass)).Count;
                ctx.DamageDefending(atk.Damage * grass);
                return true;
            }
            if (atk.Name == "Jungle Hammer")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 50);
                return true;
            }
            return false;
        }
    }

    public sealed class ShayminTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Pinpoint Dive") return false;
            ctx.DamageOneBenched(60, m => TefPokemon.IsExOrV(m.Def));
            return true;
        }
    }

    /// <summary>Wafting Heal (when played to evolve): heal all damage from your Active Grass Pokémon, then discard all its Energy.</summary>
    public sealed class WhimsicottTEF : Mon
    {
        public override void OnEvolvedFromHand(EffectContext ctx)
        {
            var active = ctx.Me.Active;
            bool ready = active != null && active.DamageCounters > 0 && ctx.Engine.IsType(active, EnergyType.Grass);
            if (!TriggerOnce(ctx, "Wafting Heal", ready, "Use Wafting Heal to heal all damage from " + (active == null ? "your Active Pokémon" : active.Name) + " (then discard all of its Energy)?")) return;
            int healed = ctx.Heal(active, active.DamageCounters * 10);
            if (healed > 0) ctx.Engine.DiscardAllEnergy(active);
        }
    }

    public sealed class DeerlingTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Leaf Litter Tackle") return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(ctx.Pokemon))
                ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Leaf Litter Tackle: discard a Grass Energy from this Pokémon",
                    e => ctx.Engine.EnergyUnitsOf(ctx.Pokemon, e).Any(u => EnergyMask.Provides(u, EnergyType.Grass)));
            return true;
        }
    }

    /// <summary>Changing Seasons: once during your turn, search your deck for a Stadium.</summary>
    public sealed class SawsbuckTEF : Mon
    {
        protected override string Activated { get { return "Changing Seasons"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        protected override void Ability(EffectContext ctx)
        {
            ctx.SearchToHand(d => d.IsStadiumCard, 1, "Changing Seasons: choose a Stadium card");
        }
    }

    public sealed class GrubbinTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Flock") return false;
            if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                ctx.SearchToBench(d => d.Name == "Grubbin", 2, "Flock: put up to 2 Grubbin onto your Bench");
            return true;
        }
    }

    public sealed class DhelmiseTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Steel Anchor") return false;
            bool metal = ctx.MyBench().Any(m => ctx.Engine.IsType(m, EnergyType.Metal));
            ctx.DamageDefending(atk.Damage + (metal ? 80 : 0));
            return true;
        }
    }

    /// <summary>Resilient Soul: +50 HP for each Prize card the opponent has taken.</summary>
    public sealed class BrambleghastTEF : Mon
    {
        public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
        {
            return mon == src.Pokemon ? hp + 50 * src.OpponentPrizesTaken : hp;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Powerful Needles") return false;
            int heads = ctx.FlipCoins(ctx.EnergyCount(ctx.Pokemon));
            ctx.DamageDefending(atk.Damage * heads);
            return true;
        }
    }

    public sealed class ScovillainEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Chili Snapper Bind")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
            if (atk.Name == "Two-Headed Crushing")
            {
                ctx.DamageDefending(atk.Damage);
                TefPokemon.DiscardRandomFromOpponentHand(ctx);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
            return false;
        }
    }

    // ================================================================== Fire

    public sealed class SlugmaTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Roasting Heat") return false;
            bool burned = ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Burned);
            ctx.DamageDefending(atk.Damage + (burned ? 40 : 0));
            return true;
        }
    }

    /// <summary>Lava Zone: whenever the opponent's Active Pokémon moves to the Bench during their turn, their new Active Pokémon is Burned.</summary>
    public sealed class MagcargoTEF : Mon
    {
        public override void OnOpponentActiveMovedToBench(EffectContext src, int player, PokemonInPlay newActive)
        {
            if (player == src.Player || newActive == null || src.Opp.Active != newActive) return;
            if (src.ApplyCondition(newActive, SpecialCondition.Burned))
                src.Log("Lava Zone (" + src.Pokemon.Name + ") burned " + newActive.Name + ".");
        }
    }

    public sealed class VictiniTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Flippity Flap")
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(6);
                return true;
            }
            if (atk.Name == "Singe Off")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Singe Off: discard a Special Energy from the opponent's Active Pokémon", e => e.Def.IsAttachedSpecialEnergy);
                return true;
            }
            return false;
        }
    }

    public sealed class HeatmorTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Licking Flames") return false;
            ctx.DamageDefending(atk.Damage);
            int tails = 3 - ctx.FlipCoins(3);
            if (tails > 0 && ctx.InPlay(ctx.Pokemon))
                ctx.DiscardEnergyFrom(ctx.Pokemon, tails, "Licking Flames: discard " + tails + " Energy from this Pokémon");
            return true;
        }
    }

    /// <summary>Hustle Play: attacks cost [C] less for each of the opponent's Benched Pokémon.</summary>
    public sealed class IncineroarEx : Mon
    {
        public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
        {
            if (mon != src.Pokemon) return;
            int n = src.Opp.Bench.Count;
            for (int i = 0; i < n; i++) if (!cost.Remove(EnergyType.Colorless)) break;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Blaze Blast") return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
            return true;
        }
    }

    public sealed class TurtonatorTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Spit-Out Shot") return false;
            ctx.DamageOneBenched(40);
            return true;
        }
    }

    public sealed class CentiskorchTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Singe")
            {
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Burned);
                return true;
            }
            if (atk.Name == "Charring Breath")
            {
                if (ctx.Defending == null || !ctx.Defending.Has(SpecialCondition.Burned)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
            return false;
        }
    }

    /// <summary>Blaze Blitz: can't be used again until this Pokémon leaves the Active Spot. Used for Blaze Blitz, Meteor Assault (the attack name is passed in).</summary>
    public sealed class MeteorAssaultMon : Mon
    {
        private readonly string attack;

        public MeteorAssaultMon(string attack)
        {
            this.attack = attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive)
                ctx.AddEffect(new CantUseAttackEffect(ctx.Pokemon.Name + " can't use " + attack + " until it leaves the Active Spot", ctx.Player, ctx.Pokemon.Uid, int.MaxValue, attack));
            return true;
        }
    }

    // ================================================================== Water

    /// <summary>Torrential Heart: 5 damage counters on itself for +120 damage this turn. Giant Wave: can't use it next turn.</summary>
    public sealed class FeraligatrTEF : Mon
    {
        protected override string Activated { get { return "Torrential Heart"; } }

        protected override void Ability(EffectContext ctx)
        {
            ctx.PlaceCounters(ctx.Pokemon, 5);
            int uid = ctx.Pokemon.Uid;
            ctx.AddEffect(new ConditionalDamageBonusEffect(ctx.Pokemon.Name + "'s attacks do 120 more damage this turn (Torrential Heart)", ctx.Player, ctx.Turn, 120,
                (engine, attacker, target) => attacker.Uid == uid));
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Giant Wave") return false;
            ctx.DamageDefending(atk.Damage);
            ctx.SelfCantUseAttackNextTurn(atk.Name);
            return true;
        }
    }

    public sealed class SharpedoTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Chew Off") return false;
            ctx.DamageDefending(atk.Damage);
            int heads = ctx.FlipCoins(3);
            for (int i = 0; i < heads; i++) if (!TefPokemon.DiscardRandomFromOpponentHand(ctx)) break;
            return true;
        }
    }

    public sealed class FrosmothTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Freezing Chill") return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (TefPokemon.StillDefending(ctx, defending))
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Freezing Chill)", ctx.Player, defending.Uid, ctx.Turn + 1));
            return true;
        }
    }

    public sealed class WiglettTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Lucky Find")
            {
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsItem, 1, "Lucky Find: choose an Item card");
                return true;
            }
            if (atk.Name == "Aqua Bomb")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(20);
                return true;
            }
            return false;
        }
    }

    public sealed class PalafinTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Vanguard Punch")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10 * ctx.Pokemon.DamageCounters);
                return true;
            }
            if (atk.Name == "Double Hit")
            {
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(2));
                return true;
            }
            return false;
        }
    }

    /// <summary>Azure Seas: damage from this Pokémon's attacks ignores effects on the Defending Pokémon. Catharsis Roar: +120 if it has a Special Condition.</summary>
    public sealed class WalkingWakeEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            int damage = atk.Damage;
            if (atk.Name == "Catharsis Roar" && ctx.Defending != null && ctx.Defending.Conditions != SpecialCondition.None) damage += 120;
            bool ignore = ctx.Engine.HasAbilityNamed(ctx.Pokemon, "Azure Seas");
            ctx.DamageWith(ctx.Defending, damage, true, true, ignore);
            return true;
        }
    }

    // ================================================================== Lightning

    /// <summary>Collateral Bolts: 50 damage to every other Pokémon in play that has damage counters (both sides).</summary>
    public sealed class RaichuTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Collateral Bolts") return false;
            var theirs = ctx.OpponentPokemon(m => m.DamageCounters > 0);
            var mine = ctx.MyPokemon(m => m != ctx.Pokemon && m.DamageCounters > 0);
            foreach (var m in theirs) ctx.DamageTarget(m, 50);
            foreach (var m in mine) ctx.DamageWith(m, 50, false, false, false);
            return true;
        }
    }

    public sealed class ElectivireTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Short-Circuit Knuckle") return false;
            bool water = ctx.OpponentPokemon().Any(m => ctx.Engine.IsType(m, EnergyType.Water));
            ctx.DamageDefending(atk.Damage + (water ? 120 : 0));
            return true;
        }
    }

    public sealed class VikavoltTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Circuit Cannon") return false;
            int charjabug = ctx.MyBench().Count(m => m.Name == "Charjabug");
            ctx.DamageDefending(atk.Damage + 80 * charjabug);
            return true;
        }
    }

    public sealed class ZeraoraTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Shocking Knuckle")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
            if (atk.Name == "Strong Volt")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Strong Volt: discard an Energy from this Pokémon");
                return true;
            }
            return false;
        }
    }

    /// <summary>Electrifying Dash: search for up to 2 Basic Lightning Energy and attach them to your Benched Pokémon.</summary>
    public sealed class BoltundTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Electrifying Dash") return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Lightning, 2, "Electrifying Dash: choose up to 2 Basic Lightning Energy");
            ctx.AttachCardsAnyWay(found, found.Count, (e, m) => m.Owner == ctx.Player && ctx.Me.Bench.Contains(m), "Electrifying Dash", false);
            ctx.ShuffleMyDeck();
            return true;
        }
    }

    /// <summary>Tricolor Pump: discard up to 3 Energy cards from your hand, 60 damage to 1 opposing Pokémon for each.</summary>
    public sealed class WugtrioEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Tricolor Pump")
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var picks = ctx.ChooseCards(energy, 0, 3, "Tricolor Pump: discard up to 3 Energy cards from your hand (60 damage each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                if (picks.Count > 0) ctx.DamageAnyOpponent(60 * picks.Count);
                return true;
            }
            if (atk.Name == "Numbing Hold")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
            return false;
        }
    }

    public sealed class IronHandsTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Volt Wave")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
            if (atk.Name == "Superalloy Hands")
            {
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && TefPokemon.IsExOrV(ctx.Defending.Def) ? 80 : 0));
                return true;
            }
            return false;
        }
    }

    /// <summary>Destructo-Press: reveal the top 5, 70 damage per Future card, discard those and shuffle the rest back.</summary>
    public sealed class IronThornsTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Destructo-Press") return false;
            var top = ctx.LookAtTop(5);
            if (top.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " revealed " + string.Join(", ", top.Select(c => c.Def.Name).ToArray()) + ".");
            var future = top.Where(c => c.Def.IsFuture).ToList();
            ctx.DamageDefending(atk.Damage * future.Count);
            foreach (var c in future)
            {
                ctx.Me.Deck.Remove(c);
                ctx.Me.Discard.Add(c);
                ctx.Engine.Emit(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " + c.Def.Name + ".", c.Id);
            }
            if (top.Count > 0) ctx.ShuffleMyDeck();
            return true;
        }
    }

    // ================================================================== Psychic

    /// <summary>Look-Alike Show: use the effect of a Supporter in the opponent's hand as this attack's effect.</summary>
    public sealed class MrMimeTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Eerie Wave")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
            if (atk.Name != "Look-Alike Show") return false;
            TefPokemon.RevealOpponentHand(ctx);
            var usable = new List<CardInstance>();
            var seen = new HashSet<string>();
            foreach (var c in ctx.Opp.Hand)
            {
                if (!c.Def.IsSupporter || !seen.Add(c.Def.Name)) continue;
                if (WouldDoSomething(ctx, c)) usable.Add(c);
            }
            if (usable.Count == 0) return true;
            var labels = usable.Select(c => "Use " + c.Def.Name + "'s effect").ToList();
            labels.Add("Don't use a Supporter's effect");
            int pick = ctx.ChooseOption("Look-Alike Show: use the effect of a Supporter from your opponent's hand?", labels.ToArray());
            if (pick >= usable.Count) return true;
            var card = usable[pick];
            ctx.Log(ctx.Pokemon.Name + " uses the effect of " + card.Def.Name + ".");
            ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
            return true;
        }

        /// <summary>
        /// Ruling: only a Supporter whose effect would do something can be chosen. A Supporter's playable check covers
        /// that (an empty deck to search, nothing to heal, ...). Supporters that only add damage for the rest of the turn
        /// do nothing during this attack.
        /// </summary>
        internal static bool WouldDoSomething(EffectContext ctx, CardInstance c)
        {
            switch (c.Def.Name)
            {
                case "Black Belt's Training":
                case "Gladion's Final Battle":
                case "Briar":
                    return false;
                case "Kieran":
                    return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; // the switch; its damage option does nothing here
            }
            return ctx.Engine.Scripts.For(c.Def).CanPlayTrainer(new EffectContext(ctx.Engine, ctx.Player, c, null));
        }
    }

    public sealed class AzumarillTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Play Rough")
            {
                ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 30 : 0));
                return true;
            }
            if (atk.Name == "Power Tackle")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
            return false;
        }
    }

    /// <summary>Used for Psychic Assault, Plus (the attack name is passed in).</summary>
    public sealed class PlusMon : Mon
    {
        private readonly string attack;

        public PlusMon(string attack)
        {
            this.attack = attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            int counters = ctx.Defending == null ? 0 : ctx.Defending.DamageCounters;
            ctx.DamageDefending(atk.Damage + 10 * counters);
            return true;
        }
    }

    /// <summary>Summoning Gate: look at the top 8 and put any number of Basic Pokémon from them onto the Bench.</summary>
    public sealed class ReuniclusTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Brain Shake")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
            if (atk.Name != "Summoning Gate") return false;
            // Compendium: it can't go over the Bench limit; with a full Bench the attack does nothing (you don't even look).
            if (ctx.BenchSpace <= 0) return true;
            var top = ctx.LookAtTop(8);
            if (top.Count == 0) return true;
            // Any Pokémon, Evolution Pokémon too (your ruling): they go onto the Bench on their own, not evolved.
            var mons = top.Where(c => c.Def.IsPokemon).ToList();
            int room = ctx.BenchSpace;
            var picks = room > 0 && mons.Count > 0 ? ctx.ChooseCards(mons, 0, Math.Min(room, mons.Count), "Summoning Gate: put any number of Pokémon onto your Bench") : new List<CardInstance>();
            foreach (var c in picks)
            {
                if (ctx.BenchSpace <= 0) break;
                ctx.Me.Deck.Remove(c);
                ctx.Engine.PutOntoBench(ctx.Player, c, false);
            }
            ctx.ShuffleMyDeck();
            return true;
        }
    }

    public sealed class BeheeyemTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Cosmic Beatdown") return false;
            ctx.DamageDefending(atk.Damage * ctx.MyPokemon().Count);
            return true;
        }
    }

    /// <summary>Plentiful Pollen: during your next turn, take 2 more Prize cards if the Defending Pokémon is Knocked Out.</summary>
    public sealed class RibombeeTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Plentiful Pollen") return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!TefPokemon.StillDefending(ctx, defending) || ctx.Engine.IsKnockedOut(defending)) return true;
            var effect = new KnockOutPrizeBonusEffect("If " + defending.Name + " is Knocked Out during " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn, they take 2 more Prize cards (Plentiful Pollen)",
                ctx.Player, defending.Uid, ctx.Turn + 2, 2);
            effect.ActiveFromTurn = ctx.Turn + 2;
            ctx.AddEffect(effect);
            return true;
        }
    }

    public sealed class ScreamTailTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Supportive Singing") return false;
            var targets = ctx.MyBench().Where(m => m.Def.IsAncient && m.DamageCounters > 0).ToList();
            if (targets.Count == 0) return true;
            ctx.Heal(ctx.ChoosePokemon(targets, "Supportive Singing: heal 100 damage from which Benched Ancient Pokémon?"), 100);
            return true;
        }
    }

    public sealed class IronValiantTEF79 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Gemini Laser")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(20);
                return true;
            }
            if (atk.Name == "Zen Blade")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
            return false;
        }
    }

    /// <summary>Calculation: rearrange the top 4 cards. Majestic Sword: +100 if you played a Future Supporter this turn.</summary>
    public sealed class IronValiantTEF80 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Calculation")
            {
                var top = ctx.LookAtTop(4);
                if (top.Count < 2) return true;
                foreach (var c in top) ctx.Me.Deck.Remove(c);
                var ordered = new List<CardInstance>();
                var left = top.ToList();
                while (left.Count > 1)
                {
                    var pick = ctx.ChooseCards(left, 1, 1, "Calculation: choose the card to put " + (ordered.Count == 0 ? "on top of your deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                    ordered.Add(pick);
                    left.Remove(pick);
                }
                ordered.Add(left[0]);
                ctx.Me.Deck.InsertRange(0, ordered);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + ordered.Count + " cards back on top of their deck in any order.");
                return true;
            }
            if (atk.Name == "Majestic Sword")
            {
                var supporter = ctx.Me.SupporterThisTurn;
                ctx.DamageDefending(atk.Damage + (supporter != null && supporter.IsFuture ? 100 : 0));
                return true;
            }
            return false;
        }
    }

    // ================================================================== Fighting

    /// <summary>Memory Dive: each of your evolved Pokémon can use the attacks of its previous Evolutions.</summary>
    public sealed class RelicanthTEF : Mon
    {
        public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
        {
            if (src.Pokemon == null || mon.Owner != src.Player || mon.Stack.Count < 2) yield break;
            for (int s = 0; s < mon.Stack.Count - 1; s++)
            {
                var d = mon.Stack[s].Def;
                if (!d.IsPokemon) continue;
                for (int i = 0; i < d.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(d, i);
            }
        }
    }

    public sealed class LycanrocTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Turbo Edge") return false;
            ctx.DamageDefending(atk.Damage);
            var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
            if (energy.Count > 0 && ctx.Me.Bench.Count > 0)
                ctx.AttachCardsAnyWay(energy, 2, (e, m) => ctx.Me.Bench.Contains(m), "Turbo Edge: attach a Basic Fighting Energy from your discard pile to a Benched Pokémon");
            return true;
        }
    }

    public sealed class MudsdaleTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Mud Stock")
            {
                var bench = ctx.MyBench();
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                if (bench.Count == 0 || energy.Count == 0) return true;
                // One each. With fewer Energy than Benched Pokémon, the player picks which Pokémon get one.
                var targets = energy.Count >= bench.Count ? bench
                    : ctx.ChoosePokemonMany(bench, energy.Count, energy.Count, "Mud Stock: choose " + energy.Count + " Benched Pokémon to get a Basic Fighting Energy");
                for (int i = 0; i < targets.Count && i < energy.Count; i++)
                {
                    ctx.Me.Discard.Remove(energy[i]);
                    ctx.Engine.AttachEnergyCard(energy[i], targets[i]);
                }
                return true;
            }
            if (atk.Name == "High Horsepower")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(40);
                return true;
            }
            return false;
        }
    }

    public sealed class CoalossalTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Gatling Tar")
            {
                ctx.DamageDefending(atk.Damage + 80 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fire));
                return true;
            }
            if (atk.Name == "Cragalanche")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
                return true;
            }
            return false;
        }
    }

    public sealed class GreatTuskTEF96 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Wrathful Charge") return false;
            bool damaged = ctx.MyBench().Any(m => m.DamageCounters > 0);
            ctx.DamageDefending(atk.Damage + (damaged ? 80 : 0));
            return true;
        }
    }

    public sealed class GreatTuskTEF97 : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Land Collapse") return false;
            var supporter = ctx.Me.SupporterThisTurn;
            int cards = 1 + (supporter != null && supporter.IsAncient ? 3 : 0);
            ctx.Engine.DiscardTopOfDeck(ctx.Opponent, cards);
            return true;
        }
    }

    public sealed class SandyShocksTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Magnetic Burst") return false;
            int energyInPlay = ctx.MyPokemon().Sum(m => ctx.EnergyCount(m));
            int damage = atk.Damage + (energyInPlay >= 3 ? 70 : 0);
            ctx.DamageWith(ctx.Defending, damage, false, true, false);
            return true;
        }
    }

    /// <summary>Repulsor Axe: 8 damage counters on any Pokémon that damages this one next turn. Power Stomp: discard 2 Energy.</summary>
    public sealed class IronBoulderEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Repulsor Axe")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new RetaliateEffect(ctx.Pokemon.Name + " puts 8 damage counters on any Pokémon that damages it during the opponent's next turn (Repulsor Axe)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 8));
                return true;
            }
            if (atk.Name == "Power Stomp")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Power Stomp: discard 2 Energy from this Pokémon");
                return true;
            }
            return false;
        }
    }

    // ================================================================== Darkness

    public sealed class EkansTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Poison Blend") return false;
            if (ctx.Flip() && ctx.Defending != null)
            {
                ctx.ApplyCondition(ctx.Defending, SpecialCondition.Confused);
                ctx.ApplyCondition(ctx.Defending, SpecialCondition.Poisoned);
            }
            return true;
        }
    }

    public sealed class ArbokTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Panic Poison") return false;
            var d = ctx.Defending;
            if (d == null) return true;
            ctx.ApplyCondition(d, SpecialCondition.Burned);
            ctx.ApplyCondition(d, SpecialCondition.Confused);
            ctx.ApplyCondition(d, SpecialCondition.Poisoned);
            return true;
        }
    }

    public sealed class GastlyTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Mysterious Beam") return false;
            if (ctx.Flip() && ctx.Defending != null && ctx.Defending.Energy.Count > 0)
                ctx.DiscardEnergyFrom(ctx.Defending, 1, "Mysterious Beam: discard an Energy from the opponent's Active Pokémon");
            return true;
        }
    }

    /// <summary>Gnawing Curse: whenever the opponent attaches an Energy card from their hand, 2 damage counters on that Pokémon.</summary>
    public sealed class GengarEx : Mon
    {
        public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
        {
            if (src.Pokemon == null || target == null || target.Owner == src.Player) return;
            src.Engine.Emit(GameEventType.UseAbility, src.Player, src.Engine.PlayerName(src.Player) + "'s " + src.Pokemon.Name + ": Gnawing Curse.", src.Card.Id, src.Pokemon.Uid);
            src.PlaceCounters(target, 2);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Tricky Steps") return false;
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
            if (!ctx.AskYesNo("Tricky Steps: move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon?")) return true;
            var energy = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Tricky Steps: choose the Energy to move")[0];
            var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Tricky Steps: move it to which of your opponent's Benched Pokémon?");
            ctx.Engine.MoveEnergy(defending, to, energy);
            return true;
        }
    }

    /// <summary>Damage Collection: move any number of damage counters from the opponent's Benched Pokémon to their Active Pokémon.</summary>
    public sealed class SableyeTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Damage Collection") return false;
            var active = ctx.Defending;
            if (active == null) return true;
            for (int guard = 0; guard < 20; guard++)
            {
                var from = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                if (from.Count == 0 || !ctx.InPlay(active)) break;
                if (!ctx.AskYesNo("Damage Collection: move damage counters from your opponent's Benched Pokémon to their Active Pokémon?")) break;
                var src = ctx.ChoosePokemon(from, "Damage Collection: move damage counters from which Benched Pokémon?");
                int n = ctx.AskNumber(1, src.DamageCounters, "How many damage counters?");
                if (ctx.Engine.MoveDamageCounters(src, active, n, "Damage Collection") == 0) break;
            }
            return true;
        }
    }

    /// <summary>Armor Tail: prevent all damage done to this Pokémon by attacks from the opponent's Basic Pokémon ex.</summary>
    public sealed class FarigirafEx : Mon
    {
        public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
        {
            var a = info.Attacker;
            if (info.Target != src.Pokemon || a == null || a.Owner == src.Player) return damage;
            return a.Def.IsBasicPokemon && (a.Def.IsEx || a.Def.IsMegaEx) ? 0 : damage;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Dirty Beam") return false;
            ctx.DamageDefending(atk.Damage);
            ctx.DamageOneBenched(30);
            return true;
        }
    }

    public sealed class RoaringMoonTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Vengeance Fletching") return false;
            int ancient = ctx.Me.Discard.Count(c => c.Def.IsAncient);
            ctx.DamageDefending(atk.Damage + 10 * ancient);
            return true;
        }
    }

    // ================================================================== Metal

    public sealed class ForretressTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Spike Cannon")
            {
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                return true;
            }
            if (atk.Name == "Steel Tackle")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(40);
                return true;
            }
            return false;
        }
    }

    public sealed class ScizorEx : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Steel Wing")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.SelfTakesLessNextTurn(50);
                return true;
            }
            if (atk.Name == "Cross Breaker")
            {
                var metal = ctx.Pokemon.Energy.Where(e => ctx.Engine.EnergyUnitsOf(ctx.Pokemon, e).Any(u => EnergyMask.Provides(u, EnergyType.Metal))).ToList();
                var picks = ctx.ChooseCards(metal, 0, 2, "Cross Breaker: discard up to 2 Metal Energy from this Pokémon (120 damage each)");
                int discarded = 0;
                foreach (var e in picks) if (ctx.Engine.DiscardEnergy(ctx.Pokemon, e)) discarded++;
                ctx.DamageDefending(atk.Damage * discarded);
                return true;
            }
            return false;
        }
    }

    public sealed class MawileTEF : Mon
    {
        /// <summary>Ruling: Invite and Strike (attack 0) can't be used while the opponent has no Benched Pokémon.</summary>
        public override bool CanUseAttack(EffectContext ctx, int attackIndex)
        {
            return attackIndex != 0 || ctx.Opp.Bench.Count > 0;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Invite and Strike") return false;
            // The 30 damage is to "the new Active Pokémon": if the switch is prevented there's no new Active Pokémon, so no damage.
            if (ctx.GustOpponent("Invite and Strike: choose 1 of your opponent's Benched Pokémon to switch in") != null)
                ctx.DamageDefending(30);
            return true;
        }
    }

    /// <summary>Meteor Mash: next turn, Meteor Mash does 60 more damage. Luster Blast: discard 2 Energy.</summary>
    public sealed class MetagrossTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Meteor Mash")
            {
                int uid = ctx.Pokemon.Uid;
                int bonus = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>()
                    .Where(e => e.TargetUid == uid && e.AttackName == "Meteor Mash").Sum(e => e.Bonus);
                ctx.DamageDefending(atk.Damage + bonus);
                if (ctx.InPlay(ctx.Pokemon))
                {
                    var boost = new AttackBoostEffect(ctx.Pokemon.Name + "'s Meteor Mash does 60 more damage during its next turn", ctx.Player, uid, ctx.Turn + 2, "Meteor Mash", 60);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                }
                return true;
            }
            if (atk.Name == "Luster Blast")
            {
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Luster Blast: discard 2 Energy from this Pokémon");
                return true;
            }
            return false;
        }
    }

    /// <summary>Dual Core: Fighting and Metal type while it has a Future Booster Energy Capsule. Wheel Pass: move an Energy to the Bench.</summary>
    public sealed class IronTreadsTEF : Mon
    {
        public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
        {
            if (mon != src.Pokemon || !mon.HasToolNamed("Future Booster Energy Capsule") || (src.Engine.ToolsDisabled() && !src.Engine.StadiumShielded(mon))) return;
            if (!types.Contains(EnergyType.Fighting)) types.Add(EnergyType.Fighting);
            if (!types.Contains(EnergyType.Metal)) types.Add(EnergyType.Metal);
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Wheel Pass") return false;
            ctx.DamageDefending(atk.Damage);
            if (!ctx.InPlay(ctx.Pokemon) || ctx.Pokemon.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
            var energy = ctx.ChooseCards(ctx.Pokemon.Energy.ToList(), 1, 1, "Wheel Pass: move an Energy from this Pokémon")[0];
            var to = ctx.ChoosePokemon(ctx.MyBench(), "Wheel Pass: move it to which Benched Pokémon?");
            ctx.Engine.MoveEnergy(ctx.Pokemon, to, energy);
            return true;
        }
    }

    // ================================================================== Dragon

    public sealed class KoraidonTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Primordial Beatdown")
            {
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => m.Def.IsAncient).Count);
                return true;
            }
            if (atk.Name == "Shred")
            {
                ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return true;
            }
            return false;
        }
    }

    public sealed class KoraidonExTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Retribution Strike")
            {
                ctx.DamageDefending(atk.Damage + 10 * ctx.Pokemon.DamageCounters);
                return true;
            }
            if (atk.Name == "Kaiser Tackle")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(60);
                return true;
            }
            return false;
        }
    }

    /// <summary>Peak Acceleration: search for up to 2 Basic Energy and attach them to your Future Pokémon.</summary>
    public sealed class MiraidonTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Peak Acceleration") return false;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Me.Deck.Count == 0 || !ctx.MyPokemon().Any(m => m.Def.IsFuture)) return true;
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Peak Acceleration: choose up to 2 Basic Energy");
            ctx.AttachCardsAnyWay(found, found.Count, (e, m) => m.Owner == ctx.Player && m.Def.IsFuture, "Peak Acceleration", false);
            ctx.ShuffleMyDeck();
            return true;
        }
    }

    public sealed class MiraidonExTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Repulsion Bolt")
            {
                bool damaged = ctx.Defending != null && ctx.Defending.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage + (damaged ? 100 : 0));
                return true;
            }
            if (atk.Name == "Cyber Drive")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
            return false;
        }
    }

    // ================================================================== Colorless

    /// <summary>Tongue Pull: the opponent reveals their hand; put up to 2 Basic Pokémon from it onto their Bench.</summary>
    public sealed class LickitungTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Tongue Pull") return false;
            TefPokemon.RevealOpponentHand(ctx);
            int room = ctx.Engine.BenchSpace(ctx.Opponent);
            var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon).ToList();
            if (room <= 0 || basics.Count == 0) return true;
            var picks = ctx.ChooseCards(basics, 0, Math.Min(2, room), "Tongue Pull: put up to 2 Basic Pokémon from your opponent's hand onto their Bench");
            foreach (var c in picks)
            {
                if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) break;
                ctx.Opp.Hand.Remove(c);
                ctx.Engine.PutOntoBench(ctx.Opponent, c, false);
            }
            return true;
        }
    }

    public sealed class LickilickyTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Body Slam")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
            if (atk.Name == "Tonguenado")
            {
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(4));
                return true;
            }
            return false;
        }
    }

    /// <summary>Used for Talon Hunt, Wise Headbutt (the attack name is passed in).</summary>
    public sealed class WiseHeadbuttMon : Mon
    {
        private readonly string attack;

        public WiseHeadbuttMon(string attack)
        {
            this.attack = attack;
        }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != attack) return false;
            ctx.DamageDefending(atk.Damage);
            // Compendium (Beak Catch, Brilliant Flare): searching is optional, but a search for any card takes at least 1.
            if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo(attack + ": search your deck for up to 2 cards?"))
                ctx.SearchToHand(d => true, 2, attack + ": choose up to 2 cards", false, 1);
            return true;
        }
    }

    /// <summary>Energy Blender: move any amount of Energy between your Pokémon.</summary>
    public sealed class DelcattyTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Tail Trickery")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
            if (atk.Name != "Energy Blender") return false;
            ctx.DamageDefending(atk.Damage);
            for (int guard = 0; guard < 30; guard++)
            {
                var mine = ctx.MyPokemon();
                var withEnergy = mine.Where(m => m.Energy.Count > 0).ToList();
                if (withEnergy.Count == 0 || mine.Count < 2) break;
                if (!ctx.AskYesNo("Energy Blender: move an Energy between your Pokémon?")) break;
                var from = ctx.ChoosePokemon(withEnergy, "Energy Blender: move an Energy from which Pokémon?");
                var energy = ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Energy Blender: choose the Energy to move")[0];
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Energy Blender: move " + energy.Def.Name + " to which Pokémon?");
                ctx.Engine.MoveEnergy(from, to, energy);
            }
            return true;
        }
    }

    /// <summary>Emergency Evolution: with 30 HP or less left, evolve this Pidove straight into Unfezant or Unfezant ex from the deck.</summary>
    public sealed class PidoveTEF : Mon
    {
        protected override string Activated { get { return "Emergency Evolution"; } }

        private static bool IsUnfezant(CardDef d) { return d.IsPokemon && (d.Name == "Unfezant" || d.Name == "Unfezant ex"); }

        protected override bool AbilityReady(EffectContext ctx)
        {
            var mon = ctx.Pokemon;
            var state = ctx.State;
            if (ctx.RemainingHp(mon) > 30) return false;
            // Ruling: it ignores the normal evolution timing (first turn, came into play or evolved this turn).
            // Card effects that stop evolving still apply.
            if (mon.CantEvolveOnTurn == state.TurnNumber || ctx.Engine.EvolveLocked(ctx.Player)) return false;
            return ctx.Me.Deck.Any(c => IsUnfezant(c.Def));
        }

        protected override void Ability(EffectContext ctx)
        {
            var found = ctx.SearchMyDeck(IsUnfezant, 1, "Emergency Evolution: choose Unfezant or Unfezant ex");
            if (found.Count > 0) ctx.Engine.EvolvePokemon(ctx.Pokemon, found[0], false);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class UnfezantTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name == "Opposing Winds")
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Opposing Winds: put 2 Energy attached to the opponent's Active Pokémon into their hand?")) return true;
                var picks = defending.Energy.Count <= 2 ? defending.Energy.ToList()
                    : ctx.ChooseCards(defending.Energy.ToList(), 2, 2, "Opposing Winds: choose 2 Energy to put into your opponent's hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(defending, e);
                return true;
            }
            if (atk.Name == "Boundless Power")
            {
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
            return false;
        }
    }

    public sealed class MinccinoTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Cleaning Up") return false;
            for (int i = 0; i < 2; i++)
            {
                var withTool = ctx.OpponentPokemon(m => m.Tool != null && ctx.CanAffect(m));
                if (withTool.Count == 0) break;
                var options = withTool.SelectMany(m => m.Tools).ToList();
                var picks = ctx.ChooseCards(options, 0, 1, "Cleaning Up: discard a Pokémon Tool from your opponent's Pokémon (" + (i + 1) + " of up to 2)");
                if (picks.Count == 0) break;
                var holder = withTool.First(m => m.HasTool(picks[0]));
                if (!ctx.CanAffect(holder))
                {
                    ctx.Log("The effect on " + holder.Name + " was prevented.");
                    continue;
                }
                ctx.Engine.DiscardTool(holder, picks[0]);
            }
            return true;
        }
    }

    public sealed class CinccinoTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Special Roll") return false;
            ctx.DamageDefending(atk.Damage * ctx.Pokemon.Energy.Count(e => e.Def.IsAttachedSpecialEnergy));
            return true;
        }
    }

    public sealed class DrampaTEF : Mon
    {
        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            if (atk.Name != "Raging Cannon") return false;
            var bench = ctx.MyBench();
            bool all = bench.Count > 0 && bench.All(m => m.DamageCounters > 0);
            ctx.DamageDefending(atk.Damage + (all ? 120 : 0));
            return true;
        }
    }

    /// <summary>Automated Combat: if Active and damaged by an opponent's attack, 3 damage counters on the Attacking Pokémon.</summary>
    public sealed class IronJugulisTEF : Mon
    {
        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (!holderWasActive || attacker == null || !ctx.InPlay(attacker) || attacker.Owner == ctx.Player) return;
            ctx.PlaceCounters(attacker, 3);
        }
    }
}
