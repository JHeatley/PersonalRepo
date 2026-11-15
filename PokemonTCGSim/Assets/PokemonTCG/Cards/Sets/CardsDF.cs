using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragon Frontiers (DF, 2006; v171): the cards not already in the pool (the others are prints). Unlimited only. Every Pokémon
    /// in the set is δ. Pokémon here (1-71); Trainers, Pokémon-ex and Pokémon ★ in CardsDFEx.cs. Compendium rulings used: Holon
    /// Veil covers only its owner's Pokémon (and makes Fossils δ while they're Pokémon); Battle Aura and Extra Smoke ride on the
    /// attacking Pokémon; Sharing doesn't use up your Supporter for the turn; Delta Reduction stays on the Defending Pokémon; Imprison
    /// and Shock-wave markers stay until the Pokémon evolves, devolves or leaves play (or Tropical Heal); Island Hermit must turn at
    /// least 1 face-down Prize card face up; Link Wing doesn't cover Pokémon ★; Rage Aura sets the attack costs.
    /// </summary>
    public static partial class CardsDF
    {
        public const string ShockWaveMarker = "Shock-wave";

        public static void Register(CardScriptRegistry r)
        {
            // Rares (1-12)
            r.RegisterByKey("DF-1", new AmpharosHolonVeil());
            r.RegisterByKey("DF-2", new FeraligatrBattleAura());
            r.RegisterByKey("DF-3", new CardsLM.ShiningHornMon(new ComboMon(new DiscardEnergyToHandMon("Dig Deep"),
                                                                            CardsSS.ConditionBonus("Extra Claws", 20, d => d.Def.IsOldEx))));
            r.RegisterByKey("DF-4", new MeganiumEvolutionaryCall());
            r.RegisterByKey("DF-5", new MiloticSharing());
            r.RegisterByKey("DF-6", new ComboMon(new CardsSSB.AnyOpponentTextMon("Linear Attack", 30), new DarkHornMon()));
            r.RegisterByKey("DF-7", new NidoqueenInvitation());
            r.RegisterByKey("DF-8", new NinetalesVolunteer());
            r.RegisterByKey("DF-9", new PinsirArmor());
            r.RegisterByKey("DF-10", new SnorlaxBedhead());
            r.RegisterByKey("DF-11", new DeltaCopyMon());
            r.RegisterByKey("DF-12", new TyphlosionShadyMove());

            // Uncommons (13-42)
            r.RegisterByKey("DF-13", new ComboMon(new BurnedAndPoisonedMon("Burning Venom"),
                                                  new CardsFST2.ConditionBonusMon("Strangle", 30, c => c.Defending != null && CardsDS.Delta(c, c.Defending))));
            r.RegisterByKey("DF-14", new CloysterSolidShell());
            r.RegisterByKey("DF-15", new DeltaProtectionMon(new ComboMon(new ParalyzeFlipMon("Ice Beam"),
                new CardsFST2.ConditionBonusMon("Surge", 20, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water) >= 2))));
            r.RegisterByKey("DF-16", new ComboMon(new StingTurnMon(), new ExBonusConditionMon("Tail Sting", 10, SpecialCondition.Poisoned)));
            r.RegisterByKey("DF-17", new JynxStagesOfEvolution());
            r.RegisterByKey("DF-18", new LedianProwl());
            r.RegisterByKey("DF-19", new ComboMon(new DrawAttackMon("Lap Up", 2), new DeltaMindMon()));
            r.RegisterByKey("DF-20", new MantinePowerCirculation());
            r.RegisterByKey("DF-21", new QuagsireDigUp());
            r.RegisterByKey("DF-22", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("DF-23", new TropiusTropicalHeal());
            r.RegisterByKey("DF-24", new CardsDX.TypeLiftMon("Psychic Wing", EnergyType.Psychic, new FlipBonusMon("Quick Blow", 20)));
            r.RegisterByKey("DF-25", new XatuExtraFeather());
            r.RegisterByKey("DF-26", new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("DF-27", new CardsEM.ScaryFaceMon());
            r.RegisterByKey("DF-28", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("DF-29", new ElectabuzzPowerOfEvolution());
            r.RegisterByKey("DF-30", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("DF-31", new SwitchSelfMon("Reverse Thrust", false));
            r.RegisterByKey("DF-33", new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep));
            r.RegisterByKey("DF-34", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("DF-35", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("DF-36", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("DF-37", CardsSS.ConditionBonus("Extra Ball", 30, d => d.Def.IsOldEx));
            r.RegisterByKey("DF-38", new RecoilMon("Double-edge", 10));
            r.RegisterByKey("DF-39", new ComboMon(new DrawAttackMon("Collect", 1), new ExBonusConditionMon("Flickering Tail", 10, SpecialCondition.Asleep)));
            r.RegisterByKey("DF-40", new SwellowExtraWing());
            r.RegisterByKey("DF-41", new CardsMEG.WeakenDefendingMon("Charm", 20));
            r.RegisterByKey("DF-42", new CardsCG.IfDefendingConditionMon("Sonic Noise", SpecialCondition.Confused, m => m.Def.IsOldEx));

            // Commons (43-71)
            r.RegisterByKey("DF-43", new ShieldAttackMon("Granite Head", 10));
            r.RegisterByKey("DF-44", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("DF-45", new CardsMA.SwiftMon("Swift"));
            r.RegisterByKey("DF-46", new PlainMon());
            r.RegisterByKey("DF-47", new PlainMon());
            r.RegisterByKey("DF-48", new CardsUF.BabyEvolutionAnyMon(new[] { "Electabuzz" }, new CardsSSB.AnyOpponentTextMon("Thunder Spear", 10)));
            r.RegisterByKey("DF-49", new CardsASR1.TimesMon("Flail", CardsCG.SelfCounters));
            r.RegisterByKey("DF-50", new ConditionAttackMon("Sleepy Ball", SpecialCondition.Asleep));
            r.RegisterByKey("DF-52", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("DF-53", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("DF-54", new PlainMon());
            r.RegisterByKey("DF-55", new PlainMon());
            r.RegisterByKey("DF-56", new CardsLOR1.EntanglingStringMon("Tail Whip"));
            r.RegisterByKey("DF-57", new CardsPAR2.FlipConditionMon("Poison Horn", SpecialCondition.Poisoned));
            r.RegisterByKey("DF-59", new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"));
            r.RegisterByKey("DF-61", new CardsE3B.HealSelfMon("Calm Mind", 20));
            r.RegisterByKey("DF-62", new PlainMon());
            r.RegisterByKey("DF-63", new ParalyzeFlipMon("Shell Grab"));
            r.RegisterByKey("DF-64", new CardsUF.BabyEvolutionAnyMon(new[] { "Jynx" }, new AlluringKissMon()));
            r.RegisterByKey("DF-65", new CardsFST2.ConditionBonusMon("Splash About", 10, c => c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending)));
            r.RegisterByKey("DF-66", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("DF-67", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("DF-68", new PlainMon());
            r.RegisterByKey("DF-69", new CardsCES1.FeintAttackMon("Quick Bind", 10));
            r.RegisterByKey("DF-70", new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep));
            r.RegisterByKey("DF-71", new CardsTEU1.AmnesiaMon("Amnesia"));

            RegisterExAndTrainers(r);
        }

        // ================================================================== Shared

        /// <summary>A Stage 2 Pokémon-ex (Extra Feather, Extra Wing, Extra Smoke, Extra Boost).</summary>
        internal static bool Stage2Ex(PokemonInPlay mon) { return mon != null && mon.Def.IsOldEx && mon.Def.Stage == Stage.Stage2; }

        /// <summary>"Search your discard pile for an Energy card ... and put it into your hand" (Dig Deep).</summary>
        internal sealed class DiscardEnergyToHandMon : Mon
        {
            private readonly string attack;

            public DiscardEnergyToHandMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, attack + ": put which Energy card into your hand?"));
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon is Pokémon-ex, this attack does N more damage and it is now [condition]" (Tail Sting, Flickering Tail).</summary>
        internal sealed class ExBonusConditionMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly SpecialCondition condition;

            public ExBonusConditionMon(string attack, int bonus, SpecialCondition condition)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool ex = defending != null && defending.Def.IsOldEx;
                ctx.DamageDefending(atk.Damage + (ex ? bonus : 0));
                if (ex && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>The Defending Pokémon is now Burned and Poisoned (Burning Venom).</summary>
        internal sealed class BurnedAndPoisonedMon : Mon
        {
            private readonly string attack;

            public BurnedAndPoisonedMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Delta Protection (Poké-Body): attack damage from the opponent's δ Pokémon is 40 less (after W/R).</summary>
        internal sealed class DeltaProtectionMon : Mon
        {
            private readonly Mon attacks;

            public DeltaProtectionMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return CardsDS.Delta(src, a) ? damage - 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Rares

        /// <summary>
        /// Holon Veil (Poké-Body): your Pokémon in play, and the Basic Pokémon and Evolution cards in your deck, discard pile and hand,
        /// count as δ Pokémon (CardsDS.Delta / DeltaCard). Delta Circle: 10 more for each of your δ Pokémon in play.
        /// </summary>
        private sealed class AmpharosHolonVeil : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Circle") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.MyPokemon(m => CardsDS.Delta(ctx, m)).Count);
                return true;
            }
        }

        /// <summary>Battle Aura (Poké-Body): each of your δ Pokémon's attacks do 10 more to the Defending Pokémon (before W/R). Drag Off.</summary>
        private sealed class FeraligatrBattleAura : Mon
        {
            private static readonly Mon Drag = new CardsE3B.DragOffMon(true);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && CardsDS.Delta(src, a) ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drag.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Evolutionary Call (Poké-Power): when you play Meganium from your hand to evolve, you may search your deck for up to 3
        /// Basic Pokémon and Evolution cards and put them into your hand. Delta Reduction: the Defending Pokémon's attacks do 30 less
        /// during the opponent's next turn (before W/R).
        /// </summary>
        internal sealed class MeganiumEvolutionaryCall : Mon
        {
            private const string Power = "Evolutionary Call";
            private static readonly Mon Reduction = new CardsMEG.WeakenDefendingMon("Delta Reduction", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, ctx.Me.Deck.Count > 0, "Use " + Power + "? Search your deck for up to 3 Basic Pokémon and Evolution cards.")) return;
                ctx.SearchToHand(d => d.IsPokemon, 3, Power + ": choose up to 3 Basic Pokémon and Evolution cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Reduction.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Sharing (Poké-Power): once during your turn, look at your opponent's hand; you may use the effect of a Supporter there as
        /// this power's effect (it stays in their hand; it doesn't use up your Supporter: Compendium). 1 Sharing a turn; not with a
        /// Special Condition.
        /// </summary>
        private sealed class MiloticSharing : Mon
        {
            protected override string Activated { get { return "Sharing"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Sharing: your opponent's hand", ctx.Source);
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var card in hand)
                    if (card.Def.IsSupporter && seen.Add(card.Def.Name) && MrMimeTEF.WouldDoSomething(ctx, card)) usable.Add(card);
                if (usable.Count == 0) return;
                var labels = usable.Select(c => "Use " + c.Def.Name + "'s effect").ToList();
                labels.Add("Don't use a Supporter's effect");
                int pick = ctx.ChooseOption("Sharing: use the effect of a Supporter from your opponent's hand?", labels.ToArray());
                if (pick < 0 || pick >= usable.Count) return;
                var chosen = usable[pick];
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + chosen.Def.Name + " (Sharing).");
                ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
                // The card stays in the opponent's hand, whatever its effect says about the card itself.
                ctx.Me.Hand.Remove(chosen);
                ctx.Me.Discard.Remove(chosen);
                ctx.Me.Deck.Remove(chosen);
                if (!ctx.Opp.Hand.Contains(chosen)) ctx.Opp.Hand.Add(chosen);
            }
        }

        /// <summary>Dark Horn: you may discard a Basic Pokémon or Evolution card from your hand for 20 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class DarkHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Horn") return false;
                ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (cards.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
                var pick = ctx.ChooseCards(cards, 0, 1, "Dark Horn: discard a Basic Pokémon or Evolution card from your hand for 20 damage to 1 of your opponent's Benched Pokémon?");
                if (pick.Count == 0) return true;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>
        /// Invitation (Poké-Power): once during your turn, search your deck for a Basic Pokémon or Evolution card and put it into your
        /// hand. Not with a Special Condition. Vengeance: 10 more for each Pokémon card in your discard pile (up to 60 more).
        /// </summary>
        private sealed class NidoqueenInvitation : Mon
        {
            private static readonly Mon Vengeance = new CardsASR1.PlusMon("Vengeance", 10, c => Math.Min(6, c.Me.Discard.Count(x => x.Def.IsPokemon)));

            protected override string Activated { get { return "Invitation"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(d => d.IsPokemon, 1, "Invitation: choose a Basic Pokémon or Evolution card"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Vengeance.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Volunteer (Poké-Power): once during your turn, remove 4 damage counters from Ninetales and discard it from Vulpix; if you
        /// do, search your deck for Ninetales δ or Ninetales ex and put it onto Vulpix (evolving it). Trick Tail: attach an Energy
        /// card from your discard pile to 1 of your Pokémon.
        /// </summary>
        private sealed class NinetalesVolunteer : Mon
        {
            protected override string Activated { get { return "Volunteer"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return me != null && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Vulpix";
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, Math.Min(40, 10 * me.DamageCounters));
                ctx.Engine.DevolveIntoDiscard(me, 1);
                if (!ctx.InPlay(me)) return;
                if (ctx.Me.Deck.Count > 0)
                {
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == "Vulpix" && (d.Name == "Ninetales" && d.IsDeltaSpecies || d.Name == "Ninetales ex"), 1,
                                                 "Volunteer: choose Ninetales δ or Ninetales ex to put onto Vulpix");
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(me, found[0]);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick Tail") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Trick Tail: attach which Energy card from your discard pile?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Trick Tail: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card))
                    ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Armor (Poké-Body): while the opponent has 5 or more cards in hand, attack damage to Pinsir is 30 less (after W/R). Delta Call.</summary>
        private sealed class PinsirArmor : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Opp.Hand.Count >= 5 ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Call") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => CardsDS.DeltaCard(ctx, ctx.Player, d), 1, "Delta Call: choose a Pokémon that has δ on its card");
                return true;
            }
        }

        /// <summary>
        /// Bedhead (Poké-Body): while Snorlax stays Asleep between turns, 2 damage counters on the Defending Pokémon. Dozing
        /// (Poké-Power): once during your turn, if Snorlax is Active, remove 2 damage counters from it and it's now Asleep; not with a
        /// Special Condition.
        /// </summary>
        private sealed class SnorlaxBedhead : Mon
        {
            protected override string Activated { get { return "Dozing"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsDS.PowerReady(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, 20);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
            }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !me.Has(SpecialCondition.Asleep) || src.Engine.AbilityBlocked(me, "Bedhead")) return;
                var defending = src.Opp.Active;
                if (defending == null) return;
                Announce(src, "Bedhead");
                src.Engine.PlaceDamageCounters(defending, 2, "Bedhead");
            }
        }

        /// <summary>Delta Copy: use an attack of 1 of your opponent's δ Pokémon in play as this attack, without its Energy cost.</summary>
        private sealed class DeltaCopyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Copy") return false;
                var deltas = ctx.OpponentPokemon(m => CardsDS.Delta(ctx, m) && m.Def.Attacks.Count > 0);
                if (deltas.Count == 0) return true;
                var mon = deltas.Count == 1 ? deltas[0] : ctx.ChoosePokemon(deltas, "Delta Copy: copy an attack of which of your opponent's δ Pokémon?");
                CardDef source;
                int index;
                if (!ChooseCopiedAttack(ctx, mon, "Delta Copy: copy which attack?", out source, out index)) return true;
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>
        /// Shady Move (Poké-Power): once during your turn, if Typhlosion is Active, move 1 damage counter from any Pokémon to another
        /// (either player's). Not with a Special Condition. Burning Ball: with 2 or more Fire Energy on Typhlosion, Burned.
        /// </summary>
        private sealed class TyphlosionShadyMove : Mon
        {
            private const string Power = "Shady Move";

            private static List<PokemonInPlay> All(EffectContext ctx) { return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList(); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.PokemonIsActive && All(ctx).Count > 1 && All(ctx).Any(m => m.DamageCounters > 0);
            }

            protected override void Ability(EffectContext ctx)
            {
                var all = All(ctx);
                var from = ctx.ChoosePokemon(all.Where(m => m.DamageCounters > 0).ToList(), Power + ": move a damage counter from which Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(all.Where(m => m != from).ToList(), Power + ": move it to which Pokémon?");
                if (to != null) ctx.Engine.MoveDamageCounters(from, to, 1, Power);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Ball") return false;
                var defending = ctx.Defending;
                bool burn = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fire) >= 2;
                ctx.DamageDefending(atk.Damage);
                if (burn && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        // ================================================================== Uncommons

        /// <summary>Solid Shell (Poké-Body): no effects of the opponent's attacks, damage included, to your Benched δ Pokémon. Grind.</summary>
        private sealed class CloysterSolidShell : Mon
        {
            private static readonly Mon Grind = new CardsASR1.PlusMon("Grind", 10, c => c.EnergyCount(c.Pokemon));

            private static bool Covers(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && src.Me.Bench.Contains(target) && CardsDS.Delta(src, target);
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (a == null || a.Owner == src.Player || !Covers(src, info.Target)) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (kind != EffectSourceKind.Attack || !Covers(src, target)) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Grind.HandleAttack(ctx, atk); }
        }

        /// <summary>Sting Turn: flip a coin; heads, the Defending Pokémon is now Paralyzed and you switch Gligar with 1 of your Benched Pokémon.</summary>
        private sealed class StingTurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sting Turn") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Stages of Evolution (Poké-Body): while Jynx is an Evolved Pokémon, you pay 1 Colorless less to retreat your Fire and Psychic Pokémon.</summary>
        private sealed class JynxStagesOfEvolution : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner != src.Player || cost <= 0 || !CardsE2.IsEvolved(me)) return cost;
                return src.Engine.IsType(mon, EnergyType.Fire) || src.Engine.IsType(mon, EnergyType.Psychic) ? cost - 1 : cost;
            }
        }

        /// <summary>Prowl (Poké-Power): when you play Ledian from your hand to evolve, you may search your deck for any 1 card. Metal Star: with a Pokémon Tool on Ledian, draw 3 cards.</summary>
        private sealed class LedianProwl : Mon
        {
            private const string Power = "Prowl";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || ctx.Engine.AbilityBlocked(ctx.Pokemon, Power)) return;
                if (!TriggerOnce(ctx, Power, ctx.Me.Deck.Count > 0, Power + ": search your deck for any card?")) return;
                ctx.SearchToHand(d => true, 1, Power + ": choose any card", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Star") return false;
                bool tool = ctx.Pokemon != null && ctx.Pokemon.Tools.Count > 0;
                ctx.DamageDefending(atk.Damage);
                if (tool) ctx.Draw(3);
                return true;
            }
        }

        /// <summary>Delta Mind: 1 damage counter on 1 of the opponent's Pokémon, 3 if it has δ.</summary>
        private sealed class DeltaMindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Mind") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Delta Mind: put damage counters on which of your opponent's Pokémon? (3 on a δ Pokémon, otherwise 1)");
                if (target != null) ctx.PlaceCounters(target, CardsDS.Delta(ctx, target) ? 3 : 1);
                return true;
            }
        }

        /// <summary>
        /// Power Circulation (Poké-Power): once during your turn, put a basic Energy card from your discard pile on top of your deck;
        /// if you do, 1 damage counter on Mantine. Not with a Special Condition. Spiral Drain: remove 1 damage counter from Mantine.
        /// </summary>
        private sealed class MantinePowerCirculation : Mon
        {
            private const string Power = "Power Circulation";
            private static readonly Mon Drain = new CardsE3B.HealSelfMon("Spiral Drain", 10);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.BasicEnergyInDiscard().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var cards = ctx.BasicEnergyInDiscard();
                if (cards.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": put which basic Energy card on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { card }, Power, Power + ": the card put on top of your opponent's deck", ctx.Source);
                ctx.Me.Deck.Insert(0, card);
                ctx.Log(Power + ": " + card.Def.Name + " was put on top of " + ctx.Engine.Possessive(ctx.Player) + " deck.");
                ctx.PlaceCounters(ctx.Pokemon, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drain.HandleAttack(ctx, atk); }
        }

        /// <summary>Dig Up (Poké-Power): when you play Quagsire from your hand to evolve, you may put up to 2 Pokémon Tools from your discard pile into your hand. Pump Out: 20 more with a Tool on Quagsire.</summary>
        private sealed class QuagsireDigUp : Mon
        {
            private const string Power = "Dig Up";
            private static readonly Mon Pump = new CardsFST2.ConditionBonusMon("Pump Out", 20, c => c.Pokemon != null && c.Pokemon.Tools.Count > 0);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var tools = ctx.Me.Discard.Where(c => c.Def.IsTool).ToList();
                if (me == null || tools.Count == 0 || ctx.Engine.AbilityBlocked(me, Power) || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Put up to 2 Pokémon Tool cards from your discard pile into your hand.")) return;
                var picks = ctx.ChooseCards(tools, 1, Math.Min(2, tools.Count), Power + ": choose up to 2 Pokémon Tool cards");
                ctx.Engine.ShowCards(ctx.Opponent, picks, Power, Power + ": the cards your opponent took", ctx.Source);
                ctx.DiscardToHand(picks);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Tropical Heal (Poké-Power): when you put Tropius onto your Bench from your hand, you may remove all Special Conditions,
        /// Imprison markers and Shock-wave markers from your Pokémon. Grind: 10 for each Energy on Tropius.
        /// </summary>
        private sealed class TropiusTropicalHeal : Mon
        {
            private const string Power = "Tropical Heal";
            private static readonly Mon Grind = new CardsASR1.TimesMon("Grind", c => c.EnergyCount(c.Pokemon));

            private static bool Needed(EffectContext ctx)
            {
                return ctx.MyPokemon().Any(m => CardsCG.ConditionCount(m) > 0 || m.Markers.ContainsKey(PokemonInPlay.ImprisonMarker) || m.Markers.ContainsKey(ShockWaveMarker));
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, Needed(ctx), "Use " + Power + "? Remove all Special Conditions, Imprison markers and Shock-wave markers from your Pokémon.")) return;
                foreach (var mon in ctx.MyPokemon())
                {
                    ctx.Engine.RemoveAllConditions(mon);
                    mon.Markers.Remove(PokemonInPlay.ImprisonMarker);
                    mon.Markers.Remove(ShockWaveMarker);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Grind.HandleAttack(ctx, atk); }
        }

        /// <summary>Extra Feather (Poké-Body): each of your Stage 2 Pokémon-ex does 10 more damage to the Defending Pokémon (before W/R). Confuse Ray.</summary>
        private sealed class XatuExtraFeather : Mon
        {
            private static readonly Mon Ray = new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Stage2Ex(a) ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Power of Evolution (Poké-Power): once during your turn, if Electabuzz is an Evolved Pokémon, draw the bottom card of your
        /// deck. Not with a Special Condition. Swift.
        /// </summary>
        private sealed class ElectabuzzPowerOfEvolution : Mon
        {
            private static readonly Mon Swift = new CardsMA.SwiftMon("Swift");

            protected override string Activated { get { return "Power of Evolution"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && CardsE2.IsEvolved(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                var card = deck[deck.Count - 1];
                deck.RemoveAt(deck.Count - 1);
                ctx.Me.Hand.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew the bottom card of " + ctx.Engine.Their(ctx.Player) + " deck (Power of Evolution).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swift.HandleAttack(ctx, atk); }
        }

        /// <summary>Extra Wing (Poké-Body): your Stage 2 Pokémon-ex have no Retreat Cost. Agility.</summary>
        private sealed class SwellowExtraWing : Mon
        {
            private static readonly Mon Agility = new DodgeMon("Agility", false);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && Stage2Ex(mon) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Agility.HandleAttack(ctx, atk); }
        }

        // ================================================================== Commons

        /// <summary>Alluring Kiss: search your deck for a Basic Pokémon and a basic Energy card and put them into your hand.</summary>
        private sealed class AlluringKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Alluring Kiss") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(d => d.IsBasicPokemon, 1, "Alluring Kiss: choose a Basic Pokémon"));
                if (ctx.Me.Deck.Count > 0) found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Alluring Kiss: choose a basic Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }
    }
}
