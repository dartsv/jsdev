using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class TDDExtension
    {
        partial void TDDExtension_Created()
        {
            string userId = this.Application.User.Name;
            
            this.Employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == userId).FirstOrDefault();
        }
    }
}
