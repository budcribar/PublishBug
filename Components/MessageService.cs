using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace SureCheck.Components
{
    public class MessageService : INotifyPropertyChanged
    {
        private string message = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Message
        {
            get => message;
            set => SetProperty<string>(ref message, value);
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
