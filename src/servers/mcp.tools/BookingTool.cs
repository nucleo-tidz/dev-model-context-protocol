using ModelContextProtocol.Server;
using System.ComponentModel;

namespace mcp.tools
{
    [McpServerToolType]
    public class BookingContainerTool
    {
        [McpServerTool, Description("Create booking from origin to destination for container")]
        public  string Create(string containerType, string from, string To)
        {
            return "Booking Created";
        }


    }
}
