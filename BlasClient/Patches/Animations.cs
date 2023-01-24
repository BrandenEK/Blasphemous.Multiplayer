using Gameplay.GameControllers.AnimationBehaviours.Player.Dash;
using UnityEngine;
using HarmonyLib;

namespace BlasClient.Patches
{
    // Dashing
    [HarmonyPatch(typeof(DashBehaviour), "OnStateEnter")]
    public class DashEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(DashBehaviour), "OnStateUpdate")]
    public class DashUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(DashBehaviour), "OnStateExit")]
    public class DashExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
}
