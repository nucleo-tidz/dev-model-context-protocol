namespace client
{
    using System.Threading.Tasks;

    using ModelContextProtocol.Client;

    public class ClientFactory
    {
        public async Task<IMcpClient> Create()
        {
            var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
            {
                Name = "nucleotidz-mcp-server",                
                Command = "dotnet",
                Arguments = ["run", "--project", "C:\\Workspace\\nucleo-tidz\\dev-model-context-protocol\\src\\server", "--no-build"],
                
            });
            return await McpClientFactory.CreateAsync(clientTransport);
        }
    }
}
