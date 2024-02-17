
namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public interface IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress);

        public void SendAllProgress();

        public string GetProgressNotification(ProgressUpdate progress);

        // Additional functionality to obtain progress through gameplay
    }
}
