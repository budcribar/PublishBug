using Components.Shared;
using Microsoft.AspNetCore.Components;
using ToolFrameworkPackage;

namespace Components.Custom
{
    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }

    ///<summary>
    /// This component is meant to render component test execution generically for tests without a test specific UI.
    /// The DefaultComponentTestComponent will take status updates and show test progress using a progress bar.
    ///</summary>
    public partial class DefaultComponentTestComponent : ComponentTest
    {


        [Parameter]
        public EventCallback<ComponentTestExecutionState> onComplete { get; set; }

        [Parameter]
        public EventCallback onTestSetComplete { get; set; }

        public string? additionalInfo;

        //Public getter required for aot compilation, variable is used in view but object scope is protected.
        ToolComponent? testExecutingRef => base.currentTestExecuting;

        string testIndexAndTotalDisplayText
        {
            get
            {
                var formattedText = Localizer!.STR_HELP_SYSTEMCHECK_EXEC_TEST;
                formattedText = formattedText.ReplaceFirst("%d", base.CurrentTestIndex.ToString());
                formattedText = formattedText.ReplaceFirst("%d", base.TotalTestCount.ToString());

                return formattedText;
            }
        }

        string? getInstanceName()
        {
            return base.currentToolInstance?.id;
        }

        public override void StartTest(ToolComponent? testToExecute)
        {
            ///<summary>
            /// Attempt to begin executing test or test instance. 
            /// onStart event fired upon successful test start. 
            ///</summary>
            base.selectedTest = testToExecute?.Id;
            var formattedText = Localizer!.STR_HELP_SYSTEMCHECK_EXEC_TEST;
            formattedText = formattedText.ReplaceFirst("%d", base.CurrentTestIndex.ToString());
            formattedText = formattedText.ReplaceFirst("%d", base.TotalTestCount.ToString());
            //this.testIndexAndTotalDisplayText = formattedText;

            // this will insert custom content while the test is running
            // to show a localized message here add the localized content as "AdditionalInfo" in the wrapper's Resources/Localizations.resx
            // "AdditionalInfo" will be included in the test object that gets passed down from the GroupLoad controller and eventually ends up in testToExecute
            this.additionalInfo = testToExecute?.Description;

            base.StartTest(testToExecute);
        }

        public override void OnTestStatusUpdate(bool isExecuting, bool isCancelling, int percentComplete, State? status)
        {
            ///summary>
            /// This method should be called by a parent component that controls test execution and status monitoring
            /// and reports each status update to this component via this method. Status related UI elements rely on this call.
            ///</summary>
            ///

            base.UpdateTestStatus(isExecuting, isCancelling, percentComplete, status);
        }

        //      public override void cancelTestExecution()
        //{
        //          base.cancelTestExecution();
        //}

        public override bool OnExecutionComplete()
        {
            ///<summary>
            /// Returns true if execution completed. 
            /// Returns false if a single test instance execution completed but one or more test 
            /// instances still need to be tested as part of this execution set. 
            ///</summary>     
            var isComplete = base.resetTestOnTestComplete();
            if (isComplete)
                base.selectedTest = null;

            return isComplete;
        }

        public string formatPercentString()
        {
            var stringVal = Localizer!.STR_HELP_PROGRESS + " ";
            var percent = base.currentTestExecuting?.Progress + " %";
            return stringVal + percent;
        }
    }
}
