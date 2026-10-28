using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Flashfire (FLF, v104), part 2: the Fighting, Darkness, Metal, Fairy, Dragon and Colorless Pokémon (45-87). Geodude,
    /// Graveler, Golem and Snorlax are Generations' cards, Flabébé (63) and Floette (65) its Radiant Collection ones; Goomy,
    /// Sliggoo and Furfrou are Phantom Forces' cards.
    /// </summary>
    public static class CardsFLF2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FLF-48", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("FLF-49", new RockRushMon());
            r.RegisterByKey("FLF-50", new CardsTEU3.OpponentDiscardsMon("Flash Claw", 1));
            r.RegisterByKey("FLF-51", new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep));
            r.RegisterByKey("FLF-52", new ComboMon(new CallForFamilyMon(), new CardsPFL.DamagedBonusMon("Claw Rend", 30)));
            r.RegisterByKey("FLF-53", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("FLF-54", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("FLF-55", new ComboMon(new CardsASC.PushDownMon("Push Down"), new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray")));
            r.RegisterByKey("FLF-56", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("FLF-57", new ComboMon(new NoRetreatMon("Dark Clamp"), new CoinsMon("Double Swing", 2)));
            r.RegisterByKey("FLF-58", new CoinsMon("Tail Rap", 2));
            r.RegisterByKey("FLF-59", new NoRetreatMon("Corner"));
            r.RegisterByKey("FLF-60", new ForretressTempest());
            r.RegisterByKey("FLF-61", new ComboMon(new ChipOffMon(), new FlipBonusMon("X-Scissor", 20)));
            r.RegisterByKey("FLF-62", new CardsSHF1.HealEachMon("Aromatherapy", 10, false));
            r.RegisterByKey("FLF-64", new FloetteVeil());
            r.RegisterByKey("FLF-66", new ComboMon(new CardsLOT4.BrilliantSearchMon(), new CardsCEC3.SpreadAmountMon("Petal Blizzard", 20)));
            r.RegisterByKey("FLF-67", new TailsNothingMon("Fickle Attack"));
            r.RegisterByKey("FLF-68", new ComboMon(new CardsFCO2.FlipProtectMon("Crystal Barrier"),
                new CardsASR1.PlusMon("Wonder Blast", 20, c => c.Pokemon.Energy.Count(en => CardsDRI1.ProvidesType(c, c.Pokemon, en, EnergyType.Fairy)))));
            r.RegisterByKey("FLF-69", new CardsBRS1.MillSelfMon("Wild Blaze", 5));
            r.RegisterByKey("FLF-70", new Cards30C.RetaliateBonusMon("Revenge", 70));
            r.RegisterByKey("FLF-71", new DragalgeBarrier());
            r.RegisterByKey("FLF-74", new GoodraRegeneration());
            r.RegisterByKey("FLF-75", new CardsBLK.PeckOffMon("Peck Off"));
            r.RegisterByKey("FLF-76", new ComboMon(new CardsMEG.SandAttackMon("Sand-Attack"), new FlipBonusMon("Ambush", 20)));
            r.RegisterByKey("FLF-77", new ComboMon(new NoRetreatMon("Clutch"), new CardsASC.NextTurnBoostMon("Strong Gust", 60)));
            r.RegisterByKey("FLF-78", new ComboMon(new DrawAttackMon("Triple Draw", 3), new FlipBonusMon("Kindred Kick", 30)));
            r.RegisterByKey("FLF-79", new CardsFCO2.FlipUntilTailsMoreMon("Wham Bam Punch", 30));
            r.RegisterByKey("FLF-82", new ComboMon(new BuriedTreasureHuntMon(), new CoinsMon("Slam", 2)));
            r.RegisterByKey("FLF-83", new CardsFST2.ConditionBonusMon("Powerful Friends", 70, c => c.Me.Bench.Any(m => m.Def.Stage == Stage.Stage2)));
            r.RegisterByKey("FLF-84", new SwitchSelfMon("Bounce", false));
            r.RegisterByKey("FLF-85", new LopunnyJump());
            r.RegisterByKey("FLF-86", new FlipBonusMon("Quick Attack", 10));
        }

        // ================================================================== Shared helpers

        // ================================================================== Fighting

        /// <summary>Rock Rush: discard as many Fighting Energy cards from your hand as you like; 30 damage for each one.</summary>
        private sealed class RockRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Rush") return false;
                var fighting = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting)).ToList();
                int n = 0;
                if (fighting.Count > 0)
                {
                    var picks = ctx.ChooseCards(fighting, 0, fighting.Count, "Rock Rush: discard any number of Fighting Energy cards from your hand (30 damage each)");
                    foreach (var card in picks)
                    {
                        if (!ctx.Me.Hand.Contains(card)) continue;
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                }
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>
        /// Thorn Tempest: when you play this Pokémon from your hand to evolve, you may put 1 damage counter on each of the opponent's
        /// Pokémon. Iron Crash: 20 more for each [C] in the opponent's Active Pokémon's Retreat Cost.
        /// </summary>
        private sealed class ForretressTempest : Mon
        {
            private static readonly Mon Crash = new CardsASR1.PlusMon("Iron Crash", 20, c => c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending));

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.Opp.HasPokemonInPlay;
                if (!TriggerOnce(ctx, "Thorn Tempest", ready, "Use Thorn Tempest? Put 1 damage counter on each of your opponent's Pokémon.")) return;
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crash.HandleAttack(ctx, atk); }
        }

        /// <summary>Chip Off: discard cards at random from the opponent's hand until they have 4.</summary>
        private sealed class ChipOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chip Off") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int extra = ctx.Opp.Hand.Count - 4;
                if (extra > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, extra);
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Flower Veil: each of your Grass Pokémon in play gets +20 HP (each Floette's adds up, your ruling).</summary>
        private sealed class FloetteVeil : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !src.Engine.IsType(mon, EnergyType.Grass)) return hp;
                return hp + 20;
            }
        }

        // ================================================================== Dragon

        /// <summary>Poison Barrier: the opponent's Poisoned Pokémon can't retreat. Poison Breath: flip for Poisoned.</summary>
        private sealed class DragalgeBarrier : Mon
        {
            private static readonly Mon Breath = new CardsPAR2.FlipConditionMon("Poison Breath", SpecialCondition.Poisoned);

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && mon.Has(SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Breath.HandleAttack(ctx, atk); }
        }

        /// <summary>Gooey Regeneration: as often as you like during your turn, discard an Energy from this Pokémon to heal 60 from it. Heavy Whip.</summary>
        private sealed class GoodraRegeneration : Mon
        {
            private const string Regen = "Gooey Regeneration";
            private static readonly Mon Whip = new FlipBonusMon("Heavy Whip", 40);

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Regen
                    && ctx.IsMyTurn && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.Energy.Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => ctx.InPlay(me) && me.Energy.Count > 0, offerDone =>
                {
                    if (offerDone && !ctx.AskYesNo(Regen + ": discard an Energy from " + me.Name + " to heal 60 damage from it?")) return false;
                    if (ctx.DiscardEnergyFrom(me, 1, Regen + ": discard an Energy from " + me.Name) == 0) return false;
                    ctx.Heal(me, 60);
                    return true;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Whip.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Buried Treasure Hunt: look at the top 4 cards of your deck, put 2 of them into your hand and discard the others.</summary>
        private sealed class BuriedTreasureHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Buried Treasure Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return true;
                int n = Math.Min(2, top.Count);
                var keep = n == top.Count ? top : ctx.ChooseCards(top, n, n, "Buried Treasure Hunt: put 2 of these cards into your hand (the others are discarded)");
                ctx.DeckToHand(keep);
                foreach (var card in top)
                {
                    if (keep.Contains(card)) continue;
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Discard.Add(card);
                }
                return true;
            }
        }

        /// <summary>
        /// Big Jump: once during your turn, return this Pokémon and all attached cards to your hand, from the Active Spot or the
        /// Bench, even when it's your only Pokémon in play (then you have no Pokémon left and lose; your ruling). Sitdown Bounce:
        /// flip; tails, it can't attack during your next turn.
        /// </summary>
        private sealed class LopunnyJump : Mon
        {
            protected override string Activated { get { return "Big Jump"; } }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.Engine.ReturnPokemonToHand(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sitdown Bounce") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }
    }
}
