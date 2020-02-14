using Kataskopeya.ViewModels;
using System;
using System.Windows;

namespace Kataskopeya.Views
{
    public partial class CameraDetailsView : Window
    {
        public CameraDetailsView()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var viewModel = new CameraDetailsViewModel();
            this.DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
        }
    }
}
