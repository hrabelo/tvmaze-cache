using Newtonsoft.Json;
using TVMazeCache.ApiClient.Models;
using TVMazeCache.Domain.Models;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.ApiClient
{
    public class TvMazeApiClient : ITvMazeApiClient
    {
        private static string GetShowsEndpoint(int page) => $"shows?page={page}";
        private static string GetCastEndpoint(int showId) => $"shows/{showId}/cast";

        private readonly Func<HttpClient> _httpClientFactory;

        public TvMazeApiClient(Func<HttpClient> httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Show>> GetShows(int page, CancellationToken token)
        {
            var httpClient = _httpClientFactory();
            var response = await httpClient.GetAsync(GetShowsEndpoint(page), token);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Enumerable.Empty<Show>();

            response.EnsureSuccessStatusCode();

            var resultAsString = await response.Content.ReadAsStringAsync(token);
            var showsDto = JsonConvert.DeserializeObject<IEnumerable<ShowDto>>(resultAsString);
            return showsDto!.Select(_ => _.ToDomain());
        }

        public async Task<IEnumerable<Person>> GetCast(int showId, CancellationToken token)
        {
            var httpClient = _httpClientFactory();
            var response = await httpClient.GetAsync(GetCastEndpoint(showId), token);

            response.EnsureSuccessStatusCode();

            var resultAsString = await response.Content.ReadAsStringAsync(token);
            var castDto = JsonConvert.DeserializeObject<IEnumerable<CastDto>>(resultAsString);
            return castDto!.Select(_ => _.Person.ToDomain());
        }

    }
}
