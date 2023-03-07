using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI;
using BlasClient.Managers;
using BlasClient.Structures;
using BlasClient.Data;
using ModdingAPI;

namespace BlasClient
{
    public class Multiplayer : PersistentMod
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
        public PlayerList playerList { get; private set; }
        public string playerName { get; private set; }
        public bool inLevel { get; private set; }
        public byte playerTeam { get; private set; }
        public string serverIp => client.serverIp;
        public bool inRando => IsModLoaded("com.damocles.blasphemous.randomizer");
        private List<string> interactedPersistenceObjects;
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

        public override string PersistentID => "ID_MULTIPLAYER";

        public Multiplayer(string modId, string modName, string modVersion) : base(modId, modName, modVersion) { }

        protected override void Initialize()
        {
            RegisterCommand(new MultiplayerCommand());

            // Create managers
            playerManager = new PlayerManager();
            progressManager = new ProgressManager();
            notificationManager = new NotificationManager();
            mapScreenManager = new MapScreenManager();
            client = new Client();

            // Initialize data
            config = FileUtil.loadConfig<Config>();
            PersistentStates.loadPersistentObjects();
            playerList = new PlayerList();
            interactedPersistenceObjects = new List<string>();
            playerName = "";
            playerTeam = (byte)(config.team > 0 && config.team <= 10 ? config.team : 10);
            sentAllProgress = false;
        }

