using TVMazeCache.Domain.Models;

namespace TVMazeCache.ApiClient.Models
{
    public class PersonDto
    {
        public readonly int Id;
        public readonly string Name;
        public readonly DateTime? Birthday;

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
