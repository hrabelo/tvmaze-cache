using FluentAssertions;
using Moq;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;
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

        private void TvMazeApiClientReturnsEmptyShowList() =>
            _tvMazeApiClientMock.Setup(_ => _.GetShows(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Show>());

        private StoreShowsWithCastUseCase CreateSut() =>
            new(_tvMazeApiClientMock.Object, _showsRepositoryMock.Object);
    }
}
