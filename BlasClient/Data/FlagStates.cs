
namespace BlasClient.Data
{
    public static class FlagStates
    {
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

        private static FlagState[] flags = new FlagState[]
        {
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
            new FlagState("BROTHERS_EVENTPERPETVA_COMPLETED", null), // Perpetva
            new FlagState("BROTHERS_PERPETUA_DEFEATED", "has defeated Perpetva"),

            new FlagState("GEMINO_QI57_OWNED", null), // Gemino
            new FlagState("GEMINO_CAVE", null),
            new FlagState("GEMINO_ENTRANCE_OPEN", null),
            new FlagState("GEMINO_RB10_REWARD", null),
            new FlagState("GEMINO_OFFERING_DONE", null),

            new FlagState("REDENTO_*", null), // Redento

            new FlagState("PENITENT_MET_CLEOFAS", null), // Cleofas
            new FlagState("PENITENT_MET_LVDOVICO", null),
            new FlagState("SOCORRO_*", null),
            new FlagState("CLEOFAS_*", null),

            new FlagState("ALTASGRACIAS_FIRST_OFFERING", null), // Altasgracias
            new FlagState("ALTASGRACIAS_SECOND_OFFERING", null),
            new FlagState("ALTASGRACIAS_EGG_BROKEN", null),
            new FlagState("ALTASGRACIAS_LAST_REWARD", null),
            new FlagState("ALTASGRACIAS_ALTAR*", null),

            new FlagState("LOTL_1OFFERING_DONE", null), // LOTL
            new FlagState("LOTL_2OFFERING_DONE", null),
            new FlagState("LOTL_3OFFERING_DONE", null),

            new FlagState("TIRSO_FIRSTREWARD_DONE", null), // Tirso
            new FlagState("TIRSO_LASTREWARD_DONE", null),
            new FlagState("TIRSO_QI*", "has delivered an herb to Tirso"),
            new FlagState("TIRSO_KISSER*", null),
            new FlagState("TIRSO_TIRSO_DEAD", null),

            new FlagState("LVDOVICO_FIRST_OFFER_DONE", null), // Lvdovico
            new FlagState("LVDOVICO_SECOND_OFFER_DONE", null),
            new FlagState("LVDOVICO_QUEST_DONE", null),
            new FlagState("LVDOVICO_CLEOFAS_DONE", null),

            new FlagState("MIRIAM_CHALLENGE_1", "has completed Miriam challenge 1"), // Miriam
            new FlagState("MIRIAM_CHALLENGE_2", "has completed Miriam challenge 2"),
            new FlagState("MIRIAM_CHALLENGE_3", "has completed Miriam challenge 3"),
            new FlagState("MIRIAM_CHALLENGE_4", "has completed Miriam challenge 4"),
            new FlagState("MIRIAM_CHALLENGE_5", "has completed Miriam challenge 5"),
            // Must sync actual quest status

            new FlagState("SERENO_DLC2QUEST_FINISHED", null), // Diosdado
            new FlagState("SERENO_DOOR_REVEALED", null),

            new FlagState("PERPETUA_EVENT_FINISHED", null), // Crisanta
            new FlagState("ESDRAS_CHAPEL", null),
            new FlagState("ESDRAS_KEY_REWARD", null),
            new FlagState("ABNEGATION_LEFT_EYE", null),
            new FlagState("ABNEGATION_RIGHT_EYE", null),
            new FlagState("ABNEGATION_EYES_RETRIEVED", null),
            new FlagState("CRISANTA_LIBERATED", null),

            new FlagState("SANTOS_DOOR_OPENED", "has opened Jibrael's door"), // Amanecidas
            new FlagState("SANTOS_AMANECIDA_LOCATION1_ACTIVATED", "has activated an Amanecida"),
            new FlagState("SANTOS_AMANECIDA_LOCATION2_ACTIVATED", "has activated an Amanecida"),
            new FlagState("SANTOS_AMANECIDA_LOCATION3_ACTIVATED", "has activated an Amanecida"),
            new FlagState("SANTOS_AMANECIDA_LOCATION4_ACTIVATED", "has activated an Amanecida"),
            new FlagState("SANTOS_GATE_OPENED", null),
            new FlagState("SANTOS_LAUDES_ACTIVATED", null),

            new FlagState("NACIMIENTO_*", null), // Nacimiento

            new FlagState("QI11_BOUGHT_AT_CANDELARIA", null), // Candelaria
            new FlagState("QI49_BOUGHT_AT_CANDELARIA", null),
            new FlagState("QI58_BOUGHT_AT_CANDELARIA", null),
            new FlagState("QI71_BOUGHT_AT_CANDELARIA", null),
            new FlagState("RB02_BOUGHT_AT_CANDELARIA", null),
            new FlagState("RB05_BOUGHT_AT_CANDELARIA", null),
            new FlagState("RB09_BOUGHT_AT_CANDELARIA", null),
            new FlagState("RB12_BOUGHT_AT_CANDELARIA", null),
            new FlagState("RB37_BOUGHT_AT_CANDELARIA", null),

            // Undertaker
            new FlagState("LEFT_TOP_OSSUARY_FLAG", null),
            new FlagState("LEFT_MIDDLE_OSSUARY_FLAG", null),
            new FlagState("LEFT_BOTTOM_1_OSSUARY_FLAG", null),
            new FlagState("LEFT_BOTTOM_2_OSSUARY_FLAG", null),
            new FlagState("MIDDLE_TOP_LEFT_OSSUARY_FLAG", null),
            new FlagState("MIDDLE_TOP_RIGHT_OSSUARY_FLAG", null),
            new FlagState("MIDDLE_BOTTOM_LEFT_OSSUARY_FLAG", null),
            new FlagState("MIDDLE_BOTTOM_RIGHT_OSSUARY_FLAG", null),
            new FlagState("RIGHT_TOP_OSSUARY_FLAG", null),
            new FlagState("RIGHT_MIDDLE_OSSUARY_FLAG", null),
            new FlagState("RIGHT_BOTTOM_1_OSSUARY_FLAG", null),
            new FlagState("RIGHT_BOTTOM_2_OSSUARY_FLAG", null),
            new FlagState("ISIDORA_SINGING", null),

            // Item usage
            new FlagState("QI11_USED", null),
            new FlagState("QI41_USED", null),
            new FlagState("QI45_USED", null),
            new FlagState("QI46_USED", null),
            new FlagState("QI47_USED", null),
            new FlagState("QI48_USED", null),
            new FlagState("QI49_USED", null),
            new FlagState("QI50_USED", null),
            new FlagState("QI51_USED", null),

            // World states
            new FlagState("RESCUED_CHERUB_*", "has freed a Child of Moonlight"),
            new FlagState("TELEPORT_D*", null),
            new FlagState("CLAIMED_*", null),
            new FlagState("CONFESSOR_*", null),
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

            new FlagState("D01Z02S07_TELEPORT_ALBERO", null), // Albero
            new FlagState("D01Z02S02_SECRETWALL", null),
            new FlagState("D01Z02S03_ELEVATOR", null),
            new FlagState("D01BZ06S01_DOOR", null),

            new FlagState("D01Z03S01_TREEPATH", null), // Wastelands

            new FlagState("D01Z05S12_SHORTCUTGATESEWERS", null), // Cistern
            new FlagState("D01Z05S21_PUZZLESOLVED", null),
            new FlagState("D01Z05S02_ELEVATORPATHOPENED", null),
            new FlagState("D01Z05S23_CHALICEPUZZLE", null),
            new FlagState("D01Z05S24_SHORTCUTGATESEWERS", null),

            new FlagState("D02Z01S01_CAVEEXIT", null), // Olive Trees

            new FlagState("D02Z05S01_ELEVATOR1_ENABLED", null), // Graveyard

            new FlagState("D04Z02S15_BLOODGATE", null), // Convent
            new FlagState("D02Z03S02_ARCHDEACONROOM", null),
            new FlagState("D05Z01S15_ARCHDEACON2VISITED", null),
            new FlagState("D05Z01S15_ARCHDEACON2ITEMTAKEN", null),

            //new FlagState("D03Z01S02_BELLCARRIER", null), // Mountains (Not until breakable wall also)

            new FlagState("BELL_PUZZLE1_ACTIVATED", "has broke the eastern bell"), // Jondo
            new FlagState("BELL_PUZZLE2_ACTIVATED", "has broke the western bell"),
            new FlagState("BELL_ACTIVATED", "has activated the bell"),
            new FlagState("D03Z02S13_CHALLENGEOVER", null),

            new FlagState("D03Z03S12_GHOSTARENA", null), // Grievance Ascends
            new FlagState("D03Z03S03_GHOSTKNIGHT", null),
            new FlagState("D03Z03S05_MIXEDCOMBATROUND2_TOP", null),
            new FlagState("D03Z04S01_GATEISOPEN", null),

            new FlagState("D08Z01S02_FACE_BROKEN", null), // Bridge

            new FlagState("D04Z01S03_SECRETPASSAGE", null), // Patio

            new FlagState("D04Z02S06_LADDERUNFOLDED", null), // Mothers
            new FlagState("D04Z02S17_ARCHDEACON1VISITED", null),
            new FlagState("D04Z02S17_ARCHDEACON1ITEMTAKEN", null),
            new FlagState("D04Z04S01_DREAMVISITED", null),

            new FlagState("D02Z03S19_ARCHDEACON3VISITED", null), // Library
            new FlagState("D02Z03S19_ARCHDEACON3ITEMTAKEN", null),
            new FlagState("D05Z01S09_RIDDLESOLVED", null),

            new FlagState("D05Z02S09_RIDDLESOLVED", null), // Canvases
            new FlagState("D05Z02S06_GATESTATE", null),

            new FlagState("D06Z01S03_COMBATCOMPLETED", null), // Rooftops
            new FlagState("D06Z01S20_COMBATCOMPLETED", null),
            new FlagState("D06Z01S06A_COMBATCOMPLETED", null),
            new FlagState("D06Z01S06B_COMBATCOMPLETED", null),
            new FlagState("D06Z01S21_COMBATCOMPLETED", null),
            new FlagState("D06Z01S23_LADDERUNFOLDED", null),
            new FlagState("ELEVATOR_POSITION_2_UNLOCKED", null),
            new FlagState("ELEVATOR_POSITION_3_UNLOCKED", null),
            new FlagState("ELEVATOR_FULL_UNLOCKED", null),

            new FlagState("D09Z01S01_BROSDEAD", null), // Wall
            new FlagState("D09Z01S02_GATERIDDLE", null),
            new FlagState("D09Z01S08_AMBUSHOVER", null),
            new FlagState("D09Z01S08_GROUNDDESTROYED", null),
            new FlagState("D09Z01S08_WALLDESTROYED", null),
            new FlagState("D09Z01S10_ELEVATORUSED", null),

            new FlagState("D20Z01S11_PERPETVAWALL", null), // Echoes

            // Randomizer
            new FlagState("LOCATION_*", null),
            new FlagState("ITEM_*", null),
            new FlagState("OSSUARY_REWARD_*", null),
            new FlagState("RMIRIAM_*", null)
        };
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
}
