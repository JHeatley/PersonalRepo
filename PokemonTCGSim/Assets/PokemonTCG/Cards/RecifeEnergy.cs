using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Special Energy cards. Basic Energy needs no script.</summary>
    public static class RecifeEnergy
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Boomerang Energy", new BoomerangEnergy());
            r.RegisterByName("Enriching Energy", new EnrichingEnergy());
            r.RegisterByName("Growing Grass Energy", new GrowingGrassEnergy());
            r.RegisterByName("Ignition Energy", new IgnitionEnergy());
            r.RegisterByName("Legacy Energy", new LegacyEnergy());
            r.RegisterByName("Mist Energy", new MistEnergy());
            r.RegisterByName("Neo Upper Energy", new NeoUpperEnergy());
            r.RegisterByName("Prism Energy", new PrismEnergy());
            r.RegisterByName("Rocky Fighting Energy", new RockyFightingEnergy());
            r.RegisterByName("Spiky Energy", new SpikyEnergy());
            r.RegisterByName("Team Rocket's Energy", new TeamRocketsEnergy());
            r.RegisterByName("Telepathic Psychic Energy", new TelepathicPsychicEnergy());
        }

        public static List<int> Units(int unit, int count)
        {
            var list = new List<int>();
            for (int i = 0; i < count; i++) list.Add(unit);
            return list;
        }
    }

    public sealed class BoomerangEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }
        public override bool ReturnsAfterAttackDiscard(EffectContext ctx) { return true; }
    }

    public sealed class EnrichingEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }
        public override void OnAttachedFromHand(EffectContext ctx) { ctx.Draw(4); }
    }

    public sealed class GrowingGrassEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Grass), 1); }

        public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
        {
            return mon == src.Pokemon && mon.Def.PokemonType == EnergyType.Grass ? hp + 20 : hp;
        }
    }

    /// <summary>Colorless, or 3 Colorless on an Evolution Pokémon. Discarded at the end of its owner's turn.</summary>
    public sealed class IgnitionEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx)
        {
            return RecifeEnergy.Units(EnergyMask.Colorless, ctx.Pokemon != null && ctx.Pokemon.Def.IsEvolution ? 3 : 1);
        }

        public override void OnEndOfTurn(EffectContext src)
        {
            if (src.Pokemon != null && src.Pokemon.Energy.Contains(src.Card)) src.Engine.DiscardEnergy(src.Pokemon, src.Card);
        }
    }

    /// <summary>Any type (1 at a time). Once per game: 1 fewer Prize when its holder is Knocked Out by an opponent's attack damage.</summary>
    public sealed class LegacyEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 1); }

        public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
        {
            if (knockedOut != src.Pokemon || taker == src.Player || src.Me.LegacyEnergyUsed) return prizes;
            if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
            src.Me.LegacyEnergyUsed = true;
            src.Log("Legacy Energy: " + src.Engine.PlayerName(taker) + " takes 1 fewer Prize card.");
            return prizes - 1;
        }
    }

    public sealed class MistEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return target == src.Pokemon && kind == EffectSourceKind.Attack;
        }
    }

    /// <summary>Colorless, or 2 Energy of any type on a Stage 2 Pokémon.</summary>
    public sealed class NeoUpperEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx)
        {
            bool stage2 = ctx.Pokemon != null && ctx.Pokemon.Def.Stage == Stage.Stage2;
            return stage2 ? RecifeEnergy.Units(EnergyMask.AnyType, 2) : RecifeEnergy.Units(EnergyMask.Colorless, 1);
        }
    }

    /// <summary>Colorless, or 1 Energy of any type on a Basic Pokémon.</summary>
    public sealed class PrismEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx)
        {
            bool basic = ctx.Pokemon != null && ctx.Pokemon.Def.IsBasicPokemon;
            return RecifeEnergy.Units(basic ? EnergyMask.AnyType : EnergyMask.Colorless, 1);
        }
    }

    public sealed class RockyFightingEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Fighting), 1); }

        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return target == src.Pokemon && kind == EffectSourceKind.Attack && src.Engine.IsType(target, EnergyType.Fighting);
        }
    }

    public sealed class SpikyEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (holderWasActive) ctx.Engine.PlaceDamageCounters(attacker, 2, "Spiky Energy");
        }
    }

    /// <summary>Only on Team Rocket's Pokémon; provides 2 Energy, each Psychic or Darkness.</summary>
    public sealed class TeamRocketsEnergy : CardScript
    {
        private static readonly int PsychicOrDarkness = EnergyMask.Of(EnergyType.Psychic) | EnergyMask.Of(EnergyType.Darkness);

        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(PsychicOrDarkness, 2); }
        public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target.Def.IsTeamRockets; }
        public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Pokemon.Def.IsTeamRockets; }
    }

    public sealed class TelepathicPsychicEnergy : CardScript
    {
        public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Psychic), 1); }

        public override void OnAttachedFromHand(EffectContext ctx)
        {
            if (ctx.Pokemon.Def.PokemonType != EnergyType.Psychic || ctx.Me.Deck.Count == 0) return;
            if (ctx.BenchSpace == 0)
            {
                ctx.ShuffleMyDeck();
                return;
            }
            ctx.SearchToBench(d => d.PokemonType == EnergyType.Psychic, 2, "Telepathic Psychic Energy: put up to 2 Basic Psychic Pokémon onto your Bench");
        }
    }
}
