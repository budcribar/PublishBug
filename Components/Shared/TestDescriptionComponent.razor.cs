using ClientInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using ToolFrameworkPackage;

namespace Components.Shared
{
    public partial class TestDescriptionComponent : ComponentBase
    {
        List<ToolPacket> toolList = new();

        public List<IUIToolPacket> uiToolList { get; set; } = new();

        bool isInitialized = false;

        void CheckForEnter(KeyboardEventArgs args, Action action)
        {
            if (args.Code == "Enter")
                action.Invoke();
        }

        public void loadAboutInfo()
        {
            this.isInitialized = false;
            this.getToolsAbout(this);
        }


        public string formatVersionInfo(string versionNumber)
        {
            var formattedString = Localizer.STR_HELP_VERSION.Replace("%s", versionNumber);
            return formattedString;

        }

        public void toggleToolContent(IUIToolPacket uiTool)
        {
            foreach (var tool in this.uiToolList)
            {
                tool.isExpanded = tool.tool.Name == uiTool.tool.Name ? !uiTool.isExpanded : false;
            }
        }

        async void getToolsAbout(TestDescriptionComponent tac)
        {
            this.uiToolList = new();
            this.toolList = new();

            await LoadTask;
            this.toolList = Framework.ToolsAbout().ToList();

            this.uiToolList = toolList.Select(x => new UIToolPacket(false, x) as IUIToolPacket).OrderBy(x => x.tool.Name).ToList();
            this.isInitialized = true;
            StateHasChanged();
        }

        private int indexOfToolAbout(string toolName)
        {
            for (var i = 0; i < this.uiToolList.Count; i++)
            {
                var toolAbout = this.uiToolList[i];
                if (toolAbout.tool.Name == toolName)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
