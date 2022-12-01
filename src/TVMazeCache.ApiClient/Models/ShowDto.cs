using TVMazeCache.Domain.Models;

namespace TVMazeCache.ApiClient.Models
{
    public class ShowDto
    {
        public readonly int Id;
        public readonly string Name;

        public ShowDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Show ToDomain() =>
            new(Id, Name);
    }
}
