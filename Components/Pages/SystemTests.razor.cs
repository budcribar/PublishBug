using ClientInterfaces;
using Components.Custom;
using Components.Shared;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Pages
{
    public partial class SystemTests
    {
        CustomComponentTestComponent? customComponent;
        DefaultComponentTestComponent? defaultComponent;
        TestResultsComponent? testResultsComponent;

        //bool IS_GROUP_EXPANDED_DEFAULT = false; // True all tests for group will show, false all the respective test group's children are hidden.
        //bool IS_TEST_EXPANDED_DEFAULT = false; // True all instances for test will show, false all the respective test's children are hidden.
        int POLL_TEST_STATUS_INTERVAL_MS = 2000; // Minimum interval between test status polling calls.
        bool _IsExecuting = false; // Should only be set by test status monitoring logic based on IsExecuting state of server.
        bool isLoading = false; // Determines if data is currently being retrieved from the server. Used to display/hide UI elements.
                                //    statusSubscription: Subscription; // Reference to polling subscription needed to unsubscribe when execution completes.
        Status? latestStatusSnapshot; // Most recent status update response object.
                                      //string? errorText = null;
        ToolComponent? currentTestExecuting = null;
        public static ComponentTest? testViewComponent; // Store the current test specific view template. Generic type used to execute all test types.
                                                        //bool IsExecuting = false; // Should only be set by test status monitoring logic based on IsExecuting state of server.
        bool testExecutionComplete = false;
        //    // NEW GROUPING VARIABLES
        List<ClientTestGroupComponent> masterTestList = new(); // Contains the origional set of test groups retrieved from the server.
        List<ClientTestGroupComponent> testViews = new(); // Stores pre-defined view templates for quick test set execution.
        List<ToolComponent> executionTestList = new(); // List of tests selected by the user for execution.
        List<ToolComponent> selectedTests = new();


        ClientTestGroupComponent? selectedTestView;
        string? selectedViewInstructions;
        bool isMasterSelectChecked = false;
        bool _isCancelled = false;

        void CheckForEnter(KeyboardEventArgs args, Action action)
        {
            if (args.Code == "Enter")
                action.Invoke();
        }

        void selectTestView(ClientTestGroupComponent? viewToSelect)
        {
            if (viewToSelect == null)
            {
                this.selectedTests = new();
                this.selectedTestView = null;
                this.selectedViewInstructions = null;
                return;
            }
            if (viewToSelect.Name == Localizer.STR_TITLE_SYSTEM_FAST_TEST)
            {
                this.selectedViewInstructions = Localizer.STR_HELP_SYSTEM_FAST_TEST1 + "<br>" +
                                                Localizer.STR_HELP_FOLLOWING_TESTS_PERFORMED; //"The Quick System Test will perform a 10 to 15 minute check of the system, and will run the following tests:";
            }
            else if (viewToSelect.Name == Localizer.STR_TITLE_SYSTEM_EXTENSIVE_TEST)
            {
                this.selectedViewInstructions = Localizer.STR_HELP_SYSTEM_EXTENSIVE_TEST1 + "<br>" +
                                                Localizer.STR_HELP_SYSTEM_EXTENSIVE_TEST2 + "<br>" +
                                                Localizer.STR_HELP_SYSTEM_TEST5 + "<br>" +
                                                Localizer.STR_HELP_SYSTEM_EXTENSIVE_TEST_DISABLE_SLEEP + "<br>" +
                                                Localizer.STR_HELP_FOLLOWING_TESTS_PERFORMED; //"The Extensive Test will provide a longer and more comprehensive test of the hardware sub-systems.  It will take one or more hours to complete, and will run the following tests:";
            }

            else
            {
                this.selectedViewInstructions = Localizer.STR_HELP_COMPONENT_TESTS;
            }

            if (this.selectedTestView != null)
            {
                //If view already selected, clear it's selections before adding selections for new view.
                foreach (var tool in this.selectedTestView.Tools)
                {
                    tool.IsSelected = false;
                }
            }

            this.selectedTestView = viewToSelect;
            this.selectedTests = new();

            //Pre-select all tests if Quick or Extensive views.
            if (viewToSelect.isView)
            {
                this.selectedTestView?.Tools.ForEach((tool) => { tool.IsSelected = true; });
            }

            //Copy done to prevent copying origional array pointer reference.
            this.selectedTests = this.selectedTestView?.Tools.Where((tool) => { return tool.IsSelected; }).ToList() ?? new();
        }

        async void executeComponentTests()
        {
            ///<summary>
            /// Attempt to start executing the list of user selected tests.
            /// This method will do nothing if called while test execution is already in progress.
            ///</summary>
            if (this.IsExecuting)
                return;
            //Reset test executing variables. This is needed for consecutive test set executions.
            this.currentTestExecuting = null;
            this.executionTestList = new();
            foreach (var group in masterTestList)
            {
                foreach (var tool in group.Tools)
                {
                    tool.State = State.ReadyToInstall;
                    foreach (var instance in tool.Instances)
                        instance.state = State.ReadyToInstall;
                }
            }

            //Done to make sidebar hide sooner, seemed to lag when actually waiting on test polling to start before setting IsExecuting.
            this._IsExecuting = true;
            //this.testService.IsExecuting = true;
            //Need to create new component test object which contains UI specific properties
            //(properties or data structure which doesn't exist on model returned from the server because they are UI specific properties).

            foreach (var selectedTest in this.selectedTests)
            {
                var copy = ((ToolComponent)selectedTest)?.DeepCopy<ToolComponent>();

                if (copy != null)
                {
                    copy.State = State.ReadyToInstall;
                    this.executionTestList.Add(copy);
                }

            }
            //Begin executing all selected tests in the order in which they were selected.
            await this.executeTestsInSeq();
        }

        void toggleViewsTestSelections(ClientTestGroupComponent view)
        {

            //
            if (view?.Tools?.Count() > 0)
            {

                this.isMasterSelectChecked = !this.isMasterSelectChecked;
                view.Tools.ForEach((tool) =>
                {
                    this.selectTest(tool, this.isMasterSelectChecked, true);
                });
            }
        }

        void selectTest(ToolComponent test, bool value, bool checkIndeterminate)
        {
            ///<summary>
            /// This method attempts to select/deselect the specified test and any test instances belonging to the respective test.
            /// Test select/deselect logic is disabled during test execution to prevent potentially problematic changes to the
            /// testing set after test execution starts.
            ///</summary>
            if (this.IsExecuting)
                return;
            test.IsSelected = value;
            if (test.IsSelected)
                test.IsExpanded = true;
            //Propsgate selection state to instances belonging to the specified test.
            foreach (var instance in test.Instances)
            {
                this.selectTestInstance(instance, test, value);
            }
            //Remove the test from the selected list if it was previously selected and no has been deselected.
            for (int i = 0; i < this.selectedTests.Count(); i++)
            {
                if (this.selectedTests[i].Id == test.Id)
                {
                    if (!value)
                        this.selectedTests.RemoveAt(i);
                    return;
                }
            }
            if (value)
                this.addSelectedTestHelper(test);
        }

        public void selectTestInstance(IClientTestInstanceComponent testInstance, ToolComponent parentTest, bool value)
        {
            ///<summary>
            /// This method attempts to select/deselect the specified test instance AND the test it belongs to.
            /// Test instance select/deselect logic is disabled during test execution to prevent potentially problematic changes
            /// to the selected testing set after test executon starts.
            ///</summary>
            if (this.IsExecuting)
                return;

            testInstance.isSelected = value;

            //When an instance is selected, the test it belongs to needs to be selected for the instance to be picked up for testing.
            if (testInstance.isSelected)
            {
                parentTest.IsSelected = true;
                this.addSelectedTestHelper(parentTest);
            }
        }

        private void addSelectedTestHelper(ToolComponent test)
        {
            ///<summary>
            /// Helper method to add a test to the selected test list ONLY IF it doesn't already exist in the list of selected tests.
            ///</summary>
            for (int i = 0; i < this.selectedTests.Count; i++)
            {
                if (this.selectedTests[i].Id == test.Id)
                {
                    return;
                }
            }
            this.selectedTests.Add(test);
        }

        bool hasTestsSelected => this.selectedTests.Count > 0;

        bool IsExecuting
        {
            get
            {
                return this._IsExecuting || this.currentTestExecuting != null || (this.latestStatusSnapshot != null && this.latestStatusSnapshot.Executing);
                ;
            }
        }

        bool IsCancelling => latestStatusSnapshot?.Cancelling ?? false;
        bool IsCancelled
        {
            get
            {
                if (!this._isCancelled && (this.latestStatusSnapshot == null || this.latestStatusSnapshot.Statuses == null))
                    return false;
                return this._isCancelled;
            }
        }

        bool IsCustomTest => testViewComponent is CustomComponentTestComponent;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await componentLoad();
        }

        private async Task<bool> componentLoad()
        {
            ///<summary>
            /// Attempts to load the available set of diagnostic component tests from the server.
            /// Test and Test instance objects returned from the server are wrapped in UI specific
            /// data models to allow UI specific properties to be added and used (isSelected, isExpanded ...).
            ///</summary>
            this.isLoading = true;
            this.testViews = new();
            this.masterTestList = new();
            this.selectedTests = new();
            this._isCancelled = false;

            await LoadTask;

            isLoading = false;
            this._isCancelled = false;
            var groups = FrameworkController.GroupLoad();

            foreach (var group in groups)
                group.Tools = group.Tools.Where(x => x.State != State.InstallationFailed && x.State != State.PreConditionsFailed && x.State != State.PostConditionsFailed).ToList();

            var testGroups = groups.Where(g => !g.IsView && g.Tools.Count > 0);

            testViews = groups.Where(x => x.IsView && x.Tools.Count > 0).Select(g => new ClientTestGroupComponent { Name = g.Name, Tools = g.Tools.Select(x => new ToolComponent { Name = x.Name, GroupName = x.Name, Id = x.Id, Description = x.Description, AdditionalInfo = x.AdditionalInfo }).ToList() }).ToList();

            foreach (var g in testGroups)
                masterTestList.Add(new ClientTestGroupComponent { Name = g.Name, Tools = g.Tools.Select(x => new ToolComponent { Name = x.Name, GroupName = x.Name, Id = x.Id, Description = x.Description, AdditionalInfo = x.AdditionalInfo, Instances = x.Instances.Select(x => new ClientTestInstanceComponent { errorCode = x.ErrorCode, failureId = x.FailureId, id = x.Id, state = x.State }).ToList(), Instructions = x.Instructions, State = x.State }).ToList() });

            selectTestView(testViews.Count > 0 ? testViews[0] : null);

            this.testExecutionComplete = false;

            // TODO isExecuting
            //this.testService.isExecuting = false;

            foreach (var group in this.masterTestList)
            {
                foreach (var tool in group.Tools)
                {
                    tool.State = State.InstallationPassed;
                }
            }

            return true;
        }

        public async void onTestResultViewClosed()
        {
            //Reset view to test selection screen.
            this.testExecutionComplete = false;
            // this.testService.IsExecuting = false; //Allow other application components (like side nav panel) to react to test executing status.
            await this.componentLoad();
        }

        private async Task executeTestsInSeq()
        {
            ///<summary>
            /// Executes each test/test instance in the list of selected tests in the order in which the
            /// tests were selected by the user. Utilizes class property currentTestExecuting and method
            /// getNextTestToExecute() to execute tests sequentially.
            ///</summary>
            ///
            this.currentTestExecuting = this.getNextTestToExecute();
            if (this.currentTestExecuting == null)
            {
                //Test Execution Complete.
                this._IsExecuting = false;
                if (!this.IsCancelled)
                    this.testExecutionComplete = true;

                if (this.testResultsComponent != null)
                    await this.testResultsComponent.ReadHistoryRecords();

                await InvokeAsync(StateHasChanged);
                return;
            }

            //Reset test to execute status for new execution run.
            var testToExe = this.currentTestExecuting;
            this.currentTestExecuting.State = State.InstallationPassed;
            this.currentTestExecuting.Progress = 0;

            ClientInterfaces.IToolStatus freshTestStatus = new ClientToolStatus
            {
                Name = this.currentTestExecuting.Name,
                State = this.currentTestExecuting.State,
                Id = this.currentTestExecuting.Id,
            };

            this.updateTestStatus(freshTestStatus.Name, freshTestStatus.State); //Reset test state, used for consecutive executions of a test.
                                                                                //Only custom tests will result in a javascript file returned from server. Use this to determine component test type.
            string? jsSource = Framework.Tool(this.currentTestExecuting.Id).GuiJavascript;
            testViewComponent = string.IsNullOrEmpty(jsSource) ? this.defaultComponent : this.customComponent;

            // Sanity check
            if (testViewComponent == null)
                return;

            testViewComponent.TestList = this.executionTestList;
            // TODO this is set in ComponentTests
            //this._IsExecuting = true;

            await InvokeAsync(StateHasChanged);
            testViewComponent?.StartTest(this.currentTestExecuting);
        }

        private ToolComponent? getNextTestToExecute()
        {
            ///<summary>
            /// Helper method used to get the next test to be executing in the set of selected tests based
            /// on the currentTestExecuting reference. Returns next item in list after the current currentTestExecuting.
            /// This should only be called when actually retrieving the next test to execute.
            ///</summary>
            var testsToExecute = this.executionTestList;
            if (testsToExecute.Count() == 0 || this.IsCancelled)
                return null;
            if (this.currentTestExecuting == null)
                return testsToExecute[0]; //Starting at beggining of execution seq, start at index 0.
            else
            {
                //Already executing and need to find next test to execute in sequence.
                for (var i = 0; i < testsToExecute.Count; i++)
                {
                    var test = testsToExecute[i];
                    if (test.Id == this.currentTestExecuting.Id)
                    {
                        if (i + 1 < testsToExecute.Count)
                        {
                            return testsToExecute[i + 1];
                        }

                        return null;
                    }
                }
            }

            return null;
        }
        public void handleTestStart()
        {
            ///<summary>
            /// Called when child components reports that it has complete test execution startup logic.
            ///</summary>
            ///
            // TODO Polling Hack
            this.pollTestStatuses();
        }


        private async void pollTestStatuses()
        {
            ///<summary>
            /// Calls getTestExecutionStatus which polls the status endpoint on a set interval.
            /// Updates are propegated to subscomponents. Controls test execution state flow.
            ///</summary>
            ///

            Status testStatusResponse = Framework.GetStatus();

            //Propagate status updates to UI bound objects.
            this.latestStatusSnapshot = testStatusResponse;
            this.latestStatusSnapshot.Statuses.ForEach(status => this.updateTestStatus(status.Name, status.State));

            // console.log(this.latestStatusSnapshot);
            var status = this.getTestStatusByName(SystemTests.testViewComponent?.GetTestName() ?? String.Empty, this.latestStatusSnapshot.Statuses);

            //Allow other application components (like side nav panel) to react to test executing status.
            // this.testService.IsExecuting = this.latestStatusSnapshot.executing;

            //Propagate updated status to the test component which is currently executing.
            testViewComponent?.OnTestStatusUpdate(
                testStatusResponse.Executing,
                this.IsCancelled,
                testStatusResponse.PercentComplete,
                status);

            //Keep polling test status until executing completes. On completion, unstage all tests.
            if (!this.latestStatusSnapshot.Executing)
            {
                await InvokeAsync(StateHasChanged);
                var isComplete = testViewComponent?.OnExecutionComplete() ?? true;
                if (!this.IsCancelling && !isComplete && !this.IsCancelled)
                {
                    return;
                }

                // Test Execution Complete.
                // When test execution completes, all staged tests need to be unstage, to prevent issue on consecutive test set executions.
                await this.unstageAllStaged();
                this.latestStatusSnapshot = null;
            }
            else
            {
                //Recursively poll test statuses every POLL_TEST_STATUS_INTERVAL_MS while executing.
                //Allow other application components (like side nav panel) to react to test executing status.

                // this.testService.IsExecuting = this.latestStatusSnapshot.executing;
                //setTimeout(() => this.pollTestStatuses(), this.POLL_TEST_STATUS_INTERVAL_MS);

                _ = Task.Run(async () =>
                {
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(POLL_TEST_STATUS_INTERVAL_MS);
                    await InvokeAsync(pollTestStatuses);
                });

            }

        }

        private async Task unstageAllStaged()
        {
            ///<summary>
            /// Goes through the most recent test execution state snapshot and unstages all unstaged tests.
            ///</summary>
            List<ToolStatus> testsToUnstage = new();

            if (this.latestStatusSnapshot == null) return;

            foreach (var toolStatus in this.latestStatusSnapshot.Statuses)
            {
                var isUnstage = false;
                foreach (var unstagedTool in this.latestStatusSnapshot.Unstaged)
                {
                    if (unstagedTool == toolStatus.Id)
                    {
                        isUnstage = true;
                        break;
                    }
                }
                if (!isUnstage)
                    testsToUnstage.Add(toolStatus);
            }

            if (testsToUnstage.Count > 0)
            {
                await this.recursiveUnstage(testsToUnstage);
            }
        }

        private async Task recursiveUnstage(List<ToolStatus> testsToUnstage)
        {
            if (testsToUnstage.Count == 0)
            {
                await this.executeTestsInSeq();
                return;
            }
            var testToUnstage = testsToUnstage[0];
            testsToUnstage.RemoveAt(0);

            Framework.UnStage(testToUnstage.Id);
            await this.recursiveUnstage(testsToUnstage);
        }



        private State? getTestStatusByName(string name, List<ToolFrameworkPackage.ToolStatus> statuses)
        {
            ///<summary>
            /// Helper method that searches for a test with the specified name within the list of provided statuses
            /// and returns the test status display name if a matching test is found, otherwise null is returned.
            ///</summary>
            ///
            return statuses.FirstOrDefault(x => x.Name == name)?.State;

        }


        public void handleTestCancelled()
        {
            // TODO this next statement is in componenttests
            // this.executionTestList = new(); //Prevents other tests from being executed after the current test is cancelled.
            if (this.currentTestExecuting != null)
                this.currentTestExecuting.State = State.Cancelled;
            this._isCancelled = true;
        }

        public void handleTestComplete(IComponentTestExecutionState testResult)
        {
            ///<summary>
            /// Called when child components reports that it has complete test execution completion logic.
            ///</summary>
            ///

            // TODO this is in componenttests
            //this._IsExecuting = false;

            this.updateTestStatus(testResult.test.Name, testResult.test.State);
        }

        private void updateTestStatus(string testName, State updatedState)
        {
            ///<summary>
            /// Helper method that attempts to update the UI bound test object with the provided updatedTestObj.
            ///</summary>
            ///
            var tool = masterTestList.SelectMany(x => x.Tools).FirstOrDefault(x => x.Name == testName);
            if (tool == null)
                return;
            tool.State = updatedState;
        }
    }
}