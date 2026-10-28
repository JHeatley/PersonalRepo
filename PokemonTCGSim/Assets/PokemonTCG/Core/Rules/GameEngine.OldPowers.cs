using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Fossil's rule-changing Pokémon Powers (v116). Each stops working while its Pokémon is Asleep, Confused or Paralyzed.
    /// - Muk's Toxic Gas: every other Pokémon Power is ignored (your ruling: only old Pokémon Powers; Poké-Powers, Poké-Bodies
    ///   and Abilities keep working).
    /// - Aerodactyl's Prehistoric Power: no Evolution cards can be played, by either player, in any way (your ruling).
    /// - Ditto's Transform: while Active it is a copy of the opponent's Active Pokémon (type, HP, Weakness, Resistance, Retreat
    ///   Cost, attacks, and, by your ruling, its Pokémon Powers and Abilities); its Energy counts as every type.
    /// - Omanyte's Clairvoyance: the opponent plays with their hand face up.
    /// - Haunter's Transparency: whenever an attack (any attack, your ruling) does anything to Haunter, its owner flips a coin
    ///   once for that attack; heads, the attack does nothing to Haunter.
    /// Team Rocket (v121): Goop Gas Attack and Dark Arbok's Stare are timed effects that stop Pokémon Powers
    /// (ContinuousEffect.StopsPokemonPower), Toxic Gas included.
    /// </summary>
    public sealed partial class GameEngine
    {
        public const string ToxicGasPower = "Toxic Gas";
        public const string PrehistoricPowerName = "Prehistoric Power";
        public const string TransformPower = "Transform";
        public const string ClairvoyancePower = "Clairvoyance";
        public const string TransparencyPower = "Transparency";
        private const string TransparencyMarker = "Transparency flip";

        /// <summary>Counts attacks, so Transparency flips once per attack.</summary>
        private int attackSerial;

        private static bool PowerAwake(PokemonInPlay mon)
        {
            return mon != null && !mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Confused) && !mon.Has(SpecialCondition.Paralyzed);
        }

        private static bool HasPrintedPower(CardDef def, string name)
        {
            if (def == null) return false;
            foreach (var a in def.Abilities)
                if (a.Name == name && a.Kind == AbilityDef.PokemonPower) return true;
            return false;
        }

        /// <summary>
        /// A Muk's Toxic Gas works right now (its printed power). Nothing stops Toxic Gas but the Muk's own condition and the timed
        /// effects that stop Pokémon Powers (Goop Gas Attack, Stare: v121).
        /// </summary>
        public bool ToxicGasActive()
        {
            for (int i = 0; i < 2; i++)
                foreach (var mon in State.Players[i].InPlay())
                    if (HasPrintedPower(mon.Def, ToxicGasPower) && PowerAwake(mon) && !PokemonPowerStoppedByEffect(mon, ToxicGasPower)) return true;
            return false;
        }

        /// <summary>A printed Pokémon Power on this Pokémon that works now (awake, not ignored by Toxic Gas, not stopped by an effect).</summary>
        private bool PrintedPowerWorks(PokemonInPlay mon, string name)
        {
            return HasPrintedPower(mon.Def, name) && PowerAwake(mon) && !PokemonPowerStoppedByEffect(mon, name) && (name == ToxicGasPower || !ToxicGasActive());
        }

        /// <summary>
        /// v121: a timed effect stops this Pokémon's Pokémon Power: Goop Gas Attack (every Pokémon Power until the end of the
        /// opponent's next turn) or Dark Arbok's Stare (that Pokémon's). Only old Pokémon Powers (your ruling).
        /// </summary>
        public bool PokemonPowerStoppedByEffect(PokemonInPlay mon, string powerName)
        {
            if (mon == null) return false;
            foreach (var e in State.Effects)
                if (e.ActiveFromTurn <= State.TurnNumber && e.StopsPokemonPower(this, mon, powerName)) return true;
            // v142: a Pokémon Power in play that stops other Pokémon's Pokémon Powers (Dark Feraligatr's Scare).
            for (int i = 0; i < 2; i++)
                foreach (var other in State.Players[i].InPlay())
                {
                    if (other == mon || other.Def.Abilities.Count == 0) continue;
                    if (Scripts.For(other.Def).StopsPokemonPowersOf(new EffectContext(this, other.Owner, other.Top, other), mon)) return true;
                }
            return false;
        }

        /// <summary>
        /// v146: a timed effect stops this Pokémon's Poké-Power (Aquapolis Houndoom's Dark Impact). Only Poké-Powers. v147: so does a
        /// Poké-Body in play (Skyridge Umbreon's Dark Gaze stops the Benched Pokémon's Poké-Powers); the script checks its own body works.
        /// </summary>
        public bool PokePowerStoppedByEffect(PokemonInPlay mon, string powerName)
        {
            if (mon == null) return false;
            foreach (var e in State.Effects)
                if (e.ActiveFromTurn <= State.TurnNumber && e.StopsPokePower(this, mon, powerName)) return true;
            // v162: a Stadium that stops Poké-Powers (Mt. Moon: Pokémon with less than 70 HP).
            if (State.Stadium != null && !StadiumSwitchedOff() && Scripts.For(State.Stadium.Def).StopsPokePowersOf(StadiumContext(), mon)) return true;
            // v168: a Pokémon Tool on it that stops its Poké-Powers (Energy Root).
            foreach (var tool in mon.Tools)
                if (Scripts.For(tool.Def).StopsPokePowersOf(new EffectContext(this, mon.Owner, tool, mon), mon)) return true;
            for (int i = 0; i < 2; i++)
                foreach (var other in State.Players[i].InPlay())
                {
                    if (other == mon || other.Def.Abilities.Count == 0) continue;
                    if (Scripts.For(other.Def).StopsPokePowersOf(new EffectContext(this, other.Owner, other.Top, other), mon)) return true;
                }
            return false;
        }

        /// <summary>v147: a timed effect stops this Pokémon's Poké-Body (Skyridge Aerodactyl's Ancient Wind). Only Poké-Bodies.</summary>
        public bool PokeBodyStoppedByEffect(PokemonInPlay mon, string bodyName)
        {
            if (mon == null) return false;
            foreach (var e in State.Effects)
                if (e.ActiveFromTurn <= State.TurnNumber && e.StopsPokeBody(this, mon, bodyName)) return true;
            // v165: a Stadium that has Poké-Bodies ignored (Space Center: Basic Pokémon's, except Pokémon-ex and owners' Pokémon).
            if (State.Stadium != null && !StadiumSwitchedOff())
            {
                var stadium = Scripts.For(State.Stadium.Def);
                if (Overrides(stadium, "StopsPokeBodiesOf") && stadium.StopsPokeBodiesOf(StadiumContext(), mon, bodyName)) return true;
            }
            // v168: a Pokémon Tool on it that stops its Poké-Bodies (Energy Root).
            foreach (var tool in mon.Tools)
            {
                var toolScript = Scripts.For(tool.Def);
                if (Overrides(toolScript, "StopsPokeBodiesOf") && toolScript.StopsPokeBodiesOf(new EffectContext(this, mon.Owner, tool, mon), mon, bodyName)) return true;
            }
            // v159: a Poké-Body in play that stops other Pokémon's Poké-Bodies (Muk ex's Toxic Gas).
            for (int i = 0; i < 2; i++)
                foreach (var other in State.Players[i].InPlay())
                {
                    if (other == mon || other.Def.Abilities.Count == 0) continue;
                    var script = Scripts.For(other.Def);
                    if (!Overrides(script, "StopsPokeBodiesOf")) continue; // cheap: almost no card has it
                    if (script.StopsPokeBodiesOf(new EffectContext(this, other.Owner, other.Top, other), mon, bodyName)) return true;
                }
            return false;
        }

        /// <summary>Aerodactyl's Prehistoric Power: no one can evolve a Pokémon right now.</summary>
        public bool PrehistoricPowerActive()
        {
            for (int i = 0; i < 2; i++)
                foreach (var mon in State.Players[i].InPlay())
                    if (PrintedPowerWorks(mon, PrehistoricPowerName)) return true;
            return false;
        }

        /// <summary>
        /// Fossil Ditto's Transform: the opponent's Active Pokémon this Ditto is a copy of right now, or null. A Ditto copies the
        /// other Pokémon's printed card, so two Ditto never copy each other in a loop.
        /// </summary>
        public PokemonInPlay TransformTarget(PokemonInPlay mon)
        {
            if (mon == null || !State.IsActive(mon) || !PrintedPowerWorks(mon, TransformPower)) return null;
            var opp = State.Players[1 - mon.Owner].Active;
            return opp == null || opp == mon ? null : opp;
        }

        /// <summary>The card whose printed Weakness, Resistance and Retreat Cost the Pokémon has (a BREAK Pokémon's lower card; Transform).</summary>
        public CardDef StatsCardOf(PokemonInPlay mon)
        {
            var copied = TransformTarget(mon);
            if (copied != null) return PrintedStatsOf(copied);
            // Brock's Ninetales's Shapeshift (v126): the Weakness, Resistance and Retreat Cost of the Pokémon it's treated as.
            return ShapeshiftCard(mon) ?? PrintedStatsOf(mon);
        }

        /// <summary>Omanyte's Clairvoyance: this player's hand is face up (the opponent has a working Clairvoyance).</summary>
        public bool HandFaceUp(int player)
        {
            foreach (var mon in State.Players[1 - player].InPlay())
                if (PrintedPowerWorks(mon, ClairvoyancePower)) return true;
            return false;
        }

        /// <summary>
        /// Haunter's Transparency against the attack resolving now: the first time that attack does anything to Haunter, its owner
        /// flips a coin; heads, that attack does nothing more to it (the result is kept for the rest of the attack).
        /// </summary>
        public bool TransparencyPrevents(PokemonInPlay target)
        {
            if (target == null || attacksResolving <= 0 || CurrentSourceKind != EffectSourceKind.Attack) return false;
            int seen;
            if (target.Markers.TryGetValue(TransparencyMarker, out seen) && seen / 2 == attackSerial) return seen % 2 == 1;
            string power = PrintedPowerWorks(target, TransparencyPower) ? TransparencyPower : DodgeFlipPower(target);
            if (power == null) return false;
            bool heads = FlipCoin(target.Owner, power);
            target.Markers[TransparencyMarker] = attackSerial * 2 + (heads ? 1 : 0);
            if (heads) Emit(GameEventType.DamagePrevented, target.Owner, power + ": the attack does nothing to " + target.Name + ".", target.Top.Id, target.Uid);
            return heads;
        }

        /// <summary>
        /// v146: a working Poké-Body of the Pokémon that flips like Transparency against the attack resolving now (Aquapolis
        /// Jumpluff's Fluff), or null.
        /// </summary>
        private string DodgeFlipPower(PokemonInPlay target)
        {
            if (target.Def.Abilities.Count == 0 || !HasUnblockedAbility(target, 0)) return null;
            return Scripts.For(target.Def).FlipsToDodgeAttack(new EffectContext(this, target.Owner, target.Top, target), target);
        }

        /// <summary>
        /// An Evolution card that Prehistoric Power stops from going onto a Pokémon goes back where such cards come from: the hand
        /// for a card played from it, otherwise the deck (shuffled).
        /// </summary>
        private void ReturnBlockedEvolution(PokemonInPlay target, CardInstance evolution, bool fromHand)
        {
            var p = State.Players[evolution.Owner];
            if (fromHand) p.Hand.Add(evolution);
            else
            {
                p.Deck.Add(evolution);
                State.Rng.Shuffle(p.Deck);
            }
            Emit(GameEventType.Info, target.Owner, target.Name + " can't evolve: Prehistoric Power. " + evolution.Def.Name + " went back to " + Their(evolution.Owner) + (fromHand ? " hand." : " deck, which was shuffled."), evolution.Id, target.Uid);
        }
    }
}
