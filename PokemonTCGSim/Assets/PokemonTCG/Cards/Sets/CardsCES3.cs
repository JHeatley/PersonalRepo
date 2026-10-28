using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Celestial Storm (CES, v83), part 3: Darkness, Metal, Dragon and Colorless (CES 84-122). Scizor-GX, Celesteela, Kartana
    /// and Stakataka-GX (Hidden Fates Shiny Vault) were already in the pool. Kecleon's Unit Color 2 makes it only
    /// the 3 listed types, like Lost Thunder's Kecleon (your ruling, v81). Pokémon Tools are not Item cards (your ruling, v90).
    /// </summary>
    public static class CardsCES3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Darkness
            r.RegisterByKey("CES-84", new SearchAttackMon("Call for the Boss", d => d.Name == "Alolan Raticate" || d.Name == "Alolan Raticate-GX", 1));
            r.RegisterByKey("CES-85", new ComboMon(new ChuckAwayMon(), new TailsNothingMon("Hyper Fang"), new SearchAttackMon("Item Maniac-GX", CardsLOT4.IsItemCard, 6)));
            r.RegisterByKey("CES-86", new ComboMon(new CoinsMon("Fury Swipes", 3), new CardsASR1.TimesMon("Beat Up", c => c.FlipCoins(c.MyPokemon().Count))));
            r.RegisterByKey("CES-87", new ComboMon(new CoinsMon("Slam", 2), new TrampleMon()));
            r.RegisterByKey("CES-88", new SableyeCES());
            // Metal
            r.RegisterByKey("CES-89", new FlipBonusMon("Tail Crush", 40));
            r.RegisterByKey("CES-91", new ComboMon(new MiningMon(), new CardsFST2.ConditionBonusMon("Bite Off", 30, c => c.Defending != null && c.Defending.Def.IsGXorEX)));
            r.RegisterByKey("CES-92", new BeldumCES());
            r.RegisterByKey("CES-93", new TailsNothingMon("Single Smash"));
            r.RegisterByKey("CES-94", new CardsWHT.FlipMoreMon("Bullet Punch", 2, 20));
            r.RegisterByKey("CES-95", new CardsASC.NextTurnBoostMon("Meteor Mash", 60));
            r.RegisterByKey("CES-96", new CardsFST1.TakeLessMon(20, new CardsFST2.ConditionBonusMon("Silver Fist", 60, c => c.Defending != null && c.Engine.HasAbility(c.Defending))));
            r.RegisterByKey("CES-97", new JirachiPrismStar());
            r.RegisterByKey("CES-98", new ComboMon(new SteelworksMon(), new RecoilMon("Steel Tackle", 30)));
            r.RegisterByKey("CES-99", new SolgaleoCES());
            // Dragon
            r.RegisterByKey("CES-103", new CardsASC.SelfGuardMon("Rock Head", 10));
            r.RegisterByKey("CES-104", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("CES-105", new CardsASC.SelfDamagedBonusMon("Raging Blade", 50));
            r.RegisterByKey("CES-106", new SalamenceCES());
            r.RegisterByKey("CES-107", new DreamyMistMon());
            r.RegisterByKey("CES-108", new CardsASR1.TimesMon("Dragon Fleet", c => c.MyPokemon(m => m.Def.IsEvolution && c.Engine.IsType(m, EnergyType.Dragon)).Count));
            r.RegisterByKey("CES-109", new RayquazaGXCES());
            // Colorless
            r.RegisterByKey("CES-110", new ComboMon(new StrikeAndRunMon(), new ParalyzeFlipMon("Sudden Flash")));
            r.RegisterByKey("CES-112", new ComboMon(new FirefightingMon(), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("CES-113", new ComboMon(new TailsNothingMon("Claw"), new SlackOffMon()));
            r.RegisterByKey("CES-114", new ComboMon(new CoinsMon("Fury Swipes", 3), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("CES-115", new SlakingCES());
            r.RegisterByKey("CES-116", new BawlMon());
            r.RegisterByKey("CES-117", new WailMon());
            r.RegisterByKey("CES-118", new ConditionAttackMon("Cracking Voice", SpecialCondition.Confused));
            r.RegisterByKey("CES-119", new DangerousConcertMon());
            r.RegisterByKey("CES-120", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("CES-121", new DelcattyCES());
            r.RegisterByKey("CES-122", new UnitColorMon("Unit Energy LPM", EnergyType.Lightning, EnergyType.Psychic, EnergyType.Metal));
        }

        // ================================================================== Shared helpers

        /// <summary>
        /// Unit Color: while the named Unit Energy is attached, the Pokémon is only the listed types (your ruling for Lost Thunder's
        /// Kecleon, v81). Its attacks have no effects.
        /// </summary>
        internal sealed class UnitColorMon : Mon
        {
            private readonly string unit;
            private readonly EnergyType[] colors;

            public UnitColorMon(string unit, params EnergyType[] colors)
            {
                this.unit = unit;
                this.colors = colors;
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!mon.Energy.Any(e => e.Def.Name == unit)) return;
                types.Clear();
                types.AddRange(colors);
            }
        }

        /// <summary>"Your opponent can't play any Trainer cards from their hand during their next turn" (Bawl).</summary>
        internal sealed class TrainerLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public TrainerLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card != null && card.Def.IsTrainer;
            }
        }

        // ================================================================== Darkness

        /// <summary>Chuck Away: discard up to 2 cards from your hand; 40 damage for each card discarded.</summary>
        private sealed class ChuckAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chuck Away") return false;
                var hand = ctx.Me.Hand.ToList();
                int n = 0;
                if (hand.Count > 0)
                    foreach (var c in ctx.ChooseCards(hand, 0, Math.Min(2, hand.Count), "Chuck Away: discard up to 2 cards from your hand (40 damage each)"))
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, c);
                        n++;
                    }
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log("Chuck Away did no damage.");
                return true;
            }
        }

        /// <summary>
        /// Trample: for each Benched Pokémon (both players'), flip a coin; on heads, 60 damage to it. "This attack's damage isn't affected
        /// by Weakness or Resistance" covers the damage to the Active Pokémon too (Bench damage never gets them anyway).
        /// </summary>
        private sealed class TrampleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trample") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.DamageWith(defending, atk.Damage, false, false, false);
                foreach (var t in ctx.OpponentBench())
                    if (ctx.Flip() && ctx.InPlay(t)) ctx.DamageWith(t, 60, false, false, false);
                foreach (var t in ctx.MyBench())
                    if (ctx.Flip() && ctx.InPlay(t)) ctx.DamageWith(t, 60, false, false, false);
                return true;
            }
        }

        /// <summary>Excavate: once during your turn, look at the top card of your deck; you may discard it. Disable: 1 of the Defending Pokémon's attacks is off next turn.</summary>
        private sealed class SableyeCES : Mon
        {
            private static readonly Mon Disable = new CardsPOR.MemoryLockMon("Disable");

            protected override string Activated { get { return "Excavate"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return;
                if (ctx.AskYesNo("Excavate: the top card of your deck is " + top[0].Def.Name + ". Discard it?"))
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Disable.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>
        /// Mining (your ruling, v90): search your deck for an Item card and put it into your hand. Pokémon Tools aren't Item cards, so
        /// it can't find one and the printed Tool clause never applies.
        /// </summary>
        private sealed class MiningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mining") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(CardsLOT4.IsItemCard, 1, "Mining: choose an Item card");
                return true;
            }
        }

        /// <summary>Conductive Body: while this Pokémon is Active, its Retreat Cost is [C] less for each Beldum on your Bench.</summary>
        private sealed class BeldumCES : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon != me || !src.IsActive(me)) return cost;
                int beldum = src.Me.Bench.Count(m => m.Def.Name == "Beldum");
                return Math.Max(0, cost - beldum);
            }
        }

        /// <summary>
        /// Wish Upon a Star: taken as a face-down Prize card during your turn with room on your Bench, you may put it onto your Bench and
        /// take 1 more Prize card. It works from the Prize cards, so Ability blockers in play don't stop it; with a full Bench it goes to
        /// the hand (Compendium, 2018-08-02). Perish Dream: this Pokémon is now Asleep; at the end of the opponent's next turn, the
        /// Defending Pokémon is Knocked Out.
        /// </summary>
        private sealed class JirachiPrismStar : Mon
        {
            public override int OnTakenAsPrize(EffectContext ctx)
            {
                if (!ctx.Engine.BenchHasRoom(ctx.Player)) return -1;
                if (!ctx.AskYesNo("Wish Upon a Star: put Jirachi ◇ onto your Bench and take 1 more Prize card?")) return -1;
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put Jirachi ◇ onto the Bench and takes 1 more Prize card (Wish Upon a Star).");
                return 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Perish Dream") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of your opponent's next turn (Perish Dream)",
                    ctx.Player, defending.Uid, ctx.Turn, "Perish Dream", 0));
                return true;
            }
        }

        /// <summary>Steelworks: look at the top 4 cards of your deck, attach any number of Metal Energy cards there to 1 of your Pokémon, shuffle the rest back.</summary>
        private sealed class SteelworksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Steelworks") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return true;
                var metal = top.Where(x => CardsSSP1.IsBasicOf(x.Def, EnergyType.Metal)).ToList();
                if (metal.Count == 0)
                    ctx.Engine.ShowCards(ctx.Player, top, "Steelworks", "There's no Metal Energy card in the top " + top.Count + " cards of your deck", ctx.Source);
                else
                {
                    var picks = ctx.ChooseCards(metal, 0, metal.Count, "Steelworks: attach any number of these Metal Energy cards to 1 of your Pokémon");
                    if (picks.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Steelworks: attach the Metal Energy to which Pokémon?");
                        if (target != null)
                            foreach (var c in picks)
                                if (ctx.Me.Deck.Remove(c)) ctx.Engine.AttachEnergyCard(c, target);
                    }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Full Metal Body: with any Metal Energy attached, this Pokémon has no Weakness. Rising Dash: not affected by Resistance.</summary>
        private sealed class SolgaleoCES : Mon
        {
            private static readonly Mon Dash = new CardsFLF1.NoResistanceMon("Rising Dash");

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon == src.Pokemon && src.Engine.HasEnergyOfType(mon, EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dash.HandleAttack(ctx, atk); }
        }

        // ================================================================== Dragon

        /// <summary>Dragon Wind: once during your turn, if this Pokémon is Active, you may switch 1 of the opponent's Benched Pokémon with their Active Pokémon.</summary>
        private sealed class SalamenceCES : Mon
        {
            protected override string Activated { get { return "Dragon Wind"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Opp.Active != null && ctx.GustTargets().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.GustOpponent("Dragon Wind: switch which of your opponent's Benched Pokémon into the Active Spot?");
            }
        }

        /// <summary>Dreamy Mist: attach a basic Energy card from your discard pile to each of your Benched Basic Dragon Pokémon.</summary>
        private sealed class DreamyMistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dreamy Mist") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Where(m => m.Def.IsBasicPokemon && ctx.Engine.IsType(m, EnergyType.Dragon)).ToList())
                {
                    var energy = ctx.BasicEnergyInDiscard();
                    if (energy.Count == 0) break;
                    if (!ctx.InPlay(mon)) continue;
                    var pick = energy.Count == 1 ? energy : ctx.ChooseCards(energy, 1, 1, "Dreamy Mist: attach which basic Energy card to " + mon.Name + "?");
                    if (pick.Count == 0 || !ctx.Me.Discard.Remove(pick[0])) continue;
                    ctx.Engine.AttachEnergyCard(pick[0], mon);
                }
                return true;
            }
        }

        /// <summary>
        /// Stormy Winds: when you play this Pokémon from your hand onto your Bench, you may discard the top 3 cards of your deck; if you
        /// do, attach a basic Energy card from your discard pile to it (usable with at least 1 card in the deck: Compendium, 2018-09-06).
        /// Dragon Break: 30 for each basic Grass and basic Lightning Energy on your Pokémon (2 units from one basic card count twice:
        /// 2018-08-09). Tempest-GX: discard your hand and draw 10 cards.
        /// </summary>
        private sealed class RayquazaGXCES : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!TriggerOnce(ctx, "Stormy Winds", ctx.Me.Deck.Count > 0, "Use Stormy Winds? Discard the top 3 cards of your deck, then attach a basic Energy card from your discard pile to " + me.Name + ".")) return;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                if (milled.Count == 0 || !ctx.InPlay(me)) return;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return;
                var pick = ctx.ChooseCards(energy, 1, 1, "Stormy Winds: attach which basic Energy card to " + me.Name + "?");
                if (pick.Count == 0 || !ctx.Me.Discard.Remove(pick[0])) return;
                ctx.Engine.AttachEnergyCard(pick[0], me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Dragon Break")
                {
                    int units = 0;
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var e in mon.Energy)
                        {
                            if (!e.Def.IsBasicEnergy) continue;
                            foreach (int u in ctx.Engine.EnergyUnitsOf(mon, e))
                                if (EnergyMask.Provides(u, EnergyType.Grass) || EnergyMask.Provides(u, EnergyType.Lightning)) units++;
                        }
                    if (units > 0) ctx.DamageDefending(atk.Damage * units);
                    else ctx.Log("Dragon Break did no damage.");
                    return true;
                }
                if (atk.Name == "Tempest-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardHand(ctx.Player);
                    ctx.Draw(10);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Strike and Run: put up to 3 Basic Pokémon from your deck onto your Bench; if you did, you may switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class StrikeAndRunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strike and Run") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var found = ctx.SearchToBench(d => true, 3, "Strike and Run: choose up to 3 Basic Pokémon to put onto your Bench");
                var me = ctx.Pokemon;
                if (found.Count == 0 || !ctx.InPlay(me) || !ctx.IsActive(me) || ctx.Me.Bench.Count == 0) return true;
                if (ctx.AskYesNo("Strike and Run: switch " + me.Name + " with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Firefighting: discard a Fire Energy from the opponent's Active Pokémon.</summary>
        private sealed class FirefightingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Firefighting") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.DiscardEnergyFrom(defending, 1, "Firefighting: discard a Fire Energy from " + defending.Name, c => CardsTEU1.ProvidesType(ctx, defending, c, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Slack Off: heal all damage from this Pokémon; it can't attack during your next turn.</summary>
        private sealed class SlackOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slack Off") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                CardsTEU1.HealAll(ctx, me);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Lazy: while this Pokémon is your Active Pokémon, the opponent's Pokémon in play have no Abilities, except Lazy. Critical Move:
        /// discard an Energy from this Pokémon; it can't attack during your next turn.
        /// </summary>
        private sealed class SlakingCES : Mon
        {
            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                var me = src.Pokemon;
                return mon != null && me != null && mon.Owner != src.Player && abilityName != "Lazy" && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Critical Move") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Critical Move: discard an Energy from " + me.Name);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Bawl: only if you went second and on your first turn; the opponent can't play Trainer cards from their hand during their next turn.</summary>
        private sealed class BawlMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Bawl" || CardsUNB1.SecondPlayersFirstTurn(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bawl") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new TrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards from their hand during their next turn (Bawl)",
                    ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Wail: the opponent switches their Active Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class WailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wail") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Dangerous Concert: also 30 damage to each of the opponent's Benched Pokémon that has any damage counters on it.</summary>
        private sealed class DangerousConcertMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dangerous Concert") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var t in ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList()) ctx.DamageTarget(t, 30);
                return true;
            }
        }

        /// <summary>Search for Friends: when you play this Pokémon from your hand to evolve, you may put 2 Supporter cards from your discard pile into your hand.</summary>
        private sealed class DelcattyCES : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (!TriggerOnce(ctx, "Search for Friends", supporters.Count > 0, "Use Search for Friends? Put 2 Supporter cards from your discard pile into your hand.")) return;
                int n = Math.Min(2, supporters.Count);
                var picks = supporters.Count <= n ? supporters : ctx.ChooseCards(supporters, n, n, "Search for Friends: put which 2 Supporter cards into your hand?");
                ctx.DiscardToHand(picks);
            }
        }
    }
}
