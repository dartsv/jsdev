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
    public class FeatureStatusToColorConverter : IValueConverter
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
                    case "Cancelled":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0xFF, 0xFF, 0xFF));
                    case "Reassigned":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0xFF, 0xFF, 0xFF));
                    case "Not Started":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0xFF, 0xFF, 0xFF));
                    case "Analysis":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0xA4, 0xB5, 0xCA));
                    case "Development":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0x67, 0x93, 0xCC));
                    case "Unit Testing/QA":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0x45, 0x80, 0xCC));
                    case "Testing":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0x14, 0x51, 0x9E));
                    case "Issue Resolution":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0x29, 0x4B, 0x77));
                    case "Client Review":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0x07, 0x31, 0x67));
                    case "Completed":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0x04, 0x1B, 0x39));
                    case "On-Hold":
                        return new SolidColorBrush(Color.FromArgb(0xE0, 0xE7, 0xA6, 0x14));

                    default:
                        return new SolidColorBrush(Colors.Transparent);
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
