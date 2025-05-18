namespace mcp.tools
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using ModelContextProtocol.Server;

    [McpServerToolType]
    public class VesselTool
    {
        [McpServerTool, Description("Check the number of legs for vesse")]
        public static string GetVesselLegs(string vesselId)
        {
            string[] VesselLegs = { "1", "2","3","4" };
            return VesselLegs[new Random().Next(0, 4)];
        }
    }
}
