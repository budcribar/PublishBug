using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SureCheck.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public abstract partial class ComponentTest : ComponentBase, IDisposable
    {
        [Inject]
        public IToolFramework? Framework { get; set; }
        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public ComponentsLocalizer.LocalizedContent? Localizer { get; set; }

        [Inject]
        public MessageService? MessageService { get; set; }

        public RenderFragment? Fragment { get; set; }

        public bool isExecuting = false; //Tracks if current test is still executing on the server.
        public bool isCancelling = false; //Tracks if current tests executing has been cancelled.
        protected ToolComponent? currentTestExecuting; //Reference to currently executing test. 
        protected ClientInterfaces.ITestInstanceComponent? currentToolInstance = null; //Reference to currently executing test instance, if one exists.
        protected int currentInstanceIndex = -1; //Used to find next selected test instance for executing multiple instances in sequence.

        [Parameter]
        public bool Show { get; set; } = false;

        [Parameter]
        public EventCallback onStart { get; set; }

        [Parameter]
        public EventCallback onCancelled { get; set; }

        public Guid? selectedTest = null;
        public List<ToolComponent> TestList { get; set; } = new List<ToolComponent>();

        public new void StateHasChanged()
        {
            base.StateHasChanged();
        }
        public new Task InvokeAsync(Action workItem)
        {
            return base.InvokeAsync(workItem);
        }
        public virtual void InvokeAsync(string name, object data)
        {
            var task = Task.Run(async () =>
            {
                await JSRuntime!.InvokeVoidAsync(name, data);
                await InvokeAsync(StateHasChanged);
            });

        }

        public string? GetInstanceName()
        {
            return currentToolInstance?.id;
        }

        public string? GetTestName()
        {
            return this.currentTestExecuting?.Name;
        }

        public Guid? id => this.currentTestExecuting?.Id;

        public int TotalTestCount
        {
            get
            {
                var total = 0;

                if (this.TestList == null)
                    return 0;



                foreach (var entry in this.TestList)
                {

                    bool hasSelectedInstance = false;

                    foreach (var instance in entry.Instances)
                    {
                        if (instance.isSelected)
                        {
                            hasSelectedInstance = true;
                            total++; //Add one for each individual instance.

                            if (instance.state == State.Running)
                                CurrentTestIndex = total;
                        }
                    }


                    if (!hasSelectedInstance)
                    {
                        total++; //Add one for the test itself if no instances were selected, prevents double counting of parent and instance.
                        if (entry.State == State.Running)
                            this.CurrentTestIndex = total;
                    }
                }

                return total;
            }
        }

        public int CurrentTestIndex { get; set; } = 1;


        protected void UpdateTestStatus(bool isExecuting, bool isCancelling, int percentComplete, State? status)
        {
            if (status == null) return;
            if (this.currentToolInstance != null)
                this.currentToolInstance.state = status.Value;

            this.isExecuting = isExecuting;
            this.isCancelling = isCancelling;

            if (this.currentTestExecuting == null) return;

            this.currentTestExecuting.State = status.Value;
            this.currentTestExecuting.Progress = percentComplete;
        }




        public async virtual void StartTest(ToolComponent? testToExecute)
        {
            if (testToExecute == null)
                return;  //Function guard.

            this.currentTestExecuting = testToExecute;

            Framework?.Stage(testToExecute.Id);

            var selectedInstanceCount = testToExecute.Instances?.Count((instance) => instance.isSelected);
            if (selectedInstanceCount > 0)
            {
                //Test Instance Execution. 
                this.currentToolInstance = this.getNextInstanceToTest();
                if (this.currentToolInstance != null)
                    this.startTestInstance(testToExecute.Id, this.currentToolInstance.id);
            }
            else
            {
                //Standard test execution
                Framework!.Execute();
                await onStart.InvokeAsync();
            }
        }

        protected bool resetTestOnTestComplete()
        {
            if (!this.isCancelling && this.currentToolInstance != null)
            {
                // Execute next selected test instance in sequence
                this.currentToolInstance = this.getNextInstanceToTest();
                if (this.currentToolInstance != null)
                {
                    this.startTestInstance(this.currentTestExecuting?.Id ?? Guid.Empty, this.currentToolInstance.id);
                    return false;
                }
            }

            //Test execution complete. Reset test variables. 
            this.isExecuting = false;
            this.isCancelling = false;
            this.currentTestExecuting = null;
            this.currentToolInstance = null;
            this.currentInstanceIndex = -1;

            return true;
        }

        public abstract bool OnExecutionComplete();

        public abstract void OnTestStatusUpdate(bool isExecuting, bool isCancelling, int percentComplete, State? status);


        protected ClientInterfaces.ITestInstanceComponent? getNextInstanceToTest()
        {
            ///<summary>
            /// Returns the next selected test instance from the list of instances for the current test.
            /// SIDE EFFECT: the currentInstanceIndex class variable is updated so that the next time this
            /// method is called, it can be used as the starting search index to find the next selected test instance. 
            ///</summary>

            var instances = this.currentTestExecuting?.Instances;
            if (instances?.Count == 0)
                return null; //No instances to select from.

            var startIndex = this.currentInstanceIndex >= 0 ? this.currentInstanceIndex + 1 : 0;
            if (startIndex >= instances?.Count)
                return null; //All selected instances have completed execution.

            for (int i = startIndex; i < instances?.Count; i++)
            {
                if (instances[i].isSelected)
                {
                    this.currentInstanceIndex = i;
                    return instances[i];
                }
            }
            return null;
        }

        public virtual void cancelTestExecution()
        {
            this.isCancelling = true;
            this.onCancelled.InvokeAsync();
            Framework!.Cancel();
        }

        protected void startTestInstance(Guid testId, string testInstanceId)
        {
            Framework!.Tool(testId).Instance = testInstanceId;
            Framework!.Execute();
            onStart.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            if (MessageService != null)
                MessageService.PropertyChanged += MessageServicePropertyChanged;
            base.OnInitialized();
        }

        private async void MessageServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose()
        {
            if (MessageService != null)
                MessageService.PropertyChanged -= MessageServicePropertyChanged;
        }
    }
}

