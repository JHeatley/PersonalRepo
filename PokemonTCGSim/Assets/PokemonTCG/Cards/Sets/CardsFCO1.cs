using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Fates Collide (FCO, v94), part 1: the Grass, Fire, Water, Lightning and Psychic Pokémon (1-35). Fates Collide brought the
    /// first Restored Pokémon (Omanyte, Kabuto, Aerodactyl: Stage.Restored, put into play by their Fossil Items).
    /// </summary>
    public static class CardsFCO1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FCO-1", new ComboMon(new HideABerryMon(), new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("FCO-3", new ComboMon(new SolarRayMon(), new FlipBonusMon("Leaf Cutter", 30)));
            r.RegisterByKey("FCO-4", new MothimFCO());
            r.RegisterByKey("FCO-5", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("FCO-6", new ServineFCO());
            r.RegisterByKey("FCO-7", new ComboMon(new CardsFST1.NextTurnBonusMon("Coil", 60), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("FCO-8", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("FCO-9", new CardsSUM5.OptionalBoostRecoilMon("Flying Flare", 40, 20));
            r.RegisterByKey("FCO-10", new CardsDAA1.FlipCantAttackMon("Tail Whip"));
            r.RegisterByKey("FCO-11", new CardsCRZ1.InviteOutMon());
            r.RegisterByKey("FCO-12", new CardsASC.FlipDiscardDefendingEnergyMon("Destructive Flame"));
            r.RegisterByKey("FCO-13", new ComboMon(new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep), new CardsASR1.TimesMon("Psystorm", EnergyOnAllPokemon)));
            r.RegisterByKey("FCO-14", new DelphoxBreakFCO());
            r.RegisterByKey("FCO-16", new FreezingBreathMon());
            r.RegisterByKey("FCO-18", new OmastarFCO());
            r.RegisterByKey("FCO-19", new OmastarBreakFCO());
            r.RegisterByKey("FCO-20", new ComboMon(new CardsMEW1.CounterBonusMon("Second Bite", 10), new CardsASR1.FrostWallMon("Crystal Ray", 9999)));
            r.RegisterByKey("FCO-21", new ComboMon(new BurningIciclesMon(), new LockAfterAttackMon("Blizzard Burn", false)));
            r.RegisterByKey("FCO-22", new CardsWHT.FlipMoreMon("Swing Around", 2, 20));
            r.RegisterByKey("FCO-23", new BarbaracleFCO());
            r.RegisterByKey("FCO-24", new ComboMon(new CardsSMP2.EnergyExtractMon(), new PlasmagicMon()));
            r.RegisterByKey("FCO-25", new SuppressionMon());
            r.RegisterByKey("FCO-26", new MAlakazamEX());
            r.RegisterByKey("FCO-27", new CardsUNM1.NamedBenchSearchMon("Division", "Koffing", 2));
            r.RegisterByKey("FCO-28", new ComboMon(new BalloonBombMon(), new ThickLiquidMon()));
            r.RegisterByKey("FCO-29", new MewFCO());
            r.RegisterByKey("FCO-30", new TailsNothingMon("Rocket Jump"));
            r.RegisterByKey("FCO-31", new ComboMon(new HeadWalkingMon(), new CardsDRI1.WhirlwindMon("Knock Back")));
            r.RegisterByKey("FCO-32", new ConditionAttackMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("FCO-33", new LinkMon("Mini Link", 1));
            r.RegisterByKey("FCO-34", new LinkMon("Double Link", 2));
            r.RegisterByKey("FCO-35", new LinkMon("Link Fusion", 3));
        }

        private static int EnergyOnAllPokemon(EffectContext ctx)
        {
            return ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)) + ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
        }

        // ================================================================== Grass

        /// <summary>Hide a Berry: draw 2 cards, then put a card from your hand on the bottom of your deck.</summary>
        private sealed class HideABerryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hide a Berry") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(2);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var card = ctx.ChooseCards(hand, 1, 1, "Hide a Berry: put a card from your hand on the bottom of your deck")[0];
                if (ctx.Me.Hand.Remove(card))
                {
                    ctx.Me.Deck.Add(card);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card from " + ctx.Engine.Their(ctx.Player) + " hand on the bottom of " + ctx.Engine.Their(ctx.Player) + " deck (Hide a Berry).");
                }
                return true;
            }
        }

        /// <summary>Solar Ray: heal 20 damage from each of your Pokémon.</summary>
        internal sealed class SolarRayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Solar Ray") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, 20);
                return true;
            }
        }

        /// <summary>
        /// Wormadam First: as often as you like during your turn (before your attack), move 1 damage counter from 1 of your Wormadam
        /// to another of your Pokémon (another Wormadam too). Gust: no effect.
        /// </summary>
        private sealed class MothimFCO : Mon
        {
            private const string WormadamFirst = "Wormadam First";

            private static List<PokemonInPlay> Damaged(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Def.Name == "Wormadam" && m.DamageCounters > 0);
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != WormadamFirst) return false;
                return ctx.IsMyTurn && ctx.MyPokemon().Count >= 2 && Damaged(ctx).Count > 0 && !ctx.Engine.CounterMovementBlocked();
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var sources = Damaged(ctx);
                if (sources.Count == 0) return;
                var wormadam = ctx.ChoosePokemon(sources, "Wormadam First: move a damage counter from which Wormadam?");
                if (wormadam == null) return;
                var targets = ctx.MyPokemon(m => m != wormadam);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Wormadam First: move it to which of your Pokémon?");
                if (target != null) ctx.Engine.MoveDamageCounters(wormadam, target, 1, WormadamFirst);
            }
        }

        /// <summary>Serpentine Strangle: when played from your hand to evolve, you may flip a coin; heads, the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class ServineFCO : Mon
        {
            private const string Strangle = "Serpentine Strangle";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (!TriggerOnce(ctx, Strangle, defending != null, "Serpentine Strangle: flip a coin to Paralyze your opponent's Active Pokémon?")) return;
                if (ctx.Flip() && ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Paralyzed);
            }
        }

        // ================================================================== Fire

        /// <summary>Flare Witch: once during your turn (before your attack), search your deck for a Fire Energy card and attach it to 1 of your Pokémon.</summary>
        private sealed class DelphoxBreakFCO : Mon
        {
            protected override string Activated { get { return "Flare Witch"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fire, 1, "Flare Witch: choose a Fire Energy card");
                foreach (var card in found)
                {
                    var mine = ctx.MyPokemon();
                    var target = ctx.ChoosePokemon(mine, "Flare Witch: attach " + card.Def.Name + " to which of your Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }
        }

        // ================================================================== Water

        /// <summary>Freezing Breath: flip a coin; heads, the opponent's Active Pokémon is Paralyzed; tails, Asleep.</summary>
        private sealed class FreezingBreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Freezing Breath") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, heads ? SpecialCondition.Paralyzed : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Restoring Beam: once during your turn (before your attack), search your deck for a Restored Pokémon and put it onto your Bench.</summary>
        private sealed class OmastarFCO : Mon
        {
            protected override string Activated { get { return "Restoring Beam"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Restored && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1,
                    "Restoring Beam: choose a Restored Pokémon to put onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Dangerous Tentacle: once during your turn (before your attack), switch 1 of your opponent's Benched Pokémon-EX (the XY-era
        /// ones) with their Active Pokémon. The Omastar under it keeps Restoring Beam (a BREAK keeps the Abilities, v92).
        /// </summary>
        private sealed class OmastarBreakFCO : Mon
        {
            protected override string Activated { get { return "Dangerous Tentacle"; } }

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentBench().Where(m => m.Def.IsPokemonEX && ctx.CanAffect(m)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Dangerous Tentacle: switch which of your opponent's Benched Pokémon-EX into the Active Spot?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
            }
        }

        /// <summary>Burning Icicles: if this Pokémon has any Fire Energy attached, also 20 damage to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class BurningIciclesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Icicles") return false;
                bool fire = ctx.Engine.HasEnergyOfType(ctx.Pokemon, EnergyType.Fire);
                ctx.DamageDefending(atk.Damage);
                if (fire) CardsLOR1.DamageBenched(ctx, 20, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Hand Block: if you have a Stadium card in play, your opponent can't attach Special Energy cards from their hand to their Pokémon.</summary>
        private sealed class BarbaracleFCO : Mon
        {
            public override bool BlocksEnergyCard(EffectContext src, int player, CardInstance card)
            {
                if (src.Pokemon == null || player == src.Player || card == null || !card.Def.IsSpecialEnergy) return false;
                return src.State.Stadium != null && src.State.StadiumOwner == src.Player;
            }
        }

        // ================================================================== Lightning / Psychic

        /// <summary>Plasmagic: move 2 damage counters from each of your Pokémon (1 if that's all it has) to the opponent's Active Pokémon.</summary>
        private sealed class PlasmagicMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plasmagic") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                {
                    var defending = ctx.Defending;
                    if (defending == null) break;
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Engine.MoveDamageCounters(mon, defending, Math.Min(2, mon.DamageCounters), atk.Name);
                }
                return true;
            }
        }

        /// <summary>
        /// Suppression: 3 damage counters on each of the opponent's Pokémon that has any Energy attached. Kinesis (Alakazam-EX's
        /// Ability) happens when M Alakazam-EX is played on it: see MAlakazamEX.
        /// </summary>
        private sealed class SuppressionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Suppression") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Count > 0))
                    if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }

        /// <summary>
        /// Kinesis (Alakazam-EX's Ability): "When you play M Alakazam-EX from your hand to evolve this Pokémon, before it evolves,
        /// you may put 2 damage counters on your opponent's Active Pokémon and 3 damage counters on 1 of your opponent's Benched
        /// Pokémon." Asked as M Alakazam-EX is played on an Alakazam-EX with Kinesis. Zen Force: 30 more for each damage counter
        /// on the opponent's Active Pokémon.
        /// </summary>
        private sealed class MAlakazamEX : Mon
        {
            private const string Kinesis = "Kinesis";
            private static readonly Mon Zen = new CardsMEW1.CounterBonusMon("Zen Force", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || me.Stack.Count < 2) return;
                var lower = me.Stack[me.Stack.Count - 2].Def;
                if (!lower.Abilities.Any(a => a.Name == Kinesis)) return;
                bool any = ctx.Opp.Active != null || ctx.Opp.Bench.Count > 0;
                if (!TriggerOnce(ctx, Kinesis, any, "Kinesis: put 2 damage counters on your opponent's Active Pokémon and 3 on 1 of their Benched Pokémon?")) return;
                if (ctx.Opp.Active != null) ctx.PlaceCounters(ctx.Opp.Active, 2);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return;
                var target = ctx.ChoosePokemon(bench, "Kinesis: put 3 damage counters on which of your opponent's Benched Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Zen.HandleAttack(ctx, atk); }
        }

        /// <summary>Balloon Bomb: flip 2 coins; for each heads, discard the top 2 cards of the opponent's deck.</summary>
        private sealed class BalloonBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Balloon Bomb") return false;
                int heads = ctx.FlipCoins(2);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 2 * heads);
                return true;
            }
        }

        /// <summary>Thick Liquid: both Active Pokémon are now Confused and Poisoned.</summary>
        private sealed class ThickLiquidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thick Liquid") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in new[] { ctx.Opp.Active, ctx.Me.Active })
                {
                    if (mon == null) continue;
                    ctx.ApplyCondition(mon, SpecialCondition.Confused);
                    ctx.ApplyCondition(mon, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>
        /// Memories of Dawn: this Pokémon can use the attacks of any of your Basic Pokémon in play (it still needs the Energy).
        /// Encounter: search your deck for a Pokémon and put it into your hand.
        /// </summary>
        private sealed class MewFCO : Mon
        {
            private static readonly Mon Encounter = new SearchAttackMon("Encounter", d => d.IsPokemon, 1);

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (src.Pokemon == null) return list;
                var seen = new HashSet<string>();
                foreach (var other in src.Me.InPlay())
                {
                    if (other == src.Pokemon || !other.Def.IsBasicPokemon || !seen.Add(other.Def.BaseKey)) continue;
                    for (int i = 0; i < other.Def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(other.Def, i));
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Encounter.HandleAttack(ctx, atk); }
        }

        /// <summary>Head Walking: put a Basic Pokémon from the opponent's discard pile onto their Bench, then 3 damage counters on it.</summary>
        private sealed class HeadWalkingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Head Walking") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return true;
                var basics = ctx.Opp.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (basics.Count == 0) return true;
                var card = ctx.ChooseCards(basics, 1, 1, "Head Walking: put which Basic Pokémon from your opponent's discard pile onto their Bench?")[0];
                if (!ctx.Opp.Discard.Remove(card)) return true;
                var mon = ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
                if (mon != null && ctx.InPlay(mon)) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }

        /// <summary>
        /// Mini Link / Double Link / Link Fusion: 30 more if Solosis is on your Bench, 60 more if Duosion is, 90 more if Reuniclus is
        /// (each attack checks the first 1, 2 or 3 of those; any number of them can count at once).
        /// </summary>
        private sealed class LinkMon : Mon
        {
            private static readonly string[] Names = { "Solosis", "Duosion", "Reuniclus" };
            private readonly string attack;
            private readonly int levels;

            public LinkMon(string attack, int levels)
            {
                this.attack = attack;
                this.levels = levels;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int bonus = 0;
                for (int i = 0; i < levels; i++)
                {
                    string name = Names[i];
                    if (ctx.Me.Bench.Any(m => m.Def.Name == name)) bonus += 30 * (i + 1);
                }
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }
    }
}
