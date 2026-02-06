using Blasphemous.Multiplayer.Server.Models;
using System;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Server;

internal static class Core
{
    private static Server server;
    private static readonly Dictionary<byte, TeamInfo> teamGameDatas = [];

    static void Main(string[] args)
    {
        // Title
        Console.Title = "Blasphemous Multiplayer Server";
        Console.WriteLine(string.Empty);

        // Read settings from args
        var cmd = new ServerCommand();
        cmd.Process(args);

        // Create server
        server = new Server(cmd.MaxPlayers, cmd.Password);
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
