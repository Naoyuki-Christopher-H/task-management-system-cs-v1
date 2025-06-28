using System.Windows;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.ViewModels;

namespace task_management_system_cs_v1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(FileService fileService, string username)
        {
            InitializeComponent();
            DataContext = new MainViewModel(fileService, username);
        }
    }
}