using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Surging Sparks (SSP) 1-61. Pokémon whose attacks only do their printed damage need no script:
    /// Morelull, Capsakid, Pansear, Larvesta, Sizzlipede, Sealeo, Shellos, Quaxwell, Magnemite.
    /// </summary>
    public static class CardsSSP1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SSP-1", new PrecociousExeggcuteSSP());
            r.RegisterByKey("SSP-2", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("SSP-3", new ExeggutorSSP());
            r.RegisterByKey("SSP-4", new DurantExSSP());
            r.RegisterByKey("SSP-5", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("SSP-6", new SwitchSelfMon("Wander About", false));
            r.RegisterByKey("SSP-7", new VivillonSSP());
            r.RegisterByKey("SSP-9", new ShiinoticSSP());
            r.RegisterByKey("SSP-10", new DhelmiseSSP());
            r.RegisterByKey("SSP-11", new ZarudeSSP());
            r.RegisterByKey("SSP-13", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SSP-14", new RabscaSSP());
            r.RegisterByKey("SSP-15", new WoChienSSP());
            r.RegisterByKey("SSP-16", new RecoilMon("Take Down", 10));
            r.RegisterByKey("SSP-17", new Cards30CC.BenchHitMon("Fire Soul", 30));
            r.RegisterByKey("SSP-19", new HoOhSSP());
            r.RegisterByKey("SSP-20", new CastformSunnySSP());
            r.RegisterByKey("SSP-21", new VictiniSSP());
            r.RegisterByKey("SSP-23", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("SSP-25", new VolcaronaSSP());
            r.RegisterByKey("SSP-26", new OricorioSSP("Fireworks"));
            r.RegisterByKey("SSP-28", new BillowingHeatWaveMon("Billowing Heat Wave", 30));
            r.RegisterByKey("SSP-29", new ConditionAttackMon("Heat Burn", SpecialCondition.Burned));
            r.RegisterByKey("SSP-30", new FlipBonusMon("Heat Breath", 50));
            r.RegisterByKey("SSP-31", new SkeledirgeSSP());
            r.RegisterByKey("SSP-33", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("SSP-34", new ArmarougeSSP());
            r.RegisterByKey("SSP-35", new CeruledgeSSP());
            r.RegisterByKey("SSP-37", new ScovillainExSSP());
            r.RegisterByKey("SSP-38", new CardsCRZ1.LimitBreakMon("Blazing Charge", 4, 70));
            r.RegisterByKey("SSP-39", new PaldeanTaurosWaterSSP());
            r.RegisterByKey("SSP-40", new CardsMEG.SnipeMon("Aqua Dive", 50));
            r.RegisterByKey("SSP-41", new SwitchSelfMon("Leap Out", false));
            r.RegisterByKey("SSP-42", new MiloticExSSP());
            r.RegisterByKey("SSP-43", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("SSP-45", new WalreinSSP());
            r.RegisterByKey("SSP-47", new CryogonalSSP());
            r.RegisterByKey("SSP-48", new BlackKyuremExSSP());
            r.RegisterByKey("SSP-49", new BruxishSSP());
            r.RegisterByKey("SSP-50", new FlipBonusMon("Aerial Ace", 20));
            r.RegisterByKey("SSP-52", new QuaquavalSSP());
            r.RegisterByKey("SSP-53", new CardsTWM1.HealSelfMon("Draining Fin", 20));
            r.RegisterByKey("SSP-54", new CardsDRI2.MudCoatMon(30));
            r.RegisterByKey("SSP-55", new CardsSTS3.SkyDropMon("Gusting Collision", 50));
            r.RegisterByKey("SSP-57", new PikachuExSSP());
            r.RegisterByKey("SSP-59", new MagnetonSSP());
            r.RegisterByKey("SSP-60", new MagnezoneSSP());
            r.RegisterByKey("SSP-61", new RotomSSP());
        }

        // ================================================================== Shared helpers (also used by CardsSSP2/3)

        public static bool IsBasicOf(CardDef d, EnergyType type) { return d.IsBasicEnergy && d.EnergyProvides == type; }

        /// <summary>Evolves the Pokémon with a card from the deck that evolves from it (an attack effect: no timing limits). Doesn't shuffle.</summary>
        public static bool EvolveFromDeck(EffectContext ctx, PokemonInPlay mon, string prompt)
        {
            if (mon == null || !ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) return false;
            string name = mon.Name;
            var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name && !ctx.Engine.Scripts.For(d).PutIntoPlayOnlyByEffect, 1, prompt, false);
            if (found.Count == 0) return false;
            ctx.Engine.EvolvePokemon(mon, found[0]);
            return true;
        }

        /// <summary>Energy Assist: attach up to N Basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        public static void EnergyAssist(EffectContext ctx, int max, string attack)
        {
            var energy = ctx.BasicEnergyInDiscard();
            if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return;
            var target = ctx.ChoosePokemon(ctx.MyBench(), attack + ": attach Basic Energy from your discard pile to which Benched Pokémon?");
            if (target == null) return;
            var picks = ctx.ChooseCards(energy, 1, Math.Min(max, energy.Count), attack + ": choose up to " + max + " Basic Energy to attach to " + target.Name);
            foreach (var c in picks)
                if (ctx.Me.Discard.Remove(c)) ctx.Engine.AttachEnergyCard(c, target);
        }

        /// <summary>Moves every Energy from one of your Pokémon to another.</summary>
        public static void MoveAllEnergy(EffectContext ctx, PokemonInPlay from, PokemonInPlay to)
        {
            if (from == null || to == null) return;
            foreach (var e in from.Energy.ToList()) ctx.Engine.MoveEnergy(from, to, e);
        }

        /// <summary>Type berries: damage from an opponent's [type] Pokémon's attack is 60 less, then the berry is discarded.</summary>
        public sealed class TypeBerry : CardScript
        {
            private readonly EnergyType type;

            public TypeBerry(EnergyType type) { this.type = type; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || damage <= 0) return damage;
                if (!src.Engine.IsType(info.Attacker, type) || !holder.HasTool(src.Card)) return damage;
                src.Log(src.Card.Def.Name + ": " + holder.Name + " takes 60 less damage.");
                src.Engine.DiscardTool(holder, src.Card);
                return damage - 60;
            }
        }

        // ================================================================== Grass

        /// <summary>Precocious Evolution: usable on the first player's first turn; evolve this Pokémon from your deck.</summary>
        private sealed class PrecociousExeggcuteSSP : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Precocious Evolution"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Precocious Evolution") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                EvolveFromDeck(ctx, ctx.Pokemon, "Precocious Evolution: choose a card that evolves from " + ctx.Pokemon.Name);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Barrage O'Clock: flip a coin per Energy on both Active Pokémon; 60 per heads.</summary>
        private sealed class ExeggutorSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Barrage O'Clock") return false;
                var defending = ctx.Defending;
                int coins = ctx.EnergyCount(ctx.Pokemon) + (defending == null ? 0 : ctx.EnergyCount(defending));
                ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }

        /// <summary>Sudden Shearing: played from the hand to the Bench, you may discard the top card of the opponent's deck. Vengeful Crush: +30 per Prize the opponent took.</summary>
        private sealed class DurantExSSP : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Sudden Shearing", ctx.Opp.Deck.Count > 0, "Sudden Shearing: discard the top card of your opponent's deck?"))
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vengeful Crush") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.OpponentPrizesTaken);
                return true;
            }
        }

        /// <summary>Evo-Powder: evolve each of your Benched Pokémon with a card from your deck.</summary>
        private sealed class VivillonSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evo-Powder") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                foreach (var benched in ctx.MyBench())
                    EvolveFromDeck(ctx, benched, "Evo-Powder: choose a card that evolves from " + benched.Name);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Calming Light: once during your turn, while Active, make the opponent's Active Pokémon Asleep. Spiral Rush: +30 per heads until tails.</summary>
        private sealed class ShiinoticSSP : Mon
        {
            protected override string Activated { get { return "Calming Light"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Rush") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>Rescue Kedge: up to 2 Pokémon from your discard pile into your hand. Annihilating Anchor: first discard the opponent's Active Pokémon's Tools.</summary>
        private sealed class DhelmiseSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Rescue Kedge")
                {
                    var mons = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                    if (mons.Count > 0)
                        ctx.DiscardToHand(ctx.ChooseCards(mons, 1, Math.Min(2, mons.Count), "Rescue Kedge: put up to 2 Pokémon into your hand"));
                    return true;
                }
                if (atk.Name == "Annihilating Anchor")
                {
                    var defending = ctx.Defending;
                    if (defending != null && defending.Tool != null && ctx.CanAffect(defending)) ctx.Engine.DiscardTool(defending);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Leaf Drain: heal 20. Jungle Whip: you may put all Energy from this Pokémon into your hand for 80 more damage.</summary>
        private sealed class ZarudeSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Leaf Drain")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 20);
                    return true;
                }
                if (atk.Name == "Jungle Whip")
                {
                    var me = ctx.Pokemon;
                    bool more = false;
                    if (me.Energy.Count > 0 && ctx.Engine.CanPutIntoHand(me)
                        && ctx.AskYesNo("Jungle Whip: put all Energy from " + me.Name + " into your hand for 80 more damage?"))
                    {
                        foreach (var e in me.Energy.ToList()) ctx.Engine.ReturnEnergyToHand(me, e);
                        more = true;
                    }
                    ctx.DamageDefending(atk.Damage + (more ? 80 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Triple Draw: draw 3. Counterturn: +200 with 3 or fewer cards in your deck.</summary>
        private sealed class RabscaSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Triple Draw")
                {
                    ctx.Draw(3);
                    return true;
                }
                if (atk.Name == "Counterturn")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Me.Deck.Count <= 3 ? 200 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hazardous Greed: with 3 or fewer cards in your deck, also 120 to 2 opposing Benched Pokémon. Entangling Whip: discard the top 3 cards of your deck.</summary>
        private sealed class WoChienSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hazardous Greed")
                {
                    bool greedy = ctx.Me.Deck.Count <= 3;
                    ctx.DamageDefending(atk.Damage);
                    var bench = ctx.OpponentBench();
                    if (!greedy || bench.Count == 0) return true;
                    var targets = ctx.ChoosePokemonMany(bench, Math.Min(2, bench.Count), 2, "Hazardous Greed: choose 2 of your opponent's Benched Pokémon (120 damage each)");
                    foreach (var t in targets) ctx.DamageTarget(t, 120);
                    return true;
                }
                if (atk.Name == "Entangling Whip")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        private sealed class HoOhSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shining Blaze") return false;
                bool tera = ctx.Me.Bench.Any(b => b.Def.IsTera);
                ctx.DamageDefending(atk.Damage + (tera ? 100 : 0));
                return true;
            }
        }

        /// <summary>Singe: Burned. Sunny Assist: move all Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class CastformSunnySSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Singe")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Burned);
                    return true;
                }
                if (atk.Name == "Sunny Assist")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Sunny Assist: move all Energy from " + me.Name + " to which Benched Pokémon?");
                    MoveAllEnergy(ctx, me, target);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Victory Cheer: your Evolution Fire Pokémon's attacks do 10 more damage to the opponent's Active Pokémon.</summary>
        private sealed class VictiniSSP : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive) return damage;
                return a.Def.IsEvolution && src.Engine.IsType(a, EnergyType.Fire) ? damage + 10 : damage;
            }
        }

        /// <summary>Leech Life: heal as much as the damage done. Turbulent Wings: 50 to itself.</summary>
        private sealed class VolcaronaSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Leech Life")
                {
                    int done = ctx.DamageDefending(atk.Damage);
                    if (done > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, done);
                    return true;
                }
                if (atk.Name == "Turbulent Wings")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(50);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Oricorio (both forms): Energy Assist, plus Fireworks (discard an Energy) or Dazzle Dance (Confused).</summary>
        public sealed class OricorioSSP : Mon
        {
            private readonly string second;

            public OricorioSSP(string second) { this.second = second; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Assist")
                {
                    EnergyAssist(ctx, 2, atk.Name);
                    return true;
                }
                if (atk.Name != second) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (second == "Fireworks") CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                else if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Billowing Heat Wave: also 30 to each of your own Benched Pokémon. Used for Billowing Heat Wave, Earthquake (the attack name and the numbers are passed in).</summary>
        internal sealed class BillowingHeatWaveMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BillowingHeatWaveMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var b in ctx.MyBench()) ctx.DamageTarget(b, amount);
                return true;
            }
        }

        /// <summary>Unaware: effects of the opponent's attacks don't affect this Pokémon. Torcherto: +20 per Benched Pokémon (both sides).</summary>
        private sealed class SkeledirgeSSP : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Torcherto") return false;
                ctx.DamageDefending(atk.Damage + 20 * (ctx.Me.Bench.Count + ctx.Opp.Bench.Count));
                return true;
            }
        }

        /// <summary>Crimson Blaster: discard all Fire Energy from this Pokémon; 180 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class ArmarougeSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crimson Blaster") return false;
                var me = ctx.Pokemon;
                foreach (var e in me.Energy.ToList())
                    if (ctx.Engine.EnergyUnitsOf(me, e).Any(u => EnergyMask.Provides(u, EnergyType.Fire))) ctx.Engine.DiscardEnergy(me, e);
                ctx.DamageOneBenched(180);
                return true;
            }
        }

        /// <summary>Cursed Edge: discard all Special Energy from all of the opponent's Pokémon. Black Blaze Slash: can't attack next turn.</summary>
        private sealed class CeruledgeSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cursed Edge")
                {
                    foreach (var m in ctx.OpponentPokemon())
                        foreach (var e in m.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(m, e);
                    return true;
                }
                if (atk.Name == "Black Blaze Slash")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Double Type: Grass and Fire type while in play. Spicy Rage: +70 per damage counter on this Pokémon.</summary>
        private sealed class ScovillainExSSP : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Grass)) types.Add(EnergyType.Grass);
                if (!types.Contains(EnergyType.Fire)) types.Add(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spicy Rage") return false;
                ctx.DamageDefending(atk.Damage + 70 * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Upthrusting Horns: you may put 2 Energy from the opponent's Active Stage 2 Pokémon into their hand.</summary>
        private sealed class PaldeanTaurosWaterSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Upthrusting Horns") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Def.Stage != Stage.Stage2 || defending.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Upthrusting Horns: put 2 Energy from " + defending.Name + " into your opponent's hand?")) return true;
                var picks = defending.Energy.Count <= 2 ? defending.Energy.ToList()
                    : ctx.ChooseCards(defending.Energy.ToList(), 2, 2, "Upthrusting Horns: choose 2 Energy");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(defending, e);
                return true;
            }
        }

        /// <summary>Sparkling Scales: prevent all damage and effects of attacks from the opponent's Tera Pokémon. Hypno Splash: Asleep.</summary>
        private sealed class MiloticExSSP : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && info.Attacker.Def.IsTera;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Def.IsTera;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypno Splash") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Frigid Fangs: during the opponent's next turn, their Pokémon with 2 or fewer Energy can't attack.</summary>
        private sealed class FrigidFangsEffect : ContinuousEffect
        {
            public FrigidFangsEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon)
            {
                return mon.Owner != Owner && engine.EnergyCount(mon) <= 2;
            }
        }

        private sealed class WalreinSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Frigid Fangs")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.AddEffect(new FrigidFangsEffect(ctx.Engine.Possessive(ctx.Opponent) + " Pokémon with 2 or fewer Energy can't attack next turn (Frigid Fangs)",
                        ctx.Player, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Megaton Fall")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(50);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Call Sign: search for a Pokémon. Ice Beam: flip; heads Paralyzed.</summary>
        private sealed class CryogonalSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Call Sign")
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsPokemon, 1, "Call Sign: choose a Pokémon");
                    return true;
                }
                if (atk.Name == "Ice Beam")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Ice Age: a Dragon Defending Pokémon is Paralyzed. Black Frost: 30 to itself.</summary>
        private sealed class BlackKyuremExSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Ice Age")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.Engine.IsType(defending, EnergyType.Dragon))
                        ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Black Frost")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Counterattack: damaged by an opponent's attack while Active, 3 damage counters on the Attacking Pokémon. Big Bite: no retreat.</summary>
        private sealed class BruxishSSP : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (ctx.Pokemon == null || !holderWasActive || attacker == null || !ctx.InPlay(attacker) || attacker.Owner == ctx.Player) return;
                ctx.Log("Counterattack (" + ctx.Pokemon.Name + ") puts 3 damage counters on " + attacker.Name + ".");
                ctx.PlaceCounters(attacker, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Up-Tempo: put a card from your hand on the bottom of your deck, then draw until you have 5.</summary>
        private sealed class QuaquavalSSP : Mon
        {
            protected override string Activated { get { return "Up-Tempo"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Count >= 1 && ctx.Me.Hand.Count - 1 < 5 && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Up-Tempo: put a card from your hand on the bottom of your deck")[0];
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card on the bottom of " + ctx.Engine.Their(ctx.Player) + " deck.");
                ctx.DrawUntil(5);
            }
        }

        // ================================================================== Lightning

        /// <summary>Resolute Heart: from full HP, survives an attack's Knock Out with 10 HP. Topaz Bolt: discard 3 Energy.</summary>
        private sealed class PikachuExSSP : Mon
        {
            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Topaz Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 3, atk.Name);
                return true;
            }
        }

        /// <summary>Overvolt Discharge: attach up to 3 Basic Energy from your discard pile to your Lightning Pokémon, then this Pokémon is Knocked Out.</summary>
        private sealed class MagnetonSSP : Mon
        {
            protected override string Activated { get { return "Overvolt Discharge"; } }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == "Overvolt Discharge"; }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard().Count > 0 && ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Lightning)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), 3, (c, m) => ctx.Engine.IsType(m, EnergyType.Lightning),
                    "Overvolt Discharge: attach a Basic Energy to 1 of your Lightning Pokémon");
                ctx.Engine.KnockOut(ctx.Pokemon, "Overvolt Discharge");
            }
        }

        /// <summary>Mighty Magnetism: Confused and can't retreat. Zap Cannon: not again next turn.</summary>
        private sealed class MagnezoneSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mighty Magnetism")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending)) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Zap Cannon")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantUseAttackNextTurn("Zap Cannon");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Crushing Pulse: the opponent reveals their hand; discard every Item and Pokémon Tool in it. Energy Short: 20 per Energy on the opponent's Active Pokémon.</summary>
        private sealed class RotomSSP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Crushing Pulse")
                {
                    CardsTWM1.RevealOpponentHand(ctx);
                    foreach (var c in ctx.Opp.Hand.Where(x => x.Def.IsItem || x.Def.IsTool).ToList()) ctx.Engine.DiscardFromHand(ctx.Opponent, c);
                    return true;
                }
                if (atk.Name == "Energy Short")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage * (defending == null ? 0 : ctx.EnergyCount(defending)));
                    return true;
                }
                return false;
            }
        }
    }
}
