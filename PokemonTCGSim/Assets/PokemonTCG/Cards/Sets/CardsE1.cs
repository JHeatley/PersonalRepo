using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Expedition Base Set Pokémon (E1 1-32, 36, 39, 46, 53, 64, 67, 71-136; v144). Unlimited only. 33-70 hold the non-holo prints
    /// of 1-32. Poké-Powers can't be used while their Pokémon is affected by any Special Condition (Burned and Poisoned too).
    /// Your rulings: Burning Energy makes every basic Energy on your Pokémon Fire for the rest of the turn, Energy attached later
    /// included; each Venusaur's Harvest Bounty gives 1 more attachment; Jet Stream's "Then" part happens on heads only, like your
    /// earlier rulings. Magby and Pichu are Baby Pokémon. Pokémon whose attacks only do their printed damage: Machoke 85, Pupitar
    /// 90, Goldeen 111, Houndour 113, Krabby 115, Larvitar 116, Poliwag 125.
    /// </summary>
    public static class CardsE1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E1-1", new AlakazamE1());
            r.RegisterByKey("E1-2", new AmpharosE1());
            r.RegisterByKey("E1-3", new ArbokE1());
            r.RegisterByKey("E1-4", new BlastoiseE1());
            r.RegisterByKey("E1-5", new ButterfreeE1());
            r.RegisterByKey("E1-6", new CharizardE1());
            r.RegisterByKey("E1-7", new ClefableE1());
            r.RegisterByKey("E1-8", new ComboMon(new ParalyzeFlipMon("Lick"), new CoinsMon("Auto Fire", 4)));
            r.RegisterByKey("E1-9", new DragoniteE1());
            r.RegisterByKey("E1-10", new MagnitudeMon());
            r.RegisterByKey("E1-11", new ComboMon(new ClutchMon(), new TailsNothingMon("Supersonic Flight")));
            r.RegisterByKey("E1-12", new FeraligatrE1());
            r.RegisterByKey("E1-13", new GengarE1());
            r.RegisterByKey("E1-14", new GolemE1());
            r.RegisterByKey("E1-15", new ComboMon(new ParalyzeFlipMon("Bubble"), new TailsNothingMon("Giant Claw")));
            r.RegisterByKey("E1-16", new MachampE1());
            r.RegisterByKey("E1-17", new EnergyCatchMon());
            r.RegisterByKey("E1-18", new MeganiumE1());
            r.RegisterByKey("E1-19", new SuperPsywaveMon());
            r.RegisterByKey("E1-20", new ComboMon(new CardsPAR2.FlipConditionMon("Hypnosis", SpecialCondition.Asleep), new CardsN2.PsychicPlusMon()));
            r.RegisterByKey("E1-21", new ComboMon(new MisleadMon(), new EtherealFlameMon()));
            r.RegisterByKey("E1-22", new EnergyPatchMon());
            r.RegisterByKey("E1-23", new PidgeotE1());
            r.RegisterByKey("E1-24", new PoliwrathE1());
            r.RegisterByKey("E1-25", new ComboMon(new PlasmaMon(), new ShockBoltMon()));
            r.RegisterByKey("E1-26", new OverrunMon());
            r.RegisterByKey("E1-27", new ComboMon(new FlipBonusMon("Steel Beak", 10), new TailsNothingMon("Air Cutter")));
            r.RegisterByKey("E1-28", new TyphlosionE1());
            r.RegisterByKey("E1-29", new TyranitarE1());
            r.RegisterByKey("E1-30", new VenusaurE1());
            r.RegisterByKey("E1-31", new VileplumeE1());
            r.RegisterByKey("E1-32", new ComboMon(new CardsSI.HeadsTailsConditionMon("Foul Gas", SpecialCondition.Poisoned, SpecialCondition.Confused), new MisfireMon()));
            r.RegisterByKey("E1-36", new ComboMon(new ParalyzeFlipMon("Bubble"), new CoinsMon("Double Cannon", 2)));
            r.RegisterByKey("E1-39", new CardsBS1.DiscardCostMon("Flamethrower", EnergyType.Fire, 1, null, new TailsNothingMon("Tail Smash")));
            r.RegisterByKey("E1-46", new DoubleClawMon());
            r.RegisterByKey("E1-53", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("E1-64", new ComboMon(new FlipBonusMon("Quick Attack", 20), new ThermalBlastMon()));
            r.RegisterByKey("E1-67", new ComboMon(new LeechSeedMon(), new CoinsMon("Fury Strikes", 3)));
            r.RegisterByKey("E1-71", new CardsPAR2.FlipConditionMon("Mysterious Powder", SpecialCondition.Confused));
            r.RegisterByKey("E1-72", new ComboMon(new BindWoundMon(), new DogpileMon()));
            r.RegisterByKey("E1-73", new CardsBS1.DiscardCostMon("Flamethrower", EnergyType.Fire, 1, null, new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("E1-74", new RecoilMon("Take Down", 10));
            r.RegisterByKey("E1-75", new SpiralWaveMon());
            r.RegisterByKey("E1-76", new ComboMon(new EnergyDrawMon(), new ReflectEnergyMon()));
            r.RegisterByKey("E1-77", new CardsFST1.FlipRecoilMon("Thunder Jolt", 20));
            r.RegisterByKey("E1-78", new FoulOdorMon());
            r.RegisterByKey("E1-79", new ComboMon(new NoResistanceMon("Rock Hurl"), new RockSlideMon()));
            r.RegisterByKey("E1-80", new ComboMon(new ConditionAttackMon("Nightmare", SpecialCondition.Asleep), new DreamEaterMon()));
            r.RegisterByKey("E1-81", new StretchKickMon());
            r.RegisterByKey("E1-82", new ConditionAttackMon("Sleep Seed", SpecialCondition.Asleep));
            r.RegisterByKey("E1-83", new ComboMon(new ParalyzeFlipMon("Ice Punch"), new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep)));
            r.RegisterByKey("E1-84", new ComboMon(new EnergyRecallMon(), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("E1-85", new PlainMon());
            r.RegisterByKey("E1-86", new ComboMon(new CardsPAR2.FlipConditionMon("Flaming Punch", SpecialCondition.Burned), new CardsBS1.ThrashMon("Thrash")));
            r.RegisterByKey("E1-87", new ExoskeletonMon());
            r.RegisterByKey("E1-88", new WhirlwindMon());
            r.RegisterByKey("E1-89", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("E1-90", new PlainMon());
            r.RegisterByKey("E1-91", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("E1-92", new ComboMon(new CoinsMon("Doubleslap", 2), new LockAfterAttackMon("Giant Wave", false)));
            r.RegisterByKey("E1-93", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("E1-94", new CardsPAR2.FlipConditionMon("Poison Seed", SpecialCondition.Poisoned));
            r.RegisterByKey("E1-95", new ConditionAttackMon("Sleep Seed", SpecialCondition.Asleep));
            r.RegisterByKey("E1-96", new ParalyzeFlipMon("Entangling Thread"));
            r.RegisterByKey("E1-97", new CoinsMon("Tail Rap", 2));
            r.RegisterByKey("E1-98", new CardsPAR2.FlipConditionMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("E1-99", new ComboMon(new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep), new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("E1-100", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("E1-101", new ConditionAttackMon("Shining Fingers", SpecialCondition.Asleep));
            r.RegisterByKey("E1-102", new CoinsMon("Spike Cannon", 3));
            r.RegisterByKey("E1-103", new TailsNothingMon("Bone Smash"));
            r.RegisterByKey("E1-104", new ComboMon(new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("E1-105", new CardsBS1.DiscardCostMon("Ember", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("E1-106", new CoinsMon("Fury Headbutt", 3));
            r.RegisterByKey("E1-107", new TailsNothingMon("Dragon Smash"));
            r.RegisterByKey("E1-108", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("E1-109", new CardsSI.HeadsTailsConditionMon("Bad Dream", SpecialCondition.Asleep, SpecialCondition.Confused));
            r.RegisterByKey("E1-110", new NoResistanceMon("Rock Hurl"));
            r.RegisterByKey("E1-111", new PlainMon());
            r.RegisterByKey("E1-112", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("E1-113", new PlainMon());
            r.RegisterByKey("E1-114", new CardsPAR2.FlipConditionMon("Confusion Gas", SpecialCondition.Confused));
            r.RegisterByKey("E1-115", new PlainMon());
            r.RegisterByKey("E1-116", new PlainMon());
            r.RegisterByKey("E1-117", new CoinsMon("Double Chop", 2));
            r.RegisterByKey("E1-118", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("E1-119", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("E1-120", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("E1-121", new ComboMon(new CoinsMon("Double Scratch", 2), new PayDayMon()));
            r.RegisterByKey("E1-122", new ConditionAttackMon("Sleep Seed", SpecialCondition.Asleep));
            r.RegisterByKey("E1-123", new CoinsMon("Fury Strikes", 3));
            r.RegisterByKey("E1-124", new ParalyzeFlipMon("Lightning Tail"));
            r.RegisterByKey("E1-125", new PlainMon());
            r.RegisterByKey("E1-126", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("E1-127", new ParalyzeFlipMon("Stun Needle"));
            r.RegisterByKey("E1-128", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("E1-129", new ParalyzeFlipMon("Lick"));
            r.RegisterByKey("E1-130", new TailsNothingMon("Razor Wind"));
            r.RegisterByKey("E1-131", new CoinsMon("Doubleslap", 2));
            r.RegisterByKey("E1-132", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("E1-133", new ComboMon(new TailsNothingMon("Horn Hazard"), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("E1-134", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("E1-135", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("E1-136", new ConditionAttackMon("Tail Shake", SpecialCondition.Asleep));
        }

        // ================================================================== Shared

        /// <summary>e-card Poké-Powers: "This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        internal static bool NoCondition(PokemonInPlay mon) { return mon != null && mon.Conditions == SpecialCondition.None; }

        private static bool Provides(EffectContext ctx, PokemonInPlay mon, CardInstance card, EnergyType type)
        {
            return CardsDRI1.ProvidesType(ctx, mon, card, type);
        }

        private static List<CardInstance> EnergyCardsOf(EffectContext ctx, PokemonInPlay mon, EnergyType type)
        {
            return mon == null ? new List<CardInstance>() : mon.Energy.Where(e => Provides(ctx, mon, e, type)).ToList();
        }

        private static bool IsBasicEnergyOf(CardDef d, EnergyType type) { return d != null && d.IsBasicEnergy && d.EnergyProvides == type; }

        /// <summary>Look at the top N cards of your deck and put them back in any order.</summary>
        internal static void ReorderMyTop(EffectContext ctx, int count, string source)
        {
            var deck = ctx.Me.Deck;
            var top = deck.Take(count).ToList();
            if (top.Count == 0) return;
            if (top.Count == 1)
            {
                ctx.Engine.ShowCards(ctx.Player, top, source, "The top card of your deck.", ctx.Source);
                return;
            }
            foreach (var c in top) deck.Remove(c);
            var ordered = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = ctx.ChooseCards(left, 1, 1, source + ": choose the card to put " + (ordered.Count == 0 ? "on top of your deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                ordered.Add(pick);
                left.Remove(pick);
            }
            ordered.Add(left[0]);
            deck.InsertRange(0, ordered);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the top " + ordered.Count + " cards of their deck back in any order (" + source + ").");
        }

        /// <summary>"Don't apply Resistance" (Rock Hurl, Rock Tumble).</summary>
        private sealed class NoResistanceMon : Mon
        {
            private readonly string attack;

            public NoResistanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                return true;
            }
        }

        // ================================================================== Holo Pokémon

        /// <summary>
        /// Psymimic (Poké-Power): instead of its own attack, Alakazam may use any attack of the opponent's Pokémon in play, paying
        /// its cost (not while affected by a Special Condition). Syncroblast: 80, or 20 if the Energy card counts differ.
        /// </summary>
        private sealed class AlakazamE1 : Mon
        {
            public override bool ExtraAttacksMayShareName { get { return true; } }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (!NoCondition(src.Pokemon) || !src.IsMyTurn) return list;
                var seen = new HashSet<string>();
                foreach (var other in src.OpponentPokemon())
                {
                    var defs = new List<CardDef> { other.Def };
                    if ((other.Def.IsLevelUp || other.Def.IsBreak) && other.Stack.Count >= 2) defs.Add(other.Stack[other.Stack.Count - 2].Def);
                    foreach (var def in defs)
                    {
                        if (!seen.Add(def.BaseKey)) continue;
                        for (int i = 0; i < def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(def, i));
                    }
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Syncroblast") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool same = me != null && defending != null && me.Energy.Count == defending.Energy.Count;
                ctx.DamageDefending(same ? atk.Damage : 20);
                return true;
            }
        }

        /// <summary>
        /// Energy Connect (Poké-Power): as often as you like during your turn, move a basic Energy card from 1 of your Benched
        /// Pokémon to your Active Pokémon. Lightning Strike: you may discard all Lightning Energy cards for 80 instead of 40.
        /// </summary>
        private sealed class AmpharosE1 : Mon
        {
            private const string Power = "Energy Connect";

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyBench().Where(m => m.Energy.Any(e => e.Def.IsBasicEnergy)).ToList(); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && NoCondition(ctx.Pokemon) && ctx.Me.Active != null && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var donors = Donors(ctx);
                var active = ctx.Me.Active;
                if (donors.Count == 0 || active == null) return;
                var from = donors.Count == 1 ? donors[0] : ctx.ChoosePokemon(donors, Power + ": move a basic Energy card from which Benched Pokémon?");
                if (from == null) return;
                var cards = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, Power + ": move which Energy card to " + active.Name + "?")[0];
                ctx.Engine.MoveEnergy(from, active, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Strike") return false;
                var me = ctx.Pokemon;
                var lightning = EnergyCardsOf(ctx, me, EnergyType.Lightning);
                bool big = false;
                if (lightning.Count > 0 && ctx.AskYesNo("Lightning Strike: discard all Lightning Energy cards from " + me.Name + " for 80 damage instead of 40?"))
                {
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                    big = true;
                }
                ctx.DamageDefending(big ? 80 : atk.Damage);
                return true;
            }
        }

        /// <summary>Poison Spray: the Defending Pokémon is now Poisoned. Poison Reaction: 40 against a Poisoned Defending Pokémon, otherwise 20.</summary>
        private sealed class ArbokE1 : Mon
        {
            private static readonly Mon Spray = new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Poison Reaction")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage + (defending != null && defending.Has(SpecialCondition.Poisoned) ? 20 : 0));
                    return true;
                }
                return Spray.HandleAttack(ctx, atk);
            }
        }

        /// <summary>
        /// Jet Stream (Poké-Power): once during your turn, while Active, flip; heads, discard an Energy card from Blastoise, then an
        /// Energy card from the Defending Pokémon (both on heads only). Energy Cannon: 40 + 10 for each Energy not used for its cost (20 more at most).
        /// </summary>
        private sealed class BlastoiseE1 : Mon
        {
            private const string Power = "Jet Stream";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var me = ctx.Pokemon;
                if (me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, Power + ": discard which Energy card from " + me.Name + "?");
                var defending = ctx.Defending;
                if (defending != null && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, Power + ": discard which Energy card from " + defending.Name + "?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Cannon") return false;
                int extra = ctx.Pokemon == null ? 0 : Math.Max(0, Math.Min(2, ctx.EnergyCount(ctx.Pokemon) - atk.Cost.Length));
                ctx.DamageDefending(atk.Damage + 10 * extra);
                return true;
            }
        }

        /// <summary>Miraculous Powder (Poké-Power): once during your turn, your Active Pokémon recovers from all Special Conditions. Spiral Drain: heads, 2 counters off itself.</summary>
        private sealed class ButterfreeE1 : Mon
        {
            protected override string Activated { get { return "Miraculous Powder"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.Me.Active != null && ctx.Me.Active.Conditions != SpecialCondition.None;
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                foreach (SpecialCondition c in new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned, SpecialCondition.Burned })
                    if (active.Has(c)) ctx.Engine.RemoveCondition(active, c);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Drain") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Flip() && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                return true;
            }
        }

        /// <summary>
        /// Burning Energy (Poké-Power): once during your turn, every basic Energy attached to your Pokémon is Fire Energy for the
        /// rest of the turn (your ruling: Energy attached later too). Scorching Whirlwind: 2 coins; 1 tails, discard 2 Energy cards;
        /// 2 tails, discard all of them.
        /// </summary>
        private sealed class CharizardE1 : Mon
        {
            protected override string Activated { get { return "Burning Energy"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AddEffect(new BurningEnergyEffect(ctx.Engine.Possessive(ctx.Player) + " basic Energy is Fire Energy this turn (Burning Energy)", ctx.Player, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Whirlwind") return false;
                int tails = 2 - ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (tails == 0 || !ctx.InPlay(me) || CardsBS1.CopyingWithMetronome) return true;
                if (tails == 2) ctx.Engine.DiscardAllEnergy(me);
                else ctx.DiscardEnergyFrom(me, 2, "Scorching Whirlwind: discard 2 Energy cards from " + me.Name);
                return true;
            }
        }

        private sealed class BurningEnergyEffect : ContinuousEffect
        {
            public BurningEnergyEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override void ModifyProvidedEnergy(GameEngine engine, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (energy == null || holder == null || holder.Owner != Owner || !energy.Def.IsBasicEnergy) return;
                for (int i = 0; i < units.Count; i++) if (units[i] != 0) units[i] = EnergyMask.Of(EnergyType.Fire);
            }
        }

        /// <summary>
        /// Moonlight (Poké-Power): once during your turn, put a card from your hand on top of your deck; then search your deck for a
        /// basic Energy card and put it into your hand. Doubleslap: 2 coins, 20 per heads.
        /// </summary>
        private sealed class ClefableE1 : Mon
        {
            private const string Power = "Moonlight";
            private static readonly Mon Doubleslap = new CoinsMon("Doubleslap", 2);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, Power + ": put which card from your hand on top of your deck?");
                if (pick.Count == 0 || !ctx.Me.Hand.Remove(pick[0])) return;
                ctx.Me.Deck.Insert(0, pick[0]);
                ctx.SearchToHand(d => d.IsBasicEnergy, 1, Power + ": choose a basic Energy card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Doubleslap.HandleAttack(ctx, atk); }
        }

        /// <summary>Tailwind (Poké-Power): once during your turn, while this Pokémon is Benched, your Active Pokémon's Retreat Cost is 0 this turn. Dragon Tail: 2 coins, 40 per heads.</summary>
        private sealed class DragoniteE1 : Mon
        {
            private static readonly Mon DragonTail = new CoinsMon("Dragon Tail", 2);

            protected override string Activated { get { return "Tailwind"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active != null) ctx.AddEffect(new NoRetreatCostEffect(active.Name + " has no Retreat Cost this turn (Tailwind)", ctx.Player, active.Uid, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DragonTail.HandleAttack(ctx, atk); }
        }

        private sealed class NoRetreatCostEffect : ContinuousEffect
        {
            public NoRetreatCostEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost) { return mon != null && mon.Uid == TargetUid ? 0 : cost; }
        }

        /// <summary>Magnitude: 10 damage to each Benched Pokémon, both sides (no Weakness or Resistance).</summary>
        private sealed class MagnitudeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnitude") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var benched in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>Clutch: the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class ClutchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clutch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>
        /// Major Tsunami (Poké-Power): once during your turn, while Active, the opponent switches their Active Pokémon with 1 of their
        /// Benched Pokémon (if any); either way, you switch Feraligatr with 1 of your Benched Pokémon (if any). Rending Jaws: 70, or
        /// 40 against a Defending Pokémon with no damage counters.
        /// </summary>
        private sealed class FeraligatrE1 : Mon
        {
            protected override string Activated { get { return "Major Tsunami"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && NoCondition(ctx.Pokemon) && (ctx.Opp.Bench.Count > 0 || ctx.Me.Bench.Count > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                if (ctx.Me.Bench.Count > 0 && ctx.PokemonIsActive) ctx.SwitchSelfWithBench();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rending Jaws") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(defending != null && defending.DamageCounters > 0 ? atk.Damage : 40);
                return true;
            }
        }

        /// <summary>
        /// Chaos Move (Poké-Power): once during your turn, if the opponent has 3 or fewer Prize cards left, move 1 damage counter
        /// from any Pokémon to another (even if it Knocks it Out). Hide in Shadows: then switch with 1 of your Benched Pokémon.
        /// </summary>
        private sealed class GengarE1 : Mon
        {
            private const string Power = "Chaos Move";

            protected override string Activated { get { return Power; } }

            private static List<PokemonInPlay> All(EffectContext ctx) { return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList(); }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var all = All(ctx);
                return NoCondition(ctx.Pokemon) && ctx.Opp.Prizes.Count <= 3 && all.Count >= 2 && all.Any(m => m.DamageCounters > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                var from = All(ctx).Where(m => m.DamageCounters > 0).ToList();
                if (from.Count == 0) return;
                var source = from.Count == 1 ? from[0] : ctx.ChoosePokemon(from, Power + ": move a damage counter from which Pokémon?");
                if (source == null) return;
                var to = All(ctx).Where(m => m != source).ToList();
                if (to.Count == 0) return;
                var target = to.Count == 1 ? to[0] : ctx.ChoosePokemon(to, Power + ": move it to which Pokémon?");
                if (target != null) ctx.Engine.MoveDamageCounters(source, target, 1, Power);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hide in Shadows") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Rock Body (Poké-Body): damage done by attacks to Golem is 10 less (after W/R). Rock Tumble: no Resistance.</summary>
        private sealed class GolemE1 : Mon
        {
            private static readonly Mon Tumble = new NoResistanceMon("Rock Tumble");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Math.Max(0, damage - 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tumble.HandleAttack(ctx, atk); }
        }

        /// <summary>Exoskeleton (Poké-Body): damage done by attacks to Metapod is 20 less (after W/R).</summary>
        private sealed class ExoskeletonMon : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>
        /// Terraforming (Poké-Power): once during your turn, look at the top 4 cards of your deck and put them back in any order.
        /// Iron Fist: count your Pokémon with damage counters; heads, 50 + 10 for each.
        /// </summary>
        private sealed class MachampE1 : Mon
        {
            protected override string Activated { get { return "Terraforming"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ReorderMyTop(ctx, 4, "Terraforming"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Fist") return false;
                int hurt = ctx.MyPokemon(m => m.DamageCounters > 0).Count;
                ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 10 * hurt : 0));
                return true;
            }
        }

        /// <summary>Energy Catch: heads, a basic Energy card from your discard pile into your hand.</summary>
        private sealed class EnergyCatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Catch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                ctx.DiscardToHand(energy.Count == 1 ? energy : ctx.ChooseCards(energy, 1, 1, "Energy Catch: put which basic Energy card into your hand?"));
                return true;
            }
        }

        /// <summary>Soothing Aroma (Poké-Power): once during your turn, flip; heads, 1 damage counter off each of your Pokémon. Poisonpowder.</summary>
        private sealed class MeganiumE1 : Mon
        {
            private static readonly Mon Powder = new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned);

            protected override string Activated { get { return "Soothing Aroma"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Powder.HandleAttack(ctx, atk); }
        }

        /// <summary>Super Psywave: damage counters on 1 of the opponent's Pokémon equal to the number of Energy cards attached to it.</summary>
        private sealed class SuperPsywaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Psywave") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = all.Count == 1 ? all[0] : ctx.ChoosePokemon(all, "Super Psywave: choose 1 of your opponent's Pokémon");
                if (target != null && target.Energy.Count > 0) ctx.PlaceCounters(target, target.Energy.Count);
                return true;
            }
        }

        /// <summary>Mislead: 2 coins; if either is heads, Confused.</summary>
        private sealed class MisleadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mislead") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Ethereal Flame: discard all Fire Energy cards attached; 30 + 20 for each.</summary>
        private sealed class EtherealFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ethereal Flame") return false;
                var me = ctx.Pokemon;
                int n = 0;
                if (!CardsBS1.CopyingWithMetronome)
                    foreach (var card in EnergyCardsOf(ctx, me, EnergyType.Fire))
                        if (ctx.Engine.DiscardEnergy(me, card)) n++;
                ctx.DamageDefending(30 + 20 * n);
                return true;
            }
        }

        /// <summary>Energy Patch: move a basic Energy card from 1 of your Pokémon to another.</summary>
        internal sealed class EnergyPatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Patch") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var donors = ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy));
                if (donors.Count == 0 || ctx.MyPokemon().Count < 2) return true;
                var from = donors.Count == 1 ? donors[0] : ctx.ChoosePokemon(donors, "Energy Patch: move a basic Energy card from which Pokémon?");
                if (from == null) return true;
                var cards = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Energy Patch: move which Energy card?")[0];
                var to = ctx.MyPokemon(m => m != from);
                var target = to.Count == 1 ? to[0] : ctx.ChoosePokemon(to, "Energy Patch: attach " + card.Def.Name + " to which Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(from, target, card);
                return true;
            }
        }

        /// <summary>Beating Wings (Poké-Power): once during your turn, while Active, shuffle 1 of your Benched Pokémon and its cards into your deck. Sharp Beak: heads, 30 more.</summary>
        private sealed class PidgeotE1 : Mon
        {
            private static readonly Mon SharpBeak = new FlipBonusMon("Sharp Beak", 30);

            protected override string Activated { get { return "Beating Wings"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && NoCondition(ctx.Pokemon) && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var bench = ctx.MyBench();
                if (bench.Count == 0) return;
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Beating Wings: shuffle which Benched Pokémon into your deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SharpBeak.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Plunge (Poké-Power): once during your turn, while this Pokémon is Benched, flip; heads, move every Energy card from your
        /// Active Pokémon to it, then switch it with your Active Pokémon. Water Punch: a coin for each Water Energy, 40 + 10 per heads.
        /// </summary>
        private sealed class PoliwrathE1 : Mon
        {
            protected override string Activated { get { return "Plunge"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var me = ctx.Pokemon;
                var active = ctx.Me.Active;
                if (active == null || !ctx.Me.Bench.Contains(me)) return;
                foreach (var card in active.Energy.ToList()) ctx.Engine.MoveEnergy(active, me, card);
                if (ctx.Me.Bench.Contains(me)) ctx.Engine.SwitchActive(ctx.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Punch") return false;
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>Plasma: with Lightning Energy cards in your discard pile, flip; heads, attach 1 of them to this Pokémon.</summary>
        internal sealed class PlasmaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plasma") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.Me.Discard.Where(c => IsBasicEnergyOf(c.Def, EnergyType.Lightning)).ToList();
                if (cards.Count == 0 || !ctx.InPlay(me) || !ctx.Flip()) return true;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Plasma: attach which Lightning Energy card?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Shock Bolt: discard all Lightning Energy cards attached to this Pokémon or the attack does nothing.</summary>
        private sealed class ShockBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shock Bolt") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var lightning = EnergyCardsOf(ctx, me, EnergyType.Lightning);
                    if (lightning.Count == 0) return Nothing(ctx);
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Overrun: with Benched Pokémon on the other side, flip; heads, 10 damage to 1 of them.</summary>
        internal sealed class OverrunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overrun") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Flip()) ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>
        /// Heat Up (Poké-Power): once during your turn, if the opponent's Pokémon have more Energy cards attached than yours, you may
        /// attach a Fire Energy card from your deck to 1 of your Benched Pokémon. Super Singe: heads, Burned.
        /// </summary>
        private sealed class TyphlosionE1 : Mon
        {
            private const string Power = "Heat Up";
            private static readonly Mon Singe = new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned);

            protected override string Activated { get { return Power; } }

            private static int EnergyCards(IEnumerable<PokemonInPlay> mons) { return mons.Sum(m => m.Energy.Count); }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return NoCondition(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.Me.Deck.Count > 0 && EnergyCards(ctx.OpponentPokemon()) > EnergyCards(ctx.MyPokemon());
            }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => IsBasicEnergyOf(d, EnergyType.Fire), 1, Power + ": choose a Fire Energy card");
                foreach (var card in found)
                {
                    var bench = ctx.MyBench();
                    var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, Power + ": attach it to which Benched Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Singe.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Aura (Poké-Body): every Energy attached to Tyranitar is Darkness Energy. Stamp: heads, 60 and 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class TyranitarE1 : Mon
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon) return;
                for (int i = 0; i < units.Count; i++) if (units[i] != 0) units[i] = EnergyMask.Of(EnergyType.Darkness);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stamp") return false;
                if (!ctx.Flip())
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                ctx.DamageDefending(atk.Damage + 10);
                foreach (var benched in ctx.OpponentBench())
                    if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>
        /// Harvest Bounty (Poké-Power): once during your turn, when you attach an Energy card from your hand to your Active Pokémon for
        /// the turn, you may attach another one to it (your ruling: each Venusaur gives 1 more). Body Slam: heads, Paralyzed.
        /// </summary>
        private sealed class VenusaurE1 : Mon
        {
            private const string Power = "Harvest Bounty";
            private static readonly Mon BodySlam = new ParalyzeFlipMon("Body Slam");

            public override bool OffersExtraEnergyAttachment(EffectContext src, PokemonInPlay target)
            {
                return src.IsMyTurn && target != null && target == src.Me.Active && NoCondition(src.Pokemon) && !src.AbilityUsedThisTurn(Power);
            }

            public override void OnExtraEnergyAttachmentUsed(EffectContext src)
            {
                src.MarkAbilityUsed(Power);
                Announce(src, Power);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BodySlam.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Pollen (Poké-Power): once during your turn, flip; heads, the Defending Pokémon is Poisoned. Petal Dance: 3 coins, 30 per heads, then Confused.</summary>
        private sealed class VileplumeE1 : Mon
        {
            protected override string Activated { get { return "Poison Pollen"; } }

            protected override bool AbilityReady(EffectContext ctx) { return NoCondition(ctx.Pokemon) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (ctx.Flip() && defending != null) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Petal Dance") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Misfire: tails, 6 damage counters on this Pokémon.</summary>
        private sealed class MisfireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Misfire") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, 6);
                return true;
            }
        }

        // ================================================================== Rare, uncommon and common Pokémon

        /// <summary>Double Claw: 2 coins, 30 + 20 per heads.</summary>
        internal sealed class DoubleClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Claw") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.FlipCoins(2));
                return true;
            }
        }

        /// <summary>Thermal Blast: heads, 10 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class ThermalBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thermal Blast") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                    foreach (var benched in ctx.OpponentBench())
                        if (ctx.InPlay(benched)) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        /// <summary>Leech Seed: if it damages the Defending Pokémon, 1 damage counter off this Pokémon.</summary>
        private sealed class LeechSeedMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leech Seed") return false;
                int done = ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (done > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Bind Wound: heads, 2 damage counters off 1 of your Pokémon.</summary>
        private sealed class BindWoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bind Wound") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0 || !ctx.Flip()) return true;
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Bind Wound: remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Dogpile: 10 for each of your Benched Pokémon, to the Defending Pokémon and to Chansey itself.</summary>
        private sealed class DogpileMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dogpile") return false;
                int n = ctx.Me.Bench.Count;
                ctx.DamageDefending(atk.Damage * n);
                if (n > 0 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Spiral Wave: flip until tails, 20 per heads.</summary>
        private sealed class SpiralWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Wave") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>Energy Draw: attach a basic Energy card from your deck to this Pokémon.</summary>
        internal sealed class EnergyDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Energy Draw: choose a basic Energy card to attach to " + me.Name);
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Reflect Energy: move a basic Energy card from this Pokémon to 1 of your Benched Pokémon.</summary>
        internal sealed class ReflectEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflect Energy") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var cards = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (cards.Count == 0) return true;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Reflect Energy: move which basic Energy card?")[0];
                var bench = ctx.MyBench();
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Reflect Energy: attach " + card.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Foul Odor: both the Defending Pokémon and this Pokémon are now Confused (after the damage).</summary>
        private sealed class FoulOdorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foul Odor") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Rock Slide: 10 damage to 2 of the opponent's Benched Pokémon (1 if only 1).</summary>
        private sealed class RockSlideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Slide") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                int n = Math.Min(2, bench.Count);
                var picks = bench.Count == n ? bench : ctx.ChoosePokemonMany(bench, n, n, "Rock Slide: choose 2 of your opponent's Benched Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Dream Eater: does nothing unless the Defending Pokémon is Asleep.</summary>
        private sealed class DreamEaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream Eater") return false;
                var defending = ctx.Defending;
                if (defending == null || !defending.Has(SpecialCondition.Asleep)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Stretch Kick: 30 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class StretchKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stretch Kick") return false;
                if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(30);
                return true;
            }
        }

        /// <summary>Energy Recall: attach 2 basic Energy cards from your discard pile to this Pokémon (1 if only 1).</summary>
        private sealed class EnergyRecallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Recall") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || !ctx.InPlay(me)) return true;
                int n = Math.Min(2, energy.Count);
                var picks = energy.Count == n ? energy : ctx.ChooseCards(energy, n, n, "Energy Recall: attach which 2 basic Energy cards to " + me.Name + "?");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Whirlwind: after the damage, the opponent switches the Defending Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class WhirlwindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whirlwind") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Pay Day: heads, draw a card.</summary>
        private sealed class PayDayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pay Day") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.Draw(1);
                return true;
            }
        }
    }
}
