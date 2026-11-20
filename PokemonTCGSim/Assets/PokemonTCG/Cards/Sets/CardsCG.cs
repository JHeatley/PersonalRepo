using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crystal Guardians (CG, 2006; v171): the cards not already in the pool (the others are prints). Unlimited only. 1-on-1:
    /// "1 of the Defending Pokémon" is the Defending Pokémon. Pokémon here (1-70); Pokémon-ex, Pokémon ★ and Trainers in
    /// CardsCGEx.cs. Compendium rulings used: Chlorophyll turns Boost Energy into 3 Grass Energy; Cursed Glare stops any Special
    /// Energy card but Darkness and Metal Energy; Even Game can't search with a full Bench; Mining's "Trainer" includes Supporters
    /// and Stadiums; Dark Eyes adds up per Shiftry ex; Derail's discard comes after the damage; Cessation Crystal stops old
    /// Pokémon Powers too; Crystal Beach only changes the Energy provided; Holon Circle ends an attack as soon as it's announced;
    /// Windstorm must discard at least 1 card.
    /// </summary>
    public static partial class CardsCG
    {
        public static void Register(CardScriptRegistry r)
        {
            // Rares (1-15)
            r.RegisterByKey("CG-1", new CardsSS.SafeguardExMon(new IfDefendingConditionMon("Night Murmurs", SpecialCondition.Confused, m => m.IsBasic)));
            r.RegisterByKey("CG-2", new BlastoiseShieldVeil());
            r.RegisterByKey("CG-3", new CameruptDeltaProtection());
            r.RegisterByKey("CG-4", new CharizardPealOfThunder());
            r.RegisterByKey("CG-5", new DugtrioSandVeil());
            r.RegisterByKey("CG-6", new OverzealousMon(new CardsHPB.KnockOffMon("Knock Off", false)));
            r.RegisterByKey("CG-7", new ComboMon(new EvenGameMon(), new StadiumPlayMon()));
            r.RegisterByKey("CG-8", new ComboMon(new CardsE3B.OpponentSwitchesMon("Kick Away"), new StrongCurrentMon()));
            r.RegisterByKey("CG-9", new ComboMon(new MiningMon(), CardsSS.ConditionBonus("Bite Off", 30, d => d.Def.IsOldEx)));
            r.RegisterByKey("CG-10", new SableyeExcavate());
            r.RegisterByKey("CG-11", new ComboMon(new PickAndChooseMon(), new CardsASR1.PlusMon("Reactive Poison", 20, c => ConditionCount(c.Defending))));
            r.RegisterByKey("CG-12", new TaurosCrushChance());
            r.RegisterByKey("CG-13", new WigglytuffFluffyFur());
            r.RegisterByKey("CG-14", new CardsEM.LowHpBonusMon(new HydroPumpMon()));
            r.RegisterByKey("CG-15", new CacturneSpikeStorm());

            // Uncommons (16-43)
            r.RegisterByKey("CG-16", new FocusEnergyMon());
            r.RegisterByKey("CG-17", new DusclopsCursedGlare());
            r.RegisterByKey("CG-18", new FearowDeltaSign());
            r.RegisterByKey("CG-19", new DodgeMon("Agility", false));
            r.RegisterByKey("CG-20", new ThickFatMon(new CardsASR1.TimesMon("Circular Steps", c => c.MyPokemon().Count + c.OpponentPokemon().Count - 1)));
            r.RegisterByKey("CG-21", new IgglybuffHoverLift());
            r.RegisterByKey("CG-22", new ComboMon(new ParalyzeFlipMon("Body Slam"),
                                                  new CardsFST2.ConditionBonusMon("Prop-up Pinchers", 40, c => c.Pokemon != null && c.Pokemon.Tools.Count > 0)));
            r.RegisterByKey("CG-23", new ComboMon(new CardsSS.SurpriseMon(), new CardsSSB.AnyOpponentTextMon("Bass Control", 40)));
            r.RegisterByKey("CG-24", new Cards30CC.BenchHitMon("Mud Splash", 30));
            r.RegisterByKey("CG-25", new MedichamDualArmor());
            r.RegisterByKey("CG-26", new PelipperDeltaTransport());
            r.RegisterByKey("CG-27", new SwampertEchoDraw());
            r.RegisterByKey("CG-28", new VenusaurChlorophyll());
            r.RegisterByKey("CG-29", new ComboMon(new Cards30C.SelfCounterBonusMon("Rage", 10), new CardsASC.SelfDiscardTypeMon("Flamethrower", EnergyType.Fire, 1)));
            r.RegisterByKey("CG-30", new CardsFST1.FlipRecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("CG-31", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new FlipBonusMon("Firebreathing", 20)));
            r.RegisterByKey("CG-32", new DodgeMon("Detect", false));
            r.RegisterByKey("CG-33", new CardsTEU1.AmnesiaMon("Amnesia"));
            r.RegisterByKey("CG-34", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("CG-35", new ComboMon(new CardsMEG.SnipeBenchMon("Stretch Vine", 30), new FlipBonusMon("Sharp Leaf", 20)));
            r.RegisterByKey("CG-36", new ComboMon(new CardsE3B.OpponentSwitchesMon("Roar"), new CardsFST2.SelfCountersMon("Metal Charge", 1)));
            r.RegisterByKey("CG-37", new CardsUF.PlunderMon());
            r.RegisterByKey("CG-38", new NoRetreatMon("Hug"));
            r.RegisterByKey("CG-39", new CardsCES1.AttachFromHandToSelfMon("Self Charge", EnergyType.Darkness, 1));
            r.RegisterByKey("CG-40", new CardsSIT1.AscensionMon());
            r.RegisterByKey("CG-41", new CardsCES1.FeintAttackMon("Feint Attack", 20));
            r.RegisterByKey("CG-42", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("CG-43", new CardsLOR1.EntanglingStringMon("Tail Whip"));

            // Commons (44-70)
            r.RegisterByKey("CG-44", new CardsASR1.TimesMon("Flail", SelfCounters));
            r.RegisterByKey("CG-45", new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("CG-46", new CardsCES1.AttachFromHandToSelfMon("Growth", EnergyType.Grass, 1));
            r.RegisterByKey("CG-47", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new FlipBonusMon("Rising Lunge", 20)));
            r.RegisterByKey("CG-48", new CardsASR1.TimesMon("Retaliate", SelfCounters));
            r.RegisterByKey("CG-49", new PlainMon());
            r.RegisterByKey("CG-50", new NoRetreatMon("Sand Pit"));
            r.RegisterByKey("CG-51", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("CG-52", new ComboMon(new DiscardCardToHandMon("Sniff Out"), new FlipBonusMon("Quick Blow", 10)));
            r.RegisterByKey("CG-53", new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep));
            r.RegisterByKey("CG-54", new FlipBonusMon("Vital Pinchers", 10));
            r.RegisterByKey("CG-55", new CardsDX.TypeLiftMon("Aqua Lift", EnergyType.Water, null));
            r.RegisterByKey("CG-56", new PurePowerMon());
            r.RegisterByKey("CG-57", new CardsEMB.BenchShieldMon(false, null));
            r.RegisterByKey("CG-58", new CardsE3B.HealSelfMon("Nap", 20));
            r.RegisterByKey("CG-59", new RecoilMon("Take Down", 10));
            r.RegisterByKey("CG-60", new PlainMon());
            r.RegisterByKey("CG-61", new DrawAttackMon("Spearhead", 1));
            r.RegisterByKey("CG-62", new CardsASR1.TimesMon("Psywave", c => c.Defending == null ? 0 : c.EnergyCount(c.Defending)));
            r.RegisterByKey("CG-63", new PlainMon());
            r.RegisterByKey("CG-64", new CardsFST1.TakeLessMon(10, new ParalyzeFlipMon("Bubblebeam")));
            r.RegisterByKey("CG-65", new TailsNothingMon("Claw"));
            r.RegisterByKey("CG-66", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("CG-67", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("CG-68", new CardsPAR2.FlipConditionMon("Shining Claws", SpecialCondition.Confused));
            r.RegisterByKey("CG-69", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("CG-70", new PlainMon());

            RegisterExAndTrainers(r);
        }

        // ================================================================== Shared

        internal static int SelfCounters(EffectContext ctx) { return ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters; }

        private static readonly SpecialCondition[] AllConditions =
            { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

        /// <summary>How many Special Conditions the Pokémon has (Reactive Poison).</summary>
        internal static int ConditionCount(PokemonInPlay mon) { return mon == null ? 0 : AllConditions.Count(mon.Has); }

        /// <summary>"If the Defending Pokémon is [kind], it is now [condition]" after the damage (Night Murmurs).</summary>
        internal sealed class IfDefendingConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;
            private readonly Func<PokemonInPlay, bool> kind;

            public IfDefendingConditionMon(string attack, SpecialCondition condition, Func<PokemonInPlay, bool> kind)
            {
                this.attack = attack;
                this.condition = condition;
                this.kind = kind;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool fits = defending != null && kind(defending);
                ctx.DamageDefending(atk.Damage);
                if (fits && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"Put any 1 card from your discard pile into your hand" (Sniff Out, Psychic Select).</summary>
        internal sealed class DiscardCardToHandMon : Mon
        {
            private readonly string attack;

            public DiscardCardToHandMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Discard.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, attack + ": put a card from your discard pile into your hand");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        /// <summary>
        /// Overzealous (Poké-Body): while the opponent has any Pokémon-ex in play, this Pokémon's attacks do 30 more to the Defending
        /// Pokémon (before W/R), like Hidden Legends' Machamp.
        /// </summary>
        internal sealed class OverzealousMon : Mon
        {
            private readonly Mon attacks;

            public OverzealousMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !src.OpponentPokemon().Any(m => m.Def.IsOldEx)) return damage;
                return damage + 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Thick Fat (Poké-Body): attack damage from Fire and Water Pokémon is 30 less (after W/R).</summary>
        internal sealed class ThickFatMon : Mon
        {
            private readonly Mon attacks;

            public ThickFatMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(a, EnergyType.Fire) || src.Engine.IsType(a, EnergyType.Water) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Choose 3 of your opponent's Pokémon. This attack does N damage to each of them" (all of them if fewer).</summary>
        internal sealed class ThreeTargetsMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public ThreeTargetsMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var picks = all.Count <= 3 ? all : ctx.ChoosePokemonMany(all, 3, 3, attack + ": choose 3 of your opponent's Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        // ================================================================== Rares

        /// <summary>Shield Veil (Poké-Body): each of your Active Pokémon has no Weakness. Enraged Linear Attack. Skull Bash.</summary>
        private sealed class BlastoiseShieldVeil : Mon
        {
            private static readonly Mon Linear = new CardsLM.EnragedLinearAttackMon();

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && src.IsActive(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Linear.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Protection (Poké-Body): attack damage from the opponent's δ Pokémon is 40 less (after W/R). Linear Attack: 30 to 1 of the opponent's Pokémon.</summary>
        private sealed class CameruptDeltaProtection : Mon
        {
            private static readonly Mon Linear = new CardsSSB.AnyOpponentTextMon("Linear Attack", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return CardsDS.Delta(src, a) ? damage - 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Linear.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Peal of Thunder (Poké-Power): when you play Charizard from your hand to evolve, you may look at the top 5 cards of your
        /// deck and attach any of the Energy cards there to 1 of your Pokémon; the other cards are discarded. Not with a Special
        /// Condition. Metal Burn: discard all Metal Energy from Charizard.
        /// </summary>
        private sealed class CharizardPealOfThunder : Mon
        {
            private const string Power = "Peal of Thunder";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Me.Deck.Count == 0 || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, CardsE1.NoCondition(me), "Use " + Power + "? Look at the top 5 cards of your deck, attach any Energy cards there to 1 of your Pokémon and discard the rest.")) return;
                var top = ctx.LookAtTop(5);
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                var attached = new List<CardInstance>();
                if (energy.Count > 0)
                {
                    var picks = ctx.ChooseCards(energy, 0, energy.Count, Power + ": choose the Energy cards to attach to 1 of your Pokémon");
                    if (picks.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(ctx.MyPokemon(), Power + ": attach the Energy cards to which of your Pokémon?");
                        foreach (var card in picks)
                            if (target != null && ctx.InPlay(target) && ctx.Engine.AttachEnergyCard(card, target)) attached.Add(card);
                    }
                }
                foreach (var card in top.Where(c => !attached.Contains(c) && !IsSomewhere(ctx, c))) ctx.Me.Discard.Add(card);
            }

            /// <summary>A card that AttachEnergyCard refused already went back somewhere (the hand or discard pile).</summary>
            private static bool IsSomewhere(EffectContext ctx, CardInstance card)
            {
                return ctx.Me.Hand.Contains(card) || ctx.Me.Discard.Contains(card) || ctx.Me.Deck.Contains(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Burn") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                foreach (var card in me.Energy.Where(e => e.Def.Name == "Metal Energy" || CardsRS.BasicOf(e.Def, EnergyType.Metal)).ToList())
                    ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Sand Veil (Poké-Body): no damage to your Benched Pokémon from the opponent's attacks. Dig Under. Double-edge.</summary>
        private sealed class DugtrioSandVeil : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsEMB.DigUnderMon(30), new RecoilMon("Double-edge", 10));

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                var a = info.Attacker;
                if (src.Pokemon == null || t == null || t.Owner != src.Player || !src.Me.Bench.Contains(t) || a == null || a.Owner == src.Player) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Even Game: search your deck for up to as many Basic Pokémon as your opponent has Pokémon in play and put them onto your
        /// Bench (Compendium: whoever has more; not with a full Bench).
        /// </summary>
        private sealed class EvenGameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Even Game") return false;
                int n = Math.Min(ctx.OpponentPokemon().Count, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToBench(d => d.IsBasicPokemon, n, "Even Game: put up to " + n + " Basic Pokémon onto your Bench");
                return true;
            }
        }

        /// <summary>
        /// Stadium Play: if you have a Stadium in play, remove 1 damage counter from each of your Pokémon; if your opponent has one,
        /// 10 damage to each of their Benched Pokémon (no W/R).
        /// </summary>
        private sealed class StadiumPlayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stadium Play") return false;
                bool mine = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
                bool theirs = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent;
                ctx.DamageDefending(atk.Damage);
                if (mine)
                    foreach (var mon in ctx.MyPokemon())
                        if (mon.DamageCounters > 0) ctx.Heal(mon, 10);
                if (theirs)
                    foreach (var mon in ctx.Opp.Bench.ToList())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Strong Current: with a Pokémon Tool on this Pokémon, 20 to each of the opponent's Benched Pokémon-ex (no W/R).</summary>
        private sealed class StrongCurrentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong Current") return false;
                bool tool = ctx.Pokemon != null && ctx.Pokemon.Tools.Count > 0;
                ctx.DamageDefending(atk.Damage);
                if (tool)
                    foreach (var mon in ctx.Opp.Bench.Where(m => m.Def.IsOldEx).ToList())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>
        /// Mining: search your deck for a Trainer card (Supporters and Stadiums included: Compendium) and put it into your hand, or
        /// attach a Pokémon Tool found that way to 1 of your Pokémon instead.
        /// </summary>
        private sealed class MiningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mining") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsTrainer, 1, "Mining: choose a Trainer card");
                foreach (var card in found)
                {
                    bool attached = false;
                    if (card.Def.IsTool && ctx.AskYesNo("Mining: attach " + card.Def.Name + " to 1 of your Pokémon instead of putting it into your hand?"))
                    {
                        var targets = ctx.MyPokemon(m => ctx.Engine.CanAttachToolTo(card, m));
                        var target = targets.Count == 0 ? null : ctx.ChoosePokemon(targets, "Mining: attach " + card.Def.Name + " to which of your Pokémon?");
                        if (target != null)
                        {
                            ctx.Engine.AttachTool(card, target);
                            attached = true;
                        }
                    }
                    if (!attached) ctx.Me.Hand.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Excavate (Poké-Power): once during your turn, look at the top card of your deck and put it back or discard it. Not with
        /// a Special Condition. Disable: the Defending Pokémon can't use the attack you choose during the opponent's next turn.
        /// </summary>
        private sealed class SableyeExcavate : Mon
        {
            private static readonly Mon Disable = new CardsTEU1.AmnesiaMon("Disable");

            protected override string Activated { get { return "Excavate"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return;
                if (ctx.ChooseOption("Excavate: " + top[0].Def.Name + " is on top of your deck", "Leave it on top", "Discard it") == 1)
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Disable.HandleAttack(ctx, atk); }
        }

        /// <summary>Pick and Choose: the Defending Pokémon is now Burned or Poisoned, and Asleep or Confused (you choose).</summary>
        private sealed class PickAndChooseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pick and Choose") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                var first = ctx.ChooseOption("Pick and Choose: Burned or Poisoned?", "Burned", "Poisoned") == 0 ? SpecialCondition.Burned : SpecialCondition.Poisoned;
                var second = ctx.ChooseOption("Pick and Choose: Asleep or Confused?", "Asleep", "Confused") == 0 ? SpecialCondition.Asleep : SpecialCondition.Confused;
                ctx.ApplyCondition(defending, first);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, second);
                return true;
            }
        }

        /// <summary>Crush Chance (Poké-Power): when you put Tauros onto your Bench from your hand, you may discard a Stadium card in play. Call for Family: 2 Basic Pokémon.</summary>
        private sealed class TaurosCrushChance : Mon
        {
            private const string Power = "Crush Chance";
            private static readonly Mon Family = new BenchSearchAttackMon("Call for Family", 2);

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.State.Stadium == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Discard " + ctx.State.Stadium.Def.Name + ".")) return;
                ctx.Engine.DiscardStadium();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Family.HandleAttack(ctx, atk); }
        }

        /// <summary>Fluffy Fur (Poké-Body): if Wigglytuff is your Active Pokémon and is damaged by an opponent's attack (even if Knocked Out), the Attacking Pokémon is now Asleep. Collect. Pester.</summary>
        private sealed class WigglytuffFluffyFur : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new DrawAttackMon("Collect", 3), CardsSS.ConditionBonus("Pester", 20, m => ConditionCount(m) > 0));

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || damage <= 0) return;
                Announce(ctx, "Fluffy Fur");
                ctx.ApplyCondition(attacker, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Hydro Pump: 20 more for each Water Energy not used to pay for the attack (no more than 40 more).</summary>
        private sealed class HydroPumpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Pump") return false;
                ctx.DamageDefending(atk.Damage + 20 * CardsBS1.ExtraWaterMon.Extra(ctx, atk));
                return true;
            }
        }

        /// <summary>
        /// Spike Storm (Poké-Power): once during your turn, if Cacturne is your Active Pokémon, 1 damage counter on 1 of the
        /// opponent's Pokémon that already has any. Not with a Special Condition. Triple Needle: 10 to each of 3 of their Pokémon.
        /// </summary>
        private sealed class CacturneSpikeStorm : Mon
        {
            private static readonly Mon Needle = new ThreeTargetsMon("Triple Needle", 10);

            protected override string Activated { get { return "Spike Storm"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.OpponentPokemon(m => m.DamageCounters > 0).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Spike Storm: put 1 damage counter on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Needle.HandleAttack(ctx, atk); }
        }

        // ================================================================== Uncommons

        /// <summary>Focus Energy: during your next turn, this Pokémon's High Jump Kick's base damage is 70.</summary>
        private sealed class FocusEnergyMon : Mon
        {
            private static string Mark(PokemonInPlay me) { return "Focus Energy#" + me.Uid; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus Energy")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.MarkNamedLimit(Mark(me));
                    return true;
                }
                if (atk.Name != "High Jump Kick") return false;
                int turn;
                bool focused = me != null && ctx.Me.NamedLimitUsedOnTurn.TryGetValue(Mark(me), out turn) && turn == ctx.MyLastTurn;
                ctx.DamageDefending(focused ? 70 : atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Cursed Glare (Poké-Body): while Dusclops is your Active Pokémon, the opponent can't attach Special Energy cards other than
        /// Darkness and Metal Energy from their hand to their Active Pokémon. Psychic Shield: no effects of attacks, damage
        /// included, from the opponent's Pokémon-ex during their next turn.
        /// </summary>
        private sealed class DusclopsCursedGlare : Mon
        {
            private static readonly Mon Shield = new CardsDX.ShieldFromMon("Psychic Shield", m => m.Def.IsOldEx, "Pokémon-ex");

            public override bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || energy == null || target == null || target.Owner == src.Player || !src.IsActive(target)) return false;
                var d = energy.Def;
                return (d.IsSpecialEnergy || d.AttachesAsEnergy) && d.Name != "Darkness Energy" && d.Name != "Metal Energy";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shield.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Delta Sign (Poké-Power): once during your turn, search your deck for a Pokémon with δ and put it into your hand. Only 1
        /// Delta Sign each turn; not with a Special Condition.
        /// </summary>
        private sealed class FearowDeltaSign : Mon
        {
            protected override string Activated { get { return "Delta Sign"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => CardsDS.DeltaCard(ctx, ctx.Player, d), 1, "Delta Sign: choose a Pokémon that has δ on its card");
            }
        }

        /// <summary>
        /// Hover Lift (Poké-Body): you pay 1 Colorless less to retreat your Jigglypuff, Wigglytuff, Wigglytuff ex and Igglybuff.
        /// Baby Evolution (Poké-Power): put Jigglypuff from your hand onto Igglybuff and remove all its damage counters.
        /// </summary>
        private sealed class IgglybuffHoverLift : Mon
        {
            private static readonly string[] Lifted = { "Jigglypuff", "Wigglytuff", "Wigglytuff ex", "Igglybuff" };
            private static readonly CardScript Baby = new CardsSS.BabyEvolutionMon("Jigglypuff", null);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || cost <= 0) return cost;
                return Lifted.Contains(mon.Name) ? cost - 1 : cost;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex) { return Baby.CanUseAbility(ctx, abilityIndex); }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { Baby.UseAbility(ctx, abilityIndex); }
        }

        /// <summary>Dual Armor (Poké-Body): with any Psychic Energy card attached, Medicham is both Psychic and Fighting. Psyshock. Sky Uppercut: ignores Resistance.</summary>
        private sealed class MedichamDualArmor : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ParalyzeFlipMon("Psyshock"), new CardsFLF1.NoResistanceMon("Sky Uppercut"));

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                var me = src.Pokemon;
                if (me == null || mon != me || !me.Energy.Any(e => CardsRS.BasicOf(e.Def, EnergyType.Psychic))) return;
                if (!types.Contains(EnergyType.Psychic)) types.Add(EnergyType.Psychic);
                if (!types.Contains(EnergyType.Fighting)) types.Add(EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Transport (Poké-Power): once during your turn, if Pelipper is on your Bench, switch your Active Pokémon that has δ with 1 of your Benched Pokémon. Supersonic.</summary>
        private sealed class PelipperDeltaTransport : Mon
        {
            private static readonly Mon Supersonic = new ConditionAttackMon("Supersonic", SpecialCondition.Confused);

            protected override string Activated { get { return "Delta Transport"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Active != null && CardsDS.Delta(ctx, ctx.Me.Active);
            }

            protected override void Ability(EffectContext ctx) { ctx.SwitchMyActive("Delta Transport: switch your Active Pokémon with which of your Benched Pokémon?"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Supersonic.HandleAttack(ctx, atk); }
        }

        /// <summary>Echo Draw (Poké-Power): once during your turn, draw a card. Not with a Special Condition. Rock Hurl: ignores Resistance.</summary>
        private sealed class SwampertEchoDraw : Mon
        {
            private static readonly Mon Hurl = new CardsFLF1.NoResistanceMon("Rock Hurl");

            protected override string Activated { get { return "Echo Draw"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hurl.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Chlorophyll (Poké-Body): Energy cards that provide only Colorless Energy attached to your Grass Pokémon provide Grass
        /// Energy instead (Compendium: Boost Energy gives 3 Grass). Green Blast: 10 more for each Grass Energy on your Pokémon.
        /// Toxic Sleep: Asleep and Poisoned, 2 damage counters at each Pokémon Checkup.
        /// </summary>
        private sealed class VenusaurChlorophyll : Mon
        {
            private static readonly Mon Attacks = new ComboMon(
                new CardsASR1.PlusMon("Green Blast", 10, c => c.MyPokemon().Sum(m => m.Energy.Count(e => CardsHIF1.Provides(c, m, e, EnergyType.Grass)))),
                new ToxicSleepMon());

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || holder == null || holder.Owner != src.Player || units.Count == 0 || !src.Engine.IsType(holder, EnergyType.Grass)) return;
                int colorless = EnergyMask.Of(EnergyType.Colorless);
                if (units.Any(u => u != colorless)) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Grass);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Toxic Sleep: the Defending Pokémon is now Asleep and Poisoned, 2 damage counters at each Pokémon Checkup.</summary>
        private sealed class ToxicSleepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toxic Sleep") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 2;
                return true;
            }
        }

        // ================================================================== Commons

        /// <summary>Pure Power: put 2 damage counters on your opponent's Pokémon in any way you like.</summary>
        private sealed class PurePowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pure Power") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.PlaceCountersAnyWay(2, () => ctx.OpponentPokemon());
                return true;
            }
        }
    }
}
