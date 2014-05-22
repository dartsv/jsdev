using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Data;
using System.Windows.Controls;


namespace LightSwitchApplication
{
    public partial class StatusBoardReport
    {
        partial void StatusSummaries_Loaded(bool succeeded)
        {
            //  changes color of status font
            // "ProjectStatus" is the name of the control, in this case the field on the datagrid.
            IContentItemProxy StatusControl = this.FindControl("ProjectStatus");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeProjStatusFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("StaffingStatus");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeProjStatusFontColorStatus(), BindingMode.OneWay);

        }
    }
}
