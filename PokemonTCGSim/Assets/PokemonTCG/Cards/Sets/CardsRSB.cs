using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ruby &amp; Sapphire Uncommon and Common Pokémon (RS 25-79; v149). Unlimited only. Combusken 27, Grovyle 31, Marshtomp 40,
    /// Mudkip 60, Torchic 73 and Treecko 76 are prints of Nintendo Black Star Promos already in the pool.
    /// </summary>
    public static class CardsRSB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RS-25", new CoinsMon("Double Stab", 2));
            r.RegisterByKey("RS-26", new CardsRS.HardCocoonMon(new ConditionAttackMon("Poison Thread", SpecialCondition.Poisoned)));
            r.RegisterByKey("RS-28", new CardsRS.NaturalCureMon(EnergyType.Fire, new TailsNothingMon("Lunge")));
            r.RegisterByKey("RS-29", new ComboMon(new EnergyCallMon(), new CardsRS.ExtraEnergyMon("Cannonball", -1)));
            r.RegisterByKey("RS-30", new ComboMon(new ChargeMon(), new CardsFST1.FlipRecoilMon("Thunder Jolt", 10)));
            r.RegisterByKey("RS-32", new CardsRS.NaturalCureMon(EnergyType.Grass, new PlainMon()));
            r.RegisterByKey("RS-33", new ComboMon(new ParalyzeFlipMon("Shove"), new CoinsPlusMon("Slap Down", 2, 10)));
            r.RegisterByKey("RS-34", new RemovalBeamFlipMon());
            r.RegisterByKey("RS-35", new ComboMon(new CardsPAR2.FlipConditionMon("Dazzle Dance", SpecialCondition.Confused), new LifeDrainMon()));
            r.RegisterByKey("RS-36", new PlainMon());
            r.RegisterByKey("RS-37", new ComboMon(new MagnitudeMon(), new CoinsPlusMon("One-Two Strike", 2, 20)));
            r.RegisterByKey("RS-38", new ComboMon(new SeekOutMon(), new CardsE2.FlipUntilTailsMon("Continuous Headbutt")));
            r.RegisterByKey("RS-39", new ComboMon(new ParalyzeFlipMon("Thundershock"), new GigashockMon()));
            r.RegisterByKey("RS-41", new CardsRS.NaturalCureMon(EnergyType.Water, new CardsFLF1.NoResistanceMon("Aqua Sonic")));
            r.RegisterByKey("RS-42", new FlipBonusMon("Ambush", 30));
            r.RegisterByKey("RS-43", new CardsRS.HardCocoonMon(new NoRetreatMon("Gooey Thread")));
            r.RegisterByKey("RS-44", new ComboMon(new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2), new ConditionAttackMon("Lullaby", SpecialCondition.Asleep)));
            r.RegisterByKey("RS-45", new ComboMon(new TailsNothingMon("Claw"), new SlackOffMon()));
            r.RegisterByKey("RS-46", new SwellowRS());
            r.RegisterByKey("RS-47", new ComboMon(new CoinsMon("Fury Swipes", 3), new CardsE2B.RageMon()));
            r.RegisterByKey("RS-48", new ComboMon(new RestMon(), new CardsBS1.ExtraWaterMon("Water Gun")));
            r.RegisterByKey("RS-49", new TearyEyesMon());
            r.RegisterByKey("RS-50", new PlainMon());
            r.RegisterByKey("RS-51", new CardsASC.CounterattackMon("Rough Skin", 1, new NoRetreatMon("Big Bite")));
            r.RegisterByKey("RS-52", new PlainMon());
            r.RegisterByKey("RS-53", new ParalyzeFlipMon("Super Thunder Wave"));
            r.RegisterByKey("RS-54", new RecoilMon("Double-edge", 10));
            r.RegisterByKey("RS-55", new CardsBLK.FlailMon("Flail"));
            r.RegisterByKey("RS-56", new PlainMon());
            r.RegisterByKey("RS-57", new CardsE3B.OpponentSwitchesMon("Fling"));
            r.RegisterByKey("RS-58", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("RS-59", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("RS-61", new CardsPAR2.FlipConditionMon("Burn Off", SpecialCondition.Burned));
            r.RegisterByKey("RS-62", new TremblerMon());
            r.RegisterByKey("RS-63", new PlainMon());
            r.RegisterByKey("RS-64", new CardsRS.FlipHandDiscardMon("Knock Off"));
            r.RegisterByKey("RS-65", new NoRetreatMon("Shadow Bind"));
            r.RegisterByKey("RS-66", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("RS-67", new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep));
            r.RegisterByKey("RS-68", new LinkBlastMon());
            r.RegisterByKey("RS-69", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("RS-70", new PlusEnergyMon());
            r.RegisterByKey("RS-71", new TailsNothingMon("Lunge"));
            r.RegisterByKey("RS-72", new PlainMon());
            r.RegisterByKey("RS-74", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("RS-75", new CardsPAR2.FlipConditionMon("Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("RS-77", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("RS-78", new ComboMon(new SignsOfEvolutionRS(), new ConditionAttackMon("Poison Barb", SpecialCondition.Poisoned)));
            r.RegisterByKey("RS-79", new CoinsMon("Fury Swipes", 3));
        }

        /// <summary>"Flip N coins. This attack does X damage plus Y more damage for each heads" (Slap Down, One-Two Strike).</summary>
        internal sealed class CoinsPlusMon : Mon
        {
            private readonly string attack;
            private readonly int coins;
            private readonly int per;

            public CoinsPlusMon(string attack, int coins, int per)
            {
                this.attack = attack;
                this.coins = coins;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + per * ctx.FlipCoins(coins));
                return true;
            }
        }

        /// <summary>Energy Call: attach 1 Energy card from your discard pile to your Active Pokémon.</summary>
        private sealed class EnergyCallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Call") return false;
                ctx.DamageDefending(atk.Damage);
                var active = ctx.Me.Active;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (active != null && energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, 1, (c, m) => m == active, "Energy Call: attach an Energy card from your discard pile to " + active.Name, false);
                return true;
            }
        }

        /// <summary>Charge: attach a Lightning Energy card from your discard pile to Electrike.</summary>
        private sealed class ChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var lightning = ctx.Me.Discard.Where(c => CardsRS.BasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (lightning.Count > 0 && ctx.InPlay(me))
                    ctx.AttachCardsAnyWay(lightning, 1, (c, m) => m == me, "Charge: attach a Lightning Energy card from your discard pile to " + me.Name, false);
                return true;
            }
        }

        /// <summary>Removal Beam: heads, discard 1 Energy card attached to the Defending Pokémon.</summary>
        private sealed class RemovalBeamFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Removal Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Removal Beam: discard which Energy card from " + defending.Name + "?");
                return true;
            }
        }

        /// <summary>Life Drain: heads, damage counters on the Defending Pokémon until it has 10 HP left.</summary>
        private sealed class LifeDrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Life Drain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (!ctx.Flip() || defending == null) return true;
                int counters = (ctx.Engine.RemainingHp(defending) - 10) / 10;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                return true;
            }
        }

        /// <summary>Magnitude: then 10 to each Benched Pokémon (both players', no Weakness or Resistance).</summary>
        internal sealed class MagnitudeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnitude") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Seek Out: up to 2 cards from your deck into your hand.</summary>
        private sealed class SeekOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seek Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.Me.Hand.AddRange(ctx.SearchMyDeck(d => true, 2, "Seek Out: choose up to 2 cards", false));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Gigashock: then 10 to 2 of the opponent's Benched Pokémon (1 if there's only 1), no Weakness or Resistance.</summary>
        private sealed class GigashockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigashock") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                var targets = bench.Count == n ? bench : ctx.ChoosePokemonMany(bench, n, n, "Gigashock: choose 2 of your opponent's Benched Pokémon");
                foreach (var mon in targets)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Slack Off: remove all damage counters from Slakoth; it can't attack during your next turn.</summary>
        private sealed class SlackOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slack Off") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Drive Off (Poké-Power): once during your turn, while Swellow is your Active Pokémon, your opponent switches the Defending
        /// Pokémon with 1 of their Benched Pokémon (they pick it).
        /// </summary>
        private sealed class SwellowRS : Mon
        {
            private const string Power = "Drive Off";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.PokemonIsActive && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
        }

        /// <summary>Rest: remove all Special Conditions and 4 damage counters (all if fewer) from Wailmer; then it's Asleep.</summary>
        private sealed class RestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                ctx.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) ctx.Heal(me, 40);
                if (ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Teary Eyes: during the opponent's next turn, damage done to Aron by attacks is 10 less.</summary>
        private sealed class TearyEyesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teary Eyes") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(10);
                return true;
            }
        }

        /// <summary>Trembler: heads, 10 damage to the Defending Pokémon and it's Paralyzed; tails, nothing.</summary>
        private sealed class TremblerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trembler") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(10);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Link Blast: base damage 10 instead of 40 if Ralts and the Defending Pokémon have a different amount of Energy attached.</summary>
        private sealed class LinkBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Link Blast") return false;
                var defending = ctx.Defending;
                bool same = defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(defending);
                ctx.DamageDefending(same ? atk.Damage : 10);
                return true;
            }
        }

        /// <summary>Plus Energy: attach a basic Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class PlusEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plus Energy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, 1, (c, m) => ctx.Engine.CanAttachFromHand(m), "Plus Energy: attach a basic Energy card from your hand", false);
                return true;
            }
        }

        /// <summary>
        /// Signs of Evolution: choose Silcoon and Beautifly, or Cascoon and Dustox; take 1 card or both cards of that pair from your
        /// deck, show them and put them into your hand.
        /// </summary>
        private sealed class SignsOfEvolutionRS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Signs of Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                int pair = ctx.ChooseOption("Signs of Evolution: search for which pair?", "Silcoon and Beautifly", "Cascoon and Dustox");
                var names = pair == 0 ? new[] { "Silcoon", "Beautifly" } : new[] { "Cascoon", "Dustox" };
                var found = new List<CardInstance>();
                foreach (var name in names)
                {
                    string wanted = name;
                    found.AddRange(ctx.SearchMyDeck(d => d.IsPokemon && d.Name == wanted, 1, "Signs of Evolution: choose " + wanted + " (or none)"));
                }
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }
    }
}
