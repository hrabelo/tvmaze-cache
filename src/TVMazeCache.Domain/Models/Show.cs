namespace TVMazeCache.Domain.Models
{
    public class Show
    {
        public readonly int Id;
        public readonly string Name;
        public readonly IEnumerable<Person>? Cast;

        public Show(int id, string name, IEnumerable<Person>? cast = null)
        {
            Id = id;
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Cast = cast;
        }

        public Show WithCast(IEnumerable<Person> cast) =>
            new(Id, Name, cast);
    }
}
