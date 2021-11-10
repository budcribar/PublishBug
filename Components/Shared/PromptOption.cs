using ClientInterfaces;

namespace Components.Shared
{
    public class PromptOption : IPromptOption
    {
        public string text { get; set; } = "";
        //public Action<object> callback { get; set; } = (x) => { };
    }
}
