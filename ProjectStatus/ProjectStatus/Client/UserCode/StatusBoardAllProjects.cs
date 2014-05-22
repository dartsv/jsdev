using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Data;
using System.Windows.Controls;
using Microsoft.LightSwitch.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace LightSwitchApplication
{
    public partial class StatusBoardAllProjects
    {
        private bool WasCanceled = true;

        partial void EmployeeAssignmentAddAndEditNew2_Execute()
        {   
            EmployeeAssignment newAssignment = new EmployeeAssignment();
            newAssignment.StartDate = Projects.SelectedItem.StartDate;
            newAssignment.EndDate = Projects.SelectedItem.BookedEndDate;
            newAssignment.Projects = Projects.SelectedItem;
            newAssignment.Percent = 100;
            newAssignment.Chargeable = Projects.SelectedItem.Chargeable;
            //newAssignment.EmployeeLevel = 13;
           // newAssignment.EmployeeLevel = this.DataWorkspace.ApplicationData.EmployeeLevels_SingleOrDefault(13);
        }

        partial void ExcludeJS_Execute()
        {
            this.FindControl("ExcludeJS").DisplayName = (this.ExcludedClient) ? "Show JS" : "Exclude JS";
            this.ExcludedClient = !this.ExcludedClient;

        }

        partial void ShowInactiveBtn_Execute()
        {
            if (this.ShowInactive)
            {
                this.ShowInactive = false;
                this.ProjectStatusFilter = -100;
                this.FindControl("ShowInactiveBtn").DisplayName = "Show Inactive";
            }
            else
            {
                this.ShowInactive = true;
                this.ProjectStatusFilter = 100;
                this.FindControl("ShowInactiveBtn").DisplayName = "Hide Inactive";
            }
        }


        partial void StaffingRequirementsAddAndEditNew_Execute()
        {
            StaffingRequirement newSR = new StaffingRequirement();
            newSR.DateRequired = DateTime.Today;
            newSR.EndDate = Projects.SelectedItem.BookedEndDate;
            newSR.FTEs = 1;
            newSR.Project = Projects.SelectedItem;
        }

        partial void StatusBoardAllProjects_Activated()
        {
            // Load the Excluded Client Parameter to avoid errors when saving (Parameter is required to save)
            ExcludedClient = true;
            this.ProjectStatusFilter = -100;
            this.FindControl("NewEditProject").ControlAvailable += (s, a) =>
                {
                    ChildWindow cw = a.Control as ChildWindow;
                    cw.Closing += (se, ar) =>
                    {
                        if (WasCanceled)
                            foreach (Project project in this.DataWorkspace.ApplicationData.Details.
                                    GetChanges().OfType<Project>())
                                project.Details.DiscardChanges();
                    };
                };
        }

        partial void Projects_Loaded(bool succeeded)
        {
            //  changes color of status font
            // "ProjectStatus" is the name of the control, in this case the field on the datagrid.
            IContentItemProxy StatusControl = this.FindControl("ProjectStatus");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeProjStatusFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("StaffingStatus");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeProjStatusFontColorStatus(), BindingMode.OneWay);

        }

        partial void ShowJS_Execute()
        {
            // Escriba el código aquí.

        }

        partial void gridAddAndEditNew_CanExecute(ref bool result)
        {
            // Write your code here.
        }

        partial void gridAddAndEditNew_Execute()
        {
            //Added so that the modal window open only after the new empty project has been created
            Task<Project> createProject = Task<Project>.Factory.StartNew(() => {
                Project p = this.Projects.AddNew();
                this.Projects.SelectedItem = p;
                return p; 
            });
            createProject.Wait();
            
            if (this.Projects.SelectedItem != null)
            {
                this.FindControl("NewEditProject").DisplayName = "Add Project";
                this.OpenModalWindow("NewEditProject");
            }
        }

        partial void gridEditSelected_CanExecute(ref bool result)
        {
            // Write your code here
        }

        partial void gridEditSelected_Execute()
        {
            if (this.Projects.SelectedItem != null)
            {
                this.FindControl("NewEditProject").DisplayName = "Edit Project";
                this.OpenModalWindow("NewEditProject");
            }
        }

        partial void DiscardProjectChanges_Execute()
        {
            this.WasCanceled = true;
            this.CloseModalWindow("NewEditProject");
            foreach (Project project in this.DataWorkspace.ApplicationData.Details.
                                GetChanges().OfType<Project>())
                project.Details.DiscardChanges();
        }

        partial void SaveProjectChanges_Execute()
        {
            this.WasCanceled = false;
            this.CloseModalWindow("NewEditProject");
            this.Save();
        }

        partial void ViewSkills_Execute()
        {
            if (this.EmployeeAssignment.SelectedItem != null)
            {
                this.Application.ShowManageMySkills(this.EmployeeAssignment.SelectedItem.Employee.UserID);
            }
        }

        partial void DodViewSkills_Execute()
        {
            if (this.DodEmployeeAssignments.SelectedItem != null)
            {
                this.Application.ShowManageMySkills(this.DodEmployeeAssignments.SelectedItem.Employee.UserID);
            }
        }

        partial void Employees_SelectionChanged()
        {
            var selectedEmployee = this.Employees.SelectedItem.Id;
           // this.FindControlInCollection("Id", 
            //this.EmployeeAssignment.SelectedItem.EmployeeLevel.Id= this.Employees.SelectedItem.Id;
        }

        partial void StatusBoardAllProjects_Created()
        {
            // Write your code here.

        }

     

                 
    }
}
