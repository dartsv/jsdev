using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class TimeForecast
    {
       
        partial void StartDate_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void EndDate_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Percent_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Chargeable_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Employee_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Project_Changed()
        {
            //if (StartDate == DateTime.Today && EndDate == DateTime.Today)
            if (StartDate == DateTime.MinValue)
            {
                StartDate = DateTime.Today;
                Chargeable = Project.Chargeable;
                LastUpdate = DateTime.Now;
                if (Project.ForecastException)
                {
                    //for forecast exceptions, such as PTO, set the end date = to the start date
                    EndDate = StartDate;
                }
                else
                {
                    //for regular projects, set the end date = to the project booked end date
                    EndDate = Project.BookedEndDate;
                }
            }

        }

        partial void TimeForecast_Created()
        {
            Percent = 100;
            Chargeable = true;
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
        }

        partial void Chargeable_Validate(EntityValidationResultsBuilder results)
        {
            // results.AddPropertyError("<Error-Message>");
            if (Chargeable & Project != null )
            {
                if (Project.Chargeable == false)
                {
                    results.AddPropertyError("Cannot forecast chargeable hours against a non-chargeable project.");
                }
            }

        }

        partial void StartDate_Validate(EntityValidationResultsBuilder results)
        {
            
                if (EndDate < StartDate)
                {
                    results.AddPropertyError("Start date must be less than or equal to the end date.");
                }
                if (Project != null)
                {
                    if (StartDate < Project.StartDate)
                    {
                        results.AddPropertyError("Start Date cannot be before Project Start Date of " + Project.StartDate.Date.ToString());
                    }
                }
               
        }

        partial void EndDate_Validate(EntityValidationResultsBuilder results)
        {
            if (EndDate < StartDate)
            {
                results.AddPropertyError("End date must be greater than or equal to the start date.");
            }
            if (Project != null)
            {
                if (EndDate > Project.BookedEndDate)
                {
                    results.AddPropertyError("End date can not be after the Project Booked End Date of " + Project.BookedEndDate.Date.ToString());
                }
            }
        }

        partial void Percent_Validate(EntityValidationResultsBuilder results)
        {
            if (Percent < 1 | Percent > 100)
            {
                results.AddPropertyError("Percent must be between 1 and 100.");
            }

        }
    }
}
