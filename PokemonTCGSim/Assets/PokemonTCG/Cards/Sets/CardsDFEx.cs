using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Dragon Frontiers Trainers (74-76), Pokémon-ex (90-99) and Pokémon ★ (100-101).</summary>
    public static partial class CardsDF
    {
        private static void RegisterExAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("DF-74", new HolonLegacy());
            r.RegisterByKey("DF-76", new IslandHermit());

            r.RegisterByKey("DF-90", new AltariaExExtraBoost());
            r.RegisterByKey("DF-91", new ComboMon(new CardsDS.RadarJamMon("Deafen"), new DragonRoarMon()));
            r.RegisterByKey("DF-92", new FlygonExSandDamage());
            r.RegisterByKey("DF-94", new KingdraExExtraSmoke());
            r.RegisterByKey("DF-95", new LatiasExFellowBoost());
            r.RegisterByKey("DF-96", new LatiosExLinkWing());
            r.RegisterByKey("DF-97", new RayquazaExRageAura());
            r.RegisterByKey("DF-98", new SalamenceExTypeShift());
            r.RegisterByKey("DF-99", new TyranitarExShockWave());
            r.RegisterByKey("DF-100", new ComboMon(new CardsTRR.RotatingClawsMon(), new DarkSwirlMon()));
            r.RegisterByKey("DF-101", new ComboMon(new MimicryMon(), new RainbowWaveMon()));
        }

        // ================================================================== Trainers

        /// <summary>Holon Legacy (Stadium): each δ Pokémon in play (both players') has no Weakness and can't use Poké-Powers.</summary>
        private sealed class HolonLegacy : CardScript
        {
            [ThreadStatic] private static bool checking;

            private static bool Covers(EffectContext src, PokemonInPlay mon)
            {
                if (checking || mon == null) return false;
                checking = true;
                try { return CardsDS.Delta(src, mon); }
                finally { checking = false; }
            }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return Covers(src, mon); }
        }

        /// <summary>
        /// Island Hermit (Supporter): turn up to 2 of your Prize cards face up for the rest of the game (at least 1 if you have any
        /// face-down ones: Compendium), then draw 2 cards. Face-down Prize cards can't be told apart, so the ones turned are random.
        /// </summary>
        private sealed class IslandHermit : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                var faceDown = me.Prizes.Where(c => !me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (faceDown.Count > 0)
                {
                    int n = faceDown.Count == 1 ? 1 : ctx.ChooseOption("Island Hermit: turn how many of your Prize cards face up?", "1 Prize card", "2 Prize cards") + 1;
                    ctx.State.Rng.Shuffle(faceDown);
                    var turned = faceDown.Take(n).ToList();
                    foreach (var card in turned) me.FaceUpPrizeIds.Add(card.Id);
                    ctx.Engine.ShowCards(ctx.Opponent, turned, "Island Hermit", "Island Hermit: your opponent's Prize cards turned face up", ctx.Source);
                    ctx.Log("Island Hermit: " + string.Join(", ", turned.Select(c => c.Def.Name).ToArray()) + (turned.Count == 1 ? " is" : " are") + " now face up.");
                }
                ctx.Draw(2);
            }
        }

        // ================================================================== Pokémon-ex

        /// <summary>
        /// Extra Boost (Poké-Power): once during your turn, attach a basic Energy card from your hand to 1 of your Stage 2
        /// Pokémon-ex. Not with a Special Condition. Healing Light: remove 1 damage counter from each of your Pokémon.
        /// </summary>
        private sealed class AltariaExExtraBoost : Mon
        {
            private const string Power = "Extra Boost";

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(Stage2Ex); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy) && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                var targets = Targets(ctx);
                if (cards.Count == 0 || targets.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": attach which basic Energy card from your hand?")[0];
                var target = ctx.ChoosePokemon(targets, Power + ": attach " + card.Def.Name + " to which of your Stage 2 Pokémon-ex?");
                if (target != null && ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Light") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Dragon Roar: 8 damage counters on the Defending Pokémon; any it doesn't need to be Knocked Out go on the opponent's Benched
        /// Pokémon in any way you like.
        /// </summary>
        private sealed class DragonRoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Roar") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int needed = Math.Max(0, (ctx.RemainingHp(defending) + 9) / 10);
                int onDefending = Math.Min(8, needed);
                int extra = 8 - onDefending;
                if (onDefending > 0) ctx.PlaceCounters(defending, onDefending);
                if (extra > 0 && ctx.Opp.Bench.Count > 0) ctx.PlaceCountersAnyWay(extra, () => ctx.Opp.Bench.Where(ctx.InPlay).ToList());
                return true;
            }
        }

        /// <summary>
        /// Sand Damage (Poké-Body): while Flygon ex is your Active Pokémon, 1 damage counter on each of the opponent's Benched Basic
        /// Pokémon between turns (only 1 Sand Damage counts). Psychic Pulse: 10 to each of their damaged Benched Pokémon (no W/R).
        /// </summary>
        private sealed class FlygonExSandDamage : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var first = src.MyPokemon(m => src.IsActive(m) && src.Engine.HasAbilityNamed(m, "Sand Damage")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != me) return;
                var targets = src.Opp.Bench.Where(m => m.IsBasic).ToList();
                if (targets.Count == 0) return;
                Announce(src, "Sand Damage");
                foreach (var mon in targets)
                    if (src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 1, "Sand Damage");
            }

            public override int CheckupAffects(EffectContext src) { return 1 - src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Pulse") return false;
                var damaged = ctx.Opp.Bench.Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in damaged)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Extra Smoke (Poké-Body): the opponent's attacks do 10 less to your Stage 2 Pokémon-ex (before W/R; it rides on the
        /// attacker, so Swift doesn't get past it: Compendium). Energy Link. Protective Swirl: no Weakness during the opponent's next turn.
        /// </summary>
        private sealed class KingdraExExtraSmoke : Mon
        {
            private static readonly Mon Link = new CardsDAA2.EnergyLinkMon();

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner == src.Player || info.Target == null || info.Target.Owner != src.Player || damage <= 0) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Stage2Ex(info.Target) ? damage - 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Protective Swirl") return Link.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.AddEffect(new NoWeaknessEffect(me.Name + " has no Weakness during the opponent's next turn (Protective Swirl)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoWeaknessEffect : ContinuousEffect
        {
            public NoWeaknessEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness) { return mon != null && mon.Uid == TargetUid ? null : weakness; }
        }

        /// <summary>
        /// Fellow Boost (Poké-Power): once during your turn, attach a basic Energy card from your hand to your Latias, Latias ex, Latios
        /// or Latios ex; if you do, your turn ends. Not with a Special Condition. Power Crush: if the Defending Pokémon is Knocked Out,
        /// discard 2 Fire Energy from Latias ex.
        /// </summary>
        private sealed class LatiasExFellowBoost : Mon
        {
            private const string Power = "Fellow Boost";
            private static readonly string[] Fellows = { "Latias", "Latias ex", "Latios", "Latios ex" };

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => Fellows.Contains(m.Name)); }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy) && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                var targets = Targets(ctx);
                if (cards.Count == 0 || targets.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": attach which basic Energy card from your hand?")[0];
                var target = ctx.ChoosePokemon(targets, Power + ": attach " + card.Def.Name + " to which Pokémon?");
                if (target == null || !ctx.Me.Hand.Remove(card)) return;
                if (ctx.Engine.AttachEnergyCard(card, target)) ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Crush") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                bool knockedOut = defending != null && (!ctx.InPlay(defending) || ctx.RemainingHp(defending) <= 0);
                if (knockedOut && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 2, "Power Crush: discard 2 Fire Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>
        /// Link Wing (Poké-Body): your Latias, Latias ex, Latios and Latios ex have no Retreat Cost (not Pokémon ★: Compendium). Ice
        /// Barrier: no effects of attacks, damage included, from the opponent's Pokémon-ex during their next turn.
        /// </summary>
        private sealed class LatiosExLinkWing : Mon
        {
            private static readonly string[] Linked = { "Latias", "Latias ex", "Latios", "Latios ex" };
            private static readonly Mon Barrier = new CardsDX.ShieldFromMon("Ice Barrier", m => m.Def.IsOldEx, "Pokémon-ex");

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && Linked.Contains(mon.Name) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Barrier.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rage Aura (Poké-Body): with more Prize cards left than your opponent, Special Circuit and Sky-high Claws need no Colorless
        /// Energy. Special Circuit: 30 to 1 of the opponent's Pokémon, 50 if it has a Poké-Power or Poké-Body (no W/R on the Bench).
        /// </summary>
        private sealed class RayquazaExRageAura : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack == null || (attack.Name != "Special Circuit" && attack.Name != "Sky-high Claws")) return;
                if (ctx.Me.Prizes.Count <= ctx.Opp.Prizes.Count || ctx.Engine.AbilityBlocked(ctx.Pokemon, "Rage Aura")) return;
                cost.RemoveAll(t => t == EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Circuit") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Special Circuit: choose 1 of your opponent's Pokémon (30 damage, 50 if it has a Poké-Power or Poké-Body)");
                if (target != null) ctx.DamageTarget(target, CardsHP.HasPowerOrBody(target) ? 50 : 30);
                return true;
            }
        }

        /// <summary>
        /// Type Shift (Poké-Power): once during your turn, Salamence ex becomes Fire type until the end of your turn. Dual Stream:
        /// 80 to the Defending Pokémon, or 40 to it and 40 to 1 of the opponent's Benched Pokémon (no W/R on the Bench).
        /// </summary>
        private sealed class SalamenceExTypeShift : Mon
        {
            private const string Marker = "Type Shift";

            protected override string Activated { get { return Marker; } }

            protected override void Ability(EffectContext ctx) { ctx.Pokemon.Markers[Marker] = ctx.Turn; }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                int turn;
                if (src.Pokemon == null || mon != src.Pokemon || !mon.Markers.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                types.Clear();
                types.Add(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Stream") return false;
                bool split = ctx.Opp.Bench.Count > 0
                             && ctx.ChooseOption("Dual Stream: how much damage to the Defending Pokémon?", "80 to the Defending Pokémon", "40 to it and 40 to 1 of the Benched Pokémon") == 1;
                ctx.DamageDefending(split ? 40 : atk.Damage);
                if (split) ctx.DamageOneBenched(40);
                return true;
            }
        }

        /// <summary>
        /// Electromark: a Shock-wave marker on 1 of the opponent's Pokémon (not one protected from the attack's effects: Compendium).
        /// Hyper Claws: 20 more against a Stage 2 Evolved Pokémon. Shock-wave: 1 of the opponent's Pokémon with a Shock-wave marker is
        /// Knocked Out.
        /// </summary>
        private sealed class TyranitarExShockWave : Mon
        {
            private static readonly Mon Claws = new CardsFST2.ConditionBonusMon("Hyper Claws", 20, c => c.Defending != null && c.Defending.Def.Stage == Stage.Stage2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Electromark")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var targets = ctx.OpponentPokemon(ctx.CanAffect);
                    if (targets.Count == 0) return true;
                    var target = ctx.ChoosePokemon(targets, "Electromark: put a Shock-wave marker on which of your opponent's Pokémon?");
                    if (target == null) return true;
                    target.Markers[ShockWaveMarker] = 1;
                    ctx.Log(target.Name + " has a Shock-wave marker.");
                    return true;
                }
                if (atk.Name == "Shock-wave")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var marked = ctx.OpponentPokemon(m => m.Markers.ContainsKey(ShockWaveMarker));
                    if (marked.Count == 0) return true;
                    var target = ctx.ChoosePokemon(marked, "Shock-wave: which of your opponent's Pokémon with a Shock-wave marker is Knocked Out?");
                    if (target != null) ctx.Engine.KnockOut(target, "Shock-wave");
                    return true;
                }
                return Claws.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Pokémon ★

        /// <summary>Dark Swirl: discard all Energy cards from Charizard ★ and the top 3 cards of the opponent's deck.</summary>
        private sealed class DarkSwirlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Swirl") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    foreach (var card in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, card);
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
                return true;
            }
        }

        /// <summary>Mimicry: use an attack of 1 of your opponent's Pokémon in play; it does nothing unless Mew ★ has the Energy that attack needs.</summary>
        private sealed class MimicryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mimicry") return false;
                var mons = ctx.OpponentPokemon(m => m.Def.Attacks.Count > 0);
                if (mons.Count == 0) return true;
                var mon = mons.Count == 1 ? mons[0] : ctx.ChoosePokemon(mons, "Mimicry: copy an attack of which of your opponent's Pokémon?");
                CardDef source;
                int index;
                if (!ChooseCopiedAttack(ctx, mon, "Mimicry: copy which attack?", out source, out index)) return true;
                if (!ctx.Engine.CanPayAttack(ctx.Pokemon, source.Attacks[index]))
                {
                    ctx.Log("Mimicry: " + ctx.Pokemon.Name + " doesn't have the Energy for " + source.Attacks[index].Name + ", so it does nothing.");
                    return true;
                }
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>Rainbow Wave: choose a basic Energy card on Mew ★; 20 to each of the opponent's Pokémon of that type (no W/R on the Bench).</summary>
        private sealed class RainbowWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Wave") return false;
                var basics = ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return true;
                var card = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Rainbow Wave: choose a basic Energy card attached to " + ctx.Pokemon.Name)[0];
                var type = card.Def.EnergyProvides;
                foreach (var mon in ctx.OpponentPokemon(m => ctx.Engine.IsType(m, type)))
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }
    }
}
