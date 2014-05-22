using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class Signature
    {
        partial void EmployeeName_Compute(ref string result)
        {
            result = Employee.FirstName + " " + Employee.LastName;
        }

        partial void ProjectsCovered_Compute(ref string result)
        {
            result = ReviewHeader.ProjectsCovered;
        }

        partial void ReviewerName_Compute(ref string result)
        {
            result = ReviewHeader.Reviewer.FirstName + " " + ReviewHeader.Reviewer.LastName;
        }
    }
}
