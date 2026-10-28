using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Legendary Treasures (LTR, v108): Lightning to Colorless Pokémon (46-108).</summary>
    public static class CardsLTR2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LTR-46", new ComboMon(new CardsMEG.SnipeMon("Random Spark", 50), new CoinsMon("Thundering Hurricane", 4)));
            r.RegisterByKey("LTR-47", new ComboMon(new TagDrawMon(), new FlipBonusMon("Positive Ion", 10)));
            r.RegisterByKey("LTR-48", new ComboMon(new CardsMEG.WeakenDefendingMon("Negative Ion", 30), new CardsLTR1.SpreadMon("Electrishower", 10)));
            r.RegisterByKey("LTR-49", new CallForFamilyMon());
            r.RegisterByKey("LTR-50", new ComboMon(new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1), new CardsTWM1.SelfDiscardMon("Disaster Volt", 1)));
            r.RegisterByKey("LTR-52", new ComboMon(new FlipBonusMon("Glinting Claw", 30), new CardsTWM1.SelfDiscardMon("Strong Volt", 2)));
            r.RegisterByKey("LTR-53", new CardsTWM1.SelfDiscardMon("Psyslash", 2));
            r.RegisterByKey("LTR-56", new ComboMon(new FortunateDrawMon(), new CardsPAR2.FlipConditionMon("Miracle Wing", SpecialCondition.Confused)));
            r.RegisterByKey("LTR-58", new CardsTWM1.HealSelfMon("Absorb Life", 10));
            r.RegisterByKey("LTR-59", new CardsPAR2.FlipConditionMon("Mind Bend", SpecialCondition.Confused));
            r.RegisterByKey("LTR-60", new CardsPAR2.FlipConditionMon("Mind Bend", SpecialCondition.Confused));
            r.RegisterByKey("LTR-61", new ParalyzeFlipMon("Tight Jaw"));
            r.RegisterByKey("LTR-62", new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned));
            r.RegisterByKey("LTR-63", new ComboMon(new Cards30C.RetaliateBonusMon("Revenge", 70), new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("LTR-64", new CoinsMon("Dual Cut", 2));
            r.RegisterByKey("LTR-65", new EnergyGiftMon());
            r.RegisterByKey("LTR-66", new SafeguardMon(new CardsMEG.DefendingEnergyBonusMon("Psychic", 10)));
            r.RegisterByKey("LTR-67", new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("LTR-68", new CardsBKP2.GarbodorBKP());
            r.RegisterByKey("LTR-69", new ComboMon(new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("LTR-70", new FlipBonusMon("Trip Over", 20));
            r.RegisterByKey("LTR-71", new ComboMon(new CoinsMon("Double Slap", 2), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("LTR-72", new GothitelleMagicRoom());
            r.RegisterByKey("LTR-74", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("LTR-76", new ComboMon(new ReorderTopMon("Future Sight", 5), new CardsASR1.TimesMon("Net Force", c => c.MyPokemon(m => m.Name == "Reuniclus").Count)));
            r.RegisterByKey("LTR-77", new ComboMon(new CardsLOR1.CountersAnyWayMon("Cursed Drop", 4), new Cards30C.BurnAndConfuseMon("Eerie Glow")));
            r.RegisterByKey("LTR-78", new ComboMon(new CardsMEG.DefendingEnergyBonusMon("Psychic", 20), new CardsASC.NextTurnBoostMon("Echoed Voice", 50)));
            r.RegisterByKey("LTR-79", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("LTR-80", new CardsASC.CounterattackMon("Reflexive Retaliation", 2, new CardsMEG.SnipeBenchMon("Aura Sphere", 20)));
            r.RegisterByKey("LTR-81", new ComboMon(new ParalyzeFlipMon("Nerve Shot"), new CardsASR1.PlusMon("Pandemonium Blade", 20, c => c.Me.Bench.Count(m => m.DamageCounters > 0))));
            r.RegisterByKey("LTR-82", new ComboMon(new DigOutAnyMon(), new CardsBUS1.OwnBenchQuakeMon("Break Ground", 10)));
            r.RegisterByKey("LTR-83", new ComboMon(new Cards30CC.BenchHitMon("Muddy Water", 20), new NoRetreatMon("Rumble")));
            r.RegisterByKey("LTR-84", new Cards30C.RetaliateBonusMon("Retaliate", 60));
            r.RegisterByKey("LTR-85", new ComboMon(new AbundantHarvestMon(), new GaiaHammerMon("Gaia Hammer", 10, 10)));
            r.RegisterByKey("LTR-86", new ComboMon(new MagicalDanceMon(), new CardsFCO2.FlipUntilTailsMoreMon("Shooting Star Pirouette", 30)));
            r.RegisterByKey("LTR-87", new SpiritombScream());
            r.RegisterByKey("LTR-88", new DarkraiCloak());
            r.RegisterByKey("LTR-89", new CardsSIT1.AscensionMon());
            r.RegisterByKey("LTR-90", new ComboMon(new CardsASR1.TimesMon("Brutal Bash", c => c.MyPokemon(m => c.Engine.IsType(m, EnergyType.Darkness)).Count),
                new CardsASR1.TimesMon("Dark Rush", c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("LTR-91", new ComboMon(new CardsMEG.DefendingEnergyBonusMon("Energy Press", 20), new CardsEVS1.DefenderCantAttackMon("Iron Breaker", d => true)));
            r.RegisterByKey("LTR-92", new CleaveMon());
            r.RegisterByKey("LTR-93", new ComboMon(new CardsCRZ1.DragonPulseMon(), new CardsPAF2.IgnoreDefenderEffectsMon("Shred")));
            r.RegisterByKey("LTR-95", new CardsPAF2.IgnoreDefenderEffectsMon("Shred"));
            r.RegisterByKey("LTR-96", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Mach Cut", false), new CardsBRS1.MillSelfMon("Dragonblade", 2)));
            r.RegisterByKey("LTR-97", new ComboMon(new ParalyzeFlipMon("Deep Growl"), new CardsTWM1.SelfDiscardMon("Power Breath", 1)));
            r.RegisterByKey("LTR-98", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("LTR-99", new HydreigonTrance());
            r.RegisterByKey("LTR-100", new ComboMon(new ParalyzeFlipMon("Dragon Fang"), new LockAfterAttackMon("Freeze Shock", false)));
            r.RegisterByKey("LTR-101", new ComboMon(new DragonStreamMon(), new IceBurnMon()));
            r.RegisterByKey("LTR-102", new LugiaOverflow());
            r.RegisterByKey("LTR-103", new ParalyzeFlipMon("Glare and Peck"));
            r.RegisterByKey("LTR-104", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("LTR-105", new SmoothCoatMon(new CardsASC.NextTurnBoostMon("Echoed Voice", 50)));
            r.RegisterByKey("LTR-106", new CardsASC.CounterattackMon("Rough Skin", 2, new NoRetreatMon("Clutch")));
            r.RegisterByKey("LTR-107", new ComboMon(new Cards30C.RetaliateBonusMon("Revenge", 70), new CardsFST1.FlipRecoilMon("Head Charge", 20)));
            r.RegisterByKey("LTR-108", new TornadusLTR());
        }

        // ================================================================== Lightning, Psychic

        /// <summary>Tag Draw: shuffle your hand into your deck and draw 4 cards (8 if Minun is on your Bench).</summary>
        private sealed class TagDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tag Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool minun = ctx.Me.Bench.Any(m => m.Name == "Minun");
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(minun ? 8 : 4);
                return true;
            }
        }

        /// <summary>"Look at the top N cards of your deck and put them back on top of your deck in any order" (Future Sight, Calculate).</summary>
        internal sealed class ReorderTopMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public ReorderTopMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) CardsOBF2.ReorderOwnTop(ctx, count, attack);
                return true;
            }
        }

        /// <summary>Fortunate Draw: Rock-Paper-Scissors (your ruling: people choose, bots throw at random); the winner draws 3, the loser discards the top 3 of their deck.</summary>
        private sealed class FortunateDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fortunate Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool won = CardsLOR1.PlayRockPaperScissors(ctx);
                int winner = won ? ctx.Player : ctx.Opponent;
                ctx.Engine.DrawCards(winner, 3);
                if (ctx.State.Players[1 - winner].Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(1 - winner, 3);
                return true;
            }
        }

        /// <summary>Energy Gift: flip a coin; heads, search your deck for up to 2 Psychic Energy cards and attach them to your Pokémon in any way you like.</summary>
        private sealed class EnergyGiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Gift") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 2, "Energy Gift: choose up to 2 Psychic Energy cards");
                foreach (var card in found)
                {
                    var mine = ctx.MyPokemon();
                    var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Energy Gift: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Safeguard: prevent all effects of attacks, damage included, done to this Pokémon by Pokémon-EX, your own too (Victini-EX's
        /// Turbo Energize can choose it, but the Energy is discarded: Compendium 2013-06-20).
        /// </summary>
        internal sealed class SafeguardMon : Mon
        {
            private readonly Mon attacks;

            public SafeguardMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a == src.Pokemon || !a.Def.IsPokemonEX) return damage;
                return 0;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker != src.Pokemon && attacker.Def.IsPokemonEX;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Magic Room: while this Pokémon is Active, the opponent can't play Item cards from their hand. Madkinesis: 20 more per Psychic Energy.</summary>
        private sealed class GothitelleMagicRoom : Mon
        {
            private static readonly Mon Madkinesis = new CardsMEG.EnergyBonusMon("Madkinesis", EnergyType.Psychic, 20);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                return me != null && player != src.Player && card != null && card.Def.IsItem && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Madkinesis.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Dig Out: discard the top card of your deck; if it's a basic Energy card, attach it to 1 of your Pokémon.</summary>
        private sealed class DigOutAnyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Out") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                foreach (var card in ctx.Engine.DiscardTopOfDeck(ctx.Player, 1))
                {
                    if (!card.Def.IsBasicEnergy || !ctx.Me.Discard.Contains(card) || !ctx.Me.HasPokemonInPlay) continue;
                    var mine = ctx.MyPokemon();
                    var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Dig Out: attach " + card.Def.Name + " to which Pokémon?");
                    if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                }
                return true;
            }
        }

        /// <summary>Abundant Harvest: attach a basic Energy card from your discard pile to this Pokémon.</summary>
        private sealed class AbundantHarvestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Abundant Harvest") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.BasicEnergyInDiscard();
                if (!ctx.InPlay(me) || pile.Count == 0) return true;
                var card = pile.Select(c => c.Def.Name).Distinct().Count() == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, "Abundant Harvest: attach which basic Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Gaia Hammer: 80, and 10 damage to each Benched Pokémon (both players'). Used for Gaia Hammer, Aura of the Land, Seismic Punch, Rock Drill Dive (the attack name and the numbers are passed in).</summary>
        internal sealed class GaiaHammerMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly int amount2;

            public GaiaHammerMon(string attack, int amount, int amount2)
            {
                this.attack = attack;
                this.amount = amount;
                this.amount2 = amount2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, amount);
                foreach (var mon in ctx.MyBench()) ctx.DamageTarget(mon, amount2);
                return true;
            }
        }

        /// <summary>Magical Dance: heal 30 damage from each of your Benched Pokémon.</summary>
        private sealed class MagicalDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench()) if (mon.DamageCounters > 0) ctx.Heal(mon, 30);
                return true;
            }
        }

        // ================================================================== Darkness, Metal, Dragon

        /// <summary>
        /// Sealing Scream: neither player can play ACE SPEC cards from their hand; your ruling, ACE SPEC Energy can't be attached from
        /// the hand either. Hexed Mirror: shuffle your hand into your deck, then draw as many cards as your opponent has in their hand.
        /// </summary>
        private sealed class SpiritombScream : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && card != null && card.Def.IsAceSpec;
            }

            public override bool BlocksEnergyCard(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && card != null && card.Def.IsAceSpec;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hexed Mirror") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Opp.Hand.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Dark Cloak: each of your Pokémon with any Darkness Energy attached has no Retreat Cost. Night Spear: 30 to 1 Benched Pokémon.</summary>
        private sealed class DarkraiCloak : Mon
        {
            private static readonly Mon Spear = new CardsMEG.SnipeBenchMon("Night Spear", 30);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return cost;
                return src.Engine.HasEnergyOfType(mon, EnergyType.Darkness) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spear.HandleAttack(ctx, atk); }
        }

        /// <summary>Cleave: flip 2 coins; if both are heads, discard all Energy attached to the Defending Pokémon.</summary>
        internal sealed class CleaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cleave") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, defending.Energy.Count, atk.Name + ": discard all Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Dark Trance: as often as you like during your turn, move a Darkness Energy from 1 of your Pokémon to another. Dragonblast: discard 2 Darkness Energy.</summary>
        private sealed class HydreigonTrance : Mon
        {
            private const string Trance = "Dark Trance";
            private static readonly Mon Blast = new CardsASC.SelfDiscardTypeMon("Dragonblast", EnergyType.Darkness, 2);

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Darkness)));
            }

            private static bool CanMove(EffectContext ctx) { return ctx.MyPokemon().Count > 1 && Holders(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Trance && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Trance + ": move a Darkness Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (giver == null) return false;
                var dark = giver.Energy.Where(e => CardsDRI1.ProvidesType(ctx, giver, e, EnergyType.Darkness)).ToList();
                if (dark.Count == 0) return false;
                var card = dark.Select(e => e.Def.Name).Distinct().Count() == 1 ? dark[0] : ctx.ChooseCards(dark, 1, 1, Trance + ": move which Energy?")[0];
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != giver), Trance + ": move it from " + giver.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveEnergy(giver, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Dragon Stream: flip a coin; heads, attach a basic Energy card from your discard pile to this Pokémon.</summary>
        private sealed class DragonStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Stream") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || !ctx.InPlay(me)) return true;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return true;
                var card = pile.Select(c => c.Def.Name).Distinct().Count() == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, "Dragon Stream: attach which basic Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Ice Burn: 150, discard 2 Fire Energy from this Pokémon; the Defending Pokémon is now Burned.</summary>
        private sealed class IceBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Burn") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 2, "Ice Burn: discard 2 Fire Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Overflow: when the opponent's Pokémon is Knocked Out by damage from this Pokémon's attack, take 1 more Prize card. Plasma Gale:
        /// discard a Plasma Energy from this Pokémon; if you can't, the attack does nothing.
        /// </summary>
        private sealed class LugiaOverflow : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var me = src.Pokemon;
                if (me == null || knockedOut == null || taker != src.Player || knockedOut.Owner == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.HitByPlayer != src.Player || knockedOut.HitByAttackerUid != me.Uid) return prizes;
                return prizes + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plasma Gale") return false;
                var me = ctx.Pokemon;
                var plasma = me.Energy.FirstOrDefault(e => e.Def.Name == "Plasma Energy");
                if (plasma == null || !ctx.Engine.DiscardEnergy(me, plasma))
                {
                    ctx.Log("Plasma Gale: no Plasma Energy to discard, so the attack does nothing.");
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Smooth Coat: whenever an attack would damage this Pokémon, flip a coin; heads, prevent that damage.</summary>
        internal sealed class SmoothCoatMon : Mon
        {
            private readonly Mon attacks;

            public SmoothCoatMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null && damage > 0 && src.Engine.FlipCoin(src.Player, "Smooth Coat");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Wheel: move an Energy from 1 of your Benched Pokémon to this Pokémon. Hurricane: move a basic Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class TornadusLTR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Energy Wheel")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var givers = ctx.MyBench().Where(m => m.Energy.Count > 0).ToList();
                    if (!ctx.InPlay(me) || givers.Count == 0) return true;
                    var giver = ctx.ChoosePokemon(givers, "Energy Wheel: move an Energy from which Benched Pokémon?");
                    if (giver == null) return true;
                    var card = giver.Energy.Count == 1 ? giver.Energy[0] : ctx.ChooseCards(giver.Energy.ToList(), 1, 1, "Energy Wheel: move which Energy to " + me.Name + "?")[0];
                    ctx.Engine.MoveEnergy(giver, me, card);
                    return true;
                }
                if (atk.Name == "Hurricane")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                    var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                    if (basics.Count == 0) return true;
                    var card = basics.Select(e => e.Def.Name).Distinct().Count() == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Hurricane: move which basic Energy?")[0];
                    var to = ctx.ChoosePokemon(ctx.MyBench(), "Hurricane: move " + card.Def.Name + " to which Benched Pokémon?");
                    if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                    return true;
                }
                return false;
            }
        }
    }
}
