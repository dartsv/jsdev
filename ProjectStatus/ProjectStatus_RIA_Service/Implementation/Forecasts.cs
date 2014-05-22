using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {

        private Forecast GetPeriodForecast(Employee employee, DateTime periodStart, DateTime periodEnd)
        {

            int workHours = (int)this.GetWorkHours(employee, periodStart, periodEnd);
            int weekDays = (int)TimePeriod.WeekdaysInPeriod(periodStart, periodEnd);
            int holidayHours = (int)this.GetHolidayHours(employee, periodStart, periodEnd);
            decimal chargeableHours = this.GetChargeableHours(employee,periodStart,periodEnd);
            decimal nonChargeableHours = this.GetNonChargeableHours(employee, periodStart, periodEnd);
            decimal exceptionHours = this.GetExceptionHours(employee,periodStart,periodEnd);

            decimal utilization = (chargeableHours / workHours) * 100;
            decimal availability = workHours - chargeableHours - nonChargeableHours;

            Forecast forecast = new Forecast();

            forecast.Availability = availability;
            forecast.ChargeableHours = chargeableHours;
            forecast.Country = employee.Country.Name;
            forecast.Department = employee.Department;
            forecast.EmployeeID = employee.Id;
            forecast.ExceptionHours = exceptionHours;
            forecast.FirstName = employee.FirstName;
            forecast.HolidayHours = holidayHours;
            forecast.LastName = employee.LastName;
            forecast.NonChargeableHours = nonChargeableHours;
            forecast.PeriodEnd = periodEnd;
            forecast.Region = employee.Region;
            forecast.TimePeriod = 1;
            forecast.Title = employee.Title;
            forecast.UserId = employee.UserID;
            forecast.Utilization = utilization;
            forecast.WeekDays = weekDays;
            forecast.WorkHours = workHours;

            return forecast;
        }

        #region SingleForecast
 
        /// <summary>
        /// Get the time forecast for a given employee in a certain period of time
        /// </summary>
        private Forecast GetSingleForecast(int employeeId, DateTime periodStart, DateTime periodEnd)
        {

            //Get the employee and his country
           var employeeQuery = from employee in this.Context.Employees
                                join country in this.Context.Countries
                                on employee.Country.Id equals country.Id
                                where employee.Id == employeeId
                                select new { employee, country };

            var fullExceptionList = this.Context.TimeForecastExceptionsSet.ToList();
            var periodForecasts = this.Context.TimeForecasts.ToList();
            var employeeResult = employeeQuery.ToList();

            var result = employeeResult.First();
            Employee currentEmployee = result.employee;
            currentEmployee.Country = result.country;

            var countriesHolidays = (from c in this.Context.Countries
                                     join ch in this.Context.CountryHolidays
                                     on c.Id equals ch.Country.Id
                                     where c.Id == currentEmployee.Country.Id
                                     select ch).ToList();

            //Get the holiday count
            int holidayCount = this.GetHolidayCount(periodStart, periodEnd, currentEmployee.Country);

            // Weekdays in the time period
            int weekDays = TimePeriod.WeekdaysInPeriod(periodStart, periodEnd) - holidayCount;

            // Working hours in the time period
            decimal hoursInPeriod = weekDays * 8;
            decimal algo = this.GetExceptionHours(employeeId, periodStart, periodEnd, fullExceptionList); ;

            var timeForecastQuery = from TimeFcst in this.Context.TimeForecasts
                                    where 
                                            (TimeFcst.StartDate >= periodStart && TimeFcst.StartDate <= periodEnd)
                                        ||  (TimeFcst.EndDate >= periodStart && TimeFcst.StartDate <= periodEnd) 
                                        &&  TimeFcst.TimeForecast_Employee == employeeId
                                    let StartD = (periodStart > TimeFcst.StartDate) ? periodStart : TimeFcst.StartDate
                                    let EndD = (periodEnd < TimeFcst.EndDate) ? periodEnd : TimeFcst.EndDate
                                    select new
                                    {
                                        ProjectID = TimeFcst.TimeForecast_Project,
                                        EmployeeID = TimeFcst.TimeForecast_Employee,
                                        StartDate = StartD,
                                        EndDate = EndD,
                                        Percent = TimeFcst.Percent,
                                        Chargeable = TimeFcst.Chargeable,
                                        Exception = TimeFcst.Project.ForecastException
                                    };

            // Create a new list to hold time forecast exceptions
            var ProjectsQuery = new List<TimeForecastRecords>();

            foreach (var q1 in timeForecastQuery)
            {
                TimeForecastRecords tr = new TimeForecastRecords();
                tr.ProjectID = q1.ProjectID;
                tr.EmployeeID = q1.EmployeeID;
                tr.StartDate = q1.StartDate;
                tr.EndDate = q1.EndDate;
                tr.Percent = q1.Percent;
                tr.Chargeable = q1.Chargeable;
                //tr.Exception = q1.Exception;
                ProjectsQuery.Add(tr);

            }

            var TimeFcstExceptQuery = from TimeFcstExcept in this.Context.TimeForecastExceptionsSet
                                      where
                                            (TimeFcstExcept.StartDate >= periodStart || TimeFcstExcept.StartDate <= periodEnd) 
                                        &&  (TimeFcstExcept.EndDate >= periodStart || TimeFcstExcept.EndDate <= periodEnd) 
                                        &&  TimeFcstExcept.TimeForecastExceptions_Employee == employeeId
                                      let StartD = (periodStart > TimeFcstExcept.StartDate) ? periodStart : TimeFcstExcept.StartDate
                                      let EndD = (periodEnd < TimeFcstExcept.EndDate) ? periodEnd : TimeFcstExcept.EndDate
                                      orderby employeeId, StartD, EndD
                                      select new
                                      {
                                          ProjectID = TimeFcstExcept.TimeForecastExceptions_Project,
                                          EmployeeID = TimeFcstExcept.TimeForecastExceptions_Employee,
                                          StartDate = StartD,
                                          EndDate = EndD,
                                          Percent = TimeFcstExcept.Percent,
                                      };

            var ExceptionQuery = new List<TimeForecastExceptions>();
            foreach (var q1 in TimeFcstExceptQuery)
            {
                TimeForecastExceptions tre = new TimeForecastExceptions();
                tre.TimeForecastExceptions_Project = q1.ProjectID;
                tre.TimeForecastExceptions_Employee = q1.EmployeeID;
                tre.StartDate = q1.StartDate;
                tre.EndDate = q1.EndDate;
                tre.Percent = q1.Percent;
                ExceptionQuery.Add(tre);
            }

            // Initialize a variable to track changes in user Id as we iterate through the foreach loop
            int LastID = -1;
            var TimeFcstQuery2 = new List<TimeForecastHrs>();
            foreach (var q1 in ProjectsQuery)
            {
                TimeForecastHrs tfh = new TimeForecastHrs();
                tfh.ProjectID = q1.ProjectID;
                tfh.EmployeeID = q1.EmployeeID;
                tfh.StartDate = q1.StartDate;
                tfh.EndDate = q1.EndDate;
                tfh.Percent = q1.Percent;

                int workdaysInPeriod = TimePeriod.WeekdaysInPeriod(q1.StartDate, q1.EndDate) - this.GetHolidayCount(q1.StartDate, q1.EndDate, currentEmployee.Country);
                //decimal WorkdaysInPeriodAfterExceptions = WorkdaysInPeriod - ExceptionDays(q1.StartDate, q1.EndDate, ExceptionQuery, HolidayList); 
                decimal WorkdaysInPeriodAfterExceptions = workdaysInPeriod - (algo/8);
                //tfh.Period1ChargeableHrs = (q1.Chargeable) ? WorkdaysInPeriod * 8 * q1.Percent / 100 : 0;
                tfh.Period1ChargeableHrs = (q1.Chargeable) ? WorkdaysInPeriodAfterExceptions * 8 * q1.Percent / 100 : 0;
                tfh.Period1NonChargeableHrs = (!q1.Chargeable) ? WorkdaysInPeriodAfterExceptions * 8 * q1.Percent / 100 : 0;
                //tfh.Period1ExceptionHrs = (q1.Exception) ? WorkdaysInPeriod * 8 * q1.Percent / 100 : 0;
                if (LastID != tfh.EmployeeID)
                {
                    tfh.Period1ExceptionHrs = algo;
                    //tfh.Period1ExceptionHrs = this.GetExceptionHours(tfh.EmployeeID, periodStart, periodEnd, fullExceptionList);
                    LastID = tfh.EmployeeID;
                }

                TimeFcstQuery2.Add(tfh);
            }

            //Handle the case where there are no projects billed for a time period, but there are forecast exception hours
            if (TimeFcstQuery2.Count() == 0 & ExceptionQuery.Count() > 0)
            {
                TimeForecastHrs tfh = new TimeForecastHrs();
                tfh.EmployeeID = (int)employeeId;
                tfh.Period1ExceptionHrs = this.GetExceptionHours(tfh.EmployeeID, periodStart, periodEnd, fullExceptionList);

                TimeFcstQuery2.Add(tfh);
            }

            var EmpQuery = from EmpQ in this.Context.Employees
                           where EmpQ.Id == employeeId

                           //leveraging the data structure. This has nothing to do with unassigned employees
                           select new UnassignedEmployees()
                           {
                               EmployeeID = EmpQ.Id,
                               FirstName = EmpQ.FirstName,
                               LastName = EmpQ.LastName,
                               Department = EmpQ.Department,
                               UserID = EmpQ.UserID
                           };

            //Move the EmpQuery into a memory variable for speed and reuse
            List<UnassignedEmployees> UnAssEmp = EmpQuery.ToList();

            // Documentation states that one should not join a db object with an in memory object
            // try pulling employees out of the db first for both sides of the join
            var query1 = from TimeFcst in TimeFcstQuery2
                         where TimeFcst.EmployeeID == currentEmployee.Id
                         group TimeFcst by TimeFcst.EmployeeID into TF
                         //from TFs in TF.DefaultIfEmpty()
                         select new
                         {
                             EmployeeID = currentEmployee.Id,
                             TimePeriod = 1,
                             PeriodEnd = periodEnd,
                             WeekDays = weekDays,
                             WorkHours = weekDays * 8,
                             UserId = currentEmployee.UserID,
                             ChargeableHrs = TF.Sum(o => o.Period1ChargeableHrs),
                             NonChargeableHrs = TF.Sum(o => o.Period1NonChargeableHrs),
                             ExceptionHrs = TF.Sum(o => o.Period1ExceptionHrs),
                             LastName = currentEmployee.LastName,
                             FirstName = currentEmployee.FirstName,
                             Title = currentEmployee.Title,
                             Department = currentEmployee.Department,
                             Region = currentEmployee.Region,
                             Country = currentEmployee.Country.Name
                         };


            var query2 = new List<Forecast>();
            foreach (var q1 in query1)
            {
                var eh = algo; //this.GetExceptionHours(q1.EmployeeID, periodStart, periodEnd, fullExceptionList);
               
                Forecast fc = new Forecast();
                fc.Availability = -100;
                
                fc.ChargeableHours = (q1.ChargeableHrs >= hoursInPeriod) ? q1.ChargeableHrs - eh : q1.ChargeableHrs;
                fc.LastName = q1.LastName;
                fc.FirstName = q1.FirstName;
                fc.PeriodEnd = q1.PeriodEnd;
                fc.EmployeeID = q1.EmployeeID;
                fc.Utilization = (fc.ChargeableHours > 0) ? (100 * fc.ChargeableHours) / (hoursInPeriod - q1.ExceptionHrs) : 0;
                fc.NonChargeableHours = q1.NonChargeableHrs;
                fc.ExceptionHours = eh;
                fc.Availability = hoursInPeriod - q1.NonChargeableHrs - q1.ExceptionHrs - q1.ChargeableHrs;
                fc.WorkHours = (int)(hoursInPeriod - q1.ExceptionHrs);
                fc.WeekDays = weekDays;
                fc.HolidayHours = holidayCount * 8;
                if (fc.Availability < 0) fc.Availability = 0;
                fc.Title = q1.Title;
                fc.Department = q1.Department;
                fc.Region = q1.Region;
                fc.Country = q1.Country;
                query2.Add(fc);
            }

            decimal exceptionHours = algo;//this.GetExceptionHours(currentEmployee.Id, periodStart, periodEnd, fullExceptionList);

            Forecast resultForecast = new Forecast() 
            {
                Availability = hoursInPeriod - exceptionHours,
                ChargeableHours = 0,
                Department = currentEmployee.Department,
                EmployeeID = currentEmployee.Id,
                ExceptionHours = exceptionHours,
                FirstName = currentEmployee.FirstName,
                HolidayHours = holidayCount * 8,
                LastName = currentEmployee.LastName,
                NonChargeableHours = 0,
                PeriodEnd = periodEnd,
                Region = currentEmployee.Region,
                Title = currentEmployee.Title,
                Utilization = 0,
                UserId = currentEmployee.UserID,
                WorkHours = (int)(hoursInPeriod - exceptionHours),
                WeekDays = weekDays,
                Country = currentEmployee.Country.Name
            };

            if (query2.Count > 0) resultForecast = query2.First();

            return resultForecast;
        }
        #endregion

        #region SpecificForecast

        //The search paramater is the integer employee id, not the string userid
        /// <summary>
        /// Gets a collection of consecutive forecasts starting from the current time period
        /// </summary>
        /// <param name="employeeId">The field that identifies de record (not the string UserID)</param>
        /// <returns></returns>
        public IQueryable<Forecast> GetSpecificForecast(int? employeeId)
        {

            //Max number of time periods to return
            const int MAX_TIMEPERIODS = 6;

            // Declare a list to hold the returned forecast data
            var futureForecasts = new List<Forecast>();

            //The period that today is in
            DateTime currentTimePeriod = DateTime.Today;

            //Define a record id variable as a key (vs Employee ID) to allow for the return of multiple time period forecasts
            //for the same employee.
            int recordId = 0;

            for (int timePeriodIndex = 0; timePeriodIndex < MAX_TIMEPERIODS; timePeriodIndex++)
            {

                DateTime periodStart = TimePeriod.PeriodStartDate(currentTimePeriod);
                DateTime periodEnd = TimePeriod.PeriodEndDate(currentTimePeriod);

                Forecast periodForecast = this.GetSingleForecast((int)employeeId, periodStart, periodEnd);

                periodForecast.RecID = recordId;
                recordId++;
                futureForecasts.Add(periodForecast);

                //Move to the next time period
                currentTimePeriod = TimePeriod.PeriodEndDate(periodEnd.AddDays(1));
            }

            return futureForecasts.AsQueryable<Forecast>();
        }


        #endregion

        #region PeriodForecast

        /// <summary>
        /// Returns a list of forecasts for each employee in a given period
        /// </summary>
        private List<Forecast> GetPeriodEmployeesForecast(DateTime inputDate, List<Employee> empList)
        {
            DateTime periodStart = TimePeriod.PeriodStartDate(inputDate);
            DateTime periodEnd = TimePeriod.PeriodEndDate(inputDate);

            decimal periodHours = this.GetPeriodHours(periodStart, periodEnd);

            var employeesQuery = from employee in this.Context.Employees.Include("Country")
                                 where employee.ExcludeFromAvailabilityReport != true
                                 select employee;

            //Just to simulate a primary key
            int recId = 0;

            List<Forecast> forecasts = new List<Forecast>();
            List<Employee> employeeList = empList;//employeesQuery.ToList();

            List<TimeForecast> periodForecasts = (from forecast in this.Context.TimeForecasts.Include("Employee")
                                                  where
                                                        forecast.Employee.ExcludeFromAvailabilityReport != true
                                                      && ((forecast.StartDate >= periodStart && forecast.StartDate <= periodEnd)
                                                      || (forecast.EndDate >= periodStart && forecast.EndDate <= periodEnd)
                                                      || (periodStart >= forecast.StartDate && periodEnd <= forecast.EndDate))
                                                      
                                                  select forecast
                                                 ).ToList();

            Dictionary<int, decimal> countryHolidayHoursView = new Dictionary<int, decimal>();
            foreach (var country in this.Context.Countries) 
            {
                decimal countryHolidayHours = this.GetHolidayCount(periodStart,periodEnd,country) * 8;
                countryHolidayHoursView.Add(country.Id, countryHolidayHours);
            }

            var exceptionList = this.Context.TimeForecastExceptionsSet.ToList();
            
            foreach (var employee in employeeList) 
            {

                decimal exceptionHours = this.GetExceptionHours(employee.Id, periodStart, periodEnd, exceptionList);
                int holidayHours = (int)countryHolidayHoursView.First(x => x.Key == employee.Country.Id).Value;
                int workHours = (int)(periodHours - exceptionHours - holidayHours);

                var empForecasts = from forecast in periodForecasts
                           where forecast.Employee.Id == employee.Id
                           let startDate = (forecast.StartDate > periodStart) ? forecast.StartDate : periodStart
                           let endDate = (forecast.EndDate > periodEnd) ? periodEnd : forecast.EndDate
                           let hours = (TimePeriod.WeekdaysInPeriod(startDate, endDate) * 8) * forecast.Percent / 100
                           select new { forecast, hours };

                decimal chargeableHours = (empForecasts.Where(x => x.forecast.Chargeable == true).Any()) ? empForecasts.Where(x => x.forecast.Chargeable == true).Sum(x => x.hours) : 0;
                decimal nonChargeableHours = (empForecasts.Where(x => x.forecast.Chargeable == false).Any()) ? empForecasts.Where(x => x.forecast.Chargeable == false).Sum(x => x.hours) : 0;

                decimal utilization = (workHours > 0) ? (chargeableHours / workHours) * 100 : 0;
                decimal availability = (workHours > 0) ? workHours - nonChargeableHours - chargeableHours : 0;

                Forecast periodForecast = new Forecast();
                periodForecast.EmployeeID = employee.Id;
                periodForecast.LastName = employee.LastName;
                periodForecast.FirstName = employee.FirstName;
                periodForecast.Title = employee.Title;
                periodForecast.Country = employee.Country.Name;
                periodForecast.PeriodEnd = periodEnd;
                periodForecast.ExceptionHours = exceptionHours;
                periodForecast.HolidayHours = holidayHours;
                periodForecast.PeriodHours = (int)periodHours;
                periodForecast.WorkHours = workHours;
                periodForecast.ChargeableHours = chargeableHours;
                periodForecast.NonChargeableHours = nonChargeableHours;
                periodForecast.Utilization = (utilization <= 100) ? utilization : 100;
                periodForecast.Availability = (availability >= 0) ? availability : 0;

                periodForecast.Department = employee.Department;
                periodForecast.Region = employee.Region;
                periodForecast.Country = employee.Country.Name;

                periodForecast.RecID = recId;
                forecasts.Add(periodForecast);
                recId++;
            }

            return forecasts.OrderBy(o => o.LastName).ToList();
            
        }
        #endregion

        private decimal GetAvailability(Employee e, DateTime inputDate) 
        {
            //Workhours - Chargeable hours - Non chargeable - Ex

            DateTime periodStart = TimePeriod.PeriodStartDate(inputDate);
            DateTime periodEnd = TimePeriod.PeriodEndDate(inputDate);

            decimal workhours = this.GetWorkHours(e, periodStart, periodEnd);
            decimal chargeableHours = this.GetChargeableHours(e, periodStart, periodEnd);
            decimal nonChargeableHours = this.GetNonChargeableHours(e, periodStart, periodEnd);
            decimal exceptionHours = this.GetExceptionHours(e, periodStart, periodEnd);


            return workhours - chargeableHours - nonChargeableHours;
        }

        private decimal GetUtilization(Employee e, DateTime inputDate) 
        {
            DateTime periodStart = TimePeriod.PeriodStartDate(inputDate);
            DateTime periodEnd = TimePeriod.PeriodEndDate(inputDate);

            int periodHours = TimePeriod.WeekdaysInPeriod(periodStart, periodEnd) * 8;
            int holidayHours = this.GetHolidayCount(periodEnd, periodStart, e.Country) * 8;

            List<TimeForecastExceptions> exList = new List<TimeForecastExceptions>();

            var exceptions = from exception in this.Context.TimeForecastExceptionsSet
                             select exception;

            decimal exceptionHours = this.GetExceptionDays(periodStart, periodEnd, exceptions.ToList(), e.Id) * 8;
            decimal chargeableHours = this.GetChargeableHours(e,periodStart,periodEnd);
            decimal workHours = periodHours - holidayHours - exceptionHours;

            return (chargeableHours / workHours) * 100;
        }

        #region MultiPeriodForecasts
        [Query(IsDefault = true)]
        public IQueryable<MultiPeriodForecast> GetAllMultiPeriodForecasts()
        {
            DateTime tdy = DateTime.Today;

            List<Employee> empList = this.Context.Employees.Where(e => e.ExcludeFromAvailabilityReport == false).ToList();

            List<MultiPeriodForecast> MultiPeriodForecast = new List<MultiPeriodForecast>();
            List<Forecast> SinglePeriodForecast = GetPeriodEmployeesForecast(tdy, empList);

            List<List<Forecast>> periods = new List<List<Forecast>>();

            //Getting the next X forecasts starting from the current one
            const int MAX_NEXTPERIODS = 5;

            DateTime NextTimePeriod = TimePeriod.PeriodEndDate(tdy).AddDays(1);
            for (int periodIndex = 0; periodIndex < MAX_NEXTPERIODS; periodIndex++)
            {
                List<Forecast> SinglePeriodForecast1 = GetPeriodEmployeesForecast(NextTimePeriod, empList);
                periods.Add(SinglePeriodForecast1);
                NextTimePeriod = TimePeriod.PeriodEndDate(NextTimePeriod).AddDays(1);
            }
            

            //"Header" for the multiperiodforecast
            foreach (Forecast spf in SinglePeriodForecast)
            {
                MultiPeriodForecast mpf = new MultiPeriodForecast();
                mpf.RecID = spf.RecID;
                mpf.EmployeeID = spf.EmployeeID;
                mpf.TimePeriod = spf.TimePeriod;
                mpf.PeriodEnd = spf.PeriodEnd;
                mpf.WeekDays = spf.WeekDays;
                mpf.PeriodHours = spf.PeriodHours;
                mpf.WorkHours = spf.WorkHours;
                mpf.HolidayHours = spf.HolidayHours;
                mpf.ChargeableHours = spf.ChargeableHours;
                mpf.NonChargeableHours = spf.NonChargeableHours;
                mpf.ExceptionHours = spf.ExceptionHours;
                mpf.Utilization = spf.Utilization;
                mpf.Availability = spf.Availability;

                var spfs = periods[0].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization1 = spfs.Utilization;
                mpf.Availability1 = spfs.Availability;

                spfs = periods[1].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization2 = spfs.Utilization;
                mpf.Availability2 = spfs.Availability;

                spfs = periods[2].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization3 = spfs.Utilization;
                mpf.Availability3 = spfs.Availability;

                spfs = periods[3].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization4 = spfs.Utilization;
                mpf.Availability4 = spfs.Availability;

                spfs = periods[4].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization5 = spfs.Utilization;
                mpf.Availability5 = spfs.Availability;

                mpf.UserId = spf.UserId;
                mpf.LastName = spf.LastName;
                mpf.FirstName = spf.FirstName;
                mpf.Title = spf.Title;
                mpf.Department = spf.Department;
                mpf.Region = spf.Region;
                mpf.Country = spf.Country;

                MultiPeriodForecast.Add(mpf);
            }

            return MultiPeriodForecast.AsQueryable<MultiPeriodForecast>();
        }
        #endregion

        private IQueryable<MultiPeriodForecast> GetMultiPeriodForecastsByEmployees(List<Employee> list)
        {
            DateTime tdy = DateTime.Today;

            List<Employee> empList = list;

            List<MultiPeriodForecast> MultiPeriodForecast = new List<MultiPeriodForecast>();
            List<Forecast> SinglePeriodForecast = GetPeriodEmployeesForecast(tdy, empList);

            List<List<Forecast>> periods = new List<List<Forecast>>();

            //Getting the next X forecasts starting from the current one
            const int MAX_NEXTPERIODS = 5;

            DateTime NextTimePeriod = TimePeriod.PeriodEndDate(tdy).AddDays(1);
            for (int periodIndex = 0; periodIndex < MAX_NEXTPERIODS; periodIndex++)
            {
                List<Forecast> SinglePeriodForecast1 = GetPeriodEmployeesForecast(NextTimePeriod, empList);
                periods.Add(SinglePeriodForecast1);
                NextTimePeriod = TimePeriod.PeriodEndDate(NextTimePeriod).AddDays(1);
            }


            //"Header" for the multiperiodforecast
            foreach (Forecast spf in SinglePeriodForecast)
            {
                MultiPeriodForecast mpf = new MultiPeriodForecast();
                mpf.RecID = spf.RecID;
                mpf.EmployeeID = spf.EmployeeID;
                mpf.TimePeriod = spf.TimePeriod;
                mpf.PeriodEnd = spf.PeriodEnd;
                mpf.WeekDays = spf.WeekDays;
                mpf.PeriodHours = spf.PeriodHours;
                mpf.WorkHours = spf.WorkHours;
                mpf.HolidayHours = spf.HolidayHours;
                mpf.ChargeableHours = spf.ChargeableHours;
                mpf.NonChargeableHours = spf.NonChargeableHours;
                mpf.ExceptionHours = spf.ExceptionHours;
                mpf.Utilization = spf.Utilization;
                mpf.Availability = spf.Availability;

                var spfs = periods[0].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization1 = spfs.Utilization;
                mpf.Availability1 = spfs.Availability;

                spfs = periods[1].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization2 = spfs.Utilization;
                mpf.Availability2 = spfs.Availability;

                spfs = periods[2].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization3 = spfs.Utilization;
                mpf.Availability3 = spfs.Availability;

                spfs = periods[3].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization4 = spfs.Utilization;
                mpf.Availability4 = spfs.Availability;

                spfs = periods[4].Where(x => x.EmployeeID == spf.EmployeeID).First();
                mpf.Utilization5 = spfs.Utilization;
                mpf.Availability5 = spfs.Availability;

                mpf.UserId = spf.UserId;
                mpf.LastName = spf.LastName;
                mpf.FirstName = spf.FirstName;
                mpf.Title = spf.Title;
                mpf.Department = spf.Department;
                mpf.Region = spf.Region;
                mpf.Country = spf.Country;

                MultiPeriodForecast.Add(mpf);
            }

            return MultiPeriodForecast.AsQueryable<MultiPeriodForecast>();
        }

        public IQueryable<MultiPeriodForecast> GetMultiperiodForecasts(int? skillId, string level)
        {
            var selectedSkill = this.Context.EmployeeSkills.Include("Employee.Country")
                                .Where(x => x.Level == level && x.Skill.Id == skillId)
                                .Select(x => x.Employee)
                                .ToList();

            var multiperiods = this.GetMultiPeriodForecastsByEmployees(selectedSkill);

            var skilled = from mp in multiperiods
                          join ss in selectedSkill on mp.EmployeeID equals ss.Id
                          select mp;

            return skilled.AsQueryable();
                                    
        }

        #region AllForecasts
        [Query(IsDefault = true)]
        public IQueryable<Forecast> GetAllForecasts()
        {
            DateTime tdy = DateTime.Today;
            List<Employee> list = this.Context.Employees.Where(e => e.ExcludeFromAvailabilityReport == false).ToList();
            return GetPeriodEmployeesForecast(tdy, list).AsQueryable<Forecast>();
        }
        #endregion


       

    }
}
