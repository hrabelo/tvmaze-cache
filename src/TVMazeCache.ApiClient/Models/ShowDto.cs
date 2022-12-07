using TVMazeCache.Domain.Models;

namespace TVMazeCache.ApiClient.Models
{
    public class ShowDto
    {
        public int Id { get; }
        public string Name { get; }

        public ShowDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Show ToDomain() =>
            new(Id, Name);
    }
}
