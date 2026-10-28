using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Perfect Order (POR): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Servine, Scatterbug, Fletchinder, Salandit, Seel, Staryu, Nosepass, Honedge, Bunnelby.
    /// Antique Jaw Fossil and Antique Sail Fossil are Items played as 60-HP Basic Pokémon (same rules as the other fossils).
    /// </summary>
    public static class CardsPOR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("POR-1", new NoRetreatMon("Gooey Thread"));
            r.RegisterByKey("POR-2", new PoisonRingMon("Poison Ring"));
            r.RegisterByKey("POR-3", new ShayminPOR());
            r.RegisterByKey("POR-4", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("POR-8", new DodgeMon("Hide", false));
            r.RegisterByKey("POR-9", new VivillonPOR());
            r.RegisterByKey("POR-10", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("POR-11", new CardsMEP.ExplosionYMon("Feather Shot", 99, 90));
            r.RegisterByKey("POR-12", new DecidueyeExPOR());
            r.RegisterByKey("POR-14", new TalonflamePOR());
            r.RegisterByKey("POR-16", new SalazzleExPOR());
            r.RegisterByKey("POR-17", new TurtonatorPOR());
            r.RegisterByKey("POR-19", new DewgongPOR());
            r.RegisterByKey("POR-21", new MegaStarmieExPOR());
            r.RegisterByKey("POR-22", new LaprasExPOR());
            r.RegisterByKey("POR-23", new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep));
            r.RegisterByKey("POR-24", new AurorusPOR());
            r.RegisterByKey("POR-25", new FlipPerTypedEnergyMon("Powerful Steam", EnergyType.Water));
            r.RegisterByKey("POR-26", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("POR-27", new LuxioPOR());
            r.RegisterByKey("POR-28", new LuxrayPOR());
            r.RegisterByKey("POR-29", new DedennePOR());
            r.RegisterByKey("POR-30", new FollowMeMon("Follow Me"));
            r.RegisterByKey("POR-32", new MawilePOR());
            r.RegisterByKey("POR-33", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("POR-34", new ComboMon(new ConditionAttackMon("Perplex", SpecialCondition.Confused), new CardsMEG.DefendingEnergyBonusMon("Psychic", 30)));
            r.RegisterByKey("POR-35", new SweetScentMon("Sweet Scent", 30));
            r.RegisterByKey("POR-36", new AromatissePOR());
            r.RegisterByKey("POR-38", new CardsTWM1.SelfDiscardMon("Obliterating Nose", 3));
            r.RegisterByKey("POR-39", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("POR-40", new HippowdonPOR());
            r.RegisterByKey("POR-41", new LandorusPOR());
            r.RegisterByKey("POR-46", new HawluchaPOR());
            r.RegisterByKey("POR-48", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("POR-49", new HaunterPOR());
            r.RegisterByKey("POR-50", new GengarPOR());
            r.RegisterByKey("POR-51", new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned));
            r.RegisterByKey("POR-53", new YveltalExPOR());
            r.RegisterByKey("POR-54", new ComboMon(new SwitchSelfMon("Strafe", true), new CardsASC.ExBonusMon("Rising Blade", 80)));
            r.RegisterByKey("POR-58", new LockAfterAttackMon("Metal Slash", false));
            r.RegisterByKey("POR-59", new MemoryLockMon("Memory Lock"));
            r.RegisterByKey("POR-60", new RecoilMon("Take Down", 10));
            r.RegisterByKey("POR-61", new RaticatePOR());
            r.RegisterByKey("POR-63", new SnorlaxPOR());
            r.RegisterByKey("POR-65", new DiggersbyPOR());
            r.RegisterByKey("POR-66", new SearchAttackMon("Chirp", d => d.IsPokemon && d.Resistance == EnergyType.Fighting, 2));
            r.RegisterByKey("POR-67", new FurfrouPOR());

            r.RegisterByName("Antique Jaw Fossil", new AntiqueJawFossil());
            r.RegisterByName("Antique Sail Fossil", new AntiqueSailFossil());
            r.RegisterByName("Core Memory", new CoreMemory());
            r.RegisterByName("Energy Search", new EnergySearch());
            r.RegisterByName("Energy Swatter", new EnergySwatter());
            r.RegisterByName("Hole-Digging Shovel", new HoleDiggingShovel());
            r.RegisterByName("Jacinthe", new Jacinthe());
            r.RegisterByName("Lumiose Galette", new LumioseGalette());
            r.RegisterByName("Naveen", new Naveen());
            r.RegisterByName("Poké Ball", new PokeBall());
            r.RegisterByName("Potion", new Potion());
            r.RegisterByName("Tarragon", new Tarragon());
        }

        // ================================================================== Shared helpers

        /// <summary>"Your opponent's Active Pokémon is now Poisoned. During your opponent's next turn, that Pokémon can't retreat."</summary>
        public sealed class PoisonRingMon : Mon
        {
            private readonly string attack;

            public PoisonRingMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>"Flip a coin for each [T] Energy attached to this Pokémon. This attack does N damage for each heads."</summary>
        public sealed class FlipPerTypedEnergyMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public FlipPerTypedEnergyMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int coins = ctx.Engine.EnergyCountOfType(ctx.Pokemon, type);
                ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }

        /// <summary>"Switch in 1 of your opponent's Benched Pokémon to the Active Spot."</summary>
        public sealed class FollowMeMon : Mon
        {
            private readonly string attack;

            public FollowMeMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.GustOpponent(atk.Name + ": switch in which of your opponent's Benched Pokémon?");
                return true;
            }
        }

        /// <summary>"Heal N damage from 1 of your Pokémon."</summary>
        public sealed class SweetScentMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SweetScentMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsMEG.HealOneOfMine(ctx, amount, atk.Name);
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Active Pokémon's attacks. During your opponent's next turn, that Pokémon can't use that attack."</summary>
        public sealed class MemoryLockMon : Mon
        {
            private readonly string attack;

            public MemoryLockMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                int index = ChooseAttackOf(ctx, defending.Def, atk.Name + ": choose 1 of " + defending.Name + "'s attacks");
                if (index < 0) return true;
                string name = defending.Def.Attacks[index].Name;
                ctx.AddEffect(new CantUseAttackEffect(defending.Name + " can't use " + name + " during its next turn (" + atk.Name + ")",
                    ctx.Player, defending.Uid, ctx.Turn + 1, name));
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Send Flowers: search your deck for an Energy card and attach it to 1 of your Benched Grass Pokémon.</summary>
        private sealed class ShayminPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Send Flowers") return false;
                var targets = ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Grass)).ToList();
                if (targets.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Send Flowers: choose an Energy card");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (c, m) => ctx.Me.Bench.Contains(m) && ctx.Engine.IsType(m, EnergyType.Grass), "Send Flowers: attach to a Benched Grass Pokémon");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Grand Wing: once during your turn, the opponent puts their hand on the bottom of their deck and draws 4. Blow Through: +60 with a Stadium in play.</summary>
        private sealed class VivillonPOR : Mon
        {
            protected override string Activated { get { return "Grand Wing"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return;
                ctx.State.Rng.Shuffle(hand);
                foreach (var c in hand) ctx.Opp.Hand.Remove(c);
                ctx.Opp.Deck.AddRange(hand);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put their hand (" + hand.Count + " cards) on the bottom of their deck.");
                ctx.Engine.DrawCards(ctx.Opponent, 4);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blow Through") return false;
                ctx.DamageDefending(atk.Damage + (ctx.State.Stadium != null ? 60 : 0));
                return true;
            }
        }

        /// <summary>Sniper's Eye: with exactly 4 cards in the opponent's hand, its attacks ignore Colorless costs. Crushing Arrow: discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class DecidueyeExPOR : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (ctx.Pokemon == null || ctx.Opp.Hand.Count != 4 || ctx.Engine.AbilityBlocked(ctx.Pokemon, "Sniper's Eye")) return;
                cost.RemoveAll(t => t == EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Arrow") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Crushing Arrow: discard an Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Sky Hunt: once during your turn, flip; heads, discard a random card from the opponent's hand.</summary>
        private sealed class TalonflamePOR : Mon
        {
            protected override string Activated { get { return "Sky Hunt"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
            }
        }

        /// <summary>Nasty Plot: search your deck for up to 2 cards. Dire Nails: Burned and Poisoned, then switch it with a Benched Pokémon.</summary>
        private sealed class SalazzleExPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Nasty Plot")
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 2, "Nasty Plot: search your deck for up to 2 cards", false, 1);
                    return true;
                }
                if (atk.Name == "Dire Nails")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                    {
                        ctx.ApplyCondition(defending, SpecialCondition.Burned);
                        ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    }
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Shell Spikes: when it's damaged in the Active Spot by the opponent's attack, discard an Energy from the Attacking Pokémon. Heat Breath: flip for 80 more.</summary>
        private sealed class TurtonatorPOR : Mon
        {
            private static readonly Mon HeatBreath = new FlipBonusMon("Heat Breath", 80);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || attacker.Energy.Count == 0) return;
                Announce(ctx, "Shell Spikes");
                ctx.DiscardEnergyFrom(attacker, 1, "Shell Spikes: discard an Energy from " + attacker.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HeatBreath.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Wash Out: as often as you like, move a Water Energy from a Benched Pokémon to your Active Pokémon.</summary>
        private sealed class DewgongPOR : Mon
        {
            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Water))).ToList();
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Wash Out" && ctx.IsMyTurn && ctx.Me.Active != null && Givers(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing moves as many as they like, then presses Done; bots move one per use.
                RepeatableAbility.Repeat(ctx, () => ctx.Me.Active != null && Givers(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), "Wash Out: move a Water Energy from which Benched Pokémon to "
                        + ctx.Me.Active.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null || ctx.Me.Active == null) return false;
                    var water = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, EnergyType.Water)).ToList();
                    var energy = water.Select(e => e.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Wash Out: move which Energy?")[0];
                    ctx.Engine.MoveEnergy(from, ctx.Me.Active, energy);
                    return true;
                });
            }
        }

        /// <summary>Jetting Blow: also 50 to 1 Benched Pokémon. Nebula Beam: not affected by Weakness, Resistance or effects on the Defending Pokémon.</summary>
        private sealed class MegaStarmieExPOR : Mon
        {
            private static readonly Mon JettingBlow = new CardsMEG.SnipeBenchMon("Jetting Blow", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Nebula Beam")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                    return true;
                }
                return JettingBlow.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Hydro Turn: 30 per Water Energy on it, then switch it with a Benched Pokémon.</summary>
        private sealed class LaprasExPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Turn") return false;
                ctx.DamageDefending(atk.Damage * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water));
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Tundra Wall: your Pokémon with Water Energy take 50 less from the opponent's attacks (doesn't stack). Freezing Chill: the Defending Pokémon can't attack next turn.</summary>
        private sealed class AurorusPOR : Mon
        {
            private static readonly Mon FreezingChill = new CardsBLK.NoAttackNextTurnMon("Freezing Chill");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                if (!src.Engine.HasEnergyOfType(info.Target, EnergyType.Water) || !info.ApplyOnce("Tundra Wall")) return damage;
                return damage - 50;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FreezingChill.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Fighting Roar: with a Pokémon ex as the opponent's Active Pokémon, it can evolve on your first turn or the turn you play it.</summary>
        private sealed class LuxioPOR : Mon
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                var d = src.Opp.Active;
                return src.Pokemon != null && target == src.Pokemon && d != null && (d.Def.IsEx || d.Def.IsMegaEx);
            }
        }

        /// <summary>Incessant Onslaught: 70 for each Prize card you have taken. Strong Volt: discard 2 Energy.</summary>
        private sealed class LuxrayPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Incessant Onslaught")
                {
                    ctx.DamageDefending(atk.Damage * ctx.MyPrizesTaken);
                    return true;
                }
                if (atk.Name == "Strong Volt")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Tail Generator: attach Basic Lightning Energy from your discard pile to your Lightning Pokémon, up to the Energy on all the opponent's Pokémon. Thunder Shock: flip to Paralyze.</summary>
        private sealed class DedennePOR : Mon
        {
            private static readonly Mon ThunderShock = new ParalyzeFlipMon("Thunder Shock");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tail Generator")
                {
                    int max = ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
                    var pile = ctx.BasicEnergyInDiscard(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning));
                    if (max == 0 || pile.Count == 0) return true;
                    ctx.AttachCardsAnyWay(pile, max, (c, m) => ctx.Engine.IsType(m, EnergyType.Lightning), "Tail Generator: attach Basic Lightning Energy to your Lightning Pokémon");
                    return true;
                }
                return ThunderShock.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Psychic

        /// <summary>Double Eater: discard up to 2 Energy cards from your hand, 60 each.</summary>
        private sealed class MawilePOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Eater") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Double Eater: discard up to 2 Energy cards from your hand (" + atk.Damage + " damage each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        /// <summary>Scent Collection: once during your turn, search your deck for up to 2 Basic Psychic Energy. Draining Kiss: heal 30 from it.</summary>
        private sealed class AromatissePOR : Mon
        {
            private static readonly Mon DrainingKiss = new CardsTWM1.HealSelfMon("Draining Kiss", 30);

            protected override string Activated { get { return "Scent Collection"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 2, "Scent Collection: choose up to 2 Basic Psychic Energy cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DrainingKiss.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Twister Spewing: if you played Tarragon this turn, discard the top 3 cards of the opponent's deck.</summary>
        private sealed class HippowdonPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twister Spewing") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.SupporterNameThisTurn == "Tarragon") ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                return true;
            }
        }

        /// <summary>Rock Tumble: Resistance doesn't apply. Screw Knuckle: put an Energy from it into your hand.</summary>
        private sealed class LandorusPOR : Mon
        {
            private static readonly Mon RockTumble = new CardsFLF1.NoResistanceMon("Rock Tumble");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Screw Knuckle")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                    var e = me.Energy.Select(x => x.Def.Name).Distinct().Count() == 1 ? me.Energy[0]
                        : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Screw Knuckle: put which Energy into your hand?")[0];
                    ctx.Engine.ReturnEnergyToHand(me, e);
                    return true;
                }
                return RockTumble.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Vengeful Kick: +60 if your Benched Pokémon have damage counters.</summary>
        private sealed class HawluchaPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vengeful Kick") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Bench.Any(m => m.DamageCounters > 0) ? 60 : 0));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Haunt: 3 damage counters on the opponent's Active Pokémon.</summary>
        private sealed class HaunterPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Haunt") return false;
                var d = ctx.Defending;
                if (d != null && ctx.CanAffect(d)) ctx.PlaceCounters(d, 3);
                return true;
            }
        }

        /// <summary>Infinite Shadow: Knocked Out by the opponent's attack damage, it goes to your hand (attached cards are discarded). Mind Jack: +30 per opponent's Benched Pokémon.</summary>
        private sealed class GengarPOR : Mon
        {
            public override bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                return byOpponentsAttack && mon == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Jack") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.Opp.Bench.Count);
                return true;
            }
        }

        /// <summary>Soul Destroyer: Knock Out each of the opponent's Pokémon with 50 HP or less left. Dark Strike: can't use it next turn.</summary>
        private sealed class YveltalExPOR : Mon
        {
            private static readonly Mon DarkStrike = new LockAfterAttackMon("Dark Strike", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Soul Destroyer")
                {
                    var weak = ctx.OpponentPokemon(m => ctx.Engine.RemainingHp(m) <= 50 && ctx.CanAffect(m));
                    if (weak.Count == 0) Nothing(ctx);
                    foreach (var m in weak) if (ctx.InPlay(m)) ctx.Engine.KnockOut(m, atk.Name);
                    return true;
                }
                return DarkStrike.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Colorless

        /// <summary>Scrape Off: discard the opponent's Active Pokémon's Tools before damage. Retaliatory Incisors: 40 per damage counter on your Benched Rattata.</summary>
        private sealed class RaticatePOR : Mon
        {
            private static readonly Mon ScrapeOff = new CardsBLK.PeckOffMon("Scrape Off");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Retaliatory Incisors")
                {
                    int counters = ctx.Me.Bench.Where(m => m.Name == "Rattata").Sum(m => m.DamageCounters);
                    ctx.DamageDefending(atk.Damage * counters);
                    return true;
                }
                return ScrapeOff.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Gormandizer: flip until tails; attach up to that many Basic Energy from your deck to it. Collapse: it falls Asleep.</summary>
        private sealed class SnorlaxPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gormandizer")
                {
                    int heads = ctx.FlipUntilTails();
                    if (heads > 0 && ctx.Me.Deck.Count > 0 && ctx.InPlay(ctx.Pokemon))
                    {
                        var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, heads, "Gormandizer: search your deck for up to " + heads + " Basic Energy cards");
                        foreach (var e in found) ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                        ctx.ShuffleMyDeck();
                    }
                    return true;
                }
                if (atk.Name == "Collapse")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Earthquake: also 30 to each of your own Benched Pokémon.</summary>
        private sealed class DiggersbyPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earthquake") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.Me.Bench.ToList()) ctx.DamageTarget(m, 30);
                return true;
            }
        }

        /// <summary>Hand Trim: discard random cards from the opponent's hand until they have 5.</summary>
        private sealed class FurfrouPOR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hand Trim") return false;
                int extra = ctx.Opp.Hand.Count - 5;
                if (extra > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, extra);
                else Nothing(ctx);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Intimidating Jaw: while Active, the opponent's Active Pokémon's attacks do 30 less damage.</summary>
        private sealed class AntiqueJawFossil : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.State.IsActive(info.Attacker) ? damage - 30 : damage;
            }
        }

        /// <summary>Protective Sail: the opponent's Supporter cards' effects can't affect it.</summary>
        private sealed class AntiqueSailFossil : CardScript
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && trainer != null && trainer.IsSupporter;
            }
        }

        /// <summary>Tool: a Mega Zygarde ex holding it can use Geobuster (350, discard all Energy).</summary>
        private sealed class CoreMemory : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Name != "Mega Zygarde ex" || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Pokemon == null) return;
                ctx.DamageDefending(ctx.Attack(attackIndex).Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
            }
        }

        private sealed class EnergySearch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(d => d.IsBasicEnergy, 1, "Energy Search: choose a Basic Energy card"); }
        }

        /// <summary>The opponent reveals their hand; put an Energy card from it on the bottom of their deck.</summary>
        private sealed class EnergySwatter : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                CardsTWM1.RevealOpponentHand(ctx);
                var energy = ctx.Opp.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return;
                var pick = ctx.ChooseCards(energy, 1, 1, "Energy Swatter: put which Energy card on the bottom of your opponent's deck?")[0];
                ctx.Opp.Hand.Remove(pick);
                ctx.Opp.Deck.Add(pick);
                ctx.Log(pick.Def.Name + " was put on the bottom of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck.");
            }
        }

        private sealed class HoleDiggingShovel : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Engine.DiscardTopOfDeck(ctx.Player, 2); }
        }

        /// <summary>Heal 150 damage from 1 of your Psychic Pokémon.</summary>
        private sealed class Jacinthe : CardScript
        {
            private static bool Psychic(EffectContext ctx, PokemonInPlay m) { return ctx.Engine.IsType(m, EnergyType.Psychic); }

            public override bool CanPlayTrainer(EffectContext ctx) { return CardsMEG.AnyHurt(ctx, m => Psychic(ctx, m)); }
            public override void ResolveTrainer(EffectContext ctx) { CardsMEG.HealOneOfMine(ctx, 150, "Jacinthe", m => Psychic(ctx, m)); }
        }

        /// <summary>Heal 20 damage and remove a Special Condition from your Active Pokémon.</summary>
        private sealed class LumioseGalette : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && (a.DamageCounters > 0 || a.Conditions != SpecialCondition.None);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                if (a == null) return;
                if (a.DamageCounters > 0) ctx.Heal(a, 20);
                var all = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
                var has = all.Where(c => a.Has(c)).ToList();
                if (has.Count == 0) return;
                int pick = has.Count == 1 ? 0 : ctx.ChooseOption("Lumiose Galette: remove which Special Condition?", has.Select(c => c.ToString()).ToArray());
                ctx.Engine.RemoveCondition(a, has[pick]);
            }
        }

        /// <summary>You may discard any number of cards from your hand, then draw until you have 5 (only if that draws a card).</summary>
        private sealed class Naveen : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                // Discarding first can always make room, so it's playable whenever the deck has cards.
                return ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count > 0)
                {
                    var picks = ctx.ChooseCards(hand, 0, hand.Count, "Naveen: discard any number of cards from your hand before drawing");
                    foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                }
                ctx.DrawUntil(5);
            }
        }

        private sealed class PokeBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.SearchToHand(d => d.IsPokemon, 1, "Poké Ball: choose a Pokémon");
            }
        }

        private sealed class Potion : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return CardsMEG.AnyHurt(ctx); }
            public override void ResolveTrainer(EffectContext ctx) { CardsMEG.HealOneOfMine(ctx, 30, "Potion"); }
        }

        /// <summary>Put up to 4 Fighting Pokémon and Basic Fighting Energy cards from your discard pile into your hand.</summary>
        private sealed class Tarragon : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => (c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Fighting) || CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                var picks = ctx.ChooseCards(pile, 1, Math.Min(4, pile.Count), "Tarragon: put up to 4 Fighting Pokémon and Basic Fighting Energy cards into your hand");
                ctx.DiscardToHand(picks); // through DiscardToHand so Toedscruel's Slime Mold Colony applies
            }
        }
    }
}
