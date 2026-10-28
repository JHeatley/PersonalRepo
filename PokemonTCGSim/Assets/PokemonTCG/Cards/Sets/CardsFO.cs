using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Fossil (FO, v116): the Pokémon (1-15 holo, 16-30 their non-holo prints, 31-57) and the Trainers (58-62). Psyduck is the
    /// Wizards promo's card, Slowpoke Generations' and Energy Search Perfect Order's. Toxic Gas, Prehistoric Power, Transform,
    /// Clairvoyance and Transparency change the rules, so the engine applies them (GameEngine.OldPowers); their cards register a
    /// plain script here. Mysterious Fossil follows the old fossil rules (10 HP, no Prize); Omanyte, Kabuto and Aerodactyl
    /// evolve from it.
    /// </summary>
    public static class CardsFO
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FO-1", new PlainMon()); // Prehistoric Power: the engine applies it
            r.RegisterByKey("FO-2", new ComboMon(new ParalyzeFlipMon("Freeze Dry"), new BlizzardMon()));
            r.RegisterByKey("FO-3", new PlainMon()); // Transform: the engine applies it
            r.RegisterByKey("FO-4", new DragoniteStepIn());
            r.RegisterByKey("FO-5", new GengarCurse());
            r.RegisterByKey("FO-6", new ConditionAttackMon("Nightmare", SpecialCondition.Asleep));
            r.RegisterByKey("FO-7", new Cards30CC.BenchHitMon("Stretch Kick", 20));
            r.RegisterByKey("FO-8", new ComboMon(new ProphecyMon(), new Cards30CC.BenchHitMon("Dark Mind", 10)));
            r.RegisterByKey("FO-9", new CardsJU.HalfDrainMon("Absorb"));
            r.RegisterByKey("FO-10", new ComboMon(new CardsBS1.ExtraWaterMon("Water Gun"), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("FO-11", new ComboMon(new CardsFCO2.NoWeaknessResistanceMon("Sonicboom"), new CardsBS1.SelfdestructMon("Selfdestruct", 20, 100)));
            r.RegisterByKey("FO-12", new ComboMon(new WildfireMon(), new TailsNothingMon("Dive Bomb")));
            r.RegisterByKey("FO-13", new CardsPAR2.FlipConditionMon("Sludge", SpecialCondition.Poisoned));
            r.RegisterByKey("FO-14", new GigashockMon());
            r.RegisterByKey("FO-15", new ThunderstormMon());
            r.RegisterByKey("FO-31", new ComboMon(new TerrorStrikeMon(), new ConditionAttackMon("Poison Fang", SpecialCondition.Poisoned)));
            r.RegisterByKey("FO-32", new ComboMon(new ClampMon(), new CoinsMon("Spike Cannon", 2)));
            r.RegisterByKey("FO-33", new ComboMon(new ParalyzeFlipMon("Lick"), new EnergyConversionMon()));
            r.RegisterByKey("FO-34", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("FO-35", new ComboMon(new ParalyzeFlipMon("Psyshock"), new CardsDRI2.CrunchMon("Hyper Beam")));
            r.RegisterByKey("FO-36", new CardsBS1.SelfdestructMon("Selfdestruct", 20, 100));
            r.RegisterByKey("FO-37", new CardsLOR1.HardenMon("Harden", 30));
            r.RegisterByKey("FO-38", new CardsASR1.TimesMon("Flail", c => c.Pokemon == null ? 0 : c.Pokemon.DamageCounters));
            r.RegisterByKey("FO-39", new ComboMon(new CardsMEG.SandAttackMon("Smokescreen"), new CardsPAR2.FlipConditionMon("Smog", SpecialCondition.Poisoned)));
            r.RegisterByKey("FO-40", new ComboMon(new CardsBS1.ExtraWaterMon("Water Gun"), new CoinsMon("Spike Cannon", 2)));
            r.RegisterByKey("FO-41", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("FO-42", new ComboMon(new CardsBS1.ExtraWaterMon("Water Gun"), new DodgeMon("Agility", false)));
            r.RegisterByKey("FO-43", new SlowbroStrangeBehavior());
            r.RegisterByKey("FO-44", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new ConditionAttackMon("Jellyfish Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("FO-45", new ComboMon(new CardsPAR2.FlipConditionMon("Smog", SpecialCondition.Poisoned), new CardsBS1.SelfdestructMon("Selfdestruct", 10, 60)));
            r.RegisterByKey("FO-46", new ComboMon(new CardsPAR2.FlipConditionMon("Spit Poison", SpecialCondition.Poisoned), new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("FO-47", new CardsPFL.FlipUntilTailsTimesMon("Stone Barrage"));
            r.RegisterByKey("FO-48", new ComboMon(new ParalyzeFlipMon("Nasty Goo"), new CardsJU.MinimizeMon()));
            r.RegisterByKey("FO-49", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("FO-50", new KabutoArmor());
            r.RegisterByKey("FO-51", new CardsJU.FamilyMon("Call for Family", d => d.Name == "Krabby", "a Basic Krabby"));
            r.RegisterByKey("FO-52", new CardsBS1.ExtraWaterMon("Water Gun"));
            r.RegisterByKey("FO-54", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsMEW1.WithdrawMon("Hide in Shell")));
            r.RegisterByKey("FO-56", new TentacoolCowardice());
            r.RegisterByKey("FO-57", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsDAA1.DrainMon("Leech Life")));

            r.RegisterByKey("FO-58", new MrFuji());
            r.RegisterByKey("FO-60", new Gambler());
            r.RegisterByKey("FO-61", new Recycle());
            r.RegisterByKey("FO-62", new CardScript()); // Mysterious Fossil: played like an Antique Fossil (engine)
        }

        // ================================================================== Pokémon Powers

        /// <summary>Step In: once during your turn, if Dragonite is on your Bench, switch it with your Active Pokémon. Slam: 2 coins x40.</summary>
        private sealed class DragoniteStepIn : Mon
        {
            private const string StepIn = "Step In";
            private static readonly Mon Slam = new CoinsMon("Slam", 2);

            protected override string Activated { get { return StepIn; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsBS1.PowerWorks(ctx.Pokemon) && ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Bench.Contains(ctx.Pokemon)) ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slam.HandleAttack(ctx, atk); }
        }

        /// <summary>Curse: once during your turn, move 1 damage counter from 1 of the opponent's Pokémon to another (even if that Knocks it Out).</summary>
        private sealed class GengarCurse : Mon
        {
            private const string Curse = "Curse";
            private static readonly Mon Mind = new Cards30CC.BenchHitMon("Dark Mind", 10);

            protected override string Activated { get { return Curse; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsBS1.PowerWorks(ctx.Pokemon) && ctx.OpponentPokemon().Count >= 2 && ctx.OpponentPokemon(m => m.DamageCounters > 0).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var sources = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (sources.Count == 0) return;
                var from = ctx.ChoosePokemon(sources, "Curse: move a damage counter from which of your opponent's Pokémon?");
                if (from == null) return;
                var targets = ctx.OpponentPokemon(m => m != from);
                if (targets.Count == 0) return;
                var to = ctx.ChoosePokemon(targets, "Curse: move it to which of your opponent's Pokémon?");
                if (to != null) ctx.Engine.MoveDamageCounters(from, to, 1, Curse);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Mind.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Strange Behavior: as often as you like during your turn, move 1 damage counter from 1 of your other Pokémon to Slowbro, as
        /// long as that doesn't Knock Slowbro Out. Psyshock: heads, Paralyzed.
        /// </summary>
        private sealed class SlowbroStrangeBehavior : Mon
        {
            private const string Behavior = "Strange Behavior";
            private static readonly Mon Psyshock = new ParalyzeFlipMon("Psyshock");

            private static List<PokemonInPlay> From(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.DamageCounters > 0);
            }

            private static bool CanMove(EffectContext ctx) { return ctx.RemainingHp(ctx.Pokemon) > 10 && From(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Behavior) return false;
                return ctx.IsMyTurn && CardsBS1.PowerWorks(ctx.Pokemon) && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, From(ctx), Behavior + ": move a damage counter from which of your Pokémon to " + ctx.Pokemon.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                return ctx.Engine.MoveDamageCounters(from, ctx.Pokemon, 1, Behavior) > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psyshock.HandleAttack(ctx, atk); }
        }

        /// <summary>Kabuto Armor: any attack's damage to Kabuto (after W/R) is halved, rounded down to the nearest 10.</summary>
        private sealed class KabutoArmor : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Target != src.Pokemon || !CardsBS1.PowerWorks(src.Pokemon)) return damage;
                return damage / 2 / 10 * 10;
            }
        }

        /// <summary>Cowardice: during your turn (not the turn it came into play), return Tentacool to your hand and discard all cards attached to it.</summary>
        private sealed class TentacoolCowardice : Mon
        {
            private const string Cowardice = "Cowardice";

            protected override string Activated { get { return Cowardice; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return CardsBS1.PowerWorks(me) && me.TurnEnteredPlay < ctx.Turn && ctx.Engine.CanPutIntoHand(me);
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var pokemonCards = me.Stack.ToList();
                var attached = me.Energy.Concat(me.Tools).ToList();
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.Hand.AddRange(pokemonCards);
                ctx.Engine.DiscardFromPlay(attached);
                ctx.Log(me.Name + " returned to " + ctx.Engine.Their(ctx.Player) + " hand" + (attached.Count > 0 ? " and its attached cards were discarded" : "") + " (Cowardice).");
            }
        }

        // ================================================================== Attacks

        /// <summary>Blizzard: heads, 10 to each of the opponent's Benched Pokémon; tails, 10 to each of yours (no W/R).</summary>
        internal sealed class BlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blizzard") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in (heads ? ctx.Opp.Bench : ctx.Me.Bench).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Prophecy: look at up to 3 cards from the top of either player's deck and rearrange them.</summary>
        internal sealed class ProphecyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Prophecy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsUNM2.ReorderEitherTop(ctx, 3, "Prophecy");
                return true;
            }
        }

        /// <summary>Wildfire: discard any number of Fire Energy cards from Moltres, then that many cards from the top of the opponent's deck.</summary>
        private sealed class WildfireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wildfire") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var fire = me.Energy.Where(c => c.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, me, c, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return true;
                var picks = ctx.ChooseCards(fire, 0, fire.Count, "Wildfire: discard how many Fire Energy cards? (Your opponent discards that many cards from the top of their deck.)");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Engine.DiscardEnergy(me, card)) n++;
                if (n > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }

        /// <summary>Gigashock: 10 damage to 3 of the opponent's Benched Pokémon (all of them if fewer), no W/R.</summary>
        private sealed class GigashockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigashock") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.Opp.Bench.ToList();
                if (bench.Count == 0) return true;
                int n = Math.Min(3, bench.Count);
                var picks = bench.Count <= 3 ? bench : ctx.ChoosePokemonMany(bench, n, n, "Gigashock: choose 3 of your opponent's Benched Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Thunderstorm: a coin for each of the opponent's Benched Pokémon, 20 to it on heads (no W/R); Zapdos takes 10 for each tails.</summary>
        private sealed class ThunderstormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderstorm") return false;
                ctx.DamageDefending(atk.Damage);
                int tails = 0;
                foreach (var mon in ctx.Opp.Bench.ToList())
                {
                    if (ctx.Flip()) { if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20); }
                    else tails++;
                }
                if (tails > 0 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10 * tails);
                return true;
            }
        }

        /// <summary>Terror Strike: flip a coin; heads, the opponent switches the Defending Pokémon with 1 of their Benched Pokémon (after the damage).</summary>
        private sealed class TerrorStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Terror Strike") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && ctx.Opp.Bench.Count > 0 && TefPokemon.StillDefending(ctx, defending) && !ctx.Engine.IsKnockedOut(defending)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Clamp: flip a coin; heads, 30 damage and Paralyzed; tails, nothing at all.</summary>
        internal sealed class ClampMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clamp") return false;
                var defending = ctx.Defending;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Energy Conversion: put up to 2 Energy cards from your discard pile into your hand; Gastly does 10 damage to itself.</summary>
        internal sealed class EnergyConversionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Conversion") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Energy Conversion: put up to 2 Energy cards from your discard pile into your hand"));
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Mr. Fuji: shuffle 1 of your Benched Pokémon and all cards attached to it into your deck.</summary>
        private sealed class MrFuji : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var bench = ctx.Me.Bench.ToList();
                if (bench.Count == 0) return;
                var target = ctx.ChoosePokemon(bench, "Mr. Fuji: shuffle which of your Benched Pokémon into your deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
            }
        }

        /// <summary>Gambler: shuffle your hand into your deck; flip a coin, heads draw 8 cards, tails draw 1.</summary>
        private sealed class Gambler : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Flip() ? 8 : 1);
            }
        }

        /// <summary>Recycle: flip a coin; heads, put a card from your discard pile on top of your deck (your opponent sees which).</summary>
        private sealed class Recycle : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var discard = ctx.Me.Discard.Where(c => c != ctx.Card).ToList();
                if (discard.Count == 0) return;
                var pick = ctx.ChooseCards(discard, 1, 1, "Recycle: put which card from your discard pile on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return;
                ctx.Me.Deck.Insert(0, pick);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { pick }, "card put on top of the deck (Recycle)");
            }
        }
    }
}
