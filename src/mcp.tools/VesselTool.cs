namespace mcp.tools
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using ModelContextProtocol.Server;

    [McpServerToolType]
    public class VesselTool
    {
        [McpServerTool, Description("How much space is left on the vessel")]
        public static string GetVesselLegs(string vesselId)
        {
            string[] VesselLegs = { "100 TEU", "200 TEU", "300 TEU", "400 TEU" };
            return VesselLegs[new Random().Next(0, 4)];
        }
    }
}
