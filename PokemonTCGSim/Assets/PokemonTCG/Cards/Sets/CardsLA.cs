using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legends Awakened (LA, 2008; v172): the cards not already in the pool (the others are prints). Unlimited only. Rares and LV.X
    /// here; Uncommons, Commons and Trainers in CardsLAB.cs. Riolu 117 has no card text. The fossils give a Prize card when
    /// Knocked Out.
    /// </summary>
    public static partial class CardsLA
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LA-1", new FormChangeMon("Deoxys", "Form Change", new CardsASR1.PlusMon("Energy Crush", 10, c => c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("LA-2", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"), new DracoMeteorMon()));
            r.RegisterByKey("LA-3", new ComboMon(new DestinyBondMon(), new IcyBreathMon()));
            r.RegisterByKey("LA-4", new ComboMon(new ShadowForceMon(), new CardsASR1.PlusMon("Brutal Edge", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters)));
            r.RegisterByKey("LA-5", new ComboMon(new BurningPoisonMon(), new CardsFST2.ConditionBonusMon("Pester", 40, c => c.Defending != null && c.Defending.Conditions != SpecialCondition.None)));
            r.RegisterByKey("LA-6", new EnergyPullPower("Flash Fire", EnergyType.Fire, new MagmaStormMon()));
            r.RegisterByKey("LA-7", new ComboMon(new AquaStreamMon(), new DragonPumpMon()));
            r.RegisterByKey("LA-8", new LuxrayRivalry());
            r.RegisterByKey("LA-9", new ComboMon(new IceFangMon(), new ChargeThroughMon()));
            r.RegisterByKey("LA-10", new MetagrossMagneticReversal());
            r.RegisterByKey("LA-11", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new CardsASR1.PlusMon("Psychic", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("LA-12", new PolitoedEnthusiasm());
            r.RegisterByKey("LA-13", new ProbopassSteelCoating());
            r.RegisterByKey("LA-14", new RayquazaSpeedGain());
            r.RegisterByKey("LA-15", new RegigigasSlowStart());
            r.RegisterByKey("LA-16", new SpiritombCurseBreath());
            r.RegisterByKey("LA-17", new ComboMon(new SonicWaveMon(), new PursueAndTurnMon()));
            r.RegisterByKey("LA-18", new ArmaldoFossilArmor());
            r.RegisterByKey("LA-19", new AzelfTimeWalk());
            r.RegisterByKey("LA-20", new ComboMon(new HealAllMine("Parallel Gain", 10), new BlendPollenMon()));
            r.RegisterByKey("LA-21", new ComboMon(new DrainDownMon(), new NoRetreatMon("Acid")));
            r.RegisterByKey("LA-22", new ComboMon(new GripthrowMon(), new CardsFST2.ConditionBonusMon("Prop-up Pinchers", 30, c => c.Pokemon.Tools.Concat(c.Pokemon.Attached).Any(t => t.Def.Name.StartsWith("Technical Machine", StringComparison.Ordinal)))));
            r.RegisterByKey("LA-23", new DelcattyAttractingBody());
            r.RegisterByKey("LA-24", new FormChangeMon("Deoxys", "Form Change", new PsychicBoostMon()));
            r.RegisterByKey("LA-25", new FormChangeMon("Deoxys", "Form Change", new PsychicDefenseMon()));
            r.RegisterByKey("LA-26", new FormChangeMon("Deoxys", "Form Change", new CardsCES1.FeintAttackMon("Speed Shot", 30)));
            r.RegisterByKey("LA-27", new DittoDna());
            r.RegisterByKey("LA-28", new ForretressIronShell());
            r.RegisterByKey("LA-29", new ComboMon(new WeatherAttachMon("Drought", EnergyType.Fighting), new MajorEarthquakeMon()));
            r.RegisterByKey("LA-30", new EnergyPullPower("Smelt", EnergyType.Metal, new CardsGEN1.FlipPerEnergyMoreMon("Heavy Metal", EnergyType.Metal, 20)));
            r.RegisterByKey("LA-31", new ComboMon(new OneDesireMon(), new DoomDesireMon()));
            r.RegisterByKey("LA-32", new ComboMon(new WeatherAttachMon("Drizzle", EnergyType.Water), new HighTideMon()));
            r.RegisterByKey("LA-33", new LopunnyHealingTail());
            r.RegisterByKey("LA-34", new MespritPsychicBind());
            r.RegisterByKey("LA-35", new ComboMon(new FocusPunchMon(), new RecoilMon("Submission", 20)));
            r.RegisterByKey("LA-36", new RegiceRegiMove());
            r.RegisterByKey("LA-37", new RegigigasRecoverMechanism());
            r.RegisterByKey("LA-38", new RegirockRegiCycle());
            r.RegisterByKey("LA-39", new RegisteelRegiHeal());
            r.RegisterByKey("LA-40", new ShedinjaResent());
            r.RegisterByKey("LA-41", new TorkoalWhiteSmoke());
            r.RegisterByKey("LA-42", new UnownBang());
            r.RegisterByKey("LA-44", new ComboMon(new BurningScentMon(), new EnergyDissolveMon()));
            r.RegisterByKey("LA-45", new VileplumeEnergyReaction());
            r.RegisterByKey("LA-140", new AzelfLvXPsychicAura());
            r.RegisterByKey("LA-141", new GliscorLvXShootPoison());
            r.RegisterByKey("LA-142", new MagnezoneLvXElectricTrans());
            r.RegisterByKey("LA-143", new ComboMon(new HealAllMine("Healing Look", 30, true), new SupremeBlastMon()));
            r.RegisterByKey("LA-146", new UxieLvXTradeOff());
            RegisterUncommonsAndTrainers(r);
        }

        /// <summary>
        /// Form Change / Temperament (Poké-Power): once during your turn, any Pokémon of that family from your deck (Deoxys, Castform:
        /// any form) takes this one's place with everything on it; this one goes into your deck, shuffled. Only 1 a turn.
        /// </summary>
        internal sealed class FormChangeMon : Mon
        {
            private readonly string family;
            private readonly string power;
            private readonly Mon attacks;

            public FormChangeMon(string family, string power, Mon attacks)
            {
                this.family = family;
                this.power = power;
                this.attacks = attacks;
            }

            private bool InFamily(CardDef d) { return d.IsPokemon && d.Name.StartsWith(family, StringComparison.Ordinal) && !d.IsLevelUp; }

            protected override string Activated { get { return power; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Any(c => InFamily(c.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var found = ctx.SearchMyDeck(InFamily, 1, power + ": choose a " + family, true, 1);
                if (found.Count == 0 || me == null)
                {
                    ctx.Me.Deck.AddRange(found.Where(c => !ctx.Me.Deck.Contains(c)));
                    ctx.ShuffleMyDeck();
                    return;
                }
                var old = me.Top;
                me.Stack[me.Stack.Count - 1] = found[0];
                ctx.Me.Deck.Insert(0, old);
                ctx.ShuffleMyDeck();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched " + old.Def.Name + " with " + found[0].Def.Name + " (" + power + ").");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Remove N damage from each of your (Benched) Pokémon" (Parallel Gain, Healing Look).</summary>
        internal sealed class HealAllMine : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly bool benchOnly;

            public HealAllMine(string attack, int amount, bool benchOnly = false)
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

        /// <summary>Draco Meteor: a coin for each of the opponent's Pokémon; heads, 50 to that Pokémon (no W/R on the Bench).</summary>
        private sealed class DracoMeteorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draco Meteor") return false;
                foreach (var m in ctx.OpponentPokemon().ToList())
                    if (ctx.Flip() && ctx.InPlay(m)) ctx.DamageTarget(m, 50);
                return true;
            }
        }

        /// <summary>Destiny Bond: discard a Psychic Energy; if this Pokémon is Knocked Out by attack damage during the opponent's next turn, the Attacking Pokémon is Knocked Out.</summary>
        private sealed class DestinyBondMon : Mon
        {
            private const string Marker = "Destiny Bond";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Marker) return false;
                var me = ctx.Pokemon;
                if (ctx.DiscardEnergyFrom(me, 1, "Destiny Bond: discard a Psychic Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Psychic)) == 0) return true;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 1;
                return true;
            }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                int turn;
                if (me == null || !byOpponentsAttack || !me.Markers.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                var attacker = src.Engine.AttackingPokemonOf(me);
                if (attacker == null || attacker.Owner == src.Player || src.Engine.IsKnockedOut(attacker)) return;
                Announce(src, "Destiny Bond");
                src.Engine.KnockOut(attacker, "Destiny Bond");
            }

            public override bool ReactsToOwnKnockOutWithoutAbility { get { return true; } }
        }

        /// <summary>Icy Breath: the Defending Pokémon is Asleep; 1 damage counter on each of the opponent's Benched Pokémon.</summary>
        private sealed class IcyBreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Icy Breath") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                foreach (var m in ctx.OpponentBench()) if (ctx.CanAffect(m)) ctx.PlaceCounters(m, 1);
                return true;
            }
        }

        /// <summary>Shadow Force: 20 to 1 of the opponent's Benched Pokémon; heads, prevent all effects of attacks on this Pokémon next turn.</summary>
        private sealed class ShadowForceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Force") return false;
                if (ctx.OpponentBench().Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Shadow Force: do 20 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 20);
                }
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Burning Poison: the Defending Pokémon is Burned or Poisoned (your choice); you may return this Pokémon and its cards to your hand.</summary>
        private sealed class BurningPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Poison") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending))
                    ctx.ApplyCondition(defending, ctx.ChooseOneOption("Burning Poison: the Defending Pokémon is now...", "Burned", "Poisoned") == 0 ? SpecialCondition.Burned : SpecialCondition.Poisoned);
                if (ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Burning Poison: return " + ctx.Pokemon.Name + " and its cards to your hand?")) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Flash Fire / Smelt (Poké-Power): once during your turn, move a [type] Energy from 1 of your other Pokémon to this one. Not with a Special Condition.</summary>
        private sealed class EnergyPullPower : Mon
        {
            private readonly string power;
            private readonly EnergyType type;
            private readonly Mon attacks;

            public EnergyPullPower(string power, EnergyType type, Mon attacks)
            {
                this.power = power;
                this.type = type;
                this.attacks = attacks;
            }

            private List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, type))); }

            protected override string Activated { get { return power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && Donors(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(Donors(ctx), power + ": move a " + type + " Energy from which Pokémon?");
                if (from == null) return;
                var cards = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, type)).ToList();
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, power + ": move which Energy card?")[0];
                ctx.Engine.MoveEnergy(from, ctx.Pokemon, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Magma Storm: discard any basic Energy cards from this Pokémon; 20 more for each.</summary>
        private sealed class MagmaStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Storm") return false;
                var me = ctx.Pokemon;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var picks = basics.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(basics, 0, basics.Count, "Magma Storm: discard which basic Energy cards? (20 more damage each)");
                foreach (var card in picks) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage + 20 * picks.Count);
                return true;
            }
        }

        /// <summary>Aqua Stream: show any Water Energy cards from your discard pile; 10 damage each; they go into your deck, shuffled.</summary>
        private sealed class AquaStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Stream") return false;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Water).ToList();
                var picks = pile.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(pile, 0, pile.Count, "Aqua Stream: choose Water Energy cards (10 damage each)");
                ctx.DamageDefending(atk.Damage * picks.Count);
                if (picks.Count > 0)
                {
                    ctx.DiscardToDeck(picks);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Dragon Pump: you may discard 2 cards from your hand for 20 more and 20 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class DragonPumpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Pump") return false;
                bool pump = ctx.Me.Hand.Count >= 2 && ctx.AskYesNo("Dragon Pump: discard 2 cards from your hand for more damage?");
                if (pump) foreach (var card in ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Dragon Pump: discard 2 cards")) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage + (pump ? 20 : 0));
                if (pump && ctx.OpponentBench().Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Dragon Pump: do 20 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 20);
                }
                return true;
            }
        }

        /// <summary>Rivalry (Poké-Body): with an opponent's Pokémon LV.X in play, Luxray's attacks do 50 more to the Active Pokémon (before W/R). Plasma, Shock Bolt.</summary>
        private sealed class LuxrayRivalry : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Plasma", EnergyType.Lightning), new CardsTEU2.DiscardAllTypeMon("Shock Bolt", EnergyType.Lightning));

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.OpponentPokemon().Any(m => m.Def.IsLevelUp) ? damage + 50 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Ice Fang: heads, the Defending Pokémon is Paralyzed and an Energy card on it is discarded.</summary>
        private sealed class IceFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Fang") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending)) ctx.DiscardEnergyFrom(defending, 1, "Ice Fang: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Charge Through: you may do 40 more and 40 to 1 of the opponent's Benched Pokémon; if you do, 40 to itself.</summary>
        private sealed class ChargeThroughMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charge Through") return false;
                bool more = ctx.AskYesNo("Charge Through: do 40 more damage and 40 to a Benched Pokémon (and 40 to " + ctx.Pokemon.Name + ")?");
                ctx.DamageDefending(atk.Damage + (more ? 40 : 0));
                if (!more) return true;
                if (ctx.OpponentBench().Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Charge Through: do 40 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 40);
                }
                ctx.DamageSelf(40);
                return true;
            }
        }

        /// <summary>Magnetic Reversal (Poké-Power): once during your turn, heads switches in 1 of the opponent's Benched Pokémon. Not with a Special Condition. Extra Comet Punch: base 100 during your next turn.</summary>
        private sealed class MetagrossMagneticReversal : Mon
        {
            private const string Marker = "Extra Comet Punch";

            protected override string Activated { get { return "Magnetic Reversal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.GustOpponent("Magnetic Reversal: switch in which Benched Pokémon?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Marker) return false;
                var me = ctx.Pokemon;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 100 : atk.Damage);
                if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                return true;
            }
        }

        /// <summary>Enthusiasm (Poké-Body): with Poliwag, Poliwhirl and Poliwrath in play, Politoed's attacks do 60 more to the Defending Pokémon (before W/R). Frog Hop.</summary>
        private sealed class PolitoedEnthusiasm : Mon
        {
            private static readonly Mon Hop = new FlipBonusMon("Frog Hop", 40);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                bool family = new[] { "Poliwag", "Poliwhirl", "Poliwrath" }.All(n => src.MyPokemon(m => m.Name == n).Count > 0);
                return family ? damage + 60 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hop.HandleAttack(ctx, atk); }
        }

        /// <summary>Steel Coating (Poké-Body): attack damage to Probopass is 10 less per Metal Energy on it, no more than 20 (after W/R). Metal Bomber.</summary>
        private sealed class ProbopassSteelCoating : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - Math.Min(20, 10 * src.Engine.EnergyCountOfType(me, EnergyType.Metal));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Bomber") return false;
                ctx.DamageDefending(atk.Damage);
                int max = Math.Min(ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal), ctx.OpponentBench().Count);
                if (max == 0) return true;
                foreach (var m in ctx.ChoosePokemonMany(ctx.OpponentBench(), 1, max, "Metal Bomber: do 20 damage to which Benched Pokémon?")) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Speed Gain (Poké-Power): once during your turn, flip until tails; for each heads, attach a basic Fire or Lightning Energy card from your discard pile to Rayquaza. Not with a Special Condition. Sky Judgement.</summary>
        private sealed class RayquazaSpeedGain : Mon
        {
            private static readonly Mon Judgement = new CardsTWM1.SelfDiscardMon("Sky Judgement", 99);

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire || d.EnergyProvides == EnergyType.Lightning); }

            protected override string Activated { get { return "Speed Gain"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && Pile(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < heads; i++)
                {
                    var pile = Pile(ctx);
                    if (pile.Count == 0) break;
                    var card = ctx.ChooseCards(pile, 1, 1, "Speed Gain: attach which basic Fire or Lightning Energy card?")[0];
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, ctx.Pokemon) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Judgement.HandleAttack(ctx, atk); }
        }

        /// <summary>Slow Start (Poké-Body): Regigigas can't attack until the opponent has 3 or fewer Prize cards left. Crush Grip: base 40 against a damaged Pokémon.</summary>
        private sealed class RegigigasSlowStart : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex) { return ctx.Opp.Prizes.Count <= 3 || !ctx.Engine.HasAbilityNamed(ctx.Pokemon, "Slow Start"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crush Grip") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.Defending.DamageCounters > 0 ? 40 : atk.Damage);
                return true;
            }
        }

        /// <summary>Curse Breath (Poké-Power): when you put Spiritomb from your hand onto your Bench, 1 damage counter on every Pokémon that already has damage counters (both players'); 1 Curse Breath a turn. Transfer Pain.</summary>
        private sealed class SpiritombCurseBreath : Mon
        {
            private static readonly Mon Pain = new TransferPainMon();

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool any = ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Any(m => m.DamageCounters > 0);
                if (ctx.NamedLimitUsedThisTurn("Curse Breath") || !TriggerOnce(ctx, "Curse Breath", any, "Curse Breath: put 1 damage counter on each damaged Pokémon?")) return;
                ctx.MarkNamedLimit("Curse Breath");
                foreach (var m in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(p => p.DamageCounters > 0).ToList())
                    if (m.Owner == ctx.Player || ctx.CanAffect(m)) ctx.Engine.PlaceDamageCounters(m, 1, "Curse Breath");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pain.HandleAttack(ctx, atk); }
        }

        /// <summary>Transfer Pain: move 1 damage counter from 1 of your Pokémon to 1 of the opponent's.</summary>
        private sealed class TransferPainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Transfer Pain") return false;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var from = ctx.ChoosePokemon(hurt, "Transfer Pain: move a damage counter from which of your Pokémon?");
                var to = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Transfer Pain: move it to which of your opponent's Pokémon?");
                if (from != null && to != null && ctx.CanAffect(to)) ctx.Engine.MoveDamageCounters(from, to, 1, "Transfer Pain");
                return true;
            }
        }

        /// <summary>Sonic Wave: 10 to each of the opponent's Benched Pokémon; then they switch their Active Pokémon.</summary>
        private sealed class SonicWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Wave") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.OpponentBench().ToList()) ctx.DamageTarget(m, 10);
                ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Pursue and Turn: with 2 or more damage counters on the Defending Pokémon, 20 more and switch this Pokémon with a Benched one.</summary>
        private sealed class PursueAndTurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pursue and Turn") return false;
                bool hurt = ctx.Defending != null && ctx.Defending.DamageCounters >= 2;
                ctx.DamageDefending(atk.Damage + (hurt ? 20 : 0));
                if (hurt && ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Fossil Armor (Poké-Body): attack damage of 60 or less to Armaldo is prevented. Crush Claw: next turn, attacks that damage the Defending Pokémon do 40 more.</summary>
        private sealed class ArmaldoFossilArmor : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && damage <= 60 && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && info.Attacker != null;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crush Claw") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsSW.SilverWindEffect(defending.Name + " takes 40 more damage from attacks during the next turn (Crush Claw)", ctx.Player, defending.Uid, ctx.Turn + 2, 40));
                return true;
            }
        }

        /// <summary>
        /// Time Walk (Poké-Power): when you put Azelf from your hand onto your Bench, look at your face-down Prize cards; you may take
        /// a Pokémon there into your hand, then put a card from your hand face down as a Prize card. Lock Up.
        /// </summary>
        private sealed class AzelfTimeWalk : Mon
        {
            private static readonly Mon Lock = new NoRetreatMon("Lock Up");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var faceDown = ctx.Me.Prizes.Where(c => !ctx.Me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (!TriggerOnce(ctx, "Time Walk", faceDown.Count > 0, "Time Walk: look at your face-down Prize cards?")) return;
                ctx.Engine.ShowCards(ctx.Player, faceDown, "Time Walk", "Time Walk: your face-down Prize cards", ctx.Source);
                var pokemon = faceDown.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0 || !ctx.AskYesNo("Time Walk: take a Pokémon from your Prize cards into your hand?")) return;
                var card = ctx.ChooseCards(pokemon, 1, 1, "Time Walk: take which Pokémon?")[0];
                ctx.Me.Prizes.Remove(card);
                ctx.Me.Hand.Add(card);
                var back = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Time Walk: put which card from your hand face down as a Prize card?")[0];
                ctx.Me.Hand.Remove(back);
                ctx.Me.Prizes.Add(back);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lock.HandleAttack(ctx, atk); }
        }

        /// <summary>Blend Pollen: 20 more for each Vileplume and Bellossom you have in play; heads, Burned.</summary>
        private sealed class BlendPollenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blend Pollen") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 20 * ctx.MyPokemon(m => m.Name == "Vileplume" || m.Name == "Bellossom").Count);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Drain Down: you may switch in 1 of the opponent's Benched Pokémon first; if this Knocks Out the Defending Pokémon, you may remove all damage counters from this Pokémon.</summary>
        private sealed class DrainDownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drain Down") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Drain Down: switch in 1 of your opponent's Benched Pokémon first?")) ctx.GustOpponent("Drain Down: switch in which Benched Pokémon?");
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                bool knocked = defending != null && (!ctx.InPlay(defending) || ctx.Engine.IsKnockedOut(defending));
                if (knocked && ctx.InPlay(me) && me.DamageCounters > 0 && ctx.AskYesNo("Drain Down: remove all damage counters from " + me.Name + "?")) ctx.Heal(me, me.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Gripthrow: heads, the opponent returns the Defending Pokémon and its cards to their hand.</summary>
        private sealed class GripthrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gripthrow") return false;
                var defending = ctx.Defending;
                if (ctx.Flip() && defending != null && ctx.CanAffect(defending)) ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>Attracting Body (Poké-Body): while Active, when damaged by the opponent's attack (even if Knocked Out), heads Confuses the Attacking Pokémon. Energy Assist.</summary>
        private sealed class DelcattyAttractingBody : Mon
        {
            private static readonly Mon Assist = new EnergyToBenchFromDiscardMon("Energy Assist");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                Announce(ctx, "Attracting Body");
                if (ctx.Flip()) ctx.ApplyCondition(attacker, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Assist.HandleAttack(ctx, atk); }
        }

        /// <summary>"Search your discard pile for a basic Energy card and attach it to 1 of your Benched Pokémon" (Energy Assist).</summary>
        internal sealed class EnergyToBenchFromDiscardMon : Mon
        {
            private readonly string attack;

            public EnergyToBenchFromDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, attack + ": attach which basic Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), attack + ": attach it to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Psychic Boost: during your next turn, its base damage is 20.</summary>
        private sealed class PsychicBoostMon : Mon
        {
            private const string Marker = "Psychic Boost";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Marker) return false;
                var me = ctx.Pokemon;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 20 : atk.Damage);
                if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                return true;
            }
        }

        /// <summary>Psychic Defense: during the opponent's next turn, prevent all effects of attacks on Deoxys, and attack damage to it is 20 less.</summary>
        private sealed class PsychicDefenseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Defense") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                ctx.AddEffect(new CardsSW.NoAttackEffectsEffect(me.Name + " is protected from attack effects next turn (Psychic Defense)", ctx.Player, me.Uid, ctx.Turn + 1));
                ctx.SelfTakesLessNextTurn(20);
                return true;
            }
        }

        /// <summary>
        /// Ditto DNA (Poké-Body): while Ditto is your Active Pokémon, its maximum HP is the opponent's Active Pokémon's, and it can use
        /// that Pokémon's attacks (paying for them).
        /// </summary>
        private sealed class DittoDna : Mon
        {
            [ThreadStatic] private static bool busy;

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                var me = src.Pokemon;
                var other = src.Opp.Active;
                if (busy || me == null || mon != me || !src.IsActive(me) || other == null || other.Name == "Ditto") return hp;
                busy = true;
                try { return src.Engine.MaxHp(other); }
                finally { busy = false; }
            }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var me = src.Pokemon;
                var other = src.Opp.Active;
                if (me == null || !src.IsActive(me) || other == null) yield break;
                for (int i = 0; i < other.Def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(other.Def, i);
            }
        }

        /// <summary>Iron Shell (Poké-Body): when you attach a basic Energy card from your hand to Forretress, you may flip; tails, 2 damage counters on every other Pokémon (both players'). Explosion.</summary>
        private sealed class ForretressIronShell : Mon
        {
            private static readonly Mon Boom = new RecoilMon("Explosion", 40);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (target == null || target != me || energy == null || !energy.Def.IsBasicEnergy) return;
                if (!src.AskYesNo("Iron Shell: flip a coin? (Tails: 2 damage counters on every other Pokémon)")) return;
                Announce(src, "Iron Shell");
                if (src.Flip()) return;
                foreach (var m in src.MyPokemon(p => p != me).Concat(src.OpponentPokemon()).ToList())
                    if (m.Owner == src.Player || src.CanAffect(m)) src.Engine.PlaceDamageCounters(m, 2, "Iron Shell");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Boom.HandleAttack(ctx, atk); }
        }

        /// <summary>Drought / Drizzle: attach up to 2 basic [type] Energy cards from your hand to 1 of your Pokémon (each counts as from the hand).</summary>
        private sealed class WeatherAttachMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public WeatherAttachMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var cards = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == type).ToList();
                if (cards.Count == 0) return true;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(2, cards.Count), attack + ": attach up to 2 basic " + type + " Energy cards from your hand");
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), attack + ": attach them to which Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                {
                    ctx.Me.Hand.Remove(card);
                    ctx.Engine.AttachEnergyCard(card, target, true);
                }
                return true;
            }
        }

        /// <summary>Major Earthquake: discard 2 Fighting Energy; 10 to each of your Benched Pokémon.</summary>
        private sealed class MajorEarthquakeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Major Earthquake") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Major Earthquake: discard 2 Fighting Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fighting));
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench().ToList()) ctx.DamageTarget(m, 10);
                return true;
            }
        }

        /// <summary>One Desire: search your deck for any 1 card, shuffle, then put it on top.</summary>
        private sealed class OneDesireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "One Desire") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var card = ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, 1, "One Desire: put which card on top of your deck?")[0];
                ctx.Me.Deck.Remove(card);
                ctx.ShuffleMyDeck();
                ctx.Me.Deck.Insert(0, card);
                return true;
            }
        }

        /// <summary>Doom Desire: discard all Energy from this Pokémon; the Defending Pokémon is Knocked Out at the end of the opponent's next turn.</summary>
        private sealed class DoomDesireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doom Desire") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                foreach (var e in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, e);
                if (defending != null && ctx.CanAffect(defending))
                    ctx.AddEffect(new DoomEffect(defending.Name + " is Knocked Out at the end of its next turn (Doom Desire)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class DoomEffect : ContinuousEffect
        {
            private readonly int turn;

            public DoomEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { turn = expiresAfterTurn; }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != turn) return;
                var target = engine.State.FindPokemon(TargetUid);
                if (target != null) engine.KnockOut(target, "Doom Desire");
            }
        }

        /// <summary>High Tide: discard 2 Water Energy; 20 to each of the opponent's Benched Pokémon.</summary>
        private sealed class HighTideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Tide") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "High Tide: discard 2 Water Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water));
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.OpponentBench().ToList()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Healing Tail (Poké-Body): while Lopunny is Active, remove 1 damage counter from each of your Benched Pokémon between turns. Collect, Cross-Cut.</summary>
        private sealed class LopunnyHealingTail : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new DrawAttackMon("Collect", 3), new CardsFST2.ConditionBonusMon("Cross-Cut", 30, c => c.Defending != null && CardsE2.IsEvolved(c.Defending)));

            public override int CheckupAffects(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) ? src.Player : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon)) return;
                var hurt = src.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return;
                Announce(src, "Healing Tail");
                foreach (var m in hurt) src.Heal(m, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Bind (Poké-Power): when you put Mesprit from your hand onto your Bench, the opponent can't use Poké-Powers during their next turn. Extrasensory.</summary>
        private sealed class MespritPsychicBind : Mon
        {
            private static readonly Mon Sense = new CardsFST2.ConditionBonusMon("Extrasensory", 50, c => c.Me.Hand.Count == c.Opp.Hand.Count);

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Psychic Bind", true, "Psychic Bind: your opponent can't use Poké-Powers during their next turn?")) return;
                ctx.AddEffect(new CardsSW.PokePowerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't use Poké-Powers during their next turn (Psychic Bind)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sense.HandleAttack(ctx, atk); }
        }

        /// <summary>Focus Punch: nothing if this Pokémon was damaged by an attack during the opponent's last turn.</summary>
        private sealed class FocusPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Focus Punch") return false;
                if (ctx.Pokemon.AttackDamageTurn == ctx.OpponentsLastTurn) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>"Discard 2 cards from your hand" Regi powers' shared cost.</summary>
        private static bool DiscardTwo(EffectContext ctx, string power)
        {
            if (ctx.Me.Hand.Count < 2) return false;
            foreach (var card in ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, power + ": discard 2 cards from your hand")) ctx.Engine.DiscardFromHand(ctx.Player, card);
            return true;
        }

        /// <summary>Regi Move (Poké-Power): once during your turn, discard 2 cards; the opponent switches their non-Evolved Active Pokémon with a Benched one. Not with a Special Condition. Ice Reflect.</summary>
        private sealed class RegiceRegiMove : Mon
        {
            protected override string Activated { get { return "Regi Move"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count >= 2 && ctx.Opp.Active != null && !CardsE2.IsEvolved(ctx.Opp.Active) && ctx.Opp.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (DiscardTwo(ctx, "Regi Move") && ctx.CanAffect(ctx.Opp.Active)) ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Reflect") return false;
                var defending = ctx.Defending;
                bool hurt = ctx.Pokemon.AttackDamageTurn == ctx.OpponentsLastTurn;
                ctx.DamageDefending(atk.Damage);
                if (hurt && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Recover Mechanism (Poké-Body): when you attach an Energy card from your hand to Regigigas, remove all Special Conditions from it. Gigaton Punch.</summary>
        private sealed class RegigigasRecoverMechanism : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || target.Conditions == SpecialCondition.None) return;
                Announce(src, "Recover Mechanism");
                src.Engine.RemoveAllConditions(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigaton Punch") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && ctx.OpponentBench().Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Gigaton Punch: do 20 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 20);
                }
                return true;
            }
        }

        /// <summary>Regi Cycle (Poké-Power): once during your turn, with a Fighting Energy card in your discard pile, discard 2 cards, then attach a Fighting Energy card from your discard pile to Regirock. Not with a Special Condition. Stone Edge.</summary>
        private sealed class RegirockRegiCycle : Mon
        {
            private static readonly Mon Edge = new FlipBonusMon("Stone Edge", 30);

            private static List<CardInstance> Fighting(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Fighting).ToList(); }

            protected override string Activated { get { return "Regi Cycle"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count >= 2 && Fighting(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!DiscardTwo(ctx, "Regi Cycle")) return;
                var pile = Fighting(ctx);
                if (pile.Count == 0) return;
                var card = ctx.ChooseCards(pile, 1, 1, "Regi Cycle: attach which Fighting Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, ctx.Pokemon) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Edge.HandleAttack(ctx, atk); }
        }

        /// <summary>Regi Heal (Poké-Power): once during your turn, discard 2 cards, then remove 3 damage counters from Registeel. Not with a Special Condition. Rotating Claws.</summary>
        private sealed class RegisteelRegiHeal : Mon
        {
            private static readonly Mon Claws = new CardsTRR.RotatingClawsMon();

            protected override string Activated { get { return "Regi Heal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count >= 2 && ctx.Pokemon.DamageCounters > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (DiscardTwo(ctx, "Regi Heal")) ctx.Heal(ctx.Pokemon, 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        /// <summary>Resent (Poké-Power): once during the opponent's turn, if Shedinja is Knocked Out by attack damage, 4 damage counters on the Attacking Pokémon and each of their Pokémon with the same name. Curse and Deceive.</summary>
        private sealed class ShedinjaResent : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (me == null || !byOpponentsAttack || src.IsMyTurn) return;
                var attacker = src.OpponentPokemon().FirstOrDefault(m => m.Uid == me.HitByAttackerUid);
                if (attacker == null || !src.AskYesNo("Resent: put 4 damage counters on " + attacker.Name + " and each Pokémon with its name?")) return;
                Announce(src, "Resent");
                foreach (var m in src.OpponentPokemon(p => p.Name == attacker.Name).ToList()) src.Engine.PlaceDamageCounters(m, 4, "Resent");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Curse and Deceive") return false;
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                ctx.PlaceCounters(defending, 3);
                if (ctx.Pokemon.DamageCounters > 0 && ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>White Smoke (Poké-Body): while Torkoal is Active, prevent all effects of the opponent's attacks, damage included, on your Benched Pokémon. Black Soot.</summary>
        private sealed class TorkoalWhiteSmoke : Mon
        {
            private static readonly Mon Soot = new CardsCES1.FlipConditionsMon("Black Soot", SpecialCondition.Burned, SpecialCondition.Paralyzed);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var t = info.Target;
                return me != null && src.IsActive(me) && t != null && t != me && t.Owner == src.Player && src.Me.Bench.Contains(t)
                    && info.Attacker != null && info.Attacker.Owner != src.Player && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && target != null && target != me && src.Me.Bench.Contains(target) && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Soot.HandleAttack(ctx, atk); }
        }

        /// <summary>! (Poké-Power): when you put Unown ! from your hand onto your Bench, flip: heads, 2 damage counters on 1 of the opponent's Pokémon; tails, on 1 of yours. Hidden Power: heads, 10 more; tails, 10 to itself.</summary>
        private sealed class UnownBang : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "!", true, "!: flip a coin?")) return;
                if (ctx.Flip())
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "!: put 2 damage counters on which of your opponent's Pokémon?");
                    if (target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, 2);
                }
                else
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "!: put 2 damage counters on which of your Pokémon?");
                    if (target != null) ctx.Engine.PlaceDamageCounters(target, 2, "!");
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (ctx.Flip()) ctx.DamageDefending(atk.Damage + 10);
                else
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.DamageWith(ctx.Pokemon, 10, false, false, false);
                }
                return true;
            }
        }

        /// <summary>Burning Scent: you may switch in 1 of the opponent's Benched Pokémon first; the Defending Pokémon is Burned and Poisoned.</summary>
        private sealed class BurningScentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Scent") return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Burning Scent: switch in 1 of your opponent's Benched Pokémon first?")) ctx.GustOpponent("Burning Scent: switch in which Benched Pokémon?");
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Energy Dissolve: an Energy card on the Defending Pokémon is discarded at the end of the opponent's next turn.</summary>
        private sealed class EnergyDissolveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Dissolve") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new DissolveEffect(defending.Name + " loses an Energy card at the end of its next turn (Energy Dissolve)", ctx.Player, defending.Uid, ctx.Turn + 1, ctx.Card.Def));
                return true;
            }
        }

        private sealed class DissolveEffect : ContinuousEffect
        {
            private readonly int turn;
            private readonly CardDef source;

            public DissolveEffect(string name, int owner, int targetUid, int expiresAfterTurn, CardDef source) : base(name, owner, targetUid, expiresAfterTurn, false)
            {
                turn = expiresAfterTurn;
                this.source = source;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != turn) return;
                var target = engine.State.FindPokemon(TargetUid);
                if (target == null || target.Energy.Count == 0) return;
                var card = engine.ChooseCards(Owner, target.Energy.ToList(), 1, 1, "Energy Dissolve: discard which Energy card from " + target.Name + "?", source)[0];
                engine.DiscardEnergy(target, card);
            }
        }

        /// <summary>
        /// Energy Reaction (Poké-Power): once during your turn, when you attach a Grass or Psychic Energy card from your hand to
        /// Vileplume, you may make the Defending Pokémon Asleep (Grass) or Poisoned (Psychic). Not with a Special Condition. Disturbing
        /// Pollen: heads, no Trainers, Supporters or Stadiums for the opponent next turn.
        /// </summary>
        private sealed class VileplumeEnergyReaction : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || !CardsDS.PowerReady(src)) return;
                var type = energy.Def.EnergyProvides;
                if (type != EnergyType.Grass && type != EnergyType.Psychic) return;
                var defending = src.Opp.Active;
                if (defending == null || !TriggerOnce(src, "Energy Reaction", true, "Energy Reaction: make the Defending Pokémon " + (type == EnergyType.Grass ? "Asleep" : "Poisoned") + "?")) return;
                if (src.CanAffect(defending)) src.ApplyCondition(defending, type == EnergyType.Grass ? SpecialCondition.Asleep : SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disturbing Pollen") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new AllTrainersLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer, Supporter or Stadium cards during their next turn (Disturbing Pollen)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>The player can't play any Trainer, Supporter or Stadium card from their hand (Disturbing Pollen).</summary>
        internal sealed class AllTrainersLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public AllTrainersLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected && card != null && card.Def.IsTrainer; }
        }

        // ------------------------------------------------------------------ LV.X

        /// <summary>Psychic Aura (Poké-Body): your Psychic Pokémon have no Weakness. Deep Balance: 1 damage counter on 1 of the opponent's Pokémon for each Energy on all their Pokémon.</summary>
        private sealed class AzelfLvXPsychicAura : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon != null && mon.Owner == src.Player && src.Engine.IsType(mon, EnergyType.Psychic); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deep Balance") return false;
                int n = ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Deep Balance: put damage counters on which Pokémon?");
                if (target != null && n > 0 && ctx.CanAffect(target)) ctx.PlaceCounters(target, n);
                return true;
            }
        }

        /// <summary>Shoot Poison (Poké-Power): when you level up your Active Gliscor to this from your hand, the Defending Pokémon is Paralyzed and Poisoned. Night Slash: you may switch Gliscor with a Benched Pokémon.</summary>
        private sealed class GliscorLvXShootPoison : Mon
        {
            private static readonly Mon Slash = new SwitchSelfMon("Night Slash", true);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (!ctx.PokemonIsActive || !TriggerOnce(ctx, "Shoot Poison", target != null, "Shoot Poison: Paralyze and Poison the Defending Pokémon?") || !ctx.CanAffect(target)) return;
                ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                if (ctx.InPlay(target)) ctx.ApplyCondition(target, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slash.HandleAttack(ctx, atk); }
        }

        /// <summary>Electric Trans (Poké-Power): as often as you like during your turn, move a Lightning or Metal Energy from 1 of your Pokémon to another. Not with a Special Condition. Cyber Shock.</summary>
        private sealed class MagnezoneLvXElectricTrans : Mon
        {
            private const string Power = "Electric Trans";

            private static bool Fits(EffectContext ctx, PokemonInPlay m, CardInstance e) { return CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Lightning) || CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Metal); }

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Any(e => Fits(ctx, m, e))); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.InPlay().Count() > 1 && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => ctx.Me.InPlay().Count() > 1 && Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a Lightning or Metal Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), Power + ": move it to which Pokémon?");
                    if (to == null) return false;
                    var cards = from.Energy.Where(e => Fits(ctx, from, e)).ToList();
                    var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, Power + ": move which Energy card?")[0];
                    return ctx.Engine.MoveEnergy(from, to, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cyber Shock") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DiscardEnergyFrom(me, 1, "Cyber Shock: discard a Lightning Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning));
                ctx.DiscardEnergyFrom(me, 1, "Cyber Shock: discard a Metal Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Metal));
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Supreme Blast: nothing unless you have Uxie LV.X and Azelf LV.X in play; discard all Energy from Mesprit.</summary>
        private sealed class SupremeBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supreme Blast") return false;
                if (ctx.MyPokemon(m => m.Name == "Uxie LV.X").Count == 0 || ctx.MyPokemon(m => m.Name == "Azelf LV.X").Count == 0) return Nothing(ctx);
                var me = ctx.Pokemon;
                foreach (var e in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, e);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Trade Off (Poké-Power): once during your turn, look at the top 2 cards; 1 into your hand, the other on the bottom (1 Trade Off a turn). Not with a Special Condition. Zen Blade.</summary>
        private sealed class UxieLvXTradeOff : Mon
        {
            private static readonly Mon Blade = new LockAfterAttackMon("Zen Blade", true);

            protected override string Activated { get { return "Trade Off"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return;
                var keep = ctx.ChooseCards(top, 1, 1, "Trade Off: put which card into your hand?")[0];
                ctx.DeckToHand(new[] { keep });
                foreach (var card in top.Where(c => c != keep))
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Deck.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blade.HandleAttack(ctx, atk); }
        }
    }
}
