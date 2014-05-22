using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Controls;
using Microsoft.LightSwitch.Threading;
namespace LightSwitchApplication
{
    public partial class DODProjectsPortal
    {
        partial void Projects_SelectionChanged()
        {
            //UpdateLinks();
        }
        private void UpdateLinks()
        {
            string selectedProjectName = this.Projects.SelectedItem.ProjectName;
            //string dropboxLinkText = (string)(this.FindControl("DropboxLink").GetProperty("Text"));
            //string onTimeLinkText = (string)(this.FindControl("OnTimeLink").GetProperty("Text"));

            //this.FindControl("DropboxLink").SetProperty("Text", "Dropbox\\DODProjects - JS ES and China\\" + selectedProjectName);
            //this.FindControl("OnTimeLink").SetProperty("Text", "http://ontime/" + selectedProjectName.ToLower().Replace(' ','-'));
        }

        partial void DODProjectsPortal_Created()
        {
            //UpdateLinks();
        }

        partial void OpenSkillProfile_Execute()
        {

        }

        partial void OpenDropboxLink_Execute()
        {
            // Escriba el código aquí.
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = "http://" + Application.DefaultDomain + "/links/dropbox/user/" + GlobalStrings.LoggedOnUser();
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            }
            );

        }

        partial void ViewSkills_Execute()
        {
            if (this.EmployeeAssignment.SelectedItem != null)
            {
                this.Application.ShowManageMySkills(this.EmployeeAssignment.SelectedItem.Employee.UserID);
            }
        }

        partial void DODProjectsPortal_Activated()
        {
            //Refresh the country list to 
            this.DataWorkspace.ApplicationData.Countries.GetQuery().Execute();
            
        }

        partial void gridAddAndEditNew_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void gridAddAndEditNew_Execute()
        {
            this.Projects.AddAndEditNew();
        }

        partial void gridEditSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void gridEditSelected_Execute()
        {
            this.Projects.EditSelected();
        }

        partial void gridDeleteSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void gridDeleteSelected_Execute()
        {
            this.Projects.DeleteSelected();

        }


        #region EmployeeAssignments
        partial void EmployeeAssignmentAddAndEditNew_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void EmployeeAssignmentAddAndEditNew_Execute()
        {
            this.EmployeeAssignment.AddAndEditNew();

        }
        partial void EmployeeAssignmentEditSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void EmployeeAssignmentEditSelected_Execute()
        {
            this.EmployeeAssignment.AddAndEditNew();

        }
        partial void EmployeeAssignmentDeleteSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void EmployeeAssignmentDeleteSelected_Execute()
        {
            this.EmployeeAssignment.AddAndEditNew();

        }
        #endregion
        #region StaffingRequirements
        partial void StaffingRequirementsAddAndEditNew_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void StaffingRequirementsAddAndEditNew_Execute()
        {
            this.StaffingRequirements.AddAndEditNew();

        }
        partial void StaffingRequirementsEditSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void StaffingRequirementsEditSelected_Execute()
        {
            this.StaffingRequirements.EditSelected();

        }
        partial void StaffingRequirementsDeleteSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void StaffingRequirementsDeleteSelected_Execute()
        {
            this.StaffingRequirements.DeleteSelected();

        }
        #endregion
        #region Customers
        partial void CustomersAddAndEditNew_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void CustomersAddAndEditNew_Execute()
        {
            this.Customers.AddAndEditNew();
        }
        partial void CustomersEditSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void CustomersEditSelected_Execute()
        {
            this.Customers.AddAndEditNew();
        }
        partial void CustomersDeleteSelected_CanExecute(ref bool result)
        {
            result = !this.Application.User.HasPermission(Permissions.DODUser);
        }

        partial void CustomersDeleteSelected_Execute()
        {
           // this.Customers.AddAndEditNew();
            this.Customers.DeleteSelected();
        }
        #endregion

        partial void IncludeJS_Execute()
        {
            this.ShowInternal = !this.ShowInternal;
            this.FindControl("IncludeJS").DisplayName = this.ShowInternal ? "Exclude JS" : "Include JS";

        }

        partial void DODProjectsPortal_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.ShowInternal = false;
        }

        partial void DodViewSkills_Execute()
        {
            if (this.DodEmployeeAssignments.SelectedItem != null)
            {
                this.Application.ShowManageMySkills(this.DodEmployeeAssignments.SelectedItem.Employee.UserID);
            }

        }

        partial void DodEmployeeAssignmentsDeleteSelected_CanExecute(ref bool result)
        {
            // Write your code here.

        }

        partial void DodEmployeeAssignmentsDeleteSelected_Execute()
        {
            // Write your code here.

        }

    }
}
