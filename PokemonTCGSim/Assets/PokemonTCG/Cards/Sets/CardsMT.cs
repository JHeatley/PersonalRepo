using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Mysterious Treasures (MT, 2007; v172): the cards not already in the pool (the others are prints). Unlimited only. Rares and
    /// LV.X here; Uncommons, Commons and Trainers in CardsMTB.cs. DP wording: "Trainer" means Trainer cards that aren't Supporters
    /// or Stadiums. The fossils give a Prize card when Knocked Out.
    /// </summary>
    public static partial class CardsMT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MT-1", new ComboMon(new HeapUpMon(), new HardMetalMon()));
            r.RegisterByKey("MT-2", new AlakazamPowerCancel());
            r.RegisterByKey("MT-3", new ComboMon(new TailInfluenceMon(), new CharityTailMon()));
            r.RegisterByKey("MT-4", new AzelfDownerMaterial());
            r.RegisterByKey("MT-5", new BlisseyKindEgg());
            r.RegisterByKey("MT-6", new BronzongMiracleOracle());
            r.RegisterByKey("MT-7", new ComboMon(new SearchAttachSelfMon("Sprouting", d => CardsRS.BasicOf(d, EnergyType.Grass)), new LeafTornadoMon()));
            r.RegisterByKey("MT-8", new ComboMon(new EnergyCycloneMon(), new CardsHPB.KnockOffMon("Breaking Tail", false)));
            r.RegisterByKey("MT-9", new GarchompRainbowScale());
            r.RegisterByKey("MT-10", new HonchkrowDarkGenes());
            r.RegisterByKey("MT-11", new LumineonLureRing());
            r.RegisterByKey("MT-13", new ComboMon(new UltraPowderMon(),
                new CardsFST2.ConditionBonusMon("Fire Counterattack", 30, c => c.OpponentPokemon().Any(m => c.Engine.IsType(m, EnergyType.Fire)))));
            r.RegisterByKey("MT-14", new MespritUpperMaterial());
            r.RegisterByKey("MT-16", new TyphlosionFirestarter());
            r.RegisterByKey("MT-17", new ComboMon(new PaybackMon(), new GroundBurnMon()));
            r.RegisterByKey("MT-18", new UxieMemoryOut());
            r.RegisterByKey("MT-121", new ElectivireLvXShockingTail());
            r.RegisterByKey("MT-123", new MagmortarLvXTorridWave());
            RegisterUncommonsAndTrainers(r);
        }

        internal static bool TrioInPlay(EffectContext ctx, string a, string b)
        {
            return ctx.Me.InPlay().Any(m => m.Name == a) && ctx.Me.InPlay().Any(m => m.Name == b);
        }

        /// <summary>"Search your deck for a [card] and attach it to this Pokémon" (Sprouting).</summary>
        internal sealed class SearchAttachSelfMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;

            public SearchAttachSelfMon(string attack, Func<CardDef, bool> filter)
            {
                this.attack = attack;
                this.filter = filter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                foreach (var card in ctx.SearchMyDeck(filter, 1, attack + ": choose an Energy card to attach to " + me.Name))
                    if (!ctx.InPlay(me) || !ctx.Engine.AttachEnergyCard(card, me))
                        if (!ctx.Me.Deck.Contains(card) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Heap Up: show the Energy cards in your discard pile; with any Metal Special Energy there, 30 more; they all go into your deck, shuffled.</summary>
        private sealed class HeapUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heap Up") return false;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                bool metalSpecial = energy.Any(c => c.Def.IsSpecialEnergy && c.Def.Name.Contains("Metal"));
                ctx.DamageDefending(atk.Damage + (metalSpecial ? 30 : 0));
                if (energy.Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player))
                {
                    ctx.DiscardToDeck(energy);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Hard Metal: you may do 40 more damage; if you do, 40 damage to Aggron.</summary>
        private sealed class HardMetalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hard Metal") return false;
                bool more = ctx.AskYesNo("Hard Metal: do 40 more damage (and 40 damage to " + ctx.Pokemon.Name + ")?");
                ctx.DamageDefending(atk.Damage + (more ? 40 : 0));
                if (more) ctx.DamageSelf(40);
                return true;
            }
        }

        /// <summary>
        /// Power Cancel (Poké-Power): once during your opponent's turn, when their Pokémon uses a Poké-Power, you may discard 2 cards
        /// from your hand to prevent all its effects (it still counts as used). Not with a Special Condition. Psychic Guard: during
        /// the opponent's next turn, attack damage from their Stage 2 Pokémon is 30 less (after W/R; errata: evolved or not).
        /// </summary>
        private sealed class AlakazamPowerCancel : Mon
        {
            private static string Limit(EffectContext src) { return "Power Cancel#" + src.Pokemon.Uid; }

            public override bool CancelsPokePower(EffectContext src, PokemonInPlay user, string power)
            {
                var me = src.Pokemon;
                if (me == null || user == null || user.Owner == src.Player || src.Me.Hand.Count < 2 || !CardsE1.NoCondition(me)) return false;
                int turn;
                if (src.Me.NamedLimitUsedOnTurn.TryGetValue(Limit(src), out turn) && turn == src.Turn) return false;
                if (!src.AskYesNo("Power Cancel: discard 2 cards from your hand to stop " + user.Name + "'s " + power + "?")) return false;
                src.Me.NamedLimitUsedOnTurn[Limit(src)] = src.Turn;
                foreach (var card in src.ChooseCards(src.Me.Hand.ToList(), 2, 2, "Power Cancel: discard 2 cards from your hand")) src.Engine.DiscardFromHand(src.Player, card);
                Announce(src, "Power Cancel");
                src.Log(user.Name + "'s " + power + " was cancelled (Power Cancel).");
                return true;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Guard") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    ctx.AddEffect(new GuardFromStage2Effect(me.Name + " takes 30 less damage from Stage 2 Pokémon during the opponent's next turn (Psychic Guard)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class GuardFromStage2Effect : ContinuousEffect
        {
            public GuardFromStage2Effect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target == null || info.Target.Uid != TargetUid || a == null || a.Owner == Owner || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.Def.Stage == Stage.Stage2 ? damage - 30 : damage;
            }
        }

        /// <summary>Tail Influence: your opponent flips until heads; for each tails, an Energy card from the Defending Pokémon goes on the bottom of their deck.</summary>
        private sealed class TailInfluenceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail Influence") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                int tails = 0;
                while (tails < 30 && !ctx.Engine.FlipCoin(ctx.Opponent, "Tail Influence")) tails++;
                for (int i = 0; i < tails; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Tail Influence: put which Energy card on the bottom of your opponent's deck?")[0];
                    defending.Energy.Remove(card);
                    ctx.Opp.Deck.Add(card);
                }
                return true;
            }
        }

        /// <summary>Charity Tail: before the damage, your opponent may discard 2 cards from their hand to make the base damage 10.</summary>
        private sealed class CharityTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charity Tail") return false;
                bool paid = false;
                if (ctx.Opp.Hand.Count >= 2 && ctx.Engine.AskYesNo(ctx.Opponent, "Charity Tail: discard 2 cards from your hand to make the attack do 10 damage instead of " + atk.Damage + "?", ctx.Source))
                {
                    foreach (var card in ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 2, 2, "Charity Tail: discard 2 cards", ctx.Card.Def)) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                    paid = true;
                }
                ctx.DamageDefending(paid ? 10 : atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Downer Material (Poké-Body): with Uxie and Mesprit in play, the opponent's Basic Pokémon's attacks cost 1 Colorless more
        /// (only 1 Downer Material counts). Bind Pulse: the opponent can't attach Special Energy cards from their hand next turn.
        /// </summary>
        private sealed class AzelfDownerMaterial : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || !mon.IsBasic || !TrioInPlay(src, "Uxie", "Mesprit")) return;
                var first = src.MyPokemon(m => src.Engine.HasAbilityNamed(m, "Downer Material")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first == me) cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bind Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new NoSpecialEnergyEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't attach Special Energy cards from their hand during their next turn (Bind Pulse)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoSpecialEnergyEffect : ContinuousEffect
        {
            private readonly int affected;

            public NoSpecialEnergyEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksEnergyCard(GameEngine engine, int player, CardInstance card) { return player == affected && card != null && card.Def.IsSpecialEnergy; }
        }

        /// <summary>
        /// Kind Egg (Poké-Power): once during your turn, if Happiny is under Blissey, put up to as many cards from your hand as Blissey
        /// has Energy into your deck, shuffle, and draw that many. Not with a Special Condition. Happy Chance: you may first attach a basic Energy
        /// card from your discard pile to Blissey; 10 more for each Energy on it.
        /// </summary>
        private sealed class BlisseyKindEgg : Mon
        {
            protected override string Activated { get { return "Kind Egg"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && EvolvedFrom(ctx, "Happiny") && ctx.EnergyCount(ctx.Pokemon) > 0 && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                int max = Math.Min(ctx.EnergyCount(ctx.Pokemon), ctx.Me.Hand.Count);
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, max, "Kind Egg: put up to " + max + " cards from your hand into your deck");
                foreach (var card in picks)
                {
                    ctx.Me.Hand.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                ctx.Draw(picks.Count);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Happy Chance") return false;
                var me = ctx.Pokemon;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count > 0 && ctx.AskYesNo("Happy Chance: attach a basic Energy card from your discard pile to " + me.Name + " first?"))
                {
                    var card = ctx.ChooseCards(pile, 1, 1, "Happy Chance: attach which basic Energy card?")[0];
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Discard.Add(card);
                }
                ctx.DamageDefending(atk.Damage + 10 * ctx.EnergyCount(me));
                return true;
            }
        }

        /// <summary>Miracle Oracle (Poké-Power): once during your turn, draw a card, then discard a card from your hand; if it's an Energy card, draw 1 more. Not with a Special Condition. Shady Stamp.</summary>
        private sealed class BronzongMiracleOracle : Mon
        {
            private static readonly Mon Stamp = new ConditionAttackMon("Shady Stamp", SpecialCondition.Confused);

            protected override string Activated { get { return "Miracle Oracle"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Draw(1);
                if (ctx.Me.Hand.Count == 0) return;
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Miracle Oracle: discard a card from your hand (an Energy card draws 1 more)")[0];
                bool energy = card.Def.IsEnergy;
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (energy) ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Stamp.HandleAttack(ctx, atk); }
        }

        /// <summary>Leaf Tornado: you may move any basic Grass Energy cards between your Pokémon in any way you like.</summary>
        private sealed class LeafTornadoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Tornado") return false;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 20; i++)
                {
                    var donors = ctx.MyPokemon(m => m.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Grass)));
                    if (donors.Count == 0 || ctx.Me.InPlay().Count() < 2) break;
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, donors, "Leaf Tornado: move a basic Grass Energy card from which Pokémon? (Done to stop)", true);
                    if (from == null) break;
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Leaf Tornado: move it to which Pokémon?");
                    if (to == null) break;
                    ctx.Engine.MoveEnergy(from, to, from.Energy.First(e => CardsRS.BasicOf(e.Def, EnergyType.Grass)));
                    if (!RepeatableAbility.ForPerson(ctx)) break;
                }
                return true;
            }
        }

        /// <summary>Energy Cyclone: show any Energy cards from your hand; 20 for each; they go into your deck, shuffled.</summary>
        private sealed class EnergyCycloneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Cyclone") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(energy, 0, energy.Count, "Energy Cyclone: choose Energy cards from your hand (20 damage each)");
                ctx.DamageDefending(atk.Damage * picks.Count);
                foreach (var card in picks)
                {
                    ctx.Me.Hand.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                if (picks.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Rainbow Scale (Poké-Body): if an Active Pokémon is weak to any type of Energy attached to Garchomp, its attacks do 40 more
        /// to it (before W/R). Not with a Special Energy card attached.
        /// </summary>
        private sealed class GarchompRainbowScale : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var t = info.Target;
                if (me == null || info.Attacker != me || t == null || !info.TargetIsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (me.Energy.Any(e => e.Def.IsSpecialEnergy)) return damage;
                var weak = src.Engine.WeaknessOf(t);
                if (!weak.HasValue) return damage;
                return src.Engine.EnergyCountOfType(me, weak.Value) > 0 ? damage + 40 : damage;
            }
        }

        /// <summary>
        /// Dark Genes (Poké-Body): while Honchkrow has the Energy for its attack, each of your Murkrow can use it as its own without
        /// any Energy. Dark Wing Flaps: Astonish.
        /// </summary>
        private sealed class HonchkrowDarkGenes : Mon
        {
            private static readonly Mon Flaps = new AstonishMon("Dark Wing Flaps");

            private static bool Ready(EffectContext src) { return src.Pokemon != null && src.Pokemon.Def.Attacks.Count > 0 && src.Engine.CanPayAttack(src.Pokemon, src.Pokemon.Def.Attacks[0]); }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon == src.Pokemon || mon.Owner != src.Player || mon.Name != "Murkrow" || !Ready(src)) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Pokemon.Def, 0) };
            }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon == src.Pokemon || mon.Name != "Murkrow" || mon.Owner != src.Player) return;
                if (src.Pokemon.Def.Attacks.Contains(attack)) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flaps.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Lure Ring (Poké-Power): once during your turn, if Lumineon is Active, switch in 1 of the opponent's Benched Pokémon with 100
        /// or more maximum HP. Not with a Special Condition. Reverse Stream: 10 more for each Water Energy; then that Energy returns to
        /// your hand.
        /// </summary>
        private sealed class LumineonLureRing : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.GustTargets().Where(m => ctx.Engine.MaxHp(m) >= 100).ToList(); }

            protected override string Activated { get { return "Lure Ring"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsDS.PowerReady(ctx) && Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Lure Ring: switch in which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reverse Stream") return false;
                var me = ctx.Pokemon;
                var water = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)).ToList();
                ctx.DamageDefending(atk.Damage + 10 * ctx.Engine.EnergyCountOfType(me, EnergyType.Water));
                if (ctx.InPlay(me)) foreach (var card in water) ctx.Engine.ReturnEnergyToHand(me, card);
                return true;
            }
        }

        /// <summary>Ultra Powder: 3 coins: 1st heads Poisoned, 2nd heads Burned, 3rd heads Paralyzed.</summary>
        private sealed class UltraPowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultra Powder") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var conditions = new[] { SpecialCondition.Poisoned, SpecialCondition.Burned, SpecialCondition.Paralyzed };
                foreach (var c in conditions)
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, c);
                return true;
            }
        }

        /// <summary>
        /// Upper Material (Poké-Body): with Uxie and Azelf in play, every Uxie, Mesprit and Azelf (both players') has no Retreat Cost.
        /// Teleportation Burst: you may switch Mesprit with 1 of your Benched Pokémon.
        /// </summary>
        private sealed class MespritUpperMaterial : Mon
        {
            private static readonly Mon Burst = new SwitchSelfMon("Teleportation Burst", true);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || !TrioInPlay(src, "Uxie", "Azelf")) return cost;
                return mon.Name == "Uxie" || mon.Name == "Mesprit" || mon.Name == "Azelf" ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burst.HandleAttack(ctx, atk); }
        }

        /// <summary>Firestarter (Poké-Power): once during your turn, attach a Fire Energy card from your discard pile to 1 of your Benched Pokémon. Not with a Special Condition. Evaporating Heat.</summary>
        private sealed class TyphlosionFirestarter : Mon
        {
            protected override string Activated { get { return "Firestarter"; } }

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => CardsRS.BasicOf(d, EnergyType.Fire)); }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && Pile(ctx).Count > 0 && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pile = Pile(ctx);
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Firestarter: attach a Fire Energy card to which of your Benched Pokémon?");
                if (target != null && pile.Count > 0 && ctx.Me.Discard.Remove(pile[0])) ctx.Engine.AttachEnergyCard(pile[0], target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evaporating Heat") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => CardsDRI1.ProvidesType(ctx, defending, e, EnergyType.Water)))
                    ctx.DiscardEnergyFrom(defending, 1, "Evaporating Heat: discard a Water Energy from " + defending.Name, e => CardsDRI1.ProvidesType(ctx, defending, e, EnergyType.Water));
                return true;
            }
        }

        /// <summary>Payback: if your opponent has only 1 Prize card left, 40 more and the top 3 cards of their deck are discarded.</summary>
        private sealed class PaybackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Payback") return false;
                bool last = ctx.Opp.Prizes.Count == 1;
                ctx.DamageDefending(atk.Damage + (last ? 40 : 0));
                if (last && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                return true;
            }
        }

        /// <summary>Ground Burn: each player discards the top card of their deck; 20 more for each Energy card discarded.</summary>
        private sealed class GroundBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ground Burn") return false;
                int energy = 0;
                foreach (var p in new[] { ctx.Player, ctx.Opponent })
                    energy += ctx.Engine.DiscardTopOfDeck(p, 1).Count(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + 20 * energy);
                return true;
            }
        }

        /// <summary>
        /// Memory Out (Poké-Power): once during your opponent's turn, when Uxie is damaged by their attack (even if Knocked Out), the
        /// Attacking Pokémon can't use that attack during their next turn. Mind Off: with Mesprit and Azelf in play, 20 more and Asleep.
        /// </summary>
        private sealed class UxieMemoryOut : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var atk = ctx.Engine.ResolvingAttack;
                if (attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || atk == null || damage <= 0) return;
                if (!ctx.AskYesNo("Memory Out: " + attacker.Name + " can't use " + atk.Name + " during your opponent's next turn?")) return;
                Announce(ctx, "Memory Out");
                ctx.AddEffect(new CantUseAttackEffect(attacker.Name + " can't use " + atk.Name + " during its next turn (Memory Out)", ctx.Player, attacker.Uid, ctx.Turn + 2, atk.Name));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Off") return false;
                var defending = ctx.Defending;
                bool trio = TrioInPlay(ctx, "Mesprit", "Azelf");
                ctx.DamageDefending(atk.Damage + (trio ? 20 : 0));
                if (trio && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Shocking Tail (Poké-Body): while Electivire is Active, whenever the opponent attaches an Energy card from their hand to 1 of
        /// their Pokémon, 2 damage counters on that Pokémon. Pulse Barrier: discard the opponent's Pokémon Tools and their Stadium in
        /// play; if you do, no effects of attacks (damage included) on Electivire during their next turn.
        /// </summary>
        private sealed class ElectivireLvXShockingTail : Mon
        {
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || target == null || target.Owner == src.Player || !src.InPlay(target)) return;
                Announce(src, "Shocking Tail");
                src.Engine.PlaceDamageCounters(target, 2, "Shocking Tail");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pulse Barrier") return false;
                ctx.DamageDefending(atk.Damage);
                int n = 0;
                foreach (var mon in ctx.OpponentPokemon()) n += ctx.Engine.DiscardAllTools(mon);
                if (ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent)
                {
                    ctx.Engine.DiscardStadium();
                    n++;
                }
                if (n > 0 && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Torrid Wave (Poké-Power): once during your turn, if Magmortar is Active, the Defending Pokémon is now Burned with 3 damage
        /// counters between turns instead of 2 (while Magmortar LV.X is in play). Not with a Special Condition. Flame Bluster:
        /// discard 2 Fire Energy, 100 to 1 of the opponent's Pokémon; not 2 turns in a row.
        /// </summary>
        private sealed class MagmortarLvXTorridWave : Mon
        {
            private const string Marker = "Torrid Wave";

            protected override string Activated { get { return Marker; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsDS.PowerReady(ctx) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (defending != null && ctx.ApplyCondition(defending, SpecialCondition.Burned)) defending.Markers[Marker] = 1;
            }

            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return mon != null && mon.Owner != src.Player && mon.Markers.ContainsKey(Marker) ? Math.Max(counters, 3) : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Bluster") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Flame Bluster: discard 2 Fire Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                ctx.DamageAnyOpponent(100);
                if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }
    }
}
