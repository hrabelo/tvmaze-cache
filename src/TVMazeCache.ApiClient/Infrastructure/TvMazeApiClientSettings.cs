namespace TVMazeCache.ApiClient.Infrastructure
{
    public class TvMazeApiClientSettings
    {
        public string? BaseUrl { get; set; }

        public long TimeoutMilliseconds { get; set; } = 30000;

        public int TooManyRequestsRetryCount { get; set; } = 3;

        public int TooManyRequestsWaitIntervalSeconds { get; set; } = 10;

        public int TransientErrorRetryCount { get; set; } = 1;

        public int TransientErrorWaitIntervalSeconds { get; set; } = 1;

    }
}
