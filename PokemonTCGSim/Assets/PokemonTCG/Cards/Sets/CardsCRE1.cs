using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Chilling Reign (CRE, 2021), Pokémon 1-96 and the full-art-only Galarian Rapidash V (167): Grass, Fire, Water, Lightning,
    /// Psychic and Fighting, and the Darkness Pokémon Weezing and Galarian Weezing. The rest of the Darkness Pokémon through
    /// Colorless, the secret-rare Electrode and Snorlax, the Trainers and the Special Energy are in CardsCRE2. Regulation mark
    /// E (Electrode and Snorlax: D): Expanded only.
    /// Already in the pool through Fusion Strike, Astral Radiance, Silver Tempest, Lost Origin, Crown Zenith and Evolving
    /// Skies: Celebi V, Abomasnow, Blaziken V, Blaziken VMAX, Snorunt, Froslass, Inteleon, Ice Rider Calyrex V and VMAX,
    /// Zeraora V, Galarian Articuno V, Gardevoir, Banette, Cresselia, Malamar, Shadow Rider Calyrex V and VMAX, Galarian
    /// Zapdos V, Rockruff, Passimian, Sandaconda V, Koffing, Galarian Moltres V, Blissey V, Castform, Melony and Path to the
    /// Peak.
    /// Pokémon whose attacks only do their printed damage need no script (Weedle, Snover, Bounsweet, Scorbunny, Spheal,
    /// Sealeo, Flaaffy, Galarian Slowpoke, Haunter, Shuppet, Golett, Inkay, Hatenna, Clobbopus).
    /// Your ruling (v59): Aggron's Seismic Rift also does its 30 damage to Aggron itself.
    /// </summary>
    public static class CardsCRE1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("CRE-2", new ShieldAttackMon("Stiffen", 40));
            r.RegisterByKey("CRE-3", new ComboMon(new PersistStingMon(), new CardsTWM1.SelfDiscardMon("Jet Spear", 1)));
            r.RegisterByKey("CRE-4", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("CRE-5", new ComboMon(new DrawAttackMon("Rapid Draw", 2), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("CRE-6", new CardsFST2.ConditionBonusMon("Single-Horn Throw", 160, BothCoinsHeads));
            r.RegisterByKey("CRE-8", new CelebiVmax());
            r.RegisterByKey("CRE-11", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("CRE-12", new CardsPAL1.StadiumBonusMon("Winter Horn", 80));
            r.RegisterByKey("CRE-14", new FlipBonusMon("Stomp", 30));
            r.RegisterByKey("CRE-15", new CardsOBF1.RetreatCostDamageMon("Tread On", 50, false));
            r.RegisterByKey("CRE-16", new CoinsMon("Hit Twice", 2));
            r.RegisterByKey("CRE-17", new CardsASC.RandomDiscardMon("Knock Off"));
            r.RegisterByKey("CRE-18", new ComboMon(new CardsTWM1.HealSelfMon("Wood Drain", 30), new RagingRepeatedStrikeMon()));
            r.RegisterByKey("CRE-19", new ComboMon(new PackCallMon(), new CardsMEG.EnergyBonusMon("Repeated Whip", EnergyType.Grass, 20)));
            // Fire
            r.RegisterByKey("CRE-22", new CastformCRE(new HighPressureBlastMon()));
            r.RegisterByKey("CRE-23", new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1));
            r.RegisterByKey("CRE-24", new CardsTWM1.SelfDiscardMon("Fire Spin", 2));
            r.RegisterByKey("CRE-25", new CardsFST2.ConditionBonusMon("Dynamite Tackle", 150, TenOrMoreCounters));
            r.RegisterByKey("CRE-27", new LockAfterAttackMon("Volley Kick", false));
            r.RegisterByKey("CRE-28", new CinderaceCRE());
            // Water
            r.RegisterByKey("CRE-29", new ComboMon(new SearchAttackMon("Wintry Call", IsMelony, 2), new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("CRE-30", new CardsASC.FlipDiscardDefendingEnergyMon("Cut Down"));
            r.RegisterByKey("CRE-31", new ComboMon(new TwoHitKoMon(), new SearchAttackMon("Nasty Plot", CardsEVS1.AnyCard, 2)));
            r.RegisterByKey("CRE-32", new PackageDeliveryMon());
            r.RegisterByKey("CRE-33", new CastformCRE(new RainfallMon("Rainfall", 20, 20)));
            r.RegisterByKey("CRE-34", new CastformCRE(new LockAfterAttackMon("Frosty Typhoon", true)));
            r.RegisterByKey("CRE-39", new HailPrisonMon());
            r.RegisterByKey("CRE-40", new ComboMon(new SwitchSelfMon("Smash Turn", true), new CardsDRI1.EnergyLoopMon("Ocean Loop")));
            r.RegisterByKey("CRE-41", new ComboMon(new KeepCallingMon(), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("CRE-42", new SwitchSelfMon("Bounce", false));
            r.RegisterByKey("CRE-44", new CardsASR1.TimesMon("Rapid-Fisted Rush", RapidStrikeInPlay));
            // Lightning
            r.RegisterByKey("CRE-47", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("CRE-49", new ComboMon(new ParalyzeFlipMon("Thunder Shock"), new SearchlightTailMon()));
            r.RegisterByKey("CRE-50", new CardsMEG.SnipeMon("Thunder Spear", 10));
            r.RegisterByKey("CRE-51", new ComboMon(new CardsFST2.ConditionBonusMon("Coordinated Bolt", 90, OtherRapidStrikeAttackedLastTurn), new CardsPFL.FlipUntilTailsTimesMon("Spark Rush")));
            r.RegisterByKey("CRE-52", new ComboMon(new AssistingSparkMon(), new RecoilMon("Thunder", 30)));
            // Psychic
            r.RegisterByKey("CRE-55", new CardsPAR2.FlipConditionMon("Sleep Pulse", SpecialCondition.Asleep));
            r.RegisterByKey("CRE-57", new GengarCRE());
            r.RegisterByKey("CRE-59", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("CRE-60", new MirageStepMon());
            r.RegisterByKey("CRE-66", new ComboMon(new CardsASC.ToolBonusMon("Reinforced Punch", 90), new RecoilMon("Megaton Fall", 30)));
            r.RegisterByKey("CRE-67", new CardsTWM1.HealSelfMon("Draining Kiss", 10));
            r.RegisterByKey("CRE-68", new FollowTheScentMon());
            r.RegisterByKey("CRE-72", new CardsTWM1.HealSelfMon("Spiral Drain", 30));
            r.RegisterByKey("CRE-73", new HattereneCRE());
            r.RegisterByKey("CRE-167", new ComboMon(new LibraHornMon(), new CardsMEG.DefendingEnergyBonusMon("Psychic", 30)));
            // Fighting
            r.RegisterByKey("CRE-76", new DodgeMon("Dig", false));
            r.RegisterByKey("CRE-77", new TripleHeadsMon());
            r.RegisterByKey("CRE-78", new CardsFLF1.NoResistanceMon("Puncture"));
            r.RegisterByKey("CRE-79", new LeekStrikeMon());
            r.RegisterByKey("CRE-81", new ComboMon(new CardsFLF1.NoResistanceMon("Feint"), new CardsASR1.TimesMon("Dynablade", OpponentsPokemonV)));
            r.RegisterByKey("CRE-82", new FlipBonusMon("Brutal Swing", 30));
            r.RegisterByKey("CRE-83", new RunerigusCRE());
            r.RegisterByKey("CRE-84", new CoinsMon("Double Lariat", 2));
            r.RegisterByKey("CRE-85", new CoinsMon("Double Lariat", 2));
            r.RegisterByKey("CRE-87", new CardsASR1.PlusMon("Rogue Fangs", 10, SingleStrikePokemonInDiscard));
            r.RegisterByKey("CRE-90", new ComboMon(new CardsMEW1.SpreadBenchMon("Sand Pulse", 20), new GMaxCycloneMon()));
            r.RegisterByKey("CRE-92", new GrapploctCRE());
            r.RegisterByKey("CRE-93", new TrainingMon());
            // Darkness
            r.RegisterByKey("CRE-95", new ComboMon(new MixinToxinMon(), new CardsASR1.PlusMon("Smog Burst", 20, DarknessEnergyInPlay)));
            r.RegisterByKey("CRE-96", new GalarianWeezingCRE());
        }

        // ================================================================== Filters and counters

        private static bool IsMelony(CardDef d) { return d.Name == "Melony"; }
        private static bool BothCoinsHeads(EffectContext ctx) { return ctx.FlipCoins(2) == 2; }
        private static bool TenOrMoreCounters(EffectContext ctx) { return ctx.Pokemon.DamageCounters >= 10; }
        internal static int RapidStrikeInPlay(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsRapidStrike).Count; }
        private static int OpponentsPokemonV(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Def.IsPokemonV).Count; }

        private static int SingleStrikePokemonInDiscard(EffectContext ctx)
        {
            return ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.IsSingleStrike);
        }

        private static int DarknessEnergyInPlay(EffectContext ctx)
        {
            return ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Darkness));
        }

        /// <summary>Coordinated Bolt: another of your Rapid Strike Pokémon (not this one) used an attack during your last turn.</summary>
        private static bool OtherRapidStrikeAttackedLastTurn(EffectContext ctx)
        {
            var me = ctx.Me;
            return me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackerDef != null && me.LastAttackerDef.IsRapidStrike
                   && me.LastAttackerUid != ctx.Pokemon.Uid;
        }

        // ================================================================== Grass

        /// <summary>Persist Sting: if the opponent's Active Pokémon has any Special Energy attached, it is Knocked Out.</summary>
        private sealed class PersistStingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Persist Sting") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy))
                    ctx.Engine.KnockOut(defending, "Persist Sting");
                return true;
            }
        }

        /// <summary>Curative Forest: once during your turn, heal 20 damage from each of your Grass Pokémon. Max Plant: search for up to 2 Pokémon.</summary>
        private sealed class CelebiVmax : Mon
        {
            private static readonly Mon MaxPlant = new SearchAttackMon("Max Plant", IsPokemonCard, 2);

            private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }

            protected override string Activated { get { return "Curative Forest"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Grass)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass))) ctx.Heal(mon, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MaxPlant.HandleAttack(ctx, atk); }
        }

        /// <summary>Raging Repeated Strike: discard any amount of Energy from your Pokémon; 30 more damage for each card discarded.</summary>
        private sealed class RagingRepeatedStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging Repeated Strike") return false;
                int discarded = 0;
                for (int guard = 0; guard < 60; guard++)
                {
                    var holders = ctx.MyPokemon(m => m.Energy.Count > 0);
                    if (holders.Count == 0) break;
                    var all = holders.SelectMany(m => m.Energy).ToList();
                    var pick = ctx.ChooseCards(all, 0, 1, "Raging Repeated Strike: discard an Energy from your Pokémon (+30 each), or choose none to stop (" + discarded + " so far)");
                    if (pick.Count == 0) break;
                    var holder = holders.First(m => m.Energy.Contains(pick[0]));
                    if (ctx.Engine.DiscardEnergy(holder, pick[0])) discarded++;
                    else break;
                }
                ctx.DamageDefending(atk.Damage + 30 * discarded);
                return true;
            }
        }

        /// <summary>Pack Call: search for a Grass Pokémon (up to 3 if you went second and it's your first turn).</summary>
        private sealed class PackCallMon : Mon
        {
            private static bool IsGrassPokemon(CardDef d) { return d.IsPokemon && d.HasType(EnergyType.Grass); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pack Call") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                int count = CardsEVS1.WentSecondFirstTurn(ctx) ? 3 : 1;
                ctx.SearchToHand(IsGrassPokemon, count, "Pack Call: search your deck for " + (count == 1 ? "a Grass Pokémon" : "up to 3 Grass Pokémon"));
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Weather Reading (the Castform forms): if you have 8 or more Stadium cards in your discard pile, ignore all Energy in
        /// this Pokémon's attack costs.
        /// </summary>
        private sealed class CastformCRE : Mon
        {
            private readonly Mon attacks;

            public CastformCRE(Mon attacks) { this.attacks = attacks; }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (src.Me.Discard.Count(c => c.Def.IsStadiumCard) >= 8) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>High-Pressure Blast: discard a Stadium in play; if you can't, this attack does nothing.</summary>
        private sealed class HighPressureBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High-Pressure Blast") return false;
                if (ctx.State.Stadium == null) return Nothing(ctx);
                ctx.Engine.DiscardStadium();
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Crisis Power: this Pokémon's attacks do 30 more damage to the opponent's Active Pokémon for each Prize card the
        /// opponent has taken (before Weakness and Resistance). Fireball Shot: can't attack during your next turn.
        /// </summary>
        private sealed class CinderaceCRE : Mon
        {
            private static readonly Mon FireballShot = new LockAfterAttackMon("Fireball Shot", false);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 30 * src.Engine.PrizesTaken(src.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FireballShot.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>
        /// Two-Hit KO: during your next turn, if the Defending Pokémon is damaged by an attack from one of your Rapid Strike
        /// Pokémon, it is Knocked Out. Ends if that Pokémon leaves the Active Spot.
        /// </summary>
        private sealed class TwoHitKoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Two-Hit KO") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new TwoHitKoEffect(defending.Name + " will be Knocked Out if a Rapid Strike Pokémon's attack damages it next turn (Two-Hit KO)",
                    ctx.Player, defending.Uid, ctx.Turn));
                return true;
            }
        }

        private sealed class TwoHitKoEffect : ContinuousEffect
        {
            public TwoHitKoEffect(string name, int owner, int targetUid, int currentTurn)
                : base(name, owner, targetUid, currentTurn + 2, true)
            {
                ActiveFromTurn = currentTurn + 2;
            }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || damage <= 0) return;
                var attacker = info.Attacker;
                if (attacker == null || attacker.Owner != Owner || !attacker.Def.IsRapidStrike) return;
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                if (engine.IsKnockedOut(info.Target)) return;
                engine.KnockOut(info.Target, "Two-Hit KO");
            }
        }

        /// <summary>Package Delivery: put this Pokémon and all attached cards into your deck; if you do, search your deck for a card.</summary>
        private sealed class PackageDeliveryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Package Delivery") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Engine.ShufflePokemonIntoDeck(me);
                if (ctx.InPlay(me)) return true;
                ctx.SearchToHand(CardsEVS1.AnyCard, 1, "Package Delivery: search your deck for a card", false);
                return true;
            }
        }

        /// <summary>Rainfall: 20 damage to each of the opponent's Pokémon (no Weakness or Resistance for Benched Pokémon). Used for Rainfall, Hail (the attack name and the numbers are passed in).</summary>
        internal sealed class RainfallMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly int amount2;

            public RainfallMon(string attack, int amount, int amount2)
            {
                this.attack = attack;
                this.amount = amount;
                this.amount2 = amount2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(amount);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, amount2);
                return true;
            }
        }

        /// <summary>Hail Prison: discard 2 Energy from this Pokémon; the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class HailPrisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hail Prison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, "Hail Prison: discard 2 Energy from " + me.Name);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Keep Calling: put up to 3 Basic Rapid Strike Pokémon from your deck onto your Bench.</summary>
        private sealed class KeepCallingMon : Mon
        {
            private static bool IsRapid(CardDef d) { return d.IsRapidStrike; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Keep Calling") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(IsRapid, 3, "Keep Calling: up to 3 Basic Rapid Strike Pokémon for your Bench");
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Searchlight Tail: the opponent reveals their hand; 90 more damage if there's any Energy card there.</summary>
        private sealed class SearchlightTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searchlight Tail") return false;
                TefPokemon.RevealOpponentHand(ctx);
                bool energy = ctx.Opp.Hand.Any(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + (energy ? 90 : 0));
                return true;
            }
        }

        /// <summary>Assisting Spark: you may attach a Lightning Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class AssistingSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assisting Spark") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Lightning)).ToList();
                if (pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                ctx.AttachCardsAnyWay(pile, 1, BenchedTarget(ctx), "Assisting Spark: attach a Lightning Energy card to a Benched Pokémon");
                return true;
            }

            private static Func<CardInstance, PokemonInPlay, bool> BenchedTarget(EffectContext ctx)
            {
                return (card, mon) => ctx.Me.Bench.Contains(mon);
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Last Gift: if an attack from the opponent's Pokémon Knocks Out this Pokémon, search your deck for up to 2 cards and
        /// put them into your hand (at least 1 if you can: Compendium, 2021-06-17). Pain Burst: 40 more damage for each damage
        /// counter on the opponent's Active Pokémon.
        /// </summary>
        private sealed class GengarCRE : Mon
        {
            private static readonly Mon PainBurst = new CardsMEW1.CounterBonusMon("Pain Burst", 40);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Me.Deck.Count == 0) return;
                Announce(src, "Last Gift");
                src.SearchToHand(CardsEVS1.AnyCard, 2, "Last Gift: search your deck for up to 2 cards", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PainBurst.HandleAttack(ctx, atk); }
        }

        /// <summary>Mirage Step: put up to 3 Kirlia from your deck onto your Bench.</summary>
        private sealed class MirageStepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirage Step") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int room = Math.Min(3, ctx.BenchSpace);
                if (room <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.Name == "Kirlia" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), room, "Mirage Step: up to " + room + " Kirlia for your Bench");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Follow the Scent: flip 3 coins; put up to that many cards from your discard pile into your hand.</summary>
        private sealed class FollowTheScentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Follow the Scent") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads == 0 || ctx.Me.Discard.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 0, Math.Min(heads, ctx.Me.Discard.Count), "Follow the Scent: put up to " + heads + (heads == 1 ? " card" : " cards") + " from your discard pile into your hand");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        /// <summary>
        /// Witch Rondo: once during your turn, switch your Active Pokémon with 1 of your Benched Pokémon; if you do, your
        /// opponent switches their Active Pokémon with 1 of their Benched Pokémon. Psychic: 50 more per Energy on their Active.
        /// </summary>
        private sealed class HattereneCRE : Mon
        {
            private static readonly Mon Psychic = new CardsMEG.DefendingEnergyBonusMon("Psychic", 50);

            protected override string Activated { get { return "Witch Rondo"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.SwitchMyActive("Witch Rondo: choose your new Active Pokémon")) return;
                ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psychic.HandleAttack(ctx, atk); }
        }

        /// <summary>Libra Horn: put damage counters on 1 of the opponent's Pokémon until its remaining HP is 100.</summary>
        private sealed class LibraHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Libra Horn") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => ctx.RemainingHp(m) > 100);
                if (targets.Count == 0)
                {
                    ctx.Log("Libra Horn: none of the opponent's Pokémon has more than 100 HP left.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Libra Horn: put damage counters on which Pokémon until it has 100 HP left?");
                int counters = (ctx.RemainingHp(target) - 100) / 10;
                if (counters > 0) ctx.PlaceCounters(target, counters);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Triple Heads: flip 3 coins, 60 for each heads; on 3 heads, prevent all damage and effects of attacks next turn.</summary>
        private sealed class TripleHeadsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Heads") return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (heads == 3 && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Leek Strike: with a Pokémon Tool attached, 90 more damage and Resistance doesn't apply.</summary>
        private sealed class LeekStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leek Strike") return false;
                if (ctx.Pokemon.Tools.Count == 0)
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage + 90, true, false, false);
                return true;
            }
        }

        /// <summary>
        /// Spiteful Slate: if this Pokémon is in the Active Spot and is damaged by an attack from the opponent's Pokémon VMAX
        /// (even if it's Knocked Out), put damage counters on the Attacking Pokémon equal to the damage done. Energy Press: 20
        /// more for each Energy attached to the opponent's Active Pokémon.
        /// </summary>
        private sealed class RunerigusCRE : Mon
        {
            private static readonly Mon EnergyPress = new CardsMEG.DefendingEnergyBonusMon("Energy Press", 20);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !attacker.Def.IsVmax || damage < 10) return;
                if (!ctx.InPlay(attacker)) return;
                Announce(ctx, "Spiteful Slate");
                ctx.Engine.PlaceDamageCounters(attacker, damage / 10, "Spiteful Slate");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return EnergyPress.HandleAttack(ctx, atk); }
        }

        /// <summary>G-Max Cyclone: move any amount of Energy from your Pokémon to your other Pokémon in any way you like.</summary>
        private sealed class GMaxCycloneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                for (int guard = 0; guard < 40; guard++)
                {
                    if (ctx.MyPokemon().Count < 2) break;
                    var holders = ctx.MyPokemon(m => m.Energy.Count > 0);
                    if (holders.Count == 0) break;
                    var all = holders.SelectMany(m => m.Energy).ToList();
                    var pick = ctx.ChooseCards(all, 0, 1, "G-Max Cyclone: choose an Energy to move, or none to stop");
                    if (pick.Count == 0) break;
                    var giver = holders.First(m => m.Energy.Contains(pick[0]));
                    var taker = ctx.ChoosePokemon(ctx.MyPokemon(m => m != giver), "G-Max Cyclone: move " + pick[0].Def.Name + " to which Pokémon?");
                    if (taker == null || !ctx.Engine.MoveEnergy(giver, taker, pick[0])) break;
                }
                return true;
            }
        }

        /// <summary>
        /// Stranglehold Master: while this Pokémon is in the Active Spot, the opponent's Active Pokémon's Retreat Cost is 2 more.
        /// Synchro Buster: 80 more if both players have the same number of cards in their hands.
        /// </summary>
        private sealed class GrapploctCRE : Mon
        {
            private static readonly Mon SynchroBuster = new CardsFST2.ConditionBonusMon("Synchro Buster", 80, CardsEVS1.SameHandSize);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || mon.Owner == src.Player || !src.IsActive(mon)) return cost;
                return cost + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SynchroBuster.HandleAttack(ctx, atk); }
        }

        /// <summary>Training: search your deck for a basic Energy card and attach it to this Pokémon.</summary>
        private sealed class TrainingMon : Mon
        {
            private static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Training") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                var found = ctx.SearchMyDeck(IsBasicEnergyCard, 1, "Training: search your deck for a basic Energy card");
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Mixin' Toxin: the opponent's Active Pokémon is now Confused; attach a Darkness Energy card from your discard pile to this Pokémon.</summary>
        private sealed class MixinToxinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mixin' Toxin") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var pile = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Darkness)).ToList();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, 1, SelfOnly(me), "Mixin' Toxin: attach a Darkness Energy card to " + me.Name, false);
                return true;
            }

            private static Func<CardInstance, PokemonInPlay, bool> SelfOnly(PokemonInPlay me)
            {
                return (card, mon) => mon == me;
            }
        }

        /// <summary>
        /// Energy Factory: each basic Darkness Energy attached to your Pokémon that have "Weezing" in their name provides 2
        /// Darkness Energy. More than 1 Energy Factory doesn't add more (it's applied once).
        /// </summary>
        private sealed class GalarianWeezingCRE : Mon
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || holder.Owner != src.Player || !holder.Name.Contains("Weezing")) return;
                if (!CardsSSP1.IsBasicOf(energy.Def, EnergyType.Darkness)) return;
                units.Clear();
                units.Add(EnergyMask.Of(EnergyType.Darkness));
                units.Add(EnergyMask.Of(EnergyType.Darkness));
            }
        }
    }
}