        public void connectCommand(string ip, string name, string password)
        {
            if (client.Connect(ip, name, password))
            {
                playerName = name;
            }
            else
            {
                UIController.instance.StartCoroutine(delayedNotificationCoroutine(Localize("conerr") + " " + ip));
            }

            IEnumerator delayedNotificationCoroutine(string notification)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                notificationManager.showNotification(notification);
            }
        }

        public void disconnectCommand()
        {
            client.Disconnect();
            onDisconnect(true);
        }

        public void onDisconnect(bool showNotification)
        {
            if (showNotification)
                notificationManager.showNotification(Localize("dcon") + "!");
            playerList.ClearPlayers();
            playerManager.destroyPlayers();
            playerName = "";
            sentAllProgress = false;
        }

        protected override void LevelLoaded(string oldLevel, string newLevel)
        {
            inLevel = newLevel != "MainMenu";
            notificationManager.createMessageBox();
            playerManager.loadScene(newLevel);
            progressManager.sceneLoaded(newLevel);

            if (inLevel && connectedToServer)
            {
                // Entered a new scene
                Log("Entering new scene: " + newLevel);

                // Send initial position, animation, & direction before scene enter
                lastPosition = getCurrentPosition();
                client.sendPlayerPostition(lastPosition.x, lastPosition.y);
                lastAnimation = 0;
                client.sendPlayerAnimation(lastAnimation);
                lastDirection = getCurrentDirection();
                client.sendPlayerDirection(lastDirection);

                client.sendPlayerEnterScene(newLevel);
                sendAllProgress();
            }
        }

        protected override void LevelUnloaded(string oldLevel, string newLevel)
        {
            if (inLevel && connectedToServer)
            {
                // Left a scene
                Log("Leaving old scene");
                client.sendPlayerLeaveScene();
            }

            inLevel = false;
            playerManager.unloadScene();
        }

        protected override void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //PlayerStatus test = new PlayerStatus();
                //test.currentScene = "D05Z02S06";
                //connectedPlayers.Add("Test", test);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                
            }

            if (inLevel && connectedToServer && Core.Logic.Penitent != null)
            {
                // Check & send updated position
                if (positionHasChanged(out Vector2 newPosition))
                {
                    //Main.Multiplayer.Log("Sending new player position");
                    client.sendPlayerPostition(newPosition.x, newPosition.y);
                    lastPosition = newPosition;
                }

                // Check & send updated animation clip
                if (animationHasChanged(out byte newAnimation))
                {
                    // Don't send new animations right after a special animation
                    if (currentTimeBeforeSendAnimation <= 0 && newAnimation != 255)
                    {
                        //Main.Multiplayer.Log("Sending new player animation");
                        client.sendPlayerAnimation(newAnimation);
                    }
                    lastAnimation = newAnimation;
                }

                // Check & send updated facing direction
                if (directionHasChanged(out bool newDirection))
                {
                    //Main.Multiplayer.Log("Sending new player direction");
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
            Log("Error: Animation doesn't exist!");
            return 255;
        }

        private bool getCurrentDirection()
        {
            return Core.Logic.Penitent.SpriteRenderer.flipX;
        }

        // Changed skin from menu selector
        public void changeSkin(byte[] skin)
        {
            if (connectedToServer)
            {
                Log($"Sending new player skin ({skin.Length} bytes)");
                client.sendPlayerSkin(skin);
            }
        }

        // Changed team number from command
        public void changeTeam(byte teamNumber)
        {
            playerTeam = teamNumber;
            sentAllProgress = false;

            if (connectedToServer)
            {
                client.sendPlayerTeam(teamNumber);
                if (inLevel)
                {
                    updatePlayerColors();
                    sendAllProgress();
                }
            }
        }

        // Refresh players' nametags & map icons when someone changed teams
        private void updatePlayerColors()
        {
            playerManager.refreshNametagColors();
            mapScreenManager.queueMapUpdate();
        }

        // Obtained new item, upgraded stat, set flag, etc...
        public void obtainedGameProgress(string progressId, ProgressManager.ProgressType progressType, byte progressValue)
        {
            if (connectedToServer)
            {
                Log("Sending new game progress: " + progressId);
                client.sendPlayerProgress((byte)progressType, progressValue, progressId);
            }
        }

        // Interacting with an object using a special animation
        public void usingSpecialAnimation(byte animation)
        {
            if (connectedToServer)
            {
                Log("Sending special animation");
                currentTimeBeforeSendAnimation = totalTimeBeforeSendAnimation;
                client.sendPlayerAnimation(animation);
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
        public void playerSkinUpdated(string playerName, byte[] skin)
        {
            Log("Updating player skin for " + playerName);
            UIController.instance.StartCoroutine(delaySkinUpdate());

            IEnumerator delaySkinUpdate()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                playerList.setPlayerSkinTexture(playerName, skin);
            }
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName, string scene)
        {
            playerList.setPlayerScene(playerName, scene);

            if (inLevel && Core.LevelManager.currentLevel.LevelName == scene)
                playerManager.queuePlayer(playerName, true);
            mapScreenManager.queueMapUpdate();
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            if (inLevel && Core.LevelManager.currentLevel.LevelName == playerList.getPlayerScene(playerName))
                playerManager.queuePlayer(playerName, false);

            playerList.setPlayerScene(playerName, "");
            mapScreenManager.queueMapUpdate();
        }

        // Received introResponse data from server
        public void playerIntroReceived(byte response)
        {
            // Connected succesfully
            if (response == 0)
            {
                // Send all initial data
                client.sendPlayerSkin(Core.ColorPaletteManager.GetCurrentColorPaletteSprite().texture.GetRawTextureData());
                client.sendPlayerTeam(playerTeam);

                // If already in game, send enter scene data & game progress
                if (inLevel)
                {
                    client.sendPlayerEnterScene(Core.LevelManager.currentLevel.LevelName);
                    playerManager.createPlayerNameTag();
                    sendAllProgress();
                }

                notificationManager.showNotification(Localize("con") + "!");
                return;
            }

            // Failed to connect
            onDisconnect(false);
            string reason;
            if (response == 1) reason = "refpas"; // Wrong password
            else if (response == 2) reason = "refban"; // Banned player
            else if (response == 3) reason = "refmax"; // Max player limit
            else if (response == 4) reason = "refipa"; // Duplicate ip
            else if (response == 5) reason = "refnam"; // Duplicate name
            else reason = "refunk"; // Unknown reason

            notificationManager.showNotification(Localize("refuse") + ": " + Localize(reason));
        }

        // Received player connection status from server
        public void playerConnectionReceived(string playerName, bool connected)
        {
            if (connected)
            {
                // Add this player to the list of connected players
                playerList.AddPlayer(playerName);
            }
            else
            {
                // Remove this player from the list of connected players
                playerLeftScene(playerName);
                playerList.RemovePlayer(playerName);
            }
            notificationManager.showNotification($"{playerName} {Localize(connected ? "join" : "leave")}!");
        }

        public void playerProgressReceived(string playerName, string progressId, byte progressType, byte progressValue)
        {
            // Apply the progress update
            progressManager.receiveProgress(progressId, progressType, progressValue);

            // Show notification for new progress
            if (playerName != "*")
                notificationManager.showProgressNotification(playerName, progressType, progressId, progressValue);
        }

        public void playerTeamReceived(string playerName, byte team)
        {
            Log("Updating team number for " + playerName);
            playerList.setPlayerTeam(playerName, team);
            if (inLevel)
                updatePlayerColors();
        }

        private void sendAllProgress()
        {
            if (sentAllProgress) return;
            sentAllProgress = true;

            // This is the first time loading a scene after connecting - send all player progress
            Log("Sending all player progress");
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
        public override ModPersistentData SaveGame()
        {
            MultiplayerPersistenceData multiplayerData = new MultiplayerPersistenceData();
            multiplayerData.interactedPersistenceObjects = interactedPersistenceObjects;
            return multiplayerData;
        }

        // Load list of interacted persistent objects
        public override void LoadGame(ModPersistentData data)
        {
            MultiplayerPersistenceData multiplayerData = (MultiplayerPersistenceData)data;
            interactedPersistenceObjects = multiplayerData.interactedPersistenceObjects;
        }

        // Reset list of interacted persitent objects
        public override void ResetGame()
        {
            interactedPersistenceObjects.Clear();
        }

        public override void NewGame() { }
    }
}
