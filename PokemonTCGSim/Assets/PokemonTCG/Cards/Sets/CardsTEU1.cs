using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Up (TEU, v80), part 1: Grass, Fire and Water (TEU 1-32), plus helpers the other parts share. No regulation mark:
    /// Expanded. Rulings used: a TAG TEAM's "extra Energy" is counted before the attack attaches anything (your ruling, v68, as
    /// for Crimson Flame Pillar-GX); "a [T] Energy card" outside play is basic Energy only (your ruling, v65).
    /// </summary>
    public static class CardsTEU1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("TEU-1", new CelebiVenusaurGX());
            r.RegisterByKey("TEU-2", new CardsDAA1.GustAttackMon("Tangle Drag", false));
            r.RegisterByKey("TEU-3", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("TEU-4", new KakunaTEU());
            r.RegisterByKey("TEU-5", new BeedrillTEU());
            r.RegisterByKey("TEU-7", new ParasectTEU());
            r.RegisterByKey("TEU-8", new CoinsMon("Bullet Seed", 4));
            r.RegisterByKey("TEU-9", new ComboMon(new NoRetreatMon("Grip and Squeeze"), new GuillotineHugMon()));
            r.RegisterByKey("TEU-10", new CardsASR1.TimesMon("Flower Storm", BasicEnergyOnMyPokemon));
            // Fire
            r.RegisterByKey("TEU-11", new CardsASR1.TimesMon("Reprisal", SelfCounters));
            r.RegisterByKey("TEU-12", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("TEU-13", new ConditionAttackMon("Fire Fang", SpecialCondition.Burned));
            r.RegisterByKey("TEU-14", new CharizardTEU());
            r.RegisterByKey("TEU-15", new CardsDAA1.FlipCantAttackMon("Tail Whip"));
            r.RegisterByKey("TEU-16", new NinetalesTEU());
            r.RegisterByKey("TEU-17", new FlipBonusMon("Stomp", 30));
            r.RegisterByKey("TEU-18", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new DodgeMon("Agility", false)));
            r.RegisterByKey("TEU-19", new MoltresTEU());
            r.RegisterByKey("TEU-20", new FlipBonusMon("Fasten Claws", 10));
            r.RegisterByKey("TEU-21", new CardsDRI1.WhirlwindMon("Roar"));
            // Water
            r.RegisterByKey("TEU-22", new SquirtleTEU());
            r.RegisterByKey("TEU-25", new BlastoiseTEU());
            r.RegisterByKey("TEU-26", new HeadacheMon("Headache"));
            r.RegisterByKey("TEU-27", new GolduckTEU());
            r.RegisterByKey("TEU-29", new MagikarpTEU());
            r.RegisterByKey("TEU-30", new GyaradosTEU());
            r.RegisterByKey("TEU-31", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 30)));
            r.RegisterByKey("TEU-32", new ArticunoTEU());
        }

        // ================================================================== Shared helpers

        /// <summary>Basic Energy cards attached to all of the player's Pokémon (Shaymin ◇'s Flower Storm).</summary>
        internal static int BasicEnergyOnMyPokemon(EffectContext ctx)
        {
            int n = 0;
            foreach (var mon in ctx.MyPokemon()) n += mon.Energy.Count(c => c.Def.IsBasicEnergy);
            return n;
        }

        private static int SelfCounters(EffectContext ctx) { return ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters; }

        /// <summary>A basic Energy card of this type ("a [T] Energy card" outside play: your ruling, v65).</summary>
        internal static bool BasicOf(CardDef d, EnergyType type) { return CardsSSP1.IsBasicOf(d, type); }

        /// <summary>Heals all damage from a Pokémon ("Heal all damage from this Pokémon").</summary>
        internal static void HealAll(EffectContext ctx, PokemonInPlay mon)
        {
            if (mon != null && ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
        }

        /// <summary>
        /// "Choose 1 of your opponent's Active Pokémon's attacks. During your opponent's next turn, that Pokémon can't use that
        /// attack" after the damage (Amnesia, Psyjack).
        /// </summary>
        internal sealed class AmnesiaMon : Mon
        {
            private readonly string attack;

            public AmnesiaMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                int index = ChooseAttackOf(ctx, defending.Def, attack + ": choose 1 of " + defending.Name + "'s attacks it can't use during its next turn");
                if (index < 0) return true;
                string name = defending.Def.Attacks[index].Name;
                ctx.AddEffect(new CantUseAttackEffect(defending.Name + " can't use " + name + " during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1, name));
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, your opponent can't play any Trainer cards from their hand during their next turn."</summary>
        internal sealed class HeadacheMon : Mon
        {
            private readonly string attack;

            public HeadacheMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsCEC1.TrainerLockCECEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards from their hand during their next turn (" + attack + ")",
                        ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>
        /// Pollen Hazard: Burned, Confused and Poisoned. Evergreen-GX: heal all damage from this Pokémon; with 1 extra Grass Energy,
        /// shuffle all cards from your discard pile into your deck.
        /// </summary>
        private sealed class CelebiVenusaurGX : Mon
        {
            private static readonly Mon Pollen = new CardsCES1.ConditionsMon("Pollen Hazard", SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Pollen.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Evergreen-GX") return false;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Grass);
                ctx.DamageDefending(atk.Damage);
                HealAll(ctx, ctx.Pokemon);
                if (extra && ctx.Me.Discard.Count > 0)
                {
                    var moved = ctx.DiscardToDeck(ctx.Me.Discard.ToList());
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + moved.Count + " cards from their discard pile into their deck (Evergreen-GX).");
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Grass Cushion: if this Pokémon has any Grass Energy attached, it takes 30 less damage from attacks (after Weakness and Resistance).</summary>
        private sealed class KakunaTEU : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null) return damage;
                if (!src.Engine.HasEnergyOfType(me, EnergyType.Grass)) return damage;
                return damage - 30;
            }
        }

        /// <summary>
        /// Destiny Stinger: only if this Pokémon has any damage counters on it; both Active Pokémon are Knocked Out. Reckless Charge:
        /// 10 damage to itself.
        /// </summary>
        private sealed class BeedrillTEU : Mon
        {
            private static readonly Mon Charge = new RecoilMon("Reckless Charge", 10);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Destiny Stinger") return true;
                return ctx.Pokemon != null && ctx.Pokemon.DamageCounters > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Charge.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Destiny Stinger") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.Engine.KnockOut(defending, atk.Name);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.KnockOut(ctx.Pokemon, atk.Name);
                return true;
            }
        }

        /// <summary>Panic Spores: 2 damage counters on the opponent's Confused Pokémon between turns. Mysterious Powder: flip, Confused.</summary>
        private sealed class ParasectTEU : Mon
        {
            private static readonly Mon Powder = new CardsPAR2.FlipConditionMon("Mysterious Powder", SpecialCondition.Confused);

            public override int CheckupAffects(EffectContext src) { return 1 - src.Player; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null || !src.InPlay(src.Pokemon)) return;
                var mon = src.Opp.Active;
                if (mon == null || !mon.Has(SpecialCondition.Confused)) return;
                src.Engine.PlaceDamageCounters(mon, 2, "Panic Spores");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Powder.HandleAttack(ctx, atk); }
        }

        /// <summary>Guillotine Hug: flip 2 coins; if both are heads, the opponent's Active Pokémon is Knocked Out.</summary>
        private sealed class GuillotineHugMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Guillotine Hug") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending)) ctx.Engine.KnockOut(defending, atk.Name);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Roaring Resolve: once during your turn, put 2 damage counters on this Pokémon; then search your deck for up to 2 Fire
        /// Energy cards and attach them to it. Continuous Blaze Ball: discard all Fire Energy from it, 50 more for each card.
        /// </summary>
        private sealed class CharizardTEU : Mon
        {
            protected override string Activated { get { return "Roaring Resolve"; } }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                ctx.Engine.PlaceDamageCounters(me, 2, "Roaring Resolve");
                CardsMEG.ChargeSelfMon.ChargeSelf(ctx, EnergyType.Fire, 2, "Roaring Resolve");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Continuous Blaze Ball") return false;
                var me = ctx.Pokemon;
                int discarded = 0;
                foreach (var card in me.Energy.Where(c => ProvidesType(ctx, me, c, EnergyType.Fire)).ToList())
                    if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                ctx.DamageDefending(atk.Damage + 50 * discarded);
                return true;
            }
        }

        /// <summary>
        /// True when this Energy attached to the Pokémon provides the type (Fire Energy, or a Special Energy giving Fire). Effects that
        /// change Energy types count too (Typhlosion's Blazing Energy, v81).
        /// </summary>
        internal static bool ProvidesType(EffectContext ctx, PokemonInPlay holder, CardInstance energy, EnergyType type)
        {
            if (holder == null) return energy.Def.IsBasicEnergy && energy.Def.EnergyProvides == type;
            foreach (int unit in ctx.Engine.EnergyUnitsOf(holder, energy))
                if (EnergyMask.Provides(unit, type)) return true;
            return false;
        }

        /// <summary>
        /// Nine Temptations: once during your turn, you may discard 2 Fire Energy cards from your hand; if you do, switch 1 of your
        /// opponent's Benched Pokémon with their Active Pokémon.
        /// </summary>
        private sealed class NinetalesTEU : Mon
        {
            protected override string Activated { get { return "Nine Temptations"; } }

            private static List<CardInstance> FireInHand(EffectContext ctx) { return ctx.Me.Hand.Where(c => BasicOf(c.Def, EnergyType.Fire)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return FireInHand(ctx).Count >= 2 && ctx.GustTargets().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(FireInHand(ctx), 2, 2, "Nine Temptations: discard 2 Fire Energy cards from your hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.GustOpponent("Nine Temptations: choose 1 of your opponent's Benched Pokémon to switch with their Active Pokémon");
            }
        }

        /// <summary>
        /// Top Burner: discard all Fire Energy from this Pokémon, then discard the top card of your opponent's deck for each Energy
        /// discarded. Fire Spin: discard 3 Energy from this Pokémon.
        /// </summary>
        private sealed class MoltresTEU : Mon
        {
            private static readonly Mon Spin = new CardsTWM1.SelfDiscardMon("Fire Spin", 3);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Spin.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Top Burner") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int discarded = 0;
                foreach (var card in me.Energy.Where(c => ProvidesType(ctx, me, c, EnergyType.Fire)).ToList())
                    if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                if (discarded > 0 && ctx.Opp.Deck.Count > 0)
                {
                    if (ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Log("The top cards of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck can't be discarded right now.");
                    else ctx.Engine.DiscardTopOfDeck(ctx.Opponent, discarded);
                }
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Floating Shell: if you have a Stadium card in play, this Pokémon has no Retreat Cost.</summary>
        private sealed class SquirtleTEU : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return src.State.Stadium != null && src.State.StadiumOwner == src.Player ? 0 : cost;
            }
        }

        /// <summary>
        /// Powerful Squall: once during your turn, look at the top 6 cards of your deck and attach any number of Water Energy cards
        /// you find there to your Pokémon in any way you like; shuffle the other cards back. Hydro Tackle: 30 damage to itself.
        /// </summary>
        private sealed class BlastoiseTEU : Mon
        {
            private static readonly Mon Tackle = new RecoilMon("Hydro Tackle", 30);

            protected override string Activated { get { return "Powerful Squall"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.Me.Deck.Take(6).ToList();
                var water = top.Where(c => BasicOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) ctx.Engine.ShowCards(ctx.Player, top, "Powerful Squall", "The top " + top.Count + " cards of your deck (no Water Energy)", ctx.Source);
                else ctx.AttachCardsAnyWay(water, water.Count, (c, m) => true, "Powerful Squall: attach Water Energy to your Pokémon");
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Amnesia: the Defending Pokémon can't use the attack you choose during the opponent's next turn. Swim: if any of the
        /// opponent's Pokémon have Water Energy attached, you may do the damage to 1 of their Benched Pokémon instead.
        /// </summary>
        private sealed class GolduckTEU : Mon
        {
            private static readonly Mon Amnesia = new AmnesiaMon("Amnesia");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Amnesia.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Swim") return false;
                bool water = ctx.OpponentPokemon().Any(m => ctx.Engine.HasEnergyOfType(m, EnergyType.Water));
                if (water && ctx.Opp.Bench.Count > 0 && ctx.AskYesNo("Swim: do " + atk.Damage + " damage to 1 of your opponent's Benched Pokémon instead of their Active Pokémon?"))
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Swim: choose 1 of your opponent's Benched Pokémon");
                    ctx.DamageTarget(target, atk.Damage);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Enter the Dragon: flip a coin; on heads, put a card that evolves from this Pokémon from your discard pile onto it to evolve it.</summary>
        private sealed class MagikarpTEU : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enter the Dragon") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !ctx.InPlay(me)) return true;
                var cards = ctx.Me.Discard.Where(c => CardsEVS1.EvolvesOnto(ctx, me, c.Def)).ToList();
                if (cards.Count == 0)
                {
                    ctx.Log("There's no card in the discard pile that evolves from " + me.Name + ".");
                    return true;
                }
                if (CardsEVS1.EvolveBlocked(ctx, me))
                {
                    ctx.Log(me.Name + " can't evolve right now (Enter the Dragon).");
                    return true;
                }
                var pick = ctx.ChooseCards(cards, 1, 1, "Enter the Dragon: choose a card that evolves from " + me.Name)[0];
                ctx.Me.Discard.Remove(pick);
                ctx.Engine.EvolvePokemon(me, pick);
                return true;
            }
        }

        /// <summary>
        /// Distilled Blast: reveal the top 7 cards of your deck; 30 more damage for each Water Energy card among them. Shuffle those
        /// Energy cards back into your deck and discard the other cards. Hyper Beam: discard an Energy from the opponent's Active Pokémon.
        /// </summary>
        private sealed class GyaradosTEU : Mon
        {
            private static readonly Mon Beam = new CardsDRI2.CrunchMon("Hyper Beam");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Beam.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Distilled Blast") return false;
                var me = ctx.Me;
                var top = me.Deck.Take(7).ToList();
                int water = top.Count(c => BasicOf(c.Def, EnergyType.Water));
                ctx.Engine.Reveal(ctx.Player, top, "revealed the top " + top.Count + " cards of their deck (Distilled Blast)");
                ctx.DamageDefending(atk.Damage + 30 * water);
                // The other cards are discarded; the Water Energy cards are shuffled back.
                var others = top.Where(c => !BasicOf(c.Def, EnergyType.Water) && me.Deck.Contains(c)).ToList();
                foreach (var card in others)
                {
                    me.Deck.Remove(card);
                    me.Discard.Add(card);
                }
                if (others.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + others.Count + (others.Count == 1 ? " card" : " cards") + " (Distilled Blast).");
                if (top.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Blizzard Veil: while this Pokémon is your Active Pokémon, the effects of the opponent's Supporter cards don't affect your
        /// Benched Water Pokémon. Cold Cyclone: move 2 Water Energy from this Pokémon to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class ArticunoTEU : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || trainer == null || !trainer.IsSupporter) return false;
                // The engine asks only about the other player's Trainer cards.
                if (target == null || target.Owner != src.Player || target == me || src.IsActive(target)) return false;
                return src.Engine.IsType(target, EnergyType.Water);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cold Cyclone") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var water = me.Energy.Where(c => ProvidesType(ctx, me, c, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Cold Cyclone: move 2 Water Energy to which Benched Pokémon?");
                var moving = water.Count <= 2 ? water : ctx.ChooseCards(water, 2, 2, "Cold Cyclone: choose 2 Water Energy to move");
                foreach (var card in moving) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }
    }
}
