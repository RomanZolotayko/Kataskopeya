using Kataskopeya.ViewModels;
using System;
using System.Windows;

namespace Kataskopeya.Views
{
    public partial class MenuView : Window
    {
        public MenuView()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var viewModel = new MenuViewModel();
            this.DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
        }
    }
}
