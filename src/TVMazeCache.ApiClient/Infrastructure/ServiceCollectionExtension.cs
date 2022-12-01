using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.ApiClient.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void AddTvMazeApiClient(
            this IServiceCollection services, 
            string clientName, 
            TvMazeApiClientSettings settings)
        {
            services.AddHttpClient(
                name: clientName,
                configureClient: client =>
                {
                    client.BaseAddress = new Uri(settings.BaseUrl!);
                    client.Timeout = TimeSpan.FromMilliseconds(settings.TimeoutMilliseconds);
                })
                .AddPolicyHandler((services, request) => GetRetryPolicyForRateLimit(settings, services.GetRequiredService<ILogger<TvMazeApiClient>>()))
                .AddPolicyHandler((services, request) => GetRetryPolicyForTransientErrors(services.GetRequiredService<ILogger<TvMazeApiClient>>()));
                

            services.AddTransient<ITvMazeApiClient>(sp =>
                new TvMazeApiClient(() => sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyForRateLimit(TvMazeApiClientSettings settings, ILogger<TvMazeApiClient> logger)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    settings.RetryCount,
                    retryAttepmt => TimeSpan.FromSeconds(settings.WaitIntervalSeconds),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning("Too many requests when processing path {Path}. Delaying for {Delay} ms, then making retry {Retry}.", outcome.Result.RequestMessage!.RequestUri!.AbsolutePath, timespan.TotalMilliseconds, retryAttempt);
                    });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicyForTransientErrors(ILogger<TvMazeApiClient> logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    retryAttepmt => TimeSpan.FromSeconds(5),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning("{ReasonPhrase} when processing path {Path}. Delaying for {Delay} ms, then making retry {Retry}.", outcome.Result.ReasonPhrase, outcome.Result.RequestMessage!.RequestUri!.AbsolutePath, timespan.TotalMilliseconds, retryAttempt);
                    });
        }
    }
}
