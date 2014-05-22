using LightSwitchApplication.Implementation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Text;

namespace ProjectStatus_RIA_Service
{
    public partial class ProjectStatus_RIA_Service : DomainService
    {
        struct ProjectTitleRate
        {
            public int ProjectId;
            public string EmployeeTitle;
            public decimal Rate;
        }

        //GG added==============================================================================================================
        struct ProjectLevelRate
        {
            public int ProjectId;
            public int  EmployeeLevel;
            public decimal LevelRate;
        }
        struct EmployeeAssignments
        {
            public int ProjectId;
            public int EmployeeId;
            public int LevelId;
        }

        struct ProjectRevenue
        {
            public Project Project;
            public decimal ChargeableHours;
            public decimal Rate;
            public decimal Revenue;
        }

        [Query(IsDefault = true)]
        public IQueryable<RevenueForecast> GetEmptyRevenueForecasts()
        {
            return new List<RevenueForecast>().AsQueryable();
        }

        [Query()]
        public IQueryable<RevenueForecast> GetRevenueForecasts(decimal? expPerc, int? countryId, string client, int? quarter, int? year) 
        {
            
            //Set default values for parameters
            decimal defaultExpensePercentage = 0.12M;
            int defaultCountry = 3;
            int countryFilter = (countryId == null) ? defaultCountry : (int)countryId;
            decimal expensePercentage = (expPerc == null) ? defaultExpensePercentage : (decimal)(expPerc / 100);
            string clientFilter = (string.IsNullOrWhiteSpace(client)) ? String.Empty : client ;
            int quarterFilter = (quarter == null) ? TimePeriod.GetQuarter(DateTime.Now) : (int)quarter;
            int yearFilter = (year == null) ? DateTime.Now.Year : (int)year;

            //Number of periods to show
            const int MAX_PERIODS = 6;

            //Get the current quarter
            //int currentQuarter = TimePeriod.GetQuarter(DateTime.Today);
            int currentQuarter = quarterFilter;

            //"From" and "to" dates to get all forecasts into memory with one single operation 
            //DateTime globalStart = TimePeriod.PeriodStartDate(DateTime.Today);
            DateTime globalStart = new DateTime(yearFilter, ((currentQuarter - 1) * 3) + 1, 1);
            DateTime globalEnd = (currentQuarter == 4) ? 
                                            new DateTime(yearFilter, ((currentQuarter) * 3), 31):
                                            new DateTime(yearFilter, ((currentQuarter) * 3) + 1, 1).AddDays(-1);

            //Join only with chargeable projects
            //
            var projectList = this.Context.Projects.Where(p => p.Chargeable == true 
                                                            && p.Client.IsInternal == false)
                                                    .Select(p => new { p.Id, p.ProjectName, p.Client.Account, p.Client.Name })
                                                    .ToList();

            //Filter by client if necessary
            projectList = (string.IsNullOrWhiteSpace(clientFilter)) ? 
                                                    projectList : 
                                                    projectList.Where(p => p.Name == clientFilter)
                                                        .Select(p => new { p.Id, p.ProjectName, p.Account, p.Name })
                                                        .ToList(); 

            List<RevenueForecast> revs = new List<RevenueForecast>();

            Configuration defaultRateConf = this.Context.Configurations.FirstOrDefault(x => x.Key == "DefaultRate");
            decimal defaultRate = (defaultRateConf == null) ? 0 : Decimal.Parse(defaultRateConf.Value);
            var exceptionList = this.Context.TimeForecastExceptionsSet.ToList();
            

            //Setting the report by Title
            List<ProjectTitleRate> titleRateView = new List<ProjectTitleRate>();
            foreach (var rate in this.Context.ProjectRates.Include("EmployeeTitle").Include("Project").ToList())
            {
                titleRateView.Add(new ProjectTitleRate() {
                    ProjectId = rate.Project.Id,
                    EmployeeTitle = rate.EmployeeTitle.Name,
                    Rate = rate.Rate
                });
            }

            //GG Added ==============================================================================================================
            List<ProjectLevelRate> levelRateView = new List<ProjectLevelRate>();
            foreach (var rate in this.Context.ProjectRates.Include("EmployeeLevel").Include("Project").ToList())
            {
                levelRateView.Add(new ProjectLevelRate()
                   {
                       ProjectId = rate.Project.Id,
                       EmployeeLevel = rate.EmployeeLevel.Id,
                       LevelRate = rate.Rate
                   });
            }

            //Get all the chargeable forecasts in the period
            var periodForecasts = (countryFilter == 0) ? 
                                (from forecast in this.Context.TimeForecasts
                                   join employee in this.Context.Employees.Include("Country")
                                   on forecast.Employee.Id equals employee.Id
                                   where
                                          forecast.EndDate >= globalStart
                                       && forecast.StartDate <= globalEnd
                                       && forecast.Chargeable == true
                                       
                                       select new { forecast, project = forecast.Project, employee }
                                       ).ToList() :

                                 (from forecast in this.Context.TimeForecasts
                                  join employee in this.Context.Employees.Include("Country")
                                  on forecast.Employee.Id equals employee.Id
                                  where
                                         forecast.EndDate >= globalStart
                                      && forecast.StartDate <= globalEnd
                                      && forecast.Chargeable == true
                                      && employee.Country.Id == countryFilter
                                  select new { forecast, project = forecast.Project, employee }
                                       ).ToList() ;

            int recId = 0;

            decimal periodRevenue1 = 0;
            decimal periodRevenue2 = 0;
            decimal periodRevenue3 = 0;
            decimal periodRevenue4 = 0;
            decimal periodRevenue5 = 0;
            decimal periodRevenue6 = 0;

            decimal expenseEstimate1 = 0;
            decimal monthRevenue1 = 0;
            decimal expenseEstimate2 = 0;
            decimal monthRevenue2 = 0;
            decimal expenseEstimate3 = 0;
            decimal monthRevenue3 = 0;

            decimal quarterRevenue = 0;

            List<Country> countryList = this.Context.Countries.ToList();
            
            foreach (var prj in projectList) 
            {
                RevenueForecast revForecast = new RevenueForecast();

                DateTime localPeriodStart = globalStart;
                DateTime localPeriodEnd = new DateTime();

                List<ProjectRevenue> revenueList = new List<ProjectRevenue>();
                for (int i = 0; i < MAX_PERIODS; i++)
                {

                    localPeriodEnd = TimePeriod.PeriodEndDate(localPeriodStart);

                    
                    //Here we get a "view" in the next representation:
                    //{ CountryId, HolidayHours }
                    //Used to easily retrieve the holiday hours for each country for the current period in the loop

                    Dictionary<int, decimal> countryHolidayHoursView = new Dictionary<int, decimal>();
                    foreach (var country in countryList)
                    {
                        decimal countryHolidayHours = this.GetHolidayCount(localPeriodStart, localPeriodEnd, country) * 8;
                        countryHolidayHoursView.Add(country.Id, countryHolidayHours);
                    }
                    revenueList.Clear();

                    //Get the period forecasts for the current project in the loop
                    // and that are contained in the current loop period

                    //The variables created with "let" are used to limit 
                    // the forecast to the current loop period
                    var projectForecasts = from pf in periodForecasts
                               let startDate = (pf.forecast.StartDate > localPeriodStart) ? pf.forecast.StartDate : localPeriodStart
                               let endDate = (pf.forecast.EndDate > localPeriodEnd) ? localPeriodEnd : pf.forecast.EndDate
                               where pf.project.Id == prj.Id && pf.forecast.EndDate >= localPeriodStart
                                       && pf.forecast.StartDate <= localPeriodEnd
                               select new { startDate = startDate, endDate = endDate, forecast = pf.forecast, employee = pf.employee, project = pf.project };

                    //For each periodForecast we do the calculations for worked hours, etc
                    foreach (var periodForecast in projectForecasts) 
                    {
                        decimal periodHours = this.GetPeriodHours(periodForecast.startDate, periodForecast.endDate);
                        //var definedRate = titleRateView.FirstOrDefault(x => x.EmployeeTitle.Contains(periodForecast.employee.Title.ToLowerInvariant()) && x.ProjectId == periodForecast.forecast.Project.Id);
                        
                        //GG added==============================================================================================================
                      var definedRate = titleRateView.FirstOrDefault(x => x.EmployeeTitle == periodForecast.employee.Title && x.ProjectId == periodForecast.forecast.Project.Id);
                        //var definedRate= levelRateView.FirstOrDefault(x=> x.EmployeeLevel== periodForecast.employee.EmployeeAssignments.
                        var rate = (definedRate.EmployeeTitle == null) ? defaultRate : definedRate.Rate;

                        var exceptionHours = (int)this.GetExceptionHours(periodForecast.employee.Id,localPeriodStart, localPeriodEnd,exceptionList );
                        var holidayHours = (int)countryHolidayHoursView.FirstOrDefault(x => x.Key == periodForecast.employee.Country.Id).Value;
                        var workHours = (int)(periodHours - exceptionHours - holidayHours);
                        var chargeableHours = (decimal) (workHours * (periodForecast.forecast.Percent / 100.00));

                        revenueList.Add(new ProjectRevenue()
                        {
                            Project = periodForecast.forecast.Project,
                            ChargeableHours = chargeableHours,
                            Rate = rate,
                            Revenue = (chargeableHours * rate) > 0 ? (chargeableHours * rate) : 0
                        });
                    }
                    
                    // Group the revenues by project
                    //  and sum the total
                    var revenueGroups = (from period in revenueList
                                         group period by period.Project.Id into g
                                         select new { ProjectId = g.Key, Revenue = g.Sum(x => x.Revenue) }
                                        ).ToList();

                    //var projectRevenue = revenueGroups.FirstOrDefault(x => x.ProjectId == prj.Id);

                    var projectRevenue = new { Revenue = revenueList.Where(r => r.Project.Id == prj.Id).Sum(r => r.Revenue) };

                    switch (i) 
                    {
                        case 0:
                            periodRevenue1 = (projectRevenue == null) ? 0 : projectRevenue.Revenue;
                            revForecast.PeriodRevenue1 = periodRevenue1.ToString("C2");
                            break;
                        case 1:
                            periodRevenue2 = (projectRevenue == null) ? 0 : projectRevenue.Revenue;
                            revForecast.PeriodRevenue2 = periodRevenue2.ToString("C2");
                            break;
                        case 2:
                            periodRevenue3 = (projectRevenue == null) ? 0 : projectRevenue.Revenue;
                            revForecast.PeriodRevenue3 = periodRevenue3.ToString("C2");
                            break;
                        case 3:
                            periodRevenue4 = (projectRevenue == null) ? 0 : projectRevenue.Revenue;
                            revForecast.PeriodRevenue4 = periodRevenue4.ToString("C2");
                            break;
                        case 4:
                            periodRevenue5 = (projectRevenue == null) ? 0 : projectRevenue.Revenue;
                            revForecast.PeriodRevenue5 = periodRevenue5.ToString("C2");
                            break;
                        case 5:
                            periodRevenue6 = (projectRevenue == null) ? 0 : projectRevenue.Revenue;
                            revForecast.PeriodRevenue6 = periodRevenue6.ToString("C2");
                            break;
                    }

                    expenseEstimate1 = (periodRevenue1 + periodRevenue2) * (decimal)expensePercentage;
                    revForecast.ExpenseEstimate1 = expenseEstimate1.ToString("C2");
                    monthRevenue1 = periodRevenue1 + periodRevenue2 + expenseEstimate1;
                    revForecast.MonthRevenue1 = monthRevenue1.ToString("C2");

                    expenseEstimate2 = (periodRevenue3 + periodRevenue4) * (decimal)expensePercentage;
                    revForecast.ExpenseEstimate2 = expenseEstimate2.ToString("C2");
                    monthRevenue2 = periodRevenue3 + periodRevenue4 + expenseEstimate2;
                    revForecast.MonthRevenue2 = monthRevenue2.ToString("C2");

                    expenseEstimate3 = (periodRevenue5 + periodRevenue6) * (decimal)expensePercentage;
                    revForecast.ExpenseEstimate3 = expenseEstimate3.ToString("C2");
                    monthRevenue3 = periodRevenue5 + periodRevenue6 + expenseEstimate3;
                    revForecast.MonthRevenue3 = monthRevenue3.ToString("C2");

                    quarterRevenue = monthRevenue1 + monthRevenue2 + monthRevenue3;
                    revForecast.QuarterRevenue = quarterRevenue.ToString("C2");

                    //Go to the next period
                    localPeriodStart = localPeriodEnd.AddDays(1);

                }
                 
                revForecast.RecordId = recId;

                revForecast.Client = prj.Name;
                revForecast.Account = prj.Account;
                revForecast.Project = prj.ProjectName;
                
                revs.Add(revForecast);
                recId++;
            }

            //Totals
            var totalsRow = new RevenueForecast()
            {
                RecordId = recId,
                Client = "TOTALS",
                Project = "",
                Account = "",
                PeriodRevenue1 = revs.Sum(r => decimal.Parse(r.PeriodRevenue1, NumberStyles.Currency)).ToString("C2"),
                PeriodRevenue2 = revs.Sum(r => decimal.Parse(r.PeriodRevenue2, NumberStyles.Currency)).ToString("C2"),
                PeriodRevenue3 = revs.Sum(r => decimal.Parse(r.PeriodRevenue3, NumberStyles.Currency)).ToString("C2"),
                PeriodRevenue4 = revs.Sum(r => decimal.Parse(r.PeriodRevenue4, NumberStyles.Currency)).ToString("C2"),
                PeriodRevenue5 = revs.Sum(r => decimal.Parse(r.PeriodRevenue5, NumberStyles.Currency)).ToString("C2"),
                PeriodRevenue6 = revs.Sum(r => decimal.Parse(r.PeriodRevenue6, NumberStyles.Currency)).ToString("C2"),
                ExpenseEstimate1 = revs.Sum(r => decimal.Parse(r.ExpenseEstimate1, NumberStyles.Currency)).ToString("C2"),
                ExpenseEstimate2 = revs.Sum(r => decimal.Parse(r.ExpenseEstimate2, NumberStyles.Currency)).ToString("C2"),
                ExpenseEstimate3 = revs.Sum(r => decimal.Parse(r.ExpenseEstimate3, NumberStyles.Currency)).ToString("C2"),
                MonthRevenue1 = revs.Sum(r => decimal.Parse(r.MonthRevenue1, NumberStyles.Currency)).ToString("C2"),
                MonthRevenue2 = revs.Sum(r => decimal.Parse(r.MonthRevenue2, NumberStyles.Currency)).ToString("C2"),
                MonthRevenue3 = revs.Sum(r => decimal.Parse(r.MonthRevenue3, NumberStyles.Currency)).ToString("C2"),
                QuarterRevenue = revs.Sum(r => decimal.Parse(r.QuarterRevenue, NumberStyles.Currency)).ToString("C2")
            };

            revs = revs.OrderBy(p => p.Client).ThenBy(p => p.Project).ToList();
            revs.Insert(0,totalsRow);

            return revs.AsQueryable();
        }

    }
}
