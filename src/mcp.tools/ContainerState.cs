using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcp.tools
{
    public class ContainerStateModel
    {
        public string ContainerId { get; set; }
        public string State { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
