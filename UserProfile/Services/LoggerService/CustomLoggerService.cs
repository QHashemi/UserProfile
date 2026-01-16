using System.Text.Json;
using System.Threading.Tasks;
using UserProfile.Dto.Response;
using UserProfile.Services.LoggerService;

public class CustomLoggerService : ICustomLoggerService

{
    private readonly IWebHostEnvironment _env;

    public CustomLoggerService(IWebHostEnvironment env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task Trace(string message = "", string userIdentifier ="", string logEvent ="", int statusCode=0) => await Log("TRACE", message, userIdentifier, logEvent, statusCode);

    public async Task Debug(string message = "" , string userIdentifier = "", string logEvent = "", int statusCode = 0) => await Log("DEBUG", message, userIdentifier, logEvent, statusCode);

    public async Task Info(string message = "", string userIdentifier = "", string logEvent = "", int statusCode = 0) => await Log("INFO", message, userIdentifier, logEvent ,statusCode);

    public async Task Warning(string message = "", string userIdentifier = "", string logEvent = "", int statusCode = 0) => await Log("WARN", message, userIdentifier, logEvent,statusCode);

    public async Task Error(string message = "",string userIdentifier = "", string logEvent = "", int statusCode = 0) => await Log("ERROR", message,userIdentifier, logEvent, statusCode);

    public async Task Critical(string message = "", string userIdentifier = "", string logEvent = "", int statusCode = 0) => await Log("CRITICAL", message,userIdentifier,logEvent, statusCode);

    // Process Logging
    private async Task Log(string level = "", string message = "", string userIdentifier ="", string logEvent="", int statusCode =0)
    {
        try
        {
            // get Log type
            switch (level)
            {
                case "TRACE":
                    await WriteToFileAsync(level, message, userIdentifier, logEvent, statusCode);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "DEBUG":
                    await WriteToFileAsync(level, message, userIdentifier, logEvent, statusCode);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "INFO":
                    await WriteToFileAsync(level, message, userIdentifier, logEvent, statusCode);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "WARN":
                    await WriteToFileAsync(level, message, userIdentifier, logEvent, statusCode);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "CRITICAL":
                    await WriteToFileAsync(level, message, userIdentifier, logEvent, statusCode);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "ERROR":
                    await WriteToFileAsync(level, message, userIdentifier, logEvent, statusCode);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;

                default:
                    break;
            }

        }
        catch (Exception error) { 
        
            Console.Write(error.ToString());
        }
    }


    // Function to create log file and save it inside the files
    private async Task WriteToFileAsync(string level = "", string message = "", string userIdentifier ="", string logEvent="", int statusCode=0)
    {
        try
        {
            // Format the Log file:
            var loggingText = new LoggingResponseDto
            {
                Level = level,
                Message = message,
                UserIdentifier = userIdentifier,
                LogEvent = logEvent,
                StatusCode = statusCode,
                TimeStamp = $"[{DateTime.UtcNow}]",
            };

            // get Log Path
            var logPath = Path.Combine(_env.ContentRootPath, "Logs");
            // if path not exits create one
            Directory.CreateDirectory(logPath);

            var jsonText = JsonSerializer.Serialize(loggingText);
            var logFilePath = Path.Combine(logPath, $"{level}.log");
            await File.AppendAllTextAsync(logFilePath, jsonText + Environment.NewLine);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to write log: {exception.Message}");
        }

    }

}
