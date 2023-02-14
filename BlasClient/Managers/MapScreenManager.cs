using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI.Others.MenuLogic;
using BlasClient.Structures;

namespace BlasClient.Managers
{
    public class MapScreenManager
    {
        private Vector2 activePlayerPosition;
        private bool mapUpdateQueued;

        private Transform playerMarks;
        private Sprite[] playerSprites;

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
            if (playerMarks == null || playerSprites == null)
            {
                NewMapMenuWidget widget = Object.FindObjectOfType<NewMapMenuWidget>();
                if (widget == null) return;
                Transform rootRenderer = widget.transform.Find("Background/Map/MapMask/MapRoot/RootRenderer_0");
                if (rootRenderer == null) return;

                GameObject holder = new GameObject("PlayerMarks", typeof(RectTransform));
                holder.transform.SetParent(rootRenderer, false);

                playerMarks = holder.transform;
                MapRendererConfig cfg = widget.RendererConfigs[0];
                playerSprites = new Sprite[3] { cfg.Marks[MapData.MarkType.Blue], cfg.Marks[MapData.MarkType.Green], cfg.Marks[MapData.MarkType.Red] };
            }

            // Destroy all old player marks
            for (int i = playerMarks.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(playerMarks.GetChild(i).gameObject);
            }

            // Create new marks for each player
            foreach (string playerName in Main.Multiplayer.connectedPlayers.Keys)
            {
                PlayerStatus playerStatus = Main.Multiplayer.connectedPlayers[playerName];

                // Only show other teams if config option
                if (playerStatus.team != Main.Multiplayer.playerTeam && !Main.Multiplayer.config.showOtherTeamOnMap)
                    continue;

                // Calling this function with -1000 will calculate the center position of the scene
                Core.NewMapManager.GetCellKeyFromPosition(playerStatus.lastMapScene, new Vector2(-1000, 0));
                Vector2 cellPosition = getActivePlayerPosition();
                if (cellPosition.x < 0 || cellPosition.y < 0)
                    return;

                // Calculate which icon to use
                Sprite icon = playerSprites[0];
                if (playerStatus.currentScene != playerStatus.lastMapScene)
                {
                    icon = playerSprites[1];
                }
                else if (playerStatus.team != Main.Multiplayer.playerTeam)
                {
                    icon = playerSprites[2];
                }

                // Create new image for this player
                GameObject obj = new GameObject(playerName, typeof(RectTransform));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.SetParent(playerMarks, false);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.localPosition = new Vector2(16 * cellPosition.x, 16 * cellPosition.y);
                rect.sizeDelta = new Vector2(playerSprites[0].rect.width, playerSprites[0].rect.height);
                rect.gameObject.AddComponent<Image>().sprite = icon;
                Main.UnityLog($"Creating mark at " + rect.localPosition);
            }
        }
    }
}
