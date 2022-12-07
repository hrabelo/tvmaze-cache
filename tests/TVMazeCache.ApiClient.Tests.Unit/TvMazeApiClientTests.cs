using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TVMazeCache.ApiClient.Infrastructure;
using TVMazeCache.Domain.Ports;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TVMazeCache.ApiClient.Tests.Unit
{
    public class TvMazeApiClientTests
    {
        private readonly WireMockServer _server;
        private readonly IServiceProvider _serviceProvider;
        private readonly TvMazeApiClientSettings _settings;
        public TvMazeApiClientTests()
        {
            _server = WireMockServer.Start();

            _settings = new TvMazeApiClientSettings()
            {
                BaseUrl = "http://localhost:" + _server.Ports[0],
                TimeoutMilliseconds = 60000,
                TooManyRequestsRetryCount = 3,
                TooManyRequestsWaitIntervalSeconds = 10,
                TransientErrorRetryCount = 2,
                TransientErrorWaitIntervalSeconds = 5
            };

            var services = new ServiceCollection();
            services.AddTvMazeApiClient("Test", _settings);
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task GetShows_WhenReturnsStatusCode200_ShouldReturnNonEmptyListOfShows()
        {
            GivenGetShowsReturns200();

            var cts = new CancellationTokenSource().Token;
            var shows = await CreateSut().GetShows(0, cts);
            shows.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetShows_WhenReturnsStatusCode500LessTimesThanRetryConfigAndThen200_ShouldRetryAndReturnNonEmptyListOfShows()
        {
            GivenGetShowsReturn500NTimesAndThen200(_settings.TransientErrorRetryCount);

            var cts = new CancellationTokenSource().Token;
            var shows = await CreateSut().GetShows(0, cts);
            shows.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetShows_WhenReturnsStatusCode500MoreTimesThanRetryConfig_ShouldThrowException()
        {
            GivenGetShowsReturn500NTimesAndThen200(_settings.TransientErrorRetryCount + 1);

            var cts = new CancellationTokenSource().Token;
            await Assert.ThrowsAsync<HttpRequestException>(() => CreateSut().GetShows(0, cts));
        }

        [Fact]
        public async Task GetShows_WhenReturnsStatusCode429LessTimesThanRetryConfigAndThen200_ShouldRetryAndReturnNonEmptyListOfShows()
        {
            GivenGetShowsReturn429NTimesAndThen200(_settings.TooManyRequestsRetryCount);

            var cts = new CancellationTokenSource().Token;
            var shows = await CreateSut().GetShows(0, cts);
            shows.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetShows_WhenReturnsStatusCode429MoreTimesThanRetryConfig_ShouldThrowException()
        {
            GivenGetShowsReturn429NTimesAndThen200(_settings.TooManyRequestsRetryCount + 1);

            var cts = new CancellationTokenSource().Token;
            await Assert.ThrowsAsync<HttpRequestException>(() => CreateSut().GetShows(0, cts));
        }

        [Fact]
        public async Task GetCast_WhenReturnsStatusCode200_ShouldReturnNonEmptyListOfPersons()
        {
            GivenGetCastReturns200();

            var cts = new CancellationTokenSource().Token;
            var cast = await CreateSut().GetCast(0, cts);
            cast.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetCast_WhenReturnsStatusCode500LessTimesThanRetryConfigAndThen200_ShouldRetryAndReturnNonEmptyListOfPersons()
        {
            GivenGetCastReturn500NTimesAndThen200(_settings.TransientErrorRetryCount);

            var cts = new CancellationTokenSource().Token;
            var cast = await CreateSut().GetCast(0, cts);
            cast.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetCast_WhenReturnsStatusCode500MoreTimesThanRetryConfig_ShouldThrowException()
        {
            GivenGetCastReturn500NTimesAndThen200(_settings.TransientErrorRetryCount + 1);

            var cts = new CancellationTokenSource().Token;
            await Assert.ThrowsAsync<HttpRequestException>(() => CreateSut().GetCast(0, cts));
        }

        [Fact]
        public async Task GetCast_WhenReturnsStatusCode429LessTimesThanRetryConfigAndThen200_ShouldRetryAndReturnNonEmptyListOfPersons()
        {
            GivenGetCastReturn429NTimesAndThen200(_settings.TooManyRequestsRetryCount);

            var cts = new CancellationTokenSource().Token;
            var cast = await CreateSut().GetCast(0, cts);
            cast.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetCast_WhenReturnsStatusCode429MoreTimesThanRetryConfig_ShouldThrowException()
        {
            GivenGetCastReturn429NTimesAndThen200(_settings.TooManyRequestsRetryCount + 1);

            var cts = new CancellationTokenSource().Token;
            await Assert.ThrowsAsync<HttpRequestException>(() => CreateSut().GetCast(0, cts));
        }

        private void GivenGetShowsReturns200()
        {
            _server.Given(
               Request.Create()
                   .WithPath("/shows*")
                   .UsingGet())
               .RespondWith(
                   Response.Create()
                       .WithStatusCode(200)
                       .WithBody(GetShowsExamplePayload()));
        }

        private void GivenGetShowsReturn500NTimesAndThen200(int n)
        {
            var request = Request.Create()
                    .WithPath("/shows*")
                    .UsingGet();

            var scenario = "Get Shows InternalServerError + Ok";

            _server.Given(request)
                .InScenario(scenario)
                .WillSetStateTo("FIRST_REQUEST", n)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(500));

            _server.Given(request)
                .InScenario(scenario)
                .WhenStateIs("FIRST_REQUEST")
                .WillSetStateTo("SECOND_REQUEST")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(GetShowsExamplePayload()));
        }

        private void GivenGetShowsReturn429NTimesAndThen200(int n)
        {
            var request = Request.Create()
                    .WithPath("/shows*")
                    .UsingGet();

            var scenario = "Get Shows TooManyRequests + Ok";

            _server.Given(request)
                .InScenario(scenario)
                .WillSetStateTo("FIRST_REQUEST", n)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(429));

            _server.Given(request)
                .InScenario(scenario)
                .WhenStateIs("FIRST_REQUEST")
                .WillSetStateTo("SECOND_REQUEST")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(GetShowsExamplePayload()));
        }

        private void GivenGetCastReturns200()
        {
            _server.Given(
               Request.Create()
                   .WithPath("/shows/*/cast")
                   .UsingGet())
               .RespondWith(
                   Response.Create()
                       .WithStatusCode(200)
                       .WithBody(GetShowsExamplePayload()));
        }

        private void GivenGetCastReturn500NTimesAndThen200(int n)
        {
            var request = Request.Create()
                    .WithPath("/shows/*/cast")
                    .UsingGet();

            var scenario = "Get Cast InternalServerError + Ok";

            _server.Given(request)
                .InScenario(scenario)
                .WillSetStateTo("FIRST_REQUEST", n)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(500));

            _server.Given(request)
                .InScenario(scenario)
                .WhenStateIs("FIRST_REQUEST")
                .WillSetStateTo("SECOND_REQUEST")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(GetShowsExamplePayload()));
        }

        private void GivenGetCastReturn429NTimesAndThen200(int n)
        {
            var request = Request.Create()
                    .WithPath("/shows/*/cast")
                    .UsingGet();

            var scenario = "Get Cast TooManyRequests + Ok";

            _server.Given(request)
                .InScenario(scenario)
                .WillSetStateTo("FIRST_REQUEST", n)
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(429));

            _server.Given(request)
                .InScenario(scenario)
                .WhenStateIs("FIRST_REQUEST")
                .WillSetStateTo("SECOND_REQUEST")
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(GetShowsExamplePayload()));
        }


        private static string GetShowsExamplePayload() =>
            "[{\"id\":1,\"name\":\"Under the dome\"}]";

        private static string GetCastExamplePayload() =>
            "[{\"person\": {\"id\":1, \"name\":\"John Doe\", \"birthday\": \"1979-01-12\"}}]";

        public ITvMazeApiClient CreateSut() =>
            _serviceProvider.GetRequiredService<ITvMazeApiClient>();
    }
}
