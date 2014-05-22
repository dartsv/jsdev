﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class EmployeeAssignment
    {
        partial void FullName_Compute(ref string result)
        {
            // Set result to the desired field value
            if (Employee != null)
            {
                result = Employee.FirstName + " " + Employee.LastName;
            }
        }

        partial void Level_Compute(ref string result)
        {
            // Set result to the desired field value
            if (Employee != null)
            {
                result = Employee.Title;
            }
        }

        partial void EndDate_Validate(EntityValidationResultsBuilder results)
        {
            if ( Projects != null)
            {
                if (EndDate < StartDate)
                {
                    results.AddPropertyError("End date must be greater than or equal to the start date.");
                }
                if (EndDate > Projects.BookedEndDate)
                {
                    results.AddPropertyError("Employee end date can not be after the Project End Date");
                }
            }
        }

        partial void StartDate_Validate(EntityValidationResultsBuilder results)
        {
            if (Projects != null)
            {
                if (StartDate < Projects.StartDate)
                {
                    results.AddPropertyError("Employee Start Date can not be earlier than the Project Start Date");
                }
                if (EndDate < StartDate)
                {
                    results.AddPropertyError("Start date must be less than or equal to the start date.");
                }
            }
        }

        partial void Chargeable_Validate(EntityValidationResultsBuilder results)
        {
            // results.AddPropertyError("<Error-Message>");
            if (Percent != 0) //added if statement
            {
                if ((Projects.Chargeable == false) & (Chargeable == true))
                {
                    results.AddPropertyError("Cannot assign a chargeable resource to a non-chargeable project");
                }
            }
        }

        partial void Client_Compute(ref string result)
        {
            if (Projects != null)
            {
                result = Projects.Client;
            }
        }

        partial void LastName_Compute(ref string result)
        {
            result = Employee.LastName;
        }

        partial void FirstName_Compute(ref string result)
        {
            result = Employee.FirstName;
        }

        partial void Department_Compute(ref string result)
        {
            result = Employee.Department;

        }

        partial void ProjBookedEnd_Compute(ref DateTime result)
        {
            result = Projects.BookedEndDate;

        }

        partial void ProjForecastEnd_Compute(ref DateTime result)
        {
            result = Projects.ForecastEndDate;

        }

        partial void Director_Compute(ref string result)
        {
            // Set result to the desired field value
            if (Projects.Director != null)
            {
                result = Projects.Director.FirstName + " " + Projects.Director.LastName;
            }
            
        }

        partial void Manager_Compute(ref string result)
        {
            // Set result to the desired field value
            if (Projects.Manager != null)
            {
                result = Projects.Manager.FirstName + " " + Projects.Manager.LastName;
            }
            
        }


    }
}