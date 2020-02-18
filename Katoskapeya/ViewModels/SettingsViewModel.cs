using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Models;
using Kataskopeya.Services;
using Kataskopeya.Views;
using System;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class SettingsViewModel : ObservableObject, IDisposable
    {
        public int _durationOfRecordedVideoChunk;
        private ApplicationSettings _settings;


        public SettingsViewModel()
        {
            _settings = SettingsService.GetApplicationSettings();
            FillLocalSettings(_settings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            PreviousWindow = new RelayCommand(GetToPreviousWindow);
        }

        public int DurationOfRecordedVideoChunk
        {
            get { return _durationOfRecordedVideoChunk; }
            set
            {
                Set(ref _durationOfRecordedVideoChunk, value);
                _settings.DurationOfRecordedVideoChunk = _durationOfRecordedVideoChunk;
            }
        }

        public Action CloseAction { get; set; }

        public ICommand SaveSettingsCommand { get; set; }

        public ICommand PreviousWindow { get; private set; }

        public void GetToPreviousWindow()
        {
            var menu = new MenuView();
            menu.Show();
            CloseAction();
        }

        private void SaveSettings()
        {
            SettingsService.SaveSettings(_settings);
        }

        private void FillLocalSettings(ApplicationSettings settings)
        {
            DurationOfRecordedVideoChunk = settings.DurationOfRecordedVideoChunk;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
