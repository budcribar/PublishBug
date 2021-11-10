using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Mocks
{
    public class History : IHistory
    {
        IEnumerable<HistoryRecord> IHistory.Records => new List<HistoryRecord>();

        Task IHistory.Flush()
        {
            return Task.CompletedTask;
        }

        bool? IHistory.GetLastResult(Guid id) => false;


        IEnumerable<HistoryRecord> IHistory.GetRecords(ISessionContext session) => new List<HistoryRecord>();


        IEnumerable<HistoryRecord> IHistory.GetRecords(DateTime start, DateTime end) => new List<HistoryRecord>();


        IHistory IInitialize<IHistory>.Initialize()
        {
            return new History();
        }

        void IHistory.LogReading(IEnumerable<ITestResult> tools, ISessionContext session)
        {

        }
    }
}
