using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class ToolComponent 
    {
        public string Name { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string SystemTestsName { get; set; } = "";
        public Guid Id { get; set; }
        public State State { get; set; }
        public int Progress { get; set; }
        public string FailureId { get; set; } = "";
        public string Instructions { get; set; } = "";
        public List<ClientTestInstanceComponent> Instances { get; set; } = new();
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsInstructionsShown { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? TroubleshootingInfo { get; set; }
        public string? Description { get; set; }
        public bool IsInteractive { get; set; }
    }


}
