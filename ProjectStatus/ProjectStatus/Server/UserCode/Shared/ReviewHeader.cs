using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class ReviewHeader
    {
        partial void EmployeeName_Compute(ref string result)
        {
            // Set result to the desired field value
            result = Employee.FirstName + " " + Employee.LastName;
        }

        //partial void Supervisor_Compute(ref string result)
        //{
        //    // Set result to the desired field value
        //    result = Employee.Supervsr.FirstName + " " + Employee.Supervsr.LastName;
        //}

        partial void ReviewType_Validate(EntityValidationResultsBuilder results)
        {
            // results.AddPropertyError("<Error-Message>");
            if (ReviewType == "Self" & Reviewer != Employee)
            {
                results.AddPropertyError("You cannot perform a self review on another individual.");
            }
           // if (ReviewType == "Annual" & Reviewer != Employee.Supervsr)
           // {
           //     results.AddPropertyError("Only the employee's supervisor can preform an annual review.");
          //  }
            if (Reviewer == Employee & ReviewType != "Self")
            {
                results.AddPropertyError("You can only perform a self review.");
            }
        }

        partial void ReviewerName_Compute(ref string result)
        {
            result = Reviewer.FirstName + " " + Reviewer.LastName;
        }

        partial void ReviewHeader_Created()
        {
            this.Supervisor="Legacy";
        }

        partial void ReviewHeaderDept_Compute(ref string result)
        {
            // Set result to the desired field value ---GG added
            result = Employee.DeptEmployee.Name;

        }

        

     

        
    }
}
