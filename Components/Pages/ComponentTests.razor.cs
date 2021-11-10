using ClientInterfaces;
using Components.Custom;
using Components.Shared;
using Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Pages
{
    public partial class ComponentTests
    {
        [Parameter]
        public Guid TestId { get; set; }

        //readonly bool IS_GROUP_EXPANDED_DEFAULT = false; // True all tests for group will show, false all the respective test group's children are hidden.
        //readonly bool IS_TEST_EXPANDED_DEFAULT = false; // True all instances for test will show, false all the respective test's children are hidden.
        readonly int POLL_TEST_STATUS_INTERVAL_MS = 2000; // Minimum interval between test status polling calls.
        bool RTL = false;
        bool LTR = true;
        bool _IsExecuting = false; // Should only be set by test status monitoring logic based on IsExecuting state of server.
        ToolComponent? currentTestExecuting = null;
        Status? latestStatusSnapshot = null; // Most recent status update response object.
        public static ComponentTest? testViewComponent; // Store the current test specific view template. Generic type used to execute all test types.
        bool _isCancelled = false;
        bool isLoading = false;
        bool testExecutionComplete = false;
        string Click_start_Testing = "";
        CustomComponentTestComponent? customComponent;
        DefaultComponentTestComponent? defaultComponent;
        public Shared.TestDescriptionDialog? childModal;
        TestResultsComponent? testResultsComponent;

        public void OpenAboutModal()
        {

            childModal?.Open();
        }

        public void HideChildModal()
        {
            childModal?.Close();
        }

        void CheckForEnter(KeyboardEventArgs args, Action action)
        {
            if (args.Code == "Enter")
                action.Invoke();
        }

        public async void ExecuteComponentTests()
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

            ////Done to make sidebar hide sooner, seemed to lag when actually waiting on test polling to start before setting IsExecuting.
            this._IsExecuting = true;

            // TODO IsExecuting
            //this.testService.IsExecuting = true;

            ////Need to create new component test object which contains UI specific properties
            ////(properties or data structure which doesn't exist on model returned from the server because they are UI specific properties).
            ///
            foreach (var selectedTest in this.selectedTests)
            {
                var copy = ((ToolComponent)selectedTest)?.DeepCopy<ToolComponent>();

                if (copy != null)
                {
                    copy.State = State.ReadyToInstall;
                    this.executionTestList.Add(copy);
                }
            }

            ////Begin executing all selected tests in the order in which they were selected.
            await this.executeTestsInSeq();
        }

        bool IsExecuting
        {
            get
            {
                return this._IsExecuting || this.currentTestExecuting != null || (this.latestStatusSnapshot != null && this.latestStatusSnapshot.Executing); ;
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

        bool IsCustomTest
        {
            get
            {
                if (testViewComponent == null)
                    return false;
                if (!this.IsExecuting)
                    return false;
                if (testViewComponent is CustomComponentTestComponent)
                    return true;

                return false;
            }
        }

        public void selectTest(ToolComponent test, bool value, bool checkIndeterminate)
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

            //Propagate selection state to instances belonging to the specified test.
            foreach (var instance in test.Instances)
            {
                this.selectTestInstance(instance, test, value);
            }

            //Remove the test from the selected list if it was previously selected and no has been deselected.
            for (int i = 0; i < this.selectedTests.Count; i++)
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

        //string? errorText = null;

        //    // NEW GROUPING VARIABLES
        public List<ClientTestGroupComponent> masterTestList = new();     // Contains the origional set of test groups retrieved from the server.
        List<ITestGroupComponent> testViews = new();         // Stores pre-defined view templates for quick test set execution.
        List<ToolComponent> executionTestList = new();      // List of tests selected by the user for execution.
        List<ToolComponent> selectedTests = new();

        //    selectedTestView: ITestGroupComponent;
        //    selectedViewInstructions: string;
        //    isMasterSelectChecked = false;
        //    _isCancelled = false;
        //    Click_start_Testing = this.localizationService.localize.F30140_Key5.replace('%s', this.localizationService.localize.F30140_Key7);
        //    canDeactivate() {
        //        if (this.IsExecuting) {
        //            return false;
        //        }
        //        return true;
        //    }

        bool hasTestsSelected => selectedTests.Count > 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Click_start_Testing = Localizer.F30140_Key5.Replace("%s", Localizer.F30140_Key7);


            string selectedLanguage = Context.Language;
            this.RTL = Context.isRTL;
            this.LTR = !this.RTL;


            if (TestId == Guid.Empty)
                await componentLoad();
            else
            {

                var selectedTest = this.getTestById(TestId);

                if (selectedTest != null)
                {
                    selectedTest.IsSelected = true;
                    this.selectedTests.Add(selectedTest);
                    this.ExecuteComponentTests();
                }
            }

        }

        private void abort()
        {
            ///<summary>
            /// Attempts to stop of any test that is currently executing on the server.
            ///</summary>
            ///

            Framework.Abort();
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

            var groups = FrameworkController.GroupLoad();
            foreach (var group in groups)
                group.Tools = group.Tools.Where(x => x.State != State.InstallationFailed && x.State != State.PreConditionsFailed && x.State != State.PostConditionsFailed).ToList();

            // TODO Don't add Repair Tools
            groups = groups.Where(g => g.Category == ToolGroupCategory.Standard && g.Name != "MonitoringTests" && g.Tools.Count > 0 && !g.IsView);

            testViews = groups.ToList();

            foreach (var g in testViews.Select(x => x.Name).Distinct())
                masterTestList.Add(new ClientTestGroupComponent { Name = g });


            // TODO Instances
            foreach (var m in masterTestList)
                m.Tools = testViews.Where(x => x.Name == m.Name).SelectMany(t => t.Tools).Select(x => new ToolComponent { Name = x.Name, GroupName = m.Name, Id = x.Id, Description = x.Description, AdditionalInfo = x.AdditionalInfo, Instances = x.Instances.Select(x => new ClientTestInstanceComponent { errorCode = x.ErrorCode, failureId = x.FailureId, id = x.Id, state = x.State }).ToList(), Instructions = x.Instructions, State = x.State }).ToList();

            //                tool.state !== State.PreConditionsFailed &&
            //                tool.state !== State.PostConditionsFailed);

            // Filter groups based on view property. Used filtered list to create testGroups and testViews.
            // Use group category to combine repair tools and standard tools under same group.




            //            let loader: Observable<ITestGroupComponent[]> = this.testService.loadTestGroups();

            //            return Observable.create(
            //                (observer: any) => {
            //                    loader.subscribe(
            //                        (data: Array<ITestGroupComponent>) => {
            //                            this.isLoading = false;

            //                            // Filter groups based on view property. Used filtered list to create testGroups and testViews.
            //                            // Use group category to combine repair tools and standard tools under same group.
            //                            const testGroups = data.filter((testGroup: ITestGroupComponent) => {
            //                                return this.isTestGroupable(testGroup, false);
            //    });
            //                            const combinedTestGroup = this.combineToolGroupCategories(testGroups);

            //    const testViews = data.filter((testView: ITestGroupComponent) => {
            //                                return this.isTestGroupable(testView);
            //});
            //const combinedTestViews = this.combineToolGroupCategories(testViews);

            //this.testViews = combinedTestViews; //  testViews;
            //this.masterTestList = combinedTestGroup; // testGroups;

            //this.testExecutionComplete = false;
            //this.testService.IsExecuting = false; // Allow other 

            //for (let group of this.masterTestList)
            //{
            //    for (let tool of group.tools)
            //    {
            //        tool.state = State.InstallationPassed;
            //    }
            //}
            //observer.next(true);
            //observer.complete();
            //                        }
            //                    );
            //                }
            //            );
            return true;

        }

        //    public allInstancesSelected(instances: Array<ITestInstanceComponent>): boolean {
        //        let allSelected = instances && instances.filter((i) => { return i.isSelected }).length === instances.length;
        //        return allSelected;
        //    }


        void closeAboutModal()
        {
            //this.childModal.config.backdrop = false;
            // this.childModal.toggle();
        }

        //    private isTestGroupable(iTestGroupComponent: ITestGroupComponent, isView: boolean = true): boolean {
        //        if (iTestGroupComponent === null) {
        //            return false;
        //        }

        //        // filter repair groups if not repair mode
        //        if ((iTestGroupComponent.category === ToolGroupCategory.Repair)
        //            && (this.settingsService.repairMode !== true)) {
        //            return false;
        //        }

        //        if (iTestGroupComponent.name === 'MonitoringTests') {
        //            return false;
        //        }
        //        // reset the original itestgroupcomponent's tools
        //        iTestGroupComponent.tools = iTestGroupComponent.tools.filter((tool) => {
        //            // Don't add a test to the list of available tests if the respective test failed to successfully load on the server.
        //            return (tool.state !== State.InstallationFailed &&
        //                tool.state !== State.PreConditionsFailed &&
        //                tool.state !== State.PostConditionsFailed);
        //        });
        //        return iTestGroupComponent.isView === isView && iTestGroupComponent.tools.length > 0;
        //    }

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

                // Note SystemTests has check for isCancelled
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
                Id = this.currentTestExecuting.Id
            };

            this.updateTestStatus(freshTestStatus.Name, freshTestStatus.State); //Reset test state, used for consecutive executions of a test.

            //Only custom tests will result in a javascript file returned from server. Use this to determine component test type.

            string? jsSource = Framework.Tool(this.currentTestExecuting.Id).GuiJavascript;

            var gui = Framework.Tool(this.currentTestExecuting.Id).GuiType;

            if (gui != null)
            {
                testViewComponent = this.customComponent;
                testViewComponent!.Fragment = (builder) => { builder.OpenComponent(0, gui); builder.CloseComponent(); };
            }
            else testViewComponent = string.IsNullOrEmpty(jsSource) ? this.defaultComponent : this.customComponent;

            this.currentTestExecuting.IsInteractive = !string.IsNullOrEmpty(jsSource);

            // Sanity check
            if (testViewComponent == null)
                return;

            testViewComponent.TestList = this.executionTestList;
            this._IsExecuting = true;
            //testViewComponent.Show = true;  

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
            // TODO polling is lame
            this.pollTestStatuses();
        }

        public void handleTestCancelled()
        {
            this.executionTestList = new(); //Prevents other tests from being executed after the current test is cancelled.
            if (this.currentTestExecuting != null)
                this.currentTestExecuting.State = State.Cancelled;
            this._isCancelled = true;

            InvokeAsync(StateHasChanged);
        }


        // TODO parameter is different than the typescript version
        public void handleTestComplete(IComponentTestExecutionState testResult)
        {
            ///<summary>
            /// Called when child components reports that it has complete test execution completion logic.
            ///</summary>
            this._IsExecuting = false;

            this.updateTestStatus(testResult.test.Name, testResult.test.State);

            InvokeAsync(StateHasChanged);
        }

        private void updateTestStatus(string testName, State updatedState)
        {
            ///<summary>
            /// Helper method that attempts to update the UI bound test object with the provided updatedTestObj.
            ///</summary>
            ///

            var tool = masterTestList.SelectMany(x => x.Tools).FirstOrDefault(x => x.Name == testName);
            if (tool == null) return;
            tool.State = updatedState;
            InvokeAsync(StateHasChanged);
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
            var status = this.getTestStatusByName(testViewComponent?.GetTestName() ?? "", this.latestStatusSnapshot.Statuses);

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
            ///

            if (this.latestStatusSnapshot == null) return;

            List<ToolStatus> testsToUnstage = new();

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

        //    public isExecutionCompleteStatus(instance: ITestInstanceComponent) {
        //        return this.getStateName(instance.state) === 'Passed' ||
        //            this.getStateName(instance.state) === 'Failed' ||
        //            this.getStateName(instance.state) === 'Cancelled';
        //    }




        //    private sortByGroupName(a: ITestGroupComponent, b: ITestGroupComponent) {

        //        if (a.name < b.name)
        //            return -1;
        //        if (a.name > b.name)
        //            return 1;
        //        return 0;
        //    }

        //    public navBack() {
        //        this.location.back();
        //    }

        //    public toggleViewsTestSelections(view: ITestGroupComponent) {

        //        if(!view || !view.tools || view.tools.length === 0)
        //            return;

        //        this.isMasterSelectChecked = !this.isMasterSelectChecked;
        //        view.tools.forEach((tool) => {
        //            this.selectTest(tool, this.isMasterSelectChecked, true);
        //        });
        //    }



        //        //Remove the test from the selected list if it was previously selected and no has been deselected.
        //        for (let i = 0; i < this.selectedTests.length; i++) {
        //            if (this.selectedTests[i].id === test.id) {
        //                if (!value)
        //                    this.selectedTests.splice(i, 1);
        //                return;
        //            }
        //        }

        //        if (value)
        //            this.addSelectedTestHelper(test);
        //    }

        public void selectTestInstance(ClientTestInstanceComponent testInstance, ToolComponent parentTest, bool value)
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

        private ToolComponent? getTestById(Guid testId)
        {
            return masterTestList.SelectMany(x => x.Tools).FirstOrDefault(x => x.Id == testId);
        }
    }
}
