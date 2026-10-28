using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Magma vs Team Aqua Trainer and Energy cards (MA 69-88; v160). Dual Ball 72 is Expedition's and Warp Point 85 is Gym
    /// Challenge's (prints); Strength Charm 74 is its own card (your ruling). Archie and Maxie put the Pokémon onto the Bench as a
    /// Basic Pokémon, like Revive Fossil.
    /// </summary>
    public static class CardsMAT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MA-69", new Schemer(true));
            r.RegisterByKey("MA-70", new Schemer(false));
            r.RegisterByKey("MA-71", new ArchieOrMaxie(true));
            r.RegisterByKey("MA-73", new ArchieOrMaxie(false));
            r.RegisterByKey("MA-74", new StrengthCharmMA());
            r.RegisterByKey("MA-75", new TeamBall(true));
            r.RegisterByKey("MA-76", new TeamBelt(true));
            r.RegisterByKey("MA-77", new Conspirator(true));
            r.RegisterByKey("MA-78", new TeamAquaHideout());
            r.RegisterByKey("MA-79", new CardsE3T.AttachedTM((ctx, mon) => CardsMA.Aqua(mon.Name), Miracle));
            r.RegisterByKey("MA-80", new TeamBall(false));
            r.RegisterByKey("MA-81", new TeamBelt(false));
            r.RegisterByKey("MA-82", new Conspirator(false));
            r.RegisterByKey("MA-83", new TeamMagmaHideout());
            r.RegisterByKey("MA-84", new CardsE3T.AttachedTM((ctx, mon) => CardsMA.Magma(mon.Name), CrushingMagma));
            r.RegisterByKey("MA-86", new TeamEnergy(true));
            r.RegisterByKey("MA-87", new TeamEnergy(false));
            r.RegisterByKey("MA-88", new DoubleRainbowEnergy());
        }

        private static bool Team(bool aqua, string name) { return aqua ? CardsMA.Aqua(name) : CardsMA.Magma(name); }

        private static string TeamName(bool aqua) { return aqua ? "Team Aqua" : "Team Magma"; }

        /// <summary>Team Aqua / Team Magma Schemer (Supporter): discard a Pokémon from your hand, then draw 3 cards, or 4 if it had [Team] in its name.</summary>
        private sealed class Schemer : CardScript
        {
            private readonly bool aqua;

            public Schemer(bool aqua) { this.aqua = aqua; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pokemon = ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return;
                var gone = ctx.ChooseCards(pokemon, 1, 1, ctx.SourceName + ": discard which Pokémon from your hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, gone);
                ctx.Draw(Team(aqua, gone.Def.Name) ? 4 : 3);
            }
        }

        /// <summary>
        /// Archie (Supporter): a Pokémon with Team Aqua in its name from your deck onto your Bench. Maxie (Supporter): a Pokémon with
        /// Team Magma in its name from your hand or discard pile onto your Bench. As a Basic Pokémon; a Stage 2 gets 2 damage counters.
        /// </summary>
        private sealed class ArchieOrMaxie : CardScript
        {
            private readonly bool archie;

            public ArchieOrMaxie(bool archie) { this.archie = archie; }

            private bool Fits(CardDef d) { return d.IsPokemon && Team(archie, d.Name); }

            private List<CardInstance> Sources(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && Fits(c.Def)).Concat(ctx.Me.Discard.Where(c => Fits(c.Def))).Where(c => !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                if (ctx.BenchSpace <= 0) return false;
                return archie ? ctx.Me.Deck.Count > 0 : Sources(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.BenchSpace <= 0) return;
                CardInstance card = null;
                if (archie)
                {
                    var found = ctx.SearchMyDeck(d => Fits(d) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, "Archie: choose a Pokémon with Team Aqua in its name");
                    if (found.Count > 0) card = found[0];
                }
                else
                {
                    var options = Sources(ctx);
                    if (options.Count > 0) card = ctx.ChooseCards(options, 1, 1, "Maxie: choose a Pokémon with Team Magma in its name from your hand or discard pile")[0];
                    if (card != null && !ctx.Me.Hand.Remove(card) && !ctx.Me.Discard.Remove(card)) card = null;
                }
                if (card != null)
                {
                    var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    if (card.Def.Stage == Stage.Stage2 && ctx.InPlay(mon)) ctx.PlaceCounters(mon, 2);
                }
                if (archie) ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Strength Charm (Pokémon Tool, MA's own wording): its Pokémon's attack damage to the opponent's Active Pokémon is 10 more,
        /// after Weakness and Resistance; discarded at the end of the turn that happens.
        /// </summary>
        private sealed class StrengthCharmMA : CardScript
        {
            private const string Marker = "Strength Charm (MA) used";

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !info.ApplyOnce("Strength Charm (MA)")) return damage;
                holder.Markers[Marker] = src.Turn;
                return damage + 10;
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                int turn;
                if (holder == null || !holder.Markers.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                holder.Markers.Remove(Marker);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Team Aqua / Team Magma Ball: heads, any Pokémon with [Team] in its name from your deck; tails, a Basic one.</summary>
        private sealed class TeamBall : CardScript
        {
            private readonly bool aqua;

            public TeamBall(bool aqua) { this.aqua = aqua; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool heads = ctx.Flip();
                ctx.SearchToHand(d => d.IsPokemon && Team(aqua, d.Name) && (heads || d.IsBasicPokemon), 1,
                    ctx.SourceName + ": choose a " + (heads ? "" : "Basic ") + "Pokémon with " + TeamName(aqua) + " in its name");
            }
        }

        /// <summary>
        /// Team Aqua / Team Magma Belt (Pokémon Tool, only on a Pokémon with that team's name): between turns, while its Pokémon is your
        /// Active Pokémon, a card that evolves from it comes from your deck onto it, then the Belt is discarded. It waits while the
        /// deck has no such card.
        /// </summary>
        private sealed class TeamBelt : CardScript
        {
            private readonly bool aqua;

            public TeamBelt(bool aqua) { this.aqua = aqua; }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && Team(aqua, target.Name); }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || !holder.Tools.Contains(src.Card) || src.State.Players[holder.Owner].Active != holder) return;
                if (!src.Me.Deck.Any(c => CardsEVS1.EvolvesOnto(src, holder, c.Def))) return;
                CardsEVS1.EvolveFromDeck(src, holder, src.SourceName);
                if (src.InPlay(holder) && holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Team Aqua / Team Magma Conspirator (Supporter): up to 2 cards, Basic Pokémon with [Team] in their name and basic Energy cards in any mix.</summary>
        private sealed class Conspirator : CardScript
        {
            private readonly bool aqua;

            public Conspirator(bool aqua) { this.aqua = aqua; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicEnergy || (d.IsBasicPokemon && Team(aqua, d.Name)), 2,
                    ctx.SourceName + ": choose up to 2 Basic Pokémon with " + TeamName(aqua) + " in their names or basic Energy cards");
            }
        }

        /// <summary>Team Aqua Hideout (Stadium): each Pokémon without Team Aqua in its name has a Retreat Cost 1 more.</summary>
        private sealed class TeamAquaHideout : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && !CardsMA.Aqua(mon.Name) ? cost + 1 : cost;
            }
        }

        /// <summary>Team Magma Hideout (Stadium): a Basic Pokémon without Team Magma in its name played from the hand to the Bench gets 1 damage counter.</summary>
        private sealed class TeamMagmaHideout : CardScript
        {
            public override void OnPokemonPutOnBench(EffectContext ctx, PokemonInPlay benched)
            {
                if (benched == null || !ctx.Engine.BenchingFromHand || benched.Def.Stage != Stage.Basic || CardsMA.Magma(benched.Name)) return;
                ctx.Engine.PlaceDamageCounters(benched, 1, "Team Magma Hideout");
            }
        }

        /// <summary>Miracle (Team Aqua Technical Machine 01): 10 damage, then the Defending Pokémon is affected by a Special Condition you choose.</summary>
        private static void Miracle(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!TefPokemon.StillDefending(ctx, defending)) return;
            var choices = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
            int pick = ctx.ChooseOption("Miracle: choose a Special Condition", choices.Select(c => c.ToString()).ToArray());
            if (pick < 0 || pick >= choices.Length) pick = 0;
            ctx.ApplyCondition(defending, choices[pick]);
        }

        /// <summary>Crushing Magma (Team Magma Technical Machine 01): 10 damage, then an Energy card on the Defending Pokémon goes to the bottom of its owner's deck.</summary>
        private static void CrushingMagma(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return;
            var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Crushing Magma: put which Energy card on the bottom of your opponent's deck?")[0];
            if (ctx.Engine.EnergyRemovalProtected(defending, card))
            {
                ctx.Log(card.Def.Name + " on " + defending.Name + " can't be put into the deck by Crushing Magma.");
                return;
            }
            defending.Energy.Remove(card);
            ctx.State.Players[card.Owner].Deck.Add(card);
            ctx.Log(card.Def.Name + " from " + defending.Name + " was put on the bottom of " + ctx.Engine.PlayerName(card.Owner) + "'s deck (Crushing Magma).");
        }

        /// <summary>
        /// Aqua Energy / Magma Energy: only on a Pokémon with that team's name; 2 Energy, each Water (Fighting) or Darkness in any mix
        /// (your ruling for Aqua Energy); discarded at the end of your turn.
        /// </summary>
        private sealed class TeamEnergy : CardScript
        {
            private readonly bool aqua;

            public TeamEnergy(bool aqua) { this.aqua = aqua; }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                int mask = EnergyMask.Of(aqua ? EnergyType.Water : EnergyType.Fighting) | EnergyMask.Of(EnergyType.Darkness);
                return RecifeEnergy.Units(mask, 2);
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && Team(aqua, target.Name); }

            public override void OnEndOfTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder != null && holder.Energy.Contains(src.Card)) src.Engine.DiscardEnergy(holder, src.Card);
            }
        }

        /// <summary>
        /// Double Rainbow Energy: only on an Evolved Pokémon that isn't a Pokémon-ex; every type, 2 Energy at a time; its Pokémon's
        /// damage to the opponent's Pokémon is 10 less (after Weakness and Resistance); discarded when its Pokémon is no longer
        /// Evolved.
        /// </summary>
        private sealed class DoubleRainbowEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 2); }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return CardsE2.IsEvolved(target) && !target.Def.IsOldEx; }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !CardsE2.IsEvolved(ctx.Pokemon); }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || info.Target == null || info.Target.Owner == src.Player || damage <= 0) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? Math.Max(0, damage - 10) : damage;
            }
        }
    }
}
