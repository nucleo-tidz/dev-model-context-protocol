using ModelContextProtocol.Server;
using System.ComponentModel;

namespace mcp.tools
{
    public class CapacityTool
    {
        [McpServerTool, Description("How much space is left on the vessel")]
        public string GetVesselLegs(string vesselId)
        {
            string[] VesselLegs = { "100 TEU", "200 TEU", "300 TEU", "400 TEU" };
            return VesselLegs[new Random().Next(0, 4)];
        }
    }
}
