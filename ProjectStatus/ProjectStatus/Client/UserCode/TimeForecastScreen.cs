using System;
using System.Windows.Data;

using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Media;
using System.Windows.Controls;



namespace LightSwitchApplication
{
    public partial class TimeForecastScreen
    {
        public int UserID = 0;

        partial void TimeForecastScreen_Saved()
        {
            this.Refresh();
        }

        partial void TimeForecasts_Loaded(bool succeeded)
        {
            //  changes color of status font
            // "Chargeable" is the name of the control, in this case the field on the datagrid.
            IContentItemProxy StatusControl = this.FindControl("Chargeable");
            //StatusControl.SetBinding(TextBox.BackgroundProperty, "Value", new ChangeFontColorStatus(), BindingMode.OneWay);
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeFontColorStatus(), BindingMode.OneWay);
        }

        
    }





    public class ChangeFontColorStatus : IValueConverter
    {
        //  --------------------------------------------------------------------------------------------------------

        //  class to change colors

        //  --------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string isValue = value.ToString().Trim();
                if (isValue == "Non Chargeable")
                {
                    return new SolidColorBrush(Colors.Orange);
                }
                if (isValue == "True")
                {
                    return new SolidColorBrush(Colors.Magenta);
                }
                return new SolidColorBrush(Colors.Black);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
