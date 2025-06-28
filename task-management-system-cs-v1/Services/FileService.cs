using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using task_management_system_cs_v1.Models;

namespace task_management_system_cs_v1.Services
{
    public class FileService
    {
        private const string UsersFile = "users.txt";
        private const string CommandLogFile = "command_log.txt";
        private static readonly string DataDirectory = Path.Combine(Environment.CurrentDirectory, "UserData");

        public FileService()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }
        }

        public bool RegisterUser(User user)
        {
            var users = GetAllUsers();
            if (users.Any(u => u.Username == user.Username))
            {
                return false;
            }

            File.AppendAllText(UsersFile, $"{user.Username}|{user.Password}\n");
            return true;
        }

        public bool ValidateUser(User user)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Username == user.Username && u.Password == user.Password);
        }

        private List<User> GetAllUsers()
        {
            if (!File.Exists(UsersFile))
            {
                return new List<User>();
            }

            var lines = File.ReadAllLines(UsersFile);
            return lines.Select(line =>
            {
                var parts = line.Split('|');
                return new User { Username = parts[0], Password = parts[1] };
            }).ToList();
        }

        public List<TaskItem> GetUserTasks(string username)
        {
            var filePath = GetUserFilePath(username);
            if (!File.Exists(filePath))
            {
                return new List<TaskItem>();
            }

            var lines = File.ReadAllLines(filePath);
            return lines.Select(TaskItem.FromString).ToList();
        }

        public void SaveUserTasks(string username, List<TaskItem> tasks)
        {
            var filePath = GetUserFilePath(username);
            File.WriteAllLines(filePath, tasks.Select(t => t.ToString()));
        }

        public void LogCommand(string username, string command)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{username}|{command}";
            File.AppendAllText(CommandLogFile, logEntry + Environment.NewLine);
        }

        public List<string> GetLastCommands(string username, int count = 5)
        {
            if (!File.Exists(CommandLogFile))
            {
                return new List<string>();
            }

            return File.ReadAllLines(CommandLogFile)
                .Where(line => line.Contains($"|{username}|"))
                .Reverse()
                .Take(count)
                .Reverse()
                .ToList();
        }

        private string GetUserFilePath(string username)
        {
            return Path.Combine(DataDirectory, $"{username}.txt");
        }
    }
}