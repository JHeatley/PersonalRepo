using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Pokédex heights (v108, for Cedric Juniper): each species' height in decimetres (0.1 m), regional forms included. A card's
    /// species is the longest species name in its card name ("Alolan Vulpix", "Team Rocket's Mewtwo ex", "Dark Charizard").
    /// Pokémon with a rule box (Pokémon-EX, GX, V, ex, BREAK, Prism Star, Radiant...) have no printed height.
    /// </summary>
    public static class PokedexHeights
    {
        private static readonly Dictionary<string, int> Dm = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            { "Abomasnow", 22 }, { "Abra", 9 }, { "Absol", 12 }, { "Accelgor", 8 }, { "Aegislash", 17 }, { "Aerodactyl", 18 },
            { "Aggron", 21 }, { "Aipom", 8 }, { "Alakazam", 15 }, { "Alcremie", 3 }, { "Alolan Diglett", 2 }, { "Alolan Dugtrio", 7 },
            { "Alolan Exeggutor", 109 }, { "Alolan Geodude", 4 }, { "Alolan Golem", 17 }, { "Alolan Graveler", 10 }, { "Alolan Grimer", 7 }, { "Alolan Marowak", 10 },
            { "Alolan Meowth", 4 }, { "Alolan Muk", 10 }, { "Alolan Ninetales", 11 }, { "Alolan Persian", 11 }, { "Alolan Raichu", 7 }, { "Alolan Raticate", 7 },
            { "Alolan Rattata", 3 }, { "Alolan Sandshrew", 7 }, { "Alolan Sandslash", 12 }, { "Alolan Vulpix", 6 }, { "Alomomola", 12 }, { "Altaria", 11 },
            { "Amaura", 13 }, { "Ambipom", 12 }, { "Amoonguss", 6 }, { "Ampharos", 14 }, { "Annihilape", 12 }, { "Anorith", 7 },
            { "Appletun", 4 }, { "Applin", 2 }, { "Araquanid", 18 }, { "Arbok", 35 }, { "Arboliva", 14 }, { "Arcanine", 19 },
            { "Arceus", 32 }, { "Archaludon", 20 }, { "Archen", 5 }, { "Archeops", 14 }, { "Arctibax", 8 }, { "Arctovish", 20 },
            { "Arctozolt", 23 }, { "Ariados", 11 }, { "Armaldo", 15 }, { "Armarouge", 15 }, { "Aromatisse", 8 }, { "Aron", 4 },
            { "Arrokuda", 5 }, { "Articuno", 17 }, { "Audino", 11 }, { "Aurorus", 27 }, { "Avalugg", 20 }, { "Axew", 6 },
            { "Azelf", 3 }, { "Azumarill", 8 }, { "Azurill", 2 }, { "Bagon", 6 }, { "Baltoy", 5 }, { "Banette", 11 },
            { "Barbaracle", 13 }, { "Barboach", 4 }, { "Barraskewda", 13 }, { "Basculegion", 30 }, { "Basculin", 10 }, { "Bastiodon", 13 },
            { "Baxcalibur", 21 }, { "Bayleef", 12 }, { "Beartic", 26 }, { "Beautifly", 10 }, { "Beedrill", 10 }, { "Beheeyem", 10 },
            { "Beldum", 6 }, { "Bellibolt", 12 }, { "Bellossom", 4 }, { "Bellsprout", 7 }, { "Bergmite", 10 }, { "Bewear", 21 },
            { "Bibarel", 10 }, { "Bidoof", 5 }, { "Binacle", 5 }, { "Bisharp", 16 }, { "Blacephalon", 18 }, { "Blastoise", 16 },
            { "Blaziken", 19 }, { "Blipbug", 4 }, { "Blissey", 15 }, { "Blitzle", 8 }, { "Boldore", 9 }, { "Boltund", 10 },
            { "Bombirdier", 15 }, { "Bonsly", 5 }, { "Bouffalant", 16 }, { "Bounsweet", 3 }, { "Braixen", 10 }, { "Brambleghast", 12 },
            { "Bramblin", 6 }, { "Braviary", 15 }, { "Breloom", 12 }, { "Brionne", 6 }, { "Bronzong", 13 }, { "Bronzor", 5 },
            { "Brute Bonnet", 12 }, { "Bruxish", 9 }, { "Budew", 2 }, { "Buizel", 7 }, { "Bulbasaur", 7 }, { "Buneary", 4 },
            { "Bunnelby", 4 }, { "Burmy", 2 }, { "Butterfree", 11 }, { "Buzzwole", 24 }, { "Cacnea", 4 }, { "Cacturne", 13 },
            { "Calyrex", 11 }, { "Camerupt", 19 }, { "Capsakid", 3 }, { "Carbink", 3 }, { "Carkol", 11 }, { "Carnivine", 14 },
            { "Carracosta", 12 }, { "Carvanha", 8 }, { "Cascoon", 7 }, { "Castform", 3 }, { "Caterpie", 3 }, { "Celebi", 6 },
            { "Celesteela", 92 }, { "Centiskorch", 30 }, { "Ceruledge", 16 }, { "Cetitan", 45 }, { "Cetoddle", 12 }, { "Chandelure", 10 },
            { "Chansey", 11 }, { "Charcadet", 6 }, { "Charizard", 17 }, { "Charjabug", 5 }, { "Charmander", 6 }, { "Charmeleon", 11 },
            { "Chatot", 5 }, { "Cherrim", 5 }, { "Cherubi", 4 }, { "Chesnaught", 16 }, { "Chespin", 4 }, { "Chewtle", 3 },
            { "Chi-Yu", 4 }, { "Chien-Pao", 19 }, { "Chikorita", 9 }, { "Chimchar", 5 }, { "Chimecho", 6 }, { "Chinchou", 5 },
            { "Chingling", 2 }, { "Cinccino", 5 }, { "Cinderace", 14 }, { "Clamperl", 4 }, { "Clauncher", 5 }, { "Clawitzer", 13 },
            { "Claydol", 15 }, { "Clefable", 13 }, { "Clefairy", 6 }, { "Cleffa", 3 }, { "Clobbopus", 6 }, { "Clodsire", 18 },
            { "Cloyster", 15 }, { "Coalossal", 28 }, { "Cobalion", 21 }, { "Cofagrigus", 17 }, { "Combee", 3 }, { "Combusken", 9 },
            { "Comfey", 1 }, { "Conkeldurr", 14 }, { "Copperajah", 30 }, { "Corphish", 6 }, { "Corsola", 6 }, { "Corviknight", 22 },
            { "Corvisquire", 8 }, { "Cosmoem", 1 }, { "Cosmog", 2 }, { "Cottonee", 3 }, { "Crabominable", 17 }, { "Crabrawler", 6 },
            { "Cradily", 15 }, { "Cramorant", 8 }, { "Cranidos", 9 }, { "Crawdaunt", 11 }, { "Cresselia", 15 }, { "Croagunk", 7 },
            { "Crobat", 18 }, { "Crocalor", 10 }, { "Croconaw", 11 }, { "Crustle", 14 }, { "Cryogonal", 11 }, { "Cubchoo", 5 },
            { "Cubone", 4 }, { "Cufant", 12 }, { "Cursola", 10 }, { "Cutiefly", 1 }, { "Cyclizar", 16 }, { "Cyndaquil", 5 },
            { "Dachsbun", 5 }, { "Darkrai", 15 }, { "Darmanitan", 13 }, { "Dartrix", 7 }, { "Darumaka", 6 }, { "Decidueye", 16 },
            { "Dedenne", 2 }, { "Deerling", 6 }, { "Deino", 8 }, { "Delcatty", 11 }, { "Delibird", 9 }, { "Delphox", 15 },
            { "Deoxys", 17 }, { "Dewgong", 17 }, { "Dewott", 8 }, { "Dewpider", 3 }, { "Dhelmise", 39 }, { "Dialga", 54 },
            { "Diancie", 7 }, { "Diggersby", 10 }, { "Diglett", 2 }, { "Dipplin", 4 }, { "Ditto", 3 }, { "Dodrio", 18 },
            { "Doduo", 14 }, { "Dolliv", 6 }, { "Dondozo", 120 }, { "Donphan", 11 }, { "Dottler", 4 }, { "Doublade", 8 },
            { "Dracovish", 23 }, { "Dracozolt", 18 }, { "Dragalge", 18 }, { "Dragapult", 30 }, { "Dragonair", 40 }, { "Dragonite", 22 },
            { "Drakloak", 14 }, { "Drampa", 30 }, { "Drapion", 13 }, { "Dratini", 18 }, { "Drednaw", 10 }, { "Dreepy", 5 },
            { "Drifblim", 12 }, { "Drifloon", 4 }, { "Drilbur", 3 }, { "Drizzile", 7 }, { "Drowzee", 10 }, { "Druddigon", 16 },
            { "Dubwool", 13 }, { "Ducklett", 5 }, { "Dudunsparce", 36 }, { "Dugtrio", 7 }, { "Dunsparce", 15 }, { "Duosion", 6 },
            { "Duraludon", 18 }, { "Durant", 3 }, { "Dusclops", 16 }, { "Dusknoir", 22 }, { "Duskull", 8 }, { "Dustox", 12 },
            { "Dwebble", 3 }, { "Eelektrik", 12 }, { "Eelektross", 21 }, { "Eevee", 3 }, { "Eiscue", 14 }, { "Ekans", 20 },
            { "Eldegoss", 5 }, { "Electabuzz", 11 }, { "Electivire", 18 }, { "Electrike", 6 }, { "Electrode", 12 }, { "Elekid", 6 },
            { "Elgyem", 5 }, { "Emboar", 16 }, { "Emolga", 4 }, { "Empoleon", 17 }, { "Enamorus", 16 }, { "Entei", 21 },
            { "Escavalier", 10 }, { "Espathra", 19 }, { "Espeon", 9 }, { "Espurr", 3 }, { "Eternatus", 200 }, { "Excadrill", 7 },
            { "Exeggcute", 4 }, { "Exeggutor", 20 }, { "Exploud", 15 }, { "Falinks", 30 }, { "Farfetch'd", 8 }, { "Farigiraf", 32 },
            { "Fearow", 12 }, { "Feebas", 6 }, { "Fennekin", 4 }, { "Feraligatr", 23 }, { "Ferroseed", 6 }, { "Ferrothorn", 10 },
            { "Fezandipiti", 14 }, { "Fidough", 3 }, { "Finizen", 13 }, { "Finneon", 4 }, { "Flaaffy", 8 }, { "Flabébé", 1 },
            { "Flamigo", 16 }, { "Flapple", 3 }, { "Flareon", 9 }, { "Fletchinder", 7 }, { "Fletchling", 3 }, { "Flittle", 2 },
            { "Floatzel", 11 }, { "Floette", 2 }, { "Floragato", 9 }, { "Florges", 11 }, { "Flutter Mane", 14 }, { "Flygon", 20 },
            { "Fomantis", 3 }, { "Foongus", 2 }, { "Forretress", 12 }, { "Fraxure", 10 }, { "Frigibax", 5 }, { "Frillish", 12 },
            { "Froakie", 3 }, { "Frogadier", 6 }, { "Froslass", 13 }, { "Frosmoth", 13 }, { "Fuecoco", 4 }, { "Furfrou", 12 },
            { "Furret", 18 }, { "Gabite", 14 }, { "Galarian Articuno", 17 }, { "Galarian Corsola", 6 }, { "Galarian Darmanitan", 17 }, { "Galarian Darumaka", 7 },
            { "Galarian Farfetch'd", 8 }, { "Galarian Linoone", 5 }, { "Galarian Meowth", 4 }, { "Galarian Moltres", 20 }, { "Galarian Mr. Mime", 14 }, { "Galarian Ponyta", 8 },
            { "Galarian Rapidash", 17 }, { "Galarian Slowbro", 16 }, { "Galarian Slowking", 18 }, { "Galarian Slowpoke", 12 }, { "Galarian Stunfisk", 7 }, { "Galarian Weezing", 30 },
            { "Galarian Yamask", 5 }, { "Galarian Zapdos", 16 }, { "Galarian Zigzagoon", 4 }, { "Gallade", 16 }, { "Galvantula", 8 }, { "Garbodor", 19 },
            { "Garchomp", 19 }, { "Gardevoir", 16 }, { "Garganacl", 23 }, { "Gastly", 13 }, { "Gastrodon", 9 }, { "Genesect", 15 },
            { "Gengar", 15 }, { "Geodude", 4 }, { "Gholdengo", 12 }, { "Gible", 7 }, { "Gigalith", 17 }, { "Gimmighoul", 3 },
            { "Girafarig", 15 }, { "Giratina", 45 }, { "Glaceon", 8 }, { "Glalie", 15 }, { "Glameow", 5 }, { "Glastrier", 22 },
            { "Gligar", 11 }, { "Glimmet", 7 }, { "Glimmora", 15 }, { "Gliscor", 20 }, { "Gloom", 8 }, { "Gogoat", 17 },
            { "Golbat", 16 }, { "Goldeen", 6 }, { "Golduck", 17 }, { "Golem", 14 }, { "Golett", 10 }, { "Golisopod", 20 },
            { "Golurk", 28 }, { "Goodra", 20 }, { "Goomy", 3 }, { "Gorebyss", 18 }, { "Gossifleur", 4 }, { "Gothita", 4 },
            { "Gothitelle", 15 }, { "Gothorita", 7 }, { "Gouging Fire", 35 }, { "Gourgeist", 9 }, { "Grafaiai", 7 }, { "Granbull", 14 },
            { "Grapploct", 16 }, { "Graveler", 10 }, { "Great Tusk", 22 }, { "Greavard", 6 }, { "Greedent", 6 }, { "Greninja", 15 },
            { "Grimer", 9 }, { "Grimmsnarl", 15 }, { "Grookey", 3 }, { "Grotle", 11 }, { "Groudon", 35 }, { "Grovyle", 9 },
            { "Growlithe", 7 }, { "Grubbin", 4 }, { "Grumpig", 9 }, { "Gulpin", 4 }, { "Gumshoos", 7 }, { "Gurdurr", 12 },
            { "Guzzlord", 55 }, { "Gyarados", 65 }, { "Hakamo-o", 12 }, { "Happiny", 6 }, { "Hariyama", 23 }, { "Hatenna", 4 },
            { "Hatterene", 21 }, { "Hattrem", 6 }, { "Haunter", 16 }, { "Hawlucha", 8 }, { "Haxorus", 18 }, { "Heatmor", 14 },
            { "Heatran", 17 }, { "Heliolisk", 10 }, { "Helioptile", 5 }, { "Heracross", 15 }, { "Herdier", 9 }, { "Hippopotas", 8 },
            { "Hippowdon", 20 }, { "Hisuian Arcanine", 20 }, { "Hisuian Avalugg", 14 }, { "Hisuian Braviary", 17 }, { "Hisuian Decidueye", 16 }, { "Hisuian Electrode", 12 },
            { "Hisuian Goodra", 17 }, { "Hisuian Growlithe", 8 }, { "Hisuian Lilligant", 12 }, { "Hisuian Qwilfish", 5 }, { "Hisuian Samurott", 15 }, { "Hisuian Sliggoo", 7 },
            { "Hisuian Sneasel", 9 }, { "Hisuian Typhlosion", 16 }, { "Hisuian Voltorb", 5 }, { "Hisuian Zoroark", 16 }, { "Hisuian Zorua", 7 }, { "Hitmonchan", 14 },
            { "Hitmonlee", 15 }, { "Hitmontop", 14 }, { "Ho-Oh", 38 }, { "Honchkrow", 9 }, { "Honedge", 8 }, { "Hoopa", 5 },
            { "Hoothoot", 7 }, { "Hoppip", 4 }, { "Horsea", 4 }, { "Houndoom", 14 }, { "Houndour", 6 }, { "Houndstone", 20 },
            { "Huntail", 17 }, { "Hydrapple", 18 }, { "Hydreigon", 18 }, { "Hypno", 16 }, { "Igglybuff", 3 }, { "Illumise", 6 },
            { "Impidimp", 4 }, { "Incineroar", 18 }, { "Indeedee", 9 }, { "Infernape", 12 }, { "Inkay", 4 }, { "Inteleon", 19 },
            { "Iron Boulder", 15 }, { "Iron Bundle", 6 }, { "Iron Crown", 16 }, { "Iron Hands", 18 }, { "Iron Jugulis", 13 }, { "Iron Leaves", 15 },
            { "Iron Moth", 12 }, { "Iron Thorns", 16 }, { "Iron Treads", 9 }, { "Iron Valiant", 14 }, { "Ivysaur", 10 }, { "Jangmo-o", 6 },
            { "Jellicent", 22 }, { "Jigglypuff", 5 }, { "Jirachi", 3 }, { "Jolteon", 8 }, { "Joltik", 1 }, { "Jumpluff", 8 },
            { "Jynx", 14 }, { "Kabuto", 5 }, { "Kabutops", 13 }, { "Kadabra", 13 }, { "Kakuna", 6 }, { "Kangaskhan", 22 },
            { "Karrablast", 5 }, { "Kartana", 3 }, { "Kecleon", 10 }, { "Keldeo", 14 }, { "Kilowattrel", 14 }, { "Kingambit", 20 },
            { "Kingdra", 18 }, { "Kingler", 13 }, { "Kirlia", 8 }, { "Klang", 6 }, { "Klawf", 13 }, { "Kleavor", 18 },
            { "Klefki", 2 }, { "Klink", 3 }, { "Klinklang", 6 }, { "Koffing", 6 }, { "Komala", 4 }, { "Kommo-o", 16 },
            { "Koraidon", 25 }, { "Krabby", 4 }, { "Kricketot", 3 }, { "Kricketune", 10 }, { "Krokorok", 10 }, { "Krookodile", 15 },
            { "Kubfu", 6 }, { "Kyogre", 45 }, { "Kyurem", 30 }, { "Lairon", 9 }, { "Lampent", 6 }, { "Landorus", 15 },
            { "Lanturn", 12 }, { "Lapras", 25 }, { "Larvesta", 11 }, { "Larvitar", 6 }, { "Latias", 14 }, { "Latios", 20 },
            { "Leafeon", 10 }, { "Leavanny", 12 }, { "Lechonk", 5 }, { "Ledian", 14 }, { "Ledyba", 10 }, { "Lickilicky", 17 },
            { "Lickitung", 12 }, { "Liepard", 11 }, { "Lileep", 10 }, { "Lilligant", 11 }, { "Lillipup", 4 }, { "Linoone", 5 },
            { "Litleo", 6 }, { "Litten", 4 }, { "Litwick", 3 }, { "Lokix", 10 }, { "Lombre", 12 }, { "Lopunny", 12 },
            { "Lotad", 5 }, { "Loudred", 10 }, { "Lucario", 12 }, { "Ludicolo", 15 }, { "Lugia", 52 }, { "Lumineon", 12 },
            { "Lunala", 40 }, { "Lunatone", 10 }, { "Lurantis", 9 }, { "Luvdisc", 6 }, { "Luxio", 9 }, { "Luxray", 14 },
            { "Lycanroc", 8 }, { "Mabosstiff", 11 }, { "Machamp", 16 }, { "Machoke", 15 }, { "Machop", 8 }, { "Magby", 7 },
            { "Magcargo", 8 }, { "Magearna", 10 }, { "Magikarp", 9 }, { "Magmar", 13 }, { "Magmortar", 16 }, { "Magnemite", 3 },
            { "Magneton", 10 }, { "Magnezone", 12 }, { "Makuhita", 10 }, { "Malamar", 15 }, { "Mamoswine", 25 }, { "Manaphy", 3 },
            { "Mandibuzz", 12 }, { "Manectric", 15 }, { "Mankey", 5 }, { "Mantine", 21 }, { "Mantyke", 10 }, { "Maractus", 10 },
            { "Mareanie", 4 }, { "Mareep", 6 }, { "Marill", 4 }, { "Marowak", 10 }, { "Marshadow", 7 }, { "Marshtomp", 7 },
            { "Maschiff", 5 }, { "Masquerain", 8 }, { "Maushold", 3 }, { "Mawile", 6 }, { "Medicham", 13 }, { "Meditite", 6 },
            { "Meganium", 18 }, { "Melmetal", 25 }, { "Meloetta", 6 }, { "Meltan", 2 }, { "Meowscarada", 15 }, { "Meowstic", 6 },
            { "Meowth", 4 }, { "Mesprit", 3 }, { "Metagross", 16 }, { "Metang", 12 }, { "Metapod", 7 }, { "Mew", 4 },
            { "Mewtwo", 20 }, { "Mienfoo", 9 }, { "Mienshao", 14 }, { "Mightyena", 10 }, { "Milcery", 2 }, { "Milotic", 62 },
            { "Miltank", 12 }, { "Mime Jr.", 6 }, { "Mimikyu", 2 }, { "Minccino", 4 }, { "Minior", 3 }, { "Minun", 4 },
            { "Miraidon", 35 }, { "Misdreavus", 7 }, { "Mismagius", 9 }, { "Moltres", 20 }, { "Monferno", 9 }, { "Morelull", 2 },
            { "Morgrem", 8 }, { "Morpeko", 3 }, { "Mothim", 9 }, { "Mr. Mime", 13 }, { "Mr. Rime", 15 }, { "Mudbray", 10 },
            { "Mudkip", 4 }, { "Mudsdale", 25 }, { "Muk", 12 }, { "Munchlax", 6 }, { "Munkidori", 10 }, { "Munna", 6 },
            { "Murkrow", 5 }, { "Musharna", 11 }, { "Nacli", 4 }, { "Naclstack", 6 }, { "Naganadel", 36 }, { "Natu", 2 },
            { "Necrozma", 24 }, { "Nickit", 6 }, { "Nidoking", 14 }, { "Nidoqueen", 13 }, { "Nidoran♀", 4 }, { "Nidoran♂", 5 },
            { "Nidorina", 8 }, { "Nidorino", 9 }, { "Nihilego", 12 }, { "Nincada", 5 }, { "Ninetales", 11 }, { "Ninjask", 8 },
            { "Noctowl", 16 }, { "Noibat", 5 }, { "Noivern", 15 }, { "Nosepass", 10 }, { "Numel", 7 }, { "Nuzleaf", 10 },
            { "Nymble", 2 }, { "Obstagoon", 16 }, { "Octillery", 9 }, { "Oddish", 5 }, { "Ogerpon", 12 }, { "Oinkologne", 10 },
            { "Okidogi", 18 }, { "Omanyte", 4 }, { "Omastar", 10 }, { "Onix", 88 }, { "Oranguru", 15 }, { "Orbeetle", 4 },
            { "Oricorio", 6 }, { "Orthworm", 25 }, { "Oshawott", 5 }, { "Overqwil", 25 }, { "Pachirisu", 4 }, { "Palafin", 13 },
            { "Paldean Tauros", 14 }, { "Paldean Wooper", 4 }, { "Palkia", 42 }, { "Palossand", 13 }, { "Palpitoad", 8 }, { "Pancham", 6 },
            { "Pangoro", 21 }, { "Panpour", 6 }, { "Pansage", 6 }, { "Pansear", 6 }, { "Paras", 3 }, { "Parasect", 10 },
            { "Passimian", 20 }, { "Patrat", 5 }, { "Pawmi", 3 }, { "Pawmo", 4 }, { "Pawmot", 9 }, { "Pawniard", 5 },
            { "Pecharunt", 3 }, { "Pelipper", 12 }, { "Perrserker", 8 }, { "Persian", 10 }, { "Petilil", 5 }, { "Phanpy", 5 },
            { "Phantump", 4 }, { "Pheromosa", 18 }, { "Phione", 4 }, { "Pichu", 3 }, { "Pidgeot", 15 }, { "Pidgeotto", 11 },
            { "Pidgey", 3 }, { "Pidove", 3 }, { "Pignite", 10 }, { "Pikachu", 4 }, { "Pikipek", 3 }, { "Piloswine", 11 },
            { "Pincurchin", 3 }, { "Pineco", 6 }, { "Pinsir", 15 }, { "Piplup", 4 }, { "Plusle", 4 }, { "Poipole", 6 },
            { "Politoed", 11 }, { "Poliwag", 6 }, { "Poliwhirl", 10 }, { "Poliwrath", 13 }, { "Poltchageist", 1 }, { "Polteageist", 2 },
            { "Ponyta", 10 }, { "Poochyena", 5 }, { "Popplio", 4 }, { "Porygon", 8 }, { "Porygon-Z", 9 }, { "Porygon2", 6 },
            { "Primarina", 18 }, { "Primeape", 10 }, { "Prinplup", 8 }, { "Probopass", 14 }, { "Psyduck", 8 }, { "Pumpkaboo", 4 },
            { "Pupitar", 12 }, { "Purrloin", 4 }, { "Purugly", 10 }, { "Pyroar", 15 }, { "Pyukumuku", 3 }, { "Quagsire", 14 },
            { "Quaquaval", 18 }, { "Quaxly", 5 }, { "Quaxwell", 12 }, { "Quilava", 9 }, { "Quilladin", 7 }, { "Qwilfish", 5 },
            { "Raboot", 6 }, { "Rabsca", 3 }, { "Raging Bolt", 52 }, { "Raichu", 8 }, { "Raikou", 19 }, { "Ralts", 4 },
            { "Rampardos", 16 }, { "Rapidash", 17 }, { "Raticate", 7 }, { "Rattata", 3 }, { "Rayquaza", 70 }, { "Regice", 18 },
            { "Regidrago", 21 }, { "Regieleki", 12 }, { "Regigigas", 37 }, { "Regirock", 17 }, { "Registeel", 19 }, { "Relicanth", 10 },
            { "Rellor", 2 }, { "Remoraid", 6 }, { "Reshiram", 32 }, { "Reuniclus", 10 }, { "Revavroom", 18 }, { "Rhydon", 19 },
            { "Rhyhorn", 10 }, { "Rhyperior", 24 }, { "Ribombee", 2 }, { "Rillaboom", 21 }, { "Riolu", 7 }, { "Roaring Moon", 20 },
            { "Rockruff", 5 }, { "Roggenrola", 4 }, { "Rolycoly", 3 }, { "Rookidee", 2 }, { "Roselia", 3 }, { "Roserade", 9 },
            { "Rotom", 3 }, { "Rowlet", 3 }, { "Rufflet", 5 }, { "Runerigus", 16 }, { "Sableye", 5 }, { "Salamence", 15 },
            { "Salandit", 6 }, { "Salazzle", 12 }, { "Samurott", 15 }, { "Sandaconda", 38 }, { "Sandile", 7 }, { "Sandshrew", 6 },
            { "Sandslash", 10 }, { "Sandy Shocks", 23 }, { "Sandygast", 5 }, { "Sawk", 14 }, { "Sawsbuck", 19 }, { "Scatterbug", 3 },
            { "Sceptile", 17 }, { "Scizor", 18 }, { "Scolipede", 25 }, { "Scorbunny", 3 }, { "Scovillain", 9 }, { "Scrafty", 11 },
            { "Scraggy", 6 }, { "Scream Tail", 12 }, { "Scyther", 15 }, { "Seadra", 12 }, { "Seaking", 13 }, { "Sealeo", 11 },
            { "Seedot", 5 }, { "Seel", 11 }, { "Seismitoad", 15 }, { "Sentret", 8 }, { "Serperior", 33 }, { "Servine", 8 },
            { "Seviper", 27 }, { "Sewaddle", 3 }, { "Sharpedo", 18 }, { "Shaymin", 2 }, { "Shedinja", 8 }, { "Shelgon", 11 },
            { "Shellder", 3 }, { "Shellos", 3 }, { "Shelmet", 4 }, { "Shieldon", 5 }, { "Shiftry", 13 }, { "Shiinotic", 10 },
            { "Shinx", 5 }, { "Shroodle", 2 }, { "Shroomish", 4 }, { "Shuckle", 6 }, { "Shuppet", 6 }, { "Sigilyph", 14 },
            { "Silcoon", 6 }, { "Silicobra", 22 }, { "Silvally", 23 }, { "Simipour", 10 }, { "Simisage", 11 }, { "Simisear", 10 },
            { "Sinistcha", 2 }, { "Sinistea", 1 }, { "Sirfetch'd", 8 }, { "Sizzlipede", 7 }, { "Skarmory", 17 }, { "Skeledirge", 16 },
            { "Skiddo", 9 }, { "Skiploom", 6 }, { "Skitty", 6 }, { "Skorupi", 8 }, { "Skrelp", 5 }, { "Skuntank", 10 },
            { "Skwovet", 3 }, { "Slaking", 20 }, { "Slakoth", 8 }, { "Sliggoo", 8 }, { "Slither Wing", 32 }, { "Slowbro", 16 },
            { "Slowking", 20 }, { "Slowpoke", 12 }, { "Slugma", 7 }, { "Slurpuff", 8 }, { "Smeargle", 12 }, { "Smoliv", 3 },
            { "Smoochum", 4 }, { "Sneasel", 9 }, { "Sneasler", 13 }, { "Snivy", 6 }, { "Snom", 3 }, { "Snorlax", 21 },
            { "Snorunt", 7 }, { "Snover", 10 }, { "Snubbull", 6 }, { "Sobble", 3 }, { "Solgaleo", 34 }, { "Solosis", 3 },
            { "Solrock", 12 }, { "Spearow", 3 }, { "Spectrier", 20 }, { "Spewpa", 3 }, { "Spheal", 8 }, { "Spidops", 10 },
            { "Spinarak", 5 }, { "Spinda", 11 }, { "Spiritomb", 10 }, { "Spoink", 7 }, { "Sprigatito", 4 }, { "Spritzee", 2 },
            { "Squawkabilly", 6 }, { "Squirtle", 5 }, { "Stakataka", 55 }, { "Stantler", 14 }, { "Staraptor", 12 }, { "Staravia", 6 },
            { "Starly", 3 }, { "Starmie", 11 }, { "Staryu", 8 }, { "Steelix", 92 }, { "Steenee", 7 }, { "Stonjourner", 25 },
            { "Stoutland", 12 }, { "Stufful", 5 }, { "Stunfisk", 7 }, { "Stunky", 4 }, { "Sudowoodo", 12 }, { "Suicune", 20 },
            { "Sunflora", 8 }, { "Sunkern", 3 }, { "Surskit", 5 }, { "Swablu", 4 }, { "Swadloon", 5 }, { "Swalot", 17 },
            { "Swampert", 15 }, { "Swanna", 13 }, { "Swellow", 7 }, { "Swinub", 4 }, { "Swirlix", 4 }, { "Swoobat", 9 },
            { "Sylveon", 10 }, { "Tadbulb", 3 }, { "Taillow", 3 }, { "Talonflame", 12 }, { "Tandemaus", 3 }, { "Tangela", 10 },
            { "Tangrowth", 20 }, { "Tapu Bulu", 19 }, { "Tapu Fini", 13 }, { "Tapu Koko", 18 }, { "Tapu Lele", 12 }, { "Tarountula", 3 },
            { "Tatsugiri", 3 }, { "Tauros", 14 }, { "Teddiursa", 6 }, { "Tentacool", 9 }, { "Tentacruel", 16 }, { "Tepig", 5 },
            { "Terapagos", 2 }, { "Terrakion", 19 }, { "Thievul", 12 }, { "Throh", 13 }, { "Thundurus", 15 }, { "Thwackey", 7 },
            { "Timburr", 6 }, { "Ting-Lu", 27 }, { "Tinkatink", 4 }, { "Tinkaton", 7 }, { "Tinkatuff", 7 }, { "Tirtouga", 7 },
            { "Toedscool", 9 }, { "Toedscruel", 19 }, { "Togedemaru", 3 }, { "Togekiss", 15 }, { "Togepi", 3 }, { "Togetic", 6 },
            { "Torchic", 4 }, { "Torkoal", 5 }, { "Tornadus", 15 }, { "Torracat", 7 }, { "Torterra", 22 }, { "Totodile", 6 },
            { "Toucannon", 11 }, { "Toxapex", 7 }, { "Toxel", 4 }, { "Toxicroak", 13 }, { "Toxtricity", 16 }, { "Tranquill", 6 },
            { "Trapinch", 7 }, { "Treecko", 5 }, { "Trevenant", 15 }, { "Tropius", 20 }, { "Trubbish", 6 }, { "Trumbeak", 6 },
            { "Tsareena", 12 }, { "Turtonator", 20 }, { "Turtwig", 4 }, { "Tympole", 5 }, { "Tynamo", 2 }, { "Type: Null", 19 },
            { "Typhlosion", 17 }, { "Tyranitar", 20 }, { "Tyrantrum", 25 }, { "Tyrogue", 7 }, { "Tyrunt", 8 }, { "Umbreon", 10 },
            { "Unfezant", 12 }, { "Unown", 5 }, { "Ursaluna", 24 }, { "Ursaring", 18 }, { "Urshifu", 19 }, { "Uxie", 3 },
            { "Vanillish", 11 }, { "Vanillite", 4 }, { "Vanilluxe", 13 }, { "Vaporeon", 10 }, { "Varoom", 10 }, { "Veluza", 25 },
            { "Venipede", 4 }, { "Venomoth", 15 }, { "Venonat", 10 }, { "Venusaur", 20 }, { "Vespiquen", 12 }, { "Vibrava", 11 },
            { "Victini", 4 }, { "Victreebel", 17 }, { "Vigoroth", 14 }, { "Vikavolt", 15 }, { "Vileplume", 12 }, { "Virizion", 20 },
            { "Vivillon", 12 }, { "Volbeat", 7 }, { "Volcanion", 17 }, { "Volcarona", 16 }, { "Voltorb", 5 }, { "Vullaby", 5 },
            { "Vulpix", 6 }, { "Wailmer", 20 }, { "Wailord", 145 }, { "Walking Wake", 35 }, { "Walrein", 14 }, { "Wartortle", 10 },
            { "Watchog", 11 }, { "Wattrel", 4 }, { "Weavile", 11 }, { "Weedle", 3 }, { "Weepinbell", 10 }, { "Weezing", 12 },
            { "Whimsicott", 7 }, { "Whirlipede", 12 }, { "Whiscash", 9 }, { "Whismur", 6 }, { "Wigglytuff", 10 }, { "Wiglett", 12 },
            { "Wimpod", 5 }, { "Wingull", 6 }, { "Wishiwashi", 2 }, { "Wo-Chien", 15 }, { "Wobbuffet", 13 }, { "Woobat", 4 },
            { "Wooloo", 6 }, { "Wooper", 4 }, { "Wormadam", 5 }, { "Wugtrio", 12 }, { "Wurmple", 3 }, { "Wynaut", 6 },
            { "Wyrdeer", 18 }, { "Xatu", 15 }, { "Xerneas", 30 }, { "Xurkitree", 38 }, { "Yamask", 5 }, { "Yamper", 3 },
            { "Yanma", 12 }, { "Yanmega", 19 }, { "Yungoos", 4 }, { "Yveltal", 58 }, { "Zacian", 28 }, { "Zamazenta", 29 },
            { "Zangoose", 13 }, { "Zapdos", 16 }, { "Zarude", 18 }, { "Zebstrika", 16 }, { "Zekrom", 29 }, { "Zeraora", 15 },
            { "Zigzagoon", 4 }, { "Zoroark", 16 }, { "Zorua", 7 }, { "Zubat", 8 }, { "Zweilous", 14 }, { "Zygarde", 50 },
        };

        private static string[] byLength;

        private static string[] NamesByLength
        {
            get
            {
                if (byLength == null) byLength = Dm.Keys.OrderByDescending(k => k.Length).ThenBy(k => k, StringComparer.Ordinal).ToArray();
                return byLength;
            }
        }

        /// <summary>The species a Pokémon card is, or null.</summary>
        public static string SpeciesOf(string cardName)
        {
            if (string.IsNullOrEmpty(cardName)) return null;
            foreach (var name in NamesByLength)
            {
                int at = cardName.IndexOf(name, StringComparison.Ordinal);
                while (at >= 0)
                {
                    int end = at + name.Length;
                    bool startOk = at == 0 || cardName[at - 1] == ' ' || cardName[at - 1] == '\'';
                    bool endOk = end == cardName.Length || cardName[end] == ' ' || cardName[end] == '-' || cardName[end] == '\'' || cardName[end] == '&';
                    if (startOk && endOk) return name;
                    at = cardName.IndexOf(name, at + 1, StringComparison.Ordinal);
                }
            }
            return null;
        }

        /// <summary>The card has its height printed on it: a Pokémon without a rule box whose species is known.</summary>
        public static bool HasPrintedHeight(CardDef card)
        {
            return card != null && card.IsPokemon && !card.HasRuleBox && !card.IsPrismStar && HeightOf(card) > 0;
        }

        /// <summary>The species' height in decimetres, or 0 if unknown.</summary>
        public static int HeightOf(CardDef card)
        {
            if (card == null) return 0;
            var species = SpeciesOf(card.Name);
            int dm;
            return species != null && Dm.TryGetValue(species, out dm) ? dm : 0;
        }

        /// <summary>Every height a Pokémon can have, shortest first.</summary>
        public static List<int> AllHeights()
        {
            return Dm.Values.Distinct().OrderBy(h => h).ToList();
        }

        /// <summary>"0.7 m (2'04\")": metres and the feet and inches English cards print.</summary>
        public static string Label(int dm)
        {
            int inches = (int)Math.Round(dm * 3.937007874, MidpointRounding.AwayFromZero);
            return (dm / 10) + "." + (dm % 10) + " m (" + (inches / 12) + "'" + (inches % 12).ToString("00") + "\")";
        }
    }
}
