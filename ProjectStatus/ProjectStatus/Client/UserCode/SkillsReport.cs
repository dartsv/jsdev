using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Collections.Specialized;
using Microsoft.LightSwitch.Threading;
using System.Windows.Controls;
using System.Windows.Data;

namespace LightSwitchApplication
{
    public partial class SkillsReport
    {
        partial void SkillsReport_Activated()
        {
            this.LevelFilter = "Certified";
        }

        partial void Skills_Changed(NotifyCollectionChangedEventArgs e)
        {
            this.SkillFilter = this.Skills.FirstOrDefault();
        }

        DataGrid myGrid = null;
        void MyGrid_ControlAvailable(object sender, ControlAvailableEventArgs e)
        {
            this.myGrid = (DataGrid)e.Control;
        }


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
        public static DateTime GetFirstDayOfMonth(DateTime givenDate)
        {
            return new DateTime(givenDate.Year, givenDate.Month, 1);
        }
        private static DateTime GetTheLastDayOfMonth(DateTime givenDate)
        {
            return GetFirstDayOfMonth(givenDate).AddMonths(1).Subtract(new TimeSpan(1, 0, 0, 0, 0));
        }
        private void UpdateColumnNames()
        {
            if (this.myGrid == null) throw new InvalidOperationException("Grid cannot be null");
            this.myGrid.Dispatcher.BeginInvoke(() =>
            {
                int columnStartIndex = 5;
                IContentItem rowTemplate = ((IContentItem)this.myGrid.DataContext).ChildItems[0];
                IContentItem column1a = rowTemplate.ChildItems[columnStartIndex];
                column1a.DisplayName = "Util. " + PeriodEndDate(DateTime.Today).ToString("MMM dd");
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

        partial void GetMultiperiodForecasts_Loaded(bool succeeded)
        {
            //  changes color of status font
            // "Chargeable" is the name of the control, in this case the field on the datagrid.
            IContentItemProxy StatusControl = this.FindControl("Utilization1");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization");
            StatusControl.SetBinding(TextBlock.ForegroundProperty, "Value", new ChangeUtilFontColorStatus(), BindingMode.OneWay);

            StatusControl = this.FindControl("Utilization1");
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

        partial void SkillsReport_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            this.FindControl("GetMultiperiodForecasts").ControlAvailable += MyGrid_ControlAvailable;
            UpdateColumnNames();
        }

    }
}
