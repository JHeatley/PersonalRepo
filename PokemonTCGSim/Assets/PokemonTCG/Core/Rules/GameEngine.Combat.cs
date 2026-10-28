using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    public sealed partial class GameEngine
    {
        // ------------------------------------------------------------------ Attack damage

        /// <summary>Attack damage from the attacker to the target. Weakness and Resistance apply only to the Active Pokémon.</summary>
        public int DealAttackDamage(PokemonInPlay attacker, PokemonInPlay target, int amount, bool applyWeaknessAndResistance = true)
        {
            if (target == null) return 0;
            return DealDamage(new DamageInfo
            {
                Attacker = attacker,
                Target = target,
                ApplyWeakness = applyWeaknessAndResistance,
                ApplyResistance = applyWeaknessAndResistance
            }, amount);
        }

        /// <summary>
        /// The damage pipeline, in rulebook order:
        /// 1. attacker-side changes ("does X more damage", before Weakness and Resistance),
        /// 2. Weakness x2 and Resistance -30 (Active Pokémon only),
        /// 3. Tera Pokémon on the Bench take no damage from attacks,
        /// 4. defender-side changes ("takes X less damage", prevention), unless the attack ignores them,
        /// then damage counters are placed and "if this Pokémon is damaged by an attack" effects trigger.
        /// </summary>
        public int DealDamage(DamageInfo info, int amount)
        {
            var target = info.Target;
            var attacker = info.Attacker;
            if (target == null || amount <= 0) return 0;
            if (State.FindPokemon(target.Uid) != target) return 0;
            // Lt. Surge's Secret Plan (v126): a face-down card is flipped over before it's damaged; one that isn't a Basic
            // Pokémon is discarded and takes nothing.
            if (IsFaceDown(target) && !FlipFaceDown(target)) return 0;
            info.TargetIsActive = State.IsActive(target);
            // Haunter's Transparency (v116): on heads, the attack does nothing to it, damage included.
            if (attacker != null && TransparencyPrevents(target)) return 0;
            // Phoebe (v60): damage from the player's Pokémon VMAX's attacks isn't affected by effects on the opponent's Active Pokémon.
            if (!info.IgnoreDefenderEffects && attacker != null && info.TargetIsActive && attacker.Owner != target.Owner
                && CurrentSourceKind == EffectSourceKind.Attack)
                foreach (var fx in ActiveEffects())
                    if (fx.IgnoresDefenderEffects(this, info)) { info.IgnoreDefenderEffects = true; break; }
            var sources = Sources();
            // Zacian LV.X's Bladed Armament (v67): a Poké-Body that makes its own attacks' damage ignore effects on the Defending Pokémon.
            if (!info.IgnoreDefenderEffects && attacker != null && info.TargetIsActive && attacker.Owner != target.Owner
                && CurrentSourceKind == EffectSourceKind.Attack)
                foreach (var src in sources)
                    if (src.Script.AttacksIgnoreDefenderEffects(src.Ctx, info)) { info.IgnoreDefenderEffects = true; break; }
            // Lunatone's New Moon: a Stadium doesn't change the attacks of a shielded attacker or the damage a shielded Pokémon takes.
            var dealtSources = attacker != null && StadiumShielded(attacker) ? WithoutStadium(sources) : sources;
            var takenSources = StadiumShielded(target) ? WithoutStadium(sources) : sources;
            int dmg = amount;
            // Wide Lens (v99): the holder's attack damage to the opponent's Benched Pokémon is affected by Weakness and Resistance.
            if (!info.TargetIsActive && !info.WeaknessAndResistanceOnBench && attacker != null && attacker.Owner != target.Owner && CurrentSourceKind == EffectSourceKind.Attack)
                foreach (var src in SourcesOn(attacker))
                    if (src.Script.AppliesWeaknessAndResistanceOnBench(src.Ctx, info)) { info.WeaknessAndResistanceOnBench = true; break; }

            // Hawlucha's Shining Spirit (v103): an Ability that keeps its own attacks' damage clear of Weakness and Resistance.
            if (attacker != null && attacker.Owner != target.Owner && CurrentSourceKind == EffectSourceKind.Attack && (info.ApplyWeakness || info.ApplyResistance))
                foreach (var src in SourcesOn(attacker))
                    if (src.Script.OwnAttacksIgnoreWeaknessAndResistance(src.Ctx, info)) { info.ApplyWeakness = false; info.ApplyResistance = false; break; }
            foreach (var src in dealtSources) dmg = src.Script.ModifyDamageDealt(src.Ctx, info, dmg);
            foreach (var e in ActiveEffects()) dmg = e.ModifyDamageBeforeWeakness(this, attacker, target, dmg);
            if (dmg < 0) dmg = 0;

            if ((info.TargetIsActive || info.WeaknessAndResistanceOnBench) && attacker != null && attacker != target && dmg > 0)
            {
                // A Pokémon with 2 types (Dual Core) gets Weakness or Resistance if either type matches (applied once).
                var types = TypesOf(attacker);
                // Cinnabar City Gym (v126): Weakness is ignored for this damage.
                if (info.ApplyWeakness)
                    foreach (var src in sources)
                        if (src.Script.IgnoresWeakness(src.Ctx, info)) { info.ApplyWeakness = false; break; }
                if (info.ApplyWeakness)
                {
                    var weakness = WeaknessOf(target);
                    // EX-era cards with 2 Weaknesses (v158): either one, applied once.
                    var weakness2 = SecondWeaknessOf(target);
                    if ((weakness.HasValue && types.Contains(weakness.Value)) || (weakness2.HasValue && types.Contains(weakness2.Value)))
                    {
                        if (StatsCardOf(target).WeaknessAmount > 0)
                        {
                            dmg += StatsCardOf(target).WeaknessAmount; // older cards: Weakness "+20"
                        }
                        else
                        {
                            // Illumise's Supereffective Pheromones: "apply Weakness for both Active Pokémon as x3".
                            int multiplier = 2;
                            foreach (var src in sources) multiplier = Math.Max(multiplier, src.Script.WeaknessMultiplier(src.Ctx, info));
                            dmg *= multiplier;
                        }
                    }
                }
                var resistance = ResistanceOf(target);
                var resistance2 = SecondResistanceOf(target);
                bool resists = (resistance.HasValue && types.Contains(resistance.Value)) || (resistance2.HasValue && types.Contains(resistance2.Value));
                if (info.ApplyResistance && resists && !ResistanceIgnored(sources, info) && !EffectIgnoresResistance(info))
                    dmg = Math.Max(0, dmg - ResistanceAmountOf(target));
            }

            // A Benched Tera Pokémon's protection is an effect on that Pokémon, so damage that isn't affected by effects on
            // it gets through (Compendium: Umbreon's Feint Attack).
            if (!info.TargetIsActive && target.Def.IsTera && attacker != null && !info.IgnoreDefenderEffects)
            {
                Emit(GameEventType.DamagePrevented, target.Owner, target.Name + " is protected on the Bench (Tera).", target.Top.Id, target.Uid);
                return 0;
            }

            if (!info.IgnoreDefenderEffects && dmg > 0)
            {
                foreach (var src in takenSources) dmg = src.Script.ModifyDamageTaken(src.Ctx, info, dmg);
                foreach (var e in ActiveEffects()) dmg = e.ModifyDamageTaken(this, info, dmg);
                // Δ Wild (Ancient Trait, v99): 20 less from the opponent's Grass, Fire, Water and Lightning Pokémon's attacks.
                if (HasAncientTrait(target, DeltaWild) && info.Attacker != null && info.AgainstOpponent
                    && (IsType(info.Attacker, EnergyType.Grass) || IsType(info.Attacker, EnergyType.Fire) || IsType(info.Attacker, EnergyType.Water) || IsType(info.Attacker, EnergyType.Lightning)))
                    dmg -= 20;
                if (dmg < 0) dmg = 0;
                if (dmg > 0)
                {
                    foreach (var src in takenSources)
                    {
                        if (!src.Script.PreventsFinalDamage(src.Ctx, info, dmg)) continue;
                        dmg = 0;
                        break;
                    }
                    if (dmg > 0)
                        foreach (var fx in ActiveEffects())
                        {
                            if (!fx.PreventsFinalDamage(this, info, dmg)) continue;
                            dmg = 0;
                            break;
                        }
                }
            }

            if (dmg <= 0)
            {
                Emit(GameEventType.DamagePrevented, target.Owner, target.Name + " took no damage.", target.Top.Id, target.Uid);
                return 0;
            }

            bool wasFullHp = target.DamageCounters == 0;
            int counters = dmg / 10;
            target.DamageCounters += counters;
            if (attacker != null)
            {
                target.HitByAttack = true;
                target.DevolvedAfterHit = false;
                if (info.TargetIsActive)
                {
                    target.HitWhileActive = true;
                    target.ActiveAttackDamageTurn = State.TurnNumber;
                }
                target.HitByPlayer = attacker.Owner;
                target.HitByAttackerDef = attacker.Def;
                target.HitByAttackerUid = attacker.Uid;
                if (target.AttackDamageTurn != State.TurnNumber) target.AttackDamageThatTurn = 0;
                target.AttackDamageTurn = State.TurnNumber;
                target.AttackDamageThatTurn += dmg;
            }
            Emit(GameEventType.Damage, target.Owner, OwnedName(target) + " took " + dmg + " damage (" + Math.Max(0, RemainingHp(target)) + " HP left).", target.Top.Id, target.Uid, dmg);

            // Durable Body: "If this Pokémon would be Knocked Out by damage from an attack..."
            // Torracat's Gritty Claws (v50): an attack's effect on this Pokémon does the same for the opponent's attacks.
            // Acme of Heroism-GX (v73) does the same at any HP. When more than one effect would save it (Focus Sash and Acme of
            // Heroism-GX, Compendium 2019-06-27), its owner chooses which one applies (v104).
            if (attacker != null && !target.ForcedKnockOut && IsKnockedOut(target))
            {
                var savingCards = new List<PassiveSource>();
                foreach (var src in SourcesOn(target))
                    if (src.Script.SurvivesKnockOut(src.Ctx, target)
                        || (wasFullHp && attacker.Owner != target.Owner && src.Script.SurvivesKnockOutFromFullHp(src.Ctx, target)))
                        savingCards.Add(src);
                var savingEffects = new List<ContinuousEffect>();
                if (attacker.Owner != target.Owner)
                    foreach (var fx in ActiveEffects())
                        if ((wasFullHp && fx.SurvivesKnockOutFromFullHp(this, target)) || fx.SurvivesKnockOutFromAttack(this, target))
                            savingEffects.Add(fx);
                int pick = 0;
                if (savingCards.Count + savingEffects.Count > 1)
                {
                    var options = new List<DecisionOption>();
                    foreach (var src in savingCards) options.Add(new DecisionOption { Label = src.Ctx.Card.Def.Name, Card = src.Ctx.Card.Def, Index = options.Count });
                    foreach (var fx in savingEffects) options.Add(new DecisionOption { Label = fx.Name, Index = options.Count });
                    pick = AskOne(target.Owner, DecisionKind.ChooseOption, "Which effect keeps " + target.Name + " from being Knocked Out?", options);
                }
                if (savingCards.Count + savingEffects.Count > 0)
                {
                    target.DamageCounters = Math.Max(0, (MaxHp(target) - 10) / 10);
                    target.SavedTo10HpTurn = State.TurnNumber;
                    target.SavedTo10HpCounters = target.DamageCounters;
                    target.SavedTo10HpMaxHp = MaxHp(target);
                    if (pick < savingCards.Count)
                    {
                        var src = savingCards[pick];
                        Emit(GameEventType.Info, target.Owner, target.Name + " wasn't Knocked Out (" + src.Ctx.Card.Def.Name + ").", target.Top.Id, target.Uid);
                        PushSource(src.Kind, target.Owner, target);
                        try { src.Script.OnSavedFromKnockOut(src.Ctx, target); }
                        finally { PopSource(); }
                    }
                    else
                    {
                        var fx = savingEffects[pick - savingCards.Count];
                        Emit(GameEventType.Info, target.Owner, target.Name + " wasn't Knocked Out (" + fx.Name + ").", target.Top.Id, target.Uid);
                    }
                }
            }

            if (attacker != null && attacker.Owner != target.Owner)
            {
                // Triggered effects on the Defending side happen after the attack's own effects.
                if (attacksResolving > 0) pendingDamageTriggers.Add(new KeyValuePair<DamageInfo, int>(info, dmg));
                else FireDamagedTriggers(info, dmg);
            }
            return dmg;
        }

        /// <summary>
        /// "The Attacking Pokémon" for a Pokémon damaged by an attack during the current action: the Pokémon that used the
        /// attack, wherever it is now (it may have attacked from the Bench, or switched itself out during the attack).
        /// Null if it has left play or the Pokémon wasn't damaged by an attack.
        /// </summary>
        public PokemonInPlay AttackingPokemonOf(PokemonInPlay damaged)
        {
            if (damaged == null || damaged.HitByAttackerUid < 0) return null;
            return State.FindPokemon(damaged.HitByAttackerUid);
        }

        /// <summary>
        /// Fires the "damaged by an attack" triggers queued while the attack resolved, in the order the damage was done.
        /// A Pokémon that left play during the attack (shuffled into the deck, say) has nothing left to trigger; one that
        /// was Knocked Out is still in play until Knock Outs are checked, so its triggers still happen.
        /// </summary>
        private void FlushDamageTriggers()
        {
            var pending = pendingDamageTriggers.ToList();
            pendingDamageTriggers.Clear();
            foreach (var kv in pending)
            {
                if (State.IsOver) return;
                var target = kv.Key.Target;
                if (target == null || State.FindPokemon(target.Uid) != target) continue;
                FireDamagedTriggers(kv.Key, kv.Value);
            }
        }

        /// <summary>Tools, Special Energy, Abilities and timed effects that react to the holder being damaged by an opponent's attack.</summary>
        private void FireDamagedTriggers(DamageInfo info, int dmg)
        {
            var target = info.Target;
            foreach (var src in SourcesOn(target))
            {
                PushSource(src.Kind, target.Owner, target);
                try { src.Script.OnHolderDamagedByAttack(src.Ctx, info.Attacker, info.TargetIsActive, dmg); }
                finally { PopSource(); }
            }
            // v136: printed text that isn't an Ability (Darkrai's Enigma Berry, a held item).
            if (target.Def.Abilities.Count == 0)
            {
                var ownScript = Scripts.For(target.Def);
                if (ownScript.ReactsToOwnDamageWithoutAbility)
                {
                    PushSource(EffectSourceKind.Rule, target.Owner, target);
                    try { ownScript.OnHolderDamagedByAttack(new EffectContext(this, target.Owner, target.Top, target), info.Attacker, info.TargetIsActive, dmg); }
                    finally { PopSource(); }
                }
            }
            // The owner's other cards in play that react to it (Spiritomb's Spiteful Swirl).
            foreach (var src in Sources())
            {
                if (src.Ctx.Player != target.Owner || src.Ctx.Pokemon == null || src.Ctx.Pokemon == target) continue;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnOwnPokemonDamagedByAttack(src.Ctx, target, info.Attacker, info.TargetIsActive, dmg); }
                finally { PopSource(); }
            }
            foreach (var e in ActiveEffects().ToList())
            {
                PushSource(EffectSourceKind.Attack, e.Owner, null);
                try { e.OnTargetDamaged(this, info, dmg); }
                finally { PopSource(); }
            }
        }

        // ------------------------------------------------------------------ Damage counters

        /// <summary>Puts damage counters directly. Not damage: ignores Weakness, Resistance and Tera, but effect prevention applies.</summary>
        public int PlaceDamageCounters(PokemonInPlay target, int counters, string reason)
        {
            if (target == null || counters <= 0) return 0;
            if (State.FindPokemon(target.Uid) != target) return 0;
            if (IsFaceDown(target) && !FlipFaceDown(target)) return 0; // Lt. Surge's Secret Plan (v126)
            if (CurrentEffectPrevented(target, true))
            {
                Emit(GameEventType.DamagePrevented, target.Owner, reason + ": the damage counters on " + target.Name + " were prevented.", target.Top.Id, target.Uid);
                return 0;
            }
            target.DamageCounters += counters;
            Emit(GameEventType.DamageCounters, target.Owner, reason + ": " + counters + (counters == 1 ? " damage counter" : " damage counters") + " on " + target.Name + ".", target.Top.Id, target.Uid, counters);
            return counters;
        }

        public int MoveDamageCounters(PokemonInPlay from, PokemonInPlay to, int counters, string reason)
        {
            counters = Math.Min(counters, from.DamageCounters);
            if (counters <= 0 || from == to) return 0;
            if (CounterMovementBlocked())
            {
                Emit(GameEventType.Info, to.Owner, reason + ": damage counters can't be moved.");
                return 0;
            }
            // Compendium (Shady Move vs Daunting Pose, 2017; Painful Spoons vs Fusion Strike Energy, 2023): counters can't be
            // moved off a protected Pokémon, and counters moved onto a protected Pokémon are removed but never placed.
            if (CurrentEffectPrevented(from, false))
            {
                Emit(GameEventType.DamagePrevented, from.Owner, reason + ": moving damage counters from " + from.Name + " was prevented.", from.Top.Id, from.Uid);
                return 0;
            }
            if (CurrentEffectPrevented(to, true))
            {
                from.DamageCounters -= counters;
                Emit(GameEventType.DamagePrevented, to.Owner, reason + ": " + to.Name + " is protected, so the " + counters + " damage counters taken off " + from.Name + " are removed.", to.Top.Id, to.Uid);
                return 0;
            }
            from.DamageCounters -= counters;
            to.DamageCounters += counters;
            Emit(GameEventType.DamageCounters, to.Owner, reason + ": moved " + counters + " damage counters from " + from.Name + " to " + to.Name + ".", to.Top.Id, to.Uid, counters);
            return counters;
        }

        /// <summary>
        /// Wobbuffet V's Gritty Comeback (v66): "switch all damage counters on this Pokémon with those on your opponent's Active
        /// Pokémon". The counters move both ways, so it's stopped like any move of damage counters and by a protected target.
        /// Returns true when the counters were switched.
        /// </summary>
        public bool SwapDamageCounters(PokemonInPlay mine, PokemonInPlay other, string reason)
        {
            if (mine == null || other == null || mine == other) return false;
            if (State.FindPokemon(mine.Uid) != mine || State.FindPokemon(other.Uid) != other) return false;
            if (CounterMovementBlocked())
            {
                Emit(GameEventType.Info, other.Owner, reason + ": damage counters can't be moved.");
                return false;
            }
            if (CurrentEffectPrevented(other, true))
            {
                Emit(GameEventType.DamagePrevented, other.Owner, reason + ": " + other.Name + " is protected, so no damage counters are switched.", other.Top.Id, other.Uid);
                return false;
            }
            int onMine = mine.DamageCounters;
            int onOther = other.DamageCounters;
            mine.DamageCounters = onOther;
            other.DamageCounters = onMine;
            Emit(GameEventType.DamageCounters, other.Owner, reason + ": " + mine.Name + " now has " + onOther + " and " + other.Name + " " + onMine + " damage counters.", other.Top.Id, other.Uid, Math.Max(0, onMine - onOther));
            return true;
        }

        /// <summary>"This Pokémon is Knocked Out" / "Both Active Pokémon are Knocked Out". Resolved with the other Knock Outs.</summary>
        public bool KnockOut(PokemonInPlay mon, string reason)
        {
            if (mon == null || State.FindPokemon(mon.Uid) != mon) return false;
            if (IsFaceDown(mon) && !FlipFaceDown(mon)) return false; // Lt. Surge's Secret Plan (v126)
            if (CurrentEffectPrevented(mon, false))
            {
                Emit(GameEventType.DamagePrevented, mon.Owner, reason + ": the effect on " + mon.Name + " was prevented.", mon.Top.Id, mon.Uid);
                return false;
            }
            mon.ForcedKnockOut = true;
            Emit(GameEventType.Info, mon.Owner, reason + ": " + mon.Name + " is Knocked Out.", mon.Top.Id, mon.Uid);
            return true;
        }

        // ------------------------------------------------------------------ Knock Outs and winning

        /// <summary>
        /// A Pokémon whose remaining HP became 10 this turn (Resolute Heart) keeps 10 HP if its max HP then drops with no new
        /// damage: Pikachu ex at full HP with Lively Stadium, hit for 230 (Vitality Band), survives Calamity Storm discarding
        /// the Stadium (Compendium, 2024-11-07). Hit for 220 instead, it was never saved, so the HP drop Knocks it Out.
        /// </summary>
        private void KeepSavedHp(PokemonInPlay mon)
        {
            if (mon.SavedTo10HpTurn != State.TurnNumber || mon.DamageCounters != mon.SavedTo10HpCounters) return;
            int max = MaxHp(mon);
            if (max >= mon.SavedTo10HpMaxHp || max <= 10) return;
            mon.DamageCounters = Math.Max(0, (max - 10) / 10);
            mon.SavedTo10HpCounters = mon.DamageCounters;
            mon.SavedTo10HpMaxHp = max;
            Emit(GameEventType.Info, mon.Owner, mon.Name + " keeps 10 HP.", mon.Top.Id, mon.Uid);
        }

        /// <summary>
        /// Prism Star rule (v80): a ◇ card in a discard pile goes to its owner's Lost Zone. The discard pile does this itself as
        /// cards are put in it; this catches a card put there some other way.
        /// </summary>
        public void SweepPrismStars()
        {
            foreach (var p in State.Players)
            {
                if (p.Discard.Count == 0) continue;
                for (int i = p.Discard.Count - 1; i >= 0; i--)
                {
                    var card = p.Discard[i];
                    if (card == null || card.Def == null || !card.Def.IsPrismStar) continue;
                    p.Discard.RemoveAt(i);
                    p.LostZone.Add(card);
                }
            }
        }

        public void ResolveKnockouts()
        {
            SweepPrismStars();
            if (State.IsOver) return;
            // Repeat while Knock Outs cause more (Exploding Needles puts counters on the attacker as its holder is Knocked Out).
            for (int pass = 0; pass < 10; pass++)
            {
            var knocked = new List<PokemonInPlay>();
            foreach (var p in State.Players)
                foreach (var mon in p.InPlay())
                {
                    KeepSavedHp(mon);
                    if (IsKnockedOut(mon)) knocked.Add(mon);
                }
            // Celebi's Leap Through Time (v107, your ruling): "If this Pokémon would be Knocked Out" on heads it is shuffled into its
            // owner's deck instead and was never Knocked Out (no Prize card, no Knock Out effects), whatever was Knocking it Out.
            for (int k = knocked.Count - 1; k >= 0; k--)
            {
                var mon = knocked[k];
                foreach (var src in SourcesOn(mon))
                {
                    // v141: old Pokémon Powers too (Celebi's Time Travel).
                    if (src.Kind != EffectSourceKind.Ability && src.Kind != EffectSourceKind.PokemonPower) continue;
                    PushSource(src.Kind, mon.Owner, mon);
                    bool escapes;
                    try { escapes = src.Script.EscapesKnockOut(src.Ctx, mon); }
                    finally { PopSource(); }
                    if (!escapes) continue;
                    knocked.RemoveAt(k);
                    ShufflePokemonIntoDeck(mon);
                    break;
                }
            }
            if (knocked.Count == 0) break;

            {
                // Prize counts are worked out while every Knocked Out Pokémon is still in play (Legacy Energy, Briar).
                var counts = new int[knocked.Count];
                for (int k = 0; k < knocked.Count; k++) counts[k] = PrizesFor(knocked[k]);
                // So is where they go: Gengar's Catastrophe still applies to an attacker Knocked Out at the same time as
                // Gengar (Compendium, 2010-11-04), whichever of them is removed first.
                var lostAt = new int[knocked.Count];
                for (int k = 0; k < knocked.Count; k++)
                {
                    foreach (var lzSource in Sources()) lostAt[k] = Math.Max(lostAt[k], lzSource.Script.LostZoneOnKnockOut(lzSource.Ctx, knocked[k]));
                    foreach (var lzMark in ActiveEffects()) lostAt[k] = Math.Max(lostAt[k], lzMark.LostZoneOnKnockOut(this, knocked[k]));
                }

                var prizes = new int[2];
                var smoked = new int[2];
                // "When this Pokémon is Knocked Out" effects that run once it has left play (Swelling Spite, v73).
                var removedTriggers = new List<Tuple<PassiveSource, PokemonInPlay, bool, bool>>();
                for (int k = 0; k < knocked.Count; k++)
                {
                    var mon = knocked[k];
                    var owner = State.Players[mon.Owner];
                    bool byAttack = mon.HitByAttack && !mon.ForcedKnockOut;
                    Emit(GameEventType.KnockOut, mon.Owner, OwnedName(mon) + " was Knocked Out!", mon.Top.Id, mon.Uid);
                    if (!State.InCheckup)
                    {
                        owner.LastKnockedOutTurn = State.TurnNumber;
                        if (byAttack) owner.LastKnockedOutByAttackTurn = State.TurnNumber;
                        if (mon.Def.IsTeamRockets) owner.LastTeamRocketKnockedOutTurn = State.TurnNumber;
                        owner.KnockOuts.Add(new KnockOutRecord(State.TurnNumber, mon.Def, byAttack));
                    }
                    // "If the Pokémon this card is attached to is Knocked Out..." (Heavy Baton), while it's still in play.
                    bool byOpponentsAttack = byAttack && mon.HitByPlayer == 1 - mon.Owner;
                    // Knocked Out by an attack: what counts is being Active when the damage landed, even if the attack then
                    // moved it to the Bench (Push Down). Compendium: Voltorb's Destiny Burst vs Rapid Spin (2014, reaffirmed 2018).
                    bool wasActive = byAttack ? mon.HitWhileActive : State.IsActive(mon);
                    // Billowing Smoke (v64): the Prize cards for this Knock Out are discarded instead of taken.
                    bool smoke = false;
                    if (byOpponentsAttack)
                        foreach (var src in SourcesOn(mon))
                            if (src.Script.DiscardsPrizesForHolder(src.Ctx)) { smoke = true; break; }
                    foreach (var src in SourcesOn(mon))
                    {
                        PushSource(src.Kind, mon.Owner, mon);
                        try { src.Script.OnHolderKnockedOut(src.Ctx, byOpponentsAttack, wasActive); }
                        finally { PopSource(); }
                    }
                    // v121 fix: an attack's own effect on a Pokémon with no Ability or Power (Base Set Gastly's Destiny Bond) isn't
                    // in SourcesOn, so its script is told here when it asks to be.
                    if (mon.Def.Abilities.Count == 0)
                    {
                        var ownScript = Scripts.For(mon.Def);
                        if (ownScript.ReactsToOwnKnockOutWithoutAbility)
                        {
                            PushSource(EffectSourceKind.Attack, mon.Owner, mon);
                            try { ownScript.OnHolderKnockedOut(new EffectContext(this, mon.Owner, mon.Top, mon), byOpponentsAttack, wasActive); }
                            finally { PopSource(); }
                        }
                    }
                    foreach (var src in SourcesOn(mon))
                        // v122 fix: Pokémon Powers, Poké-Powers and Poké-Bodies too (Base Set Electrode's Buzzap, Rocket's Moltres' Rebirth).
                        if (src.Kind == EffectSourceKind.Ability || src.Kind == EffectSourceKind.PokemonPower) removedTriggers.Add(Tuple.Create(src, mon, byOpponentsAttack, wasActive));
                    // "When 1 of your Pokémon is Knocked Out" (Huntail's Diver's Catch), while its cards are still attached.
                    foreach (var src in Sources())
                    {
                        if (src.Ctx.Player != mon.Owner) continue;
                        PushSource(src.Kind, mon.Owner, src.Ctx.Pokemon);
                        try { src.Script.OnOwnPokemonKnockedOut(src.Ctx, mon, byOpponentsAttack); }
                        finally { PopSource(); }
                    }
                    // "Put it into your hand instead of the discard pile" (Gengar's Infinite Shadow), checked while it's in play; Splash
                    // Energy (v96) does it too: the whole Pokémon goes to the hand, its attached cards are discarded (Compendium, 2016-02-11).
                    bool toHand = false;
                    foreach (var src in SourcesOn(mon))
                        // v125: a Pokémon Tool too (Rescue Scarf).
                        if ((src.Kind == EffectSourceKind.Ability || src.Kind == EffectSourceKind.PokemonPower || src.Kind == EffectSourceKind.Energy
                             || src.Kind == EffectSourceKind.Trainer)
                            && src.Script.GoesToHandWhenKnockedOut(src.Ctx, mon, byOpponentsAttack)) { toHand = true; break; }
                    // Lost Zone Knock Outs (Gengar's Catastrophe, Lost Crisis).
                    int lost = lostAt[k];
                    if (mon.LostZoneKnockOutTurn == State.TurnNumber && byAttack) lost = 2;
                    // Splash Energy and a Lost Zone Knock Out at once (Lost City): the owner chooses which happens (Compendium, 2022-09-08; v96).
                    if (lost > 0 && toHand && AskYesNo(mon.Owner, mon.Name + " was Knocked Out: put it into your hand instead of the Lost Zone?", null)) lost = 0;
                    RemoveFromPlay(mon);
                    if (lost > 0)
                    {
                        owner.LostZone.AddRange(mon.Stack);
                        var attached = new List<CardInstance>(mon.Energy);
                        attached.AddRange(mon.Tools);
                        if (lost >= 2) owner.LostZone.AddRange(attached);
                        else DiscardFromPlay(attached);
                        Emit(GameEventType.Info, mon.Owner, mon.Name + " went to the Lost Zone" + (lost >= 2 ? " with every card attached to it." : "; its attached cards were discarded."), -1, mon.Uid);
                    }
                    else if (toHand)
                    {
                        owner.Hand.AddRange(mon.Stack);
                        DiscardFromPlay(mon.Energy);
                        DiscardFromPlay(mon.Tools);
                        Emit(GameEventType.Info, mon.Owner, mon.Name + " went back to " + PlayerName(mon.Owner) + "'s hand; its attached cards were discarded.", mon.Top.Id, mon.Uid);
                    }
                    else
                    {
                        owner.Discard.AddRange(mon.Stack);
                        DiscardFromPlay(mon.Energy);
                        DiscardFromPlay(mon.Tools);
                    }
                    if (smoke) smoked[1 - mon.Owner] += counts[k];
                    else prizes[1 - mon.Owner] += counts[k];
                }
                foreach (var trig in removedTriggers)
                {
                    if (State.IsOver) break;
                    PushSource(trig.Item1.Kind, trig.Item2.Owner, trig.Item2);
                    try { trig.Item1.Script.OnHolderKnockedOutRemoved(trig.Item1.Ctx, trig.Item3, trig.Item4); }
                    finally { PopSource(); }
                }
                // The player whose turn is next takes their Prize cards first, then the other player.
                int next = 1 - State.CurrentPlayer;
                TakePrizes(next, prizes[next]);
                DiscardPrizes(next, smoked[next]);
                TakePrizes(1 - next, prizes[1 - next]);
                DiscardPrizes(1 - next, smoked[1 - next]);
            }
            }

            foreach (var p in State.Players)
                foreach (var mon in p.InPlay())
                {
                    mon.HitByAttack = false;
                    mon.DevolvedAfterHit = false;
                    mon.HitWhileActive = false;
                    mon.HitByPlayer = -1;
                    mon.HitByAttackerDef = null;
                    mon.HitByAttackerUid = -1;
                }

            // When both players meet a win condition at once, everything still happening finishes first (promoting, a
            // promoted Pokémon's Ability such as Tachyon Bits, the Knock Outs that causes), then whoever met more win
            // conditions wins (Compendium, 2024-09-26). Otherwise the game ends right away.
            bool simultaneous = WinConditionCount(0) > 0 && WinConditionCount(1) > 0;
            if (!simultaneous) CheckWinConditions();

            // A Bench left over its limit by the Knock Outs (the last Tera Pokémon under Area Zero Underdepths) is cut down
            // before the new Active Pokémon is chosen (Compendium: Area Zero Underdepths, 2024-09-12; Eternal Zone, 2020-08-20).
            if (!State.IsOver) EnforceBenchLimits(State.CurrentPlayer);
            if (!State.IsOver) EnforceToolLimits(State.CurrentPlayer);

            // Then the player whose turn is next promotes first, followed by the other player.
            int first = 1 - State.CurrentPlayer;
            for (int k = 0; k < 2; k++)
            {
                int i = k == 0 ? first : 1 - first;
                var p = State.Players[i];
                if (p.Active == null && p.Bench.Count > 0) PromoteActive(i, true);
            }

            StateChecks();

            if (simultaneous)
            {
                if (State.Players.Any(pl => pl.InPlay().Any(IsKnockedOut))) ResolveKnockouts();
                CheckWinConditions();
            }
        }

        /// <summary>Prize cards the opponent takes for this Knock Out, after Legacy Energy, Briar, Unown...</summary>
        public int PrizesFor(PokemonInPlay mon)
        {
            if (mon.Def.IsFossil && !mon.Def.FossilGivesPrize) return 0; // "your opponent can't take any Prize cards for it" (not Unidentified or Rare Fossil)
            int prizes = mon.Def.PrizeValue;
            int taker = 1 - mon.Owner;
            foreach (var src in Sources()) prizes = src.Script.ModifyPrizes(src.Ctx, mon, taker, prizes);
            foreach (var e in ActiveEffects()) prizes = e.ModifyPrizes(this, mon, taker, prizes);
            // Δ Plus (Ancient Trait, v99): Knocked Out by damage from that Pokémon's attack, 1 more Prize card.
            if (mon.HitByAttack && mon.HitByPlayer == taker && mon.HitByAttackerDef != null && mon.HitByAttackerDef.AncientTrait == DeltaPlus) prizes++;
            return Math.Max(0, prizes);
        }

        public void CheckWinConditions()
        {
            if (State.InSetup || State.IsOver) return;
            int c0 = WinConditionCount(0), c1 = WinConditionCount(1);
            if (c0 > 0 && c1 > 0)
            {
                // Both at once: the player who met more win conditions wins; the same number is a tie.
                if (c0 == c1) EndGame(-1, "Both players met a win condition at once, so the game is a tie.");
                else
                {
                    int winner = c0 > c1 ? 0 : 1;
                    EndGame(winner, PlayerName(winner) + " met " + Math.Max(c0, c1) + " win conditions to " + Math.Min(c0, c1) + " at the same time.");
                }
            }
            else if (c0 > 0) EndGame(0, WinReason(0));
            else if (c1 > 0) EndGame(1, WinReason(1));
        }

        /// <summary>
        /// How many win conditions the player meets right now: all Prize cards taken, the opponent has no Pokémon in play, and in a
        /// Tie-Breaker game a Prize lead (fewer Prize cards left than the opponent).
        /// </summary>
        private int WinConditionCount(int player)
        {
            int n = 0;
            if (State.Players[player].Prizes.Count == 0) n++;
            if (!State.Players[1 - player].HasPokemonInPlay) n++;
            if (PrizeLead(player)) n++;
            return n;
        }

        /// <summary>In a Tie-Breaker game: the player has fewer Prize cards left than the opponent.</summary>
        private bool PrizeLead(int player)
        {
            return TieBreaker && State.Players[player].Prizes.Count < State.Players[1 - player].Prizes.Count;
        }

        private string WinReason(int winner)
        {
            if (State.Players[winner].Prizes.Count == 0) return PlayerName(winner) + " took all of " + Their(winner) + " Prize cards.";
            if (PrizeLead(winner) && State.Players[1 - winner].HasPokemonInPlay)
                return PlayerName(winner) + " took the lead in Prize cards in the Tie-Breaker game.";
            return PlayerName(1 - winner) + (PlayerName(1 - winner) == "You" ? " have" : " has") + " no Pokémon left in play.";
        }

        /// <summary>Pinsir's Vise Coach (v58): something in play makes this attack damage ignore the target's Resistance.</summary>
        /// <summary>v142: a timed effect lets this damage ignore Resistance (Magnifier).</summary>
        private bool EffectIgnoresResistance(DamageInfo info)
        {
            foreach (var e in ActiveEffects())
                if (e.IgnoresResistance(this, info)) return true;
            return false;
        }

        private static bool ResistanceIgnored(List<PassiveSource> sources, DamageInfo info)
        {
            foreach (var src in sources)
                if (src.Script.IgnoresResistance(src.Ctx, info)) return true;
            return false;
        }

        /// <summary>"Take a Prize card" outside a Knock Out (Onyx). Checks whether that wins the game.</summary>
        public void TakePrizeCards(int player, int count)
        {
            TakePrizes(player, count);
            CheckWinConditions();
        }

        /// <summary>
        /// Billowing Smoke (v64): the player discards Prize cards instead of putting them into their hand. They don't count as
        /// taken (Compendium: Revenge Blast; Greedy Dice can't trigger), but discarding the last one still wins (your ruling).
        /// </summary>
        private void DiscardPrizes(int player, int count)
        {
            var p = State.Players[player];
            for (int k = 0; k < count && p.Prizes.Count > 0; k++)
            {
                var card = ChoosePrizeToTake(player, count - k, count);
                p.Prizes.Remove(card);
                p.FaceUpPrizeIds.Remove(card.Id);
                p.PrizeSlotByCard.Remove(card.Id);
                p.PrizesDiscarded++;
                p.Discard.Add(card);
                Emit(GameEventType.Info, player, PlayerName(player) + " discarded a Prize card (" + card.Def.Name + ") because of Billowing Smoke (" + p.Prizes.Count + " left).", card.Id);
            }
        }

        private void TakePrizes(int player, int count)
        {
            var p = State.Players[player];
            for (int k = 0; k < count && p.Prizes.Count > 0; k++)
            {
                var card = ChoosePrizeToTake(player, count - k, count);
                bool wasFaceDown = !p.FaceUpPrizeIds.Contains(card.Id);
                p.Prizes.Remove(card);
                p.FaceUpPrizeIds.Remove(card.Id);
                p.PrizeSlotByCard.Remove(card.Id);
                if (p.PrizesTakenTurn != State.TurnNumber) p.PrizesTakenThatTurn = 0;
                p.PrizesTakenTurn = State.TurnNumber;
                p.PrizesTakenThatTurn++;
                // No card id: which card was taken is hidden from the opponent.
                Emit(GameEventType.TakePrize, player, PlayerName(player) + " took a Prize card (" + p.Prizes.Count + " left).");
                // Barbaracle's Lost Block (v51): "Your opponent puts any Prize cards they would take in the Lost Zone instead of into
                // their hand." Once Barbaracle is Knocked Out it's gone, so the Prize cards for it go to the hand (Compendium).
                if (PrizesGoToLostZone(player))
                {
                    p.LostZone.Add(card);
                    Emit(GameEventType.Info, player, PlayerName(player) + " put the Prize card (" + card.Def.Name + ") in the Lost Zone instead of " + Their(player) + " hand.", card.Id);
                    continue;
                }
                // Chansey's Lucky Bonus: a face-down Prize taken during your turn may go onto the Bench instead of the hand.
                // It isn't in play yet, so Ability blockers in play don't stop it, and an extra Prize that is another Chansey can
                // use it too (your rulings).
                int extra = -1;
                // Dream Ball and Treasure Energy (v58): Trainer and Energy cards that can be used as they're taken, on any turn
                // (the card's script checks its own timing).
                bool usable = card.Def.IsPokemon ? State.CurrentPlayer == player && BenchHasRoom(player)
                                                 : p.Prizes.Count > 0 && Scripts.For(card.Def).UsableWhenTakenAsPrize;
                if (wasFaceDown && usable && !State.InSetup)
                {
                    PushSource(EffectSourceKind.Ability, player, null);
                    try { extra = Scripts.For(card.Def).OnTakenAsPrize(new EffectContext(this, player, card, null)); }
                    finally { PopSource(); }
                }
                if (extra < 0) p.Hand.Add(card);
                else count += extra;
            }
        }

        /// <summary>
        /// Which Prize card the player takes next (left = how many they still take, this one included). The person on screen
        /// picks any of them by its place on the table, unless the game is over: these are their last Prize cards, or the
        /// opponent has no Pokémon left in play. Then they're taken at once (your request, v42). Other seats take the first
        /// face-down card, or pick between face-up cards (Crescent Purge) and a face-down one.
        /// </summary>
        private CardInstance ChoosePrizeToTake(int player, int left, int total)
        {
            var p = State.Players[player];
            var faceUp = p.Prizes.Where(c => p.FaceUpPrizeIds.Contains(c.Id)).ToList();
            var faceDown = p.Prizes.Where(c => !p.FaceUpPrizeIds.Contains(c.Id)).ToList();
            // The game is over: these are the last Prize cards, or the opponent has no Pokémon left in play.
            bool gameOver = left >= p.Prizes.Count || !State.Players[1 - player].HasPokemonInPlay;
            if (gameOver || p.Prizes.Count == 1) return faceDown.Count > 0 ? faceDown[0] : p.Prizes[0];
            if (IsInteractive(player))
            {
                var slots = p.PrizeSlotList();
                var order = Enumerable.Range(0, p.Prizes.Count).OrderBy(n => slots[n]).ToList();
                var opts = new List<DecisionOption>();
                foreach (int i in order)
                {
                    var prize = p.Prizes[i];
                    bool up = p.FaceUpPrizeIds.Contains(prize.Id);
                    opts.Add(new DecisionOption
                    {
                        Label = up ? "Take the face-up " + prize.Def.Name + " (Prize " + (slots[i] + 1) + ")" : "Take Prize card " + (slots[i] + 1),
                        Index = slots[i],
                        CardId = up ? prize.Id : -1,
                        Card = up ? prize.Def : null
                    });
                }
                string prompt = total > 1 ? "Choose a Prize card to take (" + (total - left + 1) + " of " + total + ")" : "Choose a Prize card to take";
                int pick = AskOne(player, DecisionKind.ChoosePrize, prompt, opts, null, false);
                return p.Prizes[order[pick]];
            }
            // With face-up Prize cards (Crescent Purge) the player picks: a face-up card or a face-down one.
            if (faceUp.Count > 0)
            {
                var opts = faceUp.Select(c => new DecisionOption { Label = "Take the face-up " + c.Def.Name, CardId = c.Id, Card = c.Def }).ToList();
                if (faceDown.Count > 0) opts.Add(new DecisionOption { Label = "Take a face-down Prize card" });
                int pick = AskOne(player, DecisionKind.ChooseOption, "Choose a Prize card to take", opts, null, true);
                return pick < faceUp.Count ? faceUp[pick] : faceDown[0];
            }
            return faceDown[0];
        }

        public void PromoteActive(int player)
        {
            PromoteActive(player, false);
        }

        /// <summary>
        /// afterKnockOut: the Active Pokémon was just Knocked Out, so the other player's Hypnotic Pendulum (v73) may let them choose
        /// the new Active Pokémon instead.
        /// </summary>
        public void PromoteActive(int player, bool afterKnockOut)
        {
            var p = State.Players[player];
            int chooser = player;
            if (afterKnockOut)
                foreach (var src in Sources())
                {
                    if (src.Ctx.Player == player) continue;
                    bool takes;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { takes = src.Script.ChoosesOpponentsNewActive(src.Ctx, player); }
                    finally { PopSource(); }
                    if (takes) { chooser = 1 - player; break; }
                }
            var mon = chooser == player
                ? ChoosePokemon(player, p.Bench, "Choose your new Active Pokémon", null, DecisionKind.PromoteActive)
                : ChoosePokemon(chooser, p.Bench, "Choose your opponent's new Active Pokémon", null);
            p.Bench.Remove(mon);
            p.Active = mon;
            mon.MovedToActiveOnTurn = State.TurnNumber;
            mon.SpotSeq = NextLockSeq();
            Emit(GameEventType.Promote, player, PlayerName(player) + " promoted " + mon.Name + " to the Active Spot.", mon.Top.Id, mon.Uid);
            Promoting = true;
            try { MovedToActiveFromBench(player, mon); }
            finally { Promoting = false; }
        }

        /// <summary>True while a promotion's "moves from the Bench to the Active Spot" triggers run (Weavile's Assaulting Hunt can't be used then).</summary>
        public bool Promoting { get; private set; }

        public void EndGame(int winner, string reason)
        {
            State.IsOver = true;
            State.Winner = winner;
            State.EndReason = reason;
            Emit(GameEventType.GameOver, winner, winner < 0 ? "Game over: " + reason : PlayerName(winner) + " wins! " + reason);
            throw new GameOverException();
        }

        // ------------------------------------------------------------------ Board upkeep

        /// <summary>
        /// Things the rules fix right away whenever they happen: Energy that can't stay attached
        /// (Team Rocket's Energy), Special Conditions that can't apply (Festival Grounds) and Bench size
        /// over the limit (Area Zero Underdepths leaving or a player losing their last Tera Pokémon).
        /// </summary>
        public void StateChecks()
        {
            if (State.InSetup || State.IsOver) return;
            PokemonStateChecks();
            if (State.IsOver) return;
            PushSource(EffectSourceKind.Rule, -1, null);
            try
            {
                foreach (var p in State.Players)
                {
                    foreach (var mon in p.InPlay().ToList())
                    {
                        foreach (var e in mon.Energy.ToList())
                        {
                            if (!e.Def.IsSpecialEnergy) continue;
                            // Temple of Sinnoh: the Energy's text is off, so it isn't discarded while the Stadium stays (Compendium).
                            if (SpecialEnergyOff(mon)) continue;
                            if (Scripts.For(e.Def).MustDiscard(new EffectContext(this, mon.Owner, e, mon))) DiscardEnergy(mon, e);
                        }
                        // v168: a Pokémon Tool that's discarded when its Pokémon no longer fits (Unseen Forces' Curse Powder, Energy Root...).
                        foreach (var tool in mon.Tools.ToList())
                            if (Scripts.For(tool.Def).MustDiscard(new EffectContext(this, mon.Owner, tool, mon))) DiscardTool(mon, tool);
                        if (mon.Conditions != SpecialCondition.None && SpecialConditionsPrevented(mon)) RemoveAllConditions(mon);
                        else if (mon.Conditions != SpecialCondition.None) CurePreventedConditions(mon);
                    }
                }
            }
            finally
            {
                PopSource();
            }
            EnforceBenchLimits(State.CurrentPlayer);
            EnforceToolLimits(State.CurrentPlayer);
        }

        /// <summary>
        /// A Pokémon with more Tools than it can have (Rotom ex's Multi Adapter stopped working: Rotom ex left play or
        /// its Ability is blocked) has its owner discard Tools from it until it's at the limit.
        /// </summary>
        public void EnforceToolLimits(int firstPlayer)
        {
            for (int k = 0; k < 2; k++)
            {
                int i = k == 0 ? firstPlayer : 1 - firstPlayer;
                if (i < 0 || i > 1) continue;
                foreach (var mon in State.Players[i].InPlay().ToList())
                {
                    int guard = 0;
                    while (PokemonToolCount(mon) > 1 && PokemonToolCount(mon) > MaxTools(mon) && guard++ < 10)
                    {
                        int limit = MaxTools(mon);
                        var picks = ChooseCards(i, mon.Tools.Where(t => !Scripts.For(t.Def).IsAttachedNonTool).ToList(), 1, 1, mon.Name + " can only have " + limit + (limit == 1 ? " Pokémon Tool" : " Pokémon Tools") + " attached. Choose a Tool to discard", null);
                        DiscardTool(mon, picks.Count > 0 ? picks[0] : mon.Tools[mon.Tools.Count - 1]);
                    }
                }
            }
        }

        /// <summary>Each player over their Bench limit discards Benched Pokémon (their choice) until they're at it.</summary>
        public void EnforceBenchLimits(int firstPlayer)
        {
            for (int k = 0; k < 2; k++)
            {
                int i = k == 0 ? firstPlayer : 1 - firstPlayer;
                if (i < 0 || i > 1) continue;
                var p = State.Players[i];
                int guard = 0;
                while (p.Bench.Count > MaxBench(i) && guard++ < 10)
                {
                    var mon = ChoosePokemon(i, p.Bench.ToList(), "Your Bench is over its limit of " + MaxBench(i) + ". Choose a Benched Pokémon to discard", null);
                    DiscardPokemonFromPlay(mon);
                }
            }
        }
    }
}
