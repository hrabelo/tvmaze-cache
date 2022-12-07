namespace TVMazeCache.Domain.Models
{
    public class Show
    {
        public int Id { get; }
        public string Name { get; }
        public IEnumerable<Person>? Cast { get; }

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
