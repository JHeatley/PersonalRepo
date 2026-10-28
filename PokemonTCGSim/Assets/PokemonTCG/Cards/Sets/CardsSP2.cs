using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// SWSH Black Star Promos (SP), the new cards from SWSH138 on: the five Pokémon V-UNION (Pikachu, Greninja, Mewtwo, Zacian,
    /// Morpeko), Champions Festival and the later promos, plus the numbers Limitless leaves out (Special Delivery Bidoof,
    /// Galarian Obstagoon 193, Bulbasaur 303, Arcanine 304, Greninja 305; your call, v67).
    /// V-UNION: every piece carries the whole Pokémon's attacks and Abilities, so all 4 piece keys share one script; the engine
    /// combines 4 different pieces from the discard pile, each V-UNION once per game (GameEngine.CombineVUnion).
    /// </summary>
    public static class CardsSP2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SP-138", new CardsFST1.TwoBenchedMon("Berserker Blade", 20));
            RegisterUnion(r, new ComboMon(new CardsSP1.AttachUpToFromDiscardToSelfMon("Union Gain", EnergyType.Lightning, 2), new ParalyzeFlipMon("Shocking Shock"),
                new CardsDAA1.ItemLockAttackMon("Disconnect")), 139, 140, 141, 142);
            r.RegisterByKey("SP-143", new RecoilMon("Volt Tackle", 30));
            r.RegisterByKey("SP-144", new GreninjaStarSP());
            r.RegisterByKey("SP-153", new ParalyzeFlipMon("Thunder Shock"));
            RegisterUnion(r, new GreninjaVUnionSP(), 155, 156, 157, 158);
            RegisterUnion(r, new MewtwoVUnionSP(), 159, 160, 161, 162);
            RegisterUnion(r, new ComboMon(new CardsSP1.AttachUpToFromDiscardToSelfMon("Union Gain", EnergyType.Metal, 2),
                new CardsMEG.WeakenDefendingMon("Dance of the Crowned Sword", 150), new CardsTWM1.SelfDiscardMon("Master Blade", 3)), 163, 164, 165, 166);
            r.RegisterByKey("SP-174", new ComboMon(new ParalyzeFlipMon("Psy Bolt"), new CardsMEG.DefendingEnergyBonusMon("Psychic", 40)));
            r.RegisterByKey("SP-177", new ComboMon(new SearchAttackMon("Happy Delivery", IsItemCard, 1), new FlipBonusMon("Rock Smash", 30)));
            r.RegisterByKey("SP-189", new ComboMon(new FlightUpMon(), new ConditionAttackMon("Corrosive Acid", SpecialCondition.Burned)));
            r.RegisterByKey("SP-191", new ComboMon(new ShieldAttackMon("Leaf Guard", 30), new CardsOBF1.RetreatCostDamageMon("Grass Knot", 30, false)));
            r.RegisterByKey("SP-192", new ComboMon(new CardsCRZ1.SpreadAllMon("Hail"), new LockAfterAttackMon("Frosty Typhoon", true)));
            r.RegisterByKey("SP-193", new RampagingKickMon());
            r.RegisterByKey("SP-210", new MixedCallMon());
            r.RegisterByKey("SP-211", new ComboMon(new CardsRCL1.MayReturnDefendingEnergyMon("Time Out Kick"), new CardsASR1.PlusMon("Symphony Whip", 70, SupporterPlayed)));
            r.RegisterByKey("SP-214", new ComboMon(new CardsCRZ1.VBonusMon("Fighting Knuckle", 120), new AuraStarMon()));
            RegisterUnion(r, new MorpekoVUnionSP(), 215, 216, 217, 218);
            r.RegisterByKey("SP-230", new TwinkleGatheringMon());
            r.RegisterByKey("SP-234", new GiftDeliveryMon());
            r.RegisterByKey("SP-244", new CardsSP1.FlipUntilTailsDrawMon("Flaring Dash"));
            r.RegisterByKey("SP-246", new PursuitClawMon());
            r.RegisterByKey("SP-247", new ComboMon(new LimberUpMon(), new CoinsMon("Double Impact", 2)));
            r.RegisterByKey("SP-252", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Meteor Punch"), new CardsTWM1.SelfDiscardMon("Bright Flame", 2)));
            r.RegisterByKey("SP-257", new ComboMon(new LostHackMon(), new CardsTWM1.SelfDiscardMon("Extreme Current", 1)));
            r.RegisterByKey("SP-266", new CardsMEG.DefendingEnergyBonusMon("Psychic", 30));
            r.RegisterByKey("SP-273", new SubmergeSilentlyMon());
            r.RegisterByKey("SP-275", new ComboMon(new PullingCurrentsMon(), new Cards30CC.BenchHitMon("Aqua Bullet", 20)));
            r.RegisterByKey("SP-285", new LockAfterAttackMon("Pika Drive", false));
            r.RegisterByKey("SP-286", new TailChargeMon());
            r.RegisterByKey("SP-303", new CardsSP1.FlipUntilTailsDrawMon("Shake and Gather"));
            r.RegisterByKey("SP-304", new LockAfterAttackMon("Sun-Drenched Tackle", false));
            r.RegisterByKey("SP-305", new CoordinatedShurikenMon());

            // Trainers
            r.RegisterByName("Champions Festival", new ChampionsFestival());
        }

        private static void RegisterUnion(CardScriptRegistry r, CardScript script, params int[] numbers)
        {
            foreach (int n in numbers) r.RegisterByKey("SP-" + n, script);
        }

        private static bool IsItemCard(CardDef d) { return d.IsItem; }
        private static int SupporterPlayed(EffectContext ctx) { return ctx.Me.SupporterPlayedThisTurn ? 1 : 0; }

        // ================================================================== V-UNION

        /// <summary>
        /// Greninja V-UNION. Ninja Body: effects of Item cards the opponent plays from their hand are prevented on it. Antidote Jutsu:
        /// it can't be Poisoned. Feel the Way: once during your turn, the opponent reveals their hand. Union Gain: up to 2 basic Water
        /// Energy from the discard pile to it. Twister Shuriken: 100 to each of the opponent's Benched Pokémon. Waterfall Bind: the
        /// Defending Pokémon can't retreat.
        /// </summary>
        private sealed class GreninjaVUnionSP : Mon
        {
            private static readonly Mon UnionGain = new CardsSP1.AttachUpToFromDiscardToSelfMon("Union Gain", EnergyType.Water, 2);
            private static readonly Mon WaterfallBind = new NoRetreatMon("Waterfall Bind");

            protected override string Activated { get { return "Feel the Way"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx) { TefPokemon.RevealOpponentHand(ctx); }

            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && trainer != null && trainer.IsItem;
            }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & SpecialCondition.Poisoned) != 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Twister Shuriken")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 100);
                    return true;
                }
                return UnionGain.HandleAttack(ctx, atk) || WaterfallBind.HandleAttack(ctx, atk);
            }
        }

        /// <summary>
        /// Mewtwo V-UNION. Photon Barrier: effects of the opponent's attacks done to it are prevented (not damage). Union Gain: up to 2
        /// basic Psychic Energy. Super Regeneration: heal 200 from it. Psysplosion: 16 damage counters on the opponent's Pokémon in any
        /// way you like.
        /// </summary>
        private sealed class MewtwoVUnionSP : Mon
        {
            private static readonly Mon UnionGain = new CardsSP1.AttachUpToFromDiscardToSelfMon("Union Gain", EnergyType.Psychic, 2);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Super Regeneration")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 200);
                    return true;
                }
                if (atk.Name == "Psysplosion")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    ctx.PlaceCountersAnyWay(16, () => ctx.OpponentPokemon());
                    return true;
                }
                return UnionGain.HandleAttack(ctx, atk);
            }
        }

        /// <summary>
        /// Morpeko V-UNION. Union Gain: up to 2 basic Lightning Energy. All You Can Eat: draw until you have 10 cards. Burst Wheel:
        /// discard all Energy from it; 100 damage for each card discarded.
        /// </summary>
        private sealed class MorpekoVUnionSP : Mon
        {
            private static readonly Mon UnionGain = new CardsSP1.AttachUpToFromDiscardToSelfMon("Union Gain", EnergyType.Lightning, 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "All You Can Eat")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    ctx.DrawUntil(10);
                    return true;
                }
                if (atk.Name == "Burst Wheel")
                {
                    var me = ctx.Pokemon;
                    int discarded = 0;
                    foreach (var e in me.Energy.ToList())
                        if (ctx.Engine.DiscardEnergy(me, e)) discarded++;
                    if (discarded > 0) ctx.DamageDefending(atk.Damage * discarded);
                    else ctx.Log("Burst Wheel did no damage: no Energy was discarded.");
                    return true;
                }
                return UnionGain.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Pokémon

        /// <summary>
        /// Greninja ★ (Unlimited only, Pokémon ★ rule). Shadow Knife (Poké-Power): when you play it from your hand onto your Bench, you
        /// may put 1 damage counter on 1 of the opponent's Pokémon. Mist Slash: its damage ignores Weakness, Resistance and every
        /// effect on the Defending Pokémon.
        /// </summary>
        private sealed class GreninjaStarSP : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Shadow Knife", ctx.OpponentPokemon().Count > 0, "Shadow Knife: put 1 damage counter on 1 of your opponent's Pokémon?")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Shadow Knife: put 1 damage counter on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mist Slash") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        /// <summary>Flight Up: attach up to 3 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class FlightUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flight Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Flight Up: attach basic Energy from your discard pile to which Benched Pokémon?");
                if (target == null) return true;
                var picks = ctx.ChooseCards(energy, 1, Math.Min(3, energy.Count), "Flight Up: attach up to 3 basic Energy cards to " + target.Name);
                foreach (var e in picks)
                    if (ctx.Me.Discard.Remove(e)) ctx.Engine.AttachEnergyCard(e, target);
                return true;
            }
        }

        /// <summary>Rampaging Kick: discard 2 Darkness Energy from this Pokémon after the damage.</summary>
        private sealed class RampagingKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rampaging Kick") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 2, "Rampaging Kick: discard 2 Darkness Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Darkness));
                return true;
            }
        }

        /// <summary>Mixed Call: search your deck for a Pokémon and a Supporter card and put them into your hand.</summary>
        private sealed class MixedCallMon : Mon
        {
            private static bool PokemonCard(CardDef d) { return d.IsPokemon; }
            private static bool SupporterCard(CardDef d) { return d.IsSupporter; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mixed Call") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(PokemonCard, 1, "Mixed Call: search your deck for a Pokémon"));
                found.AddRange(ctx.SearchMyDeck(SupporterCard, 1, "Mixed Call: search your deck for a Supporter card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Aura Star (VSTAR Power): 70 damage for each Energy attached to all of the opponent's Pokémon.</summary>
        private sealed class AuraStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aura Star") return false;
                int energy = ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
                if (energy > 0) ctx.DamageDefending(atk.Damage * energy);
                else ctx.Log("Aura Star did no damage: your opponent's Pokémon have no Energy attached.");
                return true;
            }
        }

        /// <summary>Twinkle Gathering: search your deck for up to as many cards as the different types of your Pokémon in play.</summary>
        private sealed class TwinkleGatheringMon : Mon
        {
            private static bool AnyCard(CardDef d) { return true; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twinkle Gathering") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var types = new HashSet<EnergyType>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var t in ctx.Engine.TypesOf(mon)) types.Add(t);
                if (types.Count > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(AnyCard, types.Count, "Twinkle Gathering: search your deck for up to " + types.Count + " cards");
                return true;
            }
        }

        /// <summary>Gift Delivery: flip a coin; if heads, search your deck for an Item card and put it into your hand.</summary>
        private sealed class GiftDeliveryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gift Delivery") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Me.Deck.Count > 0) ctx.SearchToHand(IsItemCard, 1, "Gift Delivery: search your deck for an Item card");
                return true;
            }
        }

        /// <summary>Pursuit Claw: 20 damage to 1 of the opponent's Benched Pokémon for each damage counter on it (no Weakness or Resistance).</summary>
        private sealed class PursuitClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pursuit Claw") return false;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Pursuit Claw: 20 damage for each damage counter on which of your opponent's Benched Pokémon?");
                if (target != null && target.DamageCounters > 0) ctx.DamageTarget(target, 20 * target.DamageCounters);
                return true;
            }
        }

        /// <summary>Limber Up: attach a basic Energy card from your discard pile to this Pokémon, after the damage.</summary>
        private sealed class LimberUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Limber Up") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || !ctx.InPlay(me)) return true;
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Limber Up: attach which basic Energy card to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Lost Hack: put a Special Energy attached to 1 of the opponent's Pokémon in the Lost Zone.</summary>
        private sealed class LostHackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Hack") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Lost Hack: put a Special Energy from which of your opponent's Pokémon in the Lost Zone?");
                if (target == null) return true;
                var special = target.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                var card = special.Count == 1 ? special[0] : ctx.ChooseCards(special, 1, 1, "Lost Hack: which Special Energy?")[0];
                if (!target.Energy.Remove(card)) return true;
                ctx.State.Players[target.Owner].LostZone.Add(card);
                ctx.Log("Lost Hack: " + card.Def.Name + " attached to " + target.Name + " was put in the Lost Zone.");
                return true;
            }
        }

        /// <summary>
        /// Submerge Silently: usable only if you went second, during your first turn. During the opponent's next turn, prevent all damage
        /// done to this Pokémon by attacks.
        /// </summary>
        private sealed class SubmergeSilentlyMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Submerge Silently") return true;
                return CardsEVS1.WentSecondFirstTurn(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Submerge Silently") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) CardsSP1.FlipNoDamageMon.ProtectFromDamage(ctx, "Submerge Silently");
                return true;
            }
        }

        /// <summary>
        /// Pulling Currents: the opponent reveals their hand and you put up to 2 Basic Pokémon from it onto their Bench. With their Bench
        /// full, nothing happens: you don't look at their hand (Compendium, 2023-06-01).
        /// </summary>
        private sealed class PullingCurrentsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pulling Currents") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int space = ctx.Engine.BenchSpace(ctx.Opponent);
                if (space <= 0)
                {
                    ctx.Log("Pulling Currents: your opponent's Bench is full.");
                    return true;
                }
                TefPokemon.RevealOpponentHand(ctx);
                var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon && ctx.Engine.IsPlayableBasic(c.Def) && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (basics.Count == 0) return true;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(2, Math.Min(space, basics.Count)), "Pulling Currents: put up to 2 Basic Pokémon from your opponent's hand onto their Bench");
                foreach (var card in picks)
                {
                    if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0 || !ctx.Opp.Hand.Remove(card)) continue;
                    ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
                }
                return true;
            }
        }

        /// <summary>Tail Charge: attach up to 3 basic Lightning Energy cards from your discard pile to 1 of your Pokémon, after the damage.</summary>
        private sealed class TailChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail Charge") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Lightning)).ToList();
                if (energy.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Tail Charge: attach Lightning Energy from your discard pile to which of your Pokémon?");
                if (target == null) return true;
                var picks = ctx.ChooseCards(energy, 1, Math.Min(3, energy.Count), "Tail Charge: attach up to 3 Lightning Energy cards to " + target.Name);
                foreach (var e in picks)
                    if (ctx.Me.Discard.Remove(e)) ctx.Engine.AttachEnergyCard(e, target);
                return true;
            }
        }

        /// <summary>Coordinated Shuriken: also 10 damage to 3 of the opponent's Benched Pokémon (fewer if they have fewer).</summary>
        private sealed class CoordinatedShurikenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coordinated Shuriken") return false;
                ctx.DamageDefending(atk.Damage);
                CardsLOR1.DamageBenched(ctx, 10, 3, "Coordinated Shuriken");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>
        /// Champions Festival (Stadium): once during each player's turn, if that player has exactly 6 Pokémon in play (your ruling,
        /// v67), they may heal 10 damage from each of their Pokémon.
        /// </summary>
        private sealed class ChampionsFestival : StadiumAction
        {
            protected override bool Ready(EffectContext ctx)
            {
                var mine = ctx.Me.InPlay().ToList();
                return mine.Count == 6 && mine.Any(m => m.DamageCounters > 0);
            }

            protected override bool Use(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 10);
                return false;
            }
        }
    }
}
