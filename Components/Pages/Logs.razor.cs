using Components.Shared;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Pages
{
    public partial class Logs
    {
        private Modal? modal { get; set; }

        class Page
        {
            public int StartIndex { get; set; } = 0;
            public int EndIndex { get; set; } // Index of 1 element past the current segment

            public ArraySegment<DisplayedHistoryRecord2>? Entries { get; set; }
        }

        class Paging
        {
            public DisplayedHistoryRecord2[] DataSet { get; init; } = Array.Empty<DisplayedHistoryRecord2>();
            public int TotalRecords { get; init; } = 0;
            public int EntriesPerPage { get; init; } = 10;
            public Page CurrentPage = new Page();
        }

        bool isLoading = true;
        DisplayedHistoryRecord2[] testLogs = Array.Empty<DisplayedHistoryRecord2>();
        bool enabledSupport = false;
        string? selectedTestLogData = null;
        DisplayedHistoryRecord2? selectedHistoryRecord = null;
        string logDetailHeader = "";
        string emptyFailureId = " - ";
        public bool? hasInternetConnection = null; //default to null so HPSupport message doesn't show initially until connectivity has been determined
        Paging paging = new Paging();
        public bool ShowBackdrop = false;

        void CheckForEnter(KeyboardEventArgs args, Action action)
        {
            if (args.Code == "Enter")
                action.Invoke();
        }

        public async void ExportLogs()
        {
            var html = await model.LogFile();
            await Interop.SaveAs(JSRuntime, "logs.html", html);
        }

        void pagingPageChanged(bool isNextPage)
        {
            var currentStart = paging.CurrentPage.StartIndex;
            var currentEnd = paging.CurrentPage.EndIndex;
            int newStart;
            int newEnd;
            if (isNextPage)
            {
                if (paging.CurrentPage.EndIndex == paging.TotalRecords)
                    return;
                newStart = currentStart + this.paging.EntriesPerPage;
                newEnd = currentEnd + this.paging.EntriesPerPage;
            }
            else
            {
                if (paging.CurrentPage.StartIndex == 0)
                    return;
                newStart = currentStart - this.paging.EntriesPerPage;
                newEnd = newStart + this.paging.EntriesPerPage;
            }

            //Bounds Check
            newStart = newStart < 0 ? 0 : newStart;
            newStart = newStart >= this.paging.TotalRecords ? this.paging.TotalRecords : newStart;
            newEnd = newEnd >= this.paging.TotalRecords ? this.paging.TotalRecords : newEnd;
            newEnd = newEnd <= this.paging.EntriesPerPage ? this.paging.EntriesPerPage : newEnd;
            paging.CurrentPage.StartIndex = newStart;
            paging.CurrentPage.EndIndex = newEnd;
            if (paging.DataSet != null)
                paging.CurrentPage.Entries = new ArraySegment<DisplayedHistoryRecord2>(paging.DataSet, newStart, newEnd - newStart);
        }

        async Task showLogDetails(DisplayedHistoryRecord2 testLog)
        {
            ///<summary>
            /// Calls server to load detail information for the specified test/test-instance
            /// execution and displays the results in a modal dialog.
            ///</summary>
            this.selectedTestLogData = null;
            this.selectedHistoryRecord = null;
            this.logDetailHeader = Localizer.STR_BUTTON_TEST_LOGS + this.emptyFailureId + testLog.ToolName + (string.IsNullOrEmpty(testLog.Instance) ? this.emptyFailureId + testLog.Instance : "");
            var res = testLog;
            //var res =  await this.loadLogEntryDetails(testLog);
            this.selectedTestLogData = string.IsNullOrEmpty(res.LogData) ? Localizer.STR_HELP_EMPTY_LOG_DETAIL : res.LogData;
            this.selectedHistoryRecord = res;
            await modal!.Open();
        }

        async Task hideLogDetails()
        {
            await modal!.Close();
            this.selectedTestLogData = null;
            StateHasChanged();
        }

        async Task getLogs()
        {
            testLogs = (await TestLogService.GetHistoryRecords()).ToArray();
            //var validFailureId = @"/[A-Z\d]{6,6}-[A-Z\d]{6,6}-[A-Z\d]{6,6}-[A-Z\d]{6,6}/";
            foreach (var logEntry in testLogs)
            {
                logEntry.FailureId = logEntry.FailureId == null ? this.emptyFailureId : logEntry.FailureId;
            }

            this.initPaging();
            this.isLoading = false;
            // see if we need to show the HP Support message component
            this.enabledSupport = false;
            if (this.testLogs.Length > 0)
            {
                for (var i = 0; i < this.testLogs.Length; i++)
                {
                    var toolResult = this.testLogs[i];
                    if (model.getStateName(toolResult.Result).ToLower() == "failed")
                    {
                        this.enabledSupport = true;
                        break;
                    }
                }
            }
        }

        void initPaging()
        {
            paging = new Paging { DataSet = testLogs, TotalRecords = testLogs.Length };
            paging.CurrentPage.EndIndex = paging.EntriesPerPage > testLogs.Length ? testLogs.Length : paging.EntriesPerPage;
            if (paging.DataSet != null)
                paging.CurrentPage.Entries = new ArraySegment<DisplayedHistoryRecord2>(paging.DataSet, paging.CurrentPage.StartIndex, paging.CurrentPage.EndIndex - paging.CurrentPage.StartIndex);
        }

        string getStateClass(State testState)
        {
            if (model.getStateName(testState) == "Failed")
                return "min-width centered=true table-danger";
            if (model.getStateName(testState) == "Passed")
                return "min-width centered=true table-success";
            if (model.getStateName(testState) == "Cancelled")
                return "min-width centered=true table-warn";
            return "";
        }

        protected async override void OnInitialized()
        {
            var selectedLanguage = Context.Language;
            await LoadTask;
            await getLogs();

            // check connectivity after logs to avoid sometimes not showing message on initial view load
            this.hasInternetConnection = await TestService.CheckInternetConnection(JSRuntime);

            base.OnInitialized();
            // Need to refresh in case the tool load delay causes a refresh timeout
            StateHasChanged();
        }

        string pagingInfoString => paging.CurrentPage.StartIndex + 1 + "-" + this.paging.CurrentPage.EndIndex + " / " + this.paging.TotalRecords;
        Task<DisplayedHistoryRecord2> loadLogEntryDetails(DisplayedHistoryRecord2 testLog)
        {
            ///<summary>
            /// Load log data for specific Test or Test Instance run.
            ///</summary>
            if (string.IsNullOrEmpty(testLog.Instance))
                return TestLogService.GetTestLogData(testLog.Id.ToString(), testLog.StartTime.ToString(), testLog.StopTime.ToString());
            return TestLogService.GetTestInstanceLogData(testLog.Id.ToString(), testLog.Instance, testLog.StartTime.ToString(), testLog.StopTime.ToString());
        }
    }
}