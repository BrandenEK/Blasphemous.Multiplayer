namespace BlasClient
{
    public static class StaticObjects
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

        public static FlagState[] flags = new FlagState[]
        {
            new FlagState("RESCUED_CHERUB_*", "has freed a Child of Moonlight"),
            new FlagState("CLAIMED_*", null),
            new FlagState("CONFESSOR_*", null),

            // Bosses
            new FlagState("D17Z01_BOSSDEAD", "has defeated the Warden"),
            new FlagState("D01Z06S01_BOSSDEAD", "has defeated Ten Piedad"),
            new FlagState("D03Z04S01_BOSSDEAD", "has defeated Tres Angustias"),
            new FlagState("D02Z05S01_BOSSDEAD", "has deafeated Our Lady of the Charred Visage"),
            new FlagState("D08Z01S01_BOSSDEAD", "has defeated Esdras"),
            new FlagState("D04Z04S01_BOSSDEAD", "has defeated Melquiades"),
            new FlagState("D05Z01S13_BOSSDEAD", "has defeated Exposito"),
            new FlagState("D09Z01S03_BOSSDEAD", "has defeated Quirce"),
            new FlagState("D06Z01S25_BOSSDEAD", "has defeated Crisanta"),
            new FlagState("D07Z01S02_BOSSDEAD", "Has defeated Escribar"),
            new FlagState("D20Z02S08_BOSSDEAD", "has defeated Sierpes"),
            new FlagState("D01BZ08S01_BOSSDEAD", "has defeated Isidora"),
            new FlagState("SANTOS_AMANECIDA_AXE_DEFEATED", "has defeated the Amanecida of the Golden Blades"),
            new FlagState("SANTOS_AMANECIDA_BOW_DEFEATED", "has defeated the Amanecida of the Bejeweled Arrow"),
            new FlagState("SANTOS_AMANECIDA_FACCATA_DEFEATED", "has defeated the Amanecida of the Chiselled Steel"),
            new FlagState("SANTOS_AMANECIDA_LANCE_DEFEATED", "has defeated the Amanecida of the Molten Thorn"),
            new FlagState("SANTOS_LAUDES_DEFEATED", "has defeated Laudes of the Amanecidas"),

            // Randomizer
            new FlagState("LOCATION_*", null),
            new FlagState("ITEM_*", null),
            new FlagState("OSSUARY_REWARD_*", null)
        };

        public static PersistenceState[] persistentObjects = new PersistenceState[]
        {
            // Brotherhood
            new PersistenceState("47db0007-4175-4379-8b5c-f37441f6315b", "D17Z01S05", 0), // PD

            // Holy Line
            new PersistenceState("3c6d3c9c-44c2-41cd-9c11-7c9f923680b9", "D01Z01S07", 0), // PD
            new PersistenceState("5710974d-771c-4907-8f1b-b318516ec0db", "D01Z01S02", 1), // CI
            new PersistenceState("9082d3bd-0568-4908-9e5f-00044182cbc4", "D01Z01S02", 1),
            new PersistenceState("e21c696e-61f5-4ef2-808a-cd0ccf852c0c", "D01Z01S03", 1),

            // Albero
            new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D01Z02S01", 0), // PD

            // Mercy Dreams
            new PersistenceState("0c160f00-3901-4bf2-b67f-99c2830819cf", "D01Z04S03", 0), // PD

            //new PersistenceState("", "", 0),
        };

        // Gets a certain flag only if it should be synced
        public static FlagState getFlagState(string id)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                bool found = false;
                string flag = flags[i].id;

                if (flag.EndsWith("*"))
                {
                    // This is a flag with many different numbers, so only check if beginning matches
                    found = id.StartsWith(flag.Substring(0, flag.Length - 1));
                }
                // Might need a check for if it starts with *
                else
                {
                    // This flag must be exact
                    found = id == flag;
                }

                if (found)
                    return flags[i];
            }

            // This flag should not be synced
            return null;
        }

        // Gets a certain persistent object data only if it should be synced
        public static PersistenceState GetPersistenceState(string id)
        {
            for (int i = 0; i < persistentObjects.Length; i++)
            {
                if (id == persistentObjects[i].id)
                    return persistentObjects[i];
            }
            return null;
        }
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

    [System.Serializable]
    public class FlagState
    {
        public string id;
        public string notification;

        public FlagState(string id, string notification)
        {
            this.id = id;
            this.notification = notification;
        }
    }

    [System.Serializable]
    public class PersistenceState
    {
        public string id;
        public string scene;
        public byte type;

        public PersistenceState(string id, string scene, byte type)
        {
            this.id = id;
            this.scene = scene;
            this.type = type;
        }
    }
}
