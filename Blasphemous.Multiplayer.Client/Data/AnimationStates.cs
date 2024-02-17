
namespace Blasphemous.Multiplayer.Client.Data
{
    public static class AnimationStates
    {
        public static PlayerAnimState[] animations = new PlayerAnimState[]
        {
            // Idling
            new PlayerAnimState("Idle", new string[] { "GROUNDED", "RUN_STEP", "IS_CLIMBING_LADDER", "IS_IDLE_MODE", "IS_DIALOGUE_MODE" }, new bool[] { true, false, false, false, false }),
            new PlayerAnimState("IdleMode", new string[] { "IS_IDLE_MODE" }, new bool[] { true }),
            new PlayerAnimState("IdleModeLoop", new string[] { "IS_IDLE_MODE" }, new bool[] { true }),
            new PlayerAnimState("IdleToSheathed", new string[] { }, new bool[] { }),
            new PlayerAnimState("SheathedLoop", new string[] { "IS_DIALOGUE_MODE" }, new bool[] { true }),
            new PlayerAnimState("SheathedToIdle", new string[] { }, new bool[] { }),

            // Running, dashing, & crouching
            new PlayerAnimState("Run", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, true }),
            new PlayerAnimState("Run Step", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, true }),
            new PlayerAnimState("Run Start", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, true }),
            new PlayerAnimState("Run Stop", new string[] { "GROUNDED", "IS_CLIMBING_LADDER", "RUNNING" }, new bool[] { true, false, false }),
            new PlayerAnimState("Start_Run_After_Dash", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("Dash", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("Dash_Stop", new string[] { "GROUNDED", "RUNNING" }, new bool[] { true, false }),
            new PlayerAnimState("Crouch Down", new string[] { "IS_CROUCH", "STEP_ON_LADDER" }, new bool[] { true, false }),
            new PlayerAnimState("Crouch Up", new string[] { "RUNNING" }, new bool[] { false }),

            // Jumping, falling, & landing
            new PlayerAnimState("Jump", new string[] { "STICK_ON_WALL", "IS_CLIMBING_LADDER", "IS_GRABBING_CLIFF_LEDE", "GROUNDED", "AXIS_THRESHOLD" }, new bool[] { false, false, false, false, false }),
            new PlayerAnimState("Jump Forward", new string[] { "STICK_ON_WALL", "IS_CLIMBING_LADDER", "IS_GRABBING_CLIFF_LEDE", "GROUNDED", "AXIS_THRESHOLD" }, new bool[] { false, false, false, false, true }),
            new PlayerAnimState("Jump Off", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("Falling", new string[] { "STICK_ON_WALL", "IS_CLIMBING_LADDER", "IS_GRABBING_CLIFF_LEDE", "GROUNDED", "AXIS_THRESHOLD" }, new bool[] { false, false, false, false, false }),
            new PlayerAnimState("Falling Forward", new string[] { "STICK_ON_WALL", "IS_CLIMBING_LADDER", "IS_GRABBING_CLIFF_LEDE", "GROUNDED" }, new bool[] { false, false, false, false }),
            new PlayerAnimState("Falling Over", new string[] { "GROUNDED" }, new bool[] { false }),
            new PlayerAnimState("Falling Back Grounded", new string[] { }, new bool[] { }),
            new PlayerAnimState("Landing", new string[] { "IS_CLIMBING_LADDER", "RUNNING", "GROUNDED" }, new bool[] { false, false, true }),
            new PlayerAnimState("Landing Running", new string[] { "IS_CLIMBING_LADDER", "GROUNDED" }, new bool[] { false, true }),
            new PlayerAnimState("HardLanding", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("Grounding Over", new string[] { }, new bool[] { }),
            new PlayerAnimState("penitent_getting_up", new string[] { }, new bool[] { }),
            
            // Wall, ladder, & ledge climbing
            new PlayerAnimState("WallClimbContact", new string[] { "STICK_ON_WALL" }, new bool[] { true }),
            new PlayerAnimState("WallClimbIdle", new string[] { "STICK_ON_WALL" }, new bool[] { true }),
            new PlayerAnimState("WallClimbJump", new string[] { }, new bool[] { }),
            new PlayerAnimState("WallClimbUnhang", new string[] { }, new bool[] { }),
            new PlayerAnimState("grab_ladder_to_go_up", new string[] { }, new bool[] { }),
            new PlayerAnimState("grab_ladder_to_go_down", new string[] { }, new bool[] { }),
            new PlayerAnimState("ladder_going_up", new string[] { }, new bool[] { }),
            new PlayerAnimState("ladder_going_down", new string[] { "LADDER_SLIDING", "GROUNDED" }, new bool[] { false, false }),
            new PlayerAnimState("release_ladder_to_floor_up", new string[] { }, new bool[] { }),
            new PlayerAnimState("release_ladder_to_floor_down", new string[] { }, new bool[] { }),
            new PlayerAnimState("ladder_sliding", new string[] { "LADDER_SLIDING", "GROUNDED" }, new bool[] { true, false }),
            new PlayerAnimState("Player_hangonledge", new string[] { }, new bool[] { }),
            new PlayerAnimState("Player_Climb_Edge", new string[] { }, new bool[] { }),

            // Parrying, hurting, & dying
            new PlayerAnimState("ParryStart", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("ParryChance", new string[] { "PARRY", "GROUNDED" }, new bool[] { false, true }),
            new PlayerAnimState("ParryFailed", new string[] { }, new bool[] { }),
            new PlayerAnimState("ParrySuccess", new string[] { }, new bool[] { }),
            new PlayerAnimState("ParryRepost", new string[] { }, new bool[] { }),
            new PlayerAnimState("Hurt", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("Hurt In The Air", new string[] { "GROUNDED" }, new bool[] { false }),
            new PlayerAnimState("Death", new string[] { "IS_DEAD" }, new bool[] { true }),
            new PlayerAnimState("Death Spike", new string[] { }, new bool[] { }),
            new PlayerAnimState("Death Fall", new string[] { }, new bool[] { }),

            // Regular attacks
            new PlayerAnimState("Attack", new string[] { "GROUNDED", "RUNNING" }, new bool[] { true, false }),
            new PlayerAnimState("Attack_Running", new string[] { "GROUNDED", "RUNNING" }, new bool[] { true, false }),
            new PlayerAnimState("GroundUpwardAttack", new string[] { }, new bool[] { }),
            new PlayerAnimState("Crouch Attack", new string[] { "IS_CROUCH" }, new bool[] { true }),
            new PlayerAnimState("Air Attack 1", new string[] { "GROUNDED", "AXIS_THRESHOLD" }, new bool[] { false, true }),
            new PlayerAnimState("Air Attack 2", new string[] { "GROUNDED", "AXIS_THRESHOLD" }, new bool[] { false, true }),
            new PlayerAnimState("AirUpwardAttack", new string[] { "GROUNDED", "AXIS_THRESHOLD" }, new bool[] { false, false }),
            new PlayerAnimState("GuardSlide", new string[] { }, new bool[] { }),
            new PlayerAnimState("GuardToIdle",  new string[] { }, new bool[] { }),

            // Skill attacks
            new PlayerAnimState("Combo_1", new string[] { "RUNNING" }, new bool[] { false }),
            new PlayerAnimState("Combo_2", new string[] { "RUNNING" }, new bool[] { false }),
            new PlayerAnimState("Combo_3", new string[] { "RUNNING" }, new bool[] { false }),
            new PlayerAnimState("Combo_4", new string[] { "RUNNING" }, new bool[] { false }),
            new PlayerAnimState("ComboFinisherUp", new string[] { }, new bool[] { }),
            new PlayerAnimState("ComboFinisherDown", new string[] { }, new bool[] { }),
            new PlayerAnimState("Start Charging", new string[] { "IS_ATTACK_HOLD" }, new bool[] { true }),
            new PlayerAnimState("Charging", new string[] { "IS_ATTACK_HOLD", "CHARGE_ATTACK_TIER" }, new bool[] { true, false }),
            new PlayerAnimState("ChargedLoop", new string[] { "IS_ATTACK_HOLD" }, new bool[] { true }),
            new PlayerAnimState("Charged Attack Effect", new string[] { "IS_ATTACK_HOLD" }, new bool[] { true }),
            new PlayerAnimState("Charged Attack Effect Upgraded", new string[] { }, new bool[] { }),
            new PlayerAnimState("Charged Attack", new string[] { }, new bool[] { }),
            new PlayerAnimState("GroundRangeAttack", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("MidAirRangeAttack", new string[] { "GROUNDED" }, new bool[] { false }),
            new PlayerAnimState("VerticalAttackStart", new string[] { }, new bool[] { }),
            new PlayerAnimState("VerticalAttackFalling", new string[] { "GROUNDED" }, new bool[] { false }),
            new PlayerAnimState("VerticalAttackLanding", new string[] { }, new bool[] { }),
            new PlayerAnimState("LungeAttack", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("LungeAttack_Lv2", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("LungeAttack_Lv3", new string[] { "GROUNDED" }, new bool[] { true }),

            // Miscellaneous
            new PlayerAnimState("ThrowbackTrans", new string[] { }, new bool[] { }),
            new PlayerAnimState("ThrowbackDesc", new string[] { }, new bool[] { }),
            new PlayerAnimState("Healing", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("AuraTransform", new string[] { }, new bool[] { }),
            new PlayerAnimState("FervourPenance", new string[] { "GROUNDED" }, new bool[] { true }),
            new PlayerAnimState("HighWillsRespawn", new string[] { }, new bool[] { }),
            new PlayerAnimState("RegresoAPuerto", new string[] { }, new bool[] { })
        };
    }

    [System.Serializable]
    public class PlayerAnimState
    {
        public string name;
        public string[] parameterNames;
        public bool[] parameterValues;

        public PlayerAnimState(string name, string[] parameterNames, bool[] parameterValues)
        {
            this.name = name;
            this.parameterNames = parameterNames;
            this.parameterValues = parameterValues;
        }
    }
}
