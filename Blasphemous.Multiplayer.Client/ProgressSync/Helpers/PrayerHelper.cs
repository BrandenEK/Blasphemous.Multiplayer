using Framework.Inventory;
using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class PrayerHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.InventoryManager.AddPrayer(progress.Id);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            Prayer prayer = Core.InventoryManager.GetPrayer(progress.Id);
            return progress.Value == 0 && prayer != null ? $"{Main.Multiplayer.LocalizationHandler.Localize("itmnot")} {prayer.caption}" : null;
        }

        public void SendAllProgress()
        {
            foreach (Prayer prayer in Core.InventoryManager.GetPrayersOwned())
            {
                ProgressUpdate progress = new ProgressUpdate(prayer.id, ProgressType.Prayer, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryManager), "AddPrayer", typeof(Prayer))]
    public class InventoryPrayer_Patch
    {
        public static void Postfix(Prayer prayer)
        {
            if (!Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                ProgressUpdate progress = new ProgressUpdate(prayer.id, ProgressType.Prayer, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
}
