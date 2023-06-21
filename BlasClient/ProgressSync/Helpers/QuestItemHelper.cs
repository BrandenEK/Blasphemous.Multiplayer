using Framework.Inventory;
using Framework.Managers;
using HarmonyLib;

namespace BlasClient.ProgressSync.Helpers
{
    public class QuestItemHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            if (progress.Value == 1)
                Core.InventoryManager.RemoveQuestItem(progress.Id);
            else
                Core.InventoryManager.AddQuestItem(progress.Id);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            QuestItem questItem = Core.InventoryManager.GetQuestItem(progress.Id);
            return progress.Value == 0 && questItem != null ? $"{Main.Multiplayer.Localize("itmnot")} {questItem.caption}" : null;
        }

        public void SendAllProgress()
        {
            foreach (QuestItem questItem in Core.InventoryManager.GetQuestItemOwned())
            {
                ProgressUpdate progress = new ProgressUpdate(questItem.id, ProgressType.QuestItem, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryManager), "AddQuestItem", typeof(QuestItem))]
    public class InventoryQuestItem_Patch
    {
        public static void Postfix(QuestItem questItem)
        {
            if (!Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                // Don't sync chalice quest
                if (questItem.id == "QI76" || questItem.id == "QI77")
                    return;

                ProgressUpdate progress = new ProgressUpdate(questItem.id, ProgressType.QuestItem, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "RemoveQuestItem", typeof(QuestItem))]
    public class InventoryQuestItemRemove_Patch
    {
        public static void Postfix(QuestItem questItem)
        {
            if (!Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                // Don't sync chalice quest
                if (questItem.id == "QI75" || questItem.id == "QI76" || questItem.id == "QI77")
                    return;

                ProgressUpdate progress = new ProgressUpdate(questItem.id, ProgressType.QuestItem, 1);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
}
