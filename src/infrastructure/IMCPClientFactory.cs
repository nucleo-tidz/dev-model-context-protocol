using ModelContextProtocol.Client;

namespace infrastructure
{
    public interface IMCPClientFactory
    {
        Task<IMcpClient> CreateContainerClient();
        Task<IMcpClient> CreateVesselClient();
        Task<IMcpClient> CreateBookingClient();
        Task<IMcpClient> CreateCapacityClient();
    }
}
