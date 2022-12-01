namespace TVMazeCache.ApiClient.Infrastructure
{
    public class TvMazeApiClientSettings
    {
        public string? BaseUrl { get; set; }

        public long TimeoutMilliseconds { get; set; } = 30000;

        public int RetryCount { get; set; } = 3;

        public int WaitIntervalSeconds { get; set; } = 10;
    }
}
