using System;

namespace task_management_system_cs_v1.Models
{
    public class TaskItem
    {
        // Make properties nullable or initialize with default values
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = "Medium"; // Default priority
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }

        public TaskItem()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Description}|{DueDate:yyyy-MM-dd}|{Priority}|{IsCompleted}|{CreatedDate:yyyy-MM-dd HH:mm:ss}";
        }

        public static TaskItem FromString(string data)
        {
            var parts = data.Split('|');
            return new TaskItem
            {
                Description = parts[0],
                DueDate = DateTime.Parse(parts[1]),
                Priority = parts[2],
                IsCompleted = bool.Parse(parts[3]),
                CreatedDate = DateTime.Parse(parts[4])
            };
        }
    }
}