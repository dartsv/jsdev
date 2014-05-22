using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Security;

namespace LightSwitchApplication
{
    public partial class ProjectTaskComment
    {
        partial void PostDateLocalTime_Compute(ref DateTime result)
        {
            IUser user = this.Application.User;
            Employee employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == user.Name).Include("Country").FirstOrDefault();
            decimal? offset = employee.Country.TimeZoneOffset;
            if (offset != null)
            {
                result = this.PostDate.AddHours((double)offset);
            }
            else
            {
                result = this.PostDate;
            }
        }

        partial void ProjectTaskComment_Created()
        {
            IUser user = this.Application.User;
            Employee employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == user.Name).Include("Country").FirstOrDefault();
            decimal? offset = employee.Country.TimeZoneOffset;
            if (offset != null)
            {
                this.PostDate = DateTime.Now.AddHours(-(double)offset);
            }
            else
            {
                this.PostDate = DateTime.Now;
            }
            
            string userId = this.Application.User.Name;
            this.Employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == userId).FirstOrDefault();
        }
    }
}
