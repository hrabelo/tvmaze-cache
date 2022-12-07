using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            AddHttpClient(services, clientName, settings);
            RegisterClients(services, clientName);
        }

        public static void AddHttpClient(
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
                           .AddPolicyHandler((sp, _) => Policies.GetRetryPolicyForRateLimit(settings, sp.GetRequiredService<ILogger<TvMazeApiClient>>()))
                           .AddPolicyHandler((sp, _) => Policies.GetRetryPolicyForTransientErrors(settings, sp.GetRequiredService<ILogger<TvMazeApiClient>>()));
        }

        public static void RegisterClients(
            this IServiceCollection services,
            string clientName)
        {
           services.AddTransient<ITvMazeApiClient>(sp =>
                new TvMazeApiClient(() => sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName)));
        }
    }
}
