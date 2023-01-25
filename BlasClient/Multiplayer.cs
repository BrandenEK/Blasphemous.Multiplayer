using UnityEngine;
using Framework.Managers;
using Framework.FrameworkCore;

namespace BlasClient
{
    public class Multiplayer
    {
        private Client client;
        private PlayerControl playerControl;
        private NotificationManager notificationManager;

        public string playerName { get; private set; }

        private bool inLevel;
        private Vector2 lastPosition;
        private byte lastAnimation;
        private bool lastDirection;

        private bool connectedToServer
        {
            get { return client != null && client.connectionStatus == Client.ConnectionStatus.Connected; }
        }

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
            LevelManager.OnBeforeLevelLoad += onLevelUnloaded;
            playerControl = new PlayerControl();
            notificationManager = new NotificationManager();
            client = new Client();
            playerName = "";
        }
        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
            LevelManager.OnBeforeLevelLoad -= onLevelUnloaded;
        }

        public string tryConnect(string ip, string name, string password)
        {
            playerName = name;
            bool result = client.Connect(name, ip);
            if (result)
                displayNotification("Connected to server!");

            return result ? $"Successfully connected to {ip}" : $"Failed to connect to {ip}";
        }

        public void onDisconnect()
        {
            displayNotification("Disconnected from server!");
            playerControl.destroyPlayers();
            playerName = "";
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            inLevel = newLevel.LevelName != "MainMenu";
            notificationManager.createMessageBox();
            playerControl.loadScene(newLevel.LevelName);

            if (inLevel && connectedToServer)
            {
                // Entered a new scene
                Main.UnityLog("Entering new scene: " + newLevel.LevelName);
                client.sendPlayerEnterScene(newLevel.LevelName);
            }
        }

        private void onLevelUnloaded(Level oldLevel, Level newLevel)
        {
            if (inLevel && connectedToServer)
            {
                // Left a scene
                Main.UnityLog("Leaving old scene");
                client.sendPlayerLeaveScene();
            }

            inLevel = false;
            playerControl.unloadScene();
        }

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //playerControl.addPlayer("Player 2");
                //playerControl.queuePosition("Player 2", Core.Logic.Penitent.transform.position);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                //playerControl.queuePosition("Player 2", Core.Logic.Penitent.transform.position + Vector3.right * 3);
            }

            if (inLevel && connectedToServer && Core.Logic.Penitent != null)
            {
                // Check & send updated position
                Transform penitentTransform = Core.Logic.Penitent.transform;
                if (positionHasChanged(penitentTransform.position))
                {
                    //Main.UnityLog("Sending new player position");
                    client.sendPlayerPostition(penitentTransform.position.x, penitentTransform.position.y);
                    lastPosition = penitentTransform.position;
                }

                // Check & send updated animation clip
                Animator penitentAnimator = Core.Logic.Penitent.Animator;
                AnimatorStateInfo state = penitentAnimator.GetCurrentAnimatorStateInfo(0);
                if (animationHasChanged(state))
                {
                    // Animation has been updated
                    bool animationExists = false;
                    for (byte i = 0; i < PlayerAnimations.animations.Length; i++)
                    {
                        if (state.IsName(PlayerAnimations.animations[i].name))
                        {
                            //Main.UnityLog("Sending new player animation");
                            client.sendPlayerAnimation(i);
                            lastAnimation = i;
                            animationExists = true;
                            break;
                        }
                    }
                    if (!animationExists)
                    {
                        // This animation could not be found
                        Main.UnityLog("Error: Animation doesn't exist!");
                    }
                }

                // Check & send updated facing direction
                SpriteRenderer penitentRenderer = Core.Logic.Penitent.SpriteRenderer;
                if (directionHasChanged(penitentRenderer.flipX))
                {
                    //Main.UnityLog("Sending new player direction");
                    client.sendPlayerDirection(penitentRenderer.flipX);
                    lastDirection = penitentRenderer.flipX;
                }
            }

            // Update other player's data
            if (playerControl != null && inLevel)
                playerControl.updatePlayers();
            // Update notifications
            if (notificationManager != null)
                notificationManager.updateNotifications();
        }

        private bool positionHasChanged(Vector2 currentPosition)
        {
            float cutoff = 0.03f;
            return Mathf.Abs(currentPosition.x - lastPosition.x) > cutoff || Mathf.Abs(currentPosition.y - lastPosition.y) > cutoff;
        }

        private bool animationHasChanged(AnimatorStateInfo currentState)
        {
            return !currentState.IsName(PlayerAnimations.animations[lastAnimation].name);
        }

        private bool directionHasChanged(bool currentDirection)
        {
            return currentDirection != lastDirection;
        }

        // Changed skin from menu selector
        public void changeSkin(string skin)
        {
            if (connectedToServer)
            {
                Main.UnityLog("Sending new player skin");
                client.sendPlayerSkin(skin);
            }
        }

        // Obtained new item, upgraded stat, set flag, etc...
        public void obtainedGameProgress(string progressId, byte progressType, byte progressValue)
        {
            if (connectedToServer)
            {
                Main.UnityLog("Sending new game progress");
                client.sendPlayerProgress(progressType, progressValue, progressId);
            }
        }

        // Received position data from server
        public void playerPositionUpdated(string playerName, float xPos, float yPos)
        {
            if (inLevel)
                playerControl.queuePosition(playerName, new Vector2(xPos, yPos));
        }

        // Received animation data from server
        public void playerAnimationUpdated(string playerName, byte animation)
        {
            if (inLevel)
                playerControl.queueAnimation(playerName, animation);
        }

        // Received direction data from server
        public void playerDirectionUpdated(string playerName, bool direction)
        {
            if (inLevel)
                playerControl.queueDirection(playerName, direction);
        }

        // Received skin data from server
        public void playerSkinUpdated(string playerName, string skin)
        {
            // As soon as received, will update skin - This isn't locked
            playerControl.updatePlayerSkin(playerName, skin);
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName)
        {
            if (inLevel)
                playerControl.addPlayer(playerName);
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            if (inLevel)
                playerControl.removePlayer(playerName);
        }

        // Received introResponse data from server
        public void playerIntroReceived(byte response)
        {
            // Connected succesfully
            if (response == 0)
            {
                // Send skin
                client.sendPlayerSkin(Core.ColorPaletteManager.GetCurrentColorPaletteId());

                // If already in game, send enter scene data
                if (inLevel)
                    client.sendPlayerEnterScene(Core.LevelManager.currentLevel.LevelName);

                return;
            }

            // Failed to connect
            onDisconnect();
            string reason;
            if (response == 1) reason = "Player name is already taken"; // Duplicate name
            else if (response == 2) reason = "Server is full"; // Max player limit
            else reason = "Unknown reason"; // Unknown reason
            // Banned from server
            displayNotification($"({reason})");
        }

        public void displayNotification(string message)
        {
            Main.UnityLog("Notification: " + message);
            notificationManager.showNotification(message);
        }

        public void gameProgressReceived(string player, string progressId, byte progressType, byte progressValue)
        {
            // Calculate & display notification with player name & item's notification value
            // Determine what type of item it is
            // Queue the received item in the progress manager
        }
    }
}
