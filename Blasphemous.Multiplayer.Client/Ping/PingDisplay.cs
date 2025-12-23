using Blasphemous.ModdingAPI.Helpers;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Ping;

public class PingDisplay : MonoBehaviour
{
    private void OnGUI()
    {
        if (!SceneHelper.GameSceneLoaded || !Main.Multiplayer.NetworkManager.IsConnected)
            return;

        GUI.Window(2, new Rect(1920 - 110, 1080 - 110, 100, 100), PingWindow, "Test Window");
    }

    private void PingWindow(int windowID)
    {
        string text = $"Ping: {Main.Multiplayer.PingManager.AveragePing}ms";

        GUI.Label(new Rect(10, 20, 80, 30), text);
    }
}
