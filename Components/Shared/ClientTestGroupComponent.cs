using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class ClientTestGroupComponent //: IClientTestGroupComponent
    {
        public ToolGroupCategory category { get; init; }
        public string Name { get; init; } = "";
        public bool isView { get; init; }
        public List<ToolComponent> Tools { get; set; } = new();
        public string instructions { get; init; } = "";
        public bool isExpanded { get; set; }
    }
}
