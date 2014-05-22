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
    public class ProjectStatusToColorConverter : IValueConverter
    {
        //  --------------------------------------------------------------------------------------------------------
        //  class to change colors
        //  --------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string featureStatus = value.ToString();
                switch (featureStatus)
                {
                    case "Inactive":
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF));
                    case "Green":
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xA4, 0xB5, 0xCA));
                    case "Yellow":
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xF6, 0xBC, 0x0C));
                    case "Red":
                        return new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
                   
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(0x40,0xAA, 0x00, 0x00));
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
