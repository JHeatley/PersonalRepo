using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>Picks uniformly at random among legal options. Great for fuzzing the rules engine.</summary>
    public sealed class RandomAgent : IPlayerAgent
    {
        private readonly Rng rng;
        /// <summary>Chance (0-100) to end the turn when other main actions are available.</summary>
        public int EndTurnPercent = 10;

        public RandomAgent(int seed)
        {
            rng = new Rng(seed);
        }

        public List<int> Decide(DecisionRequest r)
        {
            if (r.Kind == DecisionKind.MainAction && r.Options.Count > 1)
            {
                int endIdx = r.Options.FindIndex(o => o.Action == ActionType.EndTurn);
                if (endIdx >= 0 && rng.Next(100) >= EndTurnPercent)
                {
                    int pick = rng.Next(r.Options.Count - 1);
                    if (pick >= endIdx) pick++;
                    return new List<int> { pick };
                }
            }
            int count = r.MinChoices + rng.Next(r.MaxChoices - r.MinChoices + 1);
            var idx = Enumerable.Range(0, r.Options.Count).ToList();
            rng.Shuffle(idx);
            return idx.Take(count).ToList();
        }
    }

    /// <summary>
    /// A decision answered by scores: the agent takes the Count highest-scoring options. Learning agents add
    /// their learned adjustments to Scores before the pick (see LearningAgent).
    /// </summary>
    public sealed class ScoredChoice
    {
        /// <summary>Score per option, in the order of DecisionRequest.Options; higher is more wanted.</summary>
        public double[] Scores;
        /// <summary>How many options to take (clamped to the request's MinChoices..MaxChoices when picking).</summary>
        public int Count = 1;
        /// <summary>Single choices break exact ties at random; multi-picks and fixed answers keep the listed order on ties.</summary>
        public bool Jitter;
    }

    /// <summary>
    /// Rule-of-thumb bot that can pilot any deck built from the card pool. Known cards get specific
    /// handling; anything else falls back to generic rules (energy by attack cost, bench Basics, attack
    /// for the most damage). It's the "classic bot"; LearningAgent builds on its scores and adjusts them
    /// with what it learns from games.
    ///
    /// Main-action scores: setup actions score 5-95 and happen first; attacks score about 3-4 so they
    /// come last (attacking ends the turn); harmful plays score below 1 (End turn) and are skipped.
    /// </summary>
    public sealed class HeuristicAgent : IPlayerAgent
    {
        private readonly Rng rng;

        public HeuristicAgent(int seed)
        {
            rng = new Rng(seed);
        }

        public List<int> Decide(DecisionRequest r)
        {
            return Pick(r, Score(r));
        }

        /// <summary>Scores every option of the request (and how many to take) without choosing yet.</summary>
        public ScoredChoice Score(DecisionRequest r)
        {
            var c = new ScoredChoice();
            c.Scores = new double[r.Options.Count];
            var v = r.View;
            switch (r.Kind)
            {
                case DecisionKind.MainAction:
                    Fill(c, r, o => ScoreMain(o, v), true);
                    break;
                case DecisionKind.GoFirstOrSecond:
                case DecisionKind.MulliganDraws:
                case DecisionKind.ChooseNumber:
                    Fixed(c, 0);
                    break;
                case DecisionKind.YesNo:
                    Fixed(c, SayYes(r) ? 0 : 1);
                    break;
                case DecisionKind.SetupActive:
                    Fill(c, r, o => StartingActiveScore(o.Card), true);
                    break;
                case DecisionKind.SetupBench:
                    Fill(c, r, o => BenchScore(o.Card), false);
                    c.Count = r.MaxChoices;
                    break;
                case DecisionKind.PromoteActive:
                    Fill(c, r, o => PromoteScore(v.FindPokemon(o.PokemonUid), v), true);
                    break;
                case DecisionKind.ChoosePokemon:
                    if (r.MaxChoices > 1) ScoreManyPokemon(c, r);
                    else ScorePokemon(c, r);
                    break;
                case DecisionKind.ChooseCards:
                    ScoreCards(c, r);
                    break;
                case DecisionKind.ChooseOption:
                    // Evolutions (v92): Conversion 3 makes the Defending Pokémon weak to the type of our own Pokémon.
                    if ((r.Prompt ?? "").StartsWith("Conversion 3")) Fill(c, r, o => ConversionScore(o, v), true);
                    else Fill(c, r, o => OptionScore(o, v), true);
                    break;
                default:
                    Fixed(c, 0);
                    break;
            }
            return c;
        }

        /// <summary>Takes the Count best options by these scores (Scores may have been adjusted since Score()).</summary>
        public List<int> Pick(DecisionRequest r, ScoredChoice c)
        {
            return Pick(r, c.Scores, c.Count, c.Jitter);
        }

        public List<int> Pick(DecisionRequest r, double[] scores, int count, bool jitter)
        {
            count = Math.Max(r.MinChoices, Math.Min(count, r.MaxChoices));
            if (jitter && count == 1)
            {
                int best = 0;
                double bestScore = double.MinValue;
                for (int i = 0; i < scores.Length; i++)
                {
                    double s = scores[i] + rng.Next(100) * 0.00001; // tiny jitter breaks ties
                    if (s > bestScore)
                    {
                        bestScore = s;
                        best = i;
                    }
                }
                return One(best);
            }
            return TopIndices(scores, count);
        }

        /// <summary>Indices of the count highest scores; equal scores keep the listed order.</summary>
        public static List<int> TopIndices(double[] scores, int count)
        {
            return Enumerable.Range(0, scores.Length).OrderBy(i => -scores[i]).Take(Math.Max(0, count)).ToList();
        }

        private static List<int> One(int i)
        {
            return new List<int> { i };
        }

        private static void Fill(ScoredChoice c, DecisionRequest r, Func<DecisionOption, double> score, bool jitter)
        {
            for (int i = 0; i < r.Options.Count; i++) c.Scores[i] = score(r.Options[i]);
            c.Jitter = jitter;
            c.Count = 1;
        }

        /// <summary>A fixed answer: option i, nothing else considered.</summary>
        private static void Fixed(ScoredChoice c, int pick)
        {
            for (int i = 0; i < c.Scores.Length; i++) c.Scores[i] = i == pick ? 1 : 0;
            c.Jitter = false;
            c.Count = 1;
        }

        // ---------------------------------------------------------- Main actions

        private double ScoreMain(DecisionOption o, PlayerView v)
        {
            var me = v.Me;
            var name = o.Card == null ? "" : o.Card.Name;
            switch (o.Action)
            {
                case ActionType.Evolve:
                    return o.Card.Stage == Stage.Stage2 ? 95 : 90;
                case ActionType.BenchBasic:
                    if (me.Bench.Count >= 4 && BenchScore(o.Card) < 80) return 5;
                    return BenchScore(o.Card) - 10;
                case ActionType.AttachEnergy:
                    return AttachScore(o, v);
                case ActionType.PlayTrainer:
                    return TrainerScore(o.Card, v);
                case ActionType.UseAbility:
                    // Irresistible Force can be used any number of times: only the Active Hisuian Arcanine V pulls Energy in,
                    // so two of them never pass Energy back and forth.
                    if (o.Card.Abilities[o.Index].Name == "Irresistible Force")
                        return v.Me.Active != null && v.Me.Active.Uid == o.PokemonUid ? 60 : -1;
                    // Lost Origin: Moisture Star (once per game) only heals a badly hurt Hisuian Goodra VSTAR.
                    if (o.Card.Abilities[o.Index].Name == "Moisture Star")
                    {
                        var goodra = v.FindPokemon(o.PokemonUid);
                        return goodra != null && goodra.DamageCounters >= 12 ? 85 : -1;
                    }
                    // Unbroken Bonds: Building Spite only while Spiritomb can take another damage counter safely; Metal Eater
                    // only on a badly hurt Melmetal.
                    if (o.Card.Abilities[o.Index].Name == "Building Spite")
                    {
                        var tomb = v.FindPokemon(o.PokemonUid);
                        return tomb != null && tomb.RemainingHp > 20 ? 40 : -1;
                    }
                    if (o.Card.Abilities[o.Index].Name == "Metal Eater")
                    {
                        var melmetal = v.FindPokemon(o.PokemonUid);
                        return melmetal != null && melmetal.DamageCounters >= 6 ? 60 : -1;
                    }
                    // SM promos (v91): Porygon-Z-GX's Troubleshooting throws away a Special Energy, so only to save a badly hurt one.
                    if (o.Card.Abilities[o.Index].Name == "Troubleshooting")
                    {
                        var porygon = v.FindPokemon(o.PokemonUid);
                        return porygon != null && porygon.DamageCounters >= 8 ? 60 : -1;
                    }
                    return AbilityScore(o.Card, o.Card.Abilities[o.Index].Name, v);
                case ActionType.UseStadium:
                    return StadiumScore(o.Card, v);
                case ActionType.Retreat:
                    return ShouldRetreat(v) ? 45 : -1;
                case ActionType.UseHandAbility:
                    // Lost Thunder (v81): Trumbeak's Mountain Pass puts the Trumbeak itself in the Lost Zone.
                    if (o.Label != null && o.Label.Contains("Mountain Pass")) return 25;
                    // Celestial Storm (v83): Minior's Falling Star swaps it in for the Active Pokémon, so only when retreating would be right.
                    if (o.Label != null && o.Label.Contains("Falling Star")) return ShouldRetreat(v) ? 60 : -1;
                    return 70;
                // SWSH promos: a Pokémon V-UNION is a free 3-Prize attacker, so it's combined as soon as it can be.
                case ActionType.CombineVUnion:
                    return 88;
                case ActionType.UseFromDiscard:
                    // Gengar's Netherworld Gate puts it onto the Bench for free (with 3 damage counters).
                    // Empoleon's Emergency Surfacing (only with an empty hand) is a free Benched Pokémon and 3 cards.
                    if (o.Label != null && o.Label.Contains("Emergency Surfacing")) return 70;
                    // Hidden Fates: Darkrai-GX's Restoration is a free Benched Pokémon with a Darkness Energy.
                    if (o.Label != null && o.Label.Contains("Restoration")) return 60;
                    // Lost Thunder (v81): Giratina's Distortion Door is a free Benched Pokémon and 2 damage counters.
                    if (o.Label != null && o.Label.Contains("Distortion Door")) return 65;
                    return o.Label != null && o.Label.Contains("Netherworld Gate") ? 50 : 0;
                case ActionType.Attack:
                    var atk = o.Card.Attacks[o.Index];
                    if (atk.Name == "Trading Places" || atk.Name == "Tuck Tail" || atk.Name == "Run Around") return 3;
                    if (atk.Name == "Destined Fight") return DestinedFightWorthIt(v) ? 4.4 : 1.5;
                    return 3.5 + Math.Min(500, EstimateDamage(o.Card, o.Index, AttackerView(o, v), v)) / 1000.0;
                case ActionType.EndTurn:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>The Pokémon using this attack option: the Active one, or a Benched one for attacks usable from the Bench.</summary>
        internal static PokemonView AttackerView(DecisionOption o, PlayerView v)
        {
            var user = o.PokemonUid >= 0 ? v.FindPokemon(o.PokemonUid) : null;
            int me = v.Viewer < 0 ? 0 : v.Viewer;
            return user != null && user.Owner == me ? user : v.Me.Active;
        }

        private static double AbilityScore(CardDef card, string ability, PlayerView v)
        {
            switch (ability)
            {
                case "Run Away Draw": return v.Me.HandCount <= 3 ? 70 : -1;
                case "Trade": return v.Me.HandCount >= 2 ? 82 : -1;
                case "Subjugating Chains": return ChainsWorthIt(v) ? 60 : -1;
                case "Teleporter": return -1;
                case "Cursed Blast":
                {
                    // Only worth a Prize card of our own when it Knocks Out something.
                    int damage = card != null && card.Name == "Dusknoir" ? 130 : 50;
                    var opp = v.Opponent;
                    var all = new List<PokemonView>(opp.Bench);
                    if (opp.Active != null) all.Add(opp.Active);
                    return all.Any(p => p.RemainingHp <= damage) ? 84 : -1;
                }
                case "Lunar Cycle": return v.Me.HandCount <= 6 ? 80 : -1;
                case "Changing Seasons": return 60;
                case "Torrential Heart":
                {
                    // Only right before attacking with Feraligatr itself.
                    var a = v.Me.Active;
                    return a != null && a.Top.Def.Name == "Feraligatr" && a.RemainingHp > 60 && CanAttackNow(a) ? 90 : -1;
                }
                case "Run Errand":
                case "Aerial Draw": return 86;
                // Destined Rivals: no-gain loops (Rocket Brain can be used any number of times) and card-losing trades.
                case "Rocket Brain":
                case "Life Shaker":
                case "Reconstitute":
                case "Flustered Leap": return -1;
                // White Flare: only refresh a big hand; only swap Active Pokémon when retreating would be right anyway.
                case "Distorted Future": return v.Opponent.HandCount >= 6 ? 75 : -1;
                case "Torrential Whirlpool": return ShouldRetreat(v) ? 60 : -1;
                // Mega Evolution era: Solar Transfer can be used any number of times (no-gain loop); Evidence Gathering only reshuffles.
                case "Solar Transfer":
                case "Evidence Gathering": return -1;
                // SWSH promos: Greninja V-UNION's Feel the Way only reveals the opponent's hand, which the bots can't use.
                case "Feel the Way": return -1;
                // Cosmic Eclipse: Blinking Lights can be used as often as you like and Bulldoze / Scampering Tail only rearrange a
                // deck (no gain for the bots); Whirlpool Suction gives up Phione and the opponent picks; Growing Up and Sleepy
                // Voice end the turn (only when there's nothing better); Call Signal Knocks Out Magneton (a Prize card for the
                // opponent), so only without a Supporter in hand and with Prize cards to spare.
                case "Blinking Lights":
                case "Bulldoze":
                case "Scampering Tail":
                case "Whirlpool Suction": return -1;
                case "Growing Up":
                case "Sleepy Voice": return 2;
                case "Call Signal":
                    return !v.Me.Hand.Any(c => c.Def.IsSupporter) && v.Opponent.PrizeCount >= 3 && v.Me.Bench.Count >= 2 ? 45 : -1;
                case "Fragrant Flower Garden":
                    return (v.Me.Active != null && v.Me.Active.DamageCounters > 0) || v.Me.Bench.Any(b => b.DamageCounters > 0) ? 70 : -1;
                case "Victory Heal": return v.Me.Bench.Any(b => b.DamageCounters > 0) ? 60 : -1;
                case "Flaming Shot": return 60;
                case "Spiritborne Evolution": return v.Me.HandCount >= 6 ? 45 : -1;
                case "Violent Appetite":
                    return (v.Me.Active != null && v.Me.Active.Top.Def.Name == "Naganadel & Guzzlord-GX" && v.Me.Active.DamageCounters >= 6)
                           || v.Me.Bench.Any(b => b.Top.Def.Name == "Naganadel & Guzzlord-GX" && b.DamageCounters >= 6) ? 55 : -1;
                case "Disk Reload": return v.Me.HandCount <= 3 ? 85 : -1;
                case "Excited Turbo": return 80;
                case "Excited Dash":
                case "Sky Transport": return ShouldRetreat(v) ? 60 : -1;
                case "Flaring Magic": return v.Me.HandCount <= 4 ? 80 : -1;
                case "Nighttime Maneuvers": return 50;
                // Ascended Heroes: only gather Energy onto an Active Azumarill ex; Alluring Wings helps the opponent too.
                case "Bubble Gathering": return v.Me.Active != null && v.Me.Active.Top.Def.Name == "Azumarill ex" ? 70 : -1;
                case "Alluring Wings": return v.Me.HandCount <= 3 ? 55 : -1;
                // Perfect Order: Wash Out can be used any number of times (it would strip the Bench); Grand Wing only against a big hand.
                case "Wash Out": return -1;
                case "Grand Wing": return v.Opponent.HandCount >= 5 ? 70 : -1;
                // Classic Collection: Restructure lets the opponent pick your new Active; Ultra Road only to retreat;
                // Intrepid Sword ends the turn (only when there's nothing better); Starbirth is once per game.
                case "Restructure": return -1;
                case "Ultra Road": return ShouldRetreat(v) ? 60 : -1;
                case "Intrepid Sword": return 2;
                case "Starbirth": return v.Me.HandCount <= 3 ? 70 : -1;
                // Crown Zenith: a VSTAR Power is once per game, so only when it pays off; Abilities that end the turn come
                // after everything else (attacks score above 2).
                case "Star Order":
                {
                    var mine = v.Me.Active;
                    var theirs = v.Opponent.Active;
                    bool worth = mine != null && theirs != null && mine.Top.Def.IsBasicPokemon && mine.Top.Def.IsPokemonV && CanAttackNow(mine)
                                 && (theirs.Top.Def.IsVstar || theirs.Top.Def.IsVmax);
                    return worth ? 85 : -1;
                }
                case "Ivy Star": return v.Opponent.Bench.Count > 0 && v.Me.Active != null && CanAttackNow(v.Me.Active) ? 40 : -1;
                case "Conversion Star": return v.Me.HandCount <= 2 ? 40 : -1;
                case "Shield Star": return -1;
                case "Star Abyss": return 50;
                case "Moon Cleave Star": return 55;
                case "Phantom Star": return v.Me.HandCount <= 2 ? 70 : -1;
                case "Star Portal": return 70;
                case "Star Guardian": return 90;
                case "Instant Charge":
                case "Regal Stance":
                case "Roar of the Sword": return 2;
                case "Azure Pulse": return v.Me.HandCount <= 2 ? 70 : -1;
                case "Industrious Incisors": return v.Me.HandCount <= 4 ? 85 : -1;
                // Silver Tempest: the VSTAR Powers only when they pay off; Refinement needs a card to spare.
                case "Painful Spoons": return 40;
                case "Star Rondo": return v.Me.Active != null && !CanAttackNow(v.Me.Active) && v.Opponent.Bench.Count > 0 ? 45 : -1;
                case "Legacy Star": return v.Me.HandCount <= 3 && v.Me.DeckCount >= 15 ? 60 : -1;
                case "Summoning Star": return 75;
                case "Star Alchemy": return v.Me.HandCount <= 3 ? 60 : -1;
                case "Refinement": return v.Me.HandCount >= 2 && v.Me.HandCount <= 6 ? 70 : -1;
                case "Heat Boost": return v.Me.Active != null && v.Me.Active.Top.Def.HasType(EnergyType.Fire) && CanAttackNow(v.Me.Active) ? 45 : -1;
                // Lost Origin: Vanishing Wings would only shuffle Pidgeot V away; Puppet Offering gives up the Banette, so only
                // without a Supporter in hand; Seething Currents against a big hand, or to refresh a small one of ours.
                case "Vanishing Wings": return -1;
                case "Puppet Offering": return v.Me.Hand.Any(c => c.Def.IsSupporter) ? -1 : 50;
                case "Seething Currents": return v.Opponent.HandCount >= 5 || v.Me.HandCount <= 2 ? 50 : -1;
                case "Hazard Star": return v.Opponent.Active != null ? 70 : -1;
                // Pokémon GO: Vitality Spring ends the turn, so only when there's nothing better (attacks score above 2).
                case "Vitality Spring": return 2;
                // Astral Radiance: the VSTAR Powers only when they pay off; Concealed Cards and Supernatural Orb cost an Energy
                // card; Apple Drop shuffles Flapple away, so only for a Knock Out; Metal Transfer is a no-gain loop for the bots.
                case "Star Perfume": return v.Me.DeckCount >= 10 ? 65 : -1;
                case "Star of Fortune": return v.Me.HandCount <= 3 ? 70 : -1;
                case "Concealed Cards": return v.Me.Hand.Count(c => c.Def.IsEnergy) >= 2 || v.Me.HandCount <= 3 ? 70 : -1;
                case "Supernatural Orb": return 60;
                case "Wily Stance": return v.Me.HandCount <= 5 ? 75 : -1;
                case "Buddy Catch": return v.Me.Hand.Any(c => c.Def.IsSupporter) ? -1 : 80;
                case "Back Order": return 50;
                case "Ancient Wisdom": return 80;
                case "Ice Dance": return 55;
                case "Metal Transfer": return -1;
                case "Apple Drop":
                {
                    var opp = v.Opponent;
                    var all = new List<PokemonView>(opp.Bench);
                    if (opp.Active != null) all.Add(opp.Active);
                    return all.Any(p => p.RemainingHp <= 20) ? 84 : -1;
                }
                // Brilliant Stars: Star Bloom is the VSTAR Power; Special Transfer and Primate Wisdom are no-gain loops for the
                // bots; Dream Gift ends the turn (only when there's nothing better); Roaring Resolve costs 2 damage counters.
                case "Sun-Drenched Shell": return 70;
                case "Star Bloom": return 45;
                case "Special Transfer":
                case "Primate Wisdom": return -1;
                case "Roaring Resolve": return 40;
                case "Rapid Strike Search": return 70;
                case "Single Strike Roar": return 50;
                case "Dream Gift": return 2;
                // Fusion Strike: Double Gunner costs a Water Energy card from the hand.
                case "Double Gunner": return 60;
                // Celebrations: Energy Trans is a no-gain loop for the bots; Imprison only matters against Poké-Powers and
                // Poké-Bodies; Cosmic Power refills a small hand; Rush In only when the Active Pokémon can't attack.
                case "Energy Trans": return -1;
                case "Imprison": return OpponentHasOldPowers(v) ? 60 : -1;
                case "Cosmic Power": return v.Me.HandCount <= 4 ? 70 : -1;
                case "Rush In": return v.Me.Active != null && !CanAttackNow(v.Me.Active) ? 50 : -1;
                // Evolving Skies: Greening Cells ends the turn (only when there's nothing better); Rapid Strike Connection is a
                // no-gain loop for the bots and Phantom Transformation needs judgement they don't have; Intimidating Roar only
                // sends away an undamaged Active Pokémon.
                case "Greening Cells": return 2;
                case "Cotton Lift": return v.Me.DeckCount > 0 ? 70 : -1;
                case "Rapid Strike Connection":
                case "Phantom Transformation": return -1;
                case "Intimidating Roar": return v.Opponent.Active != null && v.Opponent.Active.DamageCounters == 0 && v.Opponent.Bench.Count > 0 ? 40 : -1;
                case "Quick Shooting": return 80;
                // Chilling Reign: Mysterious Potion's coin and Buzzap Generator's Prize card need judgement the bots don't have;
                // Gormandize ends the turn (only with a small hand and nothing better); Witch Rondo also moves the opponent's
                // Active Pokémon, so only when ours should leave.
                case "Curative Forest": return 60;
                case "Witch Rondo": return ShouldRetreat(v) ? 50 : -1;
                case "Mysterious Potion":
                case "Buzzap Generator": return -1;
                case "Gormandize": return v.Me.HandCount <= 3 ? 2 : -1;
                // Battle Styles: Stance Change needs judgement the bots don't have.
                case "Spring Bloom": return 55;
                case "Ear Moves": return 50;
                case "Stance Change": return -1;
                // Shining Fates: V Transformation and Shuffle Dance need judgement the bots don't have, and Bug's Radar is a
                // no-gain shuffle for them; Mind Hat costs a card of ours too, Evolution Roar two; Delicious Aroma may bring up
                // something we don't want Active.
                case "V Transformation":
                case "Shuffle Dance":
                case "Bug's Radar": return -1;
                case "Mind Hat": return v.Opponent.HandCount >= 4 && v.Me.HandCount >= 3 ? 40 : -1;
                case "Evolution Roar": return v.Me.HandCount >= 4 && !v.Me.Hand.Any(c => c.Def.Name == "Galarian Perrserker") ? 45 : -1;
                case "Make Do": return v.Me.HandCount >= 2 && v.Me.HandCount <= 6 ? 70 : -1;
                case "Tea Break": return 70;
                case "Delicious Aroma": return 30;
                // Darkness Ablaze: Bright Heal only when one of ours is hurt; Rapid-Fire Poison only against a healthy Active.
                case "Bright Heal":
                    return (v.Me.Active != null && v.Me.Active.DamageCounters > 0) || v.Me.Bench.Any(b => b.DamageCounters > 0) ? 60 : -1;
                case "Rapid-Fire Poison":
                case "Poisonous Puddle":
                    return v.Opponent.Active != null && (v.Opponent.Active.Conditions & SpecialCondition.Poisoned) == 0 ? 75 : -1;
                // Hidden Fates: Extra Energy Bomb Knocks Out Electrode-GX (2 Prize cards for the opponent), so only with plenty
                // of Energy in the discard pile, a Pokémon that isn't a Pokémon-GX or Pokémon-EX to take it, and the opponent
                // nowhere near winning; Shady Move needs judgement the bots don't have; Instruct waits until the hand is played out;
                // Breath of the Leaves only when one of ours with Energy is hurt.
                case "Extra Energy Bomb":
                {
                    var me = v.Me;
                    bool target = (me.Active != null && !me.Active.Top.Def.IsGXorEX) || me.Bench.Any(b => !b.Top.Def.IsGXorEX);
                    return target && me.Discard.Count(c => c.Def.IsEnergy) >= 4 && v.Opponent.PrizeCount >= 3 ? 55 : -1;
                }
                case "Shady Move": return -1;
                case "Instruct": return 25; // after the hand has been played out (it's only offered with 2 or fewer cards)
                // Unified Minds: Peppy Pick and Snack Search end the turn (only when there's nothing better to do); Shadow
                // Connection can be used any number of times and Ominous Posture needs judgement the bots don't have; Tag
                // Transport only when the Active TAG TEAM should leave; Grind Up only discards the opponent's Stadium.
                case "Peppy Pick":
                case "Snack Search": return 2;
                case "Shadow Connection":
                case "Ominous Posture": return -1;
                case "Tag Transport": return ShouldRetreat(v) ? 50 : -1;
                case "Grind Up":
                    return v.StadiumOwner != v.Viewer && v.Me.Hand.Any(c => c.Def.IsBasicEnergy && (c.Def.EnergyProvides == EnergyType.Fire || c.Def.EnergyProvides == EnergyType.Metal)) ? 60 : -1;
                case "Ultra Conversion": return v.Me.DeckCount > 6 ? 50 : -1;
                case "Breath of the Leaves":
                    return (v.Me.Active != null && v.Me.Active.Energy.Count > 0 && v.Me.Active.DamageCounters >= 3)
                           || v.Me.Bench.Any(b => b.Energy.Count > 0 && b.DamageCounters >= 3) ? 70 : -1;
                // Unbroken Bonds: Bratty Kick, Excitable Draw and Playhouse Heal end the turn (only when there's nothing better
                // to do); Mysterious Message Knocks Out Mismagius (a Prize card for the opponent), so only with a tiny hand and
                // the opponent far from winning; Resetting Hole discards Marshadow, so only for the opponent's Stadium;
                // Irresistible Aroma puts the opponent's Basic Pokémon in play for them.
                case "Pupate":
                case "Emerge": return 70;
                case "Varied Pollen": return 60;
                case "Round 'n' Round": return 70;
                case "Roast Reveal": return v.Me.HandCount <= 5 && v.Me.DeckCount > 3 ? 55 : -1;
                case "Irresistible Aroma": return 30;
                case "Mysterious Message": return v.Me.HandCount <= 2 && v.Me.DeckCount > 7 && v.Opponent.PrizeCount >= 4 ? 20 : -1;
                case "Resetting Hole": return v.StadiumOwner >= 0 && v.StadiumOwner != v.Viewer ? 30 : -1;
                case "Bratty Kick": return 2;
                case "Excitable Draw": return v.Me.HandCount <= 3 && v.Me.DeckCount > 6 ? 2 : -1;
                case "Playhouse Heal":
                    return (v.Me.Active != null && v.Me.Active.DamageCounters >= 6) || v.Me.Bench.Any(b => b.DamageCounters >= 6) ? 2 : -1;
                case "Fairy Feast": return 60;
                case "Crazy Code": return 80;
                case "Cat Walk": return 80;
                // Team Up (v80): Scar Charge costs 3 damage counters; Dance of the Ancients sends Tapu Koko ◇ to the Lost Zone;
                // Stellar Wish puts Jirachi to sleep, so only with a Pokémon to switch to; Nine Temptations costs 2 Fire Energy
                // cards; Fast Call only without a Supporter in hand.
                case "Scar Charge": return 40;
                case "Dance of the Ancients": return 55;
                case "Stellar Wish": return v.Me.Bench.Count > 0 && v.Me.DeckCount > 5 ? 50 : -1;
                case "Nine Temptations": return v.Opponent.Bench.Count > 0 ? 35 : -1;
                case "Fast Call": return v.Me.Hand.Any(c => c.Def.IsSupporter) ? -1 : 75;
                case "Air Mail": return 70;
                case "Powerful Squall": return 75;
                case "Queen's Call": return 70;
                case "Nuzzly Gathering": return 60;
                // Lost Thunder (v81): the Unown win the game; Blazing Energy turns every Energy into Fire for the turn, which
                // can stop non-Fire attacks, so only with a Fire Active Pokémon; Phantom Winds shuffles a Pokémon-GX away;
                // Sprint discards the hand; Unseen Flash and Vessel of Life cost cards; Frozen Flow gives the opponent a free
                // switch of their choice.
                case "DAMAGE":
                case "HAND":
                case "MISSING": return 999;
                case "Blazing Energy": return v.Me.Active != null && v.Me.Active.Top.Def.PokemonType == EnergyType.Fire ? 30 : -1;
                case "Phantom Winds": return -1;
                case "Sprint": return v.Me.HandCount <= 3 && v.Me.DeckCount > 6 ? 65 : -1;
                case "Unseen Flash": return 45;
                case "Vessel of Life": return 35;
                case "Frozen Flow": return 30;
                case "Crushing Charge": return v.Me.DeckCount > 8 ? 70 : -1;
                case "Go for a Swim": return 50;
                case "Fluffy Pillow": return 75;
                // Dragon Majesty (v82): Weed Out discards Benched Pokémon (left to people); Downpour discards Water Energy cards
                // for Riptide, so only with Feraligatr Active.
                case "Weed Out": return -1;
                case "Downpour": return v.Me.Active != null && v.Me.Active.Top.Def.Name == "Feraligatr" ? 40 : -1;
                // Celestial Storm (v83): Power Draw costs a card, Swing Dance only draws; Excavate and Smooth Over only rearrange the
                // deck blind (left to people); Fragrance Trap and Dragon Wind switch the opponent's Active Pokémon.
                case "Power Draw": return v.Me.HandCount >= 2 && v.Me.DeckCount > 6 ? 75 : -1;
                case "Swing Dance": return v.Me.DeckCount > 6 ? 86 : -1;
                case "Excavate":
                case "Smooth Over": return -1;
                case "Fragrance Trap":
                case "Dragon Wind": return 50;
                // Forbidden Light (v84): Jet Geyser costs a Water Energy card; Wondrous Gift only puts an Item back on the deck.
                case "Jet Geyser": return v.Opponent.Bench.Count > 0 ? 40 : -1;
                case "Wondrous Gift": return 45;
                case "Mystical Torch": return 70;
                // Ultra Prism (v85): Invasion swaps Dawn Wings Necrozma-GX in for the Active Pokémon (only when retreating would
                // be right); Change Clothes and Psychic Transfer only move cards around (left to people: Psychic Transfer can be
                // used again and again).
                case "Invasion": return ShouldRetreat(v) ? 60 : -1;
                case "Change Clothes":
                case "Psychic Transfer": return -1;
                case "Illuminate": return v.Me.DeckCount > 5 ? 70 : -1;
                // Crimson Invasion (v86): Escape shuffles Starmie away (left to people); Shell On evolves Karrablast for a Shelmet.
                case "Escape": return -1;
                case "Shell On": return 75;
                // Shining Legends (v87): Energy Reload only feeds an Active Shining Genesect; Heat Cyclone lets the opponent pick
                // their new Active Pokémon; Plunge takes the Active Pokémon's Energy, so only when retreating would be right.
                case "Energy Reload": return v.Me.Active != null && v.Me.Active.Top.Def.Name == "Shining Genesect" ? 60 : -1;
                case "Heat Cyclone": return 50;
                case "Plunge": return ShouldRetreat(v) ? 55 : -1;
                // Burning Shadows (v88): Dark Invitation benches a Basic from the opponent's hand with 3 counters on it; Honey Gather
                // finds basic Energy.
                case "Dark Invitation": return 70;
                case "Honey Gather": return v.Me.DeckCount > 6 ? 60 : -1;
                // Guardians Rising (v89): Slush Rush draws; Schooling swaps in Wishiwashi-GX; Geotech System feeds the Active Pokémon.
                case "Slush Rush": return v.Me.DeckCount > 5 ? 80 : -1;
                case "Schooling": return 75;
                case "Geotech System": return 70;
                // Sun & Moon (v90): Strong Charge finds 2 Energy; Curative Pollen heals 20; Search the Premises only looks at the
                // opponent's hand; Cowardice throws away Wishiwashi's attached cards (left to people).
                case "Strong Charge": return v.Me.DeckCount > 3 ? 85 : -1;
                case "Curative Pollen": return 60;
                case "Search the Premises": return 40;
                case "Cowardice": return -1;
                // SM promos (v91): Ascension DNA evolves Eevee-GX and heals it; Hydrating Drops heals the Active Water Pokémon; Power
                // Suction moves an Energy onto Deoxys.
                case "Ascension DNA": return 75;
                case "Hydrating Drops": return 60;
                case "Power Suction": return 40;
                // Evolutions (v92): Space Beacon trades a card for 2 basic Energy; Buzzap Thunder gives up a Prize card (left to people).
                case "Space Beacon": return v.Me.Discard.Count(c => c.Def.IsBasicEnergy) >= 2 && v.Me.HandCount >= 3 ? 55 : -1;
                case "Buzzap Thunder": return -1;
                // Steam Siege (v93): Shocking Light puts 3 counters on a Pokémon-EX; Steam Up only helps a Basic Fire attacker this
                // turn; Mega Boost feeds a Mega; Tear Away (any number of times) and Wonder Lock are left to people unless the
                // opponent has an XY-era Mega.
                case "Shocking Light": return 70;
                case "Steam Up":
                    return v.Me.Active != null && v.Me.Active.Top.Def.IsBasicPokemon && v.Me.Active.Top.Def.HasType(EnergyType.Fire) && CanAttackNow(v.Me.Active) ? 45 : -1;
                case "Mega Boost": return 70;
                case "Tear Away": return -1;
                case "Wonder Lock": return v.Opponent.Active != null && v.Opponent.Active.Top.Def.IsXYMegaEvolution ? 50 : -1;
                // Fates Collide (v94): Flare Witch finds a Fire Energy; Restoring Beam benches a Restored Pokémon; Dangerous Tentacle
                // pulls up a Pokémon-EX; Wormadam First and Drive Change are left to people.
                case "Flare Witch": return v.Me.DeckCount > 3 ? 70 : -1;
                case "Restoring Beam": return v.Me.Bench.Count < 4 && v.Me.DeckCount > 3 ? 60 : -1;
                case "Dangerous Tentacle": return 50;
                case "Wormadam First":
                case "Drive Change":
                    return -1;
                // Generations (v95): Victory Kiss heals the Active Pokémon; Shadow Ear moves a damage counter onto the opponent;
                // Flash Fire pulls a Fire Energy onto Flareon-EX.
                case "Victory Kiss": return 50;
                case "Shadow Ear": return 55;
                case "Flash Fire": return 60;
                // BREAKpoint (v96): Royal Flash moves an Energy off the opponent's Active Pokémon; Purifying Fire heals Ho-Oh-EX; Hyper
                // Transfer (any number of times) and Goodnight, Babies (puts your own Active Pokémon to sleep too) are left to people.
                case "Royal Flash": return 45;
                case "Purifying Fire": return 70;
                case "Hyper Transfer":
                case "Goodnight, Babies":
                    return -1;
                // BREAKthrough (v97): Energy Color flips for an Energy from the deck; Abyssal Hand draws up to 5; Floral Breeze heals
                // the Active Pokémon; Premonition, Stand In and Second Coat need judgement and are left to people.
                case "Energy Color": return v.Me.DeckCount > 3 ? 60 : -1;
                case "Abyssal Hand": return v.Me.DeckCount > 3 ? 65 : -1;
                case "Floral Breeze": return 55;
                case "Premonition":
                case "Stand In":
                case "Second Coat":
                    return -1;
                // Ancient Origins (v98): Bright Heal is the Darkness Ablaze case above; Poisonous Nest only when the opponent's Active
                // Pokémon isn't Grass; Farewell Letter and Magnetic Warp are left to people.
                case "Poisonous Nest":
                    return v.Opponent.Active != null && !v.Opponent.Active.Top.Def.HasType(EnergyType.Grass) && v.Opponent.Active.Conditions == SpecialCondition.None
                           && (v.Me.Active == null || v.Me.Active.Top.Def.HasType(EnergyType.Grass)) ? 50 : -1;
                case "Farewell Letter":
                case "Magnetic Warp":
                    return -1;
                // Roaring Skies (v99): Turboblaze attaches a Fire Energy from the hand to a Dragon Pokémon; Wing Buzz (a card from the
                // hand for 1 card off the opponent's deck) is left to people.
                case "Turboblaze": return 60;
                case "Wing Buzz": return -1;
                // Double Crisis (v100): Burning Draft and Aqua Search; Magma Switch is left to people.
                case "Burning Draft": return 60;
                case "Aqua Search": return v.Me.DeckCount > 3 ? 55 : -1;
                case "Magma Switch": return -1;
                // Primal Clash (v101): Nurture and Heal and Captivating Rhythm; Diving Search, Energy Connect and Sand Flap are left
                // to people.
                case "Nurture and Heal": return 60;
                case "Captivating Rhythm": return 40;
                case "Diving Search":
                case "Energy Connect":
                case "Sand Flap":
                    return -1;
                // Phantom Forces (v102): Metal Links, Dark Impulse and Tasting; Flare Command is left to people.
                case "Metal Links": return 60;
                case "Dark Impulse": return 60;
                case "Tasting": return v.Me.DeckCount > 3 ? 60 : -1;
                case "Flare Command": return -1;
                // Furious Fists (v103): Wafting Scent Confuses and Poisons; Teleport Room is left to people.
                case "Wafting Scent": return 50;
                case "Teleport Room": return -1;
                // Flashfire (v104): Leaf Draw draws 3; Energy Grace (Knocks Milotic Out), Shadow Void, Gooey Regeneration and Big
                // Jump are left to people.
                case "Leaf Draw": return v.Me.DeckCount > 5 ? 60 : -1;
                case "Energy Grace":
                case "Shadow Void":
                case "Gooey Regeneration":
                case "Big Jump":
                    return -1;
                // XY (v105): Mystical Fire with a small hand, Water Shuriken, Upside-Down Evolution and Drive Off; Fairy Transfer is
                // left to people.
                case "Mystical Fire": return v.Me.HandCount <= 3 && v.Me.DeckCount > 6 ? 70 : -1;
                case "Water Shuriken": return 60;
                case "Upside-Down Evolution": return 80;
                case "Drive Off": return 40;
                case "Fairy Transfer": return -1;
                // XY promos (v107): Voice of the Sands and Aroma of Gratitude.
                case "Voice of the Sands": return 40;
                case "Aroma of Gratitude": return 60;
                // Legendary Treasures (v108): Dark Trance is left to people.
                case "Dark Trance": return -1;
                // Plasma Blast (v109): Tool Reversal, Plasma Transfer and Sinister Hand are left to people.
                case "Tool Reversal":
                case "Plasma Transfer":
                case "Sinister Hand":
                    return -1;
                // Plasma Freeze (v110): Flare Navigate, Magnetic Draw and Plasma Search fetch or draw; Six Feet Under (Knocks Out your
                // own Cofagrigus) and Diving Draw (needs a card worth discarding) are left to people.
                case "Flare Navigate": return v.Me.DeckCount > 3 ? 50 : -1;
                case "Magnetic Draw": return v.Me.HandCount < 4 && v.Me.DeckCount > 4 ? 55 : -1;
                case "Plasma Search": return v.Me.DeckCount > 3 ? 45 : -1;
                case "Six Feet Under":
                case "Diving Draw":
                    return -1;
                // Plasma Storm (v111): Fair-Weather Heal, Night Sight and Moon Guidance help; Big Wing needs judgement (Lunatone's
                // Premonition shares Gallade's case above).
                case "Fair-Weather Heal": return 60;
                case "Night Sight": return v.Me.DeckCount > 3 ? 50 : -1;
                case "Moon Guidance": return v.Me.DeckCount > 3 ? 55 : -1;
                case "Big Wing": return -1;
                // Fossil (v116): Step In only when the Active Pokémon can't attack; Curse and Strange Behavior are left to people
                // (Cowardice already is, above).
                case "Step In": return v.Me.Active != null && !CanAttackNow(v.Me.Active) ? 50 : -1;
                case "Curse":
                case "Strange Behavior":
                    return -1;
                // Jungle (v114): Shift needs a plan (its type stays) and Peek only shows a card: left to people. Heal is used.
                case "Shift":
                case "Peek":
                    return -1;
                // Base Set (v113): Damage Swap moves counters back and forth and Buzzap gives up a Prize card: left to people.
                case "Damage Swap":
                case "Buzzap":
                    return -1;
                // Boundaries Crossed (v112): Long-Distance Hypnosis can put your own Active to sleep and Transform needs a plan: people.
                case "Long-Distance Hypnosis":
                case "Transform":
                    return -1;
                default: return 88;
            }
        }

        /// <summary>The opponent has a Pokémon in play with a Poké-Power or Poké-Body (Gardevoir ex's Imprison).</summary>
        private static bool OpponentHasOldPowers(PlayerView v)
        {
            var opp = v.Opponent;
            var all = new List<PokemonView>(opp.Bench);
            if (opp.Active != null) all.Add(opp.Active);
            return all.Any(p => !p.Notes.Any(n => n.StartsWith("Imprison")) && p.Top.Def.Abilities.Any(a => a.IsPokePowerOrBody));
        }

        /// <summary>Stadium actions ("once during each player's turn").</summary>
        private static double StadiumScore(CardDef stadium, PlayerView v)
        {
            if (stadium == null) return 0;
            switch (stadium.Name)
            {
                case "Team Rocket's Factory": return 80;
                case "Community Center": return 40;
                // Primal Clash (v101): Rough Seas heals your Water and Lightning Pokémon.
                case "Rough Seas": return 45;
                case "Prism Tower": return v.Me.HandCount >= 5 ? 20 : -1;
                case "Lumiose City":
                    // Ends the turn: only when there's nothing better (attacks score above this).
                    return v.Me.Bench.Count < 3 ? 2 : -1;
                case "Mystery Garden": return 50;
                case "Surfing Beach": return ShouldRetreat(v) ? 55 : -1;
                // Ends the turn when it heals anything: only when there's no attack to make.
                case "Celebratory Fanfare": return 2;
                case "PokéStop": return v.Me.DeckCount >= 20 ? 30 : -1;
                // Jubilife Village ends the turn: only with a nearly empty hand and nothing better to do.
                case "Jubilife Village": return v.Me.HandCount <= 2 ? 2 : -1;
                case "Magma Basin": return 45;
                case "Training Court": return 40;
                // Evolving Skies: Shopping Center needs judgement the bots don't have.
                case "Crystal Cave": return 40;
                case "Stormy Mountains": return v.Me.Bench.Count < 4 ? 45 : -1;
                case "Turffield Stadium": return 45;
                // Battle Styles.
                case "Tower of Darkness": return v.Me.HandCount >= 4 ? 35 : -1;
                // Darkness Ablaze.
                case "Rose Tower": return v.Me.HandCount < 3 && v.Me.DeckCount > 0 ? 70 : -1;
                // SWSH promos: Champions Festival only heals (it's never offered without exactly 6 Pokémon in play and damage to heal).
                case "Champions Festival": return 50;
                // Cosmic Eclipse: Giant Hearth costs a card, so only for a deck with Fire Pokémon in play.
                case "Giant Hearth":
                    return v.Me.HandCount >= 2 && v.Me.DeckCount > 0
                           && ((v.Me.Active != null && v.Me.Active.Top.Def.HasType(EnergyType.Fire)) || v.Me.Bench.Any(b => b.Top.Def.HasType(EnergyType.Fire))) ? 45 : -1;
                // Hidden Fates.
                case "Brooklet Hill": return v.Me.Bench.Count < 4 ? 50 : -1;
                case "Mt. Coronet": return v.Me.HandCount <= 5 ? 45 : -1;
                // Unified Minds: Pokémon Research Lab ends the turn (only when there's nothing better to do).
                case "Pokémon Research Lab": return v.Me.Bench.Count < 4 ? 2 : -1;
                case "Viridian Forest": return v.Me.HandCount >= 3 && v.Me.DeckCount > 0 ? 45 : -1;
                // Lost Thunder (v81): Heat Factory ◇ trades a Fire Energy card for 3 cards; Life Forest ◇ heals a Grass Pokémon.
                case "Heat Factory \u25C7": return v.Me.DeckCount > 5 && v.Me.HandCount <= 6 ? 55 : -1;
                case "Life Forest \u25C7": return 60;
                // BREAKpoint (v96): All-Night Party wakes up and heals your Active Pokémon (only offered while it's Asleep).
                case "All-Night Party": return 50;
                default: return -1;
            }
        }

        private static bool DestinedFightWorthIt(PlayerView v)
        {
            var mine = v.Me.Active;
            var theirs = v.Opponent.Active;
            if (mine == null || theirs == null) return false;
            int theirPrizes = theirs.Top.Def.PrizeValue;
            int ourPrizes = mine.Top.Def.PrizeValue;
            return theirPrizes > ourPrizes && v.Me.Bench.Count > 0;
        }

        /// <summary>Yes/no questions from card effects.</summary>
        private static bool SayYes(DecisionRequest r)
        {
            string p = r.Prompt ?? "";
            var v = r.View;
            if (p.StartsWith("Strange Hacking")) return false;
            // A hand with only Snorlax Doll: the Doll can't attack or retreat, so the bots take the mulligan.
            if (p.StartsWith("Snorlax Doll:")) return false;
            // Backtrack Badge: flip again when the coins came up worse than average.
            if (p.Contains("Ignore the results and flip again?")) return ReflipWorthIt(p);
            if (p.StartsWith("Snow Sink")) return v.StadiumOwner != v.Viewer;
            if (p.StartsWith("Voltaic Fist")) return v.Me.Active != null && v.Me.Active.RemainingHp > 90;
            if (p.StartsWith("Teleportation Burst")) return v.Me.Active != null && v.Me.Active.RemainingHp <= 60;
            // Crown Zenith: Climactic Gate ends the turn (only when there's no attack to make); Psychic Leap gives up Mew V;
            // Crushing Beat only removes the opponent's Stadium.
            if (p.StartsWith("Use Climactic Gate")) return v.Me.Active == null || !CanAttackNow(v.Me.Active);
            if (p.StartsWith("Psychic Leap")) return false;
            if (p.StartsWith("Crushing Beat")) return v.StadiumOwner != v.Viewer;
            // Wafting Heal discards all Energy from the healed Pokémon: only when it's badly hurt and has little Energy.
            if (p.StartsWith("Use Wafting Heal")) return v.Me.Active != null && v.Me.Active.DamageCounters >= 10 && v.Me.Active.Energy.Count <= 1;
            // Destined Rivals: Dynamic Dive hurts Dondozo ex; Assassin's Return gives up all the attached Energy.
            if (p.StartsWith("Dynamic Dive")) return v.Me.Active != null && v.Me.Active.RemainingHp > 50;
            // 30th Celebration: Drifloon's Float Up only saves a Drifloon that's about to be Knocked Out.
            if (p.StartsWith("Float Up")) return v.Me.Active != null && v.Me.Active.RemainingHp <= 30;
            if (p.StartsWith("Assassin's Return")) return false;
            // Lost Origin: Lethargy Spores puts our own Active Pokémon to sleep too.
            if (p.StartsWith("Use Lethargy Spores")) return false;
            // Silver Tempest: Regal Blender asks again after every move (the bots keep their Energy where it is); Callous
            // Wings and Furisode Girl only switch out a Pokémon that should retreat; Aero / Tempest Dive only discard the
            // opponent's Stadium.
            if (p.StartsWith("Regal Blender")) return false;
            if (p.StartsWith("Callous Wings") || p.StartsWith("Furisode Girl")) return ShouldRetreat(v);
            if (p.StartsWith("Aero Dive") || p.StartsWith("Tempest Dive")) return v.StadiumOwner != v.Viewer;
            // Pokémon 151: Spirit Return gives the opponent a Supporter back; worth it only for Gengar's Poltergeist.
            if (p.StartsWith("Spirit Return"))
                return (v.Me.Active != null && v.Me.Active.Top.Def.Name == "Gengar") || v.Me.Bench.Any(b => b.Top.Def.Name == "Gengar");
            // Mega Evolution era: give up Energy only when the base damage isn't enough.
            var theirs = v.Opponent.Active;
            if (p.StartsWith("Riotous Blasting")) return theirs != null && theirs.RemainingHp > 200;
            if (p.StartsWith("Chrono Burst")) return theirs != null && theirs.RemainingHp > 80;
            if (p.StartsWith("Ninja Spinner")) return theirs != null && theirs.RemainingHp > 120;
            if (p.StartsWith("Strong-Arm Lariat")) return theirs != null && theirs.RemainingHp > 100;
            if (p.StartsWith("G-Max Volt Tackle")) return theirs != null && theirs.RemainingHp > 120;
            // Astral Radiance: Parallel Spin returns an Energy to the hand, so only when the 130 isn't enough.
            if (p.StartsWith("Parallel Spin")) return theirs != null && theirs.RemainingHp > 130;
            // Brilliant Stars: Lightning Blast throws away all the Lightning Energy, so only when the 100 isn't enough.
            if (p.StartsWith("Lightning Blast")) return theirs != null && theirs.RemainingHp > 100;
            // Fusion Strike: Punk Shock's Paralysis costs 70 damage to our own Toxtricity; Dig It Up keeps Energy and Pokémon.
            if (p.StartsWith("Punk Shock")) return v.Me.Active != null && v.Me.Active.RemainingHp > 100;
            if (p.StartsWith("Dig It Up")) return !(p.Contains("Energy") || p.Contains("Sandslash") || p.Contains("Sandshrew"));
            // Celebrations: Flame Ball would move Gardevoir ex's own Fire Energy away.
            if (p.StartsWith("Flame Ball")) return false;
            // Evolving Skies: Zapper Kick gives up all the Energy; Electron Crush 3 Energy (only when the 100 isn't enough);
            // Pumpkin Pit only removes the opponent's Stadium; Fumbling Hands only refreshes a small hand.
            if (p.StartsWith("Zapper Kick")) return false;
            if (p.StartsWith("Electron Crush")) return theirs != null && theirs.RemainingHp > 100;
            if (p.StartsWith("Use Pumpkin Pit")) return v.StadiumOwner != v.Viewer;
            if (p.StartsWith("Use Fumbling Hands")) return v.Me.HandCount <= 3;
            // Chilling Reign: Shadow Ripper would send Liepard V and its Energy back to the hand.
            if (p.StartsWith("Shadow Ripper")) return false;
            // Battle Styles: Nature's Judgment gives up all the Energy, so only when the 80 isn't enough.
            // Hidden Fates: Tapu Bulu-GX's version adds 60 to 120.
            if (p.StartsWith("Nature's Judgment") && p.Contains("Tapu Bulu-GX")) return theirs != null && theirs.RemainingHp > 120;
            if (p.StartsWith("Nature's Judgment")) return theirs != null && theirs.RemainingHp > 80;
            // Shining Fates: Iron Wings gives up 2 Energy, so only when Corviknight has plenty.
            if (p.StartsWith("Iron Wings")) return v.Me.Active != null && v.Me.Active.Energy.Count >= 5;
            // Vivid Voltage: Commanding Tail only cuts a big hand down to 4.
            if (p.StartsWith("Commanding Tail")) return v.Opponent.HandCount >= 6;
            // Champion's Path: Float Up would send Eldegoss V and its Energy back into the deck.
            if (p.StartsWith("Float Up")) return false;
            // Darkness Ablaze: Flying Taxi would pick up one of our Pokémon; Frozen Heat throws away all the Water Energy, so
            // only when the 110 isn't enough.
            if (p.StartsWith("Flying Taxi")) return false;
            if (p.StartsWith("Frozen Heat")) return theirs != null && theirs.RemainingHp > 110;
            // Rebel Clash: Mining Rush keeps discarding only while the damage so far doesn't Knock Out and the deck isn't thin.
            if (p.StartsWith("Mining Rush:")) return theirs != null && 30 * CountInParens(p) < theirs.RemainingHp && v.Me.DeckCount > 8;
            // Sword & Shield: Energy Slap moves all of Ferrothorn's Energy to the Bench, so only when it's about to be Knocked Out.
            if (p.StartsWith("Energy Slap")) return v.Me.Active != null && v.Me.Active.RemainingHp <= 70;
            // Hidden Fates: Haze Slash / Gale Blade only shuffle away a Pokémon that's about to be Knocked Out (with a Benched
            // Pokémon to take its place); Burning Fighter mills 5 cards; Empty Light Confuses and Poisons our own Active too.
            if (p.StartsWith("Haze Slash:") || p.StartsWith("Gale Blade:"))
                return v.Me.Bench.Count > 0 && v.Me.Active != null && v.Me.Active.RemainingHp <= 60;
            if (p.StartsWith("Use Burning Fighter")) return v.Me.DeckCount > 15;
            if (p.StartsWith("Use Empty Light")) return v.Me.Active == null || !CanAttackNow(v.Me.Active);
            // Unified Minds: Captivating Wink fills the opponent's Bench for them; Downburst only when the opponent's Active
            // Pokémon has more attached than ours; Dynamic Swing and Over Slice only when the base damage isn't enough.
            if (p.StartsWith("Captivating Wink")) return false;
            // Unbroken Bonds: Dedechange throws away the hand, so only a small one; Electricannon throws away the Lightning
            // Energy, so only when the 120 isn't enough.
            if (p.StartsWith("Dedechange")) return v.Me.HandCount <= 3;
            if (p.StartsWith("Electricannon")) return theirs != null && theirs.RemainingHp > 120;
            if (p.StartsWith("Downburst"))
            {
                var ours = v.Me.Active;
                return theirs != null && (ours == null || theirs.Energy.Count > ours.Energy.Count);
            }
            if (p.StartsWith("Dynamic Swing")) return theirs != null && theirs.RemainingHp > 100;
            if (p.StartsWith("Over Slice")) return theirs != null && theirs.RemainingHp > 80;
            // Ultra Prism (v85): Fire Blaster throws away 2 Energy, so only when the 80 isn't enough; Happy Turn only saves a
            // Lopunny that's about to be Knocked Out; Peeking Red Card only against a big hand; Magical Swap is left to people.
            if (p.StartsWith("Fire Blaster")) return theirs != null && theirs.RemainingHp > 80;
            if (p.StartsWith("Happy Turn")) return v.Me.Active != null && v.Me.Active.RemainingHp <= 60 && v.Me.Bench.Count > 0;
            if (p.StartsWith("Peeking Red Card")) return v.Opponent.HandCount >= 6;
            if (p.StartsWith("Magical Swap")) return false;
            // Crimson Invasion (v86): Special Artillery throws away a Special Energy, so only when the 40 isn't enough; asked about
            // Amazing Plea, the bots let the cards go only when the 80 damage would Knock Out their Active Pokémon.
            if (p.StartsWith("Special Artillery")) return theirs != null && theirs.RemainingHp > 40;
            if (p.StartsWith("Amazing Plea")) return v.Me.Active != null && v.Me.Active.RemainingHp <= 80;
            // Shining Legends (v87): Let Loose refreshes both hands to 4, so only with a small hand of our own or a big one opposite.
            if (p.StartsWith("Use Let Loose")) return v.Me.HandCount <= 3 || v.Opponent.HandCount >= 7;
            // Burning Shadows (v88): Vacuum Bolt hurts one of our own Pokémon, so only when the extra 80 Knocks Out and a Benched
            // Pokémon can take it; Energy Slosh is left to people; Knock Over only removes the opponent's Stadium.
            if (p.StartsWith("Vacuum Bolt")) return theirs != null && theirs.RemainingHp > 80 && theirs.RemainingHp <= 160 && v.Me.Bench.Any(b => b.RemainingHp > 80);
            if (p.StartsWith("Energy Slosh")) return false;
            if (p.StartsWith("Knock Over")) return v.StadiumOwner != v.Viewer;
            // Guardians Rising (v89): Hyper Jump only saves a Politoed about to be Knocked Out; Frozen Breath's discard is left to
            // people; Rake It In only with cards left to draw.
            if (p.StartsWith("Hyper Jump")) return v.Me.Active != null && v.Me.Active.RemainingHp <= 60 && v.Me.Bench.Count > 0;
            if (p.StartsWith("Frozen Breath")) return false;
            if (p.StartsWith("Use Rake It In")) return v.Me.DeckCount > 6;
            // Sun & Moon (v90): Superpower's extra 40 hurts Bewear, so only when the 80 isn't enough and Bewear can take it.
            if (p.StartsWith("Superpower")) return theirs != null && theirs.RemainingHp > 80 && v.Me.Active != null && v.Me.Active.RemainingHp > 20;
            // SM promos (v91): Sabrina & Brycen's discard of 5 cards is left to people.
            if (p.StartsWith("Sabrina & Brycen")) return false;
            // Evolutions (v92): Spark Bolt throws away all the Energy, so only when the 70 isn't enough.
            if (p.StartsWith("Spark Bolt")) return theirs != null && theirs.RemainingHp > 70;
            // Steam Siege (v93): Despair Ray throws away Benched Pokémon and Damage Play's moves are left to people.
            if (p.StartsWith("Despair Ray") || p.StartsWith("Damage Play")) return false;
            // Fates Collide (v94): Flying Flare's extra 40 hurts Moltres, like Superpower.
            if (p.StartsWith("Flying Flare")) return theirs != null && theirs.RemainingHp > 80 && v.Me.Active != null && v.Me.Active.RemainingHp > 20;
            // Asked of the opponent: each player taking a Prize card helps whoever is ahead in the race.
            if (p.StartsWith("Lt. Surge's Bargain"))
            {
                if (v.Me.PrizeCount <= 1) return true;
                if (v.Opponent.PrizeCount <= 1) return false;
                return v.Me.PrizeCount <= v.Opponent.PrizeCount;
            }
            return true;
        }

        /// <summary>The number right after the first "(" in a prompt ("(2 discarded so far ..." gives 2), or 0.</summary>
        private static int CountInParens(string prompt)
        {
            int at = prompt.IndexOf('(');
            if (at < 0) return 0;
            int end = at + 1;
            while (end < prompt.Length && char.IsDigit(prompt[end])) end++;
            int n;
            return int.TryParse(prompt.Substring(at + 1, end - at - 1), out n) ? n : 0;
        }

        /// <summary>"... came up H of N heads" (or "H heads before tails"): flip again when fewer than half were heads.</summary>
        private static bool ReflipWorthIt(string prompt)
        {
            int at = prompt.LastIndexOf(" came up ", StringComparison.Ordinal);
            if (at < 0) return false;
            var words = prompt.Substring(at + 9).Split(' ');
            int heads;
            if (words.Length < 1 || !int.TryParse(words[0], out heads)) return false;
            if (prompt.Contains(" heads before tails")) return heads == 0;
            int coins;
            if (words.Length < 3 || words[1] != "of" || !int.TryParse(words[2], out coins)) return false;
            return heads * 2 < coins;
        }

        /// <summary>Switch in a ready Darkness attacker when the current Active can't attack.</summary>
        private static bool ChainsWorthIt(PlayerView v)
        {
            var a = v.Me.Active;
            if (a == null || CanAttackNow(a)) return false;
            return v.Me.Bench.Any(b => b.Top.Def.PokemonType == EnergyType.Darkness && b.Top.Def.Name != "Pecharunt ex" && CanAttackNow(b));
        }

        private static bool IsDragapultLine(string name)
        {
            return name == "Dreepy" || name == "Drakloak" || name == "Dragapult ex";
        }

        private static int CountType(PokemonView p, EnergyType t)
        {
            return p.Energy.Count(e => e.Def.EnergyProvides == t);
        }

        private double AttachScore(DecisionOption o, PlayerView v)
        {
            var target = v.FindPokemon(o.PokemonUid);
            if (target == null) return 0;
            var type = o.Card.EnergyProvides;
            var tname = target.Top.Def.Name;
            bool active = v.Me.Active != null && v.Me.Active.Uid == target.Uid;
            double activeBonus = active ? 3 : 0;

            // Deck plans the generic rule can't see (the attack they want is on the evolution, or an Ability needs it).
            if (IsDragapultLine(tname))
            {
                bool wantFire = type == EnergyType.Fire && CountType(target, EnergyType.Fire) == 0;
                bool wantPsychic = type == EnergyType.Psychic && CountType(target, EnergyType.Psychic) == 0;
                if (wantFire || wantPsychic) return 75 + (tname == "Dragapult ex" ? 8 : tname == "Drakloak" ? 5 : 0) + activeBonus;
                return 12;
            }
            if ((tname == "N's Zorua" || tname == "N's Zoroark ex") && type == EnergyType.Darkness)
                return CountType(target, EnergyType.Darkness) < 2 ? 76 + (tname == "N's Zoroark ex" ? 6 : 0) + activeBonus : 10;
            if (tname == "Munkidori" && type == EnergyType.Darkness)
                return CountType(target, EnergyType.Darkness) == 0 ? 72 : 8;

            // Generic: does this Energy finish or progress an attack?
            double best = active ? 10 : 6;
            foreach (var atk in target.Top.Def.Attacks)
            {
                if (atk.Damage <= 0 && atk.Name != "Night Joker") continue;
                if (CostMet(target, atk.Cost)) continue;
                if (CostMetWith(target, atk.Cost, type)) best = Math.Max(best, 65 + Math.Min(250, atk.Damage) / 25.0 + activeBonus);
                else if (Helps(target, atk.Cost, type)) best = Math.Max(best, 40 + activeBonus);
            }
            // Sword & Shield: Aurora Energy costs another card from the hand.
            if (o.Card.Name == "Aurora Energy") best -= v.Me.HandCount <= 3 ? 30 : 12;
            return best;
        }

        private static double TrainerScore(CardDef card, PlayerView v)
        {
            var me = v.Me;
            var opp = v.Opponent;
            switch (card.Name)
            {
                case "Buddy-Buddy Poffin": return me.Bench.Count <= 3 ? 86 : -1;
                case "Poké Pad": return 80;
                case "Ultra Ball": return me.HandCount >= 5 ? 66 : -1;
                case "Night Stretcher": return 42;
                case "Crushing Hammer": return 50;
                case "Unfair Stamp": return 74;
                case "Special Red Card": return 62;
                case "Lillie's Determination": return me.HandCount <= 4 ? 78 : -1;
                case "Judge": return me.HandCount <= 3 ? 72 : -1;
                case "Crispin": return 60;
                case "Rosa's Encouragement": return 64;
                case "Cyrano": return me.HandCount <= 6 ? 60 : -1;
                case "N's PP Up": return 68;
                case "Boss's Orders":
                case "Lysandre":
                case "Prime Catcher":
                case "Pokémon Catcher":
                    return opp.Bench.Any(b => b.RemainingHp <= 200) && me.Active != null && CanAttackNow(me.Active) ? 58 : -1;
                case "Black Belt's Training":
                case "Kieran":
                    return opp.Active != null && opp.Active.Top.Def.IsEx && me.Active != null && CanAttackNow(me.Active) ? 70 : -1;
                case "Gladion's Final Battle":
                case "Premium Power Pro":
                    return me.Active != null && CanAttackNow(me.Active) ? 66 : -1;

                // Search and draw
                case "Rare Candy": return 92;
                case "Precious Trolley": return me.Bench.Count <= 2 ? 87 : -1;
                case "Hop's Bag": return me.Bench.Count <= 3 ? 85 : -1;
                case "Salvatore": return 84;
                case "Dawn": return 80;
                case "Team Rocket's Proton": return 78;
                case "Hilda": return 76;
                case "Hassel": return 76;
                case "Mega Signal":
                case "Tera Orb":
                case "Team Rocket's Transceiver": return 74;
                case "Brock's Scouting": return 72;
                case "Explorer's Guidance": return 70;
                case "Bug Catching Set":
                case "Dusk Ball": return 70;
                case "Team Rocket's Archer": return 70;
                case "Glass Trumpet": return 70;
                case "Team Rocket's Ariana": return me.HandCount <= 4 ? 72 : -1;
                case "Carmine": return me.HandCount <= 4 ? 64 : -1;
                case "Gwynn": return me.HandCount <= 5 ? 66 : -1;
                case "Secret Box": return me.HandCount >= 5 ? 66 : -1;
                case "Pokégear 3.0":
                case "Roto-Stick": return 64;
                case "Fighting Gong": return 64;
                case "Team Rocket's Petrel": return 56;
                case "Ciphermaniac's Codebreaking": return 40;
                case "Brilliant Blender": return 25;

                // Energy
                case "Philippe":
                case "Wondrous Patch": return 62;
                case "Janine's Secret Art": return 60;
                case "N's Plan": return me.Active != null && !CanAttackNow(me.Active) ? 55 : -1;
                case "Energy Retrieval": return 40;
                case "Energy Recycler": return 25;
                case "Energy Switch": return -1;

                // Switching and healing
                case "Switch":
                case "AZ's Tranquility": return ShouldRetreat(v) ? 47 : -1;
                case "Surfer": return ShouldRetreat(v) || me.HandCount <= 2 ? 50 : -1;
                case "Team Rocket's Giovanni": return 54;
                case "Pokémon Center Lady":
                case "Jumbo Ice Cream": return me.Active != null && me.Active.DamageCounters >= 6 ? 50 : -1;
                case "Bianca's Devotion": return 50;
                case "Wally's Compassion": return me.Active != null && me.Active.DamageCounters >= 10 ? 45 : -1;

                // Disruption
                case "Xerosic's Machinations": return opp.HandCount >= 6 ? 62 : -1;
                case "Eri": return 45;
                case "Ruffian":
                case "Enhanced Hammer": return 50;
                case "Tool Scrapper":
                    return (opp.Active != null && opp.Active.Tool != null) || opp.Bench.Any(b => b.Tool != null) ? 45 : -1;
                case "Briar": return me.Active != null && me.Active.Top.Def.IsTera && CanAttackNow(me.Active) ? 60 : -1;

                case "Miracle Headset": return 50;

                // Temporal Forces
                case "Master Ball": return 82;
                case "Awakening Drum": return me.Bench.Count(b => b.Top.Def.IsAncient) + (me.Active != null && me.Active.Top.Def.IsAncient ? 1 : 0) >= 2 ? 72 : -1;
                case "Reboot Pod": return 68;
                case "Morty's Conviction": return opp.Bench.Count >= 3 && me.HandCount >= 2 ? 70 : -1;
                case "Hand Trimmer": return opp.HandCount > 5 && me.HandCount <= 6 ? 52 : -1;
                // "Your turn ends": only when there's nothing better to do (attacks score above this).
                case "Boxed Order": return 2;
                case "Ancient Booster Energy Capsule": return me.Active != null && me.Active.Tool == null && me.Active.Top.Def.IsAncient ? 55 : -1;
                case "Future Booster Energy Capsule": return me.Active != null && me.Active.Tool == null && me.Active.Top.Def.IsFuture ? 55 : -1;
                case "Lana's Aid": return 40;
                case "Sacred Ash": return 30;
                case "Strange Timepiece": return -1;
                // Astral Radiance: Wait and See Turbo ends the turn (only when there's nothing better); Kamado throws the hand
                // away; Canceling Cologne needs judgement the bots don't have; Hisuian Heavy Ball only pays off with a Basic in the Prizes.
                case "Wait and See Turbo": return 2;
                case "Kamado": return me.HandCount <= 3 ? 60 : -1;
                case "Canceling Cologne": return -1;
                case "Hisuian Heavy Ball": return 20;
                case "Zisu": return opp.HandCount - me.HandCount >= 2 ? 70 : -1;
                case "Allister": return me.HandCount <= 4 ? 62 : -1;
                case "Milo": return me.HandCount >= 3 && me.HandCount <= 6 ? 58 : -1;
                case "Choy": return 60;
                case "Piers": return 65;
                case "Energy Loto":
                case "Feather Ball": return 50;
                case "Gutsy Pickaxe": return me.Bench.Count > 0 ? 55 : -1;
                case "Cyllene": return 20;
                case "Spicy Seasoned Curry": return me.Active != null && me.Active.DamageCounters >= 6 ? 45 : -1;
                case "Sweet Honey": return me.Active != null && me.Active.DamageCounters >= 4 ? 40 : -1;
                case "Switch Cart": return ShouldRetreat(v) && me.Active != null && me.Active.Top.Def.IsBasicPokemon ? 47 : -1;
                // Brilliant Stars: Café Master ends the turn (only when there's nothing better); the Style Mustards need an
                // otherwise empty hand, so they're good whenever they can be played.
                case "Acerola's Premonition": return opp.HandCount >= 4 ? 62 : -1;
                case "Barry": return me.HandCount <= 4 ? 62 : -1;
                case "Café Master": return 2;
                case "Fresh Water Set": return 35;
                case "Gloria": return me.Bench.Count <= 2 ? 76 : -1;
                case "Kindler": return 58;
                case "Marnie's Pride": return 45;
                case "Roseanne's Backup":
                case "Team Yell's Cheer": return 20;
                case "Rapid Strike Style Mustard":
                case "Single Strike Style Mustard": return 75;
                // Fusion Strike: Power Tablet only right before a Fusion Strike Pokémon attacks; Cross Switcher also moves our own
                // Active Pokémon; Sidney is only worth it against a big hand.
                case "Battle VIP Pass": return me.Bench.Count <= 3 ? 86 : -1;
                case "Quick Ball": return me.HandCount >= 4 ? 64 : -1;
                case "Chili & Cilan & Cress": return 60;
                case "Bug Catcher":
                case "Dancer":
                case "Schoolboy":
                case "Schoolgirl": return me.HandCount <= 5 ? 55 : -1;
                case "Shauna": return me.HandCount <= 3 ? 62 : -1;
                case "Sidney": return opp.HandCount >= 4 ? 40 : -1;
                case "Cram-o-matic": return 35;
                case "Cross Switcher": return opp.Bench.Count > 0 && me.Bench.Count > 0 ? 45 : -1;
                case "Crossceiver": return 40;
                case "Power Tablet": return me.Active != null && me.Active.Top.Def.IsFusionStrike && CanAttackNow(me.Active) ? 48 : -1;
                // Celebrations: Imposter Professor Oak only cuts down a big hand; Here Comes Team Rocket! only when there's
                // nothing better; Rocket's Admin. refills a small hand early in the game.
                case "Imposter Professor Oak": return opp.HandCount >= 8 ? 40 : -1;
                case "Here Comes Team Rocket!": return 2;
                case "Rocket's Admin.": return me.HandCount <= 3 && me.PrizeCount >= 4 ? 60 : -1;
                // Evolving Skies: Boost Shake ends the turn (only when there's nothing better); Switching Cups needs judgement;
                // Copycat and Zinnia's Resolve only when they draw well; Toy Catcher sets up a Knock Out.
                case "Aroma Lady": return me.HandCount <= 4 ? 52 : -1;
                case "Boost Shake": return 2;
                case "Copycat": return opp.HandCount - me.HandCount >= 2 ? 65 : -1;
                case "Lucky Ice Pop": return me.Active != null && me.Active.DamageCounters >= 2 ? 40 : -1;
                case "Switching Cups": return -1;
                case "Toy Catcher": return me.Active != null && CanAttackNow(me.Active) ? 55 : -1;
                case "Zinnia's Resolve": return me.HandCount >= 4 && opp.Bench.Count >= 3 ? 55 : -1;
                // Chilling Reign: Echoing Horn, Expedition Uniform and Peonia need judgement the bots don't have; Karen's
                // Conviction only right before a Single Strike Pokémon attacks, once the opponent has taken Prize cards.
                case "Agatha": return me.Active != null && me.Active.DamageCounters >= 3 ? 45 : -1;
                case "Avery": return opp.Bench.Count > 3 ? 70 : (me.HandCount <= 4 ? 50 : -1);
                case "Brawly": return me.Bench.Count <= 3 ? 60 : -1;
                case "Caitlin": return me.HandCount >= 4 ? 45 : -1;
                case "Doctor": return opp.Active != null && opp.Active.Top.Def.IsVmax ? 70 : (me.HandCount <= 4 ? 50 : -1);
                case "Echoing Horn":
                case "Expedition Uniform":
                case "Peonia": return -1;
                case "Flannery": return 45;
                case "Fog Crystal": return 55;
                case "Honey": return opp.Bench.Count(b => b.Top.Def.IsPokemonV) >= 2 ? 55 : -1;
                case "Karen's Conviction":
                    return v.PrizeCountAtStart - opp.PrizeCount >= 2 && me.Active != null && me.Active.Top.Def.IsSingleStrike && CanAttackNow(me.Active) ? 60 : -1;
                case "Klara": return 45;
                case "Peony": return me.HandCount <= 2 ? 50 : -1;
                case "Siebold": return 45;
                case "Welcoming Lantern": return 35;
                // Battle Styles: Camping Gear ends the turn (only when there's nothing better); Cheryl throws away Energy and
                // Phoebe needs judgement the bots don't have.
                case "Bruno": return me.HandCount <= 3 ? 60 : -1;
                case "Camping Gear": return 2;
                case "Cheryl":
                case "Phoebe": return -1;
                case "Escape Rope": return ShouldRetreat(v) ? 45 : -1;
                case "Korrina's Focus": return me.HandCount <= 3 ? 60 : -1;
                case "Level Ball": return 55;
                // Shining Fates: Cara Liss benches Rare Fossils (Darkness Ablaze), so only with room on the Bench; Rose discards
                // the hand, so only a small one; Team Yell Towel heals the opponent too, so only when ours is hurt more.
                case "Ball Guy": return 55;
                case "Bird Keeper": return ShouldRetreat(v) ? 65 : (me.HandCount <= 2 && me.Bench.Count > 0 ? 45 : -1);
                case "Cara Liss": return me.Bench.Count <= 3 ? 45 : -1;
                case "Poké Kid": return 50;
                case "Rose": return me.HandCount <= 2 ? 55 : -1;
                case "Skyla": return 50;
                case "Team Yell Towel":
                    return me.Active != null && me.Active.DamageCounters >= 5 && (opp.Active == null || opp.Active.DamageCounters < me.Active.DamageCounters) ? 40 : -1;
                // Vivid Voltage: Drone Rotom only shows cards; Hero's Medal costs 100 HP; Memory Capsule needs an evolved Pokémon
                // to give old attacks to; Moomoo Cheese only when one of ours with Energy is hurt.
                case "Beauty": return me.HandCount <= 4 ? 50 : 30;
                case "Drone Rotom": return 10;
                case "Hero's Medal": return -1;
                case "League Staff": return 55;
                case "Memory Capsule":
                    return (me.Active != null && me.Active.Stack.Count > 1) || me.Bench.Any(mv => mv.Stack.Count > 1) ? 30 : -1;
                case "Moomoo Cheese":
                    return (me.Active != null && me.Active.Energy.Count > 0 && me.Active.DamageCounters > 0)
                        || me.Bench.Any(mv => mv.Energy.Count > 0 && mv.DamageCounters > 0) ? 40 : -1;
                // Champion's Path: Rotom Bike ends the turn (only with a small hand and nothing better); Hyper Potion and
                // Suspicious Food Tin cost Energy, so only for a badly hurt Pokémon; Marnie against a big hand or with a small one.
                case "Full Heal": return me.Active != null && me.Active.Conditions != SpecialCondition.None ? 45 : -1;
                case "Hyper Potion":
                    return (me.Active != null && me.Active.Energy.Count >= 2 && me.Active.DamageCounters >= 8)
                        || me.Bench.Any(mv => mv.Energy.Count >= 2 && mv.DamageCounters >= 8) ? 35 : -1;
                case "Marnie": return opp.HandCount >= 5 || me.HandCount <= 3 ? 55 : -1;
                case "Rotom Bike": return me.HandCount <= 3 ? 2 : -1;
                case "Rotom Phone": return 15;
                case "Sonia": return 50;
                case "Suspicious Food Tin":
                    return (me.Active != null && CountType(me.Active, EnergyType.Psychic) > 0 && me.Active.DamageCounters >= 5)
                        || me.Bench.Any(mv => CountType(mv, EnergyType.Psychic) > 0 && mv.DamageCounters >= 5) ? 35 : -1;
                case "Team Yell Grunt": return 40;
                // Darkness Ablaze: Yell Horn confuses our own Active too; Struggle Gloves only matters against Weakness; Old PC
                // needs two heads; Turbo Patch one.
                case "Big Parasol": return 40;
                case "Billowing Smoke": return 45;
                case "Familiar Bell": return me.Discard.Any(c => c.Def.IsPokemon) ? 45 : -1;
                case "Old PC": return me.Discard.Count > 0 ? 15 : -1;
                case "Pokémon Breeder's Nurturing": return 70;
                case "Struggle Gloves": return 25;
                case "Turbo Patch": return me.Discard.Any(c => c.Def.IsBasicEnergy) ? 35 : -1;
                case "Yell Horn": return -1;
                // Rebel Clash: Scoop Up Net needs judgement the bots don't have; Oleana costs 2 cards, so only with a big hand;
                // Galar Mine slows both players down.
                case "Big Charm": return 45;
                case "Burning Scarf": return me.Active != null && me.Active.Top.Def.HasType(EnergyType.Fire) ? 40 : 20;
                case "Capacious Bucket": return 40;
                case "Cursed Shovel": return 30;
                case "Dan": return 50;
                case "Galar Mine": return 20;
                case "Oleana": return me.HandCount >= 6 && opp.HandCount >= 3 ? 30 : -1;
                case "Scoop Up Net": return -1;
                // Sword & Shield: Ordinary Rod only with something to shuffle back; Lum Berry best on a Pokémon with a Special
                // Condition already.
                case "Evolution Incense": return 45;
                case "Lucky Egg": return 30;
                case "Lum Berry": return me.Active != null && me.Active.Conditions != SpecialCondition.None ? 45 : 20;
                case "Metal Saucer": return 50;
                case "Ordinary Rod": return me.Discard.Count(c => c.Def.IsPokemon || c.Def.IsBasicEnergy) >= 2 ? 30 : -1;
                case "Sitrus Berry": return 30;
                // Cosmic Eclipse: Bellelba & Brycen-Man mills our own deck too and Island Challenge Amulet only hurts a Pokémon-GX
                // or Pokémon-EX; Will only right before an attack that flips; Power Plant only against Abilities on the other side.
                case "Bellelba & Brycen-Man":
                case "Island Challenge Amulet": return -1;
                case "Beastite": return (me.Active != null && me.Active.Top.Def.IsUltraBeast) || me.Bench.Any(mv => mv.Top.Def.IsUltraBeast) ? 50 : -1;
                case "Dragonium Z: Dragon Claw":
                    return (me.Active != null && me.Active.Top.Def.Attacks.Any(a => a.Name == "Dragon Claw"))
                           || me.Bench.Any(mv => mv.Top.Def.Attacks.Any(a => a.Name == "Dragon Claw")) ? 45 : -1;
                case "Great Catcher":
                    return opp.Bench.Any(b => b.Top.Def.IsGXorEX && b.RemainingHp <= 200) && me.HandCount >= 4 && me.Active != null && CanAttackNow(me.Active) ? 56 : -1;
                case "Tag Call": return 60;
                case "Clay": return me.DeckCount >= 20 ? 55 : -1;
                case "Cynthia & Caitlin": return 50;
                case "Erika": return me.HandCount <= 4 ? 55 : -1;
                case "Guzma & Hala": return 40;
                case "Lillie's Full Force": return me.HandCount <= 3 ? 60 : -1;
                case "Mallow & Lana":
                    return ShouldRetreat(v) ? 50 : (me.Active != null && me.Active.DamageCounters >= 6 && me.Bench.Count > 0 ? 45 : -1);
                case "Misty & Lorelei":
                    return (me.Active != null && me.Active.Top.Def.HasType(EnergyType.Water)) || me.Bench.Any(mv => mv.Top.Def.HasType(EnergyType.Water)) ? 50 : -1;
                case "N's Resolve": return 55;
                case "Professor Oak's Setup": return me.Bench.Count <= 2 ? 70 : -1;
                case "Red & Blue": return 70;
                case "Roller Skater": return me.HandCount <= 5 ? 55 : -1;
                case "Rosa": return 70;
                case "Roxie": return me.HandCount <= 5 ? 45 : -1;
                case "Will":
                    return me.Active != null && CanAttackNow(me.Active) && me.Active.Top.Def.Attacks.Any(a => a.Text != null && a.Text.Contains("Flip")) ? 45 : -1;
                case "Power Plant":
                    return (opp.Active != null && opp.Active.Top.Def.IsGXorEX && opp.Active.Top.Def.Abilities.Count > 0)
                           || opp.Bench.Any(b => b.Top.Def.IsGXorEX && b.Top.Def.Abilities.Count > 0) ? 40 : -1;
                // Hidden Fates: Lt. Surge's Strategy goes first, and only with 2 more Supporters to follow it; Jessie & James
                // only costs us little; Giovanni's Exile and Misty's Water Command need judgement the bots don't have; Shrine of
                // Punishment and the Gyms only when they help us more than the opponent.
                case "Lt. Surge's Strategy": return me.Hand.Count(c => c.Def.IsSupporter && c.Def.Name != card.Name) >= 2 ? 90 : -1;
                case "Jessie & James": return me.HandCount <= 3 && opp.HandCount >= 3 ? 45 : -1;
                case "Sabrina's Suggestion": return opp.HandCount >= 4 ? 50 : -1;
                case "Hiker": return 20;
                case "Giovanni's Exile":
                case "Misty's Water Command": return -1;
                case "Cynthia": return me.HandCount <= 4 ? 70 : -1;
                // Ultra Prism (v85): Missing Clover only 4 at once; Lillie and Looker draw; Cyrus ◇ when it clears at least 1
                // Benched Pokémon; Peeking Red Card against a big hand.
                case "Missing Clover": return me.Hand.Count(c => c.Def.Name == "Missing Clover") >= 4 && me.PrizeCount > 0 ? 80 : -1;
                case "Lillie": return me.HandCount <= 3 ? 70 : -1;
                case "Looker": return me.HandCount <= 4 ? 50 : -1;
                case "Mars": return 50;
                case "Cyrus \u25C7": return opp.Bench.Count >= 3 ? 72 : -1;
                case "Pokémon Fan Club": return me.Bench.Count <= 3 ? 70 : -1;
                case "Peeking Red Card": return opp.HandCount >= 6 ? 50 : -1;
                case "Volkner": return 55;
                // Crimson Invasion (v86): Gladion trades a Prize card for itself; Olivia finds 2 Pokémon-GX.
                case "Gladion": return me.PrizeCount >= 2 ? 45 : -1;
                case "Olivia": return 55;
                // Shining Legends (v87): Damage Mover is left to people; Sophocles costs 2 cards; Pokémon Breeder draws 2.
                case "Damage Mover": return -1;
                case "Sophocles": return me.HandCount <= 5 && me.DeckCount > 8 ? 50 : -1;
                case "Pokémon Breeder": return me.DeckCount > 4 ? 45 : -1;
                // Burning Shadows (v88): Acerola saves a badly hurt Active Pokémon; Kiawe ends the turn, so only when there's no attack
                // to make; Multi Switch only feeds an Active Pokémon that can't attack yet; Rotom Dex, Wicke and Po Town are left to
                // people.
                case "Acerola": return me.Active != null && me.Active.DamageCounters >= 8 && me.Bench.Count > 0 ? 50 : -1;
                case "Kiawe":
                    return me.Active != null && !CanAttackNow(me.Active)
                           && (me.Active.Top.Def.PokemonType == EnergyType.Fire || me.Bench.Any(b => b.Top.Def.PokemonType == EnergyType.Fire)) ? 40 : -1;
                case "Plumeria": return me.HandCount >= 5 && (opp.Bench.Any(b => b.Energy.Count > 0) || (opp.Active != null && opp.Active.Energy.Count > 0)) ? 35 : -1;
                case "Tormenting Spray": return opp.HandCount >= 3 ? 30 : -1;
                case "Multi Switch": return me.Active != null && !CanAttackNow(me.Active) && me.Bench.Any(b => b.Energy.Count > 0) ? 40 : -1;
                case "Rescue Stretcher": return me.Discard.Any(c => c.Def.IsPokemon) ? 35 : -1;
                case "Rotom Dex Poké Finder Mode":
                case "Wicke":
                case "Po Town": return -1;
                case "Mount Lanakila": return 20;
                // Guardians Rising (v89): Field Blower against the opponent's Tools or Stadium; Hala refreshes a small hand; Max Potion
                // saves a badly hurt Active Pokémon with little Energy; Mallow is left to people.
                case "Field Blower":
                    return (v.Stadium != null && v.StadiumOwner != v.Viewer)
                           || (opp.Active != null && opp.Active.Tools.Count > 0) || opp.Bench.Any(b => b.Tools.Count > 0) ? 40 : -1;
                case "Hala": return me.HandCount <= 3 ? 60 : -1;
                case "Max Potion": return me.Active != null && me.Active.DamageCounters >= 8 && me.Active.Energy.Count <= 1 ? 50 : -1;
                case "Mallow": return -1;
                // Sun & Moon (v90): Big Malasada for a hurt or affected Active Pokémon; Ilima refreshes a small hand (both players);
                // Professor Kukui draws 2 and adds 20 damage; Team Skull Grunt against a big hand; Timer Ball for Evolution
                // Pokémon; Rotom Dex is left to people.
                case "Big Malasada":
                    return me.Active != null && (me.Active.Conditions != SpecialCondition.None || me.Active.DamageCounters >= 2) ? 30 : -1;
                case "Ilima": return me.HandCount <= 3 ? 45 : -1;
                case "Professor Kukui": return me.DeckCount > 4 ? 50 : -1;
                case "Team Skull Grunt": return opp.HandCount >= 4 ? 40 : -1;
                case "Timer Ball": return me.DeckCount > 5 ? 40 : -1;
                case "Rotom Dex": return -1;
                // SM promos (v91): Sabrina & Brycen finds 2 basic Energy.
                case "Sabrina & Brycen": return me.DeckCount > 4 ? 45 : -1;
                // Evolutions (v92): Spirit Links only for their own Pokémon-EX; Professor Oak's Hint ends the turn, so only with a
                // small hand and nothing to attack with; Revive for an empty Bench spot; Devolution Spray, Maintenance and Pokédex
                // are left to people.
                case "Blastoise Spirit Link":
                case "Charizard Spirit Link":
                case "Gardevoir Spirit Link":
                case "Steelix Spirit Link":
                case "Alakazam Spirit Link":
                case "Altaria Spirit Link":
                case "Audino Spirit Link":
                case "Pidgeot Spirit Link":
                case "Slowbro Spirit Link":
                case "Venusaur Spirit Link":
                case "Aerodactyl Spirit Link":
                case "Mawile Spirit Link":
                case "Beedrill Spirit Link":
                case "Garchomp Spirit Link":
                case "Salamence Spirit Link":
                case "Camerupt Spirit Link":
                case "Sharpedo Spirit Link":
                case "Lucario Spirit Link":
                {
                    string ex = card.Name.Substring(0, card.Name.Length - " Spirit Link".Length) + "-EX";
                    return (me.Active != null && me.Active.Top.Def.Name == ex && me.Active.Tools.Count == 0)
                           || me.Bench.Any(b => b.Top.Def.Name == ex && b.Tools.Count == 0) ? 55 : -1;
                }
                case "Professor Oak's Hint":
                    return me.HandCount <= 2 && me.DeckCount > 8 && (me.Active == null || !CanAttackNow(me.Active)) ? 35 : -1;
                case "Revive": return me.Bench.Count < 4 ? 30 : -1;
                case "Devolution Spray":
                case "Maintenance":
                case "Pokédex":
                    return -1;
                // Steam Siege (v93): Professor Sycamore refreshes a small hand; the Fossils look for their Restored Pokémon; Special
                // Charge recycles Special Energy; Ninja Boy, Pokémon Ranger and Captivating Poké Puff are left to people.
                case "Professor Sycamore": return me.HandCount <= 3 && me.DeckCount > 8 ? 55 : -1;
                case "Armor Fossil Shieldon":
                case "Claw Fossil Anorith":
                    return me.Bench.Count < 5 ? 25 : -1;
                case "Special Charge": return me.Discard.Count(c => c.Def.IsSpecialEnergy) >= 2 ? 20 : -1;
                case "Ninja Boy":
                case "Pokémon Ranger":
                case "Captivating Poké Puff":
                    return -1;
                // Fates Collide (v94): the Fossils look for their Restored Pokémon; Random Receiver finds a Supporter; Lass's Special
                // draws for the opponent's Benched Basics; Mega Catcher pulls up a Mega; Team Rocket's Handiwork mills; Energy Reset,
                // Fairy Drop and Fossil Excavation Kit are left to people.
                case "Helix Fossil Omanyte":
                case "Dome Fossil Kabuto":
                case "Old Amber Aerodactyl":
                    return me.Bench.Count < 5 ? 25 : -1;
                case "Random Receiver": return me.DeckCount > 5 ? 45 : -1;
                case "Lass's Special": return opp.Bench.Count >= 3 ? 50 : -1;
                case "Mega Catcher": return 55;
                case "Team Rocket's Handiwork": return 30;
                case "Energy Reset":
                case "Fairy Drop":
                case "Fossil Excavation Kit":
                    return -1;
                case "Power Memory":
                    return (me.Active != null && me.Active.Top.Def.Name == "Zygarde-EX" && me.Active.Tools.Count == 0)
                           || me.Bench.Any(b => b.Top.Def.Name == "Zygarde-EX" && b.Tools.Count == 0) ? 55 : -1;
                // Generations (v95): Clemont finds Lightning Energy; Evosoda and Wally evolve from the deck; Olympia switches and
                // heals; Red Card trims a big hand; Team Flare Grunt discards an Energy; Revitalizer and Max Revive recycle Pokémon;
                // Imakuni? (Confuses your own Active Pokémon) is left to people.
                case "Clemont": return me.DeckCount > 6 ? 45 : -1;
                case "Evosoda": return 55;
                case "Wally": return 55;
                case "Olympia":
                    return me.Active != null && (me.Active.DamageCounters >= 3 || !CanAttackNow(me.Active)) && me.Bench.Any(b => CanAttackNow(b)) ? 50 : -1;
                case "Red Card": return opp.HandCount >= 6 ? 50 : -1;
                case "Team Flare Grunt": return 55;
                case "Revitalizer": return 30;
                case "Max Revive": return -1;
                case "Imakuni?": return -1;
                // BREAKpoint (v96): Tierno draws; Max Elixir powers up a Benched Basic; Delinquent needs a Stadium in play to cost the
                // opponent 3 cards; Fighting Fury Belt goes on a Basic attacker; Puzzle of Time and Psychic's Third Eye are left to people.
                case "Tierno": return me.DeckCount > 6 ? 45 : -1;
                case "Max Elixir": return 60;
                case "Delinquent": return opp.HandCount >= 3 ? 50 : -1;
                case "Fighting Fury Belt":
                    return (me.Active != null && me.Active.Top.Def.IsBasicPokemon && me.Active.Tools.Count == 0)
                           || me.Bench.Any(b => b.Top.Def.IsBasicPokemon && b.Tools.Count == 0) ? 45 : -1;
                case "Bursting Balloon": return me.Active != null && me.Active.Tools.Count == 0 ? 30 : -1;
                case "Puzzle of Time":
                case "Psychic's Third Eye":
                    return -1;
                // BREAKthrough (v97): Brigette fills an empty Bench; Giovanni's Scheme adds 20 damage before an attack; Professor's
                // Letter and Heavy Ball search; Buddy-Buddy Rescue helps the opponent too; Reserved Ticket and Town Map are left to people.
                case "Brigette": return me.Bench.Count <= 2 ? 70 : (me.Bench.Count < 4 ? 40 : -1);
                case "Giovanni's Scheme": return me.Active != null && CanAttackNow(me.Active) ? 45 : -1;
                case "Professor's Letter": return me.DeckCount > 3 ? 45 : -1;
                case "Heavy Ball": return me.DeckCount > 3 ? 40 : -1;
                case "Buddy-Buddy Rescue": return me.Discard.Any(c => c.Def.IsPokemon) ? 25 : -1;
                case "Reserved Ticket":
                case "Town Map":
                    return -1;
                // Ancient Origins (v98): Ace Trainer when behind on Prizes with a small hand; Steven and Trainers' Mail search; Paint
                // Roller against the opponent's Stadium; Faded Town only against the opponent's Megas; Hex Maniac is left to people.
                case "Ace Trainer": return me.HandCount <= 4 ? 60 : -1;
                case "Steven": return me.DeckCount > 3 ? 55 : -1;
                case "Trainers' Mail": return me.DeckCount > 4 ? 45 : -1;
                case "Paint Roller": return v.StadiumOwner >= 0 && v.StadiumOwner != v.Viewer ? 40 : -1;
                case "Eco Arm": return 15;
                case "Faded Town":
                {
                    int theirs = opp.Bench.Count(b => b.Top.Def.IsXYMegaEvolution) + (opp.Active != null && opp.Active.Top.Def.IsXYMegaEvolution ? 1 : 0);
                    int ours = me.Bench.Count(b => b.Top.Def.IsXYMegaEvolution) + (me.Active != null && me.Active.Top.Def.IsXYMegaEvolution ? 1 : 0);
                    return theirs > ours ? 40 : -1;
                }
                case "Hex Maniac": return -1;
                // Roaring Skies (v99): Winona and VS Seeker search; Mega Turbo powers up a Mega Evolution Pokémon; Sky Field only with a
                // Bench that needs the room.
                case "Winona": return me.DeckCount > 3 ? 45 : -1;
                case "VS Seeker": return 50;
                case "Mega Turbo": return 60;
                case "Sky Field": return me.Bench.Count >= 4 && (v.StadiumOwner < 0 || v.StadiumOwner != v.Viewer) ? 40 : -1;
                // Double Crisis (v100): the Admins and Great Balls for their team; the Grunts with a small hand; the Secret Bases when
                // the opponent isn't playing that team.
                case "Team Aqua Admin": return 55;
                case "Team Magma Admin": return 45;
                case "Team Aqua Grunt":
                case "Team Magma Grunt":
                    return me.HandCount <= 4 ? 50 : -1;
                case "Team Aqua's Great Ball":
                case "Team Magma's Great Ball":
                    return me.DeckCount > 3 ? 45 : -1;
                case "Team Aqua's Secret Base":
                case "Team Magma's Secret Base":
                    return v.StadiumOwner < 0 || v.StadiumOwner != v.Viewer ? 25 : -1;
                // Primal Clash (v101): Archie's and Maxie's as the last card; Dive Ball, Repeat Ball and Teammates search; Birch
                // with a small hand; Shrine of Memories and Silent Lab are left to people.
                case "Archie's Ace in the Hole":
                case "Maxie's Hidden Ball Trick":
                    return 70;
                case "Dive Ball": return me.DeckCount > 3 ? 45 : -1;
                case "Repeat Ball": return me.DeckCount > 3 ? 40 : -1;
                case "Teammates": return 60;
                case "Professor Birch's Observations": return me.HandCount <= 3 ? 50 : -1;
                case "Shrine of Memories":
                case "Silent Lab":
                    return -1;
                // Phantom Forces (v102): Head Ringer and Jamming Net on the opponent's Pokémon-EX; Xerosic against their Tools and
                // Special Energy; Roller Skates for cards; AZ, Battle Compressor, Hand Scope, Target Whistle, Lysandre's Trump Card,
                // Dimension Valley, Steel Shelter and Robo Substitute are left to people.
                case "Head Ringer":
                case "Jamming Net":
                    return opp.Bench.Concat(opp.Active != null ? new[] { opp.Active } : new PokemonView[0]).Any(b => b.Top.Def.IsPokemonEX && b.Tools.Count == 0) ? 40 : -1;
                case "Xerosic":
                    return opp.Bench.Concat(opp.Active != null ? new[] { opp.Active } : new PokemonView[0]).Any(b => b.Tools.Count > 0 || b.Energy.Any(en => en.Def.IsSpecialEnergy)) ? 45 : -1;
                case "Roller Skates": return me.DeckCount > 3 ? 25 : -1;
                case "AZ":
                case "Battle Compressor":
                case "Hand Scope":
                case "Target Whistle":
                case "Lysandre's Trump Card":
                case "Dimension Valley":
                case "Steel Shelter":
                case "Robo Substitute":
                    return -1;
                // Furious Fists (v103): Korrina and Battle Reporter for cards; the Fossils and Fossil Researcher for Restored
                // Pokémon; the Stadiums and Tool Retriever are left to people.
                case "Korrina": return me.DeckCount > 3 ? 55 : -1;
                case "Battle Reporter": return opp.HandCount - me.HandCount >= 3 ? 55 : -1;
                case "Fossil Researcher": return 40;
                case "Jaw Fossil":
                case "Sail Fossil":
                    return 30;
                case "Fighting Stadium":
                case "Mountain Ring":
                case "Training Center":
                case "Tool Retriever":
                    return -1;
                // Flashfire (v104): Blacksmith and Fiery Torch for Fire decks; Startling Megaphone against the opponent's Tools;
                // Magnetic Storm, Protection Cube and Trick Shovel are left to people.
                case "Blacksmith": return 55;
                case "Fiery Torch": return me.DeckCount > 3 ? 40 : -1;
                case "Startling Megaphone":
                    return opp.Bench.Concat(opp.Active != null ? new[] { opp.Active } : new PokemonView[0]).Any(b => b.Tools.Count > 0) ? 45 : -1;
                case "Magnetic Storm":
                case "Protection Cube":
                case "Trick Shovel":
                    return -1;
                // XY promos (v107): Trevor for a Pokémon, Gym Badge to draw; Karen is left to people.
                case "Trevor": return me.DeckCount > 3 ? 45 : -1;
                case "Gym Badge": return me.DeckCount > 5 ? 30 : -1;
                case "Karen": return -1;
                // Legendary Treasures (v108): Bianca with a small hand, Cedric Juniper to draw (a bot's opponent rarely guesses right);
                // Elesa is left to people.
                case "Bianca": return me.HandCount <= 3 && me.DeckCount > 6 ? 55 : -1;
                case "Cedric Juniper": return me.DeckCount > 5 ? 35 : -1;
                case "Elesa": return -1;
                // Plasma Blast (v109): Professor Juniper with a small hand, the Fossils for Restored Pokémon; Iris is left to people.
                case "Professor Juniper": return me.HandCount <= 3 && me.DeckCount > 8 ? 55 : -1;
                case "Cover Fossil":
                case "Plume Fossil":
                case "Root Fossil Lileep":
                    return 30;
                case "Iris": return -1;
                // Plasma Freeze (v110): Ghetsis to draw, Team Plasma Ball and Shadow Triad for Team Plasma cards; Frozen City hurts both
                // sides and is left to people.
                case "Ghetsis": return me.DeckCount > 5 ? 40 : -1;
                case "Team Plasma Ball": return me.DeckCount > 3 ? 40 : -1;
                case "Shadow Triad": return 30;
                case "Frozen City": return -1;
                // Plasma Storm (v111): Bicycle and Colress to draw, Colress Machine for Plasma Energy, Hypnotoxic Laser; Team Plasma
                // Grunt, Dowsing Machine (ACE SPEC) and the Stadiums are left to people.
                case "Bicycle": return me.HandCount <= 2 && me.DeckCount > 4 ? 45 : -1;
                case "Colress": return me.HandCount <= 3 && me.DeckCount > 8 && me.Bench.Count + opp.Bench.Count >= 5 ? 50 : -1;
                case "Colress Machine": return 40;
                case "Ether": return me.DeckCount > 3 ? 30 : -1;
                case "Hypnotoxic Laser": return 50;
                case "Team Plasma Grunt":
                case "Dowsing Machine":
                case "Plasma Frigate":
                case "Virbank City Gym":
                    return -1;
                // Fossil (v116): Gambler with a small hand; Mr. Fuji and Recycle are left to people.
                case "Gambler": return me.HandCount <= 2 && me.DeckCount > 10 ? 40 : -1;
                case "Mr. Fuji":
                case "Recycle":
                    return -1;
                // Base Set (v113): Professor Oak only with a small hand; Bill draws; Gust of Wind like Pokémon Catcher; Energy Removal
                // on the opponent's Active; PlusPower before an attack; Potion on a hurt Active. The rest (Super Energy Removal, Scoop Up,
                // Item Finder, Pokémon Trader, Pokémon Flute, Lass, Defender) are left to people.
                case "Professor Oak": return me.HandCount <= 3 && me.DeckCount > 10 ? 60 : -1;
                case "Bill": return me.DeckCount > 4 ? 65 : -1;
                case "Gust of Wind":
                    return opp.Bench.Any(b => b.RemainingHp <= 60) && me.Active != null && CanAttackNow(me.Active) ? 58 : -1;
                case "Energy Removal": return opp.Active != null && opp.Active.Energy.Count > 0 ? 50 : -1;
                case "PlusPower": return me.Active != null && CanAttackNow(me.Active) && opp.Active != null && opp.Active.RemainingHp <= 40 ? 45 : -1;
                case "Potion": return me.Active != null && me.Active.MaxHp - me.Active.RemainingHp >= 20 ? 35 : -1;
                case "Super Energy Removal":
                case "Scoop Up":
                case "Item Finder":
                case "Pokémon Trader":
                case "Pokémon Flute":
                case "Lass":
                case "Defender":
                    return -1;
                // Boundaries Crossed (v112): Gold Potion on a badly hurt Active Pokémon; Hugh, Computer Search (ACE SPEC) and Aspertia
                // City Gym are left to people.
                case "Gold Potion": return me.Active != null && me.Active.MaxHp - me.Active.RemainingHp >= 60 ? 50 : -1;
                case "Hugh":
                case "Computer Search":
                case "Aspertia City Gym":
                    return -1;
                // XY (v105): Cassius and Shadow Circle are left to people.
                case "Cassius":
                case "Shadow Circle":
                    return -1;
                case "Floral Crown":
                    return (me.Active != null && me.Active.Top.Def.IsBasicPokemon && me.Active.Tools.Count == 0)
                           || me.Bench.Any(b => b.Top.Def.IsBasicPokemon && b.Tools.Count == 0) ? 25 : -1;
                case "Guzma":
                    return opp.Bench.Any(b => b.RemainingHp <= 200) && me.Bench.Any(b => CanAttackNow(b)) ? 58 : -1;
                case "Fisherman": return me.Discard.Count(c => c.Def.IsBasicEnergy) >= 2 ? 50 : -1;
                case "Bill's Analysis": return 55;
                case "Erika's Hospitality": return opp.Bench.Count + (opp.Active != null ? 1 : 0) >= 3 ? 60 : -1;
                case "Blaine's Last Stand": return 70;
                case "Brock's Grit": return me.Discard.Count(c => c.Def.IsPokemon || c.Def.IsBasicEnergy) >= 4 ? 30 : -1;
                case "Brock's Training": return 50;
                case "Misty's Determination": return me.HandCount >= 2 && me.HandCount <= 5 ? 55 : -1;
                case "Koga's Trap": return opp.Active != null && opp.Active.Conditions == SpecialCondition.None ? 50 : -1;
                case "Aether Foundation Employee": return 40;
                case "Shrine of Punishment":
                {
                    int theirs = opp.Bench.Count(b => b.Top.Def.IsGXorEX) + (opp.Active != null && opp.Active.Top.Def.IsGXorEX ? 1 : 0);
                    int ours = me.Bench.Count(b => b.Top.Def.IsGXorEX) + (me.Active != null && me.Active.Top.Def.IsGXorEX ? 1 : 0);
                    return theirs > ours ? 40 : -1;
                }
                // Unified Minds: Channeler, Tag Switch and Grimsley need judgement the bots don't have; the Z-Crystals only on a
                // Pokémon with the attack; Blaine's Quiz Show is a coin flip for 4 cards; Reset Stamp only late in the game.
                case "Channeler":
                case "Tag Switch": return -1;
                case "Grimsley": return 20;
                case "Flyinium Z: Air Slash":
                case "Normalium Z: Tackle":
                {
                    string zBase = card.Name == "Flyinium Z: Air Slash" ? "Air Slash" : "Tackle";
                    return (me.Active != null && me.Active.Top.Def.Attacks.Any(a => a.Name == zBase))
                           || me.Bench.Any(mv => mv.Top.Def.Attacks.Any(a => a.Name == zBase)) ? 45 : -1;
                }
                case "Blaine's Quiz Show": return me.HandCount <= 4 ? 40 : -1;
                case "Blue's Tactics": return me.HandCount <= 4 ? 60 : -1;
                case "Cherish Ball": return 60;
                case "Coach Trainer": return 55;
                case "Hapu": return 55;
                case "Misty's Favor": return 60;
                case "Poké Maniac": return 25;
                case "Stadium Nav": return 25;
                case "Giant Bomb": return 30;
                case "Great Potion": return me.Active != null && me.Active.DamageCounters >= 3 ? 55 : -1;
                case "Reset Stamp": return opp.PrizeCount <= 3 && opp.HandCount >= 4 ? 60 : -1;
                // Unbroken Bonds: Devolution Spray Z, Surprise Box (a card for the opponent) and Ultra Forest Kartenvoy need
                // judgement the bots don't have; the discard-2 cards only with a full hand; Welder only with Fire Energy to attach.
                case "Devolution Spray Z":
                case "Surprise Box":
                case "Ultra Forest Kartenvoy": return -1;
                case "Chip-Chip Ice Axe": return 20;
                case "Dusk Stone": return 65;
                case "Electromagnetic Radar":
                case "Red's Challenge": return me.HandCount >= 5 ? 45 : -1;
                case "Energy Spinner": return 50;
                case "Fire Crystal": return 45;
                case "Green's Exploration": return 55;
                case "Janine": return 55;
                case "Molayne": return 20;
                case "Samson Oak": return me.HandCount <= 4 ? 55 : -1;
                case "Welder": return me.Hand.Any(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fire) ? 70 : 30;
                case "Brock's Pewter City Gym":
                case "Misty's Cerulean City Gym":
                {
                    string gx = card.Name == "Brock's Pewter City Gym" ? "Onix-GX" : "Starmie-GX";
                    return (me.Active != null && me.Active.Top.Def.Name == gx) || me.Bench.Any(b => b.Top.Def.Name == gx) ? 40 : -1;
                }
                default:
                    if (card.IsStadium) return 35;
                    if (card.TrainerKind == TrainerKind.Tool) return 50;
                    return 30;
            }
        }

        internal static bool ShouldRetreat(PlayerView v)
        {
            var a = v.Me.Active;
            if (a == null || CanAttackNow(a)) return false;
            return v.Me.Bench.Any(b => b.Top.Def.Attacks.Any(atk => atk.Damage >= 70 && CostMet(b, atk.Cost)));
        }

        internal static bool CanAttackNow(PokemonView p)
        {
            return p.Top.Def.Attacks.Any(atk => (atk.Damage > 0 || atk.Name == "Night Joker") && CostMet(p, atk.Cost));
        }

        private static bool CostMet(PokemonView mon, EnergyType[] cost)
        {
            return mon.CanPay(cost);
        }

        private static bool CostMetWith(PokemonView mon, EnergyType[] cost, EnergyType extra)
        {
            var units = new List<int>(mon.EnergyUnits) { EnergyMask.Of(extra) };
            return EnergyMask.CanPay(units, cost);
        }

        private static bool Helps(PokemonView mon, EnergyType[] cost, EnergyType extra)
        {
            int before = EnergyMask.Missing(mon.EnergyUnits, cost);
            var units = new List<int>(mon.EnergyUnits) { EnergyMask.Of(extra) };
            return EnergyMask.Missing(units, cost) < before;
        }

        /// <summary>Rough damage an attack will do right now, including the scaling ("x") attacks in the pool.</summary>
        internal static double EstimateDamage(CardDef card, int index, PokemonView attacker, PlayerView v)
        {
            var atk = card.Attacks[index];
            switch (atk.Name)
            {
                case "Night Joker":
                    double best = 0;
                    foreach (var b in v.Me.Bench)
                    {
                        var d = b.Top.Def;
                        if (!d.Name.StartsWith("N's ")) continue;
                        for (int i = 0; i < d.Attacks.Count; i++)
                            if (d.Attacks[i].Name != "Night Joker") best = Math.Max(best, EstimateDamage(d, i, attacker, v));
                    }
                    return best;
                case "Tenacious Tail":
                    int ex = (v.Opponent.Active != null && v.Opponent.Active.Top.Def.IsEx ? 1 : 0) + v.Opponent.Bench.Count(b => b.Top.Def.IsEx);
                    return atk.Damage * ex;
                case "Powerful Rage":
                    return atk.Damage * (attacker == null ? 0 : attacker.DamageCounters);
                case "Irritated Outburst":
                    return atk.Damage * Math.Max(0, 6 - v.Opponent.PrizeCount);
                case "Phantom Dive":
                    return atk.Damage + 60;
                case "Rampaging Thunder":
                    return atk.Damage - 30; // can't attack next turn
                case "Energized Slap":
                    return atk.Damage * (attacker == null ? 0 : attacker.EnergyUnits.Count);
                case "Do the Wave":
                    return atk.Damage * v.Me.Bench.Count;
                case "Rocket Rush":
                    return atk.Damage * ((v.Me.Active != null && v.Me.Active.Top.Def.IsTeamRockets ? 1 : 0) + v.Me.Bench.Count(b => b.Top.Def.IsTeamRockets));
                case "Powerful Hand":
                    return 20 * v.Me.HandCount;
                case "Fickle Spitting":
                    return v.Opponent.PrizeCount == 3 || v.Opponent.PrizeCount == 4 ? atk.Damage : 0;
                default:
                    if (atk.DamageSuffix == "x" || atk.DamageSuffix == "×") return atk.Damage * 2;
                    return atk.Damage;
            }
        }

        /// <summary>Conversion 3's type: our Active Pokémon's type first, then a type on our Bench.</summary>
        private static double ConversionScore(DecisionOption o, PlayerView v)
        {
            if (o.Label == null) return 0;
            double s = 0;
            if (v.Me.Active != null && v.Me.Active.Top.Def.PokemonType.ToString() == o.Label) s += 2;
            if (v.Me.Bench.Any(b => b.Top.Def.PokemonType.ToString() == o.Label)) s += 1;
            return s;
        }

        private static double OptionScore(DecisionOption o, PlayerView v)
        {
            if (o.Card != null && o.Card.IsPokemon && o.Index >= 0 && o.Index < o.Card.Attacks.Count)
                return EstimateDamage(o.Card, o.Index, v.Me.Active, v);
            if (o.Label != null && o.Label.Contains("more damage")) return 1;
            // Lost Origin: Arc Phone keeps the top card; Seething Currents hits a big opposing hand or refreshes a small own one.
            if (o.Label == "Don't switch") return 0.5;
            // Cosmic Eclipse: Will's chosen coin is heads.
            if (o.Label == "Heads") return 1;
            if (o.Label == "Seething Currents: your opponent") return v.Opponent.HandCount >= 5 ? 2 : 0;
            if (o.Label == "Seething Currents: you") return v.Me.HandCount <= 2 ? 1 : 0;
            return 0;
        }

        // ---------------------------------------------------------- Card and Pokémon choices

        private static double StartingActiveScore(CardDef d)
        {
            if (d == null) return 0;
            switch (d.Name)
            {
                case "Budew": return 60;
                case "Dunsparce": return 55;
                case "Yveltal": return 50;
                case "Munkidori": return 40;
                case "N's Zekrom": return 38;
                case "Dreepy": return 35;
                case "N's Zorua": return 30;
                case "N's Reshiram": return 30;
                case "Fezandipiti ex": return 25;
                case "Pecharunt ex": return 20;
                case "Meowth ex": return 15;
                case "Snorlax Doll": return 5; // can't attack or retreat
                default: return d.IsEx ? 20 : 35;
            }
        }

        private static double BenchScore(CardDef d)
        {
            if (d == null) return 0;
            switch (d.Name)
            {
                case "Dreepy": return 85;
                case "N's Zorua": return 85;
                case "Munkidori": return 60;
                case "Meowth ex": return 58;
                case "N's Zekrom": return 55;
                case "Yveltal": return 50;
                case "Pecharunt ex": return 48;
                case "N's Reshiram": return 45;
                case "Dunsparce": return 45;
                case "Fezandipiti ex": return 40;
                case "Budew": return 20;
                case "Snorlax Doll": return 10;
                case "Rare Fossil": return 40;
                default: return d.IsEx ? 35 : 55;
            }
        }

        private static double PromoteScore(PokemonView p, PlayerView v)
        {
            if (p == null) return 0;
            double s = p.RemainingHp / 100.0;
            if (CanAttackNow(p)) s += 50;
            var n = p.Top.Def.Name;
            if (n == "Dragapult ex" || n == "N's Zoroark ex") s += 20;
            return s;
        }

        /// <summary>"Choose up to N Pokémon" decisions (Tool Scrapper, Twin Shotels, Janine's Secret Art...).</summary>
        private static void ScoreManyPokemon(ScoredChoice c, DecisionRequest r)
        {
            var v = r.View;
            string prompt = r.Prompt ?? "";
            Func<DecisionOption, double> score = o =>
            {
                var p = v.FindPokemon(o.PokemonUid);
                if (p == null) return 0;
                bool theirs = p.Owner != v.Viewer;
                if (prompt.StartsWith("Tool Scrapper")) return theirs ? 10 : -10;
                if (theirs) return 500 - p.RemainingHp + (p.Top.Def.IsEx ? 15 : 0);
                return p.RemainingHp / 100.0 + (v.Me.Active != null && v.Me.Active.Uid == p.Uid ? 5 : 0);
            };
            int want = r.MaxChoices;
            if (prompt.StartsWith("Tool Scrapper"))
                want = Math.Max(r.MinChoices, r.Options.Count(o => { var p = v.FindPokemon(o.PokemonUid); return p != null && p.Owner != v.Viewer; }));
            Fill(c, r, score, false);
            c.Count = want;
        }

        private static void ScorePokemon(ScoredChoice c, DecisionRequest r)
        {
            var v = r.View;
            bool opponents = r.Options.All(o => o.Label.StartsWith("Opponent's"));
            string prompt = r.Prompt ?? "";
            if (opponents)
            {
                // Finish off whatever is closest to being Knocked Out; prefer Pokémon ex on ties.
                Fill(c, r, o =>
                {
                    var p = v.FindPokemon(o.PokemonUid);
                    if (p == null) return 0;
                    return -p.RemainingHp + (p.Top.Def.IsEx ? 15 : 0);
                }, true);
                return;
            }
            if (prompt.Contains("move damage counters from"))
            {
                Fill(c, r, o => { var p = v.FindPokemon(o.PokemonUid); return p == null ? 0 : p.DamageCounters; }, true);
                return;
            }
            if (prompt.StartsWith("Attach") && r.Source != null && r.Source.TrainerKind == TrainerKind.Tool)
            {
                // Tools: Punk Helmet wants a Darkness Pokémon in the Active Spot.
                Fill(c, r, o =>
                {
                    var p = v.FindPokemon(o.PokemonUid);
                    if (p == null) return 0;
                    double s = 0;
                    if (v.Me.Active != null && v.Me.Active.Uid == p.Uid) s += 50;
                    if (p.Top.Def.PokemonType == EnergyType.Darkness) s += 30;
                    // Evolutions (v92): a Spirit Link goes on the Pokémon-EX it's for.
                    if (r.Source.Name.EndsWith(" Spirit Link") && p.Top.Def.Name == r.Source.Name.Substring(0, r.Source.Name.Length - " Spirit Link".Length) + "-EX") s += 100;
                    // Fates Collide (v94): Power Memory only works on Zygarde-EX.
                    if (r.Source.Name == "Power Memory" && p.Top.Def.Name == "Zygarde-EX") s += 100;
                    // BREAKpoint (v96): Fighting Fury Belt only works on a Basic Pokémon.
                    if (r.Source.Name == "Fighting Fury Belt" && p.Top.Def.IsBasicPokemon) s += 60;
                    // BREAKthrough (v97): Float Stone where retreating costs the most; Heavy Boots only works with a Retreat Cost of 3 or more.
                    if (r.Source.Name == "Float Stone") s += 15 * p.Top.Def.RetreatCost;
                    if (r.Source.Name == "Heavy Boots" && p.Top.Def.RetreatCost >= 3) s += 60;
                    // Phantom Forces (v102): Head Ringer and Jamming Net on the opponent's strongest Pokémon-EX.
                    if ((r.Source.Name == "Head Ringer" || r.Source.Name == "Jamming Net") && p.Owner != v.Viewer) s += 100 + (v.Opponent.Active != null && v.Opponent.Active.Uid == p.Uid ? 50 : 0);
                    // Roaring Skies (v99): Healing Scarf on the Pokémon with the most damage.
                    if (r.Source.Name == "Healing Scarf") s += 5 * p.DamageCounters;
                    return s + p.RemainingHp / 100.0;
                }, true);
                return;
            }
            if (prompt.StartsWith("Subjugating Chains"))
            {
                Fill(c, r, o => { var p = v.FindPokemon(o.PokemonUid); return p == null ? 0 : (CanAttackNow(p) ? 50 : 0) + p.RemainingHp / 100.0; }, true);
                return;
            }

            Fill(c, r, o =>
            {
                var p = v.FindPokemon(o.PokemonUid);
                if (p == null) return 0;
                double s = p.RemainingHp / 100.0;
                switch (p.Top.Def.Name)
                {
                    case "Dragapult ex": s += 30; break;
                    case "N's Zoroark ex": s += 30; break;
                    case "Drakloak": s += 20; break;
                    case "Dreepy": s += 10; break;
                    case "N's Zorua": s += 10; break;
                }
                return s;
            }, true);
        }

        /// <summary>Card choices. Discards score the cards by how little we want to keep them.</summary>
        private static void ScoreCards(ScoredChoice c, DecisionRequest r)
        {
            string prompt = (r.Prompt ?? "").ToLowerInvariant();
            var v = r.View;
            if (prompt.Contains("damage each)"))
            {
                Fill(c, r, o => 1, false);
                c.Count = CardsForKnockOut(prompt, r);
                return;
            }
            if (prompt.Contains("opponent's hand"))
            {
                Fill(c, r, o => KeepValue(o.Card, v), false);
                c.Count = r.MaxChoices;
                return;
            }
            if (prompt.Contains("discard"))
            {
                // Discarding from the opponent's Pokémon (Crushing Hammer): take what they value most.
                if (prompt.Contains("energy to discard") && r.Source != null && r.Source.Name == "Crushing Hammer")
                {
                    Fill(c, r, o => 1, false);
                    c.Count = r.MinChoices;
                    return;
                }
                Fill(c, r, o => -KeepValue(o.Card, v), false);
                c.Count = Math.Max(1, r.MinChoices);
                return;
            }
            Fill(c, r, o => KeepValue(o.Card, v), false);
            c.Count = r.MaxChoices;
        }

        /// <summary>
        /// "(N damage each)" choices (Twin Cannons, Inferno X, Garland Ray): just enough cards to Knock Out the
        /// opponent's Active Pokémon, or all of them when even that isn't enough.
        /// </summary>
        private static int CardsForKnockOut(string prompt, DecisionRequest r)
        {
            int end = prompt.IndexOf(" damage each)", StringComparison.Ordinal);
            int start = end;
            while (start > 0 && char.IsDigit(prompt[start - 1])) start--;
            int each;
            var target = r.View.Opponent.Active;
            if (end <= 0 || start == end || !int.TryParse(prompt.Substring(start, end - start), out each) || each <= 0 || target == null)
                return r.MaxChoices;
            return Math.Max(1, (target.RemainingHp + each - 1) / each);
        }

        private static double KeepValue(CardDef d, PlayerView v)
        {
            if (d == null) return 0;
            var mine = new List<string>();
            if (v.Me.Active != null) mine.Add(v.Me.Active.Top.Def.Name);
            mine.AddRange(v.Me.Bench.Select(b => b.Top.Def.Name));
            switch (d.Name)
            {
                case "Dreepy": return mine.Count(n => IsDragapultLine(n)) < 3 ? 90 : 50;
                case "Drakloak": return mine.Contains("Dreepy") ? 88 : 55;
                case "Dragapult ex": return mine.Contains("Drakloak") ? 92 : 60;
                case "N's Zorua": return mine.Count(n => n == "N's Zorua" || n == "N's Zoroark ex") < 3 ? 90 : 50;
                case "N's Zoroark ex": return mine.Contains("N's Zorua") ? 92 : 60;
                case "N's Zekrom": return 60;
                case "N's Reshiram": return 50;
                case "Pecharunt ex": return 50;
                case "Yveltal": return 50;
                case "Munkidori": return 58;
                case "Fezandipiti ex": return 48;
                case "Meowth ex": return 52;
                case "Lillie's Determination": return 70;
                case "Crispin": return 66;
                case "Boss's Orders": return 62;
                case "Cyrano": return 60;
                case "Black Belt's Training": return 58;
                case "Judge": return 52;
                case "Rosa's Encouragement": return 50;
                case "Buddy-Buddy Poffin": return 64;
                case "Poké Pad": return 63;
                case "N's PP Up": return 55;
                case "Ultra Ball": return 56;
                case "Punk Helmet": return 45;
                case "Fire Energy":
                case "Psychic Energy": return 57;
                case "Darkness Energy": return mine.Any(n => n.StartsWith("N's ")) ? 60 : 45;
                default: return d.IsPokemon ? 50 : 40;
            }
        }
    }
}
