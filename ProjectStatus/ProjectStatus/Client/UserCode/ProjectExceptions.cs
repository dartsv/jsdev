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
    public partial class ProjectExceptions
    {
       
        partial void gridAddAndEditNew_Execute()
        {
            Project ProjExcept = new Project();
            ProjExcept.Client = this.DataWorkspace.ApplicationData.Clients.Where(p => p.IsInternal == true || p.Name == "Junction Solutions").FirstOrDefault();
            ProjExcept.Chargeable = false;
            ProjExcept.BookedEndDate = DateTime.MaxValue;
            ProjExcept.ForecastEndDate = DateTime.MaxValue;
            ProjExcept.ForecastException = true;
            ProjExcept.ProjectStatus = 1;
            ProjExcept.Inactive = false;
            ProjExcept.Phase = "Imp";
        }
    }
}
