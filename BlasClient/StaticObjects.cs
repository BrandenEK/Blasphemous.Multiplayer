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
            new FlagState("TELEPORT_D*", null),
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

            // Quests
            new FlagState("BROTHERS_EVENTPERPETVA_COMPLETED", null),
            new FlagState("BROTHERS_PERPETUA_DEFEATED", null),

            // World states
            new FlagState("DAGGER_ENCOUNTER_*", null),
            new FlagState("BAPTISMAL_0*", null),
            new FlagState("SWORD_UPGRADED_MC0*", null),
            new FlagState("CANDLE_RED_*", null),
            new FlagState("CANDLE_BLUE_*", null),
            new FlagState("ATTRITION_ALTAR_DONE", null),
            new FlagState("COMPUNCTION_ALTAR_DONE", null),
            new FlagState("CONTRITION_ALTAR_DONE", null),

            // Cutscenes
            new FlagState("DEOSGRACIAS_CUTSCENE_PLAYED", null),
            new FlagState("PONTIFF_ALBERO_EVENT", null),
            new FlagState("PONTIFF_BRIDGE_EVENT", null),
            new FlagState("PONTIFF_ARCHDEACON1_EVENT", null),
            new FlagState("PONTIFF_ARCHDEACON2_EVENT", null),
            new FlagState("BROTHERS_EVENT1_COMPLETED", null),
            new FlagState("BROTHERS_EVENT2_COMPLETED", null),
            new FlagState("BROTHERS_GRAVEYARD_EVENT", null),
            new FlagState("BROTHERS_WASTELAND_EVENT", null),

            // Room states
            new FlagState("D17Z01S04_WALLDESTROYED", null), // Brotherhood
            new FlagState("D17Z01S04_SHORTCUT", null),
            new FlagState("D01Z05S12_SHORTCUTGATESEWERS", null), // Cistern
            new FlagState("D01Z05S21_PUZZLESOLVED", null),
            new FlagState("D01Z05S02_ELEVATORPATHOPENED", null),
            //new FlagState("D03Z01S02_BELLCARRIER", null), // Mountains (Not until breakable wall also)
            new FlagState("BELL_PUZZLE1_ACTIVATED", "has broke the eastern bell"), // Jondo
            new FlagState("BELL_PUZZLE2_ACTIVATED", "has broke the western bell"),
            new FlagState("BELL_ACTIVATED", "has activated the bell"),
            new FlagState("D03Z02S13_CHALLENGEOVER", null),

            // Randomizer
            new FlagState("LOCATION_*", null),
            new FlagState("ITEM_*", null),
            new FlagState("OSSUARY_REWARD_*", null)
        };

        public static PersistenceState[] persistentObjects = new PersistenceState[]
        {
            // Brotherhood
            new PersistenceState("47db0007-4175-4379-8b5c-f37441f6315b", "D17Z01S05", 0), // PD
            new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D17Z01S13", 0),
            new PersistenceState("0f11c3cc-867e-47c3-a97c-983f2ef0b9ac", "D17BZ02S01", 1), // CI
            new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D17Z01S14", 1),
            new PersistenceState("d430347e-1b33-4a35-82fb-ac7d2e1b4a01", "D17Z01S01", 3), // CR
            new PersistenceState("5add17e9-f2d9-416e-a0e9-925d32ac4798", "D17Z01S03", 5), // GT
            new PersistenceState("0cca4185-35ae-4b75-b208-627f92ccfe29", "D17Z01S03", 7), // ST

            // Holy Line
            new PersistenceState("3c6d3c9c-44c2-41cd-9c11-7c9f923680b9", "D01Z01S07", 0), // PD
            new PersistenceState("5710974d-771c-4907-8f1b-b318516ec0db", "D01Z01S02", 1), // CI
            new PersistenceState("9082d3bd-0568-4908-9e5f-00044182cbc4", "D01Z01S02", 1),
            new PersistenceState("e21c696e-61f5-4ef2-808a-cd0ccf852c0c", "D01Z01S03", 1),
            new PersistenceState("5082226e-fcc9-4d90-bed0-9f6d4775fd75", "D01Z01S03", 2),
            new PersistenceState("c0cc4135-d795-48bd-879e-a30c6d80297c", "D01Z01S03", 3), // CR

            // Albero
            new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D01Z02S01", 0), // PD
            new PersistenceState("d402af99-cf62-4c4f-bc37-9a7d6f860a15", "D01Z02S02", 1), // CI
            new PersistenceState("fd9a8f43-cb35-4b4e-bc7b-27af46e5ced4", "D01Z02S04", 1),
            new PersistenceState("509072a9-0d58-4bcb-a025-3dadad6325e4", "D01Z02S05", 1),
            new PersistenceState("b389b291-1fb6-4b26-8b48-10bda40ca9d0", "D01Z02S03", 3), // CR

            // Wastelands
            new PersistenceState("4693a90e-fc42-4e0b-97b6-fa0f099fba03", "D01Z03S02", 1), // CI
            new PersistenceState("08b24276-55e8-414c-9ca7-55490a87fcc0", "D01Z03S01", 1),
            new PersistenceState("75914932-821e-402a-88ec-1209458f3f4b", "D01Z03S03", 1),
            new PersistenceState("d5d6022e-7bb2-44f9-bd7e-950d4044d8a1", "D01Z03S07", 1),
            new PersistenceState("cad13f65-1ca8-48df-ada0-e146fd4d25c7", "D01Z03S05", 1),
            new PersistenceState("06492b0b-12cc-4015-9227-a784b8fd1495", "D01Z03S03", 3), // CR
            new PersistenceState("85357602-4c8f-4dfa-8799-51a6e81a657b", "D01Z03S07", 3),
            new PersistenceState("6f7ed359-8963-451b-a503-cf60be7d7cbf", "D01Z03S02", 5), // GT
            new PersistenceState("bbaf880a-88d1-40d1-a1e5-e6b558be5b1c", "D01Z03S02", 7), // ST

            // Mercy Dreams
            new PersistenceState("0c160f00-3901-4bf2-b67f-99c2830819cf", "D01Z04S03", 0), // PD
            new PersistenceState("01f3f021-eecd-45fd-834e-6e8871857a54", "D01Z04S12", 0),
            new PersistenceState("e882ccea-f807-4c7a-a7ee-49e7264a9395", "D01Z04S05", 1), // CI
            new PersistenceState("6983b8ee-3bec-4bbd-9d3e-e8011218de5f", "D01Z04S06", 1),
            new PersistenceState("994d60f7-545b-4986-ba6b-546db9483c5f", "D01Z04S08", 1),
            new PersistenceState("b6c775dc-e630-4521-b87f-d6a717bb4695", "D01Z04S14", 1),
            new PersistenceState("83d661e0-0523-4dee-8302-aab6da79b6ee", "D01Z04S11", 1),
            new PersistenceState("a26f6067-cf09-4019-80ed-2741cb89de4e", "D01Z04S07", 2), // CH
            new PersistenceState("5cd7ab38-cfd1-43e4-a2c4-337bae10ee5d", "D01Z04S06", 3), // CR
            new PersistenceState("1cbf1da0-c0b9-4745-bde0-77d41a27463b", "D01Z04S01", 5), // GT
            new PersistenceState("41efe0c7-489f-4892-8857-202a7377807d", "D01Z04S15", 5),
            new PersistenceState("2c7933eb-d922-4f5b-b2ec-ca6e6b4707bd", "D01Z04S01", 7), // ST
            new PersistenceState("327ad0d9-79f8-426d-ad98-20371f0e2b79", "D01Z04S15", 7),

            // Desecrated Cistern
            new PersistenceState("8daf15d5-f87e-48c5-977f-ee5e8a012233", "D01Z05S19", 0), // PD
            new PersistenceState("290fe407-ed2b-4299-be73-f15bb79a18d8", "D01Z05S15", 1), // CI
            new PersistenceState("b94e8f00-74c9-42c8-8fcf-cba780269b36", "D01Z05S05", 1),
            new PersistenceState("00ff856c-f1ce-4dc1-8c1d-fe8f1fc3bda8", "D01Z05S05", 1),
            new PersistenceState("7f7de558-3e8b-415e-a099-51dd2e95435b", "D01Z05S08", 1),
            new PersistenceState("ae6ec466-5f49-4406-9090-af5cc3e053ab", "D01Z05S17", 1),
            new PersistenceState("aa834763-1d11-4ea9-a9ca-6892b8e36bdc", "D01BZ05S01", 1),
            new PersistenceState("f9a83a8a-700d-4d2e-9870-d2a2d0577e02", "D01Z05S25", 1),
            new PersistenceState("8ec65451-7b3c-4854-be2b-cb87b7dc0e88", "D01Z05S11", 2), // CH
            new PersistenceState("cbb342e7-3c07-4226-abc2-d8e75b9c4ce8", "D01Z05S06", 2),
            new PersistenceState("e841be55-228d-4766-80a3-ac2540cb5f55", "D01Z05S14", 3), // CR
            new PersistenceState("b7e6b472-3151-4a7a-a7ff-507fe3105141", "D01Z05S06", 3),
            new PersistenceState("7650b52f-4a21-479e-a416-d12ab6c60c3d", "D01Z05S08", 3),
            new PersistenceState("862ba152-43d4-4373-978f-b203fed0c737", "D01Z05S13", 3),
            new PersistenceState("fbacbeb4-4ec8-4c5c-aa33-2e78a0543aad", "D01Z05S25", 3),
            new PersistenceState("9bf20c23-7ae6-4ce4-be36-7da66bcc942d", "D01Z05S20", 3),
            new PersistenceState("497ac572-6d89-49c1-a39c-833b5938b114", "D01Z05S12", 4), // LV
            new PersistenceState("340cde3b-c372-4edd-99db-82e5d46410f5", "D01Z05S02", 4),
            new PersistenceState("17328a8d-02ff-4b1b-b192-1e0d79744b67", "D01Z05S08", 4),
            new PersistenceState("d9904d9e-5605-42a2-9d95-bf30073dd421", "D01Z05S13", 4),
            new PersistenceState("a49afb93-6ecb-4ac6-8600-f55f35b093c3", "D01Z05S20", 4),
            new PersistenceState("3092311f-ccc9-404f-92a5-fbbd43eaa1b1", "D01Z05S12", 5), // GT
            new PersistenceState("e41733cf-c553-4d9f-8b3a-47799be40a15", "D01Z05S13", 5),
            new PersistenceState("ff5594f9-4206-4ede-9f6c-fb3f9666fa15", "D01Z05S13", 5),
            new PersistenceState("bb43b53c-3ee7-464e-9130-2cc318debec4", "D01Z05S02", 5),
            new PersistenceState("57b57842-5b31-4d8f-99f3-b8ace711aa70", "D01Z05S08", 5),
            new PersistenceState("682b61ba-cd0b-4752-a45f-273a89a63701", "D01Z05S25", 5),
            new PersistenceState("47c07521-184d-46d5-9086-7ab68a939f9f", "D01Z05S25", 7), // ST

            // Mountaintops
            new PersistenceState("b28e1e56-7a43-4b3d-8571-e319ecd6afff", "D03Z01S02", 0), // PD
            new PersistenceState("9bce295c-5ab2-4bad-bd01-48c4b8c41ddd", "D03Z01S05", 0),
            new PersistenceState("184f2d41-e6dc-48a7-9a0d-fd31f81e1196", "D03Z01S01", 1), // CI
            new PersistenceState("ff8ff2a9-b4b2-48c9-a7a5-ea0b08d6c1eb", "D03Z01S03", 1),
            new PersistenceState("e4a343fd-16b1-4a0f-9a19-8a6e7a138a68", "D03Z01S04", 1),
            new PersistenceState("aec60464-8558-43bb-a23a-efebc104a2e0", "D03Z01S03", 3), // CR
            new PersistenceState("2b3ad147-ab35-46fe-a3dc-f11b7f44117d", "D03Z01S03", 4), // LV

            // Jondo
            new PersistenceState("c2b08e13-dbb5-478f-985e-5f9657eedd46", "D03Z02S02", 0), // PD
            new PersistenceState("af613abd-4bdf-463c-8fd6-1474e96672dc", "D03Z02S01", 1), // CI
            new PersistenceState("4c613f66-dd96-4da7-afaf-f52b92527980", "D03Z02S04", 1),
            new PersistenceState("90071382-6837-43d4-af42-dcd1c02a3ddf", "D03Z02S06", 1),
            new PersistenceState("4429d84d-6db2-48ad-a203-35bcb556599a", "D03Z02S11", 1),
            new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D03Z02S15", 1),
            new PersistenceState("3d2700bc-a988-498c-ac67-7506cf125cd7", "D03Z02S07", 1),
            new PersistenceState("bc323483-9713-4464-b026-3d54a1fa21f6", "D03Z02S08", 1),
            new PersistenceState("050edbc8-6489-4f05-a307-6f2b94a2b6b6", "D03Z02S13", 1),
            new PersistenceState("7455514c-c4ae-4429-97cf-a4d8c8c74770", "D03Z02S01", 2), // CH
            new PersistenceState("068bf48f-48e7-4c15-8e8c-7719368b2bbd", "D03Z02S12", 2),
            new PersistenceState("94810715-042e-43d3-a22e-7497033d3594", "D03Z02S05", 3), // CR
            new PersistenceState("142ebf04-0993-4dea-ab37-f63e30e791c4", "D03Z02S11", 3),
            new PersistenceState("817c8120-68a2-4a83-9c94-cb725da8794d", "D03Z02S10", 3),
            new PersistenceState("6c91560b-1f58-4de0-a037-ad9f81b7f921", "D03Z02S01", 4), // LV
            new PersistenceState("cf89f21c-6d3d-4a3a-9c53-69b9709a2882", "D03Z02S02", 4),
            new PersistenceState("168cb04f-b812-4426-98e5-0daedb36da82", "D03Z02S03", 4),
            new PersistenceState("291ad3e5-693c-4dd8-a0fa-87890aefb456", "D03Z02S11", 4),
            new PersistenceState("2d0ec12e-b183-49ac-bc26-a4c9706df99e", "D03Z02S11", 4),
            new PersistenceState("7e52698a-57bb-40c1-868a-c65271eb996c", "D03Z02S08", 4),
            new PersistenceState("11ffaece-ed38-4850-9702-15a2fcda3dc4", "D03Z02S02", 4),
            new PersistenceState("52edfe26-7a19-4c1e-a202-71e8188b3dbc", "D03Z02S02", 5), // GT
            new PersistenceState("f6050549-8c17-4654-94f4-69f7078c8429", "D03Z02S02", 6), // PF
            new PersistenceState("9e65eb47-786a-4339-a906-f885e5c984fb", "D03Z02S05", 6),
            new PersistenceState("f3013bb9-7450-4bb4-9840-f0f4ae4824ce", "D03Z02S03", 6),
            new PersistenceState("f28481cf-502c-404a-a157-b156c77a3f2b", "D03Z02S01", 6),
            new PersistenceState("bba9f987-f287-45e4-a885-e1f32d780e49", "D03Z02S01", 6),
            new PersistenceState("425e822f-21f0-4d79-af02-92ac3093d856", "D03Z02S11", 6),
            new PersistenceState("06c809e2-cf31-4566-9427-ef28050d3557", "D03Z02S11", 6),
            new PersistenceState("9568cebf-5111-4d36-9255-b6452990e3a5", "D03Z02S11", 6),
            new PersistenceState("3f39e3a4-8d4b-48ec-a067-c255f82b4e86", "D03Z02S11", 6),
            new PersistenceState("c42d75b4-930c-4ffd-aac8-4c0df9366357", "D03Z02S11", 6),
            new PersistenceState("87924c20-3624-4ce6-9179-0d9031b6821b", "D03Z02S02", 7), // ST

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
