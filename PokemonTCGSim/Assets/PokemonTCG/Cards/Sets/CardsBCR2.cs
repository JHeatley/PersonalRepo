using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Boundaries Crossed (BCR, v112): Fighting to Colorless Pokémon (78-126), the secret rares Golurk and Altaria, and the Trainers.
    /// Energy Search, Great Ball, Poké Ball, Potion, Rocky Helmet, Skyla, Switch, Town Map, Bianca and Cheren were already in the pool.
    /// </summary>
    public static class CardsBCR2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BCR-79", new ComboMon(new CardsMEG.SandAttackMon("Sand-Attack"), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("BCR-80", new ParalyzeFlipMon("Wicked Jab"));
            r.RegisterByKey("BCR-81", new ComboMon(new CardsPOR.PoisonRingMon("Poison Ring"), new SwitchSelfMon("Night Slash", false)));
            r.RegisterByKey("BCR-83", new CardsASC.FlipDiscardDefendingEnergyMon("Smithereen Smash"));
            r.RegisterByKey("BCR-84", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("BCR-85", new CrustleSturdy());
            r.RegisterByKey("BCR-86", new FlipBonusMon("Steady Punch", 10));
            r.RegisterByKey("BCR-88", new ComboMon(new CardsASC.RandomDiscardMon("Knock Off"), new CoinsMon("Double Whip", 2)));
            r.RegisterByKey("BCR-89", new ComboMon(new Cards30CC.BenchHitMon("Hammerhead", 30), new LandsJudgmentMon()));
            r.RegisterByKey("BCR-91", new ComboMon(new ConditionAttackMon("Tail Trickery", SpecialCondition.Confused), new AssistMon()));
            r.RegisterByKey("BCR-93", new CoinsMon("Dual Cut", 2));
            r.RegisterByKey("BCR-94", new ComboMon(new SteelSlashMon(), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("BCR-95", new TailsNothingMon("Claw"));
            r.RegisterByKey("BCR-96", new ComboMon(new CardsPAR2.FlipConditionMon("Metal Sound", SpecialCondition.Confused), new CardsCRZ1.PiercingMon("Swift")));
            r.RegisterByKey("BCR-97", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("BCR-98", new ComboMon(new CoinsMon("Quick Turn", 2), new CardsMEW1.SpreadBenchMon("Sand Pulse", 10)));
            r.RegisterByKey("BCR-99", new FlygonSandSlammer());
            r.RegisterByKey("BCR-100", new ComboMon(new CoinsMon("Dual Claw", 2), new CardsTWM1.SelfDiscardMon("Flash Freeze", 1)));
            r.RegisterByKey("BCR-102", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Damage Rush"), new FlipBonusMon("Cold Fire", 40)));
            r.RegisterByKey("BCR-104", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("BCR-105", new ComboMon(new DiscardDefendingToolMon("Gnaw Through"), new CardsPGO.SuperFangMon()));
            r.RegisterByKey("BCR-106", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("BCR-107", new FlipBonusMon("Hard Swing", 20));
            r.RegisterByKey("BCR-108", new DittoTransform());
            r.RegisterByKey("BCR-109", new CoinsMon("Double Lariat", 2));
            r.RegisterByKey("BCR-110", new ParalyzeFlipMon("Attract Smack"));
            r.RegisterByKey("BCR-111", new ComboMon(new DrawAttackMon("Double Draw", 2), new DodgeMon("Dig", false)));
            r.RegisterByKey("BCR-113", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("BCR-114", new ComboMon(new CardsXY2.GatherEnergyMon(), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("BCR-115", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("BCR-117", new ComboMon(new HealingMelodyMon(), new CardsASC.PushDownMon("Kick Away")));
            r.RegisterByKey("BCR-118", new CardsLTR2.ReorderTopMon("Patrol", 3));
            r.RegisterByKey("BCR-119", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new CardsBKT2.SelfBoostMon("Psych Up", 30)));
            r.RegisterByKey("BCR-120", new CardsASC.PushDownMon("Roar"));
            r.RegisterByKey("BCR-121", new RecoilMon("Take Down", 10));
            r.RegisterByKey("BCR-122", new StoutlandSentinel());
            r.RegisterByKey("BCR-123", new TailsNothingMon("Razor Wind"));
            r.RegisterByKey("BCR-124", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("BCR-125", new ComboMon(new CardsASC.PushDownMon("Wing Flick"), new FlipTailsDiscardSelfMon("Air Slash")));
            r.RegisterByKey("BCR-126", new AudinoBusybody());
            r.RegisterByKey("BCR-150", new ComboMon(new DevolutionPunchMon("Devolution Punch"), new CardsBCR1.NoWeaknessNextTurnMon("Ghost Hammer")));
            r.RegisterByKey("BCR-152", new CardsHIF2.AltariaHIF());

            r.RegisterByName("Aspertia City Gym", new AspertiaCityGym());
            r.RegisterByName("Hugh", new Hugh());
            r.RegisterByName("Computer Search", new ComputerSearch());
            r.RegisterByName("Crystal Edge", new CrystalEdge());
            r.RegisterByName("Crystal Wall", new CrystalWall());
            r.RegisterByName("Gold Potion", new GoldPotion());
        }

        // ================================================================== Fighting

        /// <summary>Sturdy: with full HP, a Knock Out by attack damage leaves it at 10 HP instead. Stone Edge.</summary>
        private sealed class CrustleSturdy : Mon
        {
            private static readonly Mon Edge = new FlipBonusMon("Stone Edge", 20);

            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Edge.HandleAttack(ctx, atk); }
        }

        /// <summary>Land's Judgment: you may discard all Fighting Energy attached to this Pokémon; if you do, 70 more damage.</summary>
        private sealed class LandsJudgmentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Land's Judgment") return false;
                var me = ctx.Pokemon;
                var fighting = me.Energy.Where(c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Fighting)).ToList();
                bool more = false;
                if (fighting.Count > 0 && ctx.AskYesNo("Land's Judgment: discard all Fighting Energy from " + me.Name + " for 70 more damage?"))
                {
                    foreach (var card in fighting) ctx.Engine.DiscardEnergy(me, card);
                    more = true;
                }
                ctx.DamageDefending(atk.Damage + (more ? 70 : 0));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Assist: flip a coin; heads, choose 1 of your Benched Pokémon's attacks and use it as this attack (no Energy needed for it).</summary>
        private sealed class AssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assist") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var options = new List<KeyValuePair<CardDef, int>>();
                foreach (var mon in ctx.Me.Bench)
                    for (int i = 0; i < mon.Def.Attacks.Count; i++)
                        if (!options.Any(o => o.Key.Name == mon.Def.Name && o.Value == i)) options.Add(new KeyValuePair<CardDef, int>(mon.Def, i));
                if (options.Count == 0)
                {
                    ctx.Log("Assist: none of your Benched Pokémon has an attack.");
                    return true;
                }
                int pick = options.Count == 1 ? 0 : ctx.ChooseOption("Assist: choose 1 of your Benched Pokémon's attacks to use",
                    options.Select(o => o.Key.Name + ": " + o.Key.Attacks[o.Value].Name).ToArray());
                if (pick < 0 || pick >= options.Count) return true;
                UseCopiedAttack(ctx, options[pick].Key, options[pick].Value);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Steel Slash: 40, and during the opponent's next turn prevent all damage done to this Pokémon by attacks from Pokémon-EX.</summary>
        private sealed class SteelSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Steel Slash") return false;
                ctx.DamageDefending(atk.Damage);
                CardsPBL.ShieldSelfFrom(ctx, atk.Name, 9999, a => a.Def.IsPokemonEX, "Pokémon-EX");
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Sand Slammer: between turns, if this Pokémon is your Active Pokémon, 1 damage counter on each of the opponent's Pokémon.
        /// Flying Beatdown: you may discard a Grass and a Fighting Energy from it; if you do, the Defending Pokémon is Paralyzed.
        /// </summary>
        private sealed class FlygonSandSlammer : Mon
        {
            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? 1 - src.Player : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || src.Opp.InPlay().Count() == 0) return;
                Announce(src, "Sand Slammer");
                foreach (var mon in src.Opp.InPlay().ToList()) src.Engine.PlaceDamageCounters(mon, 1, "Sand Slammer");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flying Beatdown") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                var grass = me.Energy.FirstOrDefault(c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Grass));
                var fighting = me.Energy.FirstOrDefault(c => c != grass && CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Fighting));
                if (grass == null || fighting == null) return true;
                if (!ctx.AskYesNo("Flying Beatdown: discard a Grass and a Fighting Energy to Paralyze the Defending Pokémon?")) return true;
                ctx.Engine.DiscardEnergy(me, grass);
                ctx.Engine.DiscardEnergy(me, fighting);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>"Discard a Pokémon Tool card attached to the Defending Pokémon" (Gnaw Through).</summary>
        internal sealed class DiscardDefendingToolMon : Mon
        {
            private readonly string attack;

            public DiscardDefendingToolMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Tools.Count == 0 || !ctx.CanAffect(defending)) return true;
                var tool = defending.Tools.Count == 1 ? defending.Tools[0] : ctx.ChooseCards(defending.Tools.ToList(), 1, 1, attack + ": discard which Pokémon Tool?")[0];
                ctx.Engine.DiscardTool(defending, tool);
                return true;
            }
        }

        /// <summary>
        /// Transform: during your turn, put a Basic Pokémon from your hand on top of this Pokémon. Attached cards, damage, Special
        /// Conditions and turns in play stay (it can evolve next turn, 2012-11-08); it isn't playing a Pokémon or evolving.
        /// </summary>
        private sealed class DittoTransform : Mon
        {
            private static List<CardInstance> Basics(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsBasicPokemon).ToList(); }

            protected override string Activated { get { return "Transform"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Pokemon.Def.Name == "Ditto" && Basics(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0) return;
                var pick = basics.Count == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, "Transform: put which Basic Pokémon on top of Ditto?")[0];
                if (!ctx.Me.Hand.Remove(pick)) return;
                var mon = ctx.Pokemon;
                mon.Stack.Add(pick);
                ctx.Log("Ditto transformed into " + pick.Def.Name + ".");
            }
        }

        /// <summary>Healing Melody: flip a coin; heads, heal 60 damage from each of your Pokémon.</summary>
        private sealed class HealingMelodyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Melody") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 60);
                return true;
            }
        }

        /// <summary>Sentinel: while this Pokémon is Active, the opponent can't play Supporter cards from their hand. Wild Tackle.</summary>
        private sealed class StoutlandSentinel : Mon
        {
            private static readonly Mon Tackle = new CardsFST1.FlipRecoilMon("Wild Tackle", 20);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && player != src.Player && card != null && card.Def.IsSupporter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>Air Slash (Unfezant): flip a coin; tails, discard an Energy attached to this Pokémon.</summary>
        private sealed class FlipTailsDiscardSelfMon : Mon
        {
            private readonly string attack;

            public FlipTailsDiscardSelfMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() && ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, attack + ": discard an Energy from " + me.Name);
                return true;
            }
        }

        /// <summary>
        /// Busybody: once during your turn, reveal this Pokémon from your hand: heal 10 damage and remove a Special Condition from your
        /// Active Pokémon, then discard it. Not usable with nothing to heal (2012-11-01). Hip Bump.
        /// </summary>
        private sealed class AudinoBusybody : Mon
        {
            private static readonly Mon Bump = new CardsUPR3.SmokescreenMon("Hip Bump", 1);

            public override bool CanUseFromHand(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && active != null
                       && (active.DamageCounters > 0 || active.Conditions != SpecialCondition.None);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!CanUseFromHand(ctx)) return;
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { ctx.Card }, "revealed it (Busybody)");
                var active = ctx.Me.Active;
                if (active.DamageCounters > 0) ctx.Heal(active, 10);
                var all = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
                var has = all.Where(c => active.Has(c)).ToList();
                if (has.Count > 0)
                {
                    var pick = has.Count == 1 ? has[0] : has[ctx.ChooseOption("Busybody: remove which Special Condition from " + active.Name + "?", has.Select(c => c.ToString()).ToArray())];
                    ctx.Engine.RemoveCondition(active, pick);
                }
                if (ctx.Me.Hand.Contains(ctx.Card)) ctx.Engine.DiscardFromHand(ctx.Player, ctx.Card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bump.HandleAttack(ctx, atk); }
        }

        /// <summary>Devolution Punch: 60, then devolve the Defending Pokémon; the top Evolution card goes to the opponent's hand (damage first, 2012-08-23). Used for Devolution Punch, Rewind Beam, Devolution Ray (the attack name is passed in).</summary>
        internal sealed class DevolutionPunchMon : Mon
        {
            private readonly string attack;

            public DevolutionPunchMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Stack.Count > 1 && ctx.CanAffect(defending)) ctx.Engine.Devolve(defending, 1);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Aspertia City Gym: each Colorless Pokémon in play (both players') gets +20 HP.</summary>
        private sealed class AspertiaCityGym : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon != null && src.Engine.IsType(mon, EnergyType.Colorless) ? hp + 20 : hp;
            }
        }

        /// <summary>Hugh: each player draws or discards until they have 5 cards (the opponent first). Not playable if nothing would change (2013-01-03).</summary>
        private sealed class Hugh : CardScript
        {
            private static bool Changes(PlayerState p, int hand) { return hand > 5 || (hand < 5 && p.Deck.Count > 0); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return Changes(ctx.Me, ctx.Me.Hand.Count(c => c != ctx.Card)) || Changes(ctx.Opp, ctx.Opp.Hand.Count);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                Fix(ctx, ctx.Opponent);
                Fix(ctx, ctx.Player);
            }

            private static void Fix(EffectContext ctx, int player)
            {
                var p = ctx.State.Players[player];
                if (p.Hand.Count < 5)
                {
                    ctx.Engine.DrawCards(player, 5 - p.Hand.Count);
                    return;
                }
                if (p.Hand.Count == 5) return;
                int n = p.Hand.Count - 5;
                var picks = ctx.Engine.ChooseCards(player, p.Hand.ToList(), n, n, "Hugh: discard " + n + (n == 1 ? " card" : " cards") + " from your hand", ctx.Source);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(player, card);
            }
        }

        /// <summary>Computer Search (ACE SPEC): discard 2 cards from your hand, then search your deck for any card (you must take one: 2013-08-15).</summary>
        private sealed class ComputerSearch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count < 2) return;
                var discarded = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Computer Search: discard 2 cards from your hand");
                foreach (var card in discarded) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.SearchToHand(d => true, 1, "Computer Search: put a card from your deck into your hand", false, 1);
            }
        }

        /// <summary>Crystal Edge (ACE SPEC): a White Kyurem-EX holder's attacks do 50 more damage to the opponent's Active Pokémon (before W/R).</summary>
        private sealed class CrystalEdge : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || holder.Name != "White Kyurem-EX" || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 50;
            }
        }

        /// <summary>Crystal Wall (ACE SPEC): a Black Kyurem-EX holder's printed HP is 300; other HP changes still apply on top (2012-11-08).</summary>
        private sealed class CrystalWall : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                var holder = src.Pokemon;
                if (holder == null || mon != holder || holder.Name != "Black Kyurem-EX") return hp;
                return hp + (300 - holder.Def.Hp);
            }
        }

        /// <summary>Gold Potion (ACE SPEC): heal 90 damage from your Active Pokémon.</summary>
        private sealed class GoldPotion : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0) ctx.Heal(ctx.Me.Active, 90);
            }
        }
    }
}
