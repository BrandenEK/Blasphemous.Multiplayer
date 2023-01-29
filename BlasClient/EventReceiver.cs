using UnityEngine;

namespace BlasClient
{
    public class EventReceiver : MonoBehaviour
    {
        public void LaunchEvent(string eventName)
        {
            if (eventName == "INTERACTION_END")
                Main.Multiplayer.finishedSpecialAnimation(gameObject.name.Substring(1));
        }
    }
}
