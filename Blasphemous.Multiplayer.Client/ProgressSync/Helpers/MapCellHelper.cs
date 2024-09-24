using Framework.Managers;
using Framework.Map;
using HarmonyLib;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class MapCellHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.NewMapManager.RevealCellInPosition(new Vector2(int.Parse(progress.Id), 0));
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            return null;
        }

        public void SendAllProgress()
        {
            Core.NewMapManager.GetCurrentPersistentState("intro", false);
        }
    }

    [HarmonyPatch(typeof(NewMapManager), "RevealCellInPosition")]
    class NewMapManager_Patch
    {
        public static bool Prefix(NewMapManager __instance, Vector2 position, MapData ___CurrentMap)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                // Received this new cell from other player, skip other stuff
                int cellIdx = Mathf.RoundToInt(position.x);
                if (___CurrentMap != null && cellIdx >= 0 && cellIdx < ___CurrentMap.Cells.Count)
                {
                    ___CurrentMap.Cells[cellIdx].Revealed = true;
                }
                return false;
            }

            // Actually revealing a new cell
            if (___CurrentMap == null || !___CurrentMap.CellsByZone.ContainsKey(__instance.CurrentScene))
                return false;

            foreach (CellData cell in ___CurrentMap.CellsByZone[__instance.CurrentScene])
            {
                if (cell.Bounding.Contains(position) && !cell.Revealed)
                {
                    ProgressUpdate progress = new ProgressUpdate(___CurrentMap.Cells.IndexOf(cell).ToString(), ProgressType.MapCell, 0);
                    Main.Multiplayer.NetworkManager.SendProgress(progress);
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NewMapManager), "GetCurrentPersistentState")]
    class MapManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, MapData ___CurrentMap)
        {
            // Calling this with 'intro' means it should send all revealed map cells
            if (dataPath != "intro") return true;

            for (int i = 0; i < ___CurrentMap.Cells.Count; i++)
            {
                if (___CurrentMap.Cells[i].Revealed)
                {
                    ProgressUpdate progress = new ProgressUpdate(i.ToString(), ProgressType.MapCell, 0);
                    Main.Multiplayer.NetworkManager.SendProgress(progress);
                }
            }
            return false;
        }
    }
}
