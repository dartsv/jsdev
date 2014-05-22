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
    public partial class ReviewListByEmployee
    {

        partial void ReviewHeadersAddAndEditNew_Execute()
        {
            // Launch the review form
            this.Application.ShowReviewForm(null,Employees.SelectedItem.Id,"ReviewListByEmployee");
        }

        partial void ReviewHeadersEditSelected_Execute()
        {
            if (ReviewHeaders.SelectedItem != null)
            {
               
                // only allow the employee, reviewer or supervisor to open the review
                Employee emp = GetLoggedOnUserID();
                if (( ReviewHeaders.SelectedItem.Published & (ReviewHeaders.SelectedItem.Employee == emp | ReviewHeaders.SelectedItem.Employee.Supervsr == emp)) 
                    | ( ReviewHeaders.SelectedItem.Reviewer == emp ))
                {
                    this.Application.ShowReviewForm(ReviewHeaders.SelectedItem.Id, Employees.SelectedItem.Id, "ReviewListByEmployee");
                }
                //todo: consider limiting this to specific departments
                else if (ReviewHeaders.SelectedItem.Published & Application.Current.User.HasPermission(Permissions.ReviewAdmin))
                {
                    this.Application.ShowReviewForm(ReviewHeaders.SelectedItem.Id, Employees.SelectedItem.Id, "ReviewListByEmployee");
                }
                else
                {
                    string Message = "You are not authorized to open this review.";
                    Exception myEX = new Exception(Message);
                    throw myEX;
                }               
            }
            else
            {
                string Message = "No review record was selected.";
                Exception myEX = new Exception(Message);
                throw myEX;
            }
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

        partial void ReviewHeadersDeleteSelected_Execute()
        {
            if (ReviewHeaders.SelectedItem != null)
            {
                if (ReviewHeaders.SelectedItem.SignedStatus != "Not Signed")
                {
                    string Message = "You cannot delete a signed review";
                    Exception myEX = new Exception(Message);
                    throw myEX;
                }
                // only allow the reviewer to delete reviews
                Employee emp = GetLoggedOnUserID();
                if (ReviewHeaders.SelectedItem.Reviewer == emp)
                {
                    ReviewHeaders.SelectedItem.Delete();
                    return; // allow delete operation to procede
                }
                else
                {
                    string Message = "You are not authorized to delete this review.";
                    Exception myEX = new Exception(Message);
                    throw myEX;
                }
            }
            else
            {
                string Message = "No review record was selected.";
                Exception myEX = new Exception(Message);
                throw myEX;
            }
        }
    }
}
