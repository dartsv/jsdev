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
    public partial class UnassignedEmployeeReport
    {
        const string CON = "Consulting";

        partial void FilterConsultingBtn_Execute()
        {
            if (ShowConsulting != CON)
            {
                ShowConsulting = CON;
                this.FindControl("FilterConsultingBtn").DisplayName = "Show All";
                this.FindControl("UnassignedEmployees").DisplayName = "Unassigned Employees - Consulting Only";
            }
            else
            {
                ShowConsulting = null;
                var ctl = this.FindControl("FilterConsultingBtn");
                ctl.DisplayName = "Show Consulting Only";
                this.FindControl("UnassignedEmployees").DisplayName = "Unassigned Employees - All Departments";
            }
        }

        partial void UnassignedEmployeeReport_Activated()
        {
            // Show consulting only by default
            ShowConsulting = CON;
           
        }

        partial void UnassignedEmployeeReport_Saving(ref bool handled)
        {
            this.DataWorkspace.ApplicationData.Details.DiscardChanges();
            this.Close(false);

        }

        partial void UnassignedEmployeeReport_Closing(ref bool cancel)
        {
            // Write your code here.
            this.DataWorkspace.ApplicationData.Details.DiscardChanges();
            this.Close(false);
        }
     
    }
}
