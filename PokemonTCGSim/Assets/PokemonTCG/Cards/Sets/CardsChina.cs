using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// v156.17: China-only cards, never released in English, added as their own cards (your choice). Captain Pikachu (Gem Pack
    /// Vol. 5 CBB5C 01 07/07, regulation mark H, so Standard; the Gem Pack Vol. 1 CBB1C 07 09/09 is a print of it).
    /// </summary>
    public static class CardsChina
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("CBB5C-107", new RecoilMon("Volt Tackle", 30));
        }
    }
}
