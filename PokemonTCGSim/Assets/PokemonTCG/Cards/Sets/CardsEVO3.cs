using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Evolutions (EVO, v92), part 3: the Fairy and Colorless Pokémon (63-72), the Trainers that aren't already in the pool, and
    /// Imakuni?'s Doduo (112, Unlimited only). Spirit Link's effect is in GameEngine.EvolvePokemon (SpiritLinkFor).
    /// </summary>
    public static class CardsEVO3
    {
        internal static readonly string[] SpiritLinks = { "Blastoise Spirit Link", "Charizard Spirit Link", "Pidgeot Spirit Link", "Slowbro Spirit Link", "Venusaur Spirit Link" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("EVO-63", new ComboMon(new CardsPAR2.FlipConditionMon("Sing", SpecialCondition.Asleep), new MetronomeMon("Metronome")));
            r.RegisterByKey("EVO-64", new ComboMon(new MirrorMoveMon("Mirror Move"), new CardsMEG.SnipeBenchMon("Feather Lance", 20)));
            r.RegisterByKey("EVO-65", new MachCycloneMon());
            r.RegisterByKey("EVO-66", new RattataEVO());
            r.RegisterByKey("EVO-67", new ComboMon(new CardsDRI2.CrunchMon("Crunch"), new CardsASR1.TimesMon("Shadowy Bite", c => c.Opp.Discard.Count(x => x.Def.IsSpecialEnergy))));
            r.RegisterByKey("EVO-68", new LockAfterAttackMon("Leek Slap", true));
            r.RegisterByKey("EVO-69", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("EVO-70", new ComboMon(new CardsSP1.FlipNoDamageMon("Scrunch"), new RecoilMon("Double-Edge", 80)));
            r.RegisterByKey("EVO-71", new Conversion3Mon());
            r.RegisterByKey("EVO-72", new DragoniteEX());
            r.RegisterByKey("EVO-112", new ImakunisDoduo());

            foreach (var link in SpiritLinks) r.RegisterByName(link, new SpiritLink());
            r.RegisterByName("Devolution Spray", new DevolutionSpray());
            r.RegisterByName("Maintenance", new Maintenance());
            r.RegisterByName("Pokédex", new Pokedex());
            r.RegisterByName("Professor Oak's Hint", new ProfessorOaksHint());
            r.RegisterByName("Revive", new Revive());
        }

        // ================================================================== Pokémon

        /// <summary>Metronome: choose 1 of the opponent's Active Pokémon's attacks and use it as this attack (a GX attack uses up yours). Used for Metronome, Phantom Gate, Foul Play (the attack name is passed in).</summary>
        internal sealed class MetronomeMon : Mon
        {
            private readonly string attack;

            public MetronomeMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                CardDef copied;
                int index;
                if (ChooseCopiedAttack(ctx, defending, attack + ": choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index))
                    UseCopiedAttack(ctx, copied, index);
                return true;
            }
        }

        /// <summary>Mirror Move: if this Pokémon was damaged by an attack during the opponent's last turn, the same amount of damage to the opponent's Active Pokémon (damage taken on the Bench counts: Compendium, 2016-11-03). Used for Mirror Move, Vengeful Wish (the attack name is passed in).</summary>
        internal sealed class MirrorMoveMon : Mon
        {
            private readonly string attack;

            public MirrorMoveMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (me.AttackDamageTurn != ctx.OpponentsLastTurn || me.AttackDamageThatTurn <= 0) return Nothing(ctx);
                ctx.DamageDefending(me.AttackDamageThatTurn);
                return true;
            }
        }

        /// <summary>Mach Cyclone: you may have the opponent switch their Active Pokémon with 1 of their Benched Pokémon (they choose).</summary>
        private sealed class MachCycloneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null
                    && ctx.AskYesNo("Mach Cyclone: have your opponent switch their Active Pokémon with 1 of their Benched Pokémon?"))
                    ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Mischievous Fang: when you play this Pokémon from your hand onto your Bench, you may discard all Pokémon Tools from the opponent's Active Pokémon.</summary>
        private sealed class RattataEVO : Mon
        {
            internal const string Fang = "Mischievous Fang";

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                bool ready = target != null && target.Tools.Count > 0 && ctx.CanAffect(target);
                if (!TriggerOnce(ctx, Fang, ready, "Mischievous Fang: discard all Pokémon Tools from " + (target == null ? "" : target.Name) + "?")) return;
                ctx.Engine.DiscardAllTools(target);
            }
        }

        /// <summary>
        /// Conversion 3: choose a type; the Defending Pokémon's Weakness is that type until the end of your next turn (a Pokémon
        /// with no printed Weakness gets ×2 Weakness to it: Compendium, 2016-12-01). It ends if that Pokémon leaves the Active Spot.
        /// </summary>
        private sealed class Conversion3Mon : Mon
        {
            internal static readonly EnergyType[] Types =
            {
                EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic,
                EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Fairy, EnergyType.Dragon
            };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Conversion 3") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                int pick = ctx.ChooseOption("Conversion 3: choose " + defending.Name + "'s new Weakness", Types.Select(t => t.ToString()).ToArray());
                var type = Types[Math.Max(0, Math.Min(Types.Length - 1, pick))];
                ctx.AddEffect(new Cards30C.WeaknessChangeEffect(defending.Name + "'s Weakness is " + type + " until the end of its opponent's next turn (Conversion 3)",
                    ctx.Player, defending.Uid, ctx.Turn + 2, type));
                return true;
            }
        }

        /// <summary>
        /// Pull Up: when you play this Pokémon from your hand onto your Bench, you may put 2 Basic Pokémon (not Dragonite-EX) from
        /// your discard pile into your hand (just 1 if that's all there is). Hyper Beam: discard an Energy from the opponent's Active Pokémon.
        /// </summary>
        private sealed class DragoniteEX : Mon
        {
            internal const string PullUp = "Pull Up";
            private static readonly Mon Beam = new CardsDRI2.CrunchMon("Hyper Beam");

            private static List<CardInstance> Pullable(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && c.Def.Name != "Dragonite-EX").ToList();
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var pile = Pullable(ctx);
                if (!TriggerOnce(ctx, PullUp, pile.Count > 0, "Pull Up: put 2 Basic Pokémon from your discard pile into your hand?")) return;
                int n = Math.Min(2, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Pull Up: choose 2 Basic Pokémon (not Dragonite-EX) to put into your hand");
                ctx.DiscardToHand(picks);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Beam.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Frenzied Escape (your ruling, v92): when this Pokémon retreats, it's thrown out of the game: it goes to the Lost Zone and
        /// the cards attached to it are discarded (no Prize card; the game screen shows it spinning into the Lost Zone). Harmonize:
        /// its damage is done when the song ends, to whatever is Active then (Compendium, 2016-11-03), so here right away.
        /// </summary>
        private sealed class ImakunisDoduo : Mon
        {
            internal const string Escape = "Frenzied Escape";

            public override void OnRetreated(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !ctx.InPlay(me) || ctx.IsActive(me)) return;
                Announce(ctx, Escape);
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.LostZone.AddRange(me.Stack);
                bool attached = me.Energy.Count + me.Tools.Count > 0;
                ctx.Engine.DiscardFromPlay(me.Energy.ToList());
                ctx.Engine.DiscardFromPlay(me.Tools.ToList());
                ctx.Log(me.Name + " ran away into the Lost Zone" + (attached ? "; its attached cards were discarded." : ".") + " (Frenzied Escape)");
            }
        }

        // ================================================================== Trainers

        /// <summary>Spirit Link (Pokémon Tool): the turn doesn't end when its Pokémon becomes the matching M Pokémon-EX (engine).</summary>
        internal sealed class SpiritLink : CardScript
        {
        }

        /// <summary>Devolution Spray: devolve 1 of your evolved Pokémon, the top Evolution card into your hand; it can't evolve this turn.</summary>
        private sealed class DevolutionSpray : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Stack.Count > 1 && m.Def.IsEvolution && !m.Def.IsMultiCard);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Devolution Spray: devolve which of your Pokémon?");
                if (target == null) return;
                ctx.Engine.Devolve(target, 1);
                target.CantEvolveOnTurn = ctx.Turn;
            }
        }

        /// <summary>Maintenance: shuffle 2 other cards from your hand into your deck (it can't be played without them), then draw a card.</summary>
        private sealed class Maintenance : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count < 2) return;
                var picks = hand.Count == 2 ? hand : ctx.ChooseCards(hand, 2, 2, "Maintenance: shuffle 2 cards from your hand into your deck");
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled 2 cards from " + ctx.Engine.Their(ctx.Player) + " hand into " + ctx.Engine.Their(ctx.Player) + " deck (Maintenance).");
                ctx.Draw(1);
            }
        }

        /// <summary>Pokédex: look at the top 5 cards of your deck and put them back in any order.</summary>
        private sealed class Pokedex : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { CardsOBF2.ReorderOwnTop(ctx, 5, "Pokédex"); }
        }

        /// <summary>Professor Oak's Hint (Supporter): draw cards until you have 7 cards in your hand. Your turn ends.</summary>
        private sealed class ProfessorOaksHint : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.DrawUntil(7);
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Revive: put a Basic Pokémon from your discard pile onto your Bench.</summary>
        private sealed class Revive : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && Basics(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0 || ctx.BenchSpace <= 0) return;
                var pick = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Revive: put which Basic Pokémon from your discard pile onto your Bench?")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Engine.PutOntoBench(ctx.Player, pick, false);
            }
        }
    }
}
