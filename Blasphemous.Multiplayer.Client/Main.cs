using BepInEx;
using Blasphemous.Multiplayer.Client.Network;
using Blasphemous.Multiplayer.Client.Ping;

namespace Blasphemous.Multiplayer.Client;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "3.0.0")]
[BepInDependency("Blasphemous.CheatConsole", "1.1.0")]
internal class Main : BaseUnityPlugin
{
    public static Multiplayer Multiplayer { get; private set; }
    public static Main Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        Multiplayer = new Multiplayer();

        gameObject.AddComponent<ConnectionDisplay>();
        gameObject.AddComponent<PlayerDisplay>();
    }
}
