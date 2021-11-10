using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Interfaces
{
    public interface IToolStatus
    {
        Guid Id { get; set; }
        List<string> Instances { get; set; }
        string Name { get; set; }
        State State { get; set; }
    }
}
