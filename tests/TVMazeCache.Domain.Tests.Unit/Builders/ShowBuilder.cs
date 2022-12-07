using AutoFixture;
using TVMazeCache.Domain.Models;

namespace TVMazeCache.Domain.Tests.Unit.Builders
{
    internal class ShowBuilder
    {
        private static readonly Fixture _fixture = new();
        
        internal static IEnumerable<Show> BuildMany(int number)
        {
            for (var i = 0; i < number; i++)
            {
                yield return Build();
            }
        }

        internal static Show Build()
        {
            return _fixture.Create<Show>();
        }
    }
}
