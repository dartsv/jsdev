using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security;

namespace LightSwitchApplication
{
    public partial class ProjectTask
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

        partial void TrackedDevelopmentHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTrackings.Where(tt => tt.Type == "Development").Sum(tt => tt.Hours), 1);
        }

        partial void TrackedAnalysisHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTrackings.Where(tt => tt.Type == "Analysis").Sum(tt => tt.Hours), 1);
        }

        partial void BillableDevHours_Compute(ref decimal result)
        {
            //result = this.TimeTrackings.Where(tt => tt.Chargeable == "Billable" && tt.IsForIssue == false).Sum(tt => tt.Hours);
            result = Math.Round((this.ReqTotalBillableHours + this.IssuesTotalBillableHours), 1);
        }

        partial void ReqTotalEstimate_Compute(ref decimal result)
        {
            result = 0;
            if (this.Requirements != null)
                result = Math.Round((decimal)this.Requirements.Sum(r => r.EstimateHours), 1);

        }

        partial void ReqTotalBillableHours_Compute(ref decimal result)
        {
            result = 0;
            if (this.Requirements != null)
                result = Math.Round((decimal)this.Requirements.Sum(r => r.BillableHours), 1);

        }

        partial void IssuesTotalEstimate_Compute(ref decimal result)
        {
            result = 0;
            if (this.Issues != null)
                result = Math.Round((decimal)this.Issues.Sum(i => i.EstimateHours), 1);
        }

        partial void IssuesTotalBillableHours_Compute(ref decimal result)
        {
            result = 0;
            if (this.Issues != null)
                result = Math.Round((decimal)this.Issues.Sum(i => i.BillableHours), 1);

        }

        partial void ProjectTask_Created()
        {
            this.Priority = "3-Medium";
            this.Status = "Not Started";
            this.DateAssigned = DateTime.Today;

        }
    }
}
