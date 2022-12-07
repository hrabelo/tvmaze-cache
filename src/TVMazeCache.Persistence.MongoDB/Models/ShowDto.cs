using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TVMazeCache.Domain.Models;

namespace TVMazeCache.Persistence.MongoDB.Models
{
    public class ShowDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? StoreId { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        public string? Name { get; set; }

        public PersonDto[]? Cast { get; set; }

        public Show ToDomain() =>
            new(Id, Name!, Cast!.Select(_ => _.ToDomain()));

        public static ShowDto FromDomain(Show s) =>
            new()
            {
                Id = s.Id,
                Name = s.Name,
                Cast = s.Cast!.Select(PersonDto.FromDomain).ToArray()
            };
    }
}
