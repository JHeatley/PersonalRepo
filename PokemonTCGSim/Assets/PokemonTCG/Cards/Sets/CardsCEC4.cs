using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Cosmic Eclipse (CEC) part 4: Trainers, Draw Energy and the gold Stadiums (CEC 185-209, 263, 268, 269). Lillie's Poké Doll is
    /// played like Unidentified Fossil (the engine does it; its script is empty). Rulings used (Compendium): TAG TEAM Supporters resolve in printed order,
    /// with the "when you play this card" discard made as the card is played, and a part that can't happen doesn't stop the other
    /// (2019-10-31); Erika is playable even if nobody draws; Will works on an attack effect's coin flip but not on a Special
    /// Condition's; each Pokémon Roxie discards uses its own Blow-Away Bomb; Chaotic Swell's discarded Stadium still used up the
    /// Stadium play for the turn; Island Challenge Amulet's reduction is only for Knock Outs by damage from the opponent's attacks;
    /// Giant Hearth (old wording) does as much as it can, even with an empty deck (2019-08-01); Martial Arts Dojo's +40 is only to
    /// the opponent's Active Pokémon (2019-05-09). Lana's Fishing Rod can be played with only a Pokémon or only a Pokémon Tool card
    /// in the discard pile (your ruling, v68). Red &amp; Blue can evolve a Pokémon that already evolved this turn (your ruling for
    /// Pokémon Breeder's Nurturing).
    /// </summary>
    public static class CardsCEC4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Beastite", new Beastite());
            r.RegisterByName("Bellelba & Brycen-Man", new BellelbaBrycenMan());
            r.RegisterByName("Chaotic Swell", new ChaoticSwell());
            r.RegisterByName("Clay", new Clay());
            r.RegisterByName("Cynthia & Caitlin", new CynthiaCaitlin());
            r.RegisterByName("Dragonium Z: Dragon Claw", new DragoniumZDragonClaw());
            r.RegisterByName("Erika", new Erika());
            r.RegisterByName("Great Catcher", new GreatCatcher());
            r.RegisterByName("Guzma & Hala", new GuzmaHala());
            r.RegisterByName("Island Challenge Amulet", new IslandChallengeAmulet());
            r.RegisterByName("Lana's Fishing Rod", new LanasFishingRod());
            r.RegisterByName("Lillie's Full Force", new LilliesFullForce());
            r.RegisterByName("Lillie's Poké Doll", new CardScript()); // played like Unidentified Fossil (engine)
            r.RegisterByName("Mallow & Lana", new MallowLana());
            r.RegisterByName("Misty & Lorelei", new MistyLorelei());
            r.RegisterByName("N's Resolve", new NsResolve());
            r.RegisterByName("Professor Oak's Setup", new ProfessorOaksSetup());
            r.RegisterByName("Red & Blue", new RedBlue());
            r.RegisterByName("Roller Skater", new RollerSkater());
            r.RegisterByName("Rosa", new Rosa());
            r.RegisterByName("Roxie", new Roxie());
            r.RegisterByName("Tag Call", new TagCall());
            r.RegisterByName("Will", new Will());
            r.RegisterByName("Draw Energy", new DrawEnergy());
            r.RegisterByName("Giant Hearth", new GiantHearth());
            r.RegisterByName("Martial Arts Dojo", new MartialArtsDojo());
            r.RegisterByName("Power Plant", new PowerPlant());
        }

        // ================================================================== Shared helpers

        private static bool IsStadiumCard(CardDef d) { return d.IsStadiumCard; }
        private static bool IsToolCard(CardDef d) { return d.IsTool; }
        private static bool IsSpecialEnergyCard(CardDef d) { return d.IsSpecialEnergy; }
        private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }
        private static bool IsWaterEnergyCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Water); }
        private static bool IsFireEnergyCard(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Fire); }

        /// <summary>Cards in the hand other than the Trainer being played (it's still in the hand while legality is checked).</summary>
        private static int OthersInHand(EffectContext ctx)
        {
            int n = 0;
            foreach (var card in ctx.Me.Hand) if (card != ctx.Card) n++;
            return n;
        }

        /// <summary>Discards chosen cards from the hand.</summary>
        private static List<CardInstance> DiscardChosen(EffectContext ctx, int count, string prompt)
        {
            if (ctx.Me.Hand.Count < count) return new List<CardInstance>();
            var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), count, count, prompt);
            foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
            return picks;
        }

        // ================================================================== Pokémon Tools

        /// <summary>Beastite: the Ultra Beast's attacks do 10 more damage to the opponent's Active Pokémon for each Prize card you have taken.</summary>
        private sealed class Beastite : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                if (!src.Pokemon.Def.IsUltraBeast) return damage;
                return damage + 10 * src.MyPrizesTaken;
            }
        }

        /// <summary>
        /// Dragonium Z: Dragon Claw: a holder with the Dragon Claw attack can use Destructive Drake-GX ([C][C][C]): discard all basic
        /// Energy from it, 80 damage for each card discarded. It's a GX attack (1 per game).
        /// </summary>
        private sealed class DragoniumZDragonClaw : CardScript
        {
            private static bool HasDragonClaw(PokemonInPlay mon)
            {
                foreach (var atk in mon.Def.Attacks) if (atk.Name == "Dragon Claw") return true;
                return false;
            }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0 || !HasDragonClaw(mon))
                    return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                var basics = me.Energy.Where(x => x.Def.IsBasicEnergy).ToList();
                int n = 0;
                foreach (var card in basics) if (ctx.Engine.DiscardEnergy(me, card)) n++;
                if (n > 0) ctx.DamageDefending(80 * n);
            }
        }

        /// <summary>
        /// Island Challenge Amulet: a Pokémon-GX or Pokémon-EX holder gets -100 HP, and when it's Knocked Out by damage from an opponent's
        /// attack, that player takes 1 fewer Prize card.
        /// </summary>
        private sealed class IslandChallengeAmulet : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.IsGXorEX ? hp - 100 : hp;
            }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut != src.Pokemon || !knockedOut.Def.IsGXorEX || taker == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                src.Log("Island Challenge Amulet: " + src.Engine.PlayerName(taker) + " takes 1 fewer Prize card.");
                return prizes - 1;
            }
        }

        // ================================================================== Items

        /// <summary>Great Catcher: only by discarding 2 other cards; switch 1 of the opponent's Benched Pokémon-GX or Pokémon-EX into the Active Spot.</summary>
        private sealed class GreatCatcher : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.GustTargets().Where(m => m.Def.IsGXorEX).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return OthersInHand(ctx) >= 2 && ctx.Opp.Active != null && Targets(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                DiscardChosen(ctx, 2, "Great Catcher: discard 2 cards from your hand");
                var targets = Targets(ctx);
                if (targets.Count == 0 || ctx.Opp.Active == null) return;
                var pick = ctx.ChoosePokemon(targets, "Great Catcher: choose 1 of your opponent's Benched Pokémon-GX or Pokémon-EX to switch in");
                ctx.Engine.SwitchActive(ctx.Opponent, pick);
            }
        }

        /// <summary>Lana's Fishing Rod: shuffle a Pokémon and a Pokémon Tool card from your discard pile into your deck (either one is enough).</summary>
        private sealed class LanasFishingRod : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(x => x.Def.IsPokemon || x.Def.IsTool);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var picks = new List<CardInstance>();
                var mons = ctx.Me.Discard.Where(x => x.Def.IsPokemon).ToList();
                if (mons.Count > 0) picks.AddRange(ctx.ChooseCards(mons, 1, 1, "Lana's Fishing Rod: choose a Pokémon to shuffle into your deck"));
                var tools = ctx.Me.Discard.Where(x => x.Def.IsTool).ToList();
                if (tools.Count > 0) picks.AddRange(ctx.ChooseCards(tools, 1, 1, "Lana's Fishing Rod: choose a Pokémon Tool card to shuffle into your deck"));
                var moved = ctx.DiscardToDeck(picks);
                if (moved.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " shuffled " + string.Join(" and ", moved.Select(x => x.Def.Name).ToArray()) + " into their deck.");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Tag Call: search your deck for up to 2 TAG TEAM cards, reveal them, put them into your hand, shuffle.</summary>
        private sealed class TagCall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(CardsCEC3.IsTagTeamCard, 2, "Tag Call: choose up to 2 TAG TEAM cards");
            }
        }

        // ================================================================== Supporters

        /// <summary>
        /// Bellelba &amp; Brycen-Man: discard the top 3 cards of each player's deck. Discarding 3 other cards as it's played: each player
        /// discards Benched Pokémon until they have 3 (the opponent first; each player chooses their own). Playable if either deck has
        /// a card, or if a player has more than 3 Benched Pokémon and you can discard 3.
        /// </summary>
        private sealed class BellelbaBrycenMan : CardScript
        {
            private static bool BigBench(EffectContext ctx) { return ctx.Me.Bench.Count > 3 || ctx.Opp.Bench.Count > 3; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || ctx.Opp.Deck.Count > 0 || (OthersInHand(ctx) >= 3 && BigBench(ctx));
            }

            private static void Trim(EffectContext ctx, int player)
            {
                var p = ctx.State.Players[player];
                while (p.Bench.Count > 3)
                {
                    var pick = ctx.Engine.ChoosePokemon(player, p.Bench.ToList(), "Bellelba & Brycen-Man: discard a Benched Pokémon (down to 3)", ctx.Source);
                    if (pick == null) return;
                    ctx.Engine.DiscardPokemonFromPlay(pick);
                }
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool bonus = BigBench(ctx) && CardsCEC1.TagTeamBonus(ctx, 3, "Bellelba & Brycen-Man: discard 3 other cards so each player discards Benched Pokémon until they have 3?");
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                if (!bonus) return;
                Trim(ctx, ctx.Opponent);
                Trim(ctx, ctx.Player);
            }
        }

        /// <summary>Clay: discard the top 7 cards of your deck; put the Item cards among them into your hand.</summary>
        private sealed class Clay : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 7);
                var items = top.Where(x => x.Def.IsItem && ctx.Me.Discard.Contains(x)).ToList();
                ctx.DiscardToHand(items);
            }
        }

        /// <summary>
        /// Cynthia &amp; Caitlin: put a Supporter (not Cynthia &amp; Caitlin, nor the card discarded for it) from your discard pile into your
        /// hand. Discarding another card as it's played: then draw 3. Playable with such a Supporter in the discard pile, or with a
        /// card to discard and a card in the deck.
        /// </summary>
        private sealed class CynthiaCaitlin : CardScript
        {
            private static bool Retrievable(CardInstance card, List<CardInstance> excluded)
            {
                return card.Def.IsSupporter && card.Def.Name != "Cynthia & Caitlin" && !excluded.Contains(card);
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var none = new List<CardInstance>();
                return ctx.Me.Discard.Any(x => Retrievable(x, none)) || (OthersInHand(ctx) >= 1 && ctx.Me.Deck.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int before = ctx.Me.Discard.Count;
                bool bonus = ctx.Me.Deck.Count > 0 && CardsCEC1.TagTeamBonus(ctx, 1, "Cynthia & Caitlin: discard another card to draw 3 cards?");
                var excluded = ctx.Me.Discard.Skip(before).ToList();
                var supporters = ctx.Me.Discard.Where(x => Retrievable(x, excluded)).ToList();
                if (supporters.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(supporters, 1, 1, "Cynthia & Caitlin: put which Supporter from your discard pile into your hand?"));
                if (bonus) ctx.Draw(3);
            }
        }

        /// <summary>Erika: each player may draw up to 3 cards, you first. Playable even if nobody draws.</summary>
        private sealed class Erika : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int mine = ctx.Me.Deck.Count == 0 ? 0 : ctx.AskNumber(0, Math.Min(3, ctx.Me.Deck.Count), "Erika: draw how many cards (up to 3)?");
                if (mine > 0) ctx.Draw(mine);
                int theirs = ctx.Opp.Deck.Count == 0 ? 0
                    : ctx.Engine.AskNumber(ctx.Opponent, 0, Math.Min(3, ctx.Opp.Deck.Count), "Erika: draw how many cards (up to 3)?", ctx.Source);
                if (theirs > 0) ctx.Engine.DrawCards(ctx.Opponent, theirs);
            }
        }

        /// <summary>
        /// Guzma &amp; Hala: search for a Stadium card. Discarding 2 other cards as it's played: also a Pokémon Tool card and a Special
        /// Energy card. The discard comes before the search; one shuffle at the end. Needs a card in the deck.
        /// </summary>
        private sealed class GuzmaHala : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool bonus = CardsCEC1.TagTeamBonus(ctx, 2, "Guzma & Hala: discard 2 other cards to also search for a Pokémon Tool card and a Special Energy card?");
                var found = ctx.SearchMyDeck(IsStadiumCard, 1, "Guzma & Hala: choose a Stadium card");
                if (bonus)
                {
                    found.AddRange(ctx.SearchMyDeck(IsToolCard, 1, "Guzma & Hala: choose a Pokémon Tool card"));
                    found.AddRange(ctx.SearchMyDeck(IsSpecialEnergyCard, 1, "Guzma & Hala: choose a Special Energy card"));
                }
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Lillie's Full Force: draw 4 cards. At the end of this turn, with 3 or more cards in your hand, you keep 2 of your choice and
        /// shuffle the rest into your deck.
        /// </summary>
        private sealed class LilliesFullForce : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(4);
                ctx.AddEffect(new FullForceEndEffect("At the end of this turn, " + ctx.Engine.PlayerName(ctx.Player) + " shuffles their hand down to 2 cards (Lillie's Full Force)", ctx.Player, ctx.Turn));
            }
        }

        private sealed class FullForceEndEffect : ContinuousEffect
        {
            public FullForceEndEffect(string name, int owner, int turn) : base(name, owner, -1, turn, false) { }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn || engine.State.CurrentPlayer != Owner) return;
                var p = engine.State.Players[Owner];
                if (p.Hand.Count < 3) return;
                var keep = engine.ChooseCards(Owner, p.Hand.ToList(), 2, 2, "Lillie's Full Force: choose the 2 cards to keep in your hand", null);
                int n = 0;
                foreach (var card in p.Hand.ToList())
                {
                    if (keep.Contains(card)) continue;
                    p.Hand.Remove(card);
                    p.Deck.Add(card);
                    n++;
                }
                engine.Log(Owner, "Lillie's Full Force: " + engine.PlayerName(Owner) + " shuffled " + n + (n == 1 ? " card" : " cards") + " from their hand into their deck.");
                engine.ShuffleDeck(Owner);
            }
        }

        /// <summary>
        /// Mallow &amp; Lana: switch your Active Pokémon with 1 of your Benched Pokémon. Discarding 2 other cards as it's played (only with a
        /// damaged Active Pokémon): heal 120 damage from the Pokémon moved to the Bench, after the switch.
        /// </summary>
        private sealed class MallowLana : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var old = ctx.Me.Active;
                bool bonus = old != null && old.DamageCounters > 0
                             && CardsCEC1.TagTeamBonus(ctx, 2, "Mallow & Lana: discard 2 other cards to heal 120 damage from " + old.Name + " after the switch?");
                if (!ctx.SwitchMyActive("Mallow & Lana: choose a Benched Pokémon to switch in")) return;
                if (bonus && ctx.InPlay(old) && ctx.Me.Bench.Contains(old)) ctx.Heal(old, 120);
            }
        }

        /// <summary>
        /// Misty &amp; Lorelei: search for up to 3 [W] Energy cards. Discarding 5 other cards as it's played (only after your GX attack was
        /// used; before the search): this turn your [W] Pokémon can use GX attacks anyway.
        /// </summary>
        private sealed class MistyLorelei : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || (ctx.Me.GxUsed && OthersInHand(ctx) >= 5);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.GxUsed && CardsCEC1.TagTeamBonus(ctx, 5, "Misty & Lorelei: discard 5 other cards so your Water Pokémon can use GX attacks this turn?"))
                {
                    ctx.Me.WaterGxThisTurn = true;
                    ctx.Log("Misty & Lorelei: during this turn, " + ctx.Engine.PlayerName(ctx.Player) + "'s Water Pokémon can use their GX attacks.");
                }
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(IsWaterEnergyCard, 3, "Misty & Lorelei: choose up to 3 Water Energy cards");
            }
        }

        /// <summary>N's Resolve: discard the top 6 cards of your deck; attach the basic Energy among them to 1 of your Benched [N] Pokémon.</summary>
        private sealed class NsResolve : CardScript
        {
            private static List<PokemonInPlay> Dragons(EffectContext ctx)
            {
                return ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Dragon)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Dragons(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 6);
                var energy = top.Where(x => x.Def.IsBasicEnergy && ctx.Me.Discard.Contains(x)).ToList();
                var dragons = Dragons(ctx);
                if (energy.Count == 0 || dragons.Count == 0) return;
                var target = ctx.ChoosePokemon(dragons, "N's Resolve: attach the basic Energy to which Benched Dragon Pokémon?");
                foreach (var card in energy)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>Professor Oak's Setup: search for up to 3 Basic Pokémon of different types and put them onto your Bench.</summary>
        private sealed class ProfessorOaksSetup : CardScript
        {
            private static bool SharesType(CardDef a, CardDef b)
            {
                return a.HasType(b.PokemonType) || (b.SecondType.HasValue && a.HasType(b.SecondType.Value));
            }

            private static bool Fits(EffectContext ctx, CardDef d, List<CardDef> chosen)
            {
                if (!d.IsBasicPokemon || ctx.Engine.PutIntoPlayBlocked(ctx.Player, d)) return false;
                foreach (var other in chosen) if (SharesType(other, d)) return false;
                return true;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var chosen = new List<CardDef>();
                int max = Math.Min(3, ctx.BenchSpace);
                for (int i = 0; i < max && ctx.BenchSpace > 0; i++)
                {
                    var sofar = chosen.ToList();
                    var found = ctx.SearchMyDeck(d => Fits(ctx, d, sofar), 1,
                        "Professor Oak's Setup: choose a Basic Pokémon of a different type (" + (i + 1) + " of up to " + max + ")");
                    if (found.Count == 0) break;
                    chosen.Add(found[0].Def);
                    ctx.Engine.PutOntoBench(ctx.Player, found[0], false);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Red &amp; Blue: search for a Pokémon-GX that evolves from 1 of your Pokémon and evolve it. Not during your first turn, nor on a
        /// Pokémon put into play this turn. Discarding 2 other cards as it's played: search for up to 2 basic Energy cards and attach
        /// them to the Pokémon evolved this way.
        /// </summary>
        private sealed class RedBlue : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.TurnEnteredPlay != ctx.Turn && !CardsEVS1.EvolveBlocked(ctx, m));
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return !ctx.IsMyFirstTurn && ctx.Me.Deck.Count > 0 && Targets(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool bonus = CardsCEC1.TagTeamBonus(ctx, 2, "Red & Blue: discard 2 other cards to also attach up to 2 basic Energy from your deck to the evolved Pokémon?");
                var targets = Targets(ctx);
                PokemonInPlay evolved = null;
                var found = ctx.SearchMyDeck(d => d.IsGX && targets.Any(t => CardsEVS1.EvolvesOnto(ctx, t, d)), 1,
                    "Red & Blue: choose a Pokémon-GX that evolves from 1 of your Pokémon", false);
                if (found.Count > 0)
                {
                    var card = found[0];
                    var onto = targets.Where(t => CardsEVS1.EvolvesOnto(ctx, t, card.Def)).ToList();
                    evolved = ctx.ChoosePokemon(onto, "Red & Blue: evolve which Pokémon into " + card.Def.Name + "?");
                    ctx.Engine.EvolvePokemon(evolved, card);
                }
                if (bonus && evolved != null && ctx.InPlay(evolved))
                {
                    var energy = ctx.SearchMyDeck(CardsCEC1.IsBasicEnergyCard, 2, "Red & Blue: choose up to 2 basic Energy cards to attach to " + evolved.Name);
                    foreach (var e in energy) ctx.Engine.AttachEnergyCard(e, evolved);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Roller Skater: discard a card from your hand; draw 2 cards, or 4 if it was an Energy card.</summary>
        private sealed class RollerSkater : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return OthersInHand(ctx) >= 1 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var picks = DiscardChosen(ctx, 1, "Roller Skater: discard a card from your hand");
                if (picks.Count == 0 || !ctx.Me.Discard.Contains(picks[0])) return;
                ctx.Draw(picks[0].Def.IsEnergy ? 4 : 2);
            }
        }

        /// <summary>Rosa: only if 1 of your Pokémon was Knocked Out during your opponent's last turn; search for a Pokémon, a Trainer card and a basic Energy card.</summary>
        private sealed class Rosa : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.WasKnockedOutLastOpponentTurn() && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(IsPokemonCard, 1, "Rosa: choose a Pokémon"));
                found.AddRange(ctx.SearchMyDeck(IsTrainerCard, 1, "Rosa: choose a Trainer card"));
                found.AddRange(ctx.SearchMyDeck(CardsCEC1.IsBasicEnergyCard, 1, "Rosa: choose a basic Energy card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Roxie: discard up to 2 Pokémon that aren't Pokémon-GX or Pokémon-EX from your hand, draw 3 cards for each. Then each
        /// discarded Koffing or Weezing may use its Blow-Away Bomb: 1 damage counter on each of the opponent's Pokémon (they stack).
        /// </summary>
        private sealed class Roxie : CardScript
        {
            private static bool Discardable(CardInstance card) { return card.Def.IsPokemon && !card.Def.IsGXorEX; }

            private static bool HasBlowAwayBomb(CardDef d)
            {
                foreach (var ab in d.Abilities) if (ab.Name == "Blow-Away Bomb") return true;
                return false;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(Discardable); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var eligible = ctx.Me.Hand.Where(Discardable).ToList();
                if (eligible.Count == 0) return;
                var picks = ctx.ChooseCards(eligible, 0, Math.Min(2, eligible.Count), "Roxie: discard up to 2 Pokémon (not Pokémon-GX or Pokémon-EX) from your hand");
                var gone = new List<CardInstance>();
                foreach (var card in picks)
                {
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                    if (ctx.Me.Discard.Contains(card)) gone.Add(card);
                }
                if (gone.Count > 0) ctx.Draw(3 * gone.Count);
                foreach (var bomb in gone)
                {
                    if (!HasBlowAwayBomb(bomb.Def) || !ctx.Me.Discard.Contains(bomb)) continue;
                    if (!ctx.AskYesNo("Blow-Away Bomb (" + bomb.Def.Name + "): put 1 damage counter on each of your opponent's Pokémon?")) continue;
                    ctx.Engine.PushSource(EffectSourceKind.Ability, ctx.Player, null);
                    try
                    {
                        foreach (var mon in ctx.OpponentPokemon()) ctx.Engine.PlaceDamageCounters(mon, 1, "Blow-Away Bomb");
                    }
                    finally { ctx.Engine.PopSource(); }
                }
            }
        }

        /// <summary>Will: the first coin of the next flip this turn for an attack, Ability or Trainer card's effect lands on the side you choose (GameEngine.FlipOne).</summary>
        private sealed class Will : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Me.WillCoinChoice = true;
                ctx.Log("Will: " + ctx.Engine.PlayerName(ctx.Player) + " chooses heads or tails for the first coin of their next flip this turn.");
            }
        }

        // ================================================================== Special Energy

        /// <summary>Draw Energy: provides [C]; when attached from your hand to a Pokémon, draw a card.</summary>
        private sealed class DrawEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx) { ctx.Draw(1); }
        }

        // ================================================================== Stadiums

        /// <summary>Chaotic Swell: a Stadium played from the hand while it's in play discards it and is then discarded too (GameEngine.PutStadiumIntoPlay).</summary>
        private sealed class ChaoticSwell : CardScript
        {
            public override bool DiscardsNewStadiums(EffectContext ctx) { return true; }
        }

        /// <summary>Giant Hearth: once during each player's turn, discard a card from your hand to search your deck for up to 2 [R] Energy cards.</summary>
        private sealed class GiantHearth : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                DiscardChosen(ctx, 1, "Giant Hearth: discard a card from your hand");
                ctx.SearchToHand(IsFireEnergyCard, 2, "Giant Hearth: choose up to 2 Fire Energy cards");
                return false;
            }
        }

        /// <summary>
        /// Martial Arts Dojo: attacks of non-Ultra Beast Pokémon with basic [F] Energy attached do 10 more damage to the opponent's
        /// Active Pokémon, or 40 more if the attacking player has more Prize cards left than their opponent.
        /// </summary>
        private sealed class MartialArtsDojo : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (attacker == null || !info.ToOpponentsActive || damage <= 0 || attacker.Def.IsUltraBeast) return damage;
                if (!attacker.Energy.Any(x => CardsEVS1.IsEnergyOf(x.Def, EnergyType.Fighting))) return damage;
                var mine = src.State.Players[attacker.Owner];
                var theirs = src.State.Players[1 - attacker.Owner];
                return damage + (mine.Prizes.Count > theirs.Prizes.Count ? 40 : 10);
            }
        }

        /// <summary>Power Plant: Pokémon-GX and Pokémon-EX in play have no Abilities.</summary>
        private sealed class PowerPlant : CardScript
        {
            public override bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon) { return mon != null && mon.Def.IsGXorEX; }
        }
    }
}
