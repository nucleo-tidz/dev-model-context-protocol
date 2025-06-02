namespace vessel.api
{
    using ModelContextProtocol.Server;
    using System;
    using System.ComponentModel;

    [McpServerToolType]
    public class VesselTool
    {

        [McpServerTool, Description("Find vessel between origin and destination")]
        public VeeselDetail GetVessel(string origin, string destination)
        {
            if (destination.ToLower() == "bangalore")
                return null;
            return
                new VeeselDetail { VesselId = "VE-1234", VesselName = "Marina", ArrivalDate = DateTime.Now.AddDays(2), DepartureDate = DateTime.Now.AddDays(15) };
        }
    }
}
