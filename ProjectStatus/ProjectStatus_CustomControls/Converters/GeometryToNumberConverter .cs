using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CustomControls.Converters
{
    public class GeometryToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var g = value as Geometry;
            var type = parameter as string;
            if (type == "Left")
                return (g.Bounds.Left + g.Bounds.Right) / 2.0;
            else if (type == "Top")
                return (g.Bounds.Top + g.Bounds.Bottom) / 2.0;
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
