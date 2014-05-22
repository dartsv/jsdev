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
    public partial class EmployeeUpdate
    {
        partial void ImportFromExcel_Execute()
        {
         
        //          'Use the Office Integration Pack Extension to do a variety of things with MS Office
        //          'Download here: http://visualstudiogallery.msdn.microsoft.com/35c4cf2a-5148-4716-afcf-0ccf8899cabf 
        //          'Dim canAccessCOM As Boolean = System.Runtime.InteropServices.Automation.AutomationFactory.IsAvailable
       
            OfficeIntegration.Excel.Import(this.Employees);

        }
    }
}
