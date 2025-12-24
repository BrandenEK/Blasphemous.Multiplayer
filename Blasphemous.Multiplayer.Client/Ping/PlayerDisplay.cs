using Blasphemous.ModdingAPI.Helpers;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Ping;

public class PlayerDisplay : MonoBehaviour
{
    private Rect _playerWindow = new Rect(Screen.width - WINDOW_WIDTH - 10, 200, WINDOW_WIDTH, 10);

    private void OnGUI()
    {
        if (!SceneHelper.GameSceneLoaded || !Main.Multiplayer.NetworkManager.IsConnected)
            return;

        _playerWindow = GUI.Window(3, _playerWindow, PlayerWindow, "Connected Players");
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

        int y = 20;

        var infos = Main.Multiplayer.OtherPlayerManager.AllConnectedPlayers
            .Select(x => new PlayerDisplayInfo(x.Name, x.Team, x.Ping))
            .Concat([new PlayerDisplayInfo(Main.Multiplayer.PlayerName, Main.Multiplayer.PlayerTeam, Main.Multiplayer.PingManager.AveragePing)])
            .OrderBy(x => x.Name);

        foreach (var group in infos.GroupBy(x => x.Team))
        {
            GUI.Label(new Rect(0, y, WINDOW_WIDTH, LINE_HEIGHT), $"Team {group.Key}", headerStyle);
            y += LINE_HEIGHT + LINE_GAP;

            GUI.Box(new Rect(0, y, WINDOW_WIDTH, group.Count() * (LINE_HEIGHT + LINE_GAP) + LINE_GAP), string.Empty);
            y += LINE_GAP;

            foreach (var info in group)
            {
                string color = _regions.OrderBy(x => x.MaxPing).First(x => info.Ping <= x.MaxPing).Color;
                string nameText = info.Name;
                string pingText = $"<color=#{color}>{info.Ping}ms</color>";

                GUI.Label(new Rect(10, y, WINDOW_WIDTH - 20, LINE_HEIGHT), pingText, pingStyle);
                GUI.Label(new Rect(10, y, WINDOW_WIDTH - 20, LINE_HEIGHT), nameText);
                y += LINE_HEIGHT + LINE_GAP;
            }
        }

        _playerWindow.height = y;
    }

    private static readonly PingRegion[] _regions =
    {
        new PingRegion(50, "109748"),
        new PingRegion(100, "FFE733"),
        new PingRegion(500, "FF8C01"),
        new PingRegion(int.MaxValue, "ED2938"),
    };

    private const int WINDOW_WIDTH = 250;
    private const int LINE_HEIGHT = 20;
    private const int LINE_GAP = 5;
}
