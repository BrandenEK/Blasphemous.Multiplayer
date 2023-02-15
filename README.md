# Blasphemous Multiplayer

A co-op mod for Blasphemous.  This is still a work in progress without an official release.

Progress board: [Trello](https://trello.com/b/FBdBWoVM/blasphemous-multiplayer)

---

## Features

- See other players as you both move around the world of Cvstodia
- Sync items, stats, skills, and other game progress across all players
- [Coming soon] Compatible with randomizer
- [Coming soon] Enable PvP to fight other players

---

## Things that sync

- All inventory items (Beads / Relics / Prayers / Hearts / Collectibles / Quest items)
- All player stats (Max health / Max fervour / Strength / Mea Culpa level / Max flasks / Flask level / Bead slots)
- All unlocked sword skills
- All revealed map cells
- Most world state progress (Bosses defeated / Gates opened / Cherubs freed / Teleports unlocked / Questline progress / etc. )

## Things that don't sync yet

- Church donation amount
- Most npc questlines

## Things that will probably never sync

- Enemies & bosses (This is a huge task and not really feasible)

---

## Requirements

#### Server:
- Microsoft .NET Core 3.1 (https://dotnet.microsoft.com/en-us/download/dotnet/3.1)

#### Client:
- Most recent version of the Blasphemous game (https://store.steampowered.com/app/774361/Blasphemous)
- A network connection to the machine running the server.  If not on the same local network, find out how to use port forwarding or Hamachi (https://vpn.net/)

---

## How to use

#### Server:
1. Unzip the BlasServer.zip file anywhere on the host machine
2. Run the BlasServer.exe program

#### Client:
1. Extract the contents of the BlasClient.zip file into the game's root folder
2. Run the game and verify on the title screen that the mod has been loaded
3. Run the connect command in the console and start a game

---

## Available commands
- Press the 'backslash' key to open the debug console
- Type the desired command followed by the parameters all separated by a single space

| Command | Parameters | Description |
| ------- | ----------- | ------- |
| `multiplayer help` | none | List all available commands |
| `multiplayer status` | none | Display connection status |
| `multiplayer connect` | SERVER, NAME, PASSWORD (Optional) | Connect to the specified server IP address |
| `multiplayer disconnect` | none | Disconnect from current server |
| `multiplayer team` | NUMBER | Change to a different team (1-10) |
| `multiplayer players` | none | List all connected players in the server |

---

## Configuration settings
- These settings can be modified in the 'multiplayer.cfg' file generated in the same folder as the application

### Server:

| Setting | Description | Default |
| ------- | ----------- | :-----: |
| serverPort | The port to start the server on | 8989 |
| maxPlayers| The maximum number of players to allow on the server| 8 |
| password | The password for players to join the server | "" |

### Client:

| Setting | Description | Default |
| ------- | ----------- | :-----: |
| serverPort | The port that the server is running on | 8989 |
| notificationDisplaySeconds | The time that a notification will remain on screen | 4.0 |
| displayNametags | Whether or not to show a nametag for other players | true |
| displayOwnNametag | Whether or not to show a nametag for this player | true |
| showPlayersOnMap | Display map icon for other players | true |
| showOtherTeamOnMap | Display a map icon for players on a different team | false |
| enablePvP | Allow players to attack and damage each other | true |
| enableFriendlyFire | Allow players on the same team to damage each other | false |
| team | The id of the team to play on (1-10) | 1 |
| syncSettings | Which types of progression will be sent & received to & from the server | All enabled |
