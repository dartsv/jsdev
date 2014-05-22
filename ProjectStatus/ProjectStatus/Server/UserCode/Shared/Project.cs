using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class Project
    {

        partial void CurrentStaffing_Compute(ref decimal result)
        // Originally written using Linq (see the code below)
        // Very slow to load
        // Refactored into a lambda expression to speed it up - it did not make a difference
        {
            DateTime tdy = DateTime.Today;
            var query = EmployeeAssignment
                .Where(ea => ea.StartDate <= tdy && ea.EndDate >= tdy);
            result = (decimal)query.Sum(t => t.Percent) / 100;
        }

        partial void NeededStaffing_Compute(ref decimal result)
        {
            result = (decimal)this.StaffingRequirements.Sum(t => t.FTEs);
        }

        partial void StartDate_Validate(EntityValidationResultsBuilder results)
        {
            // results.AddPropertyError("<Error-Message>");
            if (StartDate > BookedEndDate)
            {
                results.AddPropertyError("Start Date cannot be after the Projected End Date");
            }
        }

        partial void BookedEndDate_Validate(EntityValidationResultsBuilder results)
        {
            if (StartDate > BookedEndDate)
            {
                results.AddPropertyError("Projected End Date can not be before the Start Date");
            }
            
        }

        partial void ForecastEndDate_Validate(EntityValidationResultsBuilder results)
        {
            if (BookedEndDate > ForecastEndDate)
            {
                ForecastEndDate = BookedEndDate;
            }

        }

        partial void SharepointURL_Validate(EntityValidationResultsBuilder results)
        {
            /* TODO: Review this validation
            if (this.IsDOD) 
            {
                if (string.IsNullOrEmpty(this.SharepointURL)) 
                {
                    results.AddPropertyError("A DOD project must have a Sharepoint URL");
                }
            }
             * */
        }

        
    }
}
