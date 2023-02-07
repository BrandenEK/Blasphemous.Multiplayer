using UnityEngine;

namespace BlasClient.Managers
{
    public class MapScreenManager
    {
        private Vector2 activePlayerPosition;

        // Temporarily holds the most recent player map position
        public void setActivePlayerPosition(Vector2 position)
        {
            activePlayerPosition = position;
        }

        // Gets and clears the most recent player map position
        public Vector2 getActivePlayerPosition()
        {
            return activePlayerPosition;
        }
    }
}
