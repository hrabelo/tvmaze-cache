
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using TVMazeCache.ApiClient.Infrastructure;

namespace TVMazeCache.ApiClient.Tests.Unit
{
    public class HttpClientTests
    {
        private const string APPNAME = "HttpClientTest";

        private readonly TvMazeApiClientSettings _settings = new()
        {
            BaseUrl = "http://localhost",
            TimeoutMilliseconds = 30000,
            TooManyRequestsRetryCount = 3,
            TooManyRequestsWaitIntervalSeconds = 1,
            TransientErrorRetryCount = 1,
            TransientErrorWaitIntervalSeconds = 1
        };

        private readonly Mock<ILogger> _loggerMock = new();

        public HttpClientTests()
        {
        }

        [Fact]
        public async Task SendAsync_GivenOneOkResponse_WhenStatusCodeOk_ShouldReturnOk()
        {
            var expectedResponse = GetOkResponseMessage();
            var queue = new Queue<HttpResponseMessage>();
            queue.Enqueue(expectedResponse);

            var httpClient = CreateSut(queue).CreateClient(APPNAME);
            var response = await httpClient.SendAsync(new HttpRequestMessage());
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task SendAsync_GivenMultipleTooManyRequestsResponse_WhenStatusCodeOkBeforeMaxTrialsExceeded_ShouldReturnOk()
        {
            var expectedResponse = GetOkResponseMessage();
            var tooManyRequestsResponse = GetTooManyRequestsResponseMessage();

            var queue = new Queue<HttpResponseMessage>();
            for (var i = 0; i < _settings.TooManyRequestsRetryCount; i++)
                queue.Enqueue(tooManyRequestsResponse);
            queue.Enqueue(expectedResponse);

            var httpClient = CreateSut(queue).CreateClient(APPNAME);
            var response = await httpClient.SendAsync(new HttpRequestMessage());
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task SendAsync_GivenMultipleTooManyRequestsResponse_WhenStatusCodeOkAfterMaxTrialsExceeded_ShouldReturnTooManyRequests()
        {
            var expectedResponse = GetTooManyRequestsResponseMessage();
            var okResponse = GetOkResponseMessage();

            var queue = new Queue<HttpResponseMessage>();
            for (var i = 0; i < _settings.TooManyRequestsRetryCount + 1; i++)
                queue.Enqueue(expectedResponse);
            queue.Enqueue(okResponse);

            var httpClient = CreateSut(queue).CreateClient(APPNAME);
            var response = await httpClient.SendAsync(new HttpRequestMessage());
            response.Should().BeEquivalentTo(expectedResponse);
        }


        [Fact]
        public async Task SendAsync_GivenMultipleInternalServerErrorResponse_WhenStatusCodeOkBeforeMaxTrialsExceeded_ShouldReturnOk()
        {
            var expectedResponse = GetOkResponseMessage();
            var internalServerErrorResponse = GetInternalServerErrorResponseMessage();

            var queue = new Queue<HttpResponseMessage>();
            for (var i = 0; i < _settings.TransientErrorRetryCount; i++)
                queue.Enqueue(internalServerErrorResponse);
            queue.Enqueue(expectedResponse);

            var httpClient = CreateSut(queue).CreateClient(APPNAME);
            var response = await httpClient.SendAsync(new HttpRequestMessage());
            response.Should().BeEquivalentTo(expectedResponse);
        }


        [Fact]
        public async Task SendAsync_GivenMultipleInternalServerErrorResponse_WhenStatusCodeOkAfterTrialsExceeded_ShouldReturnInternalServerError()
        {
            var expectedResponse = GetInternalServerErrorResponseMessage();
            var okResponse = GetOkResponseMessage();

            var queue = new Queue<HttpResponseMessage>();
            for (var i = 0; i < _settings.TransientErrorRetryCount + 1; i++)
                queue.Enqueue(expectedResponse);
            queue.Enqueue(okResponse);

            var httpClient = CreateSut(queue).CreateClient(APPNAME);
            var response = await httpClient.SendAsync(new HttpRequestMessage());
            response.Should().BeEquivalentTo(expectedResponse);
        }


        private static HttpResponseMessage GetOkResponseMessage() =>
            new(HttpStatusCode.OK);

        private static HttpResponseMessage GetTooManyRequestsResponseMessage() =>
          new(HttpStatusCode.TooManyRequests);

        private static HttpResponseMessage GetInternalServerErrorResponseMessage() =>
            new(HttpStatusCode.InternalServerError);

        private static DelegatingHandler CreateDelegatingHandlerForResponse(Queue<HttpResponseMessage> responseMessages)
        {
            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => responseMessages.Dequeue())
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());

            return clientHandlerMock.Object;
        }

        private IHttpClientFactory CreateSut(Queue<HttpResponseMessage> responseMessages)
        {
            var services = new ServiceCollection();

            services.AddHttpClient(
                name: APPNAME,
                configureClient: client =>
                {
                    client.BaseAddress = new Uri(_settings.BaseUrl!);
                })
                .ConfigurePrimaryHttpMessageHandler(() => CreateDelegatingHandlerForResponse(responseMessages))
                .AddPolicyHandler((service, request) =>
                    Policies.GetRetryPolicyForRateLimit(_settings, _loggerMock.Object))
                .AddPolicyHandler((service, request) =>
                    Policies.GetRetryPolicyForTransientErrors(_settings, _loggerMock.Object));

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IHttpClientFactory>();
        }
    }
}
