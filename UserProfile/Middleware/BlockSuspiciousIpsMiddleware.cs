namespace UserProfile.Middleware
{
    public class BlockSuspiciousIpsMiddleware
    {
        private readonly RequestDelegate _next;

        // Important link to see blocked ips: https://www.abuseipdb.com/
        private static readonly HashSet<string> _blockedIps = new()
        {
            "103.165.139.145",
            "167.71.7.101",
            "172.86.94.251",
            "64.62.156.81",
            "165.22.198.0",
            "5.166.216.234",
            "113.164.234.27",
            "85.203.23.137",
            "209.38.101.192",
            "188.166.52.133",
            "164.92.158.205",
            "167.71.77.137",
            "24.143.165.146",
            "88.97.165.231",
            "101.0.104.38",
            "144.126.212.146",
            "111.48.77.157",
            "115.140.161.61",
            "64.62.156.24",
            "35.203.210.55"
        };


        // Constructor: inject the next middleware
        public BlockSuspiciousIpsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // This is called on every HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            if (clientIp != null && _blockedIps.Contains(clientIp))
            {
                // Block the request
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Forbidden");
                return; // stop pipeline
            }

            // Call the next middleware
            await _next(context);
        }
    }
}