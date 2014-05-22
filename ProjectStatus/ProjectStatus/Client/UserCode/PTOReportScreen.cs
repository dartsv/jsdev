using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using LightSwitchApplication.UserCode;
using System.Windows.Controls;
using Microsoft.LightSwitch.Threading;

namespace LightSwitchApplication
{
    public partial class PTOReportScreen
    {

        DataGrid TimeTrackGrid = null;

        partial void PTOReportScreen_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.PTORecordpFromDate = TimePeriod.PeriodStartDate(DateTime.Today);
            this.PTORecordpToDate = TimePeriod.PeriodEndDate(this.PTORecordpFromDate.AddMonths(5).AddDays(15));
            //this.CountryParam = this.DataWorkspace.ApplicationData.Countries.FirstOrDefault();
        }

        private void ChangeHeader(Object sender, ControlAvailableEventArgs args)
        {
            this.TimeTrackGrid = args.Control as DataGrid;
            ChangeHeaders();
        }
        private void ChangeHeaders()
        {
            DateTime indexDate = this.PTORecordpFromDate;
            if (this.TimeTrackGrid == null) throw new InvalidOperationException("Grid cannot be null");
            this.TimeTrackGrid.Dispatcher.BeginInvoke(() =>
            {
                int columnStartIndex = 1;
                IContentItem rowTemplate = ((IContentItem)this.TimeTrackGrid.DataContext).ChildItems[0];
                for (int i = 0; i < rowTemplate.ChildItems.Count - 1; i++)
                {
                    IContentItem column = rowTemplate.ChildItems[columnStartIndex + i];
                    column.DisplayName = TimePeriod.PeriodEndDate(indexDate).ToString("MMM dd");
                    indexDate = TimePeriod.PeriodEndDate(indexDate).AddDays(1);
                }
            });
        }

        partial void PTOReportScreen_Created()
        {
            this.FindControl("PtoReportGrid").ControlAvailable += ChangeHeader;
            this.FindControl("PtoReportGrid").ControlAvailable -= ChangeHeader;
        }

        partial void PTORecordpFromDate_Changed()
        {
            this.PTORecordpToDate = TimePeriod.PeriodEndDate(this.PTORecordpFromDate.AddMonths(5).AddDays(15));
            if (TimeTrackGrid != null)
            {
                Dispatchers.Main.BeginInvoke(() => ChangeHeaders());
            }
        }

        partial void Run_Execute()
        {
            this.PTOReport.Load();
           
        }

        partial void PTOReport_SelectionChanged()
        {
            this.TimeForecastExceptionsSet.Load();
        }
    }
}
