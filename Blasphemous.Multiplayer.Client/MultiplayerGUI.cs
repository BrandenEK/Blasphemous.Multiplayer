using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Framework.Managers;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client;

/// <summary>
/// Controls all GUI windows on screen
/// </summary>
public class MultiplayerGUI : MonoBehaviour
{
    private bool _showingConnection = false;
    private bool _connected = false;

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
            GUI.Window(1, new Rect(10, 870, 330, 100), ConnectionStatusWindow, "Connection status");
    }

    private void ConnectionInfoWindow(int windowID)
    {
    }

    private void ConnectionStatusWindow(int windowID)
    {
    }
}
