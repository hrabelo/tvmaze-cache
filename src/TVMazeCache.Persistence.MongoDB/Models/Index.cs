using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TVMazeCache.Persistence.MongoDB.Models
{
    internal class Index
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? StoreId { get; set; }

        public int LastPageNumber { get; set; }
    }
}
