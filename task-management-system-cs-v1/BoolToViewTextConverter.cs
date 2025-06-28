using System;
using System.Globalization;
using System.Windows.Data;

namespace task_management_system_cs_v1
{
    /// <summary>
    /// Converts a boolean value to display text for the view toggle button
    /// </summary>
    public class BoolToViewTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to display text
        /// </summary>
        /// <param name="value">The boolean value to convert</param>
        /// <param name="targetType">The target type (unused)</param>
        /// <param name="parameter">Optional parameter (unused)</param>
        /// <param name="culture">Culture info (unused)</param>
        /// <returns>"Show Pending Tasks" when true, "Show Completed Tasks" when false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool showCompleted)
            {
                return showCompleted ? "Show Pending Tasks" : "Show Completed Tasks";
            }
            return "Show Completed Tasks";
        }

        /// <summary>
        /// Not implemented - ConvertBack is not needed for one-way binding
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}