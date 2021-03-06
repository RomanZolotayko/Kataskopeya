﻿using Kataskopeya.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Kataskopeya.Views
{
    public partial class VideoPlayerView : Window
    {
        public VideoPlayerView()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var viewModel = new VideoPlayerViewModel();
            this.DataContext = viewModel;
            this.ArchivePlayer.LoadedBehavior = MediaState.Manual;
            this.ArchivePlayer.UnloadedBehavior = MediaState.Manual;
            viewModel.Player = this.ArchivePlayer;
            viewModel.Player.Play();
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
            viewModel.Player.MediaOpened += viewModel.GetMediaDuration;
            viewModel.Player.MediaOpened += viewModel.HandlerTimerTick;
            DurationSlider.LostMouseCapture += viewModel.SliderLostMouseCaptureHandler;
        }
    }
}
