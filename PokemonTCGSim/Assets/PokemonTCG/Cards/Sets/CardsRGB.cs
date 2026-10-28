using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>FireRed &amp; LeafGreen Uncommon and Common Pokémon (RG 31-86; v162). Unlimited only.</summary>
    public static class CardsRGB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RG-31", CardsSS.ConditionBonus("Damage Burn", 20, d => d.DamageCounters > 0));
            r.RegisterByKey("RG-32", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new FlipBonusMon("Ambush", 10)));
            r.RegisterByKey("RG-33", new ComboMon(new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("RG-34", new HaunterHeadTrip());
            r.RegisterByKey("RG-35", new ConditionAttackMon("Poison Seed", SpecialCondition.Poisoned));
            r.RegisterByKey("RG-36", new CardsCRI.CounterConditionMon("Poison Payback", SpecialCondition.Poisoned, null));
            r.RegisterByKey("RG-37", new ComboMon(new ParalyzeFlipMon("Lick"), new CardsSSB.AnyOpponentTextMon("Tongue Whip", 20)));
            r.RegisterByKey("RG-38", new MischiefMon());
            r.RegisterByKey("RG-39", new MetapodEnergyProtection());
            r.RegisterByKey("RG-40", new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 2));
            r.RegisterByKey("RG-41", new ComboMon(new CoinsMon("Double Stab", 2),
                new CardsFST2.ConditionBonusMon("Rend", 30, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("RG-42", new TunnelingMon());
            r.RegisterByKey("RG-43", new ComboMon(new EnergyPowderMon(), new ConditionAttackMon("Toxic Spore", SpecialCondition.Poisoned)));
            r.RegisterByKey("RG-44", new CardsRG.ThickSkinMon(new ComboMon(new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned),
                new ConditionAttackMon("Shining Claws", SpecialCondition.Confused))));
            r.RegisterByKey("RG-45", new NoRetreatMon("Clutch"));
            r.RegisterByKey("RG-46", new ComboMon(new EnergyStreamMon(), new ParalyzeFlipMon("Bubble")));
            r.RegisterByKey("RG-47", new ComboMon(new DataRetrievalMon(), new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("RG-48", new CardsRG.ThickSkinMon(new ComboMon(new PickupMon(), new FlipBonusMon("Quick Attack", 40))));
            r.RegisterByKey("RG-49", new CardsRG.ProtectiveDustMon(new ComboMon(
                new CardsCES1.ConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned), new TailsNothingMon("Razor Wind"))));
            r.RegisterByKey("RG-50", new ComboMon(new CardsBS1.ExtraWaterMon("Water Gun"), new SwitchSelfMon("Smash Turn", true)));
            r.RegisterByKey("RG-51", new ConditionAttackMon("Corrosive Acid", SpecialCondition.Burned));
            r.RegisterByKey("RG-52", new WigglytuffAssistance());
            r.RegisterByKey("RG-53", new PlainMon());
            r.RegisterByKey("RG-54", new CardsCES1.FlipConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned));
            r.RegisterByKey("RG-55", new FlipBonusMon("Gouge", 10));
            r.RegisterByKey("RG-56", new ComboMon(new SignsOfEvolutionMon(), new ParalyzeFlipMon("String Shot")));
            r.RegisterByKey("RG-57", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("RG-58", new PlainMon());
            r.RegisterByKey("RG-59", new CardsBS1.LureMon("Gesture"));
            r.RegisterByKey("RG-60", new CardsSSB.AnyOpponentTextMon("Linear Attack", 20));
            r.RegisterByKey("RG-61", new DigUnderMon());
            r.RegisterByKey("RG-62", new ComboMon(new SwitchSelfMon("Run Around", false), new CardsRSB.CoinsPlusMon("Random Peck", 2, 10)));
            r.RegisterByKey("RG-63", new CardsMA.DelayedConditionMon("Slow Trip Gas", SpecialCondition.Confused));
            r.RegisterByKey("RG-64", new PlainMon());
            r.RegisterByKey("RG-65", new ComboMon(new CardsE2.SleepInducerMon(false), new FlipBonusMon("Quick Blow", 20)));
            r.RegisterByKey("RG-66", new NapMon());
            r.RegisterByKey("RG-67", new ComboMon(new TailsNothingMon("Surprise Attack"), new CardsSIT1.AscensionMon()));
            r.RegisterByKey("RG-68", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("RG-69", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("RG-70", new LookForFriendsMon());
            r.RegisterByKey("RG-71", new ComboMon(new CardsSS.BenchNamedMon("Call for Family", new[] { "Nidoran ♀", "Nidoran ♂" }, false, 1), new CoinsMon("Double Stab", 2)));
            r.RegisterByKey("RG-72", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new CardsPAR2.FlipConditionMon("Toxic Spore", SpecialCondition.Poisoned)));
            r.RegisterByKey("RG-73", new NoRetreatMon("Corner"));
            r.RegisterByKey("RG-74", new CardsE1.PlasmaMon());
            r.RegisterByKey("RG-75", new CardsPAR2.FlipConditionMon("Spiral Attack", SpecialCondition.Confused));
            r.RegisterByKey("RG-76", new FlipBonusMon("Stomp", 10));
            r.RegisterByKey("RG-77", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("RG-78", new TailsNothingMon("Horn Hazard"));
            r.RegisterByKey("RG-79", new ShieldAttackMon("Minimize", 20));
            r.RegisterByKey("RG-80", new CardsCEC1.ConfusionWaveMon("Confusion Wave"));
            r.RegisterByKey("RG-81", new TailsNothingMon("Claw"));
            r.RegisterByKey("RG-82", new CardsPAR2.FlipConditionMon("Sleepy Ball", SpecialCondition.Asleep));
            r.RegisterByKey("RG-83", new ComboMon(new ParalyzeFlipMon("Bubble"), new SwitchSelfMon("Smash Turn", true)));
            r.RegisterByKey("RG-84", new PsychoWavesMon());
            r.RegisterByKey("RG-85", new CardsRG.EnergyRetreatMon(true, new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("RG-86", new ComboMon(new GrassFamilyMon(), new CardsDRB.PoisonSpurtMon()));
        }

        /// <summary>Head Trip (Poké-Power): once during your turn, if Haunter is on your Bench, your Active Pokémon is now Confused. Confuse Ray.</summary>
        private sealed class HaunterHeadTrip : Mon
        {
            private const string Power = "Head Trip";
            private static readonly Mon Ray = new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx) { ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Confused); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        /// <summary>Assistance (Poké-Power): once during your turn, if Wigglytuff is on your Bench, remove 1 Special Condition from your Active Pokémon. Expand.</summary>
        private sealed class WigglytuffAssistance : Mon
        {
            private const string Power = "Assistance";
            private static readonly Mon Expand = new ShieldAttackMon("Expand", 10);
            private static readonly SpecialCondition[] All =
                { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && ctx.Me.Bench.Contains(ctx.Pokemon) && active.Conditions != SpecialCondition.None;
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var has = All.Where(active.Has).ToList();
                if (has.Count == 0) return;
                var pick = has[ctx.ChooseOneOption(Power + ": remove which Special Condition from " + active.Name + "?", has.Select(c => c.ToString()).ToArray())];
                ctx.Engine.RemoveCondition(active, pick);
                ctx.Log(Power + ": " + active.Name + " is no longer " + pick + ".");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Expand.HandleAttack(ctx, atk); }
        }

        /// <summary>Mischief: shuffle your opponent's deck.</summary>
        private sealed class MischiefMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mischief") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + "'s deck was shuffled (Mischief).");
                return true;
            }
        }

        /// <summary>Energy Protection (Poké-Body): attack damage to Metapod is 10 less for each Energy attached to it (up to 30 less). Sharpen.</summary>
        private sealed class MetapodEnergyProtection : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || me.Energy.Count == 0) return damage;
                return damage - Math.Min(30, 10 * me.Energy.Count);
            }
        }

        /// <summary>Tunneling: 10 to each of up to 2 of the opponent's Benched Pokémon; Onix can't attack during your next turn.</summary>
        private sealed class TunnelingMon : Mon
        {
            private static readonly Mon Hit = new CardsG1.ChooseBenchedMon("Tunneling", 2, 10, false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!Hit.HandleAttack(ctx, atk)) return false;
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Energy Powder: up to 2 basic Energy cards from your deck onto your Pokémon (not Pokémon-ex) in any way you like.</summary>
        private sealed class EnergyPowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Powder") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Energy Powder: choose up to 2 basic Energy cards");
                foreach (var card in found)
                {
                    var spots = ctx.MyPokemon(m => !m.Def.IsOldEx);
                    var target = spots.Count == 0 ? null : ctx.ChoosePokemon(spots, "Energy Powder: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Energy Stream: a basic Energy card from your discard pile onto Poliwhirl.</summary>
        internal sealed class EnergyStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Stream") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0 || !ctx.InPlay(me)) return true;
                var card = ctx.ChooseCards(basics, 1, 1, "Energy Stream: attach which basic Energy card?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Data Retrieval: with fewer than 4 cards in your hand, draw until you have 4.</summary>
        private sealed class DataRetrievalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Data Retrieval") return false;
                int n = 4 - ctx.Me.Hand.Count;
                if (n > 0 && ctx.Me.Deck.Count > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Pickup: a Pokémon card, a Trainer card and an Energy card from your discard pile into your hand (each optional if there is none).</summary>
        private sealed class PickupMon : Mon
        {
            private static readonly Func<CardDef, bool>[] Kinds = { d => d.IsPokemon, d => d.IsTrainer, d => d.IsEnergy };
            private static readonly string[] Names = { "a Basic Pokémon or Evolution card", "a Trainer card", "an Energy card" };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pickup") return false;
                var taken = new List<CardInstance>();
                for (int i = 0; i < Kinds.Length; i++)
                {
                    var kind = Kinds[i];
                    var options = ctx.Me.Discard.Where(c => kind(c.Def)).ToList();
                    if (options.Count == 0) continue;
                    var pick = ctx.ChooseCards(options, 1, 1, "Pickup: choose " + Names[i])[0];
                    if (ctx.Me.Discard.Remove(pick))
                    {
                        ctx.Me.Hand.Add(pick);
                        taken.Add(pick);
                    }
                }
                if (taken.Count > 0) ctx.Engine.Reveal(ctx.Player, taken, "put these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " hand (Pickup)");
                return true;
            }
        }

        /// <summary>Signs of Evolution: a Metapod and a Butterfree card from your deck into your hand.</summary>
        private sealed class SignsOfEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Signs of Evolution") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && (d.Name == "Metapod" || d.Name == "Butterfree"), 2, "Signs of Evolution: choose a Metapod and a Butterfree card");
                var kept = new List<CardInstance>();
                foreach (var card in found)
                {
                    if (kept.Any(k => k.Def.Name == card.Def.Name)) ctx.Me.Deck.Add(card);
                    else
                    {
                        kept.Add(card);
                        ctx.Me.Hand.Add(card);
                    }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Dig Under: 10 damage to 1 of the opponent's Pokémon, not affected by Weakness or Resistance.</summary>
        internal sealed class DigUnderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Under") return false;
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 10), true);
                return true;
            }
        }

        /// <summary>Nap: 2 damage counters come off Krabby.</summary>
        private sealed class NapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nap") return false;
                if (ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 20);
                return true;
            }
        }

        /// <summary>Look for Friends: reveal cards from the top of your deck until you reveal a Basic Pokémon; it goes into your hand, the rest are shuffled back.</summary>
        internal sealed class LookForFriendsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Look for Friends") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                int index = ctx.Me.Deck.FindIndex(c => c.Def.IsBasicPokemon);
                var revealed = ctx.Me.Deck.Take(index < 0 ? ctx.Me.Deck.Count : index + 1).ToList();
                ctx.Engine.Reveal(ctx.Player, revealed, "revealed these cards from the top of " + ctx.Engine.Their(ctx.Player) + " deck (Look for Friends)");
                if (index >= 0)
                {
                    var found = ctx.Me.Deck[index];
                    ctx.Me.Deck.RemoveAt(index);
                    ctx.Me.Hand.Add(found);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Psycho Waves: discard an Energy card attached to Venonat; the Defending Pokémon is now Confused.</summary>
        internal sealed class PsychoWavesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psycho Waves") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me) && me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, 1, "Psycho Waves: discard an Energy card");
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Call for Family: up to 2 Grass Basic Pokémon from your deck onto your Bench.</summary>
        private sealed class GrassFamilyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.HasType(EnergyType.Grass), 2, "Call for Family: put up to 2 Grass Basic Pokémon onto your Bench");
                return true;
            }
        }
    }
}
