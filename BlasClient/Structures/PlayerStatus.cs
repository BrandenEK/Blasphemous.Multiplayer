
namespace BlasClient.Structures
{
    public class PlayerStatus
    {
        public byte team;
        public SkinStatus skin;

        public string currentScene;
        public string lastMapScene;
        public byte specialAnimation;

        public PlayerStatus()
        {
            currentScene = "";
            lastMapScene = "";
            skin = new SkinStatus("PENITENT_DEFAULT");
        }
    }
}
