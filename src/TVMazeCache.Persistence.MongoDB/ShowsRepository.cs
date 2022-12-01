using MongoDB.Driver;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;
using TVMazeCache.Persistence.MongoDB.Models;

namespace TVMazeCache.Persistence.MongoDB
{
    public class ShowsRepository : IShowsRepository
    {
        private const int PAGE_SIZE = 100;
        private readonly IMongoCollection<ShowDto> _showCollection;
        public ShowsRepository(IMongoDatabase mongoDatabase)
        {
            _showCollection = mongoDatabase.GetCollection<ShowDto>("shows");
        }

        public async Task<IEnumerable<Show>> Get(int page)
        {
            var zeroBasedPage = (page - 1) > 0 ? (page - 1) : 0;
            var showsDto = await _showCollection.Find(_ => true).ToListAsync();
            return showsDto.Skip(zeroBasedPage * PAGE_SIZE).Select(_ => _.ToDomain()).Take(PAGE_SIZE);
        }

        public async Task StoreBatch(IEnumerable<Show> shows, CancellationToken cancellationToken)
        {
            var showsDto = shows.Select(_ => ShowDto.FromDomain(_));
            await _showCollection.InsertManyAsync(showsDto, null, cancellationToken);
        }
    }
}