using TVMazeCache.Domain.Models;

namespace TVMazeCache.Domain.Ports
{
    public interface ITvMazeApiClient
    {
        /// <summary>
        /// Gets a paginated collection of shows.
        /// </summary>
        /// <param name="page">The number of the page</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of shows</returns>
        Task<IEnumerable<Show>> GetShows(int page, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the cast for a given show.
        /// </summary>
        /// <param name="showId">The id of the show</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of actors (cast)</returns>
        Task<IEnumerable<Person>> GetCast(int showId, CancellationToken cancellationToken);
    }
}
