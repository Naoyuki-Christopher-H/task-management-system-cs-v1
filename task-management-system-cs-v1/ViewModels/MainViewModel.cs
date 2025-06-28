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
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private string _currentUser;
        private string _searchTerm;
        private bool _showCompletedTasks;

        public ObservableCollection<TaskItem> Tasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<string> CommandHistory { get; set; } = new ObservableCollection<string>();
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand ToggleTaskStatusCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleViewCommand { get; }

        public string NewTaskDescription { get; set; }
        public DateTime NewTaskDueDate { get; set; } = DateTime.Now.AddDays(1);
        public string NewTaskPriority { get; set; } = "Medium";

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
            }
        }

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

        public MainViewModel(FileService fileService, string username)
        {
            _fileService = fileService;
            _currentUser = username;

            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ToggleTaskStatusCommand = new RelayCommand(ToggleTaskStatus);
            SearchCommand = new RelayCommand(FilterTasks);
            ToggleViewCommand = new RelayCommand(ToggleView);

            LoadTasks();
            LoadCommandHistory();
        }

        private void LoadTasks()
        {
            Tasks.Clear();
            var tasks = _fileService.GetUserTasks(_currentUser);
            foreach (var task in tasks.Where(t => !t.IsCompleted))
            {
                Tasks.Add(task);
            }
        }

        private void LoadCommandHistory()
        {
            CommandHistory.Clear();
            var commands = _fileService.GetLastCommands(_currentUser);
            foreach (var cmd in commands)
            {
                CommandHistory.Add(cmd.Split('|')[2]); // Just show the command part
            }
        }

        private void AddTask(object parameter)
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

            NewTaskDescription = string.Empty;
            OnPropertyChanged(nameof(NewTaskDescription));
        }

        private void DeleteTask(object parameter)
        {
            if (parameter is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
                _fileService.LogCommand(_currentUser, $"Deleted task: {task.Description}");
                LoadCommandHistory();
            }
        }

        private void ToggleTaskStatus(object parameter)
        {
            if (parameter is TaskItem task)
            {
                task.IsCompleted = !task.IsCompleted;
                SaveTasks();
                _fileService.LogCommand(_currentUser, $"Marked task as {(task.IsCompleted ? "completed" : "pending")}: {task.Description}");
                FilterTasks();
                LoadCommandHistory();
            }
        }

        private void FilterTasks(object parameter = null)
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

        private void ToggleView(object parameter)
        {
            ShowCompletedTasks = !ShowCompletedTasks;
        }

        private void SaveTasks()
        {
            _fileService.SaveUserTasks(_currentUser, Tasks.ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}