
declare var testListener: (data: any, scope: any, testContext: any) => void;
declare var hdwListener: (data: any) => void;
declare var testResultPromptListener: (data: any) => void;
declare var DotNet;

declare var initLocalizer: (context: ITestContext) => void;

interface IInputHtmlOption {
    hidden: boolean;
    placeholder: string;
    type: string;
    value: string;
}
interface IResultPromptData {
    title?: string;
    trueoption?: string;
    falseoption?: string;
    exitOnPass: boolean;
    ignoretruecallback?: boolean;
    ignorefalseoption?: boolean;
    query: any; // can be a string message or a list of strings to be concat.
}


interface IPromptOptions {
    title: string;
    promptContent: string;
    yesPromptOption: IPromptOption;
    noPromptOption: IPromptOption;
    inputHtmlOption: IInputHtmlOption;
}

interface IPromptOption {
    text: string;
    callback: (data?: any) => void;
}

interface IFontSizeSetting {
    fontSizeMod: number;
    baseFontSizePx: number; // The starting application font size;
    baseFontStepPx: number; // The pixels added/subtracted from font size with each zoom-in/zoom-out action.
    zoomScaledFontSize: number; // The calculated application font size based on the current zoom-level and base font size.
    fontZoomMod: number; // The number of times the zoom modifier has been applied (This is bounded using max/min zoom).
}


interface IPoint {
    x: number;
    y: number;
}

interface ITestContext {
    isLayoutPortrait: boolean;
    windowSize: IPoint;
    canvasSize: IPoint;

    id: string;

    fontSizeSettings: IFontSizeSetting;

    localize: any;
    fireTestEvent: (eventData: any) => void;
    //stageTest: (id: string) => void;
    //executeTest: () => void;
    abortTest: () => void;
    cancelTest: () => void;
    //unstageTest: (id: string) => Observable<boolean>;
    disposeTestListener: () => void;
    selectedLanguage: string;
    fullScreen: () => void;
    restoreDown: () => void;
    showPrompt: (exitOnPass: boolean
        , content: string
        , yesOption: IPromptOption
        , noOption?: IPromptOption
        , inputHtmlOption?: IInputHtmlOption
        , title?: string) => any;
    languages: Array<string>;

    createMessageOutput: (msg: string) => void;

    //onFontSizeChanged: BehaviorSubject<IFontSizeSetting>;
}

interface ICustomComponentInfo {
    html: string;
    js: string;
    testContext: ITestContext;
}



function createTestContext(componentInfo: ICustomComponentInfo) {
    componentInfo.testContext.abortTest = () => DotNet.invokeMethodAsync('Components', 'AbortTest').then(() => console.log("AbortTest"));
    componentInfo.testContext.cancelTest = () => DotNet.invokeMethodAsync('Components', 'CancelTest').then(() => console.log("CancelTest"));// TODO
    componentInfo.testContext.fireTestEvent = (data) => DotNet.invokeMethodAsync('Components', 'FireTestEvent', data).then(() => console.log("FireTestEvent" + JSON.stringify(data)));
    componentInfo.testContext.fullScreen = () => DotNet.invokeMethodAsync('Components', 'FullScreen');
    componentInfo.testContext.restoreDown = () => DotNet.invokeMethodAsync('Components', 'RestoreDown');
    componentInfo.testContext.disposeTestListener = null;
    componentInfo.testContext.localize = {}; // gets populated by localization.ts
    componentInfo.testContext.showPrompt = (exitOnPass: boolean, content: string, yesOption: IPromptOption, noOption: IPromptOption, inputHtmlOption?: IInputHtmlOption, title?: string) => DotNet.invokeMethodAsync('Components', 'ShowPrompt', exitOnPass, content, yesOption, noOption, inputHtmlOption, title);
    componentInfo.testContext.createMessageOutput = (data) => DotNet.invokeMethodAsync('Components', 'CreateMessageOutput', data);
}

declare var navigator: Navigator;

function hasInternetConnection(): boolean {
   return navigator.onLine;
}

function saveAsFile(filename: string, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

class SetupCallbacks {
    isCancelling = false;
    isExecuting = true;
    $scope = null;
    logService = {
        postTestException: (e: any) => { }
    }
    isExitOnPass = false;
    promptOptions: IPromptOptions;

    constructor() {
    }

    public localize() {
        return DotNet.invokeMethodAsync('Components', 'Localize').then(() => console.log("Localize"));
    }

    testResultPromptListener(data: IResultPromptData) {
        //Event propagated from server to display test prompt.
        this.isExitOnPass = data.exitOnPass;

        //Add new lines between query entries to distinguish each from the other. 
        let content = "";
        if (typeof data.query !== "string" && data.query.length > 0) {
            for (let q of data.query) {
                content += (q + ' \n\r ');
            }
        }
        else
            content = data.query;

        let trueOption: IPromptOption = {
            text: data.trueoption ? data.trueoption : this.localize().STR_BUTTON_YES,
            callback: (data) => {
                //this.testService.fireTestEvent(this.currentTestExecuting.id, 'true')
            }
        }
        if ((typeof (data.ignoretruecallback) !== 'undefined') && data.ignoretruecallback) {
            trueOption.callback = (data) => { } // No Op. Prevent unecessary call backs to backend
        }

        let falseOption: IPromptOption = {
            text: data.falseoption ? data.falseoption : this.localize().STR_BUTTON_NO,
            callback: (data) => {
                //this.testService.fireTestEvent(this.currentTestExecuting.id, "false")
            }
        }
        if ((typeof (data.ignorefalseoption) !== 'undefined') && data.ignorefalseoption) {
            falseOption = null;
        }

        this.promptOptions = {
            //Default prompt setup for prompt show event origionating from the server.
            yesPromptOption: trueOption,
            noPromptOption: falseOption,
            inputHtmlOption: null,
            title: data.title ? data.title : '', // TODO this.currentTestExecuting.name, // this.localize().STR_TITLE_PC_DIAG_UEFI,
            promptContent: content
        };

        this.promptOptions.promptContent = content;

        //if (this.isExecuting)
        //this.openTestPrompt();

        // TODO
        var tc = this.$scope.testContext as ITestContext;
        tc.showPrompt(data.exitOnPass, content, trueOption, falseOption, null, data.title).then((r) => { tc.fireTestEvent(r) })
    }


    hardwareEventListener(data: any) {
        //Test data event propagated from server, passed to Gui.ts which is injected using eval().
        setTimeout(() => {
            if (this.isCancelling || !this.isExecuting) {
                return; // Do not send UI updates once test is cancelling.
            }
            if (testListener && this.$scope) {
                try {
                    testListener(data, this.$scope, this.$scope.testContext);
                }
                catch (e) {
                    //If script error occurs during interactive test execution, send exception to server to be logged. 
                    DotNet.invokeMethodAsync('Components', 'LogError', e);
                }
            }
        }, 1000);
    }
}

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


function setupTestCallbacksAndStartTest(componentInfo: ICustomComponentInfo): void {
    initLocalizer = null;
    createTestContext(componentInfo);

    testResultPromptListener = setup.testResultPromptListener.bind(setup);
    setup.$scope = { testContext: {}, Yes: "", No: "" };
    setup.$scope.testContext = componentInfo.testContext;

    eval(componentInfo.js); //Load the tests custom javascript into application code.

    if (initLocalizer) initLocalizer(componentInfo.testContext);

    hdwListener = setup.hardwareEventListener.bind(setup);

    //componentInfo.testContext.abortTest();
    //componentInfo.testContext.cancelTest();
    //componentInfo.testContext.createMessageOutput('this is a message');

}
