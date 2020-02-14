using GalaSoft.MvvmLight;
using Kataskopeya.Commands;
using Kataskopeya.ViewModels;
using Kataskopeya.Views;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Kataskopeya.Models
{
    public class ArchiveVideo : ObservableObject, IDisposable
    {
        private BitmapImage _previewImage;
        private ICommand _openVideoCommand;

        public string Name { get; set; }

        public byte[] Stream { get; set; }

        public BitmapImage PreviewImage
        {
            get { return _previewImage; }
            set { Set(ref _previewImage, value); }
        }
        public ICommand OpenVideoCommand
        {
            get
            {
                return _openVideoCommand ?? (_openVideoCommand = new BaseCommandHandler(param => PlayVideo(param), true));
            }
        }

        public void PlayVideo(object param)
        {
            var videoPlayer = new VideoPlayerView();
            ((VideoPlayerViewModel)videoPlayer.DataContext).Filename = param as string;
            videoPlayer.ShowDialog();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
