namespace TVMazeCache.Domain.Models
{
    public class Person
    {
        public int Id { get; }
        public string Name { get; }
        public DateTime? Birthday { get; }

        public Person(int id, string name, DateTime? birthday)
        {
            Id = id;
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Birthday = birthday;
        }
    }
}