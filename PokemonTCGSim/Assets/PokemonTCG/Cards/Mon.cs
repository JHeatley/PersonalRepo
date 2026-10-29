using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Base for Pokémon scripts. Override Attack to give attacks effects (return false to just do the printed
    /// damage), and set Activated for a "once during your turn" Ability you choose to use.
    /// </summary>
    public abstract class Mon : CardScript
    {
        /// <summary>Name of the "once during your turn" Ability the player activates, or null.</summary>
        protected virtual string Activated { get { return null; } }
        /// <summary>"You can't use more than 1 X Ability each turn": the limit is per player, not per Pokémon.</summary>
        protected virtual bool OnePerPlayer { get { return false; } }
        protected virtual bool AbilityReady(EffectContext ctx) { return true; }
        protected virtual void Ability(EffectContext ctx) { }
        /// <summary>Returns true when the attack was handled; otherwise its printed damage is done to the Defending Pokémon.</summary>
        protected virtual bool Attack(EffectContext ctx, AttackDef atk) { return false; }

        /// <summary>Runs this script's attack handler for ComboMon. Returns true when it handled the attack.</summary>
        public bool HandleAttack(EffectContext ctx, AttackDef atk) { return Attack(ctx, atk); }

        public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
        {
            var name = Activated;
            // The card the Ability is printed on (the card under a LV.X for Poké-Powers it keeps).
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            if (name == null || abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != name) return false;
            if (!ctx.IsMyTurn || ctx.AbilityUsedThisTurn(name)) return false;
            if (OnePerPlayer && ctx.NamedLimitUsedThisTurn(name)) return false;
            return AbilityReady(ctx);
        }

        public override void UseAbility(EffectContext ctx, int abilityIndex)
        {
            ctx.MarkAbilityUsed(Activated);
            if (OnePerPlayer) ctx.MarkNamedLimit(Activated);
            Ability(ctx);
        }

        public override void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            var atk = ctx.Attack(attackIndex);
            if (!Attack(ctx, atk)) ctx.DamageDefending(atk.Damage);
        }

        // ------------------------------------------------------------------ Shared helpers

        /// <summary>For "when you play this Pokémon from your hand to evolve" Abilities: asks, marks it used and announces it.</summary>
        protected static bool TriggerOnce(EffectContext ctx, string ability, bool ready, string question)
        {
            if (!ctx.IsMyTurn || !ready || ctx.AbilityUsedThisTurn(ability)) return false;
            if (!ctx.AskYesNo(question)) return false;
            ctx.MarkAbilityUsed(ability);
            Announce(ctx, ability);
            return true;
        }

        protected static void Announce(EffectContext ctx, string ability)
        {
            ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.OwnedName(ctx.Pokemon) + " used its Ability " + ability + ".", ctx.Card.Id, ctx.Pokemon.Uid);
        }

        protected static int HideNSneakInDiscard(EffectContext ctx)
        {
            return ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.Abilities.Any(a => a.Name == "Hide 'n' Sneak"));
        }

        protected static bool ExtraEnergy(EffectContext ctx, AttackDef atk, int extra)
        {
            return ctx.Engine.HasExtraEnergy(ctx.Pokemon, atk, extra);
        }

        /// <summary>Plain damage after a coin-flip or condition check: "this attack does nothing".</summary>
        protected static bool Nothing(EffectContext ctx)
        {
            ctx.Log(ctx.Pokemon.Name + "'s attack did nothing.");
            return true;
        }

        protected static int TeamRocketsInPlay(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsTeamRockets).Count; }

        /// <summary>Choose 1 of a Pokémon's attacks (for Torment / copying). Returns the index, or -1.</summary>
        protected static int ChooseAttackOf(EffectContext ctx, CardDef def, string prompt)
        {
            if (def.Attacks.Count == 0) return -1;
            var opts = def.Attacks.Select((a, i) => new DecisionOption
            {
                Label = def.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""),
                Card = def,
                Index = i
            }).ToList();
            int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, prompt, opts, ctx.Source);
            return opts[pick].Index;
        }

        /// <summary>
        /// Choose 1 attack of another Pokémon in play, for attacks that copy it. Only attacks printed on that Pokémon count,
        /// not ones it gets from Abilities, Tools or Stadiums; a LV.X also has the attacks of the Pokémon under it, since it's
        /// an extension of that Pokémon (Compendium meta-ruling), and so does a BREAK Pokémon (Compendium: Mew-EX's Versatile vs
        /// BREAK Pokémon, 2015-12-03; v92). Returns false if it has no attacks.
        /// </summary>
        protected static bool ChooseCopiedAttack(EffectContext ctx, PokemonInPlay mon, string prompt, out CardDef source, out int index)
        {
            source = null;
            index = -1;
            if (mon == null) return false;
            var defs = new List<CardDef> { mon.Def };
            if ((mon.Def.IsLevelUp || mon.Def.IsBreak) && mon.Stack.Count >= 2) defs.Add(mon.Stack[mon.Stack.Count - 2].Def);
            var opts = new List<DecisionOption>();
            foreach (var d in defs)
            {
                for (int i = 0; i < d.Attacks.Count; i++)
                {
                    var a = d.Attacks[i];
                    // Like a VSTAR attack (Compendium: Genome Hacking), a GX attack can't be chosen once you've used your GX attack.
                    if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                    if (a.VstarPower && ctx.Me.VstarUsed) continue; // Compendium: Genome Hacking and VSTAR attacks (v50)
                    // A copied attack is used by its name, so "this Pokémon can't use X during your next turn" on the copier
                    // covers copying X again (Compendium: Mew-EX's Versatile and X Blast, 2015-02-05).
                    if (ctx.Pokemon != null && ctx.Engine.AttackNamedPrevented(ctx.Pokemon, a.Name)) continue;
                    opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                }
            }
            if (opts.Count == 0) return false;
            int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, prompt, opts, ctx.Source);
            source = opts[pick].Card;
            index = opts[pick].Index;
            return true;
        }

        /// <summary>How many copied attacks are resolving inside each other on this thread (UseCopiedAttack).</summary>
        [ThreadStatic] private static int copyDepth;

        /// <summary>Uses another card's attack as this attack (Gemstone Mimicry, Seek Inspiration).</summary>
        protected static void UseCopiedAttack(EffectContext ctx, CardDef source, int index)
        {
            var copied = source.Attacks[index];
            // v169: an attack that copies an attack that copies... (Ditto's Copy against another Ditto's Copy, Metronome against
            // Metronome) would never reach a real attack: past a few copies it does nothing instead of looping forever.
            if (copyDepth >= 3)
            {
                ctx.Log(ctx.Pokemon.Name + "'s " + copied.Name + " would only copy another copying attack, so it does nothing.");
                return;
            }
            // Copying a GX attack is using your GX attack for the game; you can't once it's used.
            if (GameEngine.IsGxAttack(copied))
            {
                if (ctx.Me.GxUsed)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " already used a GX attack this game, so " + copied.Name + " can't be used.");
                    return;
                }
                ctx.Me.GxUsed = true;
            }
            // Copying a VSTAR Power attack uses your VSTAR Power (Compendium: Genome Hacking, 2023-12-14; v50).
            if (copied.VstarPower)
            {
                if (ctx.Me.VstarUsed)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " already used a VSTAR Power this game, so " + copied.Name + " can't be used.");
                    return;
                }
                ctx.Me.VstarUsed = true;
            }
            ctx.Log(ctx.Pokemon.Name + " uses " + source.Name + "'s " + source.Attacks[index].Name + ".");
            var copy = new EffectContext(ctx.Engine, ctx.Player, ctx.Card, ctx.Pokemon, source);
            copyDepth++;
            try { ctx.Engine.Scripts.For(source).ResolveAttack(copy, index); }
            finally { copyDepth--; }
        }
    }

    /// <summary>
    /// A Pokémon that needs no script of its own: its attacks only do their printed damage, and any text it has is read by the
    /// engine or by another card's script. Registered by key so another card with the same name can't lend it its script.
    /// </summary>
    public sealed class PlainMon : Mon
    {
    }

    /// <summary>A card whose attacks are each covered by a ready-made single-attack script (no Abilities or passive effects).</summary>
    public sealed class ComboMon : Mon
    {
        private readonly Mon[] parts;

        public ComboMon(params Mon[] parts) { this.parts = parts; }

        protected override bool Attack(EffectContext ctx, AttackDef atk)
        {
            foreach (var part in parts)
                if (part.HandleAttack(ctx, atk)) return true;
            return false;
        }
    }

    /// <summary>Hide 'n' Sneak: prevent all effects of the opponent's Pokémon's attacks and Abilities on this Pokémon.</summary>
    public class HideNSneakMon : Mon
    {
        public override bool ShieldsFromOpponentsAbilities { get { return true; } }

        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return target == src.Pokemon && (kind == EffectSourceKind.Attack || kind == EffectSourceKind.Ability);
        }
    }

    /// <summary>Festival Lead: may attack twice while Festival Grounds is in play.</summary>
    public class FestivalLeadMon : Mon
    {
        public override bool AttacksTwice(EffectContext src)
        {
            return src.State.Stadium != null && src.State.Stadium.Def.Name == "Festival Grounds";
        }
    }

    /// <summary>Cursed Blast: put N damage counters on 1 of the opponent's Pokémon, then this Pokémon is Knocked Out.</summary>
    public abstract class CursedBlastMon : Mon
    {
        protected abstract int Counters { get; }
        protected override string Activated { get { return "Cursed Blast"; } }
        protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }
        public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Cursed Blast"; }

        protected override void Ability(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Cursed Blast: put " + Counters + " damage counters on which of your opponent's Pokémon?");
            ctx.PlaceCounters(target, Counters);
            ctx.Engine.KnockOut(ctx.Pokemon, "Cursed Blast");
        }
    }
}
