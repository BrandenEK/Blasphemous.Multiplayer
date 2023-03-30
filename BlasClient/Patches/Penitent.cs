using HarmonyLib;
using Gameplay.GameControllers.Penitent.Attack;

namespace BlasClient.Patches
{
    [HarmonyPatch(typeof(PenitentSword), "Attack")]
    public class PenitentSword_Patch
    {
        public static void Postfix()
        {
            //Main.Multiplayer.SendNewAttack(0);
        }
    }
}
