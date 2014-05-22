using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;

namespace LightSwitchApplication
{
    public partial class TimeForecastDetail
    {
        partial void TimeForecast_Loaded(bool succeeded)
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.TimeForecast);
        }

        partial void TimeForecast_Changed()
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.TimeForecast);
        }

        partial void TimeForecastDetail_Saved()
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.TimeForecast);
        }
    }
}