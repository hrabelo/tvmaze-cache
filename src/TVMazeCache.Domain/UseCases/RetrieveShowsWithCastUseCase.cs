using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.Domain.UseCases
{
    public class RetrieveShowsWithCastUseCase
    {
        private readonly IShowsRepository _showsRepository;

        public RetrieveShowsWithCastUseCase(
            IShowsRepository showsRepository)
        {
            _showsRepository = showsRepository;
        }

        public async Task<IEnumerable<Show>> Execute(int page) =>
            await _showsRepository.Get(page);
    }
}
