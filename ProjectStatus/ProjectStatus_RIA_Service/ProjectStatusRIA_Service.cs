
namespace ProjectStatus_RIA_Service
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using LightSwitchApplication.Implementation;
    using System.Data.EntityClient;
    using System.Web.Configuration;

    //Entities
    public class ProjectTimeLine
    {
        [Key]
        public int RecordId {get; set;}
        public string Project { get; set; }
        public string Billable { get; set; }
        public decimal DayHours0 { get; set; }
        public decimal DayHours1 { get; set; }
        public decimal DayHours2 { get; set; }
        public decimal DayHours3 { get; set; }
        public decimal DayHours4 { get; set; }
        public decimal DayHours5 { get; set; }
        public decimal DayHours6 { get; set; }
        public decimal DayHours7 { get; set; }
        public decimal DayHours8 { get; set; }
        public decimal DayHours9 { get; set; }
        public decimal DayHours10 { get; set; }
        public decimal DayHours11 { get; set; }
        public decimal DayHours12 { get; set; }
        public decimal DayHours13 { get; set; }
        public decimal DayHours14 { get; set; }
        public decimal DayHours15 { get; set; }
        public decimal TotalHours { get; set; }

    }

    public class DODFeatureChartData
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public string FeatureStatus { get; set; }
        public int FeatureCount { get; set; }
    }
    public class DODIncidentChartData
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public string IncidentStatus { get; set; }
        public int IncidentCount { get; set; }
    }
    public class DODProjectStatusChartData
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public string ProjectStatus { get; set; }
        public decimal ProjectCount { get; set; }
    }

    public class ForecastExcepRecord
    {
        [Key]
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Client { get; set; }
        public bool ForecastException { get; set; }
        public bool Inactive { get; set; }
    }

    public class UnassignedEmployees
    {
        [Key]
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserID { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
    }

    public class StatusSummary
    {
        [Key]
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Decimal AssignedFTE { get; set; }
        public Decimal RequiredFTE { get; set; }
        public string Client { get; set; }
        public string Phase { get; set; }
        public double ProjectStatus { get; set; }
        public double StaffingStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime BookedEndDate { get; set; }
        public DateTime ForecastEndDate { get; set; }
        public bool Inactive { get; set; }
        public bool Chargeable { get; set; }
        public bool ForecastException { get; set; }
        public string FollowOn { get; set; }
        public string Notes { get; set; }
        public string Director { get; set; }
        public string Manager { get; set; }
        public string Region { get; set; }
        public bool Planned { get; set; }
    }

    public class Forecast
    {
        [Key]
        public int RecID { get; set; } //add a unique record ID key to allow the return of multiple time period forecasts for each employee
        public int EmployeeID { get; set; }
        public int TimePeriod { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int WeekDays { get; set; }
        public int WorkHours { get; set; }
        public int PeriodHours { get; set; }
        public int HolidayHours { get; set; }
        public decimal ChargeableHours { get; set; }
        public decimal NonChargeableHours { get; set; }
        public decimal ExceptionHours { get; set; }
        public decimal Utilization { get; set; }
        public decimal Availability { get; set; }
        public string UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public class MultiPeriodForecast
    {
        [Key]
        public int RecID { get; set; } //add a unique record ID key to allow the return of multiple time period forecasts for each employee
        public int EmployeeID { get; set; }
        public int TimePeriod { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int WeekDays { get; set; }
        public int WorkHours { get; set; }
        public int PeriodHours { get; set; }
        public int HolidayHours { get; set; }
        public decimal ChargeableHours { get; set; }
        public decimal NonChargeableHours { get; set; }
        public decimal ExceptionHours { get; set; }
        public decimal Utilization { get; set; }
        public decimal Availability { get; set; }
        public decimal Utilization1 { get; set; }
        public decimal Availability1 { get; set; }
        public decimal Utilization2 { get; set; }
        public decimal Availability2 { get; set; }
        public decimal Utilization3 { get; set; }
        public decimal Availability3 { get; set; }
        public decimal Utilization4 { get; set; }
        public decimal Availability4 { get; set; }
        public decimal Utilization5 { get; set; }
        public decimal Availability5 { get; set; }
        public string UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public class TimeForecastHrs
    {
        [Key]
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Percent { get; set; }
        public decimal Period1ChargeableHrs { get; set; }
        public decimal Period1NonChargeableHrs { get; set; }
        public decimal Period1ExceptionHrs { get; set; }
        public decimal Period1AvailableHrs { get; set; }
    }

    public class TimeForecastRecords
    {
        [Key]
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Percent { get; set; }
        public bool Chargeable { get; set; }
        //public bool Exception { get; set; }
    }

    public class EmployeeAssignmentReport
    {
        [Key]
        public int RecId { get; set; }
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Percent { get; set; }
        public bool Chargeable { get; set; }
        public bool Exception { get; set; }
        public string Client { get; set; }
        public string Department { get; set; }
        public DateTime BookedEndDate { get; set; }
        public DateTime ForecastEndDate { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public bool Rolloff { get; set; }
        public string Title { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public class AnnualReviewStatus
    {
        [Key]
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserID { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Supervisor { get; set; }
        public string ReviewStatus { get; set; }
        public string Level { get; set; } //GG added
        public string SupervisorFullNameSorting { get; set; } //GG added
        public DateTime ReviewDate { get; set; } //GG added -- changed to be string instead of datetime
        public int YearReview { get; set; } //GG added

    }
    //GG Added
    public class ReviewSummaryReportJS
    {
        [Key]
        public int EmployeeID { get; set; }
        public string Alias { get; set; }
        public string Office { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserID { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string ReviewStatus { get; set; }
        public string Level { get; set; } 
        public string SupervisorFullNameSorting { get; set; } 
        //gg added
        public string Reviewer{ get; set; }
       // public DateTime ReviewerDate { get; set; }
        //public DateTime StartDate { get; set; }
       // public DateTime EndDate { get; set; }
        public string ReviewType { get; set; }
        public string Supervisor { get; set; }
        public string ProjectsCovered { get; set; }
        public string Role { get; set; }
       // public Boolean Promote { get; set; }
        public string Strengths{ get; set; }
        public string DevelopmentNeeds { get; set; }
        public int SummaryRating { get; set; }
        public string MajorAssignments { get; set; }
       // public Boolean EmployeeHasSigned { get; set; }
        public string SignedStatus { get; set; }
       // public Boolean Published { get; set; }
    }


    public class RevenueForecast
    {
        [Key]
        public int RecordId { get; set; }

        public string Client { get; set; }
        public string Account { get; set; }
        public string Project { get; set; }

        public string PeriodRevenue1 { get; set; }
        public string PeriodRevenue2 { get; set; }
        public string ExpenseEstimate1 { get; set; }
        public string MonthRevenue1 { get; set; }

        public string PeriodRevenue3 { get; set; }
        public string PeriodRevenue4 { get; set; }
        public string ExpenseEstimate2 { get; set; }
        public string MonthRevenue2 { get; set; }

        public string PeriodRevenue5 { get; set; }
        public string PeriodRevenue6 { get; set; }
        public string ExpenseEstimate3 { get; set; }
        public string MonthRevenue3 { get; set; }

        public string QuarterRevenue { get; set; }
    }

    public class PTORecord 
    {
        [Key]
        public int RecordId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal PeriodPTOHours0 { get; set; }
        public decimal PeriodPTOHours1 { get; set; }
        public decimal PeriodPTOHours2 { get; set; }
        public decimal PeriodPTOHours3 { get; set; }
        public decimal PeriodPTOHours4 { get; set; }
        public decimal PeriodPTOHours5 { get; set; }
        public decimal PeriodPTOHours6 { get; set; }
        public decimal PeriodPTOHours7 { get; set; }
        public decimal PeriodPTOHours8 { get; set; }
        public decimal PeriodPTOHours9 { get; set; }
        public decimal PeriodPTOHours10 { get; set; }
        public decimal PeriodPTOHours11 { get; set; }
    }


    //Partial class to distribute methods in different files but in the same class
    //All method implementations are under the Implementation folder
    public partial class ProjectStatus_RIA_Service : DomainService
    {

        #region Database connection

        // This code dynamically creates a connection to the database. LightSwitch uses “_IntrinsicData” as it’s connection string. 
        // This code looks for that connection string and uses it for the WCF RIA Service.
        private ApplicationData m_context;
        public ApplicationData Context
        {
            get
            {
                if (this.m_context == null)
                {
                    string connString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["_IntrinsicData"].ConnectionString;
                    EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder();
                    builder.Metadata = "res://*/ApplicationData.csdl|res://*/ApplicationData.ssdl|res://*/ApplicationData.msl";
                    builder.Provider = "System.Data.SqlClient";
                    builder.ProviderConnectionString = connString;

                    this.m_context = new ApplicationData(builder.ConnectionString);

                }
                return this.m_context;
            }
        }
        #endregion

    }

}


