using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Magma vs Team Aqua Rare Pokémon and Pokémon-ex (MA 1-23, 89-97; v160). Unlimited only. The dual-type Team Aqua and
    /// Team Magma Pokémon are both types (secondType). Team Magma's Groudon 9 is Celebrations' Classic Collection CC11 (a print).
    /// "With Team Aqua / Team Magma in its name" goes by the Pokémon's name.
    /// </summary>
    public static class CardsMA
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MA-1", new ComboMon(new CardsSS.DiscardForConditionMon("Dark Bind", EnergyType.Darkness, SpecialCondition.Paralyzed),
                new ConditionAttackMon("Poison Barb", SpecialCondition.Poisoned)));
            r.RegisterByKey("MA-2", new ComboMon(new TeamSwitchFirstMon("Aqua Reverse", Magma), new DamagedConditionMon("Deep Impact", SpecialCondition.Confused)));
            r.RegisterByKey("MA-3", new PowerSaverMon(Aqua, new ComboMon(new CardsSI.HeadsTailsConditionMon("Aqua Trip", SpecialCondition.Confused, SpecialCondition.Asleep),
                CardsSS.ConditionBonus("Aqua Smash", 20, d => d.Conditions != SpecialCondition.None))));
            r.RegisterByKey("MA-4", new ManectricPowerShift());
            r.RegisterByKey("MA-5", new ComboMon(new Cards30C.SelfCounterBonusMon("Rage", 10), new LockAfterAttackMon("Aqua Slash", false)));
            r.RegisterByKey("MA-6", new ComboMon(new CardsASR1.PlusMon("Power Blow", 10, c => c.EnergyCount(c.Pokemon)), new HydroReverseMon()));
            r.RegisterByKey("MA-7", new ComboMon(new CardsSS.EnergyCoinsMon("Crush", 99), new DiscardBasicsForBonusMon("Land Stream", 20)));
            r.RegisterByKey("MA-8", new ClaydolMagmaSwitch());
            r.RegisterByKey("MA-10", new ComboMon(CardsSS.ConditionBonus("Roasting Heat", 20, d => d.Has(SpecialCondition.Burned)), new MagmaSpurtMon()));
            r.RegisterByKey("MA-11", new ComboMon(new CardsFLF1.NoResistanceMon("Magma Jab"), new TeamConditionMon("Shoot Down", Aqua, SpecialCondition.Confused)));
            r.RegisterByKey("MA-12", new ComboMon(new MagmaBurnMon(), new HotAirMon()));
            r.RegisterByKey("MA-13", new ComboMon(new CardsFST1.TwoBenchedMon("Spark", 10), new KerzapMon()));
            r.RegisterByKey("MA-14", new ComboMon(CardsSS.ConditionBonus("Poison Reaction", 20, d => d.Has(SpecialCondition.Poisoned)), new CardsRSB.CoinsPlusMon("Double Claw", 2, 20)));
            r.RegisterByKey("MA-15", new ExConditionMon("Mystic Fang", SpecialCondition.Confused));
            r.RegisterByKey("MA-16", new ComboMon(new DelayedConditionMon("Aqua Trance", SpecialCondition.Asleep),
                CardsSS.ConditionBonus("Super Hypnoblast", 20, d => d.Has(SpecialCondition.Asleep))));
            r.RegisterByKey("MA-17", new ComboMon(new CallForFamilyOneMon(d => d.PokemonType == EnergyType.Grass || Aqua(d.Name)), new ThickPoisonMon()));
            r.RegisterByKey("MA-18", new ComboMon(new DelayedConditionMon("Slow-Acting Poison", SpecialCondition.Poisoned),
                CardsSS.ConditionBonus("Aqua Smash", 20, d => d.Conditions != SpecialCondition.None)));
            r.RegisterByKey("MA-19", new CameruptOverheat());
            r.RegisterByKey("MA-20", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("MA-21", new ComboMon(new BenchOnlyMon("Overrun", 10), CardsSS.ConditionBonus("Bite Off", 40, d => d.Def.IsOldEx)));
            r.RegisterByKey("MA-22", new ComboMon(CardsSS.ConditionBonus("Second Strike", 20, d => d.DamageCounters >= 2), new CardsDR.FlipDiscardSelfMon("Land Crush", null)));
            r.RegisterByKey("MA-23", new ComboMon(new CallForFamilyOneMon(d => d.PokemonType == EnergyType.Colorless || Magma(d.Name)),
                new CardsASR1.TimesMon("Team Play", c => c.MyPokemon().Count(m => Magma(m.Name)) + c.OpponentPokemon().Count(m => Magma(m.Name)))));

            r.RegisterByKey("MA-89", new ComboMon(new BlazeKickMon(), new VolcanicAshMon()));
            r.RegisterByKey("MA-90", new CradilyExPrimalVibes());
            r.RegisterByKey("MA-91", new ComboMon(new CardsPAR2.FlipConditionMon("Searing Flame", SpecialCondition.Burned), new CardsTWM1.SelfDiscardMon("Bright Flame", 2)));
            r.RegisterByKey("MA-92", new ComboMon(new CardsPAR2.FlipConditionMon("Dazzle Blast", SpecialCondition.Confused), new CardsFST1.FlipRecoilMon("Lightning Tackle", 20)));
            r.RegisterByKey("MA-93", new ComboMon(new GreenHealMon(), new CardsPOR.PoisonRingMon("Poison Ring"), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("MA-94", new ComboMon(new EnergyFlipMon(), new ReverseStreamMon()));
            r.RegisterByKey("MA-95", new ComboMon(new ExtraBasicMon("Hyper Pump", 20, 4), new CrushingWaveMon()));
            r.RegisterByKey("MA-96", new ComboMon(new FlipBonusMon("Quick Attack", 10), new CardsCES1.FeintAttackMon("Feint Attack", 30)));
            r.RegisterByKey("MA-97", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new SwiftMon("Swift")));
        }

        // ================================================================== Shared (EX era)

        internal static bool Aqua(string name) { return name != null && name.Contains("Team Aqua"); }

        internal static bool Magma(string name) { return name != null && name.Contains("Team Magma"); }

        private static bool Aqua(PokemonInPlay mon) { return mon != null && Aqua(mon.Name); }

        private static bool Magma(PokemonInPlay mon) { return mon != null && Magma(mon.Name); }

        /// <summary>
        /// Power Saver (Poké-Body): while there are 3 or fewer Pokémon in play (both players') with [Team] in their name, this
        /// Pokémon can't attack.
        /// </summary>
        internal sealed class PowerSaverMon : Mon
        {
            private readonly Func<PokemonInPlay, bool> team;
            private readonly Mon attacks;

            public PowerSaverMon(Func<PokemonInPlay, bool> team, Mon attacks)
            {
                this.team = team;
                this.attacks = attacks;
            }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon != src.Pokemon) return false;
                int count = 0;
                for (int side = 0; side < 2; side++)
                    foreach (var other in src.State.Players[side].InPlay())
                        if (team(other)) count++;
                return count <= 3;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"At the end of your opponent's next turn, the Defending Pokémon is now [condition]" (if it's still Active then).</summary>
        internal sealed class DelayedConditionEffect : ContinuousEffect
        {
            private readonly SpecialCondition condition;

            public DelayedConditionEffect(string name, int owner, int targetUid, int currentTurn, SpecialCondition condition)
                : base(name, owner, targetUid, currentTurn + 1, true)
            {
                this.condition = condition;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon == null || !engine.State.IsActive(mon)) return;
                engine.ApplyCondition(mon, condition);
            }
        }

        /// <summary>Aqua Trance, Slow-Acting Poison: the damage, then the condition at the end of the opponent's next turn.</summary>
        internal sealed class DelayedConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public DelayedConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new DelayedConditionEffect(defending.Name + " will be " + condition + " at the end of its next turn (" + attack + ")",
                        ctx.Player, defending.Uid, ctx.Turn, condition));
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon has [Team] in its name, it is now [condition]" (Shoot Down, Aqua Beam, Target Scorch).</summary>
        internal sealed class TeamConditionMon : Mon
        {
            private readonly string attack;
            private readonly Func<string, bool> team;
            private readonly SpecialCondition condition;

            public TeamConditionMon(string attack, Func<string, bool> team, SpecialCondition condition)
            {
                this.attack = attack;
                this.team = team;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && team(defending.Name)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon already has damage counters on it, it is now [condition]" (checked before the damage: Deep Impact, Land Sonic).</summary>
        internal sealed class DamagedConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public DamagedConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool damaged = defending != null && defending.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage);
                if (damaged && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon is a Pokémon-ex, it is now [condition]" (Mystic Fang).</summary>
        internal sealed class ExConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public ExConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Def.IsOldEx) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"N damage to 1 of the opponent's Benched Pokémon" and nothing to the Active one (Overrun, Pebble Throw).</summary>
        internal sealed class BenchOnlyMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BenchOnlyMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>"Search your deck for a [kind of] Basic Pokémon and put it onto your Bench" (Call for Family: 1 card).</summary>
        internal sealed class CallForFamilyOneMon : Mon
        {
            private readonly Func<CardDef, bool> filter;

            public CallForFamilyOneMon(Func<CardDef, bool> filter) { this.filter = filter; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(filter, 1, "Call for Family: choose a Basic Pokémon to put onto your Bench");
                return true;
            }
        }

        /// <summary>"You may discard any number of basic Energy cards attached to this Pokémon; N more damage for each" (Land Stream).</summary>
        internal sealed class DiscardBasicsForBonusMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public DiscardBasicsForBonusMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                int n = 0;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (!CardsBS1.CopyingWithMetronome && basics.Count > 0)
                    foreach (var card in ctx.ChooseCards(basics, 0, basics.Count, attack + ": discard any number of basic Energy cards (" + per + " more damage each)"))
                        if (ctx.Engine.DiscardEnergy(me, card)) n++;
                ctx.DamageDefending(atk.Damage + per * n);
                return true;
            }
        }

        /// <summary>"N more for each basic Energy attached but not used to pay for this attack's cost; no more than M extra" (Hyper Pump).</summary>
        internal sealed class ExtraBasicMon : Mon
        {
            private readonly string attack;
            private readonly int per;
            private readonly int maxExtra;

            public ExtraBasicMon(string attack, int per, int maxExtra)
            {
                this.attack = attack;
                this.per = per;
                this.maxExtra = maxExtra;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                int basics = me.Energy.Count(e => e.Def.IsBasicEnergy);
                int extra = Math.Min(maxExtra, Math.Min(basics, Math.Max(0, ctx.EnergyCount(me) - atk.Cost.Length)));
                ctx.DamageDefending(atk.Damage + per * extra);
                return true;
            }
        }

        /// <summary>Swift: damage not affected by Weakness, Resistance or any effects on the Defending Pokémon.</summary>
        internal sealed class SwiftMon : Mon
        {
            private readonly string attack;

            public SwiftMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>Aqua Reverse: before the damage, you may switch in 1 of the opponent's Benched Pokémon with Team Magma in its name.</summary>
        private sealed class TeamSwitchFirstMon : Mon
        {
            private readonly string attack;
            private readonly Func<PokemonInPlay, bool> team;

            public TeamSwitchFirstMon(string attack, Func<PokemonInPlay, bool> team)
            {
                this.attack = attack;
                this.team = team;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var targets = ctx.GustTargets().Where(team).ToList();
                if (targets.Count > 0 && ctx.AskYesNo(attack + ": switch 1 of your opponent's Benched Pokémon with Team Magma in its name with the Defending Pokémon first?"))
                {
                    var pick = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, attack + ": choose the new Defending Pokémon");
                    if (pick != null) ctx.Engine.SwitchActive(ctx.Opponent, pick);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Power Shift (Poké-Power): once during your turn, move any number of basic Energy cards from 1 of your Pokémon with Team
        /// Aqua in its name to another of your Pokémon; not with a Special Condition. Thunderspark: 10 to each Benched Pokémon (both
        /// sides) with Energy cards attached.
        /// </summary>
        private sealed class ManectricPowerShift : Mon
        {
            private const string Power = "Power Shift";

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => Aqua(m) && m.Energy.Any(e => e.Def.IsBasicEnergy)); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && Donors(ctx).Count > 0 && ctx.MyPokemon().Count > 1; }

            protected override void Ability(EffectContext ctx)
            {
                var donors = Donors(ctx);
                var from = donors.Count == 1 ? donors[0] : ctx.ChoosePokemon(donors, Power + ": move basic Energy cards from which Pokémon?");
                if (from == null) return;
                var targets = ctx.MyPokemon(m => m != from);
                var to = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, Power + ": move them to which Pokémon?");
                if (to == null) return;
                var basics = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                foreach (var card in ctx.ChooseCards(basics, 1, basics.Count, Power + ": move which basic Energy cards to " + to.Name + "?"))
                    ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderspark") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).Where(m => m.Energy.Count > 0).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Hydro Reverse: return any number of basic Water Energy cards attached to your Pokémon to your hand; 10 more for each.</summary>
        private sealed class HydroReverseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Reverse") return false;
                int n = 0;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var holders = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var e in mon.Energy.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Water))) holders[e] = mon;
                    if (holders.Count > 0)
                        foreach (var card in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Hydro Reverse: return any number of basic Water Energy cards to your hand (10 more damage each)"))
                            if (ctx.Engine.ReturnEnergyToHand(holders[card], card)) n++;
                }
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        /// <summary>
        /// Magma Switch (Poké-Power): once during your turn, move an Energy card from 1 of your Pokémon with Team Magma in its name to
        /// another of your Pokémon; not with a Special Condition. Dark Hand: 20 more with more cards in your hand than the opponent.
        /// </summary>
        private sealed class ClaydolMagmaSwitch : Mon
        {
            private const string Power = "Magma Switch";
            private static readonly Mon Hand = new CardsFST2.ConditionBonusMon("Dark Hand", 20, c => c.Me.Hand.Count > c.Opp.Hand.Count);

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => Magma(m) && m.Energy.Count > 0); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && Donors(ctx).Count > 0 && ctx.MyPokemon().Count > 1; }

            protected override void Ability(EffectContext ctx)
            {
                var donors = Donors(ctx);
                var from = donors.Count == 1 ? donors[0] : ctx.ChoosePokemon(donors, Power + ": move an Energy card from which Pokémon?");
                if (from == null) return;
                var targets = ctx.MyPokemon(m => m != from);
                var to = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, Power + ": move it to which Pokémon?");
                if (to == null) return;
                var card = from.Energy.Count == 1 ? from.Energy[0] : ctx.ChooseCards(from.Energy.ToList(), 1, 1, Power + ": move which Energy card?")[0];
                ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hand.HandleAttack(ctx, atk); }
        }

        /// <summary>Magma Spurt: discard the top 5 cards of your deck; 30 for each Fire and Fighting basic Energy card among them.</summary>
        private sealed class MagmaSpurtMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Spurt") return false;
                var gone = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                int n = gone.Count(c => CardsRS.BasicOf(c.Def, EnergyType.Fire) || CardsRS.BasicOf(c.Def, EnergyType.Fighting));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Magma Burn: the top card of the opponent's deck is discarded; flip a coin, tails, discard a Fire Energy from Torkoal.</summary>
        private sealed class MagmaBurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Burn") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                var me = ctx.Pokemon;
                if (!ctx.Flip() && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, "Magma Burn: discard a Fire Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Hot Air: discard a Fire Energy from Torkoal; the opponent switches the Defending Pokémon with a Benched Pokémon.</summary>
        private sealed class HotAirMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hot Air") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 1, "Hot Air: discard a Fire Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null && !ctx.Engine.IsKnockedOut(ctx.Opp.Active)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Kerzap: flip a coin; heads, 40 more and discard all Lightning Energy cards attached to Raichu.</summary>
        private sealed class KerzapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kerzap") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 40 : 0));
                var me = ctx.Pokemon;
                if (heads && ctx.InPlay(me))
                    foreach (var card in me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Thick Poison: heads, Confused and Poisoned; tails, Poisoned.</summary>
        private sealed class ThickPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thick Poison") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>
        /// Overheat (Poké-Power): once during your turn, a basic Energy card from your discard pile onto Camerupt, then 2 damage
        /// counters on it; not with a Special Condition. Flame Ball: you may move a Fire Energy card to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class CameruptOverheat : Mon
        {
            private const string Power = "Overheat";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.BasicEnergyInDiscard().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0) return;
                var card = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, Power + ": attach which basic Energy card?")[0];
                if (ctx.Me.Discard.Remove(card) && ctx.Engine.AttachEnergyCard(card, me)) ctx.PlaceCounters(me, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Ball") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var fire = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire)).ToList();
                if (!ctx.InPlay(me) || fire.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Flame Ball: move a Fire Energy card to 1 of your Benched Pokémon?")) return true;
                var card = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Flame Ball: move which Fire Energy card?")[0];
                var bench = ctx.MyBench();
                var to = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Flame Ball: move it to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Blaze Kick: heads, 20 more; tails, the Defending Pokémon is Burned.</summary>
        private sealed class BlazeKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze Kick") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Volcanic Ash: discard 2 Fire Energy from Blaziken ex, then 100 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class VolcanicAshMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Volcanic Ash") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                    ctx.DiscardEnergyFrom(me, 2, "Volcanic Ash: discard 2 Fire Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 100));
                return true;
            }
        }

        /// <summary>
        /// Primal Vibes (Poké-Body): while Cradily ex is your Active Pokémon, the opponent can't play Pokémon from their hand to
        /// evolve their Active Pokémon. Eerie Light: Confused. Acidic Poison: Poisoned and Burned.
        /// </summary>
        private sealed class CradilyExPrimalVibes : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Eerie Light", SpecialCondition.Confused), new AcidicPoisonMon());

            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target)
            {
                var me = src.Pokemon;
                return me != null && target != null && target.Owner != src.Player && src.IsActive(me) && src.IsActive(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Acidic Poison: the Defending Pokémon is now Poisoned and Burned.</summary>
        internal sealed class AcidicPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Acidic Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Green Heal: remove 4 damage counters (all if fewer) from each of your Pokémon with Grass Energy attached.</summary>
        private sealed class GreenHealMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Green Heal") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0 && m.Energy.Any(e => CardsHIF1.Provides(ctx, m, e, EnergyType.Grass))))
                    ctx.Heal(mon, 40);
                return true;
            }
        }

        /// <summary>Energy Flip: 10 damage to 1 of the opponent's Benched Pokémon; you may move an Energy card from it to another of their Pokémon.</summary>
        private sealed class EnergyFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Flip") return false;
                if (ctx.Opp.Bench.Count == 0) return true;
                var target = ctx.DamageOneBenched(Math.Max(atk.Damage, 10));
                if (target == null || !ctx.InPlay(target) || target.Energy.Count == 0 || !ctx.CanAffect(target)) return true;
                var others = ctx.OpponentPokemon(m => m != target);
                if (others.Count == 0 || !ctx.AskYesNo("Energy Flip: move an Energy card from " + target.Name + " to another of your opponent's Pokémon?")) return true;
                var card = target.Energy.Count == 1 ? target.Energy[0] : ctx.ChooseCards(target.Energy.ToList(), 1, 1, "Energy Flip: move which Energy card?")[0];
                var to = others.Count == 1 ? others[0] : ctx.ChoosePokemon(others, "Energy Flip: move it to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(target, to, card);
                return true;
            }
        }

        /// <summary>Reverse Stream: you may return all basic Energy cards on Suicune ex to your hand; 10 more for each.</summary>
        private sealed class ReverseStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reverse Stream") return false;
                var me = ctx.Pokemon;
                int n = 0;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (!CardsBS1.CopyingWithMetronome && basics.Count > 0 && ctx.AskYesNo("Reverse Stream: return all " + basics.Count + " basic Energy cards on " + me.Name + " to your hand (10 more damage each)?"))
                    foreach (var card in basics) if (ctx.Engine.ReturnEnergyToHand(me, card)) n++;
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        /// <summary>Crushing Wave: 40 to 1 of the opponent's Pokémon; then a coin, heads, the opponent discards an Energy card from it.</summary>
        private sealed class CrushingWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Wave") return false;
                var target = ctx.DamageAnyOpponent(Math.Max(atk.Damage, 40));
                if (target == null || !ctx.Flip() || !ctx.InPlay(target) || target.Energy.Count == 0 || !ctx.CanAffect(target)) return true;
                var pick = ctx.Engine.ChooseCards(ctx.Opponent, target.Energy.ToList(), 1, 1, "Crushing Wave: discard an Energy card from " + target.Name, ctx.Source);
                if (pick.Count > 0) ctx.Engine.DiscardEnergy(target, pick[0]);
                return true;
            }
        }
    }
}
