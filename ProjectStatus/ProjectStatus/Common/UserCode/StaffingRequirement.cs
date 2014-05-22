using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class StaffingRequirement
    {
        partial void Client_Compute(ref string result)
        {
            // Set result to the desired field value
            result = Project.Client;
        }

        partial void DateRequired_Validate(EntityValidationResultsBuilder results)
        {
            if (DateRequired > Project.ForecastEndDate)
            {
                results.AddPropertyError("Start Date cannot be after the Forecast Project End Date");
            }
            else if (DateRequired < Project.StartDate | DateRequired < DateTime.Today)
            {
                results.AddPropertyError("Start Date cannot be earlier than today or the project start date.");
            }
        }

        partial void ProjectName_Compute(ref string result)
        {
            result = Project.ProjectName;
        }
    }
}
