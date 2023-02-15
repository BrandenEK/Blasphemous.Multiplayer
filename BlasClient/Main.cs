using BepInEx;
using HarmonyLib;

namespace BlasClient
{
    [BepInPlugin("com.damocles.blasphemous.multiplayer", "Blasphemous Multiplayer", "1.0.0")]
    [BepInDependency("com.damocles.blasphemous.modding-api", BepInDependency.DependencyFlags.HardDependency)]
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

        // Recursive method that returns the entire hierarchy of an object
        public static string displayHierarchy(UnityEngine.Transform transform, string output, int currentLevel, int maxLevel, bool components)
        {
            // Indent
            for (int i = 0; i < currentLevel; i++)
                output += "\t";

            // Add this object
            output += transform.name;

            // Add components
            if (components)
            {
                output += " (";
                foreach (UnityEngine.Component c in transform.GetComponents<UnityEngine.Component>())
                    output += c.ToString() + ", ";
                output = output.Substring(0, output.Length - 2) + ")";
            }
            output += "\n";

            // Add children
            if (currentLevel < maxLevel)
            {
                for (int i = 0; i < transform.childCount; i++)
                    output = displayHierarchy(transform.GetChild(i), output, currentLevel + 1, maxLevel, components);
            }

            // Return output
            return output;
        }
    }
}
