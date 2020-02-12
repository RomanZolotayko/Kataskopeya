using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Common.Constants;
using Kataskopeya.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class ArchieveViewModel : ObservableObject, IDisposable
    {
        private IEnumerable<string> _fileNames;

        public ArchieveViewModel()
        {
            var directiory = new DirectoryInfo(FileSystemPaths.DebugFolder + "SurvellianceMaterials");
            var files = directiory.GetFiles("*.avi");
            FileNames = files.Select(x => x.Name);

            PreviousWindow = new RelayCommand(GetToPreviousWindow);
        }

        public Action CloseAction { get; set; }

        public ICommand PreviousWindow { get; private set; }

        public IEnumerable<string> FileNames
        {
            get { return _fileNames; }
            set { Set(ref _fileNames, value); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public void GetToPreviousWindow()
        {
            var menu = new MenuView();
            menu.Show();
            CloseAction();
        }
    }
}
