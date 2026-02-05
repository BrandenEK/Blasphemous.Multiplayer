using Blasphemous.Multiplayer.Common.Enums;
using Framework.Inventory;
using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class HeartHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.InventoryManager.AddSword(progress.Id);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            Sword sword = Core.InventoryManager.GetSword(progress.Id);
            return progress.Value == 0 && sword != null ? $"{Main.Multiplayer.LocalizationHandler.Localize("itmnot")} {sword.caption}" : null;
        }

        public void SendAllProgress()
        {
            foreach (Sword sword in Core.InventoryManager.GetSwordsOwned())
            {
                ProgressUpdate progress = new ProgressUpdate(sword.id, ProgressType.Heart, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryManager), "AddSword", typeof(Sword))]
    class InventorySword_Patch
    {
        public static void Postfix(Sword sword)
        {
            if (!Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                ProgressUpdate progress = new ProgressUpdate(sword.id, ProgressType.Heart, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
}
