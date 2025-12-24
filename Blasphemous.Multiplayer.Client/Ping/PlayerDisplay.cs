using Blasphemous.ModdingAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Ping;

public class PlayerDisplay : MonoBehaviour
{
    private void OnGUI()
    {
        if (!SceneHelper.GameSceneLoaded || !Main.Multiplayer.NetworkManager.IsConnected)
            return;

        GUI.Window(3, new Rect(1920 - WINDOW_WIDTH - 10, 200, WINDOW_WIDTH, WINDOW_HEIGHT), PlayerWindow, "Connected Players");
    }

    private void PlayerWindow(int windowID)
    {
        var headerStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
        };
        var pingStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleRight,
            richText = true,
        };

        int x = 10, y = 20;

        foreach (var group in _tempPlayers.GroupBy(x => x.Value))
        {
            byte team = group.Key;

            GUI.Label(new Rect(0, y, WINDOW_WIDTH, LINE_HEIGHT), $"Team {team}", headerStyle);
            y += LINE_HEIGHT + LINE_GAP;

            GUI.Box(new Rect(0, y, WINDOW_WIDTH, group.Count() * (LINE_HEIGHT + LINE_GAP)), string.Empty);

            foreach (string player in group.Select(x => x.Key))
            {
                string ping = $"<color=#109748>30ms</color>";

                GUI.Label(new Rect(x, y, LINE_WIDTH, LINE_HEIGHT), player);
                GUI.Label(new Rect(10 + LINE_WIDTH, y, 50, LINE_HEIGHT), ping, pingStyle);
                y += LINE_HEIGHT + LINE_GAP;
            }
        }
    }

    private readonly Dictionary<string, byte> _tempPlayers = new()
    {
        { "Damocles", 1 },
        { "Salamanthe", 2 },
        { "Obssesive Bad", 3 },
        { "NewbieElton", 1 },
        { "Xanathar", 2 },
        { "WWWWWWWWWWWWWWWW", 2 },
    };

    private const int WINDOW_WIDTH = 250;
    private const int WINDOW_HEIGHT = 400;
    private const int LINE_WIDTH = 180;
    private const int LINE_HEIGHT = 20;
    private const int LINE_GAP = 5;
}
