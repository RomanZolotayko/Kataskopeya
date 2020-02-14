using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Common.Constants;
using Kataskopeya.Common.Enums;
using Kataskopeya.Extensions;
using Kataskopeya.Models;
using Kataskopeya.Processors;
using Kataskopeya.Views;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class ArchieveViewModel : ObservableObject, IDisposable
    {
        private IEnumerable<string> _fileNames;
        private ObservableCollection<ArchiveVideo> _archivedVideos;
        private bool _isNextPageButtonVisible;
        private int _pageNumber;
        private int _pagePayload = 18;
        private PagingProcessor _pagingProcessor;
        private bool _isOrderedByDescending;

        public ArchieveViewModel()
        {
            _pagingProcessor = new PagingProcessor();
            PreviousWindow = new RelayCommand(GetToPreviousWindow);
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            SortByNameCommand = new RelayCommand(SortByName);
            VideoDirectory = new DirectoryInfo(FileSystemPaths.DebugFolder + "SurvellianceMaterials");
            LoadArchivedVideos();
        }

        public Action CloseAction { get; set; }

        public ICommand PreviousWindow { get; private set; }

        public ICommand NextPageCommand { get; private set; }

        public ICommand PreviousPageCommand { get; private set; }

        public ICommand SortByNameCommand { get; private set; }

        public IEnumerable<string> FileNames
        {
            get { return _fileNames; }
            set { Set(ref _fileNames, value); }
        }

        public bool IsNextPageButtonVisible
        {
            get { return _isNextPageButtonVisible; }
            set { Set(ref _isNextPageButtonVisible, value); }
        }

        public ObservableCollection<ArchiveVideo> ArchivedVideos
        {
            get { return _archivedVideos; }
            set { Set(ref _archivedVideos, value); }
        }

        public DirectoryInfo VideoDirectory { get; set; }

        private void LoadArchivedVideos()
        {
            ArchivedVideos = new ObservableCollection<ArchiveVideo>();

            var pagedDirectoryFiles = _pagingProcessor
                            .GetPagedDirectoryFiles(VideoDirectory, _pageNumber, _pagePayload, PagingOperations.StartPage, FileExtensions.AviExtension);

            if (pagedDirectoryFiles.Files.Count() > 18)
            {
                IsNextPageButtonVisible = true;
            }
            else
            {
                IsNextPageButtonVisible = false;
            }

            CreateaArchivedVideos(pagedDirectoryFiles.Files);

            FileNames = pagedDirectoryFiles.Files.Select(x => x.Name);
            IsNextPageButtonVisible = pagedDirectoryFiles.IsNextPage;
        }

        private void NextPage()
        {
            ArchivedVideos.Clear();

            var pagedDirectoryFiles = _pagingProcessor
                .GetPagedDirectoryFiles(VideoDirectory, _pageNumber, _pagePayload, PagingOperations.Next, FileExtensions.AviExtension);

            CreateaArchivedVideos(pagedDirectoryFiles.Files);

            IsNextPageButtonVisible = pagedDirectoryFiles.IsNextPage;
        }

        private void PreviousPage()
        {
            ArchivedVideos.Clear();

            var pagedDirectoryFiles = _pagingProcessor
                .GetPagedDirectoryFiles(VideoDirectory, _pageNumber, _pagePayload, PagingOperations.Previous, FileExtensions.AviExtension);

            CreateaArchivedVideos(pagedDirectoryFiles.Files);

            IsNextPageButtonVisible = pagedDirectoryFiles.IsNextPage;
        }

        public void GetToPreviousWindow()
        {
            var menu = new MenuView();
            menu.Show();
            CloseAction();
        }

        private void CreateaArchivedVideos(IEnumerable<FileInfo> filesInfo)
        {
            foreach (var file in filesInfo)
            {
                var archivedVideo = new ArchiveVideo
                {
                    Fullname = file.FullName,
                    Stream = File.ReadAllBytes(file.FullName),
                    Name = file.Name.Split('.').First()
                };

                ShellFile thumbNail = ShellFile.FromFilePath(archivedVideo.Fullname);
                var thumbLarge = thumbNail.Thumbnail.LargeBitmap;
                archivedVideo.PreviewImage = thumbLarge.ToBitmapImage();

                ArchivedVideos.Add(archivedVideo);

                RaisePropertyChanged("ArchivedVideos");
            }
        }

        private void SortByName()
        {
            ArchivedVideos.OrderByDescending(av => av.Name);
            RaisePropertyChanged("ArchivedVideos");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
