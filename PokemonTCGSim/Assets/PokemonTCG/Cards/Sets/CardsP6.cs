using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>POP Series 6 (P6, 2007; v172): the cards not already in the pool (the others are prints). Unlimited only.</summary>
    public static class CardsP6
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P6-7", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("P6-9", new CardsFST1.TwoBenchedMon("Spark", 10));
            r.RegisterByKey("P6-15", new FlipBonusMon("Water Spash", 10));
        }
    }
}
