using Interfaces;
using System;
using System.Collections.Generic;
using ToolFrameworkPackage;

namespace Interfaces
{
    public interface IFrameworkController
    {
        string ApplicationName();
        string ApplicationPurpose();
        IEnumerable<DisplayedHistoryRecord2> GetHistory();
        IEnumerable<HistoryRecord> GetHistory(string starttime);
        DisplayedHistoryRecord2? GetHistoryRecord(Guid id, string from, string to);
        DisplayedHistoryRecord2? GetHistoryRecord(Guid id, string instance, string from, string to);
        string GetLocale();
        string GetPassId(Guid id);
#if !NET5_0_OR_GREATER
        string GetQRCode(string failureId, string testId);
#endif
        Status GetStatus();
        string? GetWarrantyCode(string failureId, string testId);
        IEnumerable<ITestGroupComponent> GroupLoad();
        string Html(Guid id);
        void Instance(Guid id, string instance);
        string Javascript(Guid id);
        IEnumerable<ToolStatus> Load();
        IEnumerable<string> LogData(Guid id, string from, string to);
        IEnumerable<string> LogData(Guid id, string instance, string from, string to);
        void SendEvent(string level, string source, bool accepted, string[] messages);
        bool SendFeedback(string helpful, string message, string subcontext = "Home");
        bool Stage(Guid id);
        bool SubmitWarrantyCode(string failureId, string testId);
        IEnumerable<string> SupportedTests();
        bool UnStage(Guid id);
    }
}