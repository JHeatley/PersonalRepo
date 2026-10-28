using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// XY (XY, v105), part 1: the Grass, Fire, Water, Lightning and Psychic Pokémon (1-57). Venusaur-EX, M Venusaur-EX,
    /// Blastoise-EX and M Blastoise-EX are Evolutions' cards; Shellder, Cloyster, Pikachu and Raichu are Generations' cards.
    /// </summary>
    public static class CardsXY1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("XY-3", new CardsMEW1.DefendingTypeBonusMon("Leaf Munch", EnergyType.Grass, 20));
            r.RegisterByKey("XY-4", new CardsLOR1.HardenMon("Harden", 60));
            r.RegisterByKey("XY-5", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned), new FlashNeedleMon()));
            r.RegisterByKey("XY-7", new Cards30CC.BenchHitMon("Mach Punch", 10));
            r.RegisterByKey("XY-8", new ComboMon(new CardsDAA1.GustAttackMon("Luring Glow", false), new ConditionAttackMon("Signal Beam", SpecialCondition.Confused)));
            r.RegisterByKey("XY-9", new ComboMon(new SearchAttackMon("Pheromotion", d => d.IsPokemon && d.PokemonType == EnergyType.Grass, 1), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("XY-10", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("XY-11", new CardsPOR.MemoryLockMon("Torment"));
            r.RegisterByKey("XY-12", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("XY-13", new ComboMon(new CardsSP1.FlipNoDamageMon("Scrunch"), new RecoilMon("Wood Hammer", 10)));
            r.RegisterByKey("XY-14", new CardsASC.CounterattackMon("Spiky Shield", 3, new CardsTWM1.HealSelfMon("Touchdown", 20)));
            r.RegisterByKey("XY-16", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("XY-17", new ComboMon(new ConversionPowderMon(), new CardsXYP2.JudgmentBlastMon("Colorful Wind")));
            r.RegisterByKey("XY-18", new FlipLeadMon());
            r.RegisterByKey("XY-19", new ComboMon(new SearchAttackMon("Lead", d => d.IsSupporter, 2), new CardsSUM5.OptionalBoostRecoilMon("Charge Dash", 20, 20)));
            r.RegisterByKey("XY-20", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("XY-21", new MagmaMantleMon());
            r.RegisterByKey("XY-22", new CardsTWM1.SelfDiscardMon("Fireworks", 1));
            r.RegisterByKey("XY-23", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new CardsSUM1.DiscardTypeFromSelfMon("Flamethrower", EnergyType.Fire, 1)));
            r.RegisterByKey("XY-25", new ComboMon(new CardsBST1.RadarMon("Clairvoyant Eye", 3), new CardsSUM1.DiscardTypeFromSelfMon("Fire Tail Slap", EnergyType.Fire, 1)));
            r.RegisterByKey("XY-26", new DelphoxFire());
            r.RegisterByKey("XY-27", new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1));
            r.RegisterByKey("XY-28", new ComboMon(new CardsPAL1.DevastatingWindMon(), new CardsSUM1.DiscardTypeFromSelfMon("Flare Blitz", EnergyType.Fire, 99)));
            r.RegisterByKey("XY-33", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("XY-34", new ComboMon(new CardsLOR1.RecoverMon(),
                new CardsFST2.ConditionBonusMon("Core Splash", 30, c => c.Pokemon.Energy.Any(en => CardsDRI1.ProvidesType(c, c.Pokemon, en, EnergyType.Psychic)))));
            r.RegisterByKey("XY-35", new ComboMon(new SeafaringMon(), new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20)));
            r.RegisterByKey("XY-36", new ComboMon(new RefreshMon(), new CardsMEG.FlipUntilTailsBonusMon("Spiny Rush", 20)));
            r.RegisterByKey("XY-37", new FlipBonusMon("Water Splash", 20));
            r.RegisterByKey("XY-38", new RecycleMon());
            r.RegisterByKey("XY-39", new FlipSwitchSelfMon("Bounce"));
            r.RegisterByKey("XY-40", new ParalyzeFlipMon("Lick"));
            r.RegisterByKey("XY-41", new GreninjaShuriken());
            r.RegisterByKey("XY-44", new VoltorbBurst());
            r.RegisterByKey("XY-45", new CardsRCL1.FlipDiscardOpponentEnergyMon("Eerie Impulse"));
            r.RegisterByKey("XY-46", new ComboMon(new EnergyGlideMon(), new OptionalDiscardBonusMon("Electron Crush", 30)));
            r.RegisterByKey("XY-48", new ComboMon(new GastroAcidMon(), new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("XY-50", new ComboMon(new CardsEVS1.TrickyStepsMon(), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("XY-51", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("XY-52", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("XY-53", new ComboMon(new CardsWHT.FlipMoreMon("Random Peck", 2, 20), new CardsPOR.PoisonRingMon("Poison Ring")));
            r.RegisterByKey("XY-54", new CardsEVS1.AstonishMon("Astonish"));
            r.RegisterByKey("XY-55", new TrevenantCurse());
            r.RegisterByKey("XY-56", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("XY-57", new ComboMon(new EerieVoiceMon(), new SpiritScreamMon()));
        }

        // ================================================================== Shared helpers

        /// <summary>"You may discard an Energy attached to this Pokémon. If you do, this attack does N more damage." (Electron Crush).</summary>
        internal sealed class OptionalDiscardBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public OptionalDiscardBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                bool boost = me.Energy.Count > 0 && ctx.AskYesNo(attack + ": discard an Energy from " + me.Name + " for " + bonus + " more damage?")
                    && ctx.DiscardEnergyFrom(me, 1, attack + ": discard an Energy from " + me.Name) > 0;
                ctx.DamageDefending(atk.Damage + (boost ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, switch this Pokémon with 1 of your Benched Pokémon." (Bounce).</summary>
        internal sealed class FlipSwitchSelfMon : Mon
        {
            private readonly string attack;

            public FlipSwitchSelfMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0 && ctx.Flip() && ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Flash Needle: flip 3 coins, 40 for each heads; all heads, prevent all effects of attacks done to this Pokémon next turn.</summary>
        private sealed class FlashNeedleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Needle") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads == 3 && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Conversion Powder: choose Asleep or Poisoned for the opponent's Active Pokémon.</summary>
        private sealed class ConversionPowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Conversion Powder") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending == null) return true;
                int pick = ctx.ChooseOption("Conversion Powder: which Special Condition?", "Asleep", "Poisoned");
                ctx.ApplyCondition(defending, pick == 0 ? SpecialCondition.Asleep : SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Lead (Skiddo): flip a coin; heads, search your deck for a Supporter card and put it into your hand.</summary>
        private sealed class FlipLeadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lead") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => d.IsSupporter, 1, "Lead: choose a Supporter card");
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Magma Mantle: you may discard the top card of your deck; if it's a Fire Energy card, 50 more damage.</summary>
        private sealed class MagmaMantleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magma Mantle") return false;
                bool boost = false;
                if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Magma Mantle: discard the top card of your deck (50 more damage if it's a Fire Energy card)?"))
                {
                    var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                    boost = top.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
                }
                ctx.DamageDefending(atk.Damage + (boost ? 50 : 0));
                return true;
            }
        }

        /// <summary>Mystical Fire: once during your turn, draw until you have 6 cards in your hand. Blaze Ball: 20 more for each Fire Energy.</summary>
        private sealed class DelphoxFire : Mon
        {
            private static readonly Mon Ball = new CardsMEG.EnergyBonusMon("Blaze Ball", EnergyType.Fire, 20);

            protected override string Activated { get { return "Mystical Fire"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(6); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ball.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Seafaring: flip 3 coins; for each heads, attach a Water Energy card from your discard pile to your Benched Pokémon in any way.</summary>
        internal sealed class SeafaringMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Seafaring") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                if (heads == 0 || pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                ctx.AttachCardsAnyWay(pile, heads, (card, mon) => ctx.Me.Bench.Contains(mon), "Seafaring", false);
                return true;
            }
        }

        /// <summary>Refresh: heal 30 damage from this Pokémon and it recovers from all Special Conditions.</summary>
        private sealed class RefreshMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Refresh") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Heal(me, 30);
                ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }

        /// <summary>Recycle: put a card from your discard pile on top of your deck.</summary>
        private sealed class RecycleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recycle") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool trainersBlocked = ctx.Engine.TrainerDiscardToDeckBlocked(ctx.Player);
                var pile = ctx.Me.Discard.Where(c => !trainersBlocked || !c.Def.IsTrainer).ToList();
                if (pile.Count == 0) return true;
                var pick = ctx.ChooseCards(pile, 1, 1, "Recycle: put which card from your discard pile on top of your deck?");
                if (pick.Count == 0 || !ctx.Me.Discard.Remove(pick[0])) return true;
                ctx.Me.Deck.Insert(0, pick[0]);
                ctx.Log(pick[0].Def.Name + " was put on top of " + ctx.Engine.Their(ctx.Player) + " deck (Recycle).");
                return true;
            }
        }

        /// <summary>Water Shuriken: once during your turn, discard a Water Energy card from your hand to put 3 damage counters on 1 of the opponent's Pokémon. Mist Slash.</summary>
        private sealed class GreninjaShuriken : Mon
        {
            private static readonly Mon Slash = new CardsCRZ1.PiercingMon("Mist Slash");

            protected override string Activated { get { return "Water Shuriken"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.HasPokemonInPlay && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water));
            }

            protected override void Ability(EffectContext ctx)
            {
                var water = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, water[0]);
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Water Shuriken: put 3 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slash.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Destiny Burst: Knocked Out in the Active Spot by the opponent's attack, flip a coin; heads, 5 damage counters on the Attacking Pokémon.</summary>
        private sealed class VoltorbBurst : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player) return;
                Announce(src, "Destiny Burst");
                if (src.Flip()) src.Engine.PlaceDamageCounters(attacker, 5, "Destiny Burst");
            }
        }

        /// <summary>Energy Glide: search your deck for a Lightning Energy card and attach it to this Pokémon; if you did, switch it with 1 of your Benched Pokémon.</summary>
        private sealed class EnergyGlideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Glide") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(me)) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 1, "Energy Glide: choose a Lightning Energy card");
                bool attached = false;
                foreach (var en in found) attached |= ctx.Engine.AttachEnergyCard(en, me);
                ctx.ShuffleMyDeck();
                if (attached && ctx.Me.Bench.Count > 0 && ctx.IsActive(me)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Gastro Acid: the Defending Pokémon has no Abilities until the end of your next turn.</summary>
        private sealed class GastroAcidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gastro Acid") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsUNB1.NoAbilityEffect(defending.Name + " has no Abilities until the end of " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Gastro Acid)",
                        ctx.Player, defending.Uid, ctx.Turn + 2));
                return true;
            }
        }

        /// <summary>Forest's Curse: while this Pokémon is Active, the opponent can't play Item cards from their hand. Tree Slam: 20 to 2 Benched Pokémon.</summary>
        private sealed class TrevenantCurse : Mon
        {
            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                var me = src.Pokemon;
                return me != null && player != src.Player && card != null && card.Def.IsItem && src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tree Slam") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                int n = Math.Min(2, bench.Count);
                foreach (var target in ctx.ChoosePokemonMany(bench, n, n, "Tree Slam: choose 2 of your opponent's Benched Pokémon (20 damage each)"))
                    ctx.DamageTarget(target, 20);
                return true;
            }
        }

        /// <summary>Eerie Voice: 2 damage counters on each of the opponent's Pokémon.</summary>
        private sealed class EerieVoiceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eerie Voice") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 2);
                return true;
            }
        }

        /// <summary>Spirit Scream: put damage counters on both Active Pokémon until each has 10 HP left.</summary>
        private sealed class SpiritScreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirit Scream") return false;
                var defending = ctx.Defending;
                if (defending != null)
                {
                    int n = (ctx.RemainingHp(defending) - 10) / 10;
                    if (n > 0) ctx.PlaceCounters(defending, n);
                }
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.IsActive(me))
                {
                    int own = (ctx.RemainingHp(me) - 10) / 10;
                    if (own > 0) ctx.PlaceCounters(me, own);
                }
                return true;
            }
        }
    }
}
