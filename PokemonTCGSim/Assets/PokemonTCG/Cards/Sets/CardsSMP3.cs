using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>SM Black Star Promos (SMP, v91), part 3: SM153-SM200 (the promos new to the pool).</summary>
    public static class CardsSMP3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SMP-153", new FlipBonusMon("Leaf Blade", 30));
            r.RegisterByKey("SMP-154", new ComboMon(new CardsPAR2.FlipConditionMon("Smog", SpecialCondition.Poisoned), new CardsTWM1.SelfDiscardMon("Ember", 1)));
            r.RegisterByKey("SMP-157", new CardsCPA.LimberMon());
            r.RegisterByKey("SMP-162", new ComboMon(new ParalyzeFlipMon("Nuzzle"), new RecoilMon("Thunder Jolt", 10)));
            r.RegisterByKey("SMP-163", new ComboMon(new CardsUNB3.MimicMon(), new FlipBonusMon("Play Rough", 20)));
            r.RegisterByKey("SMP-164", new DeoxysSMP());
            r.RegisterByKey("SMP-165", new ShiningBurstMon());
            r.RegisterByKey("SMP-171", new ComboMon(new HeatStageMon(), new CardsASC.SelfDiscardTypeMon("Bright Flame", EnergyType.Fire, 2),
                new CardsASR1.TimesMon("Power Burner-GX", c => c.Me.Discard.Count(x => CardsEVS1.IsEnergyOf(x.Def, EnergyType.Fire)))));
            r.RegisterByKey("SMP-172", new VaporeonGX());
            r.RegisterByKey("SMP-173", new ComboMon(new Cards30CC.BenchHitMon("Electrobullet", 30), new ProtectAfterDamageMon("Swift Run-GX")));
            r.RegisterByKey("SMP-174", new EeveeGX());
            r.RegisterByKey("SMP-177", new CardsUNM1.NamedBenchSearchMon("Multiply", "Meltan", 1));
            r.RegisterByKey("SMP-178", new CardsFST1.TakeLessMon(30, new ComboMon(new CardsMEG.EnergyBonusMon("Metal Blast", EnergyType.Metal, 20), new IronForceMon())));
            r.RegisterByKey("SMP-183", new RecoilMon("Thunder Jolt", 20));
            r.RegisterByKey("SMP-184", new CardsSMP1.FlipSearchMon("Find a Friend", d => d.IsPokemon, "a Pokémon"));
            r.RegisterByKey("SMP-185", new ComboMon(new ExploderMon(), new ConditionAttackMon("Bursting Inferno", SpecialCondition.Burned)));
            r.RegisterByKey("SMP-186", new CardsASC.SelfDiscardTypeMon("Fire Spin", EnergyType.Fire, 2));
            r.RegisterByKey("SMP-187", new AlolanMarowakGX());
            r.RegisterByKey("SMP-188", new ComboMon(new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused),
                new CardsFST2.ConditionBonusMon("Enraged Strike", 80, c => c.Defending != null && c.Defending.Has(SpecialCondition.Confused)), new DrawAttackMon("Familial Combo-GX", 5)));
            r.RegisterByKey("SMP-190", new CardsTWM1.HealSelfMon("Coffee Break", 30));
            r.RegisterByKey("SMP-194", new BrilliantDeductionMon("Brilliant Deduction", 4));
            r.RegisterByKey("SMP-195", new ComboMon(new RagingDestructionMon(), new DreadfulFlamesMon()));
            r.RegisterByKey("SMP-196", new ComboMon(new CardsEPO.TelekinesisMon(50), new ConditionAttackMon("Reigning Pulse", SpecialCondition.Confused), new NoDamageAfterAttackMon("Psychic Nova-GX")));
            r.RegisterByKey("SMP-197", new GreninjaGX());
            r.RegisterByKey("SMP-200", new ParalyzeFlipMon("Paralyzing Gaze"));
        }

        // ================================================================== Shared helpers

        /// <summary>Damage, then prevent all effects of attacks, damage included, done to this Pokémon during the opponent's next turn (Swift Run-GX).</summary>
        internal sealed class ProtectAfterDamageMon : Mon
        {
            private readonly string attack;

            public ProtectAfterDamageMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Damage, then prevent all damage done to this Pokémon by attacks during the opponent's next turn (Psychic Nova-GX).</summary>
        internal sealed class NoDamageAfterAttackMon : Mon
        {
            private readonly string attack;

            public NoDamageAfterAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) CardsSP1.FlipNoDamageMon.ProtectFromDamage(ctx, attack);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>
        /// Power Suction: once during your turn, move an Energy from 1 of your other Pokémon to 1 of your Deoxys. Psycho Boost: if
        /// this Pokémon used Psycho Boost during your last turn, its base damage is 50.
        /// </summary>
        private sealed class DeoxysSMP : Mon
        {
            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.Name != "Deoxys" && m.Energy.Count > 0); }

            protected override string Activated { get { return "Power Suction"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Donors(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var donors = Donors(ctx);
                if (donors.Count == 0) return;
                var donor = ctx.ChoosePokemon(donors, "Power Suction: move an Energy from which of your Pokémon?");
                if (donor == null) return;
                var energy = ctx.ChooseCards(donor.Energy.ToList(), 1, 1, "Power Suction: move which Energy?")[0];
                var deoxys = ctx.MyPokemon(m => m.Def.Name == "Deoxys");
                var dest = ctx.ChoosePokemon(deoxys, "Power Suction: move " + energy.Def.Name + " to which Deoxys?");
                if (dest != null) ctx.Engine.MoveEnergy(donor, dest, energy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psycho Boost") return false;
                var me = ctx.Pokemon;
                bool tired = me.LastAttackName == "Psycho Boost" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(tired ? 50 : atk.Damage);
                return true;
            }
        }

        /// <summary>Shining Burst: if both players have 6 or fewer Prize cards left in total, discard all Energy from this Pokémon and do 100 more damage.</summary>
        private sealed class ShiningBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shining Burst") return false;
                bool burst = ctx.Me.Prizes.Count + ctx.Opp.Prizes.Count <= 6;
                if (burst && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                ctx.DamageDefending(atk.Damage + (burst ? 100 : 0));
                return true;
            }
        }

        /// <summary>Heat Stage: you may attach up to 3 Fire Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class HeatStageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heat Stage") return false;
                ctx.DamageDefending(atk.Damage);
                var fire = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count > 0) ctx.AttachCardsAnyWay(fire, 3, (card, mon) => true, "Heat Stage: attach Fire Energy cards from your hand", true);
                return true;
            }
        }

        /// <summary>
        /// Hydrating Drops: once during your turn, heal 30 damage from your Active Water Pokémon. Hydro Pump: 30 more for each Water
        /// Energy on this Pokémon. Cure Shower-GX: heal all damage from all of your Water Pokémon.
        /// </summary>
        private sealed class VaporeonGX : Mon
        {
            private static readonly Mon Pump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 30);

            private static bool Healable(EffectContext ctx, PokemonInPlay mon)
            {
                return mon != null && mon.DamageCounters > 0 && ctx.Engine.IsType(mon, EnergyType.Water) && !ctx.Engine.HealingBlocked(mon);
            }

            protected override string Activated { get { return "Hydrating Drops"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Healable(ctx, ctx.Me.Active); }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (Healable(ctx, active)) ctx.Heal(active, 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cure Shower-GX") return Pump.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Water))) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Ascension DNA: once during your turn, if you have a Pokémon that evolves from Eevee in your hand, heal all damage from this
        /// Pokémon and put that card on it to evolve it; not during your first turn or the turn it was put into play. Joy Maker-GX:
        /// put 3 cards from your discard pile into your hand.
        /// </summary>
        private sealed class EeveeGX : Mon
        {
            private static List<CardInstance> Evolutions(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.EvolvesFrom == "Eevee").ToList(); }

            protected override string Activated { get { return "Ascension DNA"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return me != null && !ctx.IsMyFirstTurn && me.TurnEnteredPlay != ctx.Turn && Evolutions(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var options = Evolutions(ctx);
                if (me == null || options.Count == 0) return;
                var card = options.Select(c => c.Def.Name).Distinct().Count() == 1 ? options[0]
                    : ctx.ChooseCards(options, 1, 1, "Ascension DNA: evolve into which card?")[0];
                if (me.DamageCounters > 0 && !ctx.Engine.HealingBlocked(me)) ctx.Heal(me, me.DamageCounters * 10);
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.EvolvePokemon(me, card, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Joy Maker-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(3, ctx.Me.Discard.Count);
                if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), n, n, "Joy Maker-GX: put 3 cards from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>Iron Force-GX: attach any number of Metal Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class IronForceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Force-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal);
                if (pile.Count > 0 && ctx.InPlay(me)) ctx.AttachCardsAnyWay(pile, pile.Count, (card, mon) => mon == me, "Iron Force-GX: attach Metal Energy cards to " + me.Name, true);
                return true;
            }
        }

        /// <summary>Exploder: search your deck for up to 3 Fire Energy cards and attach them to your Pokémon in any way you like.</summary>
        private sealed class ExploderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Exploder") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Fire), 3, "Exploder: search your deck for up to 3 Fire Energy cards");
                CardsSUM1.AttachFoundAnyWay(ctx, found, "Exploder");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Cursed Body: if this Pokémon is your Active Pokémon and is damaged by an opponent's attack (even if it's Knocked Out), the
        /// Attacking Pokémon is now Confused. Fiery Bone: Burned. Lost Boomerang-GX: 50 damage to 2 of the opponent's Pokémon, not
        /// affected by Weakness or Resistance; a Pokémon Knocked Out by it goes to the Lost Zone with all its cards.
        /// </summary>
        private sealed class AlolanMarowakGX : Mon
        {
            private static readonly Mon Bone = new ConditionAttackMon("Fiery Bone", SpecialCondition.Burned);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || damage <= 0 || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, SpecialCondition.Confused)) ctx.Log("Cursed Body confused " + attacker.Name + ".");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Boomerang-GX") return Bone.HandleAttack(ctx, atk);
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var targets = all.Count <= 2 ? all : ctx.ChoosePokemonMany(all, 2, 2, "Lost Boomerang-GX: choose 2 of your opponent's Pokémon");
                foreach (var target in targets)
                {
                    if (!ctx.InPlay(target)) continue;
                    if (ctx.DamageWith(target, 50, false, false, false) > 0)
                        ctx.AddEffect(new LostIfKnockedOutEffect(target.Name + " goes to the Lost Zone if it's Knocked Out (Lost Boomerang-GX)", ctx.Player, target.Uid, ctx.Turn));
                }
                return true;
            }
        }

        /// <summary>The Pokémon and all cards attached go to the Lost Zone if it's Knocked Out while this lasts (this turn).</summary>
        internal sealed class LostIfKnockedOutEffect : ContinuousEffect
        {
            public LostIfKnockedOutEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int LostZoneOnKnockOut(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid ? 2 : 0; }
        }

        /// <summary>Brilliant Deduction: look at the top 4 cards of your deck, put 1 of them into your hand and shuffle the others back. Used for Brilliant Deduction, Radar Eyes (the attack name and the numbers are passed in).</summary>
        internal sealed class BrilliantDeductionMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BrilliantDeductionMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(amount);
                if (top.Count == 0) return true;
                ctx.DeckToHand(ctx.ChooseCards(top, 1, 1, attack + ": put 1 of these cards into your hand"));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Raging Destruction: discard the top 8 cards of your deck and attach any Fire Energy cards among them to this Pokémon.</summary>
        private sealed class RagingDestructionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Destruction") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 8);
                if (!ctx.InPlay(me)) return true;
                foreach (var card in milled.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList())
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Dreadful Flames-GX: discard an Energy from each of the opponent's Pokémon.</summary>
        private sealed class DreadfulFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dreadful Flames-GX") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Count > 0))
                {
                    if (!ctx.CanAffect(mon)) continue;
                    var en = ctx.ChooseCards(mon.Energy.ToList(), 1, 1, "Dreadful Flames-GX: discard which Energy from " + mon.Name + "?")[0];
                    ctx.Engine.DiscardEnergy(mon, en);
                }
                return true;
            }
        }

        /// <summary>
        /// Elusive Master: once during your turn, if this Pokémon is the last card in your hand, you may play it onto your Bench and
        /// draw 3 cards. Mist Slash: not affected by Weakness, Resistance or any effects on the Defending Pokémon. Dark Mist-GX: put 1
        /// of the opponent's Benched Pokémon and all cards attached to it into their hand.
        /// </summary>
        private sealed class GreninjaGX : Mon
        {
            private static readonly Mon Slash = new CardsCRZ1.PiercingMon("Mist Slash");

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Hand.Count == 1 && ctx.Me.Hand[0] == ctx.Card && ctx.BenchSpace > 0
                    && !ctx.Engine.PokemonPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!ctx.Me.Hand.Remove(ctx.Card)) return;
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, true);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Mist-GX") return Slash.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench().Where(m => ctx.CanAffect(m) && ctx.Engine.CanPutIntoHand(m)).ToList();
                if (bench.Count == 0) return true;
                var pick = ctx.ChoosePokemon(bench, "Dark Mist-GX: put which of your opponent's Benched Pokémon into their hand?");
                if (pick != null) ctx.Engine.ReturnPokemonToHand(pick);
                return true;
            }
        }
    }
}
