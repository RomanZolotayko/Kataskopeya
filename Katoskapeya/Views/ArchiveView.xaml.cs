using Kataskopeya.ViewModels;
using System;
using System.Windows;

namespace Kataskopeya.Views
{
    public partial class ArchiveView : Window
    {
        public ArchiveView()
        {
            InitializeComponent();
            var viewModel = new ArchieveViewModel();
            this.DataContext = viewModel;
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
        }

    }
}
