using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsPLEra;

namespace PokemonTCG.Cards
{
    /// <summary>POP Series 9 (P9, 2009; v173): the cards not already in the pool (Gible is a print). Unlimited only.</summary>
    public static class CardsP9
    {
        /// <summary>This Pokémon evolved from the named Pokémon during this turn.</summary>
        internal static bool JustEvolvedFrom(EffectContext c, string name)
        {
            var me = c.Pokemon;
            return me != null && me.TurnEvolved == c.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == name;
        }

        internal static bool InPlayNamed(EffectContext c, string name) { return c.MyPokemon(m => m.Name == name).Count > 0; }

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P9-1", new Fx()
                .On("Dragon Rage", (c, a) => c.FlipCoins(2) == 2 ? c.DamageDefending(a.Damage) >= 0 : DidNothing(c))
                .After("Jet Sword", (c, a) =>
                {
                    c.DiscardEnergyFrom(c.Pokemon, 2, "Jet Sword: discard 2 Energy attached to " + c.Pokemon.Name);
                    HitEachBenched(c, 10);
                }));
            r.RegisterByKey("P9-2", new Fx()
                .Effect("Dive", (c, a) =>
                {
                    var top = c.LookAtTop(7);
                    if (top.Count == 0) return;
                    c.DeckToHand(c.ChooseCards(top, 1, 1, "Dive: put which card into your hand?"));
                    c.ShuffleMyDeck();
                })
                .Effect("Water Glow", (c, a) =>
                {
                    var t = c.ChoosePokemon(c.OpponentPokemon(), "Water Glow: choose 1 of your opponent's Pokémon");
                    if (t == null) return;
                    int done = c.DamageTarget(t, 20);
                    if (done > 0 && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
                }));
            r.RegisterByKey("P9-3", new Fx()
                .With(new CoinsMon("Slam", 2))
                .Damage("High Volt", (c, a) => JustEvolvedFrom(c, "Pikachu") ? 100 : a.Damage));
            r.RegisterByKey("P9-4", new Fx()
                .With(new ParalyzeFlipMon("Body Slam"))
                .Damage("Double Stomp", (c, a) => a.Damage + 20 * c.FlipCoins(2)));
            r.RegisterByKey("P9-5", new PowerFx("Type Shift", null, c =>
                {
                    c.AddEffect(new CardsGE.TypeIsEffect(c.Pokemon.Name + " is Psychic this turn (Type Shift)", c.Player, c.Pokemon.Uid, c.Turn, EnergyType.Psychic));
                }, new Fx()
                .Damage("Poltergeist", (c, a) =>
                {
                    LookAtHand(c, c.Opp);
                    return a.Damage + 10 * c.Opp.Hand.Count(x => x.Def.IsTrainer);
                })));
            r.RegisterByKey("P9-6", new Fx()
                .With(new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool"))
                .After("Super Fast", (c, a) =>
                {
                    if (InPlayNamed(c, "Pachirisu") && c.InPlay(c.Pokemon) && c.Flip()) c.ProtectSelfNextTurn();
                }));
            r.RegisterByKey("P9-7", new Fx()
                .Effect("Knock Off", (c, a) =>
                {
                    if (!c.Flip()) return;
                    var card = RandomFromHand(c, c.Opp);
                    if (card != null) c.Engine.DiscardFromHand(c.Opponent, card);
                })
                .Effect("Nimble", (c, a) =>
                {
                    int done = c.DamageDefending(a.Damage);
                    if (done > 0 && InPlayNamed(c, "Turtwig") && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, done);
                }));
            r.RegisterByKey("P9-8", new Fx()
                .Effect("Burrow", (c, a) =>
                {
                    if (c.Flip()) c.AddEffect(new NoDamageEffect(c.Pokemon.Name + " is protected from attack damage (Burrow)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                })
                .Before("Distored Wave", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && d.DamageCounters > 0) c.Heal(d, 20);
                }));
            r.RegisterByKey("P9-9", new Fx()
                .Effect("Jump Kick", (c, a) => HitBenched(c, 1, 20, "Jump Kick"))
                .After("Jazzed", (c, a) =>
                {
                    if (JustEvolvedFrom(c, "Buneary") && c.InPlay(c.Pokemon) && c.Pokemon.DamageCounters > 0) c.Heal(c.Pokemon, c.Pokemon.DamageCounters * 10);
                }));
            r.RegisterByKey("P9-10", new Fx()
                .With(new ParalyzeFlipMon("Thunder Wave"))
                .On("Poison Berry", (c, a) =>
                {
                    bool friend = InPlayNamed(c, "Croagunk");
                    var d = c.Defending;
                    c.DamageDefending(a.Damage + (friend ? 20 : 0));
                    if (friend && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Poisoned);
                    return true;
                }));
            r.RegisterByKey("P9-11", new CardsSS.BabyEvolutionMon("Pikachu", new Fx()
                .Effect("Find a Friend", (c, a) =>
                {
                    if (c.Flip()) c.SearchToHand(d => d.IsPokemon, 1, "Find a Friend: search your deck for a Pokémon");
                })));
            r.RegisterByKey("P9-12", new Fx()
                .With(new CoinsMon("Dizzy Punch", 2))
                .Effect("Defense Curl", (c, a) =>
                {
                    if (c.Flip()) c.AddEffect(new NoDamageEffect(c.Pokemon.Name + " is protected from attack damage (Defense Curl)", c.Player, c.Pokemon.Uid, c.Turn + 1));
                }));
            r.RegisterByKey("P9-13", new Fx()
                .With(new CoinsMon("Serial Swipes", 4))
                .On("Sleepy", (c, a) => FriendBonus(c, a, "Piplup", SpecialCondition.Asleep)));
            r.RegisterByKey("P9-15", new Fx()
                .Effect("Growl", (c, a) =>
                {
                    var d = c.Defending;
                    if (d != null && c.CanAffect(d)) c.AddEffect(new WeakenAttackerEffect(d.Name + "'s attacks do 20 less damage next turn (Growl)", c.Player, d.Uid, c.Turn + 1, 20));
                })
                .After("Numb", (c, a) =>
                {
                    var d = c.Defending;
                    if (JustEvolvedFrom(c, "Pichu") && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, SpecialCondition.Paralyzed);
                }));
            r.RegisterByKey("P9-16", new Fx()
                .Damage("Water Sport", (c, a) => a.Damage + (c.Defending != null && c.EnergyCount(c.Pokemon) < c.EnergyCount(c.Defending) ? 10 : 0))
                .After("Wavelet", (c, a) => { if (InPlayNamed(c, "Buizel")) HitEachBenched(c, 10); }));
            r.RegisterByKey("P9-17", new Fx()
                .With(new CardsTWM1.HealSelfMon("Absorb", 10))
                .On("Parboil", (c, a) => FriendBonus(c, a, "Chimchar", SpecialCondition.Burned)));
        }

        /// <summary>"If you have [friend] in play, this attack does N plus 20 more damage and the Defending Pokémon is now [condition]."</summary>
        private static bool FriendBonus(EffectContext c, AttackDef a, string friend, SpecialCondition condition)
        {
            bool has = InPlayNamed(c, friend);
            var d = c.Defending;
            c.DamageDefending(a.Damage + (has ? 20 : 0));
            if (has && d != null && c.InPlay(d) && c.CanAffect(d)) c.ApplyCondition(d, condition);
            return true;
        }
    }
}
