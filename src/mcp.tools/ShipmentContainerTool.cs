namespace mcp.tools
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using ModelContextProtocol.Server;

    [McpServerToolType]
    public class ShipmentContainerTool
    {
        [McpServerTool, Description("Check the state of shipment contianer")]
        public static string ContainerState(string containerId)
        {
            string[] containerStates = { "Sound", "Damaged" };
            return containerStates[new Random().Next(0, 1)];
        }

        [McpServerTool, Description("Check the type of shipment contianer")]
        public static string ContainerType(string containerId)
        {
            string[] containerType = { "DRY", "REEF", "SPECIAL" };
            return containerType[new Random().Next(0, 1)];
        }
    }
}
