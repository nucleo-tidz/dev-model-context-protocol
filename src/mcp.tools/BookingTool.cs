using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcp.tools
{
    [McpServerToolType]
    public class BookingContainerTool
    {
        [McpServerTool, Description("Create booking from origin to destination for container")]
        public static string ContainerState(string containerId,string from,string To)
        {
           
            return "Booking Created";
        }

       
    }
}
