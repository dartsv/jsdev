using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class ProjectRate
    {
        partial void ProjectRate_Created()
        {
            //GG Added, to create a registry due the EmployeeTitle field cant be deleted
            this.EmployeeTitle = this.DataWorkspace.ApplicationData.EmployeeTitles_SingleOrDefault(15); // 15= Other
           

        }
    }
}
