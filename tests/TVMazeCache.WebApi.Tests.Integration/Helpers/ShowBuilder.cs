using AutoFixture;
using TVMazeCache.Domain.Models;


namespace TVMazeCache.Persistence.MongoDB.Tests.Integration.Helpers
{
    public class ShowBuilder
    {
        private static readonly Fixture _fixture = new();

        public static IEnumerable<Show> BuildMany(int number)
        {
            var result = new List<Show>(number);
            for(var i = 0; i< number; i++)
            {
                result.Add(Build());
            }
            return result;
        }

        public static Show Build()
        {
            return _fixture.Create<Show>();
        }
    }
}
