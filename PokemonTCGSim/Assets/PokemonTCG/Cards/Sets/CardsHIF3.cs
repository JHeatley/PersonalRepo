using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Hidden Fates (HIF) part 3: the Shiny Vault's Pokémon-GX SV50-SV93 and its Trainers SV81-SV90 (SV68 Buzzwole-GX, SV86 Lady and
    /// SV94 Tapu Lele-GX are in the pool already). Your ruling (v69): Extra Energy Bomb attaches what there is with fewer than 5
    /// Energy cards in the discard pile. Compendium: Eternal Flame-GX can put a Stage 2 straight onto the Bench (2017-08-03);
    /// Haze Slash's Pokémon is replaced before Knock Outs are checked (2018-05-03); Shell Trap's counters are an attack effect
    /// (2017-08-03); Banette-GX's Shady Move moves 1 counter between any Pokémon, each Banette-GX once a turn (2018-08-02); Darkrai-GX's
    /// Restoration needs only room on the Bench, and can be used again once it's back in the discard pile (2017); Dark Call-GX
    /// takes 2 Energy from one Pokémon or 1 from each of two (2017-06-27); Berserk needs one damaged Benched Pokémon (2017-07-06);
    /// Stinger-GX keeps the Prize cards already taken as taken (2018); Guzma isn't playable with no opposing Bench, and if the
    /// opponent's switch is stopped yours doesn't happen (2017-08); Brooklet Hill can't be used with a full Bench; Fisherman takes
    /// all of the basic Energy with fewer than 4 (2010).
    /// </summary>
    public static class CardsHIF3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HIF-SV50", new ComboMon(new CardsMEG.SnipeMon("Sacred Fire", 50), new LockAfterAttackMon("Phoenix Burn", true), new EternalFlameMon()));
            r.RegisterByKey("HIF-SV51", new ComboMon(new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 2), new ConditionAttackMon("Scorching Column", SpecialCondition.Burned), new VermilionMon()));
            r.RegisterByKey("HIF-SV52", new ComboMon(new CardsFST2.ShellTrapMon(), new CardsASC.SelfDiscardTypeMon("Bright Flame", EnergyType.Fire, 2), new NitroTankMon()));
            r.RegisterByKey("HIF-SV53", new ComboMon(new CardsMEG.SnipeMon("Ice Blade", 50), new CardsTWM1.SelfDiscardMon("Blizzard Edge", 2), new IcePathMon()));
            r.RegisterByKey("HIF-SV54", new ArticunoGX());
            r.RegisterByKey("HIF-SV55", new GlaceonGX());
            r.RegisterByKey("HIF-SV56", new GreninjaGX());
            r.RegisterByKey("HIF-SV57", new ElectrodeGX());
            r.RegisterByKey("HIF-SV58", new XurkitreeGX());
            r.RegisterByKey("HIF-SV59", new ComboMon(new CardsASR1.TimesMon("Full Burst", OwnEnergy), new CardsTWM1.HealSelfMon("Super Absorption", 30), new CardsPAF2.IgnoreDefenderEffectsMon("Psystrike-GX")));
            r.RegisterByKey("HIF-SV60", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new CardsASR1.PlusMon("Psychic", 30, DefendingEnergy), new CardsLOR1.CountersAnyWayMon("Divide-GX", 10)));
            r.RegisterByKey("HIF-SV61", new BanetteGX());
            r.RegisterByKey("HIF-SV62", new NihilegoGX());
            r.RegisterByKey("HIF-SV63", new ComboMon(new CardsASR1.TimesMon("Beast Raid", UltraBeasts), new CardsFCO2.NoWeaknessResistanceMon("Jet Needle"), new StingerMon()));
            r.RegisterByKey("HIF-SV64", new ComboMon(new CardsRCL1.EvolvedFromBonusMon("Aura Strike", "Riolu", 90), new CardsASR1.TimesMon("Cantankerous Beatdown-GX", OwnCounters)));
            r.RegisterByKey("HIF-SV65", new ComboMon(new CardsSP1.AttachUpToFromDiscardToSelfMon("Cell Connector", EnergyType.Fighting, 2), new GxShieldMon("Verdict-GX")));
            r.RegisterByKey("HIF-SV66", new LycanrocBloodthirstyGX());
            r.RegisterByKey("HIF-SV67", new LycanrocTwilightGX());
            r.RegisterByKey("HIF-SV69", new ComboMon(new SwitchSelfMon("Strafe", true), new Cards30CC.BenchHitMon("Shadow Bullet", 30), new DarkCallMon()));
            r.RegisterByKey("HIF-SV70", new DarkraiGX());
            r.RegisterByKey("HIF-SV71", new ComboMon(new EatSloppilyMon(), new GluttonMon()));
            r.RegisterByKey("HIF-SV72", new ScizorGX());
            r.RegisterByKey("HIF-SV73", new KartanaGX());
            r.RegisterByKey("HIF-SV74", new StakatakaGX());
            r.RegisterByKey("HIF-SV75", new GardevoirGX());
            r.RegisterByKey("HIF-SV76", new ComboMon(new SearchAttackMon("Magical Ribbon", CardsHIF1.AnyCard, 3), new PleaMon()));
            r.RegisterByKey("HIF-SV77", new ComboMon(new GxShieldMon("Bright Tone"), new CardsPAF2.IgnoreDefenderEffectsMon("Sonic Edge"), new EuphoriaMon()));
            r.RegisterByKey("HIF-SV78", new ComboMon(new CardsDAA1.ItemLockAttackMon("Distort"), new SonicVolumeMon(), new CardsCEC3.SpreadAmountMon("Boomburst-GX", 50)));
            r.RegisterByKey("HIF-SV79", new SilvallyGyroGX());
            r.RegisterByKey("HIF-SV80", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Righteous Edge", false), new CardsSVP.BenchHurtBonusMon("Berserk", 70), new BigWheelMon()));
            r.RegisterByKey("HIF-SV91", new ComboMon(new NaturesJudgmentMon(60), new CardsTWM1.HealSelfMon("Tapu Wilderness-GX", 999)));
            r.RegisterByKey("HIF-SV92", new ComboMon(new SwitchSelfMon("Aqua Ring", true), new CardsHIF1.DiscardThenSnipeMon("Hydro Shot", EnergyType.Water, 2, 120), new TapuStormMon()));
            r.RegisterByKey("HIF-SV93", new TapuKokoGX());

            r.RegisterByName("Aether Foundation Employee", new AetherFoundationEmployee());
            r.RegisterByName("Cynthia", new Cynthia());
            r.RegisterByName("Fisherman", new Fisherman());
            r.RegisterByName("Guzma", new Guzma());
            r.RegisterByName("Hiker", new Hiker());
            r.RegisterByName("Aether Paradise Conservation Area", new AetherParadiseConservationArea());
            r.RegisterByName("Brooklet Hill", new BrookletHill());
            r.RegisterByName("Mt. Coronet", new MtCoronet());
            r.RegisterByName("Shrine of Punishment", new ShrineOfPunishment());
        }

        // ================================================================== Shared helpers

        private static int OwnEnergy(EffectContext ctx) { return ctx.EnergyCount(ctx.Pokemon); }
        private static int OwnCounters(EffectContext ctx) { return ctx.Pokemon.DamageCounters; }
        private static int DefendingEnergy(EffectContext ctx) { return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending); }
        private static int UltraBeasts(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsUltraBeast).Count; }
        private static int OpponentBenched(EffectContext ctx) { return ctx.Opp.Bench.Count; }
        private static int BothActivesEnergy(EffectContext ctx)
        {
            return (ctx.Me.Active == null ? 0 : ctx.EnergyCount(ctx.Me.Active)) + (ctx.Opp.Active == null ? 0 : ctx.EnergyCount(ctx.Opp.Active));
        }

        /// <summary>"Put N cards from your discard pile into your hand" (Hollow Hunt-GX, Tomb Hunt-GX).</summary>
        internal static void DiscardToHandAttack(EffectContext ctx, int count, string attack)
        {
            int n = Math.Min(count, ctx.Me.Discard.Count);
            if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), n, n, attack + ": put " + count + " cards from your discard pile into your hand"));
        }

        /// <summary>"You may shuffle this Pokémon and all cards attached to it into your deck" after the damage (Haze Slash, Gale Blade).</summary>
        internal static void MayShuffleSelf(EffectContext ctx, string attack)
        {
            var me = ctx.Pokemon;
            if (!ctx.InPlay(me) || !ctx.AskYesNo(attack + ": shuffle " + me.Name + " and all attached cards into your deck?")) return;
            ctx.Engine.ShufflePokemonIntoDeck(me);
        }

        /// <summary>Prize cards put in or taken out without being taken (Symbiont-GX, Lightning-GX, Stinger-GX) keep the count of Prize cards taken.</summary>
        internal static void AddToPrizes(EffectContext ctx, int player, List<CardInstance> cards)
        {
            var p = ctx.State.Players[player];
            foreach (var card in cards) p.Prizes.Add(card);
            p.PrizeAdjust += cards.Count;
        }

        /// <summary>"Prevent all damage done to this Pokémon by attacks from Pokémon-GX and Pokémon-EX during your opponent's next turn."</summary>
        internal sealed class GxShieldEffect : ContinuousEffect
        {
            public GxShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && info.Attacker.Def.IsGXorEX ? 0 : damage;
            }
        }

        /// <summary>Verdict-GX, Bright Tone: after the damage, the GX shield for the opponent's next turn.</summary>
        private sealed class GxShieldMon : Mon
        {
            private readonly string attack;

            public GxShieldMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new GxShieldEffect(ctx.Pokemon.Name + " takes no damage from attacks by Pokémon-GX and Pokémon-EX during the opponent's next turn (" + attack + ")", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Attacks

        /// <summary>Eternal Flame-GX: put 3 Fire Pokémon-GX or Fire Pokémon-EX from your discard pile onto your Bench (as many as there's room for).</summary>
        private sealed class EternalFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eternal Flame-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pool = ctx.Me.Discard.Where(x => x.Def.IsPokemon && x.Def.IsGXorEX && x.Def.HasType(EnergyType.Fire) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, x.Def)).ToList();
                int n = Math.Min(3, Math.Min(pool.Count, ctx.BenchSpace));
                if (n <= 0) return true;
                foreach (var card in ctx.ChooseCards(pool, n, n, "Eternal Flame-GX: put " + n + " Fire Pokémon-GX or Pokémon-EX onto your Bench"))
                    if (ctx.BenchSpace > 0 && ctx.Me.Discard.Remove(card)) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                return true;
            }
        }

        /// <summary>Vermilion-GX: attach up to 5 basic Fire Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class VermilionMon : Mon
        {
            private static bool AnyOfMine(CardInstance card, PokemonInPlay mon) { return true; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vermilion-GX") return false;
                ctx.DamageDefending(atk.Damage);
                var fire = ctx.Me.Hand.Where(x => CardsEVS1.IsEnergyOf(x.Def, EnergyType.Fire)).ToList();
                if (fire.Count > 0) ctx.AttachCardsAnyWay(fire, 5, AnyOfMine, "Vermilion-GX: attach Fire Energy from your hand");
                return true;
            }
        }

        /// <summary>Nitro Tank-GX: attach 5 basic Fire Energy cards from your discard pile to your Pokémon in any way you like.</summary>
        private sealed class NitroTankMon : Mon
        {
            private static bool AnyOfMine(CardInstance card, PokemonInPlay mon) { return true; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nitro Tank-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count > 0) ctx.AttachCardsAnyWay(fire, Math.Min(5, fire.Count), AnyOfMine, "Nitro Tank-GX: attach Fire Energy from your discard pile", false);
                return true;
            }
        }

        /// <summary>Ice Path-GX: move all damage counters from this Pokémon to your opponent's Active Pokémon.</summary>
        private sealed class IcePathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Path-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Defending != null && me.DamageCounters > 0) ctx.Engine.MoveDamageCounters(me, ctx.Defending, me.DamageCounters, "Ice Path-GX");
                return true;
            }
        }

        /// <summary>Stinger-GX: each player shuffles their Prize cards into their deck, then puts the top 3 cards of their deck face down as Prize cards.</summary>
        private sealed class StingerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stinger-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    int before = p.Prizes.Count;
                    p.Deck.AddRange(p.Prizes);
                    p.Prizes.Clear();
                    p.FaceUpPrizeIds.Clear();
                    ctx.Engine.ShuffleDeck(player);
                    int n = Math.Min(3, p.Deck.Count);
                    var top = p.Deck.Take(n).ToList();
                    p.Deck.RemoveRange(0, n);
                    p.Prizes.AddRange(top);
                    p.PrizeAdjust += n - before;
                    ctx.Log("Stinger-GX: " + ctx.Engine.PlayerName(player) + " now has " + n + (n == 1 ? " Prize card." : " Prize cards."));
                }
                ctx.Engine.CheckWinConditions();
                return true;
            }
        }

        /// <summary>Dark Call-GX: discard 2 Energy from your opponent's Pokémon (2 from one, or 1 each from two).</summary>
        private sealed class DarkCallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Call-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.OpponentPokemon())
                    if (ctx.CanAffect(mon))
                        foreach (var e in mon.Energy) holders[e] = mon;
                int n = Math.Min(2, holders.Count);
                if (n == 0) return true;
                foreach (var e in ctx.ChooseCards(holders.Keys.ToList(), n, n, "Dark Call-GX: discard 2 Energy from your opponent's Pokémon"))
                    ctx.Engine.DiscardEnergy(holders[e], e);
                return true;
            }
        }

        /// <summary>Eat Sloppily: discard the top 5 cards of your deck; attach the Energy cards among them to this Pokémon.</summary>
        private sealed class EatSloppilyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eat Sloppily") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                // Beast Energy ◇ (v84): the Energy is attached straight away, so a Prism Star card never stays in the Lost Zone
                // (Compendium, 2018-05-31).
                foreach (var card in top)
                    if (card.Def.IsEnergy && ctx.InPlay(ctx.Pokemon) && (ctx.Me.Discard.Remove(card) || (card.Def.IsPrismStar && ctx.Me.LostZone.Remove(card))))
                        ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Glutton-GX: if the opponent's Pokémon is Knocked Out by damage from this attack, take 2 more Prize cards.</summary>
        private sealed class GluttonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Glutton-GX") return false;
                var defending = ctx.Defending;
                if (defending != null)
                    ctx.AddEffect(new KnockOutPrizeBonusEffect("Take 2 more Prize cards if " + defending.Name + " is Knocked Out by Glutton-GX", ctx.Player, defending.Uid, ctx.Turn, 2));
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Plea-GX: put 2 of your opponent's Benched Pokémon and all attached cards into their hand.</summary>
        private sealed class PleaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plea-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench().Where(x => ctx.CanAffect(x)).ToList();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                foreach (var mon in ctx.ChoosePokemonMany(bench, n, n, "Plea-GX: put 2 of your opponent's Benched Pokémon into their hand"))
                    if (ctx.InPlay(mon)) ctx.Engine.ReturnPokemonToHand(mon);
                return true;
            }
        }

        /// <summary>Euphoria-GX: your opponent's Active Pokémon is now Asleep; heal all damage from all of your Pokémon.</summary>
        private sealed class EuphoriaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Euphoria-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Asleep);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Sonic Volume: your opponent can't play Special Energy cards from their hand during their next turn.</summary>
        private sealed class SonicVolumeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Volume") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsPAF1.SpecialEnergyLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Special Energy cards during their next turn (Sonic Volume)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Big Wheel-GX: shuffle your hand into your deck, then draw 10 cards.</summary>
        private sealed class BigWheelMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Wheel-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Me.Deck.AddRange(ctx.Me.Hand);
                ctx.Me.Hand.Clear();
                ctx.ShuffleMyDeck();
                ctx.Draw(10);
                return true;
            }
        }

        /// <summary>Nature's Judgment: you may discard all Energy from this Pokémon; if you do, N more damage.</summary>
        private sealed class NaturesJudgmentMon : Mon
        {
            private readonly int bonus;

            public NaturesJudgmentMon(int bonus) { this.bonus = bonus; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nature's Judgment") return false;
                var me = ctx.Pokemon;
                int extra = 0;
                if (me.Energy.Count > 0 && ctx.AskYesNo("Nature's Judgment: discard all Energy from " + me.Name + " for " + bonus + " more damage?"))
                    if (ctx.Engine.DiscardAllEnergy(me) > 0) extra = bonus;
                ctx.DamageDefending(atk.Damage + extra);
                return true;
            }
        }

        /// <summary>Tapu Storm-GX: shuffle your opponent's Active Pokémon and all attached cards into their deck; nothing if they have no Benched Pokémon.</summary>
        private sealed class TapuStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tapu Storm-GX") return false;
                var defending = ctx.Defending;
                if (defending == null || ctx.Opp.Bench.Count == 0) return Nothing(ctx);
                if (ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        // ================================================================== Pokémon with Abilities

        /// <summary>
        /// Legendary Ascent: when you play this Pokémon from your hand onto your Bench, you may switch it with your Active Pokémon and
        /// move any number of Water Energy from your other Pokémon to it. Cold Crush-GX: discard all Energy from both Active Pokémon.
        /// </summary>
        private sealed class ArticunoGX : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.Me.Active == null || !TriggerOnce(ctx, "Legendary Ascent", true, "Use Legendary Ascent? Switch " + me.Name + " with your Active Pokémon and move Water Energy to it.")) return;
                if (!ctx.Engine.SwitchActive(ctx.Player, me)) return;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != me))
                    foreach (var e in mon.Energy)
                        if (CardsHIF1.Provides(ctx, mon, e, EnergyType.Water)) holders[e] = mon;
                if (holders.Count == 0) return;
                foreach (var e in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Legendary Ascent: move any number of Water Energy to " + me.Name))
                    ctx.Engine.MoveEnergy(holders[e], me, e);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cold Crush-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending)) ctx.Engine.DiscardAllEnergy(defending);
                return true;
            }
        }

        /// <summary>
        /// Freezing Gaze: while this Pokémon is Active, your opponent's Pokémon-GX and Pokémon-EX in play, in their hand and in their
        /// discard pile have no Abilities, except Freezing Gaze. Frost Bullet: 30 to 1 Benched Pokémon. Polar Spear-GX: 50 for each
        /// damage counter on the opponent's Active Pokémon.
        /// </summary>
        private sealed class GlaceonGX : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new Cards30CC.BenchHitMon("Frost Bullet", 30), new CardsASC.CounterMultiplierMon("Polar Spear-GX"));

            private static bool HasFreezingGaze(CardDef d)
            {
                foreach (var ab in d.Abilities) if (ab.Name == "Freezing Gaze") return true;
                return false;
            }

            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && mon.Owner != src.Player && mon.Def.IsGXorEX && abilityName != "Freezing Gaze";
            }

            public override bool BlocksOpponentsAbilitiesOutOfPlay(EffectContext src, CardDef card)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && card.IsGXorEX && !HasFreezingGaze(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Shuriken Flurry: when you play this Pokémon from your hand to evolve, you may put 3 damage counters on 1 of your opponent's Pokémon. Haze Slash: you may shuffle it into your deck.</summary>
        private sealed class GreninjaGX : Mon
        {
            private static readonly Mon ShadowyHunter = new CardsMEG.SnipeBenchMon("Shadowy Hunter-GX", 130);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Shuriken Flurry", ctx.Opp.HasPokemonInPlay, "Use Shuriken Flurry? Put 3 damage counters on 1 of your opponent's Pokémon.")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Shuriken Flurry: put 3 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Haze Slash") return ShadowyHunter.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                MayShuffleSelf(ctx, "Haze Slash");
                return true;
            }
        }

        /// <summary>
        /// Extra Energy Bomb: once during your turn, attach 5 Energy cards from your discard pile to your Pokémon other than Pokémon-GX
        /// and Pokémon-EX (what there is, with fewer: your ruling); if you do, this Pokémon is Knocked Out. Crush and Burn-GX: discard
        /// any Energy from your Pokémon, 50 more damage for each card.
        /// </summary>
        private sealed class ElectrodeGX : Mon
        {
            private const string Bomb = "Extra Energy Bomb";

            protected override string Activated { get { return Bomb; } }

            private static bool NotGxOrEx(CardInstance card, PokemonInPlay mon) { return !mon.Def.IsGXorEX; }

            private static List<CardInstance> EnergyCards(EffectContext ctx) { return ctx.Me.Discard.Where(x => x.Def.IsEnergy).ToList(); }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == Bomb; }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return EnergyCards(ctx).Count > 0 && ctx.MyPokemon().Any(m => !m.Def.IsGXorEX);
            }

            protected override void Ability(EffectContext ctx)
            {
                var pile = EnergyCards(ctx);
                int attached = ctx.AttachCardsAnyWay(pile, Math.Min(5, pile.Count), NotGxOrEx, Bomb + ": attach Energy from your discard pile", false);
                if (attached > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Engine.KnockOut(ctx.Pokemon, Bomb);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crush and Burn-GX") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var e in mon.Energy) holders[e] = mon;
                int discarded = 0;
                if (holders.Count > 0)
                    foreach (var e in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Crush and Burn-GX: discard any Energy from your Pokémon (50 more damage each)"))
                        if (ctx.Engine.DiscardEnergy(holders[e], e)) discarded++;
                ctx.DamageDefending(atk.Damage + 50 * discarded);
                return true;
            }
        }

        /// <summary>
        /// Flashing Head: prevent all damage done to this Pokémon by attacks from the opponent's Pokémon that have any Special Energy
        /// attached. Rumbling Wires: discard the top card of their deck. Lightning-GX: they reveal their hand; put a card from it
        /// face down as 1 of their Prize cards.
        /// </summary>
        private sealed class XurkitreeGX : Mon
        {
            private static readonly Mon RumblingWires = new CardsSSP2.StompOffMon("Rumbling Wires", 1);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                return src.Pokemon != null && info.Target == src.Pokemon && attacker != null && attacker.Owner != src.Player && attacker.Energy.Any(x => x.Def.IsAttachedSpecialEnergy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning-GX") return RumblingWires.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                if (ctx.Opp.Hand.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Opp.Hand.ToList(), 1, 1, "Lightning-GX: put which card from your opponent's hand face down as 1 of their Prize cards?");
                if (pick.Count == 0 || !ctx.Opp.Hand.Remove(pick[0])) return true;
                AddToPrizes(ctx, ctx.Opponent, pick);
                ctx.Log("Lightning-GX: a card from " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand became 1 of their Prize cards.");
                return true;
            }
        }

        /// <summary>
        /// Shady Move: once during your turn, if this Pokémon is Active, move 1 damage counter from 1 Pokémon to another (any
        /// Pokémon in play). Shadow Chant: 10 more for each Supporter in your discard pile (up to 100). Tomb Hunt-GX: 3 cards from your discard pile into your hand.
        /// </summary>
        private sealed class BanetteGX : Mon
        {
            private static readonly Mon ShadowChant = new CardsASR1.PlusMon("Shadow Chant", 10, SupportersInDiscard);

            private static int SupportersInDiscard(EffectContext ctx) { return Math.Min(10, ctx.Me.Discard.Count(x => x.Def.IsSupporter)); }

            private static List<PokemonInPlay> All(EffectContext ctx) { return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList(); }

            protected override string Activated { get { return "Shady Move"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var all = All(ctx);
                return ctx.PokemonIsActive && all.Count >= 2 && all.Any(m => m.DamageCounters > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                var all = All(ctx);
                var from = ctx.ChoosePokemon(all.Where(m => m.DamageCounters > 0).ToList(), "Shady Move: move a damage counter from which Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(all.Where(m => m != from).ToList(), "Shady Move: move it to which Pokémon?");
                if (to != null) ctx.Engine.MoveDamageCounters(from, to, 1, "Shady Move");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tomb Hunt-GX") return ShadowChant.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                DiscardToHandAttack(ctx, 3, atk.Name);
                return true;
            }
        }

        /// <summary>Empty Light: when you play this Pokémon from your hand onto your Bench, you may leave both Active Pokémon Confused and Poisoned. Lock Up: no retreat. Symbiont-GX: top 2 of their deck become Prize cards.</summary>
        private sealed class NihilegoGX : Mon
        {
            private static readonly Mon LockUp = new NoRetreatMon("Lock Up");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Me.Active == null && ctx.Opp.Active == null) return;
                if (!TriggerOnce(ctx, "Empty Light", true, "Use Empty Light? Both Active Pokémon become Confused and Poisoned.")) return;
                foreach (var mon in new[] { ctx.Me.Active, ctx.Opp.Active })
                {
                    if (mon == null || !ctx.CanAffect(mon)) continue;
                    ctx.ApplyCondition(mon, SpecialCondition.Confused);
                    ctx.ApplyCondition(mon, SpecialCondition.Poisoned);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Symbiont-GX") return LockUp.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(2, ctx.Opp.Deck.Count);
                if (n == 0) return true;
                var top = ctx.Opp.Deck.Take(n).ToList();
                ctx.Opp.Deck.RemoveRange(0, n);
                AddToPrizes(ctx, ctx.Opponent, top);
                ctx.Log("Symbiont-GX: the top " + n + " cards of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck became Prize cards.");
                return true;
            }
        }

        /// <summary>Bloodthirsty Eyes: when you play this Pokémon from your hand to evolve, you may switch 1 of your opponent's Benched Pokémon with their Active Pokémon. Dangerous Rogue-GX: 50 for each of their Benched Pokémon.</summary>
        private sealed class LycanrocBloodthirstyGX : Mon
        {
            private static readonly Mon Rogue = new CardsASR1.TimesMon("Dangerous Rogue-GX", OpponentBenched);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Bloodthirsty Eyes", ctx.GustTargets().Count > 0 && ctx.Opp.Active != null, "Use Bloodthirsty Eyes? Switch 1 of your opponent's Benched Pokémon with their Active Pokémon.")) return;
                ctx.GustOpponent("Bloodthirsty Eyes: choose 1 of your opponent's Benched Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Rogue.HandleAttack(ctx, atk); }
        }

        /// <summary>Twilight Eyes: when you play this Pokémon from your hand to evolve, you may discard an Energy from your opponent's Active Pokémon. Splintered Shards-GX: 30 for each Energy card in their discard pile.</summary>
        private sealed class LycanrocTwilightGX : Mon
        {
            private static readonly Mon Shards = new CardsASR1.TimesMon("Splintered Shards-GX", EnergyInOpponentDiscard);

            private static int EnergyInOpponentDiscard(EffectContext ctx) { return ctx.Opp.Discard.Count(x => x.Def.IsEnergy); }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                bool ready = defending != null && defending.Energy.Count > 0 && ctx.CanAffect(defending);
                if (!TriggerOnce(ctx, "Twilight Eyes", ready, "Use Twilight Eyes? Discard an Energy from your opponent's Active Pokémon.")) return;
                ctx.DiscardEnergyFrom(defending, 1, "Twilight Eyes: discard which Energy from " + defending.Name + "?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shards.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Restoration: once during your turn, if this Pokémon is in your discard pile, put it onto your Bench, then attach a basic
        /// Darkness Energy card from your discard pile to it. Dark Cleave: not affected by Resistance. Dead End-GX: if the
        /// opponent's Active Pokémon has a Special Condition, it's Knocked Out.
        /// </summary>
        private sealed class DarkraiGX : Mon
        {
            private static readonly Mon DarkCleave = new CardsFLF1.NoResistanceMon("Dark Cleave");

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.BenchSpace > 0 && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Darkrai-GX used its Ability Restoration.", ctx.Card.Id, mon.Uid);
                var dark = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Darkness);
                if (dark.Count > 0 && ctx.Me.Discard.Remove(dark[0])) ctx.Engine.AttachEnergyCard(dark[0], mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dead End-GX") return DarkCleave.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending != null && defending.Conditions != SpecialCondition.None && ctx.CanAffect(defending)) ctx.Engine.KnockOut(defending, "Dead End-GX");
                else ctx.Log("Dead End-GX did nothing: the opponent's Active Pokémon has no Special Condition.");
                return true;
            }
        }

        /// <summary>Danger Perception: with 100 HP or less left, this Pokémon's attacks do 80 more to the opponent's Active Pokémon. Steel Wing: 30 less damage next turn. Cross-Cut-GX: 100 more against an Evolution Pokémon.</summary>
        private sealed class ScizorGX : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASC.SelfGuardMon("Steel Wing", 30), new CardsFST2.ConditionBonusMon("Cross-Cut-GX", 100, DefendingEvolved));

            private static bool DefendingEvolved(EffectContext ctx) { return ctx.Defending != null && ctx.Defending.Def.IsEvolution; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.RemainingHp(src.Pokemon) <= 100 ? damage + 80 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Slice Off: when you play this Pokémon from your hand onto your Bench, you may discard a Special Energy from 1 of your opponent's Pokémon. Gale Blade: may shuffle itself into the deck. Blade-GX: take a Prize card.</summary>
        private sealed class KartanaGX : Mon
        {
            private static List<PokemonInPlay> WithSpecial(EffectContext ctx)
            {
                return ctx.OpponentPokemon(m => m.Energy.Any(x => x.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m));
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Slice Off", WithSpecial(ctx).Count > 0, "Use Slice Off? Discard a Special Energy from 1 of your opponent's Pokémon.")) return;
                var target = ctx.ChoosePokemon(WithSpecial(ctx), "Slice Off: discard a Special Energy from which Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Slice Off: discard which Special Energy?", x => x.Def.IsAttachedSpecialEnergy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gale Blade")
                {
                    ctx.DamageDefending(atk.Damage);
                    MayShuffleSelf(ctx, "Gale Blade");
                    return true;
                }
                if (atk.Name != "Blade-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Prizes.Count > 0) ctx.Engine.TakePrizeCards(ctx.Player, 1);
                return true;
            }
        }

        /// <summary>Ultra Wall: your Ultra Beasts take 10 less damage from your opponent's attacks. Assembly-GX: 50 more for each Prize card you have taken.</summary>
        private sealed class StakatakaGX : Mon
        {
            private static readonly Mon Assembly = new CardsASR1.PlusMon("Assembly-GX", 50, PrizesTaken);

            private static int PrizesTaken(EffectContext ctx) { return ctx.MyPrizesTaken; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                if (src.Pokemon == null || target == null || target.Owner != src.Player || !target.Def.IsUltraBeast || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return Math.Max(0, damage - 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Assembly.HandleAttack(ctx, atk); }
        }

        /// <summary>Secret Spring: once during your turn, attach a basic Fairy Energy card from your hand to 1 of your Pokémon. Infinite Force: 30 for each Energy on both Active Pokémon. Twilight-GX: shuffle 10 cards from your discard pile into your deck.</summary>
        private sealed class GardevoirGX : Mon
        {
            private static readonly Mon InfiniteForce = new CardsASR1.TimesMon("Infinite Force", BothActivesEnergy);

            private static bool IsFairyEnergyCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Fairy); }

            protected override string Activated { get { return "Secret Spring"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(x => IsFairyEnergyCard(x.Def)) && ctx.MyPokemon().Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.Me.Hand.First(x => IsFairyEnergyCard(x.Def));
                var target = ctx.ChoosePokemon(ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)), "Secret Spring: attach a Fairy Energy card to which Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twilight-GX") return InfiniteForce.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(10, ctx.Me.Discard.Count);
                if (n == 0) return true;
                ctx.DiscardToDeck(ctx.ChooseCards(ctx.Me.Discard.ToList(), n, n, "Twilight-GX: shuffle 10 cards from your discard pile into your deck"));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Gyro Unit: your Basic Pokémon in play have no Retreat Cost. Turbo Drive: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon. Rebel-GX: 50 for each of the opponent's Benched Pokémon.</summary>
        private sealed class SilvallyGyroGX : Mon
        {
            private static readonly Mon Rebel = new CardsASR1.TimesMon("Rebel-GX", OpponentBenched);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && mon.Def.IsBasicPokemon ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Drive") return Rebel.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                CardsSSP1.EnergyAssist(ctx, 1, "Turbo Drive");
                return true;
            }
        }

        /// <summary>
        /// Aero Trail: when you play this Pokémon from your hand onto your Bench, you may move any number of Lightning Energy from your
        /// other Pokémon to it; if you do, switch it with your Active Pokémon. Tapu Thunder-GX: 50 for each Energy on all of your
        /// opponent's Pokémon.
        /// </summary>
        private sealed class TapuKokoGX : Mon
        {
            private static readonly Mon TapuThunder = new CardsASR1.TimesMon("Tapu Thunder-GX", OpponentEnergy);

            private static int OpponentEnergy(EffectContext ctx) { return ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m)); }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != me))
                    foreach (var e in mon.Energy)
                        if (CardsHIF1.Provides(ctx, mon, e, EnergyType.Lightning)) holders[e] = mon;
                if (!TriggerOnce(ctx, "Aero Trail", holders.Count > 0, "Use Aero Trail? Move Lightning Energy to " + me.Name + " and switch it with your Active Pokémon.")) return;
                int moved = 0;
                foreach (var e in ctx.ChooseCards(holders.Keys.ToList(), 1, holders.Count, "Aero Trail: move any number of Lightning Energy to " + me.Name))
                    if (ctx.Engine.MoveEnergy(holders[e], me, e)) moved++;
                if (moved > 0 && ctx.InPlay(me) && ctx.Me.Bench.Contains(me)) ctx.Engine.SwitchActive(ctx.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TapuThunder.HandleAttack(ctx, atk); }
        }

        // ================================================================== Supporters

        /// <summary>Aether Foundation Employee: put 3 Pokémon with "Alolan" in their names from your discard pile into your hand (all of them with fewer).</summary>
        private sealed class AetherFoundationEmployee : CardScript
        {
            private static List<CardInstance> Alolan(EffectContext ctx) { return ctx.Me.Discard.Where(x => x.Def.IsPokemon && x.Def.Name.Contains("Alolan")).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Alolan(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pool = Alolan(ctx);
                int n = Math.Min(3, pool.Count);
                if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(pool, n, n, "Aether Foundation Employee: put 3 Alolan Pokémon into your hand"));
            }
        }

        /// <summary>Cynthia: shuffle your hand into your deck, then draw 6 cards.</summary>
        private sealed class Cynthia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count + CardsHIF1.OthersInHand(ctx) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Me.Deck.AddRange(ctx.Me.Hand);
                ctx.Me.Hand.Clear();
                ctx.ShuffleMyDeck();
                ctx.Draw(6);
            }
        }

        /// <summary>Fisherman: put 4 basic Energy cards from your discard pile into your hand (all of them with fewer).</summary>
        private sealed class Fisherman : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pool = ctx.BasicEnergyInDiscard();
                int n = Math.Min(4, pool.Count);
                if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(pool, n, n, "Fisherman: put 4 basic Energy cards into your hand"));
            }
        }

        /// <summary>Guzma: switch 1 of your opponent's Benched Pokémon with their Active Pokémon; if you do, switch your Active Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class Guzma : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.GustOpponent("Guzma: choose 1 of your opponent's Benched Pokémon to switch in") == null) return;
                if (ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Guzma: choose 1 of your Benched Pokémon to switch in");
            }
        }

        /// <summary>Hiker: look at the top 5 cards of either player's deck and choose 1; that player shuffles the others back, then the chosen card goes on top.</summary>
        private sealed class Hiker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Opp.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int player;
                if (ctx.Me.Deck.Count == 0) player = ctx.Opponent;
                else if (ctx.Opp.Deck.Count == 0) player = ctx.Player;
                else player = ctx.ChooseOption("Hiker: look at the top 5 cards of which deck?", "Your deck", "Your opponent's deck") == 0 ? ctx.Player : ctx.Opponent;
                var deck = ctx.State.Players[player].Deck;
                var top = deck.Take(Math.Min(5, deck.Count)).ToList();
                ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of " + (player == ctx.Player ? "your" : "your opponent's") + " deck", ctx.Player);
                var pick = ctx.ChooseCards(top, 1, 1, "Hiker: choose the card to put on top of that deck")[0];
                deck.Remove(pick);
                ctx.Engine.ShuffleDeck(player);
                deck.Insert(0, pick);
                ctx.Log("Hiker: " + ctx.Engine.PlayerName(player) + "'s deck was shuffled with the chosen card put on top.");
            }
        }

        // ================================================================== Stadiums

        /// <summary>Aether Paradise Conservation Area: Basic Grass and Basic Lightning Pokémon (both players') take 30 less damage from the opponent's attacks.</summary>
        private sealed class AetherParadiseConservationArea : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                if (target == null || !target.Def.IsBasicPokemon || !info.AgainstOpponent || damage <= 0) return damage;
                if (!src.Engine.IsType(target, EnergyType.Grass) && !src.Engine.IsType(target, EnergyType.Lightning)) return damage;
                return Math.Max(0, damage - 30);
            }
        }

        /// <summary>Brooklet Hill: once during each player's turn, that player may put a Basic Water or Basic Fighting Pokémon from their deck onto their Bench.</summary>
        private sealed class BrookletHill : StadiumAction
        {
            private static bool WaterOrFighting(CardDef d) { return d.HasType(EnergyType.Water) || d.HasType(EnergyType.Fighting); }

            protected override bool Ready(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToBench(WaterOrFighting, 1, "Brooklet Hill: put a Basic Water or Fighting Pokémon onto your Bench");
                return false;
            }
        }

        /// <summary>Mt. Coronet: once during each player's turn, that player may put 2 basic Metal Energy cards from their discard pile into their hand.</summary>
        private sealed class MtCoronet : StadiumAction
        {
            private static List<CardInstance> Metal(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal); }

            protected override bool Ready(EffectContext ctx) { return Metal(ctx).Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player); }

            protected override bool Use(EffectContext ctx)
            {
                var pool = Metal(ctx);
                int n = Math.Min(2, pool.Count);
                if (n > 0) ctx.DiscardToHand(ctx.ChooseCards(pool, n, n, "Mt. Coronet: put 2 Metal Energy cards into your hand"));
                return false;
            }
        }

        /// <summary>Shrine of Punishment: between turns, 1 damage counter on each Pokémon-GX and Pokémon-EX (both players').</summary>
        private sealed class ShrineOfPunishment : CardScript
        {
            public override int CheckupAffects(EffectContext src) { return -1; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                for (int i = 0; i < 2; i++)
                    foreach (var mon in src.State.Players[i].InPlay().ToList())
                        if (mon.Def.IsGXorEX) src.Engine.PlaceDamageCounters(mon, 1, "Shrine of Punishment");
            }
        }
    }
}
