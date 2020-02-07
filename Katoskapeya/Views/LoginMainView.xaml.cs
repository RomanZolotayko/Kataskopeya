using System.Windows;
using System.Windows.Input;

namespace Kataskopeya.Views
{
    public partial class LoginMainView : Window
    {
        public LoginMainView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
