using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using task_management_system_cs_v1.Models;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.Utilities;

namespace task_management_system_cs_v1.ViewModels
{
    /// <summary>
    /// ViewModel for handling login and registration logic
    /// Implements INotifyPropertyChanged for data binding
    /// </summary>
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;

        // Initialize all fields with default values to satisfy null checks
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _registerUsername = string.Empty;
        private string _registerPassword = string.Empty;
        private string _registerConfirmPassword = string.Empty;

        /// <summary>
        /// Current username for login
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current password for login
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// New username for registration
        /// </summary>
        public string RegisterUsername
        {
            get => _registerUsername;
            set
            {
                _registerUsername = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// New password for registration
        /// </summary>
        public string RegisterPassword
        {
            get => _registerPassword;
            set
            {
                _registerPassword = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Password confirmation for registration
        /// </summary>
        public string RegisterConfirmPassword
        {
            get => _registerConfirmPassword;
            set
            {
                _registerConfirmPassword = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command for handling login
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Command for handling registration
        /// </summary>
        public ICommand RegisterCommand { get; }

        /// <summary>
        /// Initializes a new instance of the LoginViewModel
        /// </summary>
        /// <param name="fileService">File service for user operations</param>
        public LoginViewModel(FileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }

        /// <summary>
        /// Handles login logic
        /// </summary>
        private void Login(object? parameter)
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

                if (parameter is Window window)
                {
                    window.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        /// <summary>
        /// Handles registration logic
        /// </summary>
        private void Register(object? parameter)
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
            }
            else
            {
                MessageBox.Show("Username already exists.");
            }
        }

        /// <summary>
        /// Validates password meets requirements
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>True if password is valid</returns>
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

        /// <summary>
        /// Event for property change notifications
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}