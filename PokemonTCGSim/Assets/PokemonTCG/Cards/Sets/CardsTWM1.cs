using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Twilight Masquerade (TWM) 1-75. Pokémon whose attacks only do their printed damage need no script:
    /// Tangela, Spinarak, Vulpix, Corphish, Tadbulb.
    /// </summary>
    public static class CardsTWM1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TWM-2", new TangrowthTWM());
            r.RegisterByKey("TWM-3", new PinsirTWM());
            r.RegisterByKey("TWM-5", new AriadosTWM());
            r.RegisterByKey("TWM-6", new CoinsMon("Bullet Seed", 4));
            r.RegisterByKey("TWM-7", new SunfloraTWM());
            r.RegisterByKey("TWM-8", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("TWM-9", new VolbeatTWM());
            r.RegisterByKey("TWM-10", new IllumiseTWM());
            r.RegisterByKey("TWM-11", new LeafeonTWM());
            r.RegisterByKey("TWM-12", new HealSelfMon("Leech Seed", 20));
            r.RegisterByKey("TWM-13", new CardsDAA1.DrainMon("Giga Drain"));
            r.RegisterByKey("TWM-16", new RillaboomTWM());
            r.RegisterByKey("TWM-19", new IronLeavesTWM());
            r.RegisterByKey("TWM-20", new PoltchageistHideawayTWM());
            r.RegisterByKey("TWM-21", new PoltchageistTeaTWM());
            r.RegisterByKey("TWM-22", new SinistchaTWM());
            r.RegisterByKey("TWM-23", new SinistchaExTWM());
            r.RegisterByKey("TWM-24", new TealMaskOgerponTWM());
            r.RegisterByKey("TWM-27", new NinetalesTWM());
            r.RegisterByKey("TWM-28", new ConditionAttackMon("Hot Magma", SpecialCondition.Burned));
            r.RegisterByKey("TWM-29", new MagcargoExTWM());
            r.RegisterByKey("TWM-30", new TorkoalTWM());
            r.RegisterByKey("TWM-31", new FlipBonusMon("Firebreathing", 20));
            r.RegisterByKey("TWM-32", new SelfDiscardMon("Heat Blow", 1));
            r.RegisterByKey("TWM-33", new InfernapeTWM());
            r.RegisterByKey("TWM-34", new RecoilMon("Fire Tackle", 20));
            r.RegisterByKey("TWM-35", new RecoilMon("Inferno Onrush", 70));
            r.RegisterByKey("TWM-36", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("TWM-37", new SelfDiscardMon("Burn It All Up", 99));
            r.RegisterByKey("TWM-38", new ChandelureTWM());
            r.RegisterByKey("TWM-40", new HearthflameMaskOgerponExTWM());
            r.RegisterByKey("TWM-41", new CoinsMon("Tail Rap", 2));
            r.RegisterByKey("TWM-42", new PoliwhirlTWM());
            r.RegisterByKey("TWM-43", new PoliwrathTWM());
            r.RegisterByKey("TWM-45", new CardsDRI2.GnawThroughMon("Peck Off"));
            r.RegisterByKey("TWM-46", new JynxTWM());
            r.RegisterByKey("TWM-48", new CrawdauntTWM());
            r.RegisterByKey("TWM-49", new CardsBLK.FlailMon("Flail"));
            r.RegisterByKey("TWM-50", new MiloticTWM());
            r.RegisterByKey("TWM-51", new SnoruntTWM());
            r.RegisterByKey("TWM-52", new GlalieTWM());
            r.RegisterByKey("TWM-53", new FroslassTWM());
            r.RegisterByKey("TWM-54", new GlaceonTWM());
            r.RegisterByKey("TWM-55", new PhioneTWM());
            r.RegisterByKey("TWM-56", new FroakieTWM());
            r.RegisterByKey("TWM-57", new ParalyzeFlipMon("Numbing Water"));
            r.RegisterByKey("TWM-58", new SpitShotMon("Spit Shot"));
            r.RegisterByKey("TWM-59", new LockAfterAttackMon("Aqua Slash", false));
            r.RegisterByKey("TWM-60", new PalafinTWM());
            r.RegisterByKey("TWM-61", new PalafinExTWM());
            r.RegisterByKey("TWM-62", new CardsFST2.RapidSpinMon("Interjet"));
            r.RegisterByKey("TWM-63", new WalkingWakeTWM());
            r.RegisterByKey("TWM-65", new ZapdosTWM());
            r.RegisterByKey("TWM-66", new ShinxTWM());
            r.RegisterByKey("TWM-67", new NoRetreatMon("Big Bite"));
            r.RegisterByKey("TWM-68", new LuxrayExTWM());
            r.RegisterByKey("TWM-69", new EmolgaTWM());
            r.RegisterByKey("TWM-70", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("TWM-71", new RecoilMon("Wild Charge", 20));
            r.RegisterByKey("TWM-72", new MorpekoTWM());
            r.RegisterByKey("TWM-74", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("TWM-75", new ParalyzeFlipMon("Thunder Shock"));
        }

        // ================================================================== Shared helpers (also used by CardsTWM2)

        /// <summary>The opponent shows their hand to both players.</summary>
        public static void RevealOpponentHand(EffectContext ctx)
        {
            var hand = ctx.Opp.Hand.ToList();
            if (hand.Count == 0) ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed an empty hand.");
            else ctx.Engine.Reveal(ctx.Opponent, hand, "revealed " + ctx.Engine.Their(ctx.Opponent) + " hand");
        }

        /// <summary>Random cards from the opponent's hand are revealed and shuffled into their deck (Astonish, Wicked Tail).</summary>
        public static void ShuffleRandomFromOpponentHand(EffectContext ctx, int count)
        {
            var hand = ctx.Opp.Hand;
            var picked = new List<CardInstance>();
            for (int i = 0; i < count && hand.Count > 0; i++)
            {
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                hand.Remove(card);
                picked.Add(card);
            }
            if (picked.Count == 0) return;
            ctx.Engine.Reveal(ctx.Opponent, picked, "revealed and shuffled into " + ctx.Engine.Their(ctx.Opponent) + " deck");
            ctx.Opp.Deck.AddRange(picked);
            ctx.Engine.ShuffleDeck(ctx.Opponent);
        }

        /// <summary>Discards the Stadium in play, if there is one.</summary>
        public static bool DiscardStadiumInPlay(EffectContext ctx)
        {
            if (ctx.State.Stadium == null) return false;
            ctx.Engine.DiscardStadium();
            return true;
        }

        /// <summary>Puts a card from a deck in place of a Pokémon's top card; damage, attachments and effects stay (Zero to Hero, Ogre's Mask).</summary>
        public static void ReplaceTopCard(EffectContext ctx, PokemonInPlay mon, CardInstance replacement, List<CardInstance> putOldInto)
        {
            var old = mon.Top;
            mon.Stack[mon.Stack.Count - 1] = replacement;
            putOldInto.Add(old);
            ctx.Engine.Emit(GameEventType.Evolve, mon.Owner, old.Def.Name + " was switched with " + replacement.Def.Name + ".", replacement.Id, mon.Uid);
        }

        /// <summary>"At the end of your opponent's next turn, the Defending Pokémon is Knocked Out / gets N damage counters."</summary>
        public sealed class DelayedDefendingEffect : ContinuousEffect
        {
            private readonly int counters;
            private readonly string attack;

            /// <param name="counters">Damage counters to place, or 0 to Knock it Out.</param>
            public DelayedDefendingEffect(string name, int owner, int targetUid, int currentTurn, string attack, int counters)
                : base(name, owner, targetUid, currentTurn + 1, true)
            {
                this.attack = attack;
                this.counters = counters;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon == null) return;
                if (counters <= 0) engine.KnockOut(mon, attack);
                else engine.PlaceDamageCounters(mon, counters, attack);
            }
        }

        // ================================================================== Shared simple attacks

        /// <summary>"Heal N damage from this Pokémon" after the damage.</summary>
        public sealed class HealSelfMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealSelfMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, amount);
                return true;
            }
        }

        /// <summary>"Discard N Energy from this Pokémon" (99 = all) after the damage.</summary>
        public sealed class SelfDiscardMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public SelfDiscardMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                DiscardOwn(ctx, count, atk.Name);
                return true;
            }

            public static void DiscardOwn(EffectContext ctx, int count, string attackName)
            {
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return;
                if (count >= 99) ctx.Engine.DiscardAllEnergy(me);
                else ctx.DiscardEnergyFrom(me, Math.Min(count, me.Energy.Count),
                    attackName + ": discard " + (count == 1 ? "an Energy" : count + " Energy") + " from " + me.Name);
            }
        }

        /// <summary>Spit Shot / Drop Shot: discard all Energy from this Pokémon, then 120 damage to 1 of the opponent's Pokémon.</summary>
        public sealed class SpitShotMon : Mon
        {
            private readonly string attack;

            public SpitShotMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                ctx.DamageAnyOpponent(120);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Thicket Body: takes 30 less damage from attacks. Loom Over: 10 less for each damage counter on this Pokémon.</summary>
        private sealed class TangrowthTWM : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Loom Over") return false;
                ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        /// <summary>Slow Crunch: discard all Energy from this Pokémon; at the end of the opponent's next turn the Defending Pokémon is Knocked Out.</summary>
        private sealed class PinsirTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slow Crunch") return false;
                var defending = ctx.Defending;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                if (defending == null || !ctx.InPlay(defending)) return true;
                ctx.AddEffect(new DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of its next turn (Slow Crunch)",
                    ctx.Player, defending.Uid, ctx.Turn, "Slow Crunch", 0));
                return true;
            }
        }

        /// <summary>Big Net: the opponent's Active Evolution Pokémon's Retreat Cost is 1 more. String Bind: +30 per Colorless in its Retreat Cost.</summary>
        private sealed class AriadosTWM : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon.Owner == src.Player || src.Opp.Active != mon || !mon.Def.IsEvolution) return cost;
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "String Bind") return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                ctx.DamageDefending(atk.Damage + 30 * cost);
                return true;
            }
        }

        /// <summary>Redirected Sunlight: 60 per Fire Energy on all the opponent's Pokémon. Mega Drain: heal 30.</summary>
        private sealed class SunfloraTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Redirected Sunlight")
                {
                    int fire = ctx.OpponentPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Fire));
                    ctx.DamageDefending(atk.Damage * fire);
                    return true;
                }
                if (atk.Name == "Mega Drain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Quick Sign: usable on the first player's first turn; up to 2 Basic Pokémon onto the Bench. Coordinated Strike: +60 with Illumise on the Bench.</summary>
        private sealed class VolbeatTWM : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name == "Quick Sign";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Quick Sign")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => true, 2, "Quick Sign: put up to 2 Basic Pokémon onto your Bench");
                    return true;
                }
                if (atk.Name == "Coordinated Strike")
                {
                    bool illumise = ctx.Me.Bench.Any(b => b.Name == "Illumise");
                    ctx.DamageDefending(atk.Damage + (illumise ? 60 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Slowing Perfume: only on your first turn if you went second; shuffle 1 of the opponent's Benched Pokémon into their deck.</summary>
        private sealed class IllumiseTWM : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Slowing Perfume") return true;
                return ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slowing Perfume") return false;
                var targets = ctx.GustTargets();
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Slowing Perfume: shuffle which of your opponent's Benched Pokémon into their deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Leaflet Blessings: attach a Basic Grass Energy from your hand to 1 of your Benched Pokémon; if you did, heal all its damage.</summary>
        private sealed class LeafeonTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaflet Blessings") return false;
                var grass = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Grass).ToList();
                if (grass.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(grass, 1, 1, "Leaflet Blessings: choose a Basic Grass Energy to attach")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Leaflet Blessings: attach it to which Benched Pokémon?");
                if (target == null) return true;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
                if (target.DamageCounters > 0) ctx.Heal(target, target.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Drum Beating: the Defending Pokémon's attacks and Retreat Cost cost 1 Colorless more next turn.</summary>
        private sealed class DrumBeatingEffect : ContinuousEffect
        {
            public DrumBeatingEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon.Uid == TargetUid) cost.Add(EnergyType.Colorless);
            }

            public override int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost)
            {
                return mon.Uid == TargetUid ? cost + 1 : cost;
            }
        }

        private sealed class RillaboomTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Drum Beating")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                        ctx.AddEffect(new DrumBeatingEffect(defending.Name + "'s attacks and Retreat Cost cost 1 more during its next turn (Drum Beating)",
                            ctx.Player, defending.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Wood Hammer")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(50);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Recovery Net: up to 2 Pokémon from your discard pile into your hand. Avenging Edge: +60 after a Knock Out.</summary>
        private sealed class IronLeavesTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Recovery Net")
                {
                    var mons = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                    if (mons.Count > 0)
                        ctx.DiscardToHand(ctx.ChooseCards(mons, 1, Math.Min(2, mons.Count), "Recovery Net: put up to 2 Pokémon into your hand"));
                    return true;
                }
                if (atk.Name == "Avenging Edge")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutByAttackLastOpponentTurn() ? 60 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Storehouse Hideaway: on the Bench, prevent all damage and effects of the opponent's attacks done to this Pokémon.</summary>
        private sealed class PoltchageistHideawayTWM : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && !src.PokemonIsActive;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && !src.PokemonIsActive;
            }
        }

        /// <summary>Tea Server: put a Basic Grass Energy from your discard pile into your hand. Surprise Attack: tails does nothing.</summary>
        private sealed class PoltchageistTeaTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tea Server")
                {
                    var grass = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Grass);
                    if (grass.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(grass, 1, 1, "Tea Server: choose a Basic Grass Energy"));
                    return true;
                }
                if (atk.Name == "Surprise Attack")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Cursed Drop: 4 damage counters on the opponent's Pokémon any way you like. Spill the Tea: discard up to 3 Grass Energy from your Pokémon, 70 each.</summary>
        private sealed class SinistchaTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cursed Drop")
                {
                    ctx.PlaceCountersAnyWay(4, () => ctx.OpponentPokemon());
                    return true;
                }
                if (atk.Name == "Spill the Tea")
                {
                    var holders = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var m in ctx.MyPokemon())
                        foreach (var e in m.Energy)
                            if (e.Def.IsEnergy && e.Def.EnergyProvides == EnergyType.Grass) holders[e] = m;
                    int discarded = 0;
                    if (holders.Count > 0)
                    {
                        var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, Math.Min(3, holders.Count),
                            "Spill the Tea: discard up to 3 Grass Energy from your Pokémon (70 damage each)");
                        foreach (var e in picks)
                            if (ctx.Engine.DiscardEnergy(holders[e], e)) discarded++;
                    }
                    ctx.DamageDefending(atk.Damage * discarded);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Re-Brew: 2 damage counters per Basic Grass Energy in your discard pile on 1 opposing Pokémon, then shuffle them into your deck. Matcha Splash: heal 30 from each of your Pokémon.</summary>
        private sealed class SinistchaExTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Re-Brew")
                {
                    var grass = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Grass);
                    if (grass.Count == 0) return true;
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Re-Brew: put " + 2 * grass.Count + " damage counters on which of your opponent's Pokémon?");
                    // Compendium: if no damage counters are placed (prevented), the Energy isn't shuffled in either.
                    int placed = target != null ? ctx.Engine.PlaceDamageCounters(target, 2 * grass.Count, "Re-Brew") : 0;
                    if (placed == 0) return true;
                    foreach (var c in grass) ctx.Me.Discard.Remove(c);
                    ctx.Me.Deck.AddRange(grass);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + grass.Count + " Basic Grass Energy into " + ctx.Engine.Their(ctx.Player) + " deck.");
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Matcha Splash")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Mountain Stroll: up to 2 Basic Energy from your deck into your hand. Ogre Comeback: +20 per opposing Benched Pokémon.</summary>
        private sealed class TealMaskOgerponTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mountain Stroll")
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsBasicEnergy, 2, "Mountain Stroll: choose up to 2 Basic Energy cards");
                    return true;
                }
                if (atk.Name == "Ogre Comeback")
                {
                    ctx.DamageDefending(atk.Damage + 20 * ctx.Opp.Bench.Count);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        private sealed class NinetalesTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eerie Glow") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Hot Magma: Burned. Ground Burn: discard the top card of each deck; +140 for each Energy card discarded.</summary>
        private sealed class MagcargoExTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hot Magma")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    return true;
                }
                if (atk.Name == "Ground Burn")
                {
                    var mine = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                    var theirs = ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                    int energy = mine.Count(c => c.Def.IsEnergy) + theirs.Count(c => c.Def.IsEnergy);
                    ctx.DamageDefending(atk.Damage + 140 * energy);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Ramming Shell: takes 30 less next turn. Fire Spin: discard 2 Energy from this Pokémon.</summary>
        private sealed class TorkoalTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Ramming Shell")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfTakesLessNextTurn(30);
                    return true;
                }
                if (atk.Name == "Fire Spin")
                {
                    ctx.DamageDefending(atk.Damage);
                    SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Pyro Dance: once during your turn, attach a Basic Fire, a Basic Fighting, or 1 of each from your hand to your Pokémon in any way you like.</summary>
        private sealed class InfernapeTWM : Mon
        {
            protected override string Activated { get { return "Pyro Dance"; } }

            private static bool IsBasic(CardInstance c, EnergyType type) { return c.Def.IsBasicEnergy && c.Def.EnergyProvides == type; }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => IsBasic(c, EnergyType.Fire) || IsBasic(c, EnergyType.Fighting));
            }

            protected override void Ability(EffectContext ctx)
            {
                var first = ctx.Me.Hand.Where(c => IsBasic(c, EnergyType.Fire) || IsBasic(c, EnergyType.Fighting)).ToList();
                if (first.Count == 0) return;
                var card = ctx.ChooseCards(first, 1, 1, "Pyro Dance: choose a Basic Fire or Basic Fighting Energy to attach")[0];
                Attach(ctx, card);
                var otherType = IsBasic(card, EnergyType.Fire) ? EnergyType.Fighting : EnergyType.Fire;
                var second = ctx.Me.Hand.Where(c => IsBasic(c, otherType)).ToList();
                if (second.Count == 0) return;
                var more = ctx.ChooseCards(second, 0, 1, "Pyro Dance: also attach a Basic " + otherType + " Energy? (choose none to stop)");
                if (more.Count > 0) Attach(ctx, more[0]);
            }

            private static void Attach(EffectContext ctx, CardInstance card)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Pyro Dance: attach " + card.Def.Name + " to which Pokémon?");
                if (target == null) return;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Fire") return false;
                ctx.DamageDefending(atk.Damage);
                SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>Alluring Light: once during your turn, each player draws a card. Mind Ruler: 30 per card in the opponent's hand.</summary>
        private sealed class ChandelureTWM : Mon
        {
            protected override string Activated { get { return "Alluring Light"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Draw(1);
                ctx.Engine.DrawCards(ctx.Opponent, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Ruler") return false;
                ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Wrathful Hearth: 20 per damage counter on this Pokémon. Dynamic Blaze: +140 against an Evolution Pokémon, then discard all Energy from this Pokémon.</summary>
        private sealed class HearthflameMaskOgerponExTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Wrathful Hearth")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Pokemon.DamageCounters);
                    return true;
                }
                if (atk.Name == "Dynamic Blaze")
                {
                    var defending = ctx.Defending;
                    bool evolved = defending != null && defending.Def.IsEvolution;
                    ctx.DamageDefending(atk.Damage + (evolved ? 140 : 0));
                    if (evolved && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        private sealed class PoliwhirlTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hypnosis")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name == "Double Slap")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(2));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hypnosis: Asleep. Jumping Uppercut: you may do 120 more; if you do, shuffle this Pokémon and everything attached into your deck.</summary>
        private sealed class PoliwrathTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hypnosis")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name == "Jumping Uppercut")
                {
                    bool more = ctx.AskYesNo("Jumping Uppercut: do 120 more damage and shuffle " + ctx.Pokemon.Name + " and all attached cards into your deck?");
                    ctx.DamageDefending(atk.Damage + (more ? 120 : 0));
                    if (more && ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Inviting Kiss: put a Basic Pokémon from your deck onto your Bench; if you did, move an Energy from this Pokémon to it.</summary>
        private sealed class JynxTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inviting Kiss") return false;
                if (ctx.BenchSpace == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchToBench(d => true, 1, "Inviting Kiss: choose a Basic Pokémon to put onto your Bench");
                if (found.Count == 0) return true;
                var benched = ctx.Me.Bench.FirstOrDefault(b => b.Top == found[0]);
                var me = ctx.Pokemon;
                if (benched == null || me.Energy.Count == 0) return true;
                var energy = me.Energy.Count == 1 ? me.Energy[0]
                    : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Inviting Kiss: move which Energy to " + benched.Name + "?")[0];
                ctx.Engine.MoveEnergy(me, benched, energy);
                return true;
            }
        }

        /// <summary>Snip Snip: flip 2 coins; discard the top card of the opponent's deck per heads. Rampaging Hammer: can't attack next turn.</summary>
        private sealed class CrawdauntTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Snip Snip")
                {
                    ctx.DamageDefending(atk.Damage);
                    int heads = ctx.FlipCoins(2);
                    if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                    return true;
                }
                if (atk.Name == "Rampaging Hammer")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Mentally Calm: the opponent's Pokémon in play and their attached cards can't be put into the opponent's hand.</summary>
        private sealed class MiloticTWM : Mon
        {
            public override bool KeepsOutOfHand(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon.Owner != src.Player;
            }
        }

        /// <summary>Astonish: a random card from the opponent's hand is revealed and shuffled into their deck.</summary>
        private sealed class SnoruntTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Astonish") return false;
                ctx.DamageDefending(atk.Damage);
                ShuffleRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        /// <summary>Damage Beat: 20 per damage counter on the opponent's Active Pokémon. Crazy Headbutt: discard an Energy from this Pokémon.</summary>
        private sealed class GlalieTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Damage Beat")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage * (defending == null ? 0 : defending.DamageCounters));
                    return true;
                }
                if (atk.Name == "Crazy Headbutt")
                {
                    ctx.DamageDefending(atk.Damage);
                    SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Freezing Shroud: during Pokémon Checkup, 1 damage counter on each Pokémon (both sides) that has an Ability, except Froslass.</summary>
        private sealed class FroslassTWM : Mon
        {
            public override int CheckupAffects(EffectContext src) { return -1; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null) return;
                var targets = new List<PokemonInPlay>();
                foreach (var p in new[] { src.Me, src.Opp })
                    foreach (var m in p.InPlay())
                        if (m.Name != "Froslass" && src.Engine.HasAbility(m)) targets.Add(m);
                foreach (var m in targets)
                    if (src.InPlay(m)) src.Engine.PlaceDamageCounters(m, 1, "Freezing Shroud");
            }
        }

        /// <summary>Permeating Chill: at the end of the opponent's next turn, 9 damage counters on the Defending Pokémon.</summary>
        private sealed class GlaceonTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Permeating Chill") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new DelayedDefendingEffect(defending.Name + " gets 9 damage counters at the end of its next turn (Permeating Chill)",
                        ctx.Player, defending.Uid, ctx.Turn, "Permeating Chill", 9));
                return true;
            }
        }

        /// <summary>Beckon: a Supporter from your discard pile into your hand. Energy Press: 20 per Energy on the opponent's Active Pokémon.</summary>
        private sealed class PhioneTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Beckon")
                {
                    var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                    if (supporters.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(supporters, 1, 1, "Beckon: choose a Supporter to put into your hand"));
                    return true;
                }
                if (atk.Name == "Energy Press")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage * (defending == null ? 0 : ctx.EnergyCount(defending)));
                    return true;
                }
                return false;
            }
        }

        private sealed class FroakieTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flock") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.Name == "Froakie", 2, "Flock: put up to 2 Froakie onto your Bench");
                return true;
            }
        }

        /// <summary>Zero to Hero: when this Pokémon moves from the Active Spot to the Bench during your turn, you may switch it with a Palafin ex from your deck.</summary>
        private sealed class PalafinTWM : Mon
        {
            public override void OnMovedToBenchFromActive(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (!TriggerOnce(ctx, "Zero to Hero", ctx.Me.Deck.Count > 0, "Zero to Hero: search your deck for a Palafin ex and switch it with " + mon.Name + "?"))
                    return;
                var found = ctx.SearchMyDeck(d => d.Name == "Palafin ex", 1, "Zero to Hero: choose a Palafin ex");
                if (found.Count > 0 && ctx.InPlay(mon)) ReplaceTopCard(ctx, mon, found[0], ctx.Me.Deck);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Hero's Spirit: only Zero to Hero puts it into play. Giga Impact: can't attack next turn.</summary>
        private sealed class PalafinExTWM : Mon
        {
            public override bool PutIntoPlayOnlyByEffect { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Giga Impact") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Aurora Gain: heal 20. Undulating Slice: put up to 9 damage counters on this Pokémon, 20 damage each.</summary>
        private sealed class WalkingWakeTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Aurora Gain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 20);
                    return true;
                }
                if (atk.Name == "Undulating Slice")
                {
                    int n = ctx.AskNumber(0, 9, "Undulating Slice: put how many damage counters on " + ctx.Pokemon.Name + "? (20 damage each)");
                    if (n > 0) ctx.PlaceCounters(ctx.Pokemon, n);
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Lightning

        /// <summary>Thunder Wave: flip; heads Paralyzed. Thunderbolt: discard all Energy from this Pokémon.</summary>
        private sealed class ZapdosTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Thunder Wave")
                {
                    var defending = ctx.Defending;
                    if (ctx.Flip() && defending != null) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Thunderbolt")
                {
                    ctx.DamageDefending(atk.Damage);
                    SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                    return true;
                }
                return false;
            }
        }

        private sealed class ShinxTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Curiosity") return false;
                RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Piercing Gaze: the opponent reveals their hand; discard a card from it. Volt Strike: discard all Energy from this Pokémon.</summary>
        private sealed class LuxrayExTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Piercing Gaze")
                {
                    ctx.DamageDefending(atk.Damage);
                    RevealOpponentHand(ctx);
                    var hand = ctx.Opp.Hand.ToList();
                    if (hand.Count > 0)
                    {
                        var pick = ctx.ChooseCards(hand, 1, 1, "Piercing Gaze: discard a card from your opponent's hand");
                        if (pick.Count > 0) ctx.Engine.DiscardFromHand(ctx.Opponent, pick[0]);
                    }
                    return true;
                }
                if (atk.Name == "Volt Strike")
                {
                    ctx.DamageDefending(atk.Damage);
                    SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sky Wave: also 10 damage to each Benched Pokémon (both yours and your opponent's).</summary>
        private sealed class EmolgaTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sky Wave") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var b in ctx.OpponentBench()) ctx.DamageTarget(b, 10);
                foreach (var b in ctx.MyBench()) ctx.DamageTarget(b, 10);
                return true;
            }
        }

        /// <summary>Snack Seek: once during your turn, look at the top card of your deck; you may discard it. Pick and Stick: attach up to 2 Basic Energy from your discard pile.</summary>
        private sealed class MorpekoTWM : Mon
        {
            protected override string Activated { get { return "Snack Seek"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return;
                if (ctx.AskYesNo("Snack Seek: the top card of your deck is " + top[0].Def.Name + ". Discard it?"))
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pick and Stick") return false;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, 2, (c, m) => true, "Pick and Stick: attach a Basic Energy from your discard pile");
                return true;
            }
        }
    }
}
