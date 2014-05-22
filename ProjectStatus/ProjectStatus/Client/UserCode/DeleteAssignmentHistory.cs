using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows;
using System.Collections.Generic;
using System;
using Microsoft.LightSwitch.Framework.Client;

namespace LightSwitchApplication
{
    public partial class DeleteAssignmentHistory
    {
        /// <summary>
        /// Deletes all the records in the grid
        /// </summary>
        private void DeleteRecords() 
        {
            foreach (var pastAssignments in this.EmployeeAssignments)
            {
                pastAssignments.Delete();
            }
            this.Save();
        }

        /// <summary>
        /// Reloads the query if there are no validation errors
        /// </summary>
        private void ReloadData() 
        {
            if (!this.Details.ValidationResults.HasErrors) this.EmployeeAssignments.Load();
        }

        partial void DeleteAssignmentHistory_InitializeDataWorkspace(List<Microsoft.LightSwitch.IDataService> saveChangesTo)
        {
            //Set the date to yesterday as a default value
            //If there is already a value, then leave that value
            if(this.EndDate != null) this.EndDate = DateTime.Now.AddDays(-1);
        }

        partial void EndDate_Validate(ScreenValidationResultsBuilder results)
        {
            //Validate that the parameter must be a past date
            if (EndDate >= DateTime.Today)
            {
                results.AddPropertyError("The date must be a past date");
            }
        }

        partial void DeleteHistory_Execute()
        {
            string warningMessage = "Are you sure you want to delete {0} record{1}?";
            string noPermissionsMessage = "You don't have the necessary permissions to perform this action";

            int recordCount = this.EmployeeAssignments.Count;
            string multiple = recordCount > 1 ? "s" : "";

            //Validate that the user has permissions to delete
            if (this.EmployeeAssignments.CanDelete)
            {
                MessageBoxResult answer = this.ShowMessageBox(string.Format(warningMessage, recordCount, multiple), "Warning", MessageBoxOption.YesNoCancel);
                if (answer == MessageBoxResult.Yes)
                {
                    this.DeleteRecords();
                }
            }
            else
            {
                this.ShowMessageBox(noPermissionsMessage, "Error", MessageBoxOption.Ok);
            }

        }

        partial void SelectedProject_Changed()
        {
            //The parameter is an empty string if the dropdown is left blank
            ProjectName = (this.SelectedProject != null) ? this.SelectedProject.ProjectName : "";
        }

        partial void DeleteHistory_CanExecute(ref bool result)
        {
            //In case there is any kind of validation error, the button is disabled
            if(this.Details.ValidationResults.HasErrors || this.EmployeeAssignments.Count <= 0) result = false;
        }

        partial void EndDate_Changed()
        {
            this.ReloadData();
        }

        partial void ProjectName_Changed()
        {
            this.ReloadData();
        }

       
    }
}
