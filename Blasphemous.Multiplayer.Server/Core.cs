using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;

namespace Blasphemous.Multiplayer.Server;

internal static class Core
{
    public static Config config;
    private static Server server;
    private static Dictionary<byte, GameData> teamGameDatas;

    static void Main(string[] args)
    {
        // Title
        Console.Title = "Blasphemous Multiplayer Server";
        Console.WriteLine(string.Empty);

        // Load config from file
        string configPath = Path.Combine(Environment.CurrentDirectory, "multiplayer.cfg");
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            config = JsonConvert.DeserializeObject<Config>(json);
        }
        else
        {
            config = new Config();
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            Logger.Info("Creating new config at " + configPath);
        }

        // Create server
        server = new Server();
        if (!server.Start())
            Logger.Error("Server failed to start at this machine's local ip address");


        Logger.Info("Server has been started at this machine's local ip address");
        teamGameDatas = new Dictionary<byte, GameData>();

        // Start read loop
        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    public static GameData getTeamData(byte team)
    {
        if (teamGameDatas.ContainsKey(team))
            return teamGameDatas[team];

        Logger.Info("Creating new game data for team " + team);
        GameData newData = new GameData();
        teamGameDatas.Add(team, newData);
        return newData;
    }

    public static void removeUnusedGameData(Dictionary<string, PlayerStatus> allPlayers)
    {
        for (byte i = 1; i <= 10; i++)
        {
            if (!teamGameDatas.ContainsKey(i)) continue;

            // If no player is currently on this team, remove the game data
            bool teamExists = false;
            foreach (PlayerStatus player in allPlayers.Values)
            {
                if (player.team == i)
                {
                    teamExists = true;
                    break;
                }
            }
            if (!teamExists)
            {
                Logger.Info("Removing game data for team " + i);
                teamGameDatas.Remove(i);
            }
        }
    }
}
