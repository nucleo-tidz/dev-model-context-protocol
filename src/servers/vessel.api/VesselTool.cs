namespace vessel.api
{
    using infrastructure.Authorization;
    using ModelContextProtocol.Server;
    using System;
    using System.ComponentModel;

    [McpServerToolType]

    public class VesselTool(IHttpContextAccessor httpContextAccessor)
        : ToolBase(httpContextAccessor)
    {
        [McpServerTool, Description("Find vessel between origin and destination")]
        [RequireRole("mcp.vessel")]
        public VeeselDetail GetVessel(string origin, string destination)
        {
            base.IsAuthorized();
            if (destination.ToLower() == "bangalore")
                return null;
            return
                new VeeselDetail { VesselId = "VE-1234", VesselName = "Marina", ArrivalDate = DateTime.Now.AddDays(2), DepartureDate = DateTime.Now.AddDays(15) };
        }
    }
}
