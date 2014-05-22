using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;
using CommonUtilities.Statuses;
using System.Reflection;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        [Query(IsDefault = true)]
        public IQueryable<PTORecord> PTORecords()
        {
            return new List<PTORecord>().AsQueryable();
        }

        //Filter by country
        public IQueryable<PTORecord> PTOReport(int? countryId, DateTime? pFromDate, DateTime? pToDate)
        {
            //DateTimes to iterate over
            DateTime fromDate = (pFromDate == null) ? TimePeriod.PeriodStartDate(DateTime.Now) : ((DateTime)pFromDate).Date;
            DateTime toDate = (pToDate == null) ? TimePeriod.PeriodEndDate(DateTime.Now) : ((DateTime)pToDate).Date;


            var algo = from emp in this.Context.Employees
                       select emp;
                       //select new { emp.Id, Name = emp.FirstName + " " + emp.LastName };

            algo = (countryId == null) ? algo : algo.Where(a => a.Country_Employee == countryId); 

            //Dictionary of project names
            var employees = (countryId == null) ? 
                            (from emp in this.Context.Employees
                             select new { emp.Id, Name = emp.FirstName + " " + emp.LastName })
                             .OrderBy(emp => emp.Name)
                             .ToDictionary(p => p.Id, p => p.Name)
                             :
                             (from emp in this.Context.Employees
                              where emp.Country_Employee == countryId
                              select new { emp.Id, Name = emp.FirstName + " " + emp.LastName })
                             .OrderBy(emp => emp.Name)
                             .ToDictionary(p => p.Id, p => p.Name);

            var excepts = this.Context.TimeForecastExceptionsSet
                             .Where(tt => ((tt.StartDate >= fromDate && tt.StartDate <= toDate)
                                            || (tt.EndDate >= fromDate && tt.EndDate <= toDate)
                                            || (fromDate >= tt.StartDate && toDate <= tt.EndDate)));

            var exepsList = excepts.ToList();
            var exceptions = exepsList.ToArray();

            List<PTORecord> data = new List<PTORecord>();

            int recId = 0;
            foreach (var employee in employees)
            {
                DateTime index = fromDate;
                DateTime toPeriodEnd = TimePeriod.PeriodEndDate(index);
                PTORecord ptl = new PTORecord();
                ptl.RecordId = recId;
                ptl.EmployeeId = employee.Key;
                ptl.EmployeeName = employee.Value;
                for (int i = 0; index < toDate; i++)
                {
                    decimal exceptionHours = this.GetExceptionHours(employee.Key, index, TimePeriod.PeriodEndDate(index), exepsList);
                    ptl.GetType().GetProperty("PeriodPTOHours" + i).SetValue(ptl, (decimal)Math.Round(exceptionHours * 4) / (decimal)4, null);

                    index = TimePeriod.PeriodEndDate(index).AddDays(1);
                }
                data.Add(ptl);
                recId++;
            }


            return data.AsQueryable();
        }
    }
}
