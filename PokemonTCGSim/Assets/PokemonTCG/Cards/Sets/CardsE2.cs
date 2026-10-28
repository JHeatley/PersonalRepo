using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Aquapolis Rare Pokémon (E2 1-36, 38-44) and the Crystal Pokémon Kingdra and Nidoking (E2 148, 150; v146). Unlimited only.
    /// H1-H32 are holo prints of 32 of 1-44; Suicune (37, H25) is Wizards Promo 53 and Crystal Lugia (149) is 30C-CC7, both
    /// already in the pool. Poké-Powers that say so can't be used while their Pokémon is affected by any Special Condition.
    /// Engine work: Dark Impact stops Poké-Powers (ContinuousEffect.StopsPokePower), Fluff flips like Haunter's Transparency
    /// (CardScript.FlipsToDodgeAttack) and Anti-Lightning stops Lightning Energy cards from the hand (BlocksEnergyFromHandTo).
    /// </summary>
    public static class CardsE2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E2-1", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CardsE1.ReflectEnergyMon()));
            r.RegisterByKey("E2-2", new ArcanineE2());
            r.RegisterByKey("E2-3", new AriadosE2());
            r.RegisterByKey("E2-4", new AzumarillE2());
            r.RegisterByKey("E2-5", new BellossomE2());
            r.RegisterByKey("E2-6", new BlisseyE2());
            r.RegisterByKey("E2-7", new ComboMon(new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10), new CoinsMon("Double Stab", 2)));
            r.RegisterByKey("E2-8", new ElectrodeE2());
            r.RegisterByKey("E2-10", new EnteiE2());
            r.RegisterByKey("E2-11", new EspeonE2());
            r.RegisterByKey("E2-12", new ComboMon(new SuperEggsplosionMon(), new CalledShotMon()));
            r.RegisterByKey("E2-13", new ComboMon(new CardsASC.FlipPerEnergyMon("Big Eggsplosion"), new LateralEggsplosionMon()));
            r.RegisterByKey("E2-14", new ComboMon(new CardsUPR3.SmokescreenMon("Smokescreen", 1), new BurnUpMon()));
            r.RegisterByKey("E2-15", new ComboMon(new FireworksMon(), new DarkImpactMon()));
            r.RegisterByKey("E2-16", new HypnoE2());
            r.RegisterByKey("E2-17", new JumpluffE2());
            r.RegisterByKey("E2-18", new ComboMon(new MeditateMon(), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("E2-19", new KingdraE2());
            r.RegisterByKey("E2-20", new ConductionMon());
            r.RegisterByKey("E2-21", new LanturnE2());
            r.RegisterByKey("E2-22", new MagnetonE2());
            r.RegisterByKey("E2-23", new MukE2());
            r.RegisterByKey("E2-24", new NidokingE2());
            r.RegisterByKey("E2-25", new ComboMon(new MiracleTailMon(), new RoastingHeatMon()));
            r.RegisterByKey("E2-26", new OctilleryE2());
            r.RegisterByKey("E2-27", new ComboMon(new SleepInducerMon(false), new RapturePollenMon()));
            r.RegisterByKey("E2-28", new Porygon2E2());
            r.RegisterByKey("E2-29", new ComboMon(new CardsBS1.KarateChopMon(), new SuddenChargeMon()));
            r.RegisterByKey("E2-30", new ComboMon(new CardsBS1.ExtraWaterMon("Water Gun"), new CoinsMon("Slam", 2)));
            r.RegisterByKey("E2-31", new ComboMon(new DodgeMon("Agility", false), new GallopMon()));
            r.RegisterByKey("E2-32", new ScizorE2());
            r.RegisterByKey("E2-33", new SlowbroE2());
            r.RegisterByKey("E2-34", new ComboMon(new BurstingHandMon(), new ShuffleAttackMon()));
            r.RegisterByKey("E2-35", new ComboMon(new EarthRiftMon(), new IronSmashMon()));
            r.RegisterByKey("E2-36", new ComboMon(new CopyMon(), new CardsE1.EnergyDrawMon()));
            r.RegisterByKey("E2-38", new TentacruelE2());
            r.RegisterByKey("E2-39", new TogeticE2());
            r.RegisterByKey("E2-40", new ComboMon(new DestructiveRoarMon(), new GigacrushMon()));
            r.RegisterByKey("E2-41", new UmbreonE2());
            r.RegisterByKey("E2-42", new VictreebelE2());
            r.RegisterByKey("E2-43", new ComboMon(new PoisonScentMon(), new AddictivePollenMon()));
            r.RegisterByKey("E2-44", new ZapdosE2());
            r.RegisterByKey("E2-148", new CrystalTypeMon(new[] { EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic }, new ComboMon(new AquabombMon(), new DualBurnMon())));
            r.RegisterByKey("E2-150", new CrystalTypeMon(new[] { EnergyType.Grass, EnergyType.Lightning, EnergyType.Fire },
                new ComboMon(new ConditionAttackMon("Poison Horn", SpecialCondition.Poisoned), new RollingThunderMon())));
        }

        // ================================================================== Shared

        /// <summary>Energy cards attached to the Pokémon that provide this type ("[type] Energy cards attached to it").</summary>
        internal static List<CardInstance> EnergyCardsOf(EffectContext ctx, PokemonInPlay mon, EnergyType type)
        {
            return mon == null ? new List<CardInstance>() : mon.Energy.Where(e => CardsDRI1.ProvidesType(ctx, mon, e, type)).ToList();
        }

        internal static bool IsBasicEnergyOf(CardDef d, EnergyType type) { return d != null && d.IsBasicEnergy && d.EnergyProvides == type; }

        /// <summary>"Evolved Pokémon": something is on top of its Basic card (Antique Fossils and LEGEND halves aren't evolved).</summary>
        internal static bool IsEvolved(PokemonInPlay mon)
        {
            return mon != null && mon.Stack.Count > 1 && !mon.Def.IsFossil && !mon.Def.IsMultiCard;
        }

        /// <summary>Discards the Energy cards chosen from a list attached to the Pokémon. Returns how many were discarded.</summary>
        internal static int DiscardCards(EffectContext ctx, PokemonInPlay mon, IEnumerable<CardInstance> cards)
        {
            int n = 0;
            foreach (var card in cards.ToList()) if (ctx.Engine.DiscardEnergy(mon, card)) n++;
            return n;
        }

        /// <summary>Discard 1 Energy card of this type attached to the Pokémon (the player chooses which). Returns true if one was discarded.</summary>
        internal static bool DiscardOneOf(EffectContext ctx, PokemonInPlay mon, EnergyType type, string source)
        {
            var cards = EnergyCardsOf(ctx, mon, type);
            if (cards.Count == 0 || !ctx.InPlay(mon)) return false;
            var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, source + ": discard which " + type + " Energy card?")[0];
            return ctx.Engine.DiscardEnergy(mon, card);
        }

        /// <summary>Looks at the top N cards of a player's deck and puts them back in any order (the player using the effect chooses).</summary>
        internal static void ReorderTop(EffectContext ctx, int owner, int count, string source)
        {
            var deck = ctx.State.Players[owner].Deck;
            var top = deck.Take(count).ToList();
            if (top.Count == 0) return;
            string whose = owner == ctx.Player ? "your" : "your opponent's";
            if (top.Count == 1)
            {
                ctx.Engine.ShowCards(ctx.Player, top, source, "The top card of " + whose + " deck.", ctx.Source);
                return;
            }
            foreach (var c in top) deck.Remove(c);
            var ordered = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = ctx.ChooseCards(left, 1, 1, source + ": choose the card to put " + (ordered.Count == 0 ? "on top of " + whose + " deck" : "next (card " + (ordered.Count + 1) + " from the top)"))[0];
                ordered.Add(pick);
                left.Remove(pick);
            }
            ordered.Add(left[0]);
            deck.InsertRange(0, ordered);
            ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the top " + ordered.Count + " cards of " + (owner == ctx.Player ? "their" : ctx.Engine.Possessive(owner)) + " deck back in any order (" + source + ").");
        }

        /// <summary>"Flip a coin until you get tails. This attack does N damage times the number of heads."</summary>
        internal sealed class FlipUntilTailsMon : Mon
        {
            private readonly string attack;

            public FlipUntilTailsMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Pokémon. This attack does N damage to that Pokémon. Don't apply Weakness or Resistance."</summary>
        internal sealed class AnyOpponentMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public AnyOpponentMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageAnyOpponent(amount, true, false);
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Benched Pokémon and switch the Defending Pokémon with it" (no damage); optionally the new one is Asleep, and a coin first.</summary>
        internal sealed class SleepInducerMon : Mon
        {
            private readonly bool flipFirst;

            public SleepInducerMon(bool flipFirst) { this.flipFirst = flipFirst; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleep Inducer") return false;
                if (flipFirst && !ctx.Flip()) return true;
                if (ctx.GustTargets().Count == 0) return true;
                var target = ctx.GustOpponent("Sleep Inducer: choose 1 of your opponent's Benched Pokémon to switch in");
                if (target != null && ctx.Opp.Active == target) ctx.ApplyCondition(target, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Swift-style damage: not affected by Weakness, Resistance or any effects on the Defending Pokémon.</summary>
        private static void SwiftDamage(EffectContext ctx, int amount)
        {
            if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, amount, false, false, true);
        }

        // ================================================================== Grass

        /// <summary>
        /// Gluey Slime (Poké-Body): each player pays [C] more to retreat their Active Pokémon; only 1 [C] more however many Ariados
        /// are in play (the first one with a working Gluey Slime counts). Spider Force: heads 30; tails 20 and Paralyzed.
        /// </summary>
        private sealed class AriadosE2 : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || !src.IsActive(mon)) return cost;
                foreach (var other in src.Engine.SourcesFor(mon))
                {
                    if (!(other.Script is AriadosE2) || other.Ctx.Pokemon == null) continue;
                    if (other.Ctx.Pokemon != src.Pokemon) return cost;
                    break;
                }
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spider Force") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Flower Supplement (Poké-Power): flip; heads, attach a basic Energy card from your hand to 1 of your Benched Pokémon. Knife Leaf: 3 coins.</summary>
        private sealed class BellossomE2 : Mon
        {
            private const string Power = "Flower Supplement";
            private static readonly Mon KnifeLeaf = new CoinsMon("Knife Leaf", 3);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return;
                ctx.AttachCardsAnyWay(energy, 1, (card, m) => ctx.Me.Bench.Contains(m), Power, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return KnifeLeaf.HandleAttack(ctx, atk); }
        }

        /// <summary>Super Eggsplosion: discard any number of Energy cards from your Pokémon; a coin for each, 40 per heads.</summary>
        private sealed class SuperEggsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Eggsplosion") return false;
                int discarded = 0;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var all = ctx.MyPokemon().SelectMany(m => m.Energy).ToList();
                    if (all.Count > 0)
                    {
                        var picks = ctx.ChooseCards(all, 0, all.Count, "Super Eggsplosion: discard any number of Energy cards attached to your Pokémon (a coin for each)");
                        foreach (var card in picks)
                        {
                            var holder = ctx.MyPokemon().FirstOrDefault(m => m.Energy.Contains(card));
                            if (holder != null && ctx.Engine.DiscardEnergy(holder, card)) discarded++;
                        }
                    }
                }
                int heads = discarded > 0 ? ctx.FlipCoins(discarded) : 0;
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Called Shot: 10 damage times the Grass Energy attached to this Pokémon to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class CalledShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Called Shot") return false;
                int grass = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                if (grass > 0 && ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10 * grass);
                return true;
            }
        }

        /// <summary>Fluff (Poké-Body): during the opponent's turn, once it has a damage counter, the first time their attack would affect it, flip; heads, the attack does nothing to it. Cotton Punch: 2 coins.</summary>
        private sealed class JumpluffE2 : Mon
        {
            private static readonly Mon CottonPunch = new CoinsMon("Cotton Punch", 2);

            public override string FlipsToDodgeAttack(EffectContext src, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || src.IsMyTurn || target.DamageCounters < 1) return null;
                return src.Engine.CurrentSourceOwner == src.Player ? null : "Fluff";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CottonPunch.HandleAttack(ctx, atk); }
        }

        /// <summary>Heavyweight (Poké-Body): with a Grass Energy attached, its Retreat Cost is [C][C] more. Burning Sludge: a coin per Grass Energy; any heads, Poisoned and Burned.</summary>
        private sealed class MukE2 : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon != src.Pokemon || src.Engine.EnergyCountOfType(mon, EnergyType.Grass) == 0) return cost;
                return cost + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Sludge") return false;
                var defending = ctx.Defending;
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                }
                return true;
            }
        }

        /// <summary>Rapture Pollen: heads, the Defending Pokémon is Asleep; tails, 2 damage counters off this Pokémon.</summary>
        private sealed class RapturePollenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapture Pollen") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads) { if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep); }
                else if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                return true;
            }
        }

        /// <summary>Fragrance Trap (Poké-Power): flip; heads, switch 1 of the opponent's Benched Pokémon with their Active Pokémon. Corrosive Acid: heads, Burned.</summary>
        private sealed class VictreebelE2 : Mon
        {
            private const string Power = "Fragrance Trap";
            private static readonly Mon Acid = new CardsPAR2.FlipConditionMon("Corrosive Acid", SpecialCondition.Burned);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip() && ctx.GustTargets().Count > 0) ctx.GustOpponent(Power + ": choose 1 of your opponent's Benched Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Acid.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Scent: heads, Poisoned and Confused; tails, Poisoned and Asleep.</summary>
        private sealed class PoisonScentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Scent") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.ApplyCondition(defending, heads ? SpecialCondition.Confused : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Addictive Pollen: heads, the opponent can't play Supporter cards during their next turn.</summary>
        private sealed class AddictivePollenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Addictive Pollen") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsPAR2.TrainerKindLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards next turn (Addictive Pollen)", ctx.Player, ctx.Opponent, ctx.Turn + 1, true));
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Extreme Speed (Poké-Body): [C] less Retreat Cost for each Energy attached. Fire Blow: discard any number of Fire Energy cards, a coin for each, 30 more per heads.</summary>
        private sealed class ArcanineE2 : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon ? cost - src.EnergyCount(mon) : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Blow") return false;
                var me = ctx.Pokemon;
                int discarded = 0;
                var fire = EnergyCardsOf(ctx, me, EnergyType.Fire);
                if (fire.Count > 0 && !CardsBS1.CopyingWithMetronome)
                {
                    int k = ctx.AskNumber(0, fire.Count, "Fire Blow: discard how many Fire Energy cards from " + me.Name + "? (a coin for each, 30 more damage per heads)");
                    if (k > 0)
                    {
                        var picks = k == fire.Count ? fire : ctx.ChooseCards(fire, k, k, "Fire Blow: discard " + k + " Fire Energy card" + (k == 1 ? "" : "s"));
                        discarded = DiscardCards(ctx, me, picks);
                    }
                }
                int heads = discarded > 0 ? ctx.FlipCoins(discarded) : 0;
                ctx.DamageDefending(atk.Damage + 30 * heads);
                return true;
            }
        }

        /// <summary>Pure Body (Poké-Body): attaching a basic Fire Energy card from your hand to it discards an Energy card from it. Burning Fang: you may flip; heads, discard a Fire Energy card and Burned.</summary>
        private sealed class EnteiE2 : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || energy == null || !IsBasicEnergyOf(energy.Def, EnergyType.Fire)) return;
                if (!src.InPlay(target) || target.Energy.Count == 0) return;
                src.Log("Pure Body: an Energy card attached to " + target.Name + " has to be discarded.");
                src.DiscardEnergyFrom(target, 1, "Pure Body: discard an Energy card attached to " + target.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Fang") return false;
                var defending = ctx.Defending;
                bool heads = ctx.AskYesNo("Burning Fang: flip a coin? (heads: discard a Fire Energy card and the Defending Pokémon is Burned)") && ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads) return true;
                if (!CardsBS1.CopyingWithMetronome) DiscardOneOf(ctx, ctx.Pokemon, EnergyType.Fire, "Burning Fang");
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Burn Up: tails, discard all Fire Energy cards attached to this Pokémon.</summary>
        private sealed class BurnUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burn Up") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!heads && ctx.InPlay(me) && !CardsBS1.CopyingWithMetronome) DiscardCards(ctx, me, EnergyCardsOf(ctx, me, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Miracle Tail: heads, the Defending Pokémon gets the Special Condition you choose.</summary>
        internal sealed class MiracleTailMon : Mon
        {
            private static readonly SpecialCondition[] Conditions = { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
            private readonly string attack;

            public MiracleTailMon() : this("Miracle Tail") { }

            /// <summary>v147: the same attack under another name (Skyridge Venomoth's Miracle Powder).</summary>
            public MiracleTailMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads || !TefPokemon.StillDefending(ctx, defending)) return true;
                int pick = ctx.ChooseOption(attack + ": choose a Special Condition for " + defending.Name, "Asleep", "Burned", "Confused", "Paralyzed", "Poisoned");
                ctx.ApplyCondition(defending, Conditions[Math.Max(0, Math.Min(Conditions.Length - 1, pick))]);
                return true;
            }
        }

        /// <summary>Roasting Heat: 20 more against a Burned Defending Pokémon.</summary>
        private sealed class RoastingHeatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roasting Heat") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Has(SpecialCondition.Burned) ? 20 : 0));
                return true;
            }
        }

        /// <summary>Gallop: heads, discard a Fire Energy card attached to this Pokémon and 20 more damage.</summary>
        private sealed class GallopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gallop") return false;
                bool heads = ctx.Flip();
                if (heads && !CardsBS1.CopyingWithMetronome) DiscardOneOf(ctx, ctx.Pokemon, EnergyType.Fire, "Gallop");
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Bubble Turn (Poké-Power): while this Pokémon is Benched, flip; heads, it and all cards attached go back to your hand. Aqua Sonic: no Resistance.</summary>
        private sealed class AzumarillE2 : Mon
        {
            private static readonly Mon AquaSonic = new CardsFLF1.NoResistanceMon("Aqua Sonic");

            protected override string Activated { get { return "Bubble Turn"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.Flip() && ctx.Me.Bench.Contains(me)) ctx.Engine.ReturnPokemonToHand(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return AquaSonic.HandleAttack(ctx, atk); }
        }

        /// <summary>Water Cyclone (Poké-Power): as often as you like, move a Water Energy card from your Active Pokémon to 1 of your Benched Pokémon. Rapids: heads, discard an Energy card from the Defending Pokémon.</summary>
        private sealed class KingdraE2 : Mon
        {
            private const string Power = "Water Cyclone";

            private static List<CardInstance> Water(EffectContext ctx) { return EnergyCardsOf(ctx, ctx.Me.Active, EnergyType.Water); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && Water(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var active = ctx.Me.Active;
                var water = Water(ctx);
                if (active == null || water.Count == 0 || ctx.Me.Bench.Count == 0) return;
                var card = water.Count == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, Power + ": move which Water Energy card from " + active.Name + "?")[0];
                var bench = ctx.MyBench();
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, Power + ": move " + card.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(active, target, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapids") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Rapids: discard which Energy card from " + defending.Name + "?");
                return true;
            }
        }

        /// <summary>Conduction: discard all Lightning Energy cards attached; a coin for each, 30 + 40 per heads.</summary>
        private sealed class ConductionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Conduction") return false;
                var me = ctx.Pokemon;
                int n = CardsBS1.CopyingWithMetronome ? 0 : DiscardCards(ctx, me, EnergyCardsOf(ctx, me, EnergyType.Lightning));
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                ctx.DamageDefending(atk.Damage + 40 * heads);
                return true;
            }
        }

        /// <summary>Suction Cups (Poké-Body): while this Pokémon is your Active Pokémon, when the opponent's Active Pokémon retreats, all Energy cards attached to it are discarded. Smokescreen.</summary>
        private sealed class OctilleryE2 : Mon
        {
            private static readonly Mon Smokescreen = new CardsUPR3.SmokescreenMon("Smokescreen", 1);

            public override void OnPokemonRetreated(EffectContext src, PokemonInPlay retreated)
            {
                var me = src.Pokemon;
                if (me == null || retreated == null || retreated.Owner == src.Player || !src.IsActive(me) || retreated.Energy.Count == 0) return;
                src.Log("Suction Cups: every Energy card attached to " + retreated.Name + " is discarded.");
                src.Engine.DiscardAllEnergy(retreated);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smokescreen.HandleAttack(ctx, atk); }
        }

        /// <summary>Dense Body (Poké-Body): 20 less damage from attacks of Basic Pokémon (not Baby Pokémon). Energy Cannon: 10 more per Energy not used for its cost (20 more at most).</summary>
        private sealed class SlowbroE2 : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                var a = info.Attacker.Def;
                return a.IsBasicPokemon && !a.IsBaby ? Math.Max(0, damage - 20) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Cannon") return false;
                int extra = ctx.Pokemon == null ? 0 : Math.Max(0, Math.Min(2, ctx.EnergyCount(ctx.Pokemon) - atk.Cost.Length));
                ctx.DamageDefending(atk.Damage + 10 * extra);
                return true;
            }
        }

        /// <summary>
        /// Strange Tentacles (Poké-Power): if the Defending Pokémon has fewer Energy cards than your Active Pokémon, attach an Energy card
        /// from the opponent's discard pile to the Defending Pokémon. Poison Sting: heads, Poisoned.
        /// </summary>
        private sealed class TentacruelE2 : Mon
        {
            private const string Power = "Strange Tentacles";
            private static readonly Mon Sting = new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var defending = ctx.Defending;
                var mine = ctx.Me.Active;
                return CardsE1.NoCondition(ctx.Pokemon) && defending != null && mine != null && defending.Energy.Count < mine.Energy.Count
                       && ctx.Opp.Discard.Any(c => c.Def.IsEnergy) && ctx.CanAffect(defending);
            }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Defending;
                var energy = ctx.Opp.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (defending == null || energy.Count == 0) return;
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, Power + ": attach which Energy card from your opponent's discard pile to " + defending.Name + "?")[0];
                if (ctx.Opp.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, defending);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sting.HandleAttack(ctx, atk); }
        }

        /// <summary>Aquabomb: this Pokémon does 10 damage to itself.</summary>
        private sealed class AquabombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aquabomb") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Dual Burn: 2 coins; for each tails, discard an Energy card attached to this Pokémon.</summary>
        private sealed class DualBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Burn") return false;
                int tails = 2 - ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (tails > 0 && ctx.InPlay(me) && me.Energy.Count > 0 && !CardsBS1.CopyingWithMetronome)
                    ctx.DiscardEnergyFrom(me, Math.Min(tails, me.Energy.Count), "Dual Burn: discard " + tails + " Energy card" + (tails == 1 ? "" : "s") + " from " + me.Name);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Super Dynamo (Poké-Power): while Active, flip; heads, attach a Lightning Energy card from your discard pile to 1 of your Pokémon. Swift.</summary>
        private sealed class ElectrodeE2 : Mon
        {
            private const string Power = "Super Dynamo";

            protected override string Activated { get { return Power; } }

            private static List<CardInstance> Lightning(EffectContext ctx) { return ctx.Me.Discard.Where(c => IsBasicEnergyOf(c.Def, EnergyType.Lightning)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && Lightning(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var cards = Lightning(ctx);
                if (cards.Count > 0) ctx.AttachCardsAnyWay(cards, 1, (card, m) => true, Power, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swift") return false;
                SwiftDamage(ctx, atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Ion Coating (Poké-Power): once during your turn, all Lightning Energy on your Active Pokémon is Water Energy for the rest of
        /// the turn (it ends if that Pokémon leaves the Active Spot). Electric Tackle: 20 + 10 per Lightning Energy; tails, 10 to
        /// itself per Lightning Energy.
        /// </summary>
        private sealed class LanturnE2 : Mon
        {
            private const string Power = "Ion Coating";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active != null) ctx.AddEffect(new IonCoatingEffect("Lightning Energy on " + active.Name + " is Water Energy this turn (Ion Coating)", ctx.Player, active.Uid, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electric Tackle") return false;
                int lightning = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Lightning);
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + 10 * lightning);
                if (!heads && lightning > 0 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10 * lightning);
                return true;
            }
        }

        private sealed class IonCoatingEffect : ContinuousEffect
        {
            public IonCoatingEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void ModifyProvidedEnergy(GameEngine engine, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder.Uid != TargetUid) return;
                int l = EnergyMask.Of(EnergyType.Lightning), w = EnergyMask.Of(EnergyType.Water);
                for (int i = 0; i < units.Count; i++)
                    if ((units[i] & l) != 0) units[i] = (units[i] & ~l) | w;
            }
        }

        /// <summary>
        /// Anti-Lightning (Poké-Body): you can't attach Lightning Energy cards from your hand to it. Plasma: with Lightning Energy cards
        /// in your discard pile, heads, attach 1. Lightning Storm: tails, 2 damage counters on itself.
        /// </summary>
        private sealed class ZapdosE2 : Mon
        {
            private static readonly Mon Plasma = new CardsE1.PlasmaMon();

            public override bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                return target != null && target == src.Pokemon && energy != null && IsBasicEnergyOf(energy.Def, EnergyType.Lightning);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lightning Storm")
                {
                    bool heads = ctx.Flip();
                    ctx.DamageDefending(atk.Damage);
                    if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, 2);
                    return true;
                }
                return Plasma.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Psychic

        /// <summary>Energy Return (Poké-Power): as often as you like, return an Energy card attached to 1 of your Pokémon to your hand. Damage Blast: a coin per damage counter on the Defending Pokémon, 30 + 10 per heads.</summary>
        private sealed class EspeonE2 : Mon
        {
            private const string Power = "Energy Return";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var holders = ctx.MyPokemon(m => m.Energy.Count > 0);
                if (holders.Count == 0) return;
                var from = holders.Count == 1 ? holders[0] : ctx.ChoosePokemon(holders, Power + ": return an Energy card from which of your Pokémon?");
                if (from == null) return;
                var card = from.Energy.Count == 1 ? from.Energy[0] : ctx.ChooseCards(from.Energy.ToList(), 1, 1, Power + ": return which Energy card to your hand?")[0];
                ctx.Engine.ReturnEnergyToHand(from, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Blast") return false;
                var defending = ctx.Defending;
                int coins = defending == null ? 0 : defending.DamageCounters;
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>Lateral Eggsplosion: a coin for each Energy attached to your Benched Pokémon, 30 + 10 per heads.</summary>
        private sealed class LateralEggsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lateral Eggsplosion") return false;
                int coins = ctx.MyBench().Sum(m => ctx.EnergyCount(m));
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>Sleep Pendulum (Poké-Power): while Active, the Defending Pokémon is now Asleep. Spiral Aura: if the Defending Pokémon isn't Knocked Out, you may switch in 1 of the opponent's Benched Pokémon.</summary>
        private sealed class HypnoE2 : Mon
        {
            protected override string Activated { get { return "Sleep Pendulum"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Defending != null && ctx.CanAffect(ctx.Defending);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Aura") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !ctx.InPlay(defending) || ctx.Opp.Active != defending || ctx.Engine.IsKnockedOut(defending)) return true;
                if (ctx.GustTargets().Count > 0 && ctx.AskYesNo("Spiral Aura: switch 1 of your opponent's Benched Pokémon with " + defending.Name + "?"))
                    ctx.GustOpponent("Spiral Aura: choose 1 of your opponent's Benched Pokémon to switch in");
                return true;
            }
        }

        /// <summary>Meditate: 10 more for each damage counter on the Defending Pokémon.</summary>
        private sealed class MeditateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Meditate") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 10 * (defending == null ? 0 : defending.DamageCounters));
                return true;
            }
        }

        /// <summary>Bursting Hand: look at the opponent's hand; 10 damage times the Energy cards there.</summary>
        private sealed class BurstingHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bursting Hand") return false;
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count > 0) ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Bursting Hand: your opponent's hand", ctx.Source);
                ctx.DamageDefending(atk.Damage * hand.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        /// <summary>Shuffle Attack: look at as many cards on top of the opponent's deck as there are Energy cards on the Defending Pokémon and put them back in any order.</summary>
        private sealed class ShuffleAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shuffle Attack") return false;
                var defending = ctx.Defending;
                int n = defending == null ? 0 : defending.Energy.Count;
                ctx.DamageDefending(atk.Damage);
                if (n > 0) ReorderTop(ctx, ctx.Opponent, n, "Shuffle Attack");
                return true;
            }
        }

        /// <summary>
        /// Miracle Shift (Poké-Power): discard a basic Energy card attached to 1 of your Pokémon, then attach a basic Energy card from
        /// your discard pile to that Pokémon. Mini-Metronome: heads, copy 1 of the Defending Pokémon's attacks without its Energy cost.
        /// </summary>
        private sealed class TogeticE2 : Mon
        {
            private const string Power = "Miracle Shift";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var holders = ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy));
                if (holders.Count == 0) return;
                var target = holders.Count == 1 ? holders[0] : ctx.ChoosePokemon(holders, Power + ": discard a basic Energy card from which of your Pokémon?");
                if (target == null) return;
                var basics = target.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var card = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, Power + ": discard which basic Energy card?")[0];
                if (!ctx.Engine.DiscardEnergy(target, card)) return;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count > 0 && ctx.InPlay(target)) ctx.AttachCardsAnyWay(pile, 1, (c, m) => m == target, Power + ": attach a basic Energy card from your discard pile", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mini-Metronome") return false;
                if (!ctx.Flip()) return true;
                var defending = ctx.Defending;
                CardDef source;
                int index;
                if (defending == null || !ChooseCopiedAttack(ctx, defending, "Mini-Metronome: choose 1 of the Defending Pokémon's attacks to copy", out source, out index)) return Nothing(ctx);
                CardsBS1.CopyingWithMetronome = true;
                try { UseCopiedAttack(ctx, source, index); }
                finally { CardsBS1.CopyingWithMetronome = false; }
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Earth Rage (Poké-Power): while Active, flip; heads, 1 damage counter on each of the opponent's Benched Pokémon. Giant Horn: heads, 30 more.</summary>
        private sealed class NidokingE2 : Mon
        {
            private static readonly Mon GiantHorn = new FlipBonusMon("Giant Horn", 30);

            protected override string Activated { get { return "Earth Rage"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                foreach (var mon in ctx.OpponentBench())
                    if (ctx.InPlay(mon) && ctx.CanAffect(mon)) ctx.PlaceCounters(mon, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return GiantHorn.HandleAttack(ctx, atk); }
        }

        /// <summary>Sudden Charge: 10 damage to itself, then the opponent switches the Defending Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class SuddenChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sudden Charge") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null && !ctx.Engine.IsKnockedOut(ctx.Opp.Active)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Copy: copy 1 of the Defending Pokémon's attacks; it does nothing unless this Pokémon has the Energy that attack needs.</summary>
        internal sealed class CopyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Copy") return false;
                var defending = ctx.Defending;
                CardDef source;
                int index;
                if (defending == null || !ChooseCopiedAttack(ctx, defending, "Copy: choose 1 of the Defending Pokémon's attacks", out source, out index)) return Nothing(ctx);
                if (!ctx.Engine.CanPayAttack(ctx.Pokemon, source.Attacks[index]))
                {
                    ctx.Log(ctx.Pokemon.Name + " doesn't have the Energy for " + source.Attacks[index].Name + ", so Copy did nothing.");
                    return true;
                }
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Fireworks: tails, discard a Fire Energy card attached to this Pokémon.</summary>
        internal sealed class FireworksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fireworks") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && !CardsBS1.CopyingWithMetronome) DiscardOneOf(ctx, ctx.Pokemon, EnergyType.Fire, "Fireworks");
                return true;
            }
        }

        /// <summary>Dark Impact: the Defending Pokémon can't use Poké-Powers until the end of the opponent's next turn.</summary>
        private sealed class DarkImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Impact") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new NoPokePowerEffect(defending.Name + " can't use Poké-Powers (Dark Impact)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoPokePowerEffect : ContinuousEffect
        {
            public NoPokePowerEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Destructive Roar: heads, discard an Energy card attached to 1 of the opponent's Pokémon.</summary>
        private sealed class DestructiveRoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Destructive Roar") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Destructive Roar: discard an Energy card from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Destructive Roar: discard which Energy card from " + target.Name + "?");
                return true;
            }
        }

        /// <summary>Gigacrush: each player discards the top 3 cards of their deck.</summary>
        private sealed class GigacrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigacrush") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                return true;
            }
        }

        /// <summary>
        /// Dark Moon (Poké-Power): while Active with Darkness Energy attached, look at the opponent's hand and shuffle up to 1 card for
        /// each Darkness Energy into their deck; they draw that many. Shadow Bind: the Defending Pokémon can't retreat.
        /// </summary>
        private sealed class UmbreonE2 : Mon
        {
            private const string Power = "Dark Moon";
            private static readonly Mon ShadowBind = new NoRetreatMon("Shadow Bind");

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Darkness) > 0 && ctx.Opp.Hand.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                int max = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Darkness);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0 || max <= 0) return;
                var picks = ctx.ChooseCards(hand, 0, Math.Min(max, hand.Count), Power + ": choose up to " + max + " card" + (max == 1 ? "" : "s") + " from your opponent's hand to shuffle into their deck");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Remove(card)) { ctx.Opp.Deck.Add(card); n++; }
                if (n == 0) return;
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(Power + ": " + n + " card" + (n == 1 ? "" : "s") + " from " + ctx.Engine.Possessive(ctx.Opponent) + " hand went into their deck.");
                ctx.Engine.DrawCards(ctx.Opponent, n);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ShadowBind.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>
        /// Magnetic Flow (Poké-Power): while Active, flip; heads, choose 2 of the opponent's Pokémon with Energy cards and swap 1 Energy
        /// card between them. Steel Wave: 10 damage to each of the opponent's Benched Pokémon of the Defending Pokémon's type.
        /// </summary>
        private sealed class MagnetonE2 : Mon
        {
            private const string Power = "Magnetic Flow";

            protected override string Activated { get { return Power; } }

            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m)); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && Holders(ctx).Count >= 2; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var holders = Holders(ctx);
                if (holders.Count < 2) return;
                var pair = holders.Count == 2 ? holders : ctx.ChoosePokemonMany(holders, 2, 2, Power + ": choose 2 of your opponent's Pokémon");
                if (pair.Count < 2) return;
                var a = pair[0];
                var b = pair[1];
                var ca = a.Energy.Count == 1 ? a.Energy[0] : ctx.ChooseCards(a.Energy.ToList(), 1, 1, Power + ": choose an Energy card on " + a.Name)[0];
                var cb = b.Energy.Count == 1 ? b.Energy[0] : ctx.ChooseCards(b.Energy.ToList(), 1, 1, Power + ": choose an Energy card on " + b.Name)[0];
                ctx.Engine.MoveEnergy(a, b, ca);
                ctx.Engine.MoveEnergy(b, a, cb);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Steel Wave") return false;
                var defending = ctx.Defending;
                var types = defending == null ? new List<EnergyType>() : ctx.Engine.TypesOf(defending);
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench())
                    if (ctx.InPlay(mon) && ctx.Engine.TypesOf(mon).Any(types.Contains)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Poison Resistance (Poké-Body): it can't be Poisoned. Snatch: before the damage, you may switch in 1 of the opponent's Benched
        /// Pokémon with no damage counters. Heavy Metal: a coin per Metal Energy, 30 + 20 per heads.
        /// </summary>
        private sealed class ScizorE2 : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target == src.Pokemon && condition == SpecialCondition.Poisoned;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Snatch")
                {
                    var clean = ctx.GustTargets().Where(m => m.DamageCounters == 0).ToList();
                    if (clean.Count > 0 && ctx.AskYesNo("Snatch: switch 1 of your opponent's undamaged Benched Pokémon with the Defending Pokémon first?"))
                    {
                        var target = clean.Count == 1 ? clean[0] : ctx.ChoosePokemon(clean, "Snatch: choose 1 of your opponent's Benched Pokémon with no damage counters");
                        if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
                    }
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name != "Heavy Metal") return false;
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 20 * heads);
                return true;
            }
        }

        /// <summary>Earth Rift: 10 damage to each of the opponent's Benched Pokémon; tails, it can't use Earth Rift during your next turn.</summary>
        private sealed class EarthRiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earth Rift") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Iron Smash: 2 heads, 70; 2 tails, nothing; otherwise 50.</summary>
        private sealed class IronSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Smash") return false;
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage + (heads == 2 ? 20 : 0));
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Happy Healing (Poké-Power): choose 1 of your Benched Pokémon and flip; heads, remove a damage counter from it for each Energy attached to Blissey. Smash Bomber: tails, nothing.</summary>
        private sealed class BlisseyE2 : Mon
        {
            private const string Power = "Happy Healing";
            private static readonly Mon SmashBomber = new TailsNothingMon("Smash Bomber");

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var bench = ctx.MyBench();
                if (bench.Count == 0) return;
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, Power + ": choose 1 of your Benched Pokémon");
                if (target == null || !ctx.Flip()) return;
                int n = ctx.EnergyCount(ctx.Pokemon);
                if (n > 0 && target.DamageCounters > 0) ctx.Heal(target, 10 * n);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SmashBomber.HandleAttack(ctx, atk); }
        }

        /// <summary>Backup (Poké-Power): with 2 or fewer cards in your hand, draw until you have 3. Hypnotic Ray: Asleep.</summary>
        private sealed class Porygon2E2 : Mon
        {
            private static readonly Mon Ray = new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep);

            protected override string Activated { get { return "Backup"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Count <= 2 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(3); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Crystal Type (Poké-Body): attaching a basic Energy card of one of 3 types from your hand to it makes it that type until the
        /// end of the turn (like 30C-CC7 Lugia).
        /// </summary>
        internal sealed class CrystalTypeMon : Mon
        {
            private const string Mark = "Crystal Type";
            private const string TypeMark = "Crystal Type (type)";
            private readonly EnergyType[] types;
            private readonly Mon attacks;

            public CrystalTypeMon(EnergyType[] types, Mon attacks)
            {
                this.types = types;
                this.attacks = attacks;
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || energy == null || !energy.Def.IsBasicEnergy) return;
                var t = energy.Def.EnergyProvides;
                if (!types.Contains(t)) return;
                src.Pokemon.AbilityUsedOnTurn[Mark] = src.Turn;
                src.Pokemon.AbilityUsedOnTurn[TypeMark] = (int)t;
                src.Log("Crystal Type: " + src.Pokemon.Name + " is " + t + " type until the end of the turn.");
            }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> list)
            {
                if (mon != src.Pokemon) return;
                int turn, type;
                if (!mon.AbilityUsedOnTurn.TryGetValue(Mark, out turn) || turn != src.Turn) return;
                if (!mon.AbilityUsedOnTurn.TryGetValue(TypeMark, out type)) return;
                list.Clear();
                list.Add((EnergyType)type);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Rolling Thunder: heads, 10 damage to each of the opponent's Benched Pokémon; tails, 10 to each of your own.</summary>
        private sealed class RollingThunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rolling Thunder") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in heads ? ctx.OpponentBench() : ctx.MyBench())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }
    }
}
