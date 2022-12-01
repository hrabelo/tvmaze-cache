namespace TVMazeCache.Domain.Models
{
    public class Person
    {
        public readonly int Id;
        public readonly string Name;
        public readonly DateTime? Birthday;

        public Person(int id, string name, DateTime? birthday)
        {
            Id = id;
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Birthday = birthday;
        }
    }
}