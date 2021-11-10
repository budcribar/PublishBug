using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Components.Shared
{
    public partial class Modal
    {
        [Parameter]
        public RenderFragment? Title { get; set; }

        [Parameter]
        public RenderFragment? Body { get; set; }

        [Parameter]
        public RenderFragment? Footer { get; set; }

        private string modalDisplay = "display:none;";
        private string modalClass = "";
        private bool showBackdrop = false;
        public async Task Open()
        {
            modalDisplay = "display:block;";
            await Task.Delay(100);
            modalClass = "show";
            showBackdrop = true;
        }

        public async Task Close()
        {
            modalClass = "";
            await Task.Delay(250);
            modalDisplay = "display:none";
            showBackdrop = false;
        }
    }
}