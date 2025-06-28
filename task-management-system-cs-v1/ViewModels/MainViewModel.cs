using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using task_management_system_cs_v1.Models;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.Utilities;

namespace task_management_system_cs_v1.ViewModels
{
    /// <summary>
    /// ViewModel for main application window handling all task management logic
    /// Implements INotifyPropertyChanged for data binding
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private readonly string _currentUser;
        private string _searchTerm = string.Empty;
        private bool _showCompletedTasks;

        /// <summary>
        /// Collection of tasks to display in UI
        /// </summary>
        public ObservableCollection<TaskItem> Tasks { get; } = new ObservableCollection<TaskItem>();

        /// <summary>
        /// Collection of command history items from last 12 hours
        /// </summary>
        public ObservableCollection<string> CommandHistory { get; } = new ObservableCollection<string>();

        // Task creation properties with default values
        public string NewTaskTitle { get; set; } = string.Empty;
        public string NewTaskDescription { get; set; } = string.Empty;
        public DateTime NewTaskDueDate { get; set; } = DateTime.Now.AddDays(1);
        public string NewTaskPriority { get; set; } = "MEDIUM";

        // Commands for UI actions
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand ToggleTaskStatusCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleViewCommand { get; }

        /// <summary>
        /// Search term for filtering tasks
        /// </summary>
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }

        /// <summary>
        /// Flag indicating whether to show completed tasks
        /// Triggers task filtering when changed
        /// </summary>
        public bool ShowCompletedTasks
        {
            get => _showCompletedTasks;
            set
            {
                _showCompletedTasks = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }

        /// <summary>
        /// Initializes ViewModel with dependencies
        /// </summary>
        public MainViewModel(FileService fileService, string username)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _currentUser = username ?? throw new ArgumentNullException(nameof(username));

            // Initialize commands
            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ToggleTaskStatusCommand = new RelayCommand(ToggleTaskStatus);
            SearchCommand = new RelayCommand(FilterTasks);
            ToggleViewCommand = new RelayCommand(ToggleView);

            // Load initial data
            LoadTasks();
            LoadCommandHistory();
        }

        /// <summary>
        /// Adds new task with validation
        /// </summary>
        private void AddTask(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
            {
                MessageBox.Show("Task title is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newTask = new TaskItem
            {
                Title = NewTaskTitle,
                Description = NewTaskDescription,
                DueDate = NewTaskDueDate,
                Priority = NewTaskPriority,
                IsCompleted = false
            };

            Tasks.Add(newTask);
            SaveTasks();
            _fileService.LogCommand(_currentUser, $"Added task: {NewTaskTitle}");
            LoadCommandHistory();

            // Reset form fields
            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
            OnPropertyChanged(nameof(NewTaskTitle));
            OnPropertyChanged(nameof(NewTaskDescription));
        }

        /// <summary>
        /// Deletes specified task
        /// </summary>
        private void DeleteTask(object parameter)
        {
            if (parameter is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
                _fileService.LogCommand(_currentUser, $"Deleted task: {task.Title}");
                LoadCommandHistory();
            }
        }

        /// <summary>
        /// Toggles task completion status with date tracking
        /// </summary>
        private void ToggleTaskStatus(object parameter)
        {
            if (parameter is TaskItem task)
            {
                task.IsCompleted = !task.IsCompleted;
                task.CompletedDate = task.IsCompleted ? DateTime.Now : null;
                SaveTasks();
                _fileService.LogCommand(_currentUser,
                    $"Marked task as {(task.IsCompleted ? "completed" : "pending")}: {task.Title}");
                FilterTasks();
                LoadCommandHistory();
            }
        }

        /// <summary>
        /// Filters tasks based on search term and view mode
        /// </summary>
        private void FilterTasks(object parameter = null)
        {
            var allTasks = _fileService.GetUserTasks(_currentUser);
            var filteredTasks = allTasks
                .Where(t => ShowCompletedTasks || !t.IsCompleted)
                .Where(t => string.IsNullOrWhiteSpace(SearchTerm) ||
                           t.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                           t.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority == "HIGH" ? 0 : t.Priority == "MEDIUM" ? 1 : 2);

            Tasks.Clear();
            foreach (var task in filteredTasks)
            {
                Tasks.Add(task);
            }
        }

        /// <summary>
        /// Toggles between showing completed/pending tasks
        /// </summary>
        private void ToggleView(object parameter)
        {
            ShowCompletedTasks = !ShowCompletedTasks;
        }

        /// <summary>
        /// Loads tasks from file service
        /// </summary>
        private void LoadTasks()
        {
            Tasks.Clear();
            var tasks = _fileService.GetUserTasks(_currentUser)
                .Where(t => !t.IsCompleted);

            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }

        /// <summary>
        /// Loads command history from past 12 hours
        /// </summary>
        private void LoadCommandHistory()
        {
            CommandHistory.Clear();
            var commands = _fileService.GetLastCommands(_currentUser)
                .Select(c => c.Split('|')[2]); // Extract just the command text

            foreach (var cmd in commands)
            {
                CommandHistory.Add(cmd);
            }
        }

        /// <summary>
        /// Saves current task list to file
        /// </summary>
        private void SaveTasks()
        {
            _fileService.SaveUserTasks(_currentUser, Tasks.ToList());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises PropertyChanged event for data binding
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}