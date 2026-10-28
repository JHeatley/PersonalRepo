using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Lost Thunder (LOT, v81), part 5: Trainers and Memory Energy (LOT 167-194). Prism Star Stadiums ignore Item and Supporter
    /// cards (v80 engine rule). "A [T] Energy card" outside play is basic Energy only (your ruling, v65). Custom Catcher and Mixed
    /// Herbs can be played 2 at once: playing 1 asks whether to play a second copy from the hand with it.
    /// </summary>
    public static class CardsLOT5
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Adventure Bag", new AdventureBag());
            r.RegisterByName("Choice Helmet", new ChoiceHelmet());
            r.RegisterByName("Custom Catcher", new CustomCatcher());
            r.RegisterByName(CardsTEU2.Electropower, new ElectropowerCard());
            r.RegisterByName("Faba", new Faba());
            r.RegisterByName("Fairy Charm Grass", new FairyCharmLOT(EnergyType.Grass));
            r.RegisterByName("Fairy Charm Psychic", new FairyCharmLOT(EnergyType.Psychic));
            r.RegisterByName("Fairy Charm Fighting", new FairyCharmLOT(EnergyType.Fighting));
            r.RegisterByName("Fairy Charm Dragon", new FairyCharmLOT(EnergyType.Dragon));
            r.RegisterByName("Heat Factory ◇", new HeatFactory());
            r.RegisterByName("Kahili", new Kahili());
            r.RegisterByName("Life Forest ◇", new LifeForest());
            r.RegisterByName("Lost Blender", new LostBlender());
            r.RegisterByName("Lusamine ◇", new LusaminePrismStar());
            r.RegisterByName("Mina", new Mina());
            r.RegisterByName("Mixed Herbs", new MixedHerbs());
            r.RegisterByName("Moomoo Milk", new MoomooMilk());
            r.RegisterByName("Morty", new Morty());
            r.RegisterByName("Net Ball", new NetBall());
            r.RegisterByName("Professor Elm's Lecture", new ProfessorElmsLecture());
            r.RegisterByName("Sightseer", new Sightseer());
            r.RegisterByName("Spell Tag", new SpellTag());
            r.RegisterByName("Thunder Mountain ◇", new ThunderMountain());
            r.RegisterByName("Wait and See Hammer", new WaitAndSeeHammer());
            r.RegisterByName("Whitney", new Whitney());
            r.RegisterByName("Memory Energy", new MemoryEnergy());
        }

        /// <summary>
        /// "You may play 2 [this card] at once": asks whether to play another copy from the hand along with this one. The second
        /// copy is played too (it goes to the discard pile). Returns true if 2 were played.
        /// </summary>
        internal static bool PlaySecondCopy(EffectContext ctx, string prompt)
        {
            var name = ctx.Card.Def.Name;
            var other = ctx.Me.Hand.FirstOrDefault(c => c != ctx.Card && c.Def.Name == name);
            if (other == null || !ctx.AskYesNo(prompt)) return false;
            ctx.Me.Hand.Remove(other);
            ctx.Me.Discard.Add(other);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " played 2 " + name + " cards at once.");
            return true;
        }

        private static bool HasSecondCopy(EffectContext ctx)
        {
            var name = ctx.Card.Def.Name;
            return ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.Name == name);
        }

        // ================================================================== Items

        /// <summary>Adventure Bag: search your deck for up to 2 Pokémon Tool cards, reveal them and put them into your hand.</summary>
        private sealed class AdventureBag : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTool, 2, "Adventure Bag: choose up to 2 Pokémon Tool cards");
            }
        }

        /// <summary>
        /// Custom Catcher: play 1 to draw until you have 3 cards in your hand; play 2 at once to switch 1 of the opponent's Benched
        /// Pokémon with their Active Pokémon (once for the 2 cards).
        /// </summary>
        private sealed class CustomCatcher : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                bool draw = ctx.Me.Hand.Count - 1 < 3 && ctx.Me.Deck.Count > 0;
                bool gust = HasSecondCopy(ctx) && ctx.GustTargets().Count > 0;
                return draw || gust;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool canGust = ctx.GustTargets().Count > 0;
                if (canGust && PlaySecondCopy(ctx, "Custom Catcher: play 2 Custom Catcher at once to switch 1 of your opponent's Benched Pokémon with their Active Pokémon? (No: draw until you have 3 cards.)"))
                {
                    ctx.GustOpponent("Custom Catcher: choose 1 of your opponent's Benched Pokémon to switch with their Active Pokémon");
                    return;
                }
                if (ctx.Me.Hand.Count < 3) ctx.DrawUntil(3);
            }
        }

        /// <summary>Electropower: during this turn, your Lightning Pokémon's attacks do 30 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class ElectropowerCard : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new ElectropowerEffect("Your Lightning Pokémon's attacks do 30 more damage to the opponent's Active Pokémon this turn (Electropower)", ctx.Player, ctx.Turn));
            }
        }

        internal sealed class ElectropowerEffect : ContinuousEffect
        {
            public ElectropowerEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || target == null || attacker.Owner != Owner || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
                return engine.IsType(attacker, EnergyType.Lightning) ? damage + 30 : damage;
            }
        }

        /// <summary>Lost Blender: put 2 cards from your hand in the Lost Zone; if you do, draw a card.</summary>
        private sealed class LostBlender : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count < 2) return;
                var picks = ctx.ChooseCards(hand, 2, 2, "Lost Blender: put 2 cards from your hand in the Lost Zone");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card))
                    {
                        CardsCRZ1.PutInLostZone(ctx, card);
                        n++;
                    }
                if (n == 2) ctx.Draw(1);
            }
        }

        /// <summary>
        /// Mixed Herbs: play 1 to remove a Special Condition from your Active Pokémon; play 2 at once to heal 90 damage from it and
        /// remove all Special Conditions (once for the 2 cards).
        /// </summary>
        private sealed class MixedHerbs : CardScript
        {
            private static readonly SpecialCondition[] All =
            {
                SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned
            };

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return false;
                if (active.Conditions != SpecialCondition.None) return true;
                return HasSecondCopy(ctx) && active.DamageCounters > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                bool worthTwo = active.DamageCounters > 0 || active.Conditions != SpecialCondition.None;
                if (worthTwo && PlaySecondCopy(ctx, "Mixed Herbs: play 2 Mixed Herbs at once to heal 90 damage from " + active.Name + " and remove all Special Conditions? (No: remove 1 Special Condition.)"))
                {
                    ctx.Heal(active, 90);
                    if (active.Conditions != SpecialCondition.None && ctx.CanAffect(active)) ctx.Engine.RemoveAllConditions(active);
                    return;
                }
                var present = All.Where(c => active.Has(c)).ToList();
                if (present.Count == 0) return;
                var pick = present[ctx.ChooseOneOption("Mixed Herbs: remove which Special Condition from " + active.Name + "?", present.Select(c => c.ToString()).ToArray())];
                if (!ctx.CanAffect(active)) return;
                ctx.Engine.RemoveCondition(active, pick);
                ctx.Log(active.Name + " is no longer " + pick + ".");
            }
        }

        /// <summary>Moomoo Milk: choose 1 of your Pokémon and flip 2 coins; heal 30 damage from it for each heads.</summary>
        private sealed class MoomooMilk : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Moomoo Milk: choose 1 of your Pokémon");
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.Heal(target, 30 * heads);
            }
        }

        /// <summary>Net Ball: search your deck for a Basic Grass Pokémon or a Grass Energy card, reveal it and put it into your hand.</summary>
        private sealed class NetBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => (d.IsBasicPokemon && d.PokemonType == EnergyType.Grass) || CardsTEU1.BasicOf(d, EnergyType.Grass), 1,
                    "Net Ball: choose a Basic Grass Pokémon or a Grass Energy card");
            }
        }

        /// <summary>Wait and See Hammer: only if you went second, during your first turn: discard an Energy from 1 of the opponent's Pokémon.</summary>
        private sealed class WaitAndSeeHammer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return CardsUNB1.SecondPlayersFirstTurn(ctx) && ctx.OpponentPokemon().Any(m => m.Energy.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var withEnergy = ctx.OpponentPokemon().Where(m => m.Energy.Count > 0).ToList();
                if (withEnergy.Count == 0) return;
                var target = ctx.ChoosePokemon(withEnergy, "Wait and See Hammer: choose 1 of your opponent's Pokémon");
                var energy = ctx.ChooseCards(target.Energy, 1, 1, "Wait and See Hammer: choose an Energy to discard")[0];
                ctx.Engine.DiscardEnergy(target, energy);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Choice Helmet: the holder takes 30 less damage from attacks by the opponent's Pokémon-GX and Pokémon-EX (after Weakness and Resistance).</summary>
        private sealed class ChoiceHelmet : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || !info.AgainstOpponent || !info.Attacker.Def.IsGXorEX) return damage;
                return damage - 30;
            }
        }

        /// <summary>Fairy Charm [type]: prevent all damage done to the Fairy Pokémon holding this by attacks from the opponent's [type] Pokémon-GX and Pokémon-EX.</summary>
        private sealed class FairyCharmLOT : CardScript
        {
            private readonly EnergyType type;

            public FairyCharmLOT(EnergyType type) { this.type = type; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                var attacker = info.Attacker;
                if (holder == null || info.Target != holder || attacker == null || !info.AgainstOpponent) return false;
                if (!src.Engine.IsType(holder, EnergyType.Fairy) || !attacker.Def.IsGXorEX) return false;
                return src.Engine.IsType(attacker, type);
            }
        }

        /// <summary>Spell Tag: when the Psychic Pokémon holding this is Knocked Out by damage from an opponent's attack, put 4 damage counters on the opponent's Pokémon in any way you like.</summary>
        private sealed class SpellTag : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var holder = src.Pokemon;
                if (!byOpponentsAttack || holder == null || !src.Engine.IsType(holder, EnergyType.Psychic)) return;
                if (src.Opp.InPlay().Any()) src.PlaceCountersAnyWay(4, () => src.OpponentPokemon());
            }
        }

        // ================================================================== Stadiums

        /// <summary>Heat Factory ◇: once during each player's turn, that player may discard a Fire Energy card from their hand to draw 3 cards.</summary>
        private sealed class HeatFactory : StadiumAction
        {
            public override bool IgnoresItemsAndSupporters { get { return true; } }

            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(c => CardsTEU1.BasicOf(c.Def, EnergyType.Fire)); }

            protected override bool Use(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return false;
                var card = ctx.ChooseCards(fire, 1, 1, "Heat Factory: discard a Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(3);
                return false;
            }
        }

        /// <summary>Life Forest ◇: once during each player's turn, that player may heal 60 damage and remove all Special Conditions from 1 of their Grass Pokémon.</summary>
        private sealed class LifeForest : StadiumAction
        {
            public override bool IgnoresItemsAndSupporters { get { return true; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass) && (m.DamageCounters > 0 || m.Conditions != SpecialCondition.None));
            }

            protected override bool Ready(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return false;
                var target = ctx.ChoosePokemon(targets, "Life Forest: heal 60 damage and remove all Special Conditions from which Grass Pokémon?");
                if (target.DamageCounters > 0) ctx.Heal(target, 60);
                if (target.Conditions != SpecialCondition.None && !ctx.Engine.StadiumShielded(target)) ctx.Engine.RemoveAllConditions(target);
                return false;
            }
        }

        /// <summary>Thunder Mountain ◇: attacks of Lightning Pokémon (both players') cost 1 Lightning Energy less.</summary>
        private sealed class ThunderMountain : CardScript
        {
            public override bool IgnoresItemsAndSupporters { get { return true; } }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || !src.Engine.IsType(mon, EnergyType.Lightning)) return;
                cost.Remove(EnergyType.Lightning);
            }
        }

        // ================================================================== Supporters

        /// <summary>Faba: put a Pokémon Tool or Special Energy card attached to 1 of the opponent's Pokémon, or any Stadium card in play, in the Lost Zone.</summary>
        private sealed class Faba : CardScript
        {
            private static void Options(EffectContext ctx, List<string> labels, List<PokemonInPlay> holders, List<CardInstance> cards)
            {
                foreach (var mon in ctx.OpponentPokemon())
                {
                    if (!ctx.CanAffect(mon)) continue;
                    foreach (var t in mon.Tools)
                    {
                        labels.Add(t.Def.Name + " (on " + ctx.Engine.OwnedName(mon) + ")");
                        holders.Add(mon);
                        cards.Add(t);
                    }
                    foreach (var e in mon.Energy)
                    {
                        if (!e.Def.IsAttachedSpecialEnergy) continue;
                        labels.Add(e.Def.Name + " (on " + ctx.Engine.OwnedName(mon) + ")");
                        holders.Add(mon);
                        cards.Add(e);
                    }
                }
            }

            private static CardInstance OpenStadium(EffectContext ctx)
            {
                var stadium = ctx.State.Stadium;
                if (stadium == null || ctx.Engine.StadiumIgnoresCurrentTrainer()) return null;
                return stadium;
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                if (ctx.State.Stadium != null && !ctx.Engine.Scripts.For(ctx.State.Stadium.Def).IgnoresItemsAndSupporters) return true;
                return ctx.OpponentPokemon().Any(m => m.Tools.Count > 0 || m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var labels = new List<string>();
                var holders = new List<PokemonInPlay>();
                var cards = new List<CardInstance>();
                Options(ctx, labels, holders, cards);
                var stadium = OpenStadium(ctx);
                if (stadium != null) labels.Add(stadium.Def.Name + " (Stadium)");
                if (labels.Count == 0) return;
                int pick = ctx.ChooseOption("Faba: put which Pokémon Tool, Special Energy or Stadium in the Lost Zone?", labels.ToArray());
                if (pick < 0) return;
                if (pick < cards.Count)
                {
                    var holder = holders[pick];
                    var card = cards[pick];
                    if (holder.Tools.Remove(card)) CardsCRZ1.PutInLostZone(ctx, card);
                    else if (holder.Energy.Contains(card)) CardsLOR1.EnergyToLostZone(ctx, holder, card);
                    return;
                }
                if (stadium == null || ctx.State.Stadium != stadium) return;
                int owner = ctx.State.StadiumOwner;
                var partner = ctx.Engine.TakeStadiumPartner(); // the other half of a Legendary Stadium goes too (v152)
                ctx.State.Stadium = null;
                ctx.State.StadiumOwner = -1;
                CardsCRZ1.PutInLostZone(ctx, stadium);
                if (partner != null) CardsCRZ1.PutInLostZone(ctx, partner);
                ctx.Engine.Scripts.For(stadium.Def).OnLeavePlay(new EffectContext(ctx.Engine, owner, stadium, null));
            }
        }

        /// <summary>
        /// Kahili: draw 2 cards, then flip a coin; on heads, if you played this Kahili from your hand, it goes into your hand instead of
        /// the discard pile. Can't be played with an empty deck.
        /// </summary>
        private sealed class Kahili : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                bool playedFromHand = ctx.Card != null && ctx.Card.Owner == ctx.Player && ctx.Engine.ResolvingTrainer == ctx.Card.Def
                                      && !ctx.Me.Hand.Contains(ctx.Card) && !ctx.Me.Discard.Contains(ctx.Card);
                if (!ctx.Flip() || !playedFromHand) return;
                ctx.Me.Hand.Add(ctx.Card);
                ctx.CardPlacedElsewhere = true;
                ctx.Log("Kahili went back into " + ctx.Engine.Their(ctx.Player) + " hand.");
            }
        }

        /// <summary>Lusamine ◇: only if the opponent has exactly 3 Prize cards left. Prevent all damage done to your Ultra Beasts by attacks during the opponent's next turn.</summary>
        private sealed class LusaminePrismStar : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Prizes.Count == 3; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new UltraBeastGuardEffect("Prevent all damage done to " + ctx.Engine.PlayerName(ctx.Player) + "'s Ultra Beasts by attacks during the opponent's next turn (Lusamine ◇)", ctx.Player, ctx.Turn + 1));
            }
        }

        internal sealed class UltraBeastGuardEffect : ContinuousEffect
        {
            public UltraBeastGuardEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Owner == Owner && info.Target.Def.IsUltraBeast && info.Attacker != null && info.AgainstOpponent;
            }
        }

        /// <summary>Mina: search your deck for a Fairy Energy card and attach it to 1 of your Pokémon.</summary>
        private sealed class Mina : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsTEU1.BasicOf(d, EnergyType.Fairy), 1, "Mina: choose a Fairy Energy card");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Mina: attach " + found[0].Def.Name + " to which of your Pokémon?");
                    ctx.Engine.AttachEnergyCard(found[0], target);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Morty: only if 1 of your Psychic Pokémon was Knocked Out during the opponent's last turn. The opponent reveals their hand;
        /// choose 2 cards there and they shuffle those into their deck.
        /// </summary>
        private sealed class Morty : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.WasKnockedOutLastOpponentTurn(d => d.PokemonType == EnergyType.Psychic, false);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Opp.Hand.Count == 0) return;
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand.ToList();
                int n = Math.Min(2, hand.Count);
                var picks = hand.Count <= 2 ? hand : ctx.ChooseCards(hand, n, n, "Morty: choose 2 cards from your opponent's hand to shuffle into their deck");
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + picks.Count + (picks.Count == 1 ? " card" : " cards") + " from their hand into their deck (Morty).");
            }
        }

        /// <summary>Professor Elm's Lecture: search your deck for up to 3 Pokémon with 60 HP or less, reveal them and put them into your hand.</summary>
        private sealed class ProfessorElmsLecture : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.Hp > 0 && d.Hp <= 60, 3, "Professor Elm's Lecture: choose up to 3 Pokémon with 60 HP or less");
            }
        }

        /// <summary>Sightseer: discard any number of cards from your hand, then draw until you have 5 cards. Not playable if you can't draw.</summary>
        private sealed class Sightseer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count > 0)
                {
                    var picks = ctx.ChooseCards(hand, 0, hand.Count, "Sightseer: discard any number of cards from your hand (then draw until you have 5)");
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                }
                if (ctx.Me.Hand.Count < 5) ctx.DrawUntil(5);
            }
        }

        /// <summary>Whitney: draw a card, then 2 more cards for each other Whitney in your discard pile.</summary>
        private sealed class Whitney : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int others = ctx.Me.Discard.Count(c => c != ctx.Card && c.Def.Name == "Whitney");
                ctx.Draw(1);
                if (others > 0) ctx.Draw(2 * others);
            }
        }

        // ================================================================== Energy

        /// <summary>Memory Energy: provides Colorless; the holder can use any attack from its previous Evolutions (still paying their cost).</summary>
        private sealed class MemoryEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return new List<int> { EnergyMask.Colorless }; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Stack.Count < 2) yield break;
                for (int k = 0; k < mon.Stack.Count - 1; k++)
                {
                    var def = mon.Stack[k].Def;
                    if (!def.IsPokemon) continue;
                    for (int i = 0; i < def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(def, i);
                }
            }
        }
    }
}
