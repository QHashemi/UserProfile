using System.Collections.Concurrent;
using UserProfile.Utils.Interfaces;

namespace UserProfile.Utils
{
    public class LoginAttemptTracker : ILoginAttemptTracker
    {
        private static readonly ConcurrentDictionary<string, (int Count, DateTime LastAttempt)> _attempts = new();
        public bool IsBlocked(string key)
        {
            if(!_attempts.TryGetValue( key, out var entry ))
                return false;

            // block for 10 minute after 5 failure
            if (entry.Count >= 5 && DateTime.UtcNow - entry.LastAttempt < TimeSpan.FromSeconds(10))
            {
                return true;
            }
            return false;

        }
        public void RecordFailure(string key)
        {
            _attempts.AddOrUpdate(key, (1, DateTime.UtcNow), (_,existing) => {
                if (DateTime.UtcNow - existing.LastAttempt > TimeSpan.FromSeconds(5))
                {
                    return (1, DateTime.UtcNow); // rest windows
                }

                return (existing.Count + 1, DateTime.UtcNow);
            });
        }

        public void Reset(string key)
        {
            _attempts.TryRemove(key, out _);
        }
    }
}
