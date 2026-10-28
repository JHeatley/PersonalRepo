using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// One card in play whose passive hooks are active right now: a Pokémon's Ability (Kind = Ability),
    /// a Pokémon Tool (Kind = Trainer), a Special Energy (Kind = Energy) or the Stadium (Kind = Stadium).
    /// Ctx.Player is the card's owner, Ctx.Card the card itself, Ctx.Pokemon the Pokémon it's on.
    /// </summary>
    public struct PassiveSource
    {
        public EffectSourceKind Kind;
        public CardScript Script;
        public EffectContext Ctx;
    }

    public sealed partial class GameEngine
    {
        // ------------------------------------------------------------------ Sources

        /// <summary>Every card whose passive effects currently apply. Blocked Abilities and disabled Tools are left out.</summary>
        public List<PassiveSource> Sources()
        {
            var list = new List<PassiveSource>();
            bool toolsOff = ToolsDisabled();
            for (int i = 0; i < 2; i++)
            {
                foreach (var mon in State.Players[i].InPlay())
                {
                    if (mon.Def.Abilities.Count > 0 && HasUnblockedAbility(mon, 0))
                        list.Add(new PassiveSource { Kind = mon.Def.AbilitySourceKind, Script = Scripts.For(mon.Def), Ctx = new EffectContext(this, mon.Owner, mon.Top, mon) });
                    var lower = InheritedPowersCard(mon);
                    if (lower != null && InheritedWorks(mon, lower, 0))
                        list.Add(new PassiveSource { Kind = InheritedKind(mon, lower), Script = Scripts.For(lower.Def), Ctx = new EffectContext(this, mon.Owner, lower, mon) });
                    if (!toolsOff || StadiumShielded(mon))
                        foreach (var tool in mon.Tools)
                            if (ToolWorks(mon, tool)) list.Add(new PassiveSource { Kind = EffectSourceKind.Trainer, Script = Scripts.For(tool.Def), Ctx = new EffectContext(this, mon.Owner, tool, mon) });
                    bool specialOn = !mon.Energy.Any(x => x.Def.IsSpecialEnergy) || !SpecialEnergyOff(mon);
                    if (specialOn)
                        foreach (var e in mon.Energy)
                            if (e.Def.IsSpecialEnergy)
                                list.Add(new PassiveSource { Kind = EffectSourceKind.Energy, Script = Scripts.For(e.Def), Ctx = new EffectContext(this, mon.Owner, e, mon) });
                }
            }
            if (State.Stadium != null && !StadiumSwitchedOff())
                list.Add(new PassiveSource { Kind = EffectSourceKind.Stadium, Script = Scripts.For(State.Stadium.Def), Ctx = StadiumContext() });
            return list;
        }

        /// <summary>Sources on one Pokémon only (its Ability, Tool and Special Energy).</summary>
        public List<PassiveSource> SourcesOn(PokemonInPlay mon)
        {
            var list = new List<PassiveSource>();
            if (mon.Def.Abilities.Count > 0 && HasUnblockedAbility(mon, 0))
                list.Add(new PassiveSource { Kind = mon.Def.AbilitySourceKind, Script = Scripts.For(mon.Def), Ctx = new EffectContext(this, mon.Owner, mon.Top, mon) });
            var lower = InheritedPowersCard(mon);
            if (lower != null && InheritedWorks(mon, lower, 0))
                list.Add(new PassiveSource { Kind = InheritedKind(mon, lower), Script = Scripts.For(lower.Def), Ctx = new EffectContext(this, mon.Owner, lower, mon) });
            if (!ToolsDisabled() || StadiumShielded(mon))
                foreach (var tool in mon.Tools)
                    if (ToolWorks(mon, tool)) list.Add(new PassiveSource { Kind = EffectSourceKind.Trainer, Script = Scripts.For(tool.Def), Ctx = new EffectContext(this, mon.Owner, tool, mon) });
            if (!mon.Energy.Any(x => x.Def.IsSpecialEnergy) || !SpecialEnergyOff(mon))
                foreach (var e in mon.Energy)
                    if (e.Def.IsSpecialEnergy)
                        list.Add(new PassiveSource { Kind = EffectSourceKind.Energy, Script = Scripts.For(e.Def), Ctx = new EffectContext(this, mon.Owner, e, mon) });
            return list;
        }

        private bool checkingSpecialEnergyShield;
        private bool checkingSpecialEnergyAbility;

        /// <summary>
        /// Special Energy attached to this Pokémon provide [C] and have no other effect right now: Temple of Sinnoh (v53) or
        /// Dusknoir's Spectral Breach (v54). A Pokémon that Lunatone's New Moon shields from Stadium effects keeps its Special
        /// Energy working under Temple of Sinnoh; one shielded from the opponent's Abilities (Lustrous Body, Guardian of Love,
        /// Hide 'n' Sneak) keeps it working under the opponent's Spectral Breach (your ruling, v54). Both checks are guarded
        /// against re-entry (an Ability blocker that counts Energy would otherwise ask again).
        /// </summary>
        public bool SpecialEnergyOff(PokemonInPlay holder)
        {
            if (State.Stadium != null && !StadiumSwitchedOff() && Scripts.For(State.Stadium.Def).DisablesSpecialEnergy(StadiumContext()))
            {
                if (holder == null || checkingSpecialEnergyShield) return true;
                bool shielded;
                checkingSpecialEnergyShield = true;
                try { shielded = StadiumShielded(holder); }
                finally { checkingSpecialEnergyShield = false; }
                if (!shielded) return true;
            }
            return SpecialEnergyOffByAbility(holder);
        }

        /// <summary>Spectral Breach: a Pokémon in play whose working Ability turns Special Energy off reaches this holder.</summary>
        private bool SpecialEnergyOffByAbility(PokemonInPlay holder)
        {
            if (checkingSpecialEnergyAbility) return false;
            List<PokemonInPlay> breachers = null;
            for (int i = 0; i < 2; i++)
                foreach (var mon in State.Players[i].InPlay())
                {
                    if (mon.Def.Abilities.Count == 0) continue;
                    if (!Scripts.For(mon.Def).DisablesSpecialEnergy(new EffectContext(this, mon.Owner, mon.Top, mon))) continue;
                    if (breachers == null) breachers = new List<PokemonInPlay>();
                    breachers.Add(mon);
                }
            if (breachers == null) return false;
            checkingSpecialEnergyAbility = true;
            try
            {
                foreach (var breacher in breachers)
                {
                    if (!HasUnblockedAbility(breacher, 0)) continue;
                    if (holder != null && holder.Owner != breacher.Owner && ShieldedFromOpponentsAbilities(holder)) continue;
                    return true;
                }
                return false;
            }
            finally { checkingSpecialEnergyAbility = false; }
        }

        /// <summary>
        /// The Pokémon is protected from the effects of the opponent's Abilities: by one of its owner's Pokémon (Lustrous Body,
        /// Guardian of Love, Hide 'n' Sneak) or by a card attached to it (a Tool, or Fusion Strike Energy: Compendium, it
        /// stops Spectral Breach on its Pokémon).
        /// </summary>
        public bool ShieldedFromOpponentsAbilities(PokemonInPlay mon)
        {
            if (mon == null) return false;
            foreach (var protector in State.Players[mon.Owner].InPlay())
            {
                if (protector.Def.Abilities.Count == 0) continue;
                var script = Scripts.For(protector.Def);
                if (!script.ShieldsFromOpponentsAbilities || !HasUnblockedAbility(protector, 0)) continue;
                if (script.PreventsEffects(new EffectContext(this, protector.Owner, protector.Top, protector), mon, EffectSourceKind.Ability, false)) return true;
            }
            return AttachedCardShieldsFromAbilities(mon);
        }

        /// <summary>
        /// A card attached to the Pokémon (a Tool, or Special Energy such as Fusion Strike Energy) prevents the effects of the
        /// opponent's Abilities on it. The attached cards are asked directly, so this never asks about Abilities again.
        /// </summary>
        public bool AttachedCardShieldsFromAbilities(PokemonInPlay mon)
        {
            if (mon == null) return false;
            // θ Stop (v98) works the same way, and keeps the Pokémon's own Abilities on against the opponent's Ability blockers
            // (Bide Barricade: Compendium, 2015-09-03).
            if (HasAncientTrait(mon, ThetaStop)) return true;
            if (!ToolsDisabled())
                foreach (var tool in mon.Tools)
                    if (ToolWorks(mon, tool) && Scripts.For(tool.Def).PreventsEffects(new EffectContext(this, mon.Owner, tool, mon), mon, EffectSourceKind.Ability, false)) return true;
            foreach (var energy in mon.Energy)
                if (energy.Def.IsSpecialEnergy && Scripts.For(energy.Def).PreventsEffects(new EffectContext(this, mon.Owner, energy, mon), mon, EffectSourceKind.Ability, false)) return true;
            return false;
        }

        private EffectContext StadiumContext()
        {
            return new EffectContext(this, State.StadiumOwner, State.Stadium, null);
        }

        /// <summary>
        /// Sources whose effects reach this Pokémon: all of them, minus the Stadium while Lunatone's New Moon shields it.
        /// Used by every query about one Pokémon (HP, Retreat Cost, attack cost, damage taken...).
        /// </summary>
        public List<PassiveSource> SourcesFor(PokemonInPlay mon)
        {
            var list = Sources();
            if (StadiumShielded(mon)) list = WithoutStadium(list);
            // θ Stop (v98): the opponent's Abilities have no effect on this Pokémon (Psychic Zone doesn't take Golurk's Weakness
            // away: Compendium, 2020-03-12).
            if (HasAncientTrait(mon, ThetaStop)) list = list.Where(entry => entry.Kind != EffectSourceKind.Ability || entry.Ctx.Player == mon.Owner).ToList();
            return list;
        }

        /// <summary>How many Supporter cards the player may play this turn: Lt. Surge's Strategy's 3, or Dual Brains' 2 (v111).</summary>
        public int SupporterLimitFor(int player)
        {
            int limit = State.Players[player].SupporterLimit;
            foreach (var src in Sources()) limit = src.Script.ModifySupporterLimit(src.Ctx, player, limit);
            return limit;
        }

        /// <summary>A Team Plasma Pokémon: its card has the Team Plasma label, or a Team Plasma Badge makes it one (v110).</summary>
        public bool IsTeamPlasma(PokemonInPlay mon)
        {
            if (mon == null) return false;
            if (mon.Def.IsTeamPlasma) return true;
            foreach (var src in SourcesOn(mon))
                if (src.Script.MakesHolderTeamPlasma(src.Ctx)) return true;
            return false;
        }

        public const string ThetaStop = "θ Stop";
        public const string ThetaDouble = "θ Double";
        public const string ThetaMax = "θ Max";
        public const string DeltaPlus = "Δ Plus";
        public const string DeltaEvolution = "Δ Evolution";
        public const string DeltaWild = "Δ Wild";
        public const string OmegaBarrier = "Ω Barrier";
        public const string OmegaBarrage = "Ω Barrage";
        public const string AlphaGrowth = "α Growth";
        public const string AlphaRecovery = "α Recovery";

        /// <summary>The Pokémon (its top card) has this Ancient Trait (v98). Ancient Traits aren't Abilities and only work in play.</summary>
        public static bool HasAncientTrait(PokemonInPlay mon, string trait)
        {
            return mon != null && mon.Def.AncientTrait == trait;
        }

        private static List<PassiveSource> WithoutStadium(List<PassiveSource> list)
        {
            return list.Where(entry => entry.Kind != EffectSourceKind.Stadium).ToList();
        }

        /// <summary>
        /// Lunatone's New Moon: "If you have Solrock in play, prevent all effects of any Stadium done to your Pokémon in play."
        /// Every Stadium effect on the Pokémon, good or bad, is off (your ruling); Bench size isn't an effect on a Pokémon
        /// (Compendium, Collapsed Stadium, 2023-09-07).
        /// </summary>
        public bool StadiumShielded(PokemonInPlay mon)
        {
            return StadiumShieldedAt(mon, 0);
        }

        private bool StadiumShieldedAt(PokemonInPlay mon, int depth)
        {
            if (mon == null || State.Stadium == null || depth >= 2) return false;
            foreach (var other in State.Players[mon.Owner].InPlay())
            {
                if (other.Def.Abilities.Count == 0) continue;
                var script = Scripts.For(other.Def);
                if (!script.ShieldsOwnPokemonFromStadium(new EffectContext(this, other.Owner, other.Top, other))) continue;
                if (HasUnblockedAbility(other, depth + 1)) return true;
            }
            return false;
        }

        /// <summary>Timed effects that are in force this turn.</summary>
        public IEnumerable<ContinuousEffect> ActiveEffects()
        {
            foreach (var e in State.Effects)
                if (e.ActiveFromTurn <= State.TurnNumber) yield return e;
        }

        /// <summary>Dragon's Wish (v90): the player may attach any number of Energy cards from their hand this turn.</summary>
        public bool AnyNumberOfEnergyAttachments(int player)
        {
            foreach (var e in ActiveEffects())
                if (e.AllowsAnyNumberOfEnergyAttachments(this, player)) return true;
            return false;
        }

        // ------------------------------------------------------------------ Abilities

        /// <summary>True when this Ability of this Pokémon currently doesn't exist (Flutter Mane, Damp, Watchtower...).</summary>
        public bool AbilityBlocked(PokemonInPlay mon, string abilityName)
        {
            return AbilityBlockedAt(mon, abilityName, 0);
        }

        /// <summary>True when the Pokémon has no usable Abilities at all.</summary>
        public bool AbilitiesBlocked(PokemonInPlay mon)
        {
            return !HasUnblockedAbility(mon, 0);
        }

        /// <summary>
        /// "Pokémon that has an Ability" (for card effects): a real Ability, printed and not blocked. Pokémon Powers,
        /// Poké-Powers and Poké-Bodies don't count.
        /// </summary>
        public bool HasAbility(PokemonInPlay mon)
        {
            if (mon == null) return false;
            foreach (var a in mon.Def.Abilities)
                if (a.IsAbility && !AbilityBlocked(mon, a.Name)) return true;
            // A BREAK Pokémon has the Abilities of the Pokémon under it (v92).
            if (mon.Def.IsBreak)
            {
                var lower = InheritedPowersCard(mon);
                if (lower != null)
                    foreach (var a in lower.Def.Abilities)
                        if (a.IsAbility && !AbilityBlocked(mon, a.Name)) return true;
            }
            // An Ability an effect gives it (Aerodactyl VSTAR's Ancient Star counts as a Pokémon with an Ability, Compendium).
            foreach (var fx in ActiveEffects())
            {
                var gained = fx.GainedAbility(this, mon);
                if (gained != null && !AbilityBlocked(mon, gained)) return true;
            }
            return false;
        }

        /// <summary>
        /// AbilityBlocked at a given check depth, for effects that block Abilities only while an Ability of their own works
        /// (Ancient Star checks its gained Ability at depth + 1).
        /// </summary>
        public bool AbilityBlockedAtDepth(PokemonInPlay mon, string abilityName, int depth)
        {
            return AbilityBlockedAt(mon, abilityName, depth);
        }

        /// <summary>The Pokémon has an Ability, Power or Body that works right now (for the engine's own triggers).</summary>
        public bool HasWorkingAbilityOrPower(PokemonInPlay mon)
        {
            return mon != null && mon.Def.Abilities.Count > 0 && HasUnblockedAbility(mon, 0);
        }

        /// <summary>The Pokémon has an Imprison marker (Gardevoir ex δ, v56): it can't use Poké-Powers or Poké-Bodies.</summary>
        public static bool Imprisoned(PokemonInPlay mon)
        {
            return mon != null && mon.Markers.ContainsKey(PokemonInPlay.ImprisonMarker);
        }

        /// <summary>
        /// LV.X: "can use any attack, Poké-Power, or Poké-Body from its previous Level". The card under a LV.X when it has a
        /// Poké-Power or Poké-Body (not Pokémon Powers or Abilities), otherwise null. Null too while an Imprison marker stops them.
        /// </summary>
        public CardInstance InheritedPowersCard(PokemonInPlay mon)
        {
            // Fossil Ditto's Transform (v116, your ruling): a copy has the copied Pokémon's Pokémon Powers and Abilities too.
            var copied = TransformTarget(mon);
            if (copied != null) return copied.Def.Abilities.Count > 0 ? copied.Top : null;
            // Brock's Ninetales's Shapeshift (v126, your ruling): it has the Pokémon Powers of the Pokémon it's treated as.
            var shape = ShapeshiftCardInstance(mon);
            if (shape != null) return shape.Def.Abilities.Count > 0 ? shape : null;
            if (mon == null || mon.Stack.Count < 2) return null;
            var below = mon.Stack[mon.Stack.Count - 2];
            // BREAK Evolution (v92): "keeps the attacks, Abilities, Weakness, Resistance, and Retreat Cost of its previous
            // Evolution". The card under it when that card has Abilities; things that block Abilities block these too.
            if (mon.Def.IsBreak) return below.Def.Abilities.Count > 0 ? below : null;
            if (!mon.Def.IsLevelUp || Imprisoned(mon)) return null;
            foreach (var a in below.Def.Abilities) if (a.IsPokePowerOrBody) return below;
            return null;
        }

        /// <summary>
        /// A BREAK Pokémon's inherited Abilities work right now (at least 1 of them isn't blocked). LV.X Poké-Powers and
        /// Poké-Bodies always count here: Ability blockers don't stop them.
        /// </summary>
        private bool InheritedWorks(PokemonInPlay mon, CardInstance lower, int depth)
        {
            if (!mon.Def.IsBreak && TransformTarget(mon) == null && ShapeshiftCard(mon) == null) return true;
            foreach (var a in lower.Def.Abilities)
                if (!AbilityBlockedAt(mon, a.Name, depth)) return true;
            return false;
        }

        /// <summary>The source kind of the card under a LV.X (its Poké-Powers) or BREAK Pokémon (its Abilities).</summary>
        private EffectSourceKind InheritedKind(PokemonInPlay mon, CardInstance lower)
        {
            return mon.Def.IsBreak || TransformTarget(mon) != null || ShapeshiftCard(mon) != null ? lower.Def.AbilitySourceKind : EffectSourceKind.PokemonPower;
        }

        /// <summary>
        /// The working Spirit Link Tool on this Pokémon for this Mega Evolution ("Charizard Spirit Link" for M Charizard-EX), or
        /// null (Evolutions, v92). Only for XY-era Megas: a Mega Evolution Pokémon ex isn't "M ...-EX" (your rules update, v92).
        /// </summary>
        public CardInstance SpiritLinkFor(PokemonInPlay mon, CardDef mega)
        {
            if (mon == null || mega == null || !mega.IsXYMegaEvolution || mon.Tools.Count == 0) return null;
            string name = mega.Name.StartsWith("M ") ? mega.Name.Substring(2) : mega.Name;
            // Primal Reversion (v101): "Groudon Spirit Link" for Primal Groudon-EX.
            if (name.StartsWith("Primal ")) name = name.Substring(7);
            if (name.EndsWith("-EX")) name = name.Substring(0, name.Length - 3);
            name += " Spirit Link";
            if (ToolsDisabled() && !StadiumShielded(mon)) return null;
            foreach (var tool in mon.Tools)
                if (tool.Def.Name == name && ToolWorks(mon, tool)) return tool;
            return null;
        }

        /// <summary>The ability with this name on the Pokémon (or inherited by a LV.X), or null.</summary>
        private AbilityDef FindAbility(PokemonInPlay mon, string abilityName)
        {
            foreach (var a in mon.Def.Abilities) if (a.Name == abilityName) return a;
            var below = InheritedPowersCard(mon);
            if (below != null)
                foreach (var a in below.Def.Abilities) if (a.Name == abilityName) return a;
            return null;
        }

        public bool HasAbilityNamed(PokemonInPlay mon, string abilityName)
        {
            if (mon == null) return false;
            foreach (var a in mon.Def.Abilities)
                if (a.Name == abilityName && !AbilityBlocked(mon, abilityName)) return true;
            return false;
        }

        private bool HasUnblockedAbility(PokemonInPlay mon, int depth)
        {
            foreach (var a in mon.Def.Abilities)
                if (!AbilityBlockedAt(mon, a.Name, depth)) return true;
            return false;
        }

        /// <summary>
        /// A blocker only blocks while its own Ability works, so blockers are checked recursively
        /// (Flutter Mane can switch off an Active Psyduck's Damp). Depth 2 is enough for every card.
        /// </summary>
        private bool AbilityBlockedAt(PokemonInPlay mon, string abilityName, int depth)
        {
            // Pokémon Powers, Poké-Powers and Poké-Bodies aren't Abilities: nothing that blocks Abilities stops them.
            var printed = FindAbility(mon, abilityName);
            // An Imprison marker (Gardevoir ex δ) stops Poké-Powers and Poké-Bodies, not Abilities or Pokémon Powers (v56).
            if (printed != null && printed.IsPokePowerOrBody && Imprisoned(mon)) return true;
            // Muk's Toxic Gas (v116): every other Pokémon Power is ignored (your ruling: only Pokémon Powers).
            if (printed != null && printed.Kind == AbilityDef.PokemonPower && abilityName != ToxicGasPower && ToxicGasActive()) return true;
            // Goop Gas Attack and Stare (v121): timed effects that stop Pokémon Powers.
            if (printed != null && printed.Kind == AbilityDef.PokemonPower && PokemonPowerStoppedByEffect(mon, abilityName)) return true;
            // v146: timed effects that stop Poké-Powers (Dark Impact).
            if (printed != null && printed.Kind == AbilityDef.PokePower && PokePowerStoppedByEffect(mon, abilityName)) return true;
            // v147: timed effects that stop Poké-Bodies (Ancient Wind).
            if (printed != null && printed.Kind == AbilityDef.PokeBody && PokeBodyStoppedByEffect(mon, abilityName)) return true;
            if (printed != null && !printed.IsAbility) return false;
            if (State.Stadium != null && !StadiumSwitchedOff() && !StadiumShieldedAt(mon, depth))
            {
                var s = Scripts.For(State.Stadium.Def);
                var ctx = StadiumContext();
                if (s.BlocksAbilitiesOf(ctx, mon) || s.BlocksAbility(ctx, mon, abilityName)) return true;
            }
            if (depth >= 2) return false;
            for (int i = 0; i < 2; i++)
            {
                foreach (var other in State.Players[i].InPlay())
                {
                    // Its own Abilities, or (a BREAK Pokémon, v92) the ones of the Pokémon under it.
                    var blockerCard = other.Top;
                    if (other.Def.Abilities.Count == 0)
                    {
                        blockerCard = other.Def.IsBreak ? InheritedPowersCard(other) : null;
                        if (blockerCard == null) continue;
                    }
                    var script = Scripts.For(blockerCard.Def);
                    if (!script.IsAbilityBlocker) continue;
                    if (blockerCard == other.Top ? !HasUnblockedAbility(other, depth + 1) : !InheritedWorks(other, blockerCard, depth + 1)) continue;
                    if (!script.BlocksAbility(new EffectContext(this, other.Owner, blockerCard, other), mon, abilityName)) continue;
                    if (EarlierLockKeeps(mon, other, blockerCard, depth)) continue;
                    // A Tool or Special Energy that prevents the effects of the opponent's Abilities on the Pokémon keeps its
                    // Abilities (Fusion Strike Energy vs Fettered in Misfortune: Compendium, 2023-07-13; every other
                    // "have no Abilities" Ability too: your ruling, v55).
                    if (other.Owner != mon.Owner && AttachedCardShieldsFromAbilities(mon)) continue;
                    if (other.Owner != mon.Owner && ShieldedByOwnPokemon(mon, depth)) continue;
                    return true;
                }
            }
            // Timed effects that take Abilities away (Aerodactyl VSTAR's Ancient Star, v51).
            foreach (var fx in State.Effects)
                if (fx.ActiveFromTurn <= State.TurnNumber && fx.BlocksAbility(this, mon, abilityName, depth)) return true;
            return false;
        }

        /// <summary>
        /// Two Ability locks that would each switch the other off (Lazy vs Garbotoxin or Bide Barricade, Mischievous Lock vs
        /// Emperor's Eyes): the one that was on first stays on and the other can't turn on; turned on at the same time, the
        /// turn player's goes first (Compendium, 2018-08-02 and 2023). "On" is when the Pokémon came into play, evolved or
        /// devolved, moved into the spot its lock needs, or got the Tool it needs. A lock that something else is switching off
        /// doesn't keep anything.
        /// </summary>
        private bool EarlierLockKeeps(PokemonInPlay mon, PokemonInPlay blocker, CardInstance blockerCard, int depth)
        {
            if (mon == blocker || mon.Def.Abilities.Count == 0) return false;
            var mine = Scripts.For(mon.Def);
            if (!mine.IsAbilityBlocker) return false;
            var ctx = new EffectContext(this, mon.Owner, mon.Top, mon);
            bool mutual = false;
            foreach (var a in blockerCard.Def.Abilities)
                if (mine.BlocksAbility(ctx, blocker, a.Name)) { mutual = true; break; }
            if (!mutual) return false;
            int myStart = LockStart(mon, mine), theirStart = LockStart(blocker, Scripts.For(blockerCard.Def));
            bool first = myStart != theirStart ? myStart < theirStart : mon.Owner == State.CurrentPlayer;
            return first && HasUnblockedAbility(mon, depth + 1);
        }

        /// <summary>
        /// One of the Pokémon's own side keeps the opponent's Abilities off it (Lustrous Body, Guardian of Love, Hide 'n' Sneak), so
        /// the opponent's Ability locks don't reach it: Lustrous Body wins against Neutralizing Gas whichever came first
        /// (Compendium, 2021-04-08). The protector's Ability must be working, which is asked one level deeper.
        /// </summary>
        private bool ShieldedByOwnPokemon(PokemonInPlay mon, int depth)
        {
            foreach (var protector in State.Players[mon.Owner].InPlay())
            {
                if (protector.Def.Abilities.Count == 0) continue;
                var script = Scripts.For(protector.Def);
                if (!script.ShieldsFromOpponentsAbilities) continue;
                if (!script.PreventsEffects(new EffectContext(this, protector.Owner, protector.Top, protector), mon, EffectSourceKind.Ability, false)) continue;
                if (HasUnblockedAbility(protector, depth + 1)) return true;
            }
            return false;
        }

        private static int LockStart(PokemonInPlay mon, CardScript script)
        {
            int start = mon.PlaySeq;
            if (script.LockDependsOnSpot) start = Math.Max(start, mon.SpotSeq);
            if (script.LockDependsOnTool) start = Math.Max(start, mon.ToolSeq);
            return start;
        }

        // ------------------------------------------------------------------ HP, Weakness, Retreat

        public int MaxHp(PokemonInPlay mon)
        {
            // Fossil Ditto's Transform (v116): the copied Pokémon's HP.
            var copied = TransformTarget(mon);
            int hp = copied != null ? copied.Def.Hp : mon.Def.Hp;
            // Brock's Ninetales's Shapeshift (v126): the HP of the Pokémon it's treated as.
            var shape = ShapeshiftCard(mon);
            if (shape != null) hp = shape.Hp;
            foreach (var src in SourcesFor(mon)) hp = src.Script.ModifyMaxHp(src.Ctx, mon, hp);
            // Koga's Ditto's Giant Growth (v126): a timed effect.
            foreach (var fx in ActiveEffects()) hp = fx.ModifyMaxHp(this, mon, hp);
            return hp;
        }

        public int RemainingHp(PokemonInPlay mon)
        {
            return MaxHp(mon) - mon.DamageCounters * 10;
        }

        public bool IsKnockedOut(PokemonInPlay mon)
        {
            return mon.ForcedKnockOut || mon.DamageCounters * 10 >= MaxHp(mon);
        }

        public EnergyType? WeaknessOf(PokemonInPlay mon)
        {
            EnergyType? w = StatsCardOf(mon).Weakness;
            // Porygon's Conversion 1 (v113): a Weakness changed to another type.
            int converted;
            if (w.HasValue && mon.Markers.TryGetValue(PokemonInPlay.ConversionWeaknessMarker, out converted)) w = (EnergyType)converted;
            var sources = SourcesFor(mon);
            foreach (var src in sources) w = src.Script.ModifyWeakness(src.Ctx, mon, w);
            foreach (var e in ActiveEffects()) w = e.ModifyWeakness(this, mon, w);
            if (w.HasValue)
                foreach (var src in sources)
                    if (src.Script.RemovesWeakness(src.Ctx, mon)) return null;
            return w;
        }

        /// <summary>
        /// EX-era cards with 2 Weaknesses (v158): the second one. An effect that changes or removes the Pokémon's Weakness covers
        /// both, so the second only counts while the first is still the printed one.
        /// </summary>
        public EnergyType? SecondWeaknessOf(PokemonInPlay mon)
        {
            var stats = StatsCardOf(mon);
            if (!stats.SecondWeakness.HasValue || !stats.Weakness.HasValue) return null;
            var w = WeaknessOf(mon);
            return w.HasValue && w.Value == stats.Weakness.Value ? stats.SecondWeakness : null;
        }

        /// <summary>EX-era cards with 2 Resistances (v158): the second one, while the first is still the printed one (same amount).</summary>
        public EnergyType? SecondResistanceOf(PokemonInPlay mon)
        {
            var stats = StatsCardOf(mon);
            if (!stats.SecondResistance.HasValue || !stats.Resistance.HasValue) return null;
            var r = ResistanceOf(mon);
            return r.HasValue && r.Value == stats.Resistance.Value ? stats.SecondResistance : null;
        }

        /// <summary>
        /// The card whose printed Weakness, Resistance and Retreat Cost the Pokémon has: its top card, or for a BREAK Pokémon the
        /// card under it ("it keeps the Weakness, Resistance, and Retreat Cost of its previous Evolution", v96).
        /// </summary>
        public static CardDef PrintedStatsOf(PokemonInPlay mon)
        {
            if (mon.Def.IsBreak && mon.Stack.Count >= 2)
            {
                var below = mon.Stack[mon.Stack.Count - 2].Def;
                if (below.IsPokemon) return below;
            }
            return mon.Def;
        }

        /// <summary>The Pokémon's Resistance type right now: printed, or changed by an effect (Cool Porygon's Texture Magic).</summary>
        public EnergyType? ResistanceOf(PokemonInPlay mon)
        {
            EnergyType? r = StatsCardOf(mon).Resistance;
            // Porygon's Conversion 2 (v113): its Resistance changed to another type.
            int converted;
            if (mon.Markers.TryGetValue(PokemonInPlay.ConversionResistanceMarker, out converted)) r = (EnergyType)converted;
            foreach (var e in ActiveEffects()) r = e.ModifyResistance(this, mon, r);
            return r;
        }

        /// <summary>How much the Pokémon's Resistance takes off: printed, or changed by an Ability (Klefki's Key of Secrets: -40, v80).</summary>
        public int ResistanceAmountOf(PokemonInPlay mon)
        {
            int amount = StatsCardOf(mon).ResistanceAmount;
            foreach (var src in SourcesFor(mon)) amount = src.Script.ModifyResistanceAmount(src.Ctx, mon, amount);
            return Math.Max(0, amount);
        }

        /// <summary>The Pokémon's types right now: its printed type plus any an effect adds (Iron Treads' Dual Core).</summary>
        public List<EnergyType> TypesOf(PokemonInPlay mon)
        {
            // Fossil Ditto's Transform (v116): the copied Pokémon's type(s).
            var copiedType = TransformTarget(mon);
            var typeCard = copiedType != null ? copiedType.Def : (ShapeshiftCard(mon) ?? mon.Def);
            var types = new List<EnergyType> { typeCard.PokemonType };
            if (typeCard.SecondType.HasValue && !types.Contains(typeCard.SecondType.Value)) types.Add(typeCard.SecondType.Value);
            // Smeargle's Coloring counter: "That Pokémon is now the type you chose" (until it evolves or leaves play).
            int colored;
            if (mon.Markers.TryGetValue(PokemonInPlay.ColoringMarker, out colored))
            {
                types.Clear();
                types.Add((EnergyType)colored);
            }
            // Venomoth's Shift (v114): the chosen type until Shift is used again or the Pokémon leaves play (your ruling).
            int shifted;
            if (mon.Markers.TryGetValue(PokemonInPlay.ShiftTypeMarker, out shifted))
            {
                types.Clear();
                types.Add((EnergyType)shifted);
            }
            foreach (var src in SourcesFor(mon)) src.Script.ModifyTypes(src.Ctx, mon, types);
            // Arceus's Type Switch (v107): "this Pokémon is that type" until the end of its owner's next turn.
            foreach (var fx in ActiveEffects()) fx.ModifyTypes(this, mon, types);
            return types;
        }

        /// <summary>"[type] Pokémon": true if the Pokémon is that type (a Pokémon can have 2 types).</summary>
        public bool IsType(PokemonInPlay mon, EnergyType type)
        {
            return mon != null && TypesOf(mon).Contains(type);
        }

        public int RetreatCostOf(PokemonInPlay mon)
        {
            // "If a Retreat Cost is set to zero by an effect, it cannot be increased" (Compendium meta-ruling): an effect
            // that gives the Pokémon no Retreat Cost wins over "+[C]" effects. One that only subtracts down to zero doesn't.
            // A "no Retreat Cost" effect returns 0 (or less) whatever cost it's given, so ask each one about a large cost.
            const int Probe = 50;
            var sources = SourcesFor(mon);
            foreach (var src in sources) if (src.Script.ModifyRetreatCost(src.Ctx, mon, Probe) <= 0) return 0;
            foreach (var e in ActiveEffects()) if (e.ModifyRetreatCost(this, mon, Probe) <= 0) return 0;
            int cost = StatsCardOf(mon).RetreatCost;
            foreach (var src in sources) cost = src.Script.ModifyRetreatCost(src.Ctx, mon, cost);
            foreach (var e in ActiveEffects()) cost = e.ModifyRetreatCost(this, mon, cost);
            return Math.Max(0, cost);
        }

        // ------------------------------------------------------------------ Energy

        /// <summary>The Energy units one attached card provides right now (see EnergyMask).</summary>
        public List<int> EnergyUnitsOf(PokemonInPlay holder, CardInstance energy, List<PassiveSource> sources = null)
        {
            // Temple of Sinnoh: 1 [C] and nothing else (Compendium: Double Turbo Energy gives 1 [C], no damage reduction).
            if (energy.Def.IsAttachedSpecialEnergy && SpecialEnergyOff(holder)) return new List<int> { EnergyMask.Colorless };
            var units = Scripts.For(energy.Def).ProvidedEnergy(new EffectContext(this, holder.Owner, energy, holder));
            if (units == null) units = new List<int>();
            foreach (var src in sources ?? SourcesFor(holder)) src.Script.ModifyProvidedEnergy(src.Ctx, energy, holder, units);
            // Fossil Ditto's Transform (v116): its Energy counts as Energy of every type.
            if (TransformTarget(holder) != null)
                for (int k = 0; k < units.Count; k++) if (units[k] != 0) units[k] = EnergyMask.AnyType;
            foreach (var e in ActiveEffects()) e.ModifyProvidedEnergy(this, energy, holder, units);
            return units;
        }

        /// <summary>Every Energy unit attached to the Pokémon.</summary>
        public List<int> EnergyUnits(PokemonInPlay mon)
        {
            var all = new List<int>();
            if (mon.Energy.Count == 0) return all;
            var sources = SourcesFor(mon);
            foreach (var e in mon.Energy) all.AddRange(EnergyUnitsOf(mon, e, sources));
            return all;
        }

        /// <summary>"For each Energy attached": the amount of Energy provided (Neo Upper Energy can count as 2).</summary>
        public int EnergyCount(PokemonInPlay mon)
        {
            return EnergyUnits(mon).Count;
        }

        /// <summary>"For each [type] Energy attached".</summary>
        public int EnergyCountOfType(PokemonInPlay mon, EnergyType type)
        {
            int n = 0;
            foreach (int u in EnergyUnits(mon)) if (EnergyMask.Provides(u, type)) n++;
            return n;
        }

        public bool HasEnergyOfType(PokemonInPlay mon, EnergyType type)
        {
            return EnergyCountOfType(mon, type) > 0;
        }

        /// <summary>
        /// Set while Zoroark-GX's Trickster-GX resolves a copied attack (v87): the Energy on Zoroark-GX pays for Trickster-GX itself,
        /// so "extra Energy" checks in the copied attack count beyond Trickster-GX's cost (Compendium, 2020-08-13).
        /// </summary>
        public AttackDef CopiedCostBasis;

        /// <summary>The attack's cost for this Pokémon after Tools, Stadiums and Abilities.</summary>
        public List<EnergyType> AttackCost(PokemonInPlay mon, AttackDef attack)
        {
            if (CopiedCostBasis != null && attack != CopiedCostBasis && !mon.Def.Attacks.Contains(attack) && mon.Def.Attacks.Contains(CopiedCostBasis))
                attack = CopiedCostBasis;
            var modifiers = new List<Action<List<EnergyType>>>();
            // The attack's own text ("If this Pokémon is affected by a Special Condition, ignore all Energy in this attack's cost").
            if (mon.Def.Attacks.Contains(attack))
            {
                var own = new EffectContext(this, mon.Owner, mon.Top, mon);
                modifiers.Add(list => Scripts.For(mon.Def).ModifyOwnAttackCost(own, attack, list));
            }
            foreach (var src in SourcesFor(mon))
            {
                var source = src;
                modifiers.Add(list => source.Script.ModifyAttackCost(source.Ctx, mon, attack, list));
            }
            foreach (var e in ActiveEffects())
            {
                var effect = e;
                modifiers.Add(list => effect.ModifyAttackCost(this, mon, attack, list));
            }

            // Compendium (Insight vs Headwind; Plasma Bane vs Pokémon League Headquarters): a cost an effect sets to a fixed
            // amount ("costs nothing", "costs [D]") can't be increased by other effects. A cost only reduced isn't "set".
            // A modifier that sets the cost itself is always applied, even to 3 or more Energy (Iron Jugulis's Baryon Beam: "costs
            // [C][C][C]" with a Future Booster Energy Capsule): only the ones that add to a cost are skipped.
            var sets = modifiers.Select(SetsAttackCost).ToList();
            bool costIsSet = sets.Contains(true);
            var cost = new List<EnergyType>(attack.Cost);
            for (int m = 0; m < modifiers.Count; m++)
            {
                var modify = modifiers[m];
                if (costIsSet && !sets[m] && IncreasesAttackCost(modify)) continue;
                modify(cost);
            }
            return cost;
        }

        /// <summary>A modifier "sets" the cost if it gives the same result whatever cost it starts from (and changes it).</summary>
        private static bool SetsAttackCost(Action<List<EnergyType>> modify)
        {
            var a = new List<EnergyType> { EnergyType.Colorless, EnergyType.Colorless, EnergyType.Colorless, EnergyType.Colorless, EnergyType.Colorless, EnergyType.Colorless };
            var b = new List<EnergyType> { EnergyType.Fire, EnergyType.Water, EnergyType.Grass, EnergyType.Lightning, EnergyType.Psychic };
            modify(a);
            modify(b);
            if (a.Count != b.Count || a.Count >= 5) return false;
            var sa = a.OrderBy(t => (int)t).ToList();
            var sb = b.OrderBy(t => (int)t).ToList();
            for (int i = 0; i < sa.Count; i++) if (sa[i] != sb[i]) return false;
            return true;
        }

        private static bool IncreasesAttackCost(Action<List<EnergyType>> modify)
        {
            var probe = new List<EnergyType> { EnergyType.Colorless, EnergyType.Colorless };
            modify(probe);
            return probe.Count > 2;
        }

        public bool CanPayCost(PokemonInPlay mon, IList<EnergyType> cost)
        {
            return EnergyMask.CanPay(EnergyUnits(mon), cost);
        }

        public bool CanPayAttack(PokemonInPlay mon, AttackDef attack)
        {
            return CanPayCost(mon, AttackCost(mon, attack));
        }

        /// <summary>"If this Pokémon has at least N extra Energy attached (in addition to this attack's cost)".</summary>
        public bool HasExtraEnergy(PokemonInPlay mon, AttackDef attack, int extra)
        {
            var cost = AttackCost(mon, attack);
            for (int i = 0; i < extra; i++) cost.Add(EnergyType.Colorless);
            return CanPayCost(mon, cost);
        }

        // ------------------------------------------------------------------ Bench, Tools, Trainers

        /// <summary>How many Pokémon Tools this Pokémon can have attached: 1, or more while an effect allows it (Rotom ex's Multi Adapter).</summary>
        public int MaxTools(PokemonInPlay mon)
        {
            int max = 1;
            if (mon == null) return max;
            if (HasAncientTrait(mon, ThetaDouble)) max = 2; // θ Double (v98): up to 2 Pokémon Tools
            foreach (var src in SourcesFor(mon)) max = src.Script.ModifyMaxTools(src.Ctx, mon, max);
            return Math.Max(1, max);
        }

        /// <summary>True when another Pokémon Tool can be attached to this Pokémon.</summary>
        public bool CanTakeTool(PokemonInPlay mon)
        {
            return mon != null && PokemonToolCount(mon) < MaxTools(mon);
        }

        /// <summary>
        /// v144: the Pokémon Tools attached to a Pokémon, not counting old Technical Machines (Expedition's Multi Technical Machine
        /// 01): they're attached like Tools but aren't Pokémon Tools, so they don't take the Tool's place (Compendium, 2000-12-21:
        /// a Tool can go on a Pokémon with a non-Tool Trainer attached).
        /// </summary>
        public int PokemonToolCount(PokemonInPlay mon)
        {
            if (mon == null) return 0;
            int n = 0;
            foreach (var t in mon.Tools) if (!Scripts.For(t.Def).IsAttachedNonTool) n++;
            return n;
        }

        public int MaxBench(int player)
        {
            int max = PlayerState.DefaultMaxBench;
            var sources = Sources();
            foreach (var src in sources) max = src.Script.ModifyMaxBench(src.Ctx, player, max);
            // Collapsed Stadium: "If more than one effect changes the number of Benched Pokémon allowed, use the smaller number."
            foreach (var src in sources) max = Math.Min(max, src.Script.CapMaxBench(src.Ctx, player));
            return Math.Max(0, Math.Min(PlayerState.MaxBenchLimit, max));
        }

        /// <summary>Eternal Zone (v51): something stops the player putting this Pokémon into play by any means.</summary>
        public bool PutIntoPlayBlocked(int player, CardDef card)
        {
            if (card == null || !card.IsPokemon || State.InSetup) return false;
            // Shedinja's Shell Survival (v62): a Basic Pokémon that only its own named effect can put into play.
            if (card.IsBasicPokemon && Scripts.For(card).PutIntoPlayOnlyByEffect) return true;
            // A single V-UNION piece is never put into play (Compendium: Bubble Pod, Ultra Ball, 2021-09-30).
            if (card.IsVUnion) return true;
            return SourcesBlockPuttingIntoPlay(player, card);
        }

        /// <summary>Something in play stops this player putting this Pokémon into play (Eternatus VMAX's Eternal Zone and the like).</summary>
        private bool SourcesBlockPuttingIntoPlay(int player, CardDef card)
        {
            foreach (var src in Sources())
                if (src.Script.BlocksPuttingIntoPlay(src.Ctx, player, card)) return true;
            return false;
        }

        /// <summary>
        /// V-UNION (v67): the sets of 4 different pieces of one Pokémon V-UNION in this player's discard pile that could be combined
        /// now (each V-UNION once per game, with room on the Bench, during their turn). Each set lists one card of each piece, piece 1 first.
        /// </summary>
        public List<List<CardInstance>> VUnionsReady(int player)
        {
            var ready = new List<List<CardInstance>>();
            var p = State.Players[player];
            if (State.InSetup || !BenchHasRoom(player)) return ready;
            foreach (var group in p.Discard.Where(c => c.Def.IsPokemon && c.Def.IsVUnion).GroupBy(c => c.Def.Name))
            {
                if (p.VUnionsCombined.Contains(group.Key)) continue; // your ruling (v67): once per game for each V-UNION
                var pieces = VUnionPieces(group.ToList());
                if (pieces != null && !SourcesBlockPuttingIntoPlay(player, pieces[0].Def)) ready.Add(pieces);
            }
            return ready;
        }

        /// <summary>One card of each of 4 different pieces (by main printing) in piece order, or null if a piece is missing.</summary>
        private static List<CardInstance> VUnionPieces(List<CardInstance> cards)
        {
            var byPiece = new SortedDictionary<string, CardInstance>(StringComparer.Ordinal);
            foreach (var c in cards)
                if (!byPiece.ContainsKey(c.Def.BaseKey)) byPiece[c.Def.BaseKey] = c;
            if (byPiece.Count < 4) return null;
            return byPiece.Values.OrderBy(x => PieceNumber(x.Def.BaseKey)).Take(4).ToList();
        }

        private static int PieceNumber(string key)
        {
            int dash = key.LastIndexOf('-');
            int n;
            return dash >= 0 && int.TryParse(key.Substring(dash + 1), out n) ? n : int.MaxValue;
        }

        /// <summary>
        /// A card that can be put down as a Basic Pokémon during setup or benched from the hand: not Shedinja, which only Ninjask's
        /// Cast-Off Shell puts into play (v62). Snorlax Doll is handled by its own rules.
        /// </summary>
        public bool IsPlayableBasic(CardDef d)
        {
            return d != null && d.IsBasicPokemon && !Scripts.For(d).PutIntoPlayOnlyByEffect;
        }

        /// <summary>
        /// A card the player may put face down during setup like a Basic Pokémon: a Basic Pokémon, or Manectric's Electric Start
        /// when this player goes second (v83). Snorlax Doll and Cinderace are handled where setup asks for them.
        /// </summary>
        public bool SetupPlaceable(int player, CardDef d)
        {
            if (IsPlayableBasic(d)) return true;
            return d != null && d.IsPokemon && player != State.FirstPlayer && Scripts.For(d).StartsFaceDownWhenGoingSecond;
        }

        /// <summary>Shiftry's Shifty Substitution (v62): the Supporter cards in this player's hand only "Draw 3 cards".</summary>
        public bool SupportersDrawThree(int player)
        {
            foreach (var src in Sources())
                if (src.Ctx.Player != player && src.Script.TurnsOpponentsSupportersIntoDrawThree(src.Ctx)) return true;
            return false;
        }

        /// <summary>The Pokémon can take this Pokémon Tool: room for a Tool, and nothing on the Tool forbids it (Hero's Medal, v62).</summary>
        public bool CanAttachToolTo(CardInstance tool, PokemonInPlay mon)
        {
            if (mon == null) return false;
            bool nonTool = tool != null && Scripts.For(tool.Def).IsAttachedNonTool;
            if (!nonTool && !CanTakeTool(mon)) return false;
            if (tool == null || Scripts.For(tool.Def).CanAttachTo(new EffectContext(this, mon.Owner, tool, mon), mon)) return true;
            // v147: Xatu's Synchronicity: any Technical Machine (the old attached TMs with an attack) can go on it.
            if (!nonTool || tool.Def.Attacks.Count == 0) return false;
            foreach (var src in SourcesOn(mon))
                if (src.Script.AllowsAnyTechnicalMachine(src.Ctx, mon)) return true;
            return false;
        }

        /// <summary>Windup Arm (v51): the Pokémon can attack even while Asleep or Paralyzed.</summary>
        public bool CanAttackWhileAsleepOrParalyzed(PokemonInPlay mon)
        {
            if (mon == null) return false;
            foreach (var src in SourcesFor(mon))
                if (src.Script.AllowsAttackWhileAsleepOrParalyzed(src.Ctx, mon)) return true;
            return false;
        }

        /// <summary>Escape Board (v85): the Pokémon can retreat even while Asleep or Paralyzed.</summary>
        public bool CanRetreatWhileAsleepOrParalyzed(PokemonInPlay mon)
        {
            if (mon == null) return false;
            foreach (var src in SourcesFor(mon))
                if (src.Script.AllowsRetreatWhileAsleepOrParalyzed(src.Ctx, mon)) return true;
            return false;
        }

        /// <summary>True when the condition keeps the Pokémon from attacking: Asleep or Paralyzed, unless Windup Arm lets it.</summary>
        public bool ConditionStopsAttack(PokemonInPlay mon)
        {
            if (mon == null) return false;
            if (!mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Paralyzed)) return false;
            return !CanAttackWhileAsleepOrParalyzed(mon);
        }

        /// <summary>Barbaracle's Lost Block (v51): the Prize cards this player takes go to their Lost Zone.</summary>
        public bool PrizesGoToLostZone(int taker)
        {
            foreach (var src in Sources())
                if (src.Script.SendsPrizesToLostZone(src.Ctx, taker)) return true;
            return false;
        }

        public bool BenchHasRoom(int player)
        {
            return State.Players[player].Bench.Count < MaxBench(player);
        }

        public int BenchSpace(int player)
        {
            return Math.Max(0, MaxBench(player) - State.Players[player].Bench.Count);
        }

        public bool ToolsDisabled()
        {
            if (StadiumAndToolsSwitchedOff()) return true;
            return State.Stadium != null && Scripts.For(State.Stadium.Def).DisablesTools(StadiumContext());
        }

        /// <summary>Shiftry's Wicked Wind (v92): an effect that makes every Stadium and Pokémon Tool card in play have no effect.</summary>
        public bool StadiumAndToolsSwitchedOff()
        {
            foreach (var e in State.Effects)
                if (e.ActiveFromTurn <= State.TurnNumber && e.SwitchesOffStadiumAndTools(this)) return true;
            return false;
        }

        /// <summary>The Stadium in play has no effect right now (Wicked Wind). It's still in play: "if X is in play" checks still see it.</summary>
        public bool StadiumSwitchedOff()
        {
            return State.Stadium != null && StadiumAndToolsSwitchedOff();
        }

        /// <summary>
        /// Tool Jammer (v60): a Tool on an Active Pokémon has no effect while the other player's Active Pokémon has a Tool
        /// Jammer attached (Tool Jammers themselves keep working). Only the cards are looked at, so this never asks about
        /// Tools or Abilities again.
        /// </summary>
        public bool ToolWorks(PokemonInPlay mon, CardInstance tool)
        {
            if (mon == null || tool == null) return true;
            if (ToolsOffByAbility(mon)) return false;
            if (ToolsOffByOpponentsAbility(mon)) return false;
            if (!State.IsActive(mon)) return true;
            if (Scripts.For(tool.Def).JamsOpposingActiveTools) return true;
            var other = State.Players[1 - mon.Owner].Active;
            if (other == null) return true;
            foreach (var t in other.Tools)
                if (Scripts.For(t.Def).JamsOpposingActiveTools) return false;
            return true;
        }

        private bool checkingToolAbility;
        private bool checkingActiveToolAbility;

        /// <summary>
        /// Yveltal's Fright Night (v97, while it's Active) and Banette's Tool Concealment (v99, anywhere in play): a Pokémon whose
        /// working Ability makes every Pokémon Tool in play have no effect. A θ Stop Pokémon's own Tools keep working against the
        /// opponent's (Compendium: Tool Concealment vs Theta Stop, 2015). Guarded against re-entry like Ancient Custom.
        /// </summary>
        private bool ToolsOffByAbility(PokemonInPlay holder)
        {
            if (checkingActiveToolAbility) return false;
            for (int i = 0; i < 2; i++)
            {
                foreach (var blocker in State.Players[i].InPlay())
                {
                    if (blocker.Def.Abilities.Count == 0) continue;
                    var script = Scripts.For(blocker.Def);
                    bool all = script.DisablesAllTools || (script.DisablesAllToolsWhileActive && State.IsActive(blocker));
                    if (!all) continue;
                    if (holder != null && blocker.Owner != holder.Owner && HasAncientTrait(holder, ThetaStop)) continue;
                    checkingActiveToolAbility = true;
                    try { if (HasUnblockedAbility(blocker, 0)) return true; }
                    finally { checkingActiveToolAbility = false; }
                }
            }
            return false;
        }

        /// <summary>
        /// Carracosta's Ancient Custom (v72): Tools on this Pokémon have no effect while one of the other player's Pokémon has that
        /// Ability working. Guarded against re-entry: whether that Ability is blocked is asked with Tools counted as working.
        /// </summary>
        private bool ToolsOffByOpponentsAbility(PokemonInPlay mon)
        {
            if (checkingToolAbility) return false;
            List<PokemonInPlay> blockers = null;
            foreach (var other in State.Players[1 - mon.Owner].InPlay())
            {
                if (other.Def.Abilities.Count == 0 || !Scripts.For(other.Def).DisablesOpponentsTools) continue;
                if (blockers == null) blockers = new List<PokemonInPlay>();
                blockers.Add(other);
            }
            if (blockers == null) return false;
            checkingToolAbility = true;
            try
            {
                // Stealthy Hood protects its holder (Compendium, 2020-03-12), and so does anything else that keeps the opponent's
                // Abilities off this Pokémon. Tools count as working while this is asked, so it can't loop.
                if (ShieldedFromOpponentsAbilities(mon)) return false;
                foreach (var blocker in blockers)
                    if (HasUnblockedAbility(blocker, 0)) return true;
                return false;
            }
            finally { checkingToolAbility = false; }
        }

        /// <summary>Centiskorch's Overheater (v60): the other player keeps this Burned Pokémon from recovering on heads.</summary>
        public bool BurnRecoveryBlocked(PokemonInPlay mon)
        {
            if (mon == null) return false;
            foreach (var src in Sources())
            {
                // Wela Volcano Park (v82): a Stadium keeps every player's Burned Pokémon Burned, its owner's too.
                if (src.Kind == EffectSourceKind.Stadium)
                {
                    if (!StadiumShielded(mon) && src.Script.KeepsOpponentBurned(src.Ctx, mon.Owner)) return true;
                    continue;
                }
                if (src.Ctx.Player != mon.Owner && src.Script.KeepsOpponentBurned(src.Ctx, mon.Owner)) return true;
            }
            return false;
        }

        /// <summary>Something in play stops this player from playing this Trainer card (Jellicent ex, Genesect...).</summary>
        public bool TrainerBlocked(int player, CardInstance card)
        {
            // Professor Elm, PokéGear, Time Capsule (v138): "You can't play any more Trainer cards this turn."
            if (player >= 0 && State.Players[player].TrainersLockedTurn == State.TurnNumber) return true;
            foreach (var src in Sources())
                if (src.Script.BlocksTrainer(src.Ctx, player, card)) return true;
            foreach (var e in ActiveEffects())
                if (e.BlocksTrainer(this, player, card) && !AttackEffectOnPlayerIgnored(e, player)) return true;
            return false;
        }

        /// <summary>The player can't play this Pokémon card from their hand right now (Potent Glare): not onto the Bench, not to evolve.</summary>
        public bool PokemonPlayBlocked(int player, CardDef card)
        {
            if (card == null || !card.IsPokemon || State.InSetup) return false;
            foreach (var src in Sources())
                if (src.Script.BlocksPlayingPokemon(src.Ctx, player, card)) return true;
            foreach (var e in ActiveEffects())
                if (e.BlocksPlayingPokemon(this, player, card) && !AttackEffectOnPlayerIgnored(e, player)) return true;
            return false;
        }

        /// <summary>False while something keeps this Pokémon and its attached cards out of its owner's hand (Milotic's Mentally Calm).</summary>
        public bool CanPutIntoHand(PokemonInPlay mon)
        {
            if (mon == null) return false;
            foreach (var src in SourcesFor(mon))
                if (src.Script.KeepsOutOfHand(src.Ctx, mon)) return false;
            return true;
        }

        /// <summary>"If you go first, you can use this attack during your first turn" (Volbeat's Quick Sign).</summary>
        public bool UsableOnFirstTurn(PokemonInPlay mon, AttackChoice a)
        {
            var ctx = new EffectContext(this, mon.Owner, mon.Top, mon, a.Source == mon.Def ? null : a.Source);
            return Scripts.For(a.Source).UsableOnFirstTurn(ctx, a.Index);
        }

        /// <summary>False while an effect stops Energy cards being attached from the hand to this Pokémon (Corrosive Shards).</summary>
        public bool CanAttachFromHand(PokemonInPlay target)
        {
            foreach (var e in ActiveEffects()) if (e.BlocksEnergyFromHand(this, target)) return false;
            return true;
        }

        /// <summary>v146: something in play stops this Energy card being attached from the hand to this Pokémon (Aquapolis Zapdos's Anti-Lightning).</summary>
        public bool EnergyFromHandBlockedTo(CardInstance energy, PokemonInPlay target)
        {
            if (energy == null || target == null) return false;
            foreach (var src in SourcesFor(target)) if (src.Script.BlocksEnergyFromHandTo(src.Ctx, energy, target)) return true;
            return false;
        }

        /// <summary>True while an effect stops the player playing this Energy card from their hand (Noivern ex's Dominating Echo).</summary>
        public bool EnergyCardBlocked(int player, CardInstance card)
        {
            foreach (var e in ActiveEffects()) if (e.BlocksEnergyCard(this, player, card) && !AttackEffectOnPlayerIgnored(e, player)) return true;
            return EnergyCardBlockedBySource(player, card);
        }

        /// <summary>
        /// Honchkrow-GX's Ruler of the Night (v73): something in play stops this player playing this Energy card from their hand.
        /// Your ruling: that covers attaching it from the hand by any means (Crazy Code, Battery), not just the normal attachment.
        /// </summary>
        public bool EnergyCardBlockedBySource(int player, CardInstance card)
        {
            foreach (var src in Sources()) if (src.Script.BlocksEnergyCard(src.Ctx, player, card)) return true;
            return false;
        }

        public bool ForcesTrainerFlip(int player)
        {
            foreach (var e in ActiveEffects()) if (e.ForcesTrainerFlip(this, player) && !AttackEffectOnPlayerIgnored(e, player)) return true;
            return false;
        }

        public bool HasTeraInPlay(int player)
        {
            return State.Players[player].InPlay().Any(m => m.Def.IsTera);
        }

        public int PrizesTaken(int player)
        {
            // Prize cards discarded by Billowing Smoke aren't taken (Compendium, Revenge Blast).
            var p = State.Players[player];
            return Math.Max(0, PrizeCount + p.PrizeAdjust - p.Prizes.Count - p.PrizesDiscarded);
        }

        // ------------------------------------------------------------------ Preventions and locks

        /// <summary>
        /// "Prevent all effects of attacks (and Abilities) done to this Pokémon". Only effects of the other
        /// player's Pokémon can be prevented; damage is not an effect, damage counters are.
        /// </summary>
        public bool EffectsPrevented(PokemonInPlay target, EffectSourceKind kind, int sourceOwner, bool damageCounters)
        {
            if (target == null) return false;
            if (kind != EffectSourceKind.Attack && kind != EffectSourceKind.Ability) return false;
            if (sourceOwner < 0 || sourceOwner == target.Owner) return false;
            if (kind == EffectSourceKind.Ability && HasAncientTrait(target, ThetaStop)) return true;
            foreach (var src in SourcesFor(target))
                if (src.Script.PreventsEffects(src.Ctx, target, kind, damageCounters)) return true;
            foreach (var e in ActiveEffects())
                if (e.PreventsEffects(this, target, kind)) return true;
            return false;
        }

        /// <summary>
        /// EffectsPrevented with that Pokémon as the source: a lasting attack effect checked later, when nothing is resolving
        /// (Clear Vision-GX against a Keldeo-GX that came into play after it: Pure Heart looks at the attacking Pokémon).
        /// </summary>
        public bool EffectsPreventedFrom(PokemonInPlay target, EffectSourceKind kind, int sourceOwner, PokemonInPlay sourcePokemon)
        {
            sourceStack.Add(new SourceFrame { Kind = kind, Owner = sourceOwner, Pokemon = sourcePokemon });
            try
            {
                return EffectsPrevented(target, kind, sourceOwner, false);
            }
            finally
            {
                sourceStack.RemoveAt(sourceStack.Count - 1);
            }
        }

        /// <summary>Prevention check for whatever effect is resolving right now.</summary>
        public bool CurrentEffectPrevented(PokemonInPlay target, bool damageCounters)
        {
            if (target == null) return false;
            // Haunter's Transparency (v116): on heads, the attack resolving now does nothing to it.
            if (TransparencyPrevents(target)) return true;
            // Lunatone's New Moon: nothing a Stadium does reaches a shielded Pokémon (Gapejaw Bog, Risky Ruins, heals...).
            if (CurrentSourceKind == EffectSourceKind.Stadium && StadiumShielded(target)) return true;
            if (IsOpponentsAttackOrAbility(target)) return EffectsPrevented(target, CurrentSourceKind, CurrentSourceOwner, damageCounters);
            if (TrainerEffectStoppedOn(target)) return true;
            // The other player's Trainer card (Rhyperior's Wide Wall stops their Supporters).
            if (CurrentSourceKind == EffectSourceKind.Trainer && CurrentSourceOwner >= 0 && CurrentSourceOwner != target.Owner && ResolvingTrainer != null)
            {
                // Ω Barrier (v101): the opponent's Trainer cards, except Pokémon Tools and Stadiums, do nothing to it (2015-02-05).
                if (HasAncientTrait(target, OmegaBarrier) && !ResolvingTrainer.IsTool && !ResolvingTrainer.IsStadium) return true;
                foreach (var src in SourcesFor(target))
                    if (src.Script.PreventsTrainerEffects(src.Ctx, target, ResolvingTrainer)) return true;
            }
            return false;
        }

        /// <summary>
        /// Ω Barrier is the only thing stopping the opponent's Trainer card resolving now on this Pokémon (v101): without the trait,
        /// nothing would.
        /// </summary>
        public bool OnlyOmegaBarrierStops(PokemonInPlay target)
        {
            if (target == null || !HasAncientTrait(target, OmegaBarrier) || CurrentSourceKind != EffectSourceKind.Trainer || ResolvingTrainer == null) return false;
            if (CurrentSourceOwner < 0 || CurrentSourceOwner == target.Owner) return false;
            if (TrainerEffectStoppedOn(target)) return false;
            foreach (var src in SourcesFor(target))
                if (src.Script.PreventsTrainerEffects(src.Ctx, target, ResolvingTrainer)) return false;
            return true;
        }

        /// <summary>
        /// An effect stops what the Trainer card resolving now (played from the hand) does to this Pokémon: Ariados's Trapping
        /// Thread (v81) on the Defending Pokémon during the opponent's next turn.
        /// </summary>
        public bool TrainerEffectStoppedOn(PokemonInPlay target)
        {
            if (target == null || CurrentSourceKind != EffectSourceKind.Trainer || ResolvingTrainer == null) return false;
            foreach (var e in ActiveEffects())
                if (e.PreventsTrainerEffects(this, target, ResolvingTrainer)) return true;
            // v141: Legendary Body stops every player's Trainer cards on it.
            foreach (var src in SourcesOn(target))
                if (src.Script.PreventsAllTrainerEffects(src.Ctx, target, ResolvingTrainer)) return true;
            return false;
        }

        /// <summary>
        /// The Stadium in play isn't affected by the Item or Supporter card resolving now (Black Market ◇, Wondrous Labyrinth ◇:
        /// "Whenever any player plays an Item or Supporter card from their hand, prevent all effects of that card done to this
        /// Stadium card", v80).
        /// </summary>
        public bool StadiumIgnoresCurrentTrainer()
        {
            var stadium = State.Stadium;
            if (stadium == null || CurrentSourceKind != EffectSourceKind.Trainer || ResolvingTrainer == null) return false;
            if (!ResolvingTrainer.IsItem && !ResolvingTrainer.IsSupporter) return false;
            return Scripts.For(stadium.Def).IgnoresItemsAndSupporters;
        }

        /// <summary>The Trainer card whose effect is resolving right now (played from the hand), or null.</summary>
        public CardDef ResolvingTrainer { get; private set; }

        /// <summary>
        /// Milotic's Dew Guard (v58): the effect resolving now is a Supporter card the other player played from their hand, and
        /// something on this player's side prevents all effects of it done to them or their hand (draws, shuffles, reveals and
        /// discards from the hand). Effects on their Pokémon in play still happen.
        /// </summary>
        public bool SupporterEffectsPreventedOn(int player)
        {
            if (player < 0 || CurrentSourceKind != EffectSourceKind.Trainer || CurrentSourceOwner != 1 - player) return false;
            if (ResolvingTrainer == null || !ResolvingTrainer.IsSupporter) return false;
            foreach (var src in Sources())
                if (src.Ctx.Player == player && src.Script.ShieldsPlayerFromSupporters(src.Ctx)) return true;
            return false;
        }

        /// <summary>One Special Condition can't affect the Pokémon (Insomnia: Asleep).</summary>
        public bool ConditionPrevented(PokemonInPlay target, SpecialCondition condition)
        {
            foreach (var src in SourcesFor(target))
                if (src.Script.PreventsCondition(src.Ctx, target, condition)) return true;
            // Bayleef's Pollen Shield (v138): a timed effect that keeps some Special Conditions off the Pokémon.
            foreach (var e in ActiveEffects())
                if (e.PreventsCondition(this, target, condition)) return true;
            return false;
        }

        /// <summary>
        /// Phione's Murmurs of the Sea (v82): "can't be Confused. If those Pokémon are already Confused, remove that Special
        /// Condition." Removes the conditions that such a source prevents (checked with the other rule checks after each action).
        /// </summary>
        public void CurePreventedConditions(PokemonInPlay mon)
        {
            if (mon == null || mon.Conditions == SpecialCondition.None) return;
            foreach (var src in SourcesFor(mon))
            {
                if (!src.Script.CuresConditionsItPrevents) continue;
                foreach (SpecialCondition c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                    if (mon.Has(c) && src.Script.PreventsCondition(src.Ctx, mon, c)) RemoveCondition(mon, c);
            }
        }

        public bool SpecialConditionsPrevented(PokemonInPlay target)
        {
            foreach (var src in SourcesFor(target))
                if (src.Script.PreventsSpecialConditions(src.Ctx, target)) return true;
            foreach (var e in ActiveEffects())
                if (e.PreventsSpecialConditions(this, target)) return true;
            return false;
        }

        public bool HealingBlocked(PokemonInPlay mon)
        {
            foreach (var src in SourcesFor(mon))
                if (src.Script.BlocksHealing(src.Ctx, mon)) return true;
            // v85: attack effects on the Pokémon (Lunala-GX's Moongeist Beam).
            foreach (var e in ActiveEffects())
                if (e.BlocksHealing(this, mon)) return true;
            return false;
        }

        public bool CounterMovementBlocked()
        {
            foreach (var src in Sources())
                if (src.Script.BlocksCounterMovement(src.Ctx)) return true;
            return false;
        }

        public bool AttacksPrevented(PokemonInPlay mon)
        {
            foreach (var e in ActiveEffects()) if (e.PreventsAttacks(this, mon)) return true;
            foreach (var src in SourcesFor(mon)) if (src.Script.PreventsAttacksOf(src.Ctx, mon)) return true;
            return false;
        }

        /// <summary>
        /// Toedscruel's Slime Mold Colony: the effect resolving now is the player's own Ability or Trainer card, and the other
        /// player has something in play that keeps cards in the player's discard pile out of their hand.
        /// </summary>
        public bool DiscardToHandBlocked(int player)
        {
            if (player < 0) return false;
            // Pokémon Tower: "If the effect of a Pokémon Power, attack, Energy card, or Trainer card would put a card in a discard
            // pile into its owner's hand, that card stays in that discard pile instead." Abilities, Poké-Powers and Poké-Bodies
            // aren't Pokémon Powers here (your ruling, v48).
            if (CurrentEffectIsPowerAttackEnergyOrTrainer())
                foreach (var src in Sources())
                    if (src.Script.KeepsDiscardPilesOutOfHands(src.Ctx)) return true;
            if (CurrentSourceOwner != player) return false;
            var kind = CurrentSourceKind;
            if (kind != EffectSourceKind.Ability && kind != EffectSourceKind.PokemonPower && kind != EffectSourceKind.Trainer && kind != EffectSourceKind.Stadium)
                return false;
            foreach (var src in Sources())
                if (src.Ctx.Player != player && src.Script.BlocksDiscardToHand(src.Ctx, player)) return true;
            return false;
        }

        /// <summary>
        /// Pokémon Tower's list: the effect resolving now is an attack, a Trainer card (a Stadium too), a Special Energy card,
        /// or an old-style Pokémon Power (Base Set to Neo). Abilities, Poké-Powers and Poké-Bodies don't count (your ruling).
        /// </summary>
        public bool CurrentEffectIsPowerAttackEnergyOrTrainer()
        {
            var kind = CurrentSourceKind;
            if (kind == EffectSourceKind.Attack || kind == EffectSourceKind.Trainer || kind == EffectSourceKind.Stadium || kind == EffectSourceKind.Energy)
                return true;
            if (kind != EffectSourceKind.PokemonPower) return false;
            var mon = CurrentSourcePokemon;
            if (mon == null) return false;
            foreach (var a in mon.Def.Abilities)
                if (a.Kind == AbilityDef.PokemonPower) return true;
            return false;
        }

        /// <summary>Sandshrew's Sand Screen: the player's own Item or Supporter is resolving and the other player blocks it.</summary>
        public bool TrainerDiscardToDeckBlocked(int player)
        {
            if (player < 0 || CurrentSourceOwner != player || CurrentSourceKind != EffectSourceKind.Trainer) return false;
            if (ResolvingTrainer == null || !(ResolvingTrainer.IsItem || ResolvingTrainer.IsSupporter)) return false;
            foreach (var src in Sources())
                if (src.Ctx.Player != player && src.Script.BlocksTrainerDiscardToDeck(src.Ctx, player)) return true;
            return false;
        }

        /// <summary>
        /// Hoothoot's Stand Sentry (v53): the effect resolving now is the other player's Item or Supporter card, and something on
        /// the holder's side keeps this Energy from being discarded by it.
        /// </summary>
        public bool EnergyDiscardProtected(PokemonInPlay holder, CardInstance energy)
        {
            if (holder == null || energy == null) return false;
            if (EnergyProtectedByEffect(holder, energy)) return true;
            // Carbink's Energy Keeper (v94): the opponent's attacks, Abilities and Trainer cards (any kind, Stadiums too).
            if (CurrentSourceOwner >= 0 && CurrentSourceOwner != holder.Owner
                && (CurrentSourceKind == EffectSourceKind.Attack || CurrentSourceKind == EffectSourceKind.Ability || CurrentSourceKind == EffectSourceKind.PokemonPower
                    || CurrentSourceKind == EffectSourceKind.Trainer || CurrentSourceKind == EffectSourceKind.Stadium))
            {
                foreach (var src in Sources())
                    if (src.Ctx.Player == holder.Owner && src.Script.ProtectsEnergyFromOpponentsDiscard(src.Ctx, holder, energy)) return true;
            }
            if (CurrentSourceKind != EffectSourceKind.Trainer) return false;
            if (CurrentSourceOwner < 0 || CurrentSourceOwner == holder.Owner) return false;
            if (ResolvingTrainer == null || !(ResolvingTrainer.IsItem || ResolvingTrainer.IsSupporter)) return false;
            foreach (var src in Sources())
            {
                if (src.Ctx.Player != holder.Owner) continue;
                if (src.Script.ProtectsEnergyFromTrainerDiscard(src.Ctx, holder, energy)) return true;
                if (src.Script.ProtectsEnergyFromTrainerRemoval(src.Ctx, holder, energy)) return true;
            }
            return false;
        }

        /// <summary>
        /// Brock's Protection (v126): the effect resolving now is the other player's attack or Trainer card (any kind), and a timed
        /// effect on the holder keeps its Energy from being removed by it.
        /// </summary>
        private bool EnergyProtectedByEffect(PokemonInPlay holder, CardInstance energy)
        {
            if (holder == null || energy == null || CurrentSourceOwner < 0 || CurrentSourceOwner == holder.Owner) return false;
            if (CurrentSourceKind != EffectSourceKind.Attack && CurrentSourceKind != EffectSourceKind.Trainer && CurrentSourceKind != EffectSourceKind.Stadium) return false;
            foreach (var fx in ActiveEffects())
                if (fx.ProtectsEnergy(this, holder, energy)) return true;
            return false;
        }

        /// <summary>
        /// Greedent's Brazen Tail (v59): the effect resolving now is the other player's Item or Supporter card, and something on
        /// the holder's side keeps this Energy from being put into a hand or a deck by it (discarding is EnergyDiscardProtected).
        /// </summary>
        public bool EnergyRemovalProtected(PokemonInPlay holder, CardInstance energy)
        {
            if (EnergyProtectedByEffect(holder, energy)) return true;
            if (holder == null || energy == null || CurrentSourceKind != EffectSourceKind.Trainer) return false;
            if (CurrentSourceOwner < 0 || CurrentSourceOwner == holder.Owner) return false;
            if (ResolvingTrainer == null || !(ResolvingTrainer.IsItem || ResolvingTrainer.IsSupporter)) return false;
            foreach (var src in Sources())
                if (src.Ctx.Player == holder.Owner && src.Script.ProtectsEnergyFromTrainerRemoval(src.Ctx, holder, energy)) return true;
            return false;
        }

        /// <summary>
        /// Patrol Cap: the effect resolving now is the other player's attack, Ability or Trainer card (Item, Supporter or
        /// Tool), and something on this player's side keeps their deck from being discarded.
        /// </summary>
        public bool DeckDiscardBlocked(int player)
        {
            if (player < 0 || CurrentSourceOwner != 1 - player) return false;
            var kind = CurrentSourceKind;
            if (kind != EffectSourceKind.Attack && kind != EffectSourceKind.Ability && kind != EffectSourceKind.Trainer) return false;
            foreach (var src in Sources())
                if (src.Ctx.Player == player && src.Script.ProtectsDeckFromDiscard(src.Ctx, player)) return true;
            return false;
        }

        public bool AttackNamedPrevented(PokemonInPlay mon, string attackName)
        {
            foreach (var e in ActiveEffects()) if (e.PreventsAttackNamed(this, mon, attackName)) return true;
            return false;
        }

        public bool RetreatPrevented(PokemonInPlay mon)
        {
            foreach (var e in ActiveEffects()) if (e.PreventsRetreat(this, mon)) return true;
            foreach (var src in SourcesFor(mon))
            {
                if (!src.Script.PreventsRetreatOf(src.Ctx, mon)) continue;
                // v64: Fusion Strike Energy stops an opponent's Ability that keeps the Pokémon from retreating (Flygon's Labyrinth
                // of Sand, Snorlax's Block); the check is made when the Retreat starts (Compendium, 2022-03-31).
                if (src.Kind == EffectSourceKind.Ability && EffectsPrevented(mon, EffectSourceKind.Ability, src.Ctx.Player, false)) continue;
                return true;
            }
            return false;
        }

        /// <summary>Damage counters put on the Poisoned Pokémon during Pokémon Checkup (Pecharunt adds 5).</summary>
        public int BurnCounters(PokemonInPlay mon)
        {
            int n = 2;
            foreach (var src in SourcesFor(mon)) n = src.Script.ModifyBurnCounters(src.Ctx, mon, n);
            return Math.Max(0, n);
        }

        public int PoisonCounters(PokemonInPlay mon)
        {
            int n = mon.PoisonCounterBase;
            foreach (var src in SourcesFor(mon)) n = src.Script.ModifyPoisonCounters(src.Ctx, mon, n);
            return Math.Max(0, n);
        }

        // ------------------------------------------------------------------ Attacks available

        /// <summary>An attack a Pokémon can use: its own (Source = its card) or borrowed (Mew ex's Memory Helix).</summary>
        public struct AttackChoice
        {
            public CardDef Source;
            public int Index;
            public AttackDef Attack { get { return Source.Attacks[Index]; } }
        }

        /// <summary>Every attack this Pokémon has right now, before Energy and other checks.</summary>
        public List<AttackChoice> AttacksOf(PokemonInPlay mon)
        {
            var list = new List<AttackChoice>();
            var seen = new HashSet<string>();
            // Brock's Ninetales's Shapeshift (v126): the attacks of the Pokémon it's treated as, instead of its own.
            var shape = ShapeshiftCard(mon);
            if (shape != null)
            {
                for (int i = 0; i < shape.Attacks.Count; i++)
                    if (seen.Add(shape.BaseKey + "|" + i)) list.Add(new AttackChoice { Source = shape, Index = i });
                // ...and the attacks that Pokémon's Power gives it (Sabrina's Alakazam's Psylink).
                var shapeCard = InheritedPowersCard(mon);
                if (shapeCard != null && InheritedWorks(mon, shapeCard, 0))
                    foreach (var kv in Scripts.For(shapeCard.Def).ExtraAttacks(new EffectContext(this, mon.Owner, shapeCard, mon)))
                        if (kv.Key.Name != shape.Name && seen.Add(kv.Key.BaseKey + "|" + kv.Value)) list.Add(new AttackChoice { Source = kv.Key, Index = kv.Value });
                return list;
            }
            for (int i = 0; i < mon.Def.Attacks.Count; i++)
                if (seen.Add(mon.Def.BaseKey + "|" + i)) list.Add(new AttackChoice { Source = mon.Def, Index = i });
            // LV.X and BREAK Pokémon can also use the attacks of the card they were put on.
            if ((mon.Def.IsLevelUp || mon.Def.IsBreak) && mon.Stack.Count >= 2)
            {
                var below = mon.Stack[mon.Stack.Count - 2].Def;
                for (int i = 0; i < below.Attacks.Count; i++)
                    if (seen.Add(below.BaseKey + "|" + i)) list.Add(new AttackChoice { Source = below, Index = i });
            }
            if (mon.Def.Abilities.Count > 0 && HasUnblockedAbility(mon, 0))
            {
                var ownScript = Scripts.For(mon.Def);
                foreach (var kv in ownScript.ExtraAttacks(new EffectContext(this, mon.Owner, mon.Top, mon)))
                    if ((kv.Key.Name != mon.Def.Name || ownScript.ExtraAttacksMayShareName) && seen.Add(kv.Key.BaseKey + "|" + kv.Value))
                        list.Add(new AttackChoice { Source = kv.Key, Index = kv.Value });
            }
            // Fossil Ditto's Transform (v116): the copied Pokémon's attacks.
            var copiedAttacks = TransformTarget(mon);
            if (copiedAttacks != null)
                for (int i = 0; i < copiedAttacks.Def.Attacks.Count; i++)
                    if (seen.Add(copiedAttacks.Def.BaseKey + "|" + i)) list.Add(new AttackChoice { Source = copiedAttacks.Def, Index = i });
            // Other cards' effects that give this Pokémon attacks (Relicanth's Memory Dive).
            foreach (var src in SourcesFor(mon))
                foreach (var kv in src.Script.ExtraAttacksFor(src.Ctx, mon))
                    if (kv.Key.Name != mon.Def.Name && seen.Add(kv.Key.BaseKey + "|" + kv.Value)) list.Add(new AttackChoice { Source = kv.Key, Index = kv.Value });
            // Timed effects that give it attacks (Recall, v122: the attacks of the cards under it).
            foreach (var fx in ActiveEffects())
                foreach (var kv in fx.ExtraAttacksFor(this, mon))
                    if (kv.Key.Name != mon.Def.Name && seen.Add(kv.Key.BaseKey + "|" + kv.Value)) list.Add(new AttackChoice { Source = kv.Key, Index = kv.Value });
            return list;
        }

        /// <summary>
        /// Glaceon-GX's Freezing Gaze (v69): an Ability of the other player's Pokémon in play says this card of the player has
        /// no Abilities in the hand or discard pile.
        /// </summary>
        public bool OutOfPlayAbilitiesBlocked(int player, CardDef card)
        {
            if (card == null || !card.IsPokemon || card.Abilities.Count == 0) return false;
            foreach (var src in Sources())
            {
                if (src.Kind != EffectSourceKind.Ability && src.Kind != EffectSourceKind.PokemonPower) continue;
                if (src.Ctx.Player == player)
                {
                    if (src.Script.BlocksOwnAbilitiesOutOfPlay(src.Ctx, card)) return true;
                    continue;
                }
                if (src.Script.BlocksOpponentsAbilitiesOutOfPlay(src.Ctx, card)) return true;
            }
            foreach (var e in ActiveEffects())
                if (e.BlocksAbilitiesOutOfPlay(this, player, card)) return true;
            return false;
        }

        /// <summary>Misty &amp; Lorelei (v68): this turn, the player's Water Pokémon can use GX attacks even after their GX attack was used.</summary>
        public bool GxAllowedAgain(PokemonInPlay mon)
        {
            if (mon == null) return false;
            var p = State.Players[mon.Owner];
            // Bonnie (FLI, v84): "your Zygarde-GX can use its GX attack even if you have used your GX attack" this turn.
            if (p.ZygardeGxThisTurn && mon.Def.Name == "Zygarde-GX") return true;
            return p.WaterGxThisTurn && IsType(mon, EnergyType.Water);
        }

        /// <summary>GX attacks ("Sol Burst-GX"): a player can use only 1 in a game.</summary>
        public static bool IsGxAttack(AttackDef atk)
        {
            return atk != null && atk.Name != null && atk.Name.EndsWith("-GX");
        }

        /// <summary>
        /// Attacks this Benched Pokémon can use from the Bench ("You can use this attack even while this Pokémon is on the
        /// Bench"). Special Conditions only affect the Active Pokémon, so they don't stop it; "can't attack" effects on
        /// this Pokémon or its owner do.
        /// </summary>
        public List<AttackChoice> BenchAttacks(PokemonInPlay mon)
        {
            var list = new List<AttackChoice>();
            if (mon == null || State.IsActive(mon) || AttacksPrevented(mon)) return list;
            foreach (var a in UsableAttacks(mon))
                if (Scripts.For(a.Source).UsableFromBench(a.Attack)) list.Add(a);
            return list;
        }

        /// <summary>Attacks the Pokémon could use now if it were allowed to attack: Energy paid, not locked, card conditions met.</summary>
        public List<AttackChoice> UsableAttacks(PokemonInPlay mon)
        {
            var list = new List<AttackChoice>();
            // Asleep or Paralyzed: only an attack usable while Asleep (Dream Waltz, v112), and only when the Pokémon isn't Paralyzed.
            bool stopped = ConditionStopsAttack(mon);
            bool asleepOnly = stopped && !mon.Has(SpecialCondition.Paralyzed);
            foreach (var a in AttacksOf(mon))
            {
                var atk = a.Attack;
                if (stopped && (!asleepOnly || !Scripts.For(a.Source).UsableWhileAsleep(atk))) continue;
                if (AttackNamedPrevented(mon, atk.Name)) continue;
                if (IsGxAttack(atk) && State.Players[mon.Owner].GxUsed && !GxAllowedAgain(mon)) continue; // 1 GX attack per game (one that didn't happen doesn't count)
                if (atk.VstarPower && State.Players[mon.Owner].VstarUsed) continue; // 1 VSTAR Power per game (v49)
                if (!CanPayAttack(mon, atk)) continue;
                var ctx = new EffectContext(this, mon.Owner, mon.Top, mon, a.Source == mon.Def ? null : a.Source);
                if (!Scripts.For(a.Source).CanUseAttack(ctx, a.Index)) continue;
                list.Add(a);
            }
            return list;
        }
    }
}
