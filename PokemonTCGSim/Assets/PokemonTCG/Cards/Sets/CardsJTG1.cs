using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Journey Together (JTG) 1-70. Pokémon whose attacks only do their printed damage need no script:
    /// Caterpie, Petilil, Nymble, Torchic, Combusken, Torkoal, Larvesta, Volcarona, Lotad, Wingull, Pelipper,
    /// Wailmer, Alolan Geodude, Alolan Graveler, Iono's Tadbulb, Shuppet, Beldum, Metang.
    /// </summary>
    public static class CardsJTG1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("JTG-2", new MetapodJTG());
            r.RegisterByKey("JTG-3", new ButterfreeJTG());
            r.RegisterByKey("JTG-4", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("JTG-5", new ParasectJTG());
            r.RegisterByKey("JTG-7", new LilligantJTG());
            r.RegisterByKey("JTG-8", new MaractusJTG());
            r.RegisterByKey("JTG-9", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("JTG-10", new ConditionAttackMon("Spore Ball", SpecialCondition.Asleep));
            r.RegisterByKey("JTG-11", new AmoongussExJTG());
            r.RegisterByKey("JTG-12", new ShelmetJTG());
            r.RegisterByKey("JTG-13", new AccelgorJTG());
            r.RegisterByKey("JTG-14", new DurantJTG());
            r.RegisterByKey("JTG-15", new CardsTWM1.HealSelfMon("Leaf Drain", 30));
            r.RegisterByKey("JTG-16", new CoinsMon("Tons of Treading", 3));
            r.RegisterByKey("JTG-17", new CardsFFI1.MagicalLeafThirtyMon());
            r.RegisterByKey("JTG-18", new MeowscaradaJTG());
            r.RegisterByKey("JTG-20", new MagmarJTG());
            r.RegisterByKey("JTG-21", new MagmortarJTG());
            r.RegisterByKey("JTG-30", new ReshiramExJTG());
            r.RegisterByKey("JTG-31", new VolcanionExJTG());
            r.RegisterByKey("JTG-32", new ArticunoJTG());
            r.RegisterByKey("JTG-33", new CardsBLK.FlailMon("Flail"));
            r.RegisterByKey("JTG-34", new OctilleryJTG());
            r.RegisterByKey("JTG-36", new LockAfterAttackMon("Aqua Slash", false));
            r.RegisterByKey("JTG-37", new LudicoloJTG());
            r.RegisterByKey("JTG-41", new WailordJTG());
            r.RegisterByKey("JTG-42", new RegiceJTG());
            r.RegisterByKey("JTG-43", new VeluzaExJTG());
            r.RegisterByKey("JTG-46", new AlolanGolemJTG());
            r.RegisterByKey("JTG-47", new IonosVoltorbJTG());
            r.RegisterByKey("JTG-48", new IonosElectrodeJTG());
            r.RegisterByKey("JTG-49", new NsJoltikJTG());
            r.RegisterByKey("JTG-50", new DodgeMon("Toge Spark", false));
            r.RegisterByKey("JTG-51", new CardsFCO3.DoTheWaveMon("Linked Lightning", 20));
            r.RegisterByKey("JTG-53", new IonosBelliboltExJTG());
            r.RegisterByKey("JTG-54", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("JTG-55", new IonosKilowattrelJTG());
            r.RegisterByKey("JTG-57", new CardsSTS2.RetaliateMon("Retaliate", 90));
            r.RegisterByKey("JTG-58", new MrMimeJTG());
            r.RegisterByKey("JTG-60", new BanetteJTG());
            r.RegisterByKey("JTG-63", new MetagrossJTG());
            r.RegisterByKey("JTG-64", new NsSigilyphJTG());
            r.RegisterByKey("JTG-65", new CallForFamilyMon());
            r.RegisterByKey("JTG-66", new HoldStillMon("Hold Still", 10));
            r.RegisterByKey("JTG-67", new LilliesRibombeeJTG());
            r.RegisterByKey("JTG-68", new LilliesComfeyJTG());
            r.RegisterByKey("JTG-69", new MimikyuExJTG());
            r.RegisterByKey("JTG-70", new NoRetreatMon("Bind Down"));
        }

        // ================================================================== Shared helpers

        private static bool IsIonos(CardDef d) { return d != null && d.IsOwnersPokemon("Iono"); }

        private static bool IsBasicLightning(CardDef d) { return d != null && d.IsBasicEnergy && d.EnergyProvides == EnergyType.Lightning; }

        /// <summary>Searing Flame (Magmar, Magmortar): damage, then flip; if heads the opponent's Active Pokémon is Burned.</summary>
        private static bool SearingFlame(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
            return true;
        }

        /// <summary>Harden: during the opponent's next turn, prevent damage from attacks to this Pokémon if it's 60 or less.</summary>
        private sealed class HardenEffect : ContinuousEffect
        {
            private readonly int threshold;

            public HardenEffect(string name, int owner, int targetUid, int expiresAfterTurn, int threshold)
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

        private sealed class MetapodJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Harden") return false;
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new HardenEffect(ctx.Pokemon.Name + " takes no damage of 60 or less from attacks during the opponent's next turn (Harden)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 60));
                return true;
            }
        }

        private sealed class ButterfreeJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scale Hurricane") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        private sealed class ParasectJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Spore")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name == "Scissor Swing")
                {
                    ctx.DamageDefending(atk.Damage + 30 * ctx.FlipCoins(2));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Sunny Day: attacks of your Grass and Fire Pokémon do 20 more damage to the opponent's Active Pokémon.</summary>
        private sealed class LilligantJTG : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive) return damage;
                bool fits = src.Engine.IsType(a, EnergyType.Grass) || src.Engine.IsType(a, EnergyType.Fire);
                return fits ? damage + 20 : damage;
            }
        }

        /// <summary>Exploding Needles: Knocked Out in the Active Spot by an opponent's attack damage -> 6 damage counters on the Attacking Pokémon.</summary>
        private sealed class MaractusJTG : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var mon = src.Pokemon;
                if (mon == null || !byOpponentsAttack || !wasActive) return;
                // The Pokémon that used the attack, wherever it is now (Bench attacks, or it switched itself out).
                var attacker = src.Engine.AttackingPokemonOf(mon);
                if (attacker == null || attacker.Owner == src.Player || src.Engine.IsKnockedOut(attacker)) return;
                src.Log("Exploding Needles (" + mon.Name + ") puts 6 damage counters on " + attacker.Name + ".");
                src.PlaceCounters(attacker, 6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Corner") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        private sealed class AmoongussExJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Spore Ball")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name == "Champignon's Swing")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 80 : 0));
                    return true;
                }
                return false;
            }
        }

        private sealed class ShelmetJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shell Smack") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new PreventDamageEffect(ctx.Pokemon.Name + " takes no damage from attacks during the opponent's next turn (Shell Smack)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class AccelgorJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poisonous Ploy") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        private sealed class DurantJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crunch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Crunch: discard an Energy from your opponent's Active Pokémon");
                return true;
            }
        }

        /// <summary>Showtime: once during your turn, if this Pokémon is on the Bench, switch it with your Active Pokémon.</summary>
        private sealed class MeowscaradaJTG : Mon
        {
            protected override string Activated { get { return "Showtime"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rising Bloom") return false;
                var d = ctx.Defending;
                bool ex = d != null && (d.Def.IsEx || d.Def.IsMegaEx);
                ctx.DamageDefending(atk.Damage + (ex ? 90 : 0));
                return true;
            }
        }

        // ================================================================== Fire

        private sealed class MagmarJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searing Flame") return false;
                return SearingFlame(ctx, atk);
            }
        }

        /// <summary>Magma Surge: Burn puts 3 more damage counters on the opponent's Pokémon in every Pokémon Checkup.</summary>
        private sealed class MagmortarJTG : Mon
        {
            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return src.Pokemon != null && mon.Owner != src.Player ? counters + 3 : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searing Flame") return false;
                return SearingFlame(ctx, atk);
            }
        }

        private sealed class ReshiramExJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Fire") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Scorching Fire: discard an Energy from this Pokémon");
                return true;
            }
        }

        /// <summary>Scalding Steam: once during your turn, if Active, Burn the opponent's Active Pokémon. Scorching Cyclone: move an Energy to the Bench.</summary>
        private sealed class VolcanionExJTG : Mon
        {
            protected override string Activated { get { return "Scalding Steam"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Defending != null && !ctx.Defending.Has(SpecialCondition.Burned);
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.ApplyCondition(ctx.Defending, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                var self = ctx.Pokemon;
                if (!ctx.InPlay(self) || self.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.ChooseCards(self.Energy.ToList(), 1, 1, "Scorching Cyclone: choose an Energy to move from this Pokémon")[0];
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Scorching Cyclone: move " + energy.Def.Name + " to which Benched Pokémon?");
                ctx.Engine.MoveEnergy(self, to, energy);
                return true;
            }
        }

        // ================================================================== Water

        private sealed class ArticunoJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frigid Fluttering") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Water, 2,
                    "Frigid Fluttering: choose up to 2 Basic Water Energy to attach to this Pokémon");
                foreach (var e in found) ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        private sealed class OctilleryJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Aqua Wash")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                    if (!ctx.AskYesNo("Aqua Wash: put an Energy attached to your opponent's Active Pokémon into their hand?")) return true;
                    var energy = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Aqua Wash: choose the Energy to put into your opponent's hand")[0];
                    ctx.Engine.ReturnEnergyToHand(defending, energy);
                    return true;
                }
                if (atk.Name == "Octo Beatdown")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                    return true;
                }
                return false;
            }
        }

        /// <summary>Vibrant Dance: all of your Pokémon get +40 HP. Doesn't stack: only the first working Vibrant Dance counts.</summary>
        private sealed class LudicoloJTG : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return hp;
                var first = src.Me.InPlay().FirstOrDefault(p => p.Def.Abilities.Any(a => a.Name == "Vibrant Dance") && src.Engine.HasAbilityNamed(p, "Vibrant Dance"));
                return first == src.Pokemon ? hp + 40 : hp;
            }
        }

        private sealed class WailordJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Pump") return false;
                ctx.DamageDefending(atk.Damage + 50 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water));
                return true;
            }
        }

        private sealed class RegiceJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blizzard") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var benched in ctx.OpponentBench()) ctx.DamageTarget(benched, 10);
                return true;
            }
        }

        private sealed class VeluzaExJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Purging Strike") return false;
                int discarded = 0;
                if (ctx.Me.Hand.Count > 0 && ctx.AskYesNo("Purging Strike: discard your hand (" + ctx.Me.Hand.Count + " cards) for 120 more damage?"))
                {
                    discarded = ctx.Me.Hand.Count;
                    ctx.Engine.DiscardHand(ctx.Player);
                }
                ctx.DamageDefending(atk.Damage + (discarded > 0 ? 120 : 0));
                return true;
            }
        }

        // ================================================================== Lightning

        private sealed class AlolanGolemJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Electromagnetic Catapult")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                    return true;
                }
                if (atk.Name == "Megaton Fall")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(40);
                    return true;
                }
                return false;
            }
        }

        private sealed class IonosVoltorbJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Voltaic Chain") return false;
                var ionos = ctx.MyPokemon(p => IsIonos(p.Def));
                int lightning = ionos.Sum(q => ctx.Engine.EnergyCountOfType(q, EnergyType.Lightning));
                ctx.DamageDefending(atk.Damage + 20 * lightning);
                return true;
            }
        }

        private sealed class IonosElectrodeJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thump-Thump Boom") return false;
                var defending = ctx.Defending;
                ctx.DamageSelf(100);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.Engine.KnockOut(defending, "Thump-Thump Boom");
                return true;
            }
        }

        private sealed class NsJoltikJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zapping Short") return false;
                var defending = ctx.Defending;
                bool discarded = false;
                if (defending != null && defending.Tool != null)
                {
                    if (ctx.CanAffect(defending)) discarded = ctx.Engine.DiscardTool(defending);
                    else ctx.Log("The effect on " + defending.Name + " was prevented.");
                }
                if (discarded) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Electric Streamer: as often as you like, attach a Basic Lightning Energy from hand to 1 of your Iono's Pokémon.</summary>
        private sealed class IonosBelliboltExJTG : Mon
        {
            private const string Streamer = "Electric Streamer";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Streamer || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(c => IsBasicLightning(c.Def)) && ctx.Me.InPlay().Any(p => IsIonos(p.Def));
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing attaches as many as they like, then presses Done.
                if (RepeatableAbility.ForPerson(ctx))
                {
                    RepeatableAbility.AttachFromHand(ctx, IsBasicLightning, () => ctx.MyPokemon(p => IsIonos(p.Def)), Streamer, "a Basic Lightning Energy");
                    return;
                }
                var energy = ctx.Me.Hand.Where(c => IsBasicLightning(c.Def)).ToList();
                var targets = ctx.MyPokemon(p => IsIonos(p.Def));
                if (energy.Count == 0 || targets.Count == 0) return;
                var card = ctx.ChooseCards(energy, 1, 1, "Electric Streamer: choose a Basic Lightning Energy from your hand")[0];
                var target = ctx.ChoosePokemon(targets, "Electric Streamer: attach " + card.Def.Name + " to which of your Iono's Pokémon?");
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderous Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Flashing Draw: discard a Basic Lightning Energy from this Pokémon to draw until you have 6 cards (once per turn).</summary>
        private sealed class IonosKilowattrelJTG : Mon
        {
            protected override string Activated { get { return "Flashing Draw"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0 && ctx.Pokemon.Energy.Any(e => IsBasicLightning(e.Def));
            }

            protected override void Ability(EffectContext ctx)
            {
                var energy = ctx.Pokemon.Energy.Where(e => IsBasicLightning(e.Def)).ToList();
                if (energy.Count == 0) return;
                var pick = ctx.ChooseCards(energy, 1, 1, "Flashing Draw: discard a Basic Lightning Energy from this Pokémon")[0];
                if (!ctx.Engine.DiscardEnergy(ctx.Pokemon, pick)) return;
                ctx.DrawUntil(6);
            }
        }

        // ================================================================== Psychic

        private sealed class MrMimeJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mimic")
                {
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    ctx.Draw(ctx.Opp.Hand.Count);
                    return true;
                }
                if (atk.Name == "Psy Bolt")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                return false;
            }
        }

        private sealed class BanetteJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cursed Words") return false;
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                int n = Math.Min(3, hand.Count);
                var picks = n >= hand.Count ? hand
                    : ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, "Cursed Words: choose 3 cards from your hand to shuffle into your deck", ctx.Source);
                foreach (var c in picks)
                {
                    ctx.Opp.Hand.Remove(c);
                    ctx.Opp.Deck.Add(c);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + picks.Count + (picks.Count == 1 ? " card" : " cards") + " from their hand into their deck.");
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        private sealed class MetagrossJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Conjoined Beams") return false;
                bool both = ctx.Me.Bench.Any(b => b.Name == "Beldum") && ctx.Me.Bench.Any(b => b.Name == "Metang");
                ctx.DamageDefending(atk.Damage + (both ? 150 : 0));
                return true;
            }
        }

        private sealed class NsSigilyphJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Victory Symbol") return false;
                if (ctx.Me.Prizes.Count == 1)
                    ctx.Engine.EndGame(ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " used Victory Symbol with exactly 1 Prize card remaining.");
                else
                    ctx.Log("Victory Symbol did nothing (" + ctx.Me.Prizes.Count + " Prize cards remaining).");
                return true;
            }
        }

        /// <summary>Used for Hold Still, Calm Mind (the attack name and the numbers are passed in).</summary>
        internal sealed class HoldStillMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HoldStillMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.Heal(ctx.Pokemon, amount);
                return true;
            }
        }

        /// <summary>Inviting Wink (when played to evolve): the opponent reveals their hand; put any number of Basic Pokémon from it onto their Bench.</summary>
        private sealed class LilliesRibombeeJTG : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.Opp.Hand.Count > 0;
                if (!TriggerOnce(ctx, "Inviting Wink", ready, "Use Inviting Wink: have your opponent reveal their hand and put any number of Basic Pokémon from it onto their Bench?")) return;
                TefPokemon.RevealOpponentHand(ctx);
                int room = ctx.Engine.BenchSpace(ctx.Opponent);
                var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon).ToList();
                if (room <= 0 || basics.Count == 0) return;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(room, basics.Count), "Inviting Wink: put any number of Basic Pokémon from your opponent's hand onto their Bench");
                foreach (var c in picks)
                {
                    if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) break;
                    ctx.Opp.Hand.Remove(c);
                    ctx.Engine.PutOntoBench(ctx.Opponent, c, false);
                }
            }
        }

        private sealed class LilliesComfeyJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Inviting Flowers")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => d.IsOwnersPokemon("Lillie"), ctx.BenchSpace, "Inviting Flowers: put any number of Basic Lillie's Pokémon onto your Bench");
                    return true;
                }
                if (atk.Name == "Fade Out")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        private sealed class MimikyuExJTG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mischievous Hands")
                {
                    var theirs = ctx.OpponentPokemon();
                    if (theirs.Count == 0) return true;
                    int n = Math.Min(2, theirs.Count);
                    var picks = ctx.ChoosePokemonMany(theirs, n, n, "Mischievous Hands: choose 2 of your opponent's Pokémon (3 damage counters on each)");
                    foreach (var t in picks) ctx.PlaceCounters(t, 3);
                    return true;
                }
                if (atk.Name == "Ghostly Trip")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }
    }
}
