using System;
using System.Globalization;
using System.Windows.Data;

namespace task_management_system_cs_v1
{
    /// <summary>
    /// Converts a boolean value to display text for the view toggle button
    /// True: "Show Pending Tasks"
    /// False: "Show Completed Tasks"
    /// </summary>
    public class BoolToViewTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts boolean to display text
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool showCompleted)
            {
                return showCompleted ? "Show Pending Tasks" : "Show Completed Tasks";
            }
            return "Show Completed Tasks";
        }

        /// <summary>
        /// Not implemented as conversion is one-way
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}