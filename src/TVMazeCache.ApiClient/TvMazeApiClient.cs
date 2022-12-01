using Newtonsoft.Json;
using TVMazeCache.ApiClient.Models;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.ApiClient
{
    public class TvMazeApiClient : ITvMazeApiClient
    {
        private static string _getShowsEndpoint(int page) => $"shows?page={page}";
        private static string _getCastEndpoint(int showId) => $"shows/{showId}/cast";

        private readonly Func<HttpClient> _httpClientFactory;

        public TvMazeApiClient(Func<HttpClient> httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Show>> GetShows(int page, CancellationToken token)
        {
            var httpClient = _httpClientFactory();
            var response = await httpClient.GetAsync(_getShowsEndpoint(page), token);

            var resultAsString = await response.Content.ReadAsStringAsync();
            var showsDto = JsonConvert.DeserializeObject<IEnumerable<ShowDto>>(resultAsString);
            return showsDto!.Select(_ => _.ToDomain());
        }

        public async Task<IEnumerable<Person>> GetCast(int showId, CancellationToken token)
        {
            var httpClient = _httpClientFactory();
            var response = await httpClient.GetAsync(_getCastEndpoint(showId), token);

            var resultAsString = await response.Content.ReadAsStringAsync();
            var castDto = JsonConvert.DeserializeObject<IEnumerable<CastDto>>(resultAsString);
            return castDto!.Select(_ => _.Person.ToDomain());
        }

    }
}
