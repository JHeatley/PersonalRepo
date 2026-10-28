using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Silver Tempest (SIT, 2022), part 2: the Trainer Gallery Pokémon that aren't reprints of main-set cards (TG3, TG6-TG9,
    /// TG12, TG14-TG16, TG18, TG19, TG22), the Trainers and the Special Energy. TG20/TG29 (Rayquaza VMAX), TG21/TG30
    /// (Duraludon VMAX), TG25 (Judge) and TG27 (Raihan) are already in the pool, and Energy Switch (SIT 212) too. Gapejaw Bog
    /// (SIT 213) is a reprint of Astral Radiance's, added here first.
    /// </summary>
    public static class CardsSIT2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Trainer Gallery Pokémon
            r.RegisterByKey("SIT-TG3", new FlaaffySIT());
            r.RegisterByKey("SIT-TG6", new RapidStrikeTentaclesMon());
            r.RegisterByKey("SIT-TG7", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("SIT-TG8", new PassimianSIT());
            r.RegisterByKey("SIT-TG9", new Cards30C.RetaliateBonusMon("Revenge", 120));
            r.RegisterByKey("SIT-TG12", new KricketuneV());
            r.RegisterByKey("SIT-TG14", new CardsTWM1.SelfDiscardMon("Fire Spin", 2));
            r.RegisterByKey("SIT-TG15", new ComboMon(new NoRetreatMon("Clutch"), new MaxBlazeMon()));
            r.RegisterByKey("SIT-TG16", new CrossFistMon());
            r.RegisterByKey("SIT-TG18", new ComboMon(new NoRetreatMon("Clutch"), new LockAfterAttackMon("Sky Hurricane", true)));
            r.RegisterByKey("SIT-TG19", new CorviknightVmax());
            r.RegisterByKey("SIT-TG22", new BlisseyV());

            // Trainers
            r.RegisterByName("Brandon", new Brandon());
            r.RegisterByName("Candice", new Candice());
            r.RegisterByName("Capturing Aroma", new CapturingAroma());
            r.RegisterByName("Earthen Seal Stone", new EarthenSealStone());
            r.RegisterByName("Emergency Jelly", new EmergencyJelly());
            r.RegisterByName("Forest Seal Stone", new ForestSealStone());
            r.RegisterByName("Furisode Girl", new FurisodeGirl());
            r.RegisterByName("Gym Trainer", new GymTrainer());
            r.RegisterByName("Lance", new Lance());
            r.RegisterByName("Leafy Camo Poncho", new LeafyCamoPoncho());
            r.RegisterByName("Primordial Altar", new PrimordialAltar());
            r.RegisterByName("Professor Laventon", new ProfessorLaventon());
            r.RegisterByName("Quad Stone", new QuadStone());
            r.RegisterByName("Serena", new Serena());
            r.RegisterByName("Wallace", new Wallace());
            r.RegisterByName("Worker", new Worker());
            r.RegisterByName("Gapejaw Bog", new GapejawBog());
            // Unidentified Fossil's play rules are in the engine (CardDef.IsFossil with FossilPlain): Prize cards and Special
            // Conditions work as for any Pokémon (your ruling).
            r.RegisterByName("Unidentified Fossil", new CardScript());
            r.RegisterByName("Friends in Galar", new DrawThree());
            r.RegisterByName("Gordie", new Gordie());
            r.RegisterByName("Professor Burnet", new ProfessorBurnet());
            r.RegisterByName("Sordward & Shielbert", new SordwardAndShielbert());

            // Special Energy
            r.RegisterByName("Regenerative Energy", new RegenerativeEnergy());
            r.RegisterByName("V Guard Energy", new VGuardEnergy());
        }

        // ================================================================== Trainer Gallery Pokémon

        /// <summary>Dynamotor: once during your turn, attach a basic Lightning Energy from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class FlaaffySIT : Mon
        {
            protected override string Activated { get { return "Dynamotor"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Bench.Count > 0 && ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var lightning = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning);
                ctx.AttachCardsAnyWay(lightning, 1, (card, m) => !ctx.IsActive(m), "Dynamotor", false);
            }
        }

        /// <summary>Rapid Strike Tentacles: reveal any number of Rapid Strike cards from your hand, 40 damage for each, then shuffle them into your deck.</summary>
        private sealed class RapidStrikeTentaclesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Strike Tentacles") return false;
                var rapid = ctx.Me.Hand.Where(c => c.Def.IsRapidStrike).ToList();
                var shown = rapid.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(rapid, 0, rapid.Count, "Rapid Strike Tentacles: reveal any number of Rapid Strike cards from your hand (40 damage for each)");
                if (shown.Count > 0) ctx.Engine.Reveal(ctx.Player, shown, "revealed");
                ctx.DamageDefending(atk.Damage * shown.Count);
                if (shown.Count > 0)
                {
                    foreach (var card in shown)
                        if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Throwing Coach: your Rapid Strike Pokémon's attacks do 30 more to the opponent's Benched Pokémon V and GX (doesn't stack). Fling: 20 to any Pokémon.</summary>
        private sealed class PassimianSIT : Mon
        {
            private static readonly Mon fling = new CardsMEG.SnipeMon("Fling", 20);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                // Only when the attack actually damages that Benched Pokémon (Compendium, 2021-06-17).
                if (attacker == null || attacker.Owner != src.Player || !info.AgainstOpponent || info.TargetIsActive || damage <= 0) return damage;
                if (!attacker.Def.IsRapidStrike || !(info.Target.Def.IsPokemonV || info.Target.Def.IsGX)) return damage;
                if (!CardsCRZ1.IsLeadSource(src, "Throwing Coach")) return damage;
                return damage + 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return fling.HandleAttack(ctx, atk); }
        }

        /// <summary>Exciting Stage: once during your turn draw until you have 3 cards (4 if this Pokémon is Active); 1 Exciting Stage each turn. X-Scissor: 80 more on heads.</summary>
        private sealed class KricketuneV : Mon
        {
            private static readonly Mon xScissor = new FlipBonusMon("X-Scissor", 80);

            protected override string Activated { get { return "Exciting Stage"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count < (ctx.PokemonIsActive ? 4 : 3);
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.DrawUntil(ctx.PokemonIsActive ? 4 : 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return xScissor.HandleAttack(ctx, atk); }
        }

        /// <summary>Max Blaze: attach an Energy card from your discard pile to each of up to 2 of your Benched Rapid Strike Pokémon.</summary>
        private sealed class MaxBlazeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Blaze") return false;
                ctx.DamageDefending(atk.Damage);
                var rapid = ctx.MyBench().Where(m => m.Def.IsRapidStrike).ToList();
                if (rapid.Count == 0 || !ctx.Me.Discard.Any(c => c.Def.IsEnergy)) return true;
                var picks = ctx.ChoosePokemonMany(rapid, 0, Math.Min(2, rapid.Count), "Max Blaze: choose up to 2 of your Benched Rapid Strike Pokémon (an Energy card from your discard pile for each)");
                foreach (var mon in picks)
                {
                    var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                    if (energy.Count == 0) break;
                    var card = ctx.ChooseCards(energy, 1, 1, "Max Blaze: choose an Energy card to attach to " + mon.Name)[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, mon);
                }
                return true;
            }
        }

        /// <summary>Cross Fist: if another of your Rapid Strike Pokémon attacked during your last turn, also 160 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class CrossFistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross Fist") return false;
                var me = ctx.Me;
                bool partner = me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackerDef != null && me.LastAttackerDef.IsRapidStrike
                               && me.LastAttackerUid != ctx.Pokemon.Uid;
                ctx.DamageDefending(atk.Damage);
                if (partner) ctx.DamageOneBenched(160);
                return true;
            }
        }

        /// <summary>Lustrous Body: prevent all effects of the opponent's Pokémon's Abilities done to this Pokémon. G-Max Hurricane: can't use it next turn.</summary>
        private sealed class CorviknightVmax : Mon
        {
            public override bool ShieldsFromOpponentsAbilities { get { return true; } }

            private static readonly Mon hurricane = new LockAfterAttackMon("G-Max Hurricane", true);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Ability;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return hurricane.HandleAttack(ctx, atk); }
        }

        /// <summary>Natural Cure: attaching an Energy card from your hand to it removes its Special Conditions. Blissful Blast: 30 more per Energy, then up to 3 Energy cards from the discard pile.</summary>
        private sealed class BlisseyV : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || target.Conditions == SpecialCondition.None) return;
                Announce(src, "Natural Cure");
                src.Engine.RemoveAllConditions(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blissful Blast") return false;
                int done = ctx.DamageDefending(atk.Damage + 30 * ctx.EnergyCount(ctx.Pokemon));
                if (done <= 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(3, energy.Count), "Blissful Blast: attach up to 3 Energy cards from your discard pile to " + ctx.Pokemon.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                return true;
            }
        }

        // ================================================================== Supporters

        /// <summary>Brandon: only as the last card in your hand; draw a card for each Benched Pokémon (both players').</summary>
        private sealed class Brandon : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count == 1 && ctx.Me.Hand[0] == ctx.Card && CardsCRZ1.AllBenched(ctx) > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(CardsCRZ1.AllBenched(ctx));
            }
        }

        /// <summary>Candice: look at the top 7 cards; put any number of Water Pokémon and basic Water Energy among them into your hand; shuffle the rest back.</summary>
        private sealed class Candice : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var water = top.Where(c => (c.Def.IsPokemon && c.Def.HasType(EnergyType.Water)) || CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                var picks = ctx.ChooseCards(water, 0, water.Count, "Candice: put any number of Water Pokémon and Water Energy cards into your hand");
                if (picks.Count > 0)
                {
                    ctx.DeckToHand(picks);
                    ctx.Engine.Reveal(ctx.Player, picks, "revealed");
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Furisode Girl: put a Basic Pokémon from your deck onto your Bench; you may switch it with your Active Pokémon.</summary>
        private sealed class FurisodeGirl : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchToBench(d => true, 1, "Furisode Girl: choose a Basic Pokémon to put onto your Bench");
                if (found.Count == 0 || ctx.Me.Active == null) return;
                var mon = ctx.Me.Bench.FirstOrDefault(m => m.Stack.Contains(found[0]));
                if (mon != null && ctx.AskYesNo("Furisode Girl: switch " + mon.Name + " with your Active Pokémon?")) ctx.Engine.SwitchActive(ctx.Player, mon);
            }
        }

        /// <summary>Gym Trainer: draw 2 cards, or 4 if any of your Pokémon were Knocked Out during your opponent's last turn.</summary>
        private sealed class GymTrainer : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(ctx.WasKnockedOutLastOpponentTurn() ? 4 : 2);
            }
        }

        /// <summary>Lance: search your deck for up to 3 Dragon Pokémon.</summary>
        private sealed class Lance : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.HasType(EnergyType.Dragon), 3, "Lance: choose up to 3 Dragon Pokémon");
            }
        }

        /// <summary>Professor Laventon: put up to 3 Pokémon with "Hisuian" in their names from your discard pile into your hand.</summary>
        private sealed class ProfessorLaventon : CardScript
        {
            private static List<CardInstance> Hisuian(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.Name.Contains("Hisuian")).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hisuian(ctx).Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pile = Hisuian(ctx);
                var picks = ctx.ChooseCards(pile, 1, Math.Min(3, pile.Count), "Professor Laventon: put up to 3 Hisuian Pokémon from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>Serena: discard 1-3 cards from your hand and draw until you have 5, or switch in 1 of the opponent's Benched Pokémon V.</summary>
        private sealed class Serena : CardScript
        {
            private static bool CanDraw(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            private static List<PokemonInPlay> VTargets(EffectContext ctx)
            {
                return ctx.GustTargets().Where(m => m.Def.IsPokemonV).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return CanDraw(ctx) || VTargets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool draw = ctx.Me.Hand.Count > 0;
                bool gust = VTargets(ctx).Count > 0;
                int pick = draw && gust
                    ? ctx.ChooseOption("Serena: choose 1", "Discard up to 3 cards and draw until you have 5", "Switch in 1 of your opponent's Benched Pokémon V")
                    : (draw ? 0 : 1);
                if (pick == 0)
                {
                    if (ctx.Me.Hand.Count == 0) return;
                    var discards = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, Math.Min(3, ctx.Me.Hand.Count), "Serena: discard up to 3 cards from your hand (at least 1)");
                    foreach (var c in discards) ctx.Engine.DiscardFromHand(ctx.Player, c);
                    ctx.DrawUntil(5);
                    return;
                }
                var targets = VTargets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Serena: choose 1 of your opponent's Benched Pokémon V to switch with their Active Pokémon");
                ctx.Engine.SwitchActive(ctx.Opponent, target);
            }
        }

        /// <summary>Wallace: draw 3 cards; your opponent may draw a card, and if they do you draw 1 more.</summary>
        private sealed class Wallace : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                if (ctx.Opp.Deck.Count == 0) return;
                if (!ctx.Engine.AskYesNo(ctx.Opponent, "Wallace: your opponent drew 3 cards. Draw a card? (If you do, they draw 1 more.)", ctx.Source)) return;
                if (ctx.Engine.DrawCards(ctx.Opponent, 1) > 0) ctx.Draw(1);
            }
        }

        /// <summary>Worker: draw 3 cards, then discard a Stadium in play.</summary>
        private sealed class Worker : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
            }
        }

        /// <summary>Friends in Galar: draw 3 cards.</summary>
        private sealed class DrawThree : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>Gordie: look at the top 7 cards, put any number of Energy cards among them into your hand, shuffle the rest back.</summary>
        private sealed class Gordie : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                var picks = ctx.ChooseCards(energy, 0, energy.Count, "Gordie: put any number of Energy cards into your hand");
                if (picks.Count > 0)
                {
                    ctx.DeckToHand(picks);
                    ctx.Engine.Reveal(ctx.Player, picks, "revealed");
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Professor Burnet: search your deck for up to 2 cards and discard them.</summary>
        private sealed class ProfessorBurnet : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => true, 2, "Professor Burnet: choose up to 2 cards to discard", false);
                foreach (var c in found) ctx.Me.Discard.Add(c);
                if (found.Count > 0) ctx.Engine.Reveal(ctx.Player, found, "discarded");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Sordward &amp; Shielbert: choose a Trainer card in your discard pile; if your opponent agrees, put it into your hand, otherwise draw 3 cards.</summary>
        private sealed class SordwardAndShielbert : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsTrainer); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Sordward & Shielbert: choose a Trainer card from your discard pile")[0];
                // Milotic's Dew Guard: the opponent can't be asked, so the card does nothing (Compendium, 2022-11-10).
                if (ctx.Engine.SupporterEffectsPreventedOn(ctx.Opponent))
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't be asked (Dew Guard), so Sordward & Shielbert does nothing.");
                    return;
                }
                bool yes = ctx.Engine.AskYesNo(ctx.Opponent, "Sordward & Shielbert: may your opponent put " + pick.Def.Name + " from their discard pile into their hand? (If you say no, they draw 3 cards.)", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " said " + (yes ? "yes" : "no") + " to " + pick.Def.Name + ".");
                if (yes) ctx.DiscardToHand(new List<CardInstance> { pick });
                else ctx.Draw(3);
            }
        }

        // ================================================================== Items

        /// <summary>Capturing Aroma: flip a coin; heads: search for an Evolution Pokémon, tails: a Basic Pokémon.</summary>
        private sealed class CapturingAroma : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.SearchToHand(d => d.IsEvolution, 1, "Capturing Aroma (heads): choose an Evolution Pokémon");
                else ctx.SearchToHand(d => d.IsBasicPokemon, 1, "Capturing Aroma (tails): choose a Basic Pokémon");
            }
        }

        /// <summary>
        /// Quad Stone: "You may use 4 Quad Stone cards at once." One card heals 10 damage from your Active Pokémon; 4 at once
        /// heal all damage from each of your Pokémon (once for the 4 cards).
        /// </summary>
        private sealed class QuadStone : CardScript
        {
            private static List<CardInstance> Others(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.Name == "Quad Stone").ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0) return true;
                return Others(ctx).Count >= 3 && ctx.Me.InPlay().Any(m => m.DamageCounters > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = Others(ctx);
                if (others.Count >= 3 && ctx.AskYesNo("Quad Stone: use 4 Quad Stone cards at once to heal all damage from each of your Pokémon?"))
                {
                    foreach (var c in others.Take(3))
                    {
                        ctx.Me.Hand.Remove(c);
                        ctx.Me.Discard.Add(c);
                    }
                    ctx.Engine.Emit(GameEventType.PlayTrainer, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " used 4 Quad Stone cards at once.", ctx.Card.Id);
                    foreach (var mon in ctx.MyPokemon()) ctx.Heal(mon, mon.DamageCounters * 10);
                    return;
                }
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 10);
            }
        }

        // ================================================================== Pokémon Tools

        /// <summary>Earthen Seal Stone: the Pokémon V holding it can use the VSTAR Power attack Star Gravity: damage counters on each opposing Pokémon V until it has 100 HP left.</summary>
        private sealed class EarthenSealStone : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !mon.Def.IsPokemonV || src.Card.Def.Attacks.Count == 0)
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                foreach (var mon in ctx.OpponentPokemon(m => m.Def.IsPokemonV))
                {
                    int over = ctx.RemainingHp(mon) - 100;
                    if (over >= 10) ctx.PlaceCounters(mon, over / 10);
                }
            }
        }

        /// <summary>Emergency Jelly: at the end of each turn, if its Pokémon has 30 HP or less left and damage counters, heal 120 damage from it and discard this card.</summary>
        private sealed class EmergencyJelly : CardScript
        {
            public override void OnEndOfEachTurn(EffectContext src)
            {
                var mon = src.Pokemon;
                if (mon == null || !mon.HasTool(src.Card) || mon.DamageCounters == 0 || src.RemainingHp(mon) > 30) return;
                int healed = src.Heal(mon, 120);
                if (healed > 0) src.Engine.DiscardTool(mon, src.Card);
            }
        }

        /// <summary>Forest Seal Stone: the Pokémon V holding it can use the VSTAR Power Star Alchemy: search your deck for a card.</summary>
        private sealed class ForestSealStone : CardScript
        {
            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return abilityIndex == 0 && ctx.IsMyTurn && ctx.Pokemon != null && ctx.Pokemon.Def.IsPokemonV && !ctx.Me.VstarUsed && ctx.Me.Deck.Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                ctx.SearchToHand(d => true, 1, "Star Alchemy: choose a card to put into your hand", false, 1);
            }
        }

        /// <summary>Leafy Camo Poncho: prevent all effects of the opponent's Supporter cards done to the Pokémon VSTAR or VMAX holding it.</summary>
        private sealed class LeafyCamoPoncho : CardScript
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return trainer != null && trainer.IsSupporter && target == src.Pokemon && (target.Def.IsVstar || target.Def.IsVmax);
            }
        }

        // ================================================================== Stadiums

        /// <summary>Primordial Altar: once during each player's turn, they may look at the top card of their deck and may discard it.</summary>
        private sealed class PrimordialAltar : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return false;
                if (ctx.ChooseOption("Primordial Altar: the top card of your deck is " + top[0].Def.Name + ".", "Discard it", "Leave it on top") == 0)
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                return false;
            }
        }

        /// <summary>Gapejaw Bog: whenever either player puts a Basic Pokémon from their hand onto their Bench, 2 damage counters on it.</summary>
        private sealed class GapejawBog : CardScript
        {
            public override void OnPokemonPutOnBench(EffectContext ctx, PokemonInPlay benched)
            {
                if (!ctx.Engine.BenchingFromHand || !benched.IsBasic) return;
                ctx.Engine.PlaceDamageCounters(benched, 2, "Gapejaw Bog");
            }
        }

        // ================================================================== Special Energy

        /// <summary>Regenerative Energy: Colorless. Whenever you play a Pokémon from your hand to evolve the Pokémon V it's attached to, heal 100 damage from that Pokémon.</summary>
        private sealed class RegenerativeEnergy : CardScript
        {
            public override void OnHolderEvolvedFromHand(EffectContext src, CardDef before)
            {
                var mon = src.Pokemon;
                if (mon == null || before == null || !before.IsPokemonV || mon.Owner != src.Player || mon.DamageCounters == 0) return;
                src.Heal(mon, 100);
            }
        }

        /// <summary>V Guard Energy: Colorless. Its Pokémon takes 30 less damage from attacks by the opponent's Pokémon V (doesn't stack).</summary>
        private sealed class VGuardEnergy : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || info.Attacker == null || !info.AgainstOpponent || !info.Attacker.Def.IsPokemonV) return damage;
                return info.ApplyOnce("V Guard Energy") ? damage - 30 : damage;
            }
        }
    }
}
