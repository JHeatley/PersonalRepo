using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// XY Black Star Promos (XYP, v107), part 3: XY126-XY211 and the Trainers (Trevor, Karen, Gym Badge and the Spirit Links).
    /// </summary>
    public static class CardsXYP3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("XYP-126", new MewVersatile());
            r.RegisterByKey("XYP-133", new ComboMon(new DancingShurikenMon(), new LockAfterAttackMon("Ninja Blade", true)));
            r.RegisterByKey("XYP-134", new CardsASR1.TimesMon("Emperor's Command", c => c.Opp.InPlay().Count()));
            r.RegisterByKey("XYP-135", new CosmicCircleMon());
            r.RegisterByKey("XYP-136", new NightScanMon());
            r.RegisterByKey("XYP-137", new ComboMon(new CardsCES1.ConditionsMon("Dizzying Poison", SpecialCondition.Confused, SpecialCondition.Poisoned),
                new CardsASR1.PlusMon("Powder", 30, c => c.Defending == null ? 0 : c.Engine.EnergyCountOfType(c.Defending, EnergyType.Fire))));
            r.RegisterByKey("XYP-140", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Bone Rush"), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("XYP-141", new ComboMon(new CoinsMon("Dual Claw", 2), new FireLightningBlastMon()));
            r.RegisterByKey("XYP-142", new ComboMon(new CardsBKT2.PainAmplifierMon("Shining Eyes", 2), new ConditionAttackMon("Mind Bend", SpecialCondition.Confused)));
            r.RegisterByKey("XYP-148", new ShayminGratitude());
            r.RegisterByKey("XYP-153", new HoOhStokeMon());
            r.RegisterByKey("XYP-154", new LockAfterAttackMon("Shining Flame", true));
            r.RegisterByKey("XYP-155", new RightBackAtYouMon());
            r.RegisterByKey("XYP-156", new CardsWHT.FlipMoreMon("Aeroblast", 2, 20));
            r.RegisterByKey("XYP-157", new ComboMon(new DoubleScrapperMon(), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("XYP-158", new HazardStingerMon());
            r.RegisterByKey("XYP-160", new FrostyThunderMon());
            r.RegisterByKey("XYP-162", new ComboMon(new CardsCEC3.SpreadAmountMon("Aqua Shower", 20), new CoinsMon("Dual Cut", 2)));
            r.RegisterByKey("XYP-163", new WeezingLevitate());
            r.RegisterByKey("XYP-164", new ComboMon(new CardsPOR.FlipPerTypedEnergyMon("Concentrated Fire", EnergyType.Fire), new CardsASC.PushDownMon("Combustion Impact")));
            r.RegisterByKey("XYP-165", new ComboMon(new EntertainMon(), new PrismaticWaveMon()));
            r.RegisterByKey("XYP-166", new CardsCES1.ConditionsMon("Hollow Geist", SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("XYP-167", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
            r.RegisterByKey("XYP-168", new CrimsonEdgeMon());
            r.RegisterByKey("XYP-170", new ComboMon(new CardsASR1.PlusMon("Beastly Fang", 50, c => c.Opp.InPlay().Count(m => m.Def.IsPokemonEX)), new LockAfterAttackMon("Dragon Strike", true)));
            r.RegisterByKey("XYP-171", new SavageWingMon());
            r.RegisterByKey("XYP-174", new ComboMon(new ParalyzeFlipMon("Thunder Shock"), new CardsTWM1.SelfDiscardMon("Mega Thunderbolt", 99)));
            r.RegisterByKey("XYP-178", new AbsolLamentation());
            r.RegisterByKey("XYP-179", new SnorlaxImmunity());
            r.RegisterByKey("XYP-180", new TurboFlameMon());
            r.RegisterByKey("XYP-181", new SilentBiteMon());
            r.RegisterByKey("XYP-182", new WingsOfDisasterMon());
            r.RegisterByKey("XYP-184", new GiratinaDevour());
            r.RegisterByKey("XYP-198", new MagmaEruptionMon());
            r.RegisterByKey("XYP-200", new TorpedoDiveMon());
            r.RegisterByKey("XYP-202", new ComboMon(new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));

            r.RegisterByName("Trevor", new Trevor());
            r.RegisterByName("Karen", new Karen());
            r.RegisterByName("Gym Badge", new GymBadge());
            r.RegisterByName("Aerodactyl Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Mawile Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Beedrill Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Garchomp Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Salamence Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Camerupt Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Sharpedo Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Lucario Spirit Link", new CardsEVO3.SpiritLink());
        }

        // ================================================================== Pokémon

        /// <summary>Versatile: this Pokémon can use the attacks of any Pokémon in play (both sides). Replace: move Energy among your Pokémon.</summary>
        private sealed class MewVersatile : Mon
        {
            private static readonly Mon Move = new CardsPRC2.ReplaceMon();

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                var seen = new HashSet<string>();
                for (int side = 0; side < 2; side++)
                {
                    foreach (var other in src.State.Players[side].InPlay())
                    {
                        if (other == src.Pokemon) continue;
                        var defs = new List<CardDef> { other.Def };
                        if ((other.Def.IsLevelUp || other.Def.IsBreak) && other.Stack.Count >= 2) defs.Add(other.Stack[other.Stack.Count - 2].Def);
                        foreach (var def in defs)
                        {
                            if (!seen.Add(def.BaseKey)) continue;
                            for (int i = 0; i < def.Attacks.Count; i++) list.Add(new KeyValuePair<CardDef, int>(def, i));
                        }
                    }
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Move.HandleAttack(ctx, atk); }
        }

        /// <summary>Dancing Shuriken: flip 3 coins; 20 damage for each heads to 1 of the opponent's Pokémon.</summary>
        private sealed class DancingShurikenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dancing Shuriken") return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.DamageAnyOpponent(20 * heads);
                return true;
            }
        }

        /// <summary>Cosmic Circle: move any number of Psychic Energy among your Pokémon in any way.</summary>
        private sealed class CosmicCircleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cosmic Circle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                Func<PokemonInPlay, List<CardInstance>> psychic = m => m.Energy.Where(en => CardsDRI1.ProvidesType(ctx, m, en, EnergyType.Psychic)).ToList();
                RepeatableAbility.Repeat(ctx, () => ctx.MyPokemon().Count >= 2 && ctx.MyPokemon().Any(m => psychic(m).Count > 0), offerDone =>
                {
                    var givers = ctx.MyPokemon().Where(m => psychic(m).Count > 0).ToList();
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, givers, "Cosmic Circle: move a Psychic Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var cards = psychic(giver);
                    var card = ctx.ChooseCards(cards, 1, 1, "Cosmic Circle: move which Energy?")[0];
                    var others = ctx.MyPokemon().Where(m => m != giver).ToList();
                    var receiver = ctx.ChoosePokemon(others, "Cosmic Circle: move " + card.Def.Name + " to which Pokémon?");
                    return receiver != null && ctx.Engine.MoveEnergy(giver, receiver, card);
                });
                return true;
            }
        }

        /// <summary>Night Scan: the opponent reveals their hand; 30 more damage for each Trainer card there.</summary>
        private sealed class NightScanMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Scan") return false;
                TefPokemon.RevealOpponentHand(ctx);
                ctx.DamageDefending(atk.Damage + 30 * ctx.Opp.Hand.Count(c => c.Def.IsTrainer));
                return true;
            }
        }

        /// <summary>Dragon Blast (Rayquaza): discard a Fire Energy and a Lightning Energy attached to this Pokémon.</summary>
        private sealed class FireLightningBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Blast") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.DiscardEnergyFrom(me, 1, "Dragon Blast: discard a Fire Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fire));
                ctx.DiscardEnergyFrom(me, 1, "Dragon Blast: discard a Lightning Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Lightning));
                return true;
            }
        }

        /// <summary>Aroma of Gratitude: once during your turn, heal 20 damage from each of your Benched Basic Pokémon. Floral Gain: heal 20 and cure.</summary>
        private sealed class ShayminGratitude : Mon
        {
            protected override string Activated { get { return "Aroma of Gratitude"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyBench().Any(m => m.Def.IsBasicPokemon && m.DamageCounters > 0); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyBench().Where(m => m.Def.IsBasicPokemon && m.DamageCounters > 0).ToList()) ctx.Heal(mon, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Floral Gain") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Heal(me, 20);
                ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }

        /// <summary>Stoke (Ho-Oh): flip a coin; heads, search your deck for up to 2 Fire Energy cards and attach them to this Pokémon.</summary>
        private sealed class HoOhStokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stoke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0 || !ctx.InPlay(me)) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 2, "Stoke: choose up to 2 Fire Energy cards");
                foreach (var en in found) ctx.Engine.AttachEnergyCard(en, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Effect: damage done to this Pokémon by the opponent's attack is put back on the attacker as damage counters (Right Back at You).</summary>
        private sealed class RightBackEffect : ContinuousEffect
        {
            public RightBackEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage < 10) return;
                engine.PlaceDamageCounters(info.Attacker, damage / 10, "Right Back at You");
            }
        }

        /// <summary>Right Back at You: discard all Energy from this Pokémon; during the opponent's next turn, damage done to it goes back on the attacker as counters.</summary>
        private sealed class RightBackAtYouMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Right Back at You") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, me.Energy.Count, atk.Name);
                ctx.AddEffect(new RightBackEffect(me.Name + ": damage done to it during the next turn goes back on the attacker (Right Back at You)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Double Scrapper: discard up to 2 Pokémon Tools attached to the opponent's Pokémon.</summary>
        private sealed class DoubleScrapperMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Scrapper") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 2; i++)
                {
                    var holders = ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m));
                    if (holders.Count == 0) break;
                    var tools = holders.SelectMany(m => m.Tools).ToList();
                    var pick = ctx.ChooseCards(tools, 0, 1, "Double Scrapper: discard a Pokémon Tool from your opponent's Pokémon (" + (i + 1) + " of up to 2)");
                    if (pick.Count == 0) break;
                    var holder = holders.First(m => m.Tools.Contains(pick[0]));
                    ctx.Engine.DiscardTool(holder, pick[0]);
                }
                return true;
            }
        }

        /// <summary>Hazard Stinger: discard all Energy from this Pokémon; the opponent's Active Pokémon is Paralyzed and Poisoned with 4 counters between turns.</summary>
        private sealed class HazardStingerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hazard Stinger") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, me.Energy.Count, atk.Name);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 4;
                return true;
            }
        }

        /// <summary>Frosty Thunder: with any Lightning Energy attached, 20 damage to each of the opponent's Benched Pokémon too.</summary>
        private sealed class FrostyThunderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frosty Thunder") return false;
                var me = ctx.Pokemon;
                bool lightning = me.Energy.Any(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Lightning));
                ctx.DamageDefending(atk.Damage);
                if (lightning) foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Levitate: with any Energy attached, no Retreat Cost. Smokescreen.</summary>
        private sealed class WeezingLevitate : Mon
        {
            private static readonly Mon Smoke = new CardsMEG.SandAttackMon("Smokescreen");

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && mon.Energy.Count > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smoke.HandleAttack(ctx, atk); }
        }

        /// <summary>Entertain: heal 40 damage from 1 of your Benched Pokémon.</summary>
        private sealed class EntertainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entertain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Entertain: heal 40 damage from which Benched Pokémon?");
                if (target != null) ctx.Heal(target, 40);
                return true;
            }
        }

        /// <summary>Prismatic Wave: 20 damage for each type of Pokémon on the opponent's Bench.</summary>
        private sealed class PrismaticWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Prismatic Wave") return false;
                var types = new HashSet<EnergyType>();
                foreach (var mon in ctx.OpponentBench()) foreach (var t in ctx.Engine.TypesOf(mon)) types.Add(t);
                ctx.DamageDefending(atk.Damage * types.Count);
                return true;
            }
        }

        /// <summary>Crimson Edge: this Pokémon does 10 damage to itself for each damage counter on it.</summary>
        private sealed class CrimsonEdgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crimson Edge") return false;
                int counters = ctx.Pokemon.DamageCounters;
                ctx.DamageDefending(atk.Damage);
                if (counters > 0 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10 * counters);
                return true;
            }
        }

        /// <summary>Savage Wing: discard any number of basic Fire Energy from this Pokémon; 40 more for each one.</summary>
        private sealed class SavageWingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Savage Wing") return false;
                var me = ctx.Pokemon;
                var fire = me.Energy.Where(en => CardsSSP1.IsBasicOf(en.Def, EnergyType.Fire)).ToList();
                int n = 0;
                if (fire.Count > 0)
                {
                    var picks = ctx.ChooseCards(fire, 0, fire.Count, "Savage Wing: discard any number of basic Fire Energy from " + me.Name + " (40 more damage each)");
                    foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) n++;
                }
                ctx.DamageDefending(atk.Damage + 40 * n);
                return true;
            }
        }

        /// <summary>Lamentation: damaged by the opponent's attack in the Active Spot, discard a random card from the opponent's hand. Shadow Cage.</summary>
        private sealed class AbsolLamentation : Mon
        {
            private static readonly Mon Cage = new NoRetreatMon("Shadow Cage");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || ctx.Opp.Hand.Count == 0) return;
                Announce(ctx, "Lamentation");
                CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cage.HandleAttack(ctx, atk); }
        }

        /// <summary>Immunity: this Pokémon can't be affected by Special Conditions. Body Slam.</summary>
        private sealed class SnorlaxImmunity : Mon
        {
            private static readonly Mon Slam = new ParalyzeFlipMon("Body Slam");

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slam.HandleAttack(ctx, atk); }
        }

        /// <summary>Turbo Flame: attach 2 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class TurboFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Flame") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Turbo Flame: attach 2 basic Energy cards from your discard pile to which Benched Pokémon?");
                if (target == null) return true;
                int n = Math.Min(2, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Turbo Flame: choose 2 basic Energy cards");
                foreach (var card in picks) if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Silent Bite: you may Paralyze the opponent's Active Pokémon; if you do, shuffle this Pokémon and its cards into your deck.</summary>
        private sealed class SilentBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silent Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.AskYesNo("Silent Bite: Paralyze " + defending.Name + " and shuffle " + ctx.Pokemon.Name + " and all its cards into your deck?")) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Wings of Disaster: 20 damage to each of the opponent's Pokémon (no Weakness or Resistance); discard all their Pokémon Tools.</summary>
        private sealed class WingsOfDisasterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wings of Disaster") return false;
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageWith(mon, 20, false, false, false);
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0))
                {
                    if (!ctx.CanAffect(mon)) continue;
                    foreach (var tool in mon.Tools.ToList()) ctx.Engine.DiscardTool(mon, tool);
                }
                return true;
            }
        }

        /// <summary>Devour Light: Pokémon BREAK (both players') have no Abilities, the ones from their previous Evolution included. Shadow Claw.</summary>
        private sealed class GiratinaDevour : Mon
        {
            private static readonly Mon Claw = new CardsASC.RandomDiscardMon("Shadow Claw");

            public override bool BlocksAbilitiesOf(EffectContext ctx, PokemonInPlay mon)
            {
                return ctx.Pokemon != null && mon != null && mon.Def.IsBreak;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claw.HandleAttack(ctx, atk); }
        }

        /// <summary>Magma Eruption: you may discard the top 3 cards of each player's deck; 40 more damage for each Energy card discarded.</summary>
        private sealed class MagmaEruptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Eruption") return false;
                int energy = 0;
                if (ctx.AskYesNo("Magma Eruption: discard the top 3 cards of each player's deck (40 more damage for each Energy card)?"))
                {
                    energy += ctx.Engine.DiscardTopOfDeck(ctx.Player, 3).Count(c => c.Def.IsEnergy);
                    energy += ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3).Count(c => c.Def.IsEnergy);
                }
                ctx.DamageDefending(atk.Damage + 40 * energy);
                return true;
            }
        }

        /// <summary>Torpedo Dive: 10 damage to each of the opponent's Benched Pokémon for each [C] in its Retreat Cost.</summary>
        private sealed class TorpedoDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Torpedo Dive") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench())
                {
                    int cost = ctx.Engine.RetreatCostOf(mon);
                    if (cost > 0) ctx.DamageTarget(mon, 10 * cost);
                }
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Trevor: search your deck for a Pokémon and put it into your hand.</summary>
        private sealed class Trevor : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon, 1, "Trevor: choose a Pokémon");
            }
        }

        /// <summary>Karen: each player shuffles all Pokémon in their discard pile into their deck.</summary>
        private sealed class Karen : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                for (int i = 0; i < 2; i++)
                {
                    var p = ctx.State.Players[i];
                    var mons = p.Discard.Where(c => c.Def.IsPokemon).ToList();
                    foreach (var card in mons)
                    {
                        p.Discard.Remove(card);
                        p.Deck.Add(card);
                    }
                    ctx.Engine.ShuffleDeck(i);
                    if (mons.Count > 0) ctx.Log(mons.Count + " Pokémon from " + ctx.Engine.PlayerName(i) + "'s discard pile were shuffled into their deck (Karen).");
                }
            }
        }

        /// <summary>Gym Badge: flip a coin until you get tails; draw a card for each heads.</summary>
        private sealed class GymBadge : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Draw(heads);
            }
        }
    }
}
