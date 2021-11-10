using Microsoft.AspNetCore.Components;

namespace Components.Shared
{
    public partial class HpSupportComponent
    {
        [Parameter]
        public bool hasInternet { get; set; } = false;

        void contactHp()
        {
            //.contactHpSupport();
        }

        void openHpWebsite()
        {
            NavigationManager.NavigateTo("iframe/pcdiags");
        }
    }
}
