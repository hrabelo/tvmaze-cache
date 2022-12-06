using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MongoDB.Driver;
using TVMazeCache.Domain.Models;
using TVMazeCache.Persistence.MongoDB.Models;

namespace TVMazeCache.WebApi.Tests.Integration.Helpers
{
    public class MongoDbHelper
    {
        private bool _started = false;
        private readonly MongoDbTestcontainer _container;
        private readonly MongoDbTestcontainerConfiguration _mongoDbConfig = new MongoDbTestcontainerConfiguration() { Database = "tv-maze-cache", Username = "", Password = "" };

        public IMongoDatabase? MongoDatabase { get; set; }

        public MongoDbHelper()
        {
            _container = new TestcontainersBuilder<MongoDbTestcontainer>()
                .WithImage("mongo:5.0.6")
                .WithDatabase(_mongoDbConfig)
                .Build();
        }

        public void StartContainerSync() =>
            Task.Run(StartContainerAsync).Wait();
        
        public async Task StartContainerAsync()
        {
            if (!_started)
            {
                await _container.StartAsync();
                _started = true;
            }
            var client = new MongoClient(_container.ConnectionString);
            MongoDatabase = client.GetDatabase(_mongoDbConfig.Database);
        }

        public async Task InsertItemToShowCollection(Show show)
        {
            var showCollection = MongoDatabase!.GetCollection<ShowDto>("shows");
            await showCollection!.InsertOneAsync(ShowDto.FromDomain(show));
        }

        public async Task<IEnumerable<Show>> GetAll()
        {
            var showCollection = MongoDatabase!.GetCollection<ShowDto>("shows");
            var dtos = await showCollection.Find(_ => true).ToListAsync();
            return dtos.Select(_ => _.ToDomain());
        }
    }
}
