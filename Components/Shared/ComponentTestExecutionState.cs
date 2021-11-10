using ClientInterfaces;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class ComponentTestExecutionState : IComponentTestExecutionState
    {
        public ToolStatus test { get; set; } = new();
        public string group { get; set; } = "";
        public State status { get; set; }
        public int progress { get; set; }
    }
}
