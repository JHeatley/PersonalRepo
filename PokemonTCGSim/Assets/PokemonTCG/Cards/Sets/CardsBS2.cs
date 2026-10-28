using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Base Set (BS, v113): the Trainers (70-95). Every old Trainer plays as an Item (no Supporter limit). Clefairy Doll follows the
    /// Antique Fossil rules (10 HP, no Prize; your ruling: only played from the hand, never in setup). Imposter Professor Oak,
    /// Maintenance, Pokémon Center, Pokédex and Switch share their reprints' scripts. Your ruling (v113): Computer Search is a
    /// normal Item here, not an ACE SPEC. PlusPower and Defender follow their 2011 errata as timed effects (the card goes to the
    /// discard pile at once).
    /// </summary>
    public static class CardsBS2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BS-70", new CardScript()); // Clefairy Doll: played like an Antique Fossil (engine)
            r.RegisterByKey("BS-71", new ComputerSearchBS());
            r.RegisterByKey("BS-72", new DevolutionSprayBS());
            r.RegisterByKey("BS-74", new ItemFinder());
            r.RegisterByKey("BS-75", new Lass());
            r.RegisterByKey("BS-76", new RareCandy());
            r.RegisterByKey("BS-77", new PokemonTrader());
            r.RegisterByKey("BS-78", new ScoopUp());
            r.RegisterByKey("BS-79", new SuperEnergyRemoval());
            r.RegisterByKey("BS-80", new Defender());
            r.RegisterByKey("BS-81", new EnergyRetrievalBS());
            r.RegisterByKey("BS-82", new FullHealBS());
            r.RegisterByKey("BS-84", new PlusPower());
            r.RegisterByKey("BS-86", new PokemonFlute());
            r.RegisterByKey("BS-88", new ProfessorOak());
            r.RegisterByKey("BS-89", new ReviveBS());
            r.RegisterByKey("BS-90", new SuperPotionBS());
            r.RegisterByKey("BS-91", new Bill());
            r.RegisterByKey("BS-92", new EnergyRemoval());
            r.RegisterByKey("BS-93", new GustOfWind());
            r.RegisterByKey("BS-94", new PotionBS());
        }

        private static List<CardInstance> OtherCards(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card).ToList(); }

        /// <summary>Pays "discard N of the other cards from your hand": returns false if the hand no longer has them.</summary>
        private static bool DiscardOthers(EffectContext ctx, int count, string prompt)
        {
            var others = OtherCards(ctx);
            if (others.Count < count) return false;
            var picks = others.Count == count ? others : ctx.ChooseCards(others, count, count, prompt);
            foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
            return true;
        }

        /// <summary>Computer Search: discard 2 of the other cards from your hand to search your deck for any card.</summary>
        private sealed class ComputerSearchBS : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return OtherCards(ctx).Count >= 2 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!DiscardOthers(ctx, 2, "Computer Search: discard 2 cards from your hand")) return;
                ctx.SearchToHand(d => true, 1, "Computer Search: put a card from your deck into your hand", false, 1);
            }
        }

        /// <summary>
        /// Devolution Spray: choose 1 of your evolved Pokémon and a Stage; discard all its Evolution cards of that Stage or higher. It
        /// recovers from Special Conditions and other attack effects, as if it had evolved.
        /// </summary>
        private sealed class DevolutionSprayBS : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Stack.Count > 1 && m.Def.IsEvolution && !m.Def.IsMultiCard);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Devolution Spray: devolve which of your Pokémon?");
                if (target == null) return;
                var evolutions = target.Stack.Where(c => c.Def.IsEvolution).ToList();
                int count = 1;
                if (evolutions.Count > 1)
                {
                    var labels = new[] { "Discard " + evolutions[evolutions.Count - 1].Def.Name, "Discard " + string.Join(" and ", evolutions.Select(c => c.Def.Name).ToArray()) };
                    count = ctx.ChooseOption("Devolution Spray: discard which Evolution cards?", labels) == 1 ? evolutions.Count : 1;
                }
                foreach (var card in ctx.Engine.Devolve(target, count))
                    if (ctx.State.Players[card.Owner].Hand.Remove(card)) ctx.State.Players[card.Owner].Discard.Add(card);
            }
        }

        /// <summary>Item Finder: discard 2 of the other cards from your hand to put a Trainer card from your discard pile into your hand.</summary>
        private sealed class ItemFinder : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return OtherCards(ctx).Count >= 2 && ctx.Me.Discard.Any(c => c.Def.IsTrainer); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!DiscardOthers(ctx, 2, "Item Finder: discard 2 cards from your hand")) return;
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer && c != ctx.Card).ToList();
                if (trainers.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(trainers, 1, 1, "Item Finder: put a Trainer card from your discard pile into your hand"));
            }
        }

        /// <summary>Lass: both players show their hands, then shuffle all the Trainer cards from them into their decks.</summary>
        private sealed class Lass : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                for (int k = 0; k < 2; k++)
                {
                    int player = k == 0 ? ctx.Player : 1 - ctx.Player;
                    var p = ctx.State.Players[player];
                    var hand = p.Hand.Where(c => c != ctx.Card).ToList();
                    if (hand.Count > 0) ctx.Engine.Reveal(player, hand, "hand (Lass)");
                    var trainers = hand.Where(c => c.Def.IsTrainer).ToList();
                    foreach (var card in trainers) if (p.Hand.Remove(card)) p.Deck.Add(card);
                    if (trainers.Count > 0) ctx.Engine.ShuffleDeck(player);
                }
            }
        }

        /// <summary>Pokémon Trader: trade a Pokémon card from your hand for a Pokémon card from your deck; show both cards.</summary>
        private sealed class PokemonTrader : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && OtherCards(ctx).Any(c => c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = OtherCards(ctx).Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count == 0) return;
                var give = ctx.ChooseCards(mons, 1, 1, "Pokémon Trader: choose a Pokémon card from your hand to trade")[0];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { give }, "Pokémon card to trade (Pokémon Trader)");
                ctx.SearchToHand(d => d.IsPokemon, 1, "Pokémon Trader: put a Pokémon card from your deck into your hand", true, 0);
                if (ctx.Me.Hand.Remove(give))
                {
                    ctx.Me.Deck.Add(give);
                    ctx.ShuffleMyDeck();
                }
            }
        }

        /// <summary>Scoop Up: return 1 of your Pokémon's Basic Pokémon card to your hand and discard all cards attached to it.</summary>
        private sealed class ScoopUp : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = ctx.MyPokemon(m => ctx.Engine.CanPutIntoHand(m));
                if (mons.Count == 0) return;
                var target = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, "Scoop Up: return which of your Pokémon's Basic Pokémon card to your hand?");
                if (target == null) return;
                var basic = target.Stack[0];
                var rest = target.Stack.Skip(1).Concat(target.Energy).Concat(target.Tools).ToList();
                ctx.Engine.RemoveFromPlay(target);
                ctx.Me.Hand.Add(basic);
                ctx.Engine.DiscardFromPlay(rest);
                ctx.Log(basic.Def.Name + " was returned to your hand" + (rest.Count > 0 ? " and the cards attached to it were discarded" : "") + " (Scoop Up).");
            }
        }

        /// <summary>Super Energy Removal: discard an Energy card attached to 1 of your Pokémon to discard up to 2 Energy cards from 1 of your opponent's Pokémon.</summary>
        private sealed class SuperEnergyRemoval : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0 && ctx.OpponentPokemon(m => m.Energy.Count > 0).Count > 0 && CardsG1.NoRemovalGymCostPayable(ctx);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!CardsG1.PayNoRemovalGymCost(ctx)) return;
                var mine = ctx.MyPokemon(m => m.Energy.Count > 0);
                if (mine.Count == 0) return;
                var own = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Super Energy Removal: discard an Energy card from which of your Pokémon?");
                if (own == null) return;
                var pay = own.Energy.Count == 1 ? own.Energy.ToList() : ctx.ChooseCards(own.Energy.ToList(), 1, 1, "Super Energy Removal: discard which Energy card from " + own.Name + "?");
                foreach (var card in pay) ctx.Engine.DiscardEnergy(own, card);
                var theirs = ctx.OpponentPokemon(m => m.Energy.Count > 0);
                if (theirs.Count == 0) return;
                var target = theirs.Count == 1 ? theirs[0] : ctx.ChoosePokemon(theirs, "Super Energy Removal: choose 1 of your opponent's Pokémon");
                if (target == null) return;
                var picks = ctx.ChooseCards(target.Energy.ToList(), 1, Math.Min(2, target.Energy.Count), "Super Energy Removal: discard up to 2 Energy cards from " + target.Name);
                foreach (var card in picks) ctx.Engine.DiscardEnergy(target, card);
            }
        }

        /// <summary>Energy Removal: discard an Energy card attached to 1 of your opponent's Pokémon. With No Removal Gym in play (v122), discard 2 other cards from your hand first.</summary>
        private sealed class EnergyRemoval : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Energy.Count > 0).Count > 0 && CardsG1.NoRemovalGymCostPayable(ctx); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!CardsG1.PayNoRemovalGymCost(ctx)) return;
                var theirs = ctx.OpponentPokemon(m => m.Energy.Count > 0);
                if (theirs.Count == 0) return;
                var target = theirs.Count == 1 ? theirs[0] : ctx.ChoosePokemon(theirs, "Energy Removal: choose 1 of your opponent's Pokémon");
                if (target == null) return;
                var picks = target.Energy.Count == 1 ? target.Energy.ToList() : ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Energy Removal: discard which Energy card from " + target.Name + "?");
                foreach (var card in picks) ctx.Engine.DiscardEnergy(target, card);
            }
        }

        /// <summary>Defender (errata): until the end of your opponent's next turn, damage done to that Pokémon by attacks is reduced by 20 (after W/R).</summary>
        private sealed class Defender : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = ctx.MyPokemon();
                var target = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, "Defender: which of your Pokémon?");
                if (target == null) return;
                ctx.AddEffect(new DefenderEffect(target.Name + ": attacks do 20 less damage to it until the end of your opponent's next turn (Defender)", ctx.Player, target.Uid, ctx.Turn + 1));
            }
        }

        internal sealed class DefenderEffect : ContinuousEffect
        {
            public DefenderEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Target == null || info.Target.Uid != TargetUid) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>PlusPower (errata): this turn, your Pokémon's attacks do 10 more damage to your opponent's Active Pokémon (before W/R).</summary>
        private sealed class PlusPower : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new PlusPowerEffect("This turn, your Pokémon's attacks do 10 more damage to your opponent's Active Pokémon (PlusPower)", ctx.Player, ctx.Turn));
            }
        }

        internal sealed class PlusPowerEffect : ContinuousEffect
        {
            public PlusPowerEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Owner != Owner || target == null || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
                return damage + 10;
            }
        }

        /// <summary>Energy Retrieval: trade 1 of the other cards in your hand for up to 2 basic Energy cards from your discard pile.</summary>
        private sealed class EnergyRetrievalBS : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return OtherCards(ctx).Count >= 1 && (ctx.BasicEnergyInDiscard().Count > 0 || OtherCards(ctx).Any(c => c.Def.IsBasicEnergy));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!DiscardOthers(ctx, 1, "Energy Retrieval: discard 1 card from your hand")) return;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(energy, 1, Math.Min(2, energy.Count), "Energy Retrieval: put up to 2 basic Energy cards into your hand"));
            }
        }

        /// <summary>Full Heal: your Active Pokémon is no longer Asleep, Confused, Paralyzed or Poisoned.</summary>
        private sealed class FullHealBS : CardScript
        {
            private static readonly SpecialCondition[] Healed = { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && Healed.Any(c => ctx.Me.Active.Has(c)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                foreach (var c in Healed) if (active.Has(c)) ctx.Engine.RemoveCondition(active, c);
            }
        }

        /// <summary>Pokémon Flute: put a Basic Pokémon card from your opponent's discard pile onto their Bench.</summary>
        private sealed class PokemonFlute : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx)
            {
                int opp = 1 - ctx.Player;
                return ctx.Opp.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(opp, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Engine.BenchSpace(1 - ctx.Player) > 0 && Basics(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0 || ctx.Engine.BenchSpace(1 - ctx.Player) <= 0) return;
                var pick = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Pokémon Flute: put which Basic Pokémon onto your opponent's Bench?")[0];
                if (ctx.Opp.Discard.Remove(pick)) ctx.Engine.PutOntoBench(1 - ctx.Player, pick, false);
            }
        }

        /// <summary>Professor Oak: discard your hand, then draw 7 cards.</summary>
        private sealed class ProfessorOak : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var card in OtherCards(ctx)) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(7);
            }
        }

        /// <summary>Revive: put a Basic Pokémon card from your discard pile onto your Bench with damage counters equal to half its HP (rounded down to 10).</summary>
        private sealed class ReviveBS : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && Basics(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0 || ctx.BenchSpace <= 0) return;
                var pick = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Revive: put which Basic Pokémon from your discard pile onto your Bench?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, pick, false);
                int counters = pick.Def.Hp / 2 / 10;
                if (mon != null && counters > 0) ctx.Engine.PlaceDamageCounters(mon, counters, "Revive");
            }
        }

        /// <summary>Super Potion: discard an Energy card attached to 1 of your Pokémon to remove up to 4 damage counters from it.</summary>
        private sealed class SuperPotionBS : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && m.Energy.Count > 0 && !ctx.Engine.HealingBlocked(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Super Potion: discard an Energy card from which of your Pokémon (and heal 40 damage from it)?");
                if (target == null) return;
                var pay = target.Energy.Count == 1 ? target.Energy.ToList() : ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Super Potion: discard which Energy card?");
                foreach (var card in pay) ctx.Engine.DiscardEnergy(target, card);
                ctx.Heal(target, 40);
            }
        }

        /// <summary>Potion: remove up to 2 damage counters from 1 of your Pokémon.</summary>
        private sealed class PotionBS : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = Hurt(ctx);
                if (hurt.Count == 0) return;
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Potion: heal 20 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }
        }

        /// <summary>Bill: draw 2 cards.</summary>
        private sealed class Bill : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(2); }
        }

        /// <summary>Gust of Wind: switch 1 of your opponent's Benched Pokémon with their Active Pokémon.</summary>
        private sealed class GustOfWind : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.GustOpponent("Gust of Wind: choose 1 of your opponent's Benched Pokémon to switch in"); }
        }
    }
}
