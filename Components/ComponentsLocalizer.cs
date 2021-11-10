using ClientInterfaces;

namespace SureCheck.Components
{
    public abstract class BaseLocalizer : ILocalizer
    {
        public abstract string GetLocalizedString(string key);
    }

    public partial class ComponentsLocalizer : BaseLocalizer
    {
        private ILocalizeContext Localize;
        private ComponentsLocalizer(ILocalizeContext localizer)
        {
            Localize = localizer;
        }


    }
}
