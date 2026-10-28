using System;
using System.Collections.Generic;

namespace PokemonTCG.Core
{
    /// <summary>
    /// An effect that lasts for a while: "during this turn", "during your opponent's next turn",
    /// "during your next turn". Effects are immutable, so cloned game states can share them.
    ///
    /// Expiry: an effect is removed at the start of any turn after ExpiresAfterTurn.
    ///   during this turn                 -> ExpiresAfterTurn = current turn
    ///   during your opponent's next turn -> current turn + 1
    ///   during your next turn            -> current turn + 2
    /// </summary>
    public class ContinuousEffect
    {
        public readonly string Name;
        /// <summary>Player whose card created the effect.</summary>
        public readonly int Owner;
        /// <summary>Pokémon the effect is on, or -1 for effects on a player or the whole game.</summary>
        public readonly int TargetUid;
        public int ExpiresAfterTurn { get; private set; }
        /// <summary>Effects of attacks placed on a Pokémon end when it moves to the Bench, evolves or leaves play.</summary>
        public readonly bool EndsWhenTargetMoves;
        /// <summary>
        /// First turn the effect applies ("during your next turn" effects start 2 turns later). Set it before
        /// the effect is added; effects aren't changed after that.
        /// </summary>
        public int ActiveFromTurn;
        /// <summary>
        /// Created by an attack (set by GameEngine.AddEffect, v92): Pokémon Ranger removes "all effects of attacks on each player and
        /// their Pokémon". Special Conditions aren't effects of this kind (Compendium).
        /// </summary>
        public bool FromAttack;

        public ContinuousEffect(string name, int owner, int targetUid, int expiresAfterTurn, bool endsWhenTargetMoves)
        {
            Name = name;
            Owner = owner;
            TargetUid = targetUid;
            ExpiresAfterTurn = expiresAfterTurn;
            EndsWhenTargetMoves = endsWhenTargetMoves;
        }

        /// <summary>
        /// A copy that skips a turn nobody plays (v49, Star Chronos: "Take another turn after this one"): the opponent's turn
        /// between the two turns doesn't happen, so an effect meant for it ("during your opponent's next turn") covers the
        /// opponent's real next turn, 2 turns later, instead.
        /// </summary>
        public ContinuousEffect PostponedPast(int skippedTurn)
        {
            var c = (ContinuousEffect)MemberwiseClone();
            if (c.ExpiresAfterTurn == skippedTurn) c.ExpiresAfterTurn = skippedTurn + 2;
            if (c.ActiveFromTurn == skippedTurn) c.ActiveFromTurn = skippedTurn + 2;
            return c;
        }

