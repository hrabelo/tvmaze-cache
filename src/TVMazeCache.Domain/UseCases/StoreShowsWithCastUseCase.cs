using Microsoft.Extensions.Logging;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.Domain.UseCases
{
    public class StoreShowsWithCastUseCase
    {
        public StoreShowsWithCastUseCase()
        {
        }

        public async Task<IngestedBatchResult> Execute(int page, CancellationToken cancellationToken)
        {
            return await Task.FromResult(IngestedBatchResult.Success);
        }
    }
}
