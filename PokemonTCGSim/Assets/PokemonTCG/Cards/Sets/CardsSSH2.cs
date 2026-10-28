using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sword &amp; Shield (SSH, 2020), Pokémon 92-153 (Fighting, Darkness, Metal and Colorless), the Trainers and Aurora Energy.
    /// Regulation mark D: Expanded only.
    /// Already in the pool: Silicobra (107), Sandaconda (109), Clobbopus (111), Grapploct, Galarian Zigzagoon, Galarian Linoone,
    /// Galarian Obstagoon, Nickit, Thievul, Galarian Perrserker, Galarian Stunfisk, Corviknight, Zacian V, Zamazenta V, Minccino
    /// (145), Cinccino, Oranguru, Rookidee, Corvisquire, Wooloo (152), Dubwool, Cramorant V, Air Balloon, Bede, Big Charm,
    /// Crushing Hammer, Energy Retrieval, Energy Search, Energy Switch, Great Ball, Hop, Hyper Potion, Marnie, Pal Pad, Poké Kid,
    /// Pokégear 3.0, Pokémon Catcher, Pokémon Center Lady, Potion, Professor's Research, Quick Ball, Rare Candy, Rotom Bike,
    /// Switch, Team Yell Grunt and Vitality Band.
    /// Pokémon whose attacks only do their printed damage need no script (Diglett, Dugtrio, Rhyhorn 96, Baltoy, Mudbray,
    /// Silicobra 108, Clobbopus 112, Croagunk, Ferroseed, Pawniard, Snorlax, Minccino 146, Wooloo 153).
    /// </summary>
    public static class CardsSSH2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("SSH-94", new CardsASR1.PlusMon("Coordinated Strike", 80, HitmonchanOnBench));
            r.RegisterByKey("SSH-95", new CardsASR1.PlusMon("Coordinated Beatdown", 20, HitmonleeOnBench));
            r.RegisterByKey("SSH-97", new CardsSSP2.StompOffMon("Stomp Off", 2));
            r.RegisterByKey("SSH-98", new CardsFST1.TwoBenchedMon("Rock Slide", 10));
            r.RegisterByKey("SSH-99", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Tumble"), new BedrockShakeMon()));
            r.RegisterByKey("SSH-100", new ComboMon(new DrawAttackMon("Double Draw", 2), new CardsASR1.TimesMon("Flail", MyCounters)));
            r.RegisterByKey("SSH-103", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new RecoilMon("Explosion", 120)));
            r.RegisterByKey("SSH-104", new ComboMon(new CardsASR1.PlusMon("Raging Hammer", 10, MyCounters), new RecoilMon("Rocky Tackle", 30)));
            r.RegisterByKey("SSH-106", new CoinsMon("Double Impact", 2));
            r.RegisterByKey("SSH-110", new CardsFST1.TakeLessMon(30, new PowerPressMon()));
            r.RegisterByKey("SSH-114", new RecoilMon("Wild Tackle", 10));
            r.RegisterByKey("SSH-115", new ShieldAttackMon("Guard Press", 20));
            r.RegisterByKey("SSH-116", new StoneGiftMon());
            // Darkness
            r.RegisterByKey("SSH-120", new ComboMon(new LodeSearchMon(), new CardsMEW1.CounterBonusMon("Crazy Claws", 60)));
            r.RegisterByKey("SSH-121", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("SSH-122", new ComboMon(new ParalyzeFlipMon("Hard Press"), new ConditionAttackMon("Toxic Strike", SpecialCondition.Poisoned)));
            r.RegisterByKey("SSH-124", new ToxicroakSSH());
            // Metal
            r.RegisterByKey("SSH-127", new CardsRCL1.SetupBoostMon("Hone Claws", "Slash", 60));
            r.RegisterByKey("SSH-129", new ComboMon(new SearchAttackMon("Find a Friend", IsPokemonCard, 1), new CardsDRI2.CrunchMon("Crunch")));
            r.RegisterByKey("SSH-131", new ComboMon(new CoinsMon("Triple Smash", 3), new EnergySlapMon()));
            r.RegisterByKey("SSH-134", new CardsASR1.PlusMon("Charge Order", 30, BenchedPawniard));
            r.RegisterByKey("SSH-136", new FlipBonusMon("Stomp", 20));
            r.RegisterByKey("SSH-137", new ComboMon(new CardsTWM1.HealSelfMon("Dig Drain", 30), new MuscularNoseMon()));
            // Colorless
            r.RegisterByKey("SSH-141", new ComboMon(new CardsDAA1.DrainMon("Swallow"), new CardsRCL2.SelfConditionMon("Falling Down", SpecialCondition.Asleep)));
            r.RegisterByKey("SSH-142", new CardsASR1.PlusMon("G-Max Fall", 30, MyBenchCount));
            r.RegisterByKey("SSH-143", new CardsDRI1.WhirlwindMon("Send Back"));
            r.RegisterByKey("SSH-144", new CardsMEW1.ByeByeFlightMon("Carry Off"));
            r.RegisterByKey("SSH-149", new CardsCRZ1.DragonPulseMon());

            // Trainers
            r.RegisterByName("Evolution Incense", new EvolutionIncense());
            r.RegisterByName("Lucky Egg", new LuckyEgg());
            r.RegisterByName("Lum Berry", new LumBerry());
            r.RegisterByName("Metal Saucer", new MetalSaucer());
            r.RegisterByName("Ordinary Rod", new OrdinaryRod());
            r.RegisterByName("Sitrus Berry", new SitrusBerry());
            // Special Energy
            r.RegisterByName("Aurora Energy", new AuroraEnergy());
        }

        private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static int MyBenchCount(EffectContext ctx) { return ctx.Me.Bench.Count; }
        private static int MyCounters(EffectContext ctx) { return ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters; }
        private static int HitmonchanOnBench(EffectContext ctx) { return ctx.Me.Bench.Any(m => m.Name == "Hitmonchan") ? 1 : 0; }
        private static int HitmonleeOnBench(EffectContext ctx) { return ctx.Me.Bench.Any(m => m.Name == "Hitmonlee") ? 1 : 0; }
        private static int BenchedPawniard(EffectContext ctx) { return ctx.Me.Bench.Count(m => m.Name == "Pawniard"); }

        // ================================================================== Fighting

        /// <summary>
        /// Bedrock Shake: also 60 damage to each Benched Pokémon (both players') that has any damage counters on it, no Weakness or
        /// Resistance. Which ones have counters is checked as the attack starts; the damage to the Active Pokémon doesn't change it.
        /// </summary>
        private sealed class BedrockShakeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bedrock Shake") return false;
                var hurt = ctx.MyBench().Concat(ctx.OpponentBench()).Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in hurt)
                    if (ctx.InPlay(mon) && !ctx.IsActive(mon)) ctx.DamageTarget(mon, 60);
                return true;
            }
        }

        /// <summary>Power Press: 70 more damage if this Pokémon has at least 1 Fighting Energy more than the attack's cost needs.</summary>
        private sealed class PowerPressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Press") return false;
                int fighting = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fighting);
                int needed = atk.Cost.Count(t => t == EnergyType.Fighting);
                ctx.DamageDefending(atk.Damage + (fighting > needed ? 70 : 0));
                return true;
            }
        }

        /// <summary>Stone Gift: attach a basic Fighting Energy card from your hand to 1 of your Pokémon; if you do, heal 120 damage from it.</summary>
        private sealed class StoneGiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stone Gift") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fighting)).ToList();
                if (energy.Count == 0) return true;
                var targets = ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Stone Gift: attach a Fighting Energy card from your hand to which of your Pokémon? (It heals 120 damage.)");
                if (target == null) return true;
                var card = energy[0];
                if (!ctx.Me.Hand.Remove(card)) return true;
                if (ctx.Engine.AttachEnergyCard(card, target, true) && ctx.InPlay(target)) ctx.Heal(target, 120);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Lode Search: put a Trainer card from your discard pile into your hand.</summary>
        private sealed class LodeSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lode Search") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(trainers, 1, 1, "Lode Search: put a Trainer card from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>
        /// More Poison: 2 more damage counters on the opponent's Poisoned Pokémon in Pokémon Checkup; it stacks, two Toxicroak add 4
        /// (Compendium, 2020-02-06). Poison Claws: flip a coin; if heads, Poisoned.
        /// </summary>
        private sealed class ToxicroakSSH : Mon
        {
            private static readonly Mon Claws = new CardsPAR2.FlipConditionMon("Poison Claws", SpecialCondition.Poisoned);

            public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                if (src.Pokemon == null || mon.Owner == src.Player) return counters;
                // Fusion Strike Energy blocks it: it's an effect of an Ability on that Pokémon (as Radiant Hisuian Sneasler's Poison Peak).
                if (src.Engine.EffectsPrevented(mon, EffectSourceKind.Ability, src.Player, true)) return counters;
                return counters + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Energy Slap: you may move all Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class EnergySlapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Slap") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Energy Slap: move all Energy from " + me.Name + " to 1 of your Benched Pokémon?")) return true;
                var bench = ctx.MyBench().Where(b => b != me).ToList();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Energy Slap: move all Energy from " + me.Name + " to which Benched Pokémon?");
                if (target == null) return true;
                foreach (var e in me.Energy.ToList()) ctx.Engine.MoveEnergy(me, target, e);
                return true;
            }
        }

        /// <summary>Muscular Nose: if this Pokémon has 8 or more damage counters on it, this attack does nothing.</summary>
        private sealed class MuscularNoseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Muscular Nose") return false;
                if (ctx.Pokemon != null && ctx.Pokemon.DamageCounters >= 8)
                {
                    ctx.Log("Muscular Nose does nothing: " + ctx.Pokemon.Name + " has 8 or more damage counters.");
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Colorless

        // ================================================================== Trainers

        /// <summary>Evolution Incense: search your deck for an Evolution Pokémon, reveal it, and put it into your hand.</summary>
        private sealed class EvolutionIncense : CardScript
        {
            private static bool IsEvolutionPokemon(CardDef d) { return d.IsEvolution; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(IsEvolutionPokemon, 1, "Evolution Incense: choose an Evolution Pokémon");
            }
        }

        /// <summary>
        /// Lucky Egg: if its Pokémon is Knocked Out by damage from an opponent's attack, draw until you have 7 cards in your hand.
        /// This printing is played as written: the 2010 erratum ("only if it goes to the discard pile") doesn't apply (2024-12-12).
        /// </summary>
        private sealed class LuckyEgg : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Me.Hand.Count >= 7 || src.Me.Deck.Count == 0) return;
                src.Log("Lucky Egg: " + src.Engine.PlayerName(src.Player) + " draws until they have 7 cards in their hand.");
                src.DrawUntil(7);
            }
        }

        /// <summary>Lum Berry: at the end of each turn, if its Pokémon has any Special Condition, it recovers from all of them and this card is discarded.</summary>
        private sealed class LumBerry : CardScript
        {
            public override void OnEndOfEachTurn(EffectContext src)
            {
                var mon = src.Pokemon;
                if (mon == null || !mon.HasTool(src.Card) || mon.Conditions == SpecialCondition.None) return;
                src.Log("Lum Berry: " + mon.Name + " recovers from its Special Conditions.");
                src.Engine.RemoveAllConditions(mon);
                src.Engine.DiscardTool(mon, src.Card);
            }
        }

        /// <summary>Sitrus Berry: at the end of each turn, if its Pokémon has 3 or more damage counters, heal 30 damage from it and discard this card.</summary>
        private sealed class SitrusBerry : CardScript
        {
            public override void OnEndOfEachTurn(EffectContext src)
            {
                var mon = src.Pokemon;
                if (mon == null || !mon.HasTool(src.Card) || mon.DamageCounters < 3) return;
                // As Emergency Jelly: discarded only when it healed.
                int healed = src.Heal(mon, 30);
                if (healed > 0) src.Engine.DiscardTool(mon, src.Card);
            }
        }

        /// <summary>Metal Saucer: attach a basic Metal Energy card from your discard pile to 1 of your Benched Metal Pokémon.</summary>
        private sealed class MetalSaucer : CardScript
        {
            private static bool MetalCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Metal); }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Metal)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => MetalCard(c.Def)) && Targets(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = ctx.Me.Discard.FirstOrDefault(c => MetalCard(c.Def));
                var targets = Targets(ctx);
                if (energy == null || targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Metal Saucer: attach a Metal Energy card from your discard pile to which Benched Metal Pokémon?");
                if (target == null) return;
                if (ctx.Me.Discard.Remove(energy)) ctx.Engine.AttachEnergyCard(energy, target);
            }
        }

        /// <summary>
        /// Ordinary Rod: choose 1 or both: shuffle up to 2 Pokémon from your discard pile into your deck; shuffle up to 2 basic Energy
        /// cards from your discard pile into your deck. Playable when there's at least one of those to shuffle.
        /// </summary>
        private sealed class OrdinaryRod : CardScript
        {
            private static bool PokemonCard(CardDef d) { return d.IsPokemon; }
            private static bool BasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => PokemonCard(c.Def) || BasicEnergyCard(c.Def));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pokemon = ctx.Me.Discard.Where(c => PokemonCard(c.Def)).ToList();
                var energy = ctx.Me.Discard.Where(c => BasicEnergyCard(c.Def)).ToList();
                bool doPokemon = pokemon.Count > 0;
                bool doEnergy = energy.Count > 0;
                if (doPokemon && doEnergy)
                {
                    int pick = ctx.ChooseOption("Ordinary Rod: choose 1 or both", "Both", "Pokémon only", "Basic Energy only");
                    doPokemon = pick != 2;
                    doEnergy = pick != 1;
                }
                var chosen = new List<CardInstance>();
                if (doPokemon) chosen.AddRange(ctx.ChooseCards(pokemon, 1, Math.Min(2, pokemon.Count), "Ordinary Rod: shuffle up to 2 Pokémon from your discard pile into your deck"));
                if (doEnergy) chosen.AddRange(ctx.ChooseCards(energy, 1, Math.Min(2, energy.Count), "Ordinary Rod: shuffle up to 2 basic Energy cards from your discard pile into your deck"));
                if (chosen.Count > 0) ctx.DiscardToDeck(chosen);
                ctx.ShuffleMyDeck();
            }
        }

        // ================================================================== Special Energy

        /// <summary>
        /// Aurora Energy: it can be attached from the hand only by discarding another card from the hand (the engine asks for that
        /// card, GameEngine.DiscardToAttach); attached, it provides every type of Energy, 1 at a time. In the hand it has no type
        /// (Compendium, 2020-02-06), and moving it isn't attaching it from the hand (2022-02-24). Goodra's Slimy Room flips first:
        /// on tails it's discarded without the other card (your ruling, v66).
        /// </summary>
        private sealed class AuroraEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 1); }

            public override bool DiscardToAttachFromHand(EffectContext ctx) { return true; }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target)
            {
                // From the hand there must be another card there to discard.
                return ctx.Card == null || !ctx.Me.Hand.Contains(ctx.Card) || ctx.Me.Hand.Count >= 2;
            }
        }
    }
}
