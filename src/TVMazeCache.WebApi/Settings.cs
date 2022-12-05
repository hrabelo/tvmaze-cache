using TVMazeCache.ApiClient.Infrastructure;
using TVMazeCache.Persistence.MongoDB.Infrastructure;
using TVMazeCache.WebApi.BackgroundServices;

namespace TVMazeCache.WebApi
{
    internal class Settings
    {
        public StoreShowsBackgroundServiceSettings StoringBackgroundServiceSettings { get; set; }

        public TvMazeApiClientSettings TvMazeApiClientSettings { get; set; }

        public MongoDbShowsRepositorySettings MongoDbShowsRepositorySettings { get; set; }

        public Settings(IConfiguration configuration)
        {
            configuration.Bind(this);
        }
    }
}
