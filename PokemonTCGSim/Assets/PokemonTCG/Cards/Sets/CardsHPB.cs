using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Holon Phantoms Uncommon and Common Pokémon (HP 18-84, 111; v170). Aron 58, Corphish 62, Horsea 66, Magikarp 69, Pidgey 77 and
    /// Torchic 83 have no attack text. Mew 111 counts the amount of Energy (not Energy cards, unlike Expedition's), so it's its own card.
    /// Holon's Castform is a Holon's Pokémon (every type, 2 at a time). Compendium: Wreck discards the Stadium after the damage.
    /// </summary>
    public static class CardsHPB
    {
        private static readonly string[] BellossomLine = { "Oddish", "Gloom", "Vileplume", "Vileplume ex", "Bellossom" };
        private const SpecialCondition AllConditions = SpecialCondition.Asleep | SpecialCondition.Burned | SpecialCondition.Confused | SpecialCondition.Paralyzed | SpecialCondition.Poisoned;

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HP-18", new ComboMon(new ForesightMon(5), new Cards30CC.BenchHitMon("Overrun", 20)));
            r.RegisterByKey("HP-19", new BellossomFellowship());
            r.RegisterByKey("HP-20", new BlazeKickMon());
            r.RegisterByKey("HP-21", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("HP-22", new ComboMon(new FlipBonusMon("Aqua Wave", 20), new DragonbreathMon()));
            r.RegisterByKey("HP-23", new ComboMon(new CardsCES1.FeintAttackMon("Feint Attack", 10), new CardsSS.SurpriseMon()));
            r.RegisterByKey("HP-24", new ComboMon(new CardsTEU1.AmnesiaMon("Psychic Erase"), new CardsMA.SwiftMon("Swift")));
            r.RegisterByKey("HP-25", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsFST1.TwoBenchedMon("Rock Slide", 10)));
            r.RegisterByKey("HP-26", new ComboMon(new OutrageMon(), new CardsASC.SelfDiscardTypeMon("Flamethrower", EnergyType.Fire, 1)));
            r.RegisterByKey("HP-27", new CardsDX.ConditionImmuneMon(AllConditions, new CardsPAR2.FlipConditionMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("HP-28", new CardsDX.ConditionImmuneMon(AllConditions, new CardsFST1.TwoBenchedMon("Rock Slide", 10)));
            r.RegisterByKey("HP-29", new CardsDX.ConditionImmuneMon(AllConditions, new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("HP-30", new CardsDX.TypeLiftMon("Aqua Lift", EnergyType.Water, new FlipBonusMon("Knock Away", 10)));
            r.RegisterByKey("HP-31", new ComboMon(new ParalyzeFlipMon("Paralyzing Gaze"), new KnockOffMon("Knock Off", false)));
            r.RegisterByKey("HP-32", new CardsPAR2.FlipConditionMon("Poison Fang", SpecialCondition.Poisoned));
            r.RegisterByKey("HP-33", new CardsASR1.TimesMon("Grind", c => c.EnergyCount(c.Pokemon)));
            r.RegisterByKey("HP-34", new FuryCutterMon());
            r.RegisterByKey("HP-35", new AerodactylPrimalLight());
            r.RegisterByKey("HP-36", new ComboMon(new CardsRSB.MagnitudeMon(), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("HP-37", new ChimechoDeltaSupport());
            r.RegisterByKey("HP-38", new ComboMon(new WideLaserMon(), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("HP-39", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("HP-40", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Hurl"), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("HP-41", new ComboMon(new CardsASR1.PlusMon("Delta Circle", 10, c => c.MyPokemon(CardsDS.Delta).Count), new CardsDX.SplitBombMon()));
            r.RegisterByKey("HP-42", new NoRetreatMon("Acid"));
            r.RegisterByKey("HP-43", new GolduckDeltaBlock());
            r.RegisterByKey("HP-44", new CardsDS.HolonMon(true, new DeltaDrawMon()));
            r.RegisterByKey("HP-45", new CardsPFL.FlipUntilTailsTimesMon("Iron Head"));
            r.RegisterByKey("HP-46", new ComboMon(new PlasmaMon(), new FlipBonusMon("Quick Blow", 20)));
            r.RegisterByKey("HP-47", new ComboMon(new SweetScentMon(), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("HP-48", new PersianMon());
            r.RegisterByKey("HP-49", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("HP-50", new PrimeapeMon());
            r.RegisterByKey("HP-51", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("HP-52", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("HP-53", new ComboMon(new KnockOffMon("Brush Aside", true),
                new CardsFST2.ConditionBonusMon("Swift Turn", 30, c => c.Defending != null && (c.Engine.ResistanceOf(c.Defending) == EnergyType.Fighting || c.Engine.SecondResistanceOf(c.Defending) == EnergyType.Fighting))));
            r.RegisterByKey("HP-54", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("HP-55", new ComboMon(new CardsFLF1.NoResistanceMon("Pebble Hurl"), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("HP-56", new ComboMon(new BenchSearchAttackMon("Call for Friends", 2), new CardsASR1.TimesMon("Retaliate", c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("HP-57", new FlipBonusMon("Rising Lunge", 20));
            r.RegisterByKey("HP-58", new PlainMon());
            r.RegisterByKey("HP-59", new CoinsMon("Doubleslap", 2));
            r.RegisterByKey("HP-60", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("HP-61", new CardsFST1.FlipRecoilMon("Reckless Charge", 10));
            r.RegisterByKey("HP-62", new PlainMon());
            r.RegisterByKey("HP-63", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("HP-64", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("HP-65", new CardsMA.BenchOnlyMon("Pebble Throw", 20));
            r.RegisterByKey("HP-66", new PlainMon());
            r.RegisterByKey("HP-67", new ConditionAttackMon("Eerie Light", SpecialCondition.Confused));
            r.RegisterByKey("HP-68", new ConditionAttackMon("Poison Tentacles", SpecialCondition.Poisoned));
            r.RegisterByKey("HP-69", new PlainMon());
            r.RegisterByKey("HP-70", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("HP-71", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("HP-72", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("HP-73", new CardsTWM1.HealSelfMon("Blot", 20));
            r.RegisterByKey("HP-74", new ComboMon(new DrawAttackMon("Collect", 3), new CardsSSB.AnyOpponentTextMon("Water Arrow", 20)));
            r.RegisterByKey("HP-75", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("HP-76", new CardsSS.BabyEvolutionMon("Pikachu", new PasteMon()));
            r.RegisterByKey("HP-77", new PlainMon());
            r.RegisterByKey("HP-78", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new FlipBonusMon("Quick Attack", 10)));
            r.RegisterByKey("HP-79", new FlipBonusMon("Steel Headbutt", 10));
            r.RegisterByKey("HP-80", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("HP-81", new CardsTEU1.AmnesiaMon("Disable"));
            r.RegisterByKey("HP-82", new ConditionAttackMon("Super Hypno Wave", SpecialCondition.Asleep));
            r.RegisterByKey("HP-83", new PlainMon());
            r.RegisterByKey("HP-84", new NoRetreatMon("Big Bite"));
            r.RegisterByKey("HP-111", new SuperPsywaveMon());
        }

        // ================================================================== Attacks

        /// <summary>Foresight: look at the top N cards of either player's deck and put them back in any order.</summary>
        internal sealed class ForesightMon : Mon
        {
            private readonly int count;

            public ForesightMon(int count) { this.count = count; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foresight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int pick = ctx.ChooseOption("Foresight: look at the top " + count + " cards of which deck?", "Your deck", "Your opponent's deck");
                CardsE2.ReorderTop(ctx, pick == 0 ? ctx.Player : ctx.Opponent, count, "Foresight");
                return true;
            }
        }

        /// <summary>Blaze Kick: flip a coin; heads, 20 more; tails, the Defending Pokémon is now Burned.</summary>
        private sealed class BlazeKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze Kick") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Dragonbreath: flip a coin; tails, nothing; heads, the damage and the Defending Pokémon is now Paralyzed.</summary>
        private sealed class DragonbreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragonbreath") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Outrage: flip a coin; tails, nothing; heads, 10 more per damage counter on this Pokémon.</summary>
        private sealed class OutrageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Outrage") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage + 10 * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>
        /// "Choose 1 card from your opponent's hand without looking and discard it" (Knock Off; Brush Aside only with a Holon Energy
        /// card on this Pokémon).
        /// </summary>
        private sealed class KnockOffMon : Mon
        {
            private readonly string attack;
            private readonly bool needsHolonEnergy;

            public KnockOffMon(string attack, bool needsHolonEnergy)
            {
                this.attack = attack;
                this.needsHolonEnergy = needsHolonEnergy;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool works = !needsHolonEnergy || CardsDS.HolonEnergyCount(ctx.Pokemon) > 0;
                ctx.DamageDefending(atk.Damage);
                var card = works ? CardsHP.RandomOpponentCard(ctx) : null;
                if (card != null) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Fury Cutter: flip 3 coins; 1 heads, 10 more; 2 heads, 20 more; 3 heads, 40 more.</summary>
        private sealed class FuryCutterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage + (heads == 3 ? 40 : heads * 10));
                return true;
            }
        }

        /// <summary>Wide Laser: 10 damage to each of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class WideLaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wide Laser") return false;
                foreach (var mon in ctx.OpponentPokemon().ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Delta Draw: draw up to 1 card for each of your Pokémon in play with δ.</summary>
        private sealed class DeltaDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Draw") return false;
                int n = Math.Min(ctx.MyPokemon(CardsDS.Delta).Count, ctx.Me.Deck.Count);
                if (n > 0) ctx.Draw(ctx.AskNumber(0, n, "Delta Draw: draw how many cards (up to " + n + ")?"));
                return true;
            }
        }

        /// <summary>Plasma: a Lightning Energy card from your discard pile onto this Pokémon.</summary>
        private sealed class PlasmaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plasma") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                var cards = ctx.BasicEnergyInDiscard(d => CardsDS.BasicOf(d, EnergyType.Lightning));
                if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Plasma: attach which Lightning Energy card?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Sweet Scent: 2 damage counters off 1 of your Pokémon.</summary>
        private sealed class SweetScentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Scent") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Sweet Scent: remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Scratch and Draw: with a Holon Stadium in play, draw 3 cards. Deceive: the opponent chooses 1 of their Pokémon; 4 damage counters on it.</summary>
        private sealed class PersianMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Scratch and Draw")
                {
                    bool holon = CardsHP.HolonStadium(ctx);
                    ctx.DamageDefending(atk.Damage);
                    if (holon) ctx.Draw(3);
                    return true;
                }
                if (atk.Name != "Deceive") return false;
                var theirs = ctx.OpponentPokemon();
                if (theirs.Count == 0) return true;
                var target = ctx.Engine.ChoosePokemon(ctx.Opponent, theirs, "Deceive: choose 1 of your Pokémon to get 4 damage counters", ctx.Source);
                if (target != null) ctx.PlaceCounters(target, 4);
                return true;
            }
        }

        /// <summary>
        /// Wreck: with a Stadium in play, 30 more, then discard that Stadium (after the damage: Compendium). Flames of Rage: discard 2
        /// Energy from this Pokémon; 20 more per damage counter on it.
        /// </summary>
        private sealed class PrimeapeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Wreck")
                {
                    bool stadium = ctx.State.Stadium != null;
                    ctx.DamageDefending(atk.Damage + (stadium ? 30 : 0));
                    if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                    return true;
                }
                if (atk.Name != "Flames of Rage") return false;
                if (!CardsBS1.CopyingWithMetronome && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, Math.Min(2, me.Energy.Count), "Flames of Rage: discard 2 Energy");
                ctx.DamageDefending(atk.Damage + 20 * me.DamageCounters);
                return true;
            }
        }

        /// <summary>Paste: an Energy card from your discard pile onto 1 of your δ Pokémon.</summary>
        private sealed class PasteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paste") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                var targets = ctx.MyPokemon(CardsDS.Delta);
                if (cards.Count == 0 || targets.Count == 0) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Paste: attach which Energy card?")[0];
                var target = ctx.ChoosePokemon(targets, "Paste: attach " + card.Def.Name + " to which of your δ Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Super Psywave: damage counters on 1 of the opponent's Pokémon equal to the amount of Energy attached to it.</summary>
        private sealed class SuperPsywaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Psywave") return false;
                var theirs = ctx.OpponentPokemon();
                if (theirs.Count == 0) return true;
                var target = ctx.ChoosePokemon(theirs, "Super Psywave: choose 1 of your opponent's Pokémon");
                int n = target == null ? 0 : ctx.EnergyCount(target);
                if (n > 0) ctx.PlaceCounters(target, n);
                return true;
            }
        }

        // ================================================================== Pokémon with Poké-Powers and Poké-Bodies

        /// <summary>Fellowship (Poké-Body): Bellossom can use the attacks of your Oddish, Gloom, Vileplume, Vileplume ex and other Bellossom in play. Aqua Flower.</summary>
        private sealed class BellossomFellowship : Mon
        {
            private static readonly Mon Flower = new CardsBCR1.NoWeaknessNextTurnMon("Aqua Flower");

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                var seen = new HashSet<string>();
                foreach (var other in src.Me.InPlay())
                {
                    if (other == src.Pokemon || !BellossomLine.Contains(other.Name) || !seen.Add(other.Def.BaseKey)) continue;
                    for (int i = 0; i < other.Def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(other.Def, i));
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flower.HandleAttack(ctx, atk); }
        }

        /// <summary>Primal Light (Poké-Power): once during your turn, a basic Energy card from your deck into your hand. Granite Head.</summary>
        private sealed class AerodactylPrimalLight : Mon
        {
            private const string Power = "Primal Light";
            private static readonly Mon Granite = new ShieldAttackMon("Granite Head", 10);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(d => d.IsBasicEnergy, 1, Power + ": choose a basic Energy card"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Granite.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Support (Poké-Power): once during your turn, with a Holon Supporter in play, a basic Energy or δ Rainbow Energy card from your discard pile into your hand.</summary>
        private sealed class ChimechoDeltaSupport : Mon
        {
            private const string Power = "Delta Support";

            private static List<CardInstance> Cards(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy || CardsHPT.IsDeltaRainbow(c.Def)).ToList(); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var supporter = ctx.Me.SupporterThisTurn;
                return CardsDS.PowerReady(ctx) && supporter != null && supporter.Name.Contains("Holon") && Cards(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = Cards(ctx);
                if (cards.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": put which Energy card into your hand?");
                ctx.Engine.ShowCards(ctx.Opponent, card, Power, Power + ": the card your opponent took", ctx.Source);
                ctx.DiscardToHand(card);
            }
        }

        /// <summary>
        /// Delta Block (Poké-Body): with a Holon Stadium in play, the opponent can't play Stadium cards from their hand. Mind Play: a
        /// random card from the opponent's hand; a Trainer card is discarded for 30 more, anything else goes back.
        /// </summary>
        private sealed class GolduckDeltaBlock : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return player != src.Player && card != null && card.Def.IsStadium && CardsHP.HolonStadium(src);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Play") return false;
                var card = CardsHP.RandomOpponentCard(ctx);
                bool trainer = card != null && card.Def.IsTrainer;
                if (card != null) ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { card }, "Mind Play", "Mind Play: the card you chose", ctx.Source);
                ctx.DamageDefending(atk.Damage + (trainer ? 30 : 0));
                if (trainer && ctx.Opp.Hand.Contains(card)) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }
    }
}
