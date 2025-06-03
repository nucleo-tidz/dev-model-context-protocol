
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace reposition.server
{
    [McpServerToolType]
    public class RespositionTool
    {
        [McpServerTool, Description("Get the total number of containers planned in a repositioning plan based on plan id")]
        public static string GetContainers(string planId)
        {
            return new Random().Next(1, 100).ToString();
        }

        [McpServerTool, Description("Get the type of container like 20DRY in a repositioning plan based on plan id ")]
        public static string ContainerTypes(string planId)
        {
            string[] containerTypes = ["20DRY", "40DRY"];
            return new Random().Next(1, 2).ToString();
        }
    }
}
