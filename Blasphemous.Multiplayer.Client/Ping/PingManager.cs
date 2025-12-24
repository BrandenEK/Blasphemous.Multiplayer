using Blasphemous.ModdingAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Ping;

public class PingManager
{
    private readonly List<float> _delays = new(10);

    private float _currentInterval = 0;

    public ushort AveragePing
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
    }

    public void OnUpdate()
    {
        if (!Main.Multiplayer.NetworkManager.IsConnected)
            return;

        _currentInterval += Time.deltaTime;

        if (_currentInterval >= SEND_INTERVAL)
        {
            _currentInterval = 0;
            Main.Multiplayer.NetworkManager.SendPing(Time.time, AveragePing);
        }
    }

    private const int MAX_DELAYS = 10;
    private const float SEND_INTERVAL = 1;
}
