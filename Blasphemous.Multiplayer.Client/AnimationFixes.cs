using Blasphemous.Multiplayer.Client.PvP.Models;
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
using HarmonyLib;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client
{
    // Default attack
    [HarmonyPatch(typeof(DefaultSwordSlashBehaviour), "OnStateEnter")]
    class DefaultSwordSlashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Slash";
    }
    [HarmonyPatch(typeof(DefaultSwordSlashBehaviour), "OnStateExit")]
    class DefaultSwordSlashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Slash";
    }
    // Air attack
    [HarmonyPatch(typeof(AirAttackBehaviour), "OnStateEnter")]
    class AirAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.SidewaysAir);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(AirAttackBehaviour), "OnStateUpdate")]
    class AirAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(AirAttackBehaviour), "OnStateExit")]
    class AirAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Air upward
    [HarmonyPatch(typeof(AirUpwardAttackBehaviour), "OnStateEnter")]
    class AirUpwardAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.UpwardsAir);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(AirUpwardAttackBehaviour), "OnStateUpdate")]
    class AirUpwardAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(AirUpwardAttackBehaviour), "OnStateExit")]
    class AirUpwardAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Main attack
    [HarmonyPatch(typeof(AttackBehaviour), "OnStateEnter")]
    class AttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.SidewaysGrounded);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(AttackBehaviour), "OnStateUpdate")]
    class AttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Cancel combo
    [HarmonyPatch(typeof(CancelComboBehaviour), "OnStateEnter")]
    class CancelComboBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(CancelComboBehaviour), "OnStateUpdate")]
    class CancelComboBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Charged attack
    [HarmonyPatch(typeof(ChargedAttackBehaviour), "OnStateEnter")]
    class ChargedAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ChargedAttackBehaviour), "OnStateUpdate")]
    class ChargedAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ChargedAttackBehaviour), "OnStateExit")]
    class ChargedAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Charged attack effect
    [HarmonyPatch(typeof(ChargedAttackEffectBehaviour), "OnStateEnter")]
    class ChargedAttackEffectBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ChargedAttackEffectBehaviour), "OnStateUpdate")]
    class ChargedAttackEffectBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Charging attack
    [HarmonyPatch(typeof(ChargingAttackBehaviour), "OnStateEnter")]
    class ChargingAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Combo finisher starter
    [HarmonyPatch(typeof(FinishingComboStarterBehaviour), "OnStateEnter")]
    class FinishingComboStarterBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FinishingComboStarterBehaviour), "OnStateUpdate")]
    class FinishingComboStarterBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FinishingComboStarterBehaviour), "OnStateExit")]
    class FinishingComboStarterBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Combo finisher up
    [HarmonyPatch(typeof(FinishingComboUpBehaviour), "OnStateEnter")]
    class FinishingComboUpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FinishingComboUpBehaviour), "OnStateUpdate")]
    class FinishingComboUpBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FinishingComboUpBehaviour), "OnStateExit")]
    class FinishingComboUpBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Combo finisher down
    [HarmonyPatch(typeof(FinishingComboDownBehaviour), "OnStateEnter")]
    class FinishingComboDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FinishingComboDownBehaviour), "OnStateExit")]
    class FinishingComboDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ground upward
    [HarmonyPatch(typeof(GroundUpwardAttackBehaviour), "OnStateEnter")]
    class GroundUpwardAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.UpwardsGrounded);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(GroundUpwardAttackBehaviour), "OnStateUpdate")]
    class GroundUpwardAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundUpwardAttackBehaviour), "OnStateExit")]
    class GroundUpwardAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Guard to idle
    [HarmonyPatch(typeof(GuardToIdleBehaviour), "OnStateEnter")]
    class GuardToIdleBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GuardToIdleBehaviour), "OnStateExit")]
    class GuardToIdleBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Lunge attack
    [HarmonyPatch(typeof(LungeAttackBehaviour), "OnStateEnter")]
    class LungeAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LungeAttackBehaviour), "OnStateExit")]
    class LungeAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Parry riposte
    [HarmonyPatch(typeof(ParryRepostBehaviour), "OnStateEnter")]
    class ParryRepostBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ParryRepostBehaviour), "OnStateExit")]
    class ParryRepostBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Parry success
    [HarmonyPatch(typeof(ParrySuccessBehaviour), "OnStateEnter")]
    class ParrySuccessBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ParrySuccessBehaviour), "OnStateUpdate")]
    class ParrySuccessBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ParrySuccessBehaviour), "OnStateExit")]
    class ParrySuccessBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Start charging attack
    [HarmonyPatch(typeof(StartChargingAttackBehaviour), "OnStateEnter")]
    class StartChargingAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Vertical attack landing
    [HarmonyPatch(typeof(VerticalAttackLandingBehaviour), "OnStateEnter")]
    class VerticalAttackLandingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(VerticalAttackLandingBehaviour), "OnStateUpdate")]
    class VerticalAttackLandingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(VerticalAttackLandingBehaviour), "OnStateExit")]
    class VerticalAttackLandingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    

    // Climb cliff
    [HarmonyPatch(typeof(ClimbCliffLedeBehaviour), "OnStateEnter")]
    class ClimbCliffLedeBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ClimbCliffLedeBehaviour), "OnStateUpdate")]
    class ClimbCliffLedeBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ClimbCliffLedeBehaviour), "OnStateExit")]
    class ClimbCliffLedeBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Hang on cliff
    [HarmonyPatch(typeof(HangOnCliffLedeBehaviour), "OnStateEnter")]
    class HangOnCliffLedeBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HangOnCliffLedeBehaviour), "OnStateUpdate")]
    class HangOnCliffLedeBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HangOnCliffLedeBehaviour), "OnStateExit")]
    class HangOnCliffLedeBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Grab ladder up
    [HarmonyPatch(typeof(GrabLadderBehaviour), "OnStateEnter")]
    class GrabLadderBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GrabLadderBehaviour), "OnStateUpdate")]
    class GrabLadderBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GrabLadderBehaviour), "OnStateExit")]
    class GrabLadderBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Grab ladder down
    [HarmonyPatch(typeof(GrabLadderDownBehaviour), "OnStateEnter")]
    class GrabLadderDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GrabLadderDownBehaviour), "OnStateUpdate")]
    class GrabLadderDownBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GrabLadderDownBehaviour), "OnStateExit")]
    class GrabLadderDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ladder go up
    [HarmonyPatch(typeof(LadderGoingUpBehaviour), "OnStateEnter")]
    class LadderGoingUpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderGoingUpBehaviour), "OnStateUpdate")]
    class LadderGoingUpBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderGoingUpBehaviour), "OnStateExit")]
    class LadderGoingUpBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ladder go down
    [HarmonyPatch(typeof(LadderGoingDownBehaviour), "OnStateEnter")]
    class LadderGoingDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderGoingDownBehaviour), "OnStateUpdate")]
    class LadderGoingDownBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderGoingDownBehaviour), "OnStateExit")]
    class LadderGoingDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ladder sliding
    [HarmonyPatch(typeof(LadderSlidingBehaviour), "OnStateEnter")]
    class LadderSlidingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderSlidingBehaviour), "OnStateUpdate")]
    class LadderSlidingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderSlidingBehaviour), "OnStateExit")]
    class LadderSlidingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ladder release top
    [HarmonyPatch(typeof(ReleaseTopLadderBehaviour), "OnStateEnter")]
    class ReleaseTopLadderBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ReleaseTopLadderBehaviour), "OnStateExit")]
    class ReleaseTopLadderBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ladder release bottom
    [HarmonyPatch(typeof(ReleaseBottomLadderBehaviour), "OnStateEnter")]
    class ReleaseBottomLadderBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ReleaseBottomLadderBehaviour), "OnStateExit")]
    class ReleaseBottomLadderBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Crouch attack
    [HarmonyPatch(typeof(CrouchAttackBehaviour), "OnStateEnter")]
    class CrouchAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.Crouch);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(CrouchAttackBehaviour), "OnStateUpdate")]
    class CrouchAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(CrouchAttackBehaviour), "OnStateExit")]
    class CrouchAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Crouch down
    [HarmonyPatch(typeof(CrouchDownBehaviour), "OnStateEnter")]
    class CrouchDownBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(CrouchDownBehaviour), "OnStateUpdate")]
    class CrouchDownBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(CrouchDownBehaviour), "OnStateExit")]
    class CrouchDownBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Crouch up
    [HarmonyPatch(typeof(CrouchUpBehaviour), "OnStateEnter")]
    class CrouchUpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(CrouchUpBehaviour), "OnStateUpdate")]
    class CrouchUpBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Dashing
    [HarmonyPatch(typeof(DashBehaviour), "OnStateEnter")]
    class DashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(DashBehaviour), "OnStateUpdate")]
    class DashBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(DashBehaviour), "OnStateExit")]
    class DashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Air dash
    [HarmonyPatch(typeof(AirDashBehaviour), "OnStateEnter")]
    class AirDashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(AirDashBehaviour), "OnStateExit")]
    class AirDashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Run after dash
    [HarmonyPatch(typeof(RunAfterDashBehaviour), "OnStateEnter")]
    class RunAfterDashBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(RunAfterDashBehaviour), "OnStateUpdate")]
    class RunAfterDashBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(RunAfterDashBehaviour), "OnStateExit")]
    class RunAfterDashBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Death
    [HarmonyPatch(typeof(PlayerDeathAnimationBehaviour), "OnStateEnter")]
    class PlayerDeathAnimationBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(PlayerDeathAnimationBehaviour), "OnStateUpdate")]
    class PlayerDeathAnimationBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Death by fall
    [HarmonyPatch(typeof(PlayerDeathFallBehaviour), "OnStateEnter")]
    class PlayerDeathFallBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Death by spike
    [HarmonyPatch(typeof(PlayerDeathSpikeBehaviour), "OnStateEnter")]
    class PlayerDeathSpikeBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Healing
    [HarmonyPatch(typeof(HealingBehaviour), "OnStateEnter")]
    class HealingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HealingBehaviour), "OnStateExit")]
    class HealingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ground hurt
    [HarmonyPatch(typeof(GroundHurtBehaviour), "OnStateEnter")]
    class GroundHurtBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundHurtBehaviour), "OnStateUpdate")]
    class GroundHurtBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundHurtBehaviour), "OnStateExit")]
    class GroundHurtBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Air hurt
    [HarmonyPatch(typeof(AirHurtBehaviour), "OnStateEnter")]
    class AirHurtBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(AirHurtBehaviour), "OnStateUpdate")]
    class AirHurtBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Grounding over
    [HarmonyPatch(typeof(GroundingOverBehaviour), "OnStateEnter")]
    class GroundingOverBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundingOverBehaviour), "OnStateUpdate")]
    class GroundingOverBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundingOverBehaviour), "OnStateExit")]
    class GroundingOverBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Falling over
    [HarmonyPatch(typeof(FallingOverBehaviour), "OnStateEnter")]
    class FallingOverBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FallingOverBehaviour), "OnStateUpdate")]
    class FallingOverBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Jump
    [HarmonyPatch(typeof(JumpBehaviour), "OnStateEnter")]
    class JumpBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Jump forward
    [HarmonyPatch(typeof(JumpForwardBehaviour), "OnStateEnter")]
    class JumpForwardBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(JumpForwardBehaviour), "OnStateExit")]
    class JumpForwardBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Jump off
    [HarmonyPatch(typeof(JumpOffBehaviour), "OnStateEnter")]
    class JumpOffBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(JumpOffBehaviour), "OnStateUpdate")]
    class JumpOffBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(JumpOffBehaviour), "OnStateExit")]
    class JumpOffBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Falling
    [HarmonyPatch(typeof(FallingBehaviour), "OnStateEnter")]
    class FallingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FallingBehaviour), "OnStateUpdate")]
    class FallingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Falling forward
    [HarmonyPatch(typeof(FallingForwardBehaviour), "OnStateEnter")]
    class FallingForwardBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FallingForwardBehaviour), "OnStateUpdate")]
    class FallingForwardBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(FallingForwardBehaviour), "OnStateExit")]
    class FallingForwardBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Landing
    [HarmonyPatch(typeof(LandingBehaviour), "OnStateEnter")]
    class LandingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LandingBehaviour), "OnStateUpdate")]
    class LandingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LandingBehaviour), "OnStateExit")]
    class LandingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Landing running
    [HarmonyPatch(typeof(LandingRunningBehaviour), "OnStateEnter")]
    class LandingRunningBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LandingRunningBehaviour), "OnStateExit")]
    class LandingRunningBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Hard landing
    [HarmonyPatch(typeof(HardLandingBehaviour), "OnStateEnter")]
    class HardLandingBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HardLandingBehaviour), "OnStateUpdate")]
    class HardLandingBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HardLandingBehaviour), "OnStateExit")]
    class HardLandingBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Wall contact
    [HarmonyPatch(typeof(WallJumpContactBehaviour), "OnStateEnter")]
    class WallJumpContactBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Aura transform
    [HarmonyPatch(typeof(AuraTransformBehaviour), "OnStateEnter")]
    class AuraTransformBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(AuraTransformBehaviour), "OnStateExit")]
    class AuraTransformBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // High wills respawn
    [HarmonyPatch(typeof(HighWillsRespawnBehaviour), "OnStateEnter")]
    class HighWillsRespawnBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HighWillsRespawnBehaviour), "OnStateExit")]
    class HighWillsRespawnBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Return to port
    [HarmonyPatch(typeof(PR202TeleportBehaviour), "OnStateEnter")]
    class PR202TeleportBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(PR202TeleportBehaviour), "OnStateExit")]
    class PR202TeleportBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Ground range attack
    [HarmonyPatch(typeof(GroundRangeAttackBehaviour), "OnStateEnter")]
    class GroundRangeAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.Ranged);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(GroundRangeAttackBehaviour), "OnStateUpdate")]
    class GroundRangeAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundRangeAttackBehaviour), "OnStateExit")]
    class GroundRangeAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Mid air range attack
    [HarmonyPatch(typeof(MidAirRangeAttackBehaviour), "OnStateEnter")]
    class MidAirRangeAttackBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator)
        {
            if (animator.name == "Body")
            {
                Main.Multiplayer.NetworkManager.SendEffect(EffectType.Ranged);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(MidAirRangeAttackBehaviour), "OnStateUpdate")]
    class MidAirRangeAttackBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(MidAirRangeAttackBehaviour), "OnStateExit")]
    class MidAirRangeAttackBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Idle
    [HarmonyPatch(typeof(IdleAnimatonBehaviour), "OnStateEnter")]
    class IdleAnimatonBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(IdleAnimatonBehaviour), "OnStateExit")]
    class IdleAnimatonBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Move
    [HarmonyPatch(typeof(MoveAnimationBehaviour), "OnStateEnter")]
    class MoveAnimationBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Run
    [HarmonyPatch(typeof(RunStartBehaviour), "OnStateEnter")]
    class RunStartBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }


    // Sub states


    // Crouch
    [HarmonyPatch(typeof(CrouchSubStateBehaviour), "OnStateUpdate")]
    class CrouchSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ladder climbing
    [HarmonyPatch(typeof(LadderClimbingSubStateBehaviour), "OnStateEnter")]
    class LadderClimbingSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderClimbingSubStateBehaviour), "OnStateUpdate")]
    class LadderClimbingSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(LadderClimbingSubStateBehaviour), "OnStateExit")]
    class LadderClimbingSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Hurt
    [HarmonyPatch(typeof(HurtSubStateBehaviour), "OnStateEnter")]
    class HurtSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HurtSubStateBehaviour), "OnStateUpdate")]
    class HurtSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(HurtSubStateBehaviour), "OnStateExit")]
    class HurtSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Ground attack
    [HarmonyPatch(typeof(GroundAttackSubStateBehaviour), "OnStateEnter")]
    class GroundAttackSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundAttackSubStateBehaviour), "OnStateUpdate")]
    class GroundAttackSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(GroundAttackSubStateBehaviour), "OnStateExit")]
    class GroundAttackSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Air attack
    [HarmonyPatch(typeof(AirAttackSubStateBehaviour), "OnStateEnter")]
    class AirAttackSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(AirAttackSubStateBehaviour), "OnStateExit")]
    class AirAttackSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    // Charge attack
    [HarmonyPatch(typeof(ChargeAttackSubStateBehaviour), "OnStateEnter")]
    class ChargeAttackSubStateBehaviourEnter_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ChargeAttackSubStateBehaviour), "OnStateUpdate")]
    class ChargeAttackSubStateBehaviourUpdate_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
    [HarmonyPatch(typeof(ChargeAttackSubStateBehaviour), "OnStateExit")]
    class ChargeAttackSubStateBehaviourExit_Patch
    {
        public static bool Prefix(Animator animator) => animator.name == "Body";
    }
}
