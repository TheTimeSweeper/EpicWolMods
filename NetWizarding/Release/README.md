# Yes, really
Very barebones online multiplayer setup that fakes it heavily, but might just be decent enough.
- Only works for PVP
- Currently always expecting multiplayer. Uninstall mod to go back to normal
- tons of interactions untested  
- no UI.
- requires port forwarding currently I think.

## How to use (warning, port forward required I think)
Both machines will need to connect to each other as both host and client.
1. Forward the port 6969 on your router
   - if you don't know how to forward your port, give a google
   - if you'd like to play on another port that you may have already forwarded, run the game and edit the config. Host port is your own port, Client port is your friend's port
2. Install the mod and run the game
   - make sure you have the same mods installed, especially if they add any arcana
3. On Both Machines, enter the game and select Versus (co-op coming soon maybe hopefully)
4. On Both Machines, start your input device of choice and spawn the player 1 wizard as normal.
   - this will start a host server on your end, allowing others to connect to you
5. On Both Machines, press C on the keyboard to bring up the IP connect window
6. On Both Machines, type the IP of the other user you want to connect to, and hit enter.
7. You're kinda of in separate worlds, so in order to start a pvp match. You will have to begin the match on both screens manually. 

Once a client is connected to you, If all is well, the second wizard should have spawned and claimed the second "press to join" window.

To disconnect, simply leave the to tile or close the game. You can also manually disconnect by pressing X.

We should really make some UI for this stuff lol.

## How it works
Player 1 on each screen, is controlling the Player 2 on the other screen.
The mod should be networking position, move input, look input, state changes (like casting abilities), and damage events
### Stop reading now and skip this section if you don’t want to spoil the magic
The mod is player-first. if you hit the other wizard on your screen, it will send a successful damage event to the other player. Player 2 on your screen is a totally fake player that only visually moves around and casts spells. The real attacks and damage are happening from player 1 on the other computer and sent to you.  
Yes, this does mean latency is probably gonna be a bitch. Hopefully maybe someday we can add something like rollback. Call this is a proof of concept I suppose.

## Credits
- huge shoutout to brxxzzy on discord. Without their initial attempts I would have not even realized something like this is even possible. Their code also helped jumpstart this project.

## Installation (manual):
- Make sure you have the dependencies installed.
- Download and extract the zip.
- Create a new folder in your Bepinex/Plugins folder
- put all contents of zip into this new folder. be sure to keep file structure in tact.

## Changelog:

`0.1.0`
 - c: