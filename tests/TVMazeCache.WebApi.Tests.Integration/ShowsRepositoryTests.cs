
using FluentAssertions;
using TVMazeCache.Persistence.MongoDB;
using TVMazeCache.Persistence.MongoDB.Tests.Integration.Helpers;
using TVMazeCache.WebApi.Tests.Integration.Helpers;

namespace TVMazeCache.WebApi.Tests.Integration
{
    public class ShowsRepositoryTests
    {
        private static readonly MongoDbHelper MongoDbHelper = new();
        public ShowsRepositoryTests()
        {
            MongoDbHelper.StartContainerSync();
        }

        [Fact]
        public async Task StoreBatch_WhenCalled_ShouldStoreInMongoDb()
        {
            var shows = ShowBuilder.BuildMany(10);
            await CreateSut().StoreBatch(shows, CancellationToken.None);

            var retrievedShows = await MongoDbHelper.GetAll();
            retrievedShows.Should().HaveCountGreaterThanOrEqualTo(10);
        }

        [Fact]
        public async Task GetBatch_WhenCalled_ShouldStoreInMongoDb()
        {
            var show = ShowBuilder.Build();
            await MongoDbHelper.InsertItemToShowCollection(show);

            var retrievedShows = await CreateSut().Get(0);
            retrievedShows.Should().Contain(_ => _.Id == show.Id);
        }

        public ShowsRepository CreateSut() =>
            new(MongoDbHelper.MongoDatabase!);
    }
}