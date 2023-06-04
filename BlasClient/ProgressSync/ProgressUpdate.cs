
namespace BlasClient.ProgressSync
{
    public class ProgressUpdate
    {
        private readonly string id;
        public string Id => id;

        private readonly ProgressType type;
        public ProgressType Type => type;

        private readonly byte value;
        public byte Value => value;

        public ProgressUpdate(string id, ProgressType type, byte value)
        {
            this.id = id;
            this.type = type;
            this.value = value;
        }
    }
}
