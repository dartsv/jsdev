using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security;
namespace LightSwitchApplication
{
    public partial class ProjectIncident
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

        partial void TrackedAnalysisHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTrackings.Where(tt => tt.Type == "Analysis").Sum(tt => tt.Hours), 1);

        }

        partial void TrackedDevHours_Compute(ref decimal result)
        {
            result = Math.Round(this.TimeTrackings.Where(tt => tt.Type == "Development").Sum(tt => tt.Hours), 1);
        }

        partial void TrackedBillableHours_Compute(ref decimal result)
        {
            //result = Math.Round(this.TimeTrackings.Where(tt => tt.Chargeable == "Billable" && tt.IsForIssue == false).Sum(tt => tt.Hours) * 2) / (decimal) 2;
            result = Math.Round(this.ReqTotalBillableHours + this.IssuesTotalBillableHours, 1);
        }

        partial void RemainingHours_Compute(ref decimal result)
        {
            decimal totalEstimate = (this.TotalEstimatedHours == null) ? 0 : (decimal)this.TotalEstimatedHours;
            decimal consumedBillable = (this.TrackedBillableHours == null) ? 0 : (decimal)this.TrackedBillableHours;
            result = Math.Round((consumedBillable >= totalEstimate) ? 0 : totalEstimate - consumedBillable, 1);
        }

        partial void BillableOverHours_Compute(ref decimal result)
        {
            decimal totalEstimate = (this.TotalEstimatedHours == null) ? 0 : (decimal)this.TotalEstimatedHours;
            decimal consumedBillable = (this.TrackedBillableHours == null) ? 0 : (decimal)this.TrackedBillableHours;
            result = Math.Round((consumedBillable >= totalEstimate) ? -(totalEstimate - consumedBillable) : 0, 1);

        }

        partial void ReqTotalEstimate_Compute(ref decimal result)
        {
            result = 0;
            if (this.Requirements != null)
                result = Math.Round((decimal)this.Requirements.Sum(r => r.EstimateHours), 2);

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

        partial void TotalEstimatedHours_Compute(ref decimal? result)
        {
            result = Math.Round(this.ReqTotalEstimate + this.IssuesTotalEstimate, 1);
        }


    }
}
