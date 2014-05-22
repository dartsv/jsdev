using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class CountryHoliday
    {
        partial void StartDate_Validate(EntityValidationResultsBuilder results)
        {
            if(StartDate > EndDate)
                results.AddPropertyError("The start date must be less or equal to the end date");

        }

        partial void EndDate_Validate(EntityValidationResultsBuilder results)
        {
            if(EndDate < StartDate)
                results.AddPropertyError("The end date must be greater or equal to the start date");

        }
    }
}
