using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    public sealed partial class GameEngine
    {
        // ------------------------------------------------------------------ Legal actions

        public List<DecisionOption> GetMainActions(int player)
        {
            var p = State.Players[player];
            var list = new List<DecisionOption>();
            var byKey = new Dictionary<string, DecisionOption>();

            // Identical cards in the hand are listed once; the option remembers the other copies (SameCardIds) so the
            // player can drag whichever copy they like.
            void Add(DecisionOption o, string key)
            {
                DecisionOption first;
                if (!byKey.TryGetValue(key, out first))
                {
                    byKey[key] = o;
                    list.Add(o);
                    return;
                }
                if (o.CardId < 0 || o.CardId == first.CardId) return;
                if (first.SameCardIds == null) first.SameCardIds = new List<int>();
                if (!first.SameCardIds.Contains(o.CardId)) first.SameCardIds.Add(o.CardId);
            }

            bool benchRoom = BenchHasRoom(player);
            foreach (var c in p.Hand)
            {
                var d = c.Def;
                bool playBlocked = d.IsPokemon && PokemonPlayBlocked(player, d); // Potent Glare
                if (d.IsBasicPokemon && benchRoom && !playBlocked && IsPlayableBasic(d))
                    Add(new DecisionOption { Label = "Bench " + d.Name, Action = ActionType.BenchBasic, CardId = c.Id, Card = d }, "B|" + d.BaseKey);
                // LEGEND: both halves go onto the Bench together (never during setup).
                if (d.IsLegend && d.LegendHalf == "top" && benchRoom && !playBlocked && LegendBottomInHand(p, d) != null)
                    Add(new DecisionOption { Label = "Bench " + d.Name, Action = ActionType.BenchBasic, CardId = c.Id, Card = d }, "B|" + d.BaseKey);

                // A Basic Pokémon card can also evolve a Baby Pokémon (Pichu into any Pikachu, v48).
                bool ontoBaby = d.IsBasicPokemon && p.InPlay().Any(m => m.Def.BabyEvolvesInto(d));
                if ((d.IsEvolution || ontoBaby) && !playBlocked) // CanEvolveInto handles the first turn (Boosted Evolution may allow it)
                {
                    foreach (var target in p.InPlay())
                    {
                        if (!CanEvolveInto(target, d, true)) continue;
                        Add(new DecisionOption
                        {
                            Label = "Evolve " + Describe(target) + " into " + d.Name,
                            Action = ActionType.Evolve, CardId = c.Id, PokemonUid = target.Uid, Card = d
                        }, "E|" + d.BaseKey + "|" + target.Uid);
                    }
                }

                // v169: Holon's Pokémon may be attached as an Energy card instead (the attachment for the turn).
                if ((d.IsEnergy || d.AttachesAsEnergy) && (!p.EnergyAttachedThisTurn || AnyNumberOfEnergyAttachments(player)) && !EnergyCardBlocked(player, c))
                {
                    var script = Scripts.For(d);
                    foreach (var target in p.InPlay())
                    {
                        if (!script.CanAttachTo(new EffectContext(this, player, c, target), target)) continue;
                        if (!CanAttachFromHand(target) || EnergyFromHandBlockedTo(c, target)) continue;
                        Add(new DecisionOption
                        {
                            Label = "Attach " + d.Name + (d.IsPokemon ? " as Energy" : "") + " to " + Describe(target),
                            Action = ActionType.AttachEnergy, CardId = c.Id, PokemonUid = target.Uid, Card = d
                        }, "A|" + d.BaseKey + "|" + target.Uid);
                    }
                }

                if (d.IsPokemon && d.Abilities.Count > 0 && !OutOfPlayAbilitiesBlocked(player, d))
                {
                    var handScript = Scripts.For(d);
                    if (handScript.CanUseFromHand(new EffectContext(this, player, c, null)))
                        Add(new DecisionOption
                        {
                            Label = "Use " + d.Abilities[0].Name + " (" + d.Name + " in your hand)",
                            Action = ActionType.UseHandAbility, CardId = c.Id, Card = d, Index = 0
                        }, "H|" + d.BaseKey);
                }

                if (d.IsFossil)
                {
                    // Played like a Basic Pokémon, but it's still an Item (Item lock stops it). Snorlax Doll only goes into play at setup.
                    if (benchRoom && !d.IsDoll && CanPlayTrainer(player, c))
                        Add(new DecisionOption { Label = "Bench " + d.Name, Action = ActionType.BenchBasic, CardId = c.Id, Card = d }, "B|" + d.BaseKey);
                }
                else if (d.IsTrainer && CanPlayTrainer(player, c))
                    Add(new DecisionOption { Label = "Play " + d.Name, Action = ActionType.PlayTrainer, CardId = c.Id, Card = d }, "T|" + d.BaseKey);
            }

            foreach (var mon in p.InPlay())
            {
                // Its own Abilities (or Powers), then the Poké-Powers a LV.X keeps from the Pokémon under it.
                var sourceCards = new List<CardInstance>();
                if (mon.Def.Abilities.Count > 0) sourceCards.Add(mon.Top);
                var inherited = InheritedPowersCard(mon);
                if (inherited != null) sourceCards.Add(inherited);
                // A Pokémon Tool that gives its Pokémon an Ability (Sky Seal Stone's VSTAR Power, v49). Tools that don't work
                // right now (Tool-disabling effects) give nothing.
                if (!ToolsDisabled() || StadiumShielded(mon))
                    foreach (var tool in mon.Tools)
                        if (tool.Def.Abilities.Count > 0 && ToolWorks(mon, tool)) sourceCards.Add(tool);
                foreach (var sourceCard in sourceCards)
                {
                    var def = sourceCard.Def;
                    var script = Scripts.For(def);
                    for (int i = 0; i < def.Abilities.Count; i++)
                    {
                        // Under a LV.X only Poké-Powers carry over; under a BREAK Pokémon, Abilities do (v92).
                        if (def.IsPokemon && sourceCard != mon.Top && !def.Abilities[i].IsPokePowerOrBody && !mon.Def.IsBreak && TransformTarget(mon) == null && ShapeshiftCard(mon) == null) continue;
                        if (AbilityBlocked(mon, def.Abilities[i].Name)) continue;
                        if (def.Abilities[i].VstarPower && p.VstarUsed) continue; // 1 VSTAR Power per game (v49)
                        var ctx = new EffectContext(this, player, sourceCard, mon);
                        if (!script.CanUseAbility(ctx, i)) continue;
                        list.Add(new DecisionOption
                        {
                            Label = "Use " + def.Abilities[i].Name + " (" + Describe(mon) + ")",
                            Action = ActionType.UseAbility, PokemonUid = mon.Uid, Index = i, Card = def, CardId = sourceCard.Id
                        });
                    }
                }
            }

            // Cards that can be used from the discard pile (Grant, v49): one option for each different card.
            foreach (var c in p.Discard)
            {
                if (c.Def.IsPokemon && c.Def.Abilities.Count > 0 && OutOfPlayAbilitiesBlocked(player, c.Def)) continue;
                if (!Scripts.For(c.Def).CanUseFromDiscard(new EffectContext(this, player, c, null))) continue;
                string what = c.Def.IsPokemon && c.Def.Abilities.Count > 0 ? c.Def.Name + "'s " + c.Def.Abilities[0].Name : c.Def.Name;
                Add(new DecisionOption { Label = "Use " + what + " from your discard pile", Action = ActionType.UseFromDiscard, CardId = c.Id, Card = c.Def }, "D|" + c.Def.BaseKey);
            }

            // V-UNION (v67): each V-UNION once per game, combine 4 different pieces from the discard pile onto the Bench as 1 Pokémon.
            foreach (var pieces in VUnionsReady(player))
                list.Add(new DecisionOption
                {
                    Label = "Combine " + pieces[0].Def.Name + " from your discard pile",
                    Action = ActionType.CombineVUnion, CardId = pieces[0].Id, Card = pieces[0].Def
                });

            if (State.Stadium != null && !StadiumSwitchedOff() && Scripts.For(State.Stadium.Def).CanUseStadium(new EffectContext(this, player, State.Stadium, null)))
                list.Add(new DecisionOption { Label = "Use " + State.Stadium.Def.Name, Action = ActionType.UseStadium, CardId = State.Stadium.Id, Card = State.Stadium.Def });

            foreach (var mon in p.InPlay())
            {
                // Lillie's Poké Doll (v68): only while it's the Active Pokémon, and it goes to the bottom of the deck instead.
                if (mon.Def.IsPokeDoll)
                {
                    if (p.Active == mon)
                        list.Add(new DecisionOption { Label = "Put " + mon.Name + " on the bottom of your deck", Action = ActionType.DiscardFossil, PokemonUid = mon.Uid, Card = mon.Def, CardId = mon.Top.Id });
                }
                else if (mon.Def.IsFossil)
                    list.Add(new DecisionOption { Label = "Discard " + Describe(mon) + " from play", Action = ActionType.DiscardFossil, PokemonUid = mon.Uid, Card = mon.Def, CardId = mon.Top.Id });
                // Lt. Surge's Secret Plan (v126): a face-down card can be flipped over at any time during its owner's turn.
                if (IsFaceDown(mon))
                    list.Add(new DecisionOption { Label = "Flip over " + Describe(mon), Action = ActionType.FlipFaceDown, PokemonUid = mon.Uid, Card = mon.Def, CardId = mon.Top.Id });
            }

            if (CanRetreat(p))
                list.Add(new DecisionOption { Label = "Retreat " + p.Active.Name, Action = ActionType.Retreat, PokemonUid = p.Active.Uid, Card = p.Active.Def, CardId = p.Active.Top.Id });

            if (p.Active != null && CanAttackThisTurn(p))
            {
                foreach (var a in UsableAttacks(p.Active))
                {
                    if (State.TurnNumber == 1 && !Rules.FirstTurnAttack && !UsableOnFirstTurn(p.Active, a)) continue; // first player's first turn
                    bool own = a.Source == p.Active.Def;
                    list.Add(new DecisionOption
                    {
                        Label = "Attack: " + a.Attack.Name + (own ? "" : " (" + a.Source.Name + ")"),
                        Action = ActionType.Attack, PokemonUid = p.Active.Uid, Index = a.Index, Card = a.Source, CardId = p.Active.Top.Id
                    });
                }
            }

            // Attacks a Benched Pokémon can use from the Bench (Dimensional Hand). Still your 1 attack for the turn.
            if (p.Active != null)
            {
                foreach (var benched in p.Bench)
                {
                    foreach (var a in BenchAttacks(benched))
                    {
                        if (State.TurnNumber == 1 && !Rules.FirstTurnAttack && !UsableOnFirstTurn(benched, a)) continue; // first player's first turn
                        list.Add(new DecisionOption
                        {
                            Label = "Attack: " + a.Attack.Name + " (" + benched.Name + " on the Bench)",
                            Action = ActionType.Attack, PokemonUid = benched.Uid, Index = a.Index, Card = a.Source, CardId = benched.Top.Id
                        });
                    }
                }
            }

            list.Add(new DecisionOption { Label = "End turn", Action = ActionType.EndTurn });
            return list;
        }

        /// <summary>
        /// V-UNION (v67): puts 4 different pieces of the Pokémon V-UNION whose piece has this card id from the discard pile onto the
        /// Bench as 1 Pokémon (piece 1 on top, the others under it, as the LEGEND halves are). Each V-UNION once per game (your ruling).
        /// Returns the new Pokémon, or null.
        /// </summary>
        public PokemonInPlay CombineVUnion(int player, int cardId)
        {
            var p = State.Players[player];
            var card = p.Discard.FirstOrDefault(c => c.Id == cardId);
            if (card == null) return null;
            var pieces = VUnionsReady(player).FirstOrDefault(set => set.Any(c => c.Def.Name == card.Def.Name));
            if (pieces == null) return null;
            foreach (var piece in pieces) p.Discard.Remove(piece);
            p.VUnionsCombined.Add(card.Def.Name);
            Emit(GameEventType.Info, player, PlayerName(player) + " combined 4 " + card.Def.Name + " pieces from " + Their(player) + " discard pile.", pieces[0].Id);
            var mon = PutOntoBench(player, pieces[0], false);
            for (int k = 1; k < pieces.Count; k++) mon.Stack.Insert(0, pieces[k]);
            return mon;
        }

        /// <summary>The bottom half of this LEGEND card in the player's hand, or null.</summary>
        private static CardInstance LegendBottomInHand(PlayerState p, CardDef top)
        {
            return p.Hand.FirstOrDefault(x => x.Def.IsLegend && x.Def.LegendHalf == "bottom" && x.Def.Name == top.Name);
        }

        /// <summary>Can this card in hand evolve that Pokémon now (normal play from hand)?</summary>
        public bool CanEvolveInto(PokemonInPlay target, CardDef evolution)
        {
            return CanEvolveInto(target, evolution, false);
        }

        /// <summary>
        /// fromHand: the player is playing the Evolution card from their hand onto the Pokémon. Rainbow DNA (Eevee ex) only works
        /// then; effects that evolve from the deck (Grand Tree) use the plain name check.
        /// </summary>
        public bool CanEvolveInto(PokemonInPlay target, CardDef evolution, bool fromHand)
        {
            // Δ Evolution (M Rayquaza-EX): played from the hand, it ignores the first-turn and same-turn limits (v56).
            bool anyTime = fromHand && (Scripts.For(evolution).EvolvesWithoutTimingLimits || evolution.AncientTrait == DeltaEvolution);
            // Boosted Evolution (Eevee) lets a Pokémon evolve on the first turn; Forest of Vitality doesn't.
            if (State.IsPlayersFirstTurn && !anyTime && !SameTurnEvolutionAllowed(target, evolution)) return false;
            if (EvolveLocked(target.Owner)) return false;
            if (evolution.IsLevelUp && !State.IsActive(target)) return false; // LV.X: only onto the Active Pokémon
            if (Scripts.For(evolution).PutIntoPlayOnlyByEffect) return false; // Palafin ex: only by Zero to Hero
            // Baby Pokémon evolve into their Basic Pokémon (Neo rules; only cards with the Baby Rule printed, your ruling v48).
            bool babyEvolution = target.Def.BabyEvolvesInto(evolution);
            // v147: Buried Fossil's rules let it evolve into what evolves from Mysterious Fossil.
            if (!babyEvolution && target.Name != evolution.EvolvesFrom && !(fromHand && EvolutionAllowedBy(target, evolution)) && !Scripts.For(target.Def).EvolvesIntoByRule(target.Def, evolution)) return false;
            // Brock's Ninetales's Shapeshift (v126): while it's treated as another Pokémon it can't evolve.
            if (ShapeshiftCard(target) != null) return false;
            if (target.CantEvolveOnTurn == State.TurnNumber) return false;
            if (fromHand && EvolutionFromHandBlocked(target)) return false;
            bool newThisTurn = target.TurnEnteredPlay >= State.TurnNumber || target.TurnEvolved == State.TurnNumber;
            if (newThisTurn && !anyTime && !SameTurnEvolutionAllowed(target, evolution)) return false;
            return true;
        }

        /// <summary>Tsareena's Queenly Heel: no Pokémon can be played from the hand to evolve this Pokémon right now.</summary>
        public bool EvolutionFromHandBlocked(PokemonInPlay target)
        {
            if (target == null) return false;
            foreach (var e in ActiveEffects()) if (e.BlocksEvolutionFromHand(this, target)) return true;
            // Archeops's Ancient Power (v128): an Ability in play.
            foreach (var src in SourcesFor(target)) if (src.Script.BlocksEvolutionFromHand(src.Ctx, target)) return true;
            return false;
        }

        /// <summary>Rainbow DNA (Eevee ex): the Pokémon's own Ability lets it evolve into a card that names another Pokémon.</summary>
        private bool EvolutionAllowedBy(PokemonInPlay target, CardDef evolution)
        {
            foreach (var src in SourcesOn(target))
                if ((src.Kind == EffectSourceKind.Ability || src.Kind == EffectSourceKind.PokemonPower) && src.Script.AllowsEvolvingInto(src.Ctx, target, evolution)) return true;
            return false;
        }

        /// <summary>Forest of Vitality: evolving a Pokémon during the turn it was played.</summary>
        public bool SameTurnEvolutionAllowed(PokemonInPlay target, CardDef evolution)
        {
            foreach (var src in SourcesFor(target))
                if (src.Script.AllowsSameTurnEvolution(src.Ctx, target, evolution)) return true;
            // Giovanni (v126): a timed effect on the Pokémon.
            foreach (var fx in ActiveEffects())
                if (fx.AllowsSameTurnEvolution(this, target, evolution)) return true;
            return false;
        }

        public bool CanPlayTrainer(int player, CardInstance card)
        {
            var p = State.Players[player];
            var d = card.Def;
            if (TrainerBlocked(player, card)) return false;
            var script = Scripts.For(d);
            var ctx = new EffectContext(this, player, card, null);
            switch (d.TrainerKind)
            {
                case TrainerKind.Item:
                    // A plain old Trainer card (v123) isn't an Item, so Item lock doesn't stop it.
                    if (d.IsItem && ItemLocked(player)) return false;
                    return script.CanPlayTrainer(ctx);
                case TrainerKind.Supporter:
                    // Lt. Surge's Strategy (v69) raises the number of Supporters for the turn.
                    if (p.SupporterPlayedThisTurn && p.SupportersPlayedCount >= SupporterLimitFor(player)) return false;
                    if (State.TurnNumber == 1 && !Rules.FirstTurnSupporter && !script.PlayableOnFirstTurn(ctx)) return false; // first player's first turn (a format can allow it, v123)
                    // Shiftry's Shifty Substitution: the card's effect is "Draw 3 cards", so any Supporter can be played to draw
                    // while there's a card to draw (your ruling, v62).
                    if (SupportersDrawThree(player)) return p.Deck.Count > 0;
                    return script.CanPlayTrainer(ctx);
                case TrainerKind.Stadium:
                    // v152: a Legendary Stadium is played from its top half, with the other half in the hand too.
                    if (d.IsStadiumHalf && (d.LegendHalf != "top" || StadiumOtherHalf(p, card) == null)) return false;
                    if (p.StadiumPlayedThisTurn && !script.IgnoresStadiumLimit(ctx)) return false;
                    if (State.Stadium != null && State.Stadium.Def.Name == d.Name) return false;
                    return script.CanPlayTrainer(ctx);
                case TrainerKind.Tool:
                    // Since Scarlet & Violet, Pokémon Tools are their own Trainer type, so "can't play Item cards"
                    // (Itchy Pollen, Oceanic Gloom) doesn't stop them. Jellicent ex names both.
                    // Pokémon Tool F (v102) goes on the opponent's Pokémon.
                    // A format whose Tools are Items (v123): Item lock stops them too.
                    if (d.IsItem && ItemLocked(player)) return false;
                    var toolSide = script.AttachesToOpponentsPokemon ? State.Players[1 - player] : p;
                    if (!toolSide.InPlay().Any(m => CanAttachToolTo(card, m))) return false;
                    return script.CanPlayTrainer(ctx);
                default:
                    return false;
            }
        }

        public bool CanRetreat(PlayerState p)
        {
            var a = p.Active;
            if (a == null || p.RetreatedThisTurn || p.Bench.Count == 0) return false;
            if (a.Def.IsFossil) return false; // Antique Fossils can't retreat
            if ((a.Has(SpecialCondition.Asleep) || a.Has(SpecialCondition.Paralyzed)) && !CanRetreatWhileAsleepOrParalyzed(a)) return false;
            if (RetreatPrevented(a)) return false;
            int cost = RetreatCostOf(a);
            return cost == 0 || EnergyCount(a) >= cost || RetreatCostTool(a) != null;
        }

        /// <summary>v141: a working Tool on the Pokémon that's discarded instead of paying the Retreat Cost (Balloon Berry), or null.</summary>
        public CardInstance RetreatCostTool(PokemonInPlay mon)
        {
            if (mon == null) return null;
            foreach (var src in SourcesOn(mon))
                if (src.Kind == EffectSourceKind.Trainer && src.Ctx.Card != null && mon.Tools.Contains(src.Ctx.Card) && src.Script.ReplacesRetreatCost(src.Ctx))
                    return src.Ctx.Card;
            return null;
        }

        public bool CanAttackThisTurn(PlayerState p)
        {
            var a = p.Active;
            if (a == null) return false;
            // Asleep or Paralyzed, unless Windup Arm lets it attack (v51) or an attack can be used while Asleep (Dream Waltz, v112).
            if (ConditionStopsAttack(a) && !UsableAttacks(a).Any()) return false;
            if (AttacksPrevented(a)) return false;
            // The first player can't attack on their first turn, except with "If you go first, you can use this attack during your first turn".
            // A format can allow it (custom formats, v123).
            if (State.TurnNumber == 1 && !Rules.FirstTurnAttack) return UsableAttacks(a).Any(x => UsableOnFirstTurn(a, x));
            return true;
        }

        // ------------------------------------------------------------------ Carrying out actions

        private bool ExecuteMainAction(int player, DecisionOption opt)
        {
            var p = State.Players[player];
            switch (opt.Action)
            {
                case ActionType.BenchBasic:
                {
                    var card = FindInHand(p, opt.CardId);
                    p.Hand.Remove(card);
                    var bottom = card.Def.IsLegend ? LegendBottomInHand(p, card.Def) : null;
                    if (bottom != null) p.Hand.Remove(bottom);
                    var benched = PutOntoBench(player, card, true);
                    // The other half sits under the top half: one Pokémon made of 2 cards.
                    if (bottom != null && benched != null) benched.Stack.Insert(0, bottom);
                    return false;
                }
                case ActionType.Evolve:
                {
                    var card = FindInHand(p, opt.CardId);
                    var target = State.FindPokemon(opt.PokemonUid);
                    p.Hand.Remove(card);
                    EvolvePokemon(target, card, true);
                    return false;
                }
                case ActionType.AttachEnergy:
                {
                    var card = FindInHand(p, opt.CardId);
                    var target = State.FindPokemon(opt.PokemonUid);
                    p.Hand.Remove(card);
                    p.EnergyAttachedThisTurn = true;
                    // Slimy Room (tails): the card is discarded and the attachment for the turn isn't used up (v55).
                    if (!AttachEnergyCard(card, target, true)) p.EnergyAttachedThisTurn = false;
                    else SecondEnergyAttachment(player, target, card);
                    return false;
                }
                case ActionType.PlayTrainer:
                    PlayTrainer(player, FindInHand(p, opt.CardId));
                    return false;
                case ActionType.UseAbility:
                {
                    var mon = State.FindPokemon(opt.PokemonUid);
                    // The card the Ability is printed on: the Pokémon's top card, the card under a LV.X (its Poké-Powers), or
                    // an attached Tool that gives it an Ability (Sky Seal Stone).
                    // v126: or the card whose Powers it has (a Shapeshift Evolution card, or the Pokémon a Ditto is a copy of).
                    var inheritedCard = InheritedPowersCard(mon);
                    var sourceCard = mon.Stack.FirstOrDefault(c => c.Id == opt.CardId) ?? mon.Tools.FirstOrDefault(c => c.Id == opt.CardId)
                                     ?? (inheritedCard != null && inheritedCard.Id == opt.CardId ? inheritedCard : null) ?? mon.Top;
                    var ability = sourceCard.Def.Abilities[opt.Index];
                    string kind = ability.IsAbility ? "Ability" : ability.Kind;
                    Emit(GameEventType.UseAbility, player, OwnedName(mon) + " used its " + (ability.VstarPower ? "VSTAR Power " : "") + kind + " " + ability.Name + ".", mon.Top.Id, mon.Uid);
                    // A VSTAR Power Ability is used up as soon as it's used (v49).
                    if (ability.VstarPower) p.VstarUsed = true;
                    PushSource(ability.IsAbility ? EffectSourceKind.Ability : EffectSourceKind.PokemonPower, player, mon);
                    // Parasect's Allergic Pollen (v141): an old-style Pokémon Power can't take cards out of discard piles.
                    var powerShield = ability.Kind == AbilityDef.PokemonPower ? BeginDiscardShield() : null;
                    try { Scripts.For(sourceCard.Def).UseAbility(new EffectContext(this, player, sourceCard, mon), opt.Index); }
                    finally { EndDiscardShield(powerShield); PopSource(); }
                    // v90: an Active Pokémon that left play through its own Ability (Wishiwashi's Cowardice) is replaced right away;
                    // with no Benched Pokémon the player has none left in play and loses (like a Fossil, v68).
                    if (p.Active == null && p.Bench.Count > 0) PromoteActive(player);
                    else if (!p.HasPokemonInPlay) CheckWinConditions();
                    return false;
                }
                case ActionType.UseStadium:
                    return UseStadium(player);
                case ActionType.Retreat:
                    DoRetreat(player);
                    return false;
                case ActionType.Attack:
                {
                    // Normally the Active Pokémon; a Benched one for attacks usable from the Bench.
                    var user = State.FindPokemon(opt.PokemonUid);
                    if (user == null || user.Owner != player) user = p.Active;
                    DoAttack(player, user, opt.Card, opt.Index);
                    return true;
                }
                case ActionType.EndTurn:
                    return true;
                case ActionType.UseHandAbility:
                {
                    var card = FindInHand(p, opt.CardId);
                    Emit(GameEventType.UseAbility, player, PlayerName(player) + " used " + card.Def.Name + "'s Ability " + card.Def.Abilities[0].Name + " from " + Their(player) + " hand.", card.Id);
                    PushSource(card.Def.AbilitySourceKind, player, null);
                    try { Scripts.For(card.Def).UseFromHand(new EffectContext(this, player, card, null)); }
                    finally { PopSource(); }
                    return false;
                }
                case ActionType.UseFromDiscard:
                {
                    var card = p.Discard.FirstOrDefault(c => c.Id == opt.CardId);
                    if (card == null) return false;
                    Emit(GameEventType.Info, player, PlayerName(player) + " used " + card.Def.Name + " from " + Their(player) + " discard pile.", card.Id);
                    // Ho-Oh V's Reviving Flame (v50) is an Ability used from the discard pile; Grant is a Trainer.
                    PushSource(card.Def.IsPokemon ? card.Def.AbilitySourceKind : EffectSourceKind.Trainer, player, null);
                    try { Scripts.For(card.Def).UseFromDiscard(new EffectContext(this, player, card, null)); }
                    finally { PopSource(); }
                    return false;
                }
                case ActionType.CombineVUnion:
                    CombineVUnion(player, opt.CardId);
                    return false;
                case ActionType.FlipFaceDown:
                {
                    var mon = State.FindPokemon(opt.PokemonUid);
                    if (mon != null && mon.Owner == player) FlipFaceDown(mon);
                    if (p.Active == null && p.Bench.Count > 0) PromoteActive(player);
                    else if (!p.HasPokemonInPlay) CheckWinConditions();
                    return false;
                }
                case ActionType.DiscardFossil:
                {
                    var mon = State.FindPokemon(opt.PokemonUid);
                    if (mon != null && mon.Def.IsPokeDoll) PutPokemonOnDeckBottom(mon);
                    else if (mon != null && mon.Def.IsFossil) DiscardPokemonFromPlay(mon);
                    // v68: one that left the Active Spot is replaced right away (no Knock Out, so nothing else promotes);
                    // with no Benched Pokémon the player has none left in play and loses.
                    if (p.Active == null && p.Bench.Count > 0) PromoteActive(player);
                    else if (!p.HasPokemonInPlay) CheckWinConditions();
                    return false;
                }
                default:
                    throw new InvalidOperationException("Unknown action " + opt.Action);
            }
        }

        private CardInstance FindInHand(PlayerState p, int cardId)
        {
            foreach (var c in p.Hand) if (c.Id == cardId) return c;
            throw new InvalidOperationException("Card " + cardId + " is not in " + PlayerName(p.Index) + "'s hand.");
        }

        /// <summary>
        /// v138: the opponent's Pokémon Power stops the Trainer card the player just played (Slowking's Mind Games). The card did
        /// nothing: a Supporter or Stadium stopped this way doesn't use up the player's Supporter or Stadium for the turn.
        /// </summary>
        private bool OpponentStopsTrainer(int player, CardInstance card)
        {
            foreach (var src in Sources())
            {
                if (src.Ctx.Player == player || !Overrides(src.Script, "StopsOpponentsTrainer") || !StillInPlay(src)) continue;
                bool stopped;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { stopped = src.Script.StopsOpponentsTrainer(src.Ctx, player, card); }
                finally { PopSource(); }
                if (stopped) return true;
            }
            return false;
        }

        private void PlayTrainer(int player, CardInstance card)
        {
            var p = State.Players[player];
            p.Hand.Remove(card);
            // v152: both halves of a Legendary Stadium are played together as 1 Stadium card.
            var partner = card.Def.IsStadiumHalf ? StadiumOtherHalf(p, card) : null;
            if (partner != null) p.Hand.Remove(partner);
            ResolvePlayedTrainer(player, card, "");
            if (partner == null) return;
            if (State.Stadium == card) { State.StadiumPartner = partner; return; }
            // It didn't stay in play (discarded instead, or put on top of the deck): the other half goes with it.
            var owner = State.Players[card.Owner];
            int at = owner.Deck.IndexOf(card);
            if (at >= 0) owner.Deck.Insert(at + 1, partner);
            else owner.Discard.Add(partner);
        }

        /// <summary>v152: the other half of a Legendary Stadium in the hand (same name, the other half), or null.</summary>
        public CardInstance StadiumOtherHalf(PlayerState p, CardInstance half)
        {
            if (half == null || !half.Def.IsStadiumHalf) return null;
            return p.Hand.FirstOrDefault(x => x != half && x.Def.IsStadiumHalf && x.Def.Name == half.Def.Name && x.Def.LegendHalf != half.Def.LegendHalf);
        }

        /// <summary>
        /// Dream Ball (v58): an Item card the player may play although it isn't in their hand (it was just taken as a face-down
        /// Prize card). Effects that stop the player from playing Item cards still stop it.
        /// </summary>
        public bool CanPlayItemFromOutsideHand(int player, CardInstance card)
        {
            if (card == null || !card.Def.IsItem || TrainerBlocked(player, card)) return false;
            return State.Players[player].ItemLockTurn != State.TurnNumber;
        }

        /// <summary>Plays an Item card that isn't in the hand (Dream Ball from the Prize cards). It goes to the discard pile as usual.</summary>
        public void PlayItemFromOutsideHand(int player, CardInstance card, string from)
        {
            ResolvePlayedTrainer(player, card, from);
        }

        private void ResolvePlayedTrainer(int player, CardInstance card, string from)
        {
            var p = State.Players[player];
            var d = card.Def;
            Emit(GameEventType.PlayTrainer, player, PlayerName(player) + " played " + d.Name + (string.IsNullOrEmpty(from) ? "" : " " + from) + ".", card.Id);

            // Quaking Fist: "If tails, that card is discarded instead." A Supporter or Stadium discarded this way wasn't
            // played, so it doesn't use up your Supporter or Stadium for the turn (your ruling).
            if (ForcesTrainerFlip(player) && !FlipCoin(player, "Flip to play " + d.Name + " (Quaking Fist, Dizzying Wind)"))
            {
                p.Discard.Add(card);
                Emit(GameEventType.Discard, player, d.Name + " was discarded instead of being used.", card.Id);
                return;
            }
            // Chaos Gym (v126): tails, the player can't play it; their opponent may use it instead.
            if (ChaosGymStops(player, card)) return;
            // Slowking's Mind Games (v138): heads, the card does nothing and goes on top of its owner's deck.
            if (OpponentStopsTrainer(player, card)) return;

            var script = Scripts.For(d);
            var ctx = new EffectContext(this, player, card, null);

            if (d.IsStadium)
            {
                p.StadiumPlayedThisTurn = true;
                PutStadiumIntoPlay(player, card);
                return;
            }

            if (d.IsTool)
            {
                var toolSide = script.AttachesToOpponentsPokemon ? State.Players[1 - player] : p;
                var targets = toolSide.InPlay().Where(m => CanAttachToolTo(card, m)).ToList();
                var holder = ChoosePokemon(player, targets, "Attach " + d.Name + " to which Pokémon?", d);
                AttachTool(card, holder);
                if (holder.Owner != player) return; // Pokémon Tool F on the opponent's Pokémon: none of their "you attach" effects
                holder.ToolFromHandOnTurn = State.TurnNumber; // Mienshao's Whip Expert
                // "When you attach a Pokémon Tool from your hand to this Pokémon" (Tapu Lele's Charmed Charm, v81).
                foreach (var src in SourcesOn(holder))
                {
                    if (src.Kind != EffectSourceKind.Ability || State.FindPokemon(holder.Uid) != holder) continue;
                    PushSource(src.Kind, holder.Owner, holder);
                    try { src.Script.OnToolAttachedFromHand(src.Ctx, card, holder); }
                    finally { PopSource(); }
                }
                return;
            }

            if (d.IsSupporter)
            {
                p.SupporterPlayedThisTurn = true;
                p.SupporterNameThisTurn = d.Name;
                p.SupporterThisTurn = d;
                p.SupportersPlayedCount++;
                p.SupportersThisTurn.Add(d.Name);
            }
            PushSource(EffectSourceKind.Trainer, player, null);
            var outerTrainer = ResolvingTrainer;
            ResolvingTrainer = d;
            // Shiftry's Shifty Substitution (v62): a Supporter played from the hand only draws 3 cards.
            bool drawThree = d.IsSupporter && string.IsNullOrEmpty(from) && SupportersDrawThree(player);
            try
            {
                if (drawThree)
                {
                    Emit(GameEventType.Info, player, d.Name + " has the effect \"Draw 3 cards\" (Shifty Substitution).", card.Id);
                    DrawCards(player, 3);
                }
                else script.ResolveTrainer(ctx);
            }
            finally
            {
                ResolvingTrainer = outerTrainer;
                PopSource();
                if (ctx.ReturnToDeck)
                {
                    p.Deck.Add(card);
                    ShuffleDeck(player);
                    Emit(GameEventType.Shuffle, player, d.Name + " was shuffled into " + Their(player) + " deck.", card.Id);
                }
                else if (ctx.ReturnToDeckBottom)
                {
                    p.Deck.Add(card);
                    Emit(GameEventType.Info, player, d.Name + " was put on the bottom of " + Their(player) + " deck.", card.Id);
                }
                else if (!ctx.CardPlacedElsewhere) p.Discard.Add(card);
            }
        }

        /// <summary>
        /// Primarina's Harmonics (v81): after the Energy attachment for the turn, the player may attach a second Energy card from
        /// their hand while something allows it. Both cards go on the same Pokémon (your ruling, v81).
        /// </summary>
        private void SecondEnergyAttachment(int player, PokemonInPlay target, CardInstance first)
        {
            var p = State.Players[player];
            if (target == null || State.FindPokemon(target.Uid) != target || target.Owner != player || !CanAttachFromHand(target)) return;
            bool allowed = false;
            string source = "Harmonics";
            Func<CardDef, bool> which = null;
            foreach (var src in Sources())
                if (src.Ctx.Player == player && src.Script.AllowsSecondEnergyAttachment(src.Ctx)) { allowed = true; break; }
            // α Growth (v101): attaching an Energy card from the hand to this Pokémon for the turn, you may attach 1 more to it.
            if (!allowed && HasAncientTrait(target, AlphaGrowth)) { allowed = true; source = "α Growth"; }
            // Blaine (v126, your ruling): for the rest of the turn, a Fire Energy card attached to a Blaine's Pokémon lets a second one follow.
            if (!allowed)
                foreach (var fx in ActiveEffects())
                {
                    which = fx.SecondEnergyAttachment(this, player, target, first);
                    if (which == null) continue;
                    allowed = true;
                    source = fx.Name;
                    break;
                }
            if (allowed) AttachAnotherFromHand(player, target, source, which);
            // v144: Venusaur's Harvest Bounty (Expedition): each Pokémon Power that offers it lets 1 more Energy card follow onto
            // the Active Pokémon (your ruling: each Venusaur adds 1).
            foreach (var src in Sources())
            {
                if (State.IsOver || State.FindPokemon(target.Uid) != target || !CanAttachFromHand(target)) return;
                if (src.Ctx.Player != player || !StillInPlay(src) || !src.Script.OffersExtraEnergyAttachment(src.Ctx, target)) continue;
                string power = src.Ctx.Card != null && src.Ctx.Card.Def.Abilities.Count > 0 ? src.Ctx.Card.Def.Abilities[0].Name : "Extra attachment";
                PushSource(src.Kind, player, src.Ctx.Pokemon);
                try
                {
                    if (AttachAnotherFromHand(player, target, power, null)) src.Script.OnExtraEnergyAttachmentUsed(src.Ctx);
                }
                finally { PopSource(); }
            }
        }

        /// <summary>Offers 1 more Energy card from the hand onto the same Pokémon. True when one was attached.</summary>
        private bool AttachAnotherFromHand(int player, PokemonInPlay target, string source, Func<CardDef, bool> which)
        {
            var p = State.Players[player];
            var cards = p.Hand.Where(c => c.Def.IsEnergy && !EnergyCardBlocked(player, c) && (which == null || which(c.Def))).ToList();
            if (cards.Count == 0) return false;
            var opts = cards.Select(c => new DecisionOption { Label = "Attach " + c.Def.Name + " to " + target.Name, CardId = c.Id, Card = c.Def }).ToList();
            opts.Add(new DecisionOption { Label = "Don't attach another Energy" });
            int pick = AskOne(player, DecisionKind.ChooseOption, source + ": attach another Energy card from your hand to " + target.Name + "?", opts, null, false);
            if (pick < 0 || pick >= cards.Count) return false;
            var card = cards[pick];
            if (!p.Hand.Remove(card)) return false;
            return AttachEnergyCard(card, target, true);
        }

        private void DoRetreat(int player)
        {
            var p = State.Players[player];
            // Lt. Surge's Secret Plan (v126): a face-down card is flipped over when it retreats.
            if (!FlipBeforeRetreat(player)) return;
            var active = p.Active;
            // Umbreon's Pursuit (v139): "if the Defending Pokémon tries to retreat, do 10 damage to it", before anything is paid.
            // Knocked Out by it, the Pokémon doesn't retreat (the Knock Out is handled after the action).
            foreach (var e in ActiveEffects().ToList())
            {
                if (!State.Effects.Contains(e) || State.IsOver) continue;
                e.OnRetreatTried(this, active);
            }
            // v142: Unown [C]'s [Chase] (a Pokémon Power in play) reacts the same way.
            foreach (var src in Sources())
            {
                if (State.IsOver || p.Active != active) break;
                if (!StillInPlay(src)) continue;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnActiveRetreatTried(src.Ctx, active); }
                finally { PopSource(); }
            }
            if (State.IsOver || p.Active != active || IsKnockedOut(active) || p.Bench.Count == 0) return;
            int cost = RetreatCostOf(active);
            var newActive = ChoosePokemon(player, p.Bench, "Choose the Pokémon to switch in", null);
            // Goodra's Slimy Sliding: flip a coin; on tails nothing is discarded and the Pokémon don't switch.
            foreach (var src in Sources())
            {
                // v147: a Stadium (Mirage Stadium) makes every player flip, its owner too.
                if ((src.Ctx.Player == player && src.Kind != EffectSourceKind.Stadium) || !src.Script.ForcesRetreatFlip(src.Ctx, player)) continue;
                if (!FlipCoin(player, src.Ctx.Card != null ? src.Ctx.Card.Def.Name : "Retreat"))
                {
                    p.RetreatedThisTurn = true;
                    Emit(GameEventType.Info, player, PlayerName(player) + " couldn't retreat " + active.Name + " (tails).", active.Top.Id, active.Uid);
                    return;
                }
                break; // the effect doesn't stack: one flip
            }
            // Balloon Berry (v141): discarded instead of the Energy, whatever the cost (your ruling: even a cost of 0).
            var berry = RetreatCostTool(active);
            if (berry != null)
            {
                Emit(GameEventType.Info, player, berry.Def.Name + " was discarded instead of paying the Retreat Cost.", berry.Id, active.Uid);
                DiscardTool(active, berry);
            }
            else if (cost > 0) PayRetreatCost(player, active, cost);
            p.RetreatedThisTurn = true;
            active.RetreatedOnTurn = State.TurnNumber;
            Emit(GameEventType.Retreat, player, PlayerName(player) + " retreated " + active.Name + " and switched in " + newActive.Name + ".", active.Top.Id, active.Uid);
            SwitchActive(player, newActive);
            // "When this Pokémon retreats" Abilities (Imakuni?'s Doduo's Frenzied Escape, v92), once it's on the Bench.
            if (!State.IsOver && p.Bench.Contains(active) && HasWorkingAbilityOrPower(active))
            {
                PushSource(active.Def.AbilitySourceKind, player, active);
                try { Scripts.For(active.Def).OnRetreated(new EffectContext(this, player, active.Top, active)); }
                finally { PopSource(); }
            }
            // "Whenever your opponent's Active Pokémon retreats" (Dark Dugtrio's Sinkhole, v121).
            if (!State.IsOver && p.Bench.Contains(active))
                foreach (var src in Sources())
                {
                    if (src.Ctx.Pokemon == null || State.IsOver || !p.Bench.Contains(active)) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnPokemonRetreated(src.Ctx, active); }
                    finally { PopSource(); }
                }
        }

        private void PayRetreatCost(int player, PokemonInPlay active, int cost)
        {
            PushSource(EffectSourceKind.Rule, player, active);
            try
            {
                var sources = Sources();
                // Skaters' Park (v55): basic Energy that would be discarded goes to the hand instead.
                string toHand = null;
                foreach (var src in sources)
                    if (src.Script.RetreatEnergyToHand(src.Ctx, player)) { toHand = src.Ctx.Card != null ? src.Ctx.Card.Def.Name : "Retreat"; break; }
                // Dashing Pouch (v86): every Energy card, basic or not, goes to the hand.
                string anyToHand = null;
                foreach (var src in sources)
                    if (src.Script.RetreatAnyEnergyToHand(src.Ctx, active)) { anyToHand = src.Ctx.Card != null ? src.Ctx.Card.Def.Name : "Retreat"; break; }
                bool simple = active.Energy.All(e => EnergyUnitsOf(active, e, sources).Count == 1);
                if (simple)
                {
                    var energyOpts = active.Energy.Select(e => new DecisionOption { Label = e.Def.Name, CardId = e.Id, Card = e.Def }).ToList();
                    var picks = AskMany(player, DecisionKind.ChooseCards, "Choose " + cost + " Energy to discard for the Retreat Cost", energyOpts, cost, cost);
                    var toDiscard = picks.Select(i => active.Energy[i]).ToList();
                    foreach (var e in toDiscard) PayRetreatEnergy(player, active, e, toHand, anyToHand);
                    return;
                }
                // Some attached cards provide more than 1 Energy: discard one card at a time until the cost is covered.
                int remaining = cost;
                while (remaining > 0 && active.Energy.Count > 0)
                {
                    var pick = ChooseCards(player, active.Energy.ToList(), 1, 1, "Choose an Energy to discard for the Retreat Cost (" + remaining + " still to pay)", null)[0];
                    int units = Math.Max(1, EnergyUnitsOf(active, pick).Count);
                    PayRetreatEnergy(player, active, pick, toHand, anyToHand);
                    remaining -= units;
                }
            }
            finally
            {
                PopSource();
            }
        }

        /// <summary>One Energy card paid for the Retreat Cost: discarded, or put into the hand when it's basic and toHand names the card that says so.</summary>
        private void PayRetreatEnergy(int player, PokemonInPlay active, CardInstance energy, string toHand, string anyToHand)
        {
            string why = anyToHand ?? (energy.Def.IsBasicEnergy ? toHand : null);
            if (why != null && active.Energy.Remove(energy))
            {
                State.Players[energy.Owner].Hand.Add(energy);
                Emit(GameEventType.Info, player, energy.Def.Name + " from " + active.Name + " went to " + Their(player) + " hand instead of the discard pile (" + why + ").", energy.Id, active.Uid);
                return;
            }
            DiscardEnergy(active, energy);
        }

        private bool UseStadium(int player)
        {
            var stadium = State.Stadium;
            Emit(GameEventType.Info, player, PlayerName(player) + " used " + stadium.Def.Name + ".", stadium.Id);
            PushSource(EffectSourceKind.Stadium, player, null);
            try { return Scripts.For(stadium.Def).UseStadium(new EffectContext(this, player, stadium, null)); }
            finally { PopSource(); }
        }

        // ------------------------------------------------------------------ Attacking

        /// <summary>Uses an attack (source = the card the attack is printed on), then a second attack if Festival Lead or Fluffy Barrage allows it.</summary>
        private void DoAttack(int player, PokemonInPlay attacker, CardDef source, int index)
        {
            var p = State.Players[player];
            if (source == null) source = attacker.Def;
            if (!PerformAttack(player, attacker, source, index)) return;

            if (State.IsOver) return;
            // An Active Pokémon that left play during its own attack (Aqua Return, Breezy Gift) is replaced right away,
            // before Knock Outs are checked, so its owner promotes before the opponent does (Compendium: Haze Slash).
            if (p.Active == null && p.Bench.Count > 0) PromoteActive(player);
            ResolveKnockouts();
            if (p.Active != attacker || !AttacksTwice(attacker)) return;
            if (State.Players[1 - player].Active == null) return;
            if (ConditionStopsAttack(attacker) || AttacksPrevented(attacker)) return;
            var usable = UsableAttacks(attacker);
            if (usable.Count == 0) return;
            var opts = usable.Select(a => new DecisionOption
            {
                Label = "Attack again: " + a.Attack.Name,
                Action = ActionType.Attack, PokemonUid = attacker.Uid, CardId = attacker.Top.Id, Card = a.Source, Index = a.Index
            }).ToList();
            opts.Add(new DecisionOption { Label = "Don't attack again", Action = ActionType.EndTurn });
            int pick = AskOne(player, DecisionKind.ChooseOption, attacker.Name + ": attack a second time?", opts, attacker.Def, false);
            if (opts[pick].Action != ActionType.Attack) return;
            PerformAttack(player, attacker, opts[pick].Card, opts[pick].Index);
        }

        /// <summary>Festival Lead and similar: the Pokémon's own Ability lets it attack twice.</summary>
        private bool AttacksTwice(PokemonInPlay attacker)
        {
            // Ω Barrage (v101): an Ancient Trait, so nothing that blocks Abilities stops it.
            if (HasAncientTrait(attacker, OmegaBarrage)) return true;
            foreach (var src in SourcesOn(attacker))
                if ((src.Kind == EffectSourceKind.Ability || src.Kind == EffectSourceKind.PokemonPower) && src.Script.AttacksTwice(src.Ctx)) return true;
            return false;
        }

        /// <summary>The attack being resolved right now, or null (Celesteela-GX's Force Canceler checks for a GX attack; v73).</summary>
        public AttackDef ResolvingAttack { get; private set; }

        /// <summary>One attack: Confusion check, the attack's effects, then Boomerang Energy. Returns false if Confusion stopped it.</summary>
        private bool PerformAttack(int player, PokemonInPlay attacker, CardDef source, int index)
        {
            var atk = source.Attacks[index];
            var defending = State.Players[1 - player].Active;
            string fromBench = State.IsActive(attacker) ? "" : " from the Bench";
            // The announcement doesn't name a target (v55.1): many attacks hit a Benched Pokémon or none at all (Cruel Arrow,
            // snipes, searches), so "on [the Active Pokémon]" was misleading. The damage lines that follow say who was hit.
            Emit(GameEventType.Attack, player, OwnedName(attacker) + " used " + atk.Name + fromBench + "!", attacker.Top.Id, attacker.Uid);
            // Baby Rule (Neo era, v48): "If this Baby Pokémon is your Active Pokémon and your opponent announces an attack, your
            // opponent flips a coin (before doing anything else). If tails, your opponent's turn ends." So it comes before Confusion.
            if (defending != null && defending.Def.IsBaby && !FlipCoin(player, "Baby Rule (" + defending.Name + ")"))
            {
                Emit(GameEventType.Info, player, "Baby Rule: tails, so the turn ends without " + OwnedName(attacker) + "'s attack.", defending.Top.Id, defending.Uid);
                return false;
            }
            // Rulebook "Full details of attacking" (your ruling, v53): B. effects that might cancel the attack (Sand Attack,
            // Smokescreen) come before C. the Confusion check. This replaces the old 2016 Compendium order (Confusion first).
            // Sand Attack: "if the Defending Pokémon tries to use an attack, your opponent flips a coin. If tails, that attack doesn't happen."
            int attackCoins = 0;
            foreach (var e in ActiveEffects()) attackCoins = Math.Max(attackCoins, e.AttackFlipCoins(this, attacker));
            if (attackCoins > 0 && FlipCoins(player, attackCoins, attacker.Name + " tries to attack") < attackCoins)
            {
                Emit(GameEventType.Info, player, OwnedName(attacker) + "'s attack didn't happen.", attacker.Top.Id, attacker.Uid);
                return false;
            }
            // C. Confusion: only if the attack wasn't cancelled.
            if (attacker.Has(SpecialCondition.Confused) && !FlipCoin(player, attacker.Name + " is Confused"))
            {
                // Dark Primeape's Frenzy (v121, your ruling): 3 more counters ("even to itself").
                int confusionCounters = attacker.ConfusionCounters;
                foreach (var src in SourcesOn(attacker)) confusionCounters = src.Script.ModifyConfusionCounters(src.Ctx, attacker, confusionCounters);
                PushSource(EffectSourceKind.Rule, -1, null);
                try { PlaceDamageCounters(attacker, confusionCounters, "Confusion"); }
                finally { PopSource(); }
                return false;
            }

            // A GX attack is used up only once it actually happens (not when Confusion or Sand Attack stops it).
            if (IsGxAttack(atk)) State.Players[player].GxUsed = true;
            // Same for a VSTAR Power attack (v49): used up once it happens.
            if (atk.VstarPower)
            {
                State.Players[player].VstarUsed = true;
                Emit(GameEventType.Info, player, PlayerName(player) + " used " + Their(player) + " VSTAR Power (only 1 per game).", attacker.Top.Id, attacker.Uid);
            }
            // Lt. Surge's Secret Plan (v126): the attacking player needs to know what a face-down Defending Pokémon is, so it's
            // flipped over now; one that's discarded is replaced before the attack goes on.
            FlipDefendingFaceDown(player, attacker);
            returnAfterAttack.Clear();
            if (attacksResolving == 0) pendingDamageTriggers.Clear();
            // Blunder Policy (v54): the Tools on the attacker when it attacks; they still count if it's Knocked Out (your ruling).
            var toolsWhenAttacking = SourcesOn(attacker).Where(s => s.Kind == EffectSourceKind.Trainer).ToList();
            // v122: "whenever a player attacks with..." (Vermilion City Gym).
            foreach (var startSrc in Sources())
            {
                if (State.IsOver) break;
                PushSource(startSrc.Kind, startSrc.Ctx.Player, startSrc.Ctx.Pokemon);
                try { startSrc.Script.OnAttackStarting(startSrc.Ctx, attacker, atk); }
                finally { PopSource(); }
            }
            // v126: timed effects that react to an attack being announced (Koga's Ninja Trick).
            foreach (var startFx in ActiveEffects().ToList())
            {
                if (State.IsOver) break;
                if (!State.Effects.Contains(startFx)) continue;
                PushSource(EffectSourceKind.Trainer, startFx.Owner, null);
                try { startFx.OnAttackStarting(this, attacker, atk); }
                finally { PopSource(); }
            }
            int tailsBefore = attackTailsFlipped;
            var previousAttack = ResolvingAttack;
            ResolvingAttack = atk;
            PushSource(EffectSourceKind.Attack, player, attacker);
            attacksResolving++;
            attackSerial++;
            // Marowak's Bodyguard (v94): the attack can't do anything to the shielded opponent's hand.
            var handGuard = PlayerShieldedFromAttacks(1 - player) ? BeginHandGuard(1 - player) : null;
            // Parasect's Allergic Pollen (v141): the attack can't take cards out of discard piles.
            var discardShield = BeginDiscardShield();
            try
            {
                var ctx = new EffectContext(this, player, attacker.Top, attacker, source == attacker.Def ? null : source);
                Scripts.For(source).ResolveAttack(ctx, index);
            }
            finally
            {
                EndDiscardShield(discardShield);
                if (handGuard != null) EndHandGuard(handGuard);
                attacksResolving--;
                PopSource();
                ResolvingAttack = previousAttack;
                // Recorded after the attack, so while it resolves LastAttack* still describe the previous attack.
                attacker.LastAttackName = atk.Name;
                attacker.LastAttackTurn = State.TurnNumber;
                var me = State.Players[player];
                me.LastAttackName = atk.Name;
                me.LastAttackTurn = State.TurnNumber;
                me.LastAttackerDef = attacker.Def;
                me.LastAttackerUid = attacker.Uid;
                me.LastAttackSourceDef = source;
                me.LastAttackIndex = index;
            }

            // Then the Defending side's "if this Pokémon is damaged by an attack" triggers (Compendium order of operations).
            if (attacksResolving == 0) FlushDamageTriggers();
            // v122: after the attack (Vermilion City Gym's tails).
            if (attacksResolving == 0)
                foreach (var endSrc in Sources())
                {
                    if (State.IsOver) break;
                    PushSource(endSrc.Kind, endSrc.Ctx.Player, endSrc.Ctx.Pokemon);
                    try { endSrc.Script.OnAttackFinished(endSrc.Ctx, attacker, atk); }
                    finally { PopSource(); }
                }

            if (attackTailsFlipped > tailsBefore)
            {
                foreach (var tool in toolsWhenAttacking)
                {
                    PushSource(EffectSourceKind.Trainer, player, attacker);
                    try { tool.Script.OnHolderAttackFlippedTails(tool.Ctx); }
                    finally { PopSource(); }
                }
            }

            if (returnAfterAttack.Count > 0)
            {
                var pending = returnAfterAttack.ToList();
                returnAfterAttack.Clear();
                foreach (var kv in pending)
                {
                    var mon = kv.Key;
                    var energy = kv.Value;
                    var discard = State.Players[energy.Owner].Discard;
                    if (State.FindPokemon(mon.Uid) != mon || !discard.Contains(energy)) continue;
                    discard.Remove(energy);
                    AttachEnergyCard(energy, mon);
                }
            }
            return true;
        }
    }
}
