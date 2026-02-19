using Blasphemous.Multiplayer.Server.Models;
using System;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Server;

internal static class Core
{
    private static ServerHandler server;
    private static readonly Dictionary<byte, TeamInfo> teamGameDatas = [];

    static string ApplicationTitle
    {
        get
        {
            string text = "Blasphemous Multiplayer Server";
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3) ?? "Unknown";
            return $"{text} v{version}";
        }
    }

    static void Main(string[] args)
    {
        // Title
        Console.Title = ApplicationTitle;
        Console.WriteLine(string.Empty);

        // Read settings from args
        var cmd = new ServerCommand();
        cmd.Process(args);

        // Create server
        server = new ServerHandler(cmd.MaxPlayers, cmd.Password);
        if (!server.Start(cmd.Port))
        {
            Logger.Error($"Server failed to start on port {cmd.Port}");
            return;
        }

        // Initial messages
        Logger.Info($"Server has been started on port {cmd.Port}");
        Logger.Info("Press 'esc' to exit");

        // Start read loop
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Logger.Info("Shutting down server");
                Environment.Exit(0);
            }
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
            if (!teamGameDatas.ContainsKey(i))
                continue;

            // If no player is currently on this team, remove the game data
            bool teamExists = false;
            foreach (PlayerInfo player in allPlayers.Values)
            {
                if (player.Team == i)
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
