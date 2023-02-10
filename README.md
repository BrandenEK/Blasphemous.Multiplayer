# Blasphemous Multiplayer

A co-op mod for Blasphemous.  This is still a work in progress without an official release.

Progress board: [Trello](https://trello.com/b/FBdBWoVM/blasphemous-multiplayer)

## Features

- See other players as you both move around the world of Cvstodia
- Sync items, stats, skills, and other game progress across all players
- [Coming soon] Compatible with randomizer
- [Coming soon] Enable PvP to fight other players

<br/>

## Things that sync

- All inventory items (Beads / Relics / Prayers / Hearts / Collectibles / Quest items)
- All player stats (Max health / Max fervour / Strength / Mea Culpa level / Max flasks / Flask level / Bead slots)
- All unlocked sword skills
- All revealed map cells
- Most world state progress (Bosses defeated / Chests opened / Levers pulled / Gates opened / Cherubs freed / Teleports unlocked / Questline progress)

## Things that don't sync yet

- Church donation amount
- Certain questline flags

## Things that will probably never sync

- Enemies & bosses (This is a huge task and not really feasible)

<br/>

## Requirements

### Server:
- Microsoft .NET Core 3.1 (https://dotnet.microsoft.com/en-us/download/dotnet/3.1)

### Client:
- Most recent version of the Blasphemous game (https://store.steampowered.com/app/774361/Blasphemous)

<br/>

## How to use

### Server:
1. Unzip the BlasServer.zip file anywhere on the host machine
2. Run the BlasServer.exe program

### Client:
1. Unzip the BlasClient.zip file into the game's root folder
2. Run the game and verify on the title screen that the mod has been loaded
3. Press the 'backslash' key to open the debug console
4. Run the command "multiplayer connect SERVER NAME [PASSWORD]"
  - SERVER --> The ip address of the machine running the server application
  - NAME --> The player name you wish to use
  - PASSWORD --> The password to the server (Optional)
  - Run the command "multiplayer help" to list additional commands

<br/>

## Configuration settings
- These settings can be modified in the 'multiplayer.cfg' file generated in the same folder as the application

### Server:

- serverPort --> The port to start the server on
- maxPlayers --> The maximum number of players to allow on the server

### Client:

- serverPort --> The port that the server is running on
- notificationDisplaySeconds --> The time that a notification will remain on screen
- displayNametags --> Whether or not to show a nametag for other players
- displayOwnNametag --> Whether or not to show a nametag for this player
- showPlayersOnMap --> Display an icon on the map for other players
- team --> The id of the team to play on
- syncSettings --> Which types of progression will be sent & received to/from the server
