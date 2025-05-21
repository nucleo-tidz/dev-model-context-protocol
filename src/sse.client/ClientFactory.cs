namespace sse.client
{
    using System.Threading.Tasks;
    using ModelContextProtocol.Client;
    public class ClientFactory
    {
        public async Task<IMcpClient> Create()
        {
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         Endpoint = new Uri("https://localhost:7196/sse")
                     }
                 );
            return await McpClientFactory.CreateAsync(clientTransport);
        }
    }
}
