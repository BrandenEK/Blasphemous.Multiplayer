using UnityEngine;
using System.IO;
using BepInEx;
using Newtonsoft.Json;
using System.Collections.Generic;
using Framework.Managers;
using Framework.FrameworkCore;
using BlasClient.Managers;
using BlasClient.Structures;
using BlasClient.Data;

namespace BlasClient
{
    public class Multiplayer : PersistentInterface
    {
        // Application status
        private Client client;
        public Config config { get; private set; }

        // Managers
        public PlayerManager playerManager { get; private set; }
        public ProgressManager progressManager { get; private set; }
        public NotificationManager notificationManager { get; private set; }
        public MapScreenManager mapScreenManager { get; private set; }

        // Game status
        public Dictionary<string, PlayerStatus> connectedPlayers { get; private set; }
        private List<string> interactedPersistenceObjects;
        public string playerName { get; private set; }
        public bool inLevel { get; private set; }
        private bool sentAllProgress;

        // Player status
        private Vector2 lastPosition;
        private byte lastAnimation;
        private bool lastDirection;
        private float totalTimeBeforeSendAnimation = 0.5f;
        private float currentTimeBeforeSendAnimation = 0;

        public bool connectedToServer
        {
            get { return client != null && client.connectionStatus == Client.ConnectionStatus.Connected; }
        }

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
            LevelManager.OnBeforeLevelLoad += onLevelUnloaded;

