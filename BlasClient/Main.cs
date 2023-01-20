using BepInEx;
using HarmonyLib;

namespace BlasClient
{
    [BepInPlugin("com.damocles.blasphemous.multiplayer", "Blasphemous Multiplayer", "1.0.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Multiplayer Multiplayer;
        private static Main instance;

        private void Awake()
        {
            Multiplayer = new Multiplayer();
            instance = this;
            Patch();
        }

        private void LateUpdate()
        {
            Multiplayer.update();
        }

        private void Patch()
        {
            Harmony harmony = new Harmony("com.damocles.blasphemous.multiplayer");
            harmony.PatchAll();
        }

        private void Log(string message)
        {
            Logger.LogMessage(message);
        }

        public static void UnityLog(string message)
        {
            instance.Log(message);
        }
    }
}