        /// <summary>Attacker-side damage changes, applied before Weakness and Resistance.</summary>
        public virtual int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage) { return damage; }
        /// <summary>Defender-side damage changes, applied after Weakness and Resistance.</summary>
        public virtual int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage) { return damage; }
        /// <summary>Psyduck's Overthink: "whenever they flip a coin, treat it as tails" for this player's flips right now.</summary>
        public virtual bool ForcesTails(GameEngine engine, int player) { return false; }
        /// <summary>Luvdisc's Heart Wink (v104): that player doesn't draw a card at the beginning of their turn (an empty deck still loses).</summary>
        public virtual bool SkipsTurnDraw(GameEngine engine, int player) { return false; }
        /// <summary>v136: after this player draws for their turn, their turn ends (Dialga LV.X's Time Skip).</summary>
        public virtual bool EndsTurnAfterDraw(GameEngine engine, int player) { return false; }
        /// <summary>Arceus's Type Switch (v107): changes a Pokémon's types.</summary>
        public virtual void ModifyTypes(GameEngine engine, PokemonInPlay mon, List<EnergyType> types) { }
        /// <summary>Checked after every damage reduction: return true to prevent all of it ("if that damage is 60 or less").</summary>
        public virtual bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage) { return false; }
        public virtual bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind) { return false; }
        public virtual bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return false; }
        /// <summary>v139: the Active Pokémon's owner is trying to retreat it, before anything is paid (Umbreon's Pursuit).</summary>
        public virtual void OnRetreatTried(GameEngine engine, PokemonInPlay mon) { }
        /// <summary>v142: don't apply Resistance to this damage (Magnifier: the Pokémon it's attached to attacks).</summary>
        public virtual bool IgnoresResistance(GameEngine engine, DamageInfo info) { return false; }
        /// <summary>v141: an Energy card was just attached to a Pokémon, from the hand or by any effect (Crobat's Triggered Poison).</summary>
        public virtual void OnEnergyAttached(GameEngine engine, CardInstance energy, PokemonInPlay target) { }
        /// <summary>
        /// v138, Croconaw's Jaw Clamp and Gloom's Sticky Nectar: "if the effect of an attack, Pokémon Power, or Trainer card would
        /// change that player's Active Pokémon, that part of the effect does nothing".
        /// </summary>
        public virtual bool LocksActiveSpot(GameEngine engine, int player) { return false; }
        /// <summary>v138, Bayleef's Pollen Shield: the Pokémon can't get this Special Condition.</summary>
        public virtual bool PreventsCondition(GameEngine engine, PokemonInPlay mon, SpecialCondition condition) { return false; }
        public virtual bool PreventsAttacks(GameEngine engine, PokemonInPlay mon) { return false; }
        public virtual bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string attackName) { return false; }
        /// <summary>
        /// A player-wide effect (TargetUid -1) that is on this player or all of their Pokémon, other than the restrictions Channeler
        /// already finds (Item lock, attack locks...): Altered Creation-GX's bonus and extra Prize are on the GX user, so only
        /// that player's Channeler removes them (Compendium, 2020-01-09).
        /// </summary>
        public virtual bool IsOnPlayer(int player) { return false; }
        public virtual void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage) { }
        /// <summary>Phoebe (v60): this attack damage isn't affected by any effects on the opponent's Active Pokémon.</summary>
        public virtual bool IgnoresDefenderEffects(GameEngine engine, DamageInfo info) { return false; }
        public virtual int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes) { return prizes; }
        /// <summary>Quaking Fist style: the player must flip before using a Trainer card.</summary>
        public virtual bool ForcesTrainerFlip(GameEngine engine, int player) { return false; }
        /// <summary>Scream (Scream Tail ex): the player can't play this Trainer card from their hand.</summary>
        public virtual bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return false; }
        /// <summary>The player can't play this Pokémon card from their hand, onto the Bench or to evolve (Horror House-GX, v80).</summary>
        public virtual bool BlocksPlayingPokemon(GameEngine engine, int player, CardDef card) { return false; }
        /// <summary>Ariados's Trapping Thread (v81): what this Trainer card, played from the hand, does to the Pokémon is prevented.</summary>
        public virtual bool PreventsTrainerEffects(GameEngine engine, PokemonInPlay target, CardDef trainer) { return false; }
        /// <summary>Changes the Energy an attached card provides while this effect lasts (Typhlosion's Blazing Energy, v81).</summary>
        public virtual void ModifyProvidedEnergy(GameEngine engine, CardInstance energy, PokemonInPlay holder, List<int> units) { }
        /// <summary>Sand Attack: "if the Defending Pokémon tries to use an attack, your opponent flips a coin. If tails, that attack doesn't happen."</summary>
        public virtual bool ForcesAttackFlip(GameEngine engine, PokemonInPlay attacker) { return false; }
        /// <summary>Coins the attacker must flip to attack; any tails stops the attack (Jet of Ink: 2). Defaults to 1 when ForcesAttackFlip.</summary>
        public virtual int AttackFlipCoins(GameEngine engine, PokemonInPlay attacker) { return ForcesAttackFlip(engine, attacker) ? 1 : 0; }
        /// <summary>Drum Beating: attacks of the Pokémon cost more.</summary>
        public virtual void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost) { }
        /// <summary>Drum Beating: the Pokémon's Retreat Cost is more.</summary>
        public virtual int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost) { return cost; }
        /// <summary>"At the end of your opponent's next turn" (Slow Crunch, Permeating Chill): runs at the end of every turn the effect is in force, before Pokémon Checkup.</summary>
        public virtual void OnEndOfTurn(GameEngine engine) { }
        /// <summary>Daydream (Hypno): a player attached an Energy card from their hand.</summary>
        public virtual void OnEnergyAttachedFromHand(GameEngine engine, CardInstance energy, PokemonInPlay target) { }
        /// <summary>Metal Defender (no Weakness), Now You're in My Power (Weakness is Colorless).</summary>
        public virtual EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness) { return weakness; }
        /// <summary>Cool Porygon's Texture Magic: the Pokémon's Resistance is another type.</summary>
        public virtual EnergyType? ModifyResistance(GameEngine engine, PokemonInPlay mon, EnergyType? resistance) { return resistance; }
        /// <summary>Corrosive Shards: Energy cards can't be attached from the hand to this Pokémon.</summary>
        public virtual bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target) { return false; }
        /// <summary>Dominating Echo (Noivern ex): the player can't play this Energy card from their hand (their attachment for the turn).</summary>
        public virtual bool BlocksEnergyCard(GameEngine engine, int player, CardInstance card) { return false; }
        /// <summary>Tsareena's Queenly Heel: Pokémon can't be played from the hand to evolve this Pokémon (Rare Candy included).</summary>
        public virtual bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return false; }
        /// <summary>"If this Pokémon has full HP and would be Knocked Out by damage from an attack, it is not Knocked Out and its remaining HP becomes 10" (Gritty Claws, v50).</summary>
        public virtual bool SurvivesKnockOutFromFullHp(GameEngine engine, PokemonInPlay mon) { return false; }
        /// <summary>"If it would be Knocked Out by damage from an opponent's attack, it is not Knocked Out and its remaining HP becomes 10", at any HP (Acme of Heroism-GX, v73).</summary>
        public virtual bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay mon) { return false; }
        /// <summary>
        /// Ancient Star's gained Ability (v51): this Ability of the Pokémon doesn't exist right now. depth is the Ability-check
        /// depth (GameEngine.AbilityBlockedAtDepth); checks of other Abilities from here use depth + 1.
        /// </summary>
        public virtual bool BlocksAbility(GameEngine engine, PokemonInPlay mon, string abilityName, int depth) { return false; }
        /// <summary>Greninja's Shadow Stitching (v96): this player's Pokémon cards in the hand and discard pile have no Abilities.</summary>
        public virtual bool BlocksAbilitiesOutOfPlay(GameEngine engine, int player, CardDef card) { return false; }
        /// <summary>
        /// Stops an old Pokémon Power of this Pokémon (Team Rocket's Goop Gas Attack and Dark Arbok's Stare, v121). Only Pokémon
        /// Powers: Poké-Powers, Poké-Bodies and Abilities aren't stopped (your ruling, like Toxic Gas).
        /// </summary>
        public virtual bool StopsPokemonPower(GameEngine engine, PokemonInPlay mon, string powerName) { return false; }
        /// <summary>v146: stops a Poké-Power of this Pokémon (Aquapolis Houndoom's Dark Impact). Only Poké-Powers: Poké-Bodies, old Pokémon Powers and Abilities aren't stopped.</summary>
        public virtual bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return false; }
        /// <summary>v147: stops a Poké-Body of this Pokémon (Skyridge Aerodactyl's Ancient Wind: all Poké-Bodies for the rest of the turn).</summary>
        public virtual bool StopsPokeBody(GameEngine engine, PokemonInPlay mon, string bodyName) { return false; }
        /// <summary>v122: attacks this Pokémon can use (Recall: the attacks of the cards under it). Card and attack index.</summary>
        public virtual IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(GameEngine engine, PokemonInPlay mon) { yield break; }
        /// <summary>v122: the attacking player may flip this Pokémon's attack coins again, once (Sabrina's ESP).</summary>
        public virtual bool OffersAttackReflip(GameEngine engine, PokemonInPlay attacker) { return false; }
        /// <summary>The name of an Ability this effect gives the Pokémon (Ancient Star), or null. It counts as a Pokémon with an Ability.</summary>
        public virtual string GainedAbility(GameEngine engine, PokemonInPlay mon) { return null; }
        /// <summary>v85: the Pokémon can't be healed while this lasts (Lunala-GX's Moongeist Beam).</summary>
        public virtual bool BlocksHealing(GameEngine engine, PokemonInPlay mon) { return false; }
        /// <summary>"This Pokémon can't be affected by any Special Conditions" for a while (Goodra's Shining Breath, v98).</summary>
        public virtual bool PreventsSpecialConditions(GameEngine engine, PokemonInPlay mon) { return false; }
        /// <summary>Dragonair's Dragon's Wish (v90): the player may attach any number of Energy cards from their hand this turn.</summary>
        public virtual bool AllowsAnyNumberOfEnergyAttachments(GameEngine engine, int player) { return false; }
        /// <summary>Like CardScript.LostZoneOnKnockOut, for a timed mark (Alolan Marowak-GX's Lost Boomerang-GX, v91): 0, 1 or 2.</summary>
        public virtual int LostZoneOnKnockOut(GameEngine engine, PokemonInPlay mon) { return 0; }
        /// <summary>Shiftry's Wicked Wind (v92): every Stadium and Pokémon Tool card in play has no effect while this lasts.</summary>
        public virtual bool SwitchesOffStadiumAndTools(GameEngine engine) { return false; }
        /// <summary>v126, Koga's Ninja Trick: an attack was announced (after the checks that could stop it, before its damage and effects).</summary>
        public virtual void OnAttackStarting(GameEngine engine, PokemonInPlay attacker, AttackDef attack) { }
        /// <summary>v126, Brock's Protection: the opponent's attacks and Trainer cards can't remove this Energy from the Pokémon.</summary>
        public virtual bool ProtectsEnergy(GameEngine engine, PokemonInPlay holder, CardInstance energy) { return false; }
        /// <summary>v126, Giovanni: this Pokémon may evolve this turn even if it was just played or evolved, or on the first turn.</summary>
        public virtual bool AllowsSameTurnEvolution(GameEngine engine, PokemonInPlay target, CardDef evolution) { return false; }
        /// <summary>
        /// v126, Blaine: after the player's Energy attachment for the turn (first, to target), the Energy cards they may attach to the
        /// same Pokémon as well, or null.
        /// </summary>
        public virtual Func<CardDef, bool> SecondEnergyAttachment(GameEngine engine, int player, PokemonInPlay target, CardInstance first) { return null; }
        /// <summary>v126, Koga's Ditto's Giant Growth: the Pokémon's maximum HP.</summary>
        public virtual int ModifyMaxHp(GameEngine engine, PokemonInPlay mon, int hp) { return hp; }
    }

    /// <summary>"During your next turn, attacks used by this Pokémon do N more damage to your opponent's Active Pokémon" (Wind Power Charge, Slumbering Smack).</summary>
    public sealed class SelfAttackBonusEffect : ContinuousEffect
    {
        private readonly int bonus;

        /// <summary>Starts on the attacker's next turn (current turn + 2) and lasts that turn.</summary>
        public SelfAttackBonusEffect(string name, int owner, int targetUid, int currentTurn, int bonus)
            : base(name, owner, targetUid, currentTurn + 2, true)
        {
            this.bonus = bonus;
            ActiveFromTurn = currentTurn + 2;
        }

        public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
        {
            if (attacker == null || attacker.Uid != TargetUid || target == null || target.Owner == Owner) return damage;
            return engine.State.IsActive(target) ? damage + bonus : damage;
        }
    }

    /// <summary>"The Defending Pokémon can't retreat."</summary>
    public sealed class CantRetreatEffect : ContinuousEffect
    {
        public CantRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn)
            : base(name, owner, targetUid, expiresAfterTurn, true) { }

        public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon.Uid == TargetUid; }
    }

    /// <summary>"During your next turn, this Pokémon can't attack."</summary>
    public sealed class CantAttackEffect : ContinuousEffect
    {
        public CantAttackEffect(string name, int owner, int targetUid, int expiresAfterTurn)
            : base(name, owner, targetUid, expiresAfterTurn, true) { }

        public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon) { return mon.Uid == TargetUid; }
    }

    /// <summary>"This Pokémon can't use [attack]" (Brave Slash, Mega Brave, Torment...).</summary>
    public sealed class CantUseAttackEffect : ContinuousEffect
    {
        private readonly string attackName;

        public CantUseAttackEffect(string name, int owner, int targetUid, int expiresAfterTurn, string attackName)
            : base(name, owner, targetUid, expiresAfterTurn, true)
        {
            this.attackName = attackName;
        }

        public override bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string name)
        {
            return mon.Uid == TargetUid && name == attackName;
        }
    }

    /// <summary>"Attacks used by your Pokémon do X more damage to your opponent's Active Pokémon (ex)."</summary>
    public sealed class DamageBonusEffect : ContinuousEffect
    {
        private readonly int bonus;
        private readonly bool onlyAgainstEx;

        public DamageBonusEffect(string name, int owner, int expiresAfterTurn, int bonus, bool onlyAgainstEx)
            : base(name, owner, -1, expiresAfterTurn, false)
        {
            this.bonus = bonus;
            this.onlyAgainstEx = onlyAgainstEx;
        }

        public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
        {
            if (attacker == null || attacker.Owner != Owner || target.Owner == Owner) return damage;
            if (!engine.State.IsActive(target)) return damage;
            if (onlyAgainstEx && !(target.Def.IsEx || target.Def.IsMegaEx)) return damage;
            return damage + bonus;
        }
    }

    /// <summary>Attacker-side bonus with a custom condition (Kieran, Premium Power Pro, Gladion's Final Battle...).</summary>
    public sealed class ConditionalDamageBonusEffect : ContinuousEffect
    {
        private readonly int bonus;
        private readonly Func<GameEngine, PokemonInPlay, PokemonInPlay, bool> condition;

        /// <param name="condition">(engine, attacker, target) -> applies. Only checked for your attacks against the opponent's Active Pokémon.</param>
        public ConditionalDamageBonusEffect(string name, int owner, int expiresAfterTurn, int bonus, Func<GameEngine, PokemonInPlay, PokemonInPlay, bool> condition)
            : base(name, owner, -1, expiresAfterTurn, false)
        {
            this.bonus = bonus;
            this.condition = condition;
        }

        public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
        {
            if (attacker == null || attacker.Owner != Owner || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
            return condition(engine, attacker, target) ? damage + bonus : damage;
        }
    }

    /// <summary>"Attacks used by the Defending Pokémon do X less damage (before Weakness and Resistance)."</summary>
    public sealed class WeakenAttackerEffect : ContinuousEffect
    {
        private readonly int amount;

        public WeakenAttackerEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount)
            : base(name, owner, targetUid, expiresAfterTurn, true)
        {
            this.amount = amount;
        }

        public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
        {
            return attacker != null && attacker.Uid == TargetUid ? damage - amount : damage;
        }
    }

    /// <summary>"This Pokémon takes X less (or more) damage from attacks (after Weakness and Resistance)."</summary>
    public sealed class DamageTakenEffect : ContinuousEffect
    {
        private readonly int delta;

        public DamageTakenEffect(string name, int owner, int targetUid, int expiresAfterTurn, int delta)
            : base(name, owner, targetUid, expiresAfterTurn, true)
        {
            this.delta = delta;
        }

        public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
        {
            return info.Target.Uid == TargetUid && info.Attacker != null ? damage + delta : damage;
        }
    }

    /// <summary>"Prevent all damage from and effects of attacks done to this Pokémon" (Dig, Fly, Splashing Dodge).</summary>
    public sealed class ProtectEffect : ContinuousEffect
    {
        public ProtectEffect(string name, int owner, int targetUid, int expiresAfterTurn)
            : base(name, owner, targetUid, expiresAfterTurn, true) { }

        public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
        {
            return info.Target.Uid == TargetUid && info.AgainstOpponent ? 0 : damage;
        }

        public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
        {
            return target.Uid == TargetUid && kind == EffectSourceKind.Attack;
        }
    }

    /// <summary>"If this Pokémon is damaged by an attack, put N damage counters on the Attacking Pokémon" (Ready to Ram).</summary>
    public sealed class RetaliateEffect : ContinuousEffect
    {
        private readonly int counters;

        public RetaliateEffect(string name, int owner, int targetUid, int expiresAfterTurn, int counters)
            : base(name, owner, targetUid, expiresAfterTurn, true)
        {
            this.counters = counters;
        }

        public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
        {
            if (info.Target.Uid == TargetUid && info.Attacker != null && info.AgainstOpponent)
                engine.PlaceDamageCounters(info.Attacker, counters, Name);
        }
    }

    /// <summary>"Take 1 more Prize card" when a condition is met (Unown, Briar).</summary>
    public sealed class ExtraPrizeEffect : ContinuousEffect
    {
        private readonly Func<GameEngine, PokemonInPlay, bool> condition;

        public ExtraPrizeEffect(string name, int owner, int expiresAfterTurn, Func<GameEngine, PokemonInPlay, bool> condition)
            : base(name, owner, -1, expiresAfterTurn, false)
        {
            this.condition = condition;
        }

        public override bool IsOnPlayer(int player) { return player == Owner; }

        public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
        {
            return taker == Owner && knockedOut.Owner != Owner && condition(engine, knockedOut) ? prizes + 1 : prizes;
        }
    }

    /// <summary>Quaking Fist: during the opponent's next turn, they flip before using each Trainer card.</summary>
    public sealed class TrainerFlipEffect : ContinuousEffect
    {
        private readonly int affectedPlayer;

        public TrainerFlipEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
            : base(name, owner, -1, expiresAfterTurn, false)
        {
            this.affectedPlayer = affectedPlayer;
        }

        public override bool ForcesTrainerFlip(GameEngine engine, int player)
        {
            return player == affectedPlayer && engine.State.CurrentPlayer == affectedPlayer;
        }
    }

    /// <summary>"During your opponent's next turn, prevent all damage done to this Pokémon by attacks" (Iron Defense). Effects still apply.</summary>
    public sealed class PreventDamageEffect : ContinuousEffect
    {
        public PreventDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn)
            : base(name, owner, targetUid, expiresAfterTurn, true) { }

        public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
        {
            return info.Target.Uid == TargetUid && info.AgainstOpponent ? 0 : damage;
        }
    }

    /// <summary>"During your next turn, if the Defending Pokémon is Knocked Out, take N more Prize cards" (Plentiful Pollen).</summary>
    public sealed class KnockOutPrizeBonusEffect : ContinuousEffect
    {
        private readonly int extra;

        public KnockOutPrizeBonusEffect(string name, int owner, int targetUid, int expiresAfterTurn, int extra)
            : base(name, owner, targetUid, expiresAfterTurn, true)
        {
            this.extra = extra;
        }

        public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
        {
            return knockedOut.Uid == TargetUid && taker == Owner ? prizes + extra : prizes;
        }
    }

    /// <summary>
    /// Which way a two-way Stadium faces (Chaos Tower, Reverse Valley, Parallel City; v97). Owner is the side the card's script
    /// keys on; FacedPlayer is the player the printed card is turned towards (its upright half pointing at them), which the
    /// board shows.
    /// </summary>
    public class StadiumFacingEffect : ContinuousEffect
    {
        public readonly int StadiumId;
        public readonly int FacedPlayer;

        public StadiumFacingEffect(string name, int owner, int stadiumId, int facedPlayer)
            : base(name, owner, -1, int.MaxValue, false)
        {
            StadiumId = stadiumId;
            FacedPlayer = facedPlayer;
        }
    }

    /// <summary>"During your next turn, this Pokémon's [attack] does N more damage" (Meteor Mash). The attack's script reads it.</summary>
    public sealed class AttackBoostEffect : ContinuousEffect
    {
        public readonly string AttackName;
        public readonly int Bonus;

        public AttackBoostEffect(string name, int owner, int targetUid, int expiresAfterTurn, string attackName, int bonus)
            : base(name, owner, targetUid, expiresAfterTurn, true)
        {
            AttackName = attackName;
            Bonus = bonus;
        }
    }
}
