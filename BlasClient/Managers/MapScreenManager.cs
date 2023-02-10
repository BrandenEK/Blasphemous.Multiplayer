using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI.Others.MenuLogic;

namespace BlasClient.Managers
{
    public class MapScreenManager
    {
        private Vector2 activePlayerPosition;
        private bool mapUpdateQueued;

        private Transform playerMarks;
        private Sprite playerSprite;

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
                    createPlayerMarks(false);
                }
            }
            mapUpdateQueued = false;
        }

        public void createPlayerMarks(bool forceRecalculate)
        {
            // Only add marks for other players if config enabled
            if (!Main.Multiplayer.config.showPlayersOnMap)
                return;
            Main.UnityLog("Updating map with new player marks!");

            // Destroy old holder to put players on top of other marks
            if (forceRecalculate && playerMarks != null)
            {
                Object.Destroy(playerMarks.gameObject);
                playerMarks = null;
            }

            // If holder doesn't exist yet, create it
            if (playerMarks == null || playerSprite == null)
            {
                NewMapMenuWidget widget = Object.FindObjectOfType<NewMapMenuWidget>();
                if (widget == null) return;
                Transform rootRenderer = widget.transform.Find("Background/Map/MapMask/MapRoot/RootRenderer_0");
                if (rootRenderer == null) return;

                GameObject holder = new GameObject("PlayerMarks", typeof(RectTransform));
                holder.transform.SetParent(rootRenderer, false);

                playerMarks = holder.transform;
                playerSprite = widget.RendererConfigs[0].Marks[MapData.MarkType.Blue];
            }

            // Destroy all old player marks
            for (int i = playerMarks.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(playerMarks.GetChild(i).gameObject);
            }

            // Create new marks for each player
            foreach (string playerName in Main.Multiplayer.connectedPlayers.Keys)
            {
                // Calling this function with -1000 will calculate the center position of the scene
                Core.NewMapManager.GetCellKeyFromPosition(Main.Multiplayer.connectedPlayers[playerName].currentScene, new Vector2(-1000, 0));
                Vector2 cellPosition = getActivePlayerPosition();
                if (cellPosition.x < 0 || cellPosition.y < 0)
                    return;

                // Create new image for this player
                GameObject obj = new GameObject(playerName, typeof(RectTransform));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.SetParent(playerMarks, false);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.localPosition = new Vector2(16 * cellPosition.x, 16 * cellPosition.y);
                rect.sizeDelta = new Vector2(playerSprite.rect.width, playerSprite.rect.height);
                rect.gameObject.AddComponent<Image>().sprite = playerSprite;
                Main.UnityLog($"Creating mark at " + rect.localPosition);
            }
        }
    }
}
