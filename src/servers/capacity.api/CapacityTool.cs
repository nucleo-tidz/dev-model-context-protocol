using infrastructure.Authorization;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace capacity.api
{
    public class CapacityTool(IHttpContextAccessor httpContextAccessor)
        : ToolBase(httpContextAccessor)
    {
        [McpServerTool, Description("How much space is left on the vessel")]
        [RequireRole("mcp.capacity")]
        public string GetVesselCapacity(string vesselId)
        {
            base.IsAuthorized();
            string[] VesselCapacity = { "100 TEU", "200 TEU", "300 TEU", "400 TEU" };
            return VesselCapacity[new Random().Next(0, 4)];
        }
    }
}
