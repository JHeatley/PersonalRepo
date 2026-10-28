using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Delta Reign (DLR; v152): the cards shown so far. Regulation mark J. Legendary Pyroduct and Legendary Summit are Legendary
    /// Stadiums: their two halves are played from the hand together as 1 Stadium (CardDef.IsStadiumHalf, GameState.StadiumPartner);
    /// like LEGEND, a deck has up to 4 cards of the same name; v154.2: each half is a Stadium card for searches (your ruling).
    /// </summary>
    public static class CardsDLR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DLR-11", new ComboMon(
                new CardsFST2.ConditionBonusMon("Finishing Blow", 160, c => c.Defending != null && c.Defending.DamageCounters > 0),
                new NoRetreatMon("Quadruple Hold")));
            r.RegisterByKey("DLR-47", new MegaGolurkExDLR());
            r.RegisterByKey("DLR-69", new ComboMon(
                new CardsASR1.TimesMon("Psychic Marionette", c => c.Opp.Bench.Count),
                new ConditionAttackMon("Eerie Wave", SpecialCondition.Confused)));
            r.RegisterByKey("DLR-84", new MegaRayquazaExDLR());
            r.RegisterByName("Legendary Pyroduct", new LegendaryPyroduct());
            r.RegisterByName("Legendary Summit", new LegendarySummit());

            // v152.1: more Delta Reign cards and the Delta Reign-stamped Mega Promos.
            r.RegisterByKey("DLR-31", new ComboMon(new ParalyzeFlipMon("Body Slam"), new VoltaicHammerMon()));
            r.RegisterByKey("DLR-35", new IncarnateSolidarityMon(new CardsPAF2.IgnoreDefenderEffectsMon("Thunderous Edge")));
            r.RegisterByKey("DLR-54", new IncarnateSolidarityMon(new CardsPAF1.DiscardStadiumMon("Gaia Crush")));
            r.RegisterByKey("DLR-58", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new BugOutMon()));
            r.RegisterByKey("DLR-72", new CardsWHT.DragOffMon("Drag Off", 40));
            r.RegisterByKey("DLR-85", new IncarnateSolidarityMon(new CardsDAA1.MayDrawUntilMon("Corkscrew Dive", 6)));
            r.RegisterByKey("DLR-90", new AdventureLantern());
            r.RegisterByKey("MEP-111", new ComboMon(new CardsEVS1.DefenderCantAttackMon("Frightening Pattern", d => true), new BugOutMon()));
            r.RegisterByKey("MEP-112", new MagmortarBuddyBoost());
            r.RegisterByKey("MEP-113", new IncarnateSolidarityMon(
                new CardsFST2.ConditionBonusMon("Rising Heart", 100, c => c.Defending != null && c.Defending.Def.IsEx)));
            r.RegisterByKey("MEP-114", new KommoOBeatScales());

            // v152.4: Heat Rotom ex, Groudon, Delibird, Zinnia's Trust, Eevee (Kommo-o 75 is a print of MEP-114).
            r.RegisterByKey("DLR-23", new ComboMon(new RewarmMon(), new CardsTWM1.SelfDiscardMon("Strong Flare", 2)));
            r.RegisterByKey("DLR-53", new CardsFST2.ConditionBonusMon("Brutal Lands", 170,
                c => c.State.Stadium != null && c.State.Stadium.Def.Name.Contains("Legendary")));
            r.RegisterByKey("DLR-79", new DelibirdEnergizingPresent());
            r.RegisterByKey("DLR-103", new ZinniasTrust());
            r.RegisterByKey("DLR-78", new CardsE3B.HealSelfMon("Nap", 20)); // v152.6: Eevee 78 is the main print, 114 its illustration rare

            // v152.5 (Masquerain 3 is a print of MEP-111).
            r.RegisterByKey("DLR-12", new FlipBonusMon("Play Rough", 20));
            r.RegisterByKey("DLR-13", new PlainMon());
            r.RegisterByKey("DLR-14", new MagicalBulletMon());
            r.RegisterByKey("DLR-18", new CardsE3B.OpponentSwitchesMon("Roar"));
            r.RegisterByKey("DLR-19", new ComboMon(new CardsFST2.ConditionBonusMon("Eager Fang", 90, c => c.Opp.Prizes.Count <= 4), new RecoilMon("Energetic Charge", 50)));
            r.RegisterByKey("DLR-29", new WishiwashiExOceanGain());
            r.RegisterByKey("DLR-32", new CardsTWM1.SelfDiscardMon("Thunderbolt", 99));
            r.RegisterByKey("DLR-33", new RaikouExDLR());

            // v152.6 (104 Masquerain, 105 Sprigatito, 106 Growlithe, 109 Pincurchin, 114 Eevee, 136 Raikou ex are prints).
            r.RegisterByKey("DLR-36", new CardsASR1.TimesMon("Energy Crush", c => c.OpponentPokemon().Sum(m => c.EnergyCount(m))));
            r.RegisterByKey("DLR-55", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("DLR-70", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("DLR-89", new Aarune());
            r.RegisterByKey("DLR-93", new Grisham());
            r.RegisterByName("Legendary Abyss", new LegendaryAbyss());

            // v153.2 (110 Enamorus is a print of MEP-113, 139 Aarune of DLR-89).
            r.RegisterByKey("DLR-44", new ComboMon(new CardsASR1.TimesMon("Mind Ruler", c => c.Opp.Hand.Count),
                                                   new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("DLR-83", new ColorfulWhipMon());
            r.RegisterByKey("DLR-112", new PlainMon());
        }

        /// <summary>Restricted Activation: this Pokémon can't attack unless you have 10 or more cards in your hand. Goliath's Punch: 30 to itself.</summary>
        private sealed class MegaGolurkExDLR : Mon
        {
            private const string RestrictedActivation = "Restricted Activation";
            private static readonly Mon Punch = new RecoilMon("Goliath's Punch", 30);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                // Without its Ability (Legendary Pyroduct, Path to the Peak...) nothing restricts it.
                if (me == null || ctx.Engine.AbilityBlocked(me, RestrictedActivation)) return true;
                return ctx.Me.Hand.Count >= 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Ruler's Roar: when played from the hand onto the Bench during your turn, you may look at the top 4 cards of your deck and
        /// attach a Basic Energy card from them to this Pokémon; the others are shuffled and put on the bottom of the deck.
        /// Storm Emerald: 50 for each Fire Energy and each Lightning Energy on all your Pokémon. An Energy that provides every type
        /// (1 at a time) counts once (your ruling).
        /// </summary>
        private sealed class MegaRayquazaExDLR : Mon
        {
            private const string Roar = "Ruler's Roar";

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null || !ctx.IsMyTurn || ctx.Me.Deck.Count == 0 || ctx.Engine.AbilityBlocked(mon, Roar)) return;
                if (!TriggerOnce(ctx, Roar, true, "Use " + Roar + "? Look at the top 4 cards of your deck and attach a Basic Energy card from them to " + mon.Name + ".")) return;
                var top = ctx.LookAtTop(4);
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                var basics = top.Where(c => c.Def.IsBasicEnergy).ToList();
                CardInstance attached = null;
                if (basics.Count > 0)
                {
                    var pick = ctx.ChooseCards(basics, 0, 1, Roar + ": attach a Basic Energy card to " + mon.Name);
                    if (pick.Count > 0 && ctx.Engine.AttachEnergyCard(pick[0], mon)) attached = pick[0];
                }
                var rest = top.Where(x => x != attached).ToList();
                ctx.State.Rng.Shuffle(rest);
                ctx.Me.Deck.AddRange(rest);
                if (rest.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + rest.Count + " cards on the bottom of their deck.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Storm Emerald") return false;
                int n = 0;
                foreach (var mon in ctx.MyPokemon())
                    foreach (int unit in ctx.Engine.EnergyUnits(mon))
                        if (EnergyMask.Provides(unit, EnergyType.Fire) || EnergyMask.Provides(unit, EnergyType.Lightning)) n++;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Legendary Pyroduct (Legendary Stadium): Evolution Pokémon in play (both players') have no Abilities.</summary>
        private sealed class LegendaryPyroduct : CardScript
        {
            public override bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon) { return mon != null && mon.Def.IsEvolution; }
        }

        /// <summary>
        /// Legendary Summit (Legendary Stadium): when a Colorless Pokémon (either player's) is Knocked Out by damage from an attack of
        /// the other player's Pokémon, that player takes 1 fewer Prize card.
        /// </summary>
        private sealed class LegendarySummit : CardScript
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut == null || taker == knockedOut.Owner || !src.Engine.IsType(knockedOut, EnergyType.Colorless)) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                src.Log("Legendary Summit: " + src.Engine.PlayerName(taker) + " takes 1 fewer Prize card.");
                return prizes - 1;
            }
        }

        // ================================================================== v152.1

        /// <summary>
        /// Incarnate Solidarity (Tornadus, Thundurus, Landorus, Enamorus): with all four (those exact names) in play on your side,
        /// ignore all Colorless Energy in the costs of this Pokémon's attacks (like Monkey Trio).
        /// </summary>
        private sealed class IncarnateSolidarityMon : Mon
        {
            private static readonly string[] Four = { "Tornadus", "Thundurus", "Landorus", "Enamorus" };
            private readonly Mon attacks;

            public IncarnateSolidarityMon(Mon attacks) { this.attacks = attacks; }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                var names = src.Me.InPlay().Select(m => m.Name).ToList();
                if (Four.All(n => names.Contains(n))) cost.RemoveAll(t => t == EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Bug Out: reveal the bottom 7 cards of your deck; 50 for each Pokémon there with a Bug Out attack. The revealed Pokémon are
        /// shuffled back into the deck and the other cards are discarded.
        /// </summary>
        internal sealed class BugOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bug Out") return false;
                var deck = ctx.Me.Deck;
                int n = System.Math.Min(7, deck.Count);
                var bottom = deck.Skip(deck.Count - n).ToList();
                if (bottom.Count > 0) ctx.Engine.Reveal(ctx.Player, bottom, "revealed the bottom " + bottom.Count + " cards of " + ctx.Engine.Their(ctx.Player) + " deck");
                int bugs = bottom.Count(x => x.Def.IsPokemon && x.Def.Attacks.Any(a => a.Name == "Bug Out"));
                ctx.DamageDefending(atk.Damage * bugs);
                foreach (var card in bottom)
                {
                    if (card.Def.IsPokemon) continue;
                    if (deck.Remove(card)) ctx.Me.Discard.Add(card);
                }
                if (bottom.Any(x => x.Def.IsPokemon)) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Voltaic Hammer: discard any number of Basic Energy cards from this Pokémon; 60 for each card discarded.</summary>
        private sealed class VoltaicHammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Voltaic Hammer") return false;
                var me = ctx.Pokemon;
                var basics = me.Energy.Where(x => x.Def.IsBasicEnergy).ToList();
                int discarded = 0;
                if (basics.Count > 0 && !CardsBS1.CopyingWithMetronome)
                {
                    var picks = ctx.ChooseCards(basics, 0, basics.Count, "Voltaic Hammer: discard any number of Basic Energy cards (60 damage for each)");
                    foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) discarded++;
                }
                ctx.DamageDefending(atk.Damage * discarded);
                return true;
            }
        }

        /// <summary>Adventure Lantern: a Basic Fire Energy card and a Basic Lightning Energy card from your deck into your hand.</summary>
        private sealed class AdventureLantern : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Fire), 1, "Adventure Lantern: choose a Basic Fire Energy card"));
                found.AddRange(ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Lightning), 1, "Adventure Lantern: choose a Basic Lightning Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Buddy Boost: once during your turn, attach a Basic Fire Energy card, a Basic Lightning Energy card, or 1 of each from your
        /// hand to your Electivire and Magmortar (those names) in any way you like.
        /// </summary>
        private sealed class MagmortarBuddyBoost : Mon
        {
            private const string Power = "Buddy Boost";

            protected override string Activated { get { return Power; } }

            private static bool Buddy(EffectContext ctx, PokemonInPlay m)
            {
                return (m.Name == "Electivire" || m.Name == "Magmortar") && ctx.Engine.CanAttachFromHand(m);
            }

            private static bool Fits(CardDef d, List<EnergyType> used)
            {
                return (CardsRS.BasicOf(d, EnergyType.Fire) && !used.Contains(EnergyType.Fire))
                       || (CardsRS.BasicOf(d, EnergyType.Lightning) && !used.Contains(EnergyType.Lightning));
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var none = new List<EnergyType>();
                return ctx.Me.Hand.Any(x => Fits(x.Def, none)) && ctx.MyPokemon(m => Buddy(ctx, m)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var used = new List<EnergyType>();
                for (int i = 0; i < 2; i++)
                {
                    var cards = ctx.Me.Hand.Where(x => Fits(x.Def, used)).ToList();
                    var targets = ctx.MyPokemon(m => Buddy(ctx, m));
                    if (cards.Count == 0 || targets.Count == 0) return;
                    var pick = ctx.ChooseCards(cards, i == 0 ? 1 : 0, 1, Power + ": attach a Basic Fire or Lightning Energy card" + (i == 0 ? "" : " (or none)"));
                    if (pick.Count == 0) return;
                    var card = pick[0];
                    var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, Power + ": attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null || !ctx.Me.Hand.Remove(card)) return;
                    used.Add(card.Def.EnergyProvides);
                    ctx.Engine.AttachEnergyCard(card, target, true);
                }
            }
        }

        /// <summary>
        /// Beat Scales: once during your turn, look at the top 6 cards of your deck and attach any number of Basic Energy cards there to
        /// your Dragon Pokémon in any way you like; shuffle the rest back.
        /// </summary>
        private sealed class KommoOBeatScales : Mon
        {
            private const string Power = "Beat Scales";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var basics = top.Where(x => x.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0 && ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Dragon)).Count > 0)
                    ctx.AttachCardsAnyWay(basics, basics.Count, (card, m) => ctx.Engine.IsType(m, EnergyType.Dragon), Power + ": attach a Basic Energy card to your Dragon Pokémon");
                ctx.ShuffleMyDeck();
            }
        }

        // ================================================================== v152.4

        /// <summary>Rewarm: 30 for each Basic Fire Energy card in your discard pile; then shuffle those cards into your deck.</summary>
        private sealed class RewarmMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rewarm") return false;
                var fire = ctx.Me.Discard.Where(x => CardsRS.BasicOf(x.Def, EnergyType.Fire)).ToList();
                ctx.DamageDefending(atk.Damage * fire.Count);
                if (fire.Count == 0) return true;
                var moved = ctx.DiscardToDeck(fire);
                if (moved.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Energizing Present: once during your turn, while this Pokémon is Active, look at the top 6 cards of your deck and put an
        /// Energy card from them into your hand (revealed); the others are shuffled and put on the bottom.
        /// </summary>
        private sealed class DelibirdEnergizingPresent : Mon
        {
            private const string Power = "Energizing Present";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                var energy = top.Where(x => x.Def.IsEnergy).ToList();
                CardInstance taken = null;
                if (energy.Count > 0)
                {
                    var pick = ctx.ChooseCards(energy, 0, 1, Power + ": put an Energy card into your hand");
                    if (pick.Count > 0)
                    {
                        taken = pick[0];
                        ctx.Me.Hand.Add(taken);
                        ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { taken }, "put into " + ctx.Engine.Their(ctx.Player) + " hand");
                    }
                }
                var rest = top.Where(x => x != taken).ToList();
                ctx.State.Rng.Shuffle(rest);
                ctx.Me.Deck.AddRange(rest);
            }
        }

        /// <summary>
        /// Zinnia's Trust (Supporter): switch your Active Pokémon with 1 of your Benched Pokémon; if you do, move an Energy from the
        /// Pokémon that went to the Bench to the new Active Pokémon.
        /// </summary>
        private sealed class ZinniasTrust : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var old = ctx.Me.Active;
                if (!ctx.SwitchMyActive("Zinnia's Trust: choose a Benched Pokémon to switch in")) return;
                var active = ctx.Me.Active;
                if (old == null || active == null || old == active || old.Energy.Count == 0) return;
                var energy = old.Energy.ToList();
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Zinnia's Trust: move which Energy from " + old.Name + " to " + active.Name + "?")[0];
                ctx.Engine.MoveEnergy(old, active, card);
            }
        }

        // ================================================================== v152.5

        /// <summary>Magical Bullet: also 120 to 1 of the opponent's Benched Pokémon that has any damage counters (no Weakness or Resistance).</summary>
        private sealed class MagicalBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Bullet") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(120, m => m.DamageCounters > 0);
                return true;
            }
        }

        /// <summary>Ocean Gain: once during your turn, while this Pokémon is Active, heal 50 damage from it.</summary>
        private sealed class WishiwashiExOceanGain : Mon
        {
            protected override string Activated { get { return "Ocean Gain"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Pokemon.DamageCounters > 0 && !ctx.Engine.HealingBlocked(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Pokemon, 50); }
        }

        /// <summary>
        /// Thunder-Clad: usable during your first turn if you go first; an Energy card from your deck onto this Pokémon. Power Rush:
        /// flip a coin; tails, this Pokémon can't use attacks during your next turn.
        /// </summary>
        private sealed class RaikouExDLR : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Thunder-Clad"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Thunder-Clad")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Thunder-Clad: choose an Energy card to attach to " + me.Name);
                    foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name != "Power Rush") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== v152.6

        /// <summary>Aarune (Supporter): up to 3 Supporter and/or Stadium cards from your deck into your hand (v154.2: each Legendary Stadium half is a Stadium card and counts as 1).</summary>
        private sealed class Aarune : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter || d.IsStadiumCard, 3, "Aarune: choose up to 3 Supporter and/or Stadium cards");
            }
        }

        // ================================================================== v153.2

        /// <summary>
        /// Colorful Whip (Kecleon 83): reveal any number of Pokémon from your hand; 30 for each different type among them. A
        /// dual-type Pokémon adds both its types, and the player picks what to reveal (a bot reveals them all) (your rulings).
        /// </summary>
        private sealed class ColorfulWhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Whip") return false;
                var inHand = ctx.Me.Hand.Where(h => h.Def.IsPokemon).ToList();
                List<CardInstance> shown;
                if (inHand.Count == 0) shown = new List<CardInstance>();
                else if (ctx.Engine.IsInteractive(ctx.Player))
                    shown = ctx.ChooseCards(inHand, 0, inHand.Count, "Colorful Whip: reveal any number of Pokémon from your hand (30 damage for each different type)");
                else shown = inHand;
                if (shown.Count > 0) ctx.Engine.Reveal(ctx.Player, shown, "revealed for Colorful Whip");
                var types = new HashSet<EnergyType>();
                foreach (var card in shown)
                {
                    types.Add(card.Def.PokemonType);
                    if (card.Def.SecondType.HasValue) types.Add(card.Def.SecondType.Value);
                }
                ctx.DamageDefending(atk.Damage * types.Count);
                return true;
            }
        }

        /// <summary>Grisham (Supporter): heal 50 damage from each Pokémon (both players'). Like other healing cards, it needs a damaged Pokémon.</summary>
        private sealed class Grisham : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in Hurt(ctx)) ctx.Heal(mon, 50);
            }
        }

        /// <summary>Legendary Abyss (Legendary Stadium): each time a Pokémon (either player's) is healed, double the amount healed.</summary>
        private sealed class LegendaryAbyss : CardScript
        {
            public override int ModifyHealAmount(EffectContext src, PokemonInPlay target, int amount) { return amount * 2; }
        }
    }
}
