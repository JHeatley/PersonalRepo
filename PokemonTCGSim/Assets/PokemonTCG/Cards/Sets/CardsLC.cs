using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legendary Collection (LC 1-110, 2002; v143): reprints. 109 cards are prints of cards already in the pool (Base Set, Jungle,
    /// Fossil, Team Rocket, Evolutions, Generations, Celebrations and 30th Celebration Classic Collection cards, a Wizards promo);
    /// your ruling: the old Trainers are the Base Set cards and Mysterious Fossil is Fossil's 10-HP card. Only Gastly 76 is new: its
    /// Lick costs [C] (Fossil's costs [P]).
    /// </summary>
    public static class CardsLC
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LC-76", new ComboMon(new ParalyzeFlipMon("Lick"), new CardsFO.EnergyConversionMon()));
        }
    }
}
