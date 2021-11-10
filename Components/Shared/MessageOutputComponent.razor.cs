using Microsoft.AspNetCore.Components;

namespace Components.Shared
{
    public partial class MessageOutputComponent
    {
        [Parameter]
        public string messageContent { get; set; } = "";

    }
}
