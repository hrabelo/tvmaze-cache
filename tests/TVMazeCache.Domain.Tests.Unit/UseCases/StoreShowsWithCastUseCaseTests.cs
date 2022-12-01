using FluentAssertions;
using Moq;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;
using TVMazeCache.Domain.Tests.Unit.Builders;
using TVMazeCache.Domain.UseCases;
using Xunit;

namespace TVMazeCache.Domain.Tests.Unit.UseCases
{
    public class StoreShowsWithCastUseCaseTests
    {

        private readonly Mock<ITvMazeApiClient> _tvMazeApiClientMock;
        private readonly Mock<IShowsRepository> _showsRepositoryMock;

        public StoreShowsWithCastUseCaseTests()
        {
            _tvMazeApiClientMock = new Mock<ITvMazeApiClient>(MockBehavior.Strict);
            _showsRepositoryMock = new Mock<IShowsRepository>(MockBehavior.Strict);
        }

        [Fact]
        public async Task Execute_WhenShowsListIsEmpty_ShouldReturnIngestedBatchResultNothingToProcess()
        {
            var page = 0;
            var cts = new CancellationToken();

            TvMazeApiClientReturnsEmptyShowList();

            var result = await CreateSut().Execute(page, cts);

            result.Should().Be(IngestedBatchResult.NothingToProcess);
        }


        [Fact]
        public async Task Execute_GivenShowsListIsNotEmpty_WhenAllCastsAreRetrieved_ShouldStoreEnrichedShowsAndReturnIngestedBatchSuccess()
        {
            var page = 0;
            var cts = new CancellationToken();

            TvMazeApiClientReturnsShowList(10);

            TvMazeApiClientReturnsCastForAllShows();

            var shows = new List<List<Show>>();
            _showsRepositoryMock.Setup(_ => _.StoreBatch(Capture.In(shows), cts))
                .Returns(Task.CompletedTask);

            var result = await CreateSut().Execute(page, cts);

            var showsBeforeStoring = shows.Single();
            showsBeforeStoring.Should().AllSatisfy(_ => _.Cast!.Any());

            result.Should().Be(IngestedBatchResult.Success);
        }

        private void TvMazeApiClientReturnsEmptyShowList() =>
            _tvMazeApiClientMock.Setup(_ => _.GetShows(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Show>());

        private void TvMazeApiClientReturnsShowList(int number) {
            var shows = ShowBuilder.BuildMany(number);
            _tvMazeApiClientMock.Setup(_ => _.GetShows(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(shows);
        }

        private void TvMazeApiClientReturnsCastForAllShows() =>
       _tvMazeApiClientMock.Setup(_ => _.GetCast(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(PersonBuilder.BuildMany(new Random().Next(10)));

        private StoreShowsWithCastUseCase CreateSut() =>
            new(_tvMazeApiClientMock.Object, _showsRepositoryMock.Object);
    }
}
