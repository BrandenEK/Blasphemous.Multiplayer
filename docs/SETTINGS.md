# Multiplayer Configuration Settings

### Server:
- These settings can be passed as command line arguments

| Setting | Description | Default |
| ------- | ----------- | :-----: |
| -port | The port to start the server on | 27051 |
| -max-players | The maximum number of players to allow on the server| 10 |
| -password | The password for players to join the server | "" |

### Client:
- These settings can be modified in the 'Modding/config/Multiplayer.cfg' file

| Setting | Description | Default |
| ------- | ----------- | :-----: |
| notificationDisplaySeconds | The time that a notification will remain on screen | 4.0 |
| displayNametags | Whether or not to show a nametag for other players | true |
| displayOwnNametag | Whether or not to show a nametag for this player | true |
| showPlayersOnMap | Display map icon for other players | true |
| showOtherTeamOnMap | Display a map icon for players on a different team | false |
| enablePvP | Allow players to attack and damage each other | true |
| enableFriendlyFire | Allow players on the same team to damage each other | false |
| team | The id of the team to play on (1-10) | 1 |
| syncSettings | Which types of progression will be sent & received to & from the server | All enabled |