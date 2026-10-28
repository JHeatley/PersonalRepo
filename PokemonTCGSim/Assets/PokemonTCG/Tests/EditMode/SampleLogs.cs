namespace PokemonTCG.Tests
{
    /// <summary>Pokémon TCG Live battle logs you sent (v76), exactly as exported, for the log reader tests.</summary>
    internal static class SampleLogs
    {
        /// <summary>TFR89 (Mega Venusaur / Crustle) vs Ivanmp1702 (Mega Lucario), conceded on turn 6.</summary>
        public const string Crustle = @"Setup
TFR89 chose tails for the opening coin flip.
Ivanmp1702 won the coin toss.
Ivanmp1702 decided to go first.
TFR89 drew 7 cards for the opening hand.
- 7 drawn cards.
   • Lillie's Determination, Basic Grass Energy, Crustle, Bayleef, Chikorita, Basic Grass Energy, Basic Grass Energy
Ivanmp1702 drew 7 cards for the opening hand.
- 7 drawn cards.
TFR89 played Chikorita to the Active Spot.
Ivanmp1702 played Makuhita to the Active Spot.
Ivanmp1702 played Lunatone to the Bench.

Ivanmp1702's Turn
Ivanmp1702 drew a card.
Ivanmp1702 attached Basic Fighting Energy to Makuhita in the Active Spot.
Ivanmp1702 played Premium Power Pro.
Ivanmp1702 played Fighting Gong.
- Ivanmp1702 drew Riolu.
- Ivanmp1702 shuffled their deck.
Ivanmp1702 played Riolu to the Bench.
Ivanmp1702 ended their turn.

TFR89's Turn
TFR89 drew Bulbasaur.
TFR89 played Bulbasaur to the Bench.
TFR89 attached Basic Grass Energy to Chikorita in the Active Spot.
TFR89 played Lillie's Determination.
- TFR89 shuffled 4 cards into their deck.
   • Basic Grass Energy, Crustle, Bayleef, Basic Grass Energy
- TFR89 drew 8 cards.
   • Lillie's Determination, Xerosic's Machinations, Bug Catching Set, Ultra Ball, Mega Venusaur ex, Dwebble, Night Stretcher, Basic Grass Energy
TFR89 played Dwebble to the Bench.
TFR89 played Bug Catching Set.
- TFR89 drew 2 cards.
   • Bulbasaur, Basic Grass Energy
- TFR89 shuffled their deck.
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Bulbasaur, Basic Grass Energy
- TFR89 drew Teal Mask Ogerpon ex.
- TFR89 shuffled their deck.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Special Red Card.
TFR89's Chikorita used Growl.

Ivanmp1702's Turn
Ivanmp1702 drew a card.
Ivanmp1702 evolved Makuhita to Hariyama in the Active Spot.
Ivanmp1702's Hariyama used Heave-Ho Catcher.
- TFR89's Dwebble was switched with TFR89's Chikorita to become the Active Pokémon.
TFR89's Dwebble is now in the Active Spot.
Ivanmp1702 played Salvatore.
- Ivanmp1702 evolved Riolu to Mega Lucario ex on the Bench.
- Ivanmp1702 shuffled their deck.
Ivanmp1702 attached Air Balloon to Mega Lucario ex on the Bench.
Ivanmp1702 ended their turn.

TFR89's Turn
TFR89 drew Poké Pad.
TFR89 played Poké Pad.
- TFR89 drew Ivysaur.
- TFR89 shuffled their deck.
TFR89 evolved Bulbasaur to Ivysaur on the Bench.
TFR89 played Night Stretcher.
- TFR89 moved TFR89's Basic Grass Energy to their hand.
TFR89 attached Basic Grass Energy to Dwebble in the Active Spot.
TFR89 played Lillie's Determination.
- TFR89 shuffled 3 cards into their deck.
   • Xerosic's Machinations, Mega Venusaur ex, Special Red Card
- TFR89 drew 8 cards.
   • Lillie's Determination, Xerosic's Machinations, Crustle, Basic Grass Energy, Lillie's Determination, Night Stretcher, Ultra Ball, Forest of Vitality
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Bayleef.
TFR89 evolved Chikorita to Bayleef on the Bench.
TFR89 played Forest of Vitality to the Stadium spot.
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Lillie's Determination, Crustle
- TFR89 drew Mega Venusaur ex.
- TFR89 shuffled their deck.
TFR89 evolved Ivysaur to Mega Venusaur ex on the Bench.
TFR89 played Night Stretcher.
- TFR89 moved TFR89's Crustle to their hand.
TFR89 evolved Dwebble to Crustle in the Active Spot.
TFR89's Mega Venusaur ex used Solar Transfer.
- TFR89 attached Basic Grass Energy to Crustle in the Active Spot.
- TFR89 attached Basic Grass Energy to Crustle in the Active Spot.
- TFR89 attached Basic Grass Energy to Mega Venusaur ex on the Bench.
TFR89's Crustle used Superb Scissors on Ivanmp1702’s Hariyama for 120 damage.

Ivanmp1702's Turn
Ivanmp1702 drew a card.
Ivanmp1702 ended their turn.

TFR89's Turn
TFR89 drew Forest of Vitality.
TFR89 played Lillie's Determination.
- TFR89 shuffled 2 cards into their deck.
   • Xerosic's Machinations, Forest of Vitality
- TFR89 drew 8 cards.
   • Meganium, Poké Pad, Forest of Vitality, Basic Grass Energy, Chikorita, Basic Grass Energy, Forest of Vitality, Bug Catching Set
TFR89 evolved Bayleef to Meganium on the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Ultra Ball.
Opponent conceded. TFR89 wins.

";

        /// <summary>TFR89 vs superbassagera (Mega Zeraora ex), Mysterious Rock Inn walls, conceded on turn 9.</summary>
        public const string Zeraora = @"Setup
TFR89 chose tails for the opening coin flip.
superbassagera won the coin toss.
superbassagera decided to go first.
TFR89 drew 7 cards for the opening hand.
- 7 drawn cards.
   • Bug Catching Set, Crustle, Bayleef, Basic Grass Energy, Crustle, Dwebble, Lillie's Determination
superbassagera drew 7 cards for the opening hand.
- 7 drawn cards.
TFR89 played Dwebble to the Active Spot.
superbassagera played Tynamo to the Active Spot.

superbassagera's Turn
superbassagera drew a card.
superbassagera played Tynamo to the Bench.
superbassagera played Precious Trolley.
- superbassagera drew 4 cards and played them to the Bench.
   • Pikachu ex, Pikachu, Mega Zeraora ex, Iron Thorns ex
- superbassagera shuffled their deck.
superbassagera attached Basic Lightning Energy to Mega Zeraora ex on the Bench.
superbassagera played Switch.
- superbassagera's Mega Zeraora ex was switched with superbassagera's Tynamo to become the Active Pokémon.
superbassagera's Mega Zeraora ex is now in the Active Spot.
superbassagera ended their turn.

TFR89's Turn
TFR89 drew Switch.
TFR89 attached Basic Grass Energy to Dwebble in the Active Spot.
TFR89 played Bug Catching Set.
- TFR89 drew 2 cards.
   • Basic Grass Energy, Teal Mask Ogerpon ex
- TFR89 shuffled their deck.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Meganium.
TFR89 played Lillie's Determination.
- TFR89 shuffled 5 cards into their deck.
   • Crustle, Bayleef, Crustle, Switch, Meganium
- TFR89 drew 8 cards.
   • Mega Venusaur ex, Lillie's Determination, Teal Mask Ogerpon ex, Dawn, Bayleef, Fezandipiti ex, Poké Pad, Lana's Aid
TFR89 played Poké Pad.
- TFR89 drew Chikorita.
- TFR89 shuffled their deck.
TFR89 played Chikorita to the Bench.
TFR89's Dwebble used Ascension.
- TFR89 evolved Dwebble to Crustle in the Active Spot.
- TFR89 shuffled their deck.

superbassagera's Turn
superbassagera drew a card.
superbassagera attached Basic Grass Energy to Pikachu ex on the Bench.
superbassagera ended their turn.

TFR89's Turn
TFR89 drew Meganium.
TFR89 evolved Chikorita to Bayleef on the Bench.
TFR89 played Lillie's Determination.
- TFR89 shuffled 6 cards into their deck.
   • Mega Venusaur ex, Teal Mask Ogerpon ex, Dawn, Fezandipiti ex, Lana's Aid, Meganium
- TFR89 drew 8 cards.
   • Dawn, Teal Mask Ogerpon ex, Crustle, Boss's Orders, Judge, Bayleef, Boss's Orders, Ultra Ball
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Teal Mask Ogerpon ex, Bayleef
- TFR89 drew Bulbasaur.
- TFR89 shuffled their deck.
TFR89 played Bulbasaur to the Bench.
TFR89 ended their turn.

superbassagera's Turn
superbassagera drew a card.
superbassagera attached Basic Lightning Energy to Mega Zeraora ex in the Active Spot.
superbassagera's Mega Zeraora ex used Thunderous Fist on TFR89’s Crustle for 0 damage.
- Damage breakdown:
   • 2 attached Energy: 120 damage

TFR89's Crustle used Mysterious Rock Inn.
- Damage to Crustle was prevented.

TFR89's Turn
TFR89 drew Basic Grass Energy.
TFR89 played Dawn.
- TFR89 drew 3 cards.
   • Fezandipiti ex, Ivysaur, Meganium
- TFR89 shuffled their deck.
TFR89 evolved Bayleef to Meganium on the Bench.
TFR89 evolved Bulbasaur to Ivysaur on the Bench.
TFR89 attached Basic Grass Energy to Crustle in the Active Spot.
TFR89's Crustle used Superb Scissors on superbassagera’s Mega Zeraora ex for 120 damage.

superbassagera's Turn
superbassagera drew a card.
superbassagera attached Basic Metal Energy to Pikachu ex on the Bench.
superbassagera's Mega Zeraora ex used Thunderous Fist on TFR89’s Crustle for 0 damage.
- Damage breakdown:
   • 2 attached Energy: 120 damage

TFR89's Crustle used Mysterious Rock Inn.
- Damage to Crustle was prevented.

TFR89's Turn
TFR89 drew Forest of Vitality.
TFR89 played Judge.
- TFR89 shuffled 5 cards into their deck.
   • Crustle, Boss's Orders, Boss's Orders, Fezandipiti ex, Forest of Vitality
- superbassagera shuffled 3 cards into their deck.
- TFR89 drew 4 cards.
   • Forest of Vitality, Basic Grass Energy, Meganium, Boss's Orders
- superbassagera drew 4 cards.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Teal Mask Ogerpon ex.
TFR89's Crustle used Superb Scissors on superbassagera’s Mega Zeraora ex for 120 damage.

superbassagera's Turn
superbassagera drew a card.
Opponent conceded. TFR89 wins.

";

        /// <summary>TFR89 vs Bluebird124 (Metagross / Excadrill), a mulligan, Resistance, a Knock Out and a Prize card.</summary>
        public const string Metang = @"Setup
Bluebird124 chose tails for the opening coin flip.
Bluebird124 won the coin toss.
Bluebird124 decided to go first.
Bluebird124 drew 7 cards for the opening hand.
- 7 drawn cards.
TFR89 drew 7 cards for the opening hand.
- 7 drawn cards.
   • Mega Venusaur ex, Bug Catching Set, Basic Grass Energy, Chikorita, Basic Grass Energy, Judge, Ultra Ball
Bluebird124 took a mulligan.
- Cards revealed from Mulligan 1
   • Boss's Orders, Buddy-Buddy Poffin, Magnetic Metal Energy, Mega Excadrill ex, Metagross, Basic Metal Energy, Buddy-Buddy Poffin
TFR89 drew 1 more card because Bluebird124 took at least 1 mulligan.
- TFR89 drew Forest of Vitality.
Bluebird124 played Beldum to the Active Spot.
TFR89 played Chikorita to the Active Spot.

Bluebird124's Turn
Bluebird124 drew a card.
Bluebird124 attached Basic Metal Energy to Beldum in the Active Spot.
Bluebird124 ended their turn.

TFR89's Turn
TFR89 drew Bulbasaur.
TFR89 played Bulbasaur to the Bench.
TFR89 played Bug Catching Set.
- TFR89 drew 2 cards.
   • Chikorita, Basic Grass Energy
- TFR89 shuffled their deck.
TFR89 played Chikorita to the Bench.
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Mega Venusaur ex, Basic Grass Energy
- TFR89 drew Teal Mask Ogerpon ex.
- TFR89 shuffled their deck.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Scoop Up Cyclone.
TFR89 played Judge.
- TFR89 shuffled 3 cards into their deck.
   • Basic Grass Energy, Forest of Vitality, Scoop Up Cyclone
- Bluebird124 shuffled 6 cards into their deck.
- TFR89 drew 4 cards.
   • Judge, Basic Grass Energy, Forest of Vitality, Teal Mask Ogerpon ex
- Bluebird124 drew 4 cards.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Meganium.
TFR89 ended their turn.

Bluebird124's Turn
Bluebird124 drew a card.
Bluebird124 played Ultra Ball.
- Bluebird124 discarded 2 cards.
   • Battle Cage, Mega Skarmory ex
- Bluebird124 drew Genesect ex.
- Bluebird124 shuffled their deck.
Bluebird124 played Genesect ex to the Bench.
Bluebird124's Genesect ex used Metallic Signal.
- Bluebird124 drew 2 cards.
   • Metang, Mega Excadrill ex
- Bluebird124 shuffled their deck.
Bluebird124 evolved Beldum to Metang in the Active Spot.
Bluebird124's Metang used Metal Maker.
- Bluebird124 attached Basic Metal Energy to Metang in the Active Spot.
- TFR89 shuffled their cards.
Bluebird124 attached Magnetic Metal Energy to Metang in the Active Spot.
Bluebird124's Metang used Beam on TFR89’s Chikorita for 60 damage.

TFR89's Turn
TFR89 drew Lillie's Determination.
TFR89 played Forest of Vitality to the Stadium spot.
TFR89 played Lillie's Determination.
- TFR89 shuffled 2 cards into their deck.
   • Judge, Meganium
- TFR89 drew 8 cards.
   • Meganium, Basic Grass Energy, Switch, Basic Grass Energy, Lana's Aid, Basic Grass Energy, Night Stretcher, Poké Pad
TFR89 played Poké Pad.
- TFR89 drew Bayleef.
- TFR89 shuffled their deck.
TFR89 evolved Chikorita to Bayleef on the Bench.
TFR89 evolved Bayleef to Meganium on the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Scoop Up Cyclone.
TFR89 played Switch.
- TFR89's Teal Mask Ogerpon ex was switched with TFR89's Chikorita to become the Active Pokémon.
TFR89's Teal Mask Ogerpon ex is now in the Active Spot.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Dwebble.
TFR89's Teal Mask Ogerpon ex used Myriad Leaf Shower on Bluebird124’s Metang for 210 damage. Bluebird124's Metang took -30 less damage because of Grass Resistance.
- Damage breakdown:
   • Base damage: 30 damage
   • 7 attached Energy: 210 damage
   • Resistance to Grass: -30 damage
   • Total damage: 210 damage

Bluebird124's Metang was Knocked Out!
- 4 cards were discarded from Bluebird124's Metang.
   • Basic Metal Energy, Basic Metal Energy, Magnetic Metal Energy, Beldum
TFR89 took a Prize card.
Mega Venusaur ex was added to TFR89's hand.
Bluebird124's Genesect ex is now in the Active Spot.

Bluebird124's Turn
Bluebird124 drew a card.
Opponent conceded. TFR89 wins.

";

        /// <summary>TFR89 vs trev386 (Mega Gengar ex): Knock Outs, retreats, damage counters, Poison, TCG Live's Sudden Death.</summary>
        public const string Gengar = @"Setup
trev386 chose heads for the opening coin flip.
trev386 won the coin toss.
trev386 decided to go first.
trev386 drew 7 cards for the opening hand.
- 7 drawn cards.
TFR89 drew 7 cards for the opening hand.
- 7 drawn cards.
   • Mega Venusaur ex, Ultra Ball, Bulbasaur, Forest of Vitality, Basic Grass Energy, Chikorita, Basic Grass Energy
trev386 played Gastly to the Active Spot.
trev386 played Gastly to the Bench.
TFR89 played Chikorita to the Active Spot.

trev386's Turn
trev386 drew a card.
trev386 played Buddy-Buddy Poffin.
- trev386 drew 2 cards and played them to the Bench.
   • Dunsparce, Toxel
- trev386 shuffled their deck.
trev386 attached Basic Darkness Energy to Gastly in the Active Spot.
trev386 ended their turn.

TFR89's Turn
TFR89 drew Judge.
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Basic Grass Energy, Mega Venusaur ex
- TFR89 drew Teal Mask Ogerpon ex.
- TFR89 shuffled their deck.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89 played Bulbasaur to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Ultra Ball.
TFR89 played Forest of Vitality to the Stadium spot.
TFR89 played Judge.
- TFR89 shuffled Ultra Ball into their deck.
- trev386 shuffled 4 cards into their deck.
- TFR89 drew 4 cards.
   • Basic Grass Energy, Teal Mask Ogerpon ex, Ultra Ball, Basic Grass Energy
- trev386 drew 4 cards.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Basic Grass Energy.
TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
TFR89 ended their turn.

trev386's Turn
trev386 drew a card.
trev386 played Poké Pad.
- trev386 drew Dudunsparce.
- trev386 shuffled their deck.
trev386 evolved Dunsparce to Dudunsparce on the Bench.
trev386 played Poké Pad.
- trev386 drew Toxtricity.
- trev386 shuffled their deck.
trev386 evolved Toxel to Toxtricity on the Bench.
trev386 played Lillie's Determination.
- trev386 shuffled 2 cards into their deck.
- trev386 drew 8 cards.
trev386 played Buddy-Buddy Poffin.
- trev386 drew Dunsparce and played it to the Bench.
- trev386 shuffled their deck.
trev386 played Gastly to the Bench.
trev386 played Dudunsparce.
- trev386 drew 3 cards.
- trev386 shuffled 2 cards into their deck.
   • Dudunsparce, Dunsparce
trev386 played Risky Ruins to the Stadium spot.
- TFR89 discarded Forest of Vitality.
trev386 played Ultra Ball.
- trev386 discarded 2 cards.
   • Dudunsparce ex, Toxel
- trev386 drew Munkidori.
- trev386 shuffled their deck.
trev386 played Munkidori to the Bench.
Risky Ruins was activated.
- trev386 put 2 damage counters on trev386's Munkidori.
trev386 retreated Gastly to the Bench.
- Basic Darkness Energy was discarded from trev386's Gastly.
trev386's Dunsparce is now in the Active Spot.
trev386's Toxtricity used Sinister Surge.
- trev386 attached Basic Darkness Energy to Gastly on the Bench.
- trev386 shuffled their deck.
- trev386 put 2 damage counters on trev386's Gastly.
trev386 played Night Stretcher.
- trev386 moved trev386's Basic Darkness Energy to their hand.
trev386 attached Basic Darkness Energy to Munkidori on the Bench.
trev386's Munkidori used Adrena-Brain.
- trev386 moved 2 damage counters from trev386's Gastly to trev386's Chikorita.
trev386 ended their turn.

TFR89's Turn
TFR89 drew Lana's Aid.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Basic Grass Energy.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Basic Grass Energy.
TFR89 attached Basic Grass Energy to Chikorita in the Active Spot.
TFR89 retreated Chikorita to the Bench.
- Basic Grass Energy was discarded from TFR89's Chikorita.
TFR89's Teal Mask Ogerpon ex is now in the Active Spot.
TFR89's Teal Mask Ogerpon ex used Myriad Leaf Shower on trev386’s Dunsparce for 120 damage.
- Damage breakdown:
   • Base damage: 30 damage
   • 3 attached Energy: 90 damage
   • Total damage: 120 damage

trev386's Dunsparce was Knocked Out!
TFR89 took a Prize card.
Teal Mask Ogerpon ex was added to TFR89's hand.
trev386's Gastly is now in the Active Spot.

trev386's Turn
trev386 drew a card.
trev386 played Buddy-Buddy Poffin.
- trev386 drew Dunsparce and played it to the Bench.
- trev386 shuffled their deck.
Risky Ruins was activated.
- trev386 put 2 damage counters on trev386's Dunsparce.
trev386's Toxtricity used Sinister Surge.
- trev386 attached Basic Darkness Energy to Gastly on the Bench.
- trev386 shuffled their deck.
- trev386 put 2 damage counters on trev386's Gastly.
trev386 played Lillie's Determination.
- trev386 shuffled 3 cards into their deck.
- trev386 drew 8 cards.
trev386 evolved Gastly to Haunter in the Active Spot.
trev386 played Poké Pad.
- trev386 drew Dudunsparce.
- trev386 shuffled their deck.
trev386 attached Basic Darkness Energy to Gastly on the Bench.
trev386's Munkidori used Adrena-Brain.
- trev386 moved 2 damage counters from trev386's Gastly to trev386's Bulbasaur.
trev386 evolved Gastly to Haunter on the Bench.
trev386 retreated Haunter to the Bench.
- Basic Darkness Energy was discarded from trev386's Haunter.
trev386's Gastly is now in the Active Spot.
trev386 ended their turn.

TFR89's Turn
TFR89 drew Boss's Orders.
TFR89 played Boss's Orders.
- trev386's Haunter was switched with trev386's Gastly to become the Active Pokémon.
trev386's Haunter is now in the Active Spot.
TFR89's Teal Mask Ogerpon ex used Myriad Leaf Shower on trev386’s Haunter for 150 damage.
- Damage breakdown:
   • Base damage: 30 damage
   • 4 attached Energy: 120 damage
   • Total damage: 150 damage

trev386's Haunter was Knocked Out!
- 2 cards were discarded from trev386's Haunter.
   • Basic Darkness Energy, Gastly
TFR89 took a Prize card.
Teal Mask Ogerpon ex was added to TFR89's hand.
trev386's Dunsparce is now in the Active Spot.

trev386's Turn
trev386 drew a card.
trev386 evolved Haunter to Mega Gengar ex on the Bench.
trev386's Toxtricity used Sinister Surge.
- trev386 attached Basic Darkness Energy to Mega Gengar ex on the Bench.
- trev386 shuffled their deck.
- trev386 put 2 damage counters on trev386's Mega Gengar ex.
trev386 evolved Dunsparce to Dudunsparce in the Active Spot.
trev386 played Dudunsparce.
- trev386 drew 3 cards.
- trev386 shuffled 2 cards into their deck.
   • Dudunsparce, Dunsparce
trev386's Mega Gengar ex is now in the Active Spot.
trev386 played Poké Pad.
- trev386 drew Dunsparce.
- trev386 shuffled their deck.
trev386 played Dunsparce to the Bench.
Risky Ruins was activated.
- trev386 put 2 damage counters on trev386's Dunsparce.
trev386 played Grand Tree to the Stadium spot.
- trev386 discarded Risky Ruins.
trev386 played Grand Tree.
- trev386 shuffled their deck.
trev386 played Lillie's Determination.
- trev386 shuffled 4 cards into their deck.
- trev386 drew 8 cards.
trev386 attached Basic Darkness Energy to Mega Gengar ex in the Active Spot.
trev386 played Rare Candy.
- trev386 evolved Gastly to Gengar ex on the Bench.
trev386 played Night Stretcher.
- trev386 moved trev386's Dunsparce to their hand.
trev386's Munkidori used Adrena-Brain.
- trev386 moved 2 damage counters from trev386's Mega Gengar ex to trev386's Bulbasaur.
trev386 played Dunsparce to the Bench.
trev386's Mega Gengar ex used Void Gale on TFR89’s Teal Mask Ogerpon ex for 230 damage.
- trev386 attached Basic Darkness Energy to Gengar ex on the Bench.
TFR89's Teal Mask Ogerpon ex was Knocked Out!
- 3 cards were discarded from TFR89's Teal Mask Ogerpon ex.
   • Basic Grass Energy, Basic Grass Energy, Basic Grass Energy
trev386 took 2 Prize cards.
A card was added to trev386's hand.
A card was added to trev386's hand.
TFR89's Teal Mask Ogerpon ex is now in the Active Spot.

TFR89's Turn
TFR89 drew Night Stretcher.
TFR89 played Grand Tree.
- TFR89 evolved Chikorita to Bayleef on the Bench.
- TFR89 evolved Bayleef to Meganium on the Bench.
- TFR89 shuffled their deck.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89 played Lana's Aid.
- TFR89 moved TFR89's 3 cards to their hand.
   • Basic Grass Energy, Basic Grass Energy, Basic Grass Energy
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex in the Active Spot.
- TFR89 drew Ultra Ball.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Mega Venusaur ex.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Bug Catching Set.
TFR89 played Bug Catching Set.
- TFR89 drew 2 cards.
   • Basic Grass Energy, Crustle
- TFR89 shuffled their deck.
TFR89 played Night Stretcher.
- TFR89 moved TFR89's Teal Mask Ogerpon ex to their hand.
TFR89 played Teal Mask Ogerpon ex to the Bench.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Forest of Vitality.
TFR89 played Forest of Vitality to the Stadium spot.
- trev386 discarded Grand Tree.
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Ultra Ball, Crustle
- TFR89 drew Ivysaur.
- TFR89 shuffled their deck.
TFR89 evolved Bulbasaur to Ivysaur on the Bench.
TFR89 evolved Ivysaur to Mega Venusaur ex on the Bench.
TFR89's Mega Venusaur ex used Solar Transfer.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex in the Active Spot.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex in the Active Spot.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex in the Active Spot.
TFR89's Teal Mask Ogerpon ex used Myriad Leaf Shower on trev386’s Mega Gengar ex for 420 damage.
- Damage breakdown:
   • Base damage: 30 damage
   • 13 attached Energy: 390 damage
   • Total damage: 420 damage

trev386's Mega Gengar ex was Knocked Out!
- 3 cards were discarded from trev386's Mega Gengar ex.
   • Basic Darkness Energy, Haunter, Gastly
TFR89 took 2 Prize cards.
Dwebble was added to TFR89's hand.
Chikorita was added to TFR89's hand.
trev386's Gengar ex is now in the Active Spot.

trev386's Turn
trev386 drew a card.
trev386 evolved Dunsparce to Dudunsparce on the Bench.
trev386's Munkidori used Adrena-Brain.
- trev386 moved 2 damage counters from trev386's Dudunsparce to trev386's Meganium.
trev386 played Dudunsparce.
- trev386 drew 3 cards.
- trev386 shuffled 2 cards into their deck.
   • Dudunsparce, Dunsparce
trev386 played Boss's Orders.
- TFR89's Mega Venusaur ex was switched with TFR89's Teal Mask Ogerpon ex to become the Active Pokémon.
TFR89's Mega Venusaur ex is now in the Active Spot.
trev386 played Pecharunt ex to the Bench.
trev386 attached Basic Darkness Energy to Pecharunt ex on the Bench.
trev386's Gengar ex used Chaotic Pain.
- trev386 put 13 damage counters on trev386's Meganium.
TFR89's Meganium was Knocked Out!
- 2 cards were discarded from TFR89's Meganium.
   • Bayleef, Chikorita
trev386 took a Prize card.
A card was added to trev386's hand.

TFR89's Turn
TFR89 drew Judge.
TFR89 played Chikorita to the Bench.
TFR89 played Judge.
- TFR89 shuffled Dwebble into their deck.
- trev386 shuffled 7 cards into their deck.
- TFR89 drew 4 cards.
   • Lillie's Determination, Dwebble, Switch, Special Red Card
- trev386 drew 4 cards.
TFR89 played Special Red Card.
- trev386 shuffled their hand.
- trev386 put 4 cards on the bottom of their deck.
- trev386 drew 3 cards.
TFR89 played Switch.
- TFR89's Teal Mask Ogerpon ex was switched with TFR89's Mega Venusaur ex to become the Active Pokémon.
TFR89's Teal Mask Ogerpon ex is now in the Active Spot.
TFR89's Teal Mask Ogerpon ex used Myriad Leaf Shower on trev386’s Gengar ex for 270 damage.
- Damage breakdown:
   • Base damage: 30 damage
   • 8 attached Energy: 240 damage
   • Total damage: 270 damage


trev386's Turn
trev386 drew a card.
trev386 evolved Dunsparce to Dudunsparce on the Bench.
trev386 played Dudunsparce.
- trev386 drew 3 cards.
- trev386 shuffled 2 cards into their deck.
   • Dudunsparce, Dunsparce
trev386 attached Basic Darkness Energy to Pecharunt ex on the Bench.
trev386 played Dunsparce to the Bench.
trev386 played Fezandipiti ex to the Bench.
trev386 played Boss's Orders.
- TFR89's Mega Venusaur ex was switched with TFR89's Teal Mask Ogerpon ex to become the Active Pokémon.
TFR89's Mega Venusaur ex is now in the Active Spot.
trev386's Munkidori used Adrena-Brain.
- trev386 moved 3 damage counters from trev386's Gengar ex to trev386's Mega Venusaur ex.
trev386's Gengar ex used Chaotic Pain.
- trev386 put 13 damage counters on trev386's Chikorita.
TFR89's Chikorita was Knocked Out!
trev386 took a Prize card.
A card was added to trev386's hand.

TFR89's Turn
TFR89 drew Basic Grass Energy.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Bulbasaur.
TFR89 played Lillie's Determination.
- TFR89 shuffled 2 cards into their deck.
   • Dwebble, Bulbasaur
- TFR89 drew 6 cards.
   • Dawn, Meganium, Basic Grass Energy, Crustle, Lillie's Determination, Scoop Up Cyclone
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Forest of Vitality.
TFR89's Mega Venusaur ex used Solar Transfer.
- TFR89 attached Basic Grass Energy to Mega Venusaur ex in the Active Spot.
- TFR89 attached Basic Grass Energy to Mega Venusaur ex in the Active Spot.
- TFR89 attached Basic Grass Energy to Mega Venusaur ex in the Active Spot.
- TFR89 attached Basic Grass Energy to Mega Venusaur ex in the Active Spot.
TFR89 played Mega Venusaur ex.
- TFR89's Mega Venusaur ex healed 30 damage.
- trev386 flipped a coin and it landed on heads.
trev386's Gengar ex was Knocked Out!
- 3 cards were discarded from trev386's Gengar ex.
   • Basic Darkness Energy, Basic Darkness Energy, Gastly
TFR89's Mega Venusaur ex was Knocked Out!
- 6 cards were discarded from TFR89's Mega Venusaur ex.
   • Basic Grass Energy, Basic Grass Energy, Basic Grass Energy, Basic Grass Energy, Ivysaur, Bulbasaur
trev386 took 2 Prize cards.
A card was added to trev386's hand.
A card was added to trev386's hand.
TFR89 took 2 Prize cards.
Fezandipiti ex was added to TFR89's hand.
Night Stretcher was added to TFR89's hand.
Entering Sudden Death.

Setup
trev386 chose tails for the opening coin flip.
trev386 won the coin toss.
trev386 decided to go first.
trev386 drew 7 cards for the opening hand.
- 7 drawn cards.
TFR89 drew 7 cards for the opening hand.
- 7 drawn cards.
   • Crustle, Night Stretcher, Forest of Vitality, Basic Grass Energy, Ultra Ball, Basic Grass Energy, Dawn
TFR89 took a mulligan.
- Cards revealed from Mulligan 1
   • Crustle, Night Stretcher, Forest of Vitality, Basic Grass Energy, Ultra Ball, Basic Grass Energy, Dawn
trev386 drew 1 more card because TFR89 took at least 1 mulligan.
- trev386 drew a card.
trev386 played Pecharunt ex to the Active Spot.
trev386 played Toxel to the Bench.
TFR89 played Teal Mask Ogerpon ex to the Active Spot.

trev386's Turn
trev386 drew a card.
trev386 played Risky Ruins to the Stadium spot.
trev386 played Ultra Ball.
- trev386 discarded 2 cards.
   • Ultra Ball, Basic Darkness Energy
- trev386 drew Gastly.
- trev386 shuffled their deck.
trev386 played Gastly to the Bench.
trev386 ended their turn.

TFR89's Turn
TFR89 drew Teal Mask Ogerpon ex.
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex in the Active Spot.
- TFR89 drew Chikorita.
TFR89 played Special Red Card.
- trev386 shuffled their hand.
- trev386 put 3 cards on the bottom of their deck.
- trev386 drew 3 cards.
TFR89 ended their turn.

trev386's Turn
trev386 drew a card.
trev386 played Fezandipiti ex to the Bench.
trev386 played Team Rocket's Petrel.
- trev386 drew Grand Tree.
- trev386 shuffled their deck.
trev386 played Grand Tree to the Stadium spot.
- trev386 discarded Risky Ruins.
trev386 played Grand Tree.
- trev386 evolved Gastly to Haunter on the Bench.
- trev386 evolved Haunter to Mega Gengar ex on the Bench.
- trev386 shuffled their deck.
trev386 attached Basic Darkness Energy to Mega Gengar ex on the Bench.
trev386's Pecharunt ex used Subjugating Chains.
- trev386's Mega Gengar ex was switched with trev386's Pecharunt ex to become the Active Pokémon.
- trev386's Mega Gengar ex is now Poisoned.
trev386's Mega Gengar ex is now in the Active Spot.
trev386 ended their turn.

Pokémon Checkup
1 damage counter was placed on trev386's Mega Gengar ex for the Special Condition Poisoned.

TFR89's Turn
TFR89 drew Forest of Vitality.
TFR89 played Ultra Ball.
- TFR89 discarded 2 cards.
   • Ivysaur, Chikorita
- TFR89 drew Dwebble.
- TFR89 shuffled their deck.
TFR89 played Dwebble to the Bench.
TFR89 attached Basic Grass Energy to Dwebble on the Bench.
TFR89 retreated Teal Mask Ogerpon ex to the Bench.
- Basic Grass Energy was discarded from TFR89's Teal Mask Ogerpon ex.
TFR89's Dwebble is now in the Active Spot.
TFR89 played Lana's Aid.
- TFR89 moved TFR89's 3 cards to their hand.
   • Ivysaur, Chikorita, Basic Grass Energy
TFR89's Teal Mask Ogerpon ex used Teal Dance.
- TFR89 attached Basic Grass Energy to Teal Mask Ogerpon ex on the Bench.
- TFR89 drew Basic Grass Energy.
TFR89's Dwebble used Ascension.
- TFR89 evolved Dwebble to Crustle in the Active Spot.
- TFR89 shuffled their deck.

Pokémon Checkup
1 damage counter was placed on trev386's Mega Gengar ex for the Special Condition Poisoned.

trev386's Turn
trev386 drew a card.
trev386 played Dunsparce to the Bench.
trev386 played Grand Tree.
- trev386 evolved Toxel to Toxtricity on the Bench.
- trev386 shuffled their deck.
trev386's Pecharunt ex used Subjugating Chains.
- trev386's Fezandipiti ex was switched with trev386's Mega Gengar ex to become the Active Pokémon.
- trev386's Fezandipiti ex is now Poisoned.
trev386's Fezandipiti ex is now in the Active Spot.
trev386's Toxtricity used Sinister Surge.
- trev386 attached Basic Darkness Energy to Mega Gengar ex on the Bench.
- trev386 shuffled their deck.
- trev386 put 2 damage counters on trev386's Mega Gengar ex.
trev386 ended their turn.

Pokémon Checkup
1 damage counter was placed on trev386's Fezandipiti ex for the Special Condition Poisoned.

TFR89's Turn
TFR89 drew Ivysaur.
TFR89 played Forest of Vitality to the Stadium spot.
- trev386 discarded Grand Tree.
TFR89 attached Basic Grass Energy to Crustle in the Active Spot.
TFR89 ended their turn.

Pokémon Checkup
1 damage counter was placed on trev386's Fezandipiti ex for the Special Condition Poisoned.

trev386's Turn
trev386 drew a card.
trev386 played Poké Pad.
- trev386 drew Dudunsparce.
- trev386 shuffled their deck.
trev386 evolved Dunsparce to Dudunsparce on the Bench.
trev386 played Dudunsparce.
- trev386 drew 3 cards.
- trev386 shuffled 2 cards into their deck.
   • Dudunsparce, Dunsparce
trev386 played Boss's Orders.
- TFR89's Teal Mask Ogerpon ex was switched with TFR89's Crustle to become the Active Pokémon.
TFR89's Teal Mask Ogerpon ex is now in the Active Spot.
Opponent conceded. TFR89 wins.

";
    }
}
