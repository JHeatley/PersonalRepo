using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ancient Origins (AOR, v98), part 2: the Fighting, Darkness, Metal, Fairy, Dragon and Colorless Pokémon (37-68) and the θ Max
    /// secret rares (96-98). Ralts (52) is BREAKthrough's and Persian (62) Generations'; they keep those scripts.
    /// </summary>
    public static class CardsAOR2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("AOR-37", new MachampEX());
            r.RegisterByKey("AOR-38", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("AOR-39", new CardsFLF1.NoResistanceMon("Landslide"));
            r.RegisterByKey("AOR-40", new CardsFST2.ConditionBonusMon("Unyielding Rock", 60, c => c.Defending != null && c.Defending.Def.IsPokemonEX));
            r.RegisterByKey("AOR-41", new ComboMon(new DigOutMon(), new CoinsMon("Double Lariat", 2)));
            // Darkness
            r.RegisterByKey("AOR-42", new CardsBUS1.OwnBenchQuakeMon("Break Ground", 10));
            r.RegisterByKey("AOR-43", new CardsASR1.PlusMon("Destroyer King", 60, c => c.Defending == null ? 0 : c.Defending.DamageCounters));
            r.RegisterByKey("AOR-44", new ComboMon(new CardsCRZ1.PrimateActingMon("Bewitching Eyes"), new CardsBKP2.CountersOnActiveMon("Furtive Drop", 3)));
            r.RegisterByKey("AOR-45", new CardsUPR3.SmokescreenMon("Ink Spit", 1));
            r.RegisterByKey("AOR-46", new ComboMon(new EntanglingControlMon(), new TrashTentacleMon()));
            // Metal
            r.RegisterByKey("AOR-48", new CardsWHT.FlipMoreMon("Bullet Punch", 2, 20));
            r.RegisterByKey("AOR-49", new MetagrossAOR());
            r.RegisterByKey("AOR-50", new ComboMon(new CardsASR1.PlusMon("Machine Gun Stomp", 10, c => c.Me.Hand.Count), new CardsASC.SelfGuardMon("Guard Press", 20)));
            r.RegisterByKey("AOR-51", new ComboMon(new CardsASR1.TimesMon("Iron Head", c => c.FlipUntilTails()), new ForbiddenIronHammerMon()));
            // Fairy
            r.RegisterByKey("AOR-53", new CardsTWM1.HealSelfMon("Calm Mind", 30));
            r.RegisterByKey("AOR-54", new GardevoirAOR());
            r.RegisterByKey("AOR-55", new ConditionAttackMon("Cotton Bed", SpecialCondition.Asleep));
            r.RegisterByKey("AOR-56", new WindyMischiefMon());
            // Dragon
            r.RegisterByKey("AOR-57", new GiratinaEX());
            r.RegisterByKey("AOR-58", new GoomyAOR());
            r.RegisterByKey("AOR-59", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("AOR-60", new ComboMon(new LiquidBlowMon(), new ShiningBreathMon()));
            // Colorless
            r.RegisterByKey("AOR-61", new Cards30C.TypeEnergyBonusMon("Act Tough", EnergyType.Darkness, 20));
            r.RegisterByKey("AOR-63", new TailsNothingMon("Lunge"));
            r.RegisterByKey("AOR-64", new DataCheckMon());
            r.RegisterByKey("AOR-65", new CoinsMon("Tri Attack", 3));
            r.RegisterByKey("AOR-66", new ComboMon(new CyberCrushMon(), new SlowingBeamMon()));
            r.RegisterByKey("AOR-67", new ComboMon(new DigitalRebootMon(), new ConditionAttackMon("Dazzle Blast", SpecialCondition.Confused)));
            r.RegisterByKey("AOR-68", new ComboMon(new CardsASR1.TimesMon("Aero Ball", c => c.EnergyCount(c.Pokemon) + (c.Defending == null ? 0 : c.EnergyCount(c.Defending))),
                new StadiumBlowMon("Deep Hurricane", 70)));
            // θ Max secret rares
            r.RegisterByKey("AOR-96", new TidalStormMon());
            r.RegisterByKey("AOR-97", new StadiumBlowMon("Gaia Volcano", 100));
            r.RegisterByKey("AOR-98", new CardsASR1.TimesMon("Emerald Break", c => c.Me.Bench.Count));
        }

        // ================================================================== Fighting

        /// <summary>
        /// Steaming Mad: 20 damage for each damage counter on this Pokémon, then it's Confused. Crazy Hammer: 80 more while this Pokémon
        /// is affected by a Special Condition, then it recovers from all of them.
        /// </summary>
        private sealed class MachampEX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Steaming Mad")
                {
                    ctx.DamageDefending(atk.Damage * me.DamageCounters);
                    if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name != "Crazy Hammer") return false;
                bool affected = me.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage + (affected ? 80 : 0));
                if (ctx.InPlay(me)) ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }

        /// <summary>Dig Out: discard the top card of your deck; if it's a Fighting Energy card, attach it to this Pokémon.</summary>
        internal sealed class DigOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Out") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                var me = ctx.Pokemon;
                foreach (var card in top)
                    if (CardsSSP1.IsBasicOf(card.Def, EnergyType.Fighting) && ctx.InPlay(me) && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Entangling Control: switch 1 of the opponent's Benched Pokémon into the Active Spot; the new Active Pokémon is Confused.</summary>
        private sealed class EntanglingControlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entangling Control") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pulled = ctx.GustOpponent("Entangling Control: choose 1 of your opponent's Benched Pokémon to switch in");
                if (pulled != null && ctx.InPlay(pulled)) ctx.ApplyCondition(pulled, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Trash Tentacle: put a card from your discard pile into your hand.</summary>
        private sealed class TrashTentacleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trash Tentacle") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                var pick = pile.Count == 1 ? pile : ctx.ChooseCards(pile, 1, 1, "Trash Tentacle: put a card from your discard pile into your hand");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>
        /// Magnetic Warp: once during your turn, if this Pokémon is Active, switch it with 1 of your Benched Pokémon; if you do, the
        /// opponent switches their Active Pokémon. Iron Cannon: you may discard all Metal Energy from it for 80 more damage.
        /// </summary>
        private sealed class MetagrossAOR : Mon
        {
            protected override string Activated { get { return "Magnetic Warp"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active == ctx.Pokemon && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.SwitchSelfWithBench()) ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Cannon") return false;
                var me = ctx.Pokemon;
                var metal = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Metal)).ToList();
                bool more = metal.Count > 0 && ctx.AskYesNo("Iron Cannon: discard all Metal Energy from " + me.Name + " for 80 more damage?");
                if (more)
                    foreach (var card in metal) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage + (more ? 80 : 0));
                return true;
            }
        }

        /// <summary>Forbidden Iron Hammer: if the opponent's Active Pokémon is a Pokémon-EX, discard an Energy attached to it.</summary>
        private sealed class ForbiddenIronHammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Forbidden Iron Hammer") return false;
                var defending = ctx.Defending;
                bool ex = defending != null && defending.Def.IsPokemonEX;
                ctx.DamageDefending(atk.Damage);
                if (ex && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Forbidden Iron Hammer: discard an Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Bright Heal: once during your turn, heal 20 damage from each of your Pokémon. Telekinesis: 50 to 1 of the opponent's Pokémon, no W/R.</summary>
        private sealed class GardevoirAOR : Mon
        {
            protected override string Activated { get { return "Bright Heal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyPokemon().Any(m => m.DamageCounters > 0); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Telekinesis") return false;
                ctx.DamageAnyOpponent(atk.Damage, true);
                return true;
            }
        }

        /// <summary>Windy Mischief: move all damage counters from 1 of your Benched Pokémon to the opponent's Active Pokémon.</summary>
        private sealed class WindyMischiefMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Windy Mischief") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                var donors = ctx.Me.Bench.Where(m => m.DamageCounters > 0).ToList();
                if (defending == null || donors.Count == 0) return true;
                var donor = ctx.ChoosePokemon(donors, "Windy Mischief: move all damage counters from which Benched Pokémon?");
                if (donor != null) ctx.Engine.MoveDamageCounters(donor, defending, donor.DamageCounters, "Windy Mischief");
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Renegade Pulse: no damage or effects from the attacks of the opponent's Mega Evolution Pokémon (Primal Pokémon included:
        /// 2015). Jagged Saber's attachment to your own Benched Pokémon isn't an effect on it (2015-09-03). Chaos Wheel: no Tools,
        /// Special Energy or Stadiums for the opponent next turn.
        /// </summary>
        private sealed class GiratinaEX : Mon
        {
            private static readonly Mon Wheel = new CardsCIN2.ChaosWheelMon();

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != me.Owner && info.Attacker.Def.IsXYMegaEvolution;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var attacker = src.Engine.CurrentSourcePokemon;
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && attacker != null && attacker.Owner != target.Owner
                       && attacker.Def.IsXYMegaEvolution;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wheel.HandleAttack(ctx, atk); }
        }

        /// <summary>Water Down: whenever you attach a Water Energy card from your hand to this Pokémon, you may bench a Goomy from your deck.</summary>
        private sealed class GoomyAOR : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target != src.Pokemon || energy == null || !CardsSSP1.IsBasicOf(energy.Def, EnergyType.Water)) return;
                if (src.BenchSpace <= 0 || src.Me.Deck.Count == 0) return;
                if (!src.AskYesNo("Water Down: search your deck for a Goomy and put it onto your Bench?")) return;
                src.SearchToBench(d => d.Name == "Goomy", 1, "Water Down: choose a Goomy to put onto your Bench");
            }
        }

        /// <summary>Liquid Blow: 20 damage for each [C] in this Pokémon's Retreat Cost (as it is now) to 1 of the opponent's Pokémon.</summary>
        private sealed class LiquidBlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Liquid Blow") return false;
                int amount = 20 * ctx.Engine.RetreatCostOf(ctx.Pokemon);
                if (amount > 0) ctx.DamageAnyOpponent(amount);
                else ctx.Log("Liquid Blow: " + ctx.Pokemon.Name + " has no Retreat Cost, so it does no damage.");
                return true;
            }
        }

        /// <summary>This Pokémon can't be affected by Special Conditions during the opponent's next turn (Shining Breath).</summary>
        internal sealed class NoConditionsEffect : ContinuousEffect
        {
            public NoConditionsEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsSpecialConditions(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        private sealed class ShiningBreathMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shining Breath") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new NoConditionsEffect(me.Name + " can't be affected by Special Conditions during the opponent's next turn (Shining Breath)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Data Check: look at your deck, then shuffle it.</summary>
        private sealed class DataCheckMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Data Check") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.LookAtTop(ctx.Me.Deck.Count);
                ctx.ShuffleMyDeck();
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.Their(ctx.Player) + " deck and shuffled it (Data Check).");
                return true;
            }
        }

        /// <summary>Cyber Crush: discard all Special Energy from each of the opponent's Pokémon.</summary>
        private sealed class CyberCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cyber Crush") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon())
                    foreach (var en in mon.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList())
                        if (ctx.InPlay(mon)) ctx.Engine.DiscardEnergy(mon, en);
                return true;
            }
        }

        /// <summary>The Defending Pokémon's attacks cost [C] more during the opponent's next turn (Slowing Beam).</summary>
        internal sealed class AttackCostUpEffect : ContinuousEffect
        {
            public AttackCostUpEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != null && mon.Uid == TargetUid) cost.Add(EnergyType.Colorless);
            }
        }

        private sealed class SlowingBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slowing Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new AttackCostUpEffect(defending.Name + "'s attacks cost [C] more during its next turn (Slowing Beam)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Digital Reboot: devolve any of your Benched Pokémon any number of times, the Evolution cards going into your hand.</summary>
        private sealed class DigitalRebootMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Digital Reboot") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int guard = 0; guard < 20; guard++)
                {
                    var evolved = ctx.Me.Bench.Where(m => m.Stack.Count >= 2 && !m.Def.IsMultiCard).ToList();
                    if (evolved.Count == 0) break;
                    var pick = RepeatableAbility.ChoosePokemonOrDone(ctx, evolved, "Digital Reboot: devolve which Benched Pokémon? (Done to stop)", true);
                    if (pick == null) break;
                    if (ctx.Engine.Devolve(pick, 1).Count == 0) break;
                }
                return true;
            }
        }

        /// <summary>Deep Hurricane / Gaia Volcano: with any Stadium in play, N more damage, then discard that Stadium (before Knock Outs are checked: 2018-05-31).</summary>
        internal sealed class StadiumBlowMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public StadiumBlowMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? bonus : 0));
                if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Tidal Storm: move 2 Energy from this Pokémon to 1 of your Benched Pokémon; also 30 damage to each of the opponent's Benched Pokémon-EX.</summary>
        internal sealed class TidalStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tidal Storm") return false;
                var targets = ctx.Opp.Bench.Where(m => m.Def.IsPokemonEX).ToList();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var bench = ctx.MyBench();
                if (ctx.InPlay(me) && me.Energy.Count > 0 && bench.Count > 0)
                {
                    var receiver = ctx.ChoosePokemon(bench, "Tidal Storm: move 2 Energy to which Benched Pokémon?");
                    var energy = me.Energy.Count <= 2 ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), 2, 2, "Tidal Storm: move which 2 Energy?");
                    foreach (var en in energy) ctx.Engine.MoveEnergy(me, receiver, en);
                }
                foreach (var mon in targets)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }
    }
}
