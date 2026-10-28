using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unified Minds (UNM) part 2: Lightning, Psychic and Fighting (UNM 63-124). Mewtwo &amp; Mew-GX's Perfection uses the attacks of
    /// the Pokémon-GX and Pokémon-EX on your Bench and in your discard pile (ExtraAttacks); Latios-GX's Clear Vision-GX uses up the
    /// opponent's GX attack for the game.
    /// </summary>
    public static class CardsUNM2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UNM-63", new SwitchSelfMon("Wild River", false));
            r.RegisterByKey("UNM-64", new CardsMEG.ChargeSelfMon("Generate Electricity", EnergyType.Lightning, 1));
            r.RegisterByKey("UNM-65", new OversparkMon());
            r.RegisterByKey("UNM-66", new EelektrossUNM());
            r.RegisterByKey("UNM-67", new CardsSTS2.SpiritedThrowMon("Trap Bolt", 30));
            r.RegisterByKey("UNM-68", new ComboMon(new CardsFST2.ConditionBonusMon("Thunderous Gale", 50, TornadusOnBench), new CardsASR1.BenchRecoilMon("Raging Thunder", 40)));
            r.RegisterByKey("UNM-69", new NatureDiveMon());
            r.RegisterByKey("UNM-70", new ComboMon(new CardsFST2.ConditionBonusMon("Three Mirrors", 90, OpponentHas3Prizes), new ConditionAttackMon("Signal Beam", SpecialCondition.Confused)));
            r.RegisterByKey("UNM-71", new MewtwoMewGX());
            r.RegisterByKey("UNM-72", new EspeonDeoxysGX());
            r.RegisterByKey("UNM-73", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("UNM-74", new ComboMon(new ConditionAttackMon("Mind Bend", SpecialCondition.Confused), new FullCleanMon()));
            r.RegisterByKey("UNM-75", new SpiritSmashMon());
            r.RegisterByKey("UNM-76", new JynxUNM());
            r.RegisterByKey("UNM-77", new WynautUNM());
            r.RegisterByKey("UNM-78", new LatiosGX());
            r.RegisterByKey("UNM-79", new JirachiGX());
            r.RegisterByKey("UNM-81", new DrifblimUNM());
            r.RegisterByKey("UNM-82", new CardsASC.RandomDiscardMon("Knock Off"));
            r.RegisterByKey("UNM-83", new UxieUNM());
            r.RegisterByKey("UNM-84", new BenchSearchAttackMon("First Contact", 3));
            r.RegisterByKey("UNM-85", new CardsLOR1.CountersAnyWayMon("Psypower", 3));
            r.RegisterByKey("UNM-86", new GiratinaUNM());
            r.RegisterByKey("UNM-87", new ComboMon(new CardsTWM1.HealSelfMon("Aurora Gain", 30), new CardsASC.SelfDiscardTypeMon("Luminous Blade", EnergyType.Psychic, 1)));
            r.RegisterByKey("UNM-88", new FutureSightMon());
            r.RegisterByKey("UNM-89", new RestWellMon());
            r.RegisterByKey("UNM-90", new ConditionAttackMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("UNM-91", new MysteriousNoiseMon());
            r.RegisterByKey("UNM-92", new SearchAttackMon("Lucky Find", IsItemCard, 1));
            r.RegisterByKey("UNM-93", new CardsFST1.NoWeaknessResistanceMon("Slashing Cutter"));
            r.RegisterByKey("UNM-94", new CardsFST1.NoWeaknessResistanceMon("Slashing Cutter"));
            r.RegisterByKey("UNM-95", new AegislashUNM());
            r.RegisterByKey("UNM-101", new ComboMon(new CardsASC.SelfGuardMon("Barrier Attack", 30), new CardsSIT1.SpecialLaserMon("Special Laser", 60)));
            r.RegisterByKey("UNM-102", new CardsFST2.ConditionBonusMon("Last Scene", 130, BothOnLastPrize));
            r.RegisterByKey("UNM-103", new CardsASC.SelfGuardMon("Bedrock Press", 20));
            r.RegisterByKey("UNM-104", new ComboMon(new CardsSP1.AttachUpToFromDiscardToSelfMon("Ground Stream", EnergyType.Fighting, 2), new GigatonShakeMon()));
            r.RegisterByKey("UNM-105", new ComboMon(new CardsMEG.WeakenDefendingMon("Growl", 20), new CoinsMon("Bonemerang", 2)));
            r.RegisterByKey("UNM-106", new AerodactylGX());
            r.RegisterByKey("UNM-107", new TurnTheTablesMon());
            r.RegisterByKey("UNM-108", new ComboMon(new ConditionAttackMon("Spore", SpecialCondition.Asleep), new CardsRCL1.ConditionBonusMon("Pre-Dawn Strike", SpecialCondition.Asleep, 90)));
            r.RegisterByKey("UNM-109", new LockAfterAttackMon("Spirited Headbutt", true));
            r.RegisterByKey("UNM-110", new ComboMon(new CardsLOR1.CountersAnyWayMon("Pure Power", 4), new CardsFST2.ConditionBonusMon("Master Strike", 60, HasKarateBelt)));
            r.RegisterByKey("UNM-111", new ComboMon(new SearchAttackMon("Deep Sea Boring", IsTrainerCard, 1), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("UNM-114", new GarchompUNM());
            r.RegisterByKey("UNM-117", new LucarioUNM());
            r.RegisterByKey("UNM-119", new RototillerMon());
            r.RegisterByKey("UNM-120", new CardsWHT.FlipMoreMon("Endeavor", 2, 20));
            r.RegisterByKey("UNM-121", new ComboMon(new SwitchSelfMon("U-turn", false), new CardsDRI2.CrunchMon("Hyper Beam")));
            r.RegisterByKey("UNM-122", new CardsFST2.ConditionBonusMon("Cavern Counter", 150, WholeBenchDamaged));
            r.RegisterByKey("UNM-123", new ComboMon(new TagCheerMon(), new CardsMEG.FlipUntilTailsBonusMon("Shooting Star Pirouette", 30)));
            r.RegisterByKey("UNM-124", new ZygardeUNM());
        }

        // ================================================================== Shared helpers

        internal static bool IsItemCard(CardDef d) { return d.IsItem; }
        internal static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }
        private static bool TornadusOnBench(EffectContext ctx) { return ctx.Me.Bench.Any(m => m.Name == "Tornadus"); }
        private static bool OpponentHas3Prizes(EffectContext ctx) { return ctx.Opp.Prizes.Count == 3; }
        private static bool BothOnLastPrize(EffectContext ctx) { return ctx.Me.Prizes.Count == 1 && ctx.Opp.Prizes.Count == 1; }
        private static bool HasKarateBelt(EffectContext ctx) { return ctx.Pokemon != null && ctx.Pokemon.HasToolNamed("Karate Belt"); }

        /// <summary>
        /// Cavern Counter: "if all of your Benched Pokémon have at least 1 damage counter on them". With no Benched Pokémon there's
        /// nothing to check, so no bonus.
        /// </summary>
        private static bool WholeBenchDamaged(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && ctx.Me.Bench.All(m => m.DamageCounters > 0); }

        /// <summary>
        /// "Look at the top N cards of either player's deck and put them back in any order": the player picks the deck, then the
        /// order, top card first.
        /// </summary>
        internal static void ReorderEitherTop(EffectContext ctx, int count, string source)
        {
            int player = ctx.Player;
            if (ctx.Opp.Deck.Count > 0)
            {
                if (ctx.Me.Deck.Count == 0) player = ctx.Opponent;
                else if (ctx.ChooseOption(source + ": look at the top " + count + " cards of which deck?", "Your deck", "Your opponent's deck") == 1) player = ctx.Opponent;
            }
            var deck = ctx.State.Players[player].Deck;
            var top = deck.Take(count).ToList();
            if (top.Count == 0) return;
            string whose = player == ctx.Player ? "their" : ctx.Engine.Possessive(player);
            if (top.Count == 1)
            {
                ctx.Engine.ShowCards(ctx.Player, top, source, "The top card of " + (player == ctx.Player ? "your" : "your opponent's") + " deck.", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of " + whose + " deck.");
                return;
            }
            foreach (var c in top) deck.Remove(c);
            var ordered = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = ctx.ChooseCards(left, 1, 1, source + ": choose the card to put " + (ordered.Count == 0 ? "on top of the deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                ordered.Add(pick);
                left.Remove(pick);
            }
            ordered.Add(left[0]);
            deck.InsertRange(0, ordered);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the top " + ordered.Count + " cards of " + whose + " deck back in any order.");
        }

        // ================================================================== Lightning

        /// <summary>Overspark: discard all Lightning Energy from this Pokémon; 30 damage for each card discarded.</summary>
        internal sealed class OversparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overspark") return false;
                var me = ctx.Pokemon;
                int n = 0;
                foreach (var card in me.Energy.Where(x => CardsHIF1.Provides(ctx, me, x, EnergyType.Lightning)).ToList())
                    if (ctx.Engine.DiscardEnergy(me, card)) n++;
                if (n > 0) ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>
        /// Electric Swamp (from the hand): with at least 4 Lightning Energy cards in play, put this Pokémon onto your Bench, then you
        /// may move any number of Lightning Energy from your other Pokémon to it. Hover Over: the Defending Pokémon can't retreat.
        /// </summary>
        private sealed class EelektrossUNM : Mon
        {
            private static readonly Mon HoverOver = new NoRetreatMon("Hover Over");

            private static int LightningCards(EffectContext ctx)
            {
                int n = 0;
                foreach (var mon in ctx.MyPokemon())
                    foreach (var e in mon.Energy)
                        if (CardsHIF1.Provides(ctx, mon, e, EnergyType.Lightning)) n++;
                return n;
            }

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && ctx.BenchSpace > 0
                       && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def) && LightningCards(ctx) >= 4;
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!ctx.Me.Hand.Contains(ctx.Card) || ctx.BenchSpace <= 0) return;
                ctx.Me.Hand.Remove(ctx.Card);
                var me = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                var own = new EffectContext(ctx.Engine, ctx.Player, ctx.Card, me);
                Func<List<PokemonInPlay>> givers = () => own.MyPokemon(m => m != me && m.Energy.Any(x => CardsHIF1.Provides(own, m, x, EnergyType.Lightning)));
                if (givers().Count == 0 || !own.AskYesNo("Electric Swamp: move Lightning Energy from your other Pokémon to " + me.Name + "?")) return;
                RepeatableAbility.Repeat(own, () => givers().Count > 0 && own.InPlay(me), offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(own, givers(), "Electric Swamp: move a Lightning Energy from which Pokémon to " + me.Name + "?"
                        + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var lightning = from.Energy.Where(x => CardsHIF1.Provides(own, from, x, EnergyType.Lightning)).ToList();
                    var energy = lightning.Select(x => x.Def.Name).Distinct().Count() == 1 ? lightning[0] : own.ChooseCards(lightning, 1, 1, "Electric Swamp: move which Energy?")[0];
                    return own.Engine.MoveEnergy(from, me, energy);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HoverOver.HandleAttack(ctx, atk); }
        }

        /// <summary>Nature Dive: if the opponent's Active Pokémon is an Ultra Beast, 100 more damage and discard 2 Energy from this Pokémon.</summary>
        private sealed class NatureDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nature Dive") return false;
                bool beast = ctx.Defending != null && ctx.Defending.Def.IsUltraBeast;
                ctx.DamageDefending(atk.Damage + (beast ? 100 : 0));
                if (beast && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Nature Dive: discard 2 Energy from " + ctx.Pokemon.Name);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Perfection: this Pokémon can use the attacks of the Pokémon-GX and Pokémon-EX on your Bench and in your discard pile (you
        /// still need the Energy). Miraculous Duo-GX: with 1 extra Energy, heal all damage from all of your Pokémon.
        /// </summary>
        private sealed class MewtwoMewGX : Mon
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var seen = new HashSet<string>();
                var sources = src.Me.Bench.Select(m => m.Def).Concat(src.Me.Discard.Select(x => x.Def));
                foreach (var def in sources)
                {
                    if (!def.IsPokemon || !def.IsGXorEX || !seen.Add(def.BaseKey)) continue;
                    for (int i = 0; i < def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(def, i);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Duo-GX") return false;
                bool extra = ExtraEnergy(ctx, atk, 1);
                ctx.DamageDefending(atk.Damage);
                if (!extra) return true;
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Psychic Club: 30 more for each of your Benched Psychic Pokémon. Cross Division-GX: put 10 damage counters on the opponent's
        /// Pokémon in any way you like, 20 with 3 extra Energy.
        /// </summary>
        private sealed class EspeonDeoxysGX : Mon
        {
            private static readonly Mon Club = new CardsASR1.PlusMon("Psychic Club", 30, c => c.Me.Bench.Count(m => c.Engine.IsType(m, EnergyType.Psychic)));

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross Division-GX") return Club.HandleAttack(ctx, atk);
                int counters = ExtraEnergy(ctx, atk, 3) ? 20 : 10;
                ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Full Clean: discard your hand after the damage.</summary>
        private sealed class FullCleanMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Full Clean") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count > 0) ctx.Engine.DiscardHand(ctx.Player);
                return true;
            }
        }

        /// <summary>
        /// Spirit Smash: discard the top card of the opponent's deck; if it's a Pokémon, damage equal to its printed HP to the
        /// opponent's Active Pokémon.
        /// </summary>
        private sealed class SpiritSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirit Smash") return false;
                if (ctx.Opp.Deck.Count == 0 || ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) return true;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                if (milled.Count > 0 && milled[0].Def.IsPokemon && milled[0].Def.Hp > 0) ctx.DamageDefending(milled[0].Def.Hp);
                return true;
            }
        }

        /// <summary>Ominous Posture: once during your turn, move a damage counter from 1 of your Pokémon to another of your Pokémon. Attract Smack: Paralyzed on heads.</summary>
        private sealed class JynxUNM : Mon
        {
            private static readonly Mon Smack = new ParalyzeFlipMon("Attract Smack");

            protected override string Activated { get { return "Ominous Posture"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.MyPokemon().Count > 1 && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0 && !ctx.Engine.CounterMovementBlocked();
            }

            protected override void Ability(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(ctx.MyPokemon(m => m.DamageCounters > 0), "Ominous Posture: move a damage counter from which of your Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Ominous Posture: move it to which of your Pokémon?");
                if (to != null) ctx.Engine.MoveDamageCounters(from, to, 1, "Ominous Posture");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smack.HandleAttack(ctx, atk); }
        }

        /// <summary>Peppy Pick: once during your turn, flip a coin; on heads a random card from the opponent's hand is shuffled into their deck. Your turn ends.</summary>
        private sealed class WynautUNM : Mon
        {
            protected override string Activated { get { return "Peppy Pick"; } }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip() && ctx.Opp.Hand.Count > 0) CardsTWM1.ShuffleRandomFromOpponentHand(ctx, 1);
                ctx.Log("Peppy Pick: " + ctx.Engine.PlayerName(ctx.Player) + "'s turn ends.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>
        /// Clear Vision-GX: the affected player's Pokémon can't use GX attacks. It's applied to each Pokémon (Compendium, 2019):
        /// one it couldn't affect when it was used keeps its GX attacks, and so does one that comes into play later while the
        /// opponent's attack effects can't reach it (Keldeo-GX's Pure Heart). That is checked when it tries to attack.
        /// </summary>
        internal sealed class ClearVisionEffect : ContinuousEffect
        {
            private readonly int affected;
            private readonly HashSet<int> seen;
            private readonly HashSet<int> exempt;
            /// <summary>The Pokémon that used it (Pure Heart looks at whether the attacker is a Pokémon-GX).</summary>
            private readonly PokemonInPlay attacker;

            public ClearVisionEffect(string name, int owner, int affectedPlayer, HashSet<int> seen, HashSet<int> exempt, PokemonInPlay attacker)
                : base(name, owner, -1, int.MaxValue, false)
            {
                affected = affectedPlayer;
                this.seen = seen;
                this.exempt = exempt;
                this.attacker = attacker;
            }

            public override bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string name)
            {
                if (mon == null || mon.Owner != affected || name == null || !name.EndsWith("-GX", StringComparison.Ordinal)) return false;
                if (seen.Contains(mon.Uid)) return !exempt.Contains(mon.Uid);
                return !engine.EffectsPreventedFrom(mon, EffectSourceKind.Attack, Owner, attacker);
            }
        }

        /// <summary>
        /// Power Bind: with 4 or fewer Pokémon in play, this Pokémon can't attack. Tag Purge: prevent all damage from TAG TEAM
        /// Pokémon's attacks to it during the opponent's next turn. Clear Vision-GX: the opponent can't use GX attacks for the rest of
        /// the game. It's an effect on the opponent's Pokémon, not a used-up GX attack (Compendium, 2019): a Pokémon it can't affect
        /// (Keldeo-GX's Pure Heart) keeps its GX attack, Channeler removes it, and Hala doesn't count it as their GX attack.
        /// </summary>
        private sealed class LatiosGX : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Engine.AbilityBlocked(ctx.Pokemon, "Power Bind") || ctx.MyPokemon().Count > 4;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tag Purge")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon))
                        ctx.AddEffect(new CardsPBL.AttackerShieldEffect(ctx.Pokemon.Name + " takes no damage from TAG TEAM Pokémon's attacks (Tag Purge)", ctx.Player,
                            ctx.Pokemon.Uid, ctx.Turn + 1, 9999, m => m.Def.IsTagTeam));
                    return true;
                }
                if (atk.Name == "Clear Vision-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var seen = new HashSet<int>(ctx.OpponentPokemon().Select(m => m.Uid));
                    var exempt = new HashSet<int>(ctx.OpponentPokemon(m => !ctx.CanAffect(m)).Select(m => m.Uid));
                    ctx.AddEffect(new ClearVisionEffect("Clear Vision-GX: " + ctx.Engine.PlayerName(ctx.Opponent) + "'s Pokémon can't use GX attacks for the rest of the game",
                        ctx.Player, ctx.Opponent, seen, exempt, ctx.Pokemon));
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Psychic Zone: Psychic Weakness isn't applied to any Pokémon (yours and your opponent's). Star Search: search for an Energy
        /// card and attach it to 1 of your Psychic Pokémon. Star Shield-GX: prevent all effects of attacks, damage included, done to
        /// it during the opponent's next turn.
        /// </summary>
        private sealed class JirachiGX : Mon
        {
            public override EnergyType? ModifyWeakness(EffectContext src, PokemonInPlay mon, EnergyType? weakness)
            {
                return weakness == EnergyType.Psychic ? (EnergyType?)null : weakness;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Star Search")
                {
                    var psychic = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Psychic));
                    if (ctx.Me.Deck.Count == 0 || psychic.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Star Search: search your deck for an Energy card");
                    foreach (var card in found)
                    {
                        var target = ctx.ChoosePokemon(psychic, "Star Search: attach " + card.Def.Name + " to which of your Psychic Pokémon?");
                        if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Star Shield-GX")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Tag Transport: once during your turn, switch your Active TAG TEAM Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class DrifblimUNM : Mon
        {
            protected override string Activated { get { return "Tag Transport"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Active.Def.IsTagTeam && ctx.Me.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx) { ctx.SwitchMyActive("Tag Transport: switch your Active TAG TEAM Pokémon with which Benched Pokémon?"); }
        }

        /// <summary>Secret Territory: with Mesprit and Azelf in play, Weakness for every Pokémon is x4.</summary>
        private sealed class UxieUNM : Mon
        {
            public override int WeaknessMultiplier(EffectContext src, DamageInfo info)
            {
                if (src.Pokemon == null) return 2;
                bool mesprit = src.Me.InPlay().Any(m => m.Name == "Mesprit");
                bool azelf = src.Me.InPlay().Any(m => m.Name == "Azelf");
                return mesprit && azelf ? 4 : 2;
            }
        }

        /// <summary>Dimension Breach: when you play this Pokémon from your hand onto your Bench, you may discard a Special Energy from the opponent's Active Pokémon. Fade to Black: Confused.</summary>
        private sealed class GiratinaUNM : Mon
        {
            private static readonly Mon FadeToBlack = new ConditionAttackMon("Fade to Black", SpecialCondition.Confused);

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (!ctx.IsMyTurn || target == null) return;
                var special = target.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList();
                if (special.Count == 0 || !ctx.AskYesNo("Dimension Breach: discard a Special Energy from " + target.Name + "?")) return;
                Announce(ctx, "Dimension Breach");
                var card = ctx.ChooseCards(special, 1, 1, "Dimension Breach: discard which Special Energy?")[0];
                ctx.Engine.DiscardEnergy(target, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FadeToBlack.HandleAttack(ctx, atk); }
        }

        /// <summary>Future Sight: look at the top 4 cards of either player's deck and put them back in any order.</summary>
        private sealed class FutureSightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Future Sight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ReorderEitherTop(ctx, 4, "Future Sight");
                return true;
            }
        }

        /// <summary>Rest Well: both Active Pokémon are now Asleep; during your next turn this Pokémon's attacks do 100 more damage to the opponent's Active Pokémon.</summary>
        private sealed class RestWellMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest Well") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.AddEffect(new CardsCRI.NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do 100 more damage during its owner's next turn (Rest Well)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, 100));
                ctx.Engine.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Mysterious Noise: shuffle this Pokémon and everything attached into your deck; the opponent can't play Items during their next turn.</summary>
        private sealed class MysteriousNoiseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mysterious Noise") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during their next turn (Mysterious Noise).");
                return true;
            }
        }

        /// <summary>
        /// Durable Blade: Knocked Out by damage from an opponent's attack, it goes to your hand (attached cards are discarded). Trash
        /// Slash: 10 damage for each Item card in your discard pile, at most 130.
        /// </summary>
        private sealed class AegislashUNM : Mon
        {
            public override bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                return byOpponentsAttack && mon == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trash Slash") return false;
                int items = ctx.Me.Discard.Count(x => x.Def.IsItem);
                ctx.DamageDefending(Math.Min(130, atk.Damage * items));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Gigaton Shake: during your next turn, your Pokémon can't attack (including ones put into play that turn).</summary>
        private sealed class GigatonShakeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigaton Shake") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsOBF2.PlayerCantAttackEffect(ctx.Engine.PlayerName(ctx.Player) + "'s Pokémon can't attack during their next turn (Gigaton Shake)", ctx.Player, ctx.Turn + 2));
                return true;
            }
        }

        /// <summary>Primal Winds: while this Pokémon is Active, the attacks of the opponent's Basic Pokémon cost [C] more. Wild Dive-GX: 50 for each Energy on the opponent's Active Pokémon.</summary>
        private sealed class AerodactylGX : Mon
        {
            private static readonly Mon WildDive = new CardsASR1.TimesMon("Wild Dive-GX", c => c.Defending == null ? 0 : c.Engine.EnergyCount(c.Defending));

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon) || mon == null || mon.Owner == src.Player || mon.Def.Stage != Stage.Basic) return;
                cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WildDive.HandleAttack(ctx, atk); }
        }

        /// <summary>Turn the Tables: if any of the opponent's Pokémon used a GX attack during their last turn, they shuffle their Active Pokémon and everything attached into their deck.</summary>
        private sealed class TurnTheTablesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turn the Tables") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool gx = ctx.Opp.LastAttackTurn == ctx.OpponentsLastTurn && ctx.Opp.LastAttackName != null && ctx.Opp.LastAttackName.EndsWith("-GX");
                var target = ctx.Defending;
                if (!gx || target == null || !ctx.CanAffect(target)) return true;
                ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>
        /// Avenging Aura: while you have more Prize cards left than the opponent, this Pokémon's attacks do 80 more damage to the
        /// opponent's Active Pokémon (before Weakness and Resistance). Over Slice: you may discard an Energy from it for 40 more.
        /// </summary>
        private sealed class GarchompUNM : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Me.Prizes.Count > src.Opp.Prizes.Count ? damage + 80 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Over Slice") return false;
                var me = ctx.Pokemon;
                bool more = me.Energy.Count > 0 && ctx.AskYesNo("Over Slice: discard an Energy from " + me.Name + " for 40 more damage?")
                            && ctx.DiscardEnergyFrom(me, 1, "Over Slice: discard which Energy?") > 0;
                ctx.DamageDefending(atk.Damage + (more ? 40 : 0));
                return true;
            }
        }

        /// <summary>Tag Coach: your TAG TEAM Pokémon take 20 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class LucarioUNM : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target == null || info.Target.Owner != src.Player || !info.AgainstOpponent || damage <= 0) return damage;
                return info.Target.Def.IsTagTeam ? damage - 20 : damage;
            }
        }

        /// <summary>Rototiller: shuffle 4 cards from your discard pile into your deck (all of them with fewer).</summary>
        private sealed class RototillerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rototiller") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                int n = Math.Min(4, pile.Count);
                var picks = pile.Count <= 4 ? pile : ctx.ChooseCards(pile, n, n, "Rototiller: choose 4 cards to shuffle from your discard pile into your deck");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Tag Cheer: attach an Energy card from your hand to 1 of your TAG TEAM Pokémon.</summary>
        private sealed class TagCheerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tag Cheer") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                var targets = ctx.MyPokemon(m => m.Def.IsTagTeam);
                if (energy.Count == 0 || targets.Count == 0) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "Tag Cheer: attach which Energy card from your hand?")[0];
                var target = ctx.ChoosePokemon(targets, "Tag Cheer: attach " + card.Def.Name + " to which of your TAG TEAM Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card)
                    && !target.Energy.Contains(card)) ctx.Me.Hand.Add(card);
                return true;
            }
        }

        /// <summary>
        /// Cellular Companions: while this Pokémon is on your Bench, your Zygarde and Zygarde-GX do 20 more damage to the opponent's
        /// Active Pokémon (before Weakness and Resistance). Boost Fang: attach a Fighting Energy card from your discard pile to 1 of
        /// your Benched Pokémon.
        /// </summary>
        private sealed class ZygardeUNM : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || src.IsActive(src.Pokemon) || info.Attacker == null || info.Attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return info.Attacker.Name == "Zygarde" || info.Attacker.Name == "Zygarde-GX" ? damage + 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Boost Fang") return false;
                ctx.DamageDefending(atk.Damage);
                var fighting = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                if (fighting.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(fighting, 1, 1, "Boost Fang: attach which Fighting Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Boost Fang: attach " + card.Def.Name + " to which of your Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }
    }
}
