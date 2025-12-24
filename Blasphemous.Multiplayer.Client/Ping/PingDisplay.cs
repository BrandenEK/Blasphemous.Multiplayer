using Blasphemous.ModdingAPI.Helpers;
using System.Linq;
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
        ushort ping = Main.Multiplayer.PingManager.AveragePing;
        string color = _regions.OrderBy(x => x.MaxPing).First(x => ping <= x.MaxPing).Color;
        string text = $"Ping: <color=#{color}>{Main.Multiplayer.PingManager.AveragePing}ms</color>";

        var style = new GUIStyle(GUI.skin.label)
        {
            richText = true
        };

        GUI.Label(new Rect(10, 20, 80, 30), text, style);
    }

    private static readonly PingRegion[] _regions =
    {
        new PingRegion(50, "109748"),
        new PingRegion(100, "FFE733"),
        new PingRegion(500, "FF8C01"),
        new PingRegion(int.MaxValue, "ED2938"),
    };
}
