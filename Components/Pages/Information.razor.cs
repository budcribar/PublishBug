using ClientInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Components.Pages
{
    public partial class Information
    {
        private class SystemInfoEntry : ISystemInfoEntry
        {
            public string Name { get; }
            public List<KeyValuePair<string, List<KeyValuePair<string, string>>>> Data { get; }

            public SystemInfoEntry(string name, List<KeyValuePair<string, List<KeyValuePair<string, string>>>> data)
            {
                Name = name;
                Data = data;
            }
        }

        ISystemInfoEntry? currentInfoEntry = null;
        ISystemInfoEntry[] systemInfoArray = Array.Empty<ISystemInfoEntry>();
        public bool isLoading;

        public void NavToInfoPage(ISystemInfoEntry infoEntry)
        {
            this.currentInfoEntry = infoEntry;
        }

        void hideInfoPage()
        {
            this.currentInfoEntry = null;
        }

        protected async override Task OnInitializedAsync()
        {
            isLoading = true;
            await LoadTask;

            this.systemInfoArray = SystemInfo.LocalizedSystemInfo.Select(x => new SystemInfoEntry(x.Key, x.Value)).ToArray();
            this.currentInfoEntry = systemInfoArray.FirstOrDefault();

            this.isLoading = false;
            await base.OnInitializedAsync();
        }

    }
}