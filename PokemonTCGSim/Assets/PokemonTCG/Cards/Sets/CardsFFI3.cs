using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Furious Fists (FFI, v103), part 3: the Trainers and Special Energy (88-104). Energy Switch, Full Heal, Maintenance, Super Scoop
    /// Up and Strong Energy were already in the pool.
    /// </summary>
    public static class CardsFFI3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Battle Reporter", new BattleReporter());
            r.RegisterByName("Fighting Stadium", new FightingStadium());
            r.RegisterByName("Focus Sash", new FocusSash());
            r.RegisterByName("Fossil Researcher", new FossilResearcher());
            r.RegisterByName("Jaw Fossil", new CardsSTS3.FossilItem("Tyrunt"));
            r.RegisterByName("Korrina", new Korrina());
            r.RegisterByName("Mountain Ring", new MountainRing());
            r.RegisterByName("Sail Fossil", new CardsSTS3.FossilItem("Amaura"));
            r.RegisterByName("Sparkling Robe", new SparklingRobe());
            r.RegisterByName("Tool Retriever", new ToolRetriever());
            r.RegisterByName("Training Center", new TrainingCenter());
            r.RegisterByName("Herbal Energy", new HerbalEnergy());
        }

        /// <summary>Battle Reporter: draw cards until you have as many cards in your hand as your opponent.</summary>
        private sealed class BattleReporter : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = ctx.Opp.Hand.Count - ctx.Me.Hand.Count;
                if (n > 0) ctx.Draw(n);
            }
        }

        /// <summary>Fighting Stadium: each player's Fighting Pokémon's attacks do 20 more to the opponent's Active Pokémon-EX (before Weakness and Resistance; 2014-11).</summary>
        private sealed class FightingStadium : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (attacker == null || !info.ToOpponentsActive || damage <= 0 || !info.Target.Def.IsPokemonEX || !src.Engine.IsType(attacker, EnergyType.Fighting)) return damage;
                return damage + 20;
            }
        }

        /// <summary>Focus Sash: a Fighting Pokémon with full HP that would be Knocked Out by the opponent's attack damage keeps 10 HP; then discard this card.</summary>
        private sealed class FocusSash : CardScript
        {
            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Fighting);
            }

            public override void OnSavedFromKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon != null && mon.Tools.Contains(src.Card)) src.Engine.DiscardTool(mon, src.Card);
            }
        }

        /// <summary>Fossil Researcher: search your deck for up to 2 Amaura and Tyrunt in any combination and put them onto your Bench.</summary>
        private sealed class FossilResearcher : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(2, ctx.BenchSpace);
                if (n <= 0) return;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && (d.Name == "Amaura" || d.Name == "Tyrunt") && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    "Fossil Researcher: choose up to 2 Amaura and Tyrunt");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Korrina: search your deck for a Fighting Pokémon and an Item card.</summary>
        private sealed class Korrina : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsPokemon && (d.PokemonType == EnergyType.Fighting || d.SecondType == EnergyType.Fighting), 1, "Korrina: choose a Fighting Pokémon");
                found.AddRange(ctx.SearchMyDeck(d => d.IsItem, 1, "Korrina: choose an Item card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Mountain Ring: no attack damage to Benched Pokémon (both players').</summary>
        private sealed class MountainRing : CardScript
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Attacker != null && info.Target != null && !info.TargetIsActive;
            }
        }

        /// <summary>Sparkling Robe: its Pokémon can't be affected by Special Conditions (and loses any it has).</summary>
        private sealed class SparklingRobe : CardScript
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }
        }

        /// <summary>Tool Retriever: put up to 2 Pokémon Tool cards attached to your Pokémon into your hand.</summary>
        private sealed class ToolRetriever : CardScript
        {
            private static List<KeyValuePair<PokemonInPlay, CardInstance>> Tools(EffectContext ctx)
            {
                var list = new List<KeyValuePair<PokemonInPlay, CardInstance>>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var tool in mon.Tools)
                        if (tool.Owner == ctx.Player) list.Add(new KeyValuePair<PokemonInPlay, CardInstance>(mon, tool));
                return list;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Tools(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var all = Tools(ctx);
                if (all.Count == 0) return;
                var picks = ctx.ChooseCards(all.Select(kv => kv.Value).ToList(), 1, Math.Min(2, all.Count), "Tool Retriever: put up to 2 Pokémon Tools into your hand");
                foreach (var tool in picks)
                {
                    var holder = all.First(kv => kv.Value == tool).Key;
                    if (!holder.Tools.Remove(tool)) continue;
                    ctx.Me.Hand.Add(tool);
                    ctx.Log(tool.Def.Name + " was put into " + ctx.Engine.PlayerName(ctx.Player) + "'s hand from " + holder.Name + " (Tool Retriever).");
                }
            }
        }

        /// <summary>Training Center: each Stage 1 and Stage 2 Pokémon in play gets +30 HP.</summary>
        private sealed class TrainingCenter : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null) return hp;
                var stage = mon.Def.Stage;
                return stage == Stage.Stage1 || stage == Stage.Stage2 ? hp + 30 : hp;
            }
        }

        /// <summary>Herbal Energy: Grass only, on a Grass Pokémon; attached from your hand to 1 of your Grass Pokémon, heal 30 from it.</summary>
        private sealed class HerbalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Grass)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Grass) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Grass); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Grass); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null || mon.Owner != ctx.Player || mon.DamageCounters == 0 || !ctx.Engine.IsType(mon, EnergyType.Grass)) return;
                ctx.Heal(mon, 30);
            }
        }
    }
}
