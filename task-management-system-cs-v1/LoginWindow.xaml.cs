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
        public LoginWindow()
        {
            InitializeComponent();
            var fileService = new FileService();
            DataContext = new LoginViewModel(fileService);
            PasswordBox.PasswordChanged += (sender, e) => ((LoginViewModel)DataContext).Password = PasswordBox.Password;
            RegisterPasswordBox.PasswordChanged += (sender, e) => ((LoginViewModel)DataContext).RegisterPassword = RegisterPasswordBox.Password;
            RegisterConfirmPasswordBox.PasswordChanged += (sender, e) => ((LoginViewModel)DataContext).RegisterConfirmPassword = RegisterConfirmPasswordBox.Password;
        }
    }
}