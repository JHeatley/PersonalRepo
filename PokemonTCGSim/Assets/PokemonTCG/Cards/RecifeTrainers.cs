using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Trainer cards added for the decks of the Recife Regional Championship (Limitless tournament 580).</summary>
    public static class RecifeTrainers
    {
        public static void Register(CardScriptRegistry r)
        {
            // Items
            r.RegisterByName("Switch", new SwitchCard());
            r.RegisterByName("Energy Switch", new EnergySwitch());
            r.RegisterByName("Jumbo Ice Cream", new JumboIceCream());
            r.RegisterByName("Rare Candy", new RareCandy());
            r.RegisterByName("Sacred Ash", new SacredAsh());
            r.RegisterByName("Prime Catcher", new PrimeCatcher());
            r.RegisterByName("Premium Power Pro", new PremiumPowerPro());
            r.RegisterByName("Roto-Stick", new RotoStick());
            r.RegisterByName("Glass Trumpet", new GlassTrumpet());
            r.RegisterByName("Precious Trolley", new PreciousTrolley());
            r.RegisterByName("Bug Catching Set", new BugCatchingSet());
            r.RegisterByName("Pokégear 3.0", new Pokegear());
            r.RegisterByName("Miracle Headset", new MiracleHeadset());
            r.RegisterByName("Secret Box", new SecretBox());
            r.RegisterByName("Wondrous Patch", new WondrousPatch());
            r.RegisterByName("Fighting Gong", new FightingGong());
            r.RegisterByName("Tool Scrapper", new ToolScrapper());
            r.RegisterByName("Energy Recycler", new EnergyRecycler());
            r.RegisterByName("Dusk Ball", new DuskBall());
            r.RegisterByName("Energy Retrieval", new EnergyRetrieval());
            r.RegisterByName("Pokémon Catcher", new PokemonCatcher());
            r.RegisterByName("Mega Signal", new SearchItem(d => d.IsPokemon && d.IsMegaEx, "Mega Signal: choose a Mega Evolution Pokémon ex"));
            r.RegisterByName("Tera Orb", new SearchItem(d => d.IsPokemon && d.IsTera, "Tera Orb: choose a Tera Pokémon"));
            r.RegisterByName("Team Rocket's Transceiver", new SearchItem(d => d.IsSupporter && d.Name.Contains("Team Rocket"), "Choose a Team Rocket Supporter"));
            r.RegisterByName("Hop's Bag", new HopsBag());
            r.RegisterByName("Enhanced Hammer", new EnhancedHammer());
            r.RegisterByName("Strange Timepiece", new StrangeTimepiece());
            r.RegisterByName("Brilliant Blender", new BrilliantBlender());

            // Pokémon Tools
            r.RegisterByName("Air Balloon", new AirBalloon());
            r.RegisterByName("Binding Mochi", new BindingMochi());
            r.RegisterByName("Brave Bangle", new BraveBangle());
            r.RegisterByName("Cynthia's Power Weight", new CynthiasPowerWeight());
            r.RegisterByName("Handheld Fan", new HandheldFan());
            r.RegisterByName("Hero's Cape", new HerosCape());
            r.RegisterByName("Hop's Choice Band", new HopsChoiceBand());
            r.RegisterByName("Lucky Helmet", new LuckyHelmet());
            r.RegisterByName("Maximum Belt", new MaximumBelt());
            r.RegisterByName("Powerglass", new Powerglass());

            // Supporters
            r.RegisterByName("AZ's Tranquility", new AZsTranquility());
            r.RegisterByName("Bianca's Devotion", new BiancasDevotion());
            r.RegisterByName("Briar", new Briar());
            r.RegisterByName("Brock's Scouting", new BrocksScouting());
            r.RegisterByName("Carmine", new Carmine());
            r.RegisterByName("Ciphermaniac's Codebreaking", new CiphermaniacsCodebreaking());
            r.RegisterByName("Dawn", new Dawn());
            r.RegisterByName("Eri", new Eri());
            r.RegisterByName("Explorer's Guidance", new ExplorersGuidance());
            r.RegisterByName("Gladion's Final Battle", new GladionsFinalBattle());
            r.RegisterByName("Gwynn", new Gwynn());
            r.RegisterByName("Hassel", new Hassel());
            r.RegisterByName("Hilda", new Hilda());
            r.RegisterByName("Janine's Secret Art", new JaninesSecretArt());
            r.RegisterByName("Kieran", new Kieran());
            r.RegisterByName("Lana's Aid", new LanasAid());
            r.RegisterByName("N's Plan", new NsPlan());
            r.RegisterByName("Philippe", new Philippe());
            r.RegisterByName("Pokémon Center Lady", new PokemonCenterLady());
            r.RegisterByName("Ruffian", new Ruffian());
            r.RegisterByName("Salvatore", new Salvatore());
            r.RegisterByName("Surfer", new Surfer());
            r.RegisterByName("Team Rocket's Archer", new TeamRocketsArcher());
            r.RegisterByName("Team Rocket's Ariana", new TeamRocketsAriana());
            r.RegisterByName("Team Rocket's Giovanni", new TeamRocketsGiovanni());
            r.RegisterByName("Team Rocket's Petrel", new SearchItem(d => d.IsTrainer, "Team Rocket's Petrel: choose a Trainer card"));
            r.RegisterByName("Team Rocket's Proton", new TeamRocketsProton());
            r.RegisterByName("Wally's Compassion", new WallysCompassion());
            r.RegisterByName("Xerosic's Machinations", new XerosicsMachinations());

            // Stadiums
            r.RegisterByName("Academy at Night", new AcademyAtNight());
            r.RegisterByName("Area Zero Underdepths", new AreaZeroUnderdepths());
            r.RegisterByName("Battle Cage", new BattleCage());
            r.RegisterByName("Community Center", new CommunityCenter());
            r.RegisterByName("Festival Grounds", new FestivalGrounds());
            r.RegisterByName("Forest of Vitality", new ForestOfVitality());
            r.RegisterByName("Gravity Mountain", new GravityMountain());
            r.RegisterByName("Jamming Tower", new JammingTower());
            r.RegisterByName("Lumiose City", new LumioseCity());
            r.RegisterByName("Nighttime Mine", new NighttimeMine());
            r.RegisterByName("Postwick", new Postwick());
            r.RegisterByName("Prism Tower", new PrismTower());
            r.RegisterByName("Team Rocket's Factory", new TeamRocketsFactory());
        }

        public static bool IsHops(CardDef d) { return d != null && d.IsOwnersPokemon("Hop"); }
        public static bool IsCynthias(CardDef d) { return d != null && d.IsOwnersPokemon("Cynthia"); }
        public static bool IsTeamRocketSupporter(CardDef d) { return d != null && d.IsSupporter && d.Name.Contains("Team Rocket"); }

        /// <summary>Removes the cards from the deck, puts them into the hand and logs it.</summary>
        public static void TakeIntoHand(EffectContext ctx, List<CardInstance> cards)
        {
            if (cards.Count == 0) return;
            ctx.DeckToHand(cards);
            ctx.Engine.Reveal(ctx.Player, cards, "put these cards into " + ctx.Engine.Their(ctx.Player) + " hand");
        }
    }

    // ================================================================== Items

    /// <summary>Generic "search your deck for a card matching X and put it into your hand".</summary>
    public sealed class SearchItem : CardScript
    {
        private readonly Func<CardDef, bool> filter;
        private readonly string prompt;

        public SearchItem(Func<CardDef, bool> filter, string prompt)
        {
            this.filter = filter;
            this.prompt = prompt;
        }

        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToHand(filter, 1, prompt);
        }
    }

    public sealed class SwitchCard : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }
        public override void ResolveTrainer(EffectContext ctx) { ctx.SwitchMyActive("Switch: choose a Benched Pokémon to switch in"); }
    }

    public sealed class EnergySwitch : CardScript
    {
        private static List<PokemonInPlay> Sources(EffectContext ctx)
        {
            return ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy));
        }

        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.MyPokemon().Count >= 2 && Sources(ctx).Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var from = ctx.ChoosePokemon(Sources(ctx), "Energy Switch: move a Basic Energy from which Pokémon?");
            var energy = ctx.ChooseCards(from.Energy.Where(e => e.Def.IsBasicEnergy).ToList(), 1, 1, "Choose the Basic Energy to move")[0];
            var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Energy Switch: move " + energy.Def.Name + " to which Pokémon?");
            ctx.Engine.MoveEnergy(from, to, energy);
        }
    }

    public sealed class JumboIceCream : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            var a = ctx.Me.Active;
            return a != null && a.DamageCounters > 0 && ctx.EnergyCount(a) >= 3;
        }

        public override void ResolveTrainer(EffectContext ctx) { ctx.Heal(ctx.Me.Active, 80); }
    }

    /// <summary>Stage 2 from hand onto a Basic it evolves from (via its Stage 1). Counts as evolving from the hand.</summary>
    public sealed class RareCandy : CardScript
    {
        private static bool Fits(EffectContext ctx, CardDef stage2, PokemonInPlay basic)
        {
            string stage1 = stage2.EvolvesFrom;
            string basicName;
            // Any printing's line counts: Rampardos ex skips a Cranidos that evolves from Antique Skull Fossil, even though
            // other Cranidos cards evolve from other Fossils.
            return stage2.IsPokemon && stage2.Stage == Stage.Stage2 && !string.IsNullOrEmpty(stage1)
                && ((ctx.Engine.PreEvolutionOf.TryGetValue(stage1, out basicName) && basicName == basic.Name)
                    || EvolutionLines.EvolvesFrom(stage1, basic.Name))
                && !ctx.Engine.PokemonPlayBlocked(ctx.Player, stage2); // Potent Glare
        }

        private static List<PokemonInPlay> Targets(EffectContext ctx)
        {
            // An Antique Fossil in play is a Basic Pokémon, so it can be candied (Compendium: Rare Candy + Fossils, 2003-10-16).
            return ctx.MyPokemon(m => m.IsBasic && m.TurnEnteredPlay < ctx.Turn && m.CantEvolveOnTurn != ctx.Turn
                                      && !ctx.Engine.EvolutionFromHandBlocked(m) // Queenly Heel
                                      && ctx.Me.Hand.Any(c => Fits(ctx, c.Def, m)));
        }

        public override bool CanPlayTrainer(EffectContext ctx)
        {
            if (ctx.State.IsPlayersFirstTurn || ctx.Engine.EvolveLocked(ctx.Player)) return false;
            return Targets(ctx).Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var basic = ctx.ChoosePokemon(Targets(ctx), "Rare Candy: choose a Basic Pokémon to evolve");
            var cards = ctx.Me.Hand.Where(c => Fits(ctx, c.Def, basic)).ToList();
            var stage2 = ctx.ChooseCards(cards, 1, 1, "Rare Candy: choose the Stage 2 card")[0];
            ctx.Me.Hand.Remove(stage2);
            ctx.Engine.EvolvePokemon(basic, stage2, true);
        }
    }

    public sealed class SacredAsh : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsPokemon); }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var mons = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
            int n = Math.Min(5, mons.Count);
            var picks = ctx.ChooseCards(mons, n, n, "Sacred Ash: choose " + n + " Pokémon to shuffle into your deck");
            foreach (var c in picks)
            {
                ctx.Me.Discard.Remove(c);
                ctx.Me.Deck.Add(c);
            }
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class PrimeCatcher : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            if (ctx.GustOpponent("Prime Catcher: choose 1 of your opponent's Benched Pokémon to switch in") != null)
                ctx.SwitchMyActive("Prime Catcher: choose your Benched Pokémon to switch in");
        }
    }

    public sealed class PremiumPowerPro : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.AddEffect(new ConditionalDamageBonusEffect("Premium Power Pro: Fighting Pokémon do 30 more damage this turn", ctx.Player, ctx.Turn, 30,
                (e, attacker, target) => e.IsType(attacker, EnergyType.Fighting)));
        }
    }

    public sealed class RotoStick : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var top = ctx.LookAtTop(4);
            var supporters = top.Where(c => c.Def.IsSupporter).ToList();
            var picks = ctx.ChooseCards(supporters, 0, supporters.Count, "Roto-Stick: put any Supporters into your hand");
            RecifeTrainers.TakeIntoHand(ctx, picks);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class GlassTrumpet : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx)
        {
            return ctx.Me.Bench.Where(m => ctx.Engine.IsType(m, EnergyType.Colorless)).ToList();
        }

        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Engine.HasTeraInPlay(ctx.Player) && Targets(ctx).Count > 0 && ctx.BasicEnergyInDiscard().Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var used = new HashSet<int>();
            for (int k = 0; k < 2; k++)
            {
                var energy = ctx.BasicEnergyInDiscard();
                var targets = Targets(ctx).Where(m => !used.Contains(m.Uid)).ToList();
                if (energy.Count == 0 || targets.Count == 0) break;
                var picks = ctx.ChooseCards(energy, k == 0 ? 1 : 0, 1, "Glass Trumpet: choose a Basic Energy to attach (" + (k + 1) + " of up to 2)");
                if (picks.Count == 0) break;
                var target = ctx.ChoosePokemon(targets, "Glass Trumpet: attach " + picks[0].Def.Name + " to which Benched Colorless Pokémon?");
                ctx.Me.Discard.Remove(picks[0]);
                ctx.Engine.AttachEnergyCard(picks[0], target);
                used.Add(target.Uid);
            }
        }
    }

    public sealed class PreciousTrolley : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToBench(d => true, ctx.BenchSpace, "Precious Trolley: put any number of Basic Pokémon onto your Bench");
        }
    }

    public sealed class BugCatchingSet : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var top = ctx.LookAtTop(7);
            var ok = top.Where(c => (c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Grass)
                                    || (c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Grass)).ToList();
            var picks = ctx.ChooseCards(ok, 0, 2, "Bug Catching Set: put up to 2 Grass Pokémon / Basic Grass Energy into your hand");
            RecifeTrainers.TakeIntoHand(ctx, picks);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class Pokegear : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var top = ctx.LookAtTop(7);
            var supporters = top.Where(c => c.Def.IsSupporter).ToList();
            var picks = ctx.ChooseCards(supporters, 0, 1, "Pokégear 3.0: you may put a Supporter into your hand");
            RecifeTrainers.TakeIntoHand(ctx, picks);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class MiracleHeadset : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsSupporter); }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var picks = ctx.ChooseCards(ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList(), 1, 2, "Miracle Headset: put up to 2 Supporters into your hand");
            ctx.DiscardToHand(picks);
        }
    }

    public sealed class SecretBox : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 3; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var discards = ctx.ChooseCards(ctx.Me.Hand.ToList(), 3, 3, "Secret Box: discard 3 cards from your hand");
            foreach (var c in discards) ctx.Engine.DiscardFromHand(ctx.Player, c);
            var found = new List<CardInstance>();
            found.AddRange(ctx.SearchMyDeck(d => d.IsItem, 1, "Secret Box: choose an Item"));
            found.AddRange(ctx.SearchMyDeck(d => d.IsTool, 1, "Secret Box: choose a Pokémon Tool"));
            found.AddRange(ctx.SearchMyDeck(d => d.IsSupporter, 1, "Secret Box: choose a Supporter"));
            found.AddRange(ctx.SearchMyDeck(d => d.IsStadiumCard, 1, "Secret Box: choose a Stadium"));
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class WondrousPatch : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.Me.Bench.Where(m => ctx.Engine.IsType(m, EnergyType.Psychic)).ToList(); }
        private static List<CardInstance> Energy(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Psychic); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0 && Energy(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var energy = ctx.ChooseCards(Energy(ctx), 1, 1, "Wondrous Patch: choose a Basic Psychic Energy from your discard pile")[0];
            var target = ctx.ChoosePokemon(Targets(ctx), "Wondrous Patch: attach " + energy.Def.Name + " to which Benched Psychic Pokémon?");
            ctx.Me.Discard.Remove(energy);
            ctx.Engine.AttachEnergyCard(energy, target);
        }
    }

    public sealed class FightingGong : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToHand(d => (d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fighting) || (d.IsBasicPokemon && d.PokemonType == EnergyType.Fighting),
                1, "Fighting Gong: choose a Basic Fighting Energy or Basic Fighting Pokémon");
        }
    }

    public sealed class ToolScrapper : CardScript
    {
        private static List<PokemonInPlay> WithTools(EffectContext ctx)
        {
            return ctx.MyPokemon(m => m.Tool != null).Concat(ctx.OpponentPokemon(m => m.Tool != null)).ToList();
        }

        public override bool CanPlayTrainer(EffectContext ctx) { return WithTools(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var tools = WithTools(ctx).SelectMany(m => m.Tools).ToList();
            var picks = ctx.ChooseCards(tools, 1, 2, "Tool Scrapper: choose up to 2 Pokémon Tools to discard");
            foreach (var tool in picks)
            {
                var holder = ctx.Engine.ToolHolder(tool);
                if (holder != null) ctx.Engine.DiscardTool(holder, tool);
            }
        }
    }

    public sealed class EnergyRecycler : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var picks = ctx.ChooseCards(ctx.BasicEnergyInDiscard(), 1, 5, "Energy Recycler: shuffle up to 5 Basic Energy into your deck");
            foreach (var c in picks)
            {
                ctx.Me.Discard.Remove(c);
                ctx.Me.Deck.Add(c);
            }
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class DuskBall : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var deck = ctx.Me.Deck;
            int n = Math.Min(7, deck.Count);
            var bottom = deck.Skip(deck.Count - n).ToList();
            var picks = ctx.ChooseCards(bottom.Where(c => c.Def.IsPokemon).ToList(), 0, 1, "Dusk Ball: choose a Pokémon from the bottom 7 cards");
            if (picks.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " revealed " + picks[0].Def.Name + ".");
            RecifeTrainers.TakeIntoHand(ctx, picks);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class EnergyRetrieval : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.DiscardToHand(ctx.ChooseCards(ctx.BasicEnergyInDiscard(), 1, 2, "Energy Retrieval: put up to 2 Basic Energy into your hand"));
        }
    }

    public sealed class PokemonCatcher : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            if (ctx.Flip()) ctx.GustOpponent("Pokémon Catcher: choose 1 of your opponent's Benched Pokémon to switch in");
        }
    }

    public sealed class HopsBag : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToBench(RecifeTrainers.IsHops, 2, "Hop's Bag: put up to 2 Basic Hop's Pokémon onto your Bench");
        }
    }

    public sealed class EnhancedHammer : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy)); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Enhanced Hammer: choose 1 of your opponent's Pokémon");
            var energy = ctx.ChooseCards(target.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList(), 1, 1, "Choose a Special Energy to discard")[0];
            ctx.Engine.DiscardEnergy(target, energy);
        }
    }

    public sealed class StrangeTimepiece : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx)
        {
            return ctx.MyPokemon(m => m.Stack.Count > 1 && ctx.Engine.IsType(m, EnergyType.Psychic));
        }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Strange Timepiece: devolve which Pokémon?");
            int n = ctx.AskNumber(1, target.Stack.Count - 1, "How many Evolution cards to put into your hand?");
            ctx.Engine.Devolve(target, n);
            target.CantEvolveOnTurn = ctx.Turn;
        }
    }

    public sealed class BrilliantBlender : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var found = ctx.SearchMyDeck(d => true, 5, "Brilliant Blender: choose up to 5 cards to discard", false, 1);
            ctx.Me.Discard.AddRange(found);
            if (found.Count > 0)
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", found.Select(c => c.Def.Name).ToArray()) + ".");
            ctx.ShuffleMyDeck();
        }
    }

    // ================================================================== Pokémon Tools

    public sealed class AirBalloon : CardScript
    {
        public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return mon == src.Pokemon ? cost - 2 : cost; }
    }

    public sealed class BindingMochi : CardScript
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            return info.Attacker == src.Pokemon && info.ToOpponentsActive && src.Pokemon.Has(SpecialCondition.Poisoned) ? damage + 40 : damage;
        }
    }

    public sealed class BraveBangle : CardScript
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            return info.Attacker == src.Pokemon && !src.Pokemon.Def.HasRuleBox && info.ToOpponentsActive && info.Target.Def.IsEx ? damage + 30 : damage;
        }
    }

    public sealed class CynthiasPowerWeight : CardScript
    {
        public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return mon == src.Pokemon && RecifeTrainers.IsCynthias(mon.Def) ? hp + 70 : hp; }
    }

    public sealed class HandheldFan : CardScript
    {
        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (!holderWasActive || !ctx.InPlay(attacker) || attacker.Energy.Count == 0) return;
            var bench = ctx.State.Players[attacker.Owner].Bench.ToList();
            if (bench.Count == 0) return;
            var energy = ctx.ChooseCards(attacker.Energy.ToList(), 1, 1, "Handheld Fan: choose an Energy on the Attacking Pokémon to move")[0];
            var to = ctx.ChoosePokemon(bench, "Handheld Fan: move " + energy.Def.Name + " to which of your opponent's Benched Pokémon?");
            ctx.Engine.MoveEnergy(attacker, to, energy);
        }
    }

    public sealed class HerosCape : CardScript
    {
        public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return mon == src.Pokemon ? hp + 100 : hp; }
    }

    public sealed class HopsChoiceBand : CardScript
    {
        public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
        {
            if (mon == src.Pokemon && RecifeTrainers.IsHops(mon.Def)) cost.Remove(EnergyType.Colorless);
        }

        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            return info.Attacker == src.Pokemon && RecifeTrainers.IsHops(src.Pokemon.Def) && info.ToOpponentsActive ? damage + 30 : damage;
        }
    }

    public sealed class LuckyHelmet : CardScript
    {
        public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
        {
            if (holderWasActive) ctx.Draw(2);
        }
    }

    public sealed class MaximumBelt : CardScript
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            return info.Attacker == src.Pokemon && info.ToOpponentsActive && info.Target.Def.IsEx ? damage + 50 : damage;
        }
    }

    public sealed class Powerglass : CardScript
    {
        public override void OnEndOfTurn(EffectContext src)
        {
            if (!src.PokemonIsActive) return;
            var energy = src.BasicEnergyInDiscard();
            if (energy.Count == 0) return;
            var picks = src.ChooseCards(energy, 0, 1, "Powerglass: you may attach a Basic Energy from your discard pile to " + src.Pokemon.Name);
            if (picks.Count == 0) return;
            src.Me.Discard.Remove(picks[0]);
            src.Engine.AttachEnergyCard(picks[0], src.Pokemon);
        }
    }

    // ================================================================== Supporters

    public sealed class AZsTranquility : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var old = ctx.Me.Active;
            if (ctx.SwitchMyActive("AZ's Tranquility: choose a Benched Pokémon to switch in") && old.Def.IsEx) ctx.Heal(old, 80);
        }
    }

    public sealed class BiancasDevotion : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.RemainingHp(m) <= 30); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Bianca's Devotion: heal all damage from which Pokémon?");
            ctx.Heal(target, target.DamageCounters * 10);
        }
    }

    public sealed class Briar : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Prizes.Count == 2; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            int owner = ctx.Player;
            ctx.AddEffect(new ExtraPrizeEffect("Briar: take 1 more Prize card if your Tera Pokémon's attack Knocks Out the opponent's Active Pokémon this turn", owner, ctx.Turn,
                (e, ko) => e.State.IsActive(ko) && ko.HitByAttack && ko.HitByPlayer == owner && ko.HitByAttackerDef != null && ko.HitByAttackerDef.IsTera));
        }
    }

    public sealed class BrocksScouting : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            int pick = ctx.ChooseOption("Brock's Scouting: search for...", "Up to 2 Basic Pokémon", "1 Evolution Pokémon");
            if (pick == 0) ctx.SearchToHand(d => d.IsBasicPokemon, 2, "Brock's Scouting: choose up to 2 Basic Pokémon");
            else ctx.SearchToHand(d => d.IsEvolution, 1, "Brock's Scouting: choose an Evolution Pokémon");
        }
    }

    public sealed class Carmine : CardScript
    {
        // Compendium (Professor Juniper, 2014-11-13): a draw Trainer can't be played with an empty deck (no effect).
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override bool PlayableOnFirstTurn(EffectContext ctx) { return true; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Engine.DiscardHand(ctx.Player);
            ctx.Draw(5);
        }
    }

    public sealed class CiphermaniacsCodebreaking : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var found = ctx.SearchMyDeck(d => true, 2, "Ciphermaniac's Codebreaking: choose 2 cards to put on top of your deck", false, 2);
            ctx.ShuffleMyDeck();
            if (found.Count == 2)
            {
                var top = ctx.ChooseCards(found, 1, 1, "Choose the card to put on the very top")[0];
                var second = found[0] == top ? found[1] : found[0];
                ctx.Me.Deck.Insert(0, second);
                ctx.Me.Deck.Insert(0, top);
            }
            else
            {
                foreach (var c in found) ctx.Me.Deck.Insert(0, c);
            }
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + found.Count + " cards on top of their deck.");
        }
    }

    public sealed class Dawn : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var found = new List<CardInstance>();
            found.AddRange(ctx.SearchMyDeck(d => d.IsBasicPokemon, 1, "Dawn: choose a Basic Pokémon"));
            found.AddRange(ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Stage1, 1, "Dawn: choose a Stage 1 Pokémon"));
            found.AddRange(ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Stage2, 1, "Dawn: choose a Stage 2 Pokémon"));
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class Eri : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            if (TefPokemon.OpponentShielded(ctx)) return;
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " revealed their hand: " + string.Join(", ", ctx.Opp.Hand.Select(c => c.Def.Name).ToArray()) + ".");
            var items = ctx.Opp.Hand.Where(c => c.Def.IsItem).ToList();
            var picks = ctx.ChooseCards(items, 0, 2, "Eri: discard up to 2 Item cards from your opponent's hand");
            foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, c);
        }
    }

    public sealed class ExplorersGuidance : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var top = ctx.LookAtTop(6);
            int n = Math.Min(2, top.Count);
            var keep = ctx.ChooseCards(top, n, n, "Explorer's Guidance: put 2 cards into your hand (the rest are discarded)");
            RecifeTrainers.TakeIntoHand(ctx, keep);
            var rest = top.Where(c => !keep.Contains(c)).ToList();
            foreach (var c in rest)
            {
                ctx.Me.Deck.Remove(c);
                ctx.Me.Discard.Add(c);
            }
            if (rest.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", rest.Select(c => c.Def.Name).ToArray()) + ".");
        }
    }

    public sealed class GladionsFinalBattle : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count == 1 && ctx.Me.Hand[0] == ctx.Card; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.AddEffect(new ConditionalDamageBonusEffect("Gladion's Final Battle: Pokémon without a Rule Box do 80 more damage this turn", ctx.Player, ctx.Turn, 80,
                (e, attacker, target) => !attacker.Def.HasRuleBox));
        }
    }

    public sealed class Gwynn : CardScript
    {
        private static List<CardInstance> Options(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsPokemon && !c.Def.HasRuleBox).ToList(); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Options(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var picks = ctx.ChooseCards(Options(ctx), 1, 2, "Gwynn: discard up to 2 Pokémon without a Rule Box (draw 3 for each)");
            foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
            ctx.Draw(3 * picks.Count);
        }
    }

    public sealed class Hassel : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.WasKnockedOutLastOpponentTurn() && ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var top = ctx.LookAtTop(8);
            var picks = ctx.ChooseCards(top, 0, 3, "Hassel: put up to 3 cards into your hand");
            RecifeTrainers.TakeIntoHand(ctx, picks);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class Hilda : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var found = new List<CardInstance>();
            found.AddRange(ctx.SearchMyDeck(d => d.IsEvolution, 1, "Hilda: choose an Evolution Pokémon"));
            found.AddRange(ctx.SearchMyDeck(d => d.IsEnergy, 1, "Hilda: choose an Energy card"));
            ctx.Me.Hand.AddRange(found);
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class JaninesSecretArt : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Darkness)); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0 && ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var chosen = ctx.ChoosePokemonMany(Targets(ctx), 1, 2, "Janine's Secret Art: choose up to 2 of your Darkness Pokémon");
            bool activeGotEnergy = false;
            foreach (var mon in chosen)
            {
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Darkness, 1, "Choose a Basic Darkness Energy for " + mon.Name);
                if (found.Count == 0) continue;
                ctx.Engine.AttachEnergyCard(found[0], mon);
                if (ctx.IsActive(mon)) activeGotEnergy = true;
            }
            ctx.ShuffleMyDeck();
            if (activeGotEnergy) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Poisoned);
        }
    }

    public sealed class Kieran : CardScript
    {
        public override void ResolveTrainer(EffectContext ctx)
        {
            bool canSwitch = ctx.Me.Active != null && ctx.Me.Bench.Count > 0;
            int pick = canSwitch
                ? ctx.ChooseOption("Kieran: choose 1", "Switch your Active Pokémon", "30 more damage to Pokémon ex this turn")
                : 1;
            if (pick == 0)
            {
                ctx.SwitchMyActive("Kieran: choose a Benched Pokémon to switch in");
                return;
            }
            ctx.AddEffect(new DamageBonusEffect("Kieran: attacks do 30 more damage to the opponent's Active Pokémon ex this turn", ctx.Player, ctx.Turn, 30, true));
        }
    }

    public sealed class LanasAid : CardScript
    {
        private static List<CardInstance> Options(EffectContext ctx)
        {
            return ctx.Me.Discard.Where(c => (c.Def.IsPokemon && !c.Def.HasRuleBox) || c.Def.IsBasicEnergy).ToList();
        }

        public override bool CanPlayTrainer(EffectContext ctx) { return Options(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.DiscardToHand(ctx.ChooseCards(Options(ctx), 1, 3, "Lana's Aid: put up to 3 Pokémon without a Rule Box / Basic Energy into your hand"));
        }
    }

    public sealed class NsPlan : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Any(m => m.Energy.Count > 0); }

        public override void ResolveTrainer(EffectContext ctx)
        {
            for (int k = 0; k < 2; k++)
            {
                var energy = ctx.Me.Bench.SelectMany(m => m.Energy).ToList();
                if (energy.Count == 0) break;
                var picks = ctx.ChooseCards(energy, k == 0 ? 1 : 0, 1, "N's Plan: move an Energy from your Bench to your Active Pokémon (" + (k + 1) + " of up to 2)");
                if (picks.Count == 0) break;
                var from = ctx.Me.Bench.First(m => m.Energy.Contains(picks[0]));
                ctx.Engine.MoveEnergy(from, ctx.Me.Active, picks[0]);
            }
        }
    }

    public sealed class Philippe : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Metal)); }
        private static List<CardInstance> Energy(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0 && Energy(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Philippe: choose a Metal Pokémon");
            var picks = ctx.ChooseCards(Energy(ctx), 1, 2, "Philippe: attach up to 2 Basic Metal Energy");
            foreach (var e in picks)
            {
                ctx.Me.Discard.Remove(e);
                ctx.Engine.AttachEnergyCard(e, target);
            }
        }
    }

    public sealed class PokemonCenterLady : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 || m.Conditions != SpecialCondition.None); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Pokémon Center Lady: choose 1 of your Pokémon");
            ctx.Heal(target, 60);
            ctx.Engine.RemoveAllConditions(target);
        }
    }

    public sealed class Ruffian : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Tool != null || m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy)); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Ruffian: choose 1 of your opponent's Pokémon");
            ctx.Engine.DiscardTool(target);
            var special = target.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
            if (special.Count > 0) ctx.Engine.DiscardEnergy(target, ctx.ChooseCards(special, 1, 1, "Ruffian: choose a Special Energy to discard")[0]);
        }
    }

    /// <summary>
    /// Evolve from the deck with a card that has no Abilities, even a Pokémon played this turn.
    /// Ruling: it works during the second player's first turn (it overrides "no evolving on your first turn");
    /// the first player can't use it on turn 1 because no Supporter can be played then.
    /// </summary>
    public sealed class Salvatore : CardScript
    {
        private static bool CanEvolve(EffectContext ctx, PokemonInPlay m)
        {
            return m.TurnEvolved != ctx.Turn && m.CantEvolveOnTurn != ctx.Turn;
        }

        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => CanEvolve(ctx, m)).Count > 0;
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var mine = ctx.MyPokemon(m => CanEvolve(ctx, m));
            var names = new HashSet<string>(mine.Select(m => m.Name));
            var found = ctx.SearchMyDeck(d => d.IsEvolution && !d.HasAbility && names.Contains(d.EvolvesFrom), 1,
                "Salvatore: choose a card without Abilities that evolves from 1 of your Pokémon", false);
            if (found.Count > 0)
            {
                var card = found[0];
                var target = ctx.ChoosePokemon(mine.Where(m => m.Name == card.Def.EvolvesFrom).ToList(), "Salvatore: evolve which Pokémon into " + card.Def.Name + "?");
                ctx.Engine.EvolvePokemon(target, card);
            }
            ctx.ShuffleMyDeck();
        }
    }

    public sealed class Surfer : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            if (ctx.SwitchMyActive("Surfer: choose a Benched Pokémon to switch in")) ctx.DrawUntil(5);
        }
    }

    public sealed class TeamRocketsArcher : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.WasKnockedOutLastOpponentTurn(d => d.IsTeamRockets, false); }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
            ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
            ctx.Draw(5);
            ctx.Engine.DrawCards(ctx.Opponent, 3);
        }
    }

    public sealed class TeamRocketsAriana : CardScript
    {
        private static int Target(EffectContext ctx)
        {
            var mine = ctx.MyPokemon();
            return mine.Count > 0 && mine.All(m => m.Def.IsTeamRockets) ? 8 : 5;
        }

        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) < Target(ctx);
        }

        public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(Target(ctx)); }
    }

    public sealed class TeamRocketsGiovanni : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx)
        {
            return ctx.Me.Active != null && ctx.Me.Active.Def.IsTeamRockets && ctx.Me.Bench.Any(m => m.Def.IsTeamRockets);
        }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(ctx.Me.Bench.Where(m => m.Def.IsTeamRockets).ToList(), "Team Rocket's Giovanni: choose a Benched Team Rocket's Pokémon to switch in");
            if (ctx.Engine.SwitchActive(ctx.Player, target))
                ctx.GustOpponent("Team Rocket's Giovanni: choose 1 of your opponent's Benched Pokémon to switch in");
        }
    }

    public sealed class TeamRocketsProton : CardScript
    {
        public override bool PlayableOnFirstTurn(EffectContext ctx) { return true; }
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            ctx.SearchToHand(d => d.IsBasicPokemon && d.IsTeamRockets, 3, "Team Rocket's Proton: choose up to 3 Basic Team Rocket's Pokémon");
        }
    }

    public sealed class WallysCompassion : CardScript
    {
        private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsMegaEx && m.DamageCounters > 0); }

        public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            var target = ctx.ChoosePokemon(Targets(ctx), "Wally's Compassion: heal which Mega Evolution Pokémon ex?");
            if (ctx.Heal(target, target.DamageCounters * 10) <= 0) return;
            foreach (var e in target.Energy.ToList()) ctx.Engine.ReturnEnergyToHand(target, e);
        }
    }

    public sealed class XerosicsMachinations : CardScript
    {
        public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 3; }

        public override void ResolveTrainer(EffectContext ctx)
        {
            int n = ctx.Opp.Hand.Count - 3;
            if (n <= 0 || TefPokemon.OpponentShielded(ctx)) return;
            var picks = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), n, n, "Xerosic's Machinations: discard " + n + " cards (you keep 3)", ctx.Source);
            foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, c);
        }
    }

    // ================================================================== Stadiums

    /// <summary>
    /// Base for "once during each player's turn, that player may..." Stadiums. The limit is for this Stadium card: if it's replaced and
    /// another copy is played, that one can be used again the same turn (Compendium: Rough Seas and Delinquent, 2016-02-25; v96).
    /// </summary>
    public abstract class StadiumAction : CardScript
    {
        protected abstract bool Ready(EffectContext ctx);
        protected abstract bool Use(EffectContext ctx);

        private static string Limit(EffectContext ctx) { return ctx.Card.Def.Name + "#" + ctx.Card.Id; }

        public override bool CanUseStadium(EffectContext ctx)
        {
            return ctx.IsMyTurn && !ctx.NamedLimitUsedThisTurn(Limit(ctx)) && Ready(ctx);
        }

        public override bool UseStadium(EffectContext ctx)
        {
            ctx.MarkNamedLimit(Limit(ctx));
            return Use(ctx);
        }
    }

    public sealed class AcademyAtNight : StadiumAction
    {
        protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count > 0; }

        protected override bool Use(EffectContext ctx)
        {
            var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Academy at Night: put a card from your hand on top of your deck")[0];
            ctx.Me.Hand.Remove(card);
            ctx.Me.Deck.Insert(0, card);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card from their hand on top of their deck.");
            return false;
        }
    }

    public sealed class AreaZeroUnderdepths : CardScript
    {
        public override int ModifyMaxBench(EffectContext src, int player, int max)
        {
            return src.Engine.HasTeraInPlay(player) ? Math.Max(max, 8) : max;
        }

        public override void OnLeavePlay(EffectContext ctx)
        {
            ctx.Engine.EnforceBenchLimits(ctx.Player);
        }
    }

    public sealed class BattleCage : CardScript
    {
        public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
        {
            return damageCounters && !src.IsActive(target);
        }
    }

    public sealed class CommunityCenter : StadiumAction
    {
        protected override bool Ready(EffectContext ctx)
        {
            return ctx.Me.SupporterNameThisTurn != null && ctx.MyPokemon().Any(m => m.DamageCounters > 0);
        }

        protected override bool Use(EffectContext ctx)
        {
            foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 10);
            return false;
        }
    }

    public sealed class FestivalGrounds : CardScript
    {
        public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target) { return target.Energy.Count > 0; }
    }

    public sealed class ForestOfVitality : CardScript
    {
        public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
        {
            return !src.State.IsPlayersFirstTurn && target.Def.PokemonType == EnergyType.Grass && evolution.PokemonType == EnergyType.Grass;
        }
    }

    public sealed class GravityMountain : CardScript
    {
        public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return mon.Def.Stage == Stage.Stage2 ? hp - 30 : hp; }
    }

    public sealed class JammingTower : CardScript
    {
        public override bool DisablesTools(EffectContext src) { return true; }
    }

    public sealed class LumioseCity : StadiumAction
    {
        protected override bool Ready(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

        protected override bool Use(EffectContext ctx)
        {
            ctx.SearchToBench(d => true, 1, "Lumiose City: put a Basic Pokémon onto your Bench (your turn then ends)");
            return true;
        }
    }

    public sealed class NighttimeMine : CardScript
    {
        public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
        {
            if (mon.Def.IsTera) cost.Add(EnergyType.Colorless);
        }
    }

    public sealed class Postwick : CardScript
    {
        public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
        {
            return info.Attacker != null && RecifeTrainers.IsHops(info.Attacker.Def) && info.ToOpponentsActive ? damage + 30 : damage;
        }
    }

    public sealed class PrismTower : StadiumAction
    {
        protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count >= 2 && ctx.Me.Deck.Count > 0; }

        protected override bool Use(EffectContext ctx)
        {
            var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Prism Tower: discard 2 cards to draw 1");
            foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
            ctx.Draw(1);
            return false;
        }
    }

    public sealed class TeamRocketsFactory : StadiumAction
    {
        protected override bool Ready(EffectContext ctx)
        {
            var name = ctx.Me.SupporterNameThisTurn;
            return name != null && name.Contains("Team Rocket") && ctx.Me.Deck.Count > 0;
        }

        protected override bool Use(EffectContext ctx)
        {
            ctx.Draw(2);
            return false;
        }
    }
}
