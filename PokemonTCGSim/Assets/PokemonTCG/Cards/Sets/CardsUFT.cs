using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unseen Forces Trainer cards (UF 80, 83, 85, 86, 90, 91, 92; v168). The others are prints. Sitrus Berry 91 is its own card
    /// (your ruling). The Tools that only go on some Pokémon are discarded as soon as their Pokémon no longer fits.
    /// </summary>
    public static class CardsUFT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UF-80", new CursePowder());
            r.RegisterByKey("UF-83", new EnergyRoot());
            r.RegisterByKey("UF-85", new FluffyBerry());
            r.RegisterByKey("UF-86", new MarysRequest());
            r.RegisterByKey("UF-90", new ProtectiveOrb());
            r.RegisterByKey("UF-91", new SitrusBerryUF());
            r.RegisterByKey("UF-92", new SolidRage());
        }

        /// <summary>"1 of your Evolved Pokémon (excluding Pokémon-ex)"; discarded if it's a Basic Pokémon or Pokémon-ex.</summary>
        private static bool EvolvedNotEx(PokemonInPlay mon) { return mon != null && CardsE2.IsEvolved(mon) && !mon.Def.IsOldEx; }

        /// <summary>"1 of your Pokémon (excluding Pokémon-ex and Pokémon that has Dark or an owner in its name)".</summary>
        private static bool PlainPokemon(PokemonInPlay mon)
        {
            return mon != null && !mon.Def.IsOldEx && !CardsTRR.Dark(mon.Name) && !CardsHL.HasOwner(mon.Name);
        }

        /// <summary>The Pokémon this Tool is attached to, or null.</summary>
        private static PokemonInPlay Holder(EffectContext src)
        {
            var mon = src.Pokemon;
            return mon != null && mon.Tools.Contains(src.Card) ? mon : null;
        }

        /// <summary>Curse Powder: if its Pokémon is your Active Pokémon and is Knocked Out by damage from an opponent's attack, 3 damage counters on the Attacking Pokémon.</summary>
        private sealed class CursePowder : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return EvolvedNotEx(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !EvolvedNotEx(ctx.Pokemon); }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var mon = src.Pokemon;
                if (mon == null || !byOpponentsAttack || !wasActive) return;
                var attacker = src.Engine.AttackingPokemonOf(mon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                src.Log("Curse Powder: 3 damage counters on " + attacker.Name + ".");
                src.PlaceCounters(attacker, 3);
            }
        }

        /// <summary>Energy Root: its Pokémon gets +20 HP and can't use any Poké-Powers or Poké-Bodies.</summary>
        private sealed class EnergyRoot : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return PlainPokemon(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !PlainPokemon(ctx.Pokemon); }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon != null && mon == Holder(src) ? hp + 20 : hp;
            }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return mon != null && mon == Holder(src); }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName) { return mon != null && mon == Holder(src); }
        }

        /// <summary>Fluffy Berry: its Pokémon's Retreat Cost is 0.</summary>
        private sealed class FluffyBerry : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return PlainPokemon(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !PlainPokemon(ctx.Pokemon); }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == Holder(src) ? 0 : cost;
            }
        }

        /// <summary>Protective Orb: its Pokémon has no Weakness.</summary>
        private sealed class ProtectiveOrb : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return EvolvedNotEx(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !EvolvedNotEx(ctx.Pokemon); }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && mon == Holder(src); }
        }

        /// <summary>
        /// Sitrus Berry (Unseen Forces): at any time between turns, with 3 or more damage counters on its Pokémon, 3 come off and the
        /// berry is discarded (like Neo Genesis Berry: you may between turns, and it happens at the start of the next turn otherwise).
        /// </summary>
        private sealed class SitrusBerryUF : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return PlainPokemon(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !PlainPokemon(ctx.Pokemon); }

            public override void OnBetweenTurns(EffectContext src, bool startOfTurn)
            {
                var holder = Holder(src);
                if (holder == null || holder.DamageCounters < 3) return;
                if (!startOfTurn && !src.Engine.AskYesNo(src.Player, "Sitrus Berry: remove 3 damage counters from " + holder.Name + " and discard Sitrus Berry?", src.Source)) return;
                src.Log("Sitrus Berry removes 3 damage counters from " + holder.Name + ".");
                src.Heal(holder, 30);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Solid Rage: with more Prize cards left than the opponent, its Pokémon's attacks do 20 more to the Active Pokémon (before W/R).</summary>
        private sealed class SolidRage : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return EvolvedNotEx(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !EvolvedNotEx(ctx.Pokemon); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = Holder(src);
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Me.Prizes.Count > src.Opp.Prizes.Count ? damage + 20 : damage;
            }
        }

        /// <summary>Mary's Request (Supporter): draw a card; if you have no Stage 2 Evolved Pokémon in play, draw 2 more.</summary>
        private sealed class MarysRequest : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(1);
                if (!ctx.Me.InPlay().Any(m => m.Def.Stage == Stage.Stage2)) ctx.Draw(2);
            }
        }
    }
}
