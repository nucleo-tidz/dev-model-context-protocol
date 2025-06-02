using ModelContextProtocol.Server;
using System.ComponentModel;

namespace capacity.api
{
    public class CapacityTool
    {
        [McpServerTool, Description("How much space is left on the vessel")]
        public string GetVesselCapacity(string vesselId)
        {
            string[] VesselCapacity = { "100 TEU", "200 TEU", "300 TEU", "400 TEU" };
            return VesselCapacity[new Random().Next(0, 4)];
        }
    }
}
