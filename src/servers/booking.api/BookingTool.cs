using infrastructure.Authorization;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace booking.api
{
    [McpServerToolType]
    public class BookingContainerTool(IHttpContextAccessor httpContextAccessor)
        : ToolBase(httpContextAccessor)
    {
        [McpServerTool, Description("Create booking from origin to destination for container")]
        [RequireRole("mcp.booking")]
        public string Create(string containerType, string from, string To)
        {
            base.IsAuthorized();
            return "Booking Created with Booking ID BE-9891729137";
        }
    }
}
