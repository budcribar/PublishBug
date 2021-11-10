using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Interfaces
{
    public enum TestProperties
    {
        Generic = 0,
        IncludedInFastSystemTest,
        IncludedInExtendedSystemTest,
        Stressor,
        Interactive,
    }

    public interface ITestComponent
    {
        /// <summary>
        /// The name of the tool
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The ID of the tool
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// Tool is running or not running.
        /// </summary>     
        State State { get; set; }
        /// <summary>
        /// The current state the tool is in. Passed, Failed, Cancelled or InstallationPassed
        /// </summary>
        State LastResult { get; set; }
        List<ITestInstanceComponent> Instances { get; set; }
        //List<TestProperties> Properties { get; set; }
        string Instructions { get; set; }
        string Description { get; set; }
        string AdditionalInfo { get; set; }
        string TroubleshootingInfo { get; set; }
    }
}
