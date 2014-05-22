using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Specialized;

namespace LightSwitchApplication
{
    public partial class TimeForecastReport
    {

         const string CON = "Consulting";

        
        partial void FilterConsultingBtn_Execute()
        {
            // ShowConsulting is an optional parameter
            //CON is a string variable that holds the value "Consulting"
            if (ShowConsulting != CON)
            {
                ShowConsulting = CON;
                this.FindControl("FilterConsultingBtn").DisplayName = "Show All";
                this.FindControl("MultiPeriodForecasts").DisplayName = "Time Forecasts - Consulting Only";
                this.FindControl("UtilizationTotal").DisplayName = "Utilization - Consulting Only";
               
            }
            else
            {
                ShowConsulting = null;
                var ctl = this.FindControl("FilterConsultingBtn");
                ctl.DisplayName = "Show Consulting Only";
                this.FindControl("MultiPeriodForecasts").DisplayName = "Time Forecasts - All Departments";
                this.FindControl("UtilizationTotal").DisplayName = "Utilization - All Departments";
            }
        }

        partial void TimeForecastReport_Activated()
        {
            // Show consulting only by default
            ShowConsulting = CON;
           
        }

        //todo look at adding totals to the report - this code works and puts the total in a property called UtilizationTotal
        partial void MultiPeriodForecasts_Changed(NotifyCollectionChangedEventArgs e)
        {
            // Utilization is the name of the property in my MultiPeriodForecasts entity that I want to average
            this.UtilizationTotal = this.MultiPeriodForecasts.Average(p => p.Utilization);
        }

        // Dynamically change grid headers - see example below
        //http://msdnrss.thecoderblogs.com/2012/07/dynamically-setting-column-names-on-an-editable-grid-babar-ismail/ 
        //
        DataGrid myGrid = null;
        partial void TimeForecastReport_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.FindControl("MultiPeriodForecasts").ControlAvailable += MyGrid_ControlAvailable;
            UpdateColumnNames();
        }
        void MyGrid_ControlAvailable(object sender, ControlAvailableEventArgs e)
        {
            this.myGrid = (DataGrid)e.Control;
        }

        private void UpdateColumnNames()
        {
            if (this.myGrid == null) throw new InvalidOperationException("Grid cannot be null");
            this.myGrid.Dispatcher.BeginInvoke(() =>
            {
                int columnStartIndex = 4;
                IContentItem rowTemplate = ((IContentItem)this.myGrid.DataContext).ChildItems[0];
                IContentItem column1a = rowTemplate.ChildItems[columnStartIndex];
                column1a.DisplayName = "Util. " + PeriodEndDate(DateTime.Today).ToString("MMM dd") ;
                IContentItem column1b = rowTemplate.ChildItems[++columnStartIndex];
                column1b.DisplayName = "Avail. " + PeriodEndDate(DateTime.Today).ToString("MMM dd");
                
                DateTime NextPeriodEnd = PeriodEndDate(PeriodEndDate(DateTime.Today).AddDays(1));
                IContentItem column2a = rowTemplate.ChildItems[++columnStartIndex];
                column2a.DisplayName = "Util. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
                IContentItem column2b = rowTemplate.ChildItems[++columnStartIndex];
                column2b.DisplayName = "Avail. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
               
                NextPeriodEnd = PeriodEndDate(NextPeriodEnd.AddDays(1));
                IContentItem column3a = rowTemplate.ChildItems[++columnStartIndex];
                column3a.DisplayName = "Util. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
                IContentItem column3b = rowTemplate.ChildItems[++columnStartIndex];
                column3b.DisplayName = "Avail. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");

                NextPeriodEnd = PeriodEndDate(NextPeriodEnd.AddDays(1));
                IContentItem column4a = rowTemplate.ChildItems[++columnStartIndex];
                column4a.DisplayName = "Util. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
                IContentItem column4b = rowTemplate.ChildItems[++columnStartIndex];
                column4b.DisplayName = "Avail. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");

                NextPeriodEnd = PeriodEndDate(NextPeriodEnd.AddDays(1));
                IContentItem column5a = rowTemplate.ChildItems[++columnStartIndex];
                column5a.DisplayName = "Util. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
                IContentItem column5b = rowTemplate.ChildItems[++columnStartIndex];
                column5b.DisplayName = "Avail. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");

                NextPeriodEnd = PeriodEndDate(NextPeriodEnd.AddDays(1));
                IContentItem column6a = rowTemplate.ChildItems[++columnStartIndex];
                column6a.DisplayName = "Util. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
                IContentItem column6b = rowTemplate.ChildItems[++columnStartIndex];
                column6b.DisplayName = "Avail. " + PeriodEndDate(NextPeriodEnd).ToString("MMM dd");
            });
        }


        partial void MultiPeriodForecasts_Loaded(bool succeeded)
        {
            //  changes color of status font
            // "Chargeable" is the name of the control, in this case the field on the datagrid.
            IContentItemProxy StatusControl = this.FindControl("Utilization1");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization11");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization11");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization2");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization3");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization4");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization5");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

        }


        //Period Start and Period End manipulation code
        //*************************************************************************************************************  
        /// <summary>
        /// Gets the first day of the month.
        /// </summary>
        /// <param name="givenDate">The given date.</param>
        /// <returns>the first day of the month</returns>
        public static DateTime GetFirstDayOfMonth(DateTime givenDate)
        {
            return new DateTime(givenDate.Year, givenDate.Month, 1);
        }

        private static DateTime GetTheLastDayOfMonth(DateTime givenDate)
        {
            return GetFirstDayOfMonth(givenDate).AddMonths(1).Subtract(new TimeSpan(1, 0, 0, 0, 0));
        }

        /// <summary>
        /// Gets the start date of the time period for a given date.
        /// </summary>
        /// <param name="givenDate">The given date.</param>
        /// <returns>the start date of the time period</returns>
        private static DateTime PeriodStartDate(DateTime givenDate)
        {
            if (givenDate.Day >= 16)
            {
                return new DateTime(givenDate.Year, givenDate.Month, 16);
            }
            else
            {
                return new DateTime(givenDate.Year, givenDate.Month, 1);
            }
        }

        /// <summary>
        /// Gets the end date of the time period for a given date.
        /// </summary>
        /// <param name="givenDate">The given date.</param>
        /// <returns>the end date of the time period</returns>
        private static DateTime PeriodEndDate(DateTime givenDate)
        {
            if (givenDate.Day <= 15)
            {
                return new DateTime(givenDate.Year, givenDate.Month, 15);
            }
            else
            {
                return GetTheLastDayOfMonth(givenDate);
            }
        }

        //todo look at adding totals to the report - this code works and puts the total in a property called UtilizationTotal
        partial void Forecasts_Changed(NotifyCollectionChangedEventArgs e)
        {
            // Utilization is the name of the property in my Forecasts entity that I want to sum
            this.UtilizationTotal = this.Forecasts.Average(p => p.Utilization);
        }
    }
    public class ChangeUtilFontColorStatus : IValueConverter
    {
        //  --------------------------------------------------------------------------------------------------------

        //  class to change colors

        //  --------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                decimal isValue = (decimal) value;
                if (isValue < 80)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                if (isValue < 100)
                {
                    return new SolidColorBrush(Colors.Orange);
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
