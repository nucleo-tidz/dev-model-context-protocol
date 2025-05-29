namespace mcp.tools
{
    using ModelContextProtocol.Server;
    using System;
    using System.ComponentModel;

    [McpServerToolType]
    public class ShipmentContainerTool
    {
        [McpServerTool, Description("Check the state of shipment contianer")]
        public static ContainerStateModel ContainerState(string containerId)
        {
            string[] containerStates = { "Sound", "Damaged" };
            ContainerStateModel containerStateModel = new()
            {
                ContainerId = containerId,
                LastUpdate = DateTime.Now,
                State = containerStates[new Random().Next(0, 1)]
            };
            return containerStateModel;
        }

        [McpServerTool, Description("Check the type of shipment contianer")]
        public static string ContainerType(string containerId)
        {
            string[] containerType = { "DRY", "REEF", "SPECIAL" };
            return containerType[new Random().Next(0, 1)];
        }
    }
}
