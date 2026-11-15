using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Stormfront (SF, 2008; v172): the cards not already in the pool (the others are prints). Unlimited only. Rares, LV.X and
    /// Charizard 103 here; Uncommons, Commons, Trainers and the SH cards in CardsSFB.cs. Energy Link's effect is listed as its
    /// ability in the pool so the engine can offer it (a Pokémon Tool with an ability).
    /// </summary>
    public static partial class CardsSF
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SF-1", new DusknoirShadowCommand());
            r.RegisterByKey("SF-2", new CardsDS.EvolutionaryMon("Emperor Aura", EmperorAura, c => c.PokemonIsActive,
                new ComboMon(new CardsASC.SelfGuardMon("Steel Wing", 20), new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool"))));
            r.RegisterByKey("SF-3", new CardsDS.EvolutionaryMon("Blaze Dance", BlazeDance, c => c.Me.Deck.Count > 0, new ComboMon(new CloseCombatMon(), new SpreadingFireMon())));
            r.RegisterByKey("SF-4", new LumineonFinLuster());
            r.RegisterByKey("SF-5", new MagnezoneMagneticSearch());
            r.RegisterByKey("SF-6", new MagnezoneSuperConnectivity());
            r.RegisterByKey("SF-7", new ComboMon(new CrashChantMon(), new HorrorChantMon()));
            r.RegisterByKey("SF-8", new ComboMon(new LockAfterAttackMon("Slice", true), new SplitBallMon(), new BurstBallMon()));
            r.RegisterByKey("SF-9", new RegigigasRegiForm());
            r.RegisterByKey("SF-10", new SceptileEnergyTrans());
            r.RegisterByKey("SF-11", new TorterraSunshineSong());
            r.RegisterByKey("SF-96", new DusknoirLvXEctoplasm());
            r.RegisterByKey("SF-98", new MachampLvXNoGuard());
            r.RegisterByKey("SF-99", new RaichuLvXLinkLightning());
            r.RegisterByKey("SF-103", new CharizardEnergyBurn());
            RegisterUncommonsAndTrainers(r);
        }

        /// <summary>Shadow Command (Poké-Power): once during your turn, draw 2 cards; with 7 or more in hand, discard down to 6; then 2 damage counters on Dusknoir. Not with a Special Condition. Damage Even, Night Spin.</summary>
        private sealed class DusknoirShadowCommand : Mon
        {
            protected override string Activated { get { return "Shadow Command"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Draw(2);
                if (ctx.Me.Hand.Count >= 7)
                {
                    int n = ctx.Me.Hand.Count - 6;
                    foreach (var c in ctx.ChooseCards(ctx.Me.Hand.ToList(), n, n, "Shadow Command: discard " + n + " cards (down to 6)")) ctx.Engine.DiscardFromHand(ctx.Player, c);
                }
                ctx.Engine.PlaceDamageCounters(ctx.Pokemon, 2, "Shadow Command");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Damage Even")
                {
                    int n = ctx.Pokemon.DamageCounters;
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Damage Even: put " + n + " damage counters on which Pokémon?");
                    if (n > 0 && target != null && ctx.CanAffect(target)) ctx.PlaceCounters(target, n);
                    return true;
                }
                if (atk.Name != "Night Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new LightAttackerShieldEffect(ctx.Pokemon.Name + " is protected from Pokémon with 2 or fewer Energy next turn (Night Spin)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class LightAttackerShieldEffect : ContinuousEffect
        {
            public LightAttackerShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            private static bool Light(GameEngine engine, PokemonInPlay a) { return a != null && engine.EnergyUnits(a).Count <= 2; }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.Attacker.Owner != Owner && Light(engine, info.Attacker) && engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return target != null && target.Uid == TargetUid && kind == EffectSourceKind.Attack && Light(engine, engine.State.Players[1 - Owner].Active);
            }
        }

        /// <summary>Emperor Aura: the opponent can't attach Energy cards from their hand during their next turn.</summary>
        private static void EmperorAura(EffectContext ctx)
        {
            ctx.AddEffect(new NoAttachEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't attach Energy cards from their hand during their next turn (Emperor Aura)", ctx.Player, ctx.Opponent, ctx.Turn + 1, false));
        }

        /// <summary>The player can't attach Energy cards from their hand (to any Pokémon, or only to the Active one).</summary>
        internal sealed class NoAttachEffect : ContinuousEffect
        {
            private readonly int affected;
            private readonly bool activeOnly;

            public NoAttachEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn, bool activeOnly) : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
                this.activeOnly = activeOnly;
            }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target)
            {
                return target != null && target.Owner == affected && (!activeOnly || engine.State.Players[affected].Active == target);
            }
        }

        /// <summary>Blaze Dance: flip; heads, up to 4 Fire Energy cards from your deck attached to your Pokémon in any way.</summary>
        private static void BlazeDance(EffectContext ctx)
        {
            if (!ctx.Flip()) return;
            var picks = ctx.SearchMyDeck(d => CardsRS.BasicOf(d, EnergyType.Fire), 4, "Blaze Dance: choose up to 4 Fire Energy cards");
            if (picks.Count > 0) ctx.AttachCardsAnyWay(picks, picks.Count, (c, m) => true, "Blaze Dance: attach this Fire Energy card to which Pokémon?", false);
            foreach (var card in picks) if (!ctx.MyPokemon().Any(m => m.Energy.Contains(card)) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Deck.Add(card);
            ctx.ShuffleMyDeck();
        }

        /// <summary>Close Combat: during the opponent's next turn, attack damage to this Pokémon is 30 more (after W/R).</summary>
        private sealed class CloseCombatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Close Combat") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.AddEffect(new TakesMoreEffect(ctx.Pokemon.Name + " takes 30 more damage next turn (Close Combat)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 30));
                return true;
            }
        }

        private sealed class TakesMoreEffect : ContinuousEffect
        {
            private readonly int amount;

            public TakesMoreEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true) { this.amount = amount; }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || damage <= 0 || info.Attacker == null || info.Attacker.Owner == Owner || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + amount;
            }
        }

        /// <summary>Spreading Fire: discard 2 Fire Energy; 20 to each of the opponent's Benched Pokémon.</summary>
        private sealed class SpreadingFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spreading Fire") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Spreading Fire: discard 2 Fire Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.OpponentBench().ToList()) ctx.DamageTarget(m, 20);
                return true;
            }
        }

        /// <summary>Fin Luster (Poké-Power): once during your turn, if Lumineon is Active, look at the opponent's hand; with room on their Bench, bench 1 Basic Pokémon from it and switch it in. Not with a Special Condition. Quick Swim, Elegant Swim.</summary>
        private sealed class LumineonFinLuster : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsE2.AnyOpponentMon("Quick Swim", 20), new DodgeMon("Elegant Swim", false));

            protected override string Activated { get { return "Fin Luster"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsDS.PowerReady(ctx) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Fin Luster", "Fin Luster: your opponent's hand", ctx.Source);
                var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon && !c.Def.IsLevelUp).ToList();
                if (basics.Count == 0 || ctx.Opp.Bench.Count >= 5) return;
                var card = ctx.ChooseCards(basics, 1, 1, "Fin Luster: put which Basic Pokémon onto your opponent's Bench?")[0];
                ctx.Opp.Hand.Remove(card);
                var mon = ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
                if (mon != null) ctx.Engine.SwitchActive(ctx.Opponent, mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Magnetic Search (Poké-Power): once during your turn, a Lightning or Metal Pokémon from your deck into your hand. Not with a Special Condition. Speed Shot, Crush Volt.</summary>
        private sealed class MagnezoneMagneticSearch : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsCES1.FeintAttackMon("Speed Shot", 30), new CardsTWM1.SelfDiscardMon("Crush Volt", 1));

            protected override string Activated { get { return "Magnetic Search"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && (d.HasType(EnergyType.Lightning) || d.HasType(EnergyType.Metal)), 1, "Magnetic Search: choose a Lightning or Metal Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Super Connectivity (Poké-Power): once during your turn, attach a Lightning or Metal Energy card from your discard pile to your Active Pokémon, then 1 damage counter on it. Not with a Special Condition. Gyro Ball.</summary>
        private sealed class MagnezoneSuperConnectivity : Mon
        {
            private static readonly Mon Gyro = new CardsFST2.RapidSpinMon("Gyro Ball");

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsEnergy && (c.Def.EnergyProvides == EnergyType.Lightning || c.Def.EnergyProvides == EnergyType.Metal)).ToList(); }

            protected override string Activated { get { return "Super Connectivity"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Active != null && Pile(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(Pile(ctx), 1, 1, "Super Connectivity: attach which Energy card to your Active Pokémon?")[0];
                var active = ctx.Me.Active;
                if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, active) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                if (ctx.InPlay(active)) ctx.Engine.PlaceDamageCounters(active, 1, "Super Connectivity");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Gyro.HandleAttack(ctx, atk); }
        }

        /// <summary>Crash Chant: discard up to 4 Pokémon Tools and Technical Machines in play (both players'); 20 times the number discarded.</summary>
        private sealed class CrashChantMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crash Chant") return false;
                int n = 0;
                for (int i = 0; i < 4; i++)
                {
                    var holders = ctx.MyPokemon(m => m.Tools.Count > 0).Concat(ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m))).ToList();
                    if (holders.Count == 0) break;
                    var holder = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, "Crash Chant: discard a Pokémon Tool or Technical Machine from which Pokémon? (Done to stop)", true);
                    if (holder == null) break;
                    var tool = holder.Tools.Count == 1 ? holder.Tools[0] : ctx.ChooseCards(holder.Tools.ToList(), 1, 1, "Crash Chant: discard which card?")[0];
                    if (ctx.Engine.DiscardTool(holder, tool)) n++;
                }
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Horror Chant: with 4 or more Benched Pokémon on the opponent's side, 1 of them and its cards go back to their hand.</summary>
        private sealed class HorrorChantMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Horror Chant") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count < 4) return true;
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Horror Chant: return which Benched Pokémon to your opponent's hand?");
                if (target != null && ctx.CanAffect(target)) ctx.Engine.ReturnPokemonToHand(target);
                return true;
            }
        }

        /// <summary>Split Ball: move an Energy card from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class SplitBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Split Ball") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Split Ball: move which Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Split Ball: move it to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>Burst Ball: discard 3 Energy from your Pokémon in any way you like.</summary>
        private sealed class BurstBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burst Ball") return false;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 3; i++)
                {
                    var holders = ctx.MyPokemon(m => m.Energy.Count > 0);
                    if (holders.Count == 0) break;
                    var holder = holders.Count == 1 ? holders[0] : ctx.ChoosePokemon(holders, "Burst Ball: discard an Energy from which Pokémon? (" + (i + 1) + " of 3)");
                    if (holder == null) break;
                    ctx.DiscardEnergyFrom(holder, 1, "Burst Ball: discard an Energy from " + holder.Name);
                }
                return true;
            }
        }

        /// <summary>Regi Form (Poké-Body): with Regirock, Regice and Registeel in play, Regigigas's attacks cost 1 Colorless less. Giga Power.</summary>
        private sealed class RegigigasRegiForm : Mon
        {
            private static readonly Mon Giga = new CardsSUM5.OptionalBoostRecoilMon("Giga Power", 40, 40);

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (new[] { "Regirock", "Regice", "Registeel" }.All(n => ctx.MyPokemon(m => m.Name == n).Count > 0)) cost.Remove(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Giga.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Trans (Poké-Power): as often as you like during your turn, move a Grass Energy card from 1 of your Pokémon to another. Not with a Special Condition. Poison Leaf, Slice Drain.</summary>
        private sealed class SceptileEnergyTrans : Mon
        {
            private const string Power = "Energy Trans";
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Poison Leaf", SpecialCondition.Poisoned), new CardsTWM1.HealSelfMon("Slice Drain", 20));

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsEnergy && e.Def.EnergyProvides == EnergyType.Grass)); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.InPlay().Count() > 1 && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => ctx.Me.InPlay().Count() > 1 && Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a Grass Energy card from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), Power + ": move it to which Pokémon?");
                    if (to == null) return false;
                    return ctx.Engine.MoveEnergy(from, to, from.Energy.First(e => e.Def.IsEnergy && e.Def.EnergyProvides == EnergyType.Grass));
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Sunshine Song (Poké-Power): when Torterra evolves from your hand, evolve any of your Grass Pokémon from your deck. Crash
        /// Impact. Land Shake: during the opponent's next turn, each Basic Pokémon they bench from their hand gets 2 damage counters.
        /// </summary>
        private sealed class TorterraSunshineSong : Mon
        {
            private const string Marker = "Land Shake";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var grass = ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass));
                if (!TriggerOnce(ctx, "Sunshine Song", grass.Count > 0 && ctx.Me.Deck.Count > 0, "Sunshine Song: evolve your Grass Pokémon from your deck?")) return;
                foreach (var m in ctx.ChoosePokemonMany(grass, 0, grass.Count, "Sunshine Song: evolve which Grass Pokémon?"))
                    if (ctx.InPlay(m) && ctx.Me.Deck.Count > 0) CardsEVS1.EvolveFromDeck(ctx, m, "Sunshine Song");
            }

            public override void OnPokemonBenchedFromHand(EffectContext src, PokemonInPlay benched)
            {
                int turn;
                var me = src.Pokemon;
                if (me == null || benched == null || benched.Owner == src.Player || !me.Markers.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                src.Log("Land Shake: 2 damage counters on " + benched.Name + ".");
                src.Engine.PlaceDamageCounters(benched, 2, Marker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Crash Impact")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(20);
                    ctx.OpponentSwitchesActive();
                    return true;
                }
                if (atk.Name != Marker) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Pokemon.Markers[Marker] = ctx.Turn + 1;
                return true;
            }
        }

        // ------------------------------------------------------------------ LV.X and Charizard

        /// <summary>
        /// Ectoplasm (Poké-Power): if Dusknoir LV.X is your Active Pokémon and is Knocked Out by damage from the opponent's attack, you
        /// may put it into play as a Stadium instead (its attached cards are discarded; it still counts as Knocked Out). As that
        /// Stadium, 1 damage counter on each of the opponent's Pokémon between turns; if it's discarded, it goes back to your hand.
        /// </summary>
        private sealed class DusknoirLvXEctoplasm : Mon
        {
            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var card = src.Card;
                if (!byOpponentsAttack || !wasActive || card == null || !src.Me.Discard.Contains(card)) return;
                if (!src.AskYesNo("Ectoplasm: put Dusknoir LV.X into play as a Stadium?")) return;
                Announce(src, "Ectoplasm");
                src.Me.Discard.Remove(card);
                if (src.State.Stadium != null) src.Engine.DiscardStadium();
                src.State.Stadium = card;
                src.State.StadiumOwner = src.Player;
                src.Log("Dusknoir LV.X is now a Stadium (Ectoplasm).");
            }

            public override int CheckupAffects(EffectContext src) { return src.Pokemon == null && src.State.Stadium == src.Card ? src.Opponent : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon != null || src.State.Stadium != src.Card) return;
                foreach (var m in src.OpponentPokemon()) src.Engine.PlaceDamageCounters(m, 1, "Ectoplasm");
            }

            public override bool GoesToHandWhenDiscardedFromPlay { get { return true; } }
        }

        /// <summary>
        /// No Guard (Poké-Body): while Machamp is Active, its attacks do 60 more to the Active Pokémon (before W/R) and damage to it
        /// from the opponent's Pokémon is 60 more (after W/R). Strong-Willed: during the opponent's next turn, if it would be Knocked
        /// Out by attack damage, flip: heads, it stays at 10 HP.
        /// </summary>
        private sealed class MachampLvXNoGuard : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !src.IsActive(me) || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 60;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || !src.IsActive(me) || damage <= 0 || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 60;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong-Willed") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.AddEffect(new StrongWilledEffect(ctx.Pokemon.Name + " may survive a Knock Out next turn (Strong-Willed)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class StrongWilledEffect : ContinuousEffect
        {
            public StrongWilledEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay mon)
            {
                return mon != null && mon.Uid == TargetUid && engine.FlipCoin(Owner, "Strong-Willed");
            }
        }

        /// <summary>Link Lightning (Poké-Body): the turn you level up to Raichu LV.X, after using Voltage Shoot it may attack again. Not with a Special Condition. Voltage Shoot: discard 2 Lightning Energy cards from your hand; 80 to 1 of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class RaichuLvXLinkLightning : Mon
        {
            public override bool AttacksTwice(EffectContext src)
            {
                var me = src.Pokemon;
                return me != null && me.TurnEvolved == src.Turn && me.LastAttackName == "Voltage Shoot" && me.LastAttackTurn == src.Turn && CardsE1.NoCondition(me);
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                return atk == null || atk.Name != "Voltage Shoot" || ctx.Me.Hand.Count(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Lightning) >= 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Voltage Shoot") return false;
                var cards = ctx.Me.Hand.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList();
                if (cards.Count < 2) return Nothing(ctx);
                foreach (var c in ctx.ChooseCards(cards, 2, 2, "Voltage Shoot: discard 2 Lightning Energy cards from your hand")) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.DamageAnyOpponent(80);
                return true;
            }
        }

        /// <summary>Energy Burn (Poké-Body): all Energy attached to Charizard is Fire Energy. Fire Spin.</summary>
        private sealed class CharizardEnergyBurn : Mon
        {
            private static readonly Mon Spin = new CardsTWM1.SelfDiscardMon("Fire Spin", 2);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spin.HandleAttack(ctx, atk); }
        }
    }
}
