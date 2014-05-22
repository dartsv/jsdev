using System;
using System.Linq;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.VisualStudio.ExtensibilityHosting;
using Microsoft.LightSwitch.Sdk.Proxy;


namespace LightSwitchApplication
{
    public partial class EmployeeAssignReport

    {
        const string CON = "Consulting";

        

        partial void EmployeeAssignReport_Closing(ref bool cancel)
        {
            // Write your code here.
            this.DataWorkspace.ApplicationData.Details.DiscardChanges();
            this.Close(false);
        }

        partial void EmployeeAssignReport_Saving(ref bool handled)
        {
            this.DataWorkspace.ApplicationData.Details.DiscardChanges();
            this.Close(false);
        }

        partial void FilterConsultingBtn_Execute()
        {
            // ShowConsulting is an optional parameter
            //CON is a string variable that holds the value "Consulting"
            if (ShowConsulting != CON)
            {
                ShowConsulting = CON;
                this.FindControl("FilterConsultingBtn").DisplayName = "Show All";
                this.FindControl("EmployeeAssignmentReports").DisplayName = "Employee Assignments - Consulting Only";
               
                //todo code to eliminate sort settings
                //***************************************************************************************************
                //var p = VsExportProviderService.GetExportedValue<IServiceProxy>();
                //string name = "EmployeeAssignReport.EmployeeAssignmentsByName.SortSettings";
                //p.UserSettingsService.ResetSetting(name);
                //this.Refresh();
                //p.UserSettingsService.ResetSetting(name);
                //***************************************************************************************************

            }
            else
            {
                ShowConsulting = null;
                var ctl = this.FindControl("FilterConsultingBtn");
                ctl.DisplayName = "Show Consulting Only";
                this.FindControl("EmployeeAssignmentReports").DisplayName = "Employee Assignments - All Departments";
            }
        }

        partial void EmployeeAssignReport_Activated()
        {
            // Show consulting only by default
            ShowConsulting = CON;
           
        }
    }
}
