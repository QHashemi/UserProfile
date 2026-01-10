namespace UserProfile.Dto.Response
{
    public class LoggingResponseDto
    {
        public string Message { get; set; }

        public string Level { get; set; }

        public string LogEvent { get; set; }

        public string TimeStamp { get; set; }

        public string UserIdentifier { get; set; }

        public int StatusCode { get; set; } 
    }
}
