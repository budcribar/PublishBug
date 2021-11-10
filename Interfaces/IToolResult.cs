using Newtonsoft.Json.Linq;
using System;
using ToolFrameworkPackage;

namespace Interfaces
{
    public interface IToolResult
    {
        int ResultType { get; set; }
        int ResultCode { get; set; }
        string ExecutableVersion { get; set; }
        string ToolVersion { get; set; }
        DateTime ApplicationStartTime { get; set; }
        string FormattedApplicationStartTime { get; set; }
        DateTime StartTime { get; set; }
        string FormattedStartTime { get; set; }
        DateTime StopTime { get; set; }
        string FormattedStopTime { get; set; }
        TimeSpan ElapsedTime { get; set; }
        string FormattedElapsedTime { get; set; }
        string FormattedResult { get; set; }
        State Result { get; set; }
        string LogData { get; set; }
        Guid Id { get; set; }
        string ToolName { get; set; }
        JObject? MetaData { get; set; }
        Type? MetaDataType { get; set; }
        string FailureId { get; set; }
        string ErrorCode { get; set; }
        string? Instance { get; set; }
    }
}