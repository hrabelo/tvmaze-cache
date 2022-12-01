using System.Diagnostics;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;
using TVMazeCache.Domain.UseCases;

namespace TVMazeCache.WebApi.BackgroundServices
{
    internal class StoreShowsBackgroundService : BackgroundService
    {
        private readonly StoreShowsWithCastUseCase _useCase;
        private readonly IIndexRepository _indexRepository;
        private readonly int _delayInMilliseconds;
        private readonly ILogger<StoreShowsBackgroundService> _logger;

        public StoreShowsBackgroundService(
            StoreShowsWithCastUseCase useCase,
            IIndexRepository indexRepository,
            StoringBackgroundServiceSettings settings, 
            ILogger<StoreShowsBackgroundService> logger)
        {
            _useCase = useCase;
            _indexRepository = indexRepository;
            _delayInMilliseconds = settings.DelayInMilliseconds;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            var page = await _indexRepository.GetLastPageNumber(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var executionStopwatch = new Stopwatch();

                executionStopwatch.Start();
                var result = await _useCase.Execute(page, stoppingToken);
                executionStopwatch.Stop();

                if (result == IngestedBatchResult.NothingToProcess)
                {
                    totalStopwatch.Stop();
                    _logger.LogInformation("Scrapping took {Seconds} seconds in total", totalStopwatch.ElapsedMilliseconds / 1000);
                    return;
                }

                page++;

                await _indexRepository.UpdateLastPageNumber(page, stoppingToken);

                _logger.LogInformation("Scrapping page {Page} took {Seconds} seconds ", page, executionStopwatch.ElapsedMilliseconds / 1000);

                await Task.Delay(_delayInMilliseconds, stoppingToken);
            }
        }
    }
}
