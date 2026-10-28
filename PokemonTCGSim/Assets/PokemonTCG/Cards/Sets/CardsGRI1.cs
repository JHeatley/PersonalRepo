using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Guardians Rising (GRI, v89), part 1: Grass, Fire and Water (GRI 1-39). No regulation mark: Expanded. Turtonator-GX, Alolan
    /// Vulpix and Alolan Ninetales-GX (Hidden Fates Shiny Vault) were already in the pool and keep their scripts. "A [T] Energy card"
    /// outside play is basic Energy only (your ruling, v65).
    /// </summary>
    public static class CardsGRI1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("GRI-2", new CardsASC.FlipDiscardDefendingEnergyMon("Muddy Acid"));
            r.RegisterByKey("GRI-3", new ComboMon(new CardsCES1.ConditionsMon("Pollen Hazard", SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned),
                new StickAndAbsorbMon()));
            r.RegisterByKey("GRI-4", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("GRI-5", new ComboMon(new CardsCEC3.SpreadAmountMon("Petal Blizzard", 10), new CardsCEC3.PetalDanceMon()));
            r.RegisterByKey("GRI-6", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("GRI-7", new ComboMon(new CardsFST1.RevealCountMon("Poltergeist", d => d.IsTrainer, "Trainer"), new CardsTWM1.HealSelfMon("Horn Leech", 30)));
            r.RegisterByKey("GRI-8", new ShuffleSelfAwayMon("Scamper Away", false));
            r.RegisterByKey("GRI-9", new GolisopodGRI());
            // Fire
            r.RegisterByKey("GRI-10", new VictiniGRI());
            r.RegisterByKey("GRI-11", new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep));
            r.RegisterByKey("GRI-13", new ChandelureGRI());
            r.RegisterByKey("GRI-14", new ComboMon(new PassionateDanceMon(), new CardsSP1.SelfThenDefendingDiscardMon("Kindle")));
            r.RegisterByKey("GRI-15", new CardsBLK.VenoshockMon(40));
            r.RegisterByKey("GRI-16", new SalazzleGRI());
            r.RegisterByKey("GRI-17", new ParalyzeFlipMon("Body Slam"));
            // Water
            r.RegisterByKey("GRI-19", new CardsSP1.FlipNoDamageMon("Defense Curl"));
            r.RegisterByKey("GRI-20", new AlolanSandslashGRI());
            r.RegisterByKey("GRI-23", new SludgeShockMon());
            r.RegisterByKey("GRI-24", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new CardsROS1.HeadlockMon("Seething Tentacles", 40)));
            r.RegisterByKey("GRI-25", new ComboMon(new RollCallMon(), new ShuffleSelfAwayMon("Hyper Jump", true)));
            r.RegisterByKey("GRI-26", new ComboMon(new AllThePresentsMon(), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("GRI-30", new ComboMon(new DodgeMon("Dive", false), new HealTypeMon("Open Sea", EnergyType.Water, 30)));
            r.RegisterByKey("GRI-32", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"),
                new CardsFST2.ConditionBonusMon("Raging Floe", 80, c => c.Pokemon.DamageCounters > 0)));
            r.RegisterByKey("GRI-33", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("GRI-34", new CardsFST2.ConditionBonusMon("Ice Shard", 30, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Fighting)));
            r.RegisterByKey("GRI-35", new ComboMon(new CardsCEC3.SpreadAmountMon("Hail", 20), new FrozenBreathMon()));
            r.RegisterByKey("GRI-36", new CardsASR1.BorneAshoreMon());
            r.RegisterByKey("GRI-37", new WishiwashiGRI());
            r.RegisterByKey("GRI-38", new ComboMon(new TorrentialVortexMon(), new MoveAllEnergyToBenchMon("Blue Surge-GX")));
            r.RegisterByKey("GRI-39", new BailOutAnyMon());
        }

        // ================================================================== Shared

        /// <summary>"Shuffle this Pokémon and all cards attached to it into your deck" (Scamper Away; Hyper Jump asks first).</summary>
        internal sealed class ShuffleSelfAwayMon : Mon
        {
            private readonly string attack;
            private readonly bool optional;

            public ShuffleSelfAwayMon(string attack, bool optional)
            {
                this.attack = attack;
                this.optional = optional;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (optional && !ctx.AskYesNo(attack + ": shuffle " + me.Name + " and all cards attached to it into your deck?")) return true;
                bool wasActive = ctx.IsActive(me);
                ctx.Engine.ShufflePokemonIntoDeck(me);
                if (wasActive && ctx.Me.Active == null && ctx.Me.Bench.Count > 0) ctx.Engine.PromoteActive(ctx.Player);
                return true;
            }
        }

        /// <summary>"Heal N damage from each of your [T] Pokémon" (Open Sea).</summary>
        internal sealed class HealTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int amount;

            public HealTypeMon(string attack, EnergyType type, int amount)
            {
                this.attack = attack;
                this.type = type;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0 && ctx.Engine.IsType(mon, type)) ctx.Heal(mon, amount);
                return true;
            }
        }

        /// <summary>
        /// "Move all Energy from this Pokémon to your Benched Pokémon in any way you like" (Blue Surge-GX, Wings of the Moone). With no
        /// Benched Pokémon the Energy stays (Compendium, Tropic Breeze).
        /// </summary>
        internal sealed class MoveAllEnergyToBenchMon : Mon
        {
            private readonly string attack;

            public MoveAllEnergyToBenchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                foreach (var en in me.Energy.ToList())
                {
                    var dest = ctx.ChoosePokemon(ctx.MyBench(), attack + ": move " + en.Def.Name + " to which Benched Pokémon?");
                    if (dest != null) ctx.Engine.MoveEnergy(me, dest, en);
                }
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Stick and Absorb: heal 20 damage from this Pokémon; the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class StickAndAbsorbMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stick and Absorb") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.Heal(me, 20);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Armor: this Pokémon takes 30 less damage from attacks (after Weakness and Resistance). Resolute Claws: 70 more against a Pokémon-GX or Pokémon-EX.</summary>
        private sealed class GolisopodGRI : Mon
        {
            private static readonly Mon Claws = new CardsFST2.ConditionBonusMon("Resolute Claws", 70, c => c.Defending != null && c.Defending.Def.IsGXorEX);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Victory Star: once during your turn, after you flip any coins for an attack, you may flip them again (1 Victory Star per turn; the engine asks).</summary>
        private sealed class VictiniGRI : Mon
        {
            public override string OwnAttacksReflipAbility(EffectContext src) { return "Victory Star"; }
        }

        /// <summary>
        /// Shady Move: once during your turn, move 1 damage counter from 1 Pokémon (either player's) to another. Counters can't be moved
        /// off or onto a protected Pokémon (Compendium, Daunting Pose). Super Singe: Burned.
        /// </summary>
        private sealed class ChandelureGRI : Mon
        {
            private static readonly Mon Singe = new ConditionAttackMon("Super Singe", SpecialCondition.Burned);

            private static List<PokemonInPlay> All(EffectContext ctx) { return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList(); }

            protected override string Activated { get { return "Shady Move"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var all = All(ctx);
                return all.Count >= 2 && all.Any(m => m.DamageCounters > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                var all = All(ctx);
                var origin = ctx.ChoosePokemon(all.Where(m => m.DamageCounters > 0).ToList(), "Shady Move: move a damage counter from which Pokémon?");
                if (origin == null) return;
                var dest = ctx.ChoosePokemon(all.Where(m => m != origin).ToList(), "Shady Move: move it to which Pokémon?");
                if (dest != null) ctx.Engine.MoveDamageCounters(origin, dest, 1, "Shady Move");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Singe.HandleAttack(ctx, atk); }
        }

        /// <summary>Passionate Dance: search your deck for up to 3 Basic Fire Pokémon and put them onto your Bench.</summary>
        private sealed class PassionateDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Passionate Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToBench(d => d.PokemonType == EnergyType.Fire, 3, "Passionate Dance: search your deck for up to 3 Basic Fire Pokémon");
                return true;
            }
        }

        /// <summary>
        /// Hot Poison: when you play this Pokémon from your hand to evolve, you may make the opponent's Active Pokémon Burned and
        /// Poisoned (both, if you use it: Compendium, 2017-05-04). Flamethrower: discard an Energy from this Pokémon.
        /// </summary>
        private sealed class SalazzleGRI : Mon
        {
            private static readonly Mon Flame = new CardsTWM1.SelfDiscardMon("Flamethrower", 1);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Hot Poison", ctx.Opp.Active != null, "Use Hot Poison? Your opponent's Active Pokémon is now Burned and Poisoned.")) return;
                var target = ctx.Opp.Active;
                if (target == null) return;
                ctx.ApplyCondition(target, SpecialCondition.Burned);
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flame.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Slush Rush: once during your turn, you may draw a card. Smash Turn: switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class AlolanSandslashGRI : Mon
        {
            private static readonly Mon Turn = new SwitchSelfMon("Smash Turn", false);

            protected override string Activated { get { return "Slush Rush"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Turn.HandleAttack(ctx, atk); }
        }

        /// <summary>Sludge Shock: the Defending Pokémon can't be healed during the opponent's next turn.</summary>
        private sealed class SludgeShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sludge Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsUPR4.NoHealingEffect(defending.Name + " can't be healed during its next turn (Sludge Shock)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>
        /// Roll Call: search your deck for a Poliwag, a Poliwhirl and a Poliwrath and put them onto your Bench (Evolution cards too,
        /// unevolved). Only as many as you have room for, and you may leave some (Compendium, 2017-05-04).
        /// </summary>
        private sealed class RollCallMon : Mon
        {
            private static readonly string[] Names = { "Poliwag", "Poliwhirl", "Poliwrath" };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roll Call") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int room = Math.Min(3, ctx.BenchSpace);
                if (room <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && Names.Contains(d.Name) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), room,
                    "Roll Call: search your deck for a Poliwag, a Poliwhirl and a Poliwrath (1 of each)");
                var keep = new List<CardInstance>();
                foreach (var card in found)
                {
                    if (keep.Any(k => k.Def.Name == card.Def.Name)) ctx.Me.Deck.Add(card);
                    else keep.Add(card);
                }
                CardsDRM2.BenchCards(ctx, keep);
                foreach (var card in keep)
                    if (!ctx.Me.InPlay().Any(m => m.Stack.Contains(card))) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>All the Presents: flip a coin until you get tails; for each heads, you may search your deck for a card and put it into your hand.</summary>
        private sealed class AllThePresentsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "All the Presents") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads <= 0 || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => true, heads, "All the Presents: search your deck for up to " + heads + (heads == 1 ? " card" : " cards"), false);
                return true;
            }
        }

        /// <summary>Frozen Breath: you may discard 2 Water Energy from this Pokémon; if you do, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class FrozenBreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frozen Breath") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                var water = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water)).ToList();
                if (water.Count < 2 || !ctx.AskYesNo("Frozen Breath: discard 2 Water Energy from " + me.Name + " to Paralyze the opponent's Active Pokémon?")) return true;
                if (ctx.DiscardEnergyFrom(me, 2, "Frozen Breath: discard 2 Water Energy", en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water)) < 2) return true;
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Schooling: once during your turn, you may switch this Pokémon with a Wishiwashi-GX from your hand; attached cards, damage
        /// counters, Special Conditions, turns in play and other effects stay. Sharpshooting: 10 damage to 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class WishiwashiGRI : Mon
        {
            private static readonly Mon Shot = new CardsMEG.SnipeMon("Sharpshooting", 10);

            private static List<CardInstance> School(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Name == "Wishiwashi-GX").ToList(); }

            protected override string Activated { get { return "Schooling"; } }
            protected override bool AbilityReady(EffectContext ctx) { return School(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var options = School(ctx);
                if (options.Count == 0 || me == null) return;
                var pick = options.Count == 1 ? options[0] : ctx.ChooseCards(options, 1, 1, "Schooling: switch with which Wishiwashi-GX from your hand?")[0];
                ctx.Me.Hand.Remove(pick);
                CardsTWM1.ReplaceTopCard(ctx, me, pick, ctx.Me.Hand);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shot.HandleAttack(ctx, atk); }
        }

        /// <summary>Torrential Vortex: discard a Special Energy from the opponent's Active Pokémon.</summary>
        private sealed class TorrentialVortexMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Torrential Vortex") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Torrential Vortex: discard a Special Energy from " + defending.Name, en => en.Def.IsAttachedSpecialEnergy);
                return true;
            }
        }

        /// <summary>Bail Out: put a Pokémon from your discard pile into your hand.</summary>
        private sealed class BailOutAnyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bail Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pokemon = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(pokemon, 1, 1, "Bail Out: put a Pokémon from your discard pile into your hand"));
                return true;
            }
        }
    }
}
