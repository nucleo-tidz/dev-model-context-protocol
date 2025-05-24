using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace function.call
{
    public class RepositionPlugin
    {
        [KernelFunction, Description("Get the total number of containers planned in a repositioning plan based on plan id")]
        public static string GetContainers(string planId)
        {
            return new Random().Next(1, 100).ToString();
        }
    }
}
