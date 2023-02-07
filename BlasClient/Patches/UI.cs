using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Framework.Map;
using Framework.Managers;

namespace BlasClient.Patches
{
    // Add multiplayer version to main menu
    [HarmonyPatch(typeof(VersionNumber), "Start")]
    public class VersionNumber_Patch
    {
        public static void Postfix(VersionNumber __instance)
        {
            Text version = __instance.GetComponent<Text>();
            if (version.text.Contains("v."))
                version.text = "";
            version.text += "Multiplayer v" + PluginInfo.PLUGIN_VERSION + "\n";
        }
    }

    // Show other players on map screen
    [HarmonyPatch(typeof(MapRenderer), "UpdateMarks")]
    public class MapRenderer_Patch
    {
        public static void Postfix(MapRenderer __instance, RectTransform ___markRoot, float ___CellSizeX, float ___CellSizeY)
        {
            Main.UnityLog("Updating marks!");

            // Make sure sprite is valid
            if (!__instance.Config.Marks.TryGetValue(MapData.MarkType.Blue, out Sprite sprite) || sprite == null)
                return;

            // For each player, add a mark to their scene
            foreach (string playerName in Main.Multiplayer.connectedPlayers.Keys)
            {
                // Calling this function with -1000 will calculate the center position of the scene
                Core.NewMapManager.GetCellKeyFromPosition(Main.Multiplayer.connectedPlayers[playerName].currentScene, new Vector2(-1000, 0));
                Vector2 cellPosition = Main.Multiplayer.mapScreenManager.getActivePlayerPosition();
                if (cellPosition.x < 0 || cellPosition.y < 0)
                    return;

                // Create new image for this player
                GameObject obj = new GameObject(playerName, typeof(RectTransform));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.SetParent(___markRoot);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.localPosition = new Vector2(___CellSizeX * cellPosition.x, ___CellSizeY * cellPosition.y);
                rect.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
                rect.gameObject.AddComponent<Image>().sprite = sprite;
                Main.UnityLog($"Creating mark at " + rect.localPosition);
            }
        }
    }

    // Get center position of room
    [HarmonyPatch(typeof(NewMapManager), "GetCellKeyFromPosition", typeof(string), typeof(Vector2))]
    public class MapManagerRoom_Patch
    {
        public static bool Prefix(string scene, Vector2 position, MapData ___CurrentMap)
        {
            if (position.x > -999f) return true;

            ZoneKey zone = new ZoneKey(scene.Substring(0, 3), scene.Substring(3, 3), scene.Substring(6, 3));
            if (___CurrentMap.CellsByZone.ContainsKey(zone))
            {
                // Loop through each cell and find average position
                Vector2 totalPosition = Vector2.zero;
                foreach (CellData cell in ___CurrentMap.CellsByZone[zone])
                {
                    totalPosition += new Vector2(cell.CellKey.X, cell.CellKey.Y);
                }

                // Calculate average position and send it to map manager
                int totalCells = ___CurrentMap.CellsByZone[zone].Count;
                Vector2 averagePosition = new Vector2(totalPosition.x / totalCells, totalPosition.y / totalCells);
                Main.Multiplayer.mapScreenManager.setActivePlayerPosition(averagePosition);
                Main.UnityLog("Average position for scene " + scene + " is " + averagePosition);
            }
            else
            {
                Main.Multiplayer.mapScreenManager.setActivePlayerPosition(new Vector2(-1, -1));
                Main.UnityLog("Player zone " + scene + " does not exist!");
            }
            return false;
        }
    }
}
