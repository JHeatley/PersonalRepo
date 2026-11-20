using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// v174: Aura Seeker (ASK), the Japanese set Flowing Fists comes from (PokéBeach's translations, Japanese numbering). Sigilyph
    /// ex (31), Riolu (44) and Mega Lucario Z ex (57) are prints of Flowing Fists' cards and use their scripts. Pokémon whose
    /// attacks only do their printed damage need no script of their own but are registered as plain: Weepinbell, Yanma, Grovyle,
    /// Karrablast, Wooper, Meditite, Machop, Machoke, Barboach, Kubfu, Inkay, Varoom.
    /// </summary>
    public static class CardsASK
    {
        private const string Fervor = "Crasher Wake's Fervor";

        public static void Register(CardScriptRegistry r)
        {
            foreach (var n in new[] { 2, 4, 7, 9, 17, 28, 37, 38, 42, 48, 51, 60 }) r.RegisterByKey("ASK-" + n, new PlainMon());

            // Grass
            r.RegisterByKey("ASK-1", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1));
            r.RegisterByKey("ASK-3", new VictreebelASK());
            r.RegisterByKey("ASK-5", new CardsASC.CounterMultiplierMon("Damage Beat"));
            r.RegisterByKey("ASK-6", new DodgeMon("Hide", false));
            r.RegisterByKey("ASK-8", new Fx()
                .With(new ConditionAttackMon("Poison Claw", SpecialCondition.Poisoned))
                .Damage("Assassin Blade", (c, a) => a.Damage + 50 * ConditionCount(c.Defending)));
            r.RegisterByKey("ASK-10", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("ASK-11", new Fx().Damage("Stealth Slash", (c, a) => a.Damage * c.OpponentPokemon(m => m.Def.IsEvolution).Count));

            // Fire
            r.RegisterByKey("ASK-12", new CardsPFL.FlipUntilTailsTimesMon("Continuous Steps"));
            r.RegisterByKey("ASK-13", new NinetalesASK());

            // Water
            r.RegisterByKey("ASK-14", new CardsMEG.SnipeMon("Splatter", 10));
            r.RegisterByKey("ASK-15", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("ASK-16", new BlastoiseASK());
            r.RegisterByKey("ASK-18", new Fx().Damage("Hot-Blooded Head", (c, a) => a.Damage + (RevealFervor(c, 1, "Hot-Blooded Head", false) > 0 ? 120 : 0)));
            r.RegisterByKey("ASK-19", new SearchAttackMon("Guide", d => d.IsSupporter, 1));
            r.RegisterByKey("ASK-20", new Fx().Damage("Hustle Screw", (c, a) => a.Damage + 100 * RevealFervor(c, 99, "Hustle Screw", true)));
            r.RegisterByKey("ASK-21", new Fx().Effect("Hail", (c, a) => { foreach (var m in c.OpponentPokemon()) c.DamageTarget(m, 20); }));

            // Lightning
            r.RegisterByKey("ASK-22", new Fx().Effect("Passion Dance", (c, a) => PassionDance(c)));
            r.RegisterByKey("ASK-23", new DrawAttackMon("Quick Draw", 1));
            r.RegisterByKey("ASK-24", new Fx().Damage("Assault Bolt", (c, a) => a.Damage + (c.Defending != null && c.Defending.Tools.Count > 0 ? 80 : 0)));
            r.RegisterByKey("ASK-25", new PawmotASK());

            // Psychic
            r.RegisterByKey("ASK-26", new CardsMEG.WeakenDefendingMon("Scary Face", 30));
            r.RegisterByKey("ASK-27", new XatuASK());
            r.RegisterByKey("ASK-29", new Fx()
                .Effect("Yoga Power", (c, a) => c.PlaceCountersAnyWay(3, () => c.OpponentPokemon()))
                .With(new RecoilMon("Guts Fight", 20)));
            r.RegisterByKey("ASK-30", new Fx().After("Bloody Fear", (c, a) => BloodyFear(c)));
            r.RegisterByKey("ASK-32", new ComboMon(new BenchSearchAttackMon("Welcoming Song", 3), new ConditionAttackMon("Perplex", SpecialCondition.Confused)));
            r.RegisterByKey("ASK-33", new Fx().Damage("Sage's Command", (c, a) => a.Damage * c.Me.Bench.Count(m => m.Def.IsMegaEx)));
            r.RegisterByKey("ASK-34", new Fx().Effect("Future Sight", (c, a) => FutureSight(c, 3, "Future Sight")));
            r.RegisterByKey("ASK-35", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("ASK-36", new Fx()
                .With(new ConditionAttackMon("Psybeam", SpecialCondition.Confused))
                .Damage("Magic Tentacle", (c, a) => a.Damage + 100 * MagicTentacle(c)));

            // Fighting
            r.RegisterByKey("ASK-39", new MachampASK());
            r.RegisterByKey("ASK-40", new ComboMon(new CoinsMon("Triple Kick", 3), new RecoilMon("Guts Fight", 20)));
            r.RegisterByKey("ASK-41", new Fx().After("Mountain Munch", (c, a) => { if (c.Opp.Deck.Count > 0) c.Engine.DiscardTopOfDeck(c.Opponent, 1); }));
            r.RegisterByKey("ASK-43", new Fx().On("Great Tremor", (c, a) =>
            {
                if (!CardsTWM1.DiscardStadiumInPlay(c)) { c.Log(c.Pokemon.Name + "'s Great Tremor did nothing: there's no Stadium in play."); return true; }
                c.DamageDefending(a.Damage);
                return true;
            }));
            r.RegisterByKey("ASK-45", new ComboMon(new CardsE3B.OpponentSwitchesMon("Circle Throw"), new RecoilMon("Guts Fight", 20)));
            r.RegisterByKey("ASK-46", new HawluchaASK());
            r.RegisterByKey("ASK-47", new FalinksASK());
            r.RegisterByKey("ASK-49", new CardsFLF1.OptionalTypeDiscardBonusMon("Wicked Blow", EnergyType.Darkness, 140));
            r.RegisterByKey("ASK-50", new Fx().On("Torrential Strike", (c, a) => TorrentialStrike(c, a)));

            // Darkness
            r.RegisterByKey("ASK-52", new MalamarASK());
            r.RegisterByKey("ASK-53", new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned));
            r.RegisterByKey("ASK-54", new Fx()
                .Effect("Toxic Gem", (c, a) => ToxicGem(c))
                .With(new LockAfterAttackMon("Great Spin", false)));
            r.RegisterByKey("ASK-55", new Fx().Damage("Heavy Bomber", (c, a) => a.Damage - (c.Defending == null ? 0 : 50 * c.Engine.RetreatCostOf(c.Defending))));

            // Metal
            r.RegisterByKey("ASK-56", new Fx().Damage("Big Jaw", (c, a) => a.Damage * c.Opp.Hand.Count));
            r.RegisterByKey("ASK-58", new Fx().Damage("Cavalry Thrust", (c, a) => a.Damage * c.OpponentPokemon(m => m.Def.IsBasicPokemon).Count));
            r.RegisterByKey("ASK-59", new TogedemaruExASK());
            r.RegisterByKey("ASK-61", new Fx().On("Long Drive", (c, a) =>
            {
                c.DamageDefending(a.Damage);
                c.DamageOneBenched(100);
                if (c.InPlay(c.Pokemon)) c.Engine.DiscardAllEnergy(c.Pokemon);
                return true;
            }));
            r.RegisterByKey("ASK-62", new Fx()
                .Effect("Burrow and Resupply", (c, a) => BurrowAndResupply(c))
                .With(new CardsTWM1.SelfDiscardMon("Smash Head", 2)));

            // Dragon
            r.RegisterByKey("ASK-63", new VibravaASK());
            r.RegisterByKey("ASK-64", new Fx().After("Desert Wind", (c, a) => DesertWind(c)));
            r.RegisterByKey("ASK-65", new Fx().After("Dragon Pulse", (c, a) => DiscardOwnTop(c, 1)));
            r.RegisterByKey("ASK-66", new Fx().After("Dragon Pulse", (c, a) => DiscardOwnTop(c, 2)));
            r.RegisterByKey("ASK-67", new Fx()
                .Damage("Dragon Avenger", (c, a) => a.Damage * c.Me.Discard.Count(x => x.Def.IsPokemon && x.Def.PokemonType == EnergyType.Dragon))
                .With(new CardsPAF2.IgnoreDefenderEffectsMon("Sonic Edge")));
            r.RegisterByKey("ASK-68", new Fx()
                .After("Power Breath", (c, a) => PowerBreath(c))
                .With(new LockAfterAttackMon("Latent Power", false)));

            // Colorless
            r.RegisterByKey("ASK-69", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("ASK-70", new PowerFx("Large Coin",
                c => c.Me.Deck.Count > 0 && c.MyPokemon(m => m.Def.Attacks.Any(x => x.Name == "Pay Day")).Count >= 2,
                c => c.Draw(2),
                new Fx().Damage("Greedy Slash", (c, a) => a.Damage * c.Me.Hand.Count), anyCondition: true));
            r.RegisterByKey("ASK-71", new KecleonASK());
            r.RegisterByKey("ASK-72", new SearchAttackMon("Thudding March", d => d.IsPokemon && d.RetreatCost == 4, 3));
            r.RegisterByKey("ASK-73", new Fx().On("Destructive Hammer", (c, a) =>
            {
                int max = Math.Min(10, c.Me.Deck.Count);
                int n = max == 0 ? 0 : c.AskNumber(0, max, "Destructive Hammer: discard how many cards from the top of your deck (10 more damage each)?");
                int discarded = n > 0 ? c.Engine.DiscardTopOfDeck(c.Player, n).Count : 0;
                c.DamageDefending(a.Damage + 10 * discarded);
                return true;
            }));

            // Trainers
            r.RegisterByKey("ASK-74", new BondingSash());
            r.RegisterByKey("ASK-75", new KorrinasSeriousness());
            r.RegisterByKey("ASK-76", new CrasherWakesFervor());
            r.RegisterByKey("ASK-77", new Reporter());
            r.RegisterByKey("ASK-78", new FightArena());
        }

        // ================================================================== Shared helpers

        private static readonly SpecialCondition[] Conditions =
            { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

        /// <summary>How many Special Conditions affect a Pokémon (Assassin Blade).</summary>
        internal static int ConditionCount(PokemonInPlay mon)
        {
            return mon == null ? 0 : Conditions.Count(mon.Has);
        }

        /// <summary>
        /// Reveal Crasher Wake's Fervor cards from the hand (up to max, optional). Hot-Blooded Head keeps the card in the hand; Hustle
        /// Screw shuffles the revealed cards into the deck. Returns how many were revealed.
        /// </summary>
        internal static int RevealFervor(EffectContext ctx, int max, string attack, bool shuffleIn)
        {
            var fervors = ctx.Me.Hand.Where(c => c.Def.Name == Fervor).ToList();
            if (fervors.Count == 0) return 0;
            List<CardInstance> picks;
            if (max == 1) picks = ctx.AskYesNo(attack + ": reveal a " + Fervor + " from your hand for 120 more damage?") ? fervors.Take(1).ToList() : new List<CardInstance>();
            else picks = ctx.ChooseCards(fervors, 0, Math.Min(max, fervors.Count), attack + ": reveal any number of " + Fervor + " cards from your hand (100 more damage each)");
            if (picks.Count == 0) return 0;
            ctx.Engine.Reveal(ctx.Player, picks, "revealed for " + attack);
            if (shuffleIn)
            {
                foreach (var c in picks)
                {
                    ctx.Me.Hand.Remove(c);
                    ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
            }
            return picks.Count;
        }

        /// <summary>Passion Dance: look at the opponent's hand and put up to 3 Basic Pokémon from it onto their Bench.</summary>
        private static void PassionDance(EffectContext ctx)
        {
            ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", "Passion Dance: your opponent's hand", ctx.Source);
            int space = ctx.Engine.BenchSpace(ctx.Opponent);
            var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
            if (space <= 0 || basics.Count == 0) return;
            var picks = ctx.ChooseCards(basics, 0, Math.Min(3, Math.Min(space, basics.Count)), "Passion Dance: put up to 3 Basic Pokémon from your opponent's hand onto their Bench");
            foreach (var c in picks)
            {
                if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0 || !ctx.Opp.Hand.Remove(c)) continue;
                ctx.Engine.PutOntoBench(ctx.Opponent, c, false);
            }
        }

        /// <summary>Bloody Fear: during the opponent's next turn, no Stadium cards and no Special Energy attached from their hand.</summary>
        private static void BloodyFear(EffectContext ctx)
        {
            ctx.AddEffect(new CardsMEG.StadiumLockEffect("Bloody Fear: no Stadium cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1));
            ctx.AddEffect(new CardsPAF1.SpecialEnergyLockEffect("Bloody Fear: no Special Energy from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1));
        }

        /// <summary>Look at the top N cards of either player's deck and put them back in any order (Future Sight).</summary>
        internal static void FutureSight(EffectContext ctx, int count, string name)
        {
            if (ctx.Me.Deck.Count == 0 && ctx.Opp.Deck.Count == 0) return;
            bool mine = ctx.Opp.Deck.Count == 0 || (ctx.Me.Deck.Count > 0 && ctx.ChooseOption(name + ": look at whose deck?", "Your deck", "Your opponent's deck") == 0);
            if (mine)
            {
                CardsOBF2.ReorderOwnTop(ctx, count, name);
                return;
            }
            var deck = ctx.Opp.Deck;
            var top = deck.Take(count).ToList();
            ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of your opponent's deck", ctx.Player);
            foreach (var card in top) deck.Remove(card);
            var order = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = ctx.ChooseCards(left, 1, 1, name + ": which card goes next from the top of your opponent's deck?")[0];
                order.Add(pick);
                left.Remove(pick);
            }
            order.AddRange(left);
            deck.InsertRange(0, order);
        }

        /// <summary>Magic Tentacle: discard the top card of each player's deck; returns how many of them were Trainer cards.</summary>
        private static int MagicTentacle(EffectContext ctx)
        {
            var discarded = new List<CardInstance>();
            if (ctx.Me.Deck.Count > 0) discarded.AddRange(ctx.Engine.DiscardTopOfDeck(ctx.Player, 1));
            if (ctx.Opp.Deck.Count > 0) discarded.AddRange(ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1));
            return discarded.Count(c => c.Def.IsTrainer);
        }

        /// <summary>Torrential Strike: you may discard a Water Energy from this Pokémon for 40 to each of the opponent's Benched Pokémon.</summary>
        private static bool TorrentialStrike(EffectContext ctx, AttackDef atk)
        {
            var me = ctx.Pokemon;
            bool spread = false;
            if (ctx.Opp.Bench.Count > 0 && me.Energy.Any(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water))
                && ctx.AskYesNo("Torrential Strike: discard a Water Energy from " + me.Name + " to do 40 damage to each of your opponent's Benched Pokémon?"))
                spread = ctx.DiscardEnergyFrom(me, 1, "Torrential Strike: discard a Water Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)) > 0;
            ctx.DamageDefending(atk.Damage);
            if (spread) foreach (var m in ctx.OpponentBench()) ctx.DamageTarget(m, 40);
            return true;
        }

        /// <summary>Toxic Gem: damage counters on 2 of the opponent's Pokémon until each has 60 HP left.</summary>
        private static void ToxicGem(EffectContext ctx)
        {
            var targets = ctx.OpponentPokemon(m => ctx.RemainingHp(m) > 60);
            if (targets.Count == 0) return;
            int n = Math.Min(2, targets.Count);
            var picks = ctx.ChoosePokemonMany(targets, n, n, "Toxic Gem: choose 2 of your opponent's Pokémon (damage counters until they have 60 HP left)");
            foreach (var m in picks)
            {
                int counters = (ctx.RemainingHp(m) - 60) / 10;
                if (counters > 0) ctx.PlaceCounters(m, counters);
            }
        }

        /// <summary>Burrow and Resupply: switch this Pokémon with a Benched Pokémon, then attach a Basic Metal Energy from the discard pile to it.</summary>
        private static void BurrowAndResupply(EffectContext ctx)
        {
            var me = ctx.Pokemon;
            if (ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
            if (!ctx.InPlay(me)) return;
            var metal = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal);
            if (metal.Count == 0) return;
            var pick = ctx.ChooseCards(metal, 1, 1, "Burrow and Resupply: attach a Basic Metal Energy from your discard pile to " + me.Name)[0];
            ctx.Me.Discard.Remove(pick);
            if (!ctx.Engine.AttachEnergyCard(pick, me) && !ctx.Me.Discard.Contains(pick) && !ctx.Me.Hand.Contains(pick)) ctx.Me.Discard.Add(pick);
        }

        /// <summary>Desert Wind: you may shuffle 2 Energy attached to the opponent's Active Pokémon into their deck.</summary>
        private static void DesertWind(EffectContext ctx)
        {
            var defending = ctx.Defending;
            if (defending == null || !ctx.InPlay(defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return;
            if (!ctx.AskYesNo("Desert Wind: shuffle 2 Energy attached to " + defending.Name + " into your opponent's deck?")) return;
            int n = Math.Min(2, defending.Energy.Count);
            var picks = ctx.ChooseCards(defending.Energy.ToList(), n, n, "Desert Wind: choose " + n + " Energy attached to " + defending.Name);
            if (ctx.Engine.ShuffleEnergyIntoDeck(defending, picks) > 0) ctx.Engine.ShuffleDeck(ctx.Opponent);
        }

        private static void DiscardOwnTop(EffectContext ctx, int count)
        {
            if (ctx.Me.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, Math.Min(count, ctx.Me.Deck.Count));
        }

        /// <summary>Power Breath: search the deck for a Basic Energy card and attach it to 1 of your Dragon Pokémon.</summary>
        private static void PowerBreath(EffectContext ctx)
        {
            var dragons = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Dragon));
            if (ctx.Me.Deck.Count == 0 || dragons.Count == 0) return;
            var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Power Breath: choose a Basic Energy card to attach to 1 of your Dragon Pokémon");
            foreach (var card in found)
            {
                var target = ctx.ChoosePokemon(dragons, "Power Breath: attach " + card.Def.Name + " to which of your Dragon Pokémon?");
                if (target == null || !ctx.Engine.AttachEnergyCard(card, target))
                    if (!ctx.Me.Deck.Contains(card) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Deck.Add(card);
            }
            ctx.ShuffleMyDeck();
        }

        // ================================================================== Pokémon

        /// <summary>Acid Trap: if you have a Stadium in play, once during your turn, the opponent's Active Pokémon is Poisoned and Burned. Leaf Drain: heal 30 from this Pokémon.</summary>
        private sealed class VictreebelASK : PowerFx
        {
            public VictreebelASK() : base("Acid Trap",
                c => c.State.Stadium != null && c.State.Stadium.Owner == c.Player && c.Opp.Active != null,
                c =>
                {
                    var active = c.Opp.Active;
                    c.ApplyCondition(active, SpecialCondition.Poisoned);
                    c.ApplyCondition(active, SpecialCondition.Burned);
                },
                new CardsTWM1.HealSelfMon("Leaf Drain", 30), anyCondition: true) { }
        }

        /// <summary>Deception: if this Pokémon is Knocked Out while you have exactly 5 cards in your hand, the opponent takes no Prize cards for it. Searing Flame: Burned.</summary>
        private sealed class NinetalesASK : BodyMon
        {
            public NinetalesASK() : base(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)) { }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut != src.Pokemon || taker == src.Player || src.Me.Hand.Count != 5) return prizes;
                src.Log("Deception: " + src.Engine.PlayerName(taker) + " takes no Prize cards for " + knockedOut.Name + ".");
                return 0;
            }
        }

        /// <summary>
        /// Torrential Pump: attacks used by your Water Pokémon do 30 more damage to the opponent's Benched Pokémon (before Weakness and
        /// Resistance). It doesn't stack: only your first Blastoise with a working Torrential Pump adds it.
        /// </summary>
        private sealed class BlastoiseASK : BodyMon
        {
            private const string Pump = "Torrential Pump";

            public BlastoiseASK() : base(null) { }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || damage <= 0 || info.Attacker == null || info.Attacker.Owner != src.Player || info.Target == null) return damage;
                if (info.Target.Owner == src.Player || info.TargetIsActive || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (!src.Engine.IsType(info.Attacker, EnergyType.Water)) return damage;
                var first = src.Me.InPlay().FirstOrDefault(m => m.Def.Abilities.Any(a => a.Name == Pump) && !src.Engine.AbilityBlocked(m, Pump));
                return first == me ? damage + 30 : damage;
            }
        }

        /// <summary>Double Strike: this Pokémon may use an attack it has twice.</summary>
        private sealed class PawmotASK : Mon
        {
            public override bool AttacksTwice(EffectContext src) { return true; }
        }

        /// <summary>
        /// Energy Leap: when you play this Pokémon from your hand to evolve, you may look at the opponent's hand and attach an Energy
        /// card from it to 1 of their Pokémon. Psychic: 40 more for each Energy attached to the opponent's Active Pokémon.
        /// </summary>
        private sealed class XatuASK : Mon
        {
            private const string Leap = "Energy Leap";
            private static readonly Mon Psy = new CardsMEG.DefendingEnergyBonusMon("Psychic", 40);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, Leap, ctx.Opp.Hand.Count > 0 && ctx.Opp.HasPokemonInPlay, Leap + ": look at your opponent's hand?")) return;
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", Leap + ": your opponent's hand", ctx.Source);
                var energy = ctx.Opp.Hand.Where(c => c.Def.Kind == CardKind.Energy).ToList();
                if (energy.Count == 0) return;
                var pick = ctx.ChooseCards(energy, 1, 1, Leap + ": choose an Energy card from your opponent's hand")[0];
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), Leap + ": attach " + pick.Def.Name + " to which of your opponent's Pokémon?");
                if (target == null || !ctx.Opp.Hand.Remove(pick)) return;
                if (!ctx.Engine.AttachEnergyCard(pick, target) && !ctx.Opp.Hand.Contains(pick) && !ctx.Opp.Discard.Contains(pick)) ctx.Opp.Hand.Add(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psy.HandleAttack(ctx, atk); }
        }

        /// <summary>Trained Arms: this Pokémon's attacks aren't affected by Resistance or by effects on the opponent's Active Pokémon. Guts Fight: 20 to itself.</summary>
        private sealed class MachampASK : BodyMon
        {
            public MachampASK() : base(new RecoilMon("Guts Fight", 20)) { }

            public override bool IgnoresResistance(EffectContext src, DamageInfo info) { return src.Pokemon != null && info.Attacker == src.Pokemon; }

            public override bool AttacksIgnoreDefenderEffects(EffectContext src, DamageInfo info) { return src.Pokemon != null && info.Attacker == src.Pokemon; }
        }

        /// <summary>Overflowing Cheer: the Guts Fight attack used by your Pokémon costs [C] less.</summary>
        private sealed class HawluchaASK : BodyMon
        {
            public HawluchaASK() : base(null) { }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon.Owner != src.Player || attack == null || attack.Name != "Guts Fight") return;
                cost.Remove(EnergyType.Colorless);
            }
        }

        /// <summary>
        /// Solidarity Formation: once during your turn, while this Pokémon is Active, for each of your Benched Pokémon with "Falinks"
        /// in its name, search your deck for a Basic Fighting Energy and attach it to that Pokémon. Strong Attack: discard an Energy.
        /// </summary>
        private sealed class FalinksASK : PowerFx
        {
            public FalinksASK() : base("Solidarity Formation",
                c => c.PokemonIsActive && c.Me.Deck.Count > 0 && c.Me.Bench.Any(m => m.Def.Name.Contains("Falinks")),
                c =>
                {
                    foreach (var m in c.Me.Bench.Where(x => x.Def.Name.Contains("Falinks")).ToList())
                    {
                        var found = c.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fighting, 1,
                            "Solidarity Formation: choose a Basic Fighting Energy card to attach to " + m.Name);
                        foreach (var card in found)
                            if (!c.Engine.AttachEnergyCard(card, m) && !c.Me.Deck.Contains(card) && !c.Me.Discard.Contains(card) && !c.Me.Hand.Contains(card)) c.Me.Deck.Add(card);
                    }
                    c.ShuffleMyDeck();
                },
                new CardsTWM1.SelfDiscardMon("Strong Attack", 1), anyCondition: true) { }
        }

        /// <summary>Skill Control: use 1 of the attacks of the opponent's Active Mega Evolution Pokémon ex as this attack.</summary>
        private sealed class MalamarASK : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Skill Control") return false;
                var defending = ctx.Defending;
                if (defending == null || !defending.Def.IsMegaEx)
                {
                    ctx.Log(ctx.Pokemon.Name + "'s Skill Control did nothing: your opponent's Active Pokémon isn't a Mega Evolution Pokémon ex.");
                    return true;
                }
                CardDef source;
                int index;
                if (ChooseCopiedAttack(ctx, defending, "Skill Control: choose 1 of " + defending.Name + "'s attacks to use", out source, out index))
                    UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>Dual Type: this Pokémon is both Lightning and Metal. Full-Power Needle: during your next turn, it does 60 damage.</summary>
        private sealed class TogedemaruExASK : BodyMon
        {
            private const string Needle = "Full-Power Needle";

            public TogedemaruExASK() : base(new Fx().On(Needle, (c, a) =>
            {
                int uid = c.Pokemon.Uid;
                bool weakened = c.Engine.ActiveEffects().OfType<AttackBoostEffect>().Any(e => e.TargetUid == uid && e.AttackName == Needle);
                c.DamageDefending(weakened ? 60 : a.Damage);
                if (c.InPlay(c.Pokemon))
                {
                    var tired = new AttackBoostEffect(c.Pokemon.Name + "'s " + Needle + " does 60 damage during its next turn", c.Player, uid, c.Turn + 2, Needle, 0);
                    tired.ActiveFromTurn = c.Turn + 2;
                    c.AddEffect(tired);
                }
                return true;
            })) { }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Lightning)) types.Add(EnergyType.Lightning);
                if (!types.Contains(EnergyType.Metal)) types.Add(EnergyType.Metal);
            }
        }

        /// <summary>Vibrating Wings: once during your turn, while this card is in your hand, discard it and look at the top card of your deck.</summary>
        private sealed class VibravaASK : Mon
        {
            private const string Wings = "Vibrating Wings";

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && !ctx.NamedLimitUsedThisTurn(Wings)
                       && !ctx.Engine.OutOfPlayAbilitiesBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!CanUseFromHand(ctx) || !ctx.Me.Hand.Remove(ctx.Card)) return;
                ctx.MarkNamedLimit(Wings);
                ctx.Me.Discard.Add(ctx.Card);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " used " + ctx.Card.Def.Name + "'s Ability " + Wings + " from the hand and discarded it.", ctx.Card.Id, -1);
                var top = ctx.LookAtTop(1);
                if (top.Count > 0) ctx.Engine.ShowCards(ctx.Player, top, "Top of your deck", Wings + ": the top card of your deck", ctx.Source);
            }
        }

        /// <summary>Synchro Body: while in play, this Pokémon also has the types of all your Benched Pokémon. Pounce: flip for 60 more.</summary>
        private sealed class KecleonASK : BodyMon
        {
            public KecleonASK() : base(new FlipBonusMon("Pounce", 60)) { }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                // The printed types of the Benched Pokémon (not what effects add), so two Kecleon don't keep asking each other.
                foreach (var b in src.Me.Bench)
                {
                    if (b == mon) continue;
                    if (!types.Contains(b.Def.PokemonType)) types.Add(b.Def.PokemonType);
                    if (b.Def.SecondType.HasValue && !types.Contains(b.Def.SecondType.Value)) types.Add(b.Def.SecondType.Value);
                }
            }
        }

        // ================================================================== Trainers

        /// <summary>Bonding Sash (Tool): if the Mega Evolution Pokémon ex it's attached to is Knocked Out by damage from an opponent's attack, search your deck for a card and put it into your hand.</summary>
        private sealed class BondingSash : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var mon = src.Pokemon;
                if (mon == null || !byOpponentsAttack || !mon.Def.IsMegaEx || src.Me.Deck.Count == 0) return;
                src.Log("Bonding Sash: " + src.Engine.PlayerName(src.Player) + " searches their deck for a card.");
                src.SearchToHand(d => true, 1, "Bonding Sash: choose a card to put into your hand", false, 1);
            }
        }

        /// <summary>Korrina's Seriousness: this turn, your Mega Evolution Pokémon ex's attacks do 20 more damage to the opponent's Active Pokémon for each Prize card the opponent has taken.</summary>
        private sealed class KorrinasSeriousness : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int bonus = 20 * ctx.OpponentPrizesTaken;
                if (bonus <= 0)
                {
                    ctx.Log("Korrina's Seriousness: your opponent hasn't taken any Prize cards, so it adds no damage.");
                    return;
                }
                ctx.AddEffect(new ConditionalDamageBonusEffect("Korrina's Seriousness: your Mega Evolution Pokémon ex do " + bonus + " more damage this turn",
                    ctx.Player, ctx.Turn, bonus, (e, attacker, target) => attacker.Def.IsMegaEx));
            }
        }

        /// <summary>Crasher Wake's Fervor: shuffle 1 of your Water Pokémon and all attached cards into your deck.</summary>
        private sealed class CrasherWakesFervor : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Water) && (!ctx.IsActive(m) || ctx.Me.Bench.Count > 0));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var pick = ctx.ChoosePokemon(targets, "Crasher Wake's Fervor: shuffle which of your Water Pokémon and all attached cards into your deck?");
                if (pick != null) ctx.Engine.ShufflePokemonIntoDeck(pick);
            }
        }

        /// <summary>Reporter: draw 2 cards; the opponent reveals their hand; look at the top card of the opponent's deck.</summary>
        private sealed class Reporter : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.Reveal(ctx.Opponent, ctx.Opp.Hand.ToList(), "their hand (Reporter)");
                var top = ctx.Opp.Deck.Take(1).ToList();
                if (top.Count == 0) return;
                ctx.Engine.NoteLookedAt(top, "Top card of your opponent's deck", ctx.Player);
                ctx.Engine.ShowCards(ctx.Player, top, "Top of your opponent's deck", "Reporter: the top card of your opponent's deck", ctx.Source);
            }
        }

        /// <summary>Fight Arena (Stadium): once during each player's turn, they may heal 20 damage from each of their Pokémon without a Rule Box.</summary>
        private sealed class FightArena : CardScript
        {
            private const string Limit = "Fight Arena";

            private static List<PokemonInPlay> Healable(EffectContext ctx) { return ctx.MyPokemon(m => !m.Def.HasRuleBox && m.DamageCounters > 0); }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && Healable(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                foreach (var m in Healable(ctx)) ctx.Heal(m, 20);
                return false;
            }
        }
    }
}
