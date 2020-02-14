using Kataskopeya.ViewModels;
using System;
using System.Windows;

namespace Kataskopeya.Views
{
    public partial class NewCameraView : Window
    {
        public NewCameraView()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var viewModel = new NewCameraViewModel();
            this.DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
        }
    }
}
