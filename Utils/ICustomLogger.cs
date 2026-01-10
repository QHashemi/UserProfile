using System.ComponentModel.DataAnnotations;

namespace UserProfile.Utils
{
    public interface ICustomLogger
    {
        public Task Trace(string message = "", string userIdentifier = "" , string logEvent = "", int statusCode=0);
        public Task Debug(string message = "", string userIdentifier = "" , string logEvent = "", int statusCode = 0);
        public Task Info(string message = "", string userIdentifier = "" , string logEvent = "", int statusCode = 0);
        public Task Warning(string message = "", string userIdentifier = "" , string logEvent = "", int statusCode = 0);
        public Task Error(string message = "", string userIdentifier = "" , string logEvent = "", int statusCode = 0);
        public Task Critical(string message = "", string userIdentifier = "" ,string logEvent = "", int statusCode = 0);
    }
}
