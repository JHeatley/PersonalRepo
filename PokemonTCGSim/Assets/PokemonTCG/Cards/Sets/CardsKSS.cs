using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Kalos Starter Set (KSS, v106): the Pokémon (1-32; Furfrou 33 is a print of 32, which is Phantom Forces' card). The Trainers
    /// (Crushing Hammer, Poké Ball, Pokémon Catcher, Potion, Switch, Tierno) were already in the pool.
    /// </summary>
    public static class CardsKSS
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("KSS-1", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("KSS-2", new ComboMon(new DrawAttackMon("Collect", 1), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("KSS-4", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("KSS-5", new FlipBonusMon("Tumbling Attack", 40));
            r.RegisterByKey("KSS-7", new ComboMon(new DrawAttackMon("Collect", 1), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("KSS-9", new FlipBonusMon("Firebreathing", 20));
            r.RegisterByKey("KSS-10", new CardsTWM1.SelfDiscardMon("Fire Blast", 1));
            r.RegisterByKey("KSS-11", new ComboMon(new DrawAttackMon("Collect", 1), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("KSS-13", new CardsWHT.FlipMoreMon("Aqua Wave", 2, 20));
            r.RegisterByKey("KSS-14", new CardsASC.FlipDiscardDefendingEnergyMon("Mat Block"));
            r.RegisterByKey("KSS-17", new FlipBonusMon("Hard Bite", 20));
            r.RegisterByKey("KSS-20", new ParalyzeFlipMon("Wicked Jab"));
            r.RegisterByKey("KSS-21", new CardsPFL.FlipUntilTailsTimesMon("Continuous Slash"));
            r.RegisterByKey("KSS-23", new CardsWHT.FlipMoreMon("Double Stomp", 2, 20));
            r.RegisterByKey("KSS-24", new CardsTWM1.HealSelfMon("Draining Kiss", 10));
            r.RegisterByKey("KSS-26", new FlipBonusMon("Rock Smash", 30));
            r.RegisterByKey("KSS-27", new FlipBonusMon("Sitdown Splash", 20));
            r.RegisterByKey("KSS-28", new FlipBonusMon("Jump On", 20));
            r.RegisterByKey("KSS-31", new TailsNothingMon("Razor Wind"));
        }
    }
}
