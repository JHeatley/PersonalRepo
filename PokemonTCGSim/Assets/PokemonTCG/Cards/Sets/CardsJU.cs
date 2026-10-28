using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Jungle (JU, v114): the Pokémon (1-16 holo, 17-32 their non-holo prints, 33-63). Poké Ball (64) is Perfect Order's card.
    /// Pokémon Powers can't be used (and passive ones stop) while the Pokémon is Asleep, Confused or Paralyzed. Your ruling (v114):
    /// Venomoth's Shift lasts until Shift is used again or Venomoth leaves play.
    /// </summary>
    public static class CardsJU
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("JU-1", new ComboMon(new CardsBS1.MetronomeMon(), new MinimizeMon()));
            r.RegisterByKey("JU-2", new ChainLightningMon());
            r.RegisterByKey("JU-3", new CardsBS1.DiscardCostMon("Flamethrower", EnergyType.Fire, 1, null, new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("JU-4", new ComboMon(new FlipBonusMon("Quick Attack", 20), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("JU-5", new ComboMon(new DrawAttackMon("Fetch", 1), new CoinsMon("Comet Punch", 4)));
            r.RegisterByKey("JU-6", new MrMimeInvisibleWall());
            r.RegisterByKey("JU-7", new BoyfriendsMon());
            r.RegisterByKey("JU-8", new HurricaneReturnMon());
            r.RegisterByKey("JU-9", new ParalyzeFlipMon("Irongrip"));
            r.RegisterByKey("JU-10", new SwordsDanceSlashMon());
            r.RegisterByKey("JU-11", new SnorlaxThickSkinned());
            r.RegisterByKey("JU-12", new ComboMon(new FlipBonusMon("Quick Attack", 20), new CardsBS1.ExtraWaterMon("Water Gun")));
            r.RegisterByKey("JU-13", new VenomothShift());
            r.RegisterByKey("JU-14", new ComboMon(new CardsBS1.LureMon("Lure"), new AcidMon()));
            r.RegisterByKey("JU-15", new VileplumeHeal());
            r.RegisterByKey("JU-16", new ComboMon(new ConditionAttackMon("Lullaby", SpecialCondition.Asleep), new CardsFCO3.DoTheWaveMon("Do the Wave", 10)));
            r.RegisterByKey("JU-33", new ComboMon(new CardsASC.PushDownMon("Whirlwind"), new HalfDrainMon("Mega Drain")));
            r.RegisterByKey("JU-34", new DodrioRetreatAid());
            r.RegisterByKey("JU-35", new ComboMon(new SwitchSelfMon("Teleport", false), new BigEggsplosionMon()));
            r.RegisterByKey("JU-36", new DodgeMon("Agility", false));
            r.RegisterByKey("JU-37", new ComboMon(new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned), new CardsCEC1.ConfusionWaveMon("Foul Odor")));
            r.RegisterByKey("JU-38", new ComboMon(new ParalyzeFlipMon("Tongue Wrap"), new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused)));
            r.RegisterByKey("JU-39", new ComboMon(new CoinsMon("Bonemerang", 2), new FamilyMon("Call for Friend", d => d.PokemonType == EnergyType.Fighting, "a Basic Fighting Pokémon")));
            r.RegisterByKey("JU-40", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("JU-41", new ConditionAttackMon("Spore", SpecialCondition.Asleep));
            r.RegisterByKey("JU-42", new ReduceFromDefendingMon("Pounce", 10));
            r.RegisterByKey("JU-43", new ComboMon(new CoinsMon("Fury Swipes", 3), new TantrumSelfMon()));
            r.RegisterByKey("JU-44", new ComboMon(new FlipBonusMon("Stomp", 10), new DodgeMon("Agility", false)));
            r.RegisterByKey("JU-45", new RamMon());
            r.RegisterByKey("JU-47", new ComboMon(new FlipBonusMon("Stomp", 10), new RampageMon()));
            r.RegisterByKey("JU-48", new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("JU-49", new FamilyMon("Call for Family", d => d.Name == "Bellsprout", "a Basic Bellsprout"));
            r.RegisterByKey("JU-50", new ComboMon(new ReduceFromDefendingMon("Snivel", 20), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("JU-51", new ComboMon(new CantAttackThisMon("Tail Wag"), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("JU-52", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new CardsBS1.LeechSeedMon()));
            r.RegisterByKey("JU-54", new ConditionAttackMon("Lullaby", SpecialCondition.Asleep));
            r.RegisterByKey("JU-55", new MankeyPeek());
            r.RegisterByKey("JU-56", new PayDayFlipMon());
            r.RegisterByKey("JU-57", new ComboMon(new CoinsMon("Fury Swipes", 3), new FamilyMon("Call for Family", d => d.Name == "Nidoran♂" || d.Name == "Nidoran♀", "a Basic Nidoran♂ or Nidoran♀")));
            r.RegisterByKey("JU-58", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new FamilyMon("Sprout", d => d.Name == "Oddish", "a Basic Oddish")));
            r.RegisterByKey("JU-59", new ConditionAttackMon("Spore", SpecialCondition.Asleep));
            r.RegisterByKey("JU-60", new Cards30CC.BenchHitMon("Spark", 10));
            r.RegisterByKey("JU-61", new CantAttackThisMon("Leer"));
            r.RegisterByKey("JU-62", new CardsBS1.MirrorMoveMon());
            r.RegisterByKey("JU-63", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new CardsDAA1.DrainMon("Leech Life")));
        }

        // ================================================================== Timed effects

        /// <summary>
        /// "Damage done to this Pokémon by attacks is reduced by N (after Weakness and Resistance)" during the opponent's next turn.
        /// With an attacker (Pounce, Snivel) only that Pokémon's attacks count, and benching either Pokémon ends it.
        /// </summary>
        internal sealed class TakeLessEffect : ContinuousEffect
        {
            private readonly int protectedUid;
            private readonly int attackerUid;
            private readonly int amount;

            public TakeLessEffect(string name, int owner, int protectedUid, int attackerUid, int expiresAfterTurn, int amount)
                : base(name, owner, attackerUid >= 0 ? attackerUid : protectedUid, expiresAfterTurn, true)
            {
                this.protectedUid = protectedUid;
                this.attackerUid = attackerUid;
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Target == null || info.Target.Uid != protectedUid) return damage;
                if (attackerUid >= 0 && (info.Attacker.Uid != attackerUid || !engine.State.IsActive(info.Target))) return damage;
                return Math.Max(0, damage - amount);
            }
        }

        /// <summary>Tail Wag / Leer: the Defending Pokémon can't attack while the protected Pokémon is the Active Pokémon; benching either ends it.</summary>
        internal sealed class CantAttackThisEffect : ContinuousEffect
        {
            private readonly int protectedUid;

            public CantAttackThisEffect(string name, int owner, int defendingUid, int protectedUid, int expiresAfterTurn)
                : base(name, owner, defendingUid, expiresAfterTurn, true)
            {
                this.protectedUid = protectedUid;
            }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon)
            {
                if (mon == null || mon.Uid != TargetUid) return false;
                var protectedMon = engine.State.FindPokemon(protectedUid);
                return protectedMon != null && engine.State.IsActive(protectedMon);
            }
        }

        // ================================================================== Pokémon Powers

        /// <summary>Invisible Wall: whenever an attack (even your own) does 30 or more damage to Mr. Mime after W/R, prevent that damage.</summary>
        private sealed class MrMimeInvisibleWall : Mon
        {
            private static readonly Mon Meditate = new CardsMEW1.CounterBonusMon("Meditate", 10);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Attacker != null && info.Target == src.Pokemon && damage >= 30 && CardsBS1.PowerWorks(src.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Meditate.HandleAttack(ctx, atk); }
        }

        /// <summary>Thick Skinned: Snorlax can't become Asleep, Confused, Paralyzed or Poisoned. Body Slam: heads, Paralyzed.</summary>
        private sealed class SnorlaxThickSkinned : Mon
        {
            private static readonly Mon Slam = new ParalyzeFlipMon("Body Slam");

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target == src.Pokemon && condition != SpecialCondition.Burned && CardsBS1.PowerWorks(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slam.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Shift: once during your turn, Venomoth becomes the type of another non-Colorless Pokémon in play (your ruling: until Shift
        /// is used again or it leaves play). Venom Powder: heads, Confused and Poisoned.
        /// </summary>
        private sealed class VenomothShift : Mon
        {
            private const string Shift = "Shift";

            protected override string Activated { get { return Shift; } }

            private static List<EnergyType> Choices(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var mine = ctx.Engine.TypesOf(me);
                var all = ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(m => m != me);
                return all.SelectMany(m => ctx.Engine.TypesOf(m)).Where(t => t != EnergyType.Colorless && !(mine.Count == 1 && mine[0] == t)).Distinct().OrderBy(t => (int)t).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return CardsBS1.PowerWorks(ctx.Pokemon) && Choices(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var choices = Choices(ctx);
                if (choices.Count == 0) return;
                int pick = ctx.ChooseOneOption("Shift: change " + ctx.Pokemon.Name + " to which type?", choices.Select(t => t.ToString()).ToArray());
                if (pick < 0 || pick >= choices.Count) return;
                ctx.Pokemon.Markers[PokemonInPlay.ShiftTypeMarker] = (int)choices[pick];
                ctx.Log(ctx.Pokemon.Name + " is now a " + choices[pick] + " Pokémon (Shift).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Venom Powder") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Heal: once during your turn, flip a coin; heads, remove 1 damage counter from 1 of your Pokémon. Petal Dance: 3 coins x40, then Confused.</summary>
        private sealed class VileplumeHeal : Mon
        {
            private const string Heal = "Heal";
            private static readonly Mon Dance = new CardsCEC3.PetalDanceMon();

            protected override string Activated { get { return Heal; } }

            private static List<PokemonInPlay> Hurt(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)); }

            protected override bool AbilityReady(EffectContext ctx) { return CardsBS1.PowerWorks(ctx.Pokemon) && Hurt(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var hurt = Hurt(ctx);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Heal: remove 1 damage counter from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dance.HandleAttack(ctx, atk); }
        }

        /// <summary>Retreat Aid: while Dodrio is on your Bench, your Active Pokémon's Retreat Cost is [C] less. Rage: +10 per counter on Dodrio.</summary>
        private sealed class DodrioRetreatAid : Mon
        {
            private static readonly Mon Rage = new Cards30C.SelfCounterBonusMon("Rage", 10);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon != src.Me.Active || !src.Me.Bench.Contains(me) || !CardsBS1.PowerWorks(me)) return cost;
                return Math.Max(0, cost - 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Rage.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Peek: once during your turn, look at 1 of these: the top card of either deck, a random card from your opponent's hand, or 1
        /// of either player's Prize cards.
        /// </summary>
        private sealed class MankeyPeek : Mon
        {
            private const string Peek = "Peek";

            protected override string Activated { get { return Peek; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsBS1.PowerWorks(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                var labels = new List<string>();
                var actions = new List<int>();
                if (ctx.Me.Deck.Count > 0) { labels.Add("The top card of your deck"); actions.Add(0); }
                if (ctx.Opp.Deck.Count > 0) { labels.Add("The top card of your opponent's deck"); actions.Add(1); }
                if (ctx.Opp.Hand.Count > 0) { labels.Add("A random card from your opponent's hand"); actions.Add(2); }
                if (ctx.Me.Prizes.Count > 0) { labels.Add("1 of your Prize cards"); actions.Add(3); }
                if (ctx.Opp.Prizes.Count > 0) { labels.Add("1 of your opponent's Prize cards"); actions.Add(4); }
                if (labels.Count == 0) return;
                int pick = ctx.ChooseOneOption("Peek: look at which card?", labels.ToArray());
                if (pick < 0 || pick >= labels.Count) return;
                CardInstance card;
                string what;
                switch (actions[pick])
                {
                    case 0: card = ctx.Me.Deck[0]; what = "the top card of your deck"; break;
                    case 1: card = ctx.Opp.Deck[0]; what = "the top card of your opponent's deck"; break;
                    case 2: card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)]; what = "a random card from your opponent's hand"; break;
                    default:
                    {
                        var prizes = actions[pick] == 3 ? ctx.Me.Prizes : ctx.Opp.Prizes;
                        var slots = new List<string>();
                        for (int k = 0; k < prizes.Count; k++) slots.Add("Prize card " + (k + 1));
                        int slot = ctx.ChooseOneOption("Peek: look at which Prize card?", slots.ToArray());
                        if (slot < 0 || slot >= prizes.Count) return;
                        card = prizes[slot];
                        what = actions[pick] == 3 ? "1 of your Prize cards" : "1 of your opponent's Prize cards";
                        break;
                    }
                }
                var shown = new List<CardInstance> { card };
                ctx.Engine.NoteLookedAt(shown, what, ctx.Player);
                ctx.Engine.ShowCards(ctx.Player, shown, Peek, what, ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " used Peek to look at a card (" + labels[pick] + ").");
            }
        }

        // ================================================================== Attacks

        /// <summary>Minimize: attack damage to Clefable is reduced by 20 (after W/R) during the opponent's next turn.</summary>
        internal sealed class MinimizeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Minimize") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var fx = new TakeLessEffect(me.Name + ": attacks do 20 less damage to it during the opponent's next turn (Minimize)", ctx.Player, me.Uid, -1, ctx.Turn + 1, 20);
                fx.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>Pounce / Snivel: attacks from the Defending Pokémon do N less damage to this Pokémon during the opponent's next turn.</summary>
        internal sealed class ReduceFromDefendingMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public ReduceFromDefendingMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                var fx = new TakeLessEffect(defending.Name + "'s attacks do " + amount + " less damage to " + me.Name + " during its next turn (" + attack + ")",
                    ctx.Player, me.Uid, defending.Uid, ctx.Turn + 1, amount);
                fx.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>Tail Wag / Leer: flip a coin; heads, the Defending Pokémon can't attack this Pokémon during the opponent's next turn.</summary>
        internal sealed class CantAttackThisMon : Mon
        {
            private readonly string attack;

            public CantAttackThisMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!heads || !ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                var fx = new CantAttackThisEffect(defending.Name + " can't attack " + me.Name + " during its next turn (" + attack + ")", ctx.Player, defending.Uid, me.Uid, ctx.Turn + 1);
                fx.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>Chain Lightning: if the Defending Pokémon isn't Colorless, 10 damage to each Benched Pokémon (both sides) of its type (no W/R).</summary>
        private sealed class ChainLightningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chain Lightning") return false;
                var defending = ctx.Defending;
                var types = defending == null ? new List<EnergyType>() : ctx.Engine.TypesOf(defending).Where(t => t != EnergyType.Colorless).ToList();
                ctx.DamageDefending(atk.Damage);
                if (types.Count == 0) return true;
                foreach (var mon in ctx.Opp.Bench.Concat(ctx.Me.Bench).ToList())
                    if (ctx.InPlay(mon) && types.Any(t => ctx.Engine.IsType(mon, t))) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Boyfriends: 20 more damage for each Nidoking you have in play.</summary>
        private sealed class BoyfriendsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Boyfriends") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.MyPokemon(m => m.Name == "Nidoking").Count);
                return true;
            }
        }

        /// <summary>Hurricane: unless it Knocks Out the Defending Pokémon, return it and all cards attached to it to the opponent's hand.</summary>
        private sealed class HurricaneReturnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hurricane") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && !ctx.Engine.IsKnockedOut(defending) && ctx.CanAffect(defending))
                    ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>Swords Dance: during your next turn, Slash's base damage is 60 instead of 30.</summary>
        private sealed class SwordsDanceSlashMon : Mon
        {
            private const string Marker = "Swords Dance";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Swords Dance")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Pokemon.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name == "Slash")
                {
                    int turn;
                    bool danced = ctx.Pokemon.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn;
                    ctx.DamageDefending(danced ? 60 : atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Acid: flip a coin; heads, the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class AcidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Acid") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Mega Drain / Absorb: remove damage counters from this Pokémon equal to half the damage done (after W/R), rounded up to 10.</summary>
        internal sealed class HalfDrainMon : Mon
        {
            private readonly string attack;

            public HalfDrainMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int done = ctx.DamageDefending(atk.Damage);
                int heal = (done / 2 + 9) / 10 * 10;
                if (heal > 0 && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, heal);
                return true;
            }
        }

        /// <summary>Big Eggsplosion: flip a coin for each Energy attached to this Pokémon; 20 damage for each heads.</summary>
        private sealed class BigEggsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Eggsplosion") return false;
                int coins = ctx.EnergyCount(ctx.Pokemon);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>"Search your deck for a Basic Pokémon (matching) and put it onto your Bench"; it can't be used with a full Bench.</summary>
        internal sealed class FamilyMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;

            public FamilyMon(string attack, Func<CardDef, bool> filter, string what)
            {
                this.attack = attack;
                this.filter = filter;
                this.what = what;
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != attack || ctx.BenchSpace > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace <= 0) return true;
                ctx.SearchToBench(d => d.IsBasicPokemon && filter(d), 1, atk.Name + ": put " + what + " from your deck onto your Bench");
                return true;
            }
        }

        /// <summary>Tantrum: flip a coin; tails, this Pokémon is now Confused (after doing damage).</summary>
        private sealed class TantrumSelfMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tantrum") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Ram: 20 damage to itself; then the opponent switches the Defending Pokémon with 1 of their Benched Pokémon (even if Rhydon is Knocked Out).</summary>
        private sealed class RamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ram") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20);
                if (TefPokemon.StillDefending(ctx, defending) && !ctx.Engine.IsKnockedOut(defending) && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Rampage: 10 more damage for each counter on this Pokémon; flip a coin, tails, this Pokémon is now Confused.</summary>
        internal sealed class RampageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rampage") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.Pokemon.DamageCounters);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Pay Day: flip a coin; heads, draw a card.</summary>
        private sealed class PayDayFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pay Day") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads) ctx.Draw(1);
                return true;
            }
        }
    }
}
