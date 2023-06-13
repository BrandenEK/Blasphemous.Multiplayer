using Framework.Inventory;
using Framework.Managers;
using HarmonyLib;

namespace BlasClient.ProgressSync.Helpers
{
    public class RelicHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.InventoryManager.AddRelic(progress.Id);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            Relic relic = Core.InventoryManager.GetRelic(progress.Id);
            return progress.Value == 0 && relic != null ? $"{Main.Multiplayer.Localize("itmnot")} {relic.caption}" : null;
        }

        public void SendAllProgress()
        {
            foreach (Relic relic in Core.InventoryManager.GetRelicsOwned())
            {
                ProgressUpdate progress = new ProgressUpdate(relic.id, ProgressType.Relic, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryManager), "AddRelic", typeof(Relic))]
    public class InventoryRelic_Patch
    {
        public static void Postfix(Relic relic)
        {
            if (!Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                ProgressUpdate progress = new ProgressUpdate(relic.id, ProgressType.Relic, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
}
