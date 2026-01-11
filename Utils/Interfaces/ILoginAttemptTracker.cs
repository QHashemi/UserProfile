namespace UserProfile.Utils.Interfaces
{
    public interface ILoginAttemptTracker
    {
        public bool IsBlocked(string key);
        public void RecordFailure(string key);
        public void Reset(string key);
    }
}
