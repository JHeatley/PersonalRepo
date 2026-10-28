using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ascended Heroes (ASC): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Erika's Bellsprout, Wurmple, Charmander, Charmeleon, Tepig, Snorunt, Snom, Pikachu,
    /// Helioptile, Swirlix, Slurpuff, Rolycoly, Galarian Zigzagoon, Galarian Linoone, Vullaby, Dratini, Skitty,
    /// Larry's Staravia.
    /// </summary>
    public static class CardsASC
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("ASC-1", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("ASC-2", new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned));
            r.RegisterByKey("ASC-3", new ErikasVileplumeExASC());
            r.RegisterByKey("ASC-5", new LockAfterAttackMon("Leafy Cyclone", false));
            r.RegisterByKey("ASC-6", new ErikasVictreebelASC());
            r.RegisterByKey("ASC-7", new ErikasTangelaASC());
            r.RegisterByKey("ASC-12", new SilcoonASC());
            r.RegisterByKey("ASC-13", new BeautiflyASC());
            r.RegisterByKey("ASC-14", new SwitchSelfMon("Trading Places", false));
            r.RegisterByKey("ASC-15", new DustoxASC());
            r.RegisterByKey("ASC-25", new CardsMEG.EnergyInPlayBonusMon("Flare Fall", EnergyType.Fire, 4, 90));
            r.RegisterByKey("ASC-27", new NumelASC());
            r.RegisterByKey("ASC-28", new CameruptASC());
            r.RegisterByKey("ASC-30", new ConditionAttackMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("ASC-41", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("ASC-42", new FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("ASC-47", new MegaFroslassExASC());
            r.RegisterByKey("ASC-48", new RegiExASC(EnergyType.Water));
            r.RegisterByKey("ASC-49", new CallForFamilyMon());
            r.RegisterByKey("ASC-50", new CardsBLK.NoAttackNextTurnMon("Sheer Cold"));
            r.RegisterByKey("ASC-51", new NsVanilluxeASC());
            r.RegisterByKey("ASC-53", new FrosmothASC());
            r.RegisterByKey("ASC-54", new ComboMon(new CardsMEG.SnipeBenchMon("Ice Shot", 20), new LockAfterAttackMon("Frosty Typhoon", true)));
            r.RegisterByKey("ASC-56", new ComboMon(new FlipBonusMon("Quick Blow", 50), new SelfDiscardTypeMon("Strong Volt", EnergyType.Lightning, 1)));
            r.RegisterByKey("ASC-58", new CardsMEG.FlipUntilTailsBonusMon("Hundred-Hitting Ball", 100));
            r.RegisterByKey("ASC-61", new MegaEelektrossExASC());
            r.RegisterByKey("ASC-64", new HelioliskASC());
            r.RegisterByKey("ASC-67", new TapuKokoASC());
            r.RegisterByKey("ASC-68", new CounterattackMon("Counterattack Quills", 3, new DrawAttackMon("Spiky Thunder", 2)));
            r.RegisterByKey("ASC-73", new ExBonusMon("Hadron Spark", 120));
            r.RegisterByKey("ASC-77", new FlipBonusMon("Focused Wish", 20));
            r.RegisterByKey("ASC-78", new TeamRocketsExeggutorASC());
            r.RegisterByKey("ASC-83", new DodgeMon("Hide", false));
            r.RegisterByKey("ASC-84", new AzumarillExASC());
            r.RegisterByKey("ASC-85", new MisdreavusASC());
            r.RegisterByKey("ASC-86", new MismagiusASC());
            r.RegisterByKey("ASC-92", new RotomASC());
            r.RegisterByKey("ASC-100", new TeamRocketsDiglettASC());
            r.RegisterByKey("ASC-101", new TeamRocketsDugtrioASC());
            r.RegisterByKey("ASC-102", new CardsDAA1.DeepSeaSwirlMon("Spin and Draw", 6));
            r.RegisterByKey("ASC-103", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("ASC-104", new MedichamASC());
            r.RegisterByKey("ASC-108", new RecoilMon("Megaton Fall", 30));
            r.RegisterByKey("ASC-114", new ComboMon(new NoRetreatMon("Big Bite"), new SelfDamagedBonusMon("Flopping Trap", 100)));
            r.RegisterByKey("ASC-115", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("ASC-116", new MegaHawluchaExASC());
            r.RegisterByKey("ASC-117", new CarbinkASC());
            r.RegisterByKey("ASC-119", new SelfGuardMon("Guard Press", 20));
            r.RegisterByKey("ASC-120", new CoalossalASC());
            r.RegisterByKey("ASC-122", new CardsGRI3.SettleTheScoreMon("Settle the Score", 60));
            r.RegisterByKey("ASC-132", new ComboMon(new CounterMultiplierMon("Scarring Shout"), new CardsTWM1.SelfDiscardMon("Punk Smash", 1)));
            r.RegisterByKey("ASC-134", new RandomDiscardMon("Knock Off"));
            r.RegisterByKey("ASC-135", new CounterattackMon("Counterattacking Crest", 5, new OutlawLegMon()));
            r.RegisterByKey("ASC-139", new ComboMon(new CardsMEG.SnipeMon("Bone Shot", 50), new RandomDiscardMon("Vulture Claw")));
            r.RegisterByKey("ASC-140", new PangoroASC());
            r.RegisterByKey("ASC-141", new ComboMon(new DrawAttackMon("Filch", 2), new LockAfterAttackMon("Knuckle Impact", false)));
            r.RegisterByKey("ASC-143", new PecharuntASC());
            r.RegisterByKey("ASC-144", new CallForFamilyMon());
            r.RegisterByKey("ASC-145", new RegiExASC(EnergyType.Metal));
            r.RegisterByKey("ASC-146", new PushDownMon("Push Down"));
            r.RegisterByKey("ASC-147", new DrawAttackMon("Rapid Draw", 2));
            r.RegisterByKey("ASC-148", new KingambitASC());
            r.RegisterByKey("ASC-149", new ComboMon(new ParalyzeFlipMon("Stun Needle"), new NextTurnBoostMon("Spiky Rolling", 80)));
            r.RegisterByKey("ASC-151", new DragonairASC());
            r.RegisterByKey("ASC-153", new CardsBKP2.GaleThrustMon("Breakthrough Assault", 90));
            r.RegisterByKey("ASC-156", new SearchAttackMon("Knickknack Carrying", d => d.IsTool, 1));
            r.RegisterByKey("ASC-157", new ComboMon(new DodgeMon("Agility", false), new ToolBonusMon("Enhanced Blade", 70)));
            r.RegisterByKey("ASC-163", new FlipBonusMon("Rising Lunge", 20));
            r.RegisterByKey("ASC-164", new FlipPerEnergyMon("Work Rush"));
            r.RegisterByKey("ASC-166", new DelcattyASC());
            r.RegisterByKey("ASC-167", new ComboMon(new DrawAttackMon("Spike Draw", 2), new RecoilMon("Wild Scissors", 30)));
            r.RegisterByKey("ASC-168", new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("ASC-170", new LarrysStaraptorASC());
            r.RegisterByKey("ASC-172", new MegaAudinoExASC());
            r.RegisterByKey("ASC-173", new CardsPFL.DamagedBonusMon("Peck the Wound", 80));
            r.RegisterByKey("ASC-174", new ComboMon(new NoRetreatMon("Clutch"), new RecoilMon("Brave Bird", 30)));
            r.RegisterByKey("ASC-175", new LarrysKomalaASC());
            r.RegisterByKey("ASC-176", new LockAfterAttackMon("Dragon Strike", true));
            r.RegisterByKey("ASC-251", new CardsTWM1.HealSelfMon("Magical Leaf", 30));

            r.RegisterByName("Anthea & Concordia", new AntheaAndConcordia());
            r.RegisterByName("Canari", new Canari());
            r.RegisterByName("Light Ball", new LightBall());
            r.RegisterByName("Team Rocket's Hypnotizer", new TeamRocketsHypnotizer());
            r.RegisterByName("Thick Scale", new ThickScale());
            r.RegisterByName("Urbain", new Urbain());
            r.RegisterByName("Waitress", new Waitress());
        }

        // ================================================================== Shared helpers

        /// <summary>"If this Pokémon is in the Active Spot and is damaged by an attack from your opponent's Pokémon (even if it is Knocked Out), place N damage counters on the Attacking Pokémon." Attacks come from another script.</summary>
        public sealed class CounterattackMon : Mon
        {
            private readonly string ability;
            private readonly int counters;
            private readonly Mon attacks;

            public CounterattackMon(string ability, int counters, Mon attacks)
            {
                this.ability = ability;
                this.counters = counters;
                this.attacks = attacks;
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, ability);
                ctx.Engine.PlaceDamageCounters(attacker, counters, ability);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                return attacks != null && attacks.HandleAttack(ctx, atk);
            }
        }

        /// <summary>"During your next turn, this Pokémon's [attack] does N more damage" (Mochi Rush) / "if this Pokémon used [attack] during your last turn" (Spiky Rolling).</summary>
        public sealed class NextTurnBoostMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public NextTurnBoostMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int uid = ctx.Pokemon.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>()
                    .Where(e => e.TargetUid == uid && e.AttackName == attack).Sum(e => e.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                if (ctx.InPlay(ctx.Pokemon))
                {
                    var boost = new AttackBoostEffect(ctx.Pokemon.Name + "'s " + attack + " does " + bonus + " more damage during its next turn", ctx.Player, uid, ctx.Turn + 2, attack, bonus);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                }
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, this Pokémon takes N less damage from attacks (after Weakness and Resistance)."</summary>
        public sealed class SelfGuardMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SelfGuardMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(amount);
                return true;
            }
        }

        /// <summary>"Discard N [T] Energy from this Pokémon."</summary>
        public sealed class SelfDiscardTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;

            public SelfDiscardTypeMon(string attack, EnergyType type, int count)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, count, atk.Name + ": discard " + count + " " + type + " Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, type));
                return true;
            }
        }

        /// <summary>"If this Pokémon has any damage counters on it, this attack does N more damage."</summary>
        public sealed class SelfDamagedBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public SelfDamagedBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.DamageCounters > 0 ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If your opponent's Active Pokémon is a Pokémon ex, this attack does N more damage."</summary>
        public sealed class ExBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public ExBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var d = ctx.Defending;
                bool ex = d != null && (d.Def.IsEx || d.Def.IsMegaEx);
                ctx.DamageDefending(atk.Damage + (ex ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If this Pokémon has a Pokémon Tool attached, this attack does N more damage."</summary>
        public sealed class ToolBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public ToolBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Tool != null ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Discard a random card from your opponent's hand."</summary>
        public sealed class RandomDiscardMon : Mon
        {
            private readonly string attack;

            public RandomDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                DiscardRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        public static void DiscardRandomFromOpponentHand(EffectContext ctx, int count)
        {
            for (int i = 0; i < count && ctx.Opp.Hand.Count > 0; i++)
            {
                var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            }
        }

        /// <summary>"Flip a coin. If heads, discard an Energy from your opponent's Active Pokémon."</summary>
        public sealed class FlipDiscardDefendingEnergyMon : Mon
        {
            private readonly string attack;

            public FlipDiscardDefendingEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, atk.Name + ": discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>"This attack does N damage for each damage counter on your opponent's Active Pokémon."</summary>
        public sealed class CounterMultiplierMon : Mon
        {
            private readonly string attack;

            public CounterMultiplierMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * (ctx.Defending == null ? 0 : ctx.Defending.DamageCounters));
                return true;
            }
        }

        /// <summary>"Flip a coin for each Energy attached to this Pokémon. This attack does N damage for each heads."</summary>
        public sealed class FlipPerEnergyMon : Mon
        {
            private readonly string attack;

            public FlipPerEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int coins = ctx.EnergyCount(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }

        /// <summary>"Switch out your opponent's Active Pokémon to the Bench. (Your opponent chooses the new Active Pokémon.)"</summary>
        public sealed class PushDownMon : Mon
        {
            private readonly string attack;

            public PushDownMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Both Special Conditions of "Asleep and Poisoned" attacks (Bloom Powder, Twilight Poison).</summary>
        private static bool SleepAndPoison(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (TefPokemon.StillDefending(ctx, defending))
            {
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }
            return true;
        }

        private static bool IsNamedGroup(PokemonInPlay m, string owner) { return m.Name.StartsWith(owner + "'s "); }

        // ================================================================== Grass

        /// <summary>Lovely Fragrance: once during your turn, heal 30 from each of your Pokémon. Bloom Powder: Asleep and Poisoned.</summary>
        private sealed class ErikasVileplumeExASC : Mon
        {
            protected override string Activated { get { return "Lovely Fragrance"; } }
            protected override bool AbilityReady(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var m in ctx.MyPokemon())
                    if (m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)) ctx.Heal(m, 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                return atk.Name == "Bloom Powder" && SleepAndPoison(ctx, atk);
            }
        }

        /// <summary>Flower Garden Rondo: 40 for each of your Erika's Pokémon in play.</summary>
        private sealed class ErikasVictreebelASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flower Garden Rondo") return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => IsNamedGroup(m, "Erika")).Count);
                return true;
            }
        }

        /// <summary>Gathering of Blossoms: once during your turn, search your deck for an Erika's Pokémon. Bind: flip to Paralyze.</summary>
        private sealed class ErikasTangelaASC : Mon
        {
            private static readonly Mon Bind = new ParalyzeFlipMon("Bind");

            protected override string Activated { get { return "Gathering of Blossoms"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.Name.StartsWith("Erika's "), 1, "Gathering of Blossoms: choose an Erika's Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bind.HandleAttack(ctx, atk); }
        }

        /// <summary>Multiplying Cocoon: when it evolves from the hand, put a Silcoon or Cascoon from your deck onto your Bench.</summary>
        private sealed class SilcoonASC : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0;
                if (!TriggerOnce(ctx, "Multiplying Cocoon", ready, "Multiplying Cocoon: search your deck for a Silcoon or a Cascoon and put it onto your Bench?")) return;
                var found = ctx.SearchMyDeck(d => d.Name == "Silcoon" || d.Name == "Cascoon", 1, "Multiplying Cocoon: choose a Silcoon or a Cascoon");
                foreach (var c in found) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Stun Spore: flip to Paralyze. Energy Straw: 80 for each Energy card in the opponent's hand.</summary>
        private sealed class BeautiflyASC : Mon
        {
            private static readonly Mon StunSpore = new ParalyzeFlipMon("Stun Spore");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Straw")
                {
                    CardsTWM1.RevealOpponentHand(ctx);
                    ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count(c => c.Def.IsEnergy));
                    return true;
                }
                return StunSpore.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Boisterous Wind: once during your turn, flip; heads, an Energy on the opponent's Active Pokémon goes to their hand. Twilight Poison: Asleep and Poisoned.</summary>
        private sealed class DustoxASC : Mon
        {
            protected override string Activated { get { return "Boisterous Wind"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var d = ctx.Opp.Active;
                if (!ctx.Flip() || d == null || d.Energy.Count == 0 || !ctx.CanAffect(d)) return;
                var e = ctx.ChooseCards(d.Energy.ToList(), 1, 1, "Boisterous Wind: put which Energy into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(d, e);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                return atk.Name == "Twilight Poison" && SleepAndPoison(ctx, atk);
            }
        }

        // ================================================================== Fire

        /// <summary>Incandescent Body: when it's damaged by an attack in the Active Spot (even if Knocked Out), the Attacking Pokémon is Burned.</summary>
        private sealed class NumelASC : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, SpecialCondition.Burned)) Announce(ctx, "Incandescent Body");
            }
        }

        /// <summary>Roasting Burn: nothing unless the opponent's Active Pokémon is Burned. Power Stomp: discard 2 Energy.</summary>
        private sealed class CameruptASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Roasting Burn")
                {
                    if (ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Burned)) ctx.DamageDefending(atk.Damage);
                    else Nothing(ctx);
                    return true;
                }
                if (atk.Name == "Power Stomp")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        /// <summary>Resentful Refrain: 50 for each card in the opponent's hand. Absolute Snow: Asleep.</summary>
        private sealed class MegaFroslassExASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Resentful Refrain")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count);
                    return true;
                }
                if (atk.Name == "Absolute Snow")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Regice ex / Registeel ex: Regi Charge attaches up to 2 Basic Energy of its type from the discard pile to it; Ice Prison (discard 2, Paralyzed); Protecting Steel (takes 50 less next turn).</summary>
        private sealed class RegiExASC : Mon
        {
            private readonly EnergyType type;

            public RegiExASC(EnergyType type) { this.type = type; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Regi Charge")
                {
                    var pile = ctx.BasicEnergyInDiscard(d => CardsSSP1.IsBasicOf(d, type));
                    if (pile.Count > 0 && ctx.InPlay(me))
                        ctx.AttachCardsAnyWay(pile, 2, (c, m) => m == me, "Regi Charge: attach Basic " + type + " Energy to " + me.Name);
                    return true;
                }
                if (atk.Name == "Ice Prison")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Protecting Steel")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.SelfTakesLessNextTurn(50);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Snow Coating: double the damage counters on each of the opponent's Pokémon. Blizzard: 10 to each of their Benched Pokémon.</summary>
        private sealed class NsVanilluxeASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Snow Coating")
                {
                    foreach (var m in ctx.OpponentPokemon(m => m.DamageCounters > 0))
                        if (ctx.CanAffect(m)) ctx.PlaceCounters(m, m.DamageCounters);
                    return true;
                }
                if (atk.Name == "Blizzard")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var m in ctx.Opp.Bench.ToList()) ctx.DamageTarget(m, 10);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Alluring Wings: once during your turn, while Active, each player draws a card. Cold Cyclone: move a Water Energy from it to the Bench.</summary>
        private sealed class FrosmothASC : Mon
        {
            protected override string Activated { get { return "Alluring Wings"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && (ctx.Me.Deck.Count > 0 || ctx.Opp.Deck.Count > 0); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Draw(1);
                ctx.Engine.DrawCards(ctx.Opponent, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cold Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var water = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                var energy = water.Select(e => e.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Cold Cyclone: move which Water Energy?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Cold Cyclone: move it to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, energy);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Split Bomb: 60 to each of 2 of the opponent's Pokémon. Disaster Shock: may discard 2 Lightning Energy to Paralyze.</summary>
        private sealed class MegaEelektrossExASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Split Bomb")
                {
                    CardsDRI1.DamageTwoOpponents(ctx, 60, atk.Name);
                    return true;
                }
                if (atk.Name == "Disaster Shock")
                {
                    var me = ctx.Pokemon;
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var lightning = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList();
                    if (lightning.Count < 2 || !ctx.AskYesNo("Disaster Shock: discard 2 Lightning Energy from " + me.Name + " to Paralyze your opponent's Active Pokémon?")) return true;
                    ctx.DiscardEnergyFrom(me, 2, "Disaster Shock: discard 2 Lightning Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning));
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Frilled Generator: once during your turn, if you played Canari this turn, attach up to 2 Basic Lightning Energy from your deck to it. Powerful Bolt: a coin per Energy, 70 per heads.</summary>
        private sealed class HelioliskASC : Mon
        {
            private static readonly Mon PowerfulBolt = new FlipPerEnergyMon("Powerful Bolt");

            protected override string Activated { get { return "Frilled Generator"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.SupporterNameThisTurn == "Canari" && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { CardsMEG.ChargeSelfMon.ChargeSelf(ctx, EnergyType.Lightning, 2, "Frilled Generator"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PowerfulBolt.HandleAttack(ctx, atk); }
        }

        /// <summary>Fast Flight: usable on your first turn if you go first; discard your hand and draw 5. Thunder Blast: discard 2 Energy.</summary>
        private sealed class TapuKokoASC : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex)
            {
                return ctx.Pokemon != null && ctx.Pokemon.Def.Attacks[attackIndex].Name == "Fast Flight";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fast Flight")
                {
                    foreach (var c in ctx.Me.Hand.ToList()) ctx.Engine.DiscardFromHand(ctx.Player, c);
                    ctx.Draw(5);
                    return true;
                }
                if (atk.Name == "Thunder Blast")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Psychic

        /// <summary>Tri Kinesis: flip 3; all heads, Knock Out 1 of the opponent's Pokémon. Double-Edge: 30 to itself.</summary>
        private sealed class TeamRocketsExeggutorASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tri Kinesis")
                {
                    if (ctx.FlipCoins(3) < 3 || !ctx.Opp.HasPokemonInPlay) return true;
                    var targets = ctx.OpponentPokemon(m => ctx.CanAffect(m));
                    if (targets.Count == 0) return true;
                    var target = ctx.ChoosePokemon(targets, "Tri Kinesis: Knock Out which of your opponent's Pokémon?");
                    if (target != null) ctx.Engine.KnockOut(target, atk.Name);
                    return true;
                }
                if (atk.Name == "Double-Edge")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Bubble Gathering: as often as you like, move an Energy from another of your Pokémon to it. Energized Balloon: +40 per Psychic Energy.</summary>
        private sealed class AzumarillExASC : Mon
        {
            private static readonly Mon Balloon = new CardsMEG.EnergyBonusMon("Energized Balloon", EnergyType.Psychic, 40);

            private static List<PokemonInPlay> Givers(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.Energy.Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Bubble Gathering" && ctx.IsMyTurn && Givers(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing moves as many as they like, then presses Done; bots move one per use.
                RepeatableAbility.Repeat(ctx, () => Givers(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), "Bubble Gathering: move an Energy from which Pokémon?"
                        + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var energy = from.Energy.Select(e => e.Def.Name).Distinct().Count() == 1 ? from.Energy[0]
                        : ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Bubble Gathering: move which Energy?")[0];
                    ctx.Engine.MoveEnergy(from, ctx.Pokemon, energy);
                    return true;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Balloon.HandleAttack(ctx, atk); }
        }

        /// <summary>Ascension: search your deck for a card that evolves from it and evolve it.</summary>
        private sealed class MisdreavusASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ascension") return false;
                CardsSSP1.EvolveFromDeck(ctx, ctx.Pokemon, "Ascension: choose a card that evolves from " + ctx.Pokemon.Name);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Assassin's Magic: if the opponent's Active Pokémon has a Special Condition, 6 damage counters on 1 of their Benched Pokémon.</summary>
        private sealed class MismagiusASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assassin's Magic") return false;
                bool special = ctx.Defending != null && ctx.Defending.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage);
                if (!special) return true;
                var bench = ctx.Opp.Bench.Where(m => ctx.CanAffect(m)).ToList();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Assassin's Magic: put 6 damage counters on which Benched Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 6);
                return true;
            }
        }

        /// <summary>Roto Call: bench any number of Rotom from your deck. Gadget Show: 30 per Tool on your Pokémon.</summary>
        private sealed class RotomASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Roto Call")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => d.Name.Contains("Rotom"), ctx.BenchSpace, "Roto Call: put any number of Rotom onto your Bench");
                    return true;
                }
                if (atk.Name == "Gadget Show")
                {
                    ctx.DamageDefending(atk.Damage * CardsDRI1.ToolsInPlay(ctx));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fighting

        /// <summary>Relentless Burrowing: flip until tails; discard the top card of the opponent's deck for each heads.</summary>
        private sealed class TeamRocketsDiglettASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Relentless Burrowing") return false;
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        /// <summary>Holes: whenever the opponent's Active Pokémon moves to the Bench during their turn, 2 damage counters on it.</summary>
        private sealed class TeamRocketsDugtrioASC : Mon
        {
            public override void OnOpponentPokemonMovedToBench(EffectContext src, int player, PokemonInPlay movedToBench)
            {
                if (player == src.Player || movedToBench == null || !src.Opp.Bench.Contains(movedToBench)) return;
                Announce(src, "Holes");
                src.Engine.PlaceDamageCounters(movedToBench, 2, "Holes");
            }
        }

        /// <summary>Seventh Kick: nothing unless you have exactly 7 cards in your hand.</summary>
        private sealed class MedichamASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seventh Kick") return false;
                if (ctx.Me.Hand.Count == 7) ctx.DamageDefending(atk.Damage);
                else Nothing(ctx);
                return true;
            }
        }

        /// <summary>Tenacious Body: when it would be Knocked Out by attack damage, flip; heads, it stays with 10 HP. Somersault Dive: +140 with a Stadium in play, then discard it.</summary>
        private sealed class MegaHawluchaExASC : Mon
        {
            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                return mon == src.Pokemon && src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Somersault Dive") return false;
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? 140 : 0));
                if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Double Type: it's also a Psychic Pokémon. Counter Jewel: +100 if the opponent has 2 or fewer Prize cards left.</summary>
        private sealed class CarbinkASC : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == src.Pokemon && !types.Contains(EnergyType.Psychic)) types.Add(EnergyType.Psychic);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Counter Jewel") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.Prizes.Count <= 2 ? 100 : 0));
                return true;
            }
        }

        /// <summary>Tar Cannon: 140 to 1 of the opponent's Pokémon, only with 10+ Basic Fighting Energy in your discard pile. Bulky Bump: discard 3 Energy.</summary>
        private sealed class CoalossalASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tar Cannon")
                {
                    if (ctx.BasicEnergyInDiscard(d => CardsSSP1.IsBasicOf(d, EnergyType.Fighting)).Count >= 10) ctx.DamageAnyOpponent(140);
                    else Nothing(ctx);
                    return true;
                }
                if (atk.Name == "Bulky Bump")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 3, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Darkness

        /// <summary>Outlaw Leg: discard a random card from the opponent's hand and the top card of their deck.</summary>
        private sealed class OutlawLegMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Outlaw Leg") return false;
                ctx.DamageDefending(atk.Damage);
                DiscardRandomFromOpponentHand(ctx, 1);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>Master's Punch: +120 if any of your Benched Pancham have damage counters.</summary>
        private sealed class PangoroASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Master's Punch") return false;
                bool hurt = ctx.Me.Bench.Any(m => m.Name == "Pancham" && m.DamageCounters > 0);
                ctx.DamageDefending(atk.Damage + (hurt ? 120 : 0));
                return true;
            }
        }

        /// <summary>Final Chain: when it's Knocked Out by an opponent's attack, search your deck for a card. Mochi Rush: +50 next turn.</summary>
        private sealed class PecharuntASC : Mon
        {
            private static readonly Mon MochiRush = new NextTurnBoostMon("Mochi Rush", 50);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Me.Deck.Count == 0) return;
                Announce(src, "Final Chain");
                src.SearchToHand(d => true, 1, "Final Chain: search your deck for a card", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MochiRush.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Supreme Overlord: its attacks do 30 more to the opponent's Active Pokémon per Prize card the opponent has taken. Double-Edged Slash: 50 to itself.</summary>
        private sealed class KingambitASC : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive) return damage;
                return damage + 30 * src.Engine.PrizesTaken(src.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double-Edged Slash") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(50);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Evolutionary Guidance: once during your turn, with Energy attached, search your deck for an Evolution Pokémon.</summary>
        private sealed class DragonairASC : Mon
        {
            protected override string Activated { get { return "Evolutionary Guidance"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon.Energy.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsEvolution, 1, "Evolutionary Guidance: choose an Evolution Pokémon");
            }
        }

        // ================================================================== Colorless

        /// <summary>Energy Crush: 40 for each Energy attached to all of the opponent's Pokémon.</summary>
        private sealed class DelcattyASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Crush") return false;
                ctx.DamageDefending(atk.Damage * ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m)));
                return true;
            }
        }

        /// <summary>Facade: +100 while Burned or Poisoned. Feathery Strike: discard 2 Energy, also 50 to 1 Benched Pokémon.</summary>
        private sealed class LarrysStaraptorASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Facade")
                {
                    bool hurt = me.Has(SpecialCondition.Burned) || me.Has(SpecialCondition.Poisoned);
                    ctx.DamageDefending(atk.Damage + (hurt ? 100 : 0));
                    return true;
                }
                if (atk.Name == "Feathery Strike")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    ctx.DamageOneBenched(50);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Kaleidowaltz: flip 3; for each heads, up to 2 Basic Energy from your deck to your Pokémon. Ear Force: +80 per Energy on the opponent's Active Pokémon.</summary>
        private sealed class MegaAudinoExASC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Kaleidowaltz")
                {
                    int heads = ctx.FlipCoins(3);
                    if (heads == 0 || ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2 * heads, "Kaleidowaltz: search your deck for up to " + (2 * heads) + " Basic Energy cards");
                    CardsMEG.AttachFromDeckAnyWay(ctx, found, (c, m) => true, "Kaleidowaltz: attach Basic Energy to your Pokémon");
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Ear Force")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Defending == null ? 0 : 80 * ctx.EnergyCount(ctx.Defending)));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Lethargic Charge: once during your turn, from the Bench, attach an Energy card from your hand to your Active Larry's Pokémon. Dozing Draw: it falls Asleep; draw 2.</summary>
        private sealed class LarrysKomalaASC : Mon
        {
            protected override string Activated { get { return "Lethargic Charge"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return ctx.Me.Bench.Contains(ctx.Pokemon) && a != null && IsNamedGroup(a, "Larry") && ctx.Engine.CanAttachFromHand(a)
                       && ctx.Me.Hand.Any(c => c.Def.IsEnergy);
            }

            protected override void Ability(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var card = energy.Select(c => c.Def.Name).Distinct().Count() == 1 ? energy[0]
                    : ctx.ChooseCards(energy, 1, 1, "Lethargic Charge: attach which Energy card to " + a.Name + "?")[0];
                if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, a, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dozing Draw") return false;
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                ctx.Draw(2);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Only with all six of N's named Pokémon in play: this turn, a Knock Out of the opponent's Active Pokémon by your N's Pokémon's attack gives 3 more Prize cards.</summary>
        private sealed class AntheaAndConcordia : CardScript
        {
            private static readonly string[] Needed = { "N's Darmanitan", "N's Zoroark ex", "N's Vanilluxe", "N's Klinklang", "N's Reshiram", "N's Zekrom" };

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var names = new HashSet<string>(ctx.MyPokemon().Select(m => m.Name));
                return Needed.All(names.Contains);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new NsPrizeBonusEffect("Your N's Pokémon take 3 more Prize cards for Knocking Out the opponent's Active Pokémon this turn (Anthea & Concordia)",
                    ctx.Player, ctx.Turn));
            }
        }

        private sealed class NsPrizeBonusEffect : ContinuousEffect
        {
            public NsPrizeBonusEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (taker != Owner || knockedOut.Owner == Owner || !engine.State.IsActive(knockedOut)) return prizes;
                bool byN = knockedOut.HitByAttack && !knockedOut.ForcedKnockOut && knockedOut.HitByAttackerDef != null
                           && knockedOut.HitByAttackerDef.Name.StartsWith("N's ");
                return byN ? prizes + 3 : prizes;
            }
        }

        /// <summary>Discard another card from your hand to search your deck for up to 4 Lightning Pokémon.</summary>
        private sealed class Canari : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var cost = ctx.ChooseCards(others, 1, 1, "Canari: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, cost);
                if (ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(d => d.IsPokemon && d.PokemonType == EnergyType.Lightning, 4, "Canari: choose up to 4 Lightning Pokémon");
            }
        }

        /// <summary>Tool: Pikachu ex's attacks do 50 more to the opponent's Active Pokémon ex.</summary>
        private sealed class LightBall : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || holder.Name != "Pikachu ex" || !info.ToOpponentsActive) return damage;
                return info.Target.Def.IsEx || info.Target.Def.IsMegaEx ? damage + 50 : damage;
            }
        }

        /// <summary>Tool: when the Team Rocket's Pokémon holding it is damaged in the Active Spot by the opponent's attack, the Attacking Pokémon falls Asleep.</summary>
        private sealed class TeamRocketsHypnotizer : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (holder == null || !holder.Def.IsTeamRockets || !holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, SpecialCondition.Asleep))
                    ctx.Log("Team Rocket's Hypnotizer put " + attacker.Name + " to sleep.");
            }
        }

        /// <summary>Tool: a Dragon Pokémon holding it takes 50 less from the opponent's Grass, Fire, Water and Lightning Pokémon.</summary>
        private sealed class ThickScale : CardScript
        {
            private static readonly EnergyType[] Types = { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning };

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || !src.Engine.IsType(holder, EnergyType.Dragon)) return damage;
                return Types.Any(t => src.Engine.IsType(info.Attacker, t)) ? damage - 50 : damage;
            }
        }

        /// <summary>Draw 3 cards.</summary>
        private sealed class Urbain : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>Look at the top 6 cards: attach a Basic Energy from them to 1 of your Pokémon, shuffle the rest back.</summary>
        private sealed class Waitress : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0 && ctx.Me.HasPokemonInPlay)
                {
                    var pick = ctx.ChooseCards(energy, 0, 1, "Waitress: attach a Basic Energy card to 1 of your Pokémon");
                    if (pick.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Waitress: attach " + pick[0].Def.Name + " to which Pokémon?");
                        if (target != null && ctx.Me.Deck.Remove(pick[0])) ctx.Engine.AttachEnergyCard(pick[0], target);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }
    }
}
