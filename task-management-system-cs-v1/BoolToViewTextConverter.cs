using System;
using System.Globalization;
using System.Windows.Data;

namespace task_management_system_cs_v1
{
    /// <summary>
    /// Converts between boolean values and view text for the task view toggle button
    /// Implements IValueConverter for XAML binding support
    /// </summary>
    public class BoolToViewTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to display text for the view toggle button
        /// </summary>
        /// <param name="value">Boolean value indicating if completed tasks are shown</param>
        /// <param name="targetType">The target type (not used)</param>
        /// <param name="parameter">Optional parameter (not used)</param>
        /// <param name="culture">Culture info (not used)</param>
        /// <returns>"Show Pending Tasks" when true, "Show Completed Tasks" when false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool showCompleted && showCompleted)
            {
                return "Show Pending Tasks";
            }
            return "Show Completed Tasks";
        }

        /// <summary>
        /// Not implemented as conversion is one-way only
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported");
        }
    }
}