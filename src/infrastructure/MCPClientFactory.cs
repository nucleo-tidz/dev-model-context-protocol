using infrastructure.Service;
using ModelContextProtocol.Client;

namespace infrastructure
{
    public class MCPClientFactory(IAccessTokenService accessTokenService): IMCPClientFactory
    {
        public async Task<IMcpClient> CreateContainerClient()
        {
           string token= await accessTokenService.GetAccessTokenAsync();
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         UseStreamableHttp=true,
                         Endpoint = new Uri("https://localhost:7196"),
                         AdditionalHeaders = new Dictionary<string, string> {
                           { "Authorization", $"Bearer {token}" }
                               },
                        
                     }
                 );

            return await McpClientFactory.CreateAsync(clientTransport);
        }

        public async Task<IMcpClient> CreateVesselClient()
        {
            string token = await accessTokenService.GetAccessTokenAsync();
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         UseStreamableHttp = true,
                         Endpoint = new Uri("https://localhost:7289"),
                         AdditionalHeaders =new Dictionary<string, string> {
                               { "Authorization", $"Bearer {token}" }
                               }
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }

        public async Task<IMcpClient> CreateBookingClient()
        {
            string token = await accessTokenService.GetAccessTokenAsync();
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         UseStreamableHttp = true,
                         Endpoint = new Uri("https://localhost:7044"),
                         AdditionalHeaders = new Dictionary<string, string> {
                                { "Authorization", $"Bearer {token}" }
                               }
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }
        public async Task<IMcpClient> CreateCapacityClient()
        {
            string token = await accessTokenService.GetAccessTokenAsync();
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         UseStreamableHttp = true,
                         Endpoint = new Uri("https://localhost:7061"),
                         AdditionalHeaders = new Dictionary<string, string> {
                               { "Authorization", $"Bearer {token}" }
                               }
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }
    }
}
