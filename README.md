# KardsAgainstKrews
An online implementation of a similarly-named party card game.

Kards Against Krews is a casual party game for up to 32 players in which black "prompt" cards are revealed, and players submit white "response" cards to fill in the blanks or answer open-ended questions.

Each round, an appointed judge will select the card they like best (traditionally judging based on humour, but it's entirely up to the judge's discretion).

This project is powered by the LowLevelNetworking package written by my long-time friend and mentor Jos Yule. Taking it for a test drive on this project was as enjoyable a ride as I'd hoped.

## Quick Setup Guide
1. Install \*.cardpack files
	- Refer to the [Installing Card Packs](https://github.com/AdamEaton/KardsAgainstKrews#installing-card-packs) section for information on creating and installing card packs
2. The host player should start the game and select `HOST GAME` (creating the Server Instance)
	- The host player must enable port forwarding to their device on port 7777 (if not already enabled) to enable remote connections
3. The remaining players (and the host, if they're playing) should each launch an instance of the game and select `JOIN GAME` (creating a Client Instance for each player)
4. All players should enter the IP address of the host player
	- If the host is running the Server and Client Instances on the same device, then `127.0.0.1` should be entered in their Client Instance
	- For players connected to the same network as the host, the host's *local* IP address should be entered
	- For remote players, the external IP address displayed in the Server Instance should be entered
5. Each player should choose a display name to finalize their registration
6. Once all players are registered, the host may set the game configuration and start the game
	- Refer to the [Game Metrics](https://github.com/AdamEaton/KardsAgainstKrews#game-metrics) section for information on the effects of the different options

## Gameplay
Each round proceeds as follows:
1. A new judge and prompt are selected
2. All other players will play a response to the prompt; if the prompt contains multiple blanks, the players must play enough cards to fulfill each of them
	- To play a card, click the card once to highlight it, then again to confirm
	- Blank white cards function as "write your own" responses, and will trigger a prompt to type in a response
3. Once all responses have been submitted, all responses are revealed to all players
4. The judge selects their favourite response, earning it 1 point
	- As the judge, press the large `>` button to continue to the next round

## Host Privileges
As the host, there are a handful of tools available to keep the game moving smoothly.

Because the game was designed with small, familiar groups in mind, there are no built-in timers. If a player is not paying attention when their input is needed, clicking the `!` next to that player's name in the Server Instance will flash the player's screen to regain their attention. The `PING STRAGGLERS` button is a convenient way to ping all players that the game is waiting on an action from. For players that have abandoned their post completely (or simply gotten on your bad side), the `X` button next to their name will kick them out of the room.

For temporary delays, the `SHOW RESPONSES` button will immediately reveal the current responses and allow the judge to select a winner, and the `NEXT ROUND` button will immediately select a new judge and begin the next round.

## Game Metrics
- Hand Size: The standard number of cards each player will have when playing on a single-response prompt; more cards will automatically be given out for multiple response prompts
- Target Score: The number of points a player must earn to win and end the game
- Blank Responses: The number of blank "write your own" response cards to include in the game
- Personal Responses: The number of copies of each player's name to include as response cards
- Checkpoint Frequency: The number of regular rounds to play before each checkpoint round, wherein players have an opportunity to remove any undesired cards from their hand
- Maximum Response Cards: The total number of response cards (from all players) that can be played during a round; prompts that require too many response cards will be automatically skipped if this option is enabled
- Pack Responses: Enable or disable pre-written responses from card packs; when disabled, only blank and personal response cards will be dealt to players

## Installing Card Packs
Card packs are automatically loaded by the game from the `%AppData%\..\LocalLow\Adam Eaton\Kards Against Krews\Packs\` directory. The game will attempt to parse all `\*.cardpack` files placed in this directory and offer them as optional inclusions in the server's new game menu.

A \*.cardpack file is parsed line-by-line. The first line is the pack's display name. The next line, as well as all non-empty following lines are interpreted as prompt cards. Once the first blank line (or the end of the file) is found, the remaining lines are interpreted as response cards.
- Card text cannot include newline characters (as they are used as delimiters in the pack list).
- Prompts can contain any number of blanks to fill in, represented by a `_` in the text.
- Prompts without blanks will accept one response card by default
- Each additional blank after the first will require an additional response card from players, as well as cause them to draw an additional card before responding
- Prompts and responses can both include `~` characters, which will be automatically replaced with a randomly chosen player's name
- Prompts and responses can also use Unity's UI.Text rich formatting options
	- The most commonly used formatting is \<i\>italics\</i\> for emphasis

For convenience, the included `Template.cardpack` file also explains and demonstrates this formatting.
