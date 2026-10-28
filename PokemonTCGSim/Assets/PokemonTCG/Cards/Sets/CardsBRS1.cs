using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Brilliant Stars (BRS, 2022), Pokémon 1-128 plus Pikachu V (157). The Trainer Gallery, the Trainers and Magma Basin are
    /// in CardsBRS2. Many BRS cards came into the pool earlier through Crown Zenith's Galarian Gallery (Turtwig, Charizard V
    /// and VSTAR, Magmortar, Entei V, Lumineon V, Manaphy, Raikou V, Duskull, Riolu, Zamazenta V, Arceus VSTAR...), Silver
    /// Tempest, Lost Origin, Paldea Evolved and Astral Radiance (Double Turbo Energy). Regulation mark F: Expanded only.
    /// Pokémon whose attacks only do their printed damage need no script (Exeggcute, Shroomish, Burmy, Karrablast, Magmar,
    /// Prinplup, Cubchoo, Shinx, Clefairy, Nosepass, Vibrava, Throh, Golett, Sneasel, Morgrem, Klink, Gible, Gabite, Fraxure,
    /// Staravia, Bidoof).
    /// Your rulings (v54): Clefable's Moonlit Miracle and Axew's Ultra Evolution ignore the timing limits on evolving.
    /// </summary>
    public static class CardsBRS1
    {
        private static readonly Mon MatronsAnger = new CardsASR1.PlusMon("Matron's Anger", 10, PokemonInDiscard);

        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("BRS-2", new CardsTWM1.HealSelfMon("Mega Drain", 30));
            r.RegisterByKey("BRS-4", new BreloomBRS());
            r.RegisterByKey("BRS-5", new TropiusBRS());
            r.RegisterByKey("BRS-7", new GrotleBRS());
            r.RegisterByKey("BRS-8", new CardsASR1.TimesMon("Evopress", MyEvolutionPokemon));
            r.RegisterByKey("BRS-10", new ComboMon(MatronsAnger, new CardsTWM1.HealSelfMon("Leaf Drain", 30)));
            r.RegisterByKey("BRS-11", new RaidMon());
            r.RegisterByKey("BRS-12", new LivelyFruitMon());
            r.RegisterByKey("BRS-13", new CardsASR1.PlusMon("Revenge Blast", 40, OpponentPrizesTaken));
            r.RegisterByKey("BRS-14", new ShayminVstar());
            r.RegisterByKey("BRS-16", new ComboMon(new Cards30CC.BenchHitMon("Leap to Leap", 30), new CardsCRZ1.VBonusMon("Jungle Rage", 120)));
            // Fire
            r.RegisterByKey("BRS-21", new InfernoWingsMon());
            r.RegisterByKey("BRS-23", new ComboMon(new FlipBonusMon("Firebreathing", 30), new ShieldAttackMon("Guard Press", 30)));
            r.RegisterByKey("BRS-24", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("BRS-25", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("BRS-26", new ComboMon(new InfernalVortexMon(), new CardsTWM1.SelfDiscardMon("Burning Kick", 99)));
            // Water
            r.RegisterByKey("BRS-28", new ComboMon(new BubblesMon("Falling Bubbles", true), new RecoilMon("Raging Pincer", 30)));
            r.RegisterByKey("BRS-29", new ComboMon(new BubblesMon("Bubbles Galore", false), new RecoilMon("G-Max Pincer", 30)));
            r.RegisterByKey("BRS-30", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("BRS-33", new CardsTWM1.SelfDiscardMon("Crab Impact", 2));
            r.RegisterByKey("BRS-34", new ParalyzeFlipMon("Ice Breath"));
            r.RegisterByKey("BRS-35", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("BRS-37", new EmpoleonBRS());
            r.RegisterByKey("BRS-38", new DodgeMon("Agility", false));
            r.RegisterByKey("BRS-39", new FloatifyMon());
            r.RegisterByKey("BRS-43", new CardsBLK.NoAttackNextTurnMon("Sheer Cold"));
            r.RegisterByKey("BRS-44", new BlockSliderMon());
            // Lightning
            r.RegisterByKey("BRS-45", new RaichuV());
            r.RegisterByKey("BRS-46", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("BRS-50", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("BRS-51", new ComboMon(new CardsASR1.TimesMon("Energy Crush", OpponentEnergyInPlay), new CardsASR1.BenchRecoilMon("Flash Impact", 30)));
            r.RegisterByKey("BRS-52", new CardsASR1.TimesMon("Windup Thunder", ToolsOnMyPokemon));
            // Psychic
            r.RegisterByKey("BRS-54", new MoonlitMiracleMon());
            r.RegisterByKey("BRS-55", new CardsMEG.DefendingEnergyBonusMon("Psychic", 30));
            r.RegisterByKey("BRS-56", new CardsTWM1.HealSelfMon("Life Sucker", 20));
            r.RegisterByKey("BRS-57", new ComboMon(new Cards30C.SelfCounterBonusMon("Chomp", 10), new RecoilMon("Bull Dash", 30)));
            r.RegisterByKey("BRS-58", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("BRS-59", new CoincidingFiguresMon());
            r.RegisterByKey("BRS-62", new DusknoirBRS());
            r.RegisterByKey("BRS-63", new SearchAttackMon("Clear Tone", IsSpecialEnergyCard, 2));
            r.RegisterByKey("BRS-64", new ComboMon(new FluffMon(), new ShieldAttackMon("Cotton Guard", 30)));
            r.RegisterByKey("BRS-65", new ComboMon(new TrickWindMon(), new FluffballStarMon()));
            r.RegisterByKey("BRS-66", new ComboMon(new TriRechargeMon(), new CardsMEG.DefendingEnergyBonusMon("Psychic", 30)));
            r.RegisterByKey("BRS-67", new DedeFlashMon());
            r.RegisterByKey("BRS-68", new MimikyuV());
            r.RegisterByKey("BRS-69", new ComboMon(new OminousNumbersMon(), new CardsASC.RandomDiscardMon("Max Shadow")));
            r.RegisterByKey("BRS-70", new SearchAttackMon("Lead", IsSupporterCard, 1));
            r.RegisterByKey("BRS-71", new CardsASR1.PlusMon("Rainbow Flavor", 40, BasicEnergyTypesInPlay));
            r.RegisterByKey("BRS-72", new DrawAttackMon("Spinning Draw", 1));
            // Fighting
            r.RegisterByKey("BRS-74", new FlipBonusMon("Rising Lunge", 10));
            r.RegisterByKey("BRS-76", new CardsOBF1.RetreatCostDamageMon("Desert Pillar", 50, true));
            r.RegisterByKey("BRS-77", new ComboMon(MatronsAnger, new NoRetreatMon("Bind Down")));
            r.RegisterByKey("BRS-79", new LucarioBRS());
            r.RegisterByKey("BRS-81", new CardsASC.FlipDiscardDefendingEnergyMon("Sweep the Leg"));
            r.RegisterByKey("BRS-83", new CardsASR1.PlusMon("Big Hand", 10, HandSize));
            // Darkness
            r.RegisterByKey("BRS-84", new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("BRS-85", new MukBRS());
            r.RegisterByKey("BRS-87", new RansackMon());
            r.RegisterByKey("BRS-88", new HonchkrowV());
            r.RegisterByKey("BRS-89", new ComboMon(new TickingTerrorMon(), new CardsLOR1.CountersAnyWayMon("Cursed Drop", 2)));
            r.RegisterByKey("BRS-90", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("BRS-91", new LiepardBRS());
            r.RegisterByKey("BRS-92", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("BRS-94", new LonghairShotMon());
            r.RegisterByKey("BRS-95", new ComboMon(new SwitchSelfMon("Gnaw and Run", true), new SupporterBonusMon("Hangry Spike", "Marnie's Pride", 120)));
            // Metal
            r.RegisterByKey("BRS-96", new ComboMon(new RockSlideMon(), new CardsPFL.DamagedBonusMon("Merciless Strike", 150)));
            r.RegisterByKey("BRS-97", new ComboMon(new CardsSSP2.StompOffMon("Cracking Stomp", 1), new RecoilMon("Max Take Down", 30)));
            r.RegisterByKey("BRS-98", MatronsAnger);
            r.RegisterByKey("BRS-99", new ComboMon(new CardsWHT.DragOffMon("Magnetic Tension", 40), new RecoilMon("Iron Tackle", 20)));
            r.RegisterByKey("BRS-100", new ComboMon(new ShieldAttackMon("Guard Claw", 30), new Cards30C.TypeEnergyBonusMon("Iron Hammer", EnergyType.Fire, 80)));
            r.RegisterByKey("BRS-101", new EscavalierBRS());
            r.RegisterByKey("BRS-103", new ShieldAttackMon("Guard Press", 30));
            r.RegisterByKey("BRS-104", new KlinklangBRS());
            // Dragon
            r.RegisterByKey("BRS-106", new DraconicImpulseMon());
            r.RegisterByKey("BRS-109", new GarchompBRS());
            r.RegisterByKey("BRS-110", new UltraEvolutionMon());
            r.RegisterByKey("BRS-112", new RecoilMon("Wild Axe", 30));
            r.RegisterByKey("BRS-114", new ComboMon(new SloshNCrashMon(), new LockAfterAttackMon("Dragon Strike", true)));
            // Colorless
            r.RegisterByKey("BRS-115", new CardsMEW1.CounterBonusMon("Leek Lash", 10));
            r.RegisterByKey("BRS-116", new ComboMon(new DrawAttackMon("Double Draw", 2), new HurricaneMon()));
            r.RegisterByKey("BRS-119", new ComboMon(new StrongBreezeMon(), new CardsTWM1.SelfDiscardMon("Spinning Bird", 2)));
            r.RegisterByKey("BRS-122", new TrinityChargeMon());
            r.RegisterByKey("BRS-124", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("BRS-125", new CoinsMon("Triple Axel", 3));
            r.RegisterByKey("BRS-126", new TornadusBRS());
            r.RegisterByKey("BRS-127", new ComboMon(new ShowboatingPoseMon(), new CardsPAL1.EvolutionBonusMon("Cross-Cut", 30)));
            r.RegisterByKey("BRS-128", new ComboMon(new DrawAttackMon("Spike Draw", 2), new MillSelfMon("Dragon Pulse", 2)));
            // Full-art only Pokémon
            r.RegisterByKey("BRS-157", new LightningBlastMon());
        }

        // ================================================================== Filters and counters

        private static bool IsSpecialEnergyCard(CardDef d) { return d.IsSpecialEnergy; }
        private static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }

        private static int PokemonInDiscard(EffectContext ctx) { return ctx.Me.Discard.Count(c => c.Def.IsPokemon); }
        private static int OpponentPrizesTaken(EffectContext ctx) { return ctx.OpponentPrizesTaken; }
        private static int MyEvolutionPokemon(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsEvolution).Count; }
        private static int ToolsOnMyPokemon(EffectContext ctx) { return ctx.MyPokemon().Sum(m => m.Tools.Count); }
        private static int HandSize(EffectContext ctx) { return ctx.Me.Hand.Count; }

        private static int OpponentEnergyInPlay(EffectContext ctx)
        {
            return ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
        }

        /// <summary>Rainbow Flavor: the different types of basic Energy cards attached to all of your Pokémon.</summary>
        private static int BasicEnergyTypesInPlay(EffectContext ctx)
        {
            var types = new HashSet<EnergyType>();
            foreach (var mon in ctx.Me.InPlay())
                foreach (var card in mon.Energy)
                    if (card.Def.IsBasicEnergy) types.Add(card.Def.EnergyProvides);
            return types.Count;
        }

        // ================================================================== Shared helpers

        /// <summary>
        /// "Search your deck for up to N [cards] and attach them to your [Pokémon] in any way you like. Then, shuffle your deck."
        /// The cards found are all attached (to eligible Pokémon); anything that couldn't be goes back before the shuffle.
        /// </summary>
        internal static int SearchAttachAnyWay(EffectContext ctx, Func<CardDef, bool> filter, int max, Func<PokemonInPlay, bool> eligible, string name, string what)
        {
            if (ctx.Me.Deck.Count == 0) return 0;
            if (!ctx.MyPokemon().Any(eligible))
            {
                ctx.ShuffleMyDeck();
                return 0;
            }
            var found = ctx.SearchMyDeck(filter, max, name + ": search your deck for up to " + max + " " + what);
            int attached = found.Count > 0 ? ctx.AttachCardsAnyWay(found, found.Count, (card, target) => eligible(target), name, false) : 0;
            foreach (var c in found)
                if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c)) && !ctx.Me.Deck.Contains(c)) ctx.Me.Deck.Add(c);
            ctx.ShuffleMyDeck();
            return attached;
        }

        /// <summary>"If you played [Supporter] from your hand during this turn, this attack does N more damage" (Hangry Spike).</summary>
        internal sealed class SupporterBonusMon : Mon
        {
            private readonly string attack;
            private readonly string supporter;
            private readonly int bonus;

            public SupporterBonusMon(string attack, string supporter, int bonus)
            {
                this.attack = attack;
                this.supporter = supporter;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.PlayedSupporterThisTurn(supporter) ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Discard the top N cards of your deck" after the damage (Dragonblade, Dragon Pulse).</summary>
        internal sealed class MillSelfMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public MillSelfMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Player, count);
                return true;
            }
        }

        /// <summary>
        /// Mimikyu V's Dummy Doll: "prevent all damage done to this Mimikyu V by attacks from your opponent's Pokémon until the
        /// end of your opponent's next turn". An Ability's effect, so it stays if the Pokémon changes spots.
        /// </summary>
        internal sealed class DummyDollEffect : ContinuousEffect
        {
            public DummyDollEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent ? 0 : damage;
            }
        }

        /// <summary>Trick Wind: during the opponent's next turn, they can't play Pokémon Tool or Special Energy cards from their hand.</summary>
        internal sealed class TrickWindEffect : ContinuousEffect
        {
            private readonly int affected;

            public TrickWindEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card.Def.IsTool;
            }

            public override bool BlocksEnergyCard(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card.Def.IsSpecialEnergy;
            }
        }

        // ================================================================== Grass

        /// <summary>Spore Ball: Asleep. Powdery Uppercut: only if this Pokémon used Spore Ball during your last turn.</summary>
        private sealed class BreloomBRS : Mon
        {
            private static readonly Mon SporeBall = new ConditionAttackMon("Spore Ball", SpecialCondition.Asleep);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Powdery Uppercut") return true;
                var me = ctx.Pokemon;
                return me.LastAttackName == "Spore Ball" && me.LastAttackTurn == ctx.MyLastTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SporeBall.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Curative Bower: your Pokémon that have Grass Energy attached can't be Confused, and recover if they are (checked when
        /// Energy is attached from the hand, when Tropius is played and at the end of each turn and in Pokémon Checkup, like
        /// Therapeutic Energy).
        /// </summary>
        private sealed class TropiusBRS : Mon
        {
            private static bool Covered(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && src.Engine.HasEnergyOfType(target, EnergyType.Grass);
            }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return (condition & SpecialCondition.Confused) != 0 && Covered(src, target);
            }

            private static void Recover(EffectContext src)
            {
                if (src.Pokemon == null) return;
                foreach (var mon in src.MyPokemon())
                    if (mon.Has(SpecialCondition.Confused) && Covered(src, mon)) src.Engine.RemoveCondition(mon, SpecialCondition.Confused);
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Recover(src); }
            public override void OnPlayedFromHandToBench(EffectContext ctx) { Recover(ctx); }
            public override void OnPokemonCheckup(EffectContext src) { Recover(src); }
            public override void OnEndOfEachTurn(EffectContext src) { Recover(src); }
        }

        /// <summary>Sun-Drenched Shell: once during your turn, search your deck for a Grass Pokémon and put it into your hand.</summary>
        private sealed class GrotleBRS : Mon
        {
            protected override string Activated { get { return "Sun-Drenched Shell"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(IsGrassPokemon, 1, "Sun-Drenched Shell: choose a Grass Pokémon to put into your hand");
            }

            private static bool IsGrassPokemon(CardDef d) { return d.IsPokemon && d.HasType(EnergyType.Grass); }
        }

        /// <summary>Raid: 90 more damage if this Pokémon evolved from Burmy during this turn.</summary>
        private sealed class RaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raid") return false;
                var me = ctx.Pokemon;
                bool fromBurmy = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Burmy";
                ctx.DamageDefending(atk.Damage + (fromBurmy ? 90 : 0));
                return true;
            }
        }

        /// <summary>Lively Fruit, Cocoon Cover, Defensive Scales, Unaware: prevent all effects of the opponent's attacks done to this Pokémon (damage isn't an effect).</summary>
        internal sealed class LivelyFruitMon : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>Star Bloom (VSTAR Power): heal 120 damage from each of your Benched Grass Pokémon. Revenge Blast: 40 more per Prize the opponent took.</summary>
        private sealed class ShayminVstar : Mon
        {
            private static readonly Mon RevengeBlast = new CardsASR1.PlusMon("Revenge Blast", 40, OpponentPrizesTaken);

            protected override string Activated { get { return "Star Bloom"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Bench.Any(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Grass));
            }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyBench())
                    if (mon.DamageCounters > 0 && ctx.Engine.IsType(mon, EnergyType.Grass)) ctx.Heal(mon, 120);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return RevengeBlast.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Inferno Wings: 70 more damage if this Pokémon has any damage counters on it; not affected by Weakness.</summary>
        private sealed class InfernoWingsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inferno Wings") return false;
                int damage = atk.Damage + (ctx.Pokemon.DamageCounters > 0 ? 70 : 0);
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, damage, false, true, false);
                return true;
            }
        }

        /// <summary>
        /// Infernal Vortex: reveal the top 5 cards of your deck; 80 damage for each Energy card there. Then discard those Energy
        /// cards and shuffle the rest back into your deck.
        /// </summary>
        private sealed class InfernalVortexMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Infernal Vortex") return false;
                var top = ctx.LookAtTop(5);
                if (top.Count > 0) ctx.Engine.Reveal(ctx.Player, top, "revealed the top " + top.Count + (top.Count == 1 ? " card" : " cards") + " of their deck");
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0) ctx.DamageDefending(atk.Damage * energy.Count);
                else ctx.Log(ctx.Pokemon.Name + "'s Infernal Vortex found no Energy, so it did no damage.");
                foreach (var card in energy)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Discard.Add(card);
                if (top.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Falling Bubbles (flip a coin first) / Bubbles Galore: search your deck for up to 5 Water Energy cards and attach them
        /// to your Pokémon in any way you like.
        /// </summary>
        private sealed class BubblesMon : Mon
        {
            private readonly string attack;
            private readonly bool flip;

            public BubblesMon(string attack, bool flip)
            {
                this.attack = attack;
                this.flip = flip;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (flip && !ctx.Flip()) return true;
                SearchAttachAnyWay(ctx, IsWaterEnergy, 5, AnyPokemon, attack, "Water Energy cards");
                return true;
            }

            private static bool IsWaterEnergy(CardDef d) { return CardsSSP1.IsBasicOf(d, EnergyType.Water); }
            private static bool AnyPokemon(PokemonInPlay mon) { return true; }
        }

        /// <summary>
        /// Emergency Surfacing: once during your turn, if this Pokémon is in your discard pile and you have no cards in your hand,
        /// you may put it onto your Bench and draw 3 cards. Water Arrow: 60 damage to 1 of your opponent's Pokémon.
        /// </summary>
        private sealed class EmpoleonBRS : Mon
        {
            private const string Surfacing = "Emergency Surfacing";
            private static readonly Mon WaterArrow = new CardsMEG.SnipeMon("Water Arrow", 60);

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.Me.Hand.Count == 0 && ctx.BenchSpace > 0
                       && !ctx.NamedLimitUsedThisTurn(Surfacing + "#" + ctx.Card.Id) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                ctx.MarkNamedLimit(Surfacing + "#" + ctx.Card.Id);
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Empoleon used its Ability Emergency Surfacing.", ctx.Card.Id, mon.Uid);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WaterArrow.HandleAttack(ctx, atk); }
        }

        /// <summary>Floatify: put up to 2 Item cards from your discard pile into your hand.</summary>
        private sealed class FloatifyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Floatify") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                var picks = ctx.ChooseCards(items, 0, Math.Min(2, items.Count), "Floatify: put up to 2 Item cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Block Slider: 40 damage to 1 of your opponent's Pokémon for each Fusion Strike Energy attached to all of your Pokémon.</summary>
        private sealed class BlockSliderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Block Slider") return false;
                int n = ctx.MyPokemon().Sum(m => m.Energy.Count(e => e.Def.Name == "Fusion Strike Energy"));
                if (n > 0) ctx.DamageAnyOpponent(40 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Block Slider did no damage (no Fusion Strike Energy attached).");
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Fast Charge: usable on your first turn if you go first; a Lightning Energy from your deck onto this Pokémon. Dynamic
        /// Spark: discard any amount of Lightning Energy from your Pokémon; 60 damage for each card discarded.
        /// </summary>
        private sealed class RaichuV : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name == "Fast Charge";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fast Charge")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    CardsMEG.ChargeSelfMon.ChargeSelf(ctx, EnergyType.Lightning, 1, "Fast Charge");
                    return true;
                }
                if (atk.Name != "Dynamic Spark") return false;
                var pool = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon()) pool.AddRange(mon.Energy.Where(e => CardsASR1.ProvidesTypeAttached(e.Def, EnergyType.Lightning)));
                int n = 0;
                if (pool.Count > 0)
                {
                    var picks = ctx.ChooseCards(pool, 0, pool.Count, "Dynamic Spark: discard any amount of Lightning Energy from your Pokémon (60 damage for each)");
                    foreach (var card in picks)
                    {
                        var holder = ctx.MyPokemon().FirstOrDefault(m => m.Energy.Contains(card));
                        if (holder != null && ctx.Engine.DiscardEnergy(holder, card)) n++;
                    }
                }
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Dynamic Spark did no damage.");
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Moonlit Miracle: flip 3 coins; choose up to that many of your Pokémon, and evolve each with a card from your deck that
        /// evolves from it. It ignores the timing limits on evolving (your ruling, v54).
        /// </summary>
        private sealed class MoonlitMiracleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moonlit Miracle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads == 0 || ctx.Me.Deck.Count == 0) return true;
                var mine = ctx.MyPokemon();
                var chosen = ctx.ChoosePokemonMany(mine, 0, Math.Min(heads, mine.Count), "Moonlit Miracle: choose up to " + heads + " of your Pokémon to evolve from your deck");
                foreach (var mon in chosen)
                    CardsSSP1.EvolveFromDeck(ctx, mon, "Moonlit Miracle: choose a card that evolves from " + mon.Name);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Coinciding Figures: 90 more damage if both players have the same number of Benched Pokémon.</summary>
        private sealed class CoincidingFiguresMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coinciding Figures") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Bench.Count == ctx.Opp.Bench.Count ? 90 : 0));
                return true;
            }
        }

        /// <summary>Special Transfer: as often as you like during your turn, move a Special Energy from 1 of your Pokémon to another.</summary>
        private sealed class DusknoirBRS : Mon
        {
            private const string Transfer = "Special Transfer";

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
            }

            private static bool CanMove(EffectContext ctx) { return ctx.MyPokemon().Count > 1 && Holders(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == Transfer && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Transfer + ": move a Special Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                var special = from.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                if (special.Count == 0) return false;
                var card = special.Select(e => e.Def.Name).Distinct().Count() == 1 ? special[0] : ctx.ChooseCards(special, 1, 1, Transfer + ": move which Special Energy?")[0];
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), Transfer + ": move it from " + from.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveEnergy(from, to, card);
            }
        }

        /// <summary>Fluff Gets in the Way: if the Defending Pokémon is a Basic Pokémon, it can't attack during your opponent's next turn.</summary>
        private sealed class FluffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fluff Gets in the Way") return false;
                var defending = ctx.Defending;
                bool basic = defending != null && defending.Def.IsBasicPokemon;
                ctx.DamageDefending(atk.Damage);
                if (!basic || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Fluff Gets in the Way)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Trick Wind: during your opponent's next turn, they can't play Pokémon Tool or Special Energy cards from their hand.</summary>
        private sealed class TrickWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick Wind") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new TrickWindEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Pokémon Tool or Special Energy cards from their hand during their next turn (Trick Wind)",
                    ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Fluffball Star (VSTAR Power): 60 damage to 1 of your opponent's Pokémon for each Energy attached to this Pokémon.</summary>
        private sealed class FluffballStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fluffball Star") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                if (n > 0) ctx.DamageAnyOpponent(60 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Fluffball Star did no damage.");
                return true;
            }
        }

        /// <summary>Tri Recharge: flip 3 coins; attach up to that many basic Energy from your discard pile to your Benched Pokémon in any way you like.</summary>
        private sealed class TriRechargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tri Recharge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads == 0 || ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, heads, (card, target) => ctx.Me.Bench.Contains(target), "Tri Recharge");
                return true;
            }
        }

        /// <summary>Dede-Flash: if your opponent has exactly 1 Prize card remaining, 60 more damage and their Active Pokémon is Confused.</summary>
        private sealed class DedeFlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dede-Flash") return false;
                var defending = ctx.Defending;
                bool lastPrize = ctx.Opp.Prizes.Count == 1;
                ctx.DamageDefending(atk.Damage + (lastPrize ? 60 : 0));
                if (lastPrize && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Dummy Doll: when you play this Pokémon from your hand onto your Bench during your turn, you may prevent all damage done
        /// to it by the opponent's attacks until the end of their next turn. Jealous Eyes: 3 damage counters on the opponent's
        /// Active Pokémon for each Prize card they have taken.
        /// </summary>
        private sealed class MimikyuV : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Dummy Doll", true, "Dummy Doll: prevent all damage done to this Mimikyu V by your opponent's attacks until the end of their next turn?")) return;
                ctx.AddEffect(new DummyDollEffect(ctx.Pokemon.Name + " takes no damage from the opponent's attacks until the end of their next turn (Dummy Doll)",
                    ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jealous Eyes") return false;
                int counters = 3 * ctx.OpponentPrizesTaken;
                if (counters > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, counters);
                else ctx.Log(ctx.Pokemon.Name + "'s Jealous Eyes did nothing.");
                return true;
            }
        }

        /// <summary>Ominous Numbers: 4 damage counters on the opponent's Pokémon in any way you like, or 13 if you played Acerola's Premonition this turn.</summary>
        private sealed class OminousNumbersMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ominous Numbers") return false;
                int counters = ctx.Me.SupporterNameThisTurn == "Acerola's Premonition" ? 13 : 4;
                ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentPokemon());
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>
        /// Roaring Resolve: once during your turn, put 2 damage counters on this Pokémon to attach a Fighting Energy from your
        /// deck to it. Aura Sphere Volley: discard all Fighting Energy from this Pokémon; 60 more damage for each card discarded.
        /// </summary>
        private sealed class LucarioBRS : Mon
        {
            protected override string Activated { get { return "Roaring Resolve"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.PlaceCounters(ctx.Pokemon, 2);
                CardsMEG.ChargeSelfMon.ChargeSelf(ctx, EnergyType.Fighting, 1, "Roaring Resolve");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aura Sphere Volley") return false;
                var me = ctx.Pokemon;
                var fighting = me.Energy.Where(e => CardsASR1.ProvidesTypeAttached(e.Def, EnergyType.Fighting)).ToList();
                int n = 0;
                foreach (var card in fighting)
                    if (ctx.Engine.DiscardEnergy(me, card)) n++;
                ctx.DamageDefending(atk.Damage + 60 * n);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Sludge Street: the Retreat Cost of your opponent's Poisoned Pokémon is [C] more. Shrieking Poison: Confused and Poisoned.</summary>
        private sealed class MukBRS : Mon
        {
            private static readonly Mon ShriekingPoison = new CardsCES1.ConditionsMon("Shrieking Poison", SpecialCondition.Confused, SpecialCondition.Poisoned);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon.Owner != src.Player && mon.Has(SpecialCondition.Poisoned) ? cost + 1 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ShriekingPoison.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Ransack: flip 2 coins. If either is heads, your opponent reveals their hand; for each heads, put a card from it on the
        /// bottom of their deck.
        /// </summary>
        private sealed class RansackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ransack") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand;
                int n = Math.Min(heads, hand.Count);
                if (n == 0) return true;
                var picks = ctx.ChooseCards(hand.ToList(), n, n, "Ransack: choose " + n + (n == 1 ? " card" : " cards") + " from your opponent's hand to put on the bottom of their deck");
                foreach (var card in picks)
                    if (hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + picks.Count + (picks.Count == 1 ? " card" : " cards") + " from their hand on the bottom of their deck.");
                return true;
            }
        }

        /// <summary>Boss Pockets: this Pokémon can have up to 4 Pokémon Tools (down to 1 if it loses the Ability). Fearsome Shadow: the opponent reveals their hand.</summary>
        private sealed class HonchkrowV : Mon
        {
            public override int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(max, 4) : max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fearsome Shadow") return false;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Ticking Terror: until the end of your next turn, the Defending Pokémon's Weakness is Darkness.</summary>
        private sealed class TickingTerrorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ticking Terror") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new Cards30C.WeaknessChangeEffect(defending.Name + "'s Weakness is Darkness until the end of your next turn (Ticking Terror)",
                    ctx.Player, defending.Uid, ctx.Turn + 2, EnergyType.Darkness));
                return true;
            }
        }

        /// <summary>Trade: discard a card from your hand (a cost) to draw 2 cards, once during your turn. Not with an empty deck (Compendium).</summary>
        private sealed class LiepardBRS : Mon
        {
            protected override string Activated { get { return "Trade"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Trade: discard a card from your hand to draw 2 cards");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(2);
            }
        }

        /// <summary>Longhair Shot: 30 damage to 1 of your opponent's Pokémon for each Darkness Energy attached to this Pokémon.</summary>
        private sealed class LonghairShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Longhair Shot") return false;
                int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Darkness);
                if (n > 0) ctx.DamageAnyOpponent(30 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Longhair Shot did no damage.");
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Rock Slide: also 30 damage to 2 of your opponent's Benched Pokémon.</summary>
        private sealed class RockSlideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Slide") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var targets = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, "Rock Slide: choose 2 of your opponent's Benched Pokémon (30 damage each)");
                foreach (var target in targets) ctx.DamageTarget(target, 30);
                return true;
            }
        }

        /// <summary>Miraculous Armor: takes 100 less damage from attacks from the opponent's Pokémon V. Pike: also 30 to 1 Benched Pokémon.</summary>
        private sealed class EscavalierBRS : Mon
        {
            private static readonly Mon Pike = new Cards30CC.BenchHitMon("Pike", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent || !info.Attacker.Def.IsPokemonV) return damage;
                return damage - 100;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pike.HandleAttack(ctx, atk); }
        }

        /// <summary>Gear Wall: your Basic Pokémon take 20 less damage from the opponent's attacks. Tumbling Attack: flip, heads 90 more.</summary>
        private sealed class KlinklangBRS : Mon
        {
            private static readonly Mon Tumbling = new FlipBonusMon("Tumbling Attack", 90);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target.Owner != src.Player || !info.AgainstOpponent || !info.Target.Def.IsBasicPokemon) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tumbling.HandleAttack(ctx, atk); }
        }

        // ================================================================== Dragon

        /// <summary>Draconic Impulse: if the opponent's Active Pokémon is a Pokémon VMAX, 160 more damage, and discard 3 Energy from this Pokémon.</summary>
        private sealed class DraconicImpulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draconic Impulse") return false;
                var defending = ctx.Defending;
                bool vmax = defending != null && defending.Def.IsVmax;
                ctx.DamageDefending(atk.Damage + (vmax ? 160 : 0));
                if (vmax) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 3, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Sonic Slip: when you play this Pokémon from your hand to evolve, you may prevent all damage from and effects of the
        /// opponent's attacks done to it until the end of their next turn. Dragonblade: discard the top 2 cards of your deck.
        /// </summary>
        private sealed class GarchompBRS : Mon
        {
            private static readonly Mon Dragonblade = new MillSelfMon("Dragonblade", 2);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Sonic Slip", true, "Sonic Slip: prevent all damage from and effects of your opponent's attacks done to Garchomp until the end of their next turn?")) return;
                ctx.AddEffect(new CardsPAR1.StanceEffect(ctx.Pokemon.Name + " is protected from the opponent's attacks until the end of their next turn (Sonic Slip)",
                    ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dragonblade.HandleAttack(ctx, atk); }
        }

        /// <summary>Ultra Evolution: flip a coin; if heads, search your deck for a Haxorus and put it onto this Axew to evolve it (your ruling: no timing limits).</summary>
        private sealed class UltraEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultra Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                var me = ctx.Pokemon;
                var found = ctx.SearchMyDeck(IsHaxorus, 1, "Ultra Evolution: search your deck for a Haxorus", false);
                if (found.Count > 0)
                {
                    if (ctx.InPlay(me) && me.Name == "Axew") ctx.Engine.EvolvePokemon(me, found[0]);
                    else ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }

            private static bool IsHaxorus(CardDef d) { return d.IsPokemon && d.Name == "Haxorus"; }
        }

        /// <summary>Slosh 'n' Crash: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon; if any were discarded, 120 more damage.</summary>
        private sealed class SloshNCrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slosh 'n' Crash") return false;
                var defending = ctx.Defending;
                int discarded = 0;
                if (defending != null && defending.Tools.Count > 0 && ctx.CanAffect(defending))
                    foreach (var tool in defending.Tools.ToList())
                        if (ctx.Engine.DiscardTool(defending, tool)) discarded++;
                ctx.DamageDefending(atk.Damage + (discarded > 0 ? 120 : 0));
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Hurricane: move a basic Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class HurricaneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hurricane") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return true;
                var card = basics.Select(e => e.Def.Name).Distinct().Count() == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Hurricane: move which basic Energy?")[0];
                var bench = ctx.MyBench();
                var to = ctx.ChoosePokemon(bench, "Hurricane: move " + card.Def.Name + " to which Benched Pokémon?");
                ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Strong Breeze: your opponent shuffles their Active Pokémon and all attached cards into their deck.</summary>
        internal sealed class StrongBreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong Breeze") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending != null && ctx.InPlay(defending) && ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        /// <summary>Trinity Charge: search your deck for up to 3 basic Energy cards and attach them to your Pokémon V in any way you like.</summary>
        private sealed class TrinityChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trinity Charge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                SearchAttachAnyWay(ctx, IsBasicEnergy, 3, IsPokemonV, "Trinity Charge", "basic Energy cards for your Pokémon V");
                return true;
            }

            private static bool IsBasicEnergy(CardDef d) { return d.IsBasicEnergy; }
            private static bool IsPokemonV(PokemonInPlay mon) { return mon.Def.IsPokemonV; }
        }

        /// <summary>Sudden Cyclone: when you play this Pokémon from your hand onto your Bench, you may have your opponent switch their Active Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class TornadusBRS : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool ready = ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0;
                if (!TriggerOnce(ctx, "Sudden Cyclone", ready, "Sudden Cyclone: have your opponent switch their Active Pokémon with 1 of their Benched Pokémon?")) return;
                ctx.OpponentSwitchesActive();
            }
        }

        /// <summary>Showboating Pose: attach up to 2 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class ShowboatingPoseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Showboating Pose") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                var energy = ctx.BasicEnergyInDiscard();
                if (bench.Count == 0 || energy.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Showboating Pose: attach basic Energy from your discard pile to which Benched Pokémon?");
                var picks = ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Showboating Pose: choose up to 2 basic Energy cards to attach to " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        // ================================================================== Full-art only

        /// <summary>Lightning Blast: you may discard all Lightning Energy from this Pokémon; if you do, 120 more damage.</summary>
        private sealed class LightningBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Blast") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(e => CardsASR1.ProvidesTypeAttached(e.Def, EnergyType.Lightning)).ToList();
                int bonus = 0;
                if (lightning.Count > 0 && ctx.AskYesNo("Lightning Blast: discard all Lightning Energy from this Pokémon for 120 more damage?"))
                {
                    int n = 0;
                    foreach (var card in lightning)
                        if (ctx.Engine.DiscardEnergy(me, card)) n++;
                    if (n > 0) bonus = 120;
                }
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }
    }
}
