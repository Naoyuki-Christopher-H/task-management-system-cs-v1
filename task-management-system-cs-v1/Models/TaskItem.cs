using System;

namespace task_management_system_cs_v1.Models
{
    /// <summary>
    /// Represents a task item with all required properties including title, description,
    /// due date, priority, and completion status.
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// Title of the task (required field)
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the task
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Due date for task completion
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Priority level (LOW, MEDIUM, HIGH)
        /// </summary>
        public string Priority { get; set; } = "MEDIUM";

        /// <summary>
        /// Indicates whether the task is completed
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Date when the task was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Date when the task was marked as completed (nullable)
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Initializes a new task with creation timestamp
        /// </summary>
        public TaskItem()
        {
            CreatedDate = DateTime.Now;
        }

        /// <summary>
        /// Serializes the task to a string for file storage
        /// </summary>
        public override string ToString()
        {
            return $"{Title}|{Description}|{DueDate:yyyy-MM-dd}|{Priority}|{IsCompleted}|{CreatedDate:yyyy-MM-dd HH:mm:ss}|{(CompletedDate.HasValue ? CompletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "")}";
        }

        /// <summary>
        /// Creates a TaskItem from a serialized string
        /// </summary>
        public static TaskItem FromString(string data)
        {
            var parts = data.Split('|');
            return new TaskItem
            {
                Title = parts[0],
                Description = parts[1],
                DueDate = DateTime.Parse(parts[2]),
                Priority = parts[3],
                IsCompleted = bool.Parse(parts[4]),
                CreatedDate = DateTime.Parse(parts[5]),
                CompletedDate = string.IsNullOrEmpty(parts[6]) ? null : DateTime.Parse(parts[6])
            };
        }
    }
}