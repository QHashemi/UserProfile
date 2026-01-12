namespace UserProfile.Dto.Response
{
    public class TestControllersResponseDto
    {
        public string Status {  get; set; } = string.Empty;
        public DateTime TimeStamp {  get; set; }
        public TimeSpan Uptime { get; set; }
     
    }
}
