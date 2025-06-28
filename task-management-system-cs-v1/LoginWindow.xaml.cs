using System.Windows;
using System.Windows.Controls;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.ViewModels;

namespace task_management_system_cs_v1
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private PasswordBox passwordBox;
        private PasswordBox registerPasswordBox;
        private PasswordBox registerConfirmPasswordBox;

        public LoginWindow()
        {
            InitializeComponent();
            var fileService = new FileService();
            DataContext = new LoginViewModel(fileService);

            // Hook up password boxes after initialization
            passwordBox = (PasswordBox)FindName("PasswordBox");
            registerPasswordBox = (PasswordBox)FindName("RegisterPasswordBox");
            registerConfirmPasswordBox = (PasswordBox)FindName("RegisterConfirmPasswordBox");

            if (passwordBox != null)
                passwordBox.PasswordChanged += (sender, e) =>
                    ((LoginViewModel)DataContext).Password = passwordBox.Password;

            if (registerPasswordBox != null)
                registerPasswordBox.PasswordChanged += (sender, e) =>
                    ((LoginViewModel)DataContext).RegisterPassword = registerPasswordBox.Password;

            if (registerConfirmPasswordBox != null)
                registerConfirmPasswordBox.PasswordChanged += (sender, e) =>
                    ((LoginViewModel)DataContext).RegisterConfirmPassword = registerConfirmPasswordBox.Password;
        }
    }
}