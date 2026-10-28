using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Astral Radiance (ASR) part 2: the Trainer Gallery Pokémon that weren't in the pool (TG1-TG22), the Trainers, Stadiums and
    /// Double Turbo Energy. Trainer Gallery reprints of other ASR cards (Wyrdeer, Kleavor, Mightyena, Hoothoot, Starmie V,
    /// Garchomp V, the second Calyrex VMAX) and of pool cards (Zacian V, Bea, Melony) share those cards' scripts.
    /// Compendium rulings applied: Temple of Sinnoh turns Double Turbo Energy into 1 [C] with no damage reduction, and Special
    /// Energy stay Special Energy with their names; Canceling Cologne affects the Active Spot itself, not the Pokémon in it (your
    /// ruling too); Kamado draws 4 even if nothing is discarded, as long as a card could be chosen; Gutsy Pickaxe needs a
    /// Benched Pokémon; Spicy Seasoned Curry is unplayable only if the Active Pokémon is undamaged and already Burned.
    /// </summary>
    public static class CardsASR2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("ASR-TG1", new AbomasnowTG());
            r.RegisterByKey("ASR-TG2", new FlappleTG());
            r.RegisterByKey("ASR-TG3", new KingdraTG());
            r.RegisterByKey("ASR-TG4", new FrosmothTG());
            r.RegisterByKey("ASR-TG5", new GardevoirTG());
            r.RegisterByKey("ASR-TG7", new CardsASR1.TimesMon("Rapid Strike Squad", RapidStrikeInPlay));
            r.RegisterByKey("ASR-TG10", new ObstagoonTG());
            r.RegisterByKey("ASR-TG11", new BronzongTG());
            r.RegisterByKey("ASR-TG14", new CardsTWM1.SelfDiscardMon("Glacial Lance", 2));
            r.RegisterByKey("ASR-TG15", new ComboMon(new CardsASR1.PlusMon("Ride of the High King", 30, OpponentBenched), new MaxLanceMon()));
            r.RegisterByKey("ASR-TG16", new GalarianArticunoV());
            r.RegisterByKey("ASR-TG17", new ComboMon(new ShadowMistMon(), new AstralBarrageMon()));
            r.RegisterByKey("ASR-TG18", new ShadowRiderCalyrexVmax());
            r.RegisterByKey("ASR-TG19", new GalarianZapdosV());
            r.RegisterByKey("ASR-TG20", new GalarianMoltresV());
            r.RegisterByKey("ASR-TG22", new ZamazentaV());

            r.RegisterByName("Canceling Cologne", new CancelingCologne());
            r.RegisterByName("Choy", new Choy());
            r.RegisterByName("Cyllene", new Cyllene());
            r.RegisterByName("Energy Loto", new EnergyLoto());
            r.RegisterByName("Feather Ball", new FeatherBall());
            r.RegisterByName("Gutsy Pickaxe", new GutsyPickaxe());
            r.RegisterByName("Hisuian Heavy Ball", new HisuianHeavyBall());
            r.RegisterByName("Jubilife Village", new JubilifeVillage());
            r.RegisterByName("Kamado", new Kamado());
            r.RegisterByName("Spicy Seasoned Curry", new SpicySeasonedCurry());
            r.RegisterByName("Supereffective Glasses", new SupereffectiveGlasses());
            r.RegisterByName("Sweet Honey", new SweetHoney());
            r.RegisterByName("Switch Cart", new SwitchCart());
            r.RegisterByName("Temple of Sinnoh", new TempleOfSinnoh());
            r.RegisterByName("Wait and See Turbo", new WaitAndSeeTurbo());
            r.RegisterByName("Zisu", new Zisu());
            r.RegisterByName("Path to the Peak", new PathToThePeak());
            r.RegisterByName("Double Turbo Energy", new DoubleTurboEnergy());
            r.RegisterByName("Allister", new Allister());
            r.RegisterByName("Milo", new Milo());
            r.RegisterByName("Piers", new Piers());
        }

        private static int RapidStrikeInPlay(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsRapidStrike).Count; }
        private static int OpponentBenched(EffectContext ctx) { return ctx.Opp.Bench.Count; }

        // ================================================================== Trainer Gallery Pokémon

        /// <summary>Toughness Boost: your Single Strike Pokémon except Abomasnow get +50 HP (only 1 Toughness Boost at a time).</summary>
        private sealed class AbomasnowTG : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon.Owner != src.Player || mon.Name == "Abomasnow" || !mon.Def.IsSingleStrike) return hp;
                return CardsCRZ1.IsLeadSource(src, "Toughness Boost") ? hp + 50 : hp;
            }
        }

        /// <summary>
        /// Apple Drop: once during your turn, put 2 damage counters on 1 of your opponent's Pokémon; if you placed any, shuffle
        /// this Pokémon and all attached cards into your deck. Acid Spray: flip, heads discards an Energy from the Defending Pokémon.
        /// </summary>
        private sealed class FlappleTG : Mon
        {
            private static readonly Mon spray = new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray");

            protected override string Activated { get { return "Apple Drop"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Apple Drop: put 2 damage counters on which of your opponent's Pokémon?");
                if (target == null) return;
                if (ctx.Engine.PlaceDamageCounters(target, 2, "Apple Drop") > 0 && ctx.InPlay(ctx.Pokemon))
                    ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return spray.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Deep Sea King: when your Active Pokémon is Knocked Out by damage from an opponent's attack, you may move any amount of
        /// Water Energy from it to this Pokémon (after the attack's own effects, Compendium). Aqua Burst: 40 for each Water Energy on it.
        /// </summary>
        private sealed class KingdraTG : Mon
        {
            private static readonly Mon burst = new CardsASR1.TimesMon("Aqua Burst", OwnWater);

            private static int OwnWater(EffectContext ctx) { return ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water); }

            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                var me = src.Pokemon;
                if (!byOpponentsAttack || mon == null || me == null || mon == me || mon.Owner != src.Player || !mon.HitWhileActive) return;
                if (src.Engine.IsKnockedOut(me)) return;
                var water = mon.Energy.Where(e => CardsDRI1.ProvidesType(src, mon, e, EnergyType.Water)).ToList();
                if (water.Count == 0) return;
                var picks = src.ChooseCards(water, 0, water.Count, "Deep Sea King: move any amount of Water Energy from " + mon.Name + " to " + me.Name + "?");
                if (picks.Count == 0) return;
                Announce(src, "Deep Sea King");
                foreach (var w in picks) src.Engine.MoveEnergy(mon, me, w);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return burst.HandleAttack(ctx, atk); }
        }

        /// <summary>Ice Dance: as often as you like during your turn, attach a Water Energy card from your hand to 1 of your Benched Water Pokémon.</summary>
        private sealed class FrosmothTG : Mon
        {
            private const string Dance = "Ice Dance";

            private static bool IsWaterCard(CardDef d) { return CardsASR1.IsEnergyCardOf(d, EnergyType.Water); }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Water) && ctx.Engine.CanAttachFromHand(m)).ToList();
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Dance || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(c => IsWaterCard(c.Def)) && Targets(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.AttachFromHand(ctx, IsWaterCard, () => Targets(ctx), Dance, "a Water Energy card");
            }
        }

        /// <summary>
        /// Shining Arcana: once during your turn, look at the top 2 cards of your deck, attach any number of basic Energy cards
        /// there to your Pokémon in any way you like, and put the other cards into your hand. Brainwave: 30 more per Psychic Energy.
        /// </summary>
        private sealed class GardevoirTG : Mon
        {
            private static readonly Mon brainwave = new CardsMEG.EnergyBonusMon("Brainwave", EnergyType.Psychic, 30);

            protected override string Activated { get { return "Shining Arcana"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, energy.Count, (card, mon) => true, "Shining Arcana: attach a basic Energy card to 1 of your Pokémon");
                ctx.DeckToHand(top.Where(c => ctx.Me.Deck.Contains(c)).ToList());
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return brainwave.HandleAttack(ctx, atk); }
        }

        /// <summary>Untamed Shout: when you play this Pokémon from your hand to evolve, you may put 3 damage counters on 1 of your opponent's Pokémon. Obstruct.</summary>
        private sealed class ObstagoonTG : Mon
        {
            private static readonly Mon obstruct = new CardsCRI.BasicShieldAttackMon("Obstruct");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Untamed Shout", ctx.Opp.HasPokemonInPlay, "Untamed Shout: put 3 damage counters on 1 of your opponent's Pokémon?")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Untamed Shout: put 3 damage counters on which Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return obstruct.HandleAttack(ctx, atk); }
        }

        /// <summary>Metal Transfer: as often as you like during your turn, move a Metal Energy from 1 of your Pokémon to another of your Pokémon.</summary>
        private sealed class BronzongTG : Mon
        {
            private const string Transfer = "Metal Transfer";

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Metal)));
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
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Transfer + ": move a Metal Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                var metal = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, EnergyType.Metal)).ToList();
                if (metal.Count == 0) return false;
                var card = metal.Select(e => e.Def.Name).Distinct().Count() == 1 ? metal[0] : ctx.ChooseCards(metal, 1, 1, Transfer + ": move which Energy?")[0];
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), Transfer + ": move it from " + from.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveEnergy(from, to, card);
            }
        }

        /// <summary>Max Lance: you may discard up to 2 Energy from this Pokémon; 120 more damage for each card discarded.</summary>
        private sealed class MaxLanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Lance") return false;
                var me = ctx.Pokemon;
                int n = 0;
                if (me.Energy.Count > 0)
                {
                    var picks = ctx.ChooseCards(me.Energy.ToList(), 0, Math.Min(2, me.Energy.Count), "Max Lance: discard up to 2 Energy from this Pokémon (120 more damage each)");
                    foreach (var e in picks) if (ctx.Engine.DiscardEnergy(me, e)) n++;
                }
                ctx.DamageDefending(atk.Damage + 120 * n);
                return true;
            }
        }

        /// <summary>Reconstitute: once during your turn, discard 2 cards from your hand (a cost) to draw a card. Psyray: Confused.</summary>
        private sealed class GalarianArticunoV : Mon
        {
            private static readonly Mon psyray = new ConditionAttackMon("Psyray", SpecialCondition.Confused);

            protected override string Activated { get { return "Reconstitute"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count >= 2 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Reconstitute: discard 2 cards from your hand");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return psyray.HandleAttack(ctx, atk); }
        }

        /// <summary>Shadow Mist: during your opponent's next turn, they can't play Special Energy or Stadium cards from their hand.</summary>
        private sealed class ShadowMistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Mist") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsPAF1.SpecialEnergyLockEffect("Shadow Mist: no Special Energy cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                ctx.AddEffect(new CardsMEG.StadiumLockEffect("Shadow Mist: no Stadium cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Astral Barrage: choose 2 of your opponent's Pokémon and put 5 damage counters on each.</summary>
        private sealed class AstralBarrageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Astral Barrage") return false;
                var all = ctx.OpponentPokemon();
                int n = Math.Min(2, all.Count);
                if (n == 0) return true;
                foreach (var t in ctx.ChoosePokemonMany(all, n, n, "Astral Barrage: choose " + n + " of your opponent's Pokémon (5 damage counters each)"))
                    ctx.PlaceCounters(t, 5);
                return true;
            }
        }

        /// <summary>
        /// Underworld Door: once during your turn, attach a Psychic Energy card from your hand to 1 of your Benched Psychic
        /// Pokémon; if you did, draw 2 cards (usable with an empty deck, Compendium). Max Geist: 30 more per Psychic Energy on your Pokémon.
        /// </summary>
        private sealed class ShadowRiderCalyrexVmax : Mon
        {
            private static readonly Mon geist = new CardsASR1.PlusMon("Max Geist", 30, PsychicInPlay);

            private static int PsychicInPlay(EffectContext ctx) { return ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Psychic)); }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Psychic) && ctx.Engine.CanAttachFromHand(m)).ToList();
            }

            protected override string Activated { get { return "Underworld Door"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsASR1.IsEnergyCardOf(c.Def, EnergyType.Psychic)) && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = ctx.Me.Hand.Where(c => CardsASR1.IsEnergyCardOf(c.Def, EnergyType.Psychic)).ToList();
                var card = cards.Select(c => c.Def.Name).Distinct().Count() == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Underworld Door: attach which Psychic Energy card?")[0];
                var target = ctx.ChoosePokemon(Targets(ctx), "Underworld Door: attach " + card.Def.Name + " to which Benched Psychic Pokémon?");
                if (target == null) return;
                CardsCRZ1.AttachFromHand(ctx, card, target);
                if (target.Energy.Contains(card)) ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return geist.HandleAttack(ctx, atk); }
        }

        /// <summary>Fighting Instinct: this Pokémon's attacks cost 1 Colorless less for each of your opponent's Pokémon V in play. Thunderous Kick.</summary>
        private sealed class GalarianZapdosV : Mon
        {
            private static readonly Mon kick = new CardsASR1.DiscardDefendingSpecialMon("Thunderous Kick", true);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != src.Pokemon) return;
                int n = src.Opp.InPlay().Count(m => m.Def.IsPokemonV);
                for (int i = 0; i < n; i++) if (!cost.Remove(EnergyType.Colorless)) break;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return kick.HandleAttack(ctx, atk); }
        }

        /// <summary>Direflame Wings: once during your turn, attach a Darkness Energy card from your discard pile to this Pokémon (only 1 Direflame Wings each turn). Aura Burn.</summary>
        private sealed class GalarianMoltresV : Mon
        {
            private static readonly Mon burn = new RecoilMon("Aura Burn", 30);

            protected override string Activated { get { return "Direflame Wings"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => CardsASR1.IsEnergyCardOf(c.Def, EnergyType.Darkness));
            }

            protected override void Ability(EffectContext ctx)
            {
                var dark = ctx.Me.Discard.Where(c => CardsASR1.IsEnergyCardOf(c.Def, EnergyType.Darkness)).ToList();
                var card = dark.Select(c => c.Def.Name).Distinct().Count() == 1 ? dark[0] : ctx.ChooseCards(dark, 1, 1, "Direflame Wings: attach which Darkness Energy card?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return burn.HandleAttack(ctx, atk); }
        }

        /// <summary>Dauntless Shield: prevent all damage done to this Pokémon by attacks from your opponent's Pokémon VMAX. Assault Tackle.</summary>
        private sealed class ZamazentaV : Mon
        {
            private static readonly Mon tackle = new CardsASR1.DiscardDefendingSpecialMon("Assault Tackle", false);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || info.Attacker == null || !info.AgainstOpponent) return damage;
                return info.Attacker.Def.IsVmax ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return tackle.HandleAttack(ctx, atk); }
        }

        // ================================================================== Items

        /// <summary>Canceling Cologne: until the end of your turn, the opponent's Active Spot has no Abilities (a new Active Pokémon too).</summary>
        private sealed class CancelingCologne : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new CologneEffect("Canceling Cologne: " + ctx.Engine.Possessive(ctx.Opponent) + " Active Pokémon has no Abilities this turn", ctx.Player, ctx.Turn));
            }
        }

        /// <summary>The Active Spot of the other player has no Abilities until the end of the turn (whichever Pokémon is there).</summary>
        internal sealed class CologneEffect : ContinuousEffect
        {
            private readonly int affected;

            public CologneEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = 1 - owner;
            }

            public override bool BlocksAbility(GameEngine engine, PokemonInPlay mon, string abilityName, int depth)
            {
                return mon != null && engine.State.Players[affected].Active == mon;
            }
        }

        /// <summary>Energy Loto: look at the top 7 cards; you may reveal an Energy card there and put it into your hand; shuffle the rest back.</summary>
        private sealed class EnergyLoto : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0)
                {
                    var pick = ctx.ChooseCards(energy, 0, 1, "Energy Loto: put an Energy card into your hand?");
                    if (pick.Count > 0)
                    {
                        ctx.Engine.Reveal(ctx.Player, pick, "put into " + ctx.Engine.Their(ctx.Player) + " hand (Energy Loto)");
                        ctx.DeckToHand(pick);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Feather Ball: search your deck for a Pokémon that has no Retreat Cost (its printed cost: it isn't in play).</summary>
        private sealed class FeatherBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.RetreatCost == 0, 1, "Feather Ball: choose a Pokémon with no Retreat Cost");
            }
        }

        /// <summary>Gutsy Pickaxe: reveal the top card; a Fighting Energy card is attached to 1 of your Benched Pokémon, anything else goes to your hand.</summary>
        private sealed class GutsyPickaxe : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Me.Deck[0];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { top }, "revealed from the top of " + ctx.Engine.Their(ctx.Player) + " deck (Gutsy Pickaxe)");
                if (CardsASR1.IsEnergyCardOf(top.Def, EnergyType.Fighting) && ctx.Me.Bench.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Gutsy Pickaxe: attach " + top.Def.Name + " to which Benched Pokémon?");
                    if (target != null && ctx.Me.Deck.Remove(top))
                    {
                        ctx.Engine.AttachEnergyCard(top, target);
                        return;
                    }
                }
                ctx.DeckToHand(new List<CardInstance> { top });
            }
        }

        /// <summary>
        /// Hisuian Heavy Ball: look at your face-down Prize cards; you may take a Basic Pokémon there into your hand and put this
        /// card in its place as a face-down Prize card (otherwise it's discarded). Then shuffle your face-down Prize cards.
        /// </summary>
        private sealed class HisuianHeavyBall : CardScript
        {
            private static List<int> FaceDown(PlayerState me)
            {
                var list = new List<int>();
                for (int i = 0; i < me.Prizes.Count; i++)
                    if (!me.FaceUpPrizeIds.Contains(me.Prizes[i].Id)) list.Add(i);
                return list;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return FaceDown(ctx.Me).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                var prizes = FaceDown(me).Select(i => me.Prizes[i]).ToList();
                ctx.Engine.NoteLookedAt(prizes, "Your face-down Prize cards", ctx.Player);
                var basics = prizes.Where(c => c.Def.IsBasicPokemon).ToList();
                // With no Basic Pokémon there, you still see your Prize cards (press Done) before they're shuffled (v69).
                if (basics.Count == 0) ctx.ChooseCards(basics, 0, 1, "Hisuian Heavy Ball: there's no Basic Pokémon in your face-down Prize cards");
                if (basics.Count > 0)
                {
                    var pick = ctx.ChooseCards(basics, 0, 1, "Hisuian Heavy Ball: take a Basic Pokémon into your hand (Hisuian Heavy Ball takes its place)?");
                    if (pick.Count > 0)
                    {
                        var taken = pick[0];
                        var slots = me.PrizeSlotList();
                        int index = me.Prizes.IndexOf(taken);
                        int slot = slots[index];
                        me.Prizes[index] = ctx.Card;
                        me.PrizeSlotByCard.Remove(taken.Id);
                        me.PrizeSlotByCard[ctx.Card.Id] = slot;
                        me.Hand.Add(taken);
                        ctx.CardPlacedElsewhere = true;
                        ctx.Engine.Reveal(ctx.Player, pick, "took from " + ctx.Engine.Their(ctx.Player) + " Prize cards (Hisuian Heavy Ball took its place)");
                    }
                }
                ShuffleFaceDownPrizes(ctx);
            }

            /// <summary>Shuffles the face-down Prize cards among their own table slots (face-up ones stay put).</summary>
            internal static void ShuffleFaceDownPrizes(EffectContext ctx)
            {
                var me = ctx.Me;
                var slots = me.PrizeSlotList();
                var idx = FaceDown(me);
                var cards = idx.Select(i => me.Prizes[i]).ToList();
                ctx.State.Rng.Shuffle(cards);
                for (int k = 0; k < idx.Count; k++)
                {
                    me.Prizes[idx[k]] = cards[k];
                    me.PrizeSlotByCard[cards[k].Id] = slots[idx[k]];
                }
                ctx.Engine.ForgetLookedAt();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + ctx.Engine.Their(ctx.Player) + " face-down Prize cards.");
            }
        }

        /// <summary>Spicy Seasoned Curry: your Active Pokémon is now Burned; heal 40 damage from it.</summary>
        private sealed class SpicySeasonedCurry : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && (a.DamageCounters > 0 || !a.Has(SpecialCondition.Burned));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                if (a == null) return;
                ctx.ApplyCondition(a, SpecialCondition.Burned);
                ctx.Heal(a, 40);
            }
        }

        /// <summary>Sweet Honey: choose 1 of your Pokémon, then flip until tails; heal 40 damage from it for each heads.</summary>
        private sealed class SweetHoney : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Sweet Honey: choose 1 of your Pokémon to heal");
                if (target == null) return;
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Heal(target, 40 * heads);
            }
        }

        /// <summary>Switch Cart: switch your Active Basic Pokémon with 1 of your Benched Pokémon; if you do, heal 30 from the one moved to the Bench.</summary>
        private sealed class SwitchCart : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Active.IsBasic && ctx.Me.Bench.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var old = ctx.Me.Active;
                if (old == null || !old.IsBasic) return;
                if (ctx.SwitchMyActive("Switch Cart: choose the Benched Pokémon to switch in") && ctx.InPlay(old)) ctx.Heal(old, 30);
            }
        }

        /// <summary>Wait and See Turbo: only if you go second, during your first turn; attach a basic Energy from your deck to 1 of your Pokémon. Your turn ends.</summary>
        private sealed class WaitAndSeeTurbo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count > 0)
                {
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Wait and See Turbo: choose a basic Energy card to attach");
                    if (found.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Wait and See Turbo: attach " + found[0].Def.Name + " to which Pokémon?");
                        if (target != null) ctx.Engine.AttachEnergyCard(found[0], target);
                        else ctx.Me.Deck.Add(found[0]);
                    }
                    ctx.ShuffleMyDeck();
                }
                ctx.Engine.RequestEndTurn();
            }
        }

        // ================================================================== Tools and Energy

        /// <summary>Supereffective Glasses: Weakness to the holder's attacks on the opponent's Active Pokémon is x3.</summary>
        private sealed class SupereffectiveGlasses : CardScript
        {
            public override int WeaknessMultiplier(EffectContext src, DamageInfo info)
            {
                return src.Pokemon != null && info.Attacker == src.Pokemon && info.ToOpponentsActive ? 3 : 2;
            }
        }

        /// <summary>Double Turbo Energy: 2 Colorless Energy; the holder's attacks do 20 less damage to the opponent's Pokémon (before Weakness and Resistance).</summary>
        private sealed class DoubleTurboEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return new List<int> { EnergyMask.Colorless, EnergyMask.Colorless };
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.AgainstOpponent) return damage;
                return damage - 20;
            }
        }

        // ================================================================== Stadiums

        /// <summary>Jubilife Village: once during each player's turn, they may shuffle their hand into their deck and draw 5 cards; their turn then ends.</summary>
        private sealed class JubilifeVillage : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count + ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
                return true;
            }
        }

        /// <summary>Temple of Sinnoh: Special Energy attached to Pokémon provide Colorless Energy and have no other effect (engine: SpecialEnergyOff).</summary>
        private sealed class TempleOfSinnoh : CardScript
        {
            public override bool DisablesSpecialEnergy(EffectContext src) { return true; }
        }

        /// <summary>Path to the Peak: Pokémon with a Rule Box in play (both players') have no Abilities.</summary>
        private sealed class PathToThePeak : CardScript
        {
            public override bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon) { return mon != null && mon.Def.HasRuleBox; }
        }

        // ================================================================== Supporters

        /// <summary>Choy: each player reveals their hand; draw 3 cards.</summary>
        private sealed class Choy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mine = ctx.Me.Hand.Select(c => c.Def.Name).ToArray();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " revealed their hand: " + (mine.Length == 0 ? "no cards" : string.Join(", ", mine)) + ".");
                if (!TefPokemon.OpponentShielded(ctx)) TefPokemon.RevealOpponentHand(ctx);
                ctx.Draw(3);
            }
        }

        /// <summary>Cyllene: flip 2 coins; put up to that many cards from your discard pile on top of your deck in any order.</summary>
        private sealed class Cyllene : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 0 || ctx.Me.Discard.Count == 0) return;
                var picks = ctx.ChooseCards(ctx.Me.Discard.ToList(), 0, Math.Min(heads, ctx.Me.Discard.Count), "Cyllene: put up to " + heads + (heads == 1 ? " card" : " cards") + " from your discard pile on top of your deck");
                var moved = new List<CardInstance>();
                foreach (var c in picks) if (ctx.Me.Discard.Remove(c)) moved.Add(c);
                if (moved.Count == 0) return;
                ctx.Engine.Reveal(ctx.Player, moved, "put on top of " + ctx.Engine.Their(ctx.Player) + " deck (Cyllene)");
                CardsCRZ1.PutOnTopInOrder(ctx, moved, "Cyllene");
            }
        }

        /// <summary>Kamado: choose a card in your hand and discard the rest; if you do, draw 4 (needs another card in your hand).</summary>
        private sealed class Kamado : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var keep = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Kamado: choose the card to keep (the others are discarded)")[0];
                foreach (var c in ctx.Me.Hand.Where(x => x != keep).ToList()) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.Draw(4);
            }
        }

        /// <summary>Zisu: draw until you have 1 more card in your hand than your opponent.</summary>
        private sealed class Zisu : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Opp.Hand.Count + 1 > ctx.Me.Hand.Count(c => c != ctx.Card);
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(ctx.Opp.Hand.Count + 1); }
        }

        /// <summary>Allister: draw 3 cards; if you drew any, discard up to 3 cards from your hand (at least 1).</summary>
        private sealed class Allister : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Draw(3) == 0 || ctx.Me.Hand.Count == 0) return;
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, Math.Min(3, ctx.Me.Hand.Count), "Allister: discard 1 to 3 cards from your hand");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
            }
        }

        /// <summary>Milo: discard up to 2 cards from your hand and draw 2 cards for each card discarded.</summary>
        private sealed class Milo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card) && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 0, Math.Min(2, ctx.Me.Hand.Count), "Milo: discard up to 2 cards from your hand (draw 2 for each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                if (picks.Count > 0) ctx.Draw(2 * picks.Count);
            }
        }

        /// <summary>Piers: search your deck for an Energy card and a Darkness Pokémon.</summary>
        private sealed class Piers : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Piers: choose an Energy card");
                found.AddRange(ctx.SearchMyDeck(d => d.IsPokemon && d.HasType(EnergyType.Darkness), 1, "Piers: choose a Darkness Pokémon"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }
    }
}
