using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Up (TEU, v80), part 3: Fighting, Darkness, Metal, Fairy, Dragon and Colorless (TEU 71-132), and Magikarp &amp;
    /// Wailord-GX (TEU 160, printed only as a full art and a rainbow rare). Lycanroc-GX (TEU 82) is the card already in the pool
    /// as Hidden Fates SV67. Rulings used: Devilish Hands-GX adds up the 30s for each Pokémon and does that damage once (your
    /// ruling, v80); Klefki's Key of Secrets changes only a Resistance a Metal Pokémon already has (your ruling, v80).
    /// </summary>
    public static class CardsTEU3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("TEU-71", new Cards30C.PeerAtMon("Scout"));
            r.RegisterByKey("TEU-72", new CardsAOR2.StadiumBlowMon("Wreck", 80));
            r.RegisterByKey("TEU-73", new HitmonleeTEU());
            r.RegisterByKey("TEU-74", new SwitchSelfMon("Hit and Run", false));
            r.RegisterByKey("TEU-75", new ParalyzeFlipMon("Tickle"));
            r.RegisterByKey("TEU-76", new OmastarTEU());
            r.RegisterByKey("TEU-77", new ShieldAttackMon("Ramming Shell", 20));
            r.RegisterByKey("TEU-78", new KabutopsTEU());
            r.RegisterByKey("TEU-79", new CardsPAF2.IgnoreDefenderEffectsMon("Chip Away"));
            r.RegisterByKey("TEU-80", new CardsFST2.ConditionBonusMon("Payback", 90, ctx => ctx.Opp.Prizes.Count == 1));
            r.RegisterByKey("TEU-81", new ArmThrustMon());
            // Darkness
            r.RegisterByKey("TEU-83", new CardsASR1.PlusMon("Chemical Breath", 50, DefendingConditions));
            r.RegisterByKey("TEU-84", new AlolanMukTEU());
            r.RegisterByKey("TEU-85", new ComboMon(new CardsCEC3.SpreadAmountMon("Spinning Tail", 30),
                new CardsFST2.ConditionBonusMon("Bite Off", 100, ctx => ctx.Defending != null && ctx.Defending.Def.IsGXorEX)));
            r.RegisterByKey("TEU-86", new SearchAttackMon("Howl in the Dark", d => d.IsPokemon && d.PokemonType == EnergyType.Darkness, 2));
            r.RegisterByKey("TEU-87", new CardsASC.RandomDiscardMon("Dark Fang"));
            r.RegisterByKey("TEU-88", new AbsolTEU());
            r.RegisterByKey("TEU-89", new SpiritCompressorMon());
            r.RegisterByKey("TEU-90", new SwitchSelfMon("Hide in Shadows", false));
            r.RegisterByKey("TEU-91", new ComboMon(new CardsDAA1.GustAttackMon("Taunt", false), new NightPunishmentMon()));
            r.RegisterByKey("TEU-92", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("TEU-93", new ComboMon(new TrashCrashMon(), new RecoilMon("Brave Bird", 30)));
            r.RegisterByKey("TEU-94", new ComboMon(new OpponentDiscardsMon("Tighten Up", 2), new CardsRCL2.SelfConditionMon("Tantrum", SpecialCondition.Confused)));
            r.RegisterByKey("TEU-95", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Derail", false), new NoRetreatMon("Clutch")));
            r.RegisterByKey("TEU-96", new HoopaGX());
            r.RegisterByKey("TEU-97", new IncineroarGX());
            // Metal
            r.RegisterByKey("TEU-98", new ComboMon(new CardsFST2.ConditionBonusMon("Calm Strike", 70, ctx => ctx.Me.GxUsed), new ShieldAttackMon("Steel Wing", 30)));
            r.RegisterByKey("TEU-99", new JirachiTEU());
            r.RegisterByKey("TEU-100", new EvolutionaryAdvantageMon());
            r.RegisterByKey("TEU-101", new BronzongTEU());
            r.RegisterByKey("TEU-102", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("TEU-103", new ComboMon(new ShieldAttackMon("Guard Press", 20), new ThornPodThrowMon()));
            r.RegisterByKey("TEU-104", new ShieldAttackMon("Rigidify", 30));
            r.RegisterByKey("TEU-105", new CardsFST2.ConditionBonusMon("Single Lunge", 90, ctx => ctx.Pokemon != null && ctx.Pokemon.DamageCounters == 0));
            r.RegisterByKey("TEU-106", new CobalionGX());
            r.RegisterByKey("TEU-107", new CardsASC.FlipDiscardDefendingEnergyMon("Cut Down"));
            r.RegisterByKey("TEU-108", new CardsASR1.TimesMon("Tool Drop", ToolsInPlay));
            r.RegisterByKey("TEU-109", new AegislashTEU());
            r.RegisterByKey("TEU-110", new KlefkiTEU());
            // Fairy
            r.RegisterByKey("TEU-111", new AlolanNinetalesTEU());
            r.RegisterByKey("TEU-112", new MimikyuTEU());
            // Dragon
            r.RegisterByKey("TEU-113", new LatiasLatiosGX());
            r.RegisterByKey("TEU-114", new CardsASR1.PlusMon("Tropical Shake", 20, BasicEnergyTypesInDiscard));
            r.RegisterByKey("TEU-115", new ComboMon(new ParadiseDrawMon(), new CardsFST1.DiscardFromHandTimesMon("Egg Splat", d => d.Name == "Exeggcute", 99, "Exeggcute")));
            r.RegisterByKey("TEU-116", new CardsBWP.BothHeadsOrNothingMon("Dragon Rage"));
            r.RegisterByKey("TEU-117", new CardsBRS1.LivelyFruitMon());
            r.RegisterByKey("TEU-119", new DragoniteTEU());
            // Colorless
            r.RegisterByKey("TEU-120", new EeveeSnorlaxGX());
            r.RegisterByKey("TEU-121", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("TEU-122", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("TEU-123", new PidgeottoTEU());
            r.RegisterByKey("TEU-124", new ComboMon(new CardsDRI1.WhirlwindMon("Whirlwind"), new SpinStormMon()));
            r.RegisterByKey("TEU-126", new ComboMon(new MakeEmPayMon(), new FlipBonusMon("Sharp Claws", 60)));
            r.RegisterByKey("TEU-127", new ComboMon(new DrawAttackMon("Collect", 2), new ToolBusterMon()));
            r.RegisterByKey("TEU-128", new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 2));
            r.RegisterByKey("TEU-129", new CardsASR1.PlusMon("Raging Herd", 10, ctx => ctx.MyPokemon(m => m.Name == "Tauros" || m.Name == "Tauros-GX").Sum(m => m.DamageCounters)));
            r.RegisterByKey("TEU-130", new ComboMon(new ConditionAttackMon("Supersonic", SpecialCondition.Confused),
                new CardsFST2.ConditionBonusMon("Fossil Fangs", 90, ctx => !ctx.Me.Bench.Any(m => m.Def.IsGXorEX))));
            r.RegisterByKey("TEU-131", new PowerChargerMon());
            r.RegisterByKey("TEU-132", new HeadHuntMon());
            // Magikarp & Wailord-GX (full art only)
            r.RegisterByKey("TEU-160", new MagikarpWailordGX());
        }

        private static int DefendingConditions(EffectContext ctx)
        {
            var d = ctx.Defending;
            if (d == null) return 0;
            int n = 0;
            foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                if (d.Has(c)) n++;
            return n;
        }

        private static int ToolsInPlay(EffectContext ctx)
        {
            int n = 0;
            foreach (var p in ctx.State.Players)
                foreach (var mon in p.InPlay()) n += mon.Tools.Count;
            return n;
        }

        /// <summary>Different types of basic Energy cards in the player's discard pile, up to 5 (Tropical Shake: no more than 100 more damage).</summary>
        internal static int BasicEnergyTypesInDiscard(EffectContext ctx)
        {
            var types = new HashSet<EnergyType>();
            foreach (var c in ctx.Me.Discard)
                if (c.Def.IsBasicEnergy) types.Add(c.Def.EnergyProvides);
            return Math.Min(5, types.Count);
        }

        // ================================================================== Shared

        /// <summary>"Your opponent discards N cards from their hand" (they choose).</summary>
        internal sealed class OpponentDiscardsMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public OpponentDiscardsMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                int n = Math.Min(count, hand.Count);
                var picks = n == hand.Count ? hand : ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, attack + ": discard " + n + (n == 1 ? " card" : " cards") + " from your hand", ctx.Source);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Evolutionary Advantage: if you go second, this Pokémon can evolve during your first turn (not on the turn it was played).</summary>
        internal sealed class EvolutionaryAdvantageMon : Mon
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                var st = src.State;
                if (src.Pokemon == null || target != src.Pokemon) return false;
                if (!st.IsPlayersFirstTurn || st.FirstPlayer == src.Player) return false;
                return target.TurnEnteredPlay < st.TurnNumber && target.TurnEvolved != st.TurnNumber;
            }
        }

        // ================================================================== Fighting

        /// <summary>
        /// Special Combo: only if your Hitmonchan used Hit and Run during your last turn; 90 damage to 1 of your opponent's Benched
        /// Pokémon.
        /// </summary>
        private sealed class HitmonleeTEU : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Special Combo") return true;
                var me = ctx.Me;
                return me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackName == "Hit and Run" && me.LastAttackerDef != null && me.LastAttackerDef.Name == "Hitmonchan";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Combo") return false;
                ctx.DamageOneBenched(90);
                return true;
            }
        }

        /// <summary>Fossil Bind: as long as you have fewer Pokémon in play than your opponent, they can't play Item cards from their hand.</summary>
        private sealed class OmastarTEU : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                if (src.Pokemon == null || player == src.Player || card == null || !card.Def.IsItem) return false;
                return src.Me.InPlay().Count() < src.Opp.InPlay().Count();
            }
        }

        /// <summary>
        /// Fossilized Memories: as long as this Pokémon is your Active Pokémon, your opponent can't play Supporter cards from their
        /// hand. Rock Slide: 20 damage to 2 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class KabutopsTEU : Mon
        {
            private static readonly Mon Slide = new CardsHIF1.BenchedSpreadMon("Rock Slide", 2, 20);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && player != src.Player && card != null && card.Def.IsSupporter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slide.HandleAttack(ctx, atk); }
        }

        /// <summary>Arm Thrust: flip a coin; on heads, discard an Energy from the opponent's Active Pokémon; on tails, this attack does nothing.</summary>
        private sealed class ArmThrustMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Arm Thrust") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Arm Thrust: discard an Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>
        /// Adventurous Appetite: when you play this Pokémon from your hand to evolve 1 of your Pokémon during your turn, you may look
        /// at the top 6 cards of your opponent's deck and discard any number of Item cards there; your opponent shuffles the other
        /// cards back. Gunk Shot: Poisoned.
        /// </summary>
        private sealed class AlolanMukTEU : Mon
        {
            private static readonly Mon Gunk = new ConditionAttackMon("Gunk Shot", SpecialCondition.Poisoned);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Adventurous Appetite", ctx.Opp.Deck.Count > 0, "Adventurous Appetite: look at the top 6 cards of your opponent's deck?")) return;
                var opp = ctx.Opp;
                var top = opp.Deck.Take(6).ToList();
                var items = top.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0 || ctx.Engine.DeckDiscardBlocked(ctx.Opponent))
                    ctx.Engine.ShowCards(ctx.Player, top, "Adventurous Appetite", "The top " + top.Count + " cards of your opponent's deck", ctx.Source);
                else
                {
                    var picks = ctx.ChooseCards(items, 0, items.Count, "Adventurous Appetite: discard any number of Item cards from the top of your opponent's deck");
                    foreach (var card in picks)
                    {
                        opp.Deck.Remove(card);
                        opp.Discard.Add(card);
                    }
                    if (picks.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", picks.Select(c => c.Def.Name).ToArray()) + " from the top of their opponent's deck.");
                }
                ctx.Engine.ShuffleDeck(ctx.Opponent);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Gunk.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Dark Ambition: if your opponent's Active Pokémon is a Basic Pokémon, its Retreat Cost is [C] more. Shadow Seeker: 30 more
        /// damage for each [C] in the opponent's Active Pokémon's Retreat Cost.
        /// </summary>
        private sealed class AbsolTEU : Mon
        {
            private static readonly Mon Seeker = new CardsASR1.PlusMon("Shadow Seeker", 30, ctx => ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending));

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner == src.Player || !src.IsActive(mon) || !mon.IsBasic) return cost;
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Seeker.HandleAttack(ctx, atk); }
        }

        /// <summary>Spirit Compressor: search your deck for up to 4 Pokémon and discard them.</summary>
        private sealed class SpiritCompressorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirit Compressor") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon, 4, "Spirit Compressor: search your deck for up to 4 Pokémon to discard", false);
                foreach (var card in found) ctx.Me.Discard.Add(card);
                if (found.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", found.Select(c => c.Def.Name).ToArray()) + " (Spirit Compressor).");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Night Punishment: 20 damage for each Pokémon in your discard pile, up to 200.</summary>
        private sealed class NightPunishmentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Punishment") return false;
                int pokemon = ctx.Me.Discard.Count(c => c.Def.IsPokemon);
                ctx.DamageDefending(Math.Min(200, atk.Damage * pokemon));
                return true;
            }
        }

        /// <summary>Trash Crash: discard an Item card from your hand; if you do, 60 damage to 1 of the opponent's Pokémon (no Weakness or Resistance).</summary>
        private sealed class TrashCrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trash Crash") return false;
                var items = ctx.Me.Hand.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " has no Item card to discard, so Trash Crash did nothing.");
                    return true;
                }
                var pick = ctx.ChooseCards(items, 1, 1, "Trash Crash: discard an Item card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.DamageAnyOpponent(60, true);
                return true;
            }
        }

        /// <summary>
        /// Rogue Ring: search your deck for up to 2 cards and put them into your hand. Dark Strike: can't use Dark Strike next turn.
        /// Devilish Hands-GX: choose the opponent's Pokémon-GX or Pokémon-EX 6 times; 30 damage for each time each was chosen, added
        /// up and done once (your ruling), not affected by Weakness or Resistance.
        /// </summary>
        private sealed class HoopaGX : Mon
        {
            private static readonly Mon Strike = new LockAfterAttackMon("Dark Strike", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Strike.HandleAttack(ctx, atk)) return true;
                if (atk.Name == "Rogue Ring")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 2, "Rogue Ring: search your deck for up to 2 cards", false, 1);
                    return true;
                }
                if (atk.Name != "Devilish Hands-GX") return false;
                var counts = new Dictionary<int, int>();
                var order = new List<PokemonInPlay>();
                for (int i = 0; i < 6; i++)
                {
                    var targets = ctx.OpponentPokemon(m => m.Def.IsGXorEX);
                    if (targets.Count == 0) break;
                    var t = ctx.ChoosePokemon(targets, "Devilish Hands-GX: choose 1 of your opponent's Pokémon-GX or Pokémon-EX (" + (i + 1) + " of 6)");
                    int n;
                    counts.TryGetValue(t.Uid, out n);
                    if (n == 0) order.Add(t);
                    counts[t.Uid] = n + 1;
                }
                if (order.Count == 0) ctx.Log("Your opponent has no Pokémon-GX or Pokémon-EX, so Devilish Hands-GX did nothing.");
                foreach (var t in order)
                    if (ctx.InPlay(t)) ctx.DamageWith(t, 30 * counts[t.Uid], false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Scar Charge: once during your turn, put 3 damage counters on this Pokémon; then search your deck for up to 3 Darkness
        /// Energy cards and attach them to it. Crushing Punch: discard a Special Energy from the opponent's Active Pokémon. Darkest
        /// Tornado-GX: 50 more damage for each damage counter on this Pokémon.
        /// </summary>
        private sealed class IncineroarGX : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Crushing Punch", false),
                new Cards30C.SelfCounterBonusMon("Darkest Tornado-GX", 50));

            protected override string Activated { get { return "Scar Charge"; } }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.PlaceDamageCounters(ctx.Pokemon, 3, "Scar Charge");
                CardsMEG.ChargeSelfMon.ChargeSelf(ctx, EnergyType.Darkness, 3, "Scar Charge");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>
        /// Stellar Wish: once during your turn, if this Pokémon is your Active Pokémon, look at the top 5 cards of your deck and put
        /// a Trainer card you find there into your hand; shuffle the others back, and this Pokémon is now Asleep.
        /// </summary>
        private sealed class JirachiTEU : Mon
        {
            protected override string Activated { get { return "Stellar Wish"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(5);
                var trainers = top.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) ctx.Engine.ShowCards(ctx.Player, top, "Stellar Wish", "The top " + top.Count + " cards of your deck (no Trainer card)", ctx.Source);
                else
                {
                    var pick = ctx.ChooseCards(trainers, 0, 1, "Stellar Wish: put a Trainer card into your hand");
                    if (pick.Count > 0)
                    {
                        ctx.DeckToHand(pick);
                        ctx.Engine.Reveal(ctx.Player, pick, "put this card into their hand (Stellar Wish)");
                    }
                }
                ctx.ShuffleMyDeck();
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
            }
        }

        /// <summary>Heatproof: prevent all damage done to this Pokémon by attacks from the opponent's Fire Pokémon. Shady Stamp: Confused.</summary>
        private sealed class BronzongTEU : Mon
        {
            private static readonly Mon Stamp = new ConditionAttackMon("Shady Stamp", SpecialCondition.Confused);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.AgainstOpponent && src.Engine.IsType(info.Attacker, EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Stamp.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Thorn Pod Throw: also 20 damage times the Metal Energy attached to this Pokémon to 1 of the opponent's Benched Pokémon,
        /// up to 100.
        /// </summary>
        private sealed class ThornPodThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thorn Pod Throw") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                int amount = Math.Min(100, 20 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal));
                if (amount > 0) ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>
        /// Metal Symbol: your Pokémon with any Metal Energy attached can't be affected by Special Conditions (the engine removes any
        /// they have). Dueling Saber: 60 more damage if a Stadium is in play. Iron Rule-GX: your opponent's Pokémon can't attack
        /// during their next turn.
        /// </summary>
        private sealed class CobalionGX : Mon
        {
            private static readonly Mon Saber = new CardsFST2.ConditionBonusMon("Dueling Saber", 60, ctx => ctx.State.Stadium != null);

            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && src.Engine.HasEnergyOfType(target, EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Saber.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Iron Rule-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsOBF2.PlayerCantAttackEffect(ctx.Engine.PlayerName(ctx.Opponent) + "'s Pokémon can't attack during their next turn (Iron Rule-GX)", ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Royal Guard: this Pokémon takes 40 less damage from attacks. Shield Bash: not affected by effects on the Defending Pokémon.</summary>
        private sealed class AegislashTEU : Mon
        {
            private static readonly Mon Bash = new CardsPAF2.IgnoreDefenderEffectsMon("Shield Bash");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null ? damage - 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bash.HandleAttack(ctx, atk); }
        }

        /// <summary>Key of Secrets: each of your Metal Pokémon's Resistance is now -40 (only a Resistance it has: your ruling).</summary>
        private sealed class KlefkiTEU : Mon
        {
            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !src.Engine.IsType(mon, EnergyType.Metal)) return amount;
                return src.Engine.ResistanceOf(mon).HasValue ? 40 : amount;
            }
        }

        // ================================================================== Fairy

        /// <summary>Luminous Barrier: prevent all effects of attacks, including damage, done to this Pokémon by the opponent's Pokémon-GX and Pokémon-EX.</summary>
        private sealed class AlolanNinetalesTEU : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsGXorEX;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && info.Attacker.Def.IsGXorEX ? 0 : damage;
            }
        }

        /// <summary>
        /// Filch: draw 2 cards. Copycat: if your opponent's Pokémon used an attack that isn't a GX attack during their last turn,
        /// use it as this attack.
        /// </summary>
        private sealed class MimikyuTEU : Mon
        {
            private static readonly Mon Filch = new DrawAttackMon("Filch", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Filch.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Copycat") return false;
                var opp = ctx.Opp;
                var def = opp.LastAttackerDef;
                if (opp.LastAttackTurn != ctx.OpponentsLastTurn || def == null || string.IsNullOrEmpty(opp.LastAttackName))
                {
                    ctx.Log("Your opponent's Pokémon didn't attack during their last turn, so Copycat did nothing.");
                    return true;
                }
                int index = def.Attacks.FindIndex(a => a.Name == opp.LastAttackName);
                if (index < 0 || GameEngine.IsGxAttack(def.Attacks[index]))
                {
                    ctx.Log("Copycat can't copy " + opp.LastAttackName + ".");
                    return true;
                }
                UseCopiedAttack(ctx, def, index);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Buster Purge: discard 3 Energy from this Pokémon. Aero Unit-GX: attach 5 basic Energy cards from your discard pile to
        /// your Pokémon in any way you like; with 1 extra Energy (counted before attaching), prevent all effects of attacks,
        /// including damage, done to this Pokémon during your opponent's next turn.
        /// </summary>
        private sealed class LatiasLatiosGX : Mon
        {
            private static readonly Mon Purge = new CardsTWM1.SelfDiscardMon("Buster Purge", 3);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Purge.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Aero Unit-GX") return false;
                bool extra = ExtraEnergy(ctx, atk, 1);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, Math.Min(5, pile.Count), (c, m) => true, "Aero Unit-GX", false);
                if (extra) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Paradise Draw: you may discard any number of cards from your hand, then draw until you have 6 cards in your hand.</summary>
        private sealed class ParadiseDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paradise Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count > 0)
                {
                    var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 0, ctx.Me.Hand.Count, "Paradise Draw: discard any number of cards from your hand");
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                }
                ctx.DrawUntil(6);
                return true;
            }
        }

        /// <summary>Fast Call: once during your turn, search your deck for a Supporter card and put it into your hand.</summary>
        private sealed class DragoniteTEU : Mon
        {
            protected override string Activated { get { return "Fast Call"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter, 1, "Fast Call: search your deck for a Supporter card");
            }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Cheer Up: attach an Energy card from your hand to 1 of your Pokémon. Dump Truck Press: 120 more against an Evolution
        /// Pokémon. Megaton Friends-GX: with 1 extra Energy, draw until you have 10 cards in your hand.
        /// </summary>
        private sealed class EeveeSnorlaxGX : Mon
        {
            private static readonly Mon Press = new CardsPAL1.EvolutionBonusMon("Dump Truck Press", 120);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Press.HandleAttack(ctx, atk)) return true;
                if (atk.Name == "Cheer Up")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                    if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, 1, (c, m) => true, "Cheer Up");
                    return true;
                }
                if (atk.Name != "Megaton Friends-GX") return false;
                bool extra = ExtraEnergy(ctx, atk, 1);
                ctx.DamageDefending(atk.Damage);
                if (extra) ctx.DrawUntil(10);
                return true;
            }
        }

        /// <summary>Air Mail: once during your turn, look at the top 2 cards of your deck; put 1 into your hand and the other on the bottom of your deck.</summary>
        private sealed class PidgeottoTEU : Mon
        {
            protected override string Activated { get { return "Air Mail"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                var pick = ctx.ChooseCards(top, 1, 1, "Air Mail: put 1 of these cards into your hand (the other goes on the bottom of your deck)");
                ctx.DeckToHand(pick);
                foreach (var card in top)
                {
                    if (pick.Contains(card)) continue;
                    ctx.Me.Deck.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 card into their hand" + (top.Count > 1 ? " and 1 on the bottom of their deck" : "") + " (Air Mail).");
            }
        }

        /// <summary>Spin Storm: your opponent puts their Active Pokémon and all cards attached to it into their hand.</summary>
        private sealed class SpinStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spin Storm") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>
        /// Make 'Em Pay: if your opponent has 4 or more cards in their hand, they reveal it, and you discard cards from it until
        /// they have exactly 4.
        /// </summary>
        private sealed class MakeEmPayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Make 'Em Pay") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count < 4) return true;
                if (hand.Count == 4)
                {
                    ctx.Engine.ShowCards(ctx.Player, hand, "Make 'Em Pay", ctx.Engine.PlayerName(ctx.Opponent) + "'s hand", ctx.Source);
                    return true;
                }
                int n = hand.Count - 4;
                var picks = ctx.ChooseCards(hand, n, n, "Make 'Em Pay: discard " + n + (n == 1 ? " card" : " cards") + " from your opponent's hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Tool Buster: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon; if you did, 70 more damage.</summary>
        private sealed class ToolBusterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tool Buster") return false;
                var defending = ctx.Defending;
                int discarded = 0;
                if (defending != null && defending.Tools.Count > 0 && ctx.CanAffect(defending)) discarded = ctx.Engine.DiscardAllTools(defending);
                ctx.DamageDefending(atk.Damage + (discarded > 0 ? 70 : 0));
                return true;
            }
        }

        /// <summary>Power Charger: search your deck for a basic Energy card and attach it to this Pokémon.</summary>
        private sealed class PowerChargerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Charger") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Power Charger: search your deck for a basic Energy card");
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Head Hunt: look at the top 6 cards of your deck, put any number of Pokémon there into your hand, and discard the others.</summary>
        private sealed class HeadHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Head Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(6);
                if (top.Count == 0) return true;
                var pokemon = top.Where(c => c.Def.IsPokemon).ToList();
                var picks = pokemon.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(pokemon, 0, pokemon.Count, "Head Hunt: put any number of Pokémon into your hand (the other cards are discarded)");
                if (picks.Count > 0)
                {
                    ctx.DeckToHand(picks);
                    ctx.Engine.Reveal(ctx.Player, picks, "put these cards into their hand (Head Hunt)");
                }
                var rest = top.Where(c => !picks.Contains(c)).ToList();
                foreach (var card in rest)
                {
                    ctx.Me.Deck.Remove(card);
                    ctx.Me.Discard.Add(card);
                }
                if (rest.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + string.Join(", ", rest.Select(c => c.Def.Name).ToArray()) + " (Head Hunt).");
                return true;
            }
        }

        /// <summary>Towering Splash-GX: with 7 extra Water Energy, 100 damage to each of your opponent's Benched Pokémon too.</summary>
        private sealed class MagikarpWailordGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Towering Splash-GX") return false;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Water, EnergyType.Water, EnergyType.Water, EnergyType.Water, EnergyType.Water, EnergyType.Water, EnergyType.Water);
                ctx.DamageDefending(atk.Damage);
                if (extra)
                    foreach (var mon in ctx.OpponentBench())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 100);
                return true;
            }
        }
    }
}
