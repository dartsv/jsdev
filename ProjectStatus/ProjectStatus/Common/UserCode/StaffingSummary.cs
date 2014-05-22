using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class StaffingSummary
    {
        partial void AssignedFTEFormatted_Compute(ref decimal result)
        {
            if (AssignedFTE != null)
            {
                result = (decimal)AssignedFTE / 100;
            }
        }
    }
}
