using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using task_management_system_cs_v1.Models;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.Utilities;

namespace task_management_system_cs_v1.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _registerUsername = string.Empty;
        private string _registerPassword = string.Empty;
        private string _registerConfirmPassword = string.Empty;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string RegisterUsername
        {
            get => _registerUsername;
            set { _registerUsername = value; OnPropertyChanged(); }
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set { _registerPassword = value; OnPropertyChanged(); }
        }

        public string RegisterConfirmPassword
        {
            get => _registerConfirmPassword;
            set { _registerConfirmPassword = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(FileService fileService)
        {
            _fileService = fileService;
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }

        // ... rest of the methods remain the same ...

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}