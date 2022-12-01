namespace TVMazeCache.ApiClient.Models
{
    public class CastDto
    {
        public readonly PersonDto Person;
        
        public CastDto(PersonDto person)
        {
            Person = person;
        }
    }
}
