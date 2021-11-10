using ClientInterfaces;
using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class ClientToolStatus : IToolStatus
    {
        public string Name { get; set; } = "";
        public Guid Id { get; set; }
        public State State { get; set; }
        public List<string> Instances { get; set; } = new();
        public List<IToolInstance> InstancesUi { get; set; } = new();
    }
}
