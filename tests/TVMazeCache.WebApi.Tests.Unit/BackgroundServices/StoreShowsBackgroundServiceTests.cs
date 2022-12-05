using Microsoft.Extensions.Logging;
using Moq;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;
using TVMazeCache.Domain.UseCases;
using TVMazeCache.WebApi.BackgroundServices;

namespace TVMazeCache.WebApi.Tests.Unit.BackgroundServices
{
    public class StoreShowsBackgroundServiceTests
    {
        private readonly Mock<ITvMazeApiClient> _tvMazeApiClientMock = new(MockBehavior.Strict);
        private readonly Mock<IShowsRepository> _showsRepositoryMock = new(MockBehavior.Strict);

        private readonly Mock<IIndexRepository> _indexRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ILogger<StoreShowsBackgroundService>> _loggerMock = new(MockBehavior.Loose);
        private readonly StoreShowsBackgroundServiceSettings _settings = new() { DelayInMilliseconds = 10 };
        public StoreShowsBackgroundServiceTests()
        {

        }

        [Fact]
        public async Task ExecuteAsync_WhenUseCaseThrowsHttpExceptionForGivenPage_ShouldContinueToNextPage()
        {
            GivenStartingIndex(0);
            GivenUpdatingIndexIsOk();
            GivenUseCaseThrowsHttpExceptionForPage(0);
            GivenUseCaseReturnsNothingToProcess(1);

            await CreateSut().ExecuteAsync();

            _indexRepositoryMock.Verify(_ => _.UpdateLastPageNumber(0, It.IsAny<CancellationToken>()), Times.Never);
            _indexRepositoryMock.Verify(_ => _.UpdateLastPageNumber(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenUseCaseThrowsGenericExceptionForGivenPage_ShouldAbort()
        {
            GivenStartingIndex(0);
            GivenUpdatingIndexIsOk();
            GivenUseCaseThrowsExceptionForPage(0);
            GivenUseCaseReturnsNothingToProcess(1);

            await CreateSut().ExecuteAsync();

            _indexRepositoryMock.Verify(_ => _.UpdateLastPageNumber(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenUseCaseReturnsNothingToProcess_ShouldFinish()
        {
            GivenStartingIndex(0);
            GivenUpdatingIndexIsOk();
            GivenUseCaseReturnsSuccess(0);
            GivenUseCaseReturnsSuccess(1);
            GivenUseCaseReturnsSuccess(2);
            GivenUseCaseReturnsNothingToProcess(3);

            await CreateSut().ExecuteAsync();

            _indexRepositoryMock.Verify(_ => _.UpdateLastPageNumber(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        }

        private void GivenStartingIndex(int page) =>
           _indexRepositoryMock.Setup(_ => _.GetLastPageNumber(It.IsAny<CancellationToken>()))
               .ReturnsAsync(page);

        private void GivenUpdatingIndexIsOk() =>
          _indexRepositoryMock.Setup(_ => _.UpdateLastPageNumber(It.IsAny<int>(), It.IsAny<CancellationToken>()))
              .Returns(Task.CompletedTask);

        private void GivenUseCaseThrowsHttpExceptionForPage(int page) =>
            _tvMazeApiClientMock.Setup(_ => _.GetShows(page, It.IsAny<CancellationToken>()))
                .Throws(() => new HttpRequestException());

        private void GivenUseCaseThrowsExceptionForPage(int page) =>
        _showsRepositoryMock.Setup(_ => _.StoreBatch(It.IsAny<IEnumerable<Show>>(), It.IsAny<CancellationToken>()))
            .Throws(() => new Exception("Generic"));

        private void GivenUseCaseReturnsSuccess(int page)
        {
            _tvMazeApiClientMock.Setup(_ => _.GetShows(page, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Show>() { new Show(1,"Test") });
            _tvMazeApiClientMock.Setup(_ => _.GetCast(page, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Person>() { new Person(1,"John Doe", new DateTime(1968,1,2)) });

            _showsRepositoryMock.Setup(_ => _.StoreBatch(It.IsAny<IEnumerable<Show>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private void GivenUseCaseReturnsNothingToProcess(int page) =>
          _tvMazeApiClientMock.Setup(_ => _.GetShows(page, It.IsAny<CancellationToken>()))
              .ReturnsAsync(Enumerable.Empty<Show>());

        private StoreShowsBackgroundService CreateSut() =>
              new(
                new StoreShowsWithCastUseCase(_tvMazeApiClientMock.Object, _showsRepositoryMock.Object),
                _indexRepositoryMock.Object,
                _settings,
                _loggerMock.Object);

    }
}