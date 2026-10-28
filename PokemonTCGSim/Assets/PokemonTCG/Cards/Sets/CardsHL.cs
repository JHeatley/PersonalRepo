using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Hidden Legends Rare Pokémon and Pokémon-ex (HL 1-27, 93-102; v161). Unlimited only. Metang 21 is the Nintendo Black Star
    /// Promo 23 (a print). 1-on-1: "each Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsHL
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HL-1", new ComboMon(new CardsASR1.PlusMon("Shadow Steal", 20, c => c.Opp.Discard.Count(x => x.Def.IsEnergy && !x.Def.IsBasicEnergy)), new DarknessChantMon()));
            r.RegisterByKey("HL-2", new ClaydolPrimalPull());
            r.RegisterByKey("HL-3", new ComboMon(new FlutterTrickMon(), new CardsBS1.ToxicMon("Triple Poison", 3)));
            r.RegisterByKey("HL-4", new ComboMon(new DarkProvideMon(), new ConditionAttackMon("Leaf Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("HL-5", new ComboMon(new CardsMA.SwiftMon("Swift"), new MassDestructionMon()));
            r.RegisterByKey("HL-6", new ComboMon(new CardsSS.EachOpponentMon("Breaking Sound", 10), new CardsSSB.AnyOpponentTextMon("Bass Control", 30),
                new ConditionAttackMon("Thunderous Roar", SpecialCondition.Confused)));
            r.RegisterByKey("HL-7", new ComboMon(new ExtraDrawMon(), new NoWeaknessResistanceMon("Sonicboom")));
            r.RegisterByKey("HL-8", new ComboMon(new MakeAWishMon(), new CardsPAR2.FlipConditionMon("Mind Bend", SpecialCondition.Confused)));
            r.RegisterByKey("HL-9", new MachampOverzealous());
            r.RegisterByKey("HL-10", new ComboMon(new CardsASR1.PlusMon("Meditate", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters),
                new CardsASR1.PlusMon("Chakra Points", 10, c => c.Opp.Hand.Count)));
            r.RegisterByKey("HL-11", new MetagrossMetalJuncture());
            r.RegisterByKey("HL-12", new MiloticHealingShower());
            r.RegisterByKey("HL-13", new PinsirCrust());
            r.RegisterByKey("HL-14", new ComboMon(new PushAsideMon(), new CardsFST2.ConditionBonusMon("Supernatural Power", 40, c => c.Me.Hand.Count == c.Opp.Hand.Count)));
            r.RegisterByKey("HL-15", new WalreinCrushDraw());
            r.RegisterByKey("HL-16", new BellossomHealDance());
            r.RegisterByKey("HL-17", new ComboMon(new ChimeMon(), new CardsASR1.TimesMon("Psychic Boom", c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("HL-18", new ComboMon(new ParalyzeFlipMon("Stun Needle"),
                new CardsASR1.PlusMon("Mystic Water", 10, c => c.MyPokemon().Concat(c.OpponentPokemon()).Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Psychic)))));
            r.RegisterByKey("HL-19", new CardsASR1.PlusMon("Dark Splash", 10, c => c.MyPokemon().Concat(c.OpponentPokemon()).Count(m => c.Engine.IsType(m, EnergyType.Darkness))));
            r.RegisterByKey("HL-20", new ComboMon(new CardsE3B.OpponentSwitchesMon("Whirlwind"), new SilverWindMon()));
            r.RegisterByKey("HL-22", new CardsSS.SafeguardExMon(new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("HL-23", new CastformMon("Rain Castform", new ComboMon(new ScatteredShowerMon(), new StadiumConditionMon("Rainy Day Blues", "Low Pressure System", SpecialCondition.Confused))));
            r.RegisterByKey("HL-24", new RelicanthDeepSleep());
            r.RegisterByKey("HL-25", new CastformMon("Snow-cloud Castform", new ComboMon(new CardsSI.HeadsTailsConditionMon("Flurries", SpecialCondition.Paralyzed, SpecialCondition.Asleep),
                new CardsFST2.ConditionBonusMon("White Snow", 20, c => StadiumNamed(c, "Magnetic Storm")))));
            r.RegisterByKey("HL-26", new CastformMon("Sunny Castform", new ComboMon(new SunshineMon(), new StadiumConditionMon("Sunburn", "High Pressure System", SpecialCondition.Burned))));
            r.RegisterByKey("HL-27", new FruitionMon());

            r.RegisterByKey("HL-93", new MarkOfAntiquityMon("Groudon ex", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Tumble"), new CrushingMantleMon())));
            r.RegisterByKey("HL-94", new MarkOfAntiquityMon("Kyogre ex", new ComboMon(new CardsSSB.AnyOpponentTextMon("Water Arrow", 20), new SuperTidalWaveMon())));
            r.RegisterByKey("HL-95", new ComboMon(new CardsSS.SwitchFirstMon("Metal Reversal", null), new ExtraCometPunchMon()));
            r.RegisterByKey("HL-96", new ComboMon(new IntenseGlareMon(), new CardsSUM1.DiscardTypeFromSelfMon("Fire Blast", EnergyType.Fire, 1)));
            r.RegisterByKey("HL-97", new RegiceExCrystalBody());
            r.RegisterByKey("HL-98", new CardsSS.RainDishMon("Healing Stone", new TonnageMon()));
            r.RegisterByKey("HL-99", new CardsFST1.TakeLessMon(10, new SteelWaveMon()));
            r.RegisterByKey("HL-100", new VileplumeExBlockDust());
            r.RegisterByKey("HL-101", new ComboMon(new ConditionAttackMon("Sleepy Song", SpecialCondition.Asleep), new CardsASR1.PlusMon("Do the Wave", 10, c => c.Me.Bench.Count)));
            r.RegisterByKey("HL-102", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10));
        }

        // ================================================================== Shared (EX era)

        /// <summary>"Pokémon that has an owner in its name" (Team Aqua's Kyogre, Rocket's Zapdos, Brock's Onix...).</summary>
        internal static bool HasOwner(string name) { return name != null && name.Contains("'s "); }

        internal static bool StadiumNamed(EffectContext ctx, string name)
        {
            return ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == name && !ctx.Engine.StadiumSwitchedOff();
        }

        /// <summary>"This attack's damage isn't affected by Weakness or Resistance" (Sonicboom).</summary>
        internal sealed class NoWeaknessResistanceMon : Mon
        {
            private readonly string attack;

            public NoWeaknessResistanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>Use the effect of a Supporter card from the opponent's discard pile as this attack's effect (Chime), like Look-Alike Show.</summary>
        private sealed class ChimeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chime") return false;
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var card in ctx.Opp.Discard)
                    if (card.Def.IsSupporter && seen.Add(card.Def.Name) && MrMimeTEF.WouldDoSomething(ctx, card)) usable.Add(card);
                if (usable.Count == 0) return true;
                var labels = usable.Select(x => "Use " + x.Def.Name + "'s effect").ToList();
                labels.Add("Don't use a Supporter's effect");
                int pick = ctx.ChooseOption("Chime: use the effect of a Supporter from your opponent's discard pile?", labels.ToArray());
                if (pick < 0 || pick >= usable.Count) return true;
                var chosen = usable[pick];
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + chosen.Def.Name + ".");
                ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
                return true;
            }
        }

        /// <summary>"If [Stadium] is in play, the Defending Pokémon is now [condition]" (Rainy Day Blues, Sunburn).</summary>
        internal sealed class StadiumConditionMon : Mon
        {
            private readonly string attack;
            private readonly string stadium;
            private readonly SpecialCondition condition;

            public StadiumConditionMon(string attack, string stadium, SpecialCondition condition)
            {
                this.attack = attack;
                this.stadium = stadium;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (StadiumNamed(ctx, stadium) && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>
        /// Temperamental Weather (Poké-Power): once during your turn, a different Castform from your deck takes this one's place
        /// (everything on it stays: attached cards, damage counters, Special Conditions, effects), and this one is shuffled into the
        /// deck. Only 1 Temperamental Weather each turn. No Special Condition clause in the text.
        /// </summary>
        internal sealed class CastformMon : Mon
        {
            private const string Power = "Temperamental Weather";
            private static readonly string[] Forms = { "Castform", "Rain Castform", "Sunny Castform", "Snow-cloud Castform" };
            private readonly string self;
            private readonly Mon attacks;

            public CastformMon(string self, Mon attacks)
            {
                this.self = self;
                this.attacks = attacks;
            }

            private bool Other(CardDef d) { return d.IsPokemon && d.Name != self && Forms.Contains(d.Name); }

            protected override string Activated { get { return Power; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Any(c => Other(c.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var found = ctx.SearchMyDeck(Other, 1, Power + ": choose a Castform to switch with " + me.Name);
                if (found.Count == 0 || !ctx.InPlay(me))
                {
                    foreach (var card in found) ctx.Me.Deck.Add(card);
                    ctx.ShuffleMyDeck();
                    return;
                }
                var old = me.Top;
                me.Stack[me.Stack.Count - 1] = found[0];
                ctx.Me.Deck.Add(old);
                ctx.ShuffleMyDeck();
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " switched " + old.Def.Name + " with " + found[0].Def.Name + " from their deck (Temperamental Weather).", found[0].Id, me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Pokémon

        /// <summary>Darkness Chant: as many damage counters on the Defending Pokémon as Pokémon cards in your discard pile, no more than 6.</summary>
        private sealed class DarknessChantMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Chant") return false;
                int n = Math.Min(6, ctx.Me.Discard.Count(c => c.Def.IsPokemon));
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>
        /// Primal Pull (Poké-Body): while Claydol is your Active Pokémon, each player's Evolved Pokémon pay Colorless more for
        /// their attacks. Muddy Eye: 10 for each basic Energy card on Claydol and the Defending Pokémon.
        /// </summary>
        private sealed class ClaydolPrimalPull : Mon
        {
            private static readonly Mon Eye = new CardsASR1.TimesMon("Muddy Eye", c => c.Pokemon.Energy.Count(e => e.Def.IsBasicEnergy) + (c.Defending == null ? 0 : c.Defending.Energy.Count(e => e.Def.IsBasicEnergy)));

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon != null && src.IsActive(src.Pokemon) && CardsE2.IsEvolved(mon)) cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Eye.HandleAttack(ctx, atk); }
        }

        /// <summary>Flutter Trick: flip a coin; heads, look at the opponent's hand and choose a card for them to discard.</summary>
        private sealed class FlutterTrickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flutter Trick") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Opp.Hand.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Opp.Hand.ToList(), 1, 1, "Flutter Trick: choose a card from your opponent's hand to discard")[0];
                ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
                return true;
            }
        }

        /// <summary>Dark Provide: you may attach a Grass or Darkness Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class DarkProvideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Provide") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Hand.Where(c => c.Def.IsEnergy && (c.Def.EnergyProvides == EnergyType.Grass || c.Def.EnergyProvides == EnergyType.Darkness)).ToList();
                if (cards.Count > 0) ctx.AttachCardsAnyWay(cards, 1, (c, m) => true, "Dark Provide: attach a Grass or Darkness Energy card");
                return true;
            }
        }

        /// <summary>Mass Destruction: both Electrode and the Defending Pokémon are Knocked Out; nothing if Electrode has Special Energy attached.</summary>
        private sealed class MassDestructionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mass Destruction") return false;
                var me = ctx.Pokemon;
                if (me.Energy.Any(e => !e.Def.IsBasicEnergy)) return Nothing(ctx);
                var defending = ctx.Defending;
                if (defending != null) ctx.Engine.KnockOut(defending, "Mass Destruction");
                if (ctx.InPlay(me)) ctx.Engine.KnockOut(me, "Mass Destruction");
                return true;
            }
        }

        /// <summary>Extra Draw: if the opponent has a Pokémon-ex in play, up to 2 Grass Energy cards from your deck onto Heracross.</summary>
        private sealed class ExtraDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extra Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.OpponentPokemon().Any(m => m.Def.IsOldEx) || ctx.Me.Deck.Count == 0) return true;
                foreach (var card in ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Grass), 2, "Extra Draw: choose up to 2 Grass Energy cards"))
                    if (!ctx.InPlay(me) || !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Make a Wish: a card that evolves from 1 of your Pokémon, from your deck onto it (no timing limits, like Wally's Training); if you do, 1 damage counter on Jirachi.</summary>
        private sealed class MakeAWishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Make a Wish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var targets = ctx.MyPokemon(m => !CardsEVS1.EvolveBlocked(ctx, m) && ctx.Me.Deck.Any(c => CardsEVS1.EvolvesOnto(ctx, m, c.Def)));
                if (targets.Count == 0)
                {
                    if (ctx.Me.Deck.Count > 0) ctx.ShuffleMyDeck();
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Make a Wish: evolve which of your Pokémon?");
                if (target != null && CardsEVS1.EvolveFromDeck(ctx, target, "Make a Wish") && ctx.InPlay(me)) ctx.PlaceCounters(me, 1);
                return true;
            }
        }

        /// <summary>
        /// Overzealous (Poké-Body): while the opponent has a Pokémon-ex in play, Machamp's attacks do 30 more to the Defending Pokémon.
        /// Brick Smash: not affected by Weakness, Resistance or effects on the Defending Pokémon. Cross Chop: heads, 30 more.
        /// </summary>
        private sealed class MachampOverzealous : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsMA.SwiftMon("Brick Smash"), new FlipBonusMon("Cross Chop", 30));

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !src.OpponentPokemon().Any(m => m.Def.IsOldEx)) return damage;
                return damage + 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Metal Juncture (Poké-Power): as often as you like during your turn, move a Metal Energy card from 1 of your Benched Pokémon
        /// to your Active Pokémon; not with a Special Condition. Squared Attack: 4 coins, 30 each.
        /// </summary>
        private sealed class MetagrossMetalJuncture : Mon
        {
            private const string Power = "Metal Juncture";
            private static readonly Mon Squared = new CoinsMon("Squared Attack", 4);

            private static bool MetalCard(CardInstance e) { return e.Def.IsEnergy && e.Def.EnergyProvides == EnergyType.Metal; }

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyBench().Where(m => m.Energy.Any(MetalCard)).ToList(); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Active != null && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => Donors(ctx).Count > 0 && ctx.Me.Active != null, offerDone =>
                {
                    var active = ctx.Me.Active;
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a Metal Energy card from which Benched Pokémon to " + active.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var cards = from.Energy.Where(MetalCard).ToList();
                    return ctx.Engine.MoveEnergy(from, active, cards[0]);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Squared.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Healing Shower (Poké-Power): when you play Milotic from your hand to evolve, you may remove all damage counters from every
        /// Pokémon in play (both players') except Pokémon-ex. Distorted Wave: 3 damage counters come off the Defending Pokémon first.
        /// </summary>
        private sealed class MiloticHealingShower : Mon
        {
            private const string Power = "Healing Shower";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(m => m.DamageCounters > 0 && !m.Def.IsOldEx).ToList();
                if (!TriggerOnce(ctx, Power, hurt.Count > 0, Power + ": remove all damage counters from every Pokémon in play except Pokémon-ex?")) return;
                foreach (var mon in hurt) if (ctx.InPlay(mon)) ctx.Heal(mon, mon.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Distorted Wave") return false;
                var defending = ctx.Defending;
                if (defending != null && defending.DamageCounters > 0) ctx.Heal(defending, 30);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Crust (Poké-Body): the opponent's Basic Pokémon's attacks do 30 less damage to Pinsir (after Weakness and Resistance). Sonicboom.</summary>
        private sealed class PinsirCrust : Mon
        {
            private static readonly Mon Boom = new NoWeaknessResistanceMon("Sonicboom");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return info.Attacker.Def.IsBasicPokemon ? Math.Max(0, damage - 30) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Boom.HandleAttack(ctx, atk); }
        }

        /// <summary>Push Aside: look at the opponent's hand; a Basic Pokémon or Evolution card you choose goes to the bottom of their deck.</summary>
        private sealed class PushAsideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Push Aside") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Push Aside: your opponent's hand", ctx.Source);
                var pokemon = hand.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return true;
                var pick = ctx.ChooseCards(pokemon, 1, 1, "Push Aside: put which card on the bottom of your opponent's deck?")[0];
                if (ctx.Opp.Hand.Remove(pick))
                {
                    ctx.Opp.Deck.Add(pick);
                    ctx.Log("Push Aside: " + pick.Def.Name + " was put on the bottom of " + ctx.Engine.Possessive(ctx.Opponent) + " deck.");
                }
                return true;
            }
        }

        /// <summary>
        /// Crush Draw (Poké-Power): once during your turn, reveal the top card of your deck; a basic Energy card is attached to 1 of
        /// your Pokémon, anything else stays on top; not with a Special Condition. Sheer Cold: heads, the Defending Pokémon can't attack.
        /// </summary>
        private sealed class WalreinCrushDraw : Mon
        {
            private const string Power = "Crush Draw";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.Me.Deck[0];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { top }, Power, Power + ": the top card of your deck", ctx.Source);
                ctx.Log(Power + ": the top card of " + ctx.Engine.Possessive(ctx.Player) + " deck is " + top.Def.Name + ".");
                if (!top.Def.IsBasicEnergy) return;
                var mine = ctx.MyPokemon();
                var target = ctx.ChoosePokemon(mine, Power + ": attach " + top.Def.Name + " to which Pokémon?");
                if (target != null && ctx.Me.Deck.Remove(top)) ctx.Engine.AttachEnergyCard(top, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sheer Cold") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Sheer Cold)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>
        /// Heal Dance (Poké-Power): once during your turn, remove 2 damage counters from 1 of your Pokémon; only 1 Heal Dance each
        /// turn; not with a Special Condition. Miracle Powder: heads, a Special Condition you choose. Solarbeam.
        /// </summary>
        private sealed class BellossomHealDance : Mon
        {
            private const string Power = "Heal Dance";

            protected override string Activated { get { return Power; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, Power + ": remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miracle Powder") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                var choices = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
                int pick = ctx.ChooseOption("Miracle Powder: choose a Special Condition", choices.Select(c => c.ToString()).ToArray());
                ctx.ApplyCondition(defending, choices[pick < 0 || pick >= choices.Length ? 0 : pick]);
                return true;
            }
        }

        /// <summary>During the attacker's next turn, attacks that damage this Pokémon do N more (after Weakness and Resistance): Silver Wind.</summary>
        internal sealed class ExtraDamageNextTurnEffect : ContinuousEffect
        {
            private readonly int amount;

            public ExtraDamageNextTurnEffect(string name, int owner, int targetUid, int currentTurn, int amount)
                : base(name, owner, targetUid, currentTurn + 2, true)
            {
                this.amount = amount;
                ActiveFromTurn = currentTurn + 2;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || damage <= 0) return damage;
                return engine.CurrentSourceKind == EffectSourceKind.Attack ? damage + amount : damage;
            }
        }

        /// <summary>Silver Wind: during your next turn, attacks that damage the Defending Pokémon do 30 more (after Weakness and Resistance).</summary>
        private sealed class SilverWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silver Wind") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new ExtraDamageNextTurnEffect(defending.Name + " takes 30 more damage from attacks during " + ctx.Engine.Possessive(ctx.Player) + " next turn (Silver Wind)",
                        ctx.Player, defending.Uid, ctx.Turn, 30));
                return true;
            }
        }

        /// <summary>Scattered Shower: shuffle your hand into your deck and draw up to 5 cards.</summary>
        private sealed class ScatteredShowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scattered Shower") return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                int max = Math.Min(5, ctx.Me.Deck.Count);
                if (max > 0) ctx.Draw(max <= 1 ? max : ctx.AskNumber(0, max, "Scattered Shower: draw how many cards (up to " + max + ")?"));
                return true;
            }
        }

        /// <summary>Deep Sleep (Poké-Body): while Relicanth is your Active Pokémon, every Asleep Pokémon's owner flips 2 coins between turns; it wakes only on 2 heads. Yawn.</summary>
        private sealed class RelicanthDeepSleep : Mon
        {
            private static readonly Mon Yawn = new CardsPAR2.FlipConditionMon("Yawn", SpecialCondition.Asleep);

            public override int ModifySleepCoins(EffectContext src, PokemonInPlay mon, int coins)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) ? Math.Max(coins, 2) : coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Yawn.HandleAttack(ctx, atk); }
        }

        /// <summary>Sunshine: a Stadium card from your discard pile into your hand.</summary>
        private sealed class SunshineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sunshine") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var stadiums = ctx.Me.Discard.Where(c => c.Def.IsStadiumCard).ToList();
                if (stadiums.Count == 0) return true;
                ctx.DiscardToHand(stadiums.Count == 1 ? stadiums : ctx.ChooseCards(stadiums, 1, 1, "Sunshine: put which Stadium card into your hand?"));
                return true;
            }
        }

        /// <summary>Fruition: move an Energy card from Tropius to another of your Pokémon and remove 4 damage counters from that Pokémon.</summary>
        private sealed class FruitionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fruition") return false;
                var me = ctx.Pokemon;
                var others = ctx.MyPokemon(m => m != me);
                if (me.Energy.Count == 0 || others.Count == 0) return true;
                var card = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Fruition: move which Energy card?")[0];
                var to = ctx.ChoosePokemon(others, "Fruition: move it to which Pokémon?");
                if (to != null && ctx.Engine.MoveEnergy(me, to, card) && to.DamageCounters > 0) ctx.Heal(to, 40);
                return true;
            }
        }

        /// <summary>Mark of Antiquity (Poké-Body): while this Pokémon is your Active Pokémon, each player's [the other two] can't attack.</summary>
        private sealed class MarkOfAntiquityMon : Mon
        {
            private static readonly string[] Trio = { "Groudon ex", "Kyogre ex", "Rayquaza ex" };
            private readonly string self;
            private readonly Mon attacks;

            public MarkOfAntiquityMon(string self, Mon attacks)
            {
                this.self = self;
                this.attacks = attacks;
            }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                return me != null && mon != null && src.IsActive(me) && mon.Name != self && Trio.Contains(mon.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Crushing Mantle: discard any number of Energy cards from your hand; 10 more for each.</summary>
        private sealed class CrushingMantleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Mantle") return false;
                int n = 0;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (!CardsBS1.CopyingWithMetronome && energy.Count > 0)
                    foreach (var card in ctx.ChooseCards(energy, 0, energy.Count, "Crushing Mantle: discard any number of Energy cards from your hand (10 more damage each)"))
                        if (ctx.Me.Hand.Contains(card))
                        {
                            ctx.Engine.DiscardFromHand(ctx.Player, card);
                            n++;
                        }
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        /// <summary>Super Tidal Wave: you may show your hand; 10 more for each Energy card in it, then those Energy cards are shuffled into your deck.</summary>
        private sealed class SuperTidalWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Tidal Wave") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                bool show = energy.Count > 0 && ctx.AskYesNo("Super Tidal Wave: show your hand for 10 more damage per Energy card in it (" + energy.Count + ")?");
                if (show) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " showed their hand: " + string.Join(", ", ctx.Me.Hand.Select(c => c.Def.Name).ToArray()) + ".");
                ctx.DamageDefending(atk.Damage + (show ? 10 * energy.Count : 0));
                if (!show) return true;
                foreach (var card in energy)
                    if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Extra Comet Punch: 50 more if Metagross ex used Extra Comet Punch during your last turn.</summary>
        private sealed class ExtraCometPunchMon : Mon
        {
            private const string Punch = "Extra Comet Punch";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Punch) return false;
                var me = ctx.Pokemon;
                int turn;
                bool boosted = me.Markers.TryGetValue(Punch, out turn) && turn == ctx.MyLastTurn;
                ctx.DamageDefending(atk.Damage + (boosted ? 50 : 0));
                if (ctx.InPlay(me)) me.Markers[Punch] = ctx.Turn;
                return true;
            }
        }

        /// <summary>Intense Glare: switch in 1 of the opponent's Benched Pokémon; the new Defending Pokémon is Burned and Confused (nothing without a Bench, as Luring Flame).</summary>
        private sealed class IntenseGlareMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Intense Glare") return false;
                if (ctx.GustTargets().Count == 0) return Nothing(ctx);
                var old = ctx.Defending;
                ctx.GustOpponent("Intense Glare: choose the new Defending Pokémon");
                var now = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (now == null || now == old || !TefPokemon.StillDefending(ctx, now)) return true;
                ctx.ApplyCondition(now, SpecialCondition.Burned);
                ctx.ApplyCondition(now, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Crystal Body (Poké-Body): no effects of attacks other than damage reach Regice ex. Freeze Lock: heads, the opponent can't
        /// attach Energy cards from their hand to the Defending Pokémon during their next turn.
        /// </summary>
        private sealed class RegiceExCrystalBody : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Freeze Lock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsCRI.NoEnergyFromHandEffect("No Energy from the hand onto " + defending.Name + " (Freeze Lock)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Tonnage: you may do 20 more; if you do, Regirock ex does 30 damage to itself.</summary>
        private sealed class TonnageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tonnage") return false;
                bool more = ctx.AskYesNo("Tonnage: do 20 more damage and 30 damage to Regirock ex?");
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                if (more && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Steel Wave: the damage, and 20 to each of the opponent's Benched Pokémon of the Defending Pokémon's type.</summary>
        private sealed class SteelWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Steel Wave") return false;
                var defending = ctx.Defending;
                var types = defending == null ? new List<EnergyType>() : ctx.Engine.TypesOf(defending);
                var hit = ctx.OpponentBench().Where(m => ctx.Engine.TypesOf(m).Any(types.Contains)).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in hit) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>
        /// Block Dust (Poké-Body): while Vileplume ex is your Active Pokémon, the opponent can't play Trainer cards other than
        /// Supporters from their hand. Special Formula: heads, Asleep and Poisoned; tails, Confused.
        /// </summary>
        private sealed class VileplumeExBlockDust : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                return me != null && player != src.Player && card != null && card.Def.IsTrainer && !card.Def.IsSupporter && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Formula") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads)
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                else ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
