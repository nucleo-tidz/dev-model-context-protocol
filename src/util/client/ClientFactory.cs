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
                Arguments = ["run", "--project", "C:\\Personal-Workspace\\MCP\\src\\util\\server", "--no-build"],
                
            });
            return await McpClientFactory.CreateAsync(clientTransport);
        }
    }
}
