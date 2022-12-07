using TVMazeCache.Domain.Models;

namespace TVMazeCache.ApiClient.Models
{
    public class PersonDto
    {
        public int Id { get; }
        public string Name { get; }
        public DateTime? Birthday { get; }

        public PersonDto(int id, string name, DateTime? birthday)
        {
            Id = id;
            Name = name;
            Birthday = birthday;
        }

        public Person ToDomain() =>
            new(Id, Name, Birthday);
    }
}
