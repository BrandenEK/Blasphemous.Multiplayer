using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Blasphemous.ModdingAPI.Persistence;
using Blasphemous.Multiplayer.Client.Data;
using Blasphemous.Multiplayer.Client.Map;
using Blasphemous.Multiplayer.Client.Network;
using Blasphemous.Multiplayer.Client.Notifications;
using Blasphemous.Multiplayer.Client.Ping;
using Blasphemous.Multiplayer.Client.Players;
using Blasphemous.Multiplayer.Client.ProgressSync;
using Blasphemous.Multiplayer.Client.PvP;
using Framework.Managers;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client;

/// <summary>
/// Handles connecting to server, syncing progress, and tracking players
/// </summary>
public class Multiplayer : BlasMod, ISlotPersistentMod<MultiplayerSlotData>, IGlobalPersistentMod<MultiplayerGlobalData>
{
    internal Multiplayer() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    // Managers
    public AttackManager AttackManager { get; private set; }
    public MainPlayerManager MainPlayerManager { get; private set; }
    public Map.MapManager MapManager { get; private set; }
    public NetworkManager NetworkManager { get; private set; }
    public NotificationManager NotificationManager { get; private set; }
    public OtherPlayerManager OtherPlayerManager { get; private set; }
    public ProgressManager ProgressManager { get; private set; }
    public DamageCalculator DamageCalculator { get; private set; }
    public PingManager PingManager { get; private set; }

    // Game status
    public bool RandomizerMode => ModHelper.IsModLoadedByName("Randomizer");
    public Config config { get; private set; }
    public bool CurrentlyInLevel { get; private set; }

    // Player status
    public string PlayerName { get; private set; }
    public byte PlayerTeam { get; private set; }

    // Set to false when receiving a stat upgrade from someone in the same room & not in randomizer
    // Set to true upon loading a new scene
    // Must be true to naturally obtain stat upgrades and send them
    public bool CanObtainStatUpgrades { get; set; }

    /// <summary>
    /// The most recent connectioninfo that should be used as the default
    /// </summary>
    public ConnectionInfo LastConnectionInfo { get; set; } = new();

    protected override void OnInitialize()
    {
        LocalizationHandler.RegisterDefaultLanguage("en");

        // Create managers
        AttackManager = new AttackManager();
        MainPlayerManager = new MainPlayerManager();
        MapManager = new Map.MapManager();
        NetworkManager = new NetworkManager();
        NotificationManager = new NotificationManager();
        OtherPlayerManager = new OtherPlayerManager();
        ProgressManager = new ProgressManager();
        DamageCalculator = new DamageCalculator();
        PingManager = new PingManager();

        NetworkManager.OnConnect += OnConnect;

        // Initialize data
        config = ConfigHandler.Load<Config>();
        PersistentStates.loadPersistentObjects();
        PlayerName = string.Empty;
        PlayerTeam = 1;
    }

    protected override void OnRegisterServices(ModServiceProvider provider)
    {
        provider.RegisterCommand(new MultiplayerCommand());
    }

    public void OnConnect(string ipAddress, string playerName, string password)
    {
        PlayerName = playerName;
    }

    public void OnDisconnect()
    {
        NotificationManager.DisplayNotification(LocalizationHandler.Localize("dcon"));
        ProgressManager.ResetProgressSentStatus();
        OtherPlayerManager.RemoveAllConnectedPlayers();
        OtherPlayerManager.RemoveAllActivePlayers();
        PlayerName = string.Empty;
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        CurrentlyInLevel = newLevel != "MainMenu";
        NotificationManager.LevelLoaded();
        OtherPlayerManager.LevelLoaded(newLevel);
        ProgressManager.LevelLoaded(newLevel);
        CanObtainStatUpgrades = true;

        if (CurrentlyInLevel && NetworkManager.IsConnected)
        {
            // Entered a new scene
            ModLog.Info("Entering new scene: " + newLevel);

            // Send location, scene, and progress
            MainPlayerManager.SendAllLocationData();
            NetworkManager.SendEnterScene(newLevel);
            ProgressManager.SendAllProgress();
        }

        if (newLevel == "D06Z01S01")
            FixElevatorLevers();
    }

