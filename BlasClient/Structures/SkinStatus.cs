namespace BlasClient.Structures
{
    public class SkinStatus
    {
        // Name of the skin, is updated whenever receiving a skin update packet
        public string skinName;
        // Determines whether to set player objects skin texture in an update cycle
        // 0 - Already updated, do nothing
        // 1 - When object is first created
        // 2 - First update cycle
        public byte updateStatus;

        public SkinStatus(string skinName)
        {
            this.skinName = skinName;
        }
    }
}
