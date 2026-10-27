# Pokémon TCG — Unity front end

A Unity 6 (6000.2.8f1) board for playing against the self-play bot in
`../PokemonTCGBot`. The game itself (rules, cards and the neural network)
runs in a small Python server on your own computer; Unity draws the board and
sends your choices to it over `http://127.0.0.1:8765`. Nothing is hosted
online and nothing costs money.

## Running it

1. Start the game server (leave this terminal open):

   ```
   cd PokemonTCGBot
   pip install -r requirements.txt
   python server.py
   ```

2. In Unity Hub choose **Add > Add project from disk**, pick this
   `PokemonTCGUnity` folder and open it with 6000.2.8f1. The first import
   takes a minute.
3. Press **Play**. There's no scene to set up: the menu builds itself in
   whatever scene is open.
4. Pick your deck, the bot's deck and which bot to play, then **Play**, or
   **Watch bot vs bot** to watch it play itself (use Slower/Faster).

## Playing

- Cards with a yellow outline can be used right now. Click one to play it.
  If a card can do several things (attach to which Pokémon, evolve which
  one...), the options panel narrows to just those.
- Every choice is also listed as a button in the options panel on the right,
  which you can always use instead.
- Hover any card to see it large with its full text.
- The log under the options shows everything both players did.

Card pictures are downloaded from images.pokemontcg.io the first time
they're shown. If they can't be downloaded, cards show as coloured frames
with their names, and the game still works.

## Troubleshooting

- **"Can't reach the game server"**: start `python server.py` first, then
  click *Reconnect to server*. If you changed the port, edit the address on
  the menu.
- **Clicks do nothing**: in *Edit > Project Settings > Player > Active Input
  Handling* choose **Both**, or add the Input System package.
- **"Insecure connection not allowed"**: the editor script
  `Assets/PokemonTCG/Editor/AllowLocalServer.cs` normally allows HTTP to the
  local server; if you removed it, set *Player > Allow downloads over HTTP* to
  *Always allowed*.

## Code

All UI is built from code in `Assets/PokemonTCG/Scripts`:
`TcgApp` (menu, board and game flow), `CardView` (a card on screen),
`TcgClient` (talks to the server), `ImageCache` (card art), `Ui` (uGUI helpers),
`TcgModels` (the JSON the server sends) and `TcgBootstrap` (starts it all on Play).
