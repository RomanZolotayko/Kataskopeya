using Kataskopeya.ViewModels;
using System;
using System.Windows;

namespace Kataskopeya.Views
{
    public partial class CamerasMainView : Window
    {
        public CamerasMainView()
        {
            InitializeComponent();
            var viewModel = new CamerasMainViewModel();
            this.DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
        }
    }
}
