
using TVMazeCache.Domain.Models;

namespace TVMazeCache.WebApi.V1.Models
{
    public class ShowDto
    {
        public int Id { get; }
        public string Name { get; }
        public IEnumerable<PersonDto> Cast { get; }

        public ShowDto(int id, string name, IEnumerable<PersonDto> cast)
        {
            Id = id;
            Name = name;
            Cast = cast;
        }

        public static ShowDto FromDomain(Show s) =>
            new(s.Id, s.Name, s.Cast.Select(p => PersonDto.FromDomain(p)).OrderBy(p => p.Birthday));
    }
}
