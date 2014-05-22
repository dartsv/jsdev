using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security;
namespace LightSwitchApplication
{
    public partial class Issue
    {
        partial void LastUpdateInLocalTime_Compute(ref DateTime result)
        {
            IUser user = this.Application.User;
            Employee employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == user.Name).Include("Country").FirstOrDefault();
            if (employee == null)
            {
                result = this.LastUpdate;
                return;
            }
            decimal? offset = employee.Country.TimeZoneOffset;
            if (offset != null)
            {
                result = this.LastUpdate.AddHours((double)offset);
            }
            else
            {
                result = this.LastUpdate;
            }
        }

        partial void Issue_Created()
        {
            IUser user = this.Application.User;
            this.ReportedAt = DateTime.Now;
            this.AssignedTo = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == user.Name).FirstOrDefault();            
            this.Status = "Not Started";
            this.Priority = "Normal";
        }

        partial void BillableHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTracks.Where(tt => tt.Billable == "Billable").Sum(tt => tt.Hours), 1);

           
        }

        partial void NonBillableHours_Compute(ref decimal result)
        {
            result = Math.Round (this.TimeTracks.Where(tt => tt.Billable == "Non-Billable").Sum(tt => tt.Hours), 1);

            
        }
    }
}
