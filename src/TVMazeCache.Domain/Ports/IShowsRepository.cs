using TVMazeCache.Domain.Models;

namespace TVMazeCache.Domain.Ports
{
    public interface IShowsRepository
    {
        /// <summary>
        /// Stores a batch of shows.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="shows">The collection of shows</param>
        /// <returns></returns>
        Task StoreBatch(IEnumerable<Show> shows, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a collection of shows based on the index (paginated).
        /// </summary>
        /// <param name="index">The index for the collection of the show (page)</param>
        /// <returns>A collection of shows</returns>
        Task<IEnumerable<Show>> Get(int index);
    }
}
