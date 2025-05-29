using ModelContextProtocol.Server;
using System.ComponentModel;

namespace mcp.tools
{
    [McpServerToolType]
    public class BookingContainerTool
    {
        [McpServerTool, Description("Create booking from origin to destination for container")]
        public static string ContainerState(string containerId, string from, string To)
        {

            return "Booking Created";
        }


    }
}
