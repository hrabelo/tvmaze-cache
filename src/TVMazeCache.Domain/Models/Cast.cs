namespace TVMazeCache.Domain.Models
{
    public class Cast
    {
        public readonly IEnumerable<Person> Persons;
        
        public Cast(IEnumerable<Person> persons)
        {
            Persons = persons;
        }
    }
}