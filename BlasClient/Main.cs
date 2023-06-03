﻿using BepInEx;

namespace BlasClient
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.3.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Multiplayer Multiplayer { get; private set; }
        public static Main Instance { get; private set; }

        private void Start()
        {
            Multiplayer = new Multiplayer(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION);
            if (Instance == null)
                Instance = this;
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
