function createTestContext(componentInfo) {
    componentInfo.testContext.abortTest = function () { return DotNet.invokeMethodAsync('Components', 'AbortTest').then(function () { return console.log("AbortTest"); }); };
    componentInfo.testContext.cancelTest = function () { return DotNet.invokeMethodAsync('Components', 'CancelTest').then(function () { return console.log("CancelTest"); }); }; // TODO
    componentInfo.testContext.fireTestEvent = function (data) { return DotNet.invokeMethodAsync('Components', 'FireTestEvent', data).then(function () { return console.log("FireTestEvent" + JSON.stringify(data)); }); };
    componentInfo.testContext.fullScreen = function () { return DotNet.invokeMethodAsync('Components', 'FullScreen'); };
    componentInfo.testContext.restoreDown = function () { return DotNet.invokeMethodAsync('Components', 'RestoreDown'); };
    componentInfo.testContext.disposeTestListener = null;
    componentInfo.testContext.localize = {}; // gets populated by localization.ts
    componentInfo.testContext.showPrompt = function (exitOnPass, content, yesOption, noOption, inputHtmlOption, title) { return DotNet.invokeMethodAsync('Components', 'ShowPrompt', exitOnPass, content, yesOption, noOption, inputHtmlOption, title); };
    componentInfo.testContext.createMessageOutput = function (data) { return DotNet.invokeMethodAsync('Components', 'CreateMessageOutput', data); };
}
function hasInternetConnection() {
    return navigator.onLine;
}
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}
var SetupCallbacks = /** @class */ (function () {
    function SetupCallbacks() {
        this.isCancelling = false;
        this.isExecuting = true;
        this.$scope = null;
        this.logService = {
            postTestException: function (e) { }
        };
        this.isExitOnPass = false;
    }
    SetupCallbacks.prototype.localize = function () {
        return DotNet.invokeMethodAsync('Components', 'Localize').then(function () { return console.log("Localize"); });
    };
    SetupCallbacks.prototype.testResultPromptListener = function (data) {
        //Event propagated from server to display test prompt.
        this.isExitOnPass = data.exitOnPass;
        //Add new lines between query entries to distinguish each from the other. 
        var content = "";
        if (typeof data.query !== "string" && data.query.length > 0) {
            for (var _i = 0, _a = data.query; _i < _a.length; _i++) {
                var q = _a[_i];
                content += (q + ' \n\r ');
            }
        }
        else
            content = data.query;
        var trueOption = {
            text: data.trueoption ? data.trueoption : this.localize().STR_BUTTON_YES,
            callback: function (data) {
                //this.testService.fireTestEvent(this.currentTestExecuting.id, 'true')
            }
        };
        if ((typeof (data.ignoretruecallback) !== 'undefined') && data.ignoretruecallback) {
            trueOption.callback = function (data) { }; // No Op. Prevent unecessary call backs to backend
        }
        var falseOption = {
            text: data.falseoption ? data.falseoption : this.localize().STR_BUTTON_NO,
            callback: function (data) {
                //this.testService.fireTestEvent(this.currentTestExecuting.id, "false")
            }
        };
        if ((typeof (data.ignorefalseoption) !== 'undefined') && data.ignorefalseoption) {
            falseOption = null;
        }
        this.promptOptions = {
            //Default prompt setup for prompt show event origionating from the server.
            yesPromptOption: trueOption,
            noPromptOption: falseOption,
            inputHtmlOption: null,
            title: data.title ? data.title : '',
            promptContent: content
        };
        this.promptOptions.promptContent = content;
        //if (this.isExecuting)
        //this.openTestPrompt();
        // TODO
        var tc = this.$scope.testContext;
        tc.showPrompt(data.exitOnPass, content, trueOption, falseOption, null, data.title).then(function (r) { tc.fireTestEvent(r); });
    };
    SetupCallbacks.prototype.hardwareEventListener = function (data) {
        var _this = this;
        //Test data event propagated from server, passed to Gui.ts which is injected using eval().
        setTimeout(function () {
            if (_this.isCancelling || !_this.isExecuting) {
                return; // Do not send UI updates once test is cancelling.
            }
            if (testListener && _this.$scope) {
                try {
                    testListener(data, _this.$scope, _this.$scope.testContext);
                }
                catch (e) {
                    //If script error occurs during interactive test execution, send exception to server to be logged. 
                    DotNet.invokeMethodAsync('Components', 'LogError', e);
                }
            }
        }, 1000);
    };
    return SetupCallbacks;
}());
var setup = new SetupCallbacks();
function CleanupTest() {
    ///<summary>
    /// Destroy the dynamically created angular module and component.
    /// Removes all test related event listeners and resets all variables
    /// related to test execution in preparation for consecutive runs.
    ///</summary>
    // Call cleanup logic on parent component if a dispose method is defined.
    if (setup.$scope && setup.$scope.testContext && setup.$scope.testContext.disposeTestListener) {
        setup.$scope.testContext.disposeTestListener();
    }
    //        Clear test related callback function references.
    hdwListener = null;
    testListener = null;
    testResultPromptListener = null;
    //       Reset prompt variables for consecutive runs.Prevents possible "flash" of old to new data during databinding and view rendering.
    setup.isExitOnPass = false;
    setup.promptOptions = null;
    setup.$scope = null;
    //setup._testHtmlString = "";
    //setup.closeTestPrompt(); //Make sure to hide prompt once component is no longer in use.
    //setup.selectedTest = null;
    initLocalizer = null;
}
function setupTestCallbacksAndStartTest(componentInfo) {
    initLocalizer = null;
    createTestContext(componentInfo);
    testResultPromptListener = setup.testResultPromptListener.bind(setup);
    setup.$scope = { testContext: {}, Yes: "", No: "" };
    setup.$scope.testContext = componentInfo.testContext;
    eval(componentInfo.js); //Load the tests custom javascript into application code.
    if (initLocalizer)
        initLocalizer(componentInfo.testContext);
    hdwListener = setup.hardwareEventListener.bind(setup);
    //componentInfo.testContext.abortTest();
    //componentInfo.testContext.cancelTest();
    //componentInfo.testContext.createMessageOutput('this is a message');
}
//# sourceMappingURL=Interop.js.map