using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using Tools.Level.Interactables;


using BlasClient.Data;
using BlasClient.Map;
using BlasClient.Network;
using BlasClient.Notifications;
using BlasClient.Players;
using BlasClient.ProgressSync;
using BlasClient.PvP;
using ModdingAPI;

namespace BlasClient
{
    public class Multiplayer : PersistentMod
    {
        // Application status

        // Managers
        public AttackManager AttackManager { get; private set; }
        public MainPlayerManager MainPlayerManager { get; private set; }
        public Map.MapManager MapManager { get; private set; }
        public NetworkManager NetworkManager { get; private set; }
        public NotificationManager NotificationManager { get; private set; }
        public OtherPlayerManager OtherPlayerManager { get; private set; }
        public ProgressManager ProgressManager { get; private set; }

        // Game status
        public bool RandomizerMode => IsModLoaded("com.damocles.blasphemous.randomizer");
        public Config config { get; private set; }
        public bool CurrentlyInLevel { get; private set; }

        // Player status
        public string PlayerName { get; private set; }
        public byte PlayerTeam { get; private set; }

        private List<string> interactedPersistenceObjects;

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
            MainPlayerManager = new MainPlayerManager();
            MapManager = new Map.MapManager();
            NetworkManager = new NetworkManager();
            NotificationManager = new NotificationManager();
            OtherPlayerManager = new OtherPlayerManager();
            ProgressManager = new ProgressManager();

            // Initialize data
            config = FileUtil.loadConfig<Config>();
            PersistentStates.loadPersistentObjects();
            interactedPersistenceObjects = new List<string>();
            PlayerName = string.Empty;
            PlayerTeam = (byte)(config.team > 0 && config.team <= 10 ? config.team : 10);
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
            ProgressManager.ResetProgressSentStatus();
            OtherPlayerManager.RemoveAllConnectedPlayers();
            OtherPlayerManager.RemoveAllActivePlayers();
            PlayerName = string.Empty;
        }

        protected override void LevelLoaded(string oldLevel, string newLevel)
        {
            CurrentlyInLevel = newLevel != "MainMenu";
            NotificationManager.LevelLoaded();
            OtherPlayerManager.LevelLoaded(newLevel);
            ProgressManager.LevelLoaded(newLevel);
            CanObtainStatUpgrades = true;

            if (CurrentlyInLevel && NetworkManager.IsConnected)
            {
                // Entered a new scene
                Log("Entering new scene: " + newLevel);

                // Send location, scene, and progress
                MainPlayerManager.SendAllLocationData();
                NetworkManager.SendEnterScene(newLevel);
                ProgressManager.SendAllProgress();
            }

            if (newLevel == "D06Z01S01")
                FixElevatorLevers();
        }

        protected override void LevelUnloaded(string oldLevel, string newLevel)
        {
            if (CurrentlyInLevel && NetworkManager.IsConnected)
            {
                // Left a scene
                Log("Leaving old scene");
                NetworkManager.SendLeaveScene();
            }

            CurrentlyInLevel = false;
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

            NetworkManager.ProcessQueue();

            MapManager.Update();
            NotificationManager.Update();
            if (CurrentlyInLevel)
            {
                OtherPlayerManager.Update();
                MainPlayerManager.Update();
                ProgressManager.Update();
            }

            NetworkManager.SendQueue();
        }

        // Changed team number from command
        public void changeTeam(byte teamNumber)
        {
            PlayerTeam = teamNumber;
            ProgressManager.ResetProgressSentStatus();

            if (NetworkManager.IsConnected)
            {
                NetworkManager.SendTeam(teamNumber);
                if (CurrentlyInLevel)
                {
                    RefreshPlayerColors();
                    ProgressManager.SendAllProgress();
                }
            }
        }

        // Refresh players' nametags & map icons when someone changed teams
        public void RefreshPlayerColors()
        {
            OtherPlayerManager.RefreshNametagColors();
            MapManager.QueueMapUpdate();
        }

