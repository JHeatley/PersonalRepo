using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legendary Treasures (LTR, v108): Grass, Fire and Water Pokémon (1-45). Reshiram (28, 114, RC22) and Zekrom are Celebrations'
    /// Classic Collection cards, Mewtwo-EX too, and Mew-EX (RC24) is the XY promo's card; the Radiant Collection Pokémon are in CardsLTR3.
    /// </summary>
    public static class CardsLTR1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LTR-1", new FlipBonusMon("Flog", 20));
            r.RegisterByKey("LTR-2", new ComboMon(new NoRetreatMon("Bind Down"), new FlipBonusMon("Flog", 30)));
            r.RegisterByKey("LTR-3", new ComboMon(new CardsOBF1.HealBenchedMon("Share", 40), new CoinsMon("Double Stab", 2)));
            r.RegisterByKey("LTR-4", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("LTR-5", new AmbushBiteMon());
            r.RegisterByKey("LTR-6", new FlipBonusMon("Leaf Blade", 30));
            r.RegisterByKey("LTR-7", new WringOutMon());
            r.RegisterByKey("LTR-8", new SerperiorRoyalHeal());
            r.RegisterByKey("LTR-9", new CoinsMon("Leaf Boomerang", 2));
            r.RegisterByKey("LTR-11", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("LTR-12", new LeavannyTailor());
            r.RegisterByKey("LTR-14", new ParalyzeFlipMon("Hard Press"));
            r.RegisterByKey("LTR-15", new ComboMon(new DrawAttackMon("Double Draw", 2), new CardsASC.NextTurnBoostMon("Leaf Wallop", 40)));
            r.RegisterByKey("LTR-16", new CardsMEG.EnergyBonusMon("Gaia Blaster", EnergyType.Grass, 20));
            r.RegisterByKey("LTR-17", new DrawInMon());
            r.RegisterByKey("LTR-18", new Cards30C.SelfCounterBonusMon("Raging Claws", 10));
            r.RegisterByKey("LTR-19", new ComboMon(new TwoOpponentsMon("Split Bomb", 40), new CardsASC.SelfDiscardTypeMon("Scorching Fire", EnergyType.Fire, 1)));
            r.RegisterByKey("LTR-20", new FlipBonusMon("Firebreathing", 10));
            r.RegisterByKey("LTR-21", new ColorCoordinationMon());
            r.RegisterByKey("LTR-22", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new CardsASC.SelfDiscardTypeMon("Fire Blast", EnergyType.Fire, 1)));
            r.RegisterByKey("LTR-23", new VictiniLTR());
            r.RegisterByKey("LTR-24", new ComboMon(new TurboEnergizeMon(), new CardsFST2.ConditionBonusMon("Intensifying Burn", 50, c => c.Defending != null && c.Defending.Def.IsPokemonEX)));
            r.RegisterByKey("LTR-25", new RecoilMon("Take Down", 10));
            r.RegisterByKey("LTR-26", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("LTR-27", new CardsWHT.EmboarWHT());
            r.RegisterByKey("LTR-29", new ComboMon(new FlipBonusMon("Glinting Claw", 30), new CardsFST1.FlipRecoilMon("Brave Fire", 50)));
            r.RegisterByKey("LTR-30", new CardsFST1.FlipRecoilMon("Flailing Flop", 10));
            r.RegisterByKey("LTR-31", new CardsASR1.TimesMon("Howling Rampage", c => c.MyPrizesTaken + c.OpponentPrizesTaken));
            r.RegisterByKey("LTR-32", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("LTR-33", new FlipBonusMon("Water Splash", 20));
            r.RegisterByKey("LTR-34", new FlipBonusMon("Water Splash", 30));
            r.RegisterByKey("LTR-35", new CardsFST2.ConditionBonusMon("Emperor's Strike", 60, c => c.Defending != null && c.RemainingHp(c.Pokemon) < c.RemainingHp(c.Defending)));
            r.RegisterByKey("LTR-36", new PhioneLTR());
            r.RegisterByKey("LTR-37", new FlipBonusMon("Razor Shell", 20));
            r.RegisterByKey("LTR-38", new CardsGEN1.FlipPerEnergyMoreMon("Aqua Tail", EnergyType.Water, 10));
            r.RegisterByKey("LTR-39", new CardsFST1.TakeLessMon(20, new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10)));
            r.RegisterByKey("LTR-41", new CardsBLK.RoundMon());
            r.RegisterByKey("LTR-42", new CardsBLK.RoundMon());
            r.RegisterByKey("LTR-43", new ComboMon(new Cards30C.SelfCounterBonusMon("Outrage", 10), new SpreadMon("Glaciate", 30)));
            r.RegisterByKey("LTR-44", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Frozen Wings", false), new LockAfterAttackMon("Hail Blizzard", true)));
            r.RegisterByKey("LTR-45", new KeldeoRushIn());
        }

        // ================================================================== Shared

        /// <summary>"This attack does N damage to each of your opponent's Pokémon" (Weakness and Resistance only for the Active one).</summary>
        internal sealed class SpreadMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SpreadMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>"This attack does N damage to 2 of your opponent's Pokémon" (Charizard's Split Bomb).</summary>
        internal sealed class TwoOpponentsMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public TwoOpponentsMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                CardsDRI1.DamageTwoOpponents(ctx, amount, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// "Move as many [type] Energy attached to your Pokémon to your other Pokémon in any way you like" (Serperior's Leaf Tornado):
        /// one at a time until Done is pressed (like Mew-EX's Replace).
        /// </summary>
        internal sealed class MoveEnergyAnyWayMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType? type;

            public MoveEnergyAnyWayMon(string attack, EnergyType? type)
            {
                this.attack = attack;
                this.type = type;
            }

            private bool Movable(EffectContext ctx, PokemonInPlay holder, CardInstance energy)
            {
                return type == null || CardsDRI1.ProvidesType(ctx, holder, energy, type.Value);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                Func<List<PokemonInPlay>> givers = () => ctx.MyPokemon(m => m.Energy.Any(e => Movable(ctx, m, e)));
                RepeatableAbility.Repeat(ctx, () => ctx.MyPokemon().Count >= 2 && givers().Count > 0, offerDone =>
                {
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, givers(), attack + ": move an Energy from which of your Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var movable = giver.Energy.Where(e => Movable(ctx, giver, e)).ToList();
                    if (movable.Count == 0) return false;
                    var card = ctx.ChooseCards(movable, 1, 1, attack + ": move which Energy from " + giver.Name + "?")[0];
                    var receiver = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != giver), attack + ": move " + card.Def.Name + " to which Pokémon?", offerDone);
                    return receiver != null && ctx.Engine.MoveEnergy(giver, receiver, card);
                });
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Ambush Bite: flip a coin; heads, 20 more damage and discard an Energy from the Defending Pokémon.</summary>
        private sealed class AmbushBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ambush Bite") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, atk.Name + ": discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Wring Out: flip a coin; heads, the Defending Pokémon is Paralyzed and loses an Energy.</summary>
        private sealed class WringOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wring Out") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, atk.Name + ": discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Royal Heal: between turns (Pokémon Checkup), heal 10 damage from each of your Pokémon. Leaf Tornado: move Grass Energy around.</summary>
        private sealed class SerperiorRoyalHeal : Mon
        {
            private static readonly Mon Tornado = new MoveEnergyAnyWayMon("Leaf Tornado", EnergyType.Grass);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var hurt = src.MyPokemon(m => m.DamageCounters > 0 && !src.Engine.HealingBlocked(m));
                if (hurt.Count == 0) return;
                Announce(src, "Royal Heal");
                foreach (var mon in hurt) src.Heal(mon, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tornado.HandleAttack(ctx, atk); }
        }

        /// <summary>Leaf Tailor: each of your Pokémon with any Energy attached has no Weakness. Cutting Arm: 20 more per heads (2 coins).</summary>
        private sealed class LeavannyTailor : Mon
        {
            private static readonly Mon Arm = new CardsWHT.FlipMoreMon("Cutting Arm", 2, 20);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && mon.Energy.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Arm.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Draw In: attach 2 Fire Energy cards from your discard pile to this Pokémon (as many as there are, up to 2).</summary>
        private sealed class DrawInMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draw In") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count == 0) return true;
                var picks = fire.Count <= 2 ? fire : ctx.ChooseCards(fire, 2, 2, "Draw In: attach which 2 Fire Energy cards to " + me.Name + "?");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Color Coordination: 40 more if this Pokémon has a basic Energy card attached of the Defending Pokémon's type.</summary>
        private sealed class ColorCoordinationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Color Coordination") return false;
                var defending = ctx.Defending;
                bool match = false;
                if (defending != null)
                {
                    var types = ctx.Engine.TypesOf(defending);
                    match = ctx.Pokemon.Energy.Any(e => e.Def.IsBasicEnergy && types.Contains(e.Def.EnergyProvides));
                }
                ctx.DamageDefending(atk.Damage + (match ? 40 : 0));
                return true;
            }
        }

        /// <summary>Victory Star: once during your turn, flip an attack's coins again (1 Victory Star per turn). Stored Power: move all its Energy to 1 Benched Pokémon.</summary>
        private sealed class VictiniLTR : Mon
        {
            public override string OwnAttacksReflipAbility(EffectContext src) { return "Victory Star"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stored Power") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Stored Power: move all of " + me.Name + "'s Energy to which Benched Pokémon?");
                if (target == null) return true;
                foreach (var card in me.Energy.ToList()) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        /// <summary>
        /// Turbo Energize: search your deck for up to 2 basic Energy cards and attach them to your Benched Pokémon in any way you like.
        /// One given to a Pokémon protected from the attack (Sigilyph's Safeguard) is discarded.
        /// </summary>
        private sealed class TurboEnergizeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Energize") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                if (ctx.Me.Bench.Count == 0)
                {
                    ctx.ShuffleMyDeck();
                    return true;
                }
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Turbo Energize: choose up to 2 basic Energy cards for your Benched Pokémon");
                foreach (var card in found)
                {
                    var bench = ctx.MyBench();
                    if (bench.Count == 0) { ctx.Me.Deck.Add(card); continue; }
                    var target = ctx.ChoosePokemon(bench, "Turbo Energize: attach " + card.Def.Name + " to which Benched Pokémon?");
                    // A Pokémon protected from the attack's effects (Sigilyph's Safeguard) can be chosen, but the Energy is discarded (2013-06-20).
                    if (target != null && (!ctx.CanAffect(target) || CardsLTR2.SafeguardMon.StopsOwnAttack(ctx, target)))
                    {
                        ctx.Me.Discard.Add(card);
                        ctx.Log("Turbo Energize: " + target.Name + " is protected, so " + card.Def.Name + " is discarded.");
                        continue;
                    }
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Pull Out: put a card from your discard pile on top of your deck. Aqua Boomerang: 20, then return this Pokémon and its cards to your hand.</summary>
        private sealed class PhioneLTR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Pull Out")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var pile = ctx.Me.Discard.ToList();
                    if (pile.Count == 0) return true;
                    var pick = ctx.ChooseCards(pile, 1, 1, "Pull Out: put which card from your discard pile on top of your deck?")[0];
                    if (!ctx.Me.Discard.Remove(pick)) return true;
                    ctx.Me.Deck.Insert(0, pick);
                    ctx.Log("Pull Out: " + ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Def.Name + " from the discard pile on top of the deck.");
                    return true;
                }
                if (atk.Name == "Aqua Boomerang")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (ctx.InPlay(me) && ctx.Engine.CanPutIntoHand(me)) ctx.Engine.ReturnPokemonToHand(me);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Rush In: once during your turn, if it's on your Bench, switch it with your Active Pokémon. Secret Sword: 20 more per Water Energy.</summary>
        private sealed class KeldeoRushIn : Mon
        {
            private static readonly Mon Sword = new CardsMEG.EnergyBonusMon("Secret Sword", EnergyType.Water, 20);

            protected override string Activated { get { return "Rush In"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx) { ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Sword.HandleAttack(ctx, atk); }
        }
    }
}
