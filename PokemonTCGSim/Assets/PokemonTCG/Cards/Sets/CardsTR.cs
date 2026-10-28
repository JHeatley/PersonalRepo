using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Rocket (TR, v121): the Dark Pokémon and the rest of the set. 18-31 are the non-holo prints of 1-14 and 72 of 16; Dark
    /// Gyarados is the Celebrations Classic Collection card and Here Comes Team Rocket! Celebrations'. Rainbow Energy (17, 80 its
    /// print) is its own card (v121.1): its text was never errata'd, so it does 10 damage instead of placing a damage counter.
    /// Pokémon Powers stop while their Pokémon is Asleep, Confused or Paralyzed (when the card says so).
    /// Your rulings (v121): Frenzy adds 30 to Dark Primeape's attacks while it's Confused and 3 counters to a failed Confusion flip;
    /// Mirror Shell sends back the damage with the attacker's Weakness and Resistance; Goop Gas Attack and Stare stop only old
    /// Pokémon Powers.
    /// </summary>
    public static class CardsTR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TR-1", new ComboMon(new TeleportBlastMon(), new CardsFST1.NoWeaknessResistanceMon("Mind Shock")));
            r.RegisterByKey("TR-2", new ComboMon(new StareMon(), new PoisonVaporMon()));
            r.RegisterByKey("TR-3", new ComboMon(new HydrocannonMon(), new RocketTackleMon()));
            r.RegisterByKey("TR-4", new ContinuousFireballMon());
            r.RegisterByKey("TR-5", new DragoniteSummonMinions());
            r.RegisterByKey("TR-6", new DugtrioSinkhole());
            r.RegisterByKey("TR-7", new GolbatSneakAttack());
            r.RegisterByKey("TR-9", new BenchManipulationMon());
            r.RegisterByKey("TR-10", new FlingMachamp());
            r.RegisterByKey("TR-11", new ComboMon(new CardsFST1.NoWeaknessResistanceMon("Sonicboom"), new MagneticLinesMon()));
            r.RegisterByKey("TR-12", new SlowbroReelIn());
            r.RegisterByKey("TR-13", new VileplumeHayFever());
            r.RegisterByKey("TR-14", new ComboMon(new MassExplosionMon(), new StunGasMon()));

            r.RegisterByKey("TR-32", new FireballCharmeleon());
            r.RegisterByKey("TR-33", new DragonairEvolutionaryLight());
            r.RegisterByKey("TR-34", new EnergyBombMon());
            r.RegisterByKey("TR-35", new FlareonPlayingWithFire());
            r.RegisterByKey("TR-36", new GloomPollenStench());
            r.RegisterByKey("TR-37", new CardsBS1.DiscardCostMon("Third Eye", null, 1, ThirdEyeEffect, null));
            r.RegisterByKey("TR-38", new ComboMon(new CardsMEG.SandAttackMon("Lightning Flash"), new ThunderAttackMon()));
            r.RegisterByKey("TR-39", new KadabraMatterExchange());
            r.RegisterByKey("TR-40", new MachokeDragOff());
            r.RegisterByKey("TR-41", new MukStickyGoo());
            r.RegisterByKey("TR-42", new PersianFascinate());
            r.RegisterByKey("TR-43", new PrimeapeFrenzy());
            r.RegisterByKey("TR-44", new FlamePillarMon());
            r.RegisterByKey("TR-45", new CardsDRI2.CrunchMon("Whirlpool"));
            r.RegisterByKey("TR-46", new ComboMon(new CoinsMon("Doubleslap", 2), new MirrorShellMon()));
            r.RegisterByKey("TR-47", new RapidEvolutionMon());
            r.RegisterByKey("TR-48", new ComboMon(new CardsBS1.PorygonConversion(), new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("TR-49", new ComboMon(new VanishMon(), new ParalyzeFlipMon("Psyshock")));
            r.RegisterByKey("TR-50", new CharmanderGatherFire());
            r.RegisterByKey("TR-51", new TailsNothingMon("Hyper Fang"));
            r.RegisterByKey("TR-52", new ChooseTargetMon("Dig Under", 10, false));
            r.RegisterByKey("TR-53", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("TR-54", new DrowzeeHypnosis());
            r.RegisterByKey("TR-55", new CardsMEG.SandAttackMon("Sand-attack"));
            r.RegisterByKey("TR-56", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("TR-57", new ComboMon(new ConditionAttackMon("Poison Gas", SpecialCondition.Asleep), new CardsDRX.SqueezeMon("Sticky Hands")));
            r.RegisterByKey("TR-58", new CardsPAR2.FlipConditionMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("TR-60", new CardsASR1.PlusMon("Magnetism", 10, c => c.Me.Bench.Count(m => m.Name == "Magnemite" || m.Name == "Magneton" || m.Name == "Dark Magneton")));
            r.RegisterByKey("TR-61", new ComboMon(new MischiefMon(), new CardsWHT.FlipMoreMon("Anger", 1, 20)));
            r.RegisterByKey("TR-62", new ChooseTargetMon("Coin Hurl", 20, true));
            r.RegisterByKey("TR-63", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("TR-64", new CardsBS1.DiscardCostMon("Ember", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("TR-65", new ComboMon(new DrawAttackMon("Dizziness", 1), new CardsBS1.ExtraWaterMon("Water Gun")));
            r.RegisterByKey("TR-66", new RattataTrickery());
            r.RegisterByKey("TR-67", new AfternoonNapMon());
            r.RegisterByKey("TR-83", new SurpriseThunderMon());

            r.RegisterByKey("TR-16", new RocketsSneakAttack());
            r.RegisterByKey("TR-73", new TheBossesWay());
            r.RegisterByKey("TR-74", new Challenge());
            r.RegisterByKey("TR-75", new Digger());
            r.RegisterByKey("TR-76", new ImposterOaksRevenge());
            r.RegisterByKey("TR-77", new NightlyGarbageRun());
            r.RegisterByKey("TR-78", new GoopGasAttack());
            r.RegisterByKey("TR-79", new SleepTrainer());
            r.RegisterByKey("TR-17", new RainbowEnergyTR());
            r.RegisterByKey("TR-81", new FullHealEnergy());
            r.RegisterByKey("TR-82", new PotionEnergy());
        }

        // ================================================================== Shared

        private static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        private static bool IsFireCard(EffectContext ctx, PokemonInPlay mon, CardInstance card)
        {
            return card.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, mon, card, EnergyType.Fire);
        }

        private static List<CardInstance> FireCards(EffectContext ctx, PokemonInPlay mon)
        {
            return mon == null ? new List<CardInstance>() : mon.Energy.Where(c => IsFireCard(ctx, mon, c)).ToList();
        }

        /// <summary>Discards n of the Fire Energy cards attached to the Pokémon (the player chooses which). Returns how many.</summary>
        private static int DiscardFire(EffectContext ctx, PokemonInPlay mon, int n, string what)
        {
            var fire = FireCards(ctx, mon);
            if (n <= 0 || fire.Count == 0) return 0;
            n = Math.Min(n, fire.Count);
            var picks = fire.Count == n ? fire : ctx.ChooseCards(fire, n, n, what + ": discard " + n + " Fire Energy card" + (n == 1 ? "" : "s"));
            int done = 0;
            foreach (var card in picks)
                if (ctx.Engine.DiscardEnergy(mon, card)) done++;
            return done;
        }

        /// <summary>
        /// Damage a power or an effect does "applying Weakness and Resistance" outside an attack (Sneak Attack, Mirror Shell): the
        /// source Pokémon's types against the target's Weakness (x2, or its +N) and Resistance.
        /// </summary>
        internal static int WithWeaknessAndResistance(GameEngine engine, PokemonInPlay source, PokemonInPlay target, int damage)
        {
            if (source == null || target == null || damage <= 0) return damage;
            var types = engine.TypesOf(source);
            var weakness = engine.WeaknessOf(target);
            if (weakness.HasValue && types.Contains(weakness.Value))
                damage = target.Def.WeaknessAmount > 0 ? damage + target.Def.WeaknessAmount : damage * 2;
            var resistance = engine.ResistanceOf(target);
            if (resistance.HasValue && types.Contains(resistance.Value)) damage = Math.Max(0, damage - target.Def.ResistanceAmount);
            return damage;
        }

        // ================================================================== Pokémon Powers

        /// <summary>Summon Minions: when you play Dark Dragonite from your hand, search your deck for up to 2 Basic Pokémon and put them onto your Bench. Giant Tail: tails, nothing.</summary>
        private sealed class DragoniteSummonMinions : Mon
        {
            private const string Summon = "Summon Minions";
            private static readonly Mon Tail = new TailsNothingMon("Giant Tail");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, Summon, ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0, Summon + ": search your deck for up to 2 Basic Pokémon and put them onto your Bench?")) return;
                ctx.SearchToBench(d => true, 2, Summon + ": choose up to 2 Basic Pokémon to put onto your Bench");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tail.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Sinkhole: whenever the opponent's Active Pokémon retreats, they flip a coin; tails, 20 damage to that Pokémon (no W/R).
        /// Knock Down: the opponent flips a coin; tails, 20 more damage.
        /// </summary>
        private sealed class DugtrioSinkhole : Mon
        {
            public override void OnPokemonRetreated(EffectContext src, PokemonInPlay retreated)
            {
                var me = src.Pokemon;
                if (me == null || retreated == null || retreated.Owner == src.Player || !PowerWorks(me) || !src.InPlay(retreated)) return;
                Announce(src, "Sinkhole");
                if (src.Engine.FlipCoin(retreated.Owner, "Sinkhole")) return;
                src.Engine.PlaceDamageCounters(retreated, 2, "Sinkhole");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Down") return false;
                bool heads = ctx.Engine.FlipCoin(ctx.Opponent, "Knock Down");
                ctx.DamageDefending(atk.Damage + (heads ? 0 : 20));
                return true;
            }
        }

        /// <summary>
        /// Sneak Attack: when you play Dark Golbat from your hand, you may do 10 damage to 1 of the opponent's Pokémon (Weakness and
        /// Resistance apply). Flitter: 20 damage to 1 of the opponent's Pokémon, no W/R.
        /// </summary>
        private sealed class GolbatSneakAttack : Mon
        {
            private const string Sneak = "Sneak Attack";
            private static readonly Mon Flitter = new ChooseTargetMon("Flitter", 20, false);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var targets = ctx.OpponentPokemon();
                if (!TriggerOnce(ctx, Sneak, targets.Count > 0, Sneak + ": do 10 damage to 1 of your opponent's Pokémon?")) return;
                targets = ctx.OpponentPokemon();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, Sneak + ": choose 1 of your opponent's Pokémon");
                if (target == null) return;
                int damage = WithWeaknessAndResistance(ctx.Engine, ctx.Pokemon, target, 10);
                ctx.Log(Sneak + " does " + damage + " damage to " + target.Name + ".");
                if (damage >= 10) ctx.Engine.PlaceDamageCounters(target, damage / 10, Sneak);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flitter.HandleAttack(ctx, atk); }
        }

        /// <summary>Reel In: when you play Dark Slowbro from your hand, put up to 3 Basic Pokémon and/or Evolution cards from your discard pile into your hand. Fickle Attack: tails, nothing.</summary>
        private sealed class SlowbroReelIn : Mon
        {
            private const string Reel = "Reel In";
            private static readonly Mon Fickle = new TailsNothingMon("Fickle Attack");

            private static List<CardInstance> Cards(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon || c.Def.IsEvolution).ToList();
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, Reel, Cards(ctx).Count > 0, Reel + ": put up to 3 Basic Pokémon and/or Evolution cards from your discard pile into your hand?")) return;
                var cards = Cards(ctx);
                if (cards.Count == 0) return;
                ctx.DiscardToHand(ctx.ChooseCards(cards, 1, Math.Min(3, cards.Count), Reel + ": choose up to 3 Basic Pokémon and/or Evolution cards"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fickle.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Hay Fever: no Trainer cards can be played (by either player) while Dark Vileplume isn't Asleep, Confused or Paralyzed.
        /// Petal Whirlwind: 3 coins, 30 for each heads; 2 or more heads, Dark Vileplume is now Confused.
        /// </summary>
        private sealed class VileplumeHayFever : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && card != null && card.Def.IsTrainer && PowerWorks(src.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Petal Whirlwind") return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Evolutionary Light: once during your turn, search your deck for an Evolution card, show it and put it into your hand. Tail Strike: heads, 20 more.</summary>
        private sealed class DragonairEvolutionaryLight : Mon
        {
            private const string Light = "Evolutionary Light";
            private static readonly Mon Strike = new CardsWHT.FlipMoreMon("Tail Strike", 1, 20);

            protected override string Activated { get { return Light; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsEvolution, 1, Light + ": choose an Evolution card to put into your hand");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Strike.HandleAttack(ctx, atk); }
        }

        /// <summary>Pollen Stench: once during your turn, flip a coin; heads, the opponent's Active Pokémon is Confused; tails, yours is. Poisonpowder: Poisoned.</summary>
        private sealed class GloomPollenStench : Mon
        {
            private const string Stench = "Pollen Stench";
            private static readonly Mon Powder = new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned);

            protected override string Activated { get { return Stench; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Active != null && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                bool heads = ctx.Flip();
                var target = heads ? ctx.Opp.Active : ctx.Me.Active;
                if (target != null) ctx.ApplyCondition(target, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Powder.HandleAttack(ctx, atk); }
        }

        /// <summary>Long-Distance Hypnosis: once during your turn, flip a coin; heads, the opponent's Active Pokémon is Asleep; tails, yours is. Nightmare: Asleep.</summary>
        private sealed class DrowzeeHypnosis : Mon
        {
            private const string Hypnosis = "Long-Distance Hypnosis";
            private static readonly Mon Nightmare = new ConditionAttackMon("Nightmare", SpecialCondition.Asleep);

            protected override string Activated { get { return Hypnosis; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Active != null && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                bool heads = ctx.Flip();
                var target = heads ? ctx.Opp.Active : ctx.Me.Active;
                if (target != null) ctx.ApplyCondition(target, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Nightmare.HandleAttack(ctx, atk); }
        }

        /// <summary>Matter Exchange: once during your turn, discard a card from your hand to draw a card. Mind Shock: no W/R.</summary>
        private sealed class KadabraMatterExchange : Mon
        {
            private const string Exchange = "Matter Exchange";
            private static readonly Mon Shock = new CardsFST1.NoWeaknessResistanceMon("Mind Shock");

            protected override string Activated { get { return Exchange; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, Exchange + ": discard a card from your hand to draw a card")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shock.HandleAttack(ctx, atk); }
        }

        /// <summary>Sticky Goo: while Dark Muk is your Active Pokémon, the opponent pays [C][C] more to retreat their Active Pokémon. Sludge Punch: Poisoned.</summary>
        private sealed class MukStickyGoo : Mon
        {
            private static readonly Mon Punch = new ConditionAttackMon("Sludge Punch", SpecialCondition.Poisoned);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || !src.State.IsActive(me) || !src.State.IsActive(mon) || !PowerWorks(me)) return cost;
                return cost + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Frenzy: while Dark Primeape is Confused, its attacks do 30 more damage, and a failed Confusion flip puts 3 more damage
        /// counters on it (your ruling, v121: "even to itself"). Frenzied Attack: Dark Primeape is now Confused.
        /// </summary>
        private sealed class PrimeapeFrenzy : Mon
        {
            private static readonly Mon Frenzied = new CardsRCL2.SelfConditionMon("Frenzied Attack", SpecialCondition.Confused);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || damage <= 0 || !me.Has(SpecialCondition.Confused)) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 30;
            }

            public override int ModifyConfusionCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return mon == src.Pokemon && counters > 0 ? counters + 3 : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Frenzied.HandleAttack(ctx, atk); }
        }

        /// <summary>Gather Fire: once during your turn, move 1 Fire Energy card from 1 of your other Pokémon to Charmander.</summary>
        private sealed class CharmanderGatherFire : Mon
        {
            private const string Gather = "Gather Fire";

            protected override string Activated { get { return Gather; } }

            private static List<PokemonInPlay> FireHolders(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && FireCards(ctx, m).Count > 0);
            }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && FireHolders(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var holders = FireHolders(ctx);
                if (holders.Count == 0) return;
                var from = ctx.ChoosePokemon(holders, Gather + ": move a Fire Energy card from which of your Pokémon?");
                if (from == null) return;
                var fire = FireCards(ctx, from);
                if (fire.Count == 0) return;
                var card = ctx.ChooseCards(fire, 1, 1, Gather + ": move which Fire Energy card?")[0];
                ctx.Engine.MoveEnergy(from, ctx.Pokemon, card);
            }
        }

        /// <summary>Trickery: once during your turn, switch 1 of your Prize cards with the top card of your deck. Quick Attack: heads, 10 more.</summary>
        private sealed class RattataTrickery : Mon
        {
            private const string Trickery = "Trickery";
            private static readonly Mon Quick = new CardsWHT.FlipMoreMon("Quick Attack", 1, 10);

            protected override string Activated { get { return Trickery; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Prizes.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var prizes = ctx.Me.Prizes;
                if (prizes.Count == 0 || ctx.Me.Deck.Count == 0) return;
                // Face-down Prize cards are picked blind; face-up ones (Here Comes Team Rocket!) by name.
                var labels = prizes.Select((c, i) => ctx.Me.FaceUpPrizeIds.Contains(c.Id) ? c.Def.Name + " (Prize card " + (i + 1) + ")" : "Prize card " + (i + 1)).ToArray();
                int pick = prizes.Count == 1 ? 0 : ctx.ChooseOption(Trickery + ": switch which Prize card with the top card of your deck?", labels);
                if (pick < 0 || pick >= prizes.Count) pick = 0;
                var prize = prizes[pick];
                var top = ctx.Me.Deck[0];
                prizes[pick] = top;
                ctx.Me.Deck[0] = prize;
                ctx.Me.FaceUpPrizeIds.Remove(prize.Id);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched a Prize card with the top card of " + ctx.Engine.Their(ctx.Player) + " deck (Trickery).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Quick.HandleAttack(ctx, atk); }
        }

        // ================================================================== Attacks

        /// <summary>Teleport Blast: after the damage, you may switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class TeleportBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teleport Blast") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Teleport Blast: switch " + ctx.Pokemon.Name + " with 1 of your Benched Pokémon?"))
                    ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Pokémon" and do N damage to it, no W/R (Flitter, Dig Under); flipFirst: heads only (Coin Hurl).</summary>
        private sealed class ChooseTargetMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly bool flip;

            public ChooseTargetMon(string attack, int amount, bool flip)
            {
                this.attack = attack;
                this.amount = amount;
                this.flip = flip;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, attack + ": choose 1 of your opponent's Pokémon");
                if (target == null) return true;
                if (flip && !ctx.Flip()) return Nothing(ctx);
                ctx.DamageWith(target, amount, false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Stare: 10 damage to 1 of the opponent's Pokémon (no W/R); its Pokémon Power stops working until the end of the opponent's
        /// next turn (only Pokémon Powers: your ruling, v121).
        /// </summary>
        private sealed class StareMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stare") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Stare: choose 1 of your opponent's Pokémon");
                if (target == null) return true;
                ctx.DamageWith(target, 10, false, false, false);
                if (ctx.InPlay(target) && target.Def.Abilities.Any(a => a.Kind == AbilityDef.PokemonPower))
                    ctx.AddEffect(new StareEffect(target.Name + "'s Pokémon Power stops working until the end of your opponent's next turn (Stare)", ctx.Player, target.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class StareEffect : ContinuousEffect
        {
            public StareEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool StopsPokemonPower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Poison Vapor: the Defending Pokémon is Poisoned; 10 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class PoisonVaporMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Vapor") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                foreach (var mon in ctx.Opp.Bench.ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Hydrocannon: 20 more for each Water Energy not used for the cost (up to 40 more).</summary>
        private sealed class HydrocannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydrocannon") return false;
                ctx.DamageDefending(atk.Damage + 20 * CardsBS1.ExtraWaterMon.Extra(ctx, atk));
                return true;
            }
        }

        /// <summary>Rocket Tackle: 10 damage to itself; heads, prevent all damage done to it during the opponent's next turn.</summary>
        private sealed class RocketTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rocket Tackle") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.DamageSelf(10);
                if (ctx.InPlay(me) && ctx.Flip())
                    ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage from attacks during the opponent's next turn (Rocket Tackle)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Continuous Fireball: a coin for each Fire Energy card attached, 50 for each heads; discard that many Fire Energy cards.</summary>
        private sealed class ContinuousFireballMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Continuous Fireball") return false;
                var me = ctx.Pokemon;
                int coins = FireCards(ctx, me).Count;
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                if (ctx.InPlay(me)) DiscardFire(ctx, me, heads, "Continuous Fireball");
                return true;
            }
        }

        /// <summary>Bench Manipulation: the opponent flips a coin for each of their Benched Pokémon; 20 for each tails, no W/R.</summary>
        private sealed class BenchManipulationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bench Manipulation") return false;
                int n = ctx.Opp.Bench.Count;
                if (n == 0) return Nothing(ctx);
                int tails = n - ctx.Engine.FlipCoins(ctx.Opponent, n, "Bench Manipulation");
                if (tails == 0) return Nothing(ctx);
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage * tails, false, false, false);
                return true;
            }
        }

        /// <summary>Fling: the opponent shuffles their Active Pokémon and all cards attached to it into their deck. Not without an opposing Bench.</summary>
        private sealed class FlingMachamp : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Fling" || ctx.Opp.Bench.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fling") return false;
                var target = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (target == null || !ctx.InPlay(target) || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(target)) return true;
                ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Magnetic Lines: move 1 basic Energy card from the Defending Pokémon to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class MagneticLinesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Lines") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var basic = defending.Energy.Where(c => c.Def.IsEnergy && c.Def.IsBasicEnergy).ToList();
                if (basic.Count == 0) return true;
                var card = ctx.ChooseCards(basic, 1, 1, "Magnetic Lines: move which basic Energy card?")[0];
                var bench = ctx.Opp.Bench.ToList();
                var to = ctx.ChoosePokemon(bench, "Magnetic Lines: attach it to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>
        /// Mass Explosion: 20 for each Koffing, Weezing and Dark Weezing in play (W/R apply), then 20 to each of them (even your own),
        /// no W/R.
        /// </summary>
        private sealed class MassExplosionMon : Mon
        {
            private static bool Family(PokemonInPlay m) { return m.Name == "Koffing" || m.Name == "Weezing" || m.Name == "Dark Weezing"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mass Explosion") return false;
                var family = ctx.MyPokemon(Family).Concat(ctx.OpponentPokemon(Family)).ToList();
                ctx.DamageDefending(atk.Damage * family.Count);
                foreach (var mon in family)
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Stun Gas: flip a coin; heads, Poisoned; tails, Paralyzed.</summary>
        private sealed class StunGasMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stun Gas") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Poisoned : SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Fireball: only with a Fire Energy card attached; heads, 70 and discard 1 of them; tails, nothing at all.</summary>
        private sealed class FireballCharmeleon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Fireball" || FireCards(ctx, ctx.Pokemon).Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fireball") return false;
                if (FireCards(ctx, ctx.Pokemon).Count == 0 || !ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) DiscardFire(ctx, ctx.Pokemon, 1, "Fireball");
                return true;
            }
        }

        /// <summary>
        /// Rage: 10 more for each damage counter on Dark Flareon. Playing with Fire: only with a Fire Energy card attached; heads,
        /// discard 1 of them for 20 more damage.
        /// </summary>
        private sealed class FlareonPlayingWithFire : Mon
        {
            private static readonly Mon Rage = new CardsASR1.PlusMon("Rage", 10, c => c.Pokemon == null ? 0 : c.Pokemon.DamageCounters);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Playing with Fire" || FireCards(ctx, ctx.Pokemon).Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Playing with Fire") return Rage.HandleAttack(ctx, atk);
                bool more = ctx.Flip() && DiscardFire(ctx, ctx.Pokemon, 1, "Playing with Fire") > 0;
                ctx.DamageDefending(atk.Damage + (more ? 20 : 0));
                return true;
            }
        }

        /// <summary>Energy Bomb: move all Energy cards from Dark Electrode to your Benched Pokémon in any way; with no Bench, discard them.</summary>
        private sealed class EnergyBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Bomb") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var energy = me.Energy.ToList();
                if (energy.Count == 0) return true;
                if (ctx.Me.Bench.Count == 0)
                {
                    foreach (var card in energy) ctx.Engine.DiscardEnergy(me, card);
                    return true;
                }
                foreach (var card in energy)
                {
                    var bench = ctx.Me.Bench.ToList();
                    if (bench.Count == 0 || !me.Energy.Contains(card)) continue;
                    var to = ctx.ChoosePokemon(bench, "Energy Bomb: attach " + card.Def.Name + " to which of your Benched Pokémon?");
                    if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                }
                return true;
            }
        }

        /// <summary>Third Eye (discard 1 Energy card to use it): draw up to 3 cards.</summary>
        private static void ThirdEyeEffect(EffectContext ctx, AttackDef atk)
        {
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            int max = Math.Min(3, ctx.Me.Deck.Count);
            if (max <= 0) return;
            int n = ctx.AskNumber(0, max, "Third Eye: draw how many cards (up to " + max + ")?");
            if (n > 0) ctx.Draw(n);
        }

        /// <summary>Thunder Attack: heads, Paralyzed; tails, this Pokémon does 10 damage to itself.</summary>
        private sealed class ThunderAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Attack") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                {
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                }
                else if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>
        /// Drag Off: before the damage, switch 1 of the opponent's Benched Pokémon (your choice) with the Defending Pokémon and damage
        /// the new one; not without an opposing Bench. Knock Back: the opponent switches after the damage.
        /// </summary>
        private sealed class MachokeDragOff : Mon
        {
            private static readonly Mon Drag = new CardsWHT.DragOffMon("Drag Off", 20);
            private static readonly Mon Knock = new CardsASC.PushDownMon("Knock Back");

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Drag Off" || ctx.Opp.Bench.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                return Drag.HandleAttack(ctx, atk) || Knock.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Fascinate: heads, switch 1 of the opponent's Benched Pokémon (your choice) with the Defending Pokémon; not without an opposing Bench. Poison Claws: heads, Poisoned.</summary>
        private sealed class PersianFascinate : Mon
        {
            private static readonly Mon Claws = new CardsPAR2.FlipConditionMon("Poison Claws", SpecialCondition.Poisoned);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Fascinate" || ctx.Opp.Bench.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fascinate") return Claws.HandleAttack(ctx, atk);
                if (!ctx.Flip()) return Nothing(ctx);
                if (ctx.Opp.Bench.Count > 0) ctx.GustOpponent("Fascinate: choose 1 of your opponent's Benched Pokémon to switch in");
                return true;
            }
        }

        /// <summary>Flame Pillar: you may discard a Fire Energy card; if you do, 10 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class FlamePillarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Pillar") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || FireCards(ctx, me).Count == 0) return true;
                if (!ctx.AskYesNo("Flame Pillar: discard a Fire Energy card" + (ctx.Opp.Bench.Count > 0 ? " to do 10 damage to 1 of your opponent's Benched Pokémon?" : "? (Your opponent has no Benched Pokémon.)"))) return true;
                if (DiscardFire(ctx, me, 1, "Flame Pillar") > 0 && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>
        /// Mirror Shell: if an attack damages Dark Wartortle during the opponent's next turn (even if it's Knocked Out), the attacker
        /// takes the same damage, with its Weakness and Resistance applied (your ruling, v121).
        /// </summary>
        private sealed class MirrorShellMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Shell") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new MirrorShellEffect(me.Name + " sends back the damage attacks do to it during the opponent's next turn (Mirror Shell)", ctx.Player, me.Uid, ctx.Turn + 1) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class MirrorShellEffect : ContinuousEffect
        {
            public MirrorShellEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                var me = info.Target;
                var attacker = info.Attacker;
                if (me == null || me.Uid != TargetUid || damage <= 0 || attacker == null || attacker.Owner == Owner) return;
                if (engine.State.FindPokemon(attacker.Uid) != attacker) return;
                int back = WithWeaknessAndResistance(engine, me, attacker, damage);
                engine.Emit(GameEventType.Info, Owner, "Mirror Shell: " + me.Name + " does " + back + " damage to " + attacker.Name + ".", me.Top.Id, me.Uid);
                if (back >= 10) engine.PlaceDamageCounters(attacker, back / 10, "Mirror Shell");
            }
        }

        /// <summary>Rapid Evolution: search your deck for a Gyarados or Dark Gyarados and evolve this Pokémon into it.</summary>
        private sealed class RapidEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                if (CardsEVS1.EvolveBlocked(ctx, me))
                {
                    ctx.Log(me.Name + " can't evolve right now (Rapid Evolution).");
                    return true;
                }
                var found = ctx.SearchMyDeck(d => (d.Name == "Gyarados" || d.Name == "Dark Gyarados") && CardsEVS1.EvolvesOnto(ctx, me, d), 1, "Rapid Evolution: choose a Gyarados or Dark Gyarados", false);
                if (found.Count > 0) ctx.Engine.EvolvePokemon(me, found[0]);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Vanish: shuffle this Pokémon into your deck and discard all cards attached to it.</summary>
        private sealed class VanishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vanish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var pokemonCards = me.Stack.ToList();
                var attached = me.Energy.Concat(me.Tools).ToList();
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.Deck.AddRange(pokemonCards);
                ctx.ShuffleMyDeck();
                ctx.Engine.DiscardFromPlay(attached);
                ctx.Log(me.Name + " was shuffled into " + ctx.Engine.Their(ctx.Player) + " deck" + (attached.Count > 0 ? " and its attached cards were discarded" : "") + " (Vanish).");
                return true;
            }
        }

        /// <summary>Mischief: shuffle your opponent's deck.</summary>
        private sealed class MischiefMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mischief") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + "'s deck was shuffled (Mischief).");
                return true;
            }
        }

        /// <summary>Afternoon Nap: search your deck for a Psychic Energy card and attach it to this Pokémon.</summary>
        private sealed class AfternoonNapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Afternoon Nap") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsEnergy && d.IsBasicEnergy && d.EnergyProvides == EnergyType.Psychic, 1, "Afternoon Nap: choose a Psychic Energy card to attach");
                if (found.Count > 0) ctx.Engine.AttachEnergyCard(found[0], me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Surprise Thunder: heads, flip again: heads, 20 to each of the opponent's Benched Pokémon; tails, 10 to each.</summary>
        private sealed class SurpriseThunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise Thunder") return false;
                bool first = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!first) return true;
                int amount = ctx.Flip() ? 20 : 10;
                foreach (var mon in ctx.Opp.Bench.ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Rocket's Sneak Attack: look at the opponent's hand; choose a Trainer card there and they shuffle it into their deck.</summary>
        private sealed class RocketsSneakAttack : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return;
                ctx.Engine.Reveal(ctx.Opponent, hand, "hand (Rocket's Sneak Attack)");
                var trainers = hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Rocket's Sneak Attack: your opponent shuffles which Trainer card into their deck?")[0];
                if (!ctx.Opp.Hand.Remove(pick)) return;
                ctx.Opp.Deck.Add(pick);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + pick.Def.Name + " into " + ctx.Engine.Their(ctx.Opponent) + " deck (Rocket's Sneak Attack).");
            }
        }

        private static readonly Regex DarkWord = new Regex(@"\bDark\b");

        /// <summary>The Boss's Way: search your deck for an Evolution card with "Dark" in its name and put it into your hand.</summary>
        private sealed class TheBossesWay : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsEvolution && DarkWord.IsMatch(d.Name), 1, "The Boss's Way: choose an Evolution card with \"Dark\" in its name");
            }
        }

        /// <summary>
        /// Challenge!: the opponent accepts or declines. Declined (or both Benches full): draw 2 cards. Accepted: each player puts any
        /// number of Basic Pokémon from their deck onto their Bench, then both decks are shuffled.
        /// </summary>
        private sealed class Challenge : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var opp = new EffectContext(ctx.Engine, ctx.Opponent, ctx.Card, null);
                bool bothFull = ctx.BenchSpace == 0 && opp.BenchSpace == 0;
                bool accepted = !bothFull && ctx.Engine.AskYesNo(ctx.Opponent, "Challenge!: do you accept? (If you decline, your opponent draws 2 cards. If you accept, each player puts any number of Basic Pokémon from their deck onto their Bench.)", ctx.Card.Def);
                if (!accepted)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + (bothFull ? " can't take the challenge (both Benches are full)." : " declined the challenge."));
                    ctx.Draw(2);
                    return;
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " accepted the challenge.");
                if (ctx.BenchSpace > 0) ctx.SearchToBench(d => true, ctx.BenchSpace, "Challenge!: choose any number of Basic Pokémon to put onto your Bench");
                if (opp.BenchSpace > 0) opp.SearchToBench(d => true, opp.BenchSpace, "Challenge!: choose any number of Basic Pokémon to put onto your Bench");
            }
        }

        /// <summary>Digger: you flip a coin; tails, 10 damage to your Active Pokémon; heads, your opponent flips, and so on until someone gets tails.</summary>
        private sealed class Digger : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int flipper = ctx.Player;
                for (int guard = 0; guard < 200; guard++)
                {
                    if (ctx.Engine.FlipCoin(flipper, "Digger"))
                    {
                        flipper = 1 - flipper;
                        continue;
                    }
                    var active = ctx.State.Players[flipper].Active;
                    if (active != null)
                    {
                        ctx.Log("Digger does 10 damage to " + active.Name + ".");
                        ctx.Engine.PlaceDamageCounters(active, 1, "Digger");
                    }
                    return;
                }
            }
        }

        /// <summary>Imposter Oak's Revenge: discard a card from your hand to play it; the opponent shuffles their hand into their deck and draws 4 cards.</summary>
        private sealed class ImposterOaksRevenge : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var pick = ctx.ChooseCards(others, 1, 1, "Imposter Oak's Revenge: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
            }
        }

        /// <summary>Nightly Garbage Run: shuffle up to 3 Basic Pokémon, Evolution cards and/or basic Energy cards from your discard pile into your deck.</summary>
        private sealed class NightlyGarbageRun : CardScript
        {
            private static List<CardInstance> Cards(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c != ctx.Card && (c.Def.IsBasicPokemon || c.Def.IsEvolution || (c.Def.IsEnergy && c.Def.IsBasicEnergy))).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Cards(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Cards(ctx);
                if (cards.Count == 0) return;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(3, cards.Count), "Nightly Garbage Run: shuffle up to 3 Basic Pokémon, Evolution cards and/or basic Energy cards into your deck");
                if (picks.Count == 0) return;
                ctx.Engine.Reveal(ctx.Player, picks, "cards shuffled into the deck (Nightly Garbage Run)");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Goop Gas Attack: all Pokémon Powers stop working until the end of the opponent's next turn (only Pokémon Powers: your ruling, v121).</summary>
        private sealed class GoopGasAttack : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new GoopGasEffect("All Pokémon Powers stop working until the end of your opponent's next turn (Goop Gas Attack)", ctx.Player, ctx.Turn + 1));
                ctx.Log("All Pokémon Powers stop working until the end of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s next turn (Goop Gas Attack).");
            }
        }

        internal sealed class GoopGasEffect : ContinuousEffect
        {
            public GoopGasEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool StopsPokemonPower(GameEngine engine, PokemonInPlay mon, string powerName) { return true; }
        }

        /// <summary>Sleep!: flip a coin; heads, the Defending Pokémon is now Asleep.</summary>
        private sealed class SleepTrainer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target != null && ctx.Flip()) ctx.ApplyCondition(target, SpecialCondition.Asleep);
            }
        }

        // ================================================================== Energy

        /// <summary>
        /// Team Rocket's Rainbow Energy (v121.1): every type of basic Energy, 1 at a time. Attached from the hand, it does 10 damage
        /// to that Pokémon (no Weakness or Resistance). Its text was never errata'd: it's damage, not a damage counter, so effects
        /// that reduce or prevent damage apply (the later Rainbow Energy places 1 damage counter instead).
        /// </summary>
        private sealed class RainbowEnergyTR : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null) return;
                ctx.Engine.DealDamage(new DamageInfo { Target = mon, ApplyWeakness = false, ApplyResistance = false }, 10);
            }
        }

        /// <summary>Full Heal Energy: played from the hand, the Pokémon recovers from Asleep, Confused, Paralyzed and Poisoned. Provides [C].</summary>
        private sealed class FullHealEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null) return;
                foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                    if (mon.Has(c)) ctx.Engine.RemoveCondition(mon, c);
            }
        }

        /// <summary>Potion Energy: played from the hand, remove 1 damage counter from the Pokémon. Provides [C].</summary>
        private sealed class PotionEnergy : CardScript
        {
            public override void OnAttachedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon != null && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 10);
            }
        }
    }
}
