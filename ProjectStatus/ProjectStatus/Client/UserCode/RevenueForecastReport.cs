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
using Microsoft.LightSwitch.Presentation.Implementation.Controls;
using LightSwitchApplication.UserCode;

namespace LightSwitchApplication
{
    public partial class RevenueForecastReport
    {
        partial void RevenueForecastReport_Created()
        {
            this.CurrentQuarterName = this.GetCurrentQuarterName();
            this.FindControl("RevenueForecastGrid").ControlAvailable += ChangeHeader;
        }

        private string GetCurrentQuarterName() 
        {
            DateTime today = DateTime.Today;

            int currentQuarterNumber = TimePeriod.GetQuarter(today);
            int currentQuarterYear = today.Year;

            return string.Format("Q{0} {1}", currentQuarterNumber, currentQuarterYear);
        }

        private void ChangeHeader(Object sender, ControlAvailableEventArgs args)
        {
            DataGrid myGrid;
            myGrid = args.Control as DataGrid;

            DataGridColumn column;
            ContentItemWrapperForColumnHeader headerFeature;

            column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "PERIOD REVENUE1").FirstOrDefault();
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = "JUL 31";

            column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "PERIOD REVENUE2").FirstOrDefault();
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = "AUG 15";

        }

    }

}
