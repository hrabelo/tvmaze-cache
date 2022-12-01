using TVMazeCache.Domain.Models;

namespace TVMazeCache.Domain.UseCases
{
    public class RetrieveShowsWithCastUseCase
    {
        public RetrieveShowsWithCastUseCase()
        {
        }

        public async Task<IEnumerable<Show>> Execute(int page)
        {
            return await Task.FromResult(new List<Show>());
        }
    }
}
