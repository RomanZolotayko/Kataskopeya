using GalaSoft.MvvmLight;
using System;

namespace Kataskopeya.ViewModels
{
    public class SettingsViewModel : ObservableObject, IDisposable
    {
        public Action CloseAction { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
