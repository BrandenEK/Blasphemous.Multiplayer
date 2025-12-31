using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Network;

/// <summary>
/// Controls all GUI windows on screen
/// </summary>
public class ConnectionDisplay : MonoBehaviour
{
    private bool _showingConnection = false;
    private bool _attemptingConnection = false;
    private bool _firstShowing = true;

    private ConnectionInfo _connection = new();

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

        if (Main.Multiplayer.NetworkManager.IsConnected)
        {
            GUI.Window(1, new Rect(10, Screen.height - 150 - 10, 330, 150), ConnectionStatusWindow, "Connection status");
        }
        else if (_attemptingConnection)
        {
            GUI.Window(4, new Rect(10, Screen.height - 400 - 10, 330, 400), ConnectionWaitWindow, "Enter connection info");
        }
        else
        {
            GUI.Window(0, new Rect(10, Screen.height - 400 - 10, 330, 400), ConnectionInfoWindow, "Enter connection info");
        }
    }

    private void ConnectionInfoWindow(int windowID)
    {
        if (_firstShowing)
            _connection = Main.Multiplayer.LastConnectionInfo;

        // Clean server ip
        string server = ReadTextField("Server IP:", _connection.ServerIp, 0);
        if (_firstShowing || server != _connection.ServerIp)
        {
            //ModLog.Info("Validating server ip");
            server = CleanTextField(server, 64, VALID_IP);
        }

        // Clean room name
        string room = ReadTextField("Room name:", _connection.RoomName, 1);
        if (_firstShowing || room != _connection.RoomName)
        {
            //ModLog.Info("Validating room name");
            room = CleanTextField(room, 16, VALID_ROOM);
        }

        // Clean player name
        string player = ReadTextField("Player name:", _connection.PlayerName, 2);
        if (_firstShowing || player != _connection.PlayerName)
        {
            //ModLog.Info("Validating player name");
            player = CleanTextField(player, 16, VALID_PLAYER);
        }

        // Clean password
        string password = ReadTextField("Password:", _connection.Password, 3);
        if (_firstShowing || password != _connection.Password)
        {
            //ModLog.Info("Validating password");
            password = CleanTextField(password, 32, VALID_PASSWORD);
        }

        // Clean team number
        int team = ReadChoiceBox("Team number:", _connection.TeamNumber - 1, Enumerable.Range(1, 8).Select(x => x.ToString()).ToArray(), 4) + 1;
        if (_firstShowing || team != _connection.TeamNumber)
        {
            //ModLog.Info("Validating team number");
            team = team >= 1 && team <= 8 ? team : 1;
        }

        _connection = new ConnectionInfo(server, room, player, password, (byte)team);
        _firstShowing = false;

        if (ReadButton("Connect", 5))
            ValidateAndConnect();
    }

    private string CleanTextField(string text, int maxLength, string validChars)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (sb.Length < maxLength && (char.IsLetterOrDigit(c) || validChars.Contains(c)))
                sb.Append(c);
        }
        return sb.ToString();
    }

    private void ConnectionStatusWindow(int windowID)
    {
        // Show actual connection details
        string text = $"You are connected to {_connection.ServerIp} ({_connection.RoomName}) as {_connection.PlayerName} on team {_connection.TeamNumber}";
        ShowLabel(text, 0);

        if (ReadButton("Disconnect", 2))
            TryDisconnect();
    }

    private void ConnectionWaitWindow(int windowID)
    {
        var bigStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 20,
        };

        GUI.Label(new Rect(0, 20, 330, 380), "Connnecting...", bigStyle);
    }

    private void ValidateAndConnect()
    {
        // Validate server ip


        // Validate room name


        // Validate player name


        // Validate password


        // Validate team number


        // If anything is invalid, display the notification for it and return early

        StartCoroutine(TryConnect());
    }

    private IEnumerator TryConnect()
    {
        ModLog.Info($"Attempting to connect to {_connection.ServerIp}");

        string[] ipParts = _connection.ServerIp.Split(':');  // Do this better

        _attemptingConnection = true;
        Main.Multiplayer.NetworkManager.OnConnect += OnConnect;

        yield return null;

        bool result = Main.Multiplayer.NetworkManager.Connect(ipParts[0], int.Parse(ipParts[1]), _connection.RoomName, _connection.PlayerName, _connection.Password, (byte)_connection.TeamNumber);
    }

    private void OnConnect(bool success, byte errorCode)
    {
        _attemptingConnection = false;

        Main.Multiplayer.LastConnectionInfo = _connection;
        Main.Multiplayer.NetworkManager.OnConnect -= OnConnect;
    }

    private void TryDisconnect()
    {
        ModLog.Info($"Disconnecting from {_connection.ServerIp}");
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

    private const string VALID_IP = "-:.";
    private const string VALID_ROOM = "_-";
    private const string VALID_PLAYER = "_-.' ";
    private const string VALID_PASSWORD = "_-.";
}
