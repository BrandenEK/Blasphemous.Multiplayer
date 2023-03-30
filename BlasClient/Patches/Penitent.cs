using HarmonyLib;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Attack;

namespace BlasClient.Patches
{
    //[HarmonyPatch(typeof(PenitentSword), "Attack")]
    //public class PenitentSword_Patch
    //{
    //    public static void Postfix()
    //    {
    //        //Main.Multiplayer.SendNewAttack(0);
    //    }
    //}

    //[HarmonyPatch(typeof(PenitentAttack), "CurrentWeaponAttack", typeof(DamageArea.DamageType), typeof(bool))]
    //public class PenitentAttackOne_Patch
    //{
    //    public static void Postfix()
    //    {
    //        Main.Multiplayer.LogError("Attacking");
    //        Main.Multiplayer.SendNewAttack(0);
    //    }
    //}
    //[HarmonyPatch(typeof(PenitentAttack), "CurrentWeaponAttack", typeof(DamageArea.DamageType))]
    //public class PenitentAttackTwo_Patch
    //{
    //    public static void Postfix()
    //    {
    //        Main.Multiplayer.LogError("Attacking");
    //        Main.Multiplayer.SendNewAttack(0);
    //    }
    //}
}
