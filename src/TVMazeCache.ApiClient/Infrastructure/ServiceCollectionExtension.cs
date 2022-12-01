using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.ApiClient.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void AddTvMazeApiClient(
            this IServiceCollection services, 
            string clientName, TvMazeApiClientSettings settings)
        {
            services.AddHttpClient(
                name: clientName,
                configureClient: client =>
                {
                    client.BaseAddress = new Uri(settings.BaseUrl!);
                    client.Timeout = TimeSpan.FromMilliseconds(settings.TimeoutMilliseconds);
                })
                .AddPolicyHandler(GetRetryPolicy(settings));

            services.AddTransient<ITvMazeApiClient>(sp =>
                new TvMazeApiClient(() => sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(TvMazeApiClientSettings settings)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(settings.RetryCount, retryAttempt => TimeSpan.FromSeconds(settings.WaitIntervalSeconds));
        }
    }
}
