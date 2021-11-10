using Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Components.Shared
{
    public partial class TestResultsDialog
    {
        [Inject]
        public IFrameworkController? FrameworkController { get; set; }
        [Inject]
        IPlatformInfo? PlatformInfo { get; set; }
        [Parameter]
        public string modalHeader { get; set; } = "";

        [Parameter]
        public string testId { get; set; } = "";

        [Parameter]
        public bool hasInternetConnection { get; set; }

        [Parameter]
        public EventCallback TroubleShootStatusChanged { get; set; }

        [Parameter]
        public EventCallback ContactSupportStatusChanged { get; set; }

        private string modalDisplay = "display:none;";
        private string modalClass = "";
        private bool showBackdrop = false;
        private bool modalActive = false;

        string failureId = "";
        string qrCodeValue = "";
        bool promptMoreInformation = false;
        bool promptTroubleshoot = false;
        bool promptPleaseMakeSureGuidanceStepsTaken = false;
        bool promptHaveGuidanceStepsBeenTaken = false;
        bool promptGuidanceStepsNotResolved = false;
        bool promptPrivacyStatementQuestion = false;
        bool promptHPCustomerSupport = false;
        string? instance;
        string? passID;
        string? productId;
        string toolName = "";
        string emptyFailureId = " - ";
        string localizedTestResult = "";
        string serialNumber = "";
        Dictionary<string, DateTime> alreadySubmitedFailureIds = new();

        protected override void OnInitialized()
        {
            if (PlatformInfo != null)
            {
                this.productId = PlatformInfo.Product;
                this.serialNumber = PlatformInfo.SerialNumber;
            }

            base.OnInitialized();
        }

        public async Task OpenModal()
        {
            modalDisplay = "display:block;";
            StateHasChanged();
            await Task.Delay(100);
            modalClass = "show";
            showBackdrop = true;
        }

        public async void ToggleModal()
        {
            if (string.IsNullOrEmpty(modalClass))
                await OpenModal();
            else
                await CloseModal();
        }
        public async Task CloseModal()
        {
            modalClass = "";
            await Task.Delay(250);
            modalDisplay = "display:none";
            showBackdrop = false;
        }

        void LogUserAcceptence(string message, bool eventid)
        {
            var PrivacyDisclaimarLogMessage = message + " " + this.failureId;
            string[] eventLoggerMessage = new string[] { PrivacyDisclaimarLogMessage };

            FrameworkController?.SendEvent("information", "Privacy disclaimer", eventid, eventLoggerMessage);
        }

        public void popModalTroubleshoot(ToolResult history)
        {
            this.promptMoreInformation = false;
            this.promptTroubleshoot = true;
            this.promptHaveGuidanceStepsBeenTaken = true;
            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptGuidanceStepsNotResolved = false;
            this.promptHPCustomerSupport = false;
            this.promptPrivacyStatementQuestion = false;
            this.modalHeader = Localizer.STR_TROUBLESHOOT;
            this.qrCodeValue = "";
            this.passID = "";
            this.instance = history.Instance;
            this.failureId = history.FailureId;
            this.toolName = history.ToolName;
            this.testId = history.Id.ToString();
            modalActive = true;
            this.ToggleModal();
        }

        private async void updateTroubleshootStatus()
        {
            await TroubleShootStatusChanged.InvokeAsync(this);
        }

        private async void updateContactSupportStatus()
        {
            await ContactSupportStatusChanged.InvokeAsync(this);
        }

        string getQRCode(string failureId, string testId)
        {
            // TODO QR Code
            return "fake qr code";
        }

        public void PromptHPCustomerSupport()
        {
            this.LogUserAcceptence("User agreed to the privacy statement for", true);
            this.updateTroubleshootStatus();
            this.updateContactSupportStatus();

            this.qrCodeValue = getQRCode(this.failureId, this.testId);
            this.promptMoreInformation = false;
            this.promptTroubleshoot = false;
            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptHaveGuidanceStepsBeenTaken = false;
            this.promptGuidanceStepsNotResolved = false;
            this.promptPrivacyStatementQuestion = false;
            this.promptHPCustomerSupport = true;
            this.modalHeader = Localizer.STR_CONTACT_HP_SUPPORT;
        }
        public bool submitIfConnected()
        {
            if (this.alreadySubmitedFailureIds.ContainsKey(this.failureId))
            {
                var now = DateTime.Now;
                var submitTime = this.alreadySubmitedFailureIds[this.failureId];

                if (now.Subtract(submitTime) < TimeSpan.FromSeconds(10))
                {
                    return false; // TODO Original code has this mark
                }
            }

            this.alreadySubmitedFailureIds[this.failureId] = DateTime.Now;

            FrameworkController?.SubmitWarrantyCode(this.failureId, this.testId);

            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptHaveGuidanceStepsBeenTaken = true;
            this.promptGuidanceStepsNotResolved = false;
            this.promptHPCustomerSupport = false;
            this.modalHeader = Localizer.STR_TROUBLESHOOT;
            this.qrCodeValue = "";
            this.passID = "";
            this.instance = null;
            this.failureId = "";
            this.toolName = "";
            this.testId = "";

            this.ToggleModal();
            return true;
        }

        public void PromptPleaseMakeSureGuidanceStepsTaken()
        {
            this.promptMoreInformation = false;
            this.promptTroubleshoot = true;
            this.promptHaveGuidanceStepsBeenTaken = false;
            this.promptPleaseMakeSureGuidanceStepsTaken = true;
            this.promptGuidanceStepsNotResolved = false;
            this.promptHPCustomerSupport = false;
            this.promptPrivacyStatementQuestion = false;
        }
        public void PromptGuidanceStepsNotResolved()
        {
            this.promptMoreInformation = false;
            this.promptTroubleshoot = true;
            this.promptHaveGuidanceStepsBeenTaken = false;
            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptGuidanceStepsNotResolved = true;
            this.promptHPCustomerSupport = false;
            this.promptPrivacyStatementQuestion = false;
        }
        public void PromptHaveGuidanceStepsBeenTaken()
        {
            this.promptMoreInformation = false;
            this.promptTroubleshoot = true;
            this.promptHaveGuidanceStepsBeenTaken = true;
            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptGuidanceStepsNotResolved = false;
            this.promptHPCustomerSupport = false;
            this.promptPrivacyStatementQuestion = false;
        }

        public void popModalContactHPSupport(ToolResult history)
        {
            this.failureId = history.FailureId;
            this.toolName = history.ToolName;
            this.testId = history.Id.ToString();
            this.PromptPrivacyStatementQuestion();
            this.modalActive = true;
            this.ToggleModal();
        }

        public void popModalMoreInformation(ToolResult history)
        {
            this.promptMoreInformation = true;
            this.promptTroubleshoot = false;
            this.promptHaveGuidanceStepsBeenTaken = false;
            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptGuidanceStepsNotResolved = false;
            this.promptHPCustomerSupport = false;
            this.promptPrivacyStatementQuestion = false;
            this.modalHeader = Localizer.F30140_Key11; // More Info
            this.qrCodeValue = "";
            this.failureId = "";
            this.toolName = history.ToolName;
            this.testId = history.Id.ToString();
            this.localizedTestResult = Model.getLocalizedStateName(history.Result);
            this.passID = history.PassID;
            this.instance = history.Instance;
            this.modalActive = true;
            this.ToggleModal();
        }

        public void PromptPrivacyStatementQuestion()
        {
            this.promptMoreInformation = false;
            this.promptTroubleshoot = false;
            this.promptHaveGuidanceStepsBeenTaken = false;
            this.promptPleaseMakeSureGuidanceStepsTaken = false;
            this.promptGuidanceStepsNotResolved = false;
            this.promptHPCustomerSupport = false;
            this.promptPrivacyStatementQuestion = true;
            this.modalHeader = Localizer.STR_CONTACT_HP_SUPPORT;
        }
        public void AdditionalStepsFixedProblem()
        {
            this.updateTroubleshootStatus();
            this.updateContactSupportStatus();
            this.ToggleModal();
        }
        public void UserDisagreedToPrivacyStatement()
        {
            this.LogUserAcceptence("User disagreed to the privacy statement for", false);
            this.ToggleModal();
        }

        public bool copyToClipboard(string failureId)
        {
            if (string.IsNullOrEmpty(failureId) || failureId == this.emptyFailureId)
                return false;

            Clipboard.SetText(failureId);
            return true;
        }
    }
}