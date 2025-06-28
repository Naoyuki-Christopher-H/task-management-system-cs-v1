using System.Windows;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.ViewModels;

namespace task_management_system_cs_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(FileService fileService, string username) : this()
        {
            DataContext = new MainViewModel(fileService, username);
        }
    }
}