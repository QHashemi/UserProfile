namespace UserProfile.Utils
{
    public interface ICustomLogger
    {
        public Task Trace(string message);
        public Task Debug(string message);
        public Task Info(string message);
        public Task Warning(string message);
        public Task Error(string message, Exception? exception = null);
        public Task Critical(string message, Exception? exception = null);
    }
}
