namespace TVMazeCache.ApiClient.Models
{
    public class CastDto
    {
        public PersonDto Person { get; }
        
        public CastDto(PersonDto person)
        {
            Person = person;
        }
    }
}
