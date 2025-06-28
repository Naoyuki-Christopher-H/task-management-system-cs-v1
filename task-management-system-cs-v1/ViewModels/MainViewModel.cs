using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using task_management_system_cs_v1.Models;
using task_management_system_cs_v1.Services;
using task_management_system_cs_v1.Utilities;

namespace task_management_system_cs_v1.ViewModels
{
    /// <summary>
    /// ViewModel for the main task management interface
    /// Implements INotifyPropertyChanged for data binding
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private readonly string _currentUser;

        // Initialize fields with default values
        private string _searchTerm = string.Empty;
        private bool _showCompletedTasks;

        /// <summary>
        /// Collection of tasks to display
        /// </summary>
        public ObservableCollection<TaskItem> Tasks { get; } = new ObservableCollection<TaskItem>();

        /// <summary>
        /// Collection of command history items
        /// </summary>
        public ObservableCollection<string> CommandHistory { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Command for adding new tasks
        /// </summary>
        public ICommand AddTaskCommand { get; }

        /// <summary>
        /// Command for deleting tasks
        /// </summary>
        public ICommand DeleteTaskCommand { get; }

        /// <summary>
        /// Command for toggling task status
        /// </summary>
        public ICommand ToggleTaskStatusCommand { get; }

        /// <summary>
        /// Command for searching tasks
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// Command for toggling view between completed/pending tasks
        /// </summary>
        public ICommand ToggleViewCommand { get; }

        /// <summary>
        /// Description for new task
        /// </summary>
        public string NewTaskDescription { get; set; } = string.Empty;

        /// <summary>
        /// Due date for new task
        /// </summary>
        public DateTime NewTaskDueDate { get; set; } = DateTime.Now.AddDays(1);

        /// <summary>
        /// Priority for new task
        /// </summary>
        public string NewTaskPriority { get; set; } = "Medium";

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
            }
        }

        /// <summary>
        /// Flag indicating whether to show completed tasks
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
        /// Initializes a new instance of the MainViewModel
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
        /// Loads tasks from file service
        /// </summary>
        private void LoadTasks()
        {
            Tasks.Clear();
            var tasks = _fileService.GetUserTasks(_currentUser);
            foreach (var task in tasks.Where(t => !t.IsCompleted))
            {
                Tasks.Add(task);
            }
        }

        /// <summary>
        /// Loads command history from file service
        /// </summary>
        private void LoadCommandHistory()
        {
            CommandHistory.Clear();
            var commands = _fileService.GetLastCommands(_currentUser);
            foreach (var cmd in commands)
            {
                CommandHistory.Add(cmd.Split('|')[2]); // Just show the command part
            }
        }

        /// <summary>
        /// Adds a new task
        /// </summary>
        private void AddTask(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewTaskDescription))
                return;

            var newTask = new TaskItem
            {
                Description = NewTaskDescription,
                DueDate = NewTaskDueDate,
                Priority = NewTaskPriority,
                IsCompleted = false
            };

            Tasks.Add(newTask);
            SaveTasks();
            _fileService.LogCommand(_currentUser, $"Added task: {NewTaskDescription}");
            LoadCommandHistory();

            // Reset new task fields
            NewTaskDescription = string.Empty;
            OnPropertyChanged(nameof(NewTaskDescription));
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        private void DeleteTask(object? parameter)
        {
            if (parameter is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
                _fileService.LogCommand(_currentUser, $"Deleted task: {task.Description}");
                LoadCommandHistory();
            }
        }

        /// <summary>
        /// Toggles task completion status
        /// </summary>
        private void ToggleTaskStatus(object? parameter)
        {
            if (parameter is TaskItem task)
            {
                task.IsCompleted = !task.IsCompleted;
                SaveTasks();
                _fileService.LogCommand(_currentUser,
                    $"Marked task as {(task.IsCompleted ? "completed" : "pending")}: {task.Description}");
                FilterTasks();
                LoadCommandHistory();
            }
        }

        /// <summary>
        /// Filters tasks based on search term and view mode
        /// </summary>
        private void FilterTasks(object? parameter = null)
        {
            Tasks.Clear();
            var allTasks = _fileService.GetUserTasks(_currentUser);
            var filteredTasks = allTasks.Where(t =>
                (ShowCompletedTasks || !t.IsCompleted) &&
                (string.IsNullOrWhiteSpace(SearchTerm) ||
                 t.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(t => t.DueDate)
                .ThenByDescending(t => t.Priority);

            foreach (var task in filteredTasks)
            {
                Tasks.Add(task);
            }
        }

        /// <summary>
        /// Toggles between showing completed/pending tasks
        /// </summary>
        private void ToggleView(object? parameter)
        {
            ShowCompletedTasks = !ShowCompletedTasks;
        }

        /// <summary>
        /// Saves tasks to file
        /// </summary>
        private void SaveTasks()
        {
            _fileService.SaveUserTasks(_currentUser, Tasks.ToList());
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