using ClientInterfaces;

namespace Components.Shared
{
    public class PromptOptions : IPromptOptions
    {
        public string title { get; set; } = "";
        public string promptContent { get; set; } = "";
        public IPromptOption yesPromptOption { get; set; } = new PromptOption();
        public IPromptOption noPromptOption { get; set; } = new PromptOption();
        public IInputHtmlOption inputHtmlOption { get; set; } = new InputHtmlOption();
    }
}
