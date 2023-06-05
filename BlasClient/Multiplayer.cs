using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI;
using Tools.Level.Interactables;
using BlasClient.Managers;
using BlasClient.Structures;
using BlasClient.Data;
using BlasClient.PvP;


using BlasClient.Map;
using BlasClient.Network;
using BlasClient.Notifications;
using BlasClient.ProgressSync;
using ModdingAPI;

namespace BlasClient
{
    public class Multiplayer : PersistentMod
    {
        // Application status

        // Managers
        public AttackManager AttackManager { get; private set; }
        public Map.MapManager MapManager { get; private set; }
        public NetworkManager NetworkManager { get; private set; }
        public NotificationManager NotificationManager { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public ProgressManager ProgressManager { get; private set; }

        // Game status
        public bool RandomizerMode => IsModLoaded("com.damocles.blasphemous.randomizer");
        public Config config { get; private set; }
        public bool CurrentlyInLevel => inLevel;
        public bool inLevel { get; private set; } // CHnage this !!

        public string PlayerName { get; private set; }
        public byte PlayerTeam { get; private set; }

        public PlayerList playerList { get; private set; }
        private List<string> interactedPersistenceObjects;
        private bool sentAllProgress;

        // Player status
        private Vector2 lastPosition;
        private byte lastAnimation;
        private bool lastDirection;
        private float totalTimeBeforeSendAnimation = 0.5f;
        private float currentTimeBeforeSendAnimation = 0;

        // Set to false when receiving a stat upgrade from someone in the same room & not in randomizer
        // Set to true upon loading a new scene
        // Must be true to naturally obtain stat upgrades and send them
        public bool CanObtainStatUpgrades { get; set; }

        public override string PersistentID => "ID_MULTIPLAYER";

        public Multiplayer(string modId, string modName, string modVersion) : base(modId, modName, modVersion) { }

        protected override void Initialize()
        {
            RegisterCommand(new MultiplayerCommand());

            // Create managers
            AttackManager = new AttackManager();
            MapManager = new Map.MapManager();
            NetworkManager = new NetworkManager();
            NotificationManager = new NotificationManager();
            PlayerManager = new PlayerManager();
            ProgressManager = new ProgressManager();

            // Initialize data
            config = FileUtil.loadConfig<Config>();
            PersistentStates.loadPersistentObjects();
            playerList = new PlayerList();
            interactedPersistenceObjects = new List<string>();
            PlayerName = string.Empty;
            PlayerTeam = (byte)(config.team > 0 && config.team <= 10 ? config.team : 10);
            sentAllProgress = false;
        }

        public void connectCommand(string ipAddress, string playerName, string password)
        {
            //if (client.Connect(ip, name, password))
            //{
            //    playerName = name;
            //}
            //else
            //{
            //    UIController.instance.StartCoroutine(delayedNotificationCoroutine(Localize("conerr") + " " + ip));
            //}

            //IEnumerator delayedNotificationCoroutine(string notification)
            //{
            //    yield return new WaitForEndOfFrame();
            //    yield return new WaitForEndOfFrame();
            //    NotificationManager.DisplayNotification(notification);
            //}
        }

        public void OnConnect(string ipAddress, string playerName, string password)
        {
            PlayerName = playerName;
        }

        public void OnDisconnect()
        {
            NotificationManager.DisplayNotification(Localize("dcon"));
            playerList.ClearPlayers();
            PlayerManager.destroyPlayers();
            PlayerName = string.Empty;
            sentAllProgress = false;
        }

        protected override void LevelLoaded(string oldLevel, string newLevel)
        {
            inLevel = newLevel != "MainMenu";
            NotificationManager.LevelLoaded();
            PlayerManager.loadScene(newLevel);
            ProgressManager.LevelLoaded(newLevel);
            CanObtainStatUpgrades = true;

            if (inLevel && NetworkManager.IsConnected)
            {
                // Entered a new scene
                Log("Entering new scene: " + newLevel);

                // Send initial position, animation, & direction before scene enter
                SendAllLocationData();

                NetworkManager.SendEnterScene(newLevel);
                sendAllProgress();
            }

            if (newLevel == "D06Z01S01")
                FixElevatorLevers();
        }

        protected override void LevelUnloaded(string oldLevel, string newLevel)
        {
            if (inLevel && NetworkManager.IsConnected)
            {
                // Left a scene
                Log("Leaving old scene");
                NetworkManager.SendLeaveScene();
            }

            inLevel = false;
            PlayerManager.unloadScene();
        }

        protected override void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //PlayerStatus test = new PlayerStatus();
                //test.currentScene = "D05Z02S06";
                //connectedPlayers.Add("Test", test);
                //attackManager.TakeHit("", 0);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                //GameObject prayer = Core.Logic.Penitent.PrayerCast.crawlerBallsPrayer.projectilePrefab;
                //if (prayer != null)
                //    Main.Multiplayer.LogError("Verdiales speed: " + Core.Logic.Penitent.PrayerCast.crawlerBallsPrayer.projectileSpeed);
                    //Main.Multiplayer.LogError(Main.displayHierarchy(prayer.transform, "", 0, 5, true));
                //List<GameObject> prayers = Core.Logic.Penitent.PrayerCast.lightBeamPrayer.areaPrefabs;
                //if (prayers != null)
                //{
                //    Main.Multiplayer.LogWarning("Using list");
                //    foreach (GameObject pray in prayers)
                //    {
                //        Main.Multiplayer.LogError(Main.displayHierarchy(pray.transform, "", 0, 5, true));
                //    }
                //}
            }

