using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch.Threading;
using System.Windows.Browser;
namespace LightSwitchApplication
{

    public partial class Home
    {
      

        partial void Home_Activated()
        {
            // in order to display info for the current user on the home screen
            // need to determine the user id for the logged on user
            // get the integer ID number for the employee given the userid
            string uID = GlobalStrings.LoggedOnUser();

            // within the lightswitch project employees is a paged datasource
            // get the first page of employees into a list
            List<Employee> EmpList = Employees.ToList();

            int pages = Employees.Details.PageCount;
            int testpagecount = Employees.Details.PageNumber;
            for (int p =2; p <= pages; )
            {
                Employees.Details.PageNumber++;
                List<Employee> empPage = Employees.ToList();
                EmpList.AddRange(empPage);
                p++;
            }

            foreach (Employee emp in EmpList)
            {
                if (emp.UserID.Trim() == uID.Trim())
                {
                    EmployeeID = emp.Id;
                    break;
                    //string Message = "LoggedOnUser=" + uID + "   Emp.User.ID=" + emp.UserID + "   EmployeeIndex=" + EmployeeID;
                    //Exception myEX = new Exception(Message);//todo replace this code with a break
                    //throw myEX;
                }
            }
            //// if we hit this point, no employee Id was found
            //string Message1 = "Your Employee ID :" + uID + " was not found. No time forecast data will be displayed";
            //Exception myEX1 = new Exception(Message1);
            //throw myEX1;

            //}

            //var empQ = (from emp in Employees
            //            where emp.UserID == uID
            //            select new
            //            {
            //                empID = emp.Id,
            //                UserID = emp.UserID
            //            }).FirstOrDefault();

            //if (empQ != null)
            //{
            //    EmployeeID = empQ.empID;
            //    string Message = "LoggedOnUser=" + uID + "   Emp.User.ID=" + empQ.UserID + "   EmployeeIndex=" + EmployeeID;
            //    //Exception myEX = new Exception(Message);
            //    //throw myEX;
            //}
            //else
            //{
            //    string Message = "Your Employee ID :" + uID + " was not found. No time forecast data will be displayed";
            //    //Exception myEX = new Exception(Message);
            //    //throw myEX;
            //}
     
        }

        partial void Home_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.DataWorkspace.ApplicationData.Clients.GetQuery().Execute();

            //Hide certain screen elements for the customer users
            if (this.Application.User.HasPermission(Permissions.DODCustomer))
            {
                this.FindControl("EmployeeAssignments").IsVisible = false;
                this.FindControl("GetSpecificForecast").IsVisible = false;
                this.FindControl("Help").IsVisible = false;
                this.FindControl("Announcements").IsVisible = false;
            }

        }

        partial void LogOut_Execute()
        {
            Dispatchers.Main.Invoke(() =>
            {
                HtmlPage.Window.Navigate(new Uri("../LogOut.aspx", UriKind.Relative));
            });
        }

        partial void Help_Execute()
        {
            Dispatchers.Main.Invoke(() =>
            {
                HtmlPage.Window.Navigate(new Uri("http://sdrv.ms/1a1kfP6"), "_blank");
            });
        }

    }
}
