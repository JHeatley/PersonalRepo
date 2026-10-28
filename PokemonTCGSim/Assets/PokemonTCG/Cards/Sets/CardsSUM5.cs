using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sun &amp; Moon (SUM, v90), part 5: Colorless and Trainers (SUM 97-137). Eevee and Oranguru (Hidden Fates Shiny Vault), the
    /// Trainers reprinted in later sets and Double Colorless and Rainbow Energy were already in the pool and keep their scripts.
    /// </summary>
    public static class CardsSUM5
    {
        public static void Register(CardScriptRegistry r)
        {
            // Colorless
            r.RegisterByKey("SUM-97", new CardsFST2.ConditionBonusMon("Peck Bugs", 30, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Grass)));
            r.RegisterByKey("SUM-98", new ComboMon(new ShuffleOpponentHandDrawMon("Devastating Wind", 3), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("SUM-99", new ComboMon(new CardsPAL1.EvolutionBonusMon("Cross-Cut", 30), new CoinsMon("Hurricane Punch", 4)));
            r.RegisterByKey("SUM-100", new ComboMon(new Cards30C.SelfCounterBonusMon("Rage", 10), new CardsASR1.TimesMon("Mad Bull-GX", c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("SUM-102", new ConditionAttackMon("Teeter Punch", SpecialCondition.Confused));
            r.RegisterByKey("SUM-103", new CardsFST1.NextTurnBonusMon("Work Up", 20));
            r.RegisterByKey("SUM-104", new HerdierSUM());
            r.RegisterByKey("SUM-105", new CardsMEG.WeakenDefendingMon("Ferocious Bellow", 50));
            r.RegisterByKey("SUM-106", new FlipBonusMon("Rock Smash", 10));
            r.RegisterByKey("SUM-107", new CoinsMon("Bullet Seed", 4));
            r.RegisterByKey("SUM-108", new ComboMon(new CardsASC.NextTurnBoostMon("Echoed Voice", 60), new CardsPAR2.FlipConditionMon("Beak Blast", SpecialCondition.Burned)));
            r.RegisterByKey("SUM-110", new GumshoosGX());
            r.RegisterByKey("SUM-112", new ComboMon(new NoRetreatMon("Bear Hug"), new OptionalBoostRecoilMon("Superpower", 40, 20)));
            // Trainers
            r.RegisterByName("Big Malasada", new BigMalasada());
            r.RegisterByName("Ilima", new Ilima());
            r.RegisterByName("Poison Barb", new PoisonBarb());
            r.RegisterByName("Professor Kukui", new ProfessorKukui());
            r.RegisterByName("Rotom Dex", new RotomDex());
            r.RegisterByName("Team Skull Grunt", new TeamSkullGrunt());
            r.RegisterByName("Timer Ball", new TimerBall());
        }

        // ================================================================== Colorless

        /// <summary>"Your opponent shuffles their hand into their deck and draws N cards" (Devastating Wind).</summary>
        internal sealed class ShuffleOpponentHandDrawMon : Mon
        {
            private readonly string attack;
            private readonly int cards;

            public ShuffleOpponentHandDrawMon(string attack, int cards)
            {
                this.attack = attack;
                this.cards = cards;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, cards);
                return true;
            }
        }

        /// <summary>"You may do N more damage. If you do, this Pokémon does M damage to itself" (Superpower).</summary>
        internal sealed class OptionalBoostRecoilMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly int recoil;

            public OptionalBoostRecoilMon(string attack, int bonus, int recoil)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool more = ctx.AskYesNo(attack + ": do " + bonus + " more damage? If you do, " + ctx.Pokemon.Name + " does " + recoil + " damage to itself.");
                ctx.DamageDefending(atk.Damage + (more ? bonus : 0));
                if (more && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(recoil);
                return true;
            }
        }

        /// <summary>
        /// Treasure Hunt: when you play this Pokémon from your hand to evolve, you may put an Item card from your discard pile into your
        /// hand (Pokémon Tools aren't Item cards: your ruling, v90).
        /// </summary>
        private sealed class HerdierSUM : Mon
        {
            private static List<CardInstance> Items(EffectContext ctx) { return ctx.Me.Discard.Where(c => CardsLOT4.IsItemCard(c.Def)).ToList(); }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Treasure Hunt", Items(ctx).Count > 0, "Use Treasure Hunt? Put an Item card from your discard pile into your hand.")) return;
                var items = Items(ctx);
                if (items.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(items, 1, 1, "Treasure Hunt: put which Item card into your hand?"));
            }
        }

        /// <summary>
        /// Search the Premises: once during your turn, you may have your opponent reveal their hand. Gumshoe Chance-GX: 50 more for each
        /// Energy attached to the opponent's Active Pokémon.
        /// </summary>
        private sealed class GumshoosGX : Mon
        {
            private static readonly Mon Chance = new CardsASR1.PlusMon("Gumshoe Chance-GX", 50, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending));

            protected override string Activated { get { return "Search the Premises"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx) { CardsBUS4.RevealHand(ctx, "Search the Premises"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Chance.HandleAttack(ctx, atk); }
        }

        // ================================================================== Trainers

        /// <summary>Big Malasada: heal 20 damage and remove a Special Condition (your choice) from your Active Pokémon.</summary>
        private sealed class BigMalasada : CardScript
        {
            private static readonly SpecialCondition[] All =
                { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                if (active.DamageCounters > 0) ctx.Heal(active, 20);
                var has = All.Where(c => active.Has(c)).ToList();
                if (has.Count == 0) return;
                var pick = has.Count == 1 ? has[0] : has[ctx.ChooseOption("Big Malasada: remove which Special Condition from " + active.Name + "?", has.Select(c => c.ToString()).ToArray())];
                ctx.Engine.RemoveCondition(active, pick);
            }
        }

        /// <summary>Ilima: each player shuffles their hand into their deck and flips a coin; heads, that player draws 6 cards, tails 3.</summary>
        private sealed class Ilima : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    ctx.Engine.ShuffleHandIntoDeck(player);
                    bool heads = ctx.Engine.FlipCoin(player, "Ilima");
                    ctx.Engine.DrawCards(player, heads ? 6 : 3);
                }
            }
        }

        /// <summary>Poison Barb: if the holder is your Active Pokémon and is damaged by an opponent's attack (even if Knocked Out), the Attacking Pokémon is now Poisoned.</summary>
        private sealed class PoisonBarb : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || damage <= 0 || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, SpecialCondition.Poisoned)) ctx.Log("Poison Barb poisoned " + attacker.Name + ".");
            }
        }

        /// <summary>Professor Kukui: draw 2 cards; this turn your Pokémon's attacks do 20 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class ProfessorKukui : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                ctx.AddEffect(new DamageBonusEffect("Professor Kukui: your attacks do 20 more damage to the opponent's Active Pokémon this turn", ctx.Player, ctx.Turn, 20, false));
            }
        }

        /// <summary>Rotom Dex: count your Prize cards and shuffle them into your deck, then put that many cards from the top of your deck face down as your Prize cards.</summary>
        private sealed class RotomDex : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Prizes.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                int n = me.Prizes.Count;
                if (n == 0) return;
                me.Deck.AddRange(me.Prizes);
                me.Prizes.Clear();
                me.FaceUpPrizeIds.Clear();
                ctx.ShuffleMyDeck();
                for (int k = 0; k < n && me.Deck.Count > 0; k++)
                {
                    var card = me.Deck[0];
                    me.Deck.RemoveAt(0);
                    me.Prizes.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + n + " Prize card" + (n == 1 ? "" : "s") + " into " + ctx.Engine.Their(ctx.Player) + " deck and put " + me.Prizes.Count + " new ones face down (Rotom Dex).");
            }
        }

        /// <summary>Team Skull Grunt: your opponent reveals their hand; discard 2 Energy cards from it (you choose; all of them if fewer).</summary>
        private sealed class TeamSkullGrunt : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Opp.Hand.Count == 0) return;
                CardsBUS4.RevealHand(ctx, "Team Skull Grunt");
                var energy = ctx.Opp.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return;
                var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Team Skull Grunt: discard 2 Energy cards from your opponent's hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            }
        }

        /// <summary>Timer Ball: flip 2 coins; for each heads, search your deck for an Evolution Pokémon and put it into your hand.</summary>
        private sealed class TimerBall : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 0 || ctx.Me.Deck.Count == 0) return;
                ctx.SearchToHand(d => d.IsEvolution, heads, "Timer Ball: search your deck for up to " + heads + " Evolution Pokémon");
            }
        }
    }
}