            if (inLevel && NetworkManager.IsConnected && Core.Logic.Penitent != null)
            {
                // Check & send updated position
                if (positionHasChanged(out Vector2 newPosition))
                {
                    NetworkManager.SendPosition(newPosition);
                    lastPosition = newPosition;
                }

                // Check & send updated animation clip
                if (animationHasChanged(out byte newAnimation))
                {
                    // Don't send new animations right after a special animation
                    if (currentTimeBeforeSendAnimation <= 0 && newAnimation != 255)
                    {
                        NetworkManager.SendAnimation(newAnimation);
                    }
                    lastAnimation = newAnimation;
                }

                // Check & send updated facing direction
                if (directionHasChanged(out bool newDirection))
                {
                    NetworkManager.SendDirection(newDirection);
                    lastDirection = newDirection;
                }
            }

            // Decrease frame counter for special animation delay
            if (currentTimeBeforeSendAnimation > 0)
                currentTimeBeforeSendAnimation -= Time.deltaTime;

            MapManager.Update();
            NotificationManager.Update();
            if (inLevel)
            {
                PlayerManager.updatePlayers();
                ProgressManager.Update();
            }

            NetworkManager.SendQueue();
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

        // Changed team number from command
        public void changeTeam(byte teamNumber)
        {
            PlayerTeam = teamNumber;
            sentAllProgress = false;

            if (NetworkManager.IsConnected)
            {
                NetworkManager.SendTeam(teamNumber);
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
            PlayerManager.refreshNametagColors();
            MapManager.QueueMapUpdate();
        }

        // Obtained new item, upgraded stat, set flag, etc...
        //public void obtainedGameProgress(string progressId, ProgressType progressType, byte progressValue)
        //{
        //    if (connectedToServer)
        //    {
        //        Log("Sending new game progress: " + progressId);
        //        client.sendPlayerProgress((byte)progressType, progressValue, progressId);
        //    }
        //}

        // Interacting with an object using a special animation
        //public void usingSpecialAnimation(byte animation)
        //{
        //    if (connectedToServer)
        //    {
        //        Log("Sending special animation");
        //        currentTimeBeforeSendAnimation = totalTimeBeforeSendAnimation;
        //        client.sendPlayerAnimation(animation);
        //    }
        //}

        public void UseSpecialAnimation(byte animation)
        {
            currentTimeBeforeSendAnimation = totalTimeBeforeSendAnimation;
            NetworkManager.SendAnimation(animation);
        }

        // Creates and sends a new attack to other players in the same scene
        //public void SendNewAttack(string hitPlayerName, AttackType attack)
        //{
        //    if (connectedToServer)
        //    {
        //        client.sendPlayerAttack(hitPlayerName, (byte)attack);
        //    }
        //}

        // Sends a new attacking effect to other players in the same scene
        //public void SendNewEffect(EffectType effect)
        //{
        //    if (connectedToServer)
        //    {
        //        client.sendPlayerEffect(playerName, (byte)effect);
        //    }
        //}

        // Sends the current position/animation/direction when first entering a scene or joining server
        // Make sure you are connected to server first
        private void SendAllLocationData()
        {
            NetworkManager.SendPosition(lastPosition = getCurrentPosition());
            NetworkManager.SendAnimation(lastAnimation = 0);
            NetworkManager.SendDirection(lastDirection = getCurrentDirection());
        }

        //// Received position data from server
        //public void playerPositionUpdated(string playerName, float xPos, float yPos)
        //{
        //    if (inLevel)
        //        PlayerManager.queuePosition(playerName, new Vector2(xPos, yPos));
        //}

        //// Received animation data from server
        //public void playerAnimationUpdated(string playerName, byte animation)
        //{
        //    if (inLevel)
        //        PlayerManager.queueAnimation(playerName, animation);
        //}

        //// Received direction data from server
        //public void playerDirectionUpdated(string playerName, bool direction)
        //{
        //    if (inLevel)
        //        PlayerManager.queueDirection(playerName, direction);
        //}

        public void UpdateSkinData(string playerName, byte[] skinData)
        {
            Log("Updating player skin for " + playerName);
            Main.Instance.StartCoroutine(delaySkinUpdate());

            IEnumerator delaySkinUpdate()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                playerList.setPlayerSkinTexture(playerName, skinData);
            }
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName, string scene)
        {
            playerList.setPlayerScene(playerName, scene);

            if (inLevel && Core.LevelManager.currentLevel.LevelName == scene)
                PlayerManager.queuePlayer(playerName, true);
            MapManager.QueueMapUpdate();
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            if (inLevel && Core.LevelManager.currentLevel.LevelName == playerList.getPlayerScene(playerName))
                PlayerManager.queuePlayer(playerName, false);

            playerList.setPlayerScene(playerName, "");
            MapManager.QueueMapUpdate();
        }

