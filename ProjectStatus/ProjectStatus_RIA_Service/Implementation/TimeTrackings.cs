using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;
using CommonUtilities.Statuses;
using System.Reflection;
using System.IO;
using System.IO.IsolatedStorage;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        [Query(IsDefault = true)]
        public IQueryable<ProjectTimeLine> TimeTrackings()
        {
            return new List<ProjectTimeLine>().AsQueryable();
        }

        //Lines for PTO and other exceptions are still missing
        public IQueryable<ProjectTimeLine> TimeTrackReport(int? employeeId, DateTime? pFromDate, DateTime? pToDate)
        {
            //DateTimes to iterate over
            DateTime fromDate = (pFromDate == null) ? TimePeriod.PeriodStartDate(DateTime.Now) : ((DateTime)pFromDate).Date;
            DateTime toDate = (pToDate == null) ? TimePeriod.PeriodEndDate(DateTime.Now).AddDays(1).AddSeconds(-1) : ((DateTime)pToDate).Date.AddDays(1).AddSeconds(-1);
            int days = (toDate - fromDate).Days + 1;

            //Dictionary of project names
            var projs = (from proj in this.Context.Projects
                         select new { proj.Id, proj.ProjectName }).ToDictionary(p => p.Id, p => p.ProjectName);

            //Dictionary of project billing
            var projBilling = (from proj in this.Context.Projects
                               select new { proj.Id, proj.Chargeable }).ToDictionary(p => p.Id, p => p.Chargeable);

            //All Employee timetrackings-- JBurgos
            /*  var timeTrackings = this.Context.TimeTrackings
                             .Include("ProjectFeature")
                             .Include("ProjectIncident")
                             .Include("ProjectTask")
                             .Where(tt => tt.Employee.Id == employeeId)
                             .Where(tt => tt.ProjectFeature != null || tt.ProjectIncident != null || tt.ProjectTask != null)
                             .Where(tt => ((tt.StartTime >= fromDate && tt.StartTime <= toDate)
                                             || (tt.EndTime >= fromDate && tt.EndTime <= toDate)
                                             || (fromDate >= tt.StartTime && toDate <= tt.EndTime)))
                         .ToList();
             */


        
               var timeTrackings = this.Context.TimeTracks
                              .Include("Requirement")
                              .Include("Issue")
                              .Include("Requirement.ProjectFeature") //
                              .Include("Requirement.ProjectIncident")  //
                              .Include("Requirement.ProjectTask")  //
                              .Include("Issue.ProjectFeature")  //
                              .Include("Issue.ProjectIncident")  //
                              .Include("Issue.ProjectTask")  //
                              .Where(tt => tt.Employee.Id == employeeId)
                              .Where(tt => tt.TimeTrack_Requirement != null || tt.TimeTrack_Issue != null)
                             .Where(tt => ((tt.StartTime >= fromDate && tt.StartTime <= toDate)
                                              || (tt.EndTime >= fromDate && tt.EndTime <= toDate)
                                              || (fromDate >= tt.StartTime && toDate <= tt.EndTime)))
                          .ToList();


               var exceptions = this.Context.TimeForecastExceptionsSet
                                 .Where(tt => tt.TimeForecastExceptions_Employee == employeeId
                                     && ((tt.StartDate >= fromDate && tt.StartDate <= toDate)
                                                || (tt.EndDate >= fromDate && tt.EndDate <= toDate)
                                                || (fromDate >= tt.StartDate && toDate <= tt.EndDate))
                                      )
                                 .ToArray();
                
               //All projects worked on for the given period
            var exes = (from tt in exceptions
                        join pr in projs on tt.TimeForecastExceptions_Project equals pr.Key
                        select new { pr.Key, pr.Value })
                           .GroupBy(p => p.Key, p => p.Value, (key, g) => new { Key = key, Value = g.FirstOrDefault() })
                           .OrderBy(p => p.Key);

            //Get the time trackings by project------------JBurgos
            /* var timeTrackingsByProject = from row in timeTrackings
                                           select new
                                           {
                                               ProjectId = (row.ProjectFeature != null) ? row.ProjectFeature.ProjectFeature_Project
                                                        : (row.ProjectIncident != null) ? row.ProjectIncident.ProjectIncident_Project : row.ProjectTask.ProjectTask_Project,
                                               StartTime = row.StartTime,
                                               EndTime = row.EndTime,
                                               Chargeable = row.Chargeable
                                           };*/
           
                var timeTrackingsByProject = from row in timeTrackings
                                             select new
                                             {
                                                 
                                                 // Added new logic for timetracking by project 
                                                  ProjectId = (row.TimeTrack_Requirement != null) ? 
                                                                 (row.Requirement.ProjectFeature!=null)?  row.Requirement.ProjectFeature.ProjectFeature_Project
                                                                     : (row.Requirement.ProjectIncident!=null)? row.Requirement.ProjectIncident.ProjectIncident_Project
                                                                         :row.Requirement.ProjectTask.ProjectTask_Project  
                                                             :
                                                              (row.TimeTrack_Issue!=null)? 
                                                                    ( row.Issue.ProjectFeature!= null)? row.Issue.ProjectFeature.ProjectFeature_Project
                                                                      :(row.Issue.ProjectIncident!=null)? row.Issue.ProjectIncident.ProjectIncident_Project
                                                                         :row.Issue.ProjectTask.ProjectTask_Project: 0 ,

                                             
                                                 StartTime = row.StartTime,
                                                 EndTime = row.EndTime,
                                                 Chargeable = row.Billable
                                             };

         
           //All projects worked on for the given period
            var projects = (from tt in timeTrackingsByProject
                            join pr in projs on tt.ProjectId equals pr.Key
                            select new { pr.Key, pr.Value })
                           .GroupBy(p => p.Key, p => p.Value, (key, g) => new { Key = key, Value = g.FirstOrDefault() })
                           .OrderBy(p => p.Key).Concat(exes);

            List<ProjectTimeLine> data = new List<ProjectTimeLine>();

            int recId = 0;
            foreach (var project in projects)
            {
                DateTime index = fromDate;
                ProjectTimeLine ptl = new ProjectTimeLine();
                ptl.RecordId = recId;
                ptl.Project = project.Value == "PTO" ? "JS - Paid Time Off (PTO)" : project.Value;

                if (exceptions.Any(e => e.TimeForecastExceptions_Project == project.Key) &&
                        this.Context.Projects.Any(p => p.ProjectName == project.Value && p.ForecastException))
                {
                    bool hasData = false;
                    ptl.Billable = "Non-Bil";
                    for (int i = 0; i < days; i++)
                    {
                        decimal exceptionHours = this.GetExceptionHours((int)employeeId, index, index, exceptions.Where(e => e.TimeForecastExceptions_Project == project.Key).ToList());
                        hasData = exceptionHours > 0 || hasData;
                        
                        var roundedDayHours = Math.Ceiling(exceptionHours * 2) / (decimal)2;
                        ptl.GetType().GetProperty("DayHours" + i).SetValue(ptl, roundedDayHours, null);
                        ptl.TotalHours += roundedDayHours;

                        index = index.AddDays(1);
                    }
                    if (hasData)
                    {
                        data.Add(ptl);
                        recId++;
                    }
                }
                else
                {
                    index = fromDate;
                    ptl = new ProjectTimeLine();
                    ptl.RecordId = recId;
                    ptl.Project = project.Value;

                    //Billable
                    ptl.Billable = "Billable";
                    bool hasData = false;
                    for (int i = 0; i < days; i++)
                    {
                        decimal dayHours = 0;

                        var dayTimeTrackings = timeTrackingsByProject.Where(tt => tt.StartTime.Date == index || tt.EndTime.Date == index);
                        dayHours = (decimal)dayTimeTrackings.Where(tt => tt.ProjectId == project.Key && tt.Chargeable == "Billable").Sum(tt => (tt.EndTime - tt.StartTime).TotalHours);
                        hasData = dayHours > 0 || hasData;
                        
                        var roundedDayHours = Math.Ceiling(dayHours *2) / (decimal)2;
                        ptl.GetType().GetProperty("DayHours" + i).SetValue(ptl, roundedDayHours, null);
                        ptl.TotalHours += roundedDayHours;

                        index = index.AddDays(1);
                    }
                    if (hasData)
                    {
                        data.Add(ptl);
                        recId++;
                    }

                    index = fromDate;
                    ptl = new ProjectTimeLine();
                    ptl.RecordId = recId;
                    ptl.Project = project.Value;
                    //Non-Billable
                    ptl.Billable = "Non-Bil";

                    hasData = false;
                    for (int i = 0; i < days; i++)
                    {
                        decimal dayHours = 0;

                        var dayTimeTrackings = timeTrackingsByProject.Where(tt => tt.StartTime.Date == index || tt.EndTime.Date == index);
                        dayHours = (decimal)dayTimeTrackings.Where(tt => tt.ProjectId == project.Key && tt.Chargeable == "Non-Billable").Sum(tt => (tt.EndTime - tt.StartTime).TotalHours);
                        hasData = dayHours > 0 || hasData;
                        var roundedDayHours = Math.Round(dayHours * 2) /(decimal) 2;
                        ptl.GetType().GetProperty("DayHours" + i).SetValue(ptl, roundedDayHours, null);
                        ptl.TotalHours += roundedDayHours;

                        index = index.AddDays(1);
                    }
                    if (hasData)
                    {
                        data.Add(ptl);
                        recId++;
                    }
                }

                
            }

            //TODO: check this validation...
            Country c = this.Context.Employees.Include("Country").FirstOrDefault(e => e.Id == employeeId).Country;
            if (this.Context.CountryHolidays.Any(ch => ch.CountryHoliday_Country == c.Id && ch.StartDate >= fromDate))
            {
                bool hasData = false;
                ProjectTimeLine tempPtl = new ProjectTimeLine();
                DateTime index = fromDate;
                tempPtl.RecordId = recId;
                tempPtl.Project = "JS - Holiday";
                tempPtl.Billable = "Non-Bil";
                for (int i = 0; i < days; i++)
                {
                    decimal holidayHours = (decimal)(8.0 * this.GetHolidayCount(index, index, c, this.Context.CountryHolidays.Where(ch => ch.CountryHoliday_Country == c.Id).ToList()));
                    hasData = holidayHours > 0 || hasData;
                    var roundedDayHours = Math.Round(holidayHours *2) / (decimal)2.0;

                    //Reflection
                    //Instead of using a switch for each day in the loop
                    // we just call with reflection each property
                    tempPtl.GetType().GetProperty("DayHours" + i).SetValue(tempPtl, roundedDayHours, null);
                    tempPtl.TotalHours += roundedDayHours;
                    index = index.AddDays(1);
                }
                if(hasData) 
                {
                    data.Add(tempPtl);
                    recId++;
                }
            }

            // add row for leftover hours
            bool dataExists = false;
            ProjectTimeLine leftPtl = new ProjectTimeLine();
            DateTime dateIndex = fromDate;
            leftPtl.Billable = "Non-Bil";
            leftPtl.RecordId = recId;
            leftPtl.Project = "JS - Consulting Internal: El Salvador";

            for (int i = 0; i < days; i++)
            {
                decimal val = data.Sum(t => (decimal)t.GetType().GetProperty("DayHours" + i).GetValue(t, null));
                var roundedDayHours = Math.Round(val * 2) / (decimal) 2;
                var leftOver = 8 - roundedDayHours;
                if (leftOver > 0 && dateIndex.DayOfWeek != DayOfWeek.Saturday && dateIndex.DayOfWeek != DayOfWeek.Sunday)
                {
                    dataExists = leftOver > 0 || dataExists;
                    leftPtl.GetType().GetProperty("DayHours" + i).SetValue(leftPtl, leftOver, null);
                    leftPtl.TotalHours += leftOver;
                }
                dateIndex = dateIndex.AddDays(1);
            }
            if (dataExists)
            {
                data.Add(leftPtl);
                recId++;
            }

            return data.AsQueryable();
        }
    }
}
