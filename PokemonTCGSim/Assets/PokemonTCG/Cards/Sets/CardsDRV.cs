using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragon Vault (DRV 1-21, 2012; Expanded). 6 and 7 are prints of Plasma Blast's Bagon and Shelgon, 11 of Legendary Treasures'
    /// Rayquaza; Exp. Share (18) and Super Rod (20) were already in the pool. Your rulings (v124): First Ticket works only in a
    /// format that draws the hands before the coin flip; Salamence's Scornful Storm has no Active Pokémon condition and the opponent
    /// chooses the cards they discard.
    /// </summary>
    public static class CardsDRV
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DRV-1", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("DRV-2", new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep));
            r.RegisterByKey("DRV-3", new DiscardTopMon("Dragon Pulse", 1));
            r.RegisterByKey("DRV-4", new ComboMon(new HealEachMon("Healing Melody", 10), new CoinsMon("Slam", 2)));
            r.RegisterByKey("DRV-5", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"), new CoinsMon("Hurricane Tail", 4)));
            r.RegisterByKey("DRV-8", new SalamenceScornfulStorm());
            r.RegisterByKey("DRV-9", new ComboMon(new EnergyAssistOneMon(), new BenchPartnerMon("Sky Heal", "Latios", 0, 20)));
            r.RegisterByKey("DRV-10", new BenchPartnerMon("Sky Blade", "Latias", 20, 0));
            r.RegisterByKey("DRV-12", new SignsOfEvolutionMon("Fraxure"));
            r.RegisterByKey("DRV-13", new TailsNothingMon("Lunge"));
            r.RegisterByKey("DRV-14", new FraxureGrit());
            r.RegisterByKey("DRV-16", new ComboMon(new AxeSluggerMon(), new DiscardTopMon("Dragon Pulse", 1)));
            r.RegisterByKey("DRV-17", new ComboMon(new ParalyzeFlipMon("Tight Jaw"), new CoinsMon("Dragon Tail", 2)));
            r.RegisterByKey("DRV-19", new FirstTicket());
            r.RegisterByKey("DRV-21", new BenchSpreadMon("Blizzard", 10));
        }

        /// <summary>Dragon Pulse: the damage, then discard the top N cards of your deck.</summary>
        private sealed class DiscardTopMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public DiscardTopMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, Math.Min(count, ctx.Me.Deck.Count));
                return true;
            }
        }

        /// <summary>Healing Melody: heal N damage from each of your Pokémon.</summary>
        private sealed class HealEachMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealEachMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, amount);
                return true;
            }
        }

        /// <summary>
        /// Scornful Storm (Ability): once during your turn (before your attack), you may have your opponent discard cards from their
        /// hand until they have 4. They choose the cards (your ruling: Active or Benched, no condition).
        /// </summary>
        private sealed class SalamenceScornfulStorm : Mon
        {
            private const string Storm = "Scornful Storm";
            private static readonly Mon Shred = new CardsPAF2.IgnoreDefenderEffectsMon("Shred");

            protected override string Activated { get { return Storm; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 4; }

            protected override void Ability(EffectContext ctx)
            {
                int extra = ctx.Opp.Hand.Count - 4;
                if (extra <= 0) return;
                var opp = new EffectContext(ctx.Engine, ctx.Opponent, ctx.Card, null);
                var hand = ctx.Opp.Hand.ToList();
                var picks = opp.ChooseCards(hand, extra, extra, Storm + ": discard " + extra + " card" + (extra == 1 ? "" : "s") + " from your hand (down to 4)");
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Contains(card)) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " discarded " + picks.Count + " card" + (picks.Count == 1 ? "" : "s") + " down to 4 (Scornful Storm).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shred.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Assist: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class EnergyAssistOneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, 1, (card, mon) => !ctx.IsActive(mon), "Energy Assist: attach a basic Energy card to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        /// <summary>
        /// "If [partner] is on your Bench": Sky Heal (heal 20 from this Pokémon after the damage), Sky Blade (20 more damage). The
        /// partner is a Pokémon with exactly that name (not Latias-EX or Latios-EX).
        /// </summary>
        private sealed class BenchPartnerMon : Mon
        {
            private readonly string attack;
            private readonly string partner;
            private readonly int bonus;
            private readonly int heal;

            public BenchPartnerMon(string attack, string partner, int bonus, int heal)
            {
                this.attack = attack;
                this.partner = partner;
                this.bonus = bonus;
                this.heal = heal;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool withPartner = ctx.Me.Bench.Any(m => m.Name == partner);
                ctx.DamageDefending(atk.Damage + (withPartner ? bonus : 0));
                var me = ctx.Pokemon;
                if (withPartner && heal > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, heal);
                return true;
            }
        }

        /// <summary>Signs of Evolution: flip a coin; heads, search your deck for a card with that name and put it into your hand.</summary>
        private sealed class SignsOfEvolutionMon : Mon
        {
            private readonly string wanted;

            public SignsOfEvolutionMon(string wanted) { this.wanted = wanted; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Signs of Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.Name == wanted, 1, "Signs of Evolution: choose " + wanted);
                return true;
            }
        }

        /// <summary>Grit (Ability): while this Pokémon has any Special Condition, its attacks do 40 more damage to the opponent's Active Pokémon (before W/R).</summary>
        private sealed class FraxureGrit : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || me.Conditions == SpecialCondition.None) return damage;
                return damage + 40;
            }
        }

        /// <summary>Axe Slugger: 60 more damage if the Defending Pokémon is a Colorless Pokémon.</summary>
        private sealed class AxeSluggerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Axe Slugger") return false;
                var defending = ctx.Defending;
                bool colorless = defending != null && ctx.Engine.IsType(defending, EnergyType.Colorless);
                ctx.DamageDefending(atk.Damage + (colorless ? 60 : 0));
                return true;
            }
        }

        /// <summary>Blizzard: the damage, and N to each of the opponent's Benched Pokémon (no W/R).</summary>
        private sealed class BenchSpreadMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BenchSpreadMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Opp.Bench.ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>
        /// First Ticket: played from the opening hand before the coin flip for who goes first (GameEngine.PlayFirstTickets), only in
        /// a format that draws the hands first (your ruling). It can't be played during a game.
        /// </summary>
        private sealed class FirstTicket : CardScript
        {
            public override bool PlayableBeforeCoinFlip { get { return true; } }

            public override bool CanPlayTrainer(EffectContext ctx) { return false; }
        }
    }
}
