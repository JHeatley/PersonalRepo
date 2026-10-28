using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Hidden Fates (HIF) part 2: the Shiny Vault's Pokémon SV1-SV49 (Sun &amp; Moon era cards). Rulings used (Compendium): Wimp Out
    /// works only while Wimpod is in play (2017-09-14); Sparkling Wish may be used with nothing to evolve and can evolve a Pokémon
    /// put into play this turn (2017-08); First Impression doesn't count a Pokémon that evolved after moving to the Active Spot
    /// (2017-08-03); More Poison adds 1 to any other Poison count (2017-08-03); Fight Song stacks (2012-08-23); Energy Evolution
    /// isn't offered for a type with no Eevee evolution (no Fighting, Metal or Dragon one); like Solar Evolution, it ignores the
    /// usual timing (your ruling, v58).
    /// </summary>
    public static class CardsHIF2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HIF-SV1", new ComboMon(new TwinPlayMon(), new DodgeMon("Agility", false)));
            r.RegisterByKey("HIF-SV3", new ComboMon(new CardsMEG.SnipeMon("Sharp Blade Quill", 20), new FlipBonusMon("Leaf Blade", 20)));
            r.RegisterByKey("HIF-SV4", new WimpodHIF());
            r.RegisterByKey("HIF-SV5", new CardsFST2.ConditionBonusMon("White Ray", 90, OnePrizeLeft));
            r.RegisterByKey("HIF-SV6", new ConditionAttackMon("Fire Fang", SpecialCondition.Burned));
            r.RegisterByKey("HIF-SV7", new CharmeleonHIF());
            r.RegisterByKey("HIF-SV8", new SearchAttackMon("Beacon", CardsHIF1.IsPokemonCard, 2));
            r.RegisterByKey("HIF-SV10", new QuagsireHIF());
            r.RegisterByKey("HIF-SV11", new CardsSVP.FloatMon(EnergyType.Water));
            r.RegisterByKey("HIF-SV12", new FrogadierHIF());
            r.RegisterByKey("HIF-SV13", new VoltorbHIF());
            r.RegisterByKey("HIF-SV14", new ComboMon(new ConditionAttackMon("Dazzle Blast", SpecialCondition.Confused), new CablegramMon()));
            r.RegisterByKey("HIF-SV15", new SeviperHIF());
            r.RegisterByKey("HIF-SV17", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("HIF-SV18", new MalamarHIF());
            r.RegisterByKey("HIF-SV19", new ComboMon(new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned), new KnockoutReviverMon()));
            r.RegisterByKey("HIF-SV20", new SudowoodoHIF());
            r.RegisterByKey("HIF-SV21", new DodgeMon("Detect", false));
            r.RegisterByKey("HIF-SV22", new LucarioHIF());
            r.RegisterByKey("HIF-SV23", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("HIF-SV24", new ComboMon(new CardsFST2.ConditionBonusMon("Sledgehammer", 90, OpponentFourLeft), new CardsWHT.FlipMoreMon("Swing Around", 2, 20)));
            r.RegisterByKey("HIF-SV26", new LordsValleyMon());
            r.RegisterByKey("HIF-SV27", new SearchAttackMon("Searching Magnet", IsMetalEnergyCard, 3));
            r.RegisterByKey("HIF-SV28", new LockAfterAttackMon("Zap Cannon", true));
            r.RegisterByKey("HIF-SV29", new MagnezoneHIF());
            r.RegisterByKey("HIF-SV30", new CardsASC.SelfDiscardTypeMon("Core Beam", EnergyType.Metal, 1));
            r.RegisterByKey("HIF-SV31", new CardsASC.SelfDiscardTypeMon("Core Beam", EnergyType.Metal, 1));
            r.RegisterByKey("HIF-SV32", new CelesteelaHIF());
            r.RegisterByKey("HIF-SV33", new CardsFST2.ConditionBonusMon("Divine Paper", 90, OpponentSixLeft));
            r.RegisterByKey("HIF-SV34", new CardsTWM1.HealSelfMon("Draining Kiss", 10));
            r.RegisterByKey("HIF-SV36", new DiancieHIF());
            r.RegisterByKey("HIF-SV37", new AltariaHIF());
            r.RegisterByKey("HIF-SV38", new CardsSIT1.AscensionMon());
            r.RegisterByKey("HIF-SV39", new CardsSIT1.AscensionMon());
            r.RegisterByKey("HIF-SV40", new ComboMon(new CardsMEG.SnipeMon("Quick Dive", 50), new CardsBRS1.SupporterBonusMon("Royal Blades", "Cynthia", 100)));
            r.RegisterByKey("HIF-SV41", new EeveeHIF());
            r.RegisterByKey("HIF-SV42", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("HIF-SV43", new DodgeMon("Agility", false));
            r.RegisterByKey("HIF-SV44", new OranguruHIF());
            r.RegisterByKey("HIF-SV45", new CardsASC.SelfGuardMon("Armor Press", 30));
            r.RegisterByKey("HIF-SV46", new LeafeonGX());
            r.RegisterByKey("HIF-SV47", new DecidueyeGX());
            r.RegisterByKey("HIF-SV48", new GolisopodGX());
            r.RegisterByKey("HIF-SV49", new ComboMon(new CardsASC.SelfDiscardTypeMon("Crimson Storm", EnergyType.Fire, 3), new CardsSSP2.StompOffMon("Raging Out-GX", 10)));
        }

        // ================================================================== Shared helpers

        internal static bool IsMetalEnergyCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Metal); }
        private static bool OnePrizeLeft(EffectContext ctx) { return ctx.Me.Prizes.Count == 1; }
        private static bool OpponentFourLeft(EffectContext ctx) { return ctx.Opp.Prizes.Count == 4; }
        private static bool OpponentSixLeft(EffectContext ctx) { return ctx.Opp.Prizes.Count == 6; }

        /// <summary>
        /// "Search your deck for a card that evolves from 1 of your Pokémon and put it onto that Pokémon to evolve it": the
        /// Pokémon chosen must be able to evolve now unless timing is ignored (Sparkling Wish ignores it, Compendium 2017-08).
        /// Returns the evolved Pokémon, or null.
        /// </summary>
        internal static PokemonInPlay EvolveOneFromDeck(EffectContext ctx, List<PokemonInPlay> candidates, string what)
        {
            if (ctx.Me.Deck.Count == 0 || candidates.Count == 0) return null;
            var found = ctx.SearchMyDeck(d => candidates.Any(t => CardsEVS1.EvolvesOnto(ctx, t, d)), 1, what + ": choose a card that evolves from 1 of your Pokémon", false);
            if (found.Count == 0) return null;
            var card = found[0];
            var onto = candidates.Where(t => CardsEVS1.EvolvesOnto(ctx, t, card.Def)).ToList();
            var target = ctx.ChoosePokemon(onto, what + ": evolve which Pokémon into " + card.Def.Name + "?");
            if (target == null)
            {
                ctx.Me.Deck.Add(card);
                return null;
            }
            ctx.Engine.EvolvePokemon(target, card);
            return target;
        }

        // ================================================================== Attacks

        /// <summary>Twin Play: put up to 2 Scyther from your deck onto your Bench.</summary>
        private sealed class TwinPlayMon : Mon
        {
            private static bool IsScyther(CardDef d) { return d.Name == "Scyther"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twin Play") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(IsScyther, 2, "Twin Play: put up to 2 Scyther onto your Bench");
                return true;
            }
        }

        /// <summary>Cablegram: if you have exactly 3 Prize cards left, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class CablegramMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cablegram") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Prizes.Count == 3 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>No Prize cards for the Pokémon if it's Knocked Out during the opponent's next turn (Knockout Reviver).</summary>
        internal sealed class NoPrizeEffect : ContinuousEffect
        {
            public NoPrizeEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return knockedOut.Uid == TargetUid && taker != Owner ? 0 : prizes;
            }
        }

        /// <summary>Knockout Reviver: during your opponent's next turn, if this Pokémon is Knocked Out, they take no Prize cards for it.</summary>
        private sealed class KnockoutReviverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knockout Reviver") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                var fx = new NoPrizeEffect("No Prize cards for " + ctx.Pokemon.Name + " if it's Knocked Out during the opponent's next turn (Knockout Reviver)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1);
                fx.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>Lord's Valley: if you have exactly 2, 4 or 6 Prize cards left, discard the top 10 cards of your deck.</summary>
        private sealed class LordsValleyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lord's Valley") return false;
                ctx.DamageDefending(atk.Damage);
                int left = ctx.Me.Prizes.Count;
                if (left == 2 || left == 4 || left == 6) ctx.Engine.DiscardTopOfDeck(ctx.Player, 10);
                return true;
            }
        }

        // ================================================================== Pokémon with Abilities

        /// <summary>Wimp Out: during your first turn, this Pokémon has no Retreat Cost.</summary>
        private sealed class WimpodHIF : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.IsMyFirstTurn ? 0 : cost;
            }
        }

        /// <summary>
        /// Burning Fighter: when you play this Pokémon from your hand to evolve, you may discard the top 5 cards of your deck and
        /// attach the basic Fire Energy cards among them to it. Flamethrower: discard an Energy from this Pokémon.
        /// </summary>
        private sealed class CharmeleonHIF : Mon
        {
            private static readonly Mon Flamethrower = new CardsTWM1.SelfDiscardMon("Flamethrower", 1);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Burning Fighter", ctx.Me.Deck.Count > 0, "Use Burning Fighter? Discard the top 5 cards of your deck and attach the Fire Energy cards among them to " + ctx.Pokemon.Name + ".")) return;
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                foreach (var card in top)
                    if (CardsEVS1.IsEnergyOf(card.Def, EnergyType.Fire) && ctx.InPlay(ctx.Pokemon) && ctx.Me.Discard.Remove(card))
                        ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flamethrower.HandleAttack(ctx, atk); }
        }

        /// <summary>Wash Out: as often as you like, move a Water Energy from 1 of your Benched Pokémon to your Active Pokémon. Hydro Pump: 20 more for each Water Energy.</summary>
        private sealed class QuagsireHIF : Mon
        {
            private static readonly Mon HydroPump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20);

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => m.Energy.Any(x => CardsHIF1.Provides(ctx, m, x, EnergyType.Water))).ToList();
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Wash Out" && ctx.IsMyTurn && ctx.Me.Active != null && Givers(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => ctx.Me.Active != null && Givers(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), "Wash Out: move a Water Energy from which Benched Pokémon to "
                        + ctx.Me.Active.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null || ctx.Me.Active == null) return false;
                    var water = from.Energy.Where(x => CardsHIF1.Provides(ctx, from, x, EnergyType.Water)).ToList();
                    var energy = water.Select(x => x.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Wash Out: move which Energy?")[0];
                    return ctx.Engine.MoveEnergy(from, ctx.Me.Active, energy);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HydroPump.HandleAttack(ctx, atk); }
        }

        /// <summary>Gale Shuriken: when you play this Pokémon from your hand to evolve, you may put 2 damage counters on 1 of your opponent's Pokémon.</summary>
        private sealed class FrogadierHIF : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Gale Shuriken", ctx.Opp.HasPokemonInPlay, "Use Gale Shuriken? Put 2 damage counters on 1 of your opponent's Pokémon.")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Gale Shuriken: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }
        }

        /// <summary>Floating Electrons: with any Energy attached, this Pokémon has no Retreat Cost. Thunder Shock: flip, Paralyzed on heads.</summary>
        private sealed class VoltorbHIF : Mon
        {
            private static readonly Mon ThunderShock = new ParalyzeFlipMon("Thunder Shock");

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ThunderShock.HandleAttack(ctx, atk); }
        }

        /// <summary>More Poison: 1 more damage counter on your opponent's Poisoned Pokémon between turns (each Seviper adds 1). Venomous Fang: Poisoned.</summary>
        private sealed class SeviperHIF : Mon
        {
            private static readonly Mon VenomousFang = new ConditionAttackMon("Venomous Fang", SpecialCondition.Poisoned);

            public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player ? counters + 1 : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return VenomousFang.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Recharge: once during your turn, attach a basic Psychic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class MalamarHIF : Mon
        {
            protected override string Activated { get { return "Psychic Recharge"; } }

            private static List<CardInstance> Psychic(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Psychic); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && Psychic(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pile = Psychic(ctx);
                if (pile.Count == 0 || ctx.Me.Bench.Count == 0) return;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Psychic Recharge: attach a Psychic Energy card to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(pile[0])) ctx.Engine.AttachEnergyCard(pile[0], target);
            }
        }

        /// <summary>Roadblock: your opponent can't have more than 4 Benched Pokémon (the smaller limit wins).</summary>
        private sealed class SudowoodoHIF : Mon
        {
            public override int CapMaxBench(EffectContext src, int player)
            {
                return src.Pokemon != null && player != src.Player ? 4 : int.MaxValue;
            }
        }

        /// <summary>Precognitive Aura: once during your turn, if you have Garchomp in play, search your deck for a card and put it into your hand. Missile Jab: not affected by Resistance.</summary>
        private sealed class LucarioHIF : Mon
        {
            private static readonly Mon MissileJab = new CardsFLF1.NoResistanceMon("Missile Jab");

            protected override string Activated { get { return "Precognitive Aura"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.MyPokemon().Any(m => m.Name == "Garchomp");
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(CardsHIF1.AnyCard, 1, "Precognitive Aura: search your deck for a card", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MissileJab.HandleAttack(ctx, atk); }
        }

        /// <summary>Magnetic Circuit: as often as you like during your turn, attach a basic Metal Energy card from your hand to 1 of your Pokémon. Zap Cannon: can't be used next turn.</summary>
        private sealed class MagnezoneHIF : Mon
        {
            private const string Circuit = "Magnetic Circuit";
            private static readonly Mon ZapCannon = new LockAfterAttackMon("Zap Cannon", true);

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Circuit || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(x => IsMetalEnergyCard(x.Def)) && ctx.MyPokemon().Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.AttachFromHand(ctx, IsMetalEnergyCard, () => ctx.MyPokemon(), Circuit, "a Metal Energy card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ZapCannon.HandleAttack(ctx, atk); }
        }

        /// <summary>Moon Raker: if both players' remaining Prize cards add up to exactly 6, it costs only [M].</summary>
        private sealed class CelesteelaHIF : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack == null || attack.Name != "Moon Raker") return;
                if (ctx.Me.Prizes.Count + ctx.Opp.Prizes.Count != 6) return;
                cost.Clear();
                cost.Add(EnergyType.Metal);
            }
        }

        /// <summary>Sparkling Wish: search your deck for a card that evolves from 1 of your Pokémon and evolve it (any timing). Diamond Storm: heal 30 from each of your Fairy Pokémon.</summary>
        private sealed class DiancieHIF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Sparkling Wish")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var mine = ctx.MyPokemon(m => !CardsEVS1.EvolveBlocked(ctx, m));
                    if (ctx.Me.Deck.Count > 0) EvolveOneFromDeck(ctx, mine, "Sparkling Wish");
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name != "Diamond Storm") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fairy)))
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 30);
                return true;
            }
        }

        /// <summary>Fight Song: your Dragon Pokémon's attacks do 20 more damage to the opponent's Active Pokémon (each Altaria adds 20).</summary>
        internal sealed class AltariaHIF : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(attacker, EnergyType.Dragon) ? damage + 20 : damage;
            }
        }

        /// <summary>
        /// Energy Evolution: when you attach a basic Energy card from your hand to this Pokémon during your turn, you may evolve it
        /// from your deck into a card of that Energy's type (any timing). Quick Draw: flip; draw a card on heads.
        /// </summary>
        private sealed class EeveeHIF : Mon
        {
            private static readonly Mon QuickDraw = new CardsG2.GraspingVineMon("Quick Draw", 1);
            private static readonly EnergyType[] NoEeveelution = { EnergyType.Fighting, EnergyType.Metal, EnergyType.Dragon, EnergyType.Colorless };

            public override bool EvolvesOnOwnerAttachFromHand { get { return true; } }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var mon = src.Pokemon;
                if (mon == null || target != mon || energy == null || !energy.Def.IsBasicEnergy || !src.IsMyTurn || !src.InPlay(mon)) return;
                var type = energy.Def.EnergyProvides;
                if (NoEeveelution.Contains(type) || src.Me.Deck.Count == 0 || CardsEVS1.EvolveBlocked(src, mon)) return;
                if (!src.AskYesNo("Energy Evolution: search your deck for a " + type + " card that evolves from " + mon.Name + " and evolve it?")) return;
                Announce(src, "Energy Evolution");
                var found = src.SearchMyDeck(d => d.HasType(type) && CardsEVS1.EvolvesOnto(src, mon, d), 1, "Energy Evolution: choose a " + type + " card that evolves from " + mon.Name, false);
                if (found.Count > 0) src.Engine.EvolvePokemon(mon, found[0]);
                src.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return QuickDraw.HandleAttack(ctx, atk); }
        }

        /// <summary>Instruct: once during your turn, draw cards until you have 3 in your hand. Psychic: 20 more for each Energy attached to the opponent's Active Pokémon.</summary>
        private sealed class OranguruHIF : Mon
        {
            private static readonly Mon Psychic = new CardsASR1.PlusMon("Psychic", 20, DefendingEnergy);

            private static int DefendingEnergy(EffectContext ctx) { return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending); }

            protected override string Activated { get { return "Instruct"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 3 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(3); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psychic.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Breath of the Leaves: once during your turn, if this Pokémon is Active, heal 50 damage from 1 of your Pokémon that has
        /// any Energy attached. Grand Bloom-GX: evolve each of your Benched Basic Pokémon from your deck.
        /// </summary>
        private sealed class LeafeonGX : Mon
        {
            protected override string Activated { get { return "Breath of the Leaves"; } }

            private static List<PokemonInPlay> Healable(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Count > 0 && m.DamageCounters > 0); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && Healable(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(Healable(ctx), "Breath of the Leaves: heal 50 damage from which Pokémon?");
                if (target != null) ctx.Heal(target, 50);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Bloom-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Where(m => m.Def.IsBasicPokemon && !CardsEVS1.EvolveBlocked(ctx, m)).ToList())
                {
                    if (ctx.Me.Deck.Count == 0) break;
                    var basic = mon;
                    var found = ctx.SearchMyDeck(d => CardsEVS1.EvolvesOnto(ctx, basic, d), 1, "Grand Bloom-GX: choose a card that evolves from " + basic.Name, false);
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(basic, found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Feather Arrow: once during your turn, put 2 damage counters on 1 of your opponent's Pokémon. Hollow Hunt-GX: put 3 cards from your discard pile into your hand.</summary>
        private sealed class DecidueyeGX : Mon
        {
            protected override string Activated { get { return "Feather Arrow"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Feather Arrow: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hollow Hunt-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(3, ctx.Me.Discard.Count);
                if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), n, n, "Hollow Hunt-GX: put 3 cards from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>First Impression: 90 more if this Pokémon moved from your Bench to the Active Spot this turn. Armor Press: 20 less damage next turn. Crossing Cut-GX: switch with a Benched Pokémon.</summary>
        private sealed class GolisopodGX : Mon
        {
            private static readonly Mon Others = new ComboMon(new CardsASC.SelfGuardMon("Armor Press", 20), new SwitchSelfMon("Crossing Cut-GX", false));

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "First Impression") return Others.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn ? 90 : 0));
                return true;
            }
        }
    }
}
