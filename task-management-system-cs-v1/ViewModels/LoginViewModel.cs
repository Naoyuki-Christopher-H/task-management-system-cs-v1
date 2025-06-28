using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using task_management_system_cs_v1.Models;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.Utilities;
using task_management_system_cs_v1.Views;

namespace task_management_system_cs_v1.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private string _username;
        private string _password;
        private string _registerUsername;
        private string _registerPassword;
        private string _registerConfirmPassword;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string RegisterUsername
        {
            get => _registerUsername;
            set
            {
                _registerUsername = value;
                OnPropertyChanged();
            }
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set
            {
                _registerPassword = value;
                OnPropertyChanged();
            }
        }

        public string RegisterConfirmPassword
        {
            get => _registerConfirmPassword;
            set
            {
                _registerConfirmPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(FileService fileService)
        {
            _fileService = fileService;
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }

        private void Login(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            var user = new User { Username = Username, Password = Password };
            if (_fileService.ValidateUser(user))
            {
                var mainWindow = new MainWindow(_fileService, Username);
                mainWindow.Show();
                CloseWindow(parameter);
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private void Register(object parameter)
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername) ||
                string.IsNullOrWhiteSpace(RegisterPassword) ||
                string.IsNullOrWhiteSpace(RegisterConfirmPassword))
            {
                MessageBox.Show("Please fill all registration fields.");
                return;
            }

            if (RegisterPassword != RegisterConfirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            if (!ValidatePassword(RegisterPassword))
            {
                MessageBox.Show("Password must be 8-12 characters long and contain both letters and numbers.");
                return;
            }

            var newUser = new User { Username = RegisterUsername, Password = RegisterPassword };
            if (_fileService.RegisterUser(newUser))
            {
                MessageBox.Show("Registration successful. Please login with your new credentials.");
                Username = RegisterUsername;
                Password = RegisterPassword;
                RegisterUsername = string.Empty;
                RegisterPassword = string.Empty;
                RegisterConfirmPassword = string.Empty;
                OnPropertyChanged(nameof(RegisterUsername));
                OnPropertyChanged(nameof(RegisterPassword));
                OnPropertyChanged(nameof(RegisterConfirmPassword));
            }
            else
            {
                MessageBox.Show("Username already exists.");
            }
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 8 || password.Length > 12)
                return false;

            bool hasLetter = false;
            bool hasNumber = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasNumber = true;
            }

            return hasLetter && hasNumber;
        }

        private void CloseWindow(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}