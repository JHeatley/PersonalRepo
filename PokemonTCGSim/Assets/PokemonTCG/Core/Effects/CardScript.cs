using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Behaviour for one card. Cards with only plain attacks (damage and nothing else) need no script:
    /// the default implementation already handles them. Override only what the card text adds.
    ///
    /// "Passive" hooks (Modify..., Prevents..., Blocks...) are asked of every active source in play:
    /// each Pokémon's Ability (unless blocked), each attached Pokémon Tool (unless Tools are disabled),
    /// each attached Special Energy, and the Stadium. In those hooks ctx.Player is the source's owner,
    /// ctx.Card is the source card and ctx.Pokemon is the Pokémon it's on (null for a Stadium).
    /// </summary>
    public class CardScript
    {
        // ------------------------------------------------------------------ Trainers
        public virtual bool CanPlayTrainer(EffectContext ctx) { return true; }
        public virtual void ResolveTrainer(EffectContext ctx) { }
        /// <summary>Supporters like Carmine: "If you go first, you may use this card during your first turn."</summary>
        public virtual bool PlayableOnFirstTurn(EffectContext ctx) { return false; }
        public virtual void OnStadiumPlayed(EffectContext ctx) { }
        /// <summary>Stadium hook: a Pokémon was just put onto someone's Bench (ctx.Card is the Stadium).</summary>
        public virtual void OnPokemonPutOnBench(EffectContext ctx, PokemonInPlay benched) { }
        /// <summary>Stadium "once during each player's turn" actions. ctx.Player is the player who'd use it.</summary>
        public virtual bool CanUseStadium(EffectContext ctx) { return false; }
        /// <summary>Returns true if using the Stadium ends the turn (Lumiose City).</summary>
        public virtual bool UseStadium(EffectContext ctx) { return false; }
        /// <summary>Stadium hook: this Stadium was just discarded or replaced (ctx.Player is its owner).</summary>
        public virtual void OnLeavePlay(EffectContext ctx) { }

        // ------------------------------------------------------------------ Pokémon
        /// <summary>Abilities used while this card is in the hand (Klinklang's Emergency Rotation). ctx.Card is the card, ctx.Pokemon is null.</summary>
        public virtual bool CanUseFromHand(EffectContext ctx) { return false; }
        public virtual void UseFromHand(EffectContext ctx) { }
        /// <summary>Effects used while this card is in your discard pile (Grant, v49). ctx.Card is the card, ctx.Pokemon is null.</summary>
        public virtual bool CanUseFromDiscard(EffectContext ctx) { return false; }
        public virtual void UseFromDiscard(EffectContext ctx) { }
        /// <summary>Activated abilities only. Triggered abilities return false here and use a hook instead.</summary>
        public virtual bool CanUseAbility(EffectContext ctx, int abilityIndex) { return false; }
        public virtual void UseAbility(EffectContext ctx, int abilityIndex) { }
        /// <summary>Triggered when this card is played from hand onto the Bench during your turn.</summary>
        public virtual void OnPlayedFromHandToBench(EffectContext ctx) { }
        /// <summary>Triggered when this card is played from hand to evolve a Pokémon (ctx.Pokemon is the evolved Pokémon).</summary>
        public virtual void OnEvolvedFromHand(EffectContext ctx) { }
        /// <summary>Extra conditions on an attack beyond its Energy cost.</summary>
        public virtual bool CanUseAttack(EffectContext ctx, int attackIndex) { return true; }
        /// <summary>"If you go first, you can use this attack during your first turn."</summary>
        public virtual bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return false; }
        /// <summary>"You can use this attack even while this Pokémon is on the Bench" (Alakazam ex's Dimensional Hand).</summary>
        public virtual bool UsableFromBench(AttackDef attack) { return false; }
        /// <summary>The attack's own cost change (Gutsy Swing). ctx.Pokemon is the attacker; asked even without an Ability.</summary>
        public virtual void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost) { }
        /// <summary>"Put this Pokémon into play only with the effect of ..." (Palafin ex): it can't evolve from the hand or by other cards.</summary>
        public virtual bool PutIntoPlayOnlyByEffect { get { return false; } }
        /// <summary>Called on this Pokémon's own Ability when it moves from the Active Spot to the Bench during its owner's turn (Zero to Hero).</summary>
        public virtual void OnMovedToBenchFromActive(EffectContext ctx) { }
        /// <summary>Called on this Pokémon's own Ability when it moves from the Bench to the Active Spot during its owner's turn (Yanmega ex's Buzzing Boost).</summary>
        public virtual void OnMovedToActiveFromBench(EffectContext ctx) { }
        /// <summary>
        /// Called on this Pokémon's own Ability right after it retreats (it's on the Bench now): Imakuni?'s Doduo's Frenzied Escape
        /// (v92). Not called for other ways of moving to the Bench (switching effects).
        /// </summary>
        public virtual void OnRetreated(EffectContext ctx) { }
        /// <summary>
        /// v121: a Pokémon (either player's) has just retreated and is on the Bench now (Dark Dugtrio's Sinkhole). Called on every
        /// working source in play. Not called for switching effects.
        /// </summary>
        public virtual void OnPokemonRetreated(EffectContext src, PokemonInPlay retreated) { }
        /// <summary>
        /// v121: a Pokémon script without an Ability or Power whose own attack set up a Knock Out reaction (Base Set Gastly's Destiny
        /// Bond): the engine calls its OnHolderKnockedOut too.
        /// </summary>
        public virtual bool ReactsToOwnKnockOutWithoutAbility { get { return false; } }
        /// <summary>
        /// v122: an attack is about to resolve (after Confusion and other checks let it happen). Called on every working source in
        /// play (Vermilion City Gym asks the attacking player to flip).
        /// </summary>
        public virtual void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack) { }
        /// <summary>v122: an attack has finished resolving, its damage triggers included (Vermilion City Gym's tails damage).</summary>
        public virtual void OnAttackFinished(EffectContext src, PokemonInPlay attacker, AttackDef attack) { }
        /// <summary>v121: the damage counters a failed Confusion flip puts on this Pokémon (Dark Primeape's Frenzy: 3 more).</summary>
        public virtual int ModifyConfusionCounters(EffectContext src, PokemonInPlay mon, int counters) { return counters; }
        /// <summary>Psyduck's Damp: "Abilities that require the Pokémon using it to Knock Out itself".</summary>
        public virtual bool AbilityKnocksOutSelf(string abilityName) { return false; }

        /// <summary>Default attack: printed damage to the opponent's Active Pokémon.</summary>
        public virtual void ResolveAttack(EffectContext ctx, int attackIndex)
        {
            ctx.DamageDefending(ctx.Attack(attackIndex).Damage);
        }

        // ------------------------------------------------------------------ Energy cards
        /// <summary>Energy units this card provides while attached (ctx.Card is the Energy, ctx.Pokemon the holder).</summary>
        public virtual List<int> ProvidedEnergy(EffectContext ctx)
        {
            return new List<int> { EnergyMask.Of(ctx.Card.Def.EnergyProvides) };
        }

        public virtual bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return true; }
        /// <summary>Aurora Energy (v66): attaching this card from the hand, by any means, costs discarding another card from the hand.</summary>
        public virtual bool DiscardToAttachFromHand(EffectContext ctx) { return false; }
        /// <summary>Triggered when this Energy is attached from hand (ctx.Pokemon is the target).</summary>
        public virtual void OnAttachedFromHand(EffectContext ctx) { }
        /// <summary>Checked after every action: return true to discard this Energy (Team Rocket's Energy on another Pokémon).</summary>
        public virtual bool MustDiscard(EffectContext ctx) { return false; }
        /// <summary>Boomerang Energy: return true to re-attach this card after its holder's attack discarded it.</summary>
        public virtual bool ReturnsAfterAttackDiscard(EffectContext ctx) { return false; }

        // ------------------------------------------------------------------ Passive hooks
        public virtual int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return hp; }
        public virtual EnergyType? ModifyWeakness(EffectContext src, PokemonInPlay mon, EnergyType? weakness) { return weakness; }
        public virtual int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return cost; }
        /// <summary>How much a Pokémon's Resistance takes off (Klefki's Key of Secrets: "Each of your [M] Pokémon's Resistance is now -40", v80).</summary>
        public virtual int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount) { return amount; }
        public virtual void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost) { }
        /// <summary>Attacker side, before Weakness and Resistance.</summary>
        public virtual int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage) { return damage; }
        /// <summary>Defender side, after Weakness and Resistance. Return 0 to prevent the damage.</summary>
        public virtual int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage) { return damage; }
        /// <summary>Asked last, only when damage would still be done (Smooth Coat's coin flip). Return true to prevent it.</summary>
        public virtual bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage) { return false; }
        /// <summary>Prevent effects of the opponent's attacks/Abilities on a Pokémon (damage counters count as effects).</summary>
        public virtual bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters) { return false; }
        public virtual bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target) { return false; }
        /// <summary>Prevent effects of the other player's Trainer card on a Pokémon (Wide Wall: their Supporters). trainer is the card being played.</summary>
        public virtual bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer) { return false; }
        /// <summary>Stadium form (whole Pokémon): kept for Team Rocket's Watchtower style cards.</summary>
        public virtual bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon) { return false; }
        /// <summary>Block one named Ability of a Pokémon (Flutter Mane, Psyduck's Damp). Pokémon must also set IsAbilityBlocker.</summary>
        public virtual bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName) { return false; }
        /// <summary>True for Pokémon whose Ability can block other Abilities (keeps the Ability check fast).</summary>
        public virtual bool IsAbilityBlocker { get { return false; } }
        /// <summary>The Ability lock works only in one spot (Active or Bench), so moving there turns it on. False for locks that work anywhere in play.</summary>
        public virtual bool LockDependsOnSpot { get { return true; } }
        /// <summary>The Ability lock needs a Pokémon Tool on its Pokémon (Garbotoxin, Awakening), so getting one turns it on.</summary>
        public virtual bool LockDependsOnTool { get { return false; } }
        public virtual bool BlocksTrainer(EffectContext src, int player, CardInstance card) { return false; }
        public virtual bool BlocksHealing(EffectContext src, PokemonInPlay mon) { return false; }
        public virtual bool BlocksCounterMovement(EffectContext src) { return false; }
        /// <summary>Stadium: Pokémon Tools have no effect (Jamming Tower).</summary>
        public virtual bool DisablesTools(EffectContext src) { return false; }
        public virtual int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters) { return counters; }
        /// <summary>Damage counters Burn puts on a Pokémon in Pokémon Checkup (2 normally; Magmortar's Magma Surge adds 3).</summary>
        public virtual int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters) { return counters; }
        public virtual int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes) { return prizes; }
        /// <summary>v152.6: changes the amount a Pokémon is healed by (Legendary Abyss doubles it; with α Recovery too, ×4 by your ruling).</summary>
        public virtual int ModifyHealAmount(EffectContext src, PokemonInPlay target, int amount) { return amount; }
        /// <summary>Durable Body: return true to survive a Knock Out from attack damage at 10 HP.</summary>
        public virtual bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>Survival Brace: the holder had full HP and would be Knocked Out by damage from an opponent's attack.</summary>
        public virtual bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>After this card kept its Pokémon from being Knocked Out ("Then, discard this card").</summary>
        public virtual void OnSavedFromKnockOut(EffectContext src, PokemonInPlay mon) { }
        /// <summary>During Pokémon Checkup (Froslass's Freezing Shroud).</summary>
        public virtual void OnPokemonCheckup(EffectContext src) { }
        /// <summary>
        /// Whose Pokémon OnPokemonCheckup acts on: its owner's (the default), the opponent's (1 - src.Player) or both players'
        /// (-1), or -2 when it won't do anything now (a Pokémon that has to be Active isn't). When both players' effects act on one
        /// player's Pokémon, that player chooses their order (Compendium: Sand Slammer vs Royal Heal, 2012-11-01).
        /// </summary>
        public virtual int CheckupAffects(EffectContext src) { return src.Player; }
        /// <summary>Milotic's Mentally Calm: this Pokémon and its attached cards can't be put into its owner's hand.</summary>
        public virtual bool KeepsOutOfHand(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>Change the units an attached Energy provides (Meganium's Wild Growth).</summary>
        public virtual void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units) { }
        public virtual int ModifyMaxBench(EffectContext src, int player, int max) { return max; }
        /// <summary>Change how many Pokémon Tools this Pokémon can have attached (Rotom ex's Multi Adapter raises it to 2). Return at least max to stack with other effects.</summary>
        public virtual int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max) { return max; }
        public virtual bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution) { return false; }
        /// <summary>
        /// Δ Evolution (Ancient Trait, M Rayquaza-EX, v56): this Evolution card can be played from the hand to evolve a Pokémon
        /// during your first turn or the turn that Pokémon was played. Asked of the Evolution card's own script.
        /// </summary>
        public virtual bool EvolvesWithoutTimingLimits { get { return false; } }
        /// <summary>Rainbow DNA: this Pokémon (src.Pokemon) can evolve into a card that evolves from another Pokémon, when that card is played from the hand.</summary>
        public virtual bool AllowsEvolvingInto(EffectContext src, PokemonInPlay target, CardDef evolution) { return false; }
        /// <summary>Archeops's Ancient Power (v128): no Pokémon can be played from the hand to evolve this Pokémon (Rare Candy included).</summary>
        public virtual bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target) { return false; }
        /// <summary>Insomnia: one Special Condition can't affect the Pokémon.</summary>
        public virtual bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition) { return false; }
        /// <summary>Phione's Murmurs of the Sea (v82): a Pokémon already affected by a condition this prevents recovers from it.</summary>
        public virtual bool CuresConditionsItPrevents { get { return false; } }
        /// <summary>Called at the end of the source owner's turn, before Pokémon Checkup.</summary>
        public virtual void OnEndOfTurn(EffectContext src) { }
        /// <summary>Called at the end of every turn (either player's), after OnEndOfTurn and before Pokémon Checkup (Emergency Jelly, v50).</summary>
        public virtual void OnEndOfEachTurn(EffectContext src) { }
        /// <summary>Attacks this Pokémon can use besides its own (Mew ex's Memory Helix). Only asked of the Pokémon's own Ability.</summary>
        public virtual IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src) { return Enumerable.Empty<KeyValuePair<CardDef, int>>(); }
        /// <summary>Festival Lead: this Pokémon may attack twice.</summary>
        public virtual bool AttacksTwice(EffectContext src) { return false; }
        /// <summary>Tools, Special Energy and the Pokémon's own Ability: the holder was damaged by an opponent's attack.</summary>
        public virtual void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage) { }
        /// <summary>Tools, Special Energy and the Pokémon's own Ability: the holder is being Knocked Out (it's still in play). Heavy Baton.</summary>
        public virtual void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive) { }
        /// <summary>Tools and Special Energy: their holder was just evolved by a card played from the hand; before is the card it was (Regenerative Energy, v50).</summary>
        public virtual void OnHolderEvolvedFromHand(EffectContext src, CardDef before) { }
        /// <summary>Attacks another Pokémon can use because of this card (Relicanth's Memory Dive). Asked of every source.</summary>
        public virtual IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon) { return Enumerable.Empty<KeyValuePair<CardDef, int>>(); }
        /// <summary>Add types to a Pokémon (Iron Treads' Dual Core). types starts with the printed type.</summary>
        public virtual void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types) { }
        /// <summary>The other player's Active Pokémon just moved to the Bench during their own turn (Magcargo's Lava Zone).</summary>
        public virtual void OnOpponentActiveMovedToBench(EffectContext src, int player, PokemonInPlay newActive) { }
        /// <summary>"Whenever your opponent's Active Pokémon moves to the Bench during their turn, ... on that Pokémon" (Team Rocket's Dugtrio's Holes).</summary>
        public virtual void OnOpponentPokemonMovedToBench(EffectContext src, int player, PokemonInPlay movedToBench) { }
        /// <summary>The other player just attached an Energy card from their hand (Gengar ex's Gnawing Curse).</summary>
        public virtual void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { }
        /// <summary>Your own cards in play react when you attach an Energy card from your hand (Magearna's Auto Heal).</summary>
        public virtual void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { }
        /// <summary>
        /// v139: the source's owner just attached an Energy card to 1 of their Pokémon during their turn, from the hand or by any
        /// effect (Eevee's Energy Evolution). Called after the from-hand reactions.
        /// </summary>
        public virtual void OnOwnerAttachedEnergyCard(EffectContext src, CardInstance energy, PokemonInPlay target) { }
        /// <summary>v139: 1 of the other player's Benched Pokémon just became their Active Pokémon during their turn, by a switch or a promotion (Forretress's Spikes).</summary>
        public virtual void OnOpponentBenchedBecameActive(EffectContext src, int player, PokemonInPlay mon) { }
        /// <summary>
        /// v141: this Pokémon just moved from the Active Spot to the Bench, in any player's turn and by any means (Magcargo's Magma
        /// Pool); newActive is the Pokémon that replaced it.
        /// </summary>
        public virtual void OnLeftActiveSpot(EffectContext ctx, PokemonInPlay newActive) { }
        /// <summary>v141: a Pokémon Tool that's discarded instead of the Energy when its holder retreats (Balloon Berry).</summary>
        public virtual bool ReplacesRetreatCost(EffectContext src) { return false; }
        /// <summary>v141: cards in discard piles aren't affected by attacks or Pokémon Powers while this works (Parasect's Allergic Pollen).</summary>
        public virtual bool ShieldsDiscardPiles(EffectContext src) { return false; }
        /// <summary>
        /// v141: the Trainer card resolving now, whoever played it, does nothing to this Pokémon (Legendary Body: "aren't affected by
        /// effects from Trainer cards other than Stadium cards", the owner's own included by your ruling).
        /// </summary>
        public virtual bool PreventsAllTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer) { return false; }
        /// <summary>One of the source owner's Pokémon is being Knocked Out; it and its cards are still in play (Huntail's Diver's Catch).</summary>
        public virtual void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack) { }
        /// <summary>The other player just played a Pokémon from their hand to evolve 1 of their Pokémon (Team Rocket's Ampharos's Darkest Impulse).</summary>
        public virtual void OnOpponentEvolvedFromHand(EffectContext src, PokemonInPlay evolved) { }
        /// <summary>The player can't play this Pokémon card from their hand: onto the Bench or to evolve (Team Rocket's Arbok's Potent Glare).</summary>
        public virtual bool BlocksPlayingPokemon(EffectContext src, int player, CardDef card) { return false; }
        /// <summary>Another of the source owner's Pokémon moved from the Bench to the Active Spot during their turn (Latios's Lustrous Assist).</summary>
        public virtual void OnOwnPokemonMovedToActive(EffectContext src, PokemonInPlay mon) { }
        /// <summary>Another of the source owner's Pokémon was damaged by an attack from the opponent's Pokémon (Spiritomb's Spiteful Swirl).</summary>
        public virtual void OnOwnPokemonDamagedByAttack(EffectContext src, PokemonInPlay target, PokemonInPlay attacker, bool targetWasActive, int damage) { }
        /// <summary>This card may be put face down in the Active Spot during setup although it isn't a Basic Pokémon (Cinderace's Explosiveness).</summary>
        public virtual bool CanStartAsActive { get { return false; } }
        /// <summary>
        /// An opening hand with no Basic Pokémon but this card may be kept, putting this card down as the Active Pokémon, or
        /// mulliganed: the player chooses (Talonflame's Gale Wings, Compendium 2016-08-04; v93).
        /// </summary>
        public virtual bool MayKeepOpeningHandWithIt { get { return false; } }
        /// <summary>
        /// First Ticket (Dragon Vault, v124): played from the opening hand before the coin flip for who goes first. Only in a format
        /// that draws the hands before that flip (your ruling).
        /// </summary>
        public virtual bool PlayableBeforeCoinFlip { get { return false; } }
        /// <summary>
        /// Manectric's Electric Start (v83): a player going second may put this card face down as their Active Pokémon or on their
        /// Bench during setup, although it isn't a Basic Pokémon. It also lets them keep an opening hand with no Basic Pokémon
        /// (your ruling, v83).
        /// </summary>
        public virtual bool StartsFaceDownWhenGoingSecond { get { return false; } }
        /// <summary>The player must flip a coin to retreat; on tails the retreat doesn't happen and no Energy is discarded (Goodra's Slimy Sliding).</summary>
        public virtual bool ForcesRetreatFlip(EffectContext src, int player) { return false; }
        /// <summary>Confused Pokémon stay Confused when they evolve or devolve (Dizzying Valley).</summary>
        public virtual bool KeepsConfusionOnEvolve(EffectContext src) { return false; }
        /// <summary>Sea of Nothingness (v86): Special Conditions aren't removed when the Pokémon evolves or devolves.</summary>
        public virtual bool KeepsConditionsOnEvolve(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>This Poisoned Pokémon stays Poisoned (with the same damage counters) when it evolves or devolves (Muk's Poison Sacs).</summary>
        public virtual bool KeepsPoisonOnEvolve(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>v136: evolving or devolving doesn't remove Burned from this Pokémon (Heatran LV.X's Heat Metal).</summary>
        public virtual bool KeepsBurnOnEvolve(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// v136: a Pokémon script without an Ability whose printed text reacts to the Pokémon being damaged by an attack (Darkrai's
        /// Enigma Berry, a held item): the engine calls its OnHolderDamagedByAttack too.
        /// </summary>
        public virtual bool ReactsToOwnDamageWithoutAbility { get { return false; } }
        /// <summary>
        /// v136: ExtraAttacks may give attacks of cards with this Pokémon's own name (Arceus LV.X's Omniscient: another Arceus LV.X
        /// in play). Copies of an attack it already has are still left out.
        /// </summary>
        public virtual bool ExtraAttacksMayShareName { get { return false; } }
        /// <summary>
        /// v138, Slowking's Mind Games: the opponent (player) just played this Trainer card from their hand. Return true when it
        /// does nothing; the script has put the card where it goes. A Supporter stopped this way isn't the player's Supporter for
        /// the turn (your ruling).
        /// </summary>
        public virtual bool StopsOpponentsTrainer(EffectContext src, int player, CardInstance card) { return false; }
        /// <summary>
        /// v138, Ecogym (asked of the Stadium in play): this Energy card, discarded from that Pokémon by another player's attack,
        /// Pokémon Power or Trainer card, goes back to its owner's hand.
        /// </summary>
        public virtual bool ReturnsDiscardedEnergy(EffectContext src, PokemonInPlay from, CardInstance energy) { return false; }
        /// <summary>
        /// v138, Neo Genesis Berries (asked of Pokémon Tools): "at any time between turns" (at the end of Pokémon Checkup,
        /// startOfTurn false) and "at the start of each turn" (startOfTurn true, before the draw).
        /// </summary>
        public virtual void OnBetweenTurns(EffectContext src, bool startOfTurn) { }
        /// <summary>The Pokémon goes to its owner's hand instead of the discard pile when Knocked Out (Infinite Shadow). Attached cards are discarded.</summary>
        public virtual bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack) { return false; }
        /// <summary>Celebi's Leap Through Time (v107): "If this Pokémon would be Knocked Out" - true when it is shuffled into the deck instead (not Knocked Out).</summary>
        public virtual bool EscapesKnockOut(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>Malamar's Contrary (v107): every coin flip that player makes during their own turn is tails.</summary>
        public virtual bool ForcesTailsFor(EffectContext src, int player) { return false; }
        /// <summary>Team Plasma Badge (v110): the Pokémon this card is attached to is a Team Plasma Pokémon.</summary>
        public virtual bool MakesHolderTeamPlasma(EffectContext src) { return false; }
        /// <summary>This Stadium can be played even if a Stadium was already played this turn (Ange Floette).</summary>
        public virtual bool IgnoresStadiumLimit(EffectContext ctx) { return false; }
        /// <summary>This card was discarded from its owner's deck during the opponent's turn by the opponent's attack, Ability, Item or Supporter (Ferrothorn's Startling Drop). ctx.Card is the card.</summary>
        public virtual void OnDiscardedFromDeckByOpponent(EffectContext ctx) { }
        /// <summary>
        /// True for cards that react to being discarded from their owner's hand by the opponent's attack, Ability, Item or
        /// Supporter during the opponent's turn (Amoonguss's Surprise Spores, v50); the engine then calls
        /// OnDiscardedFromHandByOpponent once that effect is over. ctx.Card is the card, now in the discard pile.
        /// </summary>
        public virtual bool WatchesHandDiscardByOpponent { get { return false; } }
        public virtual void OnDiscardedFromHandByOpponent(EffectContext ctx) { }
        /// <summary>This card was just drawn for the turn and is the last card in the hand (Metagross's Emergency Entry, v50). ctx.Card is the card.</summary>
        public virtual void OnDrawnForTurn(EffectContext ctx) { }
        /// <summary>Backtrack Badge: once during your turn, the holder's attack coin flips may be ignored and flipped again.</summary>
        public virtual bool OffersAttackReflip(EffectContext src) { return false; }
        /// <summary>Pokémon Tool F (Head Ringer, Jamming Net; v102): played onto 1 of the opponent's Pokémon instead of your own.</summary>
        public virtual bool AttachesToOpponentsPokemon { get { return false; } }
        /// <summary>
        /// Victini's Victory Star (v89): an Ability that lets its owner ignore the coin flips of any of their own Pokémon's attacks and
        /// flip again. Returns the Ability's name (1 use of that name per turn for the player), or null.
        /// </summary>
        public virtual string OwnAttacksReflipAbility(EffectContext src) { return null; }
        /// <summary>Weakness multiplier for this damage to an Active Pokémon (2 normally; Illumise's Supereffective Pheromones: 3).</summary>
        public virtual int WeaknessMultiplier(EffectContext src, DamageInfo info) { return 2; }
        /// <summary>
        /// Lost Zone Knock Outs, asked of every source while the Knocked Out Pokémon is still in play: 0 = discard as usual,
        /// 1 = the Pokémon goes to the Lost Zone and its attached cards are discarded (Gengar's Catastrophe),
        /// 2 = the Pokémon and every card attached go to the Lost Zone (Lost Crisis).
        /// </summary>
        public virtual int LostZoneOnKnockOut(EffectContext src, PokemonInPlay mon) { return 0; }
        /// <summary>This Pokémon can't attack right now because of this source (Vanilluxe's Frigid Room).</summary>
        public virtual bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// Cards in this player's discard pile can't be put into their hand by the effect resolving now, which is one of their own
        /// Abilities or Trainer cards (Toedscruel's Slime Mold Colony). Asked only of the other player's sources.
        /// </summary>
        public virtual bool BlocksDiscardToHand(EffectContext src, int player) { return false; }
        /// <summary>
        /// Pokémon Tower (Stadium): while true, the effect resolving now can't put cards from a discard pile into their owner's
        /// hand. The engine asks only when that effect is a Pokémon Power, an attack, an Energy card or a Trainer card.
        /// </summary>
        public virtual bool KeepsDiscardPilesOutOfHands(EffectContext src) { return false; }
        /// <summary>
        /// Eevee's Chain Reaction: a Pokémon (either player's) has just evolved, in any way. Asked of every source after the
        /// evolution is done, while the source's Pokémon is still in play.
        /// </summary>
        public virtual void OnAnyPokemonEvolved(EffectContext src, PokemonInPlay evolved) { }
        /// <summary>Sandshrew's Sand Screen: Trainer cards in that player's discard pile can't be put into their deck by their own Item or Supporter cards.</summary>
        public virtual bool BlocksTrainerDiscardToDeck(EffectContext src, int player) { return false; }
        /// <summary>Ability-based retreat locks (Omastar's Primordial Tentacles): true when that Pokémon can't retreat.</summary>
        public virtual bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>Protective Goggles: "has no Weakness". Applied after every effect that changes or gives Weakness, so it always wins (your ruling).</summary>
        public virtual bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// Chansey's Lucky Bonus: this card was just taken as a face-down Prize card during its owner's turn and their Bench has room.
        /// Return -1 to put it into the hand as usual; otherwise the script has put it into play and returns how many more Prize
        /// cards to take.
        /// </summary>
        public virtual int OnTakenAsPrize(EffectContext ctx) { return -1; }
        /// <summary>
        /// Lunatone's New Moon: asked of an Ability source's own Pokémon. True while every effect of the Stadium (good or bad) is
        /// kept off its owner's Pokémon in play (your ruling). The Stadium still changes things that aren't on a Pokémon, such
        /// as Bench size.
        /// </summary>
        public virtual bool ShieldsOwnPokemonFromStadium(EffectContext ctx) { return false; }
        /// <summary>Patrol Cap: true while cards in this player's deck can't be discarded by the other player's attacks, Abilities or Trainer cards.</summary>
        public virtual bool ProtectsDeckFromDiscard(EffectContext src, int player) { return false; }
        /// <summary>Coins this Asleep Pokémon flips in Pokémon Checkup; it wakes up only if all are heads (Slaking's Stir and Snooze: 2).</summary>
        public virtual int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins) { return coins; }
        /// <summary>
        /// Barbaracle's Lost Block (v51): the player taking Prize cards (taker) puts them in their Lost Zone instead of their
        /// hand. Asked when each Prize card is taken, so a Barbaracle that was just Knocked Out no longer counts.
        /// </summary>
        public virtual bool SendsPrizesToLostZone(EffectContext src, int taker) { return false; }
        /// <summary>
        /// "Each player can't have more than N Benched Pokémon" (Collapsed Stadium, v51). Applied after every effect that changes
        /// the Bench size, so the smaller number wins. int.MaxValue means no cap.
        /// </summary>
        public virtual int CapMaxBench(EffectContext src, int player) { return int.MaxValue; }
        /// <summary>Windup Arm (v51): the Pokémon can attack even if it's Asleep or Paralyzed.</summary>
        public virtual bool AllowsAttackWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>Escape Board (v85): the Pokémon can retreat even if it's Asleep or Paralyzed.</summary>
        public virtual bool AllowsRetreatWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// Eternatus VMAX's Eternal Zone (v51): the player can't put this Pokémon into play by any means (Compendium). Checked by
        /// the effects that put Pokémon from the deck or discard pile onto the Bench; cards played from the hand use
        /// BlocksPlayingPokemon.
        /// </summary>
        public virtual bool BlocksPuttingIntoPlay(EffectContext src, int player, CardDef card) { return false; }
        /// <summary>
        /// True for Pokémon whose Ability prevents effects of the opponent's Abilities (Enamorus V's Guardian of Love, Corviknight
        /// VMAX's Lustrous Body). One already in play when Aerodactyl VSTAR uses Ancient Star keeps its Abilities, and so do the
        /// Pokémon it protects; one that comes into play afterwards doesn't (your ruling, v51).
        /// </summary>
        public virtual bool ShieldsFromOpponentsAbilities { get { return false; } }
        /// <summary>
        /// Temple of Sinnoh (Stadium, v53): Special Energy attached to Pokémon (both players') provide [C] and have no other
        /// effect. They're still Special Energy with their names (Compendium). A Pokémon shielded by Lunatone's New Moon keeps them.
        /// Also asked of Pokémon in play (Dusknoir's Spectral Breach, v54): it counts only while that Pokémon's Ability works.
        /// </summary>
        public virtual bool DisablesSpecialEnergy(EffectContext src) { return false; }
        /// <summary>
        /// Hoothoot's Stand Sentry (v53): true when this Energy, attached to holder, can't be discarded by the effect of the
        /// other player's Item or Supporter card that is resolving now. Asked only of the holder owner's sources.
        /// </summary>
        public virtual bool ProtectsEnergyFromTrainerDiscard(EffectContext src, PokemonInPlay holder, CardInstance energy) { return false; }
        /// <summary>
        /// Greedent's Brazen Tail (v59): true when this Energy, attached to holder, can't be moved to a hand, deck or discard
        /// pile by the effect of the other player's Item or Supporter card that is resolving now. Pokémon Tools aren't Items
        /// here (your ruling). Asked only of the holder owner's sources.
        /// </summary>
        public virtual bool ProtectsEnergyFromTrainerRemoval(EffectContext src, PokemonInPlay holder, CardInstance energy) { return false; }
        /// <summary>
        /// Tool Jammer (v60): while the Pokémon this Tool is attached to is Active, Pokémon Tools attached to the opponent's
        /// Active Pokémon have no effect (except other Tools that also say this).
        /// </summary>
        public virtual bool JamsOpposingActiveTools { get { return false; } }
        /// <summary>
        /// Centiskorch's Overheater (v60): the other player's (player's) Burned Active Pokémon doesn't recover from Burned when
        /// its Pokémon Checkup coin is heads. Asked only of the other player's sources.
        /// </summary>
        public virtual bool KeepsOpponentBurned(EffectContext src, int player) { return false; }
        /// <summary>
        /// Eevee's Resonant Evolution (v53): one of the source owner's Pokémon was just evolved by a card played from their hand.
        /// before is the card it was. Asked of every source of that player, after the evolution and its own triggers.
        /// </summary>
        public virtual void OnOwnPokemonEvolvedFromHand(EffectContext src, PokemonInPlay evolved, CardDef before) { }
        /// <summary>
        /// Blunder Policy (v54): the Pokémon this Tool was attached to used an attack, and its owner flipped at least one tails
        /// for that attack's damage or effects. Called after the attack, even if the holder or the Tool has left play since.
        /// </summary>
        public virtual void OnHolderAttackFlippedTails(EffectContext src) { }
        /// <summary>
        /// Goodra's Slimy Room (v55): the other player (player) must flip a coin whenever they try to attach an Energy card from
        /// their hand to a Pokémon, by the normal attachment or by an effect (your ruling). On tails the card is discarded
        /// instead, and the normal attachment isn't used up. Asked only of the other player's sources; one flip per card.
        /// </summary>
        public virtual bool FlipsForEnergyFromHand(EffectContext src, int player) { return false; }
        /// <summary>Skaters' Park (v55): basic Energy discarded to pay this player's Retreat Cost goes to their hand instead.</summary>
        public virtual bool RetreatEnergyToHand(EffectContext src, int player) { return false; }
        /// <summary>Dashing Pouch (v86): any Energy the retreating Pokémon discards for its Retreat Cost goes to the hand.</summary>
        public virtual bool RetreatAnyEnergyToHand(EffectContext src, PokemonInPlay retreating) { return false; }
        /// <summary>
        /// Milotic's Dew Guard (v58): "Whenever your opponent plays a Supporter card from their hand, prevent all effects of that
        /// card done to you or your hand." Asked of the source owner's cards (GameEngine.SupporterEffectsPreventedOn).
        /// </summary>
        public virtual bool ShieldsPlayerFromSupporters(EffectContext src) { return false; }
        /// <summary>Pinsir's Vise Coach (v58): this attack damage isn't affected by the target's Resistance. Asked of every source.</summary>
        public virtual bool IgnoresResistance(EffectContext src, DamageInfo info) { return false; }
        /// <summary>Zacian LV.X's Bladed Armament (v67): this attack's damage isn't affected by any effects on the Defending Pokémon.</summary>
        public virtual bool AttacksIgnoreDefenderEffects(EffectContext src, DamageInfo info) { return false; }
        /// <summary>
        /// Dream Ball and Treasure Energy (v58): a Trainer or Energy card that does something when its owner takes it as a face-down
        /// Prize card, before it goes to the hand. The engine then calls OnTakenAsPrize (on any turn; the script checks).
        /// </summary>
        public virtual bool UsableWhenTakenAsPrize { get { return false; } }
        /// <summary>
        /// Shiftry's Shifty Substitution (v62): each Supporter card in the other player's hand has the effect "Draw 3 cards"
        /// instead of its own. Asked only of the other player's sources (GameEngine.SupportersDrawThree).
        /// </summary>
        public virtual bool TurnsOpponentsSupportersIntoDrawThree(EffectContext src) { return false; }
        /// <summary>
        /// Wyndon Stadium (v62): asked of the Stadium in play whenever either player plays a Pokémon from their hand to evolve
        /// 1 of their Pokémon. before is the card it was.
        /// </summary>
        public virtual void OnPokemonEvolvedFromHandUnderStadium(EffectContext src, PokemonInPlay evolved, CardDef before) { }
        /// <summary>
        /// Skiploom's Solar Evolution (v63): its OnOwnerAttachedEnergyFromHand may evolve the Pokémon. The attaching player
        /// orders simultaneous reactions, so it goes first, and the other player's reactions to "that Pokémon" then don't apply
        /// to the evolved Pokémon (Compendium 2022-04-28, Galarian Cursola V's Gnawing Aura).
        /// </summary>
        public virtual bool EvolvesOnOwnerAttachFromHand { get { return false; } }

        /// <summary>
        /// Frozen City (v110): its mandatory damage counters come before any "evolve it" reaction to the same attachment (Eevee's
        /// Energy Evolution + Frozen City, Compendium 2014-08-28); the Knock Out check still waits until the reactions are done.
        /// </summary>
        public virtual bool ReactsFirstToAttachFromHand { get { return false; } }

        /// <summary>Magnezone's Dual Brains (v111): how many Supporter cards a player may play this turn (2 in all, however many Magnezone).</summary>
        public virtual int ModifySupporterLimit(EffectContext src, int player, int limit) { return limit; }

        /// <summary>Musharna's Dream Waltz (v112): "This attack can be used even if this Pokémon is Asleep" (not while Paralyzed).</summary>
        public virtual bool UsableWhileAsleep(AttackDef attack) { return false; }
        /// <summary>
        /// Billowing Smoke (v64): asked of the Tools (and other cards) on a Pokémon Knocked Out by damage from the opponent's
        /// attack. True: the opponent discards the Prize cards they would take for it instead of putting them into their hand.
        /// </summary>
        public virtual bool DiscardsPrizesForHolder(EffectContext src) { return false; }
        /// <summary>
        /// Spikemuth (v64): asked of the Stadium in play whenever a player's Active Pokémon moves to the Bench during that
        /// player's own turn (retreating, switching).
        /// </summary>
        public virtual void OnActiveMovedToBenchUnderStadium(EffectContext src, int player, PokemonInPlay movedToBench) { }
        /// <summary>
        /// Glimwood Tangle (v64): asked of the Stadium in play after the turn player flips coins for an attack. True offers them
        /// to ignore the results and flip again (once during each player's turn; the engine keeps count).
        /// </summary>
        public virtual bool StadiumOffersAttackReflip(EffectContext src, PokemonInPlay attacker) { return false; }

        /// <summary>
        /// Chaotic Swell (Stadium, v68): when either player plays a Stadium card from their hand, this Stadium is discarded and
        /// then the new one is too; the new one has no effect.
        /// </summary>
        public virtual bool DiscardsNewStadiums(EffectContext ctx) { return false; }
        /// <summary>
        /// A Stadium that "prevents all effects of Item and Supporter cards played from the hand done to this Stadium card" (the
        /// Prism Star Stadiums, v80): those cards can't discard it or put it anywhere.
        /// </summary>
        public virtual bool IgnoresItemsAndSupporters { get { return false; } }
        /// <summary>
        /// Primarina's Harmonics (v81): when its owner attaches an Energy card from their hand for the turn (not with an attack,
        /// Ability or Trainer card), they may attach a second one.
        /// </summary>
        public virtual bool AllowsSecondEnergyAttachment(EffectContext src) { return false; }
        /// <summary>A Pokémon Tool was attached from its owner's hand to this source's Pokémon (Tapu Lele's Charmed Charm, v81).</summary>
        public virtual void OnToolAttachedFromHand(EffectContext src, CardInstance tool, PokemonInPlay holder) { }

        /// <summary>
        /// "At the end of your opponent's turn" checked after that turn's end-of-turn effects and Knock Outs, before Pokémon Checkup
        /// (Compendium: Wishiwashi's Scatter, 2019-10-31; v68). Called for every source, on either player's turn.
        /// </summary>
        public virtual void OnEndOfTurnAfterKnockOuts(EffectContext src) { }

        /// <summary>
        /// Glaceon-GX's Freezing Gaze (v69): asked of Abilities in play. True: this card of the other player has no Abilities
        /// in their hand or discard pile right now (its in-play Abilities are blocked through IsAbilityBlocker).
        /// </summary>
        public virtual bool BlocksOpponentsAbilitiesOutOfPlay(EffectContext src, CardDef card) { return false; }
        /// <summary>Alolan Muk's Power of Alchemy (v90): like BlocksOpponentsAbilitiesOutOfPlay, for the owner's own hand and discard pile.</summary>
        public virtual bool BlocksOwnAbilitiesOutOfPlay(EffectContext src, CardDef card) { return false; }
        /// <summary>
        /// Carracosta's Ancient Custom (v72): "Pokémon Tool cards attached to your opponent's Pokémon have no effect." Asked of the
        /// Pokémon's own script (not as a passive source), while its Ability works.
        /// </summary>
        public virtual bool DisablesOpponentsTools { get { return false; } }
        /// <summary>Yveltal's Fright Night (v97): while this Pokémon is its owner's Active Pokémon, every Pokémon Tool in play has no effect.</summary>
        public virtual bool DisablesAllToolsWhileActive { get { return false; } }
        /// <summary>Banette's Tool Concealment (v99): while this Pokémon is in play, every Pokémon Tool in play has no effect.</summary>
        public virtual bool DisablesAllTools { get { return false; } }
        /// <summary>Wide Lens (v99): this card's holder's attacks apply Weakness and Resistance to the opponent's Benched Pokémon too.</summary>
        public virtual bool AppliesWeaknessAndResistanceOnBench(EffectContext src, DamageInfo info) { return false; }
        /// <summary>Hawlucha's Shining Spirit (v103): the holder's own attack damage isn't affected by Weakness or Resistance.</summary>
        public virtual bool OwnAttacksIgnoreWeaknessAndResistance(EffectContext src, DamageInfo info) { return false; }
        /// <summary>Bursting Balloon (v97): a Tool discarded at the end of its owner's opponent's turn, even while Tools have no effect (2016).</summary>
        public virtual bool DiscardedAtEndOfOpponentsTurn { get { return false; } }
        /// <summary>Technical Machines (v97): a Tool discarded at the end of its owner's turn, even while Tools have no effect (your ruling).</summary>
        public virtual bool DiscardedAtEndOfOwnersTurn { get { return false; } }
        /// <summary>
        /// U-Turn Board, Recycle Energy (v72): "If this card is discarded from play, put it into your hand instead of the discard
        /// pile." Asked when an attached Tool or Energy is discarded (by an effect, or with its Pokémon).
        /// </summary>
        public virtual bool GoesToHandWhenDiscardedFromPlay { get { return false; } }
        /// <summary>
        /// "Whenever you play a Pokémon ... from your hand during your turn" for a Pokémon played onto the Bench (Amoonguss's
        /// Bursting Spores, v72): the owner's sources, after the benched Pokémon's own trigger. Evolving from the hand is
        /// OnOwnPokemonEvolvedFromHand.
        /// </summary>
        public virtual void OnOwnPokemonPlayedFromHandToBench(EffectContext src, PokemonInPlay benched) { }
        /// <summary>
        /// v142: any player put a Pokémon onto their Bench from their hand, played or moved there by an effect (Dark Magcargo's Hot
        /// Plate, your ruling). Asked of every source.
        /// </summary>
        public virtual void OnPokemonBenchedFromHand(EffectContext src, PokemonInPlay benched) { }
        /// <summary>v142: a player is trying to retreat their Active Pokémon, before anything is paid (Unown [C]'s [Chase]). Asked of every source.</summary>
        public virtual void OnActiveRetreatTried(EffectContext src, PokemonInPlay retreating) { }
        /// <summary>
        /// v142: a Pokémon in play whose Pokémon Power stops another Pokémon's Pokémon Powers (Dark Feraligatr's Scare: the opponent's
        /// Baby Pokémon). Asked of Pokémon with printed powers; the script checks that its own power works.
        /// </summary>
        public virtual bool StopsPokemonPowersOf(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// v146: a Poké-Body that flips once against the attack resolving now, the first time it would affect this Pokémon; heads,
        /// the attack does nothing more to it (Aquapolis Jumpluff's Fluff, like Haunter's Transparency). Returns the body's name
        /// when it applies to this attack, otherwise null.
        /// </summary>
        public virtual string FlipsToDodgeAttack(EffectContext src, PokemonInPlay target) { return null; }
        /// <summary>v146: this Energy card can't be attached from the hand to that Pokémon (Aquapolis Zapdos's Anti-Lightning).</summary>
        public virtual bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target) { return false; }
        /// <summary>
        /// v147: a Poké-Body in play that stops another Pokémon's Poké-Powers (Skyridge Umbreon's Dark Gaze: the Benched Pokémon's).
        /// Asked of every Pokémon with Abilities; the script checks that its own body works (GameEngine.AbilityBlocked).
        /// </summary>
        public virtual bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// v159: a Poké-Body in play that stops another Pokémon's Poké-Bodies (Muk ex's Toxic Gas). bodyName is the Poké-Body
        /// being checked, so a blocker can leave its own kind alone without checking itself in a loop.
        /// </summary>
        public virtual bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName) { return false; }
        /// <summary>
        /// v147: the Pokémon itself just got a Special Condition (Skyridge Wobbuffet's Mirror Coat). kind, sourceOwner and
        /// sourcePokemon describe what gave it (the effect resolving when it was applied).
        /// </summary>
        public virtual void OnHolderConditionApplied(EffectContext src, SpecialCondition condition, EffectSourceKind kind, int sourceOwner, PokemonInPlay sourcePokemon) { }
        /// <summary>v147: this Pokémon card's own rules let it evolve into that card (Skyridge Buried Fossil: what evolves from Mysterious Fossil).</summary>
        public virtual bool EvolvesIntoByRule(CardDef self, CardDef evolution) { return false; }
        /// <summary>v147: any Technical Machine can be attached to this Pokémon, whatever the TM says (Skyridge Xatu's Synchronicity).</summary>
        public virtual bool AllowsAnyTechnicalMachine(EffectContext src, PokemonInPlay mon) { return false; }
        /// <summary>
        /// v144: after the Energy attachment from the hand for the turn, this card's power offers 1 more Energy card from the hand
        /// onto that Pokémon (Venusaur's Harvest Bounty; each one offers its own). OnExtraEnergyAttachmentUsed runs when it's taken.
        /// </summary>
        public virtual bool OffersExtraEnergyAttachment(EffectContext src, PokemonInPlay target) { return false; }
        public virtual void OnExtraEnergyAttachmentUsed(EffectContext src) { }
        /// <summary>v144: an old Technical Machine: attached like a Pokémon Tool but not one, so it doesn't take the Tool's place (GameEngine.PokemonToolCount).</summary>
        public virtual bool IsAttachedNonTool { get { return false; } }
        /// <summary>
        /// Hypno's Hypnotic Pendulum (v73): asked of the other player's sources when this player must choose a new Active Pokémon
        /// because theirs was Knocked Out. Returning true (after any coin flip) means the source's owner chooses it instead.
        /// </summary>
        public virtual bool ChoosesOpponentsNewActive(EffectContext src, int player) { return false; }
        /// <summary>
        /// "When this Pokémon is Knocked Out" effects that need it gone first (Gastly's Swelling Spite, v73): asked of its Ability
        /// while it's still in play, then run once it and its cards have left play (before Prize cards are taken).
        /// </summary>
        public virtual void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive) { }
        /// <summary>Honchkrow-GX's Ruler of the Night (v73): the player can't play this Energy card from their hand.</summary>
        public virtual bool BlocksEnergyCard(EffectContext src, int player, CardInstance card) { return false; }
        /// <summary>Dust Island (v73), asked of the Stadium in play: a Poisoned Active Pokémon switched out by a Trainer card's effect makes the new Active Pokémon Poisoned.</summary>
        public virtual bool PoisonFollowsTrainerSwitch(EffectContext src) { return false; }
        /// <summary>
        /// Marowak's Bodyguard (v94): "Prevent all effects of attacks done to you or your hand by your opponent's Pokémon. Remove
        /// any existing effects." Asked of the owner's sources (see GameEngine.PlayerShieldedFromAttacks).
        /// </summary>
        public virtual bool ShieldsPlayerFromAttackEffects(EffectContext src) { return false; }
        /// <summary>
        /// Carbink's Energy Keeper (v94): this Energy on one of the owner's Pokémon can't be discarded by the opponent's attacks,
        /// Abilities or Trainer cards.
        /// </summary>
        public virtual bool ProtectsEnergyFromOpponentsDiscard(EffectContext src, PokemonInPlay holder, CardInstance energy) { return false; }
        /// <summary>v126, Chaos Gym (a Stadium): the player who plays this Trainer card flips a coin first (GameEngine.ChaosGymStops).</summary>
        public virtual bool FlipsForTrainers(EffectContext src, int player, CardInstance card) { return false; }
        /// <summary>
        /// v126: asked of every Pokémon in play's own script with the state checks after each action (Brock's Primeape's Scram:
        /// "if it ever has exactly 10 HP left"; Shapeshift's discards when its Pokémon is Asleep, Confused or Paralyzed).
        /// </summary>
        public virtual void OnStateCheck(EffectContext src) { }
        /// <summary>v126, Cinnabar City Gym: Weakness isn't applied to this attack damage.</summary>
        public virtual bool IgnoresWeakness(EffectContext src, DamageInfo info) { return false; }
    }

    /// <summary>Everything a card script needs, scoped to the player and card whose effect is resolving.</summary>
    public sealed class EffectContext
    {
        public readonly GameEngine Engine;
        public readonly int Player;
        /// <summary>The card whose effect this is.</summary>
        public readonly CardInstance Card;
        /// <summary>The Pokémon using the attack or Ability, or holding the Tool/Energy (null for Trainers and Stadiums).</summary>
        public readonly PokemonInPlay Pokemon;
        /// <summary>Card whose attacks are being used. Differs from Pokemon.Def when an attack is copied (e.g. Night Joker).</summary>
        public readonly CardDef AttackSource;
        /// <summary>Set by a Trainer's effect: shuffle the card into the deck instead of discarding it after use (Caretaker).</summary>
        public bool ReturnToDeck;
        /// <summary>Set by a Trainer's effect: put the card on the bottom of the deck instead of discarding it after use (Egg Incubator, v52).</summary>
        public bool ReturnToDeckBottom;
        /// <summary>Set by a Trainer's effect: the card itself went somewhere else (Hisuian Heavy Ball became a Prize card, v53), so it isn't discarded.</summary>
        public bool CardPlacedElsewhere;

        public EffectContext(GameEngine engine, int player, CardInstance card, PokemonInPlay pokemon)
            : this(engine, player, card, pokemon, null)
        {
        }

        public EffectContext(GameEngine engine, int player, CardInstance card, PokemonInPlay pokemon, CardDef attackSource)
        {
            Engine = engine;
            Player = player;
            Card = card;
            Pokemon = pokemon;
            AttackSource = attackSource;
        }

        public GameState State { get { return Engine.State; } }
        public int Opponent { get { return 1 - Player; } }
        public PlayerState Me { get { return Engine.State.Players[Player]; } }
        public PlayerState Opp { get { return Engine.State.Players[1 - Player]; } }
        public int Turn { get { return Engine.State.TurnNumber; } }
        /// <summary>"During your opponent's last turn": the opponent's latest turn (PlayerState.NoTurn if they haven't had one).</summary>
        public int OpponentsLastTurn { get { return Opp.LastTurnNumber == Turn ? Opp.PreviousTurnNumber : Opp.LastTurnNumber; } }
        /// <summary>"During your last turn": this player's turn before the current one (during your own turn).</summary>
        public int MyLastTurn { get { return Me.LastTurnNumber == Turn ? Me.PreviousTurnNumber : Me.LastTurnNumber; } }
        public CardDef Source { get { return Card == null ? null : Card.Def; } }
        public bool IsMyTurn { get { return State.CurrentPlayer == Player; } }
        /// <summary>The opponent's Active Pokémon (the Defending Pokémon during an attack).</summary>
        public PokemonInPlay Defending { get { return Opp.Active; } }
        /// <summary>Name for prompts and logs: the attack or Ability's card.</summary>
        public string SourceName { get { return Source == null ? "Effect" : Source.Name; } }

        public AttackDef Attack(int index) { return (AttackSource ?? Pokemon.Def).Attacks[index]; }

        // ------------------------------------------------------------------ Once-per-turn tracking

        public bool AbilityUsedThisTurn(string abilityName)
        {
            int turn;
            return Pokemon != null && Pokemon.AbilityUsedOnTurn.TryGetValue(abilityName, out turn) && turn == Turn;
        }

        public void MarkAbilityUsed(string abilityName)
        {
            if (Pokemon != null) Pokemon.AbilityUsedOnTurn[abilityName] = Turn;
        }

        public bool NamedLimitUsedThisTurn(string limitName)
        {
            int turn;
            return Me.NamedLimitUsedOnTurn.TryGetValue(limitName, out turn) && turn == Turn;
        }

        public void MarkNamedLimit(string limitName)
        {
            Me.NamedLimitUsedOnTurn[limitName] = Turn;
        }

        /// <summary>True during this player's own first turn.</summary>
        public bool IsMyFirstTurn { get { return IsMyTurn && State.IsPlayersFirstTurn; } }

        // ------------------------------------------------------------------ History

        /// <summary>"If any of your Pokémon were Knocked Out during your opponent's last turn".</summary>
        public bool WasKnockedOutLastOpponentTurn()
        {
            return Me.LastKnockedOutTurn == OpponentsLastTurn;
        }

        /// <summary>"... Knocked Out by damage from an attack during your opponent's last turn".</summary>
        public bool WasKnockedOutByAttackLastOpponentTurn()
        {
            return Me.LastKnockedOutByAttackTurn == OpponentsLastTurn;
        }

        /// <summary>Same, limited to some Pokémon ("any of your Hop's Pokémon", "any of your Team Rocket's Pokémon").</summary>
        public bool WasKnockedOutLastOpponentTurn(Func<CardDef, bool> filter, bool byAttackOnly)
        {
            foreach (var k in Me.KnockOuts)
                if (k.Turn == OpponentsLastTurn && (!byAttackOnly || k.ByAttack) && filter(k.Def)) return true;
            return false;
        }

        /// <summary>Prize cards this player has taken so far.</summary>
        public int MyPrizesTaken { get { return Engine.PrizesTaken(Player); } }
        /// <summary>Prize cards the opponent has taken so far.</summary>
        public int OpponentPrizesTaken { get { return Engine.PrizesTaken(Opponent); } }

        // ------------------------------------------------------------------ Board questions

        public bool IsActive(PokemonInPlay mon) { return mon != null && State.IsActive(mon); }
        public bool PokemonIsActive { get { return Pokemon != null && State.IsActive(Pokemon); } }
        public bool IsMine(PokemonInPlay mon) { return mon != null && mon.Owner == Player; }
        /// <summary>The Pokémon is still in play (it may have been Knocked Out or moved by an earlier effect).</summary>
        public bool InPlay(PokemonInPlay mon) { return mon != null && State.FindPokemon(mon.Uid) == mon; }

        public List<PokemonInPlay> MyPokemon() { return new List<PokemonInPlay>(Me.InPlay()); }
        public List<PokemonInPlay> OpponentPokemon() { return new List<PokemonInPlay>(Opp.InPlay()); }
        public List<PokemonInPlay> MyBench() { return new List<PokemonInPlay>(Me.Bench); }
        public List<PokemonInPlay> OpponentBench() { return new List<PokemonInPlay>(Opp.Bench); }
        public List<PokemonInPlay> MyPokemon(Func<PokemonInPlay, bool> filter) { return Me.InPlay().Where(filter).ToList(); }
        public List<PokemonInPlay> OpponentPokemon(Func<PokemonInPlay, bool> filter) { return Opp.InPlay().Where(filter).ToList(); }

        public int BenchSpace { get { return Engine.BenchSpace(Player); } }
        public int EnergyCount(PokemonInPlay mon) { return Engine.EnergyCount(mon); }
        public int RemainingHp(PokemonInPlay mon) { return Engine.RemainingHp(mon); }
        public bool HasExtraEnergy(int attackIndex, int extra) { return Engine.HasExtraEnergy(Pokemon, Attack(attackIndex), extra); }

        /// <summary>False when an effect stops the current attack/Ability from affecting this Pokémon (Mist Energy, Hide 'n' Sneak...).</summary>
        public bool CanAffect(PokemonInPlay target) { return target != null && !Engine.CurrentEffectPrevented(target, false); }
        /// <summary>The opponent's Benched Pokémon the current effect can switch into the Active Spot.</summary>
        public List<PokemonInPlay> GustTargets() { return Opp.Bench.Where(CanAffect).ToList(); }

        // ------------------------------------------------------------------ Damage

        public int DamageDefending(int amount)
        {
            return Engine.DealAttackDamage(Pokemon, Opp.Active, amount, true);
        }

        /// <summary>Attack damage to any Pokémon. Weakness and Resistance apply only if the target is Active.</summary>
        public int DamageTarget(PokemonInPlay target, int amount)
        {
            return Engine.DealAttackDamage(Pokemon, target, amount, true);
        }

        /// <summary>Attack damage with explicit options (ignore Weakness, Resistance or effects on the Defending Pokémon).</summary>
        public int DamageWith(PokemonInPlay target, int amount, bool applyWeakness, bool applyResistance, bool ignoreDefenderEffects)
        {
            if (target == null) return 0;
            return Engine.DealDamage(new DamageInfo
            {
                Attacker = Pokemon,
                Target = target,
                ApplyWeakness = applyWeakness,
                ApplyResistance = applyResistance,
                IgnoreDefenderEffects = ignoreDefenderEffects
            }, amount);
        }

        /// <summary>Damage to the attacker itself ("This Pokémon also does 30 damage to itself"). No Weakness/Resistance.</summary>
        public void DamageSelf(int amount)
        {
            if (Pokemon == null || amount <= 0) return;
            Engine.DealDamage(new DamageInfo
            {
                Attacker = Pokemon,
                Target = Pokemon,
                ApplyWeakness = false,
                ApplyResistance = false
            }, amount);
        }

        /// <summary>"This attack also does X damage to 1 of your opponent's Benched Pokémon." Returns the Pokémon hit.</summary>
        public PokemonInPlay DamageOneBenched(int amount, Func<PokemonInPlay, bool> filter = null)
        {
            var bench = Opp.Bench.Where(m => filter == null || filter(m)).ToList();
            if (bench.Count == 0) return null;
            var target = ChoosePokemon(bench, SourceName + ": choose 1 of your opponent's Benched Pokémon to damage");
            DamageTarget(target, amount);
            return target;
        }

        /// <summary>"This attack does X damage to 1 of your opponent's Pokémon."</summary>
        public PokemonInPlay DamageAnyOpponent(int amount, bool ignoreWeaknessAndResistance = false, bool ignoreEffects = false)
        {
            var all = OpponentPokemon();
            if (all.Count == 0) return null;
            var target = ChoosePokemon(all, SourceName + ": choose 1 of your opponent's Pokémon to damage");
            if (ignoreWeaknessAndResistance || ignoreEffects) DamageWith(target, amount, !ignoreWeaknessAndResistance, !ignoreWeaknessAndResistance, ignoreEffects);
            else DamageTarget(target, amount);
            return target;
        }

        public void PlaceCounters(PokemonInPlay target, int counters)
        {
            Engine.PlaceDamageCounters(target, counters, SourceName);
        }

        /// <summary>"Put N damage counters on these Pokémon in any way you like" (one counter at a time).</summary>
        public void PlaceCountersAnyWay(int counters, Func<List<PokemonInPlay>> pool)
        {
            for (int i = 0; i < counters; i++)
            {
                var targets = pool();
                if (targets.Count == 0) return;
                var t = ChoosePokemon(targets, SourceName + ": place damage counter " + (i + 1) + " of " + counters);
                PlaceCounters(t, 1);
            }
        }

        public bool ApplyCondition(PokemonInPlay target, SpecialCondition condition)
        {
            return Engine.ApplyCondition(target, condition);
        }

        public int Heal(PokemonInPlay target, int amount)
        {
            return Engine.Heal(target, amount);
        }

        public bool AddEffect(ContinuousEffect effect) { return Engine.AddEffect(effect); }

        // Common attack effects on the Defending Pokémon or the attacker.
        public void DefendingCantRetreat(PokemonInPlay defending)
        {
            if (defending == null || !InPlay(defending)) return;
            AddEffect(new CantRetreatEffect(defending.Name + " can't retreat during its next turn (" + SourceName + ")", Player, defending.Uid, Turn + 1));
        }

        public void SelfCantAttackNextTurn()
        {
            if (!InPlay(Pokemon)) return;
            AddEffect(new CantAttackEffect(Pokemon.Name + " can't attack during its next turn (" + SourceName + ")", Player, Pokemon.Uid, Turn + 2));
        }

        public void SelfCantUseAttackNextTurn(string attackName)
        {
            if (!InPlay(Pokemon)) return;
            AddEffect(new CantUseAttackEffect(Pokemon.Name + " can't use " + attackName + " during its next turn", Player, Pokemon.Uid, Turn + 2, attackName));
        }

        /// <summary>"During your opponent's next turn, prevent all damage from and effects of attacks done to this Pokémon."</summary>
        public void ProtectSelfNextTurn()
        {
            if (!InPlay(Pokemon)) return;
            AddEffect(new ProtectEffect(Pokemon.Name + " is protected from attacks during the opponent's next turn (" + SourceName + ")", Player, Pokemon.Uid, Turn + 1));
        }

        /// <summary>"During your opponent's next turn, this Pokémon takes X less damage from attacks."</summary>
        public void SelfTakesLessNextTurn(int amount)
        {
            if (!InPlay(Pokemon)) return;
            AddEffect(new DamageTakenEffect(Pokemon.Name + " takes " + amount + " less damage during the opponent's next turn (" + SourceName + ")", Player, Pokemon.Uid, Turn + 1, -amount));
        }

        // ------------------------------------------------------------------ Cards and zones

        public int Draw(int count) { return Engine.DrawCards(Player, count); }
        public int DrawUntil(int handSize) { return Engine.DrawCards(Player, Math.Max(0, handSize - Me.Hand.Count)); }
        public bool Flip() { return Engine.FlipCoin(Player, SourceName); }
        public int FlipCoins(int count) { return Engine.FlipCoins(Player, count, SourceName); }
        public int FlipUntilTails() { return Engine.FlipUntilTails(Player, SourceName); }
        public void ShuffleMyDeck() { Engine.ShuffleDeck(Player); }

        public List<CardInstance> SearchMyDeck(Func<CardDef, bool> filter, int max, string prompt, bool reveal = true, int min = 0)
        {
            return Engine.SearchDeck(Player, filter, max, prompt, Source, reveal, min);
        }

        /// <summary>Search for up to max cards, put them into the hand, shuffle. Returns the cards found.</summary>
        public List<CardInstance> SearchToHand(Func<CardDef, bool> filter, int max, string prompt, bool reveal = true, int min = 0)
        {
            var found = SearchMyDeck(filter, max, prompt, reveal, min);
            Me.Hand.AddRange(found);
            ShuffleMyDeck();
            return found;
        }

        /// <summary>Search for up to max Basic Pokémon (limited by Bench space), put them onto the Bench, shuffle.</summary>
        public List<CardInstance> SearchToBench(Func<CardDef, bool> filter, int max, string prompt)
        {
            int n = Math.Min(max, BenchSpace);
            // Compendium (Call for Family): with a full Bench the whole effect fails, the search and shuffle included.
            if (n <= 0) return new List<CardInstance>();
            var found = SearchMyDeck(d => d.IsBasicPokemon && filter(d) && !Engine.PutIntoPlayBlocked(Player, d), n, prompt);
            foreach (var c in found) Engine.PutOntoBench(Player, c, false);
            ShuffleMyDeck();
            return found;
        }

        /// <summary>The top n cards of my deck (still in the deck; index 0 is the top).</summary>
        public List<CardInstance> LookAtTop(int n)
        {
            var top = Me.Deck.Take(Math.Min(n, Me.Deck.Count)).ToList();
            Engine.NoteLookedAt(top, "Top " + top.Count + (top.Count == 1 ? " card" : " cards") + " of your deck", Player);
            return top;
        }

        /// <summary>Moves cards that are in my deck into my hand.</summary>
        public void DeckToHand(IEnumerable<CardInstance> cards)
        {
            foreach (var c in cards.ToList())
            {
                Me.Deck.Remove(c);
                Me.Hand.Add(c);
            }
        }

        /// <summary>
        /// Moves cards from my discard pile into my deck (the caller shuffles). Trainer cards stay in the discard pile while the
        /// opponent's Sandshrew (Sand Screen) blocks it and this is my Item or Supporter. Returns the cards moved.
        /// </summary>
        public List<CardInstance> DiscardToDeck(IEnumerable<CardInstance> cards)
        {
            var moved = new List<CardInstance>();
            bool trainersBlocked = Engine.TrainerDiscardToDeckBlocked(Player);
            foreach (var c in cards.ToList())
            {
                if (trainersBlocked && c.Def.IsTrainer) continue;
                if (!Me.Discard.Remove(c)) continue;
                Me.Deck.Add(c);
                moved.Add(c);
            }
            if (trainersBlocked && moved.Count < cards.Count())
                Log("Trainer cards in " + Engine.Their(Player) + " discard pile can't be put into " + Engine.Their(Player) + " deck right now.");
            return moved;
        }

        /// <summary>Moves cards from my discard pile into my hand.</summary>
        public void DiscardToHand(IEnumerable<CardInstance> cards)
        {
            var list = cards.ToList();
            if (list.Count > 0 && Engine.DiscardToHandBlocked(Player))
            {
                Log("Cards in " + Engine.Their(Player) + " discard pile can't be put into " + Engine.Their(Player) + " hand right now.");
                return;
            }
            foreach (var c in list)
            {
                Me.Discard.Remove(c);
                Me.Hand.Add(c);
            }
            if (list.Count > 0) Engine.Reveal(Player, list, "put these cards from " + Engine.Their(Player) + " discard pile into " + Engine.Their(Player) + " hand");
        }

        public List<CardInstance> ChooseCards(IList<CardInstance> from, int min, int max, string prompt)
        {
            return Engine.ChooseCards(Player, from, min, max, prompt, Source);
        }

        public PokemonInPlay ChoosePokemon(IList<PokemonInPlay> candidates, string prompt)
        {
            return Engine.ChoosePokemon(Player, candidates, prompt, Source);
        }

        /// <summary>Choose between min and max different Pokémon at once ("choose up to 2 of your Pokémon").</summary>
        public List<PokemonInPlay> ChoosePokemonMany(IList<PokemonInPlay> candidates, int min, int max, string prompt)
        {
            if (candidates.Count == 0 || max <= 0) return new List<PokemonInPlay>();
            var opts = candidates.Select(m => new DecisionOption
            {
                Label = Engine.DescribeFor(Player, m),
                PokemonUid = m.Uid,
                CardId = m.Top.Id,
                Card = m.Def
            }).ToList();
            var picks = Engine.AskMany(Player, DecisionKind.ChoosePokemon, prompt, opts, min, max, Source);
            return picks.Select(i => candidates[i]).ToList();
        }

        public bool AskYesNo(string prompt) { return Engine.AskYesNo(Player, prompt, Source); }
        public int AskNumber(int min, int max, string prompt) { return Engine.AskNumber(Player, min, max, prompt, Source); }

        /// <summary>Pick one of several labelled choices. Returns the index.</summary>
        public int ChooseOption(string prompt, params string[] labels)
        {
            var opts = new List<DecisionOption>();
            for (int i = 0; i < labels.Length; i++) opts.Add(new DecisionOption { Label = labels[i], Index = i });
            return Engine.AskOne(Player, DecisionKind.ChooseOption, prompt, opts, Source, false);
        }

        // ------------------------------------------------------------------ Switching

        /// <summary>"Switch your Active Pokémon with 1 of your Benched Pokémon." Returns false if there was no Bench.</summary>
        public bool SwitchMyActive(string prompt = null)
        {
            if (Me.Bench.Count == 0 || Me.Active == null) return false;
            var target = ChoosePokemon(MyBench(), prompt ?? SourceName + ": choose a Benched Pokémon to switch in");
            return Engine.SwitchActive(Player, target);
        }

        /// <summary>"Switch this Pokémon with 1 of your Benched Pokémon" (only if this Pokémon is Active).</summary>
        public bool SwitchSelfWithBench()
        {
            if (!PokemonIsActive || Me.Bench.Count == 0) return false;
            var target = ChoosePokemon(MyBench(), SourceName + ": switch with which Benched Pokémon?");
            return Engine.SwitchActive(Player, target);
        }

        /// <summary>"Switch in 1 of your opponent's Benched Pokémon to the Active Spot." You choose.</summary>
        public PokemonInPlay GustOpponent(string prompt = null)
        {
            var targets = GustTargets();
            if (targets.Count == 0) return null;
            var target = ChoosePokemon(targets, prompt ?? SourceName + ": choose 1 of your opponent's Benched Pokémon to switch in");
            bool power = Engine.CurrentSourceKind == EffectSourceKind.Ability || Engine.CurrentSourceKind == EffectSourceKind.PokemonPower;
            Engine.PowerGustInProgress = power;
            try { return Engine.SwitchActive(Opponent, target) ? target : null; }
            finally { Engine.PowerGustInProgress = false; }
        }

        /// <summary>"Switch out your opponent's Active Pokémon to the Bench. (Your opponent chooses the new Active Pokémon.)"</summary>
        public bool OpponentSwitchesActive()
        {
            if (Opp.Bench.Count == 0 || Opp.Active == null || !CanAffect(Opp.Active)) return false;
            var target = Engine.ChoosePokemon(Opponent, Opp.Bench.ToList(), "Choose your new Active Pokémon", Source);
            return Engine.SwitchActive(Opponent, target);
        }

        // ------------------------------------------------------------------ Energy

        /// <summary>Basic Energy cards in my discard pile matching the filter.</summary>
        public List<CardInstance> BasicEnergyInDiscard(Func<CardDef, bool> filter = null)
        {
            return Me.Discard.Where(c => c.Def.IsBasicEnergy && (filter == null || filter(c.Def))).ToList();
        }

        /// <summary>
        /// Attach up to max cards from a pile (my discard pile, or cards already taken out of the deck) to Pokémon
        /// chosen by the player "in any way you like". Each card is removed from the pile it's in.
        /// targetFilter gets (energy card, Pokémon). Returns how many were attached.
        /// </summary>
        public int AttachCardsAnyWay(List<CardInstance> pile, int max, Func<CardInstance, PokemonInPlay, bool> targetFilter, string prompt, bool optional = true)
        {
            int attached = 0;
            var available = pile.ToList();
            for (int i = 0; i < max && available.Count > 0; i++)
            {
                var usable = available.Where(c => MyPokemon().Any(m => targetFilter(c, m))).ToList();
                if (usable.Count == 0) break;
                var picks = ChooseCards(usable, optional ? 0 : 1, 1, prompt + " (" + (i + 1) + " of up to " + max + ")");
                if (picks.Count == 0) break;
                var card = picks[0];
                var target = ChoosePokemon(MyPokemon().Where(m => targetFilter(card, m)).ToList(), prompt + ": attach " + card.Def.Name + " to which Pokémon?");
                available.Remove(card);
                // A card from the hand is attached "from the hand" (Gnawing Curse, Slimy Room react to it: v55).
                bool fromHand = !Me.Discard.Remove(card) && !Me.Deck.Remove(card) && Me.Hand.Remove(card);
                if (Engine.AttachEnergyCard(card, target, fromHand)) attached++;
            }
            return attached;
        }

        /// <summary>Discard Energy cards from a Pokémon, chosen by this player. Returns how many were discarded.</summary>
        public int DiscardEnergyFrom(PokemonInPlay mon, int count, string prompt, Func<CardInstance, bool> filter = null)
        {
            if (mon == null) return 0;
            var options = mon.Energy.Where(e => filter == null || filter(e)).ToList();
            if (options.Count == 0 || count <= 0) return 0;
            var picks = count >= options.Count ? options : ChooseCards(options, count, count, prompt);
            int n = 0;
            foreach (var e in picks) if (Engine.DiscardEnergy(mon, e)) n++;
            return n;
        }

        public void Log(string text) { Engine.Log(Player, text); }
    }

    /// <summary>Maps cards to scripts. Lookup by printing key first, then by name, so reprints share behaviour.</summary>
    public sealed class CardScriptRegistry
    {
        private readonly Dictionary<string, CardScript> byKey = new Dictionary<string, CardScript>();
        private readonly Dictionary<string, CardScript> byName = new Dictionary<string, CardScript>();
        private static readonly CardScript Vanilla = new CardScript();

        public void RegisterByName(string cardName, CardScript script) { byName[cardName] = script; }
        public void RegisterByKey(string cardKey, CardScript script) { byKey[cardKey] = script; }

        /// <summary>The script registered for a card key, or null (v85: another set's print of the same Pokémon reuses it).</summary>
        public CardScript ForKey(string cardKey)
        {
            CardScript s;
            return cardKey != null && byKey.TryGetValue(cardKey, out s) ? s : null;
        }

        public bool HasScriptFor(CardDef def)
        {
            var key = def.BaseKey;
            return (key != null && byKey.ContainsKey(key)) || byName.ContainsKey(def.Name);
        }

        /// <summary>Alternate printings (other arts, reprints) use the main printing's script.</summary>
        public CardScript For(CardDef def)
        {
            CardScript s;
            var key = def.BaseKey;
            if (key != null && byKey.TryGetValue(key, out s)) return s;
            if (byName.TryGetValue(def.Name, out s)) return s;
            return Vanilla;
        }
    }
}
