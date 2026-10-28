using System;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Hidden Legends Trainer cards (HL 84-89, 91, 92; v161). Life Herb 90 is Celestial Storm's (a print, your ruling). Magnetic
    /// Storm 91 is its own card (Flashfire's has a different effect). The Ancient Technical Machines go on an Evolved Pokémon that
    /// isn't a Pokémon-ex and has no owner in its name.
    /// </summary>
    public static class CardsHLT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HL-84", new CardsE3T.AttachedTM(AncientTarget, IceGenerator));
            r.RegisterByKey("HL-85", new CardsE3T.AttachedTM(AncientTarget, StoneGenerator));
            r.RegisterByKey("HL-86", new CardsE3T.AttachedTM(AncientTarget, SteelGenerator));
            r.RegisterByKey("HL-87", new AncientTomb());
            r.RegisterByKey("HL-88", new DesertRuins());
            r.RegisterByKey("HL-89", new IslandCave());
            r.RegisterByKey("HL-91", new MagneticStormHL());
            r.RegisterByKey("HL-92", new StevensAdvice());
        }

        /// <summary>Not a Pokémon-ex and no owner in its name (Ancient Technical Machines, Ancient Tomb).</summary>
        private static bool Plain(PokemonInPlay mon) { return mon != null && !mon.Def.IsOldEx && !CardsHL.HasOwner(mon.Name); }

        private static bool AncientTarget(EffectContext ctx, PokemonInPlay mon) { return CardsE2.IsEvolved(mon) && Plain(mon); }

        /// <summary>Ice Generator: discard all the opponent's Trainer cards in play; if you do, prevent all effects of attacks, damage included, done to the attacker during their next turn.</summary>
        private static void IceGenerator(EffectContext ctx, AttackDef atk)
        {
            int gone = 0;
            foreach (var mon in ctx.OpponentPokemon())
            {
                if (!ctx.CanAffect(mon)) continue;
                gone += ctx.Engine.DiscardAllTools(mon);
                foreach (var card in mon.Attached.Where(c => c.Def.IsTrainer).ToList())
                    if (mon.Attached.Remove(card))
                    {
                        ctx.State.Players[card.Owner].Discard.Add(card);
                        gone++;
                    }
            }
            if (ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent)
            {
                ctx.Engine.DiscardStadium();
                gone++;
            }
            if (gone > 0) ctx.ProtectSelfNextTurn();
        }

        /// <summary>Stone Generator: the highest Stage Evolution card comes off each of the opponent's Evolved Pokémon, into their hand.</summary>
        private static void StoneGenerator(EffectContext ctx, AttackDef atk)
        {
            foreach (var mon in ctx.OpponentPokemon(CardsE2.IsEvolved))
                if (ctx.InPlay(mon) && ctx.CanAffect(mon)) ctx.Engine.Devolve(mon, 1);
        }

        /// <summary>
        /// Steel Generator: if the Defending Pokémon has a Poké-Power or Poké-Body, move up to 2 basic Energy cards from 1 of the
        /// opponent's Pokémon to the Defending Pokémon.
        /// </summary>
        private static void SteelGenerator(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            if (defending == null || !defending.Def.Abilities.Any(a => a.IsPokePowerOrBody)) return;
            var donors = ctx.OpponentPokemon(m => m != defending && m.Energy.Any(e => e.Def.IsBasicEnergy));
            if (donors.Count == 0) return;
            var from = ctx.ChoosePokemon(donors, "Steel Generator: move basic Energy cards from which of your opponent's Pokémon?");
            if (from == null || !ctx.CanAffect(from)) return;
            var basics = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
            foreach (var card in ctx.ChooseCards(basics, 1, Math.Min(2, basics.Count), "Steel Generator: move up to 2 basic Energy cards to " + defending.Name))
                ctx.Engine.MoveEnergy(from, defending, card);
        }

        /// <summary>Ancient Tomb (Stadium): no Weakness for any Pokémon in play except Pokémon-ex and Pokémon with an owner in their name.</summary>
        private sealed class AncientTomb : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return Plain(mon); }
        }

        /// <summary>Desert Ruins (Stadium): between turns, each player puts 1 damage counter on each of their Pokémon-ex with 100 HP or more.</summary>
        private sealed class DesertRuins : CardScript
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                for (int side = 0; side < 2; side++)
                    foreach (var mon in src.State.Players[side].InPlay().ToList())
                        if (mon.Def.IsOldEx && src.Engine.MaxHp(mon) >= 100 && src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Desert Ruins");
            }

            public override int CheckupAffects(EffectContext src) { return -1; }
        }

        /// <summary>Island Cave (Stadium): whenever a player attaches an Energy card from their hand to a Water, Fighting or Metal Pokémon, its Special Conditions are removed.</summary>
        private sealed class IslandCave : CardScript
        {
            private static void Cure(EffectContext src, PokemonInPlay target)
            {
                if (target == null || target.Conditions == SpecialCondition.None || !src.InPlay(target)) return;
                var e = src.Engine;
                if (!e.IsType(target, EnergyType.Water) && !e.IsType(target, EnergyType.Fighting) && !e.IsType(target, EnergyType.Metal)) return;
                src.Log("Island Cave: " + target.Name + " recovers from its Special Conditions.");
                e.RemoveAllConditions(target);
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Cure(src, target); }

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Cure(src, target); }
        }

        /// <summary>Magnetic Storm (Stadium, Hidden Legends): attack damage from Psychic and Fighting Pokémon isn't affected by Resistance.</summary>
        private sealed class MagneticStormHL : CardScript
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                var a = info.Attacker;
                if (a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return false;
                return src.Engine.IsType(a, EnergyType.Psychic) || src.Engine.IsType(a, EnergyType.Fighting);
            }
        }

        /// <summary>Steven's Advice (Supporter): draw up to as many cards as the opponent has Pokémon in play; not with more than 7 cards in your hand (this one included).</summary>
        private sealed class StevensAdvice : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count <= 7 && ctx.Me.Deck.Count > 0 && ctx.OpponentPokemon().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(ctx.OpponentPokemon().Count, ctx.Me.Deck.Count);
                if (n > 0) ctx.Draw(n == 1 ? 1 : ctx.AskNumber(0, n, "Steven's Advice: draw how many cards (up to " + n + ")?"));
            }
        }
    }
}
