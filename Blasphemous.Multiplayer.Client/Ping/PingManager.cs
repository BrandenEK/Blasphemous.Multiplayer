using Blasphemous.ModdingAPI;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Ping;

public class PingManager
{
    public void ReceivePing(float time)
    {
        ModLog.Info(time + " --> " + Time.time);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Main.Multiplayer.NetworkManager.SendPing(Time.time, 0);
        }
    }
}
