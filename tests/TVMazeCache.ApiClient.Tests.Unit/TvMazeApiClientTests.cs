using FluentAssertions;
using Moq;
using Moq.Protected;
using TVMazeCache.Domain.Ports;

namespace TVMazeCache.ApiClient.Tests.Unit
{
    public class TvMazeApiClientTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
        public TvMazeApiClientTests()
        {
        }

        [Fact]
        public async Task GetShows_WhenStatusCodeIsOk_ShouldReturnNonEmptyListOfShows()
        {
            var response = GetOkResponse(GetShowsExamplePayload());
            ConfigureHttpClient(response);

            var cts = new CancellationTokenSource().Token;
            var shows = await CreateSut().GetShows(0, cts);

            shows.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetShows_WhenStatusCodeIsNotOk_ShouldThrowException()
        {
            var response = GetInternalServerError();
            ConfigureHttpClient(response);

            var cts = new CancellationTokenSource().Token;
            await Assert.ThrowsAsync<HttpRequestException>(() => CreateSut().GetShows(0, cts));
        }

        [Fact]
        public async Task GetCast_WhenStatusCodeIsOk_ShouldReturnNonEmptyListOfPersons()
        {
            var response = GetOkResponse(GetCastExamplePayload());
            ConfigureHttpClient(response);

            var cts = new CancellationTokenSource().Token;
            var cast = await CreateSut().GetCast(0, cts);

            cast.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetCast_WhenStatusCodeIsNotOk_ShouldThrowException()
        {
            var response = GetInternalServerError();
            ConfigureHttpClient(response);

            var cts = new CancellationTokenSource().Token;
            await Assert.ThrowsAsync<HttpRequestException>(() => CreateSut().GetCast(0, cts));
        }

        private static HttpResponseMessage GetOkResponse(string content) =>
            new(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };

        private static HttpResponseMessage GetInternalServerError() =>
            new(System.Net.HttpStatusCode.InternalServerError);

        private static string GetShowsExamplePayload() =>
            "[{\"id\":1,\"name\":\"Under the dome\"}]";

        private static string GetCastExamplePayload() =>
            "[{\"person\": {\"id\":1, \"name\":\"John Doe\", \"birthday\": \"1979-01-12\"}}]";


        private void ConfigureHttpClient(HttpResponseMessage responseMessage)
        {
            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage)
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());

            var httpClient = new HttpClient(clientHandlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost");
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        public ITvMazeApiClient CreateSut() =>
            new TvMazeApiClient(() => _httpClientFactoryMock.Object.CreateClient());
    }
}
