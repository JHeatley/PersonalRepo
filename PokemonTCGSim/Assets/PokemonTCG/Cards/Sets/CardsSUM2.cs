using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sun &amp; Moon (SUM, v90), part 2: Water and Lightning (SUM 28-53). Primarina-GX (Guardians Rising) was already in the pool and
    /// keeps its script; this Araquanid plays like Ultra Prism's.
    /// </summary>
    public static class CardsSUM2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Water
            r.RegisterByKey("SUM-28", new CardsCEC1.ConfusionWaveMon("Confusion Wave"));
            r.RegisterByKey("SUM-29", new DoubleJetMon());
            r.RegisterByKey("SUM-30", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10));
            r.RegisterByKey("SUM-31", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("SUM-32", new ComboMon(new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused), new CardsEVS1.WakeUpSlapMon(80)));
            r.RegisterByKey("SUM-34", new ComboMon(new CardsROS1.HeadlockMon("Headlock", 30), new ShieldAttackMon("Guard Press", 20)));
            r.RegisterByKey("SUM-35", new ComboMon(new DrawAttackMon("Collect", 3), new LockAfterAttackMon("Blizzard Burn", false),
                new ConditionAttackMon("Ice Beam-GX", SpecialCondition.Paralyzed)));
            r.RegisterByKey("SUM-36", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("SUM-37", new RoostMon("Roost", 30));
            r.RegisterByKey("SUM-38", new ComboMon(new CourierMon(), new DodgeMon("Fly", true)));
            r.RegisterByKey("SUM-40", new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused));
            r.RegisterByKey("SUM-41", new ComboMon(new ConditionAttackMon("Disarming Voice", SpecialCondition.Confused), new CardsTWM1.HealSelfMon("Sparkling Aria", 30)));
            r.RegisterByKey("SUM-43", new CardsMEW1.SpreadBenchMon("Avalanche", 10));
            r.RegisterByKey("SUM-44", new WishiwashiSUM());
            r.RegisterByKey("SUM-45", new ParalyzeFlipMon("Bubble"));
            CardsUPR1.Same(r, "SUM-46", "UPR-17"); // Water Bubble, Aqua Edge
            r.RegisterByKey("SUM-47", new PyukumukuSUM());
            r.RegisterByKey("SUM-48", new ComboMon(new SearchAttackMon("Vivid Charge", d => d.IsBasicEnergy, 3), new PsychicFangsMon()));
            // Lightning
            r.RegisterByKey("SUM-49", new SearchingLightMon());
            r.RegisterByKey("SUM-50", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new Cards30C.TypeEnergyBonusMon("Aqua Spark", EnergyType.Water, 60)));
            r.RegisterByKey("SUM-51", new ParalyzeFlipMon("Shocking Jaws"));
            r.RegisterByKey("SUM-52", new VikavoltSUM());
            r.RegisterByKey("SUM-53", new ComboMon(new CardsSP1.FlipNoDamageMon("Defense Curl"), new CardsUNB1.DischargeMon()));
        }

        // ================================================================== Water

        /// <summary>Double Jet: discard up to 2 Water Energy cards from your hand; 60 damage for each card discarded.</summary>
        private sealed class DoubleJetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Jet") return false;
                var water = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                var picks = water.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(water, 0, Math.Min(2, water.Count), "Double Jet: discard up to 2 Water Energy cards from your hand (60 damage each)");
                int n = 0;
                foreach (var card in picks)
                {
                    if (!ctx.Me.Hand.Contains(card)) continue;
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                    n++;
                }
                if (n == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>"Heal N damage from this Pokémon. It can't retreat during your next turn" (Roost).</summary>
        internal sealed class RoostMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public RoostMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Heal(me, amount);
                var effect = new CantRetreatEffect(me.Name + " can't retreat during its next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 2);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>Courier: put 1 of your Benched Pokémon and all cards attached to it into your hand.</summary>
        private sealed class CourierMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Courier") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench().Where(m => ctx.Engine.CanPutIntoHand(m)).ToList();
                if (bench.Count == 0) return true;
                var pick = ctx.ChoosePokemon(bench, "Courier: put which of your Benched Pokémon and all attached cards into your hand?");
                if (pick != null && ctx.InPlay(pick)) ctx.Engine.ReturnPokemonToHand(pick);
                return true;
            }
        }

        /// <summary>
        /// Cowardice: once during your turn, you may discard all cards attached to this Pokémon and put it into your hand; not during
        /// your first turn or the turn it was put into play. An Active Wishiwashi is replaced from the Bench right away; with no
        /// Benched Pokémon you have none in play and lose (v90, like a Fossil: v68).
        /// </summary>
        private sealed class WishiwashiSUM : Mon
        {
            protected override string Activated { get { return "Cowardice"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return me != null && !ctx.IsMyFirstTurn && me.TurnEnteredPlay != ctx.Turn && ctx.Engine.CanPutIntoHand(me);
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return;
                foreach (var en in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, en);
                if (me.Tools.Count > 0) ctx.Engine.DiscardAllTools(me);
                ctx.Engine.ReturnPokemonToHand(me);
            }
        }

        /// <summary>
        /// Innards Out: if this Pokémon is your Active Pokémon and is Knocked Out by damage from an opponent's attack, put 6 damage
        /// counters on the Attacking Pokémon. Continuous Tumble: flip until tails, 30 for each heads.
        /// </summary>
        private sealed class PyukumukuSUM : Mon
        {
            private static readonly Mon Tumble = new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (!holderWasActive || damage <= 0 || me == null || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.Engine.RemainingHp(me) > 0) return;
                Announce(ctx, "Innards Out");
                ctx.Engine.PlaceDamageCounters(attacker, 6, "Innards Out");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tumble.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Fangs: this attack's damage isn't affected by any effects on the opponent's Active Pokémon.</summary>
        private sealed class PsychicFangsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Fangs") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Searching Light: look at 1 of your face-down Prize cards.</summary>
        private sealed class SearchingLightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searching Light") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Me;
                var slots = me.PrizeSlotList();
                var opts = new List<DecisionOption>();
                for (int k = 0; k < me.Prizes.Count; k++)
                {
                    if (me.FaceUpPrizeIds.Contains(me.Prizes[k].Id)) continue;
                    opts.Add(new DecisionOption { Label = "Prize card " + (slots[k] + 1), Index = k });
                }
                if (opts.Count == 0) return true;
                int pick = opts.Count == 1 ? 0 : ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Searching Light: look at which of your face-down Prize cards?", opts, ctx.Source);
                var card = me.Prizes[opts[pick].Index];
                var shown = new List<CardInstance> { card };
                ctx.Engine.NoteLookedAt(shown, "1 of your Prize cards", ctx.Player);
                ctx.Engine.ShowCards(ctx.Player, shown, "Searching Light", "1 of your Prize cards", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at 1 of " + ctx.Engine.Their(ctx.Player) + " Prize cards (Searching Light).");
                return true;
            }
        }

        /// <summary>
        /// Strong Charge: once during your turn, search your deck for a Grass Energy card and a Lightning Energy card and attach them to
        /// your Pokémon in any way you like. Electro Cannon: discard 3 Energy from this Pokémon.
        /// </summary>
        private sealed class VikavoltSUM : Mon
        {
            private static readonly Mon Cannon = new CardsTWM1.SelfDiscardMon("Electro Cannon", 3);

            protected override string Activated { get { return "Strong Charge"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = new List<CardInstance>();
                found.AddRange(ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Grass), 1, "Strong Charge: choose a Grass Energy card"));
                if (ctx.Me.Deck.Count > 0)
                    found.AddRange(ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Lightning), 1, "Strong Charge: choose a Lightning Energy card"));
                CardsSUM1.AttachFoundAnyWay(ctx, found, "Strong Charge");
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cannon.HandleAttack(ctx, atk); }
        }
    }
}
