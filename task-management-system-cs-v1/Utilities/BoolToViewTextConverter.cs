using System;
using System.Globalization;
using System.Windows.Data;

namespace task_management_system_cs_v1.Utilities
{
    public class BoolToViewTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Show Pending Tasks" : "Show Completed Tasks";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}