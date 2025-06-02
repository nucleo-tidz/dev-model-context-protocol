namespace shipment.client
{
    using ModelContextProtocol.Client;
    using System.Threading.Tasks;
    public class ClientFactory
    {
        public async Task<IMcpClient> CreateVesselClient()
        {
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         Endpoint = new Uri("https://localhost:7289/sse")
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }

        public async Task<IMcpClient> CreateBookingClient()
        {
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         Endpoint = new Uri("https://localhost:7044/sse")
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }
        public async Task<IMcpClient> CreateCapacityClient()
        {
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         Endpoint = new Uri("https://localhost:7061/sse")
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }
    }
}
