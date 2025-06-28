using System.Windows;
using System.Windows.Controls;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.ViewModels;

namespace task_management_system_cs_v1
{
    public partial class LoginWindow : Window
    {
        // Declare fields as nullable to satisfy null checks
        private PasswordBox? passwordBox;
        private PasswordBox? registerPasswordBox;
        private PasswordBox? registerConfirmPasswordBox;

        public LoginWindow()
        {
            InitializeComponent();
            var fileService = new FileService();
            DataContext = new LoginViewModel(fileService);

            // Initialize password boxes after components are loaded
            passwordBox = (PasswordBox)FindName("PasswordBox")!;
            registerPasswordBox = (PasswordBox)FindName("RegisterPasswordBox")!;
            registerConfirmPasswordBox = (PasswordBox)FindName("RegisterConfirmPasswordBox")!;

            // Add null checks before attaching event handlers
            passwordBox.PasswordChanged += (sender, e) =>
                ((LoginViewModel)DataContext).Password = passwordBox.Password;

            registerPasswordBox.PasswordChanged += (sender, e) =>
                ((LoginViewModel)DataContext).RegisterPassword = registerPasswordBox.Password;

            registerConfirmPasswordBox.PasswordChanged += (sender, e) =>
                ((LoginViewModel)DataContext).RegisterConfirmPassword = registerConfirmPasswordBox.Password;
        }
    }
}