using ClientInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Mocks
{
    public class TestLogService : ITestLogService
    {
        List<DisplayedHistoryRecord2> history = new List<DisplayedHistoryRecord2> { new DisplayedHistoryRecord2 { ToolName = "Tool1", Id = Guid.NewGuid(), Result = State.ExecutionPassed, StartTime = DateTime.Parse("08/03/1957 05:42:00"), StopTime = DateTime.Parse("08/03/1957 05:43:00"), LogData = "LogData1" }, new DisplayedHistoryRecord2 { ToolName = "Tool2", Id = Guid.NewGuid(), Result = State.ExecutionFailed, StartTime = DateTime.Parse("08/03/1957 05:44:00"), StopTime = DateTime.Parse("08/03/1957 05:45:00"), LogData = "LogData2" } };

        public Task<DisplayedHistoryRecord2> GetTestLogData(string testId, string startTime, string endTime)
        {
            return Task.FromResult<DisplayedHistoryRecord2>(history.Where(x => x.Id.ToString() == testId && x.StartTime == DateTime.Parse(startTime)).FirstOrDefault() ?? new());
        }

        public Task<DisplayedHistoryRecord2> GetTestInstanceLogData(string testId, string instanceId, string startTime, string endTime)
        {
            return Task.FromResult<DisplayedHistoryRecord2>(history.Where(x => x.Id.ToString() == testId && x.Instance == instanceId && x.StartTime == DateTime.Parse(startTime)).FirstOrDefault() ?? new());
        }

        public Task<List<DisplayedHistoryRecord2>> GetHistoryRecords()
        {
            return Task.FromResult(history);
        }
    }
}
