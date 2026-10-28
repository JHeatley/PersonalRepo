using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Darkness Ablaze (DAA, 2020), Pokémon 1-81: Grass, Fire, Water, Lightning and Psychic. Fighting through Colorless, the
    /// Trainers and the Special Energy are in CardsDAA2. Regulation mark D: Expanded only.
    /// Already in the pool (most reprinted in Shining Fates): Rowlet, Dartrix, Decidueye, Charizard V / VMAX, Centiskorch V /
    /// VMAX, Galarian Mr. Mime, Galarian Mr. Rime, Suicune, Galarian Darmanitan (Water), Dracovish, Arctovish, Toxel,
    /// Toxtricity, Dracozolt, Arctozolt, Dedenne, Sinistea and Polteageist.
    /// Pokémon whose attacks only do their printed damage need no script (Paras, Pansage, Shelmet, Accelgor, Pansear, Panpour,
    /// Galarian Darumaka, Cubchoo, Mareep, Flaaffy, Jigglypuff, Snubbull, Golett, Golurk).
    /// </summary>
    public static class CardsDAA1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("DAA-1", new CardsCES1.ConditionsMon("Dizzying Poison", SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("DAA-2", new CardsCES1.ConditionsMon("G-Max Toxbreeze", SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("DAA-4", new ParalyzeFlipMon("Mushroom Tackle"));
            r.RegisterByKey("DAA-5", new ComboMon(new MayDrawUntilMon("Return", 5), new DrainMon("Giga Drain")));
            r.RegisterByKey("DAA-7", new LockAfterAttackMon("Giga Impact", false));
            r.RegisterByKey("DAA-8", new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt"));
            r.RegisterByKey("DAA-14", new CardsVIV1.SynthesisMon());
            r.RegisterByKey("DAA-15", new GustAttackMon("Captivate", false));
            r.RegisterByKey("DAA-16", new ComboMon(new PowerWhipMon(), new TimeOutKickMon()));
            r.RegisterByKey("DAA-17", new SwitchSelfMon("Gnaw and Run", false));
            r.RegisterByKey("DAA-18", new ComboMon(new CardsASR1.PlusMon("Hard Times Slash", 50, OpponentVAndGx), new SwitchSelfMon("Smash Turn", false)));
            // Fire
            r.RegisterByKey("DAA-21", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new VengefulFlameMon()));
            r.RegisterByKey("DAA-22", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("DAA-23", new ConditionAttackMon("Heat Beak", SpecialCondition.Burned));
            r.RegisterByKey("DAA-24", new BlazikenDAA());
            r.RegisterByKey("DAA-25", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new Cards30C.SelfCounterBonusMon("Raging Flare", 10)));
            r.RegisterByKey("DAA-27", new LockAfterAttackMon("Giga Impact", false));
            r.RegisterByKey("DAA-28", new FrozenHeatMon());
            r.RegisterByKey("DAA-29", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("DAA-30", new CardsNXD.BaredFangsMon("Cauterize"));
            r.RegisterByKey("DAA-31", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("DAA-32", new TalonflameDAA());
            // Water
            r.RegisterByKey("DAA-38", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("DAA-39", new MiloticDAA());
            r.RegisterByKey("DAA-40", new FossilSearchMon());
            r.RegisterByKey("DAA-42", new LockAfterAttackMon("Giga Impact", false));
            r.RegisterByKey("DAA-45", new ParalyzeFlipMon("Ice Over"));
            r.RegisterByKey("DAA-46", new ParalyzeFlipMon("Freezing Headbutt"));
            r.RegisterByKey("DAA-47", new VanilluxeDAA());
            r.RegisterByKey("DAA-49", new RecoilMon("Frozen Slice", 50));
            r.RegisterByKey("DAA-50", new DeepSeaSwirlMon("Deep Sea Swirl", 8));
            r.RegisterByKey("DAA-51", new ComboMon(new CardsTWM1.HealSelfMon("Regeneration", 30), new ConditionAttackMon("Poison Tentacles", SpecialCondition.Poisoned)));
            r.RegisterByKey("DAA-52", new ComboMon(new CardsLOR1.RecoverMon(), new ConditionAttackMon("Poisonous Whip", SpecialCondition.Poisoned)));
            // Lightning
            r.RegisterByKey("DAA-57", new ConditionAttackMon("Dazzle Blast", SpecialCondition.Confused));
            r.RegisterByKey("DAA-58", new CardsTWM1.SelfDiscardMon("Thunderbolt", 99));
            r.RegisterByKey("DAA-59", new ComboMon(new SwitchSelfMon("Strafe", true), new FlashImpactMon()));
            r.RegisterByKey("DAA-60", new ComboMon(new ItemLockAttackMon("Paralyzing Bolt"), new CardsTWM1.SelfDiscardMon("Super Zap Cannon", 2)));
            r.RegisterByKey("DAA-61", new DrawAttackMon("Allure", 2));
            r.RegisterByKey("DAA-64", new ComboMon(new DrawAttackMon("Double Draw", 2), new ParalyzeFlipMon("Zing Zap")));
            // Psychic
            r.RegisterByKey("DAA-68", new ComboMon(new ConditionAttackMon("Sleep Pulse", SpecialCondition.Asleep), new CoinsMon("Double Smash", 2)));
            r.RegisterByKey("DAA-69", new CardsASR1.TimesMon("X Ball", BothActiveEnergy));
            r.RegisterByKey("DAA-71", new NoRetreatMon("Jaw Lock"));
            r.RegisterByKey("DAA-72", new ComboMon(new CardsVIV1.FutureSightMon(), new CardsASR1.PlusMon("Psychic", 20, DefendingEnergy)));
            r.RegisterByKey("DAA-73", new FortunateEyeMon());
            r.RegisterByKey("DAA-74", new ComboMon(new RandomHandToDeckMon("Whiny Voice", 1), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("DAA-75", new ComboMon(new ConditionAttackMon("Mind Bend", SpecialCondition.Confused), new RandomHandToDeckMon("Bend", 2)));
            r.RegisterByKey("DAA-79", new ConditionAttackMon("Spore", SpecialCondition.Asleep));
            r.RegisterByKey("DAA-80", new ComboMon(new FlipCantAttackMon("Flickering Light"), new NamedStadiumBonusMon("Fear the Forest", "Glimwood Tangle", 60)));
            r.RegisterByKey("DAA-81", new MimikyuDAA());
        }

        private static int OpponentVAndGx(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Def.IsPokemonV || m.Def.IsGX).Count; }
        private static int DefendingEnergy(EffectContext ctx) { return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending); }
        private static int BothActiveEnergy(EffectContext ctx)
        {
            int mine = ctx.Me.Active == null ? 0 : ctx.EnergyCount(ctx.Me.Active);
            return mine + DefendingEnergy(ctx);
        }

        // ================================================================== Shared attacks (also used by CardsDAA2)

        /// <summary>"You may draw cards until you have N cards in your hand" (Return).</summary>
        internal sealed class MayDrawUntilMon : Mon
        {
            private readonly string attack;
            private readonly int size;

            public MayDrawUntilMon(string attack, int size)
            {
                this.attack = attack;
                this.size = size;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count < size && ctx.Me.Deck.Count > 0 && ctx.AskYesNo(attack + ": draw cards until you have " + size + " cards in your hand?"))
                    ctx.DrawUntil(size);
                return true;
            }
        }

        /// <summary>"Switch 1 of your opponent's Benched Pokémon with their Active Pokémon" (Captivate); confuse: the new one is Confused (Captivating Tail).</summary>
        internal sealed class GustAttackMon : Mon
        {
            private readonly string attack;
            private readonly bool confuse;

            public GustAttackMon(string attack, bool confuse)
            {
                this.attack = attack;
                this.confuse = confuse;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pulled = ctx.GustOpponent(attack + ": choose 1 of your opponent's Benched Pokémon to switch in");
                if (confuse && pulled != null && TefPokemon.StillDefending(ctx, pulled)) ctx.ApplyCondition(pulled, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, during your opponent's next turn, the Defending Pokémon can't attack."</summary>
        internal sealed class FlipCantAttackMon : Mon
        {
            private readonly string attack;

            public FlipCantAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>"Choose N random cards from your opponent's hand. Your opponent reveals them and shuffles them into their deck."</summary>
        internal sealed class RandomHandToDeckMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public RandomHandToDeckMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0) CardsTWM1.ShuffleRandomFromOpponentHand(ctx, count);
                return true;
            }
        }

        /// <summary>"If [Stadium] is in play, this attack does N more damage" (Fear the Forest).</summary>
        internal sealed class NamedStadiumBonusMon : Mon
        {
            private readonly string attack;
            private readonly string stadium;
            private readonly int bonus;

            public NamedStadiumBonusMon(string attack, string stadium, int bonus)
            {
                this.attack = attack;
                this.stadium = stadium;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool inPlay = ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == stadium;
                ctx.DamageDefending(atk.Damage + (inPlay ? bonus : 0));
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, they can't play any Item cards from their hand."</summary>
        internal sealed class ItemLockAttackMon : Mon
        {
            private readonly string attack;

            public ItemLockAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during their next turn (" + attack + ").");
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>"Heal from this Pokémon the same amount of damage it did to the opponent's Active Pokémon" (Giga Drain, Leech Life, Swallow, Gulp Down, Absorb Vitality, Suction Heal, Shadow Drain, Cling and others; the attack name is passed in).</summary>
        internal sealed class DrainMon : Mon
        {
            private readonly string attack;

            public DrainMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int done = ctx.DamageDefending(atk.Damage);
                if (done > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, done);
                return true;
            }
        }

        /// <summary>Power Whip: 20 damage to 1 of the opponent's Pokémon for each Energy attached to this Pokémon.</summary>
        internal sealed class PowerWhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Whip") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                if (n > 0) ctx.DamageAnyOpponent(20 * n);
                return true;
            }
        }

        /// <summary>Time Out Kick: you may put an Energy attached to the opponent's Active Pokémon into their hand.</summary>
        private sealed class TimeOutKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Out Kick") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var picks = ctx.ChooseCards(defending.Energy.ToList(), 0, 1, "Time Out Kick: you may put an Energy attached to " + defending.Name + " into your opponent's hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(defending, e);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Vengeful Flame: 100 more if your Benched Fire Pokémon have any damage counters on them.</summary>
        private sealed class VengefulFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vengeful Flame") return false;
                bool hurt = ctx.Me.Bench.Any(b => b.DamageCounters > 0 && ctx.Engine.IsType(b, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage + (hurt ? 100 : 0));
                return true;
            }
        }

        /// <summary>Double Type: this Pokémon is Fire and Fighting type. Turbo Drive: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class BlazikenDAA : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Fire)) types.Add(EnergyType.Fire);
                if (!types.Contains(EnergyType.Fighting)) types.Add(EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Drive") return false;
                ctx.DamageDefending(atk.Damage);
                CardsSSP1.EnergyAssist(ctx, 1, "Turbo Drive");
                return true;
            }
        }

        /// <summary>Frozen Heat: you may discard all Water Energy from this Pokémon; if you do, 60 more damage.</summary>
        private sealed class FrozenHeatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frozen Heat") return false;
                var me = ctx.Pokemon;
                var water = me.Energy.Where(card => CardsDRI1.ProvidesType(ctx, me, card, EnergyType.Water)).ToList();
                bool more = false;
                if (water.Count > 0 && ctx.AskYesNo("Frozen Heat: discard all Water Energy from " + me.Name + " for 60 more damage?"))
                {
                    foreach (var w in water) ctx.Engine.DiscardEnergy(me, w);
                    more = true;
                }
                ctx.DamageDefending(atk.Damage + (more ? 60 : 0));
                return true;
            }
        }

        /// <summary>
        /// Scorching Feathers: if this Pokémon is in the Active Spot and is damaged by an attack from the opponent's Pokémon (even
        /// if it's Knocked Out), the Attacking Pokémon is now Burned. Mach Flight: the Defending Pokémon can't retreat.
        /// </summary>
        private sealed class TalonflameDAA : Mon
        {
            private static readonly Mon MachFlight = new NoRetreatMon("Mach Flight");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Scorching Feathers");
                ctx.ApplyCondition(attacker, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MachFlight.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Bright Heal: once during your turn, heal 20 damage from each of your Pokémon. Surf has no effect.</summary>
        private sealed class MiloticDAA : Mon
        {
            protected override string Activated { get { return "Bright Heal"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyPokemon().Any(m => m.DamageCounters > 0); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 20);
            }
        }

        /// <summary>Fossil Search: search your deck for up to 2 Rare Fossil cards and put them onto your Bench.</summary>
        private sealed class FossilSearchMon : Mon
        {
            private static bool IsRareFossil(CardDef d) { return d.Name == "Rare Fossil"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Search") return false;
                int room = Math.Min(2, ctx.BenchSpace);
                if (room <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsRareFossil, room, "Fossil Search: choose up to " + room + " Rare Fossil cards to put onto your Bench");
                foreach (var card in found)
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Bitter Cold: once during your turn, if this Pokémon is Active, you may flip a coin; heads, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class VanilluxeDAA : Mon
        {
            protected override string Activated { get { return "Bitter Cold"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Paralyzed);
            }
        }

        /// <summary>Deep Sea Swirl: shuffle your hand into your deck, then draw 8 cards. Used for Deep Sea Swirl, Windfall, Overhaul, Spin and Draw, Eeeeeeek (the attack name and the numbers are passed in).</summary>
        internal sealed class DeepSeaSwirlMon : Mon
        {
            private readonly string attack;
            private readonly int cards;

            public DeepSeaSwirlMon(string attack, int cards)
            {
                this.attack = attack;
                this.cards = cards;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(cards);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Flash Impact: this attack also does 30 damage to 1 of your Benched Pokémon.</summary>
        private sealed class FlashImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Impact") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var mine = ctx.ChoosePokemon(ctx.MyBench(), "Flash Impact: 30 damage to which of your Benched Pokémon?");
                if (mine != null) ctx.DamageTarget(mine, 30);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Fortunate Eye: look at the top 5 cards of your opponent's deck and put them back in any order.</summary>
        internal sealed class FortunateEyeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fortunate Eye") return false;
                if (ctx.Opp.Deck.Count > 0) CardsDRI2.ReorderOpponentTop(ctx, 5, "Fortunate Eye");
                return true;
            }
        }

        /// <summary>Heal Jamming: your opponent's Benched Pokémon can't be healed.</summary>
        private sealed class MimikyuDAA : Mon
        {
            public override bool BlocksHealing(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && !src.State.IsActive(mon);
            }
        }
    }
}
