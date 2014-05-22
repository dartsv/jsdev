using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;

using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Threading.Tasks;
using Microsoft.LightSwitch.Threading;

namespace LightSwitchApplication
{
    public partial class StatusBoardFiltered
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
                this.ProjectStatus = -100;
                this.FindControl("ShowInactiveBtn").DisplayName = "Show Inactive";
            }
            else
            {
                this.ShowInactive = true;
                this.ProjectStatus = 100;
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

        partial void StatusBoardFiltered_Activated()
        {
            // Load the Excluded Client Parameter to avoid errors when saving (Parameter is required to save)
            ExcludedClient = true;
            this.ProjectStatus = -100;
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

        partial void gridAddAndEditNew1_CanExecute(ref bool result)
        {
            // Write your code here.

        }

        partial void gridAddAndEditNew2_CanExecute(ref bool result)
        {
            // Write your code here.

        }

        partial void gridAddAndEditNew2_Execute()
        {
            Task createProject = Task.Factory.StartNew(() =>
            {
                // This method defaults the manager and director to the current logged on user and displays the edit screen

                // First find the employee number of the current logged on user
                // Employees must have paging disabled on the designer or the code will only bring in the first page of employee records and create errors
                string UserName = GlobalStrings.LoggedOnUser();
                int i = -1;
                int idxFound = -1;
                foreach (Employee emp in Employees)
                {
                    i++;
                    if (emp.UserID == UserName)
                    {
                        idxFound = i;
                        break;
                    }
                }
                // Throw an error if not found, otherwise initialize the fields and display the edit screen
                if (idxFound < 0)
                {
                    string Message = "There is not an employee record for the current logged on user. Unable to create a new project.";
                    Exception myEX = new Exception(Message);
                    throw myEX;
                }
                else
                {
                    // Create a new project entry in the projects grid and set the Manager and Director fields
                    Project newProject = new Project();
                    newProject.Director = Employees.ElementAt(i);
                    newProject.Manager = Employees.ElementAt(i);
                    // Set the selected item to the new project
                    this.Projects.SelectedItem = newProject;
                    // Display the Edit screen for the selected new project.
                    // this.Projects.EditSelected();

                }

            });
            createProject.Wait();

            if (this.Projects.SelectedItem != null)
            {
                this.FindControl("NewEditProject").DisplayName = "Add Project";
                this.OpenModalWindow("NewEditProject");
            }
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

        partial void gridEditSelected_CanExecute(ref bool result)
        {
            // Write your code here.

        }

        partial void gridEditSelected_Execute()
        {
            if (this.Projects.SelectedItem != null)
            {
                this.FindControl("NewEditProject").DisplayName = "Edit Project";
                this.OpenModalWindow("NewEditProject");
            }

        }

        partial void SaveProjectChanges_Execute()
        {
            this.WasCanceled = false;
            this.CloseModalWindow("NewEditProject");
            this.Save();
        }

        partial void DiscardProjectChanges_Execute()
        {
            this.WasCanceled = true;
            this.CloseModalWindow("NewEditProject");
            foreach (Project project in this.DataWorkspace.ApplicationData.Details.
                                GetChanges().OfType<Project>())
                project.Details.DiscardChanges();

        }

        partial void StatusBoardFiltered_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.ProjectUserID = this.Application.User.Name;
        }

        partial void ViewSkills_Execute()
        {
            if (this.EmployeeAssignment.SelectedItem != null)
            {
                this.Application.ShowManageMySkills(this.EmployeeAssignment.SelectedItem.Employee.UserID);
            }
        }

        partial void InvoiceSummaryReport_Execute()
        {
            string invSummaryReport = "Invoice Summary Report.xlsx";
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = String.Format("https://{0}/Static/{1}",
                    Application.DefaultDomain,
                    invSummaryReport);
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });

        }

        partial void DodViewSkills_Execute()
        {
            if (this.DODEmployeeAssignments.SelectedItem != null)
            {
                this.Application.ShowManageMySkills(this.DODEmployeeAssignments.SelectedItem.Employee.UserID);
            }
        }

        partial void CustomersDeleteSelected_CanExecute(ref bool result)
        {
            // Write your code here.

        }

        partial void CustomersDeleteSelected_Execute()
        {
            // Write your code here.
            this.Customers.DeleteSelected();

        }
    }
    public class ChangeProjStatusFontColorStatus : IValueConverter
    {
        //  --------------------------------------------------------------------------------------------------------

        //  class to change colors

        //  --------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                //decimal isValue = (decimal)value;
                //if (isValue < 80)
                //{
                //    return new SolidColorBrush(Colors.Red);
                //}
                //if (isValue < 100)
                //{
                //    return new SolidColorBrush(Colors.Orange);
                //}

                //return new SolidColorBrush(Colors.Black);
                string StatusColor = value.ToString();
                if (StatusColor == "Green")
                {
                    return new SolidColorBrush(Colors.Green);
                }
                if (StatusColor == "Red")
                {
                    return new SolidColorBrush(Colors.Red);
                }
                if (StatusColor == "Yellow")
                {
                    return new SolidColorBrush(Colors.Yellow);
                }
                return new SolidColorBrush(Colors.Black);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
