using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Black Bolt (BLK): the cards that weren't already in the pool. Pokémon whose attacks only do their printed damage
    /// need no script: Snivy, Simisage, Darumaka, Simipour, Solosis, Drilbur, Timburr, Rufflet.
    /// </summary>
    public static class CardsBLK
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BLK-2", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("BLK-3", new SerperiorExBLK());
            r.RegisterByKey("BLK-4", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("BLK-6", new DodgeMon("Hide", false));
            r.RegisterByKey("BLK-7", new CardsEPO.BemusingAromaMon());
            r.RegisterByKey("BLK-8", new LivelyNeedlesMon("Lively Needles", 100));
            r.RegisterByKey("BLK-9", new CardsWHT.StimulatedEvolutionMon("Shelmet"));
            r.RegisterByKey("BLK-10", new ConditionAttackMon("Toxic Spore", SpecialCondition.Poisoned));
            r.RegisterByKey("BLK-11", new AmoongussBLK());
            r.RegisterByKey("BLK-12", new CardsNVI.VCreateMon("V-Force"));
            r.RegisterByKey("BLK-14", new DarmanitanBLK());
            r.RegisterByKey("BLK-15", new PeckOffMon("Peck Off"));
            r.RegisterByKey("BLK-16", new VolcaronaBLK());
            r.RegisterByKey("BLK-17", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("BLK-19", new RoundMon());
            r.RegisterByKey("BLK-20", new RoundMon());
            r.RegisterByKey("BLK-21", new RoundMon());
            r.RegisterByKey("BLK-22", new TirtougaBLK());
            r.RegisterByKey("BLK-23", new CarracostaBLK());
            r.RegisterByKey("BLK-24", new AlomomolaBLK());
            r.RegisterByKey("BLK-25", new NoAttackNextTurnMon("Snotted Up"));
            r.RegisterByKey("BLK-26", new BearticBLK());
            r.RegisterByKey("BLK-27", new CardsWHT.DragOffMon("Drag Off", 20));
            r.RegisterByKey("BLK-28", new KyuremExBLK());
            r.RegisterByKey("BLK-29", new CallForFamilyMon());
            r.RegisterByKey("BLK-30", new CardsTWM1.HealSelfMon("Hold Still", 10));
            r.RegisterByKey("BLK-31", new EelektrikBLK());
            r.RegisterByKey("BLK-32", new EelektrossBLK());
            r.RegisterByKey("BLK-33", new ThundurusBLK());
            r.RegisterByKey("BLK-34", new CardsWHT.BurstExMon("Voltage Burst", 30));
            r.RegisterByKey("BLK-35", new MunnaBLK());
            r.RegisterByKey("BLK-36", new MusharnaBLK());
            r.RegisterByKey("BLK-38", new DuosionBLK());
            r.RegisterByKey("BLK-39", new ReuniclusBLK());
            r.RegisterByKey("BLK-41", new BeheeyemBLK());
            r.RegisterByKey("BLK-42", new TailsNothingMon("Best Punch"));
            r.RegisterByKey("BLK-43", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("BLK-44", new MeloettaExBLK());
            r.RegisterByKey("BLK-46", new ExcadrillExBLK());
            r.RegisterByKey("BLK-48", new CardsSSP2.StompOffMon("Hammer Arm", 1));
            r.RegisterByKey("BLK-49", new ConkeldurrBLK());
            r.RegisterByKey("BLK-50", new ThrohBLK());
            r.RegisterByKey("BLK-51", new FlailMon("Flail"));
            r.RegisterByKey("BLK-52", new CrustleBLK());
            r.RegisterByKey("BLK-53", new LandorusBLK());
            r.RegisterByKey("BLK-54", new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned));
            r.RegisterByKey("BLK-55", new VenoshockMon(60));
            r.RegisterByKey("BLK-56", new ScolipedeBLK());
            r.RegisterByKey("BLK-57", new TightenUpMon(1));
            r.RegisterByKey("BLK-58", new TightenUpMon(2));
            r.RegisterByKey("BLK-59", new KrookodileBLK());
            r.RegisterByKey("BLK-60", new RecoilMon("Wild Lances", 30));
            r.RegisterByKey("BLK-61", new ShieldAttackMon("Hard Gears", 10));
            r.RegisterByKey("BLK-62", new ShieldAttackMon("Hard Gears", 20));
            r.RegisterByKey("BLK-63", new KlinklangBLK());
            r.RegisterByKey("BLK-64", new NoRetreatMon("Corner"));
            r.RegisterByKey("BLK-65", new FinishingBlowMon("Finishing Blow"));
            r.RegisterByKey("BLK-66", new CobalionBLK());
            r.RegisterByKey("BLK-68", new SearchAttackMon("Gather Strength", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("BLK-69", new LockAfterAttackMon("Boundless Power", false));
            r.RegisterByKey("BLK-70", new HaxorusBLK());
            r.RegisterByKey("BLK-71", new ScoutMon("Scout"));
            r.RegisterByKey("BLK-72", new DodgeMon("Fly", true));
            r.RegisterByKey("BLK-73", new UnfezantBLK());
            r.RegisterByKey("BLK-74", new ReturnMon());
            r.RegisterByKey("BLK-75", new CoinsMon("Tail Slap", 2));
            r.RegisterByKey("BLK-76", new CardsFCO3.DoTheWaveMon("Do the Wave", 20));
            r.RegisterByKey("BLK-78", new FlipBonusMon("Aerial Ace", 40));

            r.RegisterByName("Energy Coin", new EnergyCoin());
            r.RegisterByName("Fennel", new Fennel());
        }

        // ================================================================== Shared helpers

        /// <summary>Peck Off / Gnaw Through: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon.</summary>
        public sealed class PeckOffMon : Mon
        {
            private readonly string attack;

            public PeckOffMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var d = ctx.Defending;
                if (d != null && d.Tool != null && ctx.CanAffect(d)) ctx.Engine.DiscardTool(d);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Round: damage for each of your Pokémon in play that has the Round attack.</summary>
        public sealed class RoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Round") return false;
                int n = ctx.MyPokemon(m => m.Def.Attacks.Any(a => a.Name == "Round")).Count;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, the Defending Pokémon can't attack."</summary>
        public sealed class NoAttackNextTurnMon : Mon
        {
            private readonly string attack;

            public NoAttackNextTurnMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                DefendingCantAttack(ctx, atk);
                return true;
            }

            public static void DefendingCantAttack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (" + atk.Name + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
            }
        }

        /// <summary>Venoshock: N more damage if the opponent's Active Pokémon is Poisoned.</summary>
        public sealed class VenoshockMon : Mon
        {
            private readonly int bonus;

            public VenoshockMon(int bonus) { this.bonus = bonus; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Venoshock") return false;
                Venoshock(ctx, atk, bonus);
                return true;
            }

            public static void Venoshock(EffectContext ctx, AttackDef atk, int bonus)
            {
                bool poisoned = ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Poisoned);
                ctx.DamageDefending(atk.Damage + (poisoned ? bonus : 0));
            }
        }

        /// <summary>Tighten Up: the opponent discards N cards from their hand (they choose).</summary>
        public sealed class TightenUpMon : Mon
        {
            private readonly int count;

            public TightenUpMon(int count) { this.count = count; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tighten Up") return false;
                TightenUp(ctx, atk, count);
                return true;
            }

            public static void TightenUp(EffectContext ctx, AttackDef atk, int count)
            {
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                int n = Math.Min(count, hand.Count);
                if (n == 0) return;
                var picks = ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, atk.Name + ": discard " + n + (n == 1 ? " card" : " cards") + " from your hand", ctx.Source);
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, c);
            }
        }

        // ================================================================== Grass

        /// <summary>Regal Cheer: your Pokémon's attacks do 20 more to the opponent's Active. Command the Grass: you may search your deck for up to 3 cards.</summary>
        private sealed class SerperiorExBLK : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker == null || info.Attacker.Owner != src.Player || !info.ToOpponentsActive) return damage;
                return damage + 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Command the Grass") return false;
                ctx.DamageDefending(atk.Damage);
                // Compendium (Beak Catch, Brilliant Flare): searching is optional, but a search for any card takes at least 1.
                if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Command the Grass: search your deck for up to 3 cards?"))
                    ctx.SearchToHand(d => true, 3, "Command the Grass: choose up to 3 cards", false, 1);
                return true;
            }
        }

        /// <summary>Lively Needles: +100 if this Pokémon was healed during this turn. Used for Lively Needles, Lively Tackle (the attack name and the numbers are passed in).</summary>
        internal sealed class LivelyNeedlesMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public LivelyNeedlesMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.HealedOnTurn == ctx.Turn ? amount : 0));
                return true;
            }
        }

        /// <summary>Dangerous Reaction: +120 if the opponent's Active Pokémon has a Special Condition.</summary>
        private sealed class AmoongussBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dangerous Reaction") return false;
                bool affected = ctx.Defending != null && ctx.Defending.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage + (affected ? 120 : 0));
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Searing Flame: Burned. Smashing Headbutt: discard 2 Energy from it.</summary>
        private sealed class DarmanitanBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Searing Flame")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    return true;
                }
                if (atk.Name == "Smashing Headbutt")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Torrid Scales: discard a Basic Fire Energy from your hand to Burn the opponent's Active Pokémon (once during your turn).</summary>
        private sealed class VolcaronaBLK : Mon
        {
            protected override string Activated { get { return "Torrid Scales"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Active != null && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
            }

            protected override void Ability(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                var pick = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Torrid Scales: discard a Basic Fire Energy")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Burned);
            }
        }

        // ================================================================== Water

        /// <summary>Ancient Seaweed: 30 for each Item card in the opponent's discard pile.</summary>
        private sealed class TirtougaBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ancient Seaweed") return false;
                ctx.DamageDefending(atk.Damage * ctx.Opp.Discard.Count(c => c.Def.IsItem));
                return true;
            }
        }

        /// <summary>Mighty Shell: prevent damage and effects of attacks from the opponent's Pokémon with Special Energy attached. Big Bite: the Defending Pokémon can't retreat.</summary>
        private sealed class CarracostaBLK : Mon
        {
            private static bool HasSpecial(PokemonInPlay mon) { return mon != null && mon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy); }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && HasSpecial(info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && HasSpecial(attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Gentle Fin: once during your turn, while Active, put a Basic Pokémon with 70 HP or less from your discard pile onto your Bench.</summary>
        private sealed class AlomomolaBLK : Mon
        {
            protected override string Activated { get { return "Gentle Fin"; } }

            private static List<CardInstance> Choices(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && c.Def.Hp <= 70).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.BenchSpace > 0 && Choices(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(Choices(ctx), 1, 1, "Gentle Fin: put which Basic Pokémon onto your Bench?")[0];
                ctx.Me.Discard.Remove(pick);
                ctx.Engine.PutOntoBench(ctx.Player, pick, false);
            }
        }

        /// <summary>Continuous Headbutt: flip until tails, 50 per heads. Sheer Cold: the Defending Pokémon can't attack next turn.</summary>
        private sealed class BearticBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Continuous Headbutt")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                    return true;
                }
                if (atk.Name == "Sheer Cold")
                {
                    NoAttackNextTurnMon.DefendingCantAttack(ctx, atk);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Blizzard Burst: also 10 to each of the opponent's Benched Pokémon for each Prize card they've taken.</summary>
        private sealed class KyuremExBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blizzard Burst") return false;
                ctx.DamageDefending(atk.Damage);
                int bench = 10 * ctx.OpponentPrizesTaken;
                if (bench > 0)
                    foreach (var m in ctx.OpponentBench()) ctx.DamageTarget(m, bench);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Dynamotor: once during your turn, attach a Lightning Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class EelektrikBLK : Mon
        {
            protected override string Activated { get { return "Dynamotor"; } }

            private static List<CardInstance> Choices(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && Choices(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var energy = Choices(ctx);
                var pick = ctx.ChooseCards(energy, 1, 1, "Dynamotor: attach which Lightning Energy from your discard pile?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Dynamotor: attach " + pick.Def.Name + " to which Benched Pokémon?");
                if (target == null) return;
                ctx.Me.Discard.Remove(pick);
                ctx.Engine.AttachEnergyCard(pick, target);
            }
        }

        /// <summary>Thunder Fang: flip, Paralyzed. Buzz Flip: 4 coins, 100 per heads.</summary>
        private sealed class EelektrossBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Thunder Fang")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Buzz Flip")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(4));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Charge: attach a Basic Lightning Energy from your deck to it. Disaster Volt: discard an Energy from it.</summary>
        private sealed class ThundurusBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Charge")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 1, "Charge: search your deck for a Basic Lightning Energy card");
                    foreach (var e in found) ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Disaster Volt")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Psychic

        /// <summary>Rest: this Pokémon is now Asleep; heal 30 from it.</summary>
        private sealed class MunnaBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        /// <summary>Dream Calling: you may search your deck for any number of Fennel. Sleep Pulse: Asleep.</summary>
        private sealed class MusharnaBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Dream Calling")
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.Name == "Fennel", ctx.Me.Deck.Count, "Dream Calling: search your deck for any number of Fennel cards");
                    return true;
                }
                if (atk.Name == "Sleep Pulse")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Cellular Evolution: evolve 1 of your Pokémon with a card from your deck.</summary>
        private sealed class DuosionBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cellular Evolution") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var targets = ctx.MyPokemon(m => ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.EvolvesFrom == m.Name));
                if (targets.Count > 0)
                {
                    var target = ctx.ChoosePokemon(targets, "Cellular Evolution: evolve which of your Pokémon?");
                    CardsSSP1.EvolveFromDeck(ctx, target, "Cellular Evolution: choose a card that evolves from " + target.Name);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Cellular Ascension: for each of your Benched Pokémon, evolve it with a card from your deck. Evo-Lariat: +40 per Evolution Pokémon you have in play.</summary>
        private sealed class ReuniclusBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cellular Ascension")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    foreach (var m in ctx.MyBench())
                        if (ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.EvolvesFrom == m.Name))
                            CardsSSP1.EvolveFromDeck(ctx, m, "Cellular Ascension: choose a card that evolves from " + m.Name);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Evo-Lariat")
                {
                    ctx.DamageDefending(atk.Damage + 40 * ctx.MyPokemon(m => m.Def.IsEvolution).Count);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Calm Mind: heal 40. Psychic: +30 per Energy on the opponent's Active Pokémon.</summary>
        private sealed class BeheeyemBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Calm Mind")
                {
                    ctx.Heal(ctx.Pokemon, 40);
                    return true;
                }
                if (atk.Name == "Psychic")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Defending == null ? 0 : 30 * ctx.EnergyCount(ctx.Defending)));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Debut Performance: if you go first, it can attack on your first turn. Echoed Voice: next turn, Echoed Voice does 80 more.</summary>
        private sealed class MeloettaExBLK : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex)
            {
                return ctx.Pokemon != null && ctx.Pokemon.Def.Abilities.Any(a => a.Name == "Debut Performance")
                       && !ctx.Engine.AbilityBlocked(ctx.Pokemon, "Debut Performance");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Echoed Voice") return false;
                int uid = ctx.Pokemon.Uid;
                int bonus = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>()
                    .Where(e => e.TargetUid == uid && e.AttackName == "Echoed Voice").Sum(e => e.Bonus);
                ctx.DamageDefending(atk.Damage + bonus);
                if (ctx.InPlay(ctx.Pokemon))
                {
                    var boost = new AttackBoostEffect(ctx.Pokemon.Name + "'s Echoed Voice does 80 more damage during its next turn", ctx.Player, uid, ctx.Turn + 2, "Echoed Voice", 80);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                }
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Piercing Drill: also 60 to 1 Benched Pokémon with damage counters. Rock Tumble: Resistance doesn't apply.</summary>
        private sealed class ExcadrillExBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Piercing Drill")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageOneBenched(60, m => m.DamageCounters > 0);
                    return true;
                }
                if (atk.Name == "Rock Tumble")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Craftsmanship: +40 HP for each Fighting Energy attached. Swing Around: 2 coins, +50 per heads.</summary>
        private sealed class ConkeldurrBLK : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return hp;
                return hp + 40 * src.Engine.EnergyCountOfType(mon, EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swing Around") return false;
                ctx.DamageDefending(atk.Damage + 50 * ctx.FlipCoins(2));
                return true;
            }
        }

        /// <summary>Shoulder Throw: 30 less for each Colorless in the opponent's Active Pokémon's Retreat Cost.</summary>
        private sealed class ThrohBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shoulder Throw") return false;
                int retreat = ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending);
                ctx.DamageDefending(Math.Max(0, atk.Damage - 30 * retreat));
                return true;
            }
        }

        /// <summary>Flail: 10 for each damage counter on it. Used for Flail, Reprisal, Get Angry, Balloon Blast (the attack name is passed in).</summary>
        internal sealed class FlailMon : Mon
        {
            private readonly string attack;

            public FlailMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>Sturdy: at full HP, it survives a Knock Out from attack damage with 10 HP. Stone Edge: flip, +60.</summary>
        private sealed class CrustleBLK : Mon
        {
            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stone Edge") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 60 : 0));
                return true;
            }
        }

        /// <summary>Abundant Harvest: attach a Basic Fighting Energy from your discard pile to it. Earthquake: 10 to each of your Benched Pokémon.</summary>
        private sealed class LandorusBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Abundant Harvest")
                {
                    var fighting = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                    if (fighting.Count == 0) return true;
                    var pick = ctx.ChooseCards(fighting, 1, 1, "Abundant Harvest: attach which Basic Fighting Energy from your discard pile?")[0];
                    ctx.Me.Discard.Remove(pick);
                    ctx.Engine.AttachEnergyCard(pick, ctx.Pokemon);
                    return true;
                }
                if (atk.Name == "Earthquake")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var m in ctx.MyBench()) ctx.DamageTarget(m, 10);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Darkness

        /// <summary>Poison Point: when it's damaged by an opponent's attack while Active, the Attacking Pokémon is Poisoned. Venoshock: +90 against a Poisoned Pokémon.</summary>
        private sealed class ScolipedeBLK : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || !ctx.State.IsActive(attacker)) return;
                Announce(ctx, "Poison Point");
                ctx.Engine.ApplyCondition(attacker, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Venoshock") return false;
                VenoshockMon.Venoshock(ctx, atk, 90);
                return true;
            }
        }

        /// <summary>Tighten Up: the opponent discards 2 cards. Cursed Slug: +120 if they have 3 or fewer cards in hand.</summary>
        private sealed class KrookodileBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tighten Up")
                {
                    TightenUpMon.TightenUp(ctx, atk, 2);
                    return true;
                }
                if (atk.Name == "Cursed Slug")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Opp.Hand.Count <= 3 ? 120 : 0));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal / Dragon

        /// <summary>Gear Coating: your Pokémon with Metal Energy attached take 20 less damage from the opponent's attacks.</summary>
        private sealed class KlinklangBLK : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                return src.Engine.HasEnergyOfType(info.Target, EnergyType.Metal) ? damage - 20 : damage;
            }
        }

        /// <summary>Finishing Blow: +60 if the opponent's Active Pokémon already has damage counters. Used for Finishing Blow, Horn Rend (the attack name is passed in).</summary>
        internal sealed class FinishingBlowMon : Mon
        {
            private readonly string attack;

            public FinishingBlowMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool hurt = ctx.Defending != null && ctx.Defending.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage + (hurt ? 60 : 0));
                return true;
            }
        }

        /// <summary>Righteous Edge: discard a Special Energy from the opponent's Active Pokémon. Metal Arms: +40 with a Pokémon Tool attached.</summary>
        private sealed class CobalionBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Righteous Edge")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                        ctx.DiscardEnergyFrom(defending, 1, "Righteous Edge: discard a Special Energy from " + defending.Name, e => e.Def.IsAttachedSpecialEnergy);
                    return true;
                }
                if (atk.Name == "Metal Arms")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Tool != null ? 40 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Cross-Cut: +80 against an Evolution Pokémon. Axe Blast: Knocks Out a Basic Active Pokémon.</summary>
        private sealed class HaxorusBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var d = ctx.Defending;
                if (atk.Name == "Cross-Cut")
                {
                    ctx.DamageDefending(atk.Damage + (d != null && d.Def.IsEvolution ? 80 : 0));
                    return true;
                }
                if (atk.Name == "Axe Blast")
                {
                    if (d != null && d.Def.IsBasicPokemon) ctx.Engine.KnockOut(d, atk.Name);
                    else Nothing(ctx);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Scout: the opponent reveals their hand. Used for Scout, See Through (the attack name is passed in).</summary>
        internal sealed class ScoutMon : Mon
        {
            private readonly string attack;

            public ScoutMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                CardsTWM1.RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Add On: draw 4. Swift Flight: flip; heads, prevent damage and effects next turn.</summary>
        private sealed class UnfezantBLK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Add On")
                {
                    ctx.Draw(4);
                    return true;
                }
                if (atk.Name == "Swift Flight")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip()) ctx.ProtectSelfNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Return: you may draw until you have 6 cards in hand.</summary>
        internal sealed class ReturnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Return") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Return: draw cards until you have 6 cards in your hand?"))
                    ctx.DrawUntil(6);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Flip 2 coins; if both are heads, attach a Basic Energy from your deck to 1 of your Pokémon.</summary>
        private sealed class EnergyCoin : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.FlipCoins(2) < 2 || ctx.Me.Deck.Count == 0) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Energy Coin: search your deck for a Basic Energy card");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Energy Coin: attach " + found[0].Def.Name + " to which of your Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(found[0], target);
                    else ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Heal 40 damage from each of your Pokémon.</summary>
        private sealed class Fennel : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 40);
            }
        }
    }
}
