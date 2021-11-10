using Microsoft.AspNetCore.Components;
using SureCheck.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public partial class TestResultsComponent : ComponentBase
    {
        [Inject]
        public MessageService? MessageService { get; set; }

        [Inject]
        public TestService? TestService { get; set; }


        [Parameter]
        public bool Show { get; set; } = false;

        [Parameter]
        public List<ToolComponent> testList { get; set; } = new();

        [Parameter]
        public bool isCancelled { get; set; }

        [Parameter]
        public string TestID { get; set; } = "";
        [Parameter]
        public EventCallback onCloseView { get; set; }

        [Parameter]
        public string Style { get; set; } = "";

        TestResultsDialog? testResultsDialog;

        public List<ToolResult> toolResults { get; set; } = new();
        List<bool> completed = new();

        bool hasInternetConnection = false;// TODO null; //default to null so HPSupport message doesn't show initially until connectivity has been determined
        string emptyFailureId = " - ";
        bool enabledSupport = false;
        bool clickTroubleshoot = false;
        bool clickContactSupport = false;
        bool clickMoreInformation = false;
        bool showTestsPassID = false;
        string testsPassID = "";

        string testId = "";
        //string localizedTestResult;

        //string serialNumber;
        int selectedResultIndex = -1;
        //string qrCodeValue;

        //string failureId;

        protected override async Task OnInitializedAsync()
        {
            if (TestService != null)
                this.hasInternetConnection = await TestService.CheckInternetConnection(JSRuntime);
            await base.OnInitializedAsync();
        }

        public async Task ReadHistoryRecords()
        {
            DisplayedHistoryRecord2[] logs = Array.Empty<DisplayedHistoryRecord2>();
            for (int i = 0; i < 10; i++)
            {
                logs = (await TestLogService.GetHistoryRecords()).ToArray();
                if (logs.Length > 0) break;
            }

            for (int i = this.testList.Count - 1; i >= 0; i--)
            {
                var tool = this.testList[i];

                if (tool.State == State.ReadyToInstall)
                {
                    // Tool didn't run
                    this.toolResults.Add(new ToolResult { Result = tool.State, Id = tool.Id, ToolName = tool.Name, Instance = null });
                    continue;
                }

                var selectedInstances = this.testList[i].Instances.Where(x => x.isSelected).ToArray();

                if (selectedInstances.Length > 0)
                {
                    //One or more test instances were executed, need to find log records for each.
                    foreach (var instance in selectedInstances)
                    {
                        if (true)
                        {
                            var logRecord = this.findMatchingLogRecord(this.testList[i].Id, instance.id, logs);
                            if (logRecord != null)
                            {
                                this.toolResults.Add(logRecord);
                                continue;

                            }
                        }

                        // Instance didn't run
                        this.toolResults.Add(new ToolResult { Result = tool.State, Id = tool.Id, ToolName = tool.Name, Instance = instance.id });
                    }
                }
                else
                {
                    if (true)
                    {
                        //Test itself (no specific instances selected).
                        var toolResult = this.findMatchingLogRecord(this.testList[i].Id, null, logs);
                        if (toolResult != null)
                        {
                            this.toolResults.Add(toolResult);
                            continue;
                        }
                    }

                    // If result shouldn't be loaded logs or can't be found in the logs, add record using front-end tool state.
                    this.toolResults.Add(new ToolResult { Result = tool.State, Id = tool.Id, ToolName = tool.Name, Instance = null });
                }
            }

            this.toolResults.Reverse();

            completed.AddRange(Enumerable.Repeat(false, this.toolResults.Count));

            this.enabledSupport = false;
            this.clickTroubleshoot = false;
            this.clickContactSupport = false;
            this.clickMoreInformation = false;
            this.showTestsPassID = this.ShowTestsPassID(this.toolResults);

            // Determine if any tests failed
            if (this.toolResults.Count > 0)
            {
                foreach (var toolResult in this.toolResults)
                {
                    if (!string.IsNullOrEmpty(toolResult.FailureId) &&
                        toolResult.FailureId != this.emptyFailureId &&
                        model.getStateName(toolResult.Result).ToLower() == "failed")
                    {
                        this.enabledSupport = true;
                        break;
                    }
                }
                foreach (var toolResult in this.toolResults)
                {
                    if (model.isInteractiveTest(toolResult.Id.ToString()) &&
                        !string.IsNullOrEmpty(toolResult.FailureId) &&
                        toolResult.FailureId != this.emptyFailureId &&
                        model.getStateName(toolResult.Result).ToLower() == "failed")
                    {
                        this.clickTroubleshoot = true;
                        break;
                    }
                }
                foreach (var toolResult in this.toolResults)
                {
                    if (!model.isInteractiveTest(toolResult.Id.ToString()) &&
                        !string.IsNullOrEmpty(toolResult.FailureId) &&
                        toolResult.FailureId != this.emptyFailureId &&
                        model.getStateName(toolResult.Result).ToLower() == "failed")
                    {
                        this.clickContactSupport = true;
                        break;
                    }
                }
                foreach (var toolResult in this.toolResults)
                {
                    if (model.getStateName(toolResult.Result).ToLower() == "passed")
                    {
                        this.clickMoreInformation = true;
                        break;
                    }
                }
            }
        }

        bool ShowTestsPassID(List<ToolResult> toolResults)
        {
            if (string.IsNullOrEmpty(this.testId))
                return false;


            foreach (var toolResult in toolResults)
            {
                if (!string.IsNullOrEmpty(toolResult.FailureId) ||
                    model.getStateName(toolResult.Result).ToLower() != "passed")
                {
                    return false;
                }
            }

            testsPassID = PassId.GetPassId(Guid.Parse(this.testId));

            return true;
        }

        private ToolResult? findMatchingLogRecord(Guid testId, string? instanceId, DisplayedHistoryRecord2[] logs)
        {
            foreach (var log in logs)
            {
                if (log.Id == testId)
                {

                    return new ToolResult { Result = log.Result, Id = log.Id, ToolName = log.ToolName, Instance = log.Instance, PassID = log.PassID, FailureId = log.FailureId, ApplicationStartTime = log.ApplicationStartTime, ElapsedTime = log.ElapsedTime, ErrorCode = log.ErrorCode, LogData = log.LogData, ResultType = log.ResultType, ExecutableVersion = log.ExecutableVersion, MetaData = log.MetaData, MetaDataType = log.MetaDataType, ResultCode = log.ResultCode, ToolVersion = log.ToolVersion };

                }
            }
            return null;
        }

        public void popModalTroubleshoot(int index)
        {
            this.selectedResultIndex = index; // Identify the result for which the steps are being taken
            testResultsDialog?.popModalTroubleshoot(this.toolResults[index]);
        }

        public void popModalContactHPSupport(int index)
        {
            this.selectedResultIndex = index; // Identify the result for which the steps are being taken
            testResultsDialog?.popModalContactHPSupport(this.toolResults[index]);
        }
        public void popModalMoreInformation(int index)
        {
            this.selectedResultIndex = index;
            testResultsDialog?.popModalMoreInformation(this.toolResults[index]);
        }
        private void updateTroubleshootStatus()
        {
            this.toolResults[this.selectedResultIndex].Completed = true;
            this.clickTroubleshoot = false;

            foreach (var toolResult in this.toolResults)
            {
                if (!toolResult.Completed &&
                    model.isInteractiveTest(toolResult.Id.ToString()) &&
                    !string.IsNullOrEmpty(toolResult.FailureId) &&
                    toolResult.FailureId != this.emptyFailureId &&
                    model.getStateName(toolResult.Result).ToLower() == "failed")
                {
                    this.clickTroubleshoot = true;
                    return;
                }
            }
        }

        private void updateContactSupportStatus()
        {

            this.toolResults[this.selectedResultIndex].Completed = true;
            this.clickContactSupport = false;
            foreach (var toolResult in this.toolResults)
            {
                if (!toolResult.Completed &&
                    !model.isInteractiveTest(toolResult.Id.ToString()) &&
                    !string.IsNullOrEmpty(toolResult.FailureId) &&
                    toolResult.FailureId != this.emptyFailureId &&
                    model.getStateName(toolResult.Result).ToLower() == "failed")
                {
                    this.clickContactSupport = true;
                    return;
                }
            }
        }

        public void closeView()
        {
            this.testList = new();

            //reset this back to null before closing any instance
            if (MessageService != null)
                MessageService.Message = "";

            this.onCloseView.InvokeAsync();
        }
    }
}
