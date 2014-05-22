using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security;
namespace LightSwitchApplication
{
    public partial class Requirement
    {
        partial void Requirement_Created()
        {
            this.LastUpdate = DateTime.Now;
   //var currentEmployee= this.DataWorkspace.ApplicationData.Employees.
           // this.AssignedTo = this.DataWorkspace.ApplicationData.Employees.Where(e => e.Id == DataWorkspace.ApplicationData.Employees.Id).FirstOrDefault();
            this.AssignedTo = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == GlobalStrings.LoggedOnUser()).FirstOrDefault();
            this.Status = "Not Started";
            //GG added
            this.Priority = "Normal";
        }

        partial void BillableHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTracks.Where(tt => tt.Billable == "Billable").Sum(tt => tt.Hours),1);
        }

        partial void NonBillableHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTracks.Where(tt => tt.Billable == "Non-Billable").Sum(tt => tt.Hours), 1);
        }

        partial void BillableRemainingHours_Compute(ref decimal result)
        {
            decimal estimate = (this.EstimateHours != null) ? (decimal)this.EstimateHours : 0;
            decimal remain = estimate - this.BillableHours;
            result = Math.Round((remain >= 0) ? remain : 0 ,1 );
        }

        partial void BillableOverHours_Compute(ref decimal result)
        {
            decimal estimate = (this.EstimateHours != null) ? (decimal)this.EstimateHours : 0;
            decimal over = estimate - this.BillableHours;
            result = Math.Round((over < 0) ? - (over) : 0, 1);
        }

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
    }
}