        // Received introResponse data from server
        public void ProcessIntroResponse(byte response)
        {
            // Connected succesfully
            if (response == 0)
            {
                // Send all initial data
                NetworkManager.SendSkin(Core.ColorPaletteManager.GetCurrentColorPaletteId());
                NetworkManager.SendTeam(PlayerTeam);

                // If already in game, send enter scene data & game progress
                if (CurrentlyInLevel)
                {
                    MainPlayerManager.SendAllLocationData();
                    NetworkManager.SendEnterScene(Core.LevelManager.currentLevel.LevelName);
                    OtherPlayerManager.AddNametag(PlayerName, true);
                    ProgressManager.SendAllProgress();
                }

                NotificationManager.DisplayNotification(Localize("con"));
                return;
            }

            string reason = response switch
            {
                1 => "refpas", // Wrong password
                2 => "refban", // Banned player
                3 => "refmax", // Max player limit
                4 => "refipa", // Duplicate ip
                5 => "refnam", // Duplicate name
                _ => "refunk", // Unknown reason
            };
            NotificationManager.DisplayNotification(Localize("refuse") + ": " + Localize(reason));
        }

        // Whenever you receive a stat upgrade, it needs to check if you are in the same room as the player who sent it.
        // If so, you can no longer obtain stat upgrades in the same room
        public void ProcessRecievedStat(string playerName, ProgressUpdate progress)
        {
            PlayerStatus player = OtherPlayerManager.FindConnectedPlayer(playerName);
            if (player == null) return;

            if (!CurrentlyInLevel || RandomizerMode || progress.Type != ProgressType.PlayerStat || Core.LevelManager.currentLevel.LevelName != player.CurrentScene)
                return;

            if (progress.Id == "LIFE" || progress.Id == "FERVOUR" || progress.Id == "STRENGTH" || progress.Id == "MEACULPA")
            {
                CanObtainStatUpgrades = false;
                LogWarning("Received stat upgrade from player in the same room!");
            }
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

        private Transform m_canvas;
        public Transform CanvasObject
        {
            get
            {
                if (m_canvas == null)
                {
                    foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
                    {
                        if (c.name == "Game UI")
                        {
                            m_canvas = c.transform;
                            break;
                        }
                    }
                }
                return m_canvas;
            }
        }

        private GameObject m_textPrefab;
        public GameObject TextObject
        {
            get
            {
                if (m_textPrefab == null)
                {
                    foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
                    {
                        if (obj.name == "PurgePoints")
                        {
                            m_textPrefab = obj.transform.GetChild(1).gameObject;
                            break;
                        }
                    }
                }
                return m_textPrefab;
            }
        }

        private RuntimeAnimatorController m_penitentAnimator;
        public RuntimeAnimatorController PlayerAnimator
        {
            get
            {
                if (m_penitentAnimator == null)
                {
                    m_penitentAnimator = Core.Logic.Penitent?.Animator.runtimeAnimatorController;
                }
                return m_penitentAnimator;
            }
        }

        private RuntimeAnimatorController m_SwordAnimator;
        public RuntimeAnimatorController PlayerSwordAnimator
        {
            get
            {
                if (m_SwordAnimator == null)
                {
                    m_SwordAnimator = Core.Logic.Penitent?.GetComponentInChildren<Gameplay.GameControllers.Penitent.Attack.SwordAnimatorInyector>().GetComponent<Animator>().runtimeAnimatorController;
                }
                return m_SwordAnimator;
            }
        }

        private Material m_penitentMaterial;
        public Material PlayerMaterial
        {
            get
            {
                if (m_penitentMaterial == null)
                {
                    m_penitentMaterial = Core.Logic.Penitent?.SpriteRenderer.material;
                }
                return m_penitentMaterial;
            }
        }
    }
}
