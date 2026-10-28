using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Lost Thunder (LOT, v81), part 2: Fire and Water (LOT 38-70). "A [T] Energy card" outside play is basic Energy only (your
    /// ruling, v65). Primarina's Harmonics puts both Energy cards on the same Pokémon (your ruling, v81).
    /// </summary>
    public static class CardsLOT2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fire
            r.RegisterByKey("LOT-38", new AssistingHeaterMon());
            r.RegisterByKey("LOT-39", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("LOT-41", new ConditionAttackMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("LOT-42", new TyphlosionLOT());
            r.RegisterByKey("LOT-43", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("LOT-44", new MagcargoGX());
            r.RegisterByKey("LOT-45", new TeamHuntMon());
            r.RegisterByKey("LOT-46", new ComboMon(new AnyCardSearchMon("Nasty Plot"), new CardsFST2.ConditionBonusMon("Attack Operation", 80, c => c.Me.Hand.Count > c.Opp.Hand.Count)));
            r.RegisterByKey("LOT-47", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new EruptionMon()));
            r.RegisterByKey("LOT-48", new ComboMon(new Cards30CC.BenchHitMon("Lava Burn", 30), new CardsBRS1.MillSelfMon("Heat Bazooka", 5)));
            r.RegisterByKey("LOT-49", new CardsASR1.TimesMon("V-Beatdown", c => c.MyPokemon(m => m.IsBasic).Count));
            r.RegisterByKey("LOT-50", new LitleoLOT());
            r.RegisterByKey("LOT-51", new CardsDRI2.CrunchMon("Crunch"));
            r.RegisterByKey("LOT-52", new BlacephalonGX());
            // Water
            r.RegisterByKey("LOT-53", new AlolanVulpixLOT());
            r.RegisterByKey("LOT-55", new ComboMon(new MemoryMeltMon(), new CardsASR1.PlusMon("Psychic", 20, DefendingEnergy)));
            r.RegisterByKey("LOT-56", new LaprasLOT());
            r.RegisterByKey("LOT-57", new HappyDeliveryMon());
            r.RegisterByKey("LOT-58", new MantineLOT());
            r.RegisterByKey("LOT-59", new SuicuneLOT());
            r.RegisterByKey("LOT-60", new SuicuneGX());
            r.RegisterByKey("LOT-61", new SecondaryChillsMon());
            r.RegisterByKey("LOT-62", new ComboMon(new CardsFST2.ConditionBonusMon("Resolute Claws", 60, c => c.Defending != null && c.Defending.Def.IsGXorEX), new LockAfterAttackMon("Blizzard Burn", false)));
            r.RegisterByKey("LOT-63", new ComboMon(new CardsCRE2.FieldCrushMon(), new CardsFST2.ConditionBonusMon("Freezing Flames", 80, c => c.Pokemon != null && c.Engine.HasEnergyOfType(c.Pokemon, EnergyType.Fire))));
            r.RegisterByKey("LOT-64", new CardsPAR2.FlipConditionMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("LOT-65", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("LOT-66", new CardsDAA1.GustAttackMon("Captivate", false));
            r.RegisterByKey("LOT-67", new PrimarinaLOT());
            r.RegisterByKey("LOT-68", new CoinsMon("Spike Cannon", 2));
            r.RegisterByKey("LOT-69", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new VenomFeverMon()));
            r.RegisterByKey("LOT-70", new ComboMon(new ConditionAttackMon("Mind Bend", SpecialCondition.Confused), new FollowTheWoundMon()));
        }

        /// <summary>Energy attached to the opponent's Active Pokémon (Psychic, Energy Press, Intercept-GX).</summary>
        internal static int DefendingEnergy(EffectContext ctx) { return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending); }

        /// <summary>"Search your deck for a card and put it into your hand" (Nasty Plot): the card isn't shown to the opponent.</summary>
        internal sealed class AnyCardSearchMon : Mon
        {
            private readonly string attack;

            public AnyCardSearchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, attack + ": choose a card to put into your hand", false);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Assisting Heater: you may attach a Fire Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class AssistingHeaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assisting Heater") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var fire = ctx.Me.Hand.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return true;
                var active = ctx.Me.Active;
                ctx.AttachCardsAnyWay(fire, 1, (c, m) => m != active, "Assisting Heater: attach a Fire Energy card from your hand to 1 of your Benched Pokémon");
                return true;
            }
        }

        /// <summary>
        /// Blazing Energy: once during your turn, all Energy attached to your Pokémon are Fire Energy instead of their usual type
        /// until the end of the turn (cards that come into play this turn too). Each unit an Energy provides becomes 1 Fire.
        /// Lost Flame: put 2 Energy attached to the opponent's Active Pokémon in the Lost Zone.
        /// </summary>
        private sealed class TyphlosionLOT : Mon
        {
            protected override string Activated { get { return "Blazing Energy"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return !ctx.Engine.ActiveEffects().Any(e => e is BlazingEnergyEffect && e.Owner == ctx.Player);
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AddEffect(new BlazingEnergyEffect("All Energy attached to " + ctx.Engine.PlayerName(ctx.Player) + "'s Pokémon are Fire Energy this turn (Blazing Energy)", ctx.Player, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Flame") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Energy.Count == 0) return true;
                var energy = defending.Energy.ToList();
                var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Lost Flame: choose 2 Energy attached to " + defending.Name + " to put in the Lost Zone");
                foreach (var card in picks) CardsLOR1.EnergyToLostZone(ctx, defending, card);
                return true;
            }
        }

        internal sealed class BlazingEnergyEffect : ContinuousEffect
        {
            public BlazingEnergyEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override void ModifyProvidedEnergy(GameEngine engine, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder.Owner != Owner) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }
        }

        /// <summary>
        /// Crushing Charge: once during your turn, discard the top card of your deck; if it's a basic Energy card, attach it to 1 of
        /// your Pokémon. Lava Flow: discard any amount of basic Energy from this Pokémon, 50 more damage for each. Burning
        /// Magma-GX: discard the top 5 cards of the opponent's deck.
        /// </summary>
        private sealed class MagcargoGX : Mon
        {
            protected override string Activated { get { return "Crushing Charge"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                if (discarded.Count == 0) return;
                var card = discarded[0];
                if (!card.Def.IsBasicEnergy || !ctx.Me.Discard.Contains(card)) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Crushing Charge: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.AttachEnergyCard(card, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lava Flow")
                {
                    var me = ctx.Pokemon;
                    var basics = me.Energy.Where(c => c.Def.IsBasicEnergy).ToList();
                    int n = 0;
                    if (basics.Count > 0)
                    {
                        var picks = ctx.ChooseCards(basics, 0, basics.Count, "Lava Flow: discard any amount of basic Energy from " + me.Name + " (50 more damage each)");
                        foreach (var card in picks)
                            if (ctx.Engine.DiscardEnergy(me, card)) n++;
                    }
                    ctx.DamageDefending(atk.Damage + 50 * n);
                    return true;
                }
                if (atk.Name == "Burning Magma-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Opp.Deck.Count == 0) return true;
                    if (ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Log("The top cards of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck can't be discarded right now.");
                    else ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 5);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Team Hunt: draw a card for each of your Houndour in play.</summary>
        private sealed class TeamHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Team Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.MyPokemon(m => m.Name == "Houndour").Count;
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Eruption: each player discards the top card of their deck; 60 more damage for each Energy card discarded.</summary>
        internal sealed class EruptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eruption") return false;
                int energy = 0;
                if (ctx.Me.Deck.Count > 0) energy += ctx.Engine.DiscardTopOfDeck(ctx.Player, 1).Count(c => c.Def.IsEnergy);
                if (ctx.Opp.Deck.Count > 0)
                {
                    if (ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Log("The top card of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck can't be discarded right now.");
                    else energy += ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1).Count(c => c.Def.IsEnergy);
                }
                ctx.DamageDefending(atk.Damage + 60 * energy);
                return true;
            }
        }

        /// <summary>Wild Dash: if your opponent has any Pokémon-GX or Pokémon-EX in play, this Pokémon has no Retreat Cost. Take Down: 10 to itself.</summary>
        private sealed class LitleoLOT : Mon
        {
            private static readonly Mon TakeDown = new RecoilMon("Take Down", 10);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return src.Opp.InPlay().Any(m => m.Def.IsGXorEX) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TakeDown.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Bursting Burn: Burned and Confused. Mind Blown: put any amount of Fire Energy attached to your Pokémon in the Lost Zone,
        /// 50 damage for each (counted, and the damage done, before they leave). Burst-GX: discard 1 of your Prize cards; if it's an Energy card, attach it to 1 of your Pokémon.
        /// </summary>
        private sealed class BlacephalonGX : Mon
        {
            private static readonly Mon Burn = new CardsCES1.ConditionsMon("Bursting Burn", SpecialCondition.Burned, SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Burn.HandleAttack(ctx, atk)) return true;
                if (atk.Name == "Mind Blown")
                {
                    var holders = new Dictionary<int, PokemonInPlay>();
                    var fire = new List<CardInstance>();
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var card in mon.Energy)
                            if (CardsTEU1.ProvidesType(ctx, mon, card, EnergyType.Fire))
                            {
                                fire.Add(card);
                                holders[card.Id] = mon;
                            }
                    // The Energy is chosen and the damage done while it's still attached, then it goes to the Lost Zone
                    // (Compendium: Mind Blown + Beast Energy, 2018-11-01). Choosing none is allowed.
                    var picks = fire.Count > 0
                        ? ctx.ChooseCards(fire, 0, fire.Count, "Mind Blown: put any amount of Fire Energy attached to your Pokémon in the Lost Zone (" + atk.Damage + " damage each)")
                        : new List<CardInstance>();
                    if (picks.Count > 0) ctx.DamageDefending(atk.Damage * picks.Count);
                    else ctx.Log("Mind Blown did no damage.");
                    foreach (var card in picks)
                    {
                        PokemonInPlay holder;
                        if (!holders.TryGetValue(card.Id, out holder) || !holder.Energy.Contains(card)) continue;
                        CardsLOR1.EnergyToLostZone(ctx, holder, card);
                    }
                    return true;
                }
                if (atk.Name == "Burst-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    BurstPrize(ctx);
                    return true;
                }
                return false;
            }

            /// <summary>Discards 1 Prize card (a face-down one at random: the player can't tell them apart); attaches it if it's Energy.</summary>
            private static void BurstPrize(EffectContext ctx)
            {
                var p = ctx.Me;
                if (p.Prizes.Count == 0) return;
                var faceUp = p.Prizes.Where(c => p.FaceUpPrizeIds.Contains(c.Id)).ToList();
                var faceDown = p.Prizes.Where(c => !p.FaceUpPrizeIds.Contains(c.Id)).ToList();
                CardInstance card;
                if (faceUp.Count == 0) card = faceDown[ctx.State.Rng.Next(faceDown.Count)];
                else
                {
                    var labels = faceUp.Select(c => "Discard the face-up " + c.Def.Name).ToList();
                    if (faceDown.Count > 0) labels.Add("Discard a face-down Prize card");
                    int pick = ctx.ChooseOption("Burst-GX: discard which Prize card?", labels.ToArray());
                    card = pick < faceUp.Count ? faceUp[pick] : faceDown[ctx.State.Rng.Next(faceDown.Count)];
                }
                p.Prizes.Remove(card);
                p.FaceUpPrizeIds.Remove(card.Id);
                p.PrizeSlotByCard.Remove(card.Id);
                p.PrizesDiscarded++;
                p.Discard.Add(card);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded a Prize card (" + card.Def.Name + ") with Burst-GX (" + p.Prizes.Count + " left).", card.Id);
                if (!card.Def.IsEnergy || !p.Discard.Contains(card) || !p.HasPokemonInPlay) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Burst-GX: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target == null || !p.Discard.Remove(card)) return;
                ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        // ================================================================== Water

        /// <summary>Secret Alleyway: if you have any Fairy Pokémon in play, this Pokémon has no Retreat Cost.</summary>
        private sealed class AlolanVulpixLOT : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return src.Me.InPlay().Any(m => src.Engine.IsType(m, EnergyType.Fairy)) ? 0 : cost;
            }
        }

        /// <summary>Memory Melt: look at your opponent's hand and put a card you find there in the Lost Zone.</summary>
        private sealed class MemoryMeltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Memory Melt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.NoteLookedAt(hand, ctx.Engine.PlayerName(ctx.Opponent) + "'s hand", ctx.Player);
                var card = ctx.ChooseCards(hand, 1, 1, "Memory Melt: choose a card from your opponent's hand to put in the Lost Zone")[0];
                if (!ctx.Opp.Hand.Remove(card)) return true;
                CardsCRZ1.PutInLostZone(ctx, card);
                return true;
            }
        }

        /// <summary>Go for a Swim: once during your turn, look at the top 2 cards of your deck and put them back in any order. Ice Beam: flip, Paralyzed.</summary>
        private sealed class LaprasLOT : Mon
        {
            private static readonly Mon Beam = new ParalyzeFlipMon("Ice Beam");

            protected override string Activated { get { return "Go for a Swim"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { CardsOBF2.ReorderOwnTop(ctx, 2, "Go for a Swim"); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Beam.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Happy Delivery: choose any number of your Benched Pokémon without a Pokémon Tool; for each, search your deck for a Pokémon
        /// Tool card and attach it to that Pokémon. Then shuffle your deck.
        /// </summary>
        private sealed class HappyDeliveryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Happy Delivery") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bare = ctx.MyBench().Where(m => m.Tools.Count == 0 && ctx.Engine.CanTakeTool(m)).ToList();
                if (bare.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var chosen = ctx.ChoosePokemonMany(bare, 0, bare.Count, "Happy Delivery: choose any of your Benched Pokémon without a Pokémon Tool");
                foreach (var mon in chosen)
                {
                    if (!ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) continue;
                    var holder = mon;
                    var found = ctx.SearchMyDeck(d => d.IsTool, 1, "Happy Delivery: choose a Pokémon Tool for " + holder.Name);
                    if (found.Count == 0) continue;
                    var tool = found[0];
                    if (ctx.Engine.CanAttachToolTo(tool, holder)) ctx.Engine.AttachTool(tool, holder);
                    else ctx.Me.Deck.Add(tool);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Mantine Surf: if this Pokémon has any Energy attached, it has no Retreat Cost.</summary>
        private sealed class MantineLOT : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return mon.Energy.Count > 0 ? 0 : cost;
            }
        }

        /// <summary>Frozen Flow: once during your turn, if this is your Active Pokémon, you may have your opponent switch out their Active Pokémon. Aurora Gain: heal 30.</summary>
        private sealed class SuicuneLOT : Mon
        {
            private static readonly Mon Gain = new CardsTWM1.HealSelfMon("Aurora Gain", 30);

            protected override string Activated { get { return "Frozen Flow"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Gain.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Phantom Winds: once during your turn, if this Pokémon is on your Bench, you may shuffle it and all attached cards into your
        /// deck. Cure Stream: the Defending Pokémon's attacks do 30 less damage next turn. Brinicles-GX: switch with a Benched Pokémon.
        /// </summary>
        private sealed class SuicuneGX : Mon
        {
            private static readonly Mon Cure = new CardsMEG.WeakenDefendingMon("Cure Stream", 30);
            private static readonly Mon Brinicles = new SwitchSelfMon("Brinicles-GX", false);

            protected override string Activated { get { return "Phantom Winds"; } }
            protected override bool AbilityReady(EffectContext ctx) { return !ctx.PokemonIsActive; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                return Cure.HandleAttack(ctx, atk) || Brinicles.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Secondary Chills: only if you went second, during your first turn: Paralyzed.</summary>
        private sealed class SecondaryChillsMon : Mon
        {
            private static readonly Mon Chills = new ConditionAttackMon("Secondary Chills", SpecialCondition.Paralyzed);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Secondary Chills") return true;
                return CardsUNB1.SecondPlayersFirstTurn(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Chills.HandleAttack(ctx, atk); }
        }

        /// <summary>Harmonics: the Energy attachment from your hand each turn may be 2 Energy cards (on the same Pokémon) instead of 1. Hypno Splash: Asleep.</summary>
        private sealed class PrimarinaLOT : Mon
        {
            private static readonly Mon Splash = new ConditionAttackMon("Hypno Splash", SpecialCondition.Asleep);

            public override bool AllowsSecondEnergyAttachment(EffectContext src) { return src.Pokemon != null; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Splash.HandleAttack(ctx, atk); }
        }

        /// <summary>Venom Fever: if the opponent's Active Pokémon is Poisoned, 50 damage for each damage counter on it (otherwise nothing).</summary>
        private sealed class VenomFeverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Venom Fever") return false;
                var defending = ctx.Defending;
                if (defending == null || !defending.Has(SpecialCondition.Poisoned) || defending.DamageCounters == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * defending.DamageCounters);
                return true;
            }
        }

        /// <summary>Follow the Wound: 60 damage to 1 of the opponent's Benched Pokémon that has any damage counters on it.</summary>
        private sealed class FollowTheWoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Follow the Wound") return false;
                if (ctx.DamageOneBenched(60, m => m.DamageCounters > 0) == null) ctx.Log("Follow the Wound: none of your opponent's Benched Pokémon has damage counters on it.");
                return true;
            }
        }
    }
}
