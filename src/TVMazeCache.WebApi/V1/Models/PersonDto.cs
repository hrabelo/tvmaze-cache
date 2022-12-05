using TVMazeCache.Domain.Models;

namespace TVMazeCache.WebApi.V1.Models
{
    public class PersonDto
    {
        public int Id { get; }
        public string Name { get; }
        public string Birthday { get; }

        public PersonDto(int id, string name, string birthday)
        {
            Id = id;
            Name = name;
            Birthday = birthday;
        }

        public static PersonDto FromDomain(Person p) =>
            new(p.Id, p.Name, p.Birthday?.Date.ToString("yyyy-MM-dd"));
    }
}
