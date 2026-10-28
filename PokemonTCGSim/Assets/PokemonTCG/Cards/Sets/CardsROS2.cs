using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Roaring Skies (ROS, v99), part 2: the Fairy, Dragon and Colorless Pokémon (43-82). Rayquaza-EX (76), Shaymin-EX's full art
    /// and the secret rares are reprints of these.
    /// </summary>
    public static class CardsROS2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("ROS-43", new CardsPLS2.SweetKissMon());
            r.RegisterByKey("ROS-44", new GoFetchMon());
            r.RegisterByKey("ROS-45", new ComboMon(new CardsASC.FlipPerEnergyMon("Powerful Slap"), new CardsMEG.SnipeBenchMon("Aura Sphere", 20)));
            r.RegisterByKey("ROS-46", new TogekissROS());
            r.RegisterByKey("ROS-47", new CarbinkROS());
            r.RegisterByKey("ROS-48", new ComboMon(new LookForKeysMon(), new FlipBonusMon("Play Rough", 20)));
            r.RegisterByKey("ROS-49", new CoinsMon("Slam", 2));
            r.RegisterByKey("ROS-50", new ComboMon(new CardsTWM1.HealSelfMon("Shed Skin", 30), new CoinsMon("Slam", 2)));
            r.RegisterByKey("ROS-51", new DragoniteMaxWind());
            r.RegisterByKey("ROS-52", new WrappedInWindMon());
            r.RegisterByKey("ROS-53", new ComboMon(new SongOfEchoesMon(), new ConditionAttackMon("Midnight Eyes", SpecialCondition.Asleep)));
            r.RegisterByKey("ROS-54", new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt"));
            r.RegisterByKey("ROS-55", new ComboMon(new ParalyzeFlipMon("Leer"), new RecoilMon("Almost Flight", 10)));
            r.RegisterByKey("ROS-56", new CardsFST1.TakeLessMon(10, null));
            r.RegisterByKey("ROS-57", new ComboMon(new CardsPAF1.DiscardStadiumMon("Shatter"), new CardsASR1.TimesMon("Power Howl", c => c.Opp.Hand.Count),
                new CardsTWM1.SelfDiscardMon("Steam Blast", 3)));
            r.RegisterByKey("ROS-58", new CardsROS1.FirstTurnMon("Fast Raid", new CardsPHF2.LightPulseMon()));
            r.RegisterByKey("ROS-59", new SonicAceMon());
            r.RegisterByKey("ROS-60", new DragonStrikeMon());
            r.RegisterByKey("ROS-61", new CardsTWM1.SelfDiscardMon("Dragon Ascent", 2));
            r.RegisterByKey("ROS-62", new HydreigonEX());
            r.RegisterByKey("ROS-63", new ReshiramROS());
            r.RegisterByKey("ROS-64", new EnergyStreamMon());
            r.RegisterByKey("ROS-66", new CardsASC.FlipDiscardDefendingEnergyMon("Drill Run"));
            r.RegisterByKey("ROS-67", new ComboMon(new DrawAttackMon("Feelin' Fine", 1), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("ROS-68", new CardsSMP4.MillOpponentMon("Burrow", 1));
            r.RegisterByKey("ROS-69", new ComboMon(new CallForFamilyMon(), new AnyStadiumBonusMon("Blow Through", 30)));
            r.RegisterByKey("ROS-70", new CoinsMon("Double Peck", 2));
            r.RegisterByKey("ROS-71", new ComboMon(new NoRetreatMon("Clutch"), new FlipBonusMon("Aerial Ace", 30)));
            r.RegisterByKey("ROS-74", new AltariaROS());
            r.RegisterByKey("ROS-75", new ComboMon(new IntensifyingBurnMon(), new CardsBRS1.MillSelfMon("Dragon Pulse", 3)));
            r.RegisterByKey("ROS-77", new ShayminEX());
            r.RegisterByKey("ROS-78", new HomingPidoveMon());
            r.RegisterByKey("ROS-79", new DodgeMon("Fly", true));
            r.RegisterByKey("ROS-80", new ComboMon(new FlipBonusMon("Quick Attack", 30), new StrongWindsMon()));
            r.RegisterByKey("ROS-81", new ComboMon(new FeatherDanceMon(), new TailsNothingMon("Sky Attack")));
            r.RegisterByKey("ROS-82", new CardsWHT.FlipMoreMon("Acrobatics", 2, 10));
        }

        // ================================================================== Fairy

        /// <summary>Go Fetch: shuffle 3 basic Energy cards from your discard pile into your deck (as many as there are, if fewer).</summary>
        private sealed class GoFetchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Go Fetch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return true;
                var picks = pile.Count <= 3 ? pile : ctx.ChooseCards(pile, 3, 3, "Go Fetch: shuffle which 3 basic Energy cards into your deck?");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.Engine.Reveal(ctx.Player, picks, "shuffled these Energy cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Serene Grace: evolving from the hand, you may look at the top 8 cards of your deck and attach any number of basic Energy
        /// cards from them to your Pokémon in any way; shuffle the rest back.
        /// </summary>
        private sealed class TogekissROS : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Serene Grace", ctx.Me.Deck.Count > 0, "Serene Grace: look at the top 8 cards of your deck and attach basic Energy from them to your Pokémon?")) return;
                var top = ctx.LookAtTop(8);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, energy.Count, (card, mon) => true, "Serene Grace");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Jewel Armor: on your Bench, no damage from any attack (yours too). Spin Tackle: tails, 20 to itself.</summary>
        private sealed class CarbinkROS : Mon
        {
            private static readonly Mon Tackle = new CardsFST1.FlipRecoilMon("Spin Tackle", 20);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && src.Me.Bench.Contains(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>Look for Keys: reveal cards from the top of your deck until you reveal an Item card; put it into your hand, shuffle the rest back.</summary>
        private sealed class LookForKeysMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Look for Keys") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                int index = ctx.Me.Deck.FindIndex(c => c.Def.IsItem);
                var revealed = ctx.Me.Deck.Take(index < 0 ? ctx.Me.Deck.Count : index + 1).ToList();
                ctx.Engine.Reveal(ctx.Player, revealed, "revealed these cards from the top of " + ctx.Engine.Their(ctx.Player) + " deck (Look for Keys)");
                if (index >= 0)
                {
                    var item = ctx.Me.Deck[index];
                    ctx.Me.Deck.RemoveAt(index);
                    ctx.Me.Hand.Add(item);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Max Wind: evolving from the hand, you may heal all damage from 1 of your Pokémon. Mach Press: 2 heads, Paralyzed.</summary>
        private sealed class DragoniteMaxWind : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon().Where(m => m.DamageCounters > 0).ToList();
                if (!TriggerOnce(ctx, "Max Wind", hurt.Count > 0, "Max Wind: heal all damage from 1 of your Pokémon?")) return;
                var target = ctx.ChoosePokemon(hurt, "Max Wind: heal all damage from which Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Press") return false;
                var defending = ctx.Defending;
                bool both = ctx.FlipCoins(2) == 2;
                ctx.DamageDefending(atk.Damage);
                if (both && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Wrapped in Wind: attach up to 2 basic Energy cards from your hand to this Pokémon.</summary>
        private sealed class WrappedInWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wrapped in Wind") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, 2, (card, mon) => mon == me, "Wrapped in Wind: attach basic Energy from your hand to " + me.Name);
                return true;
            }
        }

        /// <summary>Song of Echoes: shuffle your hand into your deck, then draw a card for each Benched Pokémon (both players').</summary>
        private sealed class SongOfEchoesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Song of Echoes") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                int n = ctx.Me.Bench.Count + ctx.Opp.Bench.Count;
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Sonic Ace: discard 2 Energy from this Pokémon, then 120 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class SonicAceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Ace") return false;
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                ctx.DamageAnyOpponent(atk.Damage);
                return true;
            }
        }

        /// <summary>Dragon Strike: flip a coin; tails, this Pokémon can't use Dragon Strike during your next turn.</summary>
        private sealed class DragonStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Strike") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Dragon Road: with any Stadium in play, each of your Dragon Pokémon retreats for 2 less. Shred: ignores effects on the Defending Pokémon.</summary>
        private sealed class HydreigonEX : Mon
        {
            private static readonly Mon Shred = new CardsPAF2.IgnoreDefenderEffectsMon("Shred");

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.State.Stadium == null || mon == null || mon.Owner != src.Player || !src.Engine.IsType(mon, EnergyType.Dragon)) return cost;
                return Math.Max(0, cost - 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shred.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Turboblaze: once during your turn, if this Pokémon is Active, you may attach a Fire Energy card from your hand to 1 of your
        /// Dragon Pokémon. Bright Wing: discard a Fire Energy from this Pokémon.
        /// </summary>
        private sealed class ReshiramROS : Mon
        {
            private static readonly Mon Wing = new CardsASC.SelfDiscardTypeMon("Bright Wing", EnergyType.Fire, 1);

            protected override string Activated { get { return "Turboblaze"; } }

            private static List<CardInstance> FireCards(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fire).ToList();
            }

            private static bool IsDragon(EffectContext ctx, PokemonInPlay mon) { return ctx.Engine.IsType(mon, EnergyType.Dragon) && ctx.Engine.CanAttachFromHand(mon); }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active == ctx.Pokemon && FireCards(ctx).Count > 0 && ctx.MyPokemon().Any(m => IsDragon(ctx, m));
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AttachCardsAnyWay(FireCards(ctx), 1, (card, mon) => IsDragon(ctx, mon), "Turboblaze: attach a Fire Energy card from your hand to 1 of your Dragon Pokémon", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wing.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Stream: attach a basic Energy card from your discard pile to this Pokémon.</summary>
        private sealed class EnergyStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Stream") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.BasicEnergyInDiscard();
                if (ctx.InPlay(me) && pile.Count > 0)
                    ctx.AttachCardsAnyWay(pile, 1, (card, mon) => mon == me, "Energy Stream: attach a basic Energy card from your discard pile to " + me.Name, false);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>"If there is any Stadium card in play, this attack does N more damage" (Blow Through).</summary>
        internal sealed class AnyStadiumBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public AnyStadiumBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.State.Stadium != null ? bonus : 0));
                return true;
            }
        }

        /// <summary>Clear Humming: each of your Colorless Pokémon has no Weakness.</summary>
        private sealed class AltariaROS : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner == src.Player && src.Engine.IsType(mon, EnergyType.Colorless);
            }
        }

        /// <summary>Intensifying Burn: 50 more damage if the opponent's Active Pokémon is a Pokémon-EX.</summary>
        private sealed class IntensifyingBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Intensifying Burn") return false;
                var d = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (d != null && d.Def.IsPokemonEX ? 50 : 0));
                return true;
            }
        }

        /// <summary>Set Up: played from your hand onto your Bench, you may draw cards until you have 6 in your hand. Sky Return: back to your hand.</summary>
        private sealed class ShayminEX : Mon
        {
            private static readonly Mon Return = new CardsPAF1.ReturnSelfMon("Sky Return");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool ready = ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0;
                if (!TriggerOnce(ctx, "Set Up", ready, "Set Up: draw cards until you have 6 cards in your hand?")) return;
                ctx.DrawUntil(6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Return.HandleAttack(ctx, atk); }
        }

        /// <summary>Homing Pidove: look at the top card of your deck; you may shuffle your deck afterward.</summary>
        private sealed class HomingPidoveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Homing Pidove") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.LookAtTop(1);
                if (ctx.AskYesNo("Homing Pidove: the top card of your deck is " + top[0].Def.Name + ". Shuffle your deck?")) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Strong Winds: each player shuffles all cards attached to each of their Pokémon (Energy and Tools) into their deck.</summary>
        private sealed class StrongWindsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong Winds") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 2; i++)
                {
                    int side = i == 0 ? ctx.Player : ctx.Opponent;
                    foreach (var mon in ctx.State.Players[side].InPlay().ToList())
                    {
                        if (side != ctx.Player && !ctx.CanAffect(mon)) continue;
                        if (mon.Energy.Count > 0) ctx.Engine.ShuffleEnergyIntoDeck(mon, mon.Energy.ToList());
                        foreach (var tool in mon.Tools.ToList())
                        {
                            mon.Tools.Remove(tool);
                            ctx.State.Players[tool.Owner].Deck.Add(tool);
                        }
                    }
                    ctx.Engine.ShuffleDeck(side);
                }
                return true;
            }
        }

        /// <summary>During your next turn, each of this Pokémon's attacks does N more damage to the opponent's Active Pokémon (Feather Dance).</summary>
        internal sealed class AllAttacksBoostEffect : ContinuousEffect
        {
            private readonly int bonus;

            public AllAttacksBoostEffect(string name, int owner, int targetUid, int expiresAfterTurn, int bonus)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.bonus = bonus;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target == null || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
                return damage + bonus;
            }
        }

        /// <summary>Feather Dance: during your next turn, each of this Pokémon's attacks does 80 more damage (before Weakness and Resistance).</summary>
        private sealed class FeatherDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Feather Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var boost = new AllAttacksBoostEffect(me.Name + "'s attacks do 80 more damage during its next turn (Feather Dance)", ctx.Player, me.Uid, ctx.Turn + 2, 80);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }
    }
}
