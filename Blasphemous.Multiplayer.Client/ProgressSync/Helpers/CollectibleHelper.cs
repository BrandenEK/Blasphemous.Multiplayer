using Blasphemous.Multiplayer.Common.Enums;
using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class CollectibleHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.InventoryManager.AddCollectibleItem(progress.Id);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            Framework.Inventory.CollectibleItem collectible = Core.InventoryManager.GetCollectibleItem(progress.Id);
            return progress.Value == 0 && collectible != null ? $"{Main.Multiplayer.LocalizationHandler.Localize("itmnot")} {collectible.caption}" : null;
        }

        public void SendAllProgress()
        {
            foreach (Framework.Inventory.CollectibleItem collectibleItem in Core.InventoryManager.GetCollectibleItemOwned())
            {
                ProgressUpdate progress = new ProgressUpdate(collectibleItem.id, ProgressType.Collectible, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryManager), "AddCollectibleItem", typeof(Framework.Inventory.CollectibleItem))]
    class InventoryCollectible_Patch
    {
        public static void Postfix(Framework.Inventory.CollectibleItem collectibleItem)
        {
            if (!Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                ProgressUpdate progress = new ProgressUpdate(collectibleItem.id, ProgressType.Collectible, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
}
