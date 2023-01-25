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
    // Air attack
    [HarmonyPatch(typeof(AirAttackBehaviour), "OnStateEnter")]
    public class AirAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirAttackBehaviour), "OnStateUpdate")]
    public class AirAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirAttackBehaviour), "OnStateExit")]
    public class AirAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Air upward
    [HarmonyPatch(typeof(AirUpwardAttackBehaviour), "OnStateEnter")]
    public class AirUpwardAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirUpwardAttackBehaviour), "OnStateUpdate")]
    public class AirUpwardAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirUpwardAttackBehaviour), "OnStateExit")]
    public class AirUpwardAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Main attack
    [HarmonyPatch(typeof(AttackBehaviour), "OnStateEnter")]
    public class AttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AttackBehaviour), "OnStateUpdate")]
    public class AttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Cancel combo
    [HarmonyPatch(typeof(CancelComboBehaviour), "OnStateEnter")]
    public class CancelComboBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(CancelComboBehaviour), "OnStateUpdate")]
    public class CancelComboBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Charged attack
    [HarmonyPatch(typeof(ChargedAttackBehaviour), "OnStateEnter")]
    public class ChargedAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ChargedAttackBehaviour), "OnStateUpdate")]
    public class ChargedAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ChargedAttackBehaviour), "OnStateExit")]
    public class ChargedAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Charged attack effect
    [HarmonyPatch(typeof(ChargedAttackEffectBehaviour), "OnStateEnter")]
    public class ChargedAttackEffectBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ChargedAttackEffectBehaviour), "OnStateUpdate")]
    public class ChargedAttackEffectBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Charging attack
    [HarmonyPatch(typeof(ChargingAttackBehaviour), "OnStateEnter")]
    public class ChargingAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Combo finisher starter
    [HarmonyPatch(typeof(FinishingComboStarterBehaviour), "OnStateEnter")]
    public class FinishingComboStarterBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FinishingComboStarterBehaviour), "OnStateUpdate")]
    public class FinishingComboStarterBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FinishingComboStarterBehaviour), "OnStateExit")]
    public class FinishingComboStarterBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Combo finisher up
    [HarmonyPatch(typeof(FinishingComboUpBehaviour), "OnStateEnter")]
    public class FinishingComboUpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FinishingComboUpBehaviour), "OnStateUpdate")]
    public class FinishingComboUpBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FinishingComboUpBehaviour), "OnStateExit")]
    public class FinishingComboUpBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Combo finisher down
    [HarmonyPatch(typeof(FinishingComboDownBehaviour), "OnStateEnter")]
    public class FinishingComboDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FinishingComboDownBehaviour), "OnStateExit")]
    public class FinishingComboDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ground upward
    [HarmonyPatch(typeof(GroundUpwardAttackBehaviour), "OnStateEnter")]
    public class GroundUpwardAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundUpwardAttackBehaviour), "OnStateUpdate")]
    public class GroundUpwardAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundUpwardAttackBehaviour), "OnStateExit")]
    public class GroundUpwardAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Guard to idle
    [HarmonyPatch(typeof(GuardToIdleBehaviour), "OnStateEnter")]
    public class GuardToIdleBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GuardToIdleBehaviour), "OnStateUpdate")]
    public class GuardToIdleBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GuardToIdleBehaviour), "OnStateExit")]
    public class GuardToIdleBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Lunge attack
    [HarmonyPatch(typeof(LungeAttackBehaviour), "OnStateEnter")]
    public class LungeAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LungeAttackBehaviour), "OnStateExit")]
    public class LungeAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Parry riposte
    [HarmonyPatch(typeof(ParryRepostBehaviour), "OnStateEnter")]
    public class ParryRepostBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ParryRepostBehaviour), "OnStateExit")]
    public class ParryRepostBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Parry success
    [HarmonyPatch(typeof(ParrySuccessBehaviour), "OnStateEnter")]
    public class ParrySuccessBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ParrySuccessBehaviour), "OnStateUpdate")]
    public class ParrySuccessBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ParrySuccessBehaviour), "OnStateExit")]
    public class ParrySuccessBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Start charging attack
    [HarmonyPatch(typeof(StartChargingAttackBehaviour), "OnStateEnter")]
    public class StartChargingAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Vertical attack landing
    [HarmonyPatch(typeof(VerticalAttackLandingBehaviour), "OnStateEnter")]
    public class VerticalAttackLandingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(VerticalAttackLandingBehaviour), "OnStateUpdate")]
    public class VerticalAttackLandingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(VerticalAttackLandingBehaviour), "OnStateExit")]
    public class VerticalAttackLandingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    

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
    [HarmonyPatch(typeof(GrabLadderDownBehaviour), "OnStateEnter")]
    public class GrabLadderDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GrabLadderDownBehaviour), "OnStateUpdate")]
    public class GrabLadderDownBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GrabLadderDownBehaviour), "OnStateExit")]
    public class GrabLadderDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
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


    // Death
    [HarmonyPatch(typeof(PlayerDeathAnimationBehaviour), "OnStateEnter")]
    public class PlayerDeathAnimationBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(PlayerDeathAnimationBehaviour), "OnStateUpdate")]
    public class PlayerDeathAnimationBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Death by fall
    [HarmonyPatch(typeof(PlayerDeathFallBehaviour), "OnStateEnter")]
    public class PlayerDeathFallBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Death by spike
    [HarmonyPatch(typeof(PlayerDeathSpikeBehaviour), "OnStateEnter")]
    public class PlayerDeathSpikeBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Healing
    [HarmonyPatch(typeof(HealingBehaviour), "OnStateEnter")]
    public class HealingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HealingBehaviour), "OnStateExit")]
    public class HealingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ground hurt
    [HarmonyPatch(typeof(GroundHurtBehaviour), "OnStateEnter")]
    public class GroundHurtBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundHurtBehaviour), "OnStateUpdate")]
    public class GroundHurtBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundHurtBehaviour), "OnStateExit")]
    public class GroundHurtBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Air hurt
    [HarmonyPatch(typeof(AirHurtBehaviour), "OnStateEnter")]
    public class AirHurtBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirHurtBehaviour), "OnStateUpdate")]
    public class AirHurtBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Grounding over
    [HarmonyPatch(typeof(GroundingOverBehaviour), "OnStateEnter")]
    public class GroundingOverBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundingOverBehaviour), "OnStateUpdate")]
    public class GroundingOverBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundingOverBehaviour), "OnStateExit")]
    public class GroundingOverBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Falling over
    [HarmonyPatch(typeof(FallingOverBehaviour), "OnStateEnter")]
    public class FallingOverBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FallingOverBehaviour), "OnStateUpdate")]
    public class FallingOverBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Jump
    [HarmonyPatch(typeof(JumpBehaviour), "OnStateEnter")]
    public class JumpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Jump forward
    [HarmonyPatch(typeof(JumpForwardBehaviour), "OnStateEnter")]
    public class JumpForwardBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(JumpForwardBehaviour), "OnStateExit")]
    public class JumpForwardBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Jump off
    [HarmonyPatch(typeof(JumpOffBehaviour), "OnStateEnter")]
    public class JumpOffBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(JumpOffBehaviour), "OnStateUpdate")]
    public class JumpOffBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(JumpOffBehaviour), "OnStateExit")]
    public class JumpOffBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Falling
    [HarmonyPatch(typeof(FallingBehaviour), "OnStateEnter")]
    public class FallingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FallingBehaviour), "OnStateUpdate")]
    public class FallingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Falling forward
    [HarmonyPatch(typeof(FallingForwardBehaviour), "OnStateEnter")]
    public class FallingForwardBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FallingForwardBehaviour), "OnStateUpdate")]
    public class FallingForwardBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(FallingForwardBehaviour), "OnStateExit")]
    public class FallingForwardBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Landing
    [HarmonyPatch(typeof(LandingBehaviour), "OnStateEnter")]
    public class LandingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LandingBehaviour), "OnStateUpdate")]
    public class LandingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LandingBehaviour), "OnStateExit")]
    public class LandingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Landing running
    [HarmonyPatch(typeof(LandingRunningBehaviour), "OnStateEnter")]
    public class LandingRunningBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(LandingRunningBehaviour), "OnStateExit")]
    public class LandingRunningBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Hard landing
    [HarmonyPatch(typeof(HardLandingBehaviour), "OnStateEnter")]
    public class HardLandingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HardLandingBehaviour), "OnStateUpdate")]
    public class HardLandingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HardLandingBehaviour), "OnStateExit")]
    public class HardLandingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Wall contact
    [HarmonyPatch(typeof(WallJumpContactBehaviour), "OnStateEnter")]
    public class WallJumpContactBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Aura transform
    [HarmonyPatch(typeof(AuraTransformBehaviour), "OnStateEnter")]
    public class AuraTransformBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AuraTransformBehaviour), "OnStateExit")]
    public class AuraTransformBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // High wills respawn
    [HarmonyPatch(typeof(HighWillsRespawnBehaviour), "OnStateEnter")]
    public class HighWillsRespawnBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HighWillsRespawnBehaviour), "OnStateExit")]
    public class HighWillsRespawnBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Return to port
    [HarmonyPatch(typeof(PR202TeleportBehaviour), "OnStateEnter")]
    public class PR202TeleportBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(PR202TeleportBehaviour), "OnStateExit")]
    public class PR202TeleportBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Ground range attack
    [HarmonyPatch(typeof(GroundRangeAttackBehaviour), "OnStateEnter")]
    public class GroundRangeAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundRangeAttackBehaviour), "OnStateUpdate")]
    public class GroundRangeAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundRangeAttackBehaviour), "OnStateExit")]
    public class GroundRangeAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Mid air range attack
    [HarmonyPatch(typeof(MidAirRangeAttackBehaviour), "OnStateEnter")]
    public class MidAirRangeAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(MidAirRangeAttackBehaviour), "OnStateUpdate")]
    public class MidAirRangeAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(MidAirRangeAttackBehaviour), "OnStateExit")]
    public class MidAirRangeAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }


    // Idle
    [HarmonyPatch(typeof(IdleAnimatonBehaviour), "OnStateEnter")]
    public class IdleAnimatonBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(IdleAnimatonBehaviour), "OnStateExit")]
    public class IdleAnimatonBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Move
    [HarmonyPatch(typeof(MoveAnimationBehaviour), "OnStateEnter")]
    public class MoveAnimationBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Run
    [HarmonyPatch(typeof(RunStartBehaviour), "OnStateEnter")]
    public class RunStartBehaviourEnter_Patch
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
    // Hurt
    [HarmonyPatch(typeof(HurtSubStateBehaviour), "OnStateEnter")]
    public class HurtSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HurtSubStateBehaviour), "OnStateUpdate")]
    public class HurtSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(HurtSubStateBehaviour), "OnStateExit")]
    public class HurtSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Ground attack
    [HarmonyPatch(typeof(GroundAttackSubStateBehaviour), "OnStateEnter")]
    public class GroundAttackSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundAttackSubStateBehaviour), "OnStateUpdate")]
    public class GroundAttackSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(GroundAttackSubStateBehaviour), "OnStateExit")]
    public class GroundAttackSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Air attack
    [HarmonyPatch(typeof(AirAttackSubStateBehaviour), "OnStateEnter")]
    public class AirAttackSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(AirAttackSubStateBehaviour), "OnStateExit")]
    public class AirAttackSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    // Charge attack
    [HarmonyPatch(typeof(ChargeAttackSubStateBehaviour), "OnStateEnter")]
    public class ChargeAttackSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ChargeAttackSubStateBehaviour), "OnStateUpdate")]
    public class ChargeAttackSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
    [HarmonyPatch(typeof(ChargeAttackSubStateBehaviour), "OnStateExit")]
    public class ChargeAttackSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) { return animator.name == "Body"; }
    }
}
