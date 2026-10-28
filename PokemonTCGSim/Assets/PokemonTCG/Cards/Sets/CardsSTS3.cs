using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Steam Siege (STS, v93), part 3: the Fairy, Dragon and Colorless Pokémon (76-97) and the Trainers. Restored Pokémon
    /// (Shieldon, Anorith) are Stage.Restored: only their Fossil Item puts them into play.
    /// </summary>
    public static class CardsSTS3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("STS-76", new CardsTWM1.HealSelfMon("Bubble Drain", 10));
            r.RegisterByKey("STS-77", new ComboMon(new FlipBonusMon("Play Rough", 30), new CardsTWM1.HealSelfMon("Bubble Drain", 30)));
            r.RegisterByKey("STS-78", new ComboMon(new LinkBlastMon(), new CardsTWM1.SelfDiscardMon("Luminous Blade", 1)));
            r.RegisterByKey("STS-79", new DespairRayMon());
            r.RegisterByKey("STS-80", new KlefkiSTS());
            r.RegisterByKey("STS-81", new ComboMon(new GeomancyMon(), new CardsTWM1.SelfDiscardMon("Rainbow Spear", 1)));
            r.RegisterByKey("STS-82", new CardsASR1.TimesMon("Life Stream", c => c.MyPokemon().Sum(m => c.EnergyCount(m))));
            r.RegisterByKey("STS-83", new ComboMon(new ProudFangMon(), new CardsBCR1.GigaHornMon("Giga Claw")));
            r.RegisterByKey("STS-84", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("STS-85", new ComboMon(new CoinsMon("Double Hit", 2), new CardsTWM1.SelfDiscardMon("Power Breath", 1)));
            r.RegisterByKey("STS-86", new ComboMon(new CardsMEG.WeakenDefendingMon("Cruel Fang", 40), new DarkBurnMon()));
            r.RegisterByKey("STS-87", new CalamityBlastMon());
            r.RegisterByKey("STS-88", new StallMon());
            r.RegisterByKey("STS-89", new ComboMon(new RaidMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("STS-90", new FiddleAroundMon());
            r.RegisterByKey("STS-91", new ComboMon(new FurryChanceMon(), new CoinsMon("Double Hit", 2)));
            r.RegisterByKey("STS-92", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("STS-93", new ComboMon(new FlipBonusMon("Ambush", 50), new SkyDropMon("Sky Drop", 20)));
            r.RegisterByKey("STS-96", new TalonflameSTS());
            r.RegisterByKey("STS-97", new HawluchaSTS());

            r.RegisterByName("Armor Fossil Shieldon", new FossilItem("Shieldon"));
            r.RegisterByName("Claw Fossil Anorith", new FossilItem("Anorith"));
            r.RegisterByName("Captivating Poké Puff", new CaptivatingPokePuff());
            r.RegisterByName("Gardevoir Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Steelix Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Greedy Dice", new GreedyDice());
            r.RegisterByName("Ninja Boy", new NinjaBoy());
            r.RegisterByName("Pokémon Ranger", new PokemonRanger());
            r.RegisterByName("Special Charge", new SpecialCharge());
            r.RegisterByName("Professor Sycamore", new ProfessorSycamore());
        }

        // ================================================================== Fairy

        /// <summary>Link Blast: 70 more if this Pokémon and the opponent's Active Pokémon have the same amount of Energy attached.</summary>
        private sealed class LinkBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Link Blast") return false;
                var defending = ctx.Defending;
                bool same = defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(defending);
                ctx.DamageDefending(atk.Damage + (same ? 70 : 0));
                return true;
            }
        }

        /// <summary>Despair Ray: discard any number of your Benched Pokémon (and their cards); 10 more for each.</summary>
        private sealed class DespairRayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Despair Ray") return false;
                var bench = ctx.MyBench();
                int discarded = 0;
                if (bench.Count > 0 && ctx.AskYesNo("Despair Ray: discard any of your Benched Pokémon for 10 more damage each?"))
                {
                    var picks = ctx.ChoosePokemonMany(bench, 1, bench.Count, "Despair Ray: choose the Benched Pokémon to discard");
                    foreach (var mon in picks)
                    {
                        if (!ctx.InPlay(mon)) continue;
                        ctx.Engine.DiscardPokemonFromPlay(mon);
                        discarded++;
                    }
                }
                ctx.DamageDefending(atk.Damage + 10 * discarded);
                return true;
            }
        }

        /// <summary>
        /// Wonder Lock: once during your turn (before your attack), if this Pokémon is on your Bench, discard all cards attached to it
        /// and attach it to 1 of your Pokémon as a Pokémon Tool card. While attached it prevents all damage done to that Pokémon by
        /// attacks from the opponent's Mega Evolution Pokémon (XY-era Megas, your rules update) and is discarded at the end of the
        /// opponent's turn. As a Tool it's a Pokémon Tool card for everything that counts or removes Tools; in the hand or discard
        /// pile it's a Pokémon card again.
        /// </summary>
        private sealed class KlefkiSTS : Mon
        {
            internal const string WonderLock = "Wonder Lock";

            protected override string Activated { get { return WonderLock; } }

            private static bool AttachedAsTool(EffectContext ctx) { return ctx.Pokemon != null && ctx.Card != null && ctx.Pokemon.Top != ctx.Card; }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && ctx.Engine.CanTakeTool(m));
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return !AttachedAsTool(ctx) && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Pokemon.Stack.Count == 1 && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Wonder Lock: attach Klefki as a Pokémon Tool to which of your Pokémon?");
                if (target == null) return;
                var card = me.Top;
                ctx.Engine.RemoveFromPlay(me);
                ctx.Engine.DiscardFromPlay(me.Energy.ToList());
                ctx.Engine.DiscardFromPlay(me.Tools.ToList());
                ctx.Engine.AttachTool(card, target);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                if (!AttachedAsTool(src)) return false;
                var attacker = info.Attacker;
                return info.Target == src.Pokemon && attacker != null && attacker.Owner != src.Player
                       && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && attacker.Def.IsXYMegaEvolution;
            }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                if (!AttachedAsTool(src) || src.State.CurrentPlayer == src.Player) return;
                var holder = src.Pokemon;
                var card = src.Card;
                if (!holder.Tools.Remove(card)) return;
                src.Engine.DiscardFromPlay(card);
                src.Log("Wonder Lock: " + card.Def.Name + " on " + holder.Name + " was discarded at the end of the opponent's turn.");
            }
        }

        /// <summary>Geomancy: choose 2 of your Benched Pokémon; for each, search your deck for a Fairy Energy card and attach it to that Pokémon; shuffle.</summary>
        private sealed class GeomancyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Geomancy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                int n = Math.Min(2, bench.Count);
                var chosen = n == bench.Count ? bench : ctx.ChoosePokemonMany(bench, n, n, "Geomancy: choose 2 of your Benched Pokémon");
                foreach (var mon in chosen)
                {
                    if (!ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) continue;
                    var holder = mon;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fairy, 1, "Geomancy: choose a Fairy Energy card for " + holder.Name);
                    foreach (var card in found) ctx.Engine.AttachEnergyCard(card, holder);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Proud Fang: 60 more if the opponent has any Pokémon BREAK in play.</summary>
        private sealed class ProudFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Proud Fang") return false;
                ctx.DamageDefending(atk.Damage + (ctx.OpponentPokemon(m => m.Def.IsBreak).Count > 0 ? 60 : 0));
                return true;
            }
        }

        /// <summary>Dark Burn: discard any number of Darkness Energy from your Pokémon; 50 damage for each.</summary>
        private sealed class DarkBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Burn") return false;
                var cards = new List<CardInstance>();
                var holders = new Dictionary<int, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var card in mon.Energy)
                        if (CardsHIF1.Provides(ctx, mon, card, EnergyType.Darkness)) { cards.Add(card); holders[card.Id] = mon; }
                int discarded = 0;
                if (cards.Count > 0)
                {
                    var picks = ctx.ChooseCards(cards, 0, cards.Count, "Dark Burn: discard any number of Darkness Energy from your Pokémon (50 damage each)");
                    foreach (var card in picks)
                        if (ctx.Engine.DiscardEnergy(holders[card.Id], card)) discarded++;
                }
                if (discarded == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * discarded);
                return true;
            }
        }

        /// <summary>Calamity Blast: discard 3 Energy from this Pokémon; 50 damage to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class CalamityBlastMon : Mon
        {
            private static readonly Mon Bench = new CardsFST1.TwoBenchedMon("Calamity Blast", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Calamity Blast") return false;
                Bench.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 3, "Calamity Blast: discard 3 Energy from " + me.Name);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Stall: only if you went second, on your first turn: discard an Energy from 1 of the opponent's Pokémon.</summary>
        private sealed class StallMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Stall") return true;
                return ctx.State.FirstPlayer != ctx.Player && ctx.Turn == 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stall") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Stall: discard an Energy from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Stall: discard an Energy from " + target.Name);
                return true;
            }
        }

        /// <summary>Raid: 30 more if this Pokémon evolved from Meowth during this turn.</summary>
        private sealed class RaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raid") return false;
                var me = ctx.Pokemon;
                bool justNow = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Meowth";
                ctx.DamageDefending(atk.Damage + (justNow ? 30 : 0));
                return true;
            }
        }

        /// <summary>Fiddle Around: look at the top 3 cards of the opponent's deck and put them back in any order.</summary>
        private sealed class FiddleAroundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fiddle Around") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0) CardsDRI2.ReorderOpponentTop(ctx, 3, "Fiddle Around");
                return true;
            }
        }

        /// <summary>Furry Chance: discard the top card of the opponent's deck; 60 more if it's an Energy card.</summary>
        private sealed class FurryChanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Furry Chance") return false;
                var top = ctx.Opp.Deck.Count > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1) : new List<CardInstance>();
                bool energy = top.Count > 0 && top[0].Def.IsEnergy;
                ctx.DamageDefending(atk.Damage + (energy ? 60 : 0));
                return true;
            }
        }

        /// <summary>Sky Drop: 20 less for each [C] in the opponent's Active Pokémon's Retreat Cost. Used for Sky Drop, Gusting Collision (the attack name and the numbers are passed in).</summary>
        internal sealed class SkyDropMon : Mon
        {
            private readonly string attack;
            private readonly int each;

            public SkyDropMon(string attack, int each)
            {
                this.attack = attack;
                this.each = each;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                ctx.DamageDefending(Math.Max(0, atk.Damage - each * cost));
                return true;
            }
        }

        /// <summary>Gale Wings: may start face down as your Active Pokémon. Aero Blitz: search your deck for up to 2 cards and put them into your hand.</summary>
        private sealed class TalonflameSTS : Mon
        {
            private static readonly Mon Blitz = new SearchAttackMon("Aero Blitz", d => true, 2);

            public override bool CanStartAsActive { get { return true; } }
            public override bool MayKeepOpeningHandWithIt { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blitz.HandleAttack(ctx, atk); }
        }

        /// <summary>Sudden Cyclone: when you play it from your hand onto your Bench, you may have the opponent switch their Active Pokémon. Acrobatics: flip 2, 20 more for each heads.</summary>
        private sealed class HawluchaSTS : Mon
        {
            internal const string Cyclone = "Sudden Cyclone";

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                bool ready = ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0;
                if (!TriggerOnce(ctx, Cyclone, ready, "Sudden Cyclone: have your opponent switch their Active Pokémon with 1 of their Benched Pokémon?")) return;
                ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Acrobatics") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.FlipCoins(2));
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Armor Fossil Shieldon / Claw Fossil Anorith: look at the bottom 7 cards of your deck; you may put that Restored Pokémon from them onto your Bench; shuffle.</summary>
        internal sealed class FossilItem : CardScript
        {
            private readonly string restored;

            public FossilItem(string restored) { this.restored = restored; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var deck = ctx.Me.Deck;
                int n = Math.Min(7, deck.Count);
                var bottom = deck.Skip(deck.Count - n).ToList();
                // The Restored Pokémon of that name (a Basic one of that name could go on the Bench too; an Evolution card can't).
                var options = bottom.Where(c => c.Def.IsPokemon && c.Def.Name == restored && (c.Def.Stage == Stage.Restored || c.Def.IsBasicPokemon)
                    && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                var picks = options.Count == 0 || ctx.BenchSpace <= 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(options, 0, 1, ctx.SourceName + ": put a " + restored + " from the bottom 7 cards of your deck onto your Bench?");
                foreach (var card in picks)
                    if (ctx.Me.Deck.Remove(card)) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Captivating Poké Puff: the opponent reveals their hand; put any number of Basic Pokémon from it onto their Bench.</summary>
        private sealed class CaptivatingPokePuff : CardScript
        {
            /// <summary>Not playable into a full Bench (Compendium, 2016).</summary>
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0 && ctx.Engine.BenchSpace(ctx.Opponent) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                TefPokemon.RevealOpponentHand(ctx);
                int room = ctx.Engine.BenchSpace(ctx.Opponent);
                var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (room <= 0 || basics.Count == 0) return;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(room, basics.Count), "Captivating Poké Puff: put any number of these Basic Pokémon onto your opponent's Bench");
                foreach (var card in picks)
                    if (ctx.Engine.BenchSpace(ctx.Opponent) > 0 && ctx.Opp.Hand.Remove(card)) ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
            }
        }

        /// <summary>
        /// Greedy Dice: only when you take it as a face-down Prize card, before it goes into your hand: flip a coin; heads, take 1 more
        /// Prize card. Item-blocking effects stop it.
        /// </summary>
        private sealed class GreedyDice : CardScript
        {
            [ThreadStatic] private static int extraPrizes;

            public override bool CanPlayTrainer(EffectContext ctx) { return false; }
            public override bool UsableWhenTakenAsPrize { get { return true; } }

            public override int OnTakenAsPrize(EffectContext ctx)
            {
                if (!ctx.Engine.CanPlayItemFromOutsideHand(ctx.Player, ctx.Card)) return -1;
                if (!ctx.AskYesNo("Greedy Dice: you took it as a Prize card. Play it now (flip a coin; heads, take 1 more Prize card)?")) return -1;
                extraPrizes = 0;
                ctx.Engine.PlayItemFromOutsideHand(ctx.Player, ctx.Card, "from " + ctx.Engine.Their(ctx.Player) + " Prize cards");
                int extra = extraPrizes;
                extraPrizes = 0;
                return extra;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                extraPrizes = ctx.Flip() ? 1 : 0;
                if (extraPrizes > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " takes 1 more Prize card (Greedy Dice).");
            }
        }

        /// <summary>
        /// Ninja Boy: switch 1 of your Basic Pokémon in play with a Basic Pokémon from your deck. Everything stays on the new Pokémon
        /// (attached cards, damage counters, Special Conditions, turns in play, effects; a used once-per-turn Ability stays used:
        /// Compendium), and the first Pokémon is shuffled into your deck.
        /// </summary>
        private sealed class NinjaBoy : CardScript
        {
            private static List<PokemonInPlay> Basics(EffectContext ctx) { return ctx.MyPokemon(m => m.Stack.Count == 1 && m.Def.IsBasicPokemon); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Basics(ctx).Count > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0) return;
                var target = ctx.ChoosePokemon(basics, "Ninja Boy: switch which of your Basic Pokémon?");
                if (target == null) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, "Ninja Boy: choose a Basic Pokémon to switch with " + target.Name);
                if (found.Count > 0 && ctx.InPlay(target) && target.Stack.Count == 1)
                {
                    var old = target.Stack[0];
                    target.Stack[0] = found[0];
                    ctx.Me.Deck.Add(old);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched " + old.Def.Name + " with " + found[0].Def.Name + " from " + ctx.Engine.Their(ctx.Player) + " deck (Ninja Boy).");
                }
                else ctx.Me.Deck.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Pokémon Ranger: remove all effects of attacks on each player and their Pokémon (not Special Conditions: Compendium).</summary>
        private sealed class PokemonRanger : CardScript
        {
            /// <summary>Can't be played when there's no effect of an attack to remove (Compendium, 2016).</summary>
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.State.Effects.Any(e => e.FromAttack); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = ctx.State.Effects.RemoveAll(e => e.FromAttack);
                ctx.Log(n == 0 ? "Pokémon Ranger: there were no effects of attacks to remove." : "Pokémon Ranger removed " + n + (n == 1 ? " effect" : " effects") + " of attacks.");
            }
        }

        /// <summary>Special Charge: shuffle 2 Special Energy cards from your discard pile into your deck (1 if that's all there is).</summary>
        private sealed class SpecialCharge : CardScript
        {
            private static List<CardInstance> Special(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsSpecialEnergy).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Special(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var special = Special(ctx);
                if (special.Count == 0) return;
                int n = Math.Min(2, special.Count);
                var picks = n == special.Count ? special : ctx.ChooseCards(special, n, n, "Special Charge: shuffle 2 Special Energy cards into your deck");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Professor Sycamore: discard your hand and draw 7 cards.</summary>
        private sealed class ProfessorSycamore : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(7);
            }
        }
    }
}
