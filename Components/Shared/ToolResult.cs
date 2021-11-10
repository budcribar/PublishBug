using Interfaces;
using Newtonsoft.Json.Linq;
using System;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class ToolResult : IToolResult
    {
        public int ResultType { get; set; }
        public int ResultCode { get; set; }
        public string ExecutableVersion { get; set; } = "";
        public string ToolVersion { get; set; } = "";
        public DateTime ApplicationStartTime { get; set; }
        public string FormattedApplicationStartTime { get; set; } = "";
        public DateTime StartTime { get; set; }
        public string FormattedStartTime { get; set; } = "";
        public DateTime StopTime { get; set; }
        public string FormattedStopTime { get; set; } = "";
        public TimeSpan ElapsedTime { get; set; }
        public string FormattedElapsedTime { get; set; } = "";
        public string FormattedResult { get; set; } = "";
        public State Result { get; set; }
        public string LogData { get; set; } = "";
        public Guid Id { get; set; }
        public string ToolName { get; set; } = "";
        public JObject? MetaData { get; set; }
        public Type? MetaDataType { get; set; }
        public string FailureId { get; set; } = "";
        public string ErrorCode { get; set; } = "";
        public string? Instance { get; set; } = null;
        public string? PassID { get; set; } = null;
        public bool Completed { get; set; }
    }
}
