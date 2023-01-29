using UnityEngine;
using System.Collections.Generic;
using Framework.Managers;
using Framework.FrameworkCore;
using BlasClient.Managers;

namespace BlasClient
{
    public class Multiplayer : PersistentInterface
    {
        // Managers
        private Client client;
        private PlayerManager playerManager;
        private ProgressManager progressManager;
        private NotificationManager notificationManager;

        // Game status
        public string playerName { get; private set; }
        public bool inLevel { get; private set; }

        // Player status
        private Vector2 lastPosition;
        private byte lastAnimation;
        private bool lastDirection;
        private float totalTimeBeforeSendAnimation = 500;
        private float currentTimeBeforeSendAnimation = 0;

        // Save data
        private List<string> interactedPersistenceObjects;

        private bool connectedToServer
        {
            get { return client != null && client.connectionStatus == Client.ConnectionStatus.Connected; }
        }

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
            LevelManager.OnBeforeLevelLoad += onLevelUnloaded;

            playerManager = new PlayerManager();
            progressManager = new ProgressManager();
            notificationManager = new NotificationManager();
            client = new Client();

            Core.Persistence.AddPersistentManager(this);
            interactedPersistenceObjects = new List<string>();
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
            playerManager.destroyPlayers();
            playerName = "";
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            inLevel = newLevel.LevelName != "MainMenu";
            notificationManager.createMessageBox();
            playerManager.loadScene(newLevel.LevelName);
            progressManager.sceneLoaded();

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
            playerManager.unloadScene();
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
                    bool animationExists = false;
                    for (byte i = 0; i < StaticObjects.animations.Length; i++)
                    {
                        if (state.IsName(StaticObjects.animations[i].name))
                        {
                            Main.UnityLog("Sending new player animation");

                            // Don't send new animations right after a special animation
                            if (currentTimeBeforeSendAnimation <= 0)
                            {
                                client.sendPlayerAnimation(i);
                            }
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

                // Once all three of these updates are added, send the queue
                client.SendQueue();
            }

            // Decrease frame counter for special animation delay
            if (currentTimeBeforeSendAnimation > 0)
                currentTimeBeforeSendAnimation -= Time.deltaTime;

            // Update game progress
            if (progressManager != null)
                progressManager.updateProgress();
            // Update other player's data
            if (playerManager != null && inLevel)
                playerManager.updatePlayers();
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
            return !currentState.IsName(StaticObjects.animations[lastAnimation].name);
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

        // Interacting with an object using a special animation
        public void usingSpecialAnimation(byte animation)
        {
            if (connectedToServer)
            {
                Main.UnityLog("Sending special animation");
                currentTimeBeforeSendAnimation = totalTimeBeforeSendAnimation;
                client.sendPlayerAnimation(animation);
            }
        }

        // A player finished their special animation
        public void finishedSpecialAnimation(string playerName)
        {
            if (inLevel)
            {
                Main.UnityLog("Finished special animation");
                playerManager.finishSpecialAnimation(playerName);
            }
        }

        // Received position data from server
        public void playerPositionUpdated(string playerName, float xPos, float yPos)
        {
            if (inLevel)
                playerManager.queuePosition(playerName, new Vector2(xPos, yPos));
        }

        // Received animation data from server
        public void playerAnimationUpdated(string playerName, byte animation)
        {
            if (inLevel)
                playerManager.queueAnimation(playerName, animation);
        }

        // Received direction data from server
        public void playerDirectionUpdated(string playerName, bool direction)
        {
            if (inLevel)
                playerManager.queueDirection(playerName, direction);
        }

        // Received skin data from server
        public void playerSkinUpdated(string playerName, string skin)
        {
            // As soon as received, will update skin - This isn't locked
            playerManager.updatePlayerSkin(playerName, skin);
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName)
        {
            if (inLevel)
                playerManager.addPlayer(playerName);
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            if (inLevel)
                playerManager.removePlayer(playerName);
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
            notificationManager.showNotification(message);
        }

        public void gameProgressReceived(string player, string progressId, byte progressType, byte progressValue)
        {
            // Apply the progress update
            progressManager.receiveProgress(progressId, progressType, progressValue);

            // Show notification for new progress
            notificationManager.showProgressNotification(player, progressType, progressId);
        }

        // Add a new persistent object that has been interacted with
        public void addPersistentObject(string persistentId)
        {
            if (!interactedPersistenceObjects.Contains(persistentId))
                interactedPersistenceObjects.Add(persistentId);
        }

        // Checks whether or not a persistent object has been interacted with
        public bool checkPersistentObject(string persistentId)
        {
            return interactedPersistenceObjects.Contains(persistentId);
        }

        // Save list of interacted persistent objects
        public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
        {
            MultiplayerPersistenceData multiplayerData = new MultiplayerPersistenceData();
            multiplayerData.interactedPersistenceObjects = interactedPersistenceObjects;
            return multiplayerData;
        }

        // Load list of interacted persistent objects
        public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
        {
            MultiplayerPersistenceData multiplayerData = (MultiplayerPersistenceData)data;
            interactedPersistenceObjects = multiplayerData.interactedPersistenceObjects;
        }

        // Reset list of interacted persitent objects
        public void ResetPersistence()
        {
            interactedPersistenceObjects.Clear();
        }

        public string GetPersistenID() { return "ID_MULTIPLAYER"; }
        public int GetOrder() { return 0; }
    }
}
