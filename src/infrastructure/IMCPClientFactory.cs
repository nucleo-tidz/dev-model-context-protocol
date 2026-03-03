using ModelContextProtocol.Client;

namespace infrastructure
{
    public interface IMCPClientFactory
    {
        Task<McpClient> CreateContainerClient();
         Task<McpClient> CreateVesselClient();
        Task<McpClient> CreateBookingClient();
        Task<McpClient> CreateCapacityClient();
    }
}
