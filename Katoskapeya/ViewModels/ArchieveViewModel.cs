using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Commands;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class ArchieveViewModel : ObservableObject, IDisposable
    {
        private IEnumerable<string> _fileNames;
        private ObservableCollection<ArchiveVideo> _archivedVideos;
        private bool _isNextPageButtonVisible;
        private int _pageNumber = 0;
        private int _pagePayload = 18;
        private PagingProcessor _pagingProcessor;
        private bool _isOrderedByDescending;
        private ICommand _searchCommand;
        private Visibility _searchButtonVisability;

        public ArchieveViewModel()
        {
            SearchButtonVisability = Visibility.Hidden;
            _pagingProcessor = new PagingProcessor();
            PreviousWindow = new RelayCommand(GetToPreviousWindow);
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            SortByNameCommand = new RelayCommand(SortByName);
            SortByDateCommand = new RelayCommand(SortByDate);
            ClearSearchResultsCommand = new RelayCommand(ClearSearchResults);
            VideoDirectory = new DirectoryInfo(FileSystemPaths.DebugFolder + FolderNames.VideoMaterialsFolder);
            LoadArchivedVideos();
        }

        public Action CloseAction { get; set; }

        public ICommand PreviousWindow { get; private set; }

        public ICommand NextPageCommand { get; private set; }

        public ICommand PreviousPageCommand { get; private set; }

        public ICommand SortByNameCommand { get; private set; }

        public ICommand SortByDateCommand { get; private set; }

        public ICommand ClearSearchResultsCommand { get; private set; }

        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new BaseCommandHandler(param => SearchHandler(param), true));
            }
        }

        public IEnumerable<string> FileNames
        {
            get { return _fileNames; }
            set { Set(ref _fileNames, value); }
        }

        public Visibility SearchButtonVisability
        {
            get { return _searchButtonVisability; }
            set { Set(ref _searchButtonVisability, value); }
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
                            .GetPagedDirectoryFiles(VideoDirectory, _pageNumber, _pagePayload, PagingOperations.None, FileExtensions.AviExtension);

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

        private void SearchHandler(object param)
        {
            var textBox = param as TextBox;
            var searchQuery = textBox.Text;

            ArchivedVideos = ArchivedVideos.FilterObservableCollection(searchQuery);
            SearchButtonVisability = Visibility.Visible;

            RaisePropertyChanged("ArchivedVideos");
            RaisePropertyChanged("SearchButtonVisability");
        }

        private void ClearSearchResults()
        {
            LoadArchivedVideos();

            SearchButtonVisability = Visibility.Hidden;
            RaisePropertyChanged("SearchButtonVisability");
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
                    Name = file.Name.Split('.').First(),
                    CreationDate = file.Name.GetDateFromFileName()
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
            if (_isOrderedByDescending)
            {
                ArchivedVideos = ArchivedVideos.SortObservableCollection(OrderingTypes.Ascending, x => x.Name);
                _isOrderedByDescending = false;
            }
            else
            {
                ArchivedVideos = ArchivedVideos.SortObservableCollection(OrderingTypes.Descending, x => x.Name);
                _isOrderedByDescending = true;
            }

            RaisePropertyChanged("ArchivedVideos");
        }

        private void SortByDate()
        {
            if (_isOrderedByDescending)
            {
                ArchivedVideos = ArchivedVideos.SortObservableCollection(OrderingTypes.Ascending, x => x.CreationDate);
                _isOrderedByDescending = false;
            }
            else
            {
                ArchivedVideos = ArchivedVideos.SortObservableCollection(OrderingTypes.Descending, x => x.CreationDate);
                _isOrderedByDescending = true;
            }

            RaisePropertyChanged("ArchivedVideos");
        }

        public void Dispose()
        {
            CloseAction();
        }

    }
}
