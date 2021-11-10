using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace ClientInterfaces
{
    public interface IClipboard
    {
        void SetText(string text);
    }

    // TODO Because of Static
    public interface IHTMLLogger
    {
        string GetLogAsStandalone(IEnumerable<HistoryRecord> records);
    }
    public interface IUIToolPacket
    {
        bool isExpanded { get; set; }
        ToolPacket tool { get; set; }
    }
    public interface IToolPacket
    {
        string name { get; set; }
        string version { get; set; }
        string description { get; set; }
    }

    public interface ITestInstanceComponent
    {
        string id { get; set; }
        string errorCode { get; set; }
        string failureId { get; set; }
        State state { get; set; }
        int progress { get; set; }
        bool isSelected { get; set; }
        bool isExpanded { get; set; }
    }

    public interface ICustomComponentInfo
    {
        string html { get; set; }
        string js { get; set; }
        ITestContext testContext { get; set; }
    }


    public interface IInputHtmlOption
    {
        bool hidden { get; set; }
        string placeholder { get; set; }
        string type { get; set; }
        string value { get; set; }
    }

    public interface IComponentTestExecutionState
    {
        ToolStatus test { get; set; }
        State status { get; set; }
        int progress { get; set; }
    }

    public interface IResultPromptData
    {
        string? title { get; set; }
        string? trueoption { get; set; }
        string? falseoption { get; set; }
        bool exitonpass { get; set; }
        bool ignoretruecallback { get; set; }
        bool ignorefalseoption { get; set; }
        object query { get; set; } // can be a string message or a list of strings to be concat.
    }



    public interface IPromptOptions
    {
        string title { get; set; }
        string promptContent { get; set; }
        IPromptOption yesPromptOption { get; set; }
        IPromptOption noPromptOption { get; set; }
        IInputHtmlOption inputHtmlOption { get; set; }
    }

    public interface IPromptOption
    {
        string text { get; set; }
        //Action<object> callback { get; set; }
    }

    public interface IPoint
    {
        int x { get; set; }
        int y { get; set; }
    }

    public interface IFontSizeSetting
    {
        int fontSizeMod { get; set; }
        int baseFontSizePx { get; set; } // The starting application font size;
        int baseFontStepPx { get; set; } // The pixels added/subtracted from font size with each zoom-in/zoom-out action.
        int zoomScaledFontSize { get; set; } // The calculated application font size based on the current zoom-level and base font size.
        int fontZoomMod { get; set; } // The number of times the zoom modifier has been applied (This is bounded using max/min zoom).
    }

    public interface ITestContext
    {
        bool isLayoutPortrait { get; set; }
        IPoint? windowSize { get; set; }
        IPoint? canvasSize { get; set; }

        string id { get; set; }

        IFontSizeSetting? fontSizeSettings { get; set; }
        string[] languages { get; set; }

        string selectedLanguage { get; set; }
    }


    public interface ITestLogService
    {
        Task<List<DisplayedHistoryRecord2>> GetHistoryRecords();
        Task<DisplayedHistoryRecord2> GetTestLogData(string testId, string startTime, string endTime);
        Task<DisplayedHistoryRecord2> GetTestInstanceLogData(string testId, string instanceId, string startTime, string endTime);
    }

    public interface ISystemInfoEntry
    {
        string Name { get; }
        List<KeyValuePair<string, List<KeyValuePair<string, string>>>> Data { get; }
    }

    public interface IToolInstance
    {
        string Name { get; }
        bool IsSelected { get; }
        int Status { get; }
    }

    public interface IToolStatus
    {
        string Name { get; set; }
        Guid Id { get; set; }
        State State { get; set; }
        List<string> Instances { get; set; }
        List<IToolInstance> InstancesUi { get; set; }
    }

    public interface IStatus
    {
        bool Executing { get; }
        IToolStatus[] Statuses { get; }
        int RemainingTime { get; }
        int PercentComplete { get; }
        bool Cancelling { get; }
        int Timestamp { get; }
        string[] Unstaged { get; }
    }


    public interface IClientTestInstanceComponent : ITestInstanceComponent
    {
        //int Progress { get; }
        //bool IsSelected { get; set; }
        //bool IsExpanded { get; }
    }

    public interface IPage
    {
        int StartIndex { get; init; }
        int EndIndex { get; init; }
        object[] Entries { get; init; }
    }

    public interface IPaging
    {
        object[] DataSet { get; init; }
        int TotalRecords { get; init; }
        int EntriesPerPage { get; init; }
        IPage CurrentPage { get; init; }
    }

    public interface IToolComponent
    {
        string Name { get; set; }

        string GroupName { get; set; }
        string SystemTestsName { get; set; }
        Guid Id { get; set; }
        State State { get; set; }
        int Progress { get; set; }
        string FailureId { get; set; }
        string Instructions { get; set; }
        List<IClientTestInstanceComponent> Instances { get; set; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        bool IsInstructionsShown { get; set; }
        string? AdditionalInfo { get; set; }
        string? TroubleshootingInfo { get; set; }
        string? Description { get; set; }
        bool IsInteractive { get; set; }

    }
}