using Interfaces;
using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Mocks
{
    public class FrameworkController : IFrameworkController
    {
        public string ApplicationName()
        {
            throw new NotImplementedException();
        }

        public string ApplicationPurpose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DisplayedHistoryRecord2> GetHistory()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HistoryRecord> GetHistory(string starttime)
        {
            throw new NotImplementedException();
        }

        public DisplayedHistoryRecord2? GetHistoryRecord(Guid id, string from, string to)
        {
            throw new NotImplementedException();
        }

        public DisplayedHistoryRecord2? GetHistoryRecord(Guid id, string instance, string from, string to)
        {
            throw new NotImplementedException();
        }

        public string GetLocale()
        {
            throw new NotImplementedException();
        }

        public string GetPassId(Guid id)
        {
            throw new NotImplementedException();
        }

        public string GetQRCode(string failureId, string testId)
        {
            throw new NotImplementedException();
        }

        public Status GetStatus()
        {
            throw new NotImplementedException();
        }

        public string? GetWarrantyCode(string failureId, string testId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITestGroupComponent> GroupLoad()
        {
            return new List<ITestGroupComponent>();
        }

        public string Html(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Instance(Guid id, string instance)
        {
            throw new NotImplementedException();
        }

        public string Javascript(Guid id)
        {
            return "";
        }

        public IEnumerable<ToolStatus> Load()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LogData(Guid id, string from, string to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LogData(Guid id, string instance, string from, string to)
        {
            throw new NotImplementedException();
        }

        public void SendEvent(string level, string source, bool accepted, string[] messages)
        {
            throw new NotImplementedException();
        }

        public bool SendFeedback(string helpful, string message, string subcontext = "Home")
        {
            return true;
        }

        public bool Stage(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool SubmitWarrantyCode(string failureId, string testId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> SupportedTests()
        {
            throw new NotImplementedException();
        }

        public bool UnStage(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
