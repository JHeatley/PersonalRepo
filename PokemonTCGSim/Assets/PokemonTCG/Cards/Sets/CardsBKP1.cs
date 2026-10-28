using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// BREAKpoint (BKP, v96), part 1: the Grass, Fire, Water and Lightning Pokémon (1-49). Greninja BREAK (41) is 30th Celebration's
    /// CC22 and keeps that script.
    /// </summary>
    public static class CardsBKP1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BKP-1", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("BKP-2", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("BKP-3", new MeganiumBKP());
            r.RegisterByKey("BKP-5", new SearchAttackMon("Bug Hunch", d => d.IsPokemon && d.HasType(EnergyType.Grass), 3));
            r.RegisterByKey("BKP-6", new CardsCES2.ScreechMon("Screech", 60));
            r.RegisterByKey("BKP-7", new CardsSHF1.HealEachMon("Aromatherapy", 10, false));
            r.RegisterByKey("BKP-8", new ComboMon(new BooHooMon(), new CoinsMon("Leaf Slice", 2)));
            r.RegisterByKey("BKP-9", new ComboMon(new CardsSSP2.StompOffMon("Mountain Munch", 1), new ScrapeDownMon()));
            r.RegisterByKey("BKP-11", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("BKP-13", new CardsLOT2.EruptionMon());
            r.RegisterByKey("BKP-14", new ComboMon(new CardsMEG.FlipUntilTailsBonusMon("Spiral Punch", 20), new CardsTWM1.SelfDiscardMon("Strong Flare", 2)));
            r.RegisterByKey("BKP-15", new CardsASC.ToolBonusMon("Reinforced Flame", 20));
            r.RegisterByKey("BKP-17", new CardsASR1.DiscardDefendingSpecialMon("Derail", false));
            r.RegisterByKey("BKP-18", new GolduckBreak());
            r.RegisterByKey("BKP-19", new SearchAttackMon("Tail Lure", d => d.IsPokemon, 1));
            r.RegisterByKey("BKP-20", new ComboMon(new FlipBonusMon("Careless Head", 50), new WalkOffHomerMon()));
            r.RegisterByKey("BKP-21", new SlowkingBKP());
            r.RegisterByKey("BKP-22", new FlipBonusMon("Razor Shell", 10));
            r.RegisterByKey("BKP-23", new ClampMon());
            r.RegisterByKey("BKP-24", new SuddenGripMon());
            r.RegisterByKey("BKP-26", new ComboMon(new StormySeasMon(), new CardsBUS1.OwnBenchQuakeMon("Splash Burn", 10)));
            r.RegisterByKey("BKP-27", new BlastGeyserMon());
            r.RegisterByKey("BKP-28", new ComboMon(new SearchAttackMon("Errand-Running", d => d.IsTrainer, 1), new FlipBonusMon("Water Splash", 20)));
            r.RegisterByKey("BKP-29", new CoinsMon("Spike Cannon", 2));
            r.RegisterByKey("BKP-30", new SuicuneBKP());
            r.RegisterByKey("BKP-31", new ComboMon(new AquaTurboMon(), new CardsMEG.SnipeBenchMon("Pearl Hurricane", 30)));
            r.RegisterByKey("BKP-32", new ManaphyEX());
            r.RegisterByKey("BKP-33", new CardsBLK.RoundMon());
            r.RegisterByKey("BKP-34", new FlipBonusMon("Frog Hop", 20));
            r.RegisterByKey("BKP-35", new ComboMon(new SiphonOffMon(), new ShakyFallMon()));
            r.RegisterByKey("BKP-36", new TailsNothingMon("Lunge"));
            r.RegisterByKey("BKP-37", new RecoilMon("Brave Bird", 20));
            r.RegisterByKey("BKP-38", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("BKP-39", new WaterDuplicatesMon());
            r.RegisterByKey("BKP-40", new ComboMon(new ShadowStitchingMon(), new MoonlightSlashMon()));
            r.RegisterByKey("BKP-44", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("BKP-45", new ParalyzeFlipMon("Thunder Fang"));
            r.RegisterByKey("BKP-46", new SnarlMon());
            r.RegisterByKey("BKP-47", new CardsMEG.FlipUntilTailsBonusMon("Wild Fury", 40));
            r.RegisterByKey("BKP-48", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("BKP-49", new ZebstrikaBKP());
        }

        // ================================================================== Grass

        /// <summary>Overgrow: with 50 HP or less left, its attacks do 70 more to the opponent's Active Pokémon (before W/R). Green Force: heal the damage done.</summary>
        private sealed class MeganiumBKP : Mon
        {
            private static readonly Mon GreenForce = new CardsDAA1.DrainMon("Green Force");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.RemainingHp(me) <= 50 ? damage + 70 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return GreenForce.HandleAttack(ctx, atk); }
        }

        /// <summary>"If your opponent attaches an Energy card from their hand to the Defending Pokémon during their next turn, that Pokémon is now Asleep."</summary>
        internal sealed class AsleepOnAttachEffect : ContinuousEffect
        {
            public AsleepOnAttachEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnEnergyAttachedFromHand(GameEngine engine, CardInstance energy, PokemonInPlay target)
            {
                if (target.Uid != TargetUid || engine.State.CurrentPlayer != target.Owner) return;
                engine.Log(Owner, Name + ": " + target.Name + " is now Asleep.");
                engine.ApplyCondition(target, SpecialCondition.Asleep);
            }
        }

        /// <summary>Boo-Hoo: if the opponent attaches an Energy card from their hand to the Defending Pokémon during their next turn, it's now Asleep.</summary>
        private sealed class BooHooMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Boo-Hoo") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new AsleepOnAttachEffect("Boo-Hoo", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Scrape Down: if this Pokémon has any damage counters on it, discard the top 4 cards of the opponent's deck.</summary>
        private sealed class ScrapeDownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scrape Down") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || ctx.Pokemon.DamageCounters == 0 || ctx.Opp.Deck.Count == 0) return true;
                if (ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Log("The top cards of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck can't be discarded right now.");
                else ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 4);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Hyper Transfer: as often as you like during your turn, move a basic Energy from 1 of your Pokémon to another of your Pokémon.</summary>
        private sealed class GolduckBreak : Mon
        {
            private const string HyperTransfer = "Hyper Transfer";

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                if (ctx.MyPokemon().Count < 2) return new List<PokemonInPlay>();
                return ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy));
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == HyperTransfer
                       && ctx.IsMyTurn && Holders(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var holders = Holders(ctx);
                if (holders.Count == 0) return;
                var source = ctx.ChoosePokemon(holders, "Hyper Transfer: move a basic Energy from which of your Pokémon?");
                if (source == null) return;
                var basics = source.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var energy = ctx.ChooseCards(basics, 1, 1, "Hyper Transfer: move which Energy?")[0];
                var targets = ctx.MyPokemon(m => m != source);
                var target = ctx.ChoosePokemon(targets, "Hyper Transfer: move " + energy.Def.Name + " to which Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(source, target, energy);
            }
        }

        /// <summary>Walk-Off Homer: if you have exactly 1 Prize card left when you use it, you win the game.</summary>
        private sealed class WalkOffHomerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Walk-Off Homer") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Prizes.Count == 1) ctx.Engine.EndGame(ctx.Player, "Walk-Off Homer with 1 Prize card left");
                else ctx.Log("Walk-Off Homer: " + ctx.Engine.PlayerName(ctx.Player) + " doesn't have exactly 1 Prize card left.");
                return true;
            }
        }

        /// <summary>
        /// Royal Flash: once during your turn, flip a coin; heads, move an Energy from the opponent's Active Pokémon to 1 of their Benched
        /// Pokémon. Psych Up: 40 more during your next turn.
        /// </summary>
        private sealed class SlowkingBKP : Mon
        {
            protected override string Activated { get { return "Royal Flash"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0 && ctx.Opp.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var active = ctx.Opp.Active;
                if (active == null || active.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(active)) return;
                var energy = ctx.ChooseCards(active.Energy.ToList(), 1, 1, "Royal Flash: move which Energy from " + active.Name + "?")[0];
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Royal Flash: move " + energy.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(active, target, energy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psych Up") return false;
                var me = ctx.Pokemon;
                int uid = me.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == "Psych Up").Sum(b => b.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                if (!ctx.InPlay(me)) return true;
                var boost = new AttackBoostEffect(me.Name + "'s Psych Up does 40 more damage during its next turn (Psych Up)", ctx.Player, uid, ctx.Turn + 2, "Psych Up", 40);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        /// <summary>Clamp: flip a coin; heads, the opponent's Active Pokémon is Paralyzed; tails, the attack does nothing.</summary>
        private sealed class ClampMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clamp") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Sudden Grip: if this Pokémon evolved from Shellder during this turn, the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class SuddenGripMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sudden Grip") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool fresh = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Shellder";
                ctx.DamageDefending(atk.Damage);
                if (fresh && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Stormy Seas: flip a coin until you get tails; for each heads, search your deck for a Water Energy card and attach it to this Pokémon.</summary>
        private sealed class StormySeasMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stormy Seas") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                var me = ctx.Pokemon;
                if (heads == 0 || !ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Water), heads, "Stormy Seas: choose up to " + heads + " Water Energy cards to attach to " + me.Name);
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Blast Geyser: you may do 20 more for each Water Energy on this Pokémon; if you do, discard the top 2 cards of your deck.</summary>
        private sealed class BlastGeyserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blast Geyser") return false;
                int water = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                bool more = water > 0 && ctx.AskYesNo("Blast Geyser: do " + (20 * water) + " more damage and discard the top 2 cards of your deck?");
                ctx.DamageDefending(atk.Damage + (more ? 20 * water : 0));
                if (more && ctx.Me.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, 2);
                return true;
            }
        }

        /// <summary>Wind Charm: while this Pokémon is your Active Pokémon, prevent all effects of the opponent's attacks, except damage, done to each of your Pokémon.</summary>
        internal sealed class SuicuneBKP : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && target != null && target.Owner == src.Player && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>Aqua Turbo: search your deck for 2 Water Energy cards and attach them to 1 of your Benched Pokémon.</summary>
        private sealed class AquaTurboMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Turbo") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var bench = ctx.MyBench();
                var target = ctx.ChoosePokemon(bench, "Aqua Turbo: attach 2 Water Energy cards to which Benched Pokémon?");
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 2, "Aqua Turbo: choose 2 Water Energy cards");
                foreach (var card in found)
                {
                    if (target != null && ctx.InPlay(target)) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Aqua Tube: each of your Pokémon with any Water Energy attached has no Retreat Cost. Mineral Pump: heal 30 from each Benched Pokémon.</summary>
        private sealed class ManaphyEX : Mon
        {
            private static readonly Mon Pump = new CardsSHF1.HealEachMon("Mineral Pump", 30, true);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return cost;
                return src.Engine.HasEnergyOfType(mon, EnergyType.Water) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Siphon Off: attach 3 Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class SiphonOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Siphon Off") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (!ctx.InPlay(me) || pile.Count == 0) return true;
                int n = Math.Min(3, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Siphon Off: attach 3 Energy cards from your discard pile to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Shaky Fall: the opponent's Active Pokémon is now Confused and can't retreat during their next turn.</summary>
        private sealed class ShakyFallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shaky Fall") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.CanAffect(defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Water Duplicates: search your deck for up to 3 Frogadier and put them onto your Bench (Stage 1 cards straight onto the Bench).</summary>
        private sealed class WaterDuplicatesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Duplicates") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int room = Math.Min(3, ctx.BenchSpace);
                if (room <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.Name == "Frogadier" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), room, "Water Duplicates: choose up to " + room + " Frogadier to put onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Shadow Stitching: until the end of the opponent's next turn, every Pokémon they have in play, in their hand and in their discard
        /// pile has no Abilities, cards put into play that turn included. Suicune's Wind Charm keeps the Abilities of the Pokémon that were
        /// in play when it was used, while Suicune stays Active; Pokémon put into play later lose theirs (Compendium, 2016-02-11 and 2016).
        /// </summary>
        internal sealed class ShadowStitchingEffect : ContinuousEffect
        {
            private readonly int affected;
            private readonly HashSet<int> inPlayThen;

            public ShadowStitchingEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn, IEnumerable<int> inPlayUids)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
                inPlayThen = new HashSet<int>(inPlayUids);
            }

            private bool WindCharmed(GameEngine engine, PokemonInPlay mon, int depth)
            {
                if (!inPlayThen.Contains(mon.Uid)) return false;
                var active = engine.State.Players[affected].Active;
                if (active == null || !active.Def.Abilities.Any(a => a.Name == "Wind Charm")) return false;
                return depth >= 1 || !engine.AbilityBlockedAtDepth(active, "Wind Charm", depth + 1);
            }

            public override bool BlocksAbility(GameEngine engine, PokemonInPlay mon, string abilityName, int depth)
            {
                return mon != null && mon.Owner == affected && CardsBKP2.GarbodorBKP.OwnAbility(mon, abilityName) && !WindCharmed(engine, mon, depth);
            }

            public override bool BlocksAbilitiesOutOfPlay(GameEngine engine, int player, CardDef card) { return player == affected; }
        }

        /// <summary>Shadow Stitching: 40 damage, then the opponent's Pokémon have no Abilities until the end of their next turn.</summary>
        private sealed class ShadowStitchingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Stitching") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new ShadowStitchingEffect(ctx.Engine.PlayerName(ctx.Opponent) + "'s Pokémon have no Abilities until the end of their next turn (Shadow Stitching)",
                    ctx.Player, ctx.Opponent, ctx.Turn + 1, ctx.Opp.InPlay().Select(m => m.Uid)));
                return true;
            }
        }

        /// <summary>Moonlight Slash: you may return a Water Energy from this Pokémon to your hand; if you do, 20 more damage.</summary>
        private sealed class MoonlightSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moonlight Slash") return false;
                var me = ctx.Pokemon;
                var water = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)).ToList();
                bool more = false;
                if (water.Count > 0 && ctx.AskYesNo("Moonlight Slash: return a Water Energy from " + me.Name + " to your hand for 20 more damage?"))
                {
                    var pick = ctx.ChooseCards(water, 1, 1, "Moonlight Slash: return which Energy?")[0];
                    more = ctx.Engine.ReturnEnergyToHand(me, pick);
                }
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>"Any damage done by attacks from the Defending Pokémon is reduced by N (after applying Weakness and Resistance)."</summary>
        internal sealed class WeakenAttackerAfterEffect : ContinuousEffect
        {
            private readonly int amount;

            public WeakenAttackerAfterEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Attacker != null && info.Attacker.Uid == TargetUid ? damage - amount : damage;
            }
        }

        /// <summary>Snarl: during the opponent's next turn, the Defending Pokémon's attacks do 20 less damage (after Weakness and Resistance).</summary>
        private sealed class SnarlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snarl") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new WeakenAttackerAfterEffect(defending.Name + "'s attacks do 20 less damage during its next turn (Snarl)", ctx.Player, defending.Uid, ctx.Turn + 1, 20));
                return true;
            }
        }

        /// <summary>
        /// Zap Zone: damage from your Lightning Pokémon's attacks isn't affected by any effects on the opponent's Active Pokémon (Weakness
        /// and Resistance still apply: Compendium, 2016-02-11). Crashing Bolt: 60 more against Fighting Resistance.
        /// </summary>
        private sealed class ZebstrikaBKP : Mon
        {
            public override bool AttacksIgnoreDefenderEffects(EffectContext src, DamageInfo info)
            {
                var attacker = info.Attacker;
                return src.Pokemon != null && attacker != null && attacker.Owner == src.Player && src.Engine.IsType(attacker, EnergyType.Lightning);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crashing Bolt") return false;
                bool resists = ctx.Defending != null && ctx.Engine.ResistanceOf(ctx.Defending) == EnergyType.Fighting;
                ctx.DamageDefending(atk.Damage + (resists ? 60 : 0));
                return true;
            }
        }
    }
}
