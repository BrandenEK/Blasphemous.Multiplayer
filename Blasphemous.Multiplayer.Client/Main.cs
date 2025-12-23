using BepInEx;

namespace Blasphemous.Multiplayer.Client;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.4.1")]
[BepInDependency("Blasphemous.CheatConsole", "1.0.1")]
internal class Main : BaseUnityPlugin
{
    public static Multiplayer Multiplayer { get; private set; }
    public static Main Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        Multiplayer = new Multiplayer();

        gameObject.AddComponent<MultiplayerGUI>();
    }
}
