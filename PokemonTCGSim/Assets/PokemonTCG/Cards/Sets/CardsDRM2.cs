using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragon Majesty (DRM, v82), part 2: Fighting, Darkness, Dragon and Colorless (DRM 32-57), Ultra Necrozma-GX (DRM 78) and the
    /// Trainers. Altaria, Altaria-GX, the Collect Swablu and Blaine's Last Stand were already in the pool (Hidden Fates).
    /// "Put onto your Bench" from the deck or discard pile takes Evolution cards too (Compendium: Eternal Flame-GX, Spirit Burner).
    /// </summary>
    public static class CardsDRM2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DRM-32", new CardsBUS1.OwnBenchQuakeMon("Mini Earthquake", 10));
            r.RegisterByKey("DRM-33", new HydreigonDRM());
            r.RegisterByKey("DRM-34", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("DRM-36", new CoinsMon("Dragon Tail", 2));
            r.RegisterByKey("DRM-37", new ComboMon(new LockAfterAttackMon("Giga Impact", false), new DragonporterMon()));
            r.RegisterByKey("DRM-38", new CardsPAF2.IgnoreDefenderEffectsMon("Sonic Edge"));
            r.RegisterByKey("DRM-39", new FlygonDRM());
            r.RegisterByKey("DRM-42", new ComboMon(new TailsNothingMon("Risky Kick"), new ConditionAttackMon("Dragon Eye", SpecialCondition.Asleep)));
            r.RegisterByKey("DRM-43", new ShelgonDRM());
            r.RegisterByKey("DRM-44", new SalamenceGX());
            r.RegisterByKey("DRM-45", new CardsASC.CounterattackMon("Rough Skin", 3, null));
            r.RegisterByKey("DRM-46", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new CardsFST1.FlipRecoilMon("Bolt Strike", 50)));
            r.RegisterByKey("DRM-47", new ComboMon(new CardsASR1.PlusMon("Outrage", 10, SelfCounters), new Cards30CC.BenchHitMon("Frost Spear", 20)));
            r.RegisterByKey("DRM-48", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new CardsFST2.ConditionBonusMon("Raging Blade", 80, Damaged),
                new CardsCES1.ConditionsMon("Dragon Nova-GX", SpecialCondition.Burned, SpecialCondition.Paralyzed)));
            r.RegisterByKey("DRM-49", new ComboMon(new NoRetreatMon("Rumble"), new CardsFST2.ConditionBonusMon("Raging Blade", 60, Damaged)));
            r.RegisterByKey("DRM-50", new ExplosiveJetMon());
            r.RegisterByKey("DRM-51", new DragonWisdomMon());
            r.RegisterByKey("DRM-52", new ShieldAttackMon("Rigidify", 20));
            r.RegisterByKey("DRM-53", new ShieldAttackMon("Guard Press", 30));
            r.RegisterByKey("DRM-54", new ComboMon(new ShieldAttackMon("Guard Press", 30), new CardsFST2.ConditionBonusMon("Raging Uppercut", 120, c => c.Pokemon != null && c.Pokemon.DamageCounters >= 8)));
            r.RegisterByKey("DRM-55", new ComboMon(new DrawAttackMon("Fetch", 1), new FlipBonusMon("One-Two Punch", 30)));
            r.RegisterByKey("DRM-57", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("DRM-78", new UltraNecrozmaGX());

            r.RegisterByName("Dragon Talon", new DragonTalon());
            r.RegisterByName("Fiery Flint", new FieryFlint());
            r.RegisterByName("Lance ◇", new LancePrismStar());
            r.RegisterByName("Switch Raft", new SwitchRaft());
            r.RegisterByName("Wela Volcano Park", new WelaVolcanoPark());
            r.RegisterByName("Zinnia", new Zinnia());
        }

        private static int SelfCounters(EffectContext ctx) { return ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters; }

        private static bool Damaged(EffectContext ctx) { return ctx.Pokemon != null && ctx.Pokemon.DamageCounters > 0; }

        private static bool IsDragon(EffectContext ctx, PokemonInPlay mon) { return mon != null && ctx.Engine.IsType(mon, EnergyType.Dragon); }

        /// <summary>Puts Pokémon cards (any Stage) onto the Bench, as many as there is room for. Returns how many went.</summary>
        internal static int BenchCards(EffectContext ctx, IEnumerable<CardInstance> cards)
        {
            int n = 0;
            foreach (var card in cards)
            {
                if (ctx.BenchSpace <= 0) break;
                ctx.Engine.PutOntoBench(ctx.Player, card, false);
                n++;
            }
            return n;
        }

        // ================================================================== Fighting and Darkness

        /// <summary>
        /// Weed Out: once during your turn, choose 3 of your Benched Pokémon and discard the others (only with more than 3, or it would
        /// do nothing). Dark Destruction: you may discard an Energy from this Pokémon; if you do, discard an Energy from the opponent's
        /// Active Pokémon.
        /// </summary>
        private sealed class HydreigonDRM : Mon
        {
            protected override string Activated { get { return "Weed Out"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Count > 3; }

            protected override void Ability(EffectContext ctx)
            {
                var bench = ctx.MyBench();
                if (bench.Count <= 3) return;
                var keep = ctx.ChoosePokemonMany(bench, 3, 3, "Weed Out: choose 3 of your Benched Pokémon to keep (the others are discarded)");
                foreach (var mon in bench)
                    if (!keep.Contains(mon) && ctx.InPlay(mon)) ctx.Engine.DiscardPokemonFromPlay(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Destruction") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Dark Destruction: discard an Energy from " + me.Name + " to discard an Energy from " + defending.Name + "?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Dark Destruction: discard an Energy from " + me.Name) == 0) return true;
                if (!ctx.CanAffect(defending) || defending.Energy.Count == 0) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Dark Destruction: choose an Energy to discard from " + defending.Name)[0];
                ctx.Engine.DiscardEnergy(defending, card);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Dragonporter-GX: put up to 3 Dragon Pokémon from your discard pile onto your Bench.</summary>
        private sealed class DragonporterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragonporter-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var dragons = ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Dragon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                int n = Math.Min(3, Math.Min(ctx.BenchSpace, dragons.Count));
                if (n <= 0) return true;
                var picks = dragons.Count <= n ? dragons : ctx.ChooseCards(dragons, n, n, "Dragonporter-GX: choose " + n + " Dragon Pokémon to put onto your Bench");
                var moving = picks.Where(c => ctx.Me.Discard.Remove(c)).ToList();
                BenchCards(ctx, moving);
                return true;
            }
        }

        /// <summary>Dragon Guard: prevent all effects of the opponent's attacks, except damage, done to your Dragon Pokémon. Sand Tomb: no retreat.</summary>
        private sealed class FlygonDRM : Mon
        {
            private static readonly Mon Tomb = new NoRetreatMon("Sand Tomb");

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target == null || target.Owner != src.Player || kind != EffectSourceKind.Attack) return false;
                return IsDragon(src, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tomb.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Guard: with any basic Energy attached, this Pokémon takes 20 less damage from attacks (after Weakness and Resistance).</summary>
        private sealed class ShelgonDRM : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || !me.Energy.Any(e => e.Def.IsBasicEnergy)) return damage;
                return damage - 20;
            }
        }

        /// <summary>
        /// Dragon Lift: your Pokémon in play, except Pokémon-GX and Pokémon-EX, have no Retreat Cost. Bright Flame: discard 2 Energy.
        /// Flame Jet-GX: 120 damage to 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class SalamenceGX : Mon
        {
            private static readonly Mon Bright = new CardsTWM1.SelfDiscardMon("Bright Flame", 2);
            private static readonly Mon Jet = new CardsMEG.SnipeMon("Flame Jet-GX", 120);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || mon.Def.IsGXorEX) return cost;
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bright.HandleAttack(ctx, atk) || Jet.HandleAttack(ctx, atk); }
        }

        /// <summary>Explosive Jet: discard any amount of Fire Energy from your Pokémon; 50 damage for each card discarded.</summary>
        internal sealed class ExplosiveJetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explosive Jet") return false;
                var holders = new Dictionary<int, PokemonInPlay>();
                var fire = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var card in mon.Energy)
                        if (CardsTEU1.ProvidesType(ctx, mon, card, EnergyType.Fire))
                        {
                            fire.Add(card);
                            holders[card.Id] = mon;
                        }
                int n = 0;
                if (fire.Count > 0)
                {
                    var picks = ctx.ChooseCards(fire, 0, fire.Count, "Explosive Jet: discard any amount of Fire Energy from your Pokémon (" + atk.Damage + " damage each)");
                    foreach (var card in picks)
                    {
                        PokemonInPlay holder;
                        if (holders.TryGetValue(card.Id, out holder) && ctx.Engine.DiscardEnergy(holder, card)) n++;
                    }
                }
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log("Explosive Jet did no damage.");
                return true;
            }
        }

        /// <summary>Dragon Wisdom: attach a basic Energy card from your discard pile to 1 of your Dragon Pokémon.</summary>
        private sealed class DragonWisdomMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Wisdom") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                var dragons = ctx.MyPokemon(m => IsDragon(ctx, m));
                if (energy.Count == 0 || dragons.Count == 0) return true;
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Dragon Wisdom: choose a basic Energy card from your discard pile")[0];
                var target = ctx.ChoosePokemon(dragons, "Dragon Wisdom: attach " + card.Def.Name + " to which of your Dragon Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(card)) return true;
                ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>
        /// Photon Geyser: discard all basic Psychic Energy from this Pokémon, 80 more damage for each. Sky-Scorching Light-GX: only if
        /// both players have 6 or fewer Prize cards left between them; 6 damage counters on each of the opponent's Pokémon.
        /// </summary>
        private sealed class UltraNecrozmaGX : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Sky-Scorching Light-GX") return true;
                return ctx.Me.Prizes.Count + ctx.Opp.Prizes.Count <= 6;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Photon Geyser")
                {
                    var me = ctx.Pokemon;
                    int n = 0;
                    foreach (var card in me.Energy.Where(e => CardsTEU1.BasicOf(e.Def, EnergyType.Psychic)).ToList())
                        if (ctx.Engine.DiscardEnergy(me, card)) n++;
                    ctx.DamageDefending(atk.Damage + 80 * n);
                    return true;
                }
                if (atk.Name == "Sky-Scorching Light-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 6);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Trainers

        /// <summary>Dragon Talon: if the Dragon Pokémon holding this is your Active Pokémon and is damaged by an opponent's attack (even if it's Knocked Out), 3 damage counters on the Attacking Pokémon.</summary>
        private sealed class DragonTalon : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (!holderWasActive || holder == null || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (!IsDragon(ctx, holder)) return;
                ctx.Engine.PlaceDamageCounters(attacker, 3, "Dragon Talon");
            }
        }

        /// <summary>Fiery Flint: discard 2 other cards from your hand (a cost: not playable with an empty deck); search your deck for up to 4 Fire Energy cards.</summary>
        private sealed class FieryFlint : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) >= 2; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count < 2) return;
                var picks = ctx.ChooseCards(hand, 2, 2, "Fiery Flint: discard 2 other cards from your hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.SearchToHand(d => CardsTEU1.BasicOf(d, EnergyType.Fire), 4, "Fiery Flint: choose up to 4 Fire Energy cards");
            }
        }

        /// <summary>Lance ◇: only if 1 of your Pokémon was Knocked Out during the opponent's last turn. Search your deck for up to 2 Dragon Pokémon and put them onto your Bench.</summary>
        private sealed class LancePrismStar : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.WasKnockedOutLastOpponentTurn() && ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(2, ctx.BenchSpace);
                if (n <= 0) return;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.PokemonType == EnergyType.Dragon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    "Lance: choose up to " + n + " Dragon Pokémon to put onto your Bench");
                BenchCards(ctx, found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Switch Raft: switch your Active Water Pokémon with 1 of your Benched Pokémon; if you do, heal 30 damage from the one moved to the Bench.</summary>
        private sealed class SwitchRaft : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Bench.Count > 0 && ctx.Engine.IsType(ctx.Me.Active, EnergyType.Water);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var old = ctx.Me.Active;
                if (old == null || !ctx.Engine.IsType(old, EnergyType.Water)) return;
                if (!ctx.SwitchMyActive("Switch Raft: choose a Benched Pokémon to switch in")) return;
                if (ctx.InPlay(old) && old.DamageCounters > 0) ctx.Heal(old, 30);
            }
        }

        /// <summary>Wela Volcano Park: a Burned Pokémon (either player's) stays Burned even when its coin between turns is heads.</summary>
        private sealed class WelaVolcanoPark : CardScript
        {
            public override bool KeepsOpponentBurned(EffectContext src, int player) { return true; }
        }

        /// <summary>Zinnia: only if 1 of your Pokémon was Knocked Out during the opponent's last turn. Attach up to 2 basic Energy cards from your hand to 1 of your Dragon Pokémon.</summary>
        private sealed class Zinnia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.WasKnockedOutLastOpponentTurn() && ctx.MyPokemon(m => IsDragon(ctx, m)).Count > 0 && ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsBasicEnergy);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var dragons = ctx.MyPokemon(m => IsDragon(ctx, m));
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (dragons.Count == 0 || energy.Count == 0) return;
                var target = ctx.ChoosePokemon(dragons, "Zinnia: attach basic Energy to which of your Dragon Pokémon?");
                ctx.AttachCardsAnyWay(energy, 2, (c, m) => m == target, "Zinnia: attach basic Energy cards from your hand to " + target.Name);
            }
        }
    }
}
