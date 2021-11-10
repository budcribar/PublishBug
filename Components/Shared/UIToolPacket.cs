using ClientInterfaces;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class UIToolPacket : IUIToolPacket
    {
        public bool isExpanded { get; set; }
        public ToolPacket tool { get; set; }

        public UIToolPacket(bool isExpanded, ToolPacket tool)
        {
            this.isExpanded = isExpanded;
            this.tool = tool;
        }
    }
}
