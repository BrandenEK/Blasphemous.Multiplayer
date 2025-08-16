using Blasphemous.Multiplayer.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Blasphemous.Multiplayer.Server;

internal static class Core
{
    public static ServerSettings TEMP_settings;
    private static Server server;
    private static Dictionary<byte, TeamInfo> teamGameDatas;

    static void Main(string[] args)
    {
        // Title
        Console.Title = "Blasphemous Multiplayer Server";
        Console.WriteLine(string.Empty);

        // Load settings from file
        ServerSettings settings = LoadSettings(Path.Combine(Environment.CurrentDirectory, "Multiplayer.cfg"));
        TEMP_settings = settings;
        Logger.Warn(settings.Port);

        // Create server
        server = new Server();
        if (!server.Start())
        {
            Logger.Error("Server failed to start at this machine's local ip address");
            return;
        }

        Logger.Info("Server has been started at this machine's local ip address");
        teamGameDatas = new Dictionary<byte, TeamInfo>();

        // Start read loop
        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    public static TeamInfo getTeamData(byte team)
    {
        if (teamGameDatas.ContainsKey(team))
            return teamGameDatas[team];

        Logger.Info("Creating new game data for team " + team);
        TeamInfo newData = new TeamInfo();
        teamGameDatas.Add(team, newData);
        return newData;
    }

    public static void removeUnusedGameData(Dictionary<string, PlayerInfo> allPlayers)
    {
        for (byte i = 1; i <= 10; i++)
        {
            if (!teamGameDatas.ContainsKey(i)) continue;

            // If no player is currently on this team, remove the game data
            bool teamExists = false;
            foreach (PlayerInfo player in allPlayers.Values)
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

    private static ServerSettings LoadSettings(string path)
    {
        ServerSettings settings;

        try
        {
            settings = JsonConvert.DeserializeObject<ServerSettings>(File.ReadAllText(path));
        }
        catch
        {
            Logger.Warn($"Failed to read config from {path}");
            settings = new ServerSettings(8989, 8, string.Empty);

            File.WriteAllText(path, JsonConvert.SerializeObject(settings, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            }));
        }

        return settings;
    }
}
