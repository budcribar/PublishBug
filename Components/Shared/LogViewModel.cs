using ClientInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SureCheck.Components;
using System.Threading.Tasks;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public class LogViewModel
    {
        ComponentsLocalizer.LocalizedContent Localizer;
        BackgroundInit<IHistory> History;
        IHTMLLogger Logger;
        IJSRuntime JSRuntime;
        public NavigationManager NavigationManager { get; set; }

        public void PopWebPagePrivacyStatement()
        {
            NavigationManager.NavigateTo("iframe/privacy");
        }
        public void PopWebPageAudioNoSoundFromComputer()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageAudioNoSoundFromComputer");
        }

        public void PopWebPageAudioNoSoundFromSpeakers()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageAudioNoSoundFromSpeakers");
        }

        public void PopWebPageAudioMicrophoneProblems()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageAudioMicrophoneProblems");
        }

        public void PopWebPageAudioMicrophoneTroubleshooting()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageAudioMicrophoneTroubleshooting");
        }

        public void PopWebPageKbdTroubleshooting()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageKbdTroubleshooting");
        }

        public void PopWebPageWiredKbdTroubleshooting()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageWiredKbdTroubleshooting");
        }

        public void PopWebPageWirelessKbdTroubleshooting()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageWirelessKbdTroubleshooting");
        }

        public void PopBrowseHpDiagnosticsWebsite()
        {
            NavigationManager.NavigateTo("iframe/BrowseHpDiagnosticsWebsite");
        }

        public void PopWebPageTroubleshootingMouseProblems()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageTroubleshootingMouseProblems");
        }

        public void PopWebPageWirelessKeyboardAndMouseConnectionIssues()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageWirelessKeyboardAndMouseConnectionIssues");
        }

        public void PopWebPageWebcamDocuments()
        {
            NavigationManager.NavigateTo("iframe/PopWebPageWebcamDocuments");
        }

        public async Task<string> LogFile()
        {
            var history = await History.Instance();
            var html = Logger.GetLogAsStandalone(history.Records);
            return html;
        }

        public async void ExportLogs()
        {
            var history = await History.Instance();
            var html = Logger.GetLogAsStandalone(history.Records);
            await Interop.SaveAs(JSRuntime, "logs.html", html);
        }


        public LogViewModel(ComponentsLocalizer.LocalizedContent localizer, BackgroundInit<IHistory> history, IHTMLLogger Logger, IJSRuntime JSRuntime, NavigationManager navigationManager)
        {
            this.Localizer = localizer;
            this.History = history;
            this.Logger = Logger;
            this.JSRuntime = JSRuntime;
            NavigationManager = navigationManager;
        }

        // TODO isInteractiveTest is a hack
        public bool isInteractiveTest(string id) =>
         (id == "8e2374d3-9406-41de-a634-b9d0fa7803bd"
            || id == "cf7d4afc-bd89-486d-92b8-ffa8f74ffc8e"
            || id == "607b14c3-aab0-4b48-8b3f-fb35b5f74662"
            || id == "c8bf2589-a5f1-4c2e-b327-5e6418ee6e15"
            || id == "c5edd890-dece-4615-ac77-5e41dd19b248"
            || id == "133336db-ad99-4b4e-b4b3-c5bf376f510c"
            || id == "44ae378e-0fa4-456a-ae8f-f4a801202b79"
            || id == "004e5668-ac98-4ac8-bd66-4e660cc06fc0"
            || id == "9dff918d-5180-4b7d-b2cd-e0d2b11d95e0");



        public string getStateName(State testStatus)
        {
            ///<summary>
            /// Temporary way to get a test result based on the test state. This should be replaced by a rolled up test status returned from server.
            /// This is used to style test UI elements based on the test's execution result.
            ///</summary>

            switch (testStatus)
            {
                case State.PreConditionsFailed:
                case State.PostConditionsFailed:
                case State.ExecutionFailed:
                case State.TimedOut:
                case State.Aborted:
                    return "Failed";
                case State.Cancelled:
                    return "Cancelled";
                case State.ExecutionPassed:
                    return "Passed";
                case State.Running:
                    return "Executing";
                default:
                    return "Ready";
            }
        }

        public string getLocalizedStateName(State result)
        {

            var logResultName = this.getStateName(result);
            switch (logResultName)
            {
                case "Failed":
                    return Localizer.STR_RESULT_FAILED;
                case "Passed":
                    return Localizer.STR_RESULT_PASSED;
                case "Cancelled":
                    return Localizer.STR_RESULT_CANCELED;
                case "NotRun":
                    return "-";
            }
            return "";
        }
    }
}