            // Load config from file
            string configPath = Paths.GameRootPath + "\\multiplayer.cfg";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(json);
                Main.UnityLog("Loaded config from " + configPath);
            }
            else
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                Main.UnityLog("Creating new config at " + configPath);
            }

            // Create managers
            playerManager = new PlayerManager();
            progressManager = new ProgressManager();
            notificationManager = new NotificationManager();
            mapScreenManager = new MapScreenManager();
            client = new Client();

            // Initialize data
            Core.Persistence.AddPersistentManager(this);
            PersistentStates.loadPersistentObjects();
            connectedPlayers = new Dictionary<string, PlayerStatus>();
            interactedPersistenceObjects = new List<string>();
            playerName = "";
            sentAllProgress = false;
        }
        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
            LevelManager.OnBeforeLevelLoad -= onLevelUnloaded;
        }

        public string connectCommand(string ip, string name, string password)
        {
            playerName = name;
            bool result = client.Connect(name, ip);
            if (result)
                displayNotification("Connected to server!");

            return result ? $"Successfully connected to {ip}" : $"Failed to connect to {ip}";
        }

        public void disconnectCommand()
        {
            client.Disconnect();
            onDisconnect();
        }

        public void onDisconnect()
        {
            displayNotification("Disconnected from server!");
            connectedPlayers.Clear();
            playerManager.destroyPlayers();
            playerName = "";
            sentAllProgress = false;
        }

        public PlayerStatus getPlayerStatus(string playerName)
        {
            if (connectedPlayers.ContainsKey(playerName))
                return connectedPlayers[playerName];

            Main.UnityLog("Error: Player is not in the server: " + playerName);
            return new PlayerStatus();
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            inLevel = newLevel.LevelName != "MainMenu";
            notificationManager.createMessageBox();
            playerManager.loadScene(newLevel.LevelName);
            progressManager.sceneLoaded(newLevel.LevelName);

            if (inLevel && connectedToServer)
            {
                // Entered a new scene
                Main.UnityLog("Entering new scene: " + newLevel.LevelName);
                client.sendPlayerEnterScene(newLevel.LevelName);
                sendAllProgress();
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
                PlayerStatus test = new PlayerStatus();
                test.currentScene = "D05Z02S06";
                connectedPlayers.Add("Test", test);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                
            }

            if (inLevel && connectedToServer && Core.Logic.Penitent != null)
            {
                // Check & send updated position
                if (positionHasChanged(out Vector2 newPosition))
                {
                    //Main.UnityLog("Sending new player position");
                    client.sendPlayerPostition(newPosition.x, newPosition.y);
                    lastPosition = newPosition;
                }

                // Check & send updated animation clip
                if (animationHasChanged(out byte newAnimation))
                {
                    // Don't send new animations right after a special animation
                    if (currentTimeBeforeSendAnimation <= 0 && newAnimation != 255)
                    {
                        //Main.UnityLog("Sending new player animation");
                        client.sendPlayerAnimation(newAnimation);
                    }
                    lastAnimation = newAnimation;
                }

                // Check & send updated facing direction
                if (directionHasChanged(out bool newDirection))
                {
                    //Main.UnityLog("Sending new player direction");
                    client.sendPlayerDirection(newDirection);
                    lastDirection = newDirection;
                }

                // Once all three of these updates are added, send the queue
                client.SendQueue();
            }

            // Decrease frame counter for special animation delay
            if (currentTimeBeforeSendAnimation > 0)
                currentTimeBeforeSendAnimation -= Time.deltaTime;

            // Update game progress
            if (progressManager != null && inLevel)
                progressManager.updateProgress();
            // Update other player's data
            if (playerManager != null && inLevel)
                playerManager.updatePlayers();
            // Update notifications
            if (notificationManager != null)
                notificationManager.updateNotifications();
            // Update map screen
            if (mapScreenManager != null)
                mapScreenManager.updateMap();
        }

        private bool positionHasChanged(out Vector2 newPosition)
        {
            float cutoff = 0.03f;
            newPosition = getCurrentPosition();
            return Mathf.Abs(newPosition.x - lastPosition.x) > cutoff || Mathf.Abs(newPosition.y - lastPosition.y) > cutoff;
        }

        private bool animationHasChanged(out byte newAnimation)
        {
            newAnimation = getCurrentAnimation();
            return newAnimation != lastAnimation;
        }

        private bool directionHasChanged(out bool newDirection)
        {
            newDirection = getCurrentDirection();
            return newDirection != lastDirection;
        }

        private Vector2 getCurrentPosition()
        {
            return Core.Logic.Penitent.transform.position;
        }

        private byte getCurrentAnimation()
        {
            AnimatorStateInfo state = Core.Logic.Penitent.Animator.GetCurrentAnimatorStateInfo(0);
            for (byte i = 0; i < AnimationStates.animations.Length; i++)
            {
                if (state.IsName(AnimationStates.animations[i].name))
                {
                    return i;
                }
            }

            // This animation could not be found
            Main.UnityLog("Error: Animation doesn't exist!");
            return 255;
        }

        private bool getCurrentDirection()
        {
            return Core.Logic.Penitent.SpriteRenderer.flipX;
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
        public void obtainedGameProgress(string progressId, ProgressManager.ProgressType progressType, byte progressValue)
        {
            if (connectedToServer)
            {
                Main.UnityLog("Sending new game progress: " + progressId);
                client.sendPlayerProgress((byte)progressType, progressValue, progressId);
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
            Main.UnityLog("Updating player skin for " + playerName);
            PlayerStatus player = getPlayerStatus(playerName);
            player.skin.skinName = skin;
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName, string scene)
        {
            PlayerStatus playerStatus = getPlayerStatus(playerName);
            playerStatus.currentScene = scene;
            if (scene.Length == 9)
                playerStatus.lastMapScene = scene;

            if (inLevel && Core.LevelManager.currentLevel.LevelName == scene)
                playerManager.queuePlayer(playerName, true);
            mapScreenManager.queueMapUpdate();
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            PlayerStatus playerStatus = getPlayerStatus(playerName);

            if (inLevel && Core.LevelManager.currentLevel.LevelName == playerStatus.currentScene)
                playerManager.queuePlayer(playerName, false);

            playerStatus.currentScene = "";
            mapScreenManager.queueMapUpdate();
        }

        // Received introResponse data from server
        public void playerIntroReceived(byte response)
        {
            // Connected succesfully
            if (response == 0)
            {
                // Send all initial data
                client.sendPlayerSkin(Core.ColorPaletteManager.GetCurrentColorPaletteId());
                // Send team (Maybe send this with intro data)

                // If already in game, send enter scene data
                if (inLevel)
                {
                    client.sendPlayerEnterScene(Core.LevelManager.currentLevel.LevelName);
                    playerManager.createPlayerNameTag();
                    sendAllProgress();
                }

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

        // Received player connection status from server
        public void playerConnectionReceived(string playerName, bool connected)
        {
            if (connected)
            {
                // Add this player to the list of connected players
                PlayerStatus newPlayer = new PlayerStatus();
                connectedPlayers.Add(playerName, newPlayer);
            }
            else
            {
                // Remove this player from the list of connected players
                playerLeftScene(playerName);
                if (connectedPlayers.ContainsKey(playerName))
                    connectedPlayers.Remove(playerName);
            }
            displayNotification($"{playerName} has {(connected ? "joined" : "left")} the server!");
        }

        public void playerProgressReceived(string player, string progressId, byte progressType, byte progressValue)
        {
            // Apply the progress update
            progressManager.receiveProgress(progressId, progressType, progressValue);

            // Show notification for new progress
            if (player != "*")
                notificationManager.showProgressNotification(player, progressType, progressId, progressValue);
        }

        public void displayNotification(string message)
        {
            notificationManager.showNotification(message);
        }

        public string getServerIp()
        {
            return client.serverIp;
        }

        private void sendAllProgress()
        {
            if (sentAllProgress) return;
            sentAllProgress = true;

            // This is the first time loading a scene after connecting - send all player progress
            Main.UnityLog("Sending all player progress");
            progressManager.loadAllProgress();
        }

        // Add a new persistent object that has been interacted with
        public void addPersistentObject(string objectSceneId)
        {
            if (!interactedPersistenceObjects.Contains(objectSceneId))
                interactedPersistenceObjects.Add(objectSceneId);
        }

        // Checks whether or not a persistent object has been interacted with
        public bool checkPersistentObject(string objectSceneId)
        {
            return interactedPersistenceObjects.Contains(objectSceneId);
        }

        // Allows progress manager to send all interacted objects on connect
        public List<string> getAllPersistentObjects()
        {
            return interactedPersistenceObjects;
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
