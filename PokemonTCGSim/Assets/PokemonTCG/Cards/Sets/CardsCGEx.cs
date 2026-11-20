using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Crystal Guardians Trainers (72-85), Pokémon-ex (89-98) and Pokémon ★ (99-100).</summary>
    public static partial class CardsCG
    {
        private static void RegisterExAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("CG-72", new Castaway());
            r.RegisterByKey("CG-74", new CessationCrystal());
            r.RegisterByKey("CG-75", new CrystalBeach());
            r.RegisterByKey("CG-79", new HolonCircle());
            r.RegisterByKey("CG-81", new MysteriousShard());
            r.RegisterByKey("CG-85", new Windstorm());

            r.RegisterByKey("CG-89", new AggronExIntimidatingArmor());
            r.RegisterByKey("CG-90", new ComboMon(new CardsE3B.DragOffMon(true), new BurnAwayMon()));
            r.RegisterByKey("CG-91", new DelcattyExConstrain());
            r.RegisterByKey("CG-92", new ExploudExExtraNoise());
            r.RegisterByKey("CG-93", new GroudonExHardRock());
            r.RegisterByKey("CG-94", new JirachiExStarLight());
            r.RegisterByKey("CG-95", new KyogreExFlotation());
            r.RegisterByKey("CG-96", new SceptileExExtraLiquid());
            r.RegisterByKey("CG-97", new ShiftryExDarkEyes());
            r.RegisterByKey("CG-98", new SwampertExEnergyRecycle());
            r.RegisterByKey("CG-99", new ComboMon(new DiscardCardToHandMon("Psychic Select"), new SkillCopyMon()));
            r.RegisterByKey("CG-100", new CelebiStarTimeTravel());
        }

        // ================================================================== Trainers

        /// <summary>Castaway (Supporter): search your deck for a Supporter card, a Pokémon Tool card and a basic Energy card and put them into your hand.</summary>
        private sealed class Castaway : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(d => d.IsSupporter, 1, "Castaway: choose a Supporter card"));
                if (ctx.Me.Deck.Count > 0) found.AddRange(ctx.SearchMyDeck(d => d.IsTool, 1, "Castaway: choose a Pokémon Tool card"));
                if (ctx.Me.Deck.Count > 0) found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Castaway: choose a basic Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Cessation Crystal (Tool; not on a Pokémon-ex): while the Pokémon it's attached to is Active, no Pokémon (either player's)
        /// can use Poké-Powers or Poké-Bodies (old Pokémon Powers too: Compendium). Discarded if its Pokémon becomes a Pokémon-ex.
        /// </summary>
        private sealed class CessationCrystal : CardScript
        {
            private static bool Works(EffectContext src) { return src.Pokemon != null && src.IsActive(src.Pokemon) && !src.Pokemon.Def.IsOldEx; }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && !target.Def.IsOldEx; }

            public override bool StopsEveryPowerAndBody(EffectContext src) { return Works(src); }

            public override void OnHolderEvolvedFromHand(EffectContext src, CardDef before)
            {
                if (src.Pokemon != null && src.Pokemon.Def.IsOldEx) src.Engine.DiscardTool(src.Pokemon, src.Card);
            }
        }

        /// <summary>
        /// Crystal Beach (Stadium): each Special Energy card that provides 2 or more Energy provides only 1 Colorless Energy (its
        /// other effects stay: Compendium).
        /// </summary>
        private sealed class CrystalBeach : CardScript
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (energy == null || !energy.Def.IsSpecialEnergy || units.Count < 2) return;
                units.Clear();
                units.Add(EnergyMask.Of(EnergyType.Colorless));
            }
        }

        /// <summary>
        /// Holon Circle (Stadium): an attack by either player's Active Pokémon ends as soon as it's announced, before anything else
        /// happens (Compendium, 2006-11-16), and Holon Circle is discarded.
        /// </summary>
        private sealed class HolonCircle : CardScript
        {
            public override bool EndsAnnouncedAttack(EffectContext src, PokemonInPlay attacker) { return attacker != null && src.IsActive(attacker); }

            public override void OnAnnouncedAttackEnded(EffectContext src, PokemonInPlay attacker)
            {
                src.Log(attacker.Name + "'s attack ends at once (Holon Circle), and Holon Circle is discarded.");
                src.Engine.DiscardStadium();
            }
        }

        /// <summary>
        /// Mysterious Shard (Tool; not on a Pokémon-ex): no effects of attacks, damage included, from the opponent's Pokémon-ex to
        /// the Pokémon it's attached to. Discarded at the end of the opponent's next turn.
        /// </summary>
        private sealed class MysteriousShard : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && !target.Def.IsOldEx; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !a.Def.IsOldEx || src.Pokemon.Def.IsOldEx) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack || src.Pokemon.Def.IsOldEx) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsOldEx;
            }

            public override void OnHolderEvolvedFromHand(EffectContext src, CardDef before)
            {
                if (src.Pokemon != null && src.Pokemon.Def.IsOldEx) src.Engine.DiscardTool(src.Pokemon, src.Card);
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                // A Tool is attached during its owner's turn, so the first opponent's turn that ends is "your opponent's next turn".
                if (src.Pokemon != null && src.State.CurrentPlayer != src.Player) src.Engine.DiscardTool(src.Pokemon, src.Card);
            }
        }

        /// <summary>Windstorm (Item): discard up to 2 Pokémon Tools and Stadium cards in play, in any combination (at least 1: Compendium).</summary>
        private sealed class Windstorm : CardScript
        {
            private static List<CardInstance> InPlay(EffectContext ctx)
            {
                var cards = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon())) cards.AddRange(mon.Tools.Where(t => t.Def.IsTool));
                if (ctx.State.Stadium != null) cards.Add(ctx.State.Stadium);
                return cards;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return InPlay(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = InPlay(ctx);
                if (cards.Count == 0) return;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(2, cards.Count), "Windstorm: choose up to 2 Pokémon Tools and Stadium cards in play to discard");
                foreach (var card in picks)
                {
                    if (card == ctx.State.Stadium)
                    {
                        ctx.Engine.DiscardStadium();
                        continue;
                    }
                    var holder = ctx.Engine.ToolHolder(card);
                    if (holder != null) ctx.Engine.DiscardTool(holder, card);
                }
            }
        }

        // ================================================================== Pokémon-ex

        /// <summary>
        /// Intimidating Armor (Poké-Body): while Aggron ex is your Active Pokémon, your opponent's Basic Pokémon can't attack or use
        /// Poké-Powers or Poké-Bodies. Split Bomb: 30 to each of 2 of the opponent's Pokémon. Mega Burn: not 2 turns in a row.
        /// </summary>
        private sealed class AggronExIntimidatingArmor : Mon
        {
            private const string Body = "Intimidating Armor";
            private static readonly Mon Attacks = new ComboMon(new CardsCES1.TwoTargetsMon("Split Bomb", 30), new LockAfterAttackMon("Mega Burn", true));

            [ThreadStatic] private static bool checking;

            private static bool Covers(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (checking || me == null || mon == null || mon.Owner == src.Player || !mon.IsBasic || !src.IsActive(me)) return false;
                checking = true;
                try { return !src.Engine.AbilityBlocked(me, Body); }
                finally { checking = false; }
            }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName) { return Covers(src, mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Burn Away: 100 to the Defending Pokémon, not affected by Weakness, Resistance or any effects on it. If that Knocks it Out,
        /// Blaziken ex does damage to itself equal to 100 minus the HP the Defending Pokémon had left.
        /// </summary>
        private sealed class BurnAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burn Away") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int left = ctx.RemainingHp(defending);
                ctx.DamageWith(defending, atk.Damage, false, false, true);
                bool knockedOut = !ctx.InPlay(defending) || ctx.RemainingHp(defending) <= 0;
                if (knockedOut && atk.Damage - left > 0 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(atk.Damage - left);
                return true;
            }
        }

        /// <summary>
        /// Constrain (Poké-Power): once during your turn, each player discards cards until they have 6 in their hand (you first).
        /// Not with a Special Condition. Upstream: 10 for each Energy card in your discard pile; they go into your deck, shuffled.
        /// </summary>
        private sealed class DelcattyExConstrain : Mon
        {
            protected override string Activated { get { return "Constrain"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                DiscardDownTo6(ctx, ctx.Player);
                DiscardDownTo6(ctx, ctx.Opponent);
            }

            private static void DiscardDownTo6(EffectContext ctx, int player)
            {
                var hand = ctx.State.Players[player].Hand;
                int extra = hand.Count - 6;
                if (extra <= 0) return;
                var picks = ctx.Engine.ChooseCards(player, hand.ToList(), extra, extra, "Constrain: discard " + extra + (extra == 1 ? " card" : " cards") + " from your hand", ctx.Card.Def);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(player, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Upstream") return false;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                ctx.DamageDefending(atk.Damage * energy.Count);
                if (energy.Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player))
                {
                    ctx.DiscardToDeck(energy);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>
        /// Extra Noise (Poké-Body): while Exploud ex is your Active Pokémon, 1 damage counter on each of your opponent's Pokémon-ex
        /// between turns. Derail: discard a Special Energy card from the Defending Pokémon after the damage (Compendium). Hyper Tail:
        /// 20 more against a Pokémon with a Poké-Power or Poké-Body.
        /// </summary>
        private sealed class ExploudExExtraNoise : Mon
        {
            private static readonly Mon Tail = CardsSS.ConditionBonus("Hyper Tail", 20, CardsHP.HasPowerOrBody);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var targets = src.OpponentPokemon(m => m.Def.IsOldEx);
                if (targets.Count == 0) return;
                Announce(src, "Extra Noise");
                foreach (var mon in targets)
                    if (src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Extra Noise");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Derail") return Tail.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => e.Def.IsSpecialEnergy))
                    ctx.DiscardEnergyFrom(defending, 1, "Derail: discard a Special Energy card from " + defending.Name, e => e.Def.IsSpecialEnergy);
                return true;
            }
        }

        /// <summary>Hard Rock (Poké-Body): with 1 Energy or less attached, attack damage to Groudon ex is 20 less (after W/R). Power Blast: discard 2 Energy.</summary>
        private sealed class GroudonExHardRock : Mon
        {
            private static readonly Mon Blast = new CardsTWM1.SelfDiscardMon("Power Blast", 2);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.EnergyCount(me) <= 1 ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Star Light (Poké-Body): while your opponent has any Pokémon-ex or Stage 2 Evolved Pokémon in play, Shield Beam and Super
        /// Psy Bolt cost 1 Colorless less. Shield Beam: your opponent can't use Poké-Powers during their next turn.
        /// </summary>
        private sealed class JirachiExStarLight : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack == null || (attack.Name != "Shield Beam" && attack.Name != "Super Psy Bolt")) return;
                if (!ctx.OpponentPokemon().Any(m => m.Def.IsOldEx || m.Def.Stage == Stage.Stage2)) return;
                cost.Remove(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shield Beam") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new NoPokePowersEffect("Your opponent can't use Poké-Powers during their next turn (Shield Beam)", ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>The owner's opponent can't use Poké-Powers until the effect ends (Shield Beam).</summary>
        private sealed class NoPokePowersEffect : ContinuousEffect
        {
            public NoPokePowersEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Owner != Owner; }
        }

        /// <summary>
        /// Flotation (Poké-Body): with 1 Energy or less attached, Kyogre ex has no Retreat Cost. Hydro Shot: discard 2 Energy from
        /// it, then 70 to 1 of the opponent's Pokémon (no W/R on the Bench).
        /// </summary>
        private sealed class KyogreExFlotation : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.EnergyCount(mon) <= 1 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Shot") return false;
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, Math.Min(2, me.Energy.Count), "Hydro Shot: discard 2 Energy from " + me.Name);
                ctx.DamageAnyOpponent(70);
                return true;
            }
        }

        /// <summary>
        /// Extra Liquid (Poké-Body): each player's Pokémon-ex can't use Poké-Powers and pays 1 Colorless more for its attacks. A
        /// Pokémon is affected by only 1 Extra Liquid (the first Sceptile ex in play does it). Power Revenge: 10 more for each Prize
        /// card your opponent has taken.
        /// </summary>
        private sealed class SceptileExExtraLiquid : Mon
        {
            private const string Body = "Extra Liquid";
            private static readonly Mon Revenge = new CardsASR1.PlusMon("Power Revenge", 10, c => c.Engine.PrizesTaken(c.Opponent));

            [ThreadStatic] private static bool checking;

            /// <summary>This Sceptile ex's Extra Liquid works, and no earlier Sceptile ex in play has a working one.</summary>
            private static bool Applies(EffectContext src)
            {
                var me = src.Pokemon;
                if (checking || me == null) return false;
                checking = true;
                try
                {
                    if (src.Engine.AbilityBlocked(me, Body)) return false;
                    foreach (var other in src.State.Players.SelectMany(p => p.InPlay()))
                        if (other != me && other.Uid < me.Uid && other.Def.Abilities.Any(a => a.Name == Body) && !src.Engine.AbilityBlocked(other, Body)) return false;
                    return true;
                }
                finally { checking = false; }
            }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return mon != null && mon.Def.IsOldEx && Applies(src); }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != null && mon.Def.IsOldEx && Applies(src)) cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Revenge.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Dark Eyes (Poké-Body): after your opponent's Pokémon uses a Poké-Power, 2 damage counters on it (each Shiftry ex adds
        /// its own: Compendium). Target Attack: 30 to 1 of the opponent's Pokémon, 50 if it already has damage counters.
        /// </summary>
        private sealed class ShiftryExDarkEyes : Mon
        {
            public override void OnPokePowerUsed(EffectContext src, PokemonInPlay user, string power)
            {
                if (src.Pokemon == null || user == null || user.Owner == src.Player || !src.InPlay(user)) return;
                Announce(src, "Dark Eyes");
                src.Engine.PlaceDamageCounters(user, 2, "Dark Eyes");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Target Attack") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Target Attack: choose 1 of your opponent's Pokémon (30 damage, 50 if it already has damage counters)");
                if (target != null) ctx.DamageTarget(target, target.DamageCounters > 0 ? 50 : 30);
                return true;
            }
        }

        /// <summary>
        /// Energy Recycle (Poké-Power): once during your turn, attach up to 3 Energy cards from your discard pile to your Pokémon in
        /// any way you like; if you do, your turn ends. Not with a Special Condition. Ultra Pump: you may discard 2 cards from your
        /// hand for 20 more damage and 20 to 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class SwampertExEnergyRecycle : Mon
        {
            protected override string Activated { get { return "Energy Recycle"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Discard.Any(c => c.Def.IsEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0) return;
                if (ctx.AttachCardsAnyWay(pile, 3, (c, m) => true, "Energy Recycle") > 0) ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultra Pump") return false;
                bool pumped = false;
                if (ctx.Me.Hand.Count >= 2 && ctx.AskYesNo("Ultra Pump: discard 2 cards from your hand for 20 more damage and 20 to 1 of your opponent's Benched Pokémon?"))
                {
                    var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Ultra Pump: discard 2 cards from your hand");
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                    pumped = true;
                }
                ctx.DamageDefending(atk.Damage + (pumped ? 20 : 0));
                if (pumped) ctx.DamageOneBenched(20);
                return true;
            }
        }

        // ================================================================== Pokémon ★

        /// <summary>
        /// Skill Copy: discard a Basic Pokémon or Evolution card from your hand and use 1 of its attacks as this attack; it does
        /// nothing unless Alakazam ★ has the Energy that attack needs.
        /// </summary>
        private sealed class SkillCopyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Skill Copy") return false;
                var cards = ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Attacks.Count > 0).ToList();
                if (cards.Count == 0) return true;
                var pick = ctx.ChooseCards(cards, 1, 1, "Skill Copy: discard a Basic Pokémon or Evolution card from your hand to copy 1 of its attacks");
                if (pick.Count == 0) return true;
                var card = pick[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                int index = card.Def.Attacks.Count == 1 ? 0
                    : ctx.ChooseOption("Skill Copy: copy which attack of " + card.Def.Name + "?", card.Def.Attacks.Select(a => a.Name).ToArray());
                if (!ctx.Engine.CanPayAttack(ctx.Pokemon, card.Def.Attacks[index]))
                {
                    ctx.Log("Skill Copy: " + ctx.Pokemon.Name + " doesn't have the Energy for " + card.Def.Attacks[index].Name + ", so it does nothing.");
                    return true;
                }
                UseCopiedAttack(ctx, card.Def, index);
                return true;
            }
        }

        /// <summary>
        /// Time Travel (Poké-Power): if Celebi ★ would be Knocked Out by damage from an opponent's attack, you may flip a coin; heads,
        /// it isn't Knocked Out: its attached cards are discarded and it goes on the bottom of your deck. Leaf Shade: as many damage
        /// counters as the Energy on Celebi ★ on 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class CelebiStarTimeTravel : Mon
        {
            public override bool EscapesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon || !mon.HitByAttack || mon.ForcedKnockOut || mon.HitByPlayer != 1 - src.Player) return false;
                if (!src.AskYesNo("Time Travel: flip a coin? Heads, Celebi ★ isn't Knocked Out and goes on the bottom of your deck.")) return false;
                Announce(src, "Time Travel");
                return src.Flip();
            }

            public override bool EscapeDiscardsAttachedToDeckBottom { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Shade") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                var all = ctx.OpponentPokemon();
                if (n <= 0 || all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Leaf Shade: put " + n + " damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, n);
                return true;
            }
        }
    }
}
