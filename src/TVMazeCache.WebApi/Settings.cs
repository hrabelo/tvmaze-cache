using TVMazeCache.WebApi.BackgroundServices;

namespace TVMazeCache.WebApi
{
    internal class Settings
    {
        public StoringBackgroundServiceSettings StoringBackgroundServiceSettings { get; set; }

        public Settings(IConfiguration configuration)
        {
            configuration.Bind(this);
        }
    }
}
