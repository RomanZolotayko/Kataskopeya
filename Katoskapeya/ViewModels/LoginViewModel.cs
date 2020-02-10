using GalaSoft.MvvmLight;
using Kataskopeya.Commands;
using Kataskopeya.EF;
using Kataskopeya.Views;
using System;
using System.Data.Entity;
using System.Linq;
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
                MessageBox.Show("Login error");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == Username && u.Password == Password);

            if (user == null)
            {
                MessageBox.Show("User not found");
            }

            var camerasView = new CamerasMainView();
            camerasView.Show();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
