using infrastructure.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using System.Net.Http.Headers;

namespace infrastructure
{
    public class MCPClientFactory(IAccessTokenService accessTokenService, IConfiguration configuration) : IMCPClientFactory
    {
        public async Task<McpClient> CreateContainerClient()
        {

            var httpClient = new HttpClient(new BearerTokenHandler(accessTokenService));
            var httpTransport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://localhost:7196"),
                TransportMode = HttpTransportMode.StreamableHttp,
            }, httpClient: httpClient);

            return await McpClient.CreateAsync(httpTransport);
         
        }

        public async Task<McpClient> CreateVesselClient()
        {
            var httpClient = new HttpClient(new BearerTokenHandler(accessTokenService));
            var httpTransport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://localhost:7289"),
                TransportMode = HttpTransportMode.StreamableHttp,
            }, httpClient: httpClient);

            return await McpClient.CreateAsync(httpTransport);
          
        }

        public async Task<McpClient> CreateBookingClient()
        {
            var httpClient = new HttpClient(new BearerTokenHandler(accessTokenService));
            var httpTransport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://localhost:7044"),
                TransportMode = HttpTransportMode.StreamableHttp,
            }, httpClient: httpClient);

            return await McpClient.CreateAsync(httpTransport);
        }
        public async Task<McpClient> CreateCapacityClient()
        {
            var httpClient = new HttpClient(new BearerTokenHandler(accessTokenService));
            var httpTransport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://localhost:7061"),
                TransportMode = HttpTransportMode.StreamableHttp,
            }, httpClient: httpClient);

            return await McpClient.CreateAsync(httpTransport);
        }
    }

    internal sealed class BearerTokenHandler : DelegatingHandler
    {
        private readonly IAccessTokenService _accessTokenService;

        public BearerTokenHandler(IAccessTokenService accessTokenService)
        {
            InnerHandler = new HttpClientHandler();
            _accessTokenService = accessTokenService;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
           HttpRequestMessage request,
           CancellationToken cancellationToken)
        {
            var accessToken = await _accessTokenService.GetAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
