using Blasphemous.ModdingAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Ping;

public class PingManager
{
    private readonly List<float> _delays = new(10);

    private ushort AveragePing
    {
        get
        {
            if (_delays.Count == 0)
                return 0;

            float average = _delays.Sum() / _delays.Count;
            return (ushort)(average * 1000);
        }
    }

    public void ReceivePing(float time)
    {
        float delay = Time.time - time;

        _delays.Add(delay);
        if (_delays.Count > MAX_DELAYS)
            _delays.RemoveAt(0);

        ModLog.Warn($"Average ping is {AveragePing} ms");
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Main.Multiplayer.NetworkManager.SendPing(Time.time, 0);
        }
    }

    private const int MAX_DELAYS = 10;
}
