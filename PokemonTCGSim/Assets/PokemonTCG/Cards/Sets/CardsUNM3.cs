using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unified Minds (UNM) part 3: Darkness, Metal, Fairy, Dragon and Colorless (UNM 125-185). Oranguru's Sage's Riddle is a
    /// guessing game: the opponent guesses the hidden Pokémon's type from the 11 types.
    /// </summary>
    public static class CardsUNM3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UNM-125", new UmbreonDarkraiGX());
            r.RegisterByKey("UNM-126", new SableyeTyranitarGX());
            r.RegisterByKey("UNM-127", new DrawAttackMon("Collect", 2));
            r.RegisterByKey("UNM-128", new CardsDAA1.RandomHandToDeckMon("Astonish", 1));
            r.RegisterByKey("UNM-130", new CardsMEG.SnipeMon("Sharpshooting", 30));
            r.RegisterByKey("UNM-132", new WeavileGX());
            r.RegisterByKey("UNM-133", new MirrorGemMon());
            r.RegisterByKey("UNM-134", new CrossPoisonMon());
            r.RegisterByKey("UNM-135", new CleaningUpMon());
            r.RegisterByKey("UNM-136", new CardsEVS1.DefenderCantAttackMon("Shadow Scratch", IsBasicPokemonCard));
            r.RegisterByKey("UNM-137", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("UNM-138", new CardsASR1.PlusMon("Turf Raid", 20, c => c.Me.Prizes.Count));
            r.RegisterByKey("UNM-139", new ComboMon(new CardsFST2.ConditionBonusMon("Blow Through", 20, StadiumInPlay), new ShadowImpactMon()));
            r.RegisterByKey("UNM-140", new ComboMon(new CardsASR1.PlusMon("Evil Admonition", 20, c => c.OpponentPokemon(m => c.Engine.HasAbility(m)).Count),
                new CardsFST1.NoWeaknessResistanceMon("Mind Shock")));
            r.RegisterByKey("UNM-141", new MawileGX());
            r.RegisterByKey("UNM-142", new CardsNXD.BaredFangsMon("Discerning Spear"));
            r.RegisterByKey("UNM-143", new DrawAttackMon("Dust Gathering", 1));
            r.RegisterByKey("UNM-144", new WhimsicottUNM());
            r.RegisterByKey("UNM-145", new CardsDAA1.MayDrawUntilMon("Return", 6));
            r.RegisterByKey("UNM-146", new GarchompGiratinaGX());
            r.RegisterByKey("UNM-147", new DodgeMon("Agility", false));
            r.RegisterByKey("UNM-148", new CardsSVP.FloatMon(EnergyType.Water, new FlipBonusMon("Jump On", 30)));
            r.RegisterByKey("UNM-149", new TwisterMon());
            r.RegisterByKey("UNM-150", new CardsDRI2.CrunchMon("Destructive Whirlpool"));
            r.RegisterByKey("UNM-151", new DragoniteUNM());
            r.RegisterByKey("UNM-152", new DragoniteGX());
            r.RegisterByKey("UNM-153", new EnergyArrowMon());
            r.RegisterByKey("UNM-154", new AxewUNM());
            r.RegisterByKey("UNM-155", new CardsASC.SelfGuardMon("Guard Press", 30));
            r.RegisterByKey("UNM-156", new HaxorusUNM());
            r.RegisterByKey("UNM-157", new ComboMon(new CardsWHT.DragOffMon("Drag Off", 30), new CoinsMon("Dragon Tail", 2)));
            r.RegisterByKey("UNM-158", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("UNM-159", new ComboMon(new CardsCEC3.SpreadAmountMon("Boomburst", 20), new CardsBRS1.MillSelfMon("Dragon Pulse", 1)));
            r.RegisterByKey("UNM-160", new NaganadelGX());
            r.RegisterByKey("UNM-161", new HeavyDrawMon());
            r.RegisterByKey("UNM-162", new LicksGoCrazyMon());
            r.RegisterByKey("UNM-163", new ComboMon(new DrawAttackMon("Double Draw", 2), new CardsASR1.TimesMon("Tag Impact", c => c.MyPokemon(m => m.Def.IsTagTeam).Count)));
            r.RegisterByKey("UNM-164", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new RecoilMon("Berserker Tackle", 10)));
            r.RegisterByKey("UNM-165", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("UNM-166", new CardsSP1.BlindsideSPMon("Blindside", 60));
            r.RegisterByKey("UNM-167", new LazyHowlMon());
            r.RegisterByKey("UNM-168", new LockAfterAttackMon("Boundless Power", false));
            r.RegisterByKey("UNM-169", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("UNM-170", new CardsASC.CounterattackMon("Counterattack", 4, new DynamicSwingMon()));
            r.RegisterByKey("UNM-171", new TailsNothingMon("Hyper Fang"));
            r.RegisterByKey("UNM-172", new BibarelUNM());
            r.RegisterByKey("UNM-173", new MunchlaxUNM());
            r.RegisterByKey("UNM-175", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("UNM-176", new DownburstMon());
            r.RegisterByKey("UNM-177", new AudinoUNM());
            r.RegisterByKey("UNM-178", new ThunderousTornadoMon());
            r.RegisterByKey("UNM-181", new AlertHeadbuttMon());
            r.RegisterByKey("UNM-182", new SagesRiddleMon());
            r.RegisterByKey("UNM-183", new FlipBonusMon("Quick Blow", 30));
            r.RegisterByKey("UNM-184", new ComboMon(new CardsGRI3.SettleTheScoreMon("Avenging Heart", 50), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("UNM-185", new KomalaUNM());
        }

        // ================================================================== Shared helpers

        internal static bool IsBasicPokemonCard(CardDef d) { return d.IsBasicPokemon; }
        private static bool StadiumInPlay(EffectContext ctx) { return ctx.State.Stadium != null; }

        /// <summary>The 11 types a Pokémon can be, for guessing games (Sage's Riddle).</summary>
        internal static readonly EnergyType[] AllTypes =
        {
            EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting,
            EnergyType.Darkness, EnergyType.Metal, EnergyType.Fairy, EnergyType.Dragon, EnergyType.Colorless
        };

        // ================================================================== Darkness

        /// <summary>
        /// Black Lance: also 60 damage to 1 of the opponent's Benched Pokémon-GX or Pokémon-EX. Dark Moon-GX: the opponent can't
        /// play Trainer cards during their next turn; with 5 extra Darkness Energy, their Active Pokémon is Knocked Out.
        /// </summary>
        private sealed class UmbreonDarkraiGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Black Lance")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Opp.Bench.Any(m => m.Def.IsGXorEX)) ctx.DamageOneBenched(60, m => m.Def.IsGXorEX);
                    return true;
                }
                if (atk.Name == "Dark Moon-GX")
                {
                    var d = EnergyType.Darkness;
                    bool extra = CardsCEC1.ExtraTyped(ctx, atk, d, d, d, d, d);
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    ctx.AddEffect(new CardsCEC1.TrainerLockCECEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (Dark Moon-GX)",
                        ctx.Player, ctx.Opponent, ctx.Turn + 1));
                    var target = ctx.Defending;
                    if (extra && target != null && ctx.CanAffect(target)) ctx.Engine.KnockOut(target, "Dark Moon-GX");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Greedy Crush: if this attack's damage Knocks Out the opponent's Pokémon-GX or Pokémon-EX, take 1 more Prize card.
        /// Gigafall-GX: with 5 extra Energy, discard the top 15 cards of the opponent's deck.
        /// </summary>
        private sealed class SableyeTyranitarGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Greedy Crush")
                {
                    int owner = ctx.Player;
                    ctx.AddEffect(new ExtraPrizeEffect("Greedy Crush: take 1 more Prize card for a Pokémon-GX or Pokémon-EX Knocked Out by this attack", owner, ctx.Turn,
                        (e, ko) => ko.HitByAttack && ko.HitByPlayer == owner && !ko.ForcedKnockOut && ko.Def.IsGXorEX));
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name == "Gigafall-GX")
                {
                    bool extra = ExtraEnergy(ctx, atk, 5);
                    ctx.DamageDefending(atk.Damage);
                    if (extra && ctx.Opp.Deck.Count > 0 && !ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 15);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Shadow Connection: as often as you like during your turn, move a basic Darkness Energy from 1 of your Pokémon to another.
        /// Nocturnal Maneuvers-GX: put any number of Basic Pokémon from your deck onto your Bench.
        /// </summary>
        private sealed class WeavileGX : Mon
        {
            private static bool BasicDark(CardInstance x) { return x.Def.IsBasicEnergy && x.Def.EnergyProvides == EnergyType.Darkness; }

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                if (ctx.MyPokemon().Count < 2) return new List<PokemonInPlay>();
                return ctx.MyPokemon(m => m.Energy.Any(BasicDark));
            }

            protected override string Activated { get { return "Shadow Connection"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Givers(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                // As often as you like: never marked as used.
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != "Shadow Connection") return false;
                return ctx.IsMyTurn && Givers(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { Ability(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(Givers(ctx), "Shadow Connection: move a basic Darkness Energy from which Pokémon?");
                if (from == null) return;
                var card = from.Energy.First(BasicDark);
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Shadow Connection: move " + card.Def.Name + " to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nocturnal Maneuvers-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int room = ctx.BenchSpace;
                if (room > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(IsBasicPokemonCard, room, "Nocturnal Maneuvers-GX: put any number of Basic Pokémon from your deck onto your Bench");
                else if (ctx.Me.Deck.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Mirror Gem: during the opponent's next turn, if this Pokémon is damaged by an attack, 8 damage counters on the Attacking Pokémon.</summary>
        private sealed class MirrorGemMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Gem") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new MirrorGemEffect(ctx.Pokemon.Name + " puts 8 damage counters on a Pokémon that damages it (Mirror Gem)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class MirrorGemEffect : ContinuousEffect
        {
            public MirrorGemEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage <= 0) return;
                engine.PlaceDamageCounters(info.Attacker, 8, "Mirror Gem");
            }
        }

        /// <summary>Cross Poison: flip 4 coins, 50 damage for each heads; with at least 2 heads the opponent's Active Pokémon is Poisoned.</summary>
        private sealed class CrossPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross Poison") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(4);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Cleaning Up: discard a Pokémon Tool from 1 of the opponent's Pokémon.</summary>
        internal sealed class CleaningUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cleaning Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Cleaning Up: discard a Pokémon Tool from which of your opponent's Pokémon?");
                if (target != null) ctx.Engine.DiscardTool(target);
                return true;
            }
        }

        /// <summary>Shadow Impact: put 3 damage counters on 1 of your Pokémon.</summary>
        private sealed class ShadowImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Impact") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Shadow Impact: put 3 damage counters on which of your Pokémon?");
                if (target != null) ctx.Engine.PlaceDamageCounters(target, 3, "Shadow Impact");
                return true;
            }
        }

        // ================================================================== Metal and Fairy

        /// <summary>
        /// Captivating Wink: when you play this Pokémon from your hand onto your Bench, you may have the opponent reveal their hand and
        /// put any number of Basic Pokémon from it onto their Bench (not usable while their Bench is full: Compendium, 2019-08-01).
        /// Wily Bite: 30 more for each of their Benched Pokémon. Big
        /// Eater-GX: the opponent reveals their hand and you discard every Supporter there.
        /// </summary>
        private sealed class MawileGX : Mon
        {
            private static readonly Mon WilyBite = new CardsASR1.PlusMon("Wily Bite", 30, c => c.Opp.Bench.Count);

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!ctx.IsMyTurn || ctx.Opp.Hand.Count == 0 || ctx.Engine.BenchSpace(ctx.Opponent) == 0) return;
                if (!ctx.AskYesNo("Captivating Wink: have your opponent reveal their hand and put Basic Pokémon from it onto their Bench?")) return;
                Announce(ctx, "Captivating Wink");
                TefPokemon.RevealOpponentHand(ctx);
                int room = ctx.Engine.BenchSpace(ctx.Opponent);
                var basics = ctx.Opp.Hand.Where(x => ctx.Engine.IsPlayableBasic(x.Def)).ToList();
                if (room == 0 || basics.Count == 0) return;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(room, basics.Count), "Captivating Wink: put any of these Basic Pokémon onto your opponent's Bench");
                foreach (var card in picks)
                {
                    if (ctx.Engine.BenchSpace(ctx.Opponent) == 0 || !ctx.Opp.Hand.Remove(card)) continue;
                    ctx.Engine.PutOntoBench(ctx.Opponent, card, false, true);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Eater-GX") return WilyBite.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                foreach (var card in ctx.Opp.Hand.Where(x => x.Def.IsSupporter).ToList()) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Prowl: when you play this Pokémon from your hand to evolve, you may search your deck for any card and put it into your hand.</summary>
        private sealed class WhimsicottUNM : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Prowl", ctx.Me.Deck.Count > 0, "Prowl: search your deck for a card and put it into your hand?"))
                    ctx.SearchToHand(CardsHIF1.AnyCard, 1, "Prowl: search your deck for a card", false);
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Linear Attack: 40 damage to 1 of the opponent's Pokémon. Calamitous Slash: 80 more if the Defending Pokémon is already
        /// damaged. GG End-GX: discard 1 of the opponent's Pokémon and everything attached (2 with 3 extra Fighting Energy).
        /// </summary>
        private sealed class GarchompGiratinaGX : Mon
        {
            private static readonly Mon Others = new ComboMon(new CardsMEG.SnipeMon("Linear Attack", 40), new CardsPFL.DamagedBonusMon("Calamitous Slash", 80));

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "GG End-GX") return Others.HandleAttack(ctx, atk);
                var f = EnergyType.Fighting;
                int count = CardsCEC1.ExtraTyped(ctx, atk, f, f, f) ? 2 : 1;
                var targets = ctx.OpponentPokemon(m => ctx.CanAffect(m));
                int n = Math.Min(count, targets.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(targets, n, n, "GG End-GX: discard " + (n == 1 ? "which of your opponent's Pokémon?" : n + " of your opponent's Pokémon")))
                    if (ctx.InPlay(target)) ctx.Engine.DiscardPokemonFromPlay(target);
                return true;
            }
        }

        /// <summary>Twister: flip 2 coins, an Energy from the opponent's Active Pokémon discarded for each heads; with 2 tails the attack does nothing.</summary>
        internal sealed class TwisterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twister") return false;
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) break;
                    ctx.DiscardEnergyFrom(defending, 1, "Twister: discard an Energy from " + defending.Name);
                }
                return true;
            }
        }

        /// <summary>Hurricane Charge: once during your turn, attach a Water Energy card, a Lightning Energy card, or 1 of each from your hand to your Pokémon. Dragon Impact: discard 3 Energy.</summary>
        private sealed class DragoniteUNM : Mon
        {
            private static readonly Mon DragonImpact = new CardsTWM1.SelfDiscardMon("Dragon Impact", 3);

            private static bool IsWaterOrLightning(CardDef d)
            {
                return d.IsEnergy && d.IsBasicEnergy && (d.EnergyProvides == EnergyType.Water || d.EnergyProvides == EnergyType.Lightning);
            }

            protected override string Activated { get { return "Hurricane Charge"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(x => IsWaterOrLightning(x.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var type in new[] { EnergyType.Water, EnergyType.Lightning })
                {
                    var t = type;
                    var cards = ctx.Me.Hand.Where(x => x.Def.IsBasicEnergy && x.Def.EnergyProvides == t).ToList();
                    if (cards.Count == 0) continue;
                    var target = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)),
                        "Hurricane Charge: attach a " + t + " Energy card to which of your Pokémon? Press Done to skip.", true);
                    if (target == null) continue;
                    var card = cards[0];
                    if (ctx.Me.Hand.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target, true) && !target.Energy.Contains(card) && !ctx.Me.Discard.Contains(card))
                        ctx.Me.Hand.Add(card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DragonImpact.HandleAttack(ctx, atk); }
        }

        /// <summary>Sky Judgment: discard 3 Energy. Mach Delivery-GX: you may discard cards from your hand down to 9 or fewer, then draw until you have 10.</summary>
        private sealed class DragoniteGX : Mon
        {
            private static readonly Mon SkyJudgment = new CardsTWM1.SelfDiscardMon("Sky Judgment", 3);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Delivery-GX") return SkyJudgment.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int over = ctx.Me.Hand.Count - 9;
                if (ctx.Me.Hand.Count > 0)
                {
                    int min = Math.Max(0, over);
                    var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), min, ctx.Me.Hand.Count,
                        "Mach Delivery-GX: discard any number of cards from your hand" + (min > 0 ? " (at least " + min + ", to have 9 or fewer)" : ""));
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                }
                if (ctx.Me.Hand.Count <= 9) ctx.DrawUntil(10);
                return true;
            }
        }

        /// <summary>Energy Arrow: choose 1 of the opponent's Pokémon; 20 damage for each Energy attached to it, to it, not affected by Weakness or Resistance.</summary>
        private sealed class EnergyArrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Arrow") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Energy Arrow: 20 damage for each Energy attached, to which of your opponent's Pokémon?");
                if (target == null) return true;
                int n = ctx.EnergyCount(target);
                if (n > 0) ctx.DamageWith(target, atk.Damage * n, false, false, false);
                return true;
            }
        }

        /// <summary>Unnerve: prevent all effects of Item and Supporter cards the opponent plays from their hand done to this Pokémon.</summary>
        private sealed class AxewUNM : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && (trainer.IsItem || trainer.IsSupporter);
            }
        }

        /// <summary>
        /// Grind Up: once during your turn, you may discard the Stadium in play; if you do, attach up to 3 Fire and/or Metal Energy
        /// cards from your hand to this Pokémon. Powerful Axe: 40 more for each basic Energy attached to it.
        /// </summary>
        private sealed class HaxorusUNM : Mon
        {
            private static bool FireOrMetal(CardDef d) { return d.IsBasicEnergy && (d.EnergyProvides == EnergyType.Fire || d.EnergyProvides == EnergyType.Metal); }

            protected override string Activated { get { return "Grind Up"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.State.Stadium != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.State.Stadium == null) return;
                ctx.Engine.DiscardStadium();
                var me = ctx.Pokemon;
                var cards = ctx.Me.Hand.Where(x => FireOrMetal(x.Def)).ToList();
                if (cards.Count == 0 || !ctx.InPlay(me) || !ctx.Engine.CanAttachFromHand(me)) return;
                var picks = ctx.ChooseCards(cards, 0, Math.Min(3, cards.Count), "Grind Up: attach up to 3 Fire and/or Metal Energy cards from your hand to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me, true) && !me.Energy.Contains(card) && !ctx.Me.Discard.Contains(card))
                        ctx.Me.Hand.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Powerful Axe") return false;
                ctx.DamageDefending(atk.Damage + 40 * ctx.Pokemon.Energy.Count(x => x.Def.IsBasicEnergy));
                return true;
            }
        }

        /// <summary>
        /// Ultra Conversion: once during your turn, discard an Ultra Beast card from your hand to draw 3 cards. Venom Shot: discard 2
        /// Energy from this Pokémon, then 170 damage to 1 of the opponent's Pokémon. Injection-GX: put a card from the opponent's
        /// discard pile into their Prize cards face down.
        /// </summary>
        private sealed class NaganadelGX : Mon
        {
            protected override string Activated { get { return "Ultra Conversion"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(x => x.Def.IsUltraBeast); }

            protected override void Ability(EffectContext ctx)
            {
                var beasts = ctx.Me.Hand.Where(x => x.Def.IsUltraBeast).ToList();
                if (beasts.Count == 0) return;
                var card = ctx.ChooseCards(beasts, 1, 1, "Ultra Conversion: discard which Ultra Beast card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (ctx.Me.Discard.Contains(card)) ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Venom Shot")
                {
                    var me = ctx.Pokemon;
                    if (me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 2, "Venom Shot: discard 2 Energy from " + me.Name);
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Venom Shot: 170 damage to which of your opponent's Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 170);
                    return true;
                }
                if (atk.Name == "Injection-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Opp.Discard.Count == 0) return true;
                    var card = ctx.ChooseCards(ctx.Opp.Discard.ToList(), 1, 1, "Injection-GX: put which card from your opponent's discard pile into their Prize cards?")[0];
                    if (!ctx.Opp.Discard.Remove(card)) return true;
                    CardsHIF3.AddToPrizes(ctx, ctx.Opponent, new List<CardInstance> { card });
                    ctx.Log("Injection-GX: " + card.Def.Name + " was put into " + ctx.Engine.PlayerName(ctx.Opponent) + "'s Prize cards face down.");
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Heavy Draw: draw a card for each of your Pokémon in play with a Retreat Cost of exactly 4.</summary>
        private sealed class HeavyDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.MyPokemon(m => ctx.Engine.RetreatCostOf(m) == 4).Count;
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Licks Go Crazy: discard a random card from the opponent's hand, the top card of their deck, and an Energy from their Active Pokémon.</summary>
        private sealed class LicksGoCrazyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Licks Go Crazy") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0) TefPokemon.DiscardRandomFromOpponentHand(ctx);
                if (ctx.Opp.Deck.Count > 0 && !ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Licks Go Crazy: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Lazy Howl: if the opponent attaches an Energy card from their hand to the Defending Pokémon during their next turn, their turn ends.</summary>
        private sealed class LazyHowlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lazy Howl") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new LazyHowlEffect("If " + defending.Name + " gets an Energy from the hand next turn, that turn ends (Lazy Howl)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class LazyHowlEffect : ContinuousEffect
        {
            public LazyHowlEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnEnergyAttachedFromHand(GameEngine engine, CardInstance energy, PokemonInPlay target)
            {
                if (target.Uid == TargetUid && engine.State.CurrentPlayer == target.Owner) engine.RequestEndTurn();
            }
        }

        /// <summary>Dynamic Swing: you may do 100 more damage; if you do, this Pokémon takes 100 more damage from attacks during the opponent's next turn.</summary>
        private sealed class DynamicSwingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dynamic Swing") return false;
                bool more = ctx.AskYesNo("Dynamic Swing: do 100 more damage? (" + ctx.Pokemon.Name + " then takes 100 more damage from attacks during your opponent's next turn)");
                ctx.DamageDefending(atk.Damage + (more ? 100 : 0));
                if (more && ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new TakeMoreEffect(ctx.Pokemon.Name + " takes 100 more damage from attacks (Dynamic Swing)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 100));
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, this Pokémon takes N more damage from attacks (after Weakness and Resistance)."</summary>
        internal sealed class TakeMoreEffect : ContinuousEffect
        {
            private readonly int amount;

            public TakeMoreEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.Attacker != null && info.AgainstOpponent && damage > 0 ? damage + amount : damage;
            }
        }

        /// <summary>Unaware: prevent all effects of the opponent's attacks, except damage, done to this Pokémon. Amnesia: the Defending Pokémon can't use the chosen attack next turn.</summary>
        private sealed class BibarelUNM : Mon
        {
            private static readonly Mon Amnesia = new CardsPOR.MemoryLockMon("Amnesia");

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && src.Engine.IsOpponentsAttackOrAbility(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Amnesia.HandleAttack(ctx, atk); }
        }

        /// <summary>Snack Search: once during your turn, flip a coin; on heads put a card from your discard pile on top of your deck. Your turn ends.</summary>
        private sealed class MunchlaxUNM : Mon
        {
            protected override string Activated { get { return "Snack Search"; } }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip() && ctx.Me.Discard.Count > 0)
                {
                    var card = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Snack Search: put which card from your discard pile on top of your deck?")[0];
                    if (ctx.Me.Discard.Remove(card))
                    {
                        ctx.Me.Deck.Insert(0, card);
                        ctx.Log("Snack Search: " + card.Def.Name + " was put on top of " + ctx.Engine.PlayerName(ctx.Player) + "'s deck.");
                    }
                }
                ctx.Log("Snack Search: " + ctx.Engine.PlayerName(ctx.Player) + "'s turn ends.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Downburst: you may have each player shuffle all cards attached to their Active Pokémon into their deck.</summary>
        private sealed class DownburstMon : Mon
        {
            private static void ShuffleAttached(EffectContext ctx, PokemonInPlay mon)
            {
                if (mon == null || !ctx.InPlay(mon)) return;
                if (mon.Owner != ctx.Player && !ctx.CanAffect(mon)) return;
                if (mon.Energy.Count > 0) ctx.Engine.ShuffleEnergyIntoDeck(mon, mon.Energy.ToList());
                var deck = ctx.State.Players[mon.Owner].Deck;
                var tools = mon.Tools.ToList();
                foreach (var tool in tools)
                {
                    mon.Tools.Remove(tool);
                    deck.Add(tool);
                }
                if (tools.Count > 0)
                {
                    ctx.Engine.ShuffleDeck(mon.Owner);
                    ctx.Log(string.Join(", ", tools.Select(x => x.Def.Name).ToArray()) + " from " + mon.Name + " was shuffled into " + ctx.Engine.Possessive(mon.Owner) + " deck.");
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Downburst") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.AskYesNo("Downburst: have each player shuffle all cards attached to their Active Pokémon into their deck?")) return true;
                ShuffleAttached(ctx, ctx.Me.Active);
                ShuffleAttached(ctx, ctx.Opp.Active);
                return true;
            }
        }

        /// <summary>Hearing: once during your turn, if this Pokémon is Active, draw a card. Drain Slap: heal 30 from this Pokémon.</summary>
        private sealed class AudinoUNM : Mon
        {
            private static readonly Mon DrainSlap = new CardsTWM1.HealSelfMon("Drain Slap", 30);

            protected override string Activated { get { return "Hearing"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DrainSlap.HandleAttack(ctx, atk); }
        }

        /// <summary>Thunderous Tornado: if Thundurus is on your Bench, also 20 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class ThunderousTornadoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderous Tornado") return false;
                bool thundurus = ctx.Me.Bench.Any(m => m.Name == "Thundurus");
                ctx.DamageDefending(atk.Damage);
                if (thundurus)
                    foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Alert Headbutt: the base damage is 30 instead of 90 if the opponent's Active Pokémon is a Pokémon-GX or Pokémon-EX.</summary>
        private sealed class AlertHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Alert Headbutt") return false;
                bool gx = ctx.Defending != null && ctx.Defending.Def.IsGXorEX;
                ctx.DamageDefending(gx ? 30 : atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Sage's Riddle: put a Pokémon from your hand face down; the opponent guesses its type; then it's revealed. A right guess: the
        /// opponent draws 4 cards; a wrong one: you draw 4. The Pokémon goes back to your hand.
        /// </summary>
        private sealed class SagesRiddleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sage's Riddle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pokemon = ctx.Me.Hand.Where(x => x.Def.IsPokemon).ToList();
                if (pokemon.Count == 0) return true;
                var hidden = ctx.ChooseCards(pokemon, 1, 1, "Sage's Riddle: put which Pokémon from your hand face down?")[0];
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a Pokémon from their hand face down.");
                var labels = AllTypes.Select(t => t.ToString()).ToArray();
                int guess = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Sage's Riddle: guess the type of the face-down Pokémon",
                    labels.Select((label, i) => new DecisionOption { Label = label, Index = i }).ToList(), ctx.Source);
                var guessed = AllTypes[Math.Max(0, Math.Min(AllTypes.Length - 1, guess))];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { hidden }, "revealed the face-down Pokémon");
                bool right = hidden.Def.PokemonType == guessed || hidden.Def.SecondType == guessed;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed " + guessed + ": " + (right ? "right." : "wrong."));
                if (right) ctx.Engine.DrawCards(ctx.Opponent, 4);
                else ctx.Draw(4);
                return true;
            }
        }

        /// <summary>Drowsing: if this Pokémon is still Asleep after the sleep flip between turns, 6 damage counters on the opponent's Active Pokémon. Snooze: this Pokémon is now Asleep.</summary>
        private sealed class KomalaUNM : Mon
        {
            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? 1 - src.Player : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || !me.Has(SpecialCondition.Asleep)) return;
                var target = src.Opp.Active;
                if (target == null) return;
                Announce(src, "Drowsing");
                src.Engine.PlaceDamageCounters(target, 6, "Drowsing");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snooze") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }
    }
}
