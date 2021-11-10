using ClientInterfaces;
using Components.Pages;
using Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SureCheck.Components;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Custom
{
    public partial class CustomComponentTestComponent : ComponentTest, IDisposable
    {
        //<summary>
        // This angular component renders diagnostic component tests which have a test specific UI (javascript + html).
        // It sub instantiates a new angular component using the JS and HTML provided and allows for custom test execution logic flows by
        // Both the backend testing server and the test specific JS/HTML which can utilize a testing prompt to control test flow.
        //</summary>

        [Parameter]
        public EventCallback<ComponentTestExecutionState> onComplete { get; set; }

        [Inject]
        ClientInterfaces.ILocalizeContext? Context { get; set; }


        private string modalDisplay = "display:none;";
        private string modalClass = "";
        private bool showBackdrop = false;

        private static SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
        private static string promptResult = "";

        public IPromptOptions? promptOptions { get; set; }
        private bool isExitOnPass = false;
        private string _testHtmlString = "";
        string testHtmlString => _testHtmlString;
        string? InstanceName => null;
        public bool LTR = true;

        protected override void OnInitialized()
        {
            if (Context != null)
            {
                this.LTR = Context.isRTL;
                Context.PropertyChanged += ContextPropertyChanged;
            }
                
            base.OnInitialized();
        }

        private void ContextPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Context != null)
            {
                this.LTR = Context.isRTL;
            }
            StateHasChanged();
        }

        public override void Dispose()
        {
            if (Context != null)
            {
                Context.PropertyChanged -= ContextPropertyChanged;
            }
            base.Dispose();
        }

        public async override void StartTest(ToolComponent? testToExecute)
        {
            ///<summary>
            /// Attempt to begin executing test or test instance.
            /// onStart event fired upon successful test start.
            ///</summary>

            var testId = testToExecute?.Id ?? Guid.Empty;
            this.currentTestExecuting = testToExecute;

            _testHtmlString = base.Framework?.Tool(testId).GuiHtml ?? "";
            var js = Framework?.Tool(testId).GuiJavascript ?? "";

            var componentInfo = new CustomComponentInfo(_testHtmlString, js, this.createTestContext(testId));

            if (JSRuntime != null)
                await JSRuntime.InvokeVoidAsync("setupTestCallbacksAndStartTest", componentInfo);


            base.StartTest(testToExecute);
        }

        //<summary>
        // Returns a new test context with the provided test id.
        // The context is used to provide access to test data and callback handles.
        //</summary>
        ITestContext createTestContext(Guid testId)
        {
            var context = new TestContext();
            context.id = testId.ToString();
            context.fontSizeSettings = new FontSizeSetting { fontSizeMod = 1, baseFontSizePx = 18, baseFontStepPx = 2, fontZoomMod = -1 };
            context.selectedLanguage = LocalizeContext.Language;
            context.languages = SupportedLanguages.Values.Select(x => x.Value).Distinct().ToArray();

            //    context.cancelTest = () => { this.onCancelled.emit(); this.testService.cancelTest(); };       
            return context;
        }


        // TODO FontSizeSettings
        static IFontSizeSetting? FontSizeSettings() { return null; }

        [JSInvokable]
        public static Task AbortTest()
        {
            ComponentTests.testViewComponent?.Framework?.Abort();
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task CancelTest()
        {
            ComponentTests.testViewComponent?.Framework?.Cancel();
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task FireTestEvent(string data)
        {
            ComponentTests.testViewComponent?.Framework?.FireTestEvent(data);
            ComponentTests.testViewComponent?.InvokeAsync(ComponentTests.testViewComponent.StateHasChanged);
            return Task.CompletedTask;
        }


        [JSInvokable]
        public static Task CreateMessageOutput(string message)
        {
            if (ComponentTests.testViewComponent != null && ComponentTests.testViewComponent.MessageService != null)
                ComponentTests.testViewComponent.MessageService.Message = message;
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task LogError(string message)
        {
            if (ComponentTests.testViewComponent != null && ComponentTests.testViewComponent.MessageService != null)
                ComponentTests.testViewComponent.MessageService.Message = message;

            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task<ComponentsLocalizer.LocalizedContent?> Localize()
        {
            return Task.FromResult(ComponentTests.testViewComponent?.Localizer);
        }

        // TODO Change to invoke on instance
        [JSInvokable]
        public static Task FullScreen()
        {
            (ComponentTests.testViewComponent as CustomComponentTestComponent)?.Window.FullScreen();
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task RestoreDown()
        {
            (ComponentTests.testViewComponent as CustomComponentTestComponent)?.Window.RestoreDown();
            return Task.CompletedTask;
        }

        // onFontSizeChanged

        [JSInvokable]
        public static async Task<string> ShowPrompt(bool exitOnPass, string content, PromptOption yesOption, PromptOption noOption, InputHtmlOption inputHtmlOption, string title)
        {
            //Event propagated from custom component to display test prompt.
            if (ComponentTests.testViewComponent == null) return "";

            if (string.IsNullOrEmpty(yesOption.text))
                yesOption.text = ComponentTests.testViewComponent?.Localizer?.STR_BUTTON_YES ?? "Yes";

            if (string.IsNullOrEmpty(noOption.text))
                noOption.text = ComponentTests.testViewComponent?.Localizer?.STR_BUTTON_NO ?? "No";

            CustomComponentTestComponent? tc = ComponentTests.testViewComponent as CustomComponentTestComponent;
            if (tc == null) return string.Empty;

            tc.isExitOnPass = exitOnPass;

            tc.promptOptions = new PromptOptions
            {
                yesPromptOption = yesOption,
                noPromptOption = noOption,
                inputHtmlOption = inputHtmlOption,
                title = title,
                promptContent = content
            };

            if (tc.isExecuting)
                await tc.openTestPrompt();

            await semaphore.WaitAsync();

            return promptResult;
        }

        public override void OnTestStatusUpdate(bool isExecuting, bool isCancelling, int percentComplete, State? status)
        {
            ///summary>
            /// This method should be called by a parent component that controls test execution and status monitoring
            /// and reports each status update to this component via this method. Status related UI elements may rely on this call.
            ///</summary>
            ///

            base.UpdateTestStatus(isExecuting, isCancelling, percentComplete, status);
        }

        public override bool OnExecutionComplete()
        {
            ///<summary>
            /// This method should be called by a parent component that controls test execution and status monitoring and should
             // be called when the parent component has determined that the custom tests execution has completed.
            // Used to clean up dynamically generated test resources.
            ///</summary>
            ///

            var completedTest = this.currentTestExecuting.DeepCopy();

            var isComplete = this.resetTestOnTestComplete();
            if (isComplete)
                this.completeTestExecutionAndRestState(new ComponentTestExecutionState { test = new ToolStatus { Name = completedTest?.Name ?? "", State = completedTest?.State ?? State.ReadyToInstall } });

            return isComplete;
        }

        public async void testPromptResultClick(bool result)
        {
            ///<summary>
            /// Callback which should only be called when the user clicks a button
            /// on the test result prompt. Two buttons, yes/no. Result arguement true for yes button press
            /// and false for the no button being pressed.
            ///</summary>
            ///


            promptResult = this.promptOptions?.inputHtmlOption?.value ?? result.ToString().ToLower();

            this.promptOptions = null;

            await this.closeTestPrompt();
        }

        public void showOrHidePassword()
        {
            if (this.promptOptions != null)
                this.promptOptions.inputHtmlOption.type = this.promptOptions.inputHtmlOption.type == "text" ? "password" : "text";
        }

        public string getClass()
        {
            if (this.promptOptions == null) return "";
            return this.promptOptions.inputHtmlOption.type == "password" ? "fa fa-fw fa-eye field-icon toggle-password" : "fa fa-eye-slash field-icon";
        }

        private async void completeTestExecutionAndRestState(ComponentTestExecutionState completedTest)
        {
            if (JSRuntime != null)
                await JSRuntime.InvokeVoidAsync("CleanupTest");

            await onComplete.InvokeAsync(completedTest);
        }

        private async Task closeTestPrompt()
        {
            modalClass = "";
            await Task.Delay(250);
            modalDisplay = "display:none;";
            showBackdrop = false;

            semaphore.Release();
        }

        private void createMessageOutput(string? msg)
        {
            if (MessageService != null)
                MessageService.Message = msg ?? "";
        }

        public async Task openTestPrompt()
        {
            // TODO Keyboard on modal
            //this.testResultPromptModal.config.backdrop = 'static';
            //this.testResultPromptModal.config.ignoreBackdropClick = true;
            //this.testResultPromptModal.config.keyboard = this.promptOptions.inputHtmlOption ? true : false;


            modalDisplay = "display:block;";
            await Task.Delay(100);
            modalClass = "show";
            showBackdrop = true;
        }
    }
}