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
            _logger.LogInformation("Resuming scrapping from page {Page}", page);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var executionStopwatch = new Stopwatch();

                    executionStopwatch.Start();
                    var result = await _useCase.Execute(page, stoppingToken);
                    executionStopwatch.Stop();

                    await _indexRepository.UpdateLastPageNumber(page, stoppingToken);

                    if (result == IngestedBatchResult.NothingToProcess)
                    {
                        totalStopwatch.Stop();
                        _logger.LogInformation("Scrapping took {Seconds} seconds in total", totalStopwatch.ElapsedMilliseconds / 1000);
                        return;
                    }

                    _logger.LogInformation("Scrapping page {Page} took {Seconds} seconds ", page, executionStopwatch.ElapsedMilliseconds / 1000);
                    await Task.Delay(_delayInMilliseconds, stoppingToken);
                }
                catch (HttpRequestException httpEx)
                {
                    // If the errors get through the polly policies, then assume them as transient and ignore the page
                    _logger.LogWarning(httpEx, "Transient failure when processing page {Page}", page);
                }
                catch (Exception ex)
                {
                    // If more generic exception, such as failing to store to Mongo, should log and abort processing
                    _logger.LogError(ex, "Fatal error");
                    return;
                }
                finally
                {
                    page++;
                }
            }
        }
    }
}