        // Received introResponse data from server
        public void playerIntroReceived(byte response)
        {
            // Connected succesfully
            if (response == 0)
            {
                // Send all initial data
                NetworkManager.SendSkin(Core.ColorPaletteManager.GetCurrentColorPaletteId());
                NetworkManager.SendTeam(PlayerTeam);

                // If already in game, send enter scene data & game progress
                if (inLevel)
                {
                    SendAllLocationData();
                    NetworkManager.SendEnterScene(Core.LevelManager.currentLevel.LevelName);
                    PlayerManager.createPlayerNameTag();
                    sendAllProgress();
                }

                NotificationManager.DisplayNotification(Localize("con"));
                return;
            }

            // Failed to connect
            string reason;
            if (response == 1) reason = "refpas"; // Wrong password
            else if (response == 2) reason = "refban"; // Banned player
            else if (response == 3) reason = "refmax"; // Max player limit
            else if (response == 4) reason = "refipa"; // Duplicate ip
            else if (response == 5) reason = "refnam"; // Duplicate name
            else reason = "refunk"; // Unknown reason

            NotificationManager.DisplayNotification(Localize("refuse") + ": " + Localize(reason));
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
            NotificationManager.DisplayNotification($"{playerName} {Localize(connected ? "join" : "leave")}");
        }

        // Whenever you receive a stat upgrade, it needs to check if you are in the same room as the player who sent it.
        // If so, you can no longer obtain stat upgrades in the same room
        public void ProcessRecievedStat(ProgressUpdate progress)
        {
            if (inLevel && progress.Type == ProgressType.PlayerStat && !RandomizerMode && Core.LevelManager.currentLevel.LevelName == playerList.getPlayerScene(PlayerName))
            {
                if (progress.Id == "LIFE" || progress.Id == "FERVOUR" || progress.Id == "STRENGTH" || progress.Id == "MEACULPA")
                {
                    CanObtainStatUpgrades = false;
                    LogWarning("Received stat upgrade from player in the same room!");
                }
            }
        }

        public void playerTeamReceived(string playerName, byte team)
        {
            Log("Updating team number for " + playerName);
            playerList.setPlayerTeam(playerName, team);
            if (inLevel)
                updatePlayerColors();
        }

        //public void playerAttackReceived(string attackerName, string receiverName, byte attack)
        //{
        //    attackManager.AttackReceived(attackerName, receiverName, (AttackType)attack);
        //}

        //public void playerEffectReceived(string playerName, byte effect)
        //{
        //    attackManager.EffectReceived(playerName, (EffectType)effect);
        //}

        private void sendAllProgress()
        {
            if (sentAllProgress) return;
            sentAllProgress = true;

            // This is the first time loading a scene after connecting - send all player progress
            Log("Sending all player progress");
            ProgressManager.SendAllProgress();
        }

        // If loading the rooftops elevator scene, set levers if they have been unlocked by someone else
        private void FixElevatorLevers()
        {
            Lever[] levers = Object.FindObjectsOfType<Lever>();
            Lever downLever = null, UpLever = null;

            // Find the levers in the scene
            foreach (Lever lever in levers)
            {
                if (lever.GetPersistenID() == "fc76ec10-a6e2-4465-ade0-642520e84efc")
                    downLever = lever;
                else if (lever.GetPersistenID() == "14b5e15b-2178-4677-8a54-468d5496037d")
                    UpLever = lever;
            }
            if (UpLever == null || downLever == null)
            {
                LogWarning("Could not find elevator levers!");
                return;
            }

            // Check where the elevator is & what positions are unlocked, and maybe set levers up
            if (Core.Events.GetFlag("ELEVATOR_POSITION_1"))
            {
                downLever.SetLeverDownInstantly();
                if (Core.Events.GetFlag("ELEVATOR_POSITION_2_UNLOCKED"))
                    UpLever.SetLeverUpInstantly();
            }
            else if (Core.Events.GetFlag("ELEVATOR_POSITION_2"))
            {
                downLever.SetLeverUpInstantly();
                if (Core.Events.GetFlag("ELEVATOR_POSITION_3_UNLOCKED"))
                    UpLever.SetLeverUpInstantly();
            }
            else if (Core.Events.GetFlag("ELEVATOR_POSITION_3"))
            {
                downLever.SetLeverUpInstantly();
                if (Core.Events.GetFlag("ELEVATOR_FULL_UNLOCKED"))
                    UpLever.SetLeverUpInstantly();
            }
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

        // Reset list of interacted persistent objects
        public override void ResetGame()
        {
            interactedPersistenceObjects.Clear();
        }

        public override void NewGame(bool NGPlus) { }
    }
}
