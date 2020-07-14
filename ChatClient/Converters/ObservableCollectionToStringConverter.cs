using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Data;

namespace ChatClient.Converters
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ObservableCollectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                return null;

            StringBuilder sb = new StringBuilder();
            foreach(string s in (ObservableCollection<string>)value)
            {
                sb.Append($"{s}\r\n");
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
