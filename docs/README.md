# Blasphemous Multiplayer

<img src="https://img.shields.io/github/downloads/BrandenEK/Blasphemous.Multiplayer/total?color=39B7C6&style=for-the-badge">

---

## Contributors

***- Programming and design -*** <br>
[@BrandenEK](https://github.com/BrandenEK), [@EltonZhang777](https://github.com/EltonZhang777)

***- Translations -*** <br>
[@ConanCimmerio](https://github.com/ConanCimmerio), [@EltonZhang777](https://github.com/EltonZhang777), [@RocherBrockas](https://github.com/RocherBrockas)

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

#### Connecting to a server running on your computer
- Use 'localhost' as the server parameter
#### Connecting to a server on your local network
- On the server machine, run 'ipconfig' in command prompt to find the local ipv4 address of the computer
- Use that as the server parameter
#### Connecting to a server on a different network
- Set up [Port forwarding](https://nordvpn.com/blog/open-ports-on-router/) on the server machine or have everyone use a vpn ([Hamachi](https://vpn.net/), [ZeroTier](https://www.zerotier.com/), or [RAdmin](https://www.radmin-vpn.com/))
- On the server machine, search 'What is my ip' on the internet to find the global ipv4 address of the computer
- Use that as the server parameter

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
A full breakdown of all configuration settings can be found [here](SETTINGS.md)