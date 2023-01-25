using Gameplay.GameControllers.AnimationBehaviours.Player.Attack;
using Gameplay.GameControllers.AnimationBehaviours.Player.ClimbClifLede;
using Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder;
using Gameplay.GameControllers.AnimationBehaviours.Player.Crouch;
using Gameplay.GameControllers.AnimationBehaviours.Player.Dash;
using Gameplay.GameControllers.AnimationBehaviours.Player.Dead;
using Gameplay.GameControllers.AnimationBehaviours.Player.Hurt;
using Gameplay.GameControllers.AnimationBehaviours.Player.Jump;
using Gameplay.GameControllers.AnimationBehaviours.Player.Prayer;
using Gameplay.GameControllers.AnimationBehaviours.Player.RangeAttack;
using Gameplay.GameControllers.AnimationBehaviours.Player.Run;
using Gameplay.GameControllers.AnimationBehaviours.Player.SubStatesBehaviours;
using UnityEngine;
using HarmonyLib;

namespace BlasClient.Patches
{
    // Climb cliff
    [HarmonyPatch(typeof(ClimbCliffLedeBehaviour), "OnStateEnter")]
    public class ClimbCliffLedeBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ClimbCliffLedeBehaviour), "OnStateUpdate")]
    public class ClimbCliffLedeBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ClimbCliffLedeBehaviour), "OnStateExit")]
    public class ClimbCliffLedeBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Hang on cliff
    [HarmonyPatch(typeof(HangOnCliffLedeBehaviour), "OnStateEnter")]
    public class HangOnCliffLedeBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HangOnCliffLedeBehaviour), "OnStateUpdate")]
    public class HangOnCliffLedeBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HangOnCliffLedeBehaviour), "OnStateExit")]
    public class HangOnCliffLedeBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Grab ladder up
    [HarmonyPatch(typeof(GrabLadderBehaviour), "OnStateEnter")]
    public class GrabLadderBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GrabLadderBehaviour), "OnStateUpdate")]
    public class GrabLadderBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GrabLadderBehaviour), "OnStateExit")]
    public class GrabLadderBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Grab ladder down
    //[HarmonyPatch(typeof(), "OnStateEnter")]
    //public class Enter_Patch
    //{
    //    public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    //}
    //[HarmonyPatch(typeof(), "OnStateUpdate")]
    //public class Update_Patch
    //{
    //    public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    //}
    //[HarmonyPatch(typeof(), "OnStateExit")]
    //public class Exit_Patch
    //{
    //    public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    //}
    // Ladder go up
    [HarmonyPatch(typeof(LadderGoingUpBehaviour), "OnStateEnter")]
    public class LadderGoingUpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderGoingUpBehaviour), "OnStateUpdate")]
    public class LadderGoingUpBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderGoingUpBehaviour), "OnStateExit")]
    public class LadderGoingUpBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ladder go down
    [HarmonyPatch(typeof(LadderGoingDownBehaviour), "OnStateEnter")]
    public class LadderGoingDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderGoingDownBehaviour), "OnStateUpdate")]
    public class LadderGoingDownBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderGoingDownBehaviour), "OnStateExit")]
    public class LadderGoingDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ladder sliding
    [HarmonyPatch(typeof(LadderSlidingBehaviour), "OnStateEnter")]
    public class LadderSlidingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderSlidingBehaviour), "OnStateUpdate")]
    public class LadderSlidingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderSlidingBehaviour), "OnStateExit")]
    public class LadderSlidingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ladder release top
    [HarmonyPatch(typeof(ReleaseTopLadderBehaviour), "OnStateEnter")]
    public class ReleaseTopLadderBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ReleaseTopLadderBehaviour), "OnStateExit")]
    public class ReleaseTopLadderBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ladder release bottom
    [HarmonyPatch(typeof(ReleaseBottomLadderBehaviour), "OnStateEnter")]
    public class ReleaseBottomLadderBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ReleaseBottomLadderBehaviour), "OnStateExit")]
    public class ReleaseBottomLadderBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Crouch attack
    [HarmonyPatch(typeof(CrouchAttackBehaviour), "OnStateEnter")]
    public class CrouchAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(CrouchAttackBehaviour), "OnStateUpdate")]
    public class CrouchAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(CrouchAttackBehaviour), "OnStateExit")]
    public class CrouchAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Crouch down
    [HarmonyPatch(typeof(CrouchDownBehaviour), "OnStateEnter")]
    public class CrouchDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(CrouchDownBehaviour), "OnStateUpdate")]
    public class CrouchDownBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(CrouchDownBehaviour), "OnStateExit")]
    public class CrouchDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Crouch up
    [HarmonyPatch(typeof(CrouchUpBehaviour), "OnStateEnter")]
    public class CrouchUpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(CrouchUpBehaviour), "OnStateUpdate")]
    public class CrouchUpBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Dashing
    [HarmonyPatch(typeof(DashBehaviour), "OnStateEnter")]
    public class DashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(DashBehaviour), "OnStateUpdate")]
    public class DashBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(DashBehaviour), "OnStateExit")]
    public class DashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Air dash
    [HarmonyPatch(typeof(AirDashBehaviour), "OnStateEnter")]
    public class AirDashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirDashBehaviour), "OnStateExit")]
    public class AirDashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Run after dash
    [HarmonyPatch(typeof(RunAfterDashBehaviour), "OnStateEnter")]
    public class RunAfterDashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(RunAfterDashBehaviour), "OnStateUpdate")]
    public class RunAfterDashBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(RunAfterDashBehaviour), "OnStateExit")]
    public class RunAfterDashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }



    // Sub states

    // Crouch
    [HarmonyPatch(typeof(CrouchSubStateBehaviour), "OnStateUpdate")]
    public class CrouchSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ladder climbing
    [HarmonyPatch(typeof(LadderClimbingSubStateBehaviour), "OnStateEnter")]
    public class LadderClimbingSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderClimbingSubStateBehaviour), "OnStateUpdate")]
    public class LadderClimbingSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LadderClimbingSubStateBehaviour), "OnStateExit")]
    public class LadderClimbingSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    /*
    // Template
    [HarmonyPatch(typeof(), "OnStateEnter")]
    public class Enter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(), "OnStateUpdate")]
    public class Update_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(), "OnStateExit")]
    public class Exit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    */
}
