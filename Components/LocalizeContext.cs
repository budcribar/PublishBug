using ClientInterfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SureCheck.Components
{
    public class LocalizeContext : ILocalizeContext, INotifyPropertyChanged
    {
        private string language = "English";

        public bool isRTL => language == "العربية" || language == "עברית";

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Language
        {
            get => language;
            set => SetProperty<string>(ref language, value);
        }

        void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
