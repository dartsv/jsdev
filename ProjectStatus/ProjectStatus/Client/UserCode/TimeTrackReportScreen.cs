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
    public partial class TimeTrackReportScreen
    {
        DataGrid TimeTrackGrid = null;
        string loggedUser = GlobalStrings.LoggedOnUser();

        partial void ToDate_Changed()
        {
            this.FromDate = LightSwitchApplication.UserCode.TimePeriod.PeriodStartDate(ToDate);
            if (TimeTrackGrid != null)
            {
                Dispatchers.Main.BeginInvoke(() => ChangeHeaders());
            }
        }

        partial void TimeTrackReportScreen_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.FromDate = LightSwitchApplication.UserCode.TimePeriod.PeriodStartDate(ToDate);
            this.ToDate = LightSwitchApplication.UserCode.TimePeriod.PeriodEndDate(DateTime.Now);
            this.Employee = this.DataWorkspace.ApplicationData.Employees.Where(e => e.UserID == loggedUser).FirstOrDefault();

            //Only Administrators can see other peoples' Report
            if(!this.Application.User.HasPermission(Permissions.SecurityAdministration)) 
                this.FindControl("EmployeeParam").IsEnabled = false;
        }

        private void ChangeHeaders()
        {
            DateTime indexDate = this.FromDate;
            if (this.TimeTrackGrid == null) throw new InvalidOperationException("Grid cannot be null");
            this.TimeTrackGrid.Dispatcher.BeginInvoke(() =>
            {
                int columnStartIndex = 2;
                IContentItem rowTemplate = ((IContentItem)this.TimeTrackGrid.DataContext).ChildItems[0];
                //In the for upper conditions it is -1 -2 because it includes the Total Hours column
                for (int i = 0; i < rowTemplate.ChildItems.Count - 1 - 2; i++)
                {
                    IContentItem column = rowTemplate.ChildItems[columnStartIndex+i];
                    column.DisplayName = indexDate.ToString("ddd M/d");
                    column.IsVisible = !(indexDate > this.ToDate);
                    indexDate = indexDate.AddDays(1);
                }
            });
        }

        private void ChangeHeader(Object sender, ControlAvailableEventArgs args)
        {
            this.TimeTrackGrid = args.Control as DataGrid;
            ChangeHeaders();
        }

        partial void TimeTrackReportScreen_Created()
        {
            this.FindControl("TimeTrackReportGrid").ControlAvailable += ChangeHeader;
            this.FindControl("TimeTrackReportGrid").ControlAvailable -= ChangeHeader;
        }

        partial void Export_Execute()
        {
            //Format Excel filename
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url =
                    String.Format("http://{0}/timetracking/{1}/{2}/export/excel",
                    Application.DefaultDomain,
                    this.Employee.Id,
                    this.ToDate.ToString("yyyy-MM-dd"));
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });
        }

     

    }
}
