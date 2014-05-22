using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public static class TimePeriod
    {
        /// <summary>
        /// Gets the first day of the month.
        /// </summary>
        /// <param name="givenDate">The given date.</param>
        /// <returns>the first day of the month</returns>
        public static DateTime GetFirstDayOfMonth(DateTime givenDate)
        {
            return new DateTime(givenDate.Year, givenDate.Month, 1);
        }

        /// <summary>
        /// Gets the last day of month.
        /// </summary>
        /// <param name="givenDate">The given date.</param>
        /// <returns>the last day of the month</returns>
        public static DateTime GetTheLastDayOfMonth(DateTime givenDate)
        {
            return GetFirstDayOfMonth(givenDate).AddMonths(1).Subtract(new TimeSpan(1, 0, 0, 0, 0));
        }

        /// <summary>
        /// Gets the start date of the time period for a given date.
        /// </summary>
        /// <param name="givenDate">The given date.</param>
        /// <returns>the start date of the time period</returns>
        public static DateTime PeriodStartDate(DateTime givenDate)
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
        public static DateTime PeriodEndDate(DateTime givenDate)
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

        /// <summary>
        /// Calculates the number of weekdays, Monday through Friday, between two dates inclusive.
        /// </summary>
        /// <param name="PeriodStart">The start date.</param>
        ///  /// <param name="PeriodEnd">The end date.</param>
        /// <returns>the number of weekdays between two dates (inclusive) </returns>
        public static int WeekdaysInPeriod(DateTime PeriodStart, DateTime PeriodEnd)
        {
            TimeSpan ts = new TimeSpan(1, 0, 0, 0, 0);
            int weekday = 0;
            int DaysInPeriod = (PeriodEnd.Date - PeriodStart.Date).Days + 1;
            int i;
            for (i = 0; i < DaysInPeriod; i++)
            {
                DateTimeOffset dayInPeriod = PeriodStart.AddDays(i);
                //Sunday is DayOfWeek 0, Saturday is DayOfWeek 6
                int dw = (int)dayInPeriod.DayOfWeek;
                if (0 < dw && dw < 6)
                {
                    weekday++;
                }
            }
            return weekday;
        }

        /// <summary>
        /// Gets the quarter that a given date is in
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Integer with the quarter number</returns>
        public static int GetQuarter(DateTime date) 
        {
            int month = date.Month;

            if (date.Month >= 1 && month <= 3) return 1;
            else if (date.Month >= 4 && month <= 6) return 2;
            else if (date.Month >= 7 && month <= 9) return 3;
            else if (date.Month >= 9 && month <= 12) return 4;
            else throw new InvalidOperationException("Month has an invalid value. Must be an integer from 1 to 12");
        }

    }

}
