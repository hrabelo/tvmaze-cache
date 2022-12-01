using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.Persistence.MongoDB.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void AddShowsRepository(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var config = sp.GetRequiredService<MongoDbShowsRepositorySettings>();
                return new MongoClient(config.ConnectionString);
            });

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<MongoDbShowsRepositorySettings>();
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase(config.DatabaseName);
            });

            services.AddTransient<IShowsRepository, ShowsRepository>();
            services.AddTransient<IIndexRepository, IndexRepository>();
        }
    }
}
