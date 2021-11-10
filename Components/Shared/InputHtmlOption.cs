using ClientInterfaces;

namespace Components.Shared
{
    public class InputHtmlOption : IInputHtmlOption
    {
        public bool hidden { get; set; }
        public string placeholder { get; set; } = "";
        public string type { get; set; } = "";
        public string value { get; set; } = "";
    }
}
