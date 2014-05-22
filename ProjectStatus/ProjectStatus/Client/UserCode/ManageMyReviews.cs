using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
namespace LightSwitchApplication
{
    public partial class ManageMyReviews
    {
        partial void ManageMyReviews_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            Employee emp = GetLoggedOnUserID();
            LoggedOnUser = emp.UserID;

        }


        private Employee GetLoggedOnUserID()
        {
            // This method defaults the manager and director to the current logged on user and displays the edit screen

            // First find the employee number of the current logged on user
            string UserName = GlobalStrings.LoggedOnUser();
            int i = -1;
            foreach (Employee emp in this.DataWorkspace.ApplicationData.Employees)
            {
                i++;
                if (emp.UserID.ToLower() == UserName.ToLower())
                {
                    return emp;
                }
            }
            // Throw an error if not found, otherwise initialize the fields and display the edit screen
            string Message = "There is not an employee record for the current logged on user. Unable to create a new review.";
            Exception myEX = new Exception(Message);
            throw myEX;
        }

        partial void Method_Execute()
        {
            if ((ReviewHeaders.SelectedItem.Reviewer.UserID == LoggedOnUser) | (ReviewHeaders.SelectedItem.Published))
            {
                Employee emp = ReviewHeaders.SelectedItem.Employee;
                Employee reviewer = ReviewHeaders.SelectedItem.Reviewer;
                this.Application.ShowReviewForm(ReviewHeaders.SelectedItem.Id, emp.Id, "ManageMyReviews");
            }
            else
            {
                this.ShowMessageBox("You are not authorized to edit or view this review");
            }
        }
    }
}
