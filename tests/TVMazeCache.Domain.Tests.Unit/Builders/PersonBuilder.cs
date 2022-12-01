using AutoFixture;
using TVMazeCache.Domain.Models;

namespace TVMazeCache.Domain.Tests.Unit.Builders
{
    internal class PersonBuilder
    {
        private static readonly Fixture _fixture = new();
        
        internal static IEnumerable<Person> BuildMany(int number)
        {
            for (var i =0; i < number; i++)
            {
                yield return Build();
            }
        }

        internal static Person Build()
        {
            return _fixture.Create<Person>();
        }
    }
}
