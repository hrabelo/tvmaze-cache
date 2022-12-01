using MongoDB.Driver;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.Persistence.MongoDB
{
    public class IndexRepository : IIndexRepository
    {
        private readonly IMongoCollection<Models.Index> _indexCollection;
        public IndexRepository(IMongoDatabase mongoDatabase)
        {
            _indexCollection = mongoDatabase.GetCollection<Models.Index>("index");
        }

        public async Task<int> GetLastPageNumber(CancellationToken cancellationToken)
        {
            var index = await _indexCollection.Find(_ => true).FirstOrDefaultAsync(cancellationToken);
            return index is null ? 0 : index.LastPageNumber;
        }

        public async Task UpdateLastPageNumber(int page, CancellationToken cancellationToken)
        {
            var item = await _indexCollection.Find(_ => true).FirstOrDefaultAsync(cancellationToken);
            if (item == null)
            {
                await _indexCollection.InsertOneAsync(new Models.Index() { LastPageNumber = page }, null, cancellationToken);
            }
            else
            {
                var filter = Builders<Models.Index>.Filter.Where(_ => true);
                var update = Builders<Models.Index>.Update.Set(_ => _.LastPageNumber, page);
                await _indexCollection.UpdateOneAsync(filter, update, null, cancellationToken);
            }
        }
    }
}