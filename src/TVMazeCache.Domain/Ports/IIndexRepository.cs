namespace TVMazeCache.Domain.Ports
{
    public interface IIndexRepository
    {

        /// <summary>
        /// Gets the number of the last processed page
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The number of the last processed page</returns>
        Task<int> GetLastPageNumber(CancellationToken cancellationToken);

        /// <summary>
        /// Updates the number of the last processed page
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="page">The new number of the last processed page</param>
        Task UpdateLastPageNumber(int page, CancellationToken cancellationToken);
    }
}
