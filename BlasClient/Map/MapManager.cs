using BlasClient.Players;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using UnityEngine.UI;

namespace BlasClient.Map
{
    public class MapManager
    {
        // Temporarily holds the most recent player map position
        public Vector2 ActivePlayerPosition { get; set; }

        // A map update is queued whenever player changes team, enters/leaves a scene, etc.
        private bool mapUpdateQueued;

        // UI created after first time opening the map
        private Transform playerMarks;
        private Sprite[] playerSprites;

        public void QueueMapUpdate()
        {
            mapUpdateQueued = true;
        }

        public void Update()
        {
            // If on map screen & just received an update, refresh map
            if (mapUpdateQueued && Core.Logic.IsPaused)
            {
                RefreshMap(false);
            }
            mapUpdateQueued = false;
        }

        public void RefreshMap(bool forceRecalculate)
        {
            // Only add marks for other players if config enabled
            if (!Main.Multiplayer.config.showPlayersOnMap)
                return;
            Main.Multiplayer.Log("Updating map with new player marks!");

            // Destroy old holder to put players on top of other marks
            if (forceRecalculate && playerMarks != null)
            {
                Object.Destroy(playerMarks.gameObject);
                playerMarks = null;
            }

            // If holder doesn't exist yet, create it
            if (playerMarks == null || playerSprites == null)
            {
                CreateMapUI();
            }

            // Destroy all old player marks
            for (int i = playerMarks.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(playerMarks.GetChild(i).gameObject);
            }

            // Create a new mark for each player
            foreach (PlayerStatus player in Main.Multiplayer.OtherPlayerManager.AllConnectedPlayers)
            {
                // Only show other teams if config option
                if (player.Team != Main.Multiplayer.PlayerTeam && !Main.Multiplayer.config.showOtherTeamOnMap)
                    continue;

                // Calling this function with -1000 will calculate the center position of the scene
                Core.NewMapManager.GetCellKeyFromPosition(player.LastMapScene, new Vector2(-1000, 0));
                Vector2 cellPosition = ActivePlayerPosition;
                if (cellPosition.x < 0 || cellPosition.y < 0)
                    continue;

                // Calculate which icon to use
                Sprite icon = playerSprites[0];
                if (player.CurrentScene != player.LastMapScene)
                {
                    icon = playerSprites[1];
                }
                else if (player.Team != Main.Multiplayer.PlayerTeam)
                {
                    icon = playerSprites[2];
                }

                // Create new image for this player
                GameObject obj = new GameObject(player.Name, typeof(RectTransform));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.SetParent(playerMarks, false);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.localPosition = new Vector2(16 * cellPosition.x, 16 * cellPosition.y);
                rect.sizeDelta = new Vector2(playerSprites[0].rect.width, playerSprites[0].rect.height);
                rect.gameObject.AddComponent<Image>().sprite = icon;
                Main.Multiplayer.Log($"Creating mark at " + rect.localPosition);
            }
        }

        private void CreateMapUI()
        {
            // Find Map widget
            NewMapMenuWidget widget = Object.FindObjectOfType<NewMapMenuWidget>();
            if (widget == null) return;
            Transform rootRenderer = widget.transform.Find("Background/Map/MapMask/MapRoot/RootRenderer_0");
            if (rootRenderer == null) return;

            // Create child of renderer
            playerMarks = new GameObject("PlayerMarks", typeof(RectTransform)).transform;
            playerMarks.SetParent(rootRenderer, false);

            // Get player images
            MapRendererConfig cfg = widget.RendererConfigs[0];
            playerSprites = new Sprite[3] { cfg.Marks[MapData.MarkType.Blue], cfg.Marks[MapData.MarkType.Green], cfg.Marks[MapData.MarkType.Red] };
        }
    }
}
