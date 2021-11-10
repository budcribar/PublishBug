using Microsoft.AspNetCore.Components.Web;
using System;
using System.Linq;
using ToolFrameworkPackage;

namespace Components.Pages
{
    public partial class LanguageSelect
    {
        string[] langList = Array.Empty<string>();
        bool isExecuting = false;
        DisplayedHistoryRecord2 dhr = new DisplayedHistoryRecord2();

        void CheckForEnter(KeyboardEventArgs args, string language)
        {
            if (args.Code == "Enter")
                LocalizeContext.Language = language;
        }

        void selectLanguage(MouseEventArgs evnt, string lang)
        {
            if (lang == this.LocalizeContext.Language)
            {
                //evnt.preventDefault(); // TODO
                return;
            }

            LocalizeContext.Language = lang;
        }
        protected override void OnInitialized()
        {
            langList = SupportedLanguages.Values.Select(x => x.Value).Distinct().ToArray();
        }
    }
}