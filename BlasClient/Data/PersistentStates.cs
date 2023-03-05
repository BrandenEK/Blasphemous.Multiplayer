using System.Collections.Generic;

namespace BlasClient.Data
{
    public static class PersistentStates
    {
        private static Dictionary<string, List<string>> persistentObjectsByScene;

        // Create the dictionary of persistent objects grouped by scene
        public static void loadPersistentObjects()
        {
            persistentObjectsByScene = new Dictionary<string, List<string>>();
            foreach (PersistenceState state in getObjectsArray())
            {
                if (!persistentObjectsByScene.ContainsKey(state.scene))
                {
                    // Create new list for this scene
                    persistentObjectsByScene.Add(state.scene, new List<string>());
                }
                persistentObjectsByScene[state.scene].Add(state.id);
            }
        }

        public static int getObjectSceneIndex(string scene, string persistentId)
        {
            if (persistentObjectsByScene.ContainsKey(scene))
            {
                for (int i = 0; i < persistentObjectsByScene[scene].Count; i++)
                {
                    if (persistentObjectsByScene[scene][i] == persistentId)
                        return i;
                }
            }
            return -1;
        }

        public static string getObjectPersistentId(string scene, int sceneIdx)
        {
            if (persistentObjectsByScene.ContainsKey(scene) && persistentObjectsByScene[scene].Count > sceneIdx)
            {
                return persistentObjectsByScene[scene][sceneIdx];
            }
            return null;
        }

