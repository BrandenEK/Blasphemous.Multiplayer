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
    private bool _connected = false;

    private string _server = "192.168.0.1:25565"; // Handle these somewhere else (In a class with proper defaults)
    private string _room = "mp test";
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

        if (!_connected)
            GUI.Window(0, new Rect(10, 670, 330, 400), ConnectionInfoWindow, "Enter connection info");
        else
            GUI.Window(1, new Rect(10, 920, 330, 150), ConnectionStatusWindow, "Connection status");
    }

    private void ConnectionInfoWindow(int windowID)
    {
        _server = ReadTextField("Server IP:", _server, 0); // Add max lengths
        _room = ReadTextField("Room name:", _room, 1);
        _player = ReadTextField("Player name:", _player, 2);
        _password = ReadTextField("Password:", _password, 3);
        _team = ReadChoiceBox("Team number:", _team - 1, Enumerable.Range(1, 8).Select(x => x.ToString()).ToArray(), 4) + 1;

        if (ReadButton("Connect", 5))
        {
            // Actually connect

            ModLog.Warn("Attempting to connect...");
            _connected = true;

            // Dont close immediately
            ToggleShowingConnection();
        }
    }

    private void ConnectionStatusWindow(int windowID)
    {
        // Show actual connection details
        string text = $"You are connected to {_server} ({_room}) as {_player} on team {_team}";
        ShowLabel(text, 0);

        if (ReadButton("Disconnect", 2))
        {
            // Actually disconnect

            ModLog.Warn("Disconnecting from server");
            _connected = false;
        }
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
