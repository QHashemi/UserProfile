using System.Threading.Tasks;
using UserProfile.Utils;

public class CustomLogger : ICustomLogger

{
    private readonly IWebHostEnvironment _env;

    public CustomLogger(IWebHostEnvironment env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task Trace(string message) => await Log("TRACE", message);

    public async Task Debug(string message) => await Log("DEBUG", message);

    public async Task Info(string message) => await Log("INFO", message);

    public async Task Warning(string message) => await Log("WARN", message);

    public async Task Error(string message, Exception? ex = null) => await Log("ERROR", message, ex);

    public async Task Critical(string message, Exception? ex = null) => await Log("CRITICAL", message, ex);

    // Process Logging
    private async Task Log(string level, string message, Exception? ex = null)
    {
        try
        {
            // get Log type
            switch (level)
            {
                case "TRACE":
                    await WriteToFileAsync(level, message);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "DEBUG":
                    await WriteToFileAsync(level, message);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "INFO":
                    await WriteToFileAsync(level, message);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "WARN":
                    await WriteToFileAsync(level, message);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "CRITICAL":
                    await WriteToFileAsync(level, message);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;
                case "ERROR":
                    await WriteToFileAsync(level, message);
                    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    break;

                default:
                    break;
            }

        }
        catch (Exception) { 
        
            await WriteToFileAsync("EXCEPTION", ex.Message);
        }
    }


    // Function to create log file and save it inside the files
    private async Task WriteToFileAsync(string fileName, string text)
    {
        try
        {
            // get Log Path
            var logPath = Path.Combine(_env.ContentRootPath, "Logs");
            // if path not exits create one
            Directory.CreateDirectory(logPath);

            var logFilePath = Path.Combine(logPath, $"{fileName}.log");
            await File.AppendAllTextAsync(logFilePath, text + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write log: {ex.Message}");
        }

    }

}
