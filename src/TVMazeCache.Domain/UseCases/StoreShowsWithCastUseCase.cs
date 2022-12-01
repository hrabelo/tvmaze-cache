using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.Domain.UseCases
{
    public class StoreShowsWithCastUseCase
    {
        private readonly ITvMazeApiClient _tvMazeApiClient;
        private readonly IShowsRepository _showsRepository;

        public StoreShowsWithCastUseCase(
            ITvMazeApiClient tvMazeApiClient,
            IShowsRepository showsRepository)
        {
            _tvMazeApiClient = tvMazeApiClient;
            _showsRepository = showsRepository;
        }

        public async Task<IngestedBatchResult> Execute(int page, CancellationToken cancellationToken)
        {
            var shows = await _tvMazeApiClient.GetShows(page, cancellationToken);

            if (!shows.Any())
                return IngestedBatchResult.NothingToProcess;

            return IngestedBatchResult.Success;
        }
    }
}
