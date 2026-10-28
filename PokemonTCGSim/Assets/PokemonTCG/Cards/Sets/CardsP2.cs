using System;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// POP Series 2 (P2 1-17; v167). Unlimited only. Pidgeot, Ivysaur, Mr. Briney's Compassion, Multi Technical Machine 01, Pokémon
    /// Park, TV Reporter and Pikachu are prints. 1-on-1: Luvdisc's attacks that need 2 Defending Pokémon do only their damage.
    /// </summary>
    public static class CardsP2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P2-1", new ComboMon(new FlipBonusMon("Stomp", 20), new CardsRS.DiscardBasicOrNothingMon("Fire Spin", 2, false)));
            r.RegisterByKey("P2-3", new ComboMon(new CardsE3B.OpponentSwitchesMon("Roar"), new CardsFST1.FlipRecoilMon("Thunder", 20)));
            r.RegisterByKey("P2-4", new CardsDX.MirrorCoatMon(new ParalyzeFlipMon("Bubblebeam")));
            r.RegisterByKey("P2-5", new ComboMon(new Cards30C.SelfCounterBonusMon("Rage", 10), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("P2-6", new ComboMon(new CardsFST1.TwoBenchedMon("Wide Solarbeam", 20), new LockAfterAttackMon("Hard Plant", true)));
            r.RegisterByKey("P2-12", new PlainMon());
            r.RegisterByKey("P2-13", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("P2-14", new ParalyzingKissMon());
            r.RegisterByKey("P2-15", new ComboMon(new CoinsMon("Double Spin", 2), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("P2-17", new CelebiExTimeReversal());
        }

        /// <summary>Paralyzing Kiss: needs 2 Defending Pokémon, so it does nothing in a 1-on-1 game. (Fast Stream: the same, so just its damage.)</summary>
        private sealed class ParalyzingKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fast Stream")
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name != "Paralyzing Kiss") return false;
                return Nothing(ctx);
            }
        }

        /// <summary>Time Reversal (Poké-Power): when you bench Celebi ex from your hand, a card from your discard pile goes on top of your deck. Psychic Shield.</summary>
        private sealed class CelebiExTimeReversal : Mon
        {
            private const string Power = "Time Reversal";
            private static readonly Mon Shield = new CardsDX.ShieldFromMon("Psychic Shield", m => m.Def.IsOldEx, "Pokémon-ex");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, ctx.Me.Discard.Count > 0, Power + ": put a card from your discard pile on top of your deck?")) return;
                var pile = ctx.Me.Discard.ToList();
                var card = ctx.ChooseCards(pile, 1, 1, Power + ": put which card on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Me.Deck.Insert(0, card);
                ctx.Engine.Reveal(ctx.Player, new System.Collections.Generic.List<CardInstance> { card }, "put this card from " + ctx.Engine.Their(ctx.Player) + " discard pile on top of " + ctx.Engine.Their(ctx.Player) + " deck (Time Reversal)");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shield.HandleAttack(ctx, atk); }
        }
    }
}
