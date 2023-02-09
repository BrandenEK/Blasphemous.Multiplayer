using UnityEngine;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;

namespace BlasClient.Managers
{
    public class MapScreenManager
    {
        private Vector2 activePlayerPosition;
        private bool mapUpdateQueued;

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

        public void queueMapUpdate()
        {
            mapUpdateQueued = true;
        }

        public void updateMap()
        {
            // If on map screen & just received an update, refresh map
            if (mapUpdateQueued && Core.Logic.IsPaused)
            {
                NewMapMenuWidget widget = Object.FindObjectOfType<NewMapMenuWidget>();
                if (widget != null && widget.CherubsText.isActiveAndEnabled)
                {
                    Main.UnityLog("Updating map!");
                    widget.Initialize();
                    widget.OnShow(PauseWidget.MapModes.SHOW);
                }
            }
            mapUpdateQueued = false;
        }
    }
}
