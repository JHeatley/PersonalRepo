using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// HGSS Black Star Promos (HSP HGSS01-HGSS25, 2010-2011; v135). Every card came out before Black &amp; White: Unlimited only.
    /// Poké-Powers can't be used while the Pokémon has a Special Condition; Poké-Bodies always work (both are switched off by
    /// what switches off Poké-Powers and Poké-Bodies, not by what blocks Abilities). Rain Dance and Leaf Trans share Blastoise's
    /// and Venusaur's Classic Collection code (CardsCEL). Your rulings: Tropical Tidal Wave discards Pokémon Tools and that
    /// player's Stadium only (Trainer cards in play as Pokémon stay); Porygon2's Shortcut stacks.
    /// Pokémon whose attacks only do their printed damage: Hitmonlee 25.
    /// </summary>
    public static class CardsHSP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HSP-1", new SacredFireMon());
            r.RegisterByKey("HSP-2", new CardsWHT.FlipMoreMon("Aeroblast", 2, 20));
            r.RegisterByKey("HSP-3", new PikachuHSP());
            r.RegisterByKey("HSP-4", new WobbuffetHSP());
            r.RegisterByKey("HSP-5", new CardsPRE.HoothootPRE());
            r.RegisterByKey("HSP-6", new NoctowlHSP());
            r.RegisterByKey("HSP-7", new FeraligatrHSP());
            r.RegisterByKey("HSP-8", new MeganiumHSP());
            r.RegisterByKey("HSP-9", new TyphlosionHSP());
            r.RegisterByKey("HSP-10", new ComboMon(new CardsDEX.EnergyAssistOneMon(), new InfiniteWindMon()));
            r.RegisterByKey("HSP-11", new LatiosHSP());
            r.RegisterByKey("HSP-12", new SweetSleepingFaceMon(new EeeeeeekMon()));
            r.RegisterByKey("HSP-13", new SweetSleepingFaceMon(new EnergyAnticsMon()));
            r.RegisterByKey("HSP-14", new CardsLOT4.BeckonMon("Ferry"));
            r.RegisterByKey("HSP-15", new ShuckleHSP());
            r.RegisterByKey("HSP-16", new ComboMon(new DrawAttackMon("Collect", 2), new CardsFST1.FlipRecoilMon("Thunder Jolt", 10)));
            r.RegisterByKey("HSP-17", new ComboMon(new BenchSearchAttackMon("Call for Family", 2),
                new CardsFST2.ConditionBonusMon("Tag Team Boost", 20, c => c.Me.Bench.Any(m => m.Name == "Plusle"))));
            r.RegisterByKey("HSP-18", new TropicalTidalWave());
            r.RegisterByKey("HSP-19", new LockAfterAttackMon("Zap Cannon", true));
            r.RegisterByKey("HSP-20", new CardsSUM1.DiscardTypeFromSelfMon("Flare Blitz", EnergyType.Fire, 99));
            r.RegisterByKey("HSP-21", new CardsLOR1.EntanglingStringMon("Sheer Cold"));
            r.RegisterByKey("HSP-22", new ComboMon(new CardsASC.SelfGuardMon("Stiffen", 20), new SearchAttackMon("Version Update", d => d.Name == "Porygon2", 1)));
            r.RegisterByKey("HSP-23", new Porygon2HSP());
            r.RegisterByKey("HSP-24", new ComboMon(new DodgeMon("Detect", false), new CardsFLF1.NoResistanceMon("Sky Uppercut")));
            r.RegisterByKey("HSP-25", new PlainMon());
        }

        /// <summary>Poké-Powers: "This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        private static bool NoCondition(PokemonInPlay mon)
        {
            return mon != null && mon.Conditions == SpecialCondition.None;
        }

        /// <summary>The ability at this index is the named one (on the card it's printed on).</summary>
        private static bool IsAbility(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        /// <summary>"[This Pokémon] is now Asleep" at the end of its own attack (it's still the Active Pokémon).</summary>
        private static void FallAsleep(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.Engine.ApplyCondition(me, SpecialCondition.Asleep);
        }

        // ================================================================== Pokémon

        /// <summary>Sacred Fire: flip a coin; heads, 80 damage to 1 of the opponent's Pokémon, not affected by Weakness or Resistance.</summary>
        private sealed class SacredFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sacred Fire") return false;
                if (ctx.Flip()) ctx.DamageAnyOpponent(80, true);
                return true;
            }
        }

        /// <summary>
        /// Recharge: flip a coin; heads, search the deck for a Lightning Energy card and attach it to this Pokémon. Thunderbolt:
        /// discard all Energy from this Pokémon (after the damage).
        /// </summary>
        private sealed class PikachuHSP : Mon
        {
            private static readonly Mon Thunderbolt = new CardsTWM1.SelfDiscardMon("Thunderbolt", 99);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recharge") return Thunderbolt.HandleAttack(ctx, atk);
                var self = ctx.Pokemon;
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Lightning, 1, "Recharge: choose a Lightning Energy card to attach to " + self.Name);
                foreach (var e in found)
                {
                    if (ctx.InPlay(self)) ctx.Engine.AttachEnergyCard(e, self);
                    else ctx.Me.Deck.Add(e);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Tenacious Bind (Poké-Body): while this Pokémon is your Active Pokémon, the opponent's Active Pokémon's Retreat Cost is [C][C] more. Trip Over: heads, 30 more.</summary>
        private sealed class WobbuffetHSP : Mon
        {
            private static readonly Mon TripOver = new FlipBonusMon("Trip Over", 30);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                return me != null && mon != null && mon.Owner != src.Player && src.IsActive(me) && src.IsActive(mon) ? cost + 2 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TripOver.HandleAttack(ctx, atk); }
        }

        /// <summary>Night Scope (Poké-Power): once during your turn, look at your opponent's hand. Hypnoblast: the Defending Pokémon is now Asleep.</summary>
        private sealed class NoctowlHSP : Mon
        {
            private const string NightScope = "Night Scope";
            private static readonly Mon Hypnoblast = new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep);

            protected override string Activated { get { return NightScope; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", NightScope + ": your opponent's hand", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand (" + NightScope + ").");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hypnoblast.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rain Dance (Poké-Power): as often as you like, attach a Water Energy card from your hand to 1 of your Water Pokémon.
        /// Hydro Crunch: 10 more for each damage counter on the Defending Pokémon.
        /// </summary>
        private sealed class FeraligatrHSP : Mon
        {
            private const string RainDance = "Rain Dance";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, RainDance) && ctx.IsMyTurn && NoCondition(ctx.Pokemon) && CardsCEL.RainDanceReady(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                CardsCEL.UseRainDance(ctx, RainDance);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Crunch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 10 * (defending != null ? defending.DamageCounters : 0));
                return true;
            }
        }

        /// <summary>Leaf Trans (Poké-Power): as often as you like, move a Grass Energy from 1 of your Pokémon to another (like Venusaur's Energy Trans).</summary>
        private sealed class MeganiumHSP : Mon
        {
            private const string LeafTrans = "Leaf Trans";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, LeafTrans) && ctx.IsMyTurn && NoCondition(ctx.Pokemon) && CardsCEL.EnergyTransReady(ctx, EnergyType.Grass);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                CardsCEL.UseEnergyTrans(ctx, LeafTrans, EnergyType.Grass);
            }
        }

        /// <summary>
        /// Afterburner (Poké-Power): once during your turn, attach a Fire Energy card from your discard pile to 1 of your Pokémon and
        /// put 1 damage counter on it. Flare Destroy: discard an Energy from this Pokémon and one from the Defending Pokémon.
        /// </summary>
        private sealed class TyphlosionHSP : Mon
        {
            private const string Afterburner = "Afterburner";
            private static readonly Mon FlareDestroy = new CardsBKT1.FlareDestroyMon();

            private static List<CardInstance> FireInDiscard(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
            }

            protected override string Activated { get { return Afterburner; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && FireInDiscard(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var fire = FireInDiscard(ctx);
                if (fire.Count == 0) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), Afterburner + ": attach a Fire Energy card from your discard pile to which of your Pokémon?");
                if (target == null) return;
                var card = fire[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                if (ctx.Engine.AttachEnergyCard(card, target) && ctx.InPlay(target)) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FlareDestroy.HandleAttack(ctx, atk); }
        }

        /// <summary>Infinite Wind: if Latios is on your Bench, heal 20 damage (2 damage counters) from each of your Benched Pokémon.</summary>
        private sealed class InfiniteWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Infinite Wind") return false;
                bool latios = ctx.Me.Bench.Any(m => m.Name == "Latios");
                ctx.DamageDefending(atk.Damage);
                if (latios)
                    foreach (var benched in ctx.MyBench()) ctx.Heal(benched, 20);
                return true;
            }
        }

        /// <summary>Luster Float (Poké-Body): with Latias in play, this Pokémon has no Retreat Cost. Infinite Wing: discard 2 Energy from it.</summary>
        private sealed class LatiosHSP : Mon
        {
            private static readonly Mon InfiniteWing = new CardsTWM1.SelfDiscardMon("Infinite Wing", 2);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Me.InPlay().Any(m => m.Name == "Latias") ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return InfiniteWing.HandleAttack(ctx, atk); }
        }

        /// <summary>Sweet Sleeping Face (Poké-Body): while this Pokémon is Asleep, prevent all damage done to it by attacks.</summary>
        private sealed class SweetSleepingFaceMon : Mon
        {
            private readonly Mon attacks;

            public SweetSleepingFaceMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !me.Has(SpecialCondition.Asleep) || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Eeeeeeek: shuffle your hand into your deck, draw 6 cards, then this Pokémon is now Asleep.</summary>
        private sealed class EeeeeeekMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eeeeeeek") return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(6);
                FallAsleep(ctx);
                return true;
            }
        }

        /// <summary>Energy Antics: move an Energy card from 1 of the opponent's Pokémon to another of theirs; this Pokémon is now Asleep.</summary>
        private sealed class EnergyAnticsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Antics") return false;
                MoveOne(ctx);
                FallAsleep(ctx);
                return true;
            }

            private static void MoveOne(EffectContext ctx)
            {
                if (ctx.OpponentPokemon().Count < 2) return;
                var givers = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (givers.Count == 0) return;
                var from = ctx.ChoosePokemon(givers, "Energy Antics: move an Energy card from which of your opponent's Pokémon?");
                if (from == null) return;
                var energy = from.Energy.Count == 1 ? from.Energy[0] : ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Energy Antics: move which Energy card?")[0];
                var receivers = ctx.OpponentPokemon(m => m != from && ctx.CanAffect(m));
                if (receivers.Count == 0) return;
                var to = ctx.ChoosePokemon(receivers, "Energy Antics: move " + energy.Def.Name + " to which of your opponent's Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(from, to, energy);
            }
        }

        /// <summary>
        /// Fermenting Liquid (Poké-Body): whenever you attach an Energy card from your hand to this Pokémon (by any means), draw a
        /// card. Shell Stunner: heads, prevent all damage done to it by attacks during the opponent's next turn.
        /// </summary>
        private sealed class ShuckleHSP : Mon
        {
            private static readonly Mon ShellStunner = new CardsSP1.FlipNoDamageMon("Shell Stunner");

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me) return;
                Announce(src, "Fermenting Liquid");
                src.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ShellStunner.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Shortcut (Poké-Body): each Porygon, Porygon2 and Porygon-Z you have in play has a Retreat Cost [C] less. It stacks: each
        /// Porygon2 with Shortcut takes off another [C] (your ruling). Reckless Charge: 10 damage to itself.
        /// </summary>
        private sealed class Porygon2HSP : Mon
        {
            private static readonly Mon RecklessCharge = new RecoilMon("Reckless Charge", 10);

            private static bool PorygonLine(PokemonInPlay mon)
            {
                var name = mon.Name;
                return name == "Porygon" || name == "Porygon2" || name == "Porygon-Z";
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && PorygonLine(mon) ? Math.Max(0, cost - 1) : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return RecklessCharge.HandleAttack(ctx, atk); }
        }

        // ================================================================== Trainers

        /// <summary>
        /// Tropical Tidal Wave (old Trainer, an Item): flip a coin. Heads, discard all Trainer and Stadium cards your opponent has
        /// in play; tails, all of yours. That's the Pokémon Tools the player owns (on anyone's Pokémon) and the Stadium if they
        /// played it; Trainer cards in play as Pokémon (Clefairy Doll, fossils, Robo Substitute) stay (your ruling).
        /// </summary>
        private sealed class TropicalTidalWave : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int player = ctx.Flip() ? ctx.Opponent : ctx.Player;
                int discarded = 0;
                for (int i = 0; i < 2; i++)
                    foreach (var mon in ctx.State.Players[i].InPlay().ToList())
                        foreach (var tool in mon.Tools.Where(t => t.Owner == player).ToList())
                            if (ctx.Engine.DiscardTool(mon, tool)) discarded++;
                if (ctx.State.Stadium != null && ctx.State.StadiumOwner == player)
                {
                    ctx.Engine.DiscardStadium();
                    discarded++;
                }
                if (discarded == 0) ctx.Log("Tropical Tidal Wave: " + ctx.Engine.PlayerName(player) + " has no Trainer or Stadium cards in play.");
            }
        }
    }
}
