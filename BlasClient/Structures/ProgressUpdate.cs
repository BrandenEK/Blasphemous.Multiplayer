namespace BlasClient.Structures
{
    public class ProgressUpdate
    {
        public string id;
        public byte type;
        public byte value;

        public ProgressUpdate(string id, byte type, byte value)
        {
            this.id = id;
            this.type = type;
            this.value = value;
        }
    }
}
