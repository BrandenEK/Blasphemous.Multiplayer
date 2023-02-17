using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Framework.Map;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;

namespace BlasClient.Patches
{
    // Show other players on map screen
    [HarmonyPatch(typeof(NewMapMenuWidget), "OnShow")]
    public class MapMenuWidget_Patch
    {
        public static void Postfix()
        {
            Main.Multiplayer.mapScreenManager.createPlayerMarks(true);
        }
    }

    // Get center position of room
    [HarmonyPatch(typeof(NewMapManager), "GetCellKeyFromPosition", typeof(string), typeof(Vector2))]
    public class MapManagerRoom_Patch
    {
        public static bool Prefix(string scene, Vector2 position, MapData ___CurrentMap)
        {
            if (position.x > -999f) return true;

            if (scene != "" && scene.Length == 9)
            {
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
                    return false;
                }
            }

            // Scene is not a valid one on the map
            Main.Multiplayer.mapScreenManager.setActivePlayerPosition(new Vector2(-1, -1));
            Main.Multiplayer.Log("Player zone '" + scene + "' does not exist!");
            return false;
        }
    }
}
