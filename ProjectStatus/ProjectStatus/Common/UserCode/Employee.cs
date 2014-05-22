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
    }
}
