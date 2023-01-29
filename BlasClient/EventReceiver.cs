using UnityEngine;

namespace BlasClient
{
    public class EventReceiver : MonoBehaviour
    {
        public void LaunchEvent(string eventName)
        {
            Main.UnityLog("Event received - " + eventName);
            Main.Multiplayer.finishedSpecialAnimation(gameObject.name.Substring(1));
        }
    }
}