    protected override void OnLevelUnloaded(string oldLevel, string newLevel)
    {
        if (CurrentlyInLevel && NetworkManager.IsConnected)
        {
            // Left a scene
            ModLog.Info("Leaving old scene");
            NetworkManager.SendLeaveScene();
        }

        CurrentlyInLevel = false;
    }

    protected override void OnLateUpdate()
    {
        NetworkManager.ProcessQueue();

        MapManager.Update();
        NotificationManager.Update();
        PingManager.OnUpdate();
        if (CurrentlyInLevel)
        {
            MainPlayerManager.Update();
            OtherPlayerManager.Update();
            ProgressManager.Update();
        }

        NetworkManager.SendQueue();
    }

    // TEMP: only called by Network manager to store name/team right now
    public void SetPlayerData(string player, byte team)
    {
        PlayerName = player;
        PlayerTeam = team;
    }

    // Refresh players' nametags & map icons when someone changed teams
    public void RefreshPlayerColors()
    {
        OtherPlayerManager.RefreshNametagColors();
        MapManager.QueueMapUpdate();
    }

    // Send more player info after successful connection or display notification
    private void OnConnect(bool success, byte errorCode)
    {
        if (!success)
        {
            string reason = errorCode switch
            {
                1 => "refpas", // Wrong password
                2 => "refban", // Banned player
                3 => "refmax", // Max player limit
                4 => "refipa", // Duplicate ip
                5 => "refnam", // Duplicate name
                6 => "refpro", // Invalid protocol
                255 => "refcon", // No connection
                _ => "refunk", // Unknown reason
            };

            NotificationManager.DisplayNotification($"{LocalizationHandler.Localize("refuse")}: {LocalizationHandler.Localize(reason)}");
            return;
        }

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

        NotificationManager.DisplayNotification(LocalizationHandler.Localize("con"));
    }

    // Whenever you receive a stat upgrade, it needs to check if you are in the same room as the player who sent it.
    // If so, you can no longer obtain stat upgrades in the same room
    public void ProcessRecievedStat(string playerName, ProgressUpdate progress)
    {
        PlayerStatus player = OtherPlayerManager.FindConnectedPlayer(playerName);
        if (player == null)
            return;

        if (!CurrentlyInLevel || RandomizerMode || progress.Type != ProgressType.PlayerStat || Core.LevelManager.currentLevel.LevelName != player.CurrentScene)
            return;

        if (progress.Id == "LIFE" || progress.Id == "FERVOUR" || progress.Id == "STRENGTH" || progress.Id == "MEACULPA")
        {
            CanObtainStatUpgrades = false;
            ModLog.Warn("Received stat upgrade from player in the same room!");
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
            ModLog.Warn("Could not find elevator levers!");
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

    /// <summary>
    /// Save list of interacted persistent objects
    /// </summary>
    public MultiplayerSlotData SaveSlot()
    {
        return new MultiplayerSlotData()
        {
            InteractedPersistenceObjects = ProgressManager.SaveInteractedObjects()
        };
    }

    /// <summary>
    /// Load list of interacted persistent objects
    /// </summary>
    public void LoadSlot(MultiplayerSlotData data)
    {
        ProgressManager.LoadInteractedObjects(data.InteractedPersistenceObjects);
    }

    /// <summary>
    /// Reset list of interacted persistent objects
    /// </summary>
    public void ResetSlot()
    {
        ProgressManager.ClearInteractedObjects();
    }

    /// <summary>
    /// Save last connection info
    /// </summary>
    public MultiplayerGlobalData SaveGlobal()
    {
        return new MultiplayerGlobalData()
        {
            LastConnection = LastConnectionInfo
        };
    }

    /// <summary>
    /// Load last connection info
    /// </summary>
    public void LoadGlobal(MultiplayerGlobalData data)
    {
        LastConnectionInfo = data.LastConnection;
    }
}
