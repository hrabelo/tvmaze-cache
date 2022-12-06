using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace TVMazeCache.ApiClient.Infrastructure
{
    public static class Policies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyForRateLimit(TvMazeApiClientSettings settings, ILogger logger)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    settings.TooManyRequestsRetryCount,
                    retryAttepmt => TimeSpan.FromSeconds(settings.TooManyRequestsWaitIntervalSeconds),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning("Too many requests when processing path {Path}. Delaying for {Delay} ms, then making retry {Retry}.", outcome.Result?.RequestMessage?.RequestUri?.AbsolutePath, timespan.TotalMilliseconds, retryAttempt);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyForTransientErrors(TvMazeApiClientSettings settings, ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: settings.TransientErrorRetryCount,
                    retryAttepmt => TimeSpan.FromSeconds(settings.TransientErrorWaitIntervalSeconds),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning("{ReasonPhrase} when processing path {Path}. Delaying for {Delay} ms, then making retry {Retry}.", outcome.Result?.ReasonPhrase, outcome.Result?.RequestMessage?.RequestUri?.AbsolutePath, timespan.TotalMilliseconds, retryAttempt);
                    });
        }
    }
}
