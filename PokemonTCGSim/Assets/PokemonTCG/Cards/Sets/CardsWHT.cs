using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// White Flare (WHT): the cards that weren't already in the pool. Pokémon whose attacks only do their printed damage
    /// need no script: Sewaddle, Deerling, Tepig, Pignite, Simisear, Oshawott, Blitzle, Woobat, Gothita, Mienfoo,
    /// Scraggy, Trubbish, Vullaby, Ferroseed.
    /// </summary>
    public static class CardsWHT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("WHT-2", new SwadloonWHT());
            r.RegisterByKey("WHT-3", new LeavannyWHT());
            r.RegisterByKey("WHT-4", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("WHT-5", new WhimsicottExWHT());
            r.RegisterByKey("WHT-7", new CardsDRI1.WhirlwindMon("Push Down"));
            r.RegisterByKey("WHT-8", new StimulatedEvolutionMon("Karrablast"));
            r.RegisterByKey("WHT-9", new AccelgorWHT());
            r.RegisterByKey("WHT-10", new VirizionWHT());
            r.RegisterByKey("WHT-13", new EmboarWHT());
            r.RegisterByKey("WHT-14", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("WHT-16", new LitwickWHT());
            r.RegisterByKey("WHT-17", new CardsTWM1.SelfDiscardMon("Fire Blast", 1));
            r.RegisterByKey("WHT-18", new ChandelureWHT());
            r.RegisterByKey("WHT-19", new SearchAttackMon("Licking Catch", d => (d.IsPokemon && d.PokemonType == EnergyType.Fire) || CardsSSP1.IsBasicOf(d, EnergyType.Fire), 3));
            r.RegisterByKey("WHT-20", new BurstExMon("Blazing Burst", 0));
            r.RegisterByKey("WHT-22", new DewottWHT());
            r.RegisterByKey("WHT-23", new SamurottWHT());
            r.RegisterByKey("WHT-24", new CardsNXD.BaredFangsMon("Bared Fangs"));
            r.RegisterByKey("WHT-25", new DucklettWHT());
            r.RegisterByKey("WHT-26", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("WHT-27", new FlipBonusMon("Ice Edge", 20));
            r.RegisterByKey("WHT-28", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("WHT-29", new VanilluxeWHT());
            r.RegisterByKey("WHT-30", new KeldeoExWHT());
            r.RegisterByKey("WHT-32", new Cards30CC.BenchHitMon("Electrobullet", 30));
            r.RegisterByKey("WHT-33", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("WHT-34", new GalvantulaWHT());
            r.RegisterByKey("WHT-35", new StunfiskWHT());
            r.RegisterByKey("WHT-37", new SwoobatWHT());
            r.RegisterByKey("WHT-38", new SigilyphWHT());
            r.RegisterByKey("WHT-39", new FlipBonusMon("Focused Wish", 20));
            r.RegisterByKey("WHT-40", new CofagrigusWHT());
            r.RegisterByKey("WHT-42", new GothoritaWHT());
            r.RegisterByKey("WHT-43", new GothitelleWHT());
            r.RegisterByKey("WHT-46", new RoggenrolaWHT());
            r.RegisterByKey("WHT-47", new BoldoreWHT());
            r.RegisterByKey("WHT-48", new GigalithWHT());
            r.RegisterByKey("WHT-49", new SawkWHT());
            r.RegisterByKey("WHT-50", new FlipMoreMon("Acrobatics", 2, 30));
            r.RegisterByKey("WHT-51", new ArcheopsWHT());
            r.RegisterByKey("WHT-53", new CardsFLF1.NoResistanceMon("Smash Uppercut"));
            r.RegisterByKey("WHT-54", new CardsSTS2.RetaliateMon("Retaliate", 80));
            r.RegisterByKey("WHT-55", new SearchAttackMon("Invite Evil", d => d.IsPokemon && d.PokemonType == EnergyType.Darkness, 3));
            r.RegisterByKey("WHT-56", new CardsSCR2.KnockOffMon());
            r.RegisterByKey("WHT-58", new ScraftyWHT());
            r.RegisterByKey("WHT-60", new ConditionAttackMon("Gunk Shot", SpecialCondition.Poisoned));
            r.RegisterByKey("WHT-61", new RecoilMon("Take Down", 10));
            r.RegisterByKey("WHT-62", new ZoroarkWHT());
            r.RegisterByKey("WHT-64", new MandibuzzWHT());
            r.RegisterByKey("WHT-65", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("WHT-66", new CoinsMon("Double Hit", 2));
            r.RegisterByKey("WHT-67", new HydreigonExWHT());
            r.RegisterByKey("WHT-69", new FerrothornWHT());
            r.RegisterByKey("WHT-70", new DurantWHT());
            r.RegisterByKey("WHT-71", new DruddigonWHT());
            r.RegisterByKey("WHT-72", new SearchAttackMon("Procurement", d => d.IsItem, 1));
            r.RegisterByKey("WHT-73", new WatchogWHT());
            r.RegisterByKey("WHT-74", new FlipBonusMon("Play Rough", 20));
            r.RegisterByKey("WHT-75", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("WHT-76", new StoutlandWHT());
            r.RegisterByKey("WHT-77", new BouffalantExWHT());
            r.RegisterByKey("WHT-78", new TornadusWHT());

            r.RegisterByName("Antique Plume Fossil", new AntiquePlumeFossil());
            r.RegisterByName("Cheren", new Cheren());
            r.RegisterByName("Harlequin", new Harlequin());
        }

        // ================================================================== Shared helpers (also used by CardsBLK)

        /// <summary>Stimulated Evolution (Shelmet, Karrablast): with the partner in play, it can evolve on your first turn or the turn it's played.</summary>
        public sealed class StimulatedEvolutionMon : Mon
        {
            private readonly string partner;

            public StimulatedEvolutionMon(string partner) { this.partner = partner; }

            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                return src.Pokemon != null && target == src.Pokemon && src.MyPokemon(m => m.Name == partner).Count > 0;
            }
        }

        /// <summary>"Flip N coins. This attack does X more damage for each heads."</summary>
        public sealed class FlipMoreMon : Mon
        {
            private readonly string attack;
            private readonly int coins;
            private readonly int bonus;

            public FlipMoreMon(string attack, int coins, int bonus)
            {
                this.attack = attack;
                this.coins = coins;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + bonus * ctx.FlipCoins(coins));
                return true;
            }
        }

        /// <summary>
        /// Reshiram ex / Zekrom ex: Slash, and a Burst attack that does 50 more for each Prize card the opponent has
        /// taken. Blazing Burst discards an Energy from this Pokémon; Voltage Burst does recoil damage.
        /// </summary>
        public sealed class BurstExMon : Mon
        {
            private readonly string attack;
            private readonly int recoil;

            public BurstExMon(string attack, int recoil)
            {
                this.attack = attack;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + 50 * ctx.OpponentPrizesTaken);
                if (recoil > 0) ctx.DamageSelf(recoil);
                else CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>"Switch in 1 of your opponent's Benched Pokémon to the Active Spot. This attack does N damage to the new Active Pokémon."</summary>
        public sealed class DragOffMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public DragOffMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                // Compendium (Drag Off, Magnetic Pull): the damage is to "the new Active Pokémon", so no switch, no damage.
                if (ctx.GustOpponent(attack + ": choose 1 of your opponent's Benched Pokémon to switch in") != null)
                    ctx.DamageDefending(amount);
                return true;
            }
        }

        /// <summary>Harden: during the opponent's next turn, prevent damage from attacks to this Pokémon if it's the threshold or less.</summary>
        public sealed class SmallDamageShieldEffect : ContinuousEffect
        {
            private readonly int threshold;

            public SmallDamageShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn, int threshold)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.threshold = threshold;
            }

            // Checked against the final amount, after every other reduction.
            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && damage <= threshold;
            }
        }

        // ================================================================== Grass

        /// <summary>Healing Leaves: once during your turn, heal 20 damage from your Active Pokémon.</summary>
        private sealed class SwadloonWHT : Mon
        {
            protected override string Activated { get { return "Healing Leaves"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && a.DamageCounters > 0 && !ctx.Engine.HealingBlocked(a);
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Me.Active, 20); }
        }

        /// <summary>Healing Wrapping: heal 100 from each of your Basic Pokémon. X-Scissor: flip, +40.</summary>
        private sealed class LeavannyWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Healing Wrapping")
                {
                    foreach (var m in ctx.MyPokemon(p => p.Def.IsBasicPokemon)) ctx.Heal(m, 100);
                    return true;
                }
                if (atk.Name == "X-Scissor")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 40 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Energy Gift: attach up to 3 Basic Energy from your deck to your Pokémon in any way. Wondrous Cotton: 50 for each Trainer in the opponent's hand.</summary>
        private sealed class WhimsicottExWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Gift")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Energy Gift: choose up to 3 Basic Energy cards to attach");
                    ctx.AttachCardsAnyWay(found, found.Count, (c, m) => true, "Energy Gift", false);
                    foreach (var c in found)
                        if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c))) ctx.Me.Deck.Add(c);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Wondrous Cotton")
                {
                    CardsTWM1.RevealOpponentHand(ctx);
                    ctx.DamageDefending(atk.Damage * ctx.Opp.Hand.Count(c => c.Def.IsTrainer));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Acid Spray: flip; heads discards an Energy from the opponent's Active Pokémon.</summary>
        private sealed class AccelgorWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Acid Spray") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Acid Spray: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Giga Drain: heal the damage done. Emerald Blade: can't attack next turn.</summary>
        private sealed class VirizionWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Giga Drain")
                {
                    int done = ctx.DamageDefending(atk.Damage);
                    if (done > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, done);
                    return true;
                }
                if (atk.Name == "Emerald Blade")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>Inferno Fandango: as often as you like during your turn, attach a Basic Fire Energy from your hand to 1 of your Pokémon.</summary>
        internal sealed class EmboarWHT : Mon
        {
            private const string Fandango = "Inferno Fandango";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Fandango || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)) && ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing attaches as many as they like, then presses Done; bots attach one per use.
                if (RepeatableAbility.ForPerson(ctx))
                {
                    RepeatableAbility.AttachFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), () => ctx.MyPokemon(), Fandango, "a Basic Fire Energy");
                    return;
                }
                var fire = ctx.Me.Hand.FirstOrDefault(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
                if (fire == null) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)), "Inferno Fandango: attach a Basic Fire Energy to which Pokémon?");
                if (target == null) return;
                ctx.Me.Hand.Remove(fire);
                ctx.Engine.AttachEnergyCard(fire, target, true);
            }
        }

        /// <summary>Brighten and Burn: look at the top card of your deck; you may discard it.</summary>
        private sealed class LitwickWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brighten and Burn") return false;
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return true;
                if (ctx.AskYesNo("Brighten and Burn: the top card of your deck is " + top[0].Def.Name + ". Discard it?"))
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                return true;
            }
        }

        /// <summary>Incendiary Pillar: +100 with 10 or more Basic Fire Energy in your discard pile. Burn It All Up: discard all Energy from it.</summary>
        private sealed class ChandelureWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Incendiary Pillar")
                {
                    bool pillar = ctx.Me.Discard.Count(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)) >= 10;
                    ctx.DamageDefending(atk.Damage + (pillar ? 100 : 0));
                    return true;
                }
                if (atk.Name == "Burn It All Up")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        /// <summary>Energized Shell: 30 for each Energy attached to this Pokémon.</summary>
        private sealed class DewottWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energized Shell") return false;
                ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
                return true;
            }
        }

        /// <summary>Torrential Whirlpool: once during your turn, switch your Active Pokémon with a Benched one; if you do, the opponent switches out their Active. Energized Slash: +50 per Energy on it.</summary>
        private sealed class SamurottWHT : Mon
        {
            protected override string Activated { get { return "Torrential Whirlpool"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.SwitchMyActive("Torrential Whirlpool: choose a Benched Pokémon to switch with your Active Pokémon")) return;
                ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energized Slash") return false;
                ctx.DamageDefending(atk.Damage + 50 * ctx.EnergyCount(ctx.Pokemon));
                return true;
            }
        }

        /// <summary>Firefighting: discard a Fire Energy from the opponent's Active Pokémon.</summary>
        private sealed class DucklettWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Firefighting") return false;
                var d = ctx.Defending;
                if (d == null) return true;
                ctx.DiscardEnergyFrom(d, 1, "Firefighting: discard a Fire Energy from " + d.Name, e => CardsDRI1.ProvidesType(ctx, d, e, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Double Freeze: 2 coins, 90 per heads; any heads Paralyzes.</summary>
        private sealed class VanilluxeWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Freeze") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Gale Thrust: +90 if it moved from the Bench to the Active Spot this turn. Sonic Edge: ignores effects on the Defending Pokémon.</summary>
        private sealed class KeldeoExWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gale Thrust")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn ? 90 : 0));
                    return true;
                }
                if (atk.Name == "Sonic Edge")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Lightning

        /// <summary>Discharge: discard all Lightning Energy from it, 50 for each.</summary>
        private sealed class GalvantulaWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Discharge") return false;
                var me = ctx.Pokemon;
                int n = 0;
                foreach (var e in me.Energy.Where(x => CardsDRI1.ProvidesType(ctx, me, x, EnergyType.Lightning)).ToList())
                    if (ctx.Engine.DiscardEnergy(me, e)) n++;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Muddy Bolt: +20 with any Fighting Energy attached.</summary>
        private sealed class StunfiskWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Muddy Bolt") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Engine.HasEnergyOfType(ctx.Pokemon, EnergyType.Fighting) ? 20 : 0));
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Happy Return: put 1 of your Benched Pokémon and its cards into your hand.</summary>
        private sealed class SwoobatWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Happy Return") return false;
                if (ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Happy Return: put which Benched Pokémon into your hand?");
                if (target != null) ctx.Engine.ReturnPokemonToHand(target);
                return true;
            }
        }

        /// <summary>Reflect: takes 40 less next turn. Telekinesis: 70 to 1 of the opponent's Pokémon, no Weakness or Resistance.</summary>
        private sealed class SigilyphWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Reflect")
                {
                    ctx.SelfTakesLessNextTurn(40);
                    return true;
                }
                if (atk.Name == "Telekinesis")
                {
                    ctx.DamageAnyOpponent(70, true);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Extended Damagriiigus: move all damage counters from 1 of your Benched Pokémon to 1 of the opponent's Pokémon. Perplex: Confused.</summary>
        private sealed class CofagrigusWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Extended Damagriiigus")
                {
                    var hurt = ctx.Me.Bench.Where(m => m.DamageCounters > 0).ToList();
                    if (hurt.Count == 0 || !ctx.Opp.HasPokemonInPlay) return true;
                    var from = ctx.ChoosePokemon(hurt, "Extended Damagriiigus: move the damage counters from which Benched Pokémon?");
                    var to = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Extended Damagriiigus: move them to which of your opponent's Pokémon?");
                    if (from != null && to != null) ctx.Engine.MoveDamageCounters(from, to, from.DamageCounters, atk.Name);
                    return true;
                }
                if (atk.Name == "Perplex")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Fortunate Eye: put the top 5 cards of the opponent's deck back in any order.</summary>
        private sealed class GothoritaWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fortunate Eye") return false;
                CardsDRI2.ReorderOpponentTop(ctx, 5, atk.Name);
                return true;
            }
        }

        /// <summary>Distorted Future: once during your turn, while Active, the opponent shuffles their hand into their deck and draws 3. Synchro Shot: +90 with equal hand sizes.</summary>
        private sealed class GothitelleWHT : Mon
        {
            protected override string Activated { get { return "Distorted Future"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchro Shot") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count == ctx.Opp.Hand.Count ? 90 : 0));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Harden: during the opponent's next turn, prevent damage of 40 or less from attacks.</summary>
        private sealed class RoggenrolaWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Harden") return false;
                var me = ctx.Pokemon;
                ctx.AddEffect(new SmallDamageShieldEffect(me.Name + " takes no damage of 40 or less during the opponent's next turn (Harden)", ctx.Player, me.Uid, ctx.Turn + 1, 40));
                return true;
            }
        }

        /// <summary>Smack Down: +50 if the opponent's Active Pokémon has Fighting Resistance.</summary>
        private sealed class BoldoreWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smack Down") return false;
                bool resists = ctx.Defending != null && ctx.Defending.Def.Resistance == EnergyType.Fighting;
                ctx.DamageDefending(atk.Damage + (resists ? 50 : 0));
                return true;
            }
        }

        /// <summary>Vengeful Cannon: 20 for each damage counter on your Benched Fighting Pokémon.</summary>
        private sealed class GigalithWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vengeful Cannon") return false;
                int counters = ctx.Me.Bench.Where(m => ctx.Engine.IsType(m, EnergyType.Fighting)).Sum(m => m.DamageCounters);
                ctx.DamageDefending(atk.Damage * counters);
                return true;
            }
        }

        /// <summary>Rising Chop: only against a Pokémon ex, and Weakness and Resistance don't apply.</summary>
        private sealed class SawkWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rising Chop") return false;
                var d = ctx.Defending;
                if (d == null || !(d.Def.IsEx || d.Def.IsMegaEx)) return Nothing(ctx);
                ctx.DamageWith(d, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>Ancient Wing: once during your turn, while Active, devolve 1 of the opponent's evolved Pokémon (the top card goes to their hand).</summary>
        private sealed class ArcheopsWHT : Mon
        {
            protected override string Activated { get { return "Ancient Wing"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.OpponentPokemon(m => m.Stack.Count > 1 && !m.Def.IsMultiCard).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m.Stack.Count > 1 && !m.Def.IsMultiCard), "Ancient Wing: devolve which of your opponent's evolved Pokémon?");
                if (target != null) ctx.Engine.Devolve(target, 1);
            }
        }

        // ================================================================== Darkness

        /// <summary>Ruffians Attack: flip a coin for each of your Darkness Pokémon, 60 per heads.</summary>
        private sealed class ScraftyWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ruffians Attack") return false;
                int n = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Darkness)).Count;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(n));
                return true;
            }
        }

        /// <summary>Mind Jack: 30 for each of the opponent's Benched Pokémon. Foul Play: use 1 of the opponent's Active Pokémon's attacks.</summary>
        private sealed class ZoroarkWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mind Jack")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Opp.Bench.Count);
                    return true;
                }
                if (atk.Name == "Foul Play")
                {
                    var defending = ctx.Defending;
                    CardDef copied;
                    int index;
                    if (ChooseCopiedAttack(ctx, defending, "Foul Play: choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index))
                        UseCopiedAttack(ctx, copied, index);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Look for Prey: once during your turn, the opponent reveals their hand and you put a Basic Pokémon with 70 HP or less from it onto their Bench.</summary>
        private sealed class MandibuzzWHT : Mon
        {
            protected override string Activated { get { return "Look for Prey"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Hand.Count > 0 && ctx.Engine.BenchHasRoom(ctx.Opponent); }

            protected override void Ability(EffectContext ctx)
            {
                CardsTWM1.RevealOpponentHand(ctx);
                var prey = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon && c.Def.Hp <= 70).ToList();
                if (prey.Count == 0) return;
                var pick = ctx.ChooseCards(prey, 1, 1, "Look for Prey: put which Basic Pokémon onto your opponent's Bench?")[0];
                ctx.Opp.Hand.Remove(pick);
                ctx.Engine.PutOntoBench(ctx.Opponent, pick, false);
            }
        }

        /// <summary>Greedy Eater: 1 more Prize card when its attack Knocks Out the opponent's Basic Pokémon. Dark Bite: the Defending Pokémon can't retreat.</summary>
        private sealed class HydreigonExWHT : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var me = src.Pokemon;
                if (me == null || taker != src.Player || knockedOut.Owner == src.Player || !src.IsActive(me)) return prizes;
                bool byMe = knockedOut.HitByAttack && !knockedOut.ForcedKnockOut && knockedOut.HitByPlayer == src.Player && knockedOut.HitByAttackerDef == me.Def;
                return byMe && knockedOut.Def.IsBasicPokemon ? prizes + 1 : prizes;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        // ================================================================== Metal / Dragon

        /// <summary>Power Whip: 20 for each Energy on it, to 1 of the opponent's Pokémon.</summary>
        private sealed class FerrothornWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Whip") return false;
                int dmg = atk.Damage * ctx.EnergyCount(ctx.Pokemon);
                if (dmg > 0) ctx.DamageAnyOpponent(dmg);
                return true;
            }
        }

        /// <summary>Bite Together: +20 if Durant is on your Bench.</summary>
        private sealed class DurantWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bite Together") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Bench.Any(m => m.Name == "Durant") ? 20 : 0));
                return true;
            }
        }

        /// <summary>Shred: ignores effects on the Defending Pokémon. Ambush: flip, +60.</summary>
        private sealed class DruddigonWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Shred")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                if (atk.Name == "Ambush")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 60 : 0));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Focus Energy: next turn Hyper Fang's base damage is 240. Hyper Fang: tails does nothing.</summary>
        private sealed class WatchogWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                int uid = ctx.Pokemon.Uid;
                if (atk.Name == "Focus Energy")
                {
                    var boost = new AttackBoostEffect(ctx.Pokemon.Name + "'s Hyper Fang does 240 base damage during its next turn", ctx.Player, uid, ctx.Turn + 2, "Hyper Fang", 160);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name == "Hyper Fang")
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    int bonus = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Any(e => e.TargetUid == uid && e.AttackName == "Hyper Fang") ? 160 : 0;
                    ctx.DamageDefending(atk.Damage + bonus);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Odor Sleuth: flip 3 coins, put up to that many cards from your discard pile into your hand. Special Fang: +100 with Special Energy attached.</summary>
        private sealed class StoutlandWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Odor Sleuth")
                {
                    int heads = ctx.FlipCoins(3);
                    if (heads == 0 || ctx.Me.Discard.Count == 0) return true;
                    var picks = ctx.ChooseCards(ctx.Me.Discard.ToList(), 0, Math.Min(heads, ctx.Me.Discard.Count), "Odor Sleuth: put up to " + heads + " cards from your discard pile into your hand");
                    ctx.DiscardToHand(picks);
                    return true;
                }
                if (atk.Name == "Special Fang")
                {
                    bool special = ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
                    ctx.DamageDefending(atk.Damage + (special ? 100 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Bouffer: takes 30 less damage from attacks. Gold Breaker: +100 against a Pokémon ex.</summary>
        private sealed class BouffalantExWHT : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gold Breaker") return false;
                var d = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (d != null && (d.Def.IsEx || d.Def.IsMegaEx) ? 100 : 0));
                return true;
            }
        }

        /// <summary>Wrapped in Wind: attach a Basic Energy from your hand to it. Hurricane: move a Basic Energy from it to a Benched Pokémon.</summary>
        private sealed class TornadusWHT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Wrapped in Wind")
                {
                    var basic = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                    if (basic.Count == 0 || !ctx.Engine.CanAttachFromHand(me)) return true;
                    var pick = ctx.ChooseCards(basic, 1, 1, "Wrapped in Wind: attach which Basic Energy to " + me.Name + "?")[0];
                    ctx.Me.Hand.Remove(pick);
                    ctx.Engine.AttachEnergyCard(pick, me, true);
                    return true;
                }
                if (atk.Name == "Hurricane")
                {
                    ctx.DamageDefending(atk.Damage);
                    var basic = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                    if (!ctx.InPlay(me) || basic.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                    var energy = basic.Count == 1 ? basic[0] : ctx.ChooseCards(basic, 1, 1, "Hurricane: move which Basic Energy?")[0];
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Hurricane: move " + energy.Def.Name + " to which Benched Pokémon?");
                    if (target != null) ctx.Engine.MoveEnergy(me, target, energy);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Trainers

        /// <summary>Plume Protection: on the Bench, prevent all damage done to it by the opponent's attacks.</summary>
        private sealed class AntiquePlumeFossil : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && !src.IsActive(src.Pokemon) && info.AgainstOpponent ? 0 : damage;
            }
        }

        private sealed class Cheren : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>Both players shuffle their hands into their decks; flip: heads you draw 5 and they draw 3, tails the other way round.</summary>
        private sealed class Harlequin : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                bool heads = ctx.Flip();
                ctx.Draw(heads ? 5 : 3);
                ctx.Engine.DrawCards(ctx.Opponent, heads ? 3 : 5);
            }
        }
    }
}
