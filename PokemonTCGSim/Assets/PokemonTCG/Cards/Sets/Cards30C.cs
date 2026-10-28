using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// 30th Celebration (30C): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Ninetales, the Pikachu with Mach Bolt, Gnaw, Rollout, Hang Down / Zap Kick and
    /// Satisfied Spark, Toxel, Toxtricity (Mach Bolt), Marill, Cosmog, Zorua, Deino, Zweilous, Hakamo-o, Kommo-o,
    /// Hisuian Zorua.
    /// </summary>
    public static class Cards30C
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("30C-1", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("30C-3", new CardsPOR.FollowMeMon("Luring Glow"));
            r.RegisterByKey("30C-4", new IllumiseC());
            r.RegisterByKey("30C-5", new RetaliateBonusMon("Rally Back", 90));
            r.RegisterByKey("30C-6", new DodgeMon("Hide", false));
            r.RegisterByKey("30C-7", new EnergyGiftMon("Energy Gift"));
            r.RegisterByKey("30C-8", new VivillonC());
            r.RegisterByKey("30C-9", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("30C-12", new HoOhC());
            r.RegisterByKey("30C-13", new CallForFamilyMon());
            r.RegisterByKey("30C-15", new FuecocoExC());
            r.RegisterByKey("30C-16", new DodgeMon("Well-Hidden", false));
            r.RegisterByKey("30C-17", new ComboMon(new SearchAttackMon("Ferry Across", d => d.IsSupporter, 1), new ParalyzeFlipMon("Ice Beam")));
            r.RegisterByKey("30C-18", new ArticunoC());
            r.RegisterByKey("30C-19", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 30));
            r.RegisterByKey("30C-20", new PalkiaC());
            r.RegisterByKey("30C-22", new WishiwashiC());
            r.RegisterByKey("30C-23", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("30C-24", new RecoilMon("Volt Tackle", 30));
            r.RegisterByKey("30C-25", new Cards30CC.BenchHitMon("Spark", 20));
            r.RegisterByKey("30C-26", new PeerAtMon("Peer At"));
            r.RegisterByKey("30C-27", new CardsTWM1.HealSelfMon("Nap", 30));
            r.RegisterByKey("30C-28", new LonelyGazePikachu());
            r.RegisterByKey("30C-29", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("30C-32", new SwitchSelfMon("Scurry About", false));
            r.RegisterByKey("30C-33", new KeepHiddenPikachu());
            r.RegisterByKey("30C-34", new PikaChainPikachu());
            r.RegisterByKey("30C-36", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("30C-37", new SearchAttackMon("Energized Tail", d => d.IsEnergy, 1));
            r.RegisterByKey("30C-38", new CardsMEG.SnipeMon("Targeted Spark", 20));
            r.RegisterByKey("30C-39", new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"));
            r.RegisterByKey("30C-40", new OverwritingBoltPikachu());
            r.RegisterByKey("30C-42", new ChargeUpDashPikachu());
            r.RegisterByKey("30C-43", new TropicalVibesPikachu());
            r.RegisterByKey("30C-44", new DodgeMon("Agility", false));
            r.RegisterByKey("30C-45", new DrawAttackMon("Nightime Stroll", 1));
            r.RegisterByKey("30C-46", new GetSomeAirPikachu());
            r.RegisterByKey("30C-47", new FlipBonusMon("Play Rough", 20));
            r.RegisterByKey("30C-48", new StoreUpPikachu());
            r.RegisterByKey("30C-49", new CardsASC.ExBonusMon("Fighting Lightning", 80));
            r.RegisterByKey("30C-51", new LightningCrashPikachu());
            r.RegisterByKey("30C-52", new SelfCounterBonusMon("Angry Bolt", 10));
            r.RegisterByKey("30C-53", new PikachuExParadeC());
            r.RegisterByKey("30C-54", new PikachuExFrenzyC());
            r.RegisterByKey("30C-55", new ZapdosC());
            r.RegisterByKey("30C-56", new TypeEnergyBonusMon("Nitro Thunder", EnergyType.Fire, 80));
            r.RegisterByKey("30C-57", new ComboMon(new DrawAttackMon("Rapid Draw", 1), new Cards30CC.BenchHitMon("Electrobullet", 20)));
            r.RegisterByKey("30C-60", new LockAfterAttackMon("Thunderous Bolt", false));
            r.RegisterByKey("30C-61", new MorpekoC());
            r.RegisterByKey("30C-62", new CardsASC.SelfDiscardTypeMon("Electro Drift", EnergyType.Lightning, 2));
            r.RegisterByKey("30C-63", new MewtwoC());
            r.RegisterByKey("30C-64", new MewtwoExC());
            r.RegisterByKey("30C-65", new CardsMEG.DefendingEnergyBonusMon("Psychic", 40));
            r.RegisterByKey("30C-68", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("30C-70", new CardsPFL.PerPokemonInPlayMon("Solar Beatdown"));
            r.RegisterByKey("30C-73", new DrifloonC());
            r.RegisterByKey("30C-74", new CardsTWM1.HealSelfMon("Aurora Gain", 30));
            r.RegisterByKey("30C-75", new BurnAndConfuseMon("Eerie Glow"));
            r.RegisterByKey("30C-76", new SearchAttackMon("Geonavigation", d => d.IsStadiumCard, 2));
            r.RegisterByKey("30C-77", new ComfeyC());
            r.RegisterByKey("30C-79", new CardsASC.SelfGuardMon("Stiffen", 60));
            r.RegisterByKey("30C-80", new LunalaC());
            r.RegisterByKey("30C-81", new GimmighoulC());
            r.RegisterByKey("30C-82", new GroudonC());
            r.RegisterByKey("30C-85", new LycanrocC());
            r.RegisterByKey("30C-86", new CardsASC.SelfDiscardTypeMon("Collision Course", EnergyType.Fighting, 2));
            r.RegisterByKey("30C-87", new CardsMEG.WeakenDefendingMon("Growl", 30));
            r.RegisterByKey("30C-89", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("30C-90", new GengarExC());
            r.RegisterByKey("30C-91", new RetaliateBonusMon("Retaliate", 100));
            r.RegisterByKey("30C-92", new CardsPFL.DamagedBonusMon("Lunatic Claw", 140));
            r.RegisterByKey("30C-93", new MurkrowC());
            r.RegisterByKey("30C-94", new CardsDRX.ReverseSpinMon("Nitpick"));
            r.RegisterByKey("30C-96", new ZoroarkC());
            r.RegisterByKey("30C-99", new HydreigonC());
            r.RegisterByKey("30C-101", new GalarianMeowthC());
            r.RegisterByKey("30C-103", new DialgaC());
            r.RegisterByKey("30C-104", new FerrothornC());
            r.RegisterByKey("30C-105", new SolgaleoC());
            r.RegisterByKey("30C-106", new ComboMon(new CardsASC.ToolBonusMon("Hardened Blade", 40), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("30C-107", new ZamazentaC());
            r.RegisterByKey("30C-108", new GholdengoC());
            r.RegisterByKey("30C-109", new SalamenceExC());
            r.RegisterByKey("30C-110", new JangmooC());
            r.RegisterByKey("30C-113", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("30C-114", new SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("30C-115", new DittoC());
            r.RegisterByKey("30C-116", new EeveeFetchC());
            r.RegisterByKey("30C-117", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("30C-119", new SnorlaxC());
            r.RegisterByKey("30C-120", new IgglybuffC());
            r.RegisterByKey("30C-121", new LugiaC());
            r.RegisterByKey("30C-123", new SwirlingResentmentMon("Swirling Resentment", 50));
            r.RegisterByKey("30C-124", new MiniorC());
            r.RegisterByKey("30C-125", new MausholdC());
        }

        // ================================================================== Shared helpers

        /// <summary>"If any of your Pokémon were Knocked Out by damage from an attack during your opponent's last turn, this attack does N more damage."</summary>
        public sealed class RetaliateBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public RetaliateBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutByAttackLastOpponentTurn() ? bonus : 0));
                return true;
            }
        }

        /// <summary>"This attack does N more damage for each damage counter on this Pokémon."</summary>
        public sealed class SelfCounterBonusMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public SelfCounterBonusMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + per * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>"If this Pokémon has any [T] Energy attached, this attack does N more damage."</summary>
        public sealed class TypeEnergyBonusMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int bonus;

            public TypeEnergyBonusMon(string attack, EnergyType type, int bonus)
            {
                this.attack = attack;
                this.type = type;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Engine.HasEnergyOfType(ctx.Pokemon, type) ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Your opponent's Active Pokémon is now Burned and Confused."</summary>
        public sealed class BurnAndConfuseMon : Mon
        {
            private readonly string attack;

            public BurnAndConfuseMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                }
                return true;
            }
        }

        /// <summary>The legendary birds: "Once during your turn, if you have [the other two] in play, attach a Basic [T] Energy card from your hand to this Pokémon."</summary>
        public abstract class FlappingMon : Mon
        {
            protected abstract string AbilityName { get; }
            protected abstract EnergyType Type { get; }
            protected abstract string[] Partners { get; }

            protected override string Activated { get { return AbilityName; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                foreach (var name in Partners)
                    if (ctx.MyPokemon(m => m.Name == name).Count == 0) return false;
                return ctx.Engine.CanAttachFromHand(ctx.Pokemon) && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, Type));
            }

            protected override void Ability(EffectContext ctx)
            {
                CardsPFL.AttachTypeFromHand(ctx, Type, new List<PokemonInPlay> { ctx.Pokemon }, AbilityName);
            }
        }

        /// <summary>Discard every Energy attached to this Pokémon that provides [T].</summary>
        public static int DiscardAllOfType(EffectContext ctx, EnergyType type)
        {
            var mon = ctx.Pokemon;
            if (mon == null || !ctx.InPlay(mon)) return 0;
            var list = mon.Energy.Where(e => ctx.Engine.EnergyUnitsOf(mon, e).Any(u => EnergyMask.Provides(u, type))).ToList();
            int n = 0;
            foreach (var e in list) if (ctx.Engine.DiscardEnergy(mon, e)) n++;
            return n;
        }

        // ================================================================== Grass

        /// <summary>Supereffective Pheromones: with Volbeat in play, Weakness for both Active Pokémon is x3.</summary>
        private sealed class IllumiseC : Mon
        {
            public override int WeaknessMultiplier(EffectContext src, DamageInfo info)
            {
                if (src.Pokemon == null || !info.TargetIsActive) return 2;
                return src.MyPokemon(m => m.Name == "Volbeat").Count > 0 ? 3 : 2;
            }
        }

        /// <summary>Energy Gift: search your deck for up to 2 Basic Energy and attach them to your Pokémon in any way you like. Used for Energy Gift, Powerful Steps (the attack name is passed in).</summary>
        internal sealed class EnergyGiftMon : Mon
        {
            private readonly string attack;

            public EnergyGiftMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, attack + ": choose up to 2 Basic Energy cards to attach to your Pokémon");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (c, m) => true, attack);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Guiding Dance: once during your turn, flip a coin; heads search your deck for a Pokémon. Poison Powder: Poisoned.</summary>
        private sealed class VivillonC : Mon
        {
            private static readonly Mon PoisonPowder = new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned);

            protected override string Activated { get { return "Guiding Dance"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.SearchToHand(d => d.IsPokemon, 1, "Guiding Dance: choose a Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PoisonPowder.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Sacred Breath: discard all Energy from this Pokémon and heal all damage from 1 of your Benched Pokémon.</summary>
        private sealed class HoOhC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sacred Breath") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                var me = ctx.Pokemon;
                CardsMEG.HealOneOfMine(ctx, 10000, "Sacred Breath", m => m != me && !ctx.IsActive(m));
                return true;
            }
        }

        /// <summary>Singe: Burned. Cheerful Flame: 70 for each Prize card you have taken.</summary>
        private sealed class FuecocoExC : Mon
        {
            private static readonly Mon Singe = new ConditionAttackMon("Singe", SpecialCondition.Burned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cheerful Flame")
                {
                    ctx.DamageDefending(atk.Damage * ctx.MyPrizesTaken);
                    return true;
                }
                return Singe.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Water

        /// <summary>Frosty Flapping (needs Moltres and Zapdos). Hail: 30 damage to each of your opponent's Pokémon.</summary>
        private sealed class ArticunoC : FlappingMon
        {
            protected override string AbilityName { get { return "Frosty Flapping"; } }
            protected override EnergyType Type { get { return EnergyType.Water; } }
            protected override string[] Partners { get { return new[] { "Moltres", "Zapdos" }; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hail") return false;
                foreach (var m in ctx.OpponentPokemon()) ctx.DamageTarget(m, 30);
                return true;
            }
        }

        /// <summary>Wormhole: switch this Pokémon with 1 of your Benched Pokémon; if you do, your opponent switches their Active Pokémon to the Bench.</summary>
        private sealed class PalkiaC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wormhole") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                if (ctx.SwitchSelfWithBench()) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>
        /// Counterattack Grouping: when your Active Wishiwashi (or Wishiwashi ex) is damaged by the opponent's attack, 3 damage
        /// counters on the Attacking Pokémon. It doesn't stack (your ruling): there's only 1 Pokémon in the Active Spot, so
        /// each hit triggers it once however many Wishiwashi with this Ability you have. Surprise Attack: tails, nothing.
        /// </summary>
        private sealed class WishiwashiC : Mon
        {
            private static readonly Mon SurpriseAttack = new TailsNothingMon("Surprise Attack");

            private static void HitBack(EffectContext src, PokemonInPlay target, PokemonInPlay attacker, bool targetWasActive)
            {
                if (!targetWasActive || target == null || (target.Name != "Wishiwashi" && target.Name != "Wishiwashi ex")) return;
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                if (src.Pokemon == null || src.Pokemon != Responder(src, target)) return;
                Announce(src, "Counterattack Grouping");
                src.Engine.PlaceDamageCounters(attacker, 3, "Counterattack Grouping");
            }

            /// <summary>The one Wishiwashi whose Counterattack Grouping answers this hit: the damaged one if its Ability works, otherwise the first in play.</summary>
            private static PokemonInPlay Responder(EffectContext src, PokemonInPlay target)
            {
                var holders = src.Engine.Sources()
                    .Where(s => s.Script is WishiwashiC && s.Ctx.Player == src.Player && s.Ctx.Pokemon != null)
                    .Select(s => s.Ctx.Pokemon)
                    .ToList();
                return holders.Contains(target) ? target : holders.FirstOrDefault();
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                HitBack(ctx, ctx.Pokemon, attacker, holderWasActive);
            }

            public override void OnOwnPokemonDamagedByAttack(EffectContext src, PokemonInPlay target, PokemonInPlay attacker, bool targetWasActive, int damage)
            {
                HitBack(src, target, attacker, targetWasActive);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SurpriseAttack.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Peer At: your opponent reveals their hand. Used for Peer At, Scout, Clairvoyance (the attack name is passed in).</summary>
        internal sealed class PeerAtMon : Mon
        {
            private readonly string attack;

            public PeerAtMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Lonely Gaze: while this Pokémon is Active, the opponent's Active Pokémon's attacks do 20 less damage (before Weakness and Resistance).</summary>
        private sealed class LonelyGazePikachu : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.State.IsActive(info.Attacker) ? damage - 20 : damage;
            }
        }

        /// <summary>Keep Hidden: while on your Bench, prevent all damage and effects of the opponent's attacks done to this Pokémon.</summary>
        private sealed class KeepHiddenPikachu : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack && !src.State.IsActive(target);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && !info.TargetIsActive && info.AgainstOpponent;
            }
        }

        /// <summary>Pika Chain: 40 damage for each of your Pikachu and Pikachu ex in play.</summary>
        private sealed class PikaChainPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pika Chain") return false;
                int n = ctx.MyPokemon(m => m.Name == "Pikachu" || m.Name == "Pikachu ex").Count;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>The Defending Pokémon's Weakness is [L] until the end of your next turn.</summary>
        public sealed class WeaknessChangeEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public WeaknessChangeEffect(string name, int owner, int targetUid, int expiresAfterTurn, EnergyType type)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.type = type;
            }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon != null && mon.Uid == TargetUid ? type : weakness;
            }
        }

        /// <summary>Overwriting Bolt: the Defending Pokémon's Weakness is Lightning until the end of your next turn.</summary>
        private sealed class OverwritingBoltPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overwriting Bolt") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new WeaknessChangeEffect(defending.Name + "'s Weakness is Lightning (Overwriting Bolt)", ctx.Player, defending.Uid, ctx.Turn + 2, EnergyType.Lightning));
                return true;
            }
        }

        /// <summary>Charge-Up Dash: flip until tails; attach up to that many Basic Lightning Energy from your deck to this Pokémon.</summary>
        private sealed class ChargeUpDashPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charge-Up Dash") return false;
                int heads = ctx.FlipUntilTails();
                if (heads <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), heads, "Charge-Up Dash: choose up to " + heads + " Basic Lightning Energy cards");
                foreach (var c in found) ctx.Engine.AttachEnergyCard(c, ctx.Pokemon);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Tropical Vibes: this Pokémon is now Asleep; draw until you have 6 cards.</summary>
        private sealed class TropicalVibesPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tropical Vibes") return false;
                if (ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                ctx.DrawUntil(6);
                return true;
            }
        }

        /// <summary>Get Some Air: this Pokémon recovers from all Special Conditions.</summary>
        private sealed class GetSomeAirPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Get Some Air") return false;
                ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Store Up: put up to 2 Basic Energy cards from your discard pile into your hand.</summary>
        private sealed class StoreUpPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Store Up") return false;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0) return true;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(2, basics.Count), "Store Up: put up to 2 Basic Energy cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Lightning Crash: discard all Lightning Energy from this Pokémon; 90 damage to 1 of your opponent's Pokémon.</summary>
        private sealed class LightningCrashPikachu : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Crash") return false;
                DiscardAllOfType(ctx, EnergyType.Lightning);
                ctx.DamageAnyOpponent(90);
                return true;
            }
        }

        /// <summary>Pika-Pika Parade: search your deck for any number of Basic Pokémon and put them onto your Bench. Thunderbolt: discard all Energy.</summary>
        private sealed class PikachuExParadeC : Mon
        {
            private static readonly Mon Thunderbolt = new CardsTWM1.SelfDiscardMon("Thunderbolt", 99);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pika-Pika Parade")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => true, ctx.BenchSpace, "Pika-Pika Parade: put any number of Basic Pokémon onto your Bench");
                    return true;
                }
                return Thunderbolt.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Zip-Zap Frenzy: attach any number of Basic Energy from your hand to your Pokémon in any way you like. Thunder: 30 to itself.</summary>
        private sealed class PikachuExFrenzyC : Mon
        {
            private static readonly Mon Thunder = new RecoilMon("Thunder", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Zip-Zap Frenzy")
                {
                    CardsSSP2.AttachFromHandAnyWay(ctx, d => d.IsBasicEnergy, 60, m => true, "Zip-Zap Frenzy");
                    return true;
                }
                return Thunder.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Flash-Pop Flapping (needs Moltres and Articuno). Thundering Lightning: 60 damage to itself.</summary>
        private sealed class ZapdosC : FlappingMon
        {
            private static readonly Mon Thundering = new RecoilMon("Thundering Lightning", 60);

            protected override string AbilityName { get { return "Flash-Pop Flapping"; } }
            protected override EnergyType Type { get { return EnergyType.Lightning; } }
            protected override string[] Partners { get { return new[] { "Moltres", "Articuno" }; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Thundering.HandleAttack(ctx, atk); }
        }

        /// <summary>Select a Snack: discard the top 3 cards of your deck and put 1 of them into your hand.</summary>
        private sealed class MorpekoC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Select a Snack") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                var still = milled.Where(c => ctx.Me.Discard.Contains(c)).ToList();
                if (still.Count == 0) return true;
                var pick = ctx.ChooseCards(still, 1, 1, "Select a Snack: put 1 of the discarded cards into your hand");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Empower: attach up to 2 Basic Energy cards from your discard pile to 1 of your Pokémon. Psydrive: discard an Energy.</summary>
        private sealed class MewtwoC : Mon
        {
            private static readonly Mon Psydrive = new CardsTWM1.SelfDiscardMon("Psydrive", 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Empower")
                {
                    var basics = ctx.BasicEnergyInDiscard();
                    if (basics.Count == 0) return true;
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Empower: attach Basic Energy from your discard pile to which of your Pokémon?");
                    if (target == null) return true;
                    var picks = ctx.ChooseCards(basics, 0, Math.Min(2, basics.Count), "Empower: choose up to 2 Basic Energy cards to attach to " + target.Name);
                    foreach (var c in picks)
                        if (ctx.Me.Discard.Remove(c)) ctx.Engine.AttachEnergyCard(c, target);
                    return true;
                }
                return Psydrive.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Photon Bullets: 50 damage to each of your opponent's Pokémon ex. Psychic Powers: can't attack next turn.</summary>
        private sealed class MewtwoExC : Mon
        {
            private static readonly Mon PsychicPowers = new LockAfterAttackMon("Psychic Powers", false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Photon Bullets")
                {
                    foreach (var m in ctx.OpponentPokemon(x => x.Def.IsEx || x.Def.IsMegaEx)) ctx.DamageTarget(m, 50);
                    return true;
                }
                return PsychicPowers.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Float Up: you may shuffle this Pokémon and all attached cards into your deck.</summary>
        private sealed class DrifloonC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Float Up") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Float Up: shuffle this Pokémon and all attached cards into your deck?"))
                    ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Comforting Aroma: heal 80 damage from 1 of your Benched Pokémon.</summary>
        private sealed class ComfeyC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Comforting Aroma") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsMEG.HealOneOfMine(ctx, 80, "Comforting Aroma", m => !ctx.IsActive(m));
                return true;
            }
        }

        /// <summary>Midnight Ray: 20 more for each Energy card in your discard pile.</summary>
        private sealed class LunalaC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Midnight Ray") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.Me.Discard.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        /// <summary>Strolls So Much: flip a coin; heads, search your deck for any card.</summary>
        private sealed class GimmighoulC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strolls So Much") return false;
                if (ctx.Flip() && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Strolls So Much: choose a card", true, 1);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Break Ground: also 20 damage to each of your Benched Pokémon.</summary>
        private sealed class GroudonC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Break Ground") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Counter: does as much more damage as this Pokémon took from attacks during your opponent's last turn.</summary>
        private sealed class LycanrocC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Counter") return false;
                var me = ctx.Pokemon;
                int taken = me.AttackDamageTurn == ctx.OpponentsLastTurn ? me.AttackDamageThatTurn : 0;
                ctx.DamageDefending(atk.Damage + taken);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>
        /// Fainting Spell: Knocked Out by the opponent's attack damage, flip; heads the Attacking Pokémon is Knocked Out. The
        /// Attacking Pokémon is the one that used the attack, even if it isn't the Active Pokémon (your ruling): it may have
        /// attacked from the Bench or switched itself out. Chaotic Pain: 13 damage counters on 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class GengarExC : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (!byOpponentsAttack || attacker == null || attacker.Owner == src.Player || src.Engine.IsKnockedOut(attacker)) return;
                Announce(src, "Fainting Spell");
                if (src.Flip()) src.Engine.KnockOut(attacker, "Fainting Spell");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chaotic Pain") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Chaotic Pain: put 13 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 13);
                return true;
            }
        }

        /// <summary>Clumsily Clutch: flip; heads the Defending Pokémon can't retreat during your opponent's next turn.</summary>
        private sealed class MurkrowC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clumsily Clutch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Nighttime Byway: while this Pokémon is on your Bench, your Active Pokémon's Retreat Cost is 2 less.</summary>
        private sealed class ZoroarkC : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || src.State.IsActive(src.Pokemon) || mon == null || mon != src.Me.Active) return cost;
                return Math.Max(0, cost - 2);
            }
        }

        /// <summary>Three-Headed Bite: flip 3 coins; for each heads, discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class HydreigonC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Three-Headed Bite") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, heads, "Three-Headed Bite: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Pay Day: draw a card. Treasure Rush: 10 damage for each card in your hand.</summary>
        private sealed class GalarianMeowthC : Mon
        {
            private static readonly Mon PayDay = new DrawAttackMon("Pay Day", 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Treasure Rush")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Me.Hand.Count);
                    return true;
                }
                return PayDay.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Reversed Clock: shuffle up to 3 Pokémon and Basic Energy cards from your discard pile into your deck.</summary>
        private sealed class DialgaC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reversed Clock") return false;
                var pool = ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList();
                if (pool.Count == 0) return true;
                var picks = ctx.ChooseCards(pool, 0, Math.Min(3, pool.Count), "Reversed Clock: shuffle up to 3 Pokémon and Basic Energy cards into your deck");
                foreach (var c in picks)
                    if (ctx.Me.Discard.Remove(c)) ctx.Me.Deck.Add(c);
                if (picks.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Kaboom Needles: 50 damage to each of your opponent's Pokémon and 130 to itself.</summary>
        private sealed class FerrothornC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kaboom Needles") return false;
                foreach (var m in ctx.OpponentPokemon()) ctx.DamageTarget(m, 50);
                ctx.DamageSelf(130);
                return true;
            }
        }

        /// <summary>Sunrise: once during your turn on the Bench, attach up to 2 Basic Metal Energy from your deck to it. Sunsteel Strike: discard all Energy.</summary>
        private sealed class SolgaleoC : Mon
        {
            private static readonly Mon Sunsteel = new CardsTWM1.SelfDiscardMon("Sunsteel Strike", 99);

            protected override string Activated { get { return "Sunrise"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return !ctx.PokemonIsActive && ctx.Me.Deck.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Metal));
            }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Metal), 2, "Sunrise: choose up to 2 Basic Metal Energy cards to attach to " + ctx.Pokemon.Name);
                foreach (var c in found) ctx.Engine.AttachEnergyCard(c, ctx.Pokemon);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sunsteel.HandleAttack(ctx, atk); }
        }

        /// <summary>Fend Off: before damage, discard the Pokémon Tools from the opponent's Active Pokémon. Shield Press: takes 50 less next turn.</summary>
        private sealed class ZamazentaC : Mon
        {
            private static readonly Mon ShieldPress = new CardsASC.SelfGuardMon("Shield Press", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fend Off")
                {
                    var defending = ctx.Defending;
                    if (defending != null && defending.Tool != null && ctx.CanAffect(defending)) ctx.Engine.DiscardTool(defending);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return ShieldPress.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Celebration: with exactly 30 cards in your hand, take 2 Prize cards and shuffle your hand into your deck. Triple Smash: 3 coins.</summary>
        private sealed class GholdengoC : Mon
        {
            private static readonly Mon TripleSmash = new CoinsMon("Triple Smash", 3);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Celebration")
                {
                    if (ctx.Me.Hand.Count != 30 || ctx.Me.Prizes.Count == 0) return Nothing(ctx);
                    // The Prize cards go to the hand first, then the whole hand is shuffled into the deck.
                    ctx.Engine.TakePrizeCards(ctx.Player, 2);
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    return true;
                }
                return TripleSmash.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Dragon

        /// <summary>Booming Call: put up to 3 Basic Dragon Pokémon from your discard pile onto your Bench. Dragon Pulse: discard the top 2 cards of your deck.</summary>
        private sealed class SalamenceExC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Booming Call")
                {
                    var dragons = ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && c.Def.PokemonType == EnergyType.Dragon).ToList();
                    int room = Math.Min(3, ctx.BenchSpace);
                    if (dragons.Count == 0 || room <= 0) return true;
                    var picks = ctx.ChooseCards(dragons, 0, Math.Min(room, dragons.Count), "Booming Call: put up to " + room + " Basic Dragon Pokémon onto your Bench");
                    foreach (var c in picks)
                    {
                        if (ctx.BenchSpace <= 0 || !ctx.Me.Discard.Remove(c)) continue;
                        ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    }
                    return true;
                }
                if (atk.Name == "Dragon Pulse")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 2);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Screech: during your next turn, the Defending Pokémon takes 30 more damage from attacks.</summary>
        private sealed class JangmooC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Screech") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var effect = new DamageTakenEffect(defending.Name + " takes 30 more damage during your next turn (Screech)", ctx.Player, defending.Uid, ctx.Turn + 2, 30);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Surprisingly Transform: flip; heads, switch a Pokémon from your deck with this Pokémon (everything stays), then put Ditto into your deck.</summary>
        private sealed class DittoC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprisingly Transform") return false;
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon, 1, "Surprisingly Transform: choose a Pokémon to switch with Ditto");
                if (found.Count > 0 && ctx.InPlay(ctx.Pokemon)) CardsTWM1.ReplaceTopCard(ctx, ctx.Pokemon, found[0], ctx.Me.Deck);
                else ctx.Me.Deck.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Fetch and Hide: your opponent reveals their hand; put an Item card from it on the bottom of their deck.</summary>
        private sealed class EeveeFetchC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fetch and Hide") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                var items = ctx.Opp.Hand.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                var pick = ctx.ChooseCards(items, 1, 1, "Fetch and Hide: put which Item card on the bottom of your opponent's deck?");
                foreach (var c in pick)
                    if (ctx.Opp.Hand.Remove(c)) ctx.Opp.Deck.Add(c);
                return true;
            }
        }

        /// <summary>Good Sleep: if this Pokémon is still Asleep during Pokémon Checkup, heal all its damage. Collapse: this Pokémon is now Asleep.</summary>
        private sealed class SnorlaxC : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || !me.Has(SpecialCondition.Asleep) || me.DamageCounters == 0) return;
                Announce(src, "Good Sleep");
                src.Heal(me, me.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Collapse") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Bouncy Circle: 30 damage for each of your Benched Pokémon with a maximum HP of 30.</summary>
        private sealed class IgglybuffC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bouncy Circle") return false;
                int n = ctx.MyBench().Count(m => ctx.Engine.MaxHp(m) == 30);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Elemental Blast: discard a Fire, a Water and a Lightning Energy from this Pokémon.</summary>
        private sealed class LugiaC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Elemental Blast") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                foreach (var type in new[] { EnergyType.Fire, EnergyType.Water, EnergyType.Lightning })
                {
                    var energy = me.Energy.FirstOrDefault(e => ctx.Engine.EnergyUnitsOf(me, e).Any(u => EnergyMask.Provides(u, type)));
                    if (energy != null) ctx.Engine.DiscardEnergy(me, energy);
                }
                return true;
            }
        }

        /// <summary>Swirling Resentment: put damage counters on the opponent's Active Pokémon until its remaining HP is 50. Used for Swirling Resentment, Dastardly Jab (the attack name and the numbers are passed in).</summary>
        internal sealed class SwirlingResentmentMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SwirlingResentmentMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int counters = (ctx.RemainingHp(defending) - amount) / 10;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                return true;
            }
        }

        /// <summary>Shoot Meteors: discard all Energy from this Pokémon; 120 damage to 1 of your opponent's Pokémon.</summary>
        private sealed class MiniorC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shoot Meteors") return false;
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                ctx.DamageAnyOpponent(120);
                return true;
            }
        }

        /// <summary>Gnaw Together: flip a coin for each Maushold you have in play; for each heads, discard the top 2 cards of your opponent's deck.</summary>
        private sealed class MausholdC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gnaw Together") return false;
                int n = ctx.MyPokemon(m => m.Name == "Maushold").Count;
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2 * heads);
                return true;
            }
        }
    }
}
