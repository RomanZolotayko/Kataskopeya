using GalaSoft.MvvmLight;
using Kataskopeya.Commands;
using Kataskopeya.EF;
using Kataskopeya.Views;
using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class LoginViewModel : ObservableObject, IDisposable
    {
        private readonly KataskopeyaContext _context;
        private string _username;
        private ICommand _loginCommand;

        public LoginViewModel()
        {
            _context = new KataskopeyaContext();
        }

        public ICommand Login
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new BaseCommandHandler(param => LoginHandler(param), true));
            }
        }

        public Action CloseAction { get; set; }

        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }

        public string Password { get; set; }

        private async void LoginHandler(object param)
        {
            var passwordBox = param as PasswordBox;
            Password = passwordBox.Password;

            if (string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Fields couldn't be empty.", "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == Username && u.Password == Password);

            if (user == null)
            {
                MessageBox.Show("Email or password incorrect.", "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var camerasView = new MenuView();
            camerasView.Show();
            CloseAction();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
