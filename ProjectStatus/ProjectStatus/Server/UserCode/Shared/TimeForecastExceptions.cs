using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.LightSwitch;
namespace LightSwitchApplication
{
    public partial class TimeForecastExceptions
    {
        
        partial void StartDate_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void EndDate_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Percent_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Employee_Changed()
        {
            LastUpdate = DateTime.Now;
        }

        partial void Project_Changed()
        {
            //if (StartDate == DateTime.Today && EndDate == DateTime.Today)
            if (StartDate == DateTime.MinValue)
            {
                StartDate = DateTime.Today;
                //Chargeable = Project1.Chargeable;
                LastUpdate = DateTime.Now;
               
                    //for forecast exceptions, such as PTO, set the end date = to the start date
                    EndDate = StartDate;
            }
        }

        partial void TimeForecastExceptions_Created()
        {
            Percent = 100;
            //Chargeable = true;
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
        }

        

        partial void StartDate_Validate(EntityValidationResultsBuilder results)
        {
            if (EndDate < StartDate)
            {
                results.AddPropertyError("Start date must be less than or equal to the end date.");
            }
            if (Project != null)
            {
                if (StartDate < Project.StartDate)
                {
                    results.AddPropertyError("Start Date cannot be before Project Start Date of " + Project.StartDate.Date.ToString());
                }
            }
            if (isOverlap(StartDate, EndDate, Id))
            {
                results.AddPropertyError("Exceptions cannot overlap and exceed 100%");
            }
               
        }

        partial void EndDate_Validate(EntityValidationResultsBuilder results)
        {
            if (EndDate < StartDate)
            {
                results.AddPropertyError("End date must be greater than or equal to the start date.");
            }
            if (Project != null)
            {
                if (EndDate > Project.BookedEndDate)
                {
                    results.AddPropertyError("End date can not be after the Project Booked End Date of " + Project.BookedEndDate.Date.ToString());
                }
            }
            if (isOverlap(StartDate, EndDate, Id))
            {
                results.AddPropertyError("Exceptions cannot overlap and exceed 100%");
            }
        }

        private Boolean isOverlap(DateTime StartDate, DateTime StopDate, int RecordID)
        {

            if (Employee == null) return false;

            // get current exceptions stored for this user
            var StoredTFE = (from objTFE in this.DataWorkspace.ApplicationData.TimeForecastExceptionsSet
                             where objTFE.Employee.Id == Employee.Id
                             select objTFE);

            List<TimeForecastExceptions> TFEList = new List<TimeForecastExceptions>();
            foreach(TimeForecastExceptions TFE in StoredTFE)
            {
                TFEList.Add(TFE);
            }

            //get change set
            EntityChangeSet ChangeSet = this.DataWorkspace.ApplicationData.Details.GetChanges();
           
            // remove deleted records
            foreach (IEntityObject Entity in ChangeSet.DeletedEntities.OfType<TimeForecastExceptions>())
            {
                TimeForecastExceptions TFE;
                TFE = (TimeForecastExceptions)Entity;
                TFEList.Remove(TFE);
            }


            //replace updated records
            foreach (IEntityObject Entity in ChangeSet.ModifiedEntities.OfType<TimeForecastExceptions>())
            {
                TimeForecastExceptions TFE;
                TFE = (TimeForecastExceptions)Entity;
                TFEList.Remove(TFE);
                TFEList.Add(TFE);
            }

           
            //add new records to StoredFTE
            foreach (IEntityObject Entity in ChangeSet.AddedEntities.OfType<TimeForecastExceptions>())
            {
                TFEList.Add((TimeForecastExceptions)Entity);
            }

            //now check each day to make sure that no day exceeds 100% exceptions
            int NumberOfDaysToValidate = (EndDate - StartDate).Days + 1;
            if (NumberOfDaysToValidate < 1) return true;
            int[] ExceptionPercentByDay = new int[NumberOfDaysToValidate];
            foreach (TimeForecastExceptions TFE in TFEList)
            {
                if (!(TFE.StartDate > EndDate | TFE.EndDate < StartDate))
                {
                    //int startD = (TFE.StartDate-StartDate).Days;
                    //StartD = (PS > TimeFcstExcept.StartDate) ? PS : TimeFcstExcept.StartDate
                    //int endD = (TFE.EndDate - StartDate).Days;

                    int startD = (TFE.StartDate > StartDate) ? (TFE.StartDate - StartDate).Days : 0;
                    int endD = (TFE.EndDate < EndDate) ? (TFE.EndDate - StartDate).Days : NumberOfDaysToValidate - 1;
                    for (int d = startD; d <= endD; d++)
                    {
                        //int dip = DayInPeriod(d);
                        ExceptionPercentByDay[d] += TFE.Percent;
                        if (ExceptionPercentByDay[d] > 100) return true;
                    }
                }
            }
            //foreach (TimeForecastExceptions TFE in TFEList)
            //{
            //    if (!(TFE.StartDate > EndDate | TFE.EndDate < StartDate))
            //    {
            //        int startD = 0;
            //        int endD = (TFE.EndDate - StartDate).Days;
            //        for (int d = startD; d <= endD; d++)
            //        {
            //            //int dip = DayInPeriod(d);
            //            ExceptionPercentByDay[d] += TFE.Percent;
            //            if (ExceptionPercentByDay[d] > 100) return true;
            //        }
            //    }
            //}
            return false;
        }
        
        //Return the integer day in the period
        private int DayInPeriod(int DayOfMonth)
        {
            if (DayOfMonth <= 15)
            {
                return DayOfMonth - 1;
            }
            else
            {
                return DayOfMonth - 16;
            }
        }

        partial void Percent_Validate(EntityValidationResultsBuilder results)
        {
            if (Percent < 1 | Percent > 100)
            {
                results.AddPropertyError("Percent must be between 1 and 100.");
            }
        }
    }
}