        private static PersistenceState[] getObjectsArray()
        {
            return new PersistenceState[]
            {
                // Brotherhood
                new PersistenceState("47db0007-4175-4379-8b5c-f37441f6315b", "D17Z01S05", 0), // PD
                new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D17Z01S13", 0),
                new PersistenceState("0f11c3cc-867e-47c3-a97c-983f2ef0b9ac", "D17BZ02S01", 1), // CI
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D17Z01S14", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D17Z01S01", 1),
                new PersistenceState("635603b6-07d5-471d-a49f-51c558d89362", "D17BZ01S01", 2), // CH
                new PersistenceState("d430347e-1b33-4a35-82fb-ac7d2e1b4a01", "D17Z01S01", 3), // CR
                new PersistenceState("5add17e9-f2d9-416e-a0e9-925d32ac4798", "D17Z01S03", 5), // GT
                new PersistenceState("0cca4185-35ae-4b75-b208-627f92ccfe29", "D17Z01S03", 7), // ST
                new PersistenceState("48fcac75-eb53-49a0-89c1-30d993e93f25", "D17Z01S04", 7),
                new PersistenceState("a7b15297-fb7a-42bc-a7f1-30294e246308", "D17Z01S04", 8), // BW
                new PersistenceState("5a77ada7-bb7c-43f7-8d37-e6fd6f50d663", "D17Z01S04", 9), // LD
                new PersistenceState("7dd8b178-e2d5-4f3c-8a38-ce1312acd3fe", "D17Z01S04", 10), // DR
                new PersistenceState("ef833003-56b4-4f34-9530-16c88e6a4d66", "D17Z01S03", 10),

                // Holy Line
                new PersistenceState("3c6d3c9c-44c2-41cd-9c11-7c9f923680b9", "D01Z01S07", 0), // PD
                new PersistenceState("5710974d-771c-4907-8f1b-b318516ec0db", "D01Z01S02", 1), // CI
                new PersistenceState("9082d3bd-0568-4908-9e5f-00044182cbc4", "D01Z01S02", 1),
                new PersistenceState("e21c696e-61f5-4ef2-808a-cd0ccf852c0c", "D01Z01S03", 1),
                new PersistenceState("5082226e-fcc9-4d90-bed0-9f6d4775fd75", "D01Z01S03", 2), // CH
                new PersistenceState("c0cc4135-d795-48bd-879e-a30c6d80297c", "D01Z01S03", 3), // CR
                new PersistenceState("e4653878-1824-4b4b-b57c-e9685fe0964a", "D01Z01S01", 8), // BW
                new PersistenceState("e4653878-1824-4b4b-b57c-e9685fe0964a", "D01Z01S03", 8),

                // Albero
                new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D01Z02S01", 0), // PD
                new PersistenceState("d402af99-cf62-4c4f-bc37-9a7d6f860a15", "D01Z02S02", 1), // CI
                new PersistenceState("fd9a8f43-cb35-4b4e-bc7b-27af46e5ced4", "D01Z02S04", 1),
                new PersistenceState("509072a9-0d58-4bcb-a025-3dadad6325e4", "D01Z02S05", 1),
                new PersistenceState("a96c43f4-fc8e-4696-ad8d-1a6e8d334571", "D01Z02S07", 1),
                new PersistenceState("57269b9b-f3fd-4f2b-be81-f7b3d72028e4", "D01BZ04S01", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D01BZ04S01", 1),
                new PersistenceState("9a0bde6c-82e8-4b9b-a181-a49f80ef4f44", "D01Z02S05", 1),
                new PersistenceState("b389b291-1fb6-4b26-8b48-10bda40ca9d0", "D01Z02S03", 3), // CR
                new PersistenceState("57c0ac09-88ae-4a22-8e97-5280de0821a5", "D01Z02S07", 5), // GT
                new PersistenceState("16bfa182-d418-411e-a5dd-7c72e11732cf", "D01Z02S07", 7), // ST
                new PersistenceState("2ebf631a-24ab-4dcd-aafd-136d78d433eb", "D01Z02S02", 8), // BW

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
                new PersistenceState("b466739d-af3c-47ec-ae80-f9336b08479b", "D01Z03S02", 8), // BW
                new PersistenceState("a1ea5a05-b583-4069-a889-41dd37907030", "D01Z03S01", 8),
                new PersistenceState("3cad581c-ffdf-4f26-a6e7-08054d34cc0c", "D01Z03S06", 8),
                new PersistenceState("28421a24-aaba-41aa-9b4f-a1d0a8ef41d4", "D01Z03S06", 8),

                // Mercy Dreams
                new PersistenceState("0c160f00-3901-4bf2-b67f-99c2830819cf", "D01Z04S03", 0), // PD
                new PersistenceState("01f3f021-eecd-45fd-834e-6e8871857a54", "D01Z04S12", 0),
                new PersistenceState("e882ccea-f807-4c7a-a7ee-49e7264a9395", "D01Z04S05", 1), // CI
                new PersistenceState("6983b8ee-3bec-4bbd-9d3e-e8011218de5f", "D01Z04S06", 1),
                new PersistenceState("994d60f7-545b-4986-ba6b-546db9483c5f", "D01Z04S08", 1),
                new PersistenceState("b6c775dc-e630-4521-b87f-d6a717bb4695", "D01Z04S14", 1),
                new PersistenceState("83d661e0-0523-4dee-8302-aab6da79b6ee", "D01Z04S11", 1),
                new PersistenceState("965c7ae5-3149-409a-a9e0-bcf8c9592524", "D01Z04S13", 1),
                new PersistenceState("a26f6067-cf09-4019-80ed-2741cb89de4e", "D01Z04S07", 2), // CH
                new PersistenceState("5cd7ab38-cfd1-43e4-a2c4-337bae10ee5d", "D01Z04S06", 3), // CR
                new PersistenceState("4e8ff3de-e4cf-4d78-8076-2df6baf26f69", "D01Z04S16", 3),
                new PersistenceState("1cbf1da0-c0b9-4745-bde0-77d41a27463b", "D01Z04S01", 5), // GT
                new PersistenceState("41efe0c7-489f-4892-8857-202a7377807d", "D01Z04S15", 5),
                new PersistenceState("1a0f3713-ea34-4595-b408-c2efdaae2a3a", "D01Z04S13", 5),
                new PersistenceState("2c7933eb-d922-4f5b-b2ec-ca6e6b4707bd", "D01Z04S01", 7), // ST
                new PersistenceState("327ad0d9-79f8-426d-ad98-20371f0e2b79", "D01Z04S15", 7),
                new PersistenceState("693246da-1b7c-44ae-8a5b-320cc073b2fe", "D01Z04S13", 7),
                new PersistenceState("0f03dec1-275b-41c7-b8db-0fe5d326ae9c", "D01Z04S05", 8), // BW
                new PersistenceState("2cee7762-f4d0-42c1-9d11-c535e204d879", "D01Z04S15", 8),
                new PersistenceState("95132efb-6d33-497d-b4f3-c9086290e343", "D01Z04S10", 8),

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
                new PersistenceState("b9f2e4b8-ac30-429c-8d14-82b67659ec8a", "D01S05S23", 5),
                new PersistenceState("1a0f3713-ea34-4595-b408-c2efdaae2a3a", "D01Z05S24", 5),
                new PersistenceState("47c07521-184d-46d5-9086-7ab68a939f9f", "D01Z05S25", 7), // ST
                new PersistenceState("16bfa182-d418-411e-a5dd-7c72e11732cf", "D01Z05S24", 7),
                new PersistenceState("f0e957d2-3313-4ae7-ba91-17f651f683d2", "D01Z05S03", 8), // BW

                // Petrous
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D01Z06S01", 1), // CI

                // Olive Trees
                new PersistenceState("d3e2f04e-0d36-4b65-8a84-df8f22818707", "D02Z01S01", 0), // PD
                new PersistenceState("07cbe4db-53be-463d-b314-7396fcf46bcf", "D02Z01S01", 1), // CI
                new PersistenceState("9770a9bd-f3db-4a6a-9858-a23d6c95e44b", "D02Z01S09", 1),
                new PersistenceState("aff230c0-eaa1-4401-b248-baf70eb9ce44", "D02Z01S05", 1),
                new PersistenceState("c2925d95-4355-4ed7-9dfd-00530ced4976", "D02Z01S01", 1),
                new PersistenceState("75fba863-7b32-4dd0-8853-a7726f55b058", "D02Z01S06", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D02Z01S04", 1),
                // Other bead/flower
                new PersistenceState("0495cb35-3807-433b-b82f-23a45692ba83", "D02Z01S02", 3), // CR
                new PersistenceState("465934da-ee64-4b6c-aaf2-78c145185e7e", "D02Z01S06", 3),
                new PersistenceState("25a8f890-f84b-437e-b6e7-88f26f6c8730", "D02Z01S04", 8), // BW
                new PersistenceState("bc87ab04-a4be-4744-b18c-14d85f16222f", "D02Z01S06", 8),

                // Graveyard
                new PersistenceState("5bd1a831-b47e-4303-b898-c3ba367a857a", "D02Z02S08", 0), // PD
                new PersistenceState("ec75430b-dc1f-4ca0-934c-622dbe56cc13", "D02Z02S08", 1), // CI
                new PersistenceState("475f05b8-1ad8-43f8-92f0-a18b17677f5a", "D02Z02S11", 1),
                new PersistenceState("8bd708a2-4eb7-484f-9f64-89cf362e76ed", "D02Z02S06", 1),
                new PersistenceState("611b9d1a-b37a-415d-bce6-e7cadc89c4aa", "D02Z02S03", 1),
                new PersistenceState("f9b2c8a6-6c43-4966-b876-2d93bd0ab095", "D02Z02S03", 1),
                new PersistenceState("309bceef-8798-48a3-ad98-650f613d456f", "D02Z02S03", 1),
                new PersistenceState("9dcb9501-a474-4d41-96c8-a3875ce8a638", "D02Z02S04", 1),
                new PersistenceState("cbde2d24-61c9-458e-8035-643c00a6d376", "D02Z02S04", 1),
                new PersistenceState("c5d58f0c-b905-4278-abd6-92a5a22ec7b9", "D02Z02S05", 1),
                new PersistenceState("39f1e0d7-26de-4937-a197-088fa1555e5b", "D02Z02S13", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D02Z02S14", 1),
                new PersistenceState("b898e95c-abca-4bcc-82e5-8bc43bdb1373", "D02Z02S11", 3), // CR
                new PersistenceState("b3e9cc99-28bb-46a5-be22-6e9896916ebc", "D02Z02S02", 3),
                new PersistenceState("656d21d9-e784-456a-aea7-59a6520bd4f0", "D02Z02S04", 3),
                new PersistenceState("2c866895-9fc8-432a-9f34-b2dfc6089aeb", "D02Z02S08", 3),
                new PersistenceState("5bd00665-3d06-49be-bb29-9ef45c0c4667", "D02Z02S13", 5), // GT
                new PersistenceState("9bed668f-a47e-4ff5-8378-e95be7430961", "D02Z02S08", 8), // BW
                new PersistenceState("25a8f890-f84b-437e-b6e7-88f26f6c8730", "D02Z02S05", 8),

                // Convent
                new PersistenceState("0264028c-cae4-4c7a-9bda-2ef31baf480c", "D02Z03S08", 0), // PD
                new PersistenceState("36b07f19-adba-4523-92dd-6f3f74faa203", "D02Z03S09", 0),
                new PersistenceState("d4c2b23f-f2eb-4eed-a8fc-61a675aead3c", "D02Z03S12", 1), // CI
                new PersistenceState("999d9b4e-f9f5-449e-9f09-d32fb3597f7b", "D02Z03S03", 1),
                new PersistenceState("2348a9a4-0a18-4a9d-85f9-b20c14c0136f", "D02Z03S05", 1),
                new PersistenceState("880c396a-c7a2-481f-8994-9efdca134a85", "D02Z03S07", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D02Z03S23", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D02Z03S17", 1),
                new PersistenceState("b225e61e-432e-48bf-907b-e37fbcfc1f35", "D02Z03S19", 1),
                new PersistenceState("200fb191-1cc7-4039-a16a-912bf58d2f6a", "D02Z03S08", 4), // LV
                new PersistenceState("bf0d9de4-3a2b-49dc-9353-2b4480af0b33", "D02Z03S02", 4),
                new PersistenceState("0c5b1590-1802-40e8-b8ee-7198c83ea6cf", "D02Z03S11", 4),
                new PersistenceState("fe8cc8d5-6402-4ab9-9c7a-0e7e548aa84f", "D02Z03S05", 5), // GT
                new PersistenceState("5270e234-3113-49b4-a055-023c8dedd20f", "D02Z03S05", 7), // ST
                new PersistenceState("51018c78-4120-4255-b205-0d65bcdd33d9", "D02Z03S07", 8), // BW
                new PersistenceState("274ffa9b-6fe0-46db-830d-cfcd77397c3d", "D02Z03S08", 9), // LD
                new PersistenceState("0a073b19-8957-4e01-b681-60f9a25931fe", "D02Z03S02", 9),
                new PersistenceState("75b8be0e-08a6-4d43-b048-1c265a4996ea", "D02Z03S11", 9),
                new PersistenceState("ffc50f60-44a3-40f8-b0c6-9a6b62002101", "D02Z03S11", 9),
                new PersistenceState("e043eb63-2f3f-4dab-9c92-77a9a424e26c", "D02Z03S11", 9),

                // Mountaintops
                new PersistenceState("b28e1e56-7a43-4b3d-8571-e319ecd6afff", "D03Z01S02", 0), // PD
                new PersistenceState("9bce295c-5ab2-4bad-bd01-48c4b8c41ddd", "D03Z01S05", 0),
                new PersistenceState("184f2d41-e6dc-48a7-9a0d-fd31f81e1196", "D03Z01S01", 1), // CI
                new PersistenceState("ff8ff2a9-b4b2-48c9-a7a5-ea0b08d6c1eb", "D03Z01S03", 1),
                new PersistenceState("e4a343fd-16b1-4a0f-9a19-8a6e7a138a68", "D03Z01S04", 1),
                new PersistenceState("aec60464-8558-43bb-a23a-efebc104a2e0", "D03Z01S03", 3), // CR
                new PersistenceState("2b3ad147-ab35-46fe-a3dc-f11b7f44117d", "D03Z01S03", 4), // LV
                new PersistenceState("33ee097c-68e1-438a-b7f5-fd294d32618f", "D03Z01S03", 6), // PF
                new PersistenceState("841829af-b61e-47e0-b7b5-886cf47d26ae", "D03Z01S03", 6),
                new PersistenceState("3a731cd0-f573-471a-8291-e4c420ba37d4", "D03Z01S03", 6),
                new PersistenceState("6909aade-20cc-4384-97bb-8c27bbb9b531", "D03Z01S03", 6),

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
                new PersistenceState("358f1fcb-47de-47f0-a0fb-f864a110a922", "D03Z02S07", 4),
                new PersistenceState("738d2218-61ae-476a-8634-34c8f81ee7d5", "D03Z02S02", 5), // GT
                new PersistenceState("52edfe26-7a19-4c1e-a202-71e8188b3dbc", "D03Z02S02", 5),
                new PersistenceState("4ef67c25-2675-4ce7-93f2-9f704b60abcb", "D03Z02S09", 5),
                new PersistenceState("65492adb-e7bd-44a7-8d0e-7d95132f82c4", "D03Z02S12", 5),
                new PersistenceState("8799a0a6-8a2c-49ef-9e2f-9b5c3eb7ec1e", "D03Z02S06", 5),
                new PersistenceState("1c681fb6-d77f-4093-b841-ffae3e8a7435", "D03Z02S07", 5),
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
                new PersistenceState("2e922626-b8e2-49ee-a1d3-e5e7be1591db", "D03Z02S13", 6),
                new PersistenceState("581d2427-4a1c-4fae-ac45-0bbf5133aded", "D03Z02S08", 6),
                new PersistenceState("7b060721-b726-4b29-8350-28dc7daa8bd9", "D03Z02S02", 6),
                new PersistenceState("6f05d48f-aed2-45f7-936d-f36d6aaae097", "D03Z02S02", 7), // ST
                new PersistenceState("87924c20-3624-4ce6-9179-0d9031b6821b", "D03Z02S02", 7),
                new PersistenceState("7ebf85de-9ee0-4809-88ee-27f4c0199ed4", "D03Z02S09", 7),
                new PersistenceState("b238fb38-7370-4524-b060-c1d39a6eb690", "D03Z02S12", 7),
                new PersistenceState("1130bce0-77e2-4a9b-b09c-d9d762ca1558", "D03Z02S06", 7),

                // Grievance
                new PersistenceState("7d80a4e3-db57-4b63-817c-fac8ce10fb02", "D03Z03S01", 0), // PD
                new PersistenceState("bf3c95db-28c5-4c2a-affe-a8c61b8634d8", "D03Z03S11", 0),
                new PersistenceState("bc75324e-991c-4e75-993e-8c034bd2d6da", "D03Z03S02", 1), // CI
                new PersistenceState("17410aaa-b4cd-420c-be34-37115d5ba5da", "D03Z03S06", 1),
                new PersistenceState("a608e5ed-71a0-48a0-abd0-83f8690d62aa", "D03Z03S08", 1),
                new PersistenceState("b225e61e-432e-48bf-907b-e37fbcfc1f35", "D03Z03S10", 1),
                new PersistenceState("abe4d306-a1ae-4a6b-a87f-24457bbcc701", "D03Z03S06", 2), // CH
                new PersistenceState("61a55d1e-5da3-413d-8d29-5b3e4232fb9e", "D03Z03S06", 3), // CR
                new PersistenceState("06138bc6-5f81-48a2-83ee-a9092a8ef3c0", "D03Z03S08", 3),
                new PersistenceState("a0234103-70b4-4b45-8851-e1a29a2a95d1", "D03Z03S09", 3),
                new PersistenceState("51018c78-4120-4255-b205-0d65bcdd33d9", "D03Z03S07", 8), // BW

                // Bridge
                new PersistenceState("3c851c94-2256-475a-8712-c440298743e3", "D08Z01S01", 0), // PD
                new PersistenceState("69a9b9c9-d04e-4d1a-a0d0-32298106834f", "D08Z01S02", 1), // CI

                // Hall
                new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D08Z03S02", 0), // PD
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D08Z03S01", 1), // CI

                // Patio
                new PersistenceState("6f3600e0-196d-43a6-aa3f-e6265d9335c2", "D04Z01S01", 0), // PD
                new PersistenceState("ff8d5a0e-fc5c-476c-b095-40cbd2a40130", "D04Z01S01", 1), // CI
                new PersistenceState("1591d68c-60ed-468f-bfa7-9ac75806f52c", "D04Z01S02", 1),
                new PersistenceState("f054b407-2cdb-4e7b-b031-f6a2020d8f5e", "D04Z01S03", 1),
                new PersistenceState("a6bfeaa0-a546-4ad9-8d98-b053006f0203", "D04Z01S03", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D04Z01S06", 1),
                new PersistenceState("aa747fc1-eeac-46ab-b95e-c416542c661e", "D04Z01S01", 3), // CR
                new PersistenceState("962fbc9e-e50d-4aad-a283-c10a01dadee5", "D04Z01S03", 3),
                new PersistenceState("7ee1df26-25ab-4a30-9ffa-1274694d163f", "D04Z01S06", 4), // LV
                new PersistenceState("51018c78-4120-4255-b205-0d65bcdd33d9", "D04Z01S03", 8), // BW
                new PersistenceState("1a0f3713-ea34-4595-b408-c2efdaae2a3a", "D04Z01S06", 9), // LD

                // Mother of Mothers
                new PersistenceState("5315f269-3f1d-4586-9d8f-05b56ba153a6", "D04Z02S21", 0), // PD
                new PersistenceState("a0b89188-37a9-4f20-b765-6696ae795a74", "D04Z02S07", 1), // CI
                new PersistenceState("739da3a9-bc7c-469c-8818-33ebfca6a91a", "D04Z02S07", 1),
                new PersistenceState("5c850694-bf0c-4704-b3c1-76307746ae55", "D04Z02S02", 1),
                new PersistenceState("791eeb94-ede6-4b67-a906-7437ffed3f2e", "D04Z02S16", 1),
                new PersistenceState("5a97c5b6-8ad1-4151-99db-bdec0b330a52", "D04Z02S06", 1),
                new PersistenceState("0bb0bba0-506d-4899-8160-cd8a2c4b6548", "D04Z02S11", 1),
                new PersistenceState("b225e61e-432e-48bf-907b-e37fbcfc1f35", "D04Z02S15", 1),
                new PersistenceState("8eddaf71-dd08-41a3-a569-6b1861737feb", "D04BZ02S01", 1),
                new PersistenceState("8a1c64f0-507b-42b5-9eaf-9480932f829f", "D04BZ02S01", 2), // CH
                new PersistenceState("15bafd86-e3ea-4cb5-977c-df7a57e50faf", "D04Z02S01", 3), // CR
                new PersistenceState("4dad058e-0f35-4101-8442-e00c6e614f50", "D04Z02S11", 3),
                new PersistenceState("8dca4577-1a1d-4eae-bdf3-a6d8696dc16f", "D04Z02S07", 4), // LV
                new PersistenceState("b7fa35a1-2725-4238-a05e-e01ce0758d70", "D04Z02S04", 4),
                new PersistenceState("7fdb3ae4-17c7-4d55-9534-daae7d92ff97", "D04Z02S01", 4),
                new PersistenceState("8c0e1da1-a9ba-44eb-a671-e2de892269bc", "D04Z02S06", 4),
                new PersistenceState("d93c45c6-a1f8-4e78-bc70-f5544277519b", "D04Z02S02", 4),
                new PersistenceState("61be0e2c-6a3d-4c00-b37a-59e7bd396929", "D04Z02S04", 8), // BW
                new PersistenceState("e160bc84-afa0-4a13-8352-16eb518098b5", "D04Z02S07", 8),
                new PersistenceState("cb3266c7-220a-4dcf-84a7-b72ecf517fe0", "D04Z02S07", 9), // LD
                new PersistenceState("3a2b8e90-8688-4647-9f80-b47f67b15e1b", "D04Z02S04", 9),
                new PersistenceState("877f73fa-f753-4971-b069-243da3ad574e", "D04Z02S01", 9),
                new PersistenceState("81f82e36-cd57-4d0b-b6f7-cf7fcf3337fa", "D04Z02S06", 9),
                new PersistenceState("564e5041-0177-4222-b136-548b24ab8bbd", "D04Z02S02", 9),

                // Knot of Words
                new PersistenceState("58e4ed6d-5107-453c-8930-96f4eb39f534", "D04Z03S01", 0), // PD

                // Library
                new PersistenceState("3ce1d1ef-9955-4319-9dfe-58af85e5521c", "D05Z01S03", 0), // PD
                new PersistenceState("9f99a219-d571-4624-a5a9-974fe070381a", "D05Z01S20", 0),
                new PersistenceState("7c70e5f2-b811-4948-abfb-3b9765505989", "D05Z01S04", 1), // CI
                new PersistenceState("4dbfc4e6-d979-47ce-b4cf-927fbca74c87", "D05Z01S05", 1),
                new PersistenceState("dac0e740-251f-40b8-962b-4a1aaac12787", "D05Z01S05", 1),
                new PersistenceState("5380262d-3478-401a-a326-665f35ee7e30", "D05Z01S11", 1),
                new PersistenceState("e2f6f817-8168-4683-83d3-d82b7b93307b", "D05Z01S11", 1),
                new PersistenceState("eaff3393-6d9d-49c8-83d6-d0353979778d", "D05Z01S15", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D05BZ01S01", 1),
                new PersistenceState("7a1ab0fd-972f-4d54-b44c-fdce0ce1a1d0", "D05Z01S06", 2), // CH
                new PersistenceState("57b6fe51-00be-4252-9df7-49a256adb07c", "D05Z01S10", 2),
                new PersistenceState("5f451460-a4de-4781-8b16-b9c3fd2bf5e5", "D05Z01S18", 2),
                new PersistenceState("8672dac8-0537-4f1f-b3ab-9cf1619ace4d", "D05Z01S04", 3), // CR
                new PersistenceState("49d91474-4567-4175-a679-8c02a0a12ccd", "D05Z01S11", 3),
                new PersistenceState("19566f8c-a976-4d2d-8ccb-1c6aa94e95e2", "D05Z01S21", 3),
                new PersistenceState("470518ad-5c02-43bb-b913-51c49a73b2fc", "D05Z01S11", 5), // GT
                new PersistenceState("0cf20cb3-27ed-4850-ba7d-df0fa79301b3", "D05Z01S11", 7), // ST
                new PersistenceState("4e6995b7-7f80-43c2-9a2c-dc47c3f4c04c", "D05Z01S05", 8), // BW
                new PersistenceState("51018c78-4120-4255-b205-0d65bcdd33d9", "D05Z01S06", 8),
                new PersistenceState("7fa486a3-e09f-4e2d-9efa-9916a6fa1e39", "D05Z01S08", 8),
                new PersistenceState("fc86b193-f25d-4716-bc1e-9a2608542cc4", "D05Z01S08", 8),
                new PersistenceState("51018c78-4120-4255-b205-0d65bcdd33d9", "D05Z01S11", 8),
                new PersistenceState("682a4e6c-3dd6-4e3b-b87b-6fce5086bae9", "D05Z01S02", 8),
                new PersistenceState("fed99b52-62b7-4c0c-a51b-bf9ec713011d", "D05BZ01S01", 8),

                // Canvases
                new PersistenceState("28da7a78-0612-47b4-b7c4-4bbcabc632b5", "D05Z02S01", 0), // PD
                new PersistenceState("89c1bddb-5b22-4498-8c52-9bb5ef6fabdd", "D05Z02S07", 0),
                new PersistenceState("00bbf955-7b39-4770-a565-e5c8c2cf0823", "D05Z02S02", 1), // CI
                new PersistenceState("b10e3250-2940-48ac-a9bf-9eac2598bf87", "D05Z02S08", 1),
                new PersistenceState("b2cdc579-6bac-4431-a25f-740cb92b46f9", "D05Z02S11", 1),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D05Z02S15", 1),
                new PersistenceState("d609740f-16a0-4a1b-a794-bf22dfc9f0d3", "D05Z02S11", 5), // GT
                new PersistenceState("bedf4186-c007-4b10-9123-54b4b2437561", "D05Z02S11", 7), // ST

                // Rooftops
                new PersistenceState("56b997a9-f708-4e44-80a1-f4cc051dfb30", "D06Z01S06", 0), // PD
                new PersistenceState("a14685d6-1b1d-455c-b597-08881e21afc2", "D06Z01S23", 0),
                new PersistenceState("68efcdb9-3e13-41a5-b7ce-920b478eac81", "D06Z01S09", 0),
                new PersistenceState("c0619612-370d-4d55-b55f-0a2c82af05d3", "D06Z01S22", 1), // CI
                new PersistenceState("30dbe500-8629-4fe0-97e1-36e9b838c818", "D06Z01S15", 1),
                new PersistenceState("2cbb1ea6-4841-4d4d-b913-47ff3530f27f", "D06Z01S12", 1),
                new PersistenceState("91f372e4-1028-4299-919a-b42aceea01f4", "D06Z01S12", 2), // CH
                new PersistenceState("b92a8ae8-3997-4339-906f-81c4fbe6ed16", "D06Z01S12", 3), // CR
                new PersistenceState("7ee1df26-25ab-4a30-9ffa-1274694d163f", "D06Z01S23", 4), // LV
                new PersistenceState("2ac29ac0-4bda-4a14-9364-be3e95d9d24a", "D06Z01S04", 5), // GT
                new PersistenceState("8cca34d9-036f-4280-96bd-9013991eadb3", "D06Z01S15", 5),
                new PersistenceState("1cce891a-cca8-4cbe-aa7a-618fb633066b", "D06Z01S12", 6), // PT
                new PersistenceState("b3bbc9c2-88d8-4873-947b-e08eb36c51b6", "D06Z01S12", 6),
                new PersistenceState("b09391c4-d58a-4c85-b322-16cec63a1637", "D06Z01S08", 6),
                new PersistenceState("017636b1-71b4-4c2c-b73a-13136f9d6211", "D06Z01S04", 7), // ST
                new PersistenceState("d8592ac2-4762-4aa8-b6d9-a1032cc581f3", "D06Z01S15", 7),
                new PersistenceState("79150270-4197-4a3b-af41-d82fd23d9606", "D06Z01S12", 7),
                new PersistenceState("4c8752f6-5cf1-4762-a51d-019eec45f0a0", "D06Z01S12", 7),
                new PersistenceState("34f6ce02-e783-4d0c-af1f-7b8ef46a5c47", "D06Z01S08", 7),
                new PersistenceState("51018c78-4120-4255-b205-0d65bcdd33d9", "D06Z01S17", 8), // BW
                new PersistenceState("9f138023-3ded-414e-abc5-1baf87b82fe1", "D06Z01S23", 9), // LD

                // Wall
                new PersistenceState("ea6075c3-e8aa-465d-a6c7-fa1e613fb775", "D09Z01S11", 0), // PD
                new PersistenceState("1ea60cae-692c-4b90-831c-3cd581ca9162", "D09BZ01S01", 1), // CI
                new PersistenceState("e88bf360-3908-4b2d-8e74-54dce9dc9aa1", "D09BZ01S01", 1),
                new PersistenceState("898ebf47-1ce6-4528-941a-54342e9a45ca", "D09BZ01S01", 1),
                new PersistenceState("dfe967b5-8601-4094-a0de-6586001f3148", "D09Z01S02", 1),
                new PersistenceState("1ad69347-4c28-402c-b1a0-575089e5fa5f", "D09Z01S10", 1),
                new PersistenceState("402c16b9-d157-4f7f-9ab7-7dd4dd4e01a3", "D09BZ01S01", 1),
                new PersistenceState("057bc369-c0a1-4797-abb2-4e49210328a5", "D09Z01S10", 1),
                new PersistenceState("9ef658de-65d3-4075-b8cf-f5c68ecaea0d", "D09BZ01S01", 1),
                new PersistenceState("a768ea2e-7853-4b73-8ca2-0982e70d896f", "D09Z01S09", 1),
                new PersistenceState("8cd5b42a-8e8c-4cf5-9097-e8cc89a8daa8", "D09BZ01S01", 1),
                new PersistenceState("27ac21b5-d35f-40ab-bd59-05c3b2d6cef0", "D09Z01S08", 1),
                new PersistenceState("84665e48-c2d1-46df-af15-c07c3ee50d89", "D09Z01S02", 2), // CH
                new PersistenceState("49fdeae9-f40c-4616-b668-b7f17cb1f01f", "D09BZ01S01", 3), // CR
                new PersistenceState("28c97b46-911f-4185-b741-01bc7b6e27a4", "D09BZ01S01", 3),
                new PersistenceState("0f9e23bf-cdcd-45d1-bf39-6772b407c53f", "D09BZ01S01", 3),
                new PersistenceState("383389c3-735a-4b50-af27-7ec71d9b8447", "D09Z01S06", 3),
                new PersistenceState("b2dd5145-5ce0-4824-98f1-4de886712bbc", "D09Z01S07", 5), // GT
                new PersistenceState("764b219e-5987-41a3-a660-e495e7d6225e", "D09Z01S05", 5),
                new PersistenceState("4f18233a-16fd-4f0b-b829-90c877e909f0", "D09Z01S08", 5),
                new PersistenceState("18f597f8-043a-453f-9d0a-3dd4d94a9560", "D09Z01S11", 5),
                new PersistenceState("debd2dd2-4061-4b29-9ea2-8db07e884f5d", "D09Z01S02", 5),
                new PersistenceState("fedc4ad7-69bc-49b9-9a79-d92faaa1b5a2", "D09Z01S07", 7), // ST
                new PersistenceState("1bd4d663-46c5-4e66-9002-04a710f4ad07", "D09Z01S05", 7),
                new PersistenceState("06fed655-aa37-4c69-a725-cef2c73cb40b", "D09Z01S08", 7),
                new PersistenceState("990d8b4d-75fe-4662-8aa1-466c6c4d0af5", "D09Z01S11", 7),
                new PersistenceState("241be671-7245-4eb4-b170-cd024f1f2747", "D09BZ01S01", 8), // BW
                new PersistenceState("8f7e5d86-e336-4a28-9cd0-3b07b264fa7f", "D09Z01S10", 8),
                new PersistenceState("e13cd24a-7698-4f4c-97ab-e126b6015164", "D09Z01S09", 8),
                new PersistenceState("78710f6c-cdc5-466a-8894-675bb6585088", "D09Z01S02", 10), // DR
                new PersistenceState("47c6f97e-3435-4ffc-bb68-d9b23a0d6fa1", "D09Z01S02", 10),
                new PersistenceState("8fc4f0af-d016-4d05-b137-222183d45231", "D09Z01S02", 10),
                new PersistenceState("d7d4edad-b727-48e1-a05a-98a8b013a4bb", "D09Z01S02", 10),
                new PersistenceState("966de5df-a5e3-4b03-8e34-d761974af73b", "D09Z01S08", 10),
                new PersistenceState("f97d0e4b-d0dc-4875-a70e-b7c878134060", "D09Z01S08", 10),
                new PersistenceState("7b46329a-296f-4865-b192-4118a710d592", "D09Z01S08", 10),
                new PersistenceState("3ee230b2-4765-446d-b40d-c53e7b2fbbb1", "D09Z01S08", 10),
                new PersistenceState("a17a810d-33fd-45a4-80e9-e9273ce8a8fc", "D09Z01S09", 10),
                new PersistenceState("2892c6cf-743a-4317-89aa-66033b35614e", "D09Z01S09", 10),
                new PersistenceState("d020645f-5196-4975-8f09-5f3efd518696", "D09Z01S10", 10),
                new PersistenceState("4e22ca88-f0fd-45ef-a1ae-da1c4f11f7ed", "D09Z01S10", 10),
                new PersistenceState("2ac71d61-d551-4ebf-acea-80a75c357c1b", "D09Z01S10", 10),

                // Echoes of Salt
                new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D20Z01S01", 0), // PD
                new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D20Z01S11", 0),
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D20Z01S03", 1), // CI
                new PersistenceState("98eafe95-57de-4d04-b28e-1219e7ef39e8", "D20Z01S09", 1),
                new PersistenceState("1a0f3713-ea34-4595-b408-c2efdaae2a3a", "D20Z01S06", 5), // GT
                new PersistenceState("693246da-1b7c-44ae-8a5b-320cc073b2fe", "D20Z01S06", 7), // ST
                new PersistenceState("eddbfcd5-d416-4b61-9257-2dc57ee61a9b", "D20Z01S11", 8), // BW

                // Mourning and Havoc
                new PersistenceState("b844bc20-d630-4b45-b301-3d6f63e63a9d", "D20Z02S01", 0), // PD
                new PersistenceState("09bd730e-5cbc-428b-9fe5-c24ceeaa369a", "D20Z02S09", 0),
                new PersistenceState("7455514c-c4ae-4429-97cf-a4d8c8c74770", "D20Z02S02", 2), // CH
                new PersistenceState("bf22d5e0-a884-4686-894e-4c48a50d0af1", "D20Z02S11", 2),
                new PersistenceState("cae6fabb-27de-48e3-b44a-f1fad9c14d3a", "D20Z02S04", 9), // LD
                new PersistenceState("5c3ea9bd-e2f1-4d72-86ce-b4ad02fbd97e", "D20Z02S06", 9),
                new PersistenceState("1a0f3713-ea34-4595-b408-c2efdaae2a3a", "D20Z02S11", 9),
                // Ladder triggers

                // Deambulatory
                new PersistenceState("696e70f3-5cf8-4713-abb5-e5f62d6a49e2", "D07Z01S01", 0) // PD
            };
        }
    }

    [System.Serializable]
    public class PersistenceState
    {
        public string id;
        public string scene;
        public byte type; // Might not need to store type anymore

        public PersistenceState(string id, string scene, byte type)
        {
            this.id = id;
            this.scene = scene;
            this.type = type;
        }
    }
}
