using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Holon Phantoms Rare Pokémon, Pokémon-ex and Pokémon ★ (HP 1-17, 99-104; v170). Unlimited only. Compendium rulings: Crystal
    /// Laser's bonus stays on Deoxys (through Form Change) and only goes to the Defending Pokémon; Delta Reduction (errata) makes
    /// the Defending Pokémon's attacks do 30 less (before W/R) rather than protecting Deoxys; Delta Reactor adds up per Gyarados
    /// and only adds to damage to the Defending Pokémon. 1-on-1: "1 of the Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsHP
    {
        private static readonly string[] OmanyteKabuto = { "Omanyte", "Omastar", "Kabuto", "Kabutops", "Kabutops ex" };
        private static readonly string[] Fossils = { "Claw Fossil", "Mysterious Fossil", "Root Fossil", "Holon Fossil" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HP-1", new ComboMon(new CardsDS.BaseIfMon("Delta Edge", 20, c => c.Me.SupporterThisTurn != null), new FossilChargeMon()));
            r.RegisterByKey("HP-2", new ComboMon(new HarshFluidMon(), new ConditionAttackMon("Poison Tentacles", SpecialCondition.Poisoned)));
            r.RegisterByKey("HP-3", new CardsDX.FormChangeMon("Deoxys", new ReturnTwoEnergyMon("Energy Loop")));
            r.RegisterByKey("HP-4", new CardsDX.FormChangeMon("Deoxys", new CardsMEG.WeakenDefendingMon("Delta Reduction", 30)));
            r.RegisterByKey("HP-5", new CardsDX.FormChangeMon("Deoxys", new CrystalLaserMon()));
            r.RegisterByKey("HP-6", new CardsDX.FormChangeMon("Deoxys", new SwitchSelfMon("Teleportation Burst", false)));
            r.RegisterByKey("HP-7", new FlygonDeltaSupply());
            r.RegisterByKey("HP-8", new GyaradosDeltaReactor());
            r.RegisterByKey("HP-9", new ComboMon(new VitalDrainMon(), new CardsASR1.PlusMon("Thunderous Blow", 10, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Lightning))));
            r.RegisterByKey("HP-10", new KingdraDragonCurse());
            r.RegisterByKey("HP-11", new DualAuraMon("Latios", new DrawAttackMon("Spearhead", 1)));
            r.RegisterByKey("HP-12", new DualAuraMon("Latias", new CardsASC.SelfDiscardTypeMon("Aqua Blast", EnergyType.Water, 1)));
            r.RegisterByKey("HP-13", new ComboMon(new ParalyzeFlipMon("Bind"),
                new CardsASR1.PlusMon("Vengeful Spikes", 10, c => Math.Min(6, c.Me.Discard.Count(x => x.Def.IsPokemon && OmanyteKabuto.Contains(x.Def.Name))))));
            r.RegisterByKey("HP-14", new PidgeotDeltaReserve());
            r.RegisterByKey("HP-15", new ComboMon(new ZzzapMon(), new MetallicThunderMon()));
            r.RegisterByKey("HP-16", new RayquazaHydroBarrier());
            r.RegisterByKey("HP-17", new VileplumePoisonPollen());

            r.RegisterByKey("HP-99", new CrawdauntExSplashBack());
            r.RegisterByKey("HP-100", new MewExPsychicVision());
            r.RegisterByKey("HP-101", new MightyenaExDrivingHowl());
            r.RegisterByKey("HP-102", new ComboMon(new SpiralGrowthMon(), new AllOutBlastMon()));
            r.RegisterByKey("HP-103", new MewtwoStar());
            r.RegisterByKey("HP-104", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CardsFST2.ConditionBonusMon("Spring Back", 50, c => c.Opp.Prizes.Count == 1)));
        }

        // ================================================================== Shared

        /// <summary>"Any Stadium card with Holon in its name is in play."</summary>
        internal static bool HolonStadium(EffectContext ctx) { return ctx.State.Stadium != null && ctx.State.Stadium.Def.Name.Contains("Holon"); }

        /// <summary>A Pokémon with a printed Poké-Power or Poké-Body (Zzzap).</summary>
        internal static bool HasPowerOrBody(PokemonInPlay mon)
        {
            return mon.Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower || a.Kind == AbilityDef.PokeBody);
        }

        /// <summary>"Choose 1 card from your opponent's hand without looking" (Knock Off, Brush Aside, Mind Play), or null.</summary>
        internal static CardInstance RandomOpponentCard(EffectContext ctx)
        {
            var hand = ctx.Opp.Hand;
            return hand.Count == 0 ? null : hand[ctx.State.Rng.Next(hand.Count)];
        }

        /// <summary>"Return 2 Energy attached to this Pokémon to your hand" (Energy Loop).</summary>
        private sealed class ReturnTwoEnergyMon : Mon
        {
            private readonly string attack;

            public ReturnTwoEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (CardsBS1.CopyingWithMetronome || !ctx.InPlay(me) || me.Energy.Count == 0) return true;
                int n = Math.Min(2, me.Energy.Count);
                var picks = me.Energy.Count == n ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), n, n, attack + ": return which 2 Energy cards to your hand?");
                foreach (var card in picks) ctx.Engine.ReturnEnergyToHand(me, card);
                return true;
            }
        }

        // ================================================================== δ Rares

        /// <summary>Fossil Charge: you may discard a Claw, Mysterious, Root or Holon Fossil from your hand for 30 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class FossilChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Charge") return false;
                ctx.DamageDefending(atk.Damage);
                var fossils = ctx.Me.Hand.Where(c => Fossils.Contains(c.Def.Name)).ToList();
                if (fossils.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
                var pick = ctx.ChooseCards(fossils, 0, 1, "Fossil Charge: discard a Fossil from your hand to do 30 damage to 1 of your opponent's Benched Pokémon?");
                if (pick.Count == 0) return true;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.DamageOneBenched(30);
                return true;
            }
        }

        /// <summary>Harsh Fluid: 5 damage counters on the Defending Pokémon at the end of the opponent's next turn.</summary>
        private sealed class HarshFluidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Harsh Fluid") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsTRR.DelayedCountersEffect(defending.Name + " gets 5 damage counters at the end of " + ctx.Engine.Possessive(ctx.Opponent) + " next turn (Harsh Fluid)",
                        ctx.Player, defending.Uid, ctx.Turn, 5, false, "Harsh Fluid"));
                return true;
            }
        }

        /// <summary>Crystal Laser: during your next turn, this Deoxys's attacks do 40 more to the Defending Pokémon (before W/R); gone if Deoxys goes to the Bench.</summary>
        private sealed class CrystalLaserEffect : ContinuousEffect
        {
            public CrystalLaserEffect(string name, int owner, int targetUid, int currentTurn) : base(name, owner, targetUid, currentTurn + 2, true)
            {
                ActiveFromTurn = currentTurn + 2;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target == null || target.Owner == Owner || damage <= 0) return damage;
                return engine.State.IsActive(target) && engine.CurrentSourceKind == EffectSourceKind.Attack ? damage + 40 : damage;
            }
        }

        private sealed class CrystalLaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crystal Laser") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && ctx.PokemonIsActive)
                    ctx.AddEffect(new CrystalLaserEffect(me.Name + "'s attacks do 40 more damage during " + ctx.Engine.Possessive(ctx.Player) + " next turn (Crystal Laser)", ctx.Player, me.Uid, ctx.Turn));
                return true;
            }
        }

        /// <summary>Delta Supply (Poké-Power): once during your turn, a basic Energy or δ Rainbow Energy card from your hand onto 1 of your δ Pokémon. Swift.</summary>
        private sealed class FlygonDeltaSupply : Mon
        {
            private const string Power = "Delta Supply";
            private static readonly Mon Swift = new CardsMA.SwiftMon("Swift");

            private static List<CardInstance> Cards(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy || CardsHPT.IsDeltaRainbow(c.Def)).ToList(); }

            private static List<PokemonInPlay> Targets(EffectContext ctx, CardInstance card)
            {
                return ctx.MyPokemon(m => CardsDS.Delta(m) && !ctx.Engine.EnergyFromHandBlockedTo(card, m));
            }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && Cards(ctx).Any(c => Targets(ctx, c).Count > 0); }

            protected override void Ability(EffectContext ctx)
            {
                var cards = Cards(ctx).Where(c => Targets(ctx, c).Count > 0).ToList();
                if (cards.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": attach which Energy card?")[0];
                var target = ctx.ChoosePokemon(Targets(ctx, card), Power + ": attach " + card.Def.Name + " to which of your δ Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swift.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Reactor (Poké-Body): with a Holon Stadium in play, each of your δ Pokémon does 10 more to the Defending Pokémon (before W/R). Hyper Beam.</summary>
        private sealed class GyaradosDeltaReactor : Mon
        {
            private static readonly Mon Beam = new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (a == null || a.Owner != src.Player || !CardsDS.Delta(a) || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && HolonStadium(src) ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Beam.HandleAttack(ctx, atk); }
        }

        /// <summary>Vital Drain: if the Defending Pokémon is Knocked Out by it, remove all Special Conditions and 7 damage counters from this Pokémon.</summary>
        private sealed class VitalDrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vital Drain") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool knockedOut = defending != null && (!ctx.InPlay(defending) || ctx.RemainingHp(defending) <= 0);
                if (!knockedOut || !ctx.InPlay(me)) return true;
                ctx.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) ctx.Heal(me, 70);
                return true;
            }
        }

        /// <summary>Dragon Curse (Poké-Power): once during your turn, if Kingdra is Active, 2 damage counters on 1 of the opponent's δ Pokémon. Extra Flame.</summary>
        private sealed class KingdraDragonCurse : Mon
        {
            private const string Power = "Dragon Curse";
            private static readonly Mon ExtraFlame = CardsSS.ConditionBonus("Extra Flame", 30, d => d.Def.IsOldEx);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.PokemonIsActive && ctx.OpponentPokemon(CardsDS.Delta).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(CardsDS.Delta), Power + ": put 2 damage counters on which of your opponent's δ Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ExtraFlame.HandleAttack(ctx, atk); }
        }

        /// <summary>Dual Aura (Poké-Body): as long as you have [partner] (or [partner] ex) in play, each player's Evolved Pokémon (not Pokémon-ex) can't use Poké-Bodies.</summary>
        private sealed class DualAuraMon : Mon
        {
            private readonly string partner;
            private readonly Mon attacks;

            /// <summary>Checking that Dual Aura itself works asks the other Poké-Body stoppers again: inside that, it stops nothing.</summary>
            [ThreadStatic] private static bool checking;

            public DualAuraMon(string partner, Mon attacks)
            {
                this.partner = partner;
                this.attacks = attacks;
            }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName)
            {
                var me = src.Pokemon;
                if (checking || me == null || mon == null || mon == me || mon.Def.IsOldEx || !CardsE2.IsEvolved(mon)) return false;
                string p = partner;
                if (!src.Me.InPlay().Any(m => m.Name == p || m.Name == p + " ex")) return false;
                checking = true;
                try { return !src.Engine.AbilityBlocked(me, "Dual Aura"); }
                finally { checking = false; }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Reserve (Poké-Body): with a Holon Energy card on Pidgeot, Pokémon without δ (both players') can't use Poké-Powers. Rotating Claws.</summary>
        private sealed class PidgeotDeltaReserve : Mon
        {
            private static readonly Mon Claws = new CardsTRR.RotatingClawsMon();

            [ThreadStatic] private static bool checking;

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (checking || me == null || mon == null || CardsDS.Delta(mon) || CardsDS.HolonEnergyCount(me) == 0) return false;
                checking = true;
                try { return !src.Engine.AbilityBlocked(me, "Delta Reserve"); }
                finally { checking = false; }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        /// <summary>Zzzap: 20 damage to each Pokémon (both players') that has a Poké-Power or Poké-Body, without Weakness or Resistance.</summary>
        private sealed class ZzzapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zzzap") return false;
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(HasPowerOrBody).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Metallic Thunder: you may discard 2 Metal Energy from this Pokémon for a base damage of 90.</summary>
        private sealed class MetallicThunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metallic Thunder") return false;
                var me = ctx.Pokemon;
                var metal = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Metal)).ToList();
                bool boost = !CardsBS1.CopyingWithMetronome && metal.Count >= 2 && ctx.AskYesNo("Metallic Thunder: discard 2 Metal Energy for 90 damage?");
                if (boost) ctx.DiscardEnergyFrom(me, 2, "Metallic Thunder: discard 2 Metal Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Metal));
                ctx.DamageDefending(boost ? 90 : atk.Damage);
                return true;
            }
        }

        /// <summary>Hydro Barrier (Poké-Body): with a Holon Energy card on Rayquaza, your Water Pokémon have no Weakness. Delta Search.</summary>
        private sealed class RayquazaHydroBarrier : Mon
        {
            private static readonly Mon Search = new CardsDS.DeltaSearchMon();

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Owner == src.Player && CardsDS.HolonEnergyCount(src.Pokemon) > 0 && src.Engine.IsType(mon, EnergyType.Water);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Search.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Pollen (Poké-Power): once during your turn, flip a coin; heads, the Defending Pokémon is Poisoned. Poltergeist: 10 more per Trainer card in the opponent's hand.</summary>
        private sealed class VileplumePoisonPollen : Mon
        {
            private const string Power = "Poison Pollen";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Defending != null; }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Defending;
                if (ctx.Flip() && defending != null) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poltergeist") return false;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Poltergeist: your opponent's hand", ctx.Source);
                ctx.DamageDefending(atk.Damage + 10 * hand.Count(c => c.Def.IsTrainer));
                return true;
            }
        }

        // ================================================================== Pokémon-ex

        /// <summary>Splash Back (Poké-Power): once during your turn, if the opponent has 4 or more Benched Pokémon, return 1 of them and its cards to their hand. Power Blow.</summary>
        private sealed class CrawdauntExSplashBack : Mon
        {
            private const string Power = "Splash Back";
            private static readonly Mon Blow = new CardsASR1.TimesMon("Power Blow", c => c.EnergyCount(c.Pokemon));

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Bench.Count >= 4; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), Power + ": return which of your opponent's Benched Pokémon to their hand?");
                if (target != null) ctx.Engine.ReturnPokemonToHand(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blow.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Psychic Vision (Poké-Power): once during your turn, if Mew ex is on your Bench, look at the opponent's hand. Devo Crush: you may
        /// discard 2 Energy from it; if you do, you may shuffle the Defending Pokémon's highest Stage Evolution card into the opponent's deck.
        /// </summary>
        private sealed class MewExPsychicVision : Mon
        {
            private const string Power = "Psychic Vision";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return !ctx.PokemonIsActive && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", Power + ": your opponent's hand", ctx.Source);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devo Crush") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (CardsBS1.CopyingWithMetronome || !ctx.InPlay(me) || me.Energy.Count < 2) return true;
                if (!ctx.AskYesNo("Devo Crush: discard 2 Energy from " + me.Name + " to devolve the Defending Pokémon?")) return true;
                ctx.DiscardEnergyFrom(me, 2, "Devo Crush: discard 2 Energy");
                if (TefPokemon.StillDefending(ctx, defending) && CardsE2.IsEvolved(defending) && ctx.CanAffect(defending)) ctx.Engine.Devolve(defending, 1, true);
                return true;
            }
        }

        /// <summary>Driving Howl (Poké-Power): once during your turn, switch the Defending Pokémon with 1 of the opponent's Benched Pokémon (they choose). Hyper Claws.</summary>
        private sealed class MightyenaExDrivingHowl : Mon
        {
            private const string Power = "Driving Howl";
            private static readonly Mon Claws = CardsSS.ConditionBonus("Hyper Claws", 40, d => d.Def.Stage == Stage.Stage2 && CardsE2.IsEvolved(d));

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx) { CardsDX.OpponentChoosesSwitch(ctx, Power); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        // ================================================================== Pokémon ★

        /// <summary>Spiral Growth: flip until tails; a basic Energy card from your discard pile onto this Pokémon for each heads.</summary>
        private sealed class SpiralGrowthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Growth") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < heads && ctx.InPlay(me); i++)
                {
                    var cards = ctx.BasicEnergyInDiscard();
                    if (cards.Count == 0) break;
                    var card = ctx.ChooseCards(cards, 1, 1, "Spiral Growth: attach which basic Energy card (" + (i + 1) + " of " + heads + ")?")[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                }
                return true;
            }
        }

        /// <summary>All-out Blast: discard cards from the top of your deck until 1 is left; 20 more damage per Energy card discarded.</summary>
        private sealed class AllOutBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "All-out Blast") return false;
                int energy = 0;
                var deck = ctx.Me.Deck;
                while (deck.Count > 1)
                {
                    var card = deck[0];
                    deck.RemoveAt(0);
                    ctx.Me.Discard.Add(card);
                    if (card.Def.IsEnergy) energy++;
                }
                ctx.Log("All-out Blast: " + energy + (energy == 1 ? " Energy card was" : " Energy cards were") + " discarded from the top of the deck.");
                ctx.DamageDefending(atk.Damage + 20 * energy);
                return true;
            }
        }

        /// <summary>Energy Absorption: an Energy card from your discard pile onto Mewtwo ★. Psychic Star: against an Evolved Pokémon, discard all its Energy for 50 more.</summary>
        private sealed class MewtwoStar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Energy Absorption")
                {
                    var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                    if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                    var card = ctx.ChooseCards(cards, 1, 1, "Energy Absorption: attach which Energy card?")[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                    return true;
                }
                if (atk.Name != "Psychic Star") return false;
                bool evolved = ctx.Defending != null && CardsE2.IsEvolved(ctx.Defending);
                if (evolved && !CardsBS1.CopyingWithMetronome)
                    foreach (var card in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage + (evolved ? 50 : 0));
                return true;
            }
        }
    }
}
