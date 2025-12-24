using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client;

/// <summary>
/// Controls all GUI windows on screen
/// </summary>
public class MultiplayerGUI : MonoBehaviour
{
    private bool _showingConnection = false;

    private string _server = "localhost:33000"; // Handle these somewhere else (In a class with proper defaults)
    private string _room = "debug";
    private string _player = "Damocles";
    private string _password = string.Empty;
    private int _team = 1;

    private void Update()
    {
        if (!SceneHelper.GameSceneLoaded)
            return;

        if (!Input.GetKeyDown(KeyCode.F9)) // Change to keybinding
            return;

        ToggleShowingConnection();
    }

    private void ToggleShowingConnection()
    {
        ModLog.Info("Toggling connection window");
        _showingConnection = !_showingConnection;

        Core.Input.SetBlocker("MULTIPLAYER", _showingConnection);
        Cursor.visible = _showingConnection;
    }

    private void OnGUI()
    {
        if (!_showingConnection)
            return;

        if (!Main.Multiplayer.NetworkManager.IsConnected)
            GUI.Window(0, new Rect(10, Screen.height - 400 - 10, 330, 400), ConnectionInfoWindow, "Enter connection info");
        else
            GUI.Window(1, new Rect(10, Screen.height - 150 - 10, 330, 150), ConnectionStatusWindow, "Connection status");
    }

    private void ConnectionInfoWindow(int windowID)
    {
        _server = ReadTextField("Server IP:", _server, 0); // Add max lengths
        _room = ReadTextField("Room name:", _room, 1);
        _player = ReadTextField("Player name:", _player, 2);
        _password = ReadTextField("Password:", _password, 3);
        _team = ReadChoiceBox("Team number:", _team - 1, Enumerable.Range(1, 8).Select(x => x.ToString()).ToArray(), 4) + 1;

        // Validate all parameters

        if (ReadButton("Connect", 5))
            TryConnect();
    }

    private void ConnectionStatusWindow(int windowID)
    {
        // Show actual connection details
        string text = $"You are connected to {_server} ({_room}) as {_player} on team {_team}";
        ShowLabel(text, 0);

        if (ReadButton("Disconnect", 2))
            TryDisconnect();
    }

    private void TryConnect()
    {
        ModLog.Info($"Attempting to connect to {_server}");

        string[] ipParts = _server.Split(':');  // Do this better

        bool result = Main.Multiplayer.NetworkManager.Connect(ipParts[0], int.Parse(ipParts[1]), _player, _password);

        if (result)
            ModLog.Info($"Successfully connected to {_server}");
        else
            ModLog.Error($"Failed to connect to {_server}");

        // Handle intro failure
    }

    private void TryDisconnect()
    {
        ModLog.Info($"Disconnecting from {_server}");
        Main.Multiplayer.NetworkManager.Disconnect();
    }

    private void ShowLabel(string label, int line)
    {
        GUI.Label(new Rect(START_X, START_Y + (LINE_HEIGHT + GAP) * line, 310, LINE_HEIGHT * 2 + GAP), label);
    }

    private string ReadTextField(string label, string value, int line)
    {
        GUI.Label(new Rect(START_X, START_Y + (LINE_HEIGHT + GAP) * line, LABEL_WIDTH, LINE_HEIGHT), label);
        return GUI.TextField(new Rect(START_X + LABEL_WIDTH + GAP, START_Y + (LINE_HEIGHT + GAP) * line, TEXT_WIDTH, LINE_HEIGHT), value);
    }

    private int ReadChoiceBox(string label, int value, string[] choices, int line)
    {
        GUI.Label(new Rect(START_X, START_Y + (LINE_HEIGHT + GAP) * line, LABEL_WIDTH, LINE_HEIGHT), label);
        return GUI.Toolbar(new Rect(START_X + LABEL_WIDTH + GAP, START_Y + (LINE_HEIGHT + GAP) * line, TEXT_WIDTH, LINE_HEIGHT), value, choices);
    }

    private bool ReadButton(string label, int line)
    {
        return GUI.Button(new Rect(START_X, START_Y + (LINE_HEIGHT + GAP) * line, BUTTON_WIDTH, LINE_HEIGHT), label);
    }

    private const int START_X = 10;
    private const int START_Y = 20;
    private const int GAP = 10;
    private const int LINE_HEIGHT = 25;
    private const int LABEL_WIDTH = 100;
    private const int TEXT_WIDTH = 200;
    private const int BUTTON_WIDTH = 90;
}
