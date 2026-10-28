using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Shining Fates (SHF, 2021): the main-set and Shiny Vault Pokémon from Fighting to Colorless, and the Trainers.
    /// Your rulings: Ditto V's V Transformation sends Ditto V to the discard pile and can't pick another Ditto V; Arctozolt's
    /// Biting Whirlpool works from the Bench and each Arctozolt adds its 2 damage counters (CardsSHF1).
    /// Pokémon whose attacks only do their printed damage need no script (Rolycoly, Carkol, Spinarak, Impidimp, Cufant,
    /// Ducklett, Skwovet, Rookidee).
    /// </summary>
    public static class CardsSHF2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("SHF-40", new CardsFST2.ConditionBonusMon("Land's Pulse", 10, StadiumInPlay));
            r.RegisterByKey("SHF-SV63", new FlipBonusMon("Rock Smash", 30));
            r.RegisterByKey("SHF-SV64", new MeteorAssaultMon("Meteor Assault"));
            r.RegisterByKey("SHF-SV65", new RecoilMon("Reckless Charge", 30));
            r.RegisterByKey("SHF-SV66", new ComboMon(new SpreadingSpiteMon(), new RecoilMon("Mad Hammer", 30)));
            r.RegisterByKey("SHF-SV69", new CoalossalSHF());
            r.RegisterByKey("SHF-SV70", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("SHF-SV71", new CardsFST1.NextTurnBonusMon("Coil", 120));
            r.RegisterByKey("SHF-SV72", new ParalyzeFlipMon("Bind"));
            r.RegisterByKey("SHF-SV73", new ComboMon(new OctolockMon(), new CardsFLF1.NoResistanceMon("Tough Swing")));
            r.RegisterByKey("SHF-SV74", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new CardsASR1.TimesMon("Team Attack", BenchedFalinks)));
            r.RegisterByKey("SHF-SV75", new PowerPressMon());
            r.RegisterByKey("SHF-SV115", new FalinksV());
            // Darkness
            r.RegisterByKey("SHF-41", new CardsSIT1.AscensionMon());
            r.RegisterByKey("SHF-42", new GalarianWeezingSHF());
            r.RegisterByKey("SHF-45", new StealthPoisonMon());
            r.RegisterByKey("SHF-46", new AmazingDestructionMon());
            r.RegisterByKey("SHF-47", new InstigateMon());
            r.RegisterByKey("SHF-48", new ComboMon(new BenchSearchAttackMon("Call for Goons", 3), new NoRetreatMon("Corner")));
            r.RegisterByKey("SHF-SV78", new GalarianZigzagoonSHF());
            r.RegisterByKey("SHF-SV79", new SwitchSelfMon("Night Slash", false));
            r.RegisterByKey("SHF-SV82", new ThiefMon());
            r.RegisterByKey("SHF-SV84", new CardsPAF2.IgnoreDefenderEffectsMon("False Surrender"));
            r.RegisterByKey("SHF-SV85", new GrimmsnarlSHF());
            r.RegisterByKey("SHF-SV116", new CardsSHF1.EnergyToHandMon("Spiky Knuckle", EnergyType.Darkness, 2));
            r.RegisterByKey("SHF-SV117", new GMaxDrillMon());
            // Metal
            r.RegisterByKey("SHF-SV86", new GalarianMeowthSHF());
            r.RegisterByKey("SHF-SV87", new PerrserkerSHF());
            r.RegisterByKey("SHF-SV88", new StunfiskSHF());
            r.RegisterByKey("SHF-SV89", new IronWingsMon());
            r.RegisterByKey("SHF-SV91", new CopperajahSHF());
            r.RegisterByKey("SHF-SV92", new MetalSharpenerMon());
            // Colorless
            r.RegisterByKey("SHF-50", new DittoV());
            r.RegisterByKey("SHF-51", new MaxTransformMon());
            r.RegisterByKey("SHF-52", new ComboMon(new SearchAttackMon("Signs of Evolution", EvolvesFromEevee, 1), new TailsNothingMon("Wild Kick")));
            r.RegisterByKey("SHF-53", new ComboMon(new CardsDRI2.CrunchMon("Crunch"), new TailsNothingMon("Stumbling Press")));
            r.RegisterByKey("SHF-54", new ComboMon(new SearchAttackMon("Beak Catch", AnyCard, 2), new SpitShotMon("Spit Shot", 160)));
            r.RegisterByKey("SHF-55", new CardsASC.FlipPerEnergyMon("Max Jet"));
            r.RegisterByKey("SHF-56", new ComboMon(new HelpingHandMon(), new CardsMEG.DefendingEnergyBonusMon("Psychic", 20)));
            r.RegisterByKey("SHF-SV93", new ComboMon(new GlanceMon(), new CoinsMon("Tail Slap", 2)));
            r.RegisterByKey("SHF-SV94", new CinccinoSHF());
            r.RegisterByKey("SHF-SV96", new CardsSHF1.SkyCircusMon(new FeatherSliceMon()));
            r.RegisterByKey("SHF-SV97", CardsSHF1.MadParty());
            r.RegisterByKey("SHF-SV100", new GreedentSHF());
            r.RegisterByKey("SHF-SV102", new CardsBLK.PeckOffMon("Pluck"));
            r.RegisterByKey("SHF-SV103", new CardsASC.SelfGuardMon("Expand", 10));
            r.RegisterByKey("SHF-SV104", new ComboMon(new CardsASC.SelfGuardMon("Cotton Guard", 30), new RecoilMon("Double-Edge", 30)));
            r.RegisterByKey("SHF-SV120", new CardsFST1.TakeLessMon(30, new CardsASR1.PlusMon("Revenge Blast", 30, OpponentPrizesTaken)));

            // Trainers
            r.RegisterByName("Ball Guy", new BallGuy());
            r.RegisterByName("Bird Keeper", new BirdKeeper());
            r.RegisterByName("Cara Liss", new CaraLiss());
            r.RegisterByName("Poké Kid", new SearchItem(CardsSHF1.IsPokemonCard, "Poké Kid: search your deck for a Pokémon"));
            r.RegisterByName("Rose", new Rose());
            r.RegisterByName("Rusted Shield", new RustedShield());
            r.RegisterByName("Rusted Sword", new RustedSword());
            r.RegisterByName("Skyla", new SearchItem(IsTrainerCard, "Skyla: search your deck for a Trainer card"));
            r.RegisterByName("Team Yell Towel", new TeamYellTowel());
        }

        // ================================================================== Filters and counters

        private static bool StadiumInPlay(EffectContext ctx) { return ctx.State.Stadium != null; }
        private static int BenchedFalinks(EffectContext ctx) { return ctx.Me.Bench.Count(m => m.Name.Contains("Falinks")); }
        private static int OpponentPrizesTaken(EffectContext ctx) { return ctx.OpponentPrizesTaken; }
        private static bool EvolvesFromEevee(CardDef d) { return d.IsPokemon && d.EvolvesFrom == "Eevee"; }
        private static bool AnyCard(CardDef d) { return true; }
        private static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }
        private static bool IsToolCard(CardDef d) { return d.IsTool; }
        private static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }

        // ================================================================== Fighting

        /// <summary>Spreading Spite: for each damage counter on this Pokémon, put 2 damage counters on the opponent's Pokémon in any way you like.</summary>
        private sealed class SpreadingSpiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spreading Spite") return false;
                int counters = 2 * ctx.Pokemon.DamageCounters;
                if (counters > 0) ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentPokemon());
                else ctx.Log("Spreading Spite: " + ctx.Pokemon.Name + " has no damage counters.");
                return true;
            }
        }

        /// <summary>Tar Generator: once during your turn, attach a Fire Energy card, a Fighting Energy card, or 1 of each from your discard pile to your Pokémon in any way you like.</summary>
        private sealed class CoalossalSHF : Mon
        {
            private static List<CardInstance> Of(EffectContext ctx, EnergyType type)
            {
                return ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, type)).ToList();
            }

            protected override string Activated { get { return "Tar Generator"; } }
            protected override bool AbilityReady(EffectContext ctx) { return Of(ctx, EnergyType.Fire).Count > 0 || Of(ctx, EnergyType.Fighting).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var kind in new[] { EnergyType.Fire, EnergyType.Fighting })
                {
                    var pile = Of(ctx, kind);
                    if (pile.Count == 0) continue;
                    ctx.AttachCardsAnyWay(pile, 1, (card, mon) => true, "Tar Generator (" + kind + " Energy)", true);
                }
            }
        }

        /// <summary>
        /// Octolock: while this Grapploct stays in the Active Spot, the Defending Pokémon's attacks cost [C][C] more and it can't
        /// retreat. The effect can't be applied more than once (using Octolock again changes nothing).
        /// </summary>
        private sealed class OctolockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Octolock") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.InPlay(me) || !ctx.PokemonIsActive || !ctx.CanAffect(defending)) return true;
                if (ctx.State.Effects.Any(e => e is OctolockEffect && e.TargetUid == defending.Uid && ((OctolockEffect)e).Working(ctx.Engine)))
                {
                    ctx.Log("Octolock: the effect can't be applied more than once.");
                    return true;
                }
                ctx.AddEffect(new OctolockEffect(defending.Name + "'s attacks cost [C][C] more and it can't retreat while " + me.Name + " stays Active (Octolock)", ctx.Player, defending.Uid, me));
                return true;
            }
        }

        /// <summary>Ends when the Defending Pokémon leaves the Active Spot or evolves; does nothing once that Grapploct has left the Active Spot.</summary>
        private sealed class OctolockEffect : ContinuousEffect
        {
            private readonly int grapploctUid;
            private readonly int grapploctActiveSince;

            public OctolockEffect(string name, int owner, int targetUid, PokemonInPlay grapploct)
                : base(name, owner, targetUid, int.MaxValue, true)
            {
                grapploctUid = grapploct.Uid;
                grapploctActiveSince = grapploct.MovedToActiveOnTurn;
            }

            public bool Working(GameEngine engine)
            {
                var g = engine.State.FindPokemon(grapploctUid);
                return g != null && engine.State.IsActive(g) && g.MovedToActiveOnTurn == grapploctActiveSince;
            }

            public override void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon.Uid != TargetUid || !Working(engine)) return;
                cost.Add(EnergyType.Colorless);
                cost.Add(EnergyType.Colorless);
            }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon)
            {
                return mon.Uid == TargetUid && Working(engine);
            }
        }

        /// <summary>Power Press: 60 more if this Pokémon has at least 1 extra Energy attached beyond the attack's cost.</summary>
        private sealed class PowerPressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Press") return false;
                ctx.DamageDefending(atk.Damage + (ExtraEnergy(ctx, atk, 1) ? 60 : 0));
                return true;
            }
        }

        /// <summary>Iron Defense Formation: your Pokémon with "Falinks" in their names take 20 less damage from the opponent's attacks. Giga Impact: can't attack next turn.</summary>
        private sealed class FalinksV : Mon
        {
            private static readonly Mon GigaImpact = new LockAfterAttackMon("Giga Impact", false);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                var a = info.Attacker;
                if (src.Pokemon == null || target == null || target.Owner != src.Player || a == null || a.Owner == src.Player) return damage;
                return target.Name.Contains("Falinks") ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return GigaImpact.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Neutralizing Gas: while this Pokémon is Active, the opponent's Pokémon in play have no Abilities except Neutralizing Gas. Severe Poison: Poisoned, 4 damage counters.</summary>
        private sealed class GalarianWeezingSHF : Mon
        {
            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && src.PokemonIsActive && mon != null && mon.Owner != src.Player && abilityName != "Neutralizing Gas";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Severe Poison") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned))
                    defending.PoisonCounterBase = 4;
                return true;
            }
        }

        /// <summary>Stealth Poison: the opponent's Active Pokémon is now Poisoned; switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class StealthPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stealth Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Amazing Destruction: the opponent's Active Pokémon is Knocked Out.</summary>
        private sealed class AmazingDestructionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amazing Destruction") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.Engine.KnockOut(defending, "Amazing Destruction");
                return true;
            }
        }

        /// <summary>Instigate: the opponent shuffles their hand and puts it on the bottom of their deck; if they put any cards there, they draw 3.</summary>
        private sealed class InstigateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Instigate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " has no cards in hand (Instigate).");
                    return true;
                }
                ctx.State.Rng.Shuffle(hand);
                foreach (var c in hand) ctx.Opp.Hand.Remove(c);
                ctx.Opp.Deck.AddRange(hand);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put their hand (" + hand.Count + " cards) on the bottom of their deck (Instigate).");
                ctx.Engine.DrawCards(ctx.Opponent, 3);
                return true;
            }
        }

        /// <summary>Headbutt Tantrum: when you play this Pokémon from your hand onto your Bench, you may put 1 damage counter on 1 of the opponent's Pokémon. Surprise Attack: tails, nothing.</summary>
        private sealed class GalarianZigzagoonSHF : Mon
        {
            private static readonly Mon SurpriseAttack = new TailsNothingMon("Surprise Attack");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Headbutt Tantrum", ctx.Opp.HasPokemonInPlay, "Use Headbutt Tantrum? Put 1 damage counter on 1 of your opponent's Pokémon.")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Headbutt Tantrum: put a damage counter on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SurpriseAttack.HandleAttack(ctx, atk); }
        }

        /// <summary>Thief: the opponent reveals their hand; put a card you find there on the bottom of their deck.</summary>
        private sealed class ThiefMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thief") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var pick = ctx.ChooseCards(hand.ToList(), 1, 1, "Thief: choose a card from your opponent's hand to put on the bottom of their deck")[0];
                if (hand.Remove(pick)) ctx.Opp.Deck.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put " + pick.Def.Name + " from their hand on the bottom of their deck (Thief).");
                return true;
            }
        }

        /// <summary>Dark Oath: while this Pokémon is Active, the opponent's Active Pokémon's attacks cost [C] more. Energy Press: 30 more per Energy on the opponent's Active.</summary>
        private sealed class GrimmsnarlSHF : Mon
        {
            private static readonly Mon EnergyPress = new CardsMEG.DefendingEnergyBonusMon("Energy Press", 30);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || mon.Owner == src.Player || !src.IsActive(mon)) return;
                cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return EnergyPress.HandleAttack(ctx, atk); }
        }

        /// <summary>G-Max Drill: 50 more for each Darkness Energy beyond the attack's cost, up to 100 more.</summary>
        private sealed class GMaxDrillMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Drill") return false;
                int needed = atk.Cost.Count(t => t == EnergyType.Darkness);
                int extra = Math.Max(0, ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Darkness) - needed);
                ctx.DamageDefending(atk.Damage + Math.Min(100, 50 * extra));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Evolution Roar: discard 2 cards from your hand to search your deck for a Galarian Perrserker, once during your turn.</summary>
        private sealed class GalarianMeowthSHF : Mon
        {
            private static bool IsPerrserker(CardDef d) { return d.IsPokemon && d.Name == "Galarian Perrserker"; }

            protected override string Activated { get { return "Evolution Roar"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count >= 2 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Evolution Roar: discard 2 cards from your hand");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.SearchToHand(IsPerrserker, 1, "Evolution Roar: search your deck for a Galarian Perrserker");
            }
        }

        /// <summary>Steely Spirit: your Metal Pokémon's attacks do 20 more damage to the opponent's Active Pokémon (each Galarian Perrserker adds 20).</summary>
        private sealed class PerrserkerSHF : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(a, EnergyType.Metal) ? damage + 20 : damage;
            }
        }

        /// <summary>Snap Trap: if this Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), discard an Energy from the Attacking Pokémon. Damage Rush.</summary>
        private sealed class StunfiskSHF : Mon
        {
            private static readonly Mon DamageRush = new CardsMEG.FlipUntilTailsBonusMon("Damage Rush", 30);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || attacker.Energy.Count == 0) return;
                Announce(ctx, "Snap Trap");
                ctx.DiscardEnergyFrom(attacker, 1, "Snap Trap: discard an Energy from " + attacker.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DamageRush.HandleAttack(ctx, atk); }
        }

        /// <summary>Iron Wings: you may discard 2 Energy from this Pokémon; if you do, it takes 100 less damage from attacks during the opponent's next turn.</summary>
        private sealed class IronWingsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Wings") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count < 2) return true;
                if (!ctx.AskYesNo("Iron Wings: discard 2 Energy from " + me.Name + " so it takes 100 less damage during your opponent's next turn?")) return true;
                if (ctx.DiscardEnergyFrom(me, 2, "Iron Wings: discard 2 Energy from " + me.Name) == 2) ctx.SelfTakesLessNextTurn(100);
                return true;
            }
        }

        /// <summary>Antibacterial Skin: this Pokémon can't be affected by any Special Conditions. Vengeful Stomp: 120 more if your Benched Pokémon have damage counters.</summary>
        private sealed class CopperajahSHF : Mon
        {
            private static readonly Mon VengefulStomp = new CardsSVP.BenchHurtBonusMon("Vengeful Stomp", 120);

            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target == src.Pokemon;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return VengefulStomp.HandleAttack(ctx, atk); }
        }

        /// <summary>Metal Sharpener: attach a Metal Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class MetalSharpenerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Sharpener") return false;
                ctx.DamageDefending(atk.Damage);
                var metal = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Metal)).ToList();
                if (metal.Count > 0) ctx.AttachCardsAnyWay(metal, 1, (card, mon) => true, "Metal Sharpener", false);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>
        /// V Transformation: once during your turn, switch this Pokémon with a Basic Pokémon V from your discard pile; attached
        /// cards, damage counters, Special Conditions, turns in play and other effects stay on the new Pokémon. Your ruling:
        /// Ditto V goes to the discard pile, and another Ditto V can't be chosen. Stick On: attach a basic Energy card from
        /// your discard pile to this Pokémon.
        /// </summary>
        private sealed class DittoV : Mon
        {
            private static List<CardInstance> Choices(EffectContext ctx)
            {
                var engine = ctx.Engine;
                int player = ctx.Player;
                return ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon && c.Def.IsPokemonV && c.Def.Name != "Ditto V" && !engine.PutIntoPlayBlocked(player, c.Def)).ToList();
            }

            protected override string Activated { get { return "V Transformation"; } }
            protected override bool AbilityReady(EffectContext ctx) { return Choices(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var choices = Choices(ctx);
                if (me == null || choices.Count == 0) return;
                var pick = ctx.ChooseCards(choices, 1, 1, "V Transformation: switch " + me.Name + " with which Basic Pokémon V from your discard pile?")[0];
                var old = me.Top;
                ctx.Me.Discard.Remove(pick);
                me.Stack[me.Stack.Count - 1] = pick;
                ctx.Me.Discard.Add(old);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " switched " + old.Def.Name + " with " + pick.Def.Name + " from their discard pile (V Transformation).", pick.Id, me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stick On") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var basics = ctx.BasicEnergyInDiscard();
                if (ctx.InPlay(me) && basics.Count > 0) ctx.AttachCardsAnyWay(basics, 1, (card, mon) => mon == me, "Stick On", false);
                return true;
            }
        }

        /// <summary>Max Transform: choose 1 of the opponent's Active Pokémon's attacks and use it as this attack.</summary>
        private sealed class MaxTransformMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Transform") return false;
                var defending = ctx.Defending;
                CardDef copied;
                int index;
                if (ChooseCopiedAttack(ctx, defending, "Max Transform: choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index))
                    UseCopiedAttack(ctx, copied, index);
                else ctx.Log("Max Transform: there's no attack to use.");
                return true;
            }
        }

        /// <summary>Spit Shot: discard all Energy from this Pokémon; 160 damage to 1 of the opponent's Pokémon. Used for Spit Shot, Drop Shot (the attack name and the numbers are passed in).</summary>
        internal sealed class SpitShotMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SpitShotMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>Helping Hand: search your deck for a basic Energy card and attach it to 1 of your Benched Pokémon.</summary>
        internal sealed class HelpingHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Helping Hand") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsBasicEnergyCard, 1, "Helping Hand: choose a basic Energy card");
                var me = ctx.Me;
                ctx.AttachCardsAnyWay(found, found.Count, (card, mon) => me.Bench.Contains(mon), "Helping Hand", false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Glance: look at the top card of the opponent's deck.</summary>
        internal sealed class GlanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Glance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count == 0) return true;
                var top = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.NoteLookedAt(top, "Top card of your opponent's deck", ctx.Player);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of " + ctx.Engine.Their(ctx.Opponent) + " deck (Glance).");
                return true;
            }
        }

        /// <summary>Make Do: discard a card from your hand to draw 2 cards, once during your turn (not with an empty deck: Compendium, 2020-02-06). Energy Assist: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class CinccinoSHF : Mon
        {
            protected override string Activated { get { return "Make Do"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var pick = ctx.ChooseCards(hand, 1, 1, "Make Do: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Me;
                var basics = ctx.BasicEnergyInDiscard();
                if (me.Bench.Count > 0 && basics.Count > 0) ctx.AttachCardsAnyWay(basics, 1, (card, mon) => me.Bench.Contains(mon), "Energy Assist", false);
                return true;
            }
        }

        /// <summary>Feather Slice: you may discard a card from your hand; if you do, 70 more damage.</summary>
        private sealed class FeatherSliceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Feather Slice") return false;
                int bonus = 0;
                if (ctx.Me.Hand.Count > 0)
                {
                    var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 0, 1, "Feather Slice: you may discard a card from your hand for 70 more damage");
                    if (picks.Count > 0)
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, picks[0]);
                        bonus = 70;
                    }
                }
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Greedy Tail: once during your turn, search your deck for a Pokémon Tool card.</summary>
        private sealed class GreedentSHF : Mon
        {
            protected override string Activated { get { return "Greedy Tail"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(IsToolCard, 1, "Greedy Tail: search your deck for a Pokémon Tool card"); }
        }

        // ================================================================== Trainers

        /// <summary>Ball Guy: search your deck for up to 3 Item cards with different names that have "Ball" in their names.</summary>
        private sealed class BallGuy : CardScript
        {
            private static bool IsBallItem(CardDef d) { return d.IsItem && d.Name.Contains("Ball"); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = new List<CardInstance>();
                for (int i = 0; i < 3; i++)
                {
                    var names = found.Select(c => c.Def.Name).ToList();
                    if (!ctx.Me.Deck.Any(c => IsBallItem(c.Def) && !names.Contains(c.Def.Name))) break;
                    var got = ctx.SearchMyDeck(d => IsBallItem(d) && !names.Contains(d.Name), 1,
                        "Ball Guy: choose an Item card with \"Ball\" in its name (" + (i + 1) + " of up to 3, each with a different name)");
                    if (got.Count == 0) break;
                    found.AddRange(got);
                }
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Bird Keeper: switch your Active Pokémon with 1 of your Benched Pokémon; if you do, draw 3 cards.</summary>
        private sealed class BirdKeeper : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.SwitchMyActive("Bird Keeper: choose a Benched Pokémon to switch in")) ctx.Draw(3);
            }
        }

        /// <summary>Cara Liss: search your deck for up to 2 Rare Fossil cards and put them onto your Bench.</summary>
        private sealed class CaraLiss : CardScript
        {
            private static bool IsRareFossil(CardDef d) { return d.Name == "Rare Fossil"; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int room = Math.Min(2, ctx.BenchSpace);
                var found = ctx.SearchMyDeck(IsRareFossil, room, "Cara Liss: choose up to " + room + " Rare Fossil cards to put onto your Bench");
                foreach (var c in found)
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    else ctx.Me.Deck.Add(c);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Rose: attach up to 2 basic Energy cards from your discard pile to 1 of your Pokémon VMAX; if you attached any, discard your hand.</summary>
        private sealed class Rose : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Def.IsVmax); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0 && ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                var basics = ctx.BasicEnergyInDiscard();
                if (targets.Count == 0 || basics.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Rose: attach basic Energy to which of your Pokémon VMAX?");
                int attached = ctx.AttachCardsAnyWay(basics, 2, (card, mon) => mon == target, "Rose", true);
                if (attached > 0) ctx.Engine.DiscardHand(ctx.Player);
            }
        }

        /// <summary>Rusted Shield: the Zamazenta V this card is attached to gets +70 HP.</summary>
        private sealed class RustedShield : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Name == "Zamazenta V" ? hp + 70 : hp;
            }
        }

        /// <summary>Rusted Sword: the attacks of the Zacian V this card is attached to do 30 more damage to the opponent's Active Pokémon.</summary>
        private sealed class RustedSword : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || src.Pokemon.Name != "Zacian V" || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 30;
            }
        }

        /// <summary>Team Yell Towel: heal 50 damage from both Active Pokémon.</summary>
        private sealed class TeamYellTowel : CardScript
        {
            private static bool Hurt(PokemonInPlay mon) { return mon != null && mon.DamageCounters > 0; }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx.Me.Active) || Hurt(ctx.Opp.Active); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (Hurt(ctx.Me.Active)) ctx.Heal(ctx.Me.Active, 50);
                if (Hurt(ctx.Opp.Active)) ctx.Heal(ctx.Opp.Active, 50);
            }
        }
    }
}
