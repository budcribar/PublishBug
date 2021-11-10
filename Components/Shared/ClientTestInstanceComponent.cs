using ClientInterfaces;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class ClientTestInstanceComponent : IClientTestInstanceComponent
    {
        public string id { get; set; } = "";
        public string errorCode { get; set; } = "";
        public string failureId { get; set; } = "";
        public State state { get; set; }


        public int progress { get; set; }

        //public int Progress { get; set; }

        public bool isSelected { get; set; }
        //public bool IsSelected { get; set; }
        public bool isExpanded { get; set; }

        //public bool IsExpanded { get; set; }
    }
}
