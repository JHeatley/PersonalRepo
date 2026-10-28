using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sandstorm Rare Pokémon and Pokémon-ex (SS 1-26, 94-100; v158). Unlimited only. As with Ruby &amp; Sapphire, a game here is
    /// 1-on-1, so "each Defending Pokémon" and "all Defending Pokémon" are the Defending Pokémon. Shared EX-era helpers used by
    /// the later sets live here too.
    /// </summary>
    public static class CardsSS
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SS-1", new ArmaldoPrimalVeil());
            r.RegisterByKey("SS-2", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, new CardsCES1.FeintAttackMon("Feint Attack", 40)));
            r.RegisterByKey("SS-3", new CradilySuctionCups());
            r.RegisterByKey("SS-4", new ComboMon(new JudgementMon(), new RandomCurseMon()));
            r.RegisterByKey("SS-5", new ComboMon(new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned),
                new CardsSUM1.DiscardTypeFromSelfMon("Flamethrower", EnergyType.Fire, 1)));
            r.RegisterByKey("SS-6", new ComboMon(new CoinsMon("Double Kick", 2), new LightningStrikeMon()));
            r.RegisterByKey("SS-7", new RainDishMon(new CardsBS1.ExtraWaterMon("Hydro Punch")));
            r.RegisterByKey("SS-8", new EclipseMon("Lunar Eclipse", "Solrock", EnergyType.Darkness));
            r.RegisterByKey("SS-9", new ComboMon(new ScamMon(), new SwitchFirstMon("Metal Hook", null)));
            r.RegisterByKey("SS-10", new ComboMon(new SupernaturalMon(), new DiscardForConditionMon("Dark Bind", EnergyType.Darkness, SpecialCondition.Paralyzed)));
            r.RegisterByKey("SS-11", new ComboMon(new DiscardForConditionMon("Deadly Poison", EnergyType.Grass, SpecialCondition.Poisoned), new ExtraPoisonMon()));
            r.RegisterByKey("SS-12", new ShiftryFanAway());
            r.RegisterByKey("SS-13", new EclipseMon("Solar Eclipse", "Lunatone", EnergyType.Fire));
            r.RegisterByKey("SS-14", new ZangooseSS());
            r.RegisterByKey("SS-15", new CardsCRI.CounterConditionMon("Fire Veil", SpecialCondition.Burned, new BurnUpMon()));
            r.RegisterByKey("SS-16", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused),
                new CardsASR1.PlusMon("Energy Crush", 10, c => c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("SS-17", new GolduckChaosFlash());
            r.RegisterByKey("SS-18", new KecleonEnergyVariation());
            r.RegisterByKey("SS-19", new ComboMon(new PullDownMon(), new WaterExtraMon("Hydrocannon", 20)));
            r.RegisterByKey("SS-20", new BabyEvolutionMon("Pikachu", new EnergyRetrievalMon()));
            r.RegisterByKey("SS-21", new ComboMon(new TextNoRetreatMon("Sand Swirl", 20), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("SS-22", new CardsCES1.FeintAttackMon("Feint Attack", 40));
            r.RegisterByKey("SS-23", new ComboMon(new Cards30C.SelfCounterBonusMon("Rage", 10), new EachOpponentMon("Spinning Tail", 20)));
            r.RegisterByKey("SS-24", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("SS-25", new CardsFLF1.NoResistanceMon("Aqua Sonic"));
            r.RegisterByKey("SS-26", new SafeguardExMon(new RecoilMon("Flip Over", 10)));

            r.RegisterByKey("SS-94", new AerodactylExPrimalLock());
            r.RegisterByKey("SS-95", new ComboMon(new CardsFST2.ConditionBonusMon("Rend", 30, c => c.Defending != null && c.Defending.DamageCounters > 0),
                new AlsoEachBenchMon("Metal Surge", 20)));
            r.RegisterByKey("SS-96", new ComboMon(new FeedbackMon(),
                new CardsASR1.TimesMon("Psystorm", c => c.MyPokemon().Sum(m => c.EnergyCount(m)) + c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("SS-97", new ComboMon(new EnergyCoinsMon("Hydrocutter", 3), new SpiralDrainMon("Spiral Drain")));
            r.RegisterByKey("SS-98", new ComboMon(new CardsPAR2.FlipConditionMon("Dazzle Blast", SpecialCondition.Confused), new CardsTWM1.SelfDiscardMon("Mega Thunderbolt", 99)));
            r.RegisterByKey("SS-99", new ComboMon(new BurnNoRetreatMon("Ring of Fire"), new CardsTWM1.SelfDiscardMon("Split Blast", 1)));
            r.RegisterByKey("SS-100", new ComboMon(new SuperDeepDiveMon(), new MinusSelfCountersMon("Dwindling Wave")));
        }

        // ================================================================== Shared (EX era)

        /// <summary>A Pokémon card in play anywhere with this exact name (a Pokémon-ex has a different name: "Aerodactyl ex").</summary>
        internal static bool NamedInPlay(EffectContext ctx, string name)
        {
            return ctx.MyPokemon().Any(m => m.Name == name) || ctx.OpponentPokemon().Any(m => m.Name == name);
        }

        /// <summary>The opponent has an Evolved Pokémon in play.</summary>
        internal static bool OpponentHasEvolved(EffectContext ctx) { return ctx.OpponentPokemon().Any(CardsE2.IsEvolved); }

        /// <summary>
        /// Baby Evolution (Poké-Power): once during your turn, put [Pokémon] from your hand onto this Pokémon (evolving it) and remove
        /// all its damage counters. Any turn (the v136 ruling for Mime Jr.); no Special Condition clause in the text.
        /// </summary>
        internal sealed class BabyEvolutionMon : Mon
        {
            private readonly string evolution;
            private readonly Mon attacks;

            public BabyEvolutionMon(string evolution, Mon attacks)
            {
                this.evolution = evolution;
                this.attacks = attacks;
            }

            private List<CardInstance> Cards(EffectContext ctx)
            {
                string wanted = evolution;
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.Name == wanted).ToList();
            }

            protected override string Activated { get { return "Baby Evolution"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Cards(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var cards = Cards(ctx);
                if (cards.Count == 0) return;
                var card = cards.Select(c => c.Def.Key).Distinct().Count() == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Baby Evolution: put which " + evolution + " onto " + me.Name + "?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.EvolvePokemon(me, card, true);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Rain Dish (Poké-Body): at any time between turns, remove 1 damage counter from this Pokémon (Spongy Stone too).</summary>
        internal sealed class RainDishMon : Mon
        {
            private readonly Mon attacks;
            private readonly string body;

            public RainDishMon(Mon attacks) : this("Rain Dish", attacks) { }

            public RainDishMon(string body, Mon attacks)
            {
                this.body = body;
                this.attacks = attacks;
            }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.InPlay(me) || me.DamageCounters == 0) return;
                Announce(src, body);
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"N more damage for each Water Energy attached but not used to pay for this attack's cost; no more than 2 extra" (Hydrocannon: 20 each).</summary>
        internal sealed class WaterExtraMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public WaterExtraMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + per * CardsBS1.ExtraWaterMon.Extra(ctx, atk));
                return true;
            }
        }

        /// <summary>"Does N damage to each Defending Pokémon. The Defending Pokémon can't retreat until the end of your opponent's next turn" (Sand Swirl).</summary>
        internal sealed class TextNoRetreatMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public TextNoRetreatMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + amount);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>"Does N damage to each of your opponent's Pokémon" (no Weakness or Resistance for Benched Pokémon): Spinning Tail.</summary>
        internal sealed class EachOpponentMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public EachOpponentMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + amount);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>The printed damage, plus N to each of the opponent's Benched Pokémon (no Weakness or Resistance): Metal Surge.</summary>
        internal sealed class AlsoEachBenchMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public AlsoEachBenchMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>"Remove 2 damage counters from this Pokémon (remove 1 if there is only 1)" after the damage (Spiral Drain).</summary>
        internal sealed class SpiralDrainMon : Mon
        {
            private readonly string attack;

            public SpiralDrainMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                return true;
            }
        }

        /// <summary>"You may discard a [type] Energy card attached to this Pokémon. If you do, the Defending Pokémon is now [condition]."</summary>
        internal sealed class DiscardForConditionMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly SpecialCondition condition;

            public DiscardForConditionMon(string attack, EnergyType type, SpecialCondition condition)
            {
                this.attack = attack;
                this.type = type;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                var cards = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, type)).ToList();
                if (cards.Count == 0 || !ctx.AskYesNo(attack + ": discard a " + type + " Energy card from " + me.Name + " to make the Defending Pokémon " + condition + "?")) return true;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, attack + ": discard which Energy card?")[0];
                if (ctx.Engine.DiscardEnergy(me, card) && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"Before doing damage, you may switch 1 of your opponent's Benched Pokémon with the Defending Pokémon" (Metal Hook; Lure Poison then Poisons).</summary>
        internal sealed class SwitchFirstMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition? condition;

            public SwitchFirstMon(string attack, SpecialCondition? condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo(attack + ": switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?"))
                    ctx.GustOpponent(attack + ": choose the new Defending Pokémon");
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (condition.HasValue && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition.Value);
                return true;
            }
        }

        /// <summary>
        /// Safeguard (Poké-Body, EX era): prevent all effects of attacks, damage included, done to this Pokémon by the opponent's
        /// Pokémon-ex.
        /// </summary>
        internal sealed class SafeguardExMon : Mon
        {
            private readonly Mon attacks;

            public SafeguardExMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player || !a.Def.IsOldEx) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsOldEx;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Choose 1 card from your opponent's hand without looking. Look at it, then your opponent shuffles it into their deck" (Surprise).</summary>
        internal sealed class SurpriseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { card }, "Surprise", "Surprise: the card you chose", ctx.Source);
                if (!hand.Remove(card)) return true;
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log("Surprise: " + card.Def.Name + " was shuffled into " + ctx.Engine.Possessive(ctx.Opponent) + " deck.");
                return true;
            }
        }

        /// <summary>Team Assembly / Influence: the named Pokémon (and, for Team Assembly, any Basic Pokémon) from the deck onto the Bench as Basic Pokémon.</summary>
        internal sealed class BenchNamedMon : Mon
        {
            private readonly string attack;
            private readonly string[] names;
            private readonly bool anyBasic;
            private readonly int max;

            public BenchNamedMon(string attack, string[] names, bool anyBasic, int max)
            {
                this.attack = attack;
                this.names = names;
                this.anyBasic = anyBasic;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(max, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && (names.Contains(d.Name) || (anyBasic && d.IsBasicPokemon)) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    attack + ": choose " + string.Join(", ", names) + (anyBasic ? " or Basic Pokémon" : "") + " to put onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Cosmic Draw: if the opponent has any Evolved Pokémon in play, draw 3 cards.</summary>
        internal sealed class CosmicDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cosmic Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (OpponentHasEvolved(ctx)) ctx.Draw(3);
                return true;
            }
        }

        /// <summary>"Flip a coin for each Energy attached to this Pokémon (no more than N coins); X damage for each heads" (Hydrocutter).</summary>
        internal sealed class EnergyCoinsMon : Mon
        {
            private readonly string attack;
            private readonly int maxCoins;

            public EnergyCoinsMon(string attack, int maxCoins)
            {
                this.attack = attack;
                this.maxCoins = maxCoins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int coins = Math.Min(maxCoins, ctx.EnergyCount(ctx.Pokemon));
                ctx.DamageDefending(coins <= 0 ? 0 : atk.Damage * ctx.FlipCoins(coins));
                return true;
            }
        }

        /// <summary>"The printed damage minus 10 for each damage counter on this Pokémon" (Dwindling Wave).</summary>
        internal sealed class MinusSelfCountersMon : Mon
        {
            private readonly string attack;

            public MinusSelfCountersMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        /// <summary>The Defending Pokémon is now Burned, and it can't retreat during the opponent's next turn (Ring of Fire).</summary>
        internal sealed class BurnNoRetreatMon : Mon
        {
            private readonly string attack;

            public BurnNoRetreatMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon [condition], this attack does N more damage."</summary>
        internal static Mon ConditionBonus(string attack, int bonus, Func<PokemonInPlay, bool> defending)
        {
            return new CardsFST2.ConditionBonusMon(attack, bonus, c => c.Defending != null && defending(c.Defending));
        }

        /// <summary>Pester: 20 more if the Defending Pokémon is affected by a Special Condition.</summary>
        internal static Mon Pester(int bonus)
        {
            return new CardsFST2.ConditionBonusMon("Pester", bonus, c => c.Defending != null && c.Defending.Conditions != SpecialCondition.None);
        }

        // ================================================================== Pokémon

        /// <summary>Primal Veil (Poké-Body): while Armaldo is your Active Pokémon, neither player can play Supporter cards.</summary>
        private sealed class ArmaldoPrimalVeil : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                return me != null && card != null && card.Def.IsSupporter && src.IsActive(me);
            }
        }

        /// <summary>
        /// Super Suction Cups (Poké-Body): while Cradily is your Active Pokémon, the opponent's Pokémon can't retreat. Lure Poison:
        /// you may switch in 1 of the opponent's Benched Pokémon first; the Defending Pokémon is now Poisoned. Spiral Drain.
        /// </summary>
        private sealed class CradilySuctionCups : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new SwitchFirstMon("Lure Poison", SpecialCondition.Poisoned), new SpiralDrainMon("Spiral Drain"));

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && mon.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Judgement: flip 2 coins; if both are heads, the Defending Pokémon is Knocked Out.</summary>
        private sealed class JudgementMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Judgement") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending)) ctx.Engine.KnockOut(defending, "Judgement");
                return true;
            }
        }

        /// <summary>Random Curse: 5 damage counters on the Defending Pokémon (1-on-1: "all Defending Pokémon in any way you like").</summary>
        private sealed class RandomCurseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Random Curse") return false;
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 5);
                return true;
            }
        }

        /// <summary>Lightning Strike: you may discard all Lightning Energy cards attached to Jolteon; if you do, base damage 70 instead of 40.</summary>
        private sealed class LightningStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Strike") return false;
                var me = ctx.Pokemon;
                int damage = atk.Damage;
                var cards = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList();
                if (!CardsBS1.CopyingWithMetronome && cards.Count > 0 && ctx.AskYesNo("Lightning Strike: discard all Lightning Energy cards from " + me.Name + " for 70 damage?"))
                {
                    int gone = 0;
                    foreach (var card in cards) if (ctx.Engine.DiscardEnergy(me, card)) gone++;
                    if (gone > 0) damage = 70;
                }
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>
        /// Lunar Eclipse / Solar Eclipse (Poké-Power): once during your turn, if [partner] is in play (either player's), this Pokémon's
        /// type is [type] until the end of your turn; not while it has a Special Condition. Cosmic Draw, and a plain attack.
        /// </summary>
        private sealed class EclipseMon : Mon
        {
            private static readonly Mon Draw = new CosmicDrawMon();

            private readonly string power;
            private readonly string partner;
            private readonly EnergyType type;

            public EclipseMon(string power, string partner, EnergyType type)
            {
                this.power = power;
                this.partner = partner;
                this.type = type;
            }

            protected override string Activated { get { return power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && NamedInPlay(ctx, partner); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Pokemon.Markers[power] = ctx.Turn;
                ctx.Log(power + ": " + ctx.Pokemon.Name + " is " + type + " until the end of the turn.");
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                int turn;
                if (src.Pokemon == null || mon != src.Pokemon || !mon.Markers.TryGetValue(power, out turn) || turn != src.Turn) return;
                types.Clear();
                types.Add(type);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Draw.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Scam: look at the opponent's hand; you may have them shuffle a Supporter card from it into their deck, and then they draw a
        /// card.
        /// </summary>
        private sealed class ScamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scam") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Scam: your opponent's hand", ctx.Source);
                var supporters = hand.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                var picks = ctx.ChooseCards(supporters, 0, 1, "Scam: choose a Supporter card for your opponent to shuffle into their deck (or none)");
                if (picks.Count == 0 || !ctx.Opp.Hand.Remove(picks[0])) return true;
                ctx.Opp.Deck.Add(picks[0]);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log("Scam: " + picks[0].Def.Name + " was shuffled into " + ctx.Engine.Possessive(ctx.Opponent) + " deck.");
                ctx.Engine.DrawCards(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>
        /// Supernatural: look at the opponent's hand and you may use the effect of a Supporter card there as this attack's effect
        /// (it stays in their hand). Like Look-Alike Show: only a Supporter whose effect would do something.
        /// </summary>
        private sealed class SupernaturalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supernatural") return false;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Supernatural: your opponent's hand", ctx.Source);
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var inHand in hand)
                    if (inHand.Def.IsSupporter && seen.Add(inHand.Def.Name) && MrMimeTEF.WouldDoSomething(ctx, inHand)) usable.Add(inHand);
                if (usable.Count == 0) return true;
                var labels = usable.Select(c => "Use " + c.Def.Name + "'s effect").ToList();
                labels.Add("Don't use a Supporter's effect");
                int pick = ctx.ChooseOption("Supernatural: use the effect of a Supporter from your opponent's hand?", labels.ToArray());
                if (pick < 0 || pick >= usable.Count) return true;
                var card = usable[pick];
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + card.Def.Name + ".");
                ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                return true;
            }
        }

        /// <summary>Extra Poison: if the Defending Pokémon is a Pokémon-ex, it is now Asleep and Poisoned.</summary>
        internal sealed class ExtraPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extra Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !defending.Def.IsOldEx) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>
        /// Fan Away (Poké-Power): once during your turn, flip a coin; heads, 1 Energy card attached to the Defending Pokémon goes back
        /// to the opponent's hand. Not while Shiftry has a Special Condition. Light Touch Throw: 80 minus 10 per Energy on the
        /// Defending Pokémon.
        /// </summary>
        private sealed class ShiftryFanAway : Mon
        {
            private const string Power = "Fan Away";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (defending == null || !ctx.Flip()) return;
                var cards = defending.Energy.ToList();
                if (cards.Count == 0) return;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, Power + ": return which Energy card to your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Light Touch Throw") return false;
                int energy = ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending);
                ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * energy));
                return true;
            }
        }

        /// <summary>Poison Resistance (Poké-Body): Zangoose can't be Poisoned. Target Slash: 30 more against Seviper. Super Slash: 30 more against an Evolved Pokémon.</summary>
        private sealed class ZangooseSS : Mon
        {
            private static readonly Mon Attacks = new ComboMon(
                new CardsFST2.ConditionBonusMon("Target Slash", 30, c => c.Defending != null && c.Defending.Name == "Seviper"),
                new CardsFST2.ConditionBonusMon("Super Slash", 30, c => CardsE2.IsEvolved(c.Defending)));

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target == src.Pokemon && (condition & SpecialCondition.Poisoned) != 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Burn Up: flip a coin; tails, discard all Fire Energy cards attached to this Pokémon.</summary>
        private sealed class BurnUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burn Up") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads || !ctx.InPlay(me)) return true;
                foreach (var card in me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>
        /// Chaos Flash (Poké-Power): once during your turn, while Golduck is your Active Pokémon, flip a coin; heads, the Defending
        /// Pokémon is Confused. Not while Golduck has a Special Condition. Special Blow: 40 more if the Defending Pokémon has any
        /// Special Energy cards attached.
        /// </summary>
        private sealed class GolduckChaosFlash : Mon
        {
            private const string Power = "Chaos Flash";
            private static readonly Mon Blow = new CardsFST2.ConditionBonusMon("Special Blow", 40, c => c.Defending != null && c.Defending.Energy.Any(e => !e.Def.IsBasicEnergy));

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.PokemonIsActive && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (defending != null && ctx.Flip()) ctx.ApplyCondition(defending, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blow.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Variation (Poké-Body): Kecleon is every type of basic Energy card attached to it (Colorless with none). Double Scratch.</summary>
        private sealed class KecleonEnergyVariation : Mon
        {
            private static readonly Mon Scratch = new CoinsMon("Double Scratch", 2);

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                var me = src.Pokemon;
                if (me == null || mon != me) return;
                // The printed type of each basic Energy card, so the check can't loop through effects that change types.
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().ToList();
                if (basics.Count == 0) return;
                types.Clear();
                types.AddRange(basics);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Scratch.HandleAttack(ctx, atk); }
        }

        /// <summary>Pull Down: the highest Stage Evolution card comes off each of the opponent's Evolved Pokémon, into their hand.</summary>
        private sealed class PullDownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull Down") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(CardsE2.IsEvolved))
                    if (ctx.InPlay(mon) && ctx.CanAffect(mon)) ctx.Engine.Devolve(mon, 1);
                return true;
            }
        }

        /// <summary>
        /// Energy Retrieval: up to 2 basic Energy cards from your discard pile onto 1 of your Pokémon, then 1 damage counter on it for
        /// each one attached.
        /// </summary>
        private sealed class EnergyRetrievalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Retrieval") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0) return true;
                var picks = ctx.ChooseCards(basics, 1, Math.Min(2, basics.Count), "Energy Retrieval: choose up to 2 basic Energy cards");
                if (picks.Count == 0) return true;
                var mine = ctx.MyPokemon();
                var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Energy Retrieval: attach them to which Pokémon?");
                if (target == null) return true;
                int attached = 0;
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card) && ctx.Engine.AttachEnergyCard(card, target)) attached++;
                if (attached > 0 && ctx.InPlay(target)) ctx.PlaceCounters(target, attached);
                return true;
            }
        }

        /// <summary>
        /// Primal Lock (Poké-Body): while Aerodactyl ex is in play, the opponent can't play Pokémon Tool cards, and any Pokémon Tool
        /// cards attached to their Pokémon are discarded. Supersonic: Confused. Wing Attack.
        /// </summary>
        private sealed class AerodactylExPrimalLock : Mon
        {
            private const string Lock = "Primal Lock";
            private static readonly Mon Sonic = new ConditionAttackMon("Supersonic", SpecialCondition.Confused);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && player != src.Player && card != null && card.Def.TrainerKind == TrainerKind.Tool;
            }

            public override void OnStateCheck(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || src.Engine.AbilityBlocked(me, Lock)) return;
                foreach (var mon in src.OpponentPokemon(m => m.Tools.Count > 0))
                {
                    src.Log(Lock + ": the Pokémon Tool cards on " + mon.Name + " are discarded.");
                    src.Engine.DiscardAllTools(mon);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sonic.HandleAttack(ctx, atk); }
        }

        /// <summary>Feedback: as many damage counters on the Defending Pokémon as the opponent has cards in their hand.</summary>
        private sealed class FeedbackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Feedback") return false;
                int n = ctx.Opp.Hand.Count;
                if (ctx.Defending != null && n > 0) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>
        /// Super Deep Dive: with no Benched Pokémon, nothing; otherwise remove 3 damage counters from Wailord ex and switch it with 1
        /// of your Benched Pokémon.
        /// </summary>
        private sealed class SuperDeepDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Deep Dive") return false;
                if (ctx.Me.Bench.Count == 0) return Nothing(ctx);
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, 30);
                if (ctx.InPlay(me) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }
    }
}
