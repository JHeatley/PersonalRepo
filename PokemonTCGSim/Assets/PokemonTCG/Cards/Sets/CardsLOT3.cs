using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Lost Thunder (LOT, v81), part 3: Lightning and Psychic (LOT 71-108).</summary>
    public static class CardsLOT3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("LOT-71", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("LOT-72", new ComboMon(new CardsMEG.ChargeSelfMon("Electrocharge", EnergyType.Lightning, 2), new CardsTWM1.SelfDiscardMon("High-Voltage Knuckle", 3)));
            r.RegisterByKey("LOT-73", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("LOT-74", new ComboMon(new SalvageMon(), new ConditionAttackMon("Signal Beam", SpecialCondition.Confused)));
            r.RegisterByKey("LOT-75", new MareepLOT());
            r.RegisterByKey("LOT-76", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("LOT-77", new ConditionAttackMon("Signal Beam", SpecialCondition.Confused));
            r.RegisterByKey("LOT-78", new AmpharosLOT());
            r.RegisterByKey("LOT-79", new CardsFST2.ConditionBonusMon("Lost Voltage", 90, c => c.Me.LostZone.Any(x => CardsTEU1.BasicOf(x.Def, EnergyType.Lightning))));
            r.RegisterByKey("LOT-80", new OvershortMon());
            r.RegisterByKey("LOT-82", new ZebstrikaLOT());
            r.RegisterByKey("LOT-83", new ComboMon(new CardsASR1.TimesMon("Flail", c => c.Pokemon == null ? 0 : c.Pokemon.DamageCounters), new ThunderBlastMon()));
            r.RegisterByKey("LOT-84", new ComboMon(new ParalyzeFlipMon("Nuzzle"), new NuzzleShotMon()));
            r.RegisterByKey("LOT-85", new ComboMon(new SwitchSelfMon("Strafe", true), new LockAfterAttackMon("Flashing Bolt", true)));
            r.RegisterByKey("LOT-86", new ZeraoraGX());
            // Psychic
            r.RegisterByKey("LOT-87", new CardsASR1.TimesMon("Lost March", CardsLOT1.LostMarchCount));
            r.RegisterByKey("LOT-88", new EnergyGazeMon());
            r.RegisterByKey("LOT-89", new ComboMon(new DrawAttackMon("Allure", 3), new CardsASR1.PlusMon("Energy Crush", 20, c => c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("LOT-90", new UnownLOT("DAMAGE"));
            r.RegisterByKey("LOT-91", new UnownLOT("HAND"));
            r.RegisterByKey("LOT-92", new UnownLOT("MISSING"));
            r.RegisterByKey("LOT-93", new WobbuffetLOT());
            r.RegisterByKey("LOT-94", new ComboMon(new GetLostMon(), new CardsLOT1.PlainDamageMon("Mind Shock", false)));
            r.RegisterByKey("LOT-95", new ShedinjaLOT());
            r.RegisterByKey("LOT-96", new SableyeLOT());
            r.RegisterByKey("LOT-97", new GiratinaLOT());
            r.RegisterByKey("LOT-98", new SigilyphGX());
            r.RegisterByKey("LOT-99", new CardsBKP2.CountersOnActiveMon("Haunt", 1));
            r.RegisterByKey("LOT-100", new SpiritJugglingMon());
            r.RegisterByKey("LOT-101", new CardsBKP2.CountersOnActiveMon("Haunt", 1));
            r.RegisterByKey("LOT-102", new CardsBKP2.CountersOnActiveMon("Haunt", 3));
            r.RegisterByKey("LOT-103", new ComboMon(new CardsLOR1.CountersAnyWayMon("Cursed Drop", 4), new CardsASR1.TimesMon("Vortex of Pain", c => c.OpponentPokemon().Sum(m => m.DamageCounters))));
            r.RegisterByKey("LOT-104", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new MiracleHarmonyMon()));
            r.RegisterByKey("LOT-105", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("LOT-106", new NihilegoLOT());
            r.RegisterByKey("LOT-107", new EyeOpenerMon());
            r.RegisterByKey("LOT-108", new NaganadelLOT());
        }

        // ================================================================== Lightning

        /// <summary>Salvage: shuffle 4 Item cards from your discard pile into your deck (as many as there are, if fewer).</summary>
        private sealed class SalvageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Salvage") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList(); // not Pokémon Tools (your ruling, v90)
                int n = Math.Min(4, items.Count);
                if (n == 0) return true;
                var picks = items.Count <= 4 ? items : ctx.ChooseCards(items, n, n, "Salvage: choose 4 Item cards to shuffle into your deck");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Fluffy Pillow: once during your turn, if this is your Active Pokémon, you may make the opponent's Active Pokémon Asleep.</summary>
        private sealed class MareepLOT : Mon
        {
            protected override string Activated { get { return "Fluffy Pillow"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.Active != null; }
            protected override void Ability(EffectContext ctx) { ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Asleep); }
        }

        /// <summary>
        /// Unseen Flash: once during your turn, you may put 2 Lightning Energy cards from your hand in the Lost Zone; if you do, the
        /// opponent's Active Pokémon is Paralyzed. Split Bomb: 50 damage to 2 of the opponent's Pokémon.
        /// </summary>
        private sealed class AmpharosLOT : Mon
        {
            protected override string Activated { get { return "Unseen Flash"; } }

            private static List<CardInstance> Lightning(EffectContext ctx) { return ctx.Me.Hand.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Lightning)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && Lightning(ctx).Count >= 2; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(Lightning(ctx), 2, 2, "Unseen Flash: put 2 Lightning Energy cards from your hand in the Lost Zone");
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) CardsCRZ1.PutInLostZone(ctx, card);
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Paralyzed);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Split Bomb") return false;
                CardsDRI1.DamageTwoOpponents(ctx, 50, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Overshort: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon; if any were discarded, 40
        /// more damage and the opponent's Active Pokémon is Paralyzed.
        /// </summary>
        private sealed class OvershortMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overshort") return false;
                var defending = ctx.Defending;
                int discarded = 0;
                if (defending != null && defending.Tools.Count > 0 && ctx.CanAffect(defending)) discarded = ctx.Engine.DiscardAllTools(defending);
                ctx.DamageDefending(atk.Damage + (discarded > 0 ? 40 : 0));
                if (discarded > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Sprint: once during your turn, you may discard your hand and draw 4 cards.</summary>
        private sealed class ZebstrikaLOT : Mon
        {
            protected override string Activated { get { return "Sprint"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count > 0) ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(4);
            }
        }

        /// <summary>Thunder Blast: discard a Lightning Energy from this Pokémon.</summary>
        private sealed class ThunderBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Blast") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 1, "Thunder Blast: discard a Lightning Energy from " + me.Name, c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Lightning));
                return true;
            }
        }

        /// <summary>Nuzzle Shot: 10 damage for each of your Pokémon in play with the Nuzzle attack, to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class NuzzleShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nuzzle Shot") return false;
                int n = ctx.MyPokemon(m => m.Def.Attacks.Any(a => a.Name == "Nuzzle")).Count;
                if (n > 0 && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10 * n);
                return true;
            }
        }

        /// <summary>
        /// Thunderclap Zone: each of your Pokémon with any Lightning Energy attached has no Retreat Cost. Plasma Fists: can't attack
        /// next turn. Full Voltage-GX: attach 5 basic Energy cards from your discard pile to your Pokémon in any way you like.
        /// </summary>
        private sealed class ZeraoraGX : Mon
        {
            private static readonly Mon Fists = new LockAfterAttackMon("Plasma Fists", false);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return cost;
                return src.Engine.HasEnergyOfType(mon, EnergyType.Lightning) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Fists.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Full Voltage-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, 5, (c, m) => true, "Full Voltage-GX: attach basic Energy from your discard pile");
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Energy Gaze: your opponent reveals their hand; if there's any Energy card there, 60 more damage.</summary>
        private sealed class EnergyGazeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Gaze") return false;
                bool energy = ctx.Opp.Hand.Any(c => c.Def.IsEnergy);
                if (ctx.Opp.Hand.Count > 0) TefPokemon.RevealOpponentHand(ctx);
                ctx.DamageDefending(atk.Damage + (energy ? 60 : 0));
                return true;
            }
        }

        /// <summary>
        /// Unown's win Abilities (once during your turn, while it's your Active Pokémon): DAMAGE (66 or more damage counters on your
        /// Benched Pokémon), HAND (35 or more cards in your hand), MISSING (12 or more Supporter cards in the opponent's Lost Zone).
        /// </summary>
        private sealed class UnownLOT : Mon
        {
            private readonly string ability;

            public UnownLOT(string ability) { this.ability = ability; }

            protected override string Activated { get { return ability; } }

            private bool Met(EffectContext ctx)
            {
                if (ability == "DAMAGE") return ctx.Me.Bench.Sum(m => m.DamageCounters) >= 66;
                if (ability == "HAND") return ctx.Me.Hand.Count >= 35;
                return ctx.Opp.LostZone.Count(c => c.Def.IsSupporter) >= 12;
            }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && Met(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.EndGame(ctx.Player, "Unown's " + ability + " Ability won the game.");
            }
        }

        /// <summary>Shady Tail: while this Pokémon is on your Bench, Prism Star (◇) Pokémon in play (both players') can't attack and have no Abilities. Knock Away: flip, 30 more.</summary>
        private sealed class WobbuffetLOT : Mon
        {
            private static readonly Mon Knock = new FlipBonusMon("Knock Away", 30);

            public override bool IsAbilityBlocker { get { return true; } }

            private static bool OnBench(EffectContext src) { return src.Pokemon != null && src.InPlay(src.Pokemon) && !src.IsActive(src.Pokemon); }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return mon != null && mon.Def.IsPrismStar && OnBench(src);
            }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Def.IsPrismStar && OnBench(src);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Knock.HandleAttack(ctx, atk); }
        }

        /// <summary>Get Lost: put 2 cards from your opponent's discard pile in the Lost Zone.</summary>
        private sealed class GetLostMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Get Lost") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Opp.Discard.ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(2, pile.Count);
                var picks = pile.Count <= 2 ? pile : ctx.ChooseCards(pile, n, n, "Get Lost: choose 2 cards from your opponent's discard pile to put in the Lost Zone");
                foreach (var card in picks) CardsCRZ1.DiscardToLostZone(ctx, card);
                return true;
            }
        }

        /// <summary>
        /// Vessel of Life: once during your turn, discard all cards attached to this Pokémon (and any card under it) and attach it to 1
        /// of your other Pokémon as a Pokémon Tool. When that Pokémon is Knocked Out, the opponent takes 1 fewer Prize card. Haunt:
        /// 3 damage counters on the opponent's Active Pokémon.
        /// </summary>
        private sealed class ShedinjaLOT : Mon
        {
            private static readonly Mon Haunt = new CardsBKP2.CountersOnActiveMon("Haunt", 3);

            protected override string Activated { get { return "Vessel of Life"; } }

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && ctx.Engine.CanTakeTool(m));
            }

            protected override bool AbilityReady(EffectContext ctx) { return Holders(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var holder = ctx.ChoosePokemon(Holders(ctx), "Vessel of Life: attach " + me.Name + " to which of your Pokémon as a Pokémon Tool?");
                if (holder == null || !ctx.InPlay(me)) return;
                var card = me.Top;
                var under = me.Stack.Where(c => c != card).ToList();
                var attached = new List<CardInstance>(me.Energy);
                attached.AddRange(me.Tools);
                ctx.Engine.RemoveFromPlay(me);
                ctx.Engine.DiscardFromPlay(attached);
                ctx.Engine.DiscardFromPlay(under);
                ctx.Engine.AttachTool(card, holder);
            }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var holder = src.Pokemon;
                if (holder == null || knockedOut != holder || src.Card == null || !holder.Tools.Contains(src.Card)) return prizes;
                return Math.Max(0, prizes - 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Haunt.HandleAttack(ctx, atk); }
        }

        /// <summary>Quick Hunt: usable on your first turn if you go first; search your deck for a card. Cursed Drop: 3 damage counters on the opponent's Pokémon in any way.</summary>
        private sealed class SableyeLOT : Mon
        {
            private static readonly Mon Hunt = new CardsLOT2.AnyCardSearchMon("Quick Hunt");
            private static readonly Mon Drop = new CardsLOR1.CountersAnyWayMon("Cursed Drop", 3);

            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Quick Hunt"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hunt.HandleAttack(ctx, atk) || Drop.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Distortion Door: once during your turn, if this Pokémon is in your discard pile, you may put it onto your Bench; if you do,
        /// 1 damage counter on 2 of the opponent's Benched Pokémon. Shadow Impact: 4 damage counters on 1 of your Pokémon.
        /// </summary>
        private sealed class GiratinaLOT : Mon
        {
            private const string Door = "Distortion Door";

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.BenchSpace > 0
                       && !ctx.NamedLimitUsedThisTurn(Door + "#" + ctx.Card.Id) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                ctx.MarkNamedLimit(Door + "#" + ctx.Card.Id);
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Giratina used its Ability Distortion Door.", ctx.Card.Id, mon.Uid);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return;
                int n = Math.Min(2, bench.Count);
                var targets = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, n, n, "Distortion Door: put 1 damage counter on 2 of your opponent's Benched Pokémon");
                foreach (var t in targets) ctx.PlaceCounters(t, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Impact") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Shadow Impact: put 4 damage counters on which of your Pokémon?");
                ctx.Engine.PlaceDamageCounters(target, 4, "Shadow Impact");
                return true;
            }
        }

        /// <summary>
        /// Mirror Counter: if this is your Active Pokémon and an opponent's Pokémon-GX or Pokémon-EX damages it with an attack (even if
        /// it's Knocked Out), put damage counters equal to that damage on the Attacking Pokémon. Sonic Wing: no Resistance.
        /// Intercept-GX: 60 times the Energy attached to the opponent's Active Pokémon.
        /// </summary>
        private sealed class SigilyphGX : Mon
        {
            private static readonly Mon Wing = new CardsLOT1.PlainDamageMon("Sonic Wing", true);
            private static readonly Mon Intercept = new CardsASR1.TimesMon("Intercept-GX", CardsLOT2.DefendingEnergy);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || !attacker.Def.IsGXorEX) return;
                int counters = damage / 10;
                if (counters <= 0) return;
                Announce(ctx, "Mirror Counter");
                ctx.Engine.PlaceDamageCounters(attacker, counters, "Mirror Counter");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wing.HandleAttack(ctx, atk) || Intercept.HandleAttack(ctx, atk); }
        }

        /// <summary>Spirit Juggling: discard any number of your Benched Pokémon; 30 more damage for each.</summary>
        private sealed class SpiritJugglingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirit Juggling") return false;
                int n = 0;
                var bench = ctx.MyBench();
                if (bench.Count > 0)
                {
                    var picks = ctx.ChoosePokemonMany(bench, 0, bench.Count, "Spirit Juggling: discard any number of your Benched Pokémon (30 more damage each)");
                    foreach (var mon in picks)
                    {
                        if (!ctx.InPlay(mon)) continue;
                        ctx.Engine.DiscardPokemonFromPlay(mon);
                        n++;
                    }
                }
                ctx.DamageDefending(atk.Damage + 30 * n);
                return true;
            }
        }

        /// <summary>Miracle Harmony: flip a coin for each of your Pokémon in play with the Sing attack; 10 damage for each heads to each of the opponent's Pokémon.</summary>
        private sealed class MiracleHarmonyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miracle Harmony") return false;
                int singers = ctx.MyPokemon(m => m.Def.Attacks.Any(a => a.Name == "Sing")).Count;
                int heads = singers > 0 ? ctx.FlipCoins(singers) : 0;
                if (heads == 0)
                {
                    ctx.Log("Miracle Harmony did no damage.");
                    return true;
                }
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, 10 * heads);
                return true;
            }
        }

        /// <summary>
        /// Nightcap: only if the opponent has exactly 2 Prize cards left; choose 1 of the opponent's Pokémon's attacks and use it as
        /// this attack. Void Tentacles: Confused and Poisoned.
        /// </summary>
        private sealed class NihilegoLOT : Mon
        {
            private static readonly Mon Tentacles = new CardsCES1.ConditionsMon("Void Tentacles", SpecialCondition.Confused, SpecialCondition.Poisoned);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Nightcap") return true;
                return ctx.Opp.Prizes.Count == 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Tentacles.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Nightcap") return false;
                var mons = ctx.OpponentPokemon(m => m.Def.Attacks.Count > 0);
                if (mons.Count == 0)
                {
                    ctx.Log("Nightcap: your opponent's Pokémon have no attacks to use.");
                    return true;
                }
                var mon = ctx.ChoosePokemon(mons, "Nightcap: choose 1 of your opponent's Pokémon whose attack to use");
                CardDef source;
                int index;
                if (!ChooseCopiedAttack(ctx, mon, "Nightcap: choose the attack to use", out source, out index)) return true;
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>Eye Opener: look at your face-down Prize cards.</summary>
        private sealed class EyeOpenerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eye Opener") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var p = ctx.Me;
                var faceDown = p.Prizes.Where(c => !p.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (faceDown.Count == 0) return true;
                ctx.Engine.NoteLookedAt(faceDown, "Your face-down Prize cards", ctx.Player);
                ctx.Engine.ShowCards(ctx.Player, faceDown, "Eye Opener", "Your face-down Prize cards", ctx.Source);
                return true;
            }
        }

        /// <summary>Charging Up: once during your turn, attach a basic Energy card from your discard pile to this Pokémon. Turning Point: 80 more with exactly 3 Prize cards left.</summary>
        private sealed class NaganadelLOT : Mon
        {
            private static readonly Mon Turning = new CardsFST2.ConditionBonusMon("Turning Point", 80, c => c.Me.Prizes.Count == 3);

            protected override string Activated { get { return "Charging Up"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var energy = ctx.BasicEnergyInDiscard();
                var card = ctx.ChooseCards(energy, 1, 1, "Charging Up: attach a basic Energy card from your discard pile to " + ctx.Pokemon.Name)[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Turning.HandleAttack(ctx, atk); }
        }
    }
}
