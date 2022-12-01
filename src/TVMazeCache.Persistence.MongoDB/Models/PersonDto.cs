using TVMazeCache.Domain.Models;

namespace TVMazeCache.Persistence.MongoDB.Models
{
    public class PersonDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime? Birthday { get; set; }

        public Person ToDomain() =>
            new(Id, Name!, Birthday);

        public static PersonDto FromDomain(Person p) =>
         new()
         {
             Id = p.Id,
             Name = p.Name,
             Birthday = p.Birthday
         };
    }
}
