using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using task_management_system_cs_v1.Models;

namespace task_management_system_cs_v1.Services
{
    /// <summary>
    /// Handles all file operations for the task management system including:
    /// - User authentication data storage
    /// - Task data persistence
    /// - Command history logging
    /// </summary>
    public class FileService
    {
        // Constants for file names and paths
        private const string UsersFile = "users.txt";
        private const string CommandLogFile = "command_log.txt";
        private static readonly string DataDirectory = Path.Combine(Environment.CurrentDirectory, "UserData");

        /// <summary>
        /// Initializes the file service and ensures required directories exist
        /// </summary>
        public FileService()
        {
            // Create data directory if it doesn't exist
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
        }

        /// <summary>
        /// Registers a new user with credentials
        /// </summary>
        /// <param name="user">User object containing username and password</param>
        /// <returns>True if registration succeeded, false if username exists</returns>
        public bool RegisterUser(User user)
        {
            // Validate input
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return false;
            }

            // Check if user already exists
            var users = GetAllUsers();
            if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // Append new user to file
            File.AppendAllText(UsersFile, $"{user.Username}|{user.Password}\n");
            return true;
        }

        /// <summary>
        /// Validates user credentials against stored data
        /// </summary>
        /// <param name="user">User object to validate</param>
        /// <returns>True if credentials are valid</returns>
        public bool ValidateUser(User user)
        {
            if (user == null) return false;

            var users = GetAllUsers();
            return users.Any(u =>
                u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == user.Password);
        }

        /// <summary>
        /// Gets all registered users from storage
        /// </summary>
        /// <returns>List of User objects</returns>
        private List<User> GetAllUsers()
        {
            if (!File.Exists(UsersFile))
            {
                return new List<User>();
            }

            try
            {
                return File.ReadAllLines(UsersFile)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line =>
                    {
                        var parts = line.Split('|');
                        return new User
                        {
                            Username = parts[0],
                            Password = parts[1]
                        };
                    })
                    .ToList();
            }
            catch
            {
                return new List<User>();
            }
        }

        /// <summary>
        /// Gets all tasks for a specific user
        /// </summary>
        /// <param name="username">Username to get tasks for</param>
        /// <returns>List of TaskItem objects</returns>
        public List<TaskItem> GetUserTasks(string username)
        {
            var filePath = GetUserFilePath(username);
            if (!File.Exists(filePath))
            {
                return new List<TaskItem>();
            }

            try
            {
                return File.ReadAllLines(filePath)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(TaskItem.FromString)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogCommand("system", $"Error loading tasks: {ex.Message}");
                return new List<TaskItem>();
            }
        }

        /// <summary>
        /// Saves tasks for a specific user
        /// </summary>
        /// <param name="username">User to save tasks for</param>
        /// <param name="tasks">List of TaskItem objects to save</param>
        public void SaveUserTasks(string username, List<TaskItem> tasks)
        {
            var filePath = GetUserFilePath(username);
            try
            {
                File.WriteAllLines(filePath, tasks.Select(t => t.ToString()));
            }
            catch (Exception ex)
            {
                LogCommand("system", $"Error saving tasks: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets command history for the specified user from the past 12 hours
        /// </summary>
        /// <param name="username">Username to filter commands</param>
        /// <returns>List of command strings with timestamps</returns>
        public List<string> GetLastCommands(string username)
        {
            if (!File.Exists(CommandLogFile))
            {
                return new List<string>();
            }

            var twelveHoursAgo = DateTime.Now.AddHours(-12);

            try
            {
                return File.ReadAllLines(CommandLogFile)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line => line.Contains($"|{username}|"))
                    .Where(line =>
                    {
                        var timestamp = DateTime.Parse(line.Split('|')[0]);
                        return timestamp >= twelveHoursAgo;
                    })
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Logs a command with timestamp for history tracking
        /// </summary>
        /// <param name="username">User who executed the command</param>
        /// <param name="command">Command description to log</param>
        public void LogCommand(string username, string command)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"{timestamp}|{username}|{command}";
                File.AppendAllText(CommandLogFile, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Fallback logging if primary logging fails
                File.AppendAllText("error_log.txt", $"{DateTime.Now}: Failed to log command - {ex.Message}\n");
            }
        }

        /// <summary>
        /// Generates the full file path for a user's task data file
        /// </summary>
        /// <param name="username">Username for the file path</param>
        /// <returns>Full path to the user's data file</returns>
        private string GetUserFilePath(string username)
        {
            // Sanitize username to prevent path injection
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeUsername = string.Join("_", username.Split(invalidChars));
            return Path.Combine(DataDirectory, $"{safeUsername}.txt");
        }
    }
}