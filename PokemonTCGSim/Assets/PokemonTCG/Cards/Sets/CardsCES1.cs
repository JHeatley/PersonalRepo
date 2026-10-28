using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Celestial Storm (CES, v83), part 1: Grass, Fire and Water (CES 1-46). No regulation mark: Expanded. Scyther and Articuno-GX
    /// were already in the pool (Hidden Fates Shiny Vault) and keep their scripts. "A [T] Energy card" outside play is basic Energy
    /// only (your ruling, v65). Printed damage plus "does N damage to 1 of your opponent's Benched Pokémon" hits the Active
    /// Pokémon too (your ruling, v83: Marshtomp's Muddy Water).
    /// </summary>
    public static class CardsCES1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("CES-1", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("CES-2", new ComboMon(new AttachFromHandToSelfMon("Growth", EnergyType.Grass, 2), new CoinsMon("Double Razor Leaf", 2)));
            r.RegisterByKey("CES-3", new VictreebelCES());
            r.RegisterByKey("CES-5", new FlipConditionsMon("Stun Poison", SpecialCondition.Paralyzed, SpecialCondition.Poisoned));
            r.RegisterByKey("CES-6", new ComboMon(new CardsASR1.PlusMon("Reactive Poison", 50, CardsASR1.DefendingConditions), new SpiderTrapMon()));
            r.RegisterByKey("CES-7", new ConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned));
            r.RegisterByKey("CES-9", new FlipBonusMon("Leaf Blade", 40));
            r.RegisterByKey("CES-10", new SceptileCES());
            r.RegisterByKey("CES-11", new CardsLOR1.HardenMon("Harden", 40));
            r.RegisterByKey("CES-13", new ClearTheRoomMon());
            r.RegisterByKey("CES-14", new ComboMon(new ConditionAttackMon("Perplex", SpecialCondition.Confused),
                new CardsFST2.ConditionBonusMon("Extrasensory", 90, c => c.Me.Hand.Count == c.Opp.Hand.Count), new DenOfIniquityMon()));
            r.RegisterByKey("CES-15", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("CES-16", new ComboMon(new SurprisingPatternMon(), new CoinsMon("Hurricane Wing", 4)));
            r.RegisterByKey("CES-17", new CardsFST2.ConditionBonusMon("Pheromone Catch", 100, IllumiseSignaledLastTurn));
            r.RegisterByKey("CES-18", new ConditionAttackMon("Pheromone Signals", SpecialCondition.Confused));
            r.RegisterByKey("CES-19", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, null));
            r.RegisterByKey("CES-20", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, new FeintAttackMon("Feint Attack", 50)));
            r.RegisterByKey("CES-21", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 2));
            r.RegisterByKey("CES-22", new ComboMon(new CardsDAA1.DrainMon("Giga Drain"), new LockAfterAttackMon("Powerful Spin", false)));
            // Fire
            r.RegisterByKey("CES-23", new NoRetreatMon("Magma Ring"));
            r.RegisterByKey("CES-24", new MagcargoCES());
            r.RegisterByKey("CES-25", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("CES-27", new ComboMon(new CoinsMon("Double Kick", 2), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("CES-28", new ComboMon(new CardsASC.SelfDiscardTypeMon("Explosive Kick", EnergyType.Fire, 2), new BlazeOutMon()));
            r.RegisterByKey("CES-29", new ComboMon(new FlamingHonkMon(), new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)));
            r.RegisterByKey("CES-30", new CaptivatingSalsaMon());
            // Water
            r.RegisterByKey("CES-32", new SearchAttackMon("Water Reserve", d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 3));
            r.RegisterByKey("CES-34", new Cards30CC.BenchHitMon("Muddy Water", 20));
            r.RegisterByKey("CES-35", new SwampertCES());
            r.RegisterByKey("CES-36", new SurpriseMon());
            r.RegisterByKey("CES-37", new CardsSVP.FloatMon(EnergyType.Water, new FlipBonusMon("Ambush", 20)));
            r.RegisterByKey("CES-38", new LudicoloCES());
            r.RegisterByKey("CES-39", new WaveSwallowerMon());
            r.RegisterByKey("CES-40", new DwindlingWaveMon());
            r.RegisterByKey("CES-41", new CardsTEU3.EvolutionaryAdvantageMon());
            r.RegisterByKey("CES-42", new ComboMon(new NoRetreatMon("Big Bite"),
                new CardsFST2.ConditionBonusMon("Dangerous Bite", 80, c => c.Defending != null && c.Defending.Def.IsBasicPokemon)));
            r.RegisterByKey("CES-43", new CardsASR1.FrostWallMon("Deflecting Splash", 9999));
            r.RegisterByKey("CES-44", new ComboMon(new EvenGameMon(), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("CES-45", new RegiceCES());
            r.RegisterByKey("CES-46", new ComboMon(new TwoTargetsMon("Dual Splash", 30), new LockAfterAttackMon("Grand Wave", true)));
        }

        // ================================================================== Shared helpers

        /// <summary>"If your Illumise used Pheromone Signals during your last turn" (Volbeat). The game record counts even if it's gone now (Compendium, 2018-08-02).</summary>
        internal static bool IllumiseSignaledLastTurn(EffectContext ctx)
        {
            var me = ctx.Me;
            return me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackName == "Pheromone Signals" && me.LastAttackerDef != null && me.LastAttackerDef.Name == "Illumise";
        }

        /// <summary>"Your opponent's Active Pokémon is now [A] and [B]" after the damage (Sleep Poison).</summary>
        internal sealed class ConditionsMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition[] conditions;

            public ConditionsMon(string attack, params SpecialCondition[] conditions)
            {
                this.attack = attack;
                this.conditions = conditions;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                foreach (var c in conditions) ctx.ApplyCondition(defending, c);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, your opponent's Active Pokémon is now [A] and [B]" (Stun Poison).</summary>
        internal sealed class FlipConditionsMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition[] conditions;

            public FlipConditionsMon(string attack, params SpecialCondition[] conditions)
            {
                this.attack = attack;
                this.conditions = conditions;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                foreach (var c in conditions) ctx.ApplyCondition(defending, c);
                return true;
            }
        }

        /// <summary>"Attach up to N [T] Energy cards from your hand to this Pokémon" (Growth). They're basic Energy cards (your ruling, v65).</summary>
        internal sealed class AttachFromHandToSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int max;

            public AttachFromHandToSelfMon(string attack, EnergyType type, int max)
            {
                this.attack = attack;
                this.type = type;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var energy = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, type)).ToList();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, Math.Min(max, energy.Count), (card, mon) => mon == me, attack + ": attach Energy from your hand to " + me.Name);
                return true;
            }
        }

        /// <summary>"This attack does N damage to 2 of your opponent's Pokémon" (Dual Splash). Weakness and Resistance only for the Active Pokémon.</summary>
        internal sealed class TwoTargetsMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public TwoTargetsMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var all = ctx.OpponentPokemon();
                var targets = all.Count <= 2 ? all : ctx.ChoosePokemonMany(all, 2, 2, attack + ": choose 2 of your opponent's Pokémon to do " + amount + " damage to");
                foreach (var t in targets) ctx.DamageTarget(t, amount);
                return true;
            }
        }

        /// <summary>
        /// Feint Attack: N damage to 1 of the opponent's Pokémon, not affected by Weakness, Resistance or any other effects on that
        /// Pokémon. The number is only in the text (no printed damage).
        /// </summary>
        internal sealed class FeintAttackMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FeintAttackMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageAnyOpponent(amount, true, true);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Fragrance Trap: once during your turn, flip a coin; on heads, switch 1 of the opponent's Benched Pokémon with their Active Pokémon. Corrosive Acid: flip for Burned.</summary>
        private sealed class VictreebelCES : Mon
        {
            private static readonly Mon Acid = new CardsPAR2.FlipConditionMon("Corrosive Acid", SpecialCondition.Burned);

            protected override string Activated { get { return "Fragrance Trap"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.GustOpponent("Fragrance Trap: switch which of your opponent's Benched Pokémon into the Active Spot?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Acid.HandleAttack(ctx, atk); }
        }

        /// <summary>Spider Trap: you may switch 1 of the opponent's Benched Pokémon with their Active Pokémon; their Active Pokémon is now Asleep and Poisoned.</summary>
        private sealed class SpiderTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spider Trap") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Active != null && ctx.GustTargets().Count > 0 && ctx.AskYesNo("Spider Trap: switch 1 of your opponent's Benched Pokémon with their Active Pokémon?"))
                    ctx.GustOpponent("Spider Trap: switch which of your opponent's Benched Pokémon into the Active Spot?");
                var active = ctx.Opp.Active;
                if (active == null) return true;
                ctx.ApplyCondition(active, SpecialCondition.Asleep);
                ctx.ApplyCondition(active, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>
        /// Power of Nature: prevent all damage done to your Pokémon that have any Grass Energy attached by attacks from the opponent's
        /// Ultra Beasts. Powerful Storm: 20 damage for each Energy attached to all of your Pokémon.
        /// </summary>
        private sealed class SceptileCES : Mon
        {
            private static readonly Mon Storm = new CardsASR1.TimesMon("Powerful Storm", c => c.MyPokemon().Sum(m => c.EnergyCount(m)));

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                var a = info.Attacker;
                if (target == null || target.Owner != src.Player || a == null || a.Owner == src.Player || !a.Def.IsUltraBeast) return false;
                return src.Engine.HasEnergyOfType(target, EnergyType.Grass);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Storm.HandleAttack(ctx, atk); }
        }

        /// <summary>Clear the Room: the opponent reveals their hand; you choose a Supporter card there and they shuffle it into their deck.</summary>
        private sealed class ClearTheRoomMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clear the Room") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                ctx.Engine.NoteLookedAt(hand, "Your opponent's hand", ctx.Player);
                var supporters = hand.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0)
                {
                    ctx.ChooseCards(supporters, 0, 1, "Clear the Room: there's no Supporter card in your opponent's hand");
                    return true;
                }
                var pick = ctx.ChooseCards(supporters, 1, 1, "Clear the Room: choose a Supporter card for your opponent to shuffle into their deck");
                if (pick.Count == 0 || !ctx.Opp.Hand.Remove(pick[0])) return true;
                ctx.Opp.Deck.Add(pick[0]);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + pick[0].Def.Name + " into their deck (Clear the Room).");
                return true;
            }
        }

        /// <summary>Den of Iniquity-GX: choose 1 of the opponent's Pokémon; they shuffle it and all attached cards into their deck.</summary>
        private sealed class DenOfIniquityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Den of Iniquity-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Den of Iniquity-GX: your opponent shuffles which of their Pokémon (and all attached cards) into their deck?");
                if (target != null && ctx.InPlay(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Surprising Pattern: discard all Special Energy from each of the opponent's Pokémon.</summary>
        private sealed class SurprisingPatternMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprising Pattern") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => ctx.CanAffect(m)))
                    foreach (var e in mon.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList())
                        ctx.Engine.DiscardEnergy(mon, e);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Smooth Over: once during your turn, search your deck for a card, shuffle your deck, then put that card on top of it.</summary>
        private sealed class MagcargoCES : Mon
        {
            protected override string Activated { get { return "Smooth Over"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => true, 1, "Smooth Over: choose a card to put on top of your deck (after shuffling)", false, 1);
                ctx.ShuffleMyDeck();
                if (found.Count == 0) return;
                ctx.Me.Deck.Insert(0, found[0]);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + ctx.Engine.Their(ctx.Player) + " deck and put a card on top of it (Smooth Over).");
            }
        }

        /// <summary>Blaze Out-GX: discard 2 Energy from the opponent's Pokémon (each card chosen counts as 1, so 2 Double Colorless Energy can go: Compendium, 2018-08-02).</summary>
        private sealed class BlazeOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze Out-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 2; i++)
                {
                    var holders = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                    if (holders.Count == 0) return true;
                    var mon = ctx.ChoosePokemon(holders, "Blaze Out-GX: discard an Energy from which of your opponent's Pokémon? (" + (i + 1) + " of 2)");
                    if (mon == null) return true;
                    ctx.DiscardEnergyFrom(mon, 1, "Blaze Out-GX: discard which Energy from " + mon.Name + "?");
                }
                return true;
            }
        }

        /// <summary>Flaming Honk: discard the top 4 cards of your deck; attach any Fire Energy cards among them to your Pokémon in any way you like.</summary>
        private sealed class FlamingHonkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flaming Honk") return false;
                ctx.DamageDefending(atk.Damage);
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 4);
                var fire = milled.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire) && ctx.Me.Discard.Contains(c)).ToList();
                if (fire.Count > 0 && ctx.Me.HasPokemonInPlay)
                    ctx.AttachCardsAnyWay(fire, fire.Count, (card, mon) => true, "Flaming Honk: attach the Fire Energy", false);
                return true;
            }
        }

        /// <summary>Captivating Salsa: switch 1 of the opponent's Benched Pokémon with their Active Pokémon; the new Active Pokémon is now Burned and Confused.</summary>
        private sealed class CaptivatingSalsaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Captivating Salsa") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Active == null || ctx.GustTargets().Count == 0) return true;
                var now = ctx.GustOpponent("Captivating Salsa: switch which of your opponent's Benched Pokémon into the Active Spot?");
                if (now == null || ctx.Opp.Active != now) return true;
                ctx.ApplyCondition(now, SpecialCondition.Burned);
                ctx.ApplyCondition(now, SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Power Draw: once during your turn, you may discard a card from your hand; if you do, draw 3 cards. The old wording makes the
        /// discard a first step, so it works with an empty deck (Compendium, 2019-03-21). Hydro Pump: 20 more for each Water Energy.
        /// </summary>
        private sealed class SwampertCES : Mon
        {
            private static readonly Mon Pump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20);

            protected override string Activated { get { return "Power Draw"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "Power Draw: discard a card from your hand (then draw 3 cards)");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Surprise: choose a random card from the opponent's hand; they reveal it and shuffle it into their deck.</summary>
        private sealed class SurpriseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { card }, "revealed (Surprise) and shuffled into their deck");
                hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Swing Dance: once during your turn, draw a card. Circular Steps: 10 more for each of your other Pokémon and each of the opponent's Pokémon.</summary>
        private sealed class LudicoloCES : Mon
        {
            protected override string Activated { get { return "Swing Dance"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Circular Steps") return false;
                int others = ctx.MyPokemon(m => m != ctx.Pokemon).Count + ctx.OpponentPokemon().Count;
                ctx.DamageDefending(atk.Damage + 10 * others);
                return true;
            }
        }

        /// <summary>Wave Swallower: flip a coin until you get tails; heal 50 damage from this Pokémon for each heads.</summary>
        private sealed class WaveSwallowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wave Swallower") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 50 * heads);
                return true;
            }
        }

        /// <summary>Dwindling Wave: 200 damage, 40 less for each damage counter on this Pokémon.</summary>
        private sealed class DwindlingWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dwindling Wave") return false;
                int dmg = Math.Max(0, atk.Damage - 40 * ctx.Pokemon.DamageCounters);
                if (dmg > 0) ctx.DamageDefending(dmg);
                else ctx.Log("Dwindling Wave did no damage.");
                return true;
            }
        }

        /// <summary>Even Game: put Basic Pokémon from your deck onto your Bench, up to the number of the opponent's Benched Pokémon.</summary>
        private sealed class EvenGameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Even Game") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Opp.Bench.Count;
                if (n > 0) ctx.SearchToBench(d => true, n, "Even Game: choose up to " + n + " Basic Pokémon to put onto your Bench");
                return true;
            }
        }

        /// <summary>Icy Barrier: while this Pokémon is your Active Pokémon, your opponent can't play Stadium cards from their hand. Icy Wind: Asleep.</summary>
        private sealed class RegiceCES : Mon
        {
            private static readonly Mon Wind = new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return player != src.Player && card != null && card.Def.IsStadium && src.Pokemon != null && src.IsActive(src.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wind.HandleAttack(ctx, atk); }
        }
    }
}
