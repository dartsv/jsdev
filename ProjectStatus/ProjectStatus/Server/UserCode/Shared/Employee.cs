using System;
namespace LightSwitchApplication
{
    public partial class Employee
    {
        partial void FullName_Compute(ref string result)
        {
            // Establece el resultado en el valor del campo deseado
            result = this.FirstName + " " + this.LastName;
        }

        partial void Employee_Created()
        {
            this.Department="legacy";
            this.Title = "legacy";
            this.Supervisor = "legacy";
            this.Supervisor = "legay";
        }

        partial void SupervisorFullName_Compute(ref string result)
        {
            //gg added // Set result to the desired field value
            result = this.Supervsr.FirstName + " " + this.Supervsr.LastName;

        }

        partial void DefaultLevelName_Compute(ref string result)
        {
             //gg added // Set result to the desired field value
            result = this.EmployeeTitle.EmployeeLevel.Name;

        }

    }
}
