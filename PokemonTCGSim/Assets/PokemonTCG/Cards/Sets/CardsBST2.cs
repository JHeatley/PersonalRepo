using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Battle Styles (BST, 2021): Pokémon 66-120 (Fighting, Darkness, Metal, Colorless), the Trainers and the Special
    /// Energy. Regulation mark E: Expanded only.
    /// Already in the pool: Falinks, the four Urshifu V and VMAX, Houndoom, Bronzong, Corviknight V and VMAX, Stoutland V,
    /// Energy Recycler, Exp. Share, Fan of Waves, both Style Mustards, Sordward &amp; Shielbert and Urn of Vitality.
    /// Pokémon whose attacks only do their printed damage need no script (Cubone, Gligar, Timburr, Gurdurr, Silicobra,
    /// Houndour, Bronzor, Spearow, Fearow, Lickitung, Glameow).
    /// Your ruling (v60): Aegislash's Stance Change is once during your turn for that Pokémon, so the Aegislash it brings in
    /// can't use it again that turn.
    /// </summary>
    public static class CardsBST2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("BST-66", new TailsNothingMon("Focus Fist"));
            r.RegisterByKey("BST-67", new ComboMon(new CardsCRE2.FieldCrushMon(), new CardsASR1.TimesMon("Steamin' Mad Strike", DamagedBenched)));
            r.RegisterByKey("BST-68", new RecoilMon("Rocky Tackle", 60));
            r.RegisterByKey("BST-70", new CardsFST1.TakeLessMon(30, new CoinsMon("Bonemerang", 2)));
            r.RegisterByKey("BST-72", new CardsWHT.FlipMoreMon("Acrobatics", 2, 40));
            r.RegisterByKey("BST-75", new HammerPressureMon());
            r.RegisterByKey("BST-76", new CoinsMon("Double Stab", 2));
            r.RegisterByKey("BST-77", new SpinningWhipMon());
            r.RegisterByKey("BST-78", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("BST-79", new RecoilMon("Wild Tackle", 10));
            r.RegisterByKey("BST-80", new ComboMon(new CardsASC.FlipPerEnergyMon("Coal Cannon"), new RecoilMon("Wild Tackle", 50)));
            r.RegisterByKey("BST-82", new BigSandCannonMon());
            r.RegisterByKey("BST-84", new ComboMon(new CardsFST2.ConditionBonusMon("Land's Pulse", 30, StadiumInPlay), new LockAfterAttackMon("Giga Hammer", true)));
            // Darkness
            r.RegisterByKey("BST-89", new SwitchSelfMon("Hide in Shadows", false));
            r.RegisterByKey("BST-90", new EvolveDrawMon("Discreet Draw", 2, new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("BST-91", new EvolveDrawMon("Drastic Draw", 3, null));
            r.RegisterByKey("BST-92", new ComboMon(new SplatteringPoisonMon(), new CardsFST2.ConditionBonusMon("Unhinged Hammer", 120, SelfHasCondition)));
            r.RegisterByKey("BST-93", new CardsASR1.RevealHandAttackMon("Scout"));
            r.RegisterByKey("BST-94", new HonchkrowBST());
            r.RegisterByKey("BST-97", new ComboMon(new CragalancheMon(), new CardsBRS1.MillSelfMon("Single Strike Crush", 4)));
            r.RegisterByKey("BST-98", new ComboMon(new GatherFoodMon(), new CardsFST2.ConditionBonusMon("Hangry Tackle", 90, EmptyHand)));
            // Metal
            r.RegisterByKey("BST-99", new CoinsMon("Steel Swing", 2));
            r.RegisterByKey("BST-100", new ComboMon(new ParalyzeFlipMon("Powerful Vise"), new CardsCRZ1.PiercingMon("Piercing Strike")));
            r.RegisterByKey("BST-103", new SwordsDanceMon());
            r.RegisterByKey("BST-104", new CardsMEW1.DefendingTypeBonusMon("Steel Slice", EnergyType.Metal, 90));
            r.RegisterByKey("BST-105", new FlipBonusMon("Ambush", 30));
            r.RegisterByKey("BST-106", new CardsASC.FlipDiscardDefendingEnergyMon("Cut Down"));
            r.RegisterByKey("BST-107", new AegislashBST(new CardsASC.SelfDiscardTypeMon("Full Metal Blade", EnergyType.Metal, 2)));
            r.RegisterByKey("BST-108", new AegislashBST(new GigatonBashMon("Gigaton Bash")));
            // Colorless
            r.RegisterByKey("BST-114", new ComboMon(new SelicktMon(), new CardsASC.PushDownMon("Pitch")));
            r.RegisterByKey("BST-116", new CaturdayMon());
            r.RegisterByKey("BST-118", new BouffalantBST());
            r.RegisterByKey("BST-119", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new CardsMEG.FlipUntilTailsBonusMon("Spiral Rush", 30)));
            r.RegisterByKey("BST-120", new ComboMon(new DrawAttackMon("Collect", 2), new CardsASR1.TimesMon("Hand Kinesis", HandSize)));

            // Trainers
            r.RegisterByName("Bruno", new Bruno());
            r.RegisterByName("Camping Gear", new CampingGear());
            r.RegisterByName("Cheryl", new Cheryl());
            r.RegisterByName("Escape Rope", new EscapeRope());
            r.RegisterByName("Korrina's Focus", new KorrinasFocus());
            r.RegisterByName("Level Ball", new LevelBall());
            r.RegisterByName("Phoebe", new Phoebe());
            r.RegisterByName("Rapid Strike Scroll of Swirls", new BstScroll(false));
            r.RegisterByName("Single Strike Scroll of Scorn", new BstScroll(true));
            r.RegisterByName("Tool Jammer", new ToolJammer());
            r.RegisterByName("Tower of Darkness", new TowerOfDarkness());
            r.RegisterByName("Tower of Waters", new TowerOfWaters());
            // Energy
            r.RegisterByName("Rapid Strike Energy", new RapidStrikeEnergy());
            r.RegisterByName("Single Strike Energy", new SingleStrikeEnergy());
        }

        // ================================================================== Filters and counters

        private static bool StadiumInPlay(EffectContext ctx) { return ctx.State.Stadium != null; }
        private static bool SelfHasCondition(EffectContext ctx) { return ctx.Pokemon.Conditions != SpecialCondition.None; }
        private static bool EmptyHand(EffectContext ctx) { return ctx.Me.Hand.Count == 0; }
        private static int HandSize(EffectContext ctx) { return ctx.Me.Hand.Count; }
        private static int DamagedBenched(EffectContext ctx) { return ctx.Me.Bench.Count(m => m.DamageCounters > 0); }

        // ================================================================== Fighting

        /// <summary>Hammer Pressure: if the Defending Pokémon is an Evolution Pokémon, it can't attack during the opponent's next turn.</summary>
        private sealed class HammerPressureMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hammer Pressure") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !defending.Def.IsEvolution || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Hammer Pressure)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Spinning Whip: the opponent's Active Pokémon is now Confused; shuffle this Pokémon and all attached cards into your deck.</summary>
        private sealed class SpinningWhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spinning Whip") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Big Sand Cannon: discard the top 6 cards of your deck; 60 damage for each Fighting Energy card discarded.</summary>
        private sealed class BigSandCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Sand Cannon") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 6);
                int n = milled.Count(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fighting));
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                else ctx.Log("Big Sand Cannon: no Fighting Energy cards were discarded.");
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Discreet Draw / Drastic Draw: when you play this Pokémon from your hand to evolve 1 of your Pokémon, you may draw N cards.</summary>
        private sealed class EvolveDrawMon : Mon
        {
            private readonly string ability;
            private readonly int cards;
            private readonly Mon attacks;

            public EvolveDrawMon(string ability, int cards, Mon attacks)
            {
                this.ability = ability;
                this.cards = cards;
                this.attacks = attacks;
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, ability, ctx.Me.Deck.Count > 0, "Use " + ability + "? Draw " + cards + " cards.")) return;
                ctx.Draw(cards);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Splattering Poison: both Active Pokémon are now Poisoned.</summary>
        private sealed class SplatteringPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splattering Poison") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.Engine.ApplyCondition(me, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Insomnia: this Pokémon can't be Asleep. Voltage Dive: 80 more if the opponent's Active Pokémon has any Special Energy attached.</summary>
        private sealed class HonchkrowBST : Mon
        {
            private static readonly Mon VoltageDive = new CardsFST2.ConditionBonusMon("Voltage Dive", 80, DefendingHasSpecialEnergy);

            private static bool DefendingHasSpecialEnergy(EffectContext ctx)
            {
                return ctx.Defending != null && ctx.Defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
            }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon && (condition & SpecialCondition.Asleep) != 0;
            }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon != null && src.Pokemon.Has(SpecialCondition.Asleep)) src.Engine.RemoveCondition(src.Pokemon, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return VoltageDive.HandleAttack(ctx, atk); }
        }

        /// <summary>Cragalanche: discard the top 2 cards of the opponent's deck.</summary>
        private sealed class CragalancheMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cragalanche") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2);
                return true;
            }
        }

        /// <summary>Gather Food: put an Item card from your discard pile into your hand.</summary>
        private sealed class GatherFoodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Food") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(items, 1, 1, "Gather Food: put an Item card from your discard pile into your hand"));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Swords Dance: during your next turn, this Pokémon's Slash does 70 more damage. Slash: 10.</summary>
        private sealed class SwordsDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Swords Dance")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s Slash does 70 more damage during its next turn (Swords Dance)", ctx.Player, me.Uid, ctx.Turn + 2, "Slash", 70);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name != "Slash") return false;
                int uid = me.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(e => e.TargetUid == uid && e.AttackName == "Slash").Sum(e => e.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                return true;
            }
        }

        /// <summary>
        /// Stance Change: once during your turn, switch this Pokémon with an Aegislash in your hand; everything attached,
        /// damage counters, Special Conditions, turns in play and other effects stay on the new Pokémon (the same Pokémon
        /// in play, so it can't use Stance Change again that turn: your ruling).
        /// </summary>
        internal sealed class AegislashBST : Mon
        {
            private readonly Mon attacks;

            public AegislashBST(Mon attacks) { this.attacks = attacks; }

            private static List<CardInstance> Others(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Name == "Aegislash").ToList(); }

            protected override string Activated { get { return "Stance Change"; } }
            protected override bool AbilityReady(EffectContext ctx) { return Others(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var others = Others(ctx);
                if (others.Count == 0 || me == null) return;
                var pick = ctx.ChooseCards(others, 1, 1, "Stance Change: switch with which Aegislash from your hand?")[0];
                var old = me.Top;
                ctx.Me.Hand.Remove(pick);
                me.Stack[me.Stack.Count - 1] = pick;
                ctx.Me.Hand.Add(old);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " switched " + me.Name + " with an Aegislash from their hand (Stance Change).", pick.Id, me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Gigaton Bash: during the opponent's next turn, prevent all damage done to this Pokémon by attacks from Pokémon VMAX. Used for Gigaton Bash, Amazing Shield (the attack name is passed in).</summary>
        internal sealed class GigatonBashMon : Mon
        {
            private readonly string attack;

            public GigatonBashMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new VmaxShieldEffect(me.Name + " takes no damage from Pokémon VMAX's attacks during the opponent's next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class VmaxShieldEffect : ContinuousEffect
        {
            public VmaxShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || !info.AgainstOpponent) return damage;
                return info.Attacker.Def.IsVmax ? 0 : damage;
            }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Selickt: the opponent chooses to discard the top 3 cards of their deck or 3 cards from their hand. With no cards in hand
        /// they must discard from the deck (Compendium, 2021-03-18).
        /// </summary>
        private sealed class SelicktMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Selickt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var opp = ctx.Opp;
                if (opp.Deck.Count == 0 && opp.Hand.Count == 0) return true;
                var opts = new List<DecisionOption>
                {
                    new DecisionOption { Label = "Discard the top 3 cards of your deck", Index = 0 },
                    new DecisionOption { Label = "Discard 3 cards from your hand", Index = 1 }
                };
                int choice = opp.Hand.Count == 0 ? 0
                    : ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Selickt: discard the top 3 cards of your deck, or 3 cards from your hand?", opts, ctx.Source, false);
                if (choice == 0)
                {
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                    return true;
                }
                var hand = opp.Hand.ToList();
                int n = Math.Min(3, hand.Count);
                if (n == 0) return true;
                foreach (var card in ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, "Selickt: discard " + n + (n == 1 ? " card" : " cards") + " from your hand", ctx.Source))
                    ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Caturday: draw 3 cards; if you do, this Pokémon is now Asleep.</summary>
        private sealed class CaturdayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Caturday") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Draw(3) > 0 && ctx.InPlay(me) && ctx.IsActive(me)) ctx.Engine.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Sap Sipper: this Pokémon's attacks do 60 more damage to the opponent's Grass Pokémon (before Weakness and Resistance). Head Charge: 30 to itself.</summary>
        private sealed class BouffalantBST : Mon
        {
            private static readonly Mon HeadCharge = new RecoilMon("Head Charge", 30);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.AgainstOpponent || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Grass) ? damage + 60 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HeadCharge.HandleAttack(ctx, atk); }
        }

        // ================================================================== Supporters

        /// <summary>Bruno: shuffle your hand into your deck and draw 4 cards (7 if any of your Pokémon were Knocked Out during the opponent's last turn).</summary>
        private sealed class Bruno : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.WasKnockedOutLastOpponentTurn() ? 7 : 4);
            }
        }

        /// <summary>Cheryl: heal all damage from each of your Evolution Pokémon, then discard all Energy from the ones healed.</summary>
        private sealed class Cheryl : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsEvolution && m.DamageCounters > 0); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in Targets(ctx))
                    if (ctx.Heal(mon, mon.DamageCounters * 10) > 0) ctx.Engine.DiscardAllEnergy(mon);
            }
        }

        /// <summary>Korrina's Focus: draw cards until you have 6 cards in your hand.</summary>
        private sealed class KorrinasFocus : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count <= 6 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(6); }
        }

        /// <summary>Phoebe: this turn, damage from your Pokémon VMAX's attacks isn't affected by any effects on the opponent's Active Pokémon.</summary>
        private sealed class Phoebe : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new PhoebeEffect("Phoebe: your Pokémon VMAX's attacks ignore effects on the opponent's Active Pokémon this turn", ctx.Player, ctx.Turn));
            }
        }

        private sealed class PhoebeEffect : ContinuousEffect
        {
            public PhoebeEffect(string name, int owner, int turn) : base(name, owner, -1, turn, false) { }

            public override bool IgnoresDefenderEffects(GameEngine engine, DamageInfo info)
            {
                return info.Attacker != null && info.Attacker.Owner == Owner && info.Attacker.Def.IsVmax;
            }
        }

        // ================================================================== Items

        /// <summary>Camping Gear: search your deck for a card and put it into your hand; your turn ends.</summary>
        private sealed class CampingGear : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(CardsEVS1.AnyCard, 1, "Camping Gear: search your deck for a card", false);
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Escape Rope: each player switches their Active Pokémon with 1 of their Benched Pokémon (the opponent first).</summary>
        private sealed class EscapeRope : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Bench.Count > 0 || ctx.Opp.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                // Togekiss's Bright Veil (v111) stops only its owner's part of the switch (Compendium 2013-02-07).
                if (ctx.Opp.Bench.Count > 0 && !CardsPLS2.BrightVeilOn(ctx.Engine, ctx.Opponent)) ctx.OpponentSwitchesActive();
                if (ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Escape Rope: choose your new Active Pokémon");
            }
        }

        /// <summary>Level Ball: search your deck for a Pokémon with 90 HP or less.</summary>
        private sealed class LevelBall : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsPokemon && d.Hp > 0 && d.Hp <= 90; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(Fits, 1, "Level Ball: a Pokémon with 90 HP or less"); }
        }

        // ================================================================== Tools

        /// <summary>
        /// Rapid Strike Scroll of Swirls / Single Strike Scroll of Scorn: a Rapid Strike (Single Strike) Pokémon with it can use
        /// its attack. Matchless Maelstrom: 30 damage to each of the opponent's Pokémon. Furious Anger: 10 more for each damage
        /// counter on this Pokémon.
        /// </summary>
        private sealed class BstScroll : CardScript
        {
            private readonly bool single;

            public BstScroll(bool single) { this.single = single; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                bool fits = single ? mon.Def.IsSingleStrike : mon.Def.IsRapidStrike;
                if (!fits) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                var me = ctx.Pokemon;
                if (me == null) return;
                if (single)
                {
                    ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                    return;
                }
                ctx.DamageDefending(30);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 30);
            }
        }

        /// <summary>Tool Jammer: while its Pokémon is Active, Tools on the opponent's Active Pokémon (other than Tool Jammer) have no effect.</summary>
        private sealed class ToolJammer : CardScript
        {
            public override bool JamsOpposingActiveTools { get { return true; } }
        }

        // ================================================================== Stadiums

        /// <summary>Tower of Darkness: once during each player's turn, that player may discard a Single Strike card from their hand to draw 2 cards.</summary>
        private sealed class TowerOfDarkness : StadiumAction
        {
            private static List<CardInstance> Strikers(EffectContext ctx) { return ctx.Me.Hand.Where(c => CardsCRE2.IsSingleStrikeCard(c.Def)).ToList(); }

            protected override bool Ready(EffectContext ctx) { return Strikers(ctx).Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var strikers = Strikers(ctx);
                if (strikers.Count == 0) return false;
                var card = ctx.ChooseCards(strikers, 1, 1, "Tower of Darkness: discard a Single Strike card to draw 2 cards")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(2);
                return false;
            }
        }

        /// <summary>Tower of Waters: the Retreat Cost of each Rapid Strike Pokémon in play (both players') is 2 less.</summary>
        private sealed class TowerOfWaters : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.Def.IsRapidStrike ? cost - 2 : cost;
            }
        }

        // ================================================================== Energy

        /// <summary>Rapid Strike Energy: only on a Rapid Strike Pokémon (discarded from anything else); provides 2 Energy, each Water or Fighting.</summary>
        private sealed class RapidStrikeEnergy : CardScript
        {
            private static readonly int WaterOrFighting = EnergyMask.Of(EnergyType.Water) | EnergyMask.Of(EnergyType.Fighting);

            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(WaterOrFighting, 2); }
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Def.IsRapidStrike; }
            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Pokemon.Def.IsRapidStrike; }
        }

        /// <summary>
        /// Single Strike Energy: only on a Single Strike Pokémon (discarded from anything else); provides Fighting or Darkness, 1
        /// at a time; its Pokémon's attacks do 20 more damage to the opponent's Active Pokémon (before Weakness and Resistance).
        /// </summary>
        private sealed class SingleStrikeEnergy : CardScript
        {
            private static readonly int FightingOrDarkness = EnergyMask.Of(EnergyType.Fighting) | EnergyMask.Of(EnergyType.Darkness);

            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(FightingOrDarkness, 1); }
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Def.IsSingleStrike; }
            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Pokemon.Def.IsSingleStrike; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 20;
            }
        }
    }
}
