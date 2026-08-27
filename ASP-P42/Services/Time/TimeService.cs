namespace ASP_P42.Services.Time
{
    public class TimeService : ITimeService
    {
        public long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}