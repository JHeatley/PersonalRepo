using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Aquapolis Trainers and Energy (E2 118-119, 121-141, 145-146; v146). Energy Switch, Darkness Energy, Metal Energy, Rainbow Energy
    /// and Warp Energy are prints of cards already in the pool. Pokémon Fan Club and Power Plant are Aquapolis's own cards (the
    /// newer cards Limitless files them with do something else). The Cube 01 Technical Machines are attached like Pokémon Tools but
    /// aren't Tools (like Expedition's Multi Technical Machine 01), and so is Weakness Guard. Your rulings: Apricorn Forest can't
    /// find Baby Pokémon; Healing Berry checks at the end of each turn, before Pokémon Checkup; Pokémon Park's heal is used by the
    /// first attachment to a damaged Benched Pokémon; Crystal Energy provides 1 Energy of any 1 of the basic types attached.
    /// </summary>
    public static class CardsE2T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E2-118", new ApricornForest());
            r.RegisterByKey("E2-119", new CubeTM(EnergyType.Darkness, Plunder));
            r.RegisterByKey("E2-121", new CubeTM(EnergyType.Fighting, ViolentRage));
            r.RegisterByKey("E2-122", new CubeTM(EnergyType.Fire, (ctx, atk) => ConditionAttack(ctx, atk, SpecialCondition.Burned)));
            r.RegisterByKey("E2-123", new ForestGuardian());
            r.RegisterByKey("E2-124", new CubeTM(EnergyType.Grass, SleepPoison));
            r.RegisterByKey("E2-125", new HealingBerry());
            r.RegisterByKey("E2-126", new Juggler());
            r.RegisterByKey("E2-127", new CubeTM(EnergyType.Lightning, Discharge));
            r.RegisterByKey("E2-128", new MemoryBerry());
            r.RegisterByKey("E2-129", new CubeTM(EnergyType.Metal, MetalReversal));
            r.RegisterByKey("E2-130", new PokemonFanClubE2());
            r.RegisterByKey("E2-131", new PokemonPark());
            r.RegisterByKey("E2-132", new CubeTM(EnergyType.Psychic, (ctx, atk) => ConditionAttack(ctx, atk, SpecialCondition.Confused)));
            r.RegisterByKey("E2-133", new Seer());
            r.RegisterByKey("E2-134", new SuperEnergyRemoval2());
            r.RegisterByKey("E2-135", new TimeShard());
            r.RegisterByKey("E2-136", new TownVolunteers());
            r.RegisterByKey("E2-137", new TravelingSalesman());
            r.RegisterByKey("E2-138", new UnderseaRuins());
            r.RegisterByKey("E2-139", new PowerPlantE2());
            r.RegisterByKey("E2-140", new CubeTM(EnergyType.Water, (ctx, atk) => ctx.DamageAnyOpponent(20, true, false)));
            r.RegisterByKey("E2-141", new WeaknessGuard());
            r.RegisterByKey("E2-145", new BoostEnergy());
            r.RegisterByKey("E2-146", new CrystalEnergy());
        }

        /// <summary>
        /// "Technical Machine and/or Pokémon Tool cards" (Traveling Salesman, Sneasel's Rob): the cards attached as Tools, except Weakness
        /// Guard, a plain Trainer that is only attached like one.
        /// </summary>
        internal static bool IsTechnicalMachineOrTool(CardDef d) { return d != null && d.IsTool && d.Name != "Weakness Guard"; }

        // ================================================================== Technical Machines

        /// <summary>
        /// Cube 01 (Technical Machine): attach it to 1 of your Pokémon of its type; that Pokémon may use its attack; discarded at the end
        /// of your turn. It isn't a Pokémon Tool, so it can share a Pokémon with one.
        /// </summary>
        private sealed class CubeTM : CardScript
        {
            private readonly EnergyType type;
            private readonly Action<EffectContext, AttackDef> resolve;

            public CubeTM(EnergyType type, Action<EffectContext, AttackDef> resolve)
            {
                this.type = type;
                this.resolve = resolve;
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, type); }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override bool DiscardedAtEndOfOwnersTurn { get { return true; } }

            public override bool IsAttachedNonTool { get { return true; } }

            public override void ResolveAttack(EffectContext ctx, int attackIndex) { resolve(ctx, ctx.Attack(attackIndex)); }
        }

        private static void ConditionAttack(EffectContext ctx, AttackDef atk, SpecialCondition condition)
        {
            var defending = ctx.Defending;
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
        }

        /// <summary>Plunder: before the damage, discard any Trainer cards attached to the Defending Pokémon.</summary>
        private static void Plunder(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            if (defending != null && ctx.CanAffect(defending))
                foreach (var tool in defending.Tools.ToList()) ctx.Engine.DiscardTool(defending, tool);
            ctx.DamageDefending(atk.Damage);
        }

        /// <summary>Violent Rage: a coin for each damage counter on this Pokémon, 10 per heads.</summary>
        private static void ViolentRage(EffectContext ctx, AttackDef atk)
        {
            int coins = ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters;
            ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
        }

        /// <summary>Sleep Poison: the Defending Pokémon is Asleep and Poisoned.</summary>
        private static void SleepPoison(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            if (!TefPokemon.StillDefending(ctx, defending)) return;
            ctx.ApplyCondition(defending, SpecialCondition.Asleep);
            ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
        }

        /// <summary>Discharge: discard all Lightning Energy cards attached to this Pokémon; a coin for each, 40 per heads.</summary>
        private static void Discharge(EffectContext ctx, AttackDef atk)
        {
            var me = ctx.Pokemon;
            int n = CardsBS1.CopyingWithMetronome ? 0 : CardsE2.DiscardCards(ctx, me, CardsE2.EnergyCardsOf(ctx, me, EnergyType.Lightning));
            ctx.DamageDefending(atk.Damage * (n > 0 ? ctx.FlipCoins(n) : 0));
        }

        /// <summary>Metal Reversal: before the damage, you may switch 1 of the opponent's Benched Pokémon with the Defending Pokémon.</summary>
        private static void MetalReversal(EffectContext ctx, AttackDef atk)
        {
            if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Metal Reversal: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                ctx.GustOpponent("Metal Reversal: choose 1 of your opponent's Benched Pokémon to switch in");
            ctx.DamageDefending(atk.Damage);
        }

        // ================================================================== Supporters

        /// <summary>Forest Guardian: shuffle your deck, look at the top 7 cards, put 1 into your hand and shuffle the rest back.</summary>
        private sealed class ForestGuardian : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.ShuffleMyDeck();
                var top = ctx.LookAtTop(7);
                if (top.Count == 0) return;
                ctx.DeckToHand(ctx.ChooseCards(top, 1, 1, "Forest Guardian: put which card into your hand?"));
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Juggler: discard up to 2 basic Energy cards from your hand; 1 discarded, draw 3; 2 discarded, draw 5.</summary>
        private sealed class Juggler : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(2, basics.Count), "Juggler: discard up to 2 basic Energy cards (1: draw 3, 2: draw 5)");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) { ctx.Me.Discard.Add(card); n++; }
                if (n == 0) return;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + n + " basic Energy card" + (n == 1 ? "" : "s") + " (Juggler).");
                ctx.Draw(n == 2 ? 5 : 3);
            }
        }

        /// <summary>Pokémon Fan Club (Aquapolis): put up to 2 Baby Pokémon and/or Basic Pokémon from your deck onto your Bench.</summary>
        private sealed class PokemonFanClubE2 : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToBench(d => true, 2, "Pokémon Fan Club: put up to 2 Baby Pokémon and/or Basic Pokémon onto your Bench");
            }
        }

        /// <summary>Seer: look at the top 6 cards of your deck, put every basic Energy card there into your hand, shuffle.</summary>
        private sealed class Seer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                ctx.Engine.ShowCards(ctx.Player, top, "Seer", "Seer: the top " + top.Count + " cards of your deck", ctx.Source);
                if (energy.Count > 0)
                {
                    ctx.Engine.ShowCards(ctx.Opponent, energy, "Seer", "Your opponent puts these cards into their hand.", ctx.Source);
                    ctx.DeckToHand(energy);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Town Volunteers: shuffle 5 Pokémon and/or basic Energy cards from your discard pile into your deck (all of them if fewer).</summary>
        private sealed class TownVolunteers : CardScript
        {
            private static List<CardInstance> Eligible(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Eligible(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Eligible(ctx);
                if (cards.Count == 0) return;
                int n = Math.Min(5, cards.Count);
                var picks = cards.Count == n ? cards : ctx.ChooseCards(cards, n, n, "Town Volunteers: shuffle 5 Pokémon and/or basic Energy cards into your deck");
                ctx.Engine.ShowCards(ctx.Opponent, picks, "Town Volunteers", "Your opponent shuffles these cards into their deck.", ctx.Source);
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Traveling Salesman: search your deck for up to 2 Technical Machine and/or Pokémon Tool cards.</summary>
        private sealed class TravelingSalesman : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(IsTechnicalMachineOrTool, 2, "Traveling Salesman: choose up to 2 Technical Machine and/or Pokémon Tool cards");
            }
        }

        // ================================================================== Items

        /// <summary>Super Energy Removal 2: 2 coins; 2 heads, discard all Energy from the Defending Pokémon; 2 tails, from your Active Pokémon.</summary>
        private sealed class SuperEnergyRemoval2 : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 2)
                {
                    var defending = ctx.Opp.Active;
                    if (defending != null && ctx.CanAffect(defending)) ctx.Engine.DiscardAllEnergy(defending);
                }
                else if (heads == 0 && ctx.Me.Active != null) ctx.Engine.DiscardAllEnergy(ctx.Me.Active);
                else ctx.Log("Super Energy Removal 2 did nothing.");
            }
        }

        /// <summary>Weakness Guard: attached to 1 of your Pokémon, which has no Weakness; discarded at the end of the opponent's next turn. Not a Pokémon Tool.</summary>
        private sealed class WeaknessGuard : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && mon == src.Pokemon; }

            public override bool DiscardedAtEndOfOpponentsTurn { get { return true; } }

            public override bool IsAttachedNonTool { get { return true; } }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Healing Berry: at the end of each turn (before Pokémon Checkup), with 20 HP or less left, remove 3 damage counters from its Pokémon and discard it.</summary>
        private sealed class HealingBerry : CardScript
        {
            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || !holder.Tools.Contains(src.Card)) return;
                int hp = src.RemainingHp(holder);
                if (hp <= 0 || hp > 20) return;
                src.Log("Healing Berry: " + holder.Name + " has " + hp + " HP left.");
                src.Heal(holder, 30);
                src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Memory Berry: its Pokémon can use the attacks of the cards it evolved from; discarded at the end of a turn in which it attacked.</summary>
        private sealed class MemoryBerry : CardScript
        {
            private const string Marker = "Memory Berry attacked";

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Stack.Count < 2) yield break;
                for (int s = 0; s < mon.Stack.Count - 1; s++)
                {
                    var d = mon.Stack[s].Def;
                    if (!d.IsPokemon) continue;
                    for (int i = 0; i < d.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(d, i);
                }
            }

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                if (src.Pokemon != null && attacker == src.Pokemon) src.Pokemon.AbilityUsedOnTurn[Marker] = src.Turn;
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                int turn;
                if (holder == null || !holder.AbilityUsedOnTurn.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                holder.AbilityUsedOnTurn.Remove(Marker);
                if (holder.Tools.Contains(src.Card)) src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>
        /// Time Shard: if its Pokémon is Knocked Out by damage from the opponent's Active Pokémon's attack during their turn, you may
        /// return up to 2 basic Energy cards attached to it to your hand.
        /// </summary>
        private sealed class TimeShard : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var holder = src.Pokemon;
                if (!byOpponentsAttack || holder == null || src.IsMyTurn) return;
                var attacker = src.Engine.AttackingPokemonOf(holder);
                if (attacker != null && !src.IsActive(attacker)) return;
                var basics = holder.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return;
                var picks = src.ChooseCards(basics, 0, Math.Min(2, basics.Count), "Time Shard: return up to 2 basic Energy cards from " + holder.Name + " to your hand");
                foreach (var card in picks) src.Engine.ReturnEnergyToHand(holder, card);
            }
        }

        // ================================================================== Stadiums

        /// <summary>
        /// Apricorn Forest: once during each player's turn, with room on their Bench, they flip; heads, they show a basic Energy card
        /// from their hand and put a Basic Pokémon of that type (not a Baby Pokémon: your ruling) from their deck onto their Bench.
        /// </summary>
        private sealed class ApricornForest : CardScript
        {
            private const string Limit = "Apricorn Forest";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.BenchSpace > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0)
                {
                    ctx.Log("Apricorn Forest: no basic Energy card in the hand to show.");
                    return false;
                }
                var shown = ctx.ChooseCards(energy, 1, 1, "Apricorn Forest: show which basic Energy card?")[0];
                var type = shown.Def.EnergyProvides;
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { shown }, "Apricorn Forest", "Your opponent shows this card.", ctx.Source);
                if (ctx.Me.Deck.Count == 0 || ctx.BenchSpace <= 0) return false;
                ctx.SearchToBench(d => !d.IsBaby && (d.PokemonType == type || (d.SecondType.HasValue && d.SecondType.Value == type)), 1,
                    "Apricorn Forest: put a " + type + " Basic Pokémon onto your Bench");
                return false;
            }
        }

        /// <summary>Pokémon Park: once during each player's turn, attaching an Energy card from their hand to a damaged Benched Pokémon of theirs removes 1 damage counter from it.</summary>
        private sealed class PokemonPark : CardScript
        {
            private const string Limit = "Pokémon Park";

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { React(src, target); }

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { React(src, target); }

            private static void React(EffectContext src, PokemonInPlay target)
            {
                if (target == null || target.DamageCounters == 0 || src.State.CurrentPlayer != target.Owner) return;
                var p = src.State.Players[target.Owner];
                if (!p.Bench.Contains(target)) return;
                int turn;
                if (p.NamedLimitUsedOnTurn.TryGetValue(Limit, out turn) && turn == src.Turn) return;
                p.NamedLimitUsedOnTurn[Limit] = src.Turn;
                src.Log("Pokémon Park: 1 damage counter is removed from " + target.Name + ".");
                src.Heal(target, 10);
            }
        }

        /// <summary>Undersea Ruins: once during each player's turn, they may flip; heads, they discard the top Evolution card from 1 of their Evolved Pokémon.</summary>
        private sealed class UnderseaRuins : CardScript
        {
            private const string Limit = "Undersea Ruins";

            private static List<PokemonInPlay> Evolved(EffectContext ctx) { return ctx.MyPokemon(m => CardsE2.IsEvolved(m)); }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && Evolved(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return false;
                var evolved = Evolved(ctx);
                if (evolved.Count == 0) return false;
                var target = ctx.ChoosePokemon(evolved, "Undersea Ruins: discard the top Evolution card from which of your Pokémon?");
                if (target != null) ctx.Engine.DevolveIntoDiscard(target, 1);
                return false;
            }
        }

        /// <summary>Power Plant (Aquapolis): once during each player's turn, they may discard a basic Energy card from their hand to put a basic Energy card from their discard pile into their hand.</summary>
        private sealed class PowerPlantE2 : CardScript
        {
            private const string Limit = "Power Plant";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy); }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var hand = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (hand.Count == 0) return false;
                var card = ctx.ChooseCards(hand, 1, 1, "Power Plant: discard which basic Energy card from your hand?")[0];
                if (!ctx.Me.Hand.Remove(card)) return false;
                ctx.Me.Discard.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + card.Def.Name + " (Power Plant).");
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return false;
                var pick = pile.Count == 1 ? pile : ctx.ChooseCards(pile, 1, 1, "Power Plant: put which basic Energy card from your discard pile into your hand?");
                ctx.Engine.ShowCards(ctx.Opponent, pick, "Power Plant", "Your opponent puts this card into their hand.", ctx.Source);
                ctx.DiscardToHand(pick);
                return false;
            }
        }

        // ================================================================== Special Energy

        /// <summary>
        /// Boost Energy: only on an Evolved Pokémon; provides [C][C][C]; its Pokémon can't retreat; discarded at the end of the turn it
        /// was attached, or as soon as its Pokémon isn't an Evolved Pokémon.
        /// </summary>
        private sealed class BoostEnergy : CardScript
        {
            private static string Mark(CardInstance card) { return "Boost Energy " + card.Id; }

            public override List<int> ProvidedEnergy(EffectContext ctx) { return new List<int> { EnergyMask.Colorless, EnergyMask.Colorless, EnergyMask.Colorless }; }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return CardsE2.IsEvolved(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !CardsE2.IsEvolved(ctx.Pokemon); }

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon) { return mon != null && mon == src.Pokemon; }

            public override void OnOwnerAttachedEnergyCard(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (energy == src.Card && target == src.Pokemon) target.AbilityUsedOnTurn[Mark(energy)] = src.Turn;
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                int turn;
                if (holder == null || !holder.Energy.Contains(src.Card) || !holder.AbilityUsedOnTurn.TryGetValue(Mark(src.Card), out turn) || turn != src.Turn) return;
                holder.AbilityUsedOnTurn.Remove(Mark(src.Card));
                src.Log("Boost Energy is discarded at the end of the turn it was attached.");
                src.Engine.DiscardEnergy(holder, src.Card);
            }
        }

        /// <summary>Crystal Energy (your ruling): 1 Energy that can be any 1 of the types of the basic Energy cards attached to its Pokémon; [C] with none.</summary>
        private sealed class CrystalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                int mask = 0;
                var holder = ctx.Pokemon;
                if (holder != null)
                    foreach (var e in holder.Energy)
                        if (e.Def.IsBasicEnergy) mask |= EnergyMask.Of(e.Def.EnergyProvides);
                return new List<int> { mask == 0 ? EnergyMask.Colorless : mask };
            }
        }
    }
}
