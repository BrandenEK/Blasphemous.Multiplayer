using Blasphemous.Multiplayer.Common.Enums;
using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class ChurchDonationHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            int tears = progress.Value * 1000;
            Core.Alms.DEBUG_SetTearsGiven(tears); // Not ideal
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            return null;
        }

        public void SendAllProgress()
        {
            ProgressUpdate progress = new ProgressUpdate("Tears", ProgressType.ChurchDonation, (byte)(Core.Alms.TearsGiven / 1000));
            Main.Multiplayer.NetworkManager.SendProgress(progress);
        }
    }

    [HarmonyPatch(typeof(AlmsManager), "ConsumeTears")]
    class AlmsManager_Patch
    {
        public static void Postfix(AlmsManager __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
                return;

            ProgressUpdate progress = new ProgressUpdate("Tears", ProgressType.ChurchDonation, (byte)(__instance.TearsGiven / 1000));
            Main.Multiplayer.NetworkManager.SendProgress(progress);
        }
    }
}
