using Kataskopeya.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Kataskopeya.Views
{
    public partial class LoginMainView : Window
    {
        public LoginMainView()
        {
            InitializeComponent();
            var vm = new LoginViewModel();
            this.DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
