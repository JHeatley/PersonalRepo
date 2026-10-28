using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Shrouded Fable (SFA). Pokémon whose attacks only do their printed damage need no script:
    /// Houndour, Cufant, Axew.
    /// </summary>
    public static class CardsSFA
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SFA-1", new DodgeMon("Splashing Dodge", false));
            r.RegisterByKey("SFA-2", new GalvantulaSFA());
            r.RegisterByKey("SFA-3", new DrawAttackMon("Add On", 1));
            r.RegisterByKey("SFA-4", new DartrixSFA());
            r.RegisterByKey("SFA-5", new DecidueyeSFA());
            r.RegisterByKey("SFA-8", new HoundoomSFA());
            r.RegisterByKey("SFA-9", new IronMothSFA());
            r.RegisterByKey("SFA-10", new CardsTWM1.HealSelfMon("Hold Still", 30));
            r.RegisterByKey("SFA-11", new SearchAttackMon("Call for Backup", d => d.IsPokemon, 3));
            r.RegisterByKey("SFA-12", new KingdraExSFA());
            r.RegisterByKey("SFA-13", new NoRetreatMon("Beset"));
            r.RegisterByKey("SFA-14", new WeavileSFA());
            r.RegisterByKey("SFA-15", new RevavroomExSFA());
            r.RegisterByKey("SFA-16", new DrowzeeSFA());
            r.RegisterByKey("SFA-17", new HypnoSFA());
            r.RegisterByKey("SFA-21", new CresseliaSFA());
            r.RegisterByKey("SFA-22", new SylveonSFA());
            r.RegisterByKey("SFA-23", new FlipBonusMon("Frog Hop", 20));
            r.RegisterByKey("SFA-24", new CardsOBF2.DaringStrikeMon("Clean Hit", 90));
            r.RegisterByKey("SFA-25", new BloodmoonUrsalunaSFA());
            r.RegisterByKey("SFA-26", new SlitherWingSFA());
            r.RegisterByKey("SFA-27", new SearchAttackMon("Lead", d => d.IsSupporter, 1));
            r.RegisterByKey("SFA-28", new LockAfterAttackMon("Pitch-Black Blade", false));
            r.RegisterByKey("SFA-29", new CrobatSFA());
            r.RegisterByKey("SFA-30", new AbsolSFA());
            r.RegisterByKey("SFA-31", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("SFA-32", new ZoroarkSFA());
            r.RegisterByKey("SFA-33", new InkaySFA());
            r.RegisterByKey("SFA-34", new MalamarSFA());
            r.RegisterByKey("SFA-35", new YveltalSFA());
            r.RegisterByKey("SFA-36", new OkidogiExSFA());
            r.RegisterByKey("SFA-37", new MunkidoriExSFA());
            r.RegisterByKey("SFA-42", new CopperajahSFA());
            r.RegisterByKey("SFA-43", new ShieldAttackMon("Rigidify", 30));
            r.RegisterByKey("SFA-45", new FraxureSFA());
            r.RegisterByKey("SFA-46", new HaxorusSFA());
            r.RegisterByKey("SFA-48", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("SFA-49", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("SFA-50", new EeveeSFA());
            r.RegisterByKey("SFA-51", new FurfrouSFA());
            r.RegisterByKey("SFA-52", new RecoilMon("Take Down", 10));
            r.RegisterByKey("SFA-53", new BewearSFA());

            r.RegisterByName("Cassiopeia", new Cassiopeia());
            r.RegisterByName("Colress's Tenacity", new ColresssTenacity());
            r.RegisterByName("Dangerous Laser", new DangerousLaser());
            r.RegisterByName("Neutralization Zone", new NeutralizationZone());
            r.RegisterByName("Poké Vital A", new PokeVitalA());
        }

        private static bool IsBasic(CardDef d, EnergyType type) { return d.IsBasicEnergy && d.EnergyProvides == type; }

        // ================================================================== Grass

        /// <summary>Compound Eyes: +50 against an opposing Active Pokémon that has an Ability. Shocking Web: +80 with Lightning Energy attached.</summary>
        private sealed class GalvantulaSFA : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive) return damage;
                return src.Engine.HasAbility(info.Target) ? damage + 50 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shocking Web") return false;
                bool charged = ctx.Engine.HasEnergyOfType(ctx.Pokemon, EnergyType.Lightning);
                ctx.DamageDefending(atk.Damage + (charged ? 80 : 0));
                return true;
            }
        }

        /// <summary>United Wings: 20 per Pokémon in your discard pile that has the United Wings attack.</summary>
        private sealed class DartrixSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "United Wings") return false;
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.Attacks.Any(a => a.Name == "United Wings"));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Stock Up on Feathers: draw until you have 7. Power Shot: discard a Basic Grass Energy from your hand, or the attack does nothing.</summary>
        private sealed class DecidueyeSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Stock Up on Feathers")
                {
                    ctx.DrawUntil(7);
                    return true;
                }
                if (atk.Name == "Power Shot")
                {
                    var grass = ctx.Me.Hand.Where(c => IsBasic(c.Def, EnergyType.Grass)).ToList();
                    if (grass.Count == 0) return Nothing(ctx);
                    var card = ctx.ChooseCards(grass, 1, 1, "Power Shot: discard a Basic Grass Energy from your hand")[0];
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>Snarl: the Defending Pokémon's attacks do 100 less damage during the opponent's next turn.</summary>
        private sealed class HoundoomSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snarl") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 100 less damage during its next turn (Snarl)",
                        ctx.Player, defending.Uid, ctx.Turn + 1, 100));
                return true;
            }
        }

        /// <summary>Prevents damage from attacks of Ancient Pokémon to one Pokémon.</summary>
        private sealed class AncientShieldEffect : ContinuousEffect
        {
            public AncientShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && info.Attacker.Def.IsAncient ? 0 : damage;
            }
        }

        /// <summary>Suction: heal as much as the damage done. Anachronism Repulsor: no damage from Ancient Pokémon's attacks next turn.</summary>
        private sealed class IronMothSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Suction")
                {
                    int done = ctx.DamageDefending(atk.Damage);
                    if (done > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, done);
                    return true;
                }
                if (atk.Name == "Anachronism Repulsor")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon))
                        ctx.AddEffect(new AncientShieldEffect(ctx.Pokemon.Name + " takes no damage from Ancient Pokémon's attacks during the opponent's next turn (Anachronism Repulsor)",
                            ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        /// <summary>King's Order: up to 3 Water Pokémon from your discard pile onto your Bench, Evolution Pokémon too (your ruling). Hydro Pump: +50 per Water Energy.</summary>
        private sealed class KingdraExSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "King's Order")
                {
                    var water = ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Water)).ToList(); // Evolution Pokémon too
                    int room = ctx.BenchSpace;
                    if (water.Count == 0 || room == 0) return true;
                    var picks = ctx.ChooseCards(water, 1, Math.Min(3, Math.Min(room, water.Count)), "King's Order: put up to 3 Water Pokémon onto your Bench");
                    foreach (var c in picks)
                    {
                        if (!ctx.Me.Discard.Remove(c)) continue;
                        ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    }
                    return true;
                }
                if (atk.Name == "Hydro Pump")
                {
                    ctx.DamageDefending(atk.Damage + 50 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hail Claw: discard all Energy from this Pokémon; the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class WeavileSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hail Claw") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Accelerator Flash: +120 if this Pokémon moved from the Bench to the Active Spot this turn. Shattering Speed: then discard this Pokémon and everything attached.</summary>
        private sealed class RevavroomExSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Accelerator Flash")
                {
                    bool moved = ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn;
                    ctx.DamageDefending(atk.Damage + (moved ? 120 : 0));
                    return true;
                }
                if (atk.Name == "Shattering Speed")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardPokemonFromPlay(ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Psychic

        private sealed class DrowzeeSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eerie Gaze") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Daydream: if the opponent attaches an Energy card from their hand to the Defending Pokémon next turn, their turn ends.</summary>
        private sealed class DaydreamEffect : ContinuousEffect
        {
            public DaydreamEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnEnergyAttachedFromHand(GameEngine engine, CardInstance energy, PokemonInPlay target)
            {
                if (target.Uid == TargetUid && engine.State.CurrentPlayer == target.Owner) engine.RequestEndTurn();
            }
        }

        private sealed class HypnoSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Daydream") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new DaydreamEffect("If " + defending.Name + " gets an Energy from the hand next turn, that turn ends (Daydream)",
                        ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Healing Pirouette: heal 20 from each of your Pokémon. Crescent Purge: you may turn 1 face-down Prize card face up for 80 more damage.</summary>
        private sealed class CresseliaSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Healing Pirouette")
                {
                    foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 20);
                    return true;
                }
                if (atk.Name == "Crescent Purge")
                {
                    var me = ctx.Me;
                    var faceDown = me.Prizes.Where(c => !me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                    bool purge = false;
                    if (faceDown.Count > 0)
                    {
                        var labels = new List<string> { "Don't turn a Prize card face up" };
                        for (int i = 0; i < faceDown.Count; i++) labels.Add("Turn face-down Prize card " + (i + 1) + " face up (+80 damage)");
                        int pick = ctx.ChooseOption("Crescent Purge: turn 1 of your face-down Prize cards face up?", labels.ToArray());
                        if (pick > 0)
                        {
                            var card = faceDown[pick - 1];
                            me.FaceUpPrizeIds.Add(card.Id);
                            ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "turned a Prize card face up");
                            purge = true;
                        }
                    }
                    ctx.DamageDefending(atk.Damage + (purge ? 80 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Mystical Return: flip; heads shuffles 1 of the opponent's Benched Pokémon into their deck. Disarming Voice: Confused.</summary>
        private sealed class SylveonSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Mystical Return")
                {
                    if (!ctx.Flip()) return true;
                    var targets = ctx.GustTargets();
                    if (targets.Count == 0) return true;
                    var target = ctx.ChoosePokemon(targets, "Mystical Return: shuffle which of your opponent's Benched Pokémon into their deck?");
                    if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
                    return true;
                }
                if (atk.Name == "Disarming Voice")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fighting

        /// <summary>Battle-Hardened: played from the hand to the Bench, you may attach up to 2 Basic Fighting Energy from your hand to it. Mad Bite: +30 per damage counter.</summary>
        private sealed class BloodmoonUrsalunaSFA : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                var fighting = ctx.Me.Hand.Where(c => IsBasic(c.Def, EnergyType.Fighting)).ToList();
                if (!TriggerOnce(ctx, "Battle-Hardened", fighting.Count > 0, "Battle-Hardened: attach up to 2 Basic Fighting Energy from your hand to " + mon.Name + "?"))
                    return;
                var picks = ctx.ChooseCards(fighting, 1, Math.Min(2, fighting.Count), "Battle-Hardened: choose up to 2 Basic Fighting Energy");
                foreach (var c in picks)
                {
                    if (!ctx.Me.Hand.Remove(c)) continue;
                    ctx.Engine.AttachEnergyCard(c, mon, true);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mad Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 30 * (defending == null ? 0 : defending.DamageCounters));
                return true;
            }
        }

        /// <summary>Iron Smasher: +120 if the opponent has a Future Pokémon in play. Smashing Wing: discard 2 Energy from this Pokémon.</summary>
        private sealed class SlitherWingSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Iron Smasher")
                {
                    bool future = ctx.Opp.InPlay().Any(m => m.Def.IsFuture);
                    ctx.DamageDefending(atk.Damage + (future ? 120 : 0));
                    return true;
                }
                if (atk.Name == "Smashing Wing")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Darkness

        /// <summary>Shadowy Envoy: after playing Janine's Secret Art this turn, draw until you have 8. Poison Fang: Poisoned, 2 counters each Checkup.</summary>
        private sealed class CrobatSFA : Mon
        {
            protected override string Activated { get { return "Shadowy Envoy"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.SupporterNameThisTurn == "Janine's Secret Art" && ctx.Me.Hand.Count < 8 && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(8); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Fang") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned))
                    defending.PoisonCounterBase = 2;
                return true;
            }
        }

        /// <summary>Darkfall: +50 if you have at least 3 Darkness Energy in play.</summary>
        private sealed class AbsolSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkfall") return false;
                int dark = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Darkness));
                ctx.DamageDefending(atk.Damage + (dark >= 3 ? 50 : 0));
                return true;
            }
        }

        /// <summary>Illusory Hijacking: 60 per Pokémon ex and Pokémon V your opponent has in play.</summary>
        private sealed class ZoroarkSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Illusory Hijacking") return false;
                ctx.DamageDefending(atk.Damage * ctx.OpponentPokemon(m => TefPokemon.IsExOrV(m.Def)).Count);
                return true;
            }
        }

        /// <summary>Mischievous Tentacles: look at the top card of the opponent's deck; you may have them shuffle their deck.</summary>
        private sealed class InkaySFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mischievous Tentacles") return false;
                if (ctx.Opp.Deck.Count == 0) return true;
                var top = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.NoteLookedAt(top, "Top card of your opponent's deck", ctx.Player);
                if (ctx.AskYesNo("Mischievous Tentacles: the top card of your opponent's deck is " + top[0].Def.Name + ". Have your opponent shuffle their deck?"))
                    ctx.Engine.ShuffleDeck(ctx.Opponent);
                else
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of the other player's deck and left it there.");
                return true;
            }
        }

        /// <summary>Colluding Tentacles: only after playing Xerosic's Machinations this turn; switch in 1 of the opponent's Benched Pokémon and do 120 to it.</summary>
        private sealed class MalamarSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colluding Tentacles") return false;
                if (ctx.Me.SupporterNameThisTurn != "Xerosic's Machinations") return Nothing(ctx);
                if (ctx.GustOpponent("Colluding Tentacles: choose 1 of your opponent's Benched Pokémon to switch in") != null) ctx.DamageDefending(120);
                return true;
            }
        }

        /// <summary>Corrosive Winds: 2 damage counters on each opposing Pokémon that has damage counters. Destructive Beam: flip, discard an Energy.</summary>
        private sealed class YveltalSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Corrosive Winds")
                {
                    foreach (var m in ctx.OpponentPokemon(x => x.DamageCounters > 0)) ctx.PlaceCounters(m, 2);
                    return true;
                }
                if (atk.Name == "Destructive Beam")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                        ctx.DiscardEnergyFrom(defending, 1, "Destructive Beam: discard an Energy from " + defending.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Poisonous Musculature: attach up to 2 Basic Darkness Energy from your deck to this Pokémon; if you did, it's Poisoned. Chain-Crazed: +130 while Poisoned.</summary>
        private sealed class OkidogiExSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Poisonous Musculature")
                {
                    var me = ctx.Pokemon;
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => IsBasic(d, EnergyType.Darkness), 2, "Poisonous Musculature: choose up to 2 Basic Darkness Energy");
                    foreach (var c in found) ctx.Engine.AttachEnergyCard(c, me);
                    ctx.ShuffleMyDeck();
                    if (found.Count > 0 && ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Poisoned);
                    return true;
                }
                if (atk.Name == "Chain-Crazed")
                {
                    bool poisoned = ctx.Pokemon.Has(SpecialCondition.Poisoned);
                    ctx.DamageDefending(atk.Damage + (poisoned ? 130 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Oh No You Don't: Knocked Out by an opponent's attack damage while you have Pecharunt ex in play, they take 1 fewer Prize. Dirty Headbutt: not again next turn.</summary>
        private sealed class MunkidoriExSFA : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut != src.Pokemon || taker == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                if (!src.Me.InPlay().Any(m => m.Name == "Pecharunt ex")) return prizes;
                return prizes - 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dirty Headbutt") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn("Dirty Headbutt");
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Massive Body: while Active, the opponent can't play Stadium cards. Nasal Lariat: you may do 100 more; then it can't attack next turn.</summary>
        private sealed class CopperajahSFA : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && src.PokemonIsActive && player != src.Player && card.Def.IsStadium;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nasal Lariat") return false;
                bool more = ctx.AskYesNo("Nasal Lariat: do 100 more damage? (" + ctx.Pokemon.Name + " can't attack during your next turn)");
                ctx.DamageDefending(atk.Damage + (more ? 100 : 0));
                if (more) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Unnerve: effects of Items and Supporters the opponent plays from their hand don't affect this Pokémon. Dragon Pulse: discard the top card of your deck.</summary>
        private sealed class FraxureSFA : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && (trainer.IsItem || trainer.IsSupporter);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                return true;
            }
        }

        /// <summary>Bring Down the Axe: Knock Out the opponent's Active Pokémon if it has Special Energy. Dragon Pulse: discard the top 3 cards of your deck.</summary>
        private sealed class HaxorusSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Bring Down the Axe")
                {
                    var defending = ctx.Defending;
                    if (defending != null && defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy)) ctx.Engine.KnockOut(defending, "Bring Down the Axe");
                    return true;
                }
                if (atk.Name == "Dragon Pulse")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Colorful Catch: up to 3 Basic Energy cards of different types from your deck into your hand.</summary>
        private sealed class EeveeSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Catch") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var taken = new List<CardInstance>();
                for (int i = 0; i < 3; i++)
                {
                    var types = new HashSet<EnergyType>(taken.Select(c => c.Def.EnergyProvides));
                    var found = ctx.Engine.SearchDeck(ctx.Player, d => d.IsBasicEnergy && !types.Contains(d.EnergyProvides), 1,
                        "Colorful Catch: choose a Basic Energy of a type you haven't taken (" + (i + 1) + " of up to 3)", ctx.Source, true);
                    if (found.Count == 0) break;
                    taken.AddRange(found);
                }
                ctx.Me.Hand.AddRange(taken);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Energy Assist: attach a Basic Energy from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class FurfrouSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0 && ctx.Me.Bench.Count > 0)
                    ctx.AttachCardsAnyWay(energy, 1, (c, m) => ctx.Me.Bench.Contains(m), "Energy Assist: attach a Basic Energy to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        /// <summary>Power Charger: attach a Basic Energy from your deck to this Pokémon.</summary>
        private sealed class BewearSFA : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Charger") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Power Charger: choose a Basic Energy to attach to " + ctx.Pokemon.Name);
                if (found.Count > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Engine.AttachEnergyCard(found[0], ctx.Pokemon);
                else ctx.Me.Deck.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Only playable as the last card in your hand: search for up to 2 cards.</summary>
        private sealed class Cassiopeia : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.All(c => c == ctx.Card) && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => true, 2, "Cassiopeia: choose up to 2 cards", false, 1);
            }
        }

        /// <summary>Search for a Stadium card and an Energy card.</summary>
        private sealed class ColresssTenacity : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var stadium = ctx.SearchMyDeck(d => d.IsStadiumCard, 1, "Colress's Tenacity: choose a Stadium card");
                ctx.Me.Hand.AddRange(stadium);
                var energy = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Colress's Tenacity: choose an Energy card");
                ctx.Me.Hand.AddRange(energy);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>The opponent's Active Pokémon is now Burned and Confused.</summary>
        private sealed class DangerousLaser : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target == null) return;
                ctx.ApplyCondition(target, SpecialCondition.Burned);
                ctx.ApplyCondition(target, SpecialCondition.Confused);
            }
        }

        /// <summary>Prevent all damage done to Pokémon without a Rule Box by attacks from the other player's Pokémon ex and Pokémon V.</summary>
        private sealed class NeutralizationZone : CardScript
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon != null || !info.AgainstOpponent) return false;
                return !info.Target.Def.HasRuleBox && TefPokemon.IsExOrV(info.Attacker.Def);
            }
        }

        /// <summary>Heal 150 damage from 1 of your Pokémon.</summary>
        private sealed class PokeVitalA : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = Hurt(ctx);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Poké Vital A: heal 150 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 150);
            }
        }
    }
}
