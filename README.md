# Blasphemous Multiplayer

<div>
  <img src="https://img.shields.io/github/v/release/BrandenEK/Blasphemous.Multiplayer?style=for-the-badge">
  <img src="https://img.shields.io/github/last-commit/BrandenEK/Blasphemous.Multiplayer?color=important&style=for-the-badge">
  <img src="https://img.shields.io/github/downloads/BrandenEK/Blasphemous.Multiplayer/total?color=success&style=for-the-badge">
</div>

---

## Features

- See other players as you both move around the world of Cvstodia
- Sync items, stats, skills, and other game progress across all players
- Compatible with randomizer (Make sure to use the same seed and settings)
- Enable PvP to fight other players

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

## PvP

PvP is now available in Blasphemous Multiplayer, but only for sword attacks right now (Prayers other than Debla do nothing).  To fight other players, change to a different team than them by either editing your config file or using the ```multiplayer team NUMBER``` command

## Connecting

First, the server should be started by running the BlasServer.exe program on the host machine.  Then all of the clients should be able to connect by running the ```multiplayer connect [SERVER] [NAME] [PASSWORD]``` command
<br><br>
However, the client must have a network connection to the machine running the server.  If not on the same local network, find out how to use port forwarding, Hamachi (https://vpn.net/), ZeroTier (https://www.zerotier.com/), or RAdmin
<br><br>
If using Hamachi, you may have to follow this [setup guide](https://github.com/BrandenEK/Windwaker-coop#how-to-set-up-hamachi-to-simulate-a-local-network) for it to work.  Then when connecting, use the IP address that Hamachi assigned to the machine running the server.

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

## Configuration settings
- These settings can be modified in the 'multiplayer.cfg' file located in the "Modding/config" folder

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

## Translations

This mod is available in these other languages in addition to English:
- Spanish (Thanks to Rodol J. "ConanCimmerio" Pérez (Ro))
- Chinese (Thanks to NewbieElton)
- French  (Thanks to Rocher)
