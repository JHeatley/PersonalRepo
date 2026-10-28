using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Surging Sparks (SSP) 125-191. Pokémon whose attacks only do their printed damage need no script:
    /// Vigoroth, Rufflet.
    /// </summary>
    public static class CardsSSP3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SSP-125", new CardsSCR2.RagingHammerMon());
            r.RegisterByKey("SSP-126", new FlipBonusMon("Shield Attack", 20));
            r.RegisterByKey("SSP-127", new CoinsMon("Double Impact", 2));
            r.RegisterByKey("SSP-128", new CardsJTG2.FruitBearingMon("Stick 'n' Draw", 2));
            r.RegisterByKey("SSP-129", new CardsTWM1.SelfDiscardMon("Duralubeam", 2));
            r.RegisterByKey("SSP-130", new ArchaludonExSSP());
            r.RegisterByKey("SSP-131", new GholdengoSSP());
            r.RegisterByKey("SSP-132", new IronCrownSSP());
            r.RegisterByKey("SSP-133", new AlolanExeggutorExSSP());
            r.RegisterByKey("SSP-134", new AltariaSSP());
            r.RegisterByKey("SSP-135", new DialgaSSP());
            r.RegisterByKey("SSP-136", new PalkiaSSP());
            r.RegisterByKey("SSP-137", new TurtonatorSSP());
            r.RegisterByKey("SSP-138", new ApplinSSP());
            r.RegisterByKey("SSP-139", new FlappleSSP());
            r.RegisterByKey("SSP-140", new AppletunSSP());
            r.RegisterByKey("SSP-141", new EternatusSSP());
            r.RegisterByKey("SSP-142", new TatsugiriExSSP());
            r.RegisterByKey("SSP-143", new BoostedEeveeSSP());
            r.RegisterByKey("SSP-144", new DrawAttackMon("Spike Draw", 1));
            r.RegisterByKey("SSP-145", new SlakothSSP());
            r.RegisterByKey("SSP-147", new SlakingExSSP());
            r.RegisterByKey("SSP-148", new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("SSP-149", new ZangooseSSP());
            r.RegisterByKey("SSP-150", new KecleonSSP());
            r.RegisterByKey("SSP-153", new CardsWHT.DragOffMon("Drag Off", 40));
            r.RegisterByKey("SSP-154", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("SSP-155", new SearchAttackMon("Parabolic Charge", d => d.IsEnergy, 4));
            r.RegisterByKey("SSP-156", new OranguruSSP());
            r.RegisterByKey("SSP-157", new FlipBonusMon("Play Rough", 10));
            r.RegisterByKey("SSP-158", new MausholdSSP());
            r.RegisterByKey("SSP-159", new CyclizarExSSP());
            r.RegisterByKey("SSP-160", new FlamigoExSSP());
            r.RegisterByKey("SSP-161", new TerapagosSSP());

            r.RegisterByName("Amulet of Hope", new AmuletOfHope());
            r.RegisterByName("Babiri Berry", new CardsSSP1.TypeBerry(EnergyType.Metal));
            r.RegisterByName("Call Bell", new CallBell());
            r.RegisterByName("Chill Teaser Toy", new ChillTeaserToy());
            r.RegisterByName("Clemont's Quick Wit", new ClemontsQuickWit());
            r.RegisterByName("Colbur Berry", new CardsSSP1.TypeBerry(EnergyType.Darkness));
            r.RegisterByName("Counter Gain", new CounterGain());
            r.RegisterByName("Deduction Kit", new DeductionKit());
            r.RegisterByName("Dragon Elixir", new DragonElixir());
            r.RegisterByName("Drasna", new Drasna());
            r.RegisterByName("Drayton", new Drayton());
            r.RegisterByName("Energy Search Pro", new EnergySearchPro());
            r.RegisterByName("Jasmine's Gaze", new JasminesGaze());
            r.RegisterByName("Lisia's Appeal", new LisiasAppeal());
            r.RegisterByName("Lively Stadium", new LivelyStadium());
            r.RegisterByName("Meddling Memo", new MeddlingMemo());
            r.RegisterByName("Megaton Blower", new MegatonBlower());
            r.RegisterByName("Passho Berry", new CardsSSP1.TypeBerry(EnergyType.Water));
            r.RegisterByName("Scramble Switch", new ScrambleSwitch());
            r.RegisterByName("Technical Machine: Fluorite", new TechnicalMachineFluorite());
            r.RegisterByName("Tyme", new Tyme());
        }

        // ================================================================== Shared helpers

        /// <summary>Took a card from the deck in a search: Basic Energy of different types, one at a time (Colorful Catch, Energy Search Pro, Prism Charge).</summary>
        private static List<CardInstance> SearchDifferentBasicEnergy(EffectContext ctx, int max, string prompt)
        {
            var taken = new List<CardInstance>();
            for (int i = 0; i < max && ctx.Me.Deck.Count > 0; i++)
            {
                var types = new HashSet<EnergyType>(taken.Select(c => c.Def.EnergyProvides));
                var found = ctx.Engine.SearchDeck(ctx.Player, d => d.IsBasicEnergy && !types.Contains(d.EnergyProvides), 1,
                    prompt + " (" + (i + 1) + (max < 99 ? " of up to " + max : "") + "; choose none to stop)", ctx.Source, true);
                if (found.Count == 0) break;
                taken.AddRange(found);
            }
            return taken;
        }

        /// <summary>Puts the cards on top of the deck in an order the player picks, one at a time from the top.</summary>
        private static void PutOnTopInOrder(EffectContext ctx, List<CardInstance> cards, string prompt)
        {
            var rest = cards.ToList();
            var order = new List<CardInstance>();
            while (rest.Count > 1)
            {
                var next = ctx.ChooseCards(rest, 1, 1, prompt + ": choose the card to put " + (order.Count == 0 ? "on top" : "next (card " + (order.Count + 1) + " from the top)"))[0];
                order.Add(next);
                rest.Remove(next);
            }
            order.AddRange(rest);
            ctx.Me.Deck.InsertRange(0, order);
        }

        /// <summary>Puts Pokémon cards that are in the player's deck onto their Bench (Evolution Pokémon too, when the card says "Pokémon").</summary>
        private static void DeckPokemonToBench(EffectContext ctx, IEnumerable<CardInstance> cards)
        {
            foreach (var c in cards)
            {
                if (ctx.BenchSpace == 0) break;
                ctx.Me.Deck.Remove(c);
                ctx.Engine.PutOntoBench(ctx.Player, c, false);
            }
        }

        // ================================================================== Metal

        /// <summary>This Pokémon has no Weakness (Metal Defender).</summary>
        internal sealed class NoWeaknessEffect : ContinuousEffect
        {
            public NoWeaknessEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon.Uid == TargetUid ? (EnergyType?)null : weakness;
            }
        }

        /// <summary>Assemble Alloy: on evolving from the hand, attach up to 2 Basic Metal Energy from your discard pile to your Metal Pokémon. Metal Defender: no Weakness next turn.</summary>
        private sealed class ArchaludonExSSP : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var metal = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal);
                if (!TriggerOnce(ctx, "Assemble Alloy", metal.Count > 0, "Assemble Alloy: attach up to 2 Basic Metal Energy from your discard pile to your Metal Pokémon?"))
                    return;
                ctx.AttachCardsAnyWay(metal, 2, (c, m) => ctx.Engine.IsType(m, EnergyType.Metal), "Assemble Alloy: attach a Basic Metal Energy");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Defender") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new NoWeaknessEffect(ctx.Pokemon.Name + " has no Weakness during the opponent's next turn (Metal Defender)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Strike It Rich: +90 if this Pokémon evolved from Gimmighoul this turn. Surf Back: you may shuffle this Pokémon into your deck.</summary>
        private sealed class GholdengoSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Strike It Rich")
                {
                    bool rich = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Gimmighoul";
                    ctx.DamageDefending(atk.Damage + (rich ? 90 : 0));
                    return true;
                }
                if (atk.Name == "Surf Back")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me) && ctx.AskYesNo("Surf Back: shuffle " + me.Name + " and all attached cards into your deck?"))
                        ctx.Engine.ShufflePokemonIntoDeck(me);
                    return true;
                }
                return false;
            }
        }

        private sealed class IronCrownSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deleting Slash") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.Bench.Count >= 3 ? 80 : 0));
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Tropical Frenzy: attach any number of Basic Energy from your hand. Swinging Sphene: heads KOs the opponent's Active Basic Pokémon, tails 1 Benched Basic Pokémon.</summary>
        private sealed class AlolanExeggutorExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tropical Frenzy")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsSSP2.AttachFromHandAnyWay(ctx, d => d.IsBasicEnergy, 99, m => true, "Tropical Frenzy: attach a Basic Energy from your hand");
                    return true;
                }
                if (atk.Name == "Swinging Sphene")
                {
                    if (ctx.Flip())
                    {
                        var defending = ctx.Defending;
                        if (defending != null && defending.Def.IsBasicPokemon) ctx.Engine.KnockOut(defending, "Swinging Sphene");
                    }
                    else
                    {
                        var basics = ctx.OpponentBench().Where(b => b.Def.IsBasicPokemon).ToList();
                        if (basics.Count > 0)
                        {
                            var target = ctx.ChoosePokemon(basics, "Swinging Sphene: Knock Out which of your opponent's Benched Basic Pokémon?");
                            if (target != null) ctx.Engine.KnockOut(target, "Swinging Sphene");
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>Humming Charge: attach up to 2 Basic Energy from your deck to your Pokémon. Cotton Wings: flip; heads no damage next turn.</summary>
        private sealed class AltariaSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Humming Charge")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Humming Charge: choose up to 2 Basic Energy to attach");
                    if (found.Count > 0)
                    {
                        ctx.AttachCardsAnyWay(found, found.Count, (c, m) => true, "Humming Charge: attach Basic Energy", false);
                        foreach (var c in found) if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c))) ctx.Me.Deck.Add(c);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Cotton Wings")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && ctx.InPlay(ctx.Pokemon))
                        ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (Cotton Wings)",
                            ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Time Manipulation: search for 2 cards, shuffle, then put them on top in any order.</summary>
        private sealed class DialgaSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Manipulation") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => true, 2, "Time Manipulation: choose 2 cards to put on top of your deck", false, 2);
                ctx.ShuffleMyDeck();
                if (found.Count > 0) PutOnTopInOrder(ctx, found, "Time Manipulation");
                return true;
            }
        }

        private sealed class PalkiaSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Space Crash") return false;
                ctx.DamageDefending(atk.Damage * ctx.Pokemon.Energy.Count(e => e.Def.IsBasicEnergy));
                return true;
            }
        }

        /// <summary>Fully Singe: discard an Energy from the opponent's Active Pokémon ex.</summary>
        private sealed class TurtonatorSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fully Singe") return false;
                var defending = ctx.Defending;
                if (defending != null && (defending.Def.IsEx || defending.Def.IsMegaEx) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Fully Singe: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Nutrients: heal 30 from 1 of your Pokémon. Trip Over: +30 on heads.</summary>
        private sealed class ApplinSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Nutrients")
                {
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(hurt, "Nutrients: heal 30 damage from which of your Pokémon?");
                        if (target != null) ctx.Heal(target, 30);
                    }
                    return true;
                }
                if (atk.Name == "Trip Over")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 30 : 0));
                    return true;
                }
                return false;
            }
        }

        private sealed class FlappleSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Acidic Spit") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage * (defending == null ? 0 : defending.DamageCounters));
                return true;
            }
        }

        /// <summary>Melting Sweetness: the Defending Pokémon can't attack next turn. Wild Tackle: 20 to itself.</summary>
        private sealed class AppletunSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Melting Sweetness")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                        ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Melting Sweetness)", ctx.Player, defending.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Wild Tackle")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(20);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Dyna-Blast: +80 against a Pokémon ex. World Ender: discard a Stadium in play, or the attack does nothing.</summary>
        private sealed class EternatusSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Dyna-Blast")
                {
                    var defending = ctx.Defending;
                    bool ex = defending != null && (defending.Def.IsEx || defending.Def.IsMegaEx);
                    ctx.DamageDefending(atk.Damage + (ex ? 80 : 0));
                    return true;
                }
                if (atk.Name == "World Ender")
                {
                    if (!CardsTWM1.DiscardStadiumInPlay(ctx)) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Surprise Pump: ignores effects on the opponent's Active Pokémon. Cinnabar Lure: put any number of Pokémon from the top 10 cards onto your Bench.</summary>
        private sealed class TatsugiriExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Surprise Pump")
                {
                    ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                if (atk.Name == "Cinnabar Lure")
                {
                    var top = ctx.LookAtTop(10);
                    var mons = top.Where(c => c.Def.IsPokemon).ToList(); // any Pokémon, Evolution Pokémon too (your ruling)
                    int room = ctx.BenchSpace;
                    if (mons.Count > 0 && room > 0)
                    {
                        var picks = ctx.ChooseCards(mons, 0, Math.Min(room, mons.Count), "Cinnabar Lure: put any number of these Pokémon onto your Bench");
                        DeckPokemonToBench(ctx, picks);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Boosted Evolution: while Active, it can evolve on your first turn or the turn you play it. Reckless Charge: 10 to itself.</summary>
        private sealed class BoostedEeveeSSP : Mon
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                return src.Pokemon != null && target == src.Pokemon && src.PokemonIsActive;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reckless Charge") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Take It Easy: heal 60; this Pokémon can't retreat during your next turn.</summary>
        private sealed class SlakothSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Take It Easy") return false;
                var me = ctx.Pokemon;
                ctx.Heal(me, 60);
                var effect = new CantRetreatEffect(me.Name + " can't retreat during your next turn (Take It Easy)", ctx.Player, me.Uid, ctx.Turn + 2);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>Born to Slack: can't attack unless the opponent has a Pokémon ex or Pokémon V in play. Great Swing: discard an Energy.</summary>
        private sealed class SlakingExSSP : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (!ctx.Engine.HasAbilityNamed(ctx.Pokemon, "Born to Slack")) return true;
                return ctx.Opp.InPlay().Any(m => TefPokemon.IsExOrV(m.Def));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Great Swing") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>Fury Cutter: flip 3 coins; +20 / +50 / +80 for 1 / 2 / 3 heads.</summary>
        private sealed class ZangooseSSP : Mon
        {
            private static readonly int[] Bonus = { 0, 20, 50, 80 };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                ctx.DamageDefending(atk.Damage + Bonus[ctx.FlipCoins(3)]);
                return true;
            }
        }

        /// <summary>Expert Hider: flip when damaged by an attack; heads prevents it. Lick Whip: 30 to 1 of the opponent's Pokémon.</summary>
        private sealed class KecleonSSP : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null) return false;
                return src.Engine.FlipCoin(src.Player, "Expert Hider (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lick Whip") return false;
                ctx.DamageAnyOpponent(30);
                return true;
            }
        }

        /// <summary>The Defending Pokémon's Weakness is Colorless (Now You're in My Power).</summary>
        private sealed class ColorlessWeaknessEffect : ContinuousEffect
        {
            public ColorlessWeaknessEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness)
            {
                return mon.Uid == TargetUid ? EnergyType.Colorless : weakness;
            }
        }

        private sealed class OranguruSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Now You're in My Power") return false;
                var defending = ctx.Defending;
                if (defending != null)
                    ctx.AddEffect(new ColorlessWeaknessEffect(defending.Name + "'s Weakness is Colorless until the end of your next turn (Now You're in My Power)",
                        ctx.Player, defending.Uid, ctx.Turn + 2));
                return true;
            }
        }

        /// <summary>Familial March: up to 2 Maushold / Maushold ex from your deck onto your Bench. Incessant Incisors: 30 per heads of 4 coins.</summary>
        private sealed class MausholdSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Familial March")
                {
                    int room = ctx.BenchSpace;
                    if (room == 0 || ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.Name == "Maushold" || d.Name == "Maushold ex", Math.Min(2, room),
                        "Familial March: choose up to 2 Maushold or Maushold ex to put onto your Bench");
                    foreach (var c in found) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Incessant Incisors")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipCoins(4));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Break Through: also 30 to 1 Benched Pokémon. Zircon Road: you may draw 5 cards.</summary>
        private sealed class CyclizarExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Break Through")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageOneBenched(30);
                    return true;
                }
                if (atk.Name == "Zircon Road")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Zircon Road: draw 5 cards?")) ctx.Draw(5);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Precise Beak: +100 if both Active Pokémon have the same amount of Energy. Brave Bird: 30 to itself.</summary>
        private sealed class FlamigoExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Precise Beak")
                {
                    var defending = ctx.Defending;
                    bool same = defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(defending);
                    ctx.DamageDefending(atk.Damage + (same ? 100 : 0));
                    return true;
                }
                if (atk.Name == "Brave Bird")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Prism Charge: up to 3 Basic Energy of different types from your deck, attached to your Tera Pokémon in any way you like.</summary>
        private sealed class TerapagosSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Prism Charge") return false;
                if (ctx.Me.Deck.Count == 0 || ctx.MyPokemon(m => m.Def.IsTera).Count == 0) return true;
                var found = SearchDifferentBasicEnergy(ctx, 3, "Prism Charge: choose a Basic Energy of a type you haven't taken");
                foreach (var c in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Def.IsTera), "Prism Charge: attach " + c.Def.Name + " to which Tera Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(c, target);
                    else ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>When its holder is Knocked Out by damage from an opponent's attack, search your deck for up to 3 cards.</summary>
        private sealed class AmuletOfHope : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Me.Deck.Count == 0) return;
                src.SearchToHand(d => true, 3, "Amulet of Hope: choose up to 3 cards", false, 1);
            }
        }

        private static bool WentSecondFirstTurn(EffectContext ctx) { return ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn; }

        /// <summary>Only on your first turn if you went second: search for a Supporter.</summary>
        private sealed class CallBell : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return WentSecondFirstTurn(ctx) && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(d => d.IsSupporter, 1, "Call Bell: choose a Supporter"); }
        }

        /// <summary>Only on your first turn if you went second: put an Energy attached to 1 of the opponent's Pokémon into their hand.</summary>
        private sealed class ChillTeaserToy : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.Engine.CanPutIntoHand(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return WentSecondFirstTurn(ctx) && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "Chill Teaser Toy: choose 1 of your opponent's Pokémon");
                if (mon == null) return;
                var energy = ctx.ChooseCards(mon.Energy.ToList(), 1, 1, "Chill Teaser Toy: put which Energy into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(mon, energy);
            }
        }

        /// <summary>Heal 60 damage from each of your Lightning Pokémon.</summary>
        private sealed class ClemontsQuickWit : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Lightning));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var m in Hurt(ctx)) ctx.Heal(m, 60);
            }
        }

        /// <summary>With more Prize cards left than your opponent, the holder's attacks cost 1 Colorless less.</summary>
        private sealed class CounterGain : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Me.Prizes.Count <= src.Opp.Prizes.Count) return;
                int i = cost.IndexOf(EnergyType.Colorless);
                if (i >= 0) cost.RemoveAt(i);
            }
        }

        /// <summary>Look at the top 3 cards: put them back in any order, or shuffle them and put them on the bottom.</summary>
        private sealed class DeductionKit : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return;
                int pick = ctx.ChooseOption("Deduction Kit: the top cards are " + string.Join(", ", top.Select(c => c.Def.Name).ToArray()) + ".",
                    "Put them back on top in any order", "Shuffle them and put them on the bottom");
                foreach (var c in top) ctx.Me.Deck.Remove(c);
                if (pick == 0)
                {
                    PutOnTopInOrder(ctx, top, "Deduction Kit");
                    return;
                }
                ctx.State.Rng.Shuffle(top);
                ctx.Me.Deck.AddRange(top);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + top.Count + " cards on the bottom of " + ctx.Engine.Their(ctx.Player) + " deck.");
            }
        }

        /// <summary>Heal 60 damage from your Active Dragon Pokémon.</summary>
        private sealed class DragonElixir : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && a.DamageCounters > 0 && ctx.Engine.IsType(a, EnergyType.Dragon) && !ctx.Engine.HealingBlocked(a);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 60);
            }
        }

        /// <summary>Shuffle your hand into your deck; flip: heads draw 8, tails draw 3.</summary>
        private sealed class Drasna : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Flip() ? 8 : 3);
            }
        }

        /// <summary>Look at the top 7 cards; you may take a Pokémon and a Trainer card from them. Shuffle the rest back.</summary>
        private sealed class Drayton : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var taken = new List<CardInstance>();
                var mons = top.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count > 0) taken.AddRange(ctx.ChooseCards(mons, 0, 1, "Drayton: you may take a Pokémon"));
                var trainers = top.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count > 0) taken.AddRange(ctx.ChooseCards(trainers, 0, 1, "Drayton: you may take a Trainer card"));
                RecifeTrainers.TakeIntoHand(ctx, taken);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Search for any number of Basic Energy cards of different types.</summary>
        private sealed class EnergySearchPro : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = SearchDifferentBasicEnergy(ctx, 99, "Energy Search Pro: choose a Basic Energy of a type you haven't taken");
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>During the opponent's next turn, your Pokémon take 30 less damage from their attacks (new ones too).</summary>
        private sealed class JasminesGazeEffect : ContinuousEffect
        {
            public JasminesGazeEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Owner == Owner && info.AgainstOpponent ? damage - 30 : damage;
            }
        }

        private sealed class JasminesGaze : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new JasminesGazeEffect(ctx.Engine.Possessive(ctx.Player) + " Pokémon take 30 less damage during the opponent's next turn (Jasmine's Gaze)",
                    ctx.Player, ctx.Turn + 1));
            }
        }

        /// <summary>Switch in 1 of the opponent's Benched Basic Pokémon; if you did, it's Confused.</summary>
        private sealed class LisiasAppeal : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Any(b => b.Def.IsBasicPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = ctx.Opp.Bench.Where(b => b.Def.IsBasicPokemon).ToList();
                if (basics.Count == 0) return;
                var target = ctx.ChoosePokemon(basics, "Lisia's Appeal: switch in which Benched Basic Pokémon?");
                if (target != null && ctx.Engine.SwitchActive(ctx.Opponent, target)) ctx.ApplyCondition(target, SpecialCondition.Confused);
            }
        }

        /// <summary>Each Basic Pokémon in play gets +30 HP.</summary>
        private sealed class LivelyStadium : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon == null && mon.Def.IsBasicPokemon ? hp + 30 : hp;
            }
        }

        /// <summary>The opponent shuffles their hand onto the bottom of their deck and draws that many cards.</summary>
        private sealed class MeddlingMemo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var opp = ctx.Opp;
                var hand = opp.Hand.ToList();
                if (hand.Count == 0) return;
                opp.Hand.Clear();
                ctx.State.Rng.Shuffle(hand);
                opp.Deck.AddRange(hand);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + hand.Count + " cards from " + ctx.Engine.Their(ctx.Opponent) + " hand on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck.");
                ctx.Engine.DrawCards(ctx.Opponent, hand.Count);
            }
        }

        /// <summary>Discard every Pokémon Tool and Special Energy from the opponent's Pokémon, and the Stadium in play.</summary>
        private sealed class MegatonBlower : CardScript
        {
            // Compendium (Surging Sparks FAQ, 2024-11-07): it can't be played for no effect, but any one of the three is enough.
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.State.Stadium != null || ctx.OpponentPokemon().Any(m => m.Tool != null || m.Energy.Any(x => x.Def.IsAttachedSpecialEnergy));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var m in ctx.OpponentPokemon())
                {
                    if (m.Tool != null && ctx.CanAffect(m)) ctx.Engine.DiscardAllTools(m);
                    foreach (var e in m.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(m, e);
                }
                CardsTWM1.DiscardStadiumInPlay(ctx);
            }
        }

        /// <summary>Switch your Active Pokémon; then you may move any Energy from the old Active to the new one.</summary>
        private sealed class ScrambleSwitch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && ctx.Me.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var old = ctx.Me.Active;
                if (!ctx.SwitchMyActive("Scramble Switch: choose your new Active Pokémon")) return;
                var now = ctx.Me.Active;
                if (old == null || now == null || old.Energy.Count == 0) return;
                var picks = ctx.ChooseCards(old.Energy.ToList(), 0, old.Energy.Count, "Scramble Switch: move any Energy from " + old.Name + " to " + now.Name);
                foreach (var e in picks) ctx.Engine.MoveEnergy(old, now, e);
            }
        }

        /// <summary>Gives its holder the Fluorite attack; discarded at the end of your turn.</summary>
        private sealed class TechnicalMachineFluorite : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            /// <summary>Discarded at the end of your turn by the engine, even while Tools have no effect (your ruling, v97).</summary>
            public override bool DiscardedAtEndOfOwnersTurn { get { return true; } }

            /// <summary>Fluorite: discard all Energy from this Pokémon and heal all damage from each of your Tera Pokémon.</summary>
            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Pokemon == null) return;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                foreach (var m in ctx.MyPokemon(x => x.Def.IsTera && x.DamageCounters > 0)) ctx.Heal(m, m.DamageCounters * 10);
            }
        }

        /// <summary>Name a Pokémon in your hand; the opponent guesses its HP. Right: they draw 4. Wrong: you draw 4.</summary>
        private sealed class Tyme : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count == 0) return;
                var card = ctx.ChooseCards(mons, 1, 1, "Tyme: choose a Pokémon from your hand")[0];
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " named " + card.Def.Name + ".");
                var opts = new List<DecisionOption>();
                for (int hp = 30; hp <= 340; hp += 10) opts.Add(new DecisionOption { Label = hp + " HP", Index = hp });
                int pick = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Tyme: guess the HP of " + card.Def.Name, opts, ctx.Source, false);
                int guess = opts[pick].Index;
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "revealed (" + card.Def.Hp + " HP; the guess was " + guess + ")");
                if (guess == card.Def.Hp) ctx.Engine.DrawCards(ctx.Opponent, 4);
                else ctx.Draw(4);
            }
        }
    }
}
