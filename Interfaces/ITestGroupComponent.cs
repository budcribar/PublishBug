using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Interfaces
{
    public interface ITestGroupComponent
    {
        ToolGroupCategory Category { get; set; }
        string Name { get; set; }
        bool IsView { get; set; }
        List<ITestComponent> Tools { get; set; }
    }
}
