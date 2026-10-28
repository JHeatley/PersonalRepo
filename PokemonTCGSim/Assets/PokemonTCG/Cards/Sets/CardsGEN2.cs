using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Generations (GEN, v95), part 2: the Trainers (59-73) and the Radiant Collection (RC1-RC32). Trainers and Pokémon already in
    /// the pool (Crushing Hammer, Shauna, Yveltal, M Gardevoir-EX...) keep their scripts through the name links in the card data.
    /// </summary>
    public static class CardsGEN2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("GEN-RC1", new SwitchSelfMon("Run Around", false));
            r.RegisterByKey("GEN-RC2", new CardsCES1.FlipConditionsMon("Worry Seed", SpecialCondition.Confused));
            r.RegisterByKey("GEN-RC3", new PlayfulMon());
            r.RegisterByKey("GEN-RC4", new SearchAttackMon("Call for Support", d => d.IsSupporter, 1));
            r.RegisterByKey("GEN-RC5", new ComboMon(new RecallMon(), new LockAfterAttackMon("Combustion Blast", true)));
            r.RegisterByKey("GEN-RC6", new FlareonEX());
            r.RegisterByKey("GEN-RC8", new FroslassGEN());
            r.RegisterByKey("GEN-RC10", new ComboMon(new RestAmountMon(30), new FlipBonusMon("Pachi", 20)));
            r.RegisterByKey("GEN-RC11", new WobbuffetGEN());
            r.RegisterByKey("GEN-RC12", new TailsNothingMon("Starvin'"));
            r.RegisterByKey("GEN-RC13", new WishMon());
            r.RegisterByKey("GEN-RC14", new ConditionAttackMon("Twinkle", SpecialCondition.Asleep));
            r.RegisterByKey("GEN-RC15", new ComboMon(new CardsUPR3.MagicalSwapMon("Ear Influence"), new CardsMEG.DefendingEnergyBonusMon("Psychic", 10)));
            r.RegisterByKey("GEN-RC18", new CardsCEC3.SpreadAmountMon("Petal Blizzard", 10));
            r.RegisterByKey("GEN-RC19", new LickAwayMon());
            r.RegisterByKey("GEN-RC20", new DrawAttackMon("Lap Up", 3));
            r.RegisterByKey("GEN-RC21", new ComboMon(new CardsASC.ToolBonusMon("Dress Up", 30), new PreciousRibbonMon()));
            r.RegisterByKey("GEN-RC22", new ComboMon(new CardsUPR3.SmokescreenMon("Sparkle", 1), new DiamondStormMon()));
            r.RegisterByKey("GEN-RC23", new BindWoundMon("Bind Wound", 20));
            r.RegisterByKey("GEN-RC24", new CardsFST2.ConditionBonusMon("Fairy Friend", 30, c => c.MyBench().Any(m => c.Engine.IsType(m, EnergyType.Fairy))));
            r.RegisterByKey("GEN-RC25", new SearchAttackMon("Warble", d => d.Name == "Fletchling", 2));
            r.RegisterByKey("GEN-RC30", new ComboMon(new CardsDAA1.DrainMon("Life Leap"), new CardsFCO3.ShiningWindMon()));

            r.RegisterByName("Clemont", new Clemont());
            r.RegisterByName("Evosoda", new Evosoda());
            r.RegisterByName("Imakuni?", new Imakuni());
            r.RegisterByName("Max Revive", new MaxRevive());
            r.RegisterByName("Olympia", new Olympia());
            r.RegisterByName("Revitalizer", new Revitalizer());
            r.RegisterByName("Red Card", new RedCard());
            r.RegisterByName("Team Flare Grunt", new TeamFlareGrunt());
            r.RegisterByName("Floral Crown", new FloralCrown());
            r.RegisterByName("Wally", new Wally());
        }

        // ================================================================== Shared helpers

        /// <summary>Rest: heal N damage from this Pokémon; it's now Asleep (Dedenne: 30).</summary>
        internal sealed class RestAmountMon : Mon
        {
            private readonly int amount;

            public RestAmountMon(int amount) { this.amount = amount; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, amount);
                if (ctx.PokemonIsActive) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// "A card that evolves from 1 of your Pokémon" for Evosoda and Wally: any Evolution card with that name, BREAK Evolution
        /// included (your ruling, v95); not LV.X or LEGEND cards, nor ones only an effect can put into play.
        /// </summary>
        internal static bool EvolvesFromDeckOnto(EffectContext ctx, PokemonInPlay mon, CardDef def)
        {
            if (mon == null || def == null || !def.IsPokemon || def.IsLevelUp || def.IsLegend) return false;
            if (ctx.Engine.Scripts.For(def).PutIntoPlayOnlyByEffect) return false;
            return mon.Name == def.EvolvesFrom || mon.Def.BabyEvolvesInto(def);
        }

        /// <summary>Searches the deck for an Evolution card for one of the targets and evolves it; shuffles the deck afterward.</summary>
        internal static void EvolveOneFromDeck(EffectContext ctx, List<PokemonInPlay> targets, Func<CardDef, bool> extra, string what)
        {
            if (targets.Count > 0 && ctx.Me.Deck.Count > 0)
            {
                var found = ctx.SearchMyDeck(d => (extra == null || extra(d)) && targets.Any(m => EvolvesFromDeckOnto(ctx, m, d)), 1,
                    what + ": choose a card that evolves from 1 of your Pokémon", false);
                if (found.Count > 0)
                {
                    var card = found[0];
                    var onto = targets.Where(m => EvolvesFromDeckOnto(ctx, m, card.Def)).ToList();
                    var target = ctx.ChoosePokemon(onto, what + ": evolve which Pokémon into " + card.Def.Name + "?");
                    if (target != null) ctx.Engine.EvolvePokemon(target, card);
                    else ctx.Me.Deck.Add(card);
                }
            }
            ctx.ShuffleMyDeck();
        }

        // ================================================================== Fire

        /// <summary>Playful: flip a coin; heads, 20 damage times the number of damage counters on this Pokémon.</summary>
        private sealed class PlayfulMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Playful") return false;
                int counters = ctx.Flip() ? ctx.Pokemon.DamageCounters : 0;
                ctx.DamageDefending(atk.Damage * counters);
                return true;
            }
        }

        /// <summary>Recall: choose 1 of the attacks of this Pokémon's previous Evolutions and use it as this attack.</summary>
        private sealed class RecallMon : Mon
        {
            private static bool ChoosePrevious(EffectContext ctx, out CardDef source, out int index)
            {
                source = null;
                index = -1;
                var me = ctx.Pokemon;
                var opts = new List<DecisionOption>();
                for (int k = 0; k < me.Stack.Count - 1; k++)
                {
                    var d = me.Stack[k].Def;
                    if (!d.IsPokemon) continue;
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                        opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                    }
                }
                if (opts.Count == 0) return false;
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Recall: choose an attack of " + me.Name + "'s previous Evolutions", opts, ctx.Source);
                source = opts[pick].Card;
                index = opts[pick].Index;
                return true;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recall") return false;
                CardDef source;
                int index;
                if (ChoosePrevious(ctx, out source, out index)) UseCopiedAttack(ctx, source, index);
                else ctx.Log("Recall did nothing: " + ctx.Pokemon.Name + " has no previous Evolutions with attacks.");
                return true;
            }
        }

        /// <summary>Flash Fire: once during your turn, move a Fire Energy from 1 of your other Pokémon to this Pokémon. Blaze Ball: 20 more for each Fire Energy on it.</summary>
        private sealed class FlareonEX : Mon
        {
            private static readonly Mon Blaze = new CardsMEG.EnergyBonusMon("Blaze Ball", EnergyType.Fire, 20);

            protected override string Activated { get { return "Flash Fire"; } }

            private static Dictionary<CardInstance, PokemonInPlay> FireElsewhere(EffectContext ctx)
            {
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != ctx.Pokemon))
                    foreach (var en in mon.Energy)
                        if (CardsDRI1.ProvidesType(ctx, mon, en, EnergyType.Fire)) holders[en] = mon;
                return holders;
            }

            protected override bool AbilityReady(EffectContext ctx) { return FireElsewhere(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var holders = FireElsewhere(ctx);
                if (holders.Count == 0) return;
                var pick = ctx.ChooseCards(holders.Keys.ToList(), 1, 1, "Flash Fire: move which Fire Energy to " + ctx.Pokemon.Name + "?")[0];
                ctx.Engine.MoveEnergy(holders[pick], ctx.Pokemon, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blaze.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water / Psychic

        /// <summary>
        /// Drag Along: if this Pokémon is your Active Pokémon and is Knocked Out by damage from an opponent's attack, flip a coin;
        /// heads, the Attacking Pokémon is Knocked Out. Snowy Drop: 4 damage counters in any way you like.
        /// </summary>
        private sealed class FroslassGEN : Mon
        {
            private static readonly Mon Snowy = new CardsLOR1.CountersAnyWayMon("Snowy Drop", 4);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Pokemon == null) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                Announce(src, "Drag Along");
                if (src.Flip()) src.Engine.KnockOut(attacker, "Drag Along");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Snowy.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Bide Barricade: while this Pokémon is your Active Pokémon, each Pokémon in play, in each player's hand and in each player's
        /// discard pile has no Abilities, except Psychic Pokémon (its printed type, so the check can't loop through type-changing
        /// Abilities). Pokémon Powers, Poké-Powers and Poké-Bodies aren't Abilities. Psychic Assault: 10 more per damage counter.
        /// </summary>
        private sealed class WobbuffetGEN : Mon
        {
            private static readonly Mon Assault = new CardsMEW1.CounterBonusMon("Psychic Assault", 10);

            private static bool On(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon); }

            private static bool Blocked(CardDef card)
            {
                return card != null && card.IsPokemon && !card.HasType(EnergyType.Psychic) && card.Abilities.Any(a => a.IsAbility);
            }

            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return On(src) && mon != null && !mon.Def.HasType(EnergyType.Psychic) && CardsBKP2.GarbodorBKP.OwnAbility(mon, abilityName);
            }

            public override bool BlocksOpponentsAbilitiesOutOfPlay(EffectContext src, CardDef card) { return On(src) && Blocked(card); }

            public override bool BlocksOwnAbilitiesOutOfPlay(EffectContext src, CardDef card) { return On(src) && Blocked(card); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Assault.HandleAttack(ctx, atk); }
        }

        /// <summary>Wish: search your deck for any card and put it into your hand.</summary>
        private sealed class WishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Wish: choose a card to put into your hand", false);
                return true;
            }
        }

        // ================================================================== Fairy / Colorless

        /// <summary>Lick Away: remove all Special Conditions from this Pokémon.</summary>
        private sealed class LickAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lick Away") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.RemoveAllConditions(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Precious Ribbon: move a Fairy Energy from this Pokémon to 1 of your Benched Pokémon; if you do, heal 50 damage from that Pokémon.</summary>
        private sealed class PreciousRibbonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Precious Ribbon") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var fairy = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fairy)).ToList();
                if (fairy.Count == 0) return true;
                var energy = ctx.ChooseCards(fairy, 1, 1, "Precious Ribbon: move which Fairy Energy?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Precious Ribbon: move " + energy.Def.Name + " to which Benched Pokémon?");
                if (target != null && ctx.Engine.MoveEnergy(me, target, energy) && target.DamageCounters > 0) ctx.Heal(target, 50);
                return true;
            }
        }

        /// <summary>Diamond Storm: heal 30 damage from each of your Fairy Pokémon.</summary>
        private sealed class DiamondStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diamond Storm") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Fairy)))
                    ctx.Heal(mon, 30);
                return true;
            }
        }

        /// <summary>Bind Wound (Swablu): heal 20 damage from 1 of your Pokémon. Used for Bind Wound, Nutrients (the attack name and the numbers are passed in).</summary>
        internal sealed class BindWoundMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BindWoundMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsMEG.HealOneOfMine(ctx, amount, attack);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Clemont: search your deck for up to 4 Lightning Energy cards and put them into your hand.</summary>
        private sealed class Clemont : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 4, "Clemont: choose up to 4 Lightning Energy cards");
            }
        }

        /// <summary>
        /// Evosoda: search your deck for a card that evolves from 1 of your Pokémon and evolve it. Not during your first turn and not
        /// on a Pokémon put into play (or evolved) this turn, Adaptive Evolution or not (Compendium, 2014-05-29 and 2014-02-06).
        /// It needs a Pokémon that could evolve at all (2018-07-26); BREAK Evolution cards count (your ruling, v95).
        /// </summary>
        private sealed class Evosoda : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                if (ctx.Engine.EvolveLocked(ctx.Player)) return new List<PokemonInPlay>();
                return ctx.MyPokemon(m => m.TurnEnteredPlay < ctx.Turn && m.TurnEvolved != ctx.Turn && m.CantEvolveOnTurn != ctx.Turn);
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return !ctx.State.IsPlayersFirstTurn && ctx.Me.Deck.Count > 0 && Targets(ctx).Any(m => EvolutionLines.CanEvolveAtAll(m.Name, true));
            }

            public override void ResolveTrainer(EffectContext ctx) { EvolveOneFromDeck(ctx, Targets(ctx), null, "Evosoda"); }
        }

        /// <summary>Imakuni?: your Active Pokémon is now Confused.</summary>
        private sealed class Imakuni : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Confused);
            }
        }

        /// <summary>Max Revive: put a Pokémon from your discard pile on top of your deck.</summary>
        private sealed class MaxRevive : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                var pick = ctx.ChooseCards(pile, 1, 1, "Max Revive: put which Pokémon on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return;
                ctx.Me.Deck.Insert(0, pick);
                ctx.Log("Max Revive: " + ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Def.Name + " from the discard pile on top of the deck.");
            }
        }

        /// <summary>Olympia: switch your Active Pokémon with 1 of your Benched Pokémon; if you do, heal 30 damage from the one moved to the Bench.</summary>
        private sealed class Olympia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var old = ctx.Me.Active;
                if (!ctx.SwitchMyActive("Olympia: choose a Benched Pokémon to switch in")) return;
                if (old != null && ctx.InPlay(old) && ctx.Me.Bench.Contains(old) && old.DamageCounters > 0) ctx.Heal(old, 30);
            }
        }

        /// <summary>Revitalizer: put 2 Grass Pokémon from your discard pile into your hand (1 if that's all there is).</summary>
        private sealed class Revitalizer : CardScript
        {
            private static List<CardInstance> Pile(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Grass)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pile(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Pile(ctx);
                if (pile.Count == 0) return;
                int n = Math.Min(2, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Revitalizer: put 2 Grass Pokémon from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Red Card: your opponent shuffles their hand into their deck and draws 4 cards.</summary>
        private sealed class RedCard : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
            }
        }

        /// <summary>Team Flare Grunt: discard an Energy attached to your opponent's Active Pokémon.</summary>
        private sealed class TeamFlareGrunt : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target == null || target.Energy.Count == 0) return;
                var energy = ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Team Flare Grunt: discard which Energy from " + target.Name + "?")[0];
                ctx.Engine.DiscardEnergy(target, energy);
            }
        }

        /// <summary>
        /// Floral Crown: at the end of your opponent's turn, heal 20 damage from the Basic Pokémon it's attached to. Checked after that
        /// turn's Knock Outs and Prizes, if still in play (Compendium, 2016-02-11).
        /// </summary>
        private sealed class FloralCrown : CardScript
        {
            public override void OnEndOfTurnAfterKnockOuts(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || src.IsMyTurn || !src.InPlay(holder) || !holder.Def.IsBasicPokemon || holder.DamageCounters == 0) return;
                if (!holder.HasTool(src.Card)) return;
                src.Log("Floral Crown: heal 20 damage from " + holder.Name + ".");
                src.Heal(holder, 20);
            }
        }

        /// <summary>
        /// Wally: search your deck for a card that evolves from 1 of your Pokémon (not a Pokémon-EX) and evolve it, even during your
        /// first turn or on a Pokémon put into play this turn, and on one that already evolved this turn (your ruling, v95). Effects
        /// that say a Pokémon can't evolve still stop it. BREAK Evolution cards count (your ruling, v95).
        /// </summary>
        private sealed class Wally : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                if (ctx.Engine.EvolveLocked(ctx.Player)) return new List<PokemonInPlay>();
                return ctx.MyPokemon(m => !m.Def.IsPokemonEX && m.CantEvolveOnTurn != ctx.Turn);
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && Targets(ctx).Any(m => EvolutionLines.CanEvolveAtAll(m.Name, true));
            }

            // "You can use this card during your first turn": the first player's first-turn Supporter rule doesn't stop it.
            public override bool PlayableOnFirstTurn(EffectContext ctx) { return true; }

            public override void ResolveTrainer(EffectContext ctx) { EvolveOneFromDeck(ctx, Targets(ctx), d => !d.IsPokemonEX, "Wally"); }
        }
    }
}
