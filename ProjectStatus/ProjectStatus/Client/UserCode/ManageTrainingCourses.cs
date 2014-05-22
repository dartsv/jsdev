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
    public partial class ManageTrainingCourses
    {
        partial void ImportFromExcel_Execute()
        {
            //  Use the Office Integration Pack Extension to do a variety of things with MS Office
            //  Download here: http://visualstudiogallery.msdn.microsoft.com/35c4cf2a-5148-4716-afcf-0ccf8899cabf 
            OfficeIntegration.Excel.Import(this.TrainingCoursesSet1);
        }

        partial void InactiveCoursesButton_CanExecute(ref bool result)
        {
            // Write your code here.
            if (this.IsInactiveObject == true)
            {
                this.IsInactiveObject = false;
                this.FindControl("InactiveCoursesButton").DisplayName = "Show Inactive";
            }
            else 
            {
                this.IsInactiveObject = true;
                this.FindControl("InactiveCoursesButton").DisplayName = "Hide Inactive";
            }

        }

        partial void InactiveCoursesButton_Execute()
        {
            // Write your code here.

        }

        partial void ManageTrainingCourses_Activated()
        {
            // Write your code here.
            this.IsInactiveObject = true;

        }

      
    }
}
