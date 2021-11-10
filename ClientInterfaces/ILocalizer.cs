using System.ComponentModel;

namespace ClientInterfaces
{
    public interface ILocalizer
    {
        string GetLocalizedString(string key);
    }
    public interface ILocalizeContext
    {
        string Language { get; set; }
        bool isRTL { get; }
        event PropertyChangedEventHandler? PropertyChanged;

    }
}
