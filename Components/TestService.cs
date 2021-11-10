using Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SureCheck.Components
{
    public class TestService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool isExecuting = false;
        public bool IsExecuting
        {
            get => isExecuting;
            set => SetProperty<bool>(ref isExecuting, value);
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

        public ValueTask<bool> CheckInternetConnection(IJSRuntime iJSRuntime) { return Interop.HasInternetConnection(iJSRuntime); }

    }
}
