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
using Microsoft.LightSwitch.Presentation.Implementation.Controls;
using Microsoft.LightSwitch.Threading;
namespace LightSwitchApplication
{
    public partial class RevenueForecast
    {

        private int CurrentQuarter = TimePeriod.GetQuarter(DateTime.Today);

        private bool HeadersChanged = false;
        DataGrid myGrid = null;

        partial void RevenueForecast_Created()
        {
            
            this.CurrentQuarterName = this.GetCurrentQuarterName();
            this.ReportGenerationDate = this.GetReportGenerationDate();
            this.FindControl("RevenueForecastGrid").ControlAvailable += ChangeHeader;
            this.FindControl("RevenueForecastGrid").ControlAvailable -= ChangeHeader;
        }

        private string GetCurrentQuarterName()
        {
            return string.Format("Q{0} {1} Revenue Forecast", this.Quarter, this.Year);
        }
        private string GetReportGenerationDate()
        {
            return string.Format("As of {0}", DateTime.Now.ToString("MMMM dd, yyyy"));
        }

        private void ChangeMonthHeaders(DataGrid myGrid, DateTime periodDate, int month)
        {

            DateTime periodEnd = TimePeriod.PeriodEndDate(periodDate);

            DataGridColumn column;
            ContentItemWrapperForColumnHeader headerFeature;

            //column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "PERIOD REVENUE" + (((month - 1) * 2) + 1).ToString()).FirstOrDefault();
            column = (DataGridColumn)myGrid.Columns[3 + (month - 1) * 4 ];
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = periodEnd.ToString("MMM dd");

            periodEnd = TimePeriod.PeriodEndDate(periodEnd.AddDays(1));

            //column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "PERIOD REVENUE" + (((month - 1) * 2) + 2).ToString()).FirstOrDefault();
            column = (DataGridColumn)myGrid.Columns[3 + (month - 1) * 4 + 1];
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = periodEnd.ToString("MMM dd");

            //column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "EXPENSE ESTIMATE" + month.ToString()).FirstOrDefault();
            column = (DataGridColumn)myGrid.Columns[3 + (month - 1) * 4 + 2];
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = string.Format("{0} EXP. EST.", periodDate.ToString("MMM"));

            //column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "MONTH REVENUE" + month.ToString()).FirstOrDefault();
            column = (DataGridColumn)myGrid.Columns[3 + (month - 1) * 4 + 3];
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = string.Format("{0} REVENUE", periodDate.ToString("MMM"));

        }

        private void ChangeHeader(Object sender, ControlAvailableEventArgs args)
        {
            myGrid = args.Control as DataGrid;
            ChangeHeaders();
        }

        private void ChangeHeaders()
        {
            //if (this.HeadersChanged == true) return;
            DateTime index = new DateTime(this.Year, ((CurrentQuarter - 1) * 3) + 1, 1);
            DateTime periodEnd = TimePeriod.PeriodEndDate(index);

            for (int i = 1; i <= 3; i++)
            {
                periodEnd = TimePeriod.PeriodEndDate(index);
                this.ChangeMonthHeaders(myGrid, index, i);
                index = periodEnd.AddMonths(1);
            }

            periodEnd = TimePeriod.PeriodEndDate(index);

            DataGridColumn column;
            ContentItemWrapperForColumnHeader headerFeature;

            //column = (DataGridColumn)myGrid.Columns.Where(a => a.Header.ToString().ToUpperInvariant() == "QUARTER REVENUE").FirstOrDefault();
            column = (DataGridColumn)myGrid.Columns[15];
            headerFeature = (ContentItemWrapperForColumnHeader)column.Header;
            headerFeature.ContentItem.DisplayName = string.Format("Q{0} {1} REVENUE", CurrentQuarter, this.Year);

            this.HeadersChanged = true;
        }

        partial void RevenueForecast_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.expPerc = 12;
            this.countryId = 0;
            this.Quarter = TimePeriod.GetQuarter(DateTime.Today);
            this.CurrentQuarter = this.Quarter;
            this.Year = DateTime.Now.Year;
        }

        partial void SelectedCountry_Changed()
        {
            if (this.SelectedCountry != null) this.countryId = this.SelectedCountry.Id;
            else this.countryId = 0;
        }

        partial void Quarter_Changed()
        {
            this.CurrentQuarterName = this.GetCurrentQuarterName();
            this.CurrentQuarter = this.Quarter;
            if (myGrid != null)
            {
                Dispatchers.Main.BeginInvoke(() => ChangeHeaders());
            }
        }

        partial void Year_Changed()
        {
            this.CurrentQuarterName = this.GetCurrentQuarterName();
            if (myGrid != null)
            {
                Dispatchers.Main.BeginInvoke(() => ChangeHeaders());
            }
        }

        partial void ExportRevenueForecastReport_Execute()
        {
            Dispatchers.Main.BeginInvoke(() =>
            {
                string url = string.Format(
                    "http://{0}/revenue-forecast/export/excel?quarter={1}&year={2}&client={3}&countryId={4}&expensePercentage={5}",
                    Application.DefaultDomain,
                    this.Quarter,
                    this.Year,
                    (this.ClientFilter == null) ? "" : this.ClientFilter.Name,
                    this.countryId,
                    this.expPerc
                    );
                Uri uri = new Uri(url);
                System.Windows.Browser.HtmlPage.Window.Navigate(uri);
            });

        }
    }
}
