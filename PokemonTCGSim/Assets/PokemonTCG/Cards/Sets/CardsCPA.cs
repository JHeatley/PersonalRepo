using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Champion's Path (CPA, 2020): the Pokémon, the 8 new Trainers and the secret rare Charizard V. Regulation mark D:
    /// Expanded only.
    /// Already in the pool: Sizzlipede, Rolycoly, Alcremie V / VMAX and Charizard VMAX (Shining Fates), Bede, Hop, Kabu, Milo,
    /// Piers, Great Ball, Poké Ball, Potion, Pokémon Center Lady, Professor's Research and Turffield Stadium.
    /// Pokémon whose attacks only do their printed damage need no script (Kakuna, Carvanha, Hatenna, Hattrem, Ekans, Arbok,
    /// Absol, Scraggy, Swablu).
    /// </summary>
    public static class CardsCPA
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("CPA-1", new ComboMon(new PollenBombMon(), new LockAfterAttackMon("Solar Typhoon", true)));
            r.RegisterByKey("CPA-2", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("CPA-4", new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned));
            r.RegisterByKey("CPA-5", new EldegossV());
            // Fire
            r.RegisterByKey("CPA-6", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("CPA-7", new DrawAttackMon("Quick Draw", 1));
            r.RegisterByKey("CPA-8", new ComboMon(new GrandFlameMon(), new RecoilMon("Flare Blitzer", 30)));
            r.RegisterByKey("CPA-10", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("CPA-79", new CardsTWM1.SelfDiscardMon("Fire Spin", 2));
            // Water
            r.RegisterByKey("CPA-12", new CardsMEG.SnipeBenchMon("Aqua Jet", 20));
            r.RegisterByKey("CPA-13", new ComboMon(new DrawUpMon(), new CoinsMon("Ocean Waves", 3)));
            r.RegisterByKey("CPA-14", new CardsFST1.TakeLessMon(30, new NoRetreatMon("Powerful Bite")));
            r.RegisterByKey("CPA-15", new CardsFST1.TakeLessMon(30, new FlipBonusMon("G-Max Headbutt", 80)));
            // Psychic
            r.RegisterByKey("CPA-16", new SwellingPulseMon());
            r.RegisterByKey("CPA-17", new CardsTWM1.HealSelfMon("Max Cure", 50));
            r.RegisterByKey("CPA-20", new HattereneCPA());
            r.RegisterByKey("CPA-21", new GalarianCursolaV());
            // Fighting
            r.RegisterByKey("CPA-24", new FlipBonusMon("Steady Punch", 40));
            r.RegisterByKey("CPA-25", new FlipBonusMon("Pummel", 70));
            r.RegisterByKey("CPA-26", new ComboMon(new CardsASR1.TimesMon("Macho Revenge", FightingPokemonInDiscard), new RecoilMon("Dynamite Punch", 50)));
            r.RegisterByKey("CPA-27", new ComboMon(new CardsMEG.SnipeBenchMon("Aura Sphere", 20), new LockAfterAttackMon("Beatdown Smash", true)));
            r.RegisterByKey("CPA-28", new CardsASC.SelfDiscardTypeMon("Power Blast", EnergyType.Fighting, 1));
            r.RegisterByKey("CPA-29", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("CPA-30", new LockAfterAttackMon("Slashing Strike", true));
            r.RegisterByKey("CPA-32", new ComboMon(new CardsEVS1.DefenderCantAttackMon("Tie Up", IsBasicPokemonCard), new FlipBonusMon("Moonsault Press", 100)));
            // Darkness
            r.RegisterByKey("CPA-35", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("CPA-36", new RecoilMon("Double-Edge", 20));
            r.RegisterByKey("CPA-37", new GalarianObstagoonCPA());
            r.RegisterByKey("CPA-39", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("CPA-40", new LimberMon());
            r.RegisterByKey("CPA-42", new ComboMon(new NoRetreatMon("Corner"), new CardsBRS1.SupporterBonusMon("Bad Brawl", "Piers", 90)));
            r.RegisterByKey("CPA-43", new CardsFST2.ConditionBonusMon("Venoshock", 50, DefendingPoisoned));
            r.RegisterByKey("CPA-44", new CardsFST2.ConditionBonusMon("Attack from Behind", 50, DefendingConfused));
            r.RegisterByKey("CPA-45", new ComboMon(new ConditionAttackMon("Eerie Wave", SpecialCondition.Confused), new CardsWHT.FlipMoreMon("Random Peck", 2, 40)));
            r.RegisterByKey("CPA-46", new DrawAttackMon("Filch", 1));
            // Metal
            r.RegisterByKey("CPA-47", new CardsFST1.TakeLessMon(30, new CardsMEG.EnergyBonusMon("Gatling Slug", EnergyType.Metal, 40)));
            // Colorless
            r.RegisterByKey("CPA-49", new AltariaCPA());

            // Trainers
            r.RegisterByName("Full Heal", new FullHeal());
            r.RegisterByName("Hyper Potion", new HyperPotion());
            r.RegisterByName("Marnie", new Marnie());
            r.RegisterByName("Rotom Bike", new RotomBike());
            r.RegisterByName("Rotom Phone", new RotomPhone());
            r.RegisterByName("Sonia", new Sonia());
            r.RegisterByName("Suspicious Food Tin", new SuspiciousFoodTin());
            r.RegisterByName("Team Yell Grunt", new TeamYellGrunt());
        }

        private static bool IsBasicPokemonCard(CardDef d) { return d.IsBasicPokemon; }
        private static bool DefendingPoisoned(EffectContext ctx) { return ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Poisoned); }
        private static bool DefendingConfused(EffectContext ctx) { return ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Confused); }
        private static int FightingPokemonInDiscard(EffectContext ctx) { return ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Fighting)); }

        /// <summary>An Energy attached to the Pokémon that provides this type (a [P] Energy for Suspicious Food Tin).</summary>
        private static bool Provides(EffectContext ctx, PokemonInPlay mon, CardInstance energy, EnergyType type)
        {
            return ctx.Engine.EnergyUnitsOf(mon, energy).Any(u => (u & EnergyMask.Of(type)) != 0);
        }

        // ================================================================== Pokémon

        /// <summary>Pollen Bomb: the opponent's Active Pokémon is now Asleep and Poisoned.</summary>
        private sealed class PollenBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pollen Bomb") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>
        /// Happy Match: when you play this Pokémon from your hand onto your Bench during your turn, you may put a Supporter card
        /// from your discard pile into your hand. Float Up: you may shuffle this Pokémon and all attached cards into your deck.
        /// </summary>
        private sealed class EldegossV : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool ready = ctx.Me.Discard.Any(c => c.Def.IsSupporter);
                if (!TriggerOnce(ctx, "Happy Match", ready, "Happy Match: put a Supporter card from your discard pile into your hand?")) return;
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                var pick = ctx.ChooseCards(supporters, 1, 1, "Happy Match: put which Supporter card into your hand?");
                ctx.DiscardToHand(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Float Up") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.AskYesNo("Float Up: shuffle " + me.Name + " and all attached cards into your deck?"))
                    ctx.Engine.ShufflePokemonIntoDeck(me);
                return true;
            }
        }

        /// <summary>Grand Flame: attach up to 2 [R] Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class GrandFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Flame") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList();
                if (pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Grand Flame: attach Fire Energy from your discard pile to which of your Benched Pokémon?");
                if (target == null) return true;
                var picks = ctx.ChooseCards(pile, 0, Math.Min(2, pile.Count), "Grand Flame: attach up to 2 Fire Energy cards to " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Draw Up: attach up to 3 [W] Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class DrawUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draw Up") return false;
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (pile.Count == 0 || !ctx.InPlay(me)) return true;
                var picks = ctx.ChooseCards(pile, 0, Math.Min(3, pile.Count), "Draw Up: attach up to 3 Water Energy cards to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Swelling Pulse: 80 more damage if this Pokémon was healed during this turn.</summary>
        private sealed class SwellingPulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swelling Pulse") return false;
                bool healed = ctx.Pokemon != null && ctx.Pokemon.HealedOnTurn == ctx.Turn;
                ctx.DamageDefending(atk.Damage + (healed ? 80 : 0));
                return true;
            }
        }

        /// <summary>
        /// Hazard Sensor: if this Pokémon is in the Active Spot and is damaged by an attack from the opponent's Pokémon (even if it's
        /// Knocked Out), the Attacking Pokémon is now Confused. Life Sucker: heal 30 from this Pokémon.
        /// </summary>
        private sealed class HattereneCPA : Mon
        {
            private static readonly Mon LifeSucker = new CardsTWM1.HealSelfMon("Life Sucker", 30);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Hazard Sensor");
                ctx.ApplyCondition(attacker, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return LifeSucker.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Gnawing Aura: while this Pokémon is Active, whenever the opponent attaches an Energy card from their hand to 1 of their
        /// Pokémon, 3 damage counters on that Pokémon (by any means: their attacks and Abilities too). Hollow Missile: 3 damage
        /// counters on the opponent's Benched Pokémon in any way you like.
        /// </summary>
        private sealed class GalarianCursolaV : Mon
        {
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target == null || target.Owner == src.Player || !src.PokemonIsActive) return;
                src.Engine.Emit(GameEventType.UseAbility, src.Player, src.Engine.PlayerName(src.Player) + "'s " + me.Name + ": Gnawing Aura.", src.Card.Id, me.Uid);
                src.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hollow Missile") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.PlaceCountersAnyWay(3, () => ctx.OpponentBench());
                return true;
            }
        }

        /// <summary>Wicked Ruler: once during your turn, your opponent discards cards from their hand until they have 4. Knuckle Impact: can't attack next turn.</summary>
        private sealed class GalarianObstagoonCPA : Mon
        {
            private static readonly Mon KnuckleImpact = new LockAfterAttackMon("Knuckle Impact", false);

            protected override string Activated { get { return "Wicked Ruler"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 4; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                int n = hand.Count - 4;
                if (n <= 0) return;
                var picks = ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, "Wicked Ruler: discard " + n + (n == 1 ? " card" : " cards") + " from your hand (you keep 4)", ctx.Source);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return KnuckleImpact.HandleAttack(ctx, atk); }
        }

        /// <summary>Limber, Pika Shield: this Pokémon can't be Paralyzed.</summary>
        internal sealed class LimberMon : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && condition == SpecialCondition.Paralyzed;
            }
        }

        /// <summary>Miraculous Charm: prevent all damage done to this Pokémon by attacks from the opponent's Pokémon V and Pokémon-GX.</summary>
        private sealed class AltariaCPA : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && CardsEVS1.IsVOrGx(info.Attacker.Def);
            }
        }

        // ================================================================== Trainers

        /// <summary>Full Heal: your Active Pokémon recovers from all Special Conditions.</summary>
        private sealed class FullHeal : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.Conditions != SpecialCondition.None; }
            public override void ResolveTrainer(EffectContext ctx) { if (ctx.Me.Active != null) ctx.Engine.RemoveAllConditions(ctx.Me.Active); }
        }

        /// <summary>
        /// Hyper Potion: heal 120 damage from 1 of your Pokémon with at least 2 Energy attached; if any was healed, discard 2
        /// Energy from it. Compendium (2020-02-06): it needs a Pokémon with damage and 2 Energy.
        /// </summary>
        private sealed class HyperPotion : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Count >= 2 && m.DamageCounters > 0); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Hyper Potion: heal 120 damage from which of your Pokémon?");
                if (ctx.Heal(target, 120) > 0)
                    ctx.DiscardEnergyFrom(target, 2, "Hyper Potion: discard 2 Energy from " + target.Name);
            }
        }

        /// <summary>
        /// Marnie: each player shuffles their hand and puts it on the bottom of their deck; if either player put any cards there,
        /// you draw 5 cards and your opponent draws 4. Milotic's Dew Guard keeps the opponent's hand out of it (Compendium).
        /// </summary>
        private sealed class Marnie : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool any = false;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    if (p.Hand.Count == 0) continue;
                    if (player != ctx.Player && TefPokemon.OpponentShielded(ctx)) continue;
                    var hand = p.Hand.ToList();
                    ctx.State.Rng.Shuffle(hand);
                    p.Hand.Clear();
                    p.Deck.AddRange(hand);
                    any = true;
                    ctx.Engine.Log(player, ctx.Engine.PlayerName(player) + " put " + hand.Count + (hand.Count == 1 ? " card" : " cards") + " on the bottom of their deck (Marnie).");
                }
                if (!any) return;
                ctx.Engine.DrawCards(ctx.Player, 5);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
            }
        }

        /// <summary>Rotom Bike: draw cards until you have 6 cards in your hand; your turn ends.</summary>
        private sealed class RotomBike : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count <= 6 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.DrawUntil(6);
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Rotom Phone: look at the top 5 cards of your deck, choose 1, shuffle the others back, and put the chosen card on top.</summary>
        private sealed class RotomPhone : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                var top = deck.Take(5).ToList();
                if (top.Count == 0) return;
                var pick = ctx.ChooseCards(top, 1, 1, "Rotom Phone: choose the card to put on top of your deck (the others are shuffled back)");
                var chosen = pick.Count > 0 ? pick[0] : top[0];
                deck.Remove(chosen);
                ctx.ShuffleMyDeck();
                deck.Insert(0, chosen);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card on top of " + ctx.Engine.Their(ctx.Player) + " deck (Rotom Phone).");
            }
        }

        /// <summary>Sonia: search your deck for up to 2 Basic Pokémon or up to 2 basic Energy cards (not a mix: Compendium 2020-04-30).</summary>
        private sealed class Sonia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var opts = new List<DecisionOption>
                {
                    new DecisionOption { Label = "Up to 2 Basic Pokémon", Index = 0 },
                    new DecisionOption { Label = "Up to 2 basic Energy cards", Index = 1 }
                };
                int choice = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Sonia: search your deck for Basic Pokémon or for basic Energy cards?", opts, ctx.Source, false);
                if (choice == 0) ctx.SearchToHand(IsBasicPokemonCard, 2, "Sonia: choose up to 2 Basic Pokémon");
                else ctx.SearchToHand(IsBasicEnergyCard, 2, "Sonia: choose up to 2 basic Energy cards");
            }

            private static bool IsBasicEnergyCard(CardDef d) { return d.IsEnergy && d.IsBasicEnergy; }
        }

        /// <summary>Suspicious Food Tin: heal 80 damage from 1 of your Pokémon with any [P] Energy attached; if any was healed, discard a [P] Energy from it.</summary>
        private sealed class SuspiciousFoodTin : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && m.Energy.Any(e => Provides(ctx, m, e, EnergyType.Psychic)));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Suspicious Food Tin: heal 80 damage from which of your Pokémon?");
                if (ctx.Heal(target, 80) > 0)
                    ctx.DiscardEnergyFrom(target, 1, "Suspicious Food Tin: discard a Psychic Energy from " + target.Name, e => Provides(ctx, target, e, EnergyType.Psychic));
            }
        }

        /// <summary>Team Yell Grunt: put an Energy attached to 1 of your opponent's Pokémon into their hand.</summary>
        private sealed class TeamYellGrunt : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.OpponentPokemon().Any(m => m.Energy.Count > 0); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var withEnergy = ctx.OpponentPokemon().Where(m => m.Energy.Count > 0).ToList();
                if (withEnergy.Count == 0) return;
                var target = ctx.ChoosePokemon(withEnergy, "Team Yell Grunt: choose 1 of your opponent's Pokémon");
                var energy = ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Team Yell Grunt: put which Energy into your opponent's hand?");
                foreach (var e in energy) ctx.Engine.ReturnEnergyToHand(target, e);
            }
        }
    }
}
