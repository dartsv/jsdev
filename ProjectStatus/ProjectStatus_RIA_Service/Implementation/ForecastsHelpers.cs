using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        /// <summary>
        /// Get the total business days hours in a certain time period
        /// </summary>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <returns></returns>
        private decimal GetPeriodHours(DateTime periodStart, DateTime periodEnd)
        {
            int weekdays = TimePeriod.WeekdaysInPeriod(periodStart, periodEnd);
            return weekdays * 8;
        }

        /// <summary>
        /// Get the holidays hours for a certain employee in a given time period
        /// </summary>
        /// <param name="e"></param>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <returns></returns>
        private decimal GetHolidayHours(Employee e, DateTime periodStart, DateTime periodEnd)
        {
            int holidayCount = this.GetHolidayCount(periodStart, periodEnd, e.Country);
            return holidayCount * 8;
        }

        /// <summary>
        /// Get the exception hours for a certain employee in a given period
        /// </summary>
        /// <param name="e"></param>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <returns></returns>
        private decimal GetExceptionHours(Employee e, DateTime periodStart, DateTime periodEnd)
        {
            var exceptions = from exception in this.Context.TimeForecastExceptionsSet
                             select exception;
            List<TimeForecastExceptions> exceptionList = exceptions.ToList();

            decimal exceptionDays = this.GetExceptionDays(periodStart, periodEnd, exceptionList, e.Id);
            return exceptionDays * 8;
        }

        /// <summary>
        /// Get the actual work hours available for an employee in a given time period
        /// </summary>
        /// <param name="e"></param>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <returns></returns>
        private decimal GetWorkHours(Employee e, DateTime periodStart, DateTime periodEnd)
        {
            decimal periodHours = this.GetPeriodHours(periodStart, periodEnd);
            decimal holidayHours = this.GetHolidayHours(e, periodStart, periodEnd);
            decimal exceptionHours = this.GetExceptionHours(e, periodStart, periodEnd);

            return periodHours - holidayHours - exceptionHours;
        }


        #region HolidayCountByCountry

        ///<summary>
        /// Get the number of holidays for a country in a certain period of time 
        ///</summary>

        private int GetHolidayCount(DateTime periodStart, DateTime periodEnd, Country employeeCountry)
        {
            int holidayCount = 0;

            var countryHolidays = (from c in this.Context.Countries
                           join ch in this.Context.CountryHolidays
                           on c.Id equals ch.Country.Id
                           where c.Id == employeeCountry.Id
                           select ch).ToList();

            foreach (CountryHoliday ch in countryHolidays)
            {
                bool noEndDate = (ch.EndDate == null);
                bool areSameDate = (ch.StartDate == ch.EndDate);

                bool isOneDay = noEndDate || areSameDate;

                //Validate that holiday dates are correct
                if (ch.StartDate == null) throw new Exception("A date for \"" + ch.Country.Name + "'s " + ch.Holiday.HolidayName + "\" was not defined");
                if (ch.StartDate > ch.EndDate) throw new Exception("The date range for \"" + ch.Country.Name + "'s " + ch.Holiday.HolidayName + "\" is not correct");

                if (isOneDay)
                {
                    if (ch.StartDate >= periodStart && ch.StartDate <= periodEnd)
                        if (ch.StartDate.DayOfWeek != DayOfWeek.Saturday && ch.StartDate.DayOfWeek != DayOfWeek.Sunday) 
                            holidayCount++;
                }
                else
                {
                    DateTime holidayEndDate = (DateTime)ch.EndDate;
                    DateTime usedEndDate = (holidayEndDate > periodEnd) ? periodEnd : holidayEndDate;
                    DateTime usedStartDate = (ch.StartDate < periodStart) ? periodStart : ch.StartDate;

                    int holidayWeekdays = TimePeriod.WeekdaysInPeriod(usedStartDate, usedEndDate);
                    holidayCount += holidayWeekdays;
                }

            }

            return holidayCount;
        }

        private int GetHolidayCount(DateTime periodStart, DateTime periodEnd, Country employeeCountry, List<CountryHoliday> countriesHolidays)
        {
            int holidayCount = 0;

            var countryHolidays = countriesHolidays;

            foreach (CountryHoliday ch in countryHolidays)
            {
                bool noEndDate = (ch.EndDate == null);
                bool areSameDate = (ch.StartDate == ch.EndDate);

                bool isOneDay = noEndDate || areSameDate;

                //Validate that holiday dates are correct
                if (ch.StartDate == null) throw new Exception("A date for \"" + ch.Country.Name + "'s " + ch.Holiday.HolidayName + "\" was not defined");
                if (ch.StartDate > ch.EndDate) throw new Exception("The date range for \"" + ch.Country.Name + "'s " + ch.Holiday.HolidayName + "\" is not correct");

                if (isOneDay)
                {
                    if (ch.StartDate >= periodStart && ch.StartDate <= periodEnd)
                        if (ch.StartDate.DayOfWeek != DayOfWeek.Saturday && ch.StartDate.DayOfWeek != DayOfWeek.Sunday)
                            holidayCount++;
                }
                else
                {
                    DateTime holidayEndDate = (DateTime)ch.EndDate;
                    DateTime usedEndDate = (holidayEndDate > periodEnd) ? periodEnd : holidayEndDate;
                    DateTime usedStartDate = (ch.StartDate < periodStart) ? periodStart : ch.StartDate;

                    int holidayWeekdays = TimePeriod.WeekdaysInPeriod(usedStartDate, usedEndDate);
                    holidayCount += holidayWeekdays;
                }

            }

            return holidayCount;
        }

        #endregion

        #region ExceptionDays

        ///<summary>
        /// Return the decimal days of exceptions in the forecast (based on an 8 hour workday).
        ///</summary>


        private decimal GetExceptionDays(DateTime periodStart, DateTime periodEnd, List<TimeForecastExceptions> ExceptionList, int employeeId)
        {
            //TODO: strip out overlapping records
            //Exception List must be ordered by Employee, Start Date and End date

            decimal ed = 0;
            decimal ExceptDays = 0;
            DateTime lateStart, earlyEnd;

            var employeeQuery = from employee in this.Context.Employees
                                join country in this.Context.Countries
                                on employee.Country.Id equals country.Id
                                where employee.Id == employeeId
                                select new { employee, country };

            var currentEmployee = employeeQuery.First();

            Country employeeCountry = currentEmployee.country;

            foreach (TimeForecastExceptions tfe in ExceptionList)
            {
                if (employeeId == tfe.TimeForecastExceptions_Employee)
                {
                    lateStart = (tfe.StartDate >= periodStart) ? tfe.StartDate : periodStart;
                    earlyEnd = (tfe.EndDate <= periodEnd) ? tfe.EndDate : periodEnd;
                    if (lateStart <= earlyEnd)
                    {
                        ed = TimePeriod.WeekdaysInPeriod(lateStart, earlyEnd);
                        //ed = ed - HolidayCount(LateStart, EarlyEnd, HolidayList);
                        ed = ed - this.GetHolidayCount(lateStart, earlyEnd, employeeCountry);
                        ed *= tfe.Percent / (decimal)100.0;
                    }
                    ExceptDays += ed;
                }
            }
            return ExceptDays;
        }

        #endregion

        private decimal GetChargeableHours(Employee e, DateTime periodStart, DateTime periodEnd) 
        {
            decimal workHours = this.GetWorkHours(e,periodStart,periodEnd);
            
            var chargeableForecasts = from forecasts in this.Context.TimeForecasts
                                      where
                                            forecasts.Chargeable == true
                                         && forecasts.Employee.Id == e.Id
                                         && forecasts.EndDate >= periodStart
                                         && forecasts.StartDate <= periodEnd

                                      let chargeableHours = ((forecasts.EndDate >= periodEnd) ? workHours : TimePeriod.WeekdaysInPeriod(periodStart, forecasts.EndDate) * 8) * forecasts.Percent / 100

                                      select new { forecasts, chargeableHours };

            decimal totalChargeableHours = (chargeableForecasts.Count() == 0) ? 0 : chargeableForecasts.Sum(x => x.chargeableHours);

            return totalChargeableHours;
        }

        private decimal GetUsedHours(int employeeId, DateTime periodStart, DateTime periodEnd, List<TimeForecast> timeforecasts, bool isChargeable) 
        {
            var allForecasts = (from forecast in timeforecasts
                                      where
                                            forecast.Chargeable == isChargeable
                                         && forecast.Employee.Id == employeeId
                                         && ((forecast.StartDate >= periodStart && forecast.StartDate <= periodEnd)
                                                || (forecast.EndDate >= periodStart && forecast.EndDate <= periodEnd))
                                    let startDate = (forecast.StartDate > periodStart) ? forecast.StartDate : periodStart
                                    let endDate = (forecast.EndDate > periodEnd) ? periodEnd : forecast.EndDate
                                    let totalHours = (TimePeriod.WeekdaysInPeriod(startDate, endDate) * 8) * forecast.Percent / 100 
                                    select new {forecast, totalHours});



            return (allForecasts.Any()) ? allForecasts.Sum(x => x.totalHours) : 0; 
        }

        private decimal GetChargeableHours(int employeeId, DateTime periodStart, DateTime periodEnd, List<TimeForecast> timeforecasts)
        {
            if (timeforecasts.Any())
            {
                var allForecasts = (from forecast in timeforecasts
                                    where
                                          forecast.Chargeable == true
                                       && forecast.Employee.Id == employeeId
                                       && ((forecast.StartDate >= periodStart && forecast.StartDate <= periodEnd)
                                              || (forecast.EndDate >= periodStart && forecast.EndDate <= periodEnd
                                    ))
                                    select forecast);

                /*let startDate = (forecast.StartDate > periodStart) ? forecast.StartDate : periodStart
                let endDate = (forecast.EndDate > periodEnd) ? periodEnd : forecast.EndDate
                let hours = (TimePeriod.WeekdaysInPeriod(startDate, endDate) * 8) * forecast.Percent / 100
                select new { forecast, hours });*/

                decimal totalHours = 0;

                allForecasts.Count();

                return totalHours;
            }
            else return 0;
            //return this.GetUsedHours(employeeId, periodStart, periodEnd, timeforecasts, workHours, true);
        }

        private decimal GetNonChargeableHours(int employeeId, DateTime periodStart, DateTime periodEnd, List<TimeForecast> timeforecasts)
        {
            return 0;
            //return this.GetUsedHours(employeeId, periodStart, periodEnd, timeforecasts, false);
        }
        
        private decimal GetNonChargeableHours(Employee e, DateTime periodStart, DateTime periodEnd) 
        {
            decimal workHours = this.GetWorkHours(e,periodStart,periodEnd);
            
            var chargeableForecasts = from forecasts in this.Context.TimeForecasts
                                      where
                                            forecasts.Chargeable == false
                                         && forecasts.Employee.Id == e.Id
                                         && forecasts.EndDate >= periodStart
                                         && forecasts.StartDate <= periodEnd

                                      let nonChargeableHours = ((forecasts.EndDate >= periodEnd) ? workHours : TimePeriod.WeekdaysInPeriod(periodStart, forecasts.EndDate) * 8) * forecasts.Percent / 100

                                      select new { forecasts, nonChargeableHours };

            decimal totalChargeableHours = (chargeableForecasts.Count() == 0) ? 0 : chargeableForecasts.Sum(x => x.nonChargeableHours);

            return totalChargeableHours;
        }


        private decimal GetExceptionHours(int employeeId, DateTime periodStart, DateTime periodEnd, List<TimeForecastExceptions> exceptionList)
        {
            
            Employee employee = this.Context.Employees.Include("Country").FirstOrDefault(e => e.Id == employeeId);

            var countriesHolidays = (from c in this.Context.Countries
                                     join ch in this.Context.CountryHolidays
                                     on c.Id equals ch.Country.Id
                                     where c.Id == employee.Country.Id
                                     select ch).ToList();

            var empExceptions = (from forecast in exceptionList
                                 let startDate = (forecast.StartDate > periodStart) ? forecast.StartDate : periodStart
                                 let endDate = (forecast.EndDate > periodEnd) ? periodEnd : forecast.EndDate
                                 let totalHours = (TimePeriod.WeekdaysInPeriod(startDate, endDate) * 8) * forecast.Percent / 100
                                 let holidayHours = this.GetHolidayCount(startDate, endDate, employee.Country, countriesHolidays) * 8
                                where forecast.TimeForecastExceptions_Employee == employeeId
                                && forecast.EndDate >= startDate
                                       && forecast.StartDate <= endDate
                                   //&& ((forecast.StartDate >= periodStart && forecast.StartDate <= periodEnd)
                                          //|| (forecast.EndDate >= periodStart && forecast.EndDate <= periodEnd))
                                
                                select new { forecast, totalHours = totalHours - holidayHours });

            //Substract any holiday hours that are inside the PTO requested period
            return (empExceptions.Any()) ? empExceptions.Sum(x => x.totalHours) : 0; 
        }
    }
}
