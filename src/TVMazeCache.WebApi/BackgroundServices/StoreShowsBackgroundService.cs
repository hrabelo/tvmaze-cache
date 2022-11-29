namespace TVMazeCache.WebApi.BackgroundServices
{
    internal class StoreShowsBackgroundService : BackgroundService
    {
        private readonly int _delayInMilliseconds;
        private readonly ILogger<StoreShowsBackgroundService> _logger;
        
        public StoreShowsBackgroundService(
            StoringBackgroundServiceSettings settings, 
            ILogger<StoreShowsBackgroundService> logger)
        {
            _delayInMilliseconds = settings.DelayInMilliseconds;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Working...");
                await Task.Delay(_delayInMilliseconds, stoppingToken);
            }
        }
    }
}
