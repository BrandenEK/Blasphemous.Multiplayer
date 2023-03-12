# Blasphemous Multiplayer

## Table of Contents

- [Features](https://github.com/BrandenEK/Blasphemous-Multiplayer#features)
- [Sync details](https://github.com/BrandenEK/Blasphemous-Multiplayer#sync-details)
- [Installation](https://github.com/BrandenEK/Blasphemous-Multiplayer#installation)
- [Connecting](https://github.com/BrandenEK/Blasphemous-Multiplayer#connecting)
- [Available commands](https://github.com/BrandenEK/Blasphemous-Multiplayer#available-commands)
- [Configuration settings](https://github.com/BrandenEK/Blasphemous-Multiplayer#configuration-settings)
- [Translations](https://github.com/BrandenEK/Blasphemous-Multiplayer#translations)

---

## Features

- See other players as you both move around the world of Cvstodia
- Sync items, stats, skills, and other game progress across all players
- Compatible with randomizer (Make sure to use the same seed and settings)
- [Coming soon] Enable PvP to fight other players

---

## Sync details

### Things that sync

- All inventory items (Beads / Relics / Prayers / Hearts / Collectibles / Quest items)
- All player stats (Max health / Max fervour / Strength / Mea Culpa level / Max flasks / Flask level / Bead slots)
- All unlocked sword skills
- All revealed map cells
- All world state progress (Bosses defeated / Gates opened / Cherubs freed / Teleports unlocked / Questline progress / etc. )
- Church donation amount

### Things that will probably never sync

- Enemies & bosses (This is a huge task and not really feasible)

---

## Installation

#### Server:
1. Download and install Microsoft .NET Core 3.1 from https://dotnet.microsoft.com/en-us/download/dotnet/3.1
2. Download the latest release of the Multiplayer server from the [Releases](https://github.com/BrandenEK/Blasphemous-Multiplayer/releases) page
3. Unzip the BlasServer.zip file anywhere on the host machine

#### Client:
1. Download the latest release of the Modding API from https://github.com/BrandenEK/Blasphemous-Modding-API/releases
2. Follow the instructions there on how to install the api
3. Download the latest release of the Multiplayer client from the [Releases](https://github.com/BrandenEK/Blasphemous-Multiplayer/releases) page
4. Extract the contents of the BlasClient.zip file into the "Modding" folder

---

## Connecting

First, the server should be started by running the BlasServer.exe program on the host machine.  Then all of the clients should be able to connect by running the ```multiplayer connect SERVER NAME [PASSWORD]``` command
<br>
However, the client must have a network connection to the machine running the server.  If not on the same local network, find out how to use port forwarding or Hamachi (https://vpn.net/)

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

---

## Translations

This mod is available in these other languages in addition to English:
- Spanish (Thanks to Rodol J. "ConanCimmerio" Pérez (Ro))
- Chinese (Thanks to NewbieElton)
