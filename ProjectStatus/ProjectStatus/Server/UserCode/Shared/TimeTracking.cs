using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class TimeTracking
    {
        partial void Hours_Compute(ref decimal result)
        {
            DateTime start = this.StartTime;
            DateTime end = this.EndTime;

            result = ((decimal)Math.Round((end - start).TotalHours * 2) / (decimal) 2);
        }

        partial void TimeTracking_Created()
        {
            string userId = this.Application.User.Name;
            /*DateTime lastEndTime = DateTime.Now;
            var previousEntries = this.DataWorkspace.ApplicationData.TimeTrackings.OrderByDescending(tt => tt.WorkDate).ThenByDescending(tt => tt.EndTime);
            if(previousEntries != null)
            {
                var lastEntry = previousEntries.FirstOrDefault();
                if (lastEntry != null) lastEndTime = lastEntry.EndTime;
            }*/
            
            this.Employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == userId).FirstOrDefault();
            this.Type = "Development";
            this.WorkDate = DateTime.Today;
            this.StartTime = DateTime.Now;
            this.EndTime = DateTime.Now;//(lastEndTime > DateTime.Now) ? lastEndTime : DateTime.Now;
            this.Chargeable = "Billable";
        }

        partial void EndTime_Validate(EntityValidationResultsBuilder results)
        {
            if (this.StartTime > this.EndTime)
            {
                results.AddPropertyError("The End Time must be greater than the Start Time");
            }
        }

        partial void WorkDate_Changed()
        {
            this.StartTime = new DateTime(this.WorkDate.Year, this.WorkDate.Month, this.WorkDate.Day, this.StartTime.Hour, this.StartTime.Minute, this.StartTime.Second);
            this.EndTime = new DateTime(this.WorkDate.Year, this.WorkDate.Month, this.WorkDate.Day, this.EndTime.Hour, this.EndTime.Minute, this.EndTime.Second);
        }
    }
}
